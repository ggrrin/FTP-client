using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTPClientGUI
{
	/// <summary>
	/// Represents controller for action associated with local "place" listview, files, etc.
	/// </summary>
	class LocalControl : IDisposable
	{

		private ListView listView;
		private Label pathLabel;
		private Image folderIcon;
		private int folderIconIndex;

		private DirectoryInfo prevDir;
		private DirectoryInfo localDirectory;

		/// <summary>
		/// Current working local directory 
		/// </summary>
		public DirectoryInfo LocalDirectory
		{
			get { return localDirectory; }
			set { prevDir = localDirectory; localDirectory = value; }
		}

		/// <summary>
		/// Gets current directory watcher
		/// </summary>
		public FileSystemWatcher DirectoryWatcher { get; private set; }

		/// <summary>
		/// InitializeLocal controller 
		/// </summary>
		/// <param name="i">local working directory</param>
		/// <param name="v">local listview</param>
		/// <param name="pathLabel">local path label</param>
		public LocalControl(DirectoryInfo i, ListView v, Label pathLabel)
		{
			this.pathLabel = pathLabel;
			LocalDirectory = i;
			listView = v;

			listView.ListViewItemSorter = new FileSorter((l) => l.Tag == null || l.Tag is DirectoryInfo);

			folderIcon = Bitmap.FromFile("folderopened_yellow.png");

			listView.Columns.Add("Name", -2, HorizontalAlignment.Left);
			listView.Columns.Add("Size", -2, HorizontalAlignment.Left);
			listView.Columns.Add("Changed", -2, HorizontalAlignment.Left);

			InitializeWatcher();			

			listView.DoubleClick += listView_DoubleClick;
			pathLabel.Click += pathLabel_Click;
		}

		async void listView_DoubleClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count > 0)
			{
				var item = listView.SelectedItems[0];

				string path = Path.Combine(LocalDirectory.FullName, item.Text);
				var atr = File.GetAttributes(path);

				if (atr.HasFlag(FileAttributes.Directory))
				{
					LocalDirectory = new DirectoryInfo(path);
					await UpdateListViewAsync();
				}
			}
		}

		async void pathLabel_Click(object sender, EventArgs e)
		{
			var fbd = new FolderBrowserDialog();
			fbd.SelectedPath = LocalDirectory.FullName;

			if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				LocalDirectory = new DirectoryInfo(fbd.SelectedPath);
				await UpdateListViewAsync();
			}
		}


		/// <summary>
		/// Gets directory async, configure awaiter false
		/// </summary>
		/// <returns>returns list of subdirectories of current directory</returns>
		private async Task<DirectoryInfo[]> GetDirectoriesAsync()
		{
			return await Task.Run(() => LocalDirectory.GetDirectories()).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets files async, configure awaiter false
		/// </summary>
		/// <returns>returns list of files in current directory</returns>
		private async Task<FileInfo[]> GetFilesAsync()
		{
			return await Task.Run(() => LocalDirectory.GetFiles()).ConfigureAwait(false);
		}


		/// <summary>
		/// Loads files icons async, configure awaiter false
		/// </summary>
		/// <param name="files">Files to get icon of</param>
		/// <returns>returns Image list of files icons in same order as provided file list</returns>
		private async Task<ImageList> LoadIconsAsync(FileInfo[] files)
		{
			return await Task.Run(() =>
			{
				var list = new ImageList();

				foreach (var i in files)
					list.Images.Add(Icon.ExtractAssociatedIcon(i.FullName));

				return list;
			}).ConfigureAwait(false);
		}

		/// <summary>
		/// Update local listview asynchronously
		/// </summary>
		/// <returns>task</returns>
		public async Task UpdateListViewAsync()
		{
			pathLabel.Text = LocalDirectory.FullName;

			listView.Items.Clear();
			listView.LabelEdit = false;
			listView.Items.Add(new ListViewItem("Loading..."));
			listView.Refresh();

			////////////////////////////////////////////////////
			DirectoryInfo[] dirs = null;
			FileInfo[] files = null;
			ImageList icons = null;

			bool update = false;

			try
			{
				dirs = await GetDirectoriesAsync();
				files = await GetFilesAsync();
				icons = await LoadIconsAsync(files);
			}
			catch (Exception e)
			{
				if (e is FileNotFoundException || e is SecurityException || e is UnauthorizedAccessException)
				{
					MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					//reset to previous directory
					LocalDirectory = prevDir;
					update = true;
					return;
				}
			}

			if (update)
				await UpdateListViewAsync();

			folderIconIndex = icons.Images.Count;

			listView.Items.Clear();
			listView.LabelEdit = true;
			listView.Items.Add(new ListViewItem("..", folderIconIndex));

			icons.Images.Add(folderIcon);
			listView.SmallImageList = listView.LargeImageList = icons;

			foreach (var i in dirs)
				AddDirectory(i, folderIconIndex);

			int k = 0;
			foreach (var i in files)
				AddFile(i, k++);

			listView.Refresh();

			DirectoryWatcher.Path = LocalDirectory.FullName;
			DirectoryWatcher.EnableRaisingEvents = true;
		}

		/// <summary>
		/// Add directory to the listview  and sets tag to appropriate directory info
		/// </summary>
		/// <param name="i">Directory info of directory to add to listview</param>
		/// <param name="folderIconIndex">index of icon in listview  of directory</param>
		private void AddDirectory(DirectoryInfo i, int folderIconIndex)
		{
			var dirItem = new ListViewItem(i.Name, folderIconIndex);
			dirItem.SubItems.Add("");
			dirItem.SubItems.Add(i.LastWriteTime.ToString());
			dirItem.Tag = i;
			listView.Items.Add(dirItem);
		}

		/// <summary>
		/// Add files to thw listview and sets tak to appropriate file info
		/// </summary>
		/// <param name="i">file info of file</param>
		/// <param name="fileIconIndex">index to image list where file icon is stored</param>
		private void AddFile(FileInfo i, int fileIconIndex)
		{
			var fileItem = new ListViewItem(i.Name, fileIconIndex++);
			fileItem.SubItems.Add(BytesToString(i.Length));
			fileItem.SubItems.Add(i.LastWriteTime.ToString());
			fileItem.Tag = i;
			listView.Items.Add(fileItem);
		}


		/// <summary>
		/// Human readable string representation of size
		/// </summary>
		/// <param name="byteCount">size in bytes</param>
		/// <returns>Human readable string of size</returns>
		internal static String BytesToString(long byteCount)
		{
			string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
			if (byteCount == 0)
				return "0" + suf[0];
			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + suf[place];
		}

		/// <summary>
		/// Initialize directory watcher for current directory
		/// </summary>
		private void InitializeWatcher()
		{
			DirectoryWatcher = new FileSystemWatcher();
			DirectoryWatcher.Path = LocalDirectory.FullName;

			DirectoryWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;

			DirectoryWatcher.Filter = "";

			DirectoryWatcher.Changed += new FileSystemEventHandler(OnChanged);
			DirectoryWatcher.Created += new FileSystemEventHandler(OnChanged);
			DirectoryWatcher.Deleted += new FileSystemEventHandler(OnChanged);
			DirectoryWatcher.Renamed += new RenamedEventHandler(OnRenamed);
		}


		private void OnRenamed(object sender, RenamedEventArgs e)
		{
			listView.Invoke(new Action(() =>
				{
					UpdateListItem(e.OldFullPath, e.FullPath);
					listView.Refresh();
				}));
		}

		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			try
			{
				listView.Invoke(new Action(() =>
				{
					switch (e.ChangeType)
					{
						case WatcherChangeTypes.Created:
							FileAttributes atr;

							try
							{
								atr = File.GetAttributes(e.FullPath);
							}
							catch (InvalidOperationException)
							{
								return;
							}
							if (atr.HasFlag(FileAttributes.Directory))
							{
								AddDirectory(new DirectoryInfo(e.FullPath), folderIconIndex);
							}
							else
							{
								AddFile(new FileInfo(e.FullPath), listView.SmallImageList.Images.Count);
								listView.SmallImageList.Images.Add(Icon.ExtractAssociatedIcon(e.FullPath));
							}

							break;

						case WatcherChangeTypes.Deleted:
							DeleteListItem(e.FullPath);
							break;

						default:
							UpdateListItem(e.FullPath, e.FullPath);
							break;
					}

					listView.Refresh();
				}));
			}
			catch (InvalidOperationException)
			{ }
		}

		/// <summary>
		/// Determines whether paths are to same file/directory
		/// </summary>
		/// <param name="path1">first path</param>
		/// <param name="path2">second path</param>
		/// <returns>Returns true whether paths are equal, otherwise false</returns>
		private bool PathEqual(string path1, string path2)
		{
			return string.Compare(path1.Trim(new char[] { '\\' }), path2.Trim(new char[] { '\\' }), StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		/// <summary>
		/// Find item in listview according to given predicate
		/// </summary>
		/// <param name="cond">Condition for item match</param>
		/// <returns>Returns first item fulfilling the condition, or null if no such a file found</returns>
		private ListViewItem FindItem(Predicate<ListViewItem> cond)
		{
			foreach (var i in listView.Items)
				if (cond(i as ListViewItem))
					return (ListViewItem)i;
			return null;
		}


		/// <summary>
		/// Update item in listview associated with old path and updated to newpath
		/// </summary>
		/// <param name="oldPath">old path</param>
		/// <param name="newPath">new path</param>
		private void UpdateListItem(string oldPath, string newPath)
		{
			var item = FindItem((i) => i.Tag != null && PathEqual((i.Tag as FileSystemInfo).FullName, oldPath));

			if (item == null)
				return;


			if (item.Tag is DirectoryInfo)
			{
				var d = new DirectoryInfo(newPath);
				item.Text = d.Name;
				item.SubItems[1].Text = d.LastWriteTime.ToString();
				item.Tag = d;
			}
			else
			{
				var f = new FileInfo(newPath);
				item.Text = f.Name;
				item.SubItems[1].Text = BytesToString(f.Length);
				item.SubItems[2].Text = f.LastWriteTime.ToString();
				item.Tag = f;
			}
		}


		/// <summary>
		/// Deletes item associated with given path from listview
		/// </summary>
		/// <param name="path">path of the file</param>
		private void DeleteListItem(string path)
		{
			var item = FindItem((i) => i.Tag != null && PathEqual((i.Tag as FileSystemInfo).FullName, path));
			listView.Items.Remove(item);
		}

		/// <summary>
		/// Returns File infos associated with selected items in listview
		/// </summary>
		/// <returns>Returns File infos associated with selected items in listview</returns>
		internal IList<FileSystemInfo> GetSelectedFiles()
		{
			var list = new List<FileSystemInfo>();

			foreach (var i in listView.SelectedItems)
			{
				var item = (ListViewItem)i;
				if (item.Tag != null)
					list.Add((FileSystemInfo)item.Tag);
			}
			return list;
		}

		/// <summary>
		/// Dispose object
		/// </summary>
		public void Dispose()
		{
			if (DirectoryWatcher != null)
			{
				DirectoryWatcher.EnableRaisingEvents = false;
				DirectoryWatcher.Dispose();
			}

			listView.Clear();
			pathLabel.Text = "";

			listView.DoubleClick -= listView_DoubleClick;
			pathLabel.Click -= pathLabel_Click;

			listView = null;
			pathLabel = null;
		}
	}
}

using FTP_Library;
using FTP_Library.Exceptions;
using FTP_Library.Queries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTPClientGUI
{
	/// <summary>
	/// Represents controller for action associated with server side
	/// </summary>
	class ServerControl : IDisposable
	{
		private ListView listView;

		private Label pathLabel;

		const string directoryTag = "dir";

		/// <summary>
		/// Gets current FTP control
		/// </summary>
		public FTPControl FTPControl { get; private set; }

		private MainForm form;
		private const string fileTag = "file";

		/// <summary>
		/// Gets or sets Progress dialog for async methods to work with
		/// </summary>
		public ProgressDialog ProgressDialog { get; set; }

		/// <summary>
		/// Initialize controller
		/// </summary>
		/// <param name="c">FTPcontrol with established session</param>
		/// <param name="v">Listview of server working directory items.</param>
		/// <param name="pathLabel">Label for current working server path</param>
		/// <param name="m">main form</param>
		public ServerControl(FTPControl c, ListView v, Label pathLabel, MainForm m)
		{
			this.pathLabel = pathLabel;
			FTPControl = c;
			listView = v;
			form = m;

            InitOutputs();
			listView.ListViewItemSorter = new FileSorter((l) => l.Tag == null || l.Tag.ToString() == directoryTag);

			listView.Columns.Add("Name", -2, HorizontalAlignment.Left);
			listView.Columns.Add("Size", -2, HorizontalAlignment.Left);
			listView.Columns.Add("Changed", -2, HorizontalAlignment.Left);

			var imgl = new ImageList();
			imgl.Images.Add(Bitmap.FromFile("folderopened_yellow.png"));
			imgl.Images.Add(Bitmap.FromFile("document.png"));
			listView.SmallImageList = listView.LargeImageList = imgl;

			listView.DoubleClick += listView_DoubleClick;
			listView.AfterLabelEdit += listView_AfterLabelEdit;
			listView.BeforeLabelEdit += listView_BeforeLabelEdit;			
		}

        private void InitOutputs()
        {
            var g = Guid.NewGuid();
            FTPControl.ResponseOutput = new StreamWriter($"{g}.respones");
            FTPControl.QueryOutput = new StreamWriter($"{g}.queries");
        }

        #region directoryListing

        /// <summary>
        /// Disable up directory editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (listView.Items[e.Item].Text == "..")
				e.CancelEdit = true;
		}


		/// <summary>
		/// Enable directory changing by double click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		async void listView_DoubleClick(object sender, EventArgs e)
		{
			var list = listView.SelectedItems;
			if (list != null && list.Count > 0)
			{
				var item = list[0];
				if (item.Tag != null && item.Tag.ToString() == directoryTag)
					await ChangeDir(item.Text);
			}
		}


		/// <summary>
		/// Change server working directory and update appropriately listview
		/// </summary>
		/// <param name="directory">relative or absolute path to new directory</param>
		/// <returns>task</returns>
		private async Task ChangeDir(string directory)
		{
			listView.Items.Clear();
			listView.LabelEdit = false;
			listView.Items.Add(new ListViewItem("Loading..."));
			listView.Refresh();

			try
			{
				await FTPControl.ExecuteQueryAsync(new ChangeWorkingDirectoryQuery { Path = directory });
			}
			catch (ControlConnectionException e)
			{
				MessageBox.Show(e.Message + " You will be logged out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				form.Logout();
				return;
			}
			catch (FTPQueryException e)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			await UpdateListViewAsync();
		}


		/// <summary>
		/// Update directory list from server
		/// </summary>
		/// <returns>task</returns>
		public async Task UpdateListViewAsync()
		{
			listView.Items.Clear();
			listView.LabelEdit = false;
			listView.Items.Add(new ListViewItem("Loading..."));
			listView.Refresh();

			var list = await ListDirectoryAsync();

			if (list == null)
			{
				return;
			}

			pathLabel.Text = FTPControl.CurrentWorkingDir;

			listView.Items.Clear();
			listView.LabelEdit = true;

			var it = new ListViewItem("..", 0);
			it.Tag = directoryTag;
			listView.Items.Add(it);

			foreach (var i in list)
				listView.Items.Add(i);

			listView.Enabled = true;
			listView.Refresh();
		}


		/// <summary>
		/// Gets list of server current working directory items
		/// </summary>
		/// <returns>directory listing</returns>
		private async Task<ICollection<ListViewItem>> ListDirectoryAsync()
		{
			var q = new ListDirectoryQuery { HumanReadable = false };
			try
			{
				await FTPControl.ExecuteQueryAsync(q);
			}
			catch (ControlConnectionException e)
			{
				MessageBox.Show(e.Message + " You will be logged out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				form.Logout();
				return null;
			}
			catch (FTPQueryException e)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}


			return ParseDirectoryList(q.Reply);
		}


		/// <summary>
		/// Parse machine readable directory listing server reply form server 
		/// </summary>
		/// <param name="p">Reply from server</param>
		/// <returns>List of listview items</returns>
		private ICollection<ListViewItem> ParseDirectoryList(string p)
		{
			using (var r = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(p)), Encoding.ASCII))
			{
				var list = new List<ListViewItem>();

				string line = null;
				while ((line = r.ReadLine()) != null)
				{
					string[] segments = line.Split(new char[] { ';' });

					string type = GetValue(segments, "type");

					string name = segments[segments.Length - 1].Trim();
					ListViewItem item;
					switch (type)
					{
						case "cdir":
						case "pdir":
							continue;

						case directoryTag:
							item = new ListViewItem(name, 0);
							item.Tag = directoryTag;
							break;

						default:
							item = new ListViewItem(name, 1);
							item.Tag = fileTag;
							break;
					}

					item.SubItems.Add("");
					item.SubItems.Add(GetValue(segments, "modify"));
					list.Add(item);
				}

				return list;
			}
		}

		/// <summary>
		/// Parse value from by given filed from segments list
		/// </summary>
		/// <param name="segments">List of parameters</param>
		/// <param name="field">parameter to find value of</param>
		/// <returns></returns>
		private string GetValue(string[] segments, string field)
		{
			return Array.Find(segments, (s) => s.StartsWith(field)).Split(new char[] { '=' })[1];
		}

		#endregion

		/// <summary>
		/// Dispose controller. That include dispose of FTP controller listview and associated event handlers
		/// </summary>
		public void Dispose()
		{
			listView.Clear();
			pathLabel.Text = "";

            //try
            //{
            //	FTPControl.ExecuteQuery(new LogoutQuery());
            //}
            //catch (FTPQueryException)
            //{
            //}

            FTPControl.QueryOutput.Close();
            FTPControl.ResponseOutput.Close();

			FTPControl.Dispose();

			listView.DoubleClick -= listView_DoubleClick;
			listView.AfterLabelEdit -= listView_AfterLabelEdit;
			listView.BeforeLabelEdit -= listView_BeforeLabelEdit;

			listView = null;
			pathLabel = null;
			FTPControl = null;
		}

		/// <summary>
		/// Created new directory on server
		/// </summary>
		/// <returns>task</returns>
		public async Task CreateFolderAsync()
		{
			bool update = false;
			const string f = "New Folder";

			var list = new List<string>();
			for (int i = 0; i < listView.Items.Count; i++)
				if (listView.Items[i].Tag.ToString() == directoryTag)
					list.Add(listView.Items[i].Text);


			var directoryName = await GetUniqueFolderName(list, f);

			var item = new ListViewItem(directoryName, 0);
			item.Tag = directoryTag;
			listView.Items.Add(item);
			item.BeginEdit();
			try
			{
				await FTPControl.ExecuteQueryAsync(new DirectoryQuery { Path = directoryName });
			}
			catch (ControlConnectionException x)
			{
				MessageBox.Show(x.Message + " You will be logged out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				form.Logout();
				return;
			}
			catch (FTPQueryException ex)
			{
				update = true;
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if(update)
				await UpdateListViewAsync(); 
		}

		public async Task<string> GetUniqueFolderName(ICollection<string> usedList, string folderName)
		{
			if (usedList.Contains(folderName))
			{
				string f = folderName;
				await Task.Run(() =>
				{
					for (int i = 1; true; i++)
					{
						string current = string.Format("{0} ({1})", folderName, i);
						if (!usedList.Contains(current))
						{
							f = current;
							break;
						}
					}
				}).ConfigureAwait(false);
				return f;
			}
			else
			{
				return folderName;
			}
		}


		/// <summary>
		/// Rename server file or directory
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		async void listView_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			var list = new List<ListViewItem>();
			for (int i = 0; i < listView.Items.Count; i++)
				if (i != e.Item)
					list.Add(listView.Items[i]);

			if (Exists(e.Label, list))
			{
				MessageBox.Show("Such a name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.CancelEdit = true;
				return;
			}

			if (e.Label != null)
			{
				bool update = false;
				try
				{
					await FTPControl.ExecuteQueryAsync(new RenamePathQuery { ServerPath = listView.Items[e.Item].Text, NewServerPath = e.Label });
				}
				catch (ControlConnectionException x)
				{
					MessageBox.Show(x.Message + " You will be logged out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					form.Logout();
					return;
				}
				catch (FTPQueryException ex)
				{
					update = true;
					MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.CancelEdit = true;
					return;
				}
				if(update)
					await UpdateListViewAsync();
				listView.Sort();
			}
		}

		#region delete


		/// <summary>
		/// Delete selected items on server
		/// </summary>
		/// <returns>task</returns>
		internal async Task DeleteSelectedAsync()
		{
			var list = listView.SelectedItems;
			if (list.Count == 0)
			{
				MessageBox.Show("No items selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			string path = FTPControl.CurrentWorkingDir;

			var items = new List<ListViewItem>();
			foreach (var i in list)
				items.Add((ListViewItem)i);

			try
			{
				await DeleteItemsAsync(items);
			}
			catch (FTPQueryException e)
			{
				ProgressDialog.Close();
				if (!CatchException(e))
					return;
			}

			try
			{
				if (path != FTPControl.CurrentWorkingDir)
					await FTPControl.ExecuteQueryAsync(new ChangeWorkingDirectoryQuery { Path = path });

				await UpdateListViewAsync();
			}
			catch (FTPQueryException e)
			{
				CatchException(e);
			}
		}


		/// <summary>
		/// Delete selected items on server
		/// </summary>
		/// <param name="list">Items to delete</param>
		/// <returns>task</returns>
		private async Task<bool> DeleteItemsAsync(ICollection<ListViewItem> list)
		{
			ProgressDialog.ItemsCount += list.Count;
			var dirList = await ListDirectoryAsync();

			foreach (var item in list)
			{
				if (ProgressDialog.IsClosed)
					return false;

				if (item.Tag.ToString() == fileTag)
				{
					if (!await DeleteFileAsync(item.Text))
						return false;
				}
				else if (item.Tag.ToString() == directoryTag)
				{
					if (!await RemoveDirectoryAsync(item.Text))
						return false;
				}
				ProgressDialog.ItemsCountComplete++;
			}
			return true;
		}


		private async Task<bool> DeleteFileAsync(string file)
		{
			bool aborted = false;

			ProgressDialog.ProgressText = file;
			EventHandler cancelRoutine = (s, ev) => aborted = true;
			ProgressDialog.Canceled += cancelRoutine;

			await FTPControl.ExecuteQueryAsync(new DeleteFileQuery { FileServerPath = file });

			ProgressDialog.Canceled -= cancelRoutine;
			return !aborted;
		}


		/// <summary>
		/// Delete directory and all its content on the server
		/// </summary>
		/// <param name="name">directory name</param>
		/// <returns>task</returns>
		private async Task<bool> RemoveDirectoryAsync(string name)
		{
			await FTPControl.ExecuteQueryAsync(new ChangeWorkingDirectoryQuery { Path = name });

			var list = await ListDirectoryAsync();
			if (!await DeleteItemsAsync(list))
				return false;

			await FTPControl.ExecuteQueryAsync(new DirectoryQuery { Path = FTPControl.CurrentWorkingDir, RemoveDirectory = true });

			await FTPControl.ExecuteQueryAsync(new ChangeWorkingDirectoryQuery { Up = true });

			return true;
		}

		#endregion

		#region recieve

		/// <summary>
		/// Receive all selected files from server
		/// </summary>
		/// <param name="localDirectoryPath">Path where to receive file to</param>
		/// <returns>returns true whether all action completed successfully, otherwise false</returns>
		internal async Task<bool> ReceiveSelectedAsync(string localDirectoryPath)
		{
			result = DialogResult.None;

			var list = listView.SelectedItems;
			if (list.Count == 0)
			{
				MessageBox.Show("No items selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			}

			string path = FTPControl.CurrentWorkingDir;

			var items = new List<ListViewItem>();
			foreach (var i in list)
				items.Add((ListViewItem)i);

			try
			{
				return await ReceiveFilesAsync(items, localDirectoryPath);
			}
			catch (FTPQueryException e)
			{
				ProgressDialog.Close();
				if (!CatchException(e))
					return false;
			}

			try
			{
				if (path != FTPControl.CurrentWorkingDir)
					await FTPControl.ExecuteQueryAsync(new ChangeWorkingDirectoryQuery { Path = path });
			}
			catch (FTPQueryException e)
			{
				CatchException(e);
			}
			return true;
		}


		/// <summary>
		/// Receive files given by list to local directory
		/// </summary>
		/// <param name="list">list of items representation server files /directories</param>
		/// <param name="localDirectoryPath">Path where receive file to</param>
		/// <returns>returns true whether all action completed successfully, otherwise false</returns>
		private async Task<bool> ReceiveFilesAsync(ICollection<ListViewItem> list, string localDirectoryPath)
		{
			ProgressDialog.ItemsCount += list.Count;
			var dirList = await ListDirectoryAsync();

			foreach (var item in list)
			{
				if (ProgressDialog.IsClosed)
					return false;

				string newFilePath = Path.Combine(localDirectoryPath, item.Text);
				bool exists = File.Exists(newFilePath) || Directory.Exists(newFilePath);

				switch (exists ? Ask(item.Text) : DialogResult.Yes)
				{
					case DialogResult.Yes:
						if (item.Tag.ToString() == fileTag)
						{
							if (!await ReceiveFileAsync(item.Text, localDirectoryPath))
								return false;
						}
						else if (item.Tag.ToString() == directoryTag)
						{
							if (!await ReceiveDirectoryAsync(item.Text, localDirectoryPath))
								return false;
						}
						break;

					case DialogResult.Cancel:
						ProgressDialog.Close();
						return false;
				}
				ProgressDialog.ItemsCountComplete++;
			}
			return true;
		}

		/// <summary>
		/// Receive given file from server to given location
		/// </summary>
		/// <param name="file">file to receive from the server</param>
		/// <param name="localDirectoryPath">Path to where receive file</param>
		/// <returns>returns true whether all action completed successfully, otherwise false</returns>
		private async Task<bool> ReceiveFileAsync(string file, string localDirectoryPath)
		{
			bool aborted = false;

			var q = new TransferFileQuery { ServerPath = file, DirectoryPath = localDirectoryPath, Mode = TransferMode.Receive };

			ProgressDialog.ProgressText = file;
			EventHandler cancelRoutine = (s, ev) =>
			{
				q.AbortTransfer();
				aborted = true;
			};
			ProgressDialog.Canceled += cancelRoutine;

			await FTPControl.ExecuteQueryAsync(q);

			ProgressDialog.Canceled -= cancelRoutine;
			return !aborted;
		}

		/// <summary>
		/// Receive given directory from server to given location
		/// </summary>
		/// <param name="directoryName">directory  to where receive from the server</param>
		/// <param name="localDirectoryPath"></param>
		/// <returns>returns true whether all action completed successfully, otherwise false</returns>
		private async Task<bool> ReceiveDirectoryAsync(string directoryName, string localDirectoryPath)
		{
			await FTPControl.ExecuteQueryAsync(new ChangeWorkingDirectoryQuery { Path = directoryName });

			DirectoryInfo d = null;
			try
			{
				d = Directory.CreateDirectory(Path.Combine(localDirectoryPath, directoryName));
			}
			catch (Exception e)
			{
				if (e is IOException || e is UnauthorizedAccessException || e is ArgumentException || e is PathTooLongException || e is DirectoryNotFoundException || e is NotSupportedException)
					return DialogResult.OK == MessageBox.Show("Cannot create directory: " + d.FullName, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
				else
					throw;
			}

			var list = await ListDirectoryAsync();
			bool res = await ReceiveFilesAsync(list, d.FullName);

			await FTPControl.ExecuteQueryAsync(new ChangeWorkingDirectoryQuery { Up = true });

			return res;
		}

		#endregion

		#region send

		/// <summary>
		/// Sends all given files to server or fail
		/// </summary>
		/// <param name="list">List of files to send</param>
		/// <returns>returns true whether all action completed successfully, otherwise false</returns>
		public async Task<bool> SendListAsync(ICollection<FileSystemInfo> list)
		{
			result = DialogResult.None;

			if (list.Count == 0)
			{
				MessageBox.Show("No items selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			}

			string path = FTPControl.CurrentWorkingDir;

			try
			{
				await SendFilesAsync(list);
			}
			catch (FTPQueryException e)
			{
				ProgressDialog.Close();
				if (!CatchException(e))
					return false;
			}

			try
			{
				if (path != FTPControl.CurrentWorkingDir)
					await FTPControl.ExecuteQueryAsync(new ChangeWorkingDirectoryQuery { Path = path });

				await UpdateListViewAsync();
			}
			catch (FTPQueryException e)
			{
				CatchException(e);
			}
			return true;
		}

		/// <summary>
		/// Sends all given files to server or fail
		/// </summary>
		/// <param name="list">List of files to send</param>
		/// <returns>returns true whether all action completed successfully, otherwise false</returns>
		private async Task<bool> SendFilesAsync(ICollection<FileSystemInfo> list)
		{
			ProgressDialog.ItemsCount += list.Count;
			var dirList = await ListDirectoryAsync();

			foreach (var item in list)
			{
				if (ProgressDialog.IsClosed)
					return false;

				bool exists = Exists(item.Name, dirList);

				switch (exists ? Ask(item.Name) : DialogResult.Yes)
				{
					case DialogResult.Yes:
						if (item is FileInfo)
						{
							if (!await SendFileAsync((FileInfo)item))
								return false;
						}
						else
						{
							if (!await SendDirectoryAsync((DirectoryInfo)item, exists))
								return false;
						}
						break;

					case DialogResult.Cancel:
						ProgressDialog.Close();
						return false;
				}
				ProgressDialog.ItemsCountComplete++;
			}

			return true;
		}

		/// <summary>
		/// Sends file to server
		/// </summary>
		/// <param name="item">File info of file to send</param>
		/// <returns>returns true whether all action completed successfully, otherwise false</returns>
		private async Task<bool> SendFileAsync(FileInfo item)
		{
			bool aborted = false;

			var q = new TransferFileQuery { LocalFile = (FileInfo)item, DirectoryPath = FTPControl.CurrentWorkingDir, Mode = TransferMode.Send };

			ProgressDialog.ProgressText = item.Name;
			EventHandler cancelRoutine = (s, ev) =>
			{
				q.AbortTransfer();
				aborted = true;
			};
			ProgressDialog.Canceled += cancelRoutine;

			await FTPControl.ExecuteQueryAsync(q);

			ProgressDialog.Canceled -= cancelRoutine;
			return !aborted;
		}
		private async Task<bool> SendDirectoryAsync(DirectoryInfo directoryInfo, bool exists)
		{
			if (!exists)
				await FTPControl.ExecuteQueryAsync(new DirectoryQuery { Path = directoryInfo.Name });

			await FTPControl.ExecuteQueryAsync(new ChangeWorkingDirectoryQuery { Path = directoryInfo.Name });

			var list = await ListLocalDirectoryAsync(directoryInfo);
			bool res = await SendFilesAsync(list);


			await FTPControl.ExecuteQueryAsync(new ChangeWorkingDirectoryQuery { Up = true });

			return res;
		}

		/// <summary>
		/// Lists given directory
		/// </summary>
		/// <param name="directoryInfo">directory to list</param>
		/// <returns>List of fileInfos </returns>
		private async Task<ICollection<FileSystemInfo>> ListLocalDirectoryAsync(DirectoryInfo directoryInfo)
		{
			try
			{
				return await Task.Run(() => directoryInfo.GetFileSystemInfos()).ConfigureAwait(false);
			}
			catch
			{
				return new List<FileSystemInfo>();
			}
		}

		#endregion


		/// <summary>
		/// Show message box with error. If e is ControlConnectionException then logout user.
		/// </summary>
		/// <param name="e">Exception </param>
		/// <returns>False whether logout required, otherwise true.</returns>
		private bool CatchException(Exception e)
		{
			if (e is ControlConnectionException)
			{
				MessageBox.Show(e.Message + " You will be logged out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				form.Logout();
				return false;
			}

			MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return true;
		}

		static DialogResult result = DialogResult.None;

		/// <summary>
		/// Show dialog to ask user whether overwrite given file
		/// </summary>
		/// <param name="name">name of the file</param>
		/// <returns>return dialog result</returns>
		private DialogResult Ask(string name)
		{
			if (result == DialogResult.None)
			{
				var d = new QuestionDialog(string.Format("Overwrite server file {0}? ( Or merge directories? )", name));
				d.ShowDialog();
				if (d.ForAll)
					result = d.DialogResult;
				return d.DialogResult;
			}
			else
				return result;
		}


		/// <summary>
		/// Determines whether given file exists in given list
		/// </summary>
		/// <param name="fileName">file name to check</param>
		/// <param name="list">list of other files</param>
		/// <returns></returns>
		private bool Exists(string fileName, IEnumerable<ListViewItem> list)
		{
			foreach (var i in list)
				if (i.Text == fileName)
					return true;

			return false;
		}


	}
}

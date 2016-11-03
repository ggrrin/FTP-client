// ***********************************************************************
// Assembly         : FTPClientGUI
// Author           : ggrrin_
// Created          : 07-18-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="MainForm.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using FTP_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FTPClientGUI
{



	/// <summary>
	/// Class MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		/// <summary>
		/// The local controller
		/// </summary>
		private LocalControl localController;

		/// <summary>
		/// The server controller
		/// </summary>
		private ServerControl serverController;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		public MainForm()
		{
			InitializeComponent();			
		}

		/// <summary>
		/// Handles the Click event of the loginToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private async void loginToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var d = new LoginDialog();
			switch (d.ShowDialog())
			{
				case DialogResult.Cancel:
					break;
				case DialogResult.OK:
					(sender as ToolStripMenuItem).Enabled = false;
					
					serverController = new ServerControl(d.FTPControl, serverListView, serverPathLabel, this);
					localController = new LocalControl(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory), localListView, localPathLabel);

					var ts = serverController.UpdateListViewAsync();
					var tl = localController.UpdateListViewAsync();

					splitContainer1.Enabled = true;
					logoutToolStripMenuItem.Enabled = true;
					localToolStripMenuItem.Enabled = serverToolStripMenuItem.Enabled = true;

					await ts;
					await tl;
					break;
			}
		}

		/// <summary>
		/// Handles the Click event of the logoutToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Logout();
		}

		/// <summary>
		/// Logout user, close session, and enable/disable appropriate controls
		/// </summary>
		public void Logout()
		{
			serverController.Dispose();
			localController.Dispose();
			serverController = null;
			localController = null;

			splitContainer1.Enabled = false;
			loginToolStripMenuItem.Enabled = true;
			logoutToolStripMenuItem.Enabled = false;
			localToolStripMenuItem.Enabled = serverToolStripMenuItem.Enabled = false;
		}

		/// <summary>
		/// Handles the Click event of the quitToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Closed" /> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnClosed(EventArgs e)
		{
			if (serverController != null)
				Logout();

			base.OnClosed(e);
		}

		/// <summary>
		/// Handles the Click event of the newFolderToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private async void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			await serverController.CreateFolderAsync();
		}

		/// <summary>
		/// Handles the Click event of the deleteSelectedToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private async void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{	
			await RunTaskWithProgressAsync(serverController.DeleteSelectedAsync);
		}

		/// <summary>
		/// Handles the Click event of the receiveSelectedToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private async void receiveSelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{			
			await RunTaskWithProgressAsync( serverController.ReceiveSelectedAsync, localController.LocalDirectory.FullName);			
		}

		/// <summary>
		/// Handles the Click event of the sendSelectedToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private async void sendSelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			await RunTaskWithProgressAsync(serverController.SendListAsync, localController.GetSelectedFiles());			
		}


		/// <summary>
		/// Run async task given by delegate with showing progress dialog
		/// </summary>
		/// <typeparam name="T">Type of task</typeparam>
		/// <param name="asyncDelegate">Delegate of async method</param>
		/// <returns>task</returns>
		async Task RunTaskWithProgressAsync<T>( Func<T> asyncDelegate) where T : Task
		{
			var dialog = new ProgressDialog();
			serverController.ProgressDialog = dialog;
			var t = asyncDelegate();
			dialog.ShowDialog();
			await t;
		}

		/// <summary>
		/// Run async task given by delegate with showing progress dialog
		/// </summary>
		/// <typeparam name="T1">Parameter of async method type</typeparam>
		/// <typeparam name="T">Type of async method return</typeparam>
		/// <param name="asyncDelegate">Delegate of async method</param>
		/// <param name="param">Parameter for async method</param>
		/// <returns>task</returns>
		async Task RunTaskWithProgressAsync<T1, T>(Func<T1, T> asyncDelegate, T1 param) where T : Task
		{
			var dialog = new ProgressDialog();
			serverController.ProgressDialog = dialog;
			var t = asyncDelegate(param);
			dialog.ShowDialog();
			await t;
		}
	}

	/// <summary>
	/// This namespace contains fundamental classes of application.
	/// </summary>
	[System.Runtime.CompilerServices.CompilerGenerated]
	internal static class NamespaceDoc
	{
	}
}

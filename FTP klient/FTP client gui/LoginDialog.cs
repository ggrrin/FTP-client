// ***********************************************************************
// Assembly         : FTPClientGUI
// Author           : ggrrin_
// Created          : 07-19-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-25-2015
// ***********************************************************************
// <copyright file="LoginDialog.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using FTP_Library;
using FTP_Library.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FTPClientGUI
{
	/// <summary>
	/// Class LoginDialog.
	/// </summary>
	public partial class LoginDialog : Form
	{
		/// <summary>
		/// Gets FTP control of established session
		/// </summary>
		/// <value>The FTP control.</value>
		public FTPControl FTPControl { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LoginDialog"/> class.
		/// </summary>
		public LoginDialog()
		{
			InitializeComponent();			
			serverTextBox.Text = "192.168.137.2";
			loginTextBox.Text = "anonymous";
			passwordTextBox.Text = "";
		}

		/// <summary>
		/// Handles the Click event of the okButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void okButton_Click(object sender, EventArgs e)
		{
			IPAddress ip;
			if (string.IsNullOrWhiteSpace(serverTextBox.Text) || !IPAddress.TryParse(serverTextBox.Text, out ip) || string.IsNullOrWhiteSpace(loginTextBox.Text))
			{
				MessageBox.Show("Fields server and login has to be filled correctly.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			EnableDisable(false);			

			var c = new FTPControl(new IPEndPoint(ip, 21212), loginTextBox.Text, passwordTextBox.Text, true);
			FTPControl = c;
			var t = Task.Run(new Action(() =>
			{
				try
				{
					c.InitializeConnection();
				}
				catch (FTPQueryException ex)
				{
					c.Dispose();

					if (c == FTPControl)//user did not abort connecting
						this.Invoke(new Action(() =>
						{
							MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							EnableDisable(true);
						}));
					return;
				}

				if (c == FTPControl && !aborted)
				{
					this.Invoke(new Action(() =>
					{
						DialogResult = System.Windows.Forms.DialogResult.OK;
						Close();
					}));
				}
				else if(aborted)//window unexpectly closed //cross X
				{
					c.Dispose();
				}
			}));

		}

		/// <summary>
		/// Handles the Click event of the cancelButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();

		}

		/// <summary>
		/// Enables the disable.
		/// </summary>
		/// <param name="b">if set to <c>true</c> [b].</param>
		private void EnableDisable(bool b)
		{
			serverTextBox.Enabled = loginTextBox.Enabled = passwordTextBox.Enabled = okButton.Enabled = cancelButton.Enabled = b;
			abortButton.Enabled = !b;

			if(!b)
				statusLabel.Text = "Connecting...";
			else
				statusLabel.Text = "";
		}

		/// <summary>
		/// Handles the Click event of the abortButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void abortButton_Click(object sender, EventArgs e)
		{

			if (FTPControl != null)
			{
				var c = FTPControl;
				FTPControl = null;

				c.Dispose();
				EnableDisable(true);
			}
		}

		/// <summary>
		/// The aborted
		/// </summary>
		bool aborted = false;

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Closed" /> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			if(FTPControl != null && !FTPControl.Connected)//Connecting
			{
				aborted = true;
			}
		}
	}
}

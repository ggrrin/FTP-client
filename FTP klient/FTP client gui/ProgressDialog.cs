// ***********************************************************************
// Assembly         : FTPClientGUI
// Author           : ggrrin_
// Created          : 07-21-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-23-2015
// ***********************************************************************
// <copyright file="ProgressDialog.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FTPClientGUI
{
	/// <summary>
	/// Class ProgressDialog.
	/// </summary>
	public partial class ProgressDialog : Form
	{
		/// <summary>
		/// Total number of items to process
		/// </summary>
		/// <value>The items count.</value>
		public int ItemsCount
		{
			get { return progressBar1.Maximum; }
			set { progressBar1.Maximum = value; }
		}


		/// <summary>
		/// Completed number of items
		/// </summary>
		/// <value>The items count complete.</value>
		public int ItemsCountComplete
		{
			get { return progressBar1.Value; }
			set
			{
				progressBar1.Value = value;
				label.Text = label.Text;
				if (ItemsCountComplete == ItemsCount)
				{
					ProgressText = "done";
					Close();
				}
			}
		}

		/// <summary>
		/// The text
		/// </summary>
		private string text;

		/// <summary>
		/// Text representation of current working item
		/// </summary>
		/// <value>The progress text.</value>
		public string ProgressText
		{
			get { return text; }
			set { label.Text = string.Format("Progress: {0}/{1} Working: {2}", ItemsCountComplete, ItemsCount, text = value); }
		}

		/// <summary>
		/// Cancel button pressed.
		/// </summary>
		public event EventHandler Canceled;


		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressDialog" /> class.
		/// </summary>
		public ProgressDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Handles the Click event of the cancelButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Gets a value indicating whether this instance is closed.
		/// </summary>
		/// <value><c>true</c> if this form is closed; otherwise, <c>false</c>.</value>
		public bool IsClosed { get; private set; }

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Closing" /> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			if (Canceled != null)
				Canceled(this, new EventArgs());

			IsClosed = true;			
		}

	}
}

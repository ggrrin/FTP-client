// ***********************************************************************
// Assembly         : FTPClientGUI
// Author           : ggrrin_
// Created          : 07-21-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-21-2015
// ***********************************************************************
// <copyright file="QuestionDialog.cs" company="">
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
	/// Class QuestionDialog.
	/// </summary>
	public partial class QuestionDialog : Form
	{
		/// <summary>
		/// Gets a value indicating whether [for all].
		/// </summary>
		/// <value><c>true</c> if [for all]; otherwise, <c>false</c>.</value>
		public bool ForAll { get { return allCheckBox.Checked; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuestionDialog"/> class.
		/// </summary>
		public QuestionDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Windows.Forms.Control" /> class with specific text.
		/// </summary>
		/// <param name="text">The text displayed by the control.</param>
		public QuestionDialog(string text) :this ()
		{
			this.textLabel.Text = text;
		}

		/// <summary>
		/// Handles the Click event of the yesButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void yesButton_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Yes;
			Close();
		}

		/// <summary>
		/// Handles the Click event of the noButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void noButton_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.No;
			Close();
		}

		/// <summary>
		/// Handles the Click event of the cancelButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
		
	}

	
}

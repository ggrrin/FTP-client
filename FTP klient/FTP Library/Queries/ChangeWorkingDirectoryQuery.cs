// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="ChangeWorkingDirectoryQuery.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;


namespace FTP_Library.Queries
{
	/// <summary>
	/// Represents query to change server working directory.
	/// Also update FTPControl current working directory
	/// </summary>
	public class ChangeWorkingDirectoryQuery : FTPQuery
	{
		/// <summary>
		/// Gets or sets server path. Use either this property or <c>Up</c>.
		/// </summary>
		/// <value>The path.</value>
		public string Path { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether going up in directory tree.
		/// Use either this property or <c>Path</c>.
		/// </summary>
		/// <value><c>true</c> if up; otherwise, <c>false</c>.</value>
		public bool Up { get; set; }

		/// <summary>
		/// Work with network is assumed. Method can be called multiple times if first attempts fail.
		/// </summary>
		public override void SendQuery()
		{
			if (!Up)
				Control.SendQuery(string.Format("CWD {0}", Path));
			else
				Control.SendQuery("CDUP");

			string message;
			int code = Control.ReceiveResponse(out message);
			switch (code)
			{
				case 200://CDUP
					//Command okay.
					UpdateCurrentDir();
					break;

				case 250://CWD
					//Requested file action okay, completed.
					UpdateCurrentDir();
					break;

				default:
					base.HandleErrorReplies(code, message);
					break;
			}
		}

		/// <summary>
		/// Update FTP control working directory
		/// </summary>
		private void UpdateCurrentDir()
		{
			Control.SendQuery("PWD");

			string message;
			int code = Control.ReceiveResponse(out message);

			switch (code)
			{
				case 257:
					//257 "PATHNAME" created.
					Control.CurrentWorkingDir = ParsePath(message.Trim());
					break;

				default:
					base.HandleErrorReplies(code, message);
					break;
			}
		}

		/// <summary>
		/// Parse path from server response message
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns>System.String.</returns>
		/// <exception cref="FTP_Library.Exceptions.ClientNotSupportException">Invalid path respond.</exception>
		private string ParsePath(string message)
		{
			int first = message.IndexOf('"');
			int last = message.LastIndexOf('"');

			if (first == -1 || last == -1)
				throw new Exceptions.ClientNotSupportException("Invalid path respond.");

			var sb = new StringBuilder();

			for(int i = first + 1; i < last; i++)
			{
				char c = message[i];

				sb.Append(c);

				//double quoting 
				if (c == '"')
					i++;
			}

			return sb.ToString();
		}

	}
}

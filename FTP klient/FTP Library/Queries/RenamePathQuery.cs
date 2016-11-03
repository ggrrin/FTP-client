// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-22-2015
// ***********************************************************************
// <copyright file="RenamePathQuery.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FTP_Library.Queries
{
	/// <summary>
	/// Represents query to rename files on ftp server. Can be used to move files.
	/// </summary>
	public class RenamePathQuery : FTPQuery
	{
		/// <summary>
		/// Path of the file on the server to rename.
		/// </summary>
		/// <value>The server path.</value>
		public string ServerPath { get; set; }

		/// <summary>
		/// New path for file on the server.
		/// </summary>
		/// <value>The new server path.</value>
		public string NewServerPath { get; set; }

		/// <summary>
		/// Method called one time before <c>SendQuery</c>. No work with network should be done.
		/// </summary>
		/// <exception cref="FTP_Library.Exceptions.ParameterException">Path is not fully specified!</exception>
		public override void BeforeSend()
		{
			if (string.IsNullOrWhiteSpace(ServerPath) || string.IsNullOrWhiteSpace(NewServerPath))
			{
				throw new Exceptions.ParameterException("Path is not fully specified!");
			}
		}

		/// <summary>
		/// Work with network is assumed. Method can be called multiple times if first attempts fail.
		/// </summary>
		public override void SendQuery()
		{
			Control.SendQuery(string.Format("RNFR {0}", ServerPath));
			string message;
			ProcessResponseRNFR(Control.ReceiveResponse(out message), message);
		}

		/// <summary>
		/// Processes the response RNFR.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="message">The message.</param>
		private void ProcessResponseRNFR(int code, string message)
		{
			switch(code)
			{
				case 350:
					RenameQuery();
					break;
				default:
					base.HandleErrorReplies(code, message);
					break;
			}
		}

		/// <summary>
		/// Finish rename query
		/// </summary>
		private void RenameQuery()
		{
			Control.SendQuery(string.Format("RNTO {0}", NewServerPath));
			string message;
			int code = Control.ReceiveResponse(out message);
			switch(code)
			{
				case 250:
					//Requested file action okay, completed.
					break;
				default:
					base.HandleErrorReplies(code, message);
					break;
			}
		}
	}
}

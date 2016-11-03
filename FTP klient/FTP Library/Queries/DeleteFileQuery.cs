// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-22-2015
// ***********************************************************************
// <copyright file="DeleteFileQuery.cs" company="">
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
	/// Represents query to delete files on ftp server.
	/// </summary>
	public class DeleteFileQuery : FTPQuery
	{
		/// <summary>
		/// Relative or absolute server path of file to delete.
		/// </summary>
		/// <value>The file server path.</value>
		public string FileServerPath { get; set; }

		/// <summary>
		/// Work with network is assumed. Method can be called multiple times if first attempts fail.
		/// </summary>
		public override void SendQuery()
		{
			Control.SendQuery(string.Format("DELE {0}", FileServerPath));

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

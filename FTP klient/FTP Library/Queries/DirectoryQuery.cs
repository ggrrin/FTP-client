// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="DirectoryQuery.cs" company="">
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
	/// Represents query to either create or remove directory.
	/// </summary>
	public class DirectoryQuery : FTPQuery
	{
		/// <summary>
		/// ABSOLUTE path of the directory to work with.
		/// </summary>
		/// <value>The path.</value>
		public string Path { get; set; }

		/// <summary>
		/// Specify whether delete or create directory
		/// </summary>
		/// <value><c>true</c> if [remove directory]; otherwise, <c>false</c>.</value>
		public bool RemoveDirectory { get; set; }

		/// <summary>
		/// Method called one time before <c>SendQuery</c>. No work with network should be done.
		/// </summary>
		/// <exception cref="FTP_Library.Exceptions.ParameterException">Invalid parameter: Path</exception>
		public override void BeforeSend()
		{
			if (string.IsNullOrWhiteSpace(Path))
				throw new Exceptions.ParameterException("Invalid parameter: Path");
		}

		/// <summary>
		/// Work with network is assumed. Method can be called multiple times if first attempts fail.
		/// </summary>
		public override void SendQuery()
		{
			if (!RemoveDirectory)
				Control.SendQuery(string.Format("MKD {0}", Path));
			else
				Control.SendQuery(string.Format("RMD {0}", Path));

			string message;
			int code = Control.ReceiveResponse(out message);
			switch (code)
			{
				case 250:
					//250 Requested file action okay, completed.
					break;
				case 257:
					//257 "PATHNAME" created.
					break;

				default:
					base.HandleErrorReplies(code, message);
					break;
			}
		}
	}
}

// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="ListDirectoryQuery.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FTP_Library.Queries
{
	/// <summary>
	/// Represents query to list directory items.
	/// </summary>
	public class ListDirectoryQuery : DataConnectionQuery
	{
		/// <summary>
		/// Determines whether format is rather human readable (but inconsistent) or machine readable
		/// </summary>
		/// <value><c>true</c> if [human readable]; otherwise, <c>false</c>.</value>
		public bool HumanReadable { get; set; }

		/// <summary>
		/// The path
		/// </summary>
		private string path = "";

		/// <summary>
		/// Directory path to list. Only in human readable mode.
		/// If not set current directory will be listed.
		/// </summary>
		/// <value>The directory path.</value>
		public string DirectoryPath 
		{
			get { return path; }
			set { path = value; }
		}

		/// <summary>
		/// String list of directory reply form server.
		/// </summary>
		/// <value>The reply.</value>
		public  string Reply { get; private set; }

		/// <summary>
		/// The reply stream
		/// </summary>
		private MemoryStream replyStream;

		/// <summary>
		/// In this method send any command requesting data connection
		/// </summary>
		protected override void DataConnectionRequestQuery()
		{
			if(!HumanReadable)
				Control.SendQuery(string.Format("MLSD {0}", DirectoryPath ?? "")); //rfc 3659
			else
				Control.SendQuery(string.Format("LIST {0}", DirectoryPath ?? ""));
		}

		/// <summary>
		/// In this method setup the streams aka specify direction of data transfer.
		/// </summary>
		/// <param name="inputStream">Streams from which will be read.</param>
		/// <param name="outputStream">Stream to which will be written.</param>
		protected override void SetupStreams(out System.IO.Stream inputStream, out System.IO.Stream outputStream)
		{
			inputStream = Control.Data;
			outputStream = replyStream = new MemoryStream();
		}

		/// <summary>
		/// Reads reply from stream.
		/// </summary>
		protected override void BeforeClose()
		{
			replyStream.Position = 0;
			using (var r = new StreamReader(replyStream, Encoding.ASCII))
			{
				Reply = r.ReadToEnd();
			}
		}
	}
}

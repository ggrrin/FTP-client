// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="TransferFileQuery.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using FTP_Library.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace FTP_Library.Queries
{
	/// <summary>
	/// Represents mode of file transfer
	/// </summary>
	public enum TransferMode
	{
		/// <summary>
		/// File send from client to server
		/// </summary>
		Send = 0,

		/// <summary>
		/// File receive from server to client
		/// </summary>
		Receive
	}

	/// <summary>
	/// Represents query for either send or receive files to/from ftp server
	/// </summary>
	public class TransferFileQuery : DataConnectionQuery
	{
		/// <summary>
		/// Defines direction of data transfer.
		/// </summary>
		/// <value>The mode.</value>
		public TransferMode Mode { get; set; }

		/// <summary>
		/// FileInfo of file to be sent. Mode send has to be set.
		/// </summary>
		/// <value>The local file.</value>
		public FileInfo LocalFile { get; set; }


		/// <summary>
		/// Path to file on server to be received. Receive mode has to be set.
		/// </summary>
		/// <value>The server path.</value>
		public string ServerPath { get; set; }

		/// <summary>
		/// Path to directory where to save the file. Either on server (mode=send) or local (mode=receive)
		/// </summary>
		/// <value>The directory path.</value>
		public string DirectoryPath { get; set; }

		/// <summary>
		/// Method called one time before <c>SendQuery</c>. No work with network should be done.
		/// </summary>
		/// <exception cref="FTP_Library.Exceptions.ParameterException">
		/// Local file is not specified correctly.
		/// or
		/// Directory is not specified correctly.
		/// or
		/// Directory is not specified correctly.
		/// or
		/// Invalid server path.
		/// </exception>
		public override void BeforeSend()
		{

			switch (Mode)
			{
				case TransferMode.Send:
					if (LocalFile == null || !LocalFile.Exists)
						throw new ParameterException("Local file is not specified correctly.");

					if (string.IsNullOrWhiteSpace(DirectoryPath))
						throw new ParameterException("Directory is not specified correctly.");

					ServerPath = string.Format("{0}/{1}", DirectoryPath == "/" ? "" : DirectoryPath, LocalFile.Name);
					break;

				case TransferMode.Receive:
					if (DirectoryPath == null)
						throw new ParameterException("Directory is not specified correctly.");

					Directory.CreateDirectory(DirectoryPath);

					int k = 0;
					if (ServerPath.Contains("/"))//absolute path
					{

						k = ServerPath.LastIndexOf('/');

						if (++k >= ServerPath.Length)
							throw new ParameterException("Invalid server path.");
					}

					LocalFile = new FileInfo(Path.Combine(DirectoryPath, ServerPath.Substring(k)));

					break;
				default:
					Debug.Assert(false, "Unexpected mode!");
					break;
			}
		}


		/// <summary>
		/// In this method send any command requesting data connection
		/// </summary>
		protected override void DataConnectionRequestQuery()
		{
			switch (Mode)
			{
				case TransferMode.Send:
					Control.SendQuery(string.Format("STOR {0}", ServerPath));
					break;
				case TransferMode.Receive:
					Control.SendQuery(string.Format("RETR {0}", ServerPath));
					break;
			}
		}

		/// <summary>
		/// In this method setup the streams aka specify direction of data transfer.
		/// </summary>
		/// <param name="inputStream">Streams from which will be read.</param>
		/// <param name="outputStream">Stream to which will be written.</param>
		/// <exception cref="FTP_Library.Exceptions.ParameterException">Can not open file.</exception>
		protected override void SetupStreams(out Stream inputStream, out Stream outputStream)
		{
			outputStream = inputStream = null;
			try
			{
				switch (Mode)
				{
					case TransferMode.Send:
						inputStream = LocalFile.OpenRead();
						outputStream = Control.Data;
						break;
					case TransferMode.Receive:
						inputStream = Control.Data;
						outputStream = LocalFile.OpenWrite();
						break;
					default:
						inputStream = outputStream = null;
						break;
				}

			}
			catch (Exception e)
			{
				if (e is UnauthorizedAccessException || e is DirectoryNotFoundException || e is IOException)
					throw new ParameterException("Can not open file.", e);

				throw;
			}
		}

		/// <summary>
		/// This method is intented to dispose any resources after successful and also aborted data transfer.
		/// </summary>
		protected override void CleanUnfinishedData()
		{
			switch (Mode)
			{
				case TransferMode.Send:
					Control.ExecuteQuery(new DeleteFileQuery { FileServerPath = ServerPath.ToString() });
					break;
				case TransferMode.Receive:
					LocalFile.Delete();
					break;
			}
		}
	}
}

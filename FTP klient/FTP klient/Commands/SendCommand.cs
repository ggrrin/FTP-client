﻿// ***********************************************************************
// Assembly         : FTPClient
// Author           : ggrrin_
// Created          : 07-17-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="SendCommand.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using FTP_Library.Exceptions;
using FTP_Library.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;


namespace FTPClient.Commands
{
	/// <summary>
	/// Represents command for sending local files to remote server.
	/// </summary>
	public class SendCommand : IInterpreter
	{
		/// <summary>
		/// Input from which commands are read.
		/// Property should be set by creator of appropriate interpreter.
		/// </summary>
		/// <value>The input.</value>
		public TextReader Input { get; set; }

		/// <summary>
		/// Output to which commands result are written.
		/// Property should be set by creator of appropriate interpreter.
		/// </summary>
		/// <value>The output.</value>
		public TextWriter Output { get; set; }

		/// <summary>
		/// Application context
		/// Property should be set by creator of appropriate interpreter.
		/// </summary>
		/// <value>The application context.</value>
		public Application AppContext { get; set; }

		/// <summary>
		/// Runs the interpreter
		/// </summary>
		/// <returns>Return s false whether application end is requested.</returns>
		public bool Run()
		{
			if (!AppContext.Connected)
			{
				Output.WriteLine("Not connected.");
				return true;
			}

			Output.WriteLine("File to send:");

			string path = Input.ReadLine().Trim();

			FileInfo f = null;

			try
			{
				if (path.Contains(':'))
					f = new FileInfo(path);
				else
					f = new FileInfo(Path.Combine(AppContext.CurrentWorkingDir.FullName, path));
			}
			catch (Exception e)
			{
				if (e is NotSupportedException || e is SecurityException || e is ArgumentException || e is PathTooLongException)
					f = null;

				throw;
			}

			if (f != null && f.Exists)
			{
				Output.WriteLine("Write server destination directory:");

				var q = new TransferFileQuery { Mode = TransferMode.Send, LocalFile = f, DirectoryPath = Input.ReadLine().Trim() };

				bool succes = true;
				try
				{
					AppContext.Control.ExecuteQuery(q);
				}
				catch (FTPQueryException e)
				{
					Output.WriteLine(e.Message);
					succes = false;
				}

				if (succes)
					Output.WriteLine("File send succeed.");
			}
			else
				Output.WriteLine("Invalid file path.");

			return true;
		}

	}
}

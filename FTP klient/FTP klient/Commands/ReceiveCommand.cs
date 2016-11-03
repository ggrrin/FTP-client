// ***********************************************************************
// Assembly         : FTPClient
// Author           : ggrrin_
// Created          : 07-17-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="ReceiveCommand.cs" company="">
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
	/// Represents command for receive file form remote ftp server.
	/// </summary>
	public class ReceiveCommand : IInterpreter
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

			Output.WriteLine("Write file to receive:");
			string serverFilePath = Input.ReadLine().Trim();
			Output.WriteLine("Write directory to save:");
			string localDirectory = Input.ReadLine().Trim();

			DirectoryInfo d = null;

			try
			{
				if (localDirectory.Contains(':'))
					d = new DirectoryInfo(localDirectory);
				else
					d = new DirectoryInfo(Path.Combine(AppContext.CurrentWorkingDir.FullName, localDirectory));
			}
			catch (Exception e)
			{
				if (e is NotSupportedException || e is SecurityException || e is ArgumentException || e is PathTooLongException)
					d = null;

				throw;
			}

			if (d != null && d.Exists)
			{

				var q = new TransferFileQuery { Mode = TransferMode.Receive, ServerPath = serverFilePath, DirectoryPath = d.FullName };

				bool success = true;
				try
				{
					AppContext.Control.ExecuteQuery(q);
				}
				catch (FTPQueryException e)
				{
					Output.WriteLine(e.Message);
					success = false;

				}

				if (success)
					Output.WriteLine("File receive succeed.");

			}
			else
				Output.WriteLine("Invalid destination directory path.");

			return true;
		}

	}
}

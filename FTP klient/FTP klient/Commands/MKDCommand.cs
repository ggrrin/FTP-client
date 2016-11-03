// ***********************************************************************
// Assembly         : FTPClient
// Author           : ggrrin_
// Created          : 07-17-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="MKDCommand.cs" company="">
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
	/// Represents command for creating new local directory
	/// </summary>
	class MKDCommand : IInterpreter
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

			Output.WriteLine("Write new server directory path:");
			string path = Input.ReadLine().Trim();

			if(string.IsNullOrWhiteSpace(path))
			{
				Output.WriteLine("Invalid directory name.");
				return true;
			}

			try
			{
				AppContext.Control.ExecuteQuery(new DirectoryQuery { RemoveDirectory = false, Path = path });
				Output.WriteLine("Directory created successfully.");
			}
			catch (FTPQueryException e)
			{
				Output.WriteLine(e.Message);
			}

			return true;
		}

	}
}

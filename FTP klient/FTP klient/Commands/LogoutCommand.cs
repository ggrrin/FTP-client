// ***********************************************************************
// Assembly         : FTPClient
// Author           : ggrrin_
// Created          : 07-17-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="LogoutCommand.cs" company="">
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
using System.Text;
using System.Threading.Tasks;


namespace FTPClient.Commands
{
	/// <summary>
	/// Class LogoutCommand.
	/// </summary>
	class LogoutCommand : IInterpreter
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
			if (AppContext.Control == null)
			{
				Output.WriteLine("Already logged out.");
			}
			else
			{
				try
				{
					AppContext.Control.ExecuteQuery(new LogoutQuery());
				}
				catch (FTPQueryException) { }
				AppContext.Control = null;
			}

			return true;
		}
	}
}

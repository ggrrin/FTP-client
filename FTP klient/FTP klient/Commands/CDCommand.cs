// ***********************************************************************
// Assembly         : FTPClient
// Author           : ggrrin_
// Created          : 07-17-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="CDCommand.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
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
	/// This namespace contains fundamental command for the application
	/// </summary>
	[System.Runtime.CompilerServices.CompilerGenerated]
	internal static class NamespaceDoc
	{
	}


	/// <summary>
	/// Represents interpreter of local change directory.
	/// </summary>
	class CDCommand : IInterpreter
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
			Output.WriteLine("Write path:");

			string path = Input.ReadLine();

			switch (path.Trim())
			{
				case ".":
					break;

				case "..":
					AppContext.CurrentWorkingDir = AppContext.CurrentWorkingDir.Parent ?? AppContext.CurrentWorkingDir;
					break;
				default:
					Workout(path.Trim());
					break;
			}

			Output.WriteLine("Local: {0}", AppContext.CurrentWorkingDir.FullName);

			return true;
		}

		/// <summary>
		/// Change directory to given path
		/// </summary>
		/// <param name="path">path to change to</param>
		private void Workout(string path)
		{
			DirectoryInfo d = null;

			try
			{
				//possibly absolute path
				if (path.Contains(':'))			
						d = new DirectoryInfo(path);		
				else //relative path
					d = new DirectoryInfo(Path.Combine(AppContext.CurrentWorkingDir.FullName, path));

			}
			catch (Exception e)
			{
				if (e is NotSupportedException || e is SecurityException || e is ArgumentException || e is PathTooLongException)
					d = null;

				throw;
			}

			if (d != null && d.Exists)
				AppContext.CurrentWorkingDir = d;
			else
				Output.WriteLine("Invalid directory path.");
		}
	}
}

// ***********************************************************************
// Assembly         : FTPClient
// Author           : ggrrin_
// Created          : 07-17-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="ClientInterpreter.cs" company="">
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


namespace FTPClient
{
	/// <summary>
	/// Represents main interpreter to recognize client commands.
	/// </summary>
	public class ClientInterpreter : IInterpreter
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
		/// Parser for parsing commands.
		/// </summary>
		private CommandParser parser;

		/// <summary>
		/// Initialize interpreter.
		/// </summary>
		/// <param name="factories">The factories.</param>
		/// <param name="input">The input.</param>
		/// <param name="output">The output.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public ClientInterpreter(IEnumerable<ICommandFactory> factories, TextReader input, TextWriter output)
		{
			if (factories == null || input == null || output == null)
				throw new ArgumentNullException();

			Input = input;
			Output = output;

			parser = new CommandParser(factories);	
		}

		/// <summary>
		/// Runs the interpreter.
		/// </summary>
		/// <returns>Return s false whether application end is requested.</returns>
		public bool Run()
		{
			Output.WriteLine("Welcome! Write a command. (Consider using \"help\" command if U R lost.)");

			while (true)
			{
				var cmdInterpret = parser.ParseCommand(Input.ReadLine());

				if (cmdInterpret != null)
				{
					cmdInterpret.Input = Input;
					cmdInterpret.Output = Output;
					cmdInterpret.AppContext = AppContext;

					if (!cmdInterpret.Run())
						break;
					else
						Output.WriteLine();
				}
				else
				{
					Output.WriteLine("Unrecognized command!");
				}
			}

			Output.WriteLine("Have a nice day.");
			return false;
		}
	}
}

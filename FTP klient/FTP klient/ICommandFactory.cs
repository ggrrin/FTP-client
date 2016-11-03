using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTPClient
{
	/// <summary>
	/// Interface for implementation of Command factory
	/// </summary>
	public interface ICommandFactory
	{

		/// <summary>
		/// Text-form command
		/// </summary>
		string CommandToken { get; }

		/// <summary>
		/// Help text for command
		/// </summary>
		string HelpText { get; }


		/// <summary>
		/// Interpreter for command
		/// </summary>
		IInterpreter CommandInterpreter { get; }

	}
}

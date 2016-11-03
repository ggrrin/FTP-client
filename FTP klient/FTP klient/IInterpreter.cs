using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FTPClient
{
	/// <summary>
	/// Interface for implementing interpreters and their transitive use.
	/// </summary>
	public interface IInterpreter
	{
		/// <summary>
		/// Input from which commands are read.
		/// Property should be set by creator of appropriate interpreter.
		/// </summary>
		TextReader Input { get; set; }

		/// <summary>
		/// Output to which commands result are written.
		/// Property should be set by creator of appropriate interpreter. 
		/// </summary>
		TextWriter Output { get; set; }

		/// <summary>
		/// Application context
		/// Property should be set by creator of appropriate interpreter.
		/// </summary>
		Application AppContext { get; set; }

		/// <summary>
		/// Runs the interpreter
		/// </summary>
		/// <returns>Return s false whether application end is requested.</returns>
		bool Run();

	}
}

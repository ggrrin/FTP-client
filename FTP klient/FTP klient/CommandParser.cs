using System;
using System.Collections.Generic;
using System.Text;

namespace FTPClient
{
	/// <summary>
	/// Represents parser for transforming command text representaion into command interpreters
	/// </summary>
	public class CommandParser
	{
		private Dictionary<string, ICommandFactory> factories = new Dictionary<string, ICommandFactory>();

		/// <summary>
		/// Initialize parser with appropirate command factories
		/// </summary>
		/// <param name="ifactories">command factories</param>
		public CommandParser(IEnumerable<ICommandFactory> ifactories)
		{
			if (ifactories == null)
				throw new ArgumentNullException();

			foreach (var f in ifactories)
			{
				if (f == null)
					throw new ArgumentNullException("One of the factories is null.");

				if (factories.ContainsKey(f.CommandToken))
					throw new ArgumentException("Ambigious tokens for different factories: " + f.CommandToken);
				else
					factories.Add(f.CommandToken, f);
			}
		}

		/// <summary>
		/// Parser string commmand representaion and returns appropriate command interpreter
		/// or return null if command unrecognized
		/// </summary>
		/// <param name="command">command text representation</param>
		/// <returns>Return appropriate command interpreter or null if unrecognized</returns>
		public IInterpreter ParseCommand(string command)
		{
			ICommandFactory factory;
			factories.TryGetValue(command.Trim(), out factory);

			if (factory != null)
				return factory.CommandInterpreter;

			return null;
		}

	}
}

using FTP_Library;
using FTPClient.Factories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient
{
	/// <summary>
	/// This namespace contains fundamental classes and intefaces of the application
	/// </summary>
	[System.Runtime.CompilerServices.CompilerGenerated]
	internal static class NamespaceDoc
	{
	}


	/// <summary>
	/// Represents state of application, which can be obtained by interprets.
	/// </summary>
	/// <remarks>
	/// <para>
	/// To run an application create new instance of <c>Application</c> and call <c>Run</c> method on it. 
	/// You can also create own commands and forward their factories in constructor.
	/// </para>
	/// <para>
	/// To create custom commands implement <c>IInterpreter</c> interface. By that you get <c>TextWriter Output</c>, <c>TextReader Input</c> and <c>Application Appcontext</c>
	/// which you can use in <c>Run</c>. Through application context you can access <c>FTPcontrol</c> to access ftp connection. You have to also create factory by implementing
	/// <c>ICommandFactory</c> interface which specify command token, returns new instance of command interpreter and also contains some help text which is used in help command.
	/// </para>
	/// </remarks>
	public class Application
	{
		/// <summary>
		/// main command interpreter
		/// </summary>
		private ClientInterpreter interpreter;

		/// <summary>
		/// Determine whether connection is established.
		/// </summary>
		public bool Connected { get { return Control != null; } }

		private bool verbose = false;

		/// <summary>
		/// Get or sets verbose mode (ftp communication is printed)
		/// </summary>
		public bool VerboseMode
		{
			get { return verbose; }

			set
			{
				verbose = value;
				SwitchVerboseMode();
			}
		}

		private FTPControl control = null;

		/// <summary>
		/// FTP control to execute ftp queries and maintain user session
		/// </summary>
		public FTPControl Control
		{
			get { return control; }

			set
			{
				if (control != null)
					control.Dispose();

				control = value;
				SwitchVerboseMode();
			}
		}

		/// <summary>
		/// Gets or sets current local working directory
		/// </summary>
		public DirectoryInfo CurrentWorkingDir { get; set; }

		/// <summary>
		/// List of CommandsFactories active in application
		/// </summary>
		public IList<ICommandFactory> Factories { get; private set; }

		/// <summary>
		/// Initialize application and sets input/output to console
		/// </summary>
		public Application()
			: this(Console.In, Console.Out)
		{

		}

		/// <summary>
		/// Initialize application sets specific input/output
		/// </summary>
		/// <param name="input">command input</param>
		/// <param name="output">command output</param>
		public Application(TextReader input, TextWriter output)
			: this(input, output, new ICommandFactory[0])
		{

		}

		/// <summary>
		/// Initialize application sets specific input/output and given command factories
		/// </summary>
		/// <param name="input">command input</param>
		/// <param name="output">command output</param>
		/// <param name="factories">command factories</param>
		public Application(TextReader input, TextWriter output, IEnumerable<ICommandFactory> factories)
		{
			CurrentWorkingDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

			var fcts = new List<ICommandFactory>{ 
				LoginFactory.Instance,
				LogoutFactory.Instance,
				ExitFactory.Instance,
				PWDFactory.Instance,
				CDFactory.Instance,
				SCDFactory.Instance,
				LSFactory.Instance,
				SLSFactory.Instance,
				SendFactory.Instance,
				ReceiveFactory.Instance,
				RemoveFactory.Instance,
				RenameFactory.Instance,
				MKDFactory.Instance,
				RMDFactory.Instance,
				HelpFactory.Instance,
				VerboseFactory.Instance
			};

			fcts.AddRange(factories);

			Factories = fcts;

			interpreter = new ClientInterpreter(fcts, input, output);
			interpreter.AppContext = this;
		}

		/// <summary>
		/// Run the application. -> client interpreter
		/// </summary>
		public void Run()
		{
			interpreter.Run();
		}

		private void SwitchVerboseMode()
		{
			if (Connected)
				if (verbose)
				{
					Control.QueryOutput = Control.ResponseOutput = interpreter.Output;
				}
				else
				{
					Control.QueryOutput = Control.ResponseOutput = null;
				}
		}

	}
}

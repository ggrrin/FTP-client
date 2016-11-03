using FTPClient.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient.Factories
{
	/// <summary>
	/// This namespace contains factories for fundamentals commands
	/// </summary>
	[System.Runtime.CompilerServices.CompilerGenerated]
	internal static class NamespaceDoc
	{
	}



	/// <summary>
	/// Represents factory for creating Login commands
	/// </summary>
	public class LoginFactory : ICommandFactory
	{
		private static LoginFactory instance = new LoginFactory();


		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static LoginFactory Instance 
		{
			get { return instance; }			
		}

		private LoginFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "login"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new LoginCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "login\t Connect to server and login the user."; }
		}
	}

	/// <summary>
	/// Represents factory for creating Logout commands
	/// </summary>
	public class LogoutFactory : ICommandFactory
	{
		private static LogoutFactory instance = new LogoutFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static LogoutFactory Instance
		{
			get { return instance; }
		}

		private LogoutFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "logout"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new LogoutCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "logout\t Disconnect from server."; }
		}
	}


	/// <summary>
	/// Represents factory for creating Exit commands
	/// </summary>
	public class ExitFactory : ICommandFactory
	{
		private static ExitFactory instance = new ExitFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static ExitFactory Instance
		{
			get { return instance; }
		}

		private ExitFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "exit"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new ExitCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "exit\t Disconnection from the server & exit the application."; }
		}
	}


	/// <summary>
	/// Represents factory for creating PWD commands
	/// </summary>
	public class PWDFactory : ICommandFactory
	{
		private static PWDFactory instance = new PWDFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static PWDFactory Instance
		{
			get { return instance; }
		}

		private PWDFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "pwd"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new PWDCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "pwd\t Print working directories, both on the server & local."; }
		}
	}


	/// <summary>
	/// Represents factory for creating CD commands
	/// </summary>
	public class CDFactory : ICommandFactory
	{
		private static CDFactory instance = new CDFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static CDFactory Instance
		{
			get { return instance; }
		}

		private CDFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "cd"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new CDCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "cd \t Change local working directory."; }
		}
	}

	/// <summary>
	/// Represents factory for creating SCD commands
	/// </summary>
	public class SCDFactory : ICommandFactory
	{
		private static SCDFactory instance = new SCDFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static SCDFactory Instance
		{
			get { return instance; }
		}

		private SCDFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "scd"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new SCDCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "scd\t Change server working directory."; }
		}
	}

	/// <summary>
	/// Represents factory for creating LS commands
	/// </summary>
	public class LSFactory : ICommandFactory
	{
		private static LSFactory instance = new LSFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static LSFactory Instance
		{
			get { return instance; }
		}

		private LSFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "ls"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new LSCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "ls\t List local working directory."; }
		}
	}

	/// <summary>
	/// Represents factory for creating SLS commands
	/// </summary>
	public class SLSFactory : ICommandFactory
	{
		private static SLSFactory instance = new SLSFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static SLSFactory Instance
		{
			get { return instance; }
		}

		private SLSFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "sls"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new SLSCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "sls\t List server working directory."; }
		}
	}

	/// <summary>
	/// Represents factory for creating Send commands
	/// </summary>
	public class SendFactory : ICommandFactory
	{
		private static SendFactory instance = new SendFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static SendFactory Instance
		{
			get { return instance; }
		}

		private SendFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "send"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new SendCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "Send\t Sends specific file to the server."; }
		}
	}


	/// <summary>
	/// Represents factory for creating Receive commands
	/// </summary>
	public class ReceiveFactory : ICommandFactory
	{
		private static ReceiveFactory instance = new ReceiveFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static ReceiveFactory Instance
		{
			get { return instance; }
		}

		private ReceiveFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "receive"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new ReceiveCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "receive\t Receives specific file from the server."; }
		}
	}


	/// <summary>
	/// Represents factory for creating Remove commands
	/// </summary>
	public class RemoveFactory : ICommandFactory
	{
		private static RemoveFactory instance = new RemoveFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static RemoveFactory Instance
		{
			get { return instance; }
		}

		private RemoveFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "remove"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new RemoveCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "remove\t Remove specific file on the server."; }
		}
	}


	/// <summary>
	/// Represents factory for creating Rename commands
	/// </summary>
	public class RenameFactory : ICommandFactory
	{
		private static RenameFactory instance = new RenameFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static RenameFactory Instance
		{
			get { return instance; }
		}

		private RenameFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "rename"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new RenameCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "rename\t Rename specific file on the server."; }
		}
	}


	/// <summary>
	/// Represents factory for creating MKD commands
	/// </summary>
	public class MKDFactory : ICommandFactory
	{
		private static MKDFactory instance = new MKDFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static MKDFactory Instance
		{
			get { return instance; }
		}

		private MKDFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "mkd"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new MKDCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "mkd\t Make directory on the server."; }
		}
	}


	/// <summary>
	/// Represents factory for creating RMD commands
	/// </summary>
	public class RMDFactory : ICommandFactory
	{
		private static RMDFactory instance = new RMDFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static RMDFactory Instance
		{
			get { return instance; }
		}

		private RMDFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "rmd"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new RMDCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "rmd\t Remove directory on the server."; }
		}
	}


	/// <summary>
	/// Represents factory for creating Help commands
	/// </summary>
	public class HelpFactory : ICommandFactory
	{
		private static HelpFactory instance = new HelpFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static HelpFactory Instance
		{
			get { return instance; }
		}

		private HelpFactory()
		{ }

		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "help"; }
		}

		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new HelpCommand(); }
		}

		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "help\t Prints help to the application & commands."; }
		}
	}

	/// <summary>
	/// Represents factory for creating Verbose commands
	/// </summary>
	public class VerboseFactory : ICommandFactory
	{
		private static VerboseFactory instance = new VerboseFactory();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static VerboseFactory Instance
		{
			get { return instance; }
		}

		private VerboseFactory()
		{ }



		/// <summary>
		/// Text-form command
		/// </summary>
		/// <value>The command token.</value>
		public string CommandToken
		{
			get { return "verbose"; }
		}


		/// <summary>
		/// Interpreter for command
		/// </summary>
		/// <value>The command interpreter.</value>
		public IInterpreter CommandInterpreter
		{
			get { return new VerboseCommand(); }
		}


		/// <summary>
		/// Help text for command
		/// </summary>
		/// <value>The help text.</value>
		public string HelpText
		{
			get { return "verbose\t Turn on/off verbose mode, which means communication with server is printed."; }
		}
	}
}

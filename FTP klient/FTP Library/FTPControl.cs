using FTP_Library.Queries;
using FTP_Library.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTP_Library
{
	/// <summary>
	/// This namespace contains fundamental base classes for establishing ftp connections
	/// </summary>
	[System.Runtime.CompilerServices.CompilerGenerated]
	internal static class NamespaceDoc
	{
	}


	/// <summary>
	/// Represents connection to FTP server. Maintain connection and provide API for executing queries.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In order to connect to ftp server create new Instance of <c>FTPcontrol</c> with appropriate parameters.
	/// If you don't need password to login leave it empty. Consider using passive mode, which should work in every scenario unless active mode.
	/// By creating new instance no connection is established yet. Only a connection parameters are set. To connect to server call <c>InitializeConnection</c> which either succeed or
	/// throw an <c>FTPQueryException</c>.
	/// </para>
	/// <para>
	/// To execute server query call <c>ExecuteQuery</c> and as a parameter forward new instance of specific query class.
	/// Every query execution either succeed or throws some descendant of <c>FTPQueryException</c>. In most scenarios exception message contains error message directly form the server.
	/// Take a look at <c>FTP_Library.Exceptions</c> to see all exception with more detailed description.
	/// </para>
	/// <para>
	/// You can also create your own queries by extending <c>FTPQuery</c> class. In that situation consider using <c>Control</c> property to access <c>FTPcontrol</c> instance
	/// of created connection. All code which is communicating with server should be placed in <c>SendQuery</c> method. You can also do any pre/post-processing in <c>BeforeSend</c> or <c>AfterSend</c> method.
	/// Only code in <c>SendQuery</c> method is run in try block and if any recoverable exception occurs. It will try to correct error and try to run <c>SendQuery</c> method once again.
	/// This error fix attempt is done only one time and also with no time delay. Which means that can fix only error such a unexpected logout or lost of server connection which is ready immediately again.
	/// If error is not able to fix exception is rethrown. In <c>SendQuery</c> method you can either use another query or use <c>FTPControl.SendQuery</c> and <c>ReceiveResponse</c> methods for sending directly ftp commands and receiving theirs responses.
	/// These two methods automatically writes to <c>QueryOutput</c> resp. <c>ResponseOutput</c>. If you do not want tu use these last two methods you can write directly to <c>FTPcontrol.ControlWriter</c>, but you should keep writing to previous TextWriters to preserve consistency.
	/// You can also use <c>FTPControl.EstablishDataConnection</c> method to establish data connection either in passive or active mode. Which depends on <c>PassiveMode</c> property. 
	/// Once data connection is established you can use network stream <c>FTPcontrol.Data</c> to either send or receive data through that.
	/// You can also extend directly <c>DataConnectionQuery</c> which do establishing data connection and sending/receiving data for you.
	/// Also note that you should catch any exception which can occurs and rethrow it as appropriate exception from <c>FTP_Library.Exceptions</c> to preserve consistency.
	/// </para>
	/// <para>
	/// After you are done with server communication you should dispose <c>FTPcontrol</c> to release all underling resources.
	/// After disposing an instance you can call again <c>InitializeConnection</c> to reestablish the connection.
	/// </para>
	/// </remarks>
	public class FTPControl : IDisposable
	{
		/// <summary>
		/// Returns whether connection is established or not.
		/// </summary>
		public bool Connected { get; private set; }

		/// <summary>
		/// Represents writer, which prints every sent command.
		/// </summary>
		public TextWriter QueryOutput { get; set; }

		/// <summary>
		/// Represents writer, which prints every received response to command.
		/// </summary>
		public TextWriter ResponseOutput { get; set; }

		/// <summary>
		/// IP address and port of the FTP server.
		/// </summary>
		public IPEndPoint Server { get; private set; }

		/// <summary>
		/// IP address and port of data connection (if it is established) to the FTP server .
		/// </summary>
		public IPEndPoint ServerData { get; set; }

		/// <summary>
		/// User name for access FTP server.
		/// Property is used by LoginQuery
		/// </summary>
		public string User { get; /*private*/ set; }


		/// <summary>
		/// Password for access FTP server.
		/// Property is used by LoginQuery
		/// </summary>
		public string Pass { get; /*private*/ set; }


		/// <summary>
		/// Represents whether data connections are established passive or active way.
		/// </summary>
		public bool PassiveMode { get; set; }

		/// <summary>
		/// Represents number of port for establishing data connection in active mode.
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Unix like path of current server working directory.
		/// Property is updated by ChangeWorkingDirQuery
		/// </summary>
		public string CurrentWorkingDir { get; set; }

		/// <summary>
		/// Represents reader which reads from control connection network stream.
		/// </summary>
		public StreamReader ControlReader { get; private set; }


		/// <summary>
		/// Represents writer which write to control connection network stream.
		/// </summary>
		public StreamWriter ControlWriter { get; private set; }

		/// <summary>
		/// Network stream of data connection (if it is established)
		/// </summary>
		public NetworkStream Data { get; private set; }

		////////////////////////////////////////

		private Socket controlConnection;
		private Socket dataConnection;

		private NetworkStream control;


		/// <summary>
		/// Initialize Control by setting appropriate Properties. For establishing connection call InitializeConnection method.
		/// For correct leaving control the dispose method should be also called.
		/// </summary>
		/// <param name="ip">Ip address of ftp server.</param>
		/// <param name="user">User name to login.</param>
		/// <param name="pass">Password to login (if needed - some server does not have to require it)</param>
		/// <param name="passiveMode">Establishing data connection mode.</param>
		public FTPControl(IPEndPoint ip, string user, string pass, bool passiveMode)
		{
			if (ip == null || user == null)
				throw new ArgumentNullException();

			Server = ip;
			User = user;
			Pass = pass;
			PassiveMode = passiveMode;
			Port = 20;
			Connected = false;
		}


		/// <summary>
		/// Initialize connection to the server, login a user specified in control
		/// properties and sets current working directory to default server value.
		/// After disposing the control, this method can be called again to reestablish the connection.
		/// </summary>
		public void InitializeConnection()
		{
			Connected = false;
			controlConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			controlConnection.ReceiveTimeout = 2000;
			controlConnection.SendTimeout = 2000;

			try
			{				
				if (QueryOutput != null)
					QueryOutput.WriteLine("Connecting to server...");
				controlConnection.Connect(Server);
			}
			catch (Exception e)
			{
				throw new ControlConnectionException("Unable to connect server", e);
			}

			control = new NetworkStream(controlConnection, true);
			ControlReader = new StreamReader(control, Encoding.ASCII, false, 4096, true);
			ControlWriter = new StreamWriter(control, Encoding.ASCII, 4096, true);
			ControlWriter.AutoFlush = true;

			CheckConnection();

			ExecuteQuery(new LoginQuery());			

			ExecuteQuery(new ChangeWorkingDirectoryQuery { Path = "." });
			Connected = true;
		}


		/// <summary>
		/// Check whether connection established correctly, otherwise throw an ControlConnectionException
		/// </summary>
		private void CheckConnection()
		{
			switch (ReceiveResponse())
			{
				case 120:
					//Service ready in nnn minutes.
					Dispose();
					throw new ControlConnectionException("Service ready in nnn minutes.");

				case 220:
					//Service ready for new user.
					break;
				case 421:
					//Service not available, closing control connection.
					Dispose();
					throw new ControlConnectionException("Service not available, closing control connection.");
			}
		}


		/// <summary>
		/// Sends command represented by query parameter
		/// After calling this method ReceiveResponse should be called for getting server response (but depends on server command logic)
		/// </summary>
		/// <param name="query">command for server</param>
		public void SendQuery(string query)
		{
			if (QueryOutput != null)
				QueryOutput.WriteLine(query);

			try
			{
				ControlWriter.WriteLine(query);
			}
			catch (IOException e)
			{
				if (e.InnerException is SocketException)
					throw new ControlConnectionException("Control connection lost.", e.InnerException);
				else
					throw;
			}
			catch (ObjectDisposedException e)
			{
				throw new ControlConnectionException("Control connection lost.", e);
			}
		}

		/// <summary>
		/// Receive response from server and parse integral ftp error code and text message
		/// Prior to calling this method the SendQuery should by called to actually make the constrain to server.
		/// </summary>
		/// <param name="message">Text representation of message received by the server.</param>
		/// <returns>Returns integral ftp error code. </returns>
		public int ReceiveResponse(out string message)
		{
			string response;
			try
			{
				response = ControlReader.ReadLine();
			}
			catch (IOException e)
			{
				if (e.InnerException is SocketException)
					throw new ControlConnectionException("Control connection lost.", e.InnerException);
				else
					throw;
			}
			catch (SocketException e)
			{
				throw new ControlConnectionException("Control connection lost.", e.InnerException);
			}

			if (response == null)
				throw new ControlConnectionException("Control connection closed.");

			if (ResponseOutput != null)
				ResponseOutput.WriteLine(response);

			message = response.Substring(4);

			return int.Parse(response.Substring(0, 3));
		}

		/// <summary>
		///  Receive response from server and parse integral ftp error code 
		/// Prior to calling this method the SendQuery should by called to actually make the constrain to server.
		/// </summary>
		/// <returns>Returns integral ftp error code.</returns>
		public int ReceiveResponse()
		{
			string m;
			return ReceiveResponse(out m);
		}

		private bool reestablishingConnection = false;

		/// <summary>
		/// Execute given query. If the connection is lost it will try to reestablish it one time. 
		/// If it fail again it throws exception and connection has to by reestablished manually.
		/// </summary>
		/// <param name="query">Query</param>
		/// <returns>Task</returns>
		public async Task ExecuteQueryAsync(FTPQuery query)
		{
			await Task.Run(() => ExecuteQuery(query)).ConfigureAwait(false);
		}

		/// <summary>
		/// Execute given query. If the connection is lost it will try to reestablish it one time. 
		/// If it fail again it throws exception and connection has to by reestablished manually.
		/// </summary>
		/// <param name="query">Query </param>
		public void ExecuteQuery(FTPQuery query)
		{
			query.Control = this;
			query.BeforeSend();

			try
			{
				query.SendQuery();
			}
			catch (FTPQueryException e)
			{
				if (e is LoginException || e is ControlConnectionException)
				{
					if (!reestablishingConnection)
					{
						reestablishingConnection = true;

						string workDir = CurrentWorkingDir;


						Dispose();

						InitializeConnection();

						ExecuteQuery(new ChangeWorkingDirectoryQuery { Path = workDir });

						reestablishingConnection = false;

						//send query again
						query.SendQuery();
					}
				}
				else
					throw;
			}

			query.AfterSend();
		}

		/// <summary>
		/// Establish data connection in appropriate mode.
		/// </summary>
		public void EstablishDataConnection()
		{
			if (!Connected)
				throw new InvalidOperationException("Control connection is not established. Do it by calling InitializeConnection method.");

			//specify mode before establishing data connection
			if (PassiveMode)
				ExecuteQuery(new DataConnectionModeQuery { ActiveMode = false });
			else
				ExecuteQuery(new DataConnectionModeQuery { ActiveMode = true, Port = this.Port });

			try
			{
				dataConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				dataConnection.ReceiveTimeout = 2000;
				dataConnection.SendTimeout = 2000;

				if (PassiveMode)
					dataConnection.Connect(ServerData);
				else
					dataConnection.Accept();

				Data = new NetworkStream(dataConnection, true);
			}
			catch (SocketException e)
			{
				throw new Exceptions.DataConnectionException("Connection refused by server.", e);
			}

		}


		/// <summary>
		/// Dispose all network object. InitializeConnection can be called again to reestablish the connection.
		/// </summary>
		public void Dispose()
		{
			//ExecuteQuery(new LogoutQuery());

			if (ControlReader != null)
				ControlReader.Close();

			if (ControlWriter != null)
				ControlWriter.Close();

			if (control != null)
				control.Close();

			if (Data != null)
				Data.Close();
		}

	}
}

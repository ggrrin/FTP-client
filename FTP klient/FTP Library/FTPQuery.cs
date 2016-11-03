using FTP_Library.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FTP_Library
{
	/// <summary>
	/// Represents base class for queries which can be executed by FTPControl.
	/// </summary>
	public abstract class FTPQuery
	{
		/// <summary>
		/// Property is set by FTPControl before query execution, thus it can be user in quires to use FTPcontrol features.
		/// </summary>
		public FTPControl Control { get; set; }

		/// <summary>
		/// Method called one time before <c>SendQuery</c>. No work with network should be done.
		/// </summary>
		public virtual void BeforeSend() { }


		/// <summary>
		/// Work with network is assumed. Method can be called multiple times if first attempts fail.
		/// </summary>
		public abstract void SendQuery();

		/// <summary>
		/// Method called one time after <c>SendQuery</c>. No work with network should be done.
		/// </summary>
		public virtual void AfterSend() { }

		/// <summary>
		/// Handle FTP server error response by throwing appropriate exception.
		/// </summary>
		/// <param name="code">Ftp response code/.</param>
		/// <param name="serverMessage">FTP error message.</param>
		protected void HandleErrorReplies(int code, string serverMessage)
		{
			switch(code)
			{
				case 421: throw new ControlConnectionException("Service not available, closing control connection." + " ServerReply: " + serverMessage);
				case 425: throw new DataConnectionException("Can't open data connection." + " ServerReply: " + serverMessage);
				case 426: throw new DataConnectionException("Connection closed; transfer aborted." + " ServerReply: " + serverMessage);
				case 450: throw new ActionRefusedException("Requested file action not taken." + " ServerReply: " + serverMessage);
				case 451: throw new ActionRefusedException("Requested action aborted: local error in processing." + " ServerReply: " + serverMessage);
				case 452: throw new ActionRefusedException("Requested action not taken." + " ServerReply: " + serverMessage);
				case 500: throw new ParameterException("Syntax error, command unrecognized." + " ServerReply: " + serverMessage);
				case 501: throw new ParameterException("Syntax error in parameters or arguments." + " ServerReply: " + serverMessage);
				case 502: throw new ClientNotSupportException("Command not implemented. Client can handle the issue." + " ServerReply: " + serverMessage);
				case 503: Debug.Assert(false, "Response unexpected!" + " ServerReply: " + serverMessage); break; //Bad sequence of commands.
				case 504: throw new ParameterException("Command not implemented for that parameter." + " ServerReply: " + serverMessage);
				case 530: throw new LoginException("Not logged in." + " ServerReply: " + serverMessage);
				case 532: throw new ClientNotSupportException("Need account for storing files." + " ServerReply: " + serverMessage);
				case 550: throw new ParameterException("Requested action not taken." + " ServerReply: " + serverMessage);
				case 551: throw new ClientNotSupportException("Requested action aborted: page type unknown." + " ServerReply: " + serverMessage);
				case 552: throw new ParameterException("Requested file action aborted. File might be too large." + " ServerReply: " + serverMessage);
				case 553: throw new ParameterException("Requested action not taken. File name not allowed." + " ServerReply: " + serverMessage);
				default:					
					break;
			}
		}
	}
}

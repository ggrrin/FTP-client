// ***********************************************************************
// Assembly         : FTPClient
// Author           : ggrrin_
// Created          : 07-17-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="LoginCommand.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using FTP_Library;
using FTP_Library.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace FTPClient.Commands
{
	/// <summary>
	/// Represents command to connect to ftp server and login the user.
	/// </summary>
	class LoginCommand : IInterpreter
	{
		/// <summary>
		/// Input from which commands are read.
		/// Property should be set by creator of appropriate interpreter.
		/// </summary>
		/// <value>The input.</value>
		public System.IO.TextReader Input { get; set; }

		/// <summary>
		/// Output to which commands result are written.
		/// Property should be set by creator of appropriate interpreter.
		/// </summary>
		/// <value>The output.</value>
		public System.IO.TextWriter Output { get; set; }

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
			if (AppContext.Control != null)
				Output.WriteLine("Already logged in.");
			else
				Login();
						
			return true;
		}

		/// <summary>
		/// Connect and login to ftp server
		/// </summary>
		private void Login()
		{
			var c = new FTPControl(GetIpAddress(), GetUser(), GetPass(), true);

			AppContext.Control = c;

			try
			{
				c.InitializeConnection();
			}
			catch (FTPQueryException e)
			{
				Output.WriteLine(e.Message);
				AppContext.Control = null;
				return;
			}
			
			Output.WriteLine("Login successful.");
		}

		/// <summary>
		/// Gets and parse ip address from user
		/// </summary>
		/// <returns>Ip address and port of the server</returns>
		public IPEndPoint GetIpAddress()
		{
			IPAddress adr;

			while (true)
			{
				Output.WriteLine("Write ip address of the server:");

				if (IPAddress.TryParse(Input.ReadLine(), out adr))
				{
					break;
				}
				else
				{
					Output.WriteLine("Invalid format.");
				}
			}

			return new IPEndPoint(adr, 21);
		}

		/// <summary>
		/// Gets user name from input.
		/// </summary>
		/// <returns>Returns user name.</returns>
		public string GetUser()
		{
			string usr;

			while (true)
			{
				Output.WriteLine("Write a user name:");

				if (!string.IsNullOrWhiteSpace(usr = Input.ReadLine()))
				{
					break;
				}
				else
				{
					Output.WriteLine("Invalid format.");
				}
			}

			return usr;
		}

		/// <summary>
		/// Gets pass from input.
		/// </summary>
		/// <returns>Returns password.</returns>
		public string GetPass()
		{
			Output.WriteLine("Write a password (if needed):");
			return Input.ReadLine();
		}


	}
}

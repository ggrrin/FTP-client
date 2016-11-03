// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="LoginQuery.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using FTP_Library.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;


namespace FTP_Library.Queries
{
	/// <summary>
	/// Represents query to login to ftp server.
	/// </summary>
	public class LoginQuery : FTPQuery
	{
		/// <summary>
		/// The user query
		/// </summary>
		private string userQuery;

		/// <summary>
		/// Method called one time before <c>SendQuery</c>. No work with network should be done.
		/// </summary>
		/// <exception cref="InvalidOperationException">User has to be specified!</exception>
		public override void BeforeSend()
		{
			if (string.IsNullOrWhiteSpace(Control.User))
				throw new InvalidOperationException("User has to be specified!");

			userQuery = string.Format("USER {0}", Control.User);
		}

		/// <summary>
		/// Work with network is assumed. Method can be called multiple times if first attempts fail.
		/// </summary>
		/// <exception cref="FTP_Library.Exceptions.ClientNotSupportException">Servers with account constrain are not supported by client!</exception>
		/// <exception cref="FTP_Library.Exceptions.ParameterException">Invalid user name.</exception>
		public override void SendQuery()
		{
			Control.SendQuery(userQuery);

			string message;
			int code = Control.ReceiveResponse(out message);

			switch (code)
			{
				case 331:
					//User name okay, need password.
					SendPass();
					break;

				case 332:
					//Need account for login
					throw new ClientNotSupportException("Servers with account constrain are not supported by client!");					

				case 230:
					//User logged in, proceed.				
					break;

				case 530:
					throw new ParameterException("Invalid user name.");					

				default:
					base.HandleErrorReplies(code, message);
					break;							
			}

		}

		/// <summary>
		/// Send pass to server if needed
		/// </summary>
		/// <exception cref="FTP_Library.Exceptions.ParameterException">
		/// Pass has to be specified for user  + Control.User
		/// or
		/// Invalid user name or pass.
		/// </exception>
		/// <exception cref="FTP_Library.Exceptions.ClientNotSupportException">Servers with account constrain are not supported by client!</exception>
		private void SendPass()
		{
			if (string.IsNullOrWhiteSpace(Control.Pass))
				throw new ParameterException("Pass has to be specified for user " + Control.User);

			Control.SendQuery(string.Format("PASS {0}", Control.Pass));
			string message;
			int code = Control.ReceiveResponse(out message);

			switch (code)
			{
				case 202:
					//Command not implemented, superfluous at this site.
					Debug.Assert(false, "Response unexpected!");
					break;

				case 332:
					//Need account for login
					throw new ClientNotSupportException("Servers with account constrain are not supported by client!");	

				case 230:
					//User logged in, proceed.
					break;

				case 530:
					throw new ParameterException("Invalid user name or pass.");	

				default:
					base.HandleErrorReplies(code, message);
					break;
			}
		}

	}
}

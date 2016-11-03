// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="DataConnectionModeQuery.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FTP_Library.Exceptions;


namespace FTP_Library.Queries
{
	/// <summary>
	/// This namespace contains classes representing fundamental ftp queries
	/// </summary>
	[System.Runtime.CompilerServices.CompilerGenerated]
	internal static class NamespaceDoc
	{
	}

	/// <summary>
	/// Represents query which setup passive or active data connection mode.
	/// </summary>
	public class DataConnectionModeQuery : FTPQuery
	{

		/// <summary>
		/// Determines active or passive mode data connection.
		/// </summary>
		/// <value><c>true</c> if [active mode]; otherwise, <c>false</c>.</value>
		public bool ActiveMode { get; set; }

		/// <summary>
		/// Determines port in active mode. Thus setting need only if active mode is selected.
		/// </summary>
		/// <value>The port.</value>
		public int Port { get; set; }

		/// <summary>
		/// The query
		/// </summary>
		string query;

		/// <summary>
		/// Method called one time before <c>SendQuery</c>. No work with network should be done.
		/// </summary>
		public override void BeforeSend()
		{
			if (ActiveMode)
				query = string.Format("PORT {0},{1}", Port / 256, Port % 256);
			else
				query = string.Format("PASV");
		}

		/// <summary>
		/// Work with network is assumed. Method can be called multiple times if first attempts fail.
		/// </summary>
		public override void SendQuery()
		{
			Control.SendQuery(query);

			string message;
			int code = Control.ReceiveResponse(out message);

			if (ActiveMode)
				ProcessActiveResponse(code, message);
			else
				ProcessPassiveResponse(code, message);
		}


		/// <summary>
		/// Process server and setup passive mode in FTPControl.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="message">The message.</param>
		private void ProcessPassiveResponse(int code, string message)
		{
			switch(code)
			{
				case 227:
					//Entering Passive Mode (h1,h2,h3,h4,p1,p2).
					EnterPassiveMode(message);
					break;

				default:
					base.HandleErrorReplies(code, message);
					break;
			}
		}

		/// <summary>
		/// parse passive mode ip from message
		/// </summary>
		/// <param name="message">server text message</param>
		private void EnterPassiveMode(string message)
		{
			int first = message.IndexOf('(');
			int last = message.IndexOf(')');

			string ipp = message.Substring(first + 1, last - first - 1);

			var ipq = ipp.Split(new char[] { ',' });

			IPAddress target = IPAddress.Parse(string.Format("{0}.{1}.{2}.{3}", ipq[0], ipq[1], ipq[2], ipq[3]));

			Control.PassiveMode = true;
			Control.ServerData = new IPEndPoint(target, int.Parse(ipq[4]) * 256 + int.Parse(ipq[5]));
		}

		/// <summary>
		/// Process server response and setup active mode in FTPControl.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <param name="message">The message.</param>
		private void ProcessActiveResponse(int code, string message)
		{
			switch (code)
			{
				case 200:
					//Command okay.
					Control.PassiveMode = false;
					break;

				default:
					HandleErrorReplies(code,  message);
					break;
			}
		}
	}
}

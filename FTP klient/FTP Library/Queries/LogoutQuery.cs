// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-22-2015
// ***********************************************************************
// <copyright file="LogoutQuery.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FTP_Library.Queries
{
	/// <summary>
	/// Represents query to logout from ftp server
	/// </summary>
	public class LogoutQuery : FTPQuery
	{
		/// <summary>
		/// Work with network is assumed. Method can be called multiple times if first attempts fail.
		/// </summary>
		public override void SendQuery()
		{
			Control.SendQuery("QUIT");
			switch(Control.ReceiveResponse())
			{
				case 221://Service closing control connection.
					break;

				case 500: //error
					break;
			}
		}


	}
}

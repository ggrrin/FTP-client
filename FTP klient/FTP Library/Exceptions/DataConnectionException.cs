// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="DataConnectionException.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FTP_Library.Exceptions
{
	/// <summary>
	/// Exception is thrown when there is an issue with data connection.
	/// </summary>
	public class DataConnectionException : FTPQueryException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataConnectionException"/> class.
		/// </summary>
		public DataConnectionException()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="DataConnectionException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public DataConnectionException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="DataConnectionException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public DataConnectionException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}

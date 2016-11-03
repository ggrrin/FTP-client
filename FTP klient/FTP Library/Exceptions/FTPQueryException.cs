// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-21-2015
// ***********************************************************************
// <copyright file="FTPQueryException.cs" company="">
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
	/// Exception is base class for all exceptions threw by ftp control. It intended to catch all exception raised by ftp communication.
	/// </summary>
	public class FTPQueryException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FTPQueryException"/> class.
		/// </summary>
		public FTPQueryException()
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public FTPQueryException(string message) : base(message)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public FTPQueryException(string message, Exception innerException)
			: base(message, innerException)
		{}
	}
}

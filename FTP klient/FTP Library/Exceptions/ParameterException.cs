// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="ParameterException.cs" company="">
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
	/// Exception thrown by query when server fails to execute the command probably due to
	/// wrong setting of query parameters (query properties- it does not have to be ftp commands parameter itself (but it is related of course.))
	/// </summary>
	public class ParameterException : FTPQueryException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterException"/> class.
		/// </summary>
		public ParameterException()
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ParameterException(string message) : base(message)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public ParameterException(string message, Exception innerException) : base(message, innerException)
		{}
	}
}

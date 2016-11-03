// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-21-2015
// ***********************************************************************
// <copyright file="ControlConnectionException.cs" company="">
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
	/// Exception is thrown when there is an issue with control connection.
	/// </summary>
	public class ControlConnectionException : FTPQueryException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ControlConnectionException"/> class.
		/// </summary>
		public ControlConnectionException()
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControlConnectionException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public ControlConnectionException(string message) : base(message)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControlConnectionException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public ControlConnectionException(string message, Exception innerException)
			: base(message, innerException)
		{}
	}
}

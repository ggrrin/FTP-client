// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="ClientNotSupportException.cs" company="">
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
	/// Exception thrown if server respond response which client cant workout. (feature is unsupported)
	/// </summary>
	public class ClientNotSupportException : FTPQueryException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ClientNotSupportException"/> class.
		/// </summary>
		public ClientNotSupportException()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientNotSupportException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public ClientNotSupportException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientNotSupportException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public ClientNotSupportException(string message, Exception innerException)
			: base(message, innerException)
		{ }

	}
}

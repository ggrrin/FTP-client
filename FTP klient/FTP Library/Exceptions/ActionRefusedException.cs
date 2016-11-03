// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="ActionRefusedException.cs" company="">
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
	/// This namespace contains exception thrown by ftp library
	/// </summary>
	[System.Runtime.CompilerServices.CompilerGenerated]
	internal static class NamespaceDoc
	{
	}


	/// <summary>
	/// Exception thrown if query is refused by server and could not be refused in future.
	/// </summary>
	public class ActionRefusedException : FTPQueryException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ActionRefusedException"/> class.
		/// </summary>
		public ActionRefusedException()
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionRefusedException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public ActionRefusedException(string message) : base(message)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionRefusedException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public ActionRefusedException(string message, Exception innerException)
			: base(message, innerException)
		{}
	}
}

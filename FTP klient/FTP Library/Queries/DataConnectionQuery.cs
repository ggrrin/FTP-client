// ***********************************************************************
// Assembly         : FTPLibrary
// Author           : ggrrin_
// Created          : 07-16-2015
//
// Last Modified By : ggrrin_
// Last Modified On : 07-26-2015
// ***********************************************************************
// <copyright file="DataConnectionQuery.cs" company="">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using FTP_Library.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FTP_Library.Queries
{
	/// <summary>
	/// Represents query for establishing data connection. To specify use of data connection inherit this class.
	/// </summary>
	/// <remarks>To simplify working with data connection you can extend this class which do some work for you. You should at least implement <c>DataConnectionRequestQuery</c>
	/// and <c>SetupStreams</c>. In the first one you should send some command which requests data connection and in second one just need to set the streams to send data in right direction.
	/// <c>FTPControl.Data</c> is established data connection and second stream is up to you. After transfer is done both streams are closed. Method <c>BeforeClose</c> is called before it happens so you can for example get received data from that stream.
	/// If an error occurs while transferring data you can clean any mess in <c>CleanUnfinishedData</c> method.</remarks>
	public abstract class DataConnectionQuery : FTPQuery
	{
		/// <summary>
		/// In this method send any command requesting data connection
		/// </summary>
		abstract protected void DataConnectionRequestQuery();

		/// <summary>
		/// In this method setup the streams aka specify direction of data transfer.
		/// </summary>
		/// <param name="inputStream">Streams from which will be read.</param>
		/// <param name="outputStream">Stream to which will be written.</param>
		abstract protected void SetupStreams(out Stream inputStream, out Stream outputStream);

		/// <summary>
		/// This method is intented to dispose any resources after successful and also aborted data transfer.
		/// </summary>
		virtual protected void CleanUnfinishedData()
		{}

		/// <summary>
		/// Method called before streams are closed. To provide chance to process gained data.
		/// </summary>
		virtual protected void BeforeClose()
		{}

		/// <summary>
		/// Work with network is assumed. Method can be called multiple times if first attempts fail.
		/// </summary>
		public sealed override void SendQuery()
		{
			Control.SendQuery("TYPE I");
			Control.ReceiveResponse();

			if (Control.PassiveMode)
				Control.EstablishDataConnection();

			DataConnectionRequestQuery();

			ProcessRespond();

			Control.SendQuery("TYPE A");
			Control.ReceiveResponse();
		}


		/// <summary>
		/// Process server respond. If positive data transfer is started.
		/// </summary>
		private void ProcessRespond()
		{
			//(110) Restart marker reply.

			string message;
			int code = Control.ReceiveResponse(out message);

			switch (code)
			{
				case 125:
					//125, Data connection already open; transfer starting.
					TransferFile();
					break;

				case 150:
					//150 File status okay; about to open data connection.
					if (!Control.PassiveMode)
						Control.EstablishDataConnection();
					TransferFile();
					break;

				case 226:
					//226, Closing data connection.
					//Transfer Complete
					break;

				case 250:
					//250 Requested file action okay, completed.
					break;

				default:
					base.HandleErrorReplies(code, message);
					break;

			}
		}

		/// <summary>
		/// The transfer aborted
		/// </summary>
		private bool transferAborted = false;

		/// <summary>
		/// Transfer data from input stream to output  stream
		/// </summary>
		/// <exception cref="FTP_Library.Exceptions.FTPQueryException">Data transfer error!</exception>
		private void TransferFile()
		{
			Stream s1 = null, s2 = null;

			try
			{
				transferAborted = false;

				SetupStreams(out s1, out s2);
				ConnectStreams(s1, s2);
			}
			catch (Exception e)
			{
				//ABOR command not useful - http://stackoverflow.com/questions/6457207/programming-ftp-how-to-abort-file-transfer
				transferAborted = true;

				throw new FTPQueryException("Data transfer error!", e);
			}
			finally
			{
				if(!transferAborted)
					BeforeClose();

				s1.Close();
				s2.Close();
				//respond to closing of data connection
				ProcessRespond();

				//remove unfinished files
				if (transferAborted)
					CleanUnfinishedData();
			}

		}


		/// <summary>
		/// Sends data from input stream to output stream.
		/// </summary>
		/// <param name="inputStream">The input stream.</param>
		/// <param name="outputStream">The output stream.</param>
		private void ConnectStreams(Stream inputStream, Stream outputStream)
		{
			const int max = 1024;
			byte[] buffer = new byte[max];

			int curLen = 0;

			while ((curLen = inputStream.Read(buffer, 0, max)) != 0)
			{
				if (transferAborted)
					break;

				outputStream.Write(buffer, 0, curLen);
				outputStream.Flush();
			}
		}


		/// <summary>
		/// Abort transfer and close data connection.
		/// </summary>
		public void AbortTransfer()
		{
			transferAborted = true;
		}
	}
}

#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
namespace DevExpress.Web {
	internal class UploadWorkerRequest : HttpWorkerRequest {
		private HttpWorkerRequest workerRequest = null;
		private byte[] workerBuffer = null;
		public UploadWorkerRequest(HttpWorkerRequest request, byte[] buffer) {
			this.workerBuffer = buffer;
			this.workerRequest = request;
		}
		public HttpWorkerRequest WorkerRequest {
			get { return workerRequest; }
		}
		public byte[] WorkerBuffer {
			get { return workerBuffer; }
			set { workerBuffer = value; }
		}
		#region Altered methods/properties
		public override int ReadEntityBody(byte[] buffer, int size) {
			return 0;
		}
		public override int GetTotalEntityBodyLength() {
			return WorkerBuffer.Length;
		}
		public override int GetPreloadedEntityBody(byte[] buffer, int offset) {
			Buffer.BlockCopy(WorkerBuffer, 0, buffer, offset, WorkerBuffer.Length);
			return WorkerBuffer.Length;
		}
		public override byte[] GetPreloadedEntityBody() {
			return WorkerBuffer;
		}
		public override int GetPreloadedEntityBodyLength() {
			return WorkerBuffer.Length;
		}
		public override int ReadEntityBody(byte[] buffer, int offset, int size) {
			return 0;
		}
		public override string GetKnownRequestHeader(int index) {
			if(index == HttpWorkerRequest.HeaderContentLength) {
				return WorkerBuffer.Length.ToString();
			} else {
				return WorkerRequest.GetKnownRequestHeader(index);
			}
		}
		public override bool IsEntireEntityBodyIsPreloaded() {
			return true;
		}
		#endregion
		#region Core HttpWorkerRequest methods/properties passed to the internal request without change
		public override void CloseConnection() {
			workerRequest.CloseConnection();
		}
		public override void EndOfRequest() {
			workerRequest.EndOfRequest();
		}
		public override bool Equals(object obj) {
			return workerRequest.Equals(obj);
		}
		public override void FlushResponse(bool finalFlush) {
			workerRequest.FlushResponse(finalFlush);
		}
		public override string GetAppPath() {
			return workerRequest.GetAppPath();
		}
		public override string GetAppPathTranslated() {
			return workerRequest.GetAppPathTranslated();
		}
		public override string GetAppPoolID() {
			return workerRequest.GetAppPoolID();
		}
		public override long GetBytesRead() {
			return workerRequest.GetBytesRead();
		}
		public override byte[] GetClientCertificate() {
			return workerRequest.GetClientCertificate();
		}
		public override byte[] GetClientCertificateBinaryIssuer() {
			return workerRequest.GetClientCertificateBinaryIssuer();
		}
		public override int GetClientCertificateEncoding() {
			return workerRequest.GetClientCertificateEncoding();
		}
		public override byte[] GetClientCertificatePublicKey() {
			return workerRequest.GetClientCertificatePublicKey();
		}
		public override DateTime GetClientCertificateValidFrom() {
			return workerRequest.GetClientCertificateValidFrom();
		}
		public override DateTime GetClientCertificateValidUntil() {
			return workerRequest.GetClientCertificateValidUntil();
		}
		public override long GetConnectionID() {
			return workerRequest.GetConnectionID();
		}
		public override string GetFilePath() {
			return workerRequest.GetFilePath();
		}
		public override string GetFilePathTranslated() {
			return workerRequest.GetFilePathTranslated();
		}
		public override int GetHashCode() {
			return workerRequest.GetHashCode();
		}
		public override string GetHttpVerbName() {
			return workerRequest.GetHttpVerbName();
		}
		public override string GetHttpVersion() {
			return workerRequest.GetHttpVersion();
		}
		public override string GetLocalAddress() {
			return workerRequest.GetLocalAddress();
		}
		public override int GetLocalPort() {
			return workerRequest.GetLocalPort();
		}
		public override string GetPathInfo() {
			return workerRequest.GetPathInfo();
		}
		public override string GetProtocol() {
			return workerRequest.GetProtocol();
		}
		public override string GetQueryString() {
			return workerRequest.GetQueryString();
		}
		public override byte[] GetQueryStringRawBytes() {
			return workerRequest.GetQueryStringRawBytes();
		}
		public override string GetRawUrl() {
			return workerRequest.GetRawUrl();
		}
		public override string GetRemoteAddress() {
			return workerRequest.GetRemoteAddress();
		}
		public override string GetRemoteName() {
			return workerRequest.GetRemoteName();
		}
		public override int GetRemotePort() {
			return workerRequest.GetRemotePort();
		}
		public override int GetRequestReason() {
			return workerRequest.GetRequestReason();
		}
		public override string GetServerName() {
			return workerRequest.GetServerName();
		}
		public override string GetServerVariable(string name) {
			return workerRequest.GetServerVariable(name);
		}
		public override string GetUnknownRequestHeader(string name) {
			return workerRequest.GetUnknownRequestHeader(name);
		}
		public override string[][] GetUnknownRequestHeaders() {
			return workerRequest.GetUnknownRequestHeaders();
		}
		public override string GetUriPath() {
			return workerRequest.GetUriPath();
		}
		public override long GetUrlContextID() {
			return workerRequest.GetUrlContextID();
		}
		public override IntPtr GetUserToken() {
			return workerRequest.GetUserToken();
		}
		public override IntPtr GetVirtualPathToken() {
			return workerRequest.GetVirtualPathToken();
		}
		public override bool HeadersSent() {
			return workerRequest.HeadersSent();
		}
		public override bool IsClientConnected() {
			return workerRequest.IsClientConnected();
		}
		public override bool IsSecure() {
			return workerRequest.IsSecure();
		}
		public override string MapPath(string virtualPath) {
			return workerRequest.MapPath(virtualPath);
		}
		public override void SendCalculatedContentLength(int contentLength) {
			workerRequest.SendCalculatedContentLength(contentLength);
		}
		public override void SendKnownResponseHeader(int index, string value) {
			workerRequest.SendKnownResponseHeader(index, value);
		}
		public override void SendResponseFromFile(IntPtr handle, long offset, long length) {
			workerRequest.SendResponseFromFile(handle, offset, length);
		}
		public override void SendResponseFromFile(string filename, long offset, long length) {
			workerRequest.SendResponseFromFile(filename, offset, length);
		}
		public override void SendResponseFromMemory(byte[] data, int length) {
			workerRequest.SendResponseFromMemory(data, length);
		}
		public override void SendResponseFromMemory(IntPtr data, int length) {
			workerRequest.SendResponseFromMemory(data, length);
		}
		public override void SendStatus(int statusCode, string statusDescription) {
			workerRequest.SendStatus(statusCode, statusDescription);
		}
		public override void SendUnknownResponseHeader(string name, string value) {
			workerRequest.SendUnknownResponseHeader(name, value);
		}
		public override void SetEndOfSendNotification(HttpWorkerRequest.EndOfSendNotification callback, object extraData) {
			workerRequest.SetEndOfSendNotification(callback, extraData);
		}
		public override string ToString() {
			return workerRequest.ToString();
		}
		public override string MachineConfigPath {
			get {
				return workerRequest.MachineConfigPath;
			}
		}
		public override string MachineInstallDirectory {
			get {
				return workerRequest.MachineInstallDirectory;
			}
		}
		public override Guid RequestTraceIdentifier {
			get {
				return workerRequest.RequestTraceIdentifier;
			}
		}
		public override string RootWebConfigPath {
			get {
				return workerRequest.RootWebConfigPath;
			}
		}
		#endregion
	}
}

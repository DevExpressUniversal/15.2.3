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

using DevExpress.Web;
using DevExpress.Web.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
namespace DevExpress.Web.Office.Internal {
	public delegate void RequestProcessingEventHandler(DocumentRequestProcessingEventArgs e);
	public class DocumentRequestProcessingEventArgs : EventArgs {
		public Guid SessionGuid { get; private set; }
		public WorkSessionBase WorkSession { get; private set; }
		public DocumentRequestProcessingEventArgs(Guid sessionGuid, WorkSessionBase workSession) {
			SessionGuid = sessionGuid;
			WorkSession = workSession;
		}
	}
	public static class DocumentRequestManager {
		public const string DefaultResponseContentType = "application/json";
		public static void ProcessRequest() {
			HttpRequest request = HttpUtils.GetRequest();
			Guid sessionGuid = DocumentRequestHelper.GetSessionGuid(request);
			DocumentHandlerResponse requestResult = null;
			WorkSessionBase workSession = WorkSessions.Get(sessionGuid, true);
			RaisesRequestProcessing(sessionGuid, workSession);
			if(workSession != null) {
				lock(workSession) {
					NameValueCollection postData = DocumentRequestHelper.GetPostData(request);
					requestResult = workSession.ProcessRequest(postData);
				}
				HttpResponse response = requestResult.PrepareResponse();
				if(response != null)
					response.End();
			}
		}
		public static event RequestProcessingEventHandler RequestProcessing;
		static void RaisesRequestProcessing(Guid sessionGuid, WorkSessionBase workSession) {
			DocumentRequestProcessingEventArgs e = new DocumentRequestProcessingEventArgs(sessionGuid, workSession);
			if(RequestProcessing != null)
				RequestProcessing(e);
		}
	}
	public class DocumentWorkSessionManagerSubscriber : IHttpModuleSubscriber {
		const string HandlerName = "DXS.axd";
		public bool RequestRecipient(HttpRequest request, RequestEvent requestEvent) {
			return requestEvent == RequestEvent.BeginRequest &&
				request.Path.EndsWith(HandlerName, StringComparison.OrdinalIgnoreCase);
		}
		public void ProcessRequest() {
			DocumentRequestManager.ProcessRequest();
		}
	}
	public static class DocumentRequestHelper {
		public static readonly string SessionGuidStateKey = "s";
		public static NameValueCollection GetPostData(HttpRequest request) {
			bool methodPost = true;
			return methodPost ? request.Params : request.QueryString;
		}
		public static Guid GetSessionGuid(HttpRequest request) {
			NameValueCollection postData = GetPostData(request);
			return GetSessionGuid(postData);
		}
		public static Guid GetSessionGuid(NameValueCollection postData) {
			return GetSessionGuid(postData, DocumentRequestHelper.SessionGuidStateKey);
		}
		public static Guid GetSessionGuid(NameValueCollection postData, string sessionGuidParamName) {
			return GetSessionGuid(postData[sessionGuidParamName]);
		}
		public static Guid GetSessionGuid(string sessionGuidStr) {
			Guid sessionGuid;
			if(Guid.TryParse(sessionGuidStr, out sessionGuid))
				return sessionGuid;
			return Guid.Empty;
		}
	}
	public abstract class DocumentHandlerResponse {
		protected string responseContentType = "application/json";
		protected Encoding responseContentEncoding = Encoding.UTF8;
		public string ContentType {
			get { return responseContentType; }
			set { responseContentType = value; }
		}
		public Encoding ContentEncoding {
			get { return responseContentEncoding; }
			set { responseContentEncoding = value; }
		}
		public abstract HttpResponse PrepareResponse();
		public abstract object GetResponseResult();
	}
	public class JSONDocumentHandlerResponse : DocumentHandlerResponse {
		private string responseResult = string.Empty;
		public string ResponseResult {
			get { return responseResult; }
			set { responseResult = value; }
		}
		public JSONDocumentHandlerResponse() { }
		public override object GetResponseResult() {
			return ResponseResult;
		}
		public override HttpResponse PrepareResponse() {
			HttpResponse response = HttpUtils.GetResponse();
			if(response != null && !string.IsNullOrEmpty(ResponseResult)) {
				HttpUtils.MakeResponseCompressed(true); 
				response.ContentType = ContentType;
				response.ContentEncoding = ContentEncoding;
				response.Write(ResponseResult);
			}
			return response;
		}
	}
	public class AttachmentDocumentHandlerResponse : DocumentHandlerResponse {
		private ResponseFileInfo responseFile = null;
		private bool autodetectContentType = false;
		public ResponseFileInfo ResponseFile {
			get {
				if(responseFile == null)
					responseFile = new ResponseFileInfo("ResponseFile.pdf", new byte[] { }) { AsAttachment = true };
				return responseFile;
			}
			set { responseFile = value; }
		}
		public bool AutodetectContentType {
			get { return autodetectContentType; }
			set { autodetectContentType = value; }
		}
		public AttachmentDocumentHandlerResponse() {
			responseContentType = "application/pdf";
			responseContentEncoding = Encoding.Default;
		}
		public override object GetResponseResult() {
			return ResponseFile.FileStreamArray;
		}
		public override HttpResponse PrepareResponse() {
			HttpResponse response = HttpUtils.GetResponse();
			if(response != null && ResponseFile.Length > 0) {
				response.Clear();
				response.ClearHeaders();
				response.Buffer = false;
				response.ContentEncoding = ContentEncoding;
				if(AutodetectContentType && Path.HasExtension(ResponseFile.FileName)) {
					ContentType = HttpUtils.GetContentType(Path.GetExtension(ResponseFile.FileName));
				}
				response.AppendHeader("Content-Type", ContentType);
				response.AppendHeader("Content-Length", ResponseFile.Length.ToString());
				response.AppendHeader("Content-Transfer-Encoding", "binary");
				response.AppendHeader("Content-Disposition",
					string.Format("{0}; filename={1}{2}{1}",
						ResponseFile.AsAttachment ? "attachment" : "inline",
						RenderUtils.Browser.IsIE ? "" : "\"",
						RenderUtils.Browser.IsIE ? HttpUtils.EncodeFileName(ResponseFile.FileName) : ResponseFile.FileName));
				response.BinaryWrite(ResponseFile.FileStreamArray);
			}
			return response;
		}
	}
	public class BinaryDocumentHandlerResponse : DocumentHandlerResponse {
		private byte[] binaryBuffer;
		public byte[] BinaryBuffer {
			get { return binaryBuffer; }
			set { binaryBuffer = value; }
		}
		public BinaryDocumentHandlerResponse() {
			responseContentType = "image/png";
			responseContentEncoding = Encoding.Default;
			BinaryBuffer = new byte[] { };
		}
		public override object GetResponseResult() {
			return BinaryBuffer;
		}
		public override HttpResponse PrepareResponse() {
			HttpResponse response = HttpUtils.GetResponse();
			if(response != null && BinaryBuffer.LongLength > 0) {
				response.Clear();
				response.ClearHeaders();
				response.BufferOutput = true;
				response.ContentType = ContentType;
				response.BinaryWrite(BinaryBuffer);
				response.Flush();
			}
			return response;
		}
	}
	public class ResponseFileInfo {
		public string FileName { get; private set; }
		public byte[] FileStreamArray { get; private set; }
		public bool AsAttachment { get; set; }
		public long Length { get { return FileStreamArray.LongLength; } }
		public ResponseFileInfo(string fileName, byte[] streamArray) {
			FileName = fileName;
			FileStreamArray = streamArray;
		}
	}
}

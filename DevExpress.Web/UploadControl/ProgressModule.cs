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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Configuration;
using System.Web.Routing;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class ASPxUploadProgressHttpHandler : IHttpHandler {
		bool IHttpHandler.IsReusable {
			get { return false; }
		}
		void IHttpHandler.ProcessRequest(HttpContext context) {
			string key = context.Request[RenderUtils.ProgressHandlerKeyQueryParamName];
			UploadProgressStatus status = null;
			if(IsHelperUploading()) {
				HelperUploadManager uploadManager = new HelperUploadManager(context, key);
				status = uploadManager.ProcessRequest();
			}
			else
				status = UploadProgressManager.GetStatus(key);
			HttpResponse response = context.Response;
			response.Cache.SetCacheability(HttpCacheability.NoCache);
			response.ContentType = "text/xml";
			if(status != null)
				response.Write(UploadProgressStatus.GetResponseString(status));
			else
				response.Write(UploadProgressStatus.EMPTY_STATUS);
			response.Flush();
		}
		private bool IsHelperUploading() {
			return !string.IsNullOrEmpty(ASPxUploadControl.GetHelperUploadingCallbackParam(HttpUtils.GetRequest()));
		}
	}
	public class UploadProgressManager {
		protected internal const string UploadingMarker = "multipart/form-data; boundary=";
		protected internal const int BufferSize = 4096;
		public static void SetStatus(string uploadKey, UploadProgressStatus status) {
			HttpContext.Current.Application[RenderUtils.ProgressHandlerKeyQueryParamName + uploadKey] = status;
		}
		public static UploadProgressStatus GetStatus(string uploadKey) {
			return HttpContext.Current.Application[RenderUtils.ProgressHandlerKeyQueryParamName + uploadKey] as UploadProgressStatus;
		}
		public static bool IsProcessRequestAllowed() {
			return IsUploading() && IsProgressProcessingEnabled() && IsValidUrl();
		}
		public static bool ProcessRequest(ref RequestProcessState state) {
			string progressKey = GetUploadingProgressKey();
			bool result;
			try {
				result = ProcessRequestInternal(ref state, progressKey);
			}
			catch(SecurityException) {
				result = false;
			}
			return result;
		}
		protected static void UpdateProgressManager(UploadProgressStatus progressStatus, RequestParser requestParser,
			int headerContentLength, int counter, ref bool statusPersisted) {
			string progressKey = GetUploadingProgressKey();
			if (!String.IsNullOrEmpty(progressKey)) {
				if (!statusPersisted) {
					UploadProgressManager.SetStatus(progressKey, progressStatus);
					statusPersisted = true;
				}
				progressStatus.UpdateStatus(requestParser.CurrentFile, requestParser.CurrentContentType, counter, headerContentLength);
			}
		}
		private static bool ProcessRequestInternal(ref RequestProcessState state, string progressKey) {
			HttpWorkerRequest worker = null;
			try {
				worker = GetWorkerRequest(HttpContext.Current);
			} catch {
				return false;
			}
			string httpRequestHeader = worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentType);
			state = RequestProcessState.Processed;
			if (httpRequestHeader != null && IsUploadingMarkerExist(httpRequestHeader)) {
				HttpRequest request = HttpUtils.GetRequest();
				int headerContentLength = int.Parse(worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentLength));
				string partSeparator = "--" + httpRequestHeader.Replace(UploadingMarker, "");
				if(IsMaxRequestLengthExceeded(headerContentLength)) {
					return false;
				}
				using (RequestParser requestParser = new RequestParser(partSeparator, request.ContentEncoding)) {
					UploadProgressStatus progressStatus = new UploadProgressStatus(headerContentLength);
					bool statusPersisted = false;
					byte[] data = null;
					int bufferSize = BufferSize;
					int loadedByteNumber = 0;
					int counter = 0;
					int preloadedEntityBodyLength = worker.GetPreloadedEntityBodyLength();
					if (preloadedEntityBodyLength > 0) {
						data = worker.GetPreloadedEntityBody();
						requestParser.Write(data, preloadedEntityBodyLength);
						UpdateProgressManager(progressStatus, requestParser,
							headerContentLength, data.Length, ref statusPersisted);
						bufferSize = data.Length;
						counter = data.Length;
					}
					bool disconnected = false;
					while (counter < headerContentLength && worker.IsClientConnected() && !disconnected) {
						System.Threading.Thread.Sleep(0);
						if (bufferSize > (headerContentLength - counter))
							bufferSize = headerContentLength - counter;
						data = new byte[bufferSize];
						loadedByteNumber = worker.ReadEntityBody(data, bufferSize);
						if (loadedByteNumber > 0) {
							requestParser.Write(data, loadedByteNumber);
							counter += loadedByteNumber;
							UpdateProgressManager(progressStatus, requestParser,
								headerContentLength, counter, ref statusPersisted);
						} else
							disconnected = true;
					}
					UploadProgressManager.SetStatus(progressKey, null);
					if (!worker.IsClientConnected() || disconnected) {
						HttpUtils.EndResponse();
						return true;
					}
					BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
					UploadWorkerRequest workerRequest = new UploadWorkerRequest(worker, requestParser.ContentBody);
					request.GetType().GetField("_wr", bindingFlags).SetValue(request, workerRequest);
				}
			}
			return true;
		}
		[System.Security.SecuritySafeCritical]
		private static HttpWorkerRequest GetWorkerRequest(HttpContext context) {
			IServiceProvider provider = (IServiceProvider)context;
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
			return (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
		}
		private static bool IsUploading() {
			return !string.IsNullOrEmpty(ASPxUploadControl.GetUploadingCallbackParam(HttpUtils.GetRequest()));
		}
		private static bool IsValidUrl() { 
			if(IsMVCMarkerExist())
				return true;
			string url = HttpUtils.GetRequest().Url.AbsolutePath;
			return IsValidDirectPageUrl(url) || HasRoute();	
		}
		static bool IsValidDirectPageUrl(string url) {
			System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@".*?(\.[^.\\\/:*?""<>|]+$)");
			return reg.IsMatch(url);
		}
		static bool HasRoute() {
			HttpContextWithoutAuthorization contextWrapper = new HttpContextWithoutAuthorization(HttpContext.Current);
			RouteData route = RouteTable.Routes.GetRouteData(contextWrapper);
			return route != null;
		}
		static bool IsProgressProcessingEnabled() {
			string progressKey = GetUploadingProgressKey();
			return !string.IsNullOrEmpty(progressKey);
		}
		private static string GetUploadingProgressKey() {
			return ASPxUploadControl.GetProgressInfoKey(HttpUtils.GetRequest());
		}
		private static bool IsMVCMarkerExist() {
			return !String.IsNullOrEmpty(HttpUtils.GetRequest().QueryString[RenderUtils.MVCQueryParamName]);
		}
		private static bool IsUploadingMarkerExist(string httpRequestHeader) {
			return string.Compare(httpRequestHeader, 0, UploadingMarker, 0, UploadingMarker.Length, true, CultureInfo.InvariantCulture) == 0;
		}
		private static bool IsMaxRequestLengthExceeded(int requestLength) {
			var maxRequestLengthBytes = HttpUtils.GetWebConfigMaxRequestLengthBytes();
			return maxRequestLengthBytes > -1 && requestLength > maxRequestLengthBytes;
		}
	}
	public class UploadProgressStatus {
		private string errorText = "";
		private string fileName = "";
		private long fileSize = 0;
		private long fileUploadedSize = 0;
		private int fileProgress = 0;
		private string contentType = "";
		private long totalUploadedSize = 0;
		private long totalSize = 0;
		private int progress = 0;
		public static string EMPTY_STATUS = "<status empty='true'></status>";
		public UploadProgressStatus(int requestSize) {
			this.totalSize = requestSize;
		}
		public UploadProgressStatus(string errorText) {
			this.errorText = errorText;
		}
		public string ErrorText {
			get { return errorText; }
		}
		public string FileName {
			get { return fileName; }
		}
		public long FileSize {
			get { return fileSize; }
		}
		public long FileUploadedSize {
			get { return fileUploadedSize; }
		}
		public int FileProgress {
			get { return fileProgress; }
		}
		public string ContentType {
			get { return contentType; }
		}
		public long TotalUploadedSize {
			get { return totalUploadedSize; }
		}
		public long TotalSize {
			get { return totalSize; }
		}
		public int Progress {
			get { return progress; }
		}
		public void UpdateStatus(string fileName, string contentType, long size, long totalSize) {
			UpdateStatus(fileName, 0, 0, contentType, size, totalSize);
		}
		public void UpdateStatus(string fileName, long fileSize, long fileUploadedSize, string contentType, long totalUploadedSize, long totalSize) {
			this.fileName = fileName;
			this.fileSize = fileSize;
			this.fileUploadedSize = fileUploadedSize;
			if(fileSize == 0)
				this.fileProgress = 100;
			else
				this.fileProgress = (int)((double)fileUploadedSize / (double)fileSize * 100);
			this.contentType = contentType;
			this.totalUploadedSize = totalUploadedSize;
			this.totalSize = totalSize;
			if(totalSize == 0)
				this.progress = 100;
			else
				this.progress = (int)((double)totalUploadedSize / (double)totalSize * 100);
		}
		protected internal static string GetResponseString(UploadProgressStatus status) {
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<status></status>");
			if(!string.IsNullOrEmpty(status.ErrorText))
				AddAttribute(doc, doc.DocumentElement, "errorText", status.ErrorText);
			else {
				AddAttribute(doc, doc.DocumentElement, "fileName", System.IO.Path.GetFileName(status.FileName));
				AddAttribute(doc, doc.DocumentElement, "fileSize", status.FileSize.ToString());
				AddAttribute(doc, doc.DocumentElement, "fileUploadedSize", status.FileUploadedSize.ToString());
				AddAttribute(doc, doc.DocumentElement, "fileProgress", status.FileProgress.ToString());
				AddAttribute(doc, doc.DocumentElement, "contentType", status.ContentType);
				AddAttribute(doc, doc.DocumentElement, "totalUploadedSize", status.TotalUploadedSize.ToString());
				AddAttribute(doc, doc.DocumentElement, "totalSize", status.TotalSize.ToString());
				AddAttribute(doc, doc.DocumentElement, "progress", status.Progress.ToString());
			}
			return doc.OuterXml;
		}
		protected internal static void AddAttribute(XmlDocument doc, XmlNode node, string name, string value) {
			XmlAttribute attr = doc.CreateAttribute(name);
			attr.Value = value;
			node.Attributes.Append(attr);
		}
	}
	class HttpContextWithoutAuthorization : HttpContextWrapper {
		public override bool SkipAuthorization {
			get { return true; }
			set { }
		}
		public HttpContextWithoutAuthorization(HttpContext httpContext) : base(httpContext) {
		}
	}
}

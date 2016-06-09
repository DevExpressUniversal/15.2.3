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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.Util;
namespace DevExpress.Web.Internal {
	public static class HttpUtils {
		public const string HttpHandlerModuleName = "ASPxHttpHandlerModule";
		public const string UploadProgressHttpHandlerName = "ASPxUploadProgressHandler";
		public const string UploadProgressHttpHandlerPagePath = "ASPxUploadProgressHandlerPage.ashx";
		private const string HttpRuntimeSectionPath = @"system.web/httpRuntime";
		public static HttpRequest GetRequest() {
			return GetRequest((Page)null);
		}
		public static HttpRequest GetRequest(Control control) {
			return GetRequest(control.Page);
		}
		public static HttpRequest GetRequest(Page page) {
			if(HttpContext.Current != null)
				return HttpContext.Current.Request;
			else if(page != null)
				return page.Request;
			else
				return null;
		}
		public static int GetWebConfigMaxRequestLengthBytes() {
			return HttpUtils.BytesFromKilobytes(GetWebConfigMaxRequestLengthKBytes());
		}
		public static int GetWebConfigMaxRequestLengthKBytes() {
			HttpRuntimeSection section = (HttpRuntimeSection)WebConfigurationManager.GetSection(HttpRuntimeSectionPath);
			return section.MaxRequestLength;
		}
		public static string GetValueFromRequest(HttpRequest request, string key) {
			return GetValueFromRequest(request, key, false);
		}
		public static string GetValueFromRequest(string key) {
			return GetValueFromRequest(key, false);
		}
		public static string GetValueFromRequest(string key, bool skipValidation) {
			return GetValueFromRequest(HttpContext.Current.Request, key, skipValidation);
		}
		public static string GetValueFromRequest(HttpRequest request, string key, bool skipValidation) {
			var unvalidatedRequest = GetUnvalidatedRequest(request);
			if(DXValidatorType == null || unvalidatedRequest == null || string.IsNullOrEmpty(request.UserAgent))
				return request.Params[key];
			var value = GetValueFromUnvalidatedRequest(unvalidatedRequest, key);
			ValidateRequestValue(DXValidatorType, value, skipValidation);
			return value;
		}
		static string GetValueFromUnvalidatedRequest(object unvalidatedRequest, string key) {
			var methodInfo = unvalidatedRequest.GetType().GetMethod("get_Item");
			return methodInfo != null ? (string)methodInfo.Invoke(unvalidatedRequest, new object[] { key }) : null;
		}
		static void ValidateRequestValue(Type validatorType, object value, bool skipValidation) {
			if(skipValidation)
				return;
			var methodInfo = validatorType.GetMethod("ValidateRequestString");
			methodInfo.Invoke(validatorType, new object[] { value });
		}
		public static object GetUnvalidatedRequest(HttpRequest request) {
			var requestType = typeof(HttpRequest).GetProperty("Unvalidated");
			if(requestType == null)
				return null;
			return requestType.GetValue(request, null);
		}
		static Type dxValidatorType;
		static Type DXValidatorType {
			get {
				if(!IsDXValidatorTypeLoaded) {
					const string FullMVCAssemblyName = AssemblyInfo.SRAssemblyMVC + AssemblyInfo.FullAssemblyVersionExtension;
					const string FullMVC5AssemblyName = AssemblyInfo.SRAssemblyMVC5 + AssemblyInfo.FullAssemblyVersionExtension;
					var assembly = AppDomain.CurrentDomain
						.GetAssemblies()
						.Where(i => i.FullName == FullMVCAssemblyName || i.FullName == FullMVC5AssemblyName)
						.FirstOrDefault();
					if(assembly != null)
						dxValidatorType = assembly.GetType("DevExpress.Web.Mvc.Internal.DevExpressRequestValidator");
					IsDXValidatorTypeLoaded = true;
				}
				return dxValidatorType;
			}
		}
		static bool IsDXValidatorTypeLoaded { get; set; }
		public static HttpResponse GetResponse() {
			return GetResponse((Page)null);
		}
		public static HttpResponse GetResponse(Control control) {
			return GetResponse(control.Page);
		}
		public static HttpResponse GetResponse(Page page) {
			if(HttpContext.Current != null)
				return HttpContext.Current.Response;
			else if(page != null)
				return page.Response;
			else
				return null;
		}
		public static NameValueCollection GetQuery() {
			return GetQuery((Page)null);
		}
		public static NameValueCollection GetQuery(Control control) {
			return GetQuery(control.Page);
		}
		public static NameValueCollection GetQuery(Page page) {
			HttpRequest request = GetRequest(page);
			return request != null ? request.QueryString : null;
		}
		public static Cache GetCache() {
			return GetCache((Page)null);
		}
		public static Cache GetCache(Control control) {
			Page page = control != null ? control.Page : null;
			return GetCache(page);
		}
		public static Cache GetCache(Page page) {
			if(HttpContext.Current != null)
				return HttpContext.Current.Cache;
			else if(page != null)
				return page.Cache;
			else
				return null;
		}
		public static HttpSessionState GetSession() {
			return GetSession((Page)null);
		}
		public static HttpSessionState GetSession(Control control) {
			return GetSession(control == null ? null : control.Page);
		}
		public static HttpSessionState GetSession(Page page) {
			if(HttpContext.Current != null)
				return HttpContext.Current.Session;
			else if(page != null)
				return page.Session;
			else
				return null;
		}
		public static string GetApplicationUrl(Control control) {
			return GetApplicationUrl(control.Page);
		}
		public static string GetApplicationUrl(Page page) {
			string url = string.Empty;
			HttpRequest request = HttpUtils.GetRequest(page);
			if(request != null) {
				if(request.ApplicationPath != "/")
					url += request.ApplicationPath + "/";
				else {
#if DebugTest
					int pos = request.Path.LastIndexOf("/");
					if(pos > -1)
						url += request.Path.Substring(0, pos + 1);
#else
					url += request.ApplicationPath;
#endif
				}
			}
			return url;
		}
		public static void EndResponse() {
			HttpContext.Current.ApplicationInstance.CompleteRequest();
			try {
				GetResponse().End();
			}
			catch(System.Threading.ThreadAbortException) {
			}
		}
		public static void WriteToResponse(string text) {
			HttpResponse response = GetResponse();
			response.Clear();
			response.AddHeader("Content-Type", "text/html; charset=utf-8");
			response.Output.Write(text);
		}
		public static bool IsUpdatePanelCallback() {
			HttpRequest request = GetRequest();
			try {
				return (request != null) ? ((request["HTTP_X_MICROSOFTAJAX"] != null) || ((request.Headers != null) && (request.Headers["HTTP_X_MICROSOFTAJAX"] != null))) : false;
			}
			catch {
			}
			return false;
		}
		public static bool IsMicrosoftAjaxCallback() {
			HttpRequest request = GetRequest();
			return (request != null) ? ((request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest"))) : false;
		}
		public static bool IsUploadControlCallback() {
			HttpRequest request = GetRequest();
			return (request != null) ? request[RenderUtils.UploadingCallbackQueryParamName] != null : false;
		}
		public static bool IsCustomErrorEnabled() {
			return HttpContext.Current != null ? HttpContext.Current.IsCustomErrorEnabled : true;
		}
		public static void MakeResponseCompressed(bool compressNonPageRequests) {
			MakeResponseCompressed(compressNonPageRequests, false);
		}
		public static void MakeResponseCompressed(bool compressNonPageRequests, bool applyToIE6) {
			if(HttpUtils.GetContextValue<bool>("IsResponseCompressed", false)) return;
			HttpRequest request = GetRequest();
			HttpResponse response = GetResponse();
			if(request != null && response != null && response.Filter != null && !IsUpdatePanelCallback()) {
				if((HttpContext.Current.CurrentHandler is Page) || compressNonPageRequests) {
					string acceptEncoding = request.Headers["Accept-Encoding"];
					if(!string.IsNullOrEmpty(acceptEncoding)) {
						acceptEncoding = acceptEncoding.ToLower();
						if(acceptEncoding.IndexOf("gzip") > -1) {
							response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
							response.AppendHeader("Content-Encoding", "gzip");
						}
						else if(acceptEncoding.IndexOf("deflate") > -1) {
							response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
							response.AppendHeader("Content-Encoding", "deflate");
						}
					}
				}
			}
			HttpUtils.SetContextValue("IsResponseCompressed", true);
		}
		public static T GetContextObject<T>(string key) {
			if(HttpContext.Current == null)
				return (T)Activator.CreateInstance(typeof(T));
			object obj = HttpContext.Current.Items[key];
			if(obj == null) {
				obj = Activator.CreateInstance(typeof(T));
				HttpContext.Current.Items[key] = obj;
			}
			return (T)obj;
		}
		public static T GetContextValue<T>(object key, T defaultValue) {
			HttpContext context = HttpContext.Current;
			return (context != null && context.Items.Contains(key)) ? (T)context.Items[key] : defaultValue;
		}
		public static void SetContextValue<T>(object key, T value) {
			HttpContext context = HttpContext.Current;
			if(HttpContext.Current != null)
				HttpContext.Current.Items[key] = value;
		}
		public static int BytesFromKilobytes(int kilobytes) {
			long bytes = kilobytes * 0x400L;
			if(bytes >= 0x7fffffffL)
				return 0x7fffffff;
			return (int)bytes;
		}
		public static void WriteFileToResponse(Page page, MemoryStream stream, string fileName, bool saveAsFile, string fileFormat) {
			WriteFileToResponse(page, stream, fileName, saveAsFile, fileFormat, GetContentType(fileFormat));
		}
		public static void WriteFileToResponse(Page page, MemoryStream stream, string fileName, bool saveAsFile, string fileFormat, string contentType) {
			HttpResponse response = PrepareFileResponse(page, fileName, saveAsFile, fileFormat, contentType, stream.Length);
			if(response == null || !stream.CanRead) return;
			if(stream.Length > 0)
				response.BinaryWrite(stream.ToArray());
			response.End();
		}
		public static void WriteFileToResponse(Page page, Stream stream, string fileName, bool saveAsFile, string fileFormat, string contentType, bool splitResponse) {
			HttpResponse response = PrepareFileResponse(page, fileName, saveAsFile, fileFormat, contentType, stream.Length);
			if(response == null || !stream.CanRead) return;
			if(stream.Length > 0) {
				if(splitResponse || stream.Length > int.MaxValue) {
					long remainLength = stream.Length;
					int bufferSize = 100000;
					byte[] buffer = new Byte[bufferSize];
					int length;
					while(remainLength > 0) {
						if(!response.IsClientConnected)
							break;
						length = stream.Read(buffer, 0, bufferSize);
						response.OutputStream.Write(buffer, 0, length);
						response.Flush();
						buffer = new Byte[bufferSize];
						remainLength -= length;
					}
				}
				else
					response.BinaryWrite(new BinaryReader(stream).ReadBytes((int)stream.Length));
			}
			response.End();
		}
		static HttpResponse PrepareFileResponse(Page page, string fileName, bool saveAsFile, string fileFormat, string contentType, long length) {
			HttpResponse response = HttpUtils.GetResponse(page);
			if(response == null) return null;
			response.Clear();
			response.ClearHeaders();
			response.Buffer = false;
			response.AppendHeader("Content-Type", contentType);
			response.AppendHeader("Content-Transfer-Encoding", "binary");
			response.AppendHeader("Content-Length", length.ToString());
			response.AppendHeader(
				"Content-Disposition",
				string.Format(
					"{0}; filename={1}{2}.{3}{1}",
					saveAsFile ? "attachment" : "inline",
					RenderUtils.Browser.IsIE ? "" : "\"",
					RenderUtils.Browser.IsIE ? EncodeFileName(fileName) : fileName,
					fileFormat
				)
			);
			return response;
		}
		public static string EncodeFileName(string fileName) {
			return HttpUtility.UrlEncode(fileName).Replace("+", "%20");
		}
		static Dictionary<string, string> contentTypes;
		static Dictionary<string, string> ContentTypes {
			get {
				if(contentTypes == null) {
					contentTypes = new Dictionary<string, string>();
					contentTypes.Add("rtf", "text/enriched");
					contentTypes.Add("html", "text/html");
					contentTypes.Add("txt", "text/plain");
					contentTypes.Add("csv", "text/csv");
					contentTypes.Add("xml", "text/xml");
					contentTypes.Add("mht", "multipart/related");
					contentTypes.Add("pdf", "application/pdf");
					contentTypes.Add("doc", "application/msword");
					contentTypes.Add("docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
					contentTypes.Add("xls", "application/vnd.ms-excel");
					contentTypes.Add("xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
					contentTypes.Add("ppt", "application/vnd.ms-powerpoint");
					contentTypes.Add("pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
					contentTypes.Add("odt", "application/vnd.oasis.opendocument.text");
					contentTypes.Add("ods", "application/vnd.oasis.opendocument.spreadsheet");
					contentTypes.Add("odp", "application/vnd.oasis.opendocument.presentation");
					contentTypes.Add("bmp", "image/x-ms-bmp"); 
					contentTypes.Add("emf", "image/x-emf");
					contentTypes.Add("wmf", "image/x-wmf");
					contentTypes.Add("gif", "image/gif");
					contentTypes.Add("ico", "image/vnd.microsoft.icon");
					contentTypes.Add("jpg", "image/jpeg");
					contentTypes.Add("jpeg", "image/jpeg");
					contentTypes.Add("png", "image/png");
					contentTypes.Add("tif", "image/tif");
					contentTypes.Add("tiff", "image/tiff");
				}
				return contentTypes;
			}
		}
		public static string GetContentType(string fileFormat) {
			fileFormat = fileFormat.ToLower().TrimStart('.');
			if(ContentTypes.ContainsKey(fileFormat))
				return ContentTypes[fileFormat];
			return "application/" + fileFormat;
		}
		public static bool IsAzureEnvironment() {
			try {
				return !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
			}
			catch (Exception) {
				return false;
			}
		}
	}
}

#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Web {
	#if (DebugTest)
	public static class ErrorInfoHelper {
		public static bool GetIgnoreError(ErrorInfo errorInfo) {
		   return errorInfo.IgnoreError;	  
	   }
		public static void SetIgnoreError(ErrorInfo errorInfo, bool value) {
		   errorInfo.IgnoreError = value;
	   }
	}
	#endif
	public interface IErrorPageTemplate {
		void Init(ErrorInfo info);
	}
	public class ErrorInfoFormattingObject {
		private string applicationName;
		private string exceptionMessage;
		public ErrorInfoFormattingObject(string applicationName, string exceptionMessage) {
			this.applicationName = applicationName;
			this.exceptionMessage = exceptionMessage;
		}
		public string ApplicationName {
			get { return applicationName; }
		}
		public string ExceptionMessage {
			get { return exceptionMessage; }
		}
	}
	public class CustomSendErrorNotificationEventArgs : HandledEventArgs {
		private string messageSubject;
		private string messageBody;
		private string errorId;
		private string errorText;
		private string errorDetails;
		private Exception exception;
		public CustomSendErrorNotificationEventArgs(string errorId, string errorText, string errorDetails, string messageSubject, string messageBody, Exception exception) {
			this.errorId = errorId;
			this.errorText = errorText;
			this.errorDetails = errorDetails;
			this.messageSubject = messageSubject;
			this.messageBody = messageBody;
			this.exception = exception;
		}
		public string MessageSubject {
			get { return messageSubject; }
		}
		public string MessageBody {
			get { return messageBody; }
		}
		public string ErrorId {
			get { return errorId; }
		}
		public string ErrorText {
			get { return errorText; }
		}
		public string ErrorDetails {
			get { return errorDetails; }
		}
		public Exception Exception {
			get { return exception; }
		}
	}
	public class CustomSendMailMessageEventArgs : CustomSendErrorNotificationEventArgs {
		private SmtpClient smtp;
		private MailMessage mailMessage;
		public CustomSendMailMessageEventArgs(CustomSendErrorNotificationEventArgs baseArgs, SmtpClient smtp, MailMessage mailMessage)
			: base(baseArgs.ErrorId, baseArgs.ErrorText, baseArgs.ErrorDetails, baseArgs.MessageSubject, baseArgs.MessageBody, baseArgs.Exception) {
			this.smtp = smtp;
			this.mailMessage = mailMessage;
		}
		public SmtpClient Smtp {
			get { return smtp; }
		}
		public MailMessage MailMessage {
			get { return mailMessage; }
		}
	}
	public class ErrorInfoFormatTextEventArgs : EventArgs {
		private string text;
		private Exception exception;
		public ErrorInfoFormatTextEventArgs(Exception exception, string text) {
			this.exception = exception;
			this.text = text;
		}
		public string Text {
			get { return text; }
			set { text = value; }
		}
		public Exception Exception {
			get { return exception; }
		}
	}
	public class NeedToCacheErrorInfoEventArgs : EventArgs {
		private ErrorInfo errorInfo;
		bool isCachingNeeded = false;
		public NeedToCacheErrorInfoEventArgs(ErrorInfo errorInfo) {
			this.errorInfo = errorInfo;
		}
		public ErrorInfo ErrorInfo {
			get { return errorInfo; }
		}
		public bool IsCachingNeeded {
			get { return isCachingNeeded; }
			set { isCachingNeeded = value; }
		}
	}
	public class ErrorInfo {
		private string id;
		private string url;
		string urlReferrer;
		private string serializedInfo;
		private Exception exception;
		private string date;
		private string lastTraceEntries;
		private string sessionID;
		private void OnFormatText(ErrorInfoFormatTextEventArgs args) {
			if(FormatText != null) {
				FormatText(this, args);
			}
		}
		public ErrorInfo(Exception exception, string lastTraceEntries) {
			Initialize(exception, lastTraceEntries);
		}
		private void Initialize(Exception exception, string lastTraceEntries) {
			this.date = DateTime.Now.ToString();
			this.id = Guid.NewGuid().ToString();
			this.exception = exception;
			HttpContext context = HttpContext.Current;
			if(context != null) {
				if(context.Session != null) {
					this.sessionID = context.Session.SessionID;
				}
				else {
					this.sessionID = "Session is null";
				}
			}
			StringBuilder builder = new StringBuilder();
			try {
				HttpRequest request = context == null ? null : context.Request;
				if(request != null) {
					this.url = request.Url.ToString();
					if(request.UrlReferrer != null) {
						this.urlReferrer = request.UrlReferrer.ToString();
					}
				}
				builder.AppendFormat("Error ID: {0}", id);
				builder.AppendLine();
				builder.AppendFormat("Date: {0}", date);
				builder.AppendLine();
				if(request != null) {
					builder.AppendLine("Request details:");
					builder.AppendLine("  Url: " + url);
					builder.AppendLine("  Url referrer: " + urlReferrer);
					builder.AppendLine("  SessionID: " + sessionID);
					builder.AppendLine("  Request type: " + request.RequestType);
					builder.AppendLine("  User agent: " + request.UserAgent);
					builder.AppendLine("  User IP: " + request.UserHostAddress);
					builder.AppendLine("  User: " + SecuritySystem.CurrentUserName);
					if(request.IsAuthenticated) {
						string userIdentity = context.User.Identity.Name;
						builder.AppendLine("Authenticated user: " + (string.IsNullOrEmpty(userIdentity) ? "[no name]" : userIdentity));
					}
				}
			}
			catch {
			}
			builder.Append(Tracing.Tracer.FormatExceptionReport(exception));
			ErrorInfoFormatTextEventArgs args = new ErrorInfoFormatTextEventArgs(exception, builder.ToString());
			OnFormatText(args);
			serializedInfo = args.Text;
			this.lastTraceEntries = lastTraceEntries;
		}
		internal bool IgnoreError { get; set; }
		public string GetErrorMessage() {
			ErrorInfoFormatTextEventArgs args = new ErrorInfoFormatTextEventArgs(exception, exception.Message);
			OnFormatText(args);
			return args.Text;
		}
		public string Id { get { return id; } }
		public string Url {
			get { return url; }
		}
		public string UrlReferrer {
			get { return urlReferrer; }
		}
		public Exception Exception {
			get { return exception; }
		}
		public string GetTextualPresentation(bool includeTrace) {
			string result = serializedInfo;
			if(includeTrace) {
				result +=
					"\n\n" +
					"Last trace entries:\n\n" +
					lastTraceEntries;
			}
			return result;
		}
		public string Date { get { return date; } }
		public static event EventHandler<ErrorInfoFormatTextEventArgs> FormatText;
	}
	public class ErrorHandling {
		public const string ExceptionMessageParameter = "ExceptionMessage";
		public const string ApplicationNameParameter = "ApplicationName";
		public const string ConfigRichErrorReportPageKeyName = "RichErrorReportPage";
		public const string ConfigSimpleErrorReportPageKeyName = "SimpleErrorReportPage";
		public const string ConfigSessionLostPageKeyName = "SessionLostPage";
		public const string ConfigErrorReportEmailSubjectKeyName = "ErrorReportEmailSubject";
		public const string ConfigErrorReportEmailKeyName = "ErrorReportEmail";
		public const string ConfigErrorReportEmailServerKeyName = "ErrorReportEmailServer";
		public const string ConfigErrorReportEmailFromKeyName = "ErrorReportEmailFrom";
		public const string ConfigErrorReportEmailFromNameKeyName = "ErrorReportEmailFromName";
		public const string ConfigDetailedErrorReaderIPKeyName = "DetailedErrorReaderIp";
		public const string ConfigErrorReportEnableSsl = "ErrorReportEnableSsl";
		private const string SimpleErrorPageContent =
			@"<html><head><title>Application error</title></head>
				<body>
					<h2>Application Error</h2>
					<p>We are currently unable to serve your request: <a href=""{REQUESTURL}"">{HTMLREQUESTURL}</a></p>
					<p>We apologize, but an error occurred and your request could not be completed.</p>
					<pre>{DETAILS}</pre>
				</body>
			</<html>";
		private const string TestableSimpleErrorPageContent =
			@"<html><head><title>Application error</title></head>
				<body>
					<h2 id='ctl5' testControlClassName='" + JSLabelTestControl.ClassName + @"' testfield='FormCaption'>Application Error</h2>
					<p>We are currently unable to serve your request: <a id='ctl7' testControlClassName='" + JSLabelTestControl.ClassName + @"' testfield='RequestUrl' href=""{REQUESTURL}"">{HTMLREQUESTURL}</a></p>
					<p>We apologize, but an error occurred and your request could not be completed.</p>
					<pre id='ctl9' testControlClassName='" + JSLabelTestControl.ClassName + @"' testfield='Details'>{DETAILS}</pre>
					<script src='{TESTCONTROLSSCRIPTPATH}' type='text/javascript'></script>
					<script type='text/javascript'>
					<!-- 
						{TESTCONTROLSDECLARATION}
					 -->
					</script>
				</body>
			</html>";
		private static Dictionary<string, ErrorInfo> applicationErrors = new Dictionary<string, ErrorInfo>();
		private Dictionary<string, ErrorInfo> pageErrors = new Dictionary<string, ErrorInfo>();
		private Dictionary<string, ErrorInfo> cachedPageErrors = new Dictionary<string, ErrorInfo>();
		private static ErrorHandling instance;
		public static ErrorHandling Instance {
			get {
				if(instance == null) {
					instance = new ErrorHandling();
				}
				return instance;
			}
			set {
				instance = value;
			}
		}
		private static bool IsAppicationAndSessionStarted() {
			return HttpContext.Current != null
				&& HttpContext.Current.ApplicationInstance != null
				&& HttpContext.Current.Session != null
				&& HttpContext.Current.Profile != null;
		}
		private static bool IsUserIpSpecifiedInConfig() {
			try {
				string userIpString = HttpContext.Current.Request.UserHostAddress;
				string userIpFromConfigString = WebConfigurationManager.AppSettings[ConfigDetailedErrorReaderIPKeyName];
				if(!string.IsNullOrEmpty(userIpString) && !string.IsNullOrEmpty(userIpFromConfigString)) {
					IPAddress userIp, userIpFromConfig;
					if(IPAddress.TryParse(userIpString, out userIp)) {
						if(IPAddress.TryParse(userIpFromConfigString, out userIpFromConfig)) {
							return userIp.Equals(userIpFromConfig) || IPAddress.Any.Equals(userIpFromConfig) || IPAddress.IPv6Any.Equals(userIpFromConfig);
						}
						else {
							Tracing.Tracer.LogError("The IP in '{0}' is incorrect", ConfigDetailedErrorReaderIPKeyName);
						}
					}
					else {
						Tracing.Tracer.LogError("Unable to determine user host IP");
					}
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(e);
			}
			return false;
		}
		private bool IsErrorInfoCachingNeeded(ErrorInfo errorInfo) {
			NeedToCacheErrorInfoEventArgs args = new NeedToCacheErrorInfoEventArgs(errorInfo);
			if(NeedToCacheErrorInfo != null) {
				NeedToCacheErrorInfo(Instance, args);
			}
			return args.IsCachingNeeded;
		}
		private void CachePageError(ErrorInfo errorInfo) {
			ClearCachedPageError();
			if(IsErrorInfoCachingNeeded(errorInfo)) {
				cachedPageErrors[GetContextKey()] = errorInfo;
			}
		}
		protected static void SetApplicationError(ErrorInfo errorInfo) {
			lock(applicationErrors) {
				applicationErrors[Instance.GetContextKey()] = errorInfo;
			}
		}
		protected static void WriteSimpleErrorMessage(ErrorInfo errorInfo) {
			try {
				if(errorInfo == null) {
					throw new ArgumentNullException("errorInfo");
				}
				if(HttpContext.Current == null) {
					throw new ArgumentNullException("HttpContext.Current");
				}
				if(HttpContext.Current.Response == null) {
					throw new ArgumentNullException("HttpContext.Current.Response");
				}
				if(HttpContext.Current.Response.Cache == null) {
					throw new ArgumentNullException("HttpContext.Current.Response.Cache");
				}
				if(HttpContext.Current.Server == null) {
					throw new ArgumentNullException("HttpContext.Current.Server");
				}
				HttpContext.Current.Response.Clear();
				HttpContext.Current.Response.Cache.SetExpires(DateTime.Now);
				HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
				string content = SimpleErrorPageContent;
				if(TestScriptsManager.EasyTestEnabled) {
					content = TestableSimpleErrorPageContent.Replace("{TESTCONTROLSSCRIPTPATH}", TestScriptsManager.StaticTestControlsScriptInclude);
					content = content.Replace("{TESTCONTROLSDECLARATION}", TestScriptsManager.GetTestControlsArrayScript(-1));
				}
				if(!string.IsNullOrEmpty(SimpleErrorPage)) {
					try {
						content = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(SimpleErrorPage));
					}
					catch(Exception e) {
						Tracing.Tracer.LogError(e);
					}
				}
				content = content.Replace("{HTMLREQUESTURL}", System.Web.HttpUtility.HtmlEncode(errorInfo.Url));
				content = content.Replace("{REQUESTURL}", errorInfo.Url.Replace("\"", "&quot;"));
				if(CanShowDetailedInformation) {
					content = content.Replace("{DETAILS}", "\n" + System.Web.HttpUtility.HtmlEncode(errorInfo.GetTextualPresentation(true)));
				}
				else {
					content = content.Replace("{DETAILS}", "");
				}
				HttpContext.Current.Response.Write(content);
				HttpContext.Current.ApplicationInstance.CompleteRequest();
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(e);
			}
		}
		protected virtual bool IsImportantException(Exception exception) {
			if(exception is System.Threading.ThreadAbortException) {
				return false;
			}
			HttpException httpException = exception as HttpException;
			if(httpException != null) {
				int httpCode = httpException.GetHttpCode();
				return httpCode != 403 && httpCode != 404;
			}
			return true;
		}
		protected virtual Exception GetLastException() {
			if(HttpContext.Current != null && HttpContext.Current.Server.GetLastError() != null) {
				return HttpContext.Current.Server.GetLastError().GetBaseException();
			}
			return null;
		}
		protected virtual void LogException(ErrorInfo errorInfo) {
			Tracing.Tracer.LogError(errorInfo.GetTextualPresentation(false));
		}
		protected virtual void SendAlertToAdmin(ErrorInfo errorInfo) {
			SendAlertToAdmin(errorInfo.Id, errorInfo.GetTextualPresentation(true), errorInfo.Exception.Message, errorInfo.Exception);
		}
		protected virtual void ShowRichErrorPage(ErrorInfo errorInfo) {
			SetApplicationError(errorInfo);
			HttpContext.Current.Server.ClearError();
			WebApplication.Redirect(RichErrorPage);
		}
		protected virtual void ShowSimpleErrorPage(ErrorInfo errorInfo) {
			WriteSimpleErrorMessage(errorInfo);
			HttpContext.Current.Server.ClearError();
		}
#if DebugTest
		public Dictionary<string, ErrorInfo> DebugTest_PageErrors {
			get{return pageErrors;}
		}
#endif
#if DebugTest
		public virtual string GetContextKey() {
#else
		protected virtual string GetContextKey() {
#endif
			if(HttpContext.Current != null && HttpContext.Current.Request != null && WebWindow.CurrentRequestWindow != null) {
				return HttpContext.Current.Request.UserHostAddress + WebWindow.CurrentRequestWindow.GetHashCode().ToString();
			}
			return "Context unavailable";
		}
		public void ProcessApplicationError() {
			Exception exception = GetLastException();
			if(exception == null) {
				exception = new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnknownException));
			}
			if(IsImportantException(exception)) {
				try {
					ErrorInfo errorInfo = new ErrorInfo(exception, Tracing.Tracer.GetLastEntriesAsString());
					SendAlertToAdmin(errorInfo);
					LogException(errorInfo);
					if(IsAppicationAndSessionStarted() && !string.IsNullOrEmpty(RichErrorPage) && HttpContext.Current.Request.RawUrl.ToLower().IndexOf(RichErrorPage.ToLower()) == -1) {
						ShowRichErrorPage(errorInfo);
					}
					else {
						ShowSimpleErrorPage(errorInfo);
					}
					ClearPageError();
				}
				catch(ThreadAbortException) {
					throw;
				}
				catch(Exception e) {
					Tracing.Tracer.LogError(e);
				}
			}
		}
		public static bool IsIgnoredException(Exception exception) {
			if(WebApplication.Instance != null && exception != null) {
				return WebApplication.Instance.IgnoredExceptions.Contains(exception.GetType());
			}
			return false;
		}
		public void SetPageError(Exception error) {
			if(error is ActionExecutionException)
				SetPageError(error.InnerException);
			else {
				if(IsImportantException(error)) {
					ErrorInfo errorInfo = new ErrorInfo(error, Tracing.Tracer.GetLastEntriesAsString());
					pageErrors[GetContextKey()] = errorInfo;
					CachePageError(errorInfo);
					SendAlertToAdmin(errorInfo.Id, errorInfo.GetTextualPresentation(true), errorInfo.Exception.Message, errorInfo.Exception);
					Tracing.Tracer.LogError(errorInfo.GetTextualPresentation(false));
				}
				if(error is ThreadAbortException)
					throw error;
			}
		}
		public ErrorInfo GetCachedPageError() {
			ErrorInfo result = null;
			cachedPageErrors.TryGetValue(GetContextKey(), out result);
			return result;
		}
		public void ClearCachedPageError(Func<ErrorInfo, bool> needRemoveErrorDelegate) {
			if(IsAppicationAndSessionStarted()) {
				ErrorInfo errorInfo = GetCachedPageError();
				if(errorInfo != null && needRemoveErrorDelegate(errorInfo)) {
					cachedPageErrors.Remove(GetContextKey());
				}
			}
			else {
				cachedPageErrors.Clear();
			}
		}
		public void ClearCachedPageError() {
			ClearCachedPageError(new Func<ErrorInfo, bool>(delegate(ErrorInfo errorInfo) { return true; }));
		}
		public ErrorInfo GetPageError() {
			ErrorInfo result = null;
			pageErrors.TryGetValue(GetContextKey(), out result);
			return result;
		}
		public void ClearPageError(Func<ErrorInfo,bool> needRemoveErrorDelegate) {
			if(IsAppicationAndSessionStarted()) {
				ErrorInfo errorInfo = GetPageError();
				if(errorInfo != null && needRemoveErrorDelegate(errorInfo)) {
					pageErrors.Remove(GetContextKey());
				}
			}
			else {
				pageErrors.Clear();
			}
		}
		public void ClearPageError() {
			ClearPageError(new Func<ErrorInfo,bool>(delegate(ErrorInfo errorInfo) { return true; }));
		}
		public static void SendAlertToAdmin(string errorId, string errorDetails, string errorText) {
			SendAlertToAdmin(errorId, errorDetails, errorText, null);
		}
		public static void SendAlertToAdmin(string errorId, string errorDetails, string errorText, Exception exception) {
			if(IsIgnoredException(exception)) {
				return;
			}
			string subjectFormatString = "{0:" + ExceptionMessageParameter + "}";
			if(!string.IsNullOrEmpty(ErrorReportEmailSubject)) {
				subjectFormatString = ErrorReportEmailSubject;
			}
			string messageSubject = "";
			try {
				messageSubject = string.Format(
					new ObjectFormatter(), subjectFormatString,
					new ErrorInfoFormattingObject(GetApplicationName(), GetShortExceptionMessage(errorText)));
			}
			catch(Exception e) {
				Tracing.Tracer.LogSubSeparator("An error occurs on formatting the email subject: '" + subjectFormatString + "'");
				Tracing.Tracer.LogError(e);
				messageSubject = "XAF Application error report: error occurs when formatting subject";
			}
			string messageBody = "Error has occurred. ID:" + errorId + "\n\nDetails:\n" + errorDetails;
			CustomSendErrorNotificationEventArgs customSendErrorNotificationArgs =
				new CustomSendErrorNotificationEventArgs(errorId, errorText, errorDetails, messageSubject, messageBody, exception);
			if(CustomSendErrorNotification != null) {
				CustomSendErrorNotification(null, customSendErrorNotificationArgs);
			}
			if(!customSendErrorNotificationArgs.Handled) {
				if(!CanSendAlertToAdmin) {
					Tracing.Tracer.LogWarning("Cannot send alert email to admin");
				}
				else {
					try {
						Tracing.Tracer.LogText("Try to send alert email");
						string errorReportEmailFromResult = ErrorReportEmailFromDefault;
						try {
							errorReportEmailFromResult = string.Format(
								new ObjectFormatter(), ErrorReportEmailFrom,
								new ErrorInfoFormattingObject(GetApplicationName(), GetShortExceptionMessage(errorText)));
						}
						catch(Exception e) {
							Tracing.Tracer.LogSubSeparator("Exception occurs on formatting ErrorReportEmailFrom: '" + ErrorReportEmailFrom + "'");
							Tracing.Tracer.LogError(e);
						}
						string errorReportEmailFromNameResult = ErrorReportEmailFromNameDefault;
						try {
							errorReportEmailFromNameResult = string.Format(
								new ObjectFormatter(), ErrorReportEmailFromName,
								new ErrorInfoFormattingObject(GetApplicationName(), GetShortExceptionMessage(errorText)));
						}
						catch(Exception e) {
							Tracing.Tracer.LogSubSeparator("Exception occurs on formatting ErrorReportEmailFromName: '" + ErrorReportEmailFromName + "'");
							Tracing.Tracer.LogError(e);
						}
						MailMessage message = new MailMessage();
						try {
							message.From = new MailAddress(errorReportEmailFromResult, errorReportEmailFromNameResult);
						}
						catch(Exception e) {
							Tracing.Tracer.LogSubSeparator("Error occurs when initializing the 'MailMessage.From' property: '" + errorReportEmailFromResult + "'");
							Tracing.Tracer.LogError(e);
						}
						foreach(string email in ErrorReportEmail.Split(';')) {
							try {
								message.To.Add(new MailAddress(email));
							}
							catch(Exception e) {
								Tracing.Tracer.LogSubSeparator("Error occurs on adding a email address: '" + email + "'");
								Tracing.Tracer.LogError(e);
							}
						}
						message.Subject = messageSubject;
						message.Body = messageBody;
						SmtpClient smtp = new SmtpClient(ErrorReportEmailServer);
						if(ErrorReportEnableSsl) {
							smtp.EnableSsl = true;
						}
						CustomSendMailMessageEventArgs CustomSendMailMessageEventArgs =
							new CustomSendMailMessageEventArgs(customSendErrorNotificationArgs, smtp, message);
						if(CustomSendMailMessage != null) {
							CustomSendMailMessage(null, CustomSendMailMessageEventArgs);
						}
						if(!CustomSendMailMessageEventArgs.Handled) {
							smtp.Send(message);
						}
					}
					catch(Exception e) {
						Tracing.Tracer.LogError(e);
					}
				}
			}
		}
		private static string GetShortExceptionMessage(string message) {
			string result = message.Replace('\n', ' ').Replace('\r', ' ');
			if(result.Length > 120) {
				result = result.Substring(0, 117) + "...";
			}
			return result;
		}
		private static string GetApplicationName() {
			if(WebApplication.Instance != null) {
				return WebApplication.Instance.ApplicationName;
			}
			else {
				return "No instance of application";
			}
		}
		public static ErrorInfo GetApplicationError() {
			lock(applicationErrors) {
				ErrorInfo result = null;
				applicationErrors.TryGetValue(Instance.GetContextKey(), out result);
				return result;
			}
		}
		public static void ClearApplicationError() {
			lock(applicationErrors) {
				applicationErrors.Remove(Instance.GetContextKey());
			}
		}
		protected static string SimpleErrorPage {
			get { return WebConfigurationManager.AppSettings[ConfigSimpleErrorReportPageKeyName]; }
		}
		protected static string RichErrorPage {
			get { return WebConfigurationManager.AppSettings[ConfigRichErrorReportPageKeyName]; }
		}
		protected static string ErrorReportEmail {
			get {
				return WebConfigurationManager.AppSettings[ConfigErrorReportEmailKeyName];
			}
		}
		protected static string ErrorReportEmailServer {
			get {
				return WebConfigurationManager.AppSettings[ConfigErrorReportEmailServerKeyName];
			}
		}
		public const string ErrorReportEmailFromDefault = "null@nospam.com";
		public const string ErrorReportEmailFromNameDefault = "{0:" + ApplicationNameParameter + "} Error handling system";
		protected static string ErrorReportEmailFrom {
			get {
				string result = WebConfigurationManager.AppSettings[ConfigErrorReportEmailFromKeyName];
				if(string.IsNullOrEmpty(result)) {
					result = ErrorReportEmailFromDefault;
				}
				return result;
			}
		}
		protected static string ErrorReportEmailFromName {
			get {
				string result = WebConfigurationManager.AppSettings[ConfigErrorReportEmailFromNameKeyName];
				if(string.IsNullOrEmpty(result)) {
					result = ErrorReportEmailFromNameDefault;
				}
				return result;
			}
		}
		protected static string ErrorReportEmailSubject {
			get {
				return WebConfigurationManager.AppSettings[ConfigErrorReportEmailSubjectKeyName];
			}
		}
		protected static bool ErrorReportEnableSsl {
			get {
				bool enable = false;
				string configValue = WebConfigurationManager.AppSettings[ConfigErrorReportEnableSsl];
				if(!string.IsNullOrEmpty(configValue) && !Boolean.TryParse(configValue, out enable)) {
					if(configValue.Equals("1")) {
						enable = true;
					}
				}
				return enable;
			}
		}
		protected static string SessionLostPage {
			get {
				return WebConfigurationManager.AppSettings[ConfigSessionLostPageKeyName];
			}
		}
		public static bool CanShowDetailedInformation {
			get {
				return HttpContext.Current.Request.IsLocal || IsUserIpSpecifiedInConfig();
			}
		}
		public static bool CanSendAlertToAdmin {
			get { return !string.IsNullOrEmpty(ErrorReportEmail) && !string.IsNullOrEmpty(ErrorReportEmailServer); }
		}
		public static event EventHandler<CustomSendErrorNotificationEventArgs> CustomSendErrorNotification;
		public static event EventHandler<CustomSendMailMessageEventArgs> CustomSendMailMessage;
		public event EventHandler<NeedToCacheErrorInfoEventArgs> NeedToCacheErrorInfo;
	}
	public class PreserveValidationErrorMessageAfterPostbackController : ViewController {
		private void ClearPageErrors() {
			ErrorHandling.Instance.ClearPageError(IsValidationErrorInfo);
			ErrorHandling.Instance.ClearCachedPageError(IsValidationErrorInfo);
		}
		private bool IsValidationErrorInfo(ErrorInfo errorInfo) {
			return errorInfo.Exception is ValidationException;
		}
		private void ErrorHandling_NeedToCacheErrorInfo(object sender, NeedToCacheErrorInfoEventArgs e) {
			e.IsCachingNeeded = IsValidationErrorInfo(e.ErrorInfo);
		}
		private ErrorInfo lastInfo;
		private ContextIdentifiers lastContextId = null;
		private void RuleSet_ValidationCompleted(object sender, DevExpress.Persistent.Validation.ValidationCompletedEventArgs e) {
			if(e.Result.ValidationOutcome < ValidationOutcome.Error) {
				if(e.Result.ValidationOutcome == ValidationOutcome.Warning && !(View is ListView)) {
					ValidationException currentException = e.Exception as ValidationException;
					if(currentException == null) {
						currentException = new DevExpress.Persistent.Validation.ValidationException(e.Result);
						e.Exception = currentException;
					}
					e.Handled = false;
					lastContextId = e.Result.ContextIDs;
					ErrorInfo cachedInfo = ErrorHandling.Instance.GetCachedPageError();
					if(cachedInfo != null) {
						lastInfo = cachedInfo;
					}
					if(lastInfo != null && IsValidationErrorInfo(lastInfo) && (currentException != null) && ((ValidationException)lastInfo.Exception).Result.CompareResults(currentException.Result)) {
						if(lastInfo.IgnoreError) {
							e.Exception = null;
							e.Handled = true;
							lastInfo = null;
							lastContextId = null;
							ClearPageErrors();
						}
					}
					lastInfo = ErrorHandling.Instance.GetCachedPageError();
					ClearPageErrors();
				}
				else {
					if(!(View is ListView) && lastContextId == e.Result.ContextIDs || e.Result.ValidationOutcome <= ValidationOutcome.Valid && lastContextId == null) {
						ClearPageErrors();
					}
					e.Exception = null;
					e.Handled = true;
				}
			}
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			ClearPageErrors();
		}
		private void Application_Disposed(object sender, EventArgs e) {
			((XafApplication)sender).Disposed -= Application_Disposed;
			ErrorHandling.Instance.NeedToCacheErrorInfo -= ErrorHandling_NeedToCacheErrorInfo;
		}
		protected override void OnActivated() {
			base.OnActivated();
			ErrorHandling.Instance.NeedToCacheErrorInfo += ErrorHandling_NeedToCacheErrorInfo;
			if(this.Frame is Window && ((Window)this.Frame).IsMain) {
				DevExpress.Persistent.Validation.Validator.RuleSet.ValidationCompleted += new EventHandler<DevExpress.Persistent.Validation.ValidationCompletedEventArgs>(RuleSet_ValidationCompleted);
			}
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			Application.Disposed += Application_Disposed;
		}
		protected override void OnDeactivated() {
			Application.Disposed -= Application_Disposed;
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			ErrorHandling.Instance.NeedToCacheErrorInfo -= ErrorHandling_NeedToCacheErrorInfo;
			ClearPageErrors();
			if(DevExpress.Persistent.Validation.Validator.RuleSet != null) { 
				DevExpress.Persistent.Validation.Validator.RuleSet.ValidationCompleted -= new EventHandler<DevExpress.Persistent.Validation.ValidationCompletedEventArgs>(RuleSet_ValidationCompleted);
			}
			base.OnDeactivated();
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			if(!View.ErrorMessages.IsEmpty) {
				ErrorInfo info = ErrorHandling.Instance.GetCachedPageError();
				if(info != null && IsValidationErrorInfo(info)) {
					ErrorHandling.Instance.SetPageError(info.Exception);
				}
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			ErrorHandling.Instance.NeedToCacheErrorInfo -= ErrorHandling_NeedToCacheErrorInfo;
			ClearPageErrors();
		}
		public PreserveValidationErrorMessageAfterPostbackController() {
			TargetViewNesting = Nesting.Root;
		}
	}
}

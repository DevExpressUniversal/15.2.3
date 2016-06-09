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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using System.Web.SessionState;
	public interface IHttpModuleSubscriber {
		bool RequestRecipient(HttpRequest request, RequestEvent requestEvent);
		void ProcessRequest();
	}
	public interface IHttpHandlerSubscriber {
		bool RequestRecipient(HttpRequest request);
		void ProcessRequest(HttpContext context);
		SessionStateBehavior SessionStateBehavior { get; }
	}
	public class ResourceManagerSubscriber : IHttpModuleSubscriber {
		public bool RequestRecipient(HttpRequest request, RequestEvent requestEvent) {
			return requestEvent == RequestEvent.BeginRequest &&
				request.Path.EndsWith(ResourceManager.ResourceHandlerName, StringComparison.OrdinalIgnoreCase);
		}
		public void ProcessRequest() {
			ResourceManager.ProcessRequest();
		}
	}
	public class BinaryStorageSubscriber : IHttpModuleSubscriber {
		public bool RequestRecipient(HttpRequest request, RequestEvent requestEvent) {
			return request.QueryString.ToString().IndexOf(BinaryStorage.CacheParamName, StringComparison.OrdinalIgnoreCase) > -1;
		}
		public void ProcessRequest() {
			if(!string.IsNullOrEmpty(HttpUtils.GetQuery()[BinaryStorage.ImageResizedParamName]))
				BinaryStorage.ProcessRequestForResizedImage();
			else
				BinaryStorage.ProcessRequest();
		}
	}
	public enum RequestProcessState { Processed, Error, Unprocessed }
	public enum RequestEvent { BeginRequest, PreRequestHandlerExecute }
	public class ASPxHttpHandlerModule : IHttpModule, IHttpHandler {
		private const string CallbackPrefix = "0|";
		static List<IHttpModuleSubscriber> subscribers = new List<IHttpModuleSubscriber>();
		static List<IHttpHandlerSubscriber> handlerSubscribers = new List<IHttpHandlerSubscriber>();
		static ASPxHttpHandlerModule() {
			Subscribe(new ResourceManagerSubscriber());
			Subscribe(new BinaryStorageSubscriber());
		}
		public static void Subscribe(IHttpModuleSubscriber subscriber) {
			subscribers.Add(subscriber);
		}
		public static void Subscribe(IHttpHandlerSubscriber subscriber) {
			handlerSubscribers.Add(subscriber);
		}
		public static void Subscribe(IHttpModuleSubscriber subscriber, bool registerTypeOnce) {
			var type = subscriber.GetType();
			if(!registerTypeOnce || !subscribers.Any(s => s.GetType().Equals(type)))
				Subscribe(subscriber);
		}
		RequestProcessState requestProcessState = RequestProcessState.Unprocessed;
		protected RequestProcessState RequestProcessState {
			get { return requestProcessState; }
		}
		protected virtual void Init(HttpApplication context) {
			HttpContext.Current.Application[ResourceManager.HandlerRegistrationFlag] = true;
			context.PreSendRequestHeaders += new EventHandler(PreSendRequestHeadersHandler);
			context.BeginRequest += new EventHandler(BeginRequestHandler);
			context.PreRequestHandlerExecute += new EventHandler(PreRequestHandlerExecuteHandler);
			context.PostRequestHandlerExecute += new EventHandler(PostRequestHandlerExecuteHandler);
			context.PostMapRequestHandler += new EventHandler(context_PostMapRequestHandler);
		}
		private void context_PostMapRequestHandler(object sender, EventArgs e) {
			HttpContext context = HttpContext.Current;
			if(context != null && context.Handler is ASPxHttpHandlerModule) {
				HttpRequest request = context.Request;
				foreach(IHttpHandlerSubscriber subscriber in handlerSubscribers) {
					if(subscriber.RequestRecipient(request)) {
						context.SetSessionStateBehavior(subscriber.SessionStateBehavior);
						break;
					}
				}
			}
		}
		protected virtual void Dispose() {
		}
		private void PreSendRequestHeadersHandler(object sender, EventArgs args) {
			if(RequestProcessState == RequestProcessState.Error)
				return;
			this.requestProcessState = RequestProcessState.Processed;
			HttpApplication app = (HttpApplication)sender;
			HttpResponse response = app.Response;
			HttpRequest request = app.Request;
			if(IsCallBack(request)) {
				if(IsErrorCode(response.StatusCode)) {
					string message = ASPxWebControl.CallbackErrorMessageInternal;
					string errorPageUrl = ConfigurationSettings.ErrorPageUrl;
					if(errorPageUrl != null) {
						try {
							errorPageUrl = new Control().ResolveClientUrl(errorPageUrl);
						}
						catch(Exception) {
							if(VirtualPathUtility.IsAppRelative(errorPageUrl))
								errorPageUrl = VirtualPathUtility.ToAbsolute(errorPageUrl);
						}
						response.RedirectLocation = errorPageUrl + ASPxWebControl.ErrorQueryString(message);
					}
					else {
						PrepareContent(ref response);
						response.Output.Write(EncodeError(message));
					}
				}
				string redirectLocation = response.RedirectLocation;
				if(!string.IsNullOrEmpty(redirectLocation)) {
					PrepareContent(ref response);
					response.Output.Write(EncodeRedirect(redirectLocation));
				}
			}
			else {
				RenderUtils.EnsureIECompatibilityMeta();
			}
		}
		private void BeginRequestHandler(object sender, EventArgs e) {
			bool requestProcessed = ProcessRequestCore(RequestEvent.BeginRequest);
			if(!requestProcessed && UploadProgressManager.IsProcessRequestAllowed()) 
				UploadProgressManager.ProcessRequest(ref requestProcessState);
		}
		private void PreRequestHandlerExecuteHandler(object sender, EventArgs e) {
			ProcessRequestCore(RequestEvent.PreRequestHandlerExecute);
		}
		private void PostRequestHandlerExecuteHandler(object sender, EventArgs e) {
			if(ConfigurationSettings.EnableHtmlCompression)
				HttpUtils.MakeResponseCompressed(false);
		}
		private bool ProcessRequestCore(RequestEvent requestEvent) {
			HttpRequest request = HttpUtils.GetRequest();
			HttpContext context = HttpContext.Current;
			foreach(IHttpModuleSubscriber subscriber in subscribers) {
				if(subscriber.RequestRecipient(request, requestEvent)) {
					subscriber.ProcessRequest();
					return true;
				}
			}
			return false;
		}
		protected bool IsCallBack(HttpRequest request) {
			if(request.HttpMethod == "GET")
				return false;
			bool hasParam = false;
			try {
				hasParam = !string.IsNullOrEmpty(HttpUtils.GetValueFromRequest(request, RenderUtils.CallbackControlIDParamName)); 
			}
			catch(HttpRequestValidationException) {
				hasParam = !string.IsNullOrEmpty(HttpUtils.GetValueFromRequest(request, RenderUtils.CallbackControlIDParamName));
			}
			return hasParam || MvcUtils.IsCallback();
		}
		protected bool IsErrorCode(int code) {
			if(code == 404 || code == 500)
				return true;
			return false;
		}
		protected string EncodeError(string error) {
			Hashtable result = new Hashtable();
			result[CallbackResultProperties.GeneralError] = error;
			return Encode(RenderUtils.CallBackResultPrefix + HtmlConvertor.ToJSON(result));
		}
		protected string EncodeRedirect(string redirectUrl) {
			Hashtable result = new Hashtable();
			result[CallbackResultProperties.Redirect] = redirectUrl;
			return Encode(RenderUtils.CallBackResultPrefix + HtmlConvertor.ToJSON(result));
		}
		protected string Encode(string request) {
			return MvcUtils.IsCallback() ? request : CallbackPrefix + request;
		}
		protected void PrepareContent(ref HttpResponse response) {
			List<HttpCookie> cookies = new List<HttpCookie>(response.Cookies.Count);
			for(int i = 0; i < response.Cookies.Count; i++)
				cookies.Add(response.Cookies[i]);
			response.ClearHeaders();
			response.ClearContent();
			response.ContentType = "text/html";
			for(int i = 0; i < cookies.Count; i++)
				response.AppendCookie(cookies[i]);
			response.Cache.SetCacheability(HttpCacheability.NoCache);
		}
		void IHttpModule.Init(HttpApplication context) {
			Init(context);
		}
		void IHttpModule.Dispose() {
			Dispose();
		}
		bool IHttpHandler.IsReusable {
			get { return true; }
		}
		void IHttpHandler.ProcessRequest(HttpContext context) {
			HttpRequest request = context.Request;
			foreach(IHttpHandlerSubscriber subscriber in handlerSubscribers) {
				if(subscriber.RequestRecipient(request)) {
					subscriber.ProcessRequest(context);
					break;
				}
			}
			CustomCssJsManager.ProcessRequest();
		}
	}
}

#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Collections.Specialized;
using System.Net;
using System.Web;
using DevExpress.Data.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using System.Web.SessionState;
namespace DevExpress.XtraReports.Web.Native.ClientControls {
	public static class ManagerSubscriber {
		public static RequestEvent RequestEvent { get; set; }
		static ManagerSubscriber() {
			RequestEvent = RequestEvent.BeginRequest;
		}
		public static bool RequestRecipient(RequestEvent requestEvent) {
			return requestEvent == RequestEvent
				&& CanProcessRequest();
		}
		static bool CanProcessRequest() {
			var ev = CanProcessHandlerRequest;
			if(ev != null) {
				var args = new CanProcessHandlerRequestEventArgs();
				ev(null, args);
				return !args.Cancel;
			}
			return true;
		}
		public static event EventHandler<CanProcessHandlerRequestEventArgs> CanProcessHandlerRequest;
	}
	public class ManagerModuleSubscriber<T> : IHttpModuleSubscriber
		where T : IRequestManager {
		readonly string handlerName;
		readonly Func<T> getRequestManager;
		public ManagerModuleSubscriber(IServiceProvider serviceProvider, string handlerName) {
			this.handlerName = handlerName;
			getRequestManager = () => serviceProvider.GetService<T>();
		}
		public bool RequestRecipient(HttpRequest request, RequestEvent requestEvent) {
			return ManagerSubscriber.RequestRecipient(requestEvent)
				&& (request.HttpMethod == WebRequestMethods.Http.Get || request.HttpMethod == WebRequestMethods.Http.Post)
				&& request.Path.EndsWith(handlerName, StringComparison.OrdinalIgnoreCase);
		}
		public void ProcessRequest() {
			HttpContext context = HttpContext.Current;
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			var manager = getRequestManager();
			NameValueCollection values = request.HttpMethod == WebRequestMethods.Http.Get
				? request.QueryString
				: request.Form;
			HttpActionResultBase result = manager.ProcessRequest(values);
			result.Write(new SystemWebHttpResponse(response));
			HttpUtils.EndResponse();
		}
	}
	public class ManagerHandlerSubscriber<T> : IHttpHandlerSubscriber
	where T : IRequestManager {
		readonly string handlerName;
		readonly Func<T> getRequestManager;
		public ManagerHandlerSubscriber(IServiceProvider serviceProvider, string handlerName, SessionStateBehavior sessionStateBehavior) {
			this.handlerName = handlerName;
			this.sessionStateBehavior = sessionStateBehavior;
			getRequestManager = () => serviceProvider.GetService<T>();
		}
		public bool RequestRecipient(HttpRequest request) {
			return (request.HttpMethod == WebRequestMethods.Http.Get || request.HttpMethod == WebRequestMethods.Http.Post)
				&& request.Path.EndsWith(handlerName, StringComparison.OrdinalIgnoreCase);
		}
		public void ProcessRequest(HttpContext context) {
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			var manager = getRequestManager();
			NameValueCollection values = request.HttpMethod == WebRequestMethods.Http.Get
				? request.QueryString
				: request.Form;
			HttpActionResultBase result = manager.ProcessRequest(values);
			result.Write(new SystemWebHttpResponse(response));
			HttpUtils.EndResponse();
		}
		SessionStateBehavior sessionStateBehavior = SessionStateBehavior.Default;
		public System.Web.SessionState.SessionStateBehavior SessionStateBehavior {
			get { return sessionStateBehavior; }
		}
	}
}

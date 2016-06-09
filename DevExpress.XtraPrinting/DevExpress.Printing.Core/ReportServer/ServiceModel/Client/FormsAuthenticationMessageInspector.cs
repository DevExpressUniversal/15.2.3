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
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using DevExpress.Utils;
namespace DevExpress.ReportServer.ServiceModel.Client {
	public class FormsAuthenticationMessageInspector : IClientMessageInspector {
		string sharedCookie;
		public FormsAuthenticationMessageInspector(Cookie cookie) {
			Guard.ArgumentNotNull(cookie, "cookie");
			this.sharedCookie = cookie.ToString();
		}
		public FormsAuthenticationMessageInspector() {
		}
		public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState) {
			HttpResponseMessageProperty httpResponse = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
			if(httpResponse != null) {
				string cookie = httpResponse.Headers[HttpResponseHeader.SetCookie];
				if(!string.IsNullOrEmpty(cookie)) {
					OnSetCookie(cookie);
				}
			}
		}
		protected virtual void OnSetCookie(string cookie) {
			sharedCookie = cookie;
		}
		public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel) {
			HttpRequestMessageProperty httpRequest;
			if(!request.Properties.ContainsKey(HttpRequestMessageProperty.Name)) {
				request.Properties.Add(HttpRequestMessageProperty.Name, new HttpRequestMessageProperty());
			}
			httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
			if(httpRequest.Headers[HttpRequestHeader.Cookie] != null) {
				throw new InvalidOperationException("Unexpected: HTTP Request object has cookie header already.");
			}
			httpRequest.Headers.Add(HttpRequestHeader.Cookie, sharedCookie);
			return null;
		}
	}
}

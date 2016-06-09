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
using System.Net;
using System.Web;
using DevExpress.Utils;
using DevExpress.Web.Internal;
namespace DevExpress.Web.WebClientUIControl.Internal {
	class DevExtremeFontProviderSubscriber : IHttpModuleSubscriber {
		struct FontInfo {
			public string UrlPostfix { get; private set; }
			public string ResourceName { get; private set; }
			public string ContentType { get; private set; }
			public FontInfo(string urlPostfix, string resourceName, string contentType)
				: this() {
				UrlPostfix = urlPostfix;
				ResourceName = resourceName;
				ContentType = contentType;
			}
		}
		static readonly FontInfo[] fonts = {
			new FontInfo("icons/dxicons.eot", RenderUtils.DevExtremeCssResourcePath + "icons.dxicons.eot", "font/eot"),
			new FontInfo("icons/dxicons.ttf", RenderUtils.DevExtremeCssResourcePath + "icons.dxicons.ttf", "font/ttf"),
			new FontInfo("icons/dxicons.woff", RenderUtils.DevExtremeCssResourcePath + "icons.dxicons.woff", "application/font-woff")
		};
		readonly Func<bool> shouldRequestRecipient;
		public DevExtremeFontProviderSubscriber(Func<bool> shouldRequestRecipient) {
			Guard.ArgumentNotNull(shouldRequestRecipient, "shouldRequestRecipient");
			this.shouldRequestRecipient = shouldRequestRecipient;
		}
		#region IHttpModuleSubscriber
		public bool RequestRecipient(HttpRequest request, RequestEvent requestEvent) {
			return shouldRequestRecipient()
				&& requestEvent == RequestEvent.BeginRequest
				&& request.HttpMethod == WebRequestMethods.Http.Get
				&& GetResourceByUrl(request.RawUrl) != null;
		}
		public void ProcessRequest() {
			var httpContext = HttpContext.Current;
			var request = httpContext.Request;
			var response = httpContext.Response;
			FontInfo fontInfo = GetResourceByUrl(request.RawUrl).Value;
			response.ContentType = fontInfo.ContentType;
			using(var stream = typeof(DevExtremeFontProviderSubscriber).Assembly.GetManifestResourceStream(fontInfo.ResourceName)) {
				stream.CopyTo(response.OutputStream);
			}
			HttpUtils.EndResponse();
		}
		#endregion
		static FontInfo? GetResourceByUrl(string url) {
			foreach(var fontInfo in fonts) {
				if(url.EndsWith(fontInfo.UrlPostfix, StringComparison.Ordinal)) {
					return fontInfo;
				}
			}
			return null;
		}
	}
}

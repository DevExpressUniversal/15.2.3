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
using System.Net;
using DevExpress.ReportServer.ServiceModel.Client;
namespace DevExpress.XtraReports.Web.Native.DocumentViewer.Remote {
	public class RSClientMessageInspector : FormsAuthenticationMessageInspector {
		public static string CookieName { get; set; }
		static RSClientMessageInspector() {
			CookieName = ".ASPXAUTH";
		}
		public event EventHandler<CookieReceivedEventArgs> CookieReceived;
		public RSClientMessageInspector(string cookieValue)
			: base(new Cookie(CookieName, cookieValue)) {
		}
		public RSClientMessageInspector() { }
		protected override void OnSetCookie(string cookieStr) {
			base.OnSetCookie(cookieStr);
			Uri parserUri = new Uri("https://CookieParser");
			CookieContainer cookieContainer = new CookieContainer();
			cookieContainer.SetCookies(parserUri, cookieStr);
			var cookie = cookieContainer.GetCookies(parserUri)[CookieName];
			if(CookieReceived != null) {
				CookieReceived(this, new CookieReceivedEventArgs(cookie.Value));
			}
		}
	}
}

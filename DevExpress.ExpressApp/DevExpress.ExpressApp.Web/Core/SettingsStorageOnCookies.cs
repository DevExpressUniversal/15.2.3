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
using System.Web;
using DevExpress.Persistent.Base;
using System.Collections.Specialized;
using DevExpress.ExpressApp.Web;
namespace DevExpress.ExpressApp.Utils {
	public class SettingsStorageOnCookies : SettingsStorage {
		private string applicationName;
		private string GenerateCookieName(string optionPath) {
			return applicationName + (string.IsNullOrEmpty(optionPath) ? "" : optionPath);
		}
		private HttpCookie GetResponseCookie(string optionPath) {
			string cookieName = GenerateCookieName(optionPath);
			HttpCookie result = HttpContext.Current.Response.Cookies.Get(cookieName);
			if(result == null) {
				result = new HttpCookie(cookieName);
			}
			return result;
		}
		private static string GetApplicationName() {
			return WebApplication.Instance != null && !string.IsNullOrEmpty(WebApplication.Instance.ApplicationName) ? WebApplication.Instance.ApplicationName : "Unknown";
		}
		public SettingsStorageOnCookies() : this(GetApplicationName()) { }
		public SettingsStorageOnCookies(string applicationName) : base() {
			if(string.IsNullOrEmpty(applicationName))
				throw new ArgumentNullException("applicationName");
			this.applicationName = applicationName;
		}
		public override bool IsPathExist(string optionPath) {
			return HttpContext.Current.Request.Cookies[GenerateCookieName(optionPath)] != null;
		}
		public override void SaveOption(string optionPath, string optionName, string optionValue) {
			if(string.IsNullOrEmpty(optionName))
				throw new ArgumentNullException("optionName");
			HttpCookie cookie;
			if(!string.IsNullOrEmpty(optionPath)) {
				cookie = GetResponseCookie(optionPath);
				if(string.IsNullOrEmpty(optionName)) {
					cookie.Value = optionValue;
				}
				else {
					cookie.Values[optionName] = optionValue;
				}
			} else {
				cookie = GetResponseCookie(optionName);
				cookie.Value = optionValue;
			}
			cookie.Expires = DateTime.Now.AddMonths(1);
			HttpContext.Current.Response.Cookies.Add(cookie);
		}
		public override string LoadOption(string optionPath, string optionName) {
			HttpCookie cookie;
			if(!string.IsNullOrEmpty(optionPath)) {
				cookie = HttpContext.Current.Request.Cookies[GenerateCookieName(optionPath)];
				if(cookie != null) {
					if(string.IsNullOrEmpty(optionName)) {
						return cookie.Value;
					}
					else {
						return cookie.Values[optionName];
					}
				}
			}
			else {
				cookie = HttpContext.Current.Request.Cookies[GenerateCookieName(optionName)];
				if(cookie != null) {
					return cookie.Value;
				}
			}
			return "";
		}
	}
}

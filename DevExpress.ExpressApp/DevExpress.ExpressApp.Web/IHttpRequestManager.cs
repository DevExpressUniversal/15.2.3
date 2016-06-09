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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.Security;
using DevExpress.ExpressApp.Web.Templates;
namespace DevExpress.ExpressApp.Web {
	public interface IHttpRequestManager {
		bool IsLogonWindow();
		bool IsPopupWindow();
		bool IsFindPopupWindow();
		bool IsApplicationWindow();
		TemplateType GetTemplateType();
		string GetQueryString(ViewShortcut viewShortcut);
		ViewShortcut GetViewShortcut(string queryString);
		string GetPopupWindowId();
		string GetPopupWindowShowActionId();
		string GetPopupWindowQueryString(ViewShortcut shortcut, string windowId);
		#region Obsolete 15.1
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		ViewShortcut GetViewShortcut();
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		TemplateContext GetTemplateContext();
		#endregion
	}
	public class DefaultHttpRequestManager : IHttpRequestManager {
		public const string WindowIDKeyName = "WindowID";
		public const string ActionIDKeyName = "ActionID";
		public virtual bool IsLogonWindow() {
			string path = Request.Path.ToLower();
			return path == FormsAuthentication.LoginUrl.ToLower() || path == WebApplication.LogonPage.ToLower();
		}
		public virtual bool IsPopupWindow() {
			return IsApplicationWindow() && Request.Params[WebApplication.DialogKeyName] != null;
		}
		public virtual bool IsFindPopupWindow() {
			return IsApplicationWindow() && Request.Params[WebApplication.FindDialogKeyName] != null;
		}
		public virtual bool IsApplicationWindow() {
			string path = Request.Path.ToLower();
			return path.Contains(WebApplication.DefaultPage.ToLower());
		}
		public TemplateType GetTemplateType() {
			TemplateType result = TemplateType.Vertical;
			if(IsLogonWindow()) {
				result = TemplateType.Logon;
			}
			else if(IsPopupWindow()) {
				result = TemplateType.Dialog;
			}
			else if(IsFindPopupWindow()) {
				result = TemplateType.FindDialog;
			}
			else if(IsApplicationWindow()) {
				result = WebApplication.PreferredApplicationWindowTemplateType;
			}
			return result;
		}
		public virtual string GetQueryString(ViewShortcut viewShortcut) {
			NameValueCollection collection = new NameValueCollection();
			for(int i = 0; i < viewShortcut.Count; i++) {
				string key = viewShortcut.GetKey(i);
				string value = viewShortcut[i];
				if(!string.IsNullOrEmpty(value)) {
					collection[key] = value;
				}
			}
			return UrlHelper.BuildQueryString(collection);
		}
		public virtual ViewShortcut GetViewShortcut(string queryString) {
			ViewShortcut result = new ViewShortcut();
			NameValueCollection collection = UrlHelper.ParseQueryString(queryString);
			foreach(string key in collection.Keys) {
				if(!string.IsNullOrEmpty(key)) {
					result[key] = collection[key];
				}
			}
			return result;
		}
		public virtual string GetPopupWindowId() {
			return Request.Params[WindowIDKeyName];
		}
		public virtual string GetPopupWindowShowActionId() {
			return Request.Params[ActionIDKeyName];
		}
		public virtual string GetPopupWindowQueryString(ViewShortcut shortcut, string windowId) {
			NameValueCollection collection = new NameValueCollection();
			collection.Add(WindowIDKeyName, windowId);
			return UrlHelper.BuildQueryString(collection);
		}
		public HttpRequest Request {
			get { return HttpContext.Current.Request; }
		}
		#region Obsolete 15.1
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string ShortcutUrlParamPrefix = "Shortcut";
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual NameValueCollection PrepareRedirectParams(ViewShortcut currentShortcut) {
			NameValueCollection collection = new NameValueCollection(HttpContext.Current.Request.QueryString);
#pragma warning disable 0618
			WriteShortcutTo(currentShortcut, collection);
#pragma warning restore 0618
			return collection;
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void WriteShortcutTo(ViewShortcut currentShortcut, NameValueCollection queryString) {
			for(int i = queryString.Count - 1; i >= 0; i--) {
				string key = queryString.GetKey(i);
				if(key != null && key.StartsWith(ShortcutUrlParamPrefix)) {
					queryString.Remove(key);
				}
			}
			for(int i = 0; i < currentShortcut.Count; i++) {
				string key = ShortcutUrlParamPrefix + currentShortcut.GetKey(i);
				if(!string.IsNullOrEmpty(currentShortcut[i])) {
					queryString[key] = currentShortcut[i];
				}
				else if(queryString[key] != null) {
					queryString.Remove(key);
				}
			}
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual ViewShortcut GetViewShortcutFromQueryString(NameValueCollection queryString) {
			ViewShortcut result = new ViewShortcut();
			foreach(string key in queryString.Keys) {
				if(key == null)
					continue;
#pragma warning disable 0618
				if(key.StartsWith(ShortcutUrlParamPrefix)) {
					string shortcutKey = key.Substring(ShortcutUrlParamPrefix.Length);
					result[shortcutKey] = queryString[key];
				}
#pragma warning restore 0618
			}
			return result;
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual ViewShortcut GetViewShortcut() {
#pragma warning disable 0618
			return GetViewShortcutFromQueryString(Request.QueryString);
#pragma warning restore 0618
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual TemplateContext GetTemplateContext() {
			TemplateContext result = TemplateContext.Undefined;
			if(IsLogonWindow()) {
				result = TemplateContext.LogonWindow;
			}
			else if(IsPopupWindow()) {
				result = TemplateContext.PopupWindow;
			}
			else if(IsFindPopupWindow()) {
				result = TemplateContext.FindPopupWindow;
			}
			else if(IsApplicationWindow()) {
				result = TemplateContext.ApplicationWindow;
			}
			return result;
		}
		#endregion
	}
}

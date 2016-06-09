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
using System.ComponentModel;
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	using System.Collections;
	using System.Web;
	[ToolboxItem(false)]
	public class MVCxTabControl : ASPxTabControl {
		public MVCxTabControl()
			: base() {
		}
		public new TabControlImages Images {
			get { return base.Images; }
		}
		public new TabControlStyles Styles {
			get { return base.Styles; }
		}
		public override bool IsLoading() {
			return false;
		}
	}
	[ToolboxItem(false)]
	public class MVCxPageControl : ASPxPageControl {
		protected const string TabsInfoKey = "tabsInfo";
		public MVCxPageControl()
			: base() {
		}
		public object CallbackRouteValues { get; set; }
		public new TabControlImages Images {
			get { return base.Images; }
		}
		public new TabControlStyles Styles {
			get { return base.Styles; }
		}
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected internal override bool IsCallBacksEnabled() {
			return IsLoadTabByCallbackInternal();
		}
		protected internal override bool IsLoadTabByCallbackInternal() {
			return CallbackRouteValues != null;
		}
		protected override string GetCallbackResultHtml() {
			return Utils.CallbackHtmlContentPlaceholder;
		}
		protected internal new Control GetCallbackResultControl() {
			return base.GetCallbackResultControl();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(TabsInfoKey, PageControlState.SaveTabsInfo(TabPages));
			return result;
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientPageControl";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxPageControl), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxPageControl), Utils.TabControlScriptResourceName);
		}
		public override bool IsLoading() {
			return false;
		}
		protected internal new void EnsureClientStateLoaded() {
			base.EnsureClientStateLoaded();
		}
		protected override bool NeedLoadClientState() {
			return !IsCallback;
		}
		internal static PageControlState GetState(string name) {
			Hashtable clientObjectState = LoadClientObjectState(HttpContext.Current.Request.Params, name);
			if(clientObjectState == null) return null;
			string serializedTabsInfo = GetClientObjectStateValue<string>(clientObjectState, TabsInfoKey);
			int activeTabIndex = GetClientObjectStateValue<int>(clientObjectState, ActiveTabIndexKey);
			return PageControlState.Load(serializedTabsInfo, activeTabIndex);
		}
	}
}

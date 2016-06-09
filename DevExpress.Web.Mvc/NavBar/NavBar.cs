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

using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	[ToolboxItem(false)]
	public class MVCxNavBar: ASPxNavBar {
		protected const string ItemsInfoKey = "itemsInfo";
		public MVCxNavBar()
			: base() {
		}
		public object CallbackRouteValues { get; set; }
		public new NavBarImages Images {
			get { return base.Images; }
		}
		public new NavBarStyles Styles {
			get { return base.Styles; }
		}
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected internal override bool IsCallBacksEnabled() {
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
			result.Add(ItemsInfoKey, NavBarState.SaveGroupsItemsGeneralInfo(Groups));
			return result;
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientNavBar";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxNavBar), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxNavBar), Utils.NavBarScriptResourceName);
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
		protected internal void ValidateProperties() {
			ValidateAutoCollapse(null);
			ValidateSelectedItem();
		}
		protected internal new string GetItemIndexPath(NavBarItem item) {
			return base.GetItemIndexPath(item);
		}
		protected internal new string SaveGroupsState() {
			return base.SaveGroupsState();
		}
		internal static NavBarState GetState(string name) {
			Hashtable clientObjectState = LoadClientObjectState(HttpContext.Current.Request.Params, name);
			if(clientObjectState == null) return null;
			string serializedGroupsInfo = GetClientObjectStateValue<string>(clientObjectState, ItemsInfoKey);
			string groupsExpandedState = GetClientObjectStateValue<string>(clientObjectState, GroupsExpandingStateKey);
			string itemsSelectionState = GetClientObjectStateValue<string>(clientObjectState, SelectedItemIndexPathKey);
			return NavBarState.Load(serializedGroupsInfo, groupsExpandedState, itemsSelectionState);
		}
	}
}

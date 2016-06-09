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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Web.FilterControl;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web.Rendering;
	using DevExpress.Web.Internal;
	using System.Collections;
	public class MVCxPopupFilterControl: ASPxPopupFilterControl {
		public MVCxPopupFilterControl(IPopupFilterControlOwner filterPopupOwner)
			: base(filterPopupOwner) {
			CallbackActionUrlCollection = new Dictionary<string, string>();
		}
		public object CallbackRouteValues { get; set; }
		public Dictionary<string, string> CallbackActionUrlCollection { get; private set; }
		protected internal new bool IsApplyCalled { get { return base.IsApplyCalled; } }
		protected internal static string GetFilterState(string filterBilderName){
			Hashtable state = LoadClientObjectState(HttpUtils.GetValueFromRequest(filterBilderName));
			return GetClientObjectStateValueString(state, FilterExpressionStateKey);
		}
		public override void ApplyFilter() {
			var grid = FilterPopupOwner as IGridAdapterOwner;
			if(!grid.EnableCustomOperations)
				base.ApplyFilter();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if(CallbackActionUrlCollection.Count > 0)
				stb.Append(localVarName + ".callbackActionUrlCollection=eval(\"" + HtmlConvertor.ToJSON(CallbackActionUrlCollection) + "\");\n");
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxGridView), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxGridView), Utils.FilterControlScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientFilterControl";
		}
	}
	public class MVCxWebFilterControlPopup: WebFilterControlPopup {
		public MVCxWebFilterControlPopup(IPopupFilterControlOwner filterPopupOwner)
			: base(filterPopupOwner) {
		}
		protected internal new MVCxPopupFilterControl FilterControl { get { return (MVCxPopupFilterControl)base.FilterControl; } }
		protected override ASPxPopupFilterControl CreatePopupFilterControl(IPopupFilterControlOwner filterPopupOwner) {
			var filterControl = new MVCxPopupFilterControl(filterPopupOwner);
			var grid = filterPopupOwner as IGridAdapterOwner;
			if(grid != null) {
				filterControl.CallbackRouteValues = grid.CallbackRouteValues;
				if(grid.CallbackActionUrlCollection.ContainsKey(FilterControlCallbackCommand.Apply))
					filterControl.CallbackActionUrlCollection[FilterControlCallbackCommand.Apply] = grid.CallbackActionUrlCollection[FilterControlCallbackCommand.Apply];
			}
			return filterControl;
		}
	}
}

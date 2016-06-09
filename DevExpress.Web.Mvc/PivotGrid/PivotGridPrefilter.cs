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

namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxPivotGrid.Internal;
	using DevExpress.Web.FilterControl;
	using DevExpress.Web.Mvc.Internal;
	public class MVCxPivotWebFilterControlPopup: PivotWebFilterControlPopup {
		public MVCxPivotWebFilterControlPopup(IPopupFilterControlOwner filterPopupOwner) :
			base(filterPopupOwner) {
		}
		protected override ASPxPopupFilterControl CreatePopupFilterControl(IPopupFilterControlOwner filterPopupOwner) {
			MVCxPivotPopupFilterControl filterControl = new MVCxPivotPopupFilterControl(filterPopupOwner);
			if (filterPopupOwner is MVCxPivotGrid)
				filterControl.CallbackRouteValues = ((MVCxPivotGrid)filterPopupOwner).CallbackRouteValues;
			return filterControl;
		}
		protected internal new MVCxPivotPopupFilterControl FilterControl { get { return (MVCxPivotPopupFilterControl)base.FilterControl; } }
	}
	public class MVCxPivotPopupFilterControl: ASPxPivotPopupFilterControl {
		public MVCxPivotPopupFilterControl(IPopupFilterControlOwner filterPopupOwner) :
			base(filterPopupOwner) {
		}
		public object CallbackRouteValues { get; set; }
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
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
}

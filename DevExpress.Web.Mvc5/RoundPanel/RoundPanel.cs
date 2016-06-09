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

using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Internal;
	using DevExpress.Web;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.Web.Mvc.Internal;
	[ToolboxItem(false)]
	public class MVCxRoundPanel : ASPxRoundPanel {
		public MVCxRoundPanel()
			: base() {
		}
		public new RoundPanelImages Images {
			get { return base.Images; }
		}
		public new RoundPanelStyles Styles {
			get { return base.Styles; }
		}
		public object CallbackRouteValues { get; set; }
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null && IsCollapsingAvailable();
		}
		protected override object GetCallbackResult() {
			return Utils.CallbackHtmlContentPlaceholder;
		}
		protected internal new Control GetCallbackResultControl() {
			return base.GetCallbackResultControl();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientRoundPanel";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxRoundPanel), Utils.UtilsScriptResourceName, HasPanelFunctionalityScripts());
			RegisterIncludeScript(typeof(MVCxRoundPanel), Utils.RoundPanelScriptResourceName, HasPanelFunctionalityScripts());
		}
		public override bool IsLoading() {
			return false;
		}
	}
}

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
using System.Text;
using DevExpress.Web.Internal;
using DevExpress.XtraCharts.Web;
namespace DevExpress.Web.Mvc {
	[ToolboxItem(false)]
	public class MVCxChartControl : WebChartControl {
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public new WebChartControlStyles Styles { get { return base.Styles; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool IsCallback { get { return MvcUtils.CallbackName == ID; } }
		public new bool EnableCallBacks { get { return true; } set { } }
		public MVCxChartControl() : base() {
			((ISupportInitialize)this).EndInit();
		}
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + DevExpress.Web.Mvc.Internal.Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if (CustomActionRouteValues != null)
				stb.Append(localVarName + ".customActionUrl=\"" + DevExpress.Web.Mvc.Internal.Utils.GetUrl(CustomActionRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientChart";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxChartControl), DevExpress.Web.Mvc.Internal.Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxChartControl), DevExpress.Web.Mvc.Internal.Utils.ChartScriptResourceName);
		}
		protected internal new void EnsureClientStateLoaded() {
			base.EnsureClientStateLoaded();
		}
		protected override bool NeedLoadClientState() {
			return !IsCallback;
		}
		public override bool IsLoading() {
			return false;
		}
	}
}

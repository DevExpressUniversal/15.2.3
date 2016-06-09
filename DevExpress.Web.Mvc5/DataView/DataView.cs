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
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	[ToolboxItem(false)]
	public class MVCxDataView : ASPxDataView {
		public MVCxDataView()
			: base() {
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public ImagesBase Images { get { return base.ImagesInternal; } }
		public new DataViewStyles Styles { get { return base.Styles; } }
		public override bool IsLoading() { return false; }
		public override bool IsCallback { get { return MvcUtils.CallbackName == ID; } }
		protected internal new bool IsEndlessPagingCallback { get { return base.IsEndlessPagingCallback; } }
		protected internal DVContentControl ContentControl { get { return MainControl.ContentControl; } }
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null;
		}
		protected override string GetCallbackContentControlResult() {
			return Utils.CallbackHtmlContentPlaceholder;
		}
		protected internal new Control GetCallbackResultControl() {
			return base.GetCallbackResultControl();
		}
		protected override object GetCallbackResult() {
			EnsureChildControls();
			return base.GetCallbackResult();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if (CustomActionRouteValues != null)
				stb.Append(localVarName + ".customActionUrl=\"" + Utils.GetUrl(CustomActionRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientDataView";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxDataView), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxDataView), Utils.DataViewScriptResourceName);
		}
		protected internal void PerformOnInit() {
			base.OnInit(EventArgs.Empty);
		}
		protected internal void ValidateProperties() {
			ValidatePageIndex();
		}
		protected internal new bool RequireDataBinding() {
			return base.RequireDataBinding();
		}
		protected override bool CanBindOnLoadPostData() {
			return false;
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new MVCxDataViewBaseDataHelper(this, name);
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	public class MVCxDataViewBaseDataHelper: DataViewBaseDataHelper {
		public MVCxDataViewBaseDataHelper(ASPxDataViewBase dataView, string name)
			: base(dataView, name) {
		}
		protected override bool CanBindOnEnsureDataBound { get { return RequiresDataBinding; } }
	}
}

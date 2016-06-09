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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp.Web;
[assembly: WebResource(DevExpress.ExpressApp.ReportsV2.Web.WebReportPopupController.ScriptResourceName, "application/x-javascript")]
namespace DevExpress.ExpressApp.ReportsV2.Web {
	public abstract class WebReportPopupController : ViewController {
		internal const string ScriptResourceName = "DevExpress.ExpressApp.ReportsV2.Web.xafreportdesigner.js";
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			SetupGlobalEmbedRequiredClientLibraries();
		}
		protected virtual void SetupGlobalEmbedRequiredClientLibraries() {
			if(EmbedRequiredClientLibraries) {
				DevExpress.Web.ASPxWebControl.GlobalEmbedRequiredClientLibraries = true;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			PopupWindow popup = Frame as PopupWindow;
			if(popup != null) {
				RegisterPopupSetupScript(popup);
			}
		}
		protected virtual void RegisterPopupSetupScript(PopupWindow popup) {
			popup.RegisterClientScriptResource(typeof(WebReportDesignerPopupController), ScriptResourceName);
			popup.RegisterStartupScript("popupSettings", PopupSetupScript);
		}
		protected bool EmbedRequiredClientLibraries {
			get {
				ReportsAspNetModuleV2 module = (ReportsAspNetModuleV2)Frame.Application.Modules.FindModule(typeof(ReportsAspNetModuleV2));
				if(module != null) {
					return module.ClientLibrariesLocation == ClientLibrariesLocations.Embedded;
				}
				return true;
			}
		}
		protected abstract string PopupSetupScript { get; }
	}
	public class WebReportDesignerPopupController : WebReportPopupController {
		public WebReportDesignerPopupController() {
			TargetViewId = ReportsAspNetModuleV2.ReportDesignDetailViewWebName;
		}
		protected override string PopupSetupScript {
			get {
				if(WebApplicationStyleManager.IsNewStyle) {
					return "XafReportPopupSetup(xafReportDesigner, 85);";
				}
				return "XafReportDesignerPopupSetup()";
			}
		}
	}
	public class WebReportViewerPopupController : WebReportPopupController {
		public WebReportViewerPopupController() {
			TargetViewId = ReportsAspNetModuleV2.ReportViewDetailViewWebName;
		}
		protected override void SetupGlobalEmbedRequiredClientLibraries() {
			if(Html5ViewerMode) {
				base.SetupGlobalEmbedRequiredClientLibraries();
			}
		}
		protected override void RegisterPopupSetupScript(PopupWindow popup) {
			if(Html5ViewerMode) {
				base.RegisterPopupSetupScript(popup);
			}
		}
		protected override string PopupSetupScript {
			get {
				if(WebApplicationStyleManager.IsNewStyle) {
					return "XafReportPopupSetup(xafReportViewer, 85);";
				}
				return "XafReportViewerPopupSetup()";
			}
		}
		protected bool Html5ViewerMode {
			get {
				ReportsAspNetModuleV2 module = (ReportsAspNetModuleV2)Frame.Application.Modules.FindModule(typeof(ReportsAspNetModuleV2));
				if(module != null) {
					return module.ReportViewerType == ReportViewerTypes.HTML5;
				}
				return false;
			}
		}
	}
}

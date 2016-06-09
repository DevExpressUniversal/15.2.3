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
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;
namespace DevExpress.ExpressApp.Web.Templates {
	public partial class DefaultVerticalTemplateContentNew : TemplateContent, IXafPopupWindowControlContainer, IXafSecurityActionContainerHolder {
		static DefaultVerticalTemplateContentNew() {
			AdditionalClass = "sizeLimit";
		}
		public static void ClearSizeLimit() { AdditionalClass = ""; }
		public static string AdditionalClass { get; set; }
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			Page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.XafNavigation.js");
			Page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.XafFooter.js");
			Page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.DefaultVerticalTemplate.js");
			if(WebWindow.CurrentRequestWindow != null) {
				WebWindow.CurrentRequestWindow.PagePreRender += new EventHandler(CurrentRequestWindow_PagePreRender);
			}
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			IModelApplicationNavigationItems modelApplicationNavigationItems = (IModelApplicationNavigationItems)WebApplication.Instance.Model;
			bool showNavigationOnStart = ((IModelRootNavigationItemsWeb)modelApplicationNavigationItems.NavigationItems).ShowNavigationOnStart;
			ClientParams.Set("ShowNavigationPanelOnStart", showNavigationOnStart);			
			if(!showNavigationOnStart && !navigation.CssClass.Contains("xafHidden")) {
				navigation.CssClass += " xafHidden";
			}			
			WebApplication.Instance.ClientInfo.SetInfo(ClientParams);
		}
		private void CurrentRequestWindow_PagePreRender(object sender, EventArgs e) {
			WebWindow window = (WebWindow)sender;
			window.RegisterStartupScript("Init", "Init();");
		}
		protected override void OnUnload(EventArgs e) {
			if(WebWindow.CurrentRequestWindow != null) {
				WebWindow.CurrentRequestWindow.PagePreRender -= new EventHandler(CurrentRequestWindow_PagePreRender);
			}
			base.OnUnload(e);
		}
		public override void SetStatus(ICollection<string> statusMessages) {
		}
		public override IActionContainer DefaultContainer {
			get {
				if(mainMenu != null) {
					return mainMenu.FindActionContainerById("View");
				}
				return null;
			}
		}
		public override object ViewSiteControl {
			get { return VSC; }
		}
		public XafPopupWindowControl XafPopupWindowControl {
			get { return PopupWindowControl; }
		}
		public override void BeginUpdate() {
			base.BeginUpdate();
			this.SAC.BeginUpdate();
			this.mainMenu.BeginUpdate();
			this.SearchAC.BeginUpdate();
		}
		public override void EndUpdate() {
			this.SAC.EndUpdate();
			this.mainMenu.EndUpdate();
			this.SearchAC.EndUpdate();
			base.EndUpdate();
		}
		ActionContainerHolder IXafSecurityActionContainerHolder.SecurityActionContainerHolder { get { return SAC; } }
	}
}

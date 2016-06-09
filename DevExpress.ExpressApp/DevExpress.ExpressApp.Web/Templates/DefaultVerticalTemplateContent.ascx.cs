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
using System.Text;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using System.Web.UI;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Controls;
namespace DevExpress.ExpressApp.Web.Templates {
	public partial class DefaultVerticalTemplateContent : TemplateContent, IHeaderImageControlContainer, IXafPopupWindowControlContainer
	{
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			WebApplication.Instance.ClientInfo.SetInfo(ClientParams);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(WebWindow.CurrentRequestWindow != null) {
				WebWindow.CurrentRequestWindow.PagePreRender += new EventHandler(CurrentRequestWindow_PagePreRender);
			}
		}
		protected override void OnUnload(EventArgs e) {
			if(WebWindow.CurrentRequestWindow != null) {
				WebWindow.CurrentRequestWindow.PagePreRender -= new EventHandler(CurrentRequestWindow_PagePreRender);
			}
			base.OnUnload(e);
		}
		private void CurrentRequestWindow_PagePreRender(object sender, EventArgs e) {
			WebWindow.CurrentRequestWindow.RegisterStartupScript("OnLoadCore", string.Format(@"Init(""{0}"", ""VerticalCS"");OnLoadCore(""LPcell"", ""separatorCell"", ""separatorImage"", true, true);", BaseXafPage.CurrentTheme), true);
			UpdateTRPVisibility();
		}
		private void UpdateTRPVisibility() {
			bool isVisible = false;
			foreach(Control control in TRP.Controls) {
				if(control is ActionContainerHolder) {
					if(((ActionContainerHolder)control).HasActiveActions()) {
						isVisible = true;
						break;
					}
				}
			}
			TRP.Visible = isVisible;
		}
		public override IActionContainer DefaultContainer {
			get {
				if(TB != null) {
					return TB.FindActionContainerById("View");
				}
				return null;
			}
		}
		public override void SetStatus(ICollection<string> statusMessages) {
			InfoMessagesPanel.Text = string.Join("<br>", new List<string>(statusMessages).ToArray());
		}
		public override object ViewSiteControl {
			get {
				return VSC;
			}
		}
		public ActionContainerHolder LeftToolsActionContainer { get { return VTC; } }
		public ActionContainerHolder DiagnosticActionContainer { get { return DAC; } }
		public ActionContainerHolder MainToolBarActionContainer { get { return TB; } }
		public ActionContainerHolder SecurityActionContainer { get { return SAC; } }
		public ActionContainerHolder TopToolsActionContainer { get { return SHC; } }
		public XafPopupWindowControl XafPopupWindowControl
		{
			get { return PopupWindowControl; }
		}
		public ThemedImageControl HeaderImageControl { get { return TIC; } }
	}
}

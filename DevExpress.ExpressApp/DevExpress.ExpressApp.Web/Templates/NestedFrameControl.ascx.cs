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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using System.Collections.Generic;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
namespace DevExpress.ExpressApp.Web.Templates {
	[ParentControlCssClass("NestedFrameControl")]
	public partial class NestedFrameControl : NestedFrameControlBase, IFrameTemplate, ISupportActionsToolbarVisibility {
		private void ToolBar_MenuItemsCreated(object sender, EventArgs e) {
			DevExpress.Web.ASPxMenu menu = ((ActionContainerHolder)sender).Menu;
			if(!menu.Visible) {
				viewSiteControl.Control.CssClass += " WithoutToolbar";
			}
			if(View is ListView) {
				menu.BorderBottom.BorderWidth = 0;
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(ToolBar != null) {
				ToolBar.MenuItemsCreated += new EventHandler(ToolBar_MenuItemsCreated);
			}
		}
		public override void Dispose() {
			if(ToolBar != null) {
				ToolBar.MenuItemsCreated -= new EventHandler(ToolBar_MenuItemsCreated);
				ToolBar.Dispose();
				ToolBar = null;
			}
			if(UPToolBar != null) {
				UPToolBar.Dispose();
				UPToolBar = null;
			}
			base.Dispose();
		}
		#region IFrameTemplate Members
		public override IActionContainer DefaultContainer {
			get {
				if(ToolBar != null) {
					return ToolBar.FindActionContainerById("View");
				}
				return null;
			}
		}
		public override object ViewSiteControl {
			get {
				return viewSiteControl;
			}
		}
		public override void SetStatus(ICollection<string> statusMessages) {
		}
		#endregion
		#region IActionBarVisibilityManager Members
		private bool toolBarVisibility = true;
		public void SetVisible(bool isVisible) {
			if(ToolBar != null) {
				ToolBar.Visible = isVisible;
			}
			else {
				toolBarVisibility = isVisible;
				Init -= new EventHandler(NestedFrameControl_Init);
				Init += new EventHandler(NestedFrameControl_Init);
			}
		}
		private void NestedFrameControl_Init(object sender, EventArgs e) {
			Init -= new EventHandler(NestedFrameControl_Init);
			ToolBar.Visible = toolBarVisibility;
		}
		#endregion
		protected override ContextActionsMenu CreateContextMenu() {
			return new ContextActionsMenu(this, "Edit", "RecordEdit", "ListView");
		}
	}
}

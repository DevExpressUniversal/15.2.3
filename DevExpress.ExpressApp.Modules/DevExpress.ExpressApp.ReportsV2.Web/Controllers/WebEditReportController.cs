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
using System.Text;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Web;
namespace DevExpress.ExpressApp.ReportsV2.Web {
	public class WebEditReportController : EditReportControllerCore {
		ListViewController listViewController;
		protected override void OnActivated() {
			listViewController = Frame.GetController<ListViewController>();
			if(listViewController != null) {
				EditReportAction = listViewController.EditAction;
			}
			WebWindow window = Frame as WebWindow;
			if(window != null) {
				window.PagePreRender += Frame_PagePreRender;
			}
			base.OnActivated();
		}
		protected override void OnDeactivated() {
			WebWindow window = Frame as WebWindow;
			if(window != null) {
				window.PagePreRender -= Frame_PagePreRender;
			}
			if(listViewController != null) {
				listViewController.EditAction.Enabled.RemoveItem("User-Defined");
				listViewController.EditAction.Enabled.RemoveItem("Security");
			}
			base.OnDeactivated();
		}
		protected virtual void HideInlineReportActions(ASPxGridListEditor gridListEditor) {
			if(gridListEditor != null && gridListEditor.Grid != null) {
				int index = 0;
				foreach(GridViewColumn column in gridListEditor.Grid.VisibleColumns) {
					if(column is GridViewDataActionColumn && IsReportAction(((GridViewDataActionColumn)column).Action.Id)) {
						column.VisibleIndex = -1;
					}
					else {
						column.VisibleIndex = index++;
					}
				}
			}
		}
		private void Frame_PagePreRender(object sender, EventArgs e) {
			if(View != null) {
				ASPxGridListEditor gridListEditor = View.Editor as ASPxGridListEditor;
				HideInlineReportActions(gridListEditor);
			}
		}
		private bool IsReportAction(string id) {
			if(id == "Edit") {
				return true;
			}
			if(id == CopyPredefinedReportsController.CopyPredefinedReportActionId) {
				return true;
			}
			if(id == ShowReportDesignerActionId) {
				return true;
			}
			return false;
		}
	}
}

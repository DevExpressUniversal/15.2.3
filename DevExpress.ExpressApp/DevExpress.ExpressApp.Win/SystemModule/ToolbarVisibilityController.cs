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
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.ExpressApp.SystemModule;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraBars;
using DevExpress.ExpressApp.Templates;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class ToolbarVisibilityController : ViewController {
		private SimpleAction showToolbarAction;
		public ToolbarVisibilityController()
			: base() {
			showToolbarAction = new SimpleAction(this, "ShowToolbar", PredefinedCategory.Menu);
			showToolbarAction.Caption = "Show Toolbar";
			showToolbarAction.Execute += new SimpleActionExecuteEventHandler(showToolbarAction_OnExecute);
			RegisterActions(showToolbarAction);
		}
		private void showToolbarAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			SetToolbarVisibility(true);			
		}
		public void SetToolbarVisibility(bool visible) {
			if(Frame != null && Frame.Template is ISupportActionsToolbarVisibility) {
				((ISupportActionsToolbarVisibility)Frame.Template).SetVisible(visible);
			}
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			Frame.TemplateChanged += new EventHandler(Frame_TemplateChanged);
		}
		private void Frame_TemplateChanged(object sender, EventArgs e) {
			if(Frame != null && Frame.Template != null && Frame.Template is ISupportActionsToolbarVisibility) {
				showToolbarAction.Active.SetItemValue("ISupportActionsToolbarVisibility", true);
			}
			else {
				showToolbarAction.Active.SetItemValue("ISupportActionsToolbarVisibility", false);
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(Frame != null) {
					Frame.TemplateChanged -= new EventHandler(Frame_TemplateChanged);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public SimpleAction ShowToolbarAction {
			get { return showToolbarAction; }
		}
	}
}

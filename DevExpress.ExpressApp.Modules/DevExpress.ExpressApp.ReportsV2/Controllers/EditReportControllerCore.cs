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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.ReportsV2 {
	public abstract class EditReportControllerCore : ReportDataViewController {
		public static readonly string ShowReportDesignerActionId = "ShowReportDesignerV2";
		private SimpleAction editReportAction;
		private SimpleAction showReportDesignerAction;
		public EditReportControllerCore()
			: base() {
			this.showReportDesignerAction = new SimpleAction(this, "ShowReportDesignerV2", PredefinedCategory.Reports);
			this.showReportDesignerAction.Caption = "Show Report Designer";
			this.showReportDesignerAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			this.showReportDesignerAction.ImageName = "MenuBar_ShowReportDesigner";
			this.showReportDesignerAction.Execute += this.showReportDesignerAction_Execute;
		}
		public SimpleAction EditReportAction {
			get { return editReportAction; }
			protected set { editReportAction = value; }
		}
		public SimpleAction ShowReportDesignerAction {
			get { return showReportDesignerAction; }
		}
		protected virtual void ShowReportDesigner(SimpleActionExecuteEventArgs args) {
			Guard.ArgumentNotNull(args.CurrentObject, "args.CurrentObject");
			IReportDataV2 reportData = GetReportData(args.CurrentObject);
			Frame.GetController<ReportServiceController>().ShowDesigner(ReportDataProvider.ReportsStorage.LoadReport(reportData), ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData));
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.SelectionChanged += new EventHandler(View_SelectionChanged);
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void showReportDesignerAction_Execute(Object sender, SimpleActionExecuteEventArgs args) {
			ShowReportDesigner(args);
		}
		private void UpdateActionState() {
			if(View.CurrentObject != null && View.SelectedObjects.Count == 1) {
				bool userDefined = !CurrentReportData.IsPredefined;
				editReportAction.Enabled.SetItemValue("User-Defined", userDefined);
				showReportDesignerAction.Enabled.SetItemValue("User-Defined", userDefined);
				editReportAction.Enabled.SetItemValue("Security", DataManipulationRight.CanEdit(((ObjectView)View).ObjectTypeInfo.Type, null, View.CurrentObject, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace));
				bool isAllowedBySecurity =
					DataManipulationRight.CanEdit(((ObjectView)View).ObjectTypeInfo.Type, null, View.CurrentObject, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace)
					&& DataManipulationRight.CanRead(((ObjectView)View).ObjectTypeInfo.Type, null, View.CurrentObject, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace);
				showReportDesignerAction.Enabled.SetItemValue("Security", isAllowedBySecurity);
			}
			else {
				showReportDesignerAction.Enabled.SetItemValue("Security", false);
				editReportAction.Enabled.SetItemValue("Security", false);
			}
		}
	}
}

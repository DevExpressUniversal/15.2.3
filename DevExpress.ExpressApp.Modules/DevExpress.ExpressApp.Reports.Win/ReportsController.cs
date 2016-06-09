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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.Reports.Win {
	public class ReportsController : ObjectViewController {
		private DevExpress.ExpressApp.Actions.SimpleAction showReportDesignerAction;
		private DevExpress.ExpressApp.Actions.SimpleAction executeReportAction;
		private Type reportDataType;
		private ListViewProcessCurrentObjectController processCurrentObjectController;
		private void executeReportAction_Execute(Object sender, SimpleActionExecuteEventArgs args) {
			ShowReportPreview(args);
		}
		private void showReportDesignerAction_Execute(Object sender, SimpleActionExecuteEventArgs args) {
			ShowReportDesigner(args);
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void processCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			if(!e.Handled) {
				if((Frame.Context != TemplateContext.LookupControl) && (Frame.Context != TemplateContext.LookupWindow)
						&& executeReportAction.Active && executeReportAction.Enabled) {
					e.Handled = true;
					executeReportAction.DoExecute();
				}
			}
		}
		private void UpdateActionState() {
			if(View.SelectedObjects.Count == 1) {
				showReportDesignerAction.Enabled.SetItemValue("Security", DataManipulationRight.CanEdit(((ListView)View).ObjectTypeInfo.Type, null, View.CurrentObject, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace));
			}
			else {
				showReportDesignerAction.Enabled.SetItemValue("Security", false);
			}
		}
		private IReportData GetReportData(Object obj) {
			if(obj is XafDataViewRecord) {
				return View.ObjectSpace.GetObject(obj) as IReportData;
			}
			else {
				return obj as IReportData;
			}
		}
		protected virtual void ShowReportPreview(SimpleActionExecuteEventArgs args) {
			Guard.ArgumentNotNull(args.CurrentObject, "args.CurrentObject");
			Frame.GetController<ReportServiceController>().ShowPreview(GetReportData(args.CurrentObject));
		}
		protected virtual void ShowReportDesigner(SimpleActionExecuteEventArgs args) {
			Guard.ArgumentNotNull(args.CurrentObject, "args.CurrentObject");
			Frame.GetController<WinReportServiceController>().ShowDesigner(GetReportData(args.CurrentObject));
			View.ObjectSpace.Refresh(); 
		}
		protected Type ReportDataType {
			get { return reportDataType; }
		}
		public ReportsController() : base() {
			this.showReportDesignerAction = new SimpleAction(this, "ShowReportDesigner", PredefinedCategory.Reports);
			this.showReportDesignerAction.Caption = "Show Report Designer";
			this.showReportDesignerAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
			this.showReportDesignerAction.ImageName = "MenuBar_ShowReportDesigner";
			this.showReportDesignerAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.showReportDesignerAction_Execute);
			this.executeReportAction = new DevExpress.ExpressApp.Actions.SimpleAction(this, "ExecuteReport", PredefinedCategory.Reports);
			this.executeReportAction.Caption = "Execute Report";
			this.executeReportAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
			this.executeReportAction.ImageName = "MenuBar_PrintPreview";
			this.executeReportAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.executeReportAction_Execute);
			TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
			TargetObjectType = typeof(IReportData);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(ReportsModule.ActivateReportController(this, out reportDataType)) {
				View.SelectionChanged += new EventHandler(View_SelectionChanged);
				View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
				UpdateActionState();
				ListView listView = View as ListView;
				if(listView != null && executeReportAction.Active) {
					processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
					if(processCurrentObjectController != null) {
						processCurrentObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
					}
				}
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(processCurrentObjectController != null) {
				processCurrentObjectController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
			}
		}
		public SimpleAction ExecuteReportAction {
			get { return executeReportAction; }
		}
		public SimpleAction ShowReportDesignerAction {
			get { return showReportDesignerAction; }
		}
	}
}

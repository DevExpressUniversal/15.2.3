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
namespace DevExpress.ExpressApp.ReportsV2 {
	public abstract class ReportsControllerCore : ReportDataViewController {
		private SimpleAction executeReportAction;
		private ListViewProcessCurrentObjectController processCurrentObjectController;
		protected abstract void InitializeActions();
		public ReportsControllerCore()
			: base() {
			InitializeActions();
		}
		public SimpleAction ExecuteReportAction {
			get { return executeReportAction; }
			protected set { executeReportAction = value; }
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(ReportsModuleV2.ActivateReportController(this)) {
				OnActivatedReportController();
			}
			View.SelectionChanged += new EventHandler(View_SelectionChanged);
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void UpdateActionState() {
			bool isReadGranted = DataManipulationRight.CanRead(View.ObjectTypeInfo.Type, null, View.CurrentObject, null, View.ObjectSpace);
			ExecuteReportAction.Enabled.SetItemValue("Security", isReadGranted);
		}
		protected virtual void OnActivatedReportController() {
			ListView listView = View as ListView;
			if(listView != null && executeReportAction.Active) {
				processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
				if(processCurrentObjectController != null) {
					processCurrentObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
				}
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			if(processCurrentObjectController != null) {
				processCurrentObjectController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
			}
		}
		protected virtual void ShowReportPreview(SimpleActionExecuteEventArgs args) {
			Guard.ArgumentNotNull(args.CurrentObject, "args.CurrentObject");
			string reportContainerHandle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(GetReportData(args.CurrentObject));
			Frame.GetController<ReportServiceController>().ShowPreview(reportContainerHandle);
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
	}
}

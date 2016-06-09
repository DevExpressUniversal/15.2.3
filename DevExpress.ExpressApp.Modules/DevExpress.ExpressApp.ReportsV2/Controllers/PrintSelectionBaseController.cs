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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class PrintSelectionBaseController : ObjectViewController {
		public enum ActionEnabledMode {
			None,
			ModifiedChanged,
			ViewMode
		}
		public static ActionEnabledMode ShowInReportActionEnableModeDefault = ActionEnabledMode.None;
		public const string ActiveKeyObjectHasKeyMember = "ObjectHasKeyMember";
		public const string ActiveKeyDisableActionWhenThereAreChanges = "DisableActionWhenThereAreChanges";
		public const string ActiveKeyInplaceReportsAreEnabledInModule = "reportsModule.EnableInplaceReports";
		public const string ActiveKeyViewSupportsSelection = "ViewSupportsSelection";
		public const string ActiveKeyDisableForLookupListView = "DisableForLookupListView";
		public const string ActiveKeyControlsCreated = "ControlsCreated";
		private SingleChoiceAction showInReportAction;
		private IReportInplaceActionsHandler reportInplaceActionsHandler;
		private void showInReportAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			if(e.SelectedObjects.Count > 0) {
				if(e.SelectedChoiceActionItem.Data is ReportDataInfo) {
					ShowInReport(e, ((ReportDataInfo)e.SelectedChoiceActionItem.Data).ReportContainerHandle);
				}
				else {
					if(reportInplaceActionsHandler != null) {
						reportInplaceActionsHandler.ProcessActionItem(e);
					}
				}
			}
		}
		private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_SelectionTypeChanged(object sender, EventArgs e) {
			if(View != null) {
				Active[ActiveKeyViewSupportsSelection] = (View.SelectionType != SelectionType.None);
			}
		}
		private void PrintSelectionBaseController_ViewEditModeChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			Initialize();
		}
		private IReportInplaceActionsHandler FindReportInplaceActionsHandler() {
			foreach(Controller controller in Frame.Controllers) {
				IReportInplaceActionsHandler reportInplaceActionContainer = controller as IReportInplaceActionsHandler;
				if(reportInplaceActionContainer != null) {
					return reportInplaceActionContainer;
				}
			}
			return null;
		}
		protected void Initialize() {
			showInReportAction.Active[ActiveKeyControlsCreated] = true;
			List<ChoiceActionItem> items = new List<ChoiceActionItem>();
			InplaceReportsCacheHelper inplaceReportsCache = ReportsModuleV2.FindReportsModule(Application.Modules).InplaceReportsCacheHelper;
			Type objectType = ((ObjectView)View).ObjectTypeInfo.Type;
			if(inplaceReportsCache != null) {
				List<ReportDataInfo> reportDataList = inplaceReportsCache.GetReportDataInfoList(objectType);
				foreach(ReportDataInfo reportData in reportDataList) {
					ChoiceActionItem item = new ChoiceActionItem(reportData.DisplayName, reportData);
					item.ImageName = "Action_Report_Object_Inplace_Preview";
					items.Add(item);
				}
			}
			reportInplaceActionsHandler = FindReportInplaceActionsHandler();
			if(reportInplaceActionsHandler != null) {
				reportInplaceActionsHandler.SuspendEvents();
				foreach(ReportInplaceActionInfo reportInplaceActionInfo in reportInplaceActionsHandler.GetReportInplaceActionInfo(objectType)) {
					ChoiceActionItem item = new ChoiceActionItem(reportInplaceActionInfo.ActionName, reportInplaceActionInfo.ActionData);
					item.ImageName = "Action_Report_Object_Inplace_Preview";
					items.Add(item);
				}
			}
			items.Sort(delegate(ChoiceActionItem left, ChoiceActionItem right) {
				return Comparer<string>.Default.Compare(left.Caption, right.Caption);
			});
			showInReportAction.Items.Clear();
			showInReportAction.Items.AddRange(items);
			if(ShowInReportActionEnableMode == ActionEnabledMode.ModifiedChanged) {
				View.ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
			}
			else if(ShowInReportActionEnableMode == ActionEnabledMode.ViewMode) {
				if(View is DetailView) {
					((DetailView)View).ViewEditModeChanged += new EventHandler<EventArgs>(PrintSelectionBaseController_ViewEditModeChanged);
				}
			}
			UpdateActionState();
		}
		protected virtual void ShowInReport(SingleChoiceActionExecuteEventArgs e, string reportContainerHandle) {
			CriteriaOperator objectsCriteria = ((BaseObjectSpace)ObjectSpace).GetObjectsCriteria(((ObjectView)View).ObjectTypeInfo, e.SelectedObjects);
			Frame.GetController<ReportServiceController>().ShowPreview(reportContainerHandle, objectsCriteria);
		}
		protected virtual void UpdateActionState() {
			if(View != null) {
				ShowInReportAction.Enabled[PrintSelectionBaseController.ActiveKeyDisableActionWhenThereAreChanges] = true; 
				if(ShowInReportActionEnableMode == ActionEnabledMode.ModifiedChanged) {
					ShowInReportAction.Enabled[PrintSelectionBaseController.ActiveKeyDisableActionWhenThereAreChanges] = !View.ObjectSpace.IsModified;
				}
				else if(ShowInReportActionEnableMode == ActionEnabledMode.ViewMode) {
					if(View is DetailView) {
						ShowInReportAction.Enabled[PrintSelectionBaseController.ActiveKeyDisableActionWhenThereAreChanges] =
							((DetailView)View).ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.View;
					}
				}
			}
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			if(View != null) {
				View.SelectionTypeChanged -= new EventHandler(View_SelectionTypeChanged);
			}
			if(Application != null) {
				ReportsModuleV2 reportsModule = ReportsModuleV2.FindReportsModule(Application.Modules);
				if(reportsModule == null) {
					Active["ReportsModule in Application.Modules"] = false;
				}
				else {
					Active[ActiveKeyInplaceReportsAreEnabledInModule] = reportsModule.EnableInplaceReports;
				}
			}
			Active[ActiveKeyViewSupportsSelection] = (view is ISelectionContext) && (((ISelectionContext)view).SelectionType != SelectionType.None);
			if((view is ObjectView) && (((ObjectView)view).ObjectTypeInfo != null)) {
				Active[ActiveKeyObjectHasKeyMember] = (((ObjectView)view).ObjectTypeInfo.KeyMember != null);
			}
			if(view != null) {
				view.SelectionTypeChanged += new EventHandler(View_SelectionTypeChanged);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			showInReportAction.Active[ActiveKeyControlsCreated] = false;
			if(View.IsControlCreated) {
				Initialize();
			}
			else {
				View.ControlsCreated += new EventHandler(View_ControlsCreated);
			}
		}
		protected override void OnDeactivated() {
			View.ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			if(View is DetailView) {
				((DetailView)View).ViewEditModeChanged -= new EventHandler<EventArgs>(PrintSelectionBaseController_ViewEditModeChanged);
			}
			base.OnDeactivated();
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if((Frame != null) && ((Frame.Context == TemplateContext.LookupWindow) || (Frame.Context == TemplateContext.LookupControl))) {
				this.Active.SetItemValue(ActiveKeyDisableForLookupListView, false);
			}
		}
		public PrintSelectionBaseController() {
			TypeOfView = typeof(ObjectView);
			showInReportAction = new SingleChoiceAction(this, "ShowInReportV2", PredefinedCategory.Reports);
			showInReportAction.Caption = "Show in Report";
			showInReportAction.ToolTip = "Show selected records in a report";
			showInReportAction.Execute += new SingleChoiceActionExecuteEventHandler(showInReportAction_Execute);
			showInReportAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			showInReportAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			showInReportAction.ImageName = "Action_Report_Object_Inplace_Preview";
			ShowInReportActionEnableMode = ShowInReportActionEnableModeDefault;
		}
		public SingleChoiceAction ShowInReportAction {
			get { return showInReportAction; }
		}
		public ActionEnabledMode ShowInReportActionEnableMode { get; set; }
	}
}

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
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.Reports {
	public class InplaceReportCacheController : WindowController {
		private LightDictionary<Type, List<IReportData>> reportsCache = new LightDictionary<Type, List<IReportData>>();
		private Type reportDataType;
		private bool isCompleteCache;
		protected virtual IObjectSpace CreateObjectSpace() {
			Guard.ArgumentNotNull(Application, "Application");
			return Application.CreateObjectSpace(ReportDataType);
		}
		protected LightDictionary<Type, List<IReportData>> ReportsCache {
			get { return reportsCache; }
		}
		static InplaceReportCacheController() {
			IgnoreSecurity = false;
		}
		public InplaceReportCacheController() {
			TargetWindowType = WindowType.Main;
			isCompleteCache = false;
		}
		public void ClearCache() {
			isCompleteCache = false;
			reportsCache.Clear();
		}
		private void EnsureCache() {
			if(!isCompleteCache && typeof(IReportData).IsAssignableFrom(ReportDataType) && typeof(IInplaceReport).IsAssignableFrom(ReportDataType) && (IgnoreSecurity || DataManipulationRight.CanRead(ReportDataType, null, null, null, null))) {
				using(IObjectSpace objectSpace = CreateObjectSpace()) {
					IList allReports = objectSpace.CreateCollection(ReportDataType, new BinaryOperator(ReportsModule.IsInplaceReportMemberName, true));
					Dictionary<string, List<IReportData>> types = new Dictionary<string, List<IReportData>>();
					List<IReportData> result = new List<IReportData>();
					foreach(IInplaceReport inplaceReport in allReports) {
						if(!string.IsNullOrEmpty(inplaceReport.DataTypeName)) {
							List<IReportData> filteredReports = null;
							if(!types.TryGetValue(inplaceReport.DataTypeName, out filteredReports)) {
								filteredReports = new List<IReportData>();
								types[inplaceReport.DataTypeName] = filteredReports;
							}
							filteredReports.Add((IReportData)inplaceReport);
						}
					}
					foreach(string key in types.Keys) {
						ITypeInfo ti = XafTypesInfo.Instance.FindTypeInfo(key);
						if(ti != null) {
							reportsCache.Add(ti.Type, types[key]);
						}
					}
				}
			}
			isCompleteCache = true;
		}
		public virtual List<IReportData> GetReportDataList(Type targetObjectType) {
			if(ReportDataType == null) {
				return new List<IReportData>();
			}
			EnsureCache();
			List<IReportData> cachedReports = new List<IReportData>();
			foreach(Type key in reportsCache.Keys) {
				if(key.IsAssignableFrom(targetObjectType)) {
					cachedReports.AddRange(reportsCache[key]);
				}
			}
			return cachedReports;
		}
		public Type ReportDataType {
			get {
				if(reportDataType == null) {
					reportDataType = ReportsModule.GetCurrentReportDataType(Application.Modules);
				}
				return reportDataType;
			}
			set { reportDataType = value; }
		}
		public static List<IReportData> GetReportDataList(Frame frame, Type targetObjectType) {
			if(frame != null) {
				Frame cachFrame = frame;
				if(frame.Application != null && frame.Application.MainWindow != null) {
					cachFrame = frame.Application.MainWindow;
				}
				InplaceReportCacheController cacheController = cachFrame.GetController<InplaceReportCacheController>();
				if(cacheController != null) {
					return cacheController.GetReportDataList(targetObjectType);
				}
			}
			return new List<IReportData>();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] 
		public static bool IgnoreSecurity { get; set; }
	}
	public class PrintSelectionBaseController : ObjectViewController, IReportInplaceActionsHandler {
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
		private void showInReportAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			if(e.SelectedObjects.Count == 0) {
				return;
			}
			ShowInReport(e, (IReportData)e.SelectedChoiceActionItem.Data);
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
		private List<ChoiceActionItem> CreateInplaceReportActionItems() {
			List<IReportData> reportDataList = InplaceReportCacheController.GetReportDataList(Frame, ((ObjectView)View).ObjectTypeInfo.Type);
			List<ChoiceActionItem> items = new List<ChoiceActionItem>();
			foreach(IReportData reportData in reportDataList) {
				ChoiceActionItem item = new ChoiceActionItem(reportData.ReportName, reportData);
				item.ImageName = "Action_Report_Object_Inplace_Preview";
				items.Add(item);
			}
			return items;
		}
		protected void Initialize() {
			showInReportAction.Active[ActiveKeyControlsCreated] = true;
			List<ChoiceActionItem> items = CreateInplaceReportActionItems();
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
		protected virtual void ShowInReport(SingleChoiceActionExecuteEventArgs e, IReportData reportData) {
			CriteriaOperator objectsCriteria = ((BaseObjectSpace)ObjectSpace).GetObjectsCriteria(((ObjectView)View).ObjectTypeInfo, e.SelectedObjects);
			Frame.GetController<ReportServiceController>().ShowPreview(reportData, objectsCriteria);
		}
		protected virtual void UpdateActionState() {
			if(View == null) {
				return;
			}
			if(ShowInReportActionEnableMode == ActionEnabledMode.ModifiedChanged) {
				ShowInReportAction.Enabled[PrintSelectionBaseController.ActiveKeyDisableActionWhenThereAreChanges] = !View.ObjectSpace.IsModified;
			}
			else if(ShowInReportActionEnableMode == ActionEnabledMode.ViewMode) {
				if(View is DetailView) {
					ShowInReportAction.Enabled[PrintSelectionBaseController.ActiveKeyDisableActionWhenThereAreChanges] =
						((DetailView)View).ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.View;
				}
				else {
					ShowInReportAction.Enabled.RemoveItem(PrintSelectionBaseController.ActiveKeyDisableActionWhenThereAreChanges);
				}
			}
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			if(View != null) {
				View.SelectionTypeChanged -= new EventHandler(View_SelectionTypeChanged);
			}
			if(Application != null) {
				ReportsModule reportsModule = ReportsModule.FindReportsModule(Application.Modules);
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
			if(isSuspended) {
				return;
			}
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
			ShowInReportAction.Enabled.RemoveItem(ActiveKeyDisableActionWhenThereAreChanges);
			showInReportAction.Active.RemoveItem(ActiveKeyControlsCreated);
			base.OnDeactivated();
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if(isSuspended) {
				return;
			}
			if((Frame != null) && ((Frame.Context == TemplateContext.LookupWindow) || (Frame.Context == TemplateContext.LookupControl))) {
				this.Active.SetItemValue(ActiveKeyDisableForLookupListView, false);
			}
		}
		public PrintSelectionBaseController() {
			TypeOfView = typeof(ObjectView);
			showInReportAction = new SingleChoiceAction(this, "ShowInReport", PredefinedCategory.Reports);
			showInReportAction.Caption = "Show in Report";
			showInReportAction.PaintStyle = Templates.ActionItemPaintStyle.CaptionAndImage;
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
		#region IReportInplaceActionsHandler Members
		private bool isSuspended  = false;
		void IReportInplaceActionsHandler.SuspendEvents() {
			isSuspended = true;
			ShowInReportAction.Active["Suspended"] = false;
			if(View != null) {
				View.ControlsCreated -= new EventHandler(View_ControlsCreated);
				View.SelectionTypeChanged -= new EventHandler(View_SelectionTypeChanged);
				View.ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
				if(View is DetailView) {
					((DetailView)View).ViewEditModeChanged -= new EventHandler<EventArgs>(PrintSelectionBaseController_ViewEditModeChanged);
				}
			}
		}
		void IReportInplaceActionsHandler.ProcessActionItem(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
			ShowInReport(singleChoiceActionExecuteEventArgs, (IReportData)singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data);
		}
		List<ReportInplaceActionInfo> IReportInplaceActionsHandler.GetReportInplaceActionInfo(Type objectType) {
			List<ReportInplaceActionInfo> items = new List<ReportInplaceActionInfo>();
			List<IReportData> reportDataList = InplaceReportCacheController.GetReportDataList(Frame, objectType);
			foreach(IReportData reportData in reportDataList) {
				items.Add(new ReportInplaceActionInfo(reportData.ReportName, reportData));
			}
			return items;
		}
		#endregion
	}
}

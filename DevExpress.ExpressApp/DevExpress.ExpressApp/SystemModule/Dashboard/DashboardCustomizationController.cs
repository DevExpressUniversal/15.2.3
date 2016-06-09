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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	public class DashboardCustomizationController : ViewController<DashboardView> {
		public const string RuntimeDashboardCustomizationEnabledKey = "RuntimeDashboardCustomizationEnabled";
		private PopupWindowShowAction organizeDashboard;
		private void organizeDashboard_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
			((DashboardOrganizer)e.PopupWindow.View.CurrentObject).SaveDashboardChangesToModel();
			View.LoadModel();
		}
		private void organizeDashboard_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
			View.SaveModel();
			DashboardOrganizer dashboardOrganizer = new DashboardOrganizer(View.Model);
			DetailView dashboardOrganizerView = Application.CreateDetailView(Application.CreateObjectSpace(typeof(DashboardOrganizer)), dashboardOrganizer);
			e.View = dashboardOrganizerView;
		}
		protected override void UpdateActionActivity(ActionBase action) {
			base.UpdateActionActivity(action);
			IModelOptionsDashboards dashboardOptions = Application.Model.Options as IModelOptionsDashboards;
			if (dashboardOptions != null) {
				organizeDashboard.Active.SetItemValue(RuntimeDashboardCustomizationEnabledKey, dashboardOptions.Dashboards.EnableCustomization);
			}
		}
		public DashboardCustomizationController() {
			organizeDashboard = new PopupWindowShowAction(this, "OrganizeDashboard", PredefinedCategory.Edit);
			organizeDashboard.ImageName = "Action_OrganizeDashboard";
			organizeDashboard.Execute += new PopupWindowShowActionExecuteEventHandler(organizeDashboard_Execute);
			organizeDashboard.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(organizeDashboard_CustomizePopupWindowParams);
		}
		public PopupWindowShowAction OrganizeDashboardAction {
			get {
				return organizeDashboard;
			}
		}
	}
	public class DashboardOrganizerItemsCollectionsController : ViewController<ListView> {
		private const string hideItemsFromDashboardActionCaption = "Hide from dashboard";
		private const string showItemsOnDashboardActionCaption = "Show on dashboard";
		private const string hideItemsFromDashboardActionTooltip = "Hide selected item(s) from dashboard";
		private const string showItemsOnDashboardActionTooltip = "Show selected item(s) on dashboard";
		private const string hideItemsFromDashboardActionImage = "Action_HideItemFromDashboard";
		private const string showItemsOnDashboardActionImage = "Action_ShowItemOnDashboard";
		private SimpleAction deleteItemAction;
		private SimpleAction hideItemsFromDashboardAction;
		private SimpleAction showItemsOnDashboardAction;
		private void newObjectController_CollectDescendantTypes(object sender, CollectTypesEventArgs e) {
			foreach (ITypeInfo descendant in XafTypesInfo.Instance.FindTypeInfo(typeof(DashboardOrganizationItem)).Descendants) {
				if (!descendant.IsAbstract) {
					e.Types.Add(descendant.Type);
				}
			}
		}
		private void newObjectController_ObjectCreating(object sender, ObjectCreatingEventArgs e) {
			e.ShowDetailView = false;
			ShowNewObjectPopup(e);
		}
		private void DisableControllers() {
			Frame.GetController<RecordsNavigationController>().Active["IsPersistent"] = false;
			Frame.GetController<ExportAnalysisController>().Active["IsExportNeeded"] = false;
		}
		private void controller_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			DetailView currentObjectView = Application.CreateDetailView(Application.CreateObjectSpace(e.InnerArgs.CurrentObject.GetType()), e.InnerArgs.CurrentObject, true);
			currentObjectView.ViewEditMode = ViewEditMode.Edit;
			ShowViewParameters parameters = new ShowViewParameters(currentObjectView);
			parameters.TargetWindow = TargetWindow.NewModalWindow;
			parameters.Context = TemplateContext.PopupWindow;
			DialogController dialogController = Application.CreateController<DialogController>();
			dialogController.CancelAction.Active.SetItemValue("Editing Dashboard Item", false);
			dialogController.AcceptAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "Close", "Close");
			dialogController.AcceptAction.ToolTip = dialogController.AcceptAction.Caption;
			parameters.Controllers.Add(dialogController);
			Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(null, null));
			e.Handled = true;
		}
		private DashboardOrganizer GetDashboardOrganizer() {
			return ((PropertyCollectionSource)View.CollectionSource).MasterObject as DashboardOrganizer;
		}
		private void deleteItemAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			foreach (object obj in e.SelectedObjects) {
				View.CollectionSource.Remove(obj);
			}
		}
		private void SetDashboardItemsVisibility(IList items, ViewItemVisibility visibility) {
			foreach (DashboardOrganizationItem item in items) {
				item.Visibility = visibility;
				View.ObjectSpace.SetModified(item);
			}
		}
		private void hideItemsFromDashboardAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			SetDashboardItemsVisibility(e.SelectedObjects, ViewItemVisibility.Hide);
		}
		private void showItemsOnDashboardAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			SetDashboardItemsVisibility(e.SelectedObjects, ViewItemVisibility.Show);
		}
		protected virtual void ShowNewObjectPopup(ObjectCreatingEventArgs args) {
			object newObject = System.Activator.CreateInstance(args.ObjectType, Application.Model);
			args.NewObject = newObject;
			DetailView newObjectView = Application.CreateDetailView(Application.CreateObjectSpace(args.ObjectType), newObject, true);
			newObjectView.ViewEditMode = ViewEditMode.Edit;
			ShowViewParameters parameters = new ShowViewParameters(newObjectView);
			parameters.TargetWindow = TargetWindow.NewModalWindow;
			parameters.Context = TemplateContext.PopupWindow;
			DialogController dialogController = Application.CreateController<DialogController>();
			dialogController.Accepting += delegate(object sender, DialogControllerAcceptingEventArgs e) {
				View.CollectionSource.Add(newObject);
			};
			dialogController.Cancelling += delegate(object sender, EventArgs e) {
				args.Cancel = true;
			};
			parameters.Controllers.Add(dialogController);
			Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(null, null));
		}
		protected override void OnActivated() {
			base.OnActivated();
			DisableControllers();
			NewObjectViewController newObjectController = Frame.GetController<NewObjectViewController>();
			if (newObjectController != null) {
				newObjectController.CollectDescendantTypes += new EventHandler<CollectTypesEventArgs>(newObjectController_CollectDescendantTypes);
				newObjectController.ObjectCreating += new EventHandler<ObjectCreatingEventArgs>(newObjectController_ObjectCreating);
			}
			ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
			if (controller != null) {
				controller.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(controller_CustomProcessSelectedItem);
			}
		}
		protected override void OnDeactivated() {
			NewObjectViewController newObjectController = Frame.GetController<NewObjectViewController>();
			if (newObjectController != null) {
				newObjectController.CollectDescendantTypes -= new EventHandler<CollectTypesEventArgs>(newObjectController_CollectDescendantTypes);
				newObjectController.ObjectCreating -= new EventHandler<ObjectCreatingEventArgs>(newObjectController_ObjectCreating);
			}
			base.OnDeactivated();
		}
		public DashboardOrganizerItemsCollectionsController() {
			TargetObjectType = typeof(DashboardOrganizationItem);
			TargetViewNesting = Nesting.Nested;
			deleteItemAction = new SimpleAction(this, "Delete Item", PredefinedCategory.Edit);
			deleteItemAction.Caption = "Delete Item";
			deleteItemAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			deleteItemAction.ImageName = "MenuBar_Delete";
			deleteItemAction.Execute += new SimpleActionExecuteEventHandler(deleteItemAction_Execute);
			deleteItemAction.ConfirmationMessage = "You are about to delete the selected item(s). Do you want to proceed?";
			hideItemsFromDashboardAction = new SimpleAction(this, "HideItemsFromDashboard", PredefinedCategory.Edit);
			hideItemsFromDashboardAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			hideItemsFromDashboardAction.ImageName = hideItemsFromDashboardActionImage;
			hideItemsFromDashboardAction.Caption = hideItemsFromDashboardActionCaption;
			hideItemsFromDashboardAction.ToolTip = hideItemsFromDashboardActionTooltip;
			hideItemsFromDashboardAction.TargetObjectsCriteria = (new OperandProperty(DashboardOrganizationItem.VisibilityPropertyName) == new OperandValue(ViewItemVisibility.Show)).ToString();
			hideItemsFromDashboardAction.TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueAtLeastForOne;
			hideItemsFromDashboardAction.Execute += new SimpleActionExecuteEventHandler(hideItemsFromDashboardAction_Execute);
			showItemsOnDashboardAction = new SimpleAction(this, "ShowItemsOnDashboard", PredefinedCategory.Edit);
			showItemsOnDashboardAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			showItemsOnDashboardAction.ImageName = showItemsOnDashboardActionImage;
			showItemsOnDashboardAction.Caption = showItemsOnDashboardActionCaption;
			showItemsOnDashboardAction.ToolTip = showItemsOnDashboardActionTooltip;
			showItemsOnDashboardAction.TargetObjectsCriteria = (new OperandProperty(DashboardOrganizationItem.VisibilityPropertyName) == new OperandValue(ViewItemVisibility.Hide)).ToString();
			showItemsOnDashboardAction.TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueAtLeastForOne;
			showItemsOnDashboardAction.Execute += new SimpleActionExecuteEventHandler(showItemsOnDashboardAction_Execute);
		}
	}
	public class DashboardDeactivateItemsActionsController : ViewController<DashboardView> {
		private const string deactivateReason = "Inactive in Dashboard";
		protected override void OnActivated() {
			base.OnActivated();
			foreach (ViewItem item in View.Items) {
				DashboardViewItem dashboardViewItem = item as DashboardViewItem;
				if (dashboardViewItem != null && dashboardViewItem.Frame != null) {
					ModificationsController controller = dashboardViewItem.Frame.GetController<ModificationsController>();
					if (controller != null) {
						controller.SaveAndCloseAction.Active.SetItemValue(deactivateReason, false);
						controller.SaveAndNewAction.Active.SetItemValue(deactivateReason, false);
					}
				}
			}
		}
	}
}

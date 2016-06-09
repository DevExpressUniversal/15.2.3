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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.SystemModule;
using System.ComponentModel;
using DevExpress.XtraScheduler;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraScheduler.Commands;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Scheduler.Web {
	public class SchedulerActionsController : ObjectViewController {
		private const string isOccurenceKey = "IsOccurence";
		private const string isChangedOccurenceKey = "IsChangedOccurence";
		private const string isEditActionActiveKey = "IsEditActionActive";
		private const string isEditActionEnabledKey = "isEditActionEnabled";
		private const string isNotPatternKey = "IsNotPattern";
		private const string editorIsSchedulerkey = "EditorIsScheduler";
		private SimpleAction editSeriesAction;
		private SimpleAction openSeriesAction;
		private SimpleAction restoreOccurrenceAction;
		private ListViewController listViewController;
		private ASPxSchedulerListEditor schedulerListEditor;
		private bool isInEditSeriesExecuting;
		private void InitializeComponents() {
			this.editSeriesAction = new SimpleAction();
			this.editSeriesAction.Caption = "Edit Series";
			this.editSeriesAction.Category = PredefinedCategory.Edit.ToString();
			this.editSeriesAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			this.editSeriesAction.Id = "EditSeries";
			this.editSeriesAction.Execute += new SimpleActionExecuteEventHandler(editSeriesAction_Execute);
			this.openSeriesAction = new SimpleAction();
			this.openSeriesAction.Caption = "Open Series";
			this.openSeriesAction.Category = PredefinedCategory.Edit.ToString();
			this.openSeriesAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			this.openSeriesAction.Id = "OpenSeries";
			this.openSeriesAction.Execute += new SimpleActionExecuteEventHandler(openSeriesAction_Execute);
			this.restoreOccurrenceAction = new SimpleAction();
			this.restoreOccurrenceAction.Caption = "Restore Occurrence";
			this.restoreOccurrenceAction.Category = PredefinedCategory.Edit.ToString();
			this.restoreOccurrenceAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			this.restoreOccurrenceAction.Id = "RestoreOccurrence";
			this.restoreOccurrenceAction.Execute += new SimpleActionExecuteEventHandler(restoreOccurrenceAction_Execute);
		}
		private void FocusRecurrencePattern(IRecurrentEvent schedulerEvent) {
			if(schedulerEvent != null && schedulerEvent.Type != (Int32)AppointmentType.Pattern) {
				if(schedulerListEditor != null) {
					schedulerListEditor.SelectEvent(schedulerEvent.RecurrencePattern);
				}
			}
		}
		private void EditFocusedEvent() {
			if(listViewController != null && listViewController.EditAction != null) {
				try {
					isInEditSeriesExecuting = true;
					listViewController.EditAction.DoExecute();
				}
				finally {
					isInEditSeriesExecuting = false;
				}
			}
		}
		private void OpenFocusedEvent() {
			if(Frame != null) {
				SimpleAction processCurrentObjectAction = GetProcessCurrentObjectAction();
				if(schedulerListEditor != null &&  processCurrentObjectAction != null) {
					if(processCurrentObjectAction != null && processCurrentObjectAction.Active) {
						bool isNotPattern = processCurrentObjectAction.Enabled[isNotPatternKey];
						processCurrentObjectAction.Enabled[isNotPatternKey] = true;
						if(processCurrentObjectAction.Enabled) {
							processCurrentObjectAction.DoExecute();
						}
						processCurrentObjectAction.Enabled[isNotPatternKey] = isNotPattern;
					}
				}
			}
		}
		private void ProcessSeriesAction(IRecurrentEvent schedulerEvent, ViewEditMode viewEditMode) {
			FocusRecurrencePattern(schedulerEvent);
			if(viewEditMode == ViewEditMode.Edit) {
				EditFocusedEvent();
			}
			else {
				OpenFocusedEvent();
			}
		}
		private void editSeriesAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			IRecurrentEvent schedulerEvent = e.SelectedObjects[0] as IRecurrentEvent;
			ProcessSeriesAction(schedulerEvent, ViewEditMode.Edit);
		}
		private void openSeriesAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			IRecurrentEvent schedulerEvent = e.SelectedObjects[0] as IRecurrentEvent;
			ProcessSeriesAction(schedulerEvent, ViewEditMode.View);
		}
		private void restoreOccurrenceAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			SchedulerListViewController schedulerListViewController = Frame.GetController<SchedulerListViewController>();
			if(schedulerListEditor != null && schedulerListViewController != null) {
				schedulerListViewController.SuppressConfirmationShowing();
				DeleteObjectsViewController deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
				if(deleteObjectsViewController != null && deleteObjectsViewController.DeleteAction != null &&
					deleteObjectsViewController.DeleteAction.Active && deleteObjectsViewController.DeleteAction.Enabled) {
					deleteObjectsViewController.DeleteAction.DoExecute();
				}
				schedulerListViewController.RestoreConfirmationShowing();
			}
		}
		private void schedulerListEditor_SelectionChanged(object sender, EventArgs e) {
			UpdateActionsState();
		}
		private void RemoveIsNotPatternKey() {
			SimpleAction processCurrentObjectAction = GetProcessCurrentObjectAction();
			if(processCurrentObjectAction != null) {
				processCurrentObjectAction.Enabled.RemoveItem(isNotPatternKey);
			}
		}
		private void UpdateActionsState() {
			bool schedulerIsNotNull = schedulerListEditor != null;
			editSeriesAction.Active[editorIsSchedulerkey] = schedulerIsNotNull;
			openSeriesAction.Active[editorIsSchedulerkey] = schedulerIsNotNull;
			restoreOccurrenceAction.Active[editorIsSchedulerkey] = schedulerIsNotNull;
			editSeriesAction.Enabled.RemoveItem(isOccurenceKey);
			editSeriesAction.Enabled.RemoveItem(isEditActionEnabledKey);
			editSeriesAction.Active.RemoveItem(isEditActionActiveKey);
			openSeriesAction.Enabled.RemoveItem(isOccurenceKey);
			restoreOccurrenceAction.Enabled.RemoveItem(isChangedOccurenceKey);
			RemoveIsNotPatternKey();
			if(schedulerIsNotNull && schedulerListEditor.IsSchedulerControlLoaded) {
				editSeriesAction.Enabled.SetItemValue(isOccurenceKey, schedulerListEditor.IsOccurrenceFocused);
				if(listViewController != null) {
					editSeriesAction.Enabled.SetItemValue(isEditActionEnabledKey, listViewController.EditAction.Enabled);
					editSeriesAction.Active.SetItemValue(isEditActionActiveKey, listViewController.EditAction.Active);
				}
				openSeriesAction.Enabled.SetItemValue(isOccurenceKey, schedulerListEditor.IsOccurrenceFocused);
				restoreOccurrenceAction.Enabled.SetItemValue(isChangedOccurenceKey, schedulerListEditor.IsChangedOccurrenceFocused);
				if(schedulerListEditor.FocusedObject != null) {
					SimpleAction processCurrentObjectAction = GetProcessCurrentObjectAction();
					if(processCurrentObjectAction != null) {
						if(schedulerListEditor.FocusedObject is XafDataViewRecord) {
							processCurrentObjectAction.Enabled[isNotPatternKey] = (int)((XafDataViewRecord)schedulerListEditor.FocusedObject)["Type"] != (int)AppointmentType.Pattern;
						}
						else {
							processCurrentObjectAction.Enabled[isNotPatternKey] = ((IEvent)schedulerListEditor.FocusedObject).Type != (int)AppointmentType.Pattern;
						}
					}
				}
			}
		}
		private SimpleAction GetProcessCurrentObjectAction() {
			ListViewProcessCurrentObjectController listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(listViewProcessCurrentObjectController != null) {
				return listViewProcessCurrentObjectController.ProcessCurrentObjectAction;
			}
			return null;
		}
		private void listViewController_CustomExecuteEdit(object sender, HandledEventArgs e) {
			if(!isInEditSeriesExecuting) {
				if(schedulerListEditor != null) {
					if(schedulerListEditor.IsPatternFocused) {
						e.Handled = true;
						schedulerListEditor.AssignCustomProperties(schedulerListEditor.SelectedAppointments[0]);
						schedulerListEditor.RaiseNewAction(schedulerListEditor.SelectedAppointments[0]);
					}
				}
			}
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateActionsState();
		}
		protected override void OnActivated() {
			base.OnActivated();
			listViewController = Frame.GetController<ListViewController>();
			if(listViewController != null) {
				listViewController.CustomExecuteEdit += new HandledEventHandler(listViewController_CustomExecuteEdit);
			}
			schedulerListEditor = ((ListView)View).Editor as ASPxSchedulerListEditor;
			if(schedulerListEditor != null) {
				schedulerListEditor.SelectionChanged += new EventHandler(schedulerListEditor_SelectionChanged);
			}
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			UpdateActionsState();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(listViewController != null) {
				listViewController.CustomExecuteEdit -= new HandledEventHandler(listViewController_CustomExecuteEdit);
			}
			if(schedulerListEditor != null) {
				schedulerListEditor.SelectionChanged -= new EventHandler(schedulerListEditor_SelectionChanged);
				schedulerListEditor = null;
			}
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			RemoveIsNotPatternKey();
		}
		public SchedulerActionsController() {
			this.TargetViewType = ViewType.ListView;
			this.TargetObjectType = typeof(IRecurrentEvent);
			InitializeComponents();
			RegisterActions(editSeriesAction, openSeriesAction, restoreOccurrenceAction);
		}
		public SimpleAction EditSeriesAction { get { return editSeriesAction; } }
		public SimpleAction OpenSeriesAction { get { return openSeriesAction; } }
		public SimpleAction RestoreOccurrenceAction { get { return restoreOccurrenceAction; } }
	}
	public class SchedulerDetailViewActionsController : ObjectViewController {
		private SimpleAction editSeriesAction;
		private WebModificationsController modificationsController;
		private string editActionCaption;
		private void InitializeComponents() {
			this.editSeriesAction = new SimpleAction();
			this.editSeriesAction.Caption = "Edit Series";
			this.editSeriesAction.Category = PredefinedCategory.Edit.ToString();
			this.editSeriesAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			this.editSeriesAction.Id = "EditSeriesDetailView";
			this.editSeriesAction.Execute += new SimpleActionExecuteEventHandler(editSeriesAction_Execute);
		}
		private void SchedulerDetailViewController_ViewEditModeChanged(object sender, EventArgs e) {
			UpdateActionsState();
		}
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			UpdateActionsState();
		}
		private void EditAction_Changed(object sender, ActionChangedEventArgs e) {
			if(e.ChangedPropertyType == ActionChangedType.Active || e.ChangedPropertyType == ActionChangedType.Enabled) {
				UpdateActionsState();
			}
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateActionsState();
		}
		private void detailViewController_Activated(object sender, EventArgs e) {
			UpdateActionsState();
		}
		private void UpdateActionsState() {
			const string isChangedOccurrenceKey = "IsChangedOccurrence";
			const string editActionActiveKey = "IsEditActionActive";
			const string editActionEnabledKey = "IsEditActionEnabled";
			editSeriesAction.Active.SetItemValue(isChangedOccurrenceKey, false);
			editSeriesAction.Active.SetItemValue(editActionActiveKey, false);
			editSeriesAction.Enabled.SetItemValue(editActionEnabledKey, false);
			if(modificationsController != null) {
				modificationsController.EditAction.Caption = editActionCaption;
				editSeriesAction.Enabled.SetItemValue(editActionEnabledKey, modificationsController.EditAction.Enabled);
				editSeriesAction.Active.SetItemValue(editActionActiveKey, modificationsController.EditAction.Active);
				if(View.CurrentObject != null) {
					AppointmentType eventType = (AppointmentType)((IEvent)View.CurrentObject).Type;
					editSeriesAction.Active.SetItemValue(isChangedOccurrenceKey, eventType == AppointmentType.ChangedOccurrence);
					if(eventType == AppointmentType.Pattern) {
						modificationsController.EditAction.Caption = editSeriesAction.Caption;
					}
				}
			}
		}
		private void editSeriesAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			IRecurrentEvent schedulerEvent = (IRecurrentEvent)View.CurrentObject;
			if(schedulerEvent.Type == (int)AppointmentType.ChangedOccurrence) {
				View.CurrentObject = schedulerEvent.RecurrencePattern;
				modificationsController.EditAction.DoExecute();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			((DetailView)View).ViewEditModeChanged += new EventHandler<EventArgs>(SchedulerDetailViewController_ViewEditModeChanged);
			modificationsController = Frame.GetController<WebModificationsController>();
			if(modificationsController != null) {
				modificationsController.Activated += new EventHandler(detailViewController_Activated);
				modificationsController.EditAction.Changed += new EventHandler<ActionChangedEventArgs>(EditAction_Changed);
				editActionCaption = modificationsController.EditAction.Caption;
			}
			ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			UpdateActionsState();
		}
		protected override void OnDeactivated() {
			((DetailView)View).ViewEditModeChanged -= new EventHandler<EventArgs>(SchedulerDetailViewController_ViewEditModeChanged);
			if(modificationsController != null) {
				modificationsController.Activated -= new EventHandler(detailViewController_Activated);
				modificationsController.EditAction.Changed -= new EventHandler<ActionChangedEventArgs>(EditAction_Changed);
				modificationsController.EditAction.Caption = editActionCaption;
			}
			ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			base.OnDeactivated();
		}
		public SchedulerDetailViewActionsController() {
			this.TargetViewType = ViewType.DetailView;
			this.TargetObjectType = typeof(IRecurrentEvent);
			InitializeComponents();
			RegisterActions(editSeriesAction);
		}
		public SimpleAction EditSeriesAction {
			get { return editSeriesAction; }
		}
	}
}

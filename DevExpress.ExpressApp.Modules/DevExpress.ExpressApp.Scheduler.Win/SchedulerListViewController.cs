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
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraScheduler;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;
using System.Windows.Forms;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.Scheduler.Win {
	public class SchedulerListViewController : SchedulerListViewControllerBase {
		private const bool CanManageEditRecurrentAppointmentFormShowingDefault = true;
		private const bool CanManagePreparePopupMenuAppointmentDefault = true;
		private SchedulerListEditor schedulerEditor;
		private SchedulerControl scheduler;
		private void scheduler_KeyDown(object sender, KeyEventArgs e) {
			if((e.KeyCode == Keys.Delete) && (e.Modifiers == Keys.None)) {
				DeleteSelectedAppointments();
			}
		}
		private void DeleteSelectedAppointments() {
			if((deleteObjectsViewController != null) && (deleteObjectsViewController.DeleteAction != null)
					&& deleteObjectsViewController.DeleteAction.Active && deleteObjectsViewController.DeleteAction.Enabled) {
				String confirmationMessage = deleteObjectsViewController.DeleteAction.GetFormattedConfirmationMessage();
				if(!ShowConfirmations || (String.IsNullOrEmpty(confirmationMessage))
						|| (WinApplication.Messaging.GetUserChoice(confirmationMessage, deleteObjectsViewController.DeleteAction.Caption, MessageBoxButtons.YesNo) == DialogResult.Yes)) {
					deleteObjectsViewController.DeleteAction.DoExecute();
				}
			}
		}
		private void DeleteAppointmentMenuItem_Click(object sender, EventArgs e) {
			DeleteSelectedAppointments();
		}
		private void scheduler_EditRecurrentAppointmentFormShowing(object sender, EditRecurrentAppointmentFormEventArgs e) {
			if(CanManageEditRecurrentAppointmentFormShowing) {
				NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
				if(controller != null) {
					SingleChoiceAction newObjectAction = controller.NewObjectAction;
					if(!newObjectAction.Enabled || !newObjectAction.Active || !IsChoiceItemForTypeActive(newObjectAction, View.ObjectTypeInfo.Type)) {
						e.QueryResult = RecurrentAppointmentAction.Series; 
						e.DialogResult = DialogResult.OK;
						e.Handled = true;
					}
				}
			}
		}
		private void scheduler_PreparePopupMenu(object sender, PopupMenuShowingEventArgs e) {
			SchedulerMenuItem restoreOccurrenceMenuItem = e.Menu.GetMenuItemById(SchedulerMenuItemId.RestoreOccurrence, true);
			if(restoreOccurrenceMenuItem != null) {
				restoreOccurrenceMenuItem.Click -= new EventHandler(restoreOccurrenceMenuItem_Click);
				restoreOccurrenceMenuItem.Click += new EventHandler(restoreOccurrenceMenuItem_Click);
			}
			SchedulerMenuItem deleteAppointmentMenuItem = e.Menu.GetMenuItemById(SchedulerMenuItemId.DeleteAppointment, true);
			if(deleteAppointmentMenuItem != null) {
				if((deleteObjectsViewController != null) && (deleteObjectsViewController.DeleteAction != null)
						&& deleteObjectsViewController.DeleteAction.Active && deleteObjectsViewController.DeleteAction.Enabled) {
					e.Menu.EnableMenuItem(SchedulerMenuItemId.DeleteAppointment);
					deleteAppointmentMenuItem.Click -= new EventHandler(DeleteAppointmentMenuItem_Click);
					deleteAppointmentMenuItem.Click += new EventHandler(DeleteAppointmentMenuItem_Click);
				}
				else {
					e.Menu.DisableMenuItem(SchedulerMenuItemId.DeleteAppointment);
				}
			}
			if(e.Menu.Id == SchedulerMenuItemId.DefaultMenu) {
				NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
				if(controller != null) {
					SingleChoiceAction newObjectAction = controller.NewObjectAction;
					if(!newObjectAction.Enabled) {
						SchedulerMenuItem newAppointmentMenuItem = e.Menu.GetMenuItemById(SchedulerMenuItemId.NewAppointment, true);
						if(newAppointmentMenuItem!=null) {
							newAppointmentMenuItem.Enabled = false;
						}
						SchedulerMenuItem newAllDayEventMenuItem = e.Menu.GetMenuItemById(SchedulerMenuItemId.NewAllDayEvent, true);
						if(newAllDayEventMenuItem!=null) {
							newAllDayEventMenuItem.Enabled = false;
						}
						SchedulerMenuItem newRecurringEventMenuItem = e.Menu.GetMenuItemById(SchedulerMenuItemId.NewRecurringEvent, true);
						if(newRecurringEventMenuItem != null) {
							newRecurringEventMenuItem.Enabled = false;
						}
					}
					if(!newObjectAction.Active || !IsChoiceItemForTypeActive(newObjectAction, View.ObjectTypeInfo.Type)) {
						e.Menu.RemoveMenuItem(SchedulerMenuItemId.NewAppointment);
						e.Menu.RemoveMenuItem(SchedulerMenuItemId.NewAllDayEvent);
						e.Menu.RemoveMenuItem(SchedulerMenuItemId.NewRecurringEvent);
					}
				}
			}
			if(CanManagePreparePopupMenuAppointment) {
				if(e.Menu.Id == SchedulerMenuItemId.AppointmentMenu) {
					NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
					if(controller != null) {
						SingleChoiceAction newObjectAction = controller.NewObjectAction;
						if(!newObjectAction.Enabled || !newObjectAction.Active || !IsChoiceItemForTypeActive(newObjectAction, View.ObjectTypeInfo.Type)) {
							e.Menu.RemoveMenuItem(SchedulerMenuItemId.OpenAppointment); 
						}
					}
				}
			}
		}
		private bool IsChoiceItemForTypeActive(SingleChoiceAction newObjectAction, Type type) {
			foreach(ChoiceActionItem item in newObjectAction.Items) {
				if((Type)item.Data == type) {
					return true;
				}
			}
			return false;
		}
		private void restoreOccurrenceMenuItem_Click(object sender, EventArgs e) {
			SuppressConfirmationShowing();
			if(schedulerEditor.GetSelectedObjects().Count == 1) {
				DeleteSelectedAppointments();
			}
			RestoreConfirmationShowing();
		}
		private void View_ControlsCreated2(object sender, EventArgs e) {
			if(((ListView)View).Editor is SchedulerListEditorBase) { 
				WinModificationsController objectViewController = Frame.GetController<WinModificationsController>();
				if(objectViewController != null) {
					objectViewController.ModificationsHandlingMode = ModificationsHandlingMode.AutoCommit;
				}
			}
		}
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			AssignResourcesDataSource();
		}
		protected override void OnViewInfoChanged() {
			base.OnViewInfoChanged();
			AssignResourcesDataSource();
		}
		protected override void OnControlsCreated() {
			base.OnControlsCreated();
			AssignResourcesDataSource();
		}
		protected override void OnDeleting(IList selectedObjects) {
			base.OnDeleting(selectedObjects);
			if(ShowConfirmations) {
				SchedulerDeleteHelper deleteHelper = schedulerEditor.SchedulerDeleteHelper;
				deleteHelper.StartProcessing(selectedObjects, ObjectSpace);
				Refill(selectedObjects, deleteHelper.GetObjectsToDelete());
			}
		}
		protected override void SubscribeToSchedulerListEditorEvents() {
			base.SubscribeToSchedulerListEditorEvents();
			schedulerEditor = ((ListView)View).Editor as SchedulerListEditor;
			if(schedulerEditor != null) {
				schedulerEditor.NewAction += new EventHandler<NewActionEventArgs>(schedulerListEditor_NewAction);
				schedulerEditor.ObjectChanged += new EventHandler(schedulerListEditor_ObjectChanged);
				schedulerEditor.DataSourceChanged += new EventHandler(schedulerListEditor_DataSourceChanged);
				NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
				if(newObjectViewController != null) {
					newObjectViewController.ObjectCreated += new EventHandler<ObjectCreatedEventArgs>(NewObjectViewController_ObjectCreated);
					newObjectViewController.ObjectCreating += new EventHandler<ObjectCreatingEventArgs>(NewObjectViewController_ObjectCreating);
				}
				scheduler = schedulerEditor.SchedulerControl as SchedulerControl;
				if(scheduler != null) {
					scheduler.PopupMenuShowing += new PopupMenuShowingEventHandler(scheduler_PreparePopupMenu);
					scheduler.EditRecurrentAppointmentFormShowing += scheduler_EditRecurrentAppointmentFormShowing;
					scheduler.KeyDown += new KeyEventHandler(scheduler_KeyDown);
				}
			}
		}
		protected override void UnsubscribeFromSchedulerListEditorEvents() {
			base.UnsubscribeFromSchedulerListEditorEvents();
			if(schedulerEditor != null) {
				schedulerEditor.NewAction -= new EventHandler<NewActionEventArgs>(schedulerListEditor_NewAction);
				schedulerEditor.ObjectChanged -= new EventHandler(schedulerListEditor_ObjectChanged);
				schedulerEditor.DataSourceChanged -= new EventHandler(schedulerListEditor_DataSourceChanged);
				NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
				if(newObjectViewController != null) {
					newObjectViewController.ObjectCreating -= new EventHandler<ObjectCreatingEventArgs>(NewObjectViewController_ObjectCreating);
					newObjectViewController.ObjectCreated -= new EventHandler<ObjectCreatedEventArgs>(NewObjectViewController_ObjectCreated);
				}
				if(scheduler != null) {
					scheduler.PopupMenuShowing -= new PopupMenuShowingEventHandler(scheduler_PreparePopupMenu);
					scheduler.EditRecurrentAppointmentFormShowing -= scheduler_EditRecurrentAppointmentFormShowing;
					scheduler.KeyDown -= new KeyEventHandler(scheduler_KeyDown);
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.ControlsCreated += new EventHandler(View_ControlsCreated2);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.ControlsCreated -= new EventHandler(View_ControlsCreated2);
			if(schedulerEditor != null) {
				schedulerEditor = null;
				scheduler = null;
			}
		}
		protected virtual void AssignResourcesDataSource() {
			if((schedulerEditor != null) && View.IsControlCreated) {
				schedulerEditor.RecreateResourcesDataSource();
			}
		}
		protected override void ShowNotifications(INotificationsServiceOwner storage) {
			if(storage != null) {
				storage.NotificationsService.Refresh();
			}
			View.Refresh();
		}
		public SchedulerListViewController()
			: base() {
			CanManageEditRecurrentAppointmentFormShowing = CanManageEditRecurrentAppointmentFormShowingDefault;
			CanManagePreparePopupMenuAppointment = CanManagePreparePopupMenuAppointmentDefault;
		}
		protected override bool CanApplyCriteria {
			get {
				return scheduler != null && scheduler.IsHandleCreated;
			}
		}
		[DefaultValue(CanManageEditRecurrentAppointmentFormShowingDefault)]
		public bool CanManageEditRecurrentAppointmentFormShowing { get; set; }
		[DefaultValue(CanManagePreparePopupMenuAppointmentDefault)]
		public bool CanManagePreparePopupMenuAppointment { get; set; }
	}
}

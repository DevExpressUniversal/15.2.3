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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.Scheduler.Web {
	public class SchedulerListViewController : SchedulerListViewControllerBase {
		private ASPxSchedulerListEditor schedulerEditor;
		private bool recurrencesConfirmated;
		private void SchedulerDeleteHelper_RecurrencesConfirmated(object sender, EventArgs e) {
			IList<IEvent> objectsToDelete = schedulerEditor.SchedulerDeleteHelper.GetObjectsToDelete();
			if(objectsToDelete.Count > 0) {
				schedulerEditor.SelectEvent(objectsToDelete[0]);
				SimpleAction deleteAction = deleteObjectsViewController != null ? deleteObjectsViewController.DeleteAction : null;
				if(deleteAction != null && deleteAction.Active && deleteAction.Enabled) {
					recurrencesConfirmated = true;
					try {
						deleteAction.DoExecute();
					}
					finally {
						recurrencesConfirmated = false;
					}
					schedulerEditor.RaiseSelectionChanged();
				}
			}
		}
		protected override void SubscribeToSchedulerListEditorEvents() {
			base.SubscribeToSchedulerListEditorEvents();
			schedulerEditor = ((ListView)View).Editor as ASPxSchedulerListEditor;
			if(schedulerEditor != null) {
				schedulerEditor.NewAction += new EventHandler<NewActionEventArgs>(schedulerListEditor_NewAction);
				schedulerEditor.ObjectChanged += new EventHandler(schedulerListEditor_ObjectChanged);
				schedulerEditor.DataSourceChanged += new EventHandler(schedulerListEditor_DataSourceChanged);
				schedulerEditor.SchedulerDeleteHelper.RecurrencesConfirmated += new EventHandler(SchedulerDeleteHelper_RecurrencesConfirmated);
				NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
				if(newObjectViewController != null) {
					newObjectViewController.ObjectCreating += new EventHandler<ObjectCreatingEventArgs>(NewObjectViewController_ObjectCreating);
					newObjectViewController.ObjectCreated += new EventHandler<ObjectCreatedEventArgs>(NewObjectViewController_ObjectCreated);
				}
			}
		}
		protected override void UnsubscribeFromSchedulerListEditorEvents() {
			if(schedulerEditor != null) {
				schedulerEditor.NewAction -= new EventHandler<NewActionEventArgs>(schedulerListEditor_NewAction);
				schedulerEditor.ObjectChanged -= new EventHandler(schedulerListEditor_ObjectChanged);
				schedulerEditor.DataSourceChanged -= new EventHandler(schedulerListEditor_DataSourceChanged);
				if(schedulerEditor.SchedulerDeleteHelper != null) {
					schedulerEditor.SchedulerDeleteHelper.RecurrencesConfirmated -= new EventHandler(SchedulerDeleteHelper_RecurrencesConfirmated);
				}
				NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
				if(newObjectViewController != null) {
					newObjectViewController.ObjectCreating -= new EventHandler<ObjectCreatingEventArgs>(NewObjectViewController_ObjectCreating);
					newObjectViewController.ObjectCreated -= new EventHandler<ObjectCreatedEventArgs>(NewObjectViewController_ObjectCreated);
				}
			}
			base.UnsubscribeFromSchedulerListEditorEvents();
		}
		protected override void OnDeleting(IList objectsToDelete) {
			base.OnDeleting(objectsToDelete);
			if(schedulerEditor != null && ShowConfirmations) {
				if(!recurrencesConfirmated) {
					schedulerEditor.SchedulerDeleteHelper.StartProcessing(objectsToDelete, ObjectSpace);
					objectsToDelete.Clear();
				}
				else {
					Refill(objectsToDelete, schedulerEditor.SchedulerDeleteHelper.GetObjectsToDelete());
				}
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(schedulerEditor != null) {
				schedulerEditor = null;
			}
		}
	}
}

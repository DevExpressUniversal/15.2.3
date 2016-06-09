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

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
namespace DevExpress.ExpressApp.Notifications {
	public class NotificationsDialogViewController : ObjectViewController<DetailView, NotificationsObject> {
		private SimpleAction dismissAll;
		private SimpleAction dismiss;
		private SimpleAction snooze;
		private SimpleAction refresh;
		protected NotificationsService service;
		protected ListView listView;
		private void DismissAll_Execute(object sender, SimpleActionExecuteEventArgs e) {
			DismissCore(ViewCurrentObject.Notifications.ToList());
			DisableActions("NotificationsChanged");
		}
		private void Dismiss_Execute(object sender, SimpleActionExecuteEventArgs e) {
			List<Notification> items = listView.SelectedObjects.OfType<Notification>().ToList();
			DismissCore(items);
		}
		private void DismissCore(List<Notification> items) {
			try {
				service.Dismiss(items.Select(x => x.Source));
				service.Refresh();
			}
			catch(UserFriendlyException exception) {
				DisableActions("NotificationsChanged");
				throw exception;
			}
		}
		private void Snooze_Execute(object sender, SimpleActionExecuteEventArgs e) {
			List<Notification> items = listView.SelectedObjects.OfType<Notification>().ToList();
			try {
				if (ViewCurrentObject.Postpone.RemindIn.HasValue) {
					service.Postpone(items.Select(x => x.Source), (ViewCurrentObject.Postpone.RemindIn.Value));
				}
				service.Refresh();
			}
			catch(UserFriendlyException exception) {
				DisableActions("NotificationsChanged");
				throw exception;
			}
		}
		private void refresh_Execute(object sender, SimpleActionExecuteEventArgs e) {
			service.Refresh();
		}
		private void service_NotificationTriggered(object sender, NotificationItemsEventArgs e) {
			UpdateNotifications(e.NotificationItems);
		}
		private void editor_ControlCreated(object sender, EventArgs e) {
			listView = ((ListPropertyEditor)View.FindItem("Notifications")).ListView;
			listView.SelectionChanged += delegate(object s, EventArgs args) {
				if(IsOperationGrantedBySecurity(listView.SelectedObjects.OfType<Notification>().ToList())) {
					EnableActions("Security");
				}
				else {
					DisableActions("Security");
				}
			};
			dismiss.SelectionContext = listView;
			snooze.SelectionContext = listView;
		}
		private void obj_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "NotificationsState") {
				service.Refresh();
			}
		}
		protected void UpdateNotifications(IList<INotificationItem> notificationItems) {
			if(View != null) {
				if(View.CurrentObject != null) {
					ViewCurrentObject.SetNotifications(notificationItems.Select(x => new Notification(x)).ToList<Notification>());
				}
				listView.Refresh();
				if(listView.Editor.List.Count > 0) {
					EnableActions("NotificationsChanged");
				}
				else {
					DisableActions("NotificationsChanged");
				}
			}
			UpdateViewCaption();
		}
		protected virtual void UpdateViewCaption() {
			this.Frame.GetController<WindowTemplateController>().UpdateWindowCaption();
		}
		protected bool IsOperationGrantedBySecurity(List<Notification> items) {
			IObjectSpace objectSpace = listView.CollectionSource.ObjectSpace;
			foreach(Notification item in items) {
				object sourceObject = item.NotificationSource;
				Type objectType = objectSpace.GetObjectType(sourceObject);
				if(!DataManipulationRight.CanEdit(objectType, "AlarmTime", sourceObject, listView.CollectionSource, objectSpace)) {
					return false;
				}
			}
			return true;
		}
		protected virtual void DisableActions(string value) {
			dismissAll.Enabled.SetItemValue(value, false);
			dismiss.Enabled.SetItemValue(value, false);
			snooze.Enabled.SetItemValue(value, false);
		}
		protected virtual void EnableActions(string value) {
			dismissAll.Enabled.SetItemValue(value, true);
			dismiss.Enabled.SetItemValue(value, true);
			snooze.Enabled.SetItemValue(value, true);
		}
		protected override void OnActivated() {
			base.OnActivated();
			service = GetService();
			service.NotificationTriggered += service_NotificationTriggered;
			DisableDialogController();
			ListPropertyEditor editor = (ListPropertyEditor)View.FindItem("Notifications");
			if(editor != null) {
				editor.ControlCreated += editor_ControlCreated;
			}
			INotifyPropertyChanged obj = ViewCurrentObject as INotifyPropertyChanged;
			if (obj != null) {
				obj.PropertyChanged += obj_PropertyChanged;
			}
		}
		protected override void OnDeactivated() {
			INotifyPropertyChanged obj = ViewCurrentObject as INotifyPropertyChanged;
			if (obj != null) {
				obj.PropertyChanged -= obj_PropertyChanged;
			}
			ListPropertyEditor editor = (ListPropertyEditor)View.FindItem("Notifications");
			if(editor != null) {
				editor.ControlCreated -= editor_ControlCreated;
			}
			service.NotificationTriggered -= service_NotificationTriggered;
			base.OnDeactivated();
		}
		protected virtual NotificationsService GetService() {
			return Application.Modules.FindModule<NotificationsModule>().NotificationsService;
		}
		protected virtual void DisableDialogController() {
			Controller dialogController = Frame.GetController<DialogController>();
			if(dialogController != null) {
				dialogController.Active["OwnActions"] = false;
			}
		}
		public NotificationsDialogViewController() {
			TargetViewType = ViewType.DetailView;
			dismissAll = new SimpleAction(this, "DismissAll", "DismissAll");
			dismissAll.SelectionDependencyType = SelectionDependencyType.Independent;
			dismissAll.Execute += DismissAll_Execute;
			Actions.Add(dismissAll);
			dismiss = new SimpleAction(this, "Dismiss", "Dismiss");
			dismiss.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			dismiss.Execute += Dismiss_Execute;
			Actions.Add(dismiss);
			refresh = new SimpleAction(this, "RefreshNotifications", "RefreshNotifications");
			refresh.Execute += refresh_Execute;
			Actions.Add(refresh);
			snooze = new SimpleAction(this, "Snooze", "Snooze");
			snooze.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			Actions.Add(snooze);
			snooze.Execute += Snooze_Execute;
		}
		public SimpleAction DismissAll {
			get {
				return dismissAll;
			}
		}
		public SimpleAction Dismiss {
			get {
				return dismiss;
			}
		}
		public SimpleAction Snooze {
			get {
				return snooze;
			}
		}
		public SimpleAction Refresh {
			get {
				return refresh;
			}
		}
	}
}

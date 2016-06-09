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
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.General;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.ExpressApp.Notifications {
	public abstract class NotificationsController : WindowController {
		protected NotificationsService service;
		protected NotificationsModule notificationsModule;
		protected bool isViewShowing = false;
		protected bool isShowByActionClick = false;
		public SimpleAction ShowNotificationsAction;
		private View CreateNotificationsView(IList<INotificationItem> currentNotificationItems) {
			IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(NotificationsObject));
			NotificationsObject obj = new NotificationsObject(currentNotificationItems.Select(x => new Notification(x)), notificationsModule.ShowNotificationsWindow);
			obj.PropertyChanged += delegate(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
				if(e.PropertyName == "ShowNotificationsWindow") {
					notificationsModule.ShowNotificationsWindow = ((NotificationsObject)sender).ShowNotificationsWindow;
				}
			};
			DetailView view = Application.CreateDetailView(objectSpace, obj);
			view.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
			return view;
		}
		private void ShowNotificationsAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			RefreshNotificationServiceByActionClick();
		}
		protected virtual int GetNotificationItemsCount() {
			return service.GetActiveNotificationsCount();
		}
		private void service_NotificationsChanged(object sender, EventArgs e) {
			if(service != null) {
				ShowNotificationsAction.Caption = GetNotificationItemsCount().ToString();
				ShowNotificationsAction.Enabled["HasNotificationItems"] = (GetNotificationItemsCount() > 0) ? true : false;
			}
		}
		protected virtual void RefreshNotificationServiceByActionClick() {
			if(!isViewShowing) {
				isShowByActionClick = true;
				service.Refresh();
			}
		}
		protected ShowViewParameters PrepareNotificationViewParameters(IList<INotificationItem> currentNotificationItems) {
			ShowViewParameters showViewParameters = new ShowViewParameters();
			showViewParameters.Context = TemplateContext.PopupWindow;
			showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
			showViewParameters.CreatedView = CreateNotificationsView(currentNotificationItems);
			CustomizeShowViewParametersEventArgs args = new CustomizeShowViewParametersEventArgs(showViewParameters);
			if(CustomizeNotificationViewParameters != null) {
				CustomizeNotificationViewParameters(this, args);
			}
			return args.ShowViewParameters;
		}
		protected virtual void ProcessCurrentNotificationItemsCore(IList<INotificationItem> currentNotificationItems) {
			if(CustomProcessNotifications != null) {
				NotificationItemsEventArgs args = new NotificationItemsEventArgs(currentNotificationItems);
				CustomProcessNotifications(this, args);
				if(args.Handled) {
					return;
				}
			}
			ShowViewParameters viewParameters = PrepareNotificationViewParameters(currentNotificationItems);
			viewParameters.CreatedView.Closed += delegate(object sender, EventArgs e) {
				RefreshNotifications();
			};
			ShowNotificationViewCore(viewParameters);
		}
		protected void ProcessCurrentNotificationItems(IList<INotificationItem> currentNotificationItems) {
			if(!isViewShowing) {
				isViewShowing = true;
				if(currentNotificationItems == null || currentNotificationItems.Count == 0) {
					RefreshNotifications();
				}
				else {
					ProcessCurrentNotificationItemsCore(currentNotificationItems);
				}
			}
		}
		protected virtual void ShowNotificationViewCore(ShowViewParameters showViewParameters) {
			Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));
		}
		protected override void OnActivated() {
			base.OnActivated();
			notificationsModule = Application.Modules.FindModule<NotificationsModule>();
			if(notificationsModule != null) {
				service = notificationsModule.NotificationsService;
			}
			if(service != null) {
				service.NotificationTriggered += service_NotificationTriggered;
				service.NotificationsChanged += service_NotificationsChanged;
				int notificationsCount = GetNotificationItemsCount();
				ShowNotificationsAction.Enabled["HasNotificationItems"] = (notificationsCount > 0) ? true : false;
				ShowNotificationsAction.Caption = notificationsCount.ToString();
			}
		}
		protected override void OnDeactivated() {
			if(service != null) {
				service.NotificationTriggered -= service_NotificationTriggered;
				service.NotificationsChanged -= service_NotificationsChanged;
				service = null;
			}
			base.OnDeactivated();
		}
		protected virtual void service_NotificationTriggered(object sender, NotificationItemsEventArgs e) {
			if(!e.Handled) {
				if(notificationsModule.ShowNotificationsWindow || isShowByActionClick) {
					isShowByActionClick = false;
					ProcessCurrentNotificationItems(e.NotificationItems);
				}
				else {
					RefreshNotifications();
				}
				e.Handled = true;
			}
		}
		protected virtual void RefreshShowNotificationsAction() {
			string enabledCaption = CaptionHelper.GetLocalizedText("Notifications", "ShowNotificationsActionEnabledTooltip");
			string disabledCaption = CaptionHelper.GetLocalizedText("Notifications", "ShowNotificationsActionDisabledTooltip");
			ShowNotificationsAction.ToolTip = (ShowNotificationsAction.Enabled.ResultValue) ? enabledCaption : disabledCaption;
		}
		public NotificationsController() {
			this.TargetWindowType = WindowType.Main;
			ShowNotificationsAction = new SimpleAction(this, "ShowNotificationsAction", "Notifications");
			ShowNotificationsAction.ImageName = "Action_Bell";
			ShowNotificationsAction.Enabled.Changed += delegate(object sender, EventArgs e) {
				RefreshShowNotificationsAction();
			};
			ShowNotificationsAction.Execute += ShowNotificationsAction_Execute;
			ShowNotificationsAction.SelectionDependencyType = SelectionDependencyType.Independent;
		}
		public virtual void RefreshNotifications() {
			isViewShowing = false;
			service.RefreshNotifications();
		}
		public NotificationsService Service {
			get { return service; }
		}
		public event EventHandler<CustomizeShowViewParametersEventArgs> CustomizeNotificationViewParameters;
		public event EventHandler<NotificationItemsEventArgs> CustomProcessNotifications;
	}
}

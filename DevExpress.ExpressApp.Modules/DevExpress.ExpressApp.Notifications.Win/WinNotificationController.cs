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

using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System;
namespace DevExpress.ExpressApp.Notifications.Win {
	public class WinNotificationsController : NotificationsController {
		private static System.Windows.Forms.Timer timer;
		private static int notificationControllersCount = 0;
		private System.Windows.Forms.Timer startTimer;
		private void RaiseRefreshEvent() {
			timer.Stop();
			try {
				if(service.GetActiveNotificationsCount() > 0) {
					service.Refresh();
				}
			}
			finally {
				isViewShowing = false;
				timer.Start();
			}
		}
		private void CreateStartTimer() {
			startTimer = new System.Windows.Forms.Timer();
			startTimer.Interval = (int)notificationsModule.NotificationsStartDelay.TotalMilliseconds;
			startTimer.Tick += (s, e) => {
				startTimer.Dispose();
				if(timer != null) {
					RaiseRefreshEvent();
				}
			};
			startTimer.Start();
		}
		private void Window_TemplateChanged(object sender, EventArgs e) {
			OnWindowTemplateChanged();
		}
		private void Window_TemplateChanging(object sender, EventArgs e) {
			if(GetToolTipController() != null) {
				GetToolTipController().BeforeShow -= WinNotificationController_BeforeShow;
			}
		}
		private void OnWindowTemplateChanged() {
			if(GetToolTipController() != null) {
				GetToolTipController().BeforeShow += WinNotificationController_BeforeShow;
			}
		}
		private ToolTipController GetToolTipController() {
			IBarManagerHolder barManagerHolder = Window.Template as IBarManagerHolder;
			if(barManagerHolder != null && barManagerHolder.BarManager != null) {
				return barManagerHolder.BarManager.GetToolTipController();
			}
			return null;
		}
		private void WinNotificationController_BeforeShow(object sender, DevExpress.Utils.ToolTipControllerShowEventArgs e) {
			if(e.ToolTip == ShowNotificationsAction.ToolTip) {
				e.ToolTipType = DevExpress.Utils.ToolTipType.Standard;
			};
		}
		protected override void RefreshNotificationServiceByActionClick() {
			if(timer.Enabled) {
				timer.Stop();
				base.RefreshNotificationServiceByActionClick();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(timer == null && notificationsModule!= null) {
				int refreshinterval = (int)notificationsModule.NotificationsRefreshInterval.TotalMilliseconds;
				if(refreshinterval > 0) {
					timer = new System.Windows.Forms.Timer();
					timer.Interval = refreshinterval;
					timer.Tick += delegate(object sender, EventArgs e) {
						RaiseRefreshEvent();
					};
				}
				CreateStartTimer();
			}
			notificationControllersCount++;
			Window.TemplateChanged += new EventHandler(Window_TemplateChanged);
			Window.TemplateChanging += new EventHandler(Window_TemplateChanging);
			OnWindowTemplateChanged();
		}
		protected override void OnDeactivated() {
			Window.TemplateChanged -= new EventHandler(Window_TemplateChanged);
			Window.TemplateChanging -= new EventHandler(Window_TemplateChanging);
			if(startTimer != null) {
				startTimer.Dispose();
				startTimer = null;
			}
			notificationControllersCount--;
			if(notificationControllersCount == 0 && timer != null) {
				timer.Dispose();
				timer = null;
			}
			base.OnDeactivated();
		}
		protected override void ShowNotificationViewCore(ShowViewParameters showViewParameters) {
			timer.Stop();
			base.ShowNotificationViewCore(showViewParameters);
		}
		public override void RefreshNotifications() {
			base.RefreshNotifications();
			timer.Start();
		}
	}
}

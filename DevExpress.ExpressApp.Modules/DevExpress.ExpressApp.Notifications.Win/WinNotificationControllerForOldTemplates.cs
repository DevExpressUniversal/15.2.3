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

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Notifications;
using DevExpress.ExpressApp.Notifications.Win;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.Notifications.Win {
	public class WinNotificationsControllerForOldTemplates : WinNotificationsController {
		private BarButtonItem item;
		private void WinNotificationController_CustomizeStatusBar(object sender, EventArgs e) {
			if((item == null) && (Window != null)) {
				BarItemLinkCollection statusBarItemLinks = null;
				IBarManagerHolder barManagerHolder = Window.Template as IBarManagerHolder;
				if(barManagerHolder != null && barManagerHolder.BarManager != null && barManagerHolder.BarManager.StatusBar != null) {
					statusBarItemLinks = barManagerHolder.BarManager.StatusBar.ItemLinks;
				}
				else if(Window.Template is RibbonForm && ((RibbonForm)Window.Template).StatusBar != null) {
					statusBarItemLinks = ((RibbonForm)Window.Template).StatusBar.ItemLinks;
				}
				if(statusBarItemLinks != null) {
					item = new BarButtonItem();
					item.Name = "ShowNotificationsBarItem";
					item.Visibility = (this.Active.ResultValue == true) ? BarItemVisibility.OnlyInRuntime : BarItemVisibility.Never;
					item.Glyph = ImageLoader.Instance.GetImageInfo(ShowNotificationsAction.ImageName).Image;
					item.MergeType = BarMenuMerge.Replace;
					item.Alignment = BarItemLinkAlignment.Right;
					item.Enabled = ShowNotificationsAction.Enabled;
					item.Caption = ShowNotificationsAction.ToolTip;
					item.Hint = ShowNotificationsAction.Caption;
					item.ItemClick += delegate(object s, ItemClickEventArgs args) {
						RefreshNotificationServiceByActionClick();
					};
					statusBarItemLinks.Add(item);
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(item != null) {
				item.Visibility = BarItemVisibility.OnlyInRuntime;
			}
		}
		protected override void OnDeactivated() {
			if(Frame.GetController<WindowTemplateController>() != null) {
				Frame.GetController<WindowTemplateController>().CustomizeStatusBar -= WinNotificationController_CustomizeStatusBar;
			}
			if(item != null) {
				item.Visibility = BarItemVisibility.Never;
			}
			base.OnDeactivated();
		}
		protected override void RefreshShowNotificationsAction() {
			base.RefreshShowNotificationsAction();
			if(item != null) {
				item.Enabled = ShowNotificationsAction.Enabled;
				item.Caption = ShowNotificationsAction.ToolTip;
				item.Hint = ShowNotificationsAction.Caption;
			}
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if(Frame.GetController<WindowTemplateController>() != null) {
				Frame.GetController<WindowTemplateController>().CustomizeStatusBar += WinNotificationController_CustomizeStatusBar;
			}
		}
		public WinNotificationsControllerForOldTemplates() {
			ShowNotificationsAction.Active["Detouched"] = false;
		}
	}
}

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

using DevExpress.Persistent.Base.General;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Notifications {
	public class UpdateNotificationsSourceListViewController : WindowController {
		protected List<object> itemsToUpdateInObjectSpace = new List<object>();
		protected NotificationsService service;
		private void service_ItemsProcessed(object sender, NotificationItemsEventArgs e) {
			foreach(INotificationItem item in e.NotificationItems) {
				itemsToUpdateInObjectSpace.Add(item.NotificationSource);
			}
		}
		private void service_NotificationsChanged(object sender, HandledEventArgs e) {
			if(!e.Handled) {
				UpdateSourceObjectSpace();
				e.Handled = true;
			}
		}
		protected virtual void UpdateSourceObjectSpace() {
			if(itemsToUpdateInObjectSpace.Count > 0 && this.Frame != null && this.Frame.View != null && (this.Frame.View is ListView) && this.Frame.View.ObjectTypeInfo != null) {
				UpdateSourceObjectSpace(this.Frame);
				itemsToUpdateInObjectSpace.Clear();
			}
		}
		protected virtual void UpdateSourceObjectSpace(Frame frame) {
			foreach(object obj in itemsToUpdateInObjectSpace) {
				if(frame.View.ObjectTypeInfo.Type.IsAssignableFrom(obj.GetType())) {
					frame.View.ObjectSpace.ReloadObject(obj);
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Application.Modules.FindModule<NotificationsModule>() != null) {
				service = Application.Modules.FindModule<NotificationsModule>().NotificationsService;
				if(service != null) {
					service.ItemsProcessed += service_ItemsProcessed;
					service.NotificationsChanged += service_NotificationsChanged;
				}
			}
		}
		protected override void OnDeactivated() {
			if(service != null) {
				service.ItemsProcessed -= service_ItemsProcessed;
				service.NotificationsChanged -= service_NotificationsChanged;
			}
			base.OnDeactivated();
		}
		public UpdateNotificationsSourceListViewController() {
			this.TargetWindowType = WindowType.Main;
		}
	}
}

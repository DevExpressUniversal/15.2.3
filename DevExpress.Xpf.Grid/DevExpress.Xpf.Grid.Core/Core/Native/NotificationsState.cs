#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Windows;
using System.Collections.Generic;
namespace DevExpress.Xpf.Core.Native {
	public interface ILayoutNotificationHelperOwner {
		DependencyObject NotificationManager { get; }
		void InvalidateMeasure();
	}
	public class LayoutNotificationHelper : INotificationListener {
		bool subscribed;
		protected ILayoutNotificationHelperOwner owner;
		public LayoutNotificationHelper(ILayoutNotificationHelperOwner owner) {
			this.owner = owner;
		}
		DependencyObject manager;
		public void Subscribe() {
			if(subscribed && manager == owner.NotificationManager)
				return;
			manager = owner.NotificationManager;
			subscribed = NotificationEventManager.AddListener(manager, this);
		}
		public void Unsubscribe() {
			NotificationEventManager.RemoveListener(manager as INotificationManager, this);
			subscribed = false;
		}
		IEnumerable<NotificationType> INotificationListener.SupportedNotifications {
			get { return SupportedNotificationsCore; }
		}
		protected virtual IEnumerable<NotificationType> SupportedNotificationsCore {
			get { return new NotificationType[] { NotificationType.Layout }; }
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(NotificationEventManager)) {
				NotificationEventArgs ne = (NotificationEventArgs)e;
				ReceiveNotification(ne.Sender, ne.Notification);
				return true;
			}
			return false;
		}
		protected virtual void ReceiveNotification(object sender, NotificationType notification) {
			if(notification == NotificationType.Layout) {
				owner.InvalidateMeasure();
				ItemsControlBase itemsControl = owner as ItemsControlBase;
				if(itemsControl != null && itemsControl.Panel != null)
					itemsControl.Panel.InvalidateMeasure();
			}
		}
	}
}

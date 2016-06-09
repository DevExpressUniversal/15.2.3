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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.Notifications {
	public class NotificationsService : INotificationsService, IDisposable {
		private DefaultNotificationsProvider defaultNotificationsProvider;
		private List<INotificationsProvider> notificationsProviders;
		private void OnItemsProcessed(IList<INotificationItem> notificationItems) {
			if(ItemsProcessed != null) {
				ItemsProcessed(this, new NotificationItemsEventArgs(notificationItems));
			}
		}
		public NotificationsService(DefaultNotificationsProvider defaultNotificationsProvider) {
			this.defaultNotificationsProvider = defaultNotificationsProvider;
			notificationsProviders = new List<INotificationsProvider>();
			notificationsProviders.Add(defaultNotificationsProvider);
		}
		public void Dispose() {
			if(notificationsProviders != null) {
				notificationsProviders.Clear();
				notificationsProviders = null;
			}
			NotificationTriggered = null;
		}
		public void Refresh() {
			IList<INotificationItem> currentNotificationItems = new List<INotificationItem>();
			foreach(INotificationsProvider provider in notificationsProviders) {
				foreach(INotificationItem item in provider.GetNotificationItems()) {
					currentNotificationItems.Add(item);
				}
			}
			if(NotificationTriggered != null) {
				NotificationItemsEventArgs e = new NotificationItemsEventArgs(currentNotificationItems);
				NotificationTriggered(this, e);
			}
		}
		public int GetActiveNotificationsCount() {
			int result = 0;
			foreach(INotificationsProvider provider in NotificationsProviders) {
				result += provider.GetActiveNotificationsCount();
			}
			return result;
		}
		public int GetPostponedNotificationsCount() {
			int result = 0;
			foreach(INotificationsProvider provider in NotificationsProviders) {
				result += provider.GetPostponedNotificationsCount();
			}
			return result;
		}
		public virtual void Postpone(IEnumerable<INotificationItem> itemsToPostpone, TimeSpan postponeTime) {
			foreach(INotificationsProvider provider in notificationsProviders) {
				provider.Postpone(itemsToPostpone, postponeTime);
			}
			OnItemsProcessed(itemsToPostpone.ToList());
		}
		public virtual void Dismiss(IEnumerable<INotificationItem> itemsToDismiss) {
			foreach(INotificationsProvider provider in notificationsProviders) {
				provider.Dismiss(itemsToDismiss);
			}
			OnItemsProcessed(itemsToDismiss.ToList());
		}
		public virtual void SetItemChanged(INotificationItem item) {
			if(item != null && item.NotificationSource != null) {
				List<INotificationItem> resultList = new List<INotificationItem>() { item };
				OnItemsProcessed(resultList);
			}
		}
		public void RegisterNotificationsProvider(INotificationsProvider notificationProvider) {
			if(notificationProvider != null && notificationProvider.NotificationTypesInfo != null) {
				if(!notificationsProviders.Contains(notificationProvider)) {
					notificationsProviders.Add(notificationProvider);
					if(defaultNotificationsProvider != null) {
						foreach(ITypeInfo type in notificationProvider.NotificationTypesInfo) {
							if(defaultNotificationsProvider.NotificationTypesInfo.Contains(type)) {
								defaultNotificationsProvider.NotificationTypesInfo.Remove(type);
							}
						}
					}
				}
			}
		}
		public void UnregisterNotificationsProvider(INotificationsProvider notificationProvider) {
			if(notificationProvider != null && notificationsProviders.Contains(notificationProvider)) {
				notificationsProviders.Remove(notificationProvider);
			}
		}
		public void UpdateDefaultNotificationsProvider(DefaultNotificationsProvider newDefaultNotificationsProvider) {
			notificationsProviders.Remove(this.defaultNotificationsProvider);
			if(defaultNotificationsProvider != null) {
				defaultNotificationsProvider.Dispose();
			}
			this.defaultNotificationsProvider = newDefaultNotificationsProvider;
			List<INotificationsProvider> temporaryNotificationProviders = new List<INotificationsProvider>(notificationsProviders);
			notificationsProviders.Clear();
			if(defaultNotificationsProvider != null) {
				notificationsProviders.Add(defaultNotificationsProvider);
			}
			foreach(INotificationsProvider notificationProvider in temporaryNotificationProviders) {
				RegisterNotificationsProvider(notificationProvider);
			}
		}
		public void RefreshNotifications() {
			if(NotificationsChanged != null) {
				NotificationsChanged(this, new HandledEventArgs());
			}
		}
		public ReadOnlyCollection<INotificationsProvider> NotificationsProviders {
			get { return notificationsProviders.AsReadOnly(); }
		}
		public event EventHandler<NotificationItemsEventArgs> ItemsProcessed;
		public event EventHandler<NotificationItemsEventArgs> NotificationTriggered;
		public event EventHandler<HandledEventArgs> NotificationsChanged;
		#region Obsolete 15.2
		[Obsolete("Use the NotificationsService(DefaultNotificationsProvider defaultNotificationsProvider) constructor instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public NotificationsService(XafApplication application, DefaultNotificationsProvider defaultNotificationsProvider) {
			this.defaultNotificationsProvider = defaultNotificationsProvider;
			notificationsProviders = new List<INotificationsProvider>();
			notificationsProviders.Add(defaultNotificationsProvider);
		}
		#endregion
	}
}

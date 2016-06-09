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

using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.Persistent.Base.General {
	public interface ISupportNotifications {
		DateTime? AlarmTime { get; set; }
		object UniqueId { get; }
		string NotificationMessage { get; }
		bool IsPostponed { get; set; }
	}
	public interface INotificationsService {
		event EventHandler<NotificationItemsEventArgs> NotificationTriggered;
		void RegisterNotificationsProvider(INotificationsProvider notificationProvider);
		void UnregisterNotificationsProvider(INotificationsProvider notificationProvider);
		void Refresh();
	}
	public interface INotificationsProvider {
		HashSet<ITypeInfo> NotificationTypesInfo { get; }
		IList<INotificationItem> GetNotificationItems();
		int GetActiveNotificationsCount();
		int GetPostponedNotificationsCount();
		void Dismiss(IEnumerable<INotificationItem> notificationItems);
		void Postpone(IEnumerable<INotificationItem> notificationItems, TimeSpan postponeTime);
	}
	public interface INotificationsServiceOwner {
		INotificationsService NotificationsService{ get; }
	}
	public interface INotificationItem {
		ISupportNotifications NotificationSource { get; }
	}
	public class NotificationItemsEventArgs : HandledEventArgs {
		public NotificationItemsEventArgs(IList<INotificationItem> notificationItems) {
			this.NotificationItems = notificationItems;
		}
		public IList<INotificationItem> NotificationItems {
			get;
			private set;
		}
	}
}

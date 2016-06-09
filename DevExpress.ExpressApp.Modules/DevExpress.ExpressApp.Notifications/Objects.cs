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
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.SystemModule.Notifications;
namespace DevExpress.ExpressApp.Notifications {
	public enum NotificationsType { Active, Postponed, All }
	[DomainComponent, DefaultProperty("Subject")]
	public class Notification {
		private static long maxId = 0;
		private INotificationItem source;
		private long id;
		public Notification(INotificationItem source) {
			id = maxId++;
			this.source = source;
		}
		public long ID {
			get {
				return id;
			}
		}
		[Browsable(false)]
		public INotificationItem Source {
			get { return source; }
		}
		[Browsable(false)]
		public ISupportNotifications NotificationSource {
			get { return source.NotificationSource; }
		}
		public string Subject {
			get {
				return source.NotificationSource.NotificationMessage;
			}
		}
		[ModelDefaultAttribute("DisplayFormat", "{0:G}")]
		public DateTime AlarmTime {
			get {
				return (source.NotificationSource.AlarmTime != null) ? source.NotificationSource.AlarmTime.Value : DateTime.MinValue;
			}
		}
		public NotificationItemState State {
			get {
				return (source.NotificationSource.IsPostponed) ? NotificationItemState.Postponed : NotificationItemState.Active;
			}
		}
	}
	[DomainComponent]
	[ImageName("Notifications")]
	public class NotificationsObject : INotifyPropertyChanged {
		private IList<Notification> notifications;
		private IList<PostponeTime> postponeTimesList;
		private PostponeTime selectedPostponeTime;
		private bool showNotificationsWindow;
		private IList<PostponeTime> CreatePostponeTimes() {
			CustomizeNotificationsPostponeTimeListEventArgs args = new CustomizeNotificationsPostponeTimeListEventArgs(PostponeTime.CreateDefaultPostponeTimesList());
			if(CustomizePostponeLookup != null) {
				CustomizePostponeLookup(this, args);
			}
			PostponeTime.SortPostponeTimesList(args.PostponeTimesList);
			return args.PostponeTimesList.Where(x => x.RemindIn != null).ToList();
		}
		public NotificationsObject(IEnumerable<Notification> notificationItems, bool showNotificationsWindow) {
			this.showNotificationsWindow = showNotificationsWindow;
			SetNotifications(notificationItems.ToList<Notification>());
		}
		public void SetNotifications(IList<Notification> newNotifications) {
			notifications = newNotifications.ToList<Notification>();
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs("Notifications"));
			}
		}
		private IList<Notification> GetNotifications() {
			switch (notificationsState) {
				case NotificationsType.Active:
					return notifications.Where(x => x.AlarmTime <= DateTime.Now).ToList<Notification>();
				case NotificationsType.Postponed:
					return notifications.Where(x => x.Source.NotificationSource.IsPostponed && x.AlarmTime > DateTime.Now).ToList<Notification>();
				default:
					return notifications;
			}
		}
		private NotificationsType notificationsState;
		[ImmediatePostData]
		public NotificationsType NotificationsState {
			get { return notificationsState; }
			set {
				if (notificationsState != value) {
					notificationsState = value;
					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("NotificationsState"));
					}
				}
			}
		}
		public IList<Notification> Notifications {
			get { return GetNotifications(); }
		}
		[Browsable(false)]
		public int NotificationsCount {
			get { return notifications.Count; }
		}
		[ModelDefault("AllowClear", "False")]
		[DataSourceProperty("PostponeTimesList")]
		public PostponeTime Postpone {
			get {
				if(selectedPostponeTime == null) {
					selectedPostponeTime = PostponeTimesList.First();
				}
				return selectedPostponeTime;
			}
			set { selectedPostponeTime = value; }
		}
		[Browsable(false)]
		public IEnumerable<PostponeTime> PostponeTimesList {
			get {
				if(postponeTimesList == null) {
					postponeTimesList = CreatePostponeTimes();
				}
				return postponeTimesList;
			}
		}
		[ImmediatePostData]
		public bool ShowNotificationsWindow {
			get { return showNotificationsWindow; }
			set {
				showNotificationsWindow = value;
				if(PropertyChanged != null) {
					PropertyChanged(this, new PropertyChangedEventArgs("ShowNotificationsWindow"));
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<CustomizeNotificationsPostponeTimeListEventArgs> CustomizePostponeLookup;
	}
	public enum NotificationItemState {
		Active, Postponed
	}
}

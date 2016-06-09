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
namespace DevExpress.Xpf.Core.Native {
	public class NotificationEventManager : WeakEventManager {
		class DataType { }
		class LayoutType { }
		public static bool AddListener(DependencyObject element, INotificationListener listener) {
			INotificationManager manager = (INotificationManager)LayoutHelper.FindParentObject<INotificationManager>(element);
			if(manager == null)
				return false;
			foreach(NotificationType notification in listener.SupportedNotifications) {
				GetManager(notification).ProtectedAddListener(manager, listener);
			}
			return true;
		}
		public static void RemoveListener(INotificationManager manager, INotificationListener listener) {
			foreach(NotificationType notification in listener.SupportedNotifications) {
				GetManager(notification).ProtectedRemoveListener(manager, listener);
			}
		}
		static NotificationEventManager GetManager(NotificationType notification) {
			Type managerType;
			switch(notification) {
				case NotificationType.Data:
					managerType = typeof(DataType);
					break;
				case NotificationType.Layout:
					managerType = typeof(LayoutType);
					break;
				default:
					throw new NotImplementedException();
			}
			NotificationEventManager currentManager = (NotificationEventManager)WeakEventManager.GetCurrentManager(managerType);
			if(currentManager == null) {
				currentManager = new NotificationEventManager(notification);
				WeakEventManager.SetCurrentManager(managerType, currentManager);
			}
			return currentManager;
		}
		NotificationType notification;
		NotificationEventManager(NotificationType notification) {
			this.notification = notification;
		}
		void OnRequireMeasure(object sender, EventArgs args) {
			DeliverEvent(sender, args);
		}
		protected override void StartListening(object source) {
			INotificationManager manager = (INotificationManager)source;
			manager.SubscribeRequireMeasure(notification, OnRequireMeasure);
		}
		protected override void StopListening(object source) {
			INotificationManager manager = (INotificationManager)source;
			manager.UnsubscribeRequireMeasure(notification, OnRequireMeasure);
		}
	}
}

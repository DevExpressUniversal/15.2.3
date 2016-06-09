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
using System.Collections.Generic;
using System.IO;
using System.Text;
using DevExpress.Charts.NotificationCenter.Native;
namespace DevExpress.Charts.NotificationCenter {
	[System.Diagnostics.DebuggerStepThrough]
	public class NotificationCenterLogger {
		static NotificationCenterLogger defaultLogger;
		public static NotificationCenterLogger LoggerToCNotificationLogTxt {
			get {
				if (defaultLogger == null)
					defaultLogger = new NotificationCenterLogger(@"C:\NotificationLog.txt");
				return defaultLogger;
			}
		}
		string file;
		StringBuilder log;
		StringBuilder processingLog;
		bool processing;
		bool recievedOnProcessing;
		public NotificationCenterLogger(string file) {
			this.log = new StringBuilder();
			this.processingLog = new StringBuilder();
			this.file = file;
		}
		void DumpLog() {
#if !DXPORTABLE
			StreamWriter writer = new StreamWriter(file);
			writer.WriteLine(log.ToString());
			writer.Dispose();
#endif
		}
		string FormatNotification(Notification notification) {
			return String.Format("{0}[sender({1}); id({2})]", 
				notification.GetType().Name, 
				(notification.Sender == null) ? "null" : notification.Sender.ToString(),
				(notification.Identifier == null) ? "null" : notification.Identifier.ToString());
		}
		string FormatObservers(List<INotificationObserver> observers) {
			StringBuilder builder = new StringBuilder();
			builder.Append("{");
			if (observers.Count == 0)
				builder.Append("nobody");
			else {
				bool firstObserver = true;
				for (int i = 0; i < observers.Count; i++) {
					if (!firstObserver)
						builder.Append(",");
					builder.Append(observers[i].GetType().Name);
					firstObserver = false;
				}
			}
			builder.Append("}");
			return builder.ToString();
		}
		public void LogDenied(Notification notification) {
			log.AppendLine("--- " + FormatNotification(notification));
			DumpLog();
		}
		public void LogProcessing(Notification notification, List<INotificationObserver> observers) {
			processingLog.AppendLine("--> " + FormatNotification(notification) + " deliver to " + FormatObservers(observers));
			DumpLog();
			processing = true;
			recievedOnProcessing = false;
		}
		public void CompleteProcessing() {
			if (recievedOnProcessing)
				log.AppendLine("");
			log.Append(processingLog.ToString());
			processingLog.Clear();
			if (recievedOnProcessing)
				log.AppendLine("");
			recievedOnProcessing = false;
			processing = false;
			DumpLog();
		}
		public void LogEnqueued(Notification notification) {
			if (!processing) {
				log.AppendLine("<-- " + FormatNotification(notification));
			} else {
				processingLog.AppendLine("<-- " + FormatNotification(notification));
				recievedOnProcessing = true;
			}
			DumpLog();
		}
	}
	public class NotificationCenter {
		public static NotificationCenter DefaultCenter { get { return new NotificationCenter(); } } 
		readonly ObserverManager observerManager;
		readonly NotificationQueue notifications;
		readonly List<INotificationObserver> observersBuffer;
		readonly Dictionary<Type, int> deniedNotifications;
		Notification processingNotification;
		public NotificationCenter() {
			this.observerManager = new ObserverManager();
			this.observersBuffer = new List<INotificationObserver>();
			this.notifications = new NotificationQueue();
			this.deniedNotifications = new Dictionary<Type, int>();
		}
		void RegisterObserver(INotificationObserver observer, NotificationFilterCredentials credentials) {
			observerManager.Register(credentials, observer);
		}
		void ProcessNotifications() {
			if (processingNotification == null) {
				processingNotification = notifications.Dequeue();
				while (processingNotification != null) {
					NotificationFilterCredentials credentials = new NotificationFilterCredentials(processingNotification);
					observerManager.SelectObserver(observersBuffer, credentials);
#if DEBUG
					if (logger != null)
						LogProcessing(processingNotification, observersBuffer);
#endif
					for (int i = 0; i < observersBuffer.Count; i++)
						observersBuffer[i].Notify(processingNotification);
#if DEBUG
					if (logger != null)
						LogCompleteProcessing();
#endif
					processingNotification = notifications.Dequeue();
				}
			}
		}
		public void RegisterObserver(INotificationObserver observer) {
			RegisterObserver(observer, new NotificationFilterCredentials());
		}
		public void RegisterObserver(INotificationObserver observer, Type notificationType) {
			RegisterObserver(observer, new NotificationFilterCredentials(notificationType));
		}
		public void RegisterObserver(INotificationObserver observer, Type notificationType, object sender) {
			RegisterObserver(observer, new NotificationFilterCredentials(notificationType, sender));
		}
		public void RegisterObserver(INotificationObserver observer, Type notificationType, object sender, object identifier) {
			RegisterObserver(observer, new NotificationFilterCredentials(notificationType, sender, identifier));
		}
		public void UnregisterObserver(INotificationObserver observer) {
			RegisterObserver(observer, new NotificationFilterCredentials());
		}
		public void UnregisterObserver(INotificationObserver observer, Type notificationType) {
			RegisterObserver(observer, new NotificationFilterCredentials(notificationType));
		}
		public void UnregisterObserver(INotificationObserver observer, Type notificationType, object sender) {
			RegisterObserver(observer, new NotificationFilterCredentials(notificationType, sender));
		}
		public void UnregisterObserver(INotificationObserver observer, Type notificationType, object sender, object identifier) {
			RegisterObserver(observer, new NotificationFilterCredentials(notificationType, sender, identifier));
		}
		bool IsDenied(Notification notification) {
			if (notification != null) {
				int counter;
				return deniedNotifications.TryGetValue(notification.GetType(), out counter);
			}
			return false;
		}
		public void Send(Notification notification) {
#if DEBUG
			if (logger != null)
				LogSend(notification);
#endif
			if (!IsDenied(notification)) {
				notifications.Enqueue(notification);
				ProcessNotifications();
			}
		}
		public void DenyNotifications(Type type) {
			int currentCount;
			if (deniedNotifications.TryGetValue(type, out currentCount))
				deniedNotifications[type] = currentCount + 1;
			else
				deniedNotifications.Add(type, 1);
		}
		public void AllowNotifications(Type type) {
			int currentCount;
			if (deniedNotifications.TryGetValue(type, out currentCount)) {
				currentCount--;
				if (currentCount <= 0)
					deniedNotifications.Remove(type);
				else
					deniedNotifications[type] = currentCount;
			}
		}
#region DEBUG
#if DEBUG
		NotificationCenterLogger logger;
		public NotificationCenterLogger Logger {
			get {
				return logger;
			}
			set {
				logger = value;
			}
		}
		void LogSend(Notification notification) {
			if (IsDenied(notification))
				logger.LogDenied(notification);
			else
				logger.LogEnqueued(notification);
		}
		void LogProcessing(Notification notification, List<INotificationObserver> observers) {
			logger.LogProcessing(notification, observers);
		}
		void LogCompleteProcessing() {
			logger.CompleteProcessing();
		}
#endif
#endregion
	}
}

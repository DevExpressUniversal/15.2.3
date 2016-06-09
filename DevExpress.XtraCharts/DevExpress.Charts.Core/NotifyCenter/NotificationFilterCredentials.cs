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
namespace DevExpress.Charts.NotificationCenter.Native {
	public enum NotificationFilterLevel : int {
		None = 0,
		Type = 1,
		Sender = 2,
		Identifier = 3
	}
	public sealed class NotificationFilterCredentials {
		public static bool operator ==(NotificationFilterCredentials a, NotificationFilterCredentials b) {
			return object.Equals(a, b);
		}
		public static bool operator !=(NotificationFilterCredentials a, NotificationFilterCredentials b) {
			return !object.Equals(a, b);
		}
		readonly object sender;
		readonly object identifier;
		readonly Type notificationType;
		NotificationFilterLevel level;
		public object Sender { get { return sender; } }
		public object Identifier { get { return identifier; } }
		public object NotficationType { get { return notificationType; } }
		public NotificationFilterLevel Level { get { return level; } }
		public int LevelValue { get { return (int)level; } }
		public NotificationFilterCredentials() {
		}
		public NotificationFilterCredentials(Type notificationType) {
			this.notificationType = notificationType;
			SetLevel();
		}
		public NotificationFilterCredentials(Type notificationType, object sender) {
			this.notificationType = notificationType;
			this.sender = sender;
			if (this.sender == null)
				throw new Exception("[sender] cannot be null. Use appropriate constructor to omit filtering criteria.");
			SetLevel();
		}
		public NotificationFilterCredentials(Type notificationType, object sender, object identifier) {
			this.notificationType = notificationType;
			this.sender = sender;
			this.identifier = identifier;
			if (this.sender == null)
				throw new Exception("[sender] cannot be null. Use appropriate constructor to omit filtering criteria.");
			if (this.identifier == null)
				throw new Exception("[identifier] cannot be null. Use appropriate constructor to omit filtering criteria.");
			SetLevel();
		}
		public NotificationFilterCredentials(Notification notification) {
			this.notificationType = notification.GetType();
			this.sender = notification.Sender;
			this.identifier = notification.Identifier;
			SetLevel();
		}
		void SetLevel() {
			level = NotificationFilterLevel.None;
			if (notificationType != null) {
				if (sender != null)
					if (identifier != null)
						level = NotificationFilterLevel.Identifier;
					else
						level = NotificationFilterLevel.Sender;
				else
					level = NotificationFilterLevel.Type;
			}
		}
		public NotificationFilterCredentials ShiftToLevel(NotificationFilterLevel level) {
			switch (level) {
				case NotificationFilterLevel.Type:
					return new NotificationFilterCredentials(notificationType);
				case NotificationFilterLevel.Sender:
					return new NotificationFilterCredentials(notificationType, sender);
				case NotificationFilterLevel.Identifier:
					return new NotificationFilterCredentials(notificationType, sender, identifier);
				default:
				case NotificationFilterLevel.None:
					return new NotificationFilterCredentials();
			}
		}
		public override bool Equals(object obj) {
			NotificationFilterCredentials credentials = obj as NotificationFilterCredentials;
			if (credentials == null)
				return false;
			return (this.sender == credentials.sender) &&
				   (this.identifier == credentials.identifier) &&
				   (this.notificationType == credentials.notificationType);
		}
		public override int GetHashCode() {
			return ((sender == null) ? 0 : sender.GetHashCode()) ^
				   ((identifier == null) ? 0 : identifier.GetHashCode()) ^
				   ((notificationType == null) ? 0 : notificationType.GetHashCode());
		}
	}
}

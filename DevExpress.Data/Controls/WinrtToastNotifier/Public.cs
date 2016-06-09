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
using System.ComponentModel;
namespace DevExpress.Internal {
	public class ToastNotificationFailedException : Exception {
		public ToastNotificationFailedException(Exception inner, int errorCode)
			: base(null, inner) {
			ErrorCode = errorCode;
		}
		public override string Message {
			get { return InnerException.Message; }
		}
		public int ErrorCode { get; private set; }
		[System.Security.SecuritySafeCritical]
		internal static ToastNotificationFailedException ToException(int hResult) {
			try {
				System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(hResult);
				return null;
			}
			catch(Exception e) {
				return new ToastNotificationFailedException(e, hResult);
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum NotificationSetting {
		Enabled = 0,
		DisabledForApplication = 1,
		DisabledForUser = 2,
		DisabledByGroupPolicy = 3,
		DisabledByManifest = 4
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum ToastDismissalReason : long {
		UserCanceled = 0,
		ApplicationHidden = 1,
		TimedOut = 2
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum PredefinedSound {
		Notification_Default,
		NoSound,
		Notification_IM,
		Notification_Mail,
		Notification_Reminder,
		Notification_SMS,
		Notification_Looping_Alarm,
		Notification_Looping_Alarm2,
		Notification_Looping_Alarm3,
		Notification_Looping_Alarm4,
		Notification_Looping_Alarm5,
		Notification_Looping_Alarm6,
		Notification_Looping_Alarm7,
		Notification_Looping_Alarm8,
		Notification_Looping_Alarm9,
		Notification_Looping_Alarm10,
		Notification_Looping_Call,
		Notification_Looping_Call2,
		Notification_Looping_Call3,
		Notification_Looping_Call4,
		Notification_Looping_Call5,
		Notification_Looping_Call6,
		Notification_Looping_Call7,
		Notification_Looping_Call8,
		Notification_Looping_Call9,
		Notification_Looping_Call10,
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum NotificationDuration {
		Default,
		Long
	}
}

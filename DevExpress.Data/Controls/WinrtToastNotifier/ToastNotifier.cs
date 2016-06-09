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
using System.Threading.Tasks;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
namespace DevExpress.Internal {
	public interface IPredefinedToastNotification {
		Task<ToastNotificationResultInternal> ShowAsync();
		void Hide();
	}
	public interface IPredefinedToastNotificationInfo {
		ToastTemplateType ToastTemplateType { get; }
		string[] Lines { get; }
		string ImagePath { get; }
		NotificationDuration Duration { get; }
		PredefinedSound Sound { get; }
	}
	public interface IPredefinedToastNotificationContent : IDisposable {
		IPredefinedToastNotificationInfo Info { get; }
		void SetDuration(NotificationDuration duration);
		void SetImage(byte[] image);
		void SetImage(string imagePath);
		void SetImage(System.Drawing.Image image);
		void SetSound(PredefinedSound sound);
	}
	public interface IPredefinedToastNotificationContentFactory {
		IPredefinedToastNotificationContent CreateContent(string bodyText);
		IPredefinedToastNotificationContent CreateOneLineHeaderContent(string headlineText, string bodyText);
		IPredefinedToastNotificationContent CreateOneLineHeaderContent(string headlineText, string bodyText1, string bodyText2);
		IPredefinedToastNotificationContent CreateTwoLineHeaderContent(string headlineText, string bodyText);
	}
	public interface IPredefinedToastNotificationFactory {
		IPredefinedToastNotificationContentFactory CreateContentFactory();
		IPredefinedToastNotification CreateToastNotification(IPredefinedToastNotificationContent content);
		IPredefinedToastNotification CreateToastNotification(string bodyText);
		IPredefinedToastNotification CreateToastNotificationOneLineHeaderContent(string headlineText, string bodyText);
		IPredefinedToastNotification CreateToastNotificationOneLineHeaderContent(string headlineText, string bodyText1, string bodyText2);
		IPredefinedToastNotification CreateToastNotificationTwoLineHeader(string headlineText, string bodyText);
	}
	public enum ToastNotificationResultInternal {
		Activated = 0,
		UserCanceled = 1,
		TimedOut = 2,
		ApplicationHidden = 3,
		Dropped = 4,
	}
}

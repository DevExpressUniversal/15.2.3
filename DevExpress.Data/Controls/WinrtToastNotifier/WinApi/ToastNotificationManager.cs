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
using System.Security;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
namespace DevExpress.Internal.WinApi {
	[CLSCompliant(false)]
	public interface IToastNotificationAdapter {
		IToastNotification Create(IPredefinedToastNotificationInfo info);
		void Show(IToastNotification notification);
		void Hide(IToastNotification notification);
	}
	[SecuritySafeCritical]
	public static class ToastNotificationManager {
		static Version Win8Version = new Version(6, 2, 9200, 0);
		static bool IsWin8OrHigher {
			get {
				OperatingSystem OS = Environment.OSVersion;
				return (OS.Platform == PlatformID.Win32NT) && (OS.Version >= Win8Version);
			}
		}
		public static bool AreToastNotificationsSupported {
			get { return IsWin8OrHigher; }
		}
		static IToastNotificationManager defaultManager;
		internal static IToastNotificationManager GetDefaultManager() {
			if(defaultManager == null)
				defaultManager = ComFunctions.RoGetActivationFactory<IToastNotificationManager>();
			return defaultManager;
		}
		internal static Window.Data.Xml.Dom.IXmlDocument GetDocument(IPredefinedToastNotificationInfo info) {
			return GetDocument(GetDefaultManager(), info);
		}
		internal static Window.Data.Xml.Dom.IXmlDocument GetDocument(IToastNotificationManager manager, IPredefinedToastNotificationInfo info) {
			return WinRTToastNotificationContent.GetDocument(manager, info);
		}
		internal static string GetXml(IPredefinedToastNotificationInfo info) {
			string xml;
			((Window.Data.Xml.Dom.IXmlNodeSerializer)GetDocument(info)).GetXml(out xml);
			return xml;
		}
		internal static IToastNotificationAdapter CreateToastNotificationAdapter(string appID) {
			return AreToastNotificationsSupported ?
						(IToastNotificationAdapter)new ToastNotificationAdapter(appID, GetDefaultManager()) :
						(IToastNotificationAdapter)new EmptyToastNotificationAdapter();
		}
		#region ToastNotifierAdapter
		class ToastNotificationAdapter : IToastNotificationAdapter {
			string appId;
			IToastNotifier notifier;
			IToastNotificationFactory factory;
			IToastNotificationManager manager;
			public ToastNotificationAdapter(string appId, IToastNotificationManager manager) {
				this.appId = appId;
				this.manager = manager;
			}
			IToastNotification IToastNotificationAdapter.Create(IPredefinedToastNotificationInfo info) {
				var content = ToastNotificationManager.GetDocument(manager, info);
				if(factory == null)
					factory = ComFunctions.RoGetActivationFactory<IToastNotificationFactory>();
				IToastNotification result;
				ComFunctions.CheckHRESULT(factory.CreateToastNotification(content, out result));
				return result;
			}
			void IToastNotificationAdapter.Show(IToastNotification notification) {
				if(notifier == null)
					ComFunctions.CheckHRESULT(manager.CreateToastNotifierWithId(appId, out notifier));
				if(notifier != null && notification != null)
					notifier.Show(notification);
			}
			void IToastNotificationAdapter.Hide(IToastNotification notification) {
				if(notifier == null)
					ComFunctions.CheckHRESULT(manager.CreateToastNotifierWithId(appId, out notifier));
				if(notifier != null && notification != null)
					notifier.Hide(notification);
			}
		}
		class EmptyToastNotificationAdapter : IToastNotificationAdapter {
			IToastNotification IToastNotificationAdapter.Create(IPredefinedToastNotificationInfo info) { return null; }
			void IToastNotificationAdapter.Show(IToastNotification notification) { }
			void IToastNotificationAdapter.Hide(IToastNotification notification) { }
		}
		#endregion
	}
}

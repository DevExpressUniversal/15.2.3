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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.MVVM.Services;
namespace DevExpress.XtraBars.ToastNotifications {
	[DXToolboxItem(true), DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation)]
	[Designer("DevExpress.XtraBars.Design.ToastNotificationsManagerDesigner, " + AssemblyInfo.SRAssemblyBarsDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	[Description("Supports displaying Windows 8 Toast notifications.")]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "ToastNotificationsManager")]
	public class ToastNotificationsManager : Docking2010.Base.BaseComponent, INotificationProvider {
		#region About
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinXtraBars));
		}
		#endregion About
		public ToastNotificationsManager()
			: base(null) {
		}
		public ToastNotificationsManager(IContainer container)
			: base(container) {
		}
		public static bool AreToastNotificationsSupported {
			get { return DevExpress.Internal.WinApi.ToastNotificationManager.AreToastNotificationsSupported; }
		}
		protected override void OnCreate() {
			base.OnCreate();
			notificationsCore = new ToastNotificationCollection();
		}
		protected override void OnDispose() {
			Docking2010.Ref.Dispose(ref notificationsCore);
			base.OnDispose();
		}
		#region Properties
		string applicationIdCore;
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Key)]
		public string ApplicationId {
			get { return applicationIdCore; }
			set { SetValue(ref applicationIdCore, value); }
		}
		DevExpress.Utils.DefaultBoolean createApplicationShortcutCore = DevExpress.Utils.DefaultBoolean.Default;
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), Category("Shortcut")]
		public DevExpress.Utils.DefaultBoolean CreateApplicationShortcut {
			get { return createApplicationShortcutCore; }
			set { SetValue(ref createApplicationShortcutCore, value); }
		}
		string applicationNameCore;
		[DefaultValue(null), Category("Shortcut")]
		public string ApplicationName {
			get { return applicationNameCore; }
			set { SetValue(ref applicationNameCore, value); }
		}
		string applicationIconPathCore;
		[DefaultValue(null), Category("Shortcut")]
		public string ApplicationIconPath {
			get { return applicationIconPathCore; }
			set { SetValue(ref applicationIconPathCore, value); }
		}
		protected bool ShouldCreateApplicationShortcut() {
			return CreateApplicationShortcut == DevExpress.Utils.DefaultBoolean.True;
		}
		ToastNotificationCollection notificationsCore;
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.XtraBars.Design.ToastNotificationCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesign,
			typeof(System.Drawing.Design.UITypeEditor)), Category("Notifications"), Localizable(true)]
		public ToastNotificationCollection Notifications {
			get { return notificationsCore; }
		}
		#endregion Properties
		#region Events
		readonly static object activated = new object();
		readonly static object timedOut = new object();
		readonly static object dropped = new object();
		readonly static object userCancelled = new object();
		readonly static object hidden = new object();
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EventHandler<ToastNotificationEventArgs> Activated {
			add { Events.AddHandler(activated, value); }
			remove { Events.RemoveHandler(activated, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EventHandler<ToastNotificationEventArgs> TimedOut {
			add { Events.AddHandler(timedOut, value); }
			remove { Events.RemoveHandler(timedOut, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EventHandler<ToastNotificationEventArgs> Dropped {
			add { Events.AddHandler(dropped, value); }
			remove { Events.RemoveHandler(dropped, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EventHandler<ToastNotificationEventArgs> UserCancelled {
			add { Events.AddHandler(userCancelled, value); }
			remove { Events.RemoveHandler(userCancelled, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EventHandler<ToastNotificationEventArgs> Hidden {
			add { Events.AddHandler(hidden, value); }
			remove { Events.RemoveHandler(hidden, value); }
		}
		void RaiseActivated(object notificationId) {
			RaiseToastNotificationEvent(activated, notificationId);
		}
		void RaiseTimedOut(object notificationId) {
			RaiseToastNotificationEvent(timedOut, notificationId);
		}
		void RaiseDropped(object notificationId) {
			RaiseToastNotificationEvent(dropped, notificationId);
		}
		void RaiseUserCanceled(object notificationId) {
			RaiseToastNotificationEvent(userCancelled, notificationId);
		}
		void RaiseHidden(object notificationId) {
			RaiseToastNotificationEvent(hidden, notificationId);
		}
		void RaiseToastNotificationEvent(object eventId, object notificationId) {
			var handler = Events[eventId] as EventHandler<ToastNotificationEventArgs>;
			if(handler != null)
				handler(this, new ToastNotificationEventArgs(notificationId));
		}
		#endregion Events
		protected virtual void OnApplicationIdChanged() {
			this.notificationsFactoryCore = null;
		}
		string GetApplicationID() {
			if(string.IsNullOrEmpty(ApplicationId))
				return GetApplicationName();
			return ApplicationId;
		}
		static string GetApplicationName() {
			string[] applicationArgs = System.Environment.GetCommandLineArgs();
			return System.IO.Path.GetFileNameWithoutExtension(applicationArgs[0]);
		}
		protected override void OnInitialized() {
			TryCreateApplicationShortcut();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void TryCreateApplicationShortcut() {
			if(string.IsNullOrEmpty(ApplicationId))
				applicationIdCore = GetApplicationName();
			if(!string.IsNullOrEmpty(ApplicationName) && ShouldCreateApplicationShortcut()) {
				string iconPath = ApplicationIconPath;
				if(!string.IsNullOrEmpty(iconPath))
					iconPath = System.IO.Path.GetFullPath(iconPath);
				DevExpress.Data.ShellHelper.TryCreateShortcut(ApplicationId, ApplicationName, iconPath);
			}
		}
		DevExpress.Internal.IPredefinedToastNotificationFactory notificationsFactoryCore;
		protected DevExpress.Internal.IPredefinedToastNotificationFactory NotificationsFactory {
			get {
				if(notificationsFactoryCore == null)
					notificationsFactoryCore = new DevExpress.Internal.WinRTToastNotificationFactory(GetApplicationID());
				return notificationsFactoryCore;
			}
		}
		public void ShowNotification(object id) {
			IToastNotificationProperties notificationProperties;
			if(Notifications.TryGetValue(id, out notificationProperties))
				ShowNotification(notificationProperties);
		}
		public void ShowNotification(IToastNotificationProperties notificationProperties) {
			DevExpress.Internal.IPredefinedToastNotificationContent content = null;
			var notification = CreateToastNotificationInternal(notificationProperties, ref content);
			if(notification != null)
				ShowNotificationCore(notification, notificationProperties.ID, content);
		}
		public void HideNotification(object id) {
			IToastNotificationProperties notificationProperties;
			if(Notifications.TryGetValue(id, out notificationProperties))
				HideNotification(notificationProperties);
		}
		public void HideNotification(IToastNotificationProperties notificationProperties) {
			if(notificationsQueue == null) return;
			IList<KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>> notifications;
			if(notificationsQueue.TryGetValue(notificationProperties.ID, out notifications)) {
				var items = ((List<KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>>)notifications).ToArray();
				DequeueNotification(notificationProperties.ID);
				if(items.Length > 0)
					items[0].Key.Hide();
			}
		}
		public void HideNotifications(object id) {
			IToastNotificationProperties notificationProperties;
			if(Notifications.TryGetValue(id, out notificationProperties))
				HideNotifications(notificationProperties);
		}
		public void HideNotifications(IToastNotificationProperties notificationProperties) {
			if(notificationsQueue == null) return;
			IList<KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>> notifications;
			if(notificationsQueue.TryGetValue(notificationProperties.ID, out notifications)) {
				var items = ((List<KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>>)notifications).ToArray();
				DequeueNotifications(notificationProperties.ID);
				for(int i = 0; i < items.Length; i++)
					items[i].Key.Hide();
			}
		}
		DevExpress.Internal.IPredefinedToastNotificationContent CreateNotificationContent(
			IToastNotificationProperties properties,
			DevExpress.Internal.IPredefinedToastNotificationContentFactory contentFactory) {
			switch(properties.Template) {
				case ToastNotificationTemplate.Text02:
				case ToastNotificationTemplate.ImageAndText02:
					return contentFactory.CreateOneLineHeaderContent(properties.Header, properties.Body);
				case ToastNotificationTemplate.Text03:
				case ToastNotificationTemplate.ImageAndText03:
					return contentFactory.CreateTwoLineHeaderContent(properties.Header, properties.Body);
				case ToastNotificationTemplate.Text04:
				case ToastNotificationTemplate.ImageAndText04:
					return contentFactory.CreateOneLineHeaderContent(properties.Header, properties.Body, properties.Body2);
				default:
					return contentFactory.CreateContent(properties.Body);
			}
		}
		void ShowNotificationCore(DevExpress.Internal.IPredefinedToastNotification notification, object notificationId, DevExpress.Internal.IPredefinedToastNotificationContent content) {
			if(notification == null) return;
			QueueNotification(notification, notificationId, content);
			var context = System.Threading.SynchronizationContext.Current;
			notification.ShowAsync().ContinueWith(t =>
			{
				DequeueNotification(notificationId);
				switch(t.Result) {
					case DevExpress.Internal.ToastNotificationResultInternal.Activated:
						context.Post(RaiseActivated, notificationId);
						break;
					case DevExpress.Internal.ToastNotificationResultInternal.TimedOut:
						context.Post(RaiseTimedOut, notificationId);
						break;
					case DevExpress.Internal.ToastNotificationResultInternal.Dropped:
						context.Post(RaiseDropped, notificationId);
						break;
					case DevExpress.Internal.ToastNotificationResultInternal.UserCanceled:
						context.Post(RaiseUserCanceled, notificationId);
						break;
					case DevExpress.Internal.ToastNotificationResultInternal.ApplicationHidden:
						context.Post(RaiseHidden, notificationId);
						break;
				}
			});
		}
		object syncObj = new object();
		IDictionary<object, IList<KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>>> notificationsQueue;
		void QueueNotification(Internal.IPredefinedToastNotification notification, object id, Internal.IPredefinedToastNotificationContent content) {
			lock(syncObj) {
				if(notificationsQueue == null)
					notificationsQueue = new Dictionary<object, IList<KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>>>();
				IList<KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>> notifications;
				if(!notificationsQueue.TryGetValue(id, out notifications)) {
					notifications = new List<KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>>();
					notificationsQueue.Add(id, notifications);
				}
				notifications.Add(new KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>(notification, content));
			}
		}
		void DequeueNotification(object id) {
			lock(syncObj) {
				if(notificationsQueue == null) return;
				IList<KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>> notifications;
				if(notificationsQueue.TryGetValue(id, out notifications)) {
					var pair = notifications[0];
					notifications.RemoveAt(0);
					pair.Value.Dispose();
					if(notifications.Count == 0)
						notificationsQueue.Remove(id);
				}
			}
		}
		void DequeueNotifications(object id) {
			lock(syncObj) {
				if(notificationsQueue == null) return;
				IList<KeyValuePair<Internal.IPredefinedToastNotification, Internal.IPredefinedToastNotificationContent>> notifications;
				if(notificationsQueue.TryGetValue(id, out notifications)) {
					foreach(var pair in notifications)
						pair.Value.Dispose();
					notifications.Clear();
					notificationsQueue.Remove(id);
				}
			}
		}
		Internal.IPredefinedToastNotification CreateToastNotificationInternal(IToastNotificationProperties notificationProperties, ref DevExpress.Internal.IPredefinedToastNotificationContent content) {
			if(string.IsNullOrEmpty(notificationProperties.Body)) return null;
			var contentFactory = NotificationsFactory.CreateContentFactory();
			content = CreateNotificationContent(notificationProperties, contentFactory);
			content.SetDuration((DevExpress.Internal.NotificationDuration)(int)notificationProperties.Duration);
			content.SetSound((DevExpress.Internal.PredefinedSound)(int)notificationProperties.Sound);
			if(notificationProperties.HasImage)
				content.SetImage(notificationProperties.Image);
			return NotificationsFactory.CreateToastNotification(content);
		}
		Internal.IPredefinedToastNotification INotificationProvider.CreateNotification(string header, string body, string body2, System.Drawing.Bitmap bitmapImage) {
			object id = Guid.NewGuid();
			ToastNotificationTemplate template = NotificationTemplateHelper.GetNotificationTemplate(header, body, body2, bitmapImage);
			NotificationTemplateHelper.ContentCorrection(ref header, ref body);
			IToastNotificationProperties notificationProperties = new ToastNotification(id, bitmapImage, header, body, body2, template);
			DevExpress.Internal.IPredefinedToastNotificationContent content = null;
			return CreateToastNotificationInternal(notificationProperties, ref content);
		}
		static class NotificationTemplateHelper {
			static NotificationTemplateHelper() {
				templateLibrary = new Dictionary<TemplateContent, ToastNotificationTemplate>();
				InitializeTemplateLibrary();
			}
			static Dictionary<TemplateContent, ToastNotificationTemplate> templateLibrary;
			public static ToastNotificationTemplate GetNotificationTemplate(string header, string body, string body2, System.Drawing.Bitmap bitmapImage) {
				TemplateContent content = GetContentWeight(header, body, body2, bitmapImage);
				return GetNotificationTemplate(content);
			}
			public static void ContentCorrection(ref string header, ref string body) {
				if(string.IsNullOrEmpty(body))
					body = "Empty body";
				if(header == null)
					header = "";
			}
			enum TemplateContent { Empty = 0, Body = 1, Header = 2, Body2 = 4, Image = 8, LongHeader = 16 }
			static void InitializeTemplateLibrary() {
				templateLibrary.Add(TemplateContent.Empty, ToastNotificationTemplate.Text01);
				templateLibrary.Add(TemplateContent.Body, ToastNotificationTemplate.Text01);
				templateLibrary.Add(TemplateContent.Header, ToastNotificationTemplate.Text03);
				templateLibrary.Add(TemplateContent.Body | TemplateContent.Header, ToastNotificationTemplate.Text02);
				templateLibrary.Add(TemplateContent.Body | TemplateContent.LongHeader, ToastNotificationTemplate.Text03);
				templateLibrary.Add(TemplateContent.Body2, ToastNotificationTemplate.Text04);
				templateLibrary.Add(TemplateContent.Body | TemplateContent.Body2, ToastNotificationTemplate.Text04);
				templateLibrary.Add(TemplateContent.Header | TemplateContent.Body2, ToastNotificationTemplate.Text04);
				templateLibrary.Add(TemplateContent.Body | TemplateContent.Header | TemplateContent.Body2, ToastNotificationTemplate.Text04);
				templateLibrary.Add(TemplateContent.Empty | TemplateContent.Image, ToastNotificationTemplate.ImageAndText01);
				templateLibrary.Add(TemplateContent.Body | TemplateContent.Image, ToastNotificationTemplate.ImageAndText01);
				templateLibrary.Add(TemplateContent.Header | TemplateContent.Image, ToastNotificationTemplate.ImageAndText03);
				templateLibrary.Add(TemplateContent.Body | TemplateContent.Header | TemplateContent.Image, ToastNotificationTemplate.ImageAndText02);
				templateLibrary.Add(TemplateContent.Body | TemplateContent.LongHeader | TemplateContent.Image, ToastNotificationTemplate.ImageAndText03);
				templateLibrary.Add(TemplateContent.Body2 | TemplateContent.Image, ToastNotificationTemplate.ImageAndText04);
				templateLibrary.Add(TemplateContent.Body | TemplateContent.Body2 | TemplateContent.Image, ToastNotificationTemplate.ImageAndText04);
				templateLibrary.Add(TemplateContent.Header | TemplateContent.Body2 | TemplateContent.Image, ToastNotificationTemplate.ImageAndText04);
				templateLibrary.Add(TemplateContent.Body | TemplateContent.Header | TemplateContent.Body2 | TemplateContent.Image, ToastNotificationTemplate.ImageAndText04);
			}
			static ToastNotificationTemplate GetNotificationTemplate(TemplateContent content) {
				ToastNotificationTemplate template = ToastNotificationTemplate.Text01;
				templateLibrary.TryGetValue(content, out template);
				return template;
			}
			static TemplateContent GetContentWeight(string header, string body, string body2, System.Drawing.Bitmap bitmapImage) {
				TemplateContent content = TemplateContent.Empty;
				if(!string.IsNullOrEmpty(body)) content |= TemplateContent.Body;
				if(!string.IsNullOrEmpty(header)) content |= TemplateContent.Header;
				if(!string.IsNullOrEmpty(body2)) content |= TemplateContent.Body2;
				if(bitmapImage != null) content |= TemplateContent.Image;
				if(content == (TemplateContent.Body | TemplateContent.Header))
					if(header.Length > body.Length) content = TemplateContent.Body | TemplateContent.LongHeader;
				if(content == (TemplateContent.Body | TemplateContent.Header | TemplateContent.Image)) 
					if(header.Length > body.Length) content = TemplateContent.Body | TemplateContent.LongHeader |TemplateContent.Image;
				return content;
			}
		}
	}
}

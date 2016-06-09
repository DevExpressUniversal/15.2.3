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

namespace DevExpress.Utils.MVVM.Services {
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using DevExpress.Internal;
	public interface INotificationProvider {
		IPredefinedToastNotification CreateNotification(string header, string body, string body2, Bitmap image);
	}
	public interface INotificationInfo {
		string Header { get; }
		string Body { get; }
		string Body2 { get; }
		Image Image { get; }
	}
	public class Notification {
		readonly IPredefinedToastNotification notification;
		protected Notification(DevExpress.Internal.IPredefinedToastNotification notification) {
			this.notification = notification;
		}
		public void Hide() {
			try {
				notification.Hide();
			}
			catch { }
		}
		static Func<IPredefinedToastNotification, object> showAsyncRoutine;
		public object ShowAsync() {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			return DynamicCastHelper.GetAsyncFunction<IPredefinedToastNotification, ToastNotificationResultInternal>(
				ref showAsyncRoutine, "ShowAsync", typesResolver.GetNotificationResultType())(notification);
		}
		#region static
		protected internal static Notification Create(IPredefinedToastNotification notification) {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			return DynamicServiceSource.Create<Notification, IPredefinedToastNotification>(
					typesResolver.GetINotificationType(), notification);
		}
		#endregion static
	}
	public class NotificationService {
		#region static
		public static NotificationService Create(INotificationProvider manager) {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			return DynamicServiceSource.Create<NotificationService, INotificationProvider>(
				typesResolver.GetINotificationServiceType(), manager);
		}
		#endregion static
		INotificationProvider notificationProvider;
		protected NotificationService(INotificationProvider notificationProvider) {
			this.notificationProvider = notificationProvider;
		}
		public object CreateCustomNotification(object viewModel) {
			INotificationInfo viewModelInfo = viewModel as INotificationInfo;
			if(viewModelInfo == null) return null;
			return CreatePredefinedNotification(viewModelInfo.Header, viewModelInfo.Body, viewModelInfo.Body2, viewModelInfo.Image);
		}
		public object CreatePredefinedNotification(string header, string body, string body2, object image = null) {
			if(notificationProvider == null) return null;
			using(var bitmapImage = GetBitmapFromObject(image)) 
				return Notification.Create(notificationProvider.CreateNotification(header, body, body2, bitmapImage));
		}
		static Bitmap GetBitmapFromObject(object source) {
			if(source == null)
				return null;
			var bitmapSource = source as System.Windows.Media.Imaging.BitmapSource;
			if(bitmapSource != null) {
				return BitmapSourceToBitmap(bitmapSource);
			}
			var image = source as Image;
			if(image != null) {
				return new Bitmap(image);
			}
			byte[] byteArray = source as byte[];
			if(byteArray != null) {
				using(var ms = new MemoryStream(byteArray)) 
					return new Bitmap(ms);
			}
			return null;
		}
		static Bitmap BitmapSourceToBitmap(System.Windows.Media.Imaging.BitmapSource source) {
			Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight, PixelFormat.Format32bppPArgb);
			BitmapData data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
			try {
				source.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
				return bmp;
			}
			finally { bmp.UnlockBits(data); }
		}
	}
}

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

using DevExpress.Internal;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
namespace DevExpress.Mvvm.UI.Native {
	internal class WpfPredefinedToastNotificationContent : IPredefinedToastNotificationContent {
		const int msDefaultDuration = 6000;
		const int msLongDuration = 25000;
		public PredefinedToastNotificationVewModel ViewModel { get; private set; }
		public int Duration { get; set; }
		public WpfPredefinedToastNotificationContent(PredefinedToastNotificationVewModel viewModel) {
			ViewModel = viewModel;
			Duration = msDefaultDuration;
		}
		public void SetImage(byte[] image) {
			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.StreamSource = new MemoryStream(image);
			bitmap.EndInit();
			ViewModel.Image = bitmap;
		}
		public void SetImage(System.Drawing.Image image) {
			throw new NotImplementedException();
		}
		public void SetImage(string path) {
			throw new NotImplementedException();
		}
		public void SetSound(DevExpress.Internal.PredefinedSound sound) {
			if (sound != DevExpress.Internal.PredefinedSound.NoSound)
				Debug.WriteLine("Only Windows 8 toast notifications support sound.");
		}
		public void SetDuration(DevExpress.Internal.NotificationDuration duration) {
			Duration = duration == DevExpress.Internal.NotificationDuration.Long ? msLongDuration : msDefaultDuration;
		}
		public void Dispose() { }
		public DevExpress.Internal.IPredefinedToastNotificationInfo Info {
			get { return new WpfTToastNotificationInfo(); }
		}
		class WpfTToastNotificationInfo : DevExpress.Internal.IPredefinedToastNotificationInfo {
			public ToastTemplateType ToastTemplateType { get; set; }
			public string[] Lines { get; set; }
			public string ImagePath { get; set; }
			public DevExpress.Internal.NotificationDuration Duration { get; set; }
			public DevExpress.Internal.PredefinedSound Sound { get; set; }
		}
	}
}

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
using System.IO;
using System.Net;
using System.Globalization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Map;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Map;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Map.Native;
using DevExpress.Map.Kml.Model;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class KmlPointData : INotifyPropertyChanged {
		static Color GetColor(ColorABGR modelColor) {
			return Color.FromArgb(modelColor.A, modelColor.R, modelColor.G, modelColor.B);
		}
		public static KmlPointData Create(ImageTextInfoCore textImageInfo) {
			KmlPointData pointData = new KmlPointData();
			pointData.TitleText = textImageInfo.Text;
			Uri imageUri = textImageInfo.ImageUri;
			pointData.ImageHref = imageUri != null ? imageUri.ToString() : null;
			pointData.ImageScale = textImageInfo.ImageScale;
			pointData.TextScale = textImageInfo.TextScale;
			pointData.TextColor = GetColor(textImageInfo.TextColor);
			IImageTransform imageTransform = textImageInfo.ImageTransform;
			double originX, originY;
			imageTransform.CalcImageOrigin(pointData.ImageWidth, pointData.ImageHeight, out originX, out originY);
			pointData.ImageOffsetX = originX;
			pointData.ImageOffsetY = originY;
			return pointData;
		}
		BitmapImage bitmap;
		Transform transform;
		Brush textBrush;
		DXMapImages mapImages = new DXMapImages();
		public Transform Transform {
			get {
				if (transform == null)
					transform = CreateTransform();
				return transform;
			}
		}
		public BitmapImage Bitmap {
			get {
				if (bitmap == null) {
					bitmap = LoadBitmap(ImageHref);
					if (bitmap != null && !bitmap.IsDownloading)
						UpdateBitmap();
				}
				return bitmap;
			}
		}
		public double ImageWidth { get { return (Bitmap != null) ? Bitmap.PixelWidth * RealImageScale : 0.0; } }
		public double ImageHeight { get { return (Bitmap != null) ? Bitmap.PixelHeight * RealImageScale : 0.0; } }
		public string TitleText { get; set; }
		public double TextSize { get { return 12 * TextScale; } }
		public Brush Foreground {
			get {
				if (textBrush == null)
					textBrush = new SolidColorBrush(TextColor);
				return textBrush;
			}
		}
		internal double ImageScale { get; set; }
		internal double TextScale { get; set; }
		internal string ImageHref { get; set; }
		internal Color TextColor { get; set; }
		internal double ImageOffsetX { get; set; }
		internal double ImageOffsetY { get; set; }
		internal double RealImageScale { get { return (Bitmap != null) ? 32.0 / (double)Math.Min(Bitmap.PixelWidth, Bitmap.PixelHeight) * ImageScale : 1.0; } }
		#region INotifyPropertyChanged members
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if ((propertyChanged != null))
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		protected virtual BitmapImage LoadBitmap(string imageHref) {
			if (imageHref == null)
				return mapImages.KmlDefaultPoint;
			if (imageHref.Length == 0 || !Uri.IsWellFormedUriString(imageHref, UriKind.Absolute))
				return null;
			bitmap = new BitmapImage(new Uri(imageHref, UriKind.Absolute));
			if (bitmap.IsDownloading)
				SubscribeBitmapEvent(bitmap);
			else
				UpdateBitmap();
			return bitmap;
		}
		protected void UpdateBitmap() {
			CreateTransform();
			RaisePropertyChanged("ImageWidth");
			RaisePropertyChanged("ImageHeight");
			RaisePropertyChanged("Transform");
		}
		protected virtual void OnBitmapImageLoaded(BitmapImage bitmap) {
			UnsubscribeBitmapEvent(bitmap);
			UpdateBitmap();
		}
		protected virtual void SubscribeBitmapEvent(BitmapImage bitmap) {
			if (bitmap == null)
				return;
			bitmap.DownloadCompleted += BitmapLoaded;
			bitmap.DownloadFailed += BitmapDownloadFailed;
		}
		protected virtual void UnsubscribeBitmapEvent(BitmapImage bitmap) {
			if (bitmap == null)
				return;
			bitmap.DownloadCompleted -= BitmapLoaded;
			bitmap.DownloadFailed -= BitmapDownloadFailed;
		}
		double GetImageOffsetX() {
			return -ImageWidth * ImageOffsetX;
		}
		double GetImageOffsetY() {
			return -ImageHeight * ImageOffsetY;
		}
		protected virtual Transform CreateTransform() {
			if (bitmap == null || bitmap.IsDownloading)
				return null;
			return new TranslateTransform(GetImageOffsetX(), GetImageOffsetY());
		}
		void BitmapLoaded(object sender, EventArgs e) {
			OnBitmapImageLoaded((BitmapImage)sender);
		}
		void BitmapDownloadFailed(object sender, ExceptionEventArgs e) {
			UnsubscribeBitmapEvent(sender as BitmapImage);
		}
	}
}

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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Spreadsheet.Extensions.Internal {
	using DevExpress.XtraSpreadsheet.Model;
	public static class PointExtensions {
		public static System.Drawing.Point ToDrawingPoint(this Point point) {
			return new System.Drawing.Point((int)point.X, (int)point.Y);
		}
		public static Point ToPoint(this System.Drawing.Point point) {
			return new Point(point.X, point.Y);
		}
	}
	public static class RectangleExtensions {
		public static Rect GetSaveRect(double x, double y, double width, double height) {
			return new Rect(x, y, Math.Max(0, width), Math.Max(0, height));
		}
		public static Rect ToRect(this System.Drawing.Rectangle rectangle) {
			return GetSaveRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
		public static System.Drawing.Rectangle ToRectangle(this Rect rect) {
			return new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}
		public static Size ToSize(this System.Drawing.Rectangle rectangle) {
			return new Size(rectangle.Width, rectangle.Height);
		}
	}
	public static class ColorExtensions {
		public static Color ToWpfColor(this System.Drawing.Color color) {
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}
		public static bool IsEmpty(this Color color) {
			return (color.A + color.R + color.G + color.B) == 0;
		}
	}
	public static class ImageExtensions {
		static float defaultTargetDpi = 220.0f;
		public static ImageSource ToImageSource(this System.Drawing.Image image) {
			return ToImageSource(image, defaultTargetDpi);
		}
		public static ImageSource ToImageSource(this System.Drawing.Image image, float targetDpi) {
			Size targetSize = new Size(
				image.Width * targetDpi / image.HorizontalResolution,
				image.Height * targetDpi / image.VerticalResolution);
			return ToImageSource(image, targetSize);
		}
		public static ImageSource ToImageSource(this System.Drawing.Image image, Size targetSize) {
			MemoryStream ms = new MemoryStream();
			System.Drawing.Imaging.Metafile metafile = image as System.Drawing.Imaging.Metafile;
			if (metafile != null) {
				int width = (int)(targetSize.Width * DocumentModel.Dpi / 96.0);
				int height = (int)(targetSize.Height * DocumentModel.Dpi / 96.0);
				using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
					using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap)) {
						graphics.DrawImage(metafile, 0, 0, width, height);
						bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					}
				}
			}
			else
				image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			ms.Position = 0;
			BitmapImage bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = ms;
			bi.EndInit();
			return bi;
		}
	}
	public static class SizeExtensions {
		public static System.Drawing.Size ToDrawingSize(this Size size) {
			return new System.Drawing.Size((int)size.Width, (int)size.Height);
		}
		public static Size ToSize(this System.Drawing.Size size){
			return new Size(size.Width, size.Height);
		}
	}
}

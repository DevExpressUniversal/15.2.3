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
using System.Linq;
using System.Printing;
using System.Security;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Utils;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.XtraPrinting;
using GdiColor = System.Drawing.Color;
using GdiFont = System.Drawing.Font;
using GdiFontStyle = System.Drawing.FontStyle;
using GdiImage = System.Drawing.Image;
using GdiStringTrimming = System.Drawing.StringTrimming;
namespace DevExpress.Xpf.Printing.Native {
	static class DrawingConverter {
		public static Rect ToRect(System.Drawing.RectangleF value) {
			return new Rect(value.X, value.Y, value.Width, value.Height);
		}
		public static Size ToSize(System.Drawing.SizeF value) {
			return new Size(value.Width, value.Height);
		}
		public static Point ToPoint(System.Drawing.PointF value) {
			return new Point(value.X, value.Y);
		}
		public static GdiColor ToGdiColor(Color color) {
#if SL
			return color;
#else
			return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
#endif
		}
		public static GdiFont CreateGdiFont(FontFamily family, System.Windows.FontStyle style, FontWeight weight, BrickTextDecorations textDecorations, double size) {
			if(family == null)
				throw new ArgumentNullException("family");
			string familyName;
#if SL
			familyName = FontHelper.GetFamilyName(family);
#else
			familyName = family.FamilyNames.Values.First();
#endif
			GdiFont font = new GdiFont(familyName, GetFontSizeInPoints(size), ToGdiFontStyle(style, weight, textDecorations));
			if(GetFontFamilyName(font.FontFamily) != family.Source) {
				GdiFont sourceFont = new GdiFont(family.Source, GetFontSizeInPoints(size), ToGdiFontStyle(style, weight, textDecorations));
				if(GetFontFamilyName(sourceFont.FontFamily) == family.Source) {
					return sourceFont;
				}
			}
			return font;
		}
#if SL
		static string GetFontFamilyName(FontFamily fontFamily) {
			return FontHelper.GetFamilyName(fontFamily);
		}
#else
		static string GetFontFamilyName(System.Drawing.FontFamily fontFamily) {
			return fontFamily.Name;
		}
#endif
		public static float GetFontSizeInPoints(double sizeInDeviceIndependentPixels) {
			return DevExpress.XtraPrinting.GraphicsUnitConverter.Convert(
				(float)sizeInDeviceIndependentPixels,
				GraphicsDpi.DeviceIndependentPixel,
				DevExpress.XtraPrinting.GraphicsDpi.Point);
		}
		public static GdiFontStyle ToGdiFontStyle(System.Windows.FontStyle style, FontWeight weight, BrickTextDecorations textDecorations) {
			GdiFontStyle fontStyle = GdiFontStyle.Regular;
			if(style == FontStyles.Italic)
				fontStyle |= GdiFontStyle.Italic;
#if !SL
			if(style == FontStyles.Oblique)
				fontStyle |= GdiFontStyle.Italic;
#endif
			if(FontWeightHelper.Compare(weight, FontWeights.SemiBold) >= 0)
				fontStyle |= GdiFontStyle.Bold;
			if((textDecorations & BrickTextDecorations.Underline) != 0)
				fontStyle |= GdiFontStyle.Underline;
#if !SL
			if((textDecorations & BrickTextDecorations.Strikethrough) != 0)
				fontStyle |= GdiFontStyle.Strikeout;
#endif
			return fontStyle;
		}
		public static GdiImage CreateGdiImage(FrameworkElement element) {
#if SL
			element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));			
			Rect rect = new Rect(0, 0, Math.Round(element.ActualWidth), Math.Round(element.ActualHeight));
			if (rect.Width == 0 || rect.Height == 0)
				return null;
			WriteableBitmap writeableBitmap = new WriteableBitmap((int)rect.Width, (int)rect.Height);
			writeableBitmap.Render(element, null);
			writeableBitmap.Invalidate();
			return Bitmap.FromWriteableBitmap(writeableBitmap);
#else
			BitmapSource bitmapSource = CreateBitmapSource(element);
			return bitmapSource != null ? FromBitmapSource(bitmapSource) : null;
#endif
		}
#if SILVERLIGHT
		public static GdiImage FromBitmapSource(BitmapSource bitmapSource) {
			return Bitmap.FromWriteableBitmap(new WriteableBitmap(bitmapSource));
		}
#else
		public static GdiImage FromBitmapSource(BitmapSource bitmapSource) {
			Guard.ArgumentNotNull(bitmapSource, "bitmapSource");
			BitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
			MemoryStream stream = new MemoryStream();
			encoder.Save(stream);
			return (System.Drawing.Bitmap)System.Drawing.Image.FromStream(stream);
		}
#endif
#if !SILVERLIGHT
		public static BitmapSource CreateBitmapSource(FrameworkElement element) {
			Rect rect = new Rect(0, 0, Math.Round(element.ActualWidth), Math.Round(element.ActualHeight));
			if(rect.Width == 0 || rect.Height == 0)
				return null;
			DrawingVisual drawingVisual = new DrawingVisual();
			using(DrawingContext drawingContext = drawingVisual.RenderOpen()) {
				VisualBrush visualBrush = new VisualBrush(element);
				drawingContext.DrawRectangle(visualBrush, null, rect);
			}
			RenderTargetBitmap bitmapSource = new RenderTargetBitmap(
				(int)rect.Width,
				(int)rect.Height,
				GraphicsDpi.DeviceIndependentPixel,
				GraphicsDpi.DeviceIndependentPixel,
				PixelFormats.Pbgra32);
			bitmapSource.Render(drawingVisual);
			return bitmapSource;
		}
#endif
		public static DevExpress.XtraPrinting.TextAlignment ToXtraPrintingTextAlignment(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment) {
			if(horizontalAlignment == HorizontalAlignment.Left) {
				if(verticalAlignment == VerticalAlignment.Top)
					return DevExpress.XtraPrinting.TextAlignment.TopLeft;
				if(verticalAlignment == VerticalAlignment.Center)
					return DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
				if(verticalAlignment == VerticalAlignment.Bottom)
					return DevExpress.XtraPrinting.TextAlignment.BottomLeft;
			} else if(horizontalAlignment == HorizontalAlignment.Center) {
				if(verticalAlignment == VerticalAlignment.Top)
					return DevExpress.XtraPrinting.TextAlignment.TopCenter;
				if(verticalAlignment == VerticalAlignment.Center)
					return DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
				if(verticalAlignment == VerticalAlignment.Bottom)
					return DevExpress.XtraPrinting.TextAlignment.BottomCenter;
			} else if(horizontalAlignment == HorizontalAlignment.Right) {
				if(verticalAlignment == VerticalAlignment.Top)
					return DevExpress.XtraPrinting.TextAlignment.TopRight;
				if(verticalAlignment == VerticalAlignment.Center)
					return DevExpress.XtraPrinting.TextAlignment.MiddleRight;
				if(verticalAlignment == VerticalAlignment.Bottom)
					return DevExpress.XtraPrinting.TextAlignment.BottomRight;
			} else {	
				System.Diagnostics.Debug.Assert(horizontalAlignment == HorizontalAlignment.Stretch);
				if(verticalAlignment == VerticalAlignment.Top)
					return DevExpress.XtraPrinting.TextAlignment.TopJustify;
				if(verticalAlignment == VerticalAlignment.Center)
					return DevExpress.XtraPrinting.TextAlignment.MiddleJustify;
				if(verticalAlignment == VerticalAlignment.Bottom)
					return DevExpress.XtraPrinting.TextAlignment.BottomJustify;
			}
			throw new NotSupportedException();
		}
		public static Color FromGdiColor(GdiColor color) {
#if SL
			return color;
#else
			return Color.FromArgb(color.A, color.R, color.G, color.B);
#endif
		}
#if !SL
		[SecuritySafeCritical]
		public static PageMediaSizeName FromPaperKind(System.Drawing.Printing.PaperKind paperKind) {
			switch(paperKind) {
				case System.Drawing.Printing.PaperKind.Letter:
					return PageMediaSizeName.NorthAmericaLetter;
				case System.Drawing.Printing.PaperKind.Legal:
					return PageMediaSizeName.NorthAmericaLegal;
				case System.Drawing.Printing.PaperKind.A3:
					return PageMediaSizeName.ISOA3;
				case System.Drawing.Printing.PaperKind.A4:
					return PageMediaSizeName.ISOA4;
				case System.Drawing.Printing.PaperKind.A5:
					return PageMediaSizeName.ISOA5;
				case System.Drawing.Printing.PaperKind.B4:
					return PageMediaSizeName.ISOB4;
				case System.Drawing.Printing.PaperKind.A4Extra:
					return PageMediaSizeName.ISOA4Extra;
				#region TODO: other formats
				#endregion
				case System.Drawing.Printing.PaperKind.Custom:
				default:
					return PageMediaSizeName.Unknown;
			}
		}
#endif
		public static GdiStringTrimming ToStringTrimming(TextTrimming textTrimming) {
			switch(textTrimming) {
				case TextTrimming.None:
					return GdiStringTrimming.None;
#if !SILVERLIGHT
				case TextTrimming.CharacterEllipsis:
					return GdiStringTrimming.EllipsisCharacter;
#endif
				case TextTrimming.WordEllipsis:
					return GdiStringTrimming.EllipsisWord;
				default:
					throw new ArgumentException();
			}
		}
	}
}

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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfRenderingCommandInterpreter : PdfCommandInterpreter {
		protected const float DefaultPageDpi = 72;
		public const float DefaultDpi = 110;
		public const float DpiFactor = DefaultDpi / DefaultPageDpi;
		static readonly PointF[] imageDestinationPoints = new PointF[] { new PointF(0, 0), new PointF(1, 0), new PointF(0, 1) };
		protected static PointF[] ImageDestinationPoints { get { return imageDestinationPoints; } }
		public static Color ConvertToGDIPlusColor(PdfColor color, double alphaConstant) {
			PdfRGBColor rgbColor = color == null ? PdfRGBColor.Create(0, 0, 0) : PdfRGBColor.FromColor(color);
			return Color.FromArgb(Convert.ToByte(alphaConstant * 255), Convert.ToByte(rgbColor.R * 255), Convert.ToByte(rgbColor.G * 255), Convert.ToByte(rgbColor.B * 255));
		}
		public static Color ConvertToGDIPlusColor(PdfColor color) {
			return ConvertToGDIPlusColor(color, 1);
		}
		public static ImageAttributes GetImageOpacityAttributes(double alphaConstant) {
			float[][] matrixItems = { 
				new float[] { 1, 0, 0, 0, 0 },
				new float[] { 0, 1, 0, 0, 0 },
				new float[] { 0, 0, 1, 0, 0 },
				new float[] { 0, 0, 0, (float)alphaConstant, 0 }, 
				new float[] { 0, 0, 0, 0, 1 }
			};
			ImageAttributes imageOpacityAttributes = new ImageAttributes();
			imageOpacityAttributes.SetColorMatrix(new ColorMatrix(matrixItems), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			return imageOpacityAttributes;
		}
		readonly PdfImageDataStorage imageDataStorage;
		readonly float scale;
		bool hasFunctionalLimits = false;
		protected float Scale { get { return scale; } }
		public bool HasFunctionalLimits {
			get { return hasFunctionalLimits; }
			set { hasFunctionalLimits = value; }
		}
		protected PdfRenderingCommandInterpreter() {
			scale = 1.0f;
		}
		protected PdfRenderingCommandInterpreter(PdfPage page, int rotateAngle, PdfImageDataStorage imageDataStorage, float scale, PdfRectangle boundingBox) : base(page, rotateAngle, boundingBox) {
			this.imageDataStorage = imageDataStorage;
			this.scale = scale;
		}
		protected PdfRenderingCommandInterpreter(PdfPage page, int rotateAngle, PdfImageDataStorage imageDataStorage, float scale) : this(page, rotateAngle, imageDataStorage, scale, page.CropBox) {
		}
		protected PdfImageData GetImageData(PdfImage image) {
			return imageDataStorage.GetImageData(image);
		}
		protected InterpolationMode GetInterpolationMode(int imageWidth, int imageHeight, int bitsPerComponent) {
			if (scale < DpiFactor)
				return InterpolationMode.HighQualityBicubic;
			PdfTransformationMatrix matrix = GraphicsState.TransformationMatrix;
			PdfPoint location = matrix.Transform(new PdfPoint(0, 0));
			double width = PdfMathUtils.CalcDistance(matrix.Transform(new PdfPoint(1, 0)), location);
			double height = PdfMathUtils.CalcDistance(matrix.Transform(new PdfPoint(0, 1)), location);
			return (bitsPerComponent == 1 && (imageWidth < width * 1.35 || imageHeight < height * 1.35)) ? InterpolationMode.NearestNeighbor : InterpolationMode.HighQualityBicubic;
		}
		protected Matrix GetImageMatrix(PointF location) {
			PdfTransformationMatrix currentMatrix = GraphicsState.TransformationMatrix;
			return new Matrix((float)currentMatrix.A * scale, (float)-currentMatrix.B * scale, (float)-currentMatrix.C * scale, (float)currentMatrix.D * scale, location.X, location.Y);
		}
		protected void DrawImage(Graphics graphics, Bitmap bitmap, PointF[] points) {
			double nonStrokingAlphaConstant = GraphicsState.NonStrokingAlphaConstant;
			if (nonStrokingAlphaConstant == 1)
				graphics.DrawImage(bitmap, points);
			else
				graphics.DrawImage(bitmap, points, new RectangleF(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel, GetImageOpacityAttributes(nonStrokingAlphaConstant));
		}
		[SecuritySafeCritical]
		protected void PerformRendering(PdfImage image, PdfImageData imageData, Action<Bitmap> action) {
			GCHandle dataHandle = GCHandle.Alloc(imageData.Data, GCHandleType.Pinned);
			try {
				PixelFormat pixelFormat;
				switch (imageData.PixelFormat) {
					case PdfPixelFormat.Gray1bit:
						pixelFormat = PixelFormat.Format1bppIndexed;
						break;
					case PdfPixelFormat.Gray8bit:
						pixelFormat = PixelFormat.Format8bppIndexed;
						break;
					case PdfPixelFormat.Argb24bpp:
						pixelFormat = PixelFormat.Format24bppRgb;
						break;
					case PdfPixelFormat.Argb32bpp:
						pixelFormat = PixelFormat.Format32bppArgb;
						break;
					default:
						pixelFormat = PixelFormat.Format24bppRgb;
						break;
				}
				using (Bitmap bitmap = new Bitmap(imageData.Width, imageData.Height, imageData.Stride, pixelFormat, dataHandle.AddrOfPinnedObject())) {
					PdfImageColor[] imagePalette = imageData.Palette;
					if (imagePalette != null) {
						ColorPalette bitmapPalette = bitmap.Palette;
						Color[] bitmapPaletteEntries = bitmapPalette.Entries;
						if (image.IsMask) {
							PdfGraphicsState graphicsState = GraphicsState;
							Color currentColor = ConvertToGDIPlusColor(graphicsState.NonStrokingColor, graphicsState.NonStrokingAlphaConstant);
							if (image.Decode[0].Min == 0) {
								bitmapPaletteEntries[0] = currentColor;
								bitmapPaletteEntries[1] = Color.FromArgb(0, 0, 0, 0);
							}
							else {
								bitmapPaletteEntries[0] = Color.FromArgb(0, 0, 0, 0);
								bitmapPaletteEntries[1] = currentColor;
							}
						}
						else {
							int length = imagePalette.Length;
							for (int i = 0; i < length; i++) {
								PdfImageColor color = imagePalette[i];
								bitmapPaletteEntries[i] = Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
							}
						}
						bitmap.Palette = bitmapPalette;
					}
					action(bitmap);
				}
			}
			finally {
				if (dataHandle.IsAllocated)
					dataHandle.Free();
			}
		}
	}
}

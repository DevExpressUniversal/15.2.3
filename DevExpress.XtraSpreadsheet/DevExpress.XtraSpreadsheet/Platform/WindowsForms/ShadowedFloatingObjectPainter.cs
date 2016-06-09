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
using System.Drawing;
using DevExpress.Utils;
using System.Drawing.Imaging;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting;
using System.Drawing.Drawing2D;
using DevExpress.Office.Layout;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraSpreadsheet.Drawing {
	public class ShadowedFloatingObjectPainter {
		DocumentLayoutUnitConverter unitConverter;
		OfficeImage image;
		Matrix transform;
		Rectangle bounds;
		Rectangle initialShapeBounds;
		Rectangle initialContentBounds;
		Color fillColor;
		Color outlineColor;
		float alpha = 1.0f;
		ImageSizeMode sizeMode = ImageSizeMode.StretchImage;
		public ShadowedFloatingObjectPainter(DocumentLayoutUnitConverter unitConverter) {
			this.unitConverter = unitConverter;
		}
		public OfficeImage Image { get { return image; } set { image = value; } }
		public ImageSizeMode SizeMode { get { return sizeMode; } set { sizeMode = value; } }
		public Matrix Transform { get { return transform; } set { transform = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle InitialShapeBounds { get { return initialShapeBounds; } set { initialShapeBounds = value; } }
		public Rectangle InitialContentBounds { get { return initialContentBounds; } set { initialContentBounds = value; } }
		public Color FillColor { get { return fillColor; } set { fillColor = value; } }
		public Color OutlineColor { get { return outlineColor; } set { outlineColor = value; } }
		public float Alpha { get { return alpha; } set { alpha = value; } }
		public void Paint(GraphicsCache cache) {
			Matrix originalTransform = null;
			SmoothingMode originalSmoothingMode = SmoothingMode.Default;
			Graphics graphics = cache.Graphics;
			if (transform != null) {
				originalTransform = graphics.Transform.Clone();
				Matrix newTransform = graphics.Transform;
				newTransform.Multiply(transform);
				graphics.Transform = newTransform;
				originalSmoothingMode = graphics.SmoothingMode;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
			}
			PaintCore(cache);
			if (transform != null) {
				graphics.Transform = originalTransform;
				graphics.SmoothingMode = originalSmoothingMode;
			}
		}
		void PaintCore(GraphicsCache cache) {
			Rectangle contentBounds = initialContentBounds;
			contentBounds.X += bounds.X - initialShapeBounds.X;
			contentBounds.Y += bounds.Y - initialShapeBounds.Y;
			contentBounds.Width += bounds.Width - initialShapeBounds.Width;
			contentBounds.Height += bounds.Height - initialShapeBounds.Height;
			DrawFeedbackShape(cache, bounds, contentBounds, FillColor, OutlineColor, unitConverter);
			if (image != null )
				DrawImage(cache, contentBounds);
			cache.DrawRectangle(cache.GetPen(Color.FromArgb(0x80, 0x00, 0x00, 0x00), 1), bounds);
		}
		void DrawImage(GraphicsCache cache, Rectangle bounds) {
			if (alpha == 1.0f) {
				cache.Graphics.DrawImage(image.NativeImage, bounds);
				return;
			}
			ColorMatrix matrix = new ColorMatrix();
			matrix.Matrix33 = alpha;
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			Size imgActualSize = image.NativeImage.Size;
			Rectangle imgRect = Rectangle.Round(DevExpress.Data.Utils.ImageTool.CalculateImageRectCore(bounds, imgActualSize, SizeMode));
			GraphicsClipState oldClipState = cache.ClipInfo.SaveClip();
			cache.ClipInfo.SetClip(bounds);
			try {
				cache.Graphics.DrawImage(image.NativeImage, new Point[] { imgRect.Location, new Point(imgRect.Right, imgRect.Top), new Point(imgRect.Left, imgRect.Bottom) }, new Rectangle(Point.Empty, imgActualSize), GraphicsUnit.Pixel, attributes);
			}
			finally {
				cache.ClipInfo.RestoreClip(oldClipState);
			}
		}
		void DrawFeedbackShape(GraphicsCache cache, Rectangle shapeBounds, Rectangle contentBounds, Color fillColor, Color outlineColor, DocumentLayoutUnitConverter unitConverter) {
			if (!DXColor.IsTransparentOrEmpty(fillColor))
				cache.FillRectangle(fillColor, contentBounds);
			if (initialShapeBounds != initialContentBounds) {
				Rectangle bounds;
				bounds = new Rectangle(shapeBounds.X, shapeBounds.Y, contentBounds.X - shapeBounds.X, shapeBounds.Height);
				cache.FillRectangle(outlineColor, bounds);
				bounds = new Rectangle(contentBounds.X, shapeBounds.Y, contentBounds.Width, contentBounds.Y - shapeBounds.Y);
				cache.FillRectangle(outlineColor, bounds);
				bounds = new Rectangle(contentBounds.Right, shapeBounds.Y, shapeBounds.Right - contentBounds.Right, shapeBounds.Height);
				cache.FillRectangle(outlineColor, bounds);
				bounds = new Rectangle(contentBounds.X, contentBounds.Bottom, contentBounds.Width, shapeBounds.Bottom - contentBounds.Bottom);
				cache.FillRectangle(outlineColor, bounds);
			}
		}
	}
}

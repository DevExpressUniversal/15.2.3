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
using System.Text;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export;
using DevExpress.Data.Utils;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class ImageBrickExporter : VisualBrickExporter {
		static PointF SnapPointF(IGraphics gr, PointF point) {
			PointF point2 = GraphicsUnitConverter.Convert(point, GraphicsDpi.Document, gr.Dpi);
			point2 = GraphicsUnitConverter.Round(point2);
			return GraphicsUnitConverter.Convert(point2, gr.Dpi, GraphicsDpi.Document);
		}
		ImageBrick ImageBrick { get { return Brick as ImageBrick; } }
		public ImageSizeMode SizeMode {
			get { return ImageBrick.SizeMode; }
			set { ImageBrick.SizeMode = value; }
		}
		public ImageAlignment ImageAlignment {
			get { return ImageBrick.ImageAlignment; }
		}
		public Image Image { get { return ImageBrick.Image; } set { ImageBrick.Image = value; } }
		protected override RectangleF GetClipRect(RectangleF rect, IGraphics gr) {
			RectangleF clipRect = BrickPaint.GetClientRect(rect);
			return ImageBrick.Padding.Deflate(clipRect, GraphicsDpi.Document);
		}
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			if (clientRect.IsEmpty || Image == null)
				return;
			RectangleF imageRect = GetImageRect(gr, clientRect);
			if (CanSnapImage(gr.Dpi))
				imageRect.Location = SnapPointF(gr, imageRect.Location);
			DrawImage(gr, imageRect);
		}
		protected override object[] GetSpecificKeyPart() {
			return new object[] { SizeMode, ImageAlignment, Image };
		}
		protected override Image DrawContentToImage(ExportContext exportContext, RectangleF rect) {
			return DrawContentToImage(exportContext.PrintingSystem, exportContext.PrintingSystem.GarbageImages, rect, true, (int)GetResolution());
		}
		protected override RectangleF ConvertClientRect(RectangleF clientRect, float resolution) {
			return GraphicsUnitConverter.Convert(clientRect, resolution, GraphicsDpi.Document);
		}
		RectangleF GetImageRect(IGraphics gr, RectangleF clientRect) {
			return ImageTool.CalculateImageRect(clientRect, GetResolutionImageSize(gr), SizeMode, ImageBrick.ImageAlignment);
		}
		bool CanSnapImage(float dpi) {
			return (SizeMode == ImageSizeMode.Normal || SizeMode == ImageSizeMode.CenterImage || SizeMode == ImageSizeMode.AutoSize) &&
					(float)Math.Round(Image.HorizontalResolution) == dpi &&
					(float)Math.Round(Image.VerticalResolution) == dpi;
		}
		void DrawImage(IGraphics gr, RectangleF imageRect) {
			Image actualImage = SizeMode != ImageSizeMode.Tile ? Image :
				GetTileImage(Image, Rectangle.Round(GraphicsUnitConverter.Convert(imageRect, GraphicsDpi.Document, GraphicsDpi.Pixel)));
			if (gr is PdfGraphics)
				gr.DrawImage(actualImage, imageRect, GetNewColor(gr.PrintingSystem));
			else
				gr.DrawImage(actualImage, imageRect);
		}
		float GetResolution() {
			if(this.Image != null) {
				float result = Math.Max(Image.VerticalResolution, GraphicsDpi.DeviceIndependentPixel);
				return this.Image is System.Drawing.Imaging.Metafile ? Math.Min(result, 300) : result;
			}
			return GraphicsDpi.DeviceIndependentPixel;
		}
		Color GetNewColor(PrintingSystemBase ps) {
			Color color = ImageBrick.BackColor;
			if (color.A > 0)
				return color;
			color = GetPageBackColor(ps);
			if (color.A > 0)
				return color;
			return DXColor.Empty;
		}
		Color GetPageBackColor(PrintingSystemBase ps) {
			Color pageBackColor = ps.Graph.PageBackColor;
			return PSNativeMethods.ValidateBackgrColor(pageBackColor);
		}
		protected override void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
			Rectangle boundsWithoutBorders = GetBoundsWithoutBorders(exportProvider.CurrentData.OriginalBounds);
			System.Drawing.Size imageSize = GetResolutionImageSize(exportProvider.ExportContext);
			Size realImageSize = new Size(boundsWithoutBorders.Width - ImageBrick.Padding.Right - ImageBrick.Padding.Left,
										  boundsWithoutBorders.Height - ImageBrick.Padding.Top - ImageBrick.Padding.Bottom);
			Image actualImage = SizeMode != ImageSizeMode.Tile ? Image : GetTileImage(Image, realImageSize);
			exportProvider.SetCellImage(actualImage, ImageBrick.HtmlImageUrl, SizeMode, ImageAlignment, boundsWithoutBorders, imageSize, ImageBrick.Padding, Url);
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			if(exportContext.RawDataMode || !exportContext.CanPublish(ImageBrick))
				return new BrickViewData[0];
			return DrawContentToViewData(exportContext, GraphicsUnitConverter.Round(rect), GetTextAlignment());
		}
		protected override void FillRtfTableCellCore(IRtfExportProvider exportProvider) {
			FillRtfTableCellWithImage(exportProvider, SizeMode, ImageAlignment);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			FillXlsxTableCellWithImage(exportProvider, SizeMode, ImageAlignment);
		}
		void FillTableCell(ITableExportProvider exportProvider) {
			FillTableCellWithImage(exportProvider, SizeMode, ImageAlignment, exportProvider.CurrentData.Bounds);
		}
		TextAlignment GetTextAlignment() {
			switch(SizeMode) {
				case ImageSizeMode.AutoSize:
					return TextAlignment.TopLeft;
				case ImageSizeMode.CenterImage:
				case ImageSizeMode.StretchImage:
				case ImageSizeMode.Tile:
					return TextAlignment.MiddleCenter;
				case ImageSizeMode.Normal:
				case ImageSizeMode.Squeeze:
				case ImageSizeMode.ZoomImage:
					switch(ImageAlignment) {
						case ImageAlignment.TopLeft:
							return TextAlignment.TopLeft;
						case ImageAlignment.TopCenter:
							return TextAlignment.TopCenter;
						case ImageAlignment.TopRight:
							return TextAlignment.TopRight;
						case ImageAlignment.MiddleLeft:
							return TextAlignment.MiddleLeft;
						case ImageAlignment.MiddleCenter:
							return TextAlignment.MiddleCenter;
						case ImageAlignment.MiddleRight:
							return TextAlignment.MiddleRight;
						case ImageAlignment.BottomLeft:
							return TextAlignment.BottomLeft;
						case ImageAlignment.BottomCenter:
							return TextAlignment.BottomCenter;
						case ImageAlignment.BottomRight:
							return TextAlignment.BottomRight;
						case ImageAlignment.Default:
							return SizeMode == ImageSizeMode.Normal ? TextAlignment.TopLeft : TextAlignment.MiddleCenter;
					}
					return TextAlignment.TopLeft;
			}
			return TextAlignment.TopLeft;
		}
		protected Size GetResolutionImageSize(IPrintingSystemContext context) {
			if (Image == null)
				return System.Drawing.Size.Empty;
			return MathMethods.Scale(ImageBrick.UseImageResolution ? PSNativeMethods.GetResolutionImageSize(Image) : Image.Size, VisualBrick.GetScaleFactor(context)).ToSize();
		}
		Image GetTileImage(Image baseImage, Rectangle rectangle) {
			return GetTileImage(baseImage, new Size(rectangle.Width, rectangle.Height));
		}
		Image GetTileImage(Image baseImage, Size clientSize) {
			if(baseImage == null) return baseImage;
			VisualBrick brick = Brick as VisualBrick;
			object key = new MultiKey(clientSize, HtmlImageHelper.GetImageHashCode(baseImage));
			Image image = brick.PrintingSystem.GarbageImages.GetImageByKey(key);
			if(image != null)
				return image;
			Bitmap bm = new Bitmap(clientSize.Width, clientSize.Height);
			Graphics gr = Graphics.FromImage(bm);
			int hCount = (int)Math.Ceiling((double)clientSize.Width / baseImage.Width);
			int vCount = (int)Math.Ceiling((double)clientSize.Height / baseImage.Height);
			for(int i = 0; i < hCount; i++) {
				for(int j = 0; j < vCount; j++) {
					gr.DrawImageUnscaled(baseImage, new Point(i * baseImage.Width, j * baseImage.Height));
				}
			}
			brick.PrintingSystem.GarbageImages.Add(key, bm);
			return (Image)bm;
		}
	}
}

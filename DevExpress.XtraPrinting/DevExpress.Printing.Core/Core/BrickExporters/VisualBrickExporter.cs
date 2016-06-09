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
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export;
using System.Windows.Forms;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport;
using DevExpress.XtraPrinting.Export.Web;
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.Printing;
using DevExpress.XtraReports.UI;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraPrinting.BrickExporters {
	public abstract class BrickImageProviderBase {
		public abstract Image CreateContentImage(PrintingSystemBase ps, RectangleF rect, bool patchTransparentBackground, float resolution);
		public abstract Image CreateImage(PrintingSystemBase ps, RectangleF rect, float resolution, Action<Graphics> callback);
	}
	public class VisualBrickExporter : BrickExporter {
		class BrickImageProvider : BrickImageProviderBase {
			VisualBrickExporter exporter;
			public BrickImageProvider(VisualBrickExporter exporter) {
				this.exporter = exporter;
			}
			public override Image CreateContentImage(PrintingSystemBase ps, RectangleF rect, bool patchTransparentBackground, float resolution) {
				BrickPaintBase store = exporter.BrickPaint;
				try {
					exporter.BrickPaint = new BrickPaintBase(patchTransparentBackground);
					return CreateImage(ps, exporter.DeflateBorderWidth(rect), resolution, null);
				} finally {
					exporter.BrickPaint = store;
				}
			}
			public override Image CreateImage(PrintingSystemBase ps, RectangleF rect, float resolution, Action<Graphics> callback) {
				RectangleF clientRect = GraphicsUnitConverter.Convert(rect, GraphicsDpi.DeviceIndependentPixel, resolution);
				Size imageSize = new Size(Convert.ToInt32(clientRect.Width), Convert.ToInt32(clientRect.Height));
				if(imageSize.Width <= 0 || imageSize.Height <= 0)
					return null;
				Bitmap img = CreateBitmap(imageSize.Width, imageSize.Height);
				CorrectResolution(img, resolution);
				if(resolution != GraphicsDpi.Document)
					clientRect = exporter.ConvertClientRect(clientRect, resolution);
				clientRect.Location = PointF.Empty;
				using(GdiGraphics gdiGraphics = new ImageGraphics(img, ps)) {
					if(callback != null) callback(gdiGraphics.Graphics);
					exporter.DrawCore(gdiGraphics, clientRect);
				}
				return img;
			}
		}
		protected static string GetActualText(string text) {
			return String.IsNullOrEmpty(text) ? " " : text;
		}
		static protected internal void SetHtmlAnchor(DXWebControlBase control, string anchorName, HtmlExportContext htmlExportContext) {
			if(control == null || string.IsNullOrEmpty(anchorName))
				return;
			DXHtmlAnchor a = new DXHtmlAnchor();
			a.Name = anchorName;
			if(htmlExportContext.ShouldBlockBookmarks)
				a.Style.Add(DXHtmlTextWriterStyle.Display, "block");
			control.Controls.AddAt(0, a);
		}
		protected VisualBrick VisualBrick { get { return Brick as VisualBrick; } }
		internal protected BrickPaintBase BrickPaint {
			get { return VisualBrick.PrintingSystem.BrickPainter; }
			private set { VisualBrick.PrintingSystem.BrickPainter = value; }
		}
		IBrickOwner BrickOwner { get { return VisualBrick.BrickOwner; } }
		protected BrickStyle Style {
			get {
				return VisualBrick.Style;
			}
		}
		protected string Text { get { return VisualBrick.Text; } set { VisualBrick.Text = value; } }
		public override void Draw(IGraphics gr, RectangleF rect, RectangleF parentRect) {
			if(gr.CanPublish(VisualBrick))
				DrawCore(gr, rect);
		}
		void DrawCore(IGraphics gr, RectangleF rect) {
			if(gr is PdfGraphics)
				CreatePdfNavigation((PdfGraphics)gr, rect);
			BrickStyle oldBrickStyle = BrickPaint.BrickStyle;
			try {
				BrickPaint.BrickStyle = Style;
				DrawObject(gr, rect);
			} finally {
				BrickPaint.BrickStyle = oldBrickStyle;
			}
		}
		void CreatePdfNavigation(PdfGraphics gr, RectangleF bounds) {
			string url = Url;
			if(!string.IsNullOrEmpty(url)) {
				if(VisualBrick.HasCrossReference)
					SetGoToArea(gr, bounds, VisualBrick.NavigationPair);
				else
					gr.SetUriArea(url, bounds);
			}
		}
		static void SetGoToArea(PdfGraphics gr, RectangleF bounds, BrickPagePair pair) {
			RectangleF rect = pair.GetBrickBounds(gr.PrintingSystem.Pages);
			if(rect == RectangleF.Empty) return;
			gr.SetGoToArea(pair.PageIndex, rect.Top, bounds);
		}
		protected virtual void DrawObject(IGraphics gr, RectangleF rect) {
			DrawBackground(gr, rect);
			DrawForeground(gr, rect);
		}
		protected virtual void DrawBackground(IGraphics gr, RectangleF rect) {
			BrickPaint.DrawRect(gr, rect, gr.PrintingSystem.Gdi);
		}
		protected virtual void DrawForeground(IGraphics gr, RectangleF rect) {
			if(VisualBrick.NoClip) {
				DrawObjectCore(gr, rect);
				return;
			}
			RectangleF clipRect = GetClipRect(rect, gr);
			if(clipRect.IsEmpty)
				return;
			RectangleF oldClip = gr.ClipBounds;
			try {
				gr.ClipBounds = RectangleF.Intersect(gr.ClipBounds, clipRect);
				DrawObjectCore(gr, rect);
			} finally {
				gr.ClipBounds = oldClip;
			}
		}
		protected virtual RectangleF GetClipRect(RectangleF rect, IGraphics gr) {
			return BrickPaint.GetClientRect(rect);
		}
		protected void DrawObjectCore(IGraphics gr, RectangleF rect) {
			DrawClientContent(gr, GetClientRect(rect));
			VisualBrick.BrickOwner.RaiseDraw(VisualBrick, gr, rect);
		}
		protected virtual Image DrawContentToImage(ExportContext exportContext, RectangleF rect) {
			return DrawContentToImage(exportContext.PrintingSystem, exportContext.PrintingSystem.GarbageImages, rect, true, GraphicsDpi.DeviceIndependentPixel);
		}
		[Obsolete("Use the DrawContentToImage(PrintingSystemBase ps, ImagesContainer container, RectangleF rect, bool patchTransparentBackground, float resolution) method instead")]
		public Image DrawContentToImage(PrintingSystemBase ps, ImagesContainer container, RectangleF rect, bool drawBorders, bool patchTransparentBackground, float resolution) {
			throw new NotSupportedException();
		}
		public Image DrawContentToImage(PrintingSystemBase ps, ImagesContainer container, RectangleF rect, bool patchTransparentBackground, float resolution) {
			object key = CreateKey(rect, false, resolution);
			Image image = container.GetImageByKey(key);
			if(image != null)
				return image;
			BrickImageProviderBase imageProvider = CreateImageProvider();
			image = imageProvider.CreateContentImage(ps, rect, patchTransparentBackground, resolution);
			return container.GetImage(key, image);
		}
		[Obsolete("Use the CreateImageProvider() method instead")]
		public Image DrawContentToImage(PrintingSystemBase ps, RectangleF rect, bool drawBorders, bool patchTransparentBackground, float resolution) {
			throw new NotSupportedException();
		}
		public BrickImageProviderBase CreateImageProvider() {
			return new BrickImageProvider(this);
		}
#if DEBUGTEST
		public static Func<int, int, Bitmap> Test_BitmapCreator;
#endif
		static Bitmap CreateBitmap(int width, int height) {
#if DEBUGTEST
			if(Test_BitmapCreator != null)
				return Test_BitmapCreator(width, height);
#endif
			return new Bitmap(width, height);
		}
		static void CorrectResolution(Bitmap img, float resolution) {
			float newResolution = resolution > GraphicsDpi.DeviceIndependentPixel ? resolution : GraphicsDpi.DeviceIndependentPixel;
			img.SetResolution(newResolution, newResolution);
		}
		protected virtual RectangleF DeflateBorderWidth(RectangleF rect) {
			RectangleF resultRect = Style.DeflateBorderWidth(rect, GraphicsDpi.DeviceIndependentPixel, true);
			return resultRect;
		}
		protected virtual void DrawClientContent(IGraphics gr, RectangleF clientRect) {
		}
		protected virtual RectangleF GetClientRect(RectangleF rect) {
			RectangleF clientRect = BrickPaint.GetClientRect(rect);
			return VisualBrick.Padding.Deflate(clientRect, GraphicsDpi.Document);
		}
		object CreateKey(RectangleF rect, bool drawBorders, float resolution) {
			if(BrickOwner.CanCacheImages) {
				object[] specificKeyPart = GetSpecificKeyPart();
				if(specificKeyPart != null) {
					object[] keyParts = new object[] { drawBorders, resolution, rect.Size, Style };
					Array.Resize<object>(ref keyParts, keyParts.Length + specificKeyPart.Length);
					Array.Copy(specificKeyPart, 0, keyParts, keyParts.Length - specificKeyPart.Length, specificKeyPart.Length);
					return new MultiKey(keyParts);
				}
			}
			return null;
		}
		protected virtual object[] GetSpecificKeyPart() {
			return null;
		}
		protected virtual void DrawBorders(IGraphics gr, RectangleF rect) {
			BrickPaint.DrawBorder(gr, rect, gr.PrintingSystem.Gdi);
		}
		protected virtual RectangleF ConvertClientRect(RectangleF clientRect, float resolution) {
			return GraphicsUnitConverter.DipToDoc(clientRect);
		}
		protected internal override void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
			FillHtmlTableCellCore(exportProvider);
			exportProvider.SetNavigationUrl(VisualBrick);
			if(exportProvider.HtmlExportContext.CrossReferenceAvailable)
				exportProvider.SetAnchor(VisualBrick.AnchorName);
			exportProvider.RaiseHtmlItemCreated(VisualBrick);
		}
		protected virtual void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
		}
		protected internal override void FillRtfTableCellInternal(IRtfExportProvider exportProvider) {
			exportProvider.SetAnchor(VisualBrick.AnchorName);
			FillRtfTableCellCore(exportProvider);
		}
		protected virtual void FillRtfTableCellCore(IRtfExportProvider exportProvider) {
		}
		protected void FillHtmlTableCellWithImage(IHtmlExportProvider exportProvider) {
			Rectangle boundsWithoutBorders = GetBoundsWithoutBorders(exportProvider.CurrentData.OriginalBounds);
			FillHtmlTableCellWithImage(exportProvider, DrawContentToImage(exportProvider.HtmlExportContext, exportProvider.CurrentData.OriginalBounds), ImageSizeMode.StretchImage, ImageAlignment.Default, boundsWithoutBorders);
		}
		PaddingInfo CalcPaddings(BrickViewData data) {
			Rectangle innerBounds = data.Bounds;
			Rectangle outerBounds = data.OriginalBounds;
			PaddingInfo padding = new PaddingInfo();
			padding.Left = Math.Max(innerBounds.Left - outerBounds.Left, 0);
			padding.Right = Math.Max(outerBounds.Right - innerBounds.Right, 0);
			padding.Top = Math.Max(innerBounds.Top - outerBounds.Top, 0);
			padding.Bottom = Math.Max(outerBounds.Bottom - innerBounds.Bottom, 0);
			return padding;
		}
		protected void FillRtfTableCellWithImage(ITableExportProvider exportProvider, ImageSizeMode sizeMode, ImageAlignment align) {
			Image image = DrawContentToImage(exportProvider.ExportContext, exportProvider.CurrentData.OriginalBoundsF);
			if(image != null) {
				PaddingInfo padding = CalcPaddings(exportProvider.CurrentData);
				exportProvider.SetCellImage(image, null, sizeMode, align, exportProvider.CurrentData.OriginalBounds, image.Size, padding, Url);
			}
		}
		protected void FillXlsxTableCellWithImage(ITableExportProvider exportProvider, ImageSizeMode sizeMode, ImageAlignment align) {
			Image image = DrawContentToImage(exportProvider.ExportContext, exportProvider.CurrentData.OriginalBoundsF);
			if(image != null) {
				PaddingInfo padding = CalcPaddings(exportProvider.CurrentData);
				string hyperlink = ((XlsExportContext)exportProvider.ExportContext).XlsExportOptions.ExportHyperlinks ? Url : string.Empty;
				exportProvider.SetCellImage(image, null, sizeMode, align, Rectangle.Empty, Size.Empty, padding, hyperlink);
			}
		}
		protected void FillXlsxTableCellWithShape(ITableExportProvider exportProvider, Color lineColor, LineDirection lineDirection, DashStyle lineStyle, float lineWidth, PaddingInfo padding) {
			exportProvider.SetCellShape(lineColor, lineDirection, lineStyle, lineWidth, padding, Url);
		}
		protected void FillTableCellWithImage(ITableExportProvider exportProvider, ImageSizeMode sizeMode, ImageAlignment align, Rectangle bounds) {
			Image image = DrawContentToImage(exportProvider.ExportContext, exportProvider.CurrentData.BoundsF);
			if(image != null)
				exportProvider.SetCellImage(image, null, sizeMode, align, bounds, image.Size, PaddingInfo.Empty, Url);
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			exportProvider.SetCellText(GetActualText(Text), null);
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			if(!exportContext.CanPublish(VisualBrick))
				return new BrickViewData[0];
			return exportContext.CreateBrickViewDataArray(Style, rect, TableCell);
		}
		protected void FillHtmlTableCellWithImage(IHtmlExportProvider exportProvider, Image image, ImageSizeMode sizeMode, ImageAlignment align, Rectangle bounds) {
			if(image != null)
				exportProvider.SetCellImage(image, null, sizeMode, align, bounds, MathMethods.Scale(image.Size, VisualBrick.GetScaleFactor(exportProvider.ExportContext)).ToSize(), PaddingInfo.Empty, Url);
		}
		protected Rectangle GetBoundsWithoutBorders(Rectangle bounds) {
			Rectangle boundsWithoutBorders = bounds;
			boundsWithoutBorders.Size = System.Drawing.Size.Round(VisualBrick.GetClientRectangle(boundsWithoutBorders, GraphicsDpi.DeviceIndependentPixel).Size);
			return boundsWithoutBorders;
		}
		protected BrickViewData[] DrawContentToViewData(ExportContext exportContext, RectangleF bounds, TextAlignment textAligment) {
			BrickStyle style = new BrickStyle(Style);
			style.TextAlignment = textAligment;
			style = exportContext.PrintingSystem.Styles.GetStyle(style);
			return exportContext.CreateBrickViewDataArray(style, bounds, TableCell);
		}
		internal BrickStyle GetAreaStyle(ExportContext exportContext, RectangleF area, RectangleF baseBounds) {
			return VisualBrick.GetAreaStyle(exportContext.PrintingSystem.Styles, Style, area, baseBounds);
		}
	}
}

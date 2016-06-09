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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using System.Drawing.Imaging;
namespace DevExpress.XtraReports.Design.Drawing {
	public interface IColumnBandViewInfo {
		string MultiColumnText { get; }
		float GetUsefulColumnWidth();
		float GetColumnSpacing();
	}
	public interface IBandViewInfo {
		IList BandControls { get; }
		Band Band { get; }
		RectangleF BandBoundsF { get; }
		float WatermarkPartHeight { get; set; }
		float WatermarkPartOffset { get; set; }
		Image GridPattern { get; }
		int LeftMargin { get; }
		int RightMargin { get; }
		bool Expanded { get; }
		float ZoomFactor { get; }
	}
	public interface IBandViewInfoEx : IBandViewInfo {
		float GridPatternCellWidth { get; }
		double ViewportLeft { get; }
	}
	public abstract class BandViewPainterBase<TBandVIewInfo> where TBandVIewInfo : IBandViewInfo {
		protected class GraphicsHelper {
			GraphicsUnit pageUnit;
			Graphics gr;
			public GraphicsHelper(Graphics gr) {
				this.gr = gr;
				pageUnit = gr.PageUnit;
			}
			public void PrepareGraphics(TBandVIewInfo viewInfo, float zoomFactor, GraphicsUnit unit) {
				gr.PageUnit = unit;
				gr.ResetTransform();
				using(Matrix matrix = new Matrix()) {
					float posX = XRConvert.Convert(viewInfo.BandBoundsF.X + viewInfo.LeftMargin, GraphicsDpi.Pixel, GraphicsDpi.UnitToDpi(unit));
					float posY = XRConvert.Convert(viewInfo.BandBoundsF.Y, GraphicsDpi.Pixel, GraphicsDpi.UnitToDpi(unit));
					matrix.Translate(posX, posY);
					gr.MultiplyTransform(matrix);
				}
				gr.ScaleTransform(zoomFactor, zoomFactor);
			}
			public void ResetGraphics() {
				gr.PageUnit = pageUnit;
				gr.ResetTransform();
			}
		}
		#region static
		protected static readonly Color foregrColor = Color.FromArgb(0xda, 0xda, 0xda);
		static readonly Color backgrColor = Color.White;
		static readonly Color hideColor = Color.FromArgb(0xec, 0xec, 0xec);
		static readonly Color spaceColor = Color.White;
		static readonly int lineWidth = 1;
		#endregion
		protected IServiceProvider serviceProvider;
		protected PrintingSystemBase printingSystem;
		protected TBandVIewInfo viewInfo;
		IColumnBandViewInfo ColumnViewInfo {
			get { return viewInfo as IColumnBandViewInfo; }
		}
		protected RectangleF BandBoundsF {
			get { return viewInfo.BandBoundsF; }
		}
		protected Band Band {
			get { return viewInfo.Band; }
		}
		protected XtraReport Report {
			get { return Band.RootReport; }
		}
		protected float ZoomFactor {
			get { return viewInfo.ZoomFactor; }
		}
		protected float ScaledPixelDpi {
			get { return ZoomFactor * GraphicsDpi.Pixel; }
		}
		public BandViewPainterBase(IServiceProvider serviceProvider, PrintingSystemBase printingSystem) {
			this.serviceProvider = serviceProvider;
			this.printingSystem = printingSystem;
		}
		protected object GetService(Type serviceType) {
			return serviceProvider.GetService(serviceType);
		}
		public void Draw(TBandVIewInfo viewInfo, IGraphicsCache cache) {
			this.viewInfo = viewInfo;
			DrawCore(cache);
		}
		protected virtual void DrawCore(IGraphicsCache cache) {
			DrawBackground(cache);
			if(Report.DrawWatermark)
				DrawWatermark(cache, BandBoundsF);
			if(Report.DrawGrid && viewInfo.GridPattern != null) {
				SizeF gridSize = XRConvert.Convert(Report.GridSizeF, Report.Dpi, ScaledPixelDpi);
				DrawGrid(cache, BandBoundsF, gridSize, XtraReport.GridCellCount);
			}
			DrawColumn(cache);
			DrawContent(cache);
			DrawMargins(cache);
		}
		protected virtual void DrawBackground(IGraphicsCache cache) {
			cache.Graphics.FillRectangle(cache.GetSolidBrush(backgrColor), BandBoundsF);
		}
		protected virtual void DrawColumn(IGraphicsCache cache) {
			if(ColumnViewInfo == null) return;
			RectangleF rect = GetNonColumnBounds();
			cache.Graphics.FillRectangle(cache.GetSolidBrush(hideColor), rect);
			Brush brush = cache.GetSolidBrush(foregrColor);
			float pixColumnSpacing = XRConvert.Convert(ColumnViewInfo.GetColumnSpacing(), Report.Dpi, ScaledPixelDpi);
			if(pixColumnSpacing > lineWidth) {
				RectangleF spaceRect = rect;
				spaceRect.Width = pixColumnSpacing;
				cache.Graphics.FillRectangle(cache.GetSolidBrush(spaceColor), spaceRect);
				cache.Graphics.FillRectangle(brush, spaceRect.Right - lineWidth, spaceRect.Y, lineWidth, spaceRect.Height);
			}
			cache.Graphics.FillRectangle(brush, rect.X, rect.Y, lineWidth, rect.Height);
			rect.Width -= viewInfo.RightMargin;
			DrawString(cache.Graphics, rect, ColumnViewInfo.MultiColumnText);
		}
		protected virtual void DrawContent(IGraphicsCache cache) {
		}
		static void DrawString(Graphics gr, RectangleF rect, string text) {
			if(string.IsNullOrEmpty(text) || rect.Width == 0 || rect.Height == 0)
				return;
			using(StringFormat sf = StringFormat.GenericTypographic.Clone() as StringFormat) {
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				gr.DrawString(text, XRControlStyle.DefaultFont, Brushes.Black, rect, sf);
			}
		}
		RectangleF GetNonColumnBounds() {
			float pixColumnWidth = XRConvert.Convert(ColumnViewInfo.GetUsefulColumnWidth(), Report.Dpi, ScaledPixelDpi);
			return RectHelper.DeflateRect(viewInfo.BandBoundsF, viewInfo.LeftMargin + pixColumnWidth, 0, 0, 0);
		}
		protected virtual void DrawMargins(IGraphicsCache cache) {
		}
		protected virtual void DrawGrid(IGraphicsCache cache, RectangleF bounds, SizeF gridSize, int cellCount) {
			float height = gridSize.Height * cellCount;
			for(float y = bounds.Top; y <= bounds.Bottom; y += height) {
				cache.Graphics.DrawImageUnscaled(viewInfo.GridPattern, (int)bounds.Left, (int)y);
			}
		}
		void DrawWatermark(IGraphicsCache cache, RectangleF bounds) {
			if(Report.Watermark.Image == null && string.IsNullOrEmpty(Report.Watermark.Text))
				return;
			GraphicsUnit oldPageUnit = cache.Graphics.PageUnit;
			IGraphics gdiGraphics = new GdiGraphics(cache.Graphics, printingSystem);
			RectangleF oldClipBounds = gdiGraphics.ClipBounds;
			RectangleF clipBounds = bounds;
			clipBounds.Height = viewInfo.WatermarkPartHeight;
			clipBounds = GraphicsUnitConverter.Convert(clipBounds, GraphicsUnit.Pixel, GraphicsUnit.Document);
			gdiGraphics.ClipBounds = clipBounds;
			RectangleF repBounds = GraphicsUnitConverter.Convert(Report.PageBounds, Report.Dpi, GraphicsDpi.Document);
			PointF offset = new PointF(bounds.X, bounds.Y - viewInfo.WatermarkPartOffset);
			PointF offset2 = XRConvert.Convert(offset, ScaledPixelDpi, GraphicsDpi.Document);
			repBounds.Offset(offset2);
			gdiGraphics.ScaleTransform(viewInfo.ZoomFactor, viewInfo.ZoomFactor);
			Report.Watermark.Draw(gdiGraphics, repBounds, 0, 1);
			gdiGraphics.ResetTransform();
			gdiGraphics.ClipBounds = oldClipBounds;
			cache.Graphics.PageUnit = oldPageUnit;
		}
		protected static void DrawXRControl(IGraphics gr, XRControl xrControl, XRWriteInfo writeInfo) {
			RectangleF bounds = XRConvert.Convert(xrControl.BoundsF, xrControl.Dpi, GraphicsDpi.Document);
			if(bounds.IntersectsWith(gr.ClipBounds)) {
				using(VisualBrick brick = xrControl.GetDesignerBrick(gr.PrintingSystem, writeInfo)) {
					if(xrControl is XRTable && writeInfo.MergedCells.Count > 1) {
						RowSpanHelper.MergeBricks(brick, writeInfo.MergedCells);
					}
					RectangleF oldBounds = gr.ClipBounds;
					try {
						gr.ClipBounds = RectangleF.Intersect(gr.ClipBounds, bounds);
						VisualBrickHelper.DrawBrick(brick, gr, bounds, gr.ClipBounds);
					} finally {
						gr.ClipBounds = oldBounds;
					}
				}
			}
		}
	}
	public static class GridPatternCreator {
		static readonly Color foregrColor1 = Color.FromArgb(221, 221, 221);
		static readonly Color foregrColor2 = Color.FromArgb(188, 188, 188);
		const int lineTransparency = 155;
		public static Image Create(SizeF gridSize, int cellCount, int width, int left, int lineWidth) {
			float height = gridSize.Height * cellCount;
			int bmpHeight = (int)Math.Ceiling(height);
			if(width <= 0 || bmpHeight <= 0)
				return null;
			Bitmap bitmap = new Bitmap(width, bmpHeight);
			if(bitmap.PixelFormat == PixelFormat.Format32bppArgb) {
				return CreatePatternFast(gridSize, cellCount, width, left, lineWidth, height, bitmap);
			} else {
				return CreatePatternUniversal(gridSize, cellCount, width, left, lineWidth, height, bitmap);
			}
		}
		static Image CreatePatternUniversal(SizeF gridSize, int cellCount, int width, int left, int lineWidth, float height, Bitmap bitmap) {
			using(Brush brush1 = new SolidBrush(foregrColor1))
			using(Brush brush2 = new SolidBrush(foregrColor2)) {
				using(Graphics gr = Graphics.FromImage(bitmap)) {
					Action<SizeF, Brush> fillGrid = delegate(SizeF size, Brush brush) {
						int count = 0;
						for(float x = left; x <= width - lineWidth; x += size.Width, count++)
							gr.FillRectangle(brush, x, 0, lineWidth, height);
						count = 0;
						for(float x = left; x > 0; x -= size.Width, count++)
							gr.FillRectangle(brush, x, 0, lineWidth, height);
						for(float y = size.Height; y <= height; y += size.Height)
							gr.FillRectangle(brush, 0, y - lineWidth, width, lineWidth);
					};
					fillGrid(gridSize, brush1);
					gridSize = new SizeF(gridSize.Width * cellCount, gridSize.Height * cellCount);
					fillGrid(gridSize, brush2);
				}
				Bitmap resultBitmap = new Bitmap(bitmap.Width, bitmap.Height);
				BitmapCreator.TransformBitmap(bitmap, resultBitmap, BitmapCreator.CreateTransparencyAttributes(lineTransparency));
				bitmap.Dispose();
				return resultBitmap;
			}
		}
		[System.Security.SecuritySafeCritical]
		static Image CreatePatternFast(SizeF gridSize, int cellCount, int width, int left, int lineWidth, float height, Bitmap bitmap) {
			  int length = bitmap.Width * bitmap.Height;
			int[] rgbValues = new int[length];
			SizeF size = gridSize;
			int foreColor1 = Color.FromArgb(255 - lineTransparency, foregrColor1).ToArgb();
			int foreColor2 = Color.FromArgb(255 - lineTransparency, foregrColor2).ToArgb();
			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
			try {
				DrawVerticalLines(left, width, lineWidth, rgbValues, size.Width, foreColor1);
				for(float y = size.Height; y < height; y += size.Height)
					DrawHorizontalLine(y, width, rgbValues, foreColor1);
				DrawVerticalLines(left, width, lineWidth, rgbValues, size.Width * cellCount, foreColor2);
				DrawHorizontalLine(bitmap.Height - 1, width, rgbValues, foreColor2);
				System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, data.Scan0, length);
			} finally {
				bitmap.UnlockBits(data);
			}
			return bitmap;
		}
		static void DrawHorizontalLine(float fy, int width, int[] rgbValues, int color) {
			int y = (int)Math.Round(fy, MidpointRounding.AwayFromZero);
			int lineStart = y * width;
			int lineEnd = lineStart + width;
			if(lineStart >= rgbValues.Length || lineEnd > rgbValues.Length)
				return;
			for(int i = lineStart; i < lineEnd; i++)
				rgbValues[i] = color;
		}
		static void DrawVerticalLines(float left, int width, int lineWidth, int[] rgbValues, float step, int color) {
			int end = width - lineWidth;
			for(float x = left; x <= end; x += step)
				for(int i = (int)Math.Round(x, MidpointRounding.AwayFromZero); i < rgbValues.Length; i += width)
					rgbValues[i] = color;
			for(float x = left; x > 0; x -= step)
				for(int i = (int)Math.Round(x, MidpointRounding.AwayFromZero); i < rgbValues.Length; i += width)
					rgbValues[i] = color;
		}
	}
	public class WatermarkInfoCalculator {
		IList<IBandViewInfo> viewInfos;
		float zoomFactor;
		XtraReport report;
		int startBand;
		int endBand;
		float bandOffset;
		float availableHeight;
		float availableHeightFromTop;
		float ReportDpi { get { return report.Dpi; } }
		float ReportPageHeight { get { return report.PageHeight; } }
		public WatermarkInfoCalculator(IList<IBandViewInfo> viewInfos, XtraReport report, float zoomFactor) {
			this.viewInfos = viewInfos;
			this.zoomFactor = zoomFactor;
			this.report = report;
		}
		public void Calculate() {
			startBand = 0;
			endBand = viewInfos.Count - 1;
			bandOffset = 0;
			availableHeightFromTop = XRConvert.Convert(ReportPageHeight, ReportDpi, zoomFactor * GraphicsDpi.Pixel);
			availableHeight = availableHeightFromTop;
			PreCalculateWatermark();
			for(int i = startBand; i <= endBand; i++) {
				IBandViewInfo currentBand = viewInfos[i];
				if(i == endBand &&
					currentBand.Band.BandKind == BandKind.ReportFooter &&
					availableHeight > GetBandHeight(currentBand) &&
					((ReportFooterBand)currentBand.Band).PrintAtBottom == true
					)
					CutWatermarkFromBottom(currentBand);
				else
					CutWatermarkFromTop(currentBand);
			}
		}
		void CutWatermarkFromTop(IBandViewInfo band) {
			band.WatermarkPartHeight = Math.Min(availableHeight, GetBandHeight(band));
			band.WatermarkPartOffset = bandOffset;
			availableHeight -= band.WatermarkPartHeight;
			bandOffset = band.WatermarkPartHeight + band.WatermarkPartOffset;
		}
		void CutWatermarkFromBottom(IBandViewInfo band) {
			band.WatermarkPartHeight = Math.Min(availableHeight, GetBandHeight(band));
			band.WatermarkPartOffset = availableHeightFromTop - band.WatermarkPartHeight;
			availableHeight -= band.WatermarkPartHeight;
			availableHeightFromTop -= band.BandBoundsF.Height;
		}
		void PreCalculateWatermark() {
			CutWatermarkFromTop(BandKind.TopMargin);
			CutWatermarkFromBottom(BandKind.BottomMargin);
			CutWatermarkFromTop(BandKind.ReportHeader);
			CutWatermarkFromTop(BandKind.PageHeader);
			CutWatermarkFromBottom(BandKind.PageFooter);
		}
		void CutWatermarkFromTop(BandKind kind) {
			IBandViewInfo band = GetBandInfo(report.Bands[kind]);
			if(band != null) {
				CutWatermarkFromTop(band);
				startBand++;
			}
		}
		void CutWatermarkFromBottom(BandKind kind) {
			IBandViewInfo band = GetBandInfo(report.Bands[kind]);
			if(band != null) {
				CutWatermarkFromBottom(band);
				endBand--;
			}
		}
		IBandViewInfo GetBandInfo(Band band) {
			if(band == null) return null;
			return viewInfos.FirstOrDefault<IBandViewInfo>(item => ReferenceEquals(item.Band, band));
		}
		float GetBandHeight(IBandViewInfo band) {
			if(band.Expanded)
				return band.BandBoundsF.Height;
			float height = CalculateHeight(band.Band);
			return XRConvert.Convert(height, ReportDpi, zoomFactor * GraphicsDpi.Pixel);
		}
		float CalculateHeight(Band band) {
			float height = 0;
			if(band is DetailReportBand) {
				foreach(Band nestedBand in ((DetailReportBand)band).Bands) {
					height += CalculateHeight(nestedBand);
				}
			} else
				height = band.HeightF;
			return height;
		}
	}
}
namespace DevExpress.XtraReports.Xpf.Drawing {
	using DevExpress.XtraReports.Design.Drawing;
	using DevExpress.XtraReports.Localization;
	public class BandViewInfo : IBandViewInfoEx {
		IList bandControls;
		public IList BandControls {
			get {
				if(bandControls == null)
					bandControls = Band.GetPrintableControls();
				return bandControls;
			}
		}
		public Band Band { get; private set; }
		public RectangleF BandBoundsF { get; set; }
		public float WatermarkPartHeight { get; set; }
		public float WatermarkPartOffset { get; set; }
		public Image GridPattern { get; set; }
		public float GridPatternCellWidth { get; set; }
		public int LeftMargin { get; set; }
		public int RightMargin { get; set; }
		public bool Expanded { get; set; }
		public float ZoomFactor { get; set; }
		public double ViewportLeft { get; set; }
		public BandViewInfo(Band band) {
			Band = band;
		}
	}
	class ColumnBandViewInfo : BandViewInfo, IColumnBandViewInfo {
		XtraReports.UI.MultiColumn multiColumn;
		public string MultiColumnText { get; set; }
		public ColumnBandViewInfo(Band band, XtraReports.UI.MultiColumn multiColumn)
			: base(band) {
			this.multiColumn = multiColumn;
		}
		public float GetUsefulColumnWidth() {
			return multiColumn.GetUsefulColumnWidth(Band.ClientRectangleF.Width, Band.Dpi);
		}
		public float GetColumnSpacing() {
			return multiColumn.GetColumnSpacingInDpi(Band.Dpi);
		}
	}
	public class BandViewInfoBuilder : IDisposable {
		XtraReport report;
		float zoomFactor;
		int gridWidth;
		Image gridPattern;
		float gridPatternCellWidth;
		float viewportLeft;
		public BandViewInfoBuilder(XtraReport report) {
			this.report = report;
		}
		public IBandViewInfoEx Build(Band band, float zoomFactor, RectangleF viewport) {
			this.viewportLeft = viewport.Left;
			var gridSize = XRConvert.Convert(report.GridSizeF, report.Dpi, zoomFactor * GraphicsDpi.Pixel);
			gridPatternCellWidth = gridSize.Width * XtraReport.GridCellCount;
			int gridColumnsCount = (int)((viewport.Right - viewportLeft) / gridPatternCellWidth) + 1;
			int gridWidth = (int)Math.Ceiling(gridColumnsCount * gridPatternCellWidth);
			if(this.zoomFactor != zoomFactor || this.gridWidth != gridWidth) {
				this.zoomFactor = zoomFactor;
				this.gridWidth = gridWidth;
				DisposeGridPattern();
				gridPattern = GridPatternCreator.Create(gridSize, XtraReport.GridCellCount, gridWidth, 0, 1);
			}
			IList<IBandViewInfoEx> viewInfos = new List<IBandViewInfoEx>();
			AddViewInfos(viewInfos, report.OrderedBands);
			new WatermarkInfoCalculator(viewInfos.Cast<IBandViewInfo>().ToList(), report, zoomFactor).Calculate();
			foreach(var viewInfo in viewInfos) {
				var writableViewInfo = (BandViewInfo)viewInfo;
				writableViewInfo.BandBoundsF = new RectangleF(new PointF(-viewport.Left, -viewport.Top), writableViewInfo.BandBoundsF.Size);
			}
			return viewInfos.Where(x => x.Band == band).First();
		}
		void AddViewInfos(IList<IBandViewInfoEx> viewInfos, IEnumerable bands) {
			foreach(Band band in bands) {
				BandViewInfo viewInfo = CreateViewInfo(band);
				viewInfos.Add(viewInfo);
				AddViewInfos(viewInfos, band.OrderedBands);
			}
		}
		BandViewInfo CreateViewInfo(Band band) {
			XtraReports.UI.MultiColumn mc;
			BandViewInfo viewInfo = Band.TryGetMultiColumn(band, out mc) ?
				new ColumnBandViewInfo(band, mc) { MultiColumnText = GetMultiColumnText(band) } :
				new BandViewInfo(band);
			viewInfo.ViewportLeft = viewportLeft;
			viewInfo.GridPattern = gridPattern;
			viewInfo.GridPatternCellWidth = gridPatternCellWidth;
			viewInfo.ZoomFactor = zoomFactor;
			float width = XRConvert.Convert(report.PageWidth, report.Dpi, zoomFactor * GraphicsDpi.Pixel);
			float height = XRConvert.Convert(band.HeightF, band.Dpi, zoomFactor * GraphicsDpi.Pixel);
			viewInfo.BandBoundsF = new RectangleF(0f, 0f, width, height);
			viewInfo.LeftMargin = XRConvert.Convert(report.Margins.Left, report.Dpi, zoomFactor * GraphicsDpi.Pixel);
			viewInfo.RightMargin = XRConvert.Convert(report.Margins.Right, report.Dpi, zoomFactor * GraphicsDpi.Pixel);
			return viewInfo;
		}
		static string GetMultiColumnText(Band band) {
			return band is DetailBand ? ReportStringId.MultiColumnDesignMsg1.GetString() + '\x000D' + '\x000A' + ReportStringId.MultiColumnDesignMsg2.GetString() : string.Empty;
		}
		public void Dispose() {
			DisposeGridPattern();
		}
		void DisposeGridPattern() {
			if(gridPattern != null) {
				gridPattern.Dispose();
				gridPattern = null;
			}
		}
	}
	public class BandViewPainter : BandViewPainterBase<IBandViewInfoEx> {
		public BandViewPainter(IServiceProvider serviceProvider, PrintingSystemBase printingSystem) :
			base(serviceProvider, printingSystem) {
		}
		protected override void DrawCore(IGraphicsCache cache) {
			RectangleF clipBounds = cache.Graphics.ClipBounds;
			if(!BandBoundsF.IntersectsWith(clipBounds)) return;
			cache.Graphics.SetClip(RectangleF.Intersect(clipBounds, viewInfo.BandBoundsF));
			try {
				base.DrawCore(cache);
			} finally {
				cache.Graphics.SetClip(clipBounds);
			}
		}
		protected override void DrawContent(IGraphicsCache cache) {
			base.DrawContent(cache);
			if(Band != null)
				DrawXRControlCollection(cache.Graphics, viewInfo.BandControls);
		}
		protected override void DrawGrid(IGraphicsCache cache, RectangleF bounds, SizeF gridSize, int cellCount) {
			float gridLeft = bounds.Left + (int)(viewInfo.ViewportLeft / viewInfo.GridPatternCellWidth) * viewInfo.GridPatternCellWidth;
			float height = gridSize.Height * cellCount;
			for(float y = bounds.Top; y <= bounds.Bottom; y += height) {
				cache.Graphics.DrawImageUnscaled(viewInfo.GridPattern, (int)gridLeft, (int)y);
			}
		}
		void DrawXRControlCollection(Graphics gr, IList xrControls) {
			GraphicsHelper helper = new GraphicsHelper(gr);
			try {
				printingSystem.Graph.PageUnit = GraphicsUnit.Document;
				helper.PrepareGraphics(viewInfo, ZoomFactor, GraphicsUnit.Document);
				IGraphics gdiGraphics = new GdiGraphics(gr, printingSystem);
				XRWriteInfo writeInfo = new XRWriteInfo(printingSystem);
				foreach(XRControl xrControl in xrControls)
					DrawXRControl(gdiGraphics, xrControl, writeInfo);
			} finally {
				helper.ResetGraphics();
			}
		}
	}
	public class GraphicsCache : IGraphicsCache {
		class ColorComparer : IEqualityComparer<Color> {
			public bool Equals(Color x, Color y) {
				return x == y;
			}
			public int GetHashCode(Color obj) {
				return obj.GetHashCode();
			}
		}
		Dictionary<Color, Brush> solidBrushes;
		public GraphicsCache(Graphics graphics) {
			Graphics = graphics;
			solidBrushes = new Dictionary<Color, Brush>(new ColorComparer());
		}
		protected static void ClearHashtable(IDictionary hash) {
			if(hash == null) return;
			foreach(IDisposable obj in hash.Values) {
				obj.Dispose();
			}
			hash.Clear();
		}
		#region IGraphicsCache Members
		public Rectangle CalcClipRectangle(Rectangle r) {
			throw new NotImplementedException();
		}
		public Rectangle CalcRectangle(Rectangle r) {
			throw new NotImplementedException();
		}
		public SizeF CalcTextSize(string text, Font font, StringFormat strFormat, int maxWidth, int maxHeight, out bool isCropped) {
			throw new NotImplementedException();
		}
		public SizeF CalcTextSize(string text, Font font, StringFormat strFormat, int maxWidth, int maxHeight) {
			throw new NotImplementedException();
		}
		public SizeF CalcTextSize(string text, Font font, StringFormat strFormat, int maxWidth) {
			throw new NotImplementedException();
		}
		public void Clear() {
			throw new NotImplementedException();
		}
		public void DrawRectangle(Pen pen, Rectangle r) {
			throw new NotImplementedException();
		}
		public void DrawString(string text, Font font, Brush foreBrush, Rectangle bounds, StringFormat strFormat) {
			throw new NotImplementedException();
		}
		public void DrawVString(string text, Font font, Brush foreBrush, Rectangle bounds, StringFormat strFormat, int angle) {
			throw new NotImplementedException();
		}
		public void FillRectangle(Color color, Rectangle rect) {
			throw new NotImplementedException();
		}
		public void FillRectangle(Brush brush, RectangleF rect) {
			throw new NotImplementedException();
		}
		public void FillRectangle(Brush brush, Rectangle rect) {
			throw new NotImplementedException();
		}
		public Font GetFont(Font font, FontStyle fontStyle) {
			throw new NotImplementedException();
		}
		public Brush GetGradientBrush(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode, int blendCount) {
			throw new NotImplementedException();
		}
		public Brush GetGradientBrush(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode) {
			throw new NotImplementedException();
		}
		public Pen GetPen(Color color, int width) {
			throw new NotImplementedException();
		}
		public Pen GetPen(Color color) {
			throw new NotImplementedException();
		}
		public Brush GetSolidBrush(Color color) {
			Brush brush = null;
			if(color.IsSystemColor) brush = GetSystemBrush(color);
			if(brush != null) return brush;
			if(solidBrushes.TryGetValue(color, out brush))
				return brush;
			brush = new SolidBrush(color);
			solidBrushes.Add(color, brush);
			return brush;
		}
		protected virtual Brush GetSystemBrush(Color color) {
			return SystemBrushes.FromSystemColor(color);
		}
		public Graphics Graphics {
			get;
			private set;
		}
		public bool IsNeedDrawRect(Rectangle r) {
			throw new NotImplementedException();
		}
		public Point Offset {
			get { throw new NotImplementedException(); }
		}
		public void ResetMatrix() {
			throw new NotImplementedException();
		}
		public Matrix TransformMatrix {
			get { throw new NotImplementedException(); }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			if(solidBrushes != null)
				ClearHashtable(solidBrushes);
		}
		#endregion
	}
}

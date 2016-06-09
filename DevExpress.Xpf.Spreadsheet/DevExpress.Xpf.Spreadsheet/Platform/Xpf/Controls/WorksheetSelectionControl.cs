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
using System.Windows;
using System.Windows.Media;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using SpreadsheetLayoutPage = DevExpress.XtraSpreadsheet.Layout.Page;
using System.IO;
using System.Reflection;
using DevExpress.Data.Utils;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region WorksheetSelectionControl
	public class WorksheetSelectionControl : WorksheetPaintControl, ISelectionPainter {
		#region Fields
		public static readonly DependencyProperty SelectionBrushProperty;
		public static readonly DependencyProperty SelectionBorderBrushProperty;
		public static readonly DependencyProperty AutoFilterBackgroundBrushProperty;
		const int smallBorderWidthInPixels = 1;
		const int largeBorderWidthInPixels = 2;
		const int borderOffsetInPixels = -3;
		double borderCornerOffset = Math.Ceiling((double)largeBorderWidthInPixels / 2);
		double[] dashPattern = new double[] { 2.5, 2 };
		DrawingContext drawingContext;
		#endregion
		static WorksheetSelectionControl() {
			Type ownerType = typeof(WorksheetSelectionControl);
			SelectionBrushProperty = DependencyProperty.Register("SelectionBrush", typeof(Brush), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((WorksheetSelectionControl)d).OnBrushChanged()));
			SelectionBorderBrushProperty = DependencyProperty.Register("SelectionBorderBrush", typeof(Brush), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((WorksheetSelectionControl)d).OnBrushChanged()));
			AutoFilterBackgroundBrushProperty = DependencyProperty.Register("AutoFilterBackgroundBrush", typeof(Brush), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((WorksheetSelectionControl)d).OnBrushChanged()));
		}
		public WorksheetSelectionControl() {
			RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
		}
		#region Properties
		public Brush SelectionBrush {
			get { return (Brush)GetValue(SelectionBrushProperty); }
			set { SetValue(SelectionBrushProperty, value); }
		}
		public Brush SelectionBorderBrush {
			get { return (Brush)GetValue(SelectionBorderBrushProperty); }
			set { SetValue(SelectionBorderBrushProperty, value); }
		}
		public Brush AutoFilterBackgroundBrush {
			get { return (Brush)GetValue(AutoFilterBackgroundBrushProperty); }
			set { SetValue(AutoFilterBackgroundBrushProperty, value); }
		}
		internal Rect SelectionBorderBounds { get; set; } 
		internal bool IsMultiSelection { get; set; }
		#endregion
		void OnBrushChanged() {
			InvalidateVisual();
		}
		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			if (LayoutInfo == null)
				return;
			this.drawingContext = dc;
			SpreadsheetProvider.ActiveView.SelectionLayout.Update();
			for (int i = 0; i < LayoutInfo.Pages.Count; i++) {
				SpreadsheetLayoutPage page = LayoutInfo.Pages[i];
				DrawPivotTableItems(page);
				DrawSelection(page);
				DrawCutCopyRange(page);
				DrawPictureSelection(page);
				DrawAutoFilters(page);
			}
		}
		void DrawPictureSelection(SpreadsheetLayoutPage page) {
			SpreadsheetView view = SpreadsheetProvider.ActiveView;
			PictureSelectionLayoutItemCollection selections = view.SelectionLayout.GetPictureSelection(page);
			int count = selections.Count;
			if (count == 0)
				return;
			for (int i = 0; i < count; i++) {
				PictureSelectionLayoutItem item = selections[i];
				item.Update(page);
				DrawPictureSelectionCore(item);
			}
		}
		private void DrawPictureSelectionCore(PictureSelectionLayoutItem item) {
			Rect bounds = item.Bounds.ToRect();
			Point center = HotZonePainter.CalcCenter(bounds);
			float angleInDegrees = item.DocumentModel.GetPictureRotationAngleInDegrees(item.PictureIndex);
			bool needToPushtransform = NeedPushRotationTransform(angleInDegrees);
			if (needToPushtransform) PushRotationTransform(drawingContext, center, angleInDegrees);
			Pen rectPen = new Pen(Brushes.LightBlue, 1);
			try {
				drawingContext.DrawRectangle(null, rectPen, bounds);
				DrawHotZones(item.HotZones);
			}
			finally {
				if (needToPushtransform)
					drawingContext.Pop();
			}
		}
		private bool NeedPushRotationTransform(float angleInDegrees) {
			return (angleInDegrees % 360f) != 0;
		}
		public void PushRotationTransform(DrawingContext dc, Point center, float angleInDegrees) {
			RotateTransform rotate = new RotateTransform(angleInDegrees) { CenterX = center.X, CenterY = center.Y };
			dc.PushTransform(rotate);
		}
		void DrawAutoFilters(Page page) {
			SpreadsheetView view = SpreadsheetProvider.ActiveView;
			AutoFilterLayout layout = view.AutoFilterLayout;
			layout.Update(page);
			DrawHotZones(layout.HotZones);
		}
		void DrawPivotTableItems(Page page) {
			PivotTableLayout layout = SpreadsheetProvider.ActiveView.PivotTableLayout;
			layout.Update(page);
			HotZonePainter painter = new HotZonePainter(drawingContext, null, Owner.AutoFilterImageCache, Owner.GetDataValidationBackgroundBrush(), Owner.GetDataValidationBorderColor());
			painter.DrawHotZones(layout.HotZones);
		}
		void DrawSelection(SpreadsheetLayoutPage page) {
			PageSelectionLayoutItem selection = SpreadsheetProvider.ActiveView.SelectionLayout.GetPageSelection(page);
			if (selection == null)
				return;
			drawingContext.PushTransform(new TranslateTransform(-1, -1));
			try {
				selection.Update(page);
				selection.Draw(this);
			}
			finally {
				drawingContext.Pop();
			}
		}
		void DrawCutCopyRange(SpreadsheetLayoutPage page) {
			if (!Spreadsheet.IsEnabled)
				return;
			CutCopyRangeDashBorderLayoutItem copiedRange = SpreadsheetProvider.ActiveView.SelectionLayout.GetCutCopyRange(page);
			if (copiedRange == null || copiedRange.Count <= 0)
				return;
			copiedRange.Update(page);
			if (copiedRange.Bounds.IsEmpty) 
				return;
			drawingContext.PushTransform(new TranslateTransform(-1, -1));
			try {
				copiedRange.InflateBounds(-1);
				copiedRange.Draw(this);
			}
			finally {
				drawingContext.Pop();
			}
		}
		#region ISelectionPainter Members
		public void Draw(PageSelectionLayoutItem item) {
			IsMultiSelection = item.Layout.IsMultiSelection;
			CombinedGeometry combined = new CombinedGeometry();
			combined.Geometry1 = GetSelectionGeometry(item);
			combined.Geometry2 = GetClipGeometry(item);
			combined.GeometryCombineMode = GeometryCombineMode.Xor;
			drawingContext.DrawGeometry(SelectionBrush, null, combined);
			item.BorderItem.Draw(this);
		}
		Geometry GetSelectionGeometry(PageSelectionLayoutItem item) {
			GeometryGroup group = new GeometryGroup();
			group.FillRule = FillRule.Nonzero;
			foreach (RangeSelectionLayoutItem rangeItem in item.InnerItems) {
				RectangleGeometry bounds = new RectangleGeometry();
				bounds.Rect = rangeItem.Bounds.ToRect();
				group.Children.Add(bounds);
			}
			return group;
		}
		Geometry GetClipGeometry(PageSelectionLayoutItem item) {
			return new RectangleGeometry(item.Bounds.ToRect());
		}
		public void Draw(RangeSelectionLayoutItem item) {
		}
		public void Draw(RangeBorderSelectionLayoutItem item) {
			Rect borderBounds = item.Bounds.ToRect();
			Pen borderPen = GetSelectionBorderPen();
			if (IsMultiSelection) {
				borderBounds.Inflate(borderOffsetInPixels, borderOffsetInPixels);
				drawingContext.DrawRectangle(null, borderPen, borderBounds);
			}
			else {
				drawingContext.PushClip(GetSelectionBorderClip(item));
				if (item.IsLeftSideVisible)
					drawingContext.DrawLine(borderPen, borderBounds.TopLeft, borderBounds.BottomLeft);
				if (item.IsRightSideVisible)
					drawingContext.DrawLine(borderPen, borderBounds.TopRight, borderBounds.BottomRight);
				double cornerLeft = borderBounds.Left - borderCornerOffset;
				double cornerRight = borderBounds.Right + borderCornerOffset;
				double cornerTop = borderBounds.Top;
				double cornerBottom = borderBounds.Bottom;
				if (item.IsTopSideVisible)
					drawingContext.DrawLine(borderPen, new Point(cornerLeft, cornerTop), new Point(cornerRight, cornerTop));
				if (item.IsBottomSideVisible)
					drawingContext.DrawLine(borderPen, new Point(cornerLeft, cornerBottom), new Point(cornerRight, cornerBottom));
				drawingContext.Pop();
			}
			SelectionBorderBounds = borderBounds; 
			DrawHotZones(item.HotZones);
		}
		Geometry GetSelectionBorderClip(RangeBorderSelectionLayoutItem item) {
			Rect borderBounds = item.Bounds.ToRect();
			borderBounds.Inflate(largeBorderWidthInPixels, largeBorderWidthInPixels);
			Rect clipBounds = item.ClipBound.ToRect();
			CombinedGeometry combined = new CombinedGeometry();
			combined.Geometry1 = new RectangleGeometry(borderBounds);
			combined.Geometry2 = new RectangleGeometry(clipBounds);
			combined.GeometryCombineMode = GeometryCombineMode.Exclude;
			return combined;
		}
		Pen GetSelectionBorderPen() {
			Pen result = new Pen();
			result.Brush = SelectionBorderBrush;
			result.Thickness = IsMultiSelection ? smallBorderWidthInPixels : largeBorderWidthInPixels;
			return result;
		}
		public void Draw(PageDashSelectionLayoutItem item) {
			for (int i = 0; i < item.BorderItems.Count; i++)
				item.BorderItems[i].Draw(this);
		}
		public void Draw(RangeBorderDashSelectionLayoutItem item) {
			DrawDashRectangle(item.Bounds.ToRect());
		}
		public void Draw(CutCopyRangeDashBorderLayoutItem item) {
			DrawDashRectangle(item.Bounds.ToRect());
		}
		void DrawDashRectangle(Rect bounds) {
			Pen pen = GetSelectionBorderPen();
			pen.DashStyle = new DashStyle(dashPattern, 0d);
			drawingContext.DrawRectangle(null, pen, bounds);
		}
		public void Draw(RangeMailMergeLayoutItem item) {
		}
		public void Draw(PictureSelectionLayoutItem item) {
		}
		public void Draw(PrintRangeSelectionLayoutSubItem item) {
		}
		#endregion
		void DrawHotZones(HotZoneCollection hotZones) {
			if (hotZones.Count == 0)
				return;
			HotZonePainter painter = new HotZonePainter(drawingContext, SelectionBorderBrush, Owner.AutoFilterImageCache, AutoFilterBackgroundBrush);
			painter.DrawHotZones(hotZones);
		}
	}
	#endregion
	#region HotZonePainter
	public class HotZonePainter : IHotZoneVisitor {
		#region Fields
		static Brush HotZoneRotationGradientBrush = Brushes.Green;
		static Brush HotZoneGradientBrush = Brushes.SlateGray;
		static Pen HotZoneLinePen = new Pen(Brushes.Gray, 1);
		static Pen pivotExpandCollapseLinePen = new Pen(new SolidColorBrush(Color.FromRgb(75, 75, 75)), 1);
		const int fillHandleOffsetInPixels = 1;
		readonly DrawingContext drawingContext;
		readonly Brush selectionBorderBrush;
		readonly AutoFilterImageCache autoFilterImageCache;
		readonly Brush autoFilterBackgroundBrush;
		readonly Color dataValidationBorderColor;
		#endregion
		public HotZonePainter(DrawingContext drawingContext)
			: this(drawingContext, null) {
		}
		public HotZonePainter(DrawingContext drawingContext, Brush selectionBorderBrush) :
			this(drawingContext, selectionBorderBrush, null, null) {
		}
		public HotZonePainter(DrawingContext drawingContext, Brush selectionBorderBrush, AutoFilterImageCache autoFilterImageCache, Brush autoFilterBackgroundBrush) :
			this(drawingContext, selectionBorderBrush, autoFilterImageCache, autoFilterBackgroundBrush, Colors.Transparent) {
		}
		public HotZonePainter(DrawingContext drawingContext, Brush selectionBorderBrush, AutoFilterImageCache autoFilterImageCache, Brush autoFilterBackgroundBrush, Color dataValidationBorderColor) {
			Guard.ArgumentNotNull(drawingContext, "drawingContext");
			this.drawingContext = drawingContext;
			this.selectionBorderBrush = selectionBorderBrush;
			this.autoFilterImageCache = autoFilterImageCache;
			this.autoFilterBackgroundBrush = autoFilterBackgroundBrush;
			this.dataValidationBorderColor = dataValidationBorderColor;
		}
		protected internal void DrawHotZones(HotZoneCollection hotZones) {
			foreach (HotZone hotZone in hotZones) {
				DrawHotZone(hotZone);
			}
		}
		protected internal void DrawHotZone(HotZone zone) {
			zone.Visit(this);
		}
		#region IHotZoneVisitor Members
		private void DrawEllipticHotZone(Rect bounds, Brush fillBrush) {
			drawingContext.DrawEllipse(fillBrush, HotZoneLinePen, CalcCenter(bounds), bounds.Width / 2, bounds.Height / 2);
		}
		private void DrawRectangularHotZone(Rect bounds, Brush fillBrush) {
			drawingContext.DrawRectangle(fillBrush, HotZoneLinePen, bounds);
		}
		internal static Point CalcCenter(Rect bounds) {
			return RectangleUtils.CenterPoint(bounds.ToRectangle()).ToPoint();
		}
		public void Visit(DrawingObjectRotationHotZone hotZone) {
			Point start = RectangleUtils.CenterPoint(hotZone.Bounds).ToPoint();
			Point end = hotZone.LineEnd.ToPoint();
			drawingContext.DrawLine(HotZoneLinePen, start, end);
			DrawEllipticHotZone(hotZone.Bounds.ToRect(), HotZoneRotationGradientBrush);
		}
		public void Visit(IResizeHotZone hotZone) {
			Rect bounds = hotZone.Bounds.ToRect();
			if (hotZone.Type == ResizeHotZoneType.Ellipse)
				DrawEllipticHotZone(bounds, HotZoneGradientBrush);
			else {
				System.Diagnostics.Debug.Assert(hotZone.Type == ResizeHotZoneType.Rectangle);
				DrawRectangularHotZone(bounds, HotZoneGradientBrush);
			}
		}
		public void Visit(RangeDragHotZone hotZone) {
		}
		public void Visit(RangeResizeHotZone hotZone) {
			System.Diagnostics.Debug.Assert(selectionBorderBrush != null);
			Rect bounds = hotZone.Bounds.ToRect();
			bounds.Offset(fillHandleOffsetInPixels, fillHandleOffsetInPixels);
			drawingContext.DrawRectangle(selectionBorderBrush, null, bounds);
		}
		public void Visit(FormulaRangeDragHotZone hotZone) {
		}
		public void Visit(FormulaRangeResizeHotZone hotZone) {
		}
		public void Visit(IFilterHotZone hotZone) {
			int imageIndex = 0;
			Rect bounds = hotZone.BoundsHotZone.ToRect();
			if (hotZone is AutoFilterColumnHotZone)
				imageIndex = autoFilterImageCache.CalculateAutoFilterImageIndex(hotZone as AutoFilterColumnHotZone);
			else
				imageIndex = autoFilterImageCache.CalculatePivotTableFilterImageIndex(hotZone as PivotTableFilterHotZone);
			DrawAutoFilterImage(bounds, imageIndex);
		}
		public void Visit(DataValidationHotZone hotZone) {
			Rect bounds = hotZone.Bounds.ToRect();
			DrawAutoFilterImage(bounds, 0);
			DrawDataValidationBorder(bounds);
		}
		void DrawDataValidationBorder(Rect bounds) {
			bounds = new Rect(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
			bounds.Offset(-0.5, -0.5); 
			Brush brush = new SolidColorBrush(dataValidationBorderColor);
			brush.Freeze();
			Pen pen = new Pen(brush, 1);
			drawingContext.DrawRectangle(null, pen, bounds);
		}
		void DrawAutoFilterImage(Rect bounds, int imageIndex) {
			ImageSource imageSource = autoFilterImageCache.GetImageSource(imageIndex);
			if (imageSource == null)
				return;
			bounds.Offset(-1, -1);
			drawingContext.DrawRectangle(autoFilterBackgroundBrush, null, bounds);
			double imageOffsetX = (bounds.Width - imageSource.Width) / 2;
			double imageOffsetY = (bounds.Height - imageSource.Height) / 2;
			Rect imageBounds = new Rect(bounds.X + imageOffsetX, bounds.Y + imageOffsetY, imageSource.Width, imageSource.Height);
			drawingContext.DrawImage(imageSource, imageBounds);
		}
		void IHotZoneVisitor.Visit(CommentDragHotZone hotZone) {
		}
		void IHotZoneVisitor.Visit(PivotTableExpandCollapseHotZone hotZone) {
			Pen borderPen = new Pen(new SolidColorBrush(dataValidationBorderColor), 1);
			borderPen.Freeze();
			Rect bounds = hotZone.Bounds.ToRect();
			Rect borderBounds = new Rect(bounds.Left + 1, bounds.Top + 1, bounds.Width - 1, bounds.Height - 1);
			drawingContext.DrawRectangle(autoFilterBackgroundBrush, null, bounds);
			drawingContext.DrawRectangle(null, borderPen, borderBounds);
			double horizontalY = bounds.Y + bounds.Height / 2;
			drawingContext.DrawLine(pivotExpandCollapseLinePen, new Point(bounds.Left + 2, horizontalY), new Point(bounds.Right - 2, horizontalY));
			if (hotZone.IsCollapsed) {
				double verticalX = bounds.X + bounds.Width / 2;
				drawingContext.DrawLine(pivotExpandCollapseLinePen, new Point(verticalX, bounds.Top + 2), new Point(verticalX, bounds.Bottom - 2));
			}
		}
		#endregion
	}
	#endregion
}

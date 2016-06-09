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

using DevExpress.Utils;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using LayoutPage = DevExpress.XtraSpreadsheet.Layout.Page;
using DrawingColor = System.Drawing.Color;
using DevExpress.Office.Model;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region WorksheetLineControl
	public class WorksheetLineControl : WorksheetPaintControl {
		#region Fields
		const int frozenPanesSeparatorThickness = 1;
		public static readonly DependencyProperty GridLinesColorProperty;
		public static readonly DependencyProperty FrozenSeparatorBrushProperty;
		#endregion
		static WorksheetLineControl() {
			FrozenSeparatorBrushProperty =
				DependencyProperty.Register("FrozenSeparatorBrush", typeof(Brush), typeof(WorksheetLineControl),
				new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
			GridLinesColorProperty =
				DependencyProperty.Register("GridLinesColor", typeof(Color), typeof(WorksheetLineControl),
				new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender));
		}
		public WorksheetLineControl() {
			this.DefaultStyleKey = typeof(WorksheetLineControl);
			RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
		}
		#region Properties
		public Brush FrozenSeparatorBrush {
			get { return (Brush)GetValue(WorksheetLineControl.FrozenSeparatorBrushProperty); }
			set { SetValue(WorksheetLineControl.FrozenSeparatorBrushProperty, value); }
		}
		public Color GridLinesColor {
			get { return (Color)GetValue(WorksheetLineControl.GridLinesColorProperty); }
			set { SetValue(WorksheetLineControl.GridLinesColorProperty, value); }
		}
		float DpiX { get { return DocumentModel.DpiX; } }
		float DpiY { get { return DocumentModel.DpiY; } }
		#endregion
		protected override void OnRender(System.Windows.Media.DrawingContext dc) {
			base.OnRender(dc);
			if (LayoutInfo == null)
				return;
			float zoomFactor = 1 / (float)(SpreadsheetProvider.ScaleFactor * DocumentModel.Dpi / 96.0);
			DrawLines(dc, zoomFactor);
			DrawFrozenPanelSeparators(dc, LayoutInfo.Pages[0], zoomFactor);
		}
		void DrawLines(System.Windows.Media.DrawingContext dc, float zoomFactor) {
			BorderLinePainter painter = new BorderLinePainter(dc, LayoutInfo, zoomFactor);
			foreach (LayoutPage page in LayoutInfo.Pages) {
				dc.PushClip(new RectangleGeometry() { Rect = new Rect(page.Bounds.X, page.Bounds.Y, page.Bounds.Width, page.Bounds.Height) });
				DrawLines(painter, page, page.HorizontalGridBorders);
				DrawLines(painter, page, page.VerticalGridBorders);
				DrawLines(painter, page, page.VerticalBorders);
				DrawLines(painter, page, page.HorizontalBorders);
				dc.Pop();
			}
		}
		void DrawLines(BorderLinePainter painter, LayoutPage page, List<PageBorderCollection> borders) {
			for (int i = 0; i < borders.Count; i++)
				DrawLinesCore(painter, page, borders[i]);
		}
		void DrawLinesCore(BorderLinePainter painter, LayoutPage page, PageBorderCollection lines) {
			foreach (BorderLineBox box in lines.Boxes) {
				DrawLine(painter, box, lines.GetBounds(page, box).ToRect());
			}
		}
		void DrawLine(BorderLinePainter painter, BorderLineBox box, Rect bounds) {
			DocumentModel documentModel = LayoutInfo.DocumentModel;
			ColorModelInfo borderColorInfo = documentModel.Cache.ColorModelInfoCache[box.ColorIndex];
			DrawingColor drawingColor = borderColorInfo.ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors);
			if (DXColor.IsEmpty(drawingColor))
				drawingColor = DXColor.Black;
			Color borderColor = drawingColor.ToWpfColor();
			if (bounds.Width < bounds.Height)
				DrawVerticalBorder(painter, bounds, box.LineStyle, borderColor);
			else
				DrawHorizontalBorder(painter, bounds, box.LineStyle, borderColor);
		}
		void DrawVerticalBorder(BorderLinePainter painter, Rect bounds, XlBorderLineStyle lineStyle, Color borderColor) {
			painter.DrawLineByStyle(bounds.TopLeft, bounds.BottomLeft, lineStyle, borderColor);
		}
		void DrawHorizontalBorder(BorderLinePainter painter, Rect bounds, XlBorderLineStyle lineStyle, Color borderColor) {
			painter.DrawLineByStyle(bounds.TopLeft, bounds.TopRight, lineStyle, borderColor);
		}
		void DrawFrozenPanelSeparators(DrawingContext dc, LayoutPage page, float zoomFactor) {
			Pen separatorPen = new Pen(GetSeparatorPenBrush(), GetSeparatorThickness(zoomFactor));
			DrawFrozenPanelSeparatorsCore(dc, separatorPen, page, zoomFactor);
		}
		Brush GetSeparatorPenBrush() {
			return FrozenSeparatorBrush;
		}
		float GetSeparatorThickness(float zoomFactor) {
			float thickness = LayoutInfo.UnitConverter.PixelsToLayoutUnitsF(frozenPanesSeparatorThickness, DocumentModel.Dpi);
			return thickness * zoomFactor;
		}
		void DrawFrozenPanelSeparatorsCore(DrawingContext dc, Pen separatorPen, LayoutPage page, float zoomFactor) {
			HeaderPage headerPage = LayoutInfo.HeaderPage;
			if (headerPage == null)
				return;
			Worksheet sheet = page.Sheet;
			if (sheet.IsOnlyColumnsFrozen())
				DrawVerticalSeparator(dc, separatorPen, page, headerPage, zoomFactor);
			else if (sheet.IsOnlyRowsFrozen())
				DrawHorizontalSeparator(dc, separatorPen, page, headerPage, zoomFactor);
			else {
				DrawVerticalSeparator(dc, separatorPen, page, headerPage, zoomFactor);
				DrawHorizontalSeparator(dc, separatorPen, page, headerPage, zoomFactor);
			}
		}
		void DrawHorizontalSeparator(DrawingContext dc, Pen separatorPen, LayoutPage page, HeaderPage headerPage, float zoomFactor) {
			float x = 0;
			float y = CalculateVerticalFrozenSeparatorPosition(page);
			float length = (float)ActualWidth * zoomFactor;
			dc.DrawLine(separatorPen, new Point(x, y), new Point(x + length, y));
		}
		int CalculateVerticalFrozenSeparatorPosition(LayoutPage topLeftPage) {
			return topLeftPage.Bounds.Bottom;
		}
		void DrawVerticalSeparator(DrawingContext dc, Pen separatorPen, LayoutPage page, HeaderPage headerPage, float zoomFactor) {
			float x = CalculateHorizontalFrozenSeparatorPosition(page);
			float y = 0;
			float length = (float)ActualHeight * zoomFactor;
			dc.DrawLine(separatorPen, new Point(x, y), new Point(x, y + length));
		}
		int CalculateHorizontalFrozenSeparatorPosition(LayoutPage topLeftPage) {
			return topLeftPage.Bounds.Right;
		}
	}
	#endregion
	#region LinePatternProvider
	public static class LinePatternProvider {
		static Dictionary<XlBorderLineStyle, float[]> patterns;
		static LinePatternProvider() {
			PopulatePatterns();
		}
		public static float[] GetPattern(XlBorderLineStyle style) {
			return patterns.ContainsKey(style) ? patterns[style] : new float[] { 1 };
		}
		private static void PopulatePatterns() {
			patterns = new Dictionary<XlBorderLineStyle, float[]>();
			patterns.Add(XlBorderLineStyle.Dashed, CreateDashPattern());
			patterns.Add(XlBorderLineStyle.Dotted, CreateDottedPattern());
			patterns.Add(XlBorderLineStyle.Hair, CreateHairPattern());
			patterns.Add(XlBorderLineStyle.MediumDashed, CreateMediumDashedPattern());
			patterns.Add(XlBorderLineStyle.DashDot, CreateDashDotPattern());
			patterns.Add(XlBorderLineStyle.MediumDashDot, CreateMediumDashDotPattern());
			patterns.Add(XlBorderLineStyle.MediumDashDotDot, CreateMediumDashDotDotPattern());
			patterns.Add(XlBorderLineStyle.SlantDashDot, CreateDashDotPattern());
			patterns.Add(XlBorderLineStyle.DashDotDot, CreateDashDotDotPattern());
		}
		private static float[] CreateDashPattern() { return new float[] { 2f, 2f }; }
		private static float[] CreateDottedPattern() { return new float[] { 1f, 3f }; }
		private static float[] CreateHairPattern() { return new float[] { 0f, 2f, }; }
		private static float[] CreateMediumDashedPattern() { return new float[] { 7f, 5f }; }
		private static float[] CreateDashDotPattern() { return new float[] { 7f, 4f, 2f, 4f }; }
		private static float[] CreateDashDotDotPattern() { return new float[] { 7f, 4f, 2f, 4f, 2f, 4f }; }
		private static float[] CreateMediumDashDotPattern() { return new float[] { 6f, 5f, 1f, 5f }; }
		private static float[] CreateMediumDashDotDotPattern() { return new float[] { 6f, 5f, 1f, 5f, 1f, 5f }; }
	}
	#endregion
	#region BorderLinePainter
	public class BorderLinePainter {
		#region Fields
		readonly DrawingContext drawingContext;
		readonly DocumentLayout layout;
		readonly float zoomFactor;
		#endregion
		public BorderLinePainter(DrawingContext drawingContext, DocumentLayout layout, float zoomFactor) {
			Guard.ArgumentNotNull(drawingContext, "drawingContext");
			Guard.ArgumentNotNull(layout, "layout");
			this.drawingContext = drawingContext;
			this.layout = layout;
			this.zoomFactor = zoomFactor;
		}
		public void DrawLineByStyle(Point start, Point end, XlBorderLineStyle style, Color color) {
			if (style == XlBorderLineStyle.None)
				return;
			Pen linePen = GetPen(color, style);
			float[] pattern = LinePatternProvider.GetPattern(style);
			if (pattern.Length > 1)
				DrawPatternLine(linePen, start, end, pattern, (float)linePen.Thickness);
			else
				DrawSolidLine(linePen, start, end, style == XlBorderLineStyle.Double);
		}
		Pen GetPen(Color color, XlBorderLineStyle style) {
			Pen result = new Pen();
			result.Brush = new SolidColorBrush(color);
			result.Thickness = layout.LineThicknessTable[style] * zoomFactor;
			return result;
		}
		void DrawPatternLine(Pen linePen, Point start, Point end, float[] pattern, float thickness) {
			linePen.DashStyle = new DashStyle(MakeFixedWidthPattern(pattern, thickness), 0);
			DrawLineCore(linePen, start, end);
		}
		double[] MakeFixedWidthPattern(float[] pattern, float thickness) {
			if (pattern == null)
				return new double[0];
			int count = pattern.Length;
			double[] result = new double[count];
			for (int i = 0; i < count; i++)
				result[i] = pattern[i] / thickness;
			return result;
		}
		void DrawSolidLine(Pen linePen, Point start, Point end, bool isDoubleLine) {
			if (isDoubleLine)
				DrawDoubleSolidLine(linePen, start, end);
			else
				DrawLineCore(linePen, start, end);
		}
		void DrawDoubleSolidLine(Pen linePen, Point start, Point end) {
			double thickness = linePen.Thickness;
			thickness /= 3.0f;
			float pixelsToUnits = PixelsToUnits(1, DocumentModel.DpiY);
			if (thickness < pixelsToUnits)
				thickness = pixelsToUnits;
			linePen.Thickness = thickness;
			if (start.X == end.X)
				DrawVerticalDoubleSolidLine(linePen, start, end);
			else if (start.Y == end.Y)
				DrawHorizontalDoubleSolidLine(linePen, start, end);
			else
				DrawDiagonalDoubleSolidLine(linePen, start, end);
		}
		void DrawVerticalDoubleSolidLine(Pen linePen, Point start, Point end) {
			double offset = linePen.Thickness * 2;
			DrawLineCore(linePen, start, end);
			DrawLineCore(linePen, new Point(start.X + offset, start.Y), new Point(end.X + offset, end.Y));
		}
		void DrawHorizontalDoubleSolidLine(Pen linePen, Point start, Point end) {
			double offset = linePen.Thickness * 2;
			DrawLineCore(linePen, start, end);
			DrawLineCore(linePen, new Point(start.X, start.Y + offset), new Point(end.X, end.Y + offset));
		}
		void DrawDiagonalDoubleSolidLine(Pen linePen, Point start, Point end) {
			double dx = start.X - end.X;
			double dy = start.Y - end.Y;
			double length = Math.Sqrt(dx * dx + dy * dy);
			if (length == 0)
				return;
			double cosA = dx / length;
			double sinA = dy / length;
			double deltaX = linePen.Thickness * sinA;
			double deltaY = linePen.Thickness * cosA;
			Point start1 = new Point(Math.Round(start.X - deltaX), Math.Round(start.Y + deltaY));
			Point end1 = new Point(Math.Round(end.X - deltaX), Math.Round(end.Y + deltaY));
			DrawLineCore(linePen, start1, end1);
			Point start2 = new Point(Math.Round(start.X + deltaX), Math.Round(start.Y - deltaY));
			Point end2 = new Point(Math.Round(end.X + deltaX), Math.Round(end.Y - deltaY));
			DrawLineCore(linePen, start2, end2);
		}
		float PixelsToUnits(float val, float dpi) {
			return layout.UnitConverter.PixelsToLayoutUnitsF(val, dpi);
		}
		void DrawLineCore(Pen linePen, Point start, Point end) {
			drawingContext.DrawLine(linePen, start, end);
		}
	}
	#endregion
}

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using Drawing = System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class WorksheetBackgroundControl : WorksheetPaintControl {
		public WorksheetBackgroundControl() {
			DefaultStyleKey = typeof(WorksheetBackgroundControl);
		}
		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			if (LayoutInfo == null)
				return;
			var pages = LayoutInfo.Pages;
			if (pages == null || pages.Count == 0)
				return;
			foreach (DevExpress.XtraSpreadsheet.Layout.Page page in pages) {
				DrawPage(dc, page);
			}
		}
		private void DrawPage(DrawingContext dc, XtraSpreadsheet.Layout.Page page) {
			dc.PushClip(new RectangleGeometry() { Rect = new Rect(page.Bounds.X, page.Bounds.Y, page.Bounds.Width, page.Bounds.Height) });
			for (int i = 0; i < page.Boxes.Count; i++) {
				DrawCellBackground(dc, page, page.Boxes[i]);
			}
			for (int i = 0; i < page.ComplexBoxes.Count; i++) {
				DrawCellBackground(dc, page, page.ComplexBoxes[i]);
			}
			dc.Pop();
		}
		void DrawCellBackground(DrawingContext dc, DevExpress.XtraSpreadsheet.Layout.Page page, ICellTextBox box) {
			CellBackgroundDisplayFormat displayFormat = box.CalculateBackgroundDisplayFormat(page, page.DocumentLayout.DocumentModel);
			if (displayFormat.Bounds == System.Drawing.Rectangle.Empty)
				return;
			Rect fillBounds = displayFormat.Bounds.ToRect();
			if (fillBounds == Rect.Empty)
				return;
			if (displayFormat.GradientFill != null)
				dc.DrawRectangle(CreateGradientBrush(displayFormat), null, fillBounds);
			else
				DrawCellBackgroundFromPatternFill(dc, displayFormat);
		}
		void DrawCellBackgroundFromPatternFill(DrawingContext dc, CellBackgroundDisplayFormat displayFormat) {
			Rect fillBounds = displayFormat.Bounds.ToRect();
			if (!DXColor.IsTransparentOrEmpty(displayFormat.BackColor))
				dc.DrawRectangle(CreateBrush(displayFormat.BackColor), null, fillBounds);
			if (displayFormat.ShouldUseForeColor) {
				Brush brush = CreatePatternBrush(displayFormat.PatternType, displayFormat.ForeColor.ToWpfColor());
				dc.DrawRectangle(brush, null, fillBounds);
			}
		}
		Brush CreateBrush(Drawing.Color color) {
			return new SolidColorBrush(color.ToWpfColor());
		}
		Brush CreateGradientBrush(CellBackgroundDisplayFormat displayFormat) {
			if (displayFormat == null)
				return Brushes.Transparent;
			IActualGradientFillInfo gradientFill = displayFormat.GradientFill;
			if (gradientFill == null)
				return Brushes.Transparent;
			IActualGradientStopCollection stops = gradientFill.GradientStops;
			if (stops.Count <= 0)
				return Brushes.Transparent;
			if (gradientFill.Type == ModelGradientFillType.Path) {
				return Brushes.Transparent;
			}
			else {
				System.Windows.Media.GradientStopCollection brushStops = new System.Windows.Media.GradientStopCollection();
				int count = stops.Count;
				for (int i = 0; i < count; i++)
					brushStops.Add(new GradientStop(stops[i].Color.ToWpfColor(), stops[i].Position));
				LinearGradientBrush linearBrush = new LinearGradientBrush(brushStops);
				Point[] points = CalculateLinearBrushPoints(displayFormat.Bounds, gradientFill.Degree);
				linearBrush.MappingMode = BrushMappingMode.Absolute;
				linearBrush.StartPoint = points[0];
				linearBrush.EndPoint = points[1];
				linearBrush.Freeze();
				return linearBrush;
			}
		}
		Point[] CalculateLinearBrushPoints(System.Drawing.Rectangle bounds, double degree) {
			double angle = Math.PI * degree / 180.0;
			bool swap = degree >= 180; 
			angle = Math.Atan(Math.Tan(angle) * bounds.Width / (double)bounds.Height);
			Point center = new Point((bounds.Left + bounds.Right) / 2.0, (bounds.Top + bounds.Bottom) / 2.0);
			double length = Math.Sqrt(bounds.Width * bounds.Width + bounds.Height * bounds.Height);
			double cosA = bounds.Width / length;
			double sinA = bounds.Height / length;
			if (angle > -Math.PI / 2 && angle < 0)
				swap = !swap;
			double distance1 = Math.Abs(length / 2.0 * (cosA * Math.Cos(angle) + sinA * Math.Sin(angle))); 
			double distance2 = Math.Abs(length / 2.0 * (cosA * Math.Cos(angle) - sinA * Math.Sin(angle))); 
			double distance = Math.Max(distance1, distance2);
			double dx = distance * Math.Cos(angle);
			double dy = distance * Math.Sin(angle);
			Point[] result = new Point[2];
			if (swap) {
				result[0] = new Point(center.X + dx, center.Y + dy);
				result[1] = new Point(center.X - dx, center.Y - dy);
			}
			else {
				result[1] = new Point(center.X + dx, center.Y + dy);
				result[0] = new Point(center.X - dx, center.Y - dy);
			}
			return result;
		}
		Brush CreatePatternBrush(XlPatternType patternType, Color foreColor) {
			return PatternFillProvider.GetPatternFillBrush(patternType, foreColor);
		}
	}
	#region PatternFillProvider
	public static class PatternFillProvider {
		#region TileInfo
		struct TileInfo {
			int length;
			bool containsLine;
			internal TileInfo(int length, bool containsLine) {
				this.length = length;
				this.containsLine = containsLine;
			}
			internal int Length { get { return length; } set { length = value; } }
			internal bool ContainsLine { get { return containsLine; } set { containsLine = value; } }
		}
		#endregion
		public static Brush GetPatternFillBrush(XlPatternType type, Color foreColor) {
			DrawingBrush result = new DrawingBrush();
			GeometryGroup group = new GeometryGroup();
			TileInfo tileInfo = PopulateTile(type, group);
			GeometryDrawing drawing = CreateDrawing(foreColor, group, tileInfo.ContainsLine);
			DrawingGroup drawingGroup = new DrawingGroup();
			drawingGroup.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
			drawingGroup.Children.Add(drawing);
			result.Drawing = drawingGroup;
			result.Stretch = Stretch.None;
			result.ViewportUnits = BrushMappingMode.Absolute;
			result.Viewport = new Rect(0, 0, tileInfo.Length, tileInfo.Length);
			result.TileMode = TileMode.Tile;
			result.Freeze();
			return result;
		}
		static TileInfo PopulateTile(XlPatternType type, GeometryGroup group) {
			switch (type) {
				case XlPatternType.Gray0625:
					return PopulateTile_HatchPercent10(group);
				case XlPatternType.Gray125:
					return PopulateTile_HatchPercent20(group);
				case XlPatternType.LightGray:
					return PopulateTile_Percent25(group);
				case XlPatternType.LightTrellis:
					return PopulateTile_Percent30(group);
				case XlPatternType.MediumGray:
					return PopulateTile_Percent50(group);
				case XlPatternType.DarkGray:
					return PopulateTile_Percent70(group);
				case XlPatternType.LightDown:
					return PopulateTile_LightDownwardDiagonal(group);
				case XlPatternType.LightUp:
					return PopulateTile_LightUpwardDiagonal(group);
				case XlPatternType.DarkDown:
					return PopulateTile_DarkDownwardDiagonal(group);
				case XlPatternType.DarkUp:
					return PopulateTile_DarkUpwardDiagonal(group);
				case XlPatternType.LightVertical:
					return PopulateTile_LightVertical(group);
				case XlPatternType.LightHorizontal:
					return PopulateTile_LightHorizontal(group);
				case XlPatternType.DarkVertical:
					return PopulateTile_DarkVertical(group);
				case XlPatternType.DarkHorizontal:
					return PopulateTile_DarkHorizontal(group);
				case XlPatternType.DarkTrellis:
					return PopulateTile_Trellis(group);
				case XlPatternType.LightGrid:
					return PopulateTile_SmallGrid(group);
				case XlPatternType.DarkGrid:
					return PopulateTile_SmallCheckerBoard(group);
			}
			Exceptions.ThrowInternalException();
			return new TileInfo();
		}
		static TileInfo PopulateTile_HatchPercent10(GeometryGroup group) {
			group.Children.Add(CreatePoint(0, 2));
			group.Children.Add(CreatePoint(4, 0));
			group.Children.Add(CreatePoint(0, 6));
			group.Children.Add(CreatePoint(4, 4));
			return new TileInfo(8, true);
		}
		static TileInfo PopulateTile_HatchPercent20(GeometryGroup group) {
			group.Children.Add(CreatePoint(0, 0));
			group.Children.Add(CreatePoint(2, 2));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_Percent25(GeometryGroup group) {
			group.Children.Add(CreatePoint(0, 0));
			group.Children.Add(CreatePoint(0, 2));
			group.Children.Add(CreatePoint(2, 1));
			group.Children.Add(CreatePoint(2, 3));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_Percent30(GeometryGroup group) {
			group.Children.Add(CreatePoint(0, 0));
			group.Children.Add(CreatePoint(1, 1));
			group.Children.Add(CreatePoint(2, 2));
			group.Children.Add(CreatePoint(3, 3));
			group.Children.Add(CreatePoint(2, 0));
			group.Children.Add(CreatePoint(0, 2));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_Percent50(GeometryGroup group) {
			group.Children.Add(CreatePoint(0, 0));
			group.Children.Add(CreatePoint(1, 1));
			return new TileInfo(2, true);
		}
		static TileInfo PopulateTile_Percent70(GeometryGroup group) {
			group.Children.Add(CreatePoint(1, 0));
			group.Children.Add(CreatePoint(2, 0));
			group.Children.Add(CreatePoint(3, 0));
			group.Children.Add(CreatePoint(0, 1));
			group.Children.Add(CreatePoint(1, 1));
			group.Children.Add(CreatePoint(3, 1));
			group.Children.Add(CreatePoint(1, 2));
			group.Children.Add(CreatePoint(2, 2));
			group.Children.Add(CreatePoint(3, 2));
			group.Children.Add(CreatePoint(0, 3));
			group.Children.Add(CreatePoint(1, 3));
			group.Children.Add(CreatePoint(3, 3));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_LightDownwardDiagonal(GeometryGroup group) {
			group.Children.Add(CreatePoint(0, 0));
			group.Children.Add(CreatePoint(1, 1));
			group.Children.Add(CreatePoint(2, 2));
			group.Children.Add(CreatePoint(3, 3));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_LightUpwardDiagonal(GeometryGroup group) {
			group.Children.Add(CreatePoint(0, 3));
			group.Children.Add(CreatePoint(1, 2));
			group.Children.Add(CreatePoint(2, 1));
			group.Children.Add(CreatePoint(3, 0));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_DarkDownwardDiagonal(GeometryGroup group) {
			group.Children.Add(CreatePoint(0, 3));
			group.Children.Add(CreatePoint(0, 0));
			group.Children.Add(CreatePoint(1, 1));
			group.Children.Add(CreatePoint(2, 2));
			group.Children.Add(CreatePoint(3, 3));
			group.Children.Add(CreatePoint(1, 0));
			group.Children.Add(CreatePoint(2, 1));
			group.Children.Add(CreatePoint(3, 2));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_DarkUpwardDiagonal(GeometryGroup group) {
			group.Children.Add(CreatePoint(0, 0));
			group.Children.Add(CreatePoint(0, 1));
			group.Children.Add(CreatePoint(1, 0));
			group.Children.Add(CreatePoint(1, 3));
			group.Children.Add(CreatePoint(2, 2));
			group.Children.Add(CreatePoint(3, 1));
			group.Children.Add(CreatePoint(2, 3));
			group.Children.Add(CreatePoint(3, 2));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_LightVertical(GeometryGroup group) {
			group.Children.Add(CreateLine(0, 0, 0, 3));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_LightHorizontal(GeometryGroup group) {
			group.Children.Add(CreateLine(0, 0, 3, 0));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_DarkVertical(GeometryGroup group) {
			group.Children.Add(CreateLine(0, 0, 0, 3));
			group.Children.Add(CreateLine(1, 0, 1, 3));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_DarkHorizontal(GeometryGroup group) {
			group.Children.Add(CreateLine(0, 0, 3, 0));
			group.Children.Add(CreateLine(0, 1, 3, 1));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_Trellis(GeometryGroup group) {
			group.Children.Add(CreateLine(0, 0, 3, 0));
			group.Children.Add(CreateLine(1, 1, 2, 1));
			group.Children.Add(CreateLine(0, 2, 3, 2));
			group.Children.Add(CreatePoint(0, 3));
			group.Children.Add(CreatePoint(3, 3));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_SmallGrid(GeometryGroup group) {
			group.Children.Add(CreateLine(0, 0, 3, 0));
			group.Children.Add(CreateLine(0, 0, 0, 3));
			return new TileInfo(4, true);
		}
		static TileInfo PopulateTile_SmallCheckerBoard(GeometryGroup group) {
			group.Children.Add(CreateRectangle(0, 0, 2, 2));
			group.Children.Add(CreateRectangle(2, 2, 2, 2));
			return new TileInfo(4, false);
		}
		static LineGeometry CreatePoint(int x, int y) {
			return new LineGeometry(new Point(x, y), new Point(x + 1, y + 1));
		}
		static LineGeometry CreateLine(int x1, int y1, int x2, int y2) {
			if (x1 != x2)
				x2 = x2 + 1;
			if (y1 != y2)
				y2 = y2 + 1;
			return new LineGeometry(new Point(x1, y1), new Point(x2, y2)); ;
		}
		static RectangleGeometry CreateRectangle(int x, int y, int width, int height) {
			Rect rect = new Rect(x, y, width, height);
			return new RectangleGeometry(rect);
		}
		static GeometryDrawing CreateDrawing(Color foreColor, GeometryGroup group, bool isNeedDrawLine) {
			SolidColorBrush brush = new SolidColorBrush(foreColor);
			brush.Freeze();
			if (!isNeedDrawLine)
				return new GeometryDrawing(brush, null, group);
			Pen pen = new Pen(brush, 1);
			pen.Freeze();
			return new GeometryDrawing(null, pen, group);
		}
	}
	#endregion
}

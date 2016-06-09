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

using DevExpress.Office.Layout;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using System;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	#region WorksheetIndicatorControl
	public class WorksheetIndicatorControl : WorksheetPaintControl {
		#region Fields
		public static readonly DependencyProperty CommentIndicatorBrushProperty;
		public static readonly DependencyProperty NumberIndicatorBrushProperty;
		public static readonly DependencyProperty IndicatorWidthProperty;
		#endregion
		static WorksheetIndicatorControl() {
			Type ownerType = typeof(WorksheetIndicatorControl);
			CommentIndicatorBrushProperty = DependencyProperty.Register("CommentIndicatorBrush", typeof(Brush), ownerType);
			NumberIndicatorBrushProperty = DependencyProperty.Register("NumberIndicatorBrush", typeof(Brush), ownerType);
			IndicatorWidthProperty = DependencyProperty.Register("IndicatorWidth", typeof(double), ownerType);
		}
		public WorksheetIndicatorControl() {
			DefaultStyleKey = typeof(WorksheetIndicatorControl);
			RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
		}
		#region Properties
		protected internal DocumentLayoutUnitConverter LayoutConverter { get { return Spreadsheet.DocumentModel.LayoutUnitConverter; } }
		public Brush CommentIndicatorBrush {
			get { return (Brush)GetValue(CommentIndicatorBrushProperty); }
			set { SetValue(CommentIndicatorBrushProperty, value); }
		}
		public Brush NumberIndicatorBrush {
			get { return (Brush)GetValue(NumberIndicatorBrushProperty); }
			set { SetValue(NumberIndicatorBrushProperty, value); }
		}
		public double IndicatorWidth {
			get { return (double)GetValue(IndicatorWidthProperty); }
			set { SetValue(IndicatorWidthProperty, value); }
		}
		#endregion
		protected override void OnRender(DrawingContext dc) {
			if (Owner == null || LayoutInfo == null)
				return;
			foreach (Page page in LayoutInfo.Pages) {
				DrawIndicators(dc, page);
			}
		}
		void DrawIndicators(DrawingContext dc, Page page) {
			foreach (IndicatorBox box in page.IndicatorBoxes) {
				DrawIndicator(dc, box);
			}
		}
		void DrawIndicator(DrawingContext dc, IndicatorBox box) {
			int widthInLayoutUnits = LayoutConverter.PixelsToLayoutUnits((int)IndicatorWidth, DocumentModel.Dpi);
			int offsetInLayoutUnits = LayoutConverter.PixelsToLayoutUnits(1, DocumentModel.Dpi);
			IndicatorType type = box.Type;
			Rect bounds = box.ClipBounds.ToRect();
			if (type == IndicatorType.Comment)
				DrawCommentIndicator(dc, bounds, widthInLayoutUnits, offsetInLayoutUnits);
			else if (type == IndicatorType.NumberFormatting)
				DrawNumberFormattingIndicator(dc, bounds, widthInLayoutUnits, offsetInLayoutUnits);
		}
		void DrawCommentIndicator(DrawingContext dc, Rect bounds, int width, int offset) {
			Point topRight = new Point(bounds.Right - offset, bounds.Top - offset);
			Point topLeft = new Point(topRight.X - width, topRight.Y);
			Point bottomRight = new Point(topRight.X, topRight.Y + width);
			Point[] points = new Point[3] { topLeft, topRight, bottomRight };
			DrawTriangle(dc, CommentIndicatorBrush, points);
		}
		void DrawNumberFormattingIndicator(DrawingContext dc, Rect bounds, int width, int offset) {
			Point topLeft = new Point(bounds.Left - offset, bounds.Top - offset);
			Point topRight = new Point(topLeft.X + width, topLeft.Y);
			Point bottomLeft = new Point(topLeft.X, topLeft.Y + width);
			Point[] points = new Point[3] { topLeft, topRight, bottomLeft };
			DrawTriangle(dc, NumberIndicatorBrush, points);
		}
		void DrawTriangle(DrawingContext dc, Brush brush, Point[] points) {
			PolyLineSegment segment = new PolyLineSegment(points, false);
			segment.Freeze();
			PathFigure figure = new PathFigure();
			figure.StartPoint = points[0];
			figure.Segments.Add(segment);
			figure.Freeze();
			PathGeometry geometry = new PathGeometry();
			geometry.Figures.Add(figure);
			geometry.Freeze();
			dc.DrawGeometry(brush, null, geometry);
		}
	}
	#endregion
}

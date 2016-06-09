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
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class TickmarkLineBase {
		readonly DiagramPoint left;
		readonly DiagramPoint right;
		protected DiagramPoint Left { get { return left; } }
		protected DiagramPoint Right { get { return right; } }
		protected TickmarkLineBase(DiagramPoint left, DiagramPoint right) {
			this.left = left;
			this.right = right;
		}
		public abstract GraphicsPath CreateGraphicsPath();
		public abstract void Render(IRenderer renderer, Color color);
	}
	public class TickmarkLineAsRectangle : TickmarkLineBase {
		public TickmarkLineAsRectangle(DiagramPoint left, DiagramPoint right) : base(left, right) {
		}
		ZPlaneRectangle CreateRectangle() {
			return ZPlaneRectangle.MakeRectangle(Left, Right);
		}
		public override GraphicsPath CreateGraphicsPath() {
			GraphicsPath path = new GraphicsPath();
			try {
				path.AddRectangle((Rectangle)CreateRectangle());
				return path;
			}
			catch {
				path.Dispose();
				return null;
			}
		}
		public override void Render(IRenderer renderer, Color color) {
			renderer.FillRectangle((RectangleF)CreateRectangle(), color);
		}
	}
	public class TickmarkLineAsLine : TickmarkLineBase {
		readonly int thickness;
		public TickmarkLineAsLine(DiagramPoint left, DiagramPoint right, int thickness) : base(left, right) {
			this.thickness = thickness;
		}			
		public override GraphicsPath CreateGraphicsPath() {
			GraphicsPath path = new GraphicsPath();
			try {
				path.AddLine((PointF)Left, (PointF)Right);
				using(Pen pen = new Pen(Color.Empty, thickness))
					path.Widen(pen);
				return path;
			}
			catch {
				path.Dispose();
				return null;
			}
		}
		public override void Render(IRenderer renderer, Color color) {
			renderer.DrawLine((Point)Left, (Point)Right, color, thickness);
		}
	}
	public class RadarAxisYTickmarksViewData {
		static GraphicsPath CreateGraphicsPath(TickmarkLineBase[] lines) {
			if(lines.Length == 0)
				return null;
			GraphicsPath path = new GraphicsPath();
			try {
				foreach(TickmarkLineBase line in lines)
					using(GraphicsPath subPath = line.CreateGraphicsPath())
						path.AddPath(subPath, false);
				return path;
			}
			catch {
				path.Dispose();
				return null;
			}
		}
		readonly RadarAxisYMapping mapping;
		readonly RadarAxisYViewData viewData;
		readonly Rectangle bounds;
		readonly double extent;
		readonly bool isVerticalOrHorizontal;
		TickmarkLineBase[] tickmarksLines = new TickmarkLineBase[0];
		TickmarkLineBase[] minorTickmarksLines = new TickmarkLineBase[0];
		public RadarAxisY Axis { get { return viewData.Axis; } }
		public Rectangle Bounds { get { return bounds; } }
		public TickmarkLineBase[] TickmarkLines { get { return tickmarksLines; } }
		public TickmarkLineBase[] MinorTickmarkLines { get { return minorTickmarksLines; } }
		public double Extent { get { return extent; } }
		RadarTickmarksY Tickmarks { get { return Axis.Tickmarks; } }
		int DrawingLength { get { return viewData.DrawingHalf + Tickmarks.Length; } }
		int DrawingCrossLength { get { return viewData.DrawingCrossHalf + Tickmarks.Length; } }
		int DrawingMinorLength { get { return viewData.DrawingHalf + Tickmarks.MinorLength; } }
		int DrawingMinorCrossLength { get { return viewData.DrawingCrossHalf + Tickmarks.MinorLength; } }
		public RadarAxisYTickmarksViewData(RadarAxisYMapping mapping, RadarAxisYViewData viewData, AxisGridDataEx gridData, bool isVerticalOrHorizontal) {
			this.mapping = mapping;
			this.viewData = viewData;
			this.isVerticalOrHorizontal = isVerticalOrHorizontal;
			this.extent = CalculateExtent();
			if (Tickmarks.Visible)
				tickmarksLines = CreateTickmarkMajorLines(gridData);
			if (Tickmarks.MinorVisible)
				minorTickmarksLines = CreateTickmarkMinorLines(gridData);
			bounds = CalculateBounds();
		}
		double CalculateExtent() {
			double length = viewData.Half + Tickmarks.Length;
			double minorLength = Tickmarks.MinorVisible ? viewData.Half + Tickmarks.MinorLength : 0;
			return Math.Max(length, minorLength);
		}
		TickmarkLineBase[] CreateTickmarkMajorLines(AxisGridDataEx gridData) {
			return CreateTickmarkLines(gridData.Items.VisibleValues, DrawingLength, Tickmarks.CrossAxis ? DrawingCrossLength : 0, Tickmarks.Thickness);
		}
		TickmarkLineBase[] CreateTickmarkMinorLines(AxisGridDataEx gridData) {
			return CreateTickmarkLines(gridData.MinorValues, DrawingMinorLength, Tickmarks.CrossAxis ? DrawingMinorCrossLength : 0, Tickmarks.MinorThickness);
		}
		TickmarkLineBase[] CreateTickmarkLines(List<double> values, int length, int crossLength, int thickness) {
			TickmarkLineBase[] lines = new TickmarkLineBase[values.Count];
			if(isVerticalOrHorizontal) {
				int halfFirst, halfSecond;
				ThicknessUtils.SplitToHalfs(thickness, out halfFirst, out halfSecond);
				for(int i = 0; i < values.Count; i++)
					lines[i] = CreateTickmarkLineAsRectangle(values[i], length, crossLength, halfFirst, halfSecond);
			}
			else {
				for(int i = 0; i < values.Count; i++)
					lines[i] = CreateTickmarkLineAsLine(values[i], length, crossLength, thickness);
			}
			return lines;
		}
		TickmarkLineBase CreateTickmarkLineAsRectangle(double value, int length, int crossLength, int halfFirst, int halfSecond) {
			DiagramPoint leftPoint = mapping.GetDiagramPoint(value, halfSecond, length);
			DiagramPoint rightPoint = mapping.GetDiagramPoint(value, -halfFirst, -crossLength);
			return new TickmarkLineAsRectangle(MathUtils.StrongRoundXY(leftPoint), MathUtils.StrongRoundXY(rightPoint));
		}
		TickmarkLineBase CreateTickmarkLineAsLine(double value, int length, int crossLength, int thickness) {
			DiagramPoint leftPoint = mapping.GetDiagramPoint(value, 0, length);
			DiagramPoint rightPoint = mapping.GetDiagramPoint(value, 0, -crossLength);
			return new TickmarkLineAsLine(leftPoint, rightPoint, thickness);
		}
		Rectangle CalculateBounds() {
			GraphicsPath path = CreateGraphicsPath();
			if(path != null)
				using(path)
					return GraphicUtils.RoundRectangle(path.GetBounds());
			else
				return Rectangle.Empty;
		}
		public GraphicsPath CreateGraphicsPath() {
			if(tickmarksLines.Length == 0 && minorTickmarksLines.Length == 0)
				return null;
			GraphicsPath path = new GraphicsPath();
			try {
				GraphicsPath subPath = CreateGraphicsPath(tickmarksLines);
				if(subPath != null)
					using(subPath)
						path.AddPath(subPath, false);
				GraphicsPath minorSubPath = CreateGraphicsPath(minorTickmarksLines);
				if(minorSubPath != null)
					using(minorSubPath)
						path.AddPath(minorSubPath, false);
				return path;
			}
			catch {
				path.Dispose();
				return null;
			}
		}
	}
}

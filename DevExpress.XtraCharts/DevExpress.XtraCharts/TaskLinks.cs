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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum TaskLinkColorSource {
		ParentColor,
		ParentBorderColor,
		ChildColor,
		ChildBorderColor,
		OwnColor
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class TaskLinkOptions : ChartElement {
		static void RenderLine(IRenderer renderer, DiagramPoint dp1, DiagramPoint dp2, Color color) {
			Point p1 = (Point)dp1;
			Point p2 = (Point)dp2;
			if (p1 == p2)
				renderer.FillRectangle(new RectangleF(p1.X, p1.Y, 1, 1), color);
			else
				renderer.DrawLine(p1, p2, color, 1);
		}
		const int DefaultArrowWidht = 7;
		const int DefaultArrowHeight = 5;
		const int DefaultThickness = 3;
		const int DefaultMinIndent = 2;
		const bool DefaultVisible = true;
		const TaskLinkColorSource DefaultColorSource = TaskLinkColorSource.ParentColor;
		static readonly Color DefaultColor = Color.Empty;
		internal const int MinArrowIndent = 2;
		int arrowWidth = DefaultArrowWidht;
		int arrowHeight = DefaultArrowHeight;
		int minIndent = DefaultMinIndent;
		int thickness = DefaultThickness;
		bool visible = DefaultVisible;
		TaskLinkColorSource colorSource = DefaultColorSource;
		Color color = DefaultColor;
		GanttSeriesView View { get { return (GanttSeriesView)Owner; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TaskLinkOptionsArrowWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TaskLinkOptions.ArrowWidth"),
		XtraSerializableProperty
		]
		public int ArrowWidth {
			get { return arrowWidth; }
			set {
				if(value <= 0 || value % 2 == 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectArrowWidth));
				if(arrowWidth == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				arrowWidth = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TaskLinkOptionsArrowHeight"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TaskLinkOptions.ArrowHeight"),
		XtraSerializableProperty
		]
		public int ArrowHeight {
			get { return arrowHeight; }
			set {
				if(value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectArrowHeight));
				if(arrowHeight == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				arrowHeight = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TaskLinkOptionsMinIndent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TaskLinkOptions.MinIndent"),
		XtraSerializableProperty
		]
		public int MinIndent {
			get { return minIndent; }
			set {
				if(value < 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectTaskLinkMinIndent));
				if(minIndent == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				minIndent = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TaskLinkOptionsThickness"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TaskLinkOptions.Thickness"),
		XtraSerializableProperty
		]
		public int Thickness {
			get { return thickness; }
			set {
				if(value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLineThickness));
				if(thickness == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				thickness = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TaskLinkOptionsVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TaskLinkOptions.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return visible; }
			set {
				if(visible == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				visible = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TaskLinkOptionsColorSource"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TaskLinkOptions.ColorSource"),
		XtraSerializableProperty
		]
		public TaskLinkColorSource ColorSource {
			get { return colorSource; }
			set {
				if(colorSource == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				colorSource = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("TaskLinkOptionsColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.TaskLinkOptions.Color"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color Color {
			get { return color; }
			set {
				if(color == value)
					return;
				if(Owner != null && !Owner.Loading) {
					if(value == Color.Empty) {
						if(colorSource == TaskLinkColorSource.OwnColor)
							colorSource = DefaultColorSource;
					} else {
						if(colorSource != TaskLinkColorSource.OwnColor)
							colorSource = TaskLinkColorSource.OwnColor;
					}
				}
				SendNotification(new ElementWillChangeNotification(this));
				color = value;
				RaiseControlChanged();
			}
		}
		internal TaskLinkOptions(ChartElement owner) : base(owner) { }
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "ArrowWidth")
				return ShouldSerializeArrowWidth();
			if(propertyName == "ArrowHeight")
				return ShouldSerializeArrowHeight();
			if(propertyName == "MinIndent")
				return ShouldSerializeMinIndent();
			if(propertyName == "Thickness")
				return ShouldSerializeThickness();
			if(propertyName == "Visible")
				return ShouldSerializeVisible();
			if(propertyName == "ColorSource")
				return ShouldSerializeColorSource();
			if(propertyName == "Color")
				return ShouldSerializeColor();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeArrowWidth() {
			return arrowWidth != DefaultArrowWidht;
		}
		void ResetArrowWidth() {
			ArrowWidth = DefaultArrowWidht;
		}
		bool ShouldSerializeArrowHeight() {
			return arrowHeight != DefaultArrowHeight;
		}
		void ResetArrowHeight() {
			ArrowHeight = DefaultArrowHeight;
		}
		bool ShouldSerializeMinIndent() {
			return minIndent != DefaultMinIndent;
		}
		void ResetMinIndent() {
			MinIndent = DefaultMinIndent;
		}
		bool ShouldSerializeThickness() {
			return thickness != DefaultThickness;
		}
		void ResetThickness() {
			Thickness = DefaultThickness;
		}
		bool ShouldSerializeVisible() {
			return visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
		}
		bool ShouldSerializeColorSource() {
			return colorSource != DefaultColorSource;
		}
		void ResetColorSource() {
			ColorSource = DefaultColorSource;
		}
		bool ShouldSerializeColor() {
			return color != DefaultColor;
		}
		void ResetColor() {
			Color = DefaultColor;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeArrowWidth() ||
				ShouldSerializeArrowHeight() ||
				ShouldSerializeMinIndent() ||
				ShouldSerializeThickness() ||
				ShouldSerializeVisible() ||
				ShouldSerializeColorSource() ||
				ShouldSerializeColor();
		}
		#endregion
		Color GetColor(SeriesPointLayout parentLayout, SeriesPointLayout childLayout) {
			switch(colorSource) {
				case TaskLinkColorSource.ParentColor:
					return parentLayout.PointData.DrawOptions.Color;
				case TaskLinkColorSource.ParentBorderColor:
					return BarSeriesView.CalculateBorderColor((BarDrawOptions)parentLayout.PointData.DrawOptions);
				case TaskLinkColorSource.ChildColor:
					return childLayout.PointData.DrawOptions.Color;
				case TaskLinkColorSource.ChildBorderColor:
					return BarSeriesView.CalculateBorderColor((BarDrawOptions)childLayout.PointData.DrawOptions);
				case TaskLinkColorSource.OwnColor:
					return this.color;
				default:
					throw new DefaultSwitchException();
			}
		}
		void RenderArrow(IRenderer renderer, LineStrip strip, Color color) {
			renderer.EnableAntialiasing(true);
			RenderLine(renderer, new DiagramPoint(strip[0].X, strip[0].Y), new DiagramPoint(strip[1].X, strip[1].Y), color);
			RenderLine(renderer, new DiagramPoint(strip[1].X, strip[1].Y), new DiagramPoint(strip[2].X, strip[2].Y), color);
			RenderLine(renderer, new DiagramPoint(strip[2].X, strip[2].Y), new DiagramPoint(strip[0].X, strip[0].Y), color);
			renderer.FillPolygon(strip, color);
			IHitTest hitTest = View.Series as IHitTest;
			if (hitTest != null && !hitTest.State.Normal)
				renderer.FillPolygon(strip, hitTest.State.HatchStyle, hitTest.State.HatchColorLight, color);
			renderer.RestoreAntialiasing();
		}
		void RenderRectangle(IRenderer renderer, ZPlaneRectangle rect, Color color) {
			renderer.FillRectangle((RectangleF)rect, color);
			IHitTest hitTest = View.Series as IHitTest;
			if (hitTest != null && !hitTest.State.Normal)
				renderer.FillRectangle((Rectangle)rect, hitTest.State.HatchStyle, hitTest.State.HatchColorLight, color);
		}
		protected override ChartElement CreateObjectForClone() {
			return new TaskLinkOptions(null);
		}
		internal void Render(IRenderer renderer, SeriesPointLayout parentLayout, TaskLinkLayout[] linkLayouts) {
			foreach (TaskLinkLayout linkLayout in linkLayouts) {
				Color color = GetColor(parentLayout, linkLayout.ChildLayout);
				if (linkLayout.LineRects != null)
					foreach (ZPlaneRectangle rect in linkLayout.LineRects)
						RenderRectangle(renderer, rect, color);
				if (linkLayout.ArrowPoints != null)
					RenderArrow(renderer, linkLayout.ArrowPoints, color);
			}			
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			TaskLinkOptions options = obj as TaskLinkOptions;
			if(options == null)
				return;
			arrowWidth = options.arrowWidth;
			arrowHeight = options.arrowHeight;
			minIndent = options.minIndent;
			thickness = options.thickness;
			visible = options.visible;
			colorSource = options.colorSource;
			color = options.color;
		}
		public override string ToString() {
			return "(TaskLinkOptions)";
		}
		public override bool Equals(object obj) {
			TaskLinkOptions options = obj as TaskLinkOptions;
			return
				options != null &&
				arrowWidth == options.arrowWidth &&
				arrowHeight == options.arrowHeight &&
				minIndent == options.minIndent &&
				thickness == options.thickness &&
				visible == options.visible &&
				colorSource == options.colorSource &&
				color == options.color;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class TaskLinkLayout {
		enum ArrowDirection {
			LeftToRight,
			RightToLeft,
			BottomToTop,
			TopToBottom
		}
		static int GetNearOffset(int size) {
			if(size % 2 != 0)
				return (size - 1) / 2;
			return size / 2;
		}
		static int GetFarOffset(int size) {
			if(size % 2 != 0)
				return (size - 1) / 2;
			return size / 2 - 1;
		}
		static DiagramPoint[] CreatePointsBottomToTop(DiagramPoint startPoint, DiagramPoint finishPoint, int startIndent, int finishIndent, int thickness) {
			DiagramPoint[] points = new DiagramPoint[6];
			points[0] = DiagramPoint.Offset(startPoint, 0, -GetFarOffset(thickness), 0);
			points[1] = DiagramPoint.Offset(points[0], 0, -startIndent, 0);
			points[5] = DiagramPoint.Offset(finishPoint, 0, GetNearOffset(thickness), 0);
			points[4] = DiagramPoint.Offset(points[5], 0, finishIndent, 0);
			int horizontalDistance = (int)Math.Abs(startPoint.X - finishPoint.X) + 1;
			if (startPoint.X > finishPoint.X) {
				points[2] = DiagramPoint.Offset(points[1], -GetFarOffset(horizontalDistance), 0, 0);
				points[3] = DiagramPoint.Offset(points[4], GetNearOffset(horizontalDistance), 0, 0);
			} 
			else {
				points[2] = DiagramPoint.Offset(points[1], GetNearOffset(horizontalDistance), 0, 0);
				points[3] = DiagramPoint.Offset(points[4], -GetFarOffset(horizontalDistance), 0, 0);
			}
			if (points[2].Y > points[3].Y) {
				int verticalDistance = (int)Math.Abs(points[2].Y - points[3].Y) + 1;
				double farOffset = GetFarOffset(verticalDistance);
				points[1].Y -= farOffset;
				points[2].Y -= farOffset;
				double nearOffset = GetNearOffset(verticalDistance);
				points[3].Y += nearOffset;
				points[4].Y += nearOffset;
			}
			return points;
		}
		public static DiagramPoint[] CreatePointsBottomToBottom(DiagramPoint startPoint, DiagramPoint finishPoint, int startIndent, int finishIndent, int thickness) {
			DiagramPoint[] points = new DiagramPoint[4];
			points[0] = DiagramPoint.Offset(startPoint, 0, -GetFarOffset(thickness), 0);
			points[1] = DiagramPoint.Offset(points[0], 0, -startIndent, 0);
			points[3] = DiagramPoint.Offset(finishPoint, 0, -GetFarOffset(thickness), 0);
			points[2] = DiagramPoint.Offset(points[3], 0, -finishIndent, 0);
			int distance = (int)Math.Abs(points[1].Y - points[2].Y);
			if (points[1].Y > points[2].Y)
				points[1].Y -= distance;
			else
				points[2].Y -= distance;
			return points;
		}
		static DiagramPoint[] CreatePointsTopToTop(DiagramPoint startPoint, DiagramPoint finishPoint, int startIndent, int finishIndent, int thickness) {
			DiagramPoint[] points = new DiagramPoint[4];
			points[0] = DiagramPoint.Offset(startPoint, 0, GetNearOffset(thickness), 0);
			points[1] = DiagramPoint.Offset(points[0], 0, startIndent, 0);
			points[3] = DiagramPoint.Offset(finishPoint, 0, GetNearOffset(thickness), 0);
			points[2] = DiagramPoint.Offset(points[3], 0, finishIndent, 0);
			int distance = (int)Math.Abs(points[1].Y - points[2].Y);
			if (points[1].Y > points[2].Y)
				points[2].Y += distance;
			else
				points[1].Y += distance;
			return points;
		}
		static DiagramPoint[] CreatePointsTopToRight(DiagramPoint startPoint, DiagramPoint finishPoint, int minIndent, int thickness) {
			if(startPoint.X > finishPoint.X || finishPoint.Y + 1 < startPoint.Y)
				return null;
			DiagramPoint[] points = new DiagramPoint[3];
			points[0] = DiagramPoint.Offset(startPoint, 0, GetNearOffset(thickness), 0);
			points[1] = DiagramPoint.Offset(points[0], 0, minIndent, 0);
			if (finishPoint.Y > points[1].Y)
				points[1].Y += (int)Math.Abs(finishPoint.Y - points[1].Y);
			points[2] = new DiagramPoint(finishPoint.X - GetFarOffset(thickness), points[1].Y, 0);
			return points;
		}
		static DiagramPoint[] CreatePointsTopToLeft(DiagramPoint startPoint, DiagramPoint finishPoint, int minIndent, int thickness) {
			if(startPoint.X < finishPoint.X || finishPoint.Y + 1 < startPoint.Y)
				return null;
			DiagramPoint[] points = new DiagramPoint[3];
			points[0] = DiagramPoint.Offset(startPoint, 0, GetNearOffset(thickness), 0);
			points[1] = DiagramPoint.Offset(points[0], 0, minIndent, 0);
			if (finishPoint.Y > points[1].Y)
				points[1].Y += (int)Math.Abs(finishPoint.Y - points[1].Y);
			points[2] = new DiagramPoint(finishPoint.X + GetNearOffset(thickness), points[1].Y, 0);
			return points;
		}
		static ZPlaneRectangle CreateHorizontalRectangle(DiagramPoint p1, DiagramPoint p2, int thickness) {
			ChartDebug.Assert(p1.Y == p2.Y);
			DiagramPoint leftPoint, rightPoint;
			if (p1.X > p2.X) {
				leftPoint = p2;
				rightPoint = p1;
			} 
			else {
				leftPoint = p1;
				rightPoint = p2;
			}
			return new ZPlaneRectangle(DiagramPoint.Offset(leftPoint, -GetNearOffset(thickness), -GetNearOffset(thickness), 0),
									   DiagramPoint.Offset(rightPoint, GetFarOffset(thickness), GetFarOffset(thickness), 0));
		}
		static ZPlaneRectangle CreateVerticalRectangle(DiagramPoint p1, DiagramPoint p2, int thickness) {
			ChartDebug.Assert(p1.X == p2.X);
			DiagramPoint bottomPoint, topPoint;
			if(p1.Y > p2.Y) {
				bottomPoint = p2;
				topPoint = p1;
			} 
			else {
				bottomPoint = p1;
				topPoint = p2;
			}
			return new ZPlaneRectangle(DiagramPoint.Offset(bottomPoint, -GetNearOffset(thickness), -GetNearOffset(thickness), 0),
									   DiagramPoint.Offset(topPoint, GetFarOffset(thickness), GetFarOffset(thickness), 0));
		}
		static bool CalcIntersection(double x11, double x12, double x21, double x22, out double x1, out double x2) {
			x1 = double.NaN;
			x2 = double.NaN;
			if(x21 >= x11 && x21 <= x12) {
				x1 = x21;
				x2 = x22 > x12 ? x12 : x22;
				return true;
			} else if(x22 >= x11 && x22 <= x12) {
				x1 = x22;
				x2 = x21 < x11 ? x11 : x21;
				return true;
			}
			return false;
		}
		static ZPlaneRectangle CalcIntersection(ZPlaneRectangle rect1, ZPlaneRectangle rect2) {
			double x1, x2, y1, y2;
			if(!CalcIntersection(rect1.LeftBottom.X, rect1.RightBottom.X, rect2.LeftBottom.X, rect2.RightBottom.X, out x1, out x2))
				return null;
			if(!CalcIntersection(rect1.LeftBottom.Y, rect1.LeftTop.Y, rect2.LeftBottom.Y, rect2.LeftTop.Y, out y1, out y2))
				return null;
			return new ZPlaneRectangle(new DiagramPoint(x1, y1, 0), new DiagramPoint(x2, y2, 0));
		}
		static List<ZPlaneRectangle> SubtractRectangles(ZPlaneRectangle rect1, ZPlaneRectangle rect2) {
			List<ZPlaneRectangle> result = new List<ZPlaneRectangle>();
			if (CalcIntersection(rect1, rect2) == null)
				result.Add(rect1);
			else {
				if (rect1.RightBottom.X > rect2.RightBottom.X)
					result.Add(new ZPlaneRectangle(new DiagramPoint(rect2.RightBottom.X + 1, rect1.LeftBottom.Y, 0),
												   new DiagramPoint(rect1.RightBottom.X, rect1.LeftTop.Y, 0)));
				if (rect1.LeftBottom.X < rect2.LeftBottom.X)
					result.Add(new ZPlaneRectangle(new DiagramPoint(rect1.LeftBottom.X, rect1.LeftBottom.Y, 0),
												   new DiagramPoint(rect2.LeftBottom.X - 1, rect1.LeftTop.Y, 0)));
				double x1 = rect2.LeftBottom.X > rect1.LeftBottom.X ? rect2.LeftBottom.X : rect1.LeftBottom.X;
				double x2 = rect2.RightBottom.X > rect1.RightBottom.X ? rect1.RightBottom.X : rect2.RightBottom.X;
				if (rect1.LeftTop.Y > rect2.LeftTop.Y)
					result.Add(new ZPlaneRectangle(new DiagramPoint(x1, rect1.LeftTop.Y, 0), new DiagramPoint(x2, rect2.LeftTop.Y + 1, 0)));
				if (rect1.LeftBottom.Y < rect2.LeftBottom.Y)
					result.Add(new ZPlaneRectangle(new DiagramPoint(x1, rect1.LeftBottom.Y, 0), new DiagramPoint(x2, rect2.LeftBottom.Y - 1, 0)));
			}
			return result;
		}
		static List<ZPlaneRectangle> SubtractRectangles(List<ZPlaneRectangle> rects, ZPlaneRectangle rect) {
			List<ZPlaneRectangle> result = new List<ZPlaneRectangle>();
			foreach (ZPlaneRectangle r in rects)
				result.AddRange(SubtractRectangles(r, rect));
			return result;
		}
		static List<ZPlaneRectangle> UnionRectangles(List<ZPlaneRectangle> rects) {
			if (rects == null || rects.Count == 0)
				return null;
			List<ZPlaneRectangle> result = new List<ZPlaneRectangle>();
			result.Add(rects[0]);
			for (int i = 1; i < rects.Count; i++) {
				List<ZPlaneRectangle> diffRects = new List<ZPlaneRectangle>();
				diffRects.Add(rects[i]);
				for (int k = 0; k < i; k++) {
					diffRects = SubtractRectangles(diffRects, rects[k]);
					if (diffRects.Count == 0)
						break;
				}
				result.AddRange(diffRects);
			}
			return result;
		}
		static List<ZPlaneRectangle> CreateRectangles(DiagramPoint[] points, int thickness) {
			if (points == null || points.Length == 0)
				return null;
			List<ZPlaneRectangle> list = new List<ZPlaneRectangle>();
			if (points.Length == 1)
				list.Add(CreateHorizontalRectangle(points[0], points[0], thickness));
			for (int i = 0; i < points.Length - 1; i++) {
				DiagramPoint p1 = points[i];
				DiagramPoint p2 = points[i + 1];
				if(p1.Y == p2.Y)
					list.Add(CreateHorizontalRectangle(p1, p2, thickness));
				else if(p1.X == p2.X)
					list.Add(CreateVerticalRectangle(p1, p2, thickness));
				else
					ChartDebug.Assert(false);
			}
			return UnionRectangles(list);
		}
		static DiagramPoint[] CreatePointsArrow(ZPlaneRectangle rect, ArrowDirection direction) {
			DiagramPoint[] points = new DiagramPoint[3];
			switch(direction) {
				case ArrowDirection.LeftToRight:
					points[0] = DiagramPoint.Offset(rect.RightBottom, 0, GetNearOffset((int)rect.Height + 1), 0);
					points[1] = rect.LeftBottom;
					points[2] = rect.LeftTop;
					break;
				case ArrowDirection.RightToLeft:
					points[0] = DiagramPoint.Offset(rect.LeftBottom, 0, GetNearOffset((int)rect.Height + 1), 0);
					points[1] = rect.RightTop;
					points[2] = rect.RightBottom;
					break;
				case ArrowDirection.BottomToTop:
					points[0] = DiagramPoint.Offset(rect.LeftTop, GetNearOffset((int)rect.Width + 1), 0, 0);
					points[1] = rect.RightBottom;
					points[2] = rect.LeftBottom;
					break;
				case ArrowDirection.TopToBottom:
					points[0] = DiagramPoint.Offset(rect.LeftBottom, GetNearOffset((int)rect.Width + 1), 0, 0);
					points[1] = rect.LeftTop;
					points[2] = rect.RightTop;
					break;
				default:
					throw new DefaultSwitchException();
			}
			return points;
		}
		static DiagramPoint[] CreatePointsArrow(DiagramPoint point, int width, int height, ArrowDirection direction) {
			ZPlaneRectangle rect;
			switch (direction) {
				case ArrowDirection.LeftToRight:
					rect = new ZPlaneRectangle(DiagramPoint.Offset(point, 0, -GetNearOffset(width), 0),
											   DiagramPoint.Offset(point, height - 1, GetFarOffset(width), 0));
					break;
				case ArrowDirection.RightToLeft:
					rect = new ZPlaneRectangle(DiagramPoint.Offset(point, 0, -GetNearOffset(width), 0),
											   DiagramPoint.Offset(point, -(height - 1), GetFarOffset(width), 0));
					break;
				case ArrowDirection.BottomToTop:
					rect = new ZPlaneRectangle(DiagramPoint.Offset(point, -GetNearOffset(width), 0, 0),
											   DiagramPoint.Offset(point, GetFarOffset(width), height - 1, 0));
					break;
				case ArrowDirection.TopToBottom:
					rect = new ZPlaneRectangle(DiagramPoint.Offset(point, GetFarOffset(width), 0, 0),
											   DiagramPoint.Offset(point, -GetNearOffset(width), -(height - 1), 0));
					break;
				default:
					throw new DefaultSwitchException();
			}
			return CreatePointsArrow(rect, direction);
		}
		static DiagramPoint GetRectangleCenter(RectangleF rect) {
			return DiagramPoint.Offset((DiagramPoint)rect.LeftTop(), GetNearOffset((int)rect.Width + 1), GetNearOffset((int)rect.Height + 1), 0);
		}
		static LineStrip TransformPoints(XYDiagramMappingBase diagramMapping, DiagramPoint[] points) {
			if(points == null)
				return null;
			MatrixTransform matrixTransform = diagramMapping.Transform;
			LineStrip transformedPoints = new LineStrip(points.Length);
			for (int i = 0; i < points.Length; i++)
				transformedPoints.Add((GRealPoint2D)matrixTransform.Apply(points[i]));
			return transformedPoints;
		}
		static List<ZPlaneRectangle> TransformRectangles(XYDiagramMappingBase diagramMapping, List<ZPlaneRectangle> rects) {
			if (rects == null)
				return null;
			List<ZPlaneRectangle> transformedRects = new List<ZPlaneRectangle>(rects.Count);
			foreach (ZPlaneRectangle rect in rects) {
				LineStrip points = TransformPoints(diagramMapping, new DiagramPoint[] { rect.LeftBottom, rect.RightTop });
				ZPlaneRectangle res = ZPlaneRectangle.MakeRectangle(new DiagramPoint(points[0].X, points[0].Y),
																	new DiagramPoint(points[1].X, points[1].Y));
				transformedRects.Add(res);
			}
			return transformedRects;
		}
		static object GetMinSeriesPointValue(SeriesPoint point) {
			object value0 = point.GetValueObject(0);
			object value1 = point.GetValueObject(1);
			if (value0 is DateTime)
				return ((DateTime)value0) > ((DateTime)value1) ? value1 : value0;
			else 
				return (double)value0 > (double)value1 ? value1 : value0;
		}
		static object GetMaxSeriesPointValue(SeriesPoint point) {
			object value0 = point.GetValueObject(0);
			object value1 = point.GetValueObject(1);
			if (value0 is DateTime)
				return ((DateTime)value0) > ((DateTime)value1) ? value0 : value1;
			else 
				return (double)value0 > (double)value1 ? value0 : value1;
		}
		static bool IsNormalFinishToStartLink(SeriesPoint parentPoint, SeriesPoint childPoint) {
			object parentMinValue = GetMinSeriesPointValue(parentPoint);
			object childMaxValue = GetMaxSeriesPointValue(childPoint);
			if (parentMinValue is DateTime)
				return ((DateTime)childMaxValue) <= ((DateTime)parentMinValue);
			else 
				return (double)childMaxValue <= (double)parentMinValue;
		}
		static int GetFinishPointMinIndent(TaskLinkOptions options) {
			return options.MinIndent > options.ArrowHeight + TaskLinkOptions.MinArrowIndent ?
				options.MinIndent - options.ArrowHeight : TaskLinkOptions.MinArrowIndent;
		}
		static int GetStartPointMinIndent(TaskLinkOptions options) {
			return options.MinIndent;
		}
		static TaskLinkLayout CreateLayoutFinishToFinish(XYDiagramMappingBase diagramMapping, TaskLinkOptions options, TaskLink link, BarSeriesPointLayout parentLayout, BarSeriesPointLayout childLayout) {
			RectangleF parentRect = parentLayout.DiagramRect;
			RectangleF childRect = childLayout.DiagramRect;
			DiagramPoint startPoint = DiagramPoint.Offset((DiagramPoint)childRect.LeftBottom(), GetNearOffset((int)childRect.Width + 1), 1, 0);
			DiagramPoint finishPoint = DiagramPoint.Offset((DiagramPoint)parentRect.LeftBottom(), GetNearOffset((int)parentRect.Width + 1), 1 + options.ArrowHeight, 0);
			DiagramPoint[] linePoints = CreatePointsTopToTop(startPoint, finishPoint, GetStartPointMinIndent(options), GetFinishPointMinIndent(options), options.Thickness);
			List<ZPlaneRectangle> lineRects = CreateRectangles(linePoints, options.Thickness);
			DiagramPoint[] arrowPoints = CreatePointsArrow(DiagramPoint.Offset(linePoints[linePoints.Length - 1], 0, -(GetNearOffset(options.Thickness) + 1), 0),
				options.ArrowWidth, options.ArrowHeight, ArrowDirection.TopToBottom);
			return new TaskLinkLayout(link, TransformRectangles(diagramMapping, lineRects), TransformPoints(diagramMapping, arrowPoints), childLayout);
		}
		static TaskLinkLayout CreateLayoutNormalFinishToStart(XYDiagramMappingBase diagramMapping, TaskLinkOptions options, TaskLink link, BarSeriesPointLayout parentLayout, BarSeriesPointLayout childLayout) {
			RectangleF parentRect = parentLayout.DiagramRect;
			RectangleF childRect = childLayout.DiagramRect;
			DiagramPoint startPoint = DiagramPoint.Offset((DiagramPoint)childRect.LeftBottom(), GetNearOffset((int)childRect.Width + 1), 1, 0);
			DiagramPoint childCenter = GetRectangleCenter(childRect);
			DiagramPoint[] linePoints = null;
			DiagramPoint[] arrowPoints = null;
			if (childCenter.X < parentRect.LeftBottom().X) {
				DiagramPoint finishPoint = DiagramPoint.Offset((DiagramPoint)parentRect.LeftTop(), -(1 + options.ArrowHeight), 0, 0);
				linePoints = CreatePointsTopToRight(startPoint, finishPoint, GetStartPointMinIndent(options), options.Thickness);
				if (linePoints != null)
					arrowPoints = CreatePointsArrow(DiagramPoint.Offset(linePoints[linePoints.Length - 1], 1 + GetFarOffset(options.Thickness), 0, 0),
						options.ArrowWidth, options.ArrowHeight, ArrowDirection.LeftToRight);
			}
			else if (childCenter.X > parentRect.RightBottom().X) {
				DiagramPoint finishPoint = DiagramPoint.Offset((DiagramPoint)parentRect.RightTop(), 1 + options.ArrowHeight, 0, 0);
				linePoints = CreatePointsTopToLeft(startPoint, finishPoint, GetStartPointMinIndent(options), options.Thickness);
				if (linePoints != null)
					arrowPoints = CreatePointsArrow(DiagramPoint.Offset(linePoints[linePoints.Length - 1], - (1 + GetNearOffset(options.Thickness)), 0, 0),
						options.ArrowWidth, options.ArrowHeight, ArrowDirection.RightToLeft);
			}
			if (linePoints == null)
				return null;
			List<ZPlaneRectangle> lineRects = CreateRectangles(linePoints, options.Thickness);
			return new TaskLinkLayout(link, TransformRectangles(diagramMapping, lineRects), TransformPoints(diagramMapping, arrowPoints), childLayout);
		}
		static TaskLinkLayout CreateLayoutFinishToStart(XYDiagramMappingBase diagramMapping, TaskLinkOptions options, TaskLink link, BarSeriesPointLayout parentLayout, BarSeriesPointLayout childLayout) {
			if (IsNormalFinishToStartLink((SeriesPoint)parentLayout.PointData.SeriesPoint, (SeriesPoint)childLayout.PointData.SeriesPoint)) {
				TaskLinkLayout layout = CreateLayoutNormalFinishToStart(diagramMapping, options, link, parentLayout, childLayout);
				if (layout != null)
					return layout;
			}
			RectangleF parentRect = parentLayout.DiagramRect;
			RectangleF childRect = childLayout.DiagramRect;
			PointF startPoint = childRect.LeftBottom().Offset(GetNearOffset((int)childRect.Width + 1), 1);
			PointF finishPoint = parentRect.LeftTop().Offset(GetNearOffset((int)parentRect.Width + 1), -(1 + options.ArrowHeight));
			DiagramPoint[] linePoints = CreatePointsBottomToTop((DiagramPoint)finishPoint, (DiagramPoint)startPoint, GetFinishPointMinIndent(options), GetStartPointMinIndent(options), options.Thickness);
			List<ZPlaneRectangle> lineRects = CreateRectangles(linePoints, options.Thickness);
			DiagramPoint[] arrowPoints = CreatePointsArrow(DiagramPoint.Offset(linePoints[0], 0, GetFarOffset(options.Thickness) + 1, 0),
				options.ArrowWidth, options.ArrowHeight, ArrowDirection.BottomToTop);
			return new TaskLinkLayout(link, TransformRectangles(diagramMapping, lineRects), TransformPoints(diagramMapping, arrowPoints), childLayout);
		}
		static TaskLinkLayout CreateLayoutStartToFinish(XYDiagramMappingBase diagramMapping, TaskLinkOptions options, TaskLink link, BarSeriesPointLayout parentLayout, BarSeriesPointLayout childLayout) {
			RectangleF parentRect = parentLayout.DiagramRect;
			RectangleF childRect = childLayout.DiagramRect;
			DiagramPoint startPoint = DiagramPoint.Offset((DiagramPoint)childRect.LeftTop(), GetNearOffset((int)childRect.Width + 1), -1, 0);
			DiagramPoint finishPoint = DiagramPoint.Offset((DiagramPoint)parentRect.LeftBottom(), GetNearOffset((int)parentRect.Width + 1), 1 + options.ArrowHeight, 0);
			DiagramPoint[] linePoints = CreatePointsBottomToTop(startPoint, finishPoint, GetStartPointMinIndent(options), GetFinishPointMinIndent(options), options.Thickness);
			List<ZPlaneRectangle> lineRects = CreateRectangles(linePoints, options.Thickness);
			DiagramPoint[] arrowPoints = CreatePointsArrow(DiagramPoint.Offset(linePoints[linePoints.Length - 1], 0, -(GetNearOffset(options.Thickness) + 1), 0),
				options.ArrowWidth, options.ArrowHeight, ArrowDirection.TopToBottom);
			return new TaskLinkLayout(link, TransformRectangles(diagramMapping, lineRects), TransformPoints(diagramMapping, arrowPoints), childLayout);
		}
		static TaskLinkLayout CreateLayoutStartToStart(XYDiagramMappingBase diagramMapping, TaskLinkOptions options, TaskLink link, BarSeriesPointLayout parentLayout, BarSeriesPointLayout childLayout) {
			RectangleF parentRect = parentLayout.DiagramRect;
			RectangleF childRect = childLayout.DiagramRect;
			DiagramPoint startPoint = DiagramPoint.Offset((DiagramPoint)childRect.LeftTop(), GetNearOffset((int)childRect.Width + 1), -1, 0);
			DiagramPoint finishPoint = DiagramPoint.Offset((DiagramPoint)parentRect.LeftTop(), GetNearOffset((int)parentRect.Width + 1), -(1 + options.ArrowHeight), 0);
			DiagramPoint[] linePoints = CreatePointsBottomToBottom(startPoint, finishPoint, GetStartPointMinIndent(options), GetFinishPointMinIndent(options), options.Thickness);
			List<ZPlaneRectangle> lineRects = CreateRectangles(linePoints, options.Thickness);
			DiagramPoint[] arrowPoints = CreatePointsArrow(DiagramPoint.Offset(linePoints[linePoints.Length - 1], 0, GetFarOffset(options.Thickness) + 1, 0),
				options.ArrowWidth, options.ArrowHeight, ArrowDirection.BottomToTop);
			return new TaskLinkLayout(link, TransformRectangles(diagramMapping, lineRects), TransformPoints(diagramMapping, arrowPoints), childLayout);
		}
		public static TaskLinkLayout CreateLayout(XYDiagramMappingBase diagramMapping, TaskLinkOptions options, TaskLink link, BarSeriesPointLayout parentLayout, BarSeriesPointLayout childLayout) {
			switch(link.LinkType) {
				case TaskLinkType.FinishToFinish:
					return CreateLayoutFinishToFinish(diagramMapping, options, link, parentLayout, childLayout);
				case TaskLinkType.FinishToStart:
					return CreateLayoutFinishToStart(diagramMapping, options, link, parentLayout, childLayout);
				case TaskLinkType.StartToFinish:
					return CreateLayoutStartToFinish(diagramMapping, options, link, parentLayout, childLayout);
				case TaskLinkType.StartToStart:
					return CreateLayoutStartToStart(diagramMapping, options, link, parentLayout, childLayout);
				default:
					throw new DefaultSwitchException();
			}
		}
		readonly TaskLink link;
		readonly List<ZPlaneRectangle> lineRects;
		readonly LineStrip arrowPoints;
		readonly BarSeriesPointLayout childLayout;
		public TaskLink Link { get { return link; } }
		public List<ZPlaneRectangle> LineRects { get { return lineRects; } }
		public LineStrip ArrowPoints { get { return arrowPoints; } }
		public BarSeriesPointLayout ChildLayout { get { return childLayout; } }
		TaskLinkLayout(TaskLink link, List<ZPlaneRectangle> lineRects, LineStrip arrowPoints, BarSeriesPointLayout childLayout) {
			this.link = link;
			this.lineRects = lineRects;
			this.arrowPoints = arrowPoints;
			this.childLayout = childLayout;
		}
	}
}

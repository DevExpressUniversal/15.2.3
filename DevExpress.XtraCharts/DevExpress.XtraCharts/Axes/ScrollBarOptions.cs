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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum ScrollBarAlignment {
		Near,
		Far
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ScrollBarOptions : ChartElement {
		const ScrollBarAlignment DefaultAlignment = ScrollBarAlignment.Near;
		const int DefaultBarThickness = 7;
		const int MinBarThickness = 3;
		const int MaxBarThickness = 25;
		const bool DefaultXAxisScrollBarVisible = true;
		const bool DefaultYAxisScrollBarVisible = true;
		static readonly Color DefaultBackColor = Color.Empty;
		static readonly Color DefaultBarColor = Color.Empty;
		static readonly Color DefaultBorderColor = Color.Empty;
		bool xAxisScrollBarVisible = DefaultXAxisScrollBarVisible;
		bool yAxisScrollBarVisible = true;
		ScrollBarAlignment xAxisScrollBarAlignment = DefaultAlignment;
		ScrollBarAlignment yAxisScrollBarAlignment = DefaultAlignment;
		Color backColor = DefaultBackColor;
		Color barColor = DefaultBarColor;
		Color borderColor = DefaultBorderColor;
		int barThickness = DefaultBarThickness;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollBarOptionsXAxisScrollBarVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollBarOptions.XAxisScrollBarVisible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool XAxisScrollBarVisible {
			get { return xAxisScrollBarVisible; }
			set {
				if (value != xAxisScrollBarVisible) {
					SendNotification(new ElementWillChangeNotification(this));
					xAxisScrollBarVisible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollBarOptionsYAxisScrollBarVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollBarOptions.YAxisScrollBarVisible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool YAxisScrollBarVisible {
			get { return yAxisScrollBarVisible; }
			set {
				if (value != yAxisScrollBarVisible) {
					SendNotification(new ElementWillChangeNotification(this));
					yAxisScrollBarVisible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollBarOptionsXAxisScrollBarAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollBarOptions.XAxisScrollBarAlignment"),
		XtraSerializableProperty
		]
		public ScrollBarAlignment XAxisScrollBarAlignment {
			get { return xAxisScrollBarAlignment; }
			set {
				if (value != xAxisScrollBarAlignment) {
					SendNotification(new ElementWillChangeNotification(this));
					xAxisScrollBarAlignment = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollBarOptionsYAxisScrollBarAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollBarOptions.YAxisScrollBarAlignment"),
		XtraSerializableProperty
		]
		public ScrollBarAlignment YAxisScrollBarAlignment {
			get { return yAxisScrollBarAlignment; }
			set {
				if (value != yAxisScrollBarAlignment) {
					SendNotification(new ElementWillChangeNotification(this));
					yAxisScrollBarAlignment = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollBarOptionsBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollBarOptions.BackColor"),
		XtraSerializableProperty
		]
		public Color BackColor {
			get { return backColor; }
			set {
				if (value != backColor) {
					SendNotification(new ElementWillChangeNotification(this));
					backColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollBarOptionsBarColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollBarOptions.BarColor"),
		XtraSerializableProperty
		]
		public Color BarColor {
			get { return barColor; }
			set {
				if (value != barColor) {
					SendNotification(new ElementWillChangeNotification(this));
					barColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollBarOptionsBorderColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollBarOptions.BorderColor"),
		XtraSerializableProperty
		]
		public Color BorderColor {
			get { return borderColor; }
			set {
				if (value != borderColor) {
					SendNotification(new ElementWillChangeNotification(this));
					borderColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScrollBarOptionsBarThickness"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScrollBarOptions.BarThickness"),
		XtraSerializableProperty
		]
		public int BarThickness {
			get { return barThickness; }
			set {
				if (value != barThickness) {
					if (value < MinBarThickness || value > MaxBarThickness)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectScrollBarThickness));
					SendNotification(new ElementWillChangeNotification(this));
					barThickness = value;
					RaiseControlChanged();
				}
			}
		}
		ScrollBarAppearance Appearance {
			get {
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this);
				return actualAppearance.ScrollBarAppearance;
			}
		}
		internal Color ActualBackColor {
			get { return backColor.IsEmpty ? Appearance.BackColor : backColor; }
		}
		internal Color ActualBarColor {
			get { return barColor.IsEmpty ? Appearance.BarColor : barColor; }
		}
		internal Color ActualBorderColor {
			get { return borderColor.IsEmpty ? Appearance.BorderColor : borderColor; }
		}
		ScrollBarOptions() : base() { }
		internal ScrollBarOptions(XYDiagramPaneBase pane)
			: base(pane) { }
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "XAxisScrollBarVisible")
				return ShouldSerializeXAxisScrollBarVisible();
			if(propertyName == "YAxisScrollBarVisible")
				return ShouldSerializeYAxisScrollBarVisible();
			if(propertyName == "XAxisScrollBarAlignment")
				return ShouldSerializeXAxisScrollBarAlignment();
			if(propertyName == "YAxisScrollBarAlignment")
				return ShouldSerializeYAxisScrollBarAlignment();
			if(propertyName == "BackColor")
				return ShouldSerializeBackColor();
			if(propertyName == "BarColor")
				return ShouldSerializeBarColor();
			if(propertyName == "BorderColor")
				return ShouldSerializeBorderColor();
			if(propertyName == "BarThickness")
				return ShouldSerializeBarThickness();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeProperties() {
			XYDiagramPaneBase pane = Owner as XYDiagramPaneBase;
			return pane == null || pane.ChartContainer == null || pane.ChartContainer.ControlType != ChartContainerType.WebControl;
		}
		bool ShouldSerializeXAxisScrollBarVisible() {
			return !xAxisScrollBarVisible && ShouldSerializeProperties();
		}
		void ResetXAxisScrollBarVisible() {
			XAxisScrollBarVisible = DefaultXAxisScrollBarVisible;
		}
		bool ShouldSerializeYAxisScrollBarVisible() {
			return !yAxisScrollBarVisible && ShouldSerializeProperties();
		}
		void ResetYAxisScrollBarVisible() {
			YAxisScrollBarVisible = DefaultYAxisScrollBarVisible;
		}
		bool ShouldSerializeXAxisScrollBarAlignment() {
			return xAxisScrollBarAlignment != DefaultAlignment && ShouldSerializeProperties();
		}
		void ResetXAxisScrollBarAlignment() {
			XAxisScrollBarAlignment = DefaultAlignment;
		}
		bool ShouldSerializeYAxisScrollBarAlignment() {
			return yAxisScrollBarAlignment != DefaultAlignment && ShouldSerializeProperties();
		}
		void ResetYAxisScrollBarAlignment() {
			YAxisScrollBarAlignment = DefaultAlignment;
		}
		bool ShouldSerializeBackColor() {
			return !backColor.IsEmpty && ShouldSerializeProperties();
		}
		void ResetBackColor() {
			BackColor = DefaultBackColor;
		}
		bool ShouldSerializeBarColor() {
			return !barColor.IsEmpty && ShouldSerializeProperties();
		}
		void ResetBarColor() {
			BorderColor = DefaultBarColor;
		}
		bool ShouldSerializeBorderColor() {
			return !borderColor.IsEmpty && ShouldSerializeProperties();
		}
		void ResetBorderColor() {
			BarColor = DefaultBorderColor;
		}
		bool ShouldSerializeBarThickness() {
			return barThickness != DefaultBarThickness && ShouldSerializeProperties();
		}
		void ResetBarThickness() {
			BarThickness = DefaultBarThickness;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeXAxisScrollBarVisible() ||  ShouldSerializeYAxisScrollBarVisible() ||
				ShouldSerializeXAxisScrollBarAlignment() ||  ShouldSerializeYAxisScrollBarAlignment() || ShouldSerializeBackColor() || 
				ShouldSerializeBarColor() || ShouldSerializeBorderColor() || ShouldSerializeBarThickness();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ScrollBarOptions();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ScrollBarOptions options = obj as ScrollBarOptions;
			if (options == null)
				return;
			xAxisScrollBarVisible = options.xAxisScrollBarVisible;
			yAxisScrollBarVisible = options.yAxisScrollBarVisible;
			xAxisScrollBarAlignment = options.xAxisScrollBarAlignment;
			yAxisScrollBarAlignment = options.yAxisScrollBarAlignment;
			backColor = options.backColor;
			barColor = options.barColor;
			borderColor = options.borderColor;
			barThickness = options.barThickness;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class ChartScrollParameters {
		readonly XYDiagramPaneBase pane;
		readonly NavigationType navigationType;
		readonly ScrollingOrientation orientation;
		readonly double relativePosition;
		readonly int range;
		public XYDiagramPaneBase Pane { get { return pane; } }
		public NavigationType NavigationType { get { return navigationType; } }
		public ScrollingOrientation Orientation { get { return orientation; } }
		public double RelativePosition { get { return relativePosition; } }
		public int Range { get { return range; } }
		public ChartScrollParameters(XYDiagramPaneBase pane, NavigationType navigationType, ScrollingOrientation orientation, double relativePosition, int range) {
			this.pane = pane;
			this.navigationType = navigationType;
			this.orientation = orientation;
			this.relativePosition = relativePosition;
			this.range = range;
		}
		public ChartScrollParameters(XYDiagramPaneBase pane, NavigationType navigationType, ScrollingOrientation orientation) : this(pane, navigationType, orientation, 0.0, 0) {
		}
	}
	public class ScrollBarInteriorViewData {
		static public ScrollBarInteriorViewData CreateInstance(AxisMapping axisMapping, Color color, int centerX, int centerY) {
			ScrollBarInteriorViewData viewData = new ScrollBarInteriorViewData();
			int minY = centerY - 1;
			int maxY = centerY + 1;
			viewData.color = color;
			viewData.centerPoint1 = axisMapping.GetScreenPoint(double.NegativeInfinity, centerX, minY);
			viewData.centerPoint2 = axisMapping.GetScreenPoint(double.NegativeInfinity, centerX, maxY);
			viewData.nearPoint1 = axisMapping.GetScreenPoint(double.NegativeInfinity, centerX - 2, minY);
			viewData.nearPoint2 = axisMapping.GetScreenPoint(double.NegativeInfinity, centerX - 2, maxY);
			viewData.farPoint1 = axisMapping.GetScreenPoint(double.NegativeInfinity, centerX + 2, minY);
			viewData.farPoint2 = axisMapping.GetScreenPoint(double.NegativeInfinity, centerX + 2, maxY);
			return viewData;
		}
		Color color;
		DiagramPoint centerPoint1;
		DiagramPoint centerPoint2;
		DiagramPoint nearPoint1;
		DiagramPoint nearPoint2;
		DiagramPoint farPoint1;
		DiagramPoint farPoint2;
		ScrollBarInteriorViewData() {
		}
		public void Render(IRenderer renderer) {
			renderer.DrawLine((Point)centerPoint1, (Point)centerPoint2, color, 1);
			renderer.DrawLine((Point)nearPoint1, (Point)nearPoint2, color, 1);
			renderer.DrawLine((Point)farPoint1, (Point)farPoint2, color, 1);
		}
	}
	public class ScrollBarArrowViewData {
		public static ScrollBarArrowViewData CreateInstance(AxisMapping axisMapping, Color color, int x, int y, bool rightArrow) {
			ScrollBarArrowViewData viewData = new ScrollBarArrowViewData();
			int pos, centerAdd;
			if (rightArrow) {
				pos = x - 2;
				centerAdd = -1;
			}
			else {
				pos = x + 2;
				centerAdd = 1;
			}
			viewData.color = color;
			viewData.centerPoint = axisMapping.GetScreenPoint(double.NegativeInfinity, x, y);
			viewData.nearPoint = axisMapping.GetScreenPoint(double.NegativeInfinity, pos, y - 2);
			viewData.farPoint = axisMapping.GetScreenPoint(double.NegativeInfinity, pos, y + 2);
			viewData.centerPoint1 = axisMapping.GetScreenPoint(double.NegativeInfinity, x + centerAdd, y);
			viewData.nearPoint1 = axisMapping.GetScreenPoint(double.NegativeInfinity, pos + centerAdd, y - 2);
			viewData.farPoint1 = axisMapping.GetScreenPoint(double.NegativeInfinity, pos + centerAdd, y + 2);
			return viewData;
		}
		Color color;
		DiagramPoint centerPoint;
		DiagramPoint centerPoint1;
		DiagramPoint nearPoint;
		DiagramPoint nearPoint1;
		DiagramPoint farPoint;
		DiagramPoint farPoint1;
		ScrollBarArrowViewData() {
		}
		public void Render(IRenderer renderer) {
			renderer.DrawLine((Point)nearPoint, (Point)centerPoint, color, 1);
			renderer.DrawLine((Point)centerPoint, (Point)farPoint, color, 1);
			renderer.DrawLine((Point)nearPoint1, (Point)centerPoint1, color, 1);
			renderer.DrawLine((Point)centerPoint1, (Point)farPoint1, color, 1);
		}
	}
	public class ScrollBarViewData {
		Axis2D axis;
		XYDiagramPaneBase pane;
		ScrollBarAlignment alignment;
		ScrollBarOptions scrollBarOptions;
		int innerExtent = 0;
		int outerExtent = 0;
		double relativeScrollPosition;
		int scrollRange;
		RectangleF rect;
		RectangleF nearAxisRect;
		RectangleF farAxisRect;
		RectangleF minValueAxisRect;
		RectangleF maxValueAxisRect;
		RectangleF barRect;
		RectangleF minDecrementRect;
		RectangleF maxDecrementRect;
		RectangleF minIncrementRect;
		RectangleF maxIncrementRect;
		ScrollBarArrowViewData leftArrowViewData;
		ScrollBarArrowViewData rightArrowViewData;
		ScrollBarInteriorViewData barInteriorViewData;
		Color backColor;
		Color barColor;
		Color borderColor;
		bool Calculated { get { return !farAxisRect.IsEmpty && !minValueAxisRect.IsEmpty && !maxValueAxisRect.IsEmpty && !barRect.IsEmpty; } }
		public ScrollBarViewData(Axis2D axis, XYDiagramPaneBase pane, Rectangle bounds, ScrollBarOptions scrollBarOptions) : base() {
			this.axis = axis;
			this.pane = pane;
			this.scrollBarOptions = scrollBarOptions;
			alignment = axis.IsValuesAxis ? scrollBarOptions.YAxisScrollBarAlignment : scrollBarOptions.XAxisScrollBarAlignment;
			Calculate(bounds);
		}
		int ApplyOffsetAligmnet(int offset) {
			return alignment == ScrollBarAlignment.Near ? -offset : offset;
		}
		void Calculate(Rectangle bounds) {
			AxisAlignment axisAlignment = alignment == ScrollBarAlignment.Near ? AxisAlignment.Near : AxisAlignment.Far;
			AxisMapping axisMapping = new AxisMapping(bounds, axis, axisAlignment);
			backColor = scrollBarOptions.ActualBackColor;
			barColor = scrollBarOptions.ActualBarColor;
			borderColor = scrollBarOptions.ActualBorderColor;
			int thickness = 0;
			DiagramPoint p1 = axisMapping.GetScreenPoint(double.NegativeInfinity, 0, ApplyOffsetAligmnet(-innerExtent));
			DiagramPoint p2 = axisMapping.GetScreenPoint(double.PositiveInfinity, 0, ApplyOffsetAligmnet(outerExtent));
			nearAxisRect = MathUtils.MakeRectangle((PointF)p1, (PointF)p2, 1.0f);
			int farRectangleOffset = outerExtent + scrollBarOptions.BarThickness + 3;
			int farRectangleExtent = farRectangleOffset + thickness;
			DiagramPoint p3 = axisMapping.GetScreenPoint(double.NegativeInfinity, 0, ApplyOffsetAligmnet(farRectangleOffset));
			DiagramPoint p4 = axisMapping.GetScreenPoint(double.PositiveInfinity, 0, ApplyOffsetAligmnet(farRectangleExtent));
			farAxisRect = MathUtils.MakeRectangle((PointF)p3, (PointF)p4, 1.0f);
			DiagramPoint p5 = axisMapping.GetScreenPoint(double.NegativeInfinity, thickness, ApplyOffsetAligmnet(farRectangleExtent));
			minValueAxisRect = MathUtils.MakeRectangle((PointF)p1, (PointF)p5, 1.0f);
			DiagramPoint p6 = axisMapping.GetScreenPoint(double.PositiveInfinity, -thickness, ApplyOffsetAligmnet(innerExtent));
			maxValueAxisRect = MathUtils.MakeRectangle((PointF)p6, (PointF)p4, 1.0f);
			rect = MathUtils.MakeRectangle((PointF)p1, (PointF)p4, 1.0f);
			CalculateBar(axisMapping, thickness);
		}
		void CalculateBar(AxisMapping axisMapping, int thickness) {
			int barOffset = (thickness + 1) * 2 + scrollBarOptions.BarThickness;
			int actualLength = axisMapping.Lenght - barOffset * 2;
			if (actualLength < scrollBarOptions.BarThickness)
				return;
			relativeScrollPosition = PaneAxesContainer.GetScrollBarPosition((IAxisData)axis);
			int offset1 = ApplyOffsetAligmnet(outerExtent + 2);
			int offset2 = offset1 + ApplyOffsetAligmnet(scrollBarOptions.BarThickness - 1);
			DiagramPoint p1 = axisMapping.GetScreenPoint(double.NegativeInfinity, 0, offset1);
			DiagramPoint p2 = axisMapping.GetScreenPoint(double.NegativeInfinity, barOffset, offset2);
			minDecrementRect = MathUtils.MakeRectangle((PointF)p1, (PointF)p2, 1.0f);
			int minIncrementPosition = axisMapping.Lenght - barOffset - 1;
			p1 = axisMapping.GetScreenPoint(double.NegativeInfinity, minIncrementPosition, offset1);
			p2 = axisMapping.GetScreenPoint(double.NegativeInfinity, minIncrementPosition + barOffset, offset2);
			minIncrementRect = MathUtils.MakeRectangle((PointF)p1, (PointF)p2, 1.0f);
			int scrollBarSize = (int)Math.Floor(PaneAxesContainer.GetScrollBarRelativeSize(axis) * actualLength);
			if (scrollBarSize < scrollBarOptions.BarThickness)
				scrollBarSize = scrollBarOptions.BarThickness;
			if (scrollBarSize % 2 == 0)
				scrollBarSize++;
			scrollRange = actualLength - scrollBarSize;
			int halfScrollBarSize = scrollBarSize / 2;
			int offset = barOffset + halfScrollBarSize;
			int position = Convert.ToInt32(relativeScrollPosition * scrollRange);
			int min = position - halfScrollBarSize;
			int max = position + halfScrollBarSize;
			min += offset;
			max += offset;
			if (min > barOffset) {
				p1 = axisMapping.GetScreenPoint(double.NegativeInfinity, barOffset, offset1);
				p2 = axisMapping.GetScreenPoint(double.NegativeInfinity, min, offset2);
				maxDecrementRect = MathUtils.MakeRectangle((PointF)p1, (PointF)p2, 1.0f);
			}
			else
				maxDecrementRect = RectangleF.Empty;
			if (max < minIncrementPosition) {
				p1 = axisMapping.GetScreenPoint(double.NegativeInfinity, max, offset1);
				p2 = axisMapping.GetScreenPoint(double.NegativeInfinity, minIncrementPosition, offset2);
				maxIncrementRect = MathUtils.MakeRectangle((PointF)p1, (PointF)p2, 1.0f);
			}
			else
				maxIncrementRect = RectangleF.Empty;
			p1 = axisMapping.GetScreenPoint(double.NegativeInfinity, min, offset1);
			p2 = axisMapping.GetScreenPoint(double.NegativeInfinity, max, offset2);
			barRect = MathUtils.MakeRectangle((PointF)p1, (PointF)p2, 1.0f);
			int centerY = (offset1 + offset2) / 2;
			leftArrowViewData = ScrollBarArrowViewData.CreateInstance(axisMapping, borderColor, barOffset / 2 - 1, centerY, false);
			rightArrowViewData = ScrollBarArrowViewData.CreateInstance(axisMapping, borderColor, axisMapping.Lenght - barOffset / 2 + 1, centerY, true);
			if (scrollBarSize >= 9)
				barInteriorViewData = ScrollBarInteriorViewData.CreateInstance(axisMapping, backColor, (min + max) / 2, centerY);
			if (axis.ActualReverse)
				relativeScrollPosition = 1.0 - relativeScrollPosition;
		} 
		public void CalculateDiagramBoundsCorrection(RectangleCorrection correction) {
			if (Calculated) {
				correction.Update(MathUtils.StrongRound(rect));
			}
		}
		public void Render(IRenderer renderer) {
			if (!Calculated)
				return;
			XYDiagram2D diagram = axis.XYDiagram2D;
			Chart chart = diagram.Chart;
			HitTestController hitTestController = chart.HitTestController;
			ScrollingOrientation orientation = axis.IsValuesAxis ? ScrollingOrientation.AxisYScroll : ScrollingOrientation.AxisXScroll;
			if (maxDecrementRect != null)
				renderer.ProcessHitTestRegion(hitTestController, chart, 
					new ChartScrollParameters(pane, NavigationType.LargeDecrement, orientation), new HitRegion(maxDecrementRect), true);
			if (maxIncrementRect != null)
				renderer.ProcessHitTestRegion(hitTestController, chart, 
					new ChartScrollParameters(pane, NavigationType.LargeIncrement, orientation), new HitRegion(maxIncrementRect), true);
			renderer.ProcessHitTestRegion(hitTestController, chart, 
				new ChartScrollParameters(pane, NavigationType.SmallDecrement, orientation), new HitRegion(minDecrementRect), true);
			renderer.ProcessHitTestRegion(hitTestController, chart, 
				new ChartScrollParameters(pane, NavigationType.SmallIncrement, orientation), new HitRegion(minIncrementRect), true);
			renderer.ProcessHitTestRegion(hitTestController, chart, 
				new ChartScrollParameters(pane, NavigationType.ThumbPosition, orientation, relativeScrollPosition, scrollRange), new HitRegion(barRect), true);
			renderer.FillRectangle((RectangleF)rect, backColor);
			renderer.FillRectangle((RectangleF)nearAxisRect, borderColor);
			renderer.FillRectangle((RectangleF)farAxisRect, borderColor);
			renderer.FillRectangle((RectangleF)minValueAxisRect, borderColor);
			renderer.FillRectangle((RectangleF)maxValueAxisRect, borderColor);
			renderer.FillRectangle((RectangleF)barRect, barColor);
			leftArrowViewData.Render(renderer);
			rightArrowViewData.Render(renderer);
			if (barInteriorViewData != null)
				barInteriorViewData.Render(renderer);
		}
	}
}

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
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[TypeConverter(typeof(Bar3DSeriesViewTypeConverter))]
	public abstract class Bar3DSeriesView : SeriesView3DColorEachSupportBase, IBarSeriesView {
		const double DefaultBarWidth = 0.6;
		const double DefaultBarDepth = DefaultBarWidth;
		const bool DefaultBarDepthAuto = true;
		const bool DefaultShowFacet = true;
		const Bar3DModel DefaultModel = Bar3DModel.Box;
		double barWidth = DefaultBarWidth;
		double barDepth = DefaultBarDepth;
		bool barDepthAuto = DefaultBarDepthAuto;
		bool showFacet = DefaultShowFacet;
		RectangleFillStyle3D fillStyle;
		Bar3DModel model;
		Bar3DSeriesViewAppearance Appearance {
			get {
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this);
				return actualAppearance.Bar3DSeriesViewAppearance;
			}
		}
		internal bool IsFlatTopModel {
			get {
				return model == Bar3DModel.Box || model == Bar3DModel.Cylinder;
			}
		}
		internal RectangleFillStyle3D ActualFillStyle {
			get {
				if (this.fillStyle.FillMode != FillMode3D.Empty)
					return this.fillStyle;
				else
					return Appearance.FillStyle;
			}
		}
		protected override int PixelsPerArgument { get { return 40; } }
		protected override FillStyleBase FillStyleInternal { get { return fillStyle; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(XYDiagram3D); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Bar3DSeriesViewBarWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Bar3DSeriesView.BarWidth"),
		XtraSerializableProperty
		]
		public double BarWidth {
			get { return barWidth; }
			set {
				if (value != barWidth) {
					if (value <= 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBarWidth));
					SendNotification(new ElementWillChangeNotification(this));
					barWidth = value;
					RaiseControlChanged(new SeriesGroupsInteractionUpdateInfo(this));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Bar3DSeriesViewBarDepth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Bar3DSeriesView.BarDepth"),
		XtraSerializableProperty
		]
		public double BarDepth {
			get { return barDepth; }
			set {
				if (value != barDepth) {
					if (value <= 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBarDepth));
					SendNotification(new ElementWillChangeNotification(this));
					barDepth = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Bar3DSeriesViewBarDepthAuto"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Bar3DSeriesView.BarDepthAuto"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool BarDepthAuto {
			get { return barDepthAuto; }
			set {
				if (value != barDepthAuto) {
					SendNotification(new ElementWillChangeNotification(this));
					barDepthAuto = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Bar3DSeriesViewFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Bar3DSeriesView.FillStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleFillStyle3D FillStyle { get { return fillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Bar3DSeriesViewModel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Bar3DSeriesView.Model"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Bar3DModel Model {
			get { return model; }
			set {
				if (value != model) {
					SendNotification(new ElementWillChangeNotification(this));
					model = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Bar3DSeriesViewShowFacet"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Bar3DSeriesView.ShowFacet"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowFacet {
			get { return showFacet; }
			set {
				if (value != showFacet) {
					SendNotification(new ElementWillChangeNotification(this));
					showFacet = value;
					RaiseControlChanged();
				}
			}
		}
		public Bar3DSeriesView()
			: base() {
			fillStyle = new RectangleFillStyle3D(this);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "BarWidth")
				return ShouldSerializeBarWidth();
			if (propertyName == "BarDepth")
				return ShouldSerializeBarDepth();
			if (propertyName == "BarDepthAuto")
				return ShouldSerializeBarDepthAuto();
			if (propertyName == "FillStyle")
				return ShouldSerializeFillStyle();
			if (propertyName == "Model")
				return ShouldSerializeModel();
			if (propertyName == "ShowFacet")
				return ShouldSerializeShowFacet();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeBarWidth() {
			return barWidth != DefaultBarWidth;
		}
		void ResetBarWidth() {
			BarWidth = DefaultBarWidth;
		}
		bool ShouldSerializeBarDepth() {
			return barDepth != DefaultBarDepth;
		}
		void ResetBarDepth() {
			BarDepth = DefaultBarDepth;
		}
		bool ShouldSerializeBarDepthAuto() {
			return barDepthAuto != DefaultBarDepthAuto;
		}
		void ResetBarDepthAuto() {
			BarDepthAuto = DefaultBarDepthAuto;
		}
		bool ShouldSerializeFillStyle() {
			return FillStyle.ShouldSerialize();
		}
		bool ShouldSerializeModel() {
			return model != DefaultModel;
		}
		void ResetModel() {
			Model = DefaultModel;
		}
		bool ShouldSerializeShowFacet() {
			return showFacet != DefaultShowFacet;
		}
		void ResetShowFacet() {
			ShowFacet = DefaultShowFacet;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeBarWidth() ||
				ShouldSerializeBarDepth() ||
				ShouldSerializeBarDepthAuto() ||
				ShouldSerializeFillStyle() ||
				ShouldSerializeModel() ||
				ShouldSerializeShowFacet();
		}
		#endregion
		internal void CalcWidthAndDepth(XYDiagram3DCoordsCalculator coordsCalculator, double initialWidth, out double width, out double depth) {
			AxisBase axis = coordsCalculator.Diagram.AxisX;
			int boundsWidth = coordsCalculator.Bounds.Width;
			double min = AxisCoordCalculator.GetCoord(axis.VisualRangeData, 0.0, boundsWidth);
			double max = AxisCoordCalculator.GetCoord(axis.VisualRangeData, initialWidth, boundsWidth);
			width = Math.Abs(min - max);
			if (barDepthAuto)
				depth = width;
			else {
				max = AxisCoordCalculator.GetCoord(axis.VisualRangeData, barDepth, boundsWidth);
				depth = Math.Abs(min - max);
			}
		}
		PlaneEquation CalcSeparatingPlane(Box boundingBox) {
			if (FillStyle.FillMode != FillMode3D.Gradient)
				return null;
			RectangleGradientFillOptions options = FillStyle.Options as RectangleGradientFillOptions;
			if (options == null)
				return null;
			if (options.GradientMode == RectangleGradientMode.ToCenterVertical ||
				options.GradientMode == RectangleGradientMode.FromCenterVertical)
				return new PlaneEquation(0, 1, 0, -(boundingBox.Location.Y + boundingBox.Height / 2));
			if (options.GradientMode == RectangleGradientMode.ToCenterHorizontal ||
				options.GradientMode == RectangleGradientMode.FromCenterHorizontal)
				return new PlaneEquation(1, 0, 0, -(boundingBox.Location.X + boundingBox.Width / 2));
			return null;
		}
		protected override void Dispose(bool disposing) {
			if (disposing && !IsDisposed && fillStyle != null) {
				fillStyle.Dispose();
				fillStyle = null;
			}
			base.Dispose(disposing);
		}
		protected override DiagramPoint? CalculateAnnotationAnchorPoint(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData, IAxisRangeData axisRangeY) {
			Bar3DData barData = CreateBarData(pointData.RefinedPoint);
			double barWidth, barDepth;
			CalcWidthAndDepth(coordsCalculator, barData.Width, out barWidth, out barDepth);
			DiagramPoint point = coordsCalculator.GetDiagramPoint(Series, barData.DisplayArgument, barData.MaxValue, false);
			point.X += barData.FixedOffset;
			return coordsCalculator.Project(point);
		}
		protected override Rectangle CorrectLegendMarkerBounds(Rectangle bounds) {
			bounds.X++;
			bounds.Y++;
			bounds.Width -= 2;
			bounds.Height -= 2;
			return bounds;
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			Bar3DDrawOptions barDrawOptions = seriesPointDrawOptions as Bar3DDrawOptions;
			if (barDrawOptions == null)
				return;
			RectangleFillStyle3D actualFillStyle = (RectangleFillStyle3D)barDrawOptions.FillStyle;
			actualFillStyle.Options.RenderRectangle(renderer, bounds, bounds, barDrawOptions.Color, barDrawOptions.ActualColor2);
		}
		protected virtual double GetMinValue(IXYPoint refinedPoint) {
			return 0;
		}
		protected virtual double GetMaxValue(IXYPoint refinedPoint) {
			return refinedPoint.Value;
		}
		protected virtual double GetTotalMaxValue(IXYPoint refinedPoint) {
			return refinedPoint.Value;
		}
		protected virtual double GetBarWidth(IXYPoint refinedPoint) {
			return BarWidth;
		}
		protected virtual double GetDisplayOffset(IXYPoint refinedPoint) {
			return 0;
		}
		protected virtual int GetFixedOffset(IXYPoint refinedPoint) {
			return 0;
		}
		protected internal Bar3DData CreateBarData(IXYPoint refinedPoint) {
			IXYPoint point = (IXYPoint)refinedPoint;
			double maxValue = GetMaxValue(refinedPoint);
			double totalMaxValue = GetTotalMaxValue(refinedPoint);
			return new Bar3DData(point.Argument, GetMinValue(refinedPoint), maxValue, totalMaxValue, GetBarWidth(refinedPoint),
				GetDisplayOffset(refinedPoint), GetFixedOffset(refinedPoint), ShowFacet && maxValue == totalMaxValue);
		}
		protected virtual SeriesPointLayout CalculateSeriesPointLayoutInternal(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData, Bar3DData barData) {
			Bar3DDrawOptions drawOptions = (Bar3DDrawOptions)pointData.DrawOptions;
			Bar3DRepresentationCalculator representationCalculator = new Bar3DRepresentationCalculator(this);
			Box boundingBox;
			PlanePolygon[] representation = representationCalculator.CalcRepresentation(coordsCalculator, barData, out boundingBox);
			PlaneEquation separatingPlane = CalcSeparatingPlane(boundingBox);
			PlanePolygon[] clippedRepresentation = coordsCalculator.ClipPolyhedron(representation, boundingBox, separatingPlane);
			if (clippedRepresentation == null)
				return null;
			List<PlanePolygon> list = new List<PlanePolygon>();
			ZPlaneRectangle gradientRect = new ZPlaneRectangle(boundingBox.Location, boundingBox.Width, boundingBox.Height);
			foreach (PlanePolygon polygon in clippedRepresentation)
				list.AddRange(drawOptions.FillStyle.FillPlanePolygon(polygon, gradientRect, drawOptions.Color, drawOptions.ActualColor2));
			return new View3DSeriesPointLayout(pointData, list.ToArray());
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new Bar3DSeriesLabel();
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new Bar3DDrawOptions(this);
		}
		protected internal override double GetSeriesDepth() {
			return this.barDepthAuto ? this.barWidth : this.barDepth;
		}
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData) {
			return CalculateSeriesPointLayoutInternal(coordsCalculator, pointData, CreateBarData(pointData.RefinedPoint));
		}
		protected internal override XYDiagram3DWholeSeriesLayout CalculateWholeSeriesLayout(XYDiagram3DCoordsCalculator coordsCalculator, SeriesLayout seriesLayout) {
			int seriesWeight = coordsCalculator.GetSeriesWeight(Series);
			foreach (SeriesPointLayout layout in seriesLayout) {
				View3DSeriesPointLayout pointLayout = layout as View3DSeriesPointLayout;
				if (pointLayout != null && pointLayout.Polygons != null) {
					foreach (PlanePolygon polygon in pointLayout.Polygons)
						polygon.Weight = seriesWeight;
					seriesWeight++;
				}
			}
			return new XYDiagram3DWholeSeriesLayout();
		}
		public override string GetValueCaption(int index) {
			if (index > 0)
				throw new IndexOutOfRangeException();
			return ChartLocalizer.GetString(ChartStringId.ValueMember);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			IBarSeriesView view = obj as IBarSeriesView;
			if (view == null)
				return;
			barWidth = view.BarWidth;
			Bar3DSeriesView barView = obj as Bar3DSeriesView;
			if (barView == null)
				return;
			barDepth = barView.barDepth;
			barDepthAuto = barView.barDepthAuto;
			model = barView.model;
			showFacet = barView.showFacet;
			fillStyle.Assign(barView.fillStyle);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			Bar3DSeriesView view = (Bar3DSeriesView)obj;
			return barWidth == view.barWidth && barDepth == view.barDepth &&
				barDepthAuto == view.barDepthAuto && fillStyle.Equals(view.FillStyle) &&
				model == view.model && showFacet == view.showFacet;
		}
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum Bar3DModel {
		Box,
		Cylinder,
		Cone,
		Pyramid
	}
}
namespace DevExpress.XtraCharts.Native {
	public class Bar3DData {
		readonly double argument;
		readonly double zeroValue;
		readonly double actualValue;
		readonly double maxValue;
		readonly double width;
		readonly double displayArgument;
		readonly int fixedOffset;
		readonly bool showFacet;
		public double ZeroValue { get { return zeroValue; } }
		public double ActualValue { get { return actualValue; } }
		public double Width { get { return width; } }
		public double DisplayArgument { get { return displayArgument; } }
		public int FixedOffset { get { return fixedOffset; } }
		public bool ShowFacet { get { return showFacet; } }
		public double MaxValue { get { return maxValue; } }
		public Bar3DData(double argument, double zeroValue, double actualValue, double maxValue, double width, double displayOffset, int fixedOffset, bool showFacet) {
			this.argument = argument;
			this.zeroValue = zeroValue;
			this.actualValue = actualValue;
			this.maxValue = maxValue;
			this.width = width;
			this.displayArgument = argument + displayOffset;
			this.fixedOffset = fixedOffset;
			this.showFacet = showFacet;
		}
	}
}

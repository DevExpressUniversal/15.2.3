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
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(BubbleSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class BubbleSeriesView : PointSeriesViewBase, IXYWSeriesView, ITransparableView {
		const byte DefaultOpacity = 255;
		const int ValueCount = 2;
		const bool DefaultAutoSize = true;
		const double MaxAutoSize = 0.2;
		const double MinAutoSize = 0.05;
		const int MinimumMinSizeInPixels = 4;
		const int MinimumMaxSizeInPixels = 12;
		const double DefaultMaxSize = 0.9;
		const double DefaultMinSize = 0.3;
		bool autoSize = DefaultAutoSize;
		byte opacity = DefaultOpacity;
		double maxSize = DefaultMaxSize;
		double minSize = DefaultMinSize;
		Color Color2 {
			get {
				FillOptionsColor2Base options = BubbleMarkerOptions.FillStyle.Options as FillOptionsColor2Base;
				return options == null ? Color.Empty : options.Color2;
			}
			set {
				FillOptionsColor2Base options = BubbleMarkerOptions.FillStyle.Options as FillOptionsColor2Base;
				if (options != null)
					options.SetColor2(value);
			}
		}
		protected override int PixelsPerArgument { get { return 40; } }
		protected internal double ActualMaxSize {
			get {
				if (AutoSize) {
					Tuple<double, double> minAxis = GetMinAxisLengthAndRange();
					if (minAxis != null) {
						double maxSize = MaxAutoSize * minAxis.Item2;
						double pixelsPerUnit = minAxis.Item1 / minAxis.Item2;
						if (pixelsPerUnit * maxSize < MinimumMaxSizeInPixels)
							return MinimumMaxSizeInPixels / pixelsPerUnit;
						return maxSize;
					}
				}
				return MaxSize;
			}
		}
		protected internal double ActualMinSize {
			get {
				if (AutoSize) {
					Tuple<double, double> minAxis = GetMinAxisLengthAndRange();
					if (minAxis != null) {
						double minSize = MinAutoSize * minAxis.Item2;
						double pixelsPerUnit = minAxis.Item1 / minAxis.Item2;
						if (pixelsPerUnit * minSize < MinimumMinSizeInPixels)
							return MinimumMinSizeInPixels / pixelsPerUnit;
						return minSize;
					}
				}
				return MinSize;
			}
		}
		protected override Type PointInterfaceType { get { return typeof(IXYWPoint); } }
		protected internal override bool DateTimeValuesSupported { get { return false; } }
		protected internal override Color ActualColor {
			get {
				Color actualColor = base.ActualColor;
				if (PaletteColorUsed)
					actualColor = ConvertToTransparentColor(actualColor, opacity);
				return actualColor;
			}
		}
		protected internal override Color ActualColor2 {
			get {
				Color color = Color2;
				return color.IsEmpty ? ConvertToTransparentColor(PaletteEntry.Color2, opacity) : color;
			}
		}
		protected internal override int PointDimension { get { return ValueCount; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnBubble); } }
		protected internal override string DefaultPointToolTipPattern {
			get {
				string argumentPattern = "{A" + GetDefaultArgumentFormat() + "}";
				string valuePattern = " : " + "{V" + GetDefaultFormat(Series.ValueScaleType) + "} : {W" + GetDefaultFormat(Series.ValueScaleType) + "}";
				return argumentPattern + valuePattern;
			}
		}
		protected internal override string DefaultLabelTextPattern {
			get { return "{" + PatternUtils.WeightPlaceholder + "}"; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BubbleSeriesViewBubbleMarkerOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BubbleSeriesView.BubbleMarkerOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public MarkerBase BubbleMarkerOptions { get { return Marker; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BubbleSeriesViewAutoSize"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BubbleSeriesView.AutoSize"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool AutoSize {
			get { return autoSize; }
			set {
				if (value != autoSize) {
					SendNotification(new ElementWillChangeNotification(this));
					autoSize = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BubbleSeriesViewMaxSize"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BubbleSeriesView.MaxSize"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double MaxSize {
			get { return maxSize; }
			set {
				if (!Loading && value <= minSize)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBubbleMaxSize));
				if (value == maxSize)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				maxSize = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BubbleSeriesViewMinSize"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BubbleSeriesView.MinSize"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double MinSize {
			get { return minSize; }
			set {
				if (!Loading && (value >= maxSize || value < 0))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBubbleMinSize));
				if (value == minSize)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				minSize = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BubbleSeriesViewTransparency"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BubbleSeriesView.Transparency"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public byte Transparency {
			get { return ConvertBetweenOpacityAndTransparency(opacity); }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				opacity = ConvertBetweenOpacityAndTransparency(value);
				if (!Loading)
					SyncColorsAndTransparency(opacity);
				RaiseControlChanged();
			}
		}
		public BubbleSeriesView() : base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "BubbleMarkerOptions")
				return ShouldSerializeBubbleMarkerOptions();
			if (propertyName == "MaxSize")
				return ShouldSerializeMaxSize();
			if (propertyName == "MinSize")
				return ShouldSerializeMinSize();
			if (propertyName == "Transparency")
				return ShouldSerializeTransparency();
			if (propertyName == "AutoSize")
				return ShouldSerializeAutoSize();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeBubbleMarkerOptions() {
			return BubbleMarkerOptions.ShouldSerialize();
		}
		bool ShouldSerializeMinSize() {
			return minSize != DefaultMinSize;
		}
		void ResetMinSize() {
			MinSize = DefaultMinSize;
		}
		bool ShouldSerializeMaxSize() {
			return maxSize != DefaultMaxSize;
		}
		void ResetMaxSize() {
			MaxSize = DefaultMaxSize;
		}
		bool ShouldSerializeTransparency() {
			return opacity != DefaultOpacity;
		}
		void ResetTransparency() {
			Transparency = ConvertBetweenOpacityAndTransparency(DefaultOpacity);
		}
		bool ShouldSerializeAutoSize() {
			return autoSize != DefaultAutoSize;
		}
		void ResetAutoSize() {
			AutoSize = DefaultAutoSize;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeBubbleMarkerOptions() ||
				ShouldSerializeMaxSize() ||
				ShouldSerializeMinSize() ||
				ShouldSerializeAutoSize() ||
				ShouldSerializeTransparency();
		}
		#endregion
		#region IXYWSeriesView
		double IXYWSeriesView.MaxSize { get { return ActualMaxSize; } }
		double IXYWSeriesView.MinSize { get { return ActualMinSize; } }
		double IXYWSeriesView.GetSideMargins(double min, double max) {
			return AutoSize ? (max - min) * MaxAutoSize : MaxSize;
		}
		#endregion
		void ITransparableView.AssignTransparency(ITransparableView view) {
			opacity = ConvertBetweenOpacityAndTransparency(view.Transparency);
		}
		int CalculatePointRenderSize(XYDiagramMappingBase diagramMapping, RefinedPoint refinedPoint) {
			IXYWPoint bubblePoint = refinedPoint;
			DiagramPoint point = diagramMapping.GetScreenPointNoRound(refinedPoint.Argument, bubblePoint.Value);
			DiagramPoint sidePoint = diagramMapping.GetScreenPointNoRound(refinedPoint.Argument + bubblePoint.Size, bubblePoint.Value);
			return (int)Math.Round(Math.Max(Math.Abs(sidePoint.X - point.X), Math.Abs(sidePoint.Y - point.Y)));
		}
		Tuple<double, double> GetMinAxisLengthAndRange() {
			double axisXRange = ActualAxisX.VisualRange.MaxValueInternal - ActualAxisX.VisualRange.MinValueInternal;
			if (ActualPane.LastMappingBounds.HasValue && !double.IsNaN(axisXRange)) {
				if (ActualPane.LastMappingBounds.Value.Height < ActualPane.LastMappingBounds.Value.Width) {
					double ratio = (double)ActualPane.LastMappingBounds.Value.Height / (double)ActualPane.LastMappingBounds.Value.Width;
					return new Tuple<double, double>(ActualPane.LastMappingBounds.Value.Width * ratio, axisXRange * ratio);
				}
				return new Tuple<double, double>(ActualPane.LastMappingBounds.Value.Width, axisXRange);
			}
			return null;
		}
		protected override void SyncColorsAndTransparency(byte opacity) {
			base.SyncColorsAndTransparency(opacity);
			if (!Color2.IsEmpty)
				Color2 = Color.FromArgb(opacity, Color2);
		}
		protected override SeriesContainer CreateContainer() {
			return new XYWSeriesContainer(this);
		}
		protected override int CalculateCrosshairPolygonSize(XYDiagramMappingBase diagramMapping, RefinedPoint refinedPoint) {
			return CalculatePointRenderSize(diagramMapping, refinedPoint);
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value || valueLevel == ValueLevelInternal.Weight;
		}
		protected internal override Color GetPointColor(int pointIndex, int pointsCount) {
			return Color.FromArgb(opacity, base.GetPointColor(pointIndex, pointsCount));
		}
		protected internal override Color GetPointColor2(int pointIndex, int pointsCount) {
			return Color.FromArgb(opacity, base.GetPointColor2(pointIndex, pointsCount));
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new BubbleSeriesLabel();
		}
		protected internal override ToolTipPointDataToStringConverter CreateToolTipValueToStringConverter() {
			return new ToolTipBubbleValueToStringConverter(Series);
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.BubbleViewPointPatterns;
		}
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(XYDiagramMappingBase diagramMapping, RefinedPointData pointData) {
			PointDrawOptionsBase drawOptions = pointData.DrawOptions as PointDrawOptionsBase;
			IXYWPoint refinedPoint = pointData.RefinedPoint;
			if (drawOptions == null || refinedPoint == null)
				return null;
			DiagramPoint point = diagramMapping.GetScreenPointNoRound(refinedPoint.Argument, refinedPoint.Value);
			int size = CalculatePointRenderSize(diagramMapping, pointData.RefinedPoint);
			if (size > 0) {
				bool isSelected = pointData.SelectionState != SelectionState.Normal;
				IPolygon polygon = drawOptions.Marker.CalculatePolygon(new GRealPoint2D(Math.Round(point.X), Math.Round(point.Y)), isSelected, size);
				return new PointSeriesPointLayout(pointData, point, polygon);
			}
			else
				return null;
		}
		protected override ChartElement CreateObjectForClone() {
			return new BubbleSeriesView();
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IXYWPoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IXYWPoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IXYWPoint)point).Value);
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		public override string GetValueCaption(int index) {
			switch (index) {
				case 0:
					return ChartLocalizer.GetString(ChartStringId.ValueMember);
				case 1:
					return ChartLocalizer.GetString(ChartStringId.WeightMember);
				default:
					throw new DefaultSwitchException();
			}
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ITransparableView transparableView = obj as ITransparableView;
			if (transparableView == null)
				return;
			((ITransparableView)this).AssignTransparency(transparableView);
			BubbleSeriesView view = obj as BubbleSeriesView;
			if (view == null)
				return;
			autoSize = view.autoSize;
			maxSize = view.maxSize;
			minSize = view.minSize;
		}
	}
}

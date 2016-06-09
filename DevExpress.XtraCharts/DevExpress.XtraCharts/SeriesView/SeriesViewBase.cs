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
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	public abstract class SeriesViewBase : ChartElement, ISupportInitialize, IPatternHolder, ISeriesView, IXtraSerializable, IXtraSupportDeserializeCollectionItem, IViewArgumentValueOptions {
		const byte maxTransparency = byte.MaxValue;
		internal const int RandomPointCount = 10;
		static readonly Color defaultColor = Color.Empty;
		protected static Random random = new Random();
		protected static byte ConvertBetweenOpacityAndTransparency(byte value) {
			return (byte)(maxTransparency - value);
		}
		protected static Color ConvertToTransparentColor(Color color, byte opacity) {
			return color.A == opacity ? color : Color.FromArgb(opacity, color);
		}
		bool loading = false;
		Color color = defaultColor;
		DrawOptions customDrawSeriesDrawOptions;
		DrawOptions CrosshairHighlightingDrawOptions {
			get { return customDrawSeriesDrawOptions ?? CreateSeriesDrawOptionsInternal(); }
		}
		protected abstract CompatibleViewType CompatibleViewType { get; }
		protected abstract bool Is3DView { get; }
		protected abstract Type PointInterfaceType { get; }
		protected abstract bool IsCorrectValueLevel(ValueLevelInternal valueLevel);
		protected virtual bool NeedSeriesInteraction { get { return false; } }
		protected virtual bool NeedSeriesGroupsInteraction { get { return false; } }
		protected bool PaletteColorUsed { get { return color.IsEmpty; } }
		protected PaletteEntry PaletteEntry {
			get {
				Chart chart = Chart;
				IList<ISeries> activeSeries = chart.ViewController.GetISeriesForLegendList();
				IList<ISeries> allSeries = chart.ViewController.GetAllSeriesArray();
				if (chart.Legend.UseCheckBoxes)
					return chart.Palette.GetEntry(allSeries.IndexOf(Series), activeSeries.Count, chart.PaletteBaseColorNumber);
				else
					return chart.Palette.GetEntry(activeSeries.IndexOf(Series), activeSeries.Count, chart.PaletteBaseColorNumber);
			}
		}
		protected SeriesBase SeriesBase { get { return (SeriesBase)base.Owner; } }
		internal IChartAppearance ActualAppearance {
			get {
				IChartAppearance appearance = CommonUtils.GetActualAppearance(Owner);
				if (appearance == null)
					appearance = AppearanceRepository.Default;
				return appearance;
			}
		}
		internal SeriesHitTestState HitTestState {
			get { return Series.HitState; }
		}
		protected internal abstract bool NeedFilterVisiblePoints { get; }
		protected internal abstract bool HitTestingSupportedForLegendMarker { get; }
		protected internal abstract string StringId { get; }
		protected internal abstract bool DateTimeValuesSupported { get; }
		protected internal virtual bool AnnotationLabelModeSupported { get { return false; } }
		protected internal virtual string ImageNameBase { get { return SeriesViewFactory.GetViewType(this).ToString().ToLower(); } }
		protected internal virtual bool ShouldSortPoints { get { return true; } }
		protected internal virtual bool IsSupportedRelations { get { return false; } }
		protected internal virtual bool IsSupportedLabel { get { return true; } }
		protected internal virtual bool IsSupportedPointOptions { get { return true; } }
		protected internal virtual bool IsSupportedToolTips { get { return false; } }
		protected internal virtual bool IsSupportedCrosshair { get { return false; } }
		protected internal virtual bool SerializeSeriesPointID { get { return false; } }
		protected internal virtual bool ShouldCalculatePointsData { get { return true; } }
		protected internal virtual int PointDimension { get { return 1; } }
		protected internal virtual bool NonNumericArgumentSupported { get { return true; } }
		protected internal HitTestController HitTestController {
			get {
				ChartDebug.Assert(Series != null, "Series can not be null");
				return Series.HitTestController;
			}
		}
		protected internal virtual bool SideMarginsEnabled { get { return true; } }
		protected internal virtual bool ActualSideMarginsEnabled { get { return true; } }
		protected internal Series Series { get { return (Series)base.Owner; } }
		protected internal Chart Chart {
			get { return (base.Owner != null) ? ((SeriesBase)base.Owner).Chart : null; }
		}
		protected internal override bool Loading { get { return loading || base.Loading; } }
		protected internal SeriesLabelBase Label { get { return SeriesBase.Label; } }
		protected internal abstract bool ActualColorEach { get; }
		protected internal virtual Color ActualColor {
			get { return PaletteColorUsed ? PaletteEntry.Color : color; }
		}
		protected internal virtual Color ActualColor2 {
			get { return PaletteEntry.Color2; }
		}
		protected internal virtual ValueLevel[] SupportedValueLevels { get { return new ValueLevel[] { ValueLevel.Value }; } }
		protected internal virtual ValueLevel DefaultValueLevel { get { return SupportedValueLevels[0]; } }
		protected internal virtual string DefaultPointToolTipPattern {
			get {
				string argumentPattern = "{A" + GetDefaultArgumentFormat() + "}";
				string valuePattern = " : " + "{V" + GetDefaultFormat(Series.ValueScaleType) + "}";
				return argumentPattern + valuePattern;
			}
		}
		protected internal virtual string DefaultLabelTextPattern {
			get { return "{" + PatternUtils.ValuePlaceholder + "}"; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public abstract Type DiagramType { get; }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesViewBaseValuesCount"),
#endif
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public int ValuesCount { get { return PointDimension; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesViewBaseColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesViewBase.Color"),
		Category("Appearance"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color Color {
			get { return color; }
			set {
				if (value != color) {
					SendNotification(new ElementWillChangeNotification(this));
					color = value;
					RaiseControlChanged();
				}
			}
		}
		[
		DefaultValue(null),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Obsolete("This property is obsolete now.")
		]
		public object HiddenObject { get { return null; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(""),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Obsolete("This property is obsolete now.")
		]
		public string HiddenSerializableString { get { return String.Empty; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return this.GetType().Name; } }
		protected SeriesViewBase()
			: base() {
		}
		#region ISupportInitialize implementation
		void ISupportInitialize.BeginInit() {
			this.loading = true;
		}
		void ISupportInitialize.EndInit() {
			this.loading = false;
		}
		#endregion
		#region ISeriesView implementation
		Type ISeriesView.PointInterfaceType {
			get {
				return this.PointInterfaceType;
			}
		}
		CompatibleViewType ISeriesView.CompatibleViewType {
			get { return CompatibleViewType; }
		}
		bool ISeriesView.ShouldSortPoints {
			get { return this.ShouldSortPoints; }
		}
		bool ISeriesView.Is3DView {
			get {
				return this.Is3DView;
			}
		}
		bool ISeriesView.NeedFilterVisiblePoints {
			get {
				return this.NeedFilterVisiblePoints;
			}
		}
		bool ISeriesView.NeedSeriesInteraction {
			get { return NeedSeriesInteraction; }
		}
		bool ISeriesView.NeedSeriesGroupsInteraction {
			get { return NeedSeriesGroupsInteraction; }
		}
		SeriesContainer ISeriesView.CreateContainer() {
			return CreateContainer();
		}
		SeriesInteractionContainer ISeriesView.CreateSeriesGroupsContainer() {
			return CreateSeriesGroupsContainer();
		}
		double ISeriesView.GetRefinedPointMax(RefinedPoint point) {
			return this.GetRefinedPointMax(point);
		}
		double ISeriesView.GetRefinedPointMin(RefinedPoint point) {
			return this.GetRefinedPointMin(point);
		}
		double ISeriesView.GetRefinedPointAbsMin(RefinedPoint point) {
			return this.GetRefinedPointAbsMin(point);
		}
		double ISeriesView.GetRefinedPointsMin(IList<RefinedPoint> points) {
			double min = double.MaxValue;
			for (int i = 0; i < points.Count; i++)
				min = Math.Min(min, ((ISeriesView)this).GetRefinedPointMin(points[i]));
			return min;
		}
		double ISeriesView.GetRefinedPointsMax(IList<RefinedPoint> points) {
			double max = double.MinValue;
			foreach (RefinedPoint point in points)
				max = Math.Max(max, ((ISeriesView)this).GetRefinedPointMin(point));
			return max;
		}
		IList<ISeriesPoint> ISeriesView.GenerateRandomPoints(Scale argumentScaleType, Scale valueScaleType) {
			return GenerateRandomPoints((ScaleType)argumentScaleType, (ScaleType)valueScaleType, RandomPointCount);
		}
		RangeValue ISeriesView.GetMinMax(IPointInteraction interaction, int index) {
			return RangeValue.Empty;
		}
		bool ISeriesView.IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return IsCorrectValueLevel(valueLevel);
		}
		MinMaxValues ISeriesView.CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram, CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CalculateMinMaxPointRangeValues(point, range, isHorizontalCrosshair, diagram, crosshairPaneInfo, snapMode);
		}
		#endregion
		#region IViewArgumentValueSupported implementation
		bool IViewArgumentValueOptions.NonNumericArgumentSupported { get { return NonNumericArgumentSupported; } }
		bool IViewArgumentValueOptions.DateTimeValuesSupported { get { return DateTimeValuesSupported; } }
		int IViewArgumentValueOptions.PointDimension { get { return PointDimension; } }
		bool IViewArgumentValueOptions.IsSupportedPointOptions { get { return IsSupportedPointOptions; } }
		ValueLevel[] IViewArgumentValueOptions.SupportedValueLevels { get { return SupportedValueLevels; } }
		#endregion
		#region IPatternHolder implementation
		PatternDataProvider IPatternHolder.GetDataProvider(PatternConstants patternConstant) {
			return GetDataProvider(patternConstant);
		}
		string IPatternHolder.PointPattern {
			get {
				if (!string.IsNullOrEmpty(this.Series.ToolTipPointPattern))
					return this.Series.ToolTipPointPattern;
				return DefaultPointToolTipPattern;
			}
		}
		#endregion
		#region Should Serialize
		bool ShouldSerializeColor() {
			return color != defaultColor;
		}
		void ResetColor() {
			Color = defaultColor;
		}
		bool ShouldSerializeHiddenObject() {
			return false;
		}
		bool ShouldSerializeHiddenSerializableString() {
			return false;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeColor() || SeriesViewFactory.GetViewType(this) != SeriesViewFactory.DefaultViewType;
		}
		#endregion
		#region IXtraSerializable
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			((ISupportInitialize)this).BeginInit();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((ISupportInitialize)this).EndInit();
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			SetIndexCollectionItemOnXtraDeserializing(propertyName, e);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return CreateCollectionItemOnXtraDeserializing(propertyName, e);
		}
		protected virtual void SetIndexCollectionItemOnXtraDeserializing(string propertyName, XtraSetItemIndexEventArgs e) {
		}
		protected virtual object CreateCollectionItemOnXtraDeserializing(string propertyName, XtraItemEventArgs e) {
			return null;
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Color")
				return ShouldSerializeColor();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		DateTimeMeasureUnit GetArgumentDateTimeMeasureUnit() {
			AxisBase axis = GetAxisX();
			return axis != null ? axis.DateTimeScaleOptions.MeasureUnit : DateTimeMeasureUnit.Day;
		}
		DateTimeMeasureUnit GetValueDateTimeMeasureUnit() {
			AxisBase axis = GetAxisY();
			return axis != null ? axis.DateTimeScaleOptions.MeasureUnit : DateTimeMeasureUnit.Day;
		}
		protected abstract PatternDataProvider GetDataProvider(PatternConstants patternConstant);
		protected abstract double GetRefinedPointMax(RefinedPoint point);
		protected abstract double GetRefinedPointMin(RefinedPoint point);
		protected abstract double GetRefinedPointAbsMin(RefinedPoint point);
		protected abstract void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState);
		protected abstract DrawOptions CreateSeriesDrawOptionsInternal();
		protected virtual SeriesContainer CreateContainer() {
			return new XYSeriesContainer(this);
		}
		protected virtual SeriesInteractionContainer CreateSeriesGroupsContainer() {
			return new SideBySideInteractionContainer(this);
		}
		protected virtual void SyncColorsAndTransparency(byte opacity) {
			if (!color.IsEmpty)
				SetColor(Color.FromArgb(opacity, color));
		}
		protected virtual Rectangle CorrectLegendMarkerBounds(Rectangle bounds) {
			return bounds;
		}
		protected virtual void RenderCrosshairMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions) {
			RenderLegendMarkerInternal(renderer, bounds, seriesPointDrawOptions, seriesDrawOptions, SelectionState.Normal);
		}
		protected void SetColor(Color color) {
			this.color = color;
		}
		protected PatternDataProvider GetXYDataProvider(PatternConstants patternConstant) {
			if (Series != null) {
				IXYSeriesView xyView = ((IXYSeriesView)this);
				IAxisData axisXData = xyView.AxisXData;
				IAxisData axisYData = xyView.AxisYData;
				switch (patternConstant) {
					case PatternConstants.Argument:
						return new PointPatternDataProvider(patternConstant, axisXData != null ? axisXData.AxisScaleTypeMap : null);
					case PatternConstants.Value:
					case PatternConstants.PercentValue:
					case PatternConstants.Value1:
					case PatternConstants.Value2:
					case PatternConstants.OpenValue:
					case PatternConstants.CloseValue:
					case PatternConstants.HighValue:
					case PatternConstants.LowValue:
					case PatternConstants.Weight:
					case PatternConstants.PointHint:
					case PatternConstants.ValueDuration:
						return new PointPatternDataProvider(patternConstant, axisYData != null ? axisYData.AxisScaleTypeMap : null);
					case PatternConstants.Series:
					case PatternConstants.SeriesGroup:
						return new SeriesPatternDataProvider(patternConstant);
				}
			}
			return null;
		}
		protected IPolygon CalculateMarkerPolygon(RefinedPointData pointData, DiagramPoint point) {
			bool isSelected = pointData != null && pointData.RefinedPoint != null && pointData.SelectionState != SelectionState.Normal;
			return ((PointDrawOptions)pointData.DrawOptions).ActualMarkerVisible ? ((PointDrawOptions)pointData.DrawOptions).Marker.CalculatePolygon(new GRealPoint2D(Math.Round(point.X), Math.Round(point.Y)), isSelected) : null;
		}
		protected string GetDefaultFormat(ScaleType scaleType) {
			if (scaleType == ScaleType.DateTime)
				return ":d";
			return "";
		}
		protected string GetDefaultArgumentFormat() {
			IXYSeriesView view = Series.View as IXYSeriesView;
			if (view != null && view.AxisXData != null && Series.ArgumentScaleType == ScaleType.DateTime && view.AxisXData.Label != null)
				return ":" + PatternUtils.GetArgumentFormat(view.AxisXData.Label.TextPattern);
			return GetDefaultFormat(Series.ArgumentScaleType);
		}
		protected virtual AxisBase GetAxisX() {
			return null;
		}
		protected virtual AxisBase GetAxisY() {
			return null;
		}
		internal virtual SeriesHitTestState CreateHitTestState() {
			return new PointHitTestState();
		}
		internal DrawOptions CreatePointDrawOptions(IRefinedSeries refinedSeries, RefinedPoint point) {
			DrawOptions drawOptions = CrosshairHighlightingDrawOptions;
			DrawOptions pointDrawOptions;
			if (ActualColorEach) {
				pointDrawOptions = (DrawOptions)drawOptions.Clone();
				int pointIndex = Math.Max(refinedSeries.Points.IndexOf(point), 0);
				pointDrawOptions.InitializeSeriesPointDrawOptions(this, refinedSeries, pointIndex);
			}
			else
				pointDrawOptions = drawOptions;
			return pointDrawOptions;
		}
		internal IList<ISeriesPoint> GenerateRandomPoints(ScaleType argumentScaleType, ScaleType valueScaleType, int pointsCount) {
			List<ISeriesPoint> points = new List<ISeriesPoint>();
			switch (argumentScaleType) {
				case ScaleType.DateTime:
					DateTime now = DateTime.Now;
					DateTimeMeasureUnitNative dateTimeUnit = (DateTimeMeasureUnitNative)GetArgumentDateTimeMeasureUnit();
					for (int i = 0; i < pointsCount; i++) {
						object[] values = GenerateRandomValues(i, pointsCount, valueScaleType);
						points.Add(new SeriesPoint(DateTimeUtils.AddRange(dateTimeUnit, dateTimeUnit, now, i, null).ToString(), values));
					}
					break;
				case ScaleType.Numerical:
					for (int i = 0; i < pointsCount; i++) {
						object[] values = GenerateRandomValues(i, pointsCount, valueScaleType);
						points.Add(new SeriesPoint(((int)Math.Round(GetRandomNumericalArgument(i, pointsCount))).ToString(), values));
					}
					break;
				case ScaleType.Qualitative:
					for (int i = 0; i < pointsCount; i++) {
						object[] values = GenerateRandomValues(i, pointsCount, valueScaleType);
						points.Add(new SeriesPoint(((char)((int)'A' + i)).ToString(), values));
					}
					break;
			}
			return points;
		}
		internal void RenderCrosshairMarker(IRenderer renderer, Rectangle bounds, IRefinedSeries refinedSeries, RefinedPoint point, Color markerColor) {
			DrawOptions drawOptions = CreateSeriesDrawOptionsInternal();
			drawOptions.Color = markerColor;
			drawOptions.ActualColor2 = markerColor;
			DrawOptions pointDrawOptions = CreatePointDrawOptions(refinedSeries, point);
			SeriesPoint seriesPoint = point.SeriesPoint as SeriesPoint;
			if (seriesPoint != null && !seriesPoint.Color.IsEmpty) {
				pointDrawOptions.Color = seriesPoint.Color;
				pointDrawOptions.ActualColor2 = seriesPoint.Color;
			}
			else {
				pointDrawOptions.Color = markerColor;
				pointDrawOptions.ActualColor2 = markerColor;
			}
			RenderCrosshairMarkerInternal(renderer, bounds, pointDrawOptions, drawOptions);
		}
		internal void RenderLegendMarker(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, RefinedSeriesData seriesData, RefinedPointData pointData) {
			if (!GraphicUtils.CheckIsSizePositive(bounds.Size))
				return;
			Image markerImage = null;
			ChartImageSizeMode markerImageSizeMode = ChartImageSizeMode.AutoSize;
			bool disposeImage = false;
			if (pointData != null) {
				markerImage = pointData.MarkerImage;
				markerImageSizeMode = pointData.MarkerImageSizeMode;
				disposeImage = pointData.DisposeMarkerImage;
			}
			else if (seriesData != null) {
				markerImage = seriesData.MarkerImage;
				markerImageSizeMode = seriesData.MarkerImageSizeMode;
				disposeImage = seriesData.DisposeMarkerImage;
			}
			if (markerImage != null) {
				renderer.DrawImage(markerImage, bounds, markerImageSizeMode);
				if (disposeImage && (markerImage != null))
					markerImage.Dispose();
			}
			else {
				bounds = CorrectLegendMarkerBounds(bounds);
				if (GraphicUtils.CheckIsSizePositive(bounds.Size))
					RenderLegendMarkerInternal(renderer, bounds, seriesPointDrawOptions, seriesDrawOptions, pointData != null ? pointData.SelectionState : Series.HitState.LegendSelectionState);
			}
		}
		internal void CheckValueLevel(ValueLevel valueLevel) {
			if (!Loading) {
				foreach (ValueLevel supportedValueLevel in SupportedValueLevels)
					if (valueLevel == supportedValueLevel)
						return;
				string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgUnsupportedValueLevel),
					new ValueLevelItem(valueLevel).ToString(), StringId);
				throw new ArgumentException(message);
			}
		}
		internal bool EqualsByType(SeriesViewBase view) {
			if (view == null)
				return false;
			return view.GetType().Equals(this.GetType());
		}
		internal int GetLegendItemCount() {
			return ActualColorEach ? ((ISeries)Series).ActualPoints.Count : 1;
		}
		internal DrawOptions CreateSeriesDrawOptions() {
			DrawOptions drawOptions = CreateSeriesDrawOptionsInternal();
			if (ContainerAdapter != null && ContainerAdapter.ShouldCustomDrawSeries)
				customDrawSeriesDrawOptions = drawOptions;
			return drawOptions;
		}
		internal virtual string LabelPatternToLegendPattern() {
			if (SeriesBase != null && Label != null)
				return Label.ActualTextPattern;
			else
				return DefaultLabelTextPattern;
		}
		protected internal abstract GraphicsCommand CreateGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions);
		protected internal abstract void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions);
		protected internal abstract GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions);
		protected internal abstract void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions);
		protected internal abstract SeriesLabelBase CreateSeriesLabel();
		protected internal virtual void CheckArgumentScaleType(ScaleType argumentScaleType) { }
		protected internal virtual void SetArgumentScaleType(ScaleType argumentScaleType) { }
		protected internal virtual void OnEndLoading() { }
		protected internal virtual void CheckOnAddingSeries() { }
		protected internal virtual PointOptions CreatePointOptions() {
			return new PointOptions();
		}
		protected internal virtual object[] GenerateRandomValues(int indexOfRandomPoint, int randomPointCount, ScaleType valueScaleType) {
			object[] values = new object[PointDimension];
			if (valueScaleType == ScaleType.DateTime) {
				DateTime now = DateTime.Now;
				DateTimeMeasureUnitNative dateTimeUnit = (DateTimeMeasureUnitNative)GetValueDateTimeMeasureUnit();
				for (int i = 0; i < values.Length; i++)
					values[i] = DateTimeUtils.AddRange(dateTimeUnit, dateTimeUnit, now, random.NextDouble() * 10, null);
			}
			else
				for (int i = 0; i < values.Length; i++)
					values[i] = Math.Round(random.NextDouble() * 10, 1);
			return values;
		}
		protected internal virtual double GetRandomNumericalArgument(int pointIndex, int pointCount) {
			return pointIndex;
		}
		protected internal virtual ToolTipPointDataToStringConverter CreateToolTipValueToStringConverter() {
			return new ToolTipValueToStringConverter(Series);
		}
		protected internal virtual string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.BaseViewPointPatterns;
		}
		protected internal virtual string[] GetAvailableSeriesPatternPlaceholders() {
			return ToolTipPatternUtils.BaseViewSeriesPatterns;
		}
		protected internal virtual void CopySettings(ChartElement element) {
			Assign(element);
		}
		protected internal virtual Color GetPointColor(int pointIndex, int pointsCount) {
			return (ActualColorEach && pointIndex >= 0) ?
				Chart.Palette.GetEntry(pointIndex, pointsCount, Chart.PaletteBaseColorNumber).Color : ActualColor;
		}
		protected internal virtual Color GetPointColor2(int pointIndex, int pointsCount) {
			PaletteEntry entry = (ActualColorEach && pointIndex >= 0) ?
				Chart.Palette.GetEntry(pointIndex, pointsCount, Chart.PaletteBaseColorNumber) : PaletteEntry;
			return entry.Color2;
		}
		protected internal virtual WholeSeriesViewData CalculateWholeSeriesViewData(RefinedSeriesData seriesData, GeometryCalculator geometryCalculator) {
			return null;
		}
		protected internal virtual void RenderWholeSeries(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) { }
		protected internal virtual GraphicsCommand CreateWholeSeriesShadowGraphicsCommand(Rectangle mappingBounds, WholeSeriesLayout layout) {
			return null;
		}
		protected internal virtual void RenderWholeSeriesShadow(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) { }
		protected internal virtual void FillPrimitivesContainer(SeriesPointLayout pointLayout, PrimitivesContainer container) { }
		protected internal virtual void SynchronizePoints() { }
		protected internal virtual MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram,
			CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CrosshairManager.CalculateMinMaxMarkerSeriesRangeValues(point, range, isHorizontalCrosshair);
		}
		protected internal virtual bool Contains(object obj) {
			return false;
		}
		protected internal virtual bool IsCompatible(SeriesViewBase view) {
			return view.DiagramType == DiagramType;
		}
		protected internal bool ShouldAddSeriesPointsInLegend() {
			return (ActualColorEach && !Series.Chart.Legend.UseCheckBoxes) ||
				   (ActualColorEach && Series.Chart.Legend.UseCheckBoxes && !Series.CheckableInLegend);
		}
		public virtual string GetValueCaption(int index) {
			if (index >= PointDimension)
				throw new IndexOutOfRangeException();
			return ChartLocalizer.GetString(ChartStringId.ValueMember) + " " + (index + 1).ToString();
		}
		public override string ToString() {
			return StringId;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override void Assign(ChartElement element) {
			base.Assign(element);
			SeriesViewBase view = element as SeriesViewBase;
			if (view != null) {
				this.Color = view.Color;
			}
		}
		public override bool Equals(object obj) {
			SeriesViewBase view = obj as SeriesViewBase;
			return
				view != null &&
				view.GetType().Equals(GetType()) &&
				view.color == this.Color;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public static class SeriesViewHelper {
		public static bool IsSupportedRelations(SeriesViewBase view) {
			return view.IsSupportedRelations;
		}
	}
}

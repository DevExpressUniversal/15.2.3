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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[TypeConverter(typeof(AxisBaseTypeConverter))]
	public abstract class AxisBase : ChartElementNamed, IAxisData, ILogarithmic, IHitTest {
		const double DefaultLogarithmicBase = 10.0;
		const bool DefaultLogarithmic = false;
		static readonly Color DefaultInterlacedColor =  Color.Empty;
		internal const double DefaultSideMarginValue = 0.5;
#pragma warning disable 618
		[Obsolete("This constant is obsolete now. Use the AxisBase.DateTimeScaleOptions.DefaultMeasureUnit constant instead.")]
		public const DateTimeMeasurementUnit DefaultDateTimeMeasurementUnit = (DateTimeMeasurementUnit)DevExpress.XtraCharts.DateTimeMeasureUnit.Day;
#pragma warning restore 618
		readonly AxisLabel label;
		readonly AxisRange range;
		readonly GridLines gridLines;
		readonly NumericScaleOptions numericScaleOptions;
		readonly DateTimeScaleOptions dateTimeScaleOptions;
		readonly QualitativeScaleOptions qualitativeScaleOptions;
		readonly HitTestState hitTestState = new HitTestState();
		int minorCount;
		double sideMarginValue = DefaultSideMarginValue;
		bool interlaced;
		Color interlacedColor = DefaultInterlacedColor;
		Scale scale = Scale.Numerical;
		bool logarithmic = DefaultLogarithmic;
		double logarithmicBase = DefaultLogarithmicBase;
		IComparer qualitativeScaleComparer;
		VisualRangeData visualRangeData;
		WholeRangeData wholeRangeData;
		VisualRange visualRange;
		WholeRange wholeRange;
		AxisScaleTypeMap axisScaleTypeMap;
		bool showMajorGridlines;
		bool showMajorTickmarks;
		bool showMinorGridlines;
		bool showMinorTickmarks;
		internal Diagram Diagram { get { return (Diagram)base.Owner; } }
		internal ActualScaleType ScaleType { get { return axisScaleTypeMap.ScaleType; } }
		internal AxisScaleTypeMap ScaleTypeMap { get { return axisScaleTypeMap; } private set { axisScaleTypeMap = value; } }
		internal bool IsSmartAxis { get { return ActualScaleOptions.IsSmartScale; } }
		protected abstract int GridSpacingFactor { get; }
		protected abstract int DefaultMinorCount { get; }
		protected virtual bool DefaultInterlaced { get { return false; } }
		protected virtual bool IsRadarAxis { get { return false; } }
		protected virtual IEnumerable<IScaleBreak> ScaleBreaksEnumeration { get { return null; } }
		protected virtual IEnumerable<IStrip> StripsEnumeration { get { return null; } }
		protected virtual IEnumerable<IConstantLine> ConstantLinesEnumeration { get { return null; } }
		protected virtual IEnumerable<ICustomAxisLabel> CustomLabelsEnumeration { get { return null; } }
		protected virtual AxisVisibilityInPanes ActualVisibilityInPanes { get { return null; } }
		protected virtual bool CanShowCustomWithAutoLabels { get { return false; } }
		protected internal abstract bool IsValuesAxis { get; }
		protected internal abstract bool IsVertical { get; }
		protected internal virtual bool ActualReverse { get { return false; } }
		protected internal virtual ScaleMode ActualDateTimeScaleMode { get { return IsValuesAxis ? ScaleMode.Continuous : ScaleMode.Manual; } }
		protected internal virtual ScaleMode DefaultDateTimeScaleMode { get { return IsValuesAxis ? ScaleMode.Continuous : ScaleMode.Manual; } }
		protected internal virtual IVisualAxisRangeData VisualRangeData { get { return visualRangeData; } }
		protected internal virtual IWholeAxisRangeData WholeRangeData { get { return wholeRangeData; } }
		internal ScaleOptionsBase ActualScaleOptions {
			get {
				switch (ScaleType) {
					case ActualScaleType.DateTime:
						return DateTimeScaleOptions;
					case ActualScaleType.Numerical:
						return NumericScaleOptions;
					case ActualScaleType.Qualitative:
						return qualitativeScaleOptions;
					default:
						return null;
				}
			}
		}
		internal AxisLabel ActualLabel { get { return label; } }
		#region Obsolete Properties
		[
		Obsolete("This property is obsolete now. Use DateTimeScaleOptions.MeasureUnit instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public DateTimeMeasurementUnit DateTimeMeasureUnit {
			get { return (DateTimeMeasurementUnit)dateTimeScaleOptions.MeasureUnit; }
			set { dateTimeScaleOptions.MeasureUnit = (DateTimeMeasureUnit)value; }
		}
		[
		Obsolete("This property is obsolete now. Use DateTimeScaleOptions.GridAlignment instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public DateTimeMeasurementUnit DateTimeGridAlignment {
			get { return (DateTimeMeasurementUnit)dateTimeScaleOptions.GridAlignment; }
			set { dateTimeScaleOptions.GridAlignment = (DateTimeGridAlignment)value; }
		}
		[
		Obsolete("This property is obsolete now. Use DateTimeScaleOptions.WorkdaysOnly instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public bool WorkdaysOnly {
			get { return dateTimeScaleOptions.WorkdaysOnly; }
			set { dateTimeScaleOptions.WorkdaysOnly = value; }
		}
		[
		Obsolete("This property is obsolete now. Use DateTimeScaleOptions.WorkdaysOptions instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public WorkdaysOptions WorkdaysOptions { get { return dateTimeScaleOptions.WorkdaysOptions; } }
		[
		Obsolete("This property is obsolete now. Use AxisBase.Label.NumericOptions instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public NumericOptions NumericOptions { get { return ActualLabel.NumericOptions; } }
		[
		Obsolete("This property is obsolete now. Use AxisX.Label.DateTimeOptions or AxisY.Label.DateTimeOptions instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public DateTimeOptions DateTimeOptions { get { return ActualLabel.DateTimeOptions; } }
		[
		Obsolete("This property is obsolete now. Use the NumericScaleOptions.AutoGrid and DateTimeScaleOptions.AutoGrid properties instead for the  numeric and date-time scales."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public virtual bool GridSpacingAuto {
			get { return ActualScaleOptions != null ? ActualScaleOptions.AutoGrid : true; }
			set {
				dateTimeScaleOptions.AutoGrid = value;
				numericScaleOptions.AutoGrid = value;
				qualitativeScaleOptions.AutoGrid = value;
			}
		}
		[
		Obsolete("This property is obsolete now. Use the NumericScaleOptions.GridSpacing and DateTimeScaleOptions.GridSpacing properties instead for the  numeric and date-time scales."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public virtual double GridSpacing {
			get { return ActualScaleOptions != null ? ActualScaleOptions.GridSpacing : ScaleOptionsBase.DefaultGridSpacing; }
			set {
				dateTimeScaleOptions.GridSpacing = value;
				numericScaleOptions.GridSpacing = value;
				qualitativeScaleOptions.GridSpacing = value;
			}
		}
		[
		Obsolete("This property is obsolete now. To specify a custom range, use the AxisBase.VisualRange and AxisBase.WholeRange properties instead. For more information, see the corresponding topic in the documentation."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseRange"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.Range"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		NonTestableProperty
		]
		public AxisRange Range { get { return range; } }
		#endregion
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseVisualRange"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.VisualRange"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		NonTestableProperty
		]
		public VisualRange VisualRange { get { return visualRange; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseWholeRange"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.WholeRange"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		NonTestableProperty
		]
		public WholeRange WholeRange { get { return wholeRange; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseGridLines"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.GridLines"),
		Category(Categories.Elements),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public GridLines GridLines { get { return gridLines; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseNumericScaleOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.NumericScaleOptions"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		NonTestableProperty
		]
		public NumericScaleOptions NumericScaleOptions { get { return numericScaleOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseDateTimeScaleOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.DateTimeScaleOptions"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public DateTimeScaleOptions DateTimeScaleOptions { get { return dateTimeScaleOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseMinorCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.MinorCount"),
		XtraSerializableProperty
		]
		public int MinorCount {
			get { return minorCount; }
			set {
				if (value < 1 || value > 99)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectMinorCount));
				if (value != minorCount) {
					SendNotification(new ElementWillChangeNotification(this));
					minorCount = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseInterlaced"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.Interlaced"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public bool Interlaced {
			get { return interlaced; }
			set {
				if (value != interlaced) {
					SendNotification(new ElementWillChangeNotification(this));
					interlaced = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseInterlacedColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.InterlacedColor"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public Color InterlacedColor {
			get { return interlacedColor; }
			set {
				if (value != interlacedColor) {
					SendNotification(new ElementWillChangeNotification(this));
					interlacedColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseLogarithmic"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.Logarithmic"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		NonTestableProperty,
		RefreshProperties(RefreshProperties.All),
		Category(Categories.Behavior)
		]
		public virtual bool Logarithmic {
			get { return logarithmic; }
			set {
				if (value != logarithmic) {
					SendNotification(new ElementWillChangeNotification(this));
					logarithmic = value;
					this.axisScaleTypeMap.BuildTransformation(this);
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseLogarithmicBase"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.LogarithmicBase"),
		XtraSerializableProperty,
		NonTestableProperty,
		Category(Categories.Behavior)
		]
		public virtual double LogarithmicBase {
			get { return logarithmicBase; }
			set {
				if (value <= 1.0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidLogarithmicBase));
				if (value != logarithmicBase) {
					SendNotification(new ElementWillChangeNotification(this));
					logarithmicBase = value;
					this.axisScaleTypeMap.BuildTransformation(this);
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseLabel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisBase.Label"),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public AxisLabel Label { get { return ActualLabel; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisBaseQualitativeScaleComparer"),
#endif
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public IComparer QualitativeScaleComparer {
			get { return qualitativeScaleComparer; }
			set {
				if (value != qualitativeScaleComparer) {
					SendNotification(new ElementWillChangeNotification(this));
					qualitativeScaleComparer = value;
					RaiseControlChanged(new PropertyUpdateInfo(this, "QualitativeScaleComparer"));
				}
			}
		}
		protected AxisBase(string name, Diagram diagram) : base(name, diagram) {
			label = CreateAxisLabel();
			visualRangeData = new VisualRangeData(this);
			wholeRangeData = new WholeRangeData(this);
			wholeRange = new WholeRange(wholeRangeData);
			visualRange = new VisualRange(visualRangeData);
			axisScaleTypeMap = new AxisNumericalMap();
			range = CreateAxisRange(wholeRangeData, visualRangeData);
			gridLines = CreateGridLines();
			numericScaleOptions = new NumericScaleOptions(this);
			dateTimeScaleOptions = new DateTimeScaleOptions(this);
			qualitativeScaleOptions = new QualitativeScaleOptions(this);
			minorCount = DefaultMinorCount;
			interlaced = DefaultInterlaced;
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeRange() {
			return false;
		}
		bool ShouldSerializeVisualRange() {
			return visualRange.ShouldSerialize();
		}
		bool ShouldSerializeWholeRange() {
			return wholeRange.ShouldSerialize();
		}
		bool ShouldSerializeGridLines() {
			return gridLines.ShouldSerialize();
		}
		bool ShouldSerializeNumericScaleOptions() {
			return numericScaleOptions.ShouldSerialize();
		}
		bool ShouldSerializeDateTimeScaleOptions() {
			return dateTimeScaleOptions.ShouldSerialize();
		}
		bool ShouldSerializeGridSpacingAuto() {
			return false;
		}
		bool ShouldSerializeGridSpacing() {
			return false;
		}
		bool ShouldSerializeWorkdaysOptions() {
			return false;
		}
		bool ShouldSerializeDateTimeOptions() {
			return false;
		}
		bool ShouldSerializeNumericOptions() {
			return false;
		}
		bool ShouldSerializeDateTimeMeasureUnit() {
			return false;
		}
		bool ShouldSerializeDateTimeGridAlignment() {
			return false;
		}
		bool ShouldSerializeWorkdaysOnly() {
			return false;
		}
		bool ShouldSerializeMinorCount() {
			return minorCount != DefaultMinorCount;
		}
		void ResetMinorCount() {
			MinorCount = DefaultMinorCount;
		}
		bool ShouldSerializeInterlaced() {
			return interlaced != DefaultInterlaced;
		}
		void ResetInterlaced() {
			Interlaced = DefaultInterlaced;
		}
		bool ShouldSerializeInterlacedColor() {
			return interlacedColor != DefaultInterlacedColor;
		}
		void ResetInterlacedColor() {
			InterlacedColor = DefaultInterlacedColor;
		}
		bool ShouldSerializeLogarithmic() {
			return logarithmic != DefaultLogarithmic;
		}
		void ResetLogarithmic() {
			Logarithmic = DefaultLogarithmic;
		}
		bool ShouldSerializeLogarithmicBase() {
			return ShouldSerializeLogarithmic() && logarithmicBase != DefaultLogarithmicBase;
		}
		void ResetLogarithmicBase() {
			LogarithmicBase = DefaultLogarithmicBase;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize()
				|| ShouldSerializeRange()
				|| ShouldSerializeVisualRange()
				|| ShouldSerializeWholeRange()
				|| ShouldSerializeGridLines()
				|| ShouldSerializeGridSpacingAuto()
				|| ShouldSerializeGridSpacing()
				|| ShouldSerializeMinorCount()
				|| ShouldSerializeInterlaced()
				|| ShouldSerializeInterlacedColor()
				|| ShouldSerializeLogarithmic()
				|| ShouldSerializeLogarithmicBase()
				|| ShouldSerializeNumericScaleOptions()
				|| ShouldSerializeDateTimeScaleOptions();
		}
		#endregion
		#region XtraShouldSerialize
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Range":
					return ShouldSerializeRange();
				case "GridLines":
					return ShouldSerializeGridLines();
				case "NumericOptions":
					return false;
				case "DateTimeOptions":
					return false;
				case "GridSpacingAuto":
					return false;
				case "GridSpacing":
					return false;
				case "MinorCount":
					return ShouldSerializeMinorCount();
				case "Interlaced":
					return ShouldSerializeInterlaced();
				case "InterlacedColor":
					return ShouldSerializeInterlacedColor();
				case "Logarithmic":
					return ShouldSerializeLogarithmic();
				case "LogarithmicBase":
					return ShouldSerializeLogarithmicBase();
				case "NumericScaleOptions":
					return ShouldSerializeNumericScaleOptions();
				case "DateTimeScaleOptions":
					return ShouldSerializeDateTimeScaleOptions();
				case "QualitativeScaleComparer":
					return false;
				case "WorkdaysOnly":
					return false;
				case "DateTimeMeasureUnit":
					return false;
				case "DateTimeGridAlignment":
					return false;
				case "WorkdaysOptions":
					return false;
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ILogarithmic
		bool ILogarithmic.Enabled { get { return logarithmic; } }
		double ILogarithmic.Base { get { return logarithmicBase; } }
		#endregion
		#region IHitTest
		object IHitTest.Object { get { return this; } }
		HitTestState IHitTest.State { get { return hitTestState; } }
		#endregion
		#region IAxisElementContainer
		IEnumerable<IScaleBreak> IAxisElementContainer.ScaleBreaks { get { return ScaleBreaksEnumeration; } }
		IEnumerable<IConstantLine> IAxisElementContainer.ConstantLines { get { return ConstantLinesEnumeration; } }
		IEnumerable<IStrip> IAxisElementContainer.Strips { get { return StripsEnumeration; } }
		IEnumerable<ICustomAxisLabel> IAxisElementContainer.CustomLabels { get { return CustomLabelsEnumeration; } }
		#endregion
		#region IAxisData implementation
		AxisVisibilityInPanes IAxisData.AxisVisibilityInPanes { get { return ActualVisibilityInPanes; } }
		int IAxisData.GridSpacingFactor { get { return GridSpacingFactor; } }
		RangeValue IAxisData.IncreaseRange(RangeValue range, bool applySideMargins) {
			range = IncreaseRange(range, applySideMargins);
			if (range.Value1 == range.Value2) {
				range.Value1 -= 0.5;
				range.Value2 += 0.5;
			}
			return range;
		}
		AxisScaleTypeMap IAxisData.AxisScaleTypeMap {
			get { return axisScaleTypeMap; }
			set { 
				axisScaleTypeMap = value;
				ActualLabel.SyncronizeTextPatterWithScaleType();
			}
		}
		bool IAxisData.Reverse { get { return ActualReverse; } }
		bool IAxisData.IsRadarAxis { get { return IsRadarAxis; } }
		bool IAxisData.IsArgumentAxis { get { return !IsValuesAxis; } }
		bool IAxisData.IsValueAxis { get { return IsValuesAxis; } }
		bool IAxisData.IsVertical { get { return IsVertical; } }
		bool IAxisData.FixedRange { get { return range.Fixed; } }
		bool IAxisData.ShowLabels { get { return ActualLabel.ActualVisibility; } }
		bool IAxisData.ShowMajorGridlines { get { return true; } }
		bool IAxisData.ShowMajorTickmarks { get { return true; } }
		bool IAxisData.ShowMinorGridlines { get { return true; } }
		bool IAxisData.ShowMinorTickmarks { get { return true; } }
		bool IAxisData.CanShowCustomWithAutoLabels { get { return CanShowCustomWithAutoLabels; } }
		IAxisLabel IAxisData.Label { get { return ActualLabel; } }
		IAxisTitle IAxisData.Title { get { return null; } }
		IAxisRange IAxisData.Range { get { return range.Data; } }
		IAxisRange IAxisData.ScrollingRange { get { return range.scrollingRange.Data; } }
		IVisualAxisRangeData IAxisData.VisualRange { get { return VisualRangeData; } }
		IWholeAxisRangeData IAxisData.WholeRange { get { return WholeRangeData; } }
		INumericScaleOptions IAxisData.NumericScaleOptions { get { return numericScaleOptions; } }
		IDateTimeScaleOptions IAxisData.DateTimeScaleOptions { get { return dateTimeScaleOptions; } }
		IScaleOptionsBase IAxisData.QualitativeScaleOptions { get { return qualitativeScaleOptions; } }
		IList<IMinMaxValues> IAxisData.CalculateRangeLimitsList(double min, double max) {
			return CalculateRangeLimitsList(min, max);
		}
		GRealRect2D IAxisData.GetLabelBounds(IPane pane) {
			return GetLabelBounds(pane);
		}
		IAxisGridMapping IAxisData.GridMapping {
			get {
				if (axisScaleTypeMap is AxisDateTimeMap)
					return new AxisDateTimeGridMapping((AxisDateTimeMap)axisScaleTypeMap, ((IDateTimeScaleOptions)dateTimeScaleOptions).GridAlignment, DateTimeScaleOptions.GridOffset);
				else if (axisScaleTypeMap is AxisNumericalMap)
					return new AxisNumericGridMapping((AxisNumericalMap)axisScaleTypeMap, ((INumericScaleOptions)numericScaleOptions).GridAlignment, NumericScaleOptions.GridOffset);
				else if (axisScaleTypeMap is AxisQualitativeMap)
					return new AxisQualitativeGridMapping((AxisQualitativeMap)axisScaleTypeMap, qualitativeScaleOptions.GridOffset);
				return null;
			}
		}
		IAxisLabelFormatterCore IAxisData.LabelFormatter {
			get { return Label.Formatter; }
			set { Label.Formatter = (IAxisLabelFormatter)value; }
		}
		void IAxisData.UpdateUserValues() { }
		void IAxisData.Deserialize() {
			RangeDataBase.Deserializer visualDeserializer = range.GetDeserializer();
			RangeDataBase.Deserializer wholeDeserializer = range.scrollingRange.GetDeserializer();
			string errorMessage;
			if (IsScrollingEnable()) {
				if (visualDeserializer != null) {
					bool deserializationSuccess = visualDeserializer.TryDeserialize(out errorMessage, visualRangeData);
					if (!deserializationSuccess)
						ThrowIncorrectRangeValueException(errorMessage);
				}
				if (wholeDeserializer != null) {
					bool deserializationSuccess = wholeDeserializer.TryDeserialize(out errorMessage, wholeRangeData);
					if (!deserializationSuccess)
						ThrowIncorrectRangeValueException(errorMessage);
				}
			}
			else if (visualDeserializer != null) {
				bool deserializationSuccess = visualDeserializer.TryDeserialize(out errorMessage, visualRangeData, wholeRangeData);
				if (!deserializationSuccess)
					ThrowIncorrectRangeValueException(errorMessage);
			}
			visualDeserializer = VisualRange.GetDeserializer();
			wholeDeserializer = WholeRange.GetDeserializer();
			if (visualDeserializer != null)
				visualDeserializer.TryDeserialize(out errorMessage, visualRangeData);
			if (wholeDeserializer != null)
				wholeDeserializer.TryDeserialize(out errorMessage, wholeRangeData);
		}
		void TryDeserializeRangeForEarchScaleMap(RangeDataBase.Deserializer deserializer, RangeDataBase rangeData) {
			string errorMessage = null;
			ScaleTypeMap = new AxisNumericalMap();
			bool deserializationSuccess1 = deserializer.TryDeserialize(out errorMessage, rangeData);
			if (!deserializationSuccess1) {
				ScaleTypeMap = new AxisDateTimeMap();
				bool deserializationSuccess2 = deserializer.TryDeserialize(out errorMessage, rangeData);
				if (!deserializationSuccess2) {
					List<object> qualitativeValues = new List<object>() { deserializer.MinValueSerializable, deserializer.MaxValueSerializable };
					ScaleTypeMap = new AxisQualitativeMap(qualitativeValues);
					bool deserializationSuccess3 = deserializer.TryDeserialize(out errorMessage, rangeData);
					if (!deserializationSuccess3)
						ThrowIncorrectRangeValueException(errorMessage);
				}
			}
		}
		bool IsCarrentScaleMapNotDefault(bool isValueAxis) {
			foreach (RefinedSeries refinedSeries in Diagram.ViewController.ActiveRefinedSeries) {
				if (isValueAxis)
					return true;
				Series series = (Series)refinedSeries.Series;
				if (series.ArgumentScaleType != XtraCharts.ScaleType.Auto)
					return true;
				if (series.Points.Count > 0)
					return true;
			}
			return false;
		}
		void ThrowIncorrectRangeValueException(string message) {
			throw new ArgumentException(message);
		}
		void IAxisData.UpdateAutoMeasureUnit() {
			UpdateAutoMeasureUnit(false);
		}
		#endregion
		bool IsScrollingEnable() {
			XYDiagram diagram = Diagram as XYDiagram;
			if (diagram != null) {
				if (IsValuesAxis)
					return diagram.EnableAxisYScrolling;
				else
					return diagram.EnableAxisXScrolling;
			}
			else {
				SwiftPlotDiagram swiftPlotDiagram = Diagram as SwiftPlotDiagram;
				if (swiftPlotDiagram != null) {
					if (IsValuesAxis)
						return swiftPlotDiagram.EnableAxisYScrolling;
					else
						return swiftPlotDiagram.EnableAxisXScrolling;
				}
			}
			return false;
		}
		MeasureUnitsCalculatorBase GetMeasureUnitsCalculator() {
			switch (ScaleType) {
				case ActualScaleType.Numerical:
					return NumericScaleOptions.NumericMeasureUnitsCalculatorCore;
				case ActualScaleType.DateTime:
					return DateTimeScaleOptions.DateTimeMeasureUnitsCalculatorCore;
				default:
					return null;
			}
		}
		internal void ResetAutoProperty() {
			if (!XtraSerializingHelper.XtraSerializing && range.SupportAuto) {			   
				VisualRangeData.Reset(true);				
				if (!IsScrollingEnable())
					((WholeRangeData)WholeRangeData).CorrectionMode = RangeCorrectionMode.Auto;
				((VisualRangeData)VisualRangeData).CorrectionMode =  RangeCorrectionMode.Auto;
			}
		}
		protected internal virtual RangeValue IncreaseRange(RangeValue range, bool applySideMargins) { return RangeValue.Empty; }
		protected abstract AxisRange CreateAxisRange(RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange);
		protected abstract GridLines CreateGridLines();
		protected virtual AxisLabel CreateAxisLabel() {
			return new AxisLabel(this);
		}		
		protected virtual IList<IMinMaxValues> CalculateRangeLimitsList(double min, double max) {
			return null;
		}
		protected virtual GRealRect2D GetLabelBounds(IPane pane) {
			return GRealRect2D.Empty;
		}
		protected internal virtual void OnEndLoading() {
		}
		protected internal void UpdateAutoMeasureUnit(bool throwUpdate) {
			IXYDiagram diagram = Diagram as IXYDiagram;
			if (diagram != null) {
				int length = diagram.GetAxisXLength(this);
				if (UpdateAutomaticMeasureUnit(length) && throwUpdate)
					RaiseControlChanged(new DataAggregationUpdate(this));
			}
		}
		protected internal bool UpdateAutomaticMeasureUnit(int axisLength) {
			if ((Diagram == null) || (Diagram.Chart == null) || (ActualScaleOptions == null))
				return false;
			IList<ISeries> seriesList = GetSeries();
			MeasureUnitsCalculatorBase calculator = GetMeasureUnitsCalculator();
			return calculator != null && seriesList != null ? calculator.UpdateAutomaticMeasureUnit(axisLength, seriesList) : false;
		}
		protected internal virtual bool IsGridSpacingSupported { get { return true; } }
		protected internal virtual double GetGridSpacingByUserValue(double userValue) {
			return userValue;
		}
		protected internal virtual bool GetGridSpacingAutoByUserValue(bool userValue) {
			return userValue;
		}
		internal void SetSideMarginValue(double value) {
			sideMarginValue = value;
		}
		internal double GetSideMarginValue() {
			return sideMarginValue;
		}
		internal bool UpdateMeasurementUnit(DateTimeMeasureUnit measureUnit) {
			if ((measureUnit == dateTimeScaleOptions.MeasureUnit)
				&& (measureUnit == (DateTimeMeasureUnit)dateTimeScaleOptions.GridAlignment))
				return false;
			dateTimeScaleOptions.SetMeasureUnitsDirect(measureUnit, (DateTimeGridAlignment)measureUnit);
			return true;
		}
		internal object ConvertBasedOnScaleType(object value) {
			return ScaleTypeMap.ConvertValue(value, CultureInfo.CurrentCulture);
		}
		internal bool IsCompatibleWith(IAxisValueContainer container) {
			return ScaleTypeMap.IsCompatible(container.GetAxisValue());
		}		
		internal int GetTextSize(Rectangle bounds) {
			return IsVertical ? bounds.Width : bounds.Height;
		}
		internal void OnWholeRangeCnanged(RangeSnapshot oldRange, RangeSnapshot newRange) {
			if ((ContainerAdapter != null) && (ContainerAdapter.EventsProvider != null)) {
				AxisRangeChangedEventArgs arg = new AxisRangeChangedEventArgs(this,
																			  new ValueChangeInfo<object>(oldRange.MinValue, newRange.MinValue),
																			  new ValueChangeInfo<object>(oldRange.MaxValue, newRange.MaxValue),
																			  new ValueChangeInfo<double>(oldRange.Min, newRange.Min),
																			  new ValueChangeInfo<double>(oldRange.Max, newRange.Max));
				ContainerAdapter.EventsProvider.OnAxisWholeRangeChanged(arg);
				if (arg.Cancel)
					((IAxisRangeData)wholeRangeData).ApplyState(oldRange);
			}
		}
		internal void OnVisualRangeCnanged(RangeSnapshot oldRange, RangeSnapshot newRange) {
			if ((ContainerAdapter != null) && (ContainerAdapter.EventsProvider != null)) {
				AxisRangeChangedEventArgs arg = new AxisRangeChangedEventArgs(this,
																			  new ValueChangeInfo<object>(oldRange.MinValue, newRange.MinValue),
																			  new ValueChangeInfo<object>(oldRange.MaxValue, newRange.MaxValue),
																			  new ValueChangeInfo<double>(oldRange.Min, newRange.Min),
																			  new ValueChangeInfo<double>(oldRange.Max, newRange.Max));
				ContainerAdapter.EventsProvider.OnAxisVisualRangeChanged(arg);
				if (arg.Cancel)
					((IAxisRangeData)visualRangeData).ApplyState(oldRange);
			}
		}
		internal string[] GetAvailablePatternPlaceholders() {
			return IsValuesAxis ? new string[1] { ToolTipPatternUtils.ValuePattern } : new string[1] { ToolTipPatternUtils.ArgumentPattern };
		}
		internal IList<ISeries> GetSeries() {
			return Diagram != null ? Diagram.Chart.ViewController.GetSeriesByAxis(this) : null;
		}
		public double GetScaleInternalValue(double numericValue) {
			if (ScaleType != ActualScaleType.Numerical)
				throw new InvalidScaleTypeException(ChartLocalizer.GetString(ChartStringId.MsgInvalidScaleType));
			return ScaleTypeMap.NativeToInternal(numericValue);
		}
		public double GetScaleInternalValue(string qualitativeValue) {
			if (ScaleType != ActualScaleType.Qualitative)
				throw new InvalidScaleTypeException(ChartLocalizer.GetString(ChartStringId.MsgInvalidScaleType));
			return ScaleTypeMap.NativeToInternal(qualitativeValue);
		}
		public double GetScaleInternalValue(DateTime dateTimeValue) {
			if (ScaleType != ActualScaleType.DateTime)
				throw new InvalidScaleTypeException(ChartLocalizer.GetString(ChartStringId.MsgInvalidScaleType));
			return ScaleTypeMap.NativeToInternal(dateTimeValue);
		}
		public object GetScaleValueFromInternal(double internalValue) {
			return ScaleTypeMap.InternalToNative(internalValue);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			AxisBase axis = obj as AxisBase;
			if (axis != null) {
				numericScaleOptions.Assign(axis.numericScaleOptions);
				dateTimeScaleOptions.Assign(axis.dateTimeScaleOptions);
				qualitativeScaleOptions.Assign(axis.qualitativeScaleOptions);
				minorCount = axis.minorCount;
				interlaced = axis.interlaced;
				interlacedColor = axis.interlacedColor;
				logarithmic = axis.logarithmic;
				logarithmicBase = axis.logarithmicBase;
				range.Assign(axis.range);
				visualRangeData.Assign(axis.visualRangeData);
				wholeRangeData.Assign(axis.wholeRangeData);
				visualRange.Assign(axis.visualRange);
				wholeRange.Assign(axis.wholeRange);
				gridLines.Assign(axis.gridLines);
				label.Assign(axis.label);
				scale = axis.scale;
				axisScaleTypeMap = axis.axisScaleTypeMap.Clone();
				showMajorGridlines = axis.showMajorGridlines;
				showMajorTickmarks = axis.showMajorTickmarks;
				showMinorGridlines = axis.showMinorGridlines;
				showMinorTickmarks = axis.showMinorTickmarks;
			}
		}
	}
}

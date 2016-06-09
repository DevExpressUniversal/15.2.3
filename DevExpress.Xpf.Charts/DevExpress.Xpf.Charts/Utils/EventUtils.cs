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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	public delegate void CustomDrawSeriesEventHandler(object sender, CustomDrawSeriesEventArgs e);
	[NonCategorized]
	public class CustomDrawSeriesEventArgs : RoutedEventArgs {
		DrawOptions drawOptions;
		string legendText;
		Series series;
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomDrawSeriesEventArgsDrawOptions")]
#endif
		public DrawOptions DrawOptions { get { return drawOptions; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomDrawSeriesEventArgsLegendText")]
#endif
		public string LegendText { get { return legendText; } set { legendText = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomDrawSeriesEventArgsSeries")]
#endif
		public Series Series { get { return series; } }
		internal CustomDrawSeriesEventArgs(RoutedEvent routedEvent, DrawOptions drawOptions, string legendText, Series series)
			: base(routedEvent) {
			this.drawOptions = drawOptions;
			this.legendText = legendText;
			this.series = series;
		}
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) {
			CustomDrawSeriesEventHandler handler = (CustomDrawSeriesEventHandler)genericHandler;
			handler(genericTarget, this);
		}
	}
	public delegate void CustomDrawCrosshairEventHandler(object sender, CustomDrawCrosshairEventArgs e);
	[NonCategorized]
	public class CustomDrawCrosshairEventArgs : RoutedEventArgs {
		List<CrosshairElement> crosshairElements;
		CrosshairLineElement crosshairLineElement;
		List<CrosshairAxisLabelElement> crosshairAxisLabelElements;
		List<CrosshairGroupHeaderElement> crosshairGroupHeaderElements;
		List<CrosshairElementGroup> crosshairElementGroups;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomDrawCrosshairEventArgsCrosshairElements"),
#endif
		Obsolete(ObsoleteMessages.CrosshairElementsProperty), 
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public List<CrosshairElement> CrosshairElements {
			get {
				if (crosshairElements == null) {
					crosshairElements = new List<CrosshairElement>();
					foreach (CrosshairElementGroup elementGroup in crosshairElementGroups)
						crosshairElements.AddRange(elementGroup.CrosshairElements);
				}
				return crosshairElements;
			}
		}
		public CrosshairLineElement CrosshairLineElement { get { return crosshairLineElement; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomDrawCrosshairEventArgsCrosshairAxisLabelElements")]
#endif
		public List<CrosshairAxisLabelElement> CrosshairAxisLabelElements { get { return crosshairAxisLabelElements; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomDrawCrosshairEventArgsCrosshairGroupHeaderElements"),
#endif
		Obsolete(ObsoleteMessages.CrosshairGroupHeaderElementsProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public List<CrosshairGroupHeaderElement> CrosshairGroupHeaderElements { 
			get {
				if (crosshairGroupHeaderElements == null) {
					crosshairGroupHeaderElements = new List<CrosshairGroupHeaderElement>();
					foreach (CrosshairElementGroup elementGroup in crosshairElementGroups)
						crosshairGroupHeaderElements.Add(elementGroup.HeaderElement);
				}
				return crosshairGroupHeaderElements; 
			} 
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomDrawCrosshairEventArgsCrosshairElementGroups")]
#endif
		public List<CrosshairElementGroup> CrosshairElementGroups { get { return crosshairElementGroups; } }
		internal CustomDrawCrosshairEventArgs(RoutedEvent routedEvent, CrosshairLineElement crossshairLineElement, List<CrosshairAxisLabelElement> crosshairAxisLabelElements, List<CrosshairElementGroup> crosshairElementGroups)
			: base(routedEvent) {
			this.crosshairLineElement = crossshairLineElement;
			this.crosshairAxisLabelElements = crosshairAxisLabelElements;
			this.crosshairElementGroups = crosshairElementGroups;
		}
	}
	[NonCategorized]
	public class CrosshairElement {
		readonly IRefinedSeries refinedSeries;
		readonly RefinedPoint refinedPoint;
		SeriesPoint seriesPoint;
		CrosshairLineElement lineElement;
		CrosshairAxisLabelElement axisLabelElement;
		CrosshairLabelElement labelElement;
		bool visible;
		internal IRefinedSeries RefinedSeries { get { return refinedSeries; } }
		internal RefinedPoint RefinedPoint { get { return refinedPoint; } }
		public SeriesPoint SeriesPoint {
			get { return seriesPoint; }
			set { seriesPoint = value; }
		}
		public Series Series { get { return seriesPoint.Series; } }
		public CrosshairLineElement LineElement {
			get { return lineElement; }
			set { lineElement = value; }
		}
		public CrosshairAxisLabelElement AxisLabelElement {
			get { return axisLabelElement; }
			set { axisLabelElement = value; }
		}
		public CrosshairLabelElement LabelElement {
			get { return labelElement; }
			set { labelElement = value; }
		}
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}
		internal CrosshairElement(SeriesPoint seriesPoint, CrosshairLineElement lineElement, CrosshairAxisLabelElement axisLabelElement, CrosshairLabelElement labelElement,IRefinedSeries refinedSeries, RefinedPoint refinedPoint) {
			this.seriesPoint = seriesPoint;
			this.lineElement = lineElement;
			this.axisLabelElement = axisLabelElement;
			this.labelElement = labelElement;
			this.visible = true;
			this.refinedSeries = refinedSeries;
			this.refinedPoint = refinedPoint;
		}
	}
	[NonCategorized]
	public class CrosshairLineElement {
		bool visible;
		Brush brush;
		LineStyle lineStyle;
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}
		public Brush Brush {
			get { return brush; }
			set { brush = value; }
		}
		public LineStyle LineStyle {
			get { return lineStyle; }
			set { lineStyle = value; }
		}
		internal CrosshairLineElement(bool visible, Brush brush, LineStyle lineStyle) {
			this.visible = visible;
			this.brush = brush;
			this.lineStyle = lineStyle;
		}
	}
	[NonCategorized]
	public class CrosshairLabelElementBase {
		bool visible;
		string text;
		Brush foreground;
		FontFamily fontFamily;
		double fontSize;
		FontStretch fontStretch;
		FontStyle fontStyle;
		FontWeight fontWeight;
		DataTemplate crosshairLabelTemplate;
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}
		public string Text {
			get { return text; }
			set { text = value; }
		}
		public Brush Foreground {
			get { return foreground; }
			set { foreground = value; }
		}
		public FontFamily FontFamily {
			get { return fontFamily; }
			set { fontFamily = value; }
		}
		public double FontSize {
			get { return fontSize; }
			set { fontSize = value; }
		}
		public FontStyle FontStyle {
			get { return fontStyle; }
			set { fontStyle = value; }
		}
		public FontWeight FontWeight {
			get { return fontWeight; }
			set { fontWeight = value; }
		}
		public FontStretch FontStretch {
			get { return fontStretch; }
			set { fontStretch = value; }
		}
		public DataTemplate CrosshairLabelTemplate {
			get { return crosshairLabelTemplate; }
			set { crosshairLabelTemplate = value; }
		}
		internal CrosshairLabelElementBase(string text, Brush foreground,
			FontFamily fontFamily, double fontSize, FontStretch fontStretch, FontStyle fontStyle, FontWeight fontWeight, bool visible, DataTemplate crosshairLabelTemplate) {
			this.text = text;
			this.foreground = foreground;
			this.visible = visible;
			this.fontFamily = fontFamily;
			this.fontSize = fontSize;
			this.fontStretch = fontStretch;
			this.fontStyle = fontStyle;
			this.fontWeight = fontWeight;
			this.crosshairLabelTemplate = crosshairLabelTemplate;
		}
	}
	[NonCategorized]
	public class CrosshairGroupHeaderElement : CrosshairLabelElementBase {
		readonly List<SeriesPoint> seriesPoints;
		public List<SeriesPoint> SeriesPoints { get { return seriesPoints; } }
		internal CrosshairGroupHeaderElement(List<SeriesPoint> seriesPoints, string text, Brush foreground,
			FontFamily fontFamily, double fontSize, FontStretch fontStretch, FontStyle fontStyle, FontWeight fontWeight, DataTemplate crosshairLabelTemplate)
			: base(text, foreground, fontFamily, fontSize, fontStretch, fontStyle, fontWeight, true, crosshairLabelTemplate) {
			this.seriesPoints = seriesPoints;
		}
	}
	[NonCategorized]
	public class CrosshairAxisLabelElement : CrosshairLabelElementBase {
		object axisValue;
		Brush background;
		public object AxisValue {
			get { return axisValue; }
			set { axisValue = value; }
		}
		public Brush Background {
			get { return background; }
			set { background = value; }
		}
		internal CrosshairAxisLabelElement(string text, Brush background, Brush foreground,
			FontFamily fontFamily, double fontSize, FontStretch fontStretch, FontStyle fontStyle, FontWeight fontWeight, bool visible, object axisValue, DataTemplate crosshairLabelTemplate)
			: base(text, foreground, fontFamily, fontSize, fontStretch, fontStyle, fontWeight, visible, crosshairLabelTemplate) {
			this.axisValue = axisValue;
			this.background = background;
		}
	}
	[NonCategorized]
	public class CrosshairLabelElement : CrosshairLabelElementBase {
		bool textVisible;
		Brush markerBrush;
		bool markerVisible;
		string headerText;
		string footerText;
		public bool TextVisible {
			get { return textVisible; }
			set { textVisible = value; }
		}
		public Brush MarkerBrush {
			get { return markerBrush; }
			set { markerBrush = value; }
		}
		public bool MarkerVisible {
			get { return markerVisible; }
			set { markerVisible = value; }
		}
		public string HeaderText {
			get { return headerText; }
			set { headerText = value; }
		}
		public string FooterText {
			get { return footerText; }
			set { footerText = value; }
		}
		internal CrosshairLabelElement(string text, Brush foreground, FontFamily fontFamily,
		double fontSize, FontStretch fontStretch, FontStyle fontStyle, FontWeight fontWeight, bool visible, DataTemplate crosshairLabelTemplate, Brush markerBrush)
			: base(text, foreground, fontFamily, fontSize, fontStretch, fontStyle, fontWeight, visible, crosshairLabelTemplate) {
			textVisible = true;
			this.markerBrush = markerBrush;
			markerVisible = true;
		}
	}
	[NonCategorized]
	public class CrosshairElementGroup {
		readonly CrosshairGroupHeaderElement headerElement;
		readonly List<CrosshairElement> crosshairElements;
		public CrosshairGroupHeaderElement HeaderElement { get { return headerElement; } }
		public IList<CrosshairElement> CrosshairElements { get { return crosshairElements; } }
		internal CrosshairElementGroup(CrosshairGroupHeaderElement headerElement, List<CrosshairElement> crosshairElements) {
			this.headerElement = headerElement;
			this.crosshairElements = crosshairElements;
		}
	}
	public delegate void CustomDrawSeriesPointEventHandler(object sender, CustomDrawSeriesPointEventArgs e);
	[NonCategorized]
	public class CustomDrawSeriesPointEventArgs : CustomDrawSeriesEventArgs {
		string[] labelsTexts;
		SeriesPoint seriesPoint;
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomDrawSeriesPointEventArgsLabelText")]
#endif
		public string LabelText { get { return labelsTexts[0]; } set { labelsTexts[0] = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomDrawSeriesPointEventArgsLabelsTexts")]
#endif
		public string[] LabelsTexts { get { return labelsTexts; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomDrawSeriesPointEventArgsSeriesPoint")]
#endif
		public SeriesPoint SeriesPoint { get { return seriesPoint; } }
		internal CustomDrawSeriesPointEventArgs(RoutedEvent routedEvent, DrawOptions drawOptions, string legendText, Series series, string[] labelsTexts, SeriesPoint seriesPoint)
			: base(routedEvent, drawOptions, legendText, series) {
			this.seriesPoint = seriesPoint;
			this.labelsTexts = labelsTexts;
		}
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) {
			CustomDrawSeriesPointEventHandler handler = (CustomDrawSeriesPointEventHandler)genericHandler;
			handler(genericTarget, this);
		}
	}
	public delegate void QueryChartCursorEventHandler(object sender, QueryChartCursorEventArgs e);
	[NonCategorized]
	public class QueryChartCursorEventArgs : RoutedEventArgs {
		Cursor cursor;
		ImageSource cursorImage;
		Point position;
		Point cursorImageOffset;
#if !SL
	[DevExpressXpfChartsLocalizedDescription("QueryChartCursorEventArgsCursor")]
#endif
		public Cursor Cursor { get { return cursor; } set { cursor = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("QueryChartCursorEventArgsCursorImage")]
#endif
		public ImageSource CursorImage { get { return cursorImage; } set { cursorImage = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("QueryChartCursorEventArgsPosition")]
#endif
		public Point Position { get { return position; } set { position = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("QueryChartCursorEventArgsCursorImageOffset")]
#endif
		public Point CursorImageOffset { get { return cursorImageOffset; } set { cursorImageOffset = value; } }
		internal QueryChartCursorEventArgs(RoutedEvent routedEvent, Cursor cursor, ImageSource cursorImage, Point position, Point cursorImageOffset)
			: base(routedEvent) {
			this.cursor = cursor;
			this.cursorImage = cursorImage;
			this.position = position;
			this.cursorImageOffset = cursorImageOffset;
		}
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) {
			QueryChartCursorEventHandler handler = (QueryChartCursorEventHandler)genericHandler;
			handler(genericTarget, this);
		}
	}
	public delegate void ToolTipOpeningEventHandler(object sender, ChartToolTipEventArgs e);
	public delegate void ToolTipClosingEventHandler(object sender, ChartToolTipEventArgs e);
	[NonCategorized]
	public class ChartToolTipEventArgs : CancelEventArgs {
		object hint;
		bool showBeak;
		string pattern;
		DataTemplate template;
		SeriesPoint seriesPoint;
		Series series;
		ChartControl chartControl;
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartToolTipEventArgsHint")]
#endif
		public object Hint { get { return hint; } set { hint = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartToolTipEventArgsShowBeak")]
#endif
		public bool ShowBeak { get { return showBeak; } set { showBeak = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartToolTipEventArgsPattern")]
#endif
		public string Pattern { get { return pattern; } set { pattern = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartToolTipEventArgsTemplate")]
#endif
		public DataTemplate Template { get { return template; } set { template = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartToolTipEventArgsSeries")]
#endif
		public Series Series { get { return series; } set { series = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartToolTipEventArgsSeriesPoint")]
#endif
		public SeriesPoint SeriesPoint { get { return seriesPoint; } set { seriesPoint = value; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartToolTipEventArgsChartControl")]
#endif
		public ChartControl ChartControl { get { return chartControl; } set { chartControl = value; } }
		internal ChartToolTipEventArgs(object hint, bool showBeak, string pattern, DataTemplate template, Series series, SeriesPoint seriesPoint, ChartControl chartControl)
			: base() {
			this.hint = hint;
			this.showBeak = showBeak;
			this.pattern = pattern;
			this.template = template;
			this.seriesPoint = seriesPoint;
			this.series = series;
			this.chartControl = chartControl;
		}
	}
	public class ValueChangeInfo<T> {
		readonly T oldValue;
		readonly T newValue;
		public T OldValue {
			get {
				return oldValue;
			}
		}
		public T NewValue {
			get {
				return newValue;
			}
		}
		public bool IsChanged { get { return !oldValue.Equals(newValue); } }
		internal ValueChangeInfo(T value) {
			this.oldValue = value;
			this.newValue = value;
		}
		internal ValueChangeInfo(T oldValue, T newValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
	public abstract class AxisScaleChangedEventArgs : RoutedEventArgs {
		readonly AxisBase axis;
		readonly ValueChangeInfo<ScaleMode> scaleModeChange;
		readonly ValueChangeInfo<AggregateFunction> aggregateFunctionChange;
		readonly ValueChangeInfo<double> gridSpacingChange;
		readonly ValueChangeInfo<double> gridOffsetChange;
		readonly ValueChangeInfo<bool> autoGridChange;
		public AxisBase Axis { get { return axis; } }
		public ValueChangeInfo<ScaleMode> ScaleModeChange { get { return scaleModeChange; } }
		public ValueChangeInfo<AggregateFunction> AggregateFunctionChange { get { return aggregateFunctionChange; } }
		public ValueChangeInfo<double> GridSpacingChange { get { return gridSpacingChange; } }
		public ValueChangeInfo<double> GridOffsetChange { get { return gridOffsetChange; } }
		public ValueChangeInfo<bool> AutoGridChange { get { return autoGridChange; } }
		public AxisScaleChangedEventArgs(AxisBase axis,
												ValueChangeInfo<ScaleMode> scaleModeChange,
												ValueChangeInfo<double> gridSpacingChange,
												ValueChangeInfo<AggregateFunction> aggregateFunctionChange,
												ValueChangeInfo<double> gridOffsetChange,
												ValueChangeInfo<bool> autoGridChange,
												RoutedEvent eventType)
			: base(eventType) {
			this.axis = axis;
			this.scaleModeChange = scaleModeChange;
			this.gridSpacingChange = gridSpacingChange;
			this.gridOffsetChange = gridOffsetChange;
			this.aggregateFunctionChange = aggregateFunctionChange;
			this.autoGridChange = autoGridChange;
		}
	}
	public class DateTimeScaleChangedEventArgs : AxisScaleChangedEventArgs {
		readonly ValueChangeInfo<DateTimeMeasureUnit> measureUnitChange;
		readonly ValueChangeInfo<DateTimeGridAlignment> gridAlignmentChange;
		public ValueChangeInfo<DateTimeMeasureUnit> MeasureUnitChange { get { return measureUnitChange; } }
		public ValueChangeInfo<DateTimeGridAlignment> GridAlignmentChange { get { return gridAlignmentChange; } }
		internal DateTimeScaleChangedEventArgs(AxisBase axis,
												ValueChangeInfo<ScaleMode> scaleModeChange,
												ValueChangeInfo<DateTimeMeasureUnit> measureUnitChange,
												ValueChangeInfo<DateTimeGridAlignment> gridAlignmentChange,
												ValueChangeInfo<double> gridSpacingChange,
												ValueChangeInfo<AggregateFunction> aggregateFunctionChange,
												ValueChangeInfo<double> gridOffsetChange,
												ValueChangeInfo<bool> autoGridChange)
			: base(axis, scaleModeChange, gridSpacingChange, aggregateFunctionChange, gridOffsetChange, autoGridChange, ChartControl.AxisScaleChangedEvent) {
			this.measureUnitChange = measureUnitChange;
			this.gridAlignmentChange = gridAlignmentChange;
		}
	}
	public class NumericScaleChangedEventArgs : AxisScaleChangedEventArgs {
		readonly ValueChangeInfo<double> measureUnitChange;
		readonly ValueChangeInfo<double> gridAlignmentChange;
		public ValueChangeInfo<double> MeasureUnitChange { get { return measureUnitChange; } }
		public ValueChangeInfo<double> GridAlignmentChange { get { return gridAlignmentChange; } }
		public NumericScaleChangedEventArgs(AxisBase axis,
											ValueChangeInfo<ScaleMode> scaleModeChange,
											ValueChangeInfo<double> measureUnitChange,
											ValueChangeInfo<double> gridAlignmentChange,
											ValueChangeInfo<double> gridSpacingChange,
											ValueChangeInfo<AggregateFunction> aggregateFunctionChange,
											ValueChangeInfo<double> gridOffsetChange,
											ValueChangeInfo<bool> autoGridChange)
			: base(axis, scaleModeChange, gridSpacingChange, aggregateFunctionChange, gridOffsetChange, autoGridChange, ChartControl.AxisScaleChangedEvent) {
			this.measureUnitChange = measureUnitChange;
			this.gridAlignmentChange = gridAlignmentChange;
		}
	}
	public delegate void AxisScaleChangedEventHandler(object sender, AxisScaleChangedEventArgs e);
}
namespace DevExpress.Charts.Native {
	public class AxisScaleChangedEventArgsHelper {
		internal static NumericScaleChangedEventArgs Create(AxisBase axis, NumericScaleOptionsBase oldOptions, NumericScaleOptionsBase newOptions) {
			bool changed = false;
			var scaleModeChange = new ValueChangeInfo<ScaleMode>(GetScaleMode(oldOptions), GetScaleMode(newOptions));
			changed |= scaleModeChange.IsChanged;
			var measureUnitChange = new ValueChangeInfo<double>(GetMeasureUnit(oldOptions), GetMeasureUnit(newOptions));
			changed |= measureUnitChange.IsChanged;
			var gridAlignmentChange = new ValueChangeInfo<double>(GetGridAlignment(oldOptions), GetGridAlignment(newOptions));
			changed |= gridAlignmentChange.IsChanged;
			var gridSpacingChange = new ValueChangeInfo<double>(GetGridSpacing(oldOptions), GetGridSpacing(newOptions));
			changed |= gridSpacingChange.IsChanged;
			var gridOffsetChange = new ValueChangeInfo<double>(GetGridOffset(oldOptions), GetGridOffset(newOptions));
			changed |= gridOffsetChange.IsChanged;
			var autoGridChange = new ValueChangeInfo<bool>(GetAutoGrid(oldOptions), GetAutoGrid(newOptions));
			changed |= autoGridChange.IsChanged;
			var aggregateFunctionChange = new ValueChangeInfo<AggregateFunction>(GetAggregateFunction(oldOptions), GetAggregateFunction(newOptions));
			changed |= aggregateFunctionChange.IsChanged;
			if (!changed)
				return null;
			return new NumericScaleChangedEventArgs(axis, scaleModeChange, measureUnitChange, gridAlignmentChange, gridSpacingChange, aggregateFunctionChange, gridOffsetChange, autoGridChange);
		}
		internal static DateTimeScaleChangedEventArgs Create(AxisBase axis, DateTimeScaleOptionsBase oldOptions, DateTimeScaleOptionsBase newOptions) {
			bool changed = false;
			var scaleModeChange = new ValueChangeInfo<ScaleMode>(GetScaleMode(oldOptions), GetScaleMode(newOptions));
			changed |= scaleModeChange.IsChanged;
			var measureUnitChange = new ValueChangeInfo<DateTimeMeasureUnit>(GetMeasureUnit(oldOptions), GetMeasureUnit(newOptions));
			changed |= measureUnitChange.IsChanged;
			var gridAlignmentChange = new ValueChangeInfo<DateTimeGridAlignment>(GetGridAlignment(oldOptions), GetGridAlignment(newOptions));
			changed |= gridAlignmentChange.IsChanged;
			var gridSpacingChange = new ValueChangeInfo<double>(GetGridSpacing(oldOptions), GetGridSpacing(newOptions));
			changed |= gridSpacingChange.IsChanged;
			var gridOffsetChange = new ValueChangeInfo<double>(GetGridOffset(oldOptions), GetGridOffset(newOptions));
			changed |= gridOffsetChange.IsChanged;
			var autoGridChange = new ValueChangeInfo<bool>(GetAutoGrid(oldOptions), GetAutoGrid(newOptions));
			changed |= autoGridChange.IsChanged;
			var aggregateFunctionChange = new ValueChangeInfo<AggregateFunction>(GetAggregateFunction(oldOptions), GetAggregateFunction(newOptions));
			changed |= aggregateFunctionChange.IsChanged;
			if (!changed)
				return null;
			return new DateTimeScaleChangedEventArgs(axis, scaleModeChange, measureUnitChange, gridAlignmentChange, gridSpacingChange, aggregateFunctionChange, gridOffsetChange, autoGridChange);
		}
		static double GetGridAlignment(INumericScaleOptions options) {
			if (options == null)
				return 1.0;
			return options.GridAlignment;
		}
		static double GetMeasureUnit(INumericScaleOptions options) {
			if (options == null)
				return 1.0;
			return options.MeasureUnit;
		}
		static DateTimeGridAlignment GetGridAlignment(IDateTimeScaleOptions options) {
			if (options == null)
				return DateTimeGridAlignment.Day;
			return (DateTimeGridAlignment)options.GridAlignment;
		}
		static DateTimeMeasureUnit GetMeasureUnit(IDateTimeScaleOptions options) {
			if (options == null)
				return DateTimeMeasureUnit.Day;
			return (DateTimeMeasureUnit)options.MeasureUnit;
		}
		static ScaleMode GetScaleMode(IScaleOptionsBase options) {
			return (ScaleMode)options.ScaleMode;
		}
		static AggregateFunction GetAggregateFunction(IScaleOptionsBase options) {
			return (AggregateFunction)options.AggregateFunction;
		}
		static bool GetAutoGrid(ScaleOptionsBase options) {
			return options.AutoGridImp;
		}
		static double GetGridOffset(IScaleOptionsBase options) {
			return options.GridOffset;
		}
		static double GetGridSpacing(IScaleOptionsBase options) {
			return options.GridSpacing;
		}
	}
}

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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.Data.ChartDataSources;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PivotGridDataSourceOptions : ChartElement {
		#region Nested types: struct DateTimeInterval, class DateTimeOptionsProvider
		struct DateTimeInterval {
			DateTime start;
			DateTime end;
			public DateTime Start { 
				get { return start; } 
				set { start = value; } 
			}
			public DateTime End { 
				get { return end; } 
				set { end = value; } 
			}
			public bool IsEmpty { get { return start == DateTime.MinValue && end == DateTime.MinValue; } }
			public void Clear() {
				start = DateTime.MinValue;
				end = DateTime.MinValue;
			}
			public bool Contains(DateTime dateTime) {
				return dateTime >= start && dateTime <= end;
			}
		}
		class DateTimeOptionsProvider : IDateTimeOptions {
			readonly DateTimeFormat format;
			readonly string formatString;
			DateTimeOptionsFormat IDateTimeOptions.Format { get { return (DateTimeOptionsFormat)format; } }
			string IDateTimeOptions.FormatString { get { return formatString; } }
			string IDateTimeOptions.QuarterFormat { get { return ChartLocalizer.GetString(ChartStringId.QuarterFormat); } }
			public DateTimeOptionsProvider(DateTimeMeasureUnitNative measureUnit, DateTime value) {
				DateTimeFormatUtils.GetDateTimeFormat((DateTimeMeasureUnit)measureUnit, out format, out formatString);
			}
		}
		#endregion
		static void ThrowNotSupportedPropertyException(string propertyName) {
			string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgPivotGridDataSourceOptionsNotSupportedProperty), propertyName);
			throw new NotSupportedException(message);
		}
		const bool DefaultAutoBindingSettingsEnabled = true;
		const bool DefaultAutoLayoutSettingsEnabled = true;
		readonly Locker locker = new Locker();
		bool autoBindingSettingsEnabled = DefaultAutoBindingSettingsEnabled;
		bool autoLayoutSettingsEnabled = DefaultAutoLayoutSettingsEnabled;
		IChartDataSource dataSource;
		List<DateTimeInterval> axisLabelsIntervals = new List<DateTimeInterval>();
		DataContainer DataContainer { get { return (DataContainer)Owner; } }
		internal Chart Chart { get { return ((DataContainer)Owner).Chart; } }
		internal IChartDataSource DataSource { get { return dataSource; } }
		internal IPivotGrid PivotGrid { get { return PivotGridDataSourceUtils.GetPivotGrid(this); } }
		internal bool HasDataSource { get { return PivotGridDataSourceUtils.HasDataSource(this); } }
		internal bool HasPivotGrid { get { return PivotGridDataSourceUtils.HasPivotGrid(this); ; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsAutoBindingSettingsEnabled"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.AutoBindingSettingsEnabled"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool AutoBindingSettingsEnabled {
			get { return autoBindingSettingsEnabled; }
			set {
				if (autoBindingSettingsEnabled != value) {
					SendNotification(new ElementWillChangeNotification(this));
					autoBindingSettingsEnabled = value;
					if (!Loading)
						UpdateAutoBindingSettings();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsAutoLayoutSettingsEnabled"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool AutoLayoutSettingsEnabled {
			get { return autoLayoutSettingsEnabled; }
			set {
				if (autoLayoutSettingsEnabled != value) {
					SendNotification(new ElementWillChangeNotification(this));
					autoLayoutSettingsEnabled = value;
					if (!Loading)
						UpdateAutoLayoutSettings();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsRetrieveDataByColumns"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveDataByColumns"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool RetrieveDataByColumns {
			get { return HasPivotGrid ? PivotGrid.RetrieveDataByColumns : false; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("RetrieveDataByColumns");
				PivotGrid.RetrieveDataByColumns = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsRetrieveEmptyCells"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveEmptyCells"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool RetrieveEmptyCells {
			get { return HasPivotGrid ? PivotGrid.RetrieveEmptyCells : false; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("RetrieveEmptyCells");
				PivotGrid.RetrieveEmptyCells = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsSelectionOnly"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.SelectionOnly"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool SelectionOnly {
			get { return HasPivotGrid ? PivotGrid.SelectionOnly : false; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("SelectionOnly");
				PivotGrid.SelectionOnly = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsSinglePageOnly"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.SinglePageOnly"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool SinglePageOnly {
			get { return HasPivotGrid ? PivotGrid.SinglePageOnly : false; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("ShowAllPages");
				PivotGrid.SinglePageOnly = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsRetrieveColumnTotals"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveColumnTotals"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool RetrieveColumnTotals {
			get { return HasPivotGrid ? PivotGrid.RetrieveColumnTotals : false; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("RetrieveColumnTotals");
				PivotGrid.RetrieveColumnTotals = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsRetrieveColumnGrandTotals"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveColumnGrandTotals"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool RetrieveColumnGrandTotals {
			get { return HasPivotGrid ? PivotGrid.RetrieveColumnGrandTotals : false; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("RetrieveColumnGrandTotals");
				PivotGrid.RetrieveColumnGrandTotals = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsRetrieveColumnCustomTotals"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveColumnCustomTotals"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool RetrieveColumnCustomTotals {
			get { return HasPivotGrid ? PivotGrid.RetrieveColumnCustomTotals : false; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("RetrieveColumnCustomTotals");
				PivotGrid.RetrieveColumnCustomTotals = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsRetrieveRowTotals"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveRowTotals"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool RetrieveRowTotals {
			get { return HasPivotGrid ? PivotGrid.RetrieveRowTotals : false; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("RetrieveRowTotals");
				PivotGrid.RetrieveRowTotals = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsRetrieveRowGrandTotals"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveRowGrandTotals"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool RetrieveRowGrandTotals {
			get { return HasPivotGrid ? PivotGrid.RetrieveRowGrandTotals : false; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("RetrieveRowGrandTotals");
				PivotGrid.RetrieveRowGrandTotals = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsRetrieveRowCustomTotals"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.RetrieveRowCustomTotals"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool RetrieveRowCustomTotals {
			get { return HasPivotGrid ? PivotGrid.RetrieveRowCustomTotals : false; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("RetrieveRowCustomTotals");
				PivotGrid.RetrieveRowCustomTotals = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsMaxAllowedSeriesCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.MaxAllowedSeriesCount"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public int MaxAllowedSeriesCount {
			get { return HasPivotGrid ? PivotGrid.MaxAllowedSeriesCount : 0; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("MaxAllowedSeriesCount");
				PivotGrid.MaxAllowedSeriesCount = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsMaxAllowedPointCountInSeries"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.MaxAllowedPointCountInSeries"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public int MaxAllowedPointCountInSeries {
			get { return HasPivotGrid ? PivotGrid.MaxAllowedPointCountInSeries : 0; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("MaxAllowedPointCountInSeries");
				PivotGrid.MaxAllowedPointCountInSeries = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PivotGridDataSourceOptionsUpdateDelay"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PivotGridDataSourceOptions.UpdateDelay"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public int UpdateDelay {
			get { return HasPivotGrid ? PivotGrid.UpdateDelay : 0; }
			set {
				if (!HasPivotGrid)
					ThrowNotSupportedPropertyException("UpdateDelay");
				PivotGrid.UpdateDelay = value;
			}
		}
		internal PivotGridDataSourceOptions(DataContainer owner) : base(owner) {
		}
		#region ShouldSerialize & Reset
		public bool ShouldSerializeAutoBindingSettingsEnabled() {
			return autoBindingSettingsEnabled != DefaultAutoBindingSettingsEnabled;
		}
		void ResetAutoBindingSettingsEnabled() {
			AutoBindingSettingsEnabled = DefaultAutoBindingSettingsEnabled;
		}
		public bool ShouldSerializeAutoLayoutSettingsEnabled() {
			return autoLayoutSettingsEnabled != DefaultAutoLayoutSettingsEnabled;
		}
		void ResetAutoLayoutSettingsEnabled() {
			AutoLayoutSettingsEnabled = DefaultAutoLayoutSettingsEnabled;
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeAutoLayoutSettingsEnabled() || ShouldSerializeAutoBindingSettingsEnabled() || base.ShouldSerialize();
		}
		#endregion
		#region XtraSerialization
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AutoBindingSettingsEnabled":
					return ShouldSerializeAutoBindingSettingsEnabled();
				case "AutoLayoutSettingsEnabled":
					return ShouldSerializeAutoLayoutSettingsEnabled();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		void AddInterval(DateTime end, ref DateTimeInterval interval) {
			interval.End = end;
			axisLabelsIntervals.Add(interval);
			interval.Clear();
		}
		void SetValueDataMembers() {
			for(int i = 0; i < DataContainer.SeriesTemplate.ValueDataMembers.Count; i++)
				DataContainer.SeriesTemplate.ValueDataMembers[i] = DataSource.ValueDataMember;
		}
		void SetResolveOverlappingMode(SeriesBase series, ResolveOverlappingMode mode) {
			if (series.Label != null)
				series.Label.ResolveOverlappingMode = series.Label.IsCorrectResolveOverlappingMode(mode) ? mode : ResolveOverlappingMode.None;
		}
		void UpdateArgumentDataMembers() {
			if (HasPivotGrid)
				PivotGrid.LockListChanged();
			ScaleType argumentScaleType = ScaleType.Qualitative;
			DataContext dataContext = Chart.ContainerAdapter.DataContext;
			if (BindingHelper.CheckDataMember(dataContext, DataSource, DataSource.ArgumentDataMember, ScaleType.Numerical))
				argumentScaleType = ScaleType.Numerical;
			else if (BindingHelper.CheckDataMember(dataContext, DataSource, DataSource.ArgumentDataMember, ScaleType.DateTime))
				argumentScaleType = ScaleType.DateTime;
			if (HasPivotGrid)
				PivotGrid.UnlockListChanged();
			DataContainer.SeriesTemplate.ArgumentScaleType = argumentScaleType;
		}
		void SetAxisTitleText(Axis2D axis, IList<string> columnNames) {
			string titleText = string.Empty;
			for (int i = 0; i < columnNames.Count; i++) {
				if (i > 0) {
					titleText += " ";
				}
				titleText += columnNames[i];
			}
			axis.Title.Text = titleText;
		}
		void UpdateXYDiagram(AxisBase axisX) {
			AxisXBase axisXBase = axisX as AxisXBase;
			if (HasPivotGrid) {
				if (axisXBase != null)
					axisXBase.ScaleBreaks.Clear();
				if (PivotGrid.ArgumentColumnNames != null) {
					Axis2D axisX2D = axisX as Axis2D;
					if (axisX2D != null)
						SetAxisTitleText(axisX2D, PivotGrid.ArgumentColumnNames);
				}
				if (PivotGrid.ValueColumnNames != null) {
					Axis2D axisY = GetAxisY();
					if (axisY != null)
						SetAxisTitleText(axisY, PivotGrid.ValueColumnNames);
				}
			}
			double axisXSideMargins = AxisBase.DefaultSideMarginValue;
			if (DataContainer.SeriesTemplate.ActualArgumentScaleType == ScaleType.DateTime) {
				if (DataSource.DateTimeArgumentMeasureUnit.HasValue) {
					DateTimeMeasureUnitNative measureUnit = DataSource.DateTimeArgumentMeasureUnit.Value;
					axisX.UpdateMeasurementUnit((DateTimeMeasureUnit)measureUnit);
					if (HasPivotGrid && axisXBase != null) {
						IDictionary<DateTime, DateTimeMeasureUnitNative> measureUnits = PivotGrid.DateTimeMeasureUnitByArgument;
						if (measureUnits != null) {
							axisXBase.ScaleBreakOptions.Style = ScaleBreakStyle.Straight;
							axisXBase.ScaleBreakOptions.SizeInPixels = -1;
							if (axisX.VisualRange.Auto) {
								List<DateTime> arguments = new List<DateTime>(measureUnits.Keys);
								arguments.Sort();
								DateTimeInterval interval = new DateTimeInterval();
								for (int i = 0; i < arguments.Count; i++) {
									if (interval.IsEmpty)
										interval.Start = DateTimeUtils.Floor(arguments[i], measureUnit);
									if (i < arguments.Count - 1) {
										DateTimeMeasureUnitNative currentMeasureUnit = measureUnits[arguments[i]];
										DateTimeMeasureUnitNative nextMeasureUnit = measureUnits[arguments[i + 1]];
										if (currentMeasureUnit != measureUnit || nextMeasureUnit != measureUnit) {
											DateTime start = DateTimeUtils.Increase(DateTimeUtils.Floor(arguments[i], measureUnit), measureUnit);
											DateTime end = DateTimeUtils.Decrease(DateTimeUtils.Floor(arguments[i + 1], measureUnit), measureUnit);
											axisXBase.ScaleBreaks.Add(new ScaleBreak(start.ToString() + " " + end.ToString(), start, end));
											axisXSideMargins = 1;
											AddInterval(DateTimeUtils.Floor(arguments[i], measureUnit), ref interval);
										}
									}
									else
										AddInterval(arguments[i], ref interval);
								}
							}
							else {
								DateTimeInterval axisRangeInterval = new DateTimeInterval();
								axisRangeInterval.Start = (DateTime)axisX.VisualRange.MinValue;
								AddInterval((DateTime)axisX.VisualRange.MaxValue, ref axisRangeInterval);
							}
						}
					}
				}
				else {
					axisX.DateTimeScaleOptions.ScaleMode = ScaleMode.Automatic;
					axisX.DateTimeScaleOptions.AggregateFunction = AggregateFunction.None;
				}
			}
			axisX.SetSideMarginValue(axisXSideMargins);
		}
		void UpdateSimpleSeriesViews() {
			int visibleInLegendSeriesNumber = GetVisibleInLegendSeriesNumber();
			bool isLegendVisible = (DataContainer.AutocreatedSeries.Count == 1 || visibleInLegendSeriesNumber > -1) && !Chart.Legend.UseCheckBoxes;
			DataContainer.SeriesTemplate.LegendTextPattern = PatternUtils.ArgumentPattern;
			if(DataContainer.SeriesTemplate.Label != null)
				SetPivotGridTextPattern(isLegendVisible);
			SimpleDiagramSeriesViewBase seriesTemplateView = DataContainer.SeriesTemplate.View as SimpleDiagramSeriesViewBase;
			if (seriesTemplateView != null) {
				seriesTemplateView.Titles.Clear();
				seriesTemplateView.Titles.Add(new SeriesTitle());
			}
			PieSeriesLabel templateLabel = DataContainer.SeriesTemplate.Label as PieSeriesLabel;
			if (templateLabel != null)
				templateLabel.Position = PieSeriesLabelPosition.TwoColumns;
			foreach (Series series in DataContainer.AutocreatedSeries) {
				series.LegendTextPattern = PatternUtils.ArgumentPattern;
				if (series.Label != null && DataContainer.SeriesTemplate.Label != null)
					series.Label.TextPattern = DataContainer.SeriesTemplate.Label.TextPattern;
				SimpleDiagramSeriesViewBase view = series.View as SimpleDiagramSeriesViewBase;
				if (view != null) {
					view.Titles.Clear();
					SeriesTitle title = new SeriesTitle();
					title.Text = series.Name;
					view.Titles.Add(title);
				}
				PieSeriesLabel label = series.Label as PieSeriesLabel;
				if (label != null)
					label.Position = PieSeriesLabelPosition.TwoColumns;
			}
			if (isLegendVisible)
				for (int i = 0; i < DataContainer.AutocreatedSeries.Count; i++)
					((Series)DataContainer.AutocreatedSeries[i]).ShowInLegend = i == visibleInLegendSeriesNumber;
			Chart.Legend.Visibility = DefaultBooleanUtils.ToDefaultBoolean(isLegendVisible || Chart.Legend.UseCheckBoxes);
		}
		void SetPivotGridTextPattern(bool isLegendVisible) {
			string pattern = isLegendVisible ? "{" + PatternUtils.ValuePlaceholder + "}" : "{" + PatternUtils.ArgumentPlaceholder + "}" + PatternUtils.ArgumentValueSeparator + "{" + PatternUtils.ValuePlaceholder + "}";
			DataContainer.SeriesTemplate.Label.PivotGridTextPattern = pattern;
			foreach (Series series in DataContainer.AutocreatedSeries)
				series.Label.PivotGridTextPattern = pattern;
		}
		int GetVisibleInLegendSeriesNumber() {
			int seriesNumber = -1;
			for (int i = 0; i < DataContainer.AutocreatedSeries.Count; i++) {
				bool isValidSeries = true;
				for (int j = 0; j < ((Series)DataContainer.AutocreatedSeries[i]).Points.Count; j++) {
					bool isValidPoint = true;
					for (int k = 0; k < DataContainer.AutocreatedSeries.Count; k++) {
						if (k == i)
							continue;
						else if (((Series)DataContainer.AutocreatedSeries[k]).Points.Count != ((Series)DataContainer.AutocreatedSeries[i]).Points.Count ||
							((Series)DataContainer.AutocreatedSeries[k]).Points[j].Argument != ((Series)DataContainer.AutocreatedSeries[i]).Points[j].Argument)
							return -1;
						else if (((Series)DataContainer.AutocreatedSeries[i]).Points[j].IsEmpty && !((Series)DataContainer.AutocreatedSeries[k]).Points[j].IsEmpty)
							isValidPoint = false;
					}
					if (!isValidPoint)
						isValidSeries = false;
				}
				if (isValidSeries && seriesNumber < 0)
					seriesNumber = i;
			}
			return seriesNumber;
		}
		protected override ChartElement CreateObjectForClone() {
			return new PivotGridDataSourceOptions(null);
		}
		internal AxisBase GetAxisX() {
			XYDiagram2DSeriesViewBase view = DataContainer.SeriesTemplate.View as XYDiagram2DSeriesViewBase;
			return view != null ? view.ActualAxisX as AxisBase : null;
		}
		internal Axis2D GetAxisY() {
			XYDiagram2DSeriesViewBase view = DataContainer.SeriesTemplate.View as XYDiagram2DSeriesViewBase;
			return view != null ? view.ActualAxisY as Axis2D : null;
		}
		internal void CheckData() {
			if (!HasPivotGrid)
				return;
			int availableSeriesCount = PivotGrid.AvailableSeriesCount;
			if (availableSeriesCount > DataContainer.AutocreatedSeries.Count && availableSeriesCount > 0) {
				PivotGridSeriesExcludedEventArgs e = new PivotGridSeriesExcludedEventArgs(DataContainer.AutocreatedSeries.Count, availableSeriesCount);
				IChartDataProvider dataProvider = Chart.ContainerAdapter.DataProvider;
				if (dataProvider != null)
					dataProvider.OnPivotGridSeriesExcluded(e);
			}
			IDictionary<object, int> availablePointCountBySeries = PivotGrid.AvailablePointCountInSeries;
			if (availablePointCountBySeries != null)
				foreach (Series series in DataContainer.AutocreatedSeries) {
					object key = series.Tag;
					if (key != null && availablePointCountBySeries.ContainsKey(key)) {
						int pointsCount = availablePointCountBySeries[key];
						if (pointsCount > series.Points.Count && pointsCount > 0) {
							PivotGridSeriesPointsExcludedEventArgs e = new PivotGridSeriesPointsExcludedEventArgs(series, series.Points.Count, pointsCount);
							IChartDataProvider dataProvider = Chart.ContainerAdapter.DataProvider;
							if (dataProvider != null)
								dataProvider.OnPivotGridSeriesPointsExcluded(e);
						}
					}
				}
		}
		internal void UpdateDataMembers(bool updateArgumentDataMember) {
			if (!locker.IsLocked && HasDataSource && AutoBindingSettingsEnabled) {
				locker.Lock();
				try {
					if (HasPivotGrid && updateArgumentDataMember)
							UpdateArgumentDataMembers();
					DevExpress.Data.Browsing.DataContext dataContext = Chart.ContainerAdapter.DataContext;
					if (DataContainer.SeriesTemplate.View.DateTimeValuesSupported && BindingHelper.CheckDataMember(dataContext, DataSource, DataSource.ValueDataMember, ScaleType.DateTime)) {
						DataContainer.SeriesTemplate.ValueScaleType = ScaleType.DateTime;
						SetValueDataMembers();
					}
					else if (BindingHelper.CheckDataMember(dataContext, DataSource, DataSource.ValueDataMember, ScaleType.Numerical)) {
						DataContainer.SeriesTemplate.ValueScaleType = ScaleType.Numerical;
						SetValueDataMembers();
					}
					else
						for(int i = 0; i < DataContainer.SeriesTemplate.ValueDataMembers.Count; i++)
							DataContainer.SeriesTemplate.ValueDataMembers[i] = String.Empty;
					Chart.ContainerAdapter.OnCustomizeAutoBindingSettings(EventArgs.Empty);
				}
				finally {
					locker.Unlock();
				}
			}
		}
		internal void UpdateAutoBindingSettings() {
			if (!locker.IsLocked && HasDataSource && AutoBindingSettingsEnabled) {
				locker.Lock();
				try {
					UpdateArgumentDataMembers();
					Chart.DataContainer.SeriesDataMember = DataSource.SeriesDataMember;
					Chart.DataContainer.SeriesTemplate.ArgumentDataMember = DataSource.ArgumentDataMember;
					UpdateDataMembers(false);
				}
				finally {
					locker.Unlock();
				}
			}
		}
		internal void UpdateAutoLayoutSettings() {
			if (!locker.IsLocked && HasDataSource && Chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled) {
				locker.Lock();
				try {
					axisLabelsIntervals.Clear();
					AxisBase axisX = GetAxisX();
					if (axisX != null)
						UpdateXYDiagram(axisX);
					else if(DataContainer.SeriesTemplate.View is SimpleDiagramSeriesViewBase)
						UpdateSimpleSeriesViews();
					if (DataSource.DateTimeArgumentMeasureUnit.HasValue)
						foreach (Series series in DataContainer.AutocreatedSeries)
							series.UpdateArgumentDateTimeFormat((DateTimeMeasureUnit)DataSource.DateTimeArgumentMeasureUnit.Value);
				}
				finally {
					locker.Unlock();
				}
				UpdateLegend();
			}
		}
		internal void UpdateXAxisLabels() {
			if (!locker.IsLocked && HasDataSource && Chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled) {
				locker.Lock();
				try {
					AxisBase axis = GetAxisX();
					if (axis != null) {
						SwiftPlotDiagramAxisXBase swiftPlotAxis = axis as SwiftPlotDiagramAxisXBase;
						AxisXBase xyAxis = axis as AxisXBase;
						if (swiftPlotAxis != null || xyAxis != null) {
							bool staggered;
							if (xyAxis != null)
								staggered = xyAxis.Label.Staggered;
							else
								staggered = swiftPlotAxis.Label.Staggered;
							CustomizeXAxisLabelsEventArgs e = new CustomizeXAxisLabelsEventArgs(axis, staggered);
							Chart.ContainerAdapter.OnPivotChartingCustomizeXAxisLabels(e);
							if (xyAxis != null)
								xyAxis.Label.Staggered = e.Staggered;
							else
								swiftPlotAxis.Label.Staggered = e.Staggered;
						}
					}
				}
				finally {
					locker.Unlock();
				}
			}
		}
		internal void UpdateAxisLabelItems(AxisBase axis, AxisLabelItemList items) {
			if (!HasDataSource || !AutoLayoutSettingsEnabled || !Object.ReferenceEquals(GetAxisX(), axis) || axisLabelsIntervals.Count == 0 ||
				axis.ScaleType != ActualScaleType.DateTime || !DataSource.DateTimeArgumentMeasureUnit.HasValue)
					return;
			DateTimeMeasureUnitNative measureUnit = DataSource.DateTimeArgumentMeasureUnit.Value;
			IPivotGrid pivotGrid = DataSource as IPivotGrid;
			if (pivotGrid != null)
				foreach (AxisLabelItemBase item in items) {
					bool visible = false;
					DateTime value = (DateTime)item.AxisValue;
					foreach (DateTimeInterval interval in axisLabelsIntervals)
						if (interval.Contains(value)) {
							visible = true;							
							break;
						}
					if (visible) {
						foreach (DateTime key in pivotGrid.DateTimeMeasureUnitByArgument.Keys)
							if (DateTimeUtils.Floor(key, measureUnit) == value) {
								DateTimeMeasureUnitNative itemMeasureUnit = pivotGrid.DateTimeMeasureUnitByArgument[key];
								if (itemMeasureUnit != measureUnit)
									item.Text = DateTimeOptionsHelper.GetValueText(value, new DateTimeOptionsProvider(itemMeasureUnit, value));
							}
					}
					else
						item.Text = String.Empty;
				}
		}
		internal void UpdateByDiagramBounds(Rectangle diagramBounds, IList<RefinedSeriesData> seriesDataList) {
			if (!HasDataSource || !AutoLayoutSettingsEnabled || DataContainer.SeriesTemplate.Label == null || !DataContainer.SeriesTemplate.Label.ResolveOverlappingSupported)
				return;
			ResolveOverlappingMode resolveOverlappingMode;
			if (DataContainer.SeriesTemplate.ActualLabelsVisibility)
				resolveOverlappingMode = DataContainer.SeriesTemplate.Label.GetResolveOverlappingMode(diagramBounds, seriesDataList);
			else
				resolveOverlappingMode = ResolveOverlappingMode.None;
			CustomizeResolveOverlappingModeEventArgs e = new CustomizeResolveOverlappingModeEventArgs(resolveOverlappingMode);
			Chart.ContainerAdapter.OnPivotChartingCustomizeResolveOverlappingMode(e);
			foreach (RefinedSeriesData seriesData in seriesDataList)
				SetResolveOverlappingMode(seriesData.Series, e.ResolveOverlappingMode);
		}
		internal void UpdateLegend() {
			if (!locker.IsLocked && HasDataSource && AutoLayoutSettingsEnabled) {
				locker.Lock();
				try {
					double maxHorizontalPercentage = 0;
					double maxVerticalPercentage = 0;
					LegendAlignmentHorizontal alignmentHorizontal = Chart.Legend.AlignmentHorizontal;
					LegendAlignmentVertical alignmentVertical = Chart.Legend.AlignmentVertical;
					bool isHorizontalOutside = alignmentHorizontal == LegendAlignmentHorizontal.RightOutside || 
											   alignmentHorizontal == LegendAlignmentHorizontal.LeftOutside;
					bool isVerticalOutside = alignmentVertical == LegendAlignmentVertical.BottomOutside || 
											 alignmentVertical == LegendAlignmentVertical.TopOutside;
					bool isCenter = alignmentVertical == LegendAlignmentVertical.Center && alignmentHorizontal == LegendAlignmentHorizontal.Center;
					if (isHorizontalOutside && isVerticalOutside || isCenter) {
						maxHorizontalPercentage = 30;
						maxVerticalPercentage = 30;
					}
					else if (isHorizontalOutside || isVerticalOutside) {
						maxHorizontalPercentage = isVerticalOutside ? 100 : 30;
						maxVerticalPercentage = isHorizontalOutside ? 100 : 30;
					}
					else {
						maxHorizontalPercentage = alignmentVertical == LegendAlignmentVertical.Center ? 30 : 60;
						maxVerticalPercentage = alignmentVertical == LegendAlignmentVertical.Center ? 60 : 30;
					}
					CustomizeLegendEventArgs e = new CustomizeLegendEventArgs(Chart.Legend, maxHorizontalPercentage, maxVerticalPercentage);
					Chart.ContainerAdapter.OnPivotChartingCustomizeLegend(e);
					Chart.Legend.MaxHorizontalPercentage = e.MaxHorizontalPercentage;
					Chart.Legend.MaxVerticalPercentage = e.MaxVerticalPercentage;
				}
				finally {
					locker.Unlock();
				}
			}
		}
		internal void Initialize(object dataSource, bool isSourceDeleted) {
			IChartDataSource chartDataSource = dataSource as IChartDataSource;
			if (chartDataSource == null && this.dataSource != null) {
				AxisBase axisX = GetAxisX();
				if (axisX != null) {
					axisX.SetSideMarginValue(AxisBase.DefaultSideMarginValue);
					AxisXBase axisXBase = axisX as AxisXBase;
					if (axisXBase != null)
						axisXBase.ScaleBreaks.Clear();
				}
				if (!isSourceDeleted && HasPivotGrid)
					PivotGrid.RetrieveDateTimeValuesAsMiddleValues = false;
			}
			this.dataSource = chartDataSource;
			if (HasPivotGrid)
				PivotGrid.RetrieveDateTimeValuesAsMiddleValues = true;
			UpdateAutoBindingSettings();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			PivotGridDataSourceOptions options = obj as PivotGridDataSourceOptions;
			if (options != null) {
				dataSource = options.dataSource;
				autoBindingSettingsEnabled = options.autoBindingSettingsEnabled;
				autoLayoutSettingsEnabled = options.autoLayoutSettingsEnabled;
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public static class PivotGridDataSourceUtils {
		public static bool HasDataSource(PivotGridDataSourceOptions options){ 
			return options.DataSource != null; 
		}
		public static bool HasPivotGrid(PivotGridDataSourceOptions options) {
			return (options.DataSource as IPivotGrid) != null;
		}
		public static IPivotGrid GetPivotGrid(PivotGridDataSourceOptions options) {
			return options.DataSource as IPivotGrid;
		}
		public static bool IsAutoBindingSettingsUsed(PivotGridDataSourceOptions options, SeriesBase series) {
			Chart chart = options.Chart;
			return series != null && object.Equals(series, chart.DataContainer.SeriesTemplate) && options.HasDataSource && options.AutoBindingSettingsEnabled;
		}
		public static bool IsAutoLayoutSettingsEnabled(PivotGridDataSourceOptions options, SeriesBase series) {
			Chart chart = options.Chart;
			return series != null && options.AutoLayoutSettingsEnabled && object.Equals(series, chart.DataContainer.SeriesTemplate) && options.HasDataSource;
		}
		public static bool IsAutoLayoutSettingsEnabledForSimpleView(PivotGridDataSourceOptions options, SeriesBase series) {
			return series.View is ISimpleSeriesView && IsAutoLayoutSettingsEnabled(options, series);
		}
		public static bool IsAutoLayoutSettingsEnabledForSimpleView(PivotGridDataSourceOptions options) {
			Chart chart = options.Chart;
			return chart.DataContainer.SeriesTemplate.View is ISimpleSeriesView && options.HasDataSource && options.AutoLayoutSettingsEnabled;
		}
		public static bool IsAutoLayoutSettingsEnabled(PivotGridDataSourceOptions options, AxisBase axis, bool checkAxisY) {
			bool isSameAxis = object.Equals(axis, options.GetAxisX());
			if(!isSameAxis && checkAxisY)
				isSameAxis = object.Equals(axis, options.GetAxisY());
			return axis != null && isSameAxis && options.HasDataSource && options.AutoLayoutSettingsEnabled;
		}
	}
}

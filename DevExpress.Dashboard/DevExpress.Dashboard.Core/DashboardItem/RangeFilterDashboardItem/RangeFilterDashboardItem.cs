#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.RangeFilter)
	]
	public class RangeFilterDashboardItem : SeriesDashboardItem, IArgumentsDashboardItem {
		internal const string xmlArgument = "Argument";
		readonly RangeFilterArgumentHolder rangeFilterArgumentHolder;
		internal static readonly List<SimpleSeriesType> compatibleSeriesTypes = new List<SimpleSeriesType>(
			new SimpleSeriesType[] { SimpleSeriesType.Line, SimpleSeriesType.StackedLine, SimpleSeriesType.FullStackedLine, 
									 SimpleSeriesType.Area, SimpleSeriesType.StackedArea, SimpleSeriesType.FullStackedArea });
		static readonly Dictionary<DateTimeGroupInterval, DateTimeGroupInterval> incompatibleDateTimeGroupIntervalsMapping = new Dictionary<DateTimeGroupInterval, DateTimeGroupInterval>();
		static readonly List<SeriesViewGroup> seriesViewGroups = new List<SeriesViewGroup>();
		internal static Dictionary<DateTimeGroupInterval, DateTimeGroupInterval> IncompatibleDateTimeGroupIntervalsMapping { get { return incompatibleDateTimeGroupIntervalsMapping; } }
		internal static IEnumerable<SeriesViewGroup> SeriesViewGroups {
			get {
				if(seriesViewGroups.Count == 0) {
					List<ChartSeriesConverter> convs = new List<ChartSeriesConverter>();
					foreach(SimpleSeriesType type in compatibleSeriesTypes)
						convs.Add(new ChartSimpleSeriesConverter(type));
					seriesViewGroups.Add(new SeriesViewGroup(DashboardLocalizer.GetString(DashboardStringId.SeriesTypeGroupRangeFilter), convs.ToArray()));
				}
				return seriesViewGroups;
			}
		}
		static RangeFilterDashboardItem() {
			incompatibleDateTimeGroupIntervalsMapping.Add(DateTimeGroupInterval.Quarter, DateTimeGroupInterval.QuarterYear);
			incompatibleDateTimeGroupIntervalsMapping.Add(DateTimeGroupInterval.Month, DateTimeGroupInterval.MonthYear);
			incompatibleDateTimeGroupIntervalsMapping.Add(DateTimeGroupInterval.Day, DateTimeGroupInterval.DayMonthYear);
			incompatibleDateTimeGroupIntervalsMapping.Add(DateTimeGroupInterval.Hour, DateTimeGroupInterval.DateHour);
			incompatibleDateTimeGroupIntervalsMapping.Add(DateTimeGroupInterval.Minute, DateTimeGroupInterval.DateHourMinute);
			incompatibleDateTimeGroupIntervalsMapping.Add(DateTimeGroupInterval.Second, DateTimeGroupInterval.DateHourMinuteSecond);
			incompatibleDateTimeGroupIntervalsMapping.Add(DateTimeGroupInterval.DayOfYear, DateTimeGroupInterval.DayMonthYear);
			incompatibleDateTimeGroupIntervalsMapping.Add(DateTimeGroupInterval.DayOfWeek, DateTimeGroupInterval.DayMonthYear);
			incompatibleDateTimeGroupIntervalsMapping.Add(DateTimeGroupInterval.WeekOfYear, DateTimeGroupInterval.DayMonthYear);
			incompatibleDateTimeGroupIntervalsMapping.Add(DateTimeGroupInterval.WeekOfMonth, DateTimeGroupInterval.DayMonthYear);
		}
		readonly RangeFilterSeriesCollection series = new RangeFilterSeriesCollection();
		readonly DataItemBox<Dimension> argumentBox;
		string errorMessage;
		ChartSeriesConverter defaultSeriesConverter = new ChartSimpleSeriesConverter(SimpleSeriesType.Line);
		object minValue;
		object maxValue;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("RangeFilterDashboardItemSeries"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.RangeFilterSeriesCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public RangeFilterSeriesCollection Series { get { return series; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("RangeFilterDashboardItemArgument"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableDimensionPropertyTypeConverter),
		RefreshProperties(RefreshProperties.Repaint),
		DefaultValue(null)
		]
		public Dimension Argument {
			get { return argumentBox.DataItem; }
			set { argumentBox.DataItem = value; }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("RangeFilterDashboardItemInteractivityOptions"),
#endif
 Category(CategoryNames.Interactivity),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public FilterableDashboardItemInteractivityOptions InteractivityOptions { get { return InteractivityOptionsBase; } }
		[
		Obsolete("The StartValue property is now obsolete"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public object StartValue { get { return null; } }
		[
		Obsolete("The EndValue property is now obsolete"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public object EndValue { get { return null; } }
		[
		Obsolete("The ActualMinValue property is now obsolete"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public object ActualMinValue { get { return null; } }
		[
		Obsolete("The ActualMaxValue property is now obsolete"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public object ActualMaxValue { get { return null; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public object MinValue {
			get { return minValue; }
			set {
				if (value != minValue) {
					minValue = value;
					OnRangeChanged();
				}
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public object MaxValue {
			get { return maxValue; }
			set {
				if (value != maxValue) {
					maxValue = value;
					OnRangeChanged();
				}
			}
		}
		internal string ErrorMessage { get { return errorMessage; } }
		internal ChartSeriesConverter DefaultSeriesConverter {
			get { return defaultSeriesConverter; }
			set {
				if(defaultSeriesConverter != value) {
					defaultSeriesConverter = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		protected internal override bool HasDataItems {
			get {
				if(Argument != null || base.HasDataItems)
					return true;
				foreach(ChartSeries item in series)
					if(item.DataItemsCount != 0)
						return true;
				return false;
			}
		}
		protected internal override bool SingleMasterFilterEnabled { get { return true; } }
		protected internal override bool MultipleMasterFilterEnabled { get { return false; } }
		protected internal override bool IsDrillDownEnabled {
			get { return false; }
			set {
			}
		}
		protected internal override DashboardItemMasterFilterMode MasterFilterMode {
			get { return DashboardItemMasterFilterMode.Single; }
			set {
			}
		}
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameRangeFilterItem); } }
		protected override IEnumerable<DataDashboardItemDescription> ValuesDescriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionValues),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), ItemKind.RangeFilterSeries, new RangeFilterSeriesDescription(SeriesViewGroups, (ChartSimpleSeriesConverter)DefaultSeriesConverter, series));
			}
		}
		protected override IEnumerable<DataDashboardItemDescription> ArgumentsDescriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemArgument),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemArgument), ItemKind.SingleDimension, rangeFilterArgumentHolder);
			}
		}
		protected override bool DefaultShowCaption { get { return false; } }
		public RangeFilterDashboardItem() {
			UpdateErrorMessage();
			series.CollectionChanged += (sender, e) => OnSeriesCollectionChanged(e.AddedItems, e.RemovedItems);
			rangeFilterArgumentHolder = new RangeFilterArgumentHolder(this);
			argumentBox = new DataItemBox<Dimension>((IDataItemContext)this, xmlArgument);
			argumentBox.Changed += (sender, e) => {
				minValue = null;
				maxValue = null;
				UpdateErrorMessage();
				OnDataItemsChanged(
					e.NewDataItem != null ? new Dimension[] { e.NewDataItem } : new Dimension[0],
					e.OldDataItem != null ? new Dimension[] { e.OldDataItem } : new Dimension[0]);
				rangeFilterArgumentHolder.OnDataItemChanged(sender, e);
			};
		}
		public void SetMinMaxValues(object newMin, object newMax) {
			if (newMin != minValue || newMax != maxValue) {
				minValue = newMin;
				maxValue = newMax;
				OnRangeChanged();
			}
		}
		public void ClearRange() {
			SetMinMaxValues(null, null);
		}
		protected override FilterableDashboardItemInteractivityOptions CreateInteractivityOptions() {
			return new FilterableDashboardItemInteractivityOptions(true);
		}		
		protected override bool ShouldRaiseMasterFilterChanged() {
			return base.ShouldRaiseMasterFilterChanged() && Series.Count > 0;
		}
		protected internal override bool IsSortingEnabled(Dimension dimension) {
			return false;
		}
		protected internal override bool IsSortingByMeasureEnabled(Dimension dimension, DashboardItemViewModel model) {
			return false;
		}
		protected internal override bool CanSpecifySortMode(Dimension dimension) {
			return false;
		}
		protected internal override bool CanSpecifyTopNOptions(Dimension dimension) {
			return false;
		}
		protected internal override bool CanSpecifyMeasureDateTimeFormat(Measure measure) {
			return false;
		}
		protected internal override bool CanSpecifyMeasureNumericFormat(Measure measure) {
			return false;
		}
		protected internal override bool CanSpecifyDimensionDateTimeFormat(Dimension dimension) {
			return Object.ReferenceEquals(dimension, Argument) && base.CanSpecifyDimensionDateTimeFormat(dimension);
		}
		protected internal override bool CanSpecifyDimensionNumericFormat(Dimension dimension) {
			return Object.ReferenceEquals(dimension, Argument) && base.CanSpecifyDimensionNumericFormat(dimension);
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			IList<ChartSeriesTemplateViewModel> seriesTemplates = new List<ChartSeriesTemplateViewModel>();
			foreach(SimpleSeries simpleSeries in series) {
				if(simpleSeries.Value != null)
					seriesTemplates.Add(simpleSeries.CreateSeriesTemplateViewModel());
			}
			return new RangeFilterDashboardItemViewModel(this, seriesTemplates);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			argumentBox.SaveToXml(element);
			series.SaveToXml(element);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			argumentBox.LoadFromXml(element);
			series.LoadFromXml(element);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			argumentBox.OnEndLoading();
			series.OnEndLoading(this);
			UpdateErrorMessage();
		}
		IList<Dimension> IArgumentsDashboardItem.Arguments {
			get {
				if(Argument != null)
					return new Dimension[] { Argument };
				else
					return new Dimension[0];
			}
		}
		void OnRangeChanged() {
			if (!Loading && Dashboard != null)
				Dashboard.OnRangeFilterRangeChanged(this);
		}
		protected override void OnChangedInternal(ChangedEventArgs e) {
			base.OnChangedInternal(e);
			UpdateErrorMessage();			
		}
		bool CheckValueInRange(object value, object start, object end) {
			int startResult = Comparer.Default.Compare(value, start);
			int endResult = Comparer.Default.Compare(value, end);
			if(startResult >= 0 && endResult <= 0)
				return true;
			return false;
		}
		void UpdateErrorMessage() {
			if(series.Count == 0)
				errorMessage = DashboardLocalizer.GetString(DashboardStringId.RangeFilterEmptySeries);
			else {
				errorMessage = String.Empty;
				foreach(SimpleSeries ser in series)
					if(!compatibleSeriesTypes.Contains(ser.SeriesType)) {
						errorMessage = DashboardLocalizer.GetString(DashboardStringId.RangeFilterIncompatibleSeriesType);
						return;
					}
				if(Argument == null)
					errorMessage = DashboardLocalizer.GetString(DashboardStringId.RangeFilterEmptyArgument);
				else {
					IDashboardDataSource dataSource = DataSource;
					if(dataSource != null) {
						DataFieldType fieldType = dataSource.GetFieldType(Argument.DataMember, DataMember);
						switch(fieldType) {
							case DataFieldType.DateTime:
								errorMessage = incompatibleDateTimeGroupIntervalsMapping.ContainsKey(Argument.DateTimeGroupInterval) ?
									DashboardLocalizer.GetString(DashboardStringId.RangeFilterIncompatibleDateTimeGroupInterval) : null;
								break;
							case DataFieldType.Integer:
							case DataFieldType.Float:
							case DataFieldType.Double:
							case DataFieldType.Decimal:
								errorMessage = Argument.IsDiscreteNumericScale ? DashboardLocalizer.GetString(DashboardStringId.RangeFilterIncompatibleNumericScale) : null;
								break;
							default:
								errorMessage = DashboardLocalizer.GetString(DashboardStringId.RangeFilterIncompatibleArgument);
								break;
						}
					}
					if(String.IsNullOrEmpty(errorMessage))
						if(Argument.SortOrder == DimensionSortOrder.Descending || Argument.SortByMeasure != null)
							errorMessage = DashboardLocalizer.GetString(DashboardStringId.RangeFilterIncompatibleArgumentSorting);
						else if(Argument.TopNOptions.Enabled)
							errorMessage = DashboardLocalizer.GetString(DashboardStringId.RangeFilterIncompatibleTopN);
				}
			}
		}
		void OnSeriesCollectionChanged(IEnumerable<SimpleSeries> addedSeries, IEnumerable<SimpleSeries> removedSeries) {
			UpdateErrorMessage();
			OnDataItemContainersChanged(addedSeries, removedSeries);
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			List<Dimension> dimensions = new List<Dimension>();
			if(Argument != null)
				dimensions.Add(Argument);
			builder.SetColumnHierarchyDimensions(DashboardDataAxisNames.ChartArgumentAxis, dimensions);
			builder.RowHierarchy = DashboardDataAxisNames.ChartSeriesAxis;
			foreach(ChartSeries series in Series) {
				foreach(Measure measure in series.Measures)
					builder.AddMeasure(measure);
			}
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.HasAdditionalDimensions = true;
			foreach(Dimension dimension in SeriesDimensions)
				description.AddAdditionalDimension(dimension);
			description.AddMainDimension(Argument);
			foreach(SimpleSeries series in Series) {
				description.AddSeries(series);
				foreach(Measure measure in series.Measures)
					description.AddMeasure(measure);
			}
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			Argument = description.SparklineArgument;
			AssignDimension(description.Latitude, SeriesDimensions);
			AssignDimension(description.Longitude, SeriesDimensions);
			foreach(Dimension dimension in description.MainDimensions)
				if(Argument == null && ((IDataItemHolder)rangeFilterArgumentHolder).IsCompatible(dimension, DataSourceSchema))
					Argument = dimension;
				else
					SeriesDimensions.Add(dimension);
			foreach(Dimension dimension in description.AdditionalDimensions)
				if(Argument == null && ((IDataItemHolder)rangeFilterArgumentHolder).IsCompatible(dimension, DataSourceSchema))
					Argument = dimension;
				else
					SeriesDimensions.Add(dimension);
			if(description.Series.Count != 0) {
				foreach(ChartSeries series in description.Series) {
					SimpleSeries simpleSeries = series as SimpleSeries;
					if(simpleSeries != null && compatibleSeriesTypes.Contains(simpleSeries.SeriesType))
						Series.Add(simpleSeries);
					else
						foreach(Measure measure in series.Measures)
							Series.Add(new SimpleSeries(measure, SimpleSeriesType.Line));
				}
			}
			else
				foreach(Measure measure in description.Measures)
					Series.Add(new SimpleSeries(measure, SimpleSeriesType.Line));
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(dimension == Argument)
				return DimensionGroupIntervalInfo.Continuous;
			return base.GetDimensionGroupIntervalInfo(dimension);
		}
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			return Series.SelectMany(s => s.Measures).ToList();
		}
		protected override SliceDataQuery GetDataQueryInternal(IActualParametersProvider provider) {
			var arguments = new Dimension[] { Argument }.NotNull();
			var series = SeriesDimensions;
			SliceDataQueryBuilder queryBuilder;
			ItemModelBuilder itemBuilder = new ItemModelBuilder(DataSourceModel.DataSourceInfo, GetDataItemUniqueName, provider);
			itemBuilder.MeasureConvertToDecimalForNonNumerical = true;
			if(IsBackCompatibilityDataSlicesRequired) {
				queryBuilder = SliceDataQueryBuilder.CreateWithPivotModel(itemBuilder, arguments, series,
					QueryMeasures, QueryFilterDimensions, GetQueryFilterCriteria(provider));
			} else {
				var dimensions = arguments.Concat(series).NotNull().ToList();
				queryBuilder = SliceDataQueryBuilder.CreateEmpty(itemBuilder, QueryFilterDimensions, GetQueryFilterCriteria(provider));
				queryBuilder.AddSlice(dimensions, QueryMeasures);
				queryBuilder.SetAxes(arguments, series);
			}
			return queryBuilder.FinalQuery();
		}
		protected override MultidimensionalDataDTO PrepareDataInternal(DataQueryResult queryResult, SliceDataQuery query, IActualParametersProvider parameters, ColorRepository coloringCache, IDictionary<string, object> filter) {
			MultidimensionalDataDTO multiData = base.PrepareDataInternal(queryResult, query, parameters, coloringCache, filter);
			if(DiscreteScaleEmulator.Emulate) {
				DiscreteScaleEmulator.PatchHierarchicalData(queryResult.HierarchicalData, Argument);
			}
			return multiData;
		}
	}
}
namespace DevExpress.DashboardCommon.Native {
	public static class DiscreteScaleEmulator {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool Emulate { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static object NextValue(object maxArgumentValue, Dimension dimension) {
			if(dimension == null || maxArgumentValue == null)
				return maxArgumentValue;
			DateTimeGroupInterval groupInterval = dimension.DateTimeGroupInterval;
			if(maxArgumentValue is DateTime) {
				return NextDateByGroupInterval((DateTime)maxArgumentValue, groupInterval);
			} else {
				return (int)maxArgumentValue + 1;
			}
		}
		internal static void PatchHierarchicalData(HierarchicalDataParams hData, Dimension dimension) {
			if(hData != null && dimension != null) {
				string argumentId = dimension.ActualId;
				DataStorage storage = hData.Storage;
				StorageColumn column = storage.CreateColumn(argumentId, true);
				StorageSlice slice = storage.GetSliceIfExists(new[] { column });
				if(slice != null && slice.Count() > 0) {
					StorageRow row = new StorageRow();
					object additionalValue = NextValue(slice.Last()[column].MaterializedValue, dimension);
					row[column] = StorageValue.CreateUnbound(additionalValue);
					slice.AddRow(row);
				}
			}
		}
		static DateTime NextDateByGroupInterval(DateTime source, DateTimeGroupInterval groupInterval) {
			switch(groupInterval) {
				case DateTimeGroupInterval.Year:
					return source.AddYears(1);
				case DateTimeGroupInterval.Quarter:
				case DateTimeGroupInterval.QuarterYear:
					return source.AddMonths(3);
				case DateTimeGroupInterval.Month:
				case DateTimeGroupInterval.MonthYear:
					return source.AddMonths(1);
				case DateTimeGroupInterval.Day:
				case DateTimeGroupInterval.DayOfYear:
				case DateTimeGroupInterval.DayOfWeek:
				case DateTimeGroupInterval.DayMonthYear:
					return source.AddDays(1);
				case DateTimeGroupInterval.WeekOfYear:
				case DateTimeGroupInterval.WeekOfMonth:
					return source.AddDays(7);
				case DateTimeGroupInterval.Hour:
				case DateTimeGroupInterval.DateHour:
					return source.AddHours(1);
				case DateTimeGroupInterval.Minute:
				case DateTimeGroupInterval.DateHourMinute:
					return source.AddMinutes(1);
				case DateTimeGroupInterval.Second:
				case DateTimeGroupInterval.DateHourMinuteSecond:
					return source.AddSeconds(1);
				case DateTimeGroupInterval.None:
					return source.AddMilliseconds(1);
				default:
					throw new Exception(Helper.GetUnknownEnumValueMessage(groupInterval));
			}
		}
	}
}

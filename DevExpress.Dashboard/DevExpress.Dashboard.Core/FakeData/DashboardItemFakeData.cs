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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DevExpress.Compatibility.System.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Native {
	public enum FakeDataAggregateType { Independent, Subcategory, Cartesian }
	static class FakeDataGeneratorFactory {
		static Dictionary<Type, Func<DataDashboardItem, IDataSourceSchema, DashboardItemFakeDataBase>> typeMap =
			new Dictionary<Type, Func<DataDashboardItem, IDataSourceSchema, DashboardItemFakeDataBase>>();
		static FakeDataGeneratorFactory() {
			typeMap.Add(typeof(GridDashboardItem), (item, dataSource) => new GridItemFakeData(item, dataSource));
			typeMap.Add(typeof(PivotDashboardItem), (item, dataSource) => new PivotItemFakeData(item, dataSource));
			typeMap.Add(typeof(ChartDashboardItem), (item, dataSource) => new ChartItemFakeData(item, dataSource));
			typeMap.Add(typeof(ScatterChartDashboardItem), (item, dataSource) => new ScatterChartItemFakeData(item, dataSource));
			typeMap.Add(typeof(PieDashboardItem), (item, dataSource) => new PieItemFakeData(item, dataSource));
			typeMap.Add(typeof(GaugeDashboardItem), (item, dataSource) => new GaugeItemFakeData(item, dataSource));
			typeMap.Add(typeof(CardDashboardItem), (item, dataSource) => new CardItemFakeData(item, dataSource));
			typeMap.Add(typeof(RangeFilterDashboardItem), (item, dataSource) => new RangeFilterItemFakeData(item, dataSource));
			typeMap.Add(typeof(ChoroplethMapDashboardItem), (item, dataSource) => new ChoroplethMapItemFakeData(item, dataSource));
			typeMap.Add(typeof(GeoPointMapDashboardItem), (item, dataSource) => new GeoPointMapItemBaseFakeData(item, dataSource));
			typeMap.Add(typeof(BubbleMapDashboardItem), (item, dataSource) => new BubbleMapItemFakeData(item, dataSource));
			typeMap.Add(typeof(PieMapDashboardItem), (item, dataSource) => new PieMapItemFakeData(item, dataSource));
			typeMap.Add(typeof(ComboBoxDashboardItem), (item, dataSource) => new FilterElementItemFakeData(item, dataSource));
			typeMap.Add(typeof(ListBoxDashboardItem), (item, dataSource) => new FilterElementItemFakeData(item, dataSource));
			typeMap.Add(typeof(TreeViewDashboardItem), (item, dataSource) => new FilterElementItemFakeData(item, dataSource));
		}
		public static DashboardItemFakeDataBase MakeGenerator(DataDashboardItem item, IDataSourceSchema dataSource) {
			Func<DataDashboardItem, IDataSourceSchema, DashboardItemFakeDataBase> createFunc = null;
			Type checkType = item.GetType();
			typeMap.TryGetValue(checkType, out createFunc);
			return createFunc == null ? null : createFunc(item, dataSource);
		}
	}
	abstract class DashboardItemFakeDataBase {
		readonly DataTable fakeDataTable;
		readonly IFieldsTypeMap fieldsMap;
		protected DataDashboardItem Item { get; set; }
		protected IFieldsTypeMap FieldsMap { get { return fieldsMap; } }
		public IList FakeListSource { get { return fakeDataTable.DefaultView; } }
		public DashboardItemFakeDataBase(DataDashboardItem dashboardItem, IDataSourceSchema dataSource) {
			this.Item = dashboardItem;
			fakeDataTable = new DataTable();
#if !DXPORTABLE
			fakeDataTable.Locale = CultureInfo.InvariantCulture;
#endif
			if (dataSource.RootNode != null)
				fieldsMap = new FieldsTypeMap(dataSource.RootNode);
			RegenerateFakeData();
		}
		public void RegenerateFakeData() {
			if (fieldsMap == null)
				return;
			HierarchyColumnAggregator aggregator = new HierarchyColumnAggregator(fieldsMap);
			AggregateHierarchy(aggregator);
			aggregator.AggregateSubcategoryIndependent(Item.DataItems, 0);
			ApplyValuesColumnToFakeTable(aggregator);
		}
		protected virtual void AggregateHierarchy(HierarchyColumnAggregator aggregator) { }
		void ApplyValuesColumnToFakeTable(HierarchyColumnAggregator aggregator) {
			Dictionary<string, IList<object>> dict = aggregator.GetValues();
			IList<object> firstList = dict.Values.FirstOrDefault();
			int rowCount = firstList != null ? firstList.Count() : 0;
			int colCount = dict.Keys.Count;
			fakeDataTable.Rows.Clear();
			fakeDataTable.Columns.Clear();
			foreach (string colName in dict.Keys)
				fakeDataTable.Columns.Add(colName, fieldsMap[colName]);
			for (int i = 0; i < rowCount; i++) {
				object[] row = new object[colCount];
				for (int j = 0; j < colCount; j++) {
					row[j] = dict[fakeDataTable.Columns[j].ColumnName][i];
				}
				fakeDataTable.Rows.Add(row);
			}
		}
		protected bool IsContainsCountSummaryType() {
			foreach (Measure measure in Item.Measures)
				if (measure.SummaryType == SummaryType.Count)
					return true;
			return false;
		}
	}
	class GridItemFakeData : DashboardItemFakeDataBase {
		GridDashboardItem GridItem { get { return (GridDashboardItem)Item; } }
		public GridItemFakeData(DataDashboardItem dashboardItem,IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			IEnumerable<DataItem> dataItems = GridItem.Columns.SelectMany(column => column.DataItems);
			IEnumerable<DataItem> dateTimeItems = dataItems.Where(x => x.DataFieldType == DataFieldType.DateTime);
			IEnumerable<DataItem> restItems = dataItems.Where(x => x.DataFieldType != DataFieldType.DateTime);
			aggregator.AggregateSubcategoryIndependent(restItems, 3);
			foreach (DataItem item in dateTimeItems) {
				aggregator.Aggregate(item, 24, FakeDataAggregateType.Independent);
			}
		}
	}
	class PivotItemFakeData : DashboardItemFakeDataBase {
		PivotDashboardItem PivotItem { get { return (PivotDashboardItem)Item; } }
		public PivotItemFakeData(DataDashboardItem dashboardItem, IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			aggregator.Aggregate(PivotItem.Columns.FirstOrDefault(), 3, FakeDataAggregateType.Cartesian);
			aggregator.Aggregate(PivotItem.Rows.FirstOrDefault(), 5, FakeDataAggregateType.Cartesian);
			List<DataItem> list = new List<DataItem>();
			list.AddRange(PivotItem.Columns.Skip(1));
			list.AddRange(PivotItem.Rows.Skip(1));
			aggregator.AggregateSubcategoryIndependent(list, 0);
		}
	}
	class ChartItemFakeData : DashboardItemFakeDataBase {
		ChartDashboardItem ChartItem { get { return (ChartDashboardItem)Item; } }
		public ChartItemFakeData(DataDashboardItem dashboardItem, IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			bool isSpecialViewForDateTime = FirstDataMember(ChartItem.SeriesDimensions) == FirstDataMember(ChartItem.Arguments)
				&& FirstType(ChartItem.SeriesDimensions) == FirstType(ChartItem.Arguments)
				&& FirstType(ChartItem.Arguments) == DataFieldType.DateTime
				&& ChartItem.SeriesDimensions.Count == 1;
			int cntArgs;
			int argCount = ChartItem.Arguments.Count;
			if (ChartItem.SeriesDimensions.Count == 0) {
				if (argCount == 1)
					cntArgs = 12;
				else
					cntArgs = 4;
			} else {
				if (argCount == 1)
					cntArgs = 6;
				else
					cntArgs = 3;
				if (isSpecialViewForDateTime) {
					cntArgs = 24;
				}
			}
			if (!isSpecialViewForDateTime)
				foreach (DataItem series in ChartItem.SeriesDimensions)
					aggregator.Aggregate(series, 2, FakeDataAggregateType.Independent);
			aggregator.Aggregate(ChartItem.Arguments.FirstOrDefault(), cntArgs, FakeDataAggregateType.Cartesian);
			if (IsContainsCountSummaryType())
				aggregator.DuplicateRows(10);
			FinanceSeriesSpecialData(aggregator);
		}
		private void FinanceSeriesSpecialData(HierarchyColumnAggregator aggregator) {
			Action<int, int, DataItem> aggregateField = (min, max, item) => {
				if (item != null && aggregator.CanAggregateField(item.DataMember)) {
					FakeDataGeneratorBase generator = new RangeDataGenerator(min, max, aggregator.IndependentGroupCount, FieldsMap[item.DataMember], item.DataMember);
					ValuesColumnWithGenerator column = new ValuesColumnWithGenerator(generator);
					aggregator.Aggregate(column, FakeDataAggregateType.Independent);
				}
			};
			OpenHighLowCloseSeries seriesOHLC = ChartItem.Series.OfType<OpenHighLowCloseSeries>().FirstOrDefault();
			HighLowCloseSeries seriesHLC = ChartItem.Series.OfType<HighLowCloseSeries>().FirstOrDefault();
			if (seriesOHLC != null) {
				aggregateField(0, 20, seriesOHLC.Low);
				aggregateField(80, 100, seriesOHLC.High);
				aggregateField(20, 80, seriesOHLC.Open);
				aggregateField(20, 80, seriesOHLC.Close);
			} else if (seriesHLC != null) {
				aggregateField(0, 20, seriesHLC.Low);
				aggregateField(80, 100, seriesHLC.High);
				aggregateField(20, 80, seriesHLC.Close);
			}
		}
		string FirstDataMember(DimensionCollection collection) {
			return collection.Count > 0 ? collection[0].DataMember : string.Empty;
		}
		DataFieldType FirstType(DimensionCollection collection) {
			return collection.Count > 0 ? collection[0].DataFieldType : DataFieldType.Unknown;
		}
	}
	class ScatterChartItemFakeData : DashboardItemFakeDataBase {
		ScatterChartDashboardItem ScatterItem { get { return (ScatterChartDashboardItem)Item; } }
		public ScatterChartItemFakeData(DataDashboardItem dashboardItem, IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			IEnumerable<DataItem> dataItems = ScatterItem.DataItems;
			IEnumerable<DataItem> dateTimeItems = dataItems.Where(x => x.DataFieldType == DataFieldType.DateTime);
			IEnumerable<DataItem> restItems = dataItems.Where(x => x.DataFieldType != DataFieldType.DateTime);
			aggregator.AggregateSubcategoryIndependent(restItems, 3);
			foreach(DataItem item in dateTimeItems) {
				aggregator.Aggregate(item, 24, FakeDataAggregateType.Independent);
			}
		}
	}
	class PieItemFakeData : DashboardItemFakeDataBase {
		PieDashboardItem PieItem { get { return (PieDashboardItem)Item; } }
		public PieItemFakeData(DataDashboardItem dashboardItem, IDataSourceSchema dataSource) : base(dashboardItem, dataSource) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			int subcategoryCount = 0;
			List<DataItem> list = new List<DataItem>();
			if (PieItem.IsDrillDownEnabled) {
				if (PieItem.IsDrillDownEnabledOnArguments) {
					list.AddRange(PieItem.Arguments);
					list.AddRange(PieItem.SeriesDimensions);
					subcategoryCount = 3;
				} else {
					list.AddRange(PieItem.SeriesDimensions);
					list.AddRange(PieItem.Arguments);
					subcategoryCount = 3;
				}
			} else {
				list.AddRange(PieItem.SeriesDimensions.Take(1));
				list.AddRange(PieItem.Arguments.Take(1));
				subcategoryCount = 2;
			}
			aggregator.AggregateSubcategoryIndependent(list, subcategoryCount);
		}
	}
	class GaugeItemFakeData : DashboardItemFakeDataBase {
		GaugeDashboardItem GaugeItem { get { return (GaugeDashboardItem)Item; } }
		public GaugeItemFakeData(DataDashboardItem dashboardItem,IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			aggregator.AggregateSubcategoryIndependent(GaugeItem.SeriesDimensions, 2);
		}
	}
	class CardItemFakeData : DashboardItemFakeDataBase {
		CardDashboardItem CardItem { get { return (CardDashboardItem)Item; } }
		public CardItemFakeData(DataDashboardItem dashboardItem,IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			aggregator.NumericPercentMode = true;
			aggregator.EnableSpread = true;
			aggregator.AggregateSubcategoryIndependent(CardItem.SeriesDimensions, 2);
			if (IsContainsCountSummaryType())
				aggregator.DuplicateRows(3);
		}
	}
	class RangeFilterItemFakeData : DashboardItemFakeDataBase {
		RangeFilterDashboardItem RangeFilterItem { get { return (RangeFilterDashboardItem)Item; } }
		public RangeFilterItemFakeData(DataDashboardItem dashboardItem,IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			List<DataItem> list = new List<DataItem>();
			list.Add(RangeFilterItem.Argument);
			list.AddRange(RangeFilterItem.SeriesDimensions);
			aggregator.AggregateSubcategoryIndependent(list, 1);
			if (IsContainsCountSummaryType())
				aggregator.DuplicateRows(3);
		}
	}
	class ChoroplethMapItemFakeData : DashboardItemFakeDataBase {
		ChoroplethMapDashboardItem MapItem { get { return (ChoroplethMapDashboardItem)Item; } }
		public ChoroplethMapItemFakeData(DataDashboardItem dashboardItem,IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			if (MapItem.AttributeDimension != null) {
				ValuesColumnWithGenerator column = new ValuesColumnWithGenerator(new StringListDataGenerator(GetAttributeNames(), MapItem.AttributeDimension.DataMember));
				aggregator.Aggregate(column, FakeDataAggregateType.Independent);
			}
			aggregator.AggregateSubcategoryIndependent(MapItem.Measures, 0);
		}
		IEnumerable<string> GetAttributeNames() {
			IEnumerable<MapShapePath> items = MapItem.MapItems.Select(x => x as MapShapePath).Where(x => x != null).Cast<MapShapePath>();
			items = items.OrderByDescending(x => OrderPosition(x)).Take(7);
			return items.Select(x => x.Attributes.Where(y => y.Name == MapItem.AttributeName).FirstOrDefault().Value as string);
		}
		double OrderPosition(MapShapePath path) {
			Func<ShapePathSegment, double> getSegmentWidth = segment => {
				IEnumerable<double> longitudeValues = segment.Points.Select(p => p.Longitude);
				double minValue = longitudeValues.OrderBy(x => x).FirstOrDefault();
				double maxValue = longitudeValues.OrderByDescending(x => x).FirstOrDefault();
				return Math.Abs(maxValue - minValue);
			};
			ShapePathSegment maxSegment = path.Segments.OrderByDescending(x => getSegmentWidth(x)).FirstOrDefault();
			return getSegmentWidth(maxSegment);
		}
	}
	class GeoPointMapItemBaseFakeData : DashboardItemFakeDataBase {
		GeoPointMapDashboardItemBase MapItem { get { return (GeoPointMapDashboardItemBase)Item; } }
		protected virtual int PointsCount { get { return 20; } }
		public GeoPointMapItemBaseFakeData(DataDashboardItem dashboardItem,IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			aggregator.IndependentGroupCount = PointsCount;
			Action<DataItem, double, double, int> AggregateCoordinateColumn = (item, min, max, shuffleParam) => {
				if (item != null && aggregator.CanAggregateField(item.DataMember)) {
					FakeDataGeneratorBase generator = new RangeDataGenerator(min, max, aggregator.IndependentGroupCount, FieldsMap[item.DataMember], item.DataMember);
					generator.ShuffleParam = shuffleParam;
					ValuesColumnWithGenerator column = new ValuesColumnWithGenerator(generator);
					aggregator.Aggregate(column, FakeDataAggregateType.Independent);
				}
			};
			double LongitudeValue1 = Math.Min(MapItem.Viewport.LeftLongitude, MapItem.Viewport.RightLongitude);
			double LongitudeValue2 = Math.Max(MapItem.Viewport.LeftLongitude, MapItem.Viewport.RightLongitude);
			double LatitudeValue1 = Math.Min(MapItem.Viewport.BottomLatitude, MapItem.Viewport.TopLatitude);
			double LatitudeValue2 = Math.Max(MapItem.Viewport.BottomLatitude, MapItem.Viewport.TopLatitude);
			if (MapItem.Longitude != null && MapItem.Latitude != null && MapItem.Longitude.DataMember == MapItem.Latitude.DataMember) {
				double value1 = Math.Max(LongitudeValue1, LatitudeValue1);
				double value2 = Math.Min(LongitudeValue2, LatitudeValue2);
				AggregateCoordinateColumn(MapItem.Latitude, value1, value2, 3);
			} else {
				AggregateCoordinateColumn(MapItem.Longitude, LongitudeValue1, LongitudeValue2, 1);
				AggregateCoordinateColumn(MapItem.Latitude, LatitudeValue1, LatitudeValue2, 3);
			}
		}
	}
	class GeoPointMapItemFakeData : GeoPointMapItemBaseFakeData {
		GeoPointMapDashboardItem MapItem { get { return (GeoPointMapDashboardItem)Item; } }
		protected override int PointsCount { get { return 10; } }
		public GeoPointMapItemFakeData(DataDashboardItem dashboardItem,IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			base.AggregateHierarchy(aggregator);
			aggregator.AggregateSubcategoryIndependent(MapItem.Measures, 0);
		}
	}
	class BubbleMapItemFakeData : GeoPointMapItemBaseFakeData {
		BubbleMapDashboardItem MapItem { get { return (BubbleMapDashboardItem)Item; } }
		public BubbleMapItemFakeData(DataDashboardItem dashboardItem,IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			base.AggregateHierarchy(aggregator);
			aggregator.Aggregate(MapItem.Weight, PointsCount / 2, FakeDataAggregateType.Independent);
			aggregator.Aggregate(MapItem.Color, PointsCount / 2, FakeDataAggregateType.Independent);
		}
	}
	class PieMapItemFakeData : GeoPointMapItemBaseFakeData {
		PieMapDashboardItem PieMapItem { get { return (PieMapDashboardItem)Item; } }
		public PieMapItemFakeData(DataDashboardItem dashboardItem,IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			base.AggregateHierarchy(aggregator);
			aggregator.Aggregate(PieMapItem.Argument, 3, FakeDataAggregateType.Cartesian);
			foreach (DataItem item in PieMapItem.Values)
				aggregator.Aggregate(item, 3 * PointsCount, FakeDataAggregateType.Independent);
		}
	}
	class FilterElementItemFakeData : DashboardItemFakeDataBase {
		FilterElementDashboardItem FilterElementItem { get { return (FilterElementDashboardItem)Item; } }
		public FilterElementItemFakeData(DataDashboardItem dashboardItem,IDataSourceSchema pickManager) : base(dashboardItem, pickManager) { }
		protected override void AggregateHierarchy(HierarchyColumnAggregator aggregator) {
			base.AggregateHierarchy(aggregator);
			aggregator.AggregateSubcategoryIndependent(FilterElementItem.FilterDimensions, 3);
		}
	}
}

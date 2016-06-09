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
using DevExpress.Data;
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public enum SummaryFunction {
		Average = SummaryItemType.Average,
		Count = SummaryItemType.Count,
		Max = SummaryItemType.Max,
		Min = SummaryItemType.Min,
		Sum = SummaryItemType.Sum
	};
	public abstract class ChartDataSourceAdapter : DataSourceAdapterBase, IChartDataAdapterCore, ILegendDataProvider, ISizeLegendSupport {
		public static readonly DependencyProperty SummaryFunctionProperty = DependencyPropertyManager.Register("SummaryFunction",
		  typeof(SummaryFunction), typeof(ChartDataSourceAdapter), new PropertyMetadata(SummaryFunction.Sum, SummaryFunctionPropertyChanged));
		public static readonly DependencyProperty MeasureRulesProperty = DependencyPropertyManager.Register("MeasureRules",
		   typeof(MeasureRules), typeof(ChartDataSourceAdapter), new PropertyMetadata(null, MeasureRulesPropertyChanged));
		public static readonly DependencyProperty ItemIdDataMemberProperty = DependencyPropertyManager.Register("ItemIdDataMember",
			typeof(string), typeof(ChartDataSourceAdapter), new PropertyMetadata(null, MappingsPropertyChanged));
		public static readonly DependencyProperty ItemMinSizeProperty = DependencyPropertyManager.Register("ItemMinSize",
		   typeof(double), typeof(ChartDataSourceAdapter), new PropertyMetadata(DefaultItemMinSize, ItemMinSizePropertyChanged, MinSizeCoerce), new ValidateValueCallback(ValidateItemSize));
		public static readonly DependencyProperty ItemMaxSizeProperty = DependencyPropertyManager.Register("ItemMaxSize",
		   typeof(double), typeof(ChartDataSourceAdapter), new PropertyMetadata(DefaultItemMaxSize, ItemMaxSizePropertyChanged, MaxSizeCoerce), new ValidateValueCallback(ValidateItemSize));
		static bool ValidateItemSize(object itemSize) {
			if ((double)itemSize <= 0.0)
				throw new ArgumentException(DXMapStrings.MsgIncorrectItemSize);
			return true;
		}
		static void ItemSizeCoerce(ChartDataSourceAdapter dataAdapter, double minValue, double maxValue) {
			if (minValue > maxValue && dataAdapter.Layer != null && dataAdapter.Layer.IsLoaded)
				throw new ArgumentException(DXMapStrings.MsgIncorrectItemMinMaxSize);
		}
		static object MinSizeCoerce(DependencyObject d, object baseValue) {
			ChartDataSourceAdapter dataAdapter = d as ChartDataSourceAdapter;
			if (dataAdapter != null)
				ItemSizeCoerce(dataAdapter, (double)baseValue, dataAdapter.ItemMaxSize);
			return baseValue;
		}
		static object MaxSizeCoerce(DependencyObject d, object baseValue) {
			ChartDataSourceAdapter dataAdapter = d as ChartDataSourceAdapter;
			if (dataAdapter != null)
				ItemSizeCoerce(dataAdapter, dataAdapter.ItemMinSize, (double)baseValue);
			return baseValue;
		}
		static void MeasureRulesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartDataSourceAdapter adapter = d as ChartDataSourceAdapter;
			if (adapter != null) {
				CommonUtils.SetOwnerForValues(e.OldValue, e.NewValue, adapter);
				adapter.ItemSizeChanged();
			}
		}
		static void ItemMinSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartDataSourceAdapter adapter = d as ChartDataSourceAdapter;
			if (adapter != null) {
				adapter.ActualItemMinSize = Math.Max(Math.Min((double)e.NewValue, adapter.ActualItemMaxSize), 0);
				adapter.ItemSizeChanged();
			}
		}
		static void ItemMaxSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartDataSourceAdapter adapter = d as ChartDataSourceAdapter;
			if(adapter != null) {
				adapter.ActualItemMaxSize = Math.Max((double)e.NewValue, adapter.ActualItemMinSize);
				adapter.ItemSizeChanged();
			}
		}
		static void SummaryFunctionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartDataSourceAdapter adapter = d as ChartDataSourceAdapter;
			if (adapter != null)
				adapter.LoadDataInternal();
		}
		[Category(Categories.Data)]
		public SummaryFunction SummaryFunction {
			get { return (SummaryFunction)GetValue(SummaryFunctionProperty); }
			set { SetValue(SummaryFunctionProperty, value); }
		}
		[Category(Categories.Data)]
		public MeasureRules MeasureRules {
			get { return (MeasureRules)GetValue(MeasureRulesProperty); }
			set { SetValue(MeasureRulesProperty, value); }
		}
		[Category(Categories.Data)]
		public string ItemIdDataMember {
			get { return (string)GetValue(ItemIdDataMemberProperty); }
			set { SetValue(ItemIdDataMemberProperty, value); }
		}
		[Category(Categories.Layout)]
		public double ItemMinSize {
			get { return (double)GetValue(ItemMinSizeProperty); }
			set { SetValue(ItemMinSizeProperty, value); }
		}
		[Category(Categories.Layout)]
		public double ItemMaxSize {
			get { return (double)GetValue(ItemMaxSizeProperty); }
			set { SetValue(ItemMaxSizeProperty, value); }
		}
		const SummaryFunction DefaultSummaryFunction = SummaryFunction.Sum;
		internal const double DefaultItemMaxSize = 50.0;
		internal const double DefaultItemMinSize = 5.0;
		double actualItemMaxSize = DefaultItemMaxSize;
		double actualItemMinSize = DefaultItemMinSize;
		protected abstract string ValueDataMember { get; }
		protected internal double ActualItemMaxSize { get { return actualItemMaxSize; } set { actualItemMaxSize = value; } }
		protected internal double ActualItemMinSize { get { return actualItemMinSize; } set { actualItemMinSize = value; } }
		protected internal override bool CanUpdateItemsOnly { get { return false; } }
		#region IChartDataAdapterCore implementation
		double IChartDataAdapterCore.ItemMaxSize { get { return ActualItemMaxSize; } }
		double IChartDataAdapterCore.ItemMinSize { get { return ActualItemMinSize; } }
		IMeasureRules IChartDataAdapterCore.MeasureRules { get { return MeasureRules; } }
		IEnumerable<IMapItemCore> IChartDataAdapterCore.Items { get { return (IEnumerable<IMapItemCore>)(ItemsCollection); } }
		#endregion
		#region ILegendDataProvider members
		IList<MapLegendItemBase> ILegendDataProvider.CreateItems(MapLegendBase legend) {
			return CreateLegendItems(legend as SizeLegend);
		}
		#endregion
		#region ISizeLegendSupport
		double ISizeLegendSupport.MaxItemSize { get { return ActualItemMaxSize; } }
		MarkerType ISizeLegendSupport.MarkerType { get { return GetMarkerType(); } }
		#endregion
		IList<MapLegendItemBase> CreateLegendItems(SizeLegend legend) {
			if(legend != null && MeasureRules != null) {
				ILegendDataProvider rulesProvider = MeasureRules as ILegendDataProvider;
				IList<MapLegendItemBase> result = (rulesProvider != null) ? rulesProvider.CreateItems(legend) : new List<MapLegendItemBase>();
				return result;
			}
			return new List<MapLegendItemBase>();
		}
		void InitializeDataAggregator(IMapDataAggregator aggregator) {
			aggregator.SummaryColumn = ValueDataMember;
			aggregator.AggregationGroups.AddRange(CreateAggregationGroups());
		}
		protected abstract IMapDataEnumerator CreateAggregatedDataEnumerator(MapDataController controller);
		protected virtual MarkerType GetMarkerType() {
			return MarkerType.Circle;
		}
		protected internal abstract GroupInfo[] CreateAggregationGroups();
		protected internal virtual bool CanAggregateData() {
			return !string.IsNullOrEmpty(ItemIdDataMember) && DataMappings != null && !string.IsNullOrEmpty(ValueDataMember);
		}
		protected internal override IMapDataEnumerator CreateDataEnumerator(MapDataController controller) {
			return CanAggregateData() ? CreateAggregatedDataEnumerator(controller) : new ListDataEnumerator(controller);
		}
		protected internal override void FillActualMappings(MappingDictionary mappings) {
			base.FillActualMappings(mappings);
			if (!string.IsNullOrEmpty(ItemIdDataMember)) {
				mappings.Add(MappingType.Id, new ChartItemIdMapping(ItemIdDataMember));
			}
		}
		protected internal override void OnDataLoaded() {
			base.OnDataLoaded();
			UpdateItemsSize();
		}
		protected internal override void Aggregate(MapDataController dataController) {
			if (!CanAggregateData())
				return;
			IMapDataAggregator aggregator = new MapDataAggregator((SummaryItemType)SummaryFunction);
			InitializeDataAggregator(aggregator);
			aggregator.Aggregate(dataController);
		}
		internal void ItemSizeChanged() {
			UpdateItemsSize();
			if (Layer != null)
				Layer.UpdateLegends();
		}
		public virtual void UpdateItemsSize() {
			ChartItemsSizeCalculator sizeCalculator = new ChartItemsSizeCalculator(this, LinearRangeDistribution.Default);
			sizeCalculator.UpdateItemsSize();
		}
		public int[] GetItemListSourceIndices(object item) {
			return DataController.GetItemListSourceIndices(item);
		}
	}
}

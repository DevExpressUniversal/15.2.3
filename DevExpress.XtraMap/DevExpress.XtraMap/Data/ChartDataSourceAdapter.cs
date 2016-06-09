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
using System.Drawing.Design;
using DevExpress.Data;
using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public abstract class ChartDataSourceAdapter : DataSourceAdapterBase, IMapChartDataAdapter, ILegendDataProvider, IChartDataAdapterCore {
		internal const SummaryFunction DefaultSummaryFunction = SummaryFunction.Sum;
		internal const int DefaultItemSize = 5;
		internal const int DefaultItemMaxSize = 50;
		internal const int DefaultItemMinSize = 5;
		MeasureRules measureRules;
		int itemMaxSize = DefaultItemMaxSize;
		int itemMinSize = DefaultItemMinSize;
		string chartItemDataMember = string.Empty;
		readonly ChartItemsSizeCalculator sizeCalculator;
		SummaryFunction summaryFunction = DefaultSummaryFunction;
		protected internal ChartItemsSizeCalculator ChartItemsSizeCalculator { get { return this.sizeCalculator; } }
		protected internal string ChartItemDataMember { 
			get { return chartItemDataMember; } 
			set {
				if (object.Equals(chartItemDataMember, value))
					return;
				this.chartItemDataMember = value;
				OnChartItemDataMemberChanged();
			}
		}
		public new MapChartItemMappingInfo Mappings { 
			get { return (MapChartItemMappingInfo)base.Mappings; } 
		}
		[Category(SRCategoryNames.Data), 
		DefaultValue(DefaultSummaryFunction),
#if !SL
	DevExpressXtraMapLocalizedDescription("ChartDataSourceAdapterSummaryFunction")
#else
	Description("")
#endif
]
		public SummaryFunction SummaryFunction {
			get { return summaryFunction; }
			set {
				if(summaryFunction == value)
					return;
				summaryFunction = value;
				OnDataPropertyChanged();
			}
		}
		[Category(SRCategoryNames.Map), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.MeasureRulesPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
#if !SL
	DevExpressXtraMapLocalizedDescription("ChartDataSourceAdapterMeasureRules")
#else
	Description("")
#endif
		]
		public MeasureRules MeasureRules {
			get { return measureRules; }
			set {
				if(Object.Equals(measureRules, value))
					return;
				MapUtils.SetOwner(measureRules, null);
				measureRules = value;
				MapUtils.SetOwner(value, this);
				OnChartPropertyChanged();
			}
		}
		[Category(SRCategoryNames.Layout), 
		DefaultValue(DefaultItemMinSize),
#if !SL
	DevExpressXtraMapLocalizedDescription("ChartDataSourceAdapterItemMinSize")
#else
	Description("")
#endif
]
		public int ItemMinSize {
			get { return itemMinSize; }
			set {
				int newMinSize = ValidateMinSize(value);
				if(newMinSize == itemMinSize)
					return;
				itemMinSize = newMinSize;
				OnChartPropertyChanged();
			}
		}
		[Category(SRCategoryNames.Layout), 
		DefaultValue(DefaultItemMaxSize),
#if !SL
	DevExpressXtraMapLocalizedDescription("ChartDataSourceAdapterItemMaxSize")
#else
	Description("")
#endif
]
		public int ItemMaxSize {
			get { return itemMaxSize; }
			set {
				int newMaxSize = ValidateMaxSize(value);
				if(newMaxSize == itemMaxSize)
					return;
				itemMaxSize = newMaxSize;
				OnChartPropertyChanged();
			}
		}
		protected ChartDataSourceAdapter() {
			sizeCalculator = new ChartItemsSizeCalculator(this, LinearRangeDistribution.Default);
		}
		#region IMapChartDataAdapter members
		void IMapChartDataAdapter.OnChartItemsUpdated() {
			UpdateItemsSize();
		}
		#endregion
		#region ILegendDataProvider members
		IList<MapLegendItemBase> ILegendDataProvider.CreateItems(MapLegendBase legend) {
			return CreateLegendItems(legend as SizeLegend);
		}
		#endregion
		#region IChartDataAdapterCore members
		IMeasureRules IChartDataAdapterCore.MeasureRules { get { return MeasureRules; } }
		double IChartDataAdapterCore.ItemMaxSize { get { return ItemMaxSize; } }
		double IChartDataAdapterCore.ItemMinSize { get { return ItemMinSize; } }
		IEnumerable<IMapItemCore> IChartDataAdapterCore.Items { get { return (IEnumerable<IMapItemCore>)InnerItems; } }
		#endregion
		IList<MapLegendItemBase> CreateLegendItems(SizeLegend legend) {
			if (legend != null && MeasureRules != null) {
				ILegendDataProvider rulesProvider = MeasureRules as ILegendDataProvider;
				IList<MapLegendItemBase> result = (rulesProvider != null) ? rulesProvider.CreateItems(legend) : new List<MapLegendItemBase>();
				return result;
			}
			return new List<MapLegendItemBase>();
		}
		int ValidateMinSize(int value) {
			return Math.Max(Math.Min(value, ItemMaxSize), 0);
		}
		int ValidateMaxSize(int value) {
			return Math.Max(value, ItemMinSize);
		}
		protected abstract IMapDataEnumerator CreateAggregatedDataEnumerator(MapDataController controller);
		protected virtual int MeasureChartItemSizeInPixels(double value) {
			return Convert.ToInt32(value);
		}
		protected virtual void OnChartItemDataMemberChanged() { }
		protected virtual void OnChartPropertyChanged() {
			UpdateItemsSize();
			NotifyDataChanged(MapUpdateType.Style | MapUpdateType.ViewInfo);
		}
		protected virtual void InitializeDataAggregator(IMapDataAggregator aggregator) {
			aggregator.SummaryColumn = Mappings.Value;
			aggregator.AggregationGroups.AddRange(CreateAggregationGroups());
		}
		protected override MapUpdateType GetDataChangedEventUpdates() {
			return base.GetDataChangedEventUpdates() | MapUpdateType.ViewInfo;
		}
		protected override void OnItemListenerChanged(object sender, EventArgs e) {
			UpdateItemsSize();
			base.OnItemListenerChanged(sender, e);
		}
		protected override void OnItemCollectionChanged(object sender, CollectionChangedEventArgs<MapItem> e) {
			UpdateItemsSize();
			base.OnItemCollectionChanged(sender, e);
		}
		protected internal abstract GroupInfo[] CreateAggregationGroups();
		protected internal virtual bool CanAggregateData() {
			return !string.IsNullOrEmpty(ChartItemDataMember) && !string.IsNullOrEmpty(Mappings.Value);
		}
		protected internal override void FillActualMappings(MappingCollection mappings) {
			base.FillActualMappings(mappings);
			if(!string.IsNullOrEmpty(ChartItemDataMember)) { 
				mappings.Add(new ChartItemArgumentMapping() { Member = ChartItemDataMember });
			}
		}
		protected internal override IMapDataEnumerator CreateDataEnumerator(MapDataController controller) {
			return CanAggregateData() ? CreateAggregatedDataEnumerator(controller) : new ListDataEnumerator(controller);
		}
		protected internal override void Aggregate(DataController controller) {
			if(!CanAggregateData())
				return;
			IMapDataAggregator aggregator = new MapDataAggregator((SummaryItemType)SummaryFunction);
			InitializeDataAggregator(aggregator);
			aggregator.Aggregate(controller);
		}
		protected internal void UpdateItemsSize() {
			sizeCalculator.UpdateItemsSize();
		}
		protected internal void MeasureRulesChanged() {
			UpdateItemsSize();
		}
		public int[] GetItemListSourceIndices(object item) {
			return DataManager.GetItemListSourceIndices(item);
		}
	}
}

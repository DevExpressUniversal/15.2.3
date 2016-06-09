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
using System.Collections.Generic;
using System.Drawing.Design;
using DevExpress.Map;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System.Drawing;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap {
	public abstract class MapItemValueProviderBase : IMeasuredItemValueProvider {
		double IMeasuredItemValueProvider.GetValue(object item) {
			return GetValue(item as MapItem);
		}
		protected abstract double GetValue(MapItem item);
	}
	public class MapItemAttributeValueProvider : MapItemValueProviderBase {
		string attributeName = string.Empty;
		[DefaultValue(""),
		Description("")]
		public string AttributeName {
			get { return attributeName; }
			set {
				if(attributeName == value)
					return;
				attributeName = value;
			}
		}
		protected override double GetValue(MapItem item) {
			if(item == null) return double.NaN;
			MapItemAttribute attr = item.Attributes[AttributeName];
			if(attr != null && attr.Value != null)
				return Convert.ToDouble(attr.Value);
			return double.NaN;
		}
		public override string ToString() {
			return "(MapItemAttributeValueProvider)";
		}
	}
	public class MeasureRules : IOwnedElement, ILegendDataProvider, IMeasureRules {
		internal const bool DefaultAproximateValues = false;
		object owner;
		IRangeDistribution rangeDistribution;
		IMeasuredItemValueProvider valueProvider;
		bool approximateValues = DefaultAproximateValues;
		readonly DoubleCollection rangeStops;
		protected internal ChartDataSourceAdapter Data { get { return owner != null ? owner as ChartDataSourceAdapter : null; } }
		[DefaultValue(DefaultAproximateValues), 
		Category(SRCategoryNames.Behavior),
#if !SL
	DevExpressXtraMapLocalizedDescription("MeasureRulesApproximateValues")
#else
	Description("")
#endif
]
		public bool ApproximateValues {
			get { return approximateValues; }
			set {
				if(approximateValues == value)
					return;
				approximateValues = value;
				OnChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DoubleCollection RangeStops { get { return rangeStops; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraMap.Design.MapItemValueProviderPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		DefaultValue(null),
#if !SL
	DevExpressXtraMapLocalizedDescription("MeasureRulesValueProvider")
#else
	Description("")
#endif
		]
		public IMeasuredItemValueProvider ValueProvider {
			get {
				return valueProvider;
			}
			set {
				if(valueProvider == value)
					return;
				valueProvider = value;
				OnChanged();
			}
		}
		[
		Category(SRCategoryNames.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraMap.Design.RangeDistributionPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
#if !SL
	DevExpressXtraMapLocalizedDescription("MeasureRulesRangeDistribution"),
#endif
		DefaultValue(null)
		]
		public IRangeDistribution RangeDistribution {
			get {
				return rangeDistribution == null ? LinearRangeDistribution.Default : rangeDistribution;
			}
			set {
				if(rangeDistribution == value)
					return;
				rangeDistribution = value;
				OnChanged();
			}
		}
		public MeasureRules() {
			rangeStops = new DoubleCollection();
		}
		#region IOwnedElementImplementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				owner = value;
				OnOwnerChanged();
			}
		}
		#endregion
		#region IMeasureRules implementation
		IList<double> IMeasureRules.RangeStops { get { return RangeStops; } }
		IRangeDistribution IMeasureRules.RangeDistribution { get { return RangeDistribution; } }
		double IMeasureRules.GetValue(IMapChartItem item) {
			return ValueProvider != null ? ValueProvider.GetValue(item) : item.Value;
		}
		#endregion
		protected virtual void OnOwnerChanged() {
			if(Data == null) {
				UnsubscribeEvents();
			} else
				SubscribeEvents();
		}
		protected void SubscribeEvents() {
			rangeStops.CollectionChanged += OnRangeStopsCollectionChanged;
		}
		protected void UnsubscribeEvents() {
			rangeStops.CollectionChanged -= OnRangeStopsCollectionChanged;
		}
		void OnRangeStopsCollectionChanged(object sender, CollectionChangedEventArgs<double> e) {
			OnChanged();
		}
		void OnChanged() {
			if(Data != null) Data.MeasureRulesChanged();
		}
		IList<MapLegendItemBase> ILegendDataProvider.CreateItems(MapLegendBase legend) {
			return CreateLegendItems();
		}
		protected virtual IList<MapLegendItemBase> CreateLegendItems() {
			MeasureRulesLegendItemsBuilder builder = CreateLegendItemBuilder();
			return builder != null ? builder.CreateItems() : new List<MapLegendItemBase>();
		}
		protected virtual MeasureRulesLegendItemsBuilder CreateLegendItemBuilder() {
			return new MeasureRulesLegendItemsBuilder(this);
		}
		public override string ToString() {
			return "(MeasureRules)";
		}
	}
	public class ChartItemValueProvider : MapItemValueProviderBase {
		protected override double GetValue(MapItem item) {
			if(item == null) return double.NaN;
			IMapChartItem chart = item as IMapChartItem;
			return chart != null ? chart.Value : double.NaN;
		}
		public override string ToString() {
			return "(ChartItemValueProvider)";
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class MeasureRulesLegendItemsBuilder {
		MeasureRules rules;
		protected MeasureRules Rules { get { return rules; } }
		public MeasureRulesLegendItemsBuilder(MeasureRules rules) {
			Guard.ArgumentNotNull(rules, "rules");
			this.rules = rules;
		}
		public virtual List<MapLegendItemBase> CreateItems() {
			List<MapLegendItemBase> result = new List<MapLegendItemBase>();
			ChartDataSourceAdapter data = rules.Data;
			if(data == null) 
				return result;
			IList<double> ranges = CoreUtils.SortDoubleCollection(Rules.RangeStops);
			int rangesCount = ranges.Count;
			ChartItemsSizeCalculator sizeCalculator = new ChartItemsSizeCalculator(data, LinearRangeDistribution.Default);
			for(int i = 0; i < rangesCount; i++) {
				double value = ranges[i];
				int radius = (int)sizeCalculator.CalculateItemSize(value);
				string text = string.Empty;
				result.Add(CreateLegendItem(radius, text, value));
			}
			return result;
		}
		MapLegendItemBase CreateLegendItem(int itemSize, string text, double value) {
			return new SizeLegendItem() { MarkerSize = itemSize, Text = text, Value = value };
		}
	}
}

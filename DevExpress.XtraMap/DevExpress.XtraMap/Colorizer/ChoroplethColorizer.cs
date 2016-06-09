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

using System.Collections.Generic;
using DevExpress.Map;
using System.Drawing;
using System;
using DevExpress.Map.Native;
using System.ComponentModel;
using DevExpress.Utils;
using System.Drawing.Design;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public class ChoroplethColorizer : GenericColorizer<ColorizerColorItem>, ILegendDataProvider {
		const bool DefaultAproximateColors = false;
		IRangeDistribution rangeDistribution;
		IColorizerValueProvider valueProvider;
		readonly DoubleCollection rangeStops;
		bool approximateColors = DefaultAproximateColors;
		protected internal new GenericColorizerItemCollection<ColorizerColorItem> ActualColorItems {
			get { return (GenericColorizerItemCollection<ColorizerColorItem>)base.ActualColorItems; }
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ChoroplethColorizerColorItems"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GenericColorizerItemCollection<ColorizerColorItem> ColorItems {
			get { return (GenericColorizerItemCollection<ColorizerColorItem>)base.ColorItems; }
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ChoroplethColorizerValueProvider"),
#endif
		Category(SRCategoryNames.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraMap.Design.ColorizerValueProviderPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		DefaultValue(null)
		]
		public IColorizerValueProvider ValueProvider {
			get {
				return valueProvider;
			}
			set {
				if (Object.Equals(valueProvider, value))
					return;
				if (valueProvider != null)
					valueProvider.Changed -= OnValueProviderChanged;
				valueProvider = value;
				if (valueProvider != null)
					valueProvider.Changed += OnValueProviderChanged;
				OnColorizerChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ChoroplethColorizerRangeDistribution"),
#endif
		Category(SRCategoryNames.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraMap.Design.RangeDistributionPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))
		]
		public IRangeDistribution RangeDistribution {
			get {
				return rangeDistribution == null ? LinearRangeDistribution.Default : rangeDistribution;
			}
			set {
				if (rangeDistribution == value)
					return;
				rangeDistribution = value;
				OnColorizerChanged();
			}
		}
		bool ShouldSerializeRangeDistribution() { return !Object.Equals(rangeDistribution, LinearRangeDistribution.Default); }
		void ResetRangeDistribution() { RangeDistribution = LinearRangeDistribution.Default; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ChoroplethColorizerApproximateColors"),
#endif
		Category(SRCategoryNames.Behavior), DefaultValue(DefaultAproximateColors)]
		public bool ApproximateColors {
			get { return approximateColors; }
			set {
				if (value == approximateColors)
					return;
				approximateColors = value;
				OnColorizerChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ChoroplethColorizerRangeStops"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DoubleCollection RangeStops { get { return rangeStops; } }
		public ChoroplethColorizer() {
			rangeStops = new DoubleCollection();
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			SubscribeRangeStopsEvents();
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			UnsubscribeRangeStopsEvents();
		}
		protected internal IList<double> GetSortedRangeStops() {
			return MapUtils.SortDoubleCollection(RangeStops);
		}
		void SubscribeRangeStopsEvents() {
			rangeStops.CollectionChanged += OnRangeStopsCollectionChanged;
		}
		void UnsubscribeRangeStopsEvents() {
			rangeStops.CollectionChanged -= OnRangeStopsCollectionChanged;
		}
		void OnValueProviderChanged(object sender, EventArgs e) {
			OnColorizerChanged();
		}
		void OnRangeStopsCollectionChanged(object sender, CollectionChangedEventArgs<double> e) {
			OnColorizerChanged();
		}
		Color GetColor(GenericColorizerItemCollection<ColorizerColorItem> coloredItems, IList<double> sortedRangeStops, double value) {
			ColorCollection colors = GetColorCollection(coloredItems);
			return ColorizerColorHelper.CalculateValue(colors, sortedRangeStops, ApproximateColors, RangeDistribution, value);
		}
		protected override ColorizerColorItem CreateColorItem(Color color) {
			return new ColorizerColorItem(color);
		}
		public override void ColorizeElement(IColorizerElement element) {
			if(element == null || ValueProvider == null)
				return;
			double value = ValueProvider.GetValue(element);
			if(double.IsNaN(value) || double.IsInfinity(value))
				 return;
			element.ColorizerColor = GetColor(ActualColorItems, GetSortedRangeStops(), value);
		}
		public override string ToString() {
			return "(ChoroplethColorizer)";
		}
		public Color GetColor(double value) {
			return GetColor(ActualColorItems, GetSortedRangeStops(), value);
		}
		IList<MapLegendItemBase> ILegendDataProvider.CreateItems(MapLegendBase legend) {
			return CreateLegendItems();
		}
		protected virtual IList<MapLegendItemBase> CreateLegendItems() {
			ColorizerLegendItemsBuilderBase builder = CreateLegendItemBuilder();
			return builder != null ? builder.CreateItems() : new List<MapLegendItemBase>();
		}
		protected virtual ColorizerLegendItemsBuilderBase CreateLegendItemBuilder() {
			return new ChoroplethColorizerLegendItemsBuilder(this);
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class ChoroplethColorizerLegendItemsBuilder : ColorizerLegendItemsBuilderBase {
		protected new ChoroplethColorizer Colorizer { get { return (ChoroplethColorizer)base.Colorizer; } }
		public ChoroplethColorizerLegendItemsBuilder(ChoroplethColorizer colorizer) : base(colorizer) {
		}
		public override List<MapLegendItemBase> CreateItems() {
			List<MapLegendItemBase> result = new List<MapLegendItemBase>();
			DoubleCollection ranges = Colorizer.RangeStops;
			int rangesCount = ranges.Count;
			for(int i = 0; i < rangesCount; i++) {
				double value = ranges[i];
				Color color = Colorizer.GetColor(value);
				string text = string.Empty;
				result.Add(CreateLegendItem(color, text, value));
			}
			return result;
		}
	}
}

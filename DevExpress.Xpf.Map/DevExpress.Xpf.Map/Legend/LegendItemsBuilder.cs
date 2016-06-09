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
using System.Windows.Media;
using DevExpress.Map.Native;
using DevExpress.Utils;
namespace DevExpress.Xpf.Map.Native {
	public interface ISizeLegendSupport {
		double MaxItemSize { get; }
		MarkerType MarkerType { get; }
	}
	public abstract class LegendItemsBuilderBase {
		MapLegendBase legend;
		protected MapLegendBase Legend { get { return legend; } }
		protected LegendItemsBuilderBase(MapLegendBase legend) {
			Guard.ArgumentNotNull(legend, "legend");
			this.legend = legend;
		}
		protected void AddItem(List<MapLegendItemBase> result, MapLegendItemBase item) {
			if(Legend.ReverseItems)
				result.Insert(0, item);
			else
				result.Add(item);
		}
	}
	public abstract class ColorizerLegendItemsBuilderBase : LegendItemsBuilderBase {
		MapColorizer colorizer;
		protected MapColorizer Colorizer { get { return colorizer; } }
		protected ColorizerLegendItemsBuilderBase(MapLegendBase legend, MapColorizer colorizer):base(legend) {
			Guard.ArgumentNotNull(colorizer, "colorizer");
			this.colorizer = colorizer;
		}
		public abstract List<MapLegendItemBase> CreateItems();
		protected MapLegendItemBase CreateLegendItem(Color color, string text, double value) {
			MapLegendItemBase item = new ColorLegendItem() { Color = color, Text = text, Value = value, Format = Legend.RangeStopsFormat };
			return item;
		}
	}
	public class KeyValueColorizerLegendItemsBuilder : ColorizerLegendItemsBuilderBase {
		protected new KeyColorColorizer Colorizer { get { return (KeyColorColorizer)base.Colorizer; } }
		public KeyValueColorizerLegendItemsBuilder(MapLegendBase legend, KeyColorColorizer colorizer)
			: base(legend, colorizer) {
		}
		public override List<MapLegendItemBase> CreateItems() {
			List<MapLegendItemBase> result = new List<MapLegendItemBase>();
			if (Colorizer.ActualKeyItems != null) {
				foreach (ColorizerKeyItem item in Colorizer.ActualKeyItems) {
					MapLegendItemBase legendItem = CreateLegendItem(item.Color, item.Text, double.NaN);
					AddItem(result, legendItem);
				}
			}
			return result;
		}
	}
	public class MeasureRulesLegendItemsBuilder : LegendItemsBuilderBase {
		MeasureRules rules;
		protected MeasureRules Rules { get { return rules; } }
		new SizeLegend Legend { get { return base.Legend as SizeLegend; } }
		public MeasureRulesLegendItemsBuilder(MapLegendBase legend, MeasureRules rules)
			: base(legend) {
			Guard.ArgumentNotNull(rules, "rules");
			this.rules = rules;
		}
		public virtual List<MapLegendItemBase> CreateItems() {
			List<MapLegendItemBase> result = new List<MapLegendItemBase>();
			ChartDataSourceAdapter data = rules.DataAdapter;
			if(data == null)
				return result;
			IList<double> ranges = CoreUtils.SortDoubleCollection(Rules.RangeStops);
			int rangesCount = ranges.Count;
			ChartItemsSizeCalculator sizeCalculator = new ChartItemsSizeCalculator(data, LinearRangeDistribution.Default);
			ISizeLegendSupport legendData = rules.DataAdapter as ISizeLegendSupport;
			for(int i = 0; i < rangesCount; i++) {
				double value = ranges[i];
				double radius = sizeCalculator.CalculateItemSize(value);
				radius = GetOddValue(radius);
				string text = string.Empty;
				SizeLegendItem legendItem = CreateLegendItem(radius, text, value);
				ApplyItemAppearance(legendItem, legendData);
				AddItem(result, legendItem);
			}
			return result;
		}
		double GetOddValue(double source) {
			return (((int)source / 2) * 2) + 1;
		}
		void ApplyItemAppearance(SizeLegendItem legendItem, ISizeLegendSupport legendData) {
			legendItem.Fill = Legend.ItemFill;
			legendItem.Stroke = Legend.ItemStroke;
			legendItem.ShowTickMark = Legend.ShowTickMarks;
			if(legendData != null) {
				legendItem.MaxItemSize = GetOddValue(legendData.MaxItemSize);
				legendItem.MarkerType = legendData.MarkerType;
			}
		}
		SizeLegendItem CreateLegendItem(double itemSize, string text, double value) {
			return new SizeLegendItem() { MarkerSize = itemSize, Text = text, Value = value };
		}
	}
	public class ChoroplethColorizerLegendItemsBuilder : ColorizerLegendItemsBuilderBase {
		protected new ChoroplethColorizer Colorizer { get { return (ChoroplethColorizer)base.Colorizer; } }
		public ChoroplethColorizerLegendItemsBuilder(MapLegendBase legend, ChoroplethColorizer colorizer)
			: base(legend, colorizer) {
		}
		public override List<MapLegendItemBase> CreateItems() {
			List<MapLegendItemBase> result = new List<MapLegendItemBase>();
			DoubleCollection ranges = Colorizer.RangeStops;
			int rangesCount = ranges.Count;
			for(int i = 0; i < rangesCount; i++) {
				double value = ranges[i];
				Color color = Colorizer.GetColor(value);
				string text = string.Empty;
				MapLegendItemBase legendItem = CreateLegendItem(color, text, value);
				AddItem(result, legendItem);
			}
			return result;
		}
	}
}

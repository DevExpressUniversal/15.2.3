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
using System.Linq;
using System.Text;
namespace DevExpress.Map.Native {
	public interface IChartDataAdapterCore {
		IMeasureRules MeasureRules { get; }
		IEnumerable<IMapItemCore> Items { get; }
		double ItemMaxSize { get; }
		double ItemMinSize { get; }
	}
	public interface IMeasureRules {
		IList<double> RangeStops { get; }
		bool ApproximateValues { get; }
		IRangeDistribution RangeDistribution { get; }
		double GetValue(IMapChartItem item);
	}
	public class ChartItemsSizeCalculator {
		readonly IChartDataAdapterCore dataAdapter;
		readonly IEnumerable<IMapItemCore> items;
		readonly IRangeDistribution defaultDistribution;
		IChartDataAdapterCore DataAdapter { get { return dataAdapter; } }
		IMeasureRules MeasureRules { get { return DataAdapter.MeasureRules; } }
		public double ItemMaxValue { get; private set; }
		public double ItemMinValue { get; private set; }
		public ChartItemsSizeCalculator(IChartDataAdapterCore dataAdapter, IRangeDistribution defaultDistribution) {
			DevExpress.Utils.Guard.ArgumentNotNull(dataAdapter, "dataAdapter");
			this.dataAdapter = dataAdapter;
			this.items = dataAdapter.Items;
			this.defaultDistribution = defaultDistribution;
			ResetItemMinMaxValue();
		}
		bool UseDefaultRangeStops() {
			IMeasureRules measureRules = MeasureRules;
			return measureRules == null || measureRules.RangeStops == null || measureRules.RangeStops.Count == 0;
		}
		void ResetItemMinMaxValue() {
			ItemMaxValue = Double.NegativeInfinity;
			ItemMinValue = Double.PositiveInfinity;
		}
		void UpdateMinMaxValue() {
			ResetItemMinMaxValue();
			foreach (IMapChartItem chartItem in items) {
				ItemMinValue = Math.Min(ItemMinValue, chartItem.Value);
				ItemMaxValue = Math.Max(ItemMaxValue, chartItem.Value);
			}
		}
		void UpdateChartItemsSize() {
			foreach (IMapChartItem item in items) {
				double itemValue = MeasureRules != null ? MeasureRules.GetValue(item) : item.Value;
				item.ValueSizeInPixels = CalculateItemSize(itemValue);
			}
		}
		public double CalculateItemSize(double value) {
			double diameter;
			IList<double> sizes = new List<double>() { DataAdapter.ItemMinSize, DataAdapter.ItemMaxSize };
			List<double> ranges = new List<double>();
			if (UseDefaultRangeStops())
				ranges.AddRange(new double[] { ItemMinValue, ItemMaxValue });
			else
				ranges.AddRange(CoreUtils.SortDoubleCollection(MeasureRules.RangeStops));
			if (MeasureRules == null)
				diameter = MeasureRulesHelper.CalculateValue(sizes, ranges, true, defaultDistribution, value);
			else
				diameter = MeasureRulesHelper.CalculateValue(sizes, ranges, MeasureRules.ApproximateValues, MeasureRules.RangeDistribution, value);
			return diameter;
		}
		public virtual void UpdateItemsSize() {
			if (UseDefaultRangeStops())
				UpdateMinMaxValue();
			UpdateChartItemsSize();
		}
	}
}

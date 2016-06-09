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
using System.Linq;
using System;
using DevExpress.PivotGrid.QueryMode.Sorting;
namespace DevExpress.PivotGrid.DataCalculation {
	class AggregationLevelsCalculator<TObject, TData> : SummaryCalculator {
		ICalculationContext<TObject, TData> source;
		ICalculationSource<TObject, TData> data;
		public AggregationLevelsCalculator(ICalculationContext<TObject, TData> source)
			: base() {
			this.source = source;
			this.data = source.GetValueProvider();
		}
		public void Calculate(IList<AggregationLevel> aggregationLevels) {
			foreach(AggregationLevel level in aggregationLevels)
				foreach(AggregationCalculation calc in level) {
					ResetSummary();
					foreach(AggregationItemValue item in calc.OrderBy((ci) => ci.SummaryType.IsTopPercentage() ? 1 : 0)) {
						object value;
						if(calc.Target == AggregationCalculatationTarget.Data)
							value = CalculateSummary(EnumerateValues(level.Row, level.Column, calc.Index).Where((a) => !IsNull(a)), item.SummaryType, item.SummaryArgument);
						else {
							if(item.SummaryType == Data.SummaryItemTypeEx.Count) {
								value = source.EnumerateFullLevel(true, level.Column).Count() * source.EnumerateFullLevel(false, level.Row).Count();
							} else {
								IEnumerable<TObject> data;
								if(calc.Target == AggregationCalculatationTarget.Column) {
									data = source.EnumerateFullLevel(true, calc.Index);
								} else {
									data = source.EnumerateFullLevel(false, calc.Index);
								}
								if(item.SummaryType == Data.SummaryItemTypeEx.Max) {
									value = source.GetDisplayValue(MaxMin(data, s => source.GetValue(s), true));
								} else
									if(item.SummaryType == Data.SummaryItemTypeEx.Min) {
										value = source.GetDisplayValue(MaxMin(data, s => source.GetValue(s), false));
									} else
										value = CalculateSummary(data.Select(s => source.GetValue(s)), item.SummaryType, item.SummaryArgument);
							}
						}
						item.SetValue(value);
					}
				}
		}
		static TSource MaxMin<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector, bool isMax) {
			return default(TSource) == null ? NullableMaxMin(source, selector, isMax) : NonNullableMaxMin(source, selector, isMax);
		}
		static TSource NonNullableMaxMin<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector, bool isMax) {
			IComparer<TResult> comparer = Comparer<TResult>.Default;
			if(!isMax)
				comparer = new InversedComparer<TResult>(comparer);
			using(IEnumerator<TSource> iterator = source.GetEnumerator()) {
				TSource max = iterator.Current;
				TResult maxValue = selector(max);
				while(iterator.MoveNext()) {
					TSource item = iterator.Current;
					TResult itemResult = selector(item);
					if(comparer.Compare(maxValue, itemResult) < 0) {
						max = item;
						maxValue = itemResult;
					}
				}
				return max;
			}
		}
		static TSource NullableMaxMin<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector, bool isMax) {
			IComparer<TResult> comparer = Comparer<TResult>.Default;
			if(!isMax)
				comparer = new InversedComparer<TResult>(comparer);
			TSource max = default(TSource);
			TResult maxValue = selector(max);
			foreach(TSource item in source) {
				TResult itemResult = selector(item);
				if(item != null) {
					if(max == null) {
						max = item;
						maxValue = itemResult;
					} else {
						if(comparer.Compare(maxValue, itemResult) < 0) {
							max = item;
							maxValue = itemResult;
						}
					}
				}
			}
			return max;
		} 
		IEnumerable<object> EnumerateValues(int rowLevel, int columnLevel, int dataIndex) {
			TData tdata = source.GetData(dataIndex);
			foreach(TObject column in source.EnumerateFullLevel(true, columnLevel))
				foreach(TObject row in source.EnumerateFullLevel(false, rowLevel))
					yield return data.GetValue(column, row, tdata);
		}
	}
}

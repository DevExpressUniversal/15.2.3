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
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.DataCalculation {
	class SummaryCalculator {
		protected static bool IsNull(object obj) {
			return ReferenceEquals(null, obj);
		}
		static PivotValueComparerBase comparer = new PivotValueComparerBase();
		int? count = null;
		PivotSummaryValue summary = null;
		public SummaryCalculator() {
		}
		protected void ResetSummary() {
			count = null;
			summary = null;
		}
		public object CalculateSummary(IEnumerable<object> enumerable, SummaryItemTypeEx summaryType, decimal argument) {
			PivotSummaryType pivotSummaryType = summaryType.ToPivotSummaryType();
			if(pivotSummaryType == PivotSummaryType.Custom) {
				switch(summaryType) {
					case SummaryItemTypeEx.Bottom:
						return enumerable.OrderBy((a) => a).Take(Convert.ToInt32(argument)).ToArray();
					case SummaryItemTypeEx.Top:
						return enumerable.OrderByDescending((a) => a).Take(Convert.ToInt32(argument)).ToArray();
					case SummaryItemTypeEx.BottomPercent:
					case SummaryItemTypeEx.TopPercent: {
							if(!count.HasValue)
								if(summary != null)
									count = summary.Count;
								else
									count = enumerable.Count();
							int select = Convert.ToInt32(Math.Min(count.Value, count.Value * argument / 100));
							return new object[] { 
								((object[])CalculateSummary(enumerable, summaryType == SummaryItemTypeEx.TopPercent ? SummaryItemTypeEx.Top : SummaryItemTypeEx.Bottom, select)).LastOrDefault() 
							};
						}
					default:
						return PivotCellValue.ErrorValue.Value;
				}
			} else
				return CalculateSummary(enumerable, pivotSummaryType);
		}
		public object CalculateSummary(IEnumerable<object> enumerable, PivotSummaryType pivotSummaryType) {
			if(pivotSummaryType == PivotSummaryType.Max)
				return enumerable.Max();
			if(pivotSummaryType == PivotSummaryType.Min)
				return enumerable.Min();
			if(pivotSummaryType == PivotSummaryType.Count)
				return enumerable.Where(v => !(object.ReferenceEquals(null, v) || v is DBNull)).Count();
			if(summary == null) {
				summary = new PivotSummaryValue(comparer);
				foreach(object obj in enumerable)
					if(!IsNull(obj)) {
						decimal val;
						try {
							val = Convert.ToDecimal(obj);
						} catch {
							summary.SetSummaryError();
							val = 0;
						}
						summary.AddValue(obj, val);
					}
			}
			return summary.GetValue(pivotSummaryType);
		}
	}
}

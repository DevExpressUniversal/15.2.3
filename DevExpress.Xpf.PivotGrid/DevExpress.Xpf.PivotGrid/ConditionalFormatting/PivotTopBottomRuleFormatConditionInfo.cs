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
using System.Linq;
using DevExpress.Data;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class PivotTopBottomRuleFormatConditionInfo : TopBottomRuleFormatConditionInfo {
		public PivotTopBottomRuleFormatConditionInfo() {
		}
		public override bool CalcCondition(FormatValueProvider provider) {
			if(Rule == Core.ConditionalFormatting.TopBottomRule.AboveAverage || Rule == Core.ConditionalFormatting.TopBottomRule.BelowAverage)
				return base.CalcCondition(provider);
			SummaryItemTypeEx ex;
			switch(Rule) {
				case Core.ConditionalFormatting.TopBottomRule.BottomItems:
					ex = SummaryItemTypeEx.Bottom;
					break;
				case Core.ConditionalFormatting.TopBottomRule.BottomPercent:
					ex = SummaryItemTypeEx.BottomPercent;
					break;
				case Core.ConditionalFormatting.TopBottomRule.TopItems:
					ex = SummaryItemTypeEx.Top;
					break;
				case Core.ConditionalFormatting.TopBottomRule.TopPercent:
					ex = SummaryItemTypeEx.TopPercent;
					break;
				default:
					throw new ArgumentException(Rule.ToString());
			}
			AggregationCalculation val = provider.GetTotalSummaryValue(ConditionalFormatSummaryType.SortedList) as AggregationCalculation;
			if(val == null || provider.Value == null)
				return false;
			AggregationItemValueStorage agg = (AggregationItemValueStorage)val.Find((f) => f.SummaryType == ex && f.SummaryArgument == Convert.ToDecimal(Threshold));
			if(agg == null)
				return false;
			object[] result = agg.Result as object[];
			if(result == null || result.Length == 0)
				return false;
			double value = Convert.ToDouble(result.Last());
			double providerValue = Convert.ToDouble(provider.Value);
			switch(Rule) {
				case Core.ConditionalFormatting.TopBottomRule.BottomItems:
				case Core.ConditionalFormatting.TopBottomRule.BottomPercent:
					return value >= providerValue;
				case Core.ConditionalFormatting.TopBottomRule.TopItems:
				case Core.ConditionalFormatting.TopBottomRule.TopPercent:
					return value <= providerValue;
				default:
					throw new ArgumentException(Rule.ToString());
			}
		}
	}
}

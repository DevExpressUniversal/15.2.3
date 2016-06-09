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
using System.Windows;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.ConditionalFormatting.Printing;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
using DevExpress.Xpf.PivotGrid.Internal;
namespace DevExpress.Xpf.PivotGrid {
	public abstract class ExpressionConditionBase : FormatConditionBase {
		[XtraSerializableProperty(XtraSerializationVisibility.Content), XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public Format Format {
			get { return (Format)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(Format), typeof(ExpressionConditionBase), new PropertyMetadata(null, OnFormatChanged, OnCoerceFreezable));
		public override DependencyProperty FormatPropertyForBinding { get { return FormatProperty; } }
		protected override void UpdateEditUnit(BaseEditUnit unit) {
			base.UpdateEditUnit(unit);
			ConditionEditUnit conditionEditUnit = unit as ConditionEditUnit;
			if(conditionEditUnit != null)
				conditionEditUnit.Format = Format;
		}
	}
	public class FormatCondition : ExpressionConditionBase {
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public ConditionRule ValueRule {
			get { return (ConditionRule)GetValue(ValueRuleProperty); }
			set { SetValue(ValueRuleProperty, value); }
		}
		public static readonly DependencyProperty ValueRuleProperty = DependencyProperty.Register("ValueRule", typeof(ConditionRule), typeof(FormatCondition), new PropertyMetadata(ConditionRule.Expression, (d, e) => ((FormatCondition)d).OnInfoPropertyChanged(e)));
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public object Value1 {
			get { return (object)GetValue(Value1Property); }
			set { SetValue(Value1Property, value); }
		}
		public static readonly DependencyProperty Value1Property = DependencyProperty.Register("Value1", typeof(object), typeof(FormatCondition), new PropertyMetadata(null, (d, e) => ((FormatCondition)d).OnInfoPropertyChanged(e)));
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public object Value2 {
			get { return (object)GetValue(Value2Property); }
			set { SetValue(Value2Property, value); }
		}
		public static readonly DependencyProperty Value2Property = DependencyProperty.Register("Value2", typeof(object), typeof(FormatCondition), new PropertyMetadata(null, (d, e) => ((FormatCondition)d).OnInfoPropertyChanged(e)));
		protected override void SyncProperty(DependencyProperty property) {
			base.SyncProperty(property);
			SyncIfNeeded(property, FormatCondition.ValueRuleProperty, () => ValueInfo.ValueRule = ValueRule);
			SyncIfNeeded(property, FormatCondition.Value1Property, () => ValueInfo.Value1 = Value1);
			SyncIfNeeded(property, FormatCondition.Value2Property, () => ValueInfo.Value2 = Value2);
		}
		protected override FormatConditionBaseInfo CreateInfo() {
			return new PivotFormatConditionInfo();
		}
		FormatConditionInfo ValueInfo { get { return Info as FormatConditionInfo; } }
		protected override BaseEditUnit CreateEmptyEditUnit() {
			return new ConditionEditUnit();
		}
		protected override void UpdateEditUnit(BaseEditUnit unit) {
			base.UpdateEditUnit(unit);
			ConditionEditUnit conditionEditUnit = unit as ConditionEditUnit;
			if(conditionEditUnit != null) {
				conditionEditUnit.ValueRule = ValueRule;
				conditionEditUnit.Value1 = Value1;
				conditionEditUnit.Value2 = Value2;
			}
		}
		internal override IEnumerable<AggregationItemValueStorage> GetSummaries() {
			 yield break;
		}
	}
	public class TopBottomRuleFormatCondition : ExpressionConditionBase {
		internal const TopBottomRule DefaultRule = TopBottomRule.AboveAverage;
		internal const double DefaultThreshold = 10;
		public TopBottomRuleFormatCondition() {
		}
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public double Threshold {
			get { return (double)GetValue(ThresholdProperty); }
			set { SetValue(ThresholdProperty, value); }
		}
		public static readonly DependencyProperty ThresholdProperty =
			DependencyProperty.Register("Threshold", typeof(double), typeof(TopBottomRuleFormatCondition), new PropertyMetadata(DefaultThreshold, (d, e) => ((TopBottomRuleFormatCondition)d).OnInfoPropertyChanged(e)));
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public TopBottomRule Rule {
			get { return (TopBottomRule)GetValue(RuleProperty); }
			set { SetValue(RuleProperty, value); }
		}
		public static readonly DependencyProperty RuleProperty =
			DependencyProperty.Register("Rule", typeof(TopBottomRule), typeof(TopBottomRuleFormatCondition), new PropertyMetadata(DefaultRule, (d, e) => ((TopBottomRuleFormatCondition)d).OnInfoPropertyChanged(e)));
		protected override void SyncProperty(DependencyProperty property) {
			base.SyncProperty(property);
			SyncIfNeeded(property, TopBottomRuleFormatCondition.ThresholdProperty, () => TopInfo.Threshold = Threshold);
			SyncIfNeeded(property, TopBottomRuleFormatCondition.RuleProperty, () => TopInfo.Rule = Rule);
		}
		protected override FormatConditionBaseInfo CreateInfo() {
			return new PivotTopBottomRuleFormatConditionInfo();
		}
		TopBottomRuleFormatConditionInfo TopInfo { get { return Info as TopBottomRuleFormatConditionInfo; } }
		protected override bool CanAttach { get { return base.CanAttach && (!string.IsNullOrEmpty(MeasureName) || Expression != null); } }
		protected override BaseEditUnit CreateEmptyEditUnit() {
			return new TopBottomEditUnit();
		}
		protected override void UpdateEditUnit(BaseEditUnit unit) {
			base.UpdateEditUnit(unit);
			TopBottomEditUnit topBottomEditUnit = unit as TopBottomEditUnit;
			if(topBottomEditUnit != null) {
				topBottomEditUnit.Threshold = Threshold;
				topBottomEditUnit.Rule = Rule;
			}
		}
		internal override IEnumerable<AggregationItemValueStorage> GetSummaries() {
			switch(Rule) {
				case TopBottomRule.AboveAverage:
				case TopBottomRule.BelowAverage:
					yield return AggregationItemValueStorage.Create(Data.SummaryItemTypeEx.Average, 0);
					break;
				case TopBottomRule.BottomItems:
					yield return AggregationItemValueStorage.Create(Data.SummaryItemTypeEx.Bottom, Convert.ToDecimal(Threshold));
					break;
				case TopBottomRule.BottomPercent:
					yield return AggregationItemValueStorage.Create(Data.SummaryItemTypeEx.BottomPercent, Convert.ToDecimal(Threshold));
					break;
				case TopBottomRule.TopItems:
					yield return AggregationItemValueStorage.Create(Data.SummaryItemTypeEx.Top, Convert.ToDecimal(Threshold));
					break;
				case TopBottomRule.TopPercent:
					yield return AggregationItemValueStorage.Create(Data.SummaryItemTypeEx.TopPercent, Convert.ToDecimal(Threshold));
					break;
				default:
					throw new ArgumentException(Rule.ToString());
			}
		}
	}
}

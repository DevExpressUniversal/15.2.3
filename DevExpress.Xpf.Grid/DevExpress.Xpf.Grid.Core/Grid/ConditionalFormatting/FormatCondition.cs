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
using System.Windows.Markup;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using System.Collections.Generic;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.Core.ConditionalFormatting.Printing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Grid {
	public abstract class ExpressionConditionBase : FormatConditionBase {
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public Format Format {
			get { return (Format)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(Format), typeof(ExpressionConditionBase), new PropertyMetadata(null, OnFormatChanged, OnCoerceFreezable));
		[XtraSerializableProperty]
		public bool ApplyToRow {
			get { return (bool)GetValue(ApplyToRowProperty); }
			set { SetValue(ApplyToRowProperty, value); }
		}
		public static readonly DependencyProperty ApplyToRowProperty =
			DependencyProperty.Register("ApplyToRow", typeof(bool), typeof(ExpressionConditionBase), new PropertyMetadata(false, (d, e) => ((ExpressionConditionBase)d).OnChanged(e)));
		public override DependencyProperty FormatPropertyForBinding { get { return FormatProperty; } }
		public override string GetApplyToFieldName() {
			return ApplyToRow ? null : base.GetApplyToFieldName();
		}
		protected override void UpdateEditUnit(BaseEditUnit unit) {
			base.UpdateEditUnit(unit);
			ConditionEditUnit conditionEditUnit = unit as ConditionEditUnit;
			if(conditionEditUnit != null) {
				conditionEditUnit.ApplyToRow = ApplyToRow;
				conditionEditUnit.Format = Format;
			}
		}
	}
	public class FormatCondition : ExpressionConditionBase {
		[XtraSerializableProperty]
		public ConditionRule ValueRule {
			get { return (ConditionRule)GetValue(ValueRuleProperty); }
			set { SetValue(ValueRuleProperty, value); }
		}
		public static readonly DependencyProperty ValueRuleProperty = DependencyProperty.Register("ValueRule", typeof(ConditionRule), typeof(FormatCondition), new PropertyMetadata(ConditionRule.Expression, (d, e) => ((FormatCondition)d).OnInfoPropertyChanged(e)));
		[XtraSerializableProperty]
		public object Value1 {
			get { return (object)GetValue(Value1Property); }
			set { SetValue(Value1Property, value); }
		}
		public static readonly DependencyProperty Value1Property = DependencyProperty.Register("Value1", typeof(object), typeof(FormatCondition), new PropertyMetadata(null, (d, e) => ((FormatCondition)d).OnInfoPropertyChanged(e)));
		[XtraSerializableProperty]
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
			return new FormatConditionInfo();
		}
		FormatConditionInfo ValueInfo { get { return Info as FormatConditionInfo; } }
		internal override FormatConditionRuleBase CreateExportWrapper() {
			var dateOccuringRule = new DateOccurringConditionRuleDetector().DetectRule(Expression);
			if(dateOccuringRule != DateOccurringConditionRule.None)
				return new FormatConditionRuleDateOccuringExportWrapper(Format, dateOccuringRule);
			return new FormatConditionRuleValueExportWrapper(this);
		}
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
	}
	public class TopBottomRuleFormatCondition : ExpressionConditionBase {
		internal const TopBottomRule DefaultRule = TopBottomRule.AboveAverage;
		internal const double DefaultThreshold = 10;
		[XtraSerializableProperty]
		public double Threshold {
			get { return (double)GetValue(ThresholdProperty); }
			set { SetValue(ThresholdProperty, value); }
		}
		public static readonly DependencyProperty ThresholdProperty =
			DependencyProperty.Register("Threshold", typeof(double), typeof(TopBottomRuleFormatCondition), new PropertyMetadata(DefaultThreshold, (d, e) => ((TopBottomRuleFormatCondition)d).OnInfoPropertyChanged(e)));
		[XtraSerializableProperty]
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
			return new TopBottomRuleFormatConditionInfo();
		}
		TopBottomRuleFormatConditionInfo TopInfo { get { return Info as TopBottomRuleFormatConditionInfo; } }
		protected override bool CanAttach { get { return !string.IsNullOrEmpty(FieldName) || Expression != null; } }
		internal override FormatConditionRuleBase CreateExportWrapper() {
			if(Rule == TopBottomRule.AboveAverage || Rule == TopBottomRule.BelowAverage)
				return new FormatConditionRuleAboveBelowAverageExportWrapper(this);
			return new FormatConditionRuleTopBottomExportWrapper(this);
		}
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
	}
}

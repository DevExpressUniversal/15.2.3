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
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public abstract class TopBottomViewModelBase : FormatEditorOwnerViewModel {
		protected TopBottomViewModelBase(IDialogContext context) : base(context) { }
		protected override void AddChanges(ConditionEditUnit unit) {
			base.AddChanges(unit);
			TopBottomEditUnit topBottomUnit = unit as TopBottomEditUnit;
			if(topBottomUnit != null) {
				topBottomUnit.Rule = GetRule();
				topBottomUnit.FieldName = Context.ColumnInfo.FieldName;
			}
		}
		protected override void InitCore(ConditionEditUnit unit) {
			base.InitCore(unit);
			(unit as TopBottomEditUnit).Do(x => InitRule(x.Rule, x.Threshold));
		}
		protected abstract TopBottomRule GetRule();
		protected abstract void InitRule(TopBottomRule rule, double threshold);
		protected override ConditionEditUnit CreateEditUnit() {
			return new TopBottomEditUnit();
		}
	}
	public class TopBottomViewModel : TopBottomViewModelBase {
		public static Func<IDialogContext, TopBottomViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new TopBottomViewModel(x)); } }
		protected TopBottomViewModel(IDialogContext context)
			: base(context) {
			Threshold = 10;
		}
		public virtual TopBottomOperatorType Rule { get; set; }
		public virtual double Threshold { get; set; }
		public virtual bool UsePercent { get; set; }
		protected override void AddChanges(ConditionEditUnit unit) {
			base.AddChanges(unit);
			TopBottomEditUnit topBottomUnit = unit as TopBottomEditUnit;
			if(topBottomUnit != null)
				topBottomUnit.Threshold = Threshold;
		}
		protected override TopBottomRule GetRule() {
			return Rule == TopBottomOperatorType.Top ? GetRule(TopBottomRule.TopItems, TopBottomRule.TopPercent) : GetRule(TopBottomRule.BottomItems, TopBottomRule.BottomPercent);
		}
		TopBottomRule GetRule(TopBottomRule itemsRule, TopBottomRule percentRule) {
			return UsePercent ? percentRule : itemsRule;
		}
		protected override void InitRule(TopBottomRule rule, double threshold) {
			UsePercent = rule == TopBottomRule.BottomPercent || rule == TopBottomRule.TopPercent;
			Rule = rule == TopBottomRule.TopItems || rule == TopBottomRule.TopPercent ? TopBottomOperatorType.Top : TopBottomOperatorType.Bottom;
			Threshold = threshold;
		}
		protected override bool CanInitCore(ConditionEditUnit unit) {
			return (unit as TopBottomEditUnit).If(x => x.IsTopBottomCondition()) != null;
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_TopBottomDescription); } }
	}
	public class AboveBelowViewModel : TopBottomViewModelBase {
		public static Func<IDialogContext, AboveBelowViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new AboveBelowViewModel(x)); } }
		protected AboveBelowViewModel(IDialogContext context) : base(context) { }
		public virtual AboveBelowOperatorType Rule { get; set; }
		protected override TopBottomRule GetRule() {
			return Rule == AboveBelowOperatorType.Above ? TopBottomRule.AboveAverage : TopBottomRule.BelowAverage;
		}
		protected override void InitRule(TopBottomRule rule, double threshold) {
			Rule = rule == TopBottomRule.AboveAverage ? AboveBelowOperatorType.Above : AboveBelowOperatorType.Below;
		}
		protected override bool CanInitCore(ConditionEditUnit unit) {
			return (unit as TopBottomEditUnit).If(x => x.IsAboveBelowCondition()) != null;
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_AboveBelowDescription); } }
	}
}

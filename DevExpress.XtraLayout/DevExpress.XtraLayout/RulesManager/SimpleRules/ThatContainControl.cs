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
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public partial class ThatContainControl : XtraUserControl, IFormatConditionRule, IRuleControl {
		FormatConditionRuleAppearanceBase rule;
		FormatRule formatRuleCore;
		public ThatContainControl() {
			InitializeComponent();
			InitThatContain();
			InitLocalizationText();
		}
		void InitLocalizationText() {
			sliInfo.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleThatContainFormatOnlyCellsWith);
			lbcConjunction.Text = Localizer.Active.GetLocalizedString(StringId.FilterClauseBetweenAnd);
		}
		void InitThatContain() {
			cmbThatContain.Properties.Items.AddRange(new object[] {
				new ThatContainDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleThatContainCellValue),
					Rule = RuleType.CellValue
				},
				new ThatContainDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleThatContainDatesOccurring),
					Rule = RuleType.DatesOccurring
				},
			});
		}
		void InitCellValue() {
			cmbContentRule.Properties.Items.Clear();
			cmbContentRule.Properties.Items.AddRange(new object[] {
				new CellValueDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsNone),
					Value = FormatCondition.None
				},
				new CellValueDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleCellValueBetween),
					Value = FormatCondition.Between
				},
				new CellValueDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleCellValueNotBetween),
					Value = FormatCondition.NotBetween
				},
				new CellValueDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleCellValueEqualTo),
					Value = FormatCondition.Equal
				},
				new CellValueDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleCellValueNotEqualTo),
					Value = FormatCondition.NotEqual
				},
				new CellValueDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleCellValueGreaterThan),
					Value = FormatCondition.Greater
				},
				new CellValueDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleCellValueLessThan),
					Value = FormatCondition.Less
				},
				new CellValueDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleCellValueGreaterThanOrEqualTo),
					Value = FormatCondition.GreaterOrEqual
				},
				new CellValueDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleCellValueLessThanOrEqualTo),
					Value = FormatCondition.LessOrEqual
				}
			});
		}
		void InitDatesOccurring() {
			cmbContentRule.Properties.Items.Clear();
			cmbContentRule.Properties.Items.AddRange(new object[] {
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringYesterday),
					Dates = FilterDateType.Yesterday
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringToday),
					Dates = FilterDateType.Today
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringTomorrow),
					Dates = FilterDateType.Tomorrow
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringLastWeek),
					Dates = FilterDateType.LastWeek
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringThisWeek),
					Dates = FilterDateType.ThisWeek
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringNextWeek),
					Dates = FilterDateType.NextWeek
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringMonthAgo1),
					Dates = FilterDateType.MonthAgo1
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringThisMonth),
					Dates = FilterDateType.ThisMonth
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringMonthAfter1),
					Dates = FilterDateType.MonthAfter1
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringEarlierThisWeek),
					Dates = FilterDateType.EarlierThisWeek
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringEarlierThisMonth),
					Dates = FilterDateType.EarlierThisMonth
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringEarlierThisYear),
					Dates = FilterDateType.EarlierThisYear
				},
				 new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringMonthAgo2),
					Dates = FilterDateType.MonthAgo2
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringMonthAgo3),
					Dates = FilterDateType.MonthAgo3
				},								  
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringMonthAgo4),
					Dates = FilterDateType.MonthAgo4
				},								  
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringMonthAgo5),
					Dates = FilterDateType.MonthAgo5
				},								  
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringMonthAgo6),
					Dates = FilterDateType.MonthAgo6
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringEarlier),
					Dates = FilterDateType.Earlier
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringPriorThisYear),
					Dates = FilterDateType.PriorThisYear
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringLaterThisWeek),
					Dates = FilterDateType.LaterThisWeek
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringLaterThisMonth),
					Dates = FilterDateType.LaterThisMonth
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringLaterThisYear),
					Dates = FilterDateType.LaterThisYear
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringMonthAfter2),
					Dates = FilterDateType.MonthAfter2
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringBeyond),
					Dates = FilterDateType.Beyond
				},
				new DatesOccurringDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDatesOccurringBeyondThisYear),
					Dates = FilterDateType.BeyondThisYear
				}
			});
		}
		void InitSpecificText() {
			cmbContentRule.Properties.Items.Clear();
			cmbContentRule.Properties.Items.AddRange(new object[] {
				new SpecificTextDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleSpecificTextContaining)
				},
				new SpecificTextDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleSpecificTextNotContaining)
				},
				new SpecificTextDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleSpecificTextBeginningWith)
				},
				new SpecificTextDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleSpecificTextEndingWith)
				}
			});
		}
		void cmbThatContain_SelectedIndexChanged(object sender, EventArgs e) {
			SetControls(((ThatContainDisplayObject)cmbThatContain.SelectedItem).Rule);
		}
		public void SetControls(RuleType ruleType) {
			switch(ruleType) {
				case RuleType.CellValue:
					cmbThatContain.SelectedIndex = 0;
					InitCellValue();
					SetVisibility(true, true, true);
					if(!(rule is FormatConditionRuleValue)) rule = new FormatConditionRuleValue();
					break;
				case RuleType.DatesOccurring:
					cmbThatContain.SelectedIndex = 1;
					InitDatesOccurring();
					SetVisibility(false, false, true);
					if(!(rule is FormatConditionRuleDateOccuring)) rule = new FormatConditionRuleDateOccuring();
					break;
			}
			cmbContentRule.SelectedIndex = 0;
			SetFormFormat();
		}
		void SetVisibility(bool p1, bool p2, bool p3) {
			lcgFullCellValue.Visibility = p1 ? LayoutVisibility.Always : LayoutVisibility.Never;
			lcgSpecificText.Visibility = p2 ? LayoutVisibility.Always : LayoutVisibility.Never;
			lciContentRule.Visibility = p3 ? LayoutVisibility.Always : LayoutVisibility.Never;
		}
		#region IFormatConditionRule
		public FormatRule GetFormatRule() {
			UpdateFormatRule();
			return formatRuleCore;
		}
		void UpdateFormatRule() {
			var ruleType = ((ThatContainDisplayObject)cmbThatContain.SelectedItem).Rule;
			GetFormFormat(ruleType);
			UpdateFormatRule(ruleType);
		}
		void UpdateFormatRule(RuleType ruleType) {
			FormatConditionRuleBase rb;
			if(ruleType == RuleType.DatesOccurring) rb = new FormatConditionRuleDateOccuring();
			else rb = new FormatConditionRuleValue();
			rb.Assign(rule);
			formatRuleCore.RuleBase = rb;
			formatRuleCore.RuleType = ruleType;
			formatRuleCore.RuleName = (ruleType == RuleType.DatesOccurring) ? ((FormatConditionRuleDateOccuring)rule).DateType.ToString() :
																				Localizer.Active.GetLocalizedString(StringId.ManageRuleThatContainCellValue);
		}
		public void SetFormatRule(FormatRule formatRule) {
			formatRuleCore = formatRule;
			var rule = formatRuleCore.RuleBase;
			if(rule == null) this.rule = new FormatConditionRuleValue(); 
			if(rule is FormatConditionRuleValue) {
				if(this.rule == null) this.rule = new FormatConditionRuleValue();
				this.rule.Assign(rule);
			}
			if(rule is FormatConditionRuleDateOccuring) {
				if(this.rule == null) this.rule = new FormatConditionRuleDateOccuring();
				this.rule.Assign(rule);
			}
			SetFormFormat();
			if(Parent != null) ((SimpleRuleBase)Parent).SetFormatRule(formatRuleCore);
		}
		#endregion
		void GetFormFormat(RuleType ruleType) {
			if(ruleType == RuleType.CellValue) {
				rule = new FormatConditionRuleValue() {
					Condition = ((CellValueDisplayObject)cmbContentRule.SelectedItem).Value,
					Value1 = tedBeginValue.Text,
					Value2 = tedEndValue.Text
				};
			} else if(ruleType == RuleType.DatesOccurring) {
				rule = new FormatConditionRuleDateOccuring() {
					DateType = ((DatesOccurringDisplayObject)cmbContentRule.SelectedItem).Dates
				};
			}
		}
		public void GetFormFormat() {
		}
		public void SetFormFormat() {
			if(rule is FormatConditionRuleValue) {
				var ruleValue = rule as FormatConditionRuleValue;
				cmbThatContain.SelectedIndex = 0;
				cmbContentRule.SelectedIndex = GetContentRule(ruleValue);
				tedBeginValue.Text = GetBeginValue(ruleValue);
				tedEndValue.Text = GetEndValue(ruleValue);
			} else if(rule is FormatConditionRuleDateOccuring) {
				var ruleDateOccurring = rule as FormatConditionRuleDateOccuring;
				cmbContentRule.SelectedIndex = 1;
				cmbContentRule.SelectedIndex = GetContentRule(ruleDateOccurring);
			}
			if(formatRuleCore.RuleBase == null) formatRuleCore.RuleBase = rule;
		}
		int GetContentRule(FormatConditionRuleDateOccuring ruleDateOccurring) {
			for(int i = 0; i < cmbContentRule.Properties.Items.Count; i++) {
				var item = (DatesOccurringDisplayObject)cmbContentRule.Properties.Items[i];
				if(item.Dates == ruleDateOccurring.DateType) return i;
			}
			return -1;
		}
		string GetBeginValue(FormatConditionRuleValue ruleValue) {
			if(ruleValue.Value1 == null) return string.Empty;
			return ruleValue.Value1.ToString();
		}
		string GetEndValue(FormatConditionRuleValue ruleValue) {
			if(ruleValue.Value2 == null) return string.Empty;
			return ruleValue.Value2.ToString();
		}
		int GetContentRule(FormatConditionRuleValue ruleValue) {
			for(int i = 0; i < cmbContentRule.Properties.Items.Count; i++) {
				var item = (CellValueDisplayObject)cmbContentRule.Properties.Items[i];
				if(item.Value == ruleValue.Condition) return i;
			}
			return -1;
		}
		void cmbContentRule_SelectedIndexChanged(object sender, EventArgs e) {
			if(cmbContentRule.SelectedItem is CellValueDisplayObject) {
				if(((CellValueDisplayObject)cmbContentRule.SelectedItem).Value == FormatCondition.Between ||
				   ((CellValueDisplayObject)cmbContentRule.SelectedItem).Value == FormatCondition.NotBetween) {
					SetVisibility(true, true, true);
				} else {
					SetVisibility(false, true, true);
				}
				if(((CellValueDisplayObject)cmbContentRule.SelectedItem).Value == FormatCondition.None) SetVisibility(false, false, true);
			}
		}
	}
	public class ThatContainDisplayObject {
		public override string ToString() { return Description; }
		public string Description { get; set; }
		public RuleType Rule { get; set; }
	}
	public class CellValueDisplayObject {
		public override string ToString() { return Description; }
		public string Description { get; set; }
		public FormatCondition Value { get; set; }
	}
	public class DatesOccurringDisplayObject {
		public override string ToString() { return Description; }
		public string Description { get; set; }
		public FilterDateType Dates { get; set; }
	}
	public class SpecificTextDisplayObject {
		public override string ToString() { return Description; }
		public string Description { get; set; }
	}
}

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

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public partial class RankedValuesControl : XtraUserControl, IFormatConditionRule, IRuleControl {
		FormatConditionRuleTopBottom ranked;
		FormatRule formatRuleCore;
		public RankedValuesControl() {
			InitializeComponent();
			InitializeTopBottom();
			CreateRule();
			InitLocalizationText();
		}
		void InitLocalizationText() {
			sliInfo.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleRankedValuesFormatValuesThatRankInThe);
			chePercent.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleRankedValuesOfTheColumnsCellValues);
		}
		void InitializeTopBottom() {
			cmbRankedValues.Properties.Items.AddRange(new object[] {
				new TopBottomTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleRankedValuesTop),
					Ranked = FormatConditionTopBottomType.Top
				},
				new TopBottomTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleRankedValuesBottom),
					Ranked = FormatConditionTopBottomType.Bottom
				}
			});
		}
		void CreateRule() {
			ranked = new FormatConditionRuleTopBottom();
		}
		#region GetForm
		public void GetFormFormat() {
			ranked.TopBottom = ((TopBottomTypeDisplayObject)cmbRankedValues.SelectedItem).Ranked;
			ranked.RankType = (chePercent.Checked) ? FormatConditionValueType.Percent : FormatConditionValueType.Number;
			decimal rank;
			if(decimal.TryParse(tedCount.Text, out rank)) { ranked.Rank = rank; }
		}
		#endregion
		#region SetForm
		public void SetFormFormat() {
			cmbRankedValues.SelectedIndex = ranked.TopBottom == FormatConditionTopBottomType.Top ? 0 : 1;
			chePercent.Checked = ranked.RankType == FormatConditionValueType.Percent;
			tedCount.Text = ranked.Rank == 0m ? "10" : ranked.Rank.ToString();
			if(formatRuleCore.RuleBase == null) formatRuleCore.RuleBase = ranked;
			if(Parent != null) ((SimpleRuleBase)Parent).SetFormatRule(formatRuleCore);
		}
		#endregion
		#region IFormatConditionRule
		public FormatRule GetFormatRule() {
			UpdateFormatRule();
			return formatRuleCore;
		}
		void UpdateFormatRule() {
			GetFormFormat();
			FormatConditionRuleTopBottom tb = new FormatConditionRuleTopBottom();
			tb.Assign(ranked);
			string percent = (ranked.RankType == FormatConditionValueType.Percent) ? " %" : string.Empty;
			formatRuleCore.RuleBase = tb;
			formatRuleCore.RuleType = RuleType.RankedValues;
			formatRuleCore.RuleName = string.Format("{0} {1}{2}", (ranked.TopBottom == FormatConditionTopBottomType.Top) ?
									  Localizer.Active.GetLocalizedString(StringId.ManageRuleRankedValuesTop) :
									  Localizer.Active.GetLocalizedString(StringId.ManageRuleRankedValuesBottom), ranked.Rank, percent);
		}
		public void SetFormatRule(FormatRule formatRule) {
			formatRuleCore = formatRule;
			var rule = formatRuleCore.RuleBase as FormatConditionRuleTopBottom;
			if(rule == null) {
				ranked = new FormatConditionRuleTopBottom();
				formatRuleCore.RuleBase = ranked;
				SetFormFormat();
				return;
			}
			ranked.Assign(rule);
			SetFormFormat();
		}
		#endregion
	}
	public class TopBottomTypeDisplayObject {
		public override string ToString() { return Description; }
		public string Description { get; set; }
		public FormatConditionTopBottomType Ranked { get; set; }
	}
}

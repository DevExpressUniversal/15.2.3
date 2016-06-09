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
namespace DevExpress.XtraEditors.Frames{
	[ToolboxItem(false)]
	public partial class AverageControl : XtraUserControl, IFormatConditionRule, IRuleControl {
		FormatConditionRuleAboveBelowAverage average;
		FormatRule formatRuleCore;
		public AverageControl() {
			InitializeComponent();
			InitializeAverage();
			CreateRule();
			InitLocalizationText();
		}
		void InitLocalizationText() {
			sliInfo.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleAverageFormatValuesThatAre);
			lciAverage.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleAverageTheAverageForTheSelectedRange);
		}
		void InitializeAverage() {
			cmbAverage.Properties.Items.AddRange(new object[] {
				new AboveBelowTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleAverageAbove),
					Average = FormatConditionAboveBelowType.Above
				},
				new AboveBelowTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleAverageBelow),
					Average = FormatConditionAboveBelowType.Below
				},
				new AboveBelowTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleAverageEqualOrAbove),
					Average = FormatConditionAboveBelowType.EqualOrAbove
				},
				new AboveBelowTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleAverageEqualOrBelow),
					Average = FormatConditionAboveBelowType.EqualOrBelow
				}
			});
		}
		void CreateRule() {
			average = new FormatConditionRuleAboveBelowAverage();
		}
		#region GetForm
		public void GetFormFormat() {
			average.AverageType = ((AboveBelowTypeDisplayObject)cmbAverage.SelectedItem).Average;
		}
		string GetAverageType(FormatConditionRuleAboveBelowAverage average) {
			string averageType = string.Empty;
			switch(average.AverageType) {
				case FormatConditionAboveBelowType.Above:
					averageType = Localizer.Active.GetLocalizedString(StringId.ManageRuleAboveAverage); break;
				case FormatConditionAboveBelowType.Below:
					averageType = Localizer.Active.GetLocalizedString(StringId.ManageRuleBelowAverage); break;
				case FormatConditionAboveBelowType.EqualOrAbove:
					averageType = Localizer.Active.GetLocalizedString(StringId.ManageRuleEqualOrAboveAverage); break;
				case FormatConditionAboveBelowType.EqualOrBelow:
					averageType = Localizer.Active.GetLocalizedString(StringId.ManageRuleEqualOrBelowAverage); break;
			}
			return averageType;
		}
		#endregion
		#region SetForm
		public void SetFormFormat() {
			cmbAverage.SelectedIndex = SetFormAverageType();
			if(formatRuleCore.RuleBase == null) formatRuleCore.RuleBase = average;
			if(Parent != null) ((SimpleRuleBase)Parent).SetFormatRule(formatRuleCore);
		}
		int SetFormAverageType() {
			int indexAverageType = 0;
			switch(average.AverageType) {
				case FormatConditionAboveBelowType.Above: indexAverageType = 0; break;
				case FormatConditionAboveBelowType.Below: indexAverageType = 1; break;
				case FormatConditionAboveBelowType.EqualOrAbove: indexAverageType = 2; break;
				case FormatConditionAboveBelowType.EqualOrBelow: indexAverageType = 3; break;
			}
			return indexAverageType;
		}
		#endregion
		#region IFormatConditionRule
		public FormatRule GetFormatRule() {
			UpdateFormatRule();
			return formatRuleCore;
		}
		void UpdateFormatRule() {
			GetFormFormat();
			FormatConditionRuleAboveBelowAverage aba = new FormatConditionRuleAboveBelowAverage();
			aba.Assign(average);
			formatRuleCore.RuleBase = aba;
			formatRuleCore.RuleType = RuleType.Average;
			formatRuleCore.RuleName = GetAverageType(aba);
		}
		public void SetFormatRule(FormatRule formatRule) {
			formatRuleCore = formatRule;
			var rule = formatRuleCore.RuleBase as FormatConditionRuleAboveBelowAverage;
			if(rule == null) {
				average = new FormatConditionRuleAboveBelowAverage();
				SetFormFormat();
				return;
			}
			average.Assign(rule);
			SetFormFormat();
		}
		#endregion
	}
	public class AboveBelowTypeDisplayObject {
		public override string ToString() { return Description; }
		public string Description { get; set; }
		public FormatConditionAboveBelowType Average { get; set; }
	}
}

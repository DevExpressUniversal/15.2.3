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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public partial class UniqueOrDuplicateControl : XtraUserControl, IFormatConditionRule, IRuleControl {
		FormatConditionRuleUniqueDuplicate uniqueDuplicate;
		FormatRule formatRuleCore;
		public UniqueOrDuplicateControl() {
			InitializeComponent();											 
			InitUniqueDuplicateType();
			CreateRule();
			InitLocalizationText();
		}
		void InitLocalizationText() {
			sliInfo.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleUniqueOrDuplicateFormatAll);
			lciUniqueOrDuplicate.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleUniqueOrDuplicateValuesInTheSelectedRange);
		}
		void CreateRule() {
			uniqueDuplicate = new FormatConditionRuleUniqueDuplicate();
		}
		void InitUniqueDuplicateType() {
			cmbUniqueOrDuplicate.Properties.Items.AddRange(new object[] {
				new UniqueDuplicateTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleUniqueOrDuplicateDuplicate),
					UniqueDuplicate = FormatConditionUniqueDuplicateType.Duplicate
				},
				new UniqueDuplicateTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleUniqueOrDuplicateUnique),
					UniqueDuplicate = FormatConditionUniqueDuplicateType.Unique
				}
			});
		}
		#region GetForm
		public void GetFormFormat() {
			uniqueDuplicate.FormatType = ((UniqueDuplicateTypeDisplayObject)cmbUniqueOrDuplicate.SelectedItem).UniqueDuplicate;
		}
		#endregion
		#region SetForm
		public void SetFormFormat() {
			cmbUniqueOrDuplicate.SelectedIndex = uniqueDuplicate.FormatType == FormatConditionUniqueDuplicateType.Unique ? 1 : 0;
			if(formatRuleCore.RuleBase == null) formatRuleCore.RuleBase = uniqueDuplicate;
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
			FormatConditionRuleUniqueDuplicate ud = new FormatConditionRuleUniqueDuplicate();
			ud.Assign(uniqueDuplicate);
			formatRuleCore.RuleBase = ud;
			formatRuleCore.RuleType = RuleType.UniqueOrDuplicate;
			formatRuleCore.RuleName = ud.FormatType == FormatConditionUniqueDuplicateType.Unique ?
									  Localizer.Active.GetLocalizedString(StringId.ManageRuleUniqueOrDuplicateUnique) :
									  Localizer.Active.GetLocalizedString(StringId.ManageRuleUniqueOrDuplicateDuplicate);
		}
		public void SetFormatRule(FormatRule formatRule) {
			formatRuleCore = formatRule;
			var rule = formatRuleCore.RuleBase as FormatConditionRuleUniqueDuplicate;
			if(rule == null) {
				uniqueDuplicate = new FormatConditionRuleUniqueDuplicate();
				SetFormFormat();
				return;
			}
			uniqueDuplicate.Assign(rule);
			SetFormFormat();
		}
		#endregion
	}
	public class UniqueDuplicateTypeDisplayObject {
		public override string ToString() { return Description; }
		public string Description { get; set; }
		public FormatConditionUniqueDuplicateType UniqueDuplicate { get; set; }
	}
}

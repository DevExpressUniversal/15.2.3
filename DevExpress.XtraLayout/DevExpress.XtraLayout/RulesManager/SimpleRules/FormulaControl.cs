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

using DevExpress.Data.Filtering;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraLayout;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public partial class FormulaControl : XtraUserControl, IFormatConditionRule, IRuleControl {
		FormatConditionRuleExpression formula;
		FormatRule formatRuleCore;
		public FormulaControl(FilterColumnCollection filterColumns, FilterColumn filterColumnDefault) {
			InitializeComponent();
			CreateRule();
			CreateFilterControl(filterColumns, filterColumnDefault);
			InitLocalizationText();
		}
		#region InitializeComponent
		public IDXMenuManager MenuManager { get; set; }
		IFilterControl fc;
		Control filterControl;
		LayoutControlItem lciFormula;
		void CreateFilterControl(FilterColumnCollection filterColumns, FilterColumn filterColumnDefault) {
			fc = FormatRuleMenuHelper.CreateFilterControl(FormatRuleMenuOptions.FilterEditorViewMode);
			FilterColumnCollection fCollection = filterColumns;
			fc.SetFilterColumnsCollection(fCollection, MenuManager);
			fc.SetDefaultColumn(filterColumnDefault);
			fc.ShowOperandTypeIcon = true;
			fc.UseMenuForOperandsAndOperators = true;
			fc.FilterCriteria = null;
			filterControl = fc as Control;
			filterControl.Name = "filterControl";
			filterControl.Size = new Size(458, 249);
			filterControl.TabIndex = 4;
			filterControl.Text = "filterControl1";
			lcFormula.Controls.Add(filterControl);
			lciFormula = new LayoutControlItem() {
				Control = filterControl,
				CustomizationFormText = "Format values where this formaula is true:",
				MaxSize = new Size(0, 265),
				MinSize = new Size(203, 265),
				Name = "lciFormula",
				Size = new Size(458, 265),
				SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom,
				Text = "Format values where this formaula is true:",
				TextLocation = DevExpress.Utils.Locations.Top,
			};
			lcgFormula.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lciFormula });
		}
		#endregion
		void InitLocalizationText() {				
			lciFormula.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormulaFormatValuesWhereThisFormulaIsTrue);
		}
		void CreateRule() {
			formula = new FormatConditionRuleExpression();
		}
		#region GetForm
		public void GetFormFormat() {
			formula.Expression = fc.FilterCriteria.LegacyToString();
		}
		#endregion
		#region SetForm
		public void SetFormFormat() {
			fc.FilterCriteria = CriteriaOperator.Parse(formula.Expression);
			if(formatRuleCore.RuleBase == null) formatRuleCore.RuleBase = formula;
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
			FormatConditionRuleExpression exp = new FormatConditionRuleExpression();
			exp.Assign(formula);
			formatRuleCore.RuleBase = exp;
			formatRuleCore.RuleType = RuleType.Formula;
			formatRuleCore.RuleName = string.Format("{0}: {1}", Localizer.Active.GetLocalizedString(StringId.ManageRuleFormula), exp.Expression);
		}
		public void SetFormatRule(FormatRule formatRule) {
			formatRuleCore = formatRule;
			var rule = formatRuleCore.RuleBase as FormatConditionRuleExpression;
			if(rule == null) {
				formula = new FormatConditionRuleExpression();
				SetFormFormat();
				return;
			}
			formula.Assign(rule);
			SetFormFormat();
		}
		#endregion
	}
}

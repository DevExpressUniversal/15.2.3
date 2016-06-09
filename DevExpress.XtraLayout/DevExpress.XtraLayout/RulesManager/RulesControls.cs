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

using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Filtering;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraEditors.Frames {
	public interface IControlRuleBase {
		PanelControl PnlFormatSetting { get; }
	}
	public enum RuleType : int { 
		ColorScale2 = 0, ColorScale3 = 1, DataBar = 2, IconSet = 3,
		ThatContain, CellValue, SpecificText, DatesOccurring,
		RankedValues, Average, UniqueOrDuplicate, Formula	
	}
	public class RulesControls : IDisposable {
		ComplexRuleBase complex;
		SimpleRuleBase simple;
		ThatContainControl thatContain;
		Dictionary<RuleType, IFormatConditionRule> ruleControls;
		IDXMenuManager menuManager;
		FilterColumnCollection filterColumns;
		FilterColumn filterColumnDefault;
		public RulesControls(FilterColumnCollection filterColumns, FilterColumn filterColumnDefault, IDXMenuManager menuManager) {
			this.menuManager = menuManager;
			this.filterColumns = filterColumns;
			this.filterColumnDefault = filterColumnDefault;
		}
		public void CreateRules() {
			thatContain = new ThatContainControl(); 
			complex = new ComplexRuleBase();
			simple = new SimpleRuleBase();
			InitRuleControls();
		}
		void InitRuleControls() {
			ruleControls = new Dictionary<RuleType, IFormatConditionRule>();
			ruleControls.Add(RuleType.ColorScale2, new ColorScale2Control());
			ruleControls.Add(RuleType.ColorScale3, new ColorScale3Control());
			ruleControls.Add(RuleType.DataBar, new DataBarControl());
			ruleControls.Add(RuleType.IconSet, new IconSetControl());
			ruleControls.Add(RuleType.CellValue, thatContain);
			ruleControls.Add(RuleType.DatesOccurring, thatContain);
			ruleControls.Add(RuleType.RankedValues, new RankedValuesControl());
			ruleControls.Add(RuleType.Average, new AverageControl());
			ruleControls.Add(RuleType.UniqueOrDuplicate, new UniqueOrDuplicateControl());
			ruleControls.Add(RuleType.Formula, new FormulaControl(filterColumns, filterColumnDefault) { MenuManager = menuManager });
		}
		public XtraUserControl GetRuleControlBaseT(FormatRule formatRule) {
			if(IsComplex(formatRule.RuleType)) {
				complex.cmbComplexRule.SelectedIndex = GetIndexComplexRule(formatRule.RuleType);
				return complex;
			}
			return simple;
		}
		public XtraUserControl GetRuleControlT(FormatRule formatRule, XtraUserControl baseControl) {
			EnsureRuleControls();
			IFormatConditionRule ruleControl = null;
			if(ruleControls.TryGetValue(formatRule.RuleType, out ruleControl)) {
				((XtraUserControl)ruleControl).Parent = baseControl;
				ruleControl.SetFormatRule(formatRule);
				if(IsThatContain(formatRule.RuleType)) ((ThatContainControl)ruleControl).SetControls(formatRule.RuleType);
			}
			return ruleControl as XtraUserControl;
		}
		public XtraUserControl GetRuleControlWithPanelT(FormatRule formatRule) {
			EnsureRuleControls();
			IControlRuleBase baseControl = GetRuleControlBaseT(formatRule) as IControlRuleBase;
			baseControl.PnlFormatSetting.Controls.Clear();
			var control = GetRuleControlT(formatRule, (XtraUserControl)baseControl);
			control.Dock = System.Windows.Forms.DockStyle.Fill;
			baseControl.PnlFormatSetting.Controls.Add(control);
			return baseControl as XtraUserControl;
		}
		bool IsThatContain(RuleType ruleType) {
			return ruleType == RuleType.CellValue || ruleType == RuleType.SpecificText || ruleType == RuleType.DatesOccurring;
		}
		bool IsComplex(RuleType ruleType) {
			return ruleType == RuleType.ColorScale2 || ruleType == RuleType.ColorScale3 || ruleType == RuleType.DataBar || ruleType == RuleType.IconSet;
		}
		int GetIndexComplexRule(RuleType ruleType) {
			return (int)ruleType;
		}
		void EnsureRuleControls() {
			if(ruleControls == null) CreateRules();
		}
		public void Dispose() {
			if(complex != null) { complex.Dispose(); complex = null; }
			if(simple != null) { simple.Dispose(); simple = null; }
			if(thatContain != null) { thatContain.Dispose(); thatContain = null; }
		}
	}
}

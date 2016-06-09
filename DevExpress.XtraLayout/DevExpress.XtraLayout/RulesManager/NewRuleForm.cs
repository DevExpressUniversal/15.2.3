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

using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Filtering;
using System;
namespace DevExpress.XtraEditors.Frames {
	public interface IFormatConditionRule {
		FormatRule GetFormatRule();
		void SetFormatRule(FormatRule formatRule);
	}
	public interface IRuleControl {
		void GetFormFormat();
		void SetFormFormat();
	}													  
	public partial class NewRuleForm : XtraForm {
		RulesControls ruleControls;			 
		FormatRule formatRuleCore;
		public NewRuleForm(FilterColumnCollection filterColumns, FilterColumn filterColumnDefault, DevExpress.Utils.Menu.IDXMenuManager menuManager) {
			InitializeComponent();
			InitializeMenu();
			CreateRulesControls(filterColumns, filterColumnDefault, menuManager);
			InitLocalizationText();
			SetSizeTouchMode();
		}
		public FormatRule FormatRule {
			get { return formatRuleCore; }
			set { formatRuleCore = value; }
		}
		void SetSizeTouchMode() {
			if(WindowsFormsSettings.TouchUIMode != DevExpress.LookAndFeel.TouchUIMode.True) return;
			lciSelectRuleType.SizeConstraintsType = XtraLayout.SizeConstraintsType.Custom;
			Size = new System.Drawing.Size(Size.Width + 50, Size.Height);
			lciSelectRuleType.MaxSize = lciSelectRuleType.MinSize = new System.Drawing.Size(505, 239);
		}
		void CreateRulesControls(FilterColumnCollection filterColumns, FilterColumn filterColumnDefault, DevExpress.Utils.Menu.IDXMenuManager menuManager) {
			ruleControls = new RulesControls(filterColumns, filterColumnDefault, menuManager);
			formatRuleCore = new FormatRule();
		}
		void InitLocalizationText() {
			grpRuleType.Text = Localizer.Active.GetLocalizedString(StringId.NewEditFormattingRuleSelectARuleType);
			grpRuleDescription.Text = Localizer.Active.GetLocalizedString(StringId.NewEditFormattingRuleEditTheRuleDescription);
			btnOK.Text = Localizer.Active.GetLocalizedString(StringId.OK);
			btnCancel.Text = Localizer.Active.GetLocalizedString(StringId.Cancel);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				ruleControls.Dispose();
			}
			base.Dispose(disposing);
		}
		void InitializeMenu() {
			lbxRuleType.Items.AddRange(new object[] {
				new RuleTypeDisplayObject(){
					Description = Localizer.Active.GetLocalizedString(StringId.NewEditFormattingRuleFormatAllCellsBasedOnTheirValues),
					RuleType = RuleType.ColorScale2
				},
				new RuleTypeDisplayObject(){
					Description = Localizer.Active.GetLocalizedString(StringId.NewEditFormattingRuleFormatOnlyCellsThatContain),
					RuleType = RuleType.CellValue
				},
				new RuleTypeDisplayObject(){
					Description = Localizer.Active.GetLocalizedString(StringId.NewEditFormattingRuleFormatOnlyTopOrBottomRankedValues),
					RuleType = RuleType.RankedValues
				},
				new RuleTypeDisplayObject(){
					Description = Localizer.Active.GetLocalizedString(StringId.NewEditFormattingRuleFormatOnlyValuesThatAreAboveOrBelowAverage),
					RuleType = RuleType.Average
				},
				new RuleTypeDisplayObject(){
					Description = Localizer.Active.GetLocalizedString(StringId.NewEditFormattingRuleFormatOnlyUniqueOrDuplicateValues),
					RuleType = RuleType.UniqueOrDuplicate
				},
				new RuleTypeDisplayObject(){
					Description = Localizer.Active.GetLocalizedString(StringId.NewEditFormattingRuleUseAFormulaToDetermineWhichCellsToFormat),
					RuleType = RuleType.Formula
				}
			});
		}
		public void LoadControl(FormatRule formatRule) {
			formatRuleCore = new Frames.FormatRule();
			formatRuleCore.ColumnFieldName = formatRule.ColumnFieldName;
			formatRuleCore.RuleName = formatRule.RuleName;
			formatRuleCore.RuleType = formatRule.RuleType;
			if(formatRule.RuleBase != null) formatRuleCore.RuleBase = formatRule.RuleBase.CreateInstance();
			if(formatRuleCore.RuleBase != null || formatRule.RuleBase != null) formatRuleCore.RuleBase.Assign(formatRule.RuleBase);
			SelectedLoadControl();
			SetControls();
		}
		void lbxRuleType_SelectedIndexChanged(object sender, EventArgs e) {
			if(init) return;
			formatRuleCore.RuleType = ((RuleTypeDisplayObject)lbxRuleType.SelectedItem).RuleType;
			SetControls();
			lbxRuleType.Refresh();
		}
		public void SetControls() {
			grpRuleDescription.Controls.Clear();
			XtraUserControl control = ruleControls.GetRuleControlWithPanelT(formatRuleCore);
			control.Dock = System.Windows.Forms.DockStyle.Fill;
			grpRuleDescription.Controls.Add(control);
			CheckFormSize();
		}
		public IFormatRule GetFormatRule() {
			IControlRuleBase crb = grpRuleDescription.Controls[0] as IControlRuleBase;
			if(crb is SimpleRuleBase) {
				return ((IFormatConditionRule)crb).GetFormatRule();
			} else {
				return ((IFormatConditionRule)crb.PnlFormatSetting.Controls[0]).GetFormatRule();
			}
		}
		internal void CheckFormSize() {
			System.Drawing.Size newSize = ClientSize;
			newSize.Height = layoutControl2.Root.MinSize.Height;
			ClientSize = newSize;
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			CheckFormSize();
		}
		bool init;
		void SelectedLoadControl() {
			init = true;
			switch(formatRuleCore.RuleType) {
				case RuleType.ColorScale2:
				case RuleType.ColorScale3:
				case RuleType.DataBar:
				case RuleType.IconSet:
					lbxRuleType.SelectedIndex = 0;
					break;
				case RuleType.ThatContain:
				case RuleType.CellValue:
				case RuleType.SpecificText:
				case RuleType.DatesOccurring:
					lbxRuleType.SelectedIndex = 1;
					break;
				case RuleType.RankedValues:
					lbxRuleType.SelectedIndex = 2;
					break;
				case RuleType.Average:
					lbxRuleType.SelectedIndex = 3;
					break;
				case RuleType.UniqueOrDuplicate:
					lbxRuleType.SelectedIndex = 4;
					break;
				case RuleType.Formula:
					lbxRuleType.SelectedIndex = 5;
					break;
			}
			init = false;
		}
	}
	public class RuleTypeDisplayObject {
		public override string ToString() { return Description; }
		public string Description { get; set; }
		public RuleType RuleType { get; set; }
	}
}

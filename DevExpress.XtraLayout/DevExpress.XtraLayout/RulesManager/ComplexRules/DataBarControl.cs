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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraLayout.Utils;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public partial class DataBarControl : XtraUserControl, IFormatConditionRule, IRuleControl {
		FormatConditionRuleDataBar dataBar;
		FormatRule formatRuleCore;
		bool init;
		public DataBarControl() {
			InitializeComponent();
			CreateRule();
			SetFormFormat();
			InitLocalizationText();
		}
		void InitLocalizationText() {
			sliMinimum.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonMinimum);
			sliMaximum.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonMaximum);
			lciTypeMin.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonType);
			lciTypeMax.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonType);
			lciValueMin.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonValue);
			lciValueMax.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonValue);
			cmbTypeMin.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonPercent),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonNumber)
			});
			cmbTypeMax.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonPercent),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonNumber)
			});
			lciPreview.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonPreview);
			sliBarAppearance.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarBarAppearance);
			lciPositiveFill.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarFill);
			lciNegativeFill.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarFill);
			cmbPositiveFill.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarSolidFill),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarGradientFill)
			});
			cmbNegativeFill.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarSolidFill),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarGradientFill)
			});
			lciPositiveColor.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonColor);
			lciNegativeColor.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonColor);
			lciPositiveBorder.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarBorder);
			lciNegativeBorder.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarBorder);
			cmbPositiveBorder.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarNoBorder),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarSolidBorder)
			});
			cmbNegativeBorder.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarNoBorder),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarSolidBorder)
			});
			lciPositiveBorderColor.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonColor);
			lciNegativeBorderColor.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonColor);
			cheDrawAxis.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarDrawAxis);
			cheUseNegativeBar.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarUseNegativeBar);
			lciAxisColor.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarAxisColor);
			lciBarDirection.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarBarDirection);
			cmbBarDirection.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarContext),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarLTR),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBarRTL)
			});
		}
		void CreateRule() {
			dataBar = new FormatConditionRuleDataBar();
		}
		#region IFormatConditionRule
		public FormatRule GetFormatRule() {
			UpdateFormatRule();
			return formatRuleCore;
		}
		void UpdateFormatRule() {
			GetFormFormat();
			FormatConditionRuleDataBar db = new FormatConditionRuleDataBar();
			db.Assign(dataBar);
			formatRuleCore.RuleBase = db;
			formatRuleCore.RuleType = RuleType.DataBar;
			formatRuleCore.RuleName = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBar);
		}
		public void SetFormatRule(FormatRule formatRule) {
			formatRuleCore = formatRule;
			var rule = formatRuleCore.RuleBase as FormatConditionRuleDataBar;
			if(rule == null) {
				dataBar = new FormatConditionRuleDataBar();
				SetFormFormat();
				return;
			}
			dataBar.Assign(rule);
			SetFormFormat();
		}
		#endregion
		#region events
		void pctPreview_Paint(object sender, PaintEventArgs e) {
			((IFormatConditionDrawPreview)dataBar).Draw(new FormatConditionDrawPreviewArgs(e.Graphics, new Rectangle(Point.Empty, pctPreview.Size), new DevExpress.Utils.AppearanceObject(), ""));
		}
		void clrpckPositiveColor_EditValueChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void clrpckPositiveBorderColor_EditValueChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void clrpckAxisColor_EditValueChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void clrpckNegativeColor_EditValueChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void clrpckNegativeBorderColor_EditValueChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void cmbPositiveFill_SelectedIndexChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void cmbNegativeFill_SelectedIndexChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void cmbBarDirection_SelectedIndexChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void cheUseNegativeBar_CheckedChanged(object sender, EventArgs e) {
			if(!init) {
				lcgNegative.Enabled = cheUseNegativeBar.Checked;
				Update();
			}
		}
		void cmbPositiveBorder_SelectedIndexChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void cmbNegativeBorder_SelectedIndexChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		void cheDrawAxis_CheckedChanged(object sender, EventArgs e) {
			if(!init) Update();
		}
		#endregion
		new void Update() {
			GetFormFormat();
			pctPreview.Invalidate();
		}
		#region GetForm
		public void GetFormFormat() {
			decimal value = 0;
			if(decimal.TryParse(tedValueMax.Text, out value)) dataBar.Maximum = value;
			if(decimal.TryParse(tedValueMin.Text, out value)) dataBar.Minimum = value;
			dataBar.MaximumType = cmbTypeMax.SelectedIndex == 0 ? FormatConditionValueType.Percent : FormatConditionValueType.Number;
			dataBar.MinimumType = cmbTypeMin.SelectedIndex == 0 ? FormatConditionValueType.Percent : FormatConditionValueType.Number;
			GetFormUseNegativeAxis();
			GetFormDrawAxis();
			GetFormAxisColor();
			GetFormPositiveColor();
			GetFormPositiveBorderColor();
			GetFormUsePositiveBorder();
			GetFormPositiveColorFill();
			GetFormNegativeColor();
			GetFormNegativeBorderColor();
			GetFormUseNegativeBorder();
			GetFormNegativeColorFill();
			GetFormDirection();
		}   
		void GetFormDrawAxis() {
			dataBar.DrawAxis = cheDrawAxis.Checked;
		}
		void GetFormUseNegativeAxis() {
			dataBar.AllowNegativeAxis = cheUseNegativeBar.Checked;
		}
		void GetFormAxisColor() {
			dataBar.AxisColor = clrpckAxisColor.Color;
		}
		void GetFormPositiveColor() {
			dataBar.Appearance.BackColor = clrpckPositiveColor.Color;
		}
		void GetFormPositiveBorderColor() {
			dataBar.Appearance.BorderColor = clrpckPositiveBorderColor.Color;
		}
		void GetFormUsePositiveBorder() {
			if(cmbPositiveBorder.SelectedIndex == 0)
				dataBar.Appearance.BorderColor = Color.Empty;
		}
		void GetFormPositiveColorFill() {
			if(cmbPositiveFill.SelectedIndex == 1) dataBar.Appearance.BackColor2 = Color.White;
			else dataBar.Appearance.BackColor2 = Color.Empty;
		}
		void GetFormNegativeColor() {
			dataBar.AppearanceNegative.BackColor2 = clrpckNegativeColor.Color;
		}
		void GetFormNegativeBorderColor() {
			dataBar.AppearanceNegative.BorderColor = clrpckNegativeBorderColor.Color;
		}
		void GetFormUseNegativeBorder() {
			if(cmbNegativeBorder.SelectedIndex == 0)
				dataBar.AppearanceNegative.BorderColor = Color.Empty;
		}
		void GetFormNegativeColorFill() {
			if(cmbNegativeFill.SelectedIndex == 1) dataBar.AppearanceNegative.BackColor = Color.White;
			else dataBar.AppearanceNegative.BackColor = clrpckNegativeColor.Color;
		}
		void GetFormDirection() {
			DevExpress.Utils.DefaultBoolean direction = DevExpress.Utils.DefaultBoolean.Default;
			switch(cmbBarDirection.SelectedIndex) {
				case 0: direction = DevExpress.Utils.DefaultBoolean.Default; break;
				case 1: direction = DevExpress.Utils.DefaultBoolean.False; break;
				case 2: direction = DevExpress.Utils.DefaultBoolean.True; break;
			}
			dataBar.RightToLeft = direction;
		}
		#endregion
		#region SetForm
		public void SetFormFormat() {
			init = true;
			CheckPredefinedName();
			tedValueMin.Text = dataBar.Minimum == 0m ? "0" : dataBar.Minimum.ToString();
			tedValueMax.Text = dataBar.Maximum == 0m ? "100" : dataBar.Maximum.ToString();
			cmbTypeMin.SelectedIndex = dataBar.MinimumType == FormatConditionValueType.Number ? 1 : 0;
			cmbTypeMax.SelectedIndex = dataBar.MaximumType == FormatConditionValueType.Number ? 1 : 0;
			cheUseNegativeBar.Checked = dataBar.AllowNegativeAxis;
			cheDrawAxis.Checked = dataBar.DrawAxis;
			clrpckAxisColor.Color = dataBar.AxisColor.IsEmpty ? Color.Black : dataBar.AxisColor;
			cmbBarDirection.SelectedIndex = SetFormDirection();
			cmbPositiveFill.SelectedIndex = ((DevExpress.XtraExport.Helpers.IFormatConditionRuleDataBar)dataBar).GradientFill ? 1 : 0;
			cmbPositiveBorder.SelectedIndex = ((DevExpress.XtraExport.Helpers.IFormatConditionRuleDataBar)dataBar).BorderColor.IsEmpty ? 0 : 1;
			clrpckPositiveColor.Color = dataBar.Appearance.BackColor.IsEmpty ? Color.LightBlue : dataBar.Appearance.BackColor;
			clrpckPositiveBorderColor.Color = dataBar.Appearance.BorderColor.IsEmpty ? Color.Blue : dataBar.Appearance.BorderColor;
			cmbNegativeFill.SelectedIndex = ((DevExpress.XtraExport.Helpers.IFormatConditionRuleDataBar)dataBar).GradientFill ? 1 : 0;
			cmbNegativeBorder.SelectedIndex = ((DevExpress.XtraExport.Helpers.IFormatConditionRuleDataBar)dataBar).BorderColor.IsEmpty ? 0 : 1;
			clrpckNegativeColor.Color = dataBar.AppearanceNegative.BackColor2.IsEmpty ? Color.FromArgb(255, 198, 198) : dataBar.AppearanceNegative.BackColor2;
			clrpckNegativeBorderColor.Color = dataBar.AppearanceNegative.BorderColor.IsEmpty ? Color.Red : dataBar.AppearanceNegative.BorderColor;
			init = false;
		}
		void CheckPredefinedName() {
			if(string.IsNullOrEmpty(dataBar.PredefinedName)) return;
			foreach(var item in FormatPredefinedDataBarSchemes.Default) {
				if(dataBar.PredefinedName == item.Key) {
					dataBar.Appearance.Assign(item.Positive);
					dataBar.AppearanceNegative.Assign(item.Negative);
					dataBar.AxisColor = item.AxisColor;
					break;
				}
			}
		}
		int SetFormDirection() {
			int directionIndex = 0;
			switch(dataBar.RightToLeft) {
				case DefaultBoolean.Default: directionIndex = 0; break;
				case DefaultBoolean.False: directionIndex = 1; break;
				case DefaultBoolean.True: directionIndex = 2; break;
			}
			return directionIndex;
		}
		#endregion
	}
}

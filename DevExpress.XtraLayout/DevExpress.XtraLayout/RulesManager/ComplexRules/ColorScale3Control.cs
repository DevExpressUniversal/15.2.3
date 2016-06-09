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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public partial class ColorScale3Control : XtraUserControl, IFormatConditionRule, IRuleControl {
		FormatConditionRule3ColorScale colorScale;
		FormatRule formatRuleCore;
		bool init; 
		public ColorScale3Control() {
			InitializeComponent();
			InitLocalizationText();
			CreateRule();
			SetFormFormat();
		}
		void InitLocalizationText() {
			sliMinimum.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonMinimum);
			sliMaximum.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonMaximum);
			lciTypeMin.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonType);
			lciTypeMax.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonType);
			lciValueMin.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonValue);
			lciValueMax.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonValue);
			lciColorMin.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonColor);
			lciColorMax.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonColor);
			sliMidpoint.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleColorScaleMidpoint);
			lciTypeMid.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonType);
			lciValueMid.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonValue);
			lciColorMid.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonColor);
			cmbTypeMin.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonPercent),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonNumber)
			});
			cmbTypeMax.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonPercent),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonNumber)
			});
			cmbTypeMid.Properties.Items.AddRange(new object[] {
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonPercent),
				Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonNumber)
			});
			lciPreview.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonPreview);
		}
		void CreateRule() {
			 colorScale = new FormatConditionRule3ColorScale();
		}
		#region IFormatConditionRule
		public FormatRule GetFormatRule() {
			UpdateFormatRule();
			return formatRuleCore;
		}
		void UpdateFormatRule() {
			GetFormFormat();
			FormatConditionRule3ColorScale cs = new FormatConditionRule3ColorScale();
			cs.Assign(colorScale);
			formatRuleCore.RuleBase = cs;
			formatRuleCore.RuleType = RuleType.ColorScale3;
			formatRuleCore.RuleName = Localizer.Active.GetLocalizedString(StringId.ManageRuleColorScale);
		}
		public void SetFormatRule(FormatRule formatRule) {
			formatRuleCore = formatRule;
			var rule = formatRuleCore.RuleBase as FormatConditionRule3ColorScale;
			if(rule == null) {
				colorScale = new FormatConditionRule3ColorScale();
				SetFormFormat();
				return;
			}
			colorScale.Assign(rule);
			SetFormFormat();
		}
		#endregion
		void pctPreview_Paint(object sender, PaintEventArgs e) {
			((IFormatConditionDrawPreview)colorScale).Draw(new FormatConditionDrawPreviewArgs(e.Graphics, new Rectangle(Point.Empty, pctPreview.Size), new DevExpress.Utils.AppearanceObject(), ""));
		}
		void clrpckMinColor_EditValueChanged(object sender, System.EventArgs e) {
			if(!init) Update();
		}
		void clrpckMidColor_EditValueChanged(object sender, System.EventArgs e) {
			if(!init) Update();
		}
		void clrpckMaxColor_EditValueChanged(object sender, System.EventArgs e) {
			if(!init) Update();
		}
		new void Update() {
			GetFormFormat();
			pctPreview.Invalidate();
		}
		#region GetForm
		public void GetFormFormat() {
			decimal value = 0;
			if(decimal.TryParse(tedValueMin.Text, out value)) { colorScale.Minimum = value; }
			if(decimal.TryParse(tedValueMax.Text, out value)) { colorScale.Maximum = value; }
			if(decimal.TryParse(tedValueMid.Text, out value)) { colorScale.Middle = value; }
			colorScale.MinimumType = cmbTypeMin.SelectedIndex == 0 ? FormatConditionValueType.Percent : FormatConditionValueType.Number;
			colorScale.MaximumType = cmbTypeMax.SelectedIndex == 0 ? FormatConditionValueType.Percent : FormatConditionValueType.Number;
			colorScale.MiddleType = cmbTypeMid.SelectedIndex == 0 ? FormatConditionValueType.Percent : FormatConditionValueType.Number;
			GetFormMinColor();
			GetFormMaxColor();
			GetFormMidColor();
		}
		void GetFormMinColor() {
			colorScale.MinimumColor = clrpckColorMin.Color;
		}
		void GetFormMaxColor() {
			colorScale.MaximumColor = clrpckColorMax.Color;
		}
		void GetFormMidColor() {
			colorScale.MiddleColor = clrpckColorMid.Color;
		}
		#endregion
		#region SetForm
		public void SetFormFormat() {
			init = true;
			CheckPredefinedName();
			cmbTypeMin.SelectedIndex = colorScale.MinimumType == FormatConditionValueType.Number ? 1 : 0;
			cmbTypeMid.SelectedIndex = colorScale.MiddleType == FormatConditionValueType.Number ? 1 : 0;
			cmbTypeMax.SelectedIndex = colorScale.MaximumType == FormatConditionValueType.Number ? 1 : 0;
			tedValueMin.Text = colorScale.Minimum == 0m ? "0" : colorScale.Minimum.ToString();
			tedValueMid.Text = colorScale.Middle == 0m ? "50" : colorScale.Middle.ToString();
			tedValueMax.Text = colorScale.Maximum == 0m ? "100" : colorScale.Maximum.ToString();
			clrpckColorMin.Color = colorScale.MinimumColor.IsEmpty ? Color.FromArgb(248, 105, 107) : colorScale.MinimumColor;
			clrpckColorMid.Color = colorScale.MiddleColor.IsEmpty ? Color.FromArgb(255, 235, 132) : colorScale.MiddleColor;
			clrpckColorMax.Color = colorScale.MaximumColor.IsEmpty ? Color.FromArgb(99, 190, 123) : colorScale.MaximumColor;
			init = false;
		}
		void CheckPredefinedName() {
			if(string.IsNullOrEmpty(colorScale.PredefinedName)) return;
			foreach(var item in FormatPredefinedColorScales.Default.Find(LookAndFeel)) {
				if(colorScale.PredefinedName == item.Key) {
					colorScale.MaximumColor = colorScale.MaximumColor.IsEmpty ? item.MaximumColor : colorScale.MaximumColor;
					colorScale.MinimumColor = colorScale.MinimumColor.IsEmpty ? item.MinimumColor : colorScale.MinimumColor;
					colorScale.MiddleColor = colorScale.MiddleColor.IsEmpty ? item.MiddleColor : colorScale.MiddleColor;
					break;
				}
			}
		}
		#endregion
	}
}

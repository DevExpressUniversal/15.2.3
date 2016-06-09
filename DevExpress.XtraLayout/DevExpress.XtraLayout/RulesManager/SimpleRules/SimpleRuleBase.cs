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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public partial class SimpleRuleBase : XtraUserControl, IControlRuleBase, IFormatConditionRule { 
		FormatCells formatCells;
		FormatRule formatRuleCore;  
		FormatConditionRuleAppearanceBase RuleAppearance {
			get {
				if(!IsRuleAppearanceBase())
					formatRuleCore.RuleBase = GetFormatRuleFromForm().RuleBase;
				return formatRuleCore.RuleBase as FormatConditionRuleAppearanceBase;
			}
		}
		bool IsRuleAppearanceBase() {
			return (formatRuleCore.RuleBase is FormatConditionRuleAppearanceBase);
		}
		FormatCells FormatCells {
			get {
				if(formatCells == null) formatCells = new FormatCells();  
				return formatCells;
			}
		}
		public SimpleRuleBase() {
			InitializeComponent();
			InitLocalizationText();
		}
		void InitLocalizationText() {
			lciPreview.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleCommonPreview);
			btnFormat.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleSimpleRuleBaseFormat);
		}
		void btnFormatForm_Click(object sender, EventArgs e) {
			FormatCells.SetAppearance(RuleAppearance.Appearance, RuleAppearance.PredefinedName);
			if(FormatCells.ShowDialog() == DialogResult.OK)
				SetFormatRuleAppearanceAndPredefinedName(FormatCells.GetAppearance(), FormatCells.GetPredefinedName());
			pctPreview.Invalidate();
			formatCells = null;
		}
		void SetFormatRuleAppearanceAndPredefinedName(AppearanceObject appearance, string predefinedName) {
			if(!(appearance.IsEqual(RuleAppearance.Appearance))) RuleAppearance.Appearance.Assign(appearance);
			if(predefinedName != null && RuleAppearance.PredefinedName != predefinedName) RuleAppearance.PredefinedName = predefinedName;
		}
		void pctPreview_Paint(object sender, PaintEventArgs e) {
			DrawPreview(e);
		}
		void DrawPreview(PaintEventArgs e) {
			ClearPreview(e);
			if(RuleAppearance == null || CheckPredefinedName(RuleAppearance.Appearance, RuleAppearance.PredefinedName).IsEqual(AppearanceObject.EmptyAppearance)) DrawNoFormat(e);
			else ((IFormatConditionDrawPreview)RuleAppearance).Draw(new FormatConditionDrawPreviewArgs(e.Graphics, new Rectangle(Point.Empty, pctPreview.Size), null, "AaBbCcYyZz"));
		}
		void ClearPreview(PaintEventArgs e) {
			Rectangle client = pctPreview.ClientRectangle;
			client.Inflate(-2, -2);
			e.Graphics.FillRectangle(Brushes.White, client);
			pctPreview.Properties.Appearance.ForeColor = Color.White;
		}
		FormatRule GetFormatRuleFromForm() {
			return ((IFormatConditionRule)PnlFormatSetting.Controls[0]).GetFormatRule();
		}
		public PanelControl PnlFormatSetting {
			get { return pnlFormatSetting; }
		}
		#region IFormatConditionRule
		public FormatRule GetFormatRule() {
			var appearance = RuleAppearance.Appearance;
			var predefinedName = RuleAppearance.PredefinedName;
			var formatRule = GetFormatRuleFromForm();
			formatRuleCore.Assign(formatRule);
			SetFormatRuleAppearanceAndPredefinedName(appearance, predefinedName);
			return formatRuleCore;
		}
		public void SetFormatRule(FormatRule formatRule) {
			formatRuleCore = new FormatRule();
			formatRuleCore.Assign(formatRule);
		}
		#endregion
		AppearanceObject CheckPredefinedName(AppearanceObject appearance, string predefinedAppearance) {
			if(string.IsNullOrEmpty(predefinedAppearance)) return appearance;
			FormatPredefinedAppearance predefined = FormatPredefinedAppearances.Default.Find(LookAndFeel, predefinedAppearance);
			if(predefined == null) return appearance;
			AppearanceObjectEx res = new AppearanceObjectEx();
			AppearanceHelper.Combine(res, new AppearanceObject[] { appearance }, predefined.Appearance);
			return res;
		}
		public PointF GetCenter(string info) {
			float locationX = (pctPreview.Width - (info.Length * Appearance.Font.SizeInPoints)) * 0.5f;
			float locationY = pctPreview.Height * 0.5f - Appearance.Font.Size;
			return new PointF(locationX, locationY);
		}											  
		void DrawNoFormat(PaintEventArgs e) {
			string info = Localizer.Active.GetLocalizedString(StringId.ManageRuleNoFormatSet);
			e.Graphics.DrawString(info, Appearance.Font, Brushes.Black, GetCenter(info));
		}
	}
}

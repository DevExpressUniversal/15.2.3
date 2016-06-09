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

using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace DevExpress.XtraEditors.Frames {
	public partial class FormatCells : XtraForm {
		RepositoryItemColorPickEdit prop;
		AppearanceObject app;
		public FormatCells() { 
			InitializeComponent();
			InitializeFontStyle();
			InitializePredefinedAppearance();							
			SetInnerColorPickControl();		 
			CreateAppearance();
			SetDefaultAppearance();
			InitLocalizationText();
			SetSizeTouchMode();
		}
		void InitLocalizationText() {
			Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsCaption);
			lcgFont.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsFont);
			lcgFill.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsFill);
			lcgPredefinedAppearance.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsPredefinedAppearance);
			lciFont.Text =  string.Format("{0}:", Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsFont));
			lciFontStyle.Text = string.Format("{0}:", Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsFontStyle));
			lciFontColor.Text = string.Format("{0}:", Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsFontColor));
			sliEffects.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsEffects);
			cheUnderline.Properties.Caption = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsUnderline);
			cheStrikethrough.Properties.Caption = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsStrikethrough);
			lciPickEdit.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsBackgroundColor);
			lciPredefinedAppearance.Text = string.Format("{0}:", Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsPredefinedAppearance));
			btnFontClear.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsClear);
			btnFillClear.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsClear);
			btnPredefinedAppearanceClear.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsClear);
			btnOK.Text = Localizer.Active.GetLocalizedString(StringId.OK);
			btnCancel.Text = Localizer.Active.GetLocalizedString(StringId.Cancel);
		}
		void CreateAppearance() {
			app = new AppearanceObjectEx();
		}
		void SetSizeTouchMode() {
			if(WindowsFormsSettings.TouchUIMode != DevExpress.LookAndFeel.TouchUIMode.True) return;
			Size = new Size(307, 465);
			int maxWidthButton = Math.Max(lciOK.Width, lciCancel.Width);
			lciOK.Width = lciCancel.Width = maxWidthButton;
		}
		void InitializeFontStyle() {
			cmbFontStyle.Properties.Items.AddRange(new object[] {
				new FontStyleDisplayObject() { Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsRegular), FontStyle = FontStyle.Regular },
				new FontStyleDisplayObject() { Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsBold), FontStyle = FontStyle.Bold },
				new FontStyleDisplayObject() { Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsItalic), FontStyle = FontStyle.Italic },
				new FontStyleDisplayObject() {
					Description = string.Format("{0} {1}", Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsBold),
														   Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsItalic)),
					FontStyle = FontStyle.Bold | FontStyle.Italic }
			});
			cmbFontStyle.SelectedIndex = 0;
		}
		void InitializePredefinedAppearance() {
			cmbPredefinedAppearance.Properties.DropDownRows = 16;
			cmbPredefinedAppearance.Properties.Items.Add(new PredefinedAppearanceDisplayObject() {
				Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleFormatCellsNone), Key = ""
			});
			foreach(FormatPredefinedAppearance app in FormatPredefinedAppearances.Default.Find(this.LookAndFeel))
				cmbPredefinedAppearance.Properties.Items.Add(new PredefinedAppearanceDisplayObject() { Description = app.Title, Key = app.Key });
			cmbPredefinedAppearance.SelectedIndex = 0;
		}
		void InitForm(AppearanceObject appP, string predefAppearance) {
			InitFont(appP);
			InitFill(appP.BackColor);
			InitAppearance(predefAppearance);
		}
		void InitFont(AppearanceObject appP) {
			fedAppearance.EditValue = appP.Font.FontFamily.Name;
			cpeColorFont.Color = appP.ForeColor;
			cheStrikethrough.Checked = appP.Font.Strikeout;
			cheUnderline.Checked = appP.Font.Underline;
			cmbFontStyle.SelectedIndex = GetFontStyleIndex(appP);
		}
		void InitFill(Color fill) {
			colorPicker.SelectedColor = fill;
		}
		void InitAppearance(string predefAppearance) {
			int i = 0;
			foreach(PredefinedAppearanceDisplayObject item in cmbPredefinedAppearance.Properties.Items) {
				if(item.Key == predefAppearance) { cmbPredefinedAppearance.SelectedIndex = i; break; }
				i++;
			}
		}
		int GetFontStyleIndex(AppearanceObject appP) {
			FontStyle fs = CheckValidFontStyle(appP.Font.Style);
			int index = 0;
			foreach(FontStyleDisplayObject item in cmbFontStyle.Properties.Items) {
				if(fs == item.FontStyle) return index;
				index++;
			}
			return -1;
		}
		FontStyle CheckValidFontStyle(FontStyle fontStyle) {
			return ~(FontStyle.Underline | FontStyle.Strikeout) & fontStyle;
		}
		void SetInnerColorPickControl() {
			this.prop = new RepositoryItemColorPickEdit();
			colorPicker.StandardColors.AddColorRange(prop.StandardColors.ToList());
			colorPicker.ThemeColors.AddColorRange(prop.ThemeColors.ToList());
		}
		void SetDefaultAppearance() {
			app.Assign(AppearanceObjectEx.EmptyAppearance);
			InitForm(app, string.Empty);
		}
		public void SetAppearance(AppearanceObject app, string predefAppearance) {
			this.app = app;
			InitForm(app, predefAppearance);
		}
		public AppearanceObject GetAppearance() {
			FontStyleDisplayObject fsObj = cmbFontStyle.SelectedItem as FontStyleDisplayObject;
			FontStyle fs = (fsObj == null) ? FontStyle.Regular : fsObj.FontStyle;
			if(cheUnderline.Checked) fs |= FontStyle.Underline;
			if(cheStrikethrough.Checked) fs |= FontStyle.Strikeout;
			app.Font = IsDefaultFont(fs) ? AppearanceObject.DefaultFont : new Font(fedAppearance.SelectedItem.ToString(), AppearanceObject.DefaultFont.Size, fs);
			app.BackColor = colorPicker.SelectedColor;   
			app.ForeColor = cpeColorFont.Color;
			return app;
		}
		public string GetPredefinedName() {
			return ((PredefinedAppearanceDisplayObject)cmbPredefinedAppearance.SelectedItem).Key;
		}
		void btnFillClear_Click(object sender, EventArgs e) {
			InitFill(Color.Empty);
		}
		void btnFontClear_Click(object sender, EventArgs e) {
			InitFont(AppearanceObjectEx.EmptyAppearance);
		}
		bool IsDefaultFont(FontStyle fs) {
			return (fs == FontStyle.Regular && fedAppearance.SelectedItem.ToString() == AppearanceObject.DefaultFont.Name);
		}
		void btnPredefinedAppearanceClear_Click(object sender, EventArgs e) {
			InitAppearance(string.Empty);
		}
	}
	public class FontStyleDisplayObject {
		public override string ToString() { return Description; }
		public string Description { get; set; }
		public FontStyle FontStyle { get; set; }
	}
	public class PredefinedAppearanceDisplayObject {
		public override string ToString() { return Description; }
		public string Description { get; set; }
		public string Key { get; set; }
	}
}

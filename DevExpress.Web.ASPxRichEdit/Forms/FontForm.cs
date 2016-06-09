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
using DevExpress.Utils.Internal;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class FontForm : RichEditDialogBase {
		protected ASPxComboBox FontNameComboBox { get; private set; }
		protected ASPxComboBox FontStyleComboBox { get; private set; }
		protected ASPxComboBox FontSizeComboBox { get; private set; }
		protected ASPxComboBox UnderlineStyleComboBox { get; private set; }
		protected ASPxCheckBox AllCapsCheckBox { get; private set; }
		protected ASPxCheckBox HiddenCheckBox { get; private set; }
		protected ASPxCheckBox UnderlineCheckBox { get; private set; }
		protected ASPxColorEdit FontColorEdit { get; private set; }
		protected ASPxColorEdit UnderlinColorEdit { get; private set; }
		protected ASPxLabel EffectLabel { get; private set; }
		protected ASPxRadioButtonList StrikeoutRadioButtonList { get; private set; }
		protected ASPxRadioButtonList SubscriptRadioButtonList { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			group.ColCount = 4;
			FontNameComboBox = group.Items.CreateComboBox("CbxFontName", colSpan: 2, buffer: Editors, location: LayoutItemCaptionLocation.Top);
			FontStyleComboBox = group.Items.CreateComboBox("CbxFontStyle", buffer: Editors, location: LayoutItemCaptionLocation.Top, cssClassName: "dxre-dialogEditor");
			FontSizeComboBox = group.Items.CreateComboBox("CbxFontSize", buffer: Editors, location: LayoutItemCaptionLocation.Top, cssClassName: "dxre-dialogEditor");
			FontColorEdit = group.Items.CreateEditor<ASPxColorEdit>("CeFontColor", buffer: Editors, location: LayoutItemCaptionLocation.Top, cssClassName: "dxre-dialogEditor");
			UnderlineStyleComboBox = group.Items.CreateComboBox("CbxUnderlineStyle", colSpan: 2, buffer: Editors, location: LayoutItemCaptionLocation.Top);
			UnderlinColorEdit = group.Items.CreateEditor<ASPxColorEdit>("CeUnderlineColor", buffer: Editors, location: LayoutItemCaptionLocation.Top, cssClassName: "dxre-dialogEditor");
			group.Items.AddEmptyLine();
			EffectLabel = group.Items.CreateLabel(buffer: Editors);
			group.Items.Add<EmptyLayoutItem>("").ColSpan = 3;
			StrikeoutRadioButtonList = group.Items.CreateEditor<ASPxRadioButtonList>("RblStrikeout", buffer: Editors, showCaption: false);
			SubscriptRadioButtonList = group.Items.CreateEditor<ASPxRadioButtonList>("RblSubscript", buffer: Editors, showCaption: false);
			var checkBoxLayout = new RichEditDialogFormLayout();
			checkBoxLayout.CssClass = "dxreCheckBoxContainer";
			group.Items.CreateItem("", checkBoxLayout).ColSpan = 2;
			AllCapsCheckBox = checkBoxLayout.Items.CreateCheckBox("ChkAllCaps", buffer: Editors);
			HiddenCheckBox = checkBoxLayout.Items.CreateCheckBox("ChkHidden", buffer: Editors);
			UnderlineCheckBox = checkBoxLayout.Items.CreateCheckBox("CbUnderlineWordsOnly", buffer: Editors);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			for(int i = 0; i < WebFontInfoCache.DefaultFonts.Length; i++)
				FontNameComboBox.Items.Add(WebFontInfoCache.DefaultFonts[i].Name, WebFontInfoCache.DefaultFonts[i].Name);
			FontStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Normal), 0);
			FontStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Bold), 1);
			FontStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Italic), 2);
			FontStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.BoldItalic), 3);
			FontStyleComboBox.ValueType = typeof(int);
			foreach(int fontSize in FontManager.GetPredefinedFontSizes())
				FontSizeComboBox.Items.Add(fontSize.ToString());
			FontSizeComboBox.ValueType = typeof(int);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_None), (int)XtraRichEdit.Model.UnderlineType.None);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_Single), (int)XtraRichEdit.Model.UnderlineType.Single);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_Dotted), (int)XtraRichEdit.Model.UnderlineType.Dotted);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_Dashed), (int)XtraRichEdit.Model.UnderlineType.Dashed);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_DashDotted), (int)XtraRichEdit.Model.UnderlineType.DashDotted);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_DashDotDotted), (int)XtraRichEdit.Model.UnderlineType.DashDotDotted);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_Double), (int)XtraRichEdit.Model.UnderlineType.Double);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_HeavyWave), (int)XtraRichEdit.Model.UnderlineType.HeavyWave);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_LongDashed), (int)XtraRichEdit.Model.UnderlineType.LongDashed);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_ThickSingle), (int)XtraRichEdit.Model.UnderlineType.ThickSingle);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_ThickDotted), (int)XtraRichEdit.Model.UnderlineType.ThickDotted);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_ThickDashed), (int)XtraRichEdit.Model.UnderlineType.ThickDashed);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_ThickDashDotted), (int)XtraRichEdit.Model.UnderlineType.ThickDashDotted);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_ThickDashDotDotted), (int)XtraRichEdit.Model.UnderlineType.ThickDashDotDotted);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_ThickLongDashed), (int)XtraRichEdit.Model.UnderlineType.ThickLongDashed);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_DoubleWave), (int)XtraRichEdit.Model.UnderlineType.DoubleWave);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_Wave), (int)XtraRichEdit.Model.UnderlineType.Wave);
			UnderlineStyleComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineType_DashSmallGap), (int)XtraRichEdit.Model.UnderlineType.DashSmallGap);
			UnderlineStyleComboBox.ValueType = typeof(int);
			MainFormLayout.FindItemOrGroupByName("RblStrikeout").VerticalAlign = FormLayoutVerticalAlign.Top;
			MainFormLayout.FindItemOrGroupByName("RblSubscript").VerticalAlign = FormLayoutVerticalAlign.Top;
			PrepareRadioButtonList();
			PrepareColorEdit(FontColorEdit);
			PrepareColorEdit(UnderlinColorEdit);
		}
		void PrepareRadioButtonList() {
			StrikeoutRadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.StrikeoutType_None), (int)XtraRichEdit.Model.StrikeoutType.None);
			StrikeoutRadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.StrikeoutType_Single), (int)XtraRichEdit.Model.StrikeoutType.Single);
			StrikeoutRadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.StrikeoutType_Double), (int)XtraRichEdit.Model.StrikeoutType.Double);
			SetupDefaultListSettings(StrikeoutRadioButtonList);
			SubscriptRadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CharacterFormattingScript_Normal), (int)DevExpress.Office.CharacterFormattingScript.Normal);
			SubscriptRadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CharacterFormattingScript_Subscript), (int)DevExpress.Office.CharacterFormattingScript.Subscript);
			SubscriptRadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CharacterFormattingScript_Superscript), (int)DevExpress.Office.CharacterFormattingScript.Superscript);
			SetupDefaultListSettings(SubscriptRadioButtonList);
		}
		void PrepareColorEdit(ASPxColorEdit colorEdit) {
			colorEdit.EnableAutomaticColorItem = true;
			colorEdit.EnableCustomColors = true;
			colorEdit.AutomaticColor = Color.Black;
			colorEdit.AutomaticColorItemValue = "Auto";
		}
		void SetupDefaultListSettings(ASPxRadioButtonList list) {
			list.ValueType = typeof(int);
			list.SelectedIndex = 0;
			list.Border.BorderWidth = 0;
		}
		protected override void Localize() {
			MainFormLayout.LocalizeField("CbxFontName", ASPxRichEditStringId.Font);
			MainFormLayout.LocalizeField("CbxFontStyle", ASPxRichEditStringId.FontStyle);
			MainFormLayout.LocalizeField("CbxFontSize", ASPxRichEditStringId.FontSize);
			MainFormLayout.LocalizeField("CeFontColor", ASPxRichEditStringId.FontColor);
			MainFormLayout.LocalizeField("CbxUnderlineStyle", ASPxRichEditStringId.UnderlineStyle);
			MainFormLayout.LocalizeField("CeUnderlineColor", ASPxRichEditStringId.UnderlineColor);
			EffectLabel.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Effects) + ":";
			AllCapsCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.AllCaps);
			HiddenCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Hidden);
			UnderlineCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.UnderlineWordsOnly);
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgFontForm";
		}
	}
}

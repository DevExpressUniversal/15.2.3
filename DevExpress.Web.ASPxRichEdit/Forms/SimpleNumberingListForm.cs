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
using DevExpress.Office.NumberConverters;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class SimpleNumberingListForm : RichEditDialogBase {
		protected ASPxComboBox NumberPositionComboBox { get; private set; }
		protected ASPxComboBox NumberStyleComboBox { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			LayoutGroup numberFormatGroup = group.Items.Add<LayoutGroup>("", "NumberFormat");
			numberFormatGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			numberFormatGroup.ColCount = 2;
			numberFormatGroup.Items.CreateTextBox("TxbNumberFormat", showCaption: false, buffer: Editors).CssClass = "dxre-dialogLongEditor";
			ASPxButton fontButton = CreateDialogButton("BtnFont", ASPxRichEditStringId.Numbering_Font);
			numberFormatGroup.Items.CreateItem("FontButton", fontButton).ShowCaption = Utils.DefaultBoolean.False;
			NumberStyleComboBox = numberFormatGroup.Items.CreateComboBox("CbxNumberStyle", cssClassName: "dxre-dialogLongEditor", location: LayoutItemCaptionLocation.Top, buffer: Editors);
			ASPxSpinEdit startAtSpinEdit = numberFormatGroup.Items.CreateEditor<ASPxSpinEdit>("SpnStartAt", cssClassName: "dxre-dialogShortEditor", buffer: Editors);
			LayoutGroup numberPositionGroup = group.Items.Add<LayoutGroup>("", "NumberPosition");
			numberPositionGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			numberPositionGroup.ColCount = 2;
			NumberPositionComboBox = numberPositionGroup.Items.CreateComboBox("CbxNumberPosition", cssClassName: "dxre-dialogLongEditor", showCaption: false, buffer: Editors);
			ASPxSpinEdit alignedAtSpinEdit = numberPositionGroup.Items.CreateEditor<ASPxSpinEdit>("SpnAlignedAt", cssClassName: "dxre-dialogShortEditor", buffer: Editors);
			alignedAtSpinEdit.SetupDefaultSettings(-4.58m, 4.58m, UnitFormatString);
			LayoutGroup textPositionGroup = group.Items.Add<LayoutGroup>("", "TextPosition");
			textPositionGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			ASPxSpinEdit indentAtSpinEdit = textPositionGroup.Items.CreateEditor<ASPxSpinEdit>("SpnIndentAt", cssClassName: "dxre-dialogShortEditor", buffer: Editors);
			indentAtSpinEdit.SetupDefaultSettings(-4.58m, 4.58m, UnitFormatString);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			MainFormLayout.FindItemOrGroupByName("FontButton").HorizontalAlign = FormLayoutHorizontalAlign.Right;
			MainFormLayout.FindItemOrGroupByName("SpnStartAt").HorizontalAlign = FormLayoutHorizontalAlign.Right;
			MainFormLayout.FindItemOrGroupByName("SpnStartAt").VerticalAlign = FormLayoutVerticalAlign.Bottom;
			MainFormLayout.FindItemOrGroupByName("SpnIndentAt").HorizontalAlign = FormLayoutHorizontalAlign.Right;
			InitNumberFormatItems();
			InitNumberStyleItems();
		}
		protected override void Localize() {
			MainFormLayout.LocalizeField("NumberFormat", ASPxRichEditStringId.Numbering_NumberFormat);
			MainFormLayout.LocalizeField("CbxNumberStyle", ASPxRichEditStringId.Numbering_NumberStyle);
			MainFormLayout.LocalizeField("NumberPosition", ASPxRichEditStringId.Numbering_NumberPosition);
			MainFormLayout.LocalizeField("TextPosition", ASPxRichEditStringId.Numbering_TextPosition);
			MainFormLayout.LocalizeField("SpnStartAt", ASPxRichEditStringId.Numbering_StartAt);
			MainFormLayout.LocalizeField("SpnAlignedAt", ASPxRichEditStringId.Numbering_AlignedAt);
			MainFormLayout.LocalizeField("SpnIndentAt", ASPxRichEditStringId.Numbering_IndentAt);
		}
		void InitNumberFormatItems() {
			NumberPositionComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Numbering_Left), (int)ListNumberAlignment.Left);
			NumberPositionComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Numbering_Center), (int)ListNumberAlignment.Center);
			NumberPositionComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Numbering_Right), (int)ListNumberAlignment.Right);
			NumberPositionComboBox.ValueType = typeof(int);
		}
		void InitNumberStyleItems() {
			NumberStyleComboBox.Items.Clear();
			List<NumberingFormat> values = OrdinalBasedNumberConverter.GetSupportNumberingFormat();
			int count = values.Count;
			for(int i = 0; i < count; i++) {
				OrdinalBasedNumberConverter converter = OrdinalBasedNumberConverter.CreateConverter(values[i], LanguageId.English);
				string text = String.Format("{0},{1},{2}...", converter.ConvertNumber(1), converter.ConvertNumber(2), converter.ConvertNumber(3));
				NumberStyleComboBox.Items.Add(text, (int)values[i]);
			}
			NumberStyleComboBox.ValueType = typeof(int);
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgSimpleNumberingListForm";
		}
	}
}

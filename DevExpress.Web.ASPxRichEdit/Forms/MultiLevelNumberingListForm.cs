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
using System.Web.UI.WebControls;
using DevExpress.Office.NumberConverters;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class MultiLevelNumberingListForm : RichEditDialogBase {
		protected ASPxComboBox NumberPositionComboBox { get; private set; }
		protected ASPxComboBox NumberStyleComboBox { get; private set; }
		protected ASPxComboBox FollowNumberWithComboBox { get; private set; }
		protected ASPxListBox LevelListBox { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			group.ColCount = 2;
			LevelListBox = new ASPxListBox();
			LevelListBox.CssClass = "dxreMultiLevelNumberingListBoxLayout";
			LevelListBox.ClientInstanceName = GetClientInstanceName("LbLevel");
			LayoutItem level = group.Items.CreateItem("Level", LevelListBox);
			level.CaptionSettings.Location = LayoutItemCaptionLocation.Top;
			level.Height = Unit.Percentage(100);
			level.RowSpan = 2;
			LayoutGroup numberFormatGroup = group.Items.Add<LayoutGroup>("", "NumberFormat");
			numberFormatGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			numberFormatGroup.ColCount = 2;
			numberFormatGroup.Items.CreateTextBox("TxbNumberFormat", colSpan: 2, showCaption: false, buffer: Editors);
			NumberStyleComboBox = numberFormatGroup.Items.CreateComboBox("CbxNumberStyle", cssClassName: "dxre-dialogLongEditor", location: LayoutItemCaptionLocation.Top, buffer: Editors);
			ASPxSpinEdit startAtSpinEdit = numberFormatGroup.Items.CreateEditor<ASPxSpinEdit>("SpnStartAt", cssClassName: "dxre-dialogShortEditor", buffer: Editors);
			numberFormatGroup.Items.AddEmptyLine();
			ASPxButton fontButton = CreateDialogButton("BtnFont", ASPxRichEditStringId.Numbering_Font);
			RenderUtils.AppendDefaultDXClassName(fontButton, "dxre-dialogShortEditor");
			numberFormatGroup.Items.CreateItem("FontButton", fontButton).ColSpan = 2;
			LayoutGroup numberPositionGroup = group.Items.Add<LayoutGroup>("", "NumberPosition");
			numberPositionGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			numberPositionGroup.ColCount = 2;
			NumberPositionComboBox = numberPositionGroup.Items.CreateComboBox("CbxNumberPosition", cssClassName: "dxre-dialogLongEditor", showCaption: false, buffer: Editors);
			ASPxSpinEdit alignedAtSpinEdit = numberPositionGroup.Items.CreateEditor<ASPxSpinEdit>("SpnAlignedAt", cssClassName: "dxre-dialogShortEditor", buffer: Editors);
			alignedAtSpinEdit.SetupDefaultSettings(-4.58m, 4.58m, UnitFormatString);
			group.Items.Add<EmptyLayoutItem>("");
			LayoutGroup textPositionGroup = group.Items.Add<LayoutGroup>("", "TextPosition");
			textPositionGroup.ColCount = 2;
			textPositionGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			textPositionGroup.Items.Add<EmptyLayoutItem>("").Width = Unit.Percentage(50);
			ASPxSpinEdit indentAtSpinEdit = textPositionGroup.Items.CreateEditor<ASPxSpinEdit>("SpnIndentAt", cssClassName: "dxre-dialogShortEditor", buffer: Editors);
			indentAtSpinEdit.SetupDefaultSettings(-4.58m, 4.58m, UnitFormatString);
			FollowNumberWithComboBox = textPositionGroup.Items.CreateComboBox("CbxFollowNumberWith", cssClassName: "dxre-dialogLongEditor", colSpan: 2, buffer: Editors);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			MainFormLayout.FindItemOrGroupByName("FontButton").HorizontalAlign = FormLayoutHorizontalAlign.Right;
			MainFormLayout.FindItemOrGroupByName("FontButton").VerticalAlign = FormLayoutVerticalAlign.Bottom;
			MainFormLayout.FindItemOrGroupByName("SpnStartAt").VerticalAlign = FormLayoutVerticalAlign.Bottom;
			MainFormLayout.FindItemOrGroupByName("SpnStartAt").HorizontalAlign = FormLayoutHorizontalAlign.Right;
			MainFormLayout.FindItemOrGroupByName("SpnAlignedAt").VerticalAlign = FormLayoutVerticalAlign.Bottom;
			MainFormLayout.FindItemOrGroupByName("SpnIndentAt").HorizontalAlign = FormLayoutHorizontalAlign.Right;
			MainFormLayout.FindItemOrGroupByName("CbxFollowNumberWith").HorizontalAlign = FormLayoutHorizontalAlign.Right;
			InitLevelItems();
			InitFollowNumberItems();
			InitNumberFormatItems();
			InitNumberStyleItems();
		}
		protected override void Localize() {
			MainFormLayout.LocalizeField("NumberFormat", ASPxRichEditStringId.Numbering_NumberFormat);
			MainFormLayout.LocalizeField("Level", ASPxRichEditStringId.Numbering_Level);
			MainFormLayout.LocalizeField("TxbNumberFormat", ASPxRichEditStringId.Numbering_NumberFormat);
			MainFormLayout.LocalizeField("CbxNumberStyle", ASPxRichEditStringId.Numbering_NumberStyle);
			MainFormLayout.LocalizeField("SpnStartAt", ASPxRichEditStringId.Numbering_StartAt);
			MainFormLayout.LocalizeField("NumberPosition", ASPxRichEditStringId.Numbering_NumberPosition);
			MainFormLayout.LocalizeField("CbxFollowNumberWith", ASPxRichEditStringId.Numbering_FollowNumberWith);
			MainFormLayout.LocalizeField("SpnAlignedAt", ASPxRichEditStringId.Numbering_AlignedAt);
			MainFormLayout.LocalizeField("SpnIndentAt", ASPxRichEditStringId.Numbering_IndentAt);
			MainFormLayout.LocalizeField("TextPosition", ASPxRichEditStringId.Numbering_TextPosition);
		}
		void InitLevelItems() {
			for(int i = 0; i < 9; i++)
				LevelListBox.Items.Add((i + 1).ToString(), i);
		}
		void InitFollowNumberItems() {
			FollowNumberWithComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Numbering_TabCharacter), 0);
			FollowNumberWithComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Numbering_Space), 1);
			FollowNumberWithComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Numbering_Nothing), 2);
			FollowNumberWithComboBox.ValueType = typeof(int);
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
			return "dxreDlgMultiLevelNumberingListForm";
		}
	}
}

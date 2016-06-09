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

using System.Web.UI.WebControls;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class BulletedListForm : RichEditDialogBase {
		static SymbolProperties[] SymbolsProperties = {
			new SymbolProperties((char)8226, "Symbol"),
			new SymbolProperties((char)9702, "Arial"),
			new SymbolProperties((char)9632, "Arial"),
			new SymbolProperties((char)9633, "Arial"),
			new SymbolProperties((char)9830, "Arial"),
			new SymbolProperties((char)9786, "Arial")
		};
		protected override void PopulateContentGroup(LayoutGroup group) {
			LayoutGroup bulletCharacterGroup = group.Items.Add<LayoutGroup>("", "BulletCharacter");
			bulletCharacterGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			bulletCharacterGroup.Items.CreateItem("Character", CreatePresets()).ShowCaption = Utils.DefaultBoolean.False;
			ASPxButton fontButton = CreateDialogButton("BtnFont", ASPxRichEditStringId.Numbering_Font);
			ASPxButton characterButton = CreateDialogButton("BtnChar", ASPxRichEditStringId.Numbering_Character);
			bulletCharacterGroup.Items.CreateItem("", fontButton, characterButton).HorizontalAlign = FormLayoutHorizontalAlign.Right;
			LayoutGroup bulletPositionGroup = group.Items.Add<LayoutGroup>("", "BulletPosition");
			bulletPositionGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			ASPxSpinEdit alignedAtSpinEdit = bulletPositionGroup.Items.CreateEditor<ASPxSpinEdit>("SpnAlignedAt", cssClassName: "dxre-dialogShortEditor", buffer: Editors);
			alignedAtSpinEdit.SetupDefaultSettings(-4.58m, 4.58m, UnitFormatString);
			LayoutGroup textPositionGroup = group.Items.Add<LayoutGroup>("", "TextPosition");
			textPositionGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			ASPxSpinEdit indentAtSpinEdit = textPositionGroup.Items.CreateEditor<ASPxSpinEdit>("SpnIndentAt", cssClassName: "dxre-dialogShortEditor", buffer: Editors);
			indentAtSpinEdit.SetupDefaultSettings(-4.58m, 4.58m, UnitFormatString);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			MainFormLayout.FindItemOrGroupByName("SpnAlignedAt").HorizontalAlign = FormLayoutHorizontalAlign.Right;
			MainFormLayout.FindItemOrGroupByName("SpnIndentAt").HorizontalAlign = FormLayoutHorizontalAlign.Right;
		}
		protected override void Localize() {
			MainFormLayout.LocalizeField("BulletCharacter", ASPxRichEditStringId.Numbering_BulletCharacter);
			MainFormLayout.LocalizeField("BulletPosition", ASPxRichEditStringId.Numbering_BulletPosition);
			MainFormLayout.LocalizeField("TextPosition", ASPxRichEditStringId.Numbering_TextPosition);
			MainFormLayout.LocalizeField("SpnAlignedAt", ASPxRichEditStringId.Numbering_AlignedAt);
			MainFormLayout.LocalizeField("SpnIndentAt", ASPxRichEditStringId.Numbering_IndentAt);
		}
		protected internal WebControl CreatePresets() {
			WebControl table = RenderUtils.CreateTable();
			table.CssClass = "dxreDlgTableWidth";
			WebControl row = RenderUtils.CreateTableRow();
			table.Controls.Add(row);
			for(int i = 0; i < SymbolsProperties.Length; i++) {
				WebControl cell = RenderUtils.CreateTableCell();
				cell.CssClass = "dxreDlgCenter";
				row.Controls.Add(cell);
				ASPxButton button = CreatePresetButton(i);
				cell.Controls.Add(button);
			}
			return table;
		}
		protected internal ASPxButton CreatePresetButton(int index) {
			ASPxButton button = new ASPxButton();
			button.Text = SymbolsProperties[index].UnicodeChar.ToString();
			button.Font.Name = SymbolsProperties[index].FontName;
			button.Font.Size = FontUnit.XLarge;
			button.AutoPostBack = false;
			button.GroupName = "bulletChar";
			button.CssClass = "dxreDlgBulletPresetBtn";
			button.ClientInstanceName = GetClientInstanceName("BulletedPreset") + index.ToString();
			return button;
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgBulletedListForm";
		}
	}
}

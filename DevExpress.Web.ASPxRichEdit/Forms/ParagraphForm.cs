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

using System.Collections.Generic;
using System.Web.UI;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class ParagraphForm : RichEditDialogBase {
		protected ASPxPageControl PageControl { get; private set; }
		protected DialogFormLayoutBase IndentsFormLayout { get; private set; }
		protected DialogFormLayoutBase PaginationFormLayout { get; private set; }
		protected ASPxCheckBox NoSpaceCheckBox { get; private set; }
		protected ASPxCheckBox KeepLinesCheckBox { get; private set; }
		protected ASPxCheckBox PageBreakCheckBox { get; private set; }
		protected ASPxComboBox AlignmentComboBox { get; private set; }
		protected ASPxComboBox OutlineComboBox { get; private set; }
		protected ASPxComboBox SpecialComboBox { get; private set; }
		protected ASPxComboBox LineSpacingComboBox { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			PageControl = new ASPxPageControl();
			group.Items.CreateItem("content", PageControl).ShowCaption = Utils.DefaultBoolean.False;
			IndentsFormLayout = CreateIndentsFormLayout();
			PaginationFormLayout = CreatePaginationFormLayout();
			PageControl.AddTab("ParagraphTab0", "ParagraphTab0", IndentsFormLayout);
			PageControl.AddTab("ParagraphTab1", "ParagraphTab1", PaginationFormLayout);
		}
		DialogFormLayoutBase CreateIndentsFormLayout() {
			var result = new RichEditDialogFormLayout();
			result.CssClass = "dxre-dialogLayoutWhithPaddings";
			result.ColCount = 3;
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.General), buffer: Editors, colSpan: 3);
			AlignmentComboBox = result.Items.CreateComboBox("CbxAlign", buffer: Editors, cssClassName: "dxre-dialogEditor", colSpan: 3);
			OutlineComboBox = result.Items.CreateComboBox("CbxOutlineLevel", buffer: Editors, cssClassName: "dxre-dialogEditor");
			result.Items.AddEmptyLine();
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Indentation), buffer: Editors, colSpan: 3);
			ASPxSpinEdit leftSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnLeft", buffer: Editors, cssClassName: "dxre-dialogEditor");
			leftSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Special) + ":", buffer: Editors);
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.By) + ":", buffer: Editors);
			ASPxSpinEdit rightSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnRight", buffer: Editors, cssClassName: "dxre-dialogEditor");
			rightSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			SpecialComboBox = result.Items.CreateComboBox("CbxSpecial", buffer: Editors, cssClassName: "dxre-dialogEditor", showCaption: false);
			ASPxSpinEdit bySpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnBy", buffer: Editors, showCaption: false, cssClassName: "dxre-dialogShortEditor");
			bySpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			result.Items.AddEmptyLine();
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Spacing), buffer: Editors, colSpan: 3);
			ASPxSpinEdit beforeSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnBefore", buffer: Editors, cssClassName: "dxre-dialogEditor");
			beforeSpinEdit.SetupDefaultSettings(0, 1584, UnitFormatString);
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.LineSpacing) + ":", buffer: Editors);
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.At) + ":", buffer: Editors);
			ASPxSpinEdit afterSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnAfter", buffer: Editors, cssClassName: "dxre-dialogEditor");
			LineSpacingComboBox = result.Items.CreateComboBox("CbxLineSpacing", buffer: Editors, cssClassName: "dxre-dialogEditor", showCaption: false);
			afterSpinEdit.SetupDefaultSettings(0, 1584, UnitFormatString);
			ASPxSpinEdit atSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnAt", buffer: Editors, showCaption: false, cssClassName: "dxre-dialogShortEditor");
			atSpinEdit.SetupDefaultSettings(0.5m, 132, "0.#");
			result.Items.AddEmptyLine();
			NoSpaceCheckBox = result.Items.CreateCheckBox("ChkNoSpace", colSpan: 3, buffer: Editors);
			result.ApplyCommonSettings();
			return result;
		}
		DialogFormLayoutBase CreatePaginationFormLayout() {
			var result = new RichEditDialogFormLayout();
			result.CssClass = "dxre-dialogLayoutWhithPaddings";
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Pagination), buffer: Editors);
			KeepLinesCheckBox = result.Items.CreateCheckBox("ChkKLT", buffer: Editors);
			PageBreakCheckBox = result.Items.CreateCheckBox("ChkPBB", buffer: Editors);
			result.ApplyCommonSettings();
			return result;
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			AlignmentComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Left), (int)XtraRichEdit.Model.ParagraphAlignment.Left);
			AlignmentComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Right), (int)XtraRichEdit.Model.ParagraphAlignment.Right);
			AlignmentComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Centered), (int)XtraRichEdit.Model.ParagraphAlignment.Center);
			AlignmentComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Justified), (int)XtraRichEdit.Model.ParagraphAlignment.Justify);
			AlignmentComboBox.ValueType = typeof(int);
			OutlineComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.BodyText), 0);
			for(int i = 1; i <= 9; i++)
				OutlineComboBox.Items.Add(string.Format("{0} {1}", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Level), i), i);
			OutlineComboBox.ValueType = typeof(int);
			SpecialComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.none), (int)XtraRichEdit.Model.ParagraphFirstLineIndent.None);
			SpecialComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.FirstLine), (int)XtraRichEdit.Model.ParagraphFirstLineIndent.Indented);
			SpecialComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Hanging), (int)XtraRichEdit.Model.ParagraphFirstLineIndent.Hanging);
			SpecialComboBox.ValueType = typeof(int);
			LineSpacingComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Single), (int)XtraRichEdit.Model.ParagraphLineSpacing.Single);
			LineSpacingComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.spacing_1_5_lines), (int)XtraRichEdit.Model.ParagraphLineSpacing.Sesquialteral);
			LineSpacingComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Double), (int)XtraRichEdit.Model.ParagraphLineSpacing.Double);
			LineSpacingComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Multiple), (int)XtraRichEdit.Model.ParagraphLineSpacing.Multiple);
			LineSpacingComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Exactly), (int)XtraRichEdit.Model.ParagraphLineSpacing.Exactly);
			LineSpacingComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.AtLeast), (int)XtraRichEdit.Model.ParagraphLineSpacing.AtLeast);
			LineSpacingComboBox.ValueType = typeof(int);
		}
		protected override void Localize() {
			PageControl.LocalizeField("ParagraphTab0", ASPxRichEditStringId.ParagraphTab0);
			PageControl.LocalizeField("ParagraphTab1", ASPxRichEditStringId.ParagraphTab1);
			IndentsFormLayout.LocalizeField("CbxAlign", ASPxRichEditStringId.Alignment);
			IndentsFormLayout.LocalizeField("CbxOutlineLevel", ASPxRichEditStringId.OutlineLevel);
			IndentsFormLayout.LocalizeField("SpnLeft", ASPxRichEditStringId.Left);
			IndentsFormLayout.LocalizeField("SpnRight", ASPxRichEditStringId.Right);
			IndentsFormLayout.LocalizeField("SpnBefore", ASPxRichEditStringId.Before);
			IndentsFormLayout.LocalizeField("SpnAfter", ASPxRichEditStringId.After);
			NoSpaceCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.NoSpace);
			KeepLinesCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.KLT);
			PageBreakCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PBB);
		}
		protected override void PopulateBottomItemControls(List<Control> controls) {
			ASPxButton tabsButton = CreateDialogButton("BtnTabs", ASPxRichEditStringId.none);
			tabsButton.Text = XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowTabsForm);
			controls.Add(tabsButton);
			base.PopulateBottomItemControls(controls);
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgParagraphForm";
		}
	}
}

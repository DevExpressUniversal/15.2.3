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
using DevExpress.Office.Localization;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class TablePropertiesForm : RichEditDialogBase {
		protected ASPxPageControl PageControl { get; private set; }
		protected DialogFormLayoutBase TableFormLayout { get; private set; }
		protected DialogFormLayoutBase RowFormLayout { get; private set; }
		protected DialogFormLayoutBase ColumnFormLayout { get; private set; }
		protected DialogFormLayoutBase CellFormLayout { get; private set; }
		protected ASPxCheckBox TablePrefWidthCheckBox { get; private set; }
		protected ASPxCheckBox ColumnPrefWidthCheckBox { get; private set; }
		protected ASPxCheckBox CellPrefWidthCheckBox { get; private set; }
		protected ASPxCheckBox SpecifyHeightCheckBox { get; private set; }
		protected ASPxComboBox TableMeasureInComboBox { get; private set; }
		protected ASPxComboBox ColumnMeasureInComboBox { get; private set; }
		protected ASPxComboBox CellMeasureInComboBox { get; private set; }
		protected ASPxComboBox RowHeightComboBox { get; private set; }
		protected ASPxRadioButtonList AlignmentList { get; private set; }
		protected ASPxRadioButtonList VerticalAlignmentList { get; private set; }
		protected ASPxCheckBox AllowSpacingCheckBox { get; private set; }
		protected ASPxCheckBox AutoResizeCheckBox { get; private set; }
		protected ASPxCheckBox SameAsTableCheckBox { get; private set; }
		protected ASPxCheckBox WrapTextCheckBox { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			PageControl = new ASPxPageControl();
			PageControl.ClientInstanceName = GetClientInstanceName("TablePropertiesPageControl");
			group.Items.CreateItem("content", PageControl).ShowCaption = Utils.DefaultBoolean.False;
			TableFormLayout = CreateTableFormLayout();
			RowFormLayout = CreateRowFormLayout();
			ColumnFormLayout = CreateColumnFormLayout();
			CellFormLayout = CreateCellFormLayout();
			PageControl.AddTab("", "0", TableFormLayout);
			PageControl.AddTab("", "1", RowFormLayout);
			PageControl.AddTab("", "2", ColumnFormLayout);
			PageControl.AddTab("", "3", CellFormLayout);
		}
		DialogFormLayoutBase CreateTableFormLayout() {
			var result = new RichEditDialogFormLayout();
			LayoutGroup sizeGroup = result.Items.Add<LayoutGroup>("", "SizeGroup");
			sizeGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			sizeGroup.ColCount = 3;
			TablePrefWidthCheckBox = sizeGroup.Items.CreateCheckBox("ChkTablePrefWidth", buffer: Editors);
			ASPxSpinEdit tablePrefWidthSpinEdit = sizeGroup.Items.CreateEditor<ASPxSpinEdit>("SpnTablePrefWidth", cssClassName: "dxre-dialogShortEditor", showCaption: false, buffer: Editors);
			tablePrefWidthSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			tablePrefWidthSpinEdit.ClientEnabled = false;
			tablePrefWidthSpinEdit.Value = 0;
			TableMeasureInComboBox = sizeGroup.Items.CreateComboBox("CbxTableMeasureIn", cssClassName: "dxre-dialogEditor", buffer: Editors);
			TableMeasureInComboBox.ClientEnabled = false;
			LayoutGroup alignmentGroup = result.Items.Add<LayoutGroup>("", "AlignmentGroup");
			alignmentGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			alignmentGroup.ColCount = 2;
			AlignmentList = alignmentGroup.Items.CreateEditor<ASPxRadioButtonList>("RblAligment", buffer: Editors, showCaption: false);
			result.FindItemOrGroupByName("RblAligment").HorizontalAlign = FormLayoutHorizontalAlign.Left;
			ASPxSpinEdit indentLeftSpinEdit = alignmentGroup.Items.CreateEditor<ASPxSpinEdit>("SpnIndentLeft", cssClassName: "dxre-dialogShortEditor", showCaption: true, buffer: Editors);
			indentLeftSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			LayoutGroup cellMarginsGroup = CreateMarginsGroup(result, "Default");
			LayoutGroup optionsGroup = result.Items.Add<LayoutGroup>("", "OptionsGroup");
			optionsGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			optionsGroup.ColCount = 2;
			AllowSpacingCheckBox = optionsGroup.Items.CreateCheckBox("ChkAllowSpacing", buffer: Editors);
			ASPxSpinEdit cellSpacingSpinEdit = optionsGroup.Items.CreateEditor<ASPxSpinEdit>("SpnSpacing", showCaption: false, buffer: Editors, cssClassName: "dxre-dialogShortEditor");
			cellSpacingSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			cellSpacingSpinEdit.ClientEnabled = false;
			optionsGroup.FindItemOrGroupByName("SpnSpacing").HorizontalAlign = FormLayoutHorizontalAlign.Right;
			AutoResizeCheckBox = optionsGroup.Items.CreateCheckBox("ChkAutoResize", buffer: Editors);
			result.ApplyCommonSettings();
			return result;
		}
		DialogFormLayoutBase CreateRowFormLayout() {
			var result = new RichEditDialogFormLayout();
			LayoutGroup sizeGroup = result.Items.Add<LayoutGroup>("", "SizeGroup");
			sizeGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			sizeGroup.ColCount = 3;
			SpecifyHeightCheckBox = sizeGroup.Items.CreateCheckBox("ChkSpecifyHeight", buffer: Editors);
			ASPxSpinEdit rowHeightSpinEdit = sizeGroup.Items.CreateEditor<ASPxSpinEdit>("SpnRowHeight", cssClassName: "dxre-dialogShortEditor", showCaption: false, buffer: Editors);
			rowHeightSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			rowHeightSpinEdit.ClientEnabled = false;
			RowHeightComboBox = sizeGroup.Items.CreateComboBox("CbxRowHeight", cssClassName: "dxre-dialogEditor", buffer: Editors);
			RowHeightComboBox.ClientEnabled = false;
			ASPxButton prevRowButton = CreateDialogButton("BtnPrevRow", ASPxRichEditStringId.TableProperties_PreviousRow);
			prevRowButton.ClientVisible = false;
			ASPxButton nextRowButton = CreateDialogButton("BtnNextRow", ASPxRichEditStringId.TableProperties_NextRow);
			nextRowButton.ClientVisible = false;
			LayoutItem navigationRow = result.Items.CreateItem("", prevRowButton, nextRowButton);
			navigationRow.HorizontalAlign = FormLayoutHorizontalAlign.Center;
			navigationRow.ShowCaption = Utils.DefaultBoolean.False;
			result.ApplyCommonSettings();
			return result;
		}
		DialogFormLayoutBase CreateColumnFormLayout() {
			var result = new RichEditDialogFormLayout();
			LayoutGroup sizeGroup = result.Items.Add<LayoutGroup>("", "SizeGroup");
			sizeGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			sizeGroup.ColCount = 3;
			ColumnPrefWidthCheckBox = sizeGroup.Items.CreateCheckBox("ChkColumnPrefWidth", buffer: Editors);
			ASPxSpinEdit columnPrefWidthSpinEdit = sizeGroup.Items.CreateEditor<ASPxSpinEdit>("SpnColumnPrefWidth", cssClassName: "dxre-dialogShortEditor", showCaption: false, buffer: Editors);
			columnPrefWidthSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			columnPrefWidthSpinEdit.ClientEnabled = false;
			ColumnMeasureInComboBox = sizeGroup.Items.CreateComboBox("CbxColumnMeasureIn", cssClassName: "dxre-dialogEditor", buffer: Editors);
			ColumnMeasureInComboBox.ClientEnabled = false;
			ASPxButton prevColumnButton = CreateDialogButton("BtnPrevColumn", ASPxRichEditStringId.TableProperties_PreviousColumn);
			prevColumnButton.ClientVisible = false;
			ASPxButton nextColumnButton = CreateDialogButton("BtnNextColumn", ASPxRichEditStringId.TableProperties_NextColumn);
			nextColumnButton.ClientVisible = false;
			LayoutItem navigationColumn = result.Items.CreateItem("", prevColumnButton, nextColumnButton);
			navigationColumn.HorizontalAlign = FormLayoutHorizontalAlign.Center;
			navigationColumn.ShowCaption = Utils.DefaultBoolean.False;
			result.ApplyCommonSettings();
			return result;
		}
		DialogFormLayoutBase CreateCellFormLayout() {
			var result = new RichEditDialogFormLayout();
			LayoutGroup sizeGroup = result.Items.Add<LayoutGroup>("", "SizeGroup");
			sizeGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			sizeGroup.ColCount = 3;
			CellPrefWidthCheckBox = sizeGroup.Items.CreateCheckBox("ChkCellPrefWidth", buffer: Editors);
			ASPxSpinEdit cellPrefWidthSpinEdit = sizeGroup.Items.CreateEditor<ASPxSpinEdit>("SpnCellPrefWidth", cssClassName: "dxre-dialogShortEditor", showCaption: false, buffer: Editors);
			cellPrefWidthSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			cellPrefWidthSpinEdit.ClientEnabled = false;
			CellMeasureInComboBox = sizeGroup.Items.CreateComboBox("CbxCellMeasureIn", cssClassName: "dxre-dialogEditor", buffer: Editors);
			CellMeasureInComboBox.ClientEnabled = false;
			LayoutGroup alignmentGroup = result.Items.Add<LayoutGroup>("", "VerticalAlignmentGroup");
			alignmentGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			VerticalAlignmentList = alignmentGroup.Items.CreateEditor<ASPxRadioButtonList>("RblVerticalAligment", buffer: Editors, showCaption: false);
			result.FindItemOrGroupByName("RblVerticalAligment").HorizontalAlign = FormLayoutHorizontalAlign.Left;
			LayoutGroup cellMarginsGroup = CreateMarginsGroup(result, null);
			SameAsTableCheckBox = cellMarginsGroup.Items.CreateCheckBox("ChkSameAsTable", buffer: Editors);
			LayoutGroup optionsGroup = result.Items.Add<LayoutGroup>("", "OptionsGroup");
			optionsGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			WrapTextCheckBox = optionsGroup.Items.CreateCheckBox("ChkWrapText", buffer: Editors);
			LayoutGroup navigationGroup = result.Items.Add<LayoutGroup>("", "NavigationGroup");
			navigationGroup.HorizontalAlign = FormLayoutHorizontalAlign.Center;
			navigationGroup.CssClass = "dxre-dialogCellNavigation";
			navigationGroup.GroupBoxDecoration = GroupBoxDecoration.None;
			navigationGroup.ColCount = 3;
			navigationGroup.SettingsItems.ShowCaption = Utils.DefaultBoolean.False;
			var buttonLeft = CreateDialogButton("BtnMoveLeftCell", ASPxRichEditStringId.Left);
			buttonLeft.ClientVisible = false;
			var buttonUp = CreateDialogButton("BtnMoveTopCell", ASPxRichEditStringId.Top);
			buttonUp.ClientVisible = false;
			var buttonDown = CreateDialogButton("BtnMoveBottomCell", ASPxRichEditStringId.Bottom);
			buttonDown.ClientVisible = false;
			var buttonRight = CreateDialogButton("BtnMoveRightCell", ASPxRichEditStringId.Right);
			buttonRight.ClientVisible = false;
			navigationGroup.Items.CreateItem("", buttonLeft).RowSpan = 2;
			navigationGroup.Items.CreateItem("", buttonUp);
			navigationGroup.Items.CreateItem("", buttonRight).RowSpan = 2;
			navigationGroup.Items.CreateItem("", buttonDown);
			navigationGroup.ClientVisible = false;
			result.ApplyCommonSettings();
			return result;
		}
		LayoutGroup CreateMarginsGroup(RichEditDialogFormLayout result, string namePostfix) {
			LayoutGroup cellMarginsGroup = result.Items.Add<LayoutGroup>("", "CellMarginsGroup");
			cellMarginsGroup.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			cellMarginsGroup.SettingsItems.ShowCaption = Utils.DefaultBoolean.False;
			DialogFormLayoutBase marginsLayout = new RichEditDialogFormLayout();
			cellMarginsGroup.Items.CreateItem("", marginsLayout).HorizontalAlign = FormLayoutHorizontalAlign.Center;		   
			marginsLayout.ColCount = 3;
			marginsLayout.SettingsItems.HorizontalAlign = FormLayoutHorizontalAlign.Center;
			marginsLayout.CssClass = "dxre-dialogCellMargins";
			marginsLayout.ApplyCommonSettings();
			ASPxSpinEdit topMarginSpinEdit = marginsLayout.Items.CreateEditor<ASPxSpinEdit>("SpnTopMargin" + namePostfix, showCaption: false, buffer: Editors, colSpan: 3, cssClassName: "dxre-dialogShortEditor");
			topMarginSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			ASPxSpinEdit leftMarginSpinEdit = marginsLayout.Items.CreateEditor<ASPxSpinEdit>("SpnLeftMargin" + namePostfix, showCaption: false, buffer: Editors, cssClassName: "dxre-dialogShortEditor");
			leftMarginSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			var cellBlock = marginsLayout.Items.Add<EmptyLayoutItem>("CellBlock");
			cellBlock.CssClass = "dxre-dialogCellBlock";
			cellBlock.ShowCaption = Utils.DefaultBoolean.False;
			ASPxSpinEdit rightMarginSpinEdit = marginsLayout.Items.CreateEditor<ASPxSpinEdit>("SpnRightMargin" + namePostfix, showCaption: false, buffer: Editors, cssClassName: "dxre-dialogShortEditor");
			rightMarginSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			ASPxSpinEdit bottomMarginSpinEdit = marginsLayout.Items.CreateEditor<ASPxSpinEdit>("SpnBottomMargin" + namePostfix, showCaption: false, buffer: Editors, colSpan: 3, cssClassName: "dxre-dialogShortEditor");
			bottomMarginSpinEdit.SetupDefaultSettings(0, 0, UnitFormatString);
			return cellMarginsGroup;
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			AlignmentList.Items.Add(XtraRichEdit.Model.TableRowAlignment.Left.ToString(), (int)XtraRichEdit.Model.TableRowAlignment.Left);
			AlignmentList.Items.Add(XtraRichEdit.Model.TableRowAlignment.Center.ToString(), (int)XtraRichEdit.Model.TableRowAlignment.Center);
			AlignmentList.Items.Add(XtraRichEdit.Model.TableRowAlignment.Right.ToString(), (int)XtraRichEdit.Model.TableRowAlignment.Right);
			PrepareRadioButtonList(AlignmentList);
			VerticalAlignmentList.Items.Add(XtraRichEdit.Model.VerticalAlignment.Top.ToString(), (int)XtraRichEdit.Model.VerticalAlignment.Top);
			VerticalAlignmentList.Items.Add(XtraRichEdit.Model.VerticalAlignment.Center.ToString(), (int)XtraRichEdit.Model.VerticalAlignment.Center);
			VerticalAlignmentList.Items.Add(XtraRichEdit.Model.VerticalAlignment.Bottom.ToString(), (int)XtraRichEdit.Model.VerticalAlignment.Bottom);
			PrepareRadioButtonList(VerticalAlignmentList);
			RowHeightComboBox.Items.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_HeightTypeExact), (int)XtraRichEdit.Model.HeightUnitType.Exact);
			RowHeightComboBox.Items.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_HeightTypeMinimum), (int)XtraRichEdit.Model.HeightUnitType.Minimum);
			RowHeightComboBox.ValueType = typeof(int);
			PrepareWidthType(TableMeasureInComboBox, ColumnMeasureInComboBox, CellMeasureInComboBox);
		}
		void PrepareRadioButtonList(ASPxRadioButtonList list) {
			list.RepeatDirection = RepeatDirection.Horizontal;
			list.ValueType = typeof(int);
			list.SelectedIndex = 0;
			list.Border.BorderStyle = BorderStyle.None;
		}
		void PrepareWidthType(params ASPxComboBox[] comboBoxes) {
			foreach(ASPxComboBox comboBox in comboBoxes) {
				comboBox.ValueType = typeof(int);
				comboBox.Items.Add(OfficeLocalizer.GetString(OfficeStringId.Caption_UnitPercent), (int)XtraRichEdit.Model.WidthUnitType.FiftiethsOfPercent);
				comboBox.Items.Add(OfficeLocalizer.GetString(OfficeStringId.Caption_UnitInches), (int)XtraRichEdit.Model.WidthUnitType.ModelUnits);
				comboBox.Value = (int)XtraRichEdit.Model.WidthUnitType.ModelUnits;
			}
		}
		protected override void Localize() {
			PageControl.LocalizeField("0", ASPxRichEditStringId.TableProperties_Table);
			PageControl.LocalizeField("1", ASPxRichEditStringId.TableProperties_Row);
			PageControl.LocalizeField("2", ASPxRichEditStringId.TableProperties_Column);
			PageControl.LocalizeField("3", ASPxRichEditStringId.TableProperties_Cell);
			TablePrefWidthCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.TableProperties_PreferredWidth);
			TableFormLayout.LocalizeField("SizeGroup", ASPxRichEditStringId.TableProperties_Size);
			TableFormLayout.LocalizeField("AlignmentGroup", ASPxRichEditStringId.TableProperties_Alignment);
			TableFormLayout.LocalizeField("CbxTableMeasureIn", ASPxRichEditStringId.TableProperties_MeasureIn);
			TableFormLayout.LocalizeField("SpnIndentLeft", ASPxRichEditStringId.TableProperties_IndentLeft);
			TableFormLayout.LocalizeField("CellMarginsGroup", ASPxRichEditStringId.TableOptions_DefaultCellMargins);
			AllowSpacingCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.TableOptions_AllowSpacing);
			TableFormLayout.LocalizeField("OptionsGroup", ASPxRichEditStringId.Options);
			AutoResizeCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.TableOptions_AutoResize);
			SpecifyHeightCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.TableProperties_SpecifyHeight);
			RowFormLayout.LocalizeField("SizeGroup", ASPxRichEditStringId.TableProperties_Size);
			RowFormLayout.LocalizeField("CbxRowHeight", ASPxRichEditStringId.TableProperties_RowHeight);
			ColumnFormLayout.LocalizeField("SizeGroup", ASPxRichEditStringId.TableProperties_Size);
			ColumnPrefWidthCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.TableProperties_PreferredWidth);
			ColumnFormLayout.LocalizeField("CbxColumnMeasureIn", ASPxRichEditStringId.TableProperties_MeasureIn);
			CellPrefWidthCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.TableProperties_PreferredWidth);
			SameAsTableCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CellOptions_SameAsTable);
			CellFormLayout.LocalizeField("SizeGroup", ASPxRichEditStringId.TableProperties_Size);
			CellFormLayout.LocalizeField("VerticalAlignmentGroup", ASPxRichEditStringId.TableProperties_VerticalAlignment);
			CellFormLayout.LocalizeField("OptionsGroup", ASPxRichEditStringId.Options);
			CellFormLayout.LocalizeField("CellMarginsGroup", ASPxRichEditStringId.CellOptions_CellMargins);
			CellFormLayout.LocalizeField("CbxCellMeasureIn", ASPxRichEditStringId.TableProperties_MeasureIn);
			WrapTextCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CellOptions_WrapText);
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgTablePropertiesForm";
		}
	}
}

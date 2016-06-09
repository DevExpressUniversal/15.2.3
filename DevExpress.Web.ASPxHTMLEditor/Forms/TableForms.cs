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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Forms {
	public abstract class TableDialogBase : HtmlEditorDialogBase {
		protected string AutomaticColorValue { get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AutomaticColorCaption); } }
		protected ASPxComboBox WidthType { get; private set; }
		protected ASPxComboBox HeightType { get; private set; }
		protected ASPxSpinEdit WidthValue { get; private set; }
		protected ASPxSpinEdit HeightValue { get; private set; }
		protected ASPxComboBox WidthValueType { get; private set; }
		protected ASPxComboBox HeightValueType { get; private set; }
		protected ASPxColorEdit BackgroundColor { get; private set; }
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			MainFormLayout.AlignItemCaptionsInAllGroups = true;
			MainFormLayout.SettingsItemCaptions.HorizontalAlign = FormLayoutHorizontalAlign.Right;
			if(BackgroundColor != null) {
				RenderUtils.AppendDefaultDXClassName(BackgroundColor, "dxhe-tableDialogBGColor");
				BackgroundColor.AutomaticColor = Color.Empty;
				BackgroundColor.NullTextStyle.CssClass = "dxhe-tableDialogNullText";
				BackgroundColor.EnableAutomaticColorItem = true;
				BackgroundColor.AutomaticColorItemValue = AutomaticColorValue;
				BackgroundColor.AutomaticColorItemCaption = AutomaticColorValue;
				BackgroundColor.NullText = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.TableElementInconsistentColor);
			}
		}
		protected void CreateBackgroundColor(LayoutGroup group) {
			BackgroundColor = group.Items.CreateEditor<ASPxColorEdit>("BackgroundColor", cssClassName: "dxhe-dialogMiddleEditor");
		}
		protected void CreateHeightElements(LayoutGroup sizeGroup) {
			HeightType = sizeGroup.Items.CreateComboBox("HeightType", cssClassName: "dxhe-dialogLargeEditor", buffer: Editors);
			HeightValue = sizeGroup.Items.CreateSpinEdit("HeightValue", cssClassName: "dxhe-dialogSmallEditor", showButtons: true, buffer: Editors);
			HeightValueType = sizeGroup.Items.CreateComboBox("HeightValueType", cssClassName: "dxhe-dialogSmallEditor", buffer: Editors);
		}
		protected void CreateWidthElements(LayoutGroup sizeGroup) {
			WidthType = sizeGroup.Items.CreateComboBox("WidthType", cssClassName: "dxhe-dialogLargeEditor", buffer: Editors);
			WidthValue = sizeGroup.Items.CreateSpinEdit("WidthValue", cssClassName: "dxhe-dialogSmallEditor", showButtons: true, buffer: Editors);
			WidthValueType = sizeGroup.Items.CreateComboBox("WidthValueType", cssClassName: "dxhe-dialogSmallEditor", buffer: Editors);
		}
		protected void PrepareWidthElements() {
			PrepareSizeElement(WidthValue, WidthValueType, WidthType, "Width", ASPxHtmlEditorStringId.InsertTable_Width, true);
		}
		protected void PrepareHeightElement() {
			PrepareSizeElement(HeightValue, HeightValueType, HeightType, "Height", ASPxHtmlEditorStringId.InsertTable_Height, false);
		}
		void PrepareSizeElement(ASPxSpinEdit value, ASPxComboBox valueType, ASPxComboBox type, string key, ASPxHtmlEditorStringId caption, bool hasFullWidthItem) {
			MainFormLayout.LocalizeField(key + "Type", caption);
			MainFormLayout.FindItemOrGroupByName(key + "Value").ShowCaption = DefaultBoolean.False;
			MainFormLayout.FindItemOrGroupByName(key + "ValueType").ShowCaption = DefaultBoolean.False;
			value.Value = 0;
			value.ClientVisible = false;
			valueType.Items.Add("px").Selected = true;
			valueType.Items.Add("%");
			valueType.ClientVisible = false;
			if(hasFullWidthItem)
				type.AddItem(ASPxHtmlEditorStringId.InsertTable_FullWidth, "100%").Selected = true;
			type.AddItem(ASPxHtmlEditorStringId.InsertTable_AutoFitToContent, "").Selected = !hasFullWidthItem;
			type.AddItem(ASPxHtmlEditorStringId.InsertTable_Custom, "custom");
		}
		ASPxSpinEdit CreateSizeSpinEdit(string name) {
			var result = new ASPxSpinEdit();
			result.ClientInstanceName = GetClientInstanceName(name);
			result.CssClass = "dxhe-dialogSmallEditor";
			result.ClientVisible = false;
			result.SpinButtons.ShowIncrementButtons = true;
			return result;
		}
		ASPxComboBox CreateSizeComboBox(string name, string className, bool clientVisible) {
			var result = new ASPxComboBox();
			result.ClientInstanceName = GetClientInstanceName(name);
			result.CssClass = className;
			result.ClientVisible = clientVisible;
			return result;
		}
	}
	public class InsertTableForm : TableDialogBase {
		protected ASPxSpinEdit ColumnCount { get; private set; }
		protected ASPxSpinEdit RowCount { get; private set; }
		protected ASPxCheckBox EqualWidth { get; private set; }
		protected ASPxSpinEdit CellPadding { get; private set; }
		protected ASPxSpinEdit CellSpacing { get; private set; }
		protected ASPxComboBox Alignment { get; private set; }
		protected ASPxColorEdit BorderColor { get; private set; }
		protected ASPxSpinEdit BorderWidth { get; private set; }
		protected ASPxCheckBox Accessibility { get; private set; }
		protected ASPxComboBox Headers { get; private set; }
		protected ASPxTextBox Caption { get; private set; }
		protected ASPxTextBox Summary { get; private set; }
		protected override ITemplate GetTopAreaTemplate() {
			return HtmlEditor.SettingsDialogs.InsertTableDialog.TopAreaTemplate;
		}
		protected override ITemplate GetBottomAreaTemplate() {
			return HtmlEditor.SettingsDialogs.InsertTableDialog.BottomAreaTemplate;
		}
		protected override void PopulateContentGroup(LayoutGroup group) {
			PopulateSizeGroup(group.Items.Add<LayoutGroup>("SizeGroup", "SizeGroup"));
			if(HtmlEditor.SettingsDialogs.InsertTableDialog.ShowLayoutSection) {
				LayoutGroup layoutGroup = group.Items.Add<LayoutGroup>("LayoutGroup", "LayoutGroup");
				layoutGroup.ColCount = 2;
				CellPadding = layoutGroup.Items.CreateSpinEdit("CellPadding", showButtons: true, cssClassName: "dxhe-dialogSmallEditor");
				Alignment = layoutGroup.Items.CreateComboBox("Alignment", cssClassName: "dxhe-dialogMiddleEditor");
				CellSpacing = layoutGroup.Items.CreateSpinEdit("CellSpacing", showButtons: true, cssClassName: "dxhe-dialogSmallEditor");
			}
			if(HtmlEditor.SettingsDialogs.InsertTableDialog.ShowAppearanceSection) {
				LayoutGroup appearanceGroup = group.Items.Add<LayoutGroup>("AppearanceGroup", "AppearanceGroup");
				appearanceGroup.ColCount = 2;
				BorderColor = appearanceGroup.Items.CreateEditor<ASPxColorEdit>("BorderColor", cssClassName: "dxhe-dialogMiddleEditor");
				BorderWidth = appearanceGroup.Items.CreateSpinEdit("BorderWidth", showButtons: true, cssClassName: "dxhe-dialogSmallEditor");
				CreateBackgroundColor(appearanceGroup);
			}
			if(HtmlEditor.SettingsDialogs.InsertTableDialog.ShowAccessibilitySection) {
				LayoutGroup accessibilityGroup = group.Items.Add<LayoutGroup>("AccessibilityGroup", "AccessibilityGroup");
				accessibilityGroup.ClientVisible = false;
				Headers = accessibilityGroup.Items.CreateComboBox("Headers", cssClassName: "dxhe-dialogMiddleEditor");
				Caption = accessibilityGroup.Items.CreateTextBox("Caption");
				Summary = accessibilityGroup.Items.CreateTextBox("Summary");
			}
		}
		protected override void PopulateBottomItemControls(List<Control> controls) {
			base.PopulateBottomItemControls(controls);
			if(HtmlEditor.SettingsDialogs.InsertTableDialog.ShowAccessibilitySection) {
				Accessibility = new ASPxCheckBox();
				Accessibility.ClientInstanceName = GetClientInstanceName("Accessibility");
				controls.Insert(0, Accessibility);
			}
		}
		protected virtual void PopulateSizeGroup(LayoutGroup sizeGroup) {
			sizeGroup.ColCount = 3;
			ColumnCount = sizeGroup.Items.CreateSpinEdit("ColumnCount", showButtons: true, cssClassName: "dxhe-dialogSmallEditor", colSpan: 3);
			RowCount = sizeGroup.Items.CreateSpinEdit("RowCount", showButtons: true, cssClassName: "dxhe-dialogSmallEditor", colSpan: 3);
			CreateWidthElements(sizeGroup);
			EqualWidth = sizeGroup.Items.CreateCheckBox("EqualWidth", showCaption: true, colSpan: sizeGroup.ColCount);
			CreateHeightElements(sizeGroup);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			PrepareColorEditControls(BorderColor, BackgroundColor);
			PrepareSizeGroupItems();
			if(HtmlEditor.SettingsDialogs.InsertTableDialog.ShowLayoutSection)
				PrepareLayoutSection();
			if(HtmlEditor.SettingsDialogs.InsertTableDialog.ShowAppearanceSection)
				PrepareAppearanceSection();
			if(HtmlEditor.SettingsDialogs.InsertTableDialog.ShowAccessibilitySection)
				PrepareAccessibilitySection();
		}
		protected virtual void PrepareAccessibilitySection() {
			Accessibility.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTable_Accessibility);
			MainFormLayout.LocalizeField("AccessibilityGroup", ASPxHtmlEditorStringId.InsertTable_Accessibility);
			MainFormLayout.LocalizeField("Headers", ASPxHtmlEditorStringId.InsertTable_Headers);
			MainFormLayout.LocalizeField("Caption", ASPxHtmlEditorStringId.InsertTable_Caption);
			MainFormLayout.LocalizeField("Summary", ASPxHtmlEditorStringId.InsertTable_Summary);
			Headers.AddItem(ASPxHtmlEditorStringId.InsertTable_None, "").Selected = true;
			Headers.AddItem(ASPxHtmlEditorStringId.InsertTable_FirstRow, "row");
			Headers.AddItem(ASPxHtmlEditorStringId.InsertTable_FirstColumn, "column");
			Headers.AddItem(ASPxHtmlEditorStringId.InsertTable_Both, "both");
		}
		protected virtual void PrepareAppearanceSection() {
			MainFormLayout.LocalizeField("AppearanceGroup", ASPxHtmlEditorStringId.InsertTable_Appearance);
			MainFormLayout.LocalizeField("BorderColor", ASPxHtmlEditorStringId.InsertTable_BorderColor);
			MainFormLayout.LocalizeField("BackgroundColor", ASPxHtmlEditorStringId.InsertTable_BgColor);
			MainFormLayout.LocalizeField("BorderWidth", ASPxHtmlEditorStringId.InsertTable_BorderSize);
			BorderWidth.Value = 1;
			BorderColor.Color = System.Drawing.Color.Black;
		}
		protected virtual void PrepareLayoutSection() {
			MainFormLayout.LocalizeField("LayoutGroup", ASPxHtmlEditorStringId.InsertTable_Layout);
			MainFormLayout.LocalizeField("CellPadding", ASPxHtmlEditorStringId.InsertTable_CellPaddings);
			MainFormLayout.LocalizeField("Alignment", ASPxHtmlEditorStringId.InsertTable_Alignment);
			MainFormLayout.LocalizeField("CellSpacing", ASPxHtmlEditorStringId.InsertTable_CellSpacing);
			CellPadding.Value = 3;
			CellSpacing.Value = 0;
			Alignment.AddItem(ASPxHtmlEditorStringId.InsertTable_None, "none").Selected = true;
			Alignment.AddItem(ASPxHtmlEditorStringId.InsertTable_Alignment_Left, "left");
			Alignment.AddItem(ASPxHtmlEditorStringId.InsertTable_Alignment_Center, "center");
			Alignment.AddItem(ASPxHtmlEditorStringId.InsertTable_Alignment_Right, "right");
		}
		protected virtual void PrepareSizeGroupItems() {
			PrepareWidthElements();
			PrepareHeightElement();
			ColumnCount.MinValue = 1;
			ColumnCount.Value = 2;
			RowCount.MinValue = 1;
			RowCount.Value = 2;
			MainFormLayout.FindItemOrGroupByName("ColumnCount").ParentContainerStyle.CssClass = "dxhe-minimizedTD";
			MainFormLayout.LocalizeField("SizeGroup", ASPxHtmlEditorStringId.InsertTable_Size);
			MainFormLayout.LocalizeField("ColumnCount", ASPxHtmlEditorStringId.InsertTable_Columns);
			MainFormLayout.LocalizeField("RowCount", ASPxHtmlEditorStringId.InsertTable_Rows);
			EqualWidth.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTable_EqualColumnWidths);
			EqualWidth.Checked = true;
		}
		protected override string GetDialogCssClassName() {
			return "dxhe-insertTableDialog";
		}
	}
	public abstract class TableElementPropertiesFormBase : TableDialogBase {
		protected abstract bool HasAligmentSection { get; }
		protected bool HasSizeSection { get { return HasWidth || HasHeight; } }
		protected abstract bool HasAppearanceSection { get; }
		protected abstract bool HasWidth { get; }
		protected abstract bool HasHeight { get; }
		protected ASPxComboBox HorizontalAlignment { get; private set; }
		protected ASPxComboBox VerticalAlignment { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			if(HasSizeSection)
				PopulateSizeGroup(group.Items.Add<LayoutGroup>("SizeGroup", "SizeGroup"));
			if(HasAligmentSection)
				PopulateAligmenGroup(group.Items.Add<LayoutGroup>("AlignmentGroup", "AlignmentGroup"));
			if(HasAppearanceSection)
				PopulateAppearanceGroup(group.Items.Add<LayoutGroup>("AppearanceGroup", "AppearanceGroup"));
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			PrepareColorEditControls(BackgroundColor);
			if(HasSizeSection)
				PrepareSizeGroupItems();
			if(HasAligmentSection)
				PrepareAligmentGroupItems();
			if(HasAppearanceSection) {
				MainFormLayout.LocalizeField("AppearanceGroup", ASPxHtmlEditorStringId.InsertTable_Appearance);
				MainFormLayout.LocalizeField("BackgroundColor", ASPxHtmlEditorStringId.InsertTable_BgColor);
			}
		}
		protected virtual void PrepareAligmentGroupItems() {
			MainFormLayout.LocalizeField("AlignmentGroup", ASPxHtmlEditorStringId.InsertTable_Alignment);
			MainFormLayout.LocalizeField("HorizontalAlignment", ASPxHtmlEditorStringId.InsertTable_HorzAlignment);
			MainFormLayout.LocalizeField("VerticalAlignment", ASPxHtmlEditorStringId.InsertTable_VertAlignment);
			HorizontalAlignment.AddItem(ASPxHtmlEditorStringId.InsertTable_None, "").Selected = true;
			HorizontalAlignment.AddItem(ASPxHtmlEditorStringId.InsertTable_Alignment_Left, "left");
			HorizontalAlignment.AddItem(ASPxHtmlEditorStringId.InsertTable_Alignment_Center, "center");
			HorizontalAlignment.AddItem(ASPxHtmlEditorStringId.InsertTable_Alignment_Right, "right");
			VerticalAlignment.AddItem(ASPxHtmlEditorStringId.InsertTable_None, "").Selected = true;
			VerticalAlignment.AddItem(ASPxHtmlEditorStringId.InsertTable_VAlignment_Top, "top");
			VerticalAlignment.AddItem(ASPxHtmlEditorStringId.InsertTable_VAlignment_Middle, "middle");
			VerticalAlignment.AddItem(ASPxHtmlEditorStringId.InsertTable_VAlignment_Bottom, "bottom");
		}
		protected virtual void PrepareSizeGroupItems() {
			MainFormLayout.LocalizeField("SizeGroup", ASPxHtmlEditorStringId.ChangeTableColumn_Size);
			if(HasWidth)
				PrepareWidthElements();
			if(HasHeight)
				PrepareHeightElement();
		}
		protected virtual void PopulateAppearanceGroup(LayoutGroup layoutGroup) {
			CreateBackgroundColor(layoutGroup);
		}
		protected virtual void PopulateAligmenGroup(LayoutGroup layoutGroup) {
			HorizontalAlignment = layoutGroup.Items.CreateComboBox("HorizontalAlignment", cssClassName: "dxhe-dialogMiddleEditor");
			VerticalAlignment = layoutGroup.Items.CreateComboBox("VerticalAlignment", cssClassName: "dxhe-dialogMiddleEditor");
		}
		protected virtual void PopulateSizeGroup(LayoutGroup sizeGroup) {
			sizeGroup.ColCount = 3;
			if(HasWidth)
				CreateWidthElements(sizeGroup);
			if(HasHeight)
				CreateHeightElements(sizeGroup);
		}
		protected override string GetDialogCssClassName() {
			return "dxhe-tableColumnDialog";
		}
	}
	public class TableColumnPropertiesForm : TableElementPropertiesFormBase {
		protected override bool HasWidth {
			get { return true; }
		}
		protected override bool HasHeight {
			get { return false; }
		}
		protected override bool HasAligmentSection {
			get { return HtmlEditor.SettingsDialogs.TableColumnPropertiesDialog.ShowAlignmentSection; }
		}
		protected override bool HasAppearanceSection {
			get { return HtmlEditor.SettingsDialogs.TableColumnPropertiesDialog.ShowAppearanceSection; }
		}
	}
	public class TableCellPropertiesForm : TableElementPropertiesFormBase {
		protected override bool HasWidth {
			get { return false; }
		}
		protected override bool HasHeight {
			get { return false; }
		}
		protected override bool HasAligmentSection {
			get { return HtmlEditor.SettingsDialogs.TableCellPropertiesDialog.ShowAlignmentSection; }
		}
		protected override bool HasAppearanceSection {
			get { return HtmlEditor.SettingsDialogs.TableCellPropertiesDialog.ShowAppearanceSection; }
		}
		protected ASPxCheckBox ApplyToAllCells { get; private set; }
		protected override void PopulateBottomItemControls(List<Control> controls) {
			base.PopulateBottomItemControls(controls);
			if(HtmlEditor.SettingsDialogs.TableCellPropertiesDialog.ShowApplyToAllCellsButton) {
				ApplyToAllCells = new ASPxCheckBox();
				ApplyToAllCells.ClientInstanceName = GetClientInstanceName("ApplyToAllCells");
				controls.Insert(0, ApplyToAllCells);
			}
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			if(HtmlEditor.SettingsDialogs.TableCellPropertiesDialog.ShowApplyToAllCellsButton)
				ApplyToAllCells.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTable_ApplyToAllCell);
		}
	}
	public class TableRowPropertiesForm : TableElementPropertiesFormBase {
		protected override bool HasWidth {
			get { return false; }
		}
		protected override bool HasHeight {
			get { return true; }
		}
		protected override bool HasAligmentSection {
			get { return HtmlEditor.SettingsDialogs.TableRowPropertiesDialog.ShowAlignmentSection; }
		}
		protected override bool HasAppearanceSection {
			get { return HtmlEditor.SettingsDialogs.TableRowPropertiesDialog.ShowAppearanceSection; }
		}
	}
}

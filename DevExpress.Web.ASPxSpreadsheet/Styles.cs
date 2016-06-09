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
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SpreadsheetStyles : StylesBase {
		public SpreadsheetStyles(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) { }
		internal const string CssClassPrefix = "dxss";
		protected internal const string
			MainElementCssClass = CssClassPrefix + "ControlSys",
			TabControlCssClass = "dxss-tcer",
			TabControlSampleStrip = "dxtc-wrapper",
			InputTargetCssClass = CssClassPrefix + "-inputTarget";
		public const string DialogChartButton = CssClassPrefix + "-chartsbtn";
		public const string PresetChartContainer = CssClassPrefix + "-chartPresetContainer";
		public const string TableStyleButton = CssClassPrefix + "-tablesbtn";
		#region Dialogs
		public const string DialogContentCellCssClass = CssClassPrefix + "DlgContentCell",
							DialogFooterCellCssClass = CssClassPrefix + "DlgFooter",
							DialogCaptionCellCssClass = CssClassPrefix + "DlgCaptionCell",
							DialogInputCellCssClass = CssClassPrefix + "DlgInputCell",
							DialogFooterButtonCssClass = CssClassPrefix + "DlgFooterBtn",
							DialogRoundPanelContentCssClass = CssClassPrefix + "DlgRoundPanelContent",
							DialogSeparatorCellCssClass = CssClassPrefix + "DlgSeparatorCell",
							DialogRadioButtonCssClass = CssClassPrefix + "DlgRadionButtonList",
							DialogContentTableCssClass = CssClassPrefix + "DlgContentTable";
		public const string RenameSheetDialogCssClass = CssClassPrefix + "DlgRenameSheetForm",
							RowHeightDialogCssClass = CssClassPrefix + "DlgRowHeightForm",
							ColumnWidthDialogCssClass = CssClassPrefix + "DlgColumnWidthForm",
							DefaultColumnWidthDialogCssClass = CssClassPrefix + "DlgDefaultColumnWidthForm",
							MoveOrCopySheetDialogCssClass = CssClassPrefix + "DlgMoveOrCopySheetForm",
							ChartSelectDataDialogCssClass = CssClassPrefix + "DlgChartSelectData",
							ModifyChartLayoutDialogCssClass = CssClassPrefix + "DlgModifyChartLayoutForm",
							ModifyChartLayoutPanelCssClass = CssClassPrefix + "ModifyChartLayoutContenPanel",
							ChartChangeTitleDialogCssClass = CssClassPrefix + "DlgChartChangeTitleForm",
							ChangeChartTypeDialogCssClass = CssClassPrefix + "DlgChangeChartType",
							DialogPageControlCssClass = CssClassPrefix + "DlgPageControl",
							ModifyChartStyleDialogCssClass = CssClassPrefix + "DlgModifyChartStyleForm",
							ModifyChartStylePanelCssClass = CssClassPrefix + "ModifyChartStyleContenPanel",
							CustomDataFilterDialogCssClass = CssClassPrefix + "DlgCustomDataFilterForm",
							DataFilterSimpleDialogCssClass = CssClassPrefix + "DlgDataFilterSimpleForm",
							Top10FilterDialogCssClass = CssClassPrefix + "DlgTop10FilterForm",
							DataValidationDialogCssClass = CssClassPrefix + "DlgDataValidationForm",
							ValidationConfirmDialogCssClass = CssClassPrefix + "DlgValidationConfirmForm",
							UnhideSheetDialogCssClass = CssClassPrefix + "DlgUnhideSheetForm",
							ChartChangeVerticalAxisTitleDialogCssClass = CssClassPrefix + "DlgChartChangeVerticalAxisTitleForm";
		#endregion
		public override string ToString() {
			return string.Empty;
		}
		protected override string GetCssClassNamePrefix() {
			return CssClassPrefix;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
		}
		protected override AppearanceStyle GetDefaultControlStyle() {
			AppearanceStyle ret = new AppearanceStyle();
			ret.CopyFrom(base.GetDefaultControlStyle());
			ret.Width = Unit.Pixel(700);
			ret.Height = Unit.Pixel(500);
			return ret;
		}
	}
	public class SpreadsheetTabControlStyles : TabControlStyles {
		public SpreadsheetTabControlStyles(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) { }
	}
	public class SpreadsheetRibbonStyles : RibbonStyles {
		public SpreadsheetRibbonStyles(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) { }
	}
	public class SpreadsheetMenuStyles : MenuStyles {
		public SpreadsheetMenuStyles(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
		public override string ToString() { return string.Empty; }
	}
	public class SpreadsheetButtonStyles : ButtonControlStyles {
		public SpreadsheetButtonStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		public override string ToString() { return string.Empty; }
	}
	public class SpreadsheetEditorsStyles : EditorStyles {
		public SpreadsheetEditorsStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		public override string ToString() { return string.Empty; }
	}
	public class SpreadsheetDialogFormStylesLite : PopupControlStyles {
		public SpreadsheetDialogFormStylesLite(ISkinOwner owner)
			: base(owner) {
		}
		protected override PopupControlModalBackgroundStyle GetDefaultModalBackgroundStyle() {
			PopupControlModalBackgroundStyle style = new PopupControlModalBackgroundStyle();
			style.CopyFrom(CreateStyleByName("ModalBackLite"));
			style.Opacity = 1;
			return style;
		}
		protected override PopupWindowContentStyle GetDefaultContentStyle() {
			PopupWindowContentStyle style = base.GetDefaultContentStyle();
			style.Paddings.Assign(new Paddings(Unit.Pixel(0)));
			return style;
		}
	}
	public class SpreadsheetFileManagerStyles : DevExpress.Web.FileManagerStyles {
		const string ControlStyleName = "ControlStyle";
		public SpreadsheetFileManagerStyles(ISkinOwner owner)
			: base(owner) {
		}
		[Browsable(false)]
		public new string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyleBase Control {
			get { return GetStyle(ControlStyleName); }
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ControlStyleName, delegate() { return new AppearanceStyleBase(); }));
		}
	}
	public class SpreadsheetFormulaBarStyles : StylesBase {
		const string MainElementSystemClassName = SpreadsheetStyles.CssClassPrefix + "-fb";
		const string ButtonSectionSystemClassName = SpreadsheetStyles.CssClassPrefix + "-fbMenu";
		const string MainElementStyleName = "Style";
		const string TextBoxStyleName = "TextBoxStyle";
		const string EnterButtonStyleName = "EnterButtonStyle";
		const string CancelButtonStyleName = "CancelButtonStyle";
		SpreadsheetFormulaBarButtonSectionStyles buttonSectionStyles;
		public SpreadsheetFormulaBarStyles(ISkinOwner owner)
			: base(owner) {
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetFormulaBarButtonSectionStyles ButtonSection {
			get { return this.buttonSectionStyles = this.buttonSectionStyles ?? new SpreadsheetFormulaBarButtonSectionStyles(Owner as ISkinOwner); }
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle EnterButton {
			get { return (AppearanceStyle)GetStyle(EnterButtonStyleName); }
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle CancelButton {
			get { return (AppearanceStyle)GetStyle(CancelButtonStyleName); }
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle TextBox {
			get { return (AppearanceStyle)GetStyle(TextBoxStyleName); }
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle MainElement {
			get { return (AppearanceStyle)GetStyle(MainElementStyleName); }
		}
		protected internal AppearanceStyle GetMainElementStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(MainElement);
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, MainElementSystemClassName);
			return style;
		}
		protected internal SpreadsheetFormulaBarButtonSectionStyles GetButtonSectionStyles() {
			SpreadsheetFormulaBarButtonSectionStyles styles = new SpreadsheetFormulaBarButtonSectionStyles(Owner as ISkinOwner);
			styles.Assign(ButtonSection);
			styles.MainElement.CssClass = RenderUtils.CombineCssClasses(styles.MainElement.CssClass, ButtonSectionSystemClassName);
			return styles;
		}
		protected internal AppearanceStyle GetEnterButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(EnterButton);
			return style;
		}
		protected internal AppearanceStyle GetCancelButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CancelButton);
			return style;
		}
		protected internal AppearanceStyle GetTextBoxStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(TextBox);
			return style;
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			SpreadsheetFormulaBarStyles formulaBarStyles = source as SpreadsheetFormulaBarStyles;
			if(formulaBarStyles != null)
				ButtonSection.CopyFrom(formulaBarStyles.ButtonSection);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ButtonSection });
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(MainElementStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(TextBoxStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(EnterButtonStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(CancelButtonStyleName, delegate() { return new AppearanceStyle(); }));
		}
	}
	public class SpreadsheetFormulaBarButtonSectionStyles : MenuStyles {
		const string ButtonStyleName = "ButtonStyle";
		const string MainElementStyleName = "ButtonSectionStyle";
		public SpreadsheetFormulaBarButtonSectionStyles(ISkinOwner owner)
			: base(owner) {
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle Button {
			get { return (AppearanceStyle)GetStyle(ButtonStyleName); }
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle MainElement {
			get { return (AppearanceStyle)GetStyle(MainElementStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new MenuItemStyle Item {
			get { return base.Item; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new MenuStyle Style {
			get { return base.Style; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LinkStyle Link {
			get { return base.Link; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new MenuScrollButtonStyle ScrollButton {
			get { return base.ScrollButton; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new MenuItemStyle SubMenuItem {
			get { return base.SubMenuItem; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new MenuStyle SubMenu {
			get { return base.SubMenu; }
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ButtonStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(MainElementStyleName, delegate() { return new AppearanceStyle(); }));
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			SpreadsheetFormulaBarButtonSectionStyles buttonSectionStyles = source as SpreadsheetFormulaBarButtonSectionStyles;
			if(buttonSectionStyles != null) {
				Button.CopyFrom(buttonSectionStyles.Button);
				MainElement.CopyFrom(buttonSectionStyles.MainElement);
			}
		}
	}
	public class SpreadsheetFormulaAutoCompeteStyles : StylesBase {
		const string ListBoxStyleName = "ListBoxStyle";
		const string ListBoxItemStyleName = "ListBoxItemStyle";
		const string FunctionInfoStyleName = "FunctionInfoStyle";
		const string FunctionArgumentInfoStyleName = "FunctionArgumentInfoStyle";
		const string FunctionInfoSystemClassName = SpreadsheetStyles.CssClassPrefix + "-funcInfo";
		public SpreadsheetFormulaAutoCompeteStyles(ISkinOwner owner)
			: base(owner) {
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle ListBox {
			get { return (AppearanceStyle)GetStyle(ListBoxStyleName); }
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ListBoxItemStyle ListBoxItem {
			get { return (ListBoxItemStyle)GetStyle(ListBoxItemStyleName); }
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle FunctionInfo {
			get { return (AppearanceStyle)GetStyle(FunctionInfoStyleName); }
		}
		[
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle FunctionArgumentInfo {
			get { return (AppearanceStyle)GetStyle(FunctionArgumentInfoStyleName); }
		}
		protected internal AppearanceStyle GetListBoxStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(ListBox);
			return style;
		}
		protected internal ListBoxItemStyle GetListBoxItemStyle() {
			ListBoxItemStyle style = new ListBoxItemStyle();
			style.CopyFrom(ListBoxItem);
			return style;
		}
		protected internal AppearanceStyle GetFunctionInfoStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(FunctionInfo);
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, FunctionInfoSystemClassName);
			return style;
		}
		protected internal AppearanceStyle GetFunctionArgumentInfoStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(FunctionArgumentInfo);
			return style;
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ListBoxStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(ListBoxItemStyleName, delegate() { return new ListBoxItemStyle(); }));
			list.Add(new StyleInfo(FunctionInfoStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(FunctionArgumentInfoStyleName, delegate() { return new AppearanceStyle(); }));
		}
	}
}

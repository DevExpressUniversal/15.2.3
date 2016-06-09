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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.DropDownEdit;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false), Themeable(false)]
	internal sealed class DropDownPopupControl : ASPxPopupControl {
		private bool adjustInnerControlsSizeOnShow = false;
		public DropDownPopupControl(bool adjustInnerControlsSizeOnShow)
			: base(null) {
			this.adjustInnerControlsSizeOnShow = adjustInnerControlsSizeOnShow;
		}
		protected override bool AdjustInnerControlsSizeOnShow {
			get { return adjustInnerControlsSizeOnShow; }
		}
		protected internal override bool IsCallBacksEnabled() {
			return false;
		}
		public override void RegisterStyleSheets() {
		}
		protected override bool IsSSLSecureBlankUrlRequired() {
			return false;
		}
		protected override bool LoadWindowsState(string state) {
			return false;
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			base.PrepareControlStyle(style);
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, DropDownPopupControlStyles.DropDownPopupControlSystemClassName);
		}
		protected override string GetCssFilePath() {
			return string.Empty;
		}
		protected override string GetImageFolder() {
			return string.Empty;
		}
		protected override string GetSpriteImageUrl() {
			return string.Empty;
		}
		protected override string GetSpriteCssFilePath() {
			return string.Empty;
		}
		protected override StylesBase CreateStyles() {
			return new DropDownPopupControlStyles(this);
		}
		protected internal override Unit GetDefaultWindowWidth() {
			return Unit.Pixel(0);
		}
	}
	public class DropDownPopupControlStyles : DevExpress.Web.PopupControlStyles {
		internal const string DropDownPopupControlSystemClassName = "dxpc-ddSys";
		public DropDownPopupControlStyles(ISkinOwner popupControl)
			: base(popupControl) {
		}
		protected internal override AppearanceStyle GetDefaultControlStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("DropDown"));
			return style;
		}
	}
	public abstract class DropDownControlBase : ButtonEditControl {
		private DropDownPopupControl popupControl = null;
		private string clickHandler = "";
		private string mainCellMouseDownHandler = "";
		private DropDownButton dropDownButton = null;
		private string dropDownHandler = "";
		private AnimationType animationType = AnimationType.Auto;
		private string popupControlId = "";
		private bool showShadow = true;
		private bool readOnly = false;
		private TableCell itemImageCell = null;
		internal DropDownPopupControl PopupControl {
			get { return popupControl; }
		}
		public DropDownControlBase(ASPxDropDownEditBase edit)
			: base(edit) {
		}
		public string ClickHandler {
			get { return clickHandler; }
			set { clickHandler = value; }
		}
		public string MainCellMouseDownHandler {
			get { return mainCellMouseDownHandler; }
			set { mainCellMouseDownHandler = value; }
		}
		public DropDownButton DropDownButton {
			get { return dropDownButton; }
			set { dropDownButton = value; }
		}
		public string DropDownHandler {
			get { return dropDownHandler; }
			set { dropDownHandler = value; }
		}
		public AnimationType AnimationType {
			get { return animationType; }
			set { animationType = value; }
		}
		public string PopupControlId {
			get { return popupControlId; }
			set { popupControlId = value; }
		}
		public bool ShowShadow {
			get { return showShadow; }
			set { showShadow = value; }
		}
		public bool ReadOnly {
			get { return readOnly; }
			set { readOnly = value; }
		}
		protected TableCell ItemImageCell {
			get { return itemImageCell; }
		}
		protected virtual bool AdjustInnerControlsSizeOnShow {
			get { return false; }
		}
		protected new ASPxDropDownEditBase Edit {
			get { return (ASPxDropDownEditBase)base.Edit; }
		}
		protected virtual bool EnableDropDownPopupControlViewState {
			get { return false; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.popupControl = null;
			this.itemImageCell = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if (Edit.NeedPopupControl) {
				this.popupControl = new DropDownPopupControl(AdjustInnerControlsSizeOnShow);
				this.popupControl.ID = PopupControlId;
				this.popupControl.ParentSkinOwner = Edit;
				this.popupControl.CloseAction = CloseAction.CloseButton;
				this.popupControl.ClientSideEvents.Shown = Edit.GetPopupControlOnShown();
				this.popupControl.EnableViewState = EnableDropDownPopupControlViewState;
				this.popupControl.PopupHorizontalAlign = GetEffectivePopupHorizontalAlign();
				this.popupControl.PopupVerticalAlign = Edit.PopupVerticalAlign;
				this.popupControl.RenderIFrameForPopupElements = Edit.RenderIFrameForPopupElements;
				this.popupControl.PopupAnimationType = AnimationType;
				this.popupControl.ShowHeader = false;
				this.popupControl.ShowShadow = ShowShadow;
				this.popupControl.Visible = !DesignMode;
				Controls.Add(this.popupControl);
				CreateDropDownControls(this.popupControl.Controls);
			}
		}
		PopupHorizontalAlign GetEffectivePopupHorizontalAlign() {
			if(IsRightToLeft) {
				switch(Edit.PopupHorizontalAlign) {
					case PopupHorizontalAlign.LeftSides:
						return PopupHorizontalAlign.RightSides;
					case PopupHorizontalAlign.OutsideLeft:
						return PopupHorizontalAlign.OutsideRight;
					case PopupHorizontalAlign.OutsideRight:
						return PopupHorizontalAlign.OutsideLeft;
					case PopupHorizontalAlign.RightSides:
						return PopupHorizontalAlign.LeftSides;
				}
			}
			return Edit.PopupHorizontalAlign;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(PopupControl != null) {
				PreparePopupControl(PopupControl);
				PrepareDropDownControls(PopupControl.Controls);
			}
			if(ItemImageCell != null) {
				RenderUtils.SetStringAttribute(ItemImageCell, "valign", "middle");
				if(Edit.NeedPopupControl)
					RenderUtils.SetStringAttribute(ItemImageCell, TouchUtils.TouchMouseDownEventName, DropDownHandler);
				PrepareItemImageCell(ItemImageCell.Controls);
			}
		}
		protected override void PrepareButton(EditButton button, ButtonCell cell) {
			base.PrepareButton(button, cell);
			if (Edit.ButtonIsDropDown(button) && NeedDropDownButtonHandler()) {
				cell.OnMouseDownScript = DropDownHandler;
				RenderUtils.SetPreventSelectionAttribute(cell);
			}
		}
		protected virtual bool NeedDropDownButtonHandler() {
			return (Edit.NeedPopupControl || Edit.IsPopupControlShared);
		}
		protected override void PrepareTable(Table table) {
			base.PrepareTable(table);
			RenderUtils.SetStringAttribute(table, "onclick", ClickHandler);
		}
		protected override void AssignMainCellEvents(TableCell mainCell) {
			base.AssignMainCellEvents(mainCell);
			RenderUtils.SetStringAttribute(mainCell, TouchUtils.TouchMouseDownEventName, MainCellMouseDownHandler);
		}
		protected virtual void PreparePopupControl(ASPxPopupControl popupControl) {
		}
		protected virtual void CreateDropDownControls(ControlCollection controls) {
		}
		protected virtual void PrepareDropDownControls(ControlCollection controls) {
		}
		protected override void CreateImageCellBeforeImput(TableRow row) {
			if(Edit.IsNeedItemImageCell) {
				this.itemImageCell = RenderUtils.CreateTableCell();
				string className = Edit.GetCssClassNamePrefix("IIC");
				if(IsRightToLeft)
					className += "R";
				RenderUtils.AppendDefaultDXClassName(ItemImageCell, className);
				row.Cells.Add(ItemImageCell);
				CreateItemImageCell(ItemImageCell.Controls);
			}
			base.CreateImageCellBeforeImput(row);
		}
		protected virtual void CreateItemImageCell(ControlCollection controls) {
		}
		protected virtual void PrepareItemImageCell(ControlCollection controls) {
		}
	}
	public class ColorEditDisplayControl : ASPxInternalWebControl {
		private TableRow indicatorRow = null;
		private string color = "";
		private WebControl colorDiv = null;
		private LiteralControl literalControl = null;
		private TableCell literalCell = null;
		private ColorEditProperties colorProperties = null;
		public string Color {
			get { return color; }
			set { color = value; }
		}
		protected TableRow IndicatorRow {
			get { return indicatorRow; }
		}
		protected WebControl ColorDiv {
			get { return colorDiv; }
		}
		protected LiteralControl LiteralControl {
			get { return literalControl; }
		}
		protected TableCell LiteralCell {
			get { return literalCell; }
		}
		protected ColorEditProperties ColorProperties {
			get { return colorProperties; }
		}
		public ColorEditDisplayControl(ColorEditProperties properties) {
			this.colorProperties = properties;
		}
		protected override void ClearControlFields() {
			this.indicatorRow = null;
			this.colorDiv = null;
			this.literalCell = null;
			this.literalControl = null;
		}
		protected override void CreateControlHierarchy() {
			CreateIndicatorRow();
			CreateColorIndicator();
			CreateTextIndicator();
		}
		protected override void PrepareControlHierarchy() {
			PrepareColorIndicator();
			PrepareTextIndicator();
		}
		protected void CreateIndicatorRow() {
			Table table = RenderUtils.CreateTable();
			Controls.Add(table);
			this.indicatorRow = RenderUtils.CreateTableRow();
			table.Rows.Add(IndicatorRow);
		}
		protected void CreateColorIndicator() {
			TableCell cell = RenderUtils.CreateTableCell();
			IndicatorRow.Cells.Add(cell);
			this.colorDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			cell.Controls.Add(ColorDiv);
		}
		protected void CreateTextIndicator() {
			this.literalCell = RenderUtils.CreateTableCell();
			IndicatorRow.Cells.Add(LiteralCell);
			this.literalControl = RenderUtils.CreateLiteralControl();
			LiteralCell.Controls.Add(LiteralControl);
		}
		protected void PrepareColorIndicator() {
			AppearanceStyleBase style = ColorProperties.GetDisplayColorIndicatorStyle();
			style.AssignToControl(ColorDiv);
			Unit width = ColorProperties.GetColorIndicatorWidth(ColorProperties.DisplayColorIndicatorWidth, style);
			RenderUtils.SetStyleStringAttribute(ColorDiv, "width", !width.IsEmpty ? width.ToString() : "", false);
			Unit height = ColorProperties.GetColorIndicatorHeight(ColorProperties.DisplayColorIndicatorHeight, style);
			RenderUtils.SetStyleStringAttribute(ColorDiv, "height", !height.IsEmpty ? height.ToString() : "", false);
			RenderUtils.SetStyleStringAttribute(ColorDiv, "background-color", Color, false);
		}
		protected void PrepareTextIndicator() {
			LiteralControl.Text = Color;
			if(ColorProperties.GetDisplayColorIndicatorSpacing() != Unit.Pixel(0))
				RenderUtils.SetStyleUnitAttribute(LiteralCell, "padding-left", ColorProperties.GetDisplayColorIndicatorSpacing(), true);
		}
	}
	public class DropDownControl : DropDownControlBase {
		protected override bool AdjustInnerControlsSizeOnShow {
			get { return true; }
		}
		public DropDownControl(ASPxDropDownEdit edit)
			: base(edit) {
		}
		protected new ASPxDropDownEdit Edit {
			get { return (ASPxDropDownEdit)base.Edit; }
		}
		protected override bool EnableDropDownPopupControlViewState {
			get { return true; }
		}
		protected override void PreparePopupControl(DevExpress.Web.ASPxPopupControl popupControl) {
			popupControl.ContentStyle.Assign(Edit.GetDropDownWindowStyle());
			base.PreparePopupControl(popupControl);
		}
		protected override void CreateDropDownControls(ControlCollection controls) {
			if(Edit.DropDownWindowTemplate != null) {
				WebControl div = RenderUtils.CreateDiv();
				div.ID = Edit.GetDropDownWindowDivContainerID();
				controls.Add(div);
				TemplateContainerBase container = new TemplateContainerBase(0, this);
				container.ID = Edit.GetDropDownWindowTemplateContainerID();
				div.Controls.Add(container);
				Edit.DropDownWindowTemplate.InstantiateIn(container);
			}
		}
		protected override void AssignMainCellEvents(TableCell mainCell) {
			base.AssignMainCellEvents(mainCell);
			if(!Edit.AllowUserInput || Edit.ReadOnly)
				RenderUtils.SetStringAttribute(mainCell, TouchUtils.TouchMouseDownEventName, DropDownHandler);
		}
	}
	public class DateEditControl : DropDownControlBase {
		public DateEditControl(ASPxDateEdit dateEdit)
			: base(dateEdit) {
			DateEdit = dateEdit;
		}
		protected ASPxDateEdit DateEdit { get; private set; }
		protected ASPxCalendar Calendar { get; private set; }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			Calendar = null;
		}
		protected override void CreateDropDownControls(ControlCollection controls) {
			Calendar = CreateCalendar();
			ClientIDHelper.EnableClientIDGeneration(this.Calendar);
			Calendar.ID = "C";
			Calendar.TimeSectionOwner = DateEdit;
			Calendar.Properties.ParentSkinOwner = DateEdit;
			Calendar.Properties.ParentImages = DateEdit.RenderImages;
			Calendar.Properties.ParentStyles = DateEdit.RenderStyles;
			Calendar.Properties.Assign(DateEdit.CalendarProperties);
			Calendar.RenderIFrameForPopupElements = DateEdit.RenderIFrameForPopupElements;
			Calendar.CustomDisabledDate += DateEdit.CalendarCutomDisabledDateHandler;
			Calendar.DayCellInitialize += DateEdit.CalendarDayCellInitializeHandler;
			Calendar.DayCellCreated += DateEdit.CalendarDayCellCreatedHandler;
			Calendar.DayCellPrepared += DateEdit.CalendarDayCellPreparedHandler;
			controls.Add(Calendar);
		}
		protected virtual ASPxCalendar CreateCalendar() {
			return new ASPxCalendar(DateEdit);
		}
		protected override void PrepareDropDownControls(ControlCollection controls) {			
			Calendar.InplaceMode = DateEdit.InplaceMode;
			Calendar.ImageFolder = DateEdit.ImageFolder;
			Calendar.IsDateEditCalendar = true;
			Calendar.VisibleDate = DateEdit.Date;
			Calendar.SelectedDate = DateEdit.Date;
			Calendar.ReadOnly = DateEdit.ReadOnly;
			Calendar.EnableViewState = false;
			Calendar.EncodeHtml = DateEdit.EncodeHtml;
			if(!DateEdit.AllowNull)
				Calendar.ShowClearButton = false;
		}
	}
	public class ComboBoxNativeControl : ListNativeControl {
		private ASPxListBox listBox = null;
		public ComboBoxNativeControl(ASPxComboBox comboBox)
			: base(comboBox) {
		}
		public ASPxComboBox ComboBox {
			get { return (ASPxComboBox)Edit; }
		}
		protected override ListEditItemCollection Items {
			get { return ComboBox.Items; }
		}
		protected override int RowCount {
			get { return 1; }
		}
		protected override int SelectedIndex {
			get { return ComboBox.SelectedIndex; }
		}
		protected override Type ValueType {
			get { return ComboBox.ValueType; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.listBox = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.listBox = ComboBox.CreateListBox();
			Controls.Add(this.listBox);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ComboBox.PrepareListBox(this.listBox);
		}
		protected override void PrepareSelectControl(WebControl selectControl) {
			base.PrepareSelectControl(selectControl);
			if(ComboBox.IsEnabled() && !DesignMode) {
				RenderUtils.SetStringAttribute(selectControl, "onfocus", ComboBox.GetOnGotFocus());
				RenderUtils.SetStringAttribute(selectControl, "onblur", ComboBox.GetOnLostFocus());
				RenderUtils.SetStringAttribute(selectControl, "onchange", ComboBox.GetOnTextChanged());
			}
		}
	}
	public class AutoCompleteControlBase : DropDownControlBase {
		private ASPxAutoCompleteBoxBase autoCompleteBox = null;
		private ASPxListBox listBox = null;
		private WebControl input = null;
		private Image itemImage = null;
		private DropDownStyle dropDownStyle = DropDownStyle.DropDownList;
		private bool autoPostBack = false;
		private string dataSourceID = "";
		private string dataMember = "";
		private string imageUrlField = "";
		private string textField = "";
		private string valueField = "";
		public AutoCompleteControlBase(ASPxAutoCompleteBoxBase autoCompleteBox)
			: base(autoCompleteBox) {
			this.autoCompleteBox = autoCompleteBox;
		}
		public ASPxListBox ListBoxControl {
			get { return listBox; }
		}
		public bool AutoPostBack {
			get { return autoPostBack; }
			set { autoPostBack = value; }
		}
		public DropDownStyle DropDownStyle {
			get { return dropDownStyle; }
			set { dropDownStyle = value; }
		}
		public new ASPxAutoCompleteBoxBase Edit {
			get { return base.Edit as ASPxAutoCompleteBoxBase; }
		}
		public string DataSourceID {
			get { return dataSourceID; }
			set { dataSourceID = value; }
		}
		public string DataMember {
			get { return dataMember; }
			set { dataMember = value; }
		}
		public string ImageUrlField {
			get { return imageUrlField; }
			set { imageUrlField = value; }
		}
		public string TextField {
			get { return textField; }
			set { textField = value; }
		}
		public string ValueField {
			get { return valueField; }
			set { valueField = value; }
		}
		protected bool IsDropDownListMode {
			get { return DropDownStyle == DropDownStyle.DropDownList || Edit.DataSecurityMode == DataSecurityMode.Strict; }
		}
		protected ASPxAutoCompleteBoxBase AutoCompleteBox {
			get { return autoCompleteBox; }
		}
		protected ASPxListBox ListBox {
			get { return listBox; }
		}
		protected WebControl Input {
			get { return input; }
		}
		protected Image ItemImage {
			get { return itemImage; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.listBox = null;
			this.input = null;
			this.itemImage = null;
		}
		protected override void CreateCells(TableRow row) {
			if (!DesignMode) {
				TableCell valueInputCell = RenderUtils.CreateTableCell();
				RenderUtils.SetStyleAttribute(valueInputCell, "display", "none", "");
				row.Cells.Add(valueInputCell);
				this.input = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
				valueInputCell.Controls.Add(this.input);
			}
			base.CreateCells(row);
		}
		protected override void CreateDropDownControls(ControlCollection controls) {
			this.listBox = CreateListBox();
			controls.Add(this.listBox);
		}
		protected ASPxListBox CreateListBox() {
			return AutoCompleteBox.CreateListBox();
		}
		protected override void CreateItemImageCell(ControlCollection controls) {
			this.itemImage = RenderUtils.CreateImage();
			controls.Add(ItemImage);
		}
		protected void Changed() {
			IPropertiesOwner owner = ListBox as IPropertiesOwner;
			if(owner != null)
				owner.Changed(ListBox.Properties);
		}
		protected override void AssignMainCellEvents(TableCell mainCell) {
			base.AssignMainCellEvents(mainCell);
			if(IsDropDownListMode && (Edit.NeedPopupControl || AutoCompleteBox.LoadDropDownOnDemand))
				RenderUtils.SetStringAttribute(mainCell, TouchUtils.TouchMouseDownEventName, DropDownHandler);
		}
		protected override void PrepareDropDownControls(ControlCollection controls) {
			Changed();
			AutoCompleteBox.PrepareListBox(ListBox);
		}
		protected override bool NeedDropDownButtonHandler() {
			return base.NeedDropDownButtonHandler() || AutoCompleteBox.LoadDropDownOnDemand;
		}
		protected override void PreparePopupControl(ASPxPopupControl popupControl) {
			base.PreparePopupControl(popupControl);
			popupControl.ClientSideEvents.Shown = AutoCompleteBox.GetPopupControlOnShown();
		}
		protected override void PrepareInputControl(InputControl input) {
			base.PrepareInputControl(input);
			if(IsDropDownListMode && AutoCompleteBox.IncrementalFilteringMode == IncrementalFilteringMode.None) {
				input.IsReadOnly = true;
				if(string.IsNullOrEmpty(input.ControlStyle.Cursor))
					input.ControlStyle.Cursor = RenderUtils.GetDefaultCursor();
			}
			if(Browser.IsSafari && Browser.Platform.IsMacOSMobile) 
				RenderUtils.SetStringAttribute(input, "autocorrect", "off");
		}
		protected override bool RequireHideDefaultIEClearButton() {
			return base.RequireHideDefaultIEClearButton() || IsDropDownListMode && AutoCompleteBox.IncrementalFilteringMode != IncrementalFilteringMode.None;
		}
		protected override void PrepareControlHierarchy() {
			if(!DesignMode) {
				RenderUtils.SetStringAttribute(Input, "name", Input.ClientID);
				RenderUtils.SetStringAttribute(Input, "type", "hidden");
				RenderUtils.SetStringAttribute(Input, "value", CommonUtils.ValueToString(Edit.Value));
			}
			if(ItemImageCell != null) {
				if(IsRightToLeft)
					RenderUtils.SetPaddings(ItemImageCell, Edit.ItemStyle.ImageSpacing, 0, 0, 0);
				else
					RenderUtils.SetPaddings(ItemImageCell, 0, 0, Edit.ItemStyle.ImageSpacing, 0);
			}
			base.PrepareControlHierarchy();
		}
	}
	public class ComboBoxControl : AutoCompleteControlBase {
		public ComboBoxControl(ASPxComboBox comboBox)
			: base(comboBox) {
		}
		public new ASPxComboBox Edit {
			get { return base.Edit as ASPxComboBox; }
		}
		protected ASPxComboBox ComboBox {
			get { return AutoCompleteBox as ASPxComboBox; }
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if (!DesignMode)
				Input.ID = ASPxComboBox.ValueHiddenInputID;
		}
		protected override void PrepareItemImageCell(ControlCollection controls) {
			ImageProperties imageProperties = ComboBox.GetImage();
			imageProperties.AssignToControl(ItemImage, DesignMode);
			if(!ComboBox.IsImageVisible())
				RenderUtils.SetStyleStringAttribute(ItemImageCell, "display", "none");
		}
	}
	public class TokenBoxControl : AutoCompleteControlBase {
		private ASPxTokenBox tokenBox;
		private WebControl tokensHiddenInput;
		private WebControl tokensValuesHiddenInput;
		private WebControl sampleToken;
		protected const string TokenBoxMainCellCssClassName = "dxictb";
		protected const string TokenBoxTableCssClassName = "dxeTokenBox";
		protected const string TokenSuffix = "Token";
		protected const string TokenTextSuffix = "TokenT";
		protected const string TokenRemoveButtonSuffix = "TokenRB";
		public TokenBoxControl(ASPxTokenBox tokenBox)
			: base(tokenBox) {
				this.tokenBox = tokenBox;
		}
		protected ASPxTokenBox TokenBox {
			get { return tokenBox; }
		}
		protected WebControl TokensHiddenInput {
			get { return tokensHiddenInput; }
		}
		protected WebControl TokensValuesHiddenInput {
			get { return tokensValuesHiddenInput; }
		}
		protected WebControl SampleToken {
			get { return sampleToken; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.tokensHiddenInput = null;
			this.tokensValuesHiddenInput = null;
			this.sampleToken = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(!DesignMode) {
				sampleToken = CreateTokenHierarchy("Text", -1);
				Parent.Controls.Add(sampleToken);
				tokensHiddenInput = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
				tokensHiddenInput.ID = ASPxTokenBox.TokensHiddenInputID;
				tokensValuesHiddenInput = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
				tokensValuesHiddenInput.ID = ASPxTokenBox.TokensValuesHiddenInputID;
				MainCell.Controls.AddAt(0, TokensValuesHiddenInput);
				MainCell.Controls.AddAt(0, TokensHiddenInput);
				if(TokenBox.Tokens.Count > 0)
					CreateTokens(TokenBox.Tokens);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(!DesignMode) {
				RenderUtils.SetStringAttribute(tokensHiddenInput, "name", TokensHiddenInput.ClientID);
				RenderUtils.SetStringAttribute(tokensHiddenInput, "type", "hidden");
				RenderUtils.SetStringAttribute(tokensHiddenInput, "value", TokenBox.SerializeTokens());
				RenderUtils.SetStringAttribute(tokensValuesHiddenInput, "name", TokensValuesHiddenInput.ClientID);
				RenderUtils.SetStringAttribute(tokensValuesHiddenInput, "type", "hidden");
				RenderUtils.SetStringAttribute(tokensValuesHiddenInput, "value", TokenBox.SerializeTokensValues());
				RenderUtils.SetStringAttribute(Table, "onClick", TokenBox.GetMainElementClick());
				RenderUtils.SetStyleAttribute(sampleToken, "display", "none", "");
				ExtendedStyleBase tokenStyle = TokenBox.GetTokenBoxTokenStyle();
				ExtendedStyleBase tokenTextStyle = TokenBox.GetTokenBoxTokenTextStyle();
				ExtendedStyleBase tokenRemoveButtonStyle = TokenBox.GetTokenBoxTokenRemoveButtonStyle();
				if(TokenBox.Tokens.Count > 0)
					PrepareTokens(tokenStyle, tokenTextStyle, tokenRemoveButtonStyle);
				PrepareTokenHierarchy(SampleToken, tokenStyle, tokenTextStyle, tokenRemoveButtonStyle);
				MainCell.CssClass = RenderUtils.CombineCssClasses(MainCell.CssClass, TokenBoxMainCellCssClassName);
				Table.CssClass = RenderUtils.CombineCssClasses(Table.CssClass, TokenBoxTableCssClassName);
				if(Table.Width.Type == UnitType.Percentage) {
					RenderUtils.SetStyleStringAttribute(Table, "table-layout", "fixed", true);
				}
			}
		}
		protected override void PrepareInputControl(InputControl input) {
			base.PrepareInputControl(input);
			input.ControlStyle.CopyFrom(TokenBox.GetTokenBoxInputStyle());
			if(IsRightToLeft)
				RenderUtils.SetStyleAttribute(input, "float", "right", "");
			if(DesignMode)
				input.Width = Unit.Percentage(100);
		}
		protected void CreateTokens(TokenCollection tokens) {
			for(int ind = tokens.Count - 1; ind >= 0; ind--) {
				string tokenText = TokenBox.EncodeHtml ? HttpUtility.HtmlEncode(tokens[ind]) : tokens[ind];
				WebControl tokenControl = CreateTokenHierarchy(tokenText, ind);
				MainCell.Controls.AddAt(0, tokenControl);
			}
		}
		protected WebControl CreateTokenHierarchy(string text, int index) {
			WebControl tokenControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			WebControl tokenRemoveButton = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			WebControl tokenText = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			tokenControl.ID = TokenSuffix + index;
			tokenText.ID = TokenTextSuffix + index;
			tokenRemoveButton.ID = TokenRemoveButtonSuffix + index;
			tokenText.Controls.Add(RenderUtils.CreateLiteralControl(text));
			tokenControl.Controls.Add(tokenText);
			tokenControl.Controls.Add(tokenRemoveButton);
			return tokenControl;
		}
		protected void PrepareTokens(ExtendedStyleBase tokenStyle, ExtendedStyleBase tokenTextStyle, ExtendedStyleBase tokenRemoveButtonStyle) {
			foreach(WebControl control in MainCell.Controls) {
				if(control.ID.Contains(TokenSuffix)) {
					PrepareTokenHierarchy(control, tokenStyle, tokenTextStyle, tokenRemoveButtonStyle);
				}
			}
		}
		protected void PrepareTokenHierarchy(WebControl tokenControl, ExtendedStyleBase tokenStyle, ExtendedStyleBase tokenTextStyle, ExtendedStyleBase tokenRemoveButtonStyle) {
			WebControl tokenText = null;
			WebControl tokenRemoveButton = null;
			foreach(WebControl control in tokenControl.Controls) {
				if(control.ID.Contains(TokenTextSuffix))
					tokenText = control;
				else if(control.ID.Contains(TokenRemoveButtonSuffix))
					tokenRemoveButton = control;
			}
			if(tokenText != null && tokenRemoveButton != null) {
				tokenStyle.AssignToControl(tokenControl);
				tokenTextStyle.AssignToControl(tokenText);
				if(String.IsNullOrEmpty(tokenRemoveButtonStyle.BackgroundImage.ImageUrl))
					TokenBox.GetTokenBoxTokenRemoveButtonDefaultImage().AssignToControl(tokenRemoveButton, DesignMode, !TokenBox.Enabled || !TokenBox.ClientEnabled);
				tokenRemoveButtonStyle.AssignToControl(tokenRemoveButton);
				tokenRemoveButton.ToolTip = TokenBox.GetTokenRemoveButtonToolTip();
				if(TokenBox.Enabled)
					RenderUtils.SetStringAttribute(tokenRemoveButton, "onClick", TokenBox.GetTokenRBClick());
				if(IsRightToLeft) {
					RenderUtils.SetStyleAttribute(tokenControl, "float", "right", "");
					RenderUtils.SetStyleAttribute(tokenText, "float", "right", "");
					RenderUtils.SetStyleAttribute(tokenRemoveButton, "float", "right", "");
				}
				if(TokenBox.Enabled) {
					tokenText.Style.Add("max-width", "30px");
					tokenText.Style.Add("text-overflow", "ellipsis");
					tokenText.Style.Add("white-space", "nowrap");
					tokenText.Style.Add("overflow", "hidden");
				}
			}
		}
	}
	public class ColorEditControl : DropDownControlBase {
		protected WebControl ColorIndicatorDiv { get; set; }
		protected ColorTablesControl ColorTablesControl { get; set; }
		protected ColorSelectorControl ColorSelectorControl { get; set; }
		protected ColorNestedControl ColorNestedControl { get; set; }
		public ColorEditControl(ASPxColorEdit edit)
			: base(edit) {
		}
		protected new ASPxColorEdit Edit {
			get { return (ASPxColorEdit)base.Edit; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ColorIndicatorDiv = null;
			ColorTablesControl = null;
			ColorSelectorControl = null;
		}
		protected override void CreateDropDownControls(ControlCollection controls) {
			ColorNestedControl = new ColorNestedControl(Edit.Properties.ColorNestedControlProperties);
			ClientIDHelper.EnableClientIDGeneration(ColorNestedControl);
			ColorNestedControl.ParentSkinOwner = Edit;
			ColorNestedControl.ColorTableStyle.CopyFrom(Edit.Properties.ColorTableStyle);
			ColorNestedControl.ColorTableCellStyle.CopyFrom(Edit.Properties.ColorTableCellStyle);
			ColorNestedControl.ColorPickerStyle.CopyFrom(Edit.Properties.ColorPickerStyle);
			controls.Add(ColorNestedControl);
		}
		protected override void PrepareDropDownControls(ControlCollection controls) {
			ColorNestedControl.EncodeHtml = Edit.EncodeHtml;
		}
		protected override void CreateItemImageCell(ControlCollection controls) {
			ColorIndicatorDiv = RenderUtils.CreateDiv();
			controls.Add(ColorIndicatorDiv);
			ColorIndicatorDiv.ID = Edit.GetColorIndicatorID();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Edit.GetColorIndicatorStyle().AssignToControl(ColorIndicatorDiv);
			RenderUtils.SetStyleStringAttribute(ColorIndicatorDiv, "background-color", Edit.GetHexColor(), false);
			Unit indicatorWidth = Edit.GetColorIndicatorWidth();
			RenderUtils.SetStyleStringAttribute(ColorIndicatorDiv, "width", !indicatorWidth.IsEmpty ? indicatorWidth.ToString() : "", false);
			Unit indicatorHeight = Edit.GetColorIndicatorHeight();
			RenderUtils.SetStyleStringAttribute(ColorIndicatorDiv, "height", !indicatorHeight.IsEmpty ? indicatorHeight.ToString() : "", false);
			if(DesignMode)
				RenderUtils.SetStyleStringAttribute(ColorIndicatorDiv, "overflow", "hidden");
		}
		protected override void PrepareInputControl(InputControl input) {
			base.PrepareInputControl(input);
			if(IsRightToLeft) {
				input.Attributes["dir"] = "ltr";
				if(input.ControlStyle.HorizontalAlign == HorizontalAlign.NotSet)
					input.ControlStyle.HorizontalAlign = HorizontalAlign.Right;
			}
		}
		protected override void AssignMainCellEvents(TableCell mainCell) {
			base.AssignMainCellEvents(mainCell);
			if(!Edit.AllowUserInput)
				RenderUtils.SetStringAttribute(mainCell, TouchUtils.TouchMouseDownEventName, DropDownHandler);
		}
	}
}

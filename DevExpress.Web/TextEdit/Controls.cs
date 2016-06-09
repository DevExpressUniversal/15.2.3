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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
namespace DevExpress.Web.Internal {
	public abstract class InputControlBase : ASPxInternalWebControl {
		private string name = "";
		private bool isReadOnly = false;
		private string text = "";
		private string onBlur = "";
		private string onChange = "";
		private string onGotFocus = "";
		private string onMouseOut = "";
		private string onMouseOver = "";
		private WebControl input = null;
		private AppearanceInputStyle controlStyle = new AppearanceInputStyle();
		public InputControlBase()
			: base() {
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public bool IsReadOnly {
			get { return isReadOnly; }
			set { isReadOnly = value; }
		}
		public string Text {
			get { return text; }
			set { text = value; }
		}
		public string OnBlur {
			get { return onBlur; }
			set { onBlur = value; }
		}
		public string OnChange {
			get { return onChange; }
			set { onChange = value; }
		}
		public string OnGotFocus {
			get { return onGotFocus; }
			set { onGotFocus = value; }
		}
		public string OnMouseOut {
			get { return onMouseOut; }
			set { onMouseOut = value; }
		}
		public string OnMouseOver {
			get { return onMouseOver; }
			set { onMouseOver = value; }
		}
		public new AppearanceInputStyle ControlStyle {
			get { return controlStyle; }
		}
		protected abstract HtmlTextWriterTag InputElementTag { get; }
		protected override void ClearControlFields() {
			this.input = null;
		}
		protected override void CreateControlHierarchy() {
			CreateInputControl(this);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(this, this.input);
			ControlStyle.AssignToControl(this.input, true);
			AssignCustomAttributesToInput(this.input);
		}
		protected virtual void AssignCustomAttributesToInput(WebControl input) {
			if(!String.IsNullOrEmpty(Name))
				RenderUtils.SetStringAttribute(input, "name", Name);
			if(IsReadOnly)
				RenderUtils.SetStringAttribute(input, "readonly", "readonly");
			if(!Enabled)
				RenderUtils.SetStringAttribute(input, "disabled", "disabled");
			if(!DesignMode)
				PrepareClientSideEvents(input);
			AssignTextToInput(input);
		}
		protected abstract void AssignTextToInput(WebControl input);
		protected void CreateInputControl(Control inputControlContainer) {
			this.input = RenderUtils.CreateWebControl(InputElementTag);
			inputControlContainer.Controls.Add(this.input);
		}
		protected virtual void PrepareClientSideEvents(WebControl input) {
			RenderUtils.SetStringAttribute(input, "onfocus", OnGotFocus);
			RenderUtils.SetStringAttribute(input, "onblur", OnBlur);
			RenderUtils.SetStringAttribute(input, "onchange", OnChange);
			RenderUtils.SetStringAttribute(input, "onmouseout", OnMouseOut);
			RenderUtils.SetStringAttribute(input, "onmouseover", OnMouseOver);
		}
	}
	public class InputControl : InputControlBase {
		private int maxLength = 0;
		private bool isPassword = false;
		private int size = 0;
		public InputControl()
			: base() {
		}
		public bool IsPassword {
			get { return isPassword; }
			set { isPassword = value; }
		}
		public int MaxLength {
			get { return maxLength; }
			set { maxLength = value; }
		}
		public int Size {
			get { return size; }
			set { size = value; }
		}
		protected override HtmlTextWriterTag InputElementTag {
			get { return HtmlTextWriterTag.Input; }
		}
		protected override void AssignCustomAttributesToInput(WebControl input) {
			base.AssignCustomAttributesToInput(input);
			RenderUtils.SetStringAttribute(input, "type", IsPassword ? "password" : "text");
			if(MaxLength > 0)
				RenderUtils.SetStringAttribute(input, "maxlength", MaxLength.ToString());
			if(Size > 0 || DesignMode)
				RenderUtils.SetStringAttribute(input, "size", Size.ToString());
		}
		protected override void AssignTextToInput(WebControl input) {
			RenderUtils.SetAttribute(input, "value", CommonUtils.ValueToString(Text), "");
		}
	}
	public class TextAreaControl : InputControlBase {
		private 
			int columns;
			bool isRightToLeft = false;
			int rows;
		public TextAreaControl(bool isRightToLeft)
			: base() {
				this.isRightToLeft = isRightToLeft;
		}
		public int Columns {
			get { return columns; }
			set { columns = value; }
		}
		public int Rows {
			get { return rows; }
			set { rows = value; }
		}
		protected override HtmlTextWriterTag InputElementTag {
			get { return HtmlTextWriterTag.Textarea; }
		}
		protected override void AssignCustomAttributesToInput(WebControl input) {
			base.AssignCustomAttributesToInput(input);
			RenderUtils.PrepareTextArea(input, Columns, Rows);
		}	 
		protected override void AssignTextToInput(WebControl input) {
			input.Controls.Clear();
			input.Controls.Add(new LiteralControl(Text));
		}
	}
	public class TableBasedControlBase : ASPxInternalWebControl {
		private Table table = null;
		private TableRow mainRow = null;
		private TableCell mainCell = null;
		protected Table Table {
			get { return this.table; }
		}
		protected TableRow MainRow {
			get { return this.mainRow; }
		}
		protected TableCell MainCell {
			get { return this.mainCell; }
		}
		protected override void ClearControlFields() {
			this.table = null;
			this.mainRow = null;
			this.mainCell = null;
		}
		protected virtual void CreateCells(TableRow row) {
			this.mainCell = RenderUtils.CreateTableCell();
			row.Cells.Add(this.mainCell);
			CreateMainCellContent(this.mainCell);
		}
		protected virtual InternalTable CreateTable() {
			return RenderUtils.CreateTable(true);
		}
		protected override void CreateControlHierarchy() {
			this.table = CreateTable();
			Controls.Add(this.table);
			this.mainRow = RenderUtils.CreateTableRow();
			this.table.Rows.Add(MainRow);
			CreateCells(MainRow);
		}
		protected virtual void CreateMainCellContent(TableCell mainCell) {
		}
		protected override void PrepareControlHierarchy() {
			PrepareTable(this.table);
			PrepareMainCell(this.mainCell);
			PrepareMainCellContent();
		}
		protected virtual void PrepareMainCell(TableCell mainCell) {
		}
		protected virtual void PrepareMainCellContent() {
		}
		protected virtual void PrepareTable(Table table) {
		}
	}
	public class TableBasedControl : TableBasedControlBase {
		private ASPxEdit edit;
		public TableBasedControl(ASPxEdit edit)
			: base() {
			this.edit = edit;
		}
		protected virtual ASPxEdit Edit {
			get { return this.edit; }
		}
		protected Unit GetEditWidth() { 
			if(Edit.ExternalTable != null) {
				if(!Width.IsEmpty && Width.Type == UnitType.Percentage) {
					return Unit.Percentage(100);
				}
			}
			return Edit.Width;
		}
		protected override void PrepareControlHierarchy() {
			Width = GetEditWidth();
			if(Edit.IsAriaSupported() && Edit.IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(Edit, "role", "presentation");
			base.PrepareControlHierarchy();
		}
		protected override void PrepareTable(Table table) {
			RenderUtils.AssignAttributes(Edit, table);
			table.Width = GetEditWidth();
			Edit.RemoveImportedStyleAttrsFromMainElement(table);
			RenderUtils.SetVisibility(table, Edit.IsClientVisible(), true);
			table.AccessKey = "";
			table.TabIndex = 0;
			base.PrepareTable(table);
			AppearanceStyleBase style = Edit.GetControlStyle();
			style.AssignToControl(table, AttributesRange.Common | AttributesRange.Font);
		}
		protected override void PrepareMainCell(TableCell editCell) {
			AppearanceStyle style = (AppearanceStyle)Edit.GetControlStyle();
			editCell.Width = !Width.IsEmpty ? Unit.Percentage(100) : Unit.Empty;
			style.Paddings.AssignToControl(editCell);
		}
	}
	public class PureTextBoxControlBase : TableBasedControl {
		private InputControl input = null;
		public PureTextBoxControlBase(ASPxPureTextBoxBase edit)
			: base(edit) {
		}
		protected new ASPxPureTextBoxBase Edit {
			get { return (ASPxPureTextBoxBase)base.Edit; }
		}
		protected bool IsRightToLeft {
			get { return (Edit as ISkinOwner).IsRightToLeft(); }
		}
		protected virtual void AssignMainCellEvents(TableCell mainCell) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.input = null;
		}
		protected override InternalTable CreateTable() {
			return RenderUtils.CreateTable(false);
		}
		protected override void CreateMainCellContent(TableCell mainCell) {
			mainCell.Controls.Add(CreateInputControl());
		}
		protected virtual Control CreateInputControl() {
			this.input = new InputControl();
			this.input.ID = ASPxTextEdit.InputControlSuffix;
			return this.input;
		}
		protected internal void SetAccessibilityInputTitle(string title) {
			RenderUtils.SetStringAttribute(this.input, "title", title);
		}
		protected override void PrepareMainCell(TableCell mainCell) {
			base.PrepareMainCell(mainCell);
			mainCell.CssClass = RenderUtils.DefaultInputCellStyleName;
			if(IsEnabled())
				AssignMainCellEvents(mainCell);
			if(Edit.IsInputStretched)
				mainCell.Width = Unit.Percentage(100);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareInputControl(this.input);
		}
		protected virtual void PrepareInputControl(InputControl input) {
			input.AccessKey = Edit.AccessKey;
			if(Edit.SendValueToServer)
				input.Name = Edit.UniqueID;
			input.Text = Edit.HtmlEncode(Edit.GetInputText(), true);
			input.TabIndex = Edit.IsEnabled() ? Edit.TabIndex : (short)-1;
			AppearanceStyleBase editStyle = Edit.GetControlStyle();
			AppearanceStyle style = Edit.GetEditAreaStyle();
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, GetSystemClassname());
			style.BackColor = editStyle.BackColor;
			style.ForeColor = editStyle.ForeColor;
			style.Cursor = editStyle.Cursor;
			input.ControlStyle.Assign(style);
			if(!Edit.IsInputStretched)
				input.Width = Unit.Empty;
			if(IsEnabled() || Edit.NeedFocusCorrectionWhenDisabled()) {
				input.OnGotFocus = Edit.GetOnGotFocus();
				input.OnBlur = Edit.GetOnLostFocus();
			}
			if(IsEnabled()) {
				input.OnChange = Edit.GetOnTextChanged();
				input.OnMouseOver = Edit.GetOnMouseOver();
			}
			if(IsRightToLeft && Edit.IsMaskCapabilitiesEnabled()) {
				input.Attributes["dir"] = "ltr";
				if(input.ControlStyle.HorizontalAlign == HorizontalAlign.NotSet)
					input.ControlStyle.HorizontalAlign = HorizontalAlign.Right;
			}
		}
		protected virtual string GetSystemClassname() {
			return EditorStyles.EditAreaSystemClassName;
		}
	}
	public class TextBoxControlBase : PureTextBoxControlBase {
		public TextBoxControlBase(ASPxTextBoxBase edit)
			: base(edit) {
		}
		protected new ASPxTextBoxBase Edit {
			get { return (ASPxTextBoxBase)base.Edit; }
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(Edit.ExternalTable != null && Browser.IsIE)
				this.Table.Style[HtmlTextWriterStyle.BorderCollapse] = "separate"; 
		}
		protected override void PrepareInputControl(InputControl input) {
			base.PrepareInputControl(input);
			if(Edit.UseReadOnlyForDisabled())
				input.IsReadOnly = Edit.ReadOnly || !Edit.IsUserInputAllowed() || !Edit.IsEnabled();
			else {
				input.Enabled = Edit.IsEnabled();
				input.IsReadOnly = Edit.ReadOnly || !Edit.IsUserInputAllowed();
			}
			input.IsPassword = Edit.Password;
			if(!Edit.IsMaskCapabilitiesEnabled())
				input.MaxLength = Edit.MaxLength;
		}
		protected void PrepareTableSpacing(Table table, int spacing) {
			if(spacing <= 0)
				RenderUtils.CollapseAndRemovePadding(table);
			else if(spacing > 1 || !RenderUtils.IsHtml5Mode(Edit))
				table.CellSpacing = spacing;
		}
	}
	public class TextBoxControl : TextBoxControlBase {
		public TextBoxControl(ASPxTextBox edit)
			: base(edit) {
		}
		protected new ASPxTextBox Edit {
			get { return (ASPxTextBox)base.Edit; }
		}
		protected override void CreateCells(TableRow row) {
			base.CreateCells(row);
			if(DesignMode)
				row.Cells.Add(RenderUtils.CreateTableCell());
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(Edit.Size == 0 && Table != null) {
				RenderUtils.AppendDefaultDXClassName(Table, EditorStyles.TextBoxDefaultWidthSystemClassName);
			}	
		}
		protected override void PrepareInputControl(InputControl input) {
			base.PrepareInputControl(input);
			input.Size = Edit.Size;
		}
		protected override string GetSystemClassname() {
			return EditorStyles.EditAreaSystemClassName;
		}
	}
	public class TextBoxNativeControl : ASPxInternalWebControl {
		private ASPxTextBox edit = null;
		private WebControl inputControl = null;
		public TextBoxNativeControl(ASPxTextBox edit)
			: base() {
			this.edit = edit;
		}
		protected ASPxTextBox Edit {
			get { return edit; }
		}
		protected WebControl InputControl {
			get { return inputControl; }
		}
		protected override void ClearControlFields() {
			this.inputControl = null;
		}
		protected override void CreateControlHierarchy() {
			this.inputControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			Controls.Add(InputControl);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(Edit, InputControl);
			RenderUtils.SetVisibility(InputControl, Edit.IsClientVisible(), true);
			Edit.GetControlStyle().AssignToControl(InputControl);
			Edit.GetPaddings().AssignToControl(InputControl);
			InputControl.Height = Edit.Height;
			InputControl.Width = Edit.Width;
			if(Edit.Size == 0)
				RenderUtils.AppendDefaultDXClassName(InputControl, EditorStyles.TextBoxDefaultWidthSystemClassName);
			RenderUtils.SetStringAttribute(InputControl, "type", Edit.Password ? "password" : "text");
			if(Edit.MaxLength > 0 && !Edit.IsMaskCapabilitiesEnabled())
				RenderUtils.SetStringAttribute(InputControl, "maxlength", Edit.MaxLength.ToString());
			if(Edit.Size > 0 || DesignMode)
				RenderUtils.SetStringAttribute(InputControl, "size", Edit.Size.ToString());
			RenderUtils.SetAttribute(InputControl, "value", CommonUtils.ValueToString(Edit.GetInputText()), "");
			if(Edit.SendValueToServer && !string.IsNullOrEmpty(Edit.UniqueID))
				RenderUtils.SetStringAttribute(InputControl, "name", Edit.UniqueID);
			if(Edit.UseReadOnlyForDisabled()) {
				if(Edit.ReadOnly || !Edit.IsUserInputAllowed() || !Edit.IsEnabled())
					RenderUtils.SetStringAttribute(InputControl, "readonly", "readonly");
			}
			else {
				if(!Edit.IsEnabled())
					RenderUtils.SetStringAttribute(InputControl, "disabled", "disabled");
				if(Edit.ReadOnly || !Edit.IsUserInputAllowed())
					RenderUtils.SetStringAttribute(InputControl, "readonly", "readonly");
			}
			if(!DesignMode) {
				if(IsEnabled() || Edit.NeedFocusCorrectionWhenDisabled()) {
					RenderUtils.SetStringAttribute(InputControl, "onfocus", Edit.GetOnGotFocus());
					RenderUtils.SetStringAttribute(InputControl, "onblur", Edit.GetOnLostFocus());
				}
				if(IsEnabled())
					RenderUtils.SetStringAttribute(InputControl, "onchange", Edit.GetOnTextChanged());
			}
		}
	}
	public class MemoControl : TableBasedControl {
		private TextAreaControl textArea = null;
		protected TextAreaControl TextArea {
			get { return textArea; }
		}
		public MemoControl(ASPxMemo edit)
			: base(edit) {
		}
		protected new ASPxMemo Edit {
			get { return (ASPxMemo)base.Edit; }
		}
		private bool IsTextAreaStretched {
			get { return Edit.Columns == 0; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.textArea = null;
		}
		protected override void CreateMainCellContent(TableCell mainCell) {
			this.textArea = new TextAreaControl(Edit.IsRightToLeft());
			mainCell.Controls.Add(this.textArea);
		}
		protected override void PrepareMainCell(TableCell editCell) {
			base.PrepareMainCell(editCell);
			editCell.Width = IsTextAreaStretched ? Unit.Percentage(100) : Unit.Empty;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if (DesignMode)
				RenderUtils.SetPaddings(MainCell, Unit.Empty, 1, Unit.Empty, Unit.Empty);
			if(Edit.MaxLengthScriptNativeImplementation()) {
				RenderUtils.SetStringAttribute(this.textArea, "maxlength", Edit.MaxLength.ToString());
			}
		}
		protected override void PrepareMainCellContent() {
			this.textArea.ID = ASPxTextEdit.InputControlSuffix;
			if(Edit.SendValueToServer)
				this.textArea.Name = Edit.UniqueID;
			this.textArea.AccessKey = Edit.AccessKey;
			this.textArea.TabIndex = IsEnabled() ? Edit.TabIndex : (short)-1;
			this.textArea.Height = (Edit.Height.Type == UnitType.Percentage) ? Unit.Percentage(100) : Edit.Height;
			if(IsTextAreaStretched)
				this.textArea.Width = Unit.Percentage(100);
			this.textArea.Columns = Edit.Columns;
			this.textArea.Rows = Edit.Rows;
			AppearanceStyleBase editStyle = Edit.GetControlStyle();
			AppearanceStyleBase style = Edit.GetMemoEditAreaStyle();
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, EditorStyles.MemoEditAreaSystemClassName);			
			style.BackColor = editStyle.BackColor;
			style.ForeColor = editStyle.ForeColor;
			style.Cursor = editStyle.Cursor;
			this.textArea.ControlStyle.Assign(style);
			if(Edit.UseReadOnlyForDisabled())
				this.textArea.IsReadOnly = Edit.ReadOnly || !Edit.IsEnabled();
			else {
				this.textArea.Enabled = Edit.IsEnabled();
				this.textArea.IsReadOnly = Edit.ReadOnly;
			}
			if(IsEnabled() || Edit.NeedFocusCorrectionWhenDisabled()) {
				this.textArea.OnGotFocus = Edit.GetOnGotFocus();
				this.textArea.OnBlur = Edit.GetOnLostFocus();
			}
			if(IsEnabled()) {
				this.textArea.OnChange = Edit.GetOnTextChanged();
				this.textArea.OnMouseOut = Edit.GetOnMouseOut();
				this.textArea.OnMouseOver = Edit.GetOnMouseOver();
			}
			this.textArea.Text = Edit.HtmlEncode(Edit.GetInputText());
		}
	}
	public class MemoNativeControl : ASPxInternalWebControl {
		private ASPxMemo edit = null;
		private WebControl textAreaControl = null;
		private LiteralControl textControl = null;
		public MemoNativeControl(ASPxMemo edit)
			: base() {
			this.edit = edit;
		}
		protected ASPxMemo Edit {
			get { return edit; }
		}
		protected WebControl TextAreaControl {
			get { return textAreaControl; }
		}
		protected LiteralControl TextControl {
			get { return textControl; }
		}
		protected override void ClearControlFields() {
			this.textAreaControl = null;
			this.textControl = null;
		}
		protected override void CreateControlHierarchy() {
			this.textAreaControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Textarea);
			Controls.Add(TextAreaControl);
			this.textControl = RenderUtils.CreateLiteralControl();
			TextAreaControl.Controls.Add(TextControl);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(Edit, TextAreaControl);
			RenderUtils.SetVisibility(TextAreaControl, Edit.IsClientVisible(), true);
			Edit.GetControlStyle().AssignToControl(TextAreaControl);
			TextAreaControl.Height = Edit.Height;
			TextAreaControl.Width = Edit.Width;
			if(Edit.MaxLengthScriptNativeImplementation()) {
				RenderUtils.SetStringAttribute(TextAreaControl, "maxlength", Edit.MaxLength.ToString());
			}
			RenderUtils.PrepareTextArea(TextAreaControl, Edit.Columns, Edit.Rows);
			if(Edit.SendValueToServer && !string.IsNullOrEmpty(Edit.UniqueID))
				RenderUtils.SetStringAttribute(TextAreaControl, "name", Edit.UniqueID);
			if(Edit.UseReadOnlyForDisabled()) {
				if(Edit.ReadOnly || !Edit.IsEnabled())
					RenderUtils.SetStringAttribute(TextAreaControl, "readonly", "readonly");
			} else {
				if(!Edit.IsEnabled())
					RenderUtils.SetStringAttribute(TextAreaControl, "disabled", "disabled");
				if(Edit.ReadOnly)
					RenderUtils.SetStringAttribute(TextAreaControl, "readonly", "readonly");
			}
			if(!DesignMode) {
				if(IsEnabled() || Edit.NeedFocusCorrectionWhenDisabled()) {
					RenderUtils.SetStringAttribute(TextAreaControl, "onfocus", Edit.GetOnGotFocus());
					RenderUtils.SetStringAttribute(TextAreaControl, "onblur", Edit.GetOnLostFocus());
				}
				if(IsEnabled()) {
					RenderUtils.SetStringAttribute(TextAreaControl, "onchange", Edit.GetOnTextChanged());
					RenderUtils.SetStringAttribute(TextAreaControl, "onmouseout", Edit.GetOnMouseOut());
					RenderUtils.SetStringAttribute(TextAreaControl, "onmouseover", Edit.GetOnMouseOver());
				}
			}
			TextControl.Text = Edit.HtmlEncode(Edit.GetInputText());
		}
	}
	public class ButtonCell : InternalTableCell {
		private string text = "";
		private string imageID = "";
		private ImagePropertiesBase image = null;
		private ImagePosition imagePosition = ImagePosition.Left;
		private Unit imageSpacing = Unit.Empty;
		private EditButtonStyle buttonStyle = new EditButtonStyle();
		private bool encodeHtml = false;
		private bool enabled = true;
		bool rtl;
		private TemplateContainerBase templateContainer = null;
		private string onClickScript = "";
		private string onFocusScript = "";
		private string onLoadImageScript = "";
		private string onLostFocusScript = "";
		private string onMouseDownScript = "";
		private string onMouseOutScript = "";
		private string onMouseUpScript = "";
		private SimpleButtonControl button = null;
		public ButtonCell()
			: this(string.Empty) {
		}
		public ButtonCell(string text)
			: this(text, new ImageProperties (), ImagePosition.Left) {
		}
		public ButtonCell(string text, ImagePropertiesBase image, ImagePosition imagePosition)
			: this(text , image , imagePosition , null) {
		}
		public ButtonCell(string text, ImagePropertiesBase image, ImagePosition imagePosition,
			TemplateContainerBase templateContainer)
			: this(text, image, imagePosition, templateContainer, false) {
		}
		public ButtonCell(string text, ImagePropertiesBase image, ImagePosition imagePosition,
			TemplateContainerBase templateContainer, bool rtl)
			: base() {
			this.text = text;
			this.image = image;
			this.imagePosition = imagePosition;
			this.templateContainer = templateContainer;
			this.rtl = rtl;
		}
		public ButtonCell(EditButton button, ASPxButtonEditBase edit, TemplateContainerBase templateContainer, bool isRightToLeft)
			: base() {
			this.text = button.Text;
			this.image = edit.GetButtonImage(button);
			this.imagePosition = button.ImagePosition;
			this.templateContainer = templateContainer;
			this.rtl = isRightToLeft;
		}
		public EditButtonStyle ButtonStyle {
			get { return buttonStyle; }
		}
		public bool EncodeHtml {
			get { return encodeHtml; }
			set { encodeHtml = value; }
		}
		public new bool Enabled {
			get { return enabled; }
			set { enabled = value; }
		}
		protected bool IsRightToLeft { get { return rtl; } set { rtl = value; } }
		public new string Text {
			get { return text; }
			set { text = value; }
		}
		public string ImageID {
			get { return imageID; }
			set { imageID = value; }
		}
		public ImagePropertiesBase Image {
			get { return image; }
			set { image = value; }
		}
		public ImagePosition ImagePosition {
			get { return imagePosition; }
			set { imagePosition = value; }
		}
		public Unit ImageSpacing {
			get { return imageSpacing; }
			set { imageSpacing = value; }
		}
		public TemplateContainerBase TemplateContainer {
			get { return templateContainer; }
			set { templateContainer = value; }
		}
		protected SimpleButtonControl Button {
			get { return button; }
		}
		public string OnClickScript {
			get { return onClickScript; }
			set { onClickScript = value; }
		}
		public string OnLoadImageScript {
			get { return onLoadImageScript; }
			set { onLoadImageScript = value; }
		}
		public string OnLostFocusScript {
			get { return onLostFocusScript; }
			set { onLostFocusScript = value; }
		}
		public string OnFocusScript {
			get { return onFocusScript; }
			set { onFocusScript = value; }
		}
		public string OnMouseDownScript {
			get { return onMouseDownScript; }
			set { onMouseDownScript = value; }
		}
		public string OnMouseOutScript {
			get { return onMouseOutScript; }
			set { onMouseOutScript = value; }
		}
		public string OnMouseUpScript {
			get { return onMouseUpScript; }
			set { onMouseUpScript = value; }
		}
		protected override void ClearControlFields() {
			this.button = null;
		}
		protected override void CreateControlHierarchy() {
			if(TemplateContainer != null) {
				Controls.Add(TemplateContainer);
			} else {
				this.button = new SimpleButtonControl(GetText(), Image, ImagePosition, string.Empty);
				Controls.Add(Button);
			}
		}
		protected override void PrepareControlHierarchy() {
			ButtonStyle.AssignToControl(this, true);
			if(Width.IsEmpty && !ButtonStyle.Width.IsEmpty)
				Width = ButtonStyle.Width;
			if(!Width.IsEmpty) {
				Style.Add("min-width", Width.ToString());
				Width = Unit.Empty;
			}
			if(Enabled) {
				if(OnClickScript != "")
					RenderUtils.SetStringAttribute(this, "onclick", OnClickScript);
				if(OnFocusScript != "")
					RenderUtils.SetStringAttribute(this, "onfocus", OnFocusScript);
				if(OnLostFocusScript != "")
					RenderUtils.SetStringAttribute(this, "onblur", OnLostFocusScript);
				if(OnMouseDownScript != "")
					RenderUtils.SetStringAttribute(this, TouchUtils.TouchMouseDownEventName, OnMouseDownScript);
				if(OnMouseOutScript != "")
					RenderUtils.SetStringAttribute(this, "onmouseout", OnMouseOutScript);
				if(OnMouseUpScript != "")
					RenderUtils.SetStringAttribute(this, "onmouseup", OnMouseUpScript);
			}
			if(Button != null) {
				Button.ButtonImageSpacing = ImageSpacing;
				Button.ButtonImageID = ImageID;
				Button.ButtonStyle = ButtonStyle;
				Button.ButtonText = GetText();
				Button.Enabled = Enabled;
				Button.IsRightToLeft = IsRightToLeft;
			}
		}
		internal void PrepareButtonImage(ButtonImageProperties buttonImageProperties) { 
			if(Button != null)
				Button.ButtonImage = buttonImageProperties;
		}
		private string GetText() {
			return EncodeHtml ? HttpUtility.HtmlEncode(Text) : Text;
		}
	}
	public class ButtonEditControl : TextBoxControlBase {
		private Dictionary<EditButton, ButtonCell> buttonCells;
		public ButtonEditControl(ASPxButtonEditBase buttonEdit)
			: base(buttonEdit) {
		}
		protected new ASPxButtonEditBase Edit {
			get { return (ASPxButtonEditBase)base.Edit; }
		}
		protected Dictionary<EditButton, ButtonCell> ButtonCells {
			get { return buttonCells; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.buttonCells = null;
		}
		protected override void CreateCells(TableRow row) {
			this.buttonCells = new Dictionary<EditButton, ButtonCell>();
			CreateButtonCells(ButtonsPosition.Left, row);
			CreateImageCellBeforeImput(row);
			base.CreateCells(row);
			CreateButtonCells(ButtonsPosition.Right, row);
			if(row.Cells.Count == 1 && DesignMode)
				row.Cells.Add(RenderUtils.CreateTableCell());
		}
		protected virtual void CreateImageCellBeforeImput(TableRow row) {}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareButtons();
			if(!RenderUtils.Browser.Platform.IsTouchUI && Edit.Properties.ClearButtonDisplayModeInternal == ClearButtonDisplayMode.OnHover) {
				string onMouseOver = string.Format("ASPx.BEMouseOver('{0}', event)", Edit.ClientID);
				string onMouseOut = string.Format("ASPx.BEMouseOut('{0}', event)", Edit.ClientID);
				RenderUtils.SetStringAttribute(Table, "onmouseover", onMouseOver);
				RenderUtils.SetStringAttribute(Table, "onmouseout", onMouseOut);
			}
		}
		protected override void PrepareInputControl(InputControl input) {
			base.PrepareInputControl(input);
			if(((Browser.IsIE && Browser.Version >= 10) || Browser.IsEdge) && RequireHideDefaultIEClearButton())
				input.ControlStyle.CssClass = RenderUtils.CombineCssClasses(input.ControlStyle.CssClass, EditorStyles.HideDefaultIEClearButton);
		}
		protected virtual bool RequireHideDefaultIEClearButton() {
			return Edit.Properties.IsClearButtonVisible();
		}
		protected virtual void PrepareButtons() {
			foreach(EditButton button in ButtonCells.Keys)
				PrepareButton(button, ButtonCells[button]);
		}
		protected bool ShowButtonToTheLeft(ButtonsPosition position) {
			return (!IsRightToLeft && position == ButtonsPosition.Left) || (IsRightToLeft && position == ButtonsPosition.Right);
		}
		protected override void PrepareTable(Table table) {
			base.PrepareTable(table);
			PrepareTableSpacing(table, Edit.ActualCellSpacing);
		}
		protected virtual List<EditButton> GetVisibleButtonsByPosition(ButtonsPosition position) {
			return Edit.GetButtons().Where(x => x.Visible && x.Position == position).ToList();
		}
		protected virtual void CreateButtonCells(ButtonsPosition position, Action<EditButton> creator) {
			var buttons = Edit.GetButtons().Where(x => x.Visible && x.Position == position).ToList();
			buttons.ForEach(creator);
		}
		protected virtual void CreateButtonCells(ButtonsPosition position, TableRow row) {
			ITemplate buttonTemplate = Edit.GetButtonTemplate();
			CreateButtonCells(position, button => {
				TemplateContainerBase templateContainer = null;
				if(buttonTemplate != null) {
					templateContainer = new TemplateContainerBase(button.Index, button);
					buttonTemplate.InstantiateIn(templateContainer);
				}
				CreateButtonCell(row, button, templateContainer);
			});
		}
		protected internal virtual void CreateButtonCell(TableRow row, EditButton button, TemplateContainerBase templateContainer) {
			ButtonCell cell = new ButtonCell(button, Edit, templateContainer, IsRightToLeft);
			ButtonCells.Add(button, cell);
			row.Cells.Add(cell);
		}
		protected virtual void PrepareButton(EditButton button, ButtonCell cell) {
			EditButtonStyle style = Edit.GetButtonStyle(button);
			cell.ButtonStyle.Assign(style);
			cell.Width = Edit.GetButtonWidth(button);
			cell.ImageID = Edit.GetButtonImageID(button);
			cell.ImageSpacing = style.ImageSpacing;
			cell.ID = Edit.GetButtonID(button);
			cell.Enabled = Edit.IsEnabled() && button.Enabled;
			cell.Text = !Edit.EncodeHtml ? button.Text : HttpUtility.HtmlEncode(button.Text);
			cell.ToolTip = button.ToolTip;
			cell.PrepareButtonImage(Edit.GetButtonImage(button));
			RenderUtils.SetVerticalAlign(cell, style.VerticalAlign);
			PrepareButtonVisibility(button, cell);
			RenderUtils.AppendDefaultDXClassName(cell, ShowButtonToTheLeft(button.Position) ? EditorStyles.ButtonEditButtonLeftSystemClassName :
				EditorStyles.ButtonEditButtonSystemClassName);
			PrepareButtonClientSideEventHandlers(button, cell);
		}
		protected void PrepareButtonVisibility(EditButton button, ButtonCell cell) {
			if(button is ClearButton && Edit.Properties.ClearButtonDisplayModeInternal == ClearButtonDisplayMode.Always)
				RenderUtils.AppendDefaultDXClassName(cell, "dxHideContent");
			else
				RenderUtils.SetVisibility(cell, button.GetClientVisible(), true);
		}
		protected virtual void PrepareButtonClientSideEventHandlers(EditButton button, ButtonCell cell) {
			cell.OnClickScript = Edit.GetButtonOnClick(button);
			cell.OnMouseDownScript = Edit.GetButtonOnMouseDown(button);
		}
	}
}

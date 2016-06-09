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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web.Internal {
	public class CheckEditDisplayControl : ASPxInternalWebControl, IInternalCheckBoxOwner {
		private bool useImages = false;
		private string text = "";
		private InternalCheckboxControl checkBox = null;
		private LiteralControl literalControl = null;
		private CheckBoxProperties properties = null;
		private CreateDisplayControlArgs args = null;
		public CheckEditDisplayControl(CheckBoxProperties properties, CreateDisplayControlArgs args)
			: base() {
			this.properties = properties;
			this.args = args;
		}
		protected LiteralControl LiteralControl {
			get { return literalControl; }
		}
		protected InternalCheckboxControl CheckBox {
			get { return checkBox; }
		}
		public bool UseImages {
			get { return useImages; }
			set { useImages = value; }
		}
		public string Text {
			get { return text; }
			set { text = value; }
		}
		protected override void ClearControlFields() {
			this.literalControl = null;
			this.checkBox = null;
		}
		protected override void CreateControlHierarchy() {
			if(UseImages) {
				checkBox = new InternalCheckboxControl(this);
				Controls.Add(checkBox);
			} else {
				literalControl = RenderUtils.CreateLiteralControl();
				Controls.Add(LiteralControl);
			}
		}
		protected override void PrepareControlHierarchy() {
			if(LiteralControl != null)
				LiteralControl.Text = Text;
		}
		protected Dictionary<string, string> GetAccessibilityCheckBoxAttributes() {
			if(!(RenderUtils.IsHtml5Mode(this) && args.SkinOwner != null && args.SkinOwner.IsAccessibilityCompliant()))
				return null;
			Dictionary<string, string> settings = AccessibilityUtils.CreateCheckBoxAttributes("checkbox", CheckState);
			settings.Add("aria-disabled", "true");
			return settings;
		}
		#region IInternalCheckBoxOwner
		public CheckState CheckState {
			get {
				bool isAllowGrayed = this.properties.IsAllowGrayedInitialized ? this.properties.AllowGrayed : true;
				return this.properties.GetCheckState(args.EditValue, isAllowGrayed);
			}
		}
		public bool ClientEnabled {
			get { return true; }
		}
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() {
			return this.properties.GetImage(CheckState, Page);
		}
		bool IInternalCheckBoxOwner.IsInputElementRequired {
			get { return false; }
		}
		string IInternalCheckBoxOwner.GetCheckBoxInputID() {
			return string.Empty;
		}
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle {
			get { return this.properties.GetCheckBoxStyle(); }
		}
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes { get { return GetAccessibilityCheckBoxAttributes(); } }
		#endregion
	}
	public abstract class CheckBoxControlBase : ASPxInternalWebControl {
		private ASPxCheckBox edit = null;
		private WebControl checkableElement = null;
		private WebControl span = null;
		private WebControl label = null;
		private LiteralControl textControl = null;
		private Table table = null;
		private TableCell inputCell = null;
		private TableCell labelCell = null;
		private Image imageControl = null;
		public CheckBoxControlBase(ASPxCheckBox edit) {
			this.edit = edit;
		}
		protected ASPxCheckBox Edit {
			get { return edit; }
		}
		protected bool IsRightToLeft {
			get { return (Edit as ISkinOwner).IsRightToLeft(); }
		}
		protected WebControl CheckableElement {
			get { return checkableElement; }
		}		
		protected WebControl Span {
			get { return span; }
		}
		protected WebControl Label {
			get { return label; }
		}
		protected LiteralControl TextControl {
			get { return textControl; }
		}
		protected Table Table {
			get { return table; }
		}
		protected Image ImageControl {
			get { return imageControl; }
		}
		protected TableCell InputCell {
			get { return inputCell; }
		}
		protected override void ClearControlFields() {
			this.checkableElement = null;
			this.span = null;
			this.label = null;
			this.textControl = null;
			this.table = null;
			this.inputCell = null;
			this.labelCell = null;
			this.imageControl = null;
		}
		protected abstract WebControl CreateCheckableElement();
		protected abstract void PrepareCheckableElement();
		protected override void CreateControlHierarchy() {
			this.checkableElement = CreateCheckableElement();
			if(Edit.HasLabel()) {
				HtmlTextWriterTag labelTag = (Edit.IsAccessibilityCompliantRender() && !Edit.Native) ? HtmlTextWriterTag.Span : HtmlTextWriterTag.Label;
				this.label = RenderUtils.CreateWebControl(labelTag);
				if(Edit.HasImage()) {
					this.imageControl = RenderUtils.CreateImage();
					Label.Controls.Add(ImageControl);
				}
				if(Edit.HasText()) {
					this.textControl = RenderUtils.CreateLiteralControl();
					Label.Controls.Add(TextControl);
				}
			}
			if(Edit.HasTable()) {
				this.table = RenderUtils.CreateTable();
				Controls.Add(Table);
				CreateTableHierarchy(Table);
			} else {
				if(Edit.HasSpan()) {
					this.span = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
					Controls.Add(Span);
					CreateFlowHierarchy(Span);
				} else {
					CreateFlowHierarchy(this);
				}
			}
		}
		protected internal virtual void CreateTableHierarchy(Table parentTable) {
			TableRow row = RenderUtils.CreateTableRow();
			parentTable.Rows.Add(row);
			this.labelCell = RenderUtils.CreateTableCell();
			this.inputCell = RenderUtils.CreateTableCell();
			this.inputCell.Controls.Add(CheckableElement);
			this.labelCell.Controls.Add(Label);
			if(Edit.TextAlign == TextAlign.Left)
				row.Cells.Add(this.labelCell);
			row.Cells.Add(this.inputCell);
			if(Edit.TextAlign == TextAlign.Right)
				row.Cells.Add(this.labelCell);
		}
		protected internal virtual void CreateFlowHierarchy(Control parent) {
			if(Edit.HasLabel()) {
				if(Edit.TextAlign == TextAlign.Left)
					parent.Controls.Add(Label);
				parent.Controls.Add(CheckableElement);
				if(Edit.TextAlign == TextAlign.Right)
					parent.Controls.Add(Label);
			} else 
				parent.Controls.Add(CheckableElement);
		}
		protected string GetTextAlignClassName() {
			return Edit.TextAlign == TextAlign.Left ? "dxeTAL" : "dxeTAR";
		}
		protected override void PrepareControlHierarchy() {
			PrepareCheckableElement();
			if(Table != null) {
				PrepareTable(this.table);
				PrepareInputCell(this.inputCell);
				PrepareLabelCell(this.labelCell);
				RenderUtils.AppendDefaultDXClassName(Table, GetTextAlignClassName());
			} else if(Span != null) {
				RenderUtils.AssignAttributes(Edit, Span, false, true);
				Span.AccessKey = "";
				Edit.RemoveImportedStyleAttrsFromMainElement(Span);
				RenderUtils.SetVisibility(Span, Edit.IsClientVisible(), true);
				RenderUtils.MoveTabIndexToInput(Span, CheckableElement);
				Edit.GetControlStyle().AssignToControl(Span);
				if(Edit.Layout == RepeatLayout.Flow)
					RenderUtils.SetWrap(span, Edit.Wrap);
				RenderUtils.AppendDefaultDXClassName(Span, GetTextAlignClassName());
			}			
			if (Label != null)
				PrepareLabel();
			if(ImageControl != null)
				Edit.Image.AssignToControl(ImageControl, DesignMode);
			if(TextControl != null)
				TextControl.Text = Edit.GetText();
		}
		protected void PrepareTable(Table table) {
			RenderUtils.AssignAttributes(Edit, table, false);
			table.AccessKey = "";
			Edit.RemoveImportedStyleAttrsFromMainElement(table);
			RenderUtils.SetVisibility(table, Edit.IsClientVisible(), true);
			Edit.GetControlStyle().AssignToControl(table, AttributesRange.Common | AttributesRange.Font);
			CorrectCollapsedTableHeight(table, Edit.GetControlStyle());
		}
		protected virtual void PrepareInputCell(TableCell inputCell) {
			RenderUtils.MoveTabIndexToInput(Table, CheckableElement);
		}
		protected void PrepareLabelCell(TableCell labelCell) {
			if(!Edit.Width.IsEmpty)
				labelCell.Width = Unit.Percentage(100);
			RenderUtils.AppendDefaultDXClassName(labelCell, "dxichTextCellSys");
		}
		protected virtual void PrepareLabel() {
			Unit textSpacing = Edit.GetTextSpacing();
			if(textSpacing.Value == 0)
				textSpacing = Unit.Empty;
			if(Edit.TextAlign == TextAlign.Left ^ IsRightToLeft)
				RenderUtils.SetHorizontalMargins(Label, Unit.Empty, textSpacing);
			else
				RenderUtils.SetHorizontalMargins(Label, textSpacing, Unit.Empty);
			if(Edit.Layout == RepeatLayout.Table)
				RenderUtils.SetWrap(Label, Edit.Wrap);
			if(Edit.IsAccessibilityCompliantRender() && !Edit.Native)
				RenderUtils.AppendDefaultDXClassName(Label, AccessibilityUtils.DefaultCursorCssClassName);
		}
		private void CorrectCollapsedTableHeight(Table table, AppearanceStyleBase controlStyle) {
			if(Browser.Family.IsNetscape)
				table.Height = UnitUtils.GetCorrectedHeight(table.Height, controlStyle, Paddings.NullPaddings);
			else if(Browser.IsOpera) {
				Unit borderWidthBottom = controlStyle.GetBorderWidthBottom();
				Unit borderWidthTop = controlStyle.GetBorderWidthTop();
				if(borderWidthBottom.Type == borderWidthTop.Type && borderWidthBottom.Type == table.Height.Type) {
					double heightDelta = Math.Floor((borderWidthTop.Value + borderWidthBottom.Value) / 2);
					table.Height = (Unit)(table.Height.Value + heightDelta);
				}
			}
		}
	}
	public class CheckBoxControl : CheckBoxControlBase {
		public CheckBoxControl(ASPxCheckBox edit)
			: base(edit) {
		}
		protected override WebControl CreateCheckableElement() {
			return new InternalCheckboxControl(Edit);
		}
		protected override void PrepareCheckableElement() {
			if(!DesignMode)
				RenderUtils.SetStringAttribute((CheckableElement as InternalCheckboxControl).Input, "name", Edit.UniqueID);
			RenderUtils.AppendDefaultDXClassName((WebControl)CheckableElement.Parent, "dxichCellSys");
		}
	}
	public class CheckBoxNativeControl : CheckBoxControlBase {
		private WebControl stateInput = null;
		public CheckBoxNativeControl(ASPxCheckBox edit)
			: base(edit) {
		}
		protected override WebControl CreateCheckableElement() {
			WebControl input = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			input.ID = ASPxCheckBox.CheckBoxInputIDSuffix;
			return input;
		}
		protected WebControl StateInput {
			get { return stateInput; }
		}
		protected override void CreateControlHierarchy() {
			if(!Edit.UsingInsideList)
				this.stateInput = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			base.CreateControlHierarchy();
		}
		protected internal override void CreateTableHierarchy(Table parentTable) {
			base.CreateTableHierarchy(parentTable);
			if(StateInput != null)
				InputCell.Controls.Add(StateInput);
		}
		protected internal override void CreateFlowHierarchy(Control parent) {
			if(StateInput != null)
				parent.Controls.Add(StateInput);
			base.CreateFlowHierarchy(parent);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(StateInput != null) {
				StateInput.ID = ASPxCheckBox.StateInputIDSuffix;
				RenderUtils.PrepareInput(StateInput, "hidden", Edit.UniqueID, Edit.GetStatus());
			}
		}
		protected override void PrepareLabel() {
			RenderUtils.SetStringAttribute(Label, "for", CheckableElement.ClientID);
			base.PrepareLabel();
		}
		protected override void PrepareCheckableElement() {
			string value = Edit.GetInputValue();
			RenderUtils.PrepareInput(CheckableElement, Edit.GetInputType(), Edit.GetInputName(), value);
			CheckableElement.AccessKey = Edit.AccessKey;
			if (string.IsNullOrEmpty(value))
				CheckableElement.Attributes.Add("value", "");
			RenderUtils.SetAttribute(CheckableElement, "checked", "checked", (Edit.Checked ? "" : "checked"));
			if (IsEnabled()) {
				if (!DesignMode) {
					RenderUtils.SetStringAttribute(CheckableElement, "onclick", Edit.GetOnClick());
					RenderUtils.SetStringAttribute(CheckableElement, "onfocus", Edit.GetOnGotFocus());
					RenderUtils.SetStringAttribute(CheckableElement, "onblur", Edit.GetOnLostFocus());
				}
			}
			else
				RenderUtils.SetStringAttribute(CheckableElement, "disabled", "disabled");
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.stateInput = null;
		}
	}
	public class RadioButtonNativeControl : CheckBoxNativeControl {
		public RadioButtonNativeControl(ASPxRadioButton edit)
			: base(edit) {
		}
		private void PrepareInput() {
			if(Edit.Layout == RepeatLayout.Flow)
				return;
			if(Browser.Family.IsNetscape || Browser.IsChrome)
				RenderUtils.SetMargins(CheckableElement, 4, 4, 3, 5);
			else if(Browser.IsIE)
				RenderUtils.SetMargins(CheckableElement, 0, 0, 0, 3);
			else if(Browser.IsOpera)
				RenderUtils.SetMargins(CheckableElement, 3, 3, 1, 4);
			else if(Browser.IsSafari)
				RenderUtils.SetMargins(CheckableElement, 4, 5, 4, 5);
			CheckableElement.Attributes["name"] = Edit.GetInputName();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareInput();
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
		}
	}
	public class RadioButtonControl : CheckBoxControl {
		public RadioButtonControl(ASPxRadioButton edit)
			: base(edit) {
		}
	}
}

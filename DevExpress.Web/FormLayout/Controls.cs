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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
using System.ComponentModel;
namespace DevExpress.Web.Internal {
	public class FormLayoutControl : ASPxInternalWebControl {
		ASPxFormLayout layout = null;
		WebControl rootItemElement = null,
			mainElement = null;
		public FormLayoutControl(ASPxFormLayout layout) {
			this.layout = layout;
		}
		public ASPxFormLayout Layout {
			get { return layout; }
		}
		protected override void ClearControlFields() {
			this.rootItemElement = null;
			this.mainElement = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.rootItemElement = FormLayoutRenderHelper.CreateLayoutElement(Layout.Root);
			this.mainElement = RenderUtils.CreateDiv();
			this.mainElement.Controls.Add(this.rootItemElement);
			this.Controls.Add(this.mainElement);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(Layout, this.mainElement);
			this.mainElement.TabIndex = 0;
			this.mainElement.AccessKey = string.Empty;
			Layout.GetControlStyle().AssignToControl(this.mainElement);
			Layout.Paddings.AssignToControl(this.mainElement);
			if(Layout.IsFlowRender && Layout.Width.IsEmpty)
				this.mainElement.Width = Unit.Percentage(100);
			if(Layout.ContainsGroupsWithoutDefaultPaddings())
				RenderUtils.AppendDefaultDXClassName(this.mainElement, ((ISkinOwner)Layout).IsRightToLeft() ? "dxflRTL" : "dxflLTR");
		}
	}
	public class InternalGroupBox : ASPxInternalWebControl {
		LayoutGroup group = null;
		WebControl mainElement = null,
			captionElement = null,
			content = null;
		LiteralControl captionLiteralControl = null;
		public InternalGroupBox(LayoutGroup group)  {
			this.group = group;
		}
		public LayoutGroup LayoutGroup {
			get { return this.group; }
		}
		public string Caption {
			get { return LayoutGroup.GetItemCaption(); }
		}
		protected override void ClearControlFields() {
			this.mainElement = null;
			this.captionElement = null;
			this.captionLiteralControl = null;
		}
		public WebControl Content {
			set { this.content = value; }
			get { return this.content; }
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.mainElement = RenderUtils.CreateDiv();
			if (LayoutGroup.GetShowCaption()) {
				this.captionElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				this.captionLiteralControl = new LiteralControl();
				this.captionElement.Controls.Add(this.captionLiteralControl);
				this.mainElement.Controls.Add(this.captionElement);
			}
			this.mainElement.Controls.Add(Content);
			Controls.Add(this.mainElement);
		}
		protected override void PrepareControlHierarchy() {
			if (this.captionLiteralControl != null)
				this.captionLiteralControl.Text = LayoutGroup.FormLayout.HtmlEncode(Caption);
			LayoutGroupBoxStyle groupBoxStyle = LayoutGroup.GetGroupBoxStyle();
			groupBoxStyle.AssignToControl(this.mainElement);
			groupBoxStyle.Paddings.AssignToControl(this.mainElement);
			this.mainElement.Height = Height;
			this.mainElement.Width = Width;
			if (LayoutGroup.GetShowCaption()) {
				LayoutGroupBoxCaptionStyle groupBoxCaptionStyle = LayoutGroup.GetGroupBoxStyle().Caption;
				groupBoxCaptionStyle.AssignToControl(this.captionElement);
				groupBoxCaptionStyle.Paddings.AssignToControl(this.captionElement);
			}
			if (DesignMode)
				PrepareDesignTimeControlHierarchy();
		}
		protected void PrepareDesignTimeControlHierarchy() {
			this.mainElement.Style[HtmlTextWriterStyle.MarginRight] = "12px";
		}
	}
	public abstract class LayoutItemControlBase : ASPxInternalWebControl {
		LayoutItemBase layoutItem = null;
		public LayoutItemControlBase(LayoutItemBase layoutItem) {
			this.layoutItem = layoutItem;
		}
		public LayoutItemBase LayoutItem {
			get { return layoutItem; }
		}
		public LayoutGroupBase ParentGroup {
			get { return LayoutItem.ParentGroupInternal; }
		}
		public ASPxFormLayout FormLayout {
			get { return LayoutItem.FormLayout; }
		}
		protected override void PrepareControlHierarchy() {
			WebControl mainElement = GetMainElement();
			Unit itemWidth = LayoutItem.GetWidth();
			Unit itemHeight = LayoutItem.GetHeight();
			mainElement.Width = GetMainElementWidth(itemWidth);
			mainElement.Height = NeedSetHundredPercentHeight(itemHeight) ? Unit.Percentage(100) : itemHeight;
		}
		protected abstract WebControl GetMainElement();
		protected Unit GetMainElementWidth(Unit itemWidth) {
			if(LayoutItem.ParentGroupInternal is LayoutGroup && itemWidth.Type == UnitType.Percentage) {
				bool itemHasHorizontalAlign = LayoutItem.GetHorizontalAlign() != FormLayoutHorizontalAlign.NotSet;
				bool isFlowRender = LayoutItem.Owner.IsFlowRender;
				return itemHasHorizontalAlign && isFlowRender ? Unit.Empty : Unit.Percentage(100);
			}
			return itemWidth;
		}
		protected bool NeedSetHundredPercentHeight(Unit itemHeight) {
			return LayoutItem.ParentGroupInternal is LayoutGroup && itemHeight.Type == UnitType.Percentage;
		}
	}
	public abstract class LayoutItemControl : LayoutItemControlBase {
		WebControl actualCaptionElement = null;
		WebControl captionLabelElement = null;
		WebControl captionSpanElement = null;
		WebControl requiredMark = null;
		WebControl optionalMark = null;
		WebControl nestedControlCell = null,
			captionCell = null,
			internalNestedControlCell = null,
			helpTextCell = null,
			mainElement = null;
		WebControl internalNestedControlTable = null,
			itemTable = null;
		LiteralControl actualCaptionLiteralControl = null;
		LiteralControl captionSpanLiteralControl = null;
		LiteralControl captionLabelLiteralControl = null;
		LiteralControl requiredMarkLiteralControl = null;
		LiteralControl optionalMarkLiteralControl = null;
		LiteralControl helpTextLiteralControl = null;
		public LayoutItemControl(LayoutItem layoutItem)
			: base(layoutItem) {
		}
		public new LayoutItem LayoutItem {
			get { return base.LayoutItem as LayoutItem; }
		}
		protected WebControl CaptionCell {
			get { return this.captionCell; }
			set { this.captionCell = value; }
		}
		protected WebControl NestedControlCell {
			get { return this.nestedControlCell; }
			set { this.nestedControlCell = value; }
		}
		protected WebControl MainElement {
			get { return this.mainElement; }
			set { this.mainElement = value; }
		}
		protected WebControl ItemTable {
			get { return this.itemTable; }
			set { this.itemTable = value; }
		}
		protected WebControl InternalNestedControlCell {
			get { return this.internalNestedControlCell; }
			set { this.internalNestedControlCell = value; }
		}
		protected WebControl HelpTextCell {
			get { return this.helpTextCell; }
			set { this.helpTextCell = value; }
		}
		protected WebControl InternalNestedControlTable {
			get { return this.internalNestedControlTable; }
			set { this.internalNestedControlTable = value; }
		}
		protected override void ClearControlFields() {
			this.nestedControlCell = null;
			this.captionCell = null;
			this.internalNestedControlCell = null;
			this.helpTextCell = null;
			this.internalNestedControlTable = null;
			this.itemTable = null;
			this.mainElement = null;
			this.actualCaptionElement = null;
			this.requiredMark = null;
			this.optionalMark = null;
			this.actualCaptionLiteralControl = null;
			this.requiredMarkLiteralControl = null;
			this.optionalMarkLiteralControl = null;
			this.helpTextLiteralControl = null;
			this.captionLabelElement = null;
			this.captionSpanElement = null;
			this.captionLabelLiteralControl = null;
			this.captionSpanLiteralControl = null;
		}
		private void CreateCaptionElements() {
			this.captionLabelElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Label);
			this.captionSpanElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			this.captionLabelLiteralControl = new LiteralControl();
			this.captionSpanLiteralControl = new LiteralControl();
			this.captionLabelElement.Controls.Add(this.captionLabelLiteralControl);
			this.captionSpanElement.Controls.Add(this.captionSpanLiteralControl);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if (LayoutItem.IsCaptionCellRequired()) {
				CreateLayoutItemTable();
				if(LayoutItem.GetShowCaption()) {
					CreateCaptionElements();
					captionCell.Controls.Add(this.captionLabelElement);
					captionCell.Controls.Add(this.captionSpanElement);
				}
				if(FormLayout.ShowItemRequiredMark(LayoutItem)) {
					this.requiredMark = RenderUtils.CreateWebControl(HtmlTextWriterTag.Em);
					this.requiredMarkLiteralControl = new LiteralControl();
					this.requiredMark.Controls.Add(this.requiredMarkLiteralControl);
					captionCell.Controls.Add(this.requiredMark);
				}
				if(FormLayout.ShowItemOptionalMark(LayoutItem)) {
					this.optionalMark = RenderUtils.CreateWebControl(HtmlTextWriterTag.Em);
					this.optionalMarkLiteralControl = new LiteralControl();
					this.optionalMark.Controls.Add(this.optionalMarkLiteralControl);
					captionCell.Controls.Add(this.optionalMark);
				}
				this.Controls.Add(this.mainElement);
			} else
				AddLayoutItemElementWithoutCaptionCell();
			ResetNestedControlContainerID();
			this.nestedControlCell.Controls.Add(GetNestedControlCellContent());
			if (!DesignMode)
				EnsureNestedControlIdAssigned();
		}
		protected void AddLayoutItemElementWithoutCaptionCell() {
			if(LayoutItem.NeedAdditionalTableForRender()) {
				Table table = RenderUtils.CreateTable();
				TableRow row = RenderUtils.CreateTableRow();
				this.nestedControlCell = RenderUtils.CreateTableCell();
				table.Rows.Add(row);
				row.Cells.Add(this.nestedControlCell as TableCell);
				this.Controls.Add(table);
			}
			else {
				this.nestedControlCell = RenderUtils.CreateDiv();
				this.Controls.Add(this.nestedControlCell);
			}
		}
		protected HtmlTextWriterTag GetCaptionElementTag() {
			return HtmlTextWriterTag.Label;
		}
		protected void ResetNestedControlContainerID() {
			LayoutItem.NestedControlContainer.ID = null;
		}
		protected bool IsHelpTextExists() {
			return !string.IsNullOrEmpty(LayoutItem.HelpText);
		}
		protected WebControl GetNestedControlCellContent() {
			if (IsHelpTextExists()) {
				CreateInternalNestedControlTable();
				this.internalNestedControlCell.Controls.Add(LayoutItem.NestedControlContainer);
				WebControl helpTextElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Label);
				this.helpTextLiteralControl = new LiteralControl();
				helpTextElement.Controls.Add(this.helpTextLiteralControl);
				this.helpTextCell.Controls.Add(helpTextElement);
				return this.internalNestedControlTable;
			}
			return LayoutItem.NestedControlContainer;
		}
		protected abstract void CreateInternalNestedControlTable();
		protected abstract void CreateLayoutItemTable();
		protected override WebControl GetMainElement() {
			return this.mainElement != null ? this.mainElement : this.nestedControlCell;
		}
		private WebControl GetCaptionElement(string nestedControlID) {
			return string.IsNullOrEmpty(nestedControlID) ? this.captionSpanElement : this.captionLabelElement;
		}
		private LiteralControl GetCaptionLiteralControl() {
			return this.actualCaptionElement == this.captionLabelElement ? this.captionLabelLiteralControl : this.captionSpanLiteralControl;
		}
		private void PrepareCaptionElements() {
			string nestedControlID = LayoutItem.GetNestedControlID();
			this.actualCaptionElement = GetCaptionElement(nestedControlID);
			if(this.actualCaptionElement == null)
				return;
			this.actualCaptionLiteralControl = GetCaptionLiteralControl();
			if(this.actualCaptionElement == this.captionLabelElement) {
				this.captionSpanElement.Visible = false;
				RenderUtils.SetStringAttribute(this.actualCaptionElement, "for", nestedControlID);
			}
			else
				this.captionLabelElement.Visible = false;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if (DesignMode)
				EnsureNestedControlIdAssigned();
			PrepareCaptionElements();
			SetLiteralControlsText();
			LayoutItemStyle layoutItemStyle = LayoutItem.GetLayoutItemStyle();
			WebControl mainElement = GetMainElement();
			layoutItemStyle.AssignToControl(mainElement);
			layoutItemStyle.Paddings.AssignToControl(mainElement);
			if (this.actualCaptionElement != null)
				LayoutItem.GetCaptionStyle().AssignToControl(this.actualCaptionElement);
			PrepareNestedControl();
			if(LayoutItemTableExists())
				PrepareLayoutItemTable();
			if(IsHelpTextExists())
				PrepareInternalNestedControlTable();
			if(LayoutItem.AllowEllipsisInText) {
				RenderUtils.AllowEllipsisInText(this.nestedControlCell, true);
				if(LayoutItem.IsCaptionCellRequired())
					MainElement.Style["table-layout"] = "fixed";
			}
		}
		protected void SetLiteralControlsText() {
			if(this.actualCaptionLiteralControl != null)
				this.actualCaptionLiteralControl.Text = FormLayout.HtmlEncode(LayoutItem.GetItemCaption());
			if(this.requiredMarkLiteralControl != null)
				this.requiredMarkLiteralControl.Text = FormLayout.HtmlEncode(FormLayout.RequiredMark);
			if (this.optionalMarkLiteralControl != null)
				this.optionalMarkLiteralControl.Text = FormLayout.HtmlEncode(FormLayout.OptionalMark);
			if(this.helpTextLiteralControl != null)
				this.helpTextLiteralControl.Text = FormLayout.HtmlEncode(LayoutItem.HelpText);
		}
		protected bool LayoutItemTableExists() {
			return this.captionCell != null;
		}
		protected virtual void PrepareInternalNestedControlTable() {
			LayoutItem.GetInternalNestedControlTableStyle().AssignToControl(this.internalNestedControlTable);
			LayoutItem.GetLayoutItemHelpTextStyle().AssignToControl(this.helpTextCell);
			WebControl nestedControl = LayoutItem.GetNestedControl() as WebControl;
			if(nestedControl != null && LayoutItem.FormLayout.NestedControlHasPercentageWidth(nestedControl))
				InternalNestedControlCell.Width = Unit.Percentage(100);
			else
				InternalNestedControlCell.Width = Unit.Pixel(1);
			if(NestedControlHasFullHeight()) {
				this.internalNestedControlTable.Height = Unit.Percentage(100);
			}
		}
		protected bool NestedControlHasFullHeight() {
			return LayoutItem.GetNestedControl() is WebControl && ((WebControl)LayoutItem.GetNestedControl()).Height == Unit.Percentage(100);
		}
		protected virtual void PrepareCaptionCell() {
			AppearanceStyle captionCellStyle = LayoutItem.GetCaptionCellStyle();
			captionCellStyle.AssignToControl(this.captionCell);
			captionCellStyle.Paddings.AssignToControl(this.captionCell);
		}
		protected virtual void PrepareNestedControlCell() {
			AppearanceStyle nestedControlCellStyle = LayoutItem.GetNestedControlCellStyle();
			nestedControlCellStyle.AssignToControl(this.nestedControlCell);
			nestedControlCellStyle.Paddings.AssignToControl(this.nestedControlCell);
		}
		protected virtual void PrepareLayoutItemTable() {
			if (this.itemTable != null)
				LayoutItem.GetItemTableStyle().AssignToControl(this.itemTable);
			PrepareCaptionCell();
			PrepareNestedControlCell();
			if(this.requiredMark != null)
				FormLayout.GetLayoutItemRequiredMarkStyle().AssignToControl(this.requiredMark);
			if(this.optionalMark != null)
				FormLayout.GetLayoutItemOptionalMarkStyle().AssignToControl(this.optionalMark);
		}
		private bool NeedToAssignDefaultWidthToNestedControl(WebControl nestedControl) {
			return nestedControl != null && 
				nestedControl.Width.IsEmpty && 
				TypeDescriptor.GetProperties(nestedControl)["Width"].IsBrowsable;
		}
		protected void PrepareNestedControl() {
			WebControl nestedControl = LayoutItem.GetNestedControl() as WebControl;
			if(NeedToAssignDefaultWidthToNestedControl(nestedControl)) {				
				Unit defaultNestedControlWidth = LayoutItem.FormLayout.GetNestedControlDefaultWidth();
				RenderUtils.SetStyleAttribute(nestedControl, "width", defaultNestedControlWidth, Unit.Empty);
			}
		}
		protected void EnsureNestedControlIdAssigned() {
			if (LayoutItem.Owner.DesignTimeEditingMode)
				return;
			Control nestedControl = LayoutItem.GetNestedControl(NestedControlSearchMode.ReturnFirstAllowedControl);
			if (nestedControl != null)
				LayoutItem.EnsureNestedControlIdAssigned(nestedControl);
		}
	}
	public class TableLayoutItemControl : LayoutItemControl {
		public TableLayoutItemControl(LayoutItem layoutItem)
			: base(layoutItem) {
		}
		protected override void CreateLayoutItemTable() {
			NestedControlCell = RenderUtils.CreateTableCell();
			CaptionCell = RenderUtils.CreateTableCell();
			InternalTable itemTable = RenderUtils.CreateTable(true);
			TableRow row = RenderUtils.CreateTableRow();
			itemTable.Rows.Add(row);
			row.Cells.Add((TableCell)NestedControlCell);
			LayoutItemCaptionLocation captionLocation = LayoutItem.CaptionSettings.GetLocation();
#pragma warning disable 618
			if(captionLocation == LayoutItemCaptionLocation.NoSet || captionLocation == LayoutItemCaptionLocation.NotSet)
				captionLocation = LayoutItemCaptionLocation.Left;
#pragma warning restore 618
			if(captionLocation == LayoutItemCaptionLocation.Left || captionLocation == LayoutItemCaptionLocation.Right) {
				int captionIndex = captionLocation == LayoutItemCaptionLocation.Left ? 0 : 1;
				row.Cells.AddAt(captionIndex, (TableCell)CaptionCell);
			}
			else {
				TableRow secondRow = RenderUtils.CreateTableRow();
				secondRow.Cells.Add((TableCell)CaptionCell);
				int captionRowIndex = captionLocation == LayoutItemCaptionLocation.Top ? 0 : 1;
				itemTable.Rows.AddAt(captionRowIndex, secondRow);
			}
			MainElement = itemTable;
		}
		protected override void CreateInternalNestedControlTable() {
			InternalNestedControlCell = RenderUtils.CreateTableCell();
			HelpTextCell = RenderUtils.CreateTableCell();
			InternalNestedControlTable = RenderUtils.CreateTable(true);
			TableRow row = RenderUtils.CreateTableRow();
			((Table)InternalNestedControlTable).Rows.Add(row);
			row.Cells.Add((TableCell)InternalNestedControlCell);
			HelpTextPosition helpTextPosition = LayoutItem.HelpTextSettings.GetPosition();
			if(helpTextPosition == HelpTextPosition.Left || helpTextPosition == HelpTextPosition.Right) {
				int helpTextIndex = helpTextPosition == HelpTextPosition.Left ? 0 : 1;
				row.Cells.AddAt(helpTextIndex, (TableCell)HelpTextCell);
			}
			else {
				TableRow secondRow = RenderUtils.CreateTableRow();
				secondRow.Cells.Add((TableCell)HelpTextCell);
				int helpTextRowIndex = helpTextPosition == HelpTextPosition.Top ? 0 : 1;
				((Table)InternalNestedControlTable).Rows.AddAt(helpTextRowIndex, secondRow);
			}
		}
		protected override void PrepareInternalNestedControlTable() {
			base.PrepareInternalNestedControlTable();
			if(NestedControlHasFullHeight())
				InternalNestedControlCell.Height = Unit.Percentage(100);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(LayoutItem.AllowEllipsisInText)
				GetMainElement().Style["table-layout"] = "fixed";
		}
		protected override void PrepareCaptionCell() {
			base.PrepareCaptionCell();
			if(NeedSetCaptionCellPixelWidth()) {
				CaptionCell.Width = Unit.Pixel(1);
			}
		}
		private bool IsCaptionLocationTopOrBottom() {
			var captionLocation = LayoutItem.CaptionSettings.GetLocation();
			return captionLocation == LayoutItemCaptionLocation.Top ||  captionLocation == LayoutItemCaptionLocation.Bottom;
		}
		private bool NeedSetCaptionCellPixelWidth() {
			if(LayoutItem.HorizontalAlign == FormLayoutHorizontalAlign.NotSet)
				return false;
			LayoutGroup parentLayoutGroup = LayoutItem.ParentGroupInternal as LayoutGroup;
			if(parentLayoutGroup == null)
				return false;
			return !parentLayoutGroup.AlignItemCaptions || IsCaptionLocationTopOrBottom();
		}
	}
	public class FlowLayoutItemControl : LayoutItemControl {
		public FlowLayoutItemControl(LayoutItem layoutItem)
			: base(layoutItem) {
		}
		protected override void CreateLayoutItemTable() {
			MainElement = RenderUtils.CreateDiv();
			CaptionCell = RenderUtils.CreateDiv();
			NestedControlCell = RenderUtils.CreateDiv();
			LayoutItemCaptionLocation captionLocation = LayoutItem.CaptionSettings.GetLocation();
			if(captionLocation == LayoutItemCaptionLocation.Left || captionLocation == LayoutItemCaptionLocation.Top) {
				MainElement.Controls.Add(CaptionCell);
				MainElement.Controls.Add(NestedControlCell);
			}
			else {
				MainElement.Controls.Add(NestedControlCell);
				MainElement.Controls.Add(CaptionCell);
			}
		}
		protected override void CreateInternalNestedControlTable() {
			InternalNestedControlTable = RenderUtils.CreateDiv();
			InternalNestedControlCell = RenderUtils.CreateDiv();
			HelpTextCell = RenderUtils.CreateDiv();
			HelpTextPosition helpTextPosition = LayoutItem.HelpTextSettings.GetPosition();
			if(helpTextPosition == HelpTextPosition.Left || helpTextPosition == HelpTextPosition.Top) {
				InternalNestedControlTable.Controls.Add(HelpTextCell);
				InternalNestedControlTable.Controls.Add(InternalNestedControlCell);
			}
			else {
				InternalNestedControlTable.Controls.Add(InternalNestedControlCell);
				InternalNestedControlTable.Controls.Add(HelpTextCell);
			}
		}
		protected override void PrepareLayoutItemTable() {
			base.PrepareLayoutItemTable();
			RenderUtils.AppendDefaultDXClassName(MainElement, FormLayoutStyles.ElementsContainerSystemClassName);
			LayoutItemCaptionLocation captionLocation = LayoutItem.CaptionSettings.GetLocation();
			if(captionLocation == LayoutItemCaptionLocation.Bottom) {
				RenderUtils.AppendDefaultDXClassName(MainElement, FormLayoutStyles.FloatedElementsContainerSystemClassName);
				RenderUtils.AppendDefaultDXClassName(CaptionCell, FormLayoutStyles.NotFloatedElementSystemClassName);
			}
			if(captionLocation == LayoutItemCaptionLocation.Top) {
				RenderUtils.AppendDefaultDXClassName(MainElement, FormLayoutStyles.FloatedElementsContainerSystemClassName);
				RenderUtils.AppendDefaultDXClassName(NestedControlCell, FormLayoutStyles.NotFloatedElementSystemClassName);
			}
		}
		protected override void PrepareInternalNestedControlTable() {
			base.PrepareInternalNestedControlTable();
			HelpTextPosition helpTextPosition = LayoutItem.HelpTextSettings.GetPosition();
			RenderUtils.AppendDefaultDXClassName(InternalNestedControlTable, FormLayoutStyles.ElementsContainerSystemClassName);
			if(helpTextPosition == HelpTextPosition.Bottom) {
				RenderUtils.AppendDefaultDXClassName(InternalNestedControlTable, FormLayoutStyles.FloatedElementsContainerSystemClassName);
				RenderUtils.AppendDefaultDXClassName(HelpTextCell, FormLayoutStyles.NotFloatedElementSystemClassName);
			}
			if(helpTextPosition == HelpTextPosition.Top) {
				RenderUtils.AppendDefaultDXClassName(InternalNestedControlTable, FormLayoutStyles.FloatedElementsContainerSystemClassName);
				RenderUtils.AppendDefaultDXClassName(InternalNestedControlCell, FormLayoutStyles.NotFloatedElementSystemClassName);
			}
		}
	}
	public abstract class LayoutGroupControl : LayoutItemControlBase {
		InternalGroupBox groupBox = null;
		WebControl contentElement = null;
		WebControl groupTable = null;
		Dictionary<int, LayoutItemControlBase> groupCells = new Dictionary<int, LayoutItemControlBase>();
		public LayoutGroupControl(LayoutGroup layoutGroup)
			: base(layoutGroup) {
		}
		protected Dictionary<int, LayoutItemControlBase> GroupCells {
			get { return this.groupCells; }
		}
		protected override void ClearControlFields() {
			this.groupBox = null;
			this.contentElement = null;
			this.groupTable = null;
			this.groupCells = new Dictionary<int, LayoutItemControlBase>();
		}
		protected LayoutGroup LayoutGroup {
			get { return base.LayoutItem as LayoutGroup; }
		}
		protected InternalGroupBox GroupBox {
			get { return groupBox; }
			set { groupBox = value; }
		}
		protected WebControl ContentElement {
			get { return contentElement; }
			set { contentElement = value; }
		}
		protected override WebControl GetMainElement() {
			GroupBoxDecoration decoration = LayoutGroup.GetGroupBoxDecoration();
			return decoration != GroupBoxDecoration.None ? GroupBox : ContentElement;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			WebControl contentElement = CreateContentElement();
			ContentElement = contentElement;
			GroupBoxDecoration decoration = LayoutGroup.GetGroupBoxDecoration();
			if(decoration != GroupBoxDecoration.None) {
				GroupBox = new InternalGroupBox(LayoutGroup);
				GroupBox.Content = ContentElement;
				Controls.Add(GroupBox);
			}
			else
				Controls.Add(ContentElement);
		}
		protected abstract WebControl CreateContentElement();
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareContentElement();
			if(this.groupTable != null)
				LayoutGroup.GetGroupTableStyle().AssignToControl(this.groupTable);
			foreach(int i in GroupCells.Keys)
				PrepareGroupCell(i);
		}
		protected virtual void PrepareContentElement() {
			LayoutGroupStyle layoutGroupStyle = LayoutGroup.GetGroupStyle();
			layoutGroupStyle.AssignToControl(ContentElement);
			layoutGroupStyle.Paddings.AssignToControl(ContentElement);
			if(LayoutGroup.AlignItemCaptions)
				ContentElement.CssClass = RenderUtils.CombineCssClasses(ContentElement.CssClass, FormLayoutStyles.AlignedLayoutGroupSystemClassName);
			if(!LayoutGroup.UseDefaultPaddings) {
				ContentElement.CssClass = RenderUtils.CombineCssClasses(ContentElement.CssClass, FormLayoutStyles.NoDefaultPaddingsSystemClassName);
			}
		}
		protected virtual void PrepareGroupCell(int groupCellKey) {
			WebControl groupCell = this.GroupCells[groupCellKey].Parent as WebControl;
			AppearanceStyle groupCellStyle = LayoutGroup.GetCellStyle(GroupCells[groupCellKey].LayoutItem);
			groupCellStyle.AssignToControl(groupCell);
			groupCellStyle.Paddings.AssignToControl(groupCell);
			if(!GroupCells[groupCellKey].LayoutItem.ClientVisible && !DesignMode)
				RenderUtils.SetVisibility(GetGroupCellElementToHide(groupCell), false, true);
			if(GroupCells[groupCellKey].LayoutItem.GetHeight() == Unit.Percentage(100))
				RenderUtils.AppendDefaultDXClassName(groupCell, "dxflFullHeightItemCellSys");
		}
		protected virtual WebControl GetGroupCellElementToHide(WebControl groupCell) {
			return groupCell;
		}
	}
	public class TableLayoutGroupControl : LayoutGroupControl {
		private class RowSpans {
			Hashtable rowSpans = new Hashtable();
			public RowSpans() { }
			public int this[int rowIndex] {
				get {
					this.rowSpans[rowIndex] = this.rowSpans[rowIndex] ?? 0;
					return (int)this.rowSpans[rowIndex];
				}
				set {
					this.rowSpans[rowIndex] = value;
				}
			}
		}
		private static class TableRenderHelper {
			private static void AddItemToRenderMatrix(LayoutItemBase item, List<LayoutItemBase[]> matrix, ref int currentRowIndex, ref int currentColIndex) {
				LayoutItemBase[] currentRow = matrix[currentRowIndex];
				int rowIndex, colIndex;
				FindFreeCellCoordinatesForItem(item, matrix, ref currentRowIndex, ref currentColIndex, out rowIndex, out colIndex);
				for (int i = rowIndex; i < rowIndex + item.GetRowSpan(); i++) {
					for (int j = colIndex; j < colIndex + item.GetColSpan(); j++) {
						if (matrix.Count <= i)
							matrix.Add(new LayoutItemBase[matrix[0].Length]);
						matrix[i][j] = item;
					}
				}
			}
			private static List<LayoutItemBase[]> CreateRenderMatrixInternal(LayoutItemCollection items, int colCount) {
				List<LayoutItemBase[]> renderMatrix = new List<LayoutItemBase[]>();
				renderMatrix.Add(new LayoutItemBase[colCount]);
				int currentRowIndex = 0, currentColIndex = 0;
				for (int i = 0; i < items.GetVisibleItemCount(); i++)
					AddItemToRenderMatrix(items.GetVisibleItemOrGroup(i), renderMatrix, ref currentRowIndex, ref currentColIndex);
				return renderMatrix;
			}
			private static void FindFreeCellCoordinatesForItem(LayoutItemBase item, List<LayoutItemBase[]> matrix,
				ref int startRowIndex, ref int startColIndex, out int rowIndex, out int colIndex) {
				bool cellFound = false;
				do {
					startColIndex = GetFreeRowPositionForItem(item, matrix[startRowIndex], startColIndex);
					cellFound = startColIndex != -1;
					if (startColIndex == -1) {
						startRowIndex++;
						startColIndex = 0;
						if (startRowIndex > matrix.Count - 1)
							break;
					}
				} while (!cellFound);
				rowIndex = startRowIndex;
				colIndex = startColIndex;
				startColIndex += item.GetColSpan();
			}
			private static int GetFreeRowPositionForItem(LayoutItemBase item, LayoutItemBase[] currentRow, int startColIndex) {
				for (int i = startColIndex; i < currentRow.Length; i++) {
					if(HasRowFreeSpace(currentRow, i, item.GetColSpan()))
						return i;
				}
				return -1;
			}
			private static bool HasRowFreeSpace(LayoutItemBase[] currentRow, int colIndex, int spaceWidth) {
				for (int i = colIndex; i < colIndex + spaceWidth; i++) {
					if (i >= currentRow.Length || currentRow[i] != null)
						return false;
				}
				return true;
			}
			private static List<List<LayoutItemBase>> PrepareRenderMatrix(List<LayoutItemBase[]> renderMatrix) {
				List<List<LayoutItemBase>> result = new List<List<LayoutItemBase>>();
				List<LayoutItemBase> alreadyAddedItemsList = new List<LayoutItemBase>();
				for (int i = 0; i < renderMatrix.Count; i++) {
					for (int j = 0; j < renderMatrix[i].Length; j++) {
						LayoutItemBase currentItem = renderMatrix[i][j];
						if (result.Count - 1 < i)
							result.Add(new List<LayoutItemBase>());
						if (!alreadyAddedItemsList.Contains(currentItem) || currentItem == null) {
							if(currentItem != null)
								alreadyAddedItemsList.Add(currentItem);
							result[i].Add(currentItem);
						}
					}
				}
				return result;
			}
			public static List<List<LayoutItemBase>> CreateRenderMatrix(LayoutItemCollection items, int colCount) {
				List<LayoutItemBase[]> renderMatrix = CreateRenderMatrixInternal(items, colCount);
				return PrepareRenderMatrix(renderMatrix);
			}
		}
		public TableLayoutGroupControl(LayoutGroup layoutGroup)
			: base(layoutGroup) {
		}
		protected override WebControl CreateContentElement() {
			return CreateTable(LayoutGroup.Items, LayoutGroup.ColCount, GroupCells);
		}
		private static InternalTable CreateTable(LayoutItemCollection items, int colCount, Dictionary<int, LayoutItemControlBase> groupCells) {
			InternalTable result = RenderUtils.CreateTable(true);
			List<List<LayoutItemBase>> renderMatrix = TableRenderHelper.CreateRenderMatrix(items, colCount);
			for (int i = 0; i < renderMatrix.Count; i++) {
				result.Rows.Add(RenderUtils.CreateTableRow());
				int currentRowIndex = result.Rows.Count - 1;
				for (int j = 0; j < renderMatrix[i].Count; j++) {
					LayoutItemBase currentItem = renderMatrix[i][j];
					TableCell cell = RenderUtils.CreateTableCell();
					if (currentItem != null) {
						if(currentItem.GetColSpan() > 1)
							cell.ColumnSpan = currentItem.GetColSpan();
						if(currentItem.GetRowSpan() > 1)
							cell.RowSpan = currentItem.GetRowSpan();
						int itemIndex = items.IndexOf(currentItem);
						groupCells[itemIndex] = FormLayoutRenderHelper.CreateLayoutElement(currentItem);
						cell.Controls.Add(groupCells[itemIndex]);
						cell.ID = currentItem.Path;
					}
					result.Rows[currentRowIndex].Controls.Add(cell);
				}
			}
			return result;
		}
		protected override void PrepareGroupCell(int groupCellKey) {
			base.PrepareGroupCell(groupCellKey);
			LayoutItemBase groupCellChildItem = GroupCells[groupCellKey].LayoutItem;
			Unit itemWidth = groupCellChildItem.GetWidth();
			Unit itemHeight = groupCellChildItem.GetHeight();
			WebControl groupCell = this.GroupCells[groupCellKey].Parent as WebControl;
			SetGroupCellWidth(groupCellChildItem, groupCell, itemWidth);
			if(itemHeight.Type == UnitType.Percentage)
				groupCell.Height = itemHeight;
			if(groupCellChildItem is LayoutItem && groupCellChildItem.GetRowSpan() > 1)
				PrepareItemWithRowSpan(groupCellChildItem as LayoutItem, GroupCells[groupCellKey].Parent as WebControl);
		}
		protected void SetGroupCellWidth(LayoutItemBase item, WebControl groupCell, Unit itemWidth) {
			if(itemWidth.Type == UnitType.Percentage)
				groupCell.Width = itemWidth;
			else if(RenderUtils.Browser.IsChrome && GroupCellHasParentWithFixedWidth(groupCell))
				groupCell.Width = Unit.Percentage(100 * item.GetColSpan() / (float)LayoutGroup.ColCount);
		}
		protected bool GroupCellHasParentWithFixedWidth(WebControl groupCell) {
			Control currentParent = groupCell.Parent;
			while(currentParent != null) {
				if(currentParent is WebControl) {
					Unit currentParentWidth = ((WebControl)currentParent).Width;
					if(!currentParentWidth.IsEmpty && currentParentWidth.Type != UnitType.Percentage)
						return true;
				}
				currentParent = currentParent.Parent;
			}
			return false;
		}
		public void PrepareItemWithRowSpan(LayoutItem item, WebControl groupCell) {
			if(!Browser.IsIE || RenderUtils.Browser.Version > 8) {
				Table groupTable = ContentElement as Table;
				if(groupTable != null) {
					int rowCount = groupTable.Rows.Count;
					int itemParentRowIndex = groupTable.Rows.GetRowIndex(groupCell.Parent as TableRow);
					if(rowCount - itemParentRowIndex == item.GetRowSpan())
						item.CssClass = RenderUtils.CombineCssClasses(item.CssClass, "dxflLastRowItemSys");
				}
			}
		}
	}
	public class FlowLayoutGroupControl : LayoutGroupControl {
		const int cellsInRowTotalPercentageWidth = 100;
		public FlowLayoutGroupControl(LayoutGroup layoutGroup)
			: base(layoutGroup) {
		}
		protected override WebControl CreateContentElement() {
			WebControl result = RenderUtils.CreateDiv();
			for(int i = 0; i < LayoutGroup.Items.GetVisibleItemCount(); i++) {
				LayoutItemBase currentItem = LayoutGroup.Items.GetVisibleItemOrGroup(i);
				WebControl groupCellContainer = RenderUtils.CreateDiv();
				WebControl groupCell = RenderUtils.CreateDiv();
				GroupCells[i] = FormLayoutRenderHelper.CreateLayoutElement(currentItem);
				groupCell.Controls.Add(GroupCells[i]);
				groupCellContainer.Controls.Add(groupCell);
				groupCellContainer.ID = currentItem.Path;
				result.Controls.Add(groupCellContainer);
			}
			return result;
		}
		protected override void PrepareContentElement() {
			base.PrepareContentElement();
			RenderUtils.AppendDefaultDXClassName(ContentElement, FormLayoutStyles.ElementsContainerSystemClassName);
			RenderUtils.AppendDefaultDXClassName(ContentElement, FormLayoutStyles.FloatedElementsContainerSystemClassName);
			int widthCounter = 0;
			List<WebControl> groupCellsInFirstRow = new List<WebControl>();
			List<WebControl> groupCellsInLastRow = new List<WebControl>();
			bool isFirstRowPassed = false;
			foreach(int i in GroupCells.Keys) {
				WebControl groupCellContainer = GroupCells[i].Parent.Parent as WebControl;
				groupCellContainer.Width = GetGroupCellContainerWidth(GroupCells[i].LayoutItem);
				widthCounter += (int)groupCellContainer.Width.Value;
				if(widthCounter > cellsInRowTotalPercentageWidth) {
					RenderUtils.AppendDefaultDXClassName(groupCellContainer, FormLayoutStyles.NotFloatedElementSystemClassName);
					widthCounter -= cellsInRowTotalPercentageWidth;
					if(!isFirstRowPassed)
						isFirstRowPassed = true;
					EnsureLastChildInRowClassName(groupCellsInLastRow);
					groupCellsInLastRow.Clear();
				}
				if(!isFirstRowPassed)
					groupCellsInFirstRow.Add(groupCellContainer);
				groupCellsInLastRow.Add(groupCellContainer);
			}
			EnsureLastChildInRowClassName(groupCellsInLastRow);
			AssignFirstLastRowElementClassNames(groupCellsInFirstRow, groupCellsInLastRow);
		}
		protected void AssignFirstLastRowElementClassNames(List<WebControl> groupCellsInFirstRow, List<WebControl> groupCellsInLastRow) {
			for(int i = 0; i < groupCellsInFirstRow.Count; i++) {
				if(i == 0)
					RenderUtils.AppendDefaultDXClassName(groupCellsInFirstRow[i], FormLayoutStyles.GroupFirstChildElementSystemClassName);
				RenderUtils.AppendDefaultDXClassName(groupCellsInFirstRow[i], FormLayoutStyles.GroupChildElementInFirstRowSystemClassName);
			}
			for(int i = 0; i < groupCellsInLastRow.Count; i++) {
				if(i == groupCellsInLastRow.Count - 1)
					RenderUtils.AppendDefaultDXClassName(groupCellsInLastRow[i], FormLayoutStyles.GroupLastChildElementSystemClassName);
				RenderUtils.AppendDefaultDXClassName(groupCellsInLastRow[i], FormLayoutStyles.GroupChildElementInLastRowSystemClassName);
			}
		}
		protected void EnsureLastChildInRowClassName(List<WebControl> groupCellsInLastRow) {
			if(!LayoutGroup.UseDefaultPaddings)
				RenderUtils.AppendDefaultDXClassName(groupCellsInLastRow[groupCellsInLastRow.Count - 1],
					FormLayoutStyles.GroupLastChildElementInRowSystemClassName);
		}
		protected Unit GetGroupCellContainerWidth(LayoutItemBase childItem) {
			Unit itemWidth = childItem.GetWidth();
			if(itemWidth.IsEmpty || itemWidth.Type != UnitType.Percentage)
				return Unit.Percentage(100 / (float)LayoutGroup.ColCount);
			return itemWidth;
		}
		protected override WebControl GetGroupCellElementToHide(WebControl groupCell) {
			return groupCell.Parent as WebControl;
		}
	}
	public class TabbedGroupControl : LayoutItemControlBase {
		WebControl mainElement = null;
		ASPxPageControl pageControl = null;
		public TabbedGroupControl(TabbedLayoutGroup tabbedGroup)
			: base(tabbedGroup) {
		}
		protected TabbedLayoutGroup TabbedLayoutGroup {
			get { return base.LayoutItem as TabbedLayoutGroup; }
		}
		protected override void ClearControlFields() {
			if(this.pageControl != null)
				this.pageControl.ViewStateLoaded -= pageControl_ViewStateLoaded;
			this.pageControl = null;
			this.mainElement = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.pageControl = TabbedLayoutGroup.PageControl;
			this.pageControl.ShowTabs = TabbedLayoutGroup.ShowGroupDecoration;
			this.pageControl.ParentSkinOwner = TabbedLayoutGroup.Owner;
			this.pageControl.ViewStateLoaded += pageControl_ViewStateLoaded;
			PopulatePageControlTabPages();
			this.pageControl.ID = TabbedLayoutGroup.PageControlId;
			if(!DesignMode)
				this.pageControl.EnableClientSideAPI = TabbedLayoutGroup.FormLayout.IsClientSideAPIEnabled();
			this.mainElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			this.mainElement.Controls.Add(this.pageControl);
			Controls.Add(this.mainElement);
			if (DesignMode && this.pageControl.TabPages.Count == 0)
				Controls.Add(CreateEmptyTabbedGroupMessageElement());
		}
		void pageControl_ViewStateLoaded(object sender, EventArgs e) {
			ResetControlHierarchy();
			FormLayoutRenderHelper.EnsureChildControlsRecursive(this);
		}
		void PopulatePageControlTabPages() {
			this.pageControl.TabPages.Clear();
			this.pageControl.TabPages.BeginUpdate();
			for(int i = 0; i < TabbedLayoutGroup.Items.GetVisibleItemCount(); i++) {
				LayoutItemBase childItem = TabbedLayoutGroup.Items.GetVisibleItemOrGroup(i);
				TabPage currentPage = this.pageControl.TabPages.Add(childItem.Caption, childItem.Path);
				currentPage.TabImage.Assign(childItem.TabImage);
				currentPage.Controls.Add(FormLayoutRenderHelper.CreateLayoutElement(childItem));
			}
			this.pageControl.TabPages.EndUpdate();
		}
		protected WebControl CreateEmptyTabbedGroupMessageElement() {
			WebControl messageControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			messageControl.Controls.Add(new LiteralControl(StringResources.FormLayout_EmptyTabbedGroupMessage));
			messageControl.ForeColor = Color.Gray;
			messageControl.Font.Italic = true;
			return messageControl;
		}
		protected override WebControl GetMainElement() {
			return this.mainElement;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			SetClientVisibleForTabPages();
			RenderUtils.AppendDefaultDXClassName(this.mainElement, "dxflPCContainerSys");
			PreparePageControl();
		}
		protected void PreparePageControl() {
			this.pageControl.Width = Unit.Percentage(100);
			this.pageControl.Height = Unit.Percentage(100);
			this.pageControl.EnableViewState = TabbedLayoutGroup.FormLayout.EnableViewState;
			this.pageControl.RightToLeft = TabbedLayoutGroup.FormLayout.RightToLeft;
			this.pageControl.EncodeHtml = TabbedLayoutGroup.FormLayout.EncodeHtml;
			if (this.pageControl.ContentStyle.Paddings.IsEmpty)
				this.pageControl.ContentStyle.Paddings.Padding = 0;
			if (!TabbedLayoutGroup.ShowGroupDecoration) {
				this.pageControl.ContentStyle.Border.BorderColor = Color.Transparent;
				this.pageControl.ContentStyle.BackColor = Color.Transparent;
			}
			this.pageControl.CssClass = RenderUtils.CombineCssClasses(this.pageControl.CssClass, FormLayoutStyles.TabbedGroupPageControlSystemClassName);
		}
		protected void SetClientVisibleForTabPages() {
			for (int i = 0; i < TabbedLayoutGroup.Items.GetVisibleItemCount(); i++) {
				LayoutItemBase childItem = TabbedLayoutGroup.Items.GetVisibleItemOrGroup(i);
				TabPage tabPage = this.pageControl.TabPages.FindByName(childItem.Path);
				tabPage.ClientVisible = childItem.ClientVisible;
			}
		}
	}
	public class EmptyLayoutItemControl : LayoutItemControlBase {
		WebControl contentContainer = null;
		public EmptyLayoutItemControl(EmptyLayoutItem emptyLayoutItem)
			: base(emptyLayoutItem) {
		}
		protected EmptyLayoutItem EmptyLayoutItem {
			get { return base.LayoutItem as EmptyLayoutItem; }
		}
		protected override void ClearControlFields() {
			this.contentContainer = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.contentContainer = RenderUtils.CreateDiv();
			Controls.Add(this.contentContainer);
		}
		protected override WebControl GetMainElement() {
			return this.contentContainer;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			EmptyLayoutItem.GetEmptyLayoutItemStyle().AssignToControl(this.contentContainer);
		}
	}
	public static class FormLayoutRenderHelper {
		private delegate PropertyType GetPropertyValueDelegate<ItemType, PropertyType>(ItemType item);
		public static string GetItemTypeClassName(LayoutItem item) {
			Control nestedControl = item.ContainsLiteralControlsOnly() ? new LiteralControl()
				: item.GetNestedControl(NestedControlSearchMode.ReturnFirstAllowedControl);
			return CssClassNameBuilder.GetCssClassNameByControl(nestedControl, FormLayoutStyles.ItemTypeSystemClassNameFormat);
		}
		public static LayoutItemControlBase CreateLayoutElement(LayoutItemBase item) {
			if(item is LayoutGroup)
				if(item.FormLayout.IsFlowRender)
					return new FlowLayoutGroupControl(item as LayoutGroup);
				else
					return new TableLayoutGroupControl(item as LayoutGroup);
			if(item is TabbedLayoutGroup)
				return new TabbedGroupControl(item as TabbedLayoutGroup);
			if(item is EmptyLayoutItem)
				return new EmptyLayoutItemControl(item as EmptyLayoutItem);
			if(item is LayoutItem) {
				if(item.FormLayout.IsFlowRender)
					return new FlowLayoutItemControl(item as LayoutItem);
				else
					return new TableLayoutItemControl(item as LayoutItem);
			}
			return null;
		}
		public static void EnsureChildControlsRecursive(Control mainControl) {
			RenderUtils.EnsureChildControlsRecursive(mainControl, delegate(Control control) {
				return control is LayoutItemNestedControlContainer;
			});
		}
	}
}

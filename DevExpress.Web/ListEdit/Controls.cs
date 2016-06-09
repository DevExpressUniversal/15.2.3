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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
using DevExpress.Utils;
namespace DevExpress.Web.Internal {
	public class ListEditDisplayControl : ASPxInternalWebControl {
		private ImageProperties imageProperties = null;
		private Unit imageSpacing = Unit.Empty;
		private string text = "";
		private Image image = null;
		private LiteralControl textControl = null;
		public ListEditDisplayControl() {
		}
		public ImageProperties ImageProperties {
			get {
				if(imageProperties == null)
					imageProperties = new ImageProperties();
				return imageProperties;
			}
		}
		public Unit ImageSpacing {
			get { return imageSpacing; }
			set { imageSpacing = value; }
		}
		public string Text {
			get { return text; }
			set { text = value; }
		}
		protected Image Image {
			get { return image; }
		}
		protected LiteralControl TextControl {
			get { return textControl; }
		}
		protected override void ClearControlFields() {
			this.image = null;
			this.textControl = null;
		}
		protected override void CreateControlHierarchy() {
			if(!ImageProperties.IsEmpty) {
				this.image = RenderUtils.CreateImage();
				Controls.Add(Image);
			}
			if(Text != "") {
				this.textControl = RenderUtils.CreateLiteralControl();
				Controls.Add(TextControl);
			}
		}
		protected override void PrepareControlHierarchy() {
			if(Image != null) {
				ImageProperties.AssignToControl(Image, DesignMode);
				RenderUtils.SetVerticalAlign(Image, VerticalAlign.Middle);
				if(!ImageSpacing.IsEmpty)
					RenderUtils.SetStyleAttribute(Image, "margin-right", ImageSpacing.ToString(), "");
			}
			if(TextControl != null)
				TextControl.Text = Text;
		}
	}
	public class ListBoxItemContainer {
		public TableRow Row;
		public TableCell ImageCell;
		public Image Image = null;
		public TableCell CheckCell;
		public WebControl CheckBox;
		public readonly List<TableCell> TextCells = new List<TableCell>();
		public readonly List<LiteralControl> LiteralControls = new List<LiteralControl>();
		private ListEditItem item = null;
		private bool isSampleItem = false;
		private int visibleIndex;
		public ListBoxItemContainer(ASPxListBox listBox, int visibleIndex) {
			this.item = listBox.GetVisibleItem(visibleIndex);
			this.visibleIndex = visibleIndex;
			this.isSampleItem = listBox.IsSampleItemIndex(visibleIndex);
		}
		public ListEditItem Item { get { return this.item; } }
		public bool IsSampleItem { get { return this.isSampleItem; } }
		public int VisibleIndex { get { return this.visibleIndex; } }
	}
	class InternalCheckBoxItem : ASPxInternalWebControl, IInternalCheckBoxOwner {
		ASPxListBox listbox;
		ListEditItem item;
		public InternalCheckBoxItem(ASPxListBox listbox, int visibleIndex) {
			this.listbox = listbox;
			this.item = ListBox.GetVisibleItem(visibleIndex);
			Controls.Add(new InternalCheckboxControl(this));
		}
		ASPxListBox ListBox { get { return listbox; } }
		ListEditItem Item { get { return item; } }
		CheckState CheckState { get { return ListBox.GetItemSelected(Item).Value ? CheckState.Checked : CheckState.Unchecked; } }
		Dictionary<string, string> GetAccessibilityCheckBoxAttributes() {
			if(!(ListBox.IsAccessibilityCompliantRender() && RenderUtils.IsHtml5Mode(this)))
				return null;
			Dictionary<string, string> settings = AccessibilityUtils.CreateCheckBoxAttributes("checkbox", CheckState);
			string label = "";
			if(ListBox.IsMultiColumn) {
				List<ListBoxColumn> visibleColumns = ListBox.RenderHelper.VisibleColumns;
				label += AccessibilityUtils.GetListBoxInternalCheckBoxItemLabel(visibleColumns, item);
			} else
				label = Item.Text;
			label += string.Format(AccessibilityUtils.ItemPositionFormatString, Item.Index + 1, ListBox.Items.Count);
			settings.Add("aria-label", label);
			return settings;
		}
		#region IInternalCheckBoxOwner
		CheckState IInternalCheckBoxOwner.CheckState {
			get { return CheckState; }
		}
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() {
			return ListBox.GetCheckableImage(CheckState);
		}
		string IInternalCheckBoxOwner.GetCheckBoxInputID() { return string.Empty; }
		bool IInternalCheckBoxOwner.IsInputElementRequired { get { return true; } }
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle { get { return ListBox.GetInternalCheckBoxStyle(); } }
		string IInternalCheckBoxOwner.AccessKey { get { return string.Empty; } }
		bool IInternalCheckBoxOwner.ClientEnabled { get { return ListBox.ClientEnabled; } }
		bool IInternalCheckBoxOwner.Enabled { get { return ListBox.Enabled; } }
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes { get { return GetAccessibilityCheckBoxAttributes(); } }
		#endregion
	}
	public static class ListEditRenderUtils {
		private const string ValueHiddenFieldID = "VI";
		public const string Nbsp = "&nbsp;";
		public const string SpaceNbsp = " &nbsp;";
		public static WebControl CreateValueCarryingHiddenField(ASPxListEdit listEdit) {
			WebControl hiddenField = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			hiddenField.ID = ValueHiddenFieldID;
			RenderUtils.SetStringAttribute(hiddenField, "type", "hidden");
			RenderUtils.SetStringAttribute(hiddenField, "name", listEdit.UniqueID);
			return hiddenField;
		}
		public static string ProtectItemText(string text) {
			text = ProtectTabs(text);
			return ProtectWhitespaceSeries(text);
		}
		public static string ProtectTabs(string text) { 
			if(RenderUtils.Browser.IsIE || RenderUtils.Browser.Family.IsWebKit)
				text = text.Replace((char)9, (char)32);
			return text;
		}
		public static string ProtectWhitespaceSeries(string text) {
			if(text == "")
				text = Nbsp;
			else {
				if(text[0] == ' ')
					text = Nbsp + text.Substring(1);
				if(text[text.Length - 1] == ' ')
					text = text.Substring(0, text.Length - 1) + Nbsp;
				text = text.Replace("  ", SpaceNbsp);
			}
			return text;
		}
	}
	public class ListBoxControl : TableBasedControlBase {
		private ASPxListBox fListBox = null;
		private List<ListBoxItemContainer> itemContainers = null;
		private ListBoxItemContainer sampleItemContainer = null;
		private KeyboardSupportInputHelper kbInput = null;
		private TableCell kbInputCell = null;
		private WebControl headerDiv;
		private InternalTable headerTable;
		private WebControl scrollDivControl = null;
		private Table sampleTable = null;
		private InternalTable itemTable = null;
		private WebControl topSpacer = null;
		private WebControl bottomSpacer = null;
		WebControl accessibilityHelper = null;
		public ListBoxControl(ASPxListBox listBox) {
			fListBox = listBox;
		}
		public ASPxListBox ListBox {
			get { return fListBox; }
		}
		public InternalTable ItemTable {
			get { return itemTable; }
		}
		protected WebControl VCHiddenField { get; private set; }
		protected KeyboardSupportInputHelper KbInput {
			get { return kbInput; }
		}
		protected TableCell KbInputCell {
			get { return kbInputCell; }
		}
		protected WebControl HeaderDiv {
			get { return headerDiv; }
		}
		protected InternalTable HeaderTable {
			get { return headerTable; }
		}
		protected List<ListBoxItemContainer> ItemContainers {
			get {
				if(itemContainers == null)
					itemContainers = new List<ListBoxItemContainer>();
				return itemContainers;
			}
		}
		protected ListBoxItemContainer SampleItemContainer {
			get {
				return sampleItemContainer;
			}
		}
		protected int SampleItemIndex {
			get { return -1; }
		}
		protected ListBoxRenderHelper RenderHelper {
			get { return ListBox.RenderHelper; }
		}
		protected bool IsNativeCheckBoxes { get { return ListBox.NativeCheckBoxes; } }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.kbInput = null;
			this.kbInputCell = null;
			this.headerDiv = null;
			this.headerTable = null;
			this.topSpacer = null;
			this.bottomSpacer = null;
			this.scrollDivControl = null;
			this.sampleTable = null;
			this.itemTable = null;
			this.itemContainers = null;
			this.sampleItemContainer = null;
		}
		protected override void CreateMainCellContent(TableCell mainCell) {
			if(ListBox.IsHeaderRequired())
				CreateHeader(mainCell);
			if(ListBox.IsAriaSupported() && ListBox.IsAccessibilityCompliantRender())
				CreateAccessibilityHelper(mainCell);
			scrollDivControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			mainCell.Controls.Add(scrollDivControl);
			VCHiddenField = ListEditRenderUtils.CreateValueCarryingHiddenField(ListBox);
			scrollDivControl.Controls.Add(VCHiddenField);
			CreateSampleTable(scrollDivControl);
			CreateTopSpacer(scrollDivControl);
			CreateListTable(scrollDivControl);
			CreateBottomSpacer(scrollDivControl);
			if(!(ListBox.IsAriaSupported() && ListBox.IsAccessibilityCompliantRender()))
				CreateKBSupportInput(mainCell);
		}
		void CreateAccessibilityHelper(TableCell mainCell) {
			accessibilityHelper = new WebControl(HtmlTextWriterTag.Div);
			for(int i = 0; i < 2; i++) {
				WebControl child = new WebControl(HtmlTextWriterTag.Div);
				accessibilityHelper.Controls.Add(child);
			}
			mainCell.Controls.Add(accessibilityHelper);
		}
		protected void CreateHeader(TableCell mainCell) {
			this.headerDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			mainCell.Controls.Add(HeaderDiv);
			HeaderDiv.ID = ASPxListBox.HeaderDivID;
			this.headerTable = RenderUtils.CreateTable(true);
			HeaderDiv.Controls.Add(HeaderTable);
			TableRow tableRow = RenderUtils.CreateTableRow();
			HeaderTable.Rows.Add(tableRow);
			if(ListBox.IsCheckColumnExists)
				CreateHeaderCell(tableRow, ListEditRenderUtils.Nbsp);
			if(RenderHelper.ImageColumnExists)
				CreateHeaderCell(tableRow, ListEditRenderUtils.Nbsp);
			List<ListBoxColumn> visibleColumns = RenderHelper.VisibleColumns;
			for(int i = 0; i < visibleColumns.Count; i++) {
				ListBoxColumn column = visibleColumns[i];
				CreateHeaderCell(tableRow, RenderHelper.GetColumnCaption(column));
			}
		}
		protected void CreateHeaderCell(TableRow tableRow, string columnCaption) {
			TableCell cell = RenderUtils.CreateTableCell();
			tableRow.Cells.Add(cell);
			LiteralControl literalControl = RenderUtils.CreateLiteralControl(columnCaption);
			cell.Controls.Add(literalControl);
		}
		protected override void PrepareMainCell(TableCell mainCell) {
			base.PrepareMainCell(mainCell);
			if(ListBox.GetHeight().IsEmpty)
				mainCell.VerticalAlign = VerticalAlign.Top;
		}
		protected override void PrepareMainCellContent() {
			PrepareHeader();
			PrepareDiv();
			PrepareScrollBar();
			PrepareSampleTable();
			PrepareListTable();
			if(!(ListBox.IsAriaSupported() && ListBox.IsAccessibilityCompliantRender()))
				PrepareKBInputCell();
			else
				PrepareAccessibilityHelper();
		}
		protected void PrepareHeader() {
			if(HeaderDiv != null) {
				ListBox.GetHeaderDivStyle().AssignToControl(HeaderDiv);
				HeaderTable.Width = Unit.Percentage(100);
				RenderUtils.SetStyleStringAttribute(HeaderTable, "table-layout", "fixed", true);
				ListBoxItemStyle headerCellStyle = ListBox.GetHeaderCellStyle();
				if(RenderHelper.CheckColumnExists) {
					PrepareHeaderCell(headerCellStyle, RenderHelper.CheckBoxCellIndex, RenderHelper.CheckBoxWidth, Unit.Empty, "");
				}
				if(RenderHelper.ImageColumnExists) {
					Unit padding = GetImageSpacing(ListBox.GetItemCellStyle(false));
					PrepareHeaderCell(headerCellStyle, RenderHelper.ImageCellIndex, RenderHelper.ItemImageWidth, padding, "");
				}
				List<ListBoxColumn> visibleColumns = RenderHelper.VisibleColumns;
				for(int cellIndex = RenderHelper.FirstTextCellIndex; cellIndex <= RenderHelper.LastTextCellIndex; cellIndex++) {
					int visibleColumnIndex = cellIndex - RenderHelper.FirstTextCellIndex;
					ListBoxColumn column = visibleColumns[visibleColumnIndex];
					PrepareHeaderCell(headerCellStyle, cellIndex, RenderHelper.GetColumnWidth(column), Unit.Empty, column.ToolTip);
				}
				if(RenderUtils.IsHtml5Mode(this) && ListBox.IsAccessibilityCompliantRender())
					RenderUtils.SetStringAttribute(HeaderTable, "role", "presentation");
			}
		}
		protected void PrepareHeaderCell(ListBoxItemStyle headerCellStyle, int cellIndex, Unit width, Unit padding, string tooltip) {
			TableCell headerCell = this.headerTable.Rows[0].Cells[cellIndex];
			headerCellStyle.AssignToControl(headerCell, true);
			RenderUtils.AppendDefaultDXClassName(headerCell, RenderHelper.GetHeaderCellClassName(cellIndex));
			if(!padding.IsEmpty)
				RenderUtils.SetStyleUnitAttribute(headerCell, RenderHelper.IsRightToLeft ? "padding-left" : "padding-right", padding, true);
			headerCell.Width = width;
			headerCell.ToolTip = tooltip;
		}
		protected override void PrepareTable(Table table) {
			RenderUtils.AssignAttributes(ListBox, table);
			table.TabIndex = 0;
			ListBox.RemoveImportedStyleAttrsFromMainElement(table);
			RenderUtils.SetVisibility(table, ListBox.IsClientVisible(), true);
			ListBox.GetControlStyle().AssignToControl(table);
			if(RenderUtils.IsHtml5Mode(this) && ListBox.IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(table, "role", "application");
		}
		protected void AddItemContainer(ListBoxItemContainer container) {
			if(container.IsSampleItem)
				this.sampleItemContainer = container;
			else
				ItemContainers.Add(container);
		}
		protected ListBoxItemContainer GetItemContainer(int index) {
			return ListBox.IsSampleItemIndex(index) ? SampleItemContainer : ItemContainers[index];
		}
		protected bool TopAndBottomSpacerRequired {
			get { return ListBox.EnableCallbackMode && ListBox.Enabled; }
		}
		protected void CreateTopSpacer(WebControl parent) {
			if(TopAndBottomSpacerRequired) {
				this.topSpacer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				parent.Controls.Add(topSpacer);
				topSpacer.ID = ListBox.GetTopSpacerId();
			}
		}
		protected void CreateBottomSpacer(WebControl parent) {
			if(TopAndBottomSpacerRequired) {
				this.bottomSpacer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				parent.Controls.Add(bottomSpacer);
				bottomSpacer.ID = ListBox.GetBottomSpacerId();
			}
		}
		protected void CreateItemsHierarchy() {
			if(ListBox.HasFakeItem)
				CreateFakeItemHierarchy(ItemTable);
			int visibleItemCount = ListBox.Properties.VisibleItems.Count;
			for(int visibleIndex = 0; visibleIndex < visibleItemCount; visibleIndex++)
				CreateItemHierarchy(ItemTable, visibleIndex);
			if(RenderUtils.IsHtml5Mode(this) && ListBox.IsAccessibilityCompliantRender() && ListBox.IsMultiColumn)
				CreateAccessibilityHeaderRow(ItemTable);
		}
		void CreateAccessibilityHeaderRow(Table parent) {
			TableRow row = RenderUtils.CreateTableRow();
			int columnCount = ListBox.Columns.Count;
			for(int i = RenderHelper.IsCheckColumnExists ? -1 : 0; i < columnCount; i++) {
				TableCell cell = RenderUtils.CreateTableHeaderCell("col");
				string caption = i == -1 ? AccessibilityUtils.CheckBoxColumnHeaderText : RenderHelper.GetColumnCaption(ListBox.Columns[i]);
				cell.Controls.Add(new LiteralControl(caption));
				row.Cells.Add(cell);
			} 
			parent.Rows.Add(row);
		}
		protected void CreateItemHierarchy(Table parent, int visibleIndex) {
			ListBoxItemContainer container = new ListBoxItemContainer(ListBox, visibleIndex);
			container.Row = RenderUtils.CreateTableRow();
			AddItemContainer(container);
			if(ListBox.IsCheckColumnExists)
				CreateCheckCell(container, visibleIndex);
			if(RenderHelper.ImageColumnExists)
				CreateImageCell(container);
			CreateTextCells(container);
			parent.Rows.Add(container.Row);
		}
		protected void CreateFakeItemHierarchy(Table parent) {
			TableRow row = RenderUtils.CreateTableRow();
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			parent.Rows.Add(row);
		}
		protected void CreateImageCell(ListBoxItemContainer container) {
			container.ImageCell = RenderUtils.CreateTableCell();
			container.Row.Cells.Add(container.ImageCell);
			if(ListBox.HasImage(container.Item, container.IsSampleItem)) {
				container.Image = RenderUtils.CreateImage();
				container.ImageCell.Controls.Add(container.Image);
			}
		}
		protected void CreateCheckCell(ListBoxItemContainer container, int visibleIndex) {
			if(IsNativeCheckBoxes) {
				container.CheckBox = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
				container.CheckBox.TabIndex = this.ListBox.TabIndex;
			} else
				container.CheckBox = new InternalCheckBoxItem(ListBox, visibleIndex);
			container.CheckCell = RenderUtils.CreateTableCell();
			container.Row.Cells.Add(container.CheckCell);
			container.CheckCell.Controls.Add(container.CheckBox);
		}
		protected void CreateTextCells(ListBoxItemContainer container) {
			if(ListBox.IsMultiColumn) {
				for(int i = RenderHelper.VisibleColumns.Count; i > 0; i--) {
					TableCell textCell = RenderUtils.CreateTableCell();
					container.TextCells.Add(textCell);
					container.Row.Cells.Add(textCell);
					LiteralControl literalControl = RenderUtils.CreateLiteralControl();
					container.LiteralControls.Add(literalControl);
					textCell.Controls.Add(literalControl);
				}
			} else {
				TableCell textCell = RenderUtils.CreateTableCell();
				container.TextCells.Add(textCell);
				container.Row.Cells.Add(textCell);
				if(container.Item.TextTemplate == null) {
					LiteralControl literalControl = RenderUtils.CreateLiteralControl();
					container.LiteralControls.Add(literalControl);
					textCell.Controls.Add(literalControl);
				} else
					CreateItemTextTemplate(textCell, container.Item);
			}
		}
		protected void CreateItemTextTemplate(WebControl parent, ListEditItem item) {
			ListEditItemTemplateContainer container = new ListEditItemTemplateContainer(item);
			container.ID = "ITTC" + item.Index;
			item.TextTemplate.InstantiateIn(container);
			parent.Controls.Add(container);
		}
		protected void CreateKBSupportInput(TableCell mainCell) {
			if(!ListBox.IsComboBoxList && ListBox.IsEnabled() && !ListBox.InternalDisableScrolling) {
				this.kbInput = new KeyboardSupportInputHelper();
				Control parent = null;
				if(Browser.IsOpera || Browser.Family.IsWebKit)
					parent = mainCell;
				else 
					parent = CreateKBInputCell((TableRow)mainCell.Parent);
				parent.Controls.Add(KbInput);
			}
		}
		protected TableCell CreateKBInputCell(TableRow MainRow) {
			this.kbInputCell = RenderUtils.CreateTableCell();
			MainRow.Cells.Add(this.kbInputCell);
			return this.kbInputCell;
		}
		protected void CreateListTable(WebControl parent) {
			this.itemTable = RenderUtils.CreateTable(true);
			this.scrollDivControl.Controls.Add(ItemTable);
			CreateItemsHierarchy();
		}
		protected void CreateSampleTable(WebControl parent) {
			if(ListBox.HasSampleItem) {
				this.sampleTable = RenderUtils.CreateTable(true);
				parent.Controls.Add(this.sampleTable);
				CreateItemHierarchy(this.sampleTable, SampleItemIndex);
			}
			ListBox.RenderHelper.SampleItemCreated = ListBox.HasSampleItem;
		}
		protected void PrepareItemsHierarchy() {
			AppearanceStyleBase itemRowStyle = ListBox.GetItemRowStyle(false);
			AppearanceStyleBase selectedItemRowStyle = ListBox.GetItemRowStyle(true);
			ListBoxItemStyle itemCellStyle = ListBox.GetItemCellStyle(false);
			ListBoxItemStyle selectedItemCellStyle = ListBox.GetItemCellStyle(true);
			int selectedIndex = ListBox.SelectedIndex;
			for(int i = 0; i < ItemContainers.Count; i++) {
				if(ListBox.GetIsItemAllowsSelectedStyle(i, selectedIndex))
					PrepareItemHierarchy(i, selectedItemRowStyle, selectedItemCellStyle);
				else
					PrepareItemHierarchy(i, itemRowStyle, itemCellStyle);
			}
			if(RenderUtils.IsHtml5Mode(this) && ListBox.IsAccessibilityCompliantRender() && ListBox.IsMultiColumn)
				GetAccessibilityHeaderRow().CssClass = AccessibilityUtils.InvisibleRowCssClassName;
		}
		TableRow GetAccessibilityHeaderRow() {
			return ItemTable.Rows[ItemTable.Rows.Count - 1];
		}
		protected void PrepareItemHierarchy(int containerIndex, AppearanceStyleBase itemRowStyle, ListBoxItemStyle itemCellStyle) {
			ListBoxItemContainer container = GetItemContainer(containerIndex);
			itemRowStyle.AssignToControl(container.Row);
			container.Row.Height = itemCellStyle.Height;
			if(ListBox.IsCheckColumnExists)
				PrepareCheckCell(container, itemCellStyle);
			if(RenderHelper.ImageColumnExists)
				PrepareImageCell(container, itemCellStyle);
			PrepareTextCells(container, itemCellStyle);
		}
		protected void PrepareCheckCell(ListBoxItemContainer container, ListBoxItemStyle cellStyle) {
			cellStyle.AssignToControl(container.CheckCell, true);
			RenderUtils.AppendDefaultDXClassName(container.CheckCell, RenderHelper.GetCheckCellClassName());
			if(container.VisibleIndex <= 0)
				container.CheckCell.Width = RenderHelper.CheckBoxWidth;
			if(IsNativeCheckBoxes) {
				RenderUtils.SetStringAttribute(container.CheckBox, "type", "checkbox");
				if(container.VisibleIndex >= 0) {
					bool isChecked = ListBox.GetItemSelected(container.Item).Value;
					RenderUtils.SetStringAttribute(container.CheckBox, "checked", isChecked ? "checked" : "");
				}
				if(!IsEnabled() || ListBox.ReadOnly)
					RenderUtils.SetStringAttribute(container.CheckBox, "disabled", "disabled");
			}
		}
		protected void PrepareImageCell(ListBoxItemContainer container, ListBoxItemStyle cellStyle) {
			cellStyle.AssignToControl(container.ImageCell, true);
			RenderUtils.AppendDefaultDXClassName(container.ImageCell, RenderHelper.GetImageCellClassName());
			RenderUtils.SetStyleUnitAttribute(container.ImageCell, RenderHelper.IsRightToLeft ? "padding-left" : "padding-right", GetImageSpacing(cellStyle), true);
			ImageProperties imageProp = ListBox.GetItemImage(container.Item, container.IsSampleItem);
			if(container.VisibleIndex <= 0)
				container.ImageCell.Width = RenderHelper.ItemImageWidth;
			if(imageProp != null)
				imageProp.AssignToControl(container.Image, DesignMode);
			else
				container.ImageCell.Text = ListEditRenderUtils.Nbsp;
		}
		protected void PrepareTextCells(ListBoxItemContainer container, ListBoxItemStyle cellStyle) {
			bool isLiteralControlsExist = container.LiteralControls.Count > 0;
			bool widthAssignmentRequired = ListBox.IsMultiColumn && container.VisibleIndex <= 0;
			for(int i = 0; i < container.TextCells.Count; i++) {
				TableCell textCell = container.TextCells[i];
				cellStyle.AssignToControl(textCell, true);
				RenderUtils.AppendDefaultDXClassName(textCell, RenderHelper.GetTextCellClassName(i));
				if(widthAssignmentRequired)
					textCell.Width = RenderHelper.GetVisibleColumnWidth(i);
				if(isLiteralControlsExist) {
					LiteralControl literalControl = container.LiteralControls[i];
					if(literalControl != null) {
						string text = ListBox.IsMultiColumn ?
							ListBox.GetVisibleItemText(container.VisibleIndex, i) :
							ListBox.GetVisibleItemText(container.VisibleIndex);
						if(text == null)
							text = "";
						if(ListBox.IsComboBoxList) 
							text = text.Replace("\r\n", " ");
						literalControl.Text = ListEditRenderUtils.ProtectItemText(text);
					}
				}
			}
		}
		protected Unit GetImageSpacing(ListBoxItemStyle style) {
			return style.ImageSpacing.IsEmpty ? Unit.Pixel(0) : style.ImageSpacing;
		}
		protected void PrepareDiv() {
			ListBox.GetScrollingDivStyle().AssignToControl(scrollDivControl);
			scrollDivControl.ID = ListBox.GetScrollDivId();
			scrollDivControl.Height = ListBox.GetHeight();
			if(!ListBox.IsMultiColumn)
				scrollDivControl.Width = Unit.Percentage(100);
			if(RenderHelper.IsRightToLeft)
				scrollDivControl.Attributes["dir"] = "rtl";
		}
		protected void PrepareListTable() {
			ItemTable.ID = ListBox.GetListTableId();
			ItemTable.Width = Unit.Percentage(100);
			if(ListBox.IsMultiColumn)
				RenderUtils.SetStyleStringAttribute(ItemTable, "table-layout", "fixed", true);
			PrepareItemsHierarchy();
			if(RenderHelper.IsRightToLeft)
				ItemTable.Attributes["dir"] = "rtl";
			if(ListBox.IsAriaSupported() && ListBox.IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(ItemTable, "role", "listbox");
		}
		protected void PrepareSampleTable() {
			if(sampleTable != null) {
				PrepareItemHierarchy(SampleItemIndex, ListBox.GetItemRowStyle(false), ListBox.GetItemCellStyle(false));
				sampleTable.Rows[0].ID = ListBox.GetSampleItemId();
				if(RenderHelper.ImageColumnExists) {
					sampleTable.Rows[0].Cells[0].ID = ListBox.GetItemImageCellId();
					sampleTable.Rows[0].Cells[1].ID = ListBox.GetItemTextCellId();
				} else
					sampleTable.Rows[0].Cells[0].ID = ListBox.GetItemImageCellId();
				RenderUtils.SetStyleStringAttribute(sampleTable, "visibility", "hidden", true);
				RenderUtils.SetStyleStringAttribute(sampleTable, "display", "none", true);
			}
		}
		protected void PrepareScrollBar() {
			if(ListBox.InternalDisableScrolling)
				return;
			if(RenderUtils.IsOverflowStyleSeparated) {
				scrollDivControl.Style.Add(HtmlTextWriterStyle.OverflowX, "hidden");
			}
			scrollDivControl.Style.Add(RenderUtils.IsOverflowStyleSeparated ? HtmlTextWriterStyle.OverflowY : HtmlTextWriterStyle.Overflow,
				DesignMode ? "hidden" : "auto");
			if(DesignMode)
				scrollDivControl.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
		}
		protected void PrepareKBInputCell() {
			bool kbInputCellCollapseRequired = Browser.IsIE || (Browser.IsFirefox && Browser.MajorVersion >= 3);
			if(KbInputCell != null && kbInputCellCollapseRequired) 
				KbInputCell.Width = 0;
			if(KbInput != null)
				KbInput.TabIndex = this.ListBox.TabIndex;
		}
		void PrepareAccessibilityHelper() {
			accessibilityHelper.ID = AccessibilityUtils.AssistantID;
			RenderUtils.SetStringAttribute(accessibilityHelper, "role", "listbox");
			RenderUtils.SetStringAttribute(accessibilityHelper, "tabindex", "0");
			bool isMultiselectable = ListBox.IsMultiSelect || ListBox.IsCheckColumnExists;
			RenderUtils.SetStringAttribute(accessibilityHelper, "aria-multiselectable", isMultiselectable.ToString().ToLower());
			accessibilityHelper.CssClass = AccessibilityUtils.InvisibleFocusableElementCssClassName;
			for(int i = 0; i < accessibilityHelper.Controls.Count; i++) {
				WebControl child = accessibilityHelper.Controls[i] as WebControl;
				child.ID = accessibilityHelper.ID + i.ToString();
				RenderUtils.SetStringAttribute(child, "role", "option");
			}
			if(Browser.IsIE || Browser.IsEdge)
				RenderUtils.SetStringAttribute(ListBox, "role", "application");
		}
	}
	public abstract class ListNativeControl : ASPxInternalWebControl {
		private ASPxEdit edit = null;
		private WebControl selectControl = null;
		private List<WebControl> optionControls = null;
		private List<LiteralControl> optionTextControls = null;
		public ListNativeControl(ASPxEdit edit) {
			this.edit = edit;
		}
		public ASPxEdit Edit {
			get { return edit; }
		}
		protected WebControl SelectControl {
			get { return selectControl; }
		}
		protected List<WebControl> OptionControls {
			get { return optionControls; }
		}
		protected List<LiteralControl> OptionTextControls {
			get { return optionTextControls; }
		}
		protected abstract ListEditItemCollection Items { get; }
		protected abstract int RowCount { get; }
		protected abstract int SelectedIndex { get; }
		protected abstract Type ValueType { get; }
		protected override void ClearControlFields() {
			this.selectControl = null;
			this.optionControls = null;
		}
		protected override void CreateControlHierarchy() {
			this.selectControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Select);
			Controls.Add(SelectControl);
			this.optionControls = new List<WebControl>();
			this.optionTextControls = new List<LiteralControl>();
			for(int i = 0; i < Items.Count; i++) {
				WebControl option = RenderUtils.CreateWebControl(HtmlTextWriterTag.Option);
				SelectControl.Controls.Add(option);
				OptionControls.Add(option);
				LiteralControl optionTextControl = RenderUtils.CreateLiteralControl();
				option.Controls.Add(optionTextControl);
				OptionTextControls.Add(optionTextControl);
			}
		}
		protected override void PrepareControlHierarchy() {
			PrepareSelectControl(SelectControl);
			for(int i = 0; i < Items.Count; i++) {
				RenderUtils.SetStringAttribute(OptionControls[i], "selected", (GetItemIndexSelected(i)) ? "selected" : "");
				RenderUtils.SetStringAttribute(OptionControls[i], "value", i.ToString());
				OptionTextControls[i].Text = Edit.HtmlEncode(Items[i].Text);
			}
		}
		protected bool GetItemIndexSelected(int index) {
			return Items[index].Index == SelectedIndex || Items[index].Selected;
		}
		protected virtual void PrepareSelectControl(WebControl selectControl) {
			RenderUtils.AssignAttributes(Edit, selectControl);
			RenderUtils.SetVisibility(selectControl, Edit.IsClientVisible(), true);
			Edit.GetControlStyle().AssignToControl(selectControl);
			RenderUtils.SetStringAttribute(selectControl, "name", Edit.UniqueID);
			RenderUtils.SetStringAttribute(selectControl, "size", RowCount.ToString());
			if(!Edit.IsEnabled())
				RenderUtils.SetStringAttribute(selectControl, "disabled", "disabled");
		}
	}
	public class ListBoxNativeControl : ListNativeControl {
		public ListBoxNativeControl(ASPxListBox listBox)
			: base(listBox) {
		}
		public ASPxListBox ListBox {
			get { return (ASPxListBox)Edit; }
		}
		protected override ListEditItemCollection Items {
			get { return ListBox.Items; }
		}
		protected override int RowCount {
			get { return ListBox.Rows; }
		}
		protected override int SelectedIndex {
			get { return ListBox.SelectedIndex; }
		}
		protected override Type ValueType {
			get { return ListBox.ValueType; }
		}
		protected override void PrepareSelectControl(WebControl selectControl) {
			base.PrepareSelectControl(selectControl);
			if(ListBox.IsEnabled() && !DesignMode) {
				RenderUtils.SetStringAttribute(selectControl, "onfocus", ListBox.GetOnGotFocus());
				RenderUtils.SetStringAttribute(selectControl, "onblur", ListBox.GetOnLostFocus());
				RenderUtils.SetStringAttribute(selectControl, "onchange", ListBox.GetOnNativeSelectedIndexChanged());
				RenderUtils.SetStringAttribute(selectControl, "ondblclick", ListBox.GetOnNativeDblClick());
				if(ListBox.IsMultiSelect)
					selectControl.Attributes.Add("multiple", "multiple");
			}
		}
	}
	public class ButtonListItemsControlBase : ItemsControl<ListEditItem> {
		private ASPxCheckListBase checkListBase = null;
		public ButtonListItemsControlBase(ASPxCheckListBase checkListBase, List<ListEditItem> items)
			: base(items, checkListBase.RepeatColumns, checkListBase.RepeatDirection, checkListBase.RepeatLayout) {
			this.checkListBase = checkListBase;
		}
		public ASPxCheckListBase CheckListBase {
			get { return checkListBase; }
		}
		protected WebControl VCHiddenField { get; private set; }
		protected override void OnCreatingMainCellContent(TableCell mainCell) {
			VCHiddenField = ListEditRenderUtils.CreateValueCarryingHiddenField(CheckListBase);
			mainCell.Controls.Add(VCHiddenField);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(MainTable != null) {
				MainTable.TabIndex = 0;
				RenderUtils.SetVisibility(MainTable, CheckListBase.IsClientVisible(), true);
				CheckListBase.RemoveImportedStyleAttrsFromMainElement(MainTable);
				if(CheckListBase.IsAriaSupported() && CheckListBase.IsAccessibilityCompliantRender() && CheckListBase is ASPxRadioButtonList) {
					RenderUtils.SetStringAttribute(MainTable, "role", "radiogroup");
					string owns = "";
					for(int i = 0; i < CheckListBase.Items.Count; i++)
						owns += string.Format("{0}_{1}_I_D ", CheckListBase.ID, CheckListBase.GetItemID(i));
					RenderUtils.SetStringAttribute(MainTable, "aria-owns", owns);
				}
			}
		}
		protected override Control CreateItemControl(int index, ListEditItem item) {
			return CheckListBase.CreateItemControl(index, item);
		}
		protected override void PrepareItemControl(Control control, int index, ListEditItem item) {
			CheckListBase.PrepareItemControl(control, index, item);
		}
	}
	[ToolboxItem(false)]
	public class CheckBoxListItemControl : ASPxCheckBox, IInternalCheckBoxOwner {
		private const string ReadonlyClickHandlerName = "ASPx.ERBLICancel('{0}')"; 
		private const string ClickHandlerName = "ASPx.ERBLIClick('{0}', {1})";		 
		private ASPxCheckListBase checkListBase = null;
		private ListEditItem item = null;
		private int controlIndex = -1;
		public CheckBoxListItemControl(ASPxCheckListBase checkListBase, ListEditItem item, int controlIndex)
			: base() {
			this.checkListBase = checkListBase;
			this.item = item;
			this.controlIndex = controlIndex;
			UsingInsideList = true;
		}
		Dictionary<string, string> GetAccessibilityCheckBoxAttributes() {
			if(!RenderUtils.IsHtml5Mode(this) || !IsAccessibilityCompliantRender())
				return null;
			Dictionary<string, string> settings = AccessibilityUtils.CreateCheckBoxAttributes("checkbox", CheckState);
			settings.Add("aria-label", string.Format(AccessibilityUtils.CheckBoxListItemFormatString, ControlIndex + 1, CheckListBase.Items.Count, Item.Text));
			return settings;
		}
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() {
			return this.checkListBase.Properties.GetImage(item.Selected ? CheckState.Checked : CheckState.Unchecked, Page);
		}
		string IInternalCheckBoxOwner.GetCheckBoxInputID() {
			return null;
		}
		bool IInternalCheckBoxOwner.IsInputElementRequired {
			get { return true; }
		}
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle {
			get { return checkListBase.GetInternalCheckBoxStyle(); }
		}
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes {
			get { return GetAccessibilityCheckBoxAttributes(); }
		}
		protected internal override bool IsErrorFrameRequired {
			get { return false; }
		}
		protected ListEditItem Item {
			get { return item; }
		}
		protected internal int ControlIndex {
			get { return controlIndex; }
		}
		protected ASPxCheckListBase CheckListBase {
			get { return checkListBase; }
		}
		protected override void PrepareControlHierarchy() {
			EncodeHtml = CheckListBase.EncodeHtml;
			base.PrepareControlHierarchy();
		}
		public override void RegisterEditorIncludeScripts() {
		}
		protected override string GetStartupScript() {
			return string.Empty;
		}
		protected override string GetOnClickNormal() {
			return string.Format(ClickHandlerName, CheckListBase.ClientID, ControlIndex);
		}
		protected override string GetOnClickReadonly() {
			return string.Format(ReadonlyClickHandlerName, CheckListBase.ClientID);
		}
		protected internal override string GetOnGotFocus() {
			return CheckListBase.GetOnGotFocus();
		}
		protected internal override string GetOnLostFocus() {
			return CheckListBase.GetOnLostFocus();
		}
		protected internal override string GetInputName() {
			return CheckListBase.UniqueID + "_RB";
		}
		protected internal override string GetInputValue() {
			return string.Empty;
		}
		public override void RegisterStyleSheets() {
		}
		protected override DisabledStyle GetDisabledStyle() {
			return new DisabledStyle();
		}
		protected override bool IsDefaultAppearanceEnabled() {
			return false;
		}
	}
	[ToolboxItem(false)]
	public class RadioButtonListItemControl : ASPxRadioButton, IInternalCheckBoxOwner {
		private new const string ReadonlyClickHandlerName = "ASPx.ERBLICancel('{0}')";
		private const string ClickHandlerName = "ASPx.ERBLIClick('{0}', {1})";
		private ASPxCheckListBase checkListBase = null;
		private ListEditItem item = null;
		private int controlIndex = -1;
		public RadioButtonListItemControl(ASPxCheckListBase checkListBase, ListEditItem item, int controlIndex)
			: base() {
			this.checkListBase = checkListBase;
			this.item = item;
			this.controlIndex = controlIndex;
			UsingInsideList = true;
		}
		Dictionary<string, string> GetAccessibilityCheckBoxAttributes() {
			if(!IsAriaSupported() || !IsAccessibilityCompliantRender())
				return null;
			Dictionary<string, string> settings = AccessibilityUtils.CreateCheckBoxAttributes("radio", CheckState);
			settings.Add("aria-label", Item.Text);
			if(Browser.IsIE || Browser.IsEdge)
				settings.Add("aria-selected", AccessibilityUtils.GetStringCheckedState(CheckState));
			settings.Add("aria-posinset", (ControlIndex + 1).ToString());
			settings.Add("aria-setsize", CheckListBase.Items.Count.ToString());
			return settings;
		}
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() {
			return this.checkListBase.Properties.GetImage(CheckState, Page);
		}
		string IInternalCheckBoxOwner.GetCheckBoxInputID() {
			return null;
		}
		bool IInternalCheckBoxOwner.IsInputElementRequired {
			get { return true; }
		}
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle {
			get { return checkListBase.GetInternalCheckBoxStyle(); }
		}
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes { get { return GetAccessibilityCheckBoxAttributes(); } }
		protected internal override bool IsErrorFrameRequired {
			get { return false; }
		}
		protected ListEditItem Item {
			get { return item; }
		}
		protected internal int ControlIndex {
			get { return controlIndex; }
		}
		protected ASPxCheckListBase CheckListBase {
			get { return checkListBase; }
		}
		protected override void PrepareControlHierarchy() {
			EncodeHtml = CheckListBase.EncodeHtml;
			base.PrepareControlHierarchy();
		}
		public override void RegisterEditorIncludeScripts() {
		}
		protected override string GetStartupScript() {
			return string.Empty;
		}
		protected override string GetOnClickNormal() {
			return string.Format(ClickHandlerName, CheckListBase.ClientID, ControlIndex);
		}
		protected override string GetOnClickReadonly() {
			return string.Format(ReadonlyClickHandlerName, CheckListBase.ClientID);
		}
		protected internal override string GetOnGotFocus() {
			return CheckListBase.GetOnGotFocus();
		}
		protected internal override string GetOnLostFocus() {
			return CheckListBase.GetOnLostFocus();
		}
		protected internal override string GetInputName() {
			return CheckListBase.UniqueID + "_RB";
		}
		protected internal override string GetInputValue() {
			return string.Empty;
		}
		public override void RegisterStyleSheets() {
		}
		protected override DisabledStyle GetDisabledStyle() {
			return new DisabledStyle();
		}
		protected override bool IsDefaultAppearanceEnabled() {
			return false;
		}
	}
}

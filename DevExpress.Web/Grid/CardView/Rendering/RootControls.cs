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

using DevExpress.Utils;
using DevExpress.Web.Data;
using DevExpress.Web.FilterControl;
using DevExpress.Web.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
namespace DevExpress.Web.Rendering {
	public class CardViewUpdatableContainer : GridUpdatableContainer {
		public CardViewUpdatableContainer(ASPxCardView grid)
			: base(grid) {
		}
		protected new ASPxCardView Grid { get { return base.Grid as ASPxCardView; } }
		protected new CardViewRenderHelper RenderHelper { get { return base.RenderHelper as CardViewRenderHelper; } }
		protected override Table CreateMainTable() {
			if(RenderHelper.IsFlowLayout)
				return new CardViewHtmlFlowLayoutMainTable(RenderHelper);
			return new CardViewHtmlTableLayoutMainTable(RenderHelper);
		}
		protected override GridHtmlStatusBar CreateStatusBar() {
			return new CardViewHtmlStatusBar(RenderHelper);
		}
		protected override void CreateFooterPanel() {
			if(Grid.Settings.ShowSummaryPanel)
				Controls.Add(new CardViewHtmlSummaryPanel(RenderHelper));
		}
		protected override void CreateAdaptiveHeaderPanel() {
			if(Grid.Settings.ShowHeaderPanel)
				AddControl(new CardViewHtmlHeaderPanel(Grid), CardViewRenderHelper.HeaderPanelID);
		}
		protected override GridCustomizationWindow CreateCustWindowControl() {
			return new CardViewCustomizationWindow(Grid);
		}
		protected override GridEditFormPopup CreateEditFormPopupControl() {
			return new CardViewEditFormPopup(Grid, DataProxy.EditingRowVisibleIndex);
		}
		protected override GridHtmlScrollableControl CreateScrollableControl() {
			return new CardViewHtmlScrollableControl(RenderHelper);
		}
		protected override void CreateEndlessPagingAdditionalControls() {
			base.CreateEndlessPagingAdditionalControls();
			if(!RenderHelper.ShowVerticalScrolling && Grid.SettingsPager.EndlessPagingMode == CardViewEndlessPagingMode.OnClick)
				AddControl(new EndlessPagingMoreButtonContainer(RenderHelper), CardViewRenderHelper.EndlessPagingMoreButtonContainer);
		}
	}
	public class CardViewHtmlHeaderPanel : ASPxInternalWebControl {
		public CardViewHtmlHeaderPanel(ASPxCardView grid) {
			Grid = grid;
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }				
		protected ASPxCardView Grid { get; set; }
		protected CardViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		protected override void CreateControlHierarchy() {
			if(RenderHelper.AddHeaderPanelTemplateControl(this))
				return;
			foreach(CardViewColumn column in ColumnHelper.AllVisibleDataColumns)
				AddColumnHeader(column);
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetHeaderPanelStyle().AssignToControl(this);
		}
		void AddColumnHeader(CardViewColumn column){
			if(column.ColumnAdapter.AllowSort || column.ColumnAdapter.HasFilterButton)
				Controls.Add(new CardViewHtmlHeader(RenderHelper, column));
		}
	}
	public class CardViewHtmlHeader : CardViewHeaderCell {
		public CardViewHtmlHeader(CardViewRenderHelper renderHelper, CardViewColumn column) : base(renderHelper, column) { }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
	}
	public class CardViewHtmlScrollableControl : GridHtmlScrollableControl {
		public CardViewHtmlScrollableControl(CardViewRenderHelper renderHelper)
			: base(renderHelper) {
		}
		protected new ASPxCardView Grid { get { return (ASPxCardView)base.Grid; } }
		protected new CardViewRenderHelper RenderHelper { get { return (CardViewRenderHelper)base.RenderHelper; } }
		protected override Table CreateContentTable(WebControl container) {
			Table table = CreateTable();
			table.ID = CardViewRenderHelper.MainTableID;
			ContentScrollDiv.Controls.Add(table);
			if(RenderHelper.UseEndlessPaging && Grid.SettingsPager.EndlessPagingMode == CardViewEndlessPagingMode.OnClick)
				ContentScrollDiv.Controls.Add(new EndlessPagingMoreButtonContainer(RenderHelper) { ID = CardViewRenderHelper.EndlessPagingMoreButtonContainer });
			return table;
		}
		protected override void CreateEndlessPagingLoadingPanelContainer() {
			if(Grid.SettingsPager.EndlessPagingMode == CardViewEndlessPagingMode.OnClick) 
				return;
			base.CreateEndlessPagingLoadingPanelContainer();
		}
		Table CreateTable() {
			if(RenderHelper.IsFlowLayout)
				return new CardViewHtmlFlowLayoutMainTable(RenderHelper);
			return new CardViewHtmlTableLayoutMainTable(RenderHelper);
		}
	}
	public abstract class CardViewHtmlMainTableBase : GridHtmlMainTable {
		public CardViewHtmlMainTableBase(CardViewRenderHelper renderHelper)
			: base(renderHelper) {
		}
		protected new CardViewRenderHelper RenderHelper { get { return base.RenderHelper as CardViewRenderHelper; } }
		protected new ASPxCardView Grid { get { return base.Grid as ASPxCardView; } }
		protected ASPxCardViewPagerSettings SettingsPager { get { return Grid.SettingsPager; } }
		protected override bool RequireUseDblClick { get { return !string.IsNullOrEmpty(Grid.ClientSideEvents.CardDblClick) || RenderHelper.IsBatchEditDblClickStartAction; } }
		protected bool CanCreateNewCard(GridViewNewItemRowPosition position) {
			if(Grid.SettingsEditing.NewItemPosition != position)
				return false;
			return DataProxy.IsNewRowEditing || RenderHelper.AllowBatchEditing;
		}
		protected override void CreateControlHierarchy() {
			if(CanCreateNewCard(GridViewNewItemRowPosition.Top))
				CreateNewCard();
			if(DataProxy.VisibleRowCount > 0)
				CreateDataCards();
			else if(!DataProxy.IsNewRowEditing)
				CreateEmptyCard();
			if(CanCreateNewCard(GridViewNewItemRowPosition.Bottom))
				CreateNewCard();
		}
		protected abstract void CreateNewCard();
		protected abstract void CreateDataCards();
		protected abstract void CreateEmptyCard(); 
	}
	public class CardViewHtmlTableLayoutMainTable : CardViewHtmlMainTableBase {
		public CardViewHtmlTableLayoutMainTable(CardViewRenderHelper renderHelper)
			: base(renderHelper) {
			SeparatorCells = new List<TableCell>();
		}
		protected int LayoutRowsPerPage { get { return (int)Math.Ceiling(DataProxy.VisibleRowCountOnPage / (double)SettingsPager.SettingsTableLayout.ColumnCount); } }
		protected int LayoutColumnCount { get { return SettingsPager.SettingsTableLayout.ColumnCount; } }
		protected int SeparatorColSpan { get { return LayoutColumnCount + LayoutColumnCount > 1 ? LayoutColumnCount - 1 : 0; } } 
		protected List<TableCell> SeparatorCells { get; private set; }
		protected override void CreateDataCards() {
			var colCount = LayoutColumnCount;
			var rowCount = LayoutRowsPerPage;
			for(int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				if(RenderHelper.RequireEndlessPagingPartialLoad && rowIndex == 0)
					CreateSeparatorRow();
				var dataRow = CreateRow();
				for(int colIndex = 0; colIndex < colCount; colIndex++) {
					var cell = CreateCardCell(colIndex, rowIndex);
					dataRow.Cells.Add(cell);
					if(RequireSeparatorItem(colIndex, colCount))
						CreateSeparatorCell(dataRow);
				}
				if(RequireSeparatorItem(rowIndex, rowCount))
					CreateSeparatorRow();
			}
		}
		protected TableRow CreateOneCardRow(int cardVisibleIndex) {
			var dataRow = CreateRow();
			for(int i = 0; i < LayoutColumnCount; i++) {
				var visibleIndex = i == 0 ? cardVisibleIndex : -i;
				dataRow.Cells.Add(new CardViewHtmlCardCell(RenderHelper, visibleIndex));
				if(RequireSeparatorItem(i, LayoutColumnCount))
					CreateSeparatorCell(dataRow);
			}			
			return dataRow;
		}
		protected override void CreateNewCard() {
			if(Grid.SettingsEditing.NewItemPosition == GridViewNewItemRowPosition.Bottom)
				CreateSeparatorRow();
			CreateOneCardRow(WebDataProxy.NewItemRow);
			if(Grid.SettingsEditing.NewItemPosition == GridViewNewItemRowPosition.Top)
				CreateSeparatorRow();
			if(RenderHelper.AllowBatchEditing) {
				foreach(var row in Rows.OfType<TableRow>().Skip(Rows.Count - 2))
					row.Style["display"] = "none";
			}
		}
		protected override void CreateEmptyCard() {
			CreateOneCardRow(CardViewRenderHelper.EmptyCardVisibleIndex);
		}
		protected TableCell CreateCardCell(int colIndex, int rowIndex) {
			var visibleIndex = DataProxy.VisibleStartIndex + rowIndex * LayoutColumnCount + colIndex;
			return new CardViewHtmlCardCell(RenderHelper, visibleIndex);
		}
		protected TableRow CreateSeparatorRow() {
			var row = new CardViewHtmlTableLayoutSeparatorRow(RenderHelper, SettingsPager.SettingsTableLayout);
			Rows.Add(row);
			return row;
		}
		protected void CreateSeparatorCell(TableRow row) {
			var cell = CreateCell(row);
			SeparatorCells.Add(cell);
			cell.Controls.Add(RenderUtils.CreateDiv());
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareSeparatorCells();
		}
		protected virtual void PrepareSeparatorCells() {
			foreach(var cell in SeparatorCells)
				RenderHelper.GetSeparatorStyle().AssignToControl(cell);
		}
		bool RequireSeparatorItem(int elementIndex, int elementCount) {
			if(elementIndex == 0 && elementCount < 2)
				return false;
			return elementIndex != elementCount - 1;
		}
		TableRow CreateRow() {
			var row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			return row;
		}
		TableCell CreateCell(TableRow row) {
			var cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			return cell;
		}
	}
	public class CardViewHtmlTableLayoutSeparatorRow : InternalTableRow {
		public CardViewHtmlTableLayoutSeparatorRow(CardViewRenderHelper renderHelper, CardViewTableLayoutSettings tableLayout) {
			RenderHelper = renderHelper;
			SettingsTableLayout = tableLayout;
		}
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected CardViewTableLayoutSettings SettingsTableLayout { get; private set; }
		protected int LayoutRowsPerPage { get { return SettingsTableLayout.RowsPerPage; } }
		protected int LayoutColumnCount { get { return SettingsTableLayout.ColumnCount; } }
		protected int SeparatorColSpan { get { return LayoutColumnCount > 1 ? (2 * LayoutColumnCount - 1) : 0; } } 
		protected override void CreateControlHierarchy() {
			var cell = RenderUtils.CreateTableCell();
			Cells.Add(cell);
			cell.Controls.Add(RenderUtils.CreateDiv());
		}
		protected override void PrepareControlHierarchy() {
			if(Cells.Count < 1) return;
			var cell = Cells[0];
			cell.ColumnSpan = SeparatorColSpan;
			RenderHelper.GetSeparatorStyle().AssignToControl(cell);
		}
	}
	public class CardViewHtmlCardCell : InternalTableCell {
		public CardViewHtmlCardCell(CardViewRenderHelper renderHelper, int visibleIndex) {
			RenderHelper = renderHelper;
			VisibleIndex = visibleIndex;
			ID = RenderHelper.GetCardId(VisibleIndex);
		}
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected int VisibleIndex { get; private set; }
		protected override void CreateControlHierarchy() {
			Controls.Add(new CardViewHtmlCardContent(RenderHelper, VisibleIndex));
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetCardStyle(VisibleIndex).AssignToControl(this);
		}
	}
	public class CardViewHtmlFlowLayoutMainTable : CardViewHtmlMainTableBase {
		public CardViewHtmlFlowLayoutMainTable(CardViewRenderHelper renderHelper)
			: base(renderHelper) {
		}
		protected int ItemsPerPage { get { return SettingsPager.SettingsFlowLayout.ItemsPerPage; } }
		TableCell ContentCell { get; set; }
		protected override void CreateControlHierarchy() {
			var contentRow = RenderUtils.CreateTableRow();
			Controls.Add(contentRow);
			ContentCell = RenderUtils.CreateTableCell();
			contentRow.Controls.Add(ContentCell);
			base.CreateControlHierarchy();
		}
		protected override void CreateDataCards() {
			int visibleItemsCount = RenderHelper.DataProxy.VisibleRowCountOnPage;
			for(int i = 0; i < visibleItemsCount; i++) {
				Control cardDiv = CreateCardDiv(i);
				ContentCell.Controls.Add(cardDiv);
			}
		}
		protected override void CreateNewCard() {			
			ContentCell.Controls.Add(new CardViewHtmlCardDiv(RenderHelper, WebDataProxy.NewItemRow));
		}
		protected override void CreateEmptyCard() {
			ContentCell.Controls.Add(new CardViewHtmlCardDiv(RenderHelper, CardViewRenderHelper.EmptyCardVisibleIndex));			
		}
		Control CreateCardDiv(int index) {
			int visibleIndex = DataProxy.VisibleStartIndex + index;
			return new CardViewHtmlCardDiv(RenderHelper, visibleIndex);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CssClass = RenderUtils.CombineCssClasses(CssClass, RenderHelper.Styles.FlowTableClassName);
		}
		protected override IEnumerable<WebControl> GetEndlessPagingItems() {
			return ContentCell.Controls.OfType<WebControl>();
		}
	}
	public class CardViewHtmlCardDiv : InternalHtmlControl {
		public CardViewHtmlCardDiv(CardViewRenderHelper renderHelper, int visibleIndex)
			: base(HtmlTextWriterTag.Div) {
			RenderHelper = renderHelper;
			VisibleIndex = visibleIndex;
			ID = RenderHelper.GetCardId(VisibleIndex);
		}
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected int VisibleIndex { get; private set; }
		protected override void CreateControlHierarchy() {
			Controls.Add(new CardViewHtmlCardContent(RenderHelper, VisibleIndex));
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetCardStyle(VisibleIndex).AssignToControl(this);
			if(RenderHelper.IsBatchEditEtalonCard(VisibleIndex))
				Style["display"] = "none";
		}
	}
	public class CardViewHtmlCardContent : ASPxInternalWebControl {
		public CardViewHtmlCardContent(CardViewRenderHelper renderHelper, int visibleIndex) {
			RenderHelper = renderHelper;
			VisibleIndex = visibleIndex;
		}
		protected ASPxCardView Grid { get { return RenderHelper.Grid; } }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected int VisibleIndex { get; private set; }
		protected bool IsEmptyCard { get { return RenderHelper.IsEmptyCard(VisibleIndex); } }
		protected bool IsEmptyHiddenCard { get { return RenderHelper.IsEmptyHiddenCard(VisibleIndex); } }
		protected bool IsFlowLayoutMode { get { return Grid.Settings.LayoutMode == Layout.Flow; } }
		protected WebControl FlowLayoutEmptyCardWrapper { get; set; }
		protected WebControl FlowLayoutEmptyCardContainer { get; set; }
		protected WebControl EmptyCardContainer { get { return IsFlowLayoutMode ? FlowLayoutEmptyCardContainer : this; } }
		protected override void CreateControlHierarchy() {
			if(IsEmptyHiddenCard)
				return;
			if(IsEmptyCard)
				CreateEmptyCardContent();
			else
				CreateDataCardContent();
		}
		protected void CreateEmptyCardContent() {
			if(IsFlowLayoutMode)
				CreateFlowEmptyCardLayout();
			if(RenderHelper.ShowNewButtonInEmptyCard()) {
				var newButton = RenderHelper.CreateCommandButtonControl(GridCommandButtonType.New, -1, true);
				if(newButton != null)
					EmptyCardContainer.Controls.Add(newButton);
			}
			var textContainer = RenderUtils.CreateDiv();
			textContainer.Controls.Add(RenderUtils.CreateLiteralControl(Grid.SettingsText.GetEmptyDataRow()));
			EmptyCardContainer.Controls.Add(textContainer);
		}
		protected void CreateFlowEmptyCardLayout() {
			FlowLayoutEmptyCardWrapper = RenderUtils.CreateDiv();
			Controls.Add(FlowLayoutEmptyCardWrapper);
			FlowLayoutEmptyCardContainer = RenderUtils.CreateDiv();
			FlowLayoutEmptyCardWrapper.Controls.Add(FlowLayoutEmptyCardContainer);
		}
		protected void CreateDataCardContent(){
			if(Grid.Settings.ShowCardHeader)
				Controls.Add(new CardViewHtmlCardHeader(RenderHelper, VisibleIndex));
			Controls.Add(CreateCardLayoutContainer());
			if(Grid.Settings.ShowCardFooter)
				Controls.Add(new CardViewHtmlCardFooter(RenderHelper, VisibleIndex));
		}
		protected WebControl CreateCardLayoutContainer() {
			if(RenderHelper.IsCardEditing(VisibleIndex) && !Grid.SettingsEditing.DisplayEditingRow)
				return new CardViewHtmlEditCardLayoutContainer(RenderHelper, VisibleIndex);
			return new CardViewHtmlCardLayoutContainer(RenderHelper, VisibleIndex);
		}
		protected override void PrepareControlHierarchy() {
			if(FlowLayoutEmptyCardWrapper != null)
				FlowLayoutEmptyCardWrapper.CssClass = RenderHelper.Styles.FlowLayoutEmptyCardWrapperClassName;
		}
	}
	public class CardViewHtmlCardLayoutContainer : ASPxInternalWebControl {
		public CardViewHtmlCardLayoutContainer(CardViewRenderHelper renderHelper, int visibleIndex) {
			RenderHelper = renderHelper;
			VisibleIndex = visibleIndex;
		}
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected int VisibleIndex { get; private set; }
		protected bool HasCardTemplate { get { return RenderHelper.Grid.Templates.Card != null; } }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override bool HasRootTag() { return HasCardTemplate; }
		protected override void CreateControlHierarchy() {
			if(!RenderHelper.AddCardTemplateControl(VisibleIndex, this))
				Controls.Add(new CardViewHtmlCardLayout(RenderHelper, VisibleIndex));
		}
	}
	public class CardViewHtmlCardLayout : ASPxInternalWebControl, IInternalCheckBoxOwner, IValueProvider {
		public CardViewHtmlCardLayout(CardViewRenderHelper renderHelper, int visibleIndex) : this(renderHelper, visibleIndex, false) { }
		public CardViewHtmlCardLayout(CardViewRenderHelper renderHelper, int visibleIndex, bool createEditForm) {
			RenderHelper = renderHelper;
			VisibleIndex = visibleIndex;
			CreateEditForm = createEditForm;
		}
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected int VisibleIndex { get; private set; }
		protected ASPxCardView Grid { get { return RenderHelper.Grid; } }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		protected bool CreateEditForm { get; private set; }
		protected bool SelectCheckboxEnabled { get; set; }
		protected bool IsNewCardEditing { get { return DataProxy.IsNewRowEditing; } }
		protected bool PostponeButtonClick { get { return !RenderHelper.RequireRenderEditFormPopup || !CreateEditForm; } }
		protected CardViewFormLayoutProperties LayoutProperties { get { return CreateEditForm ? Grid.EditFormLayoutProperties : Grid.CardLayoutProperties; } }
		protected ASPxFormLayout FormLayout { get; private set; }
		protected override void CreateControlHierarchy() {
			CreateFormLayout();
			FormLayout.ForEach(PopulateLayoutItem);
		}
		protected override void PrepareControlHierarchy() {
			FormLayout.ForEach((item) => {
				var commandItem = item as CommandLayoutItem;
				if(commandItem == null) return;
				foreach(var control in commandItem.Controls) {
					var button = control as GridCommandColumnButtonControl;
					if(button != null)
						button.AssignInnerControlStyle(RenderHelper.GetCommandItemStyle());
				}
			});
		}
		protected virtual void CreateFormLayout() {
			LayoutProperties.ValidateLayoutItemColumnNames();
			FormLayout = new ASPxFormLayout();
			FormLayout.ID = RenderHelper.GetCardLayoutID(VisibleIndex);
			FormLayout.EnableViewState = false;
			FormLayout.ParentSkinOwner = Grid;
			FormLayout.Width = Unit.Percentage(100);
			FormLayout.Properties.DataOwner = LayoutProperties.DataOwner;
			FormLayout.EnableClientSideAPIInternal = RenderHelper.AllowBatchEditing;
			FormLayout.Properties.Assign(LayoutProperties);
			if(CreateEditForm && FormLayout.Items.IsEmpty) 
				FormLayout.Properties.Assign(Grid.CardLayoutProperties);
			if(FormLayout.Items.IsEmpty) {
				var prop = Grid.GenerateDefaultLayout(false);
				FormLayout.Properties.Assign(prop);
			}
			if(!CreateEditForm)
				FormLayout.CssClass = FormLayoutStyles.ViewFormLayoutSystemClassName;
			Controls.Add(FormLayout);
		}
		protected virtual void PopulateLayoutItem(LayoutItemBase item) {
			CreateCommandControl(item as CommandLayoutItem);
			CreateColumnControl(item as CardViewColumnLayoutItem);
		}
		protected virtual void CreateColumnControl(CardViewColumnLayoutItem item) {
			if(item == null) return;
			var column = item.Column;
			if(CreateEditForm)
				CreateEditorControl(item, column);
			else {
				CreateColumnDisplayControl(item, column);
				item.NestedControlCellStyle.CopyFrom(RenderHelper.GetConditionalFormatCellStyle(column, VisibleIndex));
			}
		}
		protected void CreateEditorControl(CardViewColumnLayoutItem layoutItem, CardViewColumn column) {			
			if(RenderHelper.AddEditItemTemplateControl(VisibleIndex, column, layoutItem)) 
				return;
			if (column != null)
				RenderHelper.CreateEditor(VisibleIndex, column, layoutItem.NestedControlContainer, EditorInplaceMode.EditForm, layoutItem.GetRowSpan(), true);
		}
		protected void CreateColumnDisplayControl(CardViewColumnLayoutItem layoutItem, CardViewColumn column) {
			if(RenderHelper.AddDataItemTemplateControl(VisibleIndex, column, layoutItem))
				return;
			if(column != null)
				RenderHelper.AddDisplayControlToDataCell(layoutItem.NestedControlContainer, column, VisibleIndex, this);
		}
		protected virtual void CreateCommandControl(CommandLayoutItem layoutItem) {
			CreateCommandButtons(layoutItem as CardViewCommandLayoutItem);
			CreateEditCommandButtons(layoutItem as EditModeCommandLayoutItem);
		}
		protected virtual void CreateCommandButtons(CardViewCommandLayoutItem layoutItem) {
			if(layoutItem == null) return;
			if(layoutItem.ShowSelectCheckbox)
				CreateCommandButtonControl(layoutItem, GridCommandButtonType.SelectCheckbox);
			if(layoutItem.ShowEditButton)
				CreateCommandButtonControl(layoutItem, GridCommandButtonType.Edit);
			if(layoutItem.ShowNewButton)
				CreateCommandButtonControl(layoutItem, GridCommandButtonType.New);
			if(layoutItem.ShowDeleteButton)
				CreateCommandButtonControl(layoutItem, GridCommandButtonType.Delete);
			if(layoutItem.ShowSelectButton)
				CreateCommandButtonControl(layoutItem, GridCommandButtonType.Select);
			foreach(CardViewCustomCommandButton customButton in layoutItem.CustomButtons)
				CreateCustomCommandButtonControl(layoutItem, customButton);
		}
		protected virtual void CreateEditCommandButtons(EditModeCommandLayoutItem layoutItem) {
			if(layoutItem == null) return;
			if(layoutItem.ShowUpdateButton)
				CreateCommandButtonControl(layoutItem, GridCommandButtonType.Update);
			if(layoutItem.ShowCancelButton)
				CreateCommandButtonControl(layoutItem, GridCommandButtonType.Cancel);
		}
		protected virtual void CreateCommandButtonControl(CommandLayoutItem layoutItem, GridCommandButtonType buttonType) {
			if(!CanShowCommandButton(buttonType)) return;
			WebControl control = null;
			if(buttonType == GridCommandButtonType.SelectCheckbox)
				control = CreateSelectCheckbox();
			else
				control = RenderHelper.CreateCommandButtonControl(buttonType, VisibleIndex, PostponeButtonClick);
			AddCommandButtonControl(layoutItem, control);
		}
		protected virtual void CreateCustomCommandButtonControl(CommandLayoutItem layoutItem, CardViewCustomCommandButton button) {
			var isRowEditing = RenderHelper.DataProxy.IsRowEditing(VisibleIndex);
			var e = new ASPxCardViewCustomCommandButtonEventArgs(button, VisibleIndex, isRowEditing);
			Grid.RaiseCustomButtonInitialize(e);
			switch(e.Visible) {
				case DefaultBoolean.False:
					return;
				case DefaultBoolean.Default: 
					break;
			}
			AddCommandButtonControl(layoutItem, new GridCommandColumnButtonControl(e, Grid, RenderHelper.Scripts.GetCustomButtonFuncArgs, PostponeButtonClick));
		}
		protected virtual WebControl CreateSelectCheckbox() {
			if(CreateEditForm) return null;
			var e = new ASPxCardViewCommandButtonEventArgs(CardViewCommandButtonType.SelectCheckbox, VisibleIndex, CreateEditForm); 
			Grid.RaiseCommandButtonInitialize(e);
			if(!e.Visible) return null;
			var checkBox = new InternalCheckboxControl(this);
			SelectCheckboxEnabled = RenderHelper.IsGridEnabled && e.Enabled;
			if(!SelectCheckboxEnabled)
				checkBox.MainElement.CssClass = RenderUtils.CombineCssClasses(checkBox.MainElement.CssClass, Grid.Styles.DisabledCheckboxClassName);
			return checkBox;
		}
		protected void AddCommandButtonControl(ContentPlaceholderLayoutItem layoutItem, WebControl buttonControl) {
			if(buttonControl != null) {
				CreateSpacerIfNeeded(layoutItem);
				layoutItem.Controls.Add(buttonControl);
			}
		}
		protected virtual void CreateSpacerIfNeeded(ContentPlaceholderLayoutItem layoutItem) {
			if(layoutItem.Controls.Count < 1) return;
		}
		protected Unit ItemSpacing { get { return 5; } } 
		protected bool CanShowCommandButton(GridCommandButtonType buttonType) {
			switch(buttonType) {
				case GridCommandButtonType.New:
				case GridCommandButtonType.Edit:
				case GridCommandButtonType.Delete:
				case GridCommandButtonType.Select:
				case GridCommandButtonType.SelectCheckbox:
					return !(CreateEditForm || RenderHelper.IsCardEditing(VisibleIndex) && Grid.SettingsEditing.DisplayEditingRow);
				case GridCommandButtonType.Update:
				case GridCommandButtonType.Cancel:
					return CreateEditForm;
			}
			return false;
		}
		#region IInternalCheckBoxOwner
		CheckState IInternalCheckBoxOwner.CheckState { get { return DataProxy.Selection.IsRowSelected(VisibleIndex) ? CheckState.Checked : CheckState.Unchecked; } }
		bool IInternalCheckBoxOwner.ClientEnabled { get { return SelectCheckboxEnabled; } }
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() { return RenderHelper.GetCheckImage((this as IInternalCheckBoxOwner).CheckState, RenderHelper.AllowSelectSingleRowOnly); }
		string IInternalCheckBoxOwner.GetCheckBoxInputID() { return RenderHelper.GetSelectButtonId(VisibleIndex); }
		bool IInternalCheckBoxOwner.IsInputElementRequired { get { return true; } }
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle { get { return RenderHelper.GetCheckBoxStyle(); } }
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes { get { return null; } }
		#endregion
		object IValueProvider.GetValue(string fieldName) {
			return DataProxy.GetRowValue(VisibleIndex, fieldName);
		}
	}
	public class CardViewHtmlCardHeader : InternalHtmlControl {
		public CardViewHtmlCardHeader(CardViewRenderHelper renderHelper, int visibleIndex)
			: base(HtmlTextWriterTag.Div) {
			RenderHelper = renderHelper;
			VisibleIndex = visibleIndex;
		}
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected int VisibleIndex { get; private set; }
		protected override void CreateControlHierarchy() {
			if(RenderHelper.AddCardHeaderTemplateControl(VisibleIndex, this)) return;
			Controls.Add(new LiteralControl("Header")); 
		}
		protected override void PrepareControlHierarchy() { } 
	}
	public class CardViewHtmlCardFooter : InternalHtmlControl {
		public CardViewHtmlCardFooter(CardViewRenderHelper renderHelper, int visibleIndex)
			: base(HtmlTextWriterTag.Div) {
			RenderHelper = renderHelper;
			VisibleIndex = visibleIndex;
		}
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected int VisibleIndex { get; private set; }
		protected override void CreateControlHierarchy() {
			if(RenderHelper.AddCardFooterTemplateControl(VisibleIndex, this)) return;
			Controls.Add(new LiteralControl("Footer")); 
		}
		protected override void PrepareControlHierarchy() { } 
	}
	public class CardViewHtmlSummaryPanel : GridHtmlSummaryPanel {
		public CardViewHtmlSummaryPanel(CardViewRenderHelper renderHelper) : base(renderHelper) { }
		public new CardViewRenderHelper RenderHelper { get { return base.RenderHelper as CardViewRenderHelper; } }
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetSummaryPanelStyle().AssignToControl(this);
			SummaryItemControls.ForEach(c => RenderHelper.GetSummaryItemStyle().AssignToControl(c)); 
		}
	}
	public class CardViewEditFormPopup : GridEditFormPopup {
		public CardViewEditFormPopup(ASPxCardView grid, int visibleIndex)
			: base(grid, visibleIndex) {
		}
		public new CardViewRenderHelper RenderHelper { get { return base.RenderHelper as CardViewRenderHelper; } }
		protected override WebControl CreateContentContainer() {
			return new CardViewHtmlCardLayout(RenderHelper, VisibleIndex, true);
		}
		protected override WebControl CreateErrorHtmlControl() {
			return RenderHelper.HasEditingError ? new CardViewHtmlCardErrorContainer(RenderHelper) : RenderUtils.CreateDiv();
		}
		protected override string GetPopupElementID() {
			if(RenderHelper.DataProxy.VisibleRowCountOnPage > 0)
				return RenderHelper.GetCardId(VisibleIndex);
			return base.GetPopupElementID();
		}
	}
	public class CardViewHtmlEditCardLayoutContainer : ASPxInternalWebControl {
		public CardViewHtmlEditCardLayoutContainer(CardViewRenderHelper renderHelper, int visibleIndex) {
			RenderHelper = renderHelper;
			VisibleIndex = visibleIndex;
		}
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected int VisibleIndex { get; private set; }
		protected bool HasEditFormTemplate { get { return RenderHelper.Grid.Templates.EditForm != null; } }
		protected bool HasError { get { return RenderHelper.HasEditingError; } }
		protected override bool HasRootTag() { return HasEditFormTemplate || HasError; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected WebControl ErrorCell { get; set; }
		protected WebControl EditFormCell { get; set; }
		protected WebControl EditFormContainer { get { return HasError ? EditFormCell : this; } }
		protected override void CreateControlHierarchy() {
			if(HasError)
				CreateErrorLayout();
			if(!RenderHelper.AddEditFormTemplateControl(EditFormContainer, VisibleIndex))
				EditFormContainer.Controls.Add(new CardViewHtmlCardLayout(RenderHelper, VisibleIndex, true));
		}
		void CreateErrorLayout() {
			EditFormCell = CreateCell();
			ErrorCell = CreateCell();
			ErrorCell.Controls.Add(new CardViewHtmlCardErrorContainer(RenderHelper));				
		}
		WebControl CreateCell() {
			var row = RenderUtils.CreateDiv();
			Controls.Add(row);
			var cell = RenderUtils.CreateDiv();
			row.Controls.Add(cell);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			if(!HasError) return;
			CssClass = RenderHelper.Styles.EditCardErrorLayoutContainerClassName;
			ErrorCell.CssClass = RenderHelper.Styles.EditCardErrorLayoutContainerErrorCellClassName;
		}
	}
	public class CardViewHtmlStatusBar : GridHtmlStatusBar {
		public CardViewHtmlStatusBar(CardViewRenderHelper renderHelper)
			: base(renderHelper) {
		}
		protected new CardViewRenderHelper RenderHelper { get { return base.RenderHelper as CardViewRenderHelper; } }
		protected override TableCell CreateCommandItemsCell() {
			return new CardViewCommandItemsCell(RenderHelper, GetAllowedCommandItems());
		}
	}
	public class CardViewCommandItemsCell : GridCommandButtonsCell {
		public CardViewCommandItemsCell(CardViewRenderHelper renderHelper, IEnumerable<GridCommandButtonType> buttonTypes)
			: base(renderHelper, buttonTypes) {
		}
		protected override Unit ItemSpacing { get { return Unit.Empty; } } 
		protected override GridCommandColumnButtonControl CreateButtonControl(GridCommandButtonType buttonType) {
			return RenderHelper.CreateCommandButtonControl(buttonType, -1, false); 
		}
	}
	public class EndlessPagingMoreButtonContainer : ASPxInternalWebControl {
		public EndlessPagingMoreButtonContainer(CardViewRenderHelper renderHelper)
			: base() {
			RenderHelper = renderHelper;
		}
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void CreateControlHierarchy() {
			var buttonDiv = CreateDiv(CardViewRenderHelper.EndlessPagingMoreButtonDivID);
			buttonDiv.Controls.Add(RenderHelper.CreateCommandButtonControl(GridCommandButtonType.EndlessShowMoreCards, -1, false));
			if(RenderHelper.Grid.SettingsLoadingPanel.Mode == GridViewLoadingPanelMode.Default)
				CreateDiv(GridRenderHelper.EndlessPagingLoadingPanelContainerID);
		}
		protected override void PrepareControlHierarchy() {
			CssClass = RenderHelper.Styles.EndlessPagingMoreButtonContainerClassName;
		}
		protected WebControl CreateDiv(string id) {
			WebControl div = RenderUtils.CreateDiv();
			div.ID = id;
			Controls.Add(div);
			return div;
		}
	}
	public class CardViewHtmlCardErrorContainer : ASPxInternalWebControl {
		public CardViewHtmlCardErrorContainer(CardViewRenderHelper renderHelper)
			: base() {
			RenderHelper = renderHelper;
			ID = GridViewRenderHelper.EditingErrorItemID;
		}
		protected CardViewRenderHelper RenderHelper { get; private set; }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void CreateControlHierarchy() {
			if(!RenderHelper.AllowBatchEditing)
				Controls.Add(new LiteralControl(RenderHelper.EditingErrorText));
		}
		protected override void PrepareControlHierarchy() {
			RenderHelper.GetCardErrorStyle().AssignToControl(this);
		}
	}
	[ToolboxItem(false)]
	public class ASPxCardViewPager : ASPxGridPager {
		public ASPxCardViewPager(ASPxCardView grid)
			: base(grid) {
		}
		protected new ASPxCardView Grid { get { return (ASPxCardView)base.Grid; } }
		protected internal override int GetPageSize() {
			if(Grid.RenderHelper.IsFlowLayout)
				return Grid.SettingsPager.PageSize;
			return Grid.SettingsPager.SettingsTableLayout.RowsPerPage;
		}
	}
}

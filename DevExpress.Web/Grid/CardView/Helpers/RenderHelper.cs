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

using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Cookies;
using DevExpress.Web.Rendering;
using System;
using System.Linq;
using DevExpress.Data;
using System.Collections.Generic;
using System.Text;
using DevExpress.Web.Data;
namespace DevExpress.Web.Internal {
	public class CardViewTextBuilder : GridTextBuilder {
		public CardViewTextBuilder(ASPxCardView grid)
			: base(grid) {
		}
		protected ASPxCardView CardView { get { return (ASPxCardView)Grid; } }
		protected override string GetEditorDisplayTextCore(EditPropertiesBase editor, IWebGridDataColumn column, int visibleIndex, object value, bool encodeValue = false) {
			return editor.GetExportDisplayText(GetDisplayControlArgsCore(column, visibleIndex, value));
		}
		public CreateDisplayControlArgs GetDisplayControlArgs(IWebGridDataColumn column, int visibleIndex) {
			return GetDisplayControlArgsCore(column, visibleIndex, DataProxy.GetRowValue(visibleIndex, column.FieldName));
		}
		protected internal override CreateDisplayControlArgs GetDisplayControlArgsCore(IWebGridDataColumn column, int visibleIndex, IValueProvider provider, ASPxGridBase grid, object value) {
			CreateDisplayControlArgs controlArgs = base.GetDisplayControlArgsCore(column, visibleIndex, provider, grid, value);
			controlArgs.EncodeHtml = false;
			return controlArgs;
		}
		protected override ASPxGridSummaryDisplayTextEventArgs GetSummaryDisplayTextEventArgs(ASPxSummaryItemBase item, object value, string text) {
			return new ASPxCardViewSummaryDisplayTextEventArgs((ASPxCardViewSummaryItem)item, value, text);
		}
	}
	public class CardViewRenderHelper : GridRenderHelper {
		public const string 
			DataCardID = "DXDataCard",
			EmptyCardID = "DXEmptyCard",
			EditingCardID = "DXEditingCard",
			HeaderPanelID = "HeaderPanel",
			CustomizationButtonID = "CustB",
			CardLayoutID = "DXCardLayout",
			EndlessPagingMoreButtonContainer = "EPMBC",
			EndlessPagingMoreButtonDivID = "EPMBD";
		public const int 
			EmptyCardVisibleIndex = -1000000;
		public CardViewRenderHelper(ASPxCardView cardView)
			: base(cardView) {
			CardHeaderTemplates = new TemplateContainerCollection(Grid);
			CardFooterTemplates = new TemplateContainerCollection(Grid);
			CardTemplates = new TemplateContainerCollection(Grid);
			DataItemTemplates = new TemplateContainerCollection(Grid);
			EditItemTemplates = new TemplateContainerCollection(Grid);
			EditFormTemplates = new TemplateContainerCollection(Grid);
			HeaderTemplates = new TemplateContainerCollection(Grid);
			PagerBarTemplates = new TemplateContainerCollection(Grid);
			TitleTemplates = new TemplateContainerCollection(Grid);
			StatusBarTemplates = new TemplateContainerCollection(Grid);
			HeaderPanelTemplates = new TemplateContainerCollection(Grid);
		}
		public new ASPxCardView Grid { get { return base.Grid as ASPxCardView; } }
		public new CardViewTextBuilder TextBuilder { get { return base.TextBuilder as CardViewTextBuilder; } }
		public new CardViewStyles Styles { get { return (CardViewStyles)Grid.Styles; } }
		public new ASPxCardViewScripts Scripts { get { return base.Scripts as ASPxCardViewScripts; } }
		public TemplateContainerCollection CardHeaderTemplates { get; private set; }
		public TemplateContainerCollection CardFooterTemplates { get; private set; }
		public TemplateContainerCollection CardTemplates { get; private set; }
		public TemplateContainerCollection DataItemTemplates { get; private set; }
		public TemplateContainerCollection EditItemTemplates { get; private set; }
		public TemplateContainerCollection EditFormTemplates { get; private set; }
		public TemplateContainerCollection HeaderTemplates { get; private set; }
		public TemplateContainerCollection PagerBarTemplates { get; private set; }
		public TemplateContainerCollection TitleTemplates { get; private set; }
		public TemplateContainerCollection StatusBarTemplates { get; private set; }
		public TemplateContainerCollection HeaderPanelTemplates { get; private set; }
		public bool ShowHeaderCard { get { return true; } }
		public bool IsEmptyCard(int visibleIndex) { return visibleIndex == EmptyCardVisibleIndex; }
		public bool IsBatchEditEtalonCard(int visibleIndex) { return visibleIndex == WebDataProxy.NewItemRow && AllowBatchEditing; }
		public bool IsEmptyHiddenCard(int visibleIndex) { 
			if(DataProxy.IsEditing && visibleIndex == DataProxy.EditingRowVisibleIndex || IsEmptyCard(visibleIndex) || IsBatchEditEtalonCard(visibleIndex))
				return false;
			return visibleIndex < 0 || visibleIndex >= DataProxy.VisibleRowCount;
		}
		public string GetCardId(int visibleIndex) {
			if(IsEmptyCard(visibleIndex))
				return EmptyCardID;
			if(IsEmptyHiddenCard(visibleIndex))
				return string.Empty;
			if(DataProxy.EditingRowVisibleIndex == visibleIndex)
				return EditingCardID;
			var postfix = visibleIndex.ToString();
			if(IsBatchEditEtalonCard(visibleIndex))
				postfix = "new";
			return DataCardID + postfix;
		}
		public string GetCardLayoutID(int visibleIndex) {
			var postfix = visibleIndex.ToString();
			if(IsBatchEditEtalonCard(visibleIndex))
				postfix = "Etalon";
			return CardViewRenderHelper.CardLayoutID + postfix;
		}
		protected override GridCookiesBase CreateGridSEO() {
			return new CardViewCookies(Grid);
		}
		protected override ASPxGridScripts CreateGridScripts() {
			return new ASPxCardViewScripts(Grid);
		}
		protected override GridTextBuilder CreateTextBuilder() {
			return new CardViewTextBuilder(Grid);
		}
		public bool IsFlowLayout { get { return Grid.Settings.LayoutMode == Layout.Flow; } }
		protected override bool HasAnySelectCheckBoxInternal {
			get {
				var result = false;
				Grid.CardLayoutProperties.ForEach((item) => {
					var commandItem = item as CardViewCommandLayoutItem;
					result |= commandItem != null && commandItem.ShowSelectCheckbox;
				});
				return result;
			}
		}
		public override bool IsEndlessPagingRequireControlScrolling { get { return false; } }
		public override Unit GetRootTableWidth() {
			if(!Grid.Width.IsEmpty)
				return Grid.Width;
			if(!ShowVerticalScrolling) 
				return Unit.Empty;
			var colCount = Grid.Settings.LayoutMode == Layout.Table ? Grid.SettingsPager.SettingsTableLayout.ColumnCount : 3;
			return colCount * 250;
		}
		public AppearanceStyle GetCardStyle(int visibleIndex) {
			if(IsEmptyHiddenCard(visibleIndex))
				return GetEmptyHiddenCardStyle();
			if(IsEmptyCard(visibleIndex))
				return GetEmptyCardStyle();
			return GetRegularCardStyle(visibleIndex);
		}
		public AppearanceStyle GetRegularCardStyle(int visibleIndex) {
			if(Grid.EditingCardVisibleIndex == visibleIndex && !RequireRenderEditFormPopup)
				return GetEditFormCardStyle();
			CardViewCardStyle style;
			AppearanceStyle formatConditionsStyle = GetConditionalFormatItemStyle(visibleIndex);
			if(formatConditionsStyle.IsEmpty)
				style = GetCardStyle();
			else {
				style = new CardViewCardStyle();
				style.CopyFrom(GetCardStyle());
				style.CopyFrom(formatConditionsStyle);
			}
			return style;
		}
		class CardViewStyleCacheKeys {
			public static readonly object
				Card = new object(),
				FlowCard = new object();
		}
		public CardViewCardStyle GetCardStyle() {
			return IsFlowLayout ? GetFlowCardStyle() : GetTableCardStyle();
		}
		public CardViewCardStyle GetEmptyCardStyle() {
			return MergeStyle<CardViewCardStyle>(CardViewStyles.EmptyCardStyleName, Styles.EmptyCard);
		}
		public CardViewCardStyle GetEmptyHiddenCardStyle() {
			return MergeStyle<CardViewCardStyle>(CardViewStyles.EmptyHiddenCardStyleName);
		}
		protected CardViewCardStyle GetTableCardStyle() {
			return Grid.GetCachedStyle<CardViewCardStyle>(
				() => MergeStyle<CardViewCardStyle>(CardViewStyles.CardStyleName, Styles.Card),
				CardViewStyleCacheKeys.Card
			);
		}
		protected CardViewCardStyle GetFlowCardStyle() {
			return Grid.GetCachedStyle<CardViewCardStyle>(
				() => MergeStyle<CardViewCardStyle>(CardViewStyles.FlowCardStyleName, Styles.FlowCard),
				CardViewStyleCacheKeys.FlowCard
			);
		}
		public CardViewCardStyle GetSelectedCardStyle() {
			return MergeStyle<CardViewCardStyle>(CardViewStyles.SelectedCardStyleName, Styles.SelectedCard);
		}
		public CardViewCardStyle GetFocusedCardStyle() {
			return MergeStyle<CardViewCardStyle>(CardViewStyles.FocusedCardStyleName, Styles.FocusedCard);
		}
		public CardViewCardStyle GetEditFormCardStyle() {
			return MergeStyle<CardViewCardStyle>(CardViewStyles.EditFormCardStyleName, GetCardStyle(), Styles.EditFormCard);
		}
		public CardViewStyleBase GetHeaderPanelStyle() {
			return MergeStyle<CardViewStyleBase>(CardViewStyles.HeaderPanelStyleName, Styles.HeaderPanel);
		}
		public override AppearanceStyleBase GetRowHotTrackStyle() {
			return MergeStyle<CardViewStyleBase>(CardViewStyles.CardHoverStyleName, Styles.CardHotTrack);
		}
		public AppearanceStyleBase GetCardErrorStyle() {
			return MergeStyle<CardViewStyleBase>(CardViewStyles.CardErrorStyleName, Styles.CardError);
		}
		protected override GridHeaderStyle GetHeaderStyleCore(IWebGridColumn column) {
			CardViewHeaderStyle result = MergeStyle<CardViewHeaderStyle>(GridStyles.HeaderStyleName, Styles.Header);
			var gridColumn = column as CardViewColumn;
			if(gridColumn != null)
				result.CopyFrom(gridColumn.HeaderStyle);
			if(IsRightToLeft && result.HorizontalAlign == HorizontalAlign.NotSet)
				result.HorizontalAlign = HorizontalAlign.Right;
			return result;
		}
		public AppearanceStyle GetSeparatorStyle() {
			return MergeStyle<AppearanceStyle>(CardViewStyles.SeparatorStyleName);
		}
		public AppearanceStyle GetCommandItemStyle() {
			return MergeStyle<AppearanceStyle>(CardViewStyles.CommandItemStyleName, Styles.CommandItem);
		}
		public AppearanceStyle GetSummaryItemStyle() {
			return MergeStyle<AppearanceStyle>(CardViewStyles.SummaryItemStyleName, Styles.SummaryItem);
		}
		public AppearanceStyle GetSummaryPanelStyle() {
			return MergeStyle<AppearanceStyle>(CardViewStyles.SummaryPanelStyleName, Styles.SummaryPanel);
		}
		public override AppearanceStyle GetBatchEditCellStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.BatchEditCellStyleName, Styles.BatchEditCell);
		}
		public override AppearanceStyle GetBatchEditModifiedCellStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.BatchEditModifiedCellStyleName, Styles.BatchEditModifiedCell);
		}
		public bool AddCardHeaderTemplateControl(int visibleIndex, Control templateContainer) {
			ITemplate template = Grid.Templates.CardHeader;
			if(template == null || IsGridExported)
				return false;
			AddTemplateToControl(templateContainer, template, new CardViewCardHeaderTemplateContainer(Grid, visibleIndex, DataProxy.GetRowForTemplate(visibleIndex)), CardHeaderTemplates);
			return true;
		}
		public bool AddCardFooterTemplateControl(int visibleIndex, Control templateContainer) {
			ITemplate template = Grid.Templates.CardFooter;
			if(template == null || IsGridExported)
				return false;
			AddTemplateToControl(templateContainer, template, new CardViewCardFooterTemplateContainer(Grid, visibleIndex, DataProxy.GetRowForTemplate(visibleIndex)), CardFooterTemplates);
			return true;
		}
		public bool AddCardTemplateControl(int visibleIndex, Control templateContainer) {
			ITemplate template = Grid.Templates.Card;
			if(template == null || IsGridExported)
				return false;
			AddTemplateToControl(templateContainer, template, new CardViewCardTemplateContainer(Grid, visibleIndex, DataProxy.GetRowForTemplate(visibleIndex)), CardTemplates);
			return true;
		}
		public bool AddDataItemTemplateControl(int visibleIndex, CardViewColumn column, CardViewColumnLayoutItem layoutItem) {
			ITemplate template = GetTemplate(Grid.Templates.DataItem, GetColumnDataItemTemplate(column), layoutItem.Template);
			if(template == null || IsGridExported)
				return false;
			AddDataItemTemplateControlCore(layoutItem, template, new CardViewDataItemTemplateContainer(Grid, visibleIndex, DataProxy.GetRowForTemplate(visibleIndex), column, layoutItem));
			return true;
		}
		protected virtual void AddDataItemTemplateControlCore(CardViewColumnLayoutItem layoutItem, ITemplate template, CardViewDataItemTemplateContainer templateContainer) {
			AddTemplateToControl(layoutItem.NestedControlContainer, template, templateContainer, DataItemTemplates);
		}
		public override bool AddHeaderTemplateControl(IWebGridColumn column, Control templateContainer, GridHeaderLocation headerLocation) {
			var gridColumn = column as CardViewColumn;
			if(gridColumn == null)
				return false;
			ITemplate template = GetTemplate(Grid.Templates.Header, gridColumn.HeaderTemplate);
			if(template == null || IsGridExported)
				return false;
			AddTemplateToControl(templateContainer, template, new CardViewHeaderTemplateContainer(gridColumn, headerLocation), HeaderTemplates);
			return true;
		}
		public override bool AddPagerBarTemplateControl(WebControl templateContainer, GridViewPagerBarPosition position, string pagerId) {
			ITemplate template = Grid.Templates.PagerBar;
			if(template == null || IsGridExported)
				return false;
			AddTemplateToControl(templateContainer, template, new CardViewPagerBarTemplateContainer(Grid, position, pagerId), PagerBarTemplates);
			return true;
		}
		public bool AddEditItemTemplateControl(int visibleIndex, CardViewColumn column, CardViewColumnLayoutItem layoutItem) {
			ITemplate template = GetTemplate(Grid.Templates.EditItem, GetColumnEditItemTemplate(column), layoutItem.Template);
			if(template == null || IsGridExported)
				return false;
			AddEditItemTemplateControlCore(visibleIndex, column, template, layoutItem);
			return true;
		}
		protected virtual void AddEditItemTemplateControlCore(int visibleIndex, CardViewColumn column, ITemplate template, CardViewColumnLayoutItem layoutItem) {
			AddTemplateToControl(layoutItem.NestedControlContainer, template, new CardViewEditItemTemplateContainer(Grid, visibleIndex, DataProxy.GetRowForTemplate(visibleIndex), column, layoutItem), EditItemTemplates);
		}
		public bool AddEditItemTemplateControl(int visibleIndex, CardViewColumn column, WebControl templateContainer) {
			ITemplate template = GetTemplate(Grid.Templates.EditItem, GetColumnEditItemTemplate(column));
			if(template == null || IsGridExported)
				return false;
			AddTemplateToControl(templateContainer, template, new CardViewEditItemTemplateContainer(Grid, visibleIndex, DataProxy.GetRowForTemplate(visibleIndex), column, null), EditItemTemplates);
			return true;
		}
		public override bool AddEditFormTemplateControl(WebControl templateContainer, int visibleIndex) {
			ITemplate template = Grid.Templates.EditForm;
			if(template == null || IsGridExported)
				return false;
			AddTemplateToControl(templateContainer, template, new CardViewEditFormTemplateContainer(Grid, visibleIndex, DataProxy.GetRowForTemplate(visibleIndex)), EditFormTemplates);
			return true;
		}
		public override bool AddTitleTemplateControl(WebControl templateContainer) {
			ITemplate template = Grid.Templates.TitlePanel;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new CardViewTitleTemplateContainer(Grid), TitleTemplates);
			return true;
		}
		public override bool AddStatusBarTemplateControl(WebControl templateContainer) {
			ITemplate template = Grid.Templates.StatusBar;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new CardViewStatusBarTemplateContainer(Grid), StatusBarTemplates);
			return true;
		}
		public bool AddHeaderPanelTemplateControl(WebControl templateContainer) {
			ITemplate template = Grid.Templates.HeaderPanel;
			if(template == null || IsGridExported) return false;
			AddTemplateToControl(templateContainer, template, new CardViewHeaderPanelTemplateContainer(Grid), HeaderPanelTemplates);
			return true;
		}
		protected ITemplate GetColumnDataItemTemplate(CardViewColumn column) {
			return column != null ? column.DataItemTemplate : null;
		}
		protected ITemplate GetColumnEditItemTemplate(CardViewColumn column) {
			return column != null ? column.EditItemTemplate : null;
		}
		protected override void InvalidateTemplates() {
			base.InvalidateTemplates();
			CardHeaderTemplates.Clear();
			CardFooterTemplates.Clear();
			CardTemplates.Clear();
			DataItemTemplates.Clear();
			EditItemTemplates.Clear();
			EditFormTemplates.Clear();
			HeaderTemplates.Clear();
			PagerBarTemplates.Clear();
			TitleTemplates.Clear();
			StatusBarTemplates.Clear();
			HeaderPanelTemplates.Clear();
		}
		public override GridCommandColumnButtonControl CreateCommandButtonControl(WebColumnBase column, GridCommandButtonType buttonType, int visibleIndex, bool postponeClick) { 
			if(!CanCreateCommandButton(buttonType))
				return null;
			var buttonSettings = GetCommandButtonSettings(buttonType);
			var renderType = GetButtonType(buttonSettings, GridCommandButtonRenderMode.Default);
			var style = new ButtonControlStyles(null);
			style.CopyFrom(buttonSettings.Styles);
			var image = new ImageProperties();
			image.CopyFrom(buttonSettings.Image);
			var text = buttonSettings.Text;
			if(string.IsNullOrEmpty(text))
				text = Grid.SettingsText.GetCommandButtonText(buttonType, AllowBatchEditing);
			var isNewRow = DataProxy.IsNewRowEditing && visibleIndex == BaseListSourceDataController.NewItemRow;
			var isEditingRow = isNewRow || visibleIndex >= 0 && DataProxy.IsRowEditing(visibleIndex);
			var args = new ASPxCardViewCommandButtonEventArgs(ConvertButtonType(buttonType), visibleIndex, isEditingRow, renderType, text, image, style); 
			Grid.RaiseCommandButtonInitialize(args);
			if(!args.Visible)
				return null;
			return new GridCommandColumnButtonControl(args, Grid, GetCommandButtonClickHandlerArgs(buttonType), postponeClick);
		}
		protected override GridCommandButtonSettings GetCommandButtonSettings(GridCommandButtonType buttonType) {
			switch(buttonType) {
				case GridCommandButtonType.EndlessShowMoreCards:
					return Grid.SettingsCommandButton.EndlessPagingShowMoreCardsButton;
			}
			return base.GetCommandButtonSettings(buttonType);
		}
		protected override GetCommandColumnButtonClickHandlerArgs GetCommandButtonClickHandlerArgs(GridCommandButtonType buttonType) {
			switch(buttonType) {
				case GridCommandButtonType.EndlessShowMoreCards:
					return Scripts.GetEndlessShowMoreFuncArgs;
			}
			return base.GetCommandButtonClickHandlerArgs(buttonType);
		}
		public static CardViewCommandButtonType ConvertButtonType(GridCommandButtonType source) {
			switch(source) {
				case GridCommandButtonType.Edit:
					return CardViewCommandButtonType.Edit;
				case GridCommandButtonType.New:
					return CardViewCommandButtonType.New;
				case GridCommandButtonType.Delete:
					return CardViewCommandButtonType.Delete;
				case GridCommandButtonType.Select:
					return CardViewCommandButtonType.Select;
				case GridCommandButtonType.Update:
					return CardViewCommandButtonType.Update;
				case GridCommandButtonType.Cancel:
					return CardViewCommandButtonType.Cancel;
				case GridCommandButtonType.SelectCheckbox:
					return CardViewCommandButtonType.SelectCheckbox;
				case GridCommandButtonType.ApplySearchPanelFilter:
					return CardViewCommandButtonType.ApplySearchPanelFilter;
				case GridCommandButtonType.ClearSearchPanelFilter:
					return CardViewCommandButtonType.ClearSearchPanelFilter;
				case GridCommandButtonType.EndlessShowMoreCards:
					return CardViewCommandButtonType.EndlessPagingShowMoreCards;
			}
			throw new ArgumentException();
		}
		public bool IsCardEditing(int visibleIndex) {
			return DataProxy.IsEditing && DataProxy.EditingRowVisibleIndex == visibleIndex;
		}
		public bool ShowNewButtonInEmptyCard() {
			return Grid.CardLayoutProperties.Items.OfType<CardViewCommandLayoutItem>().Any(i => i.ShowNewButton);
		}
	}
	public class CardViewClientStylesInfo : GridClientStylesInfo {
		public CardViewClientStylesInfo(ASPxCardView grid)
			: base(grid) {
		}
		protected new ASPxCardView Grid { get { return (ASPxCardView)base.Grid; } }
		protected new CardViewRenderHelper RenderHelper { get { return (CardViewRenderHelper)base.RenderHelper; } }
		protected override AppearanceStyle GetSelectedItemStyle() { return RenderHelper.GetSelectedCardStyle(); }
		protected override AppearanceStyle GetFocusedItemStyle() { return RenderHelper.GetFocusedCardStyle(); }
		protected override WebControl CreateErrorItemControl() { return new CardViewHtmlCardErrorContainer(RenderHelper); }
	}
}

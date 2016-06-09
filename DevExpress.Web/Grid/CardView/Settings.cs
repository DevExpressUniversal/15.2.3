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

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.XtraGrid;
using DevExpress.Web.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum CardViewCommandButtonType { Edit, New, Delete, Select, Update, Cancel, SelectCheckbox, ApplySearchPanelFilter, ClearSearchPanelFilter, EndlessPagingShowMoreCards }
	public enum CardViewEditingMode { EditForm, PopupEditForm, Batch }
	public enum CardViewEndlessPagingMode { OnClick, OnScroll }
	public enum CardViewBatchEditMode { Cell, Card }
	public class ASPxCardViewPagerSettings : ASPxGridPagerSettings {
		public ASPxCardViewPagerSettings(ASPxCardView grid)
			: base(grid) {
			SettingsFlowLayout = new CardViewFlowLayoutSettings(grid);
			SettingsTableLayout = new CardViewTableLayoutSettings(grid);
		}
		protected internal new ASPxCardView Grid { get { return base.Grid as ASPxCardView; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPagerSettingsMode"),
#endif
 DefaultValue(GridViewPagerMode.ShowPager), NotifyParentProperty(true), AutoFormatDisable]
		public new GridViewPagerMode Mode { get { return base.Mode; } set { base.Mode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPagerSettingsPosition"),
#endif
 NotifyParentProperty(true), DefaultValue(PagerPosition.Bottom)]
		public override PagerPosition Position { get { return base.Position; } set { base.Position = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPagerSettingsSEOFriendly"),
#endif
 Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatDisable]
		public override SEOFriendlyMode SEOFriendly { get { return base.SEOFriendly; } set { base.SEOFriendly = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPagerSettingsAlwaysShowPager"),
#endif
 DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public new bool AlwaysShowPager { get { return base.AlwaysShowPager; } set { base.AlwaysShowPager = value; } }
		[ DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowEmptyCards { get { return ShowEmptyGridItems; } set { ShowEmptyGridItems = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPagerSettingsSettingsFlowLayout"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewFlowLayoutSettings SettingsFlowLayout { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPagerSettingsSettingsFlowLayout"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewTableLayoutSettings SettingsTableLayout { get; private set; }
		[ AutoFormatDisable, DefaultValue(CardViewEndlessPagingMode.OnClick), NotifyParentProperty(true)]
		public CardViewEndlessPagingMode EndlessPagingMode {
			get { return (CardViewEndlessPagingMode)GetEnumProperty("EndlessPagingMode", CardViewEndlessPagingMode.OnClick); }
			set {
				if(EndlessPagingMode == value) return;
				SetEnumProperty("EndlessPagingMode", CardViewEndlessPagingMode.OnClick, value);
				Changed();
			}
		}
		protected internal override int PageSize {
			get {
				if(Grid == null || Grid.Settings.LayoutMode == Layout.Table)
					return SettingsTableLayout.ItemsPerPage;
				return SettingsFlowLayout.ItemsPerPage;
			}
			set { }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as ASPxCardViewPagerSettings;
			if(src == null) return;
			SettingsFlowLayout.Assign(src.SettingsFlowLayout);
			SettingsTableLayout.Assign(src.SettingsTableLayout);
			EndlessPagingMode = src.EndlessPagingMode;
		}
		protected override PageSizeItemSettings CreatePageSizeItemSettings(IPropertiesOwner owner) {
			return new CardViewPageSizeItemSettings(this);
		}
	}
	public class CardViewFlowLayoutSettings : ASPxGridSettingsBase {
		protected internal const int DefaultItemsPerPage = 10;
		public CardViewFlowLayoutSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewFlowLayoutSettingsItemsPerPage"),
#endif
		DefaultValue(DefaultItemsPerPage), AutoFormatDisable, NotifyParentProperty(true)]
		public int ItemsPerPage {
			get {
				int itemsPerPage = GetIntProperty("ItemsPerPage", DefaultItemsPerPage);
				if(Grid != null)
					itemsPerPage = Grid.CallbackState.Get<int>("ItemsPerPage", DefaultItemsPerPage);
				return itemsPerPage;
			}
			set {
				if(ItemsPerPage == value) return;
				SetIntProperty("ItemsPerPage", DefaultItemsPerPage, value);
				if(Grid != null)
					Grid.CallbackState.Put("ItemsPerPage", value);
				Changed();
			}
		}
		protected new ASPxCardView Grid { get { return (ASPxCardView)base.Grid; } }
		protected ASPxCardViewPagerSettings SettingsPager { get { return Grid.SettingsPager; } }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as CardViewFlowLayoutSettings;
			if(src != null)
				ItemsPerPage = src.ItemsPerPage;
		}
	}
	public class CardViewTableLayoutSettings : ASPxGridSettingsBase {
		protected internal const int 
			DefaultColumnCount = 3,
			DefaultRowPerPage = 3;
		public CardViewTableLayoutSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewTableLayoutSettingsColumnCount"),
#endif
 DefaultValue(DefaultColumnCount), AutoFormatDisable, NotifyParentProperty(true)]
		public int ColumnCount {
			get { return GetIntProperty("ColumnCount", DefaultColumnCount); }
			set {
				if(ColumnCount == value) return;
				SetIntProperty("ColumnCount", DefaultColumnCount, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewTableLayoutSettingsRowsPerPage"),
#endif
 DefaultValue(DefaultRowPerPage), AutoFormatDisable, NotifyParentProperty(true)]
		public int RowsPerPage {
			get {
				int rowsPerPage = GetIntProperty("RowsPerPage", DefaultRowPerPage);
				if(Grid != null)
					rowsPerPage = Grid.CallbackState.Get<int>("RowsPerPage", DefaultRowPerPage);
				return rowsPerPage;
			}
			set {
				if(RowsPerPage == value) return;
				SetIntProperty("RowsPerPage", DefaultRowPerPage, value);
				if(Grid != null)
					Grid.CallbackState.Put("RowsPerPage", value);
				Changed();
			}
		}
		protected internal int ItemsPerPage { get { return ColumnCount * RowsPerPage; } }
		protected new ASPxCardView Grid { get { return (ASPxCardView)base.Grid; } }
		protected ASPxCardViewPagerSettings SettingsPager { get { return Grid.SettingsPager; } }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as CardViewTableLayoutSettings;
			if(src == null) return;
			ColumnCount = src.ColumnCount;
			RowsPerPage = src.RowsPerPage;
		}
	}
	public class ASPxCardViewBehaviorSettings : ASPxGridBehaviorSettings {
		public ASPxCardViewBehaviorSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsAllowSort"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowSort { get { return base.AllowSort; } set { base.AllowSort = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsSelectionStoringMode"),
#endif
 DefaultValue(GridViewSelectionStoringMode.DataIntegrityOptimized), NotifyParentProperty(true)]
		public new GridViewSelectionStoringMode SelectionStoringMode { get { return base.SelectionStoringMode; } set { base.SelectionStoringMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsConfirmDelete"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ConfirmDelete { get { return base.ConfirmDelete; } set { base.ConfirmDelete = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsEncodeErrorHtml"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool EncodeErrorHtml { get { return base.EncodeErrorHtml; } set { base.EncodeErrorHtml = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsProcessSelectionChangedOnServer"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ProcessSelectionChangedOnServer { get { return base.ProcessSelectionChangedOnServer; } set { base.ProcessSelectionChangedOnServer = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsHeaderFilterMaxRowCount"),
#endif
 DefaultValue(-1), NotifyParentProperty(true)]
		public new int HeaderFilterMaxRowCount { get { return base.HeaderFilterMaxRowCount; } set { base.HeaderFilterMaxRowCount = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsSortMode"),
#endif
 DefaultValue(ColumnSortMode.Default), NotifyParentProperty(true)]
		public new ColumnSortMode SortMode { get { return base.SortMode; } set { base.SortMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsAllowClientEventsOnLoad"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowClientEventsOnLoad { get { return base.AllowClientEventsOnLoad; } set { base.AllowClientEventsOnLoad = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsEnableCustomizationWindow"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool EnableCustomizationWindow { get { return base.EnableCustomizationWindow; } set { base.EnableCustomizationWindow = value; } }
		[DefaultValue(false), NotifyParentProperty(true)]
		public new bool AllowEllipsisInText { get { return base.AllowEllipsisInText; } set { base.AllowEllipsisInText = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsAllowFocusedCard"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowFocusedCard { get { return base.AllowFocusedItem; } set { base.AllowFocusedItem = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsAllowSelectByCardClick"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowSelectByCardClick { get { return base.AllowSelectByItemClick; } set { base.AllowSelectByItemClick = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsAllowSelectSingleCardOnly"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowSelectSingleCardOnly { get { return base.AllowSelectSingleItemOnly; } set { base.AllowSelectSingleItemOnly = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsProcessFocusedCardChangedOnServer"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ProcessFocusedCardChangedOnServer { get { return base.ProcessFocusedItemChangedOnServer; } set { base.ProcessFocusedItemChangedOnServer = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBehaviorSettingsEnableCardHotTrack"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool EnableCardHotTrack { get { return base.EnableItemHotTrack; } set { base.EnableItemHotTrack = value; } }		
	}
	public class ASPxCardViewSettings : ASPxGridSettings {
		public ASPxCardViewSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsEnableFilterControlPopupMenuScrolling"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool EnableFilterControlPopupMenuScrolling { get { return base.EnableFilterControlPopupMenuScrolling; } set { base.EnableFilterControlPopupMenuScrolling = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsShowTitlePanel"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowTitlePanel { get { return base.ShowTitlePanel; } set { base.ShowTitlePanel = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsShowHeaderFilterButton"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowHeaderFilterButton { get { return base.ShowHeaderFilterButton; } set { base.ShowHeaderFilterButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsShowHeaderFilterBlankItems"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowHeaderFilterBlankItems { get { return base.ShowHeaderFilterBlankItems; } set { base.ShowHeaderFilterBlankItems = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsVerticalScrollableHeight"),
#endif
 DefaultValue(200), NotifyParentProperty(true)]
		public new int VerticalScrollableHeight { get { return base.VerticalScrollableHeight; } set { base.VerticalScrollableHeight = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsVerticalScrollBarStyle"),
#endif
 DefaultValue(GridViewVerticalScrollBarStyle.Standard), NotifyParentProperty(true)]
		public new GridViewVerticalScrollBarStyle VerticalScrollBarStyle { get { return base.VerticalScrollBarStyle; } set { base.VerticalScrollBarStyle = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsShowStatusBar"),
#endif
 DefaultValue(GridViewStatusBarMode.Auto), NotifyParentProperty(true)]
		public new GridViewStatusBarMode ShowStatusBar { get { return base.ShowStatusBar; } set { base.ShowStatusBar = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsShowFilterBar"),
#endif
 DefaultValue(GridViewStatusBarMode.Hidden), NotifyParentProperty(true)]
		public new GridViewStatusBarMode ShowFilterBar { get { return base.ShowFilterBar; } set { base.ShowFilterBar = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsVerticalScrollBarMode"),
#endif
 DefaultValue(ScrollBarMode.Hidden), NotifyParentProperty(true)]
		public new ScrollBarMode VerticalScrollBarMode { get { return base.VerticalScrollBarMode; } set { base.VerticalScrollBarMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsShowCardHeader"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowCardHeader {
			get { return GetBoolProperty("ShowCardHeader", false); }
			set {
				if(ShowCardHeader == value) return;
				SetBoolProperty("ShowCardHeader", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsShowCardFooter"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowCardFooter {
			get { return GetBoolProperty("ShowCardFooter", false); }
			set {
				if(ShowCardFooter == value) return;
				SetBoolProperty("ShowCardFooter", false, value);
				Changed();
			}
		}
		[ DefaultValue(Layout.Table), NotifyParentProperty(true), AutoFormatDisable]
		public Layout LayoutMode {
			get { return (Layout)GetEnumProperty("LayoutMode", Layout.Table); }
			set {
				if(LayoutMode == value) return;
				SetEnumProperty("LayoutMode", Layout.Table, value);
				Changed();
			}
		}
		[ DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowHeaderPanel {
			get { return GetBoolProperty("ShowHeaderPanel", false); }
			set {
				if(ShowHeaderPanel == value)
					return;
				SetBoolProperty("ShowHeaderPanel", false, value);
				Changed();
			}
		}
		[ DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowSummaryPanel {
			get { return GetBoolProperty("ShowSummaryPanel", false); }
			set {
				if(ShowSummaryPanel == value) return;
				SetBoolProperty("ShowSummaryPanel", false, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as ASPxCardViewSettings;
			if(src == null) return;			
			ShowCardHeader = src.ShowCardHeader;
			ShowCardFooter = src.ShowCardFooter;
			ShowHeaderPanel = src.ShowHeaderPanel;
			LayoutMode = src.LayoutMode;
			ShowSummaryPanel = src.ShowSummaryPanel;
		}
	}
	public class ASPxCardViewEditingSettings : ASPxGridEditingSettings {
		public ASPxCardViewEditingSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewEditingSettingsMode"),
#endif
 DefaultValue(CardViewEditingMode.EditForm), NotifyParentProperty(true)]
		public CardViewEditingMode Mode {
			get { return (CardViewEditingMode)GetIntProperty("Mode", (int)CardViewEditingMode.EditForm); }
			set {
				if(value == Mode) return;
				SetIntProperty("Mode", (int)CardViewEditingMode.EditForm, (int)value);
				Changed();
			}
		}
		[ DefaultValue(GridViewNewItemRowPosition.Top), NotifyParentProperty(true)]
		public GridViewNewItemRowPosition NewCardPosition { get { return NewItemPosition; } set { NewItemPosition = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewEditingSettingsBatchEditSettings"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewBatchEditSettings BatchEditSettings { get { return base.BatchEditSettings as CardViewBatchEditSettings; } }
		protected new ASPxCardView Grid { get { return (ASPxCardView)base.Grid; } }
		protected override GridBatchEditSettings CreateBatchEditSettings() {
			return new CardViewBatchEditSettings(Grid);
		}
		protected internal override bool DisplayEditingRow { get { return Mode == CardViewEditingMode.PopupEditForm; } }
		protected internal override bool IsPopupEditForm { get { return Mode == CardViewEditingMode.PopupEditForm; } }
		protected internal override bool IsBatchEdit { get { return Mode == CardViewEditingMode.Batch; } }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxCardViewEditingSettings;
				if(src != null) {
					Mode = src.Mode;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class CardViewBatchEditSettings : GridBatchEditSettings {
		public CardViewBatchEditSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewBatchEditSettingsEditMode"),
#endif
 DefaultValue(CardViewBatchEditMode.Cell), NotifyParentProperty(true)]
		public new CardViewBatchEditMode EditMode { 
			get { return base.EditMode == GridViewBatchEditMode.Row ? CardViewBatchEditMode.Card : CardViewBatchEditMode.Cell; } 
			set { base.EditMode = value == CardViewBatchEditMode.Card ? GridViewBatchEditMode.Row : GridViewBatchEditMode.Cell; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewBatchEditSettingsStartEditAction"),
#endif
 DefaultValue(GridViewBatchStartEditAction.Click), NotifyParentProperty(true)]
		public new GridViewBatchStartEditAction StartEditAction { get { return base.StartEditAction; } set { base.StartEditAction = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewBatchEditSettingsShowConfirmOnLosingChanges"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowConfirmOnLosingChanges { get { return base.ShowConfirmOnLosingChanges; } set { base.ShowConfirmOnLosingChanges = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewBatchEditSettingsAllowValidationOnEndEdit"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowValidationOnEndEdit { get { return base.AllowValidationOnEndEdit; } set { base.AllowValidationOnEndEdit = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewBatchEditSettingsAllowEndEditOnValidationError"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowEndEditOnValidationError { get { return base.AllowEndEditOnValidationError; } set { base.AllowEndEditOnValidationError = value; } }
	}
	public class ASPxCardViewLoadingPanelSettings : ASPxGridLoadingPanelSettings {
		public ASPxCardViewLoadingPanelSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewLoadingPanelSettingsMode"),
#endif
 DefaultValue(GridViewLoadingPanelMode.Default), AutoFormatEnable, NotifyParentProperty(true)]
		public new GridViewLoadingPanelMode Mode { get { return base.Mode; } set { base.Mode = value; } }
	}
	public class ASPxCardViewCookiesSettings : ASPxGridCookiesSettings {
		public ASPxCardViewCookiesSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCookiesSettingsStoreSorting"),
#endif
 DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreSorting { get { return StoreGroupingAndSortingInternal; } set { StoreGroupingAndSortingInternal = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCookiesSettingsEnabled"),
#endif
 DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCookiesSettingsCookiesID"),
#endif
 DefaultValue(""), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public new string CookiesID { get { return base.CookiesID; } set { base.CookiesID = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCookiesSettingsVersion"),
#endif
 DefaultValue(""), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public new string Version { get { return base.Version; } set { base.Version = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCookiesSettingsStorePaging"),
#endif
 DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public new bool StorePaging { get { return base.StorePaging; } set { base.StorePaging = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCookiesSettingsStoreFiltering"),
#endif
 DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public new bool StoreFiltering { get { return base.StoreFiltering; } set { base.StoreFiltering = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCookiesSettingsStoreSearchPanelFiltering"),
#endif
 DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public new bool StoreSearchPanelFiltering { get { return base.StoreSearchPanelFiltering; } set { base.StoreSearchPanelFiltering = value; } }
	}
	public class ASPxCardViewCommandButtonSettings : ASPxGridCommandButtonSettings {
		CardViewCommandButtonSettings customizeButton;
		public ASPxCardViewCommandButtonSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonSettingsNewButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewCommandButtonSettings NewButton { get { return base.NewButton as CardViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonSettingsUpdateButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewCommandButtonSettings UpdateButton { get { return base.UpdateButton as CardViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonSettingsCancelButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewCommandButtonSettings CancelButton { get { return base.CancelButton as CardViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonSettingsEditButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewCommandButtonSettings EditButton { get { return base.EditButton as CardViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonSettingsDeleteButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewCommandButtonSettings DeleteButton { get { return base.DeleteButton as CardViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonSettingsSelectButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewCommandButtonSettings SelectButton { get { return base.SelectButton as CardViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonSettingsSearchPanelApplyButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewCommandButtonSettings SearchPanelApplyButton { get { return base.SearchPanelApplyButton as CardViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonSettingsSearchPanelClearButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewCommandButtonSettings SearchPanelClearButton { get { return base.SearchPanelClearButton as CardViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonSettingsEncodeHtml"),
#endif
 DefaultValue(false), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonSettingsEndlessPagingShowMoreCardsButton"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewCommandButtonSettings EndlessPagingShowMoreCardsButton {
			get {
				if(customizeButton == null)
					customizeButton = CreateButtonSettings() as CardViewCommandButtonSettings;				
				return customizeButton;
			}
		}
		protected new ASPxCardView Grid { get { return (ASPxCardView)base.Grid; } }
		protected override GridCommandButtonSettings CreateButtonSettings() {
			return new CardViewCommandButtonSettings(Grid);
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as ASPxCardViewCommandButtonSettings;
			if(src != null)
				EndlessPagingShowMoreCardsButton.Assign(src.EndlessPagingShowMoreCardsButton);
		}
	}
	public class ASPxCardViewDataSecuritySettings : ASPxGridDataSecuritySettings {
		public ASPxCardViewDataSecuritySettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewDataSecuritySettingsAllowInsert"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowInsert { get { return base.AllowInsert; } set { base.AllowInsert = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewDataSecuritySettingsAllowEdit"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowEdit { get { return base.AllowEdit; } set { base.AllowEdit = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewDataSecuritySettingsAllowDelete"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowDelete { get { return base.AllowDelete; } set { base.AllowDelete = value; } }
	}
	public class CardViewCommandButtonSettings : GridCommandButtonSettings {
		public CardViewCommandButtonSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCommandButtonSettingsButtonType"),
#endif
 DefaultValue(GridCommandButtonRenderMode.Default), NotifyParentProperty(true)]
		public new GridCommandButtonRenderMode ButtonType { get { return base.ButtonType; } set { base.ButtonType = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCommandButtonSettingsText"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string Text { get { return base.Text; } set { base.Text = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCommandButtonSettingsImage"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ImageProperties Image { get { return base.Image; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCommandButtonSettingsStyles"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public new ButtonControlStyles Styles { get { return base.Styles; } }
	}
	public class ASPxCardViewPopupControlSettings : ASPxGridPopupControlSettings {
		public ASPxCardViewPopupControlSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPopupControlSettingsEditForm"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewEditFormPopupSettings EditForm { get { return base.EditForm as CardViewEditFormPopupSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPopupControlSettingsHeaderFilter"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewHeaderFilterPopupSettings HeaderFilter { get { return base.HeaderFilter as CardViewHeaderFilterPopupSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPopupControlSettingsHeaderFilter"),
#endif
 AutoFormatDisable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewCustomizationWindowPopupSettings CustomizationWindow { get { return base.CustomizationWindow as CardViewCustomizationWindowPopupSettings; } }
		protected new ASPxCardView Grid { get { return base.Grid as ASPxCardView; } }
		protected override GridEditFormPopupSettings CreateEditFormSettings() { return new CardViewEditFormPopupSettings(Grid); }
		protected override GridHeaderFilterPopupSettings CreateHeaderFilterPopupSettings() { return new CardViewHeaderFilterPopupSettings(Grid); }
		protected override GridCustomizationWindowPopupSettings CreateCustomizationPopupSettings() { return new CardViewCustomizationWindowPopupSettings(Grid); }
	}
	public class CardViewEditFormPopupSettings : GridEditFormPopupSettings {
		public CardViewEditFormPopupSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Width { get { return base.Width; } set { base.Width = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Height { get { return base.Height; } set { base.Height = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsMinWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit MinWidth { get { return base.MinWidth; } set { base.MinWidth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsMinHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit MinHeight { get { return base.MinHeight; } set { base.MinHeight = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsHorizontalAlign"),
#endif
 NotifyParentProperty(true), DefaultValue(PopupHorizontalAlign.RightSides)]
		public new PopupHorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsVerticalAlign"),
#endif
 NotifyParentProperty(true), DefaultValue(PopupVerticalAlign.Below)]
		public new PopupVerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsHorizontalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(0)]
		public new int HorizontalOffset { get { return base.HorizontalOffset; } set { base.HorizontalOffset = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsVerticalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(-1)]
		public new int VerticalOffset { get { return base.VerticalOffset; } set { base.VerticalOffset = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsShowHeader"),
#endif
 NotifyParentProperty(true), DefaultValue(true)]
		public new bool ShowHeader { get { return base.ShowHeader; } set { base.ShowHeader = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsAllowResize"),
#endif
 NotifyParentProperty(true), DefaultValue(false)]
		public new bool AllowResize { get { return base.AllowResize; } set { base.AllowResize = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsResizingMode"),
#endif
 NotifyParentProperty(true), DefaultValue(ResizingMode.Live)]
		public new ResizingMode ResizingMode { get { return base.ResizingMode; } set { base.ResizingMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsModal"),
#endif
 NotifyParentProperty(true), DefaultValue(false)]
		public new bool Modal { get { return base.Modal; } set { base.Modal = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewEditFormPopupSettingsCloseOnEscape"),
#endif
 NotifyParentProperty(true), DefaultValue(AutoBoolean.Auto)]
		public new AutoBoolean CloseOnEscape { get { return base.CloseOnEscape; } set { base.CloseOnEscape = value; } }
	}
	public class CardViewCustomizationWindowPopupSettings : GridCustomizationWindowPopupSettings {
		public CardViewCustomizationWindowPopupSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCustomizationWindowPopupSettingsWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Width { get { return base.Width; } set { base.Width = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCustomizationWindowPopupSettingsHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Height { get { return base.Height; } set { base.Height = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCustomizationWindowPopupSettingsHorizontalAlign"),
#endif
 NotifyParentProperty(true), DefaultValue(PopupHorizontalAlign.RightSides)]
		public new PopupHorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCustomizationWindowPopupSettingsVerticalAlign"),
#endif
 NotifyParentProperty(true), DefaultValue(PopupVerticalAlign.BottomSides)]
		public new PopupVerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCustomizationWindowPopupSettingsHorizontalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(0)]
		public new int HorizontalOffset { get { return base.HorizontalOffset; } set { base.HorizontalOffset = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCustomizationWindowPopupSettingsVerticalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(0)]
		public new int VerticalOffset { get { return base.VerticalOffset; } set { base.VerticalOffset = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewCustomizationWindowPopupSettingsCloseOnEscape"),
#endif
 NotifyParentProperty(true), DefaultValue(AutoBoolean.Auto)]
		public new AutoBoolean CloseOnEscape { get { return base.CloseOnEscape; } set { base.CloseOnEscape = value; } }
	}
	public class CardViewHeaderFilterPopupSettings : GridHeaderFilterPopupSettings {
		public CardViewHeaderFilterPopupSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewHeaderFilterPopupSettingsWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Width { get { return base.Width; } set { base.Width = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewHeaderFilterPopupSettingsHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit Height { get { return base.Height; } set { base.Height = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewHeaderFilterPopupSettingsMinWidth"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit MinWidth { get { return base.MinWidth; } set { base.MinWidth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewHeaderFilterPopupSettingsMinHeight"),
#endif
 NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public new Unit MinHeight { get { return base.MinHeight; } set { base.MinHeight = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewHeaderFilterPopupSettingsResizingMode"),
#endif
 NotifyParentProperty(true), DefaultValue(ResizingMode.Live)]
		public new ResizingMode ResizingMode { get { return base.ResizingMode; } set { base.ResizingMode = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewHeaderFilterPopupSettingsCloseOnEscape"),
#endif
 NotifyParentProperty(true), DefaultValue(AutoBoolean.Auto)]
		public new AutoBoolean CloseOnEscape { get { return base.CloseOnEscape; } set { base.CloseOnEscape = value; } }
	}
	public class ASPxCardViewSearchPanelSettings : ASPxGridSearchPanelSettings {
		public ASPxCardViewSearchPanelSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelSettingsVisible"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool Visible { get { return base.Visible; } set { base.Visible = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelSettingsHighlightResults"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool HighlightResults { get { return base.HighlightResults; } set { base.HighlightResults = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelSettingsColumnNames"),
#endif
 DefaultValue("*"), NotifyParentProperty(true)]
		public new string ColumnNames { get { return base.ColumnNames; } set { base.ColumnNames = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelSettingsDelay"),
#endif
 DefaultValue(ASPxGridViewSearchPanelSettings.DefaultInputDelay), NotifyParentProperty(true)]
		public new int Delay { get { return base.Delay; } set { base.Delay = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelSettingsShowApplyButton"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowApplyButton { get { return base.ShowApplyButton; } set { base.ShowApplyButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelSettingsShowClearButton"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowClearButton { get { return base.ShowClearButton; } set { base.ShowClearButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelSettingsCustomEditorID"),
#endif
 DefaultValue(""), NotifyParentProperty(true)]
		public new string CustomEditorID { get { return base.CustomEditorID; } set { base.CustomEditorID = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelSettingsAllowTextInputTimer"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool AllowTextInputTimer { get { return base.AllowTextInputTimer; } set { base.AllowTextInputTimer = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelSettingsGroupOperator"),
#endif
 DefaultValue(GridViewSearchPanelGroupOperator.And), NotifyParentProperty(true)]
		public new GridViewSearchPanelGroupOperator GroupOperator { get { return base.GroupOperator; } set { base.GroupOperator = value; } }
	}
	public class ASPxCardViewFilterControlSettings : ASPxGridFilterControlSettings {
		public ASPxCardViewFilterControlSettings(ASPxCardView grid)
			: base(grid) {
		}
		[ DefaultValue(FilterControlViewMode.Visual), NotifyParentProperty(true)]
		public new FilterControlViewMode ViewMode { get { return base.ViewMode; } set { base.ViewMode = value; } }
		[ DefaultValue(false), NotifyParentProperty(true)]
		public new bool AllowHierarchicalColumns { get { return base.AllowHierarchicalColumns; } set { base.AllowHierarchicalColumns = value; } }
		[ DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowAllDataSourceColumns { get { return base.ShowAllDataSourceColumns; } set { base.ShowAllDataSourceColumns = value; } }
		[ DefaultValue(ASPxGridFilterControlSettings.DefaultMaxHierarchyDepth), NotifyParentProperty(true)]
		public new int MaxHierarchyDepth { get { return base.MaxHierarchyDepth; } set { base.MaxHierarchyDepth = value; } }
		[ DefaultValue(false), NotifyParentProperty(true)]
		public new bool ShowOperandTypeButton { get { return base.ShowOperandTypeButton; } set { base.ShowOperandTypeButton = value; } }
		[
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new FilterControlGroupOperationsVisibility GroupOperationsVisibility { get { return base.GroupOperationsVisibility; } }
	}
	public class ASPxCardViewTextSettings : ASPxGridTextSettings {
		public ASPxCardViewTextSettings(ASPxCardView grid)
			: base(grid) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsTitle"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string Title { get { return base.Title; } set { base.Title = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsConfirmDelete"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string ConfirmDelete { get { return base.ConfirmDelete; } set { base.ConfirmDelete = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsCustomizationWindowCaption"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CustomizationWindowCaption { get { return base.CustomizationWindowCaption; } set { base.CustomizationWindowCaption = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsPopupEditFormCaption"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string PopupEditFormCaption { get { return base.PopupEditFormCaption; } set { base.PopupEditFormCaption = value; } }
		[ DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string EmptyCard { get { return base.EmptyDataRow; } set { base.EmptyDataRow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsCommandEdit"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandEdit { get { return base.CommandEdit; } set { base.CommandEdit = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsCommandNew"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandNew { get { return base.CommandNew; } set { base.CommandNew = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsCommandDelete"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandDelete { get { return base.CommandDelete; } set { base.CommandDelete = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsCommandSelect"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandSelect { get { return base.CommandSelect; } set { base.CommandSelect = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsCommandCancel"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandCancel { get { return base.CommandCancel; } set { base.CommandCancel = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsCommandUpdate"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandUpdate { get { return base.CommandUpdate; } set { base.CommandUpdate = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsCommandApplySearchPanelFilter"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandApplySearchPanelFilter { get { return base.CommandApplySearchPanelFilter; } set { base.CommandApplySearchPanelFilter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsCommandClearSearchPanelFilter"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string CommandClearSearchPanelFilter { get { return base.CommandClearSearchPanelFilter; } set { base.CommandClearSearchPanelFilter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsSearchPanelEditorNullText"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string SearchPanelEditorNullText { get { return base.SearchPanelEditorNullText; } set { base.SearchPanelEditorNullText = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterShowAll"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterShowAll { get { return base.HeaderFilterShowAll; } set { base.HeaderFilterShowAll = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterShowBlanks"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterShowBlanks { get { return base.HeaderFilterShowBlanks; } set { base.HeaderFilterShowBlanks = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterShowNonBlanks"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterShowNonBlanks { get { return base.HeaderFilterShowNonBlanks; } set { base.HeaderFilterShowNonBlanks = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterSelectAll"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterSelectAll { get { return base.HeaderFilterSelectAll; } set { base.HeaderFilterSelectAll = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterYesterday"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterYesterday { get { return base.HeaderFilterYesterday; } set { base.HeaderFilterYesterday = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterToday"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterToday { get { return base.HeaderFilterToday; } set { base.HeaderFilterToday = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterTomorrow"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterTomorrow { get { return base.HeaderFilterTomorrow; } set { base.HeaderFilterTomorrow = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterLastWeek"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterLastWeek { get { return base.HeaderFilterLastWeek; } set { base.HeaderFilterLastWeek = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterThisWeek"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterThisWeek { get { return base.HeaderFilterThisWeek; } set { base.HeaderFilterThisWeek = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterNextWeek"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterNextWeek { get { return base.HeaderFilterNextWeek; } set { base.HeaderFilterNextWeek = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterLastMonth"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterLastMonth { get { return base.HeaderFilterLastMonth; } set { base.HeaderFilterLastMonth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterThisMonth"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterThisMonth { get { return base.HeaderFilterThisMonth; } set { base.HeaderFilterThisMonth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterNextMonth"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterNextMonth { get { return base.HeaderFilterNextMonth; } set { base.HeaderFilterNextMonth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterLastYear"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterLastYear { get { return base.HeaderFilterLastYear; } set { base.HeaderFilterLastYear = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterThisYear"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterThisYear { get { return base.HeaderFilterThisYear; } set { base.HeaderFilterThisYear = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterNextYear"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterNextYear { get { return base.HeaderFilterNextYear; } set { base.HeaderFilterNextYear = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsFilterControlPopupCaption"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string FilterControlPopupCaption { get { return base.FilterControlPopupCaption; } set { base.FilterControlPopupCaption = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsFilterBarClear"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string FilterBarClear { get { return base.FilterBarClear; } set { base.FilterBarClear = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsFilterBarCreateFilter"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string FilterBarCreateFilter { get { return base.FilterBarCreateFilter; } set { base.FilterBarCreateFilter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterOkButton"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterOkButton { get { return base.HeaderFilterOkButton; } set { base.HeaderFilterOkButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsHeaderFilterCancelButton"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public new string HeaderFilterCancelButton { get { return base.HeaderFilterCancelButton; } set { base.HeaderFilterCancelButton = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTextSettingsCommandEndlessPagingShowMoreCards"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string CommandEndlessPagingShowMoreCards {
			get { return GetStringProperty("CommandEndlessPagingShowMoreCards", string.Empty); }
			set {
				if(value == CommandEndlessPagingShowMoreCards) return;
				SetStringProperty("CommandEndlessPagingShowMoreCards", string.Empty, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as ASPxCardViewTextSettings;
				if(src != null) {
					CommandEndlessPagingShowMoreCards = src.CommandEndlessPagingShowMoreCards;
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal string GetCommandEndlessPagingShowMoreCardsButton() {
			if(!string.IsNullOrEmpty(CommandEndlessPagingShowMoreCards)) return CommandEndlessPagingShowMoreCards;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CardView_EndlessPagingShowMoreCardsButton);
		}
		protected internal override string GetCustomizationWindowCaption() {
			if(!string.IsNullOrEmpty(CustomizationWindowCaption)) return CustomizationWindowCaption;
			return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CardView_CustomizationWindowCaption);
		}
		protected internal override string GetCommandButtonText(GridCommandButtonType buttonType, bool isBatchEditMode) {
			if(buttonType == GridCommandButtonType.EndlessShowMoreCards)
				return GetCommandEndlessPagingShowMoreCardsButton();
			return base.GetCommandButtonText(buttonType, isBatchEditMode);
		}
	}
}
namespace DevExpress.Web.Internal {
	public class CardViewPageSizeItemSettings : PageSizeItemSettings {
		static readonly string[] DefaultRowsPerPageItems = new string[] { "3", "5", "10", "20" };
		public CardViewPageSizeItemSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal new ASPxCardViewPagerSettings PagerSettings { get { return Owner as ASPxCardViewPagerSettings; } }
		protected virtual bool IsFlowLayout { get { return PagerSettings.Grid != null && PagerSettings.Grid.RenderHelper.IsFlowLayout; } }
		protected override string[] GetDefaultPageSizeItems() {
			return IsFlowLayout ? base.GetDefaultPageSizeItems() : DefaultRowsPerPageItems;
		}
		protected internal override string GetDefaultCaption() {
			return IsFlowLayout ? base.GetDefaultCaption() : ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.CardView_PagerRowPerPage);
		}
	}
}

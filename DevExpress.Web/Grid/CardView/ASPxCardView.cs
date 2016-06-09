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

using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data.IO;
using DevExpress.Data.Summary;
using DevExpress.Utils;
using DevExpress.Web.Cookies;
using DevExpress.Web.Data;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.Web.FilterControl;
using DevExpress.Web.Internal.InternalCheckBox;
using DevExpress.Web.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxCardView"),
	Designer("DevExpress.Web.Design.CardViewDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxCardView.bmp")
	]
	public class ASPxCardView : ASPxGridBase {
		public const string 
			CardViewScriptResourceName = GridScriptResourcePath + "CardView.js",
			CardViewResourceImagePath = "DevExpress.Web.Images.CardView.",
			CardViewCssResourcePath = "DevExpress.Web.Css.CardView.",
			CardViewDefaultCssResourceName = CardViewCssResourcePath + "default.css",
			CardViewSpriteCssResourceName = CardViewCssResourcePath + "sprite.css",
			SkinControlName = "CardView";
		public const int InvalidCardIndex = DataController.InvalidRow;
		static readonly object
			customColumnDisplayText = new object(),
			customUnboundColumnData = new object(),
			beforeHeaderFilterFillItems = new object(),
			headerFilterFillItems = new object(),
			commandButtonInitialize = new object(),
			customButtonInitialize = new object(),
			cellEditorInitialize = new object(),
			searchPanelEditorCreate = new object(),
			searchPanelEditorInitialize = new object(),
			beforeColumnSortingGrouping = new object(),
			customCallback = new object(),
			customDataCallback = new object(),
			customButtonCallback = new object(),
			afterPerformCallback = new object(),
			customColumnSort = new object(),
			customErrorText = new object(),
			substituteFilter = new object(),
			substituteSortInfo = new object(),
			pageIndexChanged = new object(),
			pageSizeChanged = new object(),
			focusedRowChanged = new object(),
			selectionChanged = new object(),
			startRowEditing = new object(),
			cancelRowEditing = new object(),
			customFilterExpressionDisplayText = new object(),
			rowInserting = new object(),
			rowInserted = new object(),
			rowUpdating = new object(),
			rowUpdated = new object(),
			rowDeleting = new object(),
			rowDeleted = new object(),
			parseValue = new object(),
			initNewRow = new object(),
			rowValidating = new object(),
			summaryDisplayText = new object(),
			customSummaryCalculate = new object(),
			batchUpdate = new object();
		public ASPxCardView()
			: base() {
			Templates = new CardViewTemplates(this);
			CardLayoutProperties = CreateCardLayoutProperties();
			TotalSummary.SummaryChanged += new CollectionChangeEventHandler(OnSummaryChanged);
		}
		protected override string GetSkinControlName() { return SkinControlName; }
		protected override string DefaultCssResourceName { get { return CardViewDefaultCssResourceName; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewClientInstanceName"),
#endif
 Category("Client-Side"), AutoFormatDisable, DefaultValue(""), Localizable(false)]
		public string ClientInstanceName { get { return base.ClientInstanceNameInternal; } set { base.ClientInstanceNameInternal = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewClientVisible"),
#endif
 Category("Client-Side"), AutoFormatDisable, DefaultValue(true), Localizable(false)]
		public new bool ClientVisible { get { return base.ClientVisible; } set { base.ClientVisible = value; } }
		[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor)), 
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable, MergableProperty(false)]
		public new CardViewClientSideEvents ClientSideEvents { get { return base.ClientSideEvents as CardViewClientSideEvents; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewJSProperties"),
#endif
 Category("Client-Side"), Browsable(false), AutoFormatDisable, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties { get { return JSPropertiesInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewEnableCallBacks"),
#endif
 DefaultValue(true), AutoFormatDisable, Category("Behavior")]
		public new bool EnableCallBacks { get { return base.EnableCallBacks; } set { base.EnableCallBacks = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewEnableCallbackAnimation"),
#endif
 Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public new bool EnableCallbackAnimation { get { return base.EnableCallbackAnimation; } set { base.EnableCallbackAnimation = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewEnablePagingCallbackAnimation"),
#endif
 Category("Behavior"), DefaultValue(DefaultEnableSlideCallbackAnimation), AutoFormatDisable]
		public new bool EnablePagingCallbackAnimation { get { return base.EnablePagingCallbackAnimation; } set { base.EnablePagingCallbackAnimation = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewEnablePagingGestures"),
#endif
 Category("Behavior"), DefaultValue(AutoBoolean.Auto), AutoFormatDisable]
		public new AutoBoolean EnablePagingGestures { get { return base.EnablePagingGestures; } set { base.EnablePagingGestures = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewEnableCallbackCompression"),
#endif
 Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public new bool EnableCallbackCompression { get { return base.EnableCallbackCompression; } set { base.EnableCallbackCompression = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsPager"),
#endif
 Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewPagerSettings SettingsPager { get { return base.SettingsPager as ASPxCardViewPagerSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsEditing"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewEditingSettings SettingsEditing { get { return base.SettingsEditing as ASPxCardViewEditingSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettings"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewSettings Settings { get { return base.Settings as ASPxCardViewSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsBehavior"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewBehaviorSettings SettingsBehavior { get { return base.SettingsBehavior as ASPxCardViewBehaviorSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsCookies"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewCookiesSettings SettingsCookies { get { return base.SettingsCookies as ASPxCardViewCookiesSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsCommandButton"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewCommandButtonSettings SettingsCommandButton { get { return base.SettingsCommandButton as ASPxCardViewCommandButtonSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsDataSecurity"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewDataSecuritySettings SettingsDataSecurity { get { return base.SettingsDataSecurity as ASPxCardViewDataSecuritySettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsPopup"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewPopupControlSettings SettingsPopup { get { return base.SettingsPopup as ASPxCardViewPopupControlSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsSearchPanel"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewSearchPanelSettings SettingsSearchPanel { get { return base.SettingsSearchPanel as ASPxCardViewSearchPanelSettings; } }
		[ Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewFilterControlSettings SettingsFilterControl { get { return base.SettingsFilterControl as ASPxCardViewFilterControlSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsLoadingPanel"),
#endif
 Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewLoadingPanelSettings SettingsLoadingPanel { get { return (ASPxCardViewLoadingPanelSettings)base.SettingsLoadingPanel; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSettingsText"),
#endif
 Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxCardViewTextSettings SettingsText { get { return base.SettingsText as ASPxCardViewTextSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewStylesPopup"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewPopupControlStyles StylesPopup { get { return base.StylesPopup as CardViewPopupControlStyles; } }
		[Browsable(false), AutoFormatEnable, Category("Templates"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CardViewTemplates Templates { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewDataSourceForceStandardPaging"),
#endif
 DefaultValue(false), AutoFormatDisable]
		public new bool DataSourceForceStandardPaging { get { return base.DataSourceForceStandardPaging; } set { base.DataSourceForceStandardPaging = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewColumns"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue(null), AutoFormatDisable, TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public new CardViewColumnCollection Columns { get { return base.Columns as CardViewColumnCollection; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewKeyFieldName"),
#endif
 DefaultValue(""), Category("Data"), Localizable(false), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), AutoFormatDisable]
		public new string KeyFieldName { get { return base.KeyFieldName; } set { base.KeyFieldName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewAutoGenerateColumns"),
#endif
 DefaultValue(true), Category("Data"), AutoFormatDisable]
		public new bool AutoGenerateColumns { get { return base.AutoGenerateColumns; } set { base.AutoGenerateColumns = value; } }
		[ DefaultValue(true), AutoFormatDisable, Category("Behavior")]
		public bool EnableCardsCache { get { return base.EnableRowsCache; } set { base.EnableRowsCache = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewRightToLeft"),
#endif
 Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public new DefaultBoolean RightToLeft { get { return base.RightToLeft; } set { base.RightToLeft = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewEditFormLayoutProperties"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Settings"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue(null), AutoFormatDisable]
		public new CardViewFormLayoutProperties EditFormLayoutProperties { get { return (CardViewFormLayoutProperties)base.EditFormLayoutProperties; } }
		[ PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Settings"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue(null), AutoFormatDisable]
		public CardViewFormLayoutProperties CardLayoutProperties { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewStyles"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewStyles Styles { get { return base.Styles as CardViewStyles; } }
		[Browsable(false)]
		public new CardViewSelection Selection { get { return base.Selection as CardViewSelection; } }
		[Browsable(false)]
		public int VisibleCardCount { get { return base.VisibleRowCount; } }
		[Browsable(false)]
		public new int VisibleStartIndex { get { return base.VisibleStartIndex; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FocusedCardIndex { get { return base.FocusedRowIndex; } set { base.FocusedRowIndex = value; } }
		public new object GetRow(int visibleIndex) { return base.GetRow(visibleIndex); }
		public new DataRow GetDataRow(int visibleIndex) { return base.GetDataRow(visibleIndex); }
		public new List<object> GetSelectedFieldValues(params string[] fieldNames) { return base.GetSelectedFieldValues(fieldNames); }
		public new List<object> GetFilteredSelectedValues(params string[] fieldNames) { return base.GetFilteredSelectedValues(fieldNames); }
		public object GetCardValues(int visibleIndex, params string[] fieldNames) { return base.GetRowValues(visibleIndex, fieldNames); }
		public object GetCardValuesByKeyValue(object keyValue, params string[] fieldNames) { return base.GetRowValuesByKeyValue(keyValue, fieldNames); }
		public new int FindVisibleIndexByKeyValue(object keyValue) { return base.FindVisibleIndexByKeyValue(keyValue); }
		public List<object> GetCurrentPageCardValues(params string[] fieldNames) { return base.GetCurrentPageRowValues(fieldNames); }
		public bool MakeCardVisible(object keyValue) { return base.MakeRowVisible(keyValue); }
		[Browsable(false), DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int PageIndex { get { return base.PageIndex; } set { base.PageIndex = value; } }
		[Browsable(false), AutoFormatDisable]
		public new int PageCount { get { return base.PageCount; } }
		[Browsable(false), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string FilterExpression { get { return base.FilterExpression; } set { base.FilterExpression = value; } }
		[Browsable(false), DefaultValue(true), AutoFormatDisable]
		public new bool FilterEnabled { get { return base.FilterEnabled; } set { base.FilterEnabled = value; } }
		[Browsable(false), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SearchPanelFilter { get { return base.SearchPanelFilter; } set { base.SearchPanelFilter = value; } }
		[Browsable(false)]
		public new int SortCount { get { return base.SortCount; } }
		public void DoCardValidation() {
			base.DoRowValidation();
		}
		public new void StartEdit(int visibleIndex) {
			base.StartEdit(visibleIndex);
		}
		public new void UpdateEdit() {
			base.UpdateEdit();
		}
		public new void CancelEdit() {
			base.CancelEdit();
		}
		public void AddNewCard() {
			base.AddNewRow();
		}
		public void DeleteCard(int visibleIndex) {
			base.DeleteRow(visibleIndex);
		}
		public new void ShowFilterControl() { base.ShowFilterControl(); }
		public new void HideFilterControl() { base.HideFilterControl(); }
		[Browsable(false)]
		public new bool IsFilterControlVisible { get { return base.IsFilterControlVisible; } }
		[AutoFormatDisable, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new int ScrollToVisibleIndexOnClient { get { return base.ScrollToVisibleIndexOnClient; } set { base.ScrollToVisibleIndexOnClient = value; } }
		[Browsable(false)]
		public new bool IsEditing { get { return base.IsEditing; } }
		[Browsable(false)]
		public bool IsNewCardEditing { get { return base.IsNewRowEditing; } }
		[Browsable(false)]
		public int EditingCardVisibleIndex { get { return base.EditingRowVisibleIndex; } }
		public new void BeginUpdate() { base.BeginUpdate(); }
		public new void EndUpdate() { base.EndUpdate(); }
		[Browsable(false)]
		public new bool IsLockUpdate { get { return base.IsLockUpdate; } }
		public new void ClearSort() { base.ClearSort(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewImagesEditors"),
#endif
 Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new EditorImages ImagesEditors { get { return base.ImagesEditors; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewImagesFilterControl"),
#endif
 Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new FilterControlImages ImagesFilterControl { get { return base.ImagesFilterControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewStylesPager"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new PagerStyles StylesPager { get { return base.StylesPager; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewStylesEditors"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new EditorStyles StylesEditors { get { return base.StylesEditors; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewStylesFilterControl"),
#endif
 Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new FilterControlStyles StylesFilterControl { get { return base.StylesFilterControl; } }
		public new string SaveClientLayout() { return base.SaveClientState(); }
		public new void LoadClientLayout(string layoutData) { base.LoadClientLayout(layoutData); }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewImages"),
#endif
 Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewImages Images { get { return (CardViewImages)base.Images; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewTotalSummary"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable, TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public new ASPxCardViewSummaryItemCollection TotalSummary { get { return base.TotalSummary as ASPxCardViewSummaryItemCollection; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewFormatConditions"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null), AutoFormatDisable, TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
		Editor("DevExpress.Web.Design.CardViewFormatConditionCommonEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new CardViewFormatConditionCollection FormatConditions { get { return (CardViewFormatConditionCollection)base.FormatConditions; } }
		public object GetTotalSummaryValue(ASPxCardViewSummaryItem item) { return base.GetTotalSummaryValue(item); }
		protected internal new CardViewRenderHelper RenderHelper { get { return base.RenderHelper as CardViewRenderHelper; } }
		protected internal new CardViewBatchEditHelper BatchEditHelper { get { return base.BatchEditHelper as CardViewBatchEditHelper; } }
		protected internal new CardViewColumnHelper ColumnHelper { get { return base.ColumnHelper as CardViewColumnHelper; } }
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] { CardLayoutProperties }); 
		}
		protected override Dictionary<string, object> GetEditTemplateValuesCore() {
			if(!IsEditing) return null;
			Dictionary<object, ITemplate> columnEditItems = GetColumnEditItems();
			Dictionary<string, object> itemValues = RenderHelper.EditItemTemplates.FindTwoWayBindings(columnEditItems, new TemplateContainerFinder(FindEditItemTemplate));
			Dictionary<object, ITemplate> formLayoutEditItems = GetFormLayoutEditItems();
			Dictionary<string, object> formLayoutItemValues = RenderHelper.EditItemTemplates.FindTwoWayBindings(formLayoutEditItems, new TemplateContainerFinder(FindEditItemTemplate));
			Dictionary<string, object> formValues = RenderHelper.EditFormTemplates.FindTwoWayBindings(Templates.EditForm);
			return MergeDictionaries(itemValues, formLayoutItemValues, formValues);
		}
		Dictionary<object, ITemplate> GetColumnEditItems() {
			Dictionary<object, ITemplate> editItems = new Dictionary<object, ITemplate>();
			foreach(CardViewColumn column in Columns) {
				if(column.EditItemTemplate == null) continue;
				editItems[column] = column.EditItemTemplate;
			}
			return editItems;
		}
		Control FindEditItemTemplate(Control container, object parameters, string id) {
			CardViewEditItemTemplateContainer item = (CardViewEditItemTemplateContainer)container;
			if(parameters != null && item.Column != parameters) return null;
			if(id == null) return item;
			return item.FindControl(id);
		}
		protected virtual CardViewFormLayoutProperties CreateCardLayoutProperties() { return new CardViewFormLayoutProperties(this) { IsStandalone = false, DataOwner = this }; }
		protected override GridFormLayoutProperties CreateEditFormLayoutProperties() { return new CardViewFormLayoutProperties(this); }
		protected override ClientSideEventsBase CreateClientSideEvents() { return new CardViewClientSideEvents(); }
		protected override GridEndlessPagingHelper CreateEndlessPagingHelper() { return new GridEndlessPagingHelper(this); }
		protected override GridBatchEditHelper CreateBatchEditHelper() { return new CardViewBatchEditHelper(this); }
		protected override GridRenderHelper CreateRenderHelper() { return new CardViewRenderHelper(this); }
		protected override GridColumnHelper CreateColumnHelper() { return new CardViewColumnHelper(this); }
		protected override GridFilterHelper CreateFilterHelper() { return new GridFilterHelper(this); }
		protected override GridClientStylesInfo CreateClientStylesInfo() { return new CardViewClientStylesInfo(this); }
		protected override WebDataSelection CreateSelection(WebDataProxy proxy) { return new CardViewSelection(proxy); }
		protected override ASPxGridTextSettings CreateSettingsText() { return new ASPxCardViewTextSettings(this); }
		protected override GridPopupControlStyles CreatePopupControlStyles() { return new CardViewPopupControlStyles(this); }
		protected override EditorImages CreateEditorImages() { return new EditorImages(this); }
		protected override StylesBase CreateStyles() { return new CardViewStyles(this); }
		protected override EditorStyles CreateEditorStyles() { return new EditorStyles(this); }
		protected override PagerStyles CreatePagerStyles() { return new PagerStyles(this); }
		protected override ASPxGridPagerSettings CreateSettingsPager() { return new ASPxCardViewPagerSettings(this); }
		protected override ASPxGridEditingSettings CreateSettingsEditing() { return new ASPxCardViewEditingSettings(this); }
		protected override ASPxGridSettings CreateGridSettings() { return new ASPxCardViewSettings(this); }
		protected override ASPxGridBehaviorSettings CreateBehaviorSettings() { return new ASPxCardViewBehaviorSettings(this); }
		protected override ASPxGridCookiesSettings CreateCookiesSettings() { return new ASPxCardViewCookiesSettings(this); }
		protected override ASPxGridCommandButtonSettings CreateCommandButtonSettings() { return new ASPxCardViewCommandButtonSettings(this); }
		protected override ASPxGridDataSecuritySettings CreateDataSecuritySettings() { return new ASPxCardViewDataSecuritySettings(this); }
		protected override ASPxGridPopupControlSettings CreatePopupSettings() { return new ASPxCardViewPopupControlSettings(this); }
		protected override ASPxGridSearchPanelSettings CreateSearchPanelSettings() { return new ASPxCardViewSearchPanelSettings(this); }
		protected override ASPxGridFilterControlSettings CreateFilterControlSettings() { return new ASPxCardViewFilterControlSettings(this); }
		protected override IList CreateSummaryItemCollection() { return new ASPxCardViewSummaryItemCollection(this); }
		protected override ASPxSummaryItemBase CreateTotalSummaryItem() { return new ASPxCardViewSummaryItem(); }
		protected override GridFormatConditionCollection CreateFormatConditions() { return new CardViewFormatConditionCollection(this, OnFormatConditionSummaryChanged); }
		protected override SettingsLoadingPanel CreateSettingsLoadingPanel() { return new ASPxCardViewLoadingPanelSettings(this); }
		protected override GridCookiesBase CreateControlCookies() { return new CardViewCookies(this); }
		protected override GridUpdatableContainer CreateContainerControl() { return new CardViewUpdatableContainer(this); }
		protected internal override ASPxGridPager CreatePagerControl(string id) { return new ASPxCardViewPager(this) { ID = id }; }
		protected override GridSortData CreateSortData() { return new GridSortData(this); }
		protected override GridColumnCollection CreateColumnCollection() { return new CardViewColumnCollection(this); }
		protected override ImagesBase CreateImages() { return new CardViewImages(this); }
		protected override IWebGridDataColumn CreateEditColumn(Type dataType) { return CardViewEditColumn.CreateColumn(dataType); }
		protected internal override IWebGridColumn FindColumnByName(string columnName) {			
			return Columns[columnName];
		}
		protected internal override FormLayoutProperties GenerateDefaultLayout(bool fromControlDesigner) {
			var prop = new CardViewFormLayoutProperties();
			if(fromControlDesigner)
				prop.Items.AddCommandItem(new CardViewCommandLayoutItem() { HorizontalAlign = CommandLayoutItemDefaultHorizontalAlign });
			var columns = ColumnHelper.AllVisibleDataColumns;
			GridUniqueColumnInfo info = fromControlDesigner ? new GridUniqueColumnInfo(columns) : null;
			foreach(CardViewColumn column in columns) {
				CardViewColumnLayoutItem layoutItem;
				if(fromControlDesigner)
					layoutItem = new CardViewColumnLayoutItem() { ColumnName = info.GetUniqueColumnName(column) };
				else
					layoutItem = new CardViewColumnLayoutItem(column);
				prop.Items.Add(layoutItem);
			}
			if(fromControlDesigner)
				prop.Items.AddCommandItem(new EditModeCommandLayoutItem() { HorizontalAlign = CommandLayoutItemDefaultHorizontalAlign });
			return prop;
		}
		protected override void SetPageSizeInternal(int newPageSize) {
			if(RenderHelper.IsFlowLayout)
				SettingsPager.SettingsFlowLayout.ItemsPerPage = newPageSize;
			else
				SettingsPager.SettingsTableLayout.RowsPerPage = newPageSize;
		}
		#region Events
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomColumnDisplayText"),
#endif
 Category("Rendering")]
		public event ASPxCardViewColumnDisplayTextEventHandler CustomColumnDisplayText { add { Events.AddHandler(customColumnDisplayText, value); } remove { Events.RemoveHandler(customColumnDisplayText, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomUnboundColumnData"),
#endif
 Category("Data")]
		public event ASPxCardViewColumnDataEventHandler CustomUnboundColumnData { add { Events.AddHandler(customUnboundColumnData, value); } remove { Events.RemoveHandler(customUnboundColumnData, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBeforeHeaderFilterFillItems"),
#endif
 Category("Data")]
		public event ASPxCardViewBeforeHeaderFilterFillItemsEventHandler BeforeHeaderFilterFillItems { add { Events.AddHandler(beforeHeaderFilterFillItems, value); } remove { Events.RemoveHandler(beforeHeaderFilterFillItems, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewHeaderFilterFillItems"),
#endif
 Category("Data")]
		public event ASPxCardViewHeaderFilterEventHandler HeaderFilterFillItems { add { Events.AddHandler(headerFilterFillItems, value); } remove { Events.RemoveHandler(headerFilterFillItems, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCommandButtonInitialize"),
#endif
 Category("Rendering")]
		public event ASPxCardViewCommandButtonEventHandler CommandButtonInitialize { add { Events.AddHandler(commandButtonInitialize, value); } remove { Events.RemoveHandler(commandButtonInitialize, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomButtonInitialize"),
#endif
 Category("Rendering")]
		public event ASPxCardViewCustomButtonEventHandler CustomButtonInitialize { add { Events.AddHandler(customButtonInitialize, value); } remove { Events.RemoveHandler(customButtonInitialize, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCellEditorInitialize"),
#endif
 Category("Rendering")]
		public event ASPxCardViewEditorEventHandler CellEditorInitialize { add { Events.AddHandler(cellEditorInitialize, value); } remove { Events.RemoveHandler(cellEditorInitialize, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelEditorCreate"),
#endif
 Category("Rendering")]
		public event ASPxCardViewSearchPanelEditorCreateEventHandler SearchPanelEditorCreate { add { Events.AddHandler(searchPanelEditorCreate, value); } remove { Events.RemoveHandler(searchPanelEditorCreate, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSearchPanelEditorInitialize"),
#endif
 Category("Rendering")]
		public event ASPxCardViewSearchPanelEditorEventHandler SearchPanelEditorInitialize { add { Events.AddHandler(searchPanelEditorInitialize, value); } remove { Events.RemoveHandler(searchPanelEditorInitialize, value); } }
		[ Category("Data")]
		public event ASPxCardViewBeforeColumnSortingEventHandler BeforeColumnSorting { add { Events.AddHandler(beforeColumnSortingGrouping, value); } remove { Events.RemoveHandler(beforeColumnSortingGrouping, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomCallback"),
#endif
 Category("Action")]
		public event ASPxCardViewCustomCallbackEventHandler CustomCallback { add { Events.AddHandler(customCallback, value); } remove { Events.RemoveHandler(customCallback, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomDataCallback"),
#endif
 Category("Data")]
		public event ASPxCardViewCustomDataCallbackEventHandler CustomDataCallback { add { Events.AddHandler(customDataCallback, value); } remove { Events.RemoveHandler(customDataCallback, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomButtonCallback"),
#endif
 Category("Action")]
		public event ASPxCardViewCustomButtonCallbackEventHandler CustomButtonCallback { add { Events.AddHandler(customButtonCallback, value); } remove { Events.RemoveHandler(customButtonCallback, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewAfterPerformCallback"),
#endif
 Category("Action")]
		public event ASPxCardViewAfterPerformCallbackEventHandler AfterPerformCallback { add { Events.AddHandler(afterPerformCallback, value); } remove { Events.RemoveHandler(afterPerformCallback, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomColumnSort"),
#endif
 Category("Data")]
		public event ASPxCardViewCustomColumnSortEventHandler CustomColumnSort { add { Events.AddHandler(customColumnSort, value); } remove { Events.RemoveHandler(customColumnSort, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomJSProperties"),
#endif
 Category("Client-Side")]
		public event ASPxCardViewClientJSPropertiesEventHandler CustomJSProperties { add { Events.AddHandler(EventCustomJsProperties, value); } remove { Events.RemoveHandler(EventCustomJsProperties, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomErrorText"),
#endif
 Category("Rendering")]
		public event ASPxCardViewCustomErrorTextEventHandler CustomErrorText { add { Events.AddHandler(customErrorText, value); } remove { Events.RemoveHandler(customErrorText, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSubstituteFilter"),
#endif
 Category("Data")]
		public event EventHandler<SubstituteFilterEventArgs> SubstituteFilter { add { Events.AddHandler(substituteFilter, value); } remove { Events.RemoveHandler(substituteFilter, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSubstituteSortInfo"),
#endif
 Category("Data")]
		public event EventHandler<SubstituteSortInfoEventArgs> SubstituteSortInfo { add { Events.AddHandler(substituteSortInfo, value); } remove { Events.RemoveHandler(substituteSortInfo, value); } }
		[ Category("Action")]
		public event EventHandler FocusedCardChanged { add { Events.AddHandler(focusedRowChanged, value); } remove { Events.RemoveHandler(focusedRowChanged, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSelectionChanged"),
#endif
 Category("Action")]
		public event EventHandler SelectionChanged { add { Events.AddHandler(selectionChanged, value); } remove { Events.RemoveHandler(selectionChanged, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPageIndexChanged"),
#endif
 Category("Action")]
		public event EventHandler PageIndexChanged { add { Events.AddHandler(pageIndexChanged, value); } remove { Events.RemoveHandler(pageIndexChanged, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewPageSizeChanged"),
#endif
 Category("Action")]
		public event EventHandler PageSizeChanged { add { Events.AddHandler(pageSizeChanged, value); } remove { Events.RemoveHandler(pageSizeChanged, value); } }
		[ Category("Action")]
		public event ASPxStartCardEditingEventHandler StartCardEditing { add { Events.AddHandler(startRowEditing, value); } remove { Events.RemoveHandler(startRowEditing, value); } }
		[ Category("Action")]
		public event ASPxStartCardEditingEventHandler CancelCardEditing { add { Events.AddHandler(cancelRowEditing, value); } remove { Events.RemoveHandler(cancelRowEditing, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomFilterExpressionDisplayText"),
#endif
 Category("Rendering")]
		public event CustomFilterExpressionDisplayTextEventHandler CustomFilterExpressionDisplayText { add { Events.AddHandler(customFilterExpressionDisplayText, value); } remove { Events.RemoveHandler(customFilterExpressionDisplayText, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewFilterControlOperationVisibility"),
#endif
 Category("Rendering")]
		public new event FilterControlOperationVisibilityEventHandler FilterControlOperationVisibility { add { base.FilterControlOperationVisibility += value; } remove { FilterControlOperationVisibility -= value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewFilterControlParseValue"),
#endif
 Category("Rendering")]
		public new event FilterControlParseValueEventHandler FilterControlParseValue { add { base.FilterControlParseValue += value; } remove { base.FilterControlParseValue -= value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewFilterControlCustomValueDisplayText"),
#endif
 Category("Rendering")]
		public new event FilterControlCustomValueDisplayTextEventHandler FilterControlCustomValueDisplayText { add { base.FilterControlCustomValueDisplayText += value; } remove { base.FilterControlCustomValueDisplayText -= value; } }
		[ Category("Rendering")]
		public new event FilterControlColumnsCreatedEventHandler FilterControlColumnsCreated { add { base.FilterControlColumnsCreated += value; } remove { base.FilterControlColumnsCreated -= value; } }
		[ Category("Rendering")]
		public new event FilterControlCriteriaValueEditorInitializeEventHandler FilterControlCriteriaValueEditorInitialize { add { base.FilterControlCriteriaValueEditorInitialize += value; } remove { base.FilterControlCriteriaValueEditorInitialize -= value; } }
		[ Category("Rendering")]
		public new event FilterControlCriteriaValueEditorCreateEventHandler FilterControlCriteriaValueEditorCreate { add { base.FilterControlCriteriaValueEditorCreate += value; } remove { base.FilterControlCriteriaValueEditorCreate -= value; } }
		[ Category("Action")]
		public event ASPxDataInsertingEventHandler CardInserting { add { Events.AddHandler(rowInserting, value); } remove { Events.RemoveHandler(rowInserting, value); } }
		[ Category("Action")]
		public event ASPxDataInsertedEventHandler CardInserted { add { Events.AddHandler(rowInserted, value); } remove { Events.RemoveHandler(rowInserted, value); } }
		[ Category("Action")]
		public event ASPxDataUpdatingEventHandler CardUpdating { add { Events.AddHandler(rowUpdating, value); } remove { Events.RemoveHandler(rowUpdating, value); } }
		[ Category("Action")]
		public event ASPxDataUpdatedEventHandler CardUpdated { add { Events.AddHandler(rowUpdated, value); } remove { Events.RemoveHandler(rowUpdated, value); } }
		[ Category("Action")]
		public event ASPxDataDeletingEventHandler CardDeleting { add { Events.AddHandler(rowDeleting, value); } remove { Events.RemoveHandler(rowDeleting, value); } }
		[ Category("Action")]
		public event ASPxDataDeletedEventHandler CardDeleted { add { Events.AddHandler(rowDeleted, value); } remove { Events.RemoveHandler(rowDeleted, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewParseValue"),
#endif
 Category("Data")]
		public event ASPxParseValueEventHandler ParseValue { add { Events.AddHandler(parseValue, value); } remove { Events.RemoveHandler(parseValue, value); } }
		[ Category("Data")]
		public event ASPxDataInitNewRowEventHandler InitNewCard { add { Events.AddHandler(initNewRow, value); } remove { Events.RemoveHandler(initNewRow, value); } }
		[ Category("Action")]
		public event ASPxCardViewDataValidationEventHandler CardValidating { add { Events.AddHandler(rowValidating, value); } remove { Events.RemoveHandler(rowValidating, value); } }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxCardViewClientLayout")]
#endif
		public new event ASPxClientLayoutHandler ClientLayout { add { base.ClientLayout += value; } remove { base.ClientLayout -= value; } }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxCardViewBeforeGetCallbackResult")]
#endif
		public event EventHandler BeforeGetCallbackResult { add { Events.AddHandler(EventBeforeGetCallbackResult, value); } remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewSummaryDisplayText"),
#endif
 Category("Rendering")]
		public event ASPxCardViewSummaryDisplayTextEventHandler SummaryDisplayText { add { Events.AddHandler(summaryDisplayText, value); } remove { Events.RemoveHandler(summaryDisplayText, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewCustomSummaryCalculate"),
#endif
 Category("Data")]
		public event CustomSummaryEventHandler CustomSummaryCalculate { add { Events.AddHandler(customSummaryCalculate, value); } remove { Events.RemoveHandler(customSummaryCalculate, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewBatchUpdate"),
#endif
 Category("Action")]
		public event ASPxDataBatchUpdateEventHandler BatchUpdate { add { Events.AddHandler(batchUpdate, value); } remove { Events.RemoveHandler(batchUpdate, value); } }
		protected internal override void RaiseCustomColumnDisplayText(ASPxGridColumnDisplayTextEventArgs e) {
			var handler = (ASPxCardViewColumnDisplayTextEventHandler)Events[customColumnDisplayText];
			if(handler != null) handler(this, (ASPxCardViewColumnDisplayTextEventArgs)e);
		}
		protected override void RaiseCustomUnboundColumnData(ASPxGridColumnDataEventArgs e) {
			var handler = Events[customUnboundColumnData] as ASPxCardViewColumnDataEventHandler;
			if(handler != null) handler(this, e as ASPxCardViewColumnDataEventArgs);
		}
		protected internal override void RaiseBeforeHeaderFilterFillItems(ASPxGridBeforeHeaderFilterFillItemsEventArgs e) {
			var handler = (ASPxCardViewBeforeHeaderFilterFillItemsEventHandler)Events[beforeHeaderFilterFillItems];
			if(handler != null) handler(this, e as ASPxCardViewBeforeHeaderFilterFillItemsEventArgs);
		}
		protected internal override void RaiseHeaderFilterFillItems(ASPxGridHeaderFilterEventArgs e) {
			var handler = (ASPxCardViewHeaderFilterEventHandler)Events[headerFilterFillItems];
			if(handler != null) handler(this, e as ASPxCardViewHeaderFilterEventArgs);
		}
		protected internal override void RaiseCommandButtonInitialize(ASPxGridCommandButtonEventArgs e) {
			var handler = (ASPxCardViewCommandButtonEventHandler)Events[commandButtonInitialize];
			if(handler != null) handler(this, e as ASPxCardViewCommandButtonEventArgs);
		}
		protected internal override void RaiseCustomButtonInitialize(ASPxGridCustomCommandButtonEventArgs e) {
			var handler = (ASPxCardViewCustomButtonEventHandler)Events[customButtonInitialize];
			if(handler != null) handler(this, e as ASPxCardViewCustomCommandButtonEventArgs);
		}
		protected internal override void RaiseEditorInitialize(ASPxGridEditorEventArgs e) {
			var handler = (ASPxCardViewEditorEventHandler)Events[cellEditorInitialize];
			if(handler != null) handler(this, e as ASPxCardViewEditorEventArgs);
		}
		protected internal override void RaiseSearchPanelEditorCreate(ASPxGridEditorCreateEventArgs e) {
			var handler = (ASPxCardViewSearchPanelEditorCreateEventHandler)Events[searchPanelEditorCreate];
			if(handler != null) handler(this, e as ASPxCardViewSearchPanelEditorCreateEventArgs);
		}
		protected internal override void RaiseSearchPanelEditorInitialize(ASPxGridEditorEventArgs e) {
			var handler = (ASPxCardViewSearchPanelEditorEventHandler)Events[searchPanelEditorInitialize];
			if(handler != null) handler(this, e as ASPxCardViewSearchPanelEditorEventArgs);
		}
		protected internal override void RaiseBeforeColumnSortingGrouping(ASPxGridBeforeColumnGroupingSortingEventArgs e) {
			var handler = (ASPxCardViewBeforeColumnSortingEventHandler)Events[beforeColumnSortingGrouping];
			if(handler != null) handler(this, e as ASPxCardViewBeforeColumnSortingEventArgs);
		}
		protected override void RaiseCustomCallback(ASPxGridCustomCallbackEventArgs e) {
			var handler = (ASPxCardViewCustomCallbackEventHandler)Events[customCallback];
			if(handler != null) handler(this, e as ASPxCardViewCustomCallbackEventArgs);
		}
		protected override void RaiseCustomDataCallback(ASPxGridCustomCallbackEventArgs e) {
			var handler = (ASPxCardViewCustomDataCallbackEventHandler)Events[customDataCallback];
			if(handler != null) handler(this, e as ASPxCardViewCustomDataCallbackEventArgs);
		}
		protected override void RaiseCustomButtonCallback(ASPxGridCustomButtonCallbackEventArgs e) {
			var handler = (ASPxCardViewCustomButtonCallbackEventHandler)Events[customButtonCallback];
			if(handler != null) handler(this, e as ASPxCardViewCustomButtonCallbackEventArgs);
		}
		protected override void RaiseAfterPerformCallback(ASPxGridAfterPerformCallbackEventArgs e) {
			var handler = (ASPxCardViewAfterPerformCallbackEventHandler)Events[afterPerformCallback];
			if(handler != null) handler(this, e as ASPxCardViewAfterPerformCallbackEventArgs);
		}
		protected internal override void RaiseCustomColumnSort(GridCustomColumnSortEventArgs e) {
			var handler = (ASPxCardViewCustomColumnSortEventHandler)Events[customColumnSort];
			if(handler != null) handler(this, e as CardViewCustomColumnSortEventArgs);
		}
		protected override void RaiseGridCustomJSProperties(CustomJSPropertiesEventArgs e) {
			var handler = Events[EventCustomJsProperties] as ASPxCardViewClientJSPropertiesEventHandler;
			if(handler != null) handler(this, e as ASPxCardViewClientJSPropertiesEventArgs);
		}
		protected internal override string RaiseCustomErrorText(ASPxGridCustomErrorTextEventArgs e) {
			var handler = (ASPxCardViewCustomErrorTextEventHandler)Events[customErrorText];
			if(handler != null) handler(this, e as ASPxCardViewCustomErrorTextEventArgs);
			return e.ErrorText;
		}
		protected internal override string RaiseSummaryDisplayText(ASPxGridSummaryDisplayTextEventArgs e) {
			var handler = (ASPxCardViewSummaryDisplayTextEventHandler)Events[summaryDisplayText];
			if(handler != null)
				handler(this, e as ASPxCardViewSummaryDisplayTextEventArgs);
			return e.Text;
		}
		protected override void RaiseCustomSummaryCalculate(CustomSummaryEventArgs e) {
			var handler = (CustomSummaryEventHandler)Events[customSummaryCalculate];
			if(handler != null)
				handler(this, e);
		}
		protected override void RaiseSubstituteFilter(SubstituteFilterEventArgs e) {
			var handler = (EventHandler<SubstituteFilterEventArgs>)Events[substituteFilter];
			if(handler != null)
				handler(this, e);
		}
		protected override void RaiseSubstituteSortInfo(SubstituteSortInfoEventArgs e) {
			var handler = (EventHandler<SubstituteSortInfoEventArgs>)Events[substituteSortInfo];
			if(handler != null)
				handler(this, e);
		}
		protected override void RaisePageIndexChanged() {
			base.RaisePageIndexChanged();
			var handler = (EventHandler)Events[pageIndexChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override void RaisePageSizeChanged() {
			var handler = (EventHandler)Events[pageSizeChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override void RaiseSelectionChanged() {
			var handler = (EventHandler)Events[selectionChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override void RaiseFocusedRowChanged() {
			var handler = (EventHandler)Events[focusedRowChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override void RaiseStartEditingRow(ASPxStartItemEditingEventArgs e) {
			var handler = (ASPxStartCardEditingEventHandler)Events[startRowEditing];
			if(handler != null) handler(this, e as ASPxStartCardEditingEventArgs);
		}
		protected override void RaiseCancelEditingRow(ASPxStartItemEditingEventArgs e) {
			var handler = (ASPxStartCardEditingEventHandler)Events[cancelRowEditing];
			if(handler != null) handler(this, e as ASPxStartCardEditingEventArgs);
		}
		protected override void RaiseFilterControlCustomFilterExpressionDisplayText(CustomFilterExpressionDisplayTextEventArgs e) {
			var handler = Events[customFilterExpressionDisplayText] as CustomFilterExpressionDisplayTextEventHandler;
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowInserting(ASPxDataInsertingEventArgs e) {
			var handler = (ASPxDataInsertingEventHandler)Events[rowInserting];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowInserted(ASPxDataInsertedEventArgs e) {
			var handler = (ASPxDataInsertedEventHandler)Events[rowInserted];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowUpdating(ASPxDataUpdatingEventArgs e) {
			var handler = (ASPxDataUpdatingEventHandler)Events[rowUpdating];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowUpdated(ASPxDataUpdatedEventArgs e) {
			var handler = (ASPxDataUpdatedEventHandler)Events[rowUpdated];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowDeleting(ASPxDataDeletingEventArgs e) {
			var handler = (ASPxDataDeletingEventHandler)Events[rowDeleting];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowDeleted(ASPxDataDeletedEventArgs e) {
			var handler = (ASPxDataDeletedEventHandler)Events[rowDeleted];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseParseValue(ASPxParseValueEventArgs e) {
			var handler = (ASPxParseValueEventHandler)Events[parseValue];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseInitNewRow(ASPxDataInitNewRowEventArgs e) {
			var handler = (ASPxDataInitNewRowEventHandler)Events[initNewRow];
			if(handler != null) handler(this, e);
		}
		protected override void RaiseRowValidating(ASPxGridDataValidationEventArgs e) {
			var handler = (ASPxCardViewDataValidationEventHandler)Events[rowValidating];
			if(handler != null) handler(this, e as ASPxCardViewDataValidationEventArgs);
		}
		protected override void RaiseBatchUpdate(ASPxDataBatchUpdateEventArgs e) {
			var handler = (ASPxDataBatchUpdateEventHandler)Events[batchUpdate];
			if(handler != null) handler(this, e);
		}
		protected internal override ASPxGridColumnDisplayTextEventArgs CreateColumnDisplayTextEventArgs(IWebGridDataColumn column, int visibleIndex, IValueProvider provider, object value) {
			return new ASPxCardViewColumnDisplayTextEventArgs((CardViewColumn)column, visibleIndex, value, provider);
		}
		protected override ASPxGridColumnDataEventArgs CreateColumnDataEventArgs(IWebGridDataColumn column, int listSourceRowIndex, object value, bool isGetAction) {
			return new ASPxCardViewColumnDataEventArgs(column as CardViewColumn, listSourceRowIndex, value, isGetAction);
		}
		protected internal override ASPxGridBeforeHeaderFilterFillItemsEventArgs CreateBeforeHeaderFilterFillItemsEventArgs(IWebGridDataColumn column) {
			return new ASPxCardViewBeforeHeaderFilterFillItemsEventArgs(column as CardViewColumn);
		}
		protected internal override ASPxGridHeaderFilterEventArgs CreateHeaderFilterFillItemsEventArgs(IWebGridDataColumn column, GridHeaderFilterValues values) {
			return new ASPxCardViewHeaderFilterEventArgs(column as CardViewColumn, values);
		}
		protected internal override ASPxGridEditorEventArgs CreateCellEditorInitializeEventArgs(IWebGridDataColumn column, int visibleIndex, ASPxEditBase editor, object keyValue, object value) {
			return new ASPxCardViewEditorEventArgs(column as CardViewColumn, visibleIndex, editor, keyValue, value);
		}
		protected internal override ASPxGridEditorCreateEventArgs CreateSearchPanelEditorCreateEventArgs(EditPropertiesBase editorProperties, object value) {
			return new ASPxCardViewSearchPanelEditorCreateEventArgs(editorProperties, value);
		}
		protected internal override ASPxGridEditorEventArgs CreateSearchPanelEditorInitializeEventArgs(ASPxEditBase editor, object value) {
			return new ASPxCardViewSearchPanelEditorEventArgs(editor, value);
		}
		protected internal override ASPxGridBeforeColumnGroupingSortingEventArgs CreateBeforeColumnSortingGroupingEventArgs(IWebGridDataColumn column, ColumnSortOrder sortOrder, int sortIndex, int groupIndex) {
			return new ASPxCardViewBeforeColumnSortingEventArgs(column as CardViewColumn, sortOrder, sortIndex);
		}
		protected internal override ASPxGridCustomCallbackEventArgs CreateCustomCallbackEventArgs(string parameters) {
			return new ASPxCardViewCustomCallbackEventArgs(parameters);
		}
		protected internal override ASPxGridCustomCallbackEventArgs CreateCustomDataCallbackEventArgs(string parameters) {
			return new ASPxCardViewCustomDataCallbackEventArgs(parameters);
		}
		protected internal override ASPxGridCustomButtonCallbackEventArgs CreateCustomButtonCallbackEventArgs(string buttonID, int visibleIndex) {
			return new ASPxCardViewCustomButtonCallbackEventArgs(buttonID, visibleIndex);
		}
		protected internal override ASPxGridAfterPerformCallbackEventArgs CreateAfterPerformCallbackEventArgs(string callbackName, string[] args) {
			return new ASPxCardViewAfterPerformCallbackEventArgs(callbackName, args);
		}
		protected internal override GridCustomColumnSortEventArgs CreateCustomColumnSortEventArgs(IWebGridDataColumn column, object value1, object value2, ColumnSortOrder sortOrder) {
			return new CardViewCustomColumnSortEventArgs(column as CardViewColumn, value1, value2, sortOrder);
		}
		protected internal override CustomJSPropertiesEventArgs CreateCustomJSPropertiesEventArgs(Dictionary<string, object> properties) {
			return new ASPxCardViewClientJSPropertiesEventArgs(properties);
		}
		protected internal override ASPxGridCustomErrorTextEventArgs CreateCustomErrorTextEventArgs(Exception exception, GridErrorTextKind errorTextKind, string errorText) {
			return new ASPxCardViewCustomErrorTextEventArgs(exception, errorTextKind, errorText);
		}
		protected internal override ASPxStartItemEditingEventArgs CreateStartEditingEventArgs(object editingKeyValue) {
			return new ASPxStartCardEditingEventArgs(editingKeyValue);
		}
		protected internal override ASPxGridDataValidationEventArgs CreateItemValidatingEventArgs(int visibleIndex, bool isNew) {
			return new ASPxCardViewDataValidationEventArgs(visibleIndex, isNew);
		}
		#endregion
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxCardView), CardViewScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientCardView";
		}
		protected override void InitializeClientObjectScript(StringBuilder stb, string localVarName, string clientID) {
			base.InitializeClientObjectScript(stb, localVarName, clientID);
			stb.AppendFormat("{0}.layoutMode={1};\n", localVarName, HtmlConvertor.ToScript((int)Settings.LayoutMode));
			if(RenderHelper.UseEndlessPaging)
				stb.AppendFormat("{0}.endlessPagingMode={1};\n", localVarName, HtmlConvertor.ToScript((int)SettingsPager.EndlessPagingMode));
			if(RenderHelper.AllowBatchEditing)
				stb.AppendFormat("{0}.colCount={1};\n", localVarName, HtmlConvertor.ToScript(SettingsPager.SettingsTableLayout.ColumnCount));
		}
		protected override string ClientColumnName { get { return "ASPxClientCardViewColumn"; } }
		protected override object[] GetClientColumnArgs(IWebGridColumn column) {
			var col = column as CardViewColumn;
			return new object[] { 
				col.Name,
				GetColumnGlobalIndex(col),
				col.FieldName,
				col.Visible,
				col.ColumnAdapter.AllowSort,
				col.ColumnAdapter.IsMultiSelectHeaderFilter
			};
		}
		protected override void PopulateBatchEditClientState(Hashtable state) {
			base.PopulateBatchEditClientState(state);
			state["layoutItemPaths"] = ColumnHelper.ColumnLayoutItems.ToDictionary(i => ColumnHelper.GetColumnGlobalIndex(i.Column), i => i.Path);
		}
		protected override string GetControlDesignerType() { return "DevExpress.Web.Design.CardViewCommonFormDesigner"; }
	}
}
namespace DevExpress.Web.Internal {
	public class CardViewCookies : GridCookiesBase {
		public CardViewCookies(ASPxCardView cardView)
			: base(cardView) {
		}
		protected override string Version { get { return Settings.Version; } }
		protected override bool StorePaging { get { return Settings.StorePaging; } }
		protected override bool StoreGroupingAndSorting { get { return Settings.StoreGroupingAndSortingInternal; } }
		protected override bool StoreFiltering { get { return Settings.StoreFiltering; } }
		protected override bool StoreSearchPanelFiltering { get { return Settings.StoreSearchPanelFiltering; } }
		protected ASPxCardViewCookiesSettings Settings { get { return Grid.SettingsCookies; } }
		protected new ASPxCardView Grid { get { return (ASPxCardView)base.Grid; } }
	}
}

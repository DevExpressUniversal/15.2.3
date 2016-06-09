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
using DevExpress.Web.FilterControl;
using DevExpress.Web.Internal;
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
using System.Web;
namespace DevExpress.Web {
	public abstract class ASPxGridBase : ASPxDataWebControl, IWebDataOwner, IWebControlPageSettings, IWebDataEvents,
		IDataControllerSort, IWebColumnsOwner, IRequiresLoadPostDataControl, IPopupFilterControlOwner, 
		IPopupFilterControlStyleOwner, IHeaderFilterPopupOwner, IPagerOwner, IControlDesigner, IFormLayoutOwner {
		static string[] ChildControlNames = new string[] { "Web", "Editors" };
		public const string 
			GridScriptResourcePath = "DevExpress.Web.Scripts.GridView.",
			GridScriptResourceName = GridScriptResourcePath + "Grid.js",
			GridEndlessPagingScriptResourceName = GridScriptResourcePath + "EndlessPaging.js",
			GridBatchEditingScriptResourceName = GridScriptResourcePath + "BatchEditing.js";
		bool fireFocusedRowChangedOnClient = false, fireSelectionChangedOnClient = false;
		bool addCustomJSPropertiesScript = false;
		CallbackInfo internalCallbackInfo;
		Dictionary<string, ASPxGridCallBackMethod> callBacks;
		Dictionary<string, ASPxGridCallBackFunction> internalCallBacks;
		int lockUpdate;
		static readonly object
			focusedRowChangedPendingFlag = new object(),
			selectionChangedPendingFlag = new object(),
			proxyPageIndexChanged = new object(),
			filterControlOperationVisibility = new object(),
			filterControlParseValue = new object(),
			filterControlCustomValueDisplayText = new object(),
			filterControlColumnsCreated = new object(),
			filterControlCriteriaValueEditorInitialize = new object(),
			filterControlCriteriaValueEditorCreate = new object();
		public ASPxGridBase() {
			DataProxy = CreateDataProxy();
			DataProxy.OwnerDataBinding = new WebDataProxyOwnerInvoker(DataBindNoControls);
			Columns = CreateColumnCollection();
			ColumnHelper = CreateColumnHelper();
			RenderHelper = CreateRenderHelper();
			FilterHelper = CreateFilterHelper();
			EndlessPagingHelper = CreateEndlessPagingHelper();
			BatchEditHelper = CreateBatchEditHelper();
			CommandButtonHelper = new GridCommandButtonHelper(this);
			ClientStylesInfo = CreateClientStylesInfo();
			ImagesEditors = CreateEditorImages();
			StylesEditors = CreateEditorStyles();
			StylesPager = CreatePagerStyles();
			SettingsPager = CreateSettingsPager();
			SettingsEditing = CreateSettingsEditing();
			Settings = CreateGridSettings();
			SettingsBehavior = CreateBehaviorSettings();
			SettingsCookies = CreateCookiesSettings();
			SettingsCommandButton = CreateCommandButtonSettings();
			SettingsDataSecurity = CreateDataSecuritySettings();
			SettingsPopup = CreatePopupSettings();
			SettingsSearchPanel = CreateSearchPanelSettings();
			SettingsFilterControl = CreateFilterControlSettings();
			FormatConditions = CreateFormatConditions();
			ImagesFilterControl = new FilterControlImages(this);
			StylesFilterControl = new FilterControlStyles(this);
			SortedColumns = new List<IWebGridDataColumn>();
			PendingEvents = new GridEventsHelper();
			CallbackState = new GridCallbackState();
			EnableCallBacks = true;
			EnableCallBacksInternal = true;
			EnableClientSideAPIInternal = true;
			ScrollToVisibleIndexOnClient = -1;
			WebColumnsOwnerImpl = CreateWebColumnsOwnerImpl();
			StylesPopup = CreatePopupControlStyles();
			SettingsText = CreateSettingsText();
			EditFormLayoutProperties = CreateEditFormLayoutProperties();
			EditFormLayoutProperties.IsStandalone = false;
			EditFormLayoutProperties.DataOwner = this;
			FilterColumnsBuilder = CreateFilterColumnsBuilder();
		}
		protected abstract GridFormLayoutProperties CreateEditFormLayoutProperties();
		protected abstract GridEndlessPagingHelper CreateEndlessPagingHelper();
		protected abstract GridBatchEditHelper CreateBatchEditHelper();
		protected abstract GridRenderHelper CreateRenderHelper();
		protected abstract GridColumnHelper CreateColumnHelper();
		protected abstract GridFilterHelper CreateFilterHelper();
		protected abstract GridClientStylesInfo CreateClientStylesInfo();
		protected abstract GridPopupControlStyles CreatePopupControlStyles();
		protected abstract ASPxGridTextSettings CreateSettingsText();
		protected abstract EditorImages CreateEditorImages();
		protected abstract EditorStyles CreateEditorStyles();
		protected abstract PagerStyles CreatePagerStyles();
		protected abstract ASPxGridPagerSettings CreateSettingsPager();
		protected abstract ASPxGridEditingSettings CreateSettingsEditing();
		protected abstract ASPxGridSettings CreateGridSettings();
		protected abstract ASPxGridBehaviorSettings CreateBehaviorSettings();
		protected abstract ASPxGridCookiesSettings CreateCookiesSettings();
		protected abstract ASPxGridCommandButtonSettings CreateCommandButtonSettings();
		protected abstract ASPxGridDataSecuritySettings CreateDataSecuritySettings();
		protected abstract ASPxGridPopupControlSettings CreatePopupSettings();
		protected abstract ASPxGridSearchPanelSettings CreateSearchPanelSettings();
		protected abstract ASPxGridFilterControlSettings CreateFilterControlSettings();
		protected abstract IList CreateSummaryItemCollection();
		protected abstract ASPxSummaryItemBase CreateTotalSummaryItem();
		protected abstract GridFormatConditionCollection CreateFormatConditions();
		protected abstract string DefaultCssResourceName { get; }
		protected virtual WebColumnsOwnerDefaultImplementation CreateWebColumnsOwnerImpl() {
			return new WebColumnsOwnerDefaultImplementation(this, Columns);
		}
		protected virtual WebDataSelection CreateSelection(WebDataProxy proxy) {
			return new WebDataSelection(proxy);
		}
		public override void Dispose() {
			DataProxy.Dispose();
			base.Dispose();
		}
		public void ForceDataRowType(Type type) {
			DataProxy.ForceDataRowType(type);
		}
		protected override string[] GetChildControlNames() { return ChildControlNames; }
		protected internal GridCallbackState CallbackState { get; private set; }
		protected GridEventsHelper PendingEvents { get; private set; }
		protected internal GridEndlessPagingHelper EndlessPagingHelper { get; private set; }
		protected internal GridBatchEditHelper BatchEditHelper { get; private set; }
		protected internal GridRenderHelper RenderHelper { get; private set; }
		protected internal GridColumnHelper ColumnHelper { get; private set; }
		protected internal GridCommandButtonHelper CommandButtonHelper { get; private set; }
		protected internal GridFilterHelper FilterHelper { get; private set; }
		protected internal GridClientStylesInfo ClientStylesInfo { get; private set; }
		protected internal FilterControlColumnBuilder FilterColumnsBuilder { get; private set; }
		protected void ResetFilterHelper() {
			FilterHelper = CreateFilterHelper();
		}
		protected virtual FilterControlColumnBuilder CreateFilterColumnsBuilder() {
			return new FilterControlColumnBuilder();
		}
		protected virtual WebDataProxy CreateDataProxy() { return new WebDataProxy(this, this, this); }
		protected internal WebDataProxy DataProxy { get; private set; }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public WebDataProxy DataBoundProxy {
			get {
				DataBindNoControls();
				return DataProxy;
			}
		}
		protected internal List<IWebGridDataColumn> SortedColumns { get; private set; }
		protected IWebColumnsOwner WebColumnsOwnerImpl { get; private set; }
		protected Table RootTable { get; private set; }
		protected TableCell RootCell { get; private set; }
		protected internal GridUpdatableContainer ContainerControl { get; private set; }
		protected static void ResetVisibleColumnsRecursive(IWebColumnsOwner owner) {
			owner.ResetVisibleColumns();
			foreach(object child in owner.Columns) {
				IWebColumnsOwner childOwner = child as IWebColumnsOwner;
				if(childOwner != null)
					ResetVisibleColumnsRecursive(childOwner);
			}
		}
		protected int VisibleRowCount { get { return DataProxy.VisibleRowCount; } }
		protected int VisibleStartIndex { get { return DataProxy.VisibleStartIndex; } }
		protected internal WebDataSelection Selection { get { return DataProxy.Selection; } }
		protected internal int FocusedRowIndex { get { return DataProxy.FocusedRowVisibleIndex; } set { DataProxy.FocusedRowVisibleIndex = value; } }
		protected object GetRow(int visibleIndex) {
			DataBindNoControls();
			return DataProxy.GetRow(visibleIndex);
		}
		protected DataRow GetDataRow(int visibleIndex) {
			DataRowView rowView = GetRow(visibleIndex) as DataRowView;
			return rowView != null ? rowView.Row : null;
		}
		protected List<object> GetSelectedFieldValues(params string[] fieldNames) {
			return DataProxy.GetSelectedValues(fieldNames);
		}
		protected List<object> GetFilteredSelectedValues(params string[] fieldNames) {
			return DataProxy.GetFilteredSelectedValues(fieldNames);
		}
		protected object GetRowValues(int visibleIndex, params string[] fieldNames) {
			return DataProxy.GetValues(visibleIndex, fieldNames);
		}
		protected object GetRowValuesByKeyValue(object keyValue, params string[] fieldNames) {
			return DataProxy.GetValuesByKeyValue(keyValue, fieldNames);
		}
		protected int FindVisibleIndexByKeyValue(object keyValue) {
			return DataProxy.FindVisibleIndexByKey(keyValue, false);
		}
		protected List<object> GetCurrentPageRowValues(params string[] fieldNames) {
			return DataProxy.GetCurrentPageRowValues(fieldNames);
		}
		protected bool MakeRowVisible(object keyValue) {
			return DataProxy.MakeRowVisible(keyValue);
		}
		[Browsable(false), UrlProperty]
		public override string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false)]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		protected internal GridClientSideEvents ClientSideEvents { get { return (GridClientSideEvents)base.ClientSideEventsInternal; } }
		protected internal bool EnableCallBacks {
			get { return GetBoolProperty("EnableCallBacks", true); }
			set {
				SetBoolProperty("EnableCallBacks", true, value);
				LayoutChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height { get { return base.Height; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override System.Drawing.Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		protected internal int GetColumnGlobalIndex(IWebGridColumn column) {
			return ColumnHelper.GetColumnGlobalIndex(column);
		}
		public void ResetToFirstPage() {
			PageIndex = 0;
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.LoadFirstPage();
		}
		protected internal int PageIndex {
			get { return PageIndexInternal; }
			set {
				if(RenderHelper.UseEndlessPaging)
					return;
				PageIndexInternal = value;
			}
		}
		protected internal int PageIndexInternal {
			get { return CheckPageIndex(CallbackState.Get<int>("PageIndex")); }
			set {
				if(ChangePageIndex(value)) {
					bool requireRaisePageSizeChanged = ASPxPagerBase.IsAnyShowAll(value, PageIndex);
					OnPageIndexChanged();
					if(requireRaisePageSizeChanged)
						RaisePageSizeChanged();
				}
			}
		}
		protected internal int PageSize {
			get { return SettingsPager.PageSize; }
			set {
				var pageSize = PageIndex == -1 ? -1 : PageSize;
				if(pageSize == value)
					return;
				var events = new Action[]{ OnPageSizeChanged }.ToList();
				if(value > 0) {
					if(PageIndex < 0) {
						ChangePageIndex(0);
						events.Add(RaisePageIndexChanged);
					}
					SetPageSizeInternal(value);
				} else if(value == -1) {
					ChangePageIndex(-1);
					events.Add(RaisePageIndexChanged);
				}
				events.ForEach(x => x());
			}
		}
		protected virtual void SetPageSizeInternal(int newPageSize) {
			SettingsPager.PageSize = newPageSize;
		}
		protected internal int InitialPageSize { get; set; }
		protected bool ChangePageIndex(int value) {
			value = CheckPageIndex(value);
			if(PageIndex == value)
				return false;
			DataProxy.PageSizeShowAllItem = (value == -1 && SettingsPager.PageSizeItemSettings.Visible);
			CallbackState.Put("PageIndex", value);
			return true;
		}
		protected internal int PageCount { get { return DataProxy.PageCount; } }
		protected internal string FilterExpression {
			get { return CallbackState.Get<string>("FilterExpression", ""); }
			set {
				value = GridRenderHelper.TrimString(value);
				if (FilterExpression == value) return;
				OnFilterExpressionChanging(value, true);
			}
		}
		protected internal bool FilterEnabled {
			get { return CallbackState.Get<bool>("FilterEnabled", true); }
			set {
				if (FilterEnabled == value) return;
				OnFilterExpressionChanging(FilterExpression, value);
			}
		}
		protected internal string SearchPanelFilter {
			get { return CallbackState.Get<string>("SearchPanelFilter", ""); }
			set {
				if(SearchPanelFilter == value) return;
				CallbackState.Put("SearchPanelFilter", value);
				OnFilterChanged();
			}
		}
		protected internal FilterControlColumnCollection CreateFilterControlColumnCollection() {  
			var descriptorContainer = DataProxy.CreateFilterControlPropertyDescriptorsContainer();
			var s = SettingsFilterControl;
			var result = FilterColumnsBuilder.GenerateColumns(descriptorContainer, s.AllowHierarchicalColumns, s.MaxHierarchyDepth, s.ShowAllDataSourceColumns, ColumnHelper.FilterControlColumns.OfType<IFilterColumn>());
			RaiseFilterControlColumnsCreated(new FilterControlColumnsCreatedEventArgs(result));
			return result;
		}
		protected bool IsApplyFilterCalled { get { return ASPxPopupFilterControl.GetIsApplyCalled(UniqueID, IdSeparator.ToString()); } }
		protected bool LockSummaryChange { get; set; }
		protected string SearchPanelColumnInfoKey { get { return CallbackState.Get<string>("SearchPanelColumnInfoKey", ""); } set { CallbackState.Put("SearchPanelColumnInfoKey", value); } }
		protected internal string KeyFieldName {
			get { return GetStringProperty("KeyFieldName", string.Empty); }
			set {
				if(value == null)
					value = string.Empty;
				if(!String.IsNullOrEmpty(KeyFieldName) && KeyFieldName != value) 
					OnKeyFieldNameChanged();
				SetStringProperty("KeyFieldName", string.Empty, value);
				DataProxy.KeyFieldName = value;
			}
		}
		protected virtual void OnKeyFieldNameChanged() {
			Selection.UnselectAll();
			DataProxy.ClearStoredPageSelectionResult();
		}
		protected internal virtual void OnSummaryChanged(object sender, CollectionChangeEventArgs e) {
			if(IsLoading() || LockSummaryChange)
				return;
			LayoutChanged();
			BindAndSynchronizeDataProxy();
		}
		protected virtual void OnFormatConditionSummaryChanged(){
			if(IsLoading() || LockSummaryChange)
				return;
			BindAndSynchronizeDataProxy();
		}
		protected internal string PreviewFieldName {
			get { return GetStringProperty("PreviewFieldName", string.Empty); }
			set {
				if(value == null)
					value = string.Empty;
				SetStringProperty("PreviewFieldName", string.Empty, value);
			}
		}
		protected internal bool AutoGenerateColumns { get { return GetBoolProperty("AutoGenerateColumns", true); } set { SetBoolProperty("AutoGenerateColumns", true, value); } }
		protected internal bool ClientVisible { get { return base.ClientVisibleInternal; } set { base.ClientVisibleInternal = value; } }
		protected internal virtual bool EnableRowsCache { get { return GetBoolProperty("EnableRowsCache", true); } set { SetBoolProperty("EnableRowsCache", true, value); } }
		protected internal bool EnableCallbackAnimation { get { return EnableCallbackAnimationInternal; } set { EnableCallbackAnimationInternal = value; } }
		protected internal bool EnableCallbackCompression { get { return base.EnableCallbackCompressionInternal; } set { base.EnableCallbackCompressionInternal = value; } }
		protected internal bool EnablePagingCallbackAnimation { get { return EnableSlideCallbackAnimationInternal; } set { EnableSlideCallbackAnimationInternal = value; } }
		protected internal AutoBoolean EnablePagingGestures { get { return EnableSwipeGesturesInternal; } set { EnableSwipeGesturesInternal = value; } }
		protected internal DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		protected internal event ASPxClientLayoutHandler ClientLayout { add { Events.AddHandler(EventClientLayout, value); } remove { Events.RemoveHandler(EventClientLayout, value); } }
		protected internal event FilterControlOperationVisibilityEventHandler FilterControlOperationVisibility { add { Events.AddHandler(filterControlOperationVisibility, value); } remove { Events.RemoveHandler(filterControlOperationVisibility, value); } }
		protected internal event FilterControlParseValueEventHandler FilterControlParseValue { add { Events.AddHandler(filterControlParseValue, value); } remove { Events.RemoveHandler(filterControlParseValue, value); } }
		protected internal event FilterControlCustomValueDisplayTextEventHandler FilterControlCustomValueDisplayText { add { Events.AddHandler(filterControlCustomValueDisplayText, value); } remove { Events.RemoveHandler(filterControlCustomValueDisplayText, value); } }
		protected internal event FilterControlColumnsCreatedEventHandler FilterControlColumnsCreated { add { Events.AddHandler(filterControlColumnsCreated, value); } remove { Events.RemoveHandler(filterControlColumnsCreated, value); } }
		protected internal event FilterControlCriteriaValueEditorInitializeEventHandler FilterControlCriteriaValueEditorInitialize { add { Events.AddHandler(filterControlCriteriaValueEditorInitialize, value); } remove { Events.RemoveHandler(filterControlCriteriaValueEditorInitialize, value); } }
		protected internal event FilterControlCriteriaValueEditorCreateEventHandler FilterControlCriteriaValueEditorCreate { add { Events.AddHandler(filterControlCriteriaValueEditorCreate, value); } remove { Events.RemoveHandler(filterControlCriteriaValueEditorCreate, value); } }
		protected virtual void OnFilterExpressionChanging(string value, bool isFilterEnabled) {
			this.columnFilterInfo = null;
			CallbackState.Put("FilterExpression", value);
			CallbackState.Put("FilterEnabled", isFilterEnabled);
			OnFilterChanged();
		}
		protected internal void OnFilterChanged() {
			LayoutChanged();
			int prevFocus = FocusedRowIndex;
			BindAndSynchronizeDataProxy();
			if(AutoExpandAllGroupsInternal)
				ExpandAllInternal();
			if(prevFocus < 0 && DataProxy.IsFocusedRowEnabled)
				FocusedRowIndex = VisibleStartIndex;
			if(prevFocus != FocusedRowIndex)
				this.fireFocusedRowChangedOnClient = true;
		}
		protected internal GridPopupControlStyles StylesPopup { get; private set; }
		protected internal ASPxGridTextSettings SettingsText { get; private set; }
		protected internal GridStyles Styles { get { return StylesInternal as GridStyles; } }
		protected internal GridImages Images { get { return ImagesInternal as GridImages; } }
		bool ShouldSerializeColumns() { return Columns.Count > 0 && !AutoGenerateColumns; }
		protected internal GridColumnCollection Columns { get; private set; }
		protected internal ASPxGridBehaviorSettings SettingsBehavior { get; private set; }
		protected internal ASPxGridPagerSettings SettingsPager { get; private set; }
		protected internal ASPxGridEditingSettings SettingsEditing { get; private set; }
		protected internal ASPxGridSettings Settings { get; private set; }
		protected internal ASPxGridCookiesSettings SettingsCookies { get; private set; }
		protected internal ASPxGridPopupControlSettings SettingsPopup { get; private set; }
		protected internal ASPxGridCommandButtonSettings SettingsCommandButton { get; private set; }
		protected internal ASPxGridDataSecuritySettings SettingsDataSecurity { get; private set; }
		protected internal ASPxGridSearchPanelSettings SettingsSearchPanel { get; private set; }
		protected internal ASPxGridFilterControlSettings SettingsFilterControl { get; private set; }
		protected internal new ASPxGridLoadingPanelSettings SettingsLoadingPanel { get { return (ASPxGridLoadingPanelSettings)base.SettingsLoadingPanel; } }
		protected internal GridFormatConditionCollection FormatConditions { get; private set; }
		protected internal Collection TotalSummary { get { return DataProxy.TotalSummary as Collection; } }
		protected internal virtual List<ASPxSummaryItemBase> GetActiveTotalSummaryItems() {
			return TotalSummary.OfType<ASPxSummaryItemBase>().Where(item => item.SummaryType != SummaryItemType.None).ToList();
		}
		protected internal virtual object GetTotalSummaryValue(ASPxSummaryItemBase item) {
			if(item == null)
				throw new ArgumentNullException("item");
			return DataProxy.GetTotalSummaryValue(item);
		}
		protected internal bool DataSourceForceStandardPaging { get { return GetBoolProperty("DataSourceForceStandardPaging", false); } set { SetBoolProperty("DataSourceForceStandardPaging", false, value); } }
		[ Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings { get { return (ControlStyle as AppearanceStyle).Paddings; } }
		protected internal string Caption { get { return GetStringProperty("Caption", ""); } set { SetStringProperty("Caption", "", value); } }
		protected internal bool KeyboardSupport {
			get { return GetBoolProperty("KeyboardSupport", false); }
			set {
				if(value == KeyboardSupport) return;
				SetBoolProperty("KeyboardSupport", false, value);
				LayoutChanged();
			}
		}
		protected internal GridFormLayoutProperties EditFormLayoutProperties { get; private set; }
		protected internal FormLayoutHorizontalAlign CommandLayoutItemDefaultHorizontalAlign { get { return !RenderHelper.IsRightToLeft ? FormLayoutHorizontalAlign.Right : FormLayoutHorizontalAlign.Left; } }
		protected override SettingsLoadingPanel CreateSettingsLoadingPanel() {
			return new ASPxGridLoadingPanelSettings(this);
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected internal int SortCount { get { return SortedColumns.Count; } }
		protected internal bool IsExported { get; private set; }
		protected void DoRowValidation() {
			if (IsEditing) {
				DataProxy.ValidateRow(true);
				LayoutChanged();
			}
		}
		protected void StartEdit(int visibleIndex) {
			if(RenderHelper.AllowBatchEditing || !SettingsDataSecurity.AllowEdit) 
				return;
			LoadDataIfNotBinded(true);
			LayoutChanged(); 
			DataProxy.StartEdit(visibleIndex);
			LayoutChanged();
		}
		protected void UpdateEdit() {
			if(!DataProxy.IsEditing && !RenderHelper.AllowBatchEditing)
				return;
			LoadDataIfNotBinded(true);
			RenderHelper.ParseEditorValues();
			bool shouldExpandAll = AutoExpandAllGroupsInternal && IsNewRowEditing && VisibleRowCount < 1 && GroupCountInternal > 0; 
			if(DataProxy.EndEdit()) {
				RequireDataBinding();
				DataBind();
				if(shouldExpandAll)
					ExpandAllInternal();
			} else {
				LayoutChanged();
				(this as IWebDataOwner).ValidateEditTemplates();
			}
		}
		protected void CancelEdit() {
			DataProxy.CancelEdit();
			LayoutChanged();
		}
		protected void AddNewRow() {
			if(RenderHelper.AllowBatchEditing || !SettingsDataSecurity.AllowInsert)
				return;
			LoadDataIfNotBinded(true);
			LayoutChanged(); 
			DataProxy.AddNewRow();
		}
		protected void DeleteRow(int visibleIndex) {
			if(!SettingsDataSecurity.AllowDelete)
				return;
			LoadDataIfNotBinded(true);
			DataProxy.DeleteRow(visibleIndex);
			RequireDataBinding();
			DataBind();
			DataProxy.CheckFocusedRowChanged(); 
		}
		protected override object GetCallbackErrorData() {
			if(RenderHelper.AllowBatchEditing && BatchEditHelper.CallbackErrorData.Count > 0)
				return BatchEditHelper.CallbackErrorData;
			return base.GetCallbackErrorData();
		}
		protected void ShowFilterControl() { ShowFilterControl(true); }
		protected void HideFilterControl() { ShowFilterControl(false); }
		protected void ShowFilterControl(bool show) {
			if (IsFilterControlVisible == show) return;
			CallbackState.Put("IsFilterControlShowing", show);
			LayoutChanged();
		}
		protected internal bool IsFilterControlVisible { get { return CallbackState.Get<bool>("IsFilterControlShowing"); } }
		protected int ScrollToVisibleIndexOnClient { get; set; }
		protected abstract Dictionary<string, object> GetEditTemplateValuesCore();
		protected Dictionary<object, ITemplate> GetFormLayoutEditItems() {
			Dictionary<object, ITemplate> editItems = new Dictionary<object, ITemplate>();
			EditFormLayoutProperties.ForEach(delegate(LayoutItemBase item) {
				if(item is ColumnLayoutItem && ((ColumnLayoutItem)item).Template != null)
					editItems[item.Path] = ((ColumnLayoutItem)item).Template;
			});
			return editItems;
		}
		protected Dictionary<string, object> MergeDictionaries(params Dictionary<string, object>[] dicts) {
			Dictionary<string, object> res = new Dictionary<string, object>();
			foreach(Dictionary<string, object> dict in dicts) {
				if(dict == null) continue;
				foreach(KeyValuePair<string, object> pair in dict) {
					res[pair.Key] = pair.Value;
				}
			}
			return res.Count == 0 ? null : res;
		}
		protected internal string EditTemplateValidationGroup { get { return ClientID; } }
		protected internal bool IsEditing { get { return DataProxy.IsEditing; } }
		protected internal bool IsNewRowEditing { get { return DataProxy.IsNewRowEditing; } }
		protected int EditingRowVisibleIndex { get { return DataProxy.EditingRowVisibleIndex; } }
		protected virtual void BeginUpdate() {
			this.lockUpdate++;
		}
		protected virtual void EndUpdate() {
			if(--this.lockUpdate == 0) OnEndUpdate();
		}
		protected virtual bool IsLockUpdate { get { return this.lockUpdate != 0; } }
		protected virtual void OnEndUpdate() {
			CheckBindAndSynchronizeDataProxy();
		}
		protected internal virtual string GetColumnFilterString(IWebGridDataColumn column) {
			if(column == null) return string.Empty;
			return CriteriaOperator.ToString(GetColumnFilter(column));
		}
		protected internal virtual CriteriaOperator GetColumnFilter(IWebGridDataColumn column) {
			if (column == null) return null;
			CriteriaOperator rv;
			ColumnFilterInfo.TryGetValue(new OperandProperty(column.FieldName), out rv);
			return rv;
		}
		protected internal void FilterByHeaderPopup(IWebGridColumn column, string value) {
			if(column == null) throw new ArgumentNullException("column");
			var dcColumn = column as IWebGridDataColumn;
			if(dcColumn == null) new ArgumentException("Column should be DataColumn", "column");
			Type columnType = dcColumn.Adapter.DataType;
			string[] stringValues = HtmlConvertor.FromJSON<ArrayList>(value).OfType<string>().Select(i => ASPxListEdit.GetClientValue(i, columnType)).ToArray();
			CriteriaOperator filter = FilterHelper.CreateHeaderFilter(dcColumn, stringValues);
			ApplyFilterToColumn(dcColumn, filter);
		}
		protected virtual void ApplyFilterToColumn(IWebGridDataColumn column, CriteriaOperator criteria) {
			if(column == null) throw new ArgumentNullException("column");
			ColumnFilterInfo[new OperandProperty(column.FieldName)] = criteria;
			string newFilterExpression = CriteriaOperator.ToString(GroupOperator.And(ColumnFilterInfo.Values));
			if(FilterExpression != newFilterExpression)
				FilterExpression = newFilterExpression;
			else
				OnFilterExpressionChanging(FilterExpression, true);
		}
		internal bool IsAllowDataSourcePaging() {
			return DataSourceForceStandardPaging && !DesignMode;
		}
		protected internal void ClearSort() {
			if(SortCount == 0) return;
			ResetSortGroup();
			BindAndSynchronizeDataProxy();
		}
		protected bool IsAllowSort(IWebGridColumn column) {
			var dataColumn = column as IWebGridDataColumn;
			return dataColumn != null && dataColumn.Adapter.AllowSort;
		}
		protected bool IsAllowGroup(IWebGridColumn column) { 
			if(IsAllowDataSourcePaging()) return false;
			var dataColumn = column as IWebGridDataColumn;
			return dataColumn != null && dataColumn.Adapter.AllowGroup;
		}
		protected internal bool IsReadOnly(IWebGridDataColumn column) {
			if(column.ReadOnly) return true;
			return DataProxy.IsReadOnly(column.FieldName);
		}
		protected internal ColumnSortOrder SortBy(IWebGridColumn column, ColumnSortOrder value) {
			if(column == null) throw new ArgumentNullException("column");
			var dcColumn = column as IWebGridDataColumn;
			if(dcColumn == null) new ArgumentException("Column should be DataColumn", "column");
			if(dcColumn.Adapter.GroupIndex < 0) {
				if (!IsAllowSort(dcColumn)) return ColumnSortOrder.None;
			}else {
				if (!IsAllowGroup(dcColumn)) return ColumnSortOrder.None;
			}
			if(dcColumn.SortOrder == value) return value;
			if(value == ColumnSortOrder.None) {
				SortedColumnsChanged(dcColumn, -1, true, ColumnSortOrder.None);
				return ColumnSortOrder.None;
			}
			int index = dcColumn.SortIndex;
			if(index < 0) index = SortCount;
			if(SortedColumnsChanged(dcColumn, index, false, value) == -1) return ColumnSortOrder.None;
			return value;
		}
		protected internal int SortBy(IWebGridColumn column, int value) {
			if(column == null) throw new ArgumentNullException("column");
			var dcColumn = column as IWebGridDataColumn;
			if(dcColumn == null) new ArgumentException("Column should be DataColumn", "column");
			if(!IsAllowSort(dcColumn)) return -1;
			ColumnSortOrder order = dcColumn.SortOrder;
			if(order == ColumnSortOrder.None)
				order = ColumnSortOrder.Ascending;
			value = SortedColumnsChanged(dcColumn, value, false, order);
			return value;
		}
		protected internal EditorImages ImagesEditors { get; private set; }
		protected internal FilterControlImages ImagesFilterControl { get; private set; }
		protected internal PagerStyles StylesPager { get; private set; }
		protected internal EditorStyles StylesEditors { get; private set; }
		protected internal FilterControlStyles StylesFilterControl { get; private set; }
		protected virtual bool AllowFireFocusedOrSelectedRowChangedOnClient { get { return !IsFirstLoad || SettingsBehavior.AllowClientEventsOnLoad; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle { get { return base.DisabledStyle; } }
		protected virtual string SaveClientLayout() {
			return SaveClientState();
		}
		protected virtual void LoadClientLayout(string layoutData) {
			LoadClientState(layoutData);
		}
		protected override bool IsStateSavedToCookies { get { return !DesignMode && SettingsCookies.Enabled; } }
		protected override string GetStateCookieName() {
			return base.GetStateCookieName(SettingsCookies.CookiesID);
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected internal override void LoadClientState(string state) { 
			ProcessCookies(CreateControlCookies(), state);
		}
		protected internal override string SaveClientState() { 
			return CreateControlCookies().SaveState(PageIndex, PageSize);
		}
		protected abstract GridCookiesBase CreateControlCookies();
		protected void ProcessSEOPaging() {
			if(Request == null || !IsMvcRender() && (Page == null || DesignMode || Page.IsPostBack))
				return;
			ProcessCookies(RenderHelper.SEO, HttpUtils.GetValueFromRequest(RenderHelper.GetSEOID()));
		}
		protected virtual void ProcessCookies(GridCookiesBase cookies, string state) {
			if(string.IsNullOrEmpty(state))
				return;
			if(!cookies.LoadState(state)) return;
			if(SettingsCookies.StoreGroupingAndSortingInternal)
				ResetSortGroup();
			LoadGridColumnsState(cookies.ColumnsState);
			if(SettingsCookies.StoreFiltering) {
				FilterExpression = cookies.FilterExpression;
				FilterEnabled = cookies.FilterEnabled;
			}
			if(SettingsCookies.StoreSearchPanelFiltering)
				SearchPanelFilter = cookies.SearchPanelFilter;
			BindAndSynchronizeDataProxy();
			cookies.SetPageSize();
			cookies.SetPageIndex();
		}
		protected int SortedColumnsChanged(IWebGridDataColumn column, int value, bool changeGrouping, ColumnSortOrder sortOrder) {
			ColumnHelper.Invalidate();
			if(!IsAllowGroup(column)) changeGrouping = false;
			if(!changeGrouping && column.Adapter.GroupIndex < 0 && !IsAllowSort(column)) return -1;
			if((changeGrouping || column.Adapter.GroupIndex > -1) && !IsAllowGroup(column)) return -1;
			var eventArgs = CreateBeforeColumnSortingGroupingEventArgs(column, column.SortOrder, column.SortIndex, column.Adapter.GroupIndex);
			if(value < 0) value = -1;
			if(changeGrouping && value != -1)
				column.Adapter.UngroupedSortOrder = column.SortOrder;
			if(changeGrouping || value == -1) column.Adapter.SetGroupIndex(-1);
			if((changeGrouping && value == -1) && (sortOrder != ColumnSortOrder.None))
				column.Adapter.SetSortOrder(sortOrder);
			else {
				SortedColumns.Remove(column);
				column.Adapter.SetSortIndex(-1);
				column.Adapter.SetSortOrder(ColumnSortOrder.None);
			}
			if(value > -1) {
				if(value >= SortedColumns.Count)
					SortedColumns.Add(column);
				else 
					SortedColumns.Insert(value, column);
				column.Adapter.SetSortOrder(sortOrder);
			}
			if(value > -1)
				value = SortedColumns.IndexOf(column);
			if(changeGrouping) 
				column.Adapter.SetGroupIndex(value);
			UpdateGroupSortIndexes();
			RaiseBeforeColumnSortingGrouping(eventArgs);
			BindAndSynchronizeDataProxy();
			if(changeGrouping && AutoExpandAllGroupsInternal) {
				ExpandAllInternal();
			}
			return value;
		}
		protected virtual bool AutoExpandAllGroupsInternal { get { return false; } } 
		protected internal virtual int GroupCountInternal { get { return 0; } } 
		protected virtual void ExpandAllInternal() { } 
		protected internal void BuildSortedColumns() {
			SortedColumns.Clear();
			SortedColumns.AddRange(ColumnHelper.AllDataColumns.Where(c => c.SortIndex > -1));
			SortedColumns.Sort(new Comparison<IWebGridDataColumn>(CompareColumnsByGroupIndex));
			UpdateGroupSortIndexes();
		}
		void UpdateGroupSortIndexes() {
			for(int i = 0; i < SortedColumns.Count; i++)
				SortedColumns[i].Adapter.SetSortIndex(i);
			SortedColumns.Sort(new Comparison<IWebGridDataColumn>(CompareColumnsByGroupIndex));
			int gc = GroupCountInternal;
			for(int i = 0; i < SortedColumns.Count; i++) {
				if(i < gc) SortedColumns[i].Adapter.SetGroupIndex(i);
				SortedColumns[i].Adapter.SetSortIndex(i);
			}
		}
		int CompareColumnsByGroupIndex(IWebGridDataColumn col1, IWebGridDataColumn col2) {
			if(col1.Adapter.GroupIndex == col2.Adapter.GroupIndex) return Comparer.Default.Compare(col1.SortIndex, col2.SortIndex);
			if(col1.Adapter.GroupIndex < 0) return 1;
			if(col2.Adapter.GroupIndex < 0) return -1;
			return Comparer.Default.Compare(col1.Adapter.GroupIndex, col2.Adapter.GroupIndex);
		}
		protected internal override void ResetControlHierarchy() {
			CommandButtonHelper.Invalidate();
			base.ResetControlHierarchy();
			this.dummyNamingContainer = null;
			DestroyFilterData();
			ColumnHelper.Invalidate();
			RenderHelper.Invalidate();
			FilterHelper.Invalidate();
		}
		protected internal new bool ChildControlsCreated { get { return base.ChildControlsCreated; } }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.dummyNamingContainer = null;
		}
		protected internal virtual void OnBeforeCreateControlHierarchy() {
			LoadDataIfNotBinded();
		}
		protected internal virtual void SyncCallbackState() {
			if(EnableRowsCache) {
				CallbackState.Put("Data", DataProxy.SaveData());
				CallbackState.Put("FormatState", FormatConditions.SaveCacheState(DataProxy));
			}
			CallbackState.Put("State", SaveGridControlState());
		}
		class InternalNamingContainer : Control, INamingContainer {
			public InternalNamingContainer() {
				EnableViewState = false;
			}
			protected override void Render(HtmlTextWriter writer) {
			}
		}
		Control dummyNamingContainer;
		protected internal Control DummyNamingContainer {
			get {
				if(dummyNamingContainer == null) {
					dummyNamingContainer = new InternalNamingContainer();
					Controls.Add(DummyNamingContainer);
				}
				return dummyNamingContainer;
			}
		}
		protected internal override bool IsSwipeGesturesEnabled() {
			return base.IsSwipeGesturesEnabled() && !RenderHelper.ShowHorizontalScrolling && !RenderHelper.UseEndlessPaging && PageCount > 1;
		}
		protected override bool HasLoadingPanel() {
			return false; 
		}
		protected internal string GetLoadingPanelIDInternal() { return GetLoadingPanelID(); }
		protected internal string GetLoadingDivIDInternal() { return GetLoadingDivID(); }
		protected abstract GridUpdatableContainer CreateContainerControl();
		protected internal abstract ASPxGridPager CreatePagerControl(string id);
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateRootControls();
		}
		protected virtual void CreateRootControls() {
			RenderHelper.CreateGridDummyEditors(DummyNamingContainer);
			RootTable = RenderUtils.CreateTable(true);
			Controls.Add(RootTable);
			var row = RenderUtils.CreateTableRow();
			RootTable.Rows.Add(row);
			RootCell = RenderUtils.CreateTableCell();
			row.Cells.Add(RootCell);
			ContainerControl = CreateContainerControl();
			RootCell.Controls.Add(ContainerControl);
			this.hierarchyChanged = true;
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(this, RootTable);
			if(KeyboardSupport)
				RootTable.AccessKey = String.Empty;
			RenderUtils.SetVisibility(RootTable, IsClientVisible(), true);
			var style = IsEnabled() ? RenderHelper.GetRootTableStyle() : RenderHelper.GetDisabledRootTableStyle();
			style.AssignToControl(RootTable, false);
			style.Paddings.AssignToControl(RootCell);
			if(Browser.IsIE) {
				var cellPadding = new Paddings(0, 0, 0, 0);
				cellPadding.CopyFrom(style.Paddings);
				cellPadding.AssignToControl(RootCell);
			}
			RootTable.CellPadding = 0;
			RootTable.CellSpacing = 0;
			RootTable.Width = RenderHelper.GetRootTableWidth();
			if(IsAccessibilityCompliantRender() && IsAriaSupported())
				RootTable.Attributes.Add("role", "presentation");
		}
		protected virtual bool IsFirstLoad { get { return Page == null || !Page.IsPostBack || !HasClientCallbackState; } } 
		protected override void OnInit(EventArgs e) {
			InitialPageSize = SettingsPager.PageSize;
			base.OnInit(e);
			if(DesignMode) BuildSortedColumns();
		}
		protected override void OnLoad(EventArgs e) {
			if(IsMvcRender())
				ViewStateUtils.TrackObjectsViewState(new IStateManager[] { TotalSummary });
			BuildSortedColumns();
			ResetVisibleColumnsRecursive(this);
			base.OnLoad(e);
			ProcessSEOPaging();
			CheckPendingEvents();
			if(!Visible || IsFirstLoad) {
				if(RenderHelper.IsFocusedRowEnabled)
					DataProxy.OnFocusedRowChanged();
			}
		}
		IDictionary<OperandProperty, CriteriaOperator> columnFilterInfo;
		protected internal IDictionary<OperandProperty, CriteriaOperator> ColumnFilterInfo {
			get {
				if(columnFilterInfo == null)
					columnFilterInfo = CriteriaColumnAffinityResolver.SplitByColumns(CriteriaOperator.Parse(FilterExpression));
				return columnFilterInfo;
			}
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			IsGridStateLoaded = false;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(Columns);
			list.Add(Settings);
			list.Add(SettingsPager);
			list.Add(SettingsEditing);
			list.Add(SettingsBehavior);
			list.Add(SettingsCookies);
			list.Add(SettingsPopup);
			list.Add(SettingsCommandButton);
			list.Add(SettingsDataSecurity);
			list.Add(SettingsSearchPanel);
			list.Add(SettingsFilterControl);
			list.Add(StylesEditors);
			list.Add(StylesFilterControl);
			list.Add(StylesPager);
			list.Add(ImagesEditors);
			list.Add(ImagesFilterControl);
			list.Add(EditFormLayoutProperties);
			list.Add(FormatConditions);
			return list.ToArray();
		}
		protected override object SaveViewState() {
			if(!Visible) {
				SyncCallbackState();
				SetStringProperty(ClientStateProperties.CallbackState, null, CallbackState.Save());
			}
			return base.SaveViewState();
		}
		protected override void TrackViewState() {
			base.TrackViewState();
			ViewStateUtils.TrackObjectsViewState(new IStateManager[] { TotalSummary });
		}
		protected override bool HasFunctionalityScripts() { return true; }
		protected override bool IsWebSourcesRegisterRequired() { return true; }
		protected override bool HasClientInitialization() { return RenderHelper.ShowHorizontalScrolling || RenderHelper.ShowVerticalScrolling || base.HasClientInitialization(); }
		protected override void RegisterIncludeScripts() { 
			base.RegisterIncludeScripts();
			RegisterDragAndDropUtilsScripts();
			RegisterTableScrollUtilsScript(RenderHelper.RequireTablesHelperScripts);
			RegisterIncludeScript(typeof(ASPxGridView), GridEndlessPagingScriptResourceName, RenderHelper.UseEndlessPaging);
			RegisterIncludeScript(typeof(ASPxGridView), GridBatchEditingScriptResourceName, RenderHelper.AllowBatchEditing);
			RegisterIncludeScript(typeof(ASPxGridView), GridScriptResourceName);
		}
		protected override void RegisterIconSpriteCssFiles() {
			base.RegisterIconSpriteCssFiles();
			GridConditionFormatingIconsHelper.RegisterSpriteCssFile(Page, FormatConditions);
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxGridBase), DefaultCssResourceName);
		}
		public override void RegisterStyleSheets() {
			base.RegisterStyleSheets();
			for(int i = 0; i < RenderHelper.DummyEditorList.Count; i++)
				RenderHelper.DummyEditorList[i].RegisterStyleSheets();
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			InitializeClientObjectScript(stb, localVarName, clientName);
		}
		protected virtual void InitializeClientObjectScript(StringBuilder stb, string localVarName, string clientID) {
			stb.AppendFormat("{0}.callBacksEnabled={1};\n", localVarName, HtmlConvertor.ToScript(EnableCallBacks));
			var rowCount = RenderHelper.UseEndlessPaging ? EndlessPagingHelper.LoadedRowCount : DataProxy.VisibleRowCountOnPage;
			stb.AppendFormat("{0}.pageRowCount={1};\n", localVarName, rowCount);
			stb.AppendFormat("{0}.pageRowSize={1};\n", localVarName, DataProxy.PageSize);
			stb.AppendFormat("{0}.pageIndex={1};\n", localVarName, SettingsPager.Mode == GridViewPagerMode.ShowAllRecords ? -1 : PageIndex);
			stb.AppendFormat("{0}.pageCount={1};\n", localVarName, PageCount);
			stb.AppendFormat("{0}.selectedWithoutPageRowCount={1};\n", localVarName, DataProxy.GetCachedSelectedRowCountWithoutCurrentPage());
			if(!RenderHelper.UseEndlessPaging)
				stb.AppendFormat("{0}.visibleStartIndex={1};\n", localVarName, DataProxy.VisibleStartIndex);
			stb.AppendFormat("{0}.focusedRowIndex={1};\n", localVarName, DataProxy.FocusedRowVisibleIndex);
			stb.AppendFormat("{0}.allowFocusedRow={1};\n", localVarName, HtmlConvertor.ToScript(DataProxy.IsFocusedRowEnabled));
			if(RenderHelper.HasAnySelectCheckbox) {
				stb.AppendFormat("{0}.checkBoxImageProperties = {1};\n", localVarName, ImagePropertiesSerializer.GetImageProperties(RenderHelper.GetCheckImages(), this));
				stb.AppendFormat("{0}.icbFocusedStyle = {1};\n", localVarName, InternalCheckboxControl.SerializeFocusedStyle(RenderHelper.GetCheckBoxFocusedStyle(), this));
			}
			stb.AppendFormat("{0}.allowSelectByItemClick={1};\n", localVarName, HtmlConvertor.ToScript(SettingsBehavior.AllowSelectByItemClick)); 
			stb.AppendFormat("{0}.allowSelectSingleRowOnly={1};\n", localVarName, HtmlConvertor.ToScript(SettingsBehavior.AllowSelectSingleItemOnly)); 
			if(ScrollToVisibleIndexOnClient > -1) {
				stb.AppendFormat("{0}.scrollToRowIndex={1};\n", localVarName, ScrollToVisibleIndexOnClient);
			}
			if(RenderHelper.ShowHorizontalScrolling)
				stb.AppendFormat("{0}.horzScroll={1};\n", localVarName, HtmlConvertor.ToScript((int)RenderHelper.HorizontalScrollBarMode));
			if(RenderHelper.ShowVerticalScrolling)
				stb.AppendFormat("{0}.vertScroll={1};\n", localVarName, HtmlConvertor.ToScript((int)RenderHelper.VerticalScrollBarMode));
			if(SettingsPager.Mode != GridViewPagerMode.ShowAllRecords) {
				if(RenderHelper.IsVirtualScrolling)
					stb.AppendFormat("{0}.isVirtualScrolling=true;\n", localVarName);
				if(RenderHelper.IsVirtualSmoothScrolling)
					stb.AppendFormat("{0}.isVirtualSmoothScrolling=true;\n", localVarName);
			}
			if(RenderHelper.UseEndlessPaging) {
				stb.AppendFormat("{0}.useEndlessPaging=true;\n", localVarName);
				stb.AppendFormat("{0}.resetScrollTop={1};\n", localVarName, HtmlConvertor.ToScript(EndlessPagingHelper.ShouldLoadFirstPage));
			}
			stb.AppendFormat("{0}.callbackOnFocusedRowChanged={1};\n", localVarName, HtmlConvertor.ToScript(SettingsBehavior.ProcessFocusedItemChangedOnServer));
			stb.AppendFormat("{0}.callbackOnSelectionChanged={1};\n", localVarName, HtmlConvertor.ToScript(SettingsBehavior.ProcessSelectionChangedOnServer));
			GenerateClientColumns(stb, localVarName);
			GeneratePendingClientEvents(stb, localVarName);
			GenerateEditorIDList(stb, localVarName);
			if(SettingsBehavior.ConfirmDelete)
				stb.AppendFormat("{0}.confirmDelete={1};\n", localVarName, HtmlConvertor.ToJSON(SettingsText.GetConfirmDelete()));
			if(SettingsBehavior.EnableItemHotTrack) {
				Style style = RenderHelper.GetRowHotTrackStyle();
				stb.AppendFormat("{0}.rowHotTrackStyle = [{1},{2}];\n", localVarName, HtmlConvertor.ToScript(style.CssClass), HtmlConvertor.ToScript(style.GetStyleAttributes(this).Value ?? ""));
			}
			stb.AppendFormat("{0}.editState={1};\n", localVarName, GetEditStateClientCode());
			stb.AppendFormat("{0}.editItemVisibleIndex={1};\n", localVarName, EditingRowVisibleIndex);
			if(KeyboardSupport) {
				stb.AppendFormat("{0}.enableKeyboard={1};\n", localVarName, HtmlConvertor.ToScript(KeyboardSupport));
				if(!String.IsNullOrEmpty(AccessKey))
					stb.AppendFormat("{0}.accessKey={1};\n", localVarName, HtmlConvertor.ToScript(AccessKey));
				if(!String.IsNullOrEmpty(RenderHelper.CustomKbdHelperName))
					stb.AppendFormat("{0}.customKbdHelperName={1};\n", localVarName, HtmlConvertor.ToScript(RenderHelper.CustomKbdHelperName));
			}
			if(RenderHelper.AllowBatchEditing) {
				stb.AppendFormat("{0}.allowBatchEditing=true;\n", localVarName);
				stb.AppendFormat("{0}.batchEditClientState={1};\n", localVarName, HtmlConvertor.ToJSON(GetBatchEditClientState()));
				stb.AppendFormat("{0}.batchEditPageValues={1};\n", localVarName, HtmlConvertor.ToJSON(BatchEditHelper.GetCurrentPageValues()));
			}
			if(SettingsSearchPanel.Delay != ASPxGridViewSearchPanelSettings.DefaultInputDelay)
				stb.AppendFormat("{0}.searchFilterDelay={1};\n", localVarName, SettingsSearchPanel.Delay);
			if(!SettingsSearchPanel.AllowTextInputTimer)
				stb.AppendFormat("{0}.allowSearchFilterTimer=false;\n", localVarName);
			if(!string.IsNullOrEmpty(RenderHelper.CustomSearchPanelEditorClientID))
				stb.AppendFormat("{0}.customSearchPanelEditorID={1};\n", localVarName, HtmlConvertor.ToScript(RenderHelper.CustomSearchPanelEditorClientID));
			stb.AppendFormat("{0}.searchPanelFilter={1};\n", localVarName, HtmlConvertor.ToScript(SearchPanelFilter));
			if(SettingsBehavior.AllowEllipsisInText)
				stb.AppendFormat("{0}.enableEllipsis=true;\n", localVarName);
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(ClientStateProperties.CallbackState, GetCallbackStateValueForRender());
			result.Add(GridClientStateProperties.SelectedState, RenderHelper.GetSelectInputValue());
			if(!RenderHelper.RequireEndlessPagingPartialLoad) {
				result.Add(GridClientStateProperties.PageKeysState, RenderHelper.GetCurrentPageKeyValues());
				if(RenderHelper.AllowBatchEditing)
					result.Add(GridClientStateProperties.BatchEditClientModifiedValues, BatchEditHelper.SavedClientState);
				if(RenderHelper.IsFocusedRowEnabled)
					result.Add(GridClientStateProperties.FocusedRowState, "");
				if(RenderHelper.HasScrolling)
					result.Add(GridClientStateProperties.ScrollState, GetClientObjectStateValue<ArrayList>(GridClientStateProperties.ScrollState));
				if(SettingsBehavior.AllowSelectByItemClick)
					result.Add(GridClientStateProperties.LastMultiSelectIndex, GetClientObjectStateValueString(GridClientStateProperties.LastMultiSelectIndex));
				if(RenderHelper.UseEndlessPaging)
					result.Add(GridClientStateProperties.EndlessPagingGroupState, EndlessPagingHelper.GetGroupState());
			}
			return result;
		}
		int GetEditStateClientCode() {
			if(IsNewRowEditing)
				return SettingsEditing.NewItemPosition == GridViewNewItemRowPosition.Bottom ? 3 : 2;
			return IsEditing ? 1 : 0;
		}
		void GenerateEditorIDList(StringBuilder stb, string localVarName) {
			if (RenderHelper.EditingRowEditorList.Count < 1) return;
			var editorIDs = RenderHelper.EditingRowEditorList.Select(e => e.ClientID).ToArray();
			stb.AppendFormat("{0}.editorIDList={1};\n", localVarName, HtmlConvertor.ToJSON(editorIDs));
		}
		protected internal string GetCallbackClientObjectScript() {
			StringBuilder stb = new StringBuilder();
			stb.AppendFormat("var {0} = ASPx.GetControlCollection().Get({1});\r\n", ShortClientLocalVariableName, HtmlConvertor.ToScript(ClientID));
			InitializeClientObjectScript(stb, ShortClientLocalVariableName, ClientID);
			if(this.addCustomJSPropertiesScript)
				GetCustomJSPropertiesScript(stb, ShortClientLocalVariableName);
			return RenderUtils.GetScriptHtml(stb.ToString());
		}
		protected virtual void GenerateClientColumns(StringBuilder stb, string localVarName) {
			stb.AppendFormat(localVarName + ".columns = [");
			foreach(var col in ColumnHelper.AllColumns) {
				if(col != ColumnHelper.AllColumns[0])
					stb.Append(",\n");
				var args = GetClientColumnArgs(col);
				stb.AppendFormat("new {0}(", ClientColumnName);
				for(int i = 0; i < args.Length; i++) {
					object arg = args[i];
					if(arg is Boolean)
						arg = (Boolean)arg ? 1 : 0;
					if(i > 0)
						stb.Append(",");
					stb.Append(HtmlConvertor.ToScript(arg));
				}
				stb.Append(")");
			}
			stb.Append("];\n");
		}
		protected abstract object[] GetClientColumnArgs(IWebGridColumn column);
		protected abstract string ClientColumnName { get; }
		protected void GeneratePendingClientEvents(StringBuilder stb, string localVarName) {
			if(EnableCallBacks && AllowFireFocusedOrSelectedRowChangedOnClient) {
				List<string> events = new List<string>();
				if(this.fireFocusedRowChangedOnClient)
					events.Add("RaiseFocusedItemChanged");
				if(this.fireSelectionChangedOnClient)
					events.Add("RaiseSelectionChangedOutOfServer");
				if(events.Count > 0)
					stb.AppendFormat("{0}.pendingEvents={1};\n", localVarName, HtmlConvertor.ToJSON(events));
			}
		}
		protected virtual Hashtable GetBatchEditClientState() {
			var result = new Hashtable();
			PopulateBatchEditClientState(result);
			return result;
		}
		protected virtual void PopulateBatchEditClientState(Hashtable state) {
			state["editMode"] = (int)SettingsEditing.BatchEditSettings.EditMode;
			state["startEditAction"] = (int)SettingsEditing.BatchEditSettings.StartEditAction;
			state["validateOnEndEdit"] = SettingsEditing.BatchEditSettings.AllowValidationOnEndEdit ? 1 : 0;
			state["allowEndEditOnError"] = SettingsEditing.BatchEditSettings.AllowEndEditOnValidationError ? 1 : 0;
			state["isNewRowOnTop"] = SettingsEditing.NewItemPosition == GridViewNewItemRowPosition.Top;
			state["editColumnIndices"] = ColumnHelper.EditColumns.Select(c => ColumnHelper.GetColumnGlobalIndex(c)).ToArray();
			state["updateInfo"] = BatchEditHelper.GetUpdateInfo();
			state["validationInfo"] = BatchEditHelper.GetClientValidationInfo();
			state["checkColumnsDisplayHtml"] = BatchEditHelper.GetCheckColumnsDisplayHtml();
			state["colorColumnsDisplayHtml"] = BatchEditHelper.GetColorEditColumnsDisplayHtml();
			state["binaryImageColumnsDisplayHtml"] = BatchEditHelper.GetBinaryImageColumnsDisplayHtml();
			state["templateColumnIndices"] = ColumnHelper.BatchEditTemplateColumns.Select(c => ColumnHelper.GetColumnGlobalIndex(c)).ToArray();
			if(SettingsEditing.BatchEditSettings.ShowConfirmOnLosingChanges)
				state["confirmUpdate"] = SettingsText.GetConfirmBatchUpdate();
		}
		protected Dictionary<string, ASPxGridCallBackMethod> CallBacks {
			get {
				if(callBacks == null) {
					callBacks = new Dictionary<string, ASPxGridCallBackMethod>();
					RegisterCallBacks(callBacks);
				}
				return callBacks;
			}
		}
		protected internal Dictionary<string, ASPxGridCallBackFunction> InternalCallBacks {
			get {
				if(internalCallBacks == null) {
					internalCallBacks = new Dictionary<string, ASPxGridCallBackFunction>();
					RegisterInternalCallBacks(internalCallBacks);
				}
				return internalCallBacks;
			}
		}
		protected internal bool IsErrorOnCallbackCore { get { return IsErrorOnCallback; } }
		protected internal CallbackInfo InternalCallbackInfo { get { return internalCallbackInfo; } }
		protected virtual GridCallbackArgumentsReader GetCreateCallbackArgumentReader(string eventArgument){
			return new GridCallbackArgumentsReader(eventArgument);
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			if (IsErrorOnCallback) return;
			this.internalCallbackInfo = null;
			GridCallbackArgumentsReader argumentsReader = GetCreateCallbackArgumentReader(eventArgument);
			SetCallbackStateString(argumentsReader.CallbackState);
			if (!string.IsNullOrEmpty(argumentsReader.PageSelectionResult)) {
				LoadDataIfNotBinded();
			}
			LoadGridControlState(argumentsReader);
			RequireDataBinding();
			SetEditorValues(argumentsReader.EditValues, CanIgnoreInvalidEditorValues(argumentsReader.CallbackArguments) || argumentsReader.InternalCallbackIndex != -1);
			if (argumentsReader.InternalCallbackIndex == -1) {
				CheckPendingEvents();
				if (argumentsReader.FocusedRowIndex != -1) {
					DataProxy.FocusedRowVisibleIndex = argumentsReader.FocusedRowIndex;
					this.fireFocusedRowChangedOnClient = false;
				}
				if(!ProcessContextMenuItemClick(argumentsReader.ContextMenuValues))
					DoCallBackPostBack(argumentsReader.CallbackArguments);
			} else {
				this.internalCallbackInfo = new CallbackInfo(argumentsReader.InternalCallbackIndex, argumentsReader.CallbackArguments);
			}
		}
		protected virtual bool ProcessContextMenuItemClick(string values) { 
			return false;
		}
		bool CanIgnoreInvalidEditorValues(string callBack) {
			if (!IsEditing)
				return false;
			return callBack == "10|CANCELEDIT" || callBack.StartsWith("9|STARTEDIT") || callBack.StartsWith("9|ADDNEWROW");
		}
		protected virtual void SetEditorValues(string editorValues, bool canIgnoreInvalidValues) {
			if (string.IsNullOrEmpty(editorValues)) return;
			GridViewCallBackEditorValuesReader reader = new GridViewCallBackEditorValuesReader(editorValues);
			reader.Process();
			if (reader.Values.Count > 0) {
				Dictionary<string, object> values = new Dictionary<string, object>();
				foreach(EditorValueInfo info in reader.Values) {
					var column = ColumnHelper.AllColumns[info.ColumnIndex] as IWebGridDataColumn;
					if(column == null || string.IsNullOrEmpty(column.FieldName))
						continue;
					var binaryImageProperties = column.PropertiesEdit as BinaryImageEditProperties;
					if(binaryImageProperties != null) { 
						var dummyEditor = new ASPxBinaryImage {
							AllowEdit = true,
							BinaryStorageMode = binaryImageProperties.BinaryStorageMode
						};
						BinaryStorageData data = BinaryStorage.GetResourceData(dummyEditor,
							binaryImageProperties.BinaryStorageMode, info.Value);
						values[column.FieldName] = data != null ? data.Content : null;
					} else
						values[column.FieldName] = info.Value;
				}
				DataProxy.SetEditorValues(values, canIgnoreInvalidValues);
			}
		}
		protected internal virtual string[] GetCallBackPostBackArgs(ref string eventArgument) {
			List<string> deserializedArgs = CommonUtils.DeserializeStringArray(eventArgument);
			string command = deserializedArgs[0];
			deserializedArgs.RemoveAt(0);
			eventArgument = command;
			return deserializedArgs.ToArray();
		}
		protected virtual void DoCallBackPostBack(string eventArgument) {
			if (string.IsNullOrEmpty(eventArgument))
				return;
			CheckPendingEvents();
			string[] args = GetCallBackPostBackArgs(ref eventArgument);
			if (CallBacks.ContainsKey(eventArgument))
				CallBacks[eventArgument](args);
			RaiseAfterPerformCallback(CreateAfterPerformCallbackEventArgs(eventArgument, args));
		}
		bool CheckRequireDataBound() {
			if (this.requireDataBound && !DataProxy.IsBound) {
				this.requireDataBound = false;
				ResetControlHierarchy();
				RequireDataBinding();
				EnsureDataBound();
				return true;
			}
			return false;
		}
		protected override object GetCallbackResult() {
			Hashtable result = new Hashtable();
			result[CallbackResultProperties.Html] = GetCallbackResultHtml();
			if(InternalCallbackInfo == null)
				result[CallbackResultProperties.StateObject] = GetClientObjectState();
			return result;
		}
		protected virtual string GetCallbackResultHtml() {
			Dictionary<string, string> globalScripts = new Dictionary<string, string>(ResourceManager.ScriptBlocksRegistrator.RegisteredScriptBlocks);
			ResourceManager.ScriptRegistrator.EnsureResourcesSynchronized(Page);
			Dictionary<string, ResourceData> scriptResources = new Dictionary<string, ResourceData>(ResourceManager.ScriptRegistrator.RegisteredResources);
			ResourceManager.CssRegistrator.EnsureResourcesSynchronized(Page);
			Dictionary<string, ResourceData> cssResources = new Dictionary<string, ResourceData>(ResourceManager.CssRegistrator.RegisteredResources);
			string res = GetCallbackResultCore();
			if(CheckRequireDataBound()) {
				ResourceManager.ScriptBlocksRegistrator.RegisteredScriptBlocks.Clear();
				foreach(KeyValuePair<string, string> pair in globalScripts) { ResourceManager.ScriptBlocksRegistrator.RegisteredScriptBlocks[pair.Key] = pair.Value; }
				ResourceManager.ScriptRegistrator.RegisteredResources.Clear();
				foreach(KeyValuePair<string, ResourceData> pair in scriptResources) { ResourceManager.ScriptRegistrator.RegisteredResources[pair.Key] = pair.Value; }
				ResourceManager.CssRegistrator.RegisteredResources.Clear();
				foreach(KeyValuePair<string, ResourceData> pair in cssResources) { ResourceManager.CssRegistrator.RegisteredResources[pair.Key] = pair.Value; }
				res = GetCallbackResultCore();
			}
			return res.Replace("\0", string.Empty); 
		}
		string GetCallbackResultCore() {
			if(InternalCallbackInfo != null)
				return GetFunctionalCallbackResultCore();
			EnsureSearchPanelFilterActual();
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.OnBeforeGetCallbackResult();
			EnsureChildControls();
			BeginRendering();
			try {
				return RenderHelper.RequireEndlessPagingPartialLoad ? EndlessPagingHelper.GetEndlessPagingCallbackResult(GetCallbackClientObjectScript()) :
					RenderUtils.GetRenderResult(ContainerControl, GetCallbackClientObjectScript()) + GetPostponeScript();
			}
			finally {
				EndRendering();
			}
		}
		string GetFunctionalCallbackResultCore() {
			string res = "FB|" + InternalCallbackInfo.CallbackId.ToString() + "|";
			string eventArgument = InternalCallbackInfo.EventArgument;
			string[] args = GetCallBackPostBackArgs(ref eventArgument);
			if(InternalCallBacks.ContainsKey(eventArgument)) {
				object value = InternalCallBacks[eventArgument](args);
				res += HtmlConvertor.ToJSON(value);
			}
			return res;
		}
		protected override void BeforeRender() {
			base.BeforeRender();
			EnsureSearchPanelFilterActual();
		}
		protected void OnPagingChanged() {
			DataProxy.ClearStoredPageSelectionResult(); 
			if(IsAllowDataSourcePaging() && !DataProxy.IsBound) 
				CallbackState.Put("PageIndex", DataHelper.RetrievePageIndex());
			if(IsAllowDataSourcePaging() && DataProxy.IsBound) {
				PerformSelect();
			} else
				DataBindNoControls();
			LayoutChanged();
		}
		protected virtual void OnPageIndexChanged() {
			OnPagingChanged();
			RaisePageIndexChanged();
		}
		protected virtual void OnPageSizeChanged() {
			DataProxy.HintGridIsPaged();
			OnPagingChanged();
			RaisePageSizeChanged();
		}
		protected byte[] SaveGridControlState() {
			using(MemoryStream stream = new MemoryStream())
			using(var writer = new TypedBinaryWriter(stream)) {
				SaveGridControlStateCore(writer);
				return stream.ToArray();
			}
		}
		protected virtual void SaveGridControlStateCore(TypedBinaryWriter writer) {
			CreateColumnsState().Save(writer);
			DataProxy.SaveDataState(writer);
			CommandButtonHelper.Save(writer);
			SaveTotalSummaryState(writer);
		}
		protected virtual GridColumnsState CreateColumnsState() {
			return new GridColumnsState(this);
		}
		protected virtual void SaveTotalSummaryState(TypedBinaryWriter writer) {
			if(!IsTotalSummaryHasChanges()) {
				writer.WriteObject(0);
				return;
			}
			writer.WriteObject(TotalSummary.Count);
			foreach(ASPxSummaryItemBase item in TotalSummary)
				item.Save(writer);				
		}
		protected virtual bool IsTotalSummaryHasChanges() {
			var state = ViewStateUtils.SaveViewState(new object(), new IStateManager[] { TotalSummary }) as object[];
			return state != null && state.Length >= 2 && (state[1] is Pair);
		}
		protected bool IsGridStateLoaded { get; private set; }
		protected virtual void LoadGridControlState(GridCallbackArgumentsReader callbackReader) {
			ArrayList pageKeyValues = !string.IsNullOrEmpty(callbackReader.PageKeyValues) ? HtmlConvertor.FromJSON<ArrayList>(callbackReader.PageKeyValues) : null;
			LoadGridControlState(GetControlClientState(callbackReader.PageSelectionResult, pageKeyValues, callbackReader.ColumnResizingResult, null, null));
		}
		protected virtual void LoadGridControlState(Dictionary<string, object> clientState) {
			this.columnFilterInfo = null; 
			byte[] webData = CallbackState.Get<byte[]>("State");
			if(webData == null || webData.Length < 1 || IsGridStateLoaded)
				return;
			LoadDataIfNotBinded();
			using(MemoryStream stream = new MemoryStream(webData))
			using(var reader = new TypedBinaryReader(stream))
				LoadGridControlStateCore(reader, clientState);
			BatchEditHelper.LoadClientState((Hashtable)clientState["BatchEditState"]);
			if(Page != null && (!Page.IsCallback || !IsCallback) && RenderHelper.UseEndlessPaging)
				ResetToFirstPage();
			this.fireSelectionChangedOnClient = false;
			IsGridStateLoaded = true;
		}
		protected virtual void LoadGridControlStateCore(TypedBinaryReader reader, Dictionary<string, object> clientState) {
			ResetSortGroup();
			var columnsState = CreateColumnsState();
			columnsState.Load(reader);
			LoadGridColumnsState(columnsState);
			if(DataProxy.IsBound)
				SynchronizeDataProxy();
			else if(!DataProxy.HasCachedProvider)
				LoadDataIfNotBinded(true);
			var keyValues = clientState["PageKeys"] != null ? (ArrayList)clientState["PageKeys"] : new ArrayList();
			DataProxy.LoadDataState(reader, (string)clientState["PageSelection"], keyValues, true);
			CommandButtonHelper.Load(reader);
			LoadTotalSummaryState(reader);
			EndlessPagingHelper.LoadClientState(columnsState, keyValues, (ArrayList)clientState["EndlessPagingState"]);			
		}
		protected virtual void LoadTotalSummaryState(TypedBinaryReader reader) {
			BeginUpdate();
			int count = reader.ReadObject<int>();
			LockSummaryChange = !DataProxy.IsBound;
			if(count != 0)
				TotalSummary.Clear();
			for(int i = 0; i < count; i++) {
				var loadingItem = CreateTotalSummaryItem();
				loadingItem.Load(reader);
				TotalSummary.Add(loadingItem);
			}
			LockSummaryChange = false;
			EndUpdate();
		}
		protected Dictionary<string, object> GetControlClientState() { 
			return GetControlClientState(
				GetClientObjectStateValueString(GridClientStateProperties.SelectedState),
				GetClientObjectStateValue<ArrayList>(GridClientStateProperties.PageKeysState),
				GetClientObjectStateValueString(GridClientStateProperties.ResizingState),
				GetClientObjectStateValue<ArrayList>(GridClientStateProperties.EndlessPagingGroupState),
				GetClientObjectStateValue<Hashtable>(GridClientStateProperties.BatchEditClientModifiedValues)
			);
		}
		protected internal Dictionary<string, object> GetControlClientState(string pageSelection, IList pageKeys, string resizedColumnWidths, 
			IList endlessPagingState, IDictionary batchEditState) {
			var result = new Dictionary<string, object>();
			result["PageSelection"] = pageSelection; 
			result["PageKeys"] = pageKeys;
			result["ResizedColumnWidths"] = resizedColumnWidths;
			result["EndlessPagingState"] = endlessPagingState;
			result["BatchEditState"] = batchEditState;
			return result;
		}
		void ResetSortGroup() {
			SortedColumns.Clear();
			foreach(var dataColumn in ColumnHelper.AllDataColumns) {
				dataColumn.Adapter.SetSortIndex(-1);
				dataColumn.Adapter.SetGroupIndex(-1);
				dataColumn.Adapter.SetSortOrder(ColumnSortOrder.None);
			}
		}
		void ResetSortOnly(IWebGridColumn excludeColumn) {
			SortedColumns.Clear();
			foreach(var dataColumn in ColumnHelper.AllDataColumns) {
				if (excludeColumn != dataColumn && dataColumn.Adapter.GroupIndex < 0) {
					dataColumn.Adapter.SetSortIndex(-1);
					dataColumn.Adapter.SetSortOrder(ColumnSortOrder.None);
				}
			}
			BuildSortedColumns();
		}
		protected void LoadGridColumnsState(GridColumnsState state) {
			state.Apply();
			ResetVisibleColumnsRecursive(this);
			BuildSortedColumns();
		}
		protected int CheckPageIndex(int pageIndex) {
			if (pageIndex < -1) pageIndex = -1;
			if (IsAllowDataSourcePaging() && !DataProxy.IsBound) return pageIndex;
			if (pageIndex >= DataProxy.PageCount) pageIndex = Math.Max(0, DataProxy.PageCount - 1);
			return pageIndex;
		}
		protected override void Render(HtmlTextWriter writer) {
			if (!PreRendered) {
				EnsureChildControls();
				PrepareControlHierarchy();
			}
			CheckRequireDataBound();
			base.Render(writer);
		}
		protected override void RenderInternal(HtmlTextWriter writer) {
			base.RenderInternal(writer);
			if(IsRenderPostponeScriptAfterMainTable)
				RenderPostponeScript(writer);
		}
		protected internal virtual bool IsRenderPostponeScriptAfterMainTable { get { return !IsCallback && !IsApplyFilterCalled; } }
		protected internal void RenderPostponeScript(HtmlTextWriter writer) {
			writer.Write(GetPostponeScript());
		}
		protected internal string GetPostponeScript() {
			if(DesignMode || !IsScriptEnabled())
				return string.Empty;
			return RenderUtils.GetScriptHtml(string.Format("ASPxClientGridBase.InitializeStyles('{0}',{1},{2})", ClientID, ClientStylesInfo.ToJSON(), HtmlConvertor.ToJSON(CommandButtonHelper.CommandButtonClientIDList)));
		}
		GridFilterData filterData;
		protected GridFilterData FilterData {
			get {
				if(filterData == null) {
					filterData = new GridFilterData(this);
					filterData.OnStart();
				}
				return filterData;
			}
		}
		GridSearchFilterData searchFilterData;
		protected GridSearchFilterData SearchFilterData {
			get { 
				if(string.IsNullOrEmpty(SearchPanelFilter))
					return null;
				if(searchFilterData == null) {
					searchFilterData = new GridSearchFilterData(this);
					searchFilterData.OnStart();
				}
				return searchFilterData;
			}
		}
		GridSortData sortData;
		protected internal GridSortData SortData {
			get {
				if(sortData == null)
					sortData = CreateSortData();
				return sortData;
			}
		}
		protected abstract GridSortData CreateSortData();
		protected void DestroyFilterData() {
			this.filterData = null;
			this.searchFilterData = null;
		}
		protected virtual bool AllowOnlyOneMasterRowExpandedInternal { get { return false; } } 
		#region IWebDataOwner Members
		bool requireDataBound = false;
		void IWebDataOwner.RequireDataBound() { this.requireDataBound = true; }
		DataSourceView IWebDataOwner.GetData() { return GetData(); }
		bool IWebDataOwner.IsDesignTime { get { return DesignMode; } }
		bool IWebDataOwner.IsForceDataSourcePaging {
			get { return IsAllowDataSourcePaging(); }
		}
		DataSourceSelectArguments IWebDataOwner.SelectArguments {
			get {
				if (DataHelper != null) return DataHelper.LastArguments;
				return new DataSourceSelectArguments();
			}
		}
		GridDataHelper DataHelper { get { return DataContainer[DefaultDataHelperName] as GridDataHelper; } }
		int IWebControlPageSettings.PageSize { get { return PageSize; } }
		int IWebControlPageSettings.PageIndex { get { return PageIndex; } set { PageIndex = value; } }
		GridViewPagerMode IWebControlPageSettings.PagerMode { get { return SettingsPager.Mode; } }
		List<IWebColumnInfo> IWebDataOwner.GetColumns() { return ColumnHelper.AllDataColumns.OfType<IWebColumnInfo>().ToList(); }
		IDataControllerSort IWebDataOwner.SortClient { get { return this; } }
		IWebControlObject IWebDataOwner.WebControl { get { return this; } }
		Dictionary<string, object> IWebDataOwner.GetEditTemplateValues() { return GetEditTemplateValuesCore(); }
		bool IWebDataOwner.AllowOnlyOneMasterRowExpanded { get { return AllowOnlyOneMasterRowExpandedInternal; } }
		bool IWebDataOwner.AllowSelectSingleRowOnly { get { return SettingsBehavior.AllowSelectSingleItemOnly; } }
		bool IWebDataOwner.ValidateEditTemplates() { return ASPxEdit.ValidateEditorsInContainer(this, EditTemplateValidationGroup); }
		bool IWebDataOwner.ValidateAutoCreatedEditors() {
			bool editorsAreValid = true;
			foreach(ASPxEditBase editorBase in RenderHelper.EditingRowEditorList) {
				ASPxEdit editor = editorBase as ASPxEdit;
				if(editor != null) {
					editor.Validate();
					editorsAreValid &= editor.IsValid;
				}
			}
			return editorsAreValid;
		}
		bool IWebDataOwner.AllowFocusedRow { get { return RenderHelper.IsFocusedRowEnabled; } }
		GridEndlessPagingHelper IWebDataOwner.EndlessPagingHelper { get { return RenderHelper.UseEndlessPaging ? EndlessPagingHelper : null; } }
		GridBatchEditHelper IWebDataOwner.BatchEditHelper { get { return RenderHelper.AllowBatchEditing ? BatchEditHelper : null; } }
		bool IWebDataOwner.AlwaysSaveSelectionViaSelectedKeys { get { return SettingsBehavior.SelectionStoringMode == GridViewSelectionStoringMode.DataIntegrityOptimized; } }
		bool IWebDataOwner.UseSelectAll { get { return RenderHelper.HasAnySelectAllCheckbox; } }
		WebDataSelection IWebDataOwner.CreateSelection(WebDataProxy proxy) {
			return CreateSelection(proxy);
		}
		IList IWebDataOwner.CreateSummaryItemCollection() {
			return CreateSummaryItemCollection();
		}
		IList IWebDataOwner.GetFormatConditionSummaries() { return FormatConditions.Summaries; }
		bool IWebDataOwner.CacheFilterBuilderColumns { get { return SettingsFilterControl.AllowHierarchicalColumns || SettingsFilterControl.ShowAllDataSourceColumns; } }
		event EventHandler IWebDataOwner.PageIndexChanged { add { Events.AddHandler(proxyPageIndexChanged, value); } remove { Events.RemoveHandler(proxyPageIndexChanged, value); } }
		ASPxStartItemEditingEventArgs IWebDataOwner.CreateStartItemEditingArgs(object editingKey) {
			return CreateStartEditingEventArgs(editingKey);
		}
		ASPxGridDataValidationEventArgs IWebDataOwner.CreateItemValidationEventArgs(int visibleIndex, bool isNew) {
			return CreateItemValidatingEventArgs(visibleIndex, isNew);
		}
		#endregion
		protected internal NameValueCollection PostDataCollectionInternal { get { return PostDataCollection; } }
		protected override bool CanLoadPostDataOnCreateControls() { return true; }
		protected override bool CanLoadPostDataOnLoad() { return false; }
		protected override bool LoadPostData(NameValueCollection postCollection) {
			int prevPageIndex = PageIndex;
			SetCallbackStateString(GetCallbackStateValue());
			LoadGridControlState(GetControlClientState());
			if(DataProxy.IsBound) SynchronizeDataProxy();
			ProcessSEOPaging();
			if(DataSourceForceStandardPaging && DataProxy.IsBound && PageIndex != prevPageIndex)
				DataBind(); 
			LoadFocusedRowIndex();
			return true;
		}
		void SetCallbackStateString(string value) {
			if(String.IsNullOrEmpty(value))
				return;
			CallbackState.Load(value);
		}
		string GetCallbackStateValue() {
			string value = GetClientObjectStateValueString(ClientStateProperties.CallbackState);
			if(!string.IsNullOrEmpty(value)) return value;
			return GetStringProperty(ClientStateProperties.CallbackState, null);
		}
		protected string GetCallbackStateValueForRender() {
			SyncCallbackState();
			return CallbackState.Save();
		}
		protected internal bool HasClientCallbackState { get { return !string.IsNullOrEmpty(GetCallbackStateValue()); } }
		protected virtual void LoadFocusedRowIndex() {
			string focusedRowString = GetClientObjectStateValueString(GridClientStateProperties.FocusedRowState);
			if(string.IsNullOrEmpty(focusedRowString))
				return;
			int newFocusedRow;
			if(int.TryParse(focusedRowString, out newFocusedRow)) {
				DataProxy.FocusedRowVisibleIndex = newFocusedRow;
				this.fireFocusedRowChangedOnClient = false;
			}
		}
		protected override void RaisePostDataChangedEvent() {
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			DoCallBackPostBack(eventArgument);
			LayoutChanged();
		}
		protected virtual void RegisterInternalCallBacks(Dictionary<string, ASPxGridCallBackFunction> callBacks) {
			callBacks[GridViewCallbackCommand.SelFieldValues] = new ASPxGridCallBackFunction(FBSelectFieldValues);
			callBacks[GridViewCallbackCommand.RowValues] = new ASPxGridCallBackFunction(FBGetRowValues);
			callBacks[GridViewCallbackCommand.PageRowValues] = new ASPxGridCallBackFunction(FBPageRowValues);
			callBacks[GridViewCallbackCommand.FilterPopup] = new ASPxGridCallBackFunction(FBFilterPopup);
			callBacks[GridViewCallbackCommand.CustomValues] = new ASPxGridCallBackFunction(FBCustomValues);
		}
		protected virtual void RegisterCallBacks(Dictionary<string, ASPxGridCallBackMethod> callBacks) {
			callBacks[GridViewCallbackCommand.NextPage] = new ASPxGridCallBackMethod(CBNextPage);
			callBacks[GridViewCallbackCommand.PreviousPage] = new ASPxGridCallBackMethod(CBPrevPage);
			callBacks[GridViewCallbackCommand.GotoPage] = new ASPxGridCallBackMethod(CBGotoPage);
			callBacks[GridViewCallbackCommand.Selection] = new ASPxGridCallBackMethod(CBSelection);
			callBacks[GridViewCallbackCommand.SelectRows] = new ASPxGridCallBackMethod(CBSelectRows);
			callBacks[GridViewCallbackCommand.SelectRowsKey] = new ASPxGridCallBackMethod(CBSelectRowsKey);
			callBacks[GridViewCallbackCommand.FocusedRow] = new ASPxGridCallBackMethod(CBFocusedRow);
			callBacks[GridViewCallbackCommand.Sort] = new ASPxGridCallBackMethod(CBSort);
			callBacks[GridViewCallbackCommand.PagerOnClick] = new ASPxGridCallBackMethod(CBPagerOnClick);
			callBacks[GridViewCallbackCommand.ApplyFilter] = new ASPxGridCallBackMethod(CBApplyFilter);
			callBacks[GridViewCallbackCommand.ApplyHeaderColumnFilter] = new ASPxGridCallBackMethod(CBApplyHeaderColumnFilter);
			callBacks[GridViewCallbackCommand.ApplySearchPanelFilter] = new ASPxGridCallBackMethod(CBApplySearchPanelFilter);
			callBacks[GridViewCallbackCommand.StartEdit] = new ASPxGridCallBackMethod(CBStartEdit);
			callBacks[GridViewCallbackCommand.CancelEdit] = new ASPxGridCallBackMethod(CBCancelEdit);
			callBacks[GridViewCallbackCommand.UpdateEdit] = new ASPxGridCallBackMethod(CBUpdateEdit);
			callBacks[GridViewCallbackCommand.AddNewRow] = new ASPxGridCallBackMethod(CBAddNewRow);
			callBacks[GridViewCallbackCommand.DeleteRow] = new ASPxGridCallBackMethod(CBDeleteRow);
			callBacks[GridViewCallbackCommand.CustomButton] = new ASPxGridCallBackMethod(CBCustomButton);
			callBacks[GridViewCallbackCommand.CustomCallback] = new ASPxGridCallBackMethod(CBCustomCallBack);
			callBacks[GridViewCallbackCommand.ShowFilterControl] = new ASPxGridCallBackMethod(CBShowFilterControl);
			callBacks[GridViewCallbackCommand.CloseFilterControl] = new ASPxGridCallBackMethod(CBCloseFilterControl);
			callBacks[GridViewCallbackCommand.SetFilterEnabled] = new ASPxGridCallBackMethod(CBSetFilterEnabled);
			callBacks[GridViewCallbackCommand.Refresh] = new ASPxGridCallBackMethod(CBRefresh);
		}
		protected object FBSelectFieldValues(string[] args) {
			if (args.Length == 0) return new List<object>();
			string[] fieldNames = args[0].Split(';');
			return GetSelectedFieldValues(fieldNames);
		}
		protected object FBGetRowValues(string[] args) {
			if (args.Length < 2) return null;
			int visibleIndex;
			if (!Int32.TryParse(args[0], out visibleIndex)) return null;
			string[] fieldNames = args[1].Split(';');
			return GetRowValues(visibleIndex, fieldNames);
		}
		protected object FBPageRowValues(string[] args) {
			if (args.Length == 0) return new List<object>();
			string[] fieldNames = args[0].Split(';');
			return GetCurrentPageRowValues(fieldNames);
		}
		protected object FBFilterPopup(string[] args) {
			if (args.Length != 3) return null;
			var column = ColumnHelper.FindColumnByKey(args[1]) as IWebGridDataColumn;
			if (column == null) return null;
			string[] result = new string[2];
			result[0] = args[0];
			DataBindNoControls();
			var container = RenderHelper.CreateHeaderFilterContainer(column, args[2] == "T");
			Controls.Add(container);
			((IASPxWebControl)container).EnsureChildControls();
			result[1] = RenderUtils.GetRenderResult(container);
			return result;
		}
		protected object FBCustomValues(string[] args) {
			var e = CreateCustomDataCallbackEventArgs(string.Join("|", args));
			RaiseCustomDataCallback(e);
			return e.Result;
		}
		protected void CBSelection(string[] args) {
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.Selection);
		}
		protected virtual void CBSelectRows(string[] args) {
			if (args.Length < 1) return;
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.LoadFirstPage();
			string selText = args[0];
			if (selText == "all") {
				Selection.SelectAll();
				return;
			}
			if (selText == "unall") {
				Selection.UnselectAll();
				return;
			}
			if(selText == "unallf") {
				Selection.UnselectAllFiltered();
			}
			DataBindNoControls();
			bool select;
			if (!bool.TryParse(selText, out select)) select = true;
			List<int> selection = new List<int>();
			for (int n = 2; n < args.Length; n++) {
				int i;
				if (!Int32.TryParse(args[n], out i)) continue;
				selection.Add(i);
			}
			for (int n = 0; n < selection.Count; n++) {
				Selection.SetSelection(selection[n], select);
			}
		}
		protected virtual void CBSelectRowsKey(string[] args) {
			if (args.Length < 2) return;
			bool select;
			if (!bool.TryParse(args[0], out select)) select = true;
			List<object> selection = new List<object>();
			for (int n = 1; n < args.Length; n++) {
				object val = null;
				try {
					val = DataProxy.ConvertValue(KeyFieldName, args[n]);
				} catch {
				}
				if (val == null) continue;
				selection.Add(val);
			}
			for (int n = 0; n < selection.Count; n++) {
				Selection.SetSelectionByKey(selection[n], select);
			}
		}
		protected void CBFocusedRow(string[] args) {
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.FocusedRow);
		}
		protected virtual void CBSort(string[] args) {
			var column = ColumnHelper.FindColumnByKey(args[0]) as IWebGridDataColumn;
			if(column == null) return;
			if(DataSourceForceStandardPaging)
				BeginUpdate();
			int sortIndex = args[1] == string.Empty ? -2 : Int32.Parse(args[1]);
			string order = args[2];
			bool reset = GetBoolArg(GetArg(args, 3), true);
			if (SortCount == 1 && column.SortOrder != ColumnSortOrder.None) reset = false;
			if (reset) {
				ResetSortOnly(column);
			}
			if (sortIndex == -1) {
				column.SortIndex = -1;
			} else {
				if (sortIndex != -2)
					SortBy(column, sortIndex);
				column.SortOrder = GetSortOrder(column, order);
			}
			if(DataSourceForceStandardPaging || !DataProxy.IsBound) 
				DataBind();
			if(DataSourceForceStandardPaging)
				EndUpdate();
		}
		protected bool GetBoolArg(string argument, bool defaultValue) {
			bool res = false;
			if (!bool.TryParse(argument, out res)) res = defaultValue;
			return res;
		}
		protected string GetArg(string[] args, int pos) {
			if (pos >= args.Length) return string.Empty;
			return args[pos];
		}
		protected virtual void CBNextPage(string[] args) {
			MovePageOnCallback(PageIndex + 1, false);
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.NextPage);
		}
		protected virtual void CBPrevPage(string[] args) {
			if(args.Length > 0 && args[0] == "T")
				DataProxy.RequireFocusBottomRowOnPageChanged();
			MovePageOnCallback(PageIndex - 1, false);
		}
		protected virtual void CBGotoPage(string[] args) {
			if (args.Length < 1) return;
			int pageIndex;
			if (!Int32.TryParse(args[0], out pageIndex)) return;
			MovePageOnCallback(pageIndex, PagerIsValidPageIndex(-1));
		}
		protected virtual void CBPagerOnClick(string[] args) {
			string command = args[0];
			if(ASPxPagerBase.IsChangePageSizeCommand(command))
				PageSize = ASPxPagerBase.GetNewPageSize(command, PageSize);
			else
				PageIndexInternal = ASPxPagerBase.GetNewPageIndex(command, PageIndex, GetPageCountOnCallback, false);
		}
		protected void MovePageOnCallback(int newIndex, bool allowNegative) {
			int pageCount = GetPageCountOnCallback();
			if (newIndex >= pageCount) newIndex = Math.Max(0, pageCount - 1);
			if (newIndex < -1) newIndex = -1;
			if (!allowNegative && newIndex < 0) newIndex = 0;
			PageIndexInternal = newIndex;
		}
		protected int GetPageCountOnCallback() {
			int pageCount = 0;
			if (IsAllowDataSourcePaging() && DataHelper != null) {
				pageCount = DataProxy.GetPageCount(DataHelper.RetrieveTotalCount());
			} else {
				DataBindNoControls();
				pageCount = DataProxy.PageCount;
			}
			return pageCount;
		}
		protected internal bool PagerIsValidPageIndex(int pageIndex) {
			if(pageIndex == -1) {
				if(SettingsPager.Visible && SettingsPager.Mode == GridViewPagerMode.ShowPager)
					return SettingsPager.AllButton.Visible ||
						(SettingsPager.PageSizeItemSettings.Visible && SettingsPager.PageSizeItemSettings.ShowAllItem);
				return false;
			}
			return true;
		}
		protected internal bool PagerIsValidPageSize(int pageSize) {
			if(pageSize == -1)
				return PagerIsValidPageIndex(-1);
			if(SettingsPager.PageSizeItemSettings.Visible)
				return pageSize == InitialPageSize ||
					   Array.Exists<string>(SettingsPager.PageSizeItemSettings.Items, delegate(string item) { return item == pageSize.ToString(); });
			return false;
		}
		protected virtual void CBApplyFilter(string[] args) {
			FilterExpression = string.Join("|", args);
		}
		protected virtual void CBApplyHeaderColumnFilter(string[] args) {
			var column = ColumnHelper.FindColumnByKey(args[0]) as IWebGridDataColumn;
			if(column == null) return;
			string filter = HttpUtility.HtmlDecode(string.Join("|", args, 1, args.Length - 1));
			FilterByHeaderPopup(column, filter);
		}
		protected virtual void CBApplySearchPanelFilter(string[] args) {
			SearchPanelFilter = args[0];
		}
		protected void CBStartEdit(string[] args) {
			LoadDataIfNotBinded(true);
			object key = DataProxy.GetKeyValueFromScript(args[0]);
			if (key == null) return;
			StartEdit(FindVisibleIndexByKeyValue(key));
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.StartEdit);
		}
		protected virtual void CBUpdateEdit(string[] args) {
			if(IsCallback)
				LayoutChanged(); 
			UpdateEdit();
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.UpdateEdit);
		}
		protected void CBAddNewRow(string[] args) {
			AddNewRow();
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.AddNewRow);
		}
		protected void CBCancelEdit(string[] args) {
			CancelEdit();
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.CancelEdit);
		}
		protected virtual void CBDeleteRow(string[] args) {
			LoadDataIfNotBinded(true);
			object key = DataProxy.GetKeyValueFromScript(args[0]);
			if (key == null) return;
			DeleteRow(FindVisibleIndexByKeyValue(key));
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.DeleteRow, key);
		}
		protected void CBCustomButton(string[] args) {
			if (args.Length != 2) return;
			string id = args[0];
			int visibleIndex;
			if (!Int32.TryParse(args[1], out visibleIndex)) return;
			RaiseCustomButtonCallback(CreateCustomButtonCallbackEventArgs(id, visibleIndex));
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.LoadFirstPage();
		}
		protected void CBCustomCallBack(string[] args) {
			var e = CreateCustomCallbackEventArgs(string.Join("|", args));
			RaiseCustomCallback(e);
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.LoadFirstPage();
		}
		protected void CBShowFilterControl(string[] args) {
			ShowFilterControl();
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.ShowFilterControl);
		}
		protected void CBCloseFilterControl(string[] args) {
			HideFilterControl();
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.ProcessCallback(GridViewCallbackCommand.CloseFilterControl);
		}
		protected virtual void CBSetFilterEnabled(string[] args) {
			if (args.Length < 1) return;
			bool isFilterEnabled;
			if (!bool.TryParse(args[0], out isFilterEnabled)) return;
			FilterEnabled = isFilterEnabled;
		}
		protected void CBRefresh(string[] args) {
			DataBind();
			if(RenderHelper.UseEndlessPaging)
				EndlessPagingHelper.LoadFirstPage();
		}
		protected ColumnSortOrder GetSortOrder(IWebGridDataColumn column, string order) {
			switch(order) {
				case "NONE": return ColumnSortOrder.None;
				case "DSC": return ColumnSortOrder.Descending;
				case "ASC": return ColumnSortOrder.Ascending;
			}
			if(column.SortOrder == ColumnSortOrder.Ascending)
				return ColumnSortOrder.Descending;
			return ColumnSortOrder.Ascending;
		}
		protected delegate void ASPxGridCallBackMethod(string[] args);
		protected internal delegate object ASPxGridCallBackFunction(string[] args);
		#region EVENTS
		protected override IDictionary<string, object> GetCustomJSProperties() {
			var e = CreateCustomJSPropertiesEventArgs(JSPropertiesInternal);
			RaiseGridCustomJSProperties(e);
			if(e.Properties.Count > 0)
				return e.Properties;
			return null;
		}
		protected virtual void CheckPendingEvents() {
			if(PendingEvents.CheckClear(focusedRowChangedPendingFlag)) RaiseFocusedRowChanged();
			if(PendingEvents.CheckClear(selectionChangedPendingFlag)) RaiseSelectionChanged();
		}
		protected virtual void OnRowValidatingCore(ASPxGridDataValidationEventArgs e) {
			RaiseRowValidating(e);
			if(RenderHelper.AllowBatchEditing) {
				BatchEditHelper.LoadValidationErrors(e);
				return;
			}
			RenderHelper.ValidationError.Clear();
			RenderHelper.EditingErrorText = string.Empty;
			if(!string.IsNullOrEmpty(e.RowError))
				RenderHelper.EditingErrorText = EncodeErrorText(RaiseCustomErrorText(CreateCustomErrorTextEventArgs(null, GridErrorTextKind.RowValidate, e.RowError)));
			foreach(KeyValuePair<WebColumnBase, string> pair in e.ErrorsInternal)
				RenderHelper.ValidationError.Add(pair.Key as IWebGridColumn, EncodeErrorText(pair.Value));
		}
		protected override string OnCallbackException(Exception e) {
			return EncodeErrorText(RaiseCustomErrorText(CreateCustomErrorTextEventArgs(e, GridErrorTextKind.General, base.OnCallbackException(e))));
		}
		string EncodeErrorText(string text) {
			if(!SettingsBehavior.EncodeErrorHtml || string.IsNullOrEmpty(text)) return text;
			return System.Web.HttpUtility.HtmlEncode(text);
		}
		protected virtual void OnSummaryExists(CustomSummaryExistEventArgs e) { } 
		void IWebDataEvents.OnStartRowEditing(ASPxStartItemEditingEventArgs e) { RaiseStartEditingRow(e); }
		void IWebDataEvents.OnCancelRowEditing(ASPxStartItemEditingEventArgs e) { RaiseCancelEditingRow(e); }
		void IWebDataEvents.OnFocusedRowChanged() {
			this.fireFocusedRowChangedOnClient = true;
			if(!Loaded)
				PendingEvents.SetPending(focusedRowChangedPendingFlag);
			else
				RaiseFocusedRowChanged();
		}
		void IWebDataEvents.OnParseValue(ASPxParseValueEventArgs e) { RaiseParseValue(e); }
		void IWebDataEvents.OnRowDeleting(ASPxDataDeletingEventArgs e) { RaiseRowDeleting(e); }
		void IWebDataEvents.OnRowDeleted(ASPxDataDeletedEventArgs e) { RaiseRowDeleted(e); }
		void IWebDataEvents.OnRowValidating(ASPxGridDataValidationEventArgs e) {
			OnRowValidatingCore(e);
		}
		void IWebDataEvents.OnInitNewRow(ASPxDataInitNewRowEventArgs e) { RaiseInitNewRow(e); }
		void IWebDataEvents.OnRowInserting(ASPxDataInsertingEventArgs e) { RaiseRowInserting(e); }
		void IWebDataEvents.OnRowInserted(ASPxDataInsertedEventArgs e) { RaiseRowInserted(e); }
		void IWebDataEvents.OnRowUpdating(ASPxDataUpdatingEventArgs e) { RaiseRowUpdating(e); }
		void IWebDataEvents.OnRowUpdated(ASPxDataUpdatedEventArgs e) { RaiseRowUpdated(e); }
		void IWebDataEvents.OnBatchUpdate(ASPxDataBatchUpdateEventArgs e) { RaiseBatchUpdate(e); }
		void IWebDataEvents.OnCustomSummary(CustomSummaryEventArgs e) { RaiseCustomSummaryCalculate(e); }
		object IWebDataEvents.GetUnboundData(int listSourceRowIndex, string fieldName, object value) {
			var e = CreateColumnDataEventArgs(ColumnHelper.FindColumnByString(fieldName) as IWebGridDataColumn, listSourceRowIndex, value, true);
			RaiseCustomUnboundColumnData(e);
			return e.Value;
		}
		void IWebDataEvents.OnSummaryExists(CustomSummaryExistEventArgs e) {
			OnSummaryExists(e);
		}
		void IWebDataEvents.SetUnboundData(int listSourceRowIndex, string fieldName, object value) {
			var e = CreateColumnDataEventArgs(ColumnHelper.FindColumnByString(fieldName) as IWebGridDataColumn, listSourceRowIndex, value, false);
			RaiseCustomUnboundColumnData(e);
		}
		void IWebDataEvents.SubstituteFilter(SubstituteFilterEventArgs e) {
			RaiseSubstituteFilter(e);
		}
		void IWebDataEvents.OnSelectionChanged() {
			this.fireSelectionChangedOnClient = true;
			LayoutChanged();
			if(!Loaded)
				PendingEvents.SetPending(selectionChangedPendingFlag);
			else
				RaiseSelectionChanged();
		}
		void IWebDataEvents.OnDetailRowsChanged() {
			LayoutChanged();
			RaiseDetailRowsChanged();
		}
		protected virtual void RaisePageIndexChanged() {
			var handler = (EventHandler)Events[proxyPageIndexChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected void RaiseFilterControlTryConvertValue(FilterControlParseValueEventArgs e) {
			var handler = (FilterControlParseValueEventHandler)Events[filterControlParseValue];
			if(handler != null) handler(this, e);
		}
		protected void RaiseFilterControlCustomValueDisplayText(FilterControlCustomValueDisplayTextEventArgs e) {
			var handler = (FilterControlCustomValueDisplayTextEventHandler)Events[filterControlCustomValueDisplayText];
			if(handler != null) handler(this, e);
		}
		protected void RaiseFilterControlIsOperationHiddenByUser(FilterControlOperationVisibilityEventArgs e) {
			var handler = (FilterControlOperationVisibilityEventHandler)Events[filterControlOperationVisibility];
			if(handler != null) handler(this, e);
		}
		protected void RaiseFilterControlColumnsCreated(FilterControlColumnsCreatedEventArgs e) {
			FilterControlColumnsCreatedEventHandler handler = (FilterControlColumnsCreatedEventHandler)Events[filterControlColumnsCreated];
			if(handler != null) handler(this, e);
		}
		protected void RaiseFilterControlCriteriaValueEditorInitialize(FilterControlCriteriaValueEditorInitializeEventArgs e) {
			FilterControlCriteriaValueEditorInitializeEventHandler handler = (FilterControlCriteriaValueEditorInitializeEventHandler)Events[filterControlCriteriaValueEditorInitialize];
			if(handler != null) handler(this, e);
		}
		protected void RaiseFilterControlCriteriaValueEditorCreate(FilterControlCriteriaValueEditorCreateEventArgs e) {
			FilterControlCriteriaValueEditorCreateEventHandler handler = (FilterControlCriteriaValueEditorCreateEventHandler)Events[filterControlCriteriaValueEditorCreate];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseBeforePerformDataSelect() { }
		protected internal virtual void RaiseCustomColumnGroup(GridCustomColumnSortEventArgs e) { }
		protected virtual void RaiseDetailRowsChanged() { }
		protected virtual void RaiseBatchUpdate(ASPxDataBatchUpdateEventArgs e) { }
		protected virtual void RaiseCustomSummaryCalculate(CustomSummaryEventArgs e) { }
		protected internal abstract void RaiseCustomColumnDisplayText(ASPxGridColumnDisplayTextEventArgs e);
		protected abstract void RaiseCustomUnboundColumnData(ASPxGridColumnDataEventArgs e);
		protected internal abstract void RaiseBeforeHeaderFilterFillItems(ASPxGridBeforeHeaderFilterFillItemsEventArgs e);
		protected internal abstract void RaiseHeaderFilterFillItems(ASPxGridHeaderFilterEventArgs e);
		protected internal abstract void RaiseCommandButtonInitialize(ASPxGridCommandButtonEventArgs e);
		protected internal abstract void RaiseCustomButtonInitialize(ASPxGridCustomCommandButtonEventArgs e);
		protected internal abstract void RaiseEditorInitialize(ASPxGridEditorEventArgs e);
		protected internal abstract void RaiseSearchPanelEditorCreate(ASPxGridEditorCreateEventArgs e);
		protected internal abstract void RaiseSearchPanelEditorInitialize(ASPxGridEditorEventArgs e);
		protected internal abstract void RaiseBeforeColumnSortingGrouping(ASPxGridBeforeColumnGroupingSortingEventArgs e);
		protected abstract void RaiseCustomCallback(ASPxGridCustomCallbackEventArgs e);
		protected abstract void RaiseCustomDataCallback(ASPxGridCustomCallbackEventArgs e);
		protected abstract void RaiseCustomButtonCallback(ASPxGridCustomButtonCallbackEventArgs e);
		protected abstract void RaiseAfterPerformCallback(ASPxGridAfterPerformCallbackEventArgs e);
		protected internal abstract void RaiseCustomColumnSort(GridCustomColumnSortEventArgs e);
		protected abstract void RaiseGridCustomJSProperties(CustomJSPropertiesEventArgs e);
		protected internal abstract string RaiseCustomErrorText(ASPxGridCustomErrorTextEventArgs e);
		protected internal abstract string RaiseSummaryDisplayText(ASPxGridSummaryDisplayTextEventArgs e);
		protected abstract void RaiseSubstituteFilter(SubstituteFilterEventArgs e);
		protected abstract void RaiseSubstituteSortInfo(SubstituteSortInfoEventArgs e);
		protected abstract void RaisePageSizeChanged();
		protected abstract void RaiseSelectionChanged();
		protected abstract void RaiseFocusedRowChanged();
		protected abstract void RaiseStartEditingRow(ASPxStartItemEditingEventArgs e);
		protected abstract void RaiseCancelEditingRow(ASPxStartItemEditingEventArgs e);
		protected abstract void RaiseFilterControlCustomFilterExpressionDisplayText(CustomFilterExpressionDisplayTextEventArgs e);
		protected abstract void RaiseRowInserting(ASPxDataInsertingEventArgs e);
		protected abstract void RaiseRowInserted(ASPxDataInsertedEventArgs e);
		protected abstract void RaiseRowUpdating(ASPxDataUpdatingEventArgs e);
		protected abstract void RaiseRowUpdated(ASPxDataUpdatedEventArgs e);
		protected abstract void RaiseRowDeleting(ASPxDataDeletingEventArgs e);
		protected abstract void RaiseRowDeleted(ASPxDataDeletedEventArgs e);
		protected abstract void RaiseParseValue(ASPxParseValueEventArgs e);
		protected abstract void RaiseInitNewRow(ASPxDataInitNewRowEventArgs e);
		protected abstract void RaiseRowValidating(ASPxGridDataValidationEventArgs e);
		protected internal abstract ASPxGridColumnDisplayTextEventArgs CreateColumnDisplayTextEventArgs(IWebGridDataColumn column, int visibleIndex, IValueProvider provider, object value);
		protected abstract ASPxGridColumnDataEventArgs CreateColumnDataEventArgs(IWebGridDataColumn column, int listSourceRowIndex, object value, bool isGetAction);
		protected internal abstract ASPxGridBeforeHeaderFilterFillItemsEventArgs CreateBeforeHeaderFilterFillItemsEventArgs(IWebGridDataColumn column);
		protected internal abstract ASPxGridHeaderFilterEventArgs CreateHeaderFilterFillItemsEventArgs(IWebGridDataColumn column, GridHeaderFilterValues values);
		protected internal abstract ASPxGridEditorEventArgs CreateCellEditorInitializeEventArgs(IWebGridDataColumn column, int visibleIndex, ASPxEditBase editor, object keyValue, object value);
		protected internal abstract ASPxGridEditorCreateEventArgs CreateSearchPanelEditorCreateEventArgs(EditPropertiesBase editorProperties, object value);
		protected internal abstract ASPxGridEditorEventArgs CreateSearchPanelEditorInitializeEventArgs(ASPxEditBase editor, object value);
		protected internal abstract ASPxGridBeforeColumnGroupingSortingEventArgs CreateBeforeColumnSortingGroupingEventArgs(IWebGridDataColumn column, ColumnSortOrder sortOrder, int sortIndex, int groupIndex);
		protected internal abstract ASPxGridCustomCallbackEventArgs CreateCustomCallbackEventArgs(string parameters);
		protected internal abstract ASPxGridCustomCallbackEventArgs CreateCustomDataCallbackEventArgs(string parameters);
		protected internal abstract ASPxGridCustomButtonCallbackEventArgs CreateCustomButtonCallbackEventArgs(string buttonID, int visibleIndex);
		protected internal abstract ASPxGridAfterPerformCallbackEventArgs CreateAfterPerformCallbackEventArgs(string callbackName, string[] args);
		protected internal abstract GridCustomColumnSortEventArgs CreateCustomColumnSortEventArgs(IWebGridDataColumn column, object value1, object value2, ColumnSortOrder sortOrder);
		protected internal abstract CustomJSPropertiesEventArgs CreateCustomJSPropertiesEventArgs(Dictionary<string, object> properties);
		protected internal abstract ASPxGridCustomErrorTextEventArgs CreateCustomErrorTextEventArgs(Exception exception, GridErrorTextKind errorTextKind, string errorText);
		protected internal abstract ASPxStartItemEditingEventArgs CreateStartEditingEventArgs(object editingKeyValue);
		protected internal abstract ASPxGridDataValidationEventArgs CreateItemValidatingEventArgs(int visibleIndex, bool isNew);
		#endregion
		protected internal void OnColumnBindingChanged() {
			if (IsLoading()) return;
			LayoutChanged();
			DataProxy.UpdateColumnBindings();
		}
		protected void LoadDataIfNotBinded() {
			LoadDataIfNotBinded(false);
		}
		protected void LoadDataIfNotBinded(bool bindIfNotCached) {
			if (EnableRowsCache) {
				if (!DataProxy.HasCachedProvider) {
					FormatConditions.LoadCacheState(CallbackState.Get<byte[]>("FormatState"));
					byte[] state = CallbackState.Get<byte[]>("Data");
					if (state != null && state.Length > 0) {
						DataProxy.LoadCachedData(state);
						if (DataProxy.IsReady) return;
						DataProxy.SetCachedDataProvider();
					}
				}
			}
			if (DataProxy.IsReady) return;
			if (bindIfNotCached) DataBindNoControls();
		}
		bool lockDataBindNoControls = false; 
		protected internal void DataBindNoControls() {
			if (!DataProxy.IsBound && !this.lockDataBindNoControls) {
				this.lockDataBindNoControls = true;
				try {
					PerformSelect();
				} finally {
					this.lockDataBindNoControls = false;
				}
			}
		}
		bool needSyncrhonizeDataProxy = false;
		protected void CheckBindAndSynchronizeDataProxy() {
			if (needSyncrhonizeDataProxy) BindAndSynchronizeDataProxy();
		}
		protected void BindAndSynchronizeDataProxy() {
			this.needSyncrhonizeDataProxy = true;
			if (IsLockUpdate) return;
			this.needSyncrhonizeDataProxy = false;
			if (!DataProxy.IsBound) {
				RequireDataBinding();
				DataBindNoControls();
			} else {
				SynchronizeDataProxy();
			}
			DataProxy.CheckFocusedRowChanged();
			LayoutChanged();
		}
		bool hierarchyChanged = false;
		bool lockSyncDataProxy = false;
		void SynchronizeDataProxy() {
			if(this.lockSyncDataProxy)
				return;
			this.lockSyncDataProxy = true;
			try {
				this.hierarchyChanged = false;
				List<IWebColumnInfo> sortList = new List<IWebColumnInfo>();
				foreach(IWebColumnInfo column in SortedColumns) {
					sortList.Add(column);
				}
				DataProxy.SortGroupChanged(sortList, GroupCountInternal, FilterHelper.ActiveFilter);
				if(!string.IsNullOrEmpty(SearchPanelFilter))
					SearchPanelColumnInfoKey = FilterHelper.GetSearchPanelColumnInfoKey();
				DataProxy.RestoreRowsState();
				if(IsFirstLoad && AutoExpandAllGroupsInternal)
					ExpandAllInternal();
				if(this.hierarchyChanged)
					ResetControlHierarchy();
				DataProxy.Selection.UpdateCachedParameters();
				FormatConditions.ResetCache();
			} finally {
				this.lockSyncDataProxy = false;
			}
		}
		protected void EnsureSearchPanelFilterActual() {
			if(!string.IsNullOrEmpty(SearchPanelFilter) && SearchPanelColumnInfoKey != FilterHelper.GetSearchPanelColumnInfoKey())
				BindAndSynchronizeDataProxy();
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new GridDataHelper(this, name);
		}
		public override void DataBind() {
			foreach(var column in ColumnHelper.AllDataColumns) {
				if(column.PropertiesEdit != null)
					column.PropertiesEdit.RequireDataBinding();
			}
			base.DataBind();
		}
		protected internal override void PerformDataBinding(string dataHelperName, System.Collections.IEnumerable data) {
			int savedPageIndex = PageIndex;
			DataProxy.ClearStoredPageSelectionResult(); 
			PopulateAutoGeneratedColumns(data);
			DataProxy.SetDataSource(data);
			ResetControlHierarchy();
			if(IsAllowDataSourcePaging() && PageIndex < savedPageIndex)
				PerformSelect();
		}
		protected override void OnDataBound(EventArgs e) {
			BuildSortedColumns();
			SynchronizeDataProxy();
			base.OnDataBound(e);
		}
		protected virtual void PopulateAutoGeneratedColumns(System.Collections.IEnumerable data) {
			if(!AutoGenerateColumns || Columns.Count > 0) return;
			if(data == null) return;
			var columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(null, data, null, false);
			if(columns == null) return;
			foreach(DataColumnInfo column in columns) {
				if(!CanPopulateAutoGeneratedColumn(column)) continue;
				var dataCol = CreateEditColumn(column.Type);
				dataCol.Adapter.AutoGenerated = true;
				dataCol.FieldName = column.Name;
				dataCol.ReadOnly = column.ReadOnly;
				Columns.Add(dataCol);
			}
		}
		protected virtual bool CanPopulateAutoGeneratedColumn(DataColumnInfo column) {
			return column.Browsable; 
		}
		protected abstract IWebGridDataColumn CreateEditColumn(Type dataType);
		internal void EnsureChildControlsCore() {
			EnsureChildControls();
		}
		protected abstract GridColumnCollection CreateColumnCollection();
		internal T GetCachedStyle<T>(CreateStyleHandler creator, params object[] keys) where T : AppearanceStyleBase {
			return (T)CreateStyle(creator, keys);
		}
		internal void BeginExport() {
			if(DataBoundProxy.PrepareDataSourceForExport())
				SynchronizeDataProxy();
			IsExported = true;
		}
		internal void EndExport() {
			if(DataBoundProxy.RestoreDataSourceAfterExport())
				SynchronizeDataProxy();
			IsExported = false;
		}
		#region IDataControllerSort Members
		string[] IDataControllerSort.GetFindByPropertyNames() {
			return ColumnHelper.SearchPanelColumnInfos.Select(c => c.Name).ToArray();
		}
		void IDataControllerSort.AfterGrouping() {
		}
		void IDataControllerSort.AfterSorting() {
		}
		void IDataControllerSort.BeforeGrouping() {
		}
		void IDataControllerSort.BeforeSorting() {
			SortData.OnStart();
		}
		string IDataControllerSort.GetDisplayText(int listSourceRow, DataColumnInfo info, object value, string columnName) {
			if(columnName.StartsWith(DxFtsContainsHelper.DxFtsPropertyPrefix) && SearchFilterData != null)
				return SearchFilterData.GetDisplayText(listSourceRow, info, value);
			return FilterData.GetDisplayText(listSourceRow, info, value);
		}
		bool IDataControllerSort.RequireDisplayText(DataColumnInfo column) {
			return FilterData.IsRequired(column);
		}
		bool? IDataControllerSort.IsEqualGroupValues(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo column) {
			if(!SortData.IsRequired(column))
				return null;
			BaseGridColumnInfo info = SortData.GetInfo(column);
			if(info == null)
				return null;
			var cmp = info.CompareGroupValues(listSourceRow1, listSourceRow2, value1, value2);
			if(cmp.HasValue)
				return cmp.Value == 0;
			return null;
		}
		ExpressiveSortInfo.Row IDataControllerSort.GetCompareRowsMethodInfo() {
			return null;
		}
		ExpressiveSortInfo.Cell IDataControllerSort.GetSortCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType, ColumnSortOrder order) {
			if(!SortData.IsRequired(dataColumnInfo))
				return null;
			var info = SortData.GetInfo(dataColumnInfo);
			if(info == null)
				return null;
			return info.GetCompareSortValuesInfo(baseExtractorType, order);
		}
		ExpressiveSortInfo.Cell IDataControllerSort.GetSortGroupCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType) {
			if(!SortData.IsRequired(dataColumnInfo))
				return null;
			var info = SortData.GetInfo(dataColumnInfo);
			if(info == null)
				return null;
			return info.GetCompareGroupValuesInfo(baseExtractorType);
		}
		void IDataControllerSort.SubstituteSortInfo(SubstituteSortInfoEventArgs args) {
			RaiseSubstituteSortInfo(args);
		}
		#endregion
		#region IWebColumnsOwner implementation
		WebColumnCollectionBase IWebColumnsOwner.Columns { get { return WebColumnsOwnerImpl.Columns; } }
		void IWebColumnsOwner.ResetVisibleColumns() { WebColumnsOwnerImpl.ResetVisibleColumns(); }
		void IWebColumnsOwner.ResetVisibleIndices() { WebColumnsOwnerImpl.ResetVisibleIndices(); }
		List<WebColumnBase> IWebColumnsOwner.GetVisibleColumns() { return WebColumnsOwnerImpl.GetVisibleColumns(); }
		void IWebColumnsOwner.SetColumnVisible(WebColumnBase column, bool value) { WebColumnsOwnerImpl.SetColumnVisible(column, value); }
		void IWebColumnsOwner.SetColumnVisibleIndex(WebColumnBase column, int value) { WebColumnsOwnerImpl.SetColumnVisibleIndex(column, value); }
		void IWebColumnsOwner.EnsureVisibleIndices() { WebColumnsOwnerImpl.EnsureVisibleIndices(); }
		void IWebColumnsOwner.OnColumnChanged(WebColumnBase column) { 
			WebColumnsOwnerImpl.OnColumnChanged(column);
			ColumnHelper.Invalidate();
		}
		void IWebColumnsOwner.OnColumnCollectionChanged() { 
			WebColumnsOwnerImpl.OnColumnCollectionChanged();
			ColumnHelper.Invalidate();
		}
		#endregion
		#region IPopupFilterControlOwner Members
		bool IPopupFilterControlOwner.EnableCallBacks { get {return EnableCallBacks; } }
		void IPopupFilterControlOwner.CloseFilterControl() { this.HideFilterControl(); }
		object IPopupFilterControlOwner.GetControlCallbackResult() {
			this.addCustomJSPropertiesScript = true;
			object result = GetFilterControlCallbackResult();
			SaveClientStateInternal();
			return result;
		}
		protected virtual object GetFilterControlCallbackResult() {
			return GetCallbackResult();
		}
		string IPopupFilterControlOwner.MainElementID { get { return ID; } }
		ASPxWebControl IPopupFilterControlOwner.OwnerControl { get { return this; } }
		string IPopupFilterControlOwner.GetJavaScriptForApplyFilterControl() { return RenderHelper.Scripts.GetApplyFilterControl(); }
		string IPopupFilterControlOwner.GetJavaScriptForCloseFilterControl() { return RenderHelper.Scripts.GetCloseFilterControl(); }
		FilterControlImages IPopupFilterControlOwner.GetImages() { return ImagesFilterControl; }
		EditorImages IPopupFilterControlOwner.GetImagesEditors() { return ImagesEditors; }
		FilterControlStyles IPopupFilterControlOwner.GetStyles() { return StylesFilterControl; }
		EditorStyles IPopupFilterControlOwner.GetStylesEditors() { return StylesEditors; }
		string IPopupFilterControlOwner.FilterPopupHeaderText { get { return SettingsText.FilterControlPopupCaption; } }
		bool IPopupFilterControlOwner.EnablePopupMenuScrolling { get { return Settings.EnableFilterControlPopupMenuScrolling; } }
		SettingsLoadingPanel IPopupFilterControlOwner.SettingsLoadingPanel { get { return SettingsLoadingPanel; } }
		#endregion
		#region IFilterControlOwner Members
		string IFilterControlOwner.FilterExpression { get { return FilterExpression; } set { FilterExpression = value; } }
		bool IFilterControlOwner.IsRightToLeft { get { return IsRightToLeft(); } }
		bool IFilterControlOwner.TryGetSpecialValueDisplayText(IFilterColumn column, object value, bool encodeValue, out string displayText) {
			var dataColumn = (IWebGridDataColumn)column;
			if(dataColumn.Adapter.FilterMode == ColumnFilterMode.DisplayText) {
				displayText = String.Concat(value);
				if(encodeValue)
					displayText = System.Web.HttpUtility.HtmlEncode(displayText);
			} else {
				displayText = RenderHelper.TextBuilder.GetFilterControlItemText(dataColumn, encodeValue, value);
			}
			return true;
		}
		bool IFilterControlOwner.IsOperationHiddenByUser(IFilterablePropertyInfo propertyInfo, DevExpress.Data.Filtering.Helpers.ClauseType operation) {
			var e = new FilterControlOperationVisibilityEventArgs(propertyInfo, operation);
			RaiseFilterControlIsOperationHiddenByUser(e);
			return !e.Visible;
		}
		bool IFilterControlOwner.TryConvertValue(IFilterablePropertyInfo propertyInfo, string text, out object value) {
			value = null;
			var e = new FilterControlParseValueEventArgs(propertyInfo, text);
			RaiseFilterControlTryConvertValue(e);
			if(e.Handled)
				value = e.Value;
			return e.Handled;
		}
		FilterControlViewMode IFilterControlOwner.ViewMode { get { return SettingsFilterControl.ViewMode; } }
		bool IFilterControlOwner.ShowOperandTypeButton { get { return SettingsFilterControl.ShowOperandTypeButton; } }
		FilterControlGroupOperationsVisibility IFilterControlOwner.GroupOperationsVisibility { get { return SettingsFilterControl.GroupOperationsVisibility; } }
		FilterControlColumnCollection IFilterControlOwner.GetFilterColumns() {
			return ColumnHelper.FilterControlCachedColumns;
		}
		void IFilterControlOwner.RaiseCustomValueDisplayText(FilterControlCustomValueDisplayTextEventArgs e) {
			RaiseFilterControlCustomValueDisplayText(e);
		}
		void IFilterControlOwner.RaiseCriteriaValueEditorInitialize(FilterControlCriteriaValueEditorInitializeEventArgs e) {
			RaiseFilterControlCriteriaValueEditorInitialize(e);
		}
		void IFilterControlOwner.RaiseCriteriaValueEditorCreate(FilterControlCriteriaValueEditorCreateEventArgs e) {
			RaiseFilterControlCriteriaValueEditorCreate(e);
		}
		#endregion
		#region IFilterControlRowOwner Members
		string IFilterControlRowOwner.GetJavaScriptForClearFilter() { return RenderHelper.Scripts.GetClearFilterFunction(); }
		string IFilterControlRowOwner.GetJavaScriptForShowFilterControl() { return RenderHelper.Scripts.GetShowFilterControl(); }
		string IFilterControlRowOwner.GetJavaScriptForSetFilterEnabledForCheckbox() { return RenderHelper.Scripts.GetSetFilterEnabledForCheckBox(); }
		bool IFilterControlRowOwner.IsFilterEnabledSupported { get { return true; } }
		bool IFilterControlRowOwner.IsFilterEnabled { get { return FilterEnabled; } }
		void IFilterControlRowOwner.AppendDefaultDXClassName(WebControl control) {
			RenderHelper.AppendGridCssClassName(control);
		}
		void IFilterControlRowOwner.AssignFilterStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignLinkStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarLinkStyle().AssignToControl(control);
		}
		void IFilterControlRowOwner.AssignCheckBoxCellStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarCheckBoxCellStyle().AssignToControl(control, true);
		}
		void IFilterControlRowOwner.AssignImageCellStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarImageCellStyle().AssignToControl(control, true);
		}
		void IFilterControlRowOwner.AssignExpressionCellStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarExpressionCellStyle().AssignToControl(control, true);
		}
		void IFilterControlRowOwner.AssignClearButtonCellStyleToControl(WebControl control) {
			RenderHelper.GetFilterBarClearButtonCellStyle().AssignToControl(control, true);
		}
		ImageProperties IFilterControlRowOwner.CreateFilterImage { get { return RenderHelper.GetFilterBarButtonImage(); } }
		string IFilterControlRowOwner.ClearButtonText { get { return SettingsText.FilterBarClear; } }
		string IFilterControlRowOwner.ShowFilterBuilderText { get { return SettingsText.FilterBarCreateFilter; } }
		void IFilterControlRowOwner.RaiseCustomFilterExpressionDisplayText(CustomFilterExpressionDisplayTextEventArgs e) {
			RaiseFilterControlCustomFilterExpressionDisplayText(e);
		}
		#endregion
		#region IPopupFilterControlStyleOwner Members
		ImageProperties IPopupFilterControlStyleOwner.CloseButtonImage { get { return Images.FilterBuilderClose; } }
		AppearanceStyle IPopupFilterControlStyleOwner.CloseButtonStyle { get { return RenderHelper.GetFilterBuilderPopupCloseButtonStyle(); } }
		AppearanceStyle IPopupFilterControlStyleOwner.HeaderStyle { get { return RenderHelper.GetFilterBuilderPopupHeaderStyle(); } }
		AppearanceStyle IPopupFilterControlStyleOwner.MainAreaStyle { get { return RenderHelper.GetFilterBuilderMainAreaStyle(); } }
		AppearanceStyle IPopupFilterControlStyleOwner.ButtonAreaStyle { get { return RenderHelper.GetFilterBuilderButtonAreaStyle(); } }
		AppearanceStyleBase IPopupFilterControlStyleOwner.ModalBackgroundStyle { get { return RenderHelper.GetFilterBuilderPopupModalBackgroundStyle(); } }
		#endregion
		#region IHeaderFilterPopupOwner Members
		bool IHeaderFilterPopupOwner.ShowButtonPanel { get { return RenderHelper.ShowHeaderFilterButtonPanel; } }
		Unit IHeaderFilterPopupOwner.PopupWidth { get { return RenderHelper.GetHeaderFilterPopupWidth(); } }
		Unit IHeaderFilterPopupOwner.PopupHeight { get { return RenderHelper.GetHeaderFilterPopupHeight(); } }
		Unit IHeaderFilterPopupOwner.PopupMinHeight { get { return RenderHelper.GetHeaderFilterPopupMinHeight(); } }
		Unit IHeaderFilterPopupOwner.PopupMinWidth { get { return RenderHelper.GetHeaderFilterPopupMinWidth(); } }
		ResizingMode IHeaderFilterPopupOwner.PopupResizeMode { get { return RenderHelper.GetHeaderFilterPopupResizeMode(); } }
		bool IHeaderFilterPopupOwner.PopupCloseOnEscape { get { return RenderHelper.GetHeaderFilterPopupCloseOnEscape(); } }
		AppearanceStyleBase IHeaderFilterPopupOwner.ControlStyle { get { return RenderHelper.GetHeaderFilterPopupControlStyle(); } }
		AppearanceStyleBase IHeaderFilterPopupOwner.ContentStyle { get { return RenderHelper.GetHeaderFilterPopupContentStyle(); } }
		AppearanceStyleBase IHeaderFilterPopupOwner.FooterStyle { get { return RenderHelper.GetHeaderFilterPopupFooterStyle(); } }
		ImageProperties IHeaderFilterPopupOwner.SizeGrip { get { return RenderHelper.GetHeaderFilterPopupSizeGripImage(); } }
		ImageProperties IHeaderFilterPopupOwner.SizeGripRtl { get { return RenderHelper.GetHeaderFilterPopupSizeGripRtlImage(); } }
		string IHeaderFilterPopupOwner.OkButtonText { get { return SettingsText.GetHeaderFilterOkButton(); } }
		string IHeaderFilterPopupOwner.CancelButtonText { get { return SettingsText.GetHeaderFilterCancelButton(); } }
		string IHeaderFilterPopupOwner.OkButtonClickScript { get { return RenderHelper.Scripts.GetHFOkButtonClickHandler(); } }
		string IHeaderFilterPopupOwner.CancelButtonClickScript { get { return RenderHelper.Scripts.GetHFCancelButtonClickHandler(); } }
		HeaderFilterButtonPanelStyles IHeaderFilterPopupOwner.ButtonPanelStyles { get { return StylesPopup.HeaderFilter.ButtonPanel; } }
		#endregion
		int IPagerOwner.InitialPageSize { get { return InitialPageSize; } }
		string IControlDesigner.DesignerType { get { return GetControlDesignerType(); } }
		string[] IFormLayoutOwner.GetColumnNames() {
			List<string> result = new List<string>();
			foreach(IWebGridColumn column in ColumnHelper.AllDataColumns)
				result.Add(column.ToString());
			return result.ToArray();
		}
		object IFormLayoutOwner.FindColumnByName(string columnName) { return FindColumnByName(columnName); }
		FormLayoutProperties IFormLayoutOwner.GenerateDefaultLayout(bool fromControlDesigner) { return GenerateDefaultLayout(fromControlDesigner); }
		protected internal abstract FormLayoutProperties GenerateDefaultLayout(bool fromControlDesigner);
		protected internal abstract IWebGridColumn FindColumnByName(string columnName);
		protected virtual string GetControlDesignerType() {
			return "DevExpress.Web.Design.GridViewCommonFormDesigner";
		}
	}
	public class WebColumnsOwnerGridViewImplementation : WebColumnsOwnerDefaultImplementation {
		public WebColumnsOwnerGridViewImplementation(IWebControlObject control, WebColumnCollectionBase columns) 
			: base(control, columns) { 
		}
		protected override int CompareColumnsByVisibleIndex(WebColumnBase col1, WebColumnBase col2) {
			GridViewColumn column1 = (GridViewColumn)col1;
			GridViewColumn column2 = (GridViewColumn)col2;
			var isRootLevel = column1.ParentBand == null;
			if(isRootLevel && column1.FixedStyle != column2.FixedStyle)
				return column1.FixedStyle == GridViewColumnFixedStyle.Left ? -1 : 1;
			return base.CompareColumnsByVisibleIndex(col1, col2);
		}
	}
	public class GridViewCallbackCommand {
		public const string NextPage = "NEXTPAGE";
		public const string PreviousPage = "PREVPAGE";
		public const string GotoPage = "GOTOPAGE";
		public const string Selection = "SELECTION";
		public const string SelectRows = "SELECTROWS";
		public const string SelectRowsKey = "SELECTROWSKEY";
		public const string FocusedRow = "FOCUSEDROW";
		public const string Group = "GROUP";
		public const string UnGroup = "UNGROUP";
		public const string Sort = "SORT";
		public const string ColumnMove = "COLUMNMOVE";
		public const string CollapseAll = "COLLAPSEALL";
		public const string ExpandAll = "EXPANDALL";
		public const string ExpandRow = "EXPANDROW";
		public const string CollapseRow = "COLLAPSEROW";
		public const string HideAllDetail = "HIDEALLDETAIL";
		public const string ShowAllDetail = "SHOWALLDETAIL";
		public const string ShowDetailRow = "SHOWDETAILROW";
		public const string HideDetailRow = "HIDEDETAILROW";
		public const string PagerOnClick = "PAGERONCLICK";
		public const string ApplyFilter = "APPLYFILTER";
		public const string ApplyColumnFilter = "APPLYCOLUMNFILTER";
		public const string ApplyMultiColumnFilter = "APPLYMULTICOLUMNFILTER";
		public const string ApplyHeaderColumnFilter = "APPLYHEADERCOLUMNFILTER";
		public const string ApplySearchPanelFilter = "APPLYSEARCHPANELFILTER";
		public const string FilterRowMenu = "FILTERROWMENU";
		public const string StartEdit = "STARTEDIT";
		public const string CancelEdit = "CANCELEDIT";
		public const string UpdateEdit = "UPDATEEDIT";
		public const string AddNewRow = "ADDNEWROW";
		public const string DeleteRow = "DELETEROW";
		public const string CustomButton = "CUSTOMBUTTON";
		public const string CustomCallback = "CUSTOMCALLBACK";
		public const string ShowFilterControl = "SHOWFILTERCONTROL";
		public const string CloseFilterControl = "CLOSEFILTERCONTROL";
		public const string SetFilterEnabled = "SETFILTERENABLED";
		public const string Refresh = "REFRESH";
		public const string FilterPopup = "FILTERPOPUP";
		public const string RowValues = "ROWVALUES";
		public const string SelFieldValues = "SELFIELDVALUES";
		public const string PageRowValues = "PAGEROWVALUES";
		public const string CustomValues = "CUSTOMVALUES";
		public const string ContextMenu = "CONTEXTMENU";
	}
}

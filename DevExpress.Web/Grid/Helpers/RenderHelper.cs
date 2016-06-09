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
using DevExpress.Web.Cookies;
using DevExpress.Web.Data;
using DevExpress.Web.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
using DevExpress.Web.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public enum GridCommandButtonType { New, Edit, Delete, Update, Cancel, Select, SelectCheckbox, 
		ApplyFilter, ClearFilter, ApplySearchPanelFilter, ClearSearchPanelFilter, EndlessShowMoreCards, ShowAdaptiveDetail, HideAdaptiveDetail }
	public abstract class GridTextBuilder {
		public GridTextBuilder(ASPxGridBase grid) {
			Grid = grid;
		}
		protected ASPxGridBase Grid { get; private set; }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected delegate string GetEditorDisplayTextCoreFunc(EditPropertiesBase editor, IWebGridDataColumn column, int visibleIndex, object value, bool encodeValue);
		public virtual string GetHeaderCaption(IWebGridColumn column) {
			return column.ToString();
		}
		public string GetRowDisplayText(IWebGridDataColumn column, int visibleIndex) {
			return GetRowDisplayTextCore(column, visibleIndex, DataProxy.GetRowValue(visibleIndex, column.FieldName));
		}
		protected virtual string GetRowDisplayTextCore(IWebGridDataColumn column, int visibleIndex, object value) {
			return GetDisplayTextCore(column, visibleIndex, value, GetEditorDisplayTextCore);
		}
		protected virtual string GetDisplayTextCore(IWebGridDataColumn column, int visibleIndex, object value, GetEditorDisplayTextCoreFunc func) {
			return GetDisplayTextCore(column, visibleIndex, value, true, func);
		}
		protected virtual string GetDisplayTextCore(IWebGridDataColumn column, int visibleIndex, object value, bool encodeValue, GetEditorDisplayTextCoreFunc func) {
			EditPropertiesBase editor = Grid.RenderHelper.GetColumnEdit(column);
			string strValue = string.Empty;
			if(editor != null)
				strValue = func(editor, column, visibleIndex, value, encodeValue);
			else {
				if(value == null || value == DBNull.Value)
					strValue = string.Empty;
				else
					strValue = value.ToString();
			}
			return strValue;
		}
		protected virtual string GetEditorDisplayTextCore(EditPropertiesBase editor, IWebGridDataColumn column, int visibleIndex, object value, bool encodeValue) {
			return editor.GetDisplayText(GetDisplayControlArgsCore(column, visibleIndex, value));
		}
		public CreateDisplayControlArgs GetDisplayControlArgs(IWebGridDataColumn column, int visibleIndex, IValueProvider provider, ASPxGridBase grid) {
			return GetDisplayControlArgs(column, visibleIndex, provider, grid, false);
		}
		public CreateDisplayControlArgs GetDisplayControlArgs(IWebGridDataColumn column, int visibleIndex, IValueProvider provider, ASPxGridBase grid, bool highlightSearchText) {
			var args = GetDisplayControlArgsCore(column, visibleIndex, provider, grid, DataProxy.GetRowValue(visibleIndex, column.FieldName));
			if(highlightSearchText && RenderHelper.RequireHighlightSearchText(column) && !Grid.IsExported)
				args.HighlightTextProcessor = RenderHelper.GetHighlightTextProcessor(column);
			return args;
		}
		protected CreateDisplayControlArgs GetDisplayControlArgsCore(IWebGridDataColumn column, int visibleIndex, object value) {
			return GetDisplayControlArgsCore(column, visibleIndex, new SimpleValueProvider(DataProxy, visibleIndex), Grid, value);
		}
		protected internal virtual CreateDisplayControlArgs GetDisplayControlArgsCore(IWebGridDataColumn column, int visibleIndex, IValueProvider provider, ASPxGridBase grid, object value) {
			var args = Grid.CreateColumnDisplayTextEventArgs(column, visibleIndex, provider, value);
			Grid.RaiseCustomColumnDisplayText(args);
			return new CreateDisplayControlArgs(value, column.Adapter.DataType, args.DisplayText, provider, grid.ImagesEditors, grid.StylesEditors, grid, grid.DummyNamingContainer, grid.DesignMode, args.EncodeHtml);
		}
		public virtual string GetFilterPopupItemText(IWebGridDataColumn column, object value) {
			return GetDisplayTextCore(column, -1, value, GetEditorFilterItemTextCore);
		}
		public virtual string GetFilterControlItemText(IWebGridDataColumn column, object value) {
			return GetDisplayTextCore(column, -1, value, GetEditorFilterItemTextCore);
		}
		public virtual string GetFilterControlItemText(IWebGridDataColumn column, bool encodeValue, object value) {
			return GetDisplayTextCore(column, -1, value, encodeValue, GetEditorFilterItemTextCore);
		}
		protected virtual string GetEditorFilterItemTextCore(EditPropertiesBase editor, IWebGridDataColumn column, int visibleIndex, object value, bool encodeValue) {
			var args = new CreateDisplayControlArgs(value, column.Adapter.DataType, null, null, Grid.ImagesEditors, Grid.StylesEditors, Grid.DummyNamingContainer, Grid.DesignMode);
			return editor.GetDisplayText(args, editor.EncodeHtml & encodeValue);
		}
		public HorizontalAlign GetColumnDisplayControlDefaultAlignment(IWebGridColumn column) {
			return RenderHelper.GetColumnDisplayControlDefaultAlignment(column);
		}
		public virtual string GetSummaryItemText(ASPxSummaryItemBase summaryItem) {
			object value = Grid.GetTotalSummaryValue(summaryItem);
			string text = summaryItem.GetSummaryDisplayText(Grid.ColumnHelper.FindColumnByString(summaryItem.FieldName), value);
			return Grid.RaiseSummaryDisplayText(GetSummaryDisplayTextEventArgs(summaryItem, value, text));
		}
		protected abstract ASPxGridSummaryDisplayTextEventArgs GetSummaryDisplayTextEventArgs(ASPxSummaryItemBase item, object value, string text);
	}
	public abstract class GridRenderHelper {
		internal const string CancelBubbleJs = "event.cancelBubble = true";
		public const string
			ScrollableContainerID = "Scrollable",
			FixedColumnsScrollableContainerID = "FixedColumnsScrollable",
			MainTableID = "DXMainTable",
			FooterTableID = "DXFooterTable",
			ScrollDivID = "DXScrollDiv",
			EditorID = "DXEditor",
			FilterRowEditorID = "DXFREditorcol",
			SelectButtonID = "DXSelBtn",
			SelectAllButtonID = "DXSelAllBtn",
			EndlessPagingUpdatableContainerID = "DXEPUC",
			EndlessPagingLoadingPanelContainerID = "DXEPLPC",
			GroupRowStringExpandedSuffix = "Exp",
			CustomizationWindowID = "custwindow",
			EditingErrorItemID = "DXEditingErrorItem",
			LoadingPanelContainerID = "DXLPContainer",
			PopupEditFormID = "DXPEForm",
			EditFormCellID = "DXEFC",
			EditFormTableID = "DXEFT",
			TopPagerPanelID = "DXTopPagerPanel",
			BottomPagerPanelID = "DXBottomPagerPanel",
			TopPagerID = "DXPagerTop",
			BottomPagerID = "DXPagerBottom",
			TitleID = "DXTitle",
			StatusBarID = "DXStatus",
			FilterBarID = "DXFilterBar",
			SearchPanelID = "DXSearchPanel",
			HeaderFilterListBoxID = "HFListBox",
			HeaderFilterSelectAllCheckBoxID = "HFSACheckBox",
			HeaderFilterCalendarID = "HFC",
			HeaderFilterFromDateEditID = "HFFDE",
			HeaderFilterToDateEditID = "HFTDE",
			BatchEditorsContainerID = "DXBEsC",
			BatchEditorContainerID = "DXBEC",
			BatchEditCellErrorTableID = "DXCErrorTable",
			ProgressBarDisplayControlIDFormat = "PBc{0}i{1}",
			ContextMenuID = "DXContextMenu",
			SearchEditorID = "DXSE",
			EditFormLayoutID = "DXEFL",
			AdaptiveFormLayoutID = "DXAFL",
			HeaderFilterButtonClassName = "dxgv__hfb",
			RequireLastVisibleRowBottomBorderCssClass = "dxgvRBB",
			HighlightEndHtml = "</em>";
		Control customSearchPanelEditor;
		string HighlightStartHtml = string.Empty;
		public GridRenderHelper(ASPxGridBase grid) {
			Grid = grid;
			EditorList = new List<ASPxEditBase>();
			DummyEditorList = new List<ASPxEditBase>();
			EditingRowEditorList = new List<ASPxEditBase>();
			ValidationError = new Dictionary<IWebGridColumn, string>();
			ColumnEditors = new Dictionary<IWebGridDataColumn, EditPropertiesBase>();
			HighlightTextProcessors = new Dictionary<IWebGridDataColumn, GridHighlightTextProcessor>();
			TextBuilder = CreateTextBuilder();
			Scripts = CreateGridScripts();
			SEO = CreateGridSEO();
			HighlightStringBuilder = new StringBuilder(100);
			HighlightStartHtml = "<em class='" + Grid.Styles.SearchPanelHighlightTextClassName + "'>";
		}
		public ASPxGridBase Grid { get; private set; }
		public GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		public WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		public ASPxGridPopupControlSettings SettingsPopup { get { return Grid.SettingsPopup; } }
		public GridStyles Styles { get { return Grid.Styles; } }
		public GridImages Images { get { return Grid.Images; } }
		public EditorStyles StylesEditors { get { return Grid.StylesEditors; } }
		public GridPopupControlStyles StylesPopup { get { return Grid.StylesPopup; } }
		public EditorImages ImagesEditors { get { return Grid.ImagesEditors; } }
		public GridTextBuilder TextBuilder { get; private set; }
		public GridCookiesBase SEO { get; private set; }
		public ASPxGridScripts Scripts { get; private set; }
		public List<ASPxEditBase> EditorList { get; private set; }
		public List<ASPxEditBase> DummyEditorList { get; private set; }
		public List<ASPxEditBase> EditingRowEditorList { get; private set; }
		public Dictionary<IWebGridColumn, string> ValidationError { get; private set; }
		protected Dictionary<IWebGridDataColumn, EditPropertiesBase> ColumnEditors { get; private set; }
		protected Dictionary<IWebGridDataColumn, GridHighlightTextProcessor> HighlightTextProcessors { get; private set; }
		protected StringBuilder HighlightStringBuilder { get; private set; }
		public string EditingErrorText { get; set; }
		public string CustomKbdHelperName { get; set; }
		public BrowserInfo Browser { get { return RenderUtils.Browser; } }
		public bool IsRightToLeft { get { return (Grid as ISkinOwner).IsRightToLeft(); } }
		public bool IsGridEnabled { get { return Grid.IsEnabled(); } }
		public bool IsGridExported { get { return Grid.IsExported; } }
		protected abstract GridCookiesBase CreateGridSEO();
		protected abstract ASPxGridScripts CreateGridScripts();
		protected abstract GridTextBuilder CreateTextBuilder();
		public abstract bool AddHeaderTemplateControl(IWebGridColumn column, Control templateContainer, GridHeaderLocation headerLocation);
		public abstract bool AddPagerBarTemplateControl(WebControl templateContainer, GridViewPagerBarPosition position, string pagerId);
		public abstract bool AddTitleTemplateControl(WebControl templateContainer);
		public abstract bool AddStatusBarTemplateControl(WebControl templateContainer);
		public abstract bool AddEditFormTemplateControl(WebControl templateContainer, int visibleIndex);
		public EditPropertiesBase GetColumnEdit(IWebGridDataColumn column) {
			if(column.PropertiesEdit != null) return column.PropertiesEdit;
			if(!ColumnEditors.ContainsKey(column))
				ColumnEditors[column] = EditRegistrationInfo.CreatePropertiesByDataType(DataProxy.GetFieldType(column.FieldName));
			return ColumnEditors[column];
		}
		public string GetSEOID() { return "seo" + Grid.ClientID; }
		public string GetVisibleIndexString(int visibleIndex) {
			if(visibleIndex >= 0) return visibleIndex.ToString();
			if(visibleIndex == ListSourceDataController.FilterRow) return "frow";
			return "new";
		}
		public string GetEditorId(IWebGridDataColumn column) {
			return EditorID + GetColumnGlobalIndex(column);
		}
		public int GetColumnGlobalIndex(IWebGridColumn column) {
			return Grid.GetColumnGlobalIndex(column);
		}
		public string GetSelectButtonId(int visibleIndex) {
			return SelectButtonID + GetVisibleIndexString(visibleIndex);
		}
		public bool HasEditingError { get { return !string.IsNullOrEmpty(EditingErrorText); } }
		public int CommandColumnsCount { get { return ColumnHelper.AllVisibleColumns.OfType<GridViewCommandColumn>().Count(); } }
		public bool ShowHeaderFilterButtonPanel { get { return ColumnHelper.AllDataColumns.Any(c => c.Visible && c.Adapter.IsMultiSelectHeaderFilter); } }
		public bool IsFocusedRowEnabled { get { return Grid.SettingsBehavior.AllowFocusedItem || Grid.KeyboardSupport; } }
		public bool AllowSelectSingleRowOnly { get { return Grid.SettingsBehavior.AllowSelectSingleItemOnly; } }  
		public bool RequireRenderHeaderFilterPopup {
			get {
				if(!IsGridEnabled) return false;
				bool hiddenColumnHasFilterButton = false;
				foreach(var column in ColumnHelper.AllDataColumns) {
					bool hasFilterButton = column.Adapter.HasFilterButton;
					if(column.Visible && hasFilterButton)
						return true;
					hiddenColumnHasFilterButton |= hasFilterButton;
				}
				if(hiddenColumnHasFilterButton && RequireRenderCustomizationWindow)
					return true;
				return false;
			}
		}
		public ScrollBarMode HorizontalScrollBarMode {
			get {
				if(Grid.Settings.HorizontalScrollBarMode == ScrollBarMode.Hidden)
					return Grid.Settings.ShowHorizontalScrollBarInternal ? ScrollBarMode.Visible : ScrollBarMode.Hidden;
				return Grid.Settings.HorizontalScrollBarMode;
			}
		}
		public ScrollBarMode VerticalScrollBarMode {
			get {
				if(Grid.Settings.VerticalScrollBarMode != ScrollBarMode.Hidden)
					return Grid.Settings.VerticalScrollBarMode;
				if(UseEndlessPaging && IsEndlessPagingRequireControlScrolling || IsVirtualScrolling)
					return ScrollBarMode.Visible;
				return Grid.Settings.ShowVerticalScrollBarInternal ? ScrollBarMode.Visible : ScrollBarMode.Hidden;
			}
		}
		public virtual bool IsEndlessPagingRequireControlScrolling { get { return true; } }
		public bool ShowVerticalScrolling { get { return VerticalScrollBarMode != ScrollBarMode.Hidden; } }
		public bool IsVirtualScrolling { get { return Grid.Settings.VerticalScrollBarStyle == GridViewVerticalScrollBarStyle.Virtual && !UseEndlessPaging || IsVirtualSmoothScrolling; } }
		public bool IsVirtualSmoothScrolling { get { return Grid.Settings.VerticalScrollBarStyle == GridViewVerticalScrollBarStyle.VirtualSmooth && !UseEndlessPaging; } }
		public bool ShowHorizontalScrolling { get { return HorizontalScrollBarMode != ScrollBarMode.Hidden; } }
		public bool HasScrolling { get { return ShowVerticalScrolling || ShowHorizontalScrolling; } }
		public virtual bool RequireHeaderTopBorder { get { return true; } }
		public virtual bool RequireFixedTableLayout { get { return HasScrolling || Grid.SettingsBehavior.AllowEllipsisInText; } }
		public virtual bool AllowColumnResizing { get { return false; } }
		public virtual bool RequireTablesHelperScripts { get { return HasScrolling; } }
		protected bool RequireRenderPagerControl {
			get {
				var pagerSettings = Grid.SettingsPager;
				if(pagerSettings.Mode != GridViewPagerMode.ShowPager || !pagerSettings.Visible)
					return false;
				if(Grid.DesignMode || Grid.PageCount > 1 || pagerSettings.AlwaysShowPager)
					return true;
				return pagerSettings.PageSizeItemSettings.Visible && Grid.PageCount > 0;
			}
		}
		public bool RequireRenderTopPagerControl { get { return RequireRenderPagerControl && Grid.SettingsPager.Position != PagerPosition.Bottom; } }
		public bool RequireRenderBottomPagerControl { get { return RequireRenderPagerControl && Grid.SettingsPager.Position != PagerPosition.Top; } }
		public bool HasEmptyDataRow { get { return DataProxy.VisibleRowCountOnPage == 0 && !DataProxy.IsEditing || AllowBatchEditing && DataProxy.VisibleRowCount <= DataProxy.VisibleRowCountOnPage; } }
		public int EmptyPagerDataRowCount {
			get {
				if(!Grid.SettingsPager.ShowEmptyGridItems || Grid.SettingsPager.Mode == GridViewPagerMode.ShowAllRecords) return 0;
				int result = Grid.SettingsPager.PageSize - DataProxy.VisibleRowCountOnPage;
				if(HasEmptyDataRow) result--;
				if(DataProxy.IsNewRowEditing) result--;
				return result;
			}
		}
		public virtual bool RequireRenderCustomizationWindow { get { return Grid.SettingsBehavior.EnableCustomizationWindow; } }
		public bool RequireRenderEditFormPopup { get { return Grid.SettingsEditing.IsPopupEditForm && DataProxy.IsEditing && !Grid.IsFilterControlVisible; } }
		public bool RequireRenderStatusBar {
			get {
				if(Grid.Settings.ShowStatusBar == GridViewStatusBarMode.Auto)
					return AllowBatchEditing || Grid.SettingsLoadingPanel.Mode == GridViewLoadingPanelMode.ShowOnStatusBar;
				return Grid.Settings.ShowStatusBar == GridViewStatusBarMode.Visible;
			}
		}
		public bool RequireRenderFilterBar {
			get {
				if(Grid.Settings.ShowFilterBar == GridViewStatusBarMode.Auto)
					return !string.IsNullOrEmpty(Grid.FilterExpression);
				return Grid.Settings.ShowFilterBar == GridViewStatusBarMode.Visible;
			}
		}
		public bool UseEndlessPaging { get { return Grid.SettingsPager.Mode == GridViewPagerMode.EndlessPaging && Grid.EnableCallBacks; } }
		public bool RequireEndlessPagingPartialLoad { get { return UseEndlessPaging && Grid.EndlessPagingHelper.PartialLoad; } }
		public bool CanCreateNewRowInEndlessPaging {
			get {
				if(!RequireEndlessPagingPartialLoad || Grid.SettingsEditing.NewItemPosition == GridViewNewItemRowPosition.Bottom)
					return true;
				var callbackCommand = Grid.EndlessPagingHelper.CallbackCommand;
				return callbackCommand == GridViewCallbackCommand.AddNewRow || callbackCommand == GridViewCallbackCommand.UpdateEdit;
			}
		}
		public bool AllowBatchEditing {
			get {
				if(!Grid.SettingsEditing.IsBatchEdit)
					return false;
				return Grid.SettingsDataSecurity.AllowInsert || Grid.SettingsDataSecurity.AllowEdit || Grid.SettingsDataSecurity.AllowDelete;
			}
		}
		public bool IsBatchEditCellMode { get { return AllowBatchEditing && Grid.SettingsEditing.BatchEditSettings.EditMode == GridViewBatchEditMode.Cell; } }
		public bool IsBatchEditDblClickStartAction { get { return AllowBatchEditing && Grid.SettingsEditing.BatchEditSettings.StartEditAction == GridViewBatchStartEditAction.DblClick; } }
		protected bool GridWidthSpecifiedInPixels { get { return !Grid.Width.IsEmpty && Grid.Width.Type != UnitType.Percentage; } }
		protected bool InplaceAllowEditorSizeRecalc { get { return GridWidthSpecifiedInPixels || RequireFixedTableLayout; } }
		public virtual bool AllowRemoveCellRightBorder { get { return true; } }
		public virtual void Invalidate() {
			EditorList.Clear();
			DummyEditorList.Clear();
			EditingRowEditorList.Clear();
			ColumnEditors.Clear();
			HighlightTextProcessors.Clear();
			InvalidateTemplates();
			this.customSearchPanelEditor = null;
		}
		protected virtual void InvalidateTemplates() { }
		public ArrayList GetCurrentPageKeyValues() {
			return DataProxy.GetPageKeyValuesForScript();
		}
		public string GetSelectInputValue() {
			StringBuilder sb = new StringBuilder();
			var firstIndexOnPage = DataProxy.VisibleStartIndex;
			var count = DataProxy.VisibleRowCountOnPage;
			if(UseEndlessPaging) {
				firstIndexOnPage = 0;
				count = Grid.EndlessPagingHelper.LoadedRowCount;
			}
			for(int i = 0; i < count; i++)
				sb.Append(DataProxy.Selection.IsRowSelected(firstIndexOnPage + i) ? 'T' : 'F');
			string result = sb.ToString();
			int n = result.LastIndexOf("T");
			result = n < 0 ? string.Empty : result.Substring(0, n + 1);
			return result;
		}
		public virtual void AddDisplayControlToDataCell(Control cell, IWebGridDataColumn column, int visibleIndex, IValueProvider row) {
			var displayControl = CreateDataCellDisplayControl(column, visibleIndex, row);
			if(column.PropertiesEdit is ProgressBarProperties)
				displayControl.ID = string.Format(ProgressBarDisplayControlIDFormat, ColumnHelper.GetColumnGlobalIndex(column), visibleIndex);
			cell.Controls.Add(displayControl);
		}
		protected virtual Control CreateDataCellDisplayControl(IWebGridDataColumn column, int visibleIndex, IValueProvider row) {
			return GetColumnEdit(column).CreateDisplayControl(TextBuilder.GetDisplayControlArgs(column, visibleIndex, row, Grid, true));
		}
		public void CreateGridDummyEditors(Control parent) {
			foreach(var column in ColumnHelper.AllDataColumns) {
				var editor = CreateGridEditor(column, null, EditorInplaceMode.Inplace, true);
				parent.Controls.Add(editor);
				DummyEditorList.Add(editor);
			}
		}
		public ASPxEditBase CreateEditor(int visibleIndex, IWebGridDataColumn column, Control editorContainer, EditorInplaceMode mode, int parentRowSpan) {
			return CreateEditor(visibleIndex, column, editorContainer, mode, parentRowSpan, false, null);
		}
		public ASPxEditBase CreateEditor(int visibleIndex, IWebGridDataColumn column, Control editorContainer, EditorInplaceMode mode, int parentRowSpan, bool insideFormLayout) {
			return CreateEditor(visibleIndex, column, editorContainer, mode, parentRowSpan, insideFormLayout, null);
		}
		public ASPxEditBase CreateEditor(int visibleIndex, IWebGridDataColumn column, Control editorContainer, EditorInplaceMode mode, int parentRowSpan, bool insideFormLayout, Func<ASPxEditBase> createCustomEditor) {
			object value = DataProxy.GetEditingRowValue(visibleIndex, column.FieldName);
			ASPxEditBase editor = CreateGridEditor(DataProxy.EditingRowVisibleIndex, column, value, mode, false, createCustomEditor);
			editorContainer.Controls.Add(editor);
			ApplyEditorSettings(editor, column);
			if(RequireFullEditorWidth(editor, insideFormLayout))
				editor.Width = Unit.Percentage(100);
			if(RequireFullEditorHeight(editor, parentRowSpan))
				editor.Height = Unit.Percentage(100);
			var e = Grid.CreateCellEditorInitializeEventArgs(column, visibleIndex, editor, DataProxy.EditingKeyValue, value);
			Grid.RaiseEditorInitialize(e);
			return editor;
		}
		bool RequireFullEditorWidth(ASPxEditBase editor, bool insideFormLayout) {
			if(insideFormLayout && !Grid.EditFormLayoutProperties.NestedControlWidth.IsEmpty)
				return false;
			return editor.Width.IsEmpty && editor as ASPxBinaryImage == null;
		}
		bool RequireFullEditorHeight(ASPxEditBase editor, int parentRowSpan) {
			ASPxMemo memo = editor as ASPxMemo;
			if(memo == null)
				return false;
			if(!memo.Height.IsEmpty || memo.Rows > 0)
				return false;
			return parentRowSpan > 1;
		}
		public ASPxEditBase CreateGridEditor(IWebGridDataColumn column, object value, EditorInplaceMode mode, bool isInternal) {
			return CreateGridEditor(-1, column, value, mode, isInternal, null);
		}
		public ASPxEditBase CreateGridEditor(int editingRowVisibleIndex, IWebGridDataColumn column, object value, EditorInplaceMode mode, bool isInternal, Func<ASPxEditBase> createCustomEditor) {
			EnsureColumnPropertiesBound(column);
			CreateEditControlArgs args = new CreateEditControlArgs(editingRowVisibleIndex, value, column.Adapter.DataType, Grid.ImagesEditors, StylesEditors, Grid, mode, InplaceAllowEditorSizeRecalc);
			ASPxEditBase baseEditor = CreateGridEditorCore(column, isInternal, args, createCustomEditor);
			baseEditor.ID = GetEditorId(column);
			baseEditor.EnableClientSideAPI = true;
			baseEditor.EnableViewState = false;
			ClientIDHelper.EnableClientIDGeneration(baseEditor);
			if(isInternal) {
				ASPxEdit editor = baseEditor as ASPxEdit;
				if(editor != null)
					editor.ValidationSettings.Assign(ValidationSettings.CreateValidationSettings(editor));
			}
			else {
				EditorList.Add(baseEditor);
				EditingRowEditorList.Add(baseEditor);
			}
			return baseEditor;
		} 
		protected ASPxEditBase CreateGridEditorCore(IWebGridDataColumn column, bool isInternal, CreateEditControlArgs args, Func<ASPxEditBase> createCustomEditor) {
			ASPxEditBase customEditor = createCustomEditor != null ? createCustomEditor() : null;
			return customEditor != null ? customEditor : GetColumnEdit(column).CreateEdit(args, isInternal);
		}
		protected void EnsureColumnPropertiesBound(IWebGridDataColumn column) {
			var autoCompleteProp = column.PropertiesEdit as AutoCompleteBoxPropertiesBase;
			if(autoCompleteProp != null && autoCompleteProp.DataSource != null && autoCompleteProp.Items.Count == 0)
				autoCompleteProp.CheckInplaceBound(column.Adapter.DataType, Grid.DummyNamingContainer, Grid.DesignMode);
		}
		public virtual void ApplyEditorSettings(ASPxEditBase baseEditor, IWebGridDataColumn column) {
			ASPxEdit editor = baseEditor as ASPxEdit;
			if(editor != null) {
				if(editor.IsValid && ValidationError.ContainsKey(column)) {
					editor.IsValid = false;
					editor.ErrorText = ValidationError[column];
				}
			}
			baseEditor.ReadOnly = Grid.IsReadOnly(column);
		}
		public ASPxEditBase CreateSearchPanelEditor(TableCell cell) {
			var prop = EditRegistrationInfo.CreateProperties("ButtonEdit");
			var args = new CreateEditControlArgs(Grid.SearchPanelFilter, typeof(string), Grid.ImagesEditors, StylesEditors, Grid, EditorInplaceMode.Inplace, InplaceAllowEditorSizeRecalc);
			var ce = Grid.CreateSearchPanelEditorCreateEventArgs(prop, args.EditValue);
			Grid.RaiseSearchPanelEditorCreate(ce);
			prop = ce.EditorProperties;
			args.EditValue = ce.Value;
			var textEditProp = prop as TextEditProperties;
			if(textEditProp != null)
				textEditProp.NullTextInternal = Grid.SettingsText.GetSearchPanelEditorNullText();
			var edit = (ASPxButtonEditBase)prop.CreateEdit(args);
			edit.ForceShowClearButtonAlways = true;
			edit.ID = GridViewRenderHelper.SearchEditorID;
			ClientIDHelper.EnableClientIDGeneration(edit);
			edit.EnableViewState = false;
			EditorList.Add(edit);
			cell.Controls.Add(edit);
			var e = Grid.CreateSearchPanelEditorInitializeEventArgs(edit, args.EditValue);
			Grid.RaiseSearchPanelEditorInitialize(e);
			return edit;
		}
		protected internal static void EnsureTemplateReplacements(Control templateContainer) {
			foreach(Control child in templateContainer.Controls) {
				if(child is ASPxGridView)
					continue;
				IASPxWebControl ensurable = child as IASPxWebControl;
				if(ensurable != null)
					ensurable.EnsureChildControls();
				EnsureTemplateReplacements(child);
			}
		}
		#region SearchPanel
		public virtual string CustomSearchPanelEditorClientID { get { return CustomSearchPanelEditor != null ? CustomSearchPanelEditor.ClientID : string.Empty; } }
		protected Control CustomSearchPanelEditor {
			get {
				if(string.IsNullOrEmpty(Grid.SettingsSearchPanel.CustomEditorID))
					return null;
				if(customSearchPanelEditor == null)
					customSearchPanelEditor = FindControlHelper.LookupControl(Grid, Grid.SettingsSearchPanel.CustomEditorID);
				return customSearchPanelEditor;
			}
		}
		public bool RequireHighlightSearchText(IWebGridDataColumn column) {
			return Grid.SettingsSearchPanel.HighlightResults && !string.IsNullOrEmpty(Grid.SearchPanelFilter) && ColumnHelper.SearchPanelColumns.Contains(column);
		}
		public GridHighlightTextProcessor GetHighlightTextProcessor(IWebGridDataColumn column) {
			if(!HighlightTextProcessors.ContainsKey(column))
				HighlightTextProcessors[column] = new GridHighlightTextProcessor(Grid, column);
			return HighlightTextProcessors[column];
		}
		public string HighlightSearchPanelText(IWebGridDataColumn column, string text, bool encode) {
			var hightlightPositions = Grid.FilterHelper.GetHighlightTextPositions(column, text, encode);
			if(hightlightPositions.Count == 0)
				return text;
			HighlightStringBuilder.Clear();
			HighlightStringBuilder.Append("<span>");
			int startIndex, endIndex, globalIndex = 0;
			foreach(var position in hightlightPositions) {
				startIndex = position.Item1;
				endIndex = position.Item2;
				if(startIndex != globalIndex)
					HighlightStringBuilder.Append(text.Substring(globalIndex, startIndex - globalIndex));
				HighlightStringBuilder.Append(HighlightStartHtml);
				HighlightStringBuilder.Append(text.Substring(startIndex, endIndex - startIndex));
				HighlightStringBuilder.Append(HighlightEndHtml);
				globalIndex = endIndex;
			}
			if(globalIndex != text.Length)
				HighlightStringBuilder.Append(text.Substring(globalIndex, text.Length - globalIndex));
			HighlightStringBuilder.Append("</span>");
			return HighlightStringBuilder.ToString();
		}
		#endregion
		#region Selection
		public bool HasAnySelectCheckbox { get { return HasAnySelectCheckBoxInternal || HasAnySelectAllCheckbox; } }
		public virtual bool HasAnySelectAllCheckbox { get { return false; } }
		protected abstract bool HasAnySelectCheckBoxInternal { get; }
		public List<InternalCheckBoxImageProperties> GetCheckImages() {
			return new List<InternalCheckBoxImageProperties>(new InternalCheckBoxImageProperties[] {
				GetCheckImage(CheckState.Checked, AllowSelectSingleRowOnly), 
				GetCheckImage(CheckState.Unchecked, AllowSelectSingleRowOnly),
				GetCheckImage(CheckState.Indeterminate, false)
			});
		}
		public InternalCheckBoxImageProperties GetCheckImage(CheckState checkState, bool isRadioButton) { 
			var result = new InternalCheckBoxImageProperties();
			string imageName = string.Empty;
			switch(checkState) {
				case CheckState.Checked:
					imageName = isRadioButton ? EditorImages.RadioButtonCheckedImageName : InternalCheckboxControl.CheckBoxCheckedImageName;
					result.MergeWith(isRadioButton ? ImagesEditors.RadioButtonChecked : ImagesEditors.CheckBoxChecked);
					break;
				case CheckState.Unchecked:
					imageName = isRadioButton ? EditorImages.RadioButtonUncheckedImageName : InternalCheckboxControl.CheckBoxUncheckedImageName;
					result.MergeWith(isRadioButton ? ImagesEditors.RadioButtonUnchecked : ImagesEditors.CheckBoxUnchecked);
					break;
				default:
					imageName = InternalCheckboxControl.CheckBoxGrayedImageName;
					result.MergeWith(ImagesEditors.CheckBoxGrayed);
					break;
			}
			result.MergeWith(isRadioButton ? ImagesEditors.GetImageProperties(Grid.Page, imageName) :
			ImagesEditors.GetImageProperties(Grid.Page, imageName));
			ImagesEditors.UpdateSpriteUrl(result, Grid.Page, isRadioButton ? InternalCheckboxControl.EditorsSpriteControlName : InternalCheckboxControl.WebSpriteControlName,
				isRadioButton ? null : typeof(ASPxWebControl), isRadioButton ? string.Empty : InternalCheckboxControl.DesignModeSpriteImagePath);
			return result;
		}
		public AppearanceStyleBase GetCheckBoxStyle() {
			var style = Styles.CreateStyleByName(string.Empty, (AllowSelectSingleRowOnly ? InternalCheckboxControl.RadioButtonClassName : InternalCheckboxControl.CheckBoxClassName));
			style.CopyFrom(AllowSelectSingleRowOnly ? StylesEditors.RadioButton : StylesEditors.CheckBox);
			return style;
		}
		public AppearanceStyleBase GetCheckBoxFocusedStyle() {
			var style = Styles.CreateStyleByName(string.Empty, (AllowSelectSingleRowOnly ? InternalCheckboxControl.FocusedRadioButtonClassName : InternalCheckboxControl.FocusedCheckBoxClassName));
			style.CopyFrom(AllowSelectSingleRowOnly ? StylesEditors.RadioButtonFocused : StylesEditors.CheckBoxFocused);
			return style;
		}
		#endregion
		#region Popup Settings
		public virtual Unit GetPopupEditFormWidth() {
			return SettingsPopup.EditForm.Width;
		}
		public virtual Unit GetPopupEditFormHeight() {
			return SettingsPopup.EditForm.Height;
		}
		public Unit GetPopupEditFormMinWidth() {
			return SettingsPopup.EditForm.MinWidth;
		}
		public Unit GetPopupEditFormMinHeight() {
			return SettingsPopup.EditForm.MinHeight;
		}
		public virtual PopupHorizontalAlign GetPopupEditFormHorizontalAlign() {
			return SettingsPopup.EditForm.HorizontalAlign;
		}
		public virtual PopupVerticalAlign GetPopupEditFormVerticalAlign() {
			return SettingsPopup.EditForm.VerticalAlign;
		}
		public virtual int GetPopupEditFormHorizontalOffset() {
			return SettingsPopup.EditForm.HorizontalOffset;
		}
		public virtual int GetPopupEditFormVerticalOffset() {
			return SettingsPopup.EditForm.VerticalOffset;
		}
		public virtual bool GetPopupEditFormShowHeader() {
			return SettingsPopup.EditForm.ShowHeader;
		}
		public virtual bool GetPopupEditFormAllowResize() {
			return SettingsPopup.EditForm.AllowResize;
		}
		public ResizingMode GetPopupEditFormResizeMode() {
			return SettingsPopup.EditForm.ResizingMode;
		}
		public virtual bool GetPopupEditFormModal() {
			return SettingsPopup.EditForm.Modal;
		}
		public bool GetPopupEditFormCloseOnEscape() {
			var closeOnEsc = SettingsPopup.EditForm.CloseOnEscape;
			return closeOnEsc == AutoBoolean.Auto ? GetPopupEditFormModal() : closeOnEsc == AutoBoolean.True;
		}
		public virtual Unit GetCustomizationWindowWidth() {
			if(!SettingsPopup.CustomizationWindow.Width.IsEmpty)
				return SettingsPopup.CustomizationWindow.Width;
			return GridViewCustomizationWindowPopupSettings.DefaultWidth;
		}
		public virtual Unit GetCustomizationWindowHeight() {
			if(!SettingsPopup.CustomizationWindow.Height.IsEmpty) {
				if(SettingsPopup.CustomizationWindow.Height.Type == UnitType.Percentage)
					return Unit.Empty;
				return SettingsPopup.CustomizationWindow.Height;
			}
			return GridViewCustomizationWindowPopupSettings.DefaultHeight;
		}
		public virtual PopupHorizontalAlign GetCustomizationWindowHorizontalAlign() {
			return SettingsPopup.CustomizationWindow.HorizontalAlign;
		}
		public virtual PopupVerticalAlign GetCustomizationWindowVerticalAlign() {
			return SettingsPopup.CustomizationWindow.VerticalAlign;
		}
		public virtual int GetCustomizationWindowHorizontalOffset() {
			return SettingsPopup.CustomizationWindow.HorizontalOffset;
		}
		public virtual int GetCustomizationWindowVerticalOffset() {
			return SettingsPopup.CustomizationWindow.VerticalOffset;
		}
		public bool GetCustomizationWindowCloseOnEscape() {
			return SettingsPopup.CustomizationWindow.CloseOnEscape == AutoBoolean.True;
		}
		public ASPxWebControlBase CreateHeaderFilterContainer(IWebGridDataColumn column, bool includeFilteredOut) {
			if(column.Adapter.IsDateRangeHeaderFilterMode)
				return new GridHtmlFilterByDateRangeContainer(column, includeFilteredOut);
			return new GridHtmlFilterByValueContainer(column, includeFilteredOut);
		}
		public Unit GetHeaderFilterPopupWidth() {
			return SettingsPopup.HeaderFilter.Width;
		}
		public Unit GetHeaderFilterPopupHeight() {
			if(!SettingsPopup.HeaderFilter.Height.IsEmpty)
				return SettingsPopup.HeaderFilter.Height;
			return Grid.SettingsBehavior.HeaderFilterHeightInternal;
		}
		public Unit GetHeaderFilterPopupMinWidth() {
			return SettingsPopup.HeaderFilter.MinWidth;
		}
		public Unit GetHeaderFilterPopupMinHeight() {
			return SettingsPopup.HeaderFilter.MinHeight;
		}
		public ResizingMode GetHeaderFilterPopupResizeMode() {
			return SettingsPopup.HeaderFilter.ResizingMode;
		}
		public bool GetHeaderFilterPopupCloseOnEscape() {
			return SettingsPopup.HeaderFilter.CloseOnEscape != AutoBoolean.False;
		}
		#endregion
		public void AppendGridCssClassName(WebControl control) {
			RenderUtils.AppendDefaultDXClassName(control, Styles.GetCssClassNamePrefix());
		}
		public void AppendIndentCellCssClassName(WebControl control) {
			RenderUtils.AppendDefaultDXClassName(control, GridViewStyles.GridIndentCellCssClass);
		}
		public void AppendAdaptiveIndentCellCssClassName(WebControl control) {
			RenderUtils.AppendDefaultDXClassName(control, GridViewStyles.GridAdaptiveIndentCellCssClass);
		}
		protected T MergeStyle<T>(string name, params Style[] extra) where T : AppearanceStyleBase, new() {
			return MergeStyle<T>(Styles.CreateStyleCopyByName<T>(name), extra);
		}
		protected T MergeStyle<T>(params Style[] extra) where T : AppearanceStyleBase, new() {
			return MergeStyle<T>(new T(), extra);
		}
		protected T MergeStyle<T>(T style, params Style[] extra) where T : AppearanceStyleBase, new() { 
			foreach(AppearanceStyleBase item in extra)
				style.CopyFrom(item);
			return style;
		}
		protected void ApplyDisplayControlTextAlign(AppearanceStyleBase style, IWebGridColumn column) {
			if(style.HorizontalAlign != HorizontalAlign.NotSet)
				return;
			style.HorizontalAlign = GetColumnDisplayControlDefaultAlignment(column);
		}
		public virtual HorizontalAlign GetColumnDisplayControlDefaultAlignment(IWebGridColumn column) {
			var dc = column as IWebGridDataColumn;
			if(dc == null) return HorizontalAlign.NotSet;
			var edit = GetColumnEdit(dc);
			HorizontalAlign align = edit == null ? HorizontalAlign.NotSet : edit.GetDisplayControlDefaultAlign();
			if(align == HorizontalAlign.NotSet) align = GetDisplayControlDefaultAlign(dc);
			return align;
		}
		protected HorizontalAlign GetDisplayControlDefaultAlign(IWebGridDataColumn column) {
			return DataUtils.IsNumericType(column.Adapter.DataType) ? HorizontalAlign.Right : HorizontalAlign.NotSet;
		}
		public void ParseEditorValues() {
			if(AllowBatchEditing) {
				Grid.BatchEditHelper.ParseEditorValues();
				return;
			}
			if(DataProxy.IsEditorValuesExists) {
				EnsureAutoCreatedEditorsHaveParsedValues();
				return;
			}
			Dictionary<string, object> values = new Dictionary<string, object>();
			foreach(ASPxEditBase editor in EditorList) {
				int index = GetEditColumnIndex(editor);
				if(index < 0) continue;
				var col = ColumnHelper.AllColumns[index] as IWebGridDataColumn;
				if(col != null)
					values[col.FieldName] = GetEditorParsedValue(editor);
			}
			DataProxy.SetEditorValues(values, false);
		}
		object GetEditorParsedValue(ASPxEditBase editor) {
			if(editor is ASPxColorEdit)
				return ((ASPxColorEdit)editor).Text;
			return editor.Value;
		}
		int GetEditColumnIndex(ASPxEditBase editor) {
			if(editor.ID.StartsWith(EditorID)) {
				int res;
				if(int.TryParse(editor.ID.Substring(EditorID.Length), out res)) return res;
			}
			return -1;
		}
		void EnsureAutoCreatedEditorsHaveParsedValues() {
			foreach(ASPxEditBase editor in EditorList) {
				int index = GetEditColumnIndex(editor);
				if(index < 0) continue;
				var col = ColumnHelper.AllColumns[index] as IWebGridDataColumn;
				if(col == null) continue;
				editor.Value = DataProxy.GetEditingRowValue(DataProxy.EditingRowVisibleIndex, col.FieldName);
			}
		}
		public Unit GetMainTableWidth(bool isScrollableTable) {
			if(ShowVerticalScrolling && !isScrollableTable) return Unit.Percentage(100);
			return !ShowHorizontalScrolling ? Unit.Percentage(100) : Unit.Empty;
		}
		public Unit GetNarrowCellWidth() {
			if(RequireFixedTableLayout)
				return Unit.Empty;
			if(Browser.IsIE && Browser.Version == 8)
				return Unit.Percentage(1);
			return Unit.Percentage(0.1);
		}
		public virtual Unit GetRootTableWidth() {
			if(!Grid.Width.IsEmpty)
				return Grid.Width;
			if(ShowHorizontalScrolling || RequireRenderPagerControl || !ShowVerticalScrolling)
				return Unit.Empty;
			if(ColumnHelper.AllVisibleColumns.Count == 0)
				return 200;
			return ColumnHelper.AllVisibleColumns.OfType<WebColumnBase>().Sum(c => {
				if(c.Width.IsEmpty)
					return 50;
				if(c.Width.Type == UnitType.Percentage)
					return 0;
				return Convert.ToInt32(c.Width.Value);
			});
		}
		public static void NormalizeColSpans(TableRowCollection rowCollection) { 
			var rows = rowCollection.OfType<TableRow>().ToList();
			if(rows.Count == 0)
				return;
			var maxCellCount = 0;
			var rowSpanedCells = new int[rows.Count];
			for(var i = 0; i < rows.Count; i++) {
				var row = rows[i];
				maxCellCount = Math.Max(maxCellCount, row.Cells.Count + rowSpanedCells[i]);
				foreach(TableCell cell in row.Cells) {
					if(cell.RowSpan < 2)
						continue;
					for(var k = i + 1; k < i + cell.RowSpan; k++)
						rowSpanedCells[k]++;
				}
			}
			for(var i = 0; i < rows.Count; i++) {
				var cells = rows[i].Cells.OfType<TableCell>();
				var colSpanSum = cells.Sum(c => c.ColumnSpan > 0 ? c.ColumnSpan : 1);
				var rowMaxCellCount = maxCellCount - rowSpanedCells[i];
				var diff = colSpanSum - rowMaxCellCount;
				for(int j = 0; j < diff; j++) {
					var maxColSpan = cells.Max(c => c.ColumnSpan);
					var maxColSpanCell = cells.First(c => c.ColumnSpan == maxColSpan);
					maxColSpanCell.ColumnSpan--;
				}
			}
			foreach(var cell in rows.SelectMany(r => r.Cells.OfType<TableCell>())) {
				if(cell.ColumnSpan == 1)
					cell.ColumnSpan = 0;
			}
		}
		public static WebControl DecorateTableForScrollableDiv(Table table, bool rtl) {
			if(!rtl)
				return table;
			WebControl div = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			div.Controls.Add(table);
			if(rtl)
				div.Attributes["dir"] = "rtl";
			return div;
		}
		public static string TrimString(string value) {
			if(value == null) return string.Empty;
			return value.Trim();
		}
		protected static void CopyCellAttributes(AppearanceStyle from, AppearanceStyle to) {
			to.CopyBordersFrom(from);
			to.Paddings.Assign(from.Paddings);
			to.VerticalAlign = from.VerticalAlign;
			to.HorizontalAlign = from.HorizontalAlign;
		}
		public void PrepareLoadingPanel(LoadingPanelControl loadingPanel) {
			var style = GetLoadingPanelStyle();
			loadingPanel.Style = style;
			loadingPanel.Paddings = style.Paddings;
			loadingPanel.ImageSpacing = style.ImageSpacing;
		}
		public void PrepareLoadingDiv(WebControl loadingDiv) {
			RenderUtils.SetVisibility(loadingDiv, false, true);
			var style = GetLoadingDivStyle();
			style.AssignToControl(loadingDiv);
			loadingDiv.Style.Add("z-index", RenderUtils.LoadingDivZIndex.ToString());
			loadingDiv.Style.Add("position", "absolute");
		}
		public AppearanceStyle GetConditionalFormatCellStyle(IWebGridDataColumn column, int visibleIndex) {
			AppearanceStyle style = new AppearanceStyle();
			IEnumerable<GridFormatConditionBase> columnConditions = Grid.FormatConditions.GetActiveColumnCellConditions(column);
			foreach(GridFormatConditionBase cond in columnConditions) {
				if(cond.IsFitToRule(DataProxy, visibleIndex))
					style.CopyFrom(cond.GetItemCellStyle(DataProxy, visibleIndex));
			}
			return style;
		}
		public AppearanceStyle GetConditionalFormatItemStyle(int visibleIndex) {
			AppearanceStyle style = new AppearanceStyle();
			IEnumerable<GridFormatConditionBase> rowConditions = Grid.FormatConditions.GetActiveItemConditions();
			foreach(GridFormatConditionBase cond in rowConditions) {
				if(cond.IsFitToRule(DataProxy, visibleIndex))
					style.CopyFrom(cond.GetItemStyle(DataProxy, visibleIndex));
			}
			return style;
		}
		public GridHeaderStyle GetHeaderStyle(IWebGridColumn column) {
			return GetHeaderStyleCore(column);
		}
		protected abstract GridHeaderStyle GetHeaderStyleCore(IWebGridColumn column);
		public ImageProperties GetBatchEditErrorCellImage() { return GetImage(GridImages.BatchEditErrorCellName); }
		public ImageProperties GetFilterBarButtonImage() { return GetImage(GridImages.FilterRowButtonName); }
		public ImageProperties GetHeaderSortImage(ColumnSortOrder sortOrder) {
			return sortOrder == ColumnSortOrder.Ascending ?
				GetImage(GridImages.HeaderSortUpName) : GetImage(GridImages.HeaderSortDownName);
		}
		public ImageProperties GetImage(string imageName) {
			return Images.GetImageProperties(Grid.Page, imageName);
		}
		public void AssignImageToControl(string imageName, Image image) {
			Images.GetImageProperties(Grid.Page, imageName).AssignToControl(image, Grid.DesignMode);
		}
		protected void AddTemplateToControl(Control destination, ITemplate template, GridBaseTemplateContainer templateContainer, TemplateContainerCollection collection) {
			string containerID = templateContainer.GetID();
			template.InstantiateIn(templateContainer);
			templateContainer.AddToHierarchy(destination, containerID);
			if(string.IsNullOrEmpty(destination.ID) && !string.IsNullOrEmpty(containerID)) {
				destination.ID = "tc" + containerID;
			}
			if(collection != null)
				collection.Add(templateContainer);
		}
		protected ITemplate GetTemplate(params ITemplate[] templates) {
			for(int n = templates.Length - 1; n >= 0; n--) {
				if(templates[n] != null)
					return templates[n];
			}
			return null;
		}
		public static GridCommandButtonType ConvertButtonType(CardViewCommandButtonType source) {
			switch(source) {
				case CardViewCommandButtonType.Edit:
					return GridCommandButtonType.Edit;
				case CardViewCommandButtonType.New:
					return GridCommandButtonType.New;
				case CardViewCommandButtonType.Delete:
					return GridCommandButtonType.Delete;
				case CardViewCommandButtonType.Select:
					return GridCommandButtonType.Select;
				case CardViewCommandButtonType.Update:
					return GridCommandButtonType.Update;
				case CardViewCommandButtonType.Cancel:
					return GridCommandButtonType.Cancel;
				case CardViewCommandButtonType.SelectCheckbox:
					return GridCommandButtonType.SelectCheckbox;
				case CardViewCommandButtonType.ApplySearchPanelFilter:
					return GridCommandButtonType.ApplySearchPanelFilter;
				case CardViewCommandButtonType.ClearSearchPanelFilter:
					return GridCommandButtonType.ClearSearchPanelFilter;
				case CardViewCommandButtonType.EndlessPagingShowMoreCards:
					return GridCommandButtonType.EndlessShowMoreCards;
			}
			throw new ArgumentException();
		}
		public static GridCommandButtonType ConvertButtonType(ColumnCommandButtonType source) {
			switch(source) {
				case ColumnCommandButtonType.Edit:
					return GridCommandButtonType.Edit;
				case ColumnCommandButtonType.New:
					return GridCommandButtonType.New;
				case ColumnCommandButtonType.Delete:
					return GridCommandButtonType.Delete;
				case ColumnCommandButtonType.Select:
					return GridCommandButtonType.Select;
				case ColumnCommandButtonType.Update:
					return GridCommandButtonType.Update;
				case ColumnCommandButtonType.Cancel:
					return GridCommandButtonType.Cancel;
				case ColumnCommandButtonType.ClearFilter:
					return GridCommandButtonType.ClearFilter;
				case ColumnCommandButtonType.SelectCheckbox:
					return GridCommandButtonType.SelectCheckbox;
				case ColumnCommandButtonType.ApplyFilter:
					return GridCommandButtonType.ApplyFilter;
				case ColumnCommandButtonType.ApplySearchPanelFilter:
					return GridCommandButtonType.ApplySearchPanelFilter;
				case ColumnCommandButtonType.ClearSearchPanelFilter:
					return GridCommandButtonType.ClearSearchPanelFilter;
			}
			throw new ArgumentException();
		}
		public virtual GridCommandColumnButtonControl CreateCommandButtonControl(GridCommandButtonType buttonType, int visibleIndex, bool postponeClick) { 
			return CreateCommandButtonControl(null, buttonType, visibleIndex, postponeClick); 
		}
		public abstract GridCommandColumnButtonControl CreateCommandButtonControl(WebColumnBase column, GridCommandButtonType buttonType, int visibleIndex, bool postponeClick); 
		protected bool CanCreateCommandButton(GridCommandButtonType commandItemType) {
			if(commandItemType == GridCommandButtonType.New && !Grid.SettingsDataSecurity.AllowInsert)
				return false;
			if(commandItemType == GridCommandButtonType.Edit && !Grid.SettingsDataSecurity.AllowEdit)
				return false;
			if(commandItemType == GridCommandButtonType.Delete && !Grid.SettingsDataSecurity.AllowDelete)
				return false;
			return true;
		}
		protected GridCommandButtonRenderMode GetButtonType(GridCommandButtonSettings buttonSettings, GridCommandButtonRenderMode buttonRenderType) {
			if(buttonSettings.ButtonType != GridCommandButtonRenderMode.Default)
				return buttonSettings.ButtonType;
			if(buttonRenderType != GridCommandButtonRenderMode.Default)
				return buttonRenderType;
			return GridCommandButtonRenderMode.Link;
		}
		protected virtual GridCommandButtonSettings GetCommandButtonSettings(GridCommandButtonType buttonType) {
			switch(buttonType) {
				case GridCommandButtonType.New:
					return Grid.SettingsCommandButton.NewButton;
				case GridCommandButtonType.Edit:
					return Grid.SettingsCommandButton.EditButton;
				case GridCommandButtonType.Update:
					return Grid.SettingsCommandButton.UpdateButton;
				case GridCommandButtonType.Cancel:
					return Grid.SettingsCommandButton.CancelButton;
				case GridCommandButtonType.Delete:
					return Grid.SettingsCommandButton.DeleteButton;
				case GridCommandButtonType.Select:
					return Grid.SettingsCommandButton.SelectButton;
				case GridCommandButtonType.ApplySearchPanelFilter:
					return Grid.SettingsCommandButton.SearchPanelApplyButton;
				case GridCommandButtonType.ClearSearchPanelFilter:
					return Grid.SettingsCommandButton.SearchPanelClearButton;
				case GridCommandButtonType.SelectCheckbox:
				default:
					return null;
			}
		}
		protected virtual GetCommandColumnButtonClickHandlerArgs GetCommandButtonClickHandlerArgs(GridCommandButtonType commandItemType) {
			switch(commandItemType) {
				case GridCommandButtonType.New:
					return Scripts.GetAddNewRowFuncArgs;
				case GridCommandButtonType.Edit:
					return Scripts.GetStartEditFuncArgs;
				case GridCommandButtonType.Update:
					return Scripts.GetUpdateEditFuncArgs;
				case GridCommandButtonType.Cancel:
					return Scripts.GetCancelEditFuncArgs;
				case GridCommandButtonType.Delete:
					return Scripts.GetDeleteRowFuncArgs;
				case GridCommandButtonType.Select:
					return Scripts.GetSelectRowFuncArgs;
				case GridCommandButtonType.ApplySearchPanelFilter:
					return Scripts.GetApplySearchFilterFuncArgs;
				case GridCommandButtonType.ClearSearchPanelFilter:
					return Scripts.GetClearSearchFilterFuncArgs;
				case GridCommandButtonType.SelectCheckbox:
				default:
					return null;
			}
		}
		#region TODO GVRefactor
		public virtual AppearanceStyleBase GetRowHotTrackStyle() { return new AppearanceStyleBase(); }
		public virtual AppearanceStyleBase GetDataCellStyle(GridViewColumn column) { return new AppearanceStyleBase(); }
		public virtual AppearanceStyle GetCommandItemsCellStyle() { return new AppearanceStyle(); }
		public virtual ImageProperties GetHeaderFilterPopupSizeGripImage() { return new ImageProperties(); }
		public virtual ImageProperties GetHeaderFilterPopupSizeGripRtlImage() { return new ImageProperties(); }
		#endregion
		public AppearanceStyle GetRootTableStyle() {
			var style = new AppearanceStyle();
			Grid.MergeParentSkinOwnerControlStyle(style);
			return MergeStyle<AppearanceStyle>(GridStyles.ControlStyleName, style, Grid.ControlStyle);
		}
		public AppearanceStyle GetDisabledRootTableStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.DisabledStyleName, GetRootTableStyle(), Styles.DisabledInternal);
		}
		public AppearanceStyle GetMainTableStyle() {
			return MergeStyle<GridViewTableStyle>(GridStyles.TableStyleName, Styles.Table);
		}
		public AppearanceStyle GetTitleStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.TitlePanelStyleName, Styles.TitlePanel);
		}
		public AppearanceStyle GetStatusBarStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.StatusBarStyleName, Styles.StatusBar);
		}
		public AppearanceStyle GetFilterBarStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.FilterBarStyleName, Styles.FilterBar);
		}
		public AppearanceStyle GetPagerTopPanelStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.PagerTopPanelStyleName, Styles.PagerTopPanel);
		}
		public AppearanceStyle GetPagerBottomPanelStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.PagerBottomPanelStyleName, Styles.PagerBottomPanel);
		}
		public AppearanceStyle GetSearchPanelStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.SearchPanelStyleName, Styles.SearchPanel);
		}
		public abstract AppearanceStyle GetBatchEditCellStyle();
		public abstract AppearanceStyle GetBatchEditModifiedCellStyle();
		public AppearanceStyle GetBatchEditMergedModifiedCellStyle() {
			return MergeStyle<GridViewCellStyle>(GetBatchEditCellStyle(), GetBatchEditModifiedCellStyle());
		}
		public AppearanceStyle GetFilterBarLinkStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.FilterBarLinkStyleName, Styles.FilterBarLink);
		}
		public AppearanceStyle GetFilterBarCheckBoxCellStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.FilterBarCheckBoxCellStyleName, Styles.FilterBarCheckBoxCell);
		}
		public AppearanceStyle GetFilterBarImageCellStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.FilterBarImageCellStyleName, Styles.FilterBarImageCell);
		}
		public AppearanceStyle GetFilterBarExpressionCellStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.FilterBarExpressionCellStyleName, Styles.FilterBarExpressionCell);
		}
		public AppearanceStyle GetFilterBarClearButtonCellStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.FilterBarClearButtonCellStyleName, Styles.FilterBarClearButtonCell);
		}
		public AppearanceStyle GetFilterBuilderMainAreaStyle() {
			return MergeStyle<AppearanceStyle>(GridViewStyles.FilterBuilderMainAreaStyleName, StylesPopup.FilterBuilder.MainArea);
		}
		public AppearanceStyle GetFilterBuilderButtonAreaStyle() {
			return MergeStyle<AppearanceStyle>(GridViewStyles.FilterBuilderButtonAreaStyleName, StylesPopup.FilterBuilder.ButtonPanel);
		}
		public AppearanceStyle GetFilterBuilderPopupHeaderStyle() {
			return MergeStyle<AppearanceStyle>(StylesPopup.Common.Header, StylesPopup.FilterBuilder.Header);
		}
		public AppearanceStyleBase GetFilterBuilderPopupModalBackgroundStyle() {
			return MergeStyle<AppearanceStyle>(StylesPopup.Common.ModalBackground, StylesPopup.FilterBuilder.ModalBackground);
		}
		public AppearanceStyle GetFilterBuilderPopupCloseButtonStyle() {
			return MergeStyle<AppearanceStyle>(StylesPopup.Common.CloseButton, StylesPopup.FilterBuilder.CloseButton);
		}
		public AppearanceStyle GetHeaderFilterPopupControlStyle() {
			return MergeStyle<AppearanceStyle>(StylesPopup.Common.Style, StylesPopup.HeaderFilter.Style);
		}
		public AppearanceStyle GetHeaderFilterPopupContentStyle() {
			return MergeStyle<AppearanceStyle>(StylesPopup.Common.Content, StylesPopup.HeaderFilter.Content);
		}
		public AppearanceStyle GetHeaderFilterPopupFooterStyle() {
			return MergeStyle<AppearanceStyle>(StylesPopup.Common.Footer, StylesPopup.HeaderFilter.Footer); 
		}
		public AppearanceStyle GetHeaderFilterPopupItemStyle() {
			return MergeStyle<ListBoxItemStyle>(StylesEditors.ListBoxItem, Styles.HeaderFilterItem);
		}
		public AppearanceStyle GetCustomizationStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.CustomizationStyleName, StylesPopup.CustomizationWindow.MainArea);
		}
		public AppearanceStyle GetCustomizationWindowControlStyle() {
			return MergeStyle<AppearanceStyle>(StylesPopup.Common.Style, StylesPopup.CustomizationWindow.Style);
		}
		public PopupWindowButtonStyle GetCustomizationWindowCloseButtonStyle() {
			return MergeStyle<PopupWindowButtonStyle>(StylesPopup.Common.CloseButton, StylesPopup.CustomizationWindow.CloseButton);
		}
		public PopupWindowStyle GetCustomizationWindowHeaderStyle() {
			return MergeStyle<PopupWindowStyle>(StylesPopup.Common.Header, StylesPopup.CustomizationWindow.Header);
		}
		public PopupWindowContentStyle GetCustomizationWindowContentStyle() {
			return MergeStyle<PopupWindowContentStyle>(StylesPopup.Common.Content, StylesPopup.CustomizationWindow.Content);
		}
		public AppearanceStyle GetPopupEditFormStyle() {
			return MergeStyle<AppearanceStyle>(GridStyles.PopupEditFormStyleName, StylesPopup.EditForm.MainArea);
		}
		public AppearanceStyle GetPopupEditFormControlStyle() {
			return MergeStyle<AppearanceStyle>(StylesPopup.Common.Style, StylesPopup.EditForm.Style);
		}
		public PopupWindowStyle GetPopupEditFormHeaderStyle() {
			return MergeStyle<PopupWindowStyle>(StylesPopup.Common.Header, StylesPopup.EditForm.Header);
		}
		public PopupWindowContentStyle GetPopupEditFormContentStyle() {
			return MergeStyle<PopupWindowContentStyle>(StylesPopup.Common.Content, StylesPopup.EditForm.Content);
		}
		public PopupWindowButtonStyle GetPopupEditFormCloseButtonStyle() {
			return MergeStyle<PopupWindowButtonStyle>(StylesPopup.Common.CloseButton, StylesPopup.EditForm.CloseButton);
		}
		public PopupControlModalBackgroundStyle GetPopupEditFormModalBackgroundStyle() {
			return MergeStyle<PopupControlModalBackgroundStyle>(StylesPopup.Common.ModalBackground, StylesPopup.EditForm.ModalBackground);
		}
		public LoadingDivStyle GetLoadingDivStyle() {
			return MergeStyle<LoadingDivStyle>(GridStyles.LoadingDivStyleName, Styles.LoadingDivInternal);
		}
		public LoadingPanelStyle GetLoadingPanelStyle() {
			var style = new LoadingPanelStyle();
			style.CopyFontFrom(GetRootTableStyle());
			string styleName = Grid.SettingsLoadingPanel.Mode == GridViewLoadingPanelMode.ShowOnStatusBar ? GridStyles.StatusBarLoadingPanelStyleName : GridStyles.LoadingPanelStyleName;
			return MergeStyle<LoadingPanelStyle>(styleName, style, Styles.LoadingPanelInternal);
		}
		public AppearanceStyleBase GetHFSelectAllCellStyle() {
			var style = new AppearanceStyleBase();
			style.CssClass = Styles.HFSelectAllCellClassName;
			style.CopyFrom(Styles.HeaderFilterItem);
			return style;
		}
	}
	public static class GridClientStateProperties {
		public const string
			SelectedState = "selection",
			PageKeysState = "keys",
			FocusedRowState = "focusedRow",
			FocusedKeyState = "focusedKey",
			ResizingState = "resizingState",
			ScrollState = "scrollState",
			LastMultiSelectIndex = "lastMultiSelectIndex",
			EndlessPagingGroupState = "endlessPagingGroupState",
			BatchEditClientModifiedValues = "batchEditClientModifiedValues";
	}
	public abstract class GridClientStylesInfo {
		public GridClientStylesInfo(ASPxGridBase grid) {
			Grid = grid;
		}
		protected ASPxGridBase Grid { get; private set; }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected abstract AppearanceStyle GetSelectedItemStyle();
		protected abstract AppearanceStyle GetFocusedItemStyle();
		public string ToJSON() {
			var styleInfo = new Dictionary<string, object>();
			PopulateStyleInfo(styleInfo);
			return HtmlConvertor.ToJSON(styleInfo);
		}
		protected virtual void PopulateStyleInfo(Dictionary<string, object> styleInfo) {
			styleInfo["sel"] = GetStyleInfo(GetSelectedItemStyle());
			styleInfo["fi"] = GetStyleInfo(GetFocusedItemStyle());
			styleInfo["bec"] = GetStyleInfo(RenderHelper.GetBatchEditCellStyle(), true);
			styleInfo["bemc"] = GetStyleInfo(RenderHelper.GetBatchEditModifiedCellStyle(), true);
			styleInfo["bemergmc"] = GetStyleInfo(RenderHelper.GetBatchEditMergedModifiedCellStyle(), true);
			styleInfo["ei"] = RenderUtils.GetRenderResult(CreateErrorItemControl());
		}
		protected abstract WebControl CreateErrorItemControl();
		IEnumerable<string> GetCssClassList(string cssClass) {
			if(string.IsNullOrEmpty(cssClass))
				return Enumerable.Empty<string>();
			return cssClass.Split(' ').Where(c => !string.IsNullOrEmpty(c));
		}
		bool IsItemSeletedOrFocused(int visibleIndex) {
			return Grid.Selection.IsRowSelected(visibleIndex) || Grid.FocusedRowIndex == visibleIndex;
		}
		protected object GetStyleInfo(AppearanceStyleBase style) {
			return GetStyleInfo(style, false);
		}
		protected object GetStyleInfo(AppearanceStyleBase style, bool requireAppendGridCssClassName) {
			var control = RenderUtils.CreateDiv();
			style.AssignToControl(control, true);
			if(requireAppendGridCssClassName)
				RenderHelper.AppendGridCssClassName(control);
			return GetControlStyleInfo(control);
		}
		protected object GetControlStyleInfo(WebControl control){
			return GetStyleInfo(GetStyleAttributes(control), control.CssClass);
		}
		protected static object GetStyleInfo(CssStyleCollection style, string cssClass) {
			var info = new Hashtable();
			if(style.Count > 0)
				info["style"] = style.Value;
			if(!string.IsNullOrEmpty(cssClass))
				info["css"] = cssClass;
			return info;
		}
		CssStyleCollection GetStyleAttributes(WebControl control) {
			CssStyleCollection styleAttributes = control.ControlStyle.GetStyleAttributes(Grid);
			CssStyleCollection style = control.Style;
			foreach(string key in style.Keys) {
				styleAttributes.Add(key, style[key]);
			}
			return styleAttributes;
		}
	}
	public delegate Control TemplateContainerFinder(Control container, object parameters, string id);
	public class TemplateContainerCollection : List<Control> {
		public TemplateContainerCollection(ASPxGridBase grid) {
			Grid = grid;
		}
		protected ASPxGridBase Grid { get; private set; }
		public Control FindChild(TemplateContainerFinder finder, object parameters, string id) {
			Grid.EnsureChildControlsCore();
			foreach(Control control in this) {
				Control res = finder(control, parameters, id);
				if(res != null) return res;
			}
			return null;
		}
		internal Dictionary<string, object> FindTwoWayBindings(ITemplate template) {
			IBindableTemplate bindable = template as IBindableTemplate;
			if(bindable == null) return null;
			Grid.EnsureChildControlsCore();
			if(Count == 0) return null;
			Dictionary<string, object> res = new Dictionary<string, object>();
			foreach(Control control in this) {
				IOrderedDictionary values = bindable.ExtractValues(control);
				if(values == null || values.Count == 0) continue;
				foreach(DictionaryEntry entry in values) {
					res[ExtractTwoWayName(entry.Key)] = entry.Value;
				}
			}
			return res.Count == 0 ? null : res;
		}
		string ExtractTwoWayName(object key) {
			string result = key.ToString();
			if(result.StartsWith("[") && result.EndsWith("]"))
				result = result.Substring(1, result.Length - 2);
			return result;
		}
		internal Dictionary<string, object> FindTwoWayBindings(Dictionary<object, ITemplate> templates, TemplateContainerFinder finder) {
			Dictionary<string, object> res = new Dictionary<string, object>();
			foreach(KeyValuePair<object, ITemplate> pair in templates) {
				IBindableTemplate bindable = pair.Value as IBindableTemplate;
				if(bindable == null) continue;
				Control control = FindChild(finder, pair.Key, null);
				if(control == null) continue;
				IOrderedDictionary values = bindable.ExtractValues(control);
				if(values == null || values.Count == 0) continue;
				foreach(DictionaryEntry entry in values) {
					res[ExtractTwoWayName(entry.Key)] = entry.Value;
				}
			}
			return res.Count == 0 ? null : res;
		}
	}
	internal class SimpleValueProvider : IValueProvider {
		int visibleIndex;
		WebDataProxy data;
		public SimpleValueProvider(WebDataProxy data, int visibleIndex) {
			this.data = data;
			this.visibleIndex = visibleIndex;
		}
		object IValueProvider.GetValue(string fieldName) {
			return data.GetRowValue(visibleIndex, fieldName);
		}
	}
	public class GridDataHelper : DataHelper {
		DataSourceSelectArguments lastArguments = new DataSourceSelectArguments();
		public GridDataHelper(ASPxGridBase grid, string name)
			: base(grid, name) {
		}
		internal DataSourceSelectArguments LastArguments { get { return lastArguments; } }
		protected ASPxGridBase Grid { get { return (ASPxGridBase)Control; } }
		public override void PerformSelect() {
			if(Grid.DesignMode && !string.IsNullOrEmpty(Grid.DataSourceID)) return;
			Grid.RaiseBeforePerformDataSelect();
			ResetSelectArguments();
			this.lastArguments = SelectArguments;
			base.PerformSelect();
		}
		public int RetrieveTotalCount() {
			if(Grid.DesignMode) return 0;
			DataSourceSelectArguments select = new DataSourceSelectArguments();
			select.RetrieveTotalRowCount = true;
			select.MaximumRows = 0;
			PerformSelectCore(select, null);
			return select.TotalRowCount;
		}
		public int RetrievePageIndex() {
			var settings = Grid.DataProxy.PageSettings;
			int pageIndex = settings.PageIndex;
			if(!Grid.IsAllowDataSourcePaging() || pageIndex < 1)
				return pageIndex;
			int totalCount = RetrieveTotalCount();
			int pageCount = totalCount / settings.PageSize;
			if(totalCount % settings.PageSize > 0)
				pageCount++;
			if(pageIndex >= pageCount)
				pageIndex = pageCount - 1;
			return pageIndex;
		}
		string GetSortExpression() {
			Grid.BuildSortedColumns();
			string res = string.Empty;
			foreach(var dc in Grid.SortedColumns) {
				if(res.Length > 0) res += ",";
				string field = dc.FieldName;
				if(field.IndexOf(' ') > -1) field = "[" + field + "]";
				res += string.Format("{0} {1}", field, dc.SortOrder == ColumnSortOrder.Ascending ? "ASC" : "DESC");
			}
			return res;
		}
		protected override DataSourceSelectArguments CreateDataSourceSelectArguments() {
			if(!Grid.IsAllowDataSourcePaging())
				return base.CreateDataSourceSelectArguments();
			string sortExpression = GetSortExpression();
			int startIndex = 0;
			int count = -1;
			IWebControlPageSettings settings = Grid.DataProxy.PageSettings;
			if(settings.PageSize > -1) {
				if(settings.PageIndex > -1) {
					count = settings.PageSize;
					startIndex = settings.PageIndex * settings.PageSize;
				} else {
					count = RetrieveTotalCount();
					startIndex = 0;
				}
			}
			DataSourceSelectArguments args = new DataSourceSelectArguments(sortExpression, startIndex, count);
			args.RetrieveTotalRowCount = true;
			return args;
		}
		protected override void OnDataSourceViewChanged() {
			if(IsDetailGrid())
				return;
			base.OnDataSourceViewChanged();
		}
		bool IsDetailGrid() {
			Control current = Control;
			while(true) {
				current = current.NamingContainer;
				if(current == null) break;
				if(current is TemplateContainerBase)
					return true;
			}
			return false;
		}
	}
}

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Data;
using System.Windows.Data;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Collections;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation; 
using DevExpress.XtraEditors.DXErrorProvider;
using System.Windows.Markup;
using System.Globalization;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Helpers;
using System.Collections.ObjectModel;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Serialization;
using System.Text;
using DevExpress.Mvvm;
using DevExpress.Core;
using System.IO;
using DevExpress.XtraPrinting;
using System.Windows.Threading;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Grid.Printing;
using System.Printing;
using DevExpress.Xpf.Data.Native;
using DevExpress.Xpf.Grid.Themes;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.Native;
using DialogService = DevExpress.Xpf.Core.DialogService;
using IDialogService = DevExpress.Mvvm.IDialogService;
using TableViewAssignableDialogServiceHelper = DevExpress.Mvvm.UI.Native.AssignableServiceHelper2<DevExpress.Xpf.Grid.TableView, DevExpress.Mvvm.IDialogService>;
using System.Linq;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraExport.Helpers;
using DevExpress.Utils;
using DevExpress.Export;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.XtraExport;
using DevExpress.Xpf.Grid.EditForm;
namespace DevExpress.Xpf.Grid {
	public partial class TableView : IGridViewFactory<ColumnWrapper, RowBaseWrapper> {
		public static readonly DependencyProperty AllowCellMergeProperty = DependencyPropertyManager.Register("AllowCellMerge", typeof(bool), typeof(TableView), new PropertyMetadata(false, (d, e) => ((TableView)d).OnAllowCellMergeChanged()));
		static partial void RegisterClassCommandBindings() {
			Type ownerType = typeof(TableView);
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(GridCommands.BestFitColumn, (d, e) => ((TableView)d).BestFitColumn(e), (d, e) => ((TableView)d).OnCanBestFitColumn(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(GridCommands.BestFitColumns, (d, e) => ((TableView)d).BestFitColumns(), (d, e) => ((TableView)d).OnCanBestFitColumns(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(GridCommands.AddNewRow, (d, e) => ((TableView)((DataViewBase)d).MasterRootRowsContainer.FocusedView).AddNewRow(), (d, e) => e.CanExecute = ((TableView)d).CanAddNewRow()));			
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowCellMerge"),
#endif
 Category(Categories.OptionsLayout), XtraSerializableProperty]
		public bool AllowCellMerge {
			get { return (bool)GetValue(AllowCellMergeProperty); }
			set { SetValue(AllowCellMergeProperty, value); }
		}
		[Category(Categories.OptionsLayout)]
		public event CellMergeEventHandler CellMerge;
		protected internal override int FindRowHandle(DependencyObject element) {
			if(ActualAllowCellMerge) {
				TableView rootView = (TableView)RootView;
				Point point = rootView.ScrollContentPresenter.TranslatePoint(rootView.TableViewBehavior.LastMousePosition, rootView);
				TableViewHitInfo hitInfo = rootView.CalcHitInfo(point);
				if(hitInfo.InRowCell)
					return hitInfo.RowHandle;
			}
			return base.FindRowHandle(element);
		}
		#region commands
		void BestFitColumn(ExecutedRoutedEventArgs e) {
			TableViewBehavior.BestFitColumn(e.Parameter);
		}
		void OnCanBestFitColumn(CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanBestFitColumn(e.Parameter);
		}
		void OnCanBestFitColumns(CanExecuteRoutedEventArgs e) {
			e.CanExecute = TableViewBehavior.CanBestFitColumns();
		}
		#endregion
		#region Print
		internal bool ShouldPrintColumnHeaders { get { return ShowColumnHeaders && PrintColumnHeaders; } }
		#endregion
		#region Export
		#region CSV
		internal CsvExportHelper CsvHelper { get { return new CsvExportHelper(this, ExportTarget.Csv, PrintHelper.ExportToCsv, PrintHelper.ExportToCsv, PrintHelper.ExportToCsv, PrintHelper.ExportToCsv); } }
		public void ExportToCsv(Stream stream) {
			CsvHelper.Export(stream);
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			CsvHelper.Export(stream, options);
		}
		public void ExportToCsv(string filePath) {
			CsvHelper.Export(filePath);
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			CsvHelper.Export(filePath, options);
		}
		#endregion
		#region XLS
		internal XlsExportHelper<XlsExportOptions> XlsHelper { get { return new XlsExportHelper<XlsExportOptions>(this, ExportTarget.Xls, PrintHelper.ExportToXls, PrintHelper.ExportToXls, PrintHelper.ExportToXls, PrintHelper.ExportToXls); } }
		public void ExportToXls(Stream stream) {
			XlsHelper.Export(stream);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			XlsHelper.Export(stream, options);
		}
		public void ExportToXls(string filePath) {
			XlsHelper.Export(filePath);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			XlsHelper.Export(filePath, options);
		}
		#endregion
		#region XLSX
		internal XlsExportHelper<XlsxExportOptions> XlsxHelper { get { return new XlsExportHelper<XlsxExportOptions>(this, ExportTarget.Xlsx, PrintHelper.ExportToXlsx, PrintHelper.ExportToXlsx, PrintHelper.ExportToXlsx, PrintHelper.ExportToXlsx); } }
		public void ExportToXlsx(Stream stream) {
			XlsxHelper.Export(stream);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			XlsxHelper.Export(stream, options);
		}
		public void ExportToXlsx(string filePath) {
			XlsxHelper.Export(filePath);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			XlsxHelper.Export(filePath, options);
		}
		#endregion
		#endregion
		#region Lightweight templates
		public static readonly DependencyProperty UseLightweightTemplatesProperty;
		public static readonly DependencyProperty RowDetailsTemplateProperty;
		public static readonly DependencyProperty RowDetailsTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualRowDetailsTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualRowDetailsTemplateSelectorProperty;
		public static readonly DependencyProperty RowDetailsVisibilityModeProperty;
		public UseLightweightTemplates? UseLightweightTemplates {
			get { return (UseLightweightTemplates?)GetValue(UseLightweightTemplatesProperty); }
			set { SetValue(UseLightweightTemplatesProperty, value); }
		}
		public DataTemplate RowDetailsTemplate {
			get { return (DataTemplate)GetValue(RowDetailsTemplateProperty); }
			set { SetValue(RowDetailsTemplateProperty, value); }
		}
		public DataTemplateSelector RowDetailsTemplateSelector {
			get { return (DataTemplateSelector)GetValue(RowDetailsTemplateSelectorProperty); }
			set { SetValue(RowDetailsTemplateSelectorProperty, value); }
		}
		public DataTemplateSelector ActualRowDetailsTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualRowDetailsTemplateSelectorProperty); }
		}
		DependencyPropertyKey ITableView.ActualRowDetailsTemplateSelectorPropertyKey { get { return ActualRowDetailsTemplateSelectorPropertyKey; } }
		public RowDetailsVisibilityMode RowDetailsVisibilityMode {
			get { return (RowDetailsVisibilityMode)GetValue(RowDetailsVisibilityModeProperty); }
			set { SetValue(RowDetailsVisibilityModeProperty, value); }
		}
		internal override FrameworkElement CreateRowElement(RowData rowData) {
			return TableViewBehavior.CreateElement(() => new RowControl(rowData), () => base.CreateRowElement(rowData), DevExpress.Xpf.Grid.UseLightweightTemplates.Row);
		}
		bool ITableView.UseRowDetailsTemplate(int rowHandle) {
			return TableViewBehavior.UseRowDetailsTemplate(rowHandle);
		}
		#endregion
		#region Format conditions
		public static readonly DependencyProperty AllowConditionalFormattingMenuProperty = TableViewBehavior.RegisterAllowConditionalFormattingMenuProperty(typeof(TableView));
		public static readonly DependencyProperty AllowConditionalFormattingManagerProperty = TableViewBehavior.RegisterAllowConditionalFormattingManagerProperty(typeof(TableView));
		public static readonly DependencyProperty PredefinedFormatsProperty = TableViewBehavior.RegisterPredefinedFormatsProperty(typeof(TableView));
		public static readonly DependencyProperty PredefinedColorScaleFormatsProperty = TableViewBehavior.RegisterPredefinedColorScaleFormatsProperty(typeof(TableView));
		public static readonly DependencyProperty PredefinedDataBarFormatsProperty = TableViewBehavior.RegisterPredefinedDataBarFormatsProperty(typeof(TableView));
		public static readonly DependencyProperty PredefinedIconSetFormatsProperty = TableViewBehavior.RegisterPredefinedIconSetFormatsProperty(typeof(TableView));
		public static readonly DependencyProperty FormatConditionDialogServiceTemplateProperty = TableViewAssignableDialogServiceHelper.RegisterServiceTemplateProperty("FormatConditionDialogServiceTemplate");
		public static readonly DependencyProperty ConditionalFormattingManagerServiceTemplateProperty = TableViewAssignableDialogServiceHelper.RegisterServiceTemplateProperty("ConditionalFormattingManagerServiceTemplate");
		[Category(Categories.Appearance), XtraSerializableProperty(true, false, false), GridUIProperty, XtraResetProperty]
		public FormatConditionCollection FormatConditions { get { return TableViewBehavior.FormatConditions; } }
		[XtraSerializableProperty]
		public bool AllowConditionalFormattingMenu {
			get { return (bool)GetValue(AllowConditionalFormattingMenuProperty); }
			set { SetValue(AllowConditionalFormattingMenuProperty, value); }
		}
		[XtraSerializableProperty]
		public bool AllowConditionalFormattingManager {
			get { return (bool)GetValue(AllowConditionalFormattingManagerProperty); }
			set { SetValue(AllowConditionalFormattingManagerProperty, value); }
		}
		public FormatInfoCollection PredefinedFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedFormatsProperty); }
			set { SetValue(PredefinedFormatsProperty, value); }
		}
		public FormatInfoCollection PredefinedColorScaleFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedColorScaleFormatsProperty); }
			set { SetValue(PredefinedColorScaleFormatsProperty, value); }
		}
		public FormatInfoCollection PredefinedDataBarFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedDataBarFormatsProperty); }
			set { SetValue(PredefinedDataBarFormatsProperty, value); }
		}
		public FormatInfoCollection PredefinedIconSetFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedIconSetFormatsProperty); }
			set { SetValue(PredefinedIconSetFormatsProperty, value); }
		}
		public DataTemplate FormatConditionDialogServiceTemplate {
			get { return (DataTemplate)GetValue(FormatConditionDialogServiceTemplateProperty); }
			set { SetValue(FormatConditionDialogServiceTemplateProperty, value); }
		}
		public DataTemplate ConditionalFormattingManagerServiceTemplate {
			get { return (DataTemplate)GetValue(ConditionalFormattingManagerServiceTemplateProperty); }
			set { SetValue(ConditionalFormattingManagerServiceTemplateProperty, value); }
		}
		public void AddFormatCondition(FormatConditionBase formatCondition) {
			TableViewBehavior.AddFormatConditionCore(formatCondition);
		}
		public void ShowFormatConditionDialog(ColumnBase column, FormatConditionDialogType dialogKind) {
			TableViewBehavior.ShowFormatConditionDialogCore(column, dialogKind);
		}
		public void ClearFormatConditionsFromAllColumns() {
			TableViewBehavior.ClearFormatConditionsFromAllColumnsCore();
		}
		public void ClearFormatConditionsFromColumn(ColumnBase column) {
			TableViewBehavior.ClearFormatConditionsFromColumnCore(column);
		}
		public void ShowConditionalFormattingManager(ColumnBase column) {
			TableViewBehavior.ShowConditionalFormattingManagerCore(column);
		}
		#endregion
		#region Serialization
		static partial void RegisterSerializationEvents() {
			Type ownerType = typeof(TableView);
			EventManager.RegisterClassHandler(ownerType, DXSerializer.CreateCollectionItemEvent, new XtraCreateCollectionItemEventHandler((s, e) => ((TableView)s).OnDeserializeCreateCollectionItem(e)));
		}
		protected virtual void OnDeserializeCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			if(e.CollectionName == "FormatConditions")
				TableViewBehavior.OnDeserializeCreateFormatCondition(e);
		}
		protected override void OnDeserializeStart(StartDeserializingEventArgs e) {
			base.OnDeserializeStart(e);
			TableViewBehavior.OnDeserializeFormatConditionsStart();
		}
		protected override void OnDeserializeEnd(EndDeserializingEventArgs e) {
			base.OnDeserializeEnd(e);
			TableViewBehavior.OnDeserializeFormatConditionsEnd();
		}
		#endregion
		#region ClipboardFormat
		GridViewClipboardHelper clipboardHelperManager;
		IClipboardManager<ColumnWrapper, RowBaseWrapper> _clipboardManager;
		IClipboardManager<ColumnWrapper, RowBaseWrapper> ClipboardManager {
			get {
				if(_clipboardManager == null)
					_clipboardManager = CreateClipboardManager();
				return _clipboardManager;
			}
		}
		IClipboardManager<ColumnWrapper, RowBaseWrapper> CreateClipboardManager() {
			clipboardHelperManager = new GridViewClipboardHelper(this);
			return (IClipboardManager<ColumnWrapper, RowBaseWrapper>)PrintHelper.ClipboardExportManagerInstance(typeof(ColumnWrapper), typeof(RowBaseWrapper), clipboardHelperManager);			
		}
		protected internal override bool SetDataAwareClipboardData() {
			try {
				SetActualClipboardOptions(OptionsClipboard);
				if(ClipboardManager != null && clipboardHelperManager != null && !clipboardHelperManager.CanCopyToClipboard())
					return false;			 
				System.Windows.Forms.DataObject data = new System.Windows.Forms.DataObject();
				ClipboardManager.AssignOptions(OptionsClipboard);
				ClipboardManager.SetClipboardData(data);
				if(data.GetFormats().Count() == 0)
					return false;
				Clipboard.SetDataObject(data);
				return true;
			} catch {
				return false;
			}
		}
		#endregion
		#region EditForm
		public static readonly DependencyProperty EditFormDialogServiceTemplateProperty = TableViewAssignableDialogServiceHelper.RegisterServiceTemplateProperty("EditFormDialogServiceTemplate");
		public static readonly DependencyProperty EditFormColumnCountProperty = DependencyProperty.Register("EditFormColumnCount", typeof(int), typeof(TableView), new PropertyMetadata(3, (d, e) => ((TableView)d).HideEditForm()));
		public static readonly DependencyProperty EditFormPostModeProperty = DependencyProperty.Register("EditFormPostMode", typeof(EditFormPostMode), typeof(TableView), new PropertyMetadata(EditFormPostMode.Cached, (d, e) => ((TableView)d).HideEditForm()));
		public static readonly DependencyProperty EditFormShowModeProperty = DependencyProperty.Register("EditFormShowMode", typeof(EditFormShowMode), typeof(TableView), new PropertyMetadata(EditFormShowMode.None, (d, e) => ((TableView)d).HideEditForm()));
		public static readonly DependencyProperty ShowEditFormOnF2KeyProperty = DependencyProperty.Register("ShowEditFormOnF2Key", typeof(bool), typeof(TableView), new PropertyMetadata(true));
		public static readonly DependencyProperty ShowEditFormOnEnterKeyProperty = DependencyProperty.Register("ShowEditFormOnEnterKey", typeof(bool), typeof(TableView), new PropertyMetadata(true));
		public static readonly DependencyProperty ShowEditFormOnDoubleClickProperty = DependencyProperty.Register("ShowEditFormOnDoubleClick", typeof(bool), typeof(TableView), new PropertyMetadata(true));
		public static readonly DependencyProperty EditFormPostConfirmationProperty = DependencyProperty.Register("EditFormPostConfirmation", typeof(PostConfirmationMode), typeof(TableView), new PropertyMetadata(PostConfirmationMode.YesNoCancel, (d, e) => ((TableView)d).HideEditForm()));
		public static readonly DependencyProperty ShowEditFormUpdateCancelButtonsProperty = DependencyProperty.Register("ShowEditFormUpdateCancelButtons", typeof(bool), typeof(TableView), new PropertyMetadata(true, (d, e) => ((TableView)d).HideEditForm()));
		public static readonly DependencyProperty EditFormTemplateProperty = DependencyProperty.Register("EditFormTemplate", typeof(DataTemplate), typeof(TableView), new PropertyMetadata(null, (d, e) => ((TableView)d).HideEditForm()));
		public DataTemplate EditFormDialogServiceTemplate {
			get { return (DataTemplate)GetValue(EditFormDialogServiceTemplateProperty); }
			set { SetValue(EditFormDialogServiceTemplateProperty, value); }
		}
		public int EditFormColumnCount {
			get { return (int)GetValue(EditFormColumnCountProperty); }
			set { SetValue(EditFormColumnCountProperty, value); }
		}
		public EditFormPostMode EditFormPostMode {
			get { return (EditFormPostMode)GetValue(EditFormPostModeProperty); }
			set { SetValue(EditFormPostModeProperty, value); }
		}
		public EditFormShowMode EditFormShowMode {
			get { return (EditFormShowMode)GetValue(EditFormShowModeProperty); }
			set { SetValue(EditFormShowModeProperty, value); }
		}
		public bool ShowEditFormOnF2Key {
			get { return (bool)GetValue(ShowEditFormOnF2KeyProperty); }
			set { SetValue(ShowEditFormOnF2KeyProperty, value); }
		}
		public bool ShowEditFormOnEnterKey {
			get { return (bool)GetValue(ShowEditFormOnEnterKeyProperty); }
			set { SetValue(ShowEditFormOnEnterKeyProperty, value); }
		}
		public bool ShowEditFormOnDoubleClick {
			get { return (bool)GetValue(ShowEditFormOnDoubleClickProperty); }
			set { SetValue(ShowEditFormOnDoubleClickProperty, value); }
		}
		public PostConfirmationMode EditFormPostConfirmation {
			get { return (PostConfirmationMode)GetValue(EditFormPostConfirmationProperty); }
			set { SetValue(EditFormPostConfirmationProperty, value); }
		}
		public BindingBase EditFormCaptionBinding { get; set; }
		public bool ShowEditFormUpdateCancelButtons {
			get { return (bool)GetValue(ShowEditFormUpdateCancelButtonsProperty); }
			set { SetValue(ShowEditFormUpdateCancelButtonsProperty, value); }
		}
		public DataTemplate EditFormTemplate {
			get { return (DataTemplate)GetValue(EditFormTemplateProperty); }
			set { SetValue(EditFormTemplateProperty, value); }
		}
		public void ShowDialogEditForm() {
			TableViewEditFormManager.ShowDialogEditForm();
		}
		public void ShowInlineEditForm() {
			TableViewEditFormManager.ShowInlineEditForm();
		}
		public void ShowEditForm() {
			TableViewEditFormManager.ShowEditForm();
		}
		public void HideEditForm() {
			TableViewEditFormManager.HideEditForm();
		}
		public void CloseEditForm() {
			TableViewEditFormManager.CloseEditForm();
		}
		internal EditFormManager TableViewEditFormManager { get { return EditFormManager as EditFormManager; } }
		internal protected override IEditFormManager CreateEditFormManager() {
			return new EditFormManager(this);
		}
		internal protected override IEditFormOwner CreateEditFormOwner() {
			return new EditFormOwner(this);
		}
		#endregion
		void OnAllowCellMergeChanged() {
			UpdateActualAllowCellMergeCore();
			UpdateCellMergingPanels();
		}
		protected internal override void UpdateActualAllowCellMergeCore() {
			ActualAllowCellMerge = (AllowCellMerge || (DataControl != null && DataControl.countColumnCellMerge > 0)) && !IsMultiSelection && NavigationStyle != GridViewNavigationStyle.Row;
		}
		protected override void OnActualAllowCellMergeChanged() {
			OnMultiSelectModeChanged();
			TableViewBehavior.UpdateViewRowData(rowData => {
				rowData.UpdateSelectionState();
				rowData.UpdateCellsPanel();
			});
		}
		protected internal override void UpdateCellMergingPanels() {
			UpdateAllRowData(data => {
				if(!data.View.ActualAllowCellMerge) return;
				data.InvalidateCellsPanel();
				data.UpdateIsFocusedCell();
			});
		}
		void UpdateAllRowData(UpdateRowDataDelegate updateMethod) {
			if(DataControl!= null)
				DataControl.UpdateAllDetailDataControls(dataControl => dataControl.viewCore.UpdateRowData(updateMethod, false, false));
		}
		protected override void OnFocusedRowHandleChangedCore(int oldRowHandle) {
			base.OnFocusedRowHandleChangedCore(oldRowHandle);
			if(isEditorOpen || (RowDetailsVisibilityMode != RowDetailsVisibilityMode.Collapsed && (RowDetailsTemplate != null || RowDetailsTemplateSelector != null)))
				UpdateCellMergingPanels();
		}
		internal readonly Locker CellMergeLocker = new Locker();
		protected override bool IsCellMerged(int visibleIndex1, int visibleIndex2, ColumnBase column, bool checkRowData, int checkMDIndex) {
			bool result = false;
			CellMergeLocker.DoLockedActionIfNotLocked(delegate {
				result = IsCellMergeCore(visibleIndex1, visibleIndex2, column, checkRowData, checkMDIndex);
			});
			return result;
		}
		bool IsCellMergeCore(int visibleIndex1, int visibleIndex2, ColumnBase column, bool checkRowData, int checkMDIndex) {
			if(column.AllowCellMerge.HasValue ? !column.AllowCellMerge.Value : !AllowCellMerge) return false;
			int checkHandle = DataControl.GetRowHandleByVisibleIndexCore(checkMDIndex);
			if(DataControl.MasterDetailProvider.IsMasterRowExpanded(checkHandle) || ViewBehavior.UseRowDetailsTemplate(checkHandle))
				return false;
			int handle = DataControl.GetRowHandleByVisibleIndexCore(visibleIndex2);
			if(DataControl.IsGroupRowHandleCore(handle) || !DataControl.IsValidRowHandleCore(handle)) return false;
			if(checkRowData) {
				RowData rowData = GetRowData(handle);
				if(rowData == null || !rowData.IsRowInView())
					return false;
			}
			int rowHandle2 = DataControl.GetRowHandleByVisibleIndexCore(visibleIndex1);
			if(!AllowMergeEditor(column, handle, rowHandle2)) return false;
			if(handle == DataControlBase.NewItemRowHandle || rowHandle2 == DataControlBase.NewItemRowHandle) return false;
			return RaiseCellMerge(column, handle, rowHandle2, true).Value;
		}
		protected virtual bool AllowMergeEditor(ColumnBase column, int rowHandle1, int rowHandle2) {
			if(!isEditorOpen || !IsKeyboardFocusWithin || IsKeyboardFocusInSearchPanel())
				return true;
			if(rowHandle1 != FocusedRowHandle && rowHandle2 != FocusedRowHandle)
				return true;
			if(column != DataControl.CurrentColumn)
				return true;
			return !CanShowEditor(FocusedRowHandle, DataControl.CurrentColumn);
		}
		CellMergeEventArgs cellMergeEventArgs;
		internal bool? RaiseCellMerge(ColumnBase column, int rowHandle1, int rowHandle2, bool checkValues) {
			object cellValue1 = DataControl.GetCellValue(rowHandle1, column.FieldName);
			object cellValue2 = DataControl.GetCellValue(rowHandle2, column.FieldName);
			TableView tableView = OriginationView == null ? this : OriginationView as TableView;
			if(tableView != null && tableView.CellMerge != null) {
				if(cellMergeEventArgs == null)
					cellMergeEventArgs = new CellMergeEventArgs();
				cellMergeEventArgs.SetArgs((GridColumn)column, rowHandle1, rowHandle2, cellValue1, cellValue2);
				tableView.CellMerge(this, cellMergeEventArgs);
				if(cellMergeEventArgs.Handled)
					return cellMergeEventArgs.Merge;
			}
			if(checkValues)
				return object.Equals(cellValue1, cellValue2);
			return null;
		}
		bool isEditorOpen;
		protected internal override void OnOpeningEditor() {
			if(!ActualAllowCellMerge) return;
			isEditorOpen = true;
			UpdateFocusAndInvalidatePanels();
		}
		protected internal override void OnHideEditor(CellEditorBase editor, bool closeEditor) {
			base.OnHideEditor(editor, closeEditor);
			if(!ActualAllowCellMerge) return;
			isEditorOpen = !closeEditor;
			UpdateFocusAndInvalidatePanels();
		}
		void UpdateFocusAndInvalidatePanels() {
			UpdateCellMergingPanels();
			ForceUpdateRowsState();
		}
		IGridView<ColumnWrapper, RowBaseWrapper> IGridViewFactory<ColumnWrapper, RowBaseWrapper>.GetIViewImplementerInstance() {
			return new GridViewReportHelper<ColumnWrapper, RowBaseWrapper>(this, ExportTarget.Xlsx);
		}
		void IGridViewFactory<ColumnWrapper, RowBaseWrapper>.ReleaseIViewImplementerInstance(IGridView<ColumnWrapper, RowBaseWrapper> instance) {
			instance.With(inst => inst as IDisposable).Do(disp => disp.Dispose());
		}
		object IGridViewFactory<ColumnWrapper, RowBaseWrapper>.GetDataSource() {
			return GridReportHelper.GetSource(this);
		}
		string IGridViewFactory<ColumnWrapper, RowBaseWrapper>.GetDataMember() {
			return string.Empty;
		}
	}
	internal class ExportHelper<T> where T : ExportOptionsBase {
		public TableView View { get; private set; }
		public ExportTarget ExportTarget { get; private set; }
		Action<TableView, Stream> ExportToStream;
		Action<TableView, Stream, T> ExportToStreamWithOptions;
		Action<TableView, string> ExportToFile;
		Action<TableView, string, T> ExportToFileWithOptions;
		public ExportHelper(TableView view, ExportTarget exportTarget, Action<TableView, Stream> exportToStream, Action<TableView, Stream, T> exportToStreamWithOptions, Action<TableView, string> exportToFile, Action<TableView, string, T> exportToFileWithOptions) {
			View = view;
			this.ExportToStream = exportToStream;
			this.ExportToStreamWithOptions = exportToStreamWithOptions;
			this.ExportToFile = exportToFile;
			this.ExportToFileWithOptions = exportToFileWithOptions;
			this.ExportTarget = exportTarget;
		}
		public void Export(Stream stream) {
			Export(() => ExportToStream(View, stream), () => ExportData(stream, null), null);
		}
		public void Export(Stream stream, T options) {
			Export(() => ExportToStreamWithOptions(View, stream, options), () => ExportData(stream, options), options as IDataAwareExportOptions);
		}
		public void Export(string filePath) {
			Export(() => ExportToFile(View, filePath), () => ExportData(filePath, null), null);
		}
		public void Export(string filePath, T options) {
			Export(() => ExportToFileWithOptions(View, filePath, options), () => ExportData(filePath, options), options as IDataAwareExportOptions);
		}
		static void Export(Action layoutExport, Action dataExport, IDataAwareExportOptions options) {
			if(IsDataAwareExport(options))
				dataExport();
			else
				layoutExport();
		}
		static bool IsDataAwareExport(IDataAwareExportOptions options) {
			ExportType exportType = DevExpress.Export.ExportSettings.DefaultExportType;
			if(options != null)
				exportType = options.ExportType;
			return exportType == ExportType.DataAware;
		}
		void ExportData(Action<GridViewExcelExporter<ColumnWrapper, RowBaseWrapper>> exportAction, T options) {
			using(GridViewExportHelper<ColumnWrapper, RowBaseWrapper> wrapper = new GridViewExportHelper<ColumnWrapper, RowBaseWrapper>(View, ExportTarget)) {
				var exporter = new GridViewExcelExporter<ColumnWrapper, RowBaseWrapper>(wrapper, CreateExporterOptions(options));
				exportAction(exporter);
			}
		}
		internal IDataAwareExportOptions CreateExporterOptions(T printOptions) {
			IDataAwareExportOptions options = DataAwareExportOptionsFactory.Create(ExportTarget, printOptions);
				ApplyViewOptions(options);
			if(printOptions != null)
				ApplyPrintOptions(options, printOptions);
			ApplyDefaults(options);
			return options;
		}
		protected virtual void ApplyDefaults(IDataAwareExportOptions options) {
			options.InitDefaults();
		}
		protected virtual void ApplyViewOptions(IDataAwareExportOptions options) {
			options.AllowCellMerge = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.AllowCellMerge, true);
			options.ShowTotalSummaries = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.ShowTotalSummaries, View.ShouldPrintTotalSummary || View.ShouldPrintFixedTotalSummary);
			options.ShowColumnHeaders = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.ShowColumnHeaders, View.ShouldPrintColumnHeaders);
			options.AllowHorzLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.AllowHorzLines, View.ShowHorizontalLines);
			options.AllowVertLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.AllowVertLines, View.ShowVerticalLines);
			options.RightToLeftDocument = View.FlowDirection == FlowDirection.RightToLeft ? DefaultBoolean.True : DefaultBoolean.False;
		}
		protected virtual void ApplyPrintOptions(IDataAwareExportOptions target, T source) {
		}
		void ExportData(string filePath, T options) {
			ExportData(exporter => exporter.Export(filePath), options);
		}
		void ExportData(Stream stream, T options) {
			ExportData(exporter => exporter.Export(stream), options);
		}
	}
	internal class XlsExportHelper<T> : ExportHelper<T> where T : XlsExportOptionsBase {
		public XlsExportHelper(TableView view, ExportTarget exportTarget, Action<TableView, Stream> exportToStream, Action<TableView, Stream, T> exportToStreamWithOptions, Action<TableView, string> exportToFile, Action<TableView, string, T> exportToFileWithOptions)
			: base(view, exportTarget, exportToStream, exportToStreamWithOptions, exportToFile, exportToFileWithOptions) { }
		protected override void ApplyPrintOptions(IDataAwareExportOptions target, T source) {
			base.ApplyPrintOptions(target, source);
			target.AllowHyperLinks = DataAwareExportOptionsFactory.UpdateDefaultBoolean(target.AllowHyperLinks, source.ExportHyperlinks);
			target.SheetName = source.SheetName;
		}
	}
	internal class CsvExportHelper : ExportHelper<CsvExportOptions> {
		public CsvExportHelper(TableView view, ExportTarget exportTarget, Action<TableView, Stream> exportToStream, Action<TableView, Stream, CsvExportOptions> exportToStreamWithOptions, Action<TableView, string> exportToFile, Action<TableView, string, CsvExportOptions> exportToFileWithOptions)
			: base(view, exportTarget, exportToStream, exportToStreamWithOptions, exportToFile, exportToFileWithOptions) { }
		protected override void ApplyDefaults(IDataAwareExportOptions options) {
			options.AllowFixedColumnHeaderPanel = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.AllowFixedColumnHeaderPanel, true);
		}
		protected override void ApplyViewOptions(IDataAwareExportOptions options) {
			options.ShowColumnHeaders = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.ShowColumnHeaders, View.ShowColumnHeaders && View.PrintColumnHeaders);
			options.AllowHorzLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.AllowHorzLines, View.ShowHorizontalLines);
			options.AllowVertLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.AllowVertLines, View.ShowVerticalLines);
		}
		protected override void ApplyPrintOptions(IDataAwareExportOptions target, CsvExportOptions source) {
			base.ApplyPrintOptions(target, source);
			target.CSVEncoding = source.Encoding;
			target.CSVSeparator = source.Separator;
		}
	}
}

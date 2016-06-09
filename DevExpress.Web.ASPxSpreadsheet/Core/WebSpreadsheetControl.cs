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
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Spreadsheet;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Forms;
using DevExpress.Office.Internal;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Controls;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public class WebSpreadsheetControl : ISpreadsheetDocumentServer, ISpreadsheetControl, IInnerSpreadsheetControlOwner {
		InnerSpreadsheetControl innerControl;
		bool isDisposing;
		bool isDisposed;
		Rectangle clientWindowRect = new Rectangle(0, 0, 100, 100);
		string lastMessage;
		public WebSpreadsheetControl() {
			IsEnabled = true;
			this.innerControl = CreateInnerControl();
			BeginInitialize();
			innerControl.SetInitialDocumentModelLayoutUnit(DefaultLayoutUnit);
			EndInitialize();
		}
		#region Properties
		public string LastMessage {
			get {
				string result = lastMessage;
				lastMessage = "";
				return result;
			}
			private set { lastMessage = value; }
		}
		public bool IsDisposed { get { return isDisposed; } }
		internal bool InnerIsDisposed { get { return isDisposed; } }
		internal bool InnerIsDisposing { get { return isDisposing; } }
		public void SetViewBounds(Rectangle clientWindowRect) {
			this.clientWindowRect = clientWindowRect;
		}
		Rectangle ISpreadsheetControl.ViewBounds { get { return clientWindowRect; } }
		Rectangle ISpreadsheetControl.LayoutViewBounds { get { return clientWindowRect; } }
		IntPtr System.Windows.Forms.IWin32Window.Handle { get { return IntPtr.Zero; } }
		Cursor ISpreadsheetControl.Cursor { get { return SpreadsheetCursors.Default.Cursor; } set { } }
		protected internal InnerSpreadsheetDocumentServer InnerDocumentServer { get { return InnerControl; } }
		InnerSpreadsheetDocumentServer ISpreadsheetControl.InnerDocumentServer { get { return this.InnerDocumentServer; } }
		protected internal InnerSpreadsheetControl InnerControl { get { return innerControl; } }
		protected internal DocumentModel DocumentModel { get { return InnerControl != null ? InnerControl.DocumentModel : null; } }
		DocumentOptions ISpreadsheetComponent.Options { get { return InnerControl.Options; } }
		public DevExpress.Spreadsheet.IWorkbook Document { get { return InnerControl != null ? InnerControl.Document : null; } }
		public DevExpress.Spreadsheet.Worksheet ActiveWorksheet { get { return InnerControl != null ? InnerControl.ActiveApiWorksheet : null; } }
		public DevExpress.Spreadsheet.Cell ActiveCell { get { return InnerControl != null ? InnerControl.ActiveApiCell : null; } }
		public DevExpress.Spreadsheet.Range SelectedCell {
			get { return InnerControl != null ? InnerControl.SelectedApiCell : null; }
			set {
				if(InnerControl != null)
					InnerControl.SelectedApiCell = value;
			}
		}
		public DevExpress.Spreadsheet.Range Selection {
			get { return InnerControl != null ? InnerControl.SelectedApiRange : null; }
			set {
				if(InnerControl != null)
					InnerControl.SelectedApiRange = value;
			}
		}
		public IList<DevExpress.Spreadsheet.Range> GetSelectedRanges() {
			if (InnerControl != null)
				return InnerControl.GetSelectedApiRanges();
			else
				return new List<DevExpress.Spreadsheet.Range>();
		}
		public bool SetSelectedRanges(IList<DevExpress.Spreadsheet.Range> ranges) {
			return SetSelectedRanges(ranges, true);
		}
		public bool SetSelectedRanges(IList<DevExpress.Spreadsheet.Range> ranges, bool expandToMergedCellsSize) {
			if (InnerControl != null)
				return InnerControl.SetSelectedApiRanges(ranges, expandToMergedCellsSize);
			else
				return false;
		}
		public DevExpress.Spreadsheet.Shape SelectedShape {
			get { return InnerControl != null ? InnerControl.SelectedApiShape : null; }
			set {
				if (InnerControl != null)
					InnerControl.SelectedApiShape = value;
			}
		}
		public DevExpress.Spreadsheet.Picture SelectedPicture {
			get { return InnerControl != null ? InnerControl.SelectedApiPicture : null; }
			set {
				if (InnerControl != null)
					InnerControl.SelectedApiPicture = value;
			}
		}
		public IList<DevExpress.Spreadsheet.Shape> GetSelectedShapes() {
			if (InnerControl != null)
				return InnerControl.GetSelectedApiShapes();
			else
				return new List<DevExpress.Spreadsheet.Shape>();
		}
		public bool SetSelectedShapes(IList<DevExpress.Spreadsheet.Shape> Shapes) {
			if (InnerControl != null)
				return InnerControl.SetSelectedApiShapes(Shapes);
			else
				return false;
		}
		#region LayoutUnit
		public DocumentLayoutUnit LayoutUnit {
			get {
				if(InnerControl != null)
					return InnerControl.LayoutUnit;
				else
					return DefaultLayoutUnit;
			}
			set {
				if(InnerControl != null)
					InnerControl.LayoutUnit = value;
			}
		}
		protected internal virtual bool ShouldSerializeLayoutUnit() {
			return LayoutUnit != DefaultLayoutUnit;
		}
		protected internal virtual void ResetLayoutUnit() {
			LayoutUnit = DefaultLayoutUnit;
		}
		DocumentLayoutUnit DefaultLayoutUnit {
			get {
				return DocumentLayoutUnit.Pixel;
			}
		}
		#endregion
		#region Unit
		[DefaultValue(DocumentUnit.Document)]
		public DocumentUnit Unit {
			get { return InnerControl != null ? InnerControl.Unit : DocumentUnit.Document; }
			set {
				if(InnerControl != null)
					InnerControl.Unit = value;
			}
		}
		#endregion
		#region Modified
		[Browsable(false)]
		public bool Modified {
			get { return InnerControl != null ? InnerControl.Modified : false; }
			set { if (InnerControl != null) InnerControl.Modified = value; }
		}
		#endregion
		public IClipboardManager Clipboard {
			get { return InnerControl != null ? InnerControl.Clipboard : null; }
		}
		#endregion
		protected internal virtual void BeginInitialize() {
			InnerControl.BeginInitialize();
		}
		protected internal virtual void EndInitialize() {
			EndInitializeCommon();
		}
		protected internal virtual void EndInitializeCommon() {
			SubscribeInnerEventsPlatformSpecific();
			SubscribeInnerControlEvents();
			InnerControl.EndInitialize();
			AddServices();
		}
		#region IDisposable implementation
		~WebSpreadsheetControl() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			this.isDisposing = true;
			try {
				if(disposing)
					DisposeCore();
			} finally {
				this.isDisposed = true;
				this.isDisposing = false;
			}
		}
		#endregion
		protected internal virtual void DisposeCommon() {
			if(innerControl != null) {
				UnsubscribeInnerEventsPlatformSpecific();
				UnsubscribeInnerControlEvents();
				innerControl.Dispose();
				innerControl = null;
			}
		}
		protected internal virtual void DisposeCore() {
			lock(this) {
				if(!IsDisposed)
					RaiseBeforeDispose();
				if(GetService<IFormulaBarControl>() != null)
					RemoveService(typeof(IFormulaBarControl));
				DisposeCommon();
			}
		}
		#region ISpreadsheetControl impl
		InnerSpreadsheetControl ISpreadsheetControl.InnerControl { get { return this.InnerControl; } }
		public bool UseGdiPlus { get { return false; } }
		DocumentUnit ISpreadsheetControl.UIUnit { get { return InnerControl != null ? InnerControl.UIUnit : DocumentUnit.Inch; } set { } }
		DialogResult ISpreadsheetControl.ShowWarningMessage(string message) {
			LastMessage = message;
			return DialogResult.Cancel;
		}
		DialogResult ISpreadsheetControl.ShowMessage(string message, string title, MessageBoxIcon icon) {
			return DialogResult.Cancel;
		}
		DialogResult ISpreadsheetControl.ShowDataValidationDialog(string message, string title, DevExpress.XtraSpreadsheet.Model.DataValidationErrorStyle errorStyle) {
			return DialogResult.Cancel;
		}
		DialogResult ISpreadsheetControl.ShowYesNoCancelMessage(string message) {
			return DialogResult.Cancel;
		}
		bool ISpreadsheetControl.ShowOkCancelMessage(string message) {
			return false;
		}
		bool ISpreadsheetControl.ShowYesNoMessage(string message) {
			return false;
		}
		void ISpreadsheetControl.ShowFormatCellsForm(FormatCellsFormProperties properties, FormatCellsFormInitialTabPage initialTabPage, ShowFormatCellsFormCallback callback) {
		}
		void ISpreadsheetControl.ShowHyperlinkForm(IHyperlinkViewInfo hyperlink, ShowHyperlinkFormCallback callback) {
		}
		void ISpreadsheetControl.ShowRenameSheetForm(RenameSheetViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowUnhideSheetForm(UnhideSheetViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowPasteSpecialLocalForm(ModelPasteSpecialOptions properties, ShowPasteSpecialFormLocalCallback callback, object callbackData) {
		}
		void ISpreadsheetControl.ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
		}
		void ISpreadsheetControl.ShowMoveOrCopySheetForm(MoveOrCopySheetViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowChangeChartTypeForm(ChangeChartTypeViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowChangeChartTitleForm(ChangeChartTitleViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowChangeChartHorizontalAxisTitleForm(ChangeChartHorizontalAxisTitleViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowChangeChartVerticalAxisTitleForm(ChangeChartVerticalAxisTitleViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowChartSelectDataForm(ChartSelectDataViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowDataMemberEditorForm(DataMemberEditorViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowSelectDataMemberForm(SelectDataMemberViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowFilterEditorForm(FilterEditorViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowGroupEditorForm(GroupEditorViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowGroupUngroupForm(GroupViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowOutlineSettingsForm(OutlineSettingsViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowOutlineSubtotalForm(OutlineSubtotalViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowGroupRangeEditorForm(GroupRangeEditorViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowMailMergePreviewForm() {
		}
		void ISpreadsheetControl.ShowProtectSheetForm(ProtectSheetViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowProtectWorkbookForm(ProtectWorkbookViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowUnprotectSheetForm(UnprotectSheetViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowUnprotectWorkbookForm(UnprotectWorkbookViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowUnprotectRangeForm(UnprotectRangeViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowInsertFunctionForm(InsertFunctionViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowInsertSymbolForm(InsertSymbolViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowFunctionArgumentsForm(FunctionArgumentsViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowFindReplaceForm(FindReplaceViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowDefineNameForm(DefineNameViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowNameManagerForm(NameManagerViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowCreateDefinedNamesFromSelectionForm(CreateDefinedNamesFromSelectionViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowProtectedRangeForm(ProtectedRangeViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowProtectedRangePermissionsForm(ProtectedRangePermissionsViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowProtectedRangeManagerForm(ProtectedRangeManagerViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowRowHeightForm(RowHeightViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowColumnWidthForm(ColumnWidthViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowDefaultColumnWidthForm(DefaultColumnWidthViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowDocumentPropertiesForm(DocumentPropertiesViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowAutoFilterForm(AutoFilterViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowGenericFilterForm(GenericFilterViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowTop10FilterForm(Top10FilterViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowSimpleFilterForm(SimpleFilterViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowAddDataSourceForm(Action<object> callback) {
		}
		void ISpreadsheetControl.ShowPageSetupForm(PageSetupViewModel viewModel, PageSetupFormInitialTabPage initialTabPage) {
		}
		void ISpreadsheetControl.ShowHeaderFooterForm(HeaderFooterViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowDataValidationForm(DataValidationViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowInsertPivotTableForm(InsertPivotTableViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowOptionsPivotTableForm(OptionsPivotTableViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowMovePivotTableForm(MovePivotTableViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowChangeDataSourcePivotTableForm(ChangeDataSourcePivotTableViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowFieldSettingsPivotTableForm(FieldSettingsPivotTableViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowDataFieldSettingsPivotTableForm(DataFieldSettingsPivotTableViewModel viewModel) {
		}
		IPivotTableFieldsPanel ISpreadsheetControl.CreatePivotTableFieldsPanel() {
			return null;
		}
		void ISpreadsheetControl.ShowPivotTableFieldsFilterItemsForm(PivotTableFieldsFilterItemsViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowPivotTableAutoFilterForm() {
		}
		void ISpreadsheetControl.ShowPivotTableValueFilterForm(PivotTableValueFiltersViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowPivotTableTop10FilterForm(PivotTableTop10FiltersViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowPivotTableDateFilterForm(PivotTableDateFiltersViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowPivotTableLabelFilterForm(PivotTableLabelFiltersViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowPivotTableShowValuesAsForm(PivotTableShowValuesAsViewModel viewModel) {
		}
		bool ISpreadsheetControl.CaptureMouse() {
			return false;
		}
		bool ISpreadsheetControl.ReleaseMouse() {
			return false;
		}
		SpreadsheetMouseHandlerStrategyFactory ISpreadsheetControl.CreateMouseHandlerStrategyFactory() {
			return new WebSpreadsheetMouseHandlerStrategyFactory();
		}
		bool ISpreadsheetControl.IsHyperlinkActive() {
			return false;
		}
		void ISpreadsheetControl.UpdateUIFromBackgroundThread(Action method) {
		}
		IPlatformSpecificScrollBarAdapter ISpreadsheetControl.CreatePlatformSpecificScrollBarAdapter() {
			return new WebFormsScrollBarAdapter();
		}
		SpreadsheetViewVerticalScrollController ISpreadsheetControl.CreateSpreadsheetViewVerticalScrollController(SpreadsheetView view) {
			return new WebFormsSpreadsheetViewVerticalScrollController(view);
		}
		SpreadsheetViewHorizontalScrollController ISpreadsheetControl.CreateSpreadsheetViewHorizontalScrollController(SpreadsheetView view) {
			return new WebFormsSpreadsheetViewHorizontalScrollController(view);
		}
		public void ShowTableInsertForm(InsertTableViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowConditionalFormattingTop10RuleForm(ConditionalFormattingTopBottomRuleViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowConditionalFormattingAverageRuleForm(ConditionalFormattingAverageRuleViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowConditionalFormattingExpressionRuleForm(ConditionalFormattingHighlightCellsRuleViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowConditionalFormattingTextRuleForm(ConditionalFormattingTextRuleViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowConditionalFormattingDuplicateValuesRuleForm(ConditionalFormattingDuplicateValuesRuleViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowConditionalFormattingDateOccurringRuleForm(ConditionalFormattingDateOccurringRuleViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowConditionalFormattingBetweenRuleForm(ConditionalFormattingBetweenRuleViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowSelectDataSourceForm(SelectDataSourceViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowManageQueriesForm() {
		}
		void ISpreadsheetControl.ShowManageRelationsForm() {
		}
		void ISpreadsheetControl.ShowManageDataSourcesForm(ManageDataSourcesViewModel viewModel) {
		}
		#endregion
		#region IInnerSpreadsheetDocumentServerOwner
		DocumentOptions IInnerSpreadsheetDocumentServerOwner.CreateOptions(InnerSpreadsheetDocumentServer documentServer) {
			return new WebSpreadsheetControlOptions(documentServer);
		}
		SpreadsheetMouseHandler IInnerSpreadsheetControlOwner.CreateMouseHandler() {
			return new SpreadsheetMouseHandler(this);
		}
		SpreadsheetViewRepository IInnerSpreadsheetControlOwner.CreateViewRepository() {
			return this.CreateViewRepository();
		}
		protected internal virtual SpreadsheetViewRepository CreateViewRepository() {
			return new WebFormsSpreadsheetViewRepository(this);
		}
		ICellInplaceEditor IInnerSpreadsheetControlOwner.CreateCellInplaceEditor(bool formulaBarShouldFocused) {
			return null;
		}
		IOfficeScrollbar IInnerSpreadsheetControlOwner.CreateVerticalScrollBar() {
			return this.CreateVerticalScrollBar();
		}
		IOfficeScrollbar IInnerSpreadsheetControlOwner.CreateHorizontalScrollBar() {
			return this.CreateHorizontalScrollBar();
		}
		ITabSelector IInnerSpreadsheetControlOwner.CreateTabSelector() {
			return this.CreateTabSelector();
		}
		void IInnerSpreadsheetControlOwner.OnResizeCore() {
		}
		protected internal ITabSelector CreateTabSelector() {
			return null;
		}
		protected internal virtual IOfficeScrollbar CreateVerticalScrollBar() {
			return new WebIOfficeScrollBar();
		}
		protected internal virtual IOfficeScrollbar CreateHorizontalScrollBar() {
			return new WebIOfficeScrollBar();
		}
		void IInnerSpreadsheetControlOwner.ActivateViewPlatformSpecific(SpreadsheetView view) {
		}
		void IInnerSpreadsheetControlOwner.DeactivateViewPlatformSpecific(SpreadsheetView view) {
		}
		void IInnerSpreadsheetControlOwner.Redraw() {
			Redraw();
		}
		void IInnerSpreadsheetControlOwner.Redraw(RefreshAction action) {
			Redraw();
		}
		void Redraw() {
		}
		void IInnerSpreadsheetControlOwner.ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions) {
		}
		void IInnerSpreadsheetControlOwner.OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
		}
		void IInnerSpreadsheetControlOwner.ResizeView() {
		}
		ICommentInplaceEditor IInnerSpreadsheetControlOwner.CreateCommentInplaceEditor() {
			return null;
		}
		IDataValidationInplaceEditor IInnerSpreadsheetControlOwner.CreateDataValidationInplaceEditor() {
			return null;
		}
		public bool IsEnabled { get; set; }
		void IInnerSpreadsheetDocumentServerOwner.RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			this.RaiseDeferredEvents(changeActions);
		}
		protected internal virtual void RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			InnerSpreadsheetControl innerControl = InnerControl;
			if(innerControl != null)
				innerControl.RaiseDeferredEventsCore(changeActions);
		}
		MeasurementAndDrawingStrategy IInnerSpreadsheetDocumentServerOwner.CreateMeasurementAndDrawingStrategy(DocumentModel documentModel) {
			return this.CreateMeasurementAndDrawingStrategy(documentModel);
		}
		protected internal virtual MeasurementAndDrawingStrategy CreateMeasurementAndDrawingStrategy(DocumentModel documentModel) {
			if(PrecalculatedMetricsFontCacheManager.ShouldUse()) 
				return new ServerPrecalculatedMetricsMeasurementAndDrawingStrategy(documentModel);
			else {
				if(UseGdiPlus)
					return new GdiPlusMeasurementAndDrawingStrategy(documentModel);
				else
					return new GdiMeasurementAndDrawingStrategy(documentModel);
			}
		}
		public bool ReadOnly {
			get { return innerControl.IsReadOnly; }
			set { innerControl.IsReadOnly = value; }
		}
		#endregion
		#region ISpreadsheetControl publics
		public void CreateNewDocument() {
			if(InnerControl != null)
				InnerControl.CreateNewDocument();
		}
		public bool LoadDocument(string fileName) {
			if(InnerControl != null)
				return InnerControl.LoadDocument(fileName);
			return false;
		}
		public bool LoadDocument(string fileName, DevExpress.Spreadsheet.DocumentFormat format) {
			if(InnerControl != null)
				return InnerControl.LoadDocument(fileName, format);
			return false;
		}
		public bool LoadDocument(Stream stream, DevExpress.Spreadsheet.DocumentFormat format) {
			if(InnerControl != null)
				return InnerControl.LoadDocument(stream, format);
			return false;
		}
		public bool LoadDocument(byte[] buffer, DevExpress.Spreadsheet.DocumentFormat format) {
			if(InnerControl != null)
				return InnerControl.LoadDocument(buffer, format);
			return false;
		}
		public void SaveDocument(string fileName) {
			if(InnerControl != null)
				InnerControl.SaveDocument(fileName);
		}
		public void SaveDocument(string fileName, DevExpress.Spreadsheet.DocumentFormat format) {
			if(InnerControl != null)
				InnerControl.SaveDocument(fileName, format);
		}
		public void SaveDocument(Stream stream, DevExpress.Spreadsheet.DocumentFormat format) {
			if(InnerControl != null)
				InnerControl.SaveDocument(stream, format);
		}
		public byte[] SaveDocument(DevExpress.Spreadsheet.DocumentFormat format) {
			if(InnerControl != null)
				return InnerControl.SaveDocument(format);
			else
				return null;
		}
		public bool IsPrintingAvailable { get { return false; } }
		bool ISpreadsheetControl.IsPrintPreviewAvailable { get { return IsPrintingAvailable; } }
		public void Print() {
		}
		public void ShowPrintDialog() {
		}
		public void ShowPrintPreview() {
		}
		public void ShowRibbonPrintPreview() {
		}
		#endregion
		#region ISpreadsheetControl events
		#region DocumentClosing
		public event CancelEventHandler DocumentClosing {
			add { if(InnerControl != null) InnerControl.DocumentClosing += value; }
			remove { if(InnerControl != null) InnerControl.DocumentClosing -= value; }
		}
		#endregion
		#region ContentChanged
		EventHandler onContentChanged;
		public event EventHandler ContentChanged { add { onContentChanged += value; } remove { onContentChanged -= value; } }
		protected internal virtual void RaiseContentChanged() {
			if(onContentChanged != null)
				onContentChanged(this, EventArgs.Empty);
		}
		#endregion
		#region SelectionChanged
		public event EventHandler SelectionChanged {
			add { if(InnerControl != null) InnerControl.SelectionChanged += value; }
			remove { if(InnerControl != null) InnerControl.SelectionChanged -= value; }
		}
		#endregion
		#region ActiveSheetChanging
		public event ActiveSheetChangingEventHandler ActiveSheetChanging {
			add { if(InnerControl != null) InnerControl.ActiveSheetChanging += value; }
			remove { if(InnerControl != null) InnerControl.ActiveSheetChanging -= value; }
		}
		#endregion
		#region ActiveSheetChanged
		public event ActiveSheetChangedEventHandler ActiveSheetChanged {
			add { if(InnerControl != null) InnerControl.ActiveSheetChanged += value; }
			remove { if(InnerControl != null) InnerControl.ActiveSheetChanged -= value; }
		}
		#endregion
		#region SheetRenaming
		public event SheetRenamingEventHandler SheetRenaming {
			add { if(InnerControl != null) InnerControl.SheetRenaming += value; }
			remove { if(InnerControl != null) InnerControl.SheetRenaming -= value; }
		}
		#endregion
		#region SheetRenamed
		public event SheetRenamedEventHandler SheetRenamed {
			add { if(InnerControl != null) InnerControl.SheetRenamed += value; }
			remove { if(InnerControl != null) InnerControl.SheetRenamed -= value; }
		}
		#endregion
		#region SheetRemoved
		public event SheetRemovedEventHandler SheetRemoved {
			add { if(InnerControl != null) InnerControl.SheetRemoved += value; }
			remove { if(InnerControl != null) InnerControl.SheetRemoved -= value; }
		}
		#endregion
		#region SheetInserted
		public event SheetInsertedEventHandler SheetInserted {
			add { if(InnerControl != null) InnerControl.SheetInserted += value; }
			remove { if(InnerControl != null) InnerControl.SheetInserted -= value; }
		}
		#endregion
		#region RowsRemoved
		public event RowsRemovedEventHandler RowsRemoved {
			add { if(InnerControl != null) InnerControl.RowsRemoved += value; }
			remove { if(InnerControl != null) InnerControl.RowsRemoved -= value; }
		}
		#endregion
		#region RowsInserted
		public event RowsInsertedEventHandler RowsInserted {
			add { if(InnerControl != null) InnerControl.RowsInserted += value; }
			remove { if(InnerControl != null) InnerControl.RowsInserted -= value; }
		}
		#endregion
		#region ColumnsRemoved
		public event ColumnsRemovedEventHandler ColumnsRemoved {
			add { if(InnerControl != null) InnerControl.ColumnsRemoved += value; }
			remove { if(InnerControl != null) InnerControl.ColumnsRemoved -= value; }
		}
		#endregion
		#region ColumnsInserted
		public event ColumnsInsertedEventHandler ColumnsInserted {
			add { if(InnerControl != null) InnerControl.ColumnsInserted += value; }
			remove { if(InnerControl != null) InnerControl.ColumnsInserted -= value; }
		}
		#endregion
		#region UnitChanging
		public event EventHandler UnitChanging {
			add {
				if(InnerControl != null)
					InnerControl.UnitChanging += value;
			}
			remove {
				if(InnerControl != null)
					InnerControl.UnitChanging -= value;
			}
		}
		#endregion
		#region UnitChanged
		public event EventHandler UnitChanged {
			add {
				if(InnerControl != null)
					InnerControl.UnitChanged += value;
			}
			remove {
				if(InnerControl != null)
					InnerControl.UnitChanged -= value;
			}
		}
		#endregion
		#region ModifiedChanged
		EventHandler onModifiedChanged;
		public event EventHandler ModifiedChanged { add { onModifiedChanged += value; } remove { onModifiedChanged -= value; } }
		protected internal virtual void RaiseModifiedChanged() {
			if(onModifiedChanged != null)
				onModifiedChanged(this, EventArgs.Empty);
		}
		#endregion
		#region DocumentLoaded
		EventHandler onDocumentLoaded;
		public event EventHandler DocumentLoaded { add { onDocumentLoaded += value; } remove { onDocumentLoaded -= value; } }
		protected internal virtual void RaiseDocumentLoaded() {
			if(onDocumentLoaded != null)
				onDocumentLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region EmptyDocumentCreated
		EventHandler onEmptyDocumentCreated;
		public event EventHandler EmptyDocumentCreated { add { onEmptyDocumentCreated += value; } remove { onEmptyDocumentCreated -= value; } }
		protected internal virtual void RaiseEmptyDocumentCreated() {
			if(onEmptyDocumentCreated != null)
				onEmptyDocumentCreated(this, EventArgs.Empty);
		}
		#endregion
		#region BeforeImport
		public event BeforeImportEventHandler BeforeImport {
			add { if(InnerControl != null) InnerControl.BeforeImport += value; }
			remove { if(InnerControl != null) InnerControl.BeforeImport -= value; }
		}
		#endregion
		#region BeforeExport
		public event BeforeExportEventHandler BeforeExport {
			add { if(InnerControl != null) InnerControl.BeforeExport += value; }
			remove { if(InnerControl != null) InnerControl.BeforeExport -= value; }
		}
		#endregion
		#region InvalidFormatException
		public event InvalidFormatExceptionEventHandler InvalidFormatException {
			add { if(InnerControl != null) InnerControl.InvalidFormatException += value; }
			remove { if(InnerControl != null) InnerControl.InvalidFormatException -= value; }
		}
		#endregion
		#region InitializeDocument
		public event EventHandler InitializeDocument {
			add { if(InnerControl != null) InnerControl.InitializeDocument += value; }
			remove { if(InnerControl != null) InnerControl.InitializeDocument -= value; }
		}
		#endregion
		#region UpdateUI
		public event EventHandler UpdateUI {
			add { if(InnerControl != null) InnerControl.UpdateUI += value; }
			remove { if(InnerControl != null) InnerControl.UpdateUI -= value; }
		}
		#endregion
		#region PanesFrozen
		public event PanesFrozenEventHandler PanesFrozen {
			add { if(InnerControl != null) InnerControl.PanesFrozen += value; }
			remove { if(InnerControl != null) InnerControl.PanesFrozen -= value; }
		}
		#endregion
		#region PanesUnfrozen
		public event PanesUnfrozenEventHandler PanesUnfrozen {
			add { if(InnerControl != null) InnerControl.PanesUnfrozen += value; }
			remove { if(InnerControl != null) InnerControl.PanesUnfrozen -= value; }
		}
		#endregion
		#region BeforePrintSheet
		public event BeforePrintSheetEventHandler BeforePrintSheet {
			add { if(InnerControl != null) InnerControl.BeforePrintSheet += value; }
			remove { if(InnerControl != null) InnerControl.BeforePrintSheet -= value; }
		}
		#endregion
		#region ScrollPositionChanged
		public event ScrollPositionChangedEventHandler ScrollPositionChanged {
			add { if (InnerControl != null) InnerControl.ScrollPositionChanged += value; }
			remove { if (InnerControl != null) InnerControl.ScrollPositionChanged -= value; }
		}
		#endregion
		#region RangeCopying
		public event RangeCopyingEventHandler RangeCopying {
			add { if (InnerControl != null) InnerControl.RangeCopying += value; }
			remove { if (InnerControl != null) InnerControl.RangeCopying -= value; }
		}
		#endregion
		#region ShapesCopying
		public event ShapesCopyingEventHandler ShapesCopying {
			add { if (InnerControl != null) InnerControl.ShapesCopying += value; }
			remove { if (InnerControl != null) InnerControl.ShapesCopying -= value; }
		}
		#endregion
		#region RangeCopied
		public event RangeCopiedEventHandler RangeCopied {
			add { if (InnerControl != null) InnerControl.RangeCopied += value; }
			remove { if (InnerControl != null) InnerControl.RangeCopied -= value; }
		}
		#endregion
		#region CopiedRangePasting
		public event CopiedRangePastingEventHandler CopiedRangePasting {
			add { if (InnerControl != null) InnerControl.CopiedRangePasting += value; }
			remove { if (InnerControl != null) InnerControl.CopiedRangePasting -= value; }
		}
		#endregion
		#region CopiedRangePasted
		public event CopiedRangePastedEventHandler CopiedRangePasted {
			add { if (InnerControl != null) InnerControl.CopiedRangePasted += value; }
			remove { if (InnerControl != null) InnerControl.CopiedRangePasted -= value; }
		}
		#endregion
		#region ClipboardDataPasting
		public event ClipboardDataPastingEventHandler ClipboardDataPasting {
			add { if (InnerControl != null) InnerControl.ClipboardDataPasting += value; }
			remove { if (InnerControl != null) InnerControl.ClipboardDataPasting -= value; }
		}
		#endregion
		#region ClipboardDataObtained
		public event ClipboardDataObtainedEventHandler ClipboardDataObtained {
			add { if (InnerControl != null) InnerControl.ClipboardDataObtained += value; }
			remove { if (InnerControl != null) InnerControl.ClipboardDataObtained -= value; }
		}
		#endregion
		#region ClipboardDataPasted
		public event EventHandler ClipboardDataPasted {
			add { if (InnerControl != null) InnerControl.ClipboardDataPasted += value; }
			remove { if (InnerControl != null) InnerControl.ClipboardDataPasted -= value; }
		}
		#endregion
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			if(InnerControl != null)
				InnerControl.BeginUpdate();
		}
		public void EndUpdate() {
			if(InnerControl != null)
				InnerControl.EndUpdate();
		}
		public void CancelUpdate() {
			if(InnerControl != null)
				InnerControl.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper {
			get {
				IBatchUpdateable updateable = InnerControl;
				if(updateable != null)
					return updateable.BatchUpdateHelper;
				else
					return null;
			}
		}
		public bool IsUpdateLocked { get { return InnerControl != null ? InnerControl.IsUpdateLocked : false; } }
		#endregion
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if(InnerControl != null)
				InnerControl.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if(InnerControl != null)
				InnerControl.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if(InnerControl != null)
				InnerControl.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if(InnerControl != null)
				InnerControl.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			if(InnerControl != null)
				InnerControl.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			if(InnerControl != null)
				InnerControl.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public virtual object GetService(Type serviceType) {
			if(InnerControl != null)
				return InnerControl.GetService(serviceType);
			else
				return null;
		}
		#endregion
		public T GetService<T>() where T : class {
			if(InnerControl != null)
				return InnerControl.GetService<T>();
			else
				return default(T);
		}
		public T ReplaceService<T>(T newService) where T : class {
			if(InnerControl != null)
				return InnerControl.ReplaceService<T>(newService);
			else
				return default(T);
		}
		#region ICommandAwareControl<SpreadsheetCommandId>
		CommandBasedKeyboardHandler<SpreadsheetCommandId> ICommandAwareControl<SpreadsheetCommandId>.KeyboardHandler { get { return InnerControl != null ? InnerControl.KeyboardHandler as CommandBasedKeyboardHandler<SpreadsheetCommandId> : null; } }
		Command ICommandAwareControl<SpreadsheetCommandId>.CreateCommand(SpreadsheetCommandId commandId) {
			return this.CreateCommand(commandId);
		}
		public virtual SpreadsheetCommand CreateCommand(SpreadsheetCommandId commandId) {
			if(InnerControl != null)
				return InnerControl.CreateCommand(commandId);
			else
				return null;
		}
		bool ICommandAwareControl<SpreadsheetCommandId>.HandleException(Exception e) {
			return this.HandleException(e);
		}
		protected internal virtual bool HandleException(Exception e) {
			if(InnerControl != null)
				return InnerControl.RaiseUnhandledException(e);
			else
				return false;
		}
		void ICommandAwareControl<SpreadsheetCommandId>.Focus() {
		}
		void ICommandAwareControl<SpreadsheetCommandId>.CommitImeContent() {
		}
		#endregion
		#region Events
		#region BeforeDispose
#if !SL && !WPF
		EventHandler onBeforeDispose;
		public event EventHandler BeforeDispose { add { onBeforeDispose += value; } remove { onBeforeDispose -= value; } }
		protected internal virtual void RaiseBeforeDispose() {
			if(onBeforeDispose != null)
				onBeforeDispose(this, EventArgs.Empty);
		}
#else
		public event EventHandler BeforeDispose { add { } remove { }
		}
#endif
		#endregion
		#endregion
		protected internal virtual InnerSpreadsheetControl CreateInnerControl() {
			return new WebInnerSpreadsheetControl(this);
		}
		protected internal virtual void SubscribeInnerControlEvents() {
		}
		protected internal virtual void UnsubscribeInnerControlEvents() {
		}
		protected internal virtual void AddServices() {
			AddServicesPlatformSpecific();
		}
		protected internal virtual void AddServicesPlatformSpecific() {
			AddService(typeof(ISpreadsheetCommandFactoryService), CreateWebSpreadsheetCommandFactoryService());
			AddService(typeof(IChartControllerFactoryService), new ChartControllerFactoryService());
			AddService(typeof(IChartImageService), new ChartImageService());
		}
		protected internal void SubscribeInnerEventsPlatformSpecific() {
		}
		protected internal void UnsubscribeInnerEventsPlatformSpecific() {
		}
		protected virtual ISpreadsheetCommandFactoryService CreateWebSpreadsheetCommandFactoryService() {
			return new WebFormsSpreadsheetCommandFactoryService(this);
		}
	}
	public class WebInnerSpreadsheetControl : InnerSpreadsheetControl {
		public WebInnerSpreadsheetControl(IInnerSpreadsheetControlOwner owner)
			: base(owner) {
		}
		public override bool AllowShowingForms { get { return false; } }
		public override XtraSpreadsheet.Layout.DocumentLayout DesignDocumentLayout {
			get {
				return base.DesignDocumentLayout;
			}
		}
		public override void CalculateDocumentLayout(XtraSpreadsheet.Layout.Engine.DocumentLayoutAnchor anchor) {
			base.CalculateDocumentLayout(anchor);
		}
		protected internal override void AddServices() {
		}
	}
	public class WebFormsScrollBarAdapter : IPlatformSpecificScrollBarAdapter {
		public virtual void OnScroll(ScrollBarAdapter adapter, object sender, ScrollEventArgs e) {
			int delta = ((int)e.NewValue) - adapter.GetRawScrollBarValue();
			if(adapter.EnsureSynchronizedCore()) {
				ScrollEventArgs args = new ScrollEventArgs(e.Type, adapter.GetRawScrollBarValue(), adapter.GetRawScrollBarValue() + delta, e.ScrollOrientation);
				adapter.RaiseScroll(args);
				e.NewValue = args.NewValue;
			} else
				adapter.RaiseScroll(e);
		}
		public virtual void ApplyValuesToScrollBarCore(ScrollBarAdapter adapter) {
			if(adapter.Maximum > (long)int.MaxValue)
				adapter.Factor = 1.0 / (1 + (adapter.Maximum / (long)int.MaxValue));
			else
				adapter.Factor = 1.0;
			adapter.ScrollBar.BeginUpdate();
			try {
				adapter.ScrollBar.Minimum = (int)Math.Round(adapter.Factor * adapter.Minimum);
				adapter.ScrollBar.Maximum = (int)Math.Round(adapter.Factor * adapter.Maximum);
				adapter.ScrollBar.LargeChange = (int)Math.Round(adapter.Factor * adapter.LargeChange);
				adapter.ScrollBar.Value = (int)Math.Round(adapter.Factor * adapter.Value);
				adapter.ScrollBar.Enabled = adapter.Enabled;
			} finally {
				adapter.ScrollBar.EndUpdate();
			}
		}
		public int GetRawScrollBarValue(ScrollBarAdapter adapter) {
			return adapter.ScrollBar.Value;
		}
		public bool SetRawScrollBarValue(ScrollBarAdapter adapter, int value) {
			if(adapter.ScrollBar.Value != value) {
				adapter.ScrollBar.Value = value;
				adapter.Value = (long)Math.Round(value / adapter.Factor);
				return true;
			} else
				return false;
		}
		public virtual int GetPageUpRawScrollBarValue(ScrollBarAdapter adapter) {
			return Math.Max(adapter.ScrollBar.Minimum, adapter.ScrollBar.Value - adapter.ScrollBar.LargeChange);
		}
		public virtual int GetPageDownRawScrollBarValue(ScrollBarAdapter adapter) {
			return Math.Min(adapter.ScrollBar.Maximum - adapter.ScrollBar.LargeChange + 1, adapter.ScrollBar.Value + adapter.ScrollBar.LargeChange);
		}
		public virtual ScrollEventArgs CreateLastScrollEventArgs(ScrollBarAdapter adapter) {
			return new ScrollEventArgs(ScrollEventType.Last, adapter.ScrollBar.Maximum - adapter.ScrollBar.LargeChange + 1);
		}
	}
	public class WebFormsSpreadsheetViewVerticalScrollController : SpreadsheetViewVerticalScrollController {
		public WebFormsSpreadsheetViewVerticalScrollController(SpreadsheetView view)
			: base(view) {
		}
		protected internal override bool IsScrollTypeValid(ScrollEventArgs e) {
			return e.Type != ScrollEventType.EndScroll;
		}
		protected internal override int CalculateScrollDelta(ScrollEventArgs e) {
			return (int)(e.NewValue - ScrollBarAdapter.GetRawScrollBarValue());
		}
		protected internal override void ApplyNewScrollValue(int value) {
			ScrollBarAdapter.SetRawScrollBarValue(value);
		}
		protected internal override void ApplyNewScrollValueToScrollEventArgs(ScrollEventArgs e, int value) {
			e.NewValue = value;
		}
	}
	public class WebFormsSpreadsheetViewHorizontalScrollController : SpreadsheetViewHorizontalScrollController {
		public WebFormsSpreadsheetViewHorizontalScrollController(SpreadsheetView view)
			: base(view) {
		}
		protected internal override bool IsScrollTypeValid(ScrollEventArgs e) {
			return e.Type != ScrollEventType.EndScroll;
		}
		protected internal override int CalculateScrollDelta(ScrollEventArgs e) {
			return (int)(e.NewValue - ScrollBarAdapter.GetRawScrollBarValue());
		}
		protected internal override void ApplyNewScrollValue(int value) {
			ScrollBarAdapter.SetRawScrollBarValue(value);
		}
		protected internal override void ApplyNewScrollValueToScrollEventArgs(ScrollEventArgs e, int value) {
			e.NewValue = value;
		}
	}
	public class WebFormsSpreadsheetViewRepository : SpreadsheetViewRepository {
		public WebFormsSpreadsheetViewRepository(WebSpreadsheetControl control)
			: base(control) {
		}
		protected internal override NormalView CreateNormalView() {
			return new NormalView(Control);
		}
	}
	#region GdiPlusMeasurementAndDrawingStrategy
	public class GdiPlusMeasurementAndDrawingStrategy : GdiPlusMeasurementAndDrawingStrategyBase {
		public GdiPlusMeasurementAndDrawingStrategy(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override Painter CreateDocumentPainter(IDrawingSurface surface) {
			return new EmptyPainter();
		}
	}
	#endregion
	#region GdiMeasurementAndDrawingStrategy
	public class GdiMeasurementAndDrawingStrategy : GdiMeasurementAndDrawingStrategyBase {
		public GdiMeasurementAndDrawingStrategy(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override Painter CreateDocumentPainter(IDrawingSurface surface) {
			return new EmptyPainter();
		}
	}
	#endregion
	#region PrecalculatedMetricsMeasurementAndDrawingStrategyBase (abstract class)
	public abstract class PrecalculatedMetricsMeasurementAndDrawingStrategyBase : MeasurementAndDrawingStrategy {
		protected PrecalculatedMetricsMeasurementAndDrawingStrategyBase(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override FontCacheManager CreateFontCacheManager() {
			return new PrecalculatedMetricsFontCacheManager(DocumentModel.LayoutUnitConverter);
		}
	}
	#endregion
	#region ServerPrecalculatedMetricsMeasurementAndDrawingStrategy
	public class ServerPrecalculatedMetricsMeasurementAndDrawingStrategy : PrecalculatedMetricsMeasurementAndDrawingStrategyBase {
		public ServerPrecalculatedMetricsMeasurementAndDrawingStrategy(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override Painter CreateDocumentPainter(IDrawingSurface surface) {
			return new EmptyPainter();
		}
	}
	#endregion
	#region WebSpreadsheetControlOptions
	public class WebSpreadsheetControlOptions : DocumentOptions {
		public WebSpreadsheetControlOptions(InnerSpreadsheetDocumentServer documentServer)
			: base(documentServer) {
		}
	}
	#endregion
	public class WebSpreadsheetMouseHandlerStrategyFactory : SpreadsheetMouseHandlerStrategyFactory {
		public override DragFloatingObjectManuallyMouseHandlerStateStrategy CreateDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state) {
			return null;
		}
		public override DragRangeManuallyMouseHandlerStateStrategy CreateDragRangeManuallyMouseHandlerStateStrategy(DragRangeManuallyMouseHandlerStateBase state) {
			return null;
		}
		public override SpreadsheetMouseHandlerStrategy CreateMouseHandlerStrategy(SpreadsheetMouseHandler mouseHandler) {
			return null;
		}
		public override ResizeColumnMouseHandlerStateStrategy CreateResizeColumnMouseHandlerStateStrategy(ContinueResizeColumnsMouseHandlerState state) {
			return null;
		}
		public override ResizeRowMouseHandlerStateStrategy CreateResizeRowMouseHandlerStateStrategy(ContinueResizeRowsMouseHandlerState state) {
			return null;
		}
		public override SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy CreateSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy(SpreadsheetRectangularObjectResizeMouseHandlerState state) {
			return null;
		}
		public override CommentMouseHandlerStateStrategy CreateCommentMouseHandlerStateStrategy(CommentMouseHandlerStateBase state) {
			return null;
		}
	}
	public class WebIOfficeScrollBar : IOfficeScrollbar {
		int val;
		int min;
		int max;
		int smallChange;
		int largeChange;
		bool enabled;
		#region IOfficeScrollbar Members
		public int Value { get { return val; } set { val = value; } }
		public int Minimum { get { return min; } set { min = value; } }
		public int Maximum { get { return max; } set { max = value; } }
		public int LargeChange { get { return largeChange; } set { largeChange = value; } }
		public int SmallChange { get { return smallChange; } set { smallChange = value; } }
		public bool Enabled { get { return enabled; } set { enabled = value; } }
		public event ScrollEventHandler Scroll { add { } remove { } }
		public void BeginUpdate() {
		}
		public void EndUpdate() {
		}
		#endregion
	}
	public class WebFormsSpreadsheetCommandFactoryService : SpreadsheetCommandFactoryService {
		static readonly Type[] constructorParametersInterface = new Type[] { typeof(WebSpreadsheetControl) };
		public WebFormsSpreadsheetCommandFactoryService(WebSpreadsheetControl control)
			: base(control) {
		}
		protected internal override void PopulateConstructorTable(SpreadsheetCommandConstructorTable table) {
			base.PopulateConstructorTable(table);
		}
	}
}

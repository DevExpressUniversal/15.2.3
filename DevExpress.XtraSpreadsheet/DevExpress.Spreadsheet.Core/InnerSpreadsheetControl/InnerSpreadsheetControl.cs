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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Services;
using DevExpress.Office;
using DevExpress.Office.Internal;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Keyboard;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel.Design;
namespace DevExpress.XtraSpreadsheet {
	public interface ISpreadsheetDocumentServer : IBatchUpdateable, IServiceContainer, IDisposable {
	}
}
namespace DevExpress.XtraSpreadsheet.Internal {
	public partial class InnerSpreadsheetControl : InnerSpreadsheetDocumentServer {
		#region Fields
		BackgroundThreadUIUpdater backgroundThreadUIUpdater;
		SpreadsheetView activeView;
		SpreadsheetViewRepository views;
		SpreadsheetViewType? activeViewTypeBeforeActiveViewCreation;
		bool isReadOnly;
		bool updateUIOnIdle;
		bool forceUpdateUIOnIdle;
		bool endInitializeIsCalled;
		bool isDocumentServerInitialized;
		IOfficeScrollbar verticalScrollbar;
		IOfficeScrollbar horizontalScrollbar;
		ITabSelector tabSelector;
		SpreadsheetControlDeferredChanges deferredChanges;
		DocumentLayout documentLayout;
		UIErrorHandler errorHandler;
		#endregion
		public InnerSpreadsheetControl(IInnerSpreadsheetControlOwner owner)
			: base(owner) {
		}
		#region Properties
		public virtual bool AllowShowingForms { get { return true; } }
		public new IInnerSpreadsheetControlOwner Owner { get { return (IInnerSpreadsheetControlOwner)base.Owner; } }
		public BackgroundThreadUIUpdater BackgroundThreadUIUpdater {
			get { return backgroundThreadUIUpdater; }
			set {
				Guard.ArgumentNotNull(value, "backgroundThreadUIUpdater");
				backgroundThreadUIUpdater = value;
			}
		}
		public Rectangle GetCellBounds(int column, int row) {
			CellPosition position = new CellPosition(column, row);
			CellRange mergedCells = DocumentModel.ActiveSheet.Selection.GetActualCellRange(position);
			Rectangle result = GetCellBoundsCore(DesignDocumentLayout.Pages, mergedCells);
			return ApplyZoomToBounds(result);
		}
		internal Rectangle GetCellBoundsCore(IList<Page> pages, CellRange range) {
			if (pages.Count == 1)
				return pages[0].CalculateRangeBounds(range);
			List<Rectangle> bounds = new List<Rectangle>();
			foreach (Page page in pages) {
				Rectangle rect = page.CalculateRangeBounds(range);
				if (rect != Rectangle.Empty)
					bounds.Add(rect);
			}
			if (bounds.Count == 0)
				return Rectangle.Empty;
			Rectangle firstRect = bounds[0];
			Rectangle lastRect = bounds[bounds.Count - 1];
			return Rectangle.FromLTRB(firstRect.Left, firstRect.Top, lastRect.Right, lastRect.Bottom);
		}
		internal Rectangle ApplyZoomToBounds(Rectangle bounds) {
			float zoomFactor = ActiveView.ZoomFactor;
			if (zoomFactor == 1.0f)
				return bounds;
			int left = (int)(Math.Round(bounds.Left * zoomFactor));
			int top = (int)(Math.Round(bounds.Top * zoomFactor));
			int right = (int)(Math.Round(bounds.Right * zoomFactor));
			int bottom = (int)(Math.Round(bounds.Bottom * zoomFactor));
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		public DevExpress.Spreadsheet.Worksheet ActiveApiWorksheet {
			get {
				IWorkbook document = this.Document;
				if (document == null)
					return null;
				return document.Worksheets[this.DocumentModel.ActiveSheetIndex];
			}
		}
		public DevExpress.Spreadsheet.Cell ActiveApiCell {
			get {
				DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
				if (sheet == null)
					return null;
				Range selectedCell = sheet.SelectedCell;
				return sheet[selectedCell.TopRowIndex, selectedCell.LeftColumnIndex];
			}
		}
		public DevExpress.Spreadsheet.Range SelectedApiCell {
			get {
				DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
				if (sheet == null)
					return null;
				return sheet.SelectedCell;
			}
			set {
				DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
				if (sheet != null)
					sheet.SelectedCell = value;
			}
		}
		public DevExpress.Spreadsheet.Range SelectedApiRange {
			get {
				DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
				if (sheet == null)
					return null;
				return sheet.Selection;
			}
			set {
				DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
				if (sheet != null)
					sheet.Selection = value;
			}
		}
		public IList<DevExpress.Spreadsheet.Range> GetSelectedApiRanges() {
			DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
			if (sheet == null)
				return new List<DevExpress.Spreadsheet.Range>();
			else
				return sheet.GetSelectedRanges();
		}
		public bool SetSelectedApiRanges(IList<DevExpress.Spreadsheet.Range> ranges, bool expandToMergedCellsSize) {
			DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
			if (sheet != null)
				return sheet.SetSelectedRanges(ranges, expandToMergedCellsSize);
			else
				return false;
		}
		public Shape SelectedApiShape {
			get {
				DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
				if (sheet != null)
					return sheet.SelectedShape;
				else
					return null;
			}
			set {
				DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
				if (sheet != null)
					sheet.SelectedShape = value;
			}
		}
		public DevExpress.Spreadsheet.Picture SelectedApiPicture {
			get {
				DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
				if (sheet != null)
					return sheet.SelectedPicture;
				else
					return null;
			}
			set {
				DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
				if (sheet != null)
					sheet.SelectedPicture = value;
			}
		}
		public IList<DevExpress.Spreadsheet.Shape> GetSelectedApiShapes() {
			DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
			if (sheet == null)
				return new List<DevExpress.Spreadsheet.Shape>();
			else
				return sheet.GetSelectedShapes();
		}
		public bool SetSelectedApiShapes(IList<DevExpress.Spreadsheet.Shape> Shapes) {
			DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
			if (sheet != null)
				return sheet.SetSelectedShapes(Shapes);
			else
				return false;
		}
		public DevExpress.Spreadsheet.Range VisibleApiRange {
			get {
				CellPosition topLeft = DocumentModel.ActiveSheet.ActiveView.TopLeftCell;
				return GetVisibleApiRangeCore(topLeft);
			}
		}
		public DevExpress.Spreadsheet.Range VisibleUnfrozenApiRange {
			get {
				CellPosition topLeft = DocumentModel.ActiveSheet.ActiveView.ScrolledTopLeftCell;
				return GetVisibleApiRangeCore(topLeft);
			}
		}
		DevExpress.Spreadsheet.Range GetVisibleApiRangeCore(CellPosition topLeft) {
			DevExpress.Spreadsheet.Worksheet sheet = ActiveApiWorksheet;
			if (sheet == null)
				return null;
			if (InnerDocumentLayout == null)
				return null;
			CellPosition bottomRight = InnerDocumentLayout.VisibleRange.BottomRight;
			return sheet.Range.FromLTRB(topLeft.Column, topLeft.Row, bottomRight.Column, bottomRight.Row);
		}
		public SpreadsheetView ActiveView { get { return activeView; } }
		public SpreadsheetViewType ActiveViewType {
			get {
				if (ActiveView != null)
					return ActiveView.Type;
				else
					return (activeViewTypeBeforeActiveViewCreation != null) ? (SpreadsheetViewType)activeViewTypeBeforeActiveViewCreation : DefaultViewType;
			}
			set {
				if (ActiveView == null) {
					activeViewTypeBeforeActiveViewCreation = value;
					return;
				}
				if (ActiveViewType == value)
					return;
				SetActiveView(views.GetViewByType(value));
				OnUpdateUI();
			}
		}
		public virtual SpreadsheetViewType DefaultViewType { get { return SpreadsheetViewType.Normal; } }
		public SpreadsheetViewRepository Views { get { return views; } }
		public override bool IsEnabled { get { return Owner.IsEnabled; } }
		#region IsReadOnly
		public override bool IsReadOnly {
			get { return isReadOnly; }
			set {
				if (IsReadOnly == value)
					return;
				isReadOnly = value;
				OnIsReadOnlyChanged();
			}
		}
		protected internal override bool ActualReadOnly { get { return IsReadOnly; } }
		#endregion
		public bool ForceUpdateUIOnIdle { get { return forceUpdateUIOnIdle; } set { forceUpdateUIOnIdle = value; } }
		public bool UpdateUIOnIdle { get { return updateUIOnIdle; } set { updateUIOnIdle = value; } }
		protected internal IOfficeScrollbar VerticalScrollBar { get { return verticalScrollbar; } }
		protected internal IOfficeScrollbar HorizontalScrollBar { get { return horizontalScrollbar; } }
		public ITabSelector TabSelector { get { return this.tabSelector; } }
		public SpreadsheetControlDeferredChanges ControlDeferredChanges { get { return deferredChanges; } }
		public Size HeadersSize { get; set; }
		public virtual DocumentLayout DesignDocumentLayout {
			get {
				if (documentLayout != null)
					return documentLayout;
				DocumentLayoutAnchor anchor = new DocumentLayoutAnchor();
				CellPosition activePaneTopLeft = this.DocumentModel.ActiveSheet.ActiveView.GetSplitPosition();
				CellPosition topLeft = this.DocumentModel.ActiveSheet.ActiveView.TopLeftCell;
				activePaneTopLeft = new CellPosition(topLeft.Column + activePaneTopLeft.Column, topLeft.Row + activePaneTopLeft.Row);
				CellPosition scrolledActivePaneTopLeft = this.DocumentModel.ActiveSheet.ActiveView.ScrolledTopLeftCell;
				anchor.CellPosition = new CellPosition(Math.Max(scrolledActivePaneTopLeft.Column, activePaneTopLeft.Column), Math.Max(scrolledActivePaneTopLeft.Row, activePaneTopLeft.Row));
				anchor.HorizontalFarAlign = false;
				anchor.VerticalFarAlign = false;
				CalculateDocumentLayout(anchor);
				return documentLayout;
			}
		}
		internal DocumentLayout InnerDocumentLayout { get { return documentLayout; } }
		public UIErrorHandler ErrorHandler { get { return errorHandler; } }
		#endregion
		protected void SetDocumentLayout(DocumentLayout layout) {
			this.documentLayout = layout;
		}
		public virtual void CalculateDocumentLayout(DocumentLayoutAnchor anchor) {
			SetDocumentLayout(new DocumentLayout(DocumentModel));
			DocumentLayoutCalculator calculator = new DocumentLayoutCalculator(this.documentLayout, this.DocumentModel.ActiveSheet, this.Owner.LayoutViewBounds, ActiveView.ZoomFactor);
			calculator.HeaderOffset = HeadersSize;
			CellRange layoutRange = new PrintRangeCalculator(calculator.Sheet).CalculateWithoutDefindeName();
			calculator.CalculateLayoutByAnchor(anchor, layoutRange);
			SetScrollPosition();
			ActiveView.VerticalScrollController.UpdateScrollBar();
			ActiveView.HorizontalScrollController.UpdateScrollBar();
			InplaceEditor.UpdateBoundsAndFont();
			InnerCommentInplaceEditor.UpdateBounds();
			DeactivateDataValidationInplaceEditor();
			ResetHoveredCommentTimer();
		}
		protected internal void ScrollToTarget(CellPosition targetPosition) {
			DocumentModel.ApplyChanges(DocumentModelChangeActions.ResetAllLayout);
			Layout.Engine.DocumentLayoutAnchor anchor = new Layout.Engine.DocumentLayoutAnchor();
			anchor.CellPosition = targetPosition;
			CalculateLayout(anchor);
			if (targetPosition.Column >= this.documentLayout.ScrollInfo.ScrollLeftColumnModelIndex)
				anchor.HorizontalFarAlign = true;
			if (targetPosition.Row >= this.documentLayout.ScrollInfo.ScrollTopRowModelIndex)
				anchor.VerticalFarAlign = true;
			if (anchor.HorizontalFarAlign || anchor.VerticalFarAlign) {
				anchor.CellPosition = targetPosition;
				CalculateLayout(anchor);
			}
			SetScrollPosition();
			ActiveView.VerticalScrollController.UpdateScrollBar();
			ActiveView.HorizontalScrollController.UpdateScrollBar();
			InplaceEditor.UpdateBoundsAndFont();
			InnerCommentInplaceEditor.UpdateBounds();
		}
		void SetScrollPosition() {
			DocumentModel.BeginUpdateFromUI();
			try {
				ScrollInfo scrollInfo = this.documentLayout.ScrollInfo;
				this.DocumentModel.ActiveSheet.SetScrollPosition(scrollInfo.ScrollLeftColumnModelIndex, scrollInfo.ScrollTopRowModelIndex);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		void CalculateLayout(Layout.Engine.DocumentLayoutAnchor anchor) {
			SetDocumentLayout(new DocumentLayout(DocumentModel));
			Model.Worksheet sheet = this.DocumentModel.ActiveSheet;
			DocumentLayoutCalculator calculator = new DocumentLayoutCalculator(this.documentLayout, sheet, this.Owner.LayoutViewBounds, ActiveView.ZoomFactor);
			calculator.HeaderOffset = HeadersSize;
			var layoutRange = new PrintRangeCalculator(sheet).CalculateWithoutDefindeName();
			calculator.CalculateLayoutByAnchor(anchor, layoutRange);
		}
		public void BeginInitialize() {
			BeginInitialize(false);
		}
		public override void BeginInitialize(bool keepExistingContent) {
			isDocumentServerInitialized = false;
			base.BeginInitialize(keepExistingContent);
			this.backgroundThreadUIUpdater = new DeferredBackgroundThreadUIUpdater();
			this.errorHandler = new UIErrorHandler(this.Owner);
		}
		public void EndInitialize(bool initializeOnlyDocumentServer) { 
			isDocumentServerInitialized = initializeOnlyDocumentServer;
			if (initializeOnlyDocumentServer) {
				base.EndInitialize();
				InitializeHandlers(); 
			}
			else
				EndInitialize();
		}
		public override void EndInitialize() {
			if (!isDocumentServerInitialized) {
				base.EndInitialize();
				InitializeHandlers(); 
			}
			this.verticalScrollbar = Owner.CreateVerticalScrollBar();
			this.horizontalScrollbar = Owner.CreateHorizontalScrollBar();
			this.tabSelector = Owner.CreateTabSelector();
			if (endInitializeIsCalled) { 
				DocumentModel.IncrementTransactionVersion(); 
				Views.RecreateScrollControllers();
				SetActiveViewCore(views.GetViewByType(ActiveViewType));
				return;
			}
			AddServices();
			CreateViews();
			CreateInplaceEditors();
			this.innerPivotTableFieldsPanel = new InnerPivotTableFieldsPanel(this);
			this.endInitializeIsCalled = true;
		}
		protected internal virtual void CreateInplaceEditors() {
			this.inplaceEditor = new InnerCellInplaceEditor(this);
			this.innerCommentInplaceEditor = new InnerCommentInplaceEditor(this);
			this.innerDataValidationInplaceEditor = new InnerDataValidationInplaceEditor(this);
		}
		protected internal virtual void InitializeHandlers() {
			InitializeKeyboardHandlers();
			InitializeMouseHandlers();
		}
		protected internal virtual void AddServices() {
			AddService(typeof(IThreadSyncService), ThreadSyncService.Create());
			AddService(typeof(IKeyboardHandlerService), new SpreadsheetKeyboardHandlerService(this));
			AddService(typeof(IMouseHandlerService), new SpreadsheetMouseHandlerService(this));
#if !SL && !DOTNET
			AddService(typeof(IRangeSecurityService), new RangeSecurityService());
#endif
		}
		public virtual SpreadsheetCommand CreateCommand(SpreadsheetCommandId commandId) {
			ISpreadsheetCommandFactoryService service = GetService<ISpreadsheetCommandFactoryService>();
			if (service == null)
				return null;
			return service.CreateCommand(commandId);
		}
		public override void OnUpdateUI() {
			if (UpdateUIOnIdle)
				ForceUpdateUIOnIdle = true;
			else
				OnUpdateUICore();
		}
		protected internal virtual void OnUpdateUICore() {
			if (IsUpdateLocked)
				deferredChanges.RaiseUpdateUI = true;
			else
				RaiseUpdateUI();
		}
		protected internal virtual void OnIsReadOnlyChanged() {
			RaiseReadOnlyChanged();
			if (IsReadOnly) {
			}
			else {
			}
			ResetPivotTableFieldsPanelVisibility();
			OnUpdateUI();
		}
		public void OnIsEnabledChanged() {
			ResetPivotTableFieldsPanelVisibility();
			OnUpdateUI();
		}
		public override void OnApplicationIdle() {
			base.OnApplicationIdle();
			if (forceUpdateUIOnIdle) {
				OnUpdateUICore();
				forceUpdateUIOnIdle = false;
			}
		}
		protected internal override void OnBeginDocumentUpdateCore() {
			base.OnBeginDocumentUpdateCore();
			this.deferredChanges = new SpreadsheetControlDeferredChanges();
			BeginScrollbarUpdate(HorizontalScrollBar);
			BeginScrollbarUpdate(VerticalScrollBar);
		}
		protected internal override DocumentModelChangeActions OnEndDocumentUpdateCore(object sender, DocumentUpdateCompleteEventArgs e) {
			DocumentModelDeferredChanges deferredChanges = e.DeferredChanges;
			DocumentModelChangeActions changeActions = deferredChanges.ChangeActions;
			if ((changeActions & DocumentModelChangeActions.RaiseEmptyDocumentCreated) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseDocumentLoaded) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseContentChanged) != 0 ||
				(changeActions & DocumentModelChangeActions.ResetAllLayout) != 0)
				ResetDocumentLayout();
			if ((changeActions & DocumentModelChangeActions.ResetHeaderContent) != 0)
				ResetHeaderContent();
			if ((changeActions & DocumentModelChangeActions.ResetPivotTableFieldsPanelVisibility) != 0)
				UpdatePivotTableFieldsPanelVisibility();
			deferredChanges.ChangeActions = BeforeLastEndUpdate(changeActions);
			return base.OnEndDocumentUpdateCore(sender, e);
		}
		public void ResetDocumentLayout() {
			this.documentLayout = null;
			HideHoveredComment();
		}
		void ResetHeaderContent() {
			if (documentLayout == null || documentLayout.HeaderPage == null)
				return;
			documentLayout.HeaderPage.InvalidateContent();
		}
		public void ResetAndRedrawHeader() {
			ResetHeaderContent();
			Owner.Redraw();
		}
		public void HideHoveredComment() {
			SpreadsheetMouseHandler handler = MouseHandler as SpreadsheetMouseHandler;
			if (handler != null)
				handler.HideHoveredComment(false);
		}
		void ResetHoveredCommentTimer() {
			SpreadsheetMouseHandler handler = MouseHandler as SpreadsheetMouseHandler;
			if (handler != null)
				handler.ResetCommentTimer();
		}
		protected internal virtual void ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions) {
			Owner.ApplyChangesCorePlatformSpecific(changeActions);
		}
		DocumentModelChangeActions BeforeLastEndUpdate(DocumentModelChangeActions changeActions) {
			EndScrollbarUpdate(VerticalScrollBar);
			EndScrollbarUpdate(HorizontalScrollBar);
#if !SL
			if (deferredChanges.Resize)
				OnResizeCore();
#endif
			if (deferredChanges.Redraw) {
				changeActions |= DocumentModelChangeActions.Redraw;
			}
			if (deferredChanges.RaiseUpdateUI)
				RaiseUpdateUI();
			this.deferredChanges = null;
			return changeActions;
		}
		protected internal virtual void BeginScrollbarUpdate(IOfficeScrollbar scrollbar) {
			if (scrollbar != null)
				scrollbar.BeginUpdate();
		}
		protected internal virtual void EndScrollbarUpdate(IOfficeScrollbar scrollbar) {
			if (scrollbar != null)
				scrollbar.EndUpdate();
		}
		protected internal override void OnInnerSelectionChanged(object sender, EventArgs e) {
			ResetPivotTableFieldsPanelVisibility();
			base.OnInnerSelectionChanged(sender, e);
			if (ActiveView != null)
				ActiveView.OnSelectionChanged();
			SwitchKeyboardHandler();
			if (!IsUpdateLocked)
				ResetAndRedrawHeader();
		}
		protected internal override void OnInnerActiveSheetChanged(object sender, ActiveSheetChangedEventArgs e) {
			ResetPivotTableFieldsPanelVisibility();
			base.OnInnerActiveSheetChanged(sender, e);
		}
		protected internal void SwitchKeyboardHandler() {
			bool isPictureKeyboardHandlerActive = (KeyboardHandler as PictureKeyboardHandler) != null;
			if (DocumentModel.ActiveSheet.Selection.IsDrawingSelected) {
				if (!isPictureKeyboardHandlerActive)
					SetNewKeyboardHandler(pictureKeyboardHandler);
			}
			else if (isPictureKeyboardHandlerActive)
				RestoreKeyboardHandler();
		}
		#region ActiveView
		protected internal virtual SpreadsheetViewRepository CreateViewRepository() {
			return Owner.CreateViewRepository();
		}
		protected internal virtual void CreateViews() {
			this.views = CreateViewRepository();
			if (activeViewTypeBeforeActiveViewCreation != null)
				SetActiveViewCore(views.GetViewByType((SpreadsheetViewType)activeViewTypeBeforeActiveViewCreation));
			else
				SetActiveViewCore(views.GetViewByType(DefaultViewType));
		}
		protected internal virtual void DisposeViews() {
			if (views != null) {
				views.Dispose();
				views = null;
			}
			activeView = null;
		}
		protected internal virtual void SetActiveView(SpreadsheetView newView) {
			BeginUpdate();
			try {
				SetActiveViewCore(newView);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void SetActiveViewCore(SpreadsheetView newView) {
			Guard.ArgumentNotNull(newView, "newView");
			Rectangle viewBounds;
			if (this.activeView != null)
				viewBounds = DeactivateView(this.activeView);
			else
				viewBounds = new Rectangle(0, 0, 10, 10);
			this.activeView = newView;
			viewBounds = CalculateActualViewBounds(viewBounds);
			viewBounds.X = 0;
			viewBounds.Y = 0;
			ActivateView(this.activeView, viewBounds);
			RaiseActiveViewChanged();
			ActiveView.CorrectZoomFactor();
		}
		protected internal virtual Rectangle DeactivateView(SpreadsheetView view) {
			if (view.IsDisposed)
				return Rectangle.Empty;
			UnsubscribeActiveViewEvents();
			DeactivateViewPlatformSpecific(view);
			view.Deactivate();
			return view.Bounds;
		}
		protected internal virtual void DeactivateViewAndClearActiveView(SpreadsheetView view) {
			activeViewTypeBeforeActiveViewCreation = ActiveViewType;
			DeactivateView(view);
			activeView = null;
		}
		protected internal virtual void ActivateView(SpreadsheetView view, Rectangle viewBounds) {
			System.Diagnostics.Debug.Assert(viewBounds.Location == Point.Empty);
			ActiveView.CorrectZoomFactor();
			ActivateViewPlatformSpecific(view);
			view.Activate(viewBounds);
			SubscribeActiveViewEvents();
		}
		protected internal virtual void SubscribeActiveViewEvents() {
			ActiveView.ZoomChanging += OnActiveViewZoomChanging;
			ActiveView.ZoomChanged += OnActiveViewZoomChanged;
			ActiveView.BackColorChanged += OnActiveViewBackColorChanged;
		}
		protected internal virtual void UnsubscribeActiveViewEvents() {
			ActiveView.ZoomChanging -= OnActiveViewZoomChanging;
			ActiveView.ZoomChanged -= OnActiveViewZoomChanged;
			ActiveView.BackColorChanged -= OnActiveViewBackColorChanged;
		}
		protected internal virtual void OnActiveViewZoomChanging(object sender, EventArgs e) {
			BeginUpdate();
			OnZoomFactorChangingPlatformSpecific();
		}
		protected internal virtual void OnActiveViewZoomChanged(object sender, EventArgs e) {
			OnResizeCore();
			UpdateVerticalScrollBar(false);
			EndUpdate();
			RaiseZoomChanged();
		}
		protected internal virtual void OnActiveViewBackColorChanged(object sender, EventArgs e) {
		}
		protected internal virtual void ActivateViewPlatformSpecific(SpreadsheetView view) {
			Owner.ActivateViewPlatformSpecific(view);
		}
		protected internal virtual void DeactivateViewPlatformSpecific(SpreadsheetView view) {
			Owner.DeactivateViewPlatformSpecific(view);
		}
		protected internal virtual void OnZoomFactorChangingPlatformSpecific() {
		}
		protected internal virtual void OnResizeCore() {
			Owner.OnResizeCore();
		}
		protected internal virtual Rectangle CalculateActualViewBounds(Rectangle previousViewBounds) {
			return Rectangle.Empty;
		}
		#endregion
		#region Load/Save content
		public virtual void LoadDocument() {
			LoadDocument(Owner);
		}
		public virtual void LoadDocument(IWin32Window parent) {
			LoadDocumentCore(parent);
		}
		protected internal virtual void LoadDocumentCore(IWin32Window parent) {
			IDocumentImportManagerService importManagerService = GetService<IDocumentImportManagerService>();
			DocumentImportHelper importHelper = new DocumentImportHelper(DocumentModel);
			ImportSource<DocumentFormat, bool> importSource = importHelper.InvokeImportDialog(parent, importManagerService);
			if (importSource == null)
				return;
			Cursor.Current = SpreadsheetCursors.WaitCursor.Cursor;
			LoadDocument(importSource.Storage, importSource.Importer.Format);
			Cursor.Current = SpreadsheetCursors.Default.Cursor;
		}
		public virtual bool SaveDocumentAs() {
			return SaveDocumentAs(Owner);
		}
		public virtual bool SaveDocumentAs(IWin32Window parent) {
			IDocumentExportManagerService exportManagerService = GetService<IDocumentExportManagerService>();
			DocumentExportHelper exportHelper = new DocumentExportHelper(DocumentModel);
			ExportTarget<DocumentFormat, bool> target = exportHelper.InvokeExportDialog(parent, exportManagerService);
			if (target == null)
				return false;
#if (!SL)
			Cursor oldCursor = Cursor.Current;
#endif
			try {
#if (!SL)
				Cursor.Current = SpreadsheetCursors.WaitCursor.Cursor;
#endif
				SaveDocument(target.Storage, target.Exporter.Format);
			}
			finally {
#if (!SL)
				Cursor.Current = oldCursor;
#else
				if (target.Storage != null)
					target.Storage.Close();
#endif
			}
			return true;
		}
		public virtual bool SaveDocument() {
			return SaveDocument(Owner);
		}
		public virtual bool SaveDocument(IWin32Window parent) {
			WorkbookSaveOptions documentSaveOptions = DocumentModel.DocumentSaveOptions;
			if (!documentSaveOptions.CanSaveToCurrentFileName || String.IsNullOrEmpty(documentSaveOptions.CurrentFileName) || documentSaveOptions.CurrentFormat == DocumentFormat.Undefined)
				return SaveDocumentAs(parent);
			else {
				SaveDocument(documentSaveOptions.CurrentFileName, documentSaveOptions.CurrentFormat);
				return true;
			}
		}
		#endregion
		#region IDisposable implementation
		protected internal override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (inplaceEditor != null) {
						if (inplaceEditor.IsActive) {
							inplaceEditor.Rollback();
							inplaceEditor.Deactivate(false);
						}
						inplaceEditor = null;
					}
					if (innerCommentInplaceEditor != null) {
						if (innerCommentInplaceEditor.IsActive)
							innerCommentInplaceEditor.Deactivate();
						innerCommentInplaceEditor = null;
					}
					if (innerDataValidationInplaceEditor != null)
						innerDataValidationInplaceEditor = null;
					if (innerPivotTableFieldsPanel != null) {
						innerPivotTableFieldsPanel.HidePanel();
						innerPivotTableFieldsPanel = null;
					}
					DisposeViews();
					if (keyboardHandlers != null) {
						keyboardHandlers.Clear();
						keyboardHandlers = null;
					}
					pictureKeyboardHandler = null;
					if (mouseHandlers != null) {
						mouseHandlers.Clear();
						mouseHandlers = null;
					}
					if (defaultMouseHandler != null) {
						defaultMouseHandler.Dispose();
						defaultMouseHandler = null;
					}
					this.horizontalScrollbar = null;
					this.verticalScrollbar = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal override void ApplyChangesCore(DocumentModelChangeActions changeActions) {
			if (!DocumentModel.IsUpdateLocked)
				ApplyChangesCorePlatformSpecific(changeActions);
			base.ApplyChangesCore(changeActions);
		}
		#region SetLayoutUnit
		protected internal override void SetLayoutUnitCore(DocumentLayoutUnit unit) {
			base.SetLayoutUnitCore(unit);
			if (ActiveView == null)
				return;
			Owner.ResizeView(); 
			UpdateVerticalScrollBar(false);
			ActiveView.UpdateHorizontalScrollbar();
			Owner.Redraw();
		}
		protected internal override void SetDocumentModelLayoutUnitCore(DocumentLayoutUnit unit) {
			if (ActiveView != null)
				ActiveView.OnLayoutUnitChanging();
			base.SetDocumentModelLayoutUnitCore(unit);
			if (ActiveView != null)
				ActiveView.OnLayoutUnitChanged();
		}
		#endregion
		protected internal virtual void UpdateVerticalScrollBar(bool avoidJump) {
			if (ActiveView == null)
				return;
			if (avoidJump)
				ActiveView.VerticalScrollController.ScrollBarAdapter.SynchronizeScrollBarAvoidJump();
			else
				ActiveView.VerticalScrollController.ScrollBarAdapter.EnsureSynchronized();
		}
		protected internal override void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			base.OnOptionsChanged(sender, e);
			OnOptionsChangedPlatformSpecific(e);
			OnUpdateUI();
		}
		protected internal virtual void OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
			if (ShouldResetHeaderOnOptionsChange(e))
				ResetHeaderContent();
			if (ShouldResetLayoutOnOptionsChange(e)) {
				if (e.Name == "ShowColumnHeaders" || e.Name == "ColumnHeaderHeight") {
					BeginUpdate();
					ResetDocumentLayout();
					OnResizeCore();
					UpdateVerticalScrollBar(false);
					EndUpdate();
					Owner.Redraw();
				}
				else {
					ResetDocumentLayout();
					Owner.Redraw();
				}
			}
			else if (ShouldRedrawOnOptionsChange(e))
				Owner.Redraw();
			else
				Owner.OnOptionsChangedPlatformSpecific(e);
		}
		bool ShouldRedrawOnOptionsChange(BaseOptionChangedEventArgs e) {
			if (e.Name == "FillHandle.Enabled")
				return true;
			if (e.Name == "Drag")
				return true;
			if (e.Name.StartsWith("Drawing."))
				return true;
			if (e.Name == "Charts.Antialiasing")
				return true;
			if (e.Name == "Charts.TextAntialiasing")
				return true;
			if (e.Name == "Pictures.HighQualityScaling")
				return true;
			if (e.Name == "Comment.Move")
				return true;
			if (e.Name == "Comment.Resize")
				return true;
			if (e.Name == "Selection.AllowExtendSelection")
				return true;
			if (e.Name == "Selection.HideSelection")
				return true;
			return false;
		}
		bool ShouldResetLayoutOnOptionsChange(BaseOptionChangedEventArgs e) {
			if (e.Name == "ShowColumnHeaders")
				return true;
			if (e.Name == "ShowRowHeaders")
				return true;
			if (e.Name == "ShowPrintArea")
				return true;
			if (e.Name == "ColumnHeaderHeight")
				return true;
			if (e.Name == "RowHeaderWidth")
				return true;
			return false;
		}
		bool ShouldResetHeaderOnOptionsChange(BaseOptionChangedEventArgs e) {
			if (e.Name == "Selection.HideSelection")
				return true;
			return false;
		}
		protected internal override void OnActiveSheetChanged(object sender, ActiveSheetChangedEventArgs e) {
			HideHoveredComment();
			base.OnActiveSheetChanged(sender, e);
		}
		protected internal override void OnActiveSheetChanging(object sender, ActiveSheetChangingEventArgs e) {
			if (!IsInplaceEditorActive) {
				RaiseActiveSheetChanging(e);
				return;
			}
			InplaceEndEditCommand command = new InplaceEndEditCommand(Owner);
			command.Execute();
			if (command.CommitSuccessfull)
				RaiseActiveSheetChanging(e);
			else
				e.Cancel = true;
		}
		public DevExpress.Spreadsheet.Cell GetCellFromPoint(Point point) {
			DevExpress.Spreadsheet.Worksheet sheet = this.ActiveApiWorksheet;
			if (sheet == null)
				return null;
			SpreadsheetHitTestResult hitTestResult = ActiveView.CalculatePageHitTest(point);
			if (hitTestResult == null)
				return null;
			if (!hitTestResult.IsValid(DocumentLayoutDetailsLevel.Cell))
				return null;
			return sheet[hitTestResult.CellPosition.Row, hitTestResult.CellPosition.Column];
		}
		public bool CanEditActiveCellContent(bool checkAccessRights) {
			return this.DocumentModel.CanEditActiveCellContent(checkAccessRights);
		}
		public bool TryEditActiveCellContent() {
			Model.Worksheet activeSheet = DocumentModel.ActiveSheet;
			if (activeSheet.ReadOnly)
				return false;
			if (CanEditActiveCellContent(true))
				return true;
			IList<ModelProtectedRange> ranges = DocumentModel.ObtainActiveCellProtectedRanges(activeSheet, activeSheet.Selection.ActiveCell);
			if (ranges.Count == 0) {
				ShowReadOnlyObjectMessage();
				return false;
			}
			foreach (ModelProtectedRange range in ranges)
				if (range.IsAccessGranted)
					return true;
			UnprotectRangeViewModel viewModel = new UnprotectRangeViewModelActiveCell(Owner, activeSheet.ProtectedRanges, activeSheet.Selection.ActiveCell);
			Owner.ShowUnprotectRangeForm(viewModel);
			foreach (ModelProtectedRange range in ranges)
				if (range.IsAccessGranted)
					return true;
			return false; 
		}
		public bool TryEditRangeContent(CellRange targetRange) {
			Model.Worksheet sheet = targetRange.Worksheet as Model.Worksheet;
			return sheet.TryEditRangeContent(targetRange, ShowUnprotectRangeForm, ShowReadOnlyObjectMessage);
		}
		public void ShowUnprotectRangeForm(CellRange targetRange) {
			ModelProtectedRangeCollection collection = (targetRange.Worksheet as Model.Worksheet).ProtectedRanges;
			UnprotectRangeViewModel viewModel = new UnprotectRangeViewModelTargetRange(Owner, collection, targetRange);
			Owner.ShowUnprotectRangeForm(viewModel);
		}
		public void ShowReadOnlyObjectMessage() {
			IThreadSyncService service = GetService<IThreadSyncService>();
			if (service != null) {
				Action action = delegate() {
					if (!IsDisposed && Owner != null) {
						if (!RaiseProtectionWarning()) {
							Owner.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_CellOrChartIsReadonly));
							Owner.Focus();
						}
					}
				};
				service.EnqueueInvokeInUIThread(action);
			}
		}
		public bool IsCellVisible(CellPosition position) {
			int count = DesignDocumentLayout.Pages.Count;
			for (int i = 0; i < count; i++)
				if (IsCellVisible(DesignDocumentLayout.Pages[i], position))
					return true;
			return false;
		}
		bool IsCellVisible(Page page, CellPosition position) {
			return page.GridRows.ActualFirst.ModelIndex <= position.Row && position.Row <= page.GridRows.ActualLast.ModelIndex &&
				page.GridColumns.ActualFirst.ModelIndex <= position.Column && position.Column <= page.GridColumns.ActualLast.ModelIndex;
		}
		public void OnBeginPaint() {
			DocumentModel.OnBeginPainting();
		}
		public void OnEndPaint() {
			DocumentModel.OnEndPainting();
		}
	}
	public interface IInnerSpreadsheetControlOwner : ISpreadsheetControl, IInnerSpreadsheetDocumentServerOwner {
		bool IsEnabled { get; }
		SpreadsheetMouseHandler CreateMouseHandler();
		SpreadsheetViewRepository CreateViewRepository();
		ICellInplaceEditor CreateCellInplaceEditor(bool formulaBarFocused);
		IOfficeScrollbar CreateVerticalScrollBar();
		IOfficeScrollbar CreateHorizontalScrollBar();
		ITabSelector CreateTabSelector();
		ICommentInplaceEditor CreateCommentInplaceEditor();
		IDataValidationInplaceEditor CreateDataValidationInplaceEditor();
		void ActivateViewPlatformSpecific(SpreadsheetView view);
		void DeactivateViewPlatformSpecific(SpreadsheetView view);
		void Redraw();
		void Redraw(RefreshAction action);
		void ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions);
		void OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e);
		void ResizeView();
		void OnResizeCore();
		bool ReadOnly { get; set; }
	}
}

#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class WinColumnsListEditor : ColumnsListEditor, IControlOrderProvider, IDXPopupMenuHolder, IComplexListEditor, IHtmlFormattingSupport, IFocusedElementCaptionProvider, ISupportAppearanceCustomization, ISupportEnabledCustomization, IExportable, ISupportFilter, ISupportUpdate, IGridViewOptions, IDataAwareExportableCsv, IDataAwareExportableXls, IDataAwareExportableXlsx {
		internal const string InvalidGridViewType = "The specified ColumnView object should implement the IModelSynchronizersHolder interface";
		private const bool CanManageActiveFilterCriteriaPropertyValueDefault = true;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static Int32 PageRowCountForServerMode = 100;
		public const string DragEnterCustomCodeId = "DragEnter";
		public const string DragDropCustomCodeId = "DragDrop";
		private RepositoryEditorsFactory repositoryFactory;
		private bool readOnlyEditors = false;
		private GridControl grid;
		private IGridControlDataSourceAdapter gridDataSourceAdapter;
		private IColumnViewActiveFilterStringAdapter gridActiveFilterStringAdapter;
		private bool allowGridActiveFilterStringAdapterSync = true;
		private bool lockActiveFilterStringChangedEventHandler = false;
		private ColumnView view;
		private bool focusedChangedRaised;
		private bool selectedChangedRaised;
		private bool isForceSelectRow;
		private int prevFocusedRowHandle;
		private CollectionSourceBase collectionSource;
		private ActionsDXPopupMenu popupMenu;
		private XafApplication application;
		private ColumnFilterMode lookupColumnFilterMode = ColumnFilterMode.Value;
		private Boolean isGridDataSourceChanging;
		private bool htmlFormattingEnabled;
		private string filter;
		private bool canManageActiveFilterCriteriaPropertyValue = CanManageActiveFilterCriteriaPropertyValueDefault;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool RestoreSelectedRowByHandle = true;
		public WinColumnsListEditor(IModelListView model)
			: base(model) {
			FilterColumnsMode = FilterColumnsMode.AllProperties;
			popupMenu = new ActionsDXPopupMenu();
			prevFocusedRowHandle = GridControl.InvalidRowHandle;
		}
		public override void Dispose() {
			BreakLinksToControls();
			ColumnCreated = null;
			GridDataSourceChanging = null;
			FilterChanged = null;
			base.Dispose();
		}
		public override void BreakLinksToControls() {
			base.BreakLinksToControls();
			if(popupMenu != null) {
				popupMenu.Dispose();
				popupMenu = null;
			}
			if(view != null) {
				UnsubscribeGridViewEvents();
				view.Dispose();
				view = null;
			}
			if(grid != null) {
				grid.DataSource = null;
				UnsubscribeFromGridEvents();
				grid.RepositoryItems.Clear();
				((IDisposable)grid).Dispose();
				grid = null;
				OnPrintableChanged();
			}
			gridActiveFilterStringAdapter = null;
			gridDataSourceAdapter = null;
		}
		public string FindColumnPropertyName(GridColumn column) {
			IGridColumnModelSynchronizer info = GetColumnModelSynchronizer(column);
			if(info != null) {
				return info.PropertyName;
			}
			else {
				return null;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RemoveColumn(ColumnWrapper column) {
			GridColumn gridColumn = ((XafGridColumnWrapper)column).Column;
			RemoveColumn(gridColumn, ColumnView as IModelSynchronizersHolder);
		}
		public virtual void RemoveColumn(GridColumn column, IModelSynchronizersHolder modelSynchronizersHolder) {
			CreateGridViewColumnFactory().RemoveColumn(column, GetLayoutElementCollection(), ColumnView, modelSynchronizersHolder);
		}
		public override void Refresh() {
			if(grid != null) {
				SaveFocusedRowHandle(view.FocusedRowHandle);
				if(IsAsyncServerMode) {
					CollectionSource.ResetCollection();
				}
				grid.RefreshDataSource();
				RestoreFocusedRow();
			}
		}
		public override IList GetSelectedObjects() {
			ArrayList selectedObjects = new ArrayList();
			if(ColumnView != null) {
				int[] selectedRows = ColumnView.GetSelectedRows();
				if((selectedRows != null) && (selectedRows.Length > 0)) {
					foreach(int rowHandle in selectedRows) {
						if(!IsGroupRowHandle(rowHandle)) {
							object obj = XtraGridUtils.GetRow(CollectionSource, ColumnView, rowHandle);
							if(obj != null) {
								selectedObjects.Add(obj);
							}
						}
					}
				}
			}
			return selectedObjects.ToArray(typeof(object));
		}
		public override Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return
				(dataAccessMode == CollectionSourceDataAccessMode.Client)
				||
				(dataAccessMode == CollectionSourceDataAccessMode.Server)
				||
				(dataAccessMode == CollectionSourceDataAccessMode.DataView);
		}
		public override String[] RequiredProperties {
			get {
				List<String> result = new List<String>();
				if(Model != null) {
					foreach(IModelColumn columnInfo in Model.Columns) {
						if(!columnInfo.Index.HasValue || (columnInfo.Index >= 0) || (columnInfo.GroupIndex >= 0)) {
							GridColumnModelSynchronizer gridColumnInfo = CreateGridColumnModelSynchronizer(columnInfo, ObjectTypeInfo, IsAsyncServerMode, false);
							string fieldName = gridColumnInfo.FieldName;
							if(!result.Contains(fieldName)) {
								result.Add(fieldName);
							}
						}
					}
				}
				return result.ToArray();
			}
		}
		public override IContextMenuTemplate ContextMenuTemplate {
			get { return popupMenu; }
		}
		public override string Name {
			get { return base.Name; }
			set {
				base.Name = value;
				SetTag();
			}
		}
		public override SelectionType SelectionType {
			get { return SelectionType.Full; }
		}
		public RepositoryEditorsFactory RepositoryFactory {
			get { return repositoryFactory; }
			set { repositoryFactory = value; }
		}
		public GridControl Grid {
			get { return grid; }
		}
		public ColumnView ColumnView {
			get { return view; }
		}
		public bool ReadOnlyEditors {
			get { return readOnlyEditors; }
			set {
				if(readOnlyEditors != value) {
					readOnlyEditors = value;
					AllowEdit = !readOnlyEditors;
				}
			}
		}
		public ColumnFilterMode LookupColumnFilterMode {
			get { return lookupColumnFilterMode; }
			set { lookupColumnFilterMode = value; }
		}
		public override IList<ColumnWrapper> Columns {
			get {
				List<ColumnWrapper> result = new List<ColumnWrapper>();
				if(ColumnView != null) {
					foreach(GridColumn column in ColumnView.Columns) {
						IGridColumnModelSynchronizer gridColumnInfo = GetColumnModelSynchronizer(column);
						if(gridColumnInfo != null) {
							result.Add(gridColumnInfo.CreateColumnWrapper(column));
						}
					}
				}
				return result;
			}
		}
		[DefaultValue(FilterColumnsMode.AllProperties)]
		public FilterColumnsMode FilterColumnsMode { get; set; }
		public void BeginUpdate() {
			LockSelectionEvents();
		}
		public void EndUpdate() {
			UnlockSelectionEvents();
		}
		public IPrintable Printable {
			get { return Grid; }
		}
		public List<ExportTarget> SupportedExportFormats {
			get {
				if(Printable == null) {
					return new List<ExportTarget>();
				}
				else {
					return GetSupportedExportFormats;
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual List<ExportTarget> GetSupportedExportFormats {
			get {
				return new List<ExportTarget>(){
				ExportTarget.Csv,
				ExportTarget.Html,
				ExportTarget.Image,
				ExportTarget.Mht,
				ExportTarget.Pdf,
				ExportTarget.Rtf,
				ExportTarget.Text,
				ExportTarget.Xls,
				ExportTarget.Xlsx
				};
			}
		}
		public void OnExporting() {
			if(Grid != null) {
				Grid.MainView.ClearDocument(); 
			}
		}
		protected virtual void OnCustomizeAppearance(string name, IAppearanceBase item, int rowHandle) {
			if(CustomizeAppearance != null) {
				CustomizeAppearanceEventArgs args = null;
				if(!IsAsyncServerMode) {
					args = new CustomizeAppearanceEventArgs(name, item, DevExpress.ExpressApp.Win.Core.XtraGridUtils.GetRow(view, rowHandle));
				}
				else {
					if(!view.IsRowLoaded(rowHandle)) {
						return; 
					}
					args = new CustomizeAppearanceEventArgs(name, item,
						rowHandle, new AsyncServerModeContextDescriptor(view, CollectionSource.ObjectSpace, CollectionSource.ObjectTypeInfo.Type));
				}
				CustomizeAppearance(this, args);
			}
		}
		protected virtual void OnCustomizeEnabled(string name, IAppearanceBase item, int rowHandle) {
			if(CustomizeEnabled != null && CollectionSource != null && !IsAsyncServerMode) {
				if(view.FocusedRowHandle != GridControl.AutoFilterRowHandle && view.FocusedColumn.UnboundType == UnboundColumnType.Bound) {
					object rowObj = DevExpress.ExpressApp.Win.Core.XtraGridUtils.GetRow(CollectionSource, view, view.FocusedRowHandle);
					if(rowObj != null) {
						CustomizeEnabledEventArgs args = new CustomizeEnabledEventArgs(name, item, rowObj, CollectionSource, CollectionSource.ObjectSpace);
						CustomizeEnabled(this, args);
					}
				}
			}
		}
		protected virtual GridControl CreateGridControl() {
			return new GridControl();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected abstract ColumnView CreateGridViewCore();
		protected internal void CancelCurrentRowEdit(ColumnView view) {
			BaseGridController gridController = view.DataController;
			if((gridController != null) && !gridController.IsDisposed && 
			(view.ActiveEditor != null) && (gridController.IsCurrentRowEditing || gridController.IsCurrentRowModified)) {
				gridController.CancelCurrentRowEdit();
			}
		}
		protected virtual void ProcessGridKeyDown(KeyEventArgs e) {
			if(FocusedObject != null && e.KeyCode == Keys.Enter) {
				if(ColumnView.ActiveEditor == null && !ReadOnlyEditors) {
					OnProcessSelectedItem();
					e.SuppressKeyPress = true;
					e.Handled = true;
				}
			}
		}
		protected virtual void OnColumnCreated(GridColumn column, IModelColumn columnInfo) {
			if(ColumnCreated != null) {
				ColumnCreatedEventArgs args = new ColumnCreatedEventArgs(column, columnInfo);
				ColumnCreated(this, args);
			}
		}
		protected override void OnFocusedObjectChanged() {
			base.OnFocusedObjectChanged();
			focusedChangedRaised = true;
		}
		protected override void OnSelectionChanged() {
			base.OnSelectionChanged();
			selectedChangedRaised = true;
			if(ColumnView.SelectedRowsCount == 0 && isForceSelectRow) {
				XtraGridUtils.SelectFocusedRow(ColumnView);
			}
		}
		protected virtual void OnGridDataSourceChanging() {
			if(GridDataSourceChanging != null) {
				GridDataSourceChanging(this, EventArgs.Empty);
			}
		}
		protected virtual void SubscribeToGridEvents() {
			grid.HandleCreated += new EventHandler(grid_HandleCreated);
			grid.KeyDown += new KeyEventHandler(grid_KeyDown);
			grid.ParentChanged += new EventHandler(grid_ParentChanged);
			grid.VisibleChanged += new EventHandler(grid_VisibleChanged);
		}
		protected virtual void UnsubscribeFromGridEvents() {
			grid.VisibleChanged -= new EventHandler(grid_VisibleChanged);
			grid.KeyDown -= new KeyEventHandler(grid_KeyDown);
			grid.HandleCreated -= new EventHandler(grid_HandleCreated);
			grid.ParentChanged -= new EventHandler(grid_ParentChanged);
		}
		private void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(Printable));
			}
		}
		protected override object CreateControlsCore() {
			if(grid == null) {
				grid = CreateGridControl();
				((System.ComponentModel.ISupportInitialize)(grid)).BeginInit();
				try {
					grid.MinimumSize = new Size(100, 75);
					grid.Dock = DockStyle.Fill;
					grid.AllowDrop = true;
					SubscribeToGridEvents();
					grid.Height = 100;
					grid.TabStop = true;
					grid.MainView = CreateGridView();
					gridDataSourceAdapter = new XafGridControlDataSourceAdapter(grid);
					gridDataSourceAdapter.ControlDataSourceChanging += gridDataSourceManager_DataSourceChanging;
					gridDataSourceAdapter.ControlDataSourceChanged += gridDataSourceManager_DataSourceChanged;
					if(CanManageActiveFilterCriteriaPropertyValue) {
						if(CollectionSource != null) {
							gridActiveFilterStringAdapter = new XafColumnViewActiveFilterStringAdapter(grid, view,
								new ObjectSpaceCriteriaOperatorParser(CollectionSource.ObjectSpace, IsAsyncServerMode, CollectionSource.ObjectTypeInfo ),
								gridDataSourceAdapter);
						}
						else {
							gridActiveFilterStringAdapter = new Core.ModelEditor.InternalDesigner.DesignXafColumnViewActiveFilterStringAdapter(grid, view);
						}
						gridActiveFilterStringAdapter.ActiveFilterStringChanged += gridActiveFilterStringManager_ActiveFilterStringChanged;
						gridActiveFilterStringAdapter.ActiveFilterString = filter;
					}
					SetupGridViewCore();
					SetGridViewOptionsCore();
					ApplyModel();
					SetTag();
				}
				finally {
					((System.ComponentModel.ISupportInitialize)(grid)).EndInit();
					grid.ForceInitialize();
				}
				OnPrintableChanged();
			}
			return grid;
		}
		void gridActiveFilterStringManager_ActiveFilterStringChanged(object sender, EventArgs e) {
			if(lockActiveFilterStringChangedEventHandler) {
				return;
			}
			try {
				allowGridActiveFilterStringAdapterSync = false;
				this.Filter = ((IColumnViewActiveFilterStringAdapter)sender).ActiveFilterString;
			}
			finally {
				allowGridActiveFilterStringAdapterSync = true;
			}
		}
		protected override void OnControlsCreated() {
			base.OnControlsCreated();
			gridDataSourceAdapter.DataSource = DataSource;
		}
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			if(gridDataSourceAdapter != null) {
				gridDataSourceAdapter.DataSource = DataSource; 
			}
		}
		private void gridDataSourceManager_DataSourceChanging(object sender, EventArgs e) {
			if(grid.IsHandleCreated) {
				if(grid.Visible) {
					focusedChangedRaised = false;
					selectedChangedRaised = false;
					LockSelectionEvents();
					grid.BeginUpdate();
					isGridDataSourceChanging = true;
					OnGridDataSourceChanging();
					UnsubscribeFromDataControllerEventsCore();
					CancelCurrentRowEdit(view);
				}
			}
		}
		private void gridDataSourceManager_DataSourceChanged(object sender, EventArgs e) {
			if(grid.IsHandleCreated) {
				if(grid.Visible) {
					SubscribeToDataControllerEventsCore();
					isGridDataSourceChanging = false;
					if(view.FocusedRowHandle > 0) {
						view.FocusedRowHandle = 0;
					}
					XtraGridUtils.SelectFocusedRow(view);
					grid.EndUpdate();
					UnlockSelectionEvents();
					if(!selectedChangedRaised) {
						OnSelectionChanged();
					}
					if(!focusedChangedRaised) {
						OnFocusedObjectChanged();
					}
				}
			}
			if(ControlDataSourceChanged != null) {
				ControlDataSourceChanged(this, EventArgs.Empty);
			}
		}
		protected override void AssignDataSourceToControl(Object dataSource) {
		}
		protected override void OnProtectedContentTextChanged() {
			base.OnProtectedContentTextChanged();
			repositoryFactory.ProtectedContentText = ProtectedContentText;
		}
		protected override void OnAllowEditChanged() {
			UpdateAllowEditGridViewAndColumnsOptionsCore();
			base.OnAllowEditChanged();
		}
		protected override void OnErrorMessagesChanged() {
			base.OnErrorMessagesChanged();
			if(grid != null && view != null) {
				grid.Refresh();
				view.LayoutChanged();
			}
		}
		protected override ColumnWrapper AddColumnCore(IModelColumn columnInfo) {
			IRepositoryItemCreator repositoryItemCreator = null;
			if(repositoryFactory != null) {
				repositoryItemCreator = new RepositoryItemCreator(repositoryFactory, AllowEdit, ReadOnlyEditors);
			}
			GridViewColumnFactory gridViewCollumnFactory = CreateGridViewColumnFactory();
			try {
				SubscribeColumnFactoryEvents(gridViewCollumnFactory);
				ColumnDataContainer columnDataContainer = gridViewCollumnFactory.AddColumn(columnInfo, view, repositoryItemCreator, this, view as IModelSynchronizersHolder); 
				if(columnDataContainer.Column.ColumnEdit != null) {
					Grid.RepositoryItems.Add(columnDataContainer.Column.ColumnEdit);
				}
				OnColumnCreated(columnDataContainer.Column, columnDataContainer.GridColumnModelSynchronizer.Model);
				if(!Grid.IsLoading && view.DataController.Columns.GetColumnIndex(columnDataContainer.Column.FieldName) == -1) {
					view.DataController.RePopulateColumns(); 
				}
				return columnDataContainer.GridColumnModelSynchronizer.CreateColumnWrapper(columnDataContainer.Column);
			}
			finally {
				UnsubscribeColumnFactoryEvents(gridViewCollumnFactory);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual GridViewColumnFactory CreateGridViewColumnFactory() {
			if(CanShowBands) {
				return new BandedGridViewColumnFactory();
			}
			else {
				return new GridViewColumnFactory();
			}
		}
		internal static bool GetCanShowBands(IModelListView model) {
			return model.BandsLayout.Enable && !DesignTimeTools.IsDesignMode;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected bool CanShowBands {
			get {
				return Model != null && GetCanShowBands(Model);
			}
		}
		protected virtual GridColumnModelSynchronizer CreateGridColumnModelSynchronizer(IModelColumn modelColumn, ITypeInfo objectTypeInfo, bool isAsyncServerMode, bool isProtectedColumn) {
			return CreateGridViewColumnFactory().CreateGridColumnModelSynchronizer(modelColumn, objectTypeInfo, isAsyncServerMode, isProtectedColumn);
		}
		protected IGridColumnModelSynchronizer GetColumnModelSynchronizer(GridColumn column) {
			IGridColumnModelSynchronizer result = null;
			if(view is IModelSynchronizersHolder) {
				result = ((IModelSynchronizersHolder)view).GetSynchronizer(column) as IGridColumnModelSynchronizer;
			}
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void SubscribeColumnFactoryEvents(GridViewColumnFactory columnFactory) {
			columnFactory.CreateCustomColumnCore += gridViewCollumnFactory_CreateCustomColumnCore;
			columnFactory.CustomizeRepositoryItem += gridViewCollumnFactory_CustomizeRepositoryItem;
			columnFactory.CreateCustomRepositoryItem += gridViewCollumnFactory_CreateCustomRepositoryItem;
			columnFactory.CreateCustomColumn += columnFactory_CreateCustomColumn;
			columnFactory.CustomizeGridColumn += columnFactory_CustomizeGridColumn;
			BandedGridViewColumnFactory bandColumnFactory = columnFactory as BandedGridViewColumnFactory;
			if(bandColumnFactory != null) {
				bandColumnFactory.CreateCustomGridBand += OnCreateCustomGridBand;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void UnsubscribeColumnFactoryEvents(GridViewColumnFactory columnFactory) {
			columnFactory.CustomizeRepositoryItem -= gridViewCollumnFactory_CustomizeRepositoryItem;
			columnFactory.CreateCustomColumnCore -= gridViewCollumnFactory_CreateCustomColumnCore;
			columnFactory.CreateCustomRepositoryItem -= gridViewCollumnFactory_CreateCustomRepositoryItem;
			columnFactory.CreateCustomColumn -= columnFactory_CreateCustomColumn;
			columnFactory.CustomizeGridColumn -= columnFactory_CustomizeGridColumn;
			BandedGridViewColumnFactory bandColumnFactory = columnFactory as BandedGridViewColumnFactory;
			if(bandColumnFactory != null) {
				bandColumnFactory.CreateCustomGridBand -= OnCreateCustomGridBand;
			}
		}
		protected internal bool IsAsyncServerMode {
			get {
				CollectionSource source = CollectionSource as CollectionSource;
				return ((source != null) && source.IsServerMode && source.IsAsyncServerMode);
			}
		}
		internal void PopulateColumns() {
			IRepositoryItemCreator repositoryItemCreator = null;
			if(repositoryFactory != null) {
				repositoryItemCreator = new RepositoryItemCreator(repositoryFactory, AllowEdit, ReadOnlyEditors);
			}
			GridViewColumnFactory gridViewCollumnFactory = CreateGridViewColumnFactory();
			try {
				SubscribeColumnFactoryEvents(gridViewCollumnFactory);
				IModelSynchronizersHolder columnModelSynchronizersHolder = view as IModelSynchronizersHolder;
				foreach(GridColumn column in gridViewCollumnFactory.PopulateColumns(GetLayoutElementCollection(), view, repositoryItemCreator, this, columnModelSynchronizersHolder)) { 
					if(column.ColumnEdit != null) {
						Grid.RepositoryItems.Add(column.ColumnEdit);
					}
					IModelColumn columnModel = null;
					if(columnModelSynchronizersHolder != null) {
						IModelSynchronizer columnModelSynchronizer = columnModelSynchronizersHolder.GetSynchronizer(column);
						if(columnModelSynchronizer != null) {
							columnModel = columnModelSynchronizer.Model as IModelColumn;
						}
					}
					OnColumnCreated(column, columnModel);
				}
			}
			finally {
				UnsubscribeColumnFactoryEvents(gridViewCollumnFactory);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual ModelLayoutElementCollection GetLayoutElementCollection() {
			if(CanShowBands) {
				return new ModelLayoutElementCollection(new ModelBandLayoutItemCollection(Model));
			}
			else {
				return new ModelLayoutElementCollection(Model.Columns);
			}
		}
		internal bool HasProtectedContentInternal(string propertyName) {
			return HasProtectedContent(propertyName);
		}
		protected override bool HasProtectedContent(string propertyName) {
			return !(ObjectTypeInfo.FindMember(propertyName) == null || DataManipulationRight.CanRead(ObjectTypeInfo.Type, propertyName, null, collectionSource, collectionSource != null ? collectionSource.ObjectSpace : null));
		}
		protected XafApplication Application {
			get { return application; }
		}
		protected ActionsDXPopupMenu PopupMenu {
			get {
				return popupMenu;
			}
		}
		bool ISupportFilter.FilterEnabled {
			get {
				return ColumnView.ActiveFilterEnabled;
			}
			set {
				ColumnView.ActiveFilterEnabled = value;
			}
		}
		public string Filter {
			get { return filter; }
			set {
				if(filter != value) {
					filter = value;
					if(gridActiveFilterStringAdapter != null && allowGridActiveFilterStringAdapterSync) {
						try {
							lockActiveFilterStringChangedEventHandler = true;
							gridActiveFilterStringAdapter.ActiveFilterString = filter;
						}
						finally {
							lockActiveFilterStringChangedEventHandler = false;
						}
					}
					if(FilterChanged != null) {
						FilterChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		public event EventHandler<EventArgs> FilterChanged;
		[DefaultValue(CanManageActiveFilterCriteriaPropertyValueDefault), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanManageActiveFilterCriteriaPropertyValue {
			get { return canManageActiveFilterCriteriaPropertyValue; }
			set { canManageActiveFilterCriteriaPropertyValue = value; } 
		}
		private BaseEdit GetEditor(Object sender) {
			if(sender is BaseEdit) {
				return (BaseEdit)sender;
			}
			if(sender is RepositoryItem) {
				return ((RepositoryItem)sender).OwnerEdit;
			}
			return null;
		}
		private void SubscribeToDataControllerEventsCore() {
			if(view.DataController != null) {
				view.DataController.BeforeListChanged += new ListChangedEventHandler(DataController_BeforeListChanged);
				view.DataController.ListChanged += new ListChangedEventHandler(DataController_ListChanged);
				SubscribeToDataControllerEvents();
			}
		}
		private void UnsubscribeFromDataControllerEventsCore() {
			if(view.DataController != null) {
				view.DataController.BeforeListChanged -= new ListChangedEventHandler(DataController_BeforeListChanged);
				view.DataController.ListChanged -= new ListChangedEventHandler(DataController_ListChanged);
				UnsubscribeFromDataControllerEvents();
			}
		}
		protected virtual void SubscribeToDataControllerEvents() {
		}
		protected virtual void UnsubscribeFromDataControllerEvents() { }
		protected virtual void SubscribeGridViewEvents() {
			view.BeforeLeaveRow += new RowAllowEventHandler(view_BeforeLeaveRow);
			view.FocusedRowObjectChanged += new FocusedRowObjectChangedEventHandler(view_FocusedRowObjectChanged);
			view.ColumnFilterChanged += new EventHandler(view_ColumnFilterChanged);
			view.SelectionChanged += new SelectionChangedEventHandler(view_SelectionChanged);
			view.FocusedRowLoaded += new RowEventHandler(view_FocusedRowLoaded);
			if(AllowEdit) {
				view.ValidateRow += new ValidateRowEventHandler(view_ValidateRow);
			}
			SubscribeToDataControllerEventsCore();
		}
		protected virtual void UnsubscribeGridViewEvents() {
			view.FocusedRowObjectChanged -= new FocusedRowObjectChangedEventHandler(view_FocusedRowObjectChanged);
			view.ColumnFilterChanged -= new EventHandler(view_ColumnFilterChanged);
			view.SelectionChanged -= new SelectionChangedEventHandler(view_SelectionChanged);
			view.ValidateRow -= new ValidateRowEventHandler(view_ValidateRow);
			view.BeforeLeaveRow -= new RowAllowEventHandler(view_BeforeLeaveRow);
			view.FocusedRowLoaded -= new RowEventHandler(view_FocusedRowLoaded);
			UnsubscribeFromDataControllerEventsCore();
		}
		private void SetGridViewOptionsCore() {
			view.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDownFocused;
			view.OptionsBehavior.Editable = true;
			view.OptionsBehavior.AutoPopulateColumns = false;
			view.OptionsBehavior.FocusLeaveOnTab = true;
			view.OptionsBehavior.CacheValuesOnRowUpdating = CacheRowValuesMode.Disabled;
			view.OptionsSelection.MultiSelect = true;
			view.OptionsFilter.DefaultFilterEditorView = FilterEditorViewMode.VisualAndText;
			view.OptionsFilter.FilterEditorAggregateEditing = FilterControlAllowAggregateEditing.AggregateWithCondition;
			view.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
			view.OptionsView.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
			ApplyHtmlFormatting(htmlFormattingEnabled);
			SetGridViewOptions();
		}
		protected virtual void SetGridViewOptions() {
		}
		protected virtual void SetupGridView() {
		}
		private void SetupGridViewCore() {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(view, "view");
			SubscribeGridViewEvents();
			SetupGridView();
		}
		private ColumnView CreateGridView() {
			view = CreateGridViewCore();
			if(!(view is IModelSynchronizersHolder)) {
				throw new InvalidOperationException(InvalidGridViewType);
			}
			return view;
		}
		private void SaveFocusedRowHandle(int focusedRowHandle) {
			prevFocusedRowHandle = focusedRowHandle;
		}
		private void RestoreFocusedRow() {
			int dataRowCount = view.DataRowCount;
			if(prevFocusedRowHandle >= dataRowCount) {
				SelectRowByHandle(dataRowCount - 1);
			}
			else {
				SelectRowByHandle(prevFocusedRowHandle);
			}
		}
		private void SelectRowByHandle(int rowHandle) {
			if(view.IsValidRowHandle(rowHandle)) {
				XtraGridUtils.SelectRowByHandle(view, rowHandle);
			}
		}
		private bool IsGroupRowHandle(int handle) {
			return handle < 0;
		}
		private void grid_HandleCreated(object sender, EventArgs e) {
		}
		private void grid_KeyDown(object sender, KeyEventArgs e) {
			ProcessGridKeyDown(e);
		}
		private void view_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			isForceSelectRow = e.Action == CollectionChangeAction.Add;
			if(!isGridDataSourceChanging) {
				OnSelectionChanged();
			}
		}
		private void view_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e) {
			if(ColumnView.DataController.IsUpdateLocked) {
				return;
			}
			if(!ColumnView.OptionsSelection.MultiSelect) {
				OnSelectionChanged(); 
			}
			OnFocusedObjectChanged();
		}
		private void view_FocusedRowLoaded(object sender, RowEventArgs e) {
			if(IsAsyncServerMode) {
				OnFocusedObjectChanged();
				OnSelectionChanged(); 
			}
		}
		private void view_ColumnFilterChanged(object sender, EventArgs e) {
			if(!ColumnView.IsLoading) {
				OnFocusedObjectChanged();
			}
		}
		private void view_ValidateRow(object sender, ValidateRowEventArgs e) {
			if(e.Valid) {
				ValidateObjectEventArgs ea = new ValidateObjectEventArgs(FocusedObject, true);
				OnValidateObject(ea);
				e.Valid = ea.Valid;
				e.ErrorText = ea.ErrorText;
			}
		}
		private void view_BeforeLeaveRow(object sender, RowAllowEventArgs e) {
			if(e.Allow) {
				if(view is ISupportNewItemRow) {
					if(!((ISupportNewItemRow)view).IsNewItemRowCancelling) {
						e.Allow = OnFocusedObjectChanging();
					}
				}
				else {
					e.Allow = OnFocusedObjectChanging();
				}
			}
		}
		private void DataController_BeforeListChanged(object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.ItemChanged || (e.ListChangedType == ListChangedType.ItemDeleted && prevFocusedRowHandle != 0)) {
				SaveFocusedRowHandle(view.FocusedRowHandle);
			}
			if(e.ListChangedType == ListChangedType.Reset && view.IsServerMode && RestoreSelectedRowByHandle) {
				SaveFocusedRowHandle(view.FocusedRowHandle);
			}
		}
		private void DataController_ListChanged(object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.ItemAdded) {
				if((grid != null) && (grid.FindForm() != null) && !grid.ContainsFocus) {
					IList dataSource = ListHelper.GetList(((BaseGridController)sender).DataSource);
					if(dataSource != null && dataSource.Count == 1) {
						IEditableObject obj = dataSource[e.NewIndex] as IEditableObject;
						if(obj != null) {
							obj.EndEdit();
						}
					}
				}
			}
			if(e.ListChangedType == ListChangedType.ItemDeleted && view.FocusedRowHandle != BaseListSourceDataController.NewItemRow) {
				RestoreFocusedRow();
				OnFocusedObjectChanged();
			}
			if(e.ListChangedType == ListChangedType.Reset) {
				if(view.IsServerMode && RestoreSelectedRowByHandle) {
					RestoreFocusedRow();
				}
				if(view.SelectedRowsCount == 0) {
					XtraGridUtils.SelectFocusedRow(view);
				}
				OnFocusedObjectChanged(); 
			}
		}
		private void SetTag() {
			if(grid != null) {
				grid.Tag = EasyTestTagHelper.FormatTestTable(Name);
			}
		}
		private void repositoryItem_EditValueChanged(object sender, EventArgs e) {
			BaseEdit editor = GetEditor(sender);
			if(editor != null && editor.InplaceType == InplaceType.Grid && editor.IsModified) {
				OnObjectChanged();
			}
		}
		private void grid_VisibleChanged(object sender, EventArgs e) {
			if(grid.Visible) {
				GridColumn defaultColumn = GetDefaultColumn();
				if(defaultColumn != null)
					view.FocusedColumn = defaultColumn;
			}
		}
		private void grid_ParentChanged(object sender, EventArgs e) {
			if(grid.Parent != null) {
				grid.ForceInitialize();
			}
		}
		protected GridColumn GetDefaultColumn() {
			GridColumn result = null;
			if(Model != null) {
				ITypeInfo classType = Model.ModelClass.TypeInfo;
				if(classType != null) {
					IMemberInfo defaultMember = classType.DefaultMember;
					if(defaultMember != null) {
						result = ColumnView.Columns[defaultMember.Name];
					}
				}
			}
			return result == null || !result.Visible ? null : result;
		}
		private void UpdateAllowEditGridViewAndColumnsOptionsCore() {
			if(view != null) {
				try {
					view.BeginUpdate();
					foreach(GridColumn column in view.Columns) {
						column.OptionsColumn.AllowEdit = column.ColumnEdit != null && GridViewColumnFactory.IsDataShownOnDropDownWindow(column.ColumnEdit) ? true : AllowEdit;
						if(column.ColumnEdit != null) {
							column.ColumnEdit.ReadOnly = !AllowEdit || ReadOnlyEditors;
							IGridColumnModelSynchronizer info = GetColumnModelSynchronizer(column);
							if(info != null) {
								column.ColumnEdit.ReadOnly |= !info.Model.AllowEdit;
							}
						}
					}
					if(AllowEdit) {
						view.ValidateRow -= new ValidateRowEventHandler(view_ValidateRow);
						view.ValidateRow += new ValidateRowEventHandler(view_ValidateRow);
					}
					else {
						view.ValidateRow -= new ValidateRowEventHandler(view_ValidateRow);
					}
					UpdateAllowEditGridViewAndColumnsOptions();
				}
				finally {
					view.EndUpdate();
				}
			}
		}
		protected virtual void UpdateAllowEditGridViewAndColumnsOptions() {
		}
		private void columnFactory_CustomizeGridColumn(object sender, CustomizeGridColumnEventArgs e) {
			OnCustomizeGridColumn(this, e);
		}
		private void columnFactory_CreateCustomColumn(object sender, CreateCustomColumnEventArgs e) {
			OnCreateCustomColumn(this, e);
		}
		protected virtual void OnCustomizeGridColumn(object sender, CustomizeGridColumnEventArgs e) {
			if(CustomizeGridColumn != null) {
				CustomizeGridColumn(this, e);
			}
		}
		protected virtual void OnCreateCustomColumn(object sender, CreateCustomColumnEventArgs e) {
			if(CreateCustomColumn != null) {
				CreateCustomColumn(this, e);
			}
		}
		protected virtual void OnCreateCustomGridBand(object sender, CreateCustomGridBandEventArgs e) {
			if(CreateCustomGridBand != null) {
				CreateCustomGridBand(this, e);
			}
		}
		private void gridViewCollumnFactory_CreateCustomRepositoryItem(object sender, CreateCustomRepositoryItemEventArgs e) {
			if(CreateCustomRepositoryItem != null) {
				CreateCustomRepositoryItem(this, e);
			}
		}
		private void gridViewCollumnFactory_CreateCustomColumnCore(object sender, CreateCustomColumnEventArgs e) {
			if(HasProtectedContent(e.ModelColumn.PropertyName)) {
				e.GridColumnInfo = CreateGridColumnModelSynchronizer(e.ModelColumn, e.ObjectTypeInfo, IsAsyncServerMode, true);
				if(CanShowBands) {
					e.Column = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
				}
				else {
					e.Column = new GridColumn();
				}
			}
		}
		private void gridViewCollumnFactory_CustomizeRepositoryItem(object sender, CustomizeRepositoryItemEventArgs e) {
			e.Item.EditValueChanged += new EventHandler(repositoryItem_EditValueChanged);
			if(CustomizeRepositoryItem != null) {
				CustomizeRepositoryItem(this, e);
			}
		}
		#region IDXPopupMenuHolder Members
		public Control PopupSite {
			get { return Grid; }
		}
		public void SetMenuManager(IDXMenuManager manager) {
			if(grid != null) {
				grid.MenuManager = manager;
			}
		}
		public abstract bool CanShowPopupMenu(Point position);
		#endregion
		#region IControlOrderProvider Members
		public int GetIndexByObject(Object obj) {
			int index = -1;
			if((DataSource != null) && (view != null)) {
				int dataSourceIndex = List.IndexOf(obj);
				index = view.GetRowHandle(dataSourceIndex);
				if(index == GridControl.InvalidRowHandle) {
					index = -1;
				}
			}
			return index;
		}
		public Object GetObjectByIndex(int index) {
			if((view != null) && (view.DataController != null)) {
				return view.GetRow(index);
			}
			return null;
		}
		public IList GetOrderedObjects() {
			List<Object> list = new List<Object>();
			if(view != null) {
				if(view.IsServerMode) {
					Int32 focusedRowVisibleIndex = view.GetVisibleIndex(view.FocusedRowHandle);
					Int32 topRowIndex = focusedRowVisibleIndex - PageRowCountForServerMode / 2;
					if(topRowIndex < 0) {
						topRowIndex = 0;
					}
					Int32 bottomRowIndex = topRowIndex + PageRowCountForServerMode - 1;
					if(bottomRowIndex > view.RowCount - 1) {
						bottomRowIndex = view.RowCount - 1;
					}
					for(Int32 i = topRowIndex; i <= bottomRowIndex; i++) {
						Int32 rowHandle = view.GetVisibleRowHandle(i);
						if(view.IsDataRow(rowHandle)) {
							if(view.IsRowLoaded(rowHandle)) {
								object obj = view.GetRow(rowHandle);
								if(obj != null) {
									list.Add(obj);
								}
							}
						}
					}
				}
				else {
					for(Int32 i = 0; i < view.DataRowCount; i++) {
						Int32 rowHandle = i;
						if(view.IsRowLoaded(rowHandle)) {
							object obj = view.GetRow(rowHandle);
							if(obj != null) {
								list.Add(obj);
							}
						}
					}
				}
			}
			return list;
		}
		#endregion
		#region IComplexListEditor Members
		public virtual void Setup(CollectionSourceBase collectionSource, XafApplication application) {
			this.collectionSource = collectionSource;
			this.application = application;
			repositoryFactory = new RepositoryEditorsFactory(application, collectionSource.ObjectSpace);
		}
		#endregion
		protected internal CollectionSourceBase CollectionSource {
			get { return collectionSource; }
		}
		#region ISupportAppearanceCustomization Members
		public event EventHandler<CustomizeAppearanceEventArgs> CustomizeAppearance;
		#endregion
		#region ISupportAppearanceCustomization2 Members
		public event EventHandler<CustomizeEnabledEventArgs> CustomizeEnabled;
		#endregion
		#region IHtmlFormattingSupport Members
		public void SetHtmlFormattingEnabled(bool htmlFormattingEnabled) {
			this.htmlFormattingEnabled = htmlFormattingEnabled;
			if(view != null) {
				ApplyHtmlFormatting(htmlFormattingEnabled);
			}
		}
		protected virtual void ApplyHtmlFormatting(bool htmlFormattingEnabled) { }
		#endregion
		#region IFocusedElementCaptionProvider Members
		object IFocusedElementCaptionProvider.FocusedElementCaption {
			get {
				if(ColumnView != null) {
					return ColumnView.GetFocusedDisplayText();
				}
				return null;
			}
		}
		#endregion
		#region IDataAwareExporter Members
		void IDataAwareExportableCsv.Export(System.IO.Stream stream, CsvExportOptionsEx options) {
			if(Grid == null) {
				throw new InvalidOperationException("'Grid' is null");
			}
			Grid.ExportToCsv(stream, options);
		}
		void IDataAwareExportableXls.Export(System.IO.Stream stream, XlsExportOptionsEx options) {
			if(Grid == null) {
				throw new InvalidOperationException("'Grid' is null");
			}
			Grid.ExportToXls(stream, options);
		}
		void IDataAwareExportableXlsx.Export(System.IO.Stream stream, XlsxExportOptionsEx options) {
			if(Grid == null) {
				throw new InvalidOperationException("'Grid' is null");
			}
			Grid.ExportToXlsx(stream, options);
		}
		#endregion
		public event EventHandler<EventArgs> ControlDataSourceChanged;
		public event EventHandler GridDataSourceChanging;
		public event EventHandler<ColumnCreatedEventArgs> ColumnCreated;
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
		public event EventHandler<CreateCustomColumnEventArgs> CreateCustomColumn;
		public event EventHandler<CreateCustomGridBandEventArgs> CreateCustomGridBand;
		public event EventHandler<CustomizeGridColumnEventArgs> CustomizeGridColumn;
		public event EventHandler<CustomizeRepositoryItemEventArgs> CustomizeRepositoryItem;
		public event EventHandler<CreateCustomRepositoryItemEventArgs> CreateCustomRepositoryItem;
#if DebugTest
		public CollectionSourceBase DebugTest_CollectionSource {
			get { return CollectionSource; }
		}
		public void DebugTest_SetView(ColumnView view) {
			this.view = view;
		}
#endif
		#region IGridEditorOptions Members
		bool IGridViewOptions.IsAsyncServerMode {
			get { return IsAsyncServerMode; }
		}
		ITypeInfo IGridViewOptions.ObjectTypeInfo {
			get { return ObjectTypeInfo; }
		}
		#endregion
		[Obsolete("Use the 'CanManageActiveFilterCriteriaPropertyValue' property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)] 
		public bool CanManageFilterPropertyValue {
			get { return CanManageActiveFilterCriteriaPropertyValue; }
			set { CanManageActiveFilterCriteriaPropertyValue = value; }
		}
	}
}

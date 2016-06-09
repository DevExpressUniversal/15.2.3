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
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data.Linq;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.GridData;
#if !SL
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using DependencyPropertyChangedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventHandler;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
#endif
namespace DevExpress.Xpf.Grid.Native {
	public abstract class DataViewBehavior {
		readonly DataViewBase view;
		protected DataViewBehavior(DataViewBase view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public DataViewBase View { get { return view; } }
		protected internal DataControlBase DataControl { get { return View.DataControl; } }
		internal virtual bool IsNavigationLocked { get { return false; } }
		internal virtual bool IsAutoFilterRowFocused { get { return false; } }
		internal virtual bool CanShowFixedColumnMenu { get { return false; } }
		internal virtual bool AllowResizingCore { get { return false; } }
		internal virtual bool UpdateAllowResizingOnWidthChanging { get { return true; } }
		internal virtual bool AutoWidthCore { get { return false; } }
		internal virtual bool AllowColumnResizingCore { get { return false; } }
		internal abstract double HorizontalViewportCore { get; }
		protected internal virtual bool IsAdditionalRow(int rowHandle) { return IsAdditionalRowCore(rowHandle); }
		protected internal virtual bool IsAdditionalRowData(RowData rowData) { return IsAdditionalRow(rowData.RowHandle.Value); }
		internal bool IsAdditionalRowCore(int rowHandle) { return rowHandle == DataControlBase.AutoFilterRowHandle || rowHandle == DataControlBase.NewItemRowHandle; }
		internal virtual bool IsNewItemRowVisible { get { return false; } }
		protected internal virtual bool IsNewItemRowEditing { get { return false; } }
		protected internal virtual FrameworkElement GetAdditionalRowElement(int rowHandle) { return null; }
		internal virtual bool AutoMoveRowFocusCore { get { return true; } }
		protected internal virtual Style AutoFilterRowCellStyle { get { return null; } }
		protected internal virtual DispatcherTimer ScrollTimer { get { return null; } }
		protected internal virtual bool AllowCascadeUpdate { get { return false; } }
		protected internal virtual double ScrollAnimationDuration { get { return 0d; } }
		protected internal virtual bool AllowScrollAnimation { get { return false; } }
		protected internal virtual ScrollAnimationMode ScrollAnimationMode { get { return ScrollAnimationMode.EaseOut; } }
		protected internal virtual bool AllowPerPixelScrolling { get { return false; } }
		protected internal virtual void StopSelection() { }
		internal virtual HorizontalNavigationStrategyBase NavigationStrategyBase {
			get { return HorizontalNavigationStrategyBase.NormalHorizontalNavigationStrategyBaseInstance; }
		}
		public virtual void ChangeHorizontalOffsetBy(double delta) {
			View.DataPresenter.SetHorizontalOffsetForce(View.DataPresenter.ScrollInfoCore.HorizontalScrollInfo.Offset + delta);
		}
		public virtual void ChangeVerticalOffsetBy(double delta) {
			View.DataPresenter.SetVerticalOffsetForce(View.DataPresenter.ScrollInfoCore.VerticalScrollInfo.Offset + delta);
		}
		internal virtual bool CheckNavigationStyle(int newValue) {
			return View.NavigationStyle == GridViewNavigationStyle.None;
		}
		internal virtual bool CanAdjustScrollbarCore() {
			return false;
		}
		internal virtual void OnColumnResizerDoubleClick(ColumnBase column) {
		}
		internal virtual bool CanBestFitColumnCore(ColumnBase column) {
			return false;
		}
		protected internal virtual IndicatorState GetIndicatorState(RowData rowData) {
			return IndicatorState.None;
		}
		protected internal virtual RowData CreateRowDataCore(DataTreeBuilder treeBuilder, bool updateOnlyData) {
			return new RowData(treeBuilder, updateOnlyData);
		}
		protected internal virtual KeyValuePair<DataViewBase, int> GetViewAndVisibleIndex(double verticalOffset) {
			return new KeyValuePair<DataViewBase,int>(View, 0);
		}
		protected internal virtual Size CorrectMeasureResult(double scrollOffset, Size constraint, Size result) {
			return result;
		}
		protected internal virtual void UpdateCellData() {
			View.HeadersData.UpdateCellData();
			UpdateRowData(rowData => rowData.UpdateCellData());
			UpdateServiceRowData(rowData => rowData.UpdateCellData());
		}
		bool isUpdateScrollInfoNeeded = true;
		bool lockTopRowIndexUpdate;
		internal void OnTopRowIndexChanged() {
			if(View.DataPresenter == null) {
				lockTopRowIndexUpdate = true;
				return;
			}
			if(View.TopRowIndex == ((IScrollInfoOwner)View.DataPresenter).Offset || !isUpdateScrollInfoNeeded) return;
			lockTopRowIndexUpdate = false;
			OnTopRowIndexChangedCore();
		}
		protected internal virtual void OnTopRowIndexChangedCore() { }
		internal void UpdateTopRowIndex() {
			if(lockTopRowIndexUpdate) {
				View.Dispatcher.BeginInvoke(new Action(() => OnTopRowIndexChanged()));
				return;
			}
			isUpdateScrollInfoNeeded = false;
			View.TopRowIndex = view.DataProviderBase != null ? View.DataProviderBase.ConvertScrollIndexToVisibleIndex(View.ScrollInfoOwner.Offset, View.AllowFixedGroupsCore) : 0;
			isUpdateScrollInfoNeeded = true;
		}
		protected internal virtual void UpdateLastPostition(DevExpress.Xpf.Core.IndependentMouseEventArgs e) { }
		protected internal virtual int? VisibleComparisonCore(BaseColumn x, BaseColumn y) {
			return null;
		}
		protected internal virtual void OnShowEditor(CellEditorBase editor) {
		}
		protected internal virtual void OnEditorActivated() {
		}
		protected internal virtual void OnHideEditor(CellEditorBase editor) {
		}
		protected internal virtual void OnCancelRowEdit() {
			View.IsFocusedRowModified = false;
		}
		protected internal virtual void OnFocusedRowCellModified() {
			View.IsFocusedRowModified = true;
			if(View.EnableImmediatePosting) {
				View.PostEditor();
				if(View.ValidationError == null && View.CurrentCellEditor != null)
					View.CurrentCellEditor.Edit.IsValueChanged = false;
			}
		}
		protected internal virtual bool EndRowEdit() {
			View.IsFocusedRowModified = false;
			if(DataControl == null) return false;
			bool wasEditing = View.DataProviderBase.IsCurrentRowEditing;
			bool result = false;
			DataControl.UpdateFocusedRowDataposponedAction.PerformLockedAction(delegate {
				result = View.DataProviderBase.EndCurrentRowEdit();
			});
			if(DataControl.DataProviderBase.IsICollectionView && wasEditing) {
				ICollectionViewHelper helper = DataControl.DataProviderBase.DataController.DataSource as ICollectionViewHelper;
				if (helper != null)
					DataControl.UpdateTotalSummary();
			}
			return result;
		}
		protected internal virtual bool GetIsCellSelected(int rowHandle, ColumnBase column) {
			return false;
		}
		internal virtual void UpdateColumnsPosition(ObservableCollection<ColumnBase> visibleColumnsList) {
			for(int i = 0; i < visibleColumnsList.Count; i++) {
				ColumnBase col = visibleColumnsList[i];
				ColumnBase.SetVisibleIndex(col, i);
				AssignColumnPosition(col, i, visibleColumnsList);
			}
		}
		internal virtual void AssignColumnPosition(ColumnBase column, int index, IList<ColumnBase> visibleColumns) {
		}
		internal ObservableCollection<ColumnBase> RebuildVisibleColumns() {
			IList<ColumnBase> slDTWorkaround = RebuildVisibleColumnsCore();
			return new ObservableCollection<ColumnBase>(slDTWorkaround);
		}
		internal protected virtual IList<ColumnBase> RebuildVisibleColumnsCore() {
			List<ColumnBase> visibleColumnsList = new List<ColumnBase>();
			bool hasFixedLeftColumns = false;
			for(int i = 0; i < View.ColumnsCore.Count; i++) {
				ColumnBase col = View.ColumnsCore[i];
				col.index = i;
				if(col.Visible && View.IsColumnVisibleInHeaders(col)) {
					visibleColumnsList.Add(col);
					hasFixedLeftColumns |= col.Fixed == FixedStyle.Left;
				}
			}
			View.PatchVisibleColumns(visibleColumnsList, hasFixedLeftColumns);
			visibleColumnsList.Sort(View.VisibleComparison);
			return visibleColumnsList;
		}
		internal virtual bool IsRowIndicator(DependencyObject originalSource) {
			return false;
		}
		internal virtual int GetValueForSelectionAnchorRowHandle(int value) {
			if(value == DataControlBase.InvalidRowHandle) {
				return DataControl.GetRowHandleByVisibleIndexCore(0);
			}
			return value;
		}
		internal virtual bool CanBestFitAllColumns() {
			return false;
		}
		internal virtual void OnCellContentPresenterRowChanged(FrameworkElement presenter) {
		}
		internal virtual void UpdateSecondaryScrollInfoCore(double secondaryOffset, bool allowUpdateViewportVisibleColumns) { 
		}
		protected internal virtual void UpdateAdditionalRowsData() {
		}
		protected internal virtual void UpdateColumnsViewInfo(bool updateDataPropertiesOnly) {
			View.UpdateColumns((column) => column.UpdateViewInfo(updateDataPropertiesOnly));
		}
		protected virtual internal DependencyObject GetRowState(int rowHandle) {
			return null;
		}
		internal virtual void ProcessPreviewKeyDown(KeyEventArgs e) {
			DataViewBase focusedView = View.FocusedView;
			if(focusedView.SelectionStrategy.ShouldInvertSelectionOnPreviewKeyDown(e)) {
				focusedView.SelectionStrategy.OnInvertSelection();
				e.Handled = true;
			}
			if(((ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)) && (e.Key == Key.V)) ||
				(ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e)) && (e.Key == Key.Insert))) &&
				!ModifierKeysHelper.IsAltPressed(ModifierKeysHelper.GetKeyboardModifiers(e)) && !View.IsKeyboardFocusInSearchPanel()) {
				bool handled = DataControl.RaisePastingFromClipboard(); 
				if(!handled)
					handled = View.RaisePastingFromClipboard();
				e.Handled = handled;
			}
		}
		protected internal virtual void OnGotKeyboardFocus() {
		}
		protected internal virtual void UpdateRowData(UpdateRowDataDelegate updateMethod, bool updateInvisibleRows = true, bool updateFocusedRow = true) {
			RowDataItemsEnumerator en = new RowDataItemsEnumerator(View.RootRowsContainer);
			while(en.MoveNext()) {
				if(updateInvisibleRows || en.Current.GetIsVisible()) updateMethod(en.Current);
			}
			if(!updateFocusedRow) return; 
			RowData focusedRowData = View.FocusedRowData;
			if(focusedRowData != null)
				updateMethod(focusedRowData);
		}
		protected internal void UpdateViewRowData(UpdateRowDataDelegate updateMethod) {
			UpdateRowData(updateMethod, true, false);
		}
		protected internal virtual void UpdateServiceRowData(Action<ColumnsRowDataBase> updateMethod) {
		}
		internal virtual GridViewNavigationBase CreateAdditionalRowNavigation() {
			return new DummyNavigation(View);
		}
		protected internal virtual double CalcColumnMaxWidth(ColumnBase column) {
			return 0;
		}
		protected internal virtual void ResetHeadersChildrenCache() {
		}
		internal virtual void OnResizingComplete() {
		}
		protected internal virtual void ApplyResize(BaseColumn column, double value, double maxWidth) {
		}
		protected internal virtual Point CorrectDragIndicatorLocation(UIElement panel, Point point) {
			return point;
		}
		protected internal virtual double CalcVerticalDragIndicatorSize(UIElement panel, Point point, double width) {
			return width;
		}
		internal virtual void UpdateViewportVisibleColumns() { }
		protected internal virtual RowData GetRowData(int rowHandle) {
			RowData rowData;
			if(View.VisualDataTreeBuilder.Rows.TryGetValue(rowHandle, out rowData)) {
				if(rowData.View != View || rowData.RowHandle.Value != rowHandle) {
#if DEBUGTEST
					throw new InvalidOperationException();
#else
					return null;
#endif
				}
				return rowData;
			}
			return null;
		}
		internal virtual void UpdateAdditionalFocusedRowData() { }
		internal virtual void UpdateColumnsLayout() { }
		internal virtual void MakeCellVisible() { }
		internal virtual void MovePrevRow() {
			MovePrevRowCore();
		}
		protected bool MovePrevRowCore() {
			if(View.IsAdditionalRowFocused) return false;
			if(DataControl.NavigateToPreviousInnerDetailRow()) return true;
			int current = FocusedRowHandle == DataControlBase.InvalidRowHandle ? 1 : CurrentIndex;
			if(IsFirst(current)) return false;
			int prev = current - 1;
			View.SetFocusedRowHandle(IndexToHandle(prev));
			return true;
		}
		internal virtual void MoveNextRow() {
			MoveNextRowCore();
		}
		protected bool MoveNextRowCore() {
			if(View.IsAdditionalRowFocused) return false;
			if(DataControl.NavigateToFirstChildDetailRow()) return true;
			int current = FocusedRowHandle == DataControlBase.InvalidRowHandle ? -1 : CurrentIndex;
			if(IsLast(current)) return false;
			int next = current + 1;
			View.SetFocusedRowHandle(IndexToHandle(next));
			return true;
		}
		int IndexToHandle(int index) {
			return DataControl.GetRowHandleByVisibleIndexCore(index);
		}
		int FocusedRowHandle {
			get { return DataControl.viewCore.FocusedRowHandle; }
		}
		int CurrentIndex {
			get { return DataControl.DataProviderBase.GetRowVisibleIndexByHandle(FocusedRowHandle); }
		}
		bool IsFirst(int index) {
			return DataControl.IsFirst(index);
		}
		bool IsLast(int index) {
			return DataControl.IsLast(index);
		}
		internal virtual void OnViewMouseLeave() {
		}
		internal virtual void OnViewMouseMove(MouseEventArgs e) {
		}
		internal virtual void ProcessMouseLeftButtonUp(MouseButtonEventArgs e) {
			View.InplaceEditorOwner.ProcessMouseLeftButtonUp(e);
			View.SelectionStrategy.OnMouseLeftButtonUp(e);
		}
		internal virtual void OnMouseLeftButtonUp() { 
		}
		internal virtual void OnAfterMouseLeftButtonDown(IDataViewHitInfo hitInfo) {
		}
		internal virtual void EnsureSurroundingsActualSize(Size finalSize) {
		}
		internal virtual void SetVerticalScrollBarWidth(double width) {
		}
		internal abstract GridViewNavigationBase CreateRowNavigation();
		internal abstract GridViewNavigationBase CreateCellNavigation();
		protected internal abstract bool OnVisibleColumnsAssigned(bool changed);
		protected internal abstract void UpdateCellData(ColumnsRowDataBase rowData);
		protected internal abstract void GetDataRowText(StringBuilder sb, int rowHandle);
		protected internal abstract double GetFixedExtent();
		protected internal virtual void UpdateFixedNoneContentWidth(ColumnsRowDataBase rowData) { }
		protected internal virtual double GetFixedNoneContentWidth(double totalWidth, int rowHandle) { return totalWidth; }
		internal virtual void OnDoubleClick(MouseButtonEventArgs e) {
			if(!e.Handled)
				View.SelectionStrategy.OnDoubleClick(e);
		}
		protected internal virtual double FirstColumnIndent { get { return 0; } }
		protected internal virtual double NewItemRowIndent { get { return 0; } }
		#region master detail
		internal virtual MasterDetailProviderBase CreateMasterDetailProvider() {
			return NullDetailProvider.Instance;
		}
		internal virtual void SetActualShowIndicator(bool showIndicator) { }
		internal virtual void UpdateActualProperties() { }
		#endregion
		protected internal virtual bool IsAlternateRow(int rowHandle) { return false; }
		protected internal virtual System.Windows.Media.Brush ActualAlternateRowBackground { get { return null; } } 
		protected internal virtual bool IsFirstColumn(BaseColumn column) { return false; }
		protected internal virtual ControlTemplate GetActualColumnChooserTemplate() { return View.ColumnChooserTemplate; }
		internal void RebuildColumnChooserColumns() {
			RebuildColumnChooserColumnsCore();
		}
		protected virtual void RebuildColumnChooserColumnsCore() {
			List<ColumnBase> columnChooserColumns = new List<ColumnBase>();
			foreach(ColumnBase col in View.ColumnsCore) {
				if(!col.Visible && col.ShowInColumnChooser)
					columnChooserColumns.Add(col);
			}
			columnChooserColumns.Sort(View.ColumnChooserColumnsSortOrderComparer);
			if(!ListHelper.AreEqual(View.ColumnChooserColumns, columnChooserColumns)) {
				View.ColumnChooserColumns = new ReadOnlyCollection<ColumnBase>(columnChooserColumns);
			}
		}
		protected internal virtual void UpdateBandsLayoutProperties() {
		}
		internal virtual void NotifyBandsLayoutChanged() { }
		internal virtual void NotifyFixedNoneBandsChanged() { }
		internal virtual void NotifyFixedLeftBandsChanged() { }
		internal virtual void NotifyFixedRightBandsChanged() { }
		protected internal void IterateCells(int startRowHandle, int startColumnIndex, int endRowHandle, int endColumnIndex, Action<int, ColumnBase> action) {
			int startIndex, colStart, endIndex, colEnd;
			ValidateIndexes(startRowHandle, startColumnIndex, endRowHandle, endColumnIndex, out startIndex, out colStart, out endIndex, out colEnd);
			for(int r = startIndex; r < endIndex + 1; r++)
				for(int c = colStart; c < colEnd + 1; c++) {
					if(c < 0 || c > View.VisibleColumnsCore.Count - 1)
						continue;
					action(DataControl.GetRowHandleByVisibleIndexCore(r), View.VisibleColumnsCore[c]);
				}
		}
		protected internal void ValidateIndexes(int startRowHandle, int startColumnIndex, int endRowHandle, int endColumnIndex, out int startIndex, out int colStart, out int endIndex, out int colEnd) {
			startIndex = DataControl.GetRowVisibleIndexByHandleCore(startRowHandle);
			endIndex = DataControl.GetRowVisibleIndexByHandleCore(endRowHandle);
			colStart = startColumnIndex;
			colEnd = endColumnIndex;
			if(colStart > colEnd) {
				int a = colEnd;
				colEnd = colStart;
				colStart = a;
			}
			if(startIndex > endIndex) {
				int a = endIndex;
				endIndex = startIndex;
				startIndex = a;
			}
		}
		internal virtual Style GetActualCellStyle(ColumnBase column) {
			return column.CellStyle == null ? View.CellStyle : column.CellStyle;
		}
		internal virtual IEnumerable<IColumnInfo> GetServiceUnboundColumns() {
			yield break;
		}
		internal virtual IEnumerable<ServiceSummaryItem> GetServiceSummaries() {
			yield break;
		}
		internal virtual void CopyToDetail(DataControlBase dataControl) { }
		internal virtual bool UseRowDetailsTemplate(int rowHandle) { return false; }
		public virtual Point LastMousePosition { get; protected set; }
	}
}

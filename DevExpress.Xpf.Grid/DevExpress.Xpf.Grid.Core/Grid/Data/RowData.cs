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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using DevExpress.Data.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Grid {
	public class RowNodePrintInfo {
		public int NextNodeLevel { get; set; }
		public RowPosition RowPosition { get; set; }
		public int ListIndex { get; set; }
		public int PrevRowHandle { get; set; }
		public int NextRowHandle { get; set; }
		public RowPosition PrevRowPosition { get; set; }
		public bool IsSelected { get; set; }
		public Dictionary<ColumnBase, int> MergeValues { get; set; }
		public bool IsLast { get; set; }
	}
	public abstract class RowNode {
		protected static IEnumerable<RowNode> EmptyEnumerable = new RowNode[0];
		bool canGenerateItems;
		bool isRowVisible = true;
		public bool IsRowVisible { get { return isRowVisible; } protected set { isRowVisible = value; } }
		public abstract object MatchKey { get; }
		public NodeContainer NodesContainer { get; protected set; }
		public int ItemsCount {
			get {
				int result = 0;
				if(IsDataExpanded && NodesContainer != null)
					result += NodesContainer.ItemCount;
				if(IsRowVisible)
					result++;
				return result;
			}
		}
#if DEBUGTEST
		public
#else
		protected
#endif
		abstract bool IsDataExpanded { get; }
		public bool IsExpanding { get; internal set; }
		public bool IsExpanded { get; internal set; }
		internal bool IsCollapsing { get; set; }
		internal bool IsFinished { get { return IsDataExpanded ? !CanGenerateItems || NodesContainer.IsFinished : true; } }
		protected virtual bool CanUpdateState { get { return true; } }
		internal virtual bool CanGenerateItems {
			get { return canGenerateItems; }
			set { canGenerateItems = value; }
		}
		internal virtual bool IsFixedNode { get { return false; } }
		protected RowNode() { }
		protected internal int GenerateItems(int count) {
			return CanGenerateItems && (NodesContainer != null) ? NodesContainer.GenerateItems(count) : 0;
		}
		internal abstract RowDataBase GetRowData();
		internal abstract FrameworkElement GetRowElement();
		internal IEnumerable<RowNode> GetChildItems() {
			return (NodesContainer != null) ? NodesContainer.Items : EmptyEnumerable;
		}
		internal IEnumerable<RowNode> GetSkipCollapsedChildItems() {
			return IsExpanded ? GetChildItems() : EmptyEnumerable;
		}
		internal bool IsMatchedRowData(RowDataBase data) {
			return object.Equals(data.MatchKey, MatchKey);
		}
		internal virtual bool IsRowExpandedForNavigation() {
			return false;
		}
		internal abstract LinkedList<FreeRowDataInfo> GetFreeRowDataQueue(SynchronizationQueues synchronizationQueues);
		internal abstract RowDataBase CreateRowData();
		internal void UpdateExpandInfo(int startVisibleIndex, bool isRowVisible) {
			if(NodesContainer == null) return;
			IsRowVisible = isRowVisible;
			CanGenerateItems = !IsCollapsing && !IsExpanding && IsDataExpanded;
			if(CanUpdateState) {
				IsExpanded = IsDataExpanded;
			}
			NodesContainer.ReGenerateExpandItems(startVisibleIndex, 0);
		}
		internal virtual RowNode GetNodeToScroll() {
			if(NodesContainer == null)
				return this;
			if(IsExpanded) {
				return NodesContainer.GetNodeToScroll() ?? this;
			}
			return this;
		}
#if DEBUGTEST
		public void Print() {
			Print(0);
			System.Diagnostics.Debug.WriteLine("");
		}
		public void Print(int level) {
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < level; i++) {
				sb.Append("\t");
			}
			PrintOnlySelf(sb.ToString());
			if(NodesContainer != null)
				NodesContainer.Print(level + 1);
		}
		public void PrintOnlySelf(string prefix = "") {
			StringBuilder sb = new StringBuilder();
			sb.Append(prefix);
			sb.Append(string.Format("Type={0}, MatchKey={1}, ", GetType().Name, MatchKey));
			sb.Append(GetNodeSpecificString());
			sb.Append(string.Format("HashCode={0}, ", GetHashCode()));
			System.Diagnostics.Debug.WriteLine(sb.ToString());
		}
		protected virtual string GetNodeSpecificString() {
			return string.Empty;
		}
#endif
	}
	public class DataRowNode : RowNode {
		internal readonly DataTreeBuilder treeBuilder;
		public override object MatchKey { get { return RowHandle; } }
		public RowNodePrintInfo PrintInfo { get; set; }
		DataViewBase View { get { return (DataViewBase)treeBuilder.View; } }
		protected DataControlBase DataControl { get { return View.DataControl; } }
#if DEBUGTEST
		public
#else
		protected
#endif
		override bool IsDataExpanded { get { return DataControl.MasterDetailProvider.IsMasterRowExpanded(RowHandle.Value); } }
		DataControllerValuesContainer controllerValues;
		internal DataControllerValuesContainer ControllerValues { 
			get { 
				return controllerValues;
			} 
			private set {
				controllerValues = value;
				ValidateControllerValues();
			} 
		}
		internal override bool CanGenerateItems {
			get { return base.CanGenerateItems && CanGenerateItemsCore; }
			set { base.CanGenerateItems = value; }
		}
		protected virtual bool CanGenerateItemsCore {
			get { return treeBuilder.SupportsMasterDetail; }
		}
		public RowHandle RowHandle { get { return ControllerValues.RowHandle; } }
		public int Level { get { return ControllerValues.Level; } }
		int supressUpdateStateCount;
		protected override bool CanUpdateState { get { return supressUpdateStateCount == 0; } }
		public DataRowNode(DataTreeBuilder treeBuilder, DataControllerValuesContainer controllerValues) 
			: base() {
			this.treeBuilder = treeBuilder;
			ControllerValues = controllerValues;
		}
		internal void Update(DataControllerValuesContainer info) {
			System.Diagnostics.Debug.Assert(object.Equals(ControllerValues.RowHandle, info.RowHandle));
			ControllerValues = info;
		}
		protected virtual void ValidateControllerValues() {
			if(RowHandle.Value < 0 && RowHandle.Value != DataControlBase.NewItemRowHandle)
				throw new ArgumentException("Internal error: RowHandle should be positive");
		}
		internal void SupressUpdateState() {
			supressUpdateStateCount++;
		}
		internal void ResumeUpdateState() {
			supressUpdateStateCount--;
		}
		internal void UpdateDetailInfo(int startVisibleIndex) {
			NodesContainer = View.DataControl.MasterDetailProvider.GetDetailNodeContainer(RowHandle.Value);
			UpdateExpandInfo(startVisibleIndex, true);
		}
		internal override RowDataBase GetRowData() {
			return View.GetRowData(RowHandle.Value);
		}
		internal override FrameworkElement GetRowElement() {
			var rowData = GetRowData();
			if(rowData != null && ((ISupportVisibleIndex)rowData).VisibleIndex >= 0)
				return View.GetRowVisibleElement(rowData);
			return null;
		}
		internal override bool IsRowExpandedForNavigation() {
			return View.IsDataRowNodeExpanded(this);
		}
		internal override LinkedList<FreeRowDataInfo> GetFreeRowDataQueue(SynchronizationQueues synchronizationQueues) {
			return synchronizationQueues.FreeRowDataQueue;
		}
		internal override RowDataBase CreateRowData() {
			return View.ViewBehavior.CreateRowDataCore(treeBuilder, false);
		}
		public virtual int CurrentLevelItemCount { get { return 1; } }
#if DEBUGTEST
		protected override string GetNodeSpecificString() {
			return string.Format("RowHandle={0}, ControllerVisibleIndex={1}", RowHandle.Value, ControllerValues.VisibleIndex);
		}
#endif
	}
	public class HeadersData : ColumnsRowDataBase {
		public HeadersData(DataTreeBuilder treeBuilder)
			: base(treeBuilder) {
		}
		internal override object MatchKey { get { throw new NotImplementedException(); } }
	}
	public interface INotifyCurrentRowDataChanged {
		void OnCurrentRowDataChanged();
	}
	public interface IGridDataRow {
		RowData RowData { get; }
		void UpdateContentLayout();
	}
	public interface IRowStateClient {
		void UpdateRowHandle(RowHandle rowHandle);
		void UpdateSelectionState(SelectionState selectionState);
		void UpdateIsFocused();
		void UpdateScrollingMargin();
		void UpdateView();
		void UpdateFixedNoneCellData();
		void UpdateFixedLeftCellData(IList<GridColumnData> oldValue);
		void UpdateFixedRightCellData(IList<GridColumnData> oldValue);
		void UpdateHorizontalLineVisibility();
		void UpdateVerticalLineVisibility();
		void UpdateFixedLineWidth();
		void UpdateFixedLineVisibility();
		void UpdateFixedNoneContentWidth();
		void UpdateIndicatorWidth();
		void UpdateShowIndicator();
		void UpdateIndicatorState();
		void UpdateIndicatorContentTemplate();
		void UpdateValidationError();
		void UpdateCellsPanel();
		void InvalidateCellsPanel();
		void UpdateFixedNoneBands();
		void UpdateFixedLeftBands();
		void UpdateFixedRightBands();
		void UpdateAlternateBackground();
		void UpdateFocusWithinState();
		void UpdateLevel();
		void UpdateRowPosition();
		void UpdateDetailExpandButtonVisibility();
		void UpdateDetailViewIndents();
		void UpdateMinHeight();
		void UpdateContent();
		void UpdateRowStyle();
		void UpdateAppearance();
		void UpdateDetails();
		void UpdateIndentScrolling();
		void UpdateShowRowBreak();
		void UpdateInlineEditForm(object editFormData);
	}
	public class RowData : ColumnsRowDataBase, ISupportVisibleIndex {
		#region inner classes
		protected class RowDataReusingStrategy : NotImplementedRowDataReusingStrategy {
			readonly protected RowData rowData;
			public RowDataReusingStrategy(RowData rowData) {
				this.rowData = rowData;
			}
			internal override void AssignFrom(RowsContainer parentRowsContainer, NodeContainer parentNodeContainer, RowNode rowNode, bool forceUpdate = false) {
				rowData.AssignFrom(parentRowsContainer, parentNodeContainer, rowNode, forceUpdate);
			}
			internal override void CacheRowData() {
				rowData.CacheRowData();
			}
			internal override FrameworkElement CreateRowElement() {
				return rowData.CreateRowElement();
			}
		}
		#endregion
#if DEBUGTEST
		internal static long GetRowHandleCount;
#endif
		public static readonly DependencyProperty RowDataProperty = DependencyProperty.RegisterAttached("RowData", typeof(RowData), typeof(RowData), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		public static void SetRowData(DependencyObject element, RowData value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(RowDataProperty, value);
		}
		public static RowData GetRowData(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (RowData)element.GetValue(RowDataProperty);
		}
		internal const string DataContextPropertyName = "DataContext";
		const string RowHandlePropertyName = "RowHandle";
		public static readonly DependencyProperty CurrentRowDataProperty;
		static RowData() {
			CurrentRowDataProperty = DependencyPropertyManager.RegisterAttached("CurrentRowData", typeof(RowData), typeof(RowData), new PropertyMetadata(null, OnCurrentRowDataChanged));
			BaseEditHelper.GetValidationErrorPropertyKey().OverrideMetadata(typeof(RowData), new FrameworkPropertyMetadata((d, e) => ((RowData)d).UpdateClientValidationError()));
		}
		void IterateNotNullData(Action<ColumnBase, GridColumnData> updateMethod) {
			View.layoutUpdatedLocker.DoLockedAction(() => IterateNotNullDataCore<GridColumnData>(CellDataCache, updateMethod));
		}
		protected static void IterateNotNullDataCore<TData>(Dictionary<ColumnBase, TData> cache, Action<ColumnBase, TData> updateMethod) where TData : GridColumnData {
			IEnumerator<KeyValuePair<ColumnBase, TData>> enumerator = cache.GetEnumerator();
			do {
				try {
					if(!enumerator.MoveNext()) return;
				} catch(InvalidOperationException) {
					return;
				}
				updateMethod(enumerator.Current.Key, enumerator.Current.Value);
			} while(true);
		}
		static void OnCurrentRowDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is INotifyCurrentRowDataChanged) 
				((INotifyCurrentRowDataChanged)d).OnCurrentRowDataChanged();
		}
		public static void SetCurrentRowData(DependencyObject element, RowData value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(CurrentRowDataProperty, value);
		}
		public static RowData GetCurrentRowData(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (RowData)element.GetValue(CurrentRowDataProperty);
		}
		static void OnRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RowData)d).OnRowChanged(e.OldValue, e.NewValue);
		}
		static void OnIsReadyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RowData)d).OnIsReadyChanged();
		}
		internal static void ReassignCurrentRowData(DependencyObject sourceElement, DependencyObject targetElement) {
			if(targetElement != null)
				RowData.SetCurrentRowData(targetElement, RowData.GetCurrentRowData(sourceElement));
		}
		internal static void SetRowHandleBinding(FrameworkElement element) {
			element.SetBinding(DataViewBase.RowHandleProperty, new Binding(RowHandlePropertyName));
		}
		internal static RowData FindRowData(DependencyObject d) {
			return DataControlBase.FindElementWithAttachedPropertyValue<RowData>(d, RowData.CurrentRowDataProperty);
		}
		internal static void SetRowDataInternal(DependencyObject element, RowData value) {
			SetRowData(element, value);
			SetCurrentRowData(element, value);
		}
		internal RowHandle RowHandleCore { get; private set; }
		RowHandle rowHandle;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataRowHandle")]
#endif
		public RowHandle RowHandle {
			get {
#if DEBUGTEST
				GetRowHandleCount++;
#endif
				return rowHandle;
			}
			internal set {
				if(rowHandle != value) {
					rowHandle = value;
					RaisePropertyChanged("RowHandle");
					OnRowHandleChanged(rowHandle);
				} 
			}
		}
		object dataContext;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataDataContext")]
#endif
		public object DataContext {
			get { return dataContext; }
			set {
				if(dataContext != value) {
					dataContext = value;
					UpdateClientAppearance();
					OnDataContextChanged();
					RaisePropertyChanged(DataContextPropertyName);
				}
			}
		}
		object row;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataRow")]
#endif
		public object Row {
			get { return row; }
			set {
				if(row != value) {
					object oldValue = row;
					OnRowChanging(row, value);
					row = value;
					RaisePropertyChanged("Row");
					OnRowChanged(oldValue, row);
				}
			}
		}
		DependencyObject rowState;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataRowState")]
#endif
		public DependencyObject RowState {
			get {
				if(rowState == null)
					rowState = DataControl.GetRowState(RowHandle.Value, true);
				return rowState; 
			}
			private set {
				if(rowState != value) {
					rowState = value;
					RaisePropertyChanged("RowState");
				}
			}
		}
#if DEBUGTEST
		internal bool IsRowStateAssignedDebugTest { get { return rowState != null; } }
#endif
		RowPosition rowPosition = RowPosition.Bottom;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataRowPosition")]
#endif
		public RowPosition RowPosition {
			get { return rowPosition; }
			private set {
				if(rowPosition != value) {
					rowPosition = value;
					RaisePropertyChanged("RowPosition");
					OnRowPositionChanged();
				}
			}
		}
		bool showBottomLine = false;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataShowBottomLine")]
#endif
		public bool ShowBottomLine {
			get { return showBottomLine; }
			protected set {
				if(showBottomLine != value) {
					showBottomLine = value;
					RaisePropertyChanged("ShowBottomLine");
				}
			}
		}
		bool evenRow;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataEvenRow")]
#endif
		public bool EvenRow {
			get { return evenRow; }
			protected set {
				if(evenRow != value) {
					evenRow = value;
					RaisePropertyChanged("EvenRow");
				}
			}
		}
		bool alternateRow;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataAlternateRow")]
#endif
		public bool AlternateRow {
			get { return alternateRow; }
			protected set {
				if(alternateRow != value) {
					alternateRow = value;
					RaisePropertyChanged("AlternateRow");
					OnAlternateRowChanged();
				}
			}
		}
		bool isSelected;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataIsSelected")]
#endif
		public bool IsSelected {
			get { return isSelected; }
			private set {
				if(isSelected != value) {
					isSelected = value;
					RaisePropertyChanged("IsSelected");
					UpdateSelectionState();
				}
			}
		}
		bool isFocused;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataIsFocused")]
#endif
		public bool IsFocused {
			get { return isFocused; }
			private set {
				if(isFocused != value) {
					isFocused = value;
					RaisePropertyChanged("IsFocused");
					UpdateSelectionState();
					UpdateClientIsFocused();
				}
			}
		}
#if DEBUGTEST
		internal bool IsRowExpandedSet { get; private set; }
#endif
		bool isRowExpanded;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataIsRowExpanded")]
#endif
		public bool IsRowExpanded {
			get { return isRowExpanded; }
			internal set {
#if DEBUGTEST
				IsRowExpandedSet = true;
#endif
				if(isRowExpanded != value) {
					isRowExpanded = value;
					RaisePropertyChanged("IsRowExpanded");
					OnIsRowExpandedChanged();
				}
			}
		}
#if DEBUGTEST
		internal bool IsRowVisibleSet { get; private set; }
#endif
		bool isRowVisible = true;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataIsRowVisible")]
#endif
		public bool IsRowVisible {
			get { return isRowVisible; }
			internal set {
#if DEBUGTEST
				IsRowVisibleSet = true;
#endif
				if(isRowVisible != value) {
					isRowVisible = value;
					RaisePropertyChanged("IsRowVisible");
					OnIsRowVisibleChanged();
				}
			}
		}
#if DEBUGTEST
		internal bool IsExpandingSet { get; private set; }
#endif
		bool isExpanding;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataIsExpanding")]
#endif
		public bool IsExpanding {
			get { return isExpanding; }
			internal set {
#if DEBUGTEST
				IsExpandingSet = true;
#endif
				if(isExpanding != value) {
					isExpanding = value;
					RaisePropertyChanged("IsExpanding");
				}
			}
		}
		SelectionState selectionState = SelectionState.None;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataSelectionState")]
#endif
		public SelectionState SelectionState {
			get { return selectionState; }
			private set {
				if(selectionState != value) {
					selectionState = value;
					RaisePropertyChanged("SelectionState");
					UpdateClientSelectionState();
				}
			}
		}
		int nextRowLevel;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataNextRowLevel")]
#endif
		public int NextRowLevel {
			get { return nextRowLevel; }
			protected set {
				if(nextRowLevel != value) {
					nextRowLevel = value;
					RaisePropertyChanged("NextRowLevel");
				}
			}
		}
		bool showRowBreak;
		public bool ShowRowBreak {
			get { return showRowBreak; }
			private set {
				if(showRowBreak != value) {
					showRowBreak = value;
					UpdateClientShowRowBreak();
					RaisePropertyChanged("ShowRowBreak");
				}
			}
		}
		void OnRowPositionChanged() {
			ShowBottomLine = GetShowBottomLine();
			UpdateClientRowPosition();
		}
		bool isReady = true;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataIsReady")]
#endif
		public bool IsReady {
			get { return isReady; }
			private set {
				if(isReady != value) {
					isReady = value;
					OnIsReadyChanged();
					RaisePropertyChanged("IsReady");
				}
			}
		}
		protected bool IsNewItemRow { get { return RowHandle != null && RowHandle.Value == DataControlBase.NewItemRowHandle; } }
		internal bool IsInlineEditFormVisible { get { return EditFormData != null; } }
		EditForm.EditFormRowData editFormDataCore;
		internal EditForm.EditFormRowData EditFormData {
			get {
				return editFormDataCore;
			}
			set {
				if(editFormDataCore != value) {
					editFormDataCore = value;
					UpdateClientInlineEditForm();
				}
			}
		}
		internal NodeContainer parentNodeContainer;
		DataControllerValuesContainer controllerValues;
		internal DataRowNode DataRowNode { get { return (DataRowNode)node; } }
		internal virtual FrameworkElement RowElement { get { return WholeRowElement; } }
		internal VisualDataTreeBuilder VisualDataTreeBuilder { get { return (VisualDataTreeBuilder)treeBuilder; } }
		internal DataControllerValuesContainer ControllerValues {
			get {
				if (controllerValues == null) {
					controllerValues = DataIteratorBase.CreateValuesContainer(treeBuilder, RowHandle);
					UpdateLevel();
				}
				return controllerValues;
			}
			private set {
				if (value == controllerValues)
					return;
				controllerValues = value;
				UpdateLevel();
			}
		}
		internal override object MatchKey { get { return RowHandle; } }
		int level;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataLevel")]
#endif
		public override int Level { get { return level; } }
		void UpdateLevel() {
			int newLevel = controllerValues.Level;
			if(this.level != newLevel) {
				this.level = newLevel;
				NotifyPropertyChanged("Level");
				UpdateClientLevel();
				ShowBottomLine = GetShowBottomLine();
			}
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("RowDataControllerVisibleIndex")]
#endif
		public int ControllerVisibleIndex { get { return ControllerValues.VisibleIndex; } }
		protected virtual void OnControllerVisibleIndexChanged() {
			if(RowHandle == null) return;
			EvenRow = (RowHandle.Value % 2) == 0;
		}
		bool updateOnlyData;
		bool updateSelectionState;
		public RowData(DataTreeBuilder treeBuilder, bool updateOnlyData = false, bool updateSelectionState = true)
			: base(treeBuilder) {
			formatInfoProvider = new DataTreeBuilderFormatInfoProvider<RowData>(this, x => x.treeBuilder, (x, fieldName) => x.treeBuilder.GetCellValue(x, fieldName));
			this.updateOnlyData = updateOnlyData;
			this.updateSelectionState = updateSelectionState;
			SetRowDataInternal(WholeRowElement, this);
		}
		protected virtual FrameworkElement CreateRowElement() {
			return View.CreateRowElement(this);
		}
		protected internal virtual void UpdateGroupSummaryData() {
		}
		protected internal virtual void UpdateEditorButtonVisibilities() {
			IterateNotNullData((column, cellData) => ((GridCellData)cellData).UpdateEditorButtonVisibility());
		}
		protected internal virtual void UpdateEditorHighlightingText() {
			if(View == null)
				return;
			IterateNotNullData((column, cellData) => ((GridCellData)cellData).UpdateEditorHighlightingText(false));
		}
		protected internal void UpdateCellDataLanguage() {
			if(View == null)
				return;
			IterateNotNullData((column, cellData) => ((GridCellData)cellData).UpdateLanguage());
		}
		protected internal virtual bool GetShowBottomLine() {
			return Level > 0 && (RowPosition == Grid.RowPosition.Bottom || RowPosition == Grid.RowPosition.Single);
		}
		protected override bool UpdateOnlyData { get { return this.updateOnlyData; } }
		bool IsAsyncServerMode { get { return DataControl.DataProviderBase.IsAsyncServerMode; } }
		bool IsGroupRowInAsyncServerMode { get { return IsAsyncServerMode && DataControl.IsGroupRowHandleCore(RowHandle.Value); } }
		internal override bool RequireUpdateRow { get { return !IsGroupRowInAsyncServerMode; } }
#if DEBUGTEST
		internal int UpdateDataFireCount { get; set; }
#endif
		protected internal virtual void UpdateData() {
			UpdateDataCore(null, DataControlBase.InvalidRowIndex);
		}
		void UpdateDataCore(ColumnBase column, int listSourceRowIndex) {
#if DEBUGTEST
			UpdateDataFireCount++;
#endif
			UpdateRowObjects(listSourceRowIndex);
			if((IsAsyncServerMode && !GetIsReady()) || updateOnlyData) return;
			UpdateRowDataError();
			if(!IsGroupRowInAsyncServerMode) {
				if(column != null) {
					UpdateCellData(column, GetCellDataByColumn(column));
				} else {
					IterateNotNullData((col, cellData) => UpdateCellData(col, cellData));
				}
			}
			UpdateGroupSummaryData();		   
			if(ControllerValues != null) OnControllerVisibleIndexChanged();
			UpdateContent();
			RaiseContentChanged();
			treeBuilder.UpdateRowData(this);
			if(IsFocused && IsAsyncServerMode)
				DataControl.UpdateCurrentItem();
			UpdateMasterDetailInfo(false, true);
			if(!SubscribeRowChangedForVisibleRows) {
				UnsubcribePropertyChanged(Row);
			}
		}
		internal override void UpdateRow() {
			Row = treeBuilder.GetRowValue(this);
		}
		internal override void ClearRow() {
			Row = null;
		}
		void UpdateRowObjects(int listSourceRowIndex) {
			if(RowHandle.Value != DataControlBase.AutoFilterRowHandle)
				DataContext = treeBuilder.GetWpfRow(this, listSourceRowIndex);
			RowState = DataControl.GetRowState(RowHandle.Value, false);
			if (!IsGroupRowInAsyncServerMode) {
				UpdateRow();
				if(Row == null && listSourceRowIndex != DataControlBase.InvalidRowIndex)
					Row = View.DataProviderBase.GetRowByListIndex(listSourceRowIndex);
			}
		}
		protected virtual void UpdateMasterDetailInfo(bool updateRowObjectIfRowExpanded, bool updateDetailRow) {
			if(!treeBuilder.SupportsMasterDetail)
				return;
			View.DataControl.MasterDetailProvider.UpdateMasterDetailInfo(this, updateDetailRow);
			if(updateRowObjectIfRowExpanded && IsRowExpanded)
				UpdateRowObjects(DataControlBase.InvalidRowIndex);
			UpdateIsMasterRowExpanded();
		}
		protected internal virtual void EnsureRowLoaded() {
			View.DataProviderBase.EnsureRowLoaded(RowHandle.Value);
		}
		void UpdateIsMasterRowExpanded() {
			IsMasterRowExpanded = DataControl.MasterDetailProvider.IsMasterRowExpanded(RowHandle.Value);
		}
#if DEBUGTEST
		public static int UpdateIsSelectedCountDebugTest { get; private set; }
#endif
		protected internal virtual void UpdateIsSelected(bool forceIsSelected) {
#if DEBUGTEST
			UpdateIsSelectedCountDebugTest++;
#endif
			IsSelected = forceIsSelected;
		}
		protected internal virtual void UpdateIsSelected() {
			UpdateIsSelected(View.IsRowSelected(RowHandle.Value));
			IterateNotNullData((column, cellData) => ((GridCellData)cellData).UpdateIsSelected(RowHandle.Value));
		}
		void UpdateIsFocusedCore() {
			IsFocused = View.IsFocusedView && (View.FocusedRowHandle == RowHandle.Value);
		}
		protected internal void UpdateIsFocused() {
			UpdateIsFocusedCore();
			UpdateSelectionState();
			UpdateIsFocusedCell();
		}
		internal void UpdateIsFocusedCell() {
			IterateNotNullData((column, cellData) => ((GridCellData)cellData).UpdateIsFocusedCell(RowHandle.Value));
		}
		internal void UpdateSelectionState() {
			SelectionState = View.GetRowSelectionState(RowHandle.Value);
			IterateNotNullData((column, cellData) => ((GridCellData)cellData).UpdateSelectionState());
		}
		internal void UpdateIndicatorState() {
			IndicatorState = View.ViewBehavior.GetIndicatorState(this);
		}
		protected internal override void UpdateFullState() {
			UpdateIsSelected();
			UpdateIsFocusedCore();
			UpdateIndicatorState();
			UpdateIsAlternateRow();
			IterateNotNullData((column, cellData) => ((GridCellData)cellData).UpdateFullState(RowHandle.Value));
		}
		protected override void OnViewChanged() {
			base.OnViewChanged();
			if(CellDataCache != null)
				IterateNotNullData((column, cellData) => ((GridCellData)cellData).OnViewChanged());
			UpdateClientView();
		}
		protected void UpdateIsAlternateRow() {
			AlternateRow = View.ViewBehavior.IsAlternateRow(RowHandle.Value);
		}
		protected virtual void UpdateNextRowLevel() {
			if(RowHandle == null || View == null || View.DataControl == null || View.DataControl.DataProviderBase == null) {
				NextRowLevel = 0;
			} else {
				NextRowLevel = treeBuilder.GetRowLevelByVisibleIndex(treeBuilder.GetRowVisibleIndexByHandleCore(RowHandle.Value) + 1);
			}
		}
		void UpdateShowRowBreak() {
			var dataController = DataControl != null ? DataControl.DataProviderBase.DataController : null;
			if(dataController != null && dataController.AllowPartialGrouping && RowHandle != null) {
				int currentRowHandle = RowHandle.Value;
				var visibleIndexes = dataController.GetVisibleIndexes();
				if(visibleIndexes.IsSingleGroupRow(currentRowHandle)) {
					ShowRowBreak = true;
					return;
				}
				int nextRowHandle = treeBuilder.GetRowHandleByVisibleIndexCore(treeBuilder.GetRowVisibleIndexByHandleCore(currentRowHandle) + 1);
				ShowRowBreak = DataControl.IsGroupRowHandleCore(nextRowHandle) || visibleIndexes.IsSingleGroupRow(nextRowHandle);
			} else
				ShowRowBreak = false;
		}
#if DEBUGTEST
		internal void SetShowRowBreakForTests(bool showRowBreak) {
			ShowRowBreak = showRowBreak;
		}
#endif
		internal override void UpdateLineLevel() {
			if(parentNodeContainer != null) 
				RowPosition = ((DataNodeContainer)parentNodeContainer).GetRowPosition(node);
			bool isLastRow = false;
			if(((RowPosition != Grid.RowPosition.Bottom) && (RowPosition != Grid.RowPosition.Single)) || 
			(View.ShowFixedTotalSummary || View.ShowTotalSummary)) {
				LineLevel = 0;
				DetailLevel = GetDetailLevel(this, 0, ref isLastRow);
				return;
			}
			if(DataControl.VisibleRowCount > (DataControl.GetRowVisibleIndexByHandleCore(RowHandle.Value) + 1)) {
				LineLevel = 0;
				DetailLevel = GetDetailLevel(this, 0, ref isLastRow);
				return;
			}
			DataViewBase targetView = null;
			int targetVisibleIndex = -1;
			if(DataControl.DataControlParent.FindMasterRow(out targetView, out targetVisibleIndex)) {
				RowData rowData = targetView.GetRowData(targetView.DataControl.GetRowHandleByVisibleIndexCore(targetVisibleIndex));
				if(rowData != null) {
					isLastRow = rowData.RowPosition == Grid.RowPosition.Bottom || rowData.RowPosition == Grid.RowPosition.Single;
					LineLevel = GetLineLevel(this, 0, 0);
					DetailLevel = GetDetailLevel(this, 0, ref isLastRow);
					if(isLastRow)
						LineLevel = DetailLevel;
					return;
				}
			}
			LineLevel = 0;
			DetailLevel = 0;
		}
		bool SupportsDataErrorInfo() {
			return (Row is IDataErrorInfo) || (Row is IDXDataErrorInfo) || DataControl.DataProviderBase.HasValidationAttributes();
		}
		internal void UpdateDataErrors(bool customValidate = true) {
			IterateNotNullData((column, cellData) => UpdateCellDataError(column, cellData, customValidate));
			UpdateRowDataError();
		}
		internal virtual void UpdateCellDataError(ColumnBase column, GridColumnData cellData, bool customValidate = true) {
			((GridCellData)cellData).UpdateCellError(RowHandle, column, customValidate);
		}
		internal virtual void UpdateRowDataError() {
			treeBuilder.UpdateRowDataError(this);
		}
		internal bool IsDirty { get; private set; }
		protected override NotImplementedRowDataReusingStrategy CreateReusingStrategy(Func<FrameworkElement> createRowElementDelegate) {
			return new RowDataReusingStrategy(this);
		}
		internal virtual void AssignFrom(RowsContainer parentRowsContainer, NodeContainer parentNodeContainer, RowNode rowNode, bool forceUpdate) {
			UpdateTreeBuilder(((DataNodeContainer)parentNodeContainer).treeBuilder);
			ControllerValues = ((DataRowNode)rowNode).ControllerValues;
			bool changeNode = this.node != rowNode || IsDirty;
			if(changeNode) {
				this.node = rowNode;
				this.parentNodeContainer = parentNodeContainer;
			}
			SyncWithNode();
			if(changeNode) {
				RowHandle = DataRowNode.RowHandle;
				if(CellData == null || forceUpdate)
					UpdateCellData();
				if(UpdateImmediately)
					RefreshData();
				else {
					treeBuilder.UpdateRowData(this);
					UpdateMasterDetailInfo(true, false);
					IsReady = false;
				}
				if(forceUpdate) {
					UpdateEditorButtonVisibilities();
				}
			}
			AssignFromCore(parentNodeContainer, rowNode, forceUpdate);
			View.ViewBehavior.UpdateFixedNoneContentWidth(this);
			ValidateRowsContainer();
			if(!updateSelectionState) return;
			UpdateIndicatorState();
			UpdateSelectionState();
			UpdateNextRowLevel();
			UpdateIsMasterRowExpanded();
			UpdateShowRowBreak();
		}
		protected virtual void SyncWithNode() { }
		protected virtual void CacheRowData() {
			VisualDataTreeBuilder.CacheRowData(this);
		}
		protected virtual void ValidateRowsContainer() { }
		protected virtual void AssignFromCore(NodeContainer nodeContainer, RowNode rowNode, bool forceUpdate) { }
		bool UpdateImmediately {
			get {
				return UpdateImmediatelyCore || this.node == null || !(View.MasterRootNodeContainer.IsScrolling || GetRootDataPresenterAdjustmentInProgress());
			}
		}
		bool GetRootDataPresenterAdjustmentInProgress() {
			return View.RootDataPresenter != null ? View.RootDataPresenter.AdjustmentInProgress : false;
		}
		protected virtual bool UpdateImmediatelyCore { get { return !View.RootView.ViewBehavior.AllowCascadeUpdate; } }
		internal BaseValidationError ValidationErrorInternal { get { return BaseEdit.GetValidationError(this); } }
		internal bool HasValidationErrorInternal { get { return ValidationErrorInternal != null; } }
		#region rowStateClient
		internal IList<DetailIndent> DetailIndents {
			get {
				var dataControl = DataControl;
				if(dataControl == null)
					return null;
				var ownerDescriptor = DataControl.OwnerDetailDescriptor;
				return ownerDescriptor != null ? ownerDescriptor.DetailViewIndents : null;
			}
		}
		IRowStateClient rowStateClient;
		internal void SetRowStateClient(IRowStateClient rowStateClient) {
			if(this.rowStateClient != null)
				throw new InvalidOperationException();
			this.rowStateClient = rowStateClient;
			UpdateClientRowHandle(RowHandleCore);
			if(SelectionState != SelectionState.None)
				UpdateClientSelectionState();
			ITableView tableView = View as ITableView;
			if(tableView != null) {
				UpdateClientScrollingMargin();
				if(!tableView.ShowHorizontalLines)
					UpdateClientHorizontalLineVisibility();
				if(!tableView.ShowVerticalLines)
					UpdateClientVerticalLineVisibility();
				UpdateClientFixedLineWidth();
				UpdateClientFixedLineVisibility();
				UpdateClientFixedNoneContentWidth();
				if(!tableView.ActualShowIndicator)
					UpdateClientShowIndicator();
				UpdateClientIndicatorWidth();
				if(IndicatorState != IndicatorState.None)
					UpdateClientIndicatorState();
				if(HasValidationErrorInternal)
					UpdateClientValidationError();
				if(DataControl.BandsLayoutCore != null) {
					UpdateCellsPanel();
					if(DataControl.BandsLayoutCore.FixedNoneVisibleBands != null)
						UpdateClientFixedNoneBands();
					if(DataControl.BandsLayoutCore.FixedRightVisibleBands != null)
						UpdateClientFixedRightBands();
					if(DataControl.BandsLayoutCore.FixedLeftVisibleBands != null)
						UpdateClientFixedLeftBands();
				}
				if(AlternateRow)
					UpdateClientAlternateBackground();
				if(tableView.ActualShowDetailButtons)
					UpdateClientDetailExpandButtonVisibility();
				if(DetailIndents != null)
					UpdateClientDetailViewIndents();
				UpdateClientMinHeight();
				UpdateClientRowStyle();
				UpdateClientAppearance();
			}
			if(Level > 0)
				UpdateClientLevel();
			UpdateClientRowPosition();
			if(!View.IsKeyboardFocusWithinView)
				UpdateClientFocusWithinState();
			if(FixedNoneCellData != null)
				UpdateClientFixedNoneCellData();
			if(FixedLeftCellData != null)
				UpdateClientFixedLeftCellData(null);
			if(FixedRightCellData != null)
				UpdateClientFixedRightCellData(null);
			UpdateClientView();
			UpdateClientShowRowBreak();
		}
		protected virtual void OnRowHandleChanged(RowHandle newValue) {
			RowHandleCore = newValue;
			UpdateClientRowHandle(newValue);
			if(IsNewItemRow && Row == null)
				UpdateContent();
		}
		void UpdateClientRowHandle(RowHandle newValue) {
			if(rowStateClient != null)
				rowStateClient.UpdateRowHandle(newValue);
		}
		void UpdateClientSelectionState() {
			if(rowStateClient != null)
				rowStateClient.UpdateSelectionState(SelectionState);
		}
		internal virtual void UpdateClientIsFocused() {
			if(rowStateClient != null)
				rowStateClient.UpdateIsFocused();
		}
		void UpdateClientAppearance() {
			if(rowStateClient != null)
				rowStateClient.UpdateAppearance();
		}
		internal void UpdateClientScrollingMargin() {
			if(rowStateClient != null)
				rowStateClient.UpdateScrollingMargin();
		}
		internal void UpdateClientHorizontalLineVisibility() {
			if(rowStateClient != null)
				rowStateClient.UpdateHorizontalLineVisibility();
		}
		internal void UpdateClientVerticalLineVisibility() {
			if(rowStateClient != null)
				rowStateClient.UpdateVerticalLineVisibility();
		}
		internal void UpdateClientFixedLineWidth() {
			if(rowStateClient != null)
				rowStateClient.UpdateFixedLineWidth();
		}
		internal void UpdateClientFixedLineVisibility() {
			if(rowStateClient != null)
				rowStateClient.UpdateFixedLineVisibility();
		}
		internal void UpdateClientIndicatorWidth() {
			if(rowStateClient != null)
				rowStateClient.UpdateIndicatorWidth();
		}
		internal void UpdateClientShowIndicator() {
			if(rowStateClient != null)
				rowStateClient.UpdateShowIndicator();
		}
		protected override void UpdateClientIndicatorState() {
			base.UpdateClientIndicatorState();
			if(rowStateClient != null)
				rowStateClient.UpdateIndicatorState();
		}
		void UpdateClientValidationError() {
			if(rowStateClient != null)
				rowStateClient.UpdateValidationError();
		}
		void UpdateClientFixedNoneContentWidth() {
			if(rowStateClient != null)
				rowStateClient.UpdateFixedNoneContentWidth();
		}
		void UpdateClientFixedNoneCellData() {
			if(rowStateClient != null)
				rowStateClient.UpdateFixedNoneCellData();
		}
		void UpdateClientFixedLeftCellData(IList<GridColumnData> oldValue) {
			if(rowStateClient != null)
				rowStateClient.UpdateFixedLeftCellData(oldValue);
		}
		void UpdateClientFixedRightCellData(IList<GridColumnData> oldValue) {
			if(rowStateClient != null)
				rowStateClient.UpdateFixedRightCellData(oldValue);
		}
		void UpdateClientView() {
			if(rowStateClient != null)
				rowStateClient.UpdateView();
		}
		internal void UpdateClientAlternateBackground() {
			if(rowStateClient != null)
				rowStateClient.UpdateAlternateBackground();
		}
		internal virtual void UpdateClientFocusWithinState() {
			if(rowStateClient != null)
				rowStateClient.UpdateFocusWithinState();
		}
		void UpdateClientLevel() {
			if(rowStateClient != null)
				rowStateClient.UpdateLevel();
		}
		void UpdateClientShowRowBreak() {
			if(rowStateClient != null)
				rowStateClient.UpdateShowRowBreak();
		}
		void UpdateClientRowPosition() {
			if(rowStateClient != null)
				rowStateClient.UpdateRowPosition();
		}
		internal void UpdateClientDetailExpandButtonVisibility() {
			if(rowStateClient != null)
				rowStateClient.UpdateDetailExpandButtonVisibility();
		}
		internal void UpdateClientDetailViewIndents() {
			if(rowStateClient != null)
				rowStateClient.UpdateDetailViewIndents();
		}
		internal void UpdateClientMinHeight() {
			if(rowStateClient != null)
				rowStateClient.UpdateMinHeight();
		}
		internal void UpdateIndicatorContentTemplate() {
			if(rowStateClient != null)
				rowStateClient.UpdateIndicatorContentTemplate();
		}
		internal void UpdateContent() {
			if(rowStateClient != null)
				rowStateClient.UpdateContent();
		}
		internal void UpdateDetails() {
			if(rowStateClient != null)
				rowStateClient.UpdateDetails();
		}
		internal void UpdateClientRowStyle() {
			if(rowStateClient != null)
				rowStateClient.UpdateRowStyle();
		}
		void OnValidationErrorChanged() {
			UpdateClientValidationError();
		}
		internal void UpdateCellsPanel() {
			if(rowStateClient != null)
				rowStateClient.UpdateCellsPanel();
		}
#if DEBUGTEST
		internal static int UpdateMergePanelsCount { get; set; }
#endif
		internal void InvalidateCellsPanel() {
#if DEBUGTEST
			UpdateMergePanelsCount++;
#endif
			if(rowStateClient != null)
				rowStateClient.InvalidateCellsPanel();
		}
		internal void UpdateClientFixedNoneBands() {
			if(rowStateClient != null)
				rowStateClient.UpdateFixedNoneBands();
		}
		internal void UpdateClientFixedLeftBands() {
			if(rowStateClient != null)
				rowStateClient.UpdateFixedLeftBands();
		}
		internal void UpdateClientFixedRightBands() {
			if(rowStateClient != null)
				rowStateClient.UpdateFixedRightBands();
		}
		void OnAlternateRowChanged() {
			UpdateClientAlternateBackground();
		}
		protected override void OnFixedNoneCellDataChanged() {
			base.OnFixedNoneCellDataChanged();
			UpdateClientFixedNoneCellData();
		}
		protected override void OnFixedLeftCellDataChanged(IList<GridColumnData> oldValue) {
			base.OnFixedLeftCellDataChanged(oldValue);
			UpdateClientFixedLeftCellData(oldValue);
		}
		protected override void OnFixedRightCellDataChanged(IList<GridColumnData> oldValue) {
			base.OnFixedRightCellDataChanged(oldValue);
			UpdateClientFixedRightCellData(oldValue);
		}
		protected override void OnFixedNoneContentWidthCahnged() {
			base.OnFixedNoneContentWidthCahnged();
			UpdateClientFixedNoneContentWidth();
		}
#if DEBUGTEST
		internal int UpdateCellBackgroundAppearanceCount { get; private set; }
#endif
		internal void UpdateCellBackgroundAppearance() {
#if DEBUGTEST
			UpdateCellBackgroundAppearanceCount++;
#endif
			IterateNotNullData((column, data) => data.UpdateCellBackgroundAppearance());
		}
#if DEBUGTEST
		internal int UpdateCellForegroundAppearanceCount { get; private set; }
#endif
		internal void UpdateCellForegroundAppearance() {
#if DEBUGTEST
			UpdateCellForegroundAppearanceCount++;
#endif
			IterateNotNullData((column, data) => data.UpdateCellForegroundAppearance());
		}
		internal void UpdateClientIndentScrolling() {
			if(rowStateClient != null)
				rowStateClient.UpdateIndentScrolling();
		}
		void UpdateClientInlineEditForm() {
			if(rowStateClient != null)
				rowStateClient.UpdateInlineEditForm(EditFormData);
		}
		#endregion
		internal void RefreshData() {
			UpdateData();
			IsDirty = false;
			UpdateIsReady();
		}
		void UpdateIsReady() {
			IsReady = GetIsReady();
		}
		internal bool GetIsReady() {
			return GetIsReady(Row);
		}
		internal static bool GetIsReady(object value) {
			return !DevExpress.Data.AsyncServerModeDataController.IsNoValue(value);
		}
		internal void UpdateIsDirty() {
			IsDirty = !IsRowInView();
		}
		internal bool IsRowInView() {
			ISupportVisibleIndex iSupportVisibleIndex = this;
			if(iSupportVisibleIndex.VisibleIndex == StackVisibleIndexPanel.InvisibleIndex) return false;
			int visibleIndex = DataControl.GetRowVisibleIndexByHandleCore(RowHandle.Value);
			int firstChildScrollIndex = DataControl.FindFirstInnerChildScrollIndex(visibleIndex);
			int totalLevel = DataControl.CalcTotalLevel(visibleIndex);
			double itemsOffset = View.RootDataPresenter.CurrentOffset;
			double viewport = ((System.Windows.Controls.Primitives.IScrollInfo)View.RootDataPresenter).ViewportHeight;
			if(!View.RootView.ViewBehavior.AllowPerPixelScrolling)
				viewport += 1;
			return firstChildScrollIndex < itemsOffset + viewport - totalLevel;
		}
		protected override void OnVisibilityChanged(bool isVisible) {
			if(!isVisible) IsDirty = true;
			if(SubscribeRowChangedForVisibleRows) {
				UnsubcribePropertyChanged(Row);
				if(isVisible) {
					SubcribePropertyChanged(Row);
				} 
			}
			if(isVisible) {
				IGridDataRow row = RowElement as IGridDataRow;
				if(row != null)
					row.UpdateContentLayout(); 
			}
		}
		PropertyChangedWeakEventHandler<RowData> propertyChangedHandler;
	static Action<RowData, object, PropertyChangedEventArgs> changeHandler = (owner, o, e) => owner.OnRowPropertyChanged(o, e);
#if DEBUGTEST
		internal
#endif
		PropertyChangedWeakEventHandler<RowData> PropertyChangedHandler {
			get {
				if(propertyChangedHandler == null) {
					propertyChangedHandler = new PropertyChangedWeakEventHandler<RowData>(this, changeHandler);
				}
				return propertyChangedHandler;
			}
		}
		private bool SubscribeRowChangedForVisibleRows { get { return DataControl != null ? DataControl.DataProviderBase.SubscribeRowChangedForVisibleRows : false; } }
		void OnRowChanging(object oldRow, object newRow) {
			IterateNotNullData((column, cellData) => ((GridCellData)cellData).OnRowChanging(oldRow, newRow));
		}
		void OnRowChanged(object oldRow, object newRow) {
			if(IsAsyncServerMode && GetIsVisible()) UpdateIsReady();
			if(SubscribeRowChangedForVisibleRows) {
				UnsubcribePropertyChanged(oldRow);
				SubcribePropertyChanged(newRow);
			}
			if(newRow != null || IsNewItemRow)
				UpdateContent();
		}
		internal void SubcribePropertyChanged(object row) {
			INotifyPropertyChanged notifyPropertyChanged = row as INotifyPropertyChanged;
			if(notifyPropertyChanged != null) {
				notifyPropertyChanged.PropertyChanged -= PropertyChangedHandler.Handler;
				notifyPropertyChanged.PropertyChanged += PropertyChangedHandler.Handler;
			}
		}
		internal void UnsubcribePropertyChanged(object row) {
			INotifyPropertyChanged notifyPropertyChanged = row as INotifyPropertyChanged;
			if(notifyPropertyChanged != null)
				notifyPropertyChanged.PropertyChanged -= PropertyChangedHandler.Handler;
		}
		void OnRowPropertyChanged(object sender, PropertyChangedEventArgs e) {
			Action propertyChangedAction = () => {
				if(DataControl == null) {
					UnsubcribePropertyChanged(Row);
					return;
				}
				if(e != null && !string.IsNullOrEmpty(e.PropertyName)) {
					bool supportDataErrorInfo = SupportsDataErrorInfo();
					List<ColumnBase> columns = DataControl.GetDependentColumns(e.PropertyName);
					for(int i = 0; i < columns.Count; i++) {
						GridColumnData data = null;
						if(CellDataCache.TryGetValue(columns[i], out data)) {
							data.UpdateValue();
							if(supportDataErrorInfo) {
								UpdateCellDataError(columns[i], data);
							}
						}
					}
					if(supportDataErrorInfo) {
						UpdateRowDataError();
					}
					if(View.ViewBehavior.GetServiceUnboundColumns().Any()) 
						UpdateData();
				} else {
					UpdateData();
				}
				View.UpdateCellMergingPanels();
			};
			if(Dispatcher.CheckAccess()) {
				propertyChangedAction();
			} else {
				Dispatcher.BeginInvoke(propertyChangedAction);
			}
		}
		internal virtual void OnIsReadyChanged() {
#if DEBUGTEST
			EventLog.Default.AddEvent(new PropertyValueSnapshot<RowData, bool>("IsReady", this, IsReady));
#endif
			IterateNotNullData((column, cellData) => ((GridCellData)cellData).UpdateIsReady());
		}
		internal void AssignFrom(int rowHandle) {
			AssignFromCore(rowHandle, DataControlBase.InvalidRowIndex, null);
		}
		internal readonly Locker conditionalFormattingLocker = new Locker();
		internal void AssignFromCore(int rowHandle, int listSourceRowIndex, ColumnBase column) {
			ControllerValues = DataIteratorBase.CreateValuesContainer(treeBuilder, new RowHandle(rowHandle));
			RowHandle = ControllerValues.RowHandle;
			UpdateDataCore(column, listSourceRowIndex);
		}
		internal override void UpdateCellData(ColumnBase column, GridColumnData cellData) {
			base.UpdateCellData(column, cellData);
			GridCellData gridCellData = (GridCellData)cellData;
			treeBuilder.UpdateCellData(this, gridCellData, column);
		}
		internal void UpdateCellDataValues() {
			IterateNotNullData((column, cellData) => cellData.UpdateValue());
		}
		internal void ClearBindingValues(ColumnBase column) {
			IterateNotNullData((cellData, index) => { if(column == cellData) index.ClearBindingValue(); });
		}
		protected internal void UpdateCellDataEditorsDisplayText() {
			IterateNotNullData((column, cellData) => {
				GridCellData gridCellData = cellData as GridCellData;
				if(gridCellData == null) return;
				gridCellData.UpdateEditorDisplayText();
			});
		}
		protected internal void UpdatePrintingMergeValue() {
			IterateNotNullData((column, cellData) => {
				GridCellData gridCellData = cellData as GridCellData;
				if(gridCellData == null) return;
				gridCellData.UpdatePrintingMergeValue();
			});
		}
		internal virtual IEnumerable<RowDataBase> GetCurrentViewChildItems() {
			return EmptyEnumerable;
		}
		protected internal override GridColumnData CreateGridCellDataCore() {
			return new EditGridCellData(this);
		}
		protected override bool ShouldUpdateCellDataCore(ColumnBase column, GridColumnData data) {
			return data.Data != DataContext;
		}
		protected internal double GetRowIndent(ColumnBase column) {
			return GetRowIndent(View.ViewBehavior.IsFirstColumn(column));
		}
		protected internal virtual double GetRowIndent(bool isFirstColumn) {
			if(IsNewItemRow && isFirstColumn) {
				return View.ViewBehavior.NewItemRowIndent;
			}
			return 0d;
		}
		protected internal virtual double GetRowLeftMargin(GridColumnData cellData) { return 0d; }
		protected internal override double GetFixedNoneContentWidth(double totalWidth) {
			return View.ViewBehavior.GetFixedNoneContentWidth(totalWidth, RowHandle.Value);
		}
#if DEBUGTEST
		protected override string GetRowSpecificString() {
			return string.Format("RowHandle={0}, ", RowHandle.Value);
		}
#endif
		protected internal override bool GetIsFocusable() {
			return base.GetIsFocusable() || View.ViewBehavior.IsAdditionalRow(RowHandle.Value);
		}
		protected internal virtual void OnHeaderCaptionChanged() { }
		DataTreeBuilderFormatInfoProvider<RowData> formatInfoProvider;
		internal FormatValueProvider GetValueProvider(string fieldName) {
			return formatInfoProvider.GetValueProvider(fieldName);
		}
		internal override bool IsFocusedRow() {
			return View.IsFocusedView && (RowHandle.Value == View.CalcActualFocusedRowHandle());
		}
		protected virtual void OnDataContextChanged() { }
		protected virtual void OnIsRowExpandedChanged() { }
		protected virtual void OnIsRowVisibleChanged() { }
		internal virtual void UpdateClientGroupValueTemplateSelector() { }
		internal virtual void UpdateClientGroupRowTemplateSelector() { }
		internal virtual void UpdateClientSummary() { }
		internal virtual void UpdateClientGroupRowStyle() { }
		internal virtual void UpdateClientCheckBoxSelector() { }
	}
	public class StandaloneRowData : RowData {
		public StandaloneRowData(DataTreeBuilder treeBuilder, bool updateOnlyData = false)
			: base(treeBuilder, updateOnlyData) {
		}
		protected override void UpdateMasterDetailInfo(bool updateRowObjectIfRowExpanded, bool updateDetailRow) { }
		internal override bool CanReuseCellData() {
			return !UpdateOnlyData;
		}
		protected override FrameworkElement CreateRowElement() {
			return new System.Windows.Controls.ContentPresenter();
		}
		internal override void UpdateCellDataError(ColumnBase column, GridColumnData cellData, bool customValidate = true) {
		}
		internal override void UpdateRowDataError() {
		}
	}
	public class AdditionalRowData : RowData {
		public AdditionalRowData(DataTreeBuilder treeBuilder)
			: base(treeBuilder) {
		}
		protected override FrameworkElement CreateRowElement() {
			return new System.Windows.Controls.ContentPresenter();
		}
	}
}

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
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid.Hierarchy;
using DevExpress.Xpf.Editors.Helpers;
using System.Text;
using System.Collections;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Validation;
using System.Windows.Data;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class RowDataBase : DataObjectBase, IItem, INotifyPropertyChanged {
		#region inner classes
		protected class NotImplementedRowDataReusingStrategy {
			public static readonly NotImplementedRowDataReusingStrategy Instance = new NotImplementedRowDataReusingStrategy();
			protected NotImplementedRowDataReusingStrategy() { }
			internal virtual void AssignFrom(RowsContainer parentRowsContainer, NodeContainer parentNodeContainer, RowNode rowNode, bool forceUpdate = false) {
				throw new NotImplementedException();
			}
			internal virtual void CacheRowData() {
				throw new NotImplementedException();
			}
			internal virtual FrameworkElement CreateRowElement() {
				throw new NotImplementedException();
			}
		}
		protected class DetailRowDataReusingStrategy : NotImplementedRowDataReusingStrategy {
			readonly protected RowDataBase rowData;
			readonly Func<FrameworkElement> createRowElementDelegate;
			public DetailRowDataReusingStrategy(RowDataBase rowData, Func<FrameworkElement> createRowElementDelegate) {
				this.rowData = rowData;
				this.createRowElementDelegate = createRowElementDelegate;
			}
			internal override void AssignFrom(RowsContainer parentRowsContainer, NodeContainer parentNodeContainer, RowNode rowNode, bool forceUpdate = false) {
				DetailInfoWithContent.DetailRowNode detailRowNode = (DetailInfoWithContent.DetailRowNode)rowNode;
				rowData.node = detailRowNode;
				rowData.RowsContainer = detailRowNode.childrenRowsContainer;
				RowData masterRowData = ((DetailInfoWithContent.DetailRowsContainer)parentRowsContainer).MasterRowData;
				detailRowNode.AssignToRowData(rowData, masterRowData);
				((DetailRowControlBase)rowData.WholeRowElement).MasterRowData = masterRowData;
			}
			internal override void CacheRowData() {
				((DetailInfoWithContent.DetailRowNode)rowData.node).CurrentRowData = rowData;
			}
			internal override FrameworkElement CreateRowElement() {
				return createRowElementDelegate();
			}
		}
		#endregion
		protected static IEnumerable<RowDataBase> EmptyEnumerable = new RowData[0];
#if SL
		static readonly DependencyPropertyKey HasValidationErrorPropertyKey;
		public static readonly DependencyProperty HasValidationErrorProperty;
		static readonly DependencyPropertyKey ValidationErrorPropertyKey;
		public static readonly DependencyProperty ValidationErrorProperty;
#endif
		static RowDataBase() {
#if SL
			HasValidationErrorPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasValidationError", typeof(bool), typeof(RowDataBase), new PropertyMetadata(false));
			HasValidationErrorProperty = HasValidationErrorPropertyKey.DependencyProperty;
			ValidationErrorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ValidationError", typeof(BaseValidationError), typeof(RowDataBase), new PropertyMetadata(null));
			ValidationErrorProperty = ValidationErrorPropertyKey.DependencyProperty;
#endif
		}
		DataViewBase view;
		internal RowNode node;
		FrameworkElement wholeRowElement;
		protected internal FrameworkElement WholeRowElement {
			get {
				if(wholeRowElement == null)
					InitWholeRowElement();
				return wholeRowElement;
			}
		}
		internal abstract object MatchKey { get; }
		public DataViewBase View {
			get { return view; }
			protected set {
				if(view == value)
					return;
				view = value;
				OnViewChanged();
				NotifyPropertyChanged("View");
			}
		}
		protected virtual void OnViewChanged() {
		}
#if DEBUGTEST
		internal bool RowsContainerSet { get; private set; }
#endif
		RowsContainer rowsContainer;
		public RowsContainer RowsContainer {
			get { return rowsContainer; }
			internal set {
#if DEBUGTEST
				RowsContainerSet = true;
#endif
				if(value != null)
					value.SetOwnerRowData(this);
				if(RowsContainer != value) {
					ClearRowsContainer();
				}
				if(rowsContainer != value) {
					rowsContainer = value;
					RaisePropertyChanged("RowsContainer");
				}
			}
		}
#if SL
		public bool HasValidationError {
			get { return (bool)GetValue(HasValidationErrorProperty); }
		}
		public BaseValidationError ValidationError {
			get { return (BaseValidationError)GetValue(ValidationErrorProperty); }
		}
#endif
		IndicatorState indicatorState;
		public IndicatorState IndicatorState {
			get { return indicatorState; }
			internal set {
				if(indicatorState != value) {
					indicatorState = value;
					RaisePropertyChanged("IndicatorState");
					UpdateClientIndicatorState();
				}
			}
		}
		bool isMasterRowExpanded;
		public bool IsMasterRowExpanded {
			get { return isMasterRowExpanded; }
			protected set {
				if(isMasterRowExpanded != value) {
					isMasterRowExpanded = value;
					RaisePropertyChanged("IsMasterRowExpanded");
				}
			}
		}
		int lineLevel;
		public int LineLevel {
			get { return lineLevel; }
			protected set {
				if(lineLevel != value) {
					lineLevel = value;
					RaisePropertyChanged("LineLevel");
				}
			}
		}
		int detailLevel;
		public int DetailLevel {
			get { return detailLevel; }
			protected set { 
				if(detailLevel != value) {
					detailLevel = value;
					RaisePropertyChanged("DetailLevel");
				}   
			}
		}
		readonly NotImplementedRowDataReusingStrategy reusingStrategy;
		protected RowDataBase(Func<FrameworkElement> createRowElementDelegate = null) {
			SetVisibleIndex(OrderPanelBase.InvisibleIndex);
			this.reusingStrategy = CreateReusingStrategy(createRowElementDelegate);
#if SL
			BindingOperations.SetBinding(this, HasValidationErrorProperty, new Binding() {
				Path = new PropertyPath(BaseEdit.HasValidationErrorProperty),
				RelativeSource = new RelativeSource(RelativeSourceMode.Self)
			});
			BindingOperations.SetBinding(this, ValidationErrorProperty, new Binding() {
				Path = new PropertyPath(BaseEdit.ValidationErrorProperty),
				RelativeSource = new RelativeSource(RelativeSourceMode.Self)
			});
#endif
		}
		protected virtual NotImplementedRowDataReusingStrategy CreateReusingStrategy(Func<FrameworkElement> createRowElementDelegate) {
			return NotImplementedRowDataReusingStrategy.Instance;
		}
		protected void ClearRowsContainer() {
			if(RowsContainer == null) return;
			RowsContainer.StoreFreeData();
			RowsContainer.RaiseItemsRemoved(RowsContainer.Items);
			RowsContainer.Items.Clear(); 
		}
		protected void InitWholeRowElement() {
			FrameworkElement rowElement = reusingStrategy.CreateRowElement();
			rowElement.DataContext = this;
			DataObjectBase.SetDataObject(rowElement, this);
			wholeRowElement = rowElement;
		}
		internal void StoreAsFreeData(RowsContainer dataContainer) {
			dataContainer.StoreFreeRowData(node, this);
			if(RowsContainer != null) RowsContainer.StoreFreeData();
			SetVisibleIndex(OrderPanelBase.InvisibleIndex);
			UpdateFullState();
		}
		protected internal virtual void UpdateFullState() { }
		internal void AssignVirtualizedRowData(RowsContainer parentRowsContainer, NodeContainer parentNodeContainer, RowNode node, bool forceUpdate) {
			bool raise = this.node != node;
			AssignFromInternal(parentRowsContainer, parentNodeContainer, node, forceUpdate);
			parentRowsContainer.UnstoreFreeRowData(node, this);
			reusingStrategy.CacheRowData();
			if(raise)
				RaiseResetEvents();
		}
		protected void AssignChildItems() {
			if(RowsContainer == null) return;
			if((node.NodesContainer != null) && (node.NodesContainer.Items.Count > 0))
				RowsContainer.Synchronize(node.NodesContainer);
			else {
				RowsContainer.BaseSyncronize(node.NodesContainer);
				ClearRowsContainer();
			}
		}
		internal void AssignFromInternal(RowsContainer parentRowsContainer, NodeContainer parentNodeContainer, RowNode rowNode, bool forceUpdate = false) {
			reusingStrategy.AssignFrom(parentRowsContainer, parentNodeContainer, rowNode, forceUpdate);
			AssignChildItems();
		}
		internal virtual bool RequireUpdateRow { get { return true; } }
		internal virtual void UpdateRow() { }
		internal virtual void ClearRow() { }
		internal virtual void UpdateLineLevel() { }
		protected virtual int GetLineLevel(RowDataBase rowData, int lineLevel, int detailCount) {
			DataViewBase targetView = null;
			int targetVisibleIndex = -1;
			if(rowData.View.DataControl.DataControlParent.FindMasterRow(out targetView, out targetVisibleIndex)) {
				RowData row = targetView.GetRowData(targetView.DataControl.GetRowHandleByVisibleIndexCore(targetVisibleIndex));
				if(row != null) {
					if(targetView.DataControl.VisibleRowCount == targetVisibleIndex + 1) {
						if((!targetView.ShowFixedTotalSummary && !targetView.ShowTotalSummary) || targetView.IsRootView) {
							lineLevel++;
							return GetLineLevel(row, lineLevel, detailCount);
						}
					} else {
						if((targetView.GroupCount > 0) &&
							((row.RowPosition == RowPosition.Bottom) || (row.RowPosition == RowPosition.Single))) {
							lineLevel++;
							if(targetView.DataControl.VisibleRowCount > targetVisibleIndex + 1) {
								return lineLevel;
							}
							return GetLineLevel(row, lineLevel, detailCount);
						}
					}
				}
			}
			return lineLevel;
		}
		protected int GetDetailLevel(RowDataBase rowData, int level, ref bool isLastRow) {
			DataViewBase targetView = null;
			int targetVisibleIndex = -1;
			if(rowData.View.DataControl.DataControlParent.FindMasterRow(out targetView, out targetVisibleIndex)) {
				RowData row = targetView.GetRowData(targetView.DataControl.GetRowHandleByVisibleIndexCore(targetVisibleIndex));
				if(row != null) {
					if((targetView.DataControl.VisibleRowCount != targetVisibleIndex + 1) ||
						((targetView.ShowFixedTotalSummary || targetView.ShowTotalSummary) && !targetView.IsRootView)) {
							isLastRow = false;
					}
					level++;
					return GetDetailLevel(row, level, ref isLastRow);
				}
			}
			return level;
		}
		protected virtual void UpdateClientIndicatorState() { 
		}
		#region IItem Members
		FrameworkElement IItem.Element {
			get { return WholeRowElement; }
		}
		IItemsContainer IItem.ItemsContainer { get { return RowsContainer ?? EmptyItemsContainer.Instance; } }
		bool IItem.IsFixedItem { get { return node.IsFixedNode; } }
		bool IItem.IsRowVisible { get { return node.IsRowVisible; } }
		bool IItem.IsItemsContainer { get { return IsItemsContainerCore; } }
		protected virtual bool IsItemsContainerCore { get { return false; } }
		#endregion
		#region ISupportVisibleIndex
		int visibleIndex = 0;
		int ISupportVisibleIndex.VisibleIndex {
			get { return visibleIndex; }
		}
		internal void SetVisibleIndex(int index) {
			if(visibleIndex == index) return;
			bool oldIsVisible = GetIsVisible();
			visibleIndex = index;
			bool newIsVisible = GetIsVisible();
			if(oldIsVisible != newIsVisible)
				OnVisibilityChanged(newIsVisible);
		}
		internal bool GetIsVisible() {
			return visibleIndex != OrderPanelBase.InvisibleIndex;
		}
		protected internal virtual bool GetIsFocusable() {
			return GetIsVisible();
		}
		protected virtual void OnVisibilityChanged(bool isVisible) { 
		}
		#endregion
		#region INotifyPropertyChanged Members
		protected void NotifyPropertyChanged(String info) {
			RaisePropertyChanged(info);
		}
		#endregion
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
			if(RowsContainer != null)
				RowsContainer.Print(level + 1);
		}
		protected virtual string GetRowSpecificString() {
			return string.Empty;
		}
		internal void PrintOnlySelf(string prefix = "") {
			StringBuilder sb = new StringBuilder();
			sb.Append(prefix);
			sb.Append(string.Format("Type={0}, visibleIndex={1}, Element={2}, ", GetType().Name, visibleIndex, WholeRowElement.GetType().Name));
			sb.Append(GetRowSpecificString());
			sb.Append(string.Format("HashCode={0}, ", GetHashCode()));
			sb.Append(string.Format("View={0}, ", View != null ? View.GetHashCode().ToString() : "null"));
			System.Diagnostics.Debug.WriteLine(sb.ToString());
		}
#endif  
		internal virtual bool IsFocusedRow() {
			return false;
		}
	}
	public interface IViewRowData {
		void SetViewAndUpdate(DataViewBase view);
	}
	public abstract class ColumnsRowDataBase : RowDataBase, IViewRowData {
		class UpdateCellDataStrategy : UpdateCellDataStrategyBase<GridColumnData> {
			readonly ColumnsRowDataBase rowData;
			public UpdateCellDataStrategy(ColumnsRowDataBase rowData) {
				this.rowData = rowData;
			}
			public override bool CanReuseCellData { get { return rowData.CanReuseCellData(); } }
			public override Dictionary<ColumnBase, GridColumnData> DataCache { get { return rowData.CellDataCache; } }
			public override GridColumnData CreateNewData() { return rowData.CreateGridCellDataCore(); }
			public override void UpdateData(ColumnBase column, GridColumnData columnData) { rowData.UpdateCellData(column, columnData); }
		}
		static ColumnsRowDataBase() {
		}
		public ColumnsRowDataBase(DataTreeBuilder treeBuilder, Func<FrameworkElement> createRowElementDelegate = null)
			: base(createRowElementDelegate) {
			UpdateTreeBuilder(treeBuilder);
			CellDataCache = new Dictionary<ColumnBase, GridColumnData>();
			updateCellDataStrategy = new UpdateCellDataStrategy(this);
		}
		IList<GridColumnData> cellData;
		public IList<GridColumnData> CellData {
			get { return cellData; }
			internal set {
				if(cellData != value) {
					cellData = value;
					RaisePropertyChanged("CellData");
				}
			}
		}
		IList<GridColumnData> fixedLeftCellData;
		public IList<GridColumnData> FixedLeftCellData {
			get { return fixedLeftCellData; }
			internal set {
				if(fixedLeftCellData != value) {
					var oldValue = fixedLeftCellData;
					fixedLeftCellData = value;
					RaisePropertyChanged("FixedLeftCellData");
					OnFixedLeftCellDataChanged(oldValue);
				}
			}
		}
		IList<GridColumnData> fixedRightCellData;
		public IList<GridColumnData> FixedRightCellData {
			get { return fixedRightCellData; }
			internal set {
				if(fixedRightCellData != value) {
					var oldValue = fixedRightCellData;
					fixedRightCellData = value;
					RaisePropertyChanged("FixedRightCellData");
					OnFixedRightCellDataChanged(oldValue);
				}
			}
		}
		IList<GridColumnData> fixedNoneCellData;
		public IList<GridColumnData> FixedNoneCellData {
			get { return fixedNoneCellData; }
			internal set {
				if(fixedNoneCellData != value) {
					fixedNoneCellData = value;
					RaisePropertyChanged("FixedNoneCellData");
					OnFixedNoneCellDataChanged();
				}
			}
		}
		double fixedNoneContentWidth;
		public double FixedNoneContentWidth {
			get { return fixedNoneContentWidth; }
			set {
				if(fixedNoneContentWidth != value) {
					fixedNoneContentWidth = value;
					RaisePropertyChanged("FixedNoneContentWidth");
					OnFixedNoneContentWidthCahnged();
				}
			}
		}
		public virtual int Level { get { return 0; } }
		readonly UpdateCellDataStrategy updateCellDataStrategy;
		internal Dictionary<ColumnBase, GridColumnData> CellDataCache { get; private set; }
		internal DataTreeBuilder treeBuilder;
		protected DataControlBase DataControl { get { return View != null ? View.DataControl : null; } }
		protected internal virtual GridColumnData CreateGridCellDataCore() {
			return new GridColumnData(this);
		}
		internal GridColumnData GetCellDataByColumn(ColumnBase column) {
			return GetCellDataByColumn(column, true);
		}
		internal GridColumnData GetCellDataByColumn(ColumnBase column, bool updateNewCellData) {
			GridColumnData data;
			if(!CellDataCache.TryGetValue(column, out data)) {
				data = CreateGridCellDataCore();
				if(updateNewCellData)
					UpdateCellData(column, data);
				CellDataCache.Add(column, data);
			}
			return data;
		}
		internal GridColumnData CreateCellDataByColumn(ColumnBase column) {
			GridColumnData data = CreateGridCellDataCore();
			UpdateCellData(column, data);
			return data;
		}
		internal void UpdateTreeBuilder(DataTreeBuilder newTreeBuilder) {
			if(treeBuilder == newTreeBuilder) return;
			treeBuilder = newTreeBuilder;
			View = treeBuilder.View;
		}
		internal bool ShouldUpdateCellData(GridColumnData data, ColumnBase column) {
			return data.Column != column || ShouldUpdateCellDataCore(column, data);
		}
		protected virtual bool ShouldUpdateCellDataCore(ColumnBase column, GridColumnData data) {
			return false;
		}
		internal void UpdateCellData() {
			View.UpdateCellData(this);
		}
		internal virtual void UpdateCellData(ColumnBase column, GridColumnData cellData) {
			cellData.Column = column;
			treeBuilder.UpdateColumnData(this, cellData, column);
		}
		internal virtual bool CanReuseCellData() {
			return true;
		}
		internal virtual void UpdateFixedLeftCellData() {
			if(UpdateOnlyData) return;
			ReuseCellDataNotVirtualized(x => x.FixedLeftCellData, (x, val) => x.FixedLeftCellData = (IList<GridColumnData>)val, treeBuilder.GetFixedLeftColumns());
		}
		internal virtual void UpdateFixedRightCellData() {
			if(UpdateOnlyData) return;
			ReuseCellDataNotVirtualized(x => x.FixedRightCellData, (x, val) => x.FixedRightCellData = (IList<GridColumnData>)val, treeBuilder.GetFixedRightColumns());
		}
		protected virtual bool UpdateOnlyData { get { return false; } }
		public GridCellDataList CreateCellDataList() {
			return UpdateOnlyData ? null : new GridCellDataList(this, treeBuilder.GetVisibleColumns());
		}
		internal void UpdateFixedNoneCellData(bool virtualized) {
			if(UpdateOnlyData) return;
			UpdateFixedNoneCellDataCore(virtualized && treeBuilder.SupportsHorizontalVirtualization);
		}
		protected virtual void UpdateFixedNoneCellDataCore(bool virtualized) {
			if(virtualized && CanReuseCellData()) {
				ITableView tableView = (ITableView)View;
				ReuseCellData<GridColumnData>(x => x.FixedNoneCellData, (x, val) => x.FixedNoneCellData = (IList<GridColumnData>)val, updateCellDataStrategy, tableView.ViewportVisibleColumns, tableView.TableViewBehavior.FixedNoneVisibleColumns.Count, 5);
			} else {
				ReuseCellDataNotVirtualized(x => x.FixedNoneCellData, (x, val) => x.FixedNoneCellData = (IList<GridColumnData>)val, treeBuilder.GetFixedNoneColumns());
			}
		}
		internal void ReuseCellDataNotVirtualized(Func<ColumnsRowDataBase, IList<GridColumnData>> getter, Action<ColumnsRowDataBase, IList> setter, IList<ColumnBase> sourceColumns) {
			ReuseCellDataNotVirtualized<GridColumnData>(getter, setter, updateCellDataStrategy, sourceColumns);
		}
		protected void ReuseCellDataNotVirtualized<TColumnData>(Func<ColumnsRowDataBase, IList<TColumnData>> getter, Action<ColumnsRowDataBase, IList> setter, UpdateCellDataStrategyBase<TColumnData> updateStrategy, IList<ColumnBase> sourceColumns) where TColumnData : GridColumnData {
			ReuseCellData<TColumnData>(getter, setter, updateStrategy, sourceColumns, sourceColumns.Count, 0);
		}
		protected void ReuseCellData<TColumnData>(Func<ColumnsRowDataBase, IList<TColumnData>> getter, Action<ColumnsRowDataBase, IList> setter, UpdateCellDataStrategyBase<TColumnData> updateStrategy, IList<ColumnBase> sourceColumns, int maxDataCount, int bufferLength) where TColumnData : GridColumnData {
			if(updateStrategy.CanReuseCellData) {
				ReuseCellDataHelper.ReuseCellData<TColumnData>(this, getter, setter, updateStrategy, sourceColumns, bufferLength, maxDataCount);
			} else {
				setter(this, new GridCellDataList(this, sourceColumns));
			}
		}
		#region IViewRowData Members
		void IViewRowData.SetViewAndUpdate(DataViewBase view) {
			UpdateTreeBuilder(view.VisualDataTreeBuilder);
			UpdateCellData();
			View.ViewBehavior.UpdateFixedNoneContentWidth(this);
		}
		#endregion
		protected internal virtual double GetFixedNoneContentWidth(double totalWidth) {
			return totalWidth;
		}
		protected virtual void OnFixedLeftCellDataChanged(IList<GridColumnData> oldValue) { }
		protected virtual void OnFixedRightCellDataChanged(IList<GridColumnData> oldValue) { }
		protected virtual void OnFixedNoneCellDataChanged() { }
		protected virtual void OnFixedNoneContentWidthCahnged() { }
	}
}
namespace DevExpress.Xpf.Grid.Native {
	public abstract class UpdateCellDataStrategyBase<TColumnData> where TColumnData : GridColumnData {
		public abstract bool CanReuseCellData { get; }
		public abstract Dictionary<ColumnBase, TColumnData> DataCache { get; }
		public abstract TColumnData CreateNewData();
		public abstract void UpdateData(ColumnBase column, TColumnData columnData);
	}
	public static class ReuseCellDataHelper {
		internal static void ReuseCellData<TColumnData>(ColumnsRowDataBase rowData, Func<ColumnsRowDataBase, IList<TColumnData>> getter, Action<ColumnsRowDataBase, IList> setter, UpdateCellDataStrategyBase<TColumnData> updateStrategy, IList<ColumnBase> sourceColumns, int bufferLength, int maxDataCount) where TColumnData : GridColumnData {
			if(sourceColumns == null)
				return;
			IList<TColumnData> cellDataList = getter(rowData);
			if(cellDataList == null) {
				cellDataList = new VersionedObservableCollection<TColumnData>(Guid.Empty);
				setter(rowData, (IList)cellDataList);
			}
			int delta = Math.Min(bufferLength + sourceColumns.Count, maxDataCount) - cellDataList.Count;
			for(int i = 0; i < delta; i++)
				cellDataList.Add(updateStrategy.CreateNewData());
			Dictionary<ColumnBase, TColumnData> matchedItems = new Dictionary<ColumnBase, TColumnData>();
			List<TColumnData> unmatchedItems = new List<TColumnData>();
			foreach(TColumnData data in cellDataList) {
				if(sourceColumns.Contains(data.Column))
					matchedItems.Add(data.Column, data);
				else
					unmatchedItems.Add(data);
			}
			for(int i = 0; i < sourceColumns.Count; i++) {
				ColumnBase column = sourceColumns[i];
				if(matchedItems.ContainsKey(column))
					continue;
				for(int j = 0; j < unmatchedItems.Count; j++) {
					TColumnData unmatchedData = unmatchedItems[j];
					if(unmatchedData.Column != null && column.ActualEditSettings != null && EditSettingsComparer.IsCompatibleEditSettings(unmatchedData.Column.ActualEditSettings, column.ActualEditSettings)) {
						matchedItems.Add(column, unmatchedData);
						unmatchedItems.Remove(unmatchedData);
						break;
					}
				}
			}
			bool changed = false;
			for(int i = 0; i < sourceColumns.Count; i++) {
				TColumnData data = GetItem<TColumnData>(matchedItems, unmatchedItems, sourceColumns[i]);
				if(data.Column != null && updateStrategy.DataCache.ContainsKey(data.Column) && updateStrategy.DataCache[data.Column] == data) {
					updateStrategy.DataCache.Remove(data.Column);
				}
				if(rowData.ShouldUpdateCellData(data, sourceColumns[i])) {
					updateStrategy.DataCache[sourceColumns[i]] = data;
					updateStrategy.UpdateData(sourceColumns[i], data);
					changed = true;
				} else {
					updateStrategy.DataCache[data.Column] = data;
				}
				if(data.VisibleIndex != i) {
					changed = true;
					data.VisibleIndex = i;
				}
			}
			foreach(TColumnData data in unmatchedItems) {
				if(!OrderPanelBase.IsInvisibleIndex(data.VisibleIndex)) {
					changed = true;
					data.VisibleIndex = OrderPanelBase.InvisibleIndex;
				}
			}
			if(changed)
				((VersionedObservableCollection<TColumnData>)cellDataList).RaiseCollectionChanged();
		}
		static TColumnData GetItem<TColumnData>(Dictionary<ColumnBase, TColumnData> matchedItems, List<TColumnData> unmatchedItems, ColumnBase column) where TColumnData : GridColumnData {
			if(matchedItems.ContainsKey(column))
				return matchedItems[column];
			int index = 0;
			for(int i = 0; i < unmatchedItems.Count; i++) {
				if(unmatchedItems[i].Column == null) {
					index = i;
					break;
				}
			}
			TColumnData result = unmatchedItems[index];
			unmatchedItems.RemoveAt(index);
			return result;
		}
	}
}

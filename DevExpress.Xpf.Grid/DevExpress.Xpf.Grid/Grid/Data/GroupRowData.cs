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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid.Hierarchy;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Collections;
using DevExpress.Xpf.Editors.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class GroupRowNodePrintInfo : RowNodePrintInfo {
		public GridColumn GroupColumn { get; set; }
		public object GroupColumnHeaderCaption { get; set; }
		public object GroupValue { get; set; }
		public string GroupDisplayText { get; set; }
		public Dictionary<SummaryItemBase, object> GroupSummaryValues { get; set; }
	}
	public class GroupNode : DataRowNode {
		public GridControl Grid { get { return (GridControl)DataControl; } }
		internal GridViewBase GridView { get { return (GridViewBase)Grid.View; } }
		public new DataNodeContainer NodesContainer { get { return (DataNodeContainer)base.NodesContainer; } }
#if DEBUGTEST
		public
#else
		protected
#endif
		override bool IsDataExpanded { get { return Grid.DataProviderBase.IsGroupRowExpanded(RowHandle.Value); } }
		internal override bool CanGenerateItems {
			get { return base.CanGenerateItems; }
			set {
				base.CanGenerateItems = value;
				if(NodesContainer == null) return;
				foreach(RowNode node in NodesContainer.Items) {
					if(node is GroupNode) node.CanGenerateItems = value;
				}
			}
		}
		protected override bool CanGenerateItemsCore { get { return true; } }
		public GroupNode(DataTreeBuilder treeBuilder, DataControllerValuesContainer controllerValues)
			: base(treeBuilder, controllerValues) {
			base.NodesContainer = new DataNodeContainer(treeBuilder, Level + 1, this);
			NodesContainer.OnDataChangedCore();
		}
		internal override LinkedList<FreeRowDataInfo> GetFreeRowDataQueue(SynchronizationQueues synchronizationQueues) {
			return synchronizationQueues.FreeGroupRowDataQueue;
		}
		internal override RowDataBase CreateRowData() {
			return GridView.CreateGroupRowDataCore(treeBuilder);
		}
		internal override bool IsFixedNode {
			get {
				if(!IsRowVisible || !IsExpanded || NodesContainer.Items.Count == 0) return false;
				IRootItemsContainer rootItemsContainer = (IRootItemsContainer)GridView.MasterRootRowsContainer;
				if(NodesContainer.Items[0].GetRowData() == rootItemsContainer.ScrollItem && rootItemsContainer.ScrollItemOffset != 0)
					return true;
				return ControllerValues.VisibleIndex + 1 < ((DataRowNode)NodesContainer.Items[0]).ControllerValues.VisibleIndex;
			}
		}
		protected override void ValidateControllerValues() {
			if(RowHandle.Value >= 0)
				throw new ArgumentException("Internal error: RowHandle should be negative");
		}
		internal override RowNode GetNodeToScroll() {
			if(GridView.AllowFixedGroupsCore || !IsRowVisible) {
				RowNode node = base.GetNodeToScroll();
				if(node == this && GridView.ShowGroupSummaryFooter && summaryNode != null) {
					return summaryNode;
				}
				else {
					return node;
				}
			}
			return this;
		}
		public override int CurrentLevelItemCount { 
			get {
				int count = NodesContainer.CurrentLevelItemCount;
				if(IsRowVisible)
					count++;
				return count; 
			}
		}
		internal RowNode summaryNode;
	}
	public class GroupRowData : RowData {
		#region UpdateGroupSummaryDataStrategy
		class UpdateGroupSummaryDataStrategy : UpdateCellDataStrategyBase<GridGroupSummaryColumnData> {
			readonly GroupRowData groupRowData;
			readonly Dictionary<ColumnBase, GridGroupSummaryColumnData> groupSymmaryDataCache;
			public UpdateGroupSummaryDataStrategy(GroupRowData groupRowData) {
				this.groupRowData = groupRowData;
				this.groupSymmaryDataCache = new Dictionary<ColumnBase, GridGroupSummaryColumnData>();
			}
			public override bool CanReuseCellData { get { return true; } }
			public override Dictionary<ColumnBase, GridGroupSummaryColumnData> DataCache { get { return groupSymmaryDataCache; } }
			public override GridGroupSummaryColumnData CreateNewData() { return groupRowData.CreateNewGroupSummaryColumnData(); }
			public override void UpdateData(ColumnBase column, GridGroupSummaryColumnData columnData) { groupRowData.UpdateGridGroupSummaryData(column, columnData); }
		}
		#endregion
		#region static
		public static readonly DependencyProperty GroupSummaryDataProperty;
		public static readonly DependencyProperty GroupValueProperty;
		public static readonly DependencyProperty GroupLevelProperty;
		static readonly DependencyPropertyKey GroupLevelPropertyKey;
		static readonly DependencyPropertyKey FixedLeftGroupSummaryDataPropertyKey;
		public static readonly DependencyProperty FixedLeftGroupSummaryDataProperty;
		static readonly DependencyPropertyKey FixedRightGroupSummaryDataPropertyKey;
		public static readonly DependencyProperty FixedRightGroupSummaryDataProperty;
		static readonly DependencyPropertyKey FixedNoneGroupSummaryDataPropertyKey;
		public static readonly DependencyProperty FixedNoneGroupSummaryDataProperty;
		static readonly DependencyPropertyKey IsLastVisibleElementRowPropertyKey;
		public static readonly DependencyProperty IsLastVisibleElementRowProperty;
		public static readonly DependencyProperty AllItemsSelectedProperty;
		static readonly DependencyPropertyKey IsLastHierarchicalRowPropertyKey;
		public static readonly DependencyProperty IsLastHierarchicalRowProperty;
		static readonly DependencyPropertyKey IsPreviewExpandedPropertyKey;
		public static readonly DependencyProperty IsPreviewExpandedProperty;
		static GroupRowData() {
			GroupSummaryDataProperty = DependencyPropertyManager.Register("GroupSummaryData", typeof(IList<GridGroupSummaryData>), typeof(GroupRowData), new PropertyMetadata(null));
			GroupValueProperty = DependencyPropertyManager.Register("GroupValue", typeof(GridGroupValueData), typeof(GroupRowData), new PropertyMetadata(null));
			GroupLevelPropertyKey = DependencyPropertyManager.RegisterReadOnly("GroupLevel", typeof(int), typeof(GroupRowData), new PropertyMetadata(0));
			GroupLevelProperty = GroupLevelPropertyKey.DependencyProperty;
			FixedLeftGroupSummaryDataPropertyKey = DependencyPropertyManager.RegisterReadOnly("FixedLeftGroupSummaryData", typeof(IList<GridGroupSummaryColumnData>), typeof(GroupRowData), new PropertyMetadata(null));
			FixedLeftGroupSummaryDataProperty = FixedLeftGroupSummaryDataPropertyKey.DependencyProperty;
			FixedRightGroupSummaryDataPropertyKey = DependencyPropertyManager.RegisterReadOnly("FixedRightGroupSummaryData", typeof(IList<GridGroupSummaryColumnData>), typeof(GroupRowData), new PropertyMetadata(null));
			FixedRightGroupSummaryDataProperty = FixedRightGroupSummaryDataPropertyKey.DependencyProperty;
			FixedNoneGroupSummaryDataPropertyKey = DependencyPropertyManager.RegisterReadOnly("FixedNoneGroupSummaryData", typeof(IList<GridGroupSummaryColumnData>), typeof(GroupRowData), new PropertyMetadata(null));
			FixedNoneGroupSummaryDataProperty = FixedNoneGroupSummaryDataPropertyKey.DependencyProperty;
			IsLastVisibleElementRowPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsLastVisibleElementRow", typeof(bool), typeof(GroupRowData), new PropertyMetadata(false));
			IsLastVisibleElementRowProperty = IsLastVisibleElementRowPropertyKey.DependencyProperty;
			IsLastHierarchicalRowPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsLastHierarchicalRow", typeof(bool), typeof(GroupRowData), new PropertyMetadata(false));
			IsLastHierarchicalRowProperty = IsLastHierarchicalRowPropertyKey.DependencyProperty;
			IsPreviewExpandedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsPreviewExpanded", typeof(bool), typeof(GroupRowData), new PropertyMetadata(false));
			IsPreviewExpandedProperty = IsPreviewExpandedPropertyKey.DependencyProperty;
			AllItemsSelectedProperty = DependencyProperty.Register("AllItemsSelected", typeof(bool?), typeof(GroupRowData), new PropertyMetadata(null, (d, e) => ((GroupRowData)d).OnAllItemsSelectedChanged()));
		}
		internal static string GetColumnDisplayFormat(ColumnBase column) {
			return column != null ? column.DisplayFormat : string.Empty;
		}
		#endregion
		readonly UpdateGroupSummaryDataStrategy updateGroupSummaryDataStrategy;
		public GroupRowData(DataTreeBuilder treeBuilder)
			: base(treeBuilder) {
				this.updateGroupSummaryDataStrategy = new UpdateGroupSummaryDataStrategy(this);
		}
		#region Dependency properties
#if !SL
	[DevExpressXpfGridLocalizedDescription("GroupRowDataGroupValue")]
#endif
		public GridGroupValueData GroupValue {
			get { return (GridGroupValueData)GetValue(GroupValueProperty); }
			set { SetValue(GroupValueProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GroupRowDataGroupLevel")]
#endif
		public int GroupLevel {
			get { return (int)GetValue(GroupLevelProperty); }
			private set { this.SetValue(GroupLevelPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GroupRowDataGroupSummaryData")]
#endif
		public IList<GridGroupSummaryData> GroupSummaryData {
			get { return (IList<GridGroupSummaryData>)GetValue(GroupSummaryDataProperty); }
			set { SetValue(GroupSummaryDataProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GroupRowDataFixedLeftGroupSummaryData")]
#endif
		public IList<GridGroupSummaryColumnData> FixedLeftGroupSummaryData {
			get { return (IList<GridGroupSummaryColumnData>)GetValue(FixedLeftGroupSummaryDataProperty); }
			private set { this.SetValue(FixedLeftGroupSummaryDataPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GroupRowDataFixedRightGroupSummaryData")]
#endif
		public IList<GridGroupSummaryColumnData> FixedRightGroupSummaryData {
			get { return (IList<GridGroupSummaryColumnData>)GetValue(FixedRightGroupSummaryDataProperty); }
			private set { this.SetValue(FixedRightGroupSummaryDataPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GroupRowDataFixedNoneGroupSummaryData")]
#endif
		public IList<GridGroupSummaryColumnData> FixedNoneGroupSummaryData {
			get { return (IList<GridGroupSummaryColumnData>)GetValue(FixedNoneGroupSummaryDataProperty); }
			private set { this.SetValue(FixedNoneGroupSummaryDataPropertyKey, value); }
		}
		public bool IsLastVisibleElementRow {
			get { return (bool)GetValue(IsLastVisibleElementRowProperty); }
			private set { this.SetValue(IsLastVisibleElementRowPropertyKey, value); }
		}
		public bool? AllItemsSelected {
			get { return (bool?)GetValue(AllItemsSelectedProperty); }
			set { SetValue(AllItemsSelectedProperty, value); }
		}
		public bool IsLastHierarchicalRow {
			get { return (bool)GetValue(IsLastHierarchicalRowProperty); }
			private set { this.SetValue(IsLastHierarchicalRowPropertyKey, value); }
		}
		public bool  IsPreviewExpanded {
			get { return (bool)GetValue(IsPreviewExpandedProperty); }
			private set { this.SetValue(IsPreviewExpandedPropertyKey, value); }
		}
		#endregion
		public DataRowsContainer DataRowsContainer { get { return (DataRowsContainer)base.RowsContainer; } }
		#region ShouldSerialize methods
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeGroupSummaryData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeCellData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedLeftCellData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedRightCellData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedNoneCellData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedLeftGroupSummaryData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedRightGroupSummaryData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedNoneGroupSummaryData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
		#endregion
		protected GridViewBase GridView { get { return (GridViewBase)View; } }
		GridControl Grid { get { return GridView.Grid; } }
		protected ITableView TableView { get { return View as ITableView; } }
		public double Offset {
			get {
				if(TableView != null)
					return TableView.LeftGroupAreaIndent * Level - (TableView.ActualShowDetailButtons ? TableView.ActualExpandDetailHeaderWidth : 0);
				if(View is CardView)
					return ((CardView)View).LeftGroupAreaIndent * Level;
				return 0;
			}
		}
		internal override FrameworkElement RowElement { get { return ((IGroupRow)base.RowElement).RowElement; } }
		protected override FrameworkElement CreateRowElement() {
			return GridView.CreateGroupControl(this);
		}
		protected internal override void UpdateGroupSummaryData() {
			RefreshFixedNoneCellData(treeBuilder.SupportsHorizontalVirtualization, true);
			UpdateClientSummary();
		}
		void UpdateGroupSummariesCore() {
			if(DataControl == null) return;
			if(GroupSummaryData == null)
				GroupSummaryData = new ObservableCollection<GridGroupSummaryData>();
			int actualSummaryCount = 0;
			IList<SummaryItemBase> summaries = treeBuilder.GetGroupSummaries();
			foreach(GridSummaryItem item in summaries) {
				object value = null;
				if(treeBuilder.TryGetGroupSummaryValue(this, item, out value) && CanUpdateSummary(item))
					actualSummaryCount++;
			}
			int delta = actualSummaryCount - GroupSummaryData.Count;
			if(delta > 0) {
				for(int i = 0; i < delta; i++) {
					GroupSummaryData.Add(new GridGroupSummaryData(this));
				}
			} else {
				for(int i = 0; i < -delta; i++) {
					GroupSummaryData.RemoveAt(GroupSummaryData.Count - 1);
				}
			}
			int index = 0;
			for(int i = 0; i < summaries.Count; i++) {
				GridSummaryItem summaryItem = (GridSummaryItem)summaries[i];
				GridColumn column = Grid.Columns[summaryItem.FieldName];
				object value = null;
				if(!treeBuilder.TryGetGroupSummaryValue(this, summaryItem, out value) || !CanUpdateSummary(summaryItem))
					continue;
				GridGroupSummaryData data = GroupSummaryData[index];
				data.Column = column;
				data.Text = GetGroupSummaryText(summaryItem, column, View as TableView, value);
				data.SummaryItem = summaryItem;
				data.Data = DataContext;
				data.Value = value;
				data.SummaryValue = value;
				data.IsLast = (index == (GroupSummaryData.Count - 1));
				data.IsFirst = (i == 0);
				index++;
			}
		}
		bool CanUpdateSummary(GridSummaryItem item) {
			return item.Visible && CanExtractGridGroupSummaryItem(item);
		}
		protected IList<GridSummaryItem> ExtractGridGroupSummaries(IList<SummaryItemBase> summaries) {
			List<GridSummaryItem> summariesList = new List<GridSummaryItem>();
			foreach(SummaryItemBase item in summaries) {
				GridSummaryItem summary = (GridSummaryItem)item;
				if(CanExtractGridGroupSummaryItem(summary))
					summariesList.Add(summary);
			}
			return summariesList;
		}
		protected virtual bool CanExtractGridGroupSummaryItem(GridSummaryItem summary) { 
			return string.IsNullOrEmpty(summary.ShowInGroupColumnFooter);
		}
		internal string GetGroupSummaryText(GridSummaryItem summaryItem, ColumnBase column, TableView tableView, object value, bool isPrinting = false) {
			if(tableView == null || GetActualGroupSummaryDisplayMode(tableView, isPrinting) == GroupSummaryDisplayMode.Default)
				return summaryItem.GetGroupDisplayText(CultureInfo.CurrentCulture, GridColumn.GetSummaryDisplayName(column, summaryItem), value, GetColumnDisplayFormat(column));
			else
				return summaryItem.GetGroupColumnDisplayText(CultureInfo.CurrentCulture, GridColumn.GetSummaryDisplayName(column, summaryItem), value, GetColumnDisplayFormat(column));
		}
		GroupSummaryDisplayMode GetActualGroupSummaryDisplayMode(TableView tableView, bool isPrinting) {
			return isPrinting ? tableView.PrintGroupSummaryDisplayMode : tableView.GroupSummaryDisplayMode;
		}
		internal override void AssignFrom(RowsContainer parentRowsContainer, NodeContainer parentNodeContainer, RowNode rowNode, bool forceUpdate) {
			base.AssignFrom(parentRowsContainer, parentNodeContainer, rowNode, forceUpdate);
			if(GroupValue == null) {
				GridGroupValueData groupValueData = new GridGroupValueData(this);
				UpdateGroupValueData(groupValueData);
				GroupValue = groupValueData;
			}
			GroupValue.Column = treeBuilder.GetGroupColumnByNode(DataRowNode);
			GroupValue.Value = treeBuilder.GetGroupValueByNode(DataRowNode);
			UpdateDisplayText();
			IsLastVisibleElementRow = GetIsLastVisibleElementRow(RowHandle.Value, true);
			IsLastHierarchicalRow = GetIsLastHierarchicalRow(RowHandle.Value);
			IsPreviewExpanded = GetIsPreviewExpanded(RowHandle.Value);
			if(groupRowStateClient != null)
				groupRowStateClient.UpdateIsPreviewExpanded();
			UpdateEditorHighlightingText();
			treeBuilder.UpdateGroupRowData(this);
		}
		protected override void SyncWithNode() {
			base.SyncWithNode();
			var groupNode = (GroupNode)DataRowNode;
			IsRowExpanded = groupNode.IsExpanded;
			IsExpanding = groupNode.IsExpanding;
			IsRowVisible = groupNode.IsRowVisible;
			GroupLevel = Level;
		}
		protected override void OnRowHandleChanged(RowHandle newValue) {
			base.OnRowHandleChanged(newValue);
			UpdateAllItemsSelected();
		}
		protected internal override void OnHeaderCaptionChanged() {
			UpdateDisplayText();
		}
		void UpdateDisplayText() {
			if(GroupValue == null)
				return;
			GroupValue.DisplayText = treeBuilder.GetGroupRowDisplayTextByNode(DataRowNode);
			UpdateClientGroupValue();
		}
		protected override void UpdateMasterDetailInfo(bool updateRowObjectIfRowExpanded, bool updateDetailRow) { }
		bool GetIsLastVisibleElementRow(int rowHandle, bool checkParentRow) {
			if(!Grid.IsGroupRowExpanded(rowHandle))
				return false;
			if(Level > 0 && checkParentRow) {
				if(GetLastRowHandle(Grid.GetParentRowHandle(rowHandle)) == rowHandle)
					return false;
			}
			int lastRowHandle = GetLastRowHandle(rowHandle);
			if(!Grid.IsGroupRowHandle(lastRowHandle))
				return true;
			return GetIsLastVisibleElementRow(lastRowHandle, false);
		}
		bool GetIsLastHierarchicalRow(int rowHandle) {
			return this.ControllerVisibleIndex == Grid.VisibleRowCount - 1;
		}
		bool GetIsPreviewExpanded(int rowHandle) {
			if(this.RowPosition == Xpf.Grid.RowPosition.Top || this.RowPosition == Xpf.Grid.RowPosition.Single)
				return false;
			return Grid.IsGroupRowExpanded(Grid.GetRowHandleByVisibleIndex(this.ControllerVisibleIndex - 1));		   
		}
		int GetLastRowHandle(int rowHandle) {
			return Grid.GridDataProvider.GetChildRowHandle(rowHandle, Grid.GridDataProvider.GetChildRowCount(rowHandle) - 1);
		}
		void UpdateGroupColumnSummaries() {
			IterateNotNullDataCore<GridGroupSummaryColumnData>(updateGroupSummaryDataStrategy.DataCache, UpdateGridGroupSummaryData);
		}
		protected override void ValidateRowsContainer() {
			if(RowsContainer == null)
				base.RowsContainer = new DataRowsContainer(treeBuilder, Level + 1);
		}
		protected internal override GridColumnData CreateGridCellDataCore() {
			return new GridCellData(this);
		}
		protected internal override void UpdateEditorButtonVisibilities() {
		}
		protected override bool UpdateImmediatelyCore { get { return !View.RootView.GetAllowGroupSummaryCascadeUpdate; } }
		internal override bool CanReuseCellData() {
			return false;
		}
		internal override IEnumerable<RowDataBase> GetCurrentViewChildItems() {
			return (RowsContainer != null) ? RowsContainer.Items : base.GetCurrentViewChildItems();
		}
		internal override void UpdateFixedLeftCellData() {
			base.UpdateFixedLeftCellData();
			GridView.PerformUpdateGroupSummaryDataAction(() => ReuseGroupSummaryDataNotVirtualized(x => ((GroupRowData)x).FixedLeftGroupSummaryData, (x, val) => ((GroupRowData)x).FixedLeftGroupSummaryData = (IList<GridGroupSummaryColumnData>)val, treeBuilder.GetFixedLeftColumns()));
			UpdateClientSummary();
		}
		internal override void UpdateFixedRightCellData() {
			base.UpdateFixedRightCellData();
			GridView.PerformUpdateGroupSummaryDataAction(() => ReuseGroupSummaryDataNotVirtualized(x => ((GroupRowData)x).FixedRightGroupSummaryData, (x, val) => ((GroupRowData)x).FixedRightGroupSummaryData = (IList<GridGroupSummaryColumnData>)val, treeBuilder.GetFixedRightColumns()));
			UpdateClientSummary();
		}
		protected override void UpdateFixedNoneCellDataCore(bool virtualized) {
			base.UpdateFixedNoneCellDataCore(virtualized);
			RefreshFixedNoneCellData(virtualized, false);
		}
		void RefreshFixedNoneCellData(bool virtualized, bool updateGroupSummary) {
			GridView.PerformUpdateGroupSummaryDataAction(() => {
				if(updateGroupSummary)
					UpdateGroupColumnSummaries();
				if(virtualized) {
					ITableView tableView = (ITableView)View;
					ReuseCellData<GridGroupSummaryColumnData>(x => ((GroupRowData)x).FixedNoneGroupSummaryData, (x, val) => ((GroupRowData)x).FixedNoneGroupSummaryData = (IList<GridGroupSummaryColumnData>)val, updateGroupSummaryDataStrategy, tableView.ViewportVisibleColumns, tableView.TableViewBehavior.FixedNoneVisibleColumns.Count, 0);
					UpdateHasRightSibling(tableView.ViewportVisibleColumns ?? treeBuilder.GetFixedNoneColumns());					
				} else
					ReuseGroupSummaryDataNotVirtualized(x => ((GroupRowData)x).FixedNoneGroupSummaryData, (x, val) => ((GroupRowData)x).FixedNoneGroupSummaryData = (IList<GridGroupSummaryColumnData>)val, treeBuilder.GetFixedNoneColumns());
				return;
			});
			UpdateGroupSummariesCore();
		}
		internal void ReuseGroupSummaryDataNotVirtualized(Func<ColumnsRowDataBase, IList<GridGroupSummaryColumnData>> getter, Action<ColumnsRowDataBase, System.Collections.IList> setter, IList<ColumnBase> sourceColumns) {
			ReuseCellDataNotVirtualized<GridGroupSummaryColumnData>(getter, setter, updateGroupSummaryDataStrategy, sourceColumns);
			UpdateHasRightSibling(sourceColumns);
		}
		void UpdateHasRightSibling(IList<ColumnBase> sourceColumns) {
			if(sourceColumns == null)
				return;
			for(int i = 0; i < sourceColumns.Count; i++) {
				ColumnBase column = sourceColumns[i];
				GridGroupSummaryColumnData data = SafeGetGroupSummaryColumnData(column);
				if(data == null)
					continue;
				bool hasRightSibling = column.HasRightSibling;
				if(hasRightSibling) {
					bool nextColumnIsEmpty = false;
					if(i < sourceColumns.Count - 1) {
						GridGroupSummaryColumnData nextData = SafeGetGroupSummaryColumnData(sourceColumns[i + 1]);
						if(nextData != null)
							nextColumnIsEmpty = string.IsNullOrEmpty((string)nextData.Value);
					}
					bool currentColumnIsEmpty = string.IsNullOrEmpty((string)data.Value);
					if(nextColumnIsEmpty && currentColumnIsEmpty)
						hasRightSibling = false;
				}
				data.HasRightSibling = hasRightSibling;
			}
		}
		protected internal GridGroupSummaryColumnData SafeGetGroupSummaryColumnData(ColumnBase column) {
			GridGroupSummaryColumnData data;
			updateGroupSummaryDataStrategy.DataCache.TryGetValue(column, out data);
			return data;
		}
		GridGroupSummaryColumnData CreateNewGroupSummaryColumnData() {
			return new GridGroupSummaryColumnData(this);
		}
		protected internal virtual void UpdateGridGroupSummaryData(ColumnBase column, GridGroupSummaryColumnData cellData) {
			IList<GridSummaryItem> summaries = ExtractGridGroupSummaries(column.GroupSummariesCore);
			cellData.Column = column;
			cellData.HasSummary = summaries.Count > 0;
			cellData.Value = GridView.GetGroupSummaryText(column, RowHandle.Value, IsGroupFooter); 
		}
		protected virtual bool IsGroupFooter { get { return false; } }
		protected override bool ShouldUpdateCellDataCore(ColumnBase column, GridColumnData data) {
			return false;
		}
		protected internal void SetRowHandle(RowHandle rowHandle) {
			RowHandle = rowHandle;
		}
		protected override bool IsItemsContainerCore { get { return true; } }
		protected override void OnDataContextChanged() {
			base.OnDataContextChanged();
			UpdateGroupValueData(GroupValue);
		}
		protected override void OnIsRowExpandedChanged() {
			base.OnIsRowExpandedChanged();
			if(groupRowStateClient != null)
				groupRowStateClient.UpdateIsRowExpanded();
		}
		protected override void OnIsRowVisibleChanged() {
			base.OnIsRowVisibleChanged();
			if(groupRowStateClient != null)
				groupRowStateClient.UpdateIsRowVisible();
		}
		void UpdateGroupValueData(GridGroupValueData groupValueData) {
			if(groupValueData != null)
				groupValueData.Data = DataContext;
		}
		#region CheckBoxSelector
		Locker UpdateAllItemsSelectedLocker = new Locker();
#if DEBUGTEST
		public static int updateAllItemsSelectedInvokeCount;
#endif
		internal void SetAllItemsSelected(bool? allItemsSelected) {
			UpdateAllItemsSelectedLocker.DoLockedAction(() => {
				AllItemsSelected = allItemsSelected;
			});
		}
		internal void UpdateAllItemsSelected() {
			if(!GridView.ActualShowCheckBoxSelectorInGroupRow)
				return;
#if DEBUGTEST
			updateAllItemsSelectedInvokeCount++;
#endif
			SetAllItemsSelected(GridView.AreAllItemsSelected(RowHandle.Value));
		}
		void OnAllItemsSelectedChanged() {
			if(!AllItemsSelected.HasValue)
				return;
			UpdateAllItemsSelectedLocker.DoIfNotLocked(() => {
				if(AllItemsSelected.Value)
					GridView.SelectRowRecursively(RowHandle.Value);
				else
					GridView.UnselectRowRecursively(RowHandle.Value);
			});
		}
		#endregion
		#region GroupRowStateClient
		IGroupRowStateClient groupRowStateClient;
		internal void SetGroupRowStateClient(IGroupRowStateClient rowStateClient) {
			if(this.groupRowStateClient != null)
				throw new InvalidOperationException();
			SetRowStateClient(rowStateClient);
			groupRowStateClient = rowStateClient;
			groupRowStateClient.UpdateGroupValue();
			groupRowStateClient.UpdateGroupRowStyle();
		}
		void UpdateClientGroupValue() {
			if(groupRowStateClient != null)
				groupRowStateClient.UpdateGroupValue();
		}
		internal override void UpdateClientGroupValueTemplateSelector() {
			if(groupRowStateClient != null)
				groupRowStateClient.UpdateGroupValueTemplateSelector();
		}
		internal override void UpdateClientGroupRowTemplateSelector() {
			if(groupRowStateClient != null)
				groupRowStateClient.UpdateGroupRowTemplateSelector();
		}
		internal override void UpdateClientSummary() {
			if(groupRowStateClient != null)
				groupRowStateClient.UpdateSummary();
		}
		internal override void UpdateClientGroupRowStyle() {
			if(groupRowStateClient != null) {
				groupRowStateClient.UpdateGroupValue();
				groupRowStateClient.UpdateGroupRowStyle();
			}
		}
		internal override void UpdateClientCheckBoxSelector() {
			if(groupRowStateClient != null)
				groupRowStateClient.UpdateCheckBoxSelector();
		}
		internal override void OnIsReadyChanged() {
			if(groupRowStateClient != null)
				groupRowStateClient.UpdateIsReady();
			foreach(var data in GetFixedGroupSummaryData())
				data.UpdateSummaryIsReady();
		}
		internal override void UpdateClientFocusWithinState() {
			base.UpdateClientFocusWithinState();
			UpdateGroupSummaryClientFocusState();
		}
		internal override void UpdateClientIsFocused() {
			base.UpdateClientIsFocused();
			UpdateGroupSummaryClientFocusState();
		}
		void UpdateGroupSummaryClientFocusState() {
			foreach(var data in GetFixedGroupSummaryData())
				data.UpdateSummaryClientFocusState();
		}
		IEnumerable<GridGroupSummaryColumnData> GetFixedGroupSummaryData() {
			if(FixedNoneGroupSummaryData != null) {
				foreach(GridGroupSummaryColumnData data in FixedNoneGroupSummaryData)
					yield return data;
			}
			if(FixedLeftGroupSummaryData != null) {
				foreach(GridGroupSummaryColumnData data in FixedLeftGroupSummaryData)
					yield return data;
			}
			if(FixedRightGroupSummaryData != null) {
				foreach(GridGroupSummaryColumnData data in FixedRightGroupSummaryData)
					yield return data;
			}
		}
		internal void UpdateCardLayout() {
			if(groupRowStateClient != null)
				groupRowStateClient.UpdateCardLayout();
		}
		#endregion
		protected internal override void UpdateEditorHighlightingText() {
			if(View == null || GroupValue == null || GroupValue.Column == null || GroupValue.Column.ActualEditSettings == null) 
				return;
			SearchControlHelper.UpdateTextHighlighting(GroupValue.Column.ActualEditSettings, View.GetTextHighlightingProperties(GroupValue.Column));
		}
	}
	public interface IGroupRowStateClient : IRowStateClient {
		void UpdateGroupValue();
		void UpdateIsRowExpanded();
		void UpdateSummary();
		void UpdateGroupValueTemplateSelector();
		void UpdateGroupRowTemplateSelector();
		void UpdateGroupRowStyle();
		void UpdateIsReady();
		void UpdateCheckBoxSelector();
		void UpdateIsRowVisible();
		void UpdateIsPreviewExpanded();
		void UpdateCardLayout();
	}
}

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
using System.Windows.Controls;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class GroupSummaryRowNode : GroupNode {
		public GroupSummaryRowNode(DataTreeBuilder treeBuilder, DataControllerValuesContainer controllerValues)
			: base(treeBuilder, controllerValues) {
			this.MatchKeyCore = new GroupSummaryRowKey(controllerValues.RowHandle, Level);
		}
		internal GroupSummaryRowData CurrentRowData { get; set; }
		protected GroupSummaryRowKey MatchKeyCore { get; private set; }
		public override object MatchKey {
			get { return MatchKeyCore; }
		}
		internal override LinkedList<FreeRowDataInfo> GetFreeRowDataQueue(SynchronizationQueues synchronizationQueues) {
			return synchronizationQueues.FreeGroupSummaryRowDataQueue;
		}
		internal override RowDataBase CreateRowData() {
			return new GroupSummaryRowData(treeBuilder, RowHandle);
		}
		internal override RowDataBase GetRowData() {
			return CurrentRowData;
		}
		internal override FrameworkElement GetRowElement() {
			return CurrentRowData.RowElement;
		}
	}
	public class GroupSummaryRowData : GroupRowData {
		public static readonly DependencyProperty FooterPositionProperty;
		static readonly DependencyPropertyKey FooterPositionPropertyKey;
		static GroupSummaryRowData() {
			FooterPositionPropertyKey = DependencyPropertyManager.RegisterReadOnly("FooterPosition", typeof(RowPosition), typeof(GroupSummaryRowData), new FrameworkPropertyMetadata(RowPosition.Single));
			FooterPositionProperty = FooterPositionPropertyKey.DependencyProperty;
		}
		public GroupSummaryRowData(DataTreeBuilder treeBuilder, RowHandle rowHandle)
			: base(treeBuilder) {
			this.RowHandle = rowHandle;
			this.MatchKeyCore = new GroupSummaryRowKey(RowHandle, Level);
		}
		public RowPosition FooterPosition {
			get { return (RowPosition)GetValue(FooterPositionProperty); }
			protected set { this.SetValue(FooterPositionPropertyKey, value); }
		}
		protected internal GroupSummaryRowKey MatchKeyCore { get; private set; }
		internal override object MatchKey { get { return MatchKeyCore; } }
		protected override RowDataBase.NotImplementedRowDataReusingStrategy CreateReusingStrategy(Func<FrameworkElement> createRowElementDelegate) {
			return new GroupSummaryRowDataReusingStrategy(this);
		}
		protected override void CacheRowData() {
			VisualDataTreeBuilder.CacheGroupSummaryRowData(this);
		}
		#region inner classes
		protected class GroupSummaryRowDataReusingStrategy : RowDataReusingStrategy {
			public GroupSummaryRowDataReusingStrategy(GroupSummaryRowData rowData) : base(rowData) { }
			protected GroupSummaryRowData CurrentRowData { get { return rowData as GroupSummaryRowData; } }
			internal override void CacheRowData() {
				base.CacheRowData();
				((GroupSummaryRowNode)CurrentRowData.node).CurrentRowData = CurrentRowData;
			}
			internal override FrameworkElement CreateRowElement() {
				return CurrentRowData.CreateRowElement();
			}
		}
		#endregion
		internal override void UpdateRow() { }
		internal override FrameworkElement RowElement { get { return ((GroupFooterRowControl)WholeRowElement).GroupFooterContentPresenter; } }
		protected internal override void EnsureRowLoaded() { }
		protected override FrameworkElement CreateRowElement() {
			return new GroupFooterRowControl();
		}
		protected internal override double GetFixedNoneContentWidth(double totalWidth) {
			return base.GetFixedNoneContentWidth(totalWidth) + GetExpandDetailHeaderWidth();
		}
		internal override void AssignFrom(RowsContainer parentRowsContainer, NodeContainer parentNodeContainer, RowNode rowNode, bool forceUpdate) {
			base.AssignFrom(parentRowsContainer, parentNodeContainer, rowNode, forceUpdate);
			if(MatchKeyCore != null && !MatchKeyCore.RowHandle.Equals(RowHandle))
			   MatchKeyCore = new GroupSummaryRowKey(RowHandle, Level);
		}
		protected double GetExpandDetailHeaderWidth() {
			return TableView.ActualShowDetailButtons ? TableView.ActualExpandDetailHeaderWidth : 0;
		}
		internal override void UpdateLineLevel() {
			int parentHandle = RowHandle.Value;
			bool isExpanded = View.DataProviderBase.IsGroupRowExpanded(parentHandle);
			bool nextItemIsGroupRow = false, prevItemIsGroupRow = false;
			int currentIndex = parentNodeContainer.Items.IndexOf(this.node);
			if(parentNodeContainer == View.RootNodeContainer && currentIndex == parentNodeContainer.Items.Count - 1) {
				FooterPosition = isExpanded ? FooterPosition = Grid.RowPosition.Bottom : Grid.RowPosition.Single;
				return;
			}
			if(currentIndex > -1) {
				if(currentIndex < parentNodeContainer.Items.Count - 1 && !(parentNodeContainer.Items[currentIndex + 1] is GroupSummaryRowNode))
					nextItemIsGroupRow = true;
				if(currentIndex > 0 && parentNodeContainer.Items[currentIndex - 1] is GroupNode)
					prevItemIsGroupRow = true;
			}
			if(!isExpanded || Level == View.DataProviderBase.DataController.GroupInfo.LevelCount - 1)
				FooterPosition = (nextItemIsGroupRow && prevItemIsGroupRow) ? Grid.RowPosition.Single : Grid.RowPosition.Top;
			else if(nextItemIsGroupRow && isExpanded)
				FooterPosition = Grid.RowPosition.Bottom;
			else
				FooterPosition = Grid.RowPosition.Middle;
		}
		protected override bool CanExtractGridGroupSummaryItem(GridSummaryItem summary) {
			return !string.IsNullOrEmpty(summary.ShowInGroupColumnFooter);
		}
		protected override bool IsGroupFooter { get { return true; } }
	}
	public class GroupFooterRowControl : ContentControl {
		public GroupFooterRowControl() {
			this.SetDefaultStyleKey(typeof(GroupFooterRowControl));
		}
		internal FrameworkElement GroupFooterContentPresenter { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			GroupFooterContentPresenter = GetTemplateChild("PART_GroupFooterContentPresenter") as FrameworkElement;
		}
	}
	public class GroupFooterSummaryControl : ContentControl, ISupportLoadingAnimation {
		public static readonly DependencyProperty HeaderWidthProperty;
		public static readonly DependencyProperty LevelProperty;
		public static readonly DependencyProperty IsReadyProperty;
		static GroupFooterSummaryControl() {
			HeaderWidthProperty = DependencyPropertyManager.Register("HeaderWidth", typeof(double), typeof(GroupFooterSummaryControl), new FrameworkPropertyMetadata(0.0, (d, e) => ((GroupFooterSummaryControl)d).UpdateWidth()));
			LevelProperty = DependencyPropertyManager.Register("Level", typeof(int), typeof(GroupFooterSummaryControl), new FrameworkPropertyMetadata(0, (d, e) => ((GroupFooterSummaryControl)d).UpdateWidth()));
			IsReadyProperty = DependencyPropertyManager.Register("IsReady", typeof(bool), typeof(GroupFooterSummaryControl), new PropertyMetadata(true, (d, e) => ((GroupFooterSummaryControl)d).OnIsReadyChanged()));
		}
		protected virtual void OnIsReadyChanged() {
			if(DataContext != null)
				LoadingAnimationHelper.ApplyAnimation();
		}
		public GroupFooterSummaryControl() {
			this.SetDefaultStyleKey(typeof(GroupFooterSummaryControl));
		}
		public double HeaderWidth {
			get { return (double)GetValue(HeaderWidthProperty); }
			set { SetValue(HeaderWidthProperty, value); }
		}
		public int Level {
			get { return (int)GetValue(LevelProperty); }
			set { SetValue(LevelProperty, value); }
		}
		public bool IsReady {
			get { return (bool)GetValue(IsReadyProperty); }
			set { SetValue(IsReadyProperty, value); }
		}
		LoadingAnimationHelper loadingAnimationHelper;
		internal LoadingAnimationHelper LoadingAnimationHelper {
			get {
				if(loadingAnimationHelper == null)
					loadingAnimationHelper = new LoadingAnimationHelper(this);
				return loadingAnimationHelper;
			}
		}
		protected GridGroupSummaryColumnData SummaryData { get { return DataContext as GridGroupSummaryColumnData; } }
		protected GroupSummaryRowData RowData { get { return SummaryData != null ? SummaryData.GroupRowData as GroupSummaryRowData : null; } }
		protected virtual void UpdateWidth() {
			Width = GroupSummaryLayoutCalculator.CalcWidth(SummaryData != null ? SummaryData.Column : null, HeaderWidth, RowData);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetValue(GridPopupMenu.GridMenuTypeProperty, GridMenuType.GroupFooterSummary);
		}
		#region ISupportLoadingAnimation Members
		DataViewBase ISupportLoadingAnimation.DataView {
			get { return SummaryData.View; }
		}
		FrameworkElement ISupportLoadingAnimation.Element {
			get { return this; }
		}
		bool ISupportLoadingAnimation.IsGroupRow {
			get { return true; }
		}
		#endregion
	}
	static class GroupSummaryLayoutCalculator {
		internal static double CalcWidth(ColumnBase column, double headerWidth, GroupRowData rowData) {
			if(column != null && headerWidth > 0 && rowData != null && IsFirstColumn(column, rowData))
				return Math.Max(0, headerWidth - rowData.Offset);
			return headerWidth;
		}
		internal static bool IsFirstColumn(ColumnBase column, RowData rowData) {
			if(column.ParentBand != null) {
				return column.ParentBand.VisibleIndex == 0 && column.BandRow != null && column.BandRow.Columns[0] == column;
			} else {
				IList<ColumnBase> columns = GetOwnedColumns(column, rowData);
				return columns != null && columns.Count > 0 && columns[0] == column;
			}
		}
		static IList<ColumnBase> GetOwnedColumns(ColumnBase column, RowData rowData) {
			IList<ColumnBase> fixedLeftColumns = rowData.VisualDataTreeBuilder.GetFixedLeftColumns();
			if(fixedLeftColumns != null && fixedLeftColumns.Count > 0)
				return fixedLeftColumns;
			return rowData.VisualDataTreeBuilder.GetFixedNoneColumns();
		}
	}
	public class GroupFooterBandsNoneDropPanel : BandsNoneDropPanel {
		internal new GroupSummaryRowData RowData { get { return DataContext as GroupSummaryRowData; } }
		protected internal override double GetColumnWidth(ColumnBase column) {
			double width = base.GetColumnWidth(column);
			if(RowData != null && GroupSummaryLayoutCalculator.IsFirstColumn(column, RowData))
				width -= RowData.Offset;
			return width;
		}
	}
}

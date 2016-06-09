#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class GridDashboardControl : GridControl, IGridControl, IDisposable {
		readonly List<ISupportInnerValuesItem> innerValuesItems = new List<ISupportInnerValuesItem>();
		event EventHandler<CustomColumnDisplayTextEventArgsBase> gridViewCustomColumnDisplayText;
		event EventHandler<CustomGridDisplayTextEventArgs> customGridDisplayText;
		event EventHandler<GridCustomDrawCellEventArgsBase> customDrawCell;
		internal GridDashboardView DashboardView { get { return MainView as GridDashboardView; } }
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new BaseView MainView {
			get { return base.MainView; }
			set {
				base.MainView = value;
				ColumnView columnView = value as ColumnView;
				if(columnView != null) {
					columnView.CustomColumnDisplayText += OnGridViewCustomColumnDisplayText;
					columnView.CustomColumnSort += OnGridViewCustomColumnSort;
				}
			}
		}
		protected override void RegisterAvailableViewsCore(InfoCollection collection) {
			base.RegisterAvailableViewsCore(collection);
			collection.Add(new GridDashboardInfoRegistrator());
		}
		protected override BaseView CreateDefaultView() {
			return CreateView("GridDashboardView");
		}
		void OnGridViewCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e) {
			string displayText = null;
			if(e.ListSourceRowIndex < 0 && DashboardSpecialValuesInternal.TryGetDisplayText(e.Value, out displayText)) {
				e.DisplayText = displayText;
				return;
			}
			if(gridViewCustomColumnDisplayText != null && !(e.Column.ColumnEdit is DashboardGridRepositoryItemMemoEdit))
				gridViewCustomColumnDisplayText(this, new CustomDashboardColumnDisplayTextEventArgs(e));
		}
		void OnGridViewCustomColumnSort(object sender, CustomColumnSortEventArgs e) {
			const int less = -1;
			const int greater = 1;
			object value1 = e.Value1;
			object value2 = e.Value2;
			if(DashboardSpecialValuesInternal.IsDashboardSpecialValue(value1) || DashboardSpecialValuesInternal.IsDashboardSpecialValue(value2)) {
				e.Result = new DashboardValueComparer().Compare(value1, value2);
				e.Handled = true;
				return;
			}
			if(object.ReferenceEquals(null, value2)) {
				if(object.ReferenceEquals(null, value1)) {
					e.Result = 0;
					e.Handled = true;
					return;
				} else {
					e.Result = greater;
					e.Handled = true;
					return;
				}
			} else
				if(object.ReferenceEquals(null, value1)) {
					e.Result = less;
					e.Handled = true;
					return;
				} 
			if (value1.GetType() != value2.GetType()) {
				try {
					double doubleValue1 = Convert.ToDouble(value1);
					double doubleValue2 = Convert.ToDouble(value2);
					e.Result = CompareDefaultValues(doubleValue1, doubleValue2);
					e.Handled = true;
				}
				catch {
				}
			}
			else {
				try {
					IList<double> sparklineValues1 = value1 as IList<double>;
					IList<double> sparklineValues2 = value2 as IList<double>;
					if(sparklineValues1 != null && sparklineValues1.Count > 0 && sparklineValues2 != null && sparklineValues2.Count > 0) {
						e.Result = CompareDefaultValues(sparklineValues1[0], sparklineValues2[0]);
						e.Handled = true;
					}
					else
						e.Result = CompareDefaultValues(value1, value2);
				}
				catch {
					e.Handled = true;
				}
			}
		}
		int CompareDefaultValues(object value1, object value2) {
			return Comparer.Default.Compare(value1, value2);
		}
		bool IGridControl.AllowIncrementalSearch {
			get { return DashboardView.OptionsBehavior.AllowIncrementalSearch; }
			set { DashboardView.OptionsBehavior.AllowIncrementalSearch = value; }
		}
		bool IGridControl.AllowCellMerge {
			get { return DashboardView.OptionsView.AllowCellMerge; }
			set { DashboardView.OptionsView.AllowCellMerge = value; }
		}
		bool IGridControl.EnableAppearanceEvenRow {
			get { return DashboardView.OptionsView.EnableAppearanceEvenRow; }
			set { DashboardView.OptionsView.EnableAppearanceEvenRow = value; }
		}
		bool IGridControl.ShowColumnHeaders {
			get { return DashboardView.OptionsView.ShowColumnHeaders; }
			set { DashboardView.OptionsView.ShowColumnHeaders = value; }
		}
		bool IGridControl.ShowHorizontalLines {
			get { return DashboardView.OptionsView.ShowHorizontalLines == DefaultBoolean.True; }
			set { DashboardView.OptionsView.ShowHorizontalLines = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		bool IGridControl.ShowVerticalLines {
			get { return DashboardView.OptionsView.ShowVerticalLines == DefaultBoolean.True; }
			set { DashboardView.OptionsView.ShowVerticalLines = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		GridColumnWidthMode IGridControl.ColumnWidthMode { 
			get { return DashboardView.ColumnWidthMode; } 
			set { DashboardView.ColumnWidthMode = value; } 
		}
		bool IGridControl.WordWrap { get; set; }
		bool IGridControl.ShowFooter {
			get { return DashboardView.OptionsView.ShowFooter; }
			set { DashboardView.OptionsView.ShowFooter = value; }
		}
		bool IGridControl.AllColumnsFixed {
			get { return DashboardView.AllColumnsFixed; }
			set { DashboardView.AllColumnsFixed = value; }
		}
		IList<IGridColumn> IGridControl.Columns {
			get {
				List<IGridColumn> columns = new List<IGridColumn>();
				foreach(IGridColumn column in DashboardView.DashboardColumns)
					columns.Add(column);
				return columns;
			}
		}
		int IGridControl.ColumnsCount { get { return DashboardView.Columns.Count; } }
		event EventHandler<CustomColumnDisplayTextEventArgsBase> IGridControl.GridViewCustomColumnDisplayText {
			add { gridViewCustomColumnDisplayText += value; }
			remove { gridViewCustomColumnDisplayText -= value; }
		}
		event EventHandler<CustomGridDisplayTextEventArgs> IGridControl.CustomGridDisplayText {
			add { customGridDisplayText += value; }
			remove { customGridDisplayText -= value; }
		}
		event EventHandler<GridCustomDrawCellEventArgsBase> IGridControl.CustomDrawCell {
			add { customDrawCell += value; }
			remove { customDrawCell -= value; }
		}
		void IGridControl.ClearColumns() {
			DashboardView.DashboardColumns.SetDesignModeOn();
			foreach(GridDashboardColumn gridColumn in DashboardView.Columns) {
				UnsubscribeRepositoryItemEvents(gridColumn.ColumnEdit);
			}
			DashboardView.Columns.Clear();
			innerValuesItems.Clear();
			DashboardView.DashboardColumns.SetDesignModeOff();
			RepositoryItems.Clear();
		}
		IGridColumn IGridControl.AddColumn(GridColumnViewModel model) {
			return ((IGridControl)this).AddColumn(model, null, null);
		}
		IGridColumn IGridControl.AddColumn(GridColumnViewModel model, ValueFormatViewModel exportFormat, GridViewerDataController dataController) {
			DashboardView.DashboardColumns.SetDesignModeOn();
			GridDashboardColumn gridColumn = new GridDashboardColumn();
			DashboardView.Columns.Add(gridColumn);
			gridColumn.MinWidth = GridDashboardColumn.DefaultMinWidth;
			gridColumn.AppearanceCell.TextOptions.HAlignment = model.HorzAlignment == GridColumnHorzAlignment.Left ? HorzAlignment.Near : HorzAlignment.Far;
			gridColumn.AppearanceCell.Options.UseTextOptions = true;
			gridColumn.OptionsFilter.AllowFilter = model.DisplayMode != GridColumnDisplayMode.Sparkline;
			if(gridColumn.OptionsColumn.AllowSort != DefaultBoolean.True) 
				gridColumn.OptionsColumn.AllowSort = DefaultBoolean.True;
			gridColumn.SortMode = ColumnSortMode.Custom;
			RepositoryItem newItem = CreateRepositoryItem(model, dataController, gridColumn);
			RepositoryItem oldItem = gridColumn.ColumnEdit;
			if(oldItem != null)
				oldItem.Dispose();
			gridColumn.ColumnEdit = newItem;
			RepositoryItems.Add(newItem);
			foreach(GridColumnTotalViewModel totalViewModel in model.Totals) {
				GridColumnSummaryItem item = gridColumn.Summary.Add(Data.SummaryItemType.Custom);
				string valueString = dataController.TryGetValueByColumnId(totalViewModel.DataId);
				item.DisplayFormat = string.Format(totalViewModel.Caption, valueString);
			}
			DashboardView.DashboardColumns.SetDesignModeOff();
			return gridColumn;
		}
		RepositoryItem CreateRepositoryItem(GridColumnViewModel model, GridViewerDataController dataController, GridDashboardColumn gridColumn) {
			if(model.DisplayMode == GridColumnDisplayMode.Image) {
				DashboardRepositoryItemPictureEdit pictureEdit = new DashboardRepositoryItemPictureEdit(DashboardView, model) { PictureStoreMode = PictureStoreMode.Image };
				pictureEdit.CustomDrawCell += OnCustomDrawCell;
				return pictureEdit;
			}
			if(model.DisplayMode == GridColumnDisplayMode.Sparkline) {
				DashboardSparklineEdit sparklineEdit = new DashboardSparklineEdit(DashboardView, model, gridColumn);
				sparklineEdit.CustomDrawCell += OnCustomDrawCell;
				innerValuesItems.Add((ISupportInnerValuesItem)sparklineEdit);
				return sparklineEdit;
			}
			if(((IGridControl)this).WordWrap && model.DisplayMode == GridColumnDisplayMode.Value) {
				DashboardGridRepositoryItemMemoEdit memoEdit = new DashboardGridRepositoryItemMemoEdit(DashboardView, gridColumn, model, model.DataId);
				memoEdit.CustomDisplayText += OnMemoEditCustomDisplayText;
				memoEdit.CustomDrawCell += OnCustomDrawCell;
				return memoEdit;
			}
			DashboardGridRepositoryItemTextEdit textEdit = new DashboardGridRepositoryItemTextEdit(DashboardView, gridColumn, model, dataController);
			textEdit.CustomDrawCell += OnCustomDrawCell;
			return textEdit;
		}
		void UnsubscribeRepositoryItemEvents(RepositoryItem repositoryItem) {
			IDashboardRepositoryItem dashboardRepositoryItem = repositoryItem as IDashboardRepositoryItem;
			if(dashboardRepositoryItem != null)
				dashboardRepositoryItem.CustomDrawCell -= OnCustomDrawCell;
			DashboardGridRepositoryItemMemoEdit memoEdit = repositoryItem as DashboardGridRepositoryItemMemoEdit;
			if(memoEdit != null)
				memoEdit.CustomDisplayText -= OnMemoEditCustomDisplayText;
		}
		void OnCustomDrawCell(object sender, GridCustomDrawCellEventArgsBase e) {
			if(customDrawCell != null)
				customDrawCell(this, e);
		}
		void OnMemoEditCustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			if(customGridDisplayText != null) {
				CustomGridDisplayTextEventArgs args = new CustomGridDisplayTextEventArgs(e.Value, ((DashboardGridRepositoryItemMemoEdit)sender).ColumnId);
				customGridDisplayText(sender, args);
				e.DisplayText = args.DisplayText;
			}
		}
		void IGridControl.SetData(ReadOnlyTypedList data) {
			if (data != null) {
				if (InvokeRequired)
					Invoke(new Action(() => {
						DataSource = data;
					}));
				else
					DataSource = data;
			}
		}
		void IGridControl.BeginUpdate() {
			BeginInit();
		}
		void IGridControl.EndUpdate() {
			EndInit();
			DashboardView.ResetColumnsWidthOptionsInfo();
			DashboardView.LayoutChanged();
		}
		void IGridControl.ResetClientState() {
			DashboardView.ResetClientState();
		}
		public void ClearInnerValues() {
			foreach(ISupportInnerValuesItem item in innerValuesItems)
				item.ClearInnerValues();
		}
		protected override void Dispose(bool isDisposing) {
			ColumnView columnView = MainView as ColumnView;
			if(columnView != null) {
				columnView.CustomColumnDisplayText -= OnGridViewCustomColumnDisplayText;
				columnView.CustomColumnSort -= OnGridViewCustomColumnSort;
			}
			base.Dispose(isDisposing);
		}
	}
}

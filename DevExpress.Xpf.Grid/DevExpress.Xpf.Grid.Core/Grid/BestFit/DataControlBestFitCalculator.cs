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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
#if SL
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Grid.Native {
	public sealed class DataControlBestFitCalculator : BestFitCalculatorBase {
		#region 
		class RowsBestFitCalculator : BestFitCalculatorBase.RowsBestFitCalculatorBase {
			public RowsBestFitCalculator(BestFitCalculatorBase owner, IEnumerable<int> rows)
				: base(owner, rows) {
			}
			protected new DataControlBestFitCalculator Owner { get { return (DataControlBestFitCalculator)base.Owner; } }
			protected DataViewBase View { get { return Owner.View; } }
			protected override bool IsValidRowHandle(int rowHandle) {
				return Owner.DataProviderBase.IsValidRowHandle(rowHandle) && !Owner.DataProviderBase.IsGroupRowHandle(rowHandle);
			}
			protected override bool IsFocusedCell(IBestFitColumn column, int rowHandle) {
				return View.FocusedRowHandle == rowHandle && View.DataControl.CurrentColumn == column;
			}
			protected override void UpdateBestFitControl(FrameworkElement bestFitControl, IBestFitColumn column, int rowHandle) {
				((BestFitControlBase)bestFitControl).Update(rowHandle);
				((BestFitControlBase)bestFitControl).UpdateIsFocusedCell(IsFocusedCell(column, rowHandle));
			}
		}
		class GroupSummaryBestFitCalculator : BestFitCalculatorBase.RowsBestFitCalculatorBase {
			public GroupSummaryBestFitCalculator(BestFitCalculatorBase owner, IEnumerable<int> rows)
				: base(owner, rows) {
			}
			protected new DataControlBestFitCalculator Owner { get { return (DataControlBestFitCalculator)base.Owner; } }
			protected DataViewBase View { get { return Owner.View; } }
			protected ITableView TableView { get { return View as ITableView; } }
			protected override bool IsValidRowHandle(int scrollIndex) {
				return View.DataProviderBase.GetVisibleIndexByScrollIndex(scrollIndex) is GroupSummaryRowKey;
			}
			protected override void UpdateBestFitControl(FrameworkElement bestFitControl, IBestFitColumn column, int scrollIndex) {
				GroupSummaryRowKey key = (GroupSummaryRowKey)View.DataProviderBase.GetVisibleIndexByScrollIndex(scrollIndex);
				bestFitControl.DataContext = TableView.TableViewBehavior.GetGroupSummaryColumnData(key.RowHandle.Value, column);
			}
		}
		#endregion
		DataViewBase view;
		public DataControlBestFitCalculator(DataViewBase view) {
			this.view = view;
		}
		protected sealed override bool IsServerMode { get { return view.DataControl.IsServerMode; } }
		DataViewBase View { get { return view; } }
		ITableView TableView { get { return (ITableView)View; } }
		DataProviderBase DataProviderBase { get { return View.DataProviderBase; } }
		protected sealed override int VisibleRowCount { 
			get {
				int visibleRowCount = (int)Math.Min(View.PageVisibleDataRowCount, View.RootDataPresenter.ViewPort);
				return visibleRowCount > 10 ? visibleRowCount : 30; 
			} 
		}
#if DEBUGTEST
		public event EventHandler BestFitControlCreated;
#endif
		protected sealed override void SetBestFitElement(FrameworkElement bestFitElement) {
			TableView.TableViewBehavior.BestFitControlDecorator.Child = bestFitElement;
#if DEBUGTEST
			if(BestFitControlCreated != null)
				BestFitControlCreated(this, EventArgs.Empty);
#endif
		}
		protected sealed override int GetRowCount(IBestFitColumn column) {
			return DataProviderBase.DataRowCount;
		}
		protected sealed override object[] GetUniqueValues(IBestFitColumn column) {
			return DataProviderBase.GetUniqueColumnValues((ColumnBase)column, false, true);
		}
		protected sealed override int GetBestFitMaxRowCount(IBestFitColumn column) {
			return column.BestFitMaxRowCount == -1 ? TableView.BestFitMaxRowCount : column.BestFitMaxRowCount;
		}
		protected sealed override BestFitMode GetBestFitMode(IBestFitColumn column) {
			return column.BestFitMode == BestFitMode.Default ? TableView.BestFitMode : column.BestFitMode;
		}
		BestFitArea GetBestFitArea(GridColumnBase column) {
			return column.BestFitArea == BestFitArea.None ? TableView.BestFitArea : column.BestFitArea;
		}
		public sealed override double CalcColumnBestFitWidth(IBestFitColumn column) {
			ColumnBase gridColumn = (ColumnBase)column;
			if(gridColumn.FixedWidth)
				return gridColumn.ActualWidth;
			if(!double.IsNaN(gridColumn.BestFitWidth))
				return gridColumn.BestFitWidth;
			if(View.RootDataPresenter == null)
				return double.NaN;
			try {
				LayoutUpdatedHelper.GlobalLocker.Lock();
				return base.CalcColumnBestFitWidth(column);
			} finally {
				LayoutUpdatedHelper.GlobalLocker.Unlock();
			}
		}
		protected sealed override double CalcColumnBestFitWidthCore(IBestFitColumn column) {
			double result = 0;
			if(View.ShowColumnHeaders && ShouldCalcBestFitArea(column, BestFitArea.Header))
				CalcHeaderBestFit(column, ref result);
			if(ShouldCalcBestFitArea(column, BestFitArea.Rows))
				CalcDataBestFit(column, ref result);
			if(View.ShowGroupSummaryFooter && ShouldCalcBestFitArea(column, BestFitArea.GroupSummary))
				CalcGroupSummaryBestFit(column, ref result);
			if(View.ShowTotalSummary && ShouldCalcBestFitArea(column, BestFitArea.TotalSummary))
				CalcTotalSummaryBestFit(column, ref result);
			return result;
		}
		bool ShouldCalcBestFitArea(IBestFitColumn column, BestFitArea testArea) {
			BestFitArea bestFitArea = GetBestFitArea((GridColumnBase)column);
			return (bestFitArea & testArea) > 0;
		}
		protected sealed override BestFitCalculatorBase.RowsBestFitCalculatorBase CreateBestFitCalculator(IEnumerable<int> rows) {
			return new RowsBestFitCalculator(this, rows);
		}
		void CalcHeaderBestFit(IBestFitColumn column, ref double result) {
			GridColumnHeaderBase header = TableView.TableViewBehavior.CreateGridColumnHeader();
#if !SL
			BaseGridHeader.SetGridColumn(header, (BaseColumn)column);
#endif
			header.DataContext = column;
			header.ColumnPosition = ((ColumnBase)column).ColumnPosition;
			SetBestFitElement(header);
			UpdateBestFitResult(header, ref result, TableView.TableViewBehavior.ViewInfo.GetHeaderIndentsWidth((ColumnBase)column));
		}
		protected sealed override void UpdateBestFitControl(FrameworkElement bestFitControl, IBestFitColumn column, object cellValue) {
			((BestFitControlBase)bestFitControl).UpdateValue(cellValue);
		}
		protected sealed override FrameworkElement CreateBestFitControl(IBestFitColumn column) {
			return TableView.TableViewBehavior.CreateBestFitControl((ColumnBase)column);
		}
		void CalcTotalSummaryBestFit(IBestFitColumn column, ref double result) {
			FrameworkElement summary = TableView.TableViewBehavior.CreateGridTotalSummaryControl();
			summary.DataContext = View.HeadersData.GetCellDataByColumn((ColumnBase)column);
			SetBestFitElement(summary);
			UpdateBestFitResult(summary, ref result);
		}
		void CalcGroupSummaryBestFit(IBestFitColumn column, ref double result) {
			FrameworkElement summary = TableView.TableViewBehavior.CreateGroupFooterSummaryControl();
			SetBestFitElement(summary);
			GetGroupSummaryBestFitDelegate(GetBestFitMode(column), column)(summary, column, ref result);
		}
		CalcBestFitDelegate GetGroupSummaryBestFitDelegate(BestFitMode bestFitMode, IBestFitColumn column) {
			if(bestFitMode == BestFitMode.Default || bestFitMode == BestFitMode.Smart)
				bestFitMode = GetSmartBestFitMode(column);
			if(bestFitMode == BestFitMode.AllRows)
				return new GroupSummaryBestFitCalculator(this, new RowsRange(0, DataProviderBase.VisibleCount + View.CalcGroupSummaryVisibleRowCount())).CalcRowsBestFit;
			return new GroupSummaryBestFitCalculator(this, new RowsRange(View.PageVisibleTopRowIndex, VisibleRowCount)).CalcRowsBestFit;
		}
		protected sealed override RowsRange CalcBestFitRowsRange(int rowCount) {
			int topRow = TableView.TableViewBehavior.GetTopRow(View.PageVisibleTopRowIndex);
			topRow = Math.Min(topRow, DataProviderBase.DataRowCount - rowCount);
			return new RowsRange(topRow, rowCount);
		}
		protected sealed override CalcBestFitDelegate GetSmartModeCalcBestFitDelegate(IBestFitColumn column) {
			CustomBestFitEventArgsBase e = TableView.TableViewBehavior.RaiseCustomBestFit((ColumnBase)column, GetSmartBestFitMode(column));
			if(e.BestFitRows != null)
				return CreateBestFitCalculator(e.BestFitRows).CalcRowsBestFit;
			else
				return GetCalcBestFitDelegate(e.BestFitMode, column);
		}
		protected override void CalcDistinctValuesBestFit(FrameworkElement bestFitControl, IBestFitColumn column, ref double result) {
			((BestFitControlBase)bestFitControl).Update(DataControlBase.InvalidRowHandle);
			base.CalcDistinctValuesBestFit(bestFitControl, column, ref result);
		}
	}
}

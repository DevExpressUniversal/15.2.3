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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Core;
using DevExpress.Data;
using DevExpress.Data.Selection;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid.Printing {
	public class ItemsGenerationSimpleStrategy : ItemsGenerationStrategyBase {
		Dictionary<ColumnBase, string> TotalSummaries { get; set; }
		string FixedLeftSummaryText { get; set; }
		string FixedRightSummaryText { get; set; }
		public override string GetTotalSummaryText(ColumnBase column) {
			if(!TotalSummaries.ContainsKey(column)) return String.Empty;
			return TotalSummaries[column] ?? String.Empty;
		}
		public override string GetFixedTotalSummaryLeftText() {
			return FixedLeftSummaryText;
		}
		public override string GetFixedTotalSummaryRightText() {
			return FixedRightSummaryText;
		}
		void StoreTotalSummary() {
			TotalSummaries = View.DataControl.ColumnsCore.Cast<ColumnBase>().ToDictionary(c => c, c => c.TotalSummaryText);
			FixedLeftSummaryText = View.DataControl.viewCore.GetFixedSummariesLeftString();
			FixedRightSummaryText = View.DataControl.viewCore.GetFixedSummariesRightString();
		}
		public override bool RequireFullExpand { get { return false; } }
		public ItemsGenerationSimpleStrategy(DataViewBase view) : base(view) {
			StoreTotalSummary();
		}
		public override object GetRowValue(RowData rowData) {
			return DataProvider.GetRowByListIndex(rowData.DataRowNode.PrintInfo.ListIndex);
		}
		public override object GetCellValue(RowData rowData, string fieldName) {
			if(TableView.IsCheckBoxSelectorColumn(fieldName))
				return rowData.DataRowNode.PrintInfo.IsSelected;
			return ((GridDataProvider)DataProvider).GetRowValueByListIndex(rowData.DataRowNode.PrintInfo.ListIndex, fieldName);
		}
	}
	public class ItemsGenerationPrintAllGroupsStrategy : ItemsGenerationSimpleStrategy {
		ListSourceRowsKeeper keeper = null;
		public override bool RequireFullExpand {
			get { return true; }
		}
		public ItemsGenerationPrintAllGroupsStrategy(DataViewBase view) : base(view) { }
		protected override void GenerateAllCore() {
			keeper = DataProvider.DataController.CreateControllerRowsKeeper();
			DataProvider.DataController.SaveRowState(keeper);
			DataProvider.ExpandAll();
		}
		protected override void ClearAllCore() {
			DataProvider.CollapseAll();
			DataProvider.DataController.RestoreRowState(keeper);
			keeper.Dispose();
			keeper = null;
		}
	}
	public abstract class ItemsGenerationServerStrategy : ItemsGenerationStrategyBase {
		new GridDataProvider DataProvider { get { return (GridDataProvider)base.DataProvider; } }
		protected internal PrintSelectedRowsInfo SelectedRowsInfo = null;
		protected virtual IList GetAllFilteredAndSortedRows() {
			return ((TableView)View).PrintSelectedRowsOnly ? PrintSelectedRowsHelper.GetSelectedRows(DataProvider, View, out SelectedRowsInfo, FetchAllFilteredAndSortedRows()) : FetchAllFilteredAndSortedRows();
		}
		protected abstract IList FetchAllFilteredAndSortedRows();
		public ItemsGenerationServerStrategy(DataViewBase view) : base(view) { }
		public override bool RequireFullExpand { get { return true; } }
		GridServerModeDataControllerPrintInfo Info { get; set; }
		BaseGridController oldController = null;
		protected override void GenerateAllCore() {
			oldController = DataProvider.DataController;
			View.DataControl.dataResetLocker.DoLockedAction(() => {
				Info = DataProvider.SubstituteDataControllerForPrinting(GetAllFilteredAndSortedRows(), View.PrintAllGroupsCore);
			});
		}
		public override string GetTotalSummaryText(ColumnBase column) {
			if(Info == null || !Info.Summaries.ContainsKey(column)) return String.Empty;
			return Info.Summaries[column] ?? String.Empty;
		}
		public override string GetFixedTotalSummaryLeftText() {
			return Info != null ? Info.FixedLeftSummaryText : null;
		}
		public override string GetFixedTotalSummaryRightText() {
			return Info != null ? Info.FixedRightSummaryText : null;
		}
		protected override void ClearAllCore() {
			View.DataControl.dataResetLocker.DoLockedAction(() => {
				DataProvider.SetDataController(oldController);
				View.UpdateDataObjects();
			});
		}
		public override object GetRowValue(RowData rowData) {
			if(Info == null) return null;
			return GridDataProvider.GetRowByListIndex(Info.Controller, rowData.DataRowNode.PrintInfo.ListIndex);
		}
		public override object GetCellValue(RowData rowData, string fieldName) {
			if(Info == null) return null;
			return GridDataProvider.GetRowValueByListIndex(Info.Controller, rowData.DataRowNode.PrintInfo.ListIndex, fieldName);
		}
		public override void Clear() {
			base.Clear();
			if(Info != null && Info.Controller != null) {
				DataProvider.ClearPrintingControllerEvents(Info.Controller);
				Info.Controller.Dispose();
				Info = null;
			}
		}
	}
	public class ItemsGenerationServerModeStrategy : ItemsGenerationServerStrategy {
		protected override IList FetchAllFilteredAndSortedRows() {
			DevExpress.Data.ServerModeDataController serverDataController = (DevExpress.Data.ServerModeDataController)DataProvider.DataController;
			return serverDataController.GetAllFilteredAndSortedRows();
		}
		public ItemsGenerationServerModeStrategy(DataViewBase view) : base(view) { }
	}
	public class ItemsGenerationAsyncServerModeStrategy : ItemsGenerationServerStrategy {
		protected override IList FetchAllFilteredAndSortedRows() {			
			DevExpress.Data.AsyncServerModeDataController asyncDataController = (DevExpress.Data.AsyncServerModeDataController)DataProvider.DataController;
			DevExpress.Data.Async.CommandGetAllFilteredAndSortedRows commandGetRows = asyncDataController.Server.GetAllFilteredAndSortedRows();
#if !SL
			string progressWindowTitle = View.GetLocalizedString(GridControlStringId.ProgressWindowTitle);
			string cancelButtonCaption = View.GetLocalizedString(GridControlStringId.ProgressWindowCancel);
			using(System.Threading.ManualResetEvent stopEvent = new System.Threading.ManualResetEvent(false)) {
				System.Threading.Thread progressThread = new System.Threading.Thread(delegate() {
					DXWindow progressWindow = ProgressControl.CreateProgressWindow(stopEvent, true, progressWindowTitle, cancelButtonCaption);
					progressWindow.Show();
					System.Windows.Threading.Dispatcher.Run();
				});
				progressThread.SetApartmentState(System.Threading.ApartmentState.STA);
				progressThread.Start();
				while(!asyncDataController.Server.WaitFor(commandGetRows)) {
					if(progressThread.Join(100)) {
						asyncDataController.Server.Cancel(commandGetRows);
						break;
					}
				}
				stopEvent.Set();
				progressThread.Join();
			}
			return commandGetRows.IsCanceled ? new List<object>() : commandGetRows.Rows;
#else
			while(!asyncDataController.Server.WaitFor(commandGetRows)) {
				System.Threading.Thread.Sleep(100);
			}
			return commandGetRows.Rows;
#endif
		}
		public ItemsGenerationAsyncServerModeStrategy(DataViewBase view) : base(view) { }
	}
	public class ItemsGenerationAsyncServerModeStrategyAsync : ItemsGenerationServerStrategy {
		IList allFilteredAndSortedRows = new List<object>();
		System.Windows.Threading.DispatcherTimer fetchingTimer;
		protected override IList FetchAllFilteredAndSortedRows() {
			return allFilteredAndSortedRows;
		}
		public ItemsGenerationAsyncServerModeStrategyAsync(DataViewBase view) : base(view) { }
		public void StartFetchingAllFilteredAndSortedRows(Action createPrintingNodeAction) {
			System.Windows.Threading.Dispatcher uiDispatcher = View.Dispatcher;
			DevExpress.Data.AsyncServerModeDataController asyncDataController = (DevExpress.Data.AsyncServerModeDataController)DataProvider.DataController;
			DevExpress.Data.Async.CommandGetAllFilteredAndSortedRows commandGetRows = asyncDataController.Server.GetAllFilteredAndSortedRows();
			string progressWindowTitle = View.GetLocalizedString(GridControlStringId.ProgressWindowTitle);
			string cancelButtonCaption = View.GetLocalizedString(GridControlStringId.ProgressWindowCancel);
			DXWindow progressWindow = ProgressControl.CreateProgressWindow(null, false, progressWindowTitle, cancelButtonCaption);
			progressWindow.Closed += (s, e) => { asyncDataController.Server.Cancel(commandGetRows); };
			progressWindow.Show();
			fetchingTimer = new System.Windows.Threading.DispatcherTimer();
			fetchingTimer.Interval = TimeSpan.FromMilliseconds(100.0);
			fetchingTimer.Tick += (s, e) => {
				if(!fetchingTimer.IsEnabled || !asyncDataController.Server.WaitFor(commandGetRows)) return;
				fetchingTimer.Stop();
				allFilteredAndSortedRows = commandGetRows.IsCanceled ? new List<object>() : commandGetRows.Rows;
				progressWindow.Close();
				uiDispatcher.BeginInvoke(createPrintingNodeAction);
			};
			fetchingTimer.Start();
		}
	}
	public class ItemsGenerationSelectedRowsStrategy : ItemsGenerationServerStrategy {
		public ItemsGenerationSelectedRowsStrategy(DataViewBase view) : base(view) { }
		public override bool RequireFullExpand { get { return true; } }
		protected override IList FetchAllFilteredAndSortedRows() {
			return PrintSelectedRowsHelper.GetSelectedRows(DataProvider, View, out SelectedRowsInfo);
		}
		protected override IList GetAllFilteredAndSortedRows() {
			return FetchAllFilteredAndSortedRows();
		}
	}
	public class PrintSelectedRowsInfo {
		public Dictionary<int, int> OriginalRowHandles { get; private set; }
		public PrintSelectedRowsInfo() {
			OriginalRowHandles = new Dictionary<int, int>();
		}
	}
#if DEBUGTEST
	public
#else
	internal 
#endif
	class PrintSelectedRowsHelper {
		DataProviderBase DataProvider;
		DataViewBase View;
		DataControlBase RootDataControl { get { return View.DataControl; } }
		Dictionary<int, object> results = new Dictionary<int, object>();
		IList allRows;
		PrintSelectedRowsInfo PrintSelectedRowsInfo;
		PrintSelectedRowsHelper(DataProviderBase dataProvider, DataViewBase view, IList allRows) {
			DataProvider = dataProvider;
			View = view;
			this.allRows = GetAllRowsAsList(allRows);
			PrintSelectedRowsInfo = new PrintSelectedRowsInfo();
		}
		IList GetAllRowsAsList(IList allRows) {
			if(allRows == null)
				return null;
			if(!allRows.IsFixedSize)
				return allRows;
			IList result = new List<object>();
			foreach(object el in allRows) result.Add(el);
			return result;
		}
		public static IList GetSelectedRows(DataProviderBase dataProvider, DataViewBase view, out PrintSelectedRowsInfo printSelectedRowsInfo, IList allRows = null) {
			return new PrintSelectedRowsHelper(dataProvider, view, allRows).GetRows(out printSelectedRowsInfo);
		}
		IList GetRows(out PrintSelectedRowsInfo printSelectedRowsInfo) {
			printSelectedRowsInfo = PrintSelectedRowsInfo;
			if(!View.IsMultiSelection) {
				AddRow(View.FocusedRowHandle);
				return results.Values.ToList();
			}
			foreach(int rowHandle in GetMasterDetailSelectedRows(View.DataControl))
				AddRow(rowHandle);
			if(allRows != null)
				return GetServerModeRows();
			return results.Values.ToList();
		}
		int[] GetMasterDetailSelectedRows(DataControlBase dataControl) {
			DataProviderBase dataProvider = dataControl.DataProviderBase;
			List<int> tempSelectedItems = GetMasterRows(dataControl).ToList();
			int[] actualSelectedRows = dataProvider.DataController.Selection.GetNormalizedSelectedRowsEx();
			for(int i = 0; i < actualSelectedRows.Length; i++) {
				if(tempSelectedItems.Contains(actualSelectedRows[i])) tempSelectedItems.Remove(actualSelectedRows[i]);
			}
			dataProvider.DataController.Selection.BeginSelection();
			foreach(int rowHandle in tempSelectedItems)
				dataProvider.DataController.Selection.SetSelected(rowHandle, true);
			int[] rows = dataProvider.DataController.Selection.GetNormalizedSelectedRowsEx();
			foreach(int rowHandle in tempSelectedItems)
				dataProvider.DataController.Selection.SetSelected(rowHandle, false);
			dataProvider.DataController.Selection.CancelSelection();
			return rows;
		}
		int[] GetMasterRows(DataControlBase dataControl) {
			List<int> result = new List<int>();
			dataControl.UpdateAllDetailDataControls(dc => UpadteDetailContainsSelectedElements(dataControl, dc, ref result), dc => UpadteDetailContainsSelectedElements(dataControl, dc, ref result));
			return result.Distinct().ToArray();
		}
#if DEBUGTEST
		public static int debug_UpadteDetailContainsSelectedElementsFireCount = 0;
#endif
		void UpadteDetailContainsSelectedElements(DataControlBase masterGrid, DataControlBase detailGrid, ref List<int> result) {
#if DEBUGTEST
			debug_UpadteDetailContainsSelectedElementsFireCount++;
#endif
			if(detailGrid.GetMasterGridCore() != masterGrid)
				return;
			if(GetMasterDetailSelectedRows(detailGrid).Length == 0)
				return;
			int masterRowHandle = ((GridControl)detailGrid).GetMasterRowHandle();
			result.Add(masterRowHandle);
		}
		IList GetServerModeRows() {
			allRows.Clear();
			PrintSelectedRowsInfo.OriginalRowHandles.Clear();
			foreach(var kvp in results) {
				PrintSelectedRowsInfo.OriginalRowHandles.Add(allRows.Count, kvp.Key);
				allRows.Add(kvp.Value);
			}
			return allRows;
		}
		void AddRow(int rowHandle) {
			if(!View.DataControl.IsValidRowHandleCore(rowHandle))
				return;
			if(!View.DataControl.IsGroupRowHandleCore(rowHandle)) {
				object row = allRows == null || rowHandle > allRows.Count - 1 ? DataProvider.DataController.GetListSourceRow(rowHandle) : allRows[rowHandle];
				AddRowToPrintList(rowHandle, row);
				return;
			}
			AddGroupRowData(rowHandle);
		}
		void AddRowToPrintList(int rowHandle, object row) {
			if(!results.ContainsKey(rowHandle)) {
				PrintSelectedRowsInfo.OriginalRowHandles.Add(results.Count, rowHandle);
				results.Add(rowHandle, row);
			}
		}
		void AddGroupRowData(int rowHandle) {
			BaseGridController controller = DataProvider.DataController;
			GroupRowInfo groupRowInfo = controller.GroupInfo.GetGroupRowInfoByHandle(rowHandle);
			int rowChildrenCount = controller.GroupInfo.GetChildCount(groupRowInfo);
			for(int i = 0; i < rowChildrenCount; i++) {
				int childRowHandle = controller.GroupInfo.GetChildRow(groupRowInfo, i);
				if(View.DataControl.IsGroupRowHandleCore(childRowHandle)) {
					AddGroupRowData(childRowHandle);
					continue;
				}
				object row = allRows == null || childRowHandle > allRows.Count - 1 ? controller.GetListSourceRow(childRowHandle) : allRows[childRowHandle];
				if(row == null)
					continue;
				AddRowToPrintList(childRowHandle, row);
			}
		}
	}
}

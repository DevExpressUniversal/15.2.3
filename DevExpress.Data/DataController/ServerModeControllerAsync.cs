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
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.Async;
using DevExpress.Data.Async.Helpers;
using DevExpress.Utils;
using System.Linq;
using System.Threading;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Collections;
#if !SL
using DevExpress.Data.Details;
using System.Windows.Forms;
#else
using DevExpress.Xpf.Collections;
using DictionaryEntry = System.Collections.Generic.KeyValuePair<object, object>;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Data {
	public class NotLoadedObject : object {
		public override string ToString() {
			return string.Empty;
		}
		public override int GetHashCode() { return 0; }
		public override bool Equals(object obj) {
			if(obj is NotLoadedObject) return true;
			return false;
		}
	}
	public class UnboundCriteriaInliner: IClientCriteriaVisitor<CriteriaOperator> {
		CriteriaOperator topMostCriteria;
		List<OperandProperty> recursionWatch;
		List<CriteriaOperator> expands;
		DataColumnInfoCollection columns;
		public UnboundCriteriaInliner(DataColumnInfoCollection columns) {
			this.columns = columns;
		}
		CriteriaOperator GetUnboundCriteria(OperandProperty theOperand) {
			DataColumnInfo column = columns[theOperand.PropertyName];
			if(column == null || !column.UnboundWithExpression) return null;
			return CriteriaOperator.TryParse(column.UnboundExpression);
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(AggregateOperand theOperand) {
			return null;	
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(OperandProperty theOperand) {
			CriteriaOperator substExpression = GetUnboundCriteria(theOperand);
			if(ReferenceEquals(null, substExpression))
				return null;
			if(recursionWatch == null) {
				recursionWatch = new List<OperandProperty>();
				expands = new List<CriteriaOperator>();
			}
			bool recurrent = recursionWatch.IndexOf(theOperand) >= 0;
			recursionWatch.Add(theOperand);
			expands.Add(substExpression);
			if(recurrent) {
				string path = string.Empty;
				for(int i = 0; i < recursionWatch.Count; ++i) {
					if(!string.IsNullOrEmpty(path)) path += ", ";
					path += string.Format("\"{0}\"(\"{1}\")", recursionWatch[i], expands[i]);
				}
				throw new InvalidOperationException(string.Format("Unbound expressions recursion detected for criterion \"{0}\". Recursive path is: {1}", topMostCriteria, path));
			}
			CriteriaOperator processedSubst = Process(substExpression);
			recursionWatch.RemoveAt(recursionWatch.Count - 1);
			expands.RemoveAt(expands.Count - 1);
			return processedSubst ?? substExpression;
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(JoinOperand theOperand) {
			return null;	
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BetweenOperator theOperator) {
			CriteriaOperator pt = Process(theOperator.TestExpression);
			CriteriaOperator pb = Process(theOperator.BeginExpression);
			CriteriaOperator pe = Process(theOperator.EndExpression);
			if(ReferenceEquals(null, pt) && ReferenceEquals(null, pb) && ReferenceEquals(null, pe))
				return null;
			else
				return new BetweenOperator(pt ?? theOperator.TestExpression, pb ?? theOperator.BeginExpression, pe ?? theOperator.EndExpression);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BinaryOperator theOperator) {
			CriteriaOperator pl = Process(theOperator.LeftOperand);
			CriteriaOperator pr = Process(theOperator.RightOperand);
			if(ReferenceEquals(null, pl) && ReferenceEquals(null, pr))
				return null;
			else
				return new BinaryOperator(pl ?? theOperator.LeftOperand, pr ?? theOperator.RightOperand, theOperator.OperatorType);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(UnaryOperator theOperator) {
			CriteriaOperator processed = Process(theOperator.Operand);
			if(ReferenceEquals(null, processed))
				return null;
			else
				return new UnaryOperator(theOperator.OperatorType, processed);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(InOperator theOperator) {
			CriteriaOperator pl = Process(theOperator.LeftOperand);
			ICollection<CriteriaOperator> pops = Process(theOperator.Operands);
			if(ReferenceEquals(null, pl) && pops == null)
				return null;
			else
				return new InOperator(pl ?? theOperator.LeftOperand, pops ?? theOperator.Operands);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(GroupOperator theOperator) {
			CriteriaOperator[] processed = Process(theOperator.Operands);
			if(processed == null)
				return null;
			else
				return new GroupOperator(theOperator.OperatorType, processed);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(OperandValue theOperand) {
			return null;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(FunctionOperator theOperator) {
			CriteriaOperator[] processed = Process(theOperator.Operands);
			if(processed == null)
				return null;
			else
				return new FunctionOperator(theOperator.OperatorType, processed);
		}
		CriteriaOperator Process(CriteriaOperator op) {
			if(ReferenceEquals(null, op))
				return null;
			return op.Accept(this);
		}
		CriteriaOperator[] Process(IEnumerable<CriteriaOperator> ops) {
			List<CriteriaOperator> rv = new List<CriteriaOperator>();
			bool changed = false;
			foreach(CriteriaOperator op in ops) {
				CriteriaOperator processed = Process(op);
				if(ReferenceEquals(null, processed)) {
					rv.Add(op);
				} else {
					rv.Add(processed);
					changed = true;
				}
			}
			if(changed)
				return rv.ToArray();
			else
				return null;
		}
		public static CriteriaOperator Process(CriteriaOperator op, DataColumnInfoCollection columns) {
			if(ReferenceEquals(null, op))
				return null;
			UnboundCriteriaInliner inst = new UnboundCriteriaInliner(columns);
			inst.topMostCriteria = op;
			CriteriaOperator processed = inst.Process(op);
			return processed ?? op;
		}
	}
	public class AsyncServerModeDataController: ServerModeDataControllerBase {
		public static readonly object NoValue = new NotLoadedObject();
		public static bool IsNoValue(object value) {
			return object.ReferenceEquals(value, NoValue);
		}
		public AsyncServerModeDataController() {
		}
		public override void Dispose() {
			this.currentControllerRowObjectEx = null;
			DisposeWrapper();
			base.Dispose();
		}
		public new AsyncServerModeGroupRowInfoCollection GroupInfo { get { return base.GroupInfo as AsyncServerModeGroupRowInfoCollection; } }
		protected new AsyncServerModeListSourceRowsKeeper RowsKeeper { get { return base.RowsKeeper as AsyncServerModeListSourceRowsKeeper; } }
		protected override BaseDataControllerHelper CreateHelper() {
			if(ListSource == null) return new BaseDataControllerHelper(this);
			return new AsyncListDataControllerHelper(this);
		}
		protected override VisibleIndexCollection CreateVisibleIndexCollection() { return new AsyncServerModeDataControllerVisibleIndexCollection(this); }
		protected override GroupRowInfoCollection CreateGroupRowInfoCollection() { return new AsyncServerModeGroupRowInfoCollection(this); }
		public override bool AllowNew { get { return true; } }
		protected override void SetListSourceCore(IList value) {
			DisposeWrapper();
			base.SetListSourceCore(value);
		}
		protected void DisposeWrapper() {
			if(Wrapper == null) return;
			Wrapper.Dispose();
		}
		public override bool IsReady { get { return Server != null; } }
		public override ListSourceRowsKeeper CreateControllerRowsKeeper() { return new AsyncServerModeListSourceRowsKeeper(this, CreateSelectionKeeper()); }
		protected override SelectedRowsKeeper CreateSelectionKeeper() { return new AsyncServerModeCurrentAndSelectedRowsKeeper(this); }
		protected override FilterHelper CreateFilterHelper() { return new AsyncServerModeDataControllerFilterHelper(this); }
		protected override IList GetListSource() {
			if(DataSource == null) return null;
			object source = DataSource;
			IListSource ls = source as IListSource;
			if(ls != null) source = ls.GetList();
			if(source is IAsyncListServer && source is ITypedList) {
				return new AsyncListWrapper(this, (IAsyncListServer)source);
			}
			return source as IList;
		}
		protected internal AsyncListWrapper Wrapper { get { return ListSource as AsyncListWrapper; } }
		public IAsyncListServer Server {
			get {
				if(Wrapper != null) return Wrapper.Server;
				return null;
			}
		}
		ListSortDescriptionCollection GetSortCollection() {
			if(!IsSorted) return null;
			ListSortDescription[] desc = new ListSortDescription[SortInfo.Count];
			for(int n = 0; n < SortInfo.Count; n++) {
				DataColumnSortInfo item = SortInfo[n];
				desc[n] = new ListSortDescription(item.ColumnInfo.PropertyDescriptor,
					item.SortOrder == ColumnSortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
			}
			return new ListSortDescriptionCollection(desc);
		}
		internal object GetLoadedRowKey(int controllerRow) { return GetLoadedRowKey(controllerRow, false); }
		internal object GetLoadedRowKey(int controllerRow, bool allowGroupRow) {
			if(IsGroupRowHandle(controllerRow)) {
				if(allowGroupRow) {
					var res = GroupInfo.GetGroupRowInfoByHandle(controllerRow) as ServerModeGroupRowInfo;
					if(res != null) return res.ListGroupInfo;
				}
				return null;
			}
			return Wrapper.GetLoadedRowKey(GetListSourceRowIndex(controllerRow));
		}
		protected override void CheckCurrentControllerRowObjectOnRefresh() { }
		public override int FindRowByValue(string columnName, object value, params OperationCompleted[] completed) {
			if(!IsReady) return InvalidRow;
			DataColumnInfo colInfo = Columns[columnName];
			if(colInfo == null) return InvalidRow;
			if(colInfo.Unbound && !colInfo.UnboundWithExpression) return base.FindRowByValue(columnName, value);
			int row = Wrapper.FindRowByValue(colInfo, value);
			if(row >= 0) return row;
			Server.LocateByValue(ServerModeDataControllerBase.DescriptorToCriteria(colInfo), value, -1, false, AsyncOperationCompletedHelper.GetCommandParameter(completed));
			return OperationInProgress;
		}
		protected override object GetCurrentControllerRowObject() {
			return GetLoadedRowKey(CurrentControllerRow);
		}
		public override int FindRowByRowValue(object value) {
			return InvalidRow;
		}
		public override int FindRowByBeginWith(string columnName, string text) {
			return InvalidRow;
		}
		public override int FindIncremental(string text, int columnHandle, int startRowHandle, bool down, bool ignoreStartRow, bool allowLoop, CompareIncrementalValue compareValue, params OperationCompleted[] completed) {
			if(!IsReady) return InvalidRow;
			DataColumnInfo colInfo = Columns[columnHandle];
			if(colInfo == null || colInfo.Unbound) return InvalidRow;
			if(completed == null || completed[0] == null) return InvalidRow;
			if(ignoreStartRow) {
				if(startRowHandle < 0) startRowHandle = 0;
				startRowHandle += (down ? 1 : -1);
				if(startRowHandle >= VisibleListSourceRowCount) startRowHandle = VisibleListSourceRowCount - 1;
				if(startRowHandle < 0) startRowHandle = Math.Max(0, VisibleListSourceRowCount - 1);
			}
			Wrapper.FindIncremental(ServerModeDataControllerBase.DescriptorToCriteria(colInfo), text, GetListSourceRowIndex(startRowHandle), !down, ignoreStartRow, allowLoop, completed[0]);
			return OperationInProgress;
		}
		protected override void DoSortRows() {
			if(!IsReady) return;
			Wrapper.ApplySort();
		}
		object currentControllerRowObjectEx;
		protected override void OnCurrentControllerRowObjectChanging(object oldObject, object newObject, int level) {
			base.OnCurrentControllerRowObjectChanging(oldObject, newObject, level);
			this.currentControllerRowObjectEx = null;
			if(newObject != null) {
				int additionalRow = GetAdditionalCurrentRow();
				if(additionalRow == InvalidRow) return;
				currentControllerRowObjectEx = GetLoadedRowKey(additionalRow, true);
				if(currentControllerRowObjectEx == null) {
					GetRow(additionalRow, (e) => {
						if(currentControllerRowObjectEx == null) currentControllerRowObjectEx = GetLoadedRowKey(additionalRow);
					});
				}
			}
		}
		int GetAdditionalCurrentRow() {
			if(!IsGrouped) {
				return IsValidControllerRowHandle(CurrentControllerRow + 1) ? CurrentControllerRow + 1 : CurrentControllerRow - 1;
			}
			GroupRowInfo group = GroupInfo.GetGroupRowInfoByControllerRowHandle(CurrentControllerRow);
			if(group == null) return InvalidRow;
			if(CurrentControllerRow + 1 < group.ChildControllerRow + group.ChildControllerRowCount) {
				return CurrentControllerRow + 1;
			}
			if(group.ChildControllerRowCount > 1) return CurrentControllerRow - 1;
			GroupRowInfo root = group.RootGroup;
			if(root != null) {
				int i = GroupInfo.RootGroups.IndexOf(root);
				if(i > 0) return GroupInfo.RootGroups[i - 1].Handle;
			}
			return InvalidRow;
		}
		internal object CurrentControllerRowObjectEx { get { return currentControllerRowObjectEx; } }
		protected override void OnPostRefresh(bool useRowsKeeper) {
			RowsKeeper.Restore();
			if(RowsKeeper.GroupHashEx.AllExpanded && RowsKeeper.AllowRestoreGrouping) {
				this.requireExpandAll = true;
			}
			OnRefreshed();
			OnPostRefreshUpdateSelection();
		}
		public override void LoadRows(int startFrom, int count) {
			if(!IsReady) return;
			for(int n = 0; n < count; n++) {
				GetRow(n + startFrom);
			}
		}
		public override void ClearInvalidRowsCache() {
			if(Wrapper != null) Wrapper.ClearInvalidRowsCache();
		}
		public override void CancelWeakFindIncremental() {
			if(Server != null) Server.WeakCancel<CommandFindIncremental>();
		}
		public override void CancelFindIncremental() {
			if(Server != null) Server.Cancel<CommandFindIncremental>();
		}
		public override void ScrollingCheckRowLoaded(int rowHandle) {
			if(IsGroupRowHandle(rowHandle)) return;
			GetRow(rowHandle);
		}
		public override void ScrollingCancelAllGetRows() {
			if(!IsReady) return;
			Wrapper.CancelAllGetRows();
		}
		protected void BaseClearVisibleInfo() {
			base.ClearVisibleInfoOnRefresh();
		}
		protected override void ClearVisibleInfoOnRefresh() {
			if(!IsGrouped) GroupInfo.Clear();
		}
		int groupedColumnCount = 0;
		protected override void DoGroupRows() {
			this.lastGroupedColumnCount = groupedColumnCount;
			this.groupedColumnCount = GroupedColumnCount;
			if(!IsGrouped || Wrapper == null) return;
			GetServerGroupInfo(null, AutoExpandAllGroups);
		}
		protected override void ChangeExpandedLevel(int groupLevel, bool expanded, bool recursive) {
			throw new NotImplementedException("ServerMode doesn't support Expand/Collapse group levels");
		}
		protected override void ChangeExpanded(int groupRowHandle, bool expanded, bool recursive) {
			ServerModeGroupRowInfo sgroup = (ServerModeGroupRowInfo)GroupInfo.GetGroupRowInfoByHandle(groupRowHandle);
			if(sgroup == null) return;
			if(sgroup.ListGroupInfo == null || (expanded && !sgroup.ChildrenReady)) RequestChildren(sgroup, recursive);
			base.ChangeExpanded(groupRowHandle, expanded, recursive);
		}
		protected override void Reset() {
			this.currentControllerRowObjectEx = null;
			base.Reset();
		}
		protected override void ChangeAllExpanded(bool expanded) {
			this.requireExpandAll = false;
			if(!IsGrouped) return;
			ResetRowsKeeperEx();
			if(expanded) {
				if(CheckIsAllGroupsReady()) {
					base.ChangeAllExpanded(true);
					return;
				}
				DoExpandAll();
			}
			else {
				this.requireExpandAll = false;
				base.ChangeAllExpanded(false);
			}
		}
		void DoExpandAll() {
			if(this.requireExpandAll) return;
			this.requireExpandAll = true;
			for(int n = 0; n < GroupInfo.RootGroups.Count; n++) {
				ChangeExpanded(GroupInfo.RootGroups[n].Handle, true, true);
			}
			for(int n = 0; n < GroupInfo.Count; n++) {
				GroupRowInfo group = GroupInfo[n];
				if(group.Level > 0) {
					ChangeExpanded(group.Handle, true, false);
				}
			}
		}
		protected bool CheckIsAllGroupsReady() {
			if(GroupInfo.Count == 0) return false;
			foreach(ServerModeGroupRowInfo group in GroupInfo) {
				if(!group.ChildrenReady) return false;
			}
			return true;
		}
		public override void EnsureRowLoaded(int controllerRow, OperationCompleted completed) {
			if(completed == null) {
				LoadRow(controllerRow);
				return;
			}
			if(Wrapper == null || !IsValidControllerRowHandle(controllerRow)) {
				completed(null);
				return;
			}
			if(IsRowLoaded(controllerRow)) {
				completed(GetRow(controllerRow));
				return;
			}
			GetRow(controllerRow, completed);
		}
		public override bool IsRowLoaded(int controllerRow) {
			if(controllerRow == InvalidRow || controllerRow == OperationInProgress) return false;
			if(controllerRow < 0) return true;
			if(Wrapper == null) return false;
			return Wrapper.IsRowLoaded(GetListSourceRowIndex(controllerRow));
		}
		protected internal override void RestoreGroupExpanded(GroupRowInfo group) {
			ChangeExpanded(group.Handle, true, false);
		}
		protected internal override void MakeGroupRowVisible(GroupRowInfo group) {
		}
		protected override void CheckRaiseVisibleCountChanged(int prevVCount) {
		}
		protected override void DoRefresh(bool useRowsKeeper) {
			this.requireExpandAll = false;
			base.DoRefresh(useRowsKeeper);
		}
		protected override void CalcTotalSummary() { 
			if(IsUpdateLocked) return;
			TotalSummary.IsDirty = false;
			foreach(SummaryItem item in TotalSummary) item.SummaryValue = null;
			if(TotalSummary.ActiveCount == 0) return;
			Server.GetTotals();
		}
		public override bool PrefetchAllData(Function<bool> callBackMethod) {
#if SILVERLIGHT
			return false;
#else
			if(Server == null) return false;
			int counter = 0;
			int loadCycle = 0;
			while(true) {
				ServerModeGroupRowInfo[] sgroups = GroupInfo.Select(q => ((ServerModeGroupRowInfo)q)).Where(q => !q.ChildrenReady).ToArray();
				ListSourceGroupInfo[] groups = sgroups.Select(q => q.ListGroupInfo).ToArray();
				if(groups.Length == 0) {
					groups = null;
					if(loadCycle > 0) break;
				}
				CommandPrefetchRows allRows = Server.PrefetchRows(groups);
				while(!Server.WaitFor(allRows)) {
					Thread.Sleep(10);
					if(callBackMethod != null) {
						if(callBackMethod()) {
							Server.Cancel<CommandPrefetchRows>();
							return false;
						}
					}
				}
				if(allRows.Successful) {
					loadCycle++;
					PreloadGroups(sgroups);
					if(groups == null) break;
					counter = 0;
					continue;
				}
				if(++counter > 3) return false;
			}
			return PreloadDataRows(callBackMethod);
#endif
		}
		bool PreloadDataRows(Function<bool> callBackMethod) {
#if SILVERLIGHT
			return false;
#else
			int count = Wrapper.Count;
			CommandGetRow lastRow = null;
			for(int n = 0; n < count; n++) {
				lastRow = Server.GetRow(n);
			}
			if(lastRow != null) {
				while(!Server.WaitFor(lastRow)) {
					Thread.Sleep(10);
					if(callBackMethod != null) {
						if(callBackMethod()) {
							Server.Cancel<CommandGetRow>();
							return false;
						}
					}
				}
			}
			return true;
#endif
		}
		void PreloadGroups(ServerModeGroupRowInfo[] sgroups) {
			if(sgroups == null || sgroups.Length == 0)  return;
			List<CommandGetGroupInfo> getGroups = new List<CommandGetGroupInfo>();
			foreach(var sg in sgroups) {
				var command = new CommandGetGroupInfo(sg.ListGroupInfo,
					new DictionaryEntry(Tag_ExpandChildren_bool, false), new DictionaryEntry(Tag_PGroup_ServerModeGroupRowInfo, sg));
				var children = (sg.ListGroupInfo as DevExpress.Data.Helpers.ServerModeCache.ServerModeGroupInfo).ChildrenGroups;
				if(children == null) continue;
				command.ChildrenGroups = new List<ListSourceGroupInfo>();
				command.ChildrenGroups.AddRange(children);
				getGroups.Add(command);
			}
			OnAsyncGroupInfoReceived(getGroups);
		}
		public override IList GetAllFilteredAndSortedRows(Function<bool> callBackMethod) {
#if SILVERLIGHT
			return new List<object>();
#else
			if(Server == null) return new List<object>();
			CommandGetAllFilteredAndSortedRows allRows = Server.GetAllFilteredAndSortedRows();
			while(!Server.WaitFor(allRows)) {
				Thread.Sleep(10);
				if(callBackMethod != null) {
					if(callBackMethod()) {
						Server.Cancel(allRows);
						return null;
					}
				}
			}
			return allRows.Rows;
#endif
		}
		protected override GroupRowInfo RequestSummary(GroupRowInfo group) {
			ServerModeGroupRowInfo sgroup = group as ServerModeGroupRowInfo;
			if(sgroup == null) return sgroup;
			if(sgroup.ListGroupInfo == null || sgroup.ListGroupInfo.Summary == null) return sgroup;
			if(sgroup.IsSummaryReady) return sgroup;
			sgroup.IsSummaryReady = true;
			sgroup.SetSummary(GroupSummary, sgroup.ListGroupInfo.Summary);
			return sgroup;
		}
		public override void UpdateGroupSummary(List<SummaryItem> changedItems) {
			if(GroupSummary.Count > 0 && SortInfo.GroupCount > 0) {
				for(int n = 0; n < GroupInfo.Count; n++) {
					GroupInfo[n].ClearSummary();
				}
				DoRefresh(); 
			}
			base.UpdateGroupSummary(changedItems);
		}
		internal bool requireExpandAll = false;
		bool asyncStatusIsBusy = false;
		public override bool IsBusy { get { return asyncStatusIsBusy; } }
		internal void OnAsyncRowReceived(int rowIndex) {
			VisualClient.UpdateRow(rowIndex);
			ThreadClient.OnRowLoaded(rowIndex);
			if(rowIndex == CurrentListSourceIndex) CheckCurrentControllerRowObject();
		}
		internal void OnAsyncBusyChanged(bool busy) {
			this.asyncStatusIsBusy = busy;
			if(!busy) {
				if(IsGrouped && requireExpandAll) {
					this.requireExpandAll = false;
					for(int n = 0; n < GroupInfo.Count; n++) {
						if(((ServerModeGroupRowInfo)GroupInfo[n]).ChildrenReady) GroupInfo[n].Expanded = true;
					}
					BuildVisibleIndexes();
					VisualClient.UpdateLayout();
				}
			}
			if(busy)
				ThreadClient.OnAsyncBegin();
			else
				ThreadClient.OnAsyncEnd();
		}
		internal void OnAsyncTotalsReceived(CommandGetTotals result) {
			UpdateTotalSummaryResult(result.TotalSummary);
			if(!IsGrouped) {
				BuildVisibleIndexes();
				OnTotalsReceived();
			}
			VisualClient.UpdateLayout();
		}
		internal void OnTotalsRequested() {
		}
		protected virtual void OnTotalsReceived() {
			ThreadClient.OnTotalsReceived();
			if(this.requireExpandAll) return;
			RowsKeeper.OnTotalsReceived();
		}
		protected virtual void OnRootGroupReceived() {
			OnTotalsReceived();
		}
		public override void LoadRowHierarchy(int rowHandle, OperationCompleted completed) {
			if(!IsValidControllerRowHandle(rowHandle)) return;
			if(completed == null) completed = (x) => {
			};
			if(!IsGrouped) {
				completed(true);
				return;
			}
			if(GetParentRowHandle(rowHandle) != InvalidRow) {
				completed(true);
				return;
			}
			object res = Wrapper.GetRowInfo(rowHandle, (a) => {
				if(a is AsyncRowInfo) LoadRowHierarchyCore(a as AsyncRowInfo, completed);
			});
			if(res == null) {
				completed(false);
				return;
			}
			AsyncRowInfo info = res as AsyncRowInfo;
			if(info == null) {
				completed(false); 
				return;
			}
			LoadRowHierarchyCore(info, completed);
		}
		bool LoadRowHierarchyCore(AsyncRowInfo info, OperationCompleted completed) {
			Server.GetRowIndexByKey(info.Key, AsyncOperationCompletedHelper.GetCommandParameter(new OperationCompleted((a) => {
				CommandGetRowIndexByKey command = a as CommandGetRowIndexByKey;
				if(command == null) return;
				RestoreGroupHierarchy(command, false, completed);
			})));
			return false;
		}
		internal void RestoreGroupHierarchy(CommandGetRowIndexByKey result, bool expandGroups, OperationCompleted completed) {
			int refIndex = 0;
			ServerModeGroupRowInfo lastGroupInfo = null;
			ListSourceGroupInfo lastListSourceGroup = null;
			for(int n = 0; n < result.Groups.Count; n++) {
				ListSourceGroupInfo listSourceGroup = FindMatchedGroup(ref refIndex, result.Index, result.Groups[n].ChildrenGroups);
				if(listSourceGroup == null) return;
				ServerModeGroupRowInfo groupInfo = GroupInfo.FindGroup(listSourceGroup);
				if(groupInfo == null) {
					if(lastGroupInfo == null) return;
					CommandGetGroupInfo command = new CommandGetGroupInfo(lastListSourceGroup, new DictionaryEntry(Tag_ExpandChildren_bool, false), new DictionaryEntry(Tag_PGroup_ServerModeGroupRowInfo, lastGroupInfo));
					command.ChildrenGroups = result.Groups[n].ChildrenGroups;
					OnAsyncGroupInfoReceived(new List<CommandGetGroupInfo>(new CommandGetGroupInfo[] { command }));
					groupInfo = GroupInfo.FindGroup(listSourceGroup);
					if(groupInfo != null && expandGroups) groupInfo.Expanded = true;
				}
				else {
					if(expandGroups) groupInfo.Expanded = true;
				}
				lastListSourceGroup = listSourceGroup;
				lastGroupInfo = groupInfo;
			}
			OnAfterAsyncGroupInfoReceived(false);
			if(completed != null) completed(true);
		}
		ListSourceGroupInfo FindMatchedGroup(ref int groupStartIndex, int index, List<ListSourceGroupInfo> list) {
			foreach(ListSourceGroupInfo group in list) {
				int endIndex = groupStartIndex + group.ChildDataRowCount;
				if(index >= groupStartIndex && index < endIndex) return group;
				groupStartIndex += group.ChildDataRowCount;
			}
			return null;
		}
		internal void OnAsyncGroupInfoReceived(List<CommandGetGroupInfo> results) {
			bool isRootGroup = this.rootGroupInfoRequested;
			foreach(CommandGetGroupInfo result in results) OnAsyncGroupInfoReceivedCore(result);
			GroupInfo.UpdateIndexes();
			OnAfterAsyncGroupInfoReceived(isRootGroup);
		}
		internal void OnAfterAsyncGroupInfoReceived(bool isRootGroup) {
			int visibleCount = VisibleCount;
			BuildVisibleIndexes();
			if(isRootGroup) OnRootGroupReceived();
			if(visibleCount != VisibleCount || !requireExpandAll) {
				VisualClient.UpdateLayout();
			}
			else {
				VisualClient.UpdateRows(0);
			}
		}
		int maxListGroupCount = 10000;
		List<ListSourceGroupInfo> CheckLimitServerGroupResult(List<ListSourceGroupInfo> childList) {
			if(childList.Count > maxListGroupCount) childList = childList.Take(maxListGroupCount).ToList(); 
			return childList;
		}
		static readonly object Tag_PGroup_ServerModeGroupRowInfo = new object();
		static readonly object Tag_ExpandChildren_bool = new object();
		internal void OnAsyncGroupInfoReceivedCore(CommandGetGroupInfo result) {
			List<ListSourceGroupInfo> childList = result.ChildrenGroups;
			ListSourceGroupInfo parent = result.ParentGroup;
			bool expandChildren;
			result.TryGetTag(Tag_ExpandChildren_bool, out expandChildren);
			childList = CheckLimitServerGroupResult(childList);
			if(parent == null) { 
				if(this.requireExpandAll) expandChildren = true;
				this.rootGroupInfoRequested = false; 
				BaseClearVisibleInfo();
				GroupInfo.Clear();
				if(!IsAllowAutoExpandGroupInfo(childList)) expandChildren = false;
				CreateGroupInfo(childList, GroupInfo, null, expandChildren);
				((AsyncServerModeGroupRowInfoCollection)GroupInfo).UpdateRootGroups();
			}
			else {
				ServerModeGroupRowInfo pgroup;
				result.TryGetTag(Tag_PGroup_ServerModeGroupRowInfo, out pgroup);
				if(pgroup != null) {
					if(pgroup.ChildrenReady) return;
					if(childList.Count > 0) {
						List<GroupRowInfo> insertList = new List<GroupRowInfo>();
						CreateGroupInfo(childList, insertList, pgroup, expandChildren);
						pgroup.ChildrenReady = true;
						((ServerDataControllerGroupRowInfoCollection)GroupInfo).UpdateChildren(pgroup, insertList);
						if(IsGroupRowHandle(CurrentControllerRow)) {
							int groupIndex = GroupRowInfo.HandleToGroupIndex(CurrentControllerRow);
							if(groupIndex > pgroup.Index) {
								groupIndex += insertList.Count;
								InternalSetControllerRow(GroupRowInfo.GroupIndexToHandle(groupIndex));
								CurrentClient.OnCurrentControllerRowChanged(new CurrentRowEventArgs());
							}
						}
					}
					else {
						pgroup.ChildrenReady = true;
					}
				}
				else {
					OnAsyncInvalidGroupInfoReceived();
					System.Diagnostics.Debug.Assert(false, "*** Error during group processing - Can't find parent group");
				}
			}
		}
		protected virtual void OnAsyncInvalidGroupInfoReceived() { }
		void RequestChildren(ServerModeGroupRowInfo sgroup, bool expandChildren) {
			if(sgroup.ChildrenReady) return;
			GetServerGroupInfo(sgroup, expandChildren);
		}
		internal bool IsAllowRequestMoreAutoExpandGroups() {
			return GroupRowCount < 50000;
		}
		void CreateGroupInfo(List<ListSourceGroupInfo> list, IList destination, ServerModeGroupRowInfo parentGroup, bool expandChildren) {
			byte level = (byte)(parentGroup == null ? 0 : (parentGroup.Level + 1));
			int startIndex = parentGroup == null ? 0 : parentGroup.ChildControllerRow;
			bool finalLevel = SortInfo.GroupCount == level + 1;
			List<ServerModeGroupRowInfo> tempDestination = new List<ServerModeGroupRowInfo>();
			for(int n = 0; n < list.Count; n++) {
				ListSourceGroupInfo linfo = list[n];
				ServerModeGroupRowInfo group = new ServerModeGroupRowInfo(level, startIndex, parentGroup, linfo);
				group.ChildControllerRowCount = linfo.ChildDataRowCount;
				group.ChildrenReady = finalLevel;
				group.Expanded = GroupInfo.AutoExpandAllGroups || level <= GroupInfo.AlwaysVisibleLevelIndex || expandChildren;
				if(IsAllowRequestMoreAutoExpandGroups() && RowsKeeper.IsExpandGroup(group) && !this.requireExpandAll) {
					group.Expanded = true;
					RequestChildren(group, false);
				}
				if(this.requireExpandAll) {
					group.Expanded = false;
				}
				tempDestination.Add(group);
				startIndex += group.ListGroupInfo.ChildDataRowCount;
			}
			SummarySortInfo sortInfo = SummarySortInfo.GetByLevel(parentGroup == null ? 0 : parentGroup.Level + 1);
			if(sortInfo != null) {
				foreach(GroupRowInfo rowInfo in tempDestination) {
					RequestSummary(rowInfo);
				}
				tempDestination.Sort(new GroupSummaryComparer(this, sortInfo));
			}
			for(int n = 0; n < tempDestination.Count; n++) {
				ServerModeGroupRowInfo group = tempDestination[n];
				destination.Add(group);
				CreateChildren(destination, level, group.ChildControllerRow, group.ListGroupInfo, group, expandChildren || this.requireExpandAll);
			}
		}
		int logCounter = 0;
		internal void Log(string text, params object[] args) {
			System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}", logCounter++, string.Format(text, args)));
		}
		bool IsAllowAutoExpandGroupInfo(List<ListSourceGroupInfo> childList) {
			int count = GroupInfo.Count + (childList == null ? 0 : childList.Count);
			return count < 10000;
		}
		bool rootGroupInfoRequested = false;
		void GetServerGroupInfo(ServerModeGroupRowInfo groupInfo, bool expandChildren) {
			if(groupInfo == null) {
				this.rootGroupInfoRequested = true;
			}
			else {
				if(!IsAllowAutoExpandGroupInfo(null)) expandChildren = false;
			}
			if(groupInfo != null && this.rootGroupInfoRequested) return; 
			var tags = new List<DictionaryEntry>();
			tags.Add(new DictionaryEntry(Tag_ExpandChildren_bool, expandChildren));
			tags.Add(new DictionaryEntry(Tag_PGroup_ServerModeGroupRowInfo, groupInfo));
			if(expandChildren)
				tags.Add(CommandQueue.GetLowPriorityTag());
			Server.GetGroupInfo(groupInfo == null ? null : groupInfo.ListGroupInfo, tags.ToArray());
		}
		void CreateChildren(IList destination, byte level, int startIndex, ListSourceGroupInfo linfo, ServerModeGroupRowInfo group, bool expandChildren) {
			if(level + 1 <= GroupInfo.AlwaysVisibleLevelIndex || GroupInfo.AutoExpandAllGroups || expandChildren) {
				GetServerGroupInfo(group, expandChildren);
			}
		}
		internal void UpdateTotalSummaryResult(List<object> summaryResults) {
			if(summaryResults != null && summaryResults.Count == TotalSummary.Count) {
				for(int n = 0; n < summaryResults.Count; n++) {
					TotalSummary[n].SummaryValue = summaryResults[n];
				}
			}
			for(int n = 0; n < TotalSummary.Count; n++) {
				if(!TotalSummary[n].GetAllowExternalCalculate(AllowSortUnbound))
					CalcTotalSummaryItem(TotalSummary[n]);
			}
		}
	}
	public class AsyncServerModeListSourceRowsKeeperEx : ListSourceRowsKeeper {
		public AsyncServerModeListSourceRowsKeeperEx(AsyncServerModeDataController controller, SelectedRowsKeeper rowsKeeper)
			: base(controller, rowsKeeper) {
		}
		protected override GroupedRowsKeeperEx CreateGroupRowsKeeper() {
			return new ServerModeGroupedRowsKeeperEx(Controller);
		}
		protected new AsyncServerModeDataController Controller { get { return (AsyncServerModeDataController)base.Controller; } }
		protected override void RestoreSelectionCore(int count) {
			RestoreRegularRowsSelection();
		}
		protected virtual void RestoreRegularRowsSelection() {
			if(Controller.Server == null) return;
			foreach(var enLevel in SelectionHash.Levels) {
				int level = enLevel.Key;
				var rows = enLevel.Value;
				if(level == BaseRowsKeeper.DataRowsLevel) {
					foreach(var entry in rows) {
						if(entry.Key == BaseRowsKeeper.NullObject) continue;
						int index = -1; 
						if(index >= 0)
							SelectionHash.RestoreCore(index, level, entry.Value);
					}
				}
			}
		}
		protected override object ExGetGroupRowKeyCore(GroupRowInfo group) {
			ServerModeGroupRowInfo sgroup = group as ServerModeGroupRowInfo;
			if(sgroup == null || sgroup.ListGroupInfo == null) return null; 
			return GroupHashEx.GetGroupRowKeyEx(group);
		}
	}
	public class AsyncServerModeDataControllerVisibleIndexCollection : DataControllerVisibleIndexCollection {
		public AsyncServerModeDataControllerVisibleIndexCollection(DataController controller) : base(controller) { }
		protected override int GetMaxCount() { return 1000; }
		protected override List<GroupRowInfo> GetRootGroups() {
			return ((AsyncServerModeGroupRowInfoCollection)GroupInfo).RootGroups;
		}
	}
	public class AsyncListDataControllerHelper : ListDataControllerHelper {
		public AsyncListDataControllerHelper(AsyncServerModeDataController controller) : base(controller) { }
		public override object GetRowValue(int listSourceRow, int column, OperationCompleted completed) {
			DataColumnInfo columnInfo = Columns[column];
			object row = listSourceRow;
			if(!columnInfo.Unbound) {
				if(completed == null)
					row = GetRow(listSourceRow, null);
				else
					row = GetRow(listSourceRow, delegate(object args) {
				if(completed != null && !AsyncServerModeDataController.IsNoValue(args)) completed(columnInfo.PropertyDescriptor.GetValue(args));
			});
			}
			if(AsyncServerModeDataController.IsNoValue(row)) return AsyncServerModeDataController.NoValue;
			return columnInfo.PropertyDescriptor.GetValue(row);
		}
		protected override Delegate GetGetRowValueCore(DataColumnInfo columnInfo, Type expectedReturnType) {
			throw new NotSupportedException();
		}
		public override object GetRow(int listSourceRow, OperationCompleted completed) {
			if(listSourceRow < 0 || listSourceRow >= List.Count) return null;
			AsyncListWrapper wrapper = List as AsyncListWrapper;
			if(wrapper != null) return wrapper.GetRow(listSourceRow, completed);
			return List[listSourceRow];
		}
		public override object GetRowKey(int listSourceRow) {
			return listSourceRow; 
		}
	}
	public class AsyncServerModeGroupRowInfoCollection : ServerDataControllerGroupRowInfoCollection {
		List<GroupRowInfo> rootGroups;
		public AsyncServerModeGroupRowInfoCollection(AsyncServerModeDataController controller)
			: base(controller) {
			this.rootGroups = new List<GroupRowInfo>();
		}
		protected override void ClearItems() {
			base.ClearItems();
			rootGroups.Clear();
		}
		public override int RootGroupCount { get { return RootGroups.Count; } }
		internal List<GroupRowInfo> RootGroups { get { return rootGroups; } }
		internal void UpdateRootGroups() {
			this.rootGroups.Clear();
			for(int n = 0; n < Count; n++) {
				GroupRowInfo group = this[n];
				if(group.Level == 0) rootGroups.Add(group);
			}
		}
		internal ServerModeGroupRowInfo FindGroup(ListSourceGroupInfo sourceGroupInfo) {
			for(int n = 0; n < Count; n++) {
				ServerModeGroupRowInfo group = this[n] as ServerModeGroupRowInfo;
				if(group.Level != sourceGroupInfo.Level || group.ChildControllerRowCount != sourceGroupInfo.ChildDataRowCount) continue;
				if(group.ListGroupInfo == null) continue;
				if(object.Equals(group.ListGroupInfo.GroupValue, sourceGroupInfo.GroupValue)) return group;
			}
			return null;
		}
	}
}

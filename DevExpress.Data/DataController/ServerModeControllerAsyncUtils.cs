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
using DevExpress.Data.Filtering;
using DevExpress.Data.Async;
using System.Collections;
using System.Linq;
#if SL
using DictionaryEntry = System.Collections.Generic.KeyValuePair<object, object>;
#endif
namespace DevExpress.Data.Helpers {
	public class AsyncServerModeDataControllerFilterHelper : DataControllerFilterHelper {
		public AsyncServerModeDataControllerFilterHelper(AsyncServerModeDataController controller) : base(controller) { }
		public new AsyncServerModeDataController Controller { get { return (AsyncServerModeDataController)base.Controller; } }
		public override object[] GetUniqueColumnValues(int column, int maxCount, bool includeFilteredOut, bool roundDataTime, OperationCompleted completed, bool implyNullLikeEmptyStringWhenFiltering) {
			if(maxCount == 0) return null;
			if(Controller.Server == null || !Controller.IsColumnValid(column)) return null;
			DataColumnInfo colInfo = Controller.Columns[column];
			if(colInfo.Unbound) {
				if(!colInfo.UnboundWithExpression || !Controller.AllowSortUnbound) return new object[0];
			}
			if(completed == null) return new object[0];
			try {
				CriteriaOperator expression = AsyncServerModeDataController.DescriptorToCriteria(colInfo);
				if(roundDataTime && (colInfo.Type.Equals(typeof(DateTime)) || colInfo.Type.Equals(typeof(DateTime?)))) {
					expression = new FunctionOperator(FunctionOperatorType.GetDate, expression);
				}
				if(maxCount == -1 || maxCount > 10000) maxCount = 10000;
				OperationCompleted patchedCompleted;
				if(completed == null || implyNullLikeEmptyStringWhenFiltering)
					patchedCompleted = completed;
				else
					patchedCompleted = values => completed(((IEnumerable)values).Cast<object>().Where(o => o != null).ToArray());
				Controller.Server.GetUniqueColumnValues(expression, maxCount, includeFilteredOut, AsyncOperationCompletedHelper.GetCommandParameter(patchedCompleted));
				return new object[] { AsyncServerModeDataController.NoValue };
			}
			catch {
				return new object[0];
			}
		}
	}
	public class AsyncServerModeCurrentAndSelectedRowsKeeper : CurrentAndSelectedRowsKeeper {
		protected new AsyncServerModeDataController Controller { get { return (AsyncServerModeDataController)base.Controller; } }
		public AsyncServerModeCurrentAndSelectedRowsKeeper(AsyncServerModeDataController controller) : base(controller, false) { }
		protected override bool IsAllowSaveCurrentControllerRow { get { return true; } }
		protected override void RestoreCurrentRow() { }
		protected override void SaveCurrentRow() { }
	}
	public class AsyncServerModeListSourceRowsKeeper : ListSourceRowsKeeper {
		public AsyncServerModeListSourceRowsKeeper(AsyncServerModeDataController controller, SelectedRowsKeeper rowsKeeper)
			: base(controller, rowsKeeper) { }
		protected override GroupedRowsKeeperEx CreateGroupRowsKeeper() {
			return new AsyncServerModeGroupedRowsKeeperEx(Controller);
		}
		protected internal new AsyncServerModeGroupedRowsKeeperEx GroupHashEx { get { return base.GroupHashEx as AsyncServerModeGroupedRowsKeeperEx; } }
		protected new AsyncServerModeDataController Controller { get { return (AsyncServerModeDataController)base.Controller; } }
		protected internal bool AllowRestoreGrouping { get { return needRestoreGrouping; } }
		bool needRestoreGrouping = false;
		int maxRestoreGroupLevel = -1;
		List<RowInfo> currentRowInfo = new List<RowInfo>();
		public override void SaveIncremental() { }
		public override void Save() {
			this.maxRestoreGroupLevel = -1;
			this.needRestoreGrouping = false;
			this.currentRowInfo.Clear();
			this.groupColumnsInfo = GetGroupedColumns();
			if(!Controller.IsReady) return;
			SaveCurrentRow();
			GroupHashEx.Clear();
			if(Controller.KeepGroupRowsExpandedOnRefresh) {
				GroupHashEx.Save();
			}
		}
		void SaveCurrentRow() {
			int current = Controller.CurrentControllerRow;
			RowInfo info = GetCurrentRowKey(0);
			if(info != null) currentRowInfo.Add(info);
			info = GetCurrentRowKey(1);
			if(info != null) currentRowInfo.Add(info);
			if(Controller.IsGrouped) {
				GroupRowInfo group = Controller.GroupInfo.GetGroupRowInfoByControllerRowHandle(current);
				if(group != null && group.ChildControllerRowCount > 1) {
					int childIndex = 0;
					if(current == group.ChildControllerRow) {
						childIndex = current + 1;
					}
					else {
						childIndex = group.ChildControllerRow - 1;
					}
					info = GetRowKey(childIndex);
					if(info != null) currentRowInfo.Add(info);
				}
			}
		}
		protected RowInfo GetCurrentRowKey(int index) {
			int current = Controller.CurrentControllerRow;
			RowInfo res = null;
			if(Controller.IsGroupRowHandle(current) && index > 0) return null;
			res = GetRowKey(current + index);
			if(res != null) return res;
			if(Controller.IsGroupRowHandle(current)) return null;
			current += index;
			if(index == 0) {
				if(Controller.CurrentControllerRowObject != null) return new RowInfo(Controller.CurrentControllerRowObject);
			}
			if(index == 1) {
				if(Controller.CurrentControllerRowObjectEx != null) return new RowInfo(Controller.CurrentControllerRowObjectEx);
			}
			return null;
		}
		protected RowInfo GetRowKey(int controllerRow) {
			if(Controller.IsGroupRowHandle(controllerRow)) {
				ServerModeGroupRowInfo info = Controller.GroupInfo.GetGroupRowInfoByHandle(controllerRow) as ServerModeGroupRowInfo;
				if(info == null || info.GroupValue == null) return null;
				int level = info.Level;
				List<Object> keys = new List<object>();
				while(info != null) {
					keys.Insert(0, info.GroupValue);
					info = info.ParentGroup as ServerModeGroupRowInfo;
				}
				return new RowInfo(level, keys.ToArray());
			}
			object key = Controller.GetLoadedRowKey(controllerRow);
			if(key == null) return null;
			return new RowInfo(key);
		}
		protected class RowInfo {
			int level;
			object key;
			public RowInfo(object key) : this(-1, key) {
			}
			public RowInfo(int level, object key) {
				this.level = level;
				this.key = key;
			}
			public object Key { get { return key; } }
			public int Level { get { return level; } }
			public bool IsGroupRow { get { return level != -1; } }
		}
		protected override bool RestoreCore(bool clear) {
			TryRestoreCurrentControllerRow(0);
			if(CheckGroupedColumns()) {
				this.maxRestoreGroupLevel = GetMaxAllowedGroupLevel();
				if(this.maxRestoreGroupLevel >= 0) this.needRestoreGrouping = true;
			}
			return false;
		}
		public virtual bool IsExpandGroup(GroupRowInfo group) {
			if(!needRestoreGrouping) return false;
			if(GroupHashEx.AllExpanded) {
				return true;
			}
			else {
				if(group.Level <= maxRestoreGroupLevel) {
					object key = GroupHashEx.GetGroupRowKeyEx(group);
					if(key != null) {
						return GroupHashEx.Contains(key, group.Level);
					}
				}
			}
			return false;
		}
		public virtual bool TryRestoreCurrentControllerRow(int index) {
			if(!Controller.KeepFocusedRowOnUpdate) return false;
			if(Controller.IsGrouped) {
				return TryRestoreGroupedCurrentControllerRow(0);
			}
			if(Controller.LastGroupedColumnCount > 0) return false;
			if(currentRowInfo.Count == 0) return false;
			RowInfo info = currentRowInfo[0];
			currentRowInfo.RemoveAt(0);
			if(info.IsGroupRow) return false; 
			int currentRowIndex = Controller.CurrentControllerRow;
			Controller.Server.GetRowIndexByKey(info.Key, AsyncOperationCompletedHelper.GetCommandParameter(new OperationCompleted(delegate(object args) {
				CommandGetRowIndexByKey result = (CommandGetRowIndexByKey)args;
				int foundRow = result.Index;
				if(foundRow < 0) {
					if(index == 0 && currentRowInfo.Count > 0) TryRestoreCurrentControllerRow(1);
					return;
				}
				if(currentRowIndex == Controller.CurrentControllerRow) {
					Controller.CurrentControllerRow = foundRow;
					Controller.CheckCurrentControllerRowObject();
				}
			})));
			return true;
		}
		protected internal virtual void OnTotalsReceived() {
			if(!Controller.KeepFocusedRowOnUpdate) return;
			if(Controller.LastGroupedColumnCount == 0 && Controller.IsGrouped) {
				if(Controller.GroupInfo.Count > 0) Controller.CurrentControllerRow = -1;
				return;
			}
			if(Controller.LastGroupedColumnCount > 0 && !Controller.IsGrouped) {
				Controller.CurrentControllerRow = 0;
				return;
			}
			if(Controller.IsGrouped && currentRowInfo.Count > 0 && currentRowInfo[0].IsGroupRow && Controller.GroupInfo.Count > 0) {
				Controller.CurrentControllerRow = -1;
				return;
			}
		}
		bool TryRestoreGroupedCurrentControllerRow(int index) {
			if(Controller.LastGroupedColumnCount == 0) {
				return true;
			}
			RowInfo info = null;
			if(index < currentRowInfo.Count) info = currentRowInfo[index];
			if(info == null) return false;
			int currentRowIndex = Controller.CurrentControllerRow;
			OperationCompleted completed = (a) => {
				CommandGetRowIndexByKey result = (CommandGetRowIndexByKey)a;
				int foundRow = result.Index;
				if(foundRow == DataController.InvalidRow) {
					if(index < currentRowInfo.Count - 1) TryRestoreGroupedCurrentControllerRow(index + 1);
					return;
				}
				if(result.Groups != null && result.Groups.Count > 0) {
					Controller.RestoreGroupHierarchy(result, true, null);
				}
				if(currentRowIndex == Controller.CurrentControllerRow || Controller.CurrentControllerRow == -1) {
					Controller.CurrentControllerRow = foundRow;
				}
			};
			if(info.Key is ListSourceGroupInfo) {
				var res = Controller.GroupInfo.FindGroup(info.Key as ListSourceGroupInfo);
				if(res != null) completed(new CommandGetRowIndexByKey(null) { Index = res.Handle, Groups = null });
				return true;
			}
			if(info.IsGroupRow) return false;
			Controller.Server.GetRowIndexByKey(info.Key, AsyncOperationCompletedHelper.GetCommandParameter(new OperationCompleted(delegate(object args) {
				CommandGetRowIndexByKey result = (CommandGetRowIndexByKey)args;
				if(result.Index < 0) result.Index = DataController.InvalidRow;
				completed(result);
			})));
			return true;
		}
		public override void Clear() {
			this.currentRowInfo.Clear();
		}
	}
	public class AsyncServerModeGroupedRowsKeeperEx : GroupedRowsKeeperEx {
		public AsyncServerModeGroupedRowsKeeperEx(DataController controller) : base(controller) { }
		public override bool AllExpanded {
			get {
				if(!base.AllExpanded) return false;
				if(RecordsCount > Controller.VisibleListSourceRowCount / 2) return true;
				return false;
			}
		}
		protected override bool GetAllRecordsSelected() {
			int groupCount = Controller.GroupInfo.Count;
			if(groupCount == 0) return false;
			if(Controller.VisibleCount < groupCount) return false;
			foreach(GroupRowInfo info in Controller.GroupInfo) {
				if(!info.Expanded) return false;
			}
			return true;
		}
	}
	public class AsyncOperationCompletedHelper {
		OperationCompleted completed;
		AsyncOperationCompletedHelper(OperationCompleted completed) {
			this.completed = completed;
		}
		static readonly object Token = new object();
		public static DictionaryEntry GetCommandParameter(OperationCompleted completed) {
			return new DictionaryEntry(Token, new AsyncOperationCompletedHelper(completed));
		}
		public static DictionaryEntry GetCommandParameter(OperationCompleted[] completed) {
			return GetCommandParameter(Combine(completed));
		}
		static OperationCompleted Combine(params OperationCompleted[] delegates) {
			if(delegates == null || delegates.Length == 0) {
				return null;
			}
			Delegate rv = delegates[0];
			for(int i = 1; i < delegates.Length; i++) {
				rv = Delegate.Combine(rv, delegates[i]);
			}
			return (OperationCompleted)rv;
		}
		public static OperationCompleted GetCompletedDelegate(Command command) {
			AsyncOperationCompletedHelper helper;
			if(command.TryGetTag(Token, out helper)) {
				return helper.completed;
			} else {
				return null;
			}
		}
		public static void AppendCompletedDelegate(Command command, OperationCompleted next) {
			if(next == null)
				return;
			AsyncOperationCompletedHelper helper;
			if(command.TryGetTag(Token, out helper)) {
				helper.completed = (OperationCompleted)Delegate.Combine(helper.completed, next);
			} else {
				throw new InvalidOperationException(string.Format("'{0}' command did not have AsyncCompletedHelper tag to append next continuation", command));
			}
		}
	}
}

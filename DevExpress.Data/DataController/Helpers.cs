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
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Data.IO;
using DevExpress.XtraGrid;
using System.Collections.Generic;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Data;
using DevExpress.Utils.Design;
#if !SL
using System.Data;
using System.Windows.Forms;
#else
using DictionaryEntry = System.Collections.Generic.KeyValuePair<object, object>;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Xpf.Collections;
using DevExpress.Data.Browsing;
#endif
#if SL && DEBUGTEST
using DevExpress.Data.Test;
#endif 
#if !DXPORTABLE
namespace DevExpress.Data.Design {
	public class SummaryItemTypeConverter : EnumTypeConverter {
		public SummaryItemTypeConverter() : base(typeof(SummaryItemType)) { }
	}
}
#endif
namespace DevExpress.Data {
	public enum CustomSummaryProcess { Start, Calculate, Finalize }
	public enum ColumnSortOrder { None, Ascending, Descending };
#if !DXPORTABLE
	[TypeConverter(typeof(DevExpress.Data.Design.SummaryItemTypeConverter))]
#endif
	[ResourceFinder(typeof(ResFinder))]
	public enum SummaryItemType { Sum, Min, Max, Count, Average, Custom, None };
	public enum SummaryItemTypeEx { Sum, Min, Max, Count, Average, Custom, None, Top, TopPercent, Bottom, BottomPercent, Unique, Duplicate };
}
namespace DevExpress.Data.Summary {
	public static class SummaryItemTypeHelper {
		public static bool CanApplySummary(SummaryItemType summaryType, Type objectType) {
			if (summaryType == SummaryItemType.Count || summaryType == SummaryItemType.Custom) return true;
			if (objectType == null) return false;
			if (summaryType == SummaryItemType.Average || summaryType == SummaryItemType.Sum) {
				return IsNumericalType(objectType);
			}
			if (summaryType == SummaryItemType.Min || summaryType == SummaryItemType.Max) {
				Type underlyingType = Nullable.GetUnderlyingType(objectType);
				if (underlyingType != null)
					objectType = underlyingType;
				foreach (Type type in objectType.GetInterfaces()) {
					if (type == typeof(IComparable)) return true;
				}
			}
			return false;
		}
		static readonly Type[] numericalTypes = new Type[] { 
					typeof(System.Decimal), typeof(System.Decimal?),
					typeof(System.Single), typeof(System.Single?),
					typeof(System.Double), typeof(System.Double?),
					typeof(System.Int16), typeof(System.Int16?),
					typeof(System.Int32), typeof(System.Int32?),
					typeof(System.Int64), typeof(System.Int64?),
					typeof(System.UInt16), typeof(System.UInt16?),
					typeof(System.UInt32), typeof(System.UInt32?),
					typeof(System.UInt64), typeof(System.UInt64?),
					typeof(System.Byte), typeof(System.Byte?),
					typeof(System.SByte), typeof(System.SByte?),
		};
		public static bool IsDateTime(Type type) {
			return type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTime?));
		}
		public static bool IsBool(Type type) {
			return type.Equals(typeof(bool)) || type.Equals(typeof(bool?));
		}
		public static bool IsNumericalType(Type type) {
			return Array.IndexOf<Type>(numericalTypes, type) >= 0;
		}
	}
}
namespace DevExpress.Data.Helpers {
	public static class DefaultColumnAlignmentHelper {
		public static bool IsColumnFarAlignedByDefault(Type columnType) {
			if(columnType == null)
				return false;
			Type fixedType = Nullable.GetUnderlyingType(columnType);
			if(fixedType == null)
				fixedType = columnType;
			switch(DXTypeExtensions.GetTypeCode(fixedType)) {
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
				default:
					return false;
			}
		}
	}
	public class DataColumnSortInfo {
		DataColumnInfo columnInfo;
		ColumnSortOrder sortOrder;
		DefaultBoolean requireOnCellCompare;
		ColumnGroupInterval groupInterval;
		bool runningSummary, crossGroupRunningSummary;
		DataColumnInfo auxColumnInfo;
		public DataColumnSortInfo(DataColumnInfo columnInfo) : this(columnInfo, ColumnSortOrder.Ascending) { }
		public DataColumnSortInfo(DataColumnInfo columnInfo, ColumnSortOrder sortOrder) : 
			this(columnInfo, sortOrder, ColumnGroupInterval.Default) { }
		public DataColumnSortInfo(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, ColumnGroupInterval groupInterval) {
			this.groupInterval = groupInterval;
			this.columnInfo = columnInfo;
			this.sortOrder = sortOrder;
			this.requireOnCellCompare = DefaultBoolean.Default;
			this.runningSummary = false;
			this.crossGroupRunningSummary = false;
			if(ColumnInfo == null) throw new ArgumentNullException("columnInfo");
		}
		public ColumnGroupInterval GroupInterval { get { return groupInterval; } set { groupInterval = value; } }
		public DataColumnInfo ColumnInfo { get { return columnInfo; } }
		public DataColumnInfo AuxColumnInfo { get { return auxColumnInfo; } set { auxColumnInfo = value; } }
		public ColumnSortOrder SortOrder { 
			get { return sortOrder; } 
		}
		public DefaultBoolean RequireOnCellCompare {
			get { return requireOnCellCompare; }
			set { requireOnCellCompare = value; }
		}
		public bool RunningSummary {
			get { return runningSummary; }
			set { runningSummary = value; }
		}
		public bool CrossGroupRunningSummary {
			get { return crossGroupRunningSummary; }
			set { crossGroupRunningSummary = value; }
		}
		public bool IsEquals(DataColumnSortInfo info) {
			if(info == null) return false;
			return info.ColumnInfo == ColumnInfo && info.SortOrder == SortOrder;
		}
	}
	public class SummarySortInfo {
		SummaryItem summaryItem;
		ColumnSortOrder sortOrder;
		int groupLevel;
		public SummarySortInfo(SummaryItem summaryItem) : this(summaryItem, 0, ColumnSortOrder.Ascending) { }
		public SummarySortInfo(SummaryItem summaryItem, int groupLevel, ColumnSortOrder sortOrder) {
			this.summaryItem = summaryItem;
			this.sortOrder = sortOrder;
			this.groupLevel = groupLevel;
		}
		public SummaryItem SummaryItem { get { return summaryItem; } }
		public ColumnSortOrder SortOrder { get { return sortOrder; } }
		public int GroupLevel { get { return groupLevel; } }
	}
	public class SummarySortInfoCollection : NotificationCollectionBase {
		public SummarySortInfoCollection(CollectionChangeEventHandler collectionChanged) : base(collectionChanged) { }
		public SummarySortInfo this[int index] { 
			get { 
				if(index >= Count) return null;
				return List[index] as SummarySortInfo; 
			} 
		}
		public SummarySortInfo GetByLevel(int groupLevel) {
			for(int n = Count - 1; n >= 0; n--) {
				SummarySortInfo info = this[n];
				if(info.GroupLevel == groupLevel) return info;
			}
			return null;
		}
		public void ClearAndAddRange(SummarySortInfo[] sortInfos) {
			BeginUpdate();
			try {
				Clear();
				AddRange(sortInfos);
			}
			finally {
				EndUpdate();
			}
		}
		public void AddRange(SummarySortInfo[] sortInfos) {
			if(sortInfos == null) return;
			BeginUpdate();
			try {
				foreach(SummarySortInfo sortInfo in sortInfos) { 
					if(sortInfo != null)
						List.Add(sortInfo); 
				}
			}
			finally {
				EndUpdate();
			}
		}
		public SummarySortInfo Add(SummaryItem summaryItem, int groupLevel, ColumnSortOrder sortOrder) {
			SummarySortInfo sortInfo = new SummarySortInfo(summaryItem, groupLevel, sortOrder);
			List.Add(sortInfo);
			return sortInfo;
		}
		public void Remove(SummarySortInfo sortInfo) {
			List.Remove(sortInfo);
		}
		internal bool CheckSummaryCollection(SummaryItemCollection summaryCollection) {
			bool changed = false;
			for(int i = Count - 1; i >= 0; i--) {
				if(!summaryCollection.Contains(this[i].SummaryItem)) {
					changed = true;
					InnerList.RemoveAt(i);
				}
			}
			return changed;
		}
	}
	public class DataColumnSortInfoCollection: ColumnInfoNotificationCollection, IEnumerable<DataColumnSortInfo> {
		int groupCount = 0;
		public DataColumnSortInfoCollection(DataControllerBase controller) : this(controller, null) { }
		public DataColumnSortInfoCollection(DataControllerBase controller, CollectionChangeEventHandler collectionChanged) : base(controller, collectionChanged) { }
		public DataColumnSortInfo this[int index] { get { return List[index] as DataColumnSortInfo; } }
		public void ClearAndAddRange(int groupCount, params DataColumnSortInfo[] sortInfos) { ClearAndAddRange(sortInfos, groupCount); }
		public void ClearAndAddRange(params DataColumnSortInfo[] sortInfos) { ClearAndAddRange(sortInfos, 0); }
		public void ClearAndAddRange(DataColumnSortInfo[] sortInfos, int groupCount) {
			BeginUpdate();
			try {
				Clear();
				AddRange(sortInfos, groupCount);
			}
			finally {
				EndUpdate();
			}
		}
		public DataColumnSortInfo[] ToArray() {
			DataColumnSortInfo[] res = new DataColumnSortInfo[Count];
			for(int i = 0; i < Count; i++) {
				res[i] = this[i];
			}
			return res;
		}
		public int GetGroupIndex(DataColumnInfo info) {
			int gc = GroupCount;
			if(gc == 0) return -1;
			for(int n = 0; n < gc; n++) {
				if(this[n].ColumnInfo == info) return n;
			}
			return -1;
		}
		public int GetSortIndex(DataColumnInfo info) {
			int gc = Count;
			if(gc == 0) return -1;
			for(int n = 0; n < gc; n++) {
				if(this[n].ColumnInfo == info) return n;
			}
			return -1;
		}
		protected internal bool Contains(string columnName) {
			int gc = Count;
			if(gc == 0) return false;
			for(int n = 0; n < gc; n++) {
				if(this[n].ColumnInfo.Name == columnName) return true;
			}
			return false;
		}
		public void AddRange(int groupCount, params DataColumnSortInfo[] sortInfos) { AddRange(sortInfos, groupCount); }
		public void AddRange(params DataColumnSortInfo[] sortInfos) { AddRange(sortInfos, GroupCount); }
		public void AddRange(DataColumnSortInfo[] sortInfos, int groupCount) {
			BeginUpdate();
			try {
				GroupCount = groupCount;
				foreach(DataColumnSortInfo sortInfo in sortInfos) { 
					if(sortInfo == null) continue;
					List.Add(sortInfo); 
				}
			}
			finally {
				EndUpdate();
			}
		}
#if !SL
		[Obsolete()]
		public void Assign(ColumnGroupSortInfoCollection oldSort) {
			if(oldSort.IsEquals(this)) return;
			BeginUpdate();
			try {
				Clear();
				this.GroupCount = oldSort.GroupCount;
				foreach(ColumnGroupSortInfo sort in oldSort) {
					if(Controller.Columns[sort.FieldName] == null) continue;
					Add(Controller.Columns[sort.FieldName], sort.Order);
				}
			}
			finally {
				EndUpdate();
			}
		}
#endif
		public DataColumnSortInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, bool runningSummary, bool crossGroupRunningSummary) {
			DataColumnSortInfo columnSortInfo = Add(columnInfo, sortOrder);
			columnSortInfo.RunningSummary = runningSummary;
			columnSortInfo.CrossGroupRunningSummary = crossGroupRunningSummary;
			return columnSortInfo;
		}
		public DataColumnSortInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder) {
			DataColumnSortInfo columnSortInfo = new DataColumnSortInfo(columnInfo, sortOrder);
			List.Add(columnSortInfo);
			return columnSortInfo;
		}
		public int GroupCount {
			get {
				if(groupCount <= Count) return groupCount;
				return Count;
			}
			set {
				if(value < 0) value = 0;
				groupCount = value;
				OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
			}
		}
		public bool IsEquals(DataColumnSortInfoCollection collection) {
			if(collection == null || collection.Count != Count) return false;
			for(int i = 0; i < Count; i ++) {
				if(!this[i].IsEquals(collection[i]))
					return false;
			}
			return true;
		}
		public void ChangeGroupSorting(int index) {
			if(index < 0 || index >= GroupCount) return;
			ColumnSortOrder sortOrder = this[index].SortOrder == ColumnSortOrder.Ascending ? ColumnSortOrder.Descending : ColumnSortOrder.Ascending;
			InnerList[index] = new DataColumnSortInfo(this[index].ColumnInfo, sortOrder);
		}
		protected override DataColumnInfo GetColumnInfo(int index) { return this[index].ColumnInfo; }
		public virtual DataColumnSortInfoCollection Clone() {
			DataColumnSortInfoCollection res = new DataColumnSortInfoCollection(Controller);
			foreach(DataColumnSortInfo info in this) {
				res.Add(info.ColumnInfo, info.SortOrder, info.RunningSummary, info.CrossGroupRunningSummary);
			}
			res.groupCount = this.groupCount;
			return res;
		}
		IEnumerator<DataColumnSortInfo> IEnumerable<DataColumnSortInfo>.GetEnumerator() {
			foreach(DataColumnSortInfo dcsi in (IEnumerable)this)
				yield return dcsi;
		}
	}
	public enum NotifyChangeType { ItemAdded, ItemChanged, ItemDeleted }
	public class DataControllerChangedItem {
		int controllerRowHandle;
		NotifyChangeType changedType;
		bool visible;
		int visibleIndex;
		bool groupSimpleChange;
		public DataControllerChangedItem(int controllerRowHandle, NotifyChangeType changedType, GroupRowInfo parentGroupRow, bool groupSimpleChange) {
			this.controllerRowHandle = controllerRowHandle;
			this.changedType = changedType;
			this.visible = parentGroupRow == null || (parentGroupRow.IsVisible && parentGroupRow.Expanded);
			this.visibleIndex = controllerRowHandle;
			this.groupSimpleChange = groupSimpleChange;
		}
		public bool GroupSimpleChange { get { return groupSimpleChange; } }
		public int ControllerRowHandle { get { return controllerRowHandle; } }
		public NotifyChangeType ChangedType { get { return changedType; } }
		public bool Visible { get { return  visible; } }
		public int VisibleIndex { 
			get { return visibleIndex; } 
			set { 
				visibleIndex = value; 
				if(value < 0) this.visible = false;
			} 
		}
		public bool IsEqual(int controllerRowHandle, NotifyChangeType changedType) {
			return ControllerRowHandle == controllerRowHandle && changedType == ChangedType;
		}
	}
	public class DataControllerChangedItemCollection {
		List<DataControllerChangedItem> innerList;
		public int Count { get { return innerList == null ? 0 : innerList.Count; } }
		void EnsureInnerList() {
			if (innerList == null) innerList = new List<DataControllerChangedItem>();
		}
		public DataControllerChangedItem this[int index] { 
			get {
				if(innerList == null) return null;
				return innerList[index]; 
			} 
		}
		public void AddItem(int controllerRowHandle, NotifyChangeType changedType, GroupRowInfo parentGroupRow) {
			AddItem(controllerRowHandle, changedType, parentGroupRow, false);
		}
		public void AddItem(int controllerRowHandle, NotifyChangeType changedType, GroupRowInfo parentGroupRow, bool groupSimpleChange) {
			if(!HasItem(controllerRowHandle, changedType)) {
				EnsureInnerList();
				this.innerList.Add(new DataControllerChangedItem(controllerRowHandle, changedType, parentGroupRow, groupSimpleChange));
			}
		}
		public void RemoveAt(int index) {
			if(innerList != null) innerList.RemoveAt(index);
		}
		public void UpdateVisibleIndexes(VisibleIndexCollection visibleIndexes, bool isAdded) {
			if(visibleIndexes.IsEmpty) return;
			for(int i = 0; i < Count; i ++) {
				DataControllerChangedItem item = this[i];
				if(!item.Visible) {
					item.VisibleIndex = -1;
					continue;
				}
				if(isAdded && item.ChangedType == NotifyChangeType.ItemAdded) 
					item.VisibleIndex = visibleIndexes.IndexOf(item.ControllerRowHandle);
				if(!isAdded && item.ChangedType != NotifyChangeType.ItemAdded) 
					item.VisibleIndex = visibleIndexes.IndexOf(item.ControllerRowHandle);
			}
		}
		public bool AlwaysNotifyVisualClient = false;
		public virtual void NotifyVisualClient(DataController controller, IDataControllerVisualClient visualClient) {
			if(visualClient == null || Count == 0 || visualClient == NullVisualClient.Default) return;
			bool notifyTotalSummary = controller.AutoUpdateTotalSummary && controller.TotalSummary.ActiveCount > 0;
			int topRowIndexDelta = CalculateTopRowDelta(controller, visualClient);
			if(IsVisibleRangeChanged(visualClient.TopRowIndex, visualClient.PageRowCount) || AlwaysNotifyVisualClient) {
				RemoveNonVisibleItems();
				if(IsChangedOnly(controller) && !AlwaysNotifyVisualClient) {
					if(topRowIndexDelta != 0 || controller.IsGrouped)
						visualClient.UpdateRowIndexes(topRowIndexDelta + visualClient.TopRowIndex);
					NotifyChanges(visualClient);
				}
				else  {
					visualClient.UpdateRows(topRowIndexDelta);
				}
			} else {
				if(topRowIndexDelta != 0) 
					visualClient.UpdateRowIndexes(topRowIndexDelta + visualClient.TopRowIndex);
				else {
					if(IsVisibleRowCountChanged) {
						visualClient.UpdateScrollBar();
					}
				}
			}
			if(notifyTotalSummary)
				visualClient.UpdateTotalSummary();
		}
		protected virtual int CalculateTopRowDelta(DataController controller, IDataControllerVisualClient visualClient) {
			int oldTopRowIndex = visualClient.TopRowIndex;
			int res = 0;
			for(int n = Count - 1; n >= 0; n--) {
				DataControllerChangedItem item = this[n];
				if(!item.Visible || item.ChangedType == NotifyChangeType.ItemChanged || item.VisibleIndex > oldTopRowIndex) continue;
				res += (item.ChangedType == NotifyChangeType.ItemAdded ? 1 : -1);
			}
			return res;
		}
		protected virtual void RemoveNonVisibleItems() {
			for(int n = Count - 1; n >= 0; n--) {
				if(!this[n].Visible) RemoveAt(n);
			}
		}
		protected virtual bool IsChangedOnly(DataController controller) {
				for(int n = Count - 1; n >= 0; n--) {
					if(this[n].ChangedType != NotifyChangeType.ItemChanged)
						return false;
				if(!this[n].GroupSimpleChange && this[n].ControllerRowHandle < 0 && controller.IsRowExpanded(this[n].ControllerRowHandle)) return false;
				}
				return true;
			}
		void NotifyChanges(IDataControllerVisualClient visualClient) {
			for(int n = 0; n < Count; n++) 
				visualClient.UpdateRow(this[n].ControllerRowHandle);
		}
		bool IsVisibleRangeChanged(int topRowIndex, int pageRowCount) {
			for(int i = 0; i < Count; i ++) {
						if(this[i].VisibleIndex == BaseListSourceDataController.NewItemRow) return true;
				if(this[i].Visible && this[i].VisibleIndex >= topRowIndex && 
					this[i].VisibleIndex < topRowIndex + pageRowCount)
					return true;
			}
			return false;
		}
		protected virtual bool IsVisibleRowCountChanged {
			get {
				int addedRemovedItems = 0;
				for(int i = 0; i < Count; i ++) {
					if(this[i].Visible) {
						if(this[i].ChangedType == NotifyChangeType.ItemAdded)
							addedRemovedItems ++;
						if(this[i].ChangedType == NotifyChangeType.ItemDeleted)
							addedRemovedItems --;
					}
				}
				return addedRemovedItems != 0;
			}
		}
		bool HasItem(int visibleRow, NotifyChangeType changedType) {
			for(int i = 0; i < Count; i ++) {
				if(this[i].IsEqual(visibleRow, changedType)) 
					return true;
			}
			return false;
		}
	}
	public interface IIndexRenumber {
		int GetCount();
		int GetValue(int pos);
		void SetValue(int pos, int val);
	}
	public class IndexRenumber {
		public static void RenumberIndexes(IIndexRenumber list, int listSourceRow, bool increment, int maxIndex) {
			for(int n = list.GetCount() - 1; n >=0; n --) {
				int lr = list.GetValue(n);
				if(lr >= listSourceRow) {
					if(maxIndex != -1 && increment && lr == maxIndex) continue;
					list.SetValue(n, lr + (increment ? 1 : -1));
				}
			}
		}
		public static void RenumberIndexes(IIndexRenumber list, int listSourceRow, bool increment) {
			for(int n = list.GetCount() - 1; n >=0; n --) {
				int lr = list.GetValue(n);
				if(lr >= listSourceRow) {
					list.SetValue(n, lr + (increment ? 1 : -1));
				}
			}
		}
		public static void RenumberIndexes(IIndexRenumber list, int oldListSourceRow, int newListSourceRow) {
			int min = Math.Min(oldListSourceRow, newListSourceRow), max = Math.Max(oldListSourceRow, newListSourceRow);
			for(int n = list.GetCount() - 1; n >=0; n --) {
				int lr = list.GetValue(n);
				if(lr >= min && lr <= max)
					list.SetValue(n, lr + (oldListSourceRow < newListSourceRow ? -1 : 1));
			}
		}
	}
	public abstract class BaseRowsKeeper : IDisposable {
		public const int DataRowsLevel = -1;
		DataController controller;
		readonly Dictionary<int, Dictionary<object, object>> hash;
		KeyValuePair<int, Dictionary<object, object>>[] levels;
		bool allRecordsSelected;
		protected BaseRowsKeeper(DataController controller) {
			this.controller = controller;
			hash = new Dictionary<int, Dictionary<object, object>>();
			levels = null;
		}
		public void Dispose() {
			Clear();
			this.controller = null;
		}
		public bool AnyGroupRows {
			get {
				if(Levels == null) return false;
				foreach(var level in Levels) {
					if(level.Key >= 0) return true;
				}
				return false;
			}
		}
		public bool AnyDataRows {
			get {
				if(Levels == null) return false;
				foreach(var level in Levels) {
					if(level.Key == DataRowsLevel) return true;
				}
				return false;
			}
		}
		protected bool AllRecordsSelected { get { return allRecordsSelected; } set { allRecordsSelected = value; } }
		protected Dictionary<int, Dictionary<object, object>> Hash { get { return hash; } }
		protected DataController Controller { get { return controller; } }
		protected BaseDataControllerHelper Helper { get { return Controller.Helper; } }
		protected virtual bool GetAllRecordsSelected() { return false; } 
		public bool IsEmpty { get { return Hash.Count == 0; } }
		public int Count { get { return Hash.Count; } }
		public void Clear() { 
			Hash.Clear(); 
			levels = null;
		}
		public void SaveToStream(Stream stream) {
			TypedBinaryWriter writer = new TypedBinaryWriter(stream);
			bool allSelected = GetAllRecordsSelected();
			writer.WriteObject(allSelected);
			if(allSelected) return;
			writer.WriteObject(Levels.Length);
			for(int n = 0; n < Levels.Length; n++) {
				var entry = Levels[n];
				var hash = entry.Value;
				writer.WriteObject((int)entry.Key);
				writer.WriteObject(hash.Count);
				foreach(var hashEntry in hash) {
					if(SaveLevelObject(writer, hashEntry.Key)) {
						writer.WriteType(hashEntry.Value.GetType());
						writer.WriteObject(hashEntry.Value);
					}
				}
			}
		}
		protected virtual bool SaveLevelObject(TypedBinaryWriter writer, object obj) {
			throw new NotImplementedException();
		}
		protected virtual object RestoreLevelObject(TypedBinaryReader reader) {
			throw new NotImplementedException();
		}
		public void RestoreFromStream(Stream stream) {
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			bool allSelected = (bool)reader.ReadObject(typeof(bool));
			AllRecordsSelected = allSelected;
			if(allSelected) return;
			int count = (int)reader.ReadObject(typeof(int));
			Hash.Clear();
			for(int n = 0; n < count; n++) {
				int key = (int)reader.ReadObject(typeof(int));
				int lcount = (int)reader.ReadObject(typeof(int));
				var level = new Dictionary<object, object>();
				for(int l = 0; l < lcount; l++) {
					object obj = RestoreLevelObject(reader);
					if(obj == null) continue;
					Type type = reader.ReadType();
					object res = reader.ReadObject(type);
					level[obj] = res;
				}
				Hash[key] = level;
			}
			this.levels = null;
		}
		public abstract void Save();
		protected internal abstract void RestoreCore(object row, int level, object value);
		public bool Restore(object rowKey, object row) {
			bool hasChanges = false;
			for(int n = 0; n < Levels.Length; n++) {
				var entry = Levels[n];
				int level = entry.Key;
				var levelHash = entry.Value;
				object value;
				if(levelHash.TryGetValue(rowKey, out value)) {
					hasChanges = true;
					RestoreCore(row, level, value);
				}
			}
			return hasChanges;
		}
		public bool Contains(object rowKey, int level) {
			if(Levels.Length <= level) return false;
			var levelHash = Levels[level].Value;
			return levelHash.ContainsKey(rowKey);
		}
		protected internal void RemoveSelected(object rowKey, int level) {
			Dictionary<object, object> levelHash;
			if(Hash.TryGetValue(level, out levelHash)) {
				if(levelHash.ContainsKey(rowKey))
					levelHash.Remove(rowKey);
			}
		}
		protected void SetSelected(object rowKey, int level, object value) {
			Dictionary<object, object> levelHash;
			if(!Hash.TryGetValue(level, out levelHash)) {
				Hash[level] = levelHash = new Dictionary<object, object>();
				this.levels = null;
			}
			levelHash[rowKey] = value;
		}
		protected internal KeyValuePair<int, Dictionary<object, object>>[] Levels { 
			get {
				if(levels == null) {
					levels = Hash.OrderBy(pair => pair.Key).ToArray();
				}
				return levels;
			}
		}
		protected GroupRowInfo GetGroupRow(int listSourceRow, int level) {
			GroupRowInfo group = Controller.GroupInfo.GetGroupRowInfoByControllerRowHandle(GetControllerRow(listSourceRow));
			if(group != null) group = group.GetParentGroupAtLevel((int)level);
			return group;
		}
		public readonly static object NullObject = new object();
		public object GetRowKey(GroupRowInfo group) {
			if(group.GroupValue != null) return group.GroupValue;
			return GetRowKey(GetListSourceRow(group));
		}
		public object GetRowKey(int listSourceRow) {
			if(listSourceRow == DataController.InvalidRow || listSourceRow >= Helper.Count) return null;
			object res = Helper.GetRowKey(listSourceRow);
			if(res == null) return NullObject;
			return res;
		}
		protected int GetControllerRow(int listSourceRow) { return Controller.GetControllerRow(listSourceRow); }
		protected int GetListSourceRow(int controllerRow) {
			return Controller.GetListSourceRowIndex(controllerRow);
		}
		protected int GetListSourceRow(GroupRowInfo group) {
			return GetListSourceRow(group.ChildControllerRow);
		}
		public object GetGroupRowKeyEx(GroupRowInfo group) {
			if(group == null) return null;
			object[] res = new object[group.Level + 1];
			res[group.Level] = GetValue(group);
			for(int n = group.Level - 1; n >= 0; n--) {
				group = group.ParentGroup;
				if(group == null) break;
				res[n] = GetValue(group);
				if(AsyncServerModeDataController.IsNoValue(res[n])) return null;
			}
			return new GroupObjectKeyInfo(res);
		}
		object GetValue(GroupRowInfo group) {
			object res = Controller.GetGroupRowValue(group);
			if(res == null) return NullObject;
			return res;
		}
	}
	public class GroupObjectKeyInfo {
		object[] values;
		int? combinedHashCode = null;
		public GroupObjectKeyInfo(object[] values) {
			this.values = values;
		}
		public object[] Values { get { return values; } }
		public override int GetHashCode() {
			if(combinedHashCode == null) combinedHashCode = UpdateHashCode();
			return (int)combinedHashCode;
		}
		int UpdateHashCode() {
			if(Values.Length == 0) return base.GetHashCode();
			int res = 0;
			for(int n = Values.Length - 1; n >= 0; n--) {
				res += Values[n].GetHashCode();
			}
			return res;
		}
		public override bool Equals(object obj) {
			GroupObjectKeyInfo info = obj as GroupObjectKeyInfo;
			if(info == null) return false;
			if(object.ReferenceEquals(this, info)) return true;
			if(Values.Length != info.Values.Length) return false;
			for(int n = Values.Length - 1; n >= 0; n--) {
				if(!Object.Equals(Values[n], info.Values[n])) return false;
			}
			return true;
		}
	}
	public class GroupedRowsKeeperEx : BaseRowsKeeper {
		public int RecordsCount { get; set; }
		public GroupedRowsKeeperEx(DataController controller) : base(controller) {}
		public override void Save() {
			RecordsCount = Controller.VisibleListSourceRowCount;
			AllRecordsSelected = GetAllRecordsSelected();
			if(AllRecordsSelected) RecordsCount = GetExpandedDataCount();
			if(Controller.GroupedColumnCount == 0 || Controller.GroupInfo.AutoExpandAllGroups) return;
			int level = Controller.GroupedColumnCount;
			for(int n = 0; n < Controller.GroupInfo.Count; n++) {
				GroupRowInfo group = Controller.GroupInfo[n];
				if(!group.Expanded) continue;
				if(group.Level >= level) continue;
				object key = GetGroupRowKeyEx(group);
				if(key == null) continue;
				SetSelected(key, group.Level, true);
			}
		}
		public virtual bool AllExpanded { get { return AllRecordsSelected; } }
		protected internal override void RestoreCore(object row, int level, object value) {
			GroupRowInfo group = row as GroupRowInfo;
			if(group != null) Controller.RestoreGroupExpanded(group);
		}
		protected override bool GetAllRecordsSelected() {
			return Controller.VisibleCount == (Controller.ListSourceRowCount + Controller.GroupInfo.Count) && Controller.GroupInfo.Count > 0;
		}
		protected virtual int GetExpandedDataCount() {
			if(Controller.GroupInfo.Count == 0) return 0;
			int res = 0;
			foreach(GroupRowInfo info in Controller.GroupInfo) {
				if(Controller.GroupInfo.IsLastLevel(info)) res += info.ChildControllerRowCount;
			}
			return res;
		}
		protected override bool SaveLevelObject(TypedBinaryWriter writer, object obj) {
			GroupObjectKeyInfo gInfo = obj as GroupObjectKeyInfo;
			if(gInfo == null) return false;
			writer.WriteObject(gInfo.Values.Length);
			for(int n = 0; n < gInfo.Values.Length; n++) {
				if(gInfo.Values[n] == NullObject) {
					writer.Write((byte)0);
					continue;
				}
				writer.Write((byte)1);
				writer.WriteType(gInfo.Values[n] == null ? typeof(object) : gInfo.Values[n].GetType());
				writer.WriteObject(gInfo.Values[n]);
			}
			return true;
		}
		protected override object RestoreLevelObject(TypedBinaryReader reader) {
			int count = (int)reader.ReadObject(typeof(int));
			object[] values = new object[count];
			for(int n = 0; n < count; n++) {
				byte notNullObject = reader.ReadByte();
				if(notNullObject == 0) {
					values[n] = NullObject;
					continue;
				}
				Type type = reader.ReadType();
				values[n] = reader.ReadObject(type);
			}
			GroupObjectKeyInfo info = new GroupObjectKeyInfo(values);
			return info;
		}
	}
	public class ServerModeCurrentAndSelectedRowsKeeper : CurrentAndSelectedRowsKeeper {
		public ServerModeCurrentAndSelectedRowsKeeper(ServerModeDataController controller, bool allowKeepSelection) : base(controller, allowKeepSelection) { }
		protected override bool IsAllowSaveCurrentControllerRow { get { return Controller.GroupedColumnCount == 0; } } 
	}
	public class CurrentAndSelectedRowsKeeper : SelectedRowsKeeper {
		int groupCount = 0;
		int foundCurrentRow = DataController.InvalidRow;
		protected class CurrentRowData {
			public CurrentRowData(object selectedObject) : this(selectedObject, 0) { }
			public CurrentRowData(object selectedObject, int index) {
				this.Index = index;
				this.SelectedObject = selectedObject;
			}
			public object SelectedObject;
			public int Index;
		}
		public CurrentAndSelectedRowsKeeper(DataController controller, bool allowKeepSelection) : base(controller, allowKeepSelection) { }
		protected new BaseGridController Controller { get { return base.Controller as BaseGridController; } }
		public override void Save() {
			base.Save();
			this.foundCurrentRow = DataController.InvalidRow;
			this.groupCount = Controller.LastGroupedColumnCount;
			if(IsAllowSaveCurrentControllerRow && Controller.CurrentControllerRow != DataController.InvalidRow && !Controller.IsInitializing) {
				SaveCurrentRow();
			}
		}
		protected virtual void SaveCurrentRow() {
			SaveRowCore(Controller.CurrentControllerRow, new CurrentRowData(Controller.Selection.GetSelectedObject(Controller.CurrentControllerRow)));
		}
		protected virtual bool IsAllowSaveCurrentControllerRow { get { return true; } }
		protected virtual void RestoreCurrentRow() {
			if(!Controller.AllowRestoreSelection && Controller.CurrentControllerRow == BaseListSourceDataController.FilterRow) foundCurrentRow = BaseListSourceDataController.FilterRow;
			if((this.groupCount == 0 || this.foundCurrentRow == DataController.InvalidRow) && Controller.GroupedColumnCount > 0 && Controller.GroupRowCount > 0) {
				if(Controller.CurrentControllerRow != BaseListSourceDataController.FilterRow)
					Controller.CurrentControllerRow = Controller.GroupInfo[0].Handle;
			}
			else {
				if (this.foundCurrentRow == DataController.InvalidRow) {
					this.foundCurrentRow = Controller.CurrentControllerRow == BaseListSourceDataController.FilterRow ? BaseListSourceDataController.FilterRow : 0;
				}
				if(Controller.KeepFocusedRowOnUpdate)
					Controller.CurrentControllerRow = this.foundCurrentRow;
			}
		}
		public override void OnRestoreEnd() {
			base.OnRestoreEnd();
			RestoreCurrentRow();
		}
		protected internal override void RestoreCore(object row, int level, object value) {
			CurrentRowData data = value as CurrentRowData;
			if(data != null) {
				GroupRowInfo group = row as GroupRowInfo;
				int listSourceRow = DataController.InvalidRow;
				if(row is int) listSourceRow = (int)row;
				if(!(this.groupCount == 0 && Controller.GroupedColumnCount != 0)) {
					if(group != null)
						this.foundCurrentRow = group.Handle;
					else
						this.foundCurrentRow = Controller.GetControllerRow(listSourceRow);
				}
				value = data.SelectedObject;
				if(value == null) return;
			}
			base.RestoreCore(row, level, value);
		}
	}
	public class SelectedRowsKeeper : BaseRowsKeeper {
		bool allowKeepSelection;
		public SelectedRowsKeeper(DataController controller, bool allowKeepSelection) : base(controller) {
			this.allowKeepSelection = allowKeepSelection;
		}
		protected virtual void SaveGroupInfo(GroupRowInfo group, object selectedObject) {
			object groupKey = GetGroupRowKeyEx(group);
			if(groupKey == null) return;
			SetSelected(groupKey, group.Level, selectedObject);
		}
		protected virtual void SaveDataRow(int controllerRow, object selectedObject) {
			object key = GetRowKey(GetListSourceRow(controllerRow));
			if(key == null) return;
			SetSelected(key, DataRowsLevel, selectedObject);
		}
		protected virtual void SaveRowCore(int selectedHandle, object selectedObject) {
			if(Controller.IsGroupRowHandle(selectedHandle)) {
				SaveGroupInfo(Controller.GroupInfo.GetGroupRowInfoByHandle(selectedHandle), selectedObject);
			}
			else {
				SaveDataRow(selectedHandle, selectedObject);
			}
		}
		public bool AllowKeepSelection { get { return allowKeepSelection; } }
		public override void Save() {
			if(Controller.Selection.Count == 0 || !AllowKeepSelection) return;
			int[] selection = Controller.Selection.GetSelectedRows();
			for(int n = 0; n < selection.Length; n ++) {
				object selectedObject = Controller.Selection.GetSelectedObject(selection[n]);
				int selectedHandle = selection[n];
				SaveRowCore(selectedHandle, selectedObject);
			}
		}
		public virtual void OnRestoreEnd() { }
		protected internal override void RestoreCore(object row, int level, object value) {
			int listSourceRow;
			GroupRowInfo group = row as GroupRowInfo;
			if(row is int)
				listSourceRow = (int)row;
			else
				listSourceRow = Controller.GetListSourceRowIndex(group.ChildControllerRow);
			if(level == DataRowsLevel && group == null) {
				Controller.Selection.SetListSourceRowSelected(listSourceRow, true, value);
			} else {
				if(group != null) Controller.Selection.SetSelected(group.Handle, true, value);
			}
		}
	}
	public class ListSourceRowsKeeper : IDisposable {
		SelectedRowsKeeper selectionHash;
		GroupedRowsKeeperEx groupHashEx;
		internal PropertyDescriptor[] groupColumnsInfo;
		DataController controller;
		public ListSourceRowsKeeper(DataController controller, SelectedRowsKeeper rowsKeeper) {
			this.controller = controller;
			this.selectionHash = rowsKeeper;
			this.groupHashEx = CreateGroupRowsKeeper();
			this.groupColumnsInfo = null;
		}
		protected virtual GroupedRowsKeeperEx CreateGroupRowsKeeper() { return new GroupedRowsKeeperEx(controller); }
		public void Dispose() {
			Clear();
		}
		protected SelectedRowsKeeper SelectionHash { get { return selectionHash; } }
		protected internal GroupedRowsKeeperEx GroupHashEx { get { return groupHashEx; } }
		protected DataController Controller { get { return controller; } }
		protected BaseDataControllerHelper Helper { get { return Controller.Helper; } }
		protected bool HasSaved { get { return !SelectionHash.IsEmpty || !GroupHashEx.IsEmpty; } }
		public virtual void SaveIncremental() {
			if(!HasSaved || !Controller.IsReady) {
				Save();
				return;
			}
			if(!CheckGroupedColumns()) return;
			this.groupColumnsInfo = GetGroupedColumns();
			if(Controller.KeepGroupRowsExpandedOnRefresh) {
				RemoveCollapsedGroupsFromHash();
				GroupHashEx.Save();
			}
			SelectionHash.Save();
		}
		void RemoveCollapsedGroupsFromHash() {
			for(int n = 0; n < Controller.GroupInfo.Count; n++) {
				GroupRowInfo group = Controller.GroupInfo[n];
				if(group.Expanded) continue;
				object key = ExGetGroupRowKeyCore(group);
				if(key == null) continue;
				GroupHashEx.RemoveSelected(key, group.Level);
			}
		}
		public bool RestoreIncremental() {
			return RestoreCore(false);
		}
		public virtual void Save() {
			if(HasSaved) return;
			if(!Controller.IsReady) return;
			this.groupColumnsInfo = GetGroupedColumns();
			if(Controller.KeepGroupRowsExpandedOnRefresh) {
				GroupHashEx.Save();
			}
			SelectionHash.Clear();
			if(Controller.AllowRestoreSelection)
				SelectionHash.Save();
		}
		public bool RestoreStream() {
			this.groupColumnsInfo = GetGroupedColumns();
			return Restore();
		}
		public bool Restore() {
			return RestoreCore(true);
		}
		protected virtual bool RestoreCore(bool clear) {
			bool hasGroupChanges = false;
			if(Controller.IsReady) {
				if(CheckGroupedColumns()) {
					hasGroupChanges = RestoreCore();
				}
				SelectionHash.OnRestoreEnd();
			}
			if(clear) 
				Clear();
			else {
				SelectionHash.Clear();
			}
			return hasGroupChanges;
		}
		public virtual void Clear() {
			SelectionHash.Clear();
			GroupHashEx.Clear();
			this.groupColumnsInfo = null;
		}
		protected PropertyDescriptor[] GetGroupedColumns() {
			PropertyDescriptor[] res = new PropertyDescriptor[Controller.SortInfo.GroupCount];
			for(int n = 0; n < Controller.SortInfo.GroupCount; n ++) {
				DataColumnSortInfo sortInfo = Controller.SortInfo[n];
				res[n] = sortInfo.ColumnInfo.PropertyDescriptor;
			}
			return res;
		}
		protected int GetMaxAllowedGroupLevel() {
			if(this.groupColumnsInfo == null) return -1;
			PropertyDescriptor[] currentInfo = GetGroupedColumns();
			if(currentInfo == null || currentInfo.Length == 0) return -1;
			int maxLength = Math.Min(currentInfo.Length, this.groupColumnsInfo.Length);
			int maxLevel = -1;
			for(int n = 0; n < maxLength; n++) {
				if(currentInfo[n].Equals(this.groupColumnsInfo[n]))
					maxLevel = n;
				else 
					break;
			}
			return maxLevel;
		}
		internal bool CheckGroupedColumns() {
			if(this.groupColumnsInfo == null) return false;
			if(GetGroupedColumns().Length != this.groupColumnsInfo.Length || this.groupColumnsInfo.Length > 0) return GetMaxAllowedGroupLevel() > -1;
			return true;
		}
		protected bool RestoreCore() {
			bool hasGroupChanges = false;
			int count = -1; 
			bool needRestoreGrouping = !Controller.GroupInfo.AutoExpandAllGroups && Controller.GroupRowCount > 0 && ((!GroupHashEx.IsEmpty || GroupHashEx.AllExpanded));
			bool needRestoreSelection = !SelectionHash.IsEmpty && (count = Helper.Count) > 0;
			if(!needRestoreGrouping && !needRestoreSelection) return false;
			if(count == -1) count = Helper.Count;
			if(needRestoreGrouping) {
				hasGroupChanges |= RestoreGrouping();
			}
			if(needRestoreSelection) {
				RestoreSelection(count);
			}
			return hasGroupChanges;
		}
		void RestoreSelection(int count) {
			Controller.Selection.BeginSelection();
			try {
				RestoreSelectionCore(count);
			}
			finally {
				Controller.Selection.EndSelection();
			}
		}
		protected virtual void RestoreSelectionCore(int count) {
			if(SelectionHash.AnyDataRows) {
				for(int n = 0; n < count; n++) {
					object key = SelectionHash.GetRowKey(n);
					SelectionHash.Restore(key, n);
				}
			}
			if(SelectionHash.AnyGroupRows) {
				foreach(GroupRowInfo group in Controller.GroupInfo) {
					SelectionHash.Restore(selectionHash.GetGroupRowKeyEx(group), group);
				}
			}
		}
		protected virtual bool RestoreGrouping() {
			bool hasGroupChanges = false;
			if(GroupHashEx.AllExpanded) {
				Controller.ExpandAll();
				return true;
			}
			int level = GetMaxAllowedGroupLevel();
			if(level < 0) return false;
			for(int n = 0; n < Controller.GroupInfo.Count; n++) {
				GroupRowInfo group = Controller.GroupInfo[n];
				if(group.Level > level) continue;
				object key = ExGetGroupRowKeyCore(group);
				if(key == null) continue;
				if(GroupHashEx.Restore(key, group)) {
					hasGroupChanges = true;
				}
			}
			return hasGroupChanges;
		}
		protected virtual object ExGetGroupRowKeyCore(GroupRowInfo group) {
			return GroupHashEx.GetGroupRowKeyEx(group);
		}
	}
	public class ColumnGroupSortInfo {
		int columnHandle, groupIndex;
		string fieldName;
		ColumnSortOrder order;
		public ColumnGroupSortInfo() : this(-1, -1, ColumnSortOrder.None, "") {
		}
		public ColumnGroupSortInfo(int columnHandle, int groupIndex, ColumnSortOrder order, string fieldName) {
			this.columnHandle = columnHandle;
			this.groupIndex = groupIndex;
			this.order = order;
			this.fieldName = fieldName;
		}
		public string FieldName { get { return fieldName; } }
		public int ColumnHandle { get { return columnHandle; } }
		public ColumnSortOrder Order { get { return order; } }
		public int GroupIndex { get { return groupIndex; } }
		public virtual bool IsEquals(DataColumnSortInfo si) {
			return si.ColumnInfo.Index == this.ColumnHandle && si.SortOrder == this.Order && si.ColumnInfo.Name == FieldName;
		}
	}
	[Obsolete()]
	public class ColumnGroupSortInfoCollection : CollectionBase {
		public ColumnGroupSortInfo this[int index] { get { return List[index] as ColumnGroupSortInfo; } }
		public virtual void Add(ColumnGroupSortInfo sInfo) {
			List.Add(sInfo);
		}
		public virtual void Remove(ColumnGroupSortInfo sInfo) {
			List.Remove(sInfo);
		}
		public int GroupCount {
			get { 
				int gcount = 0;
				for(int n = 0; n < Count; n++) {
					if(this[n].GroupIndex > -1) gcount ++;
				}
				return gcount;
			}
		}
		public virtual bool IsEquals(DataColumnSortInfoCollection sortInfo) {
			if(Count != sortInfo.Count || GroupCount != sortInfo.GroupCount) return false;
			for(int n = 0; n < Count; n++) {
				if(!this[n].IsEquals(sortInfo[n])) return false;
			}
			return true;
		}
	}
	public class MasterDetailHelper {
#if !SL && !DXPORTABLE
		static FieldInfo columnField;
		static Type dataColumnPropertyDescriptorType;
		static Type DataColumnPropertyDescriptorType {
			get {
				if(dataColumnPropertyDescriptorType == null)
					dataColumnPropertyDescriptorType = typeof(DataColumn).Assembly.GetType("System.Data.DataColumnPropertyDescriptor");
				return dataColumnPropertyDescriptorType;
			}
		}
#endif
		public static string GetDisplayName(PropertyDescriptor descriptor) {
			return GetDisplayNameCore(descriptor, !HasDisplayAttribute(descriptor));
		}
		public static bool HasDisplayAttribute(PropertyDescriptor descriptor) {
			return descriptor.DisplayName != descriptor.Name;
		}
		public static string GetDisplayNameCore(PropertyDescriptor descriptor) {
			return GetDisplayNameCore(descriptor, false);
		}
		public static string GetDisplayNameCore(PropertyDescriptor descriptor, bool useSplitCasing) {
#if !SL && !DXPORTABLE
			string caption;
			if(TryGetDataTableDataColumnCaption(descriptor, out caption))
				return caption;
#endif
			string res = descriptor.DisplayName;
			if(useSplitCasing) res = SplitPascalCaseString(res);
			return res;
		}
#if !SL
		public static bool TryGetDataTableDataColumnCaption(PropertyDescriptor descriptor, out string caption) {
			caption = "";
#if !DXPORTABLE
			try {
				if(!DevExpress.Data.Helpers.SecurityHelper.IsPartialTrust) {
					if(descriptor.GetType() == DataColumnPropertyDescriptorType) {
						DataColumn dc = null;
						if(columnField == null) columnField = DataColumnPropertyDescriptorType.GetField("column", BindingFlags.NonPublic | BindingFlags.Instance);
						if(columnField != null) dc = columnField.GetValue(descriptor) as DataColumn;
						if(dc != null) {
							var captionDesc = TypeDescriptor.GetProperties(dc)["Caption"];
							if(captionDesc != null && !captionDesc.ShouldSerializeValue(dc)) return false;
							caption = dc.Caption;
							return true;
						}
					}
				}
			} catch {
			}
#endif
			caption = default(string);
			return false;
		}
#endif
		public static string SplitPascalCaseString(string value) {
			return SplitStringHelper.SplitPascalCaseString(value);
		}
#if !SL && !DXPORTABLE
		protected DataColumnInfo[] GetDetailColumnsInfo(IList list) {
			ListSourceDataController dc = new ListSourceDataController();
			dc.ListSource = list;
			List<DataColumnInfo> columns = new List<DataColumnInfo>(dc.DetailColumns);
			dc.Dispose();
			return columns.ToArray();
		}
		protected DataColumnInfo[] GetDetailColumnsInfo(ITypedList list, PropertyDescriptor[] accessors) {
			if(list == null) return null;
			PropertyDescriptorCollection coll = list.GetItemProperties(accessors);
			if(coll == null || coll.Count == 0) return null;
			List<DataColumnInfo> columns = new List<DataColumnInfo>();
			foreach(PropertyDescriptor pd in coll) {
				if(typeof(IList).IsAssignableFrom(pd.PropertyType) && !typeof(Array).IsAssignableFrom(pd.PropertyType)) {
					if(!pd.IsBrowsable) continue;
					columns.Add(new DataColumnInfo(pd));
				}
			}
			return columns.ToArray();
		}
		protected DataColumnInfo[] GetDataColumnInfo(ITypedList list, PropertyDescriptor[] accessors) {
			if(list == null) return null;
			PropertyDescriptorCollection coll = list.GetItemProperties(accessors);
			if(coll == null || coll.Count == 0) return null;
			List<DataColumnInfo> columns = new List<DataColumnInfo>();
			foreach(PropertyDescriptor pd in coll) {
				if(!pd.IsBrowsable) continue;
				if(typeof(IList).IsAssignableFrom(pd.PropertyType) && !typeof(Array).IsAssignableFrom(pd.PropertyType)) continue;
				columns.Add(new DataColumnInfo(pd));
			}
			return columns.ToArray();
		}
		protected virtual void PopulateTypedListRelations(DetailNodeInfo parent, ITypedList list, int level, ref int callCount, PropertyDescriptor[] accessors) {
			if(list == null || level > 7 || callCount > 500) return;
			DataColumnInfo[] info = GetDetailColumnsInfo(list, accessors);
			parent.Columns = GetDataColumnInfo(list, accessors);
			if(info == null || info.Length == 0) return;
			callCount++;
			parent.Nodes = new DetailNodeInfo[info.Length];
			for(int n = 0; n < info.Length; n++) {
				parent.Nodes[n] = new DetailNodeInfo(info[n].Name);
				PopulateTypedListRelations(parent.Nodes[n], list, level + 1, ref callCount, new PropertyDescriptor[] { info[n].PropertyDescriptor });
			}
		}
		protected virtual void PopulateListRelations(DetailNodeInfo parent, IList list, int level, ref int callCount) {
			if(list == null || level > 5 || callCount > 500) return;
			DataColumnInfo[] info = GetDetailColumnsInfo(list);
			parent.Columns = GetDataColumnInfo(list);
			if(info == null || info.Length == 0) return;
			callCount++;
			parent.Nodes = new DetailNodeInfo[info.Length];
			for(int n = 0; n < info.Length; n++) {
				parent.Nodes[n] = new DetailNodeInfo(info[n].Name);
				if(list.Count > 0) {
					try {
						IList detail = info[n].PropertyDescriptor.GetValue(list[0]) as IList;
						parent.Nodes[n].List = detail;
						PopulateListRelations(parent.Nodes[n], detail, level + 1, ref callCount);
					} catch {
					}
				}
			}
		}
		protected virtual void PopulateRelations(DetailNodeInfo parent, IRelationList list, int level, ref int callCount) {
			if(list == null || level > 7 || callCount > 500) return;
			callCount++;
			int rc = list.RelationCount;
			parent.Nodes = new DetailNodeInfo[rc];
			for(int n = 0; n < rc; n++) {
				parent.Nodes[n] = new DetailNodeInfo(list.GetRelationName(0, n));
				parent.Nodes[n].List = list.GetDetailList(0, n);
				PopulateRelations(parent.Nodes[n], list.GetDetailList(0, n) as IRelationList, level + 1, ref callCount);
			}
		}
		protected virtual void PopulateRelations(DetailNodeInfo parent, DataTable table, int level) {
			if(table == null || level > 7) return;
			List<DetailNodeInfo> list = new List<DetailNodeInfo>();
			foreach(DataRelation rel in table.ChildRelations) {
				if(CheckRecursive(table, rel.ChildTable, 0)) continue;
				DetailNodeInfo detailNode = new DetailNodeInfo(rel.RelationName);
				detailNode.List = rel.ChildTable;
				list.Add(detailNode);
				PopulateRelations(detailNode, rel.ChildTable, level + 1);
			}
			if(list.Count > 0 && parent != null) parent.Nodes = list.ToArray();
		}
		bool CheckRecursive(DataTable table, DataTable childTable, int level) {
			if(level > 10) return false;
			foreach(DataRelation rel in table.ParentRelations) {
				if(rel.ChildTable == childTable) return true;
				if(CheckRecursive(rel.ParentTable, childTable, level + 1)) return true;
			}
			return false;
		}
		public DataColumnInfo[] GetDataColumnInfo(BindingContext context, object dataSource, string dataMember) {
			return GetDataColumnInfo(context, dataSource, dataMember, true);
		}
		public DataColumnInfo[] GetDataColumnInfo(BindingContext context, object dataSource, string dataMember, bool skipException) {
			try {
				IList list = GetDataSource(context, dataSource, dataMember);
				if(list == null) return null;
				ListSourceDataController dc = new ListSourceDataController();
				dc.ListSource = list;
				List<DataColumnInfo> columns = new List<DataColumnInfo>(dc.Columns);
				dc.Dispose();
				return columns.ToArray();
			} catch(Exception e) {
				if(!skipException)
					throw e;
			}
			return null;
		}
		public DataColumnInfo[] GetDataColumnInfo(object data) {
			return GetDataColumnInfo(null, data, null);
		}
		public DetailNodeInfo[] GetDetailInfo(BindingContext context, object dataSource, string dataMember) {
			return GetDetailInfo(context, dataSource, dataMember, false);
		}
		public DetailNodeInfo[] GetDetailInfo(BindingContext context, object dataSource, string dataMember, bool allowExceptions) {
			DetailNodeInfo root = new DetailNodeInfo("root");
			try {
				int callCount = 0;
				IList list = GetDataSource(context, dataSource, dataMember);
				DataView dv = list as DataView;
				if(dv != null && dv.Table != null) {
					PopulateRelations(root, dv.Table, 0);
				}
				else {
					ITypedList tl = list as ITypedList;
					IRelationList rl = list as IRelationList;
					if(rl != null) 
						PopulateRelations(root, rl, 0, ref callCount);
					else {
						if(tl != null)
							PopulateTypedListRelations(root, tl, 0, ref callCount, null);
						else
							PopulateListRelations(root, list, 0, ref callCount);
					}
				}
			} catch {
				if(allowExceptions) throw;
			}
			return root.Nodes;
		}
		public static bool IsDataSourceReady(BindingContext context, object dataSource, string dataMember) {
			DataView view = dataSource as DataView;
			if(view != null && view.Table == null) return false;
			return true;
		}
#endif
#if DEBUGTEST || (!SL && !DXPORTABLE)
		public static IList GetDataSource(object dataSource, string dataMember) {
			return GetDataSource(null, dataSource, dataMember);
		}
		public static IList GetDataSource(BindingContext context, object dataSource, string dataMember) {
			try {
				return GetDataSourceCore(context, dataSource, dataMember);
			}
			catch {
				return null;
			}
		}
		static IList GetDataSourceCore(BindingContext context, object dataSource, string dataMember) {
			if(dataSource == null) return null;
#if !SL && !DXPORTABLE
			if(context != null) {
				CurrencyManager manager = context[dataSource, dataMember] as CurrencyManager;
				if(manager != null) return manager.List;
			}
#endif
			if(String.IsNullOrEmpty(dataMember)) {
				IListSource listSource = dataSource as IListSource;
				if(listSource != null) return listSource.GetList();
			}
			IList res = dataSource as IList;
			if(res != null) return res;
#if !SL && !DXPORTABLE
			DataSet ds = dataSource as DataSet;
			if(ds != null) {
				if(String.IsNullOrEmpty(dataMember)) 
					dataSource = ds.Tables.Count > 0 ? ds.Tables[0] : null;
				else
					dataSource = ds.Tables[dataMember];
			}
#endif
			if(dataSource is DataTable) {
				res = ((DataTable)dataSource).DefaultView;
			}
			if(res == null) {
				IListSource listSource = dataSource as IListSource;
				if(listSource != null)
					res = listSource.GetList();
			}
			if(res == null && dataSource is IEnumerable) {
				res = new ListIEnumerable(dataSource as IEnumerable);
			}
			return res;
		}
#endif
		}
		public class ListIEnumerable : IList {
		IEnumerable source;
		List<object> list = null;
		public ListIEnumerable(IEnumerable source) {
			this.source = source;
		}
		void CheckPopulateList() {
			if(list == null) {
				list = new List<object>();
				foreach(object item in source) list.Add(item);
			}
		}
		public IEnumerable Source { get { return source; } }
		public override int GetHashCode() {
			return source.GetHashCode();
		}
		#region IList Members
		int IList.Add(object value) { throw new NotImplementedException(); }
		void IList.Clear() { throw new NotImplementedException(); }
		bool IList.Contains(object value) { throw new NotImplementedException(); }
		int IList.IndexOf(object value) { throw new NotImplementedException(); }
		void IList.Insert(int index, object value) { throw new NotImplementedException(); }
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return true; } }
		void IList.Remove(object value) { throw new NotImplementedException(); }
		void IList.RemoveAt(int index) { throw new NotImplementedException(); }
		object IList.this[int index] {
			get {
				CheckPopulateList();
				return list[index];
			}
			set { throw new NotImplementedException(); }
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) { throw new NotImplementedException(); }
		int ICollection.Count {
			get {
				CheckPopulateList();
				return list.Count;
			}
		}
		bool ICollection.IsSynchronized { get { return true; } }
		object ICollection.SyncRoot { get { return this; } }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() { return source.GetEnumerator(); }
		#endregion
		internal bool IsEmpty() {
			if(list != null) return list.Count == 0;
			IEnumerator enu = source.GetEnumerator();
			if(enu == null) return true;
			return !enu.MoveNext();
		}
	}
	public class DetailNodeInfo {
		string caption;
		object list;
		DataColumnInfo[] columns;
		DetailNodeInfo[] nodes = new DetailNodeInfo[0];
		public DetailNodeInfo(string caption) {
			this.caption = caption;
			this.list = null;
			this.columns = null;
		}
		public string Caption { get { return caption; } set { caption = value; } }
		public DataColumnInfo[] Columns { get { return columns; } set { columns = value; } }
		public DetailNodeInfo[] Nodes { get { return nodes; } set { nodes = value; } }
		public bool HasChildren { get { return Nodes != null && Nodes.Length > 0; } }
		public object List { get { return list; } set { list = value; } }
		public DetailNodeInfo Find(string name) { return Find(Nodes, name); }
		public static DetailNodeInfo Find(DetailNodeInfo[] nodes, string name) {
			if(nodes == null) return null;
			for(int n = 0; n < nodes.Length; n++) {
				if(nodes[n].Caption == name) return nodes[n];
			}
			return null;
		}
	}
#if !SL && !DXPORTABLE
	public interface IStreamSupport {
		void Save(TypedBinaryWriter writer);
	}
#endif
}

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
using DevExpress.Data.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using DevExpress.Utils;
using System.Data;
using DevExpress.Data.Access;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
using System.Globalization;
namespace DevExpress.Data {
	public class UnboundErrorObject {
		static string displayText = "#Err";
		public static readonly UnboundErrorObject Value = new UnboundErrorObject();
		public static string DisplayText { get { return displayText; } set { displayText = value; } }
		UnboundErrorObject() { }
		public override string ToString() {
			return DisplayText;
		}
	}
	public class DataColumnInfo : IDataColumnInfo {
		Type type;
		string name;
		bool allowSort = true;
		PropertyDescriptor propertyDescriptor;
		int columnIndex = -1, dataIndex = -1;
		bool visible = true, unbound = false;
		object tag = null;
		internal DataColumnInfoCollection owner;
		IComparer customComparer;
		public DataColumnInfo(PropertyDescriptor descriptor) {
			SetPropertyDescriptor(descriptor);
		}
		public virtual IComparer CustomComparer {   
			get { return customComparer; }
			set { customComparer = value; }
		}
		internal void SetOwner(DataColumnInfoCollection owner) {
			this.owner = owner;
		}
		public Type GetDataType() {
			Type columnDataType = Type;
#if DXWhidbey
			if(columnDataType != null) {
				Type nullableUnderlyingType = Nullable.GetUnderlyingType(columnDataType);
				if(nullableUnderlyingType != null) {
					columnDataType = nullableUnderlyingType;
				}
			}
#endif
			return columnDataType;
		}
		public object ConvertValue(object val) { return ConvertValue(val, false); }
		public object ConvertValue(object val, bool useCurrentCulture) { return ConvertValue(val, useCurrentCulture, null); }
		public object ConvertValue(object val, bool useCurrentCulture, DataControllerBase controller) {
			if(val == null && IsDataViewDescriptor) val = DBNull.Value;
			Type columnDataType = GetDataType();
			bool isNull = val == null || DBNull.Value.Equals(val);
			Type valueType = !isNull ? val.GetType() : null;
			if(isNull || columnDataType == null || columnDataType.IsAssignableFrom(valueType))
				return val;
			if(IsDataViewDescriptor && columnDataType.IsEnum() && Enum.GetUnderlyingType(columnDataType) == valueType)
				return val;
			TypeConverter conv = PropertyDescriptor != null ? PropertyDescriptor.Converter : null;
			if(controller != null)
				conv = controller.GetActualTypeConverter(conv, PropertyDescriptor);
			if(conv != null && conv.CanConvertFrom(valueType)) {
				try {
					if(useCurrentCulture)
						return conv.ConvertFrom(val);
					else
						return conv.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, val);
				}
				catch {
					if(val is string && ((string)val).Length == 0 && !NullableHelpers.CanAcceptNull(this.Type))
						return IsDataViewDescriptor ? DBNull.Value : null;
				}
			}
			if(useCurrentCulture)
				return Convert.ChangeType(val, columnDataType, CultureInfo.CurrentCulture);
			else
				return Convert.ChangeType(val, columnDataType, System.Globalization.CultureInfo.InvariantCulture);
		}
		public bool Visible { get { return visible; } set { visible = value; } }
		public int DataIndex { get { return dataIndex; } set { dataIndex = value; } }
		public PropertyDescriptor PropertyDescriptor { get { return propertyDescriptor; } }
		string caption = null;
		public string Caption {
			get { 
				if(caption == null) caption = MasterDetailHelper.GetDisplayName(PropertyDescriptor);
				return caption;
			}
		}
		public string Name { get { return name; } }
		public Type Type { get { return type; } }
		public bool ReadOnly { get { return PropertyDescriptor.IsReadOnly; } }
		public bool Browsable { get { return PropertyDescriptor.IsBrowsable; } }
		public bool AllowSort { get { return allowSort; } set { allowSort = false; } }
		public bool Unbound { get { return unbound; } }
		public bool UnboundWithExpression { get { return Unbound && !string.IsNullOrEmpty(UnboundExpression); } }
		public string UnboundExpression {
			get {
				if(!Unbound || UnboundDescriptor == null) return string.Empty;
				return UnboundDescriptor.UnboundInfo.Expression;
			}
		}
		bool isDataViewDescriptorCore;
		public bool IsDataViewDescriptor {
			get { return isDataViewDescriptorCore; }
		}
		public object Tag { get { return tag; } set { tag = value; } }
		protected internal int ColumnIndex { get { return columnIndex; } set { columnIndex = value; } }
		public int Index { get { return columnIndex; } }
		protected void SetAsUnbound() {
			this.unbound = true;
		}
		protected internal virtual void SetPropertyDescriptor(PropertyDescriptor descriptor) {  
			this.unbound = false;
			this.propertyDescriptor = descriptor;
			this.visible = descriptor.IsBrowsable;
			this.type = PropertyDescriptor.PropertyType;
			this.name = descriptor.Name;
			Type ptype = type;
#if DXWhidbey
			Type baseType = Nullable.GetUnderlyingType(type);
			if(baseType != null) ptype = baseType;
#endif
			this.isDataViewDescriptorCore = descriptor.GetType().FullName == "System.Data.DataColumnPropertyDescriptor";
			Type[] types = ptype.GetInterfaces();
			this.allowSort = (descriptor is DevExpress.Data.Access.SimpleListPropertyDescriptor) || (types != null && Array.IndexOf(types, typeof(IComparable)) != -1);
			DevExpress.Data.Access.UnboundPropertyDescriptor unboundDescriptor = PropertyDescriptor as DevExpress.Data.Access.UnboundPropertyDescriptor;
			if(unboundDescriptor != null) {
				this.unbound = true;
				unboundDescriptor.SetInfo(this);
			}
		}
		DevExpress.Data.Access.UnboundPropertyDescriptor UnboundDescriptor {
			get { return PropertyDescriptor as DevExpress.Data.Access.UnboundPropertyDescriptor; }
		}
		public override string ToString() {
			return "{DataColumnInfo: " + Name + "}";
		}
		#region IDataColumnInfo Members
		List<IDataColumnInfo> IDataColumnInfo.Columns {
			get { return new List<IDataColumnInfo>(); }
		}
		string IDataColumnInfo.UnboundExpression {
			get {
				return UnboundExpression;;
			}
		}
		string IDataColumnInfo.Caption { get { return Caption; } }
		string IDataColumnInfo.FieldName { get { return Name; } }
		string IDataColumnInfo.Name { get { return Name; } }
		DataControllerBase IDataColumnInfo.Controller { get { return null; } }
		Type IDataColumnInfo.FieldType { get { return Type;} }
		#endregion
	}
	public class ComplexColumnInfoCollection : CollectionBase {
		public event CollectionChangeEventHandler CollectionChanged;
		public int Add(string name) {
			if(name.IndexOf(".") < 0) return -1;
			int index = IndexOf(name);
			if(index > -1) return index;
			return List.Add(new ComplexColumnInfo(name));
		}
		public int IndexOf(string name) { 
			for(int n = 0; n < Count; n++) {
				if(this[n].Name == name) return n;
			}
			return -1;
		}
		public ComplexColumnInfo this[int index] { get { return List[index] as ComplexColumnInfo; } }
		public void Remove(string name) { List.Remove(name); }
		protected override void OnInsertComplete(int index, object item) {
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int index, object item) {
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
	}
	public class ComplexColumnInfo {
		string name;
		public ComplexColumnInfo(string name) {
			this.name = name;
		}
		public string Name { get { return name; } }
	}
	public class DataColumnInfoCollection : CollectionBase, IList<DataColumnInfo> {
		Dictionary<string, DataColumnInfo> nameHash;
		public DataColumnInfoCollection() {
			this.nameHash = new Dictionary<string, DataColumnInfo>();
		}
		public bool HasUnboundColumns {
			get {
				foreach(DataColumnInfo column in this) {
					if(column.Unbound) return true;
				}
				return false;
			}
		}
		public DataColumnInfo[] ToArray() {
			return (DataColumnInfo[])InnerList.ToArray(typeof(DataColumnInfo));
		}
		public int GetColumnIndex(string fieldName) {
			DataColumnInfo col = this[fieldName];
			return col == null ? -1 : col.Index;
		}
		public DataColumnInfo this[int index] { 
			get { 
				if(index < 0 || index >= Count) return null;
				return (DataColumnInfo)List[index];
			} 
		}
		public DataColumnInfo this[string name] { 
			get {
				if(string.IsNullOrEmpty(name)) return null;
				DataColumnInfo result;
				NameHash.TryGetValue(name, out result);
				return result;
			} 
		}
		public void Add(DataColumnInfo columnInfo) {
			List.Add(columnInfo);
		}
		public DataColumnInfo Add(PropertyDescriptor descriptor) {
			DataColumnInfo column = new DataColumnInfo(descriptor);
			Add(column);
			return column;
		}
		public Dictionary<DataColumnInfo, object> ConvertValues(Dictionary<string, object> columnValues) {
			if (columnValues.Count == 0) return null;
			Dictionary<DataColumnInfo, object> values = new Dictionary<DataColumnInfo, object>();
			foreach (string columnName in columnValues.Keys) {
				DataColumnInfo column = this[columnName];
				if (column == null) return null;
				object value = columnValues[columnName];
				try {
					value = column.ConvertValue(value);
				} catch {
					return null;
				}
				values.Add(column, value);
			}
			return values;
		}
		protected override void OnInsertComplete(int position, object item) {
			DataColumnInfo info = item as DataColumnInfo;
			info.SetOwner(this);
			NameHash[info.Name] = info;
			UpdateColumnIndexes();
		}
		protected override void OnRemoveComplete(int position, object item) {
			DataColumnInfo col = item as DataColumnInfo;
			if(NameHash.ContainsKey(col.Name)) NameHash.Remove(col.Name);
			UpdateColumnIndexes();
		}
		protected override void OnClearComplete() {
			NameHash.Clear();
		}
		protected Dictionary<string, DataColumnInfo> NameHash { get { return nameHash; } }
		void UpdateColumnIndexes() {
			for(int i = 0; i < Count; i ++)
				this[i].ColumnIndex = i;
		}
		internal void ValidateColumnInfo(DataColumnInfo columnInfo) {
			var columnInfoAtIndex = this[columnInfo.Index];
			if(!ReferenceEquals(columnInfo, columnInfoAtIndex)) {
				string msg1;
				if(columnInfoAtIndex == null) {
					msg1 = "Columns collection did not contain column at index " + columnInfo.Index + ".";
				} else {
					msg1 = "Columns collection contain different instance of column at index " + columnInfo.Index + " ('" + columnInfoAtIndex.Name + "', " + columnInfoAtIndex.Index + ").";
				}
				var columnInfoSameName = this[columnInfo.Name];
				string msg2;
				if(columnInfoSameName == null) {
					msg2 = "Columns collection did not contain column '" + columnInfo.Name + "'.";
				} else {
					msg2 = "Columns collection contain column '" + columnInfoSameName.Name + "' with index " + columnInfoSameName.Index + ".";
				}
				string msg0 = "Invalid columnInfo('" + columnInfo.Name + "', " + columnInfo.Index + ");";
				throw new ArgumentException(string.Join(" ", msg0, msg1, msg2), "columnInfo");
			}
		}
		int IList<DataColumnInfo>.IndexOf(DataColumnInfo item) { return InnerList.IndexOf(item); }
		void IList<DataColumnInfo>.Insert(int index, DataColumnInfo item) {
			InnerList.Insert(index, item);
		}
		void IList<DataColumnInfo>.RemoveAt(int index) {
			InnerList.RemoveAt(index);
		}
		DataColumnInfo IList<DataColumnInfo>.this[int index] { get { return InnerList[index] as DataColumnInfo; } set { InnerList[index] = value; } }
		void ICollection<DataColumnInfo>.Add(DataColumnInfo item) {
			InnerList.Add(item);
		}
		bool ICollection<DataColumnInfo>.Contains(DataColumnInfo item) {
			return InnerList.Contains(item);
		}
		void ICollection<DataColumnInfo>.CopyTo(DataColumnInfo[] array, int arrayIndex) {
			InnerList.CopyTo(array, arrayIndex);
		}
		bool ICollection<DataColumnInfo>.IsReadOnly { get { return InnerList.IsReadOnly; } }
		bool ICollection<DataColumnInfo>.Remove(DataColumnInfo item) {
			if (InnerList.Contains(item)) {
				InnerList.Remove(item);
				return true;
			}
			else
				return false;
		}
		class TypedEnumerator : IEnumerator<DataColumnInfo> {
			IEnumerator enumerator;
			public TypedEnumerator(IEnumerator enumerator) {
				this.enumerator = enumerator;
			}
			public DataColumnInfo Current { get { return enumerator.Current as DataColumnInfo; } }
			public void Dispose() {
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null) {
					disposable.Dispose();
				}
			}
			object IEnumerator.Current {
				get { return enumerator.Current; }
			}
			public bool MoveNext() {
				return enumerator.MoveNext();
			}
			public void Reset() {
				enumerator.Reset();
			}
		}
		IEnumerator<DataColumnInfo> IEnumerable<DataColumnInfo>.GetEnumerator() {
			return new TypedEnumerator(InnerList.GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
	}
	public class VisibleIndexHeightInfo {
		static readonly int[] zeroHeight = new int[0];
		static readonly int[] selfHeight = new int[0];
		int[][] map;
		bool dirty, allowFixedGroups = true;
		VisibleIndexCollection source;
		public VisibleIndexHeightInfo(VisibleIndexCollection source) {
			this.source = source;
		}
		public void Reset() {
			this.map = null;
			this.dirty = true;
		}
		public bool IsZeroHeight(int[] height) {
			return object.ReferenceEquals(height, zeroHeight);
		}
		public bool IsSelfHeight(int[] height) {
			return object.ReferenceEquals(height, selfHeight);
		}
		public int[][] Map { 
			get {
				if(dirty) Calculate();
				return map; 
			} 
		}
		public bool AllowFixedGroups {
			get { return allowFixedGroups; }
			set {
				if(AllowFixedGroups == value) return;
				allowFixedGroups = value;
				Reset();
			}
		}
		public void Calculate() {
			this.dirty = false;
			this.map = new int[source.Count][];
			for(int n = 0; n < map.Length; n++) {
				map[n] = selfHeight;
			}
			if(!AllowFixedGroups || source.GroupInfo.Count == 0) return;
			Stack<GroupRowInfo> groupStack = new Stack<GroupRowInfo>();
			for(int n = 0; n < source.Count; n++) {
				bool nextRowValid = n + 1 < source.Count;
				int handle = source[n];
				int nextHandle = nextRowValid ? source[n + 1] : DataController.InvalidRow;
				int[] current = null;
				GroupRowInfo group = null, nextGroup = null;
				if(handle < 0) group = source.GroupInfo.GetGroupRowInfoByHandle(handle);
				if(nextRowValid && nextHandle < 0) nextGroup = source.GroupInfo.GetGroupRowInfoByHandle(nextHandle);
				if(handle >= 0 || !group.Expanded) {
					if(nextHandle >= 0 || (nextGroup != null && group != null && nextGroup.Level == group.Level)) {
						map[n] = selfHeight;
						continue;
					}
					if(groupStack.Count == 0) { 
						map[n] = selfHeight;
						break;
					}
					int size = 1 + (groupStack.Count - (nextGroup == null ? 0 : nextGroup.Level));
					current = new int[size];
					current[0] = handle;
					for(int g = 1; g < size; g++) {
						if(groupStack.Count == 0) break;
						current[g] = groupStack.Pop().Handle;
					}
					map[n] = current;
					continue;
				}
				if(handle < 0) {
					map[n] = group == null || group.Expanded ? zeroHeight : selfHeight;
					if(group == null || !group.Expanded) continue;
					groupStack.Push(group);
				}
			}
		}
	}
	public class VisibleIndexCollection : ICollection {
		int[] list, scrollableIndexes;
		int count = 0, scrollableIndexesCount = 0;
		bool expandedGroupsIncludedInScrollableIndexes;
		DataControllerBase controller;
		GroupRowInfoCollection groupInfo;
		bool isDirty = false, modified = false;
		Dictionary<int, bool> singleItems = null;
		VisibleIndexHeightInfo scrollHeightInfo;
		public VisibleIndexCollection(DataControllerBase controller, GroupRowInfoCollection groupInfo) {
			this.controller = controller;
			this.groupInfo = groupInfo;
			this.list = new int[0];
			this.modified = true;
			this.scrollableIndexes = new int[0];
			this.scrollHeightInfo = new VisibleIndexHeightInfo(this);
		}
		public bool IsModified { get { return modified; } }
		public void ResetModified() { modified = false; }
		public bool IsDirty { get { return isDirty; } }
		public void SetDirty(bool value) { this.isDirty = value; }
		public void SetDirty() { this.isDirty = true; }
		public VisibleIndexHeightInfo ScrollHeightInfo { get { return scrollHeightInfo; } }
		public void Clear() {
			ClearCore(true);
		}
		protected void ClearCore(bool recreateList) {
			this.modified = true;
			scrollHeightInfo.Reset();
			if(recreateList) {
				int maxCount = GetMaxCount();
				if(this.list.Length < maxCount) this.list = new int[maxCount];
			}
			this.count = 0;
			this.singleItems = null;
		}
		public bool ExpandedGroupsIncludedInScrollableIndexes {
			get { return expandedGroupsIncludedInScrollableIndexes; }
			set {
				if(expandedGroupsIncludedInScrollableIndexes == value)
					return;
				expandedGroupsIncludedInScrollableIndexes = value;
				modified = true;
			}
		}
		public int[] ScrollableIndexes {
			get {
				if(modified) BuildScrollableIndexes();
				return scrollableIndexes;
			}
		}
		public int ScrollableIndexesCount {
			get {
				if(modified) BuildScrollableIndexes();
				return scrollableIndexesCount;
			}
		}
		struct GroupInfoVisibleIndexPair {
			public GroupRowInfo Group;
			public int VisibleIndex;
		}
		protected virtual void BuildScrollableIndexes() {
			this.modified = false;
			this.scrollableIndexesCount = 0;
			if(GroupInfo.Count == 0) {
				return;
			}
			if(scrollableIndexes.Length < Count) scrollableIndexes = new int[Count];
			List<GroupInfoVisibleIndexPair> lastGroups = new List<GroupInfoVisibleIndexPair>(groupInfo.LevelCount);
			for(int n = 0; n < Count; n++) {
				int handle = list[n];
				if(handle < 0) {
					GroupRowInfo group = GroupInfo.GetGroupRowInfoByHandle(handle);
					if(group != null) {
						PutLastGroupsIntoScrollableIndexes(lastGroups, group.Level);
						if(group.Expanded) {
							if(ExpandedGroupsIncludedInScrollableIndexes) {
								lastGroups.Add(new GroupInfoVisibleIndexPair() { Group = group, VisibleIndex = n });
							}
							continue;
						}
					}
				}
				scrollableIndexes[scrollableIndexesCount++] = n;
			}
			PutLastGroupsIntoScrollableIndexes(lastGroups, 0);
		}
		void PutLastGroupsIntoScrollableIndexes(List<GroupInfoVisibleIndexPair> lastGroups, int tillLevel) {
			if(ExpandedGroupsIncludedInScrollableIndexes) {
				for(int i = lastGroups.Count - 1; i >= tillLevel; i--) {
					scrollableIndexes[scrollableIndexesCount++] = lastGroups[i].VisibleIndex;
					lastGroups.RemoveAt(i);
				}
			}
		}
		public int ConvertIndexToScrollIndex(int index, bool allowFixedGroups) {
			if(!allowFixedGroups || (index == 0 && !ExpandedGroupsIncludedInScrollableIndexes)) return index;
			if(IsEmpty) return index;
			int i = Array.IndexOf<int>(ScrollableIndexes, index, 0, ScrollableIndexesCount);
			if((i < 0) && !ExpandedGroupsIncludedInScrollableIndexes) {
				return FindScrollableIndex(index);
			}
			return i;
		}
		int FindScrollableIndex(int index) {
			int count = ScrollableIndexesCount;
			if(count < 1) return count;
			if(ScrollableIndexes[0] >= index) return ScrollableIndexes[0];
			if(ScrollableIndexes[count - 1] <= index) return ScrollableIndexes[count - 1];
			for(int n = 0; n < count; n++) {
				int i = ScrollableIndexes[n];
				if(i >= index) return n;
			}
			return index;
		}
		public int ConvertScrollIndexToIndex(int scrollIndex, bool allowFixedGroups) {
			if(!allowFixedGroups) return scrollIndex;
			if(IsEmpty || scrollIndex >= ScrollableIndexesCount) return scrollIndex;
			return ScrollableIndexes[scrollIndex];
		}
		protected virtual int GetMaxCount() {
			return GroupInfo.Count + Controller.ListSourceRowCount + 1;
		}
		public int Count { get { return count; } }
		protected DataControllerBase Controller { get { return controller; } }
		protected internal virtual GroupRowInfoCollection GroupInfo { get { return groupInfo; } set { groupInfo = value; } }
		public int this[int visibleIndex] { get { return list[visibleIndex]; } }
		public void Add(int controllerRowHandle) {
			if(count == list.Length) {
				ExtendArray();
			}
			list[count ++] = controllerRowHandle;
			this.modified = true;
			scrollHeightInfo.Reset();
		}
		void ExtendArray() {
			int[] newList = new int[Math.Max(2000, list.Length * 2)];
			list.CopyTo(newList, 0);
			this.list = newList;
		}
		public bool Contains(int controllerRowHandle) { return IndexOf(controllerRowHandle) != -1; }
		public int IndexOf(int controllerRowHandle)  { return Array.IndexOf(this.list, controllerRowHandle, 0, Count); }
		public bool IsEmpty { get { return Count == 0 && GroupInfo.LevelCount == 0; } }
		public int GetHandle(int visibleIndex) {
			if(IsEmpty) return visibleIndex;
			if(visibleIndex >= Count) return DataController.InvalidRow;
			return this[visibleIndex];
		}
		public void CopyTo(Array array, int index) { 
			Array.Copy(this.list, index, array, 0, Count - index);
		}
		public bool IsSynchronized { get { return true; } }
		public object SyncRoot { get { return this;	} }
		public IEnumerator GetEnumerator() { return new ArrayEnumerator(this); }
		public void BuildVisibleIndexes(int visibleCount, bool allowNonGroupedList, bool expandAll) {
			this.singleItems = null;
			SetDirty(false);
			if(GroupInfo.Count == 0) {
				ClearCore(allowNonGroupedList);
				if(!allowNonGroupedList) return;
				DataController controller = Controller as DataController;
				if(controller == null) return;
				for(int n = 0; n < visibleCount; n++) {
					Add(controller.GetControllerRowHandle(n));
				}
				return;
			}
			Clear();
			List<GroupRowInfo> rootGroups = GetRootGroups();
			for(int i = 0; i < rootGroups.Count; i ++) {
				BuildVisibleIndexesEx(rootGroups[i], expandAll, true);
			}
		}
		protected virtual List<GroupRowInfo> GetRootGroups() {
			List<GroupRowInfo> res = new List<GroupRowInfo>();
			for(int i = 0; i < GroupInfo.Count; i++) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(groupRow.Level == 0) res.Add(groupRow);
			}
			return res;
		}
		public void BuildVisibleIndexes(GroupRowInfo groupRow, bool expandAll) {
			BuildVisibleIndexesEx(groupRow, expandAll, true);
		}
		public bool IsSingleGroupRow(int controllerDataRowHanlde) {
			if(singleItems == null) return false;
			return singleItems.ContainsKey(controllerDataRowHanlde);
		}
		protected void BuildVisibleIndexesEx(GroupRowInfo groupRow, bool expandAll, bool expanded) {
			expanded = expanded || groupRow.Level == GroupInfo.AlwaysVisibleLevelIndex;
			if(Controller.AllowPartialGrouping) {
				if(groupRow.ChildControllerRowCount == 1) {
					if(singleItems == null) singleItems = new Dictionary<int, bool>();
					singleItems[groupRow.ChildControllerRow] = true;
					Add(groupRow.ChildControllerRow);
					return;
				}
			}
			if(expanded) Add(groupRow.Handle);
			if(groupRow.Expanded || expandAll || groupRow.Level < GroupInfo.AlwaysVisibleLevelIndex) {
				if(GroupInfo.IsLastLevel(groupRow)) {
					AddVisibleDataRows(groupRow);
				} else {
					int tolalChildGroupCount = GroupInfo.GetTotalChildrenGroupCount(groupRow);
					for(int i = 1; i <= tolalChildGroupCount; i ++) {
						GroupRowInfo childGroupRow = GroupInfo[groupRow.Index + i];
						if(childGroupRow.Level - 1 == groupRow.Level)
							BuildVisibleIndexesEx(childGroupRow, expandAll, (expanded && groupRow.Expanded) || expandAll);
					}
				}
			}
		}
		protected void AddVisibleDataRows(GroupRowInfo rowInfo) {
			for(int i = 0; i < rowInfo.ChildControllerRowCount; i ++)
				Add(rowInfo.ChildControllerRow + i);
		}
		class ArrayEnumerator : IEnumerator {
			VisibleIndexCollection indexes;
			int position = -1;
			public ArrayEnumerator(VisibleIndexCollection indexes) { 
				this.indexes = indexes;
			}
			object IEnumerator.Current { get { return indexes[position]; } }
			bool IEnumerator.MoveNext() { 
				if(position == indexes.Count - 1) return false;
				position ++;
				return true;
			}
			void IEnumerator.Reset() {
				this.position = -1;
			}
		}
	}
	public class VisibleIndexCollection2 : CollectionBase {
		DataController controller;
		public VisibleIndexCollection2(DataController controller) {
			this.controller = controller;
		}
		protected DataController Controller { get { return controller; } }
		public int this[int visibleIndex] { get { return (int)InnerList[visibleIndex]; } }
		public void Add(int controllerRowHandle) { InnerList.Add(controllerRowHandle); }
		public bool Contains(int controllerRowHandle) { return InnerList.Contains(controllerRowHandle); }
		public int IndexOf(int controllerRowHandle)  { return InnerList.IndexOf(controllerRowHandle); }
		public bool IsEmpty { get { return Count == 0 && Controller.GroupedColumnCount == 0; } }
		public int GetHandle(int visibleIndex) {
			if(IsEmpty) return visibleIndex;
			if(visibleIndex >= Count) return DataController.InvalidRow;
			return this[visibleIndex];
		}
	}
	public class GroupRowInfo {
		public byte Level;
		public bool Expanded;
		public GroupRowInfo ParentGroup;
		public int ChildControllerRow;
		public int ChildControllerRowCount;
		public int Index;
		Hashtable summary = null;
		public GroupRowInfo() : this(0, 0, null) { }
		public GroupRowInfo(byte level, int childControllerRow, GroupRowInfo parentGroup) { 
			this.Level = level;
			this.ChildControllerRow = childControllerRow;
			this.ChildControllerRowCount = 1;
			this.Expanded = false;
			this.ParentGroup = parentGroup;
		}
		public GroupRowInfo RootGroup {
			get {
				if(ParentGroup == null) return this;
				GroupRowInfo g = this;
				while(g != null && g.ParentGroup != null) {
					g = g.ParentGroup;
				}
				return g;
			}
		}
		public override string ToString() {
			return string.Format("{0} {3} ChildRow:{1} Count: {2}", Handle, ChildControllerRow, ChildControllerRowCount,
				new string('-', Level));
		}
		public int Handle { get { return GroupIndexToHandle(Index); } }
		public bool IsVisible { 
			get {
				GroupRowInfo groupRow = ParentGroup;
				if(groupRow == null) return true;
				while(groupRow != null) {
					if(!groupRow.Expanded) return false;
					groupRow = groupRow.ParentGroup;
				}
				return true;
			}
		}
		public int GetVisibleIndexOfControllerRow(int controllerRow) {
			if(!ContainsControllerRow(controllerRow)) return -1;
			return controllerRow - ChildControllerRow;
		}
		public bool ContainsControllerRow(int controllerRow) {
			return controllerRow >= ChildControllerRow && (controllerRow < ChildControllerRow + ChildControllerRowCount);
		}
		public static bool IsGroupRowHandle(int controllerRowHandle) {
			return (controllerRowHandle < 0 && controllerRowHandle != DataController.InvalidRow);
		}
		public static int GroupIndexToHandle(int groupIndex) { return -1 - groupIndex; }
		public static int HandleToGroupIndex(int handle) {
			if(handle == DataController.InvalidRow) return DataController.InvalidRow;
			return Math.Abs(handle) - 1; 
		}
		protected internal Hashtable Summary { get { return summary; } }
		public object GetSummaryValue(SummaryItemBase item) {
			if(Summary == null) return null;
			return Summary[item.Key];
		}
		internal object GetSummaryValue(SummaryItemBase item, out bool isValid) {
			if(Summary == null) {
				isValid = false;
				return null;
			}
			isValid = Summary.ContainsKey(item.Key);
			return Summary[item.Key];
		}
		public void SetSummaryValue(SummaryItemBase item, object value) {
			SetSummaryValueCore(item.Key, value);
		}
		protected void SetSummaryValueCore(object key, object value) {
			if(Summary == null) this.summary = new Hashtable();
			Summary[key] = value;
		}
		public void ClearSummaryItem(SummaryItemBase item) {
			if(Summary == null || !Summary.Contains(item.Key)) return;
			Summary.Remove(item.Key);
		}
		public virtual void ClearSummary() {
			if(Summary != null) {
				this.summary.Clear();
				this.summary = null;
			}
		}
		internal GroupRowInfo GetParentGroupAtLevel(int level) {
			GroupRowInfo group = this;
			while(group != null && group.Level != level) {
				group = group.ParentGroup;
			}
			return group;
		}
		public virtual object GroupValue { get; set; }
	}
	public class GroupRowInfoCollection : Collection<GroupRowInfo>, IDisposable { 
		DataControllerBase controller;
		bool autoExpandAllGroups;
		int lastExpandableLevel;
		DataColumnSortInfoCollection sortInfo;
		VisibleListSourceRowCollection visibleListSourceRows;
		int alwaysVisibleLevelIndex;
		public GroupRowInfoCollection(DataControllerBase controller, DataColumnSortInfoCollection sortInfo, VisibleListSourceRowCollection visibleListSourceRows)
			: base(new List<GroupRowInfo>()) {
			this.controller = controller;
			this.sortInfo = sortInfo;
			this.visibleListSourceRows = visibleListSourceRows;
			this.autoExpandAllGroups = false;
			this.lastExpandableLevel = -1;
			this.alwaysVisibleLevelIndex = -1;
		}
		protected List<GroupRowInfo> ListCore { get { return (List<GroupRowInfo>)Items; } }
		public virtual void Dispose() {
			this.controller = null;
			this.sortInfo = null;
			Clear();
		}
		protected DataControllerBase Controller { get { return controller; } }
		protected virtual DataColumnSortInfoCollection SortInfo { get { return sortInfo; } }
		protected virtual GroupRowInfo CreateGroupRowInfo(byte level, int childControllerRow, GroupRowInfo parentGroupRow) {
			var res = new GroupRowInfo(level, childControllerRow, parentGroupRow);
			res.GroupValue = Controller.GetGroupRowKeyValueInternal(res);
			return res;
		}
		public bool MakeVisible(GroupRowInfo groupRow, bool showChildren) {
			bool changed = false;
			if(showChildren && !groupRow.Expanded) {
				Controller.MakeGroupRowVisible(groupRow);
				changed = true;
			}
			GroupRowInfo parent = groupRow.ParentGroup;
			while(parent != null) {
				if(!parent.Expanded) {
					Controller.MakeGroupRowVisible(parent);
					parent.Expanded = true;
					changed = true;
				}
				parent = parent.ParentGroup;
			}
			return changed;
		}
		public virtual VisibleListSourceRowCollection VisibleListSourceRows { get { return visibleListSourceRows; } } 
		public bool AutoExpandAllGroups { get { return autoExpandAllGroups; } set { autoExpandAllGroups = value;}}
		public bool AllowPartialGrouping { get; set; }
		public int LastExpandableLevel { get { return lastExpandableLevel; } set { lastExpandableLevel = value;}}
		public bool IsGrouped { get { return SortInfo.GroupCount > 0; } }
		public int LevelCount { get { return SortInfo.GroupCount; } }
		public int AlwaysVisibleLevelIndex { get { return alwaysVisibleLevelIndex; } set { alwaysVisibleLevelIndex = value; } }
		public virtual GroupRowInfo Add(byte level, int ChildControllerRow, GroupRowInfo parentGroup) {
			GroupRowInfo groupRow = CreateGroupRowInfo(level, ChildControllerRow, parentGroup);
			ChangeGroupRowExpanded(groupRow, AutoExpandAllGroups);
			Add(groupRow);
			return groupRow;
		}
		public void ClearSummary() {
			for(int n = Count - 1; n >= 0; n--) {
				this[n].ClearSummary();
			}
		}
		public void MoveFromEndToMiddle(int startIndex, int count, int moveTo) {
			int index = startIndex + count - 1;
			for(int i = 0; i < count; i ++) {
				GroupRowInfo row = this[index];
				RemoveAt(index);
				Insert(moveTo, row);
			}
			UpdateIndexes();
		}
		public bool IsLastLevel(GroupRowInfo groupRow) { return groupRow.Level + 1 == LevelCount; }
		public int GetChildCount(int groupRowHandle) { return GetChildCount(GetGroupRowInfoByHandle(groupRowHandle)); }
		public int GetChildCount(GroupRowInfo groupRow) {
			if(groupRow == null) return 0;
			if(IsLastLevel(groupRow)) return groupRow.ChildControllerRowCount;
			return GetChildrenGroupCount(groupRow);
		}
		public int GetChildRow(int groupRowHandle, int childIndex) { return GetChildRow(GetGroupRowInfoByHandle(groupRowHandle), childIndex); }
		public int GetChildRow(GroupRowInfo groupRow, int childIndex) { 
			if(childIndex < 0 || groupRow == null || groupRow.ChildControllerRowCount == 0) return DataController.InvalidRow;
			if(IsLastLevel(groupRow)) {
				return childIndex >= groupRow.ChildControllerRowCount ? DataController.InvalidRow : groupRow.ChildControllerRow + childIndex;
			}
			List<GroupRowInfo> list = new List<GroupRowInfo>();
			GetChildrenGroups(groupRow, list);
			return childIndex >= list.Count ? DataController.InvalidRow : ((GroupRowInfo)list[childIndex]).Handle;
		}
		public int GetParentRow(int groupRowHandle) { return GetParentRow(GetGroupRowInfoByHandle(groupRowHandle)); }
		public int GetParentRow(GroupRowInfo groupRow) { 
			return groupRow == null ? DataController.InvalidRow : 
				(groupRow.ParentGroup == null ? DataController.InvalidRow : groupRow.ParentGroup.Handle);
		}
		public virtual int RootGroupCount { 
			get {
				int count = 0;
				for(int n = Count - 1; n >= 0; n --) {
					if(this[n].Level == 0) count ++;
				}
				return count;
			}
		}
		public GroupRowInfo GetRootGroup(int index) {
			int count = 0;
			for(int n = 0; n < Count; n ++) {
				GroupRowInfo info = this[n];
				if(info.Level == 0 && count ++ == index) return info;
			}
			return null;
		}
		public void GetChildrenGroups(GroupRowInfo groupRow, IList<GroupRowInfo> list) {
			GetChildrenGroups(groupRow, list, groupRow == null ? 0 : groupRow.Level + 1);
		}
		public void GetChildrenGroups(GroupRowInfo groupRow, IList<GroupRowInfo> list, int level) {
			int count, n = (groupRow == null ? 0 : groupRow.Level + 1);
			list.Clear();
			if(groupRow == null) {
				count = Count;
				n = 0;
			} else {
				n = groupRow.Index + 1;
				count = GetTotalChildrenGroupCount(groupRow) + n;
			}
			for(; n < count; n ++) {
				GroupRowInfo info = this[n];
				if(info.Level == level) list.Add(info);
			}
		}
		public int GetTotalGroupsCountByLevel(int level) {
			int count = 0;
			for(int i = 0; i < Count; i++) {
				if(this[i].Level == level) count++;
			}
			return count;
		}
		public int GetTotalChildrenGroupCount(GroupRowInfo groupRow) {
			if(groupRow == null) return 0;
			int count = 0;
			for(int i = groupRow.Index + 1; i < Count; i++) {
				if(this[i].Level <= groupRow.Level) break;
				count ++;
			}
			return count;
		}
		public int GetChildrenGroupCount(GroupRowInfo groupRow) {
			if(groupRow == null) return 0;
			int count = 0;
			for(int i = groupRow.Index + 1; i < Count; i++) {
				if(this[i].Level <= groupRow.Level) break;
				if(this[i].Level - 1 == groupRow.Level) count++;
			}
			return count;
		}
		public bool ChangeExpandedLevel(int groupLevel, bool expanded, bool recursive) {
			bool hasChanges = false;
			for(int n = 0; n < Count; n++) {
				GroupRowInfo group = this[n];
				if(group.Level < groupLevel) continue;
				if(group.Level == groupLevel || recursive) {
					if(group.Expanded != expanded) {
						hasChanges = true;
						group.Expanded = expanded;
					}
				}
			}
			return hasChanges;
		}
		public bool ChangeExpanded(int groupRowHandle, bool expanded, bool recursive) {
			GroupRowInfo groupRow = GetGroupRowInfoByHandle(groupRowHandle);
			if(groupRow == null) return false;
			bool hasChanges = ChangeGroupRowExpanded(groupRow, expanded);
			if(recursive)
				hasChanges |= ChangeChildExpanded(groupRow, expanded);
			return hasChanges;
		}
		public bool ChangeAllExpanded(bool expanded) {
			bool hasChanges = false;
			for(int i = Count - 1; i >= 0; i--) {
				hasChanges |= ChangeGroupRowExpanded(this[i], expanded);
			}
			return hasChanges;
		}
		public bool ChangeLevelExpanded(int level, bool expanded) {
			bool hasChanges = false;
			for(int i = Count - 1; i >= 0; i--) {
				if(this[i].Level == level)
					hasChanges |= ChangeGroupRowExpanded(this[i], expanded);
			}
			return hasChanges;
		}
		public bool ChangeChildExpanded(GroupRowInfo groupRow, bool expanded) {
			if(IsLastLevel(groupRow)) return false;
			bool hasChanges = false;
			for(int i = GetTotalChildrenGroupCount(groupRow); i > 0; i--) {
				hasChanges |= ChangeGroupRowExpanded(this[groupRow.Index + i], expanded);
			}
			return hasChanges;
		}
		protected bool ChangeGroupRowExpanded(GroupRowInfo groupRow, bool expanded) {
			if(groupRow.Expanded == expanded) return false;
			if((LastExpandableLevel >= 0) && (groupRow.Level >= LastExpandableLevel)) return false;
			groupRow.Expanded = expanded;
			return true;
		}
		public void UpdateIndexes() {
			UpdateIndexes(0);
		}
		public void UpdateIndexes(int startFrom) {
			for(int i = Count - 1; i >= startFrom; i--)
				this[i].Index = i;
		}
		public GroupRowInfo GetGroupRowInfoByControllerRowHandle(int controllerRowHandle) {
			if(GroupRowInfo.IsGroupRowHandle(controllerRowHandle))
				return GetGroupRowInfoByHandle(controllerRowHandle);
			for(int i = Count - 1; i >= 0; i --) {
				GroupRowInfo groupRow = this[i];
				if(IsLastLevel(groupRow) && groupRow.ContainsControllerRow(controllerRowHandle)) {
					return groupRow;
				}
			}
			return null;
		}
		public GroupRowInfo GetGroupRowInfoByControllerRowHandleBinary(int controllerRowHandle) {
			if(GroupRowInfo.IsGroupRowHandle(controllerRowHandle))
				return GetGroupRowInfoByHandle(controllerRowHandle);
			int i = 0;
			int num = i + this.Count - 1; 
			while (i <= num)
			{
				int currentIndex = i + (num - i >> 1);
				GroupRowInfo group = this[currentIndex];
				int compareResult = 0;
				if(group.ChildControllerRow + group.ChildControllerRowCount - 1 < controllerRowHandle) {
					compareResult = -1;
				}
				else {
					if(group.ChildControllerRow > controllerRowHandle) {
						compareResult = 1;
					}
					else {
						if(!IsLastLevel(group)) compareResult = -1;
					}
				}
				if (compareResult == 0)return group;
				if (compareResult < 0)
				{
					i = currentIndex + 1;
				}
				else
				{
					num = currentIndex - 1;
				}
			}
			return null;
		}
		public GroupRowInfo GetGroupRowInfoByHandle(int groupRowHandle) {
			int groupRowIndex = GroupRowInfo.HandleToGroupIndex(groupRowHandle);
			if(groupRowIndex < 0 || groupRowIndex >= Count) return null;
			return this[groupRowIndex];
		}
		public virtual void ReverseLevel(int level) {
			if(!ReverseLevelCore(level)) return;
			UpdateVisibleListSourceRowCollection();
			UpdateChildControllerRows();
		}
		protected bool ReverseLevelCore(int level) {
			if(level < 0 || level >= LevelCount) return false;
			ReverseGroups(level);
			UpdateIndexes();
			return true;
		}
		void ReverseGroups(int level) {
			List<GroupRowInfo> list = new List<GroupRowInfo>();
			int i = 0;
			while(i < Count) {
				if(this[i].Level == level) {
					i += ReverseParentGroup(level > 0 ? this[i - 1] : null, list);
				} else {
					AddReverseGroup(this[i++], list);
				}
			}
			for(i = 0; i < list.Count; i++) {
				this[i] = list[i];
			}
		}
		int ReverseParentGroup(GroupRowInfo parentGroupRow, List<GroupRowInfo> list) {
			int count = parentGroupRow != null ? GetTotalChildrenGroupCount(parentGroupRow) : Count;
			int level = parentGroupRow != null ? parentGroupRow.Level + 1 : 0;
			int startIndex = list.Count;
			List<GroupRowInfo> rowList = new List<GroupRowInfo>();
			for(int i = 0; i < count; i ++) {
				if(this[startIndex + i].Level == level)
					rowList.Add(this[startIndex + i]);
			}
			for(int i = rowList.Count - 1; i >= 0; i --) {
				AddReverseGroup(rowList[i], list);
				AddReverceGroupChildren(rowList[i], list);
			}
			return count;
		}
		void AddReverseGroup(GroupRowInfo groupRow, List<GroupRowInfo> list) {
			list.Add(groupRow);
		}
		void AddReverceGroupChildren(GroupRowInfo groupRow, List<GroupRowInfo> list) {
			int count = GetTotalChildrenGroupCount(groupRow);
			for(int i = 0; i < count; i ++) {
				AddReverseGroup(this[groupRow.Index + 1 + i], list);
			}
		}
		void UpdateVisibleListSourceRowCollection() {
			int[] list = new int[VisibleListSourceRows.VisibleRowCount];
			int index = 0;
			for(int i = 0; i < Count; i ++) {
				if(IsLastLevel(this[i])) {
					for(int j = 0; j < this[i].ChildControllerRowCount; j ++) {
						list[index ++] = VisibleListSourceRows.GetListSourceRow(this[i].ChildControllerRow + j);
					}
				}
			}
			VisibleListSourceRows.Init(list, list.Length, VisibleListSourceRows.AppliedFilterExpression, VisibleListSourceRows.HasUserFilter);
		}
		void UpdateChildControllerRows() {
			int index = VisibleListSourceRows.VisibleRowCount;
			for(int i = Count - 1; i >= 0; i --) {
				if(IsLastLevel(this[i])) {
					index -= this[i].ChildControllerRowCount;
				} 
				this[i].ChildControllerRow = index;
			}
		}
		List<GroupRowInfo> delayedDeleteGroups;
		protected override void RemoveItem(int index) {
			GroupRowInfo group = this[index];
			base.RemoveItem(index);			
			if(this.delayedDeleteGroups != null) {
				this.delayedDeleteGroups.Add(group);
				return;
			}
			Controller.OnGroupDeleted(group);
		}
		public GroupRowInfo DoRowAdded(int controllerRow, DataControllerChangedItemCollection changedItems) {
			if(!IsGrouped) return null;
			GroupRowInfo groupRow = DoRowAddedCore(controllerRow, changedItems);
			Controller.UpdateGroupSummary(groupRow, null);
			return groupRow;
		}
		GroupRowInfo DoRowAddedCore(int controllerRow, DataControllerChangedItemCollection changedItems) {
			RenumIndexes(controllerRow, true);
			GroupRowInfo prev = controllerRow > 0 ? GetGroupRowInfoByControllerRowHandle(controllerRow - 1) : null;
			GroupRowInfo next = GetGroupRowInfoByControllerRowHandle(controllerRow + 1);
			if(prev == next && prev != null) {
				IncrementChildControllerRowCount(prev, changedItems, false);
				return prev;
			}
			int groupLevelNext = 0, groupLevelPrev = 0;
			if(prev != null) { 
				groupLevelPrev = Controller.CompareGroupColumnRows(SortInfo, this, controllerRow - 1, controllerRow);
				if(groupLevelPrev == -1) { 
					IncrementChildControllerRowCount(prev, changedItems, false);
					return prev;
				}
			}
			if(next != null) { 
				groupLevelNext = Controller.CompareGroupColumnRows(SortInfo, this, controllerRow, controllerRow + 1);
				if(groupLevelNext == -1) { 
					IncrementChildControllerRowCount(controllerRow, next, changedItems, true, false);
					return next;
				}
			}
			return CreateNewGroup(controllerRow, prev, next, groupLevelNext, groupLevelPrev, changedItems);
		}
		public void DoRowDeleted(int controllerRow, DataControllerChangedItemCollection changedItems) {
			if(!IsGrouped) return;
			GroupRowInfo summaryUpdate;
			DoRowDeleted(controllerRow, GetGroupRowInfoByControllerRowHandle(controllerRow), changedItems, out summaryUpdate);
			if(summaryUpdate != null) Controller.UpdateGroupSummary(summaryUpdate, null);
		}
		protected void DoRowDeleted(int controllerRow, GroupRowInfo groupRow, DataControllerChangedItemCollection changedItems, out GroupRowInfo summaryUpdateRequired) {
			summaryUpdateRequired = null;
			if(groupRow == null) return;
			GroupRowInfo savedGroupRow = groupRow;
			NotifyChangeType changedType = NotifyChangeType.ItemChanged;
			while(groupRow != null) {
				if(--groupRow.ChildControllerRowCount == 0) {
					RemoveAt(groupRow.Index);
					savedGroupRow = groupRow.ParentGroup;
					changedType = NotifyChangeType.ItemDeleted;
				}
				if(groupRow.IsVisible)
					changedItems.AddItem(groupRow.Handle, changedType, groupRow.ParentGroup);
				changedType = NotifyChangeType.ItemChanged;
				groupRow = groupRow.ParentGroup;
			}
			RenumIndexes(controllerRow, false);
			UpdateIndexes();
			summaryUpdateRequired = savedGroupRow;
		}
		public void DoRowChanged(VisibleIndexCollection visibleIndexes, int oldControllerRow, int newControllerRow, DataControllerChangedItemCollection changedItems) {
			if(!IsGrouped) return;
			if(visibleIndexes != null)
				visibleIndexes.SetDirty(); 
			GroupRowInfo newGroup, oldGroup = GetGroupRowInfoByControllerRowHandle(oldControllerRow), group;
			bool sameGroup = false;
			this.delayedDeleteGroups = new List<GroupRowInfo>();
			try {
				int changedItemsCount = changedItems.Count;
				GroupRowInfo summaryUpdate;
				DoRowDeleted(oldControllerRow, oldGroup, changedItems, out summaryUpdate);
				group = newGroup = DoRowAdded(newControllerRow, changedItems);
				if(summaryUpdate != null) controller.UpdateGroupSummary(summaryUpdate, null);
				sameGroup = oldGroup == newGroup;
				if(IsSameGroup(oldGroup, newGroup)) {
					sameGroup = true;
					while(oldGroup != null && newGroup != null) {
						newGroup.Expanded = oldGroup.Expanded;
						oldGroup = oldGroup.ParentGroup;
						newGroup = newGroup.ParentGroup;
					}
				}
				newGroup = group;
				if(sameGroup && group != null) {
					while(changedItems.Count > changedItemsCount) changedItems.RemoveAt(changedItems.Count - 1);
					while(group != null) {
						changedItems.AddItem(group.Handle, NotifyChangeType.ItemChanged, group.ParentGroup, true);
						group = group.ParentGroup;
					}
				}
			} finally {
				List<GroupRowInfo> groups = this.delayedDeleteGroups;
				this.delayedDeleteGroups = null;
				if(groups != null) {
					Controller.OnGroupsDeleted(groups, sameGroup);
				}
			}
		}
		bool IsSameGroup(GroupRowInfo oldGroup, GroupRowInfo newGroup) {
			return (oldGroup != null && newGroup != null && oldGroup != newGroup && 
				oldGroup.Index == newGroup.Index && oldGroup.ChildControllerRowCount == 0 && newGroup.ChildControllerRowCount == 1);
		}
		void RenumIndexes(int controllerRow, bool increment) {
			for(int i = 0; i < Count; i ++) {
				GroupRowInfo groupRow = this[i];
				if(increment) {
					if(groupRow.ChildControllerRow >= controllerRow) 
						groupRow.ChildControllerRow ++;
				} else {
					if(groupRow.ChildControllerRow > controllerRow) 
						groupRow.ChildControllerRow --;
				}
			}
		}
		void IncrementChildControllerRowCount(GroupRowInfo groupRow, DataControllerChangedItemCollection changedItems) {
			IncrementChildControllerRowCount(groupRow, changedItems, true);
		}
		void IncrementChildControllerRowCount(GroupRowInfo groupRow, DataControllerChangedItemCollection changedItems, bool addItemAtCurrentGroup) {
			IncrementChildControllerRowCount(DataController.InvalidRow, groupRow, changedItems, false, addItemAtCurrentGroup);
		}
		void IncrementChildControllerRowCount(int controllerRow, GroupRowInfo groupRow, DataControllerChangedItemCollection changedItems, bool decrementChildVisibleRow, bool addItemAtCurrentGroup) {
			NotifyChangeType changedType = addItemAtCurrentGroup ? NotifyChangeType.ItemAdded : NotifyChangeType.ItemChanged;
			while (groupRow != null) {
				groupRow.ChildControllerRowCount ++;
				if(decrementChildVisibleRow && groupRow.ChildControllerRow > controllerRow)
					groupRow.ChildControllerRow --;
				if(changedItems != null)
					changedItems.AddItem(groupRow.Handle, changedType, groupRow.ParentGroup);
				changedType = NotifyChangeType.ItemChanged;
				groupRow = groupRow.ParentGroup;
			}
		}
		GroupRowInfo CreateNewGroup(int controllerRow, GroupRowInfo prevGroup, GroupRowInfo nextGroup,
			int groupLevelNext, int groupLevelPrev, DataControllerChangedItemCollection changedItems) {
			int newGroupLevel = Math.Max(groupLevelNext, groupLevelPrev);
			GroupRowInfo parent = groupLevelNext > groupLevelPrev ? nextGroup : prevGroup;
			if(groupLevelNext == groupLevelPrev && groupLevelPrev == 0)
				parent = null;
			if(parent != null) 
				parent = parent.GetParentGroupAtLevel(newGroupLevel - 1);
			int newGroupIndex = 0; 
			if(parent != null) {
				newGroupIndex = GetNewGroupIndex(parent, controllerRow);
			} else {
				newGroupIndex = (prevGroup == null ? 0 : nextGroup == null ? Count : nextGroup.GetParentGroupAtLevel(0).Index);
			}
			if(parent != null)
				IncrementChildControllerRowCount(controllerRow, parent, changedItems, true, false);
			GroupRowInfo groupRow = CreateNewGroup(controllerRow, parent, newGroupLevel, newGroupIndex, changedItems);
			UpdateIndexes();
			return groupRow;
		}
		GroupRowInfo CreateNewGroup(int controllerRow, GroupRowInfo parent, int groupLevel, int newGroupIndex,
			DataControllerChangedItemCollection changedItems) {
			GroupRowInfo groupRow = null;
			for(int i = groupLevel; i < LevelCount; i ++) {
				groupRow = CreateGroupRowInfo((byte)i, controllerRow, parent);
				ChangeGroupRowExpanded(groupRow, AutoExpandAllGroups);
				Insert(newGroupIndex ++, groupRow);
				parent = groupRow;
				changedItems.AddItem(GroupRowInfo.GroupIndexToHandle(newGroupIndex - 1), NotifyChangeType.ItemAdded, groupRow.ParentGroup);
			}
			return groupRow;
		}
		int GetNewGroupIndex(GroupRowInfo parent, int controllerRow) {
			for(int i = parent.Index + 1; i < Count; i ++) {
				GroupRowInfo groupRow = this[i];
				if(groupRow.Level == parent.Level + 1 && controllerRow < this[i].ChildControllerRow)
					return i;
				if(groupRow.Level <= parent.Level)
					return i;
			}
			return Count;
		}
	}
	public class ValueComparer : IComparer {
		public virtual int Compare(object x, object y) {
			if(x == UnboundErrorObject.Value) x = null;
			if(y == UnboundErrorObject.Value) y = null;
			if(x == DBNull.Value) x = null;
			if(y == DBNull.Value) y = null;
			if(x == y) return 0;
			if(x == null) return -1;
			if(y == null) return 1;
			return CompareCore(x, y);
		}
		protected virtual int CompareCore(object x, object y) {
			return Comparer.Default.Compare(x, y);
		}
		public bool ObjectEquals(object x, object y) {
			if(x == DBNull.Value) x = null;
			if(y == DBNull.Value) y = null;
			return ObjectEqualsCore(x, y);
		}
		protected virtual bool ObjectEqualsCore(object x, object y) {
			return Object.Equals(x, y);
		}
	}
	public class NotificationCollectionBase : CollectionBase {
		public NotificationCollectionBase() { }
		public NotificationCollectionBase(CollectionChangeEventHandler collectionChanged) {
			this.CollectionChanged = collectionChanged;
		}
		int lockUpdate = 0;
		public event CollectionChangeEventHandler CollectionChanged;
		internal void Reset() {
			InnerList.Clear();
		}
		public void BeginUpdate() {
			this.lockUpdate ++;
		}
		public void CancelUpdate() { this.lockUpdate --; }
		public void EndUpdate() {
			if(--this.lockUpdate == 0) OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected bool IsLockUpdate { get { return lockUpdate != 0; } }
		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(IsLockUpdate) return;
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add,  value));
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete (index, value);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
		}
		protected override void OnClear() {
			if(InnerList.Count == 0) return;
			InnerList.Clear();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
	}
	public abstract class ColumnInfoNotificationCollection : NotificationCollectionBase {
		readonly DataControllerBase controller;
		protected ColumnInfoNotificationCollection(DataControllerBase controller, CollectionChangeEventHandler collectionChanged) : base(collectionChanged) {
			this.controller = controller;
		}
		public DataControllerBase Controller { get { return controller; } }
		protected abstract DataColumnInfo GetColumnInfo(int index);
		protected internal bool RemoveUnusedColumns(IList<DataColumnInfo> unusedColumns) {
			bool changed = false;
			for(int n = Count - 1; n >= 0; n--) {
				DataColumnInfo colInfo = GetColumnInfo(n);
				if(colInfo != null && unusedColumns.Contains(colInfo)) {
					changed = true;
					RemoveAt(n);
				}
			}
			return changed;
		}
	}
	public class TotalSummaryItemCollection : SummaryItemCollection {
		bool isDirty = false;
		public TotalSummaryItemCollection(DataControllerBase controller, CollectionChangeEventHandler collectionChanged) : base(controller, collectionChanged) { }
		public bool IsDirty {
			get { return isDirty; }
			set { isDirty = value; }
		}
		public void SetDirty() { this.isDirty = true; }
		protected internal override void RequestSummaryValue() { 
			if(IsDirty) {
				this.isDirty = false;
				Controller.UpdateTotalSummary();
			}
		}
		public int IndexOf(SummaryItem item) { return List.IndexOf(item); }
		public bool RemoveItems(ICollection items) {
			if(items == null || items.Count == 0) return true;
			bool changed = true;
			BeginUpdate();
			try {
				foreach(SummaryItem item in items) {
					int i = IndexOf(item);
					if(i >= 0) {
						RemoveAt(i);
						changed = true;
					}
				}
			}
			finally {
				if(changed)
					EndUpdate();
				else
					CancelUpdate();
			}
			return changed;
		}
	}
	public class SummaryItemCollection : ColumnInfoNotificationCollection {
		public SummaryItemCollection(DataControllerBase controller, CollectionChangeEventHandler collectionChanged) : base(controller, collectionChanged) { }
		public SummaryItem this[int index] { get { return List[index] as SummaryItem; } }
		public SummaryItem GetSummaryItemByTag(object tag) {
			foreach(SummaryItem item in this) {
				if(object.Equals(item.Tag, tag)) return item;
			}
			return null;
		}
		public List<SummaryItem> GetSummaryItemByTagType(Type tagType) {
			List<SummaryItem> list = new List<SummaryItem>();
			foreach(SummaryItem item in this) {
				if(item.Tag == null) continue;
				if(tagType.IsInstanceOfType(item.Tag)) list.Add(item);
			}
			return list;
		}
		public static int GetActiveCount(IList list) {
			int res = 0;
			for(int n = list.Count - 1; n >= 0; n--) res += ((SummaryItem)list[n]).SummaryTypeEx != SummaryItemTypeEx.None ? 1 : 0;
			return res;
		}
		public SummaryItem GetSummaryItemByKey(object key) {
			foreach(SummaryItem item in this) {
				if(object.Equals(item.Key, key)) return item;
			}
			return null;
		}
		protected internal virtual void RequestSummaryValue() { }
		public bool Contains(SummaryItem item) { return List.Contains(item); }
		protected override DataColumnInfo GetColumnInfo(int index) { return this[index].ColumnInfo; }
		public virtual SummaryItem Add(SummaryItem item) { 
			List.Add(item);
			return item;
		}
		public void ClearAndAddRange(SummaryItem[] summaryItems) {
			BeginUpdate();
			try {
				Clear();
				AddRange(summaryItems);
			}
			finally {
				EndUpdate();
			}
		}
		public void AddRange(SummaryItem[] summaryItems) {
			BeginUpdate();
			try {
				foreach(SummaryItem summaryItem in summaryItems) { List.Add(summaryItem); }
			}
			finally {
				EndUpdate();
			}
		}
		protected override void OnInsertComplete(int index, object value) {
			SummaryItem item = value as SummaryItem;
			item.Collection = this;
			base.OnInsertComplete(index, value);
		}
		protected override void OnRemoveComplete(int index, object value) {
			SummaryItem item = value as SummaryItem;
			item.Collection = null;
			base.OnRemoveComplete(index, value);
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) this[n].Collection = null;
			base.OnClear();
		}
		protected internal virtual void OnSummaryItemChanged(SummaryItem item) {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh,  item));
		}
		public int ActiveCount { get { return GetActiveCount(this); } }
		public List<ListSourceSummaryItem> GetSummaryItems() {
			return GetSummaryItems(false);
		}
		public List<ListSourceSummaryItem> GetSummaryItems(bool allowUnbound) {
			List<ListSourceSummaryItem> res = new List<ListSourceSummaryItem>();
			foreach(SummaryItem item in this) {
				if(!item.GetAllowExternalCalculate(allowUnbound)) {
					res.Add(new ListSourceSummaryItem(null, SummaryItemType.None));
					continue;
				}
				res.Add(new ListSourceSummaryItem(item));
			}
			return res;
		}
	}
	public class SummaryItemBase {
		DataColumnInfo columnInfo;
		object tag;
		public SummaryItemBase(DataColumnInfo columnInfo, object tag) {
			this.columnInfo = columnInfo;
			this.tag = tag;
		}
		public SummaryItemBase(DataColumnInfo columnInfo) : this(columnInfo, null) {
		}
		public SummaryItemBase() : this(null) { }
		public DataColumnInfo ColumnInfo {
			get { return columnInfo; }
			set { 
				if(ColumnInfo == value) return;
				columnInfo = value;
				OnSummaryChanged();
			}
		}
		public object Key {
			get { return Tag == null ? this : Tag; }
		}
		public object Tag { 
			get { return tag; }
			set { tag = value; }
		}
		protected virtual void OnSummaryChanged() {
		}
	}
	public class SummaryItem: SummaryItemBase {
		SummaryItemCollection collection;
		object summaryValue;
		bool? ignoreNullValues;
		SummaryItemTypeEx summaryTypeEx;
		decimal summaryArgument;
		bool exists = true;
		public SummaryItem(DataColumnInfo columnInfo, SummaryItemType summaryType, object tag, bool? ignoreNullValues = null) : base(columnInfo, tag) {
			this.summaryType = summaryType;
			this.ignoreNullValues = ignoreNullValues;
			this.collection = null;
			this.summaryValue = null;
		}
		public SummaryItem(DataColumnInfo columnInfo, SummaryItemTypeEx summaryType, decimal argument, bool? ignoreNullValues = null)
			: this(columnInfo, SummaryItemType.None, null) {
			this.summaryTypeEx = summaryType;
			this.summaryArgument = argument;
			this.ignoreNullValues = ignoreNullValues;
		}
		public SummaryItem(DataColumnInfo columnInfo, SummaryItemType summaryType) : this(columnInfo, summaryType, null) {
		}
		public SummaryItem() : this(null, SummaryItemType.None) { }
		public bool Exists { get { return exists; } set { exists = value; } }
		public string FieldName {
			get { return ColumnInfo == null ? "" : ColumnInfo.Name; }
			set {
				if(FieldName == value || Collection == null) return;
				ColumnInfo = Collection.Controller.Columns[value];
			}
		}
		SummaryItemType summaryType {
			get {
				if(((int)summaryTypeEx) > ((int)SummaryItemType.None)) return SummaryItemType.None;
				return (SummaryItemType)summaryTypeEx;;
			}
			set { summaryTypeEx = (SummaryItemTypeEx)value; }
		}
		public decimal SummaryArgument {
			get { return summaryArgument; }
			set {
				if(SummaryArgument == value) return;
				summaryArgument = value;
				if(IsSummaryArgumentRequired) OnSummaryChanged();				
			}
		}
		public virtual bool IsListBasedSummary {
			get {
				return SummaryTypeEx == SummaryItemTypeEx.Bottom ||
					SummaryTypeEx == SummaryItemTypeEx.BottomPercent || SummaryTypeEx == SummaryItemTypeEx.Top || SummaryTypeEx == SummaryItemTypeEx.TopPercent ||
					summaryTypeEx == SummaryItemTypeEx.Unique || summaryTypeEx == SummaryItemTypeEx.Duplicate; 
			}
		}
		protected internal bool IsSummaryArgumentRequired {
			get {
				return IsListBasedSummary;
			}
		}
		protected internal bool IsPercentArgument {
			get { return SummaryTypeEx == SummaryItemTypeEx.TopPercent || SummaryTypeEx == SummaryItemTypeEx.BottomPercent; }
		}
		public SummaryItemType SummaryType {
			get { return summaryType;}
			set {
				if(SummaryTypeEx == (SummaryItemTypeEx)value) return;
				SummaryTypeEx = (SummaryItemTypeEx)value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		public SummaryItemTypeEx SummaryTypeEx {
			get { return summaryTypeEx; }
			set {
				if(SummaryTypeEx == value) return;
				summaryTypeEx = value;
				if(SummaryTypeEx == SummaryItemTypeEx.None) SummaryValue = null;
				OnSummaryChanged();
			}
		}
		public object SummaryValue { 
			get { 
				if(!IsNoneSummary && Collection != null) Collection.RequestSummaryValue();
				return summaryValue;
			} 
			set { summaryValue = value; }
		}
		public IList SummaryValueListBased {
			get { 
				var res =  SummaryValue as IList;
				if(res == null) SummaryValue = res = new List<object>();
				return res;
			}
		}
		public bool AllowCalculate {
			get {
				return !IsNoneSummary && (ColumnInfo != null || SummaryType == SummaryItemType.Count || SummaryType == SummaryItemType.Custom);
			}
		}
		[Obsolete]
		public bool AllowExternalCalculate {
			get {
				return GetAllowExternalCalculate(false);
			}
		}
		public bool GetAllowExternalCalculate(bool allowUnbound) {
			if(!AllowCalculate) return false;
			if(SummaryType == SummaryItemType.Count && (ColumnInfo == null || !ColumnInfo.Unbound)) return true;
			if(ColumnInfo == null) return false;
			if(!ColumnInfo.Unbound) return true;
			if(!allowUnbound) return false;
			if(Collection.Controller.IsServerMode) return ColumnInfo.UnboundWithExpression;
			return true;
		}
		public bool IsNoneSummary { 
			get { return SummaryTypeEx == SummaryItemTypeEx.None; }
		}
		public bool IsCustomSummary {
			get { return SummaryType == SummaryItemType.Custom; }
		}
		protected internal SummaryItemCollection Collection { get { return collection; } set { collection = value; } }
		protected override void OnSummaryChanged() {
			if(Collection == null) return;
			Collection.OnSummaryItemChanged(this);
		}
		protected internal bool IgnoreNullValues(bool defaultValue) {
			return ignoreNullValues.GetValueOrDefault(defaultValue);
		}
	}
}

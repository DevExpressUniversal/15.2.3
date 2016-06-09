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
using System.Data;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Data;
using System.Collections.Generic;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DevExpress.Utils.Design;
namespace DevExpress.XtraTreeList.Data {
	[TypeConverter(typeof(EnumTypeConverter)), ResourceFinder(typeof(DevExpress.Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum UnboundColumnType { Object, Integer, Decimal, DateTime, String, Boolean, Bound }
	public class DataColumnInfo {
		string columnName;
		string caption;
		bool readOnly, _fixed;
		int handle;
		Type type;
		PropertyDescriptor descriptor;
		string unboundExpression;
		public DataColumnInfo(TreeListColumn unboundColumn) {
			this.descriptor = null;
			this.columnName = unboundColumn.FieldName;
			this.caption = unboundColumn.Caption;
			this.readOnly = false;
			this._fixed = false;
			this.handle = -1;
			this.type = dataTypes[Convert.ToInt32(unboundColumn.UnboundType)];
			this.unboundExpression = unboundColumn.UnboundExpression;
		}
		public DataColumnInfo(PropertyDescriptor descriptor, int handle) {
			this.descriptor = descriptor;
			this.columnName = descriptor.Name;
			this.caption = descriptor.DisplayName;
			this.readOnly = descriptor.IsReadOnly;
			this.handle = handle;
			this._fixed = true;
			this.type = descriptor.PropertyType;
		}
		public DataColumnInfo(DataColumn dc) {
			this.columnName = dc.ColumnName;
			this.caption = dc.Caption;
			this.readOnly = dc.ReadOnly;
			this.handle = dc.Ordinal;
			this.type = dc.DataType;
			this._fixed = true;
		}
		public string ColumnName { get { return columnName; } }
		public string Caption { get { return caption; } }
		public bool Fixed { get { return _fixed; } }
		public bool ReadOnly { get { return readOnly; } }
		public int Handle { get { return handle; } }
		public Type Type { get { return type; } }
		public PropertyDescriptor Descriptor { get { return descriptor; } }
		public string UnboundExpression { get { return unboundExpression; } }
		internal bool IsUnbound { get { return Descriptor is TreeListUnboundPropertyDescriptor; } }
		static Type[] dataTypes = new Type[] { typeof(object), typeof(int), typeof(Decimal), typeof(DateTime), typeof(string), typeof(bool), typeof(object) };
	}
	public class ListDataColumnInfoCollection : DataColumnInfoCollection {
		public ListDataColumnInfoCollection(ListData data)
			: base(data) {
		}
		public override DataColumnInfo this[int index] {
			get {
				CheckColumns();
				return base[index];
			}
		}
		public override DataColumnInfo this[string name] {
			get {
				CheckColumns();
				return base[name];
			}
		}
		protected void CheckColumns() {
			if(Count == 0) Populate(Data.DataList);
		}
		object SourceObject { get { return Data.DataList.Count > 0 ? Data.DataList[0] : null; } }
		protected override void AddColumnDefinitions(IList dataSource) {
			PropertyDescriptorCollection collection = GetDescriptorCollection(dataSource);
			PatchPropertyDescriptorCollection(collection);
			if(collection == null) return;
			if(Data.CanUseFastProperties)
				collection = DevExpress.Data.Access.DataListDescriptor.GetFastProperties(collection);
			int j = 0;
			int lastHandle = -1;
			for(int i = 0; i < collection.Count; i++) {
				if(IsDetailDescriptor(collection[i])) {
					j++;
					continue;
				};
				lastHandle = i - j;
				List.Add(new DataColumnInfo(collection[i], lastHandle));
			}
			ComplexColumnInfoCollection complexInfoCollection = GetComplexColumnsInfo();
			if(complexInfoCollection != null && complexInfoCollection.Count != 0) {
				lastHandle++;
				for(int i = 0; i < complexInfoCollection.Count; i++) {
					ComplexColumnInfo info = complexInfoCollection[i];
					List.Add(new DataColumnInfo(new DevExpress.Data.Access.ComplexPropertyDescriptorReflection(SourceObject, info.Name), lastHandle + i));
				}
			}
			List<DataColumnInfo> unboundColumns = Data.GetUnboundColumnsInfo();
			if(unboundColumns != null) {
				lastHandle++;
				j = 0;
				for(int i = 0; i < unboundColumns.Count; i++) {
					DataColumnInfo unboundColumnInfo = unboundColumns[i];
					if(this[unboundColumnInfo.ColumnName] != null) {
						j++;
						continue;
					}
					List.Add(new DataColumnInfo(CreateUnboundPropertyDescriptor(unboundColumnInfo), lastHandle + i - j));
				}
			}
		}
		protected virtual PropertyDescriptor CreateUnboundPropertyDescriptor(DataColumnInfo unboundColumnInfo) {
			return new TreeListUnboundPropertyDescriptor(Data.DataHelper, unboundColumnInfo);
		}
		protected virtual ComplexColumnInfoCollection GetComplexColumnsInfo() {
			ComplexColumnInfoCollection res = new ComplexColumnInfoCollection();
			string keyFieldName = CheckComplexFieldName(Data.DataHelper.KeyFieldName);
			string parentFieldName = CheckComplexFieldName(Data.DataHelper.ParentFieldName);
			foreach(TreeListColumn column in Data.DataHelper.Columns) {
				string fieldName = column.FieldName;
				if(IsComplexFieldName(fieldName) && base[fieldName] == null) {
					if(fieldName == keyFieldName) 
						keyFieldName = null;
					if(fieldName == parentFieldName)
						parentFieldName = null;
					res.Add(fieldName);
				}
			}
			if(keyFieldName != null)
				res.Add(keyFieldName);
			if(parentFieldName != null)
				res.Add(parentFieldName);
			return res;
		}
		protected virtual bool IsComplexFieldName(string fieldName) { return fieldName.Contains("."); }
		string CheckComplexFieldName(string fieldName) {
			return IsComplexFieldName(fieldName) ? fieldName : null;
		}
		protected virtual PropertyDescriptorCollection PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			if(Data.DataHelper.Columns == null || collection == null)
				return collection;
			foreach(TreeListColumn col in Data.DataHelper.Columns) {
				if(collection.Find(col.FieldName, false) == null)
					collection.Find(col.FieldName, true);
			}
			return collection;
		}
		protected virtual bool IsDetailDescriptor(PropertyDescriptor descriptor) {
			return typeof(IList).IsAssignableFrom(descriptor.PropertyType) && !typeof(Array).IsAssignableFrom(descriptor.PropertyType);
		}
		Type GetListItemPropertyType(IList list) {
			System.Reflection.PropertyInfo[] props = list.GetType().GetProperties();
			for(int i = 0; i < props.Length; i++) {
				if("Item".Equals(props[i].Name) && props[i].PropertyType != typeof(object)) {
					return props[i].PropertyType;
				}
			}
			return null;
		}
		protected virtual PropertyDescriptorCollection GetDescriptorCollection(IList dataSource) {
			if(dataSource == null) return null;
			if(dataSource is ITypedList)
				return (dataSource as ITypedList).GetItemProperties(null);
			PropertyDescriptorCollection col = null;
			Type itemType = GetListItemPropertyType(dataSource);
			if(itemType != null) {
				col = TypeDescriptor.GetProperties(itemType);
			}
			else {
				if(dataSource.Count > 0) {
					object item = dataSource[0];
					if(item != null) {
						itemType = item.GetType();
						if(DevExpress.Data.Access.ExpandoPropertyDescriptor.IsDynamicType(itemType)) 
							col = GetExpandoObjectProperties(item);
						else 
							col = TypeDescriptor.GetProperties(item);
					}
				}
			}
			if(col == null || col.Count == 0) return null;
			return col;
		}
		protected virtual PropertyDescriptorCollection GetExpandoObjectProperties(object item) {
			IDictionary<string, object> properties = item as IDictionary<string, object>;
			if(properties == null) return null;
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach(KeyValuePair<string, object> pair in properties) 
				list.Add(new DevExpress.Data.Access.ExpandoPropertyDescriptor(null, pair.Key, pair.Value == null ? null : pair.Value.GetType()));
			return new PropertyDescriptorCollection(list.ToArray());
		}
		protected new ListData Data { get { return base.Data as ListData; } }
	}
	public class DataViewColumnInfoCollection : DataColumnInfoCollection {
		public DataViewColumnInfoCollection(DataViewData data) : base(data) { }
		protected override void AddColumnDefinitions(IList dataSource) {
			DataView dataView = dataSource as DataView;
			if(dataView == null || dataView.Table == null) return;
			foreach(DataColumn dc in dataView.Table.Columns)
				List.Add(new DataColumnInfo(dc));
			List<DataColumnInfo> unboundColumns = Data.GetUnboundColumnsInfo();
			if(unboundColumns != null) {
				int j = 0;
				for(int i = 0; i < unboundColumns.Count; i++) {
					DataColumnInfo unboundColumnInfo = unboundColumns[i];
					if(this[unboundColumnInfo.ColumnName] != null) {
						j++;
						continue;
					}
					List.Add(new DataColumnInfo(CreateUnboundPropertyDescriptor(unboundColumnInfo), dataView.Table.Columns.Count + i - j));
				}
			}
		}
		protected virtual PropertyDescriptor CreateUnboundPropertyDescriptor(DataColumnInfo unboundColumnInfo) {
			return new TreeListUnboundPropertyDescriptor(Data.DataHelper, unboundColumnInfo);
		}
		protected new DataViewData Data { get { return base.Data as DataViewData; } }
	}
	public class UnboundDataColumnInfoCollection : DataColumnInfoCollection {
		public UnboundDataColumnInfoCollection(UnboundData data) : base(data) { }
		protected override void AddColumnDefinitions(IList dataSource) {
			foreach(TreeListColumn col in Data.SynchColumns) {
				List.Add(new DataColumnInfo(col));
			}
		}
		protected new UnboundData Data { get { return base.Data as UnboundData; } }
	}
	public abstract class DataColumnInfoCollection : CollectionBase {
		Hashtable names;
		TreeListData data;
		public DataColumnInfoCollection(TreeListData data) {
			this.names = new Hashtable();
			this.data = data;
		}
		public virtual DataColumnInfo this[int index] { get { return (DataColumnInfo)InnerList[index]; } }
		public virtual DataColumnInfo this[string name] { get { return (DataColumnInfo)Names[name]; } }
		public int IndexOf(string name) {
			DataColumnInfo info = this[name];
			return (info == null ? -1 : info.Handle);
		}
		protected override void OnClear() {
			InnerList.Clear();
			Names.Clear();
		}
		protected abstract void AddColumnDefinitions(IList dataSource);
		bool populating = false;
		public void Populate(IList dataSource) {
			if(populating) return;
			Clear();
			populating = true;
			try {
				AddColumnDefinitions(dataSource);
			}
			finally {
				populating = false;
			}
		}
		public DataColumnInfo GetColumnByID(object columnID) {
			return this[Data.DataHelper.GetColumnNameByColumnID(columnID)];
		}
		protected override void OnInsertComplete(int index, object value) {
			DataColumnInfo info = (DataColumnInfo)value;
			Names[info.ColumnName] = info;
		}
		protected Hashtable Names { get { return names; } }
		protected TreeListData Data { get { return data; } }
	}
	public abstract class TreeListData : IDisposable {
		TreeListDataHelper dataHelper;
		IBindingList bindingList;
		DataColumnInfoCollection columns;
		public TreeListData(TreeListDataHelper dataHelper) {
			this.dataHelper = dataHelper;
			this.columns = CreateColumns();
		}
		public virtual void Dispose() {
			BindingList = null;
			Columns.Clear();
		}
		protected abstract DataColumnInfoCollection CreateColumns();
		public string GetDisplayText(int nodeID, object columnID, TreeListNode node) {
			return DataHelper.GetDisplayText(columnID, node);
		}
		public string GetErrorText(int nodeID) {
			IDataErrorInfo errorInfo = GetDataErrorInfoObject(nodeID);
			IDXDataErrorInfo dxErrorInfo = GetDXDataErrorInfoObject(nodeID);
			if(errorInfo != null && dxErrorInfo == null) return errorInfo.Error;
			if(dxErrorInfo != null) {
				ErrorInfo info = new ErrorInfo();
				dxErrorInfo.GetError(info);
				return info.ErrorText;
			}
			return null;
		}
		public ErrorType GetErrorType(int nodeID) {
			IDataErrorInfo errorInfo = GetDataErrorInfoObject(nodeID);
			IDXDataErrorInfo dxErrorInfo = GetDXDataErrorInfoObject(nodeID);
			if(errorInfo != null && dxErrorInfo == null) {
				if(errorInfo.Error != null && errorInfo.Error.Length > 0) return ErrorType.Default;
			}
			if(dxErrorInfo != null) {
				ErrorInfo info = new ErrorInfo();
				dxErrorInfo.GetError(info);
				if(info.ErrorText != null && info.ErrorText.Length > 0) return info.ErrorType;
			}
			return ErrorType.None;
		}
		public string GetColumnErrorText(int nodeID, object columnID) {
			string colName = DataHelper.GetColumnNameByColumnID(columnID);
			if(colName == string.Empty) return null;
			DataColumnInfo colInfo = Columns[colName];
			if(colInfo != null && colInfo.IsUnbound)
				return null;
			try {
				IDataErrorInfo errorInfo = GetDataErrorInfoObject(nodeID);
				IDXDataErrorInfo dxErrorInfo = GetDXDataErrorInfoObject(nodeID);
				if(errorInfo != null && dxErrorInfo == null) return errorInfo[colName];
				if(dxErrorInfo != null) {
					ErrorInfo info = new ErrorInfo();
					dxErrorInfo.GetPropertyError(colName, info);
					return info.ErrorText;
				}
			}
			catch { }
			return null;
		}
		public ErrorType GetColumnErrorType(int nodeID, object columnID) {
			string colName = DataHelper.GetColumnNameByColumnID(columnID);
			if(colName == string.Empty) return ErrorType.None;
			DataColumnInfo colInfo = Columns[colName];
			if(colInfo != null && colInfo.IsUnbound)
				return ErrorType.None;
			try {
				IDataErrorInfo errorInfo = GetDataErrorInfoObject(nodeID);
				IDXDataErrorInfo dxErrorInfo = GetDXDataErrorInfoObject(nodeID);
				if(errorInfo != null && dxErrorInfo == null) {
					if(errorInfo[colName] != null && errorInfo[colName].Length > 0) return ErrorType.Default;
				}
				if(dxErrorInfo != null) {
					ErrorInfo ei = new ErrorInfo();
					dxErrorInfo.GetPropertyError(colName, ei);
					if(ei.ErrorText != null && ei.ErrorText.Length > 0) return ei.ErrorType;
				}
			}
			catch { }
			return ErrorType.None;
		}
		public abstract object GetValue(int nodeID, object columnID);
		public virtual object GetValues(int nodeID) { return GetDataRow(nodeID); }
		public virtual DataColumnInfo GetDataColumnInfo(string columnName) {
			return Columns[columnName];
		}
		public virtual int GetColumnHandleByFieldName(string columnName) {
			DataColumnInfo colInfo = GetDataColumnInfo(columnName);
			return (colInfo == null ? -1 : colInfo.Handle);
		}
		public abstract object GetDataRow(int nodeID);
		public abstract void CreateNodes(TreeListNode parent);
		public virtual bool CanPopulate(DataColumnInfo info) { return true; }
		public abstract bool IsValidColumnHandle(int columnHandle);
		public abstract void OnColumnTypeChanged(TreeListColumn column);
		public virtual bool IsValidColumnName(string columnName) { return IsValidColumnHandle(GetColumnHandleByFieldName(columnName)); }
		public static bool IsNull(object value) { return (value == null || value == DBNull.Value); }
		public virtual void SetValue(int nodeID, object columnID, object value, bool initEdit) {
			if(!CanSetValue(columnID)) return;
			try {
				if(initEdit)
					BeginDataRowEdit(nodeID);
				bool cellChanged = SetValueCore(nodeID, columnID, ConvertValue(value, columnID));
				if(cellChanged)
					DataHelper.ListChanged(DataList, new ListChangedEventArgs(ListChangedType.ItemChanged, nodeID, -1));
			}
			catch(Exception e) {
				DataHelper.InvalidValueException(e, value);
			}
		}
		protected virtual void SetValues(int nodeID, object values) {
			try { SetValuesCore(nodeID, ConvertValues(values)); }
			catch { }
		}
		protected virtual internal void SetParentRelation(TreeListNode childNode, TreeListNode parentNode) {
			if(Columns[DataHelper.ParentFieldName] == null || Columns[DataHelper.KeyFieldName] == null) return;
			SetValue(childNode.Id, DataHelper.ParentFieldName, (parentNode == null ? DataHelper.RootValue : GetValue(parentNode.Id, DataHelper.KeyFieldName)), false);
		}
		protected internal void PopulateColumns() {
			Columns.Populate(DataList);
		}
		protected abstract bool SetValueCore(int nodeID, object columnID, object value);
		protected abstract void SetValuesCore(int nodeID, object[] values);
		public abstract bool CurrencyManagerWasReset(CurrencyManager manager, ItemChangedEventArgs e);
		public virtual TreeListNode Append(TreeListNode parent, object values) {
			return Append(parent, values, null);
		}
		public virtual TreeListNode Append(TreeListNode parent, object values, object tag) {
			TreeListNode result = null;
			if(CanAdd) {
				int rowIndex = AppendCore();
				if(rowIndex != -1) {
					result = DataHelper.CreateNode(rowIndex, parent, tag);
					SetValues(rowIndex, values);
					SetParentRelation(result, parent);
					EndDataRowEdit(rowIndex);
				}
			}
			return result;
		}
		internal int Append(object values) {
			int rowIndex = -1;
			if(CanAdd) {
				rowIndex = AppendCore();
				if(rowIndex != -1) {
					SetValues(rowIndex, values);
					EndDataRowEdit(rowIndex);
				}
			}
			return rowIndex;
		}
		public void Remove(TreeListNode node) {
			if(CanRemove(node)) {
				RemoveFromSource(node.Id);
				OnRemoveNode(node);
			}
		}
		public void RemoveList(NodesIdInfo children) {
			NodesIdInfo current = children;
			while(current != null) {
				RemoveRange(current);
				current = current.Next;
			}
		}
		protected virtual void RemoveRange(NodesIdInfo range) {
			for(int id = range.EndId; id >= range.StartId; id--)
				RemoveFromSource(id);
		}
		public void Clear() {
			if(CanClear)
				ClearCore();
		}
		protected abstract void ClearCore();
		protected abstract void RemoveFromSource(int nodeID);
		protected abstract void OnRemoveNode(TreeListNode node);
		protected abstract int AppendCore();
		protected virtual bool CanSetValue(object columnID) {
			if(columnID == null) return false;
			if(BindingList != null)
				return BindingList.AllowEdit;
			return true;
		}
		internal bool IsCurrentDataRowEditing {get; set; }
		protected internal virtual void BeginDataRowEdit(int nodeID) {
			IsCurrentDataRowEditing = true;
			IEditableObject edObj = GetEditableObject(nodeID);
			if(edObj != null)
				edObj.BeginEdit();
		}
		protected internal virtual void EndDataRowEdit(int nodeID) {
			IsCurrentDataRowEditing = false;
			IEditableObject edObj = GetEditableObject(nodeID);
			if(edObj != null)
				edObj.EndEdit();
		}
		public virtual IEditableObject GetEditableObject(int nodeID) { return (GetDataRow(nodeID) as IEditableObject); }
		protected virtual object ConvertValue(object value, object columnID) { return value; }
		protected virtual object[] ConvertValues(object dataRow) {
			object[] defValue = new object[Columns.Count];
			if(dataRow == null) return defValue;
			if(dataRow is object[] || dataRow is DataRow) {
				object[] items = dataRow as object[];
				if(items == null) items = ((DataRow)dataRow).ItemArray;
				if(items.GetLength(0) == DataHelper.Columns.Count) return items;
				CopyItems(items, defValue);
			}
			return defValue;
		}
		private void CopyItems(object[] items, object[] defValue) {
			for(int i = 0; i < items.GetLength(0); i++) {
				if(i < defValue.GetLength(0))
					defValue.SetValue(items.GetValue(i), i);
				else return;
			}
		}
		protected virtual IDataErrorInfo GetDataErrorInfoObject(int nodeID) { return (GetDataRow(nodeID) as IDataErrorInfo); }
		protected virtual IDXDataErrorInfo GetDXDataErrorInfoObject(int nodeID) { return (GetDataRow(nodeID) as IDXDataErrorInfo); }
		protected internal TreeListDataHelper DataHelper { get { return dataHelper; } }
		protected IBindingList BindingList {
			get { return bindingList; }
			set {
				if(BindingList != null)
					BindingList.ListChanged -= new ListChangedEventHandler(DataHelper.ListChanged);
				bindingList = value;
				if(BindingList != null)
					BindingList.ListChanged += new ListChangedEventHandler(DataHelper.ListChanged);
				PopulateColumns();
			}
		}
		public abstract bool IsUnboundMode { get; }
		public abstract bool CanAdd { get; }
		public virtual bool CanRemove(TreeListNode node) { return node.Id > -1 && node.Id < DataList.Count && CanClear; }
		public abstract bool CanClear { get; }
		public abstract IList DataList { get; }
		public DataColumnInfoCollection Columns { get { return columns; } }
		protected internal virtual void OnClearColumns() { }
		protected internal virtual bool SupportNotifications { get { return false; } }
	}
	public abstract class TreeListBoundData : TreeListData {
		public TreeListBoundData(TreeListDataHelper dataHelper) : base(dataHelper) { }
		public override bool CurrencyManagerWasReset(CurrencyManager manager, ItemChangedEventArgs e) {
			if(manager == null) return false;
			return (e.Index == -1 && manager.List != DataList);
		}
		public override bool IsValidColumnHandle(int columnHandle) {
			if(columnHandle > -1) {
				return columnHandle < Columns.Count;
			}
			return false;
		}
		public override void OnColumnTypeChanged(TreeListColumn column) {
			PopulateColumns();
		}
		protected override void SetValuesCore(int nodeID, object[] values) {
			try {
				if(values == null || Columns.Count != values.Length) return;
				object listItem = GetDataRow(nodeID);
				for(int i = 0; i < Columns.Count; i++) {
					SetValueCore(Columns[i], nodeID, listItem, values[i]);
				}
			}
			catch { }
		}
		protected abstract void SetValueCore(DataColumnInfo info, int nodeID, object listItem, object value);
		public override void CreateNodes(TreeListNode parent) {
			if(parent == null) DataHelper.ClearNodes();
			CreateAllNodes();
		}
		protected virtual void CreateAllNodes() {
			if(DataList == null) return;
			object keyDescriptor = GetServiceFieldDescriptor(DataHelper.KeyFieldName);
			object parentDescriptor = GetServiceFieldDescriptor(DataHelper.ParentFieldName);
			if(keyDescriptor == null || parentDescriptor == null) {
				CreateRootNodes();
				return;
			}
			Hashtable keyNodes = new Hashtable();
			ArrayList rootNodes = new ArrayList();
			int index = 0;
			for(int i = 0; i < DataList.Count; i++) {
				object key = GetKeyValue(i, keyDescriptor);
				if(key == AbsentValue ) key = index++;
				keyNodes.Add(key, i);
			}
			index = 0;
			ArrayList list = new ArrayList();
			for(int i = 0; i < DataList.Count; i++) {
				object key = GetKeyValue(i, keyDescriptor);
				if(key == AbsentValue ) key = index++;
				while(keyNodes[key] is int) {
					list.Add(key);
					object next = GetParentFieldValue((int)keyNodes[key], parentDescriptor);
					if(next == null) break;
					if(list[list.Count / 2].Equals(next)) {
						int j;
						for(j = list.Count - 1; j >= 0; j--)
							if(list[j].Equals(next)) break;
						int length = list.Count - j;
						for(j = list.Count / 2 + length - 1; j >= length; j--)
							if(!list[j].Equals(list[j - length])) break;
						list.RemoveRange(j + 1, list.Count - j - 1);
						break;
					}
					key = next;
				}
				for(int j = list.Count - 1; j >= 0; j--) {
					key = list[j];
					int nodeIndex = (int)keyNodes[key];
					TreeListNode parentNode = GetParentNode(keyNodes, GetParentFieldValue(nodeIndex, parentDescriptor));
					keyNodes[key] = DataHelper.CreateNode(nodeIndex, parentNode);
				}
				list.Clear();
				TreeListNode node = (TreeListNode)keyNodes[key];
				if(node.owner != null && node.owner.IndexOf(node) != i)
					DataHelper.SetNodeIndex(node, i);
			}
		}
		TreeListNode GetParentNode(Hashtable keyNodes, object keyID) {
			if(keyID == null) return null;
			return (keyNodes[keyID] as TreeListNode);
		}
		object GetParentFieldValue(int rowIndex, object fieldDescriptor) {
			object parentID = GetParentValue(rowIndex, fieldDescriptor);
			if(IsNull(parentID))
				parentID = DataHelper.RootValue;
			return parentID;
		}
		protected virtual void CreateRootNodes() {
			for(int i = 0; i < DataList.Count; i++) {
				DataHelper.CreateNode(i, null);
			}
		}
		protected override void OnRemoveNode(TreeListNode node) {
			DataHelper.DeleteNode(node);
		}
		protected override void ClearCore() {
			DataHelper.ClearNodes();
		}
		protected class AbsentKeyValue {
			[ThreadStatic]
			static AbsentKeyValue instance = new AbsentKeyValue();
			private AbsentKeyValue() { }
			public static AbsentKeyValue Instance { get { return instance; } }
		}
		protected AbsentKeyValue AbsentValue { get { return AbsentKeyValue.Instance; } }
		protected abstract object GetServiceFieldDescriptor(string fieldName);
		protected abstract object GetKeyValue(int rowIndex, object fieldDescriptor);
		protected abstract object GetParentValue(int rowIndex, object fieldDescriptor);
		public override bool IsUnboundMode { get { return false; } }
		#region unbound columns
		protected internal virtual List<DataColumnInfo> GetUnboundColumnsInfo() {
			List<DataColumnInfo> columns = new List<DataColumnInfo>();
			foreach(TreeListColumn col in DataHelper.Columns) {
				if(col.UnboundType == UnboundColumnType.Bound || string.IsNullOrEmpty(col.FieldName))
					continue;
				columns.Add(new DataColumnInfo(col));
			}
			return columns;
		}
		#endregion
	}
	public class DataViewData : TreeListBoundData {
		DataView dataView;
		public DataViewData(TreeListDataHelper dataHelper, DataView dataView)
			: base(dataHelper) {
			this.dataView = dataView;
			BindingList = dataView;
			this.dataView.Disposed += new EventHandler(OnDataViewDisposed);
		}
		public override void Dispose() {
			this.dataView.Disposed -= new EventHandler(OnDataViewDisposed);
			this.dataView = null;
			base.Dispose();
		}
		protected override DataColumnInfoCollection CreateColumns() { return new DataViewColumnInfoCollection(this); }
		private void OnDataViewDisposed(object sender, EventArgs e) {
			Dispose();
			DataHelper.DataSourceDisposed();
		}
		public override bool CanPopulate(DataColumnInfo info) {
			DataColumn col = dataView.Table.Columns[info.ColumnName];
			if(col == null) return false;
			return col.ColumnMapping != MappingType.Hidden;
		}
		bool CorrectNodeID(int nodeID) {
			return nodeID >= 0 && nodeID < dataView.Count; 
		}
		public override object GetValue(int nodeID, object columnID) {
			if(columnID == null || !CorrectNodeID(nodeID)) return DBNull.Value;
			string columnName = DataHelper.GetColumnNameByColumnID(columnID);
			DataColumnInfo info = GetDataColumnInfo(columnName);
			if(info == null)
				return DBNull.Value;
			if(info.IsUnbound)
				return info.Descriptor.GetValue(nodeID);
			DataRow row = dataView[nodeID].Row;
			return row[columnName];
		}
		protected override bool SetValueCore(int nodeID, object columnID, object value) {
			SetValueCore(nodeID, DataHelper.GetColumnNameByColumnID(columnID), value);
			return false;
		}
		protected override void SetValueCore(DataColumnInfo info, int nodeID, object listItem, object value) {
			SetValueCore(nodeID, info.ColumnName, value);
		}
		void SetValueCore(int nodeID, string columnName, object value) {
			DataColumnInfo info = GetDataColumnInfo(columnName);
			if(info != null && info.IsUnbound) {
				info.Descriptor.SetValue(nodeID, value);
				return;
			}
			dataView[nodeID][columnName] = value;
		}
		protected override int AppendCore() {
			DataRowView drv = dataView.AddNew();
			return (DataList.Count - 1);
		}
		protected override void RemoveFromSource(int nodeID) {
			if(dataView.Count == 0) return; 
			DataRow row = dataView[nodeID].Row;
			if(row != null)
				row.Delete();
		}
		protected override void ClearCore() {
			dataView.Table.Rows.Clear();
			base.ClearCore();
		}
		protected override object ConvertValue(object value, object columnID) {
			if(value == null) value = DBNull.Value;
			return base.ConvertValue(value, columnID);
		}
		public override object GetValues(int nodeID) {
			if(!CorrectNodeID(nodeID)) return null;
			return dataView[nodeID].Row.ItemArray; 
		}
		public override object GetDataRow(int nodeID) {
			if(!CorrectNodeID(nodeID)) return null;
			return dataView[nodeID]; 
		}
		protected override IDataErrorInfo GetDataErrorInfoObject(int nodeID) {
			if(!CorrectNodeID(nodeID)) return null;
			return (dataView[nodeID] as IDataErrorInfo);
		}
		public override IEditableObject GetEditableObject(int nodeID) { return (dataView[nodeID] as IEditableObject); }
		protected override object GetServiceFieldDescriptor(string fieldName) {
			if(dataView.Table != null && dataView.Table.Columns[fieldName] != null)
				return fieldName;
			return null;
		}
		protected override object GetKeyValue(int rowIndex, object fieldDescriptor) { return GetServiceFieldValue(rowIndex, fieldDescriptor, AbsentValue); }
		protected override object GetParentValue(int rowIndex, object fieldDescriptor) { return GetServiceFieldValue(rowIndex, fieldDescriptor, DataHelper.RootValue); }
		object GetServiceFieldValue(int rowIndex, object fieldDescriptor, object errorValue) {
			try {
				if(dataView.Table != null && dataView.Table.Columns[(string)fieldDescriptor] == null)
					return errorValue;
				return GetRowCellValue(rowIndex, fieldDescriptor); }
			catch { return errorValue; }
		}
		private object GetRowCellValue(int rowIndex, object fieldDescriptor) { return dataView[rowIndex].Row[(string)fieldDescriptor]; }
		public override bool CanAdd { get { return dataView.AllowNew; } }
		public override bool CanClear { get { return dataView.AllowDelete; } }
		public override IList DataList { get { return dataView; } }
		protected internal override bool SupportNotifications { get { return true; } }
	}
	public class ListData : TreeListBoundData {
		IList list;
		public ListData(TreeListDataHelper dataHelper, IList list)
			: base(dataHelper) {
			this.list = list;
			BindingList = (list as IBindingList);
		}
		protected override DataColumnInfoCollection CreateColumns() { return new ListDataColumnInfoCollection(this); }
		public override object GetValue(int nodeID, object columnID) {
			return GetRowCellValue(nodeID, GetPropertyDescriptorByColumnID(columnID), null);
		}
		PropertyDescriptor GetPropertyDescriptorByColumnID(object columnID) {
			DataColumnInfo info = Columns.GetColumnByID(columnID);
			return (info == null ? null : info.Descriptor);
		}
		protected override bool SetValueCore(int nodeID, object columnID, object value) {
			DataColumnInfo info = Columns.GetColumnByID(columnID);
			object listItem = GetDataRow(nodeID);
			if(BindingList != null) {
				SetValueCore(info, nodeID, listItem, value);
				return false;
			}
			object oldValue = info.Descriptor.GetValue(listItem);
			SetValueCore(info, nodeID, listItem, value);
			return !object.Equals(oldValue, value);
		}
		protected override void SetValueCore(DataColumnInfo info, int nodeID, object listItem, object value) {
			if(info.IsUnbound)
				info.Descriptor.SetValue(nodeID, value);
			else 
				info.Descriptor.SetValue(listItem, value);
		}
		protected override void RemoveFromSource(int nodeID) {
			if(BindingList == null)
				DataHelper.CheckCurrencyManagerPosition(nodeID);
			DataList.RemoveAt(nodeID);
		}
		protected override void ClearCore() {
			DataList.Clear();
			base.ClearCore();
		}
		protected override int AppendCore() {
			object listItem = null;
			if(BindingList != null)
				listItem = BindingList.AddNew();
			else {
				object firstIt = GetDataRow(0);
				try {
					listItem = Activator.CreateInstance(firstIt.GetType());
					if(listItem == null) return -1;
					DataList.Add(listItem);
				}
				catch { return -1; }
			}
			return DataList.IndexOf(listItem);
		}
		protected override bool CanSetValue(object columnID) {
			if(GetPropertyDescriptorByColumnID(columnID) == null) return false;
			return base.CanSetValue(columnID);
		}
		protected override object ConvertValue(object value, object columnID) {
			object result = base.ConvertValue(value, columnID);
			BindingSource bs = DataList as BindingSource;
			if(bs != null && bs.SyncRoot is DataView && result == null) result = DBNull.Value;
			Type objType = (result == null ? null : result.GetType());
			PropertyDescriptor p = GetPropertyDescriptorByColumnID(columnID);
			TypeConverter converter = p.Converter;
			if(converter != null && converter.CanConvertFrom(objType))
				result = converter.ConvertFrom(value);
			else if(value is IConvertible && !Convert.IsDBNull(value)) {
				Type columnType = p.PropertyType;
				if(columnType != null) {
					Type nullableUnderlyingType = Nullable.GetUnderlyingType(columnType);
					if(nullableUnderlyingType != null) 
						columnType = nullableUnderlyingType;
				}
				result = Convert.ChangeType(value, columnType);
			}
			return result;
		}
		protected override object GetServiceFieldDescriptor(string fieldName) {
			return Columns[fieldName] == null ? null : Columns[fieldName].Descriptor;
		}
		protected override object GetKeyValue(int rowIndex, object fieldDescriptor) {
			return GetRowCellValue(rowIndex, (PropertyDescriptor)fieldDescriptor, AbsentValue);
		}
		protected override object GetParentValue(int rowIndex, object fieldDescriptor) {
			return GetRowCellValue(rowIndex, (PropertyDescriptor)fieldDescriptor, DataHelper.RootValue);
		}
		private object GetRowCellValue(int rowIndex, PropertyDescriptor p, object initial) {
			object cellValue = initial;
			if(p != null) {
				if(TreeListUnboundPropertyDescriptor.IsUnbound(p))
					return p.GetValue(rowIndex);
				object row = GetDataRow(rowIndex);
				if(row != null)
					cellValue = p.GetValue(row);
			}
			return cellValue;
		}
		public override object GetDataRow(int nodeID) {
			if(nodeID < 0) return null;
			if(DataList.Count > nodeID)
				return DataList[nodeID];
			return null;
		}
		public override bool CanAdd {
			get {
				if(BindingList != null)
					return BindingList.AllowNew;
				return DataList.Count > 0;
			}
		}
		public override bool CanClear {
			get {
				if(BindingList != null)
					return BindingList.AllowRemove;
				return true;
			}
		}
		public override void Dispose() {
			this.list = null;
			base.Dispose();
		}
		public override IList DataList { get { return list; } }
		public virtual bool CanUseFastProperties { get { return DataHelper.IsDesignMode; } }
		protected internal override bool SupportNotifications { get { return BindingList != null; } }
	}
	public class UnboundData : TreeListData {
		ArrayList DataStore;
		ViewInfo.VisibleColumnsList synchColumns;
		public UnboundData(TreeListDataHelper dataHelper)
			: base(dataHelper) {
			this.DataStore = new ArrayList();
			this.synchColumns = new ViewInfo.VisibleColumnsList();
			DataHelper.Columns.CollectionChanged += new CollectionChangeEventHandler(Columns_Changed);
			SynchronizeColumns(DataHelper.Columns);
		}
		public override void Dispose() {
			base.Dispose();
			DataHelper.Columns.CollectionChanged -= new CollectionChangeEventHandler(Columns_Changed);
			DataStore.Clear();
			SynchColumns.Clear();
		}
		protected override DataColumnInfoCollection CreateColumns() { return new UnboundDataColumnInfoCollection(this); }
		public override void CreateNodes(TreeListNode parent) { }
		public override bool CurrencyManagerWasReset(CurrencyManager manager, ItemChangedEventArgs e) { return false; }
		public override bool IsValidColumnHandle(int columnHandle) { return true; }
		public override object GetValue(int nodeID, object columnID) {
			if(nodeID > -1 && nodeID < DataStore.Count) {
				return GetValue(nodeID, DataHelper.GetIndexByColumnID(columnID));
			}
			return null;
		}
		object GetValue(int nodeID, int columnID) {
			UnboundDataRow row = GetUnboundDataRow(nodeID);
			return row[columnID];
		}
		protected override void SetValuesCore(int nodeID, object[] values) {
			AddDataRow(values);
		}
		protected override bool SetValueCore(int nodeID, object columnID, object value) {
			if(nodeID < 0 || nodeID > DataStore.Count - 1) return false;
			return SetValueCore(nodeID, DataHelper.GetIndexByColumnID(columnID), value, true);
		}
		bool SetValueCore(int nodeID, int columnID, object value, bool throwExc) {
			UnboundDataRow row = GetUnboundDataRow(nodeID);
			return row.SetValue(columnID, value, throwExc);
		}
		protected override void ClearCore() { DataStore.Clear(); }
		protected override void OnRemoveNode(TreeListNode node) {
			DataHelper.ListChanged(DataStore, new ListChangedEventArgs(ListChangedType.ItemDeleted, node.Id, -1));
		}
		protected override void RemoveFromSource(int nodeID) {
			DataStore.RemoveAt(nodeID);
		}
		protected override void RemoveRange(NodesIdInfo range) {
			DataStore.RemoveRange(range.StartId, range.Length);
		}
		protected virtual UnboundDataRow AddDataRow(object[] values) {
			UnboundDataRow result = null;
			if(values == null)
				result = new UnboundDataRow(DataHelper.Columns.Count, this);
			else result = new UnboundDataRow(values, this);
			DataStore.Add(result);
			return result;
		}
		protected override int AppendCore() { return DataStore.Count; }
		public override object GetDataRow(int nodeID) {
			UnboundDataRow row = (UnboundDataRow)GetUnboundDataRow(nodeID);
			return (row == null ? null : row.ItemArray);
		}
		UnboundDataRow GetUnboundDataRow(int nodeID) {
			if(nodeID < 0 || nodeID >= DataStore.Count) return null;
			return (UnboundDataRow)DataStore[nodeID];
		}
		public override IEditableObject GetEditableObject(int nodeID) { return GetUnboundDataRow(nodeID); }
		protected internal virtual void Columns_Changed(object sender, CollectionChangeEventArgs e) {
			TreeListColumnCollection columns = sender as TreeListColumnCollection;
			if(columns.isClearing)
				return;
			ArrayList indexesToAdd = new ArrayList();
			ArrayList indexesToDelete = new ArrayList();
			ArrayList indexesToMove = new ArrayList();
			for(int i = 0; i < SynchColumns.Count; i++) {
				TreeListColumn col = SynchColumns[i];
				int ind = columns.IndexOf(col);
				if(ind == -1) indexesToDelete.Add(i);
				else if(ind != i && indexesToDelete.Count == 0) indexesToMove.Add(new int[2] { i, ind });
			}
			for(int i = 0; i < columns.Count; i++) {
				TreeListColumn col = columns[i];
				int ind = SynchColumns.IndexOf(col);
				if(ind == -1) indexesToAdd.Add(i);
			}
			SynchronizeDataStorage(indexesToAdd, indexesToDelete, indexesToMove);
			SynchronizeColumns(columns);
		}
		protected internal override void OnClearColumns() {
			for(int i = 0; i < DataStore.Count; i++) {
				UnboundDataRow row = GetUnboundDataRow(i);
				row.ItemArray.Clear();
			}
			SynchColumns.Clear();
			PopulateColumns();
		}
		int[] GetIntArray(ArrayList arr) { 
			if(arr == null)
				return new int[] { };
			return (int[])arr.ToArray(typeof(int)); 
		}
		private void SynchronizeDataStorage(ArrayList indexesToAdd, ArrayList indexesToDelete, ArrayList indexesToMove) {
			int[] add = GetIntArray(indexesToAdd);
			int[] delete = GetIntArray(indexesToDelete);
			for(int i = 0; i < DataStore.Count; i++) {
				UnboundDataRow row = GetUnboundDataRow(i);
				row.SynchronizeItemArray(add, delete, indexesToMove);
			}
		}
		private void SynchronizeColumns(TreeListColumnCollection columns) {
			SynchColumns.Clear();
			SynchColumns.AddRange(columns);
			PopulateColumns();
		}
		public override void OnColumnTypeChanged(TreeListColumn column) {
			PopulateColumns();
			ConvertColumnData(column);
			DataHelper.ListChanged(DataStore, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, column.AbsoluteIndex));
		}
		protected virtual void ConvertColumnData(TreeListColumn column) {
			for(int i = 0; i < DataList.Count; i++)
				SetValueCore(i, column.AbsoluteIndex, GetValue(i, column.AbsoluteIndex), false);
		}
		internal Type GetColumnType(TreeListColumn column) {
			return Columns[column.AbsoluteIndex].Type;
		}
		protected internal ViewInfo.VisibleColumnsList SynchColumns { get { return synchColumns; } }
		public override bool IsUnboundMode { get { return true; } }
		public override bool CanAdd { get { return true; } }
		public override bool CanClear { get { return true; } }
		public override IList DataList { get { return DataStore; } }
	}
	public class UnboundDataRow : IEditableObject {
		UnboundData table;
		ArrayList itemArray, valuesCache;
		public UnboundDataRow(ICollection collection, UnboundData table)
			: this(table) {
			this.itemArray = new ArrayList(collection);
		}
		public UnboundDataRow(int valuesCount, UnboundData table)
			: this(table) {
			this.itemArray = new ArrayList(valuesCount);
		}
		protected UnboundDataRow(UnboundData table) {
			this.table = table;
			this.valuesCache = null;
		}
		public virtual object this[int columnID] {
			get {
				if(columnID > -1 && columnID < Count)
					return ItemArray[columnID];
				return null;
			}
		}
		public virtual bool SetValue(int columnID, object value, bool throwExc) {
			if(columnID < 0 || columnID > Count - 1) return false;
			object oldVal = ItemArray[columnID];
			value = ConvertColumnValue(Table.SynchColumns[columnID], value, throwExc);
			ItemArray[columnID] = value;
			return !object.Equals(oldVal, value);
		}
		protected virtual object ConvertColumnValue(TreeListColumn column, object value, bool throwExc) {
			if(column.UnboundType == UnboundColumnType.Object || column.UnboundType == UnboundColumnType.Bound || TreeListData.IsNull(value)) return value;
			try {
				return Convert.ChangeType(value, Table.GetColumnType(column));
			}
			catch {
				if(throwExc) throw;
			}
			return null;
		}
		protected internal virtual void SynchronizeItemArray(int[] addIndexes, int[] deleteIndexes, ArrayList moveIndexes) {
			for(int i = 0; i < deleteIndexes.Length; i++)
				ItemArray.RemoveAt(deleteIndexes[i]);
			for(int i = 0; i < addIndexes.Length; i++) {
				if(addIndexes[i] > ItemArray.Count) ItemArray.Add(null);
				else ItemArray.Insert(addIndexes[i], null);
			}
			for(int i = 0; i < moveIndexes.Count; i++) {
				int[] inds = (int[])moveIndexes[i];
				object val = ItemArray[inds[0]];
				ItemArray.RemoveAt(inds[0]);
				ItemArray.Insert(inds[1], val);
			}
		}
		protected ArrayList ValuesCache { get { return valuesCache; } }
		protected UnboundData Table { get { return table; } }
		public int Count { get { return ItemArray.Count; } }
		public ArrayList ItemArray { get { return itemArray; } }
		void IEditableObject.BeginEdit() {
			if(ValuesCache != null) return;
			this.valuesCache = new ArrayList(ItemArray);
		}
		void IEditableObject.EndEdit() {
			if(ValuesCache == null) return;
			ValuesCache.Clear();
			this.valuesCache = null;
		}
		void IEditableObject.CancelEdit() {
			if(ValuesCache == null) return;
			OnBeforeCancelEdit();
			this.itemArray = ValuesCache;
			this.valuesCache = null;
		}
		protected virtual void OnBeforeCancelEdit() { }
	}
	public class VirtualDataRow : UnboundDataRow, IDXDataErrorInfo, IDataErrorInfo, IDisposable {
		object virtualNode;
		object virtualParent;
		IList list;
		ArrayList childrenCache;
		IBindingList bindingList;
		TreeListNode node;
		bool expanded;
		public IList Children { get { return list; } }
		public IList ChildrenCache { get { return childrenCache; } }
		protected new TreeListVirtualData Table { get { return base.Table as TreeListVirtualData; } }
		public object VirtualNode { get { return virtualNode; } set { virtualNode = value; } }
		public object VirtualParent { get { return virtualParent; } set { virtualParent = value; } }
		public TreeListNode Node { get { return node; } }
		internal bool Expanded { get { return this.expanded; } set { this.expanded = value; } }
		protected IBindingList BindingList {
			get { return bindingList; }
			set {
				if(BindingList != null) {
					BindingList.ListChanged -= new ListChangedEventHandler(ListChanged);
				}
				bindingList = value;
				if(BindingList != null) {
					BindingList.ListChanged += new ListChangedEventHandler(ListChanged);
				}
			}
		}
		public virtual bool CanAdd {
			get {
				if(BindingList != null) return BindingList.AllowNew;
				if(Children == null || Children.IsFixedSize || Children.IsReadOnly) return false;
				return true;
			}
		}
		public virtual bool CanRemove {
			get {
				if(BindingList != null) return BindingList.AllowRemove;
				if(Children == null || Children.IsFixedSize || Children.IsReadOnly) return false;
				return true;
			}
		}
		public virtual object AddNewChild() {
			if(!CanAdd) return null;
			object listItem = null;
			if(BindingList != null) {
				listItem = BindingList.AddNew();
			}
			else {
				if(Children.Count < 1) return null;
				object firstIt = Children[0];
				try {
					listItem = Activator.CreateInstance(firstIt.GetType());
					if(listItem == null) return null;
					Children.Add(listItem);
				}
				catch {
					return null;
				}
			}
			UpdateChildrenCache();
			return listItem;
		}
		public virtual bool RemoveChild(object child) {
			if(!CanRemove) return false;
			try {
				if(Children.Contains(child)) {
					Children.Remove(child);
				}
			}
			catch {
				return false;
			}
			UpdateChildrenCache();
			return true;
		}
		public virtual bool AddChild(object child) {
			if(child == null || !CanAdd) return false;
			try {
				Children.Add(child);
			}
			catch {
				return false;
			}
			UpdateChildrenCache();
			return true;
		}
		public VirtualDataRow(ICollection collection, TreeListVirtualData table, object virtualNode, object virtualParent, IList children, TreeListNode node)
			: base(collection, table) {
			this.virtualNode = virtualNode;
			this.virtualParent = virtualParent;
			this.node = node;
			this.expanded = false;
			this.AssignChildren(children);
			this.UpdateChildrenCache();
		}
		public VirtualDataRow(int valuesCount, TreeListVirtualData table, object virtualNode, object virtualParent, IList children, TreeListNode node)
			: base(valuesCount, table) {
			this.virtualNode = virtualNode;
			this.virtualParent = virtualParent;
			this.node = node;
			this.expanded = false;
			this.AssignChildren(children);
			this.UpdateChildrenCache();
		}
		protected internal virtual void AssignChildren(IList children) {
			this.BindingList = children as IBindingList;
			this.list = children;
			if(list != null) childrenCache = new ArrayList(list);
		}
		public void UpdateChildrenCache() {
			if(list == null) return;
			if(childrenCache != null) childrenCache.Clear();
			childrenCache = new ArrayList(list);
		}
		public void Dispose() {
			Table.RemoveFromCache(this);
			if(bindingList != null)
				bindingList.ListChanged -= new ListChangedEventHandler(ListChanged);
			bindingList = null;
			list = null;
			virtualNode = null;
			virtualParent = null;
			node = null;
			if(childrenCache != null) childrenCache.Clear();
		}
		void ListChanged(object sender, ListChangedEventArgs e) {
			Table.OnListChanged(this, e);
			UpdateChildrenCache();
		}
		public override bool SetValue(int columnID, object value, bool throwExc) {
			if((columnID < 0) || (columnID > (Count - 1))) return false;
			object oldVal = ItemArray[columnID];
			value = ConvertColumnValue(Table.SynchColumns[columnID], value, throwExc);
			bool result = Table.SetCellData(oldVal, value, virtualNode, Table.SynchColumns[columnID], Table.GetOwnedCollection(this) as DevExpress.XtraTreeList.TreeList.IVirtualTreeListData);
			if(!result) return false;
			ItemArray[columnID] = value;
			return !object.Equals(oldVal, value);
		}
		protected override void OnBeforeCancelEdit() {
			for(int i = 0; i < Table.SynchColumns.Count; i++) {
				Table.SetCellData(ItemArray[i], ValuesCache[i], VirtualNode, Table.SynchColumns[i], Table.GetOwnedCollection(this) as DevExpress.XtraTreeList.TreeList.IVirtualTreeListData);
			}
		}
		#region IDXDataErrorInfo Members
		void IDXDataErrorInfo.GetError(ErrorInfo info) {
			IDXDataErrorInfo errorInfo = virtualNode as IDXDataErrorInfo;
			if(errorInfo == null) return;
			errorInfo.GetError(info);
		}
		void IDXDataErrorInfo.GetPropertyError(string propertyName, ErrorInfo info) {
			IDXDataErrorInfo errorInfo = virtualNode as IDXDataErrorInfo;
			if(errorInfo == null) return;
			errorInfo.GetPropertyError(propertyName, info);
		}
		#endregion
		#region IDataErrorInfo Members
		string IDataErrorInfo.Error {
			get {
				IDataErrorInfo errorInfo = virtualNode as IDataErrorInfo;
				if(errorInfo == null) return null;
				return errorInfo.Error;
			}
		}
		string IDataErrorInfo.this[string columnName] {
			get {
				IDataErrorInfo errorInfo = virtualNode as IDataErrorInfo;
				if(errorInfo == null) return null;
				return errorInfo[columnName];
			}
		}
		#endregion
	}
	public class TreeListVirtualData : UnboundData {
		DevExpress.XtraTreeList.TreeList.TreeListVirtualDataHelper virtualDataHelper;
		bool fireEvents = true;
		VirtualDataRow root = null;
		bool allowDirectSetCellValue = false;
		Dictionary<object, VirtualDataRow> virtualNodeToDataRowCache = new Dictionary<object, VirtualDataRow>();
		public TreeListVirtualData(TreeListDataHelper helper, TreeList.TreeListVirtualDataHelper virtualDataHelper)
			: base(helper) {
			this.virtualDataHelper = virtualDataHelper;
		}
		protected internal TreeList.TreeListVirtualDataHelper VirtualDataHelper { get { return virtualDataHelper; } }
		protected VirtualDataRow Root { get { return root; } }
		#region VirtualTreeList interface implementation
		protected object GetCellData(object node, TreeListColumn column, TreeList.IVirtualTreeListData owner) {
			if(fireEvents) return VirtualDataHelper.GetCellDataViaEvent(node, column);
			return VirtualDataHelper.GetCellDataViaInterface(node, column, owner);
		}
		protected internal bool SetCellData(object oldValue, object newValue, object node, TreeListColumn column, TreeList.IVirtualTreeListData owner) {
			if(allowDirectSetCellValue || node == null) return true;
			if(fireEvents) return VirtualDataHelper.SetCellDataViaEvent(oldValue, newValue, node, column);
			return VirtualDataHelper.SetCellDataViaInterface(oldValue, newValue, node, column, owner);
		}
		protected IList GetChildren(object node, TreeList.IVirtualTreeListData owner) {
			if(fireEvents) return VirtualDataHelper.GetChildrenViaEvent(node);
			return VirtualDataHelper.GetChildrenViaInterface(node, owner);
		}
		#endregion
		protected override void ClearCore() {
			ClearInternal();
			base.ClearCore();
		}
		void ClearInternal() {
			if(Root != null) Root.Dispose();
			foreach(VirtualDataRow row in DataList) row.Dispose();
		}
		protected override void RemoveFromSource(int nodeID) {
			VirtualDataRow row = GetVirtualDataRow(nodeID);
			if(row != null) {
				RemoveFromParent(row);
				row.Dispose();
			}
			base.RemoveFromSource(nodeID);
		}
		protected virtual void RemoveFromParent(VirtualDataRow row) {
			if(row == Root || lockChanged) return;
			lockChanged = true;
			try {
				VirtualDataRow parentRow = GetOwner(row);
				if(parentRow == null)
					Root.RemoveChild(row.VirtualNode);
				else
					parentRow.RemoveChild(row.VirtualNode);
			}
			finally {
				lockChanged = false;
			}
		}
		protected override void RemoveRange(NodesIdInfo range) {
			for(int i = range.EndId; i >= range.StartId; i--) {
				VirtualDataRow row = DataList[i] as VirtualDataRow;
				if(row != null) {
					RemoveFromParent(row);
					row.Dispose();
				}
			}
			base.RemoveRange(range);
		}
		protected internal VirtualDataRow GetOwner(VirtualDataRow row) {
			if(row.VirtualParent == null) return Root;
			return GetVirtualDataRow(row.VirtualParent);
		}
		protected internal IList GetOwnedCollection(VirtualDataRow row) {
			VirtualDataRow parent = GetOwner(row);
			return parent == null ? Root.Children : parent.Children;
		}
		protected internal VirtualDataRow GetVirtualDataRow(int nodeID) {
			if((nodeID < 0) || (nodeID >= DataList.Count)) return null;
			return DataList[nodeID] as VirtualDataRow;
		}
		protected internal void RemoveFromCache(VirtualDataRow row) {
			if(row == null || row.VirtualNode == null) return;
			virtualNodeToDataRowCache.Remove(row.VirtualNode);
		}
		protected VirtualDataRow GetVirtualDataRow(object virtualNode) {
			VirtualDataRow row = null;
			if(virtualNode != null && virtualNodeToDataRowCache.TryGetValue(virtualNode, out row))
				return row;
			return null;
		}
		void AddVirtualDataRowToCache(object virtualNode, VirtualDataRow row) {
			if(virtualNode == null) return;
			virtualNodeToDataRowCache[virtualNode] = row;
		}
		public override object GetDataRow(int nodeID) {
			VirtualDataRow row = GetVirtualDataRow(nodeID);
			return ((row == null) ? null : row.VirtualNode);
		}
		protected override UnboundDataRow AddDataRow(object[] values) {
			return AddDataRow(values, null, null, null, null);
		}
		protected internal override void SetParentRelation(TreeListNode childNode, TreeListNode parentNode) {
			if(lockChanged) return;
			VirtualDataRow child = GetVirtualDataRow(childNode.Id);
			VirtualDataRow childParent = GetOwner(child);
			VirtualDataRow parent = (parentNode == null ? Root : GetVirtualDataRow(parentNode.Id));
			if(childParent.CanRemove && parent.CanAdd) {
				lockChanged = true;
				try {
					object item = child.VirtualNode;
					childParent.RemoveChild(item);
					parent.AddChild(item);
					if(parent.Children.Contains(item))
						child.VirtualParent = parent.VirtualNode;
				}
				catch { }
				finally {
					lockChanged = false;
				}
			}
		}
		protected virtual UnboundDataRow AddDataRow(object[] values, object virtualNode, object virtualParent, IList virtualChildCache, TreeListNode node) {
			VirtualDataRow result = null;
			if(values == null)
				result = new VirtualDataRow(base.DataHelper.Columns.Count, this, virtualNode, virtualParent, virtualChildCache, node);
			else
				result = new VirtualDataRow(values, this, virtualNode, virtualParent, virtualChildCache, node);
			DataList.Add(result);
			AddVirtualDataRowToCache(virtualNode, result);
			return result;
		}
		public override TreeListNode Append(TreeListNode parent, object values, object tag) {
			VirtualDataRow parentRow = (parent == null ? Root : GetVirtualDataRow(parent.Id));
			if(parentRow == null) return null;
			object child = parentRow.AddNewChild();
			if(child == null || !parentRow.Expanded) return null;
			if(parentRow.Children is IBindingList) return GetVirtualDataRow(child).Node;
			IList children = GetChildren(child, parentRow.Children as TreeList.IVirtualTreeListData);
			TreeListNode appendedNode = AppendInternal(parent, values, child, parentRow.VirtualNode, children);
			if(children != null && children.Count > 0) appendedNode.HasChildren = true;
			return appendedNode;
		}
		protected internal virtual TreeListNode AppendInternal(TreeListNode parent, object values, object virtualNode, object virtualParent, IList virtualChildCache) {
			TreeListNode result = null;
			if(CanAdd) {
				int rowIndex = AppendCore();
				if(rowIndex != -1) {
					result = DataHelper.CreateNode(rowIndex, parent);
					try {
						AddDataRow(ConvertValues(values), virtualNode, virtualParent, virtualChildCache, result);
					}
					catch { }
					EndDataRowEdit(rowIndex);
					VirtualDataHelper.CheckNodeVisiblity(result);
				}
			}
			return result;
		}
		public override void CreateNodes(TreeListNode parent) {
			if(parent == null) {
				if(Root != null) Root.Dispose();
				base.DataHelper.ClearNodes();
			}
			CreateRootNodes();
		}
		public virtual void CreateRootNodes() {
			virtualNodeToDataRowCache.Clear();
			object source = VirtualDataHelper.GetDataSource();
			if(source == null) return;
			if(source is TreeList.IVirtualTreeListData) fireEvents = false; else fireEvents = true;
			IList children = (source is IList ? source as IList : GetChildren(source, null));
			if(children == null) return;
			this.root = CreateRoot(source, children);
			CreateChildren(Root, !VirtualDataHelper.EnableDynamicLoading);
			Root.Expanded = true;
		}
		protected virtual VirtualDataRow CreateRoot(object source, IList children) {
			return new VirtualDataRow(0, this, source, null, children, null);
		}
		protected void CreateChildren(VirtualDataRow parentRow, bool recursive = false) {
			if(parentRow == null || parentRow.Children == null) return;
			foreach(object child in parentRow.Children) {
				if(child == null) continue;
				IList children1 = GetChildren(child, parentRow.Children as TreeList.IVirtualTreeListData);
				TreeListNode appendedNode = AppendInternal(parentRow.Node, GetCellsData(child, parentRow.Children as TreeList.IVirtualTreeListData), child, parentRow.VirtualNode, children1);
				if(children1 != null && children1.Count > 0) appendedNode.HasChildren = true;
				if(recursive) {
					VirtualDataRow row = GetVirtualDataRow(appendedNode.Id);
					CreateChildren(row, recursive);
					row.Expanded = true;
				}
			}
		}
		public virtual void OpenChildNodes(TreeListNode node) {
			if(node == null) return;
			VirtualDataRow row = GetVirtualDataRow(node.Id);
			if(row == null || row.Expanded) return;
			CreateChildren(row);
			row.Expanded = true;
		}
		protected object[] GetCellsData(object node, TreeList.IVirtualTreeListData parent) {
			object[] cell = new object[DataHelper.Columns.Count];
			for(int i = 0; i < DataHelper.Columns.Count; i++) {
				cell[i] = GetCellData(node, DataHelper.Columns[i], parent);
			}
			return cell;
		}
		void SetCellValuesInternal(TreeListNode node, object[] data) {
			if(node.Id == -1) return;
			for(int i = 0; i < DataHelper.Columns.Count; i++) SetValue(node.Id, i, data[i], false);
		}
		public virtual void CheckNodeCellsData(TreeListNode node) {
			if(node == null) return;
			allowDirectSetCellValue = true;
			try {
				VirtualDataRow parent = GetVirtualDataRow(node.Id);
				if(parent == null || parent.Children == null) return;
				foreach(object child in parent.Children) {
					VirtualDataRow childRow = GetVirtualDataRow(child);
					if(childRow == null) continue;
					SetCellValuesInternal(childRow.Node, GetCellsData(childRow.VirtualNode, GetOwnedCollection(childRow) as TreeList.IVirtualTreeListData));
				}
			}
			finally {
				allowDirectSetCellValue = false;
			}
			if(node.Expanded && node.HasChildren) {
				foreach(TreeListNode childNode in node.Nodes) {
					if(childNode.Expanded) CheckNodeCellsData(childNode);
				}
			}
		}
		#region IBindingList support implementation
		protected internal virtual void OnListChanged(object sender, ListChangedEventArgs e) {
			VirtualDataRow row = sender as VirtualDataRow;
			if(row == null || lockChanged) return;
			lockChanged = true;
			VirtualDataHelper.BeginUpdate();
			try {
				switch(e.ListChangedType) {
					case ListChangedType.ItemAdded:
						OnChildAdded(row, e.NewIndex);
						break;
					case ListChangedType.ItemDeleted:
						OnChildDeleted(row, e.NewIndex);
						break;
					case ListChangedType.ItemChanged:
						OnChildChanged(row, e.NewIndex);
						break;
					case ListChangedType.ItemMoved:
						OnChildMoved(row, e.OldIndex, e.NewIndex);
						break;
					case ListChangedType.Reset:
						OnChildrenReset(row);
						break;
				}
			}
			finally {
				VirtualDataHelper.EndUpdate();
				lockChanged = false;
			}
		}
		protected virtual void OnChildAdded(VirtualDataRow parent, int childIndex) {
			if(parent == null || parent.Children == null) return;
			if(parent != Root) parent.Node.HasChildren = true;
			if(!parent.Expanded) return;
			object newChild = parent.Children[childIndex];
			if(newChild == null) return;
			IList children = GetChildren(newChild, parent.Children as TreeList.IVirtualTreeListData);
			TreeListNode appendedNode = AppendInternal(parent.Node, GetCellsData(newChild, parent.Children as TreeList.IVirtualTreeListData), newChild, parent.VirtualNode, children);
			DataHelper.SetNodeIndex(appendedNode, childIndex);
			VirtualDataHelper.CheckAutoFocusNewNode(appendedNode);
			if(children != null && children.Count > 0) appendedNode.HasChildren = true;
			if(!VirtualDataHelper.EnableDynamicLoading)
				CreateChildren(GetVirtualDataRow(appendedNode.Id), !VirtualDataHelper.EnableDynamicLoading);
			VirtualDataHelper.OnNodeCountChanged();
		}
		protected virtual void OnChildDeleted(VirtualDataRow parent, int deletedIndex) {
			if(parent == null || !parent.Expanded) return;
			if(parent == Root)
				DataHelper.DeleteNode(GetVirtualDataRow(Root.ChildrenCache[deletedIndex]).Node, true);
			else
				DataHelper.DeleteNode(GetVirtualDataRow(parent.ChildrenCache[deletedIndex]).Node, true);
		}
		protected virtual void OnChildChanged(VirtualDataRow parent, int childIndex) {
			if(parent == null || !parent.Expanded) return;
			allowDirectSetCellValue = true;
			try {
				VirtualDataRow row = (parent == Root ? GetVirtualDataRow(Root.ChildrenCache[childIndex]) : GetVirtualDataRow(parent.ChildrenCache[childIndex]));
				if(row == null) return;
				SetCellValuesInternal(row.Node, GetCellsData(row.VirtualNode, parent.Children as TreeList.IVirtualTreeListData));
			}
			finally {
				allowDirectSetCellValue = false;
			}
		}
		protected virtual void OnChildMoved(VirtualDataRow parent, int oldIndex, int newIndex) {
			if(parent == null || oldIndex == newIndex) return;
			VirtualDataRow oldRow = (parent == Root ? GetVirtualDataRow(Root.ChildrenCache[oldIndex]) : GetVirtualDataRow(parent.ChildrenCache[oldIndex])); 
			if(oldRow == null) return;
			DataHelper.SetNodeIndex(oldRow.Node, newIndex);
		}
		bool lockChanged = false;
		protected virtual bool CanReset(VirtualDataRow parent) {
			return true;
		}
		void CollectMovedNodes(VirtualDataRow parent, ArrayList nodeList) {
			if(parent.ChildrenCache == null) return;
			for(int i = 0; i < parent.ChildrenCache.Count; i++) {
				VirtualDataRow row = GetVirtualDataRow(parent.ChildrenCache[i]);
				if(row != null) {
					if(!row.Node.HasAsParent(parent.Node)) nodeList.Add(row.Node);
					CollectMovedNodes(row, nodeList);
				}
			}
		}
		protected virtual void OnChildrenReset(VirtualDataRow parent) {
			if(parent == null || !CanReset(parent)) return;
			try {
				ArrayList nodesToDelete = new ArrayList();
				CollectMovedNodes(parent, nodesToDelete);
				foreach(TreeListNode node in nodesToDelete) {
					if(node != null) DataHelper.DeleteNode(node, true);
				}
				object savedParentVirtualNode = parent.VirtualNode;
				if(parent == Root) {
					DataHelper.ClearNodes();
				}
				else {
					foreach(object child in parent.ChildrenCache) {
						if(child == null) continue;
						VirtualDataRow row = GetVirtualDataRow(child);
						if(row != null) DataHelper.DeleteNode(row.Node, true);
					}
				}
				IList children = null;
				parent.VirtualNode = savedParentVirtualNode;
				if(parent == Root && parent.VirtualNode is IList)
					children = parent.VirtualNode as IList;
				else
					children = GetChildren(parent.VirtualNode, parent.Children as TreeList.IVirtualTreeListData);
				if(children != null && children.Count > 0 && parent.Node != null) parent.Node.HasChildren = true;
				parent.AssignChildren(children);
				if(parent.Expanded)
					CreateChildren(parent, !VirtualDataHelper.EnableDynamicLoading);
			}
			catch { }
			VirtualDataHelper.OnNodeCountChanged();
		}
		public override void Dispose() {
			ClearInternal();
			base.Dispose();
		}
		#endregion
		protected internal void RefreshRowData(int id) {
			VirtualDataRow row = GetVirtualDataRow(id);
			if(row == null)
				return;
			allowDirectSetCellValue = true;
			try {
				SetCellValuesInternal(row.Node, GetCellsData(row.VirtualNode, GetOwnedCollection(row) as TreeList.IVirtualTreeListData));
			}
			finally {
				allowDirectSetCellValue = false;
			}
		}
		protected internal void RefreshCellData(int id, object columnId) {
			VirtualDataRow row = GetVirtualDataRow(id);
			if(row == null)
				return;
			allowDirectSetCellValue = true;
			try {
				TreeListColumn column = DataHelper.GetTreeListColumnByID(columnId);
				if(column == null) 
					return;
				SetValue(id, column, GetCellData(row.VirtualNode, column, GetOwnedCollection(row) as TreeList.IVirtualTreeListData), false);
			}
			finally {
				allowDirectSetCellValue = false;
			}
		}
	}
}

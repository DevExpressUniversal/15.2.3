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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Sql.DataApi;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
namespace DevExpress.DataAccess.Native.Sql {
	public class ResultSet : IResultSet, ITypedList, IBindingList, ICloneable {
		class TypedListPropertyDescriptor : PropertyDescriptor {
			readonly string displayName;
			public TypedListPropertyDescriptor(string name, string displayName) : base(name, new Attribute[0]) {
				this.displayName = displayName;
			}
			#region Overrides of PropertyDescriptor
			public override bool CanResetValue(object component) {
				return false;
			}
			public override object GetValue(object component) {
				return ((ResultSet) component).tables[Name];
			}
			public override void ResetValue(object component) {
				throw new NotSupportedException();
			}
			public override void SetValue(object component, object value) {
				throw new NotSupportedException();
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
			public override Type ComponentType {
				get { return typeof(ResultSet); }
			}
			public override bool IsReadOnly {
				get { return true; }
			}
			public override Type PropertyType {
				get { return typeof(ResultTable); }
			}
			#endregion
			#region Overrides of MemberDescriptor
			public override string DisplayName {
				get { return this.displayName ?? base.DisplayName; }
			}
			#endregion
		}
		ConcurrentDictionary<string, ResultTable> tables = new ConcurrentDictionary<string, ResultTable>();
		readonly SqlDataSource dataSource;
		public ResultSet() : this(null) { 
		}
		public ResultSet(SqlDataSource dataSource) {
			this.dataSource = dataSource;
		}
		internal void SetTables(IEnumerable<ResultTable> tables) {
			SetTablesCore(tables);
			RaiseChanged();
		}
		internal void SetTablesCore(IEnumerable<ResultTable> tables) {
			var newTables = new ConcurrentDictionary<string, ResultTable>();
			foreach(ResultTable table in tables) {
				ResultTable resTable = table;
				if(this.tables.ContainsKey(resTable.TableName) && this.tables[resTable.TableName] != resTable) {
					this.tables[resTable.TableName].Copy(resTable);
					resTable = this.tables[resTable.TableName];
				}
				newTables.GetOrAdd(resTable.TableName, resTable);
			}
			this.tables = newTables;
		}
		public ResultSet(SqlDataSource dataSource, IEnumerable<ResultTable> collection) {
			this.dataSource = dataSource;
			this.tables = new ConcurrentDictionary<string, ResultTable>(collection.ToDictionary(tl => tl.TableName));
		}
		#region Implementation of IEnumerable<out ITable>
		IEnumerator<ITable> IEnumerable<ITable>.GetEnumerator() { return Tables.GetEnumerator(); }
		#endregion
		#region Implementation of IResultSet
		ITable IResultSet.this[int index] { get { return Tables.ElementAt(index); } }
		ITable IResultSet.this[string name] {
			get {
				ITable table;
				if(!((IResultSet)this).TryGetTable(name, out table))
					throw new ArgumentOutOfRangeException(string.Format("Table '{0}' not found.", name));
				return table;
			}
		}
		bool IResultSet.TryGetTable(string name, out ITable table) {
			table = Tables.FirstOrDefault(resultTable => resultTable.TableName == name);
			return table != null;
		}
		#endregion
		#region Implementation of ITypedList
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return Name;
		}
		public void ApplyRelations(MasterDetailInfoCollection relations) {
			foreach (var table in Tables) {
				table.Details.Clear();
			}
			foreach(MasterDetailInfo relation in relations) {
				if(!Contains(relation.MasterQueryName) || !Contains(relation.DetailQueryName))
					continue;
				ResultTable master = this[relation.MasterQueryName];
				ResultTable detail = this[relation.DetailQueryName];
				if(master == null || detail == null)
					continue;
				if(relation.HasCustomName)
					master.AddDetail(detail, relation.KeyColumns, relation.Name);
				else
					master.AddDetail(detail, relation.KeyColumns);
			}
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(listAccessors == null || listAccessors.Length == 0)
				return
					new PropertyDescriptorCollection(
						this.tables.Values.Select(t => (PropertyDescriptor) new TypedListPropertyDescriptor(t.TableName, t.DisplayName)).ToArray());
			ResultTable typedList;
			if(!this.tables.TryGetValue(listAccessors.First().Name, out typedList))
				return new PropertyDescriptorCollection(null);
			return typedList.GetItemProperties(listAccessors.Skip(1).ToArray());
		}
		#endregion
		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() {
			yield return this;
		}
		#endregion
		#region Implementation of ICollection
		void ICollection.CopyTo(Array array, int index) {
			array.SetValue(this, index);
		}
		int ICollection.Count {
			get { return 1; }
		}
		object ICollection.SyncRoot {
			get { return this; }
		}
		bool ICollection.IsSynchronized {
			get { return false; }
		}
		#endregion
		#region Implementation of IList
		int IList.Add(object value) {
			throw new NotSupportedException();
		}
		bool IList.Contains(object value) {
			return value == this;
		}
		void IList.Clear() {
			throw new NotSupportedException();
		}
		int IList.IndexOf(object value) {
			return ((IList) this).Contains(value) ? 0 : -1;
		}
		void IList.Insert(int index, object value) {
			throw new NotSupportedException();
		}
		void IList.Remove(object value) {
			throw new NotSupportedException();
		}
		void IList.RemoveAt(int index) {
			throw new NotSupportedException();
		}
		object IList.this[int index] {
			get {
				if(index != 0)
					throw new IndexOutOfRangeException();
				return this;
			}
			set { throw new NotSupportedException(); }
		}
		bool IList.IsReadOnly {
			get { return true; }
		}
		bool IList.IsFixedSize {
			get { return true; }
		}
		#endregion
		#region Implementation of IBindingList
		public object AddNew() {
			throw new NotSupportedException();
		}
		public void AddIndex(PropertyDescriptor property) {
			throw new NotSupportedException();
		}
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotSupportedException();
		}
		public int Find(PropertyDescriptor property, object key) {
			throw new NotSupportedException();
		}
		public void RemoveIndex(PropertyDescriptor property) {
			throw new NotSupportedException();
		}
		public void RemoveSort() {
			throw new NotSupportedException();
		}
		public bool AllowNew {
			get { return false; }
		}
		public bool AllowEdit {
			get { return false; }
		}
		public bool AllowRemove {
			get { return false; }
		}
		public bool SupportsChangeNotification {
			get { return true; }
		}
		public bool SupportsSearching {
			get { return false; }
		}
		public bool SupportsSorting {
			get { return false; }
		}
		public bool IsSorted {
			get { return false; }
		}
		public PropertyDescriptor SortProperty {
			get { throw new NotSupportedException(); }
		}
		public ListSortDirection SortDirection {
			get { throw new NotSupportedException(); }
		}
		public event ListChangedEventHandler ListChanged;
		#endregion
		public string Name {
			get { return this.dataSource.Name; }
		}
		public IEnumerable<ResultTable> Tables {
			get { return this.tables.OrderBy(pair => pair.Key).Select(pair => pair.Value); }
		}
		public void Add(ResultTable item) {
			this.tables.GetOrAdd(item.TableName, item);
		}
		public ResultTable this[string name] {
			get { return this.tables[name]; }
		}
		public bool Contains(string name) {
			return this.tables.ContainsKey(name);
		}
		void RaiseChanged() {
			if(ListChanged != null)
				ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		public object Clone() {
			IEnumerable<ResultTable> resultTables = Tables.Select(resultTable => (ResultTable)resultTable.Clone()).ToArray();
			return new ResultSet(this.dataSource, resultTables);
		}
	}
}

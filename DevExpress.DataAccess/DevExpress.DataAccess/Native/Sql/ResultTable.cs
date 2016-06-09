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
using System.ComponentModel;
using System.Linq;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Native.Sql {
	public class ResultTable : ResultTableBase, IBindingList, ICloneable {
		#region enumerator
		class ResultTableEnumerator : IEnumerator<ResultRow> {
			int position = -1;
			readonly ResultTable owner;
			readonly int count;
			public ResultTableEnumerator(ResultTable owner, int count) {
				this.count = count;
				this.owner = owner;
			}
			public bool MoveNext() {
				return ++this.position < this.count;
			}
			public void Reset() {
				this.position = -1;
			}
			object IEnumerator.Current {
				get { return Current; }
			}
			public ResultRow Current {
				get {
					return new ResultRow(owner) { Index = this.position };
				}
			}
			public void Dispose() {
			}
		}
		#endregion
		string tableName;
		readonly List<ResultColumn> columns = new List<ResultColumn>();
		readonly List<ResultRelation> details = new List<ResultRelation>();
		public string DisplayName { get; set; }
		public ResultTable(string tableName) {
			this.tableName = tableName;
		}
		public ResultTable(string tableName, SelectedDataEx selectedData) : this(tableName) {
			Guard.ArgumentNotNull(selectedData, "selectedData");
			SetResultTableColumns(selectedData);
		}
		#region Overrides of ResultListBase
		public override string TableName { get { return this.tableName; } set { this.tableName = value; } }
		public override List<ResultColumn> Columns { get { return this.columns; } }
		public override List<ResultRelation> Details { get { return this.details; } }
		public override IEnumerator<ResultRow> GetEnumerator() { return new ResultTableEnumerator(this, Count); }
		public override bool Contains(ResultRow item) {
			throw new NotSupportedException();
		}
		public override void CopyTo(ResultRow[] array, int arrayIndex) {
			throw new NotSupportedException();
		}
		public override int Count {
			get { return this.Columns.Count > 0 ? this.Columns[0].Count : 0; }
		}
		public override int IndexOf(ResultRow item) {
			throw new NotSupportedException();
		}
		protected override ResultRow ElementAt(int index) {
			return new ResultRow(this) { Index = index };
		}
		#endregion
		public ResultRelation AddDetail(ResultTable detail, IEnumerable<RelationColumnInfo> columns) {
			ResultRelation relation = new ResultRelation(this, detail, columns);
			Details.Add(relation);
			return relation;
		}
		public ResultRelation AddDetail(ResultTable detail, IEnumerable<RelationColumnInfo> columns, string name) {
			ResultRelation relation = new ResultRelation(this, detail, columns, name);
			Details.Add(relation);
			return relation;
		}
		void SetResultTableColumns(SelectedDataEx selectedData) {
			for(int i = 0; i < selectedData.Schema.Length; i++) {
				string name = selectedData.Schema[i].Name;
				name = CleanColumnName(name, i);
				ResultColumn columnEx = new ResultColumn(name, selectedData.Schema[i].Type, selectedData.Lists[i]);
				Columns.Add(columnEx);
			}
		}
		internal static string CleanColumnName(string name, int i) {
			return string.IsNullOrEmpty(name) ? string.Format("Column{0}", i + 1) : name;
		}
		public void Copy(ResultTable source) {
			IEnumerable<ResultColumn> columnsToRemove = Columns.Select(t => t).ToArray();
			foreach(var column in columnsToRemove) {
				Columns.Remove(column);
			}
			foreach(ResultColumn sourceColumn in source.Columns) {
				this.Columns.Add(sourceColumn);
			}
			foreach(var propertyDescriptor in this.Columns) {
				OnListChanged(ListChangedType.PropertyDescriptorAdded, propertyDescriptor);
			}
			this.Details.Clear();
			this.Details.AddRange(source.Details);
			OnListChanged(ListChangedType.Reset, -1);
		}
		void OnListChanged(ListChangedType changedType, int newIndex) {
			if(ListChanged != null) {
				ListChanged(this, new ListChangedEventArgs(changedType, newIndex));
			}
		}
		void OnListChanged(ListChangedType changedType, PropertyDescriptor propertyDescriptor) {
			if(ListChanged != null) {
				ListChanged(this, new ListChangedEventArgs(changedType, propertyDescriptor));
			}
		}
		#region IBindingList Members
		public void AddIndex(PropertyDescriptor property) {
			throw new NotSupportedException();
		}
		public object AddNew() {
			return null;
		}
		public bool AllowEdit {
			get { return false; }
		}
		public bool AllowNew {
			get { return false; }
		}
		public bool AllowRemove {
			get { return false; }
		}
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotSupportedException();
		}
		public int Find(PropertyDescriptor property, object key) {
			throw new NotSupportedException();
		}
		public bool IsSorted {
			get { throw new NotSupportedException(); }
		}
		public event ListChangedEventHandler ListChanged;
		public void RemoveIndex(PropertyDescriptor property) {
			throw new NotSupportedException();
		}
		public void RemoveSort() {
			throw new NotSupportedException();
		}
		public ListSortDirection SortDirection {
			get { throw new NotSupportedException(); }
		}
		public PropertyDescriptor SortProperty {
			get { throw new NotSupportedException(); }
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
		#endregion
		public ResultColumn AddColumn(string name, Type fieldType) {
			var resultColumn = new ResultColumn(name, fieldType, null);
			Columns.Add(resultColumn);
			return resultColumn;
		}
		public object Clone() {
			var result = new ResultTable(TableName);
			foreach (var column in Columns) {
				result.Columns.Add((ResultColumn)column.Clone());
			}
			foreach (var detail in Details) {
				result.Details.Add((ResultRelation)detail.Clone());
			}
			return result;
		}
	}
}

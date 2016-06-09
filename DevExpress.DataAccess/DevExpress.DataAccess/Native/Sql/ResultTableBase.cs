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
using DevExpress.DataAccess.Sql.DataApi;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design;
namespace DevExpress.DataAccess.Native.Sql {
	public abstract class ResultTableBase : ITable, ITypedList, IList<ResultRow>, IList {
		static PropertyDescriptorCollection emptyPropertyDescriptorCollection;
		static PropertyDescriptorCollection EmptyPropertyDescriptorCollection {
			get {
				return emptyPropertyDescriptorCollection ??
					   (emptyPropertyDescriptorCollection =
						   new PropertyDescriptorCollection(new PropertyDescriptor[0], true));
			}
		}
		public abstract string TableName { get; set; }
		public abstract List<ResultColumn> Columns { get; }
		public abstract List<ResultRelation> Details { get; }
		#region Implementation of ITable
		string ITable.Name { get { return TableName; } }
		IEnumerable<IColumn> ITable.Columns { get { return Columns; } }
		IColumn ITable.GetColumn(int index) { return Columns[index]; }
		IColumn ITable.GetColumn(string name) {
			IColumn column;
			if(!((ITable)this).TryGetColumn(name, out column))
				throw new ArgumentOutOfRangeException(string.Format("Column '{0}' not found.", name));
			return column;
		}
		bool ITable.TryGetColumn(string name, out IColumn column) {
			column = Columns.FirstOrDefault(resultColumn => resultColumn.Name == name);
			return column != null;
		}
		IRow ITable.this[int rowIndex] { get { return this[rowIndex]; } }
		IEnumerable<IRelation> ITable.DetailRelations { get { return Details; } }
		IRelation ITable.GetDetailRelation(string name) {
			IRelation result;
			if(!((ITable)this).TryGetDetailRelation(name, out result))
				throw new ArgumentOutOfRangeException();
			return result;
		}
		bool ITable.TryGetDetailRelation(string name, out IRelation relation) {
			relation = Details.FirstOrDefault(r => r.Name == name);
			return relation != null;
		}
		IEnumerator<IRow> IEnumerable<IRow>.GetEnumerator() { return GetEnumerator(); }
		#endregion
		#region Implementation of ITypedList
		public string GetListName(PropertyDescriptor[] listAccessors) {
			ResultTableBase list = GetList(listAccessors);
			return list == null ? null : list.TableName;
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			ResultTableBase list = GetList(listAccessors);
			return list == null
				? EmptyPropertyDescriptorCollection
				: new PropertyDescriptorCollection(
					list.Details.Select(d => (PropertyDescriptor)d).Union(list.Columns).ToArray());
		}
		#endregion
		#region Implementation of IEnumerable
		public abstract IEnumerator<ResultRow> GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		#endregion
		#region Implementation of ICollection<ResultRow>
		void ICollection<ResultRow>.Add(ResultRow item) { throw new NotSupportedException(); }
		void ICollection<ResultRow>.Clear() { throw new NotSupportedException(); }
		public abstract bool Contains(ResultRow item);
		public abstract void CopyTo(ResultRow[] array, int arrayIndex);
		bool ICollection<ResultRow>.Remove(ResultRow item) { throw new NotSupportedException(); }
		public abstract int Count { get; }
		bool ICollection<ResultRow>.IsReadOnly { get { return true; } }
		#endregion
		#region Implementation of IList<ResultRow>
		public abstract int IndexOf(ResultRow item);
		void IList<ResultRow>.Insert(int index, ResultRow item) { throw new NotSupportedException(); }
		void IList<ResultRow>.RemoveAt(int index) { throw new NotSupportedException(); }
		public ResultRow this[int index] { get { return ElementAt(index); } set { throw new NotSupportedException(); } }
		protected abstract ResultRow ElementAt(int index);
		#endregion
		#region IList Members
		int IList.Add(object value) { throw new NotSupportedException(); }
		void IList.Clear() { throw new NotSupportedException(); }
		bool IList.Contains(object value) { return value is ResultRow && Contains((ResultRow)value); }
		int IList.IndexOf(object value) { return value is ResultRow ? IndexOf((ResultRow)value) : -1; }
		void IList.Insert(int index, object value) { throw new NotSupportedException(); }
		bool IList.IsFixedSize { get { return true; } }
		bool IList.IsReadOnly { get { return true; } }
		void IList.Remove(object value) { throw new NotSupportedException(); }
		void IList.RemoveAt(int index) { throw new NotSupportedException(); }
		object IList.this[int index] {
			get { return ElementAt(index); }
			set { throw new NotSupportedException(); }
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) { CopyTo((ResultRow[])array, index); }
		int ICollection.Count { get { return Count; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return this; } }
		#endregion
		ResultTable GetDetailList(string name) {
			ResultRelation rel = Details.FirstOrDefault(r => r.Name == name);
			if(rel == null)
				return null;
			return rel.Detail;
		}
		ResultTableBase GetList(IEnumerable<PropertyDescriptor> listAccessors) {
			ResultTableBase result = this;
			if(listAccessors != null)
				foreach(PropertyDescriptor pd in listAccessors) {
					result = result.GetDetailList(pd.Name);
					if(result == null)
						return null;
				}
			return result;
		}
	}
}

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
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Localization;
using DevExpress.Utils;
#if !DXPORTABLE
using System.Drawing.Design;
#endif
namespace DevExpress.DataAccess.Sql {
	[TypeConverter("DevExpress.DataAccess.UI.Native.Sql.SqlQueryCollectionTypeConverter," + AssemblyInfo.SRAssemblyDataAccessUI)]
#if !DXPORTABLE
	[Editor("DevExpress.DataAccess.UI.Native.Sql.SqlQueryCollectionEditor, " + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
#endif
	public class SqlQueryCollection : IList<SqlQuery>, IList {
		readonly List<SqlQuery> innerList;
		readonly SqlDataSource realOwner;
		internal SqlDataSource DataSource { get { return realOwner; } }
		internal SqlQueryCollection(SqlDataSource owner) {
			this.realOwner = owner;
			this.innerList = new List<SqlQuery>();
		}
		internal SqlQueryCollection(SqlDataSource owner, int capacity) {
			this.realOwner = owner;
			innerList = new List<SqlQuery>(capacity);
		}
		public void AddRange(IEnumerable<SqlQuery> items) {
			foreach(SqlQuery item in items) {
				item.Owner = this;
				this.innerList.Add(item);
			}
		}
		public void AddRange(SqlQuery[] items) { AddRange(items.AsEnumerable()); }
		public bool ContainsName(string name) { return innerList.Any(q => q.Name == name); }
		#region IList<SqlQuery> Members
		public int IndexOf(SqlQuery item) { return this.innerList.IndexOf(item); }
		public void Insert(int index, SqlQuery item) {
			item.Owner = this;
			this.innerList.Insert(index, item);
		}
		public void RemoveAt(int index) { this.innerList.RemoveAt(index); }
		public SqlQuery this[int index] {
			get { return this.innerList[index]; }
			set {
				Guard.ArgumentNotNull(value, "query");
				this.innerList[index] = value;
				value.Owner = this;
			}
		}
		public SqlQuery this[string name] {
			get {
				int index = innerList.FindIndex(q => q.Name == name);
				if(index < 0)
					throw new ArgumentOutOfRangeException(string.Format("There is no queries with name \"{0}\"", name));
				return this.innerList[index];
			}
			set {
				Guard.ArgumentNotNull(value, "query");
				int index = innerList.FindIndex(q => q.Name == name);
				if(index < 0)
					throw new ArgumentOutOfRangeException(string.Format("There is no queries with name \"{0}\"", name));
				this.innerList[index] = value;
				value.Owner = this;
			}
		}
		#endregion
		#region ICollection<SqlQuery> Members
		public void Add(SqlQuery item) {
			item.Owner = this;
			this.innerList.Add(item);
		}
		public void Clear() { this.innerList.Clear(); }
		public bool Contains(SqlQuery item) { return this.innerList.Contains(item); }
		public void CopyTo(SqlQuery[] array, int arrayIndex) { this.innerList.CopyTo(array, arrayIndex); }
		public int Count { get { return this.innerList.Count; } }
		bool ICollection<SqlQuery>.IsReadOnly { get { return false; } }
		public bool Remove(SqlQuery item) { return this.innerList.Remove(item); }
		#endregion
		#region IEnumerable<SqlQuery> Members
		public IEnumerator<SqlQuery> GetEnumerator() {
			return this.innerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
		#region IList Members
		int IList.Add(object value) {
			SqlQuery query = value as SqlQuery;
			if(query == null && value != null)
				throw new ArgumentException();
			Add(query);
			return Count - 1;
		}
		void IList.Clear() { Clear(); }
		bool IList.Contains(object value) {
			SqlQuery query = value as SqlQuery;
			if(query == null && value != null)
				return false;
			return Contains(query);
		}
		int IList.IndexOf(object value) {
			SqlQuery query = value as SqlQuery;
			if(query == null && value != null)
				return -1;
			return IndexOf(query);
		}
		void IList.Insert(int index, object value) {
			SqlQuery query = value as SqlQuery;
			if(query == null && value != null)
				throw new ArgumentException();
			Insert(index, query);
		}
		bool IList.IsFixedSize { get { return false; } }
		bool IList.IsReadOnly { get { return false; } }
		void IList.Remove(object value) {
			SqlQuery query = value as SqlQuery;
			if(query == null && value != null)
				return;
			Remove(query);
		}
		void IList.RemoveAt(int index) { RemoveAt(index); }
		object IList.this[int index] {
			get {
				return this[index];
			}
			set {
				SqlQuery query = value as SqlQuery;
				if(query == null && value != null)
					throw new ArgumentException();
				this[index] = query;
			}
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) { throw new NotSupportedException(); }
		int ICollection.Count { get { return Count; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		#endregion
		public void CheckQueryName(SqlQuery query) {
			Guard.ArgumentNotNull(query, "query");
			string name = query.Name;
			if(string.IsNullOrEmpty(name))
				throw new InvalidNameException(DataAccessLocalizer.GetString(DataAccessStringId.MessageInvalidItemName));
			foreach(SqlQuery item in this) {
				if(ReferenceEquals(item, query))
					continue;
				if(item.Name == name)
					throw new InvalidNameException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.MessageDuplicateItemName), name));
			}
		}
		public string GenerateUniqueName(SqlQuery query) {
			string name = query.Name;
			if(string.IsNullOrEmpty(name)) {
				TableQuery tableQuery = query as TableQuery;
				if(tableQuery != null && tableQuery.Tables.Any()) {
					name = tableQuery.Tables[0].Alias ?? tableQuery.Tables[0].Name;
				}
				else {
					name = "Query";
				}
			}
			name = name.Replace('.', '_');
			if(!ContainsName(name))
				return name;
			for(int i = 1; ; i++) {
				string anotherName = string.Format("{0}_{1}", name, i);
				if(!DataSource.Queries.ContainsName(anotherName))
					return anotherName;
			}
		}
	}
}

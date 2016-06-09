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
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess.Sql {
	public sealed class TableInfo {
		public class EqualityComparer : IEqualityComparer<TableInfo> {
			public static bool Equals(TableInfo x, TableInfo y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				if(!string.Equals(x.name, y.name, StringComparison.Ordinal) || !string.Equals(x.alias, y.alias, StringComparison.Ordinal))
					return false;
				int colCount = x.columns.Count;
				if(y.columns.Count != colCount)
					return false;
				for(int i = 0; i < colCount; i++)
					if(!ColumnInfo.EqualityComparer.Equals(x.columns[i], y.columns[i]))
						return false;
				return true;
			}
			#region IEqualityComparer<TableInfo> Members 
			bool IEqualityComparer<TableInfo>.Equals(TableInfo x, TableInfo y) { return Equals(x, y); }
			int IEqualityComparer<TableInfo>.GetHashCode(TableInfo obj) { return 0; }
			#endregion
		}
		public class ColumnInfoList : List<ColumnInfo> {
			public ColumnInfoList() { }
			public ColumnInfoList(int capacity) : base(capacity) { }
			public ColumnInfoList(IEnumerable<ColumnInfo> collection) : base(collection) { }
			public void AddRange(ColumnInfo[] collection) { base.AddRange(collection); }
		}
		string name;
		string alias;
		readonly ColumnInfoList columns;
		public TableInfo(string name, string alias) {
			this.name = name;
			this.alias = alias;
			this.columns = new ColumnInfoList();
		}
		public TableInfo(string name) : this(name, null) { }
		public TableInfo() : this(null, null) { }
		internal TableInfo(TableInfo other) : this(other.name, other.alias) {
			foreach(ColumnInfo column in other.columns)
				columns.Add(new ColumnInfo(column));
		}
		public string Name { get { return name; } set { name = value; } }
		[DefaultValue(null)]
		public string Alias { get { return alias; } set { alias = value; } }
		public bool HasAlias { get { return alias != null; } }
		public string ActualName { get { return alias ?? name; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<ColumnInfo> SelectedColumns { get { return columns; } }
		public ColumnInfo SelectColumn(string name) {
			ColumnInfo result = new ColumnInfo(name);
			this.columns.Add(result);
			return result;
		}
		public ColumnInfo SelectColumn(string name, AggregationType aggregation, string alias) {
			ColumnInfo result = new ColumnInfo(name, aggregation, alias);
			columns.Add(result);
			return result;
		}
		public void SelectColumns(params string[] columns) { 
			int argc = columns.Length;
			ColumnInfo[] infos = new ColumnInfo[argc];
			for(int i = 0; i < argc; i++)
				infos[i] = new ColumnInfo(columns[i]);
			this.columns.AddRange(infos);
		}
	}
}

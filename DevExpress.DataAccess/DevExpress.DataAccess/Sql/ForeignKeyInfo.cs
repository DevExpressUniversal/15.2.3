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
using DevExpress.Xpo.DB;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess.Sql {
	public sealed class RelationInfo  {
		public class EqualityComparer : IEqualityComparer<RelationInfo> {
			public static bool Equals(RelationInfo x, RelationInfo y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				int colCount = x.keyColumns.Count;
				if(y.keyColumns.Count != colCount)
					return false;
				if(x.joinType != y.joinType || !string.Equals(x.parentTable, y.parentTable, StringComparison.Ordinal) || !string.Equals(x.nestedTable, y.nestedTable, StringComparison.Ordinal))
					return false;
				for(int i = 0; i < colCount; i++)
					if(!RelationColumnInfo.EqualityComparer.Equals(x.keyColumns[i], y.keyColumns[i]))
						return false;
				return true;
			}
			#region IEqualityComparer<RelationInfo> Members
			bool IEqualityComparer<RelationInfo>.Equals(RelationInfo x, RelationInfo y) { return Equals(x, y); }
			int IEqualityComparer<RelationInfo>.GetHashCode(RelationInfo obj) { return 0; }
			#endregion
		}
		public class RelationColumnInfoList : List<RelationColumnInfo> {
			public RelationColumnInfoList() { }
			public RelationColumnInfoList(int capacity) : base(capacity) { }
			public RelationColumnInfoList(IEnumerable<RelationColumnInfo> collection) : base(collection) { }
			public void AddRange(RelationColumnInfo[] collection) {
				base.AddRange(collection);
			}
		}
		const JoinType defaultJoinType = JoinType.Inner;
		string parentTable;
		string nestedTable;
		JoinType joinType;
		readonly RelationColumnInfoList keyColumns;
		public RelationInfo(string parentTable, string nestedTable, JoinType joinType) {
			this.parentTable = parentTable;
			this.nestedTable = nestedTable;
			this.joinType = joinType;
			this.keyColumns = new RelationColumnInfoList();
		}
		public RelationInfo(string parentTable, string nestedTable) : this(parentTable, nestedTable, defaultJoinType) { }
		public RelationInfo() : this(null, null) { }
		public RelationInfo(string parentTable, string nestedTable, JoinType joinType, IEnumerable<RelationColumnInfo> keyColumns)
			: this(parentTable, nestedTable, joinType) {
				this.keyColumns.AddRange(keyColumns);
		}
		public RelationInfo(string parentTable, string nestedTable, IEnumerable<RelationColumnInfo> keyColumns) : this(parentTable, nestedTable, defaultJoinType, keyColumns) { }
		public RelationInfo(string parentTable, string nestedTable, JoinType joinType, RelationColumnInfo keyColumn) 
			: this(parentTable, nestedTable, joinType) {
				this.KeyColumns.Add(keyColumn);
		}
		public RelationInfo(string parentTable, string nestedTable, RelationColumnInfo keyColumn) : this(parentTable, nestedTable, defaultJoinType, keyColumn) { }
		public RelationInfo(string parentTable, string nestedTable, JoinType joinType, string parentKeyColumn, string nestedKeyColumn) : this(parentTable, nestedTable, joinType, new RelationColumnInfo(parentKeyColumn, nestedKeyColumn)) { }
		public RelationInfo(string parentTable, string nestedTable, string parentKeyColumn, string nestedKeyColumn) : this(parentTable, nestedTable, defaultJoinType, parentKeyColumn, nestedKeyColumn) { }
		internal RelationInfo(RelationInfo other) : this(other.parentTable, other.nestedTable, other.joinType) {
			foreach(RelationColumnInfo column in other.keyColumns)
				this.keyColumns.Add(new RelationColumnInfo(column));
		}
		public string ParentTable { get { return parentTable; } set { parentTable = value; } }
		public string NestedTable { get { return nestedTable; } set { nestedTable = value; } }
		[DefaultValue(defaultJoinType)]
		public JoinType JoinType { get { return joinType; } set { joinType = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RelationColumnInfoList KeyColumns { get { return keyColumns; } }
		public override string ToString() {
			return string.Format("{0}: {1} <- {2}", joinType, parentTable ?? "<null>", nestedTable ?? "<null>");
		}
	}
}

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
using System.Linq;
using System.Text;
using DevExpress.DataAccess.Localization;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	public class ConditionStringInfo {
		public struct ColumnInfo {
			public readonly string Table;
			public readonly string Column;
			public ColumnInfo(string table, string column) {
				Table = table;
				Column = column;
			}
			#region Overrides of ValueType
			public override bool Equals(object obj) {
				if(!(obj is ColumnInfo))
					return false;
				ColumnInfo other = (ColumnInfo)obj;
				return Column == other.Column && Table == other.Table;
			}
			public override int GetHashCode() { return Column == null ? 0 : Column.GetHashCode(); }
			public override string ToString() { return string.Format("[{0}].[{1}]", Table, Column); }
			#endregion
		}
		readonly DataAccessStringId format;
		readonly string joinType;
		readonly ColumnInfo[] columns;
		public ConditionStringInfo(DataAccessStringId format, params ColumnInfo[] columns)
			: this(format, null, columns) { }
		public ConditionStringInfo(DataAccessStringId format, string joinType, params ColumnInfo[] columns)
			: this(format, joinType, columns.AsEnumerable()) { }
		public ConditionStringInfo(DataAccessStringId format, IEnumerable<ColumnInfo> columns) : this(format, null, columns) { }
		public ConditionStringInfo(DataAccessStringId format, string joinType, IEnumerable<ColumnInfo> columns) {
			this.format = format;
			this.joinType = joinType;
			this.columns = columns.ToArray();
		}
		public DataAccessStringId Format { get { return format; } }
		public string JoinType { get { return joinType; } }
		public IEnumerable<ColumnInfo> Columns { get { return columns; } }
		#region Overrides of Object
		public override bool Equals(object obj) {
			if(ReferenceEquals(this, obj))
				return true;
			if(obj == null)
				return false;
			if(obj.GetType() != GetType())
				return false;
			ConditionStringInfo other = (ConditionStringInfo)obj;
			if(format != other.format)
				return false;
			if(joinType != other.joinType)
				return false;
			int n = columns.Length;
			if(other.columns.Length != n)
				return false;
			for(int i = 0; i < n; i++)
				if(!columns[i].Equals(other.columns[i]))
					return false;
			return true;
		}
		public override int GetHashCode() { return columns.Length == 0 ? 0 : columns[0].GetHashCode(); }
		public override string ToString() {
			string formatString = DataAccessLocalizer.GetString(format);
			string columnsString =
				string.Join(
					DataAccessLocalizer.GetString(DataAccessStringId.QueryDesignerJoinExpressionElementSeparator),
					columns);
			return joinType != null
				? string.Format(formatString, joinType, columnsString)
				: string.Format(formatString, columnsString);
		}
		#endregion
	}
	public class ConditionStringInfoComparer : IEqualityComparer<ConditionStringInfo> {
		#region Implementation of IEqualityComparer<in ConditionStringInfo>
		public bool Equals(ConditionStringInfo x, ConditionStringInfo y) {
			if(x == null)
				return y == null;
			return x.Equals(y);
		}
		public int GetHashCode(ConditionStringInfo obj) { return obj == null ? 0 : obj.GetHashCode(); }
		#endregion
	}
}

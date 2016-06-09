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
using System.Linq;
using System.Text;
namespace DevExpress.DataAccess.Sql {
	public sealed class GroupingInfo {
		string table;
		string column;
		public class EqualityComparer : IEqualityComparer<GroupingInfo> {
			public static bool Equals(GroupingInfo x, GroupingInfo y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				return string.Equals(x.column, y.column, StringComparison.Ordinal) &&
					   string.Equals(x.table, y.table, StringComparison.Ordinal);
			}
			public static bool Equals(GroupingInfo x, string yTable, string yColumn) {
				if(x == null)
					return false;
				return string.Equals(x.column, yColumn, StringComparison.Ordinal) &&
					   string.Equals(x.Table, yTable, StringComparison.Ordinal);
			}
			#region Implementation of IEqualityComparer<in GroupingInfo>
			bool IEqualityComparer<GroupingInfo>.Equals(GroupingInfo x, GroupingInfo y) { return Equals(x, y); }
			public int GetHashCode(GroupingInfo obj) { return 0; }
			#endregion
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public GroupingInfo() { }
		public GroupingInfo(string table, string column) {
			this.table = table;
			this.column = column;
		}
		public GroupingInfo(GroupingInfo other) : this(other.table, other.column) { }
		public string Table { get { return table; } set { table = value; } }
		public string Column { get { return column; } set { column = value; } }
	}
}

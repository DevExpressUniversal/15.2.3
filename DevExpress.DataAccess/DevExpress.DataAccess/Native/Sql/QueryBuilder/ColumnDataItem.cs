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

using System.Collections.Generic;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	public struct ColumnDataItem {
		readonly string table;
		readonly string column;
		public ColumnDataItem(string table, string column) {
			this.table = table;
			this.column = column;
		}
		public string Table { get { return this.table; } }
		public string Column { get { return this.column; } }
		public override bool Equals(object obj) {
			if(!(obj is ColumnDataItem))
				return false;
			ColumnDataItem other = (ColumnDataItem)obj;
			return (this.table == other.Table && Column == other.Column);
		}
		public override int GetHashCode() {
			return this.column != null ? this.column.GetHashCode() : 0;
		}
		public override string ToString() {
			return this.column;
		}
	}
	public class ColumnDataItemComparer : IEqualityComparer<ColumnDataItem> {
		public bool Equals(ColumnDataItem x, ColumnDataItem y) {
			return x.Equals(y);
		}
		public int GetHashCode(ColumnDataItem obj) {
			return obj.GetHashCode();
		}
	}
}

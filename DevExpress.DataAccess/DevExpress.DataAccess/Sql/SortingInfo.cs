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
using System.Text;
namespace DevExpress.DataAccess.Sql {
	public sealed class SortingInfo {
		public enum SortingDirection {
			Ascending = Xpo.DB.SortingDirection.Ascending,
			Descending = Xpo.DB.SortingDirection.Descending
		}
		public class EqualityComparer : IEqualityComparer<SortingInfo> {
			public static bool Equals(SortingInfo x, SortingInfo y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				return x.Direction == y.Direction && 
					   string.Equals(x.Column, y.Column, StringComparison.Ordinal) &&
					   string.Equals(x.Table, y.Table, StringComparison.Ordinal);
			}
			#region Implementation of IEqualityComparer<in SortingInfo>
			bool IEqualityComparer<SortingInfo>.Equals(SortingInfo x, SortingInfo y) { return Equals(x, y); }
			int IEqualityComparer<SortingInfo>.GetHashCode(SortingInfo obj) { return 0; }
			#endregion
		}
		string table;
		string column;
		SortingDirection direction;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SortingInfo() { }
		public SortingInfo(string table, string column) : this(table, column, SortingDirection.Ascending) { }
		public SortingInfo(string table, string column, SortingDirection direction) {
			this.column = column;
			this.Direction = direction;
			this.Table = table;
		}
		public SortingInfo(SortingInfo other) : this(other.Table, other.Column, other.Direction) { }
		public string Table { get { return table; } set { table = value; } }
		public string Column { get { return column; } set { column = value; } }
		[DefaultValue(SortingDirection.Ascending)]
		public SortingDirection Direction { get { return direction; } set { direction = value; } }
	}
}

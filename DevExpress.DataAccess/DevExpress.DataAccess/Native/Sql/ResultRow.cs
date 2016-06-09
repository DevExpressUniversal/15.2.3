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

using System.Collections;
using System.Collections.Generic;
using DevExpress.DataAccess.Sql.DataApi;
namespace DevExpress.DataAccess.Native.Sql {
	public class ResultRow : IRow {
		class Enumerator : IEnumerator<object> {
			readonly IEnumerator<IColumn> columnsEnumerator;
			readonly int rowIndex;
			public Enumerator(ResultRow owner) {
				columnsEnumerator = owner.owner.Columns.GetEnumerator();
				rowIndex = owner.Index;
			}
			#region Implementation of IEnumerator
			public bool MoveNext() { return columnsEnumerator.MoveNext(); }
			public void Reset() { columnsEnumerator.Reset(); }
			public object Current { get { return columnsEnumerator.Current.GetValue<object>(rowIndex); } }
			#endregion
			#region Implementation of IDisposable
			public void Dispose() { columnsEnumerator.Dispose(); }
			#endregion
		}
		readonly ITable owner;
		public ResultRow(ITable owner) { this.owner = owner; }
		public int Index { get; set; }
		#region Implementation of IRow
		T IRow.GetValue<T>(IColumn column) { return column.GetValue<T>(Index); }
		T IRow.GetValue<T>(int columnIndex) { return owner.GetColumn(columnIndex).GetValue<T>(Index); }
		T IRow.GetValue<T>(string columnName) { return owner.GetColumn(columnName).GetValue<T>(Index); }
		object IRow.this[int columnIndex] { get { return ((IRow)this).GetValue<object>(columnIndex); } }
		object IRow.this[string columnName] { get { return ((IRow)this).GetValue<object>(columnName); } }
		IEnumerable<IRow> IRow.GetDetailRows(IRelation relation) { return relation.GetDetailRows(this); }
		IEnumerable<IRow> IRow.GetDetailRows(string relationName) {
			return owner.GetDetailRelation(relationName).GetDetailRows(this);
		}
		#endregion
		#region Implementation of IEnumerable
		public IEnumerator<object> GetEnumerator() { return new Enumerator(this); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		#endregion
	}
}

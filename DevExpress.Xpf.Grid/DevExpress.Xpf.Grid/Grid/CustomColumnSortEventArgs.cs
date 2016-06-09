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
using DevExpress.Data;
#if DATAGRID
using GridColumn = DevExpress.AgDataGrid.AgDataGridColumn;
#endif
#if DATAGRID
namespace DevExpress.AgDataGrid {
#else
namespace DevExpress.Xpf.Grid {
#endif
	public class CustomColumnSortEventArgs : EventArgs {
		ColumnSortOrder sortOrder;
		bool handled = false;
		object value1, value2;
		int result = 0;
		int listSourceRowIndex1, listSourceRowIndex2;
		GridColumn column;
		public CustomColumnSortEventArgs(GridColumn column, int listSourceRowIndex1, int listSourceRowIndex2, object value1, object value2, ColumnSortOrder sortOrder) {
			this.sortOrder = sortOrder;
			this.listSourceRowIndex1 = listSourceRowIndex1;
			this.listSourceRowIndex2 = listSourceRowIndex2;
			this.value1 = value1;
			this.value2 = value2;
			this.column = column;
		}
		public ColumnSortOrder SortOrder { get { return sortOrder; } }
		public GridColumn Column { get { return column; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public int Result {
			get { return result; }
			set { result = value; }
		}
		public GridControl Source { get { return (GridControl)Column.OwnerControl; } }
		public object Value1 { get { return value1; } }
		public object Value2 { get { return value2; } }
		public int ListSourceRowIndex1 { get { return listSourceRowIndex1; } }
		public int ListSourceRowIndex2 { get { return listSourceRowIndex2; } }
		internal void SetArgs(int listSourceRowIndex1, int listSourceRowIndex2, object value1, object value2, ColumnSortOrder sortOrder) {
			this.sortOrder = sortOrder;
			this.value1 = value1;
			this.value2 = value2;
			this.result = 0;
			this.handled = false;
			this.listSourceRowIndex1 = listSourceRowIndex1;
			this.listSourceRowIndex2 = listSourceRowIndex2;
		}
		protected internal int? GetSortResult() {
			if(!Handled)
				return null;
			return Result;
		}
	}
	public delegate void CustomColumnSortEventHandler(object sender, CustomColumnSortEventArgs e);
}

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

using DevExpress.XtraPivotGrid.Data;
using System.Collections.Generic;
namespace DevExpress.XtraPivotGrid.Selection {
		public class PivotWholeColumnSelection {
		SelectionVisualItems visualItems;
		Dictionary<int, int> columns, rows;
		bool isCalculated;
		public PivotWholeColumnSelection(SelectionVisualItems visualItems) {
			this.visualItems = visualItems;
			this.columns = new Dictionary<int, int>();
			this.rows = new Dictionary<int, int>();
		}
		protected SelectionVisualItems VisualItems { get { return visualItems; } }
		protected PivotGridSelection Selection { get { return VisualItems.InnerSelection; } }
		protected int ColumnCount { get { return VisualItems.ColumnCount; } }
		protected int RowCount { get { return VisualItems.RowCount; } }
		protected Dictionary<int, int> Columns { get { return columns; } }
		protected Dictionary<int, int> Rows { get { return rows; } }
		protected void Calculate() {
			if(isCalculated) return;
			rows = Selection.Cells.GetRows();
			columns = Selection.Cells.GetColumns();
			isCalculated = true;
		}
		public void Reset() {
			isCalculated = false;
			Columns.Clear();
			Rows.Clear();
		}
		public bool IsWholeColumnSelected(int columnIndex) {
			Calculate();
			int count;
			if(!Columns.TryGetValue(columnIndex, out count))
				return false;
			return count == RowCount;
		}
		public bool IsWholeRowSelected(int rowIndex) {
			Calculate();
			int count;
			if(!Rows.TryGetValue(rowIndex, out count))
				return false;
			return count == ColumnCount;
		}
	}
}

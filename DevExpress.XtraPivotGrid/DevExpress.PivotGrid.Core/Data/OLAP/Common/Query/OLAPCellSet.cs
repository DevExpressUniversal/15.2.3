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
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using System.Linq;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPCachedCellSet {
		readonly IOLAPCellSet cellSet;
		ITupleCollection columnAxis, rowAxis;
		IEnumerator<IOLAPCell> cells;
		public ITupleCollection RowAxis { get { return rowAxis; } }
		public ITupleCollection ColumnAxis { get { return columnAxis; } }
		public OLAPCachedCellSet(AxisColumnsProviderBase axisColumnsProvider, IOLAPCellSet cellSet) {
			this.cellSet = cellSet;
			columnAxis = cellSet.GetColumnAxis(axisColumnsProvider);
			rowAxis = cellSet.GetRowAxis(axisColumnsProvider);
		}
		public int ColumnCount { get { return columnAxis.ReadedCount; } }
		public int RowCount { get { return rowAxis.ReadedCount; ; } }
		int currRowIndex = 0;
		public void MoveToRow(int index) {
			if(cells == null) {
				cells = cellSet.Cells.GetEnumerator();
				cells.MoveNext();
			}
			if(index == currRowIndex && currRowIndex == 0)
				return;
			if(index <= currRowIndex)
				throw new Exception("index");
			while(currColumn < ColumnCount) {
				currColumn++;
				if(!cells.MoveNext())
					return;
			}
			currColumn = 0;
			currRowIndex++;
			int skip = ColumnCount * (index - currRowIndex);
			for(int i = 0; i < skip; i++)
				if(!cells.MoveNext())
					return;
			currRowIndex = index;
		}
		int currColumn = 0;
		public IOLAPCell GetColumnValue(int index) {
			while(currColumn != index) {
				currColumn++;
				if(!cells.MoveNext())
					return null;
			}
			return cells.Current;
		}
		public object GetColumnValueValueSafe(int index) {
			if(cells == null) {
				cells = cellSet.Cells.GetEnumerator();
				cells.MoveNext();
			}
			while(currColumn != index) {
				currColumn++;
				if(!cells.MoveNext())
					return null;
			}
			try {
				return cells.Current.Value;
			} catch {
				return PivotCellValue.ErrorValue.Value;
			}
		}
	}
}

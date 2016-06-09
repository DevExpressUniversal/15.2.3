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
using System.Windows.Threading;
using DevExpress.XtraPivotGrid;
using System.Drawing;
namespace DevExpress.Xpf.PivotGrid.Internal {
	class PivotWpfChartDataSource : PivotChartDataSource {
		public PivotWpfChartDataSource(PivotGridWpfData data)
			: base(data) { }
		protected override void InvalidateChartDataSourceInMainThread() {
			((PivotGridWpfData)Data).PivotGrid.Dispatcher.BeginInvoke(new Action(delegate() {
				InvalidateChartDataSourceCells();
			}));
		}
		protected override PivotChartDataSourceRowBase CreateDataSourceRow(Point cell) {
			return new PivotChartDataSourceRow(this, cell);
		}
	}
}
namespace DevExpress.Xpf.PivotGrid {
	public class PivotChartDataSourceRow : PivotChartDataSourceRowBase {
		PivotFieldValueEventArgs columnValueInfo;
		PivotFieldValueEventArgs rowValueInfo;
		PivotCellBaseEventArgs cellInfo;
		internal PivotChartDataSourceRow(PivotChartDataSourceBase ds, Point cell)
			: base(ds, cell) { }
		internal PivotChartDataSourceRow(PivotChartDataSourceBase ds)
			: base(ds) { }
		public PivotFieldValueEventArgs ColumnValueInfo {
			get {
				if(ColumnItem != null)
					columnValueInfo = new PivotFieldValueEventArgs(null, ColumnItem);
				return columnValueInfo;
			}
		}
		public PivotFieldValueEventArgs RowValueInfo {
			get {
				if(RowItem != null)
					rowValueInfo = new PivotFieldValueEventArgs(null, RowItem);
				return rowValueInfo;
			}
		}
		public PivotCellBaseEventArgs CellInfo {
			get {
				if(CellItem != null)
					cellInfo = new PivotCellBaseEventArgs(null, CellItem);
				return cellInfo;
			}
		}
	}
}

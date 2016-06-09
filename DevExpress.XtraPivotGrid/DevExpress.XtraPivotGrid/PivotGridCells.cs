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

using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Selection;
using DevExpress.XtraPivotGrid.ViewInfo;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridCells {
		PivotGridViewInfoData data;
		public PivotGridCells(PivotGridViewInfoData data) {
			this.data = data;
		}
		protected PivotGridViewInfoData Data { get { return data; } }
		protected PivotVisualItems VisualItems { get { return data.VisualItems; } }
		protected PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)Data.ViewInfo; } }
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridCellsFocusedCell")]
#endif
		public Point FocusedCell { 
			get { return VisualItems.FocusedCell; } 
			set { VisualItems.FocusedCell = value; } 
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridCellsLeftTopCell")]
#endif
		public Point LeftTopCell {
			get { return ViewInfo.LeftTopCoord; }
			set { ViewInfo.LeftTopCoord = value; }
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridCellsSelection")]
#endif
		public Rectangle Selection { 
			get { return VisualItems.Selection; } 
			set { VisualItems.Selection = value; } 
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridCellsMultiSelection")]
#endif
		public IMultipleSelection MultiSelection { get { return VisualItems; } }
		public void CopySelectionToClipboard() { VisualItems.CopySelectionToClipboard(); }
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridCellsColumnCount")]
#endif
		public int ColumnCount { get { return VisualItems.ColumnCount; } }
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridCellsRowCount")]
#endif
		public int RowCount { get { return VisualItems.RowCount; } }
		public PivotCellEventArgs GetCellInfo(int columnIndex, int rowIndex) {
			if(ViewInfo.CellsArea == null) return null;
			PivotCellViewInfo cellViewInfo = (PivotCellViewInfo)ViewInfo.CellsArea.CreateCellViewInfo(columnIndex, rowIndex);
			return cellViewInfo != null ? new PivotCellEventArgs(cellViewInfo, ViewInfo) : null;
		}
		public PivotCellEventArgs GetFocusedCellInfo() {
			return GetCellInfo(FocusedCell.X, FocusedCell.Y);
		}
		public void InvalidateCell(int x, int y) {
			InvalidateCell(GetCellInfo(x, y));
		}
		public void InvalidateCell(PivotCellEventArgs cellInfo) {
			if(cellInfo == null || Data == null) return;
			Data.InvalidateCell(new Point(cellInfo.ColumnIndex, cellInfo.RowIndex));
		}
		public void SetSelectionByFieldValues(bool isColumn, object[] values) {
			VisualItems.SetSelectionByFieldValues(isColumn, values);
		}
		public void SetSelectionByFieldValues(bool isColumn, object[] values, PivotGridFieldBase dataField) {
			VisualItems.SetSelectionByFieldValues(isColumn, values, Data.GetFieldItem(dataField));
		}
	}
}

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
using System.Collections;
using DevExpress.Xpf.Core.Native;
#if SL
using DevExpress.Utils;
#endif
namespace DevExpress.Xpf.Grid.Native {
	public class ClipboardController : RowsClipboardController {
		public class CellsClipboardDataProvider : IClipboardDataProvider {
			public CellsClipboardDataProvider(IEnumerable<GridCell> cells, ClipboardController owner) {
				this.cells = cells;
				this.owner = owner;
			}
			IEnumerable<GridCell> cells;
			ClipboardController owner;
			public object GetObjectFromClipboard() {
				List<int> rows = new List<int>();
				foreach(GridCell cell in cells)
					if(!rows.Contains(cell.RowHandle))
						rows.Add(cell.RowHandle);
				return owner.GetSelectedData(rows);
			}
			public string GetTextFromClipboard() {
				return owner.GetTextInRows(cells.OrderBy(cell => cell, new CellComparer(owner.View)).ToList());
			}
		}
		protected new GridViewBase View { get { return base.View as GridViewBase; } }
		public ClipboardController(GridViewBase view) : base(view) {  }
		public void CopyCellsToClipboard(IEnumerable<GridCell> cells) {
			CopyToClipboard(() => { return CreateCellsCopyingToClipboardEventArgs(cells); }, new CellsClipboardDataProvider(cells, this));
		}
		internal string GetTextInRows(IEnumerable<GridCell> gridCells) {
			if(gridCells.Count() == 0)
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			List<GridColumn> columns = new List<GridColumn>();
			int minGroupLevel = 100, dataLevel = 0;
			GetOffsetsAndVisibleColumns(gridCells, columns, out minGroupLevel, out dataLevel);
			if(View.ActualClipboardCopyWithHeaders) AppendColumnHeadersText(sb, columns, dataLevel);
			AppendCellValues(gridCells, sb, columns, minGroupLevel, dataLevel);
			return sb.ToString();
		}
		void AppendCellValues(IEnumerable<GridCell> gridCells, StringBuilder sb, List<GridColumn> columns, int minGroupLevel, int dataLevel) {
			int lastColumnIndex = 0;
			int oldRow = gridCells.ElementAt(0).RowHandle;
			bool shoulAddGroupText = true;
			foreach(GridCell cell in gridCells) {
				if(oldRow != cell.RowHandle) {
					sb.Append(Environment.NewLine);
					lastColumnIndex = 0;
					shoulAddGroupText = true;
				}
				if(!View.Grid.IsGroupRowHandle(cell.RowHandle)) {
					if(oldRow != cell.RowHandle)
						AppendRowIndent(sb, dataLevel);
					for(int i = 0; i < columns.IndexOf(cell.Column) - lastColumnIndex; i++)
						sb.Append("\t");
					lastColumnIndex = columns.IndexOf(cell.Column);
					sb.Append(View.GetTextForClipboard(cell.RowHandle, View.VisibleColumns.IndexOf(cell.Column)));
				}
				else {
					if(shoulAddGroupText) {
						AppendRowIndent(sb, View.Grid.GetRowLevelByRowHandle(cell.RowHandle) - minGroupLevel);
						sb.Append(View.GetGroupRowDisplayText(cell.RowHandle));
						shoulAddGroupText = false;
					}
				}
				oldRow = cell.RowHandle;
			}
		}
		private void AppendColumnHeadersText(StringBuilder sb, List<GridColumn> columns, int dataLevel) {
			if(columns.Count == 0)
				return;
			AppendRowIndent(sb, dataLevel);
			foreach(GridColumn column in columns) {
				sb.Append(View.GetTextForClipboard(GridControl.InvalidRowHandle, View.VisibleColumns.IndexOf(column)) + "\t");
			}
			sb.Remove(sb.Length - 1, 1);
			sb.Append(Environment.NewLine);
		}
		void GetOffsetsAndVisibleColumns(IEnumerable<GridCell> gridCells, List<GridColumn> columns, out int minGroupLevel, out int dataLevel) {
			minGroupLevel = 100;
			dataLevel = 0;
			foreach(GridCell cell in gridCells) {
				if(View.Grid.IsGroupRowHandle(cell.RowHandle)) {
					int level = View.Grid.GetRowLevelByRowHandle(cell.RowHandle);
					minGroupLevel = Math.Min(level, minGroupLevel);
					dataLevel = Math.Max(dataLevel, level);
					continue;
				}
				if(!columns.Contains(cell.Column)) {
					columns.Add(cell.Column);
				}
			}
			columns.Sort((column1, column2) => Comparer<int>.Default.Compare(column1.VisibleIndex, column2.VisibleIndex));
			if(minGroupLevel == 100) {
				minGroupLevel = -1;
				dataLevel = 0;
			}
			else {
				dataLevel = (dataLevel - minGroupLevel) + 1;
			}
		}
		void AppendRowIndent(StringBuilder sb, int count) {
			for(int n = 0; n < count; n++)
				sb.Append('\t');
		}
		protected override bool CanAddRowToSelectedData(DataControlBase dataControl, int rowHandle) {
			GridViewBase dataView = (GridViewBase)dataControl.DataView;
			return !dataView.IsGroupRow(dataView.DataProviderBase.GetRowVisibleIndexByHandle(rowHandle), 1);
		}
		protected override int GetCountCopyRows(IEnumerable<KeyValuePair<DataControlBase, int>> rows) {
			if(View.DataProviderBase.IsServerMode && View.ClipboardCopyMaxRowCountInServerMode != -1)
				return Math.Min(View.ClipboardCopyMaxRowCountInServerMode, rows.Count());
			return base.GetCountCopyRows(rows);
		}
		protected virtual CopyingToClipboardEventArgs CreateCellsCopyingToClipboardEventArgs(IEnumerable<GridCell> cells) {
			return new CopyingToClipboardEventArgs(View, cells, true);
		}
		protected override CopyingToClipboardEventArgsBase CreateRowsCopyingToClipboardEventArgs(IEnumerable<int> rows) {
			return new CopyingToClipboardEventArgs(View, rows, true);
		}
	}
	public class CellComparer : IComparer<CellBase> {
		DataViewBase view;
		public CellComparer(DataViewBase view) {
			this.view = view;
		}
		public int Compare(CellBase x, CellBase y) {
			int visibleIndexCompare = Comparer<int>.Default.Compare(view.DataControl.GetRowVisibleIndexByHandleCore(x.RowHandleCore), view.DataControl.GetRowVisibleIndexByHandleCore(y.RowHandleCore));
			if(visibleIndexCompare != 0)
				return visibleIndexCompare;
			return Comparer<int>.Default.Compare(x.ColumnCore.VisibleIndex, y.ColumnCore.VisibleIndex);
		}
	}
}

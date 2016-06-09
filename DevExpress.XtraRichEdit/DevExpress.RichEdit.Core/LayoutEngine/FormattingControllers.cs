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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region TableCellColumnController
	public class TableCellColumnController : IColumnController {
		#region Fields
		readonly IColumnController parent;
		Column currentParentColumn;
		int top;
		int left;
		int width;
		readonly List<Column> generatedColumns;
		readonly List<TableViewInfo> generatedTableViewInfo;
		int currentColumnIndex; 
		TableCellViewInfo currentCellViewInfo;
		TableCellRow lastCreatedRow;
		TableCell currentCell;
		#endregion
		public TableCellColumnController(IColumnController parent, Column currentParentColumn, int left, int top, int width, TableViewInfo tableViewInfo, TableCell currentCell) {
			Guard.ArgumentNotNull(parent, "parent");
			Guard.ArgumentNotNull(currentParentColumn, "currentParentColumn");
			this.parent = parent;
			this.currentParentColumn = currentParentColumn;
			this.currentCell = currentCell;
			this.generatedColumns = new List<Column>();
			generatedColumns.Add(currentParentColumn);
			StartNewCell(currentParentColumn, left, top, width, currentCell);
			this.generatedTableViewInfo = new List<TableViewInfo>();			
			generatedTableViewInfo.Add(tableViewInfo);
		}
		#region Properties
		public int TopLevelColumnsCount { get { return Parent.TopLevelColumnsCount; } }
		public IColumnController Parent { get { return parent; } }
		public RunIndex PageLastRunIndex { get { return PageAreaController.PageController.PageLastRunIndex; } }
		internal Column ParentColumn { get { return generatedColumns[currentColumnIndex]; } }
		internal Column LastParentColumn { get { return generatedColumns[generatedColumns.Count - 1]; } }
		internal Column FirstParentColumn { get { return generatedColumns[0]; } }
		internal Column CurrentTopLevelColumn { get { return (parent is TableCellColumnController) ? ((TableCellColumnController)parent).CurrentTopLevelColumn : ParentColumn; } }
		internal TableViewInfo ViewInfo { get { return currentColumnIndex < generatedTableViewInfo.Count ? generatedTableViewInfo[currentColumnIndex] : null; } }
		public BoxMeasurer Measurer { get { return parent.Measurer; } }
		public PageAreaController PageAreaController { get { return Parent.PageAreaController; } }
		public virtual bool ShouldZeroSpacingBeforeWhenMoveRowToNextColumn { get { return false; } }
		public TableCellViewInfo CurrentCell { get { return currentCellViewInfo; } }
		#endregion
		#region IColumnController Members
		public void ResetToFirstColumn() {
			parent.ResetToFirstColumn();
		}
		public CompleteFormattingResult CompleteCurrentColumnFormatting(Column column) {
			if (currentColumnIndex + 1 >= generatedColumns.Count)
				return parent.CompleteCurrentColumnFormatting(column);
			else
				return CompleteFormattingResult.Success;
		}
		public Column GetNextColumn(Column column, bool keepFloatingObjects) {
			currentColumnIndex++;
			if (currentColumnIndex >= generatedColumns.Count) {
				Column newColumn = parent.GetNextColumn(currentParentColumn, keepFloatingObjects);
				generatedColumns.Add(newColumn);
			}
			else {
				TableCellColumnController parentController = parent as TableCellColumnController;
				if(parentController != null)
					parentController.MoveToNextColumn();
			}
			currentParentColumn = generatedColumns[currentColumnIndex];
			Rectangle bounds = currentParentColumn.Bounds;
			TableCellColumn result = new TableCellColumn(currentParentColumn, currentCell);
			result.Bounds = new Rectangle(bounds.Left + left, bounds.Top, width, bounds.Height);
#if DEBUGTEST || DEBUG                        
			if (result.Parent.GetType() == typeof(Column)) {
				if (result.TopLevelColumn != currentParentColumn)
					Exceptions.ThrowInternalException();
			}
#endif
			return result;
		}
		public void MoveToNextColumn() {
			currentColumnIndex++;
			TableCellColumnController parentController = parent as TableCellColumnController;
			if(parentController != null)
				parentController.MoveToNextColumn();
			currentParentColumn = generatedColumns[currentColumnIndex];
		}
		#endregion
		public Column GetStartColumn() {
			Rectangle bounds = generatedColumns[currentColumnIndex].Bounds;
			Column result = new TableCellColumn(generatedColumns[currentColumnIndex], currentCell);
			int columnTop = top;
			result.Bounds = new Rectangle(bounds.Left + left, columnTop, width, bounds.Bottom - top);
			return result;
		}
		public void StartNewCell(Column currentParentColumn, int left, int top, int width, TableCell currentCell) {
			this.left = left;
			this.top = top;
			this.width = width;
			this.currentCell = currentCell;
			SetCurrentParentColumn(currentParentColumn);
		}
		public TableCellVerticalAnchor GetMaxAnchor(TableCellVerticalAnchor anchor1, TableCellVerticalAnchor anchor2) {						
			if(anchor1.VerticalPosition > anchor2.VerticalPosition)
				return new TableCellVerticalAnchor(anchor1.VerticalPosition, anchor1.BottomTextIndent, anchor1.CellBorders);
			else
				return new TableCellVerticalAnchor(anchor2.VerticalPosition, anchor2.BottomTextIndent, anchor1.CellBorders);
		}
		public void SetCurrentParentColumn(Column column) {
			this.currentColumnIndex = generatedColumns.IndexOf(column);
			currentParentColumn = column;
			TableCellColumnController parentCellController = Parent as TableCellColumnController;
			if(parentCellController != null)
				parentCellController.SetCurrentParentColumn(((TableCellColumn)column).Parent);
		}
		internal void AddTableViewInfo(TableViewInfo currentTableViewInfo) {
			generatedTableViewInfo.Add(currentTableViewInfo);
		}
		public Row CreateRow() {
			lastCreatedRow = new TableCellRow(currentCellViewInfo);
			return lastCreatedRow;
		}
		internal void SetCurrentTableCellViewInfo(TableCellViewInfo value) {
			if (this.lastCreatedRow != null)
				lastCreatedRow.CellViewInfo = value;
			this.currentCellViewInfo = value;
		}		
		public void AddInnerTable(TableViewInfo tableViewInfo) {
			this.CurrentCell.AddInnerTable(tableViewInfo);
		}
		internal void RemoveTableViewInfo(TableViewInfo tableViewInfo) {
			generatedTableViewInfo.Remove(tableViewInfo);
		}
		public void RemoveGeneratedColumn(Column column) {			
			ColumnController columnController = parent as ColumnController;
			int initialColumnCount = generatedColumns.Count;			
			if (initialColumnCount > 1) {
				generatedColumns.RemoveAt(initialColumnCount - 1);
				if (currentColumnIndex >= generatedColumns.Count) {
					currentColumnIndex = generatedColumns.Count - 1;
					currentParentColumn = generatedColumns[currentColumnIndex];
				}
			}
			if (columnController != null && initialColumnCount > 1)
				columnController.RemoveGeneratedColumn(column);
		}
		public Column GetPreviousColumn(Column column) {
			return null;
		}
		public Rectangle GetCurrentPageBounds(Page currentPage, Column currentColumn) {		   
			return currentColumn.Bounds;
		}
		public Rectangle GetCurrentPageClientBounds(Page currentPage, Column currentColumn) {
			return currentColumn.Bounds;
		}
	}
	#endregion
}

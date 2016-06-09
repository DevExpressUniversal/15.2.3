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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Native.Templates {
	public class TemplateWriter {
		readonly SnapPieceTable pieceTable;
		InputPosition inputPosition;
		Stack<DocumentLogPosition> fieldStartPositions;
		TableCell currentTableCell;
		TableRow currentTableRow;
		Table currentTable;
		Stack<ParagraphIndex> cellStartParagraphIndex;
		public TemplateWriter(SnapPieceTable pieceTable, InputPosition inputPosition) {
			this.pieceTable = pieceTable;
			this.cellStartParagraphIndex = new Stack<ParagraphIndex>();
			this.inputPosition = inputPosition;
			fieldStartPositions = new Stack<DocumentLogPosition>();
			if (inputPosition.PieceTable != pieceTable)
				Exceptions.ThrowInternalException();
		}
		public virtual void WriteTree(InsertActionTreeNode startNode) {			
			WriteTemplateForActionsRecursive(startNode);
		}
		protected virtual void WriteTemplateForActionsRecursive(InsertActionTreeNode node) {
			node.Action.WriteStartText(this);
			foreach (InsertActionTreeNode innerAction in node.ChildNodes) {
				node.Action.BeforeWriteInnerAction(innerAction.Action, this);
				WriteTemplateForActionsRecursive(innerAction);
				node.Action.AfterWriteInnerAction(innerAction.Action, this);
			}
			node.Action.WriteEndText(this);
		}		
		public void BeginField() {
			fieldStartPositions.Push(inputPosition.LogPosition);
		}
		public Field EndField() {
			DocumentLogPosition startPosition = fieldStartPositions.Pop();
			Field field = pieceTable.CreateField(startPosition, inputPosition.LogPosition - startPosition);
			inputPosition.LogPosition += 3;
			return field;
		}
		public void BeginTemplate() {
			BeginField();
		}
		public void EndTemplate() {
			Field field = EndField();
			field.DisableUpdate = true;
		}
		public void WriteText(string text) {
			pieceTable.InsertTextCore(inputPosition, text);
		}
		public void WriteParagraphStart() {
			pieceTable.InsertParagraphCore(inputPosition);
		}
		public void WriteParagraphStart(string baseStyleName) {
			ParagraphIndex paragraphIndex = inputPosition.ParagraphIndex;
			pieceTable.InsertParagraphCore(inputPosition);
			int styleIndex = pieceTable.DocumentModel.ParagraphStyles.GetStyleIndexByName(baseStyleName);
			if(styleIndex >= 0)
				pieceTable.Paragraphs[paragraphIndex].ParagraphStyleIndex = styleIndex;
		}
		public void BeginTable() {
			Debug.Assert(
				(currentTable != null && currentTableRow != null && currentTableCell != null) ||
				(currentTable == null && currentTableRow == null && currentTableCell == null));
			currentTable = new Table(pieceTable, currentTableCell, 0, 0);
			currentTable.TableProperties.AvoidDoubleBorders = true;
			pieceTable.Tables.Add(currentTable);
			currentTableRow = null;
			currentTableCell = null;
		}
		public Table EndTable() {
			return EndTable(false);
		}
		public Table EndTable(WidthUnitInfo preferredWidthInfo) {
			return EndTable(TableStyleCollection.DefaultTableStyleName, -1, preferredWidthInfo);
		}
		public Table EndTable(bool applyPercentWidth) {
			WidthUnitInfo preferredWidthInfo = applyPercentWidth ? new WidthUnitInfo(WidthUnitType.FiftiethsOfPercent, 50 * 100) : null;
			return EndTable(preferredWidthInfo);
		}
		public Table EndTable(string styleName, int level, bool applyPercentWidth) {
			WidthUnitInfo preferredWidthInfo = applyPercentWidth ? new WidthUnitInfo(WidthUnitType.FiftiethsOfPercent, 50 * 100) : null;
			return EndTable(styleName, level, preferredWidthInfo);
		}
		public Table EndTable(string styleName, int level,  WidthUnitInfo preferredWidthInfo) {
			Table result = currentTable;
			Debug.Assert(currentTable != null);
			Debug.Assert(currentTableRow == null);
			Debug.Assert(currentTableCell == null);
			EnsureCellSpansValid(currentTable);
			ApplyTableStyle(currentTable, styleName, level);
			if (preferredWidthInfo != null) {
				PreferredWidth preferredWidth = currentTable.TableProperties.PreferredWidth;
				preferredWidth.BeginUpdate();
				try {
					preferredWidth.CopyFrom(preferredWidthInfo);					
				}
				finally {
					preferredWidth.EndUpdate();
				}
			}
			currentTableCell = currentTable.ParentCell;
			if (currentTableCell != null) {
				currentTableRow = currentTableCell.Row;
				currentTable = currentTableCell.Table;
			}			
			else
				currentTable = null;
			return result;
		}
		public void EnsureCellSpansValid(Table table) {
			int lcmColumnSpan = 1;
			table.Rows.ForEach(
				row => {
					int totalColumnSpan = table.GetTotalCellsInRowConsiderGrid(row);
					lcmColumnSpan = (int)MathUtils.LCM(lcmColumnSpan, totalColumnSpan);
				});
			table.Rows.ForEach(
				row => {
					int multiplier = lcmColumnSpan / table.GetTotalCellsInRowConsiderGrid(row);
					if (multiplier == 1)
						return;
					if(row.GridBefore > 0)
						row.Properties.GridBefore = row.GridBefore * multiplier;
					if (row.GridAfter > 0)
						row.Properties.GridAfter = row.GridAfter * multiplier;
					row.Cells.ForEach(cell => { cell.Properties.ColumnSpan = cell.ColumnSpan * multiplier; });
				});
		}
		void ApplyTableStyle(Table table, string baseStyleName, int level) {
			string styleName = DevExpress.Snap.Core.Native.StyleHelper.GetStyleName(baseStyleName, level, pieceTable.DocumentModel);
			int styleIndex = pieceTable.DocumentModel.TableStyles.GetStyleIndexByName(styleName);
			if (styleIndex >= 0)
				table.StyleIndex = styleIndex;
		}
		public void BeginTableRow() {
			currentTableRow = new TableRow(currentTable, 0);
		}
		public virtual void EndTableRow() {
			EndTableRow(false);
		}
		public virtual void EndTableRow(bool applyEqualPercentWidthToCells) {
			Debug.Assert(currentTable != null);
			Debug.Assert(currentTableRow != null);
			Debug.Assert(currentTableCell == null);
			if (currentTableRow.Cells.Count > 0) {
				currentTable.Rows.AddInternal(currentTableRow);
				if(applyEqualPercentWidthToCells)
					ApplyEqualPercentWidthToCells(currentTableRow.Cells);
			}
			currentTableRow = null;
		}
		protected virtual void ApplyEqualPercentWidthToCells(TableCellCollection cells) {
			Debug.Assert(cells != null);
			Debug.Assert(cells.Count > 0);
			int count = cells.Count;
			int rest = 50 * 100;
			for (int i = 0; i < count; i++) {
				PreferredWidth width = cells[i].Properties.PreferredWidth;
				int cellWidth = rest / (count - i);
				width.BeginUpdate();
				try {
					width.Type = WidthUnitType.FiftiethsOfPercent;
					width.Value = cellWidth;
				}
				finally {
					width.EndUpdate();
				}
				rest -= cellWidth;
			}
		}
		public void BeginTableCell() {
			BeginTableCell(false);
		}
		public void BeginTableCell(bool changeStyleProperties) {
			currentTableCell = new TableCell(currentTableRow);
			currentTableRow.Cells.AddInternal(currentTableCell);
			if (changeStyleProperties)
				TableCellHelper.PrepareMasterCell(currentTableCell);
			cellStartParagraphIndex.Push(inputPosition.ParagraphIndex);
		}
		public void EndTableCell() {
			EndTableCell(0, null);
		}
		public void EndTableCell(int preferredWidth, TableCellStyle style) {
			Debug.Assert(currentTable != null);
			Debug.Assert(currentTableRow != null);
			Debug.Assert(currentTableCell != null);
			ParagraphIndex startParagraphIndex = cellStartParagraphIndex.Pop();
			ParagraphIndex endParagraphIndex = inputPosition.ParagraphIndex - 1;
			Debug.Assert(endParagraphIndex >= startParagraphIndex);
			pieceTable.TableCellsManager.InitializeTableCell(currentTableCell, startParagraphIndex, endParagraphIndex);
			if (style != null) {
				DevExpress.Snap.Core.Native.StyleHelper.ApplyTableCellStyleDirect(style.StyleName, currentTableCell);
			}
			if (preferredWidth > 0) {
				currentTableCell.Properties.PreferredWidth.Type = WidthUnitType.ModelUnits;
				currentTableCell.Properties.PreferredWidth.Value = preferredWidth;
			}
			currentTableCell = null;			
		}
	}
}

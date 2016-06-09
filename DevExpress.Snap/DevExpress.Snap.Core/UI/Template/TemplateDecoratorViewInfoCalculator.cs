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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.UI.Templates {
	public class SnapTemplateDecoratorViewInfoCalculator {
		readonly SnapPieceTable pieceTable;
		readonly DocumentLogInterval actualResultInterval;
		readonly DocumentLogInterval actualVisibleResultInterval;
		readonly DocumentLayout documentLayout;
		public SnapTemplateDecoratorViewInfoCalculator(SnapPieceTable pieceTable, DocumentLayout documentLayout, DocumentLogInterval interval) {
			this.pieceTable = pieceTable;
			actualResultInterval = interval;
			actualVisibleResultInterval = interval;
			DocumentModelPosition startPosition = DevExpress.XtraRichEdit.Utils.PositionConverter.ToDocumentModelPosition(pieceTable, interval.Start);
			if (!pieceTable.VisibleTextFilter.IsRunVisible(startPosition.RunIndex)) {
				RunIndex index = pieceTable.VisibleTextFilter.FindVisibleRunForward(startPosition.RunIndex);
				DocumentLogPosition newStart = pieceTable.GetRunLogPosition(pieceTable.Runs[index]);
				actualVisibleResultInterval = new DocumentLogInterval(newStart, interval.Start + interval.Length - newStart);
			}
			this.documentLayout = documentLayout;
		}
		protected SnapPieceTable PieceTable { get { return pieceTable; } }
		public List<TemplateDecoratorItem> Calculate(Page page) {
			List<TemplateDecoratorItem> result = new List<TemplateDecoratorItem>();
			foreach (PageArea area in page.Areas) {
				CalculateViewInfoForPageArea(page, area, result);
			}
			return result;
		}
		protected virtual void CalculateViewInfoForPageArea(Page page, PageArea area, List<TemplateDecoratorItem> result) {
			if (!Object.ReferenceEquals(area.PieceTable, PieceTable))
				return;
			foreach (Column column in area.Columns) {
				TemplateDecoratorItem item = CalcualteViewInfoForColumn(page, area, column);
				if (item != null)
					result.Add(item);
			}
		}
		private TemplateDecoratorItem CalcualteViewInfoForColumn(Page page, PageArea area, Column column) {
			DocumentLogPosition visibleStartLogPosition = actualVisibleResultInterval.Start;
			DocumentLogPosition visibleEndLogPosition = actualVisibleResultInterval.Start + actualVisibleResultInterval.Length;
			ExactColumnAndLogPositionComparable startComparer = new ExactColumnAndLogPositionComparable(PieceTable, visibleStartLogPosition);
			ExactColumnAndLogPositionComparable endComparer = new ExactColumnAndLogPositionComparable(PieceTable, visibleEndLogPosition);
			int bookmarkStartCompareResult = startComparer.CompareTo(column);
			int bookmarkEndCompareResult = endComparer.CompareTo(column);
			bool isColumnWithinBookmark = IsColumnWithinBookmark(visibleStartLogPosition, visibleEndLogPosition, column);
			if(bookmarkEndCompareResult > 0 && !isColumnWithinBookmark)
				return null;
			if(bookmarkStartCompareResult < 0 && !isColumnWithinBookmark)
				return null;
			DocumentLogPosition startLogPosition = actualResultInterval.Start;
			DocumentLogPosition endLogPosition = actualResultInterval.Start + actualResultInterval.Length;
			DocumentLayoutPosition startPosition = null;
			if(bookmarkStartCompareResult == 0 || isColumnWithinBookmark) {
				startPosition = CalculateCharacterLayoutPosition(page, area, column, startLogPosition);
				if (startPosition == null)
					return null;
			}
			DocumentLayoutPosition endPosition = null;
			if(bookmarkEndCompareResult == 0 || isColumnWithinBookmark) {
				endPosition = CalculateCharacterLayoutPosition(page, area, column, endLogPosition);
				if (endPosition == null)
					return null;
			}
			TableCellViewInfo startCell = GetStartCell(startPosition, startLogPosition);
			TableCellViewInfo endCell = GetEndCell(endPosition, endLogPosition);
			if (startCell != null && endCell != null) {
				while (endCell.Cell.Table != startCell.Cell.Table) {
					TableCellViewInfo parentViewInfo = endCell.TableViewInfo.ParentTableCellViewInfo;
					if (parentViewInfo == null)
						break;
					endCell = parentViewInfo;
				}
			}
			TemplateDecoratorItemBoundaryBase startBoundary = GetBoundary(column, startPosition, startCell);
			TemplateDecoratorItemBoundaryBase endBoundary = GetBoundary(column, endPosition, endCell);
			return new TemplateDecoratorItem(startBoundary, endBoundary);
		}
		bool IsColumnWithinBookmark(DocumentLogPosition startPos, DocumentLogPosition endPos, Column column) {
			DocumentModelPosition columnPos = column.GetFirstPosition(PieceTable);
			if(startPos <= columnPos.LogPosition && columnPos.LogPosition <= endPos)
				return true;
			return false;
		}
		DocumentLayoutPosition CalculateCharacterLayoutPosition(Page page, PageArea area, Column column, DocumentLogPosition logPosition) {
			DocumentLayoutPosition result = new DocumentLayoutPosition(documentLayout, PieceTable, logPosition);
			result.Page = page;
			result.PageArea = area;
			result.Column = column;
			DocumentLayoutDetailsLevel level = result.UpdateCellRecursive(column, DocumentLayoutDetailsLevel.Character);
			if(level != DocumentLayoutDetailsLevel.Character && level != DocumentLayoutDetailsLevel.Column)
				return null;
			return result;
		}
		protected virtual TableCellViewInfo GetStartCell(DocumentLayoutPosition startPosition, DocumentLogPosition startLogPosition) {
			if (startPosition == null)
				return null;
			TableCellViewInfo result = startPosition.TableCell;
			if (result == null)
				return result;
			Paragraph startParagraph = PieceTable.Paragraphs[result.Cell.StartParagraphIndex];
			if (startParagraph.LogPosition == startLogPosition)
				return result;
			if (startParagraph.LogPosition == startLogPosition - 1 && PieceTable.Runs[startParagraph.FirstRunIndex] is SeparatorTextRun)
				return result;
			else
				return null;
		}
		protected virtual TableCellViewInfo GetEndCell(DocumentLayoutPosition endPosition, DocumentLogPosition endLogPosition) {
			if (endPosition == null)
				return null;
			TableCellViewInfo result = endPosition.TableCell;
			if (result == null)
				return result;
			if (PieceTable.Paragraphs[result.Cell.EndParagraphIndex].EndLogPosition == endLogPosition)
				return result;
			else
				return null;
		}
		protected virtual TemplateDecoratorItemBoundaryBase GetBoundary(Column column, DocumentLayoutPosition position, TableCellViewInfo tableCellViewInfo) {
			if (position != null) {
				if (tableCellViewInfo != null)
					return new TemplateDecoratorItemTableRowBoundary(column, tableCellViewInfo);
				if (position.Box is ParagraphMarkBox)
					return new TemplateDecoratorItemParagraphBoundary(column, position.Row);
				else
					return new TemplateDecoratorItemTextBoundary(column, position.Row, position.Character);
			}
			else {
				return new TemplateDecoratorItemColumnBoundary(column);
			}
		}
	}
}

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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region IColumnController
	public interface IColumnController {
		void ResetToFirstColumn();
		RunIndex PageLastRunIndex { get; }
		Column GetNextColumn(Column column, bool keepFloatingObjects);
		CompleteFormattingResult CompleteCurrentColumnFormatting(Column column);
		BoxMeasurer Measurer { get; }
		Row CreateRow();
		void AddInnerTable(TableViewInfo tableViewInfo);
		PageAreaController PageAreaController { get; }
		bool ShouldZeroSpacingBeforeWhenMoveRowToNextColumn { get; }
		Column GetPreviousColumn(Column column);
		int TopLevelColumnsCount { get; }
		Rectangle GetCurrentPageBounds(Page currentPage, Column currentColumn);
		Rectangle GetCurrentPageClientBounds(Page currentPage, Column currentColumn);
	}
	#endregion
	#region ColumnController
	public abstract class ColumnController : IColumnController {
		#region Fields
		List<Rectangle> columnsBounds;
		readonly PageAreaController pageAreaController;
		int nextColumnIndex;
		#endregion
		protected ColumnController(PageAreaController pageAreaController) {
			Guard.ArgumentNotNull(pageAreaController, "pageAreaController");
			this.pageAreaController = pageAreaController;
			this.columnsBounds = new List<Rectangle>();
		}
		#region Properties
		public PageAreaController PageAreaController { get { return pageAreaController; } }
		public virtual ColumnCollection Columns { get { return pageAreaController.Areas.Last.Columns; } }
		public int TopLevelColumnsCount { get { return ColumnsBounds.Count; } }
		protected internal List<Rectangle> ColumnsBounds { get { return columnsBounds; } }
		public BoxMeasurer Measurer { get { return PageAreaController.PageController.DocumentLayout.Measurer; } }
		public virtual RunIndex PageLastRunIndex { get { return PageAreaController.PageController.PageLastRunIndex; } }
		public virtual bool ShouldZeroSpacingBeforeWhenMoveRowToNextColumn { get { return true; } }
		#endregion
		#region Events
		#region GenerateNewColumn
		EventHandler onGenerateNewColumn;
		public event EventHandler GenerateNewColumn { add { onGenerateNewColumn += value; } remove { onGenerateNewColumn -= value; } }
		protected internal virtual void RaiseGenerateNewColumn() {
			if (onGenerateNewColumn != null)
				onGenerateNewColumn(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		public virtual void Reset(Section section) {
			ResetToFirstColumn();
			ColumnsBounds.Clear();
			BeginSectionFormatting(section);
		}
		public virtual void ClearInvalidatedContent(FormatterPosition pos) {
			ClearInvalidatedContentCore(pos, Columns);
		}
		protected internal void ClearInvalidatedContentCore(FormatterPosition pos, ColumnCollection columns) {
			int columnIndex = columns.BinarySearchBoxIndex(pos);
			if (columnIndex < 0) {
				columnIndex = ~columnIndex;
			}
			if (columnIndex < columns.Count) {
				Column column = columns[columnIndex];
				TableViewInfoCollection tables = column.InnerTables;
				if (tables != null) {
					int count = tables.Count;
					for (int i = count - 1; i >= 0; i--) {
						TableViewInfo tableViewInfo = tables[i];
						Table table = tableViewInfo.Table;
						if (table.Rows.Count == 0)
							tables.RemoveAt(i);
						else {
							ParagraphIndex paragraphIndex = table.Rows.First.Cells.First.StartParagraphIndex;
							if (paragraphIndex >= new ParagraphIndex(table.PieceTable.Paragraphs.Count))
								tables.RemoveAt(i);
							else {
								Paragraph paragraph = table.PieceTable.Paragraphs[paragraphIndex];
								if (paragraph.FirstRunIndex >= pos.RunIndex)
									tables.RemoveAt(i);
							}
						}
					}
				}
			}
			if (columnIndex + 1 < columns.Count)
				columns.RemoveRange(columnIndex + 1, columns.Count - columnIndex - 1);
		}
		protected internal virtual void CleanupEmptyBoxes(Column lastColumn) {
		}
		public void ResetToFirstColumn() {
			nextColumnIndex = 0;
		}
		protected internal virtual void BeginSectionFormatting(Section section) {
			ApplySectionStart(section);
			CreateColumnBounds();
		}
		protected internal virtual void RestartFormattingFromTheStartOfSection(Section section, int currentColumnIndex) {
			CreateColumnBounds();
			this.nextColumnIndex = (currentColumnIndex + 1) % columnsBounds.Count;
			ApplySectionStart(section);
		}
		protected internal virtual void RestartFormattingFromTheMiddleOfSection(Section section, int currentColumnIndex) {
			CreateColumnBounds();
			this.nextColumnIndex = (currentColumnIndex + 1) % columnsBounds.Count;
		}
		protected internal virtual void RestartFormattingFromTheStartOfRowAtCurrentPage() {
		}
		public virtual CompleteFormattingResult CompleteCurrentColumnFormatting(Column column) {
			if (nextColumnIndex > 0 || column == null)
				return CompleteFormattingResult.Success;
			else
				return PageAreaController.CompleteCurrentAreaFormatting();
		}
		public virtual Column GetNextColumn(Column column, bool keepFloatingObjects) {
			if (nextColumnIndex == 0)
				PageAreaController.GetNextPageArea(keepFloatingObjects);
			if (columnsBounds.Count <= 0)
				CreateColumnBounds();
			Column newColumn = GetNextColumnCore(column);
			AddColumn(newColumn);
			RaiseGenerateNewColumn();
			return newColumn;
		}
		public void RemoveGeneratedColumn(Column column) {
			Debug.Assert(column == this.Columns.Last);
			Columns.RemoveAt(Columns.Count - 1);
			if (Columns.Count == 0)
				PageAreaController.RemoveLastPageArea();
		}
		protected internal virtual void AddColumn(Column column) {
#if DEBUGTEST
			Debug.Assert(pageAreaController.PageController.Pages.Last.SecondaryFormattingComplete == false);
#endif
			Columns.Add(column);
		}
		protected internal virtual Column GetNextColumnCore(Column column) {
			Column result = new Column();
			Rectangle bounds = CalculateColumnBounds(nextColumnIndex);
			result.Bounds = bounds;
			nextColumnIndex = (nextColumnIndex + 1) % columnsBounds.Count;
			return result;
		}
		protected Rectangle CalculateColumnBounds(int columnIndex) {
			Rectangle result = CalculateColumnBoundsCore(columnIndex);
			Rectangle areaBounds = pageAreaController.CurrentAreaBounds;
			result.Y = areaBounds.Y;
			result.Height = areaBounds.Height;
			return result;
		}
		protected internal abstract Rectangle CalculateColumnBoundsCore(int columnIndex);
		protected internal virtual void ApplySectionStart(Section section) {
			switch (section.GeneralSettings.StartType) {
				case SectionStartType.Continuous:
					nextColumnIndex = 0;
					break;
				case SectionStartType.Column:
					if (section.GetActualColumnsCount() != this.TopLevelColumnsCount)
						nextColumnIndex = 0;
					break;
				case SectionStartType.EvenPage:
				case SectionStartType.OddPage:
				case SectionStartType.NextPage:
					nextColumnIndex = 0;
					break;
				default:
					break;
			}
		}
		protected internal virtual void CreateColumnBounds() {
			ColumnsBoundsCalculator calculator = CreateColumnBoundsCalculator();
			this.columnsBounds = calculator.Calculate(pageAreaController.PageController.CurrentSection, pageAreaController.CurrentAreaBounds);
		}
		protected internal virtual ColumnsBoundsCalculator CreateColumnBoundsCalculator() {
			return new ColumnsBoundsCalculator(pageAreaController.PageController.DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter);
		}
		public Row CreateRow() {
			return new Row();
		}
		public virtual void AddInnerTable(TableViewInfo tableViewInfo) {
			Debug.Assert(tableViewInfo.Table.ParentCell == null);
		}
		public Column GetPreviousColumn(Column column) {
			int index = Columns.IndexOf(column);
			if (index > 0)
				return Columns[index - 1];
			else
				return null;
		}
		public virtual Rectangle GetCurrentPageBounds(Page currentPage, Column currentColumn) {
			return currentPage.Bounds;
		}
		public virtual Rectangle GetCurrentPageClientBounds(Page currentPage, Column currentColumn) {
			return currentPage.ClientBounds;
		}
	}
	#endregion
	#region ColumnsBoundsCalculator
	public class ColumnsBoundsCalculator {
		readonly DocumentModelUnitToLayoutUnitConverter unitConverter;
		public ColumnsBoundsCalculator(DocumentModelUnitToLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
		public DocumentModelUnitToLayoutUnitConverter UnitConverter { get { return unitConverter; } }
		public virtual List<Rectangle> Calculate(Section section, Rectangle bounds) {
			List<Rectangle> result = new List<Rectangle>();
			SectionColumns columnsSettings = section.Columns;
			if (columnsSettings.EqualWidthColumns)
				PopulateEqualWidthColumnsBounds(result, bounds, columnsSettings.ColumnCount, unitConverter.ToLayoutUnits(columnsSettings.Space));
			else
				PopulateColumnsBounds(result, bounds, columnsSettings.GetColumns());
			return result;
		}
		protected internal virtual void PopulateColumnsBounds(List<Rectangle> result, Rectangle bounds, ColumnInfoCollection columnInfoCollection) {
			int x = bounds.Left;
			int count = columnInfoCollection.Count;
			for (int i = 0; i < count; i++) {
				Rectangle columnBounds = bounds;
				columnBounds.X = x;
				columnBounds.Width = unitConverter.ToLayoutUnits(columnInfoCollection[i].Width);
				x += columnBounds.Width + unitConverter.ToLayoutUnits(columnInfoCollection[i].Space);
				result.Add(columnBounds);
			}
		}
		protected internal virtual void PopulateEqualWidthColumnsBounds(List<Rectangle> columnBounds, Rectangle bounds, int columnCount, int spaceBetweenColumns) {
			System.Diagnostics.Debug.Assert(columnCount > 0);
			Rectangle rect = bounds;
			rect.Width -= spaceBetweenColumns * (columnCount - 1);
			Rectangle[] rects = RectangleUtils.SplitHorizontally(rect, columnCount);
			PopulateEqualWidthColumnsBoundsCore(columnBounds, rects, spaceBetweenColumns);
		}
		protected internal virtual void PopulateEqualWidthColumnsBoundsCore(List<Rectangle> result, Rectangle[] columnRects, int spaceBetweenColumns) {
			System.Diagnostics.Debug.Assert(columnRects.Length > 0);
			result.Add(columnRects[0]);
			int count = columnRects.Length;
			for (int i = 1; i < count; i++) {
				Rectangle columnBounds = columnRects[i];
				columnBounds.Offset(i * spaceBetweenColumns, 0);
				result.Add(columnBounds);
			}
		}
	}
	#endregion
}

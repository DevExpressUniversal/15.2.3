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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Office.Drawing;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout {
	#region DocumentLayoutDetailsLevel
	public enum DocumentLayoutDetailsLevel {
		None = -1,
		Page = 0,
		PageArea = 1,
		Column = 2,
		TableRow = 3,
		TableCell = 4,
		Row = 5,
		Box = 6,
		Character = 7,
		Max = int.MaxValue
	}
	#endregion
	#region HitTestAccuracy
	[Flags]
	public enum HitTestAccuracy {
		ExactPage = 0x00000001,
		NearestPage = 0x00000000,
		ExactPageArea = 0x00000002,
		NearestPageArea = 0x00000000,
		ExactColumn = 0x00000004,
		NearestColumn = 0x00000000,
		ExactRow = 0x00000008,
		NearestRow = 0x00000000,
		ExactBox = 0x00000010,
		NearestBox = 0x00000000,
		ExactCharacter = 0x00000020,
		NearestCharacter = 0x00000000,
		ExactTableRow = 0x00000040,
		NearestTableRow = 0x00000000,
		ExactTableCell = 0x00000080,
		NearestTableCell = 0x00000000,
	}
	#endregion
	#region Box (abstract class)
	public abstract class Box  {
		Rectangle bounds;
		#region Properties
		public abstract FormatterPosition StartPos { get; set; }
		public abstract FormatterPosition EndPos { get; set; }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public virtual Rectangle ActualSizeBounds { get { return bounds; } }
		public abstract bool IsVisible { get; }
		public abstract Box CreateBox();
		public abstract void ExportTo(IDocumentLayoutExporter exporter);
		public abstract BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator);
		protected internal virtual DocumentLayoutDetailsLevel DetailsLevel { get { return DocumentLayoutDetailsLevel.Box; } }
		protected internal virtual HitTestAccuracy HitTestAccuracy { get { return HitTestAccuracy.ExactBox; } }
		public abstract bool IsNotWhiteSpaceBox { get;}
		public abstract bool IsLineBreak { get; }
		public virtual bool IsInlinePicture { get { return false; } }
		public virtual bool IsHyperlinkSupported { get { return true; } }
		#endregion
		public abstract string GetText(PieceTable table);
		public abstract TextRunBase GetRun(PieceTable pieceTable);
		public abstract void OffsetRunIndices(int delta);
		public virtual FontInfo GetFontInfo(PieceTable pieceTable) {
			return pieceTable.DocumentModel.FontCache[GetRun(pieceTable).FontCacheIndex];
		}
		public virtual int CalcDescent(PieceTable pieceTable) {
			FontInfo fontInfo = GetFontInfo(pieceTable);
			return fontInfo.Descent;
		}
		public virtual int CalcAscentAndFree(PieceTable pieceTable) {
			FontInfo fontInfo = GetFontInfo(pieceTable);
			return fontInfo.AscentAndFree;
		}
		public virtual int CalcBaseDescent(PieceTable pieceTable) {
			FontInfo fontInfo = GetFontInfo(pieceTable);
			return fontInfo.GetBaseDescent(pieceTable.DocumentModel);
		}
		public virtual int CalcBaseAscentAndFree(PieceTable pieceTable) {
			FontInfo fontInfo = GetFontInfo(pieceTable);
			return fontInfo.GetBaseAscentAndFree(pieceTable.DocumentModel);
		}
		protected internal virtual DocumentModelPosition GetDocumentPosition(PieceTable pieceTable, FormatterPosition pos) {
			DocumentModelPosition result = new DocumentModelPosition(pieceTable);
			Paragraph paragraph = GetRun(pieceTable).Paragraph;
			TextRunCollection runs = pieceTable.Runs;
			RunIndex lastRunIndex = pos.RunIndex;
			int paragraphOffset = pos.Offset;
			for (RunIndex i = paragraph.FirstRunIndex; i < lastRunIndex; i++)
				paragraphOffset += runs[i].Length;
			result.LogPosition = paragraph.LogPosition + paragraphOffset;
			result.ParagraphIndex = paragraph.Index;
			result.RunIndex = pos.RunIndex;
			result.RunStartLogPosition = result.LogPosition - pos.Offset;
			return result;
		}
		public abstract FormatterPosition GetFirstFormatterPosition();
		public abstract FormatterPosition GetLastFormatterPosition();
		public abstract DocumentModelPosition GetFirstPosition(PieceTable pieceTable);
		public abstract DocumentModelPosition GetLastPosition(PieceTable pieceTable);
		protected internal virtual Color GetUnderlineColorCore(PieceTable pieceTable) {
			return GetRun(pieceTable).UnderlineColor;
		}
		protected internal virtual Color GetStrikeoutColorCore(PieceTable pieceTable) {
			return GetRun(pieceTable).StrikeoutColor;
		}
		public virtual UnderlineType GetFontUnderlineType(PieceTable pieceTable) {
			return GetRun(pieceTable).FontUnderlineType;
		}
		public virtual StrikeoutType GetFontStrikeoutType(PieceTable pieceTable) {
			return GetRun(pieceTable).FontStrikeoutType;
		}		
		public virtual Color GetActualForeColor(PieceTable pieceTable, TextColors textColors, Color backColor) {
			TextRunBase run = GetRun(pieceTable);
			Color color = run.ForeColor;
			Color runBackColor = run.BackColor;
			if (!DXColor.IsTransparentOrEmpty(runBackColor))
				return AutoColorUtils.GetActualForeColor(runBackColor, color, textColors);
			else
				return AutoColorUtils.GetActualForeColor(backColor, color, textColors);
		}		
		public virtual Color GetActualUnderlineColor(PieceTable pieceTable, TextColors textColors, Color backColor) {
			Color underlineColor = GetUnderlineColorCore(pieceTable);
			if (underlineColor == DXColor.Empty) {
				return GetActualForeColor(pieceTable, textColors, backColor);
			}
			else
				return underlineColor;
		}
		public virtual Color GetActualStrikeoutColor(PieceTable pieceTable, TextColors textColors, Color backColor) {
			Color strikeoutColor = GetStrikeoutColorCore(pieceTable);
			if (strikeoutColor == DXColor.Empty)
				return GetActualForeColor(pieceTable, textColors, backColor);
			else
				return strikeoutColor;
		}
		public virtual void MoveVertically(int deltaY) {
			bounds.Y += deltaY;
		}
		public virtual void Accept(IRowBoxesVisitor visitor) {
		}
	}
	#endregion
	public abstract class SinglePositionBox : Box {
		FormatterPosition pos;
		public override FormatterPosition StartPos { get { return pos; } set { pos = value; } }
		public override FormatterPosition EndPos { get { return pos; } set { pos = value; } }
		public override string GetText(PieceTable table) {
			return table.GetTextFromSingleRun(pos, pos);
		}
		public override TextRunBase GetRun(PieceTable pieceTable) {
			return pieceTable.Runs[pos.RunIndex];
		}
		public override void OffsetRunIndices(int delta) {
			pos.OffsetRunIndex(delta);
		}
		public override FormatterPosition GetFirstFormatterPosition() {
			return pos;
		}
		public override FormatterPosition GetLastFormatterPosition() {
			return pos;
		}
		public override DocumentModelPosition GetFirstPosition(PieceTable pieceTable) {
			return GetDocumentPosition(pieceTable, pos);
		}
		public override DocumentModelPosition GetLastPosition(PieceTable pieceTable) {
			return GetDocumentPosition(pieceTable, pos);
		}
	}
	public abstract class NoPositionBox : Box {
		static readonly FormatterPosition pos = new FormatterPosition();
		public override FormatterPosition StartPos { get { return pos; } set { } }
		public override FormatterPosition EndPos { get { return pos; } set { } }
		public override void OffsetRunIndices(int delta) {
		}
	}
	public abstract class MultiPositionBox : Box {
		FormatterPosition startPos;
		FormatterPosition endPos;
		public override FormatterPosition StartPos { get { return startPos; } set { startPos = value; } }
		public override FormatterPosition EndPos { get { return endPos; } set { endPos = value; } }
		public override string GetText(PieceTable table) {
			return table.GetTextFromSingleRun(startPos, endPos);
		}
		public override TextRunBase GetRun(PieceTable pieceTable) {
			return pieceTable.Runs[startPos.RunIndex];
		}
		public override void OffsetRunIndices(int delta) {
			startPos.OffsetRunIndex(delta);
			endPos.OffsetRunIndex(delta);
		}
		public override FormatterPosition GetFirstFormatterPosition() {
			return startPos;
		}
		public override FormatterPosition GetLastFormatterPosition() {
			return endPos;
		}
		public override DocumentModelPosition GetFirstPosition(PieceTable pieceTable) {
			return GetDocumentPosition(pieceTable, startPos);
		}
		public override DocumentModelPosition GetLastPosition(PieceTable pieceTable) {
			return GetDocumentPosition(pieceTable, endPos);
		}
	}
	#region BoxCollectionBase<T> (abstract class)
	public abstract class BoxCollectionBase<T> : List<T> where T : Box {
		protected BoxCollectionBase() {
		}
		protected BoxCollectionBase(int capacity)
			: base(capacity) {
		}
		#region Properties
		#region First
		public T First {
			get {
				if (Count <= 0)
					return null;
				else
					return this[0];
			}
		}
		#endregion
		#region Last
		public T Last {
			get {
				if (Count <= 0)
					return null;
				else
					return this[Count - 1];
			}
		}
		#endregion
		#endregion
		protected internal abstract void RegisterSuccessfullItemHitTest(BoxHitTestCalculator calculator, T item);
		protected internal abstract void RegisterFailedItemHitTest(BoxHitTestCalculator calculator);
		public void ExportTo(IDocumentLayoutExporter exporter) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].ExportTo(exporter);
		}
		public int BinarySearchBoxIndex(PieceTable pieceTable, DocumentLogPosition logPosition) {
			return Algorithms.BinarySearch(this, new BoxAndLogPositionComparable<T>(pieceTable, logPosition));
		}
		public int BinarySearchBoxIndex(FormatterPosition pos) {
			return Algorithms.BinarySearch(this, new BoxAndFormatterPositionComparable<T>(pos));
		}
		public T BinarySearchBox(PieceTable pieceTable, DocumentLogPosition logPosition) {
			int index = BinarySearchBoxIndex(pieceTable, logPosition);
			if (index < 0)
				return null;
			else
				return this[index];
		}
		public void MoveVertically(int deltaY) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].MoveVertically(deltaY);
		}
	}
	#endregion
	#region BoxCollection
	public class BoxCollection : BoxCollectionBase<Box> {
		public BoxCollection() {
		}
		public BoxCollection(int capacity)
			: base(capacity) {
		}
		protected internal override void RegisterSuccessfullItemHitTest(BoxHitTestCalculator calculator, Box item) {
			calculator.Result.Box = item;
			calculator.Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Box);
		}
		protected internal override void RegisterFailedItemHitTest(BoxHitTestCalculator calculator) {
			calculator.Result.Box = null;
		}
	}
	#endregion
	public interface IRowBoxesVisitor {
		void Visit(SectionMarkBox box);
		void Visit(ParagraphMarkBox box);
		void Visit(LineBreakBox box);
		void Visit(PageBreakBox box);
		void Visit(ColumnBreakBox box);
		void Visit(SpaceBoxa box);
		void Visit(SingleSpaceBox box);
		void Visit(TabSpaceBox box);
		void Visit(DashBox box);
		void Visit(TextBox box);
		void Visit(InlinePictureBox box);
		void Visit(FloatingObjectAnchorBox box);
		void Visit(LayoutDependentTextBox box);
		void Visit(HyphenBox box);
	}
	#region BoxAndLogPositionComparable<T>
	public class BoxAndLogPositionComparable<T> : IComparable<T> where T : Box {
		#region Fields
		readonly PieceTable pieceTable;
		readonly DocumentLogPosition logPosition;
		#endregion
		public BoxAndLogPositionComparable(PieceTable pieceTable, DocumentLogPosition logPosition) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.logPosition = logPosition;
		}
		#region Properties
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		#endregion
		#region IComparable<T> Members
		public int CompareTo(T box) {
			DocumentModelPosition firstPos = box.GetFirstPosition(PieceTable);
			if (logPosition < firstPos.LogPosition)
				return 1;
			else if (logPosition > firstPos.LogPosition) {
				DocumentModelPosition lastPos = box.GetLastPosition(PieceTable);
				if (logPosition <= lastPos.LogPosition)
					return 0;
				else
					return -1;
			}
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region BoxStartAndLogPositionComparable<T>
	public class BoxStartAndLogPositionComparable<T> : IComparable<T> where T : Box {
		#region Fields
		readonly PieceTable pieceTable;
		readonly DocumentLogPosition logPosition;
		#endregion
		public BoxStartAndLogPositionComparable(PieceTable pieceTable, DocumentLogPosition logPosition) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.logPosition = logPosition;
		}
		#region Properties
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		#endregion
		#region IComparable<T> Members
		public int CompareTo(T box) {
			DocumentModelPosition firstPos = box.GetFirstPosition(PieceTable);
			return firstPos.LogPosition - logPosition;				
		}
		#endregion
	}
	#endregion
	#region ExactPageAndLogPositionComparable
	public class ExactPageAndLogPositionComparable : IComparable<Page> {
		#region Fields
		readonly PieceTable pieceTable;
		readonly DocumentLogPosition logPosition;
		#endregion
		public ExactPageAndLogPositionComparable(PieceTable pieceTable, DocumentLogPosition logPosition) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.logPosition = logPosition;
		}
		#region Properties
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		#endregion
		#region IComparable<Page> Members
		public int CompareTo(Page page) {
			DocumentModelPosition firstPos = page.GetFirstPosition(PieceTable);
			if (firstPos.LogPosition > logPosition)
				return 1;
			if (firstPos.LogPosition == logPosition)
				return 0;
			ExactPageAreaAndLogPositionComparable areaComparable = new ExactPageAreaAndLogPositionComparable(pieceTable, logPosition);			
			int firstAreaCompareResult = areaComparable.CompareTo(page.Areas[0]);
			if (page.Areas.Count == 1 || firstAreaCompareResult >= 0)
				return firstAreaCompareResult;
			int lastAreaCompareResult = areaComparable.CompareTo(page.Areas.Last);
			if (lastAreaCompareResult >= 0)
				return 0;
			else
				return -1;
		}
		#endregion
	}
	#endregion
	#region ExactPageAreaAndLogPositionComparable
	public class ExactPageAreaAndLogPositionComparable : IComparable<PageArea> {
		#region Fields
		readonly PieceTable pieceTable;
		readonly DocumentLogPosition logPosition;
		#endregion
		public ExactPageAreaAndLogPositionComparable(PieceTable pieceTable, DocumentLogPosition logPosition) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.logPosition = logPosition;
		}
		#region Properties
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		#endregion
		#region IComparable<Page> Members
		public int CompareTo(PageArea area) {
			DocumentModelPosition firstPos = area.GetFirstPosition(PieceTable);
			if (firstPos.LogPosition > logPosition)
				return 1;
			if (firstPos.LogPosition == logPosition)
				return 0;
			ExactColumnAndLogPositionComparable columnComparable = new ExactColumnAndLogPositionComparable(pieceTable, logPosition);
			int firstBoxCompareResult = columnComparable.CompareTo(area.Columns[0]);
			if (area.Columns.Count == 1 || firstBoxCompareResult >= 0)
				return firstBoxCompareResult;
			int lastBoxCompareResult = columnComparable.CompareTo(area.Columns.Last);
			if (lastBoxCompareResult >= 0)
				return 0;
			else
				return -1;
		}
		#endregion
	}
	#endregion
	#region ExactColumnAndLogPositionComparable
	public class ExactColumnAndLogPositionComparable : IComparable<Column> {
		#region Fields
		readonly PieceTable pieceTable;
		readonly DocumentLogPosition logPosition;		
		#endregion
		public ExactColumnAndLogPositionComparable(PieceTable pieceTable, DocumentLogPosition logPosition) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.logPosition = logPosition;			
		}
		#region Properties
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		#endregion
		#region IComparable<Page> Members
		public int CompareTo(Column column) {
			DocumentModelPosition firstPos = column.GetFirstPosition(PieceTable);
			if (firstPos.LogPosition > logPosition)
				return 1;
			if (firstPos.LogPosition == logPosition)
				return 0;
			TableViewInfoCollection tables = column.InnerTables;
			if (tables != null && tables.Count > 0) {
				ParagraphIndex paragraphIndex = PieceTable.FindParagraphIndex(logPosition);
				Paragraph paragraph = PieceTable.Paragraphs[paragraphIndex];
				TableCell cell = paragraph.GetCell();
				while (cell != null) {
					TableCellViewInfo tableCellViewInfo;
					TableViewInfo tableViewInfo = FindTable(tables, cell, out tableCellViewInfo);
					if (tableViewInfo != null) {
						if (tableViewInfo.SingleColumn)
							return 0;
						if (tableCellViewInfo != null) {
							if (tableCellViewInfo.SingleColumn)
								return 0;
							Row firstRow = tableCellViewInfo.GetFirstRow(column);
							if (firstRow == null)
								return 1;
							DocumentLogPosition firstPosition = firstRow.GetFirstPosition(PieceTable).LogPosition;
							if (firstPosition > logPosition)
								return 1;
							Row lastRow = tableCellViewInfo.GetLastRow(column);
							if (lastRow != null) {
								DocumentLogPosition lastPosition = lastRow.GetLastPosition(PieceTable).LogPosition;
								if (lastPosition < logPosition)
									return -1;
								else
									return 0;
							}
							else
								return 1;
						}
						else
							return tableViewInfo.TopRowIndex - cell.Table.Rows.IndexOf(cell.Row); 
					}
					cell = cell.Table.ParentCell;
				}				
			}
			DocumentModelPosition lastPos = column.GetLastPosition(PieceTable);
			if (lastPos.LogPosition >= logPosition)
				return 0;
			else
				return -1;
		}
		#endregion
		TableViewInfo FindTable(TableViewInfoCollection tables, TableCell cell, out TableCellViewInfo cellViewInfo) {
			TableViewInfo result = null;
			for (int tableIndex = 0; tableIndex < tables.Count; tableIndex++) {
				TableViewInfo tableViewInfo = tables[tableIndex];
				if (tableViewInfo.Table == cell.Table)
					result = tableViewInfo;
				for (int cellIndex = 0; cellIndex < tableViewInfo.Cells.Count; cellIndex++) {
					if (tableViewInfo.Cells[cellIndex].Cell == cell) {
						cellViewInfo = tableViewInfo.Cells[cellIndex];
						return result;
					}
				}
			}
			cellViewInfo = null;
			return result;
		}
	}
	#endregion
	#region BoxAndFormatterPositionComparable<T>
	public class BoxAndFormatterPositionComparable<T> : IComparable<T> where T : Box {
		readonly FormatterPosition formatterPosition;
		public BoxAndFormatterPositionComparable(FormatterPosition formatterPosition) {
			this.formatterPosition = formatterPosition;
		}
		public FormatterPosition FormatterPosition { get { return formatterPosition; } }
		#region IComparable<T> Members
		public int CompareTo(T box) {
			FormatterPosition firstPos = box.GetFirstFormatterPosition();
			if (formatterPosition < firstPos)
				return 1;
			else if (formatterPosition > firstPos) {
				FormatterPosition lastPos = box.GetLastFormatterPosition();
				if (formatterPosition <= lastPos)
					return 0;
				else
					return -1;
			}
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region BoxAndPointXComparable<T>
	public class BoxAndPointXComparable<T> : IComparable<T> where T : Box {
		readonly int x;
		public BoxAndPointXComparable(Point point) {
			this.x = point.X;
		}
		public int X { get { return x; } }
		#region IComparable<T> Members
		public int CompareTo(T box) {
			Rectangle bounds = box.Bounds;
			if (x < bounds.X)
				return 1;
			else if (x >= bounds.Right)
				return -1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region BoxAndPointYComparable<T>
	public class BoxAndPointYComparable<T> : IComparable<T> where T : Box {
		readonly int y;
		public BoxAndPointYComparable(Point point) {
			this.y = point.Y;
		}
		public int Y { get { return y; } }
		#region IComparable<T> Members
		public int CompareTo(T box) {
			Rectangle bounds = box.Bounds;
			if (y < bounds.Y)
				return 1;
			else if (y >= bounds.Bottom)
				return -1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
}

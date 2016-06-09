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
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	#region IBoxHitTestCalculator
	public interface IBoxHitTestCalculator {
		BoxHitTestManager CreatePageHitTestManager(Page page); 
		BoxHitTestManager CreatePageAreaHitTestManager(PageArea pageArea); 
		BoxHitTestManager CreateColumnHitTestManager(Column column); 
		BoxHitTestManager CreateRowHitTestManager(Row row); 
		BoxHitTestManager CreateTextBoxHitTestManager(DevExpress.XtraRichEdit.Layout.TextBox box); 
		BoxHitTestManager CreateCharacterBoxHitTestManager(CharacterBox box); 
		BoxHitTestManager CreateHyphenBoxHitTestManager(HyphenBox box); 
		BoxHitTestManager CreateInlinePictureBoxHitTestManager(InlinePictureBox box); 
		BoxHitTestManager CreateCustomRunBoxHitTestManager(CustomRunBox box); 
		BoxHitTestManager CreateSeparatorMarkBoxHitTestManager(SeparatorBox box); 
		BoxHitTestManager CreateSpaceBoxHitTestManager(ISpaceBox box); 
		BoxHitTestManager CreateTabSpaceBoxHitTestManager(TabSpaceBox box); 
		BoxHitTestManager CreateNumberingListBoxHitTestManager(NumberingListBox box); 
		BoxHitTestManager CreateLineBreakBoxHitTestManager(LineBreakBox box); 
		BoxHitTestManager CreatePageBreakBoxHitTestManager(PageBreakBox box); 
		BoxHitTestManager CreateColumnBreakBoxHitTestManager(ColumnBreakBox box);
		BoxHitTestManager CreateParagraphMarkBoxHitTestManager(ParagraphMarkBox box); 
		BoxHitTestManager CreateSectionMarkBoxHitTestManager(SectionMarkBox box); 
		BoxHitTestManager CreateDataContainerRunBoxHitTestManager(DataContainerRunBox box); 
		void ProcessPageCollection(PageCollection collection);
		void ProcessPageAreaCollection(PageAreaCollection collection);
		void ProcessColumnCollection(ColumnCollection collection);
		void ProcessRowCollection(RowCollection collection);
		void ProcessBoxCollection(BoxCollection collection);
		void ProcessCharacterBoxCollection(CharacterBoxCollection collection);
	}
	#endregion
	#region BoxHitTestManager
	public class BoxHitTestManager {
		#region Fields
		readonly BoxHitTestCalculator calculator;
		readonly Box box;
		#endregion
		public BoxHitTestManager(BoxHitTestCalculator calculator, Box box) {
			Guard.ArgumentNotNull(calculator, "calculator");
			Guard.ArgumentNotNull(box, "box");
			this.calculator = calculator;
			this.box = box;
		}
		#region Properties
		protected IBoxHitTestCalculator ICalculator { get { return Calculator; } }
		public BoxHitTestCalculator Calculator { get { return calculator; } }
		public RichEditHitTestRequest Request { get { return Calculator.Request; } }
		public RichEditHitTestResult Result { get { return Calculator.Result; } }
		public Box Box { get { return box; } }
		#endregion
		protected internal virtual void CalcHitTest(bool forceStrictHitTest) {
			bool strictHitTest = ((Request.Accuracy & Box.HitTestAccuracy) != 0) || forceStrictHitTest;
			bool strictMatch = Box.Bounds.Contains(Request.LogicalPoint);
			if(!strictHitTest && Request.DetailsLevel >= Box.DetailsLevel) {
				RegisterSuccessfullHitTest(strictMatch);
				if(Request.DetailsLevel > Box.DetailsLevel)
					CalcHitTestAmongNestedBoxes();
				return;
			}
			if(strictMatch) {
				RegisterSuccessfullHitTest(strictMatch);
				if(Request.DetailsLevel > Box.DetailsLevel)
					CalcHitTestAmongNestedBoxes();
			}
			else
				RegisterFailedHitTest();
		}
		public virtual void CalcHitTestAmongNestedBoxes() {
		}
		public virtual void RegisterSuccessfullHitTest(bool strictMatch) {
			Result.Box = Box;
			Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Box);
			if (strictMatch)
				Result.Accuracy |= HitTestAccuracy.ExactBox;
		}
		public virtual void RegisterFailedHitTest() {
			Result.Box = null;
		}
	}
	#endregion
	#region PageHitTestManager
	public class PageHitTestManager : BoxHitTestManager {
		public PageHitTestManager(BoxHitTestCalculator calculator, Page box)
			: base(calculator, box) {
		}
		public Page Page { get { return (Page)Box; } }
		public override void CalcHitTestAmongNestedBoxes() {
			ICalculator.ProcessPageAreaCollection(Page.Areas);
		}
		public override void RegisterSuccessfullHitTest(bool strictMatch) {
			Result.Page = Page;
			Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Page);
			if (strictMatch)
				Result.Accuracy |= HitTestAccuracy.ExactPage;
		}
		public override void RegisterFailedHitTest() {
			Result.Page = null;
		}
	}
	#endregion
	#region PageAreaHitTestManager
	public class PageAreaHitTestManager : BoxHitTestManager {
		public enum PageAreaKind { 
			Autodetect = 0,
			Main,
			Header,
			Footer,
			TextBox,
			Comment
		};
		PageAreaKind AutodetectPageAreaKind() {
			if(Request.PieceTable.IsMain)
				return PageAreaKind.Main;
			if(Request.PieceTable.IsHeader)
				return PageAreaKind.Header;
			if(Request.PieceTable.IsFooter)
				return PageAreaKind.Footer;
			if(Request.PieceTable.IsTextBox)
				return PageAreaKind.TextBox;
			if (Request.PieceTable.IsComment)
				return PageAreaKind.Comment;
			throw new InvalidOperationException();
		}
		PageAreaKind pageAreaKind;
		public PageAreaHitTestManager(BoxHitTestCalculator calculator, PageArea box)
			: base(calculator, box) {
		}
		public PageArea PageArea { get { return (PageArea)Box; } }
		public PageAreaKind AreaKind { get { return pageAreaKind == PageAreaHitTestManager.PageAreaKind.Autodetect ? AutodetectPageAreaKind() : pageAreaKind; } set { pageAreaKind = value; } }
		public override void CalcHitTestAmongNestedBoxes() {
			ICalculator.ProcessColumnCollection(PageArea.Columns);
		}
		public override void RegisterSuccessfullHitTest(bool strictMatch) {
			Result.PageArea = PageArea;
			Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.PageArea);
			if (strictMatch)
				Result.Accuracy |= HitTestAccuracy.ExactPageArea;
		}
		public override void RegisterFailedHitTest() {
			Result.PageArea = null;
		}
		protected internal override void CalcHitTest(bool forceStrictHitTest) {
			if (!forceStrictHitTest) {
				if (!Request.IgnoreInvalidAreas && ! ShouldIgnoreFailedHitTest()) {
					switch (AreaKind) {
						case PageAreaKind.Main:
							if (((Result.Page.Header != null) && (Request.LogicalPoint.Y < PageArea.Bounds.Top)) || ((Result.Page.Footer != null) && (Request.LogicalPoint.Y > PageArea.Bounds.Bottom))) {
								RegisterFailedHitTest();
								return;
							}
							break;
						case PageAreaKind.Header:
							if (Request.LogicalPoint.Y > PageArea.Bounds.Bottom) {
								RegisterFailedHitTest();
								return;
							}
							break;
						case PageAreaKind.Footer:
							if (Request.LogicalPoint.Y < PageArea.Bounds.Top) {
								RegisterFailedHitTest();
								return;
							}
							break;
						case PageAreaKind.TextBox:
							if (!PageArea.Bounds.Contains(Request.LogicalPoint)) {
								RegisterFailedHitTest();
								return;
							}
							break;
					}
				}
				RegisterSuccessfullHitTest(true);
			}
			base.CalcHitTest(forceStrictHitTest);
		}
		bool ShouldIgnoreFailedHitTest() {
			FloatingObjectBox floatingObjectBox = Result.FloatingObjectBox;
			if(floatingObjectBox == null)
				return false;
			return floatingObjectBox.GetFloatingObjectRun().PieceTable == Request.PieceTable;			
		}
	}
	#endregion
	#region ColumnHitTestManager
	public class ColumnHitTestManager : BoxHitTestManager {
		public ColumnHitTestManager(BoxHitTestCalculator calculator, Column box)
			: base(calculator, box) {
		}
		public Column Column { get { return (Column)Box; } }
		public override void CalcHitTestAmongNestedBoxes() {
			TableViewInfo table = GetActiveTableViewInfo();
			if (table == null) {
				RowCollection rows = Column.GetOwnRows();
				int rowIndex = Calculator.FastHitTestRowIndex(rows);
				if (rowIndex >= 0) {
					Row row = rows[rowIndex];
					if (row.Bounds.Contains(Request.LogicalPoint) || !row.IsTabelCellRow)
						Calculator.CalcHitTest(row);
					else {
						TableCellRow tableCellRow = (TableCellRow)row;
						table = GetTable(tableCellRow);
						if (!CalcHitTestAmongNestedBoxesCore(table, false))
							Calculator.CalcHitTest(row);
					}
				}
				else
					rows.RegisterFailedItemHitTest(Calculator);
			}
			else
				CalcHitTestAmongNestedBoxesCore(table, true);
		}
		TableViewInfo GetTable(TableCellRow tableCellRow) {
			TableViewInfo table = tableCellRow.CellViewInfo.TableViewInfo;
			while (table.ParentTableCellViewInfo != null)
				table = table.ParentTableCellViewInfo.TableViewInfo;
			return table;
		}
		protected internal bool CalcHitTestAmongNestedBoxesCore(TableViewInfo table, bool strictTableMatch) {
			bool strictTableRowMatch = (Request.Accuracy & HitTestAccuracy.ExactTableRow) != 0;
			bool strictTableCellMatch = (Request.Accuracy & HitTestAccuracy.ExactTableCell) != 0;
			return CalcHitTestAmongNestedBoxesCore(table, strictTableMatch, strictTableRowMatch, strictTableCellMatch);
		}
		protected internal bool CalcHitTestAmongNestedBoxesCore(TableViewInfo table, bool strictTableMatch, bool strictTableRowMatch, bool strictTableCellMatch) {
			if (table.RowCount == 0)
				return false;
			Point logicalPoint = Request.LogicalPoint;
			int tableRowIndex = Calculator.FastHitTestIndexCore(table.Rows, new TableRowAnchorComparable(logicalPoint.Y, table.Table.Rows.Last), strictTableRowMatch);
			if (tableRowIndex < 0) {
				Result.TableRow = null;
				return false;
			}
			TableRowViewInfoBase tableRow = table.Rows[tableRowIndex];
			int tableCellIndex = Calculator.FastHitTestIndexCore(tableRow.Cells, new TableCellAnchorComparable(logicalPoint.X), strictTableCellMatch);
			if (tableCellIndex < 0) {
				Result.TableCell = null;
				return false;
			}
			TableCellViewInfo tableCell = tableRow.Cells[tableCellIndex];
			if (tableCell.Left <= logicalPoint.X) {
				TableViewInfoCollection innerTables = tableCell.InnerTables;
				for (int i = 0; i < innerTables.Count; i++) {
					TableViewInfo currentTable = innerTables[i];
					if (CalcHitTestAmongNestedBoxesCore(currentTable, strictTableMatch, true, true))
						return true;
				}
			}
			Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.TableRow);
			if (strictTableMatch)
				Result.Accuracy |= HitTestAccuracy.ExactTableRow;
			Result.TableRow = tableRow;
			Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.TableCell);
			if (strictTableMatch)
				Result.Accuracy |= HitTestAccuracy.ExactTableCell;
			Result.TableCell = tableCell;
			RowCollection rows = tableCell.GetRows(Column);
			ICalculator.ProcessRowCollection(rows);
			return true;
		}
		protected internal virtual TableViewInfo GetActiveTableViewInfo() {
			TableViewInfoCollection tables = Column.InnerTables;
			if (tables == null || tables.Count == 0)
				return null;
			int logicalY = Request.LogicalPoint.Y;
			for (int i = 0; i < tables.Count; i++) {
				TableViewInfo currentTable = tables[i];
				int actualTableBottom = currentTable.GetActualBottomPosition();
				if (currentTable.Anchors.First.VerticalPosition <= logicalY && actualTableBottom >= logicalY)
					return currentTable;
			}			
			return null;
		}
		public override void RegisterSuccessfullHitTest(bool strictMatch) {
			Result.Column = Column;
			Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Column);
			if (strictMatch)
				Result.Accuracy |= HitTestAccuracy.ExactColumn;
		}
		public override void RegisterFailedHitTest() {
			Result.Column = null;
		}
	}
	#endregion
	#region RowHitTestManager
	public class RowHitTestManager : BoxHitTestManager {
		public RowHitTestManager(BoxHitTestCalculator calculator, Row box)
			: base(calculator, box) {
		}
		public Row Row { get { return (Row)Box; } }
		public override void CalcHitTestAmongNestedBoxes() {
			ICalculator.ProcessBoxCollection(Row.Boxes);
		}
		public override void RegisterSuccessfullHitTest(bool strictMatch) {
			Result.Row = Row;
			Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Row);
			if (strictMatch)
				Result.Accuracy |= HitTestAccuracy.ExactRow;
		}
		public override void RegisterFailedHitTest() {
			Result.Row = null;
		}
	}
	#endregion
	#region CharacterBoxHitTestManager
	public class CharacterBoxHitTestManager : BoxHitTestManager {
		public CharacterBoxHitTestManager(BoxHitTestCalculator calculator, CharacterBox box)
			: base(calculator, box) {
		}
		public CharacterBox Character { get { return (CharacterBox)Box; } }
		public override void CalcHitTestAmongNestedBoxes() {
		}
		public override void RegisterSuccessfullHitTest(bool strictMatch) {
			Result.Character = Character;
			Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Character);
			if (strictMatch)
				Result.Accuracy |= HitTestAccuracy.ExactCharacter;
		}
		public override void RegisterFailedHitTest() {
			Result.Character = null;
		}
	}
	#endregion
	#region EmptyHitTestManager
	public class EmptyHitTestManager : BoxHitTestManager {
		public EmptyHitTestManager(BoxHitTestCalculator calculator, Box box)
			: base(calculator, box) {
		}
		public override void CalcHitTestAmongNestedBoxes() {
		}
		public override void RegisterSuccessfullHitTest(bool strictMatch) {
		}
		public override void RegisterFailedHitTest() {
		}
	}
	#endregion
	#region BoxHitTestCalculator (abstract class)
	public abstract class BoxHitTestCalculator : IBoxHitTestCalculator {
		#region Fields
		readonly RichEditHitTestRequest request;
		readonly RichEditHitTestResult result;
		#endregion
		protected BoxHitTestCalculator(RichEditHitTestRequest request, RichEditHitTestResult result) {
			Guard.ArgumentNotNull(request, "request");
			Guard.ArgumentNotNull(result, "result");
			this.request = request;
			this.result = result;
			result.LogicalPoint = request.LogicalPoint;
		}
		#region Properties
		public RichEditHitTestRequest Request { get { return request; } }
		public RichEditHitTestResult Result { get { return result; } }
		#endregion
		public void CalcHitTest(Box box) {
			CalcHitTest(box, false);
		}
		public void CalcHitTest(Box box, bool forceStrictHitTest) {
			BoxHitTestManager manager = box.CreateHitTestManager(this);
			manager.CalcHitTest(forceStrictHitTest);
		}
		protected internal int FastHitTestIndexCore<T>(IList<T> collection, IComparable<T> predicate, bool strictHitTest)  {
			if (collection.Count <= 0)
				return -1;
			int index = Algorithms.BinarySearch(collection, predicate);
			if (index >= 0)
				return index;
			if (strictHitTest)
				return -1;
			if (index == ~collection.Count) {
				if (predicate.CompareTo(collection[0]) > 0)
					return 0;
				else
					return collection.Count - 1;
			}
			else
				return ~index;
		}
		protected internal int FastHitTestCore<T>(BoxCollectionBase<T> collection, IComparable<T> predicate, bool strictHitTest) where T : Box {
			int index = FastHitTestIndexCore(collection, predicate, strictHitTest);
			if (index >= 0) {
				CalcHitTest(collection[index]);
				collection.RegisterSuccessfullItemHitTest(this, collection[index]);
			}
			else
				collection.RegisterFailedItemHitTest(this);
			return index;
		}
		protected internal void FastHitTestCharacter(CharacterBoxCollection collection, bool strictHitTest) {
			int characterIndex = FastHitTestCore(collection, new BoxAndPointXComparable<CharacterBox>(Request.LogicalPoint), strictHitTest);
			if (!strictHitTest && Result.Character != null && characterIndex >= 0) {
				Rectangle bounds = Result.Character.Bounds;
				Point point = Request.LogicalPoint;
				if (bounds.Left <= point.X && point.X <= bounds.Right) {
					int leftDistance = point.X - bounds.Left;
					int rightDistance = bounds.Right - point.X;
					if (leftDistance >= rightDistance) {
						if (characterIndex + 1 < collection.Count) {
							Result.Character = collection[characterIndex + 1];
							Result.Accuracy &= (~HitTestAccuracy.ExactCharacter);
						}
					}
				}
			}
		}
		protected internal int FastHitTestRowIndex(RowCollection rows) {
			bool strictHitTest = ((Request.Accuracy & HitTestAccuracy.ExactRow) != 0);
			return FastHitTestIndexCore(rows, new BoxAndPointYComparable<Row>(Request.LogicalPoint), strictHitTest);
		}
		protected internal void FastHitTestAssumingArrangedHorizontally<T>(BoxCollectionBase<T> collection, bool strictHitTest) where T : Box {
			FastHitTestCore(collection, new BoxAndPointXComparable<T>(Request.LogicalPoint), strictHitTest);
		}
		protected internal void FastHitTestAssumingArrangedVertically<T>(BoxCollectionBase<T> collection, bool strictHitTest) where T : Box {
			FastHitTestCore(collection, new BoxAndPointYComparable<T>(Request.LogicalPoint), strictHitTest);
		}
		#region IBoxHitTestCalculator Members
		public virtual void ProcessPageCollection(PageCollection collection) {
		}
		public virtual void ProcessPageAreaCollection(PageAreaCollection collection) {
			bool strictHitTest = ((Request.Accuracy & HitTestAccuracy.ExactPageArea) != 0);
			FastHitTestAssumingArrangedVertically(collection, strictHitTest);
		}
		public virtual void ProcessColumnCollection(ColumnCollection collection) {
			bool strictHitTest = ((Request.Accuracy & HitTestAccuracy.ExactColumn) != 0);
			FastHitTestAssumingArrangedHorizontally(collection, strictHitTest);
		}
		public virtual void ProcessRowCollection(RowCollection collection) {
			bool strictHitTest = ((Request.Accuracy & HitTestAccuracy.ExactRow) != 0);
			FastHitTestAssumingArrangedVertically(collection, strictHitTest);
		}
		public virtual void ProcessBoxCollection(BoxCollection collection) {
			bool strictHitTest = ((Request.Accuracy & HitTestAccuracy.ExactBox) != 0);
			FastHitTestAssumingArrangedHorizontally(collection, strictHitTest);
		}
		public virtual void ProcessCharacterBoxCollection(CharacterBoxCollection collection) {
			bool strictHitTest = ((Request.Accuracy & HitTestAccuracy.ExactCharacter) != 0);
			FastHitTestAssumingArrangedHorizontally(collection, strictHitTest);
		}
		#endregion
		#region IBoxHitTestCalculator Members
		public virtual BoxHitTestManager CreatePageHitTestManager(Page page) {
			return new PageHitTestManager(this, page);
		}
		public virtual BoxHitTestManager CreatePageAreaHitTestManager(PageArea pageArea) {
			return new PageAreaHitTestManager(this, pageArea);
		}
		public virtual BoxHitTestManager CreateColumnHitTestManager(Column column) {
			return new ColumnHitTestManager(this, column);
		}
		public virtual BoxHitTestManager CreateRowHitTestManager(Row row) {
			return new RowHitTestManager(this, row);
		}
		public BoxHitTestManager CreateTextBoxHitTestManager(TextBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateCharacterBoxHitTestManager(CharacterBox box) {
			return new CharacterBoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateHyphenBoxHitTestManager(HyphenBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateInlinePictureBoxHitTestManager(InlinePictureBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateSpaceBoxHitTestManager(ISpaceBox box) {
			return new BoxHitTestManager(this, box.Box);
		}
		public BoxHitTestManager CreateTabSpaceBoxHitTestManager(TabSpaceBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateNumberingListBoxHitTestManager(NumberingListBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateLineBreakBoxHitTestManager(LineBreakBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreatePageBreakBoxHitTestManager(PageBreakBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateColumnBreakBoxHitTestManager(ColumnBreakBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateParagraphMarkBoxHitTestManager(ParagraphMarkBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateSectionMarkBoxHitTestManager(SectionMarkBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateCustomRunBoxHitTestManager(CustomRunBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateSeparatorMarkBoxHitTestManager(SeparatorBox box) {
			return new BoxHitTestManager(this, box);
		}
		public BoxHitTestManager CreateDataContainerRunBoxHitTestManager(DataContainerRunBox box) {
			return new BoxHitTestManager(this, box);
		}
		#endregion
	}
	#endregion
	#region TableRowAnchorComparable
	class TableRowAnchorComparable : IComparable<TableRowViewInfoBase> {
		int pos;
		TableRow lastRow;
		public TableRowAnchorComparable(int pos, TableRow lastRow) {
			this.pos = pos;
			this.lastRow = lastRow;
		}		
		#region IComparable<TableRowViewInfoBase> Members
		public int CompareTo(TableRowViewInfoBase row) {
			if (pos < row.TopAnchor.VerticalPosition)
				return 1;
			int actualRowBottom = row.BottomAnchor.VerticalPosition;
			if (row.Row == lastRow)
				actualRowBottom += row.BottomAnchor.BottomTextIndent;
			if (pos > actualRowBottom)
				return -1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region TableCellAnchorComparable
	class TableCellAnchorComparable : IComparable<TableCellViewInfo> {
		int pos;
		public TableCellAnchorComparable(int pos) {
			this.pos = pos;
		}
		#region IComparable<RulerTickmark> Members
		public int CompareTo(TableCellViewInfo cell) {
			if (pos < cell.Left)
				return 1;
			else if (pos > cell.Left + cell.Width)
				return -1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
}

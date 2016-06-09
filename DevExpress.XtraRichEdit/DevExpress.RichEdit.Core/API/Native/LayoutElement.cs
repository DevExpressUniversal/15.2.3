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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using System.Reflection;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Export;
using ModelDocumentLayout = DevExpress.XtraRichEdit.Layout.DocumentLayout;
using ModelBox = DevExpress.XtraRichEdit.Layout.Box;
using ModelRowExtendedBoxes = DevExpress.XtraRichEdit.Layout.RowExtendedBoxes;
using ModelInlinePictureBox = DevExpress.XtraRichEdit.Layout.InlinePictureBox;
using ModelFloatingObjectAnchorBox = DevExpress.XtraRichEdit.Layout.FloatingObjectAnchorBox;
using ModelLineNumberBox = DevExpress.XtraRichEdit.Layout.LineNumberBox;
using ModelLineNumberBoxCollection = DevExpress.XtraRichEdit.Layout.LineNumberBoxCollection;
using PieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
using Paragraph = DevExpress.XtraRichEdit.Model.Paragraph;
using DocumentModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
using BorderBase = DevExpress.XtraRichEdit.Model.BorderBase;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using System.Collections;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using ModelUnderlineBox = DevExpress.XtraRichEdit.Layout.UnderlineBox;
using ModelHiddenTextUnderlineBox = DevExpress.XtraRichEdit.Layout.HiddenTextUnderlineBox;
using ModelErrorBox = DevExpress.XtraRichEdit.Layout.ErrorBox;
using ModelCustomMarkBox = DevExpress.XtraRichEdit.Layout.CustomMarkBox;
using ModelCharacterBox = DevExpress.XtraRichEdit.Layout.CharacterBox;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraRichEdit.API.Layout {
	#region LayoutType
	public enum LayoutType {
		Page = 0,
		PageArea = 1,
		Column = 2,
		Comment = 3,
		Header = 4,
		Footer = 5,
		TextBox = 6, 
		FloatingPicture = 7,
		Table = 8,
		TableRow = 9,
		TableCell = 10,
		Row = 11,
		PlainTextBox = 12, 
		SpecialTextBox = 13,
		InlinePictureBox = 14,
		FloatingObjectAnchorBox = 15,
		SpaceBox = 16,
		ParagraphMarkBox = 17,
		SectionBreakBox = 18,
		LineBreakBox = 19,
		PageBreakBox = 20,
		ColumnBreakBox = 21,
		HyphenBox = 22,
		TabSpaceBox = 23,
		NumberingListMarkBox = 24,
		NumberingListWithSeparatorBox = 25,
		UnderlineBox = 26,
		StrikeoutBox = 27,
		HighlightAreaBox = 28,
		FieldHighlightAreaBox = 29,
		RangePermissionHighlightAreaBox = 30,
		BookmarkStartBox = 31,
		BookmarkEndBox = 32,
		RangePermissionStartBox = 33,
		RangePermissionEndBox = 34,
		CommentHighlightAreaBox = 35,
		CommentStartBox = 36,
		CommentEndBox = 37,
		HiddenTextUnderlineBox = 38,
		LineNumberBox = 39,
		PageNumberBox = 40,
		CustomRunBox = 41,
		DataContainerRunBox = 42,
		CharacterBox = 43
	}
	#endregion
	#region LayoutElement
	public interface LayoutElement {
		LayoutType Type { get; }
		Rectangle Bounds { get; }
		LayoutElement Parent { get; }
		void Accept(LayoutVisitor visitor);
		Rectangle GetRelativeBounds(LayoutElement element);
		T GetParentByType<T>();
	}
	#endregion
	public interface RangedLayoutElement : LayoutElement {
		FixedRange Range { get; }
	}
	interface IPieceTableContainer {
		PieceTable PieceTable { get; }
	}
	public struct Borders {
		LayoutBorder left;
		LayoutBorder right;
		LayoutBorder top;
		LayoutBorder bottom;
		internal Borders(LayoutBorder left, LayoutBorder right, LayoutBorder top, LayoutBorder bottom) {
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}
		public LayoutBorder Left { get { return left; } }
		public LayoutBorder Right { get { return right; } }
		public LayoutBorder Top { get { return top; } }
		public LayoutBorder Bottom { get { return bottom; } }
	}
	public struct LayoutBorder {
		Color color;
		TableBorderLineStyle style;
		int thickness;
		internal LayoutBorder(Color color, TableBorderLineStyle style, int thickness) {
			this.color = color;
			this.style = style;
			this.thickness = thickness;
		}
		public int Thickness { get { return thickness; } }
		public Color Color { get { return color; } }
		public TableBorderLineStyle Style { get { return style; } }
	}
	#region LayoutElementBase
	public abstract class LayoutElementBase : LayoutElement {
		LayoutElement parent;
		Rectangle bounds = Rectangle.Empty;
		protected LayoutElementBase(LayoutElement parent) {
			this.parent = parent;
		}
		public abstract LayoutType Type { get; }
		public Rectangle Bounds {
			get {
				if (bounds == Rectangle.Empty)
					bounds = GetBounds();
				return bounds;
			}
		}
		public LayoutElement Parent { get { return parent; } }
		internal void SetBounds(Rectangle bounds) {
			this.bounds = bounds;
		}
		protected abstract Rectangle GetBounds();
		public abstract void Accept(LayoutVisitor visitor);
		public T GetParentByType<T>() {
			return GetParentByTypeCore<T>(parent);
		}
		T GetParentByTypeCore<T>(LayoutElement parent) {
			if (parent == null)
				return default(T);
			Type type = parent.GetType();
			Type resultType = typeof(T);
#if DXPORTABLE
				foreach (Type currentType in type.GetInterfaces()) {
					if (currentType.Name == resultType.Name) {
						return (T)parent;
					}
				}
#else
			if (resultType.IsInterface && type.GetInterface(resultType.Name) != null)
				return (T)parent;
#endif
			if (type == resultType)
				return (T)parent;
			return GetParentByTypeCore<T>(parent.Parent);
		}
		public Rectangle GetRelativeBounds(LayoutElement element) {
			Rectangle result = new Rectangle();
			result.Height = Bounds.Height;
			result.Width = Bounds.Width;
			result.X = Bounds.X - element.Bounds.X;
			result.Y = Bounds.Y - element.Bounds.Y;
			return result;
		}
	}
	#endregion
	#region RangedLayoutElement
	public abstract class RangedLayoutElementBase : LayoutElementBase, RangedLayoutElement {
		FixedRange range;
		protected RangedLayoutElementBase(LayoutElement parent)
			: base(parent) {
		}
		internal abstract FixedRange CreateRange();
		public FixedRange Range {
			get {
				if (range == null)
					range = CreateRange();
				return range;
			}
		}
	}
	#endregion
	#region Box
	public abstract class Box : RangedLayoutElementBase {
		ModelBox box;
		LayoutType type;
		internal Box(ModelBox box, LayoutType type, LayoutElement parent)
			: base(parent) {
			this.box = box;
			this.type = type;
		}
		internal ModelBox ModelBox { get { return box; } }
		public override LayoutType Type { get { return type; } }
		protected override Rectangle GetBounds() {
			return ModelBox.Bounds;
		}
		internal override FixedRange CreateRange() {
			IPieceTableContainer container = GetParentByType<IPieceTableContainer>();
			DocumentModelPosition startPosition = DocumentModelPosition.FromRunStart(container.PieceTable, box.StartPos.RunIndex);
			DocumentModelPosition endPosition = DocumentModelPosition.FromRunStart(container.PieceTable, box.EndPos.RunIndex);
			int start = ((IConvertToInt<DevExpress.XtraRichEdit.Model.DocumentLogPosition>)startPosition.LogPosition).ToInt() + box.StartPos.Offset;
			int end = ((IConvertToInt<DevExpress.XtraRichEdit.Model.DocumentLogPosition>)endPosition.LogPosition).ToInt() + box.EndPos.Offset;
			return new FixedRange(start, end - start + 1);
		}
	}
	#endregion
	#region PlainTextBox
	public class PlainTextBox : Box {
		string text = null;
		internal PlainTextBox(ModelBox box, LayoutType type, LayoutElement parent)
			: base(box, type, parent) {
		}
		public string Text {
			get {
				if (String.IsNullOrEmpty(text)) {
					IPieceTableContainer parent = GetParentByType<IPieceTableContainer>();
					text = ModelBox.GetText(parent.PieceTable);
				}
				return text;
			}
		}
		public override void Accept(LayoutVisitor visitor) {
			switch (Type) {
				case LayoutType.PlainTextBox:
					visitor.VisitPlainTextBox(this);
					break;
				case LayoutType.SpecialTextBox:
					visitor.VisitSpecialTextBox(this);
					break;
				case LayoutType.SpaceBox:
					visitor.VisitSpaceBox(this);
					break;
				case LayoutType.ParagraphMarkBox:
					visitor.VisitParagraphMarkBox(this);
					break;
				case LayoutType.SectionBreakBox:
					visitor.VisitSectionBreakBox(this);
					break;
				case LayoutType.LineBreakBox:
					visitor.VisitLineBreakBox(this);
					break;
				case LayoutType.PageBreakBox:
					visitor.VisitPageBreakBox(this);
					break;
				case LayoutType.ColumnBreakBox:
					visitor.VisitColumnBreakBox(this);
					break;
				case LayoutType.HyphenBox:
					visitor.VisitHyphenBox(this);
					break;
				case LayoutType.TabSpaceBox:
					visitor.VisitTabSpaceBox(this);
					break;
				case LayoutType.PageNumberBox:
					visitor.VisitPageNumberBox(this);
					break;
				case LayoutType.CustomRunBox:
					visitor.VisitCustomRunBox(this);
					break;
				case LayoutType.DataContainerRunBox:
					visitor.VisitDataContainerRunBox(this);
					break;
				default:
					break;
			}
		}
		internal void SetText(string text) {
			this.text = text;
		}
	}
	#endregion
	#region InlinePictureBox
	public class InlinePictureBox : Box {
		OfficeImage image;
		internal InlinePictureBox(ModelInlinePictureBox box, LayoutElement parent)
			: base(box, LayoutType.InlinePictureBox, parent) {
		}
		#region Image
		public OfficeImage Image {
			get {
				if (image == null) {
					IPieceTableContainer parent = GetParentByType<IPieceTableContainer>();
					image = ((ModelInlinePictureBox)ModelBox).GetImage(parent.PieceTable, true);
				}
				return image;
			}
		}
		#endregion
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitInlinePictureBox(this);
		}
	}
	#endregion
	#region FloatingObjectAnchorBox
	public class FloatingObjectAnchorBox : Box {
		LayoutFloatingObject floatingObjectBox;
		Rectangle bounds;
		internal FloatingObjectAnchorBox(ModelFloatingObjectAnchorBox anchorBox, Rectangle bounds, LayoutElement parent)
			: base(anchorBox, LayoutType.FloatingObjectAnchorBox, parent) {
			this.bounds = bounds;
		}
		public LayoutFloatingObject FloatingObjectBox {
			get {
				if (floatingObjectBox == null)
					floatingObjectBox = GetFloatingObjectBox();
				return floatingObjectBox;
			}
		}
		protected override Rectangle GetBounds() {
			return bounds;
		}
		LayoutFloatingObject GetFloatingObjectBox() {
			LayoutPage page = GetParentByType<LayoutPage>();
			IPieceTableContainer pieceTableContainer = GetParentByType<IPieceTableContainer>();
			DevExpress.XtraRichEdit.Model.FloatingObjectAnchorRun anchorRun = (DevExpress.XtraRichEdit.Model.FloatingObjectAnchorRun)ModelBox.GetRun(pieceTableContainer.PieceTable);
			if (anchorRun.FloatingObjectProperties.TextWrapType != DevExpress.XtraRichEdit.Model.FloatingObjectTextWrapType.None)
				return GetFloatingObjectBoxCore(page.FloatingObjects, anchorRun);
			if (anchorRun.FloatingObjectProperties.IsBehindDoc)
				return GetFloatingObjectBoxCore(page.BackgroundFloatingObjects, anchorRun);
			else
				return GetFloatingObjectBoxCore(page.ForegroundFloatingObjects, anchorRun);
		}
		LayoutFloatingObject GetFloatingObjectBoxCore(LayoutFloatingObjectCollection collection, DevExpress.XtraRichEdit.Model.FloatingObjectAnchorRun anchorRun) {
			for (int i = 0; i < collection.Count; i++)
				if (collection[i].AnchorRun == anchorRun)
					return collection[i];
			return null;
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitFloatingObjectAnchorBox(this);
		}
	}
	#endregion
	#region NumberingListMarkBox
	public class NumberingListMarkBox : LayoutElementBase {
		NumberingListBox box;
		int listIndex = -1;
		int listLevel = -1;
		internal NumberingListMarkBox(NumberingListBox box, LayoutElement parent)
			: base(parent) {
			this.box = box;
		}
		internal NumberingListBox ModelBox { get { return box; } }
		#region ListIndex
		public int ListIndex {
			get {
				if (listIndex < 0) {
					Paragraph paragraph = GetParagraph();
					listIndex = ((IConvertToInt<Model.NumberingListIndex>)paragraph.NumberingListIndex).ToInt();
				}
				return listIndex;
			}
		}
		#endregion
		#region ListLevel
		public int ListLevel {
			get {
				if (listLevel < 0) {
					Paragraph paragraph = GetParagraph();
					listLevel = paragraph.GetListLevelIndex();
				}
				return listLevel;
			}
		}
		#endregion
		public string Text { get { return box.NumberingListText; } }
		public override LayoutType Type { get { return LayoutType.NumberingListMarkBox; } }
		protected override Rectangle GetBounds() {
			return ModelBox.Bounds;
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitNumberingListMarkBox(this);
		}
		protected Paragraph GetParagraph() {
			IPieceTableContainer parent = GetParentByType<IPieceTableContainer>();
			DevExpress.XtraRichEdit.Model.TextRunBase run = box.GetRun(parent.PieceTable);
			return run.Paragraph;
		}
	}
	#endregion
	#region NumberingListWithSeparatorBox
	public class NumberingListWithSeparatorBox : NumberingListMarkBox {
		PlainTextBox separator;
		internal NumberingListWithSeparatorBox(NumberingListBoxWithSeparator box, LayoutElement parent)
			: base(box, parent) {
			LayoutType type = box.SeparatorBox is TabSpaceBox ? LayoutType.TabSpaceBox : LayoutType.SpaceBox;
			this.separator = new PlainTextBox(box.SeparatorBox, type, this);
			Rectangle separatorBounds = new Rectangle(separator.Bounds.X, box.Bounds.Y, separator.Bounds.Width, box.Bounds.Height);
			this.separator.SetBounds(separatorBounds);
			Paragraph paragraph = GetParagraph();
			this.separator.SetText(paragraph.GetListLevelSeparator());
		}
		public PlainTextBox Separator { get { return separator; } }
		public override LayoutType Type { get { return LayoutType.NumberingListWithSeparatorBox; } }
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitNumberingListWithSeparatorBox(this);
		}
	}
	#endregion
	#region LayoutRow
	public class LayoutRow : RangedLayoutElementBase {
		Row row;
		Lazy<BoxCollection> boxes;
		RowExtendedBoxes extendedBoxes;
		NumberingListMarkBox numberingListBox;
		internal LayoutRow(Row row, LayoutElement parent)
			: base(parent) {
			this.row = row;
			InitializeNumberingListBox();
			this.boxes = new Lazy<BoxCollection>(() => {
				LayoutRowExporter exporter = new LayoutRowExporter(this);
				exporter.ExportRow(row);
				return exporter.Boxes;
			});
			this.extendedBoxes = new RowExtendedBoxes(row.ExtendedBoxes, this);
		}
		public BoxCollection Boxes { get { return boxes.Value; } }
		public NumberingListMarkBox NumberingListBox { get { return numberingListBox; } }
		public RowExtendedBoxes ExtendedBoxes { get { return extendedBoxes; } }
		public override LayoutType Type { get { return LayoutType.Row; } }
		internal Row ModelRow { get { return row; } }
		protected override Rectangle GetBounds() {
			return ModelRow.Bounds;
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitRow(this);
		}
		void InitializeNumberingListBox() {
			if (row.NumberingListBox != null) {
				NumberingListBoxWithSeparator boxWithSeparator = row.NumberingListBox as NumberingListBoxWithSeparator;
				if (boxWithSeparator != null)
					this.numberingListBox = new NumberingListWithSeparatorBox(boxWithSeparator, this);
				else
					this.numberingListBox = new NumberingListMarkBox(row.NumberingListBox, this);
			}
		}
		internal override FixedRange CreateRange() {
			int firstBoxRangeStart = Boxes[0].Range.Start;
			FixedRange lastBoxRange = Boxes[Boxes.Count - 1].Range;
			int lastBoxRangeEnd = lastBoxRange.Start + lastBoxRange.Length;
			return new FixedRange(firstBoxRangeStart, lastBoxRangeEnd - firstBoxRangeStart);
		}
	}
	#endregion
	#region LayoutTableCell
	public class LayoutTableCell : RangedLayoutElementBase {
		TableCellViewInfo tableCell;
		Lazy<LayoutRowCollection> rows;
		Lazy<LayoutTableCollection> nestedTables;
		Borders borders;
		internal LayoutTableCell(TableCellViewInfo tableCell, LayoutElement parent)
			: base(parent) {
			this.tableCell = tableCell;
			this.rows = new Lazy<LayoutRowCollection>(() => {
				RowCollection rowCollection = tableCell.GetRows(tableCell.TableViewInfo.Column);
				return DocumentLayoutHelper.InitializeLayoutRowCollection(rowCollection, this, true);
			});
			this.nestedTables = new Lazy<LayoutTableCollection>(() => {
				LayoutTableCollection nestedTables = new LayoutTableCollection();
				for (int i = 0; i < tableCell.InnerTables.Count; i++)
					nestedTables.Add(new LayoutTable(tableCell.InnerTables[i], this));
				return nestedTables;
			});
			this.borders = DocumentLayoutHelper.CreateBorders(this, tableCell.LeftBorder, tableCell.RightBorder, tableCell.TopBorder, tableCell.BottomBorder);
		}
		public LayoutRowCollection Rows { get { return rows.Value; } }
		public LayoutTableCollection NestedTables { get { return nestedTables.Value; } }
		public override LayoutType Type { get { return LayoutType.TableCell; } }
		public Borders Borders { get { return borders; } }
		internal TableCellViewInfo ModelTableCell { get { return tableCell; } }
		protected override Rectangle GetBounds() {
			return ModelTableCell.GetBounds();
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitTableCell(this);
		}
		internal override FixedRange CreateRange() {
			IPieceTableContainer container = GetParentByType<IPieceTableContainer>();
			DevExpress.XtraRichEdit.Model.PieceTable pieceTable = container.PieceTable;
			Paragraph fistParagraph = pieceTable.Paragraphs[tableCell.Cell.StartParagraphIndex];
			Paragraph lastParagraph = pieceTable.Paragraphs[tableCell.Cell.EndParagraphIndex];
			DocumentModelPosition startPosition = DocumentModelPosition.FromRunStart(container.PieceTable, fistParagraph.FirstRunIndex);
			DocumentModelPosition endPosition = DocumentModelPosition.FromRunStart(container.PieceTable, lastParagraph.LastRunIndex);
			int start = ((IConvertToInt<DevExpress.XtraRichEdit.Model.DocumentLogPosition>)startPosition.LogPosition).ToInt();
			int end = ((IConvertToInt<DevExpress.XtraRichEdit.Model.DocumentLogPosition>)endPosition.LogPosition).ToInt();
			return new FixedRange(start, end - start + 1);
		}
	}
	#endregion
	#region LayoutTableRow
	public class LayoutTableRow : RangedLayoutElementBase {
		TableRowViewInfoBase modelTableRow;
		Lazy<LayoutTableCellCollection> tableCells;
		internal LayoutTableRow(TableRowViewInfoBase tableRow, LayoutElement parent)
			: base(parent) {
			this.modelTableRow = tableRow;
			this.tableCells = new Lazy<LayoutTableCellCollection>(() => {
				LayoutTableCellCollection tableCells = new LayoutTableCellCollection();
				for (int i = 0; i < tableRow.Cells.Count; i++) {
					TableCellViewInfo cell = tableRow.Cells[i];
					if (tableRow == cell.GetTableRow())
						tableCells.Add(new LayoutTableCell(cell, this));
				}
				return tableCells;
			});
		}
		internal TableRowViewInfoBase ModelTableRow { get { return modelTableRow; } }
		public LayoutTableCellCollection TableCells { get { return tableCells.Value; } }
		public override LayoutType Type { get { return LayoutType.TableRow; } }
		protected override Rectangle GetBounds() {
			return ModelTableRow.GetBounds();
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitTableRow(this);
		}
		internal override FixedRange CreateRange() {
			int firstTableCellRangeStart = TableCells[0].Range.Start;
			FixedRange lastTableCellRange = TableCells.Last.Range;
			int lastTableCellRangeEnd = lastTableCellRange.Start + lastTableCellRange.Length;
			return new FixedRange(firstTableCellRangeStart, lastTableCellRangeEnd - firstTableCellRangeStart);
		}
	}
	#endregion
	#region LayoutTable
	public class LayoutTable : RangedLayoutElementBase {
		TableViewInfo table;
		Lazy<LayoutTableRowCollection> tableRows;
		Borders borders;
		internal LayoutTable(TableViewInfo table, LayoutElement parent)
			: base(parent) {
			this.table = table;
			this.tableRows = new Lazy<LayoutTableRowCollection>(() => {
				LayoutTableRowCollection tableRows = new LayoutTableRowCollection();
				for (int i = 0; i < table.RowCount; i++)
					tableRows.Add(new LayoutTableRow(table.Rows[i], this));
				return tableRows;
			});
			this.borders = DocumentLayoutHelper.CreateBorders(this, table.GetActualLeftBorder(), table.GetActualRightBorder(), table.GetActualTopBorder(), table.GetActualBottomBorder());
		}
		public LayoutTableRowCollection TableRows { get { return tableRows.Value; } }
		public override LayoutType Type { get { return LayoutType.Table; } }
		public Borders Borders { get { return borders; } }
		internal TableViewInfo ModelTable { get { return table; } }
		protected override Rectangle GetBounds() {
			Rectangle firstCellBounds = table.Cells.First.GetBounds();
			Rectangle lastCellBounds = table.Cells.Last.GetBounds();
			int width = lastCellBounds.X + lastCellBounds.Width - firstCellBounds.X;
			int height = lastCellBounds.Y + lastCellBounds.Height - firstCellBounds.Y;
			return new Rectangle(firstCellBounds.X, firstCellBounds.Y, width, height);
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitTable(this);
		}
		internal override FixedRange CreateRange() {
			int firstTableRowRangeStart = TableRows[0].Range.Start;
			FixedRange lastTableRowRange = TableRows.Last.Range;
			int lastTableRowRangeEnd = lastTableRowRange.Start + lastTableRowRange.Length;
			return new FixedRange(firstTableRowRangeStart, lastTableRowRangeEnd - firstTableRowRangeStart);
		}
	}
	#endregion
	#region LayoutFloatingObject
	public abstract class LayoutFloatingObject : LayoutElementBase {
		const int UnitsPerDegree = 60000;
		FloatingObjectBox floatingObject;
		DevExpress.XtraRichEdit.Model.FloatingObjectAnchorRun anchorRun;
		FloatingObjectAnchorBox anchorBox;
		internal LayoutFloatingObject(FloatingObjectBox floatingObject, LayoutElement parent)
			: base(parent) {
			this.floatingObject = floatingObject;
		}
		#region AnchorBox
		public FloatingObjectAnchorBox AnchorBox {
			get {
				if (anchorBox == null) {
					DocumentModelPosition modelPosition = DocumentModelPosition.FromRunStart(ModelFloatingObject.PieceTable, AnchorRun.GetRunIndex());
					int position = ((IConvertToInt<DevExpress.XtraRichEdit.Model.DocumentLogPosition>)modelPosition.LogPosition).ToInt();
					LayoutPage page = GetParentByType<LayoutPage>();
					anchorBox = DocumentLayout.GetElementFromPage<FloatingObjectAnchorBox>(position, page.Document.PieceTable, page);
				}
				return anchorBox;
			}
		}
		#endregion
		internal FloatingObjectBox ModelFloatingObject { get { return floatingObject; } }
		internal DevExpress.XtraRichEdit.Model.FloatingObjectAnchorRun AnchorRun {
			get {
				if (anchorRun == null)
					anchorRun = ModelFloatingObject.GetFloatingObjectRun();
				return anchorRun;
			}
		}
		public Rectangle ContentBounds { get { return ModelFloatingObject.ContentBounds; } }
		public float RotationAngle { get { return AnchorRun.Shape.Rotation / UnitsPerDegree; } }
		protected override Rectangle GetBounds() {
			return ModelFloatingObject.ActualSizeBounds;
		}
		public Shape GetDocumentShape() {
			LayoutPage page = GetParentByType<LayoutPage>();
			DevExpress.XtraRichEdit.Model.DocumentModelPosition startPos = ModelFloatingObject.GetFirstPosition(ModelFloatingObject.PieceTable);
			DocumentRange range = page.Document.CreateRange(startPos.LogPosition, 1);
			ReadOnlyShapeCollection shapes = page.Document.Shapes.Get(range);
			return shapes[0];
		}
		public Point[] GetCoordinates() {
			return GetCoordinatesCore(Bounds);
		}
		public Point[] GetContentCoordinates() {
			return GetCoordinatesCore(ContentBounds);
		}
		Point[] GetCoordinatesCore(Rectangle bounds) {
			Matrix matrix = new Matrix();
			matrix.Rotate(ModelFloatingObject.PieceTable.DocumentModel.UnitConverter.ModelUnitsToDegreeF(AnchorRun.Shape.Rotation));
			Point[] points = new Point[] { bounds.Location, new Point(bounds.Right, bounds.Top), new Point(bounds.Right, bounds.Bottom), new Point(bounds.Left, bounds.Bottom) };
			matrix.TransformPoints(points);
			return points;
		}
	}
	#endregion
	#region LayoutTextBox
	public class LayoutTextBox : LayoutFloatingObject, RangedLayoutElement, IPieceTableContainer {
		NativeSubDocument document;
		Lazy<LayoutTableCollection> tables;
		Lazy<LayoutRowCollection> rows;
		FixedRange range;
		internal LayoutTextBox(FloatingObjectBox floatingObject, LayoutElement parent)
			: base(floatingObject, parent) {
			this.tables = new Lazy<LayoutTableCollection>(() => {
				TableViewInfoCollection modelCollection = ModelFloatingObject.DocumentLayout.Pages[0].Areas[0].Columns[0].Tables;
				return DocumentLayoutHelper.InitializeLayoutTableCollection(modelCollection, this);
			});
			this.rows = new Lazy<LayoutRowCollection>(() => {
				RowCollection modelCollection = ModelFloatingObject.DocumentLayout.Pages[0].Areas[0].Columns[0].Rows;
				return DocumentLayoutHelper.InitializeLayoutRowCollection(modelCollection, this, false);
			});
		}
		public override LayoutType Type { get { return LayoutType.TextBox; } }
		public LayoutTableCollection Tables { get { return tables.Value; } }
		public LayoutRowCollection Rows { get { return rows.Value; } }
		public SubDocument Document { get { return NativeDocument; } }
		#region NativeDocument
		internal NativeSubDocument NativeDocument { 
			get {
				if (document == null) {
					LayoutPage page = GetParentByType<LayoutPage>();
					DevExpress.XtraRichEdit.Model.DocumentModelPosition startPos = ModelFloatingObject.GetFirstPosition(ModelFloatingObject.PieceTable);
					DocumentRange textBoxRange = page.Document.CreateRange(startPos.LogPosition, 1);
					ReadOnlyShapeCollection shapes = page.Document.Shapes.Get(textBoxRange);
					document = (NativeSubDocument)shapes[0].TextBox.Document;
				}
				return document;
			}
		}
		#endregion
		public FixedRange Range {
			get {
				if (range == null)
					range = DocumentLayoutHelper.GetRangeForTablesAndRowsContainer(Rows, Tables);
				return range;
			}
		}
		PieceTable IPieceTableContainer.PieceTable { get { return ((DevExpress.XtraRichEdit.Model.TextBoxFloatingObjectContent)AnchorRun.Content).TextBox.PieceTable; } }
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitTextBox(this);
		}
	}
	#endregion
	#region LayoutFloatingPicture
	public class LayoutFloatingPicture : LayoutFloatingObject {
		OfficeImage image;
		internal LayoutFloatingPicture(FloatingObjectBox floatingObject, LayoutElement parent)
			: base(floatingObject, parent) {
		}
		public OfficeImage Image {
			get {
				if (image == null)
					image = AnchorRun.PictureContent.Image;
				return image;
			}
		}
		public override LayoutType Type { get { return LayoutType.FloatingPicture; } }
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitFloatingPicture(this);
		}
	}
	#endregion
	#region LayoutColumn
	public class LayoutColumn : RangedLayoutElementBase {
		Column column;
		Lazy<LayoutTableCollection> tables;
		Lazy<LayoutRowCollection> rows;
		Lazy<LineNumberBoxCollection> lineNumbers;
		internal LayoutColumn(Column column, LayoutElement parent)
			: base(parent) {
			this.column = column;
			this.tables = new Lazy<LayoutTableCollection>(() => {
				return DocumentLayoutHelper.InitializeLayoutTableCollection(ModelTables, this);
			});
			this.rows = new Lazy<LayoutRowCollection>(() => {
				return DocumentLayoutHelper.InitializeLayoutRowCollection(ModelRows, this, false);
			});
			this.lineNumbers = new Lazy<LineNumberBoxCollection>(() => {
				LayoutPageArea pageArea = this.Parent as LayoutPageArea;
				LineNumberBoxCollection lineNumbers = new LineNumberBoxCollection();
				if (pageArea != null) {
					for (int i = 0; i < ModelRows.Count; i++) {
						ModelLineNumberBox box = pageArea.ModelLineNumberBoxes.Find((modelLineNumberBox) => { return ModelRows[i] == modelLineNumberBox.Row; });
						if (box != null)
							lineNumbers.Add(new LineNumberBox(box, this));
					}
				}
				return lineNumbers;
			});
		}
		public override LayoutType Type { get { return LayoutType.Column; } }
		RowCollection ModelRows { get { return column.Rows; } }
		TableViewInfoCollection ModelTables { get { return column.Tables; } }
		public LayoutTableCollection Tables { get { return tables.Value; } }
		public LayoutRowCollection Rows { get { return rows.Value; } }
		public LineNumberBoxCollection LineNumbers { get { return lineNumbers.Value; } }
		internal Column ModelColumn { get { return column; } }
		protected override Rectangle GetBounds() {
			return ModelColumn.Bounds;
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitColumn(this);
		}
		internal override FixedRange CreateRange() {
			return DocumentLayoutHelper.GetRangeForTablesAndRowsContainer(Rows, Tables);
		}
	}
	#endregion
	#region LayoutHeaderFooterBase
	public abstract class LayoutHeaderFooterBase : LayoutPageAreaBase {
		HeaderFooterPageAreaBase modelHeaderFooter;
		NativeSection section;
		internal LayoutHeaderFooterBase(HeaderFooterPageAreaBase modelHeaderFooter, LayoutElement parent)
			: base(parent) {
			this.modelHeaderFooter = modelHeaderFooter;
		}
		internal HeaderFooterPageAreaBase ModelHeaderFooter { get { return modelHeaderFooter; } }
		protected override ColumnCollection ModelColumns { get { return modelHeaderFooter.Columns; } }
		protected NativeSection Section {
			get {
				if (section == null)
					section = GetSection();
				return section;
			}
		}
		protected override Rectangle GetBounds() {
			return ModelHeaderFooter.Bounds;
		}
		public abstract SubDocument BeginUpdate();
		public abstract SubDocument BeginUpdate(HeaderFooterType type);
		public abstract void EndUpdate(SubDocument document);
		protected override PieceTable GetPieceTable() {
			return modelHeaderFooter.PieceTable;
		}
		NativeSection GetSection() {
			LayoutPage page = GetParentByType<LayoutPage>();
			for (int i = 0; i < page.Document.Sections.Count; i++) {
				NativeSection section = (NativeSection)page.Document.Sections[i];
				if (section.InnerSection == modelHeaderFooter.Section)
					return section;
			}
			return null;
		}
	}
	#endregion
	#region LayoutHeader
	public class LayoutHeader : LayoutHeaderFooterBase {
		internal LayoutHeader(HeaderPageArea header, LayoutElement parent)
			: base(header, parent) {
		}
		public override LayoutType Type { get { return LayoutType.Header; } }
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitHeader(this);
		}
		public override SubDocument BeginUpdate() {
			return Section.BeginUpdateHeader();
		}
		public override SubDocument BeginUpdate(HeaderFooterType type) {
			return Section.BeginUpdateHeader(type);
		}
		public override void EndUpdate(SubDocument document) {
			Section.EndUpdateHeader(document);
		}
	}
	#endregion
	#region LayoutFooter
	public class LayoutFooter : LayoutHeaderFooterBase {
		internal LayoutFooter(FooterPageArea footer, LayoutElement parent)
			: base(footer, parent) {
		}
		public override LayoutType Type { get { return LayoutType.Footer; } }
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitFooter(this);
		}
		public override SubDocument BeginUpdate() {
			return Section.BeginUpdateFooter();
		}
		public override SubDocument BeginUpdate(HeaderFooterType type) {
			return Section.BeginUpdateFooter(type);
		}
		public override void EndUpdate(SubDocument document) {
			Section.EndUpdateFooter(document);
		}
	}
	#endregion
	#region LayoutComment
	public class LayoutComment : LayoutPageAreaBase {
		CommentViewInfo comment;
		#region CommentVisitor
		class CommentVisitor : LayoutVisitor {
			LayoutComment comment;
			public CommentVisitor(LayoutComment comment) {
				this.comment = comment;
			}
			void OffsetBounds(LayoutElementBase box, int xOffset, int yOffset) {
				Rectangle boxBounds = box.Bounds;
				Point location = new Point(boxBounds.X + xOffset, boxBounds.Y + yOffset);
				Rectangle bounds = new Rectangle(location, boxBounds.Size);
				box.SetBounds(bounds);
			}
			void OffsetBounds(LayoutElementBase box) {
				OffsetBounds(box, comment.ContentBounds.X, comment.ContentBounds.Y);
			}
			protected internal override void VisitColumn(LayoutColumn column) {
				OffsetBounds(column);
				base.VisitColumn(column);
			}
			protected internal override void VisitBookmarkEndBox(BookmarkBox bookmarkEndBox) {
				OffsetBounds(bookmarkEndBox, comment.ContentBounds.X, 0);
				base.VisitBookmarkEndBox(bookmarkEndBox);
			}
			protected internal override void VisitBookmarkStartBox(BookmarkBox bookmarkStartBox) {
				OffsetBounds(bookmarkStartBox, comment.ContentBounds.X, 0);
				base.VisitBookmarkStartBox(bookmarkStartBox);
			}
			protected internal override void VisitRangePermissionHighlightAreaBox(RangePermissionHighlightAreaBox rangePermissionHighlightAreaBox) {
				OffsetBounds(rangePermissionHighlightAreaBox);
				base.VisitRangePermissionHighlightAreaBox(rangePermissionHighlightAreaBox);
			}
			protected internal override void VisitRangePermissionStartBox(RangePermissionBox rangePermissionStartBox) {
				OffsetBounds(rangePermissionStartBox, comment.ContentBounds.X, 0);
				base.VisitRangePermissionStartBox(rangePermissionStartBox);
			}
			protected internal override void VisitRangePermissionEndBox(RangePermissionBox rangePermissionEndBox) {
				OffsetBounds(rangePermissionEndBox, comment.ContentBounds.X, 0);
				base.VisitRangePermissionEndBox(rangePermissionEndBox);
			}
			protected internal override void VisitFieldHighlightAreaBox(FieldHighlightAreaBox fieldHighlightAreaBox) {
				OffsetBounds(fieldHighlightAreaBox);
				base.VisitFieldHighlightAreaBox(fieldHighlightAreaBox);
			}
			protected internal override void VisitHiddenTextUnderlineBox(HiddenTextUnderlineBox hiddenTextUnderlineBox) {
				OffsetBounds(hiddenTextUnderlineBox);
				base.VisitHiddenTextUnderlineBox(hiddenTextUnderlineBox);
			}
			protected internal override void VisitHighlightAreaBox(HighlightAreaBox highlightAreaBox) {
				OffsetBounds(highlightAreaBox);
				base.VisitHighlightAreaBox(highlightAreaBox);
			}
			protected internal override void VisitHyphenBox(PlainTextBox hyphen) {
				OffsetBounds(hyphen);
				base.VisitHyphenBox(hyphen);
			}
			protected internal override void VisitInlinePictureBox(InlinePictureBox inlinePictureBox) {
				OffsetBounds(inlinePictureBox);
				base.VisitInlinePictureBox(inlinePictureBox);
			}
			protected internal override void VisitLineBreakBox(PlainTextBox lineBreakBox) {
				OffsetBounds(lineBreakBox);
				base.VisitLineBreakBox(lineBreakBox);
			}
			protected internal override void VisitNumberingListMarkBox(NumberingListMarkBox numberingListMarkBox) {
				OffsetBounds(numberingListMarkBox);
				base.VisitNumberingListMarkBox(numberingListMarkBox);
			}
			protected internal override void VisitNumberingListWithSeparatorBox(NumberingListWithSeparatorBox numberingListWithSeparatorBox) {
				OffsetBounds(numberingListWithSeparatorBox);
				base.VisitNumberingListWithSeparatorBox(numberingListWithSeparatorBox);
			}
			protected internal override void VisitParagraphMarkBox(PlainTextBox paragraphMarkBox) {
				OffsetBounds(paragraphMarkBox);
				base.VisitParagraphMarkBox(paragraphMarkBox);
			}
			protected internal override void VisitPlainTextBox(PlainTextBox plainTextBox) {
				OffsetBounds(plainTextBox);
				base.VisitPlainTextBox(plainTextBox);
			}
			protected internal override void VisitRow(LayoutRow row) {
				OffsetBounds(row);
				base.VisitRow(row);
			}
			protected internal override void VisitSpaceBox(PlainTextBox spaceBox) {
				OffsetBounds(spaceBox);
				base.VisitSpaceBox(spaceBox);
			}
			protected internal override void VisitSpecialTextBox(PlainTextBox specialTextBox) {
				OffsetBounds(specialTextBox);
				base.VisitSpecialTextBox(specialTextBox);
			}
			protected internal override void VisitStrikeoutBox(StrikeoutBox strikeoutBox) {
				OffsetBounds(strikeoutBox);
				base.VisitStrikeoutBox(strikeoutBox);
			}
			protected internal override void VisitTabSpaceBox(PlainTextBox tabSpaceBox) {
				OffsetBounds(tabSpaceBox);
				base.VisitTabSpaceBox(tabSpaceBox);
			}
			protected internal override void VisitUnderlineBox(UnderlineBox underlineBox) {
				OffsetBounds(underlineBox);
				base.VisitUnderlineBox(underlineBox);
			}
		}
		#endregion
		internal LayoutComment(CommentViewInfo comment, LayoutElement parent)
			: base(parent) {
			this.comment = comment;
			CommentVisitor visitor = new CommentVisitor(this);
			visitor.Visit(this);
		}
		internal CommentViewInfo ModelComment { get { return comment; } }
		public override LayoutType Type { get { return LayoutType.Comment; } }
		public Rectangle ContentBounds { get { return comment.ContentBounds; } }
		internal PieceTable PieceTable { get { return GetPieceTable(); } }
		protected override ColumnCollection ModelColumns { get { return comment.CommentDocumentLayout.Pages[0].Areas[0].Columns; } }
		protected override Rectangle GetBounds() {
			return ModelComment.Bounds;
		}
		public Comment GetDocumentComment() {
			LayoutPage page = (LayoutPage)Parent;
			return page.Document.Comments[ModelComment.Comment.Index];
		}
		protected override PieceTable GetPieceTable() {
			return this.comment.Comment.Content.PieceTable;
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitComment(this);
		}
		public SubDocument BeginUpdate() {
			Comment comment = GetComment();
			return comment.BeginUpdate();
		}
		public void EndUpdate(SubDocument document) {
			Comment comment = GetComment();
			comment.EndUpdate(document);
		}
		Comment GetComment() {
			LayoutPage page = GetParentByType<LayoutPage>();
			int index = comment.Comment.Index;
			return page.Document.Comments[index];
		}
	}
	#endregion
	#region LayoutPageAreaBase
	public abstract class LayoutPageAreaBase : RangedLayoutElementBase, IPieceTableContainer {
		Lazy<LayoutColumnCollection> columns;
		protected LayoutPageAreaBase(LayoutElement parent)
			: base(parent) {
			this.columns = new Lazy<LayoutColumnCollection>(() => {
				LayoutColumnCollection columns = new LayoutColumnCollection();
				for (int i = 0; i < ModelColumns.Count; i++)
					columns.Add(new LayoutColumn(ModelColumns[i], this));
				return columns;
			});
		}
		PieceTable IPieceTableContainer.PieceTable { get { return GetPieceTable(); } }
		protected abstract ColumnCollection ModelColumns { get; }
		public LayoutColumnCollection Columns { get { return columns.Value; } }
		protected abstract PieceTable GetPieceTable();
		internal override FixedRange CreateRange() {
			int start = Columns.First.Range.Start;
			FixedRange lastColumnRange = Columns.Last.Range;
			int end = lastColumnRange.Start + lastColumnRange.Length;
			return new FixedRange(start, end - start);
		}
	}
	#endregion
	#region LayoutPageArea
	public class LayoutPageArea : LayoutPageAreaBase {
		PageArea modelPageArea;
		NativeDocument document;
		internal LayoutPageArea(PageArea pageArea, LayoutElement parent)
			: base(parent) {
			this.modelPageArea = pageArea;
		}
		#region Document
		public Document Document {
			get {
				if (document == null) {
					LayoutPage parent = GetParentByType<LayoutPage>();
					document = parent.Document;
				}
				return document;
			}
		}
		#endregion
		internal PageArea ModelPageArea { get { return modelPageArea; } }
		public override LayoutType Type { get { return LayoutType.PageArea; } }
		internal ModelLineNumberBoxCollection ModelLineNumberBoxes { get { return ModelPageArea.LineNumbers; } }
		protected override ColumnCollection ModelColumns { get { return ModelPageArea.Columns; } }
		protected override Rectangle GetBounds() {
			return ModelPageArea.Bounds;
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitPageArea(this);
		}
		protected override PieceTable GetPieceTable() {
			return ModelPageArea.PieceTable;
		}
	}
	#endregion
	#region LayoutPage
	public class LayoutPage : LayoutElementBase {
		#region Fields
		Page page;
		NativeDocument document;
		Lazy<LayoutPageAreaCollection> pageAreas;
		LayoutHeader header;
		LayoutFooter footer;
		Lazy<LayoutCommentCollection> comments;
		Lazy<LayoutFloatingObjectCollection> backgroundFloatingObjects;
		Lazy<LayoutFloatingObjectCollection> floatingObjects;
		Lazy<LayoutFloatingObjectCollection> foregroundFloatingObjects;
		Lazy<LayoutFloatingParagraphFrameCollection> paragraphFrames;
		FixedRange mainContentRange;
		#endregion
		internal LayoutPage(Page page, NativeDocument document)
			: base(null) {
			this.page = page;
			this.document = document;
			this.pageAreas = new Lazy<LayoutPageAreaCollection>(() => {
				LayoutPageAreaCollection pageAreas = new LayoutPageAreaCollection();
				page.Areas.ForEach((pageArea) => pageAreas.Add(new LayoutPageArea(pageArea, this)));
				return pageAreas;
			});
			this.comments = new Lazy<LayoutCommentCollection>(() => {
				LayoutCommentCollection comments = new LayoutCommentCollection();
				page.Comments.ForEach((comment) => comments.Add(new LayoutComment(comment, this)));
				return comments;
			});
			this.backgroundFloatingObjects = new Lazy<LayoutFloatingObjectCollection>(() => {
				return CreateFloatingObjects(page.BackgroundFloatingObjects);
			});
			this.floatingObjects = new Lazy<LayoutFloatingObjectCollection>(() => {
				return CreateFloatingObjects(page.FloatingObjects);
			});
			this.foregroundFloatingObjects = new Lazy<LayoutFloatingObjectCollection>(() => {
				return CreateFloatingObjects(page.ForegroundFloatingObjects);
			});
			this.paragraphFrames = new Lazy<LayoutFloatingParagraphFrameCollection>(() => {
				return CreateParagraphFrames(page.ParagraphFrames);
			});
		}
		#region Properties
		internal Page ModelPage { get { return page; } }
		public LayoutPageAreaCollection PageAreas { get { return pageAreas.Value; } }
		#region Header
		public LayoutHeader Header {
			get {
				if (page.Header != null && header == null)
					header = new LayoutHeader(page.Header, this);
				return header;
			}
		}
		#endregion
		#region Footer
		public LayoutFooter Footer {
			get {
				if (page.Footer != null && footer == null)
					footer = new LayoutFooter(page.Footer, this);
				return footer;
			}
		}
		#endregion
		public LayoutCommentCollection Comments { get { return comments.Value; } }
		public LayoutFloatingObjectCollection BackgroundFloatingObjects { get { return backgroundFloatingObjects.Value; } }
		public LayoutFloatingObjectCollection FloatingObjects { get { return floatingObjects.Value; } }
		public LayoutFloatingObjectCollection ForegroundFloatingObjects { get { return foregroundFloatingObjects.Value; } }
		internal LayoutFloatingParagraphFrameCollection ParagraphFrames { get { return paragraphFrames.Value; } }
		public override LayoutType Type { get { return LayoutType.Page; } }
		public int Index { get { return ModelPage.PageIndex; } }
		internal NativeDocument Document { get { return document; } }
		public FixedRange MainContentRange {
			get {
				if (mainContentRange == null) {
					int start = PageAreas.First.Range.Start;
					int end = PageAreas.Last.Range.Start + PageAreas.Last.Range.Length;
					mainContentRange = new FixedRange(start, end - start);
				}
				return mainContentRange;
			}
		}
		#endregion
		protected override Rectangle GetBounds() {
			return ModelPage.Bounds;
		}
		LayoutFloatingObjectCollection CreateFloatingObjects(List<FloatingObjectBox> modelFloatingObjects) {
			LayoutFloatingObjectCollection floatingObjects = new LayoutFloatingObjectCollection();
			modelFloatingObjects.ForEach(
				(currentFloatingObject) => {
					if (currentFloatingObject.DocumentLayout != null)
						floatingObjects.Add(new LayoutTextBox(currentFloatingObject, this));
					else
						floatingObjects.Add(new LayoutFloatingPicture(currentFloatingObject, this));
				});
			return floatingObjects;
		}
		LayoutFloatingParagraphFrameCollection CreateParagraphFrames(List<ParagraphFrameBox> modelParagraphFrames) {
			LayoutFloatingParagraphFrameCollection floatingParagraphFrames = new LayoutFloatingParagraphFrameCollection();
			modelParagraphFrames.ForEach(
				(currentParagraphFrame) => {
					floatingParagraphFrames.Add(new LayoutFloatingParagraphFrame(currentParagraphFrame, this));
				});
			return floatingParagraphFrames;
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitPage(this);
		}
	}
	#endregion
	#region LineNumberBox
	public class LineNumberBox : LayoutElementBase {
		ModelLineNumberBox modelBox;
		LayoutRow row;
		internal LineNumberBox(ModelLineNumberBox modelBox, LayoutElement parent)
			: base(parent) {
			this.modelBox = modelBox;
		}
		internal ModelLineNumberBox ModelBox { get { return modelBox; } }
		#region Row
		public LayoutRow Row {
			get {
				if (row == null) {
					LayoutColumn column = (LayoutColumn)Parent;
					for (int i = 0; i < column.Rows.Count; i++)
						if (Object.ReferenceEquals(column.Rows[i].ModelRow, modelBox.Row)) {
							row = column.Rows[i];
							break;
						}
				}
				return row;
			}
		}
		#endregion
		public string Text { get { return modelBox.GetText(null); } }
		public override LayoutType Type { get { return LayoutType.LineNumberBox; } }
		protected override Rectangle GetBounds() {
			return ModelBox.Bounds;
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitLineNumberBox(this);
		}
	}
	#endregion
	#region RowExtendedBoxes
	public class RowExtendedBoxes {
		#region Fields
		ModelRowExtendedBoxes boxes;
		LayoutRow parent;
		Lazy<UnderlineBoxCollection> underlines;
		Lazy<StrikeoutBoxCollection> strikeouts;
		Lazy<HighlightAreaCollection> highlightAreas;
		Lazy<FieldHighlightAreaCollection> fieldHighlightAreas;
		Lazy<RangePermissionHighlightAreaCollection> rangePermissionHighlightAreas;
		Lazy<CommentHighlightAreaCollection> commentHighlightAreas;
		Lazy<BookmarkBoxCollection> bookmarkBoxes;
		Lazy<RangePermissionBoxCollection> rangePermissionBoxes;
		Lazy<CommentBoxCollection> commentBoxes;
		Lazy<HiddenTextUnderlineBoxCollection> hiddenTextUnderlineBoxes;
		Lazy<LayoutElementCollection<SpecialAreaBox>> specialAreaBoxes;
		Lazy<LayoutElementCollection<CustomMarkBox>> customMarkBoxes;
		#endregion
		internal RowExtendedBoxes(ModelRowExtendedBoxes boxes, LayoutRow parent) {
			this.boxes = boxes;
			this.parent = parent;
			this.underlines = new Lazy<UnderlineBoxCollection>(() => {
				UnderlineBoxCollection underlines = new UnderlineBoxCollection();
				for (int i = 0; i < boxes.Underlines.Count; i++)
					underlines.Add(new UnderlineBox(boxes.Underlines[i], parent));
				return underlines;
			});
			this.strikeouts = new Lazy<StrikeoutBoxCollection>(() => {
				StrikeoutBoxCollection strikeouts = new StrikeoutBoxCollection();
				for (int i = 0; i < boxes.Strikeouts.Count; i++)
					strikeouts.Add(new StrikeoutBox(boxes.Strikeouts[i], parent));
				return strikeouts;
			});
			this.highlightAreas = new Lazy<HighlightAreaCollection>(() => {
				HighlightAreaCollection highlightAreas = new HighlightAreaCollection();
				for (int i = 0; i < boxes.HighlightAreas.Count; i++)
					highlightAreas.Add(new HighlightAreaBox(boxes.HighlightAreas[i], parent));
				return highlightAreas;
			});
			this.fieldHighlightAreas = new Lazy<FieldHighlightAreaCollection>(() => {
				FieldHighlightAreaCollection fieldHighlightAreas = new FieldHighlightAreaCollection();
				for (int i = 0; i < boxes.FieldHighlightAreas.Count; i++)
					fieldHighlightAreas.Add(new FieldHighlightAreaBox(boxes.FieldHighlightAreas[i], parent));
				return fieldHighlightAreas;
			});
			this.rangePermissionHighlightAreas = new Lazy<RangePermissionHighlightAreaCollection>(() => {
				RangePermissionHighlightAreaCollection rangePermissionHighlightAreas = new RangePermissionHighlightAreaCollection();
				for (int i = 0; i < boxes.RangePermissionHighlightAreas.Count; i++)
					rangePermissionHighlightAreas.Add(new RangePermissionHighlightAreaBox(boxes.RangePermissionHighlightAreas[i], parent));
				return rangePermissionHighlightAreas;
			});
			this.commentHighlightAreas = new Lazy<CommentHighlightAreaCollection>(() => {
				CommentHighlightAreaCollection commentHighlightAreas = new CommentHighlightAreaCollection();
				for (int i = 0; i < boxes.CommentHighlightAreas.Count; i++)
					commentHighlightAreas.Add(new CommentHighlightAreaBox(boxes.CommentHighlightAreas[i], parent));
				return commentHighlightAreas;
			});
			this.bookmarkBoxes = new Lazy<BookmarkBoxCollection>(() => {
				BookmarkBoxCollection bookmarkBoxes = new BookmarkBoxCollection();
				for (int i = 0; i < boxes.BookmarkBoxes.Count; i++) {
					if (boxes.BookmarkBoxes[i] is BookmarkStartBox)
						bookmarkBoxes.Add(new BookmarkBox(boxes.BookmarkBoxes[i], LayoutType.BookmarkStartBox, parent));
					else
						bookmarkBoxes.Add(new BookmarkBox(boxes.BookmarkBoxes[i], LayoutType.BookmarkEndBox, parent));
				}
				return bookmarkBoxes;
			});
			this.rangePermissionBoxes = new Lazy<RangePermissionBoxCollection>(() => {
				RangePermissionBoxCollection rangePermissionBoxes = new RangePermissionBoxCollection();
				for (int i = 0; i < boxes.RangePermissionBoxes.Count; i++) {
					if (boxes.RangePermissionBoxes[i] is BookmarkStartBox)
						rangePermissionBoxes.Add(new RangePermissionBox(boxes.RangePermissionBoxes[i], LayoutType.RangePermissionStartBox, parent));
					else
						rangePermissionBoxes.Add(new RangePermissionBox(boxes.RangePermissionBoxes[i], LayoutType.RangePermissionEndBox, parent));
				}
				return rangePermissionBoxes;
			});
			this.commentBoxes = new Lazy<CommentBoxCollection>(() => {
				CommentBoxCollection commentBoxes = new CommentBoxCollection();
				for (int i = 0; i < boxes.CommentBoxes.Count; i++) {
					if (boxes.CommentBoxes[i] is CommentStartBox)
						commentBoxes.Add(new CommentBox(boxes.CommentBoxes[i], LayoutType.CommentStartBox, parent));
					else
						commentBoxes.Add(new CommentBox(boxes.CommentBoxes[i], LayoutType.CommentEndBox, parent));
				}
				return commentBoxes;
			});
			this.hiddenTextUnderlineBoxes = new Lazy<HiddenTextUnderlineBoxCollection>(() => {
				HiddenTextUnderlineBoxCollection hiddenTextUnderlineBoxes = new HiddenTextUnderlineBoxCollection();
				for (int i = 0; i < boxes.HiddenTextBoxes.Count; i++)
					hiddenTextUnderlineBoxes.Add(new HiddenTextUnderlineBox(boxes.HiddenTextBoxes[i], parent));
				return hiddenTextUnderlineBoxes;
			});
			this.specialAreaBoxes = new Lazy<LayoutElementCollection<SpecialAreaBox>>(() => {
				LayoutElementCollection<SpecialAreaBox> specialAreaBoxes = new LayoutElementCollection<SpecialAreaBox>();
				for (int i = 0; i < boxes.SpecialTextBoxes.Count; i++)
					specialAreaBoxes.Add(new SpecialAreaBox(boxes.SpecialTextBoxes[i], parent));
				return specialAreaBoxes;
			});
			this.customMarkBoxes = new Lazy<LayoutElementCollection<CustomMarkBox>>(() => {
				LayoutElementCollection<CustomMarkBox> customMarkBoxes = new LayoutElementCollection<CustomMarkBox>();
				for (int i = 0; i < boxes.CustomMarkBoxes.Count; i++)
					customMarkBoxes.Add(new CustomMarkBox(boxes.CustomMarkBoxes[i], parent));
				return customMarkBoxes;
			});
		}
		#region Properties
		public UnderlineBoxCollection Underlines { get { return underlines.Value; } }
		public StrikeoutBoxCollection Strikeouts { get { return strikeouts.Value; } }
		public HighlightAreaCollection HighlightAreas { get { return highlightAreas.Value; } }
		public FieldHighlightAreaCollection FieldHighlightAreas { get { return fieldHighlightAreas.Value; } }
		public RangePermissionHighlightAreaCollection RangePermissionHighlightAreas { get { return rangePermissionHighlightAreas.Value; } }
		public CommentHighlightAreaCollection CommentHighlightAreas { get { return commentHighlightAreas.Value; } }
		public BookmarkBoxCollection BookmarkBoxes { get { return bookmarkBoxes.Value; } }
		public RangePermissionBoxCollection RangePermissionBoxes { get { return rangePermissionBoxes.Value; } }
		public CommentBoxCollection CommentBoxes { get { return commentBoxes.Value; } }
		public HiddenTextUnderlineBoxCollection HiddenTextUnderlineBoxes { get { return hiddenTextUnderlineBoxes.Value; } }
		internal LayoutElementCollection<SpecialAreaBox> SpecialAreaBoxes { get { return specialAreaBoxes.Value; } }
		internal LayoutElementCollection<CustomMarkBox> CustomMarkBoxes { get { return customMarkBoxes.Value; } }
		#endregion
	}
	#endregion
	#region UnderlineBox
	public class UnderlineBox : RangedLayoutElementBase {
		ModelUnderlineBox modelBox;
		internal UnderlineBox(ModelUnderlineBox modelBox, LayoutElement parent)
			: base(parent) {
			this.modelBox = modelBox;
		}
		internal ModelUnderlineBox ModelBox { get { return modelBox; } }
		public override LayoutType Type { get { return LayoutType.UnderlineBox; } }
		public int Thickness { get { return ModelBox.UnderlineThickness; } }
		protected override Rectangle GetBounds() {
			return ModelBox.ClipBounds;
		}
		internal override FixedRange CreateRange() {
			LayoutRow row = (LayoutRow)Parent;
			Box startBox = row.Boxes[ModelBox.StartAnchorIndex];
			Box endBox = row.Boxes[ModelBox.EndAnchorIndex - 1];
			return new FixedRange(startBox.Range.Start, endBox.Range.Start + endBox.Range.Length - startBox.Range.Start);
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitUnderlineBox(this);
		}
	}
	#endregion
	#region StrikeoutBox
	public class StrikeoutBox : UnderlineBox {
		internal StrikeoutBox(ModelUnderlineBox modelBox, LayoutElement parent)
			: base(modelBox, parent) {
		}
		public override LayoutType Type { get { return LayoutType.StrikeoutBox; } }
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitStrikeoutBox(this);
		}
	}
	#endregion
	#region HighlightAreaBox
	public class HighlightAreaBox : RangedLayoutElementBase {
		HighlightArea modelBox;
		internal HighlightAreaBox(HighlightArea modelBox, LayoutElement parent)
			: base(parent) {
			this.modelBox = modelBox;
		}
		internal HighlightArea ModelBox { get { return modelBox; } }
		public override LayoutType Type { get { return LayoutType.HighlightAreaBox; } }
		protected override Rectangle GetBounds() {
			return ModelBox.Bounds;
		}
		internal override FixedRange CreateRange() {
			return DocumentLayoutHelper.GetRangeFromRowByCoordinates((LayoutRow)Parent, Bounds.X, Bounds.X + Bounds.Width);
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitHighlightAreaBox(this);
		}
	}
	#endregion
	#region FieldHighlightAreaBox
	public class FieldHighlightAreaBox : HighlightAreaBox {
		internal FieldHighlightAreaBox(HighlightArea modelBox, LayoutElement parent)
			: base(modelBox, parent) {
		}
		public override LayoutType Type { get { return LayoutType.FieldHighlightAreaBox; } }
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitFieldHighlightAreaBox(this);
		}
	}
	#endregion
	#region RangePermissionHighlightAreaBox
	public class RangePermissionHighlightAreaBox : HighlightAreaBox {
		internal RangePermissionHighlightAreaBox(HighlightArea modelBox, LayoutElement parent)
			: base(modelBox, parent) {
		}
		public override LayoutType Type { get { return LayoutType.RangePermissionHighlightAreaBox; } }
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitRangePermissionHighlightAreaBox(this);
		}
	}
	#endregion
	#region CommentHighlightAreaBox
	public class CommentHighlightAreaBox : HighlightAreaBox {
		internal CommentHighlightAreaBox(HighlightArea modelBox, LayoutElement parent)
			: base(modelBox, parent) {
		}
		public override LayoutType Type { get { return LayoutType.CommentHighlightAreaBox; } }
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitCommentHighlightAreaBox(this);
		}
	}
	#endregion
	#region BookmarkBox
	public class BookmarkBox : LayoutElementBase {
		VisitableDocumentIntervalBox modelBox;
		LayoutType type;
		internal BookmarkBox(VisitableDocumentIntervalBox modelBox, LayoutType type, LayoutElement parent)
			: base(parent) {
			this.modelBox = modelBox;
			this.type = type;
		}
		internal VisitableDocumentIntervalBox ModelBox { get { return modelBox; } }
		public override LayoutType Type { get { return type; } }
		protected override Rectangle GetBounds() {
			return new Rectangle(new Point(ModelBox.HorizontalPosition, Parent.Bounds.Top), Size.Empty);
		}
		public override void Accept(LayoutVisitor visitor) {
			if (Type == LayoutType.BookmarkStartBox)
				visitor.VisitBookmarkStartBox(this);
			else
				visitor.VisitBookmarkEndBox(this);
		}
	}
	#endregion
	#region RangePermissionBox
	public class RangePermissionBox : BookmarkBox {
		internal RangePermissionBox(VisitableDocumentIntervalBox modelBox, LayoutType type, LayoutElement parent)
			: base(modelBox, type, parent) {
		}
		public override void Accept(LayoutVisitor visitor) {
			if (Type == LayoutType.RangePermissionStartBox)
				visitor.VisitRangePermissionStartBox(this);
			else
				visitor.VisitRangePermissionEndBox(this);
		}
	}
	#endregion
	#region CommentBox
	public class CommentBox : BookmarkBox {
		internal CommentBox(VisitableDocumentIntervalBox modelBox, LayoutType type, LayoutElement parent)
			: base(modelBox, type, parent) {
		}
		public override void Accept(LayoutVisitor visitor) {
			if (Type == LayoutType.CommentStartBox)
				visitor.VisitCommentStartBox(this);
			else
				visitor.VisitCommentEndBox(this);
		}
	}
	#endregion
	#region HiddenTextUnderlineBox
	public class HiddenTextUnderlineBox : RangedLayoutElementBase {
		ModelHiddenTextUnderlineBox modelBox;
		internal HiddenTextUnderlineBox(ModelHiddenTextUnderlineBox modelBox, LayoutElement parent)
			: base(parent) {
			this.modelBox = modelBox;
		}
		internal ModelHiddenTextUnderlineBox ModelBox { get { return modelBox; } }
		public override LayoutType Type { get { return LayoutType.HiddenTextUnderlineBox; } }
		protected override Rectangle GetBounds() {
			LayoutRow row = (LayoutRow)Parent;
			int y = row.Bounds.Top + row.ModelRow.BaseLineOffset + ModelBox.BottomOffset;
			return Rectangle.FromLTRB(ModelBox.Start, y, ModelBox.End, y);
		}
		internal override FixedRange CreateRange() {
			return DocumentLayoutHelper.GetRangeFromRowByCoordinates((LayoutRow)Parent, Bounds.X, Bounds.X + Bounds.Width);
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitHiddenTextUnderlineBox(this);
		}
	}
	#endregion
	public class CharacterBox : Box {
		string text = null;
		internal CharacterBox(ModelCharacterBox modelBox, LayoutElement parent)
			: base(modelBox, LayoutType.CharacterBox, parent) {
		}
		public string Text {
			get {
				if (String.IsNullOrEmpty(text)) {
					IPieceTableContainer parent = GetParentByType<IPieceTableContainer>();
					text = ModelBox.GetText(parent.PieceTable);
				}
				return text;
			}
		}
		protected override Rectangle GetBounds() {
			return ((ModelCharacterBox)ModelBox).TightBounds;
		}
		public override void Accept(LayoutVisitor visitor) {
		}
	}
	#region LayoutFloatingParagraphFrame
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class LayoutFloatingParagraphFrame : LayoutElementBase, IPieceTableContainer {
		readonly ParagraphFrameBox modelFrame;
		Lazy<LayoutTableCollection> tables;
		Lazy<LayoutRowCollection> rows;
		internal LayoutFloatingParagraphFrame(ParagraphFrameBox modelFrame, LayoutElement parent)
			: base(parent) {
			this.modelFrame = modelFrame;
			this.tables = new Lazy<LayoutTableCollection>(() => {
				TableViewInfoCollection modelCollection = modelFrame.DocumentLayout.Pages[0].Areas[0].Columns[0].Tables;
				return DocumentLayoutHelper.InitializeLayoutTableCollection(modelCollection, this);
			});
			this.rows = new Lazy<LayoutRowCollection>(() => {
				RowCollection modelCollection = modelFrame.DocumentLayout.Pages[0].Areas[0].Columns[0].Rows;
				return DocumentLayoutHelper.InitializeLayoutRowCollection(modelCollection, this, false);
			});
		}
		public PieceTable PieceTable { get { return modelFrame.PieceTable; } }
		public LayoutTableCollection Tables { get { return tables.Value; } }
		public LayoutRowCollection Rows { get { return rows.Value; } }
		public Rectangle ContentBounds { get { return modelFrame.ContentBounds; } }
		internal ParagraphFrameBox ModelParagraphFrame { get { return modelFrame; } }
		public override LayoutType Type {
			get { throw new NotImplementedException(); }
		}
		protected override Rectangle GetBounds() {
			return this.modelFrame.Bounds;
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitFloatingParagraphFrame(this);
		}
	}
	#endregion
	#region LayoutElementCollection
	public class LayoutElementCollection<T> : ISimpleCollection<T> where T : LayoutElement {
		List<T> items = new List<T>();
		internal LayoutElementCollection() { }
		public T First { get { return this[0]; } }
		public T Last { get { return this[Count - 1]; } }
		internal void Add(T element) {
			if (!items.Contains(element))
				items.Add(element);
		}
		internal void RemoveAt(int index) {
			if (items.Count > index)
				items.RemoveAt(index);
		}
		internal void Clear() {
			items.Clear();
		}
		internal void Insert(int index, T item) {
			items.Insert(index, item);
		}
		#region ISimpleCollection<T> Members
		public T this[int index] {
			get { return GetItem(index); }
		}
		T GetItem(int index) {
			Guard.ArgumentNonNegative(index, "index");
			return items[index];
		}
		#endregion
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			return items.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			items.CopyTo((T[])array, index);
		}
		public int Count {
			get { return items.Count; }
		}
		bool ICollection.IsSynchronized {
			get { return false; }
		}
		object ICollection.SyncRoot {
			get { return this; }
		}
		#endregion
	}
	#endregion
	public class BoxCollection : LayoutElementCollection<Box> { }
	public class LayoutRowCollection : LayoutElementCollection<LayoutRow> { }
	public class LayoutTableCellCollection : LayoutElementCollection<LayoutTableCell> { }
	public class LayoutTableRowCollection : LayoutElementCollection<LayoutTableRow> { }
	public class LayoutTableCollection : LayoutElementCollection<LayoutTable> { }
	public class LayoutFloatingObjectCollection : LayoutElementCollection<LayoutFloatingObject> { }
	public class LayoutColumnCollection : LayoutElementCollection<LayoutColumn> { }
	public class LayoutCommentCollection : LayoutElementCollection<LayoutComment> { }
	public class LayoutPageAreaCollection : LayoutElementCollection<LayoutPageArea> { }
	public class LayoutPageCollection : LayoutElementCollection<LayoutPage> { }
	public class LineNumberBoxCollection : LayoutElementCollection<LineNumberBox> { }
	public class UnderlineBoxCollection : LayoutElementCollection<UnderlineBox> { }
	public class StrikeoutBoxCollection : LayoutElementCollection<StrikeoutBox> { }
	public class RangePermissionBoxCollection : LayoutElementCollection<RangePermissionBox> { }
	public class CommentBoxCollection : LayoutElementCollection<CommentBox> { }
	public class BookmarkBoxCollection : LayoutElementCollection<BookmarkBox> { }
	public class HighlightAreaCollection : LayoutElementCollection<HighlightAreaBox> { }
	public class FieldHighlightAreaCollection : LayoutElementCollection<FieldHighlightAreaBox> { }
	public class RangePermissionHighlightAreaCollection : LayoutElementCollection<RangePermissionHighlightAreaBox> { }
	public class CommentHighlightAreaCollection : LayoutElementCollection<CommentHighlightAreaBox> { }
	public class HiddenTextUnderlineBoxCollection : LayoutElementCollection<HiddenTextUnderlineBox> { }
	public class CharacterBoxCollection : LayoutElementCollection<CharacterBox> { }
	internal class LayoutFloatingParagraphFrameCollection : LayoutElementCollection<LayoutFloatingParagraphFrame> { }
	#region SpecialAreaBox
	class SpecialAreaBox : LayoutElementBase {
		SpecialTextBox modelBox;
		internal SpecialAreaBox(SpecialTextBox modelBox, LayoutElement parent)
			: base(parent) {
			this.modelBox = modelBox;
		}
		internal SpecialTextBox ModelBox { get { return modelBox; } }
		public override LayoutType Type {
			get { throw new NotImplementedException(); }
		}
		protected override Rectangle GetBounds() {
			throw new NotImplementedException();
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitSpecialAreaBox(this);
		}
	}
	#endregion
	#region CustomMarkBox
	class CustomMarkBox : LayoutElementBase {
		ModelCustomMarkBox modelBox;
		internal CustomMarkBox(ModelCustomMarkBox modelBox, LayoutElement parent)
			: base(parent) {
			this.modelBox = modelBox;
		}
		internal ModelCustomMarkBox ModelBox { get { return modelBox; } }
		public override LayoutType Type {
			get { throw new NotImplementedException(); }
		}
		protected override Rectangle GetBounds() {
			throw new NotImplementedException();
		}
		public override void Accept(LayoutVisitor visitor) {
			visitor.VisitCustomMarkBox(this);
		}
	}
	#endregion
	#region DocumentLayout
	public class DocumentLayout : IDisposable {
		IRichEditDocumentLayoutProvider provider;
		LayoutPageCollection pages;
		bool isDocumentFormattingCompleted;
		Dictionary<int, AutoResetEvent> asyncRequestedPages;
		ManualResetEvent documentFormattedResetEvent;
		internal DocumentLayout(IRichEditDocumentLayoutProvider provider) {
			this.provider = provider;
			this.pages = new LayoutPageCollection();
			this.asyncRequestedPages = new Dictionary<int, AutoResetEvent>();
			SubscribeToEvents();
		}
		#region Events
		#region DocumentLayoutInvalidated
		DocumentLayoutInvalidatedEventHandler documentLayoutInvalidated;
		public event DocumentLayoutInvalidatedEventHandler DocumentLayoutInvalidated { add { documentLayoutInvalidated += value; } remove { documentLayoutInvalidated -= value; } }
		void RaiseDocumentLayoutInvalidated(DocumentLayoutInvalidatedEventArgs args) {
			IsDocumentFormattingCompleted = false;
			if (documentLayoutInvalidated != null)
				documentLayoutInvalidated(this, args);
		}
		#endregion
		#region DocumentFormatted
		EventHandler documentFormatted;
		public event EventHandler DocumentFormatted { add { documentFormatted += value; } remove { documentFormatted -= value; } }
		void RaiseDocumentFormatted() {
			IsDocumentFormattingCompleted = true;
			if (documentFormatted != null)
				documentFormatted(this, EventArgs.Empty);
		}
		#endregion
		#region PageFormatted
		PageFormattedEventHandler pageFormatted;
		public event PageFormattedEventHandler PageFormatted { add { pageFormatted += value; } remove { pageFormatted -= value; } }
		void RaisePageFormatted(PageFormattedEventArgs args) {
			if (pageFormatted != null)
				pageFormatted(this, args);
		}
		#endregion
		void OnDocumentLayoutInvalidated(object sender, DocumentLayoutInvalidatedEventArgs e) {
			DocumentFormattedResetEvent.Reset();
			for (int i = Pages.Count - 1; i >= e.PageIndex; i--)
				Pages.RemoveAt(i);
			RaiseDocumentLayoutInvalidated(e);
		}
		void OnDocumentFormatted(object sender, EventArgs e) {
			DocumentFormattedResetEvent.Set();
			RaiseDocumentFormatted();
		}
		void OnPageFormatted(object sender, PageFormattedEventArgs e) {
			RaisePageFormatted(e);
			lock (this.asyncRequestedPages) {
				foreach (int key in AsyncRequestedPages.Keys) {
					if (key <= e.PageIndex) {
						ModelDocumentLayout documentLayout = Provider.GetDocumentLayoutAsync();
						Page page = documentLayout.Pages[key];
						if (!page.SecondaryFormattingComplete)
							Provider.PerformPageSecondaryFormatting(page);
						AsyncRequestedPages[key].Set();
					}
				}
			}
		}
		#endregion
		#region Properties
		internal IRichEditDocumentLayoutProvider Provider { get { return provider; } }
		protected LayoutPageCollection Pages { get { return pages; } }
		public bool IsDocumentFormattingCompleted { get { return isDocumentFormattingCompleted; } private set { isDocumentFormattingCompleted = value; } }
		NativeDocument Document { get { return (NativeDocument)Provider.Document; } }
		protected Dictionary<int, AutoResetEvent> AsyncRequestedPages { get { return asyncRequestedPages; } }
		internal ManualResetEvent DocumentFormattedResetEvent {
			get {
				if (documentFormattedResetEvent == null)
					documentFormattedResetEvent = new ManualResetEvent(false);
				return documentFormattedResetEvent;
			}
		}
		#endregion
		void SubscribeToEvents() {
			Provider.DocumentLayoutInvalidated += OnDocumentLayoutInvalidated;
			Provider.DocumentFormatted += OnDocumentFormatted;
			Provider.PageFormatted += OnPageFormatted;
		}
		void UnsubscribeFromEvents() {
			Provider.DocumentLayoutInvalidated -= OnDocumentLayoutInvalidated;
			Provider.DocumentFormatted -= OnDocumentFormatted;
			Provider.PageFormatted -= OnPageFormatted;
		}
		void InitializePages(ModelDocumentLayout documentLayout, int pageIndex) {
			lock (this.pages) {
				for (int i = Pages.Count; i <= pageIndex; i++)
					Pages.Add(new LayoutPage(documentLayout.Pages[i], Document));
			}
		}
		public LayoutPage GetPage(int pageIndex) {
			if (Provider.LayoutCalculationMode == CalculationModeType.Automatic) {
				LayoutPage resultPage = null;
#if DXPORTABLE
				Task task = GetPageAsync(pageIndex, new Action<LayoutPage>(page => { resultPage = page; }));
				task.Wait();
#else
				using (Task task = GetPageAsync(pageIndex, new Action<LayoutPage>(page => { resultPage = page; }))) {
					task.Wait();
				}
#endif
				return resultPage;
			}
			ModelDocumentLayout documentLayout = Provider.GetDocumentLayout();
			if (pageIndex >= documentLayout.Pages.Count)
				return null;
			return GetPageCore(pageIndex, documentLayout);
		}
		LayoutPage GetPageCore(int pageIndex, ModelDocumentLayout documentLayout) {
			if (pageIndex < Pages.Count)
				return Pages[pageIndex];
			InitializePages(documentLayout, pageIndex);
			return Pages[pageIndex];
		}
		public Task GetPageAsync(int pageIndex, Action<LayoutPage> callback) {
			return GetPageAsync(pageIndex, callback, CancellationToken.None);
		}
		public Task GetPageAsync(int pageIndex, Action<LayoutPage> callback, CancellationToken cancellationToken) {
			if (callback == null)
				ThrowNullCallbackException();
			if (Provider.LayoutCalculationMode == CalculationModeType.Manual)
				ThrowInvalidAsyncOperationException();
			LayoutPage formattedPage = TryGetPageAsyncSynchronously(pageIndex);
			if (formattedPage != null) {
				callback(formattedPage);
				return GetComletedTask();
			}
			AutoResetEvent waitEvent;
			lock (this.asyncRequestedPages) {
				if (!AsyncRequestedPages.ContainsKey(pageIndex)) {
					waitEvent = new AutoResetEvent(false);
					AsyncRequestedPages.Add(pageIndex, waitEvent);
				}
				else
					waitEvent = AsyncRequestedPages[pageIndex];
			}
			Action action = new Action(() => {
				WaitHandle[] waitHandles;
				if (cancellationToken != CancellationToken.None)
					waitHandles = new WaitHandle[] { waitEvent, DocumentFormattedResetEvent, cancellationToken.WaitHandle };
				else
					waitHandles = new WaitHandle[] { waitEvent, DocumentFormattedResetEvent };
				int waitHandleIndex = WaitHandle.WaitAny(waitHandles);
				lock (this.asyncRequestedPages) {
					if (AsyncRequestedPages.ContainsKey(pageIndex)) {
						AsyncRequestedPages[pageIndex].Dispose();
						AsyncRequestedPages.Remove(pageIndex);
					}
				}
				if (waitHandleIndex == 2 || cancellationToken.IsCancellationRequested)
					return;
				if (waitHandleIndex == 1 && pageIndex >= GetFormattedPageCount()) {
					callback(null);
					return;
				}
				ModelDocumentLayout documentLayout = Provider.GetDocumentLayoutAsync();
				InitializePages(documentLayout, pageIndex);
				callback(Pages[pageIndex]);
			});
			return Task.Factory.StartNew(action, cancellationToken);
		}
		Task GetComletedTask() {
			TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
			source.SetResult(false);
			return source.Task;
		}
		internal virtual void ThrowNullCallbackException() {
			throw new ArgumentNullException("callback");
		}
		internal virtual void ThrowInvalidAsyncOperationException() {
			throw new InvalidOperationException("Cannot obtain the page because the layout is not calculated. Set LayoutCalculationMode to Automatic to perform calculation.");
		}
		LayoutPage TryGetPageAsyncSynchronously(int pageIndex) {
			if (pageIndex < Pages.Count)
				return Pages[pageIndex];
			ModelDocumentLayout modelDocumentLayout = Provider.GetDocumentLayoutAsync();
			if (modelDocumentLayout == null || pageIndex >= modelDocumentLayout.Pages.Count || !modelDocumentLayout.Pages[pageIndex].PrimaryFormattingComplete)
				return null;
			if (!modelDocumentLayout.Pages[pageIndex].SecondaryFormattingComplete)
				Provider.PerformPageSecondaryFormatting(modelDocumentLayout.Pages[pageIndex]);
			return GetPageCore(pageIndex, modelDocumentLayout);
		}
		#region GetPageIndex
		public int GetPageIndex(LayoutElement element) {
			LayoutPage page = element as LayoutPage;
			if (page == null)
				page = element.GetParentByType<LayoutPage>();
			return page.Index;
		}
		#endregion
		#region GetFormattedPageCount
		public int GetFormattedPageCount() {
			ModelDocumentLayout documentLayout = Provider.GetDocumentLayoutAsync();
			return documentLayout != null ? documentLayout.Pages.Count : 0;
		}
		#endregion
		#region GetPageCount
		public int GetPageCount() {
			if (Provider.LayoutCalculationMode == CalculationModeType.Manual) {
				ModelDocumentLayout documentLayout = Provider.GetDocumentLayout();
				return documentLayout != null ? documentLayout.Pages.Count : 0;
			}
			if (IsDocumentFormattingCompleted)
				return GetFormattedPageCount();
			DocumentFormattedResetEvent.WaitOne();
			return GetFormattedPageCount();
		}
		#endregion
		#region GetText
		public static string GetText(LayoutElement element) {
			TextCollectorVisitor visitor = new TextCollectorVisitor();
			visitor.Visit(element);
			return visitor.Text;
		}
		#endregion
#if !DXPORTABLE
		#region Split
		public CharacterBoxCollection Split(PlainTextBox box) {
			DevExpress.XtraRichEdit.Layout.Engine.BoxMeasurer measurer = null;
			Graphics graphics = null;
			if (Provider.LayoutCalculationMode == CalculationModeType.Manual) {
				graphics = DevExpress.XtraPrinting.Native.GraphicsHelper.CreateGraphicsWithoutAspCheck();
				measurer = new GdiBoxMeasurer(Document.DocumentModel, graphics);
			}
			else {
				ModelDocumentLayout modelLayout = Provider.GetDocumentLayout();
				measurer = modelLayout.Measurer;
			}
			try {
				return SplitPlainTextBoxCore(box, measurer);
			}
			finally {
				if (Provider.LayoutCalculationMode == CalculationModeType.Manual) {
					measurer.Dispose();
					graphics.Dispose();
				}
			}
		}
		CharacterBoxCollection SplitPlainTextBoxCore(PlainTextBox box, DevExpress.XtraRichEdit.Layout.Engine.BoxMeasurer measurer) {
			XtraRichEdit.Layout.CharacterBoxCollection boxes = new XtraRichEdit.Layout.CharacterBoxCollection();
			using (CharacterBoxLevelDocumentLayoutExporter characterExporter = new CharacterBoxLevelDocumentLayoutExporter(Document.DocumentModel, boxes, measurer)) {
				characterExporter.PieceTable = box.GetParentByType<IPieceTableContainer>().PieceTable;
				characterExporter.ExportTextBox((XtraRichEdit.Layout.TextBox)box.ModelBox);
				CharacterBoxCollection result = new CharacterBoxCollection();
				for (int i = 0; i < boxes.Count; i++) {
					CharacterBox characterBox = new CharacterBox(boxes[i], box);
					Rectangle bounds = characterBox.Bounds;
					if (characterExporter.PieceTable.IsComment) {
						LayoutComment comment = box.GetParentByType<LayoutComment>();
						bounds.Offset(comment.ContentBounds.Location.X, 0);
					}
					bounds.Y = box.Bounds.Y;
					characterBox.SetBounds(bounds);
					result.Add(characterBox);
				}
				return result;
			}
		}
		#endregion
#endif
		public T GetElement<T>(DocumentPosition position) where T : RangedLayoutElement {
			Func<RangedLayoutElement, bool> comparison = element => { return element is T; };
			return (T)GetElementCore(position, comparison);
		}
		public RangedLayoutElement GetElement(DocumentPosition position, LayoutType type) {
			if (!IsRangeSupported(type))
				throw new ArgumentException("The specified layout type cannot be related to a document position.", "type");
			Func<RangedLayoutElement, bool> comparison = element => { return element.Type == type; };
			return GetElementCore(position, comparison);
		}
		bool IsRangeSupported(LayoutType type) {
			switch (type) {
				case LayoutType.Page:
					return false;
				case LayoutType.FloatingPicture:
					return false;
				case LayoutType.NumberingListMarkBox:
					return false;
				case LayoutType.NumberingListWithSeparatorBox:
					return false;
				case LayoutType.BookmarkStartBox:
					return false;
				case LayoutType.BookmarkEndBox:
					return false;
				case LayoutType.RangePermissionStartBox:
					return false;
				case LayoutType.RangePermissionEndBox:
					return false;
				case LayoutType.CommentStartBox:
					return false;
				case LayoutType.CommentEndBox:
					return false;
				case LayoutType.LineNumberBox:
					return false;
				default:
					return true;
			}
		}
		RangedLayoutElement GetElementCore(DocumentPosition position, Func<RangedLayoutElement, bool> comparison) {
			int pageIndex = 0;
			NativeDocumentPosition nativePosition = (NativeDocumentPosition)position;
			LayoutElementSearcher searcher = new LayoutElementSearcher(nativePosition.ToInt(), nativePosition.Position.PieceTable, comparison);
			while (true) {
				LayoutPage page = GetPage(pageIndex);
				if (page == null)
					return null;
				searcher.Visit(page);
				if (searcher.FindedElement != null)
					return searcher.FindedElement;
				pageIndex++;
			}
		}
		static internal T GetElementFromPage<T>(int position, PieceTable pieceTable, LayoutPage page) where T : RangedLayoutElement {
			Func<LayoutElement, bool> comparison = element => { return element is T; };
			LayoutElementSearcher searcher = new LayoutElementSearcher(position, pieceTable, comparison);
			searcher.Visit(page);
			return (T)searcher.FindedElement;
		}
		public void Dispose() {
			UnsubscribeFromEvents();
			if (this.documentFormattedResetEvent != null) {
				this.documentFormattedResetEvent.Dispose();
				this.documentFormattedResetEvent = null;
			}
		}
	}
	#endregion
	#region LayoutRowExporter
	class LayoutRowExporter : IDocumentLayoutExporter {
		LayoutRow row;
		BoxCollection boxes;
		public LayoutRowExporter(LayoutRow row) {
			this.row = row;
			this.boxes = new BoxCollection();
		}
		public BoxCollection Boxes { get { return boxes; } }
		#region IDocumentLayoutExporter Members
		public void ExportRow(Row row) {
			for (int i = 0; i < row.Boxes.Count; i++) {
				AddAnchorBox(row.Boxes[i], row.Paragraph.BoxCollection);
				row.Boxes[i].ExportTo(this);
			}
		}
		void AddAnchorBox(ModelBox box, DevExpress.XtraRichEdit.Layout.Engine.ParagraphBoxCollection collection) {
			if (!collection.ContainsFloatingObjectAnchorBox)
				return;
			int boxIndex = GetBoxIndex(box, collection);
			if (boxIndex <= 0)
				return;
			List<FloatingObjectAnchorBox> inverseList = new List<FloatingObjectAnchorBox>();
			for (int i = boxIndex - 1; i >= 0; i--) {
				ModelFloatingObjectAnchorBox modelBox = collection[i] as ModelFloatingObjectAnchorBox;
				if (modelBox == null)
					break;
				inverseList.Add(new FloatingObjectAnchorBox(modelBox, new Rectangle(collection[boxIndex].Bounds.Location, modelBox.Bounds.Size), this.row));
			}
			if (inverseList.Count == 0)
				return;
			for (int i = inverseList.Count - 1; i >= 0; i--)
				Boxes.Add(inverseList[i]);
		}
		int GetBoxIndex(ModelBox box, DevExpress.XtraRichEdit.Layout.Engine.ParagraphBoxCollection collection) {
			for (int i = 0; i < collection.Count; i++) {
				if (Object.ReferenceEquals(box, collection[i]) || (box.StartPos == collection[i].StartPos && box.EndPos == collection[i].EndPos))
					return i;
			}
			return -1;
		}
		public void ExportTextBox(XtraRichEdit.Layout.TextBox box) {
			PlainTextBox plainTextBox = new PlainTextBox(box, LayoutType.PlainTextBox, row);
			Rectangle actualBounds = plainTextBox.Bounds;
			actualBounds.Y += GetVerticalBoxOffset(plainTextBox);
			plainTextBox.SetBounds(actualBounds);
			Boxes.Add(plainTextBox);
		}
		public void ExportSpecialTextBox(SpecialTextBox box) {
			PlainTextBox specialTextBox = new PlainTextBox(box, LayoutType.SpecialTextBox, row);
			Rectangle actualBounds = specialTextBox.Bounds;
			actualBounds.Y += GetVerticalBoxOffset(specialTextBox);
			specialTextBox.SetBounds(actualBounds);
			Boxes.Add(specialTextBox);
		}
		public void ExportLayoutDependentTextBox(LayoutDependentTextBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.PageNumberBox, row));
		}
		public void ExportHyphenBox(HyphenBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.HyphenBox, row));
		}
		public void ExportInlinePictureBox(ModelInlinePictureBox box) {
			Boxes.Add(new InlinePictureBox(box, row));
		}
		public void ExportSpaceBox(ModelBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.SpaceBox, row));
		}
		public void ExportTabSpaceBox(TabSpaceBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.TabSpaceBox, row));
		}
		public void ExportNumberingListBox(NumberingListBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.NumberingListMarkBox, row));
		}
		public void ExportLineBreakBox(LineBreakBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.LineBreakBox, row));
		}
		public void ExportPageBreakBox(PageBreakBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.PageBreakBox, row));
		}
		public void ExportColumnBreakBox(ColumnBreakBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.ColumnBreakBox, row));
		}
		public void ExportParagraphMarkBox(ParagraphMarkBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.ParagraphMarkBox, row));
		}
		public void ExportSectionMarkBox(SectionMarkBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.SectionBreakBox, row));
		}
		public void ExportCustomRunBox(CustomRunBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.CustomRunBox, row));
		}
		public void ExportSeparatorBox(SeparatorBox box) {
		}
		public void ExportDataContainerRunBox(DataContainerRunBox box) {
			Boxes.Add(new PlainTextBox(box, LayoutType.DataContainerRunBox, row));
		}
		public void ExportLineNumberBox(ModelLineNumberBox box) { }
		public void ExportPage(Page page) { }
		public void ExportParagraphFramePage(Page page, RectangleF pageClipBounds, bool exportContent) { }
		public void ExportPageArea(PageArea pageArea) { }
		public void ExportColumn(Column column) { }
		public void ExportFloatingObjectBox(FloatingObjectBox box) { }
		public void ExportParagraphFrameBox(ParagraphFrameBox box) { }
		public void ExportUnderlineBox(Row row, ModelUnderlineBox underlineBox) { }
		public void ExportStrikeoutBox(Row row, ModelUnderlineBox strikeoutBox) { }
		public void ExportErrorBox(ModelErrorBox errorBox) { }
		public void ExportBookmarkStartBox(VisitableDocumentIntervalBox box) { }
		public void ExportBookmarkEndBox(VisitableDocumentIntervalBox box) { }
		public void ExportCommentStartBox(VisitableDocumentIntervalBox box) { }
		public void ExportCommentEndBox(VisitableDocumentIntervalBox box) { }
		public void ExportTableBorder(TableBorderViewInfoBase border, Rectangle cellBounds) { }
		public void ExportTableBorderCorner(CornerViewInfoBase corner, int x, int y) { }
		public void ExportTableCell(TableCellViewInfo cell) { }
		public void ExportTableRow(TableRowViewInfoBase row) { }
		public void ExportCustomMarkBox(ModelCustomMarkBox box) { }
		public bool IsAnchorVisible(ITableCellVerticalAnchor anchor) {
			throw new NotImplementedException();
		}
		public bool IsTableRowVisible(TableRowViewInfoBase row) {
			throw new NotImplementedException();
		}
		int GetVerticalBoxOffset(Box box) {
			PieceTable pieceTable = box.GetParentByType<IPieceTableContainer>().PieceTable;
			DevExpress.Office.Drawing.FontInfo fontInfo = box.ModelBox.GetFontInfo(pieceTable);
			return fontInfo.Free;
		}
		#endregion
	}
	#endregion
	#region DocumentLayoutHelper
	class DocumentLayoutHelper {
		#region InitializeLayoutRowCollection
		public static LayoutRowCollection InitializeLayoutRowCollection(RowCollection modelRows, LayoutElement parent, bool isTableCellRow) {
			LayoutRowCollection layoutRows = new LayoutRowCollection();
			if (isTableCellRow)
				InitializeTableCellRowCollection(modelRows, layoutRows, parent);
			else
				InitializeRowCollection(modelRows, layoutRows, parent);
			return layoutRows;
		}
		static void InitializeRowCollection(RowCollection modelRows, LayoutRowCollection layoutRows, LayoutElement parent) {
			for (int i = 0; i < modelRows.Count; i++) {
				if (!modelRows[i].IsTabelCellRow)
					layoutRows.Add(new LayoutRow(modelRows[i], parent));
			}
		}
		static void InitializeTableCellRowCollection(RowCollection modelRows, LayoutRowCollection layoutRows, LayoutElement parent) {
			LayoutTable table = parent.GetParentByType<LayoutTable>();
			for (int i = 0; i < modelRows.Count; i++) {
				TableCellRow row = (TableCellRow)modelRows[i];
				if (row.IsTabelCellRow && Object.ReferenceEquals(table.ModelTable, row.CellViewInfo.TableViewInfo))
					layoutRows.Add(new LayoutRow(row, parent));
			}
		}
		#endregion
		#region InitializeLayoutTableCollection
		public static LayoutTableCollection InitializeLayoutTableCollection(TableViewInfoCollection modelTables, LayoutElement parent) {
			LayoutTableCollection layoutTables = new LayoutTableCollection();
			for (int i = 0; i < modelTables.Count; i++) {
				if (modelTables[i].ParentTableCellViewInfo == null)
					layoutTables.Add(new LayoutTable(modelTables[i], parent));
			}
			return layoutTables;
		}
		#endregion
		#region CreateBorders
		public static Borders CreateBorders(LayoutElement element, BorderBase leftBorder, BorderBase rightBorder, BorderBase topBorder, BorderBase bottomBorder) {
			IPieceTableContainer container = element.GetParentByType<IPieceTableContainer>();
			int leftBorderThickness = container.PieceTable.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(leftBorder.Width);
			int rightBorderThickness = container.PieceTable.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(rightBorder.Width);
			int topBorderThickness = container.PieceTable.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(topBorder.Width);
			int bottomBorderThickness = container.PieceTable.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(bottomBorder.Width);
			LayoutBorder left = new LayoutBorder(leftBorder.Color, (TableBorderLineStyle)leftBorder.Style, leftBorderThickness);
			LayoutBorder right = new LayoutBorder(rightBorder.Color, (TableBorderLineStyle)rightBorder.Style, rightBorderThickness);
			LayoutBorder top = new LayoutBorder(topBorder.Color, (TableBorderLineStyle)topBorder.Style, topBorderThickness);
			LayoutBorder bottom = new LayoutBorder(bottomBorder.Color, (TableBorderLineStyle)bottomBorder.Style, bottomBorderThickness);
			return new Borders(left, right, top, bottom);
		}
		#endregion
		#region GetRangeForTablesAndRowsContainer
		public static FixedRange GetRangeForTablesAndRowsContainer(LayoutRowCollection rows, LayoutTableCollection tables) {
			if (rows.Count <= 0)
				return new FixedRange(tables[0].Range.Start, tables[0].Range.Length);
			int fistRowRangeStart = rows[0].Range.Start;
			FixedRange lastRowRange = rows.Last.Range;
			int lastRowRangeEnd = lastRowRange.Start + lastRowRange.Length;
			int start = tables.Count > 0 ? Math.Min(fistRowRangeStart, tables[0].Range.Start) : fistRowRangeStart;
			int end = tables.Count > 0 ? Math.Max(lastRowRangeEnd, tables.Last.Range.Start + tables.Last.Range.Length) : lastRowRangeEnd;
			return new FixedRange(start, end - start);
		}
		#endregion
		#region GetRangeFromRowByCoordinates
		public static FixedRange GetRangeFromRowByCoordinates(LayoutRow row, int x1, int x2) {
			Box startBox = null;
			Box endBox = null;
			for (int i = 0; i < row.Boxes.Count; i++) {
				if (startBox != null && endBox != null)
					break;
				Box box = row.Boxes[i];
				if (box.Type == LayoutType.FloatingObjectAnchorBox)
					continue;
				if (box.Bounds.X == x1)
					startBox = box;
				if (box.Bounds.X + box.Bounds.Width == x2)
					endBox = box;
			}
			return new FixedRange(startBox.Range.Start, endBox.Range.Start + endBox.Range.Length - startBox.Range.Start);
		}
		#endregion
	}
	#endregion
	public class DocumentLayoutInvalidatedEventArgs : EventArgs {
		int pageIndex;
		public DocumentLayoutInvalidatedEventArgs(int pageIndex) {
			this.pageIndex = pageIndex;
		}
		public int PageIndex { get { return pageIndex; } }
	}
	public delegate void DocumentLayoutInvalidatedEventHandler(object sender, DocumentLayoutInvalidatedEventArgs e);
	public class PageFormattedEventArgs : EventArgs {
		int pageIndex;
		public PageFormattedEventArgs(int pageIndex) {
			this.pageIndex = pageIndex;
		}
		public int PageIndex { get { return pageIndex; } }
	}
	public delegate void PageFormattedEventHandler(object sender, PageFormattedEventArgs e);
}

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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Compatibility.System.Drawing;
using LayoutUnit = System.Int32;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout {
	#region Column
	public class Column : NoPositionBox {
		#region Fields
		readonly RowCollection rows;
		ColumnExtendedBoxes extendedBoxes;
		#endregion
		public Column() {
			this.rows = new RowCollection();
		}
		#region Properties
		public override bool IsVisible { get { return true; } }
		public virtual RowCollection Rows { get { return rows; } }
		protected internal ColumnExtendedBoxes InnerExtendedBoxes { get { return extendedBoxes; } }
		public ColumnExtendedBoxes ExtendedBoxes {
			get {
				if (extendedBoxes == null)
					extendedBoxes = new ColumnExtendedBoxes();
				return extendedBoxes;
			}
		}
		protected internal bool ShouldProcessTables { get { return InnerTables != null && InnerTables.Count > 0; } }
		protected internal TableViewInfoCollection InnerTables {
			get {
				if (InnerExtendedBoxes == null)
					return null;
				return InnerExtendedBoxes.InnerTables;
			}
		}
		public ParagraphFrameBoxCollection ParagraphFrames { get { return ExtendedBoxes.ParagraphFrames; } }
		protected internal bool ShouldProcessParagraphFrames { get { return InnerParagraphFrames != null && InnerParagraphFrames.Count > 0; } }
		protected internal ParagraphFrameBoxCollection InnerParagraphFrames {
			get {
				if (InnerExtendedBoxes == null)
					return null;
				return InnerExtendedBoxes.InnerParagraphFrames;
			}
		}
		public TableViewInfoCollection Tables { get { return ExtendedBoxes.Tables; } }
		public bool IsEmpty { get { return rows.Count <= 0; } }
		protected internal override DocumentLayoutDetailsLevel DetailsLevel { get { return DocumentLayoutDetailsLevel.Column; } }
		protected internal override HitTestAccuracy HitTestAccuracy { get { return HitTestAccuracy.ExactColumn; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		public virtual Column TopLevelColumn { get { return this; } }
		#endregion
		public override Box CreateBox() {
			Exceptions.ThrowInternalException();
			return new Column();
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportColumn(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreateColumnHitTestManager(this);
		}
		public override FormatterPosition GetFirstFormatterPosition() {
			if (IsEmpty)
				return FormatterPosition.MaxValue;
			else
				return Rows.First.GetFirstFormatterPosition();
		}
		public override FormatterPosition GetLastFormatterPosition() {
			if (IsEmpty)
				return FormatterPosition.MaxValue;
			else
				return Rows.Last.GetLastFormatterPosition();
		}
		public override DocumentModelPosition GetFirstPosition(PieceTable pieceTable) {
			if (IsEmpty)
				return DocumentModelPosition.MaxValue;
			else
				return Rows.First.GetFirstPosition(pieceTable);
		}
		public override DocumentModelPosition GetLastPosition(PieceTable pieceTable) {
			if (IsEmpty)
				return DocumentModelPosition.MaxValue;
			else
				return Rows.Last.GetLastPosition(pieceTable);
		}
		public override void MoveVertically(int deltaY) {
			base.MoveVertically(deltaY);
			Rows.MoveVertically(deltaY);
			if (InnerTables != null)
				InnerTables.MoveVertically(deltaY);
			if (InnerParagraphFrames != null)
				InnerParagraphFrames.MoveVertically(deltaY);
		}
#if DEBUG
		public override string ToString() {
			return String.Format("Column. Bounds: {0}", Bounds);
		}
#endif
		public void RemoveTableViewInfoWithContent(TableViewInfo tableViewInfo) {
			tableViewInfo.RemoveRowsFromIndex(0);
			if (InnerTables != null)
				InnerTables.Remove(tableViewInfo);
		}
		public void RemoveTableCellViewInfoContent(TableCellViewInfo cell) {
			int rowCount = rows.Count;
			for (int i = rowCount - 1; i >= 0; i--) {
				TableCellRow row = rows[i] as TableCellRow;
				if (row != null && row.CellViewInfo == cell)
					rows.RemoveAt(i);
			}
			ClearInvalidatedParagraphFramesCore(cell.Cell.StartParagraphIndex, cell.Cell.EndParagraphIndex);
		}
		public bool IsIntersectedWithPrevColumn() {
			int rowCount = rows.Count;
			if (rowCount == 0) {
				Debug.Assert(false);
				return false;
			}
			TableCellRow firstRow = rows[0] as TableCellRow;
			if (firstRow == null)
				return false;
			TableViewInfo topLevelTableViewInfo = firstRow.CellViewInfo.TableViewInfo;
			while (topLevelTableViewInfo.ParentTableCellViewInfo != null)
				topLevelTableViewInfo = topLevelTableViewInfo.ParentTableCellViewInfo.TableViewInfo;
			TableRowViewInfoBase firstTableRow = topLevelTableViewInfo.Rows.First;
			TableCellViewInfoCollection cells = firstTableRow.Cells;
			int cellCount = cells.Count;
			for (int i = 0; i < cellCount; i++) {
				if (cells[i].IsStartOnPreviousTableViewInfo())
					return true;
			}
			return false;
		}
		public override string GetText(PieceTable table) {
			Exceptions.ThrowInternalException();
			return String.Empty;
		}
		public override TextRunBase GetRun(PieceTable pieceTable) {
			Exceptions.ThrowInternalException();
			return null;
		}
		public virtual RowCollection GetOwnRows() {
			return Rows;
		}
		public virtual void AddParagraphFrame(Paragraph paragraph) {
			if (!ContainsParagraphFrame(paragraph))
				ParagraphFrames.Add(new ParagraphFrameBox(paragraph.PieceTable, paragraph.Index));
		}
		bool ContainsParagraphFrame(Paragraph paragraph) {
			int count = ParagraphFrames.Count;
			for (int i = 0; i < count; i++) {
				if (ParagraphFrames[i].ParagraphIndex == paragraph.Index && ParagraphFrames[i].PieceTable == paragraph.PieceTable)
					return true;
			}
			return false;
		}
		protected internal virtual void ClearInvalidatedParagraphFrames(ParagraphIndex from) {
			ClearInvalidatedParagraphFramesCore(from, ParagraphIndex.MaxValue);
		}
		protected virtual void ClearInvalidatedParagraphFramesCore(ParagraphIndex from, ParagraphIndex to) {
			if (InnerParagraphFrames != null)
				InnerParagraphFrames.ClearInvalidatedParagraphFrames(from, to);
		}
		public void RemoveEmptyTableViewInfos() {
			if (InnerTables == null)
				return;
			bool tableRemoved;
			do {
				tableRemoved = false;
				int count = InnerTables.Count;
				for (int i = count - 1; i > 0; i--) {
					TableViewInfo viewInfo = InnerTables[i];
					viewInfo.RemoveEmptyCells();
					if (viewInfo.Cells.Count == 0) {
						InnerTables.RemoveAt(i);
						tableRemoved = true;
					}
				}
			} while (tableRemoved);
		}
	}
	#endregion
	#region ColumnCollection
	public class ColumnCollection : BoxCollectionBase<Column> {
		protected internal override void RegisterSuccessfullItemHitTest(BoxHitTestCalculator calculator, Column item) {
			calculator.Result.Column = item;
			calculator.Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Column);
		}
		protected internal override void RegisterFailedItemHitTest(BoxHitTestCalculator calculator) {
			calculator.Result.Column = null;
		}
	}
	#endregion
	#region TableCellColumn
	public class TableCellColumn : Column {
		readonly Column parent;
		readonly TableCell cell;
		public TableCellColumn(Column parent, TableCell cell) {
			Guard.ArgumentNotNull(parent, "parent");
			this.parent = parent;
			this.cell = cell;
		}
		public override RowCollection Rows { get { return parent.Rows; } }
		public virtual Column Parent { get { return parent; } }
		public override Column TopLevelColumn { get { return Parent.TopLevelColumn; } }
		protected TableCell Cell { get { return cell; } }
		public override RowCollection GetOwnRows() {
			return TableCellViewInfo.GetRows(Rows, Cell);
		}
		public override void AddParagraphFrame(Paragraph paragraph) {
			TopLevelColumn.AddParagraphFrame(paragraph);
		}
	}
	#endregion
	#region ColumnExtendedBoxes
	public class ColumnExtendedBoxes {
		TableViewInfoCollection tables;
		ParagraphFrameBoxCollection paragraphFrames;
		protected internal TableViewInfoCollection InnerTables { get { return tables; } }
		public TableViewInfoCollection Tables {
			get {
				if (tables == null)
					tables = new TableViewInfoCollection();
				return tables;
			}
		}
		protected internal ParagraphFrameBoxCollection InnerParagraphFrames { get { return paragraphFrames; } }
		public ParagraphFrameBoxCollection ParagraphFrames {
			get {
				if (paragraphFrames == null)
					paragraphFrames = new ParagraphFrameBoxCollection();
				return paragraphFrames;
			}
		}
	}
	#endregion
	#region ParagraphFrameBox
	public class ParagraphFrameBox : SinglePositionBox, ILayoutRectangularFloatingObject {
		#region Fields
		bool canPutTextAtLeft;
		bool canPutTextAtRight;
		bool putTextAtLargestSide;
		bool lockPosition;
		Rectangle extendedBounds;
		Rectangle contentBounds;
		Rectangle actualSizeBounds;
		Rectangle actualBounds;
		Rectangle shapeBounds;
		PieceTable pieceTable;
		DocumentLayout documentLayout;
		Row firstRow;
		RowCollection rowCollection;
		ParagraphIndex paragraphIndex;
		#endregion
		public ParagraphFrameBox(Paragraph paragraph)
			: this(paragraph.PieceTable, paragraph.Index) {
		}
		public ParagraphFrameBox(PieceTable pieceTable, ParagraphIndex paragraphIndex) {
			this.pieceTable = pieceTable;
			this.paragraphIndex = paragraphIndex;
			this.rowCollection = new RowCollection();
		}
		#region Properties
		public override bool IsVisible { get { return true; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		public bool CanPutTextAtLeft { get { return canPutTextAtLeft; } set { canPutTextAtLeft = value; } }
		public bool CanPutTextAtRight { get { return canPutTextAtRight; } set { canPutTextAtRight = value; } }
		public bool PutTextAtLargestSide { get { return putTextAtLargestSide; } set { putTextAtLargestSide = value; } }
		public Rectangle ExtendedBounds { get { return extendedBounds; } set { extendedBounds = value; } }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public override Rectangle ActualSizeBounds { get { return actualSizeBounds; } }
		public Rectangle ActualBounds { get { return actualBounds; } set { actualBounds = value; } }
		public Rectangle ShapeBounds { get { return shapeBounds; } set { shapeBounds = value; } }
		public PieceTable PieceTable { get { return pieceTable; } set { pieceTable = value; } }
		public DocumentLayout DocumentLayout { get { return documentLayout; } set { documentLayout = value; } }
		Rectangle ILayoutRectangularFloatingObject.Bounds { get { return extendedBounds; } }
		public int X { get { return extendedBounds.X; } set { extendedBounds.X = value; } }
		public int Y { get { return extendedBounds.Y; } set { extendedBounds.Y = value; } }
		public bool LockPosition { get { return lockPosition; } set { lockPosition = value; } }
		public bool WasRestart { get; set; }
		public MergedFrameProperties FrameProperties { get { return GetParagraph().GetMergedFrameProperties(); } }
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		public Row FirstRow { get { return firstRow; } set { firstRow = value; } }
		public RowCollection RowCollection { get { return rowCollection; } }
		#endregion
		public override Box CreateBox() {
			return new ParagraphFrameBox(PieceTable, ParagraphIndex);
		}
		internal void SetActualSizeBounds(Rectangle bounds) {
			this.actualSizeBounds = bounds;
		}
		public override void MoveVertically(int deltaY) {
			if (FrameProperties == null) {
				base.MoveVertically(deltaY);
				return;
			}
			if (FrameProperties.Info.VerticalPositionAlignment != ParagraphFrameVerticalPositionAlignment.None)
				return;
			base.MoveVertically(deltaY);
			extendedBounds.Y += deltaY;
			contentBounds.Y += deltaY;
			actualSizeBounds.Y += deltaY;
			if (documentLayout != null && documentLayout.Pages.Count > 0)
				documentLayout.Pages[0].MoveVertically(deltaY);
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportParagraphFrameBox(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			throw new NotImplementedException();
		}
		public static Matrix CreateTransformUnsafe(float angle, Rectangle bounds) {
			if ((angle % 360f) == 0)
				return null;
			Matrix transform = new Matrix();
			transform.RotateAt(angle, RectangleUtils.CenterPoint(bounds));
			return transform;
		}
		public Matrix CreateBackwardTransformUnsafe() {
			return CreateTransformUnsafe(-PieceTable.DocumentModel.GetBoxEffectiveRotationAngleInDegrees(this), ActualSizeBounds);
		}
		public Matrix CreateTransformUnsafe() {
			return CreateTransformUnsafe(PieceTable.DocumentModel.GetBoxEffectiveRotationAngleInDegrees(this), ActualSizeBounds);
		}
		public Point TransformPointBackward(Point point) {
			Matrix transform = CreateBackwardTransformUnsafe();
			if (transform == null)
				return point;
			else
				return transform.TransformPoint(point);
		}
		public bool IsInCell() {
			return GetParagraph().IsInCell();
		}
		public Paragraph GetParagraph() {
			return PieceTable.Paragraphs[ParagraphIndex];
		}
	}
	#endregion
	#region ParagraphFrameBoxCollection
	public class ParagraphFrameBoxCollection : List<ParagraphFrameBox> {
		public void ExportTo(IDocumentLayoutExporter exporter) {
			int count = Count;
			for (int i = 0; i < count; i++)
				exporter.ExportParagraphFrameBox(this[i]);
		}
		public void MoveVertically(LayoutUnit deltaY) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].MoveVertically(deltaY);
		}
		public void ClearInvalidatedParagraphFrames(ParagraphIndex from, ParagraphIndex to) {
			int firstIndex = Algorithms.BinarySearch(this, new ParagraphFrameBoxAndParagraphIndexComparable(from));
			if (firstIndex < 0) {
				firstIndex = ~firstIndex;
				if (firstIndex >= Count)
					return;
				if (this[firstIndex].ParagraphIndex > to)
					return;
			}
			if (to == ParagraphIndex.MaxValue) {
				RemoveRange(firstIndex, Count - firstIndex);
				return;
			}
			for (int i = Count - 1; i >= firstIndex; i--) {
				if (this[i].ParagraphIndex <= to)
					RemoveAt(i);
			}
		}
	}
	#endregion
	public class ParagraphFrameBoxAndParagraphIndexComparable : IComparable<ParagraphFrameBox> {
		readonly ParagraphIndex paragraphIndex;
		public ParagraphFrameBoxAndParagraphIndexComparable(ParagraphIndex paragraphIndex) {
			this.paragraphIndex = paragraphIndex;
		}
		#region IComparable<ParagraphFrameBox> Members
		public int CompareTo(ParagraphFrameBox other) {
			return Comparer<ParagraphIndex>.Default.Compare(other.ParagraphIndex, paragraphIndex);
		}
		#endregion
	}
}

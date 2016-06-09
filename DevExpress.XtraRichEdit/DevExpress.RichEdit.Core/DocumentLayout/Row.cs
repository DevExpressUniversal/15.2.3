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
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout {
	[Flags]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217")]
	public enum RowProcessingFlags {
		None,
		ProcessCharacterLines = 1,
		ProcessHiddenText = 2,
		ProcessTextHighlight = 4,
		ProcessSpecialTextBoxes = 8,
		ProcessLayoutDependentText = 0x10,
		Clear = ~(ProcessCharacterLines | ProcessHiddenText | ProcessTextHighlight | ProcessSpecialTextBoxes | ProcessLayoutDependentText),
		FirstParagraphRow = 0x20,
		LastParagraphRow = 0x40,
		ContainsFootNotes = 0x80,
		ContainsEndNotes = 0x100,
		LastInvisibleEmptyCellRowAfterNestedTable = 0x200,
	}
	#region Row
	public class Row : NoPositionBox {
		#region Fields
		readonly BoxCollection boxes;
		RowExtendedBoxes extendedBoxes;
		Paragraph paragraph;
		int spaceBefore;
		int baseLineOffset;
		int textOffset;
		int lineNumber;
		int lastParagraphRowOriginalHeight;
		NumberingListBox numberingListBox;
		RowProcessingFlags processingFlags;
		RowFormatterState initialState;
		RowFormatterState nextState;
		#endregion
		public Row() {
			this.boxes = new BoxCollection();
		}
		#region Properties
		public override bool IsVisible { get { return true; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel DetailsLevel { get { return DocumentLayoutDetailsLevel.Row; } }
		protected internal override HitTestAccuracy HitTestAccuracy { get { return HitTestAccuracy.ExactRow; } }
		public BoxCollection Boxes { get { return boxes; } }
		public int TextOffset { get { return textOffset; } set { textOffset = value; } }
		public int SpacingBefore { get { return spaceBefore; } set { spaceBefore = value; } }
		public NumberingListBox NumberingListBox { get { return numberingListBox; } set { numberingListBox = value; } }
		public virtual bool IsTabelCellRow { get { return false; } }
		internal RowFormatterState InitialState { get { return initialState; } set { initialState = value; } }
		internal RowFormatterState NextState { get { return nextState; } set { nextState = value; } }
		#region ExtendedBoxes
		protected internal RowExtendedBoxes ExtendedBoxes {
			get {
				if (extendedBoxes == null)
					extendedBoxes = new RowExtendedBoxes();
				return extendedBoxes;
			}
		}
		protected internal RowExtendedBoxes InnerExtendedBoxes { get { return extendedBoxes; } }
		#endregion
		#region Underlines
		public UnderlineBoxCollection Underlines { get { return ExtendedBoxes.Underlines; } }
		protected internal UnderlineBoxCollection InnerUnderlines { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerUnderlines : null; } }
		protected internal void ClearUnderlines() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearUnderlines();
		}
		#endregion
		#region Strikeouts
		public UnderlineBoxCollection Strikeouts { get { return ExtendedBoxes.Strikeouts; } }
		protected internal UnderlineBoxCollection InnerStrikeouts { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerStrikeouts : null; } }
		protected internal void ClearStrikeouts() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearStrikeouts();
		}
		#endregion
		#region Errors
		public ErrorBoxCollection Errors { get { return ExtendedBoxes.Errors; } }
		protected internal ErrorBoxCollection InnerErrors { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerErrors : null; } }
		protected internal void ClearErrors() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearErrors();
		}
		#endregion
		#region HighlightAreas
		public HighlightAreaCollection HighlightAreas { get { return ExtendedBoxes.HighlightAreas; } }
		protected internal HighlightAreaCollection InnerHighlightAreas { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerHighlightAreas : null; } }
		protected internal void ClearHighlightAreas() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearHighlightAreas();
		}
		#endregion
		#region FieldHighlightAreas
		public HighlightAreaCollection FieldHighlightAreas { get { return ExtendedBoxes.FieldHighlightAreas; } }
		protected internal HighlightAreaCollection InnerFieldHighlightAreas { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerFieldHighlightAreas : null; } }
		protected internal void ClearFieldHighlightAreas() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearFieldHighlightAreas();
		}
		#endregion
		#region RangePermissionHighlightAreas
		public HighlightAreaCollection RangePermissionHighlightAreas { get { return ExtendedBoxes.RangePermissionHighlightAreas; } }
		public HighlightAreaCollection InnerRangePermissionHighlightAreas { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerRangePermissionHighlightAreas : null; } }
		protected internal void ClearRangePermissionHighlightAreas() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearRangePermissionHighlightAreas();
		}
		#endregion
		#region CommentHighlightAreas
		public HighlightAreaCollection CommentHighlightAreas { get { return ExtendedBoxes.CommentHighlightAreas; } }
		public HighlightAreaCollection InnerCommentHighlightAreas { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerCommentHighlightAreas : null; } }
		protected internal void ClearCommentHighlightAreas() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearCommentHighlightAreas();
		}
		#endregion
		#region BookmarkBoxes
		public BookmarkBoxCollection BookmarkBoxes { get { return ExtendedBoxes.BookmarkBoxes; } }
		protected internal BookmarkBoxCollection InnerBookmarkBoxes { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerBookmarkBoxes : null; } }
		protected internal void ClearBookmarkBoxes() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearBookmarkBoxes();
		}
		#endregion
		#region RangePermissionBoxes
		public BookmarkBoxCollection RangePermissionBoxes { get { return ExtendedBoxes.RangePermissionBoxes; } }
		protected internal BookmarkBoxCollection InnerRangePermissionBoxes { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerRangePermissionBoxes : null; } }
		protected internal void ClearRangePermissionsBoxes() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearRangePermissionsBoxes();
		}
		#endregion
		#region CommentBoxes
		public BookmarkBoxCollection CommentBoxes { get { return ExtendedBoxes.CommentBoxes; } }
		protected internal BookmarkBoxCollection InnerCommentBoxes { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerCommentBoxes : null; } }
		protected internal void ClearCommentBoxes() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearCommentBoxes();
		}
		#endregion
		#region HiddenTextBoxes
		public HiddenTextUnderlineBoxCollection HiddenTextBoxes { get { return ExtendedBoxes.HiddenTextBoxes; } }
		protected internal HiddenTextUnderlineBoxCollection InnerHiddenTextBoxes { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerHiddenTextBoxes : null; } }
		protected internal void ClearHiddenTextBoxes() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearHiddenTextBoxes();
		}
		#endregion
		#region CustomMarkBoxes
		public CustomMarkBoxCollection CustomMarkBoxes { get { return ExtendedBoxes.CustomMarkBoxes; } }
		protected internal CustomMarkBoxCollection InnerCustomMarkBoxes { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerCustomMarkBoxes : null; } }
		protected internal void ClearCustomMarkBoxes() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearCustomMarkBoxes();
		}
		#endregion
		#region SpecialTextBoxes
		public SpecialTextBoxCollection SpecialTextBoxes { get { return ExtendedBoxes.SpecialTextBoxes; } }
		protected internal SpecialTextBoxCollection InnerSpecialTextBoxes { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerSpecialTextBoxes : null; } }
		protected internal void ClearSpecialTextBoxes() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearSpecialTextBoxes();
		}
		#endregion
		#region BoxRanges
		public RowBoxRangeCollection BoxRanges { get { return ExtendedBoxes.BoxRanges; } }
		protected internal RowBoxRangeCollection InnerBoxRanges { get { return InnerExtendedBoxes != null ? InnerExtendedBoxes.InnerBoxRanges : null; } }
		protected internal void ClearBoxRanges() {
			if (InnerExtendedBoxes != null)
				InnerExtendedBoxes.ClearBoxRanges();
		}
		#endregion
		public int BaseLineOffset { get { return baseLineOffset; } set { baseLineOffset = value; } }
		public Paragraph Paragraph {
			get { return paragraph; }
			set {
				Guard.ArgumentNotNull(value, "Paragraph");
				paragraph = value;
			}
		}
		public int Height {
			get { return Bounds.Height; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Height", value);
				Bounds = new Rectangle(Bounds.Location, new Size(Bounds.Width, value));
			}
		}
		public int Width {
			get { return Bounds.Width; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Width", value);
				Bounds = new Rectangle(Bounds.Location, new Size(value, Bounds.Height));
			}
		}
		public int LineNumber { get { return lineNumber; } set { lineNumber = value; } }
		public bool LastParagraphRow { get { return LastParagraphRowOriginalHeight != 0; } }
		public int LastParagraphRowOriginalHeight { get { return lastParagraphRowOriginalHeight; } set { lastParagraphRowOriginalHeight = value; } }
		public bool ShouldProcessCharacterLines { get { return (processingFlags & RowProcessingFlags.ProcessCharacterLines) != 0; } }
		public bool ShouldProcessHiddenText { get { return (processingFlags & RowProcessingFlags.ProcessHiddenText) != 0; } }
		public bool ShouldProcessTextHighlight { get { return (processingFlags & RowProcessingFlags.ProcessTextHighlight) != 0; } }
		public bool ShouldProcessLayoutDependentText { get { return (processingFlags & RowProcessingFlags.ProcessLayoutDependentText) != 0; } }
		public bool ShouldProcessSpecialTextBoxes { get { return (processingFlags & RowProcessingFlags.ProcessSpecialTextBoxes) != 0; } }
		public bool ContainsFootNotes { get { return (processingFlags & RowProcessingFlags.ContainsFootNotes) != 0; } }
		public bool ContainsEndNotes { get { return (processingFlags & RowProcessingFlags.ContainsEndNotes) != 0; } }
		public bool IsFirstParagraphRow { get { return (processingFlags & RowProcessingFlags.FirstParagraphRow) != 0; } }
		public bool IsLastParagraphRow { get { return (processingFlags & RowProcessingFlags.LastParagraphRow) != 0; } }
		public RowProcessingFlags ProcessingFlags { get { return processingFlags; } set { processingFlags = value; } }
		#endregion
		public override FormatterPosition GetFirstFormatterPosition() {
			return Boxes.First.GetFirstFormatterPosition();
		}
		public override FormatterPosition GetLastFormatterPosition() {
			return Boxes.Last.GetLastFormatterPosition();
		}
		public override DocumentModelPosition GetFirstPosition(PieceTable pieceTable) {
			return Boxes.First.GetFirstPosition(pieceTable);
		}
		public override DocumentModelPosition GetLastPosition(PieceTable pieceTable) {
			return Boxes.Last.GetLastPosition(pieceTable);
		}
		public override Box CreateBox() {
			Exceptions.ThrowInternalException();
			return new Row();
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportRow(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreateRowHitTestManager(this);
		}
		public static int FindLastNonSpaceBoxIndex(BoxCollection boxes, int startIndex) {
			if (startIndex < 0)
				startIndex = 0;
			int lastIndex = boxes.Count - 1;
			int i;
			for (i = lastIndex; i >= startIndex; i--) {
				Box box = boxes[i];
				if (!(box is ISpaceBox) && !(box is ParagraphMarkBox) && !(box is LineBreakBox) && !(box is PageBreakBox))
					break;
			}
			return i;
		}
		public override void MoveVertically(int deltaY) {
			base.MoveVertically(deltaY);
			Boxes.MoveVertically(deltaY);
			if (InnerUnderlines != null)
				InnerUnderlines.MoveVertically(deltaY);
			if (InnerStrikeouts != null)
				InnerStrikeouts.MoveVertically(deltaY);
			if (InnerErrors != null)
				InnerErrors.MoveVertically(deltaY);
			if (InnerHighlightAreas != null)
				InnerHighlightAreas.MoveVertically(deltaY);
			if (InnerFieldHighlightAreas != null)
				InnerFieldHighlightAreas.MoveVertically(deltaY);
			if (InnerRangePermissionHighlightAreas != null)
				InnerRangePermissionHighlightAreas.MoveVertically(deltaY);
			if (InnerCustomMarkBoxes != null)
				InnerCustomMarkBoxes.MoveVertically(deltaY);
			if (InnerCommentHighlightAreas != null)
				InnerCommentHighlightAreas.MoveVertically(deltaY);
			if (NumberingListBox != null)
				NumberingListBox.MoveVertically(deltaY);
		}
		public virtual TableCellViewInfo GetCellViewInfo() {
			return null;
		}
		public override string GetText(PieceTable table) {
			Exceptions.ThrowInternalException();
			return String.Empty;
		}
		public override TextRunBase GetRun(PieceTable pieceTable) {
			Exceptions.ThrowInternalException();
			return null;
		}
	}
	#endregion
	#region RowExtendedBoxes
	public class RowExtendedBoxes {
		#region Fields
		UnderlineBoxCollection underlines;
		UnderlineBoxCollection strikeouts;
		ErrorBoxCollection errors;
		HighlightAreaCollection highlightAreas;
		HighlightAreaCollection fieldHighlightAreas;
		HighlightAreaCollection rangePermissionHighlightAreas;
		HighlightAreaCollection commentHighlightAreas;
		BookmarkBoxCollection bookmarkBoxes;
		BookmarkBoxCollection rangePermissionBoxes;
		BookmarkBoxCollection commentBoxes;
		HiddenTextUnderlineBoxCollection hiddenTextBoxes;
		CustomMarkBoxCollection customMarkBoxes;
		SpecialTextBoxCollection specialTextBoxes;
		RowBoxRangeCollection boxRanges;
		#endregion
		#region Properties
		#region Underlines
		public UnderlineBoxCollection Underlines {
			get {
				if (underlines == null)
					underlines = new UnderlineBoxCollection();
				return underlines;
			}
		}
		protected internal UnderlineBoxCollection InnerUnderlines { get { return underlines; } }
		protected internal void ClearUnderlines() {
			underlines = null;
		}
		#endregion
		#region Strikeouts
		public UnderlineBoxCollection Strikeouts {
			get {
				if (strikeouts == null)
					strikeouts = new UnderlineBoxCollection();
				return strikeouts;
			}
		}
		protected internal UnderlineBoxCollection InnerStrikeouts { get { return strikeouts; } }
		protected internal void ClearStrikeouts() {
			strikeouts = null;
		}
		#endregion
		#region Errors
		public ErrorBoxCollection Errors {
			get {
				if (errors == null)
					errors = new ErrorBoxCollection();
				return errors;
			}
		}
		protected internal ErrorBoxCollection InnerErrors { get { return errors; } }
		protected internal void ClearErrors() {
			errors = null;
		}
		#endregion
		#region HighlightAreas
		public HighlightAreaCollection HighlightAreas {
			get {
				if (highlightAreas == null)
					highlightAreas = new HighlightAreaCollection();
				return highlightAreas;
			}
		}
		protected internal HighlightAreaCollection InnerHighlightAreas { get { return highlightAreas; } }
		protected internal void ClearHighlightAreas() {
			highlightAreas = null;
		}
		#endregion
		#region FieldHighlightAreas
		public HighlightAreaCollection FieldHighlightAreas {
			get {
				if (fieldHighlightAreas == null)
					fieldHighlightAreas = new HighlightAreaCollection();
				return fieldHighlightAreas;
			}
		}
		protected internal HighlightAreaCollection InnerFieldHighlightAreas { get { return fieldHighlightAreas; } }
		protected internal void ClearFieldHighlightAreas() {
			fieldHighlightAreas = null;
		}
		#endregion
		#region RangePermissionHighlightAreas
		public HighlightAreaCollection RangePermissionHighlightAreas {
			get {
				if (rangePermissionHighlightAreas == null)
					rangePermissionHighlightAreas = new HighlightAreaCollection();
				return rangePermissionHighlightAreas;
			}
		}
		public HighlightAreaCollection InnerRangePermissionHighlightAreas { get { return rangePermissionHighlightAreas; } }
		protected internal void ClearRangePermissionHighlightAreas() {
			rangePermissionHighlightAreas = null;
		}
		#endregion
		#region CommentHighlightAreas
		public HighlightAreaCollection CommentHighlightAreas {
			get {
				if (commentHighlightAreas == null)
					commentHighlightAreas = new HighlightAreaCollection();
				return commentHighlightAreas;
			}
		}
		public HighlightAreaCollection InnerCommentHighlightAreas { get { return commentHighlightAreas; } }
		protected internal void ClearCommentHighlightAreas() {
			commentHighlightAreas = null;
		}
		#endregion
		#region BookmarkBoxes
		public BookmarkBoxCollection BookmarkBoxes {
			get {
				if (bookmarkBoxes == null)
					bookmarkBoxes = new BookmarkBoxCollection();
				return bookmarkBoxes;
			}
		}
		protected internal BookmarkBoxCollection InnerBookmarkBoxes { get { return bookmarkBoxes; } }
		protected internal void ClearBookmarkBoxes() {
			bookmarkBoxes = null;
		}
		#endregion
		#region RangePermissionBoxes
		public BookmarkBoxCollection RangePermissionBoxes {
			get {
				if (rangePermissionBoxes == null)
					rangePermissionBoxes = new BookmarkBoxCollection();
				return rangePermissionBoxes;
			}
		}
		protected internal BookmarkBoxCollection InnerRangePermissionBoxes { get { return rangePermissionBoxes; } }
		protected internal void ClearRangePermissionsBoxes() {
			rangePermissionBoxes = null;
		}
		#endregion
		#region CommentBoxes
		public BookmarkBoxCollection CommentBoxes {
			get {
				if (commentBoxes == null)
					commentBoxes = new BookmarkBoxCollection();
				return commentBoxes;
			}
		}
		protected internal BookmarkBoxCollection InnerCommentBoxes { get { return commentBoxes; } }
		protected internal void ClearCommentBoxes() {
			commentBoxes = null;
		}
		#endregion
		#region HiddenTextBoxes
		public HiddenTextUnderlineBoxCollection HiddenTextBoxes {
			get {
				if (hiddenTextBoxes == null)
					hiddenTextBoxes = new HiddenTextUnderlineBoxCollection();
				return hiddenTextBoxes;
			}
		}
		protected internal HiddenTextUnderlineBoxCollection InnerHiddenTextBoxes { get { return hiddenTextBoxes; } }
		protected internal void ClearHiddenTextBoxes() {
			hiddenTextBoxes = null;
		}
		#endregion
		#region CustomMarkBoxes
		public CustomMarkBoxCollection CustomMarkBoxes {
			get {
				if (customMarkBoxes == null)
					customMarkBoxes = new CustomMarkBoxCollection();
				return customMarkBoxes;
			}
		}
		protected internal CustomMarkBoxCollection InnerCustomMarkBoxes { get { return customMarkBoxes; } }
		protected internal void ClearCustomMarkBoxes() {
			customMarkBoxes = null;
		}
		#endregion
		#region SpecialTextBoxes
		public SpecialTextBoxCollection SpecialTextBoxes {
			get {
				if (specialTextBoxes == null)
					specialTextBoxes = new SpecialTextBoxCollection();
				return specialTextBoxes;
			}
		}
		protected internal SpecialTextBoxCollection InnerSpecialTextBoxes { get { return specialTextBoxes; } }
		protected internal void ClearSpecialTextBoxes() {
			specialTextBoxes = null;
		}
		#endregion
		#region BoxRanges
		public RowBoxRangeCollection BoxRanges {
			get {
				if (boxRanges == null)
					boxRanges = new RowBoxRangeCollection();
				return boxRanges;
			}
		}
		protected internal RowBoxRangeCollection InnerBoxRanges { get { return boxRanges; } }
		protected internal void ClearBoxRanges() {
			boxRanges = null;
		}
		#endregion
		#endregion
	}
	#endregion
	#region RowBoxRange
	public class RowBoxRange {
		int firstBoxIndex;
		int lastBoxIndex;
		Rectangle bounds;
		public RowBoxRange(int firstBoxIndex, int lastBoxIndex, Rectangle bounds) {
			this.firstBoxIndex = firstBoxIndex;
			this.lastBoxIndex = lastBoxIndex;
			this.bounds = bounds;
		}
		public int FirstBoxIndex { get { return firstBoxIndex; } set { firstBoxIndex = value; } }
		public int LastBoxIndex { get { return lastBoxIndex; } set { lastBoxIndex = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
	}
	#endregion
	#region RowBoxRangeCollection
	public class RowBoxRangeCollection : List<RowBoxRange> {
	}
	#endregion
	public class TableCellRow : Row {
		TableCellViewInfo cellViewInfo;
		public TableCellRow(TableCellViewInfo cellViewInfo) {
			this.cellViewInfo = cellViewInfo;
		}
		public TableCellViewInfo CellViewInfo { get { return cellViewInfo; } set { cellViewInfo = value; } }
		public override bool IsTabelCellRow { get { return true; } }
		public override TableCellViewInfo GetCellViewInfo() {
			return cellViewInfo;
		}
	}
	#region HighlightArea
	public class HighlightArea {
		readonly Color color;
		Rectangle bounds;
		public HighlightArea(Color color, Rectangle bounds) {
			this.color = color;
			this.bounds = bounds;
		}
		public Color Color { get { return color; } }
		public Rectangle Bounds { get { return bounds; } }
		public void MoveVertically(int deltaY) {
			bounds.Y += deltaY;
		}
	}
	#endregion
	#region HighlightAreaCollection
	public class HighlightAreaCollection : List<HighlightArea> {
		public void MoveVertically(int deltaY) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].MoveVertically(deltaY);
		}
	}
	#endregion
	#region ContentBoxCollection
	public class ContentBoxCollection  {
		Row row;
		public ContentBoxCollection(Row row) {
			Debug.Assert(row.NumberingListBox != null);
			Debug.Assert(row.Boxes.Count > 0);
			this.row = row;
		}
		public Box this[int index] { get { return index == 0 ? row.NumberingListBox : row.Boxes[index - 1]; } }
		public int Count { get { return row.Boxes.Count + 1; } }
		public Box First { get { return row.NumberingListBox; } }
		public Box Last { get { return row.Boxes.Last; } }
	}
	#endregion
	#region RowCollection
	public class RowCollection : BoxCollectionBase<Row> {
		protected internal override void RegisterSuccessfullItemHitTest(BoxHitTestCalculator calculator, Row item) {
			calculator.Result.Row = item;
			calculator.Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Row);
		}
		protected internal override void RegisterFailedItemHitTest(BoxHitTestCalculator calculator) {
			calculator.Result.Row = null;
		}
	}
	#endregion
	#region HiddenTextUnderlineBox
	public class HiddenTextUnderlineBox {
		readonly int start;
		readonly int end;
		readonly int bottomOffset;
		public HiddenTextUnderlineBox(int start, int end, int bottom) {
			this.start = start;
			this.end = end;
			this.bottomOffset = bottom;
		}
		public int Start { get { return start; } }
		public int End { get { return end; } }
		public int BottomOffset { get { return bottomOffset; } }
	}
	#endregion
	#region HiddenTextUnderlineBoxCollection
	public class HiddenTextUnderlineBoxCollection : List<HiddenTextUnderlineBox> {
	}
	#endregion
}

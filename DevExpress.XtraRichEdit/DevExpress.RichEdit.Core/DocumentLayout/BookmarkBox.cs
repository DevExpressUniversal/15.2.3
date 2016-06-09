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
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout {
	#region BookmarkBox (abstract class)
	public abstract class VisitableDocumentIntervalBox {
		Color color;
		int horizontalPosition;
		VisitableDocumentInterval interval;
		public Color Color { get { return color; } set { color = value; } }
		public int HorizontalPosition { get { return horizontalPosition; } set { horizontalPosition = value; } }
		public VisitableDocumentInterval Interval { get { return interval; } set { interval = value; } }
		public abstract void ExportTo(IDocumentLayoutExporter exporter);
	}
	#endregion
	#region BookmarkStartBox
	public class BookmarkStartBox : VisitableDocumentIntervalBox {
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportBookmarkStartBox(this);
		}
	}
	#endregion
	#region BookmarkEndBox
	public class BookmarkEndBox : VisitableDocumentIntervalBox {
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportBookmarkEndBox(this);
		}
	}
	#endregion
	#region BookmarkBoxCollection
	public class BookmarkBoxCollection : List<VisitableDocumentIntervalBox> { }
	#endregion
	#region BookmarkBoxCalculator
	public class BookmarkBoxCalculator {
		readonly IBoxMeasurerProvider measurerProvider;
		PieceTable pieceTable;
		readonly Dictionary<PieceTable, VisitableDocumentIntervalBoundaryIterator> bookmarkIteratorCache;
		VisitableDocumentIntervalBoundaryIterator bookmarkIterator;
		private bool exportToPdf = false;
		public BookmarkBoxCalculator(PieceTable pieceTable, IBoxMeasurerProvider measurerProvider) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(measurerProvider, "measurerProvider");
			this.pieceTable = pieceTable;
			this.measurerProvider = measurerProvider;
			this.bookmarkIteratorCache = new Dictionary<PieceTable, VisitableDocumentIntervalBoundaryIterator>();
		}
		public PieceTable PieceTable {
			get { return pieceTable; }
			set {
				if (pieceTable == value)
					return;
				Guard.ArgumentNotNull(value, "pieceTable");
				ChangeBookmarkIterator(pieceTable, value);
				pieceTable = value;
			}
		}
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public BoxMeasurer Measurer { get { return measurerProvider.Measurer; } }
		protected internal VisitableDocumentIntervalBoundaryIterator BookmarkIterator { get { return bookmarkIterator; } }
		internal Dictionary<PieceTable, VisitableDocumentIntervalBoundaryIterator> BookmarkIteratorCache { get { return bookmarkIteratorCache; } }
		internal bool ExportToPdf { get { return exportToPdf; } set { exportToPdf = value; } }
		protected internal virtual BookmarkBoxCollection GetBookmarkBoxCollection(Row row) {
			return row.BookmarkBoxes;
		}
		protected internal virtual void ClearBookmarkBoxCollection(Row row) {
			row.ClearBookmarkBoxes();
		}
		protected internal virtual void ChangeBookmarkIterator(PieceTable oldPieceTable, PieceTable newPieceTable) {
			if (bookmarkIterator != null) {
				if (!bookmarkIterator.IsDone())
					bookmarkIteratorCache[oldPieceTable] = bookmarkIterator;
				else
					bookmarkIteratorCache.Remove(oldPieceTable);
			}
			bookmarkIteratorCache.TryGetValue(newPieceTable, out bookmarkIterator);
		}
		protected internal virtual void EnsureBookmarkIteratorIsInitialized() {
			if (bookmarkIterator != null)
				return;
			bookmarkIterator = CreateBookmarkBoundaryIterator();
		}
		public virtual void Calculate(Row row) {
			if (!ShouldCalculate())
				return;
			EnsureBookmarkIteratorIsInitialized();
			List<VisitableDocumentIntervalBox> boxes = CalculateBoxes(row);
			if (boxes.Count > 0) {
				ClearBookmarkBoxCollection(row);
				GetBookmarkBoxCollection(row).AddRange(boxes);
			}
		}
		protected internal virtual bool ShouldCalculate() {
			return DocumentModel.BookmarkOptions.Visibility == RichEditBookmarkVisibility.Visible || ExportToPdf;
		}
		protected internal virtual List<VisitableDocumentIntervalBox> CalculateBoxes(Row row) {
			List<VisitableDocumentIntervalBox> result = new List<VisitableDocumentIntervalBox>();
			for (; !bookmarkIterator.IsDone(); bookmarkIterator.MoveNext()) {
				VisitableDocumentIntervalBoundary boundary = bookmarkIterator.Current;
				FormatterPosition position = GetPosition(boundary);
				if (position > row.GetLastFormatterPosition())
					break;
				if (position < row.GetFirstFormatterPosition())
					continue;
				BoxCollection boxes = row.Boxes;
				int boxIndex = Algorithms.BinarySearch(boxes, new BoxAndFormatterPositionComparable<Box>(position));
				if (boxIndex < 0) {
					boxIndex = ~boxIndex;
					if (boxIndex >= boxes.Count)
						break;
				}
				VisitableDocumentIntervalBox box = boundary.CreateBox();
				box.Interval = bookmarkIterator.Current.VisitableInterval;
				box.HorizontalPosition = GetHorizontalPosition(position, boxes[boxIndex]);
				box.Color = GetBoxColor();
				result.Add(box);
			}
			return result;
		}
		protected internal virtual int GetHorizontalPosition(FormatterPosition position, Box box) {
			if (box.StartPos.RunIndex != position.RunIndex) 
				return box.Bounds.Left;
			if (position == box.StartPos)
				return box.Bounds.Left;
			else if (GetPrevPosition(position) == box.EndPos)
				return box.Bounds.Right;
			return box.Bounds.Left + MeasureBoxPart(position, box);
		}
		protected internal virtual FormatterPosition GetPosition(VisitableDocumentIntervalBoundary boundary) {
			return new FormatterPosition(boundary.Position.RunIndex, boundary.Position.RunOffset, 0);
		}
		protected internal virtual FormatterPosition GetPrevPosition(FormatterPosition pos) {
			RunIndex runIndex;
			int offset = pos.Offset - 1;
			if (offset < 0) {
				runIndex = pos.RunIndex - 1;
				if (runIndex < RunIndex.Zero)
					return pos;
				offset = PieceTable.Runs[runIndex].Length - 1;
			}
			else
				runIndex = pos.RunIndex;
			return new FormatterPosition(runIndex, offset, 0);
		}
		protected internal virtual int MeasureBoxPart(FormatterPosition position, Box box) {
			BoxInfo boxInfo = new BoxInfo();
			FormatterPosition boxStartPos = box.StartPos;
			boxInfo.StartPos = new FormatterPosition(boxStartPos.RunIndex, boxStartPos.Offset, -1);
			FormatterPosition prevPosition = GetPrevPosition(position);
			boxInfo.EndPos = new FormatterPosition(prevPosition.RunIndex, prevPosition.Offset, -1);
			PieceTable oldPieceTable = Measurer.PieceTable;
			try {
				Measurer.PieceTable = PieceTable;
				Measurer.MeasureText(boxInfo);
			}
			finally {
				Measurer.PieceTable = oldPieceTable;
			}
			return boxInfo.Size.Width;
		}
		public virtual void ResetFrom(DocumentModelPosition position) {
			Reset();
		}
		public virtual void Reset() {
			bookmarkIterator = null;
			bookmarkIteratorCache.Clear();
		}
		protected internal virtual VisitableDocumentIntervalBoundaryIterator CreateBookmarkBoundaryIterator() {
			return new VisitableDocumentIntervalBoundaryIterator(PieceTable, ExportToPdf);
		}
		protected internal virtual Color GetBoxColor() {
			return DocumentModel.BookmarkOptions.Color;
		}
	}
	#endregion
	#region RangePermissionBoxCalculator
	public class RangePermissionBoxCalculator : BookmarkBoxCalculator {
		public RangePermissionBoxCalculator(PieceTable pieceTable, IBoxMeasurerProvider measurerProvider)
			: base(pieceTable, measurerProvider) {
		}
		protected internal override VisitableDocumentIntervalBoundaryIterator CreateBookmarkBoundaryIterator() {
			return new RangePermissionBoundaryIterator(PieceTable);
		}
		protected internal override bool ShouldCalculate() {
			return DocumentModel.RangePermissionOptions.Visibility != RichEditRangePermissionVisibility.Hidden;
		}
		protected internal override Color GetBoxColor() {
			if (DocumentModel.ProtectionProperties.EnforceProtection)
				return DocumentModel.RangePermissionOptions.HighlightBracketsColor;
			return DocumentModel.RangePermissionOptions.BracketsColor;
		}
		protected internal override BookmarkBoxCollection GetBookmarkBoxCollection(Row row) {
			return row.RangePermissionBoxes;
		}
		protected internal override void ClearBookmarkBoxCollection(Row row) {
			row.ClearRangePermissionsBoxes();
		}
	}
	#endregion
	#region CommentBoxCalculator
	public class CommentBoxCalculator : BookmarkBoxCalculator {
		public CommentBoxCalculator(PieceTable pieceTable, IBoxMeasurerProvider measurerProvider)
			: base(pieceTable, measurerProvider) {
		}
		protected internal override VisitableDocumentIntervalBoundaryIterator CreateBookmarkBoundaryIterator() {
			return new CommentBoundaryIterator(PieceTable);
		}
		protected internal override bool ShouldCalculate() {
			return ((DocumentModel.CommentOptions.Visibility != RichEditCommentVisibility.Hidden) || DocumentModel.CommentOptions.HighlightCommentedRange);
		}
		protected internal override Color GetBoxColor() {
			Comment comment = (Comment)this.BookmarkIterator.Current.VisitableInterval;
			Color customColor = DocumentModel.CommentOptions.Color;
			Color fillColor = DocumentModel.CommentColorer.GetColor(comment);
			return CommentOptions.SetBorderColor(fillColor);
		}
		protected internal override BookmarkBoxCollection GetBookmarkBoxCollection(Row row) {
			return row.CommentBoxes;
		}
		protected internal override void ClearBookmarkBoxCollection(Row row) {
			row.ClearCommentBoxes();
		}
	}
	#endregion
	#region CommentStartBox
	public class CommentStartBox : VisitableDocumentIntervalBox {
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportCommentStartBox(this);
		}
	}
	#endregion
	#region CommentEndBox
	public class CommentEndBox : VisitableDocumentIntervalBox {
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportCommentEndBox(this);
		}
	}
	#endregion
	public class CustomMarkComparer : IComparer<CustomMark> {
		#region IComparer<CustomMark> Members
		public int Compare(CustomMark x, CustomMark y) {
			return x.Position.LogPosition - y.Position.LogPosition;
		}
		#endregion
	}
	public class CustomMarkIterator {
		int currentIndex;
		readonly PieceTable pieceTable;
		readonly List<CustomMark> customMarks;
		public CustomMarkIterator(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.customMarks = new List<CustomMark>();
			InitializeBoundaries();
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public CustomMark Current { get { return customMarks[currentIndex]; } }
		protected internal List<CustomMark> CustomMarks { get { return customMarks; } }
		public virtual bool IsDone() {
			return currentIndex >= customMarks.Count;
		}
		public virtual void MoveNext() {
			currentIndex++;
		}
		protected internal virtual void InitializeBoundaries() {
			PopulateBookmarkBoundaries();
			customMarks.Sort(new CustomMarkComparer());
		}
		protected internal virtual void PopulateBookmarkBoundaries() {
			customMarks.Clear();
			PopulateBookmarkBoundariesCore();
		}
		protected internal virtual void PopulateBookmarkBoundariesCore() {
			PopulateBookmarkBoundariesCore(PieceTable.CustomMarks);
		}
		protected internal virtual void PopulateBookmarkBoundariesCore(CustomMarkCollection customMarks) {
			int count = customMarks.Count;
			for (int i = 0; i < count; i++) {
				this.customMarks.Add(customMarks[i]);
			}
		}
		protected internal virtual bool IsVisibleBookmark(BookmarkBase bookmark) {
			return true;
		}
	}
	#region CustomMarkBoxCalculator
	public class CustomMarkBoxCalculator {
		CustomMarkIterator iterator;
		PieceTable pieceTable;
		readonly IBoxMeasurerProvider measurerProvider;
		readonly Dictionary<PieceTable, CustomMarkIterator> iteratorCache;
		public CustomMarkBoxCalculator(PieceTable pieceTable, IBoxMeasurerProvider measurerProvider) {
			this.pieceTable = pieceTable;
			this.measurerProvider = measurerProvider;
			this.iteratorCache = new Dictionary<PieceTable, CustomMarkIterator>();
		}
		public PieceTable PieceTable {
			get { return pieceTable; }
			set {
				if (pieceTable == value)
					return;
				Guard.ArgumentNotNull(value, "pieceTable");
				ChangeBookmarkIterator(pieceTable, value);
				pieceTable = value;
			}
		}
		public BoxMeasurer Measurer { get { return measurerProvider.Measurer; } }
		protected internal virtual void ChangeBookmarkIterator(PieceTable oldPieceTable, PieceTable newPieceTable) {
			if (iterator != null) {
				if (!iterator.IsDone())
					iteratorCache[oldPieceTable] = iterator;
				else
					iteratorCache.Remove(oldPieceTable);
			}
			iteratorCache.TryGetValue(newPieceTable, out iterator);
		}
		protected internal virtual void EnsureIteratorIsInitialized() {
			if (iterator != null)
				return;
			iterator = CreateIterator();
		}
		public virtual void Calculate(Row row) {
			EnsureIteratorIsInitialized();
			if (iterator.IsDone())
				return;
			FormatterPosition pos;
			FormatterPosition firstFormatterPosition = row.GetFirstFormatterPosition();
			do {
				pos = GetPosition(iterator.Current);
				if (pos < firstFormatterPosition)
					iterator.MoveNext();
				else
					break;
			} while (!iterator.IsDone());
			if (pos < firstFormatterPosition || pos > row.GetLastFormatterPosition())
				return;
			CalculateCore(row);
		}
		protected internal virtual void CalculateCore(Row row) {
			row.ClearCustomMarkBoxes();
			Rectangle rowBounds = row.Bounds;
			while (!iterator.IsDone()) {
				CustomMark customMark = iterator.Current;
				FormatterPosition position = GetPosition(customMark);
				BoxCollection boxes = row.Boxes;
				int boxIndex = Algorithms.BinarySearch(boxes, new BoxAndFormatterPositionComparable<Box>(position));
				if (boxIndex < 0) {
					boxIndex = ~boxIndex;
					if (boxIndex >= boxes.Count)
						return;
				}
				CustomMarkBox box = new CustomMarkBox();
				box.CustomMark = customMark;
				int horizontalPosition = GetHorizontalPosition(position, boxes[boxIndex]);
				box.Bounds = new Rectangle(horizontalPosition, rowBounds.Top, 0, rowBounds.Height);
				row.CustomMarkBoxes.Add(box);
				iterator.MoveNext();
			}
		}
		protected internal virtual int GetHorizontalPosition(FormatterPosition position, Box box) {
			if (box.StartPos.RunIndex != position.RunIndex) 
				return box.Bounds.Left;
			if (position == box.StartPos)
				return box.Bounds.Left;
			else if (GetPrevPosition(position) == box.EndPos)
				return box.Bounds.Right;
			return box.Bounds.Left + MeasureBoxPart(position, box);
		}
		protected internal virtual FormatterPosition GetPosition(CustomMark customMark) {
			return new FormatterPosition(customMark.Position.RunIndex, customMark.Position.RunOffset, 0);
		}
		protected internal virtual FormatterPosition GetPrevPosition(FormatterPosition pos) {
			RunIndex runIndex;
			int offset = pos.Offset - 1;
			if (offset < 0) {
				runIndex = pos.RunIndex - 1;
				if (runIndex < RunIndex.Zero)
					return pos;
				offset = PieceTable.Runs[runIndex].Length - 1;
			}
			else
				runIndex = pos.RunIndex;
			return new FormatterPosition(runIndex, offset, 0);
		}
		protected internal virtual int MeasureBoxPart(FormatterPosition position, Box box) {
			BoxInfo boxInfo = new BoxInfo();
			FormatterPosition boxStartPos = box.StartPos;
			boxInfo.StartPos = new FormatterPosition(boxStartPos.RunIndex, boxStartPos.Offset, -1);
			FormatterPosition prevPosition = GetPrevPosition(position);
			boxInfo.EndPos = new FormatterPosition(prevPosition.RunIndex, prevPosition.Offset, -1);
			PieceTable oldPieceTable = Measurer.PieceTable;
			try {
				Measurer.PieceTable = PieceTable;
				Measurer.MeasureText(boxInfo);
			}
			finally {
				Measurer.PieceTable = oldPieceTable;
			}
			return boxInfo.Size.Width;
		}
		public virtual void ResetFrom(DocumentModelPosition position) {
			Reset();
			iterator = CreateIterator();
			while (!iterator.IsDone() && iterator.Current.Position < position)
				iterator.MoveNext();
		}
		protected virtual CustomMarkIterator CreateIterator() {
			return new CustomMarkIterator(PieceTable);
		}
		public virtual void Reset() {
			iterator = null;
			iteratorCache.Clear();
		}
	}
	#endregion
}

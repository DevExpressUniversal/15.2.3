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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	#region Page
	public class Page : NoPositionBox {
		#region Fields
		readonly PageAreaCollection areas;
		HeaderPageArea header;
		FooterPageArea footer;
		Rectangle clientBounds;
		Rectangle commentBounds;
		bool primaryFormattingComplete;
		bool secondaryFormattingComplete;
		bool checkSpellingComplete;
		int pageIndex = -1;
		int pageOrdinal = -1;
		int numPages = -1;
		int numSkippedPages = 0;
		List<FloatingObjectBox> floatingObjects;
		List<FloatingObjectBox> foregroundFloatingObjects;
		List<FloatingObjectBox> backgroundFloatingObjects;
		List<ParagraphFrameBox> paragraphFrames;
		List<FootNotePageArea> footNotes;
		List<CommentViewInfo> comments;
		Page pageNumberSource;
		#endregion
		public Page()
			: this(null) {
		}
		public Page(Page pageNumberSource) {
			this.areas = new PageAreaCollection();
			this.pageNumberSource = pageNumberSource;
		}
		#region Properties
		public override bool IsVisible { get { return true; } }
		protected internal override DocumentLayoutDetailsLevel DetailsLevel { get { return DocumentLayoutDetailsLevel.Page; } }
		protected internal override HitTestAccuracy HitTestAccuracy { get { return HitTestAccuracy.ExactPage; } }
		public bool PrimaryFormattingComplete { get { return primaryFormattingComplete; } set { primaryFormattingComplete = value; } }
		public bool SecondaryFormattingComplete { get { return secondaryFormattingComplete; } set { secondaryFormattingComplete = value; } }
		public bool CheckSpellingComplete { get { return checkSpellingComplete; } set { checkSpellingComplete = value; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		public int PageIndex { get { return pageIndex; } protected internal set { pageIndex = value; } }
		public int NumPages { get { return numPages; } protected internal set { numPages = value; } }
		public int NumSkippedPages { get { return numSkippedPages; } protected internal set { numSkippedPages = value; } }
		public int PageOrdinal { get { return pageOrdinal; } protected internal set { pageOrdinal = value; } }
		public PageAreaCollection Areas { get { return areas; } }
		public Page PageNumberSource { get { return pageNumberSource != null ? pageNumberSource.PageNumberSource : this; } }
		public bool IsEmpty {
			get {
				return Areas.Count <= 0 || Areas[0].IsEmpty;
			}
		}
		public Rectangle ClientBounds { get { return clientBounds; } set { clientBounds = value; } }
		public Rectangle CommentBounds { get { return commentBounds; } set { commentBounds = value; } }
		public HeaderPageArea Header { get { return header; } protected internal set { header = value; } }
		public FooterPageArea Footer { get { return footer; } protected internal set { footer = value; } }
		public bool IsEven { get { return (PageOrdinal % 2) == 0; } }
		internal List<FloatingObjectBox> InnerFloatingObjects { get { return floatingObjects; } }
		internal List<FloatingObjectBox> FloatingObjects {
			get {
				if (floatingObjects == null)
					floatingObjects = new List<FloatingObjectBox>();
				return floatingObjects;
			}
		}
		internal List<FloatingObjectBox> InnerForegroundFloatingObjects { get { return foregroundFloatingObjects; } }
		internal List<FloatingObjectBox> ForegroundFloatingObjects {
			get {
				if (foregroundFloatingObjects == null)
					foregroundFloatingObjects = new List<FloatingObjectBox>();
				return foregroundFloatingObjects;
			}
		}
		internal List<FloatingObjectBox> InnerBackgroundFloatingObjects { get { return backgroundFloatingObjects; } }
		internal List<FloatingObjectBox> BackgroundFloatingObjects {
			get {
				if (backgroundFloatingObjects == null)
					backgroundFloatingObjects = new List<FloatingObjectBox>();
				return backgroundFloatingObjects;
			}
		}
		internal List<ParagraphFrameBox> InnerParagraphFrames { get { return paragraphFrames; } }
		internal List<ParagraphFrameBox> ParagraphFrames {
			get {
				if (paragraphFrames == null)
					paragraphFrames = new List<ParagraphFrameBox>();
				return paragraphFrames;
			}
		}
		internal List<FootNotePageArea> InnerFootNotes { get { return footNotes; } }
		internal List<FootNotePageArea> FootNotes {
			get {
				if (footNotes == null)
					footNotes = new List<FootNotePageArea>();
				return footNotes;
			}
		}
		internal List<CommentViewInfo> InnerComments { get { return comments; } }
		internal List<CommentViewInfo> Comments {
			get {
				if (comments == null)
					comments = new List<CommentViewInfo>();
				return comments;
			}
		}
		#endregion
		public void ClearFloatingObjects() {
			this.floatingObjects = null;
			this.foregroundFloatingObjects = null;
			this.backgroundFloatingObjects = null;
		}
		public void ClearInvalidatedContent(RunIndex runIndex, PieceTable pieceTable) {
			ClearFloatingObjects(pieceTable, runIndex, InnerFloatingObjects);
			ClearFloatingObjects(pieceTable, runIndex, InnerForegroundFloatingObjects);
			ClearFloatingObjects(pieceTable, runIndex, InnerBackgroundFloatingObjects);
			ClearFootNotes(runIndex);
			ClearComments(pieceTable, runIndex);
		}
		void ClearFloatingObjects(PieceTable pieceTable, RunIndex runIndex, List<FloatingObjectBox> objects) {
			if (objects == null)
				return;
			int count = objects.Count;
			for (int i = count - 1; i >= 0; i--)
				if (pieceTable == null || (objects[i].PieceTable == pieceTable && runIndex <= objects[i].StartPos.RunIndex))
					objects.RemoveAt(i);
		}
		void ClearFootNotes(RunIndex runIndex) {
			if (InnerFootNotes == null)
				return;
			int count = InnerFootNotes.Count;
			for (int i = count - 1; i >= 0; i--)
				if (runIndex <= InnerFootNotes[i].ReferenceRunIndex)
					InnerFootNotes.RemoveAt(i);
		}
		void ClearComments(PieceTable pieceTable, RunIndex runIndex) {
			if (InnerComments == null)
				return;
			int count = InnerComments.Count;
			for (int i = count - 1; i >= 0; i--)
				if ((InnerComments[i].Comment.End >= pieceTable.DocumentEndLogPosition) || (runIndex <= (PositionConverter.ToDocumentModelPosition(pieceTable, InnerComments[i].Comment.End)).RunIndex))
					InnerComments.RemoveAt(i);
		}
		public FloatingObjectBox FindFloatingObject(FloatingObjectAnchorRun run) {
			FloatingObjectBox result;
			result = FindFloatingObject(InnerFloatingObjects, run);
			if (result != null)
				return result;
			result = FindFloatingObject(InnerForegroundFloatingObjects, run);
			if (result != null)
				return result;
			result = FindFloatingObject(InnerBackgroundFloatingObjects, run);
			if (result != null)
				return result;
			return null;
		}
		FloatingObjectBox FindFloatingObject(List<FloatingObjectBox> objects, FloatingObjectAnchorRun run) {
			if (objects == null)
				return null;
			int count = objects.Count;
			for (int i = 0; i < count; i++)
				if (objects[i].GetFloatingObjectRun() == run)
					return objects[i];
			return null;
		}
		public override FormatterPosition GetFirstFormatterPosition() {
			if (IsEmpty)
				return FormatterPosition.MaxValue;
			else
				return Areas.First.GetFirstFormatterPosition();
		}
		public override FormatterPosition GetLastFormatterPosition() {
			if (IsEmpty)
				return FormatterPosition.MaxValue;
			else
				return Areas.Last.GetLastFormatterPosition();
		}
		protected internal virtual PageArea GetActiveFirstArea(PieceTable pieceTable) {
			if (Header != null && Object.ReferenceEquals(pieceTable, Header.PieceTable))
				return Header;
			if (Footer != null && Object.ReferenceEquals(pieceTable, Footer.PieceTable))
				return Footer;
			if (pieceTable.IsMain)
				return Areas.First;
			return null;
		}
		protected internal virtual PageArea GetActiveLastArea(PieceTable pieceTable) {
			if (Header != null && Object.ReferenceEquals(pieceTable, Header.PieceTable))
				return Header;
			if (Footer != null && Object.ReferenceEquals(pieceTable, Footer.PieceTable))
				return Footer;
			if (pieceTable.IsMain)
				return Areas.Last;
			return null;
		}
		public override DocumentModelPosition GetFirstPosition(PieceTable pieceTable) {
			if (IsEmpty)
				return DocumentModelPosition.MaxValue;
			else {
				PageArea area = GetActiveFirstArea(pieceTable);
				if (area == null)
					return DocumentModelPosition.MaxValue;
				return area.GetFirstPosition(pieceTable);
			}
		}
		public override DocumentModelPosition GetLastPosition(PieceTable pieceTable) {
			if (IsEmpty)
				return DocumentModelPosition.MaxValue;
			else {
				PageArea area = GetActiveLastArea(pieceTable);
				if (area == null)
					return null;
				return area.GetLastPosition(pieceTable);
			}
		}
		public override Box CreateBox() {
			Exceptions.ThrowInternalException();
			return new Page();
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportPage(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreatePageHitTestManager(this);
		}
		public override void MoveVertically(int deltaY) {
			base.MoveVertically(deltaY);
			Areas.MoveVertically(deltaY);
			if (Header != null)
				Header.MoveVertically(deltaY);
			if (Footer != null)
				Footer.MoveVertically(deltaY);
			clientBounds.Y += deltaY;
		}
		public override string GetText(PieceTable table) {
			Exceptions.ThrowInternalException();
			return String.Empty;
		}
		public override TextRunBase GetRun(PieceTable pieceTable) {
			Exceptions.ThrowInternalException();
			return null;
		}
#if DEBUG
		public override string ToString() {
			return String.Format("Page. Bounds: {0}, SecondaryFormattingComplete: {1}", Bounds, SecondaryFormattingComplete);
		}
#endif
		public Column GetLastColumn() {
			PageArea lastArea = Areas.Last;
			if (lastArea == null)
				return null;
			else
				return lastArea.Columns.Last;
		}
		public List<FloatingObjectBox> GetNonBackgroundFloatingObjects() {
			List<FloatingObjectBox> result = new List<FloatingObjectBox>();
			if (InnerFloatingObjects != null)
				result.AddRange(InnerFloatingObjects);
			if (ForegroundFloatingObjects != null)
				result.AddRange(ForegroundFloatingObjects);
			return result;
		}
		public List<ParagraphFrameBox> GetParagraphFrames() {
			List<ParagraphFrameBox> result = new List<ParagraphFrameBox>();
			if (InnerParagraphFrames != null)
				result.AddRange(InnerParagraphFrames);
			return result;
		}
		public IList<FloatingObjectBox> GetSortedNonBackgroundFloatingObjects() {
			return GetSortedNonBackgroundFloatingObjects(new FloatingObjectBoxZOrderComparer());
		}
		public IList<FloatingObjectBox> GetSortedNonBackgroundFloatingObjects(IComparer<FloatingObjectBox> comparer) {
			List<FloatingObjectBox> result = GetNonBackgroundFloatingObjects();
			result.Sort(comparer);
			return result;
		}
		public IList<ParagraphFrameBox> GetSortedParagraphFrames() {
			return GetSortedParagraphFrames(new ParagraphFrameBoxIndexComparer());
		}
		public IList<ParagraphFrameBox> GetSortedParagraphFrames(IComparer<ParagraphFrameBox> comparer) {
			List<ParagraphFrameBox> result = GetParagraphFrames();
			result.Sort(comparer);
			return result;
		}
		internal void EnsureCommentBounds(Page page, DocumentModel documentModel) {
			int minCommentWidth = documentModel.LayoutUnitConverter.DocumentsToLayoutUnits(990);
			int commentOffset = documentModel.LayoutUnitConverter.DocumentsToLayoutUnits(37);
			int commentLeft = page.ClientBounds.Right + commentOffset;
			int commentWidth = GetCommentWidth(page, commentLeft, minCommentWidth);
			page.CommentBounds = new Rectangle(commentLeft, page.Bounds.Top, commentWidth, page.Bounds.Height);
		}
		int GetCommentWidth(Page page, int commentLeft, int minCommentWidth) {
			int minPageBoundsWidth = page.Bounds.Width;
			int commentRight = commentLeft + minCommentWidth;
			if (commentRight < minPageBoundsWidth)
				return minPageBoundsWidth - commentLeft;
			else
				return minCommentWidth;
		}
	}
	#endregion
	#region PageCollection
	public class PageCollection : BoxCollectionBase<Page> {
		protected internal override void RegisterSuccessfullItemHitTest(BoxHitTestCalculator calculator, Page item) {
			calculator.Result.Page = item;
			calculator.Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Page);
		}
		protected internal override void RegisterFailedItemHitTest(BoxHitTestCalculator calculator) {
			calculator.Result.Page = null;
		}
	}
	#endregion
}

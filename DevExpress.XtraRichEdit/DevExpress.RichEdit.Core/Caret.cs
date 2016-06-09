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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit;
using DevExpress.Office.Layout;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout {
	#region CaretPosition
	public class CaretPosition  {
		#region Fields
		int x;
		int tableCellTopAnchorIndex;
		readonly RichEditView view;
		PageViewInfo pageViewInfo;
		InputPosition inputPosition;
		readonly DocumentLayoutPosition position;
		DocumentLayoutPosition caretBoundsPosition;
		DocumentModelPosition modelPosition;
		int preferredPageIndex;
		#endregion
		public CaretPosition(RichEditView view, int preferredPageIndex) {
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNonNegative(preferredPageIndex, "preferredPageIndex");
			this.view = view;
			this.position = CreateCaretDocumentLayoutPosition();
			this.caretBoundsPosition = position;
			this.PreferredPageIndex = preferredPageIndex;
		}
		#region Properties
		public PageViewInfo PageViewInfo { get { return pageViewInfo; } }
		protected internal RichEditView View { get { return view; } }
		public int X { get { return x; } set { x = value; } }
		public int TableCellTopAnchorIndex { get { return tableCellTopAnchorIndex; } set { tableCellTopAnchorIndex = value; } }
		protected internal virtual DocumentLogPosition VirtualLogPosition { get { return GetActualLogPosition(DocumentModel.Selection.VirtualEnd); } }
		protected internal virtual DocumentLogPosition LogPosition { get { return GetActualLogPosition(DocumentModel.Selection.End); } }
		protected internal virtual bool UsePreviousBoxBounds { get { return DocumentModel.Selection.UsePreviousBoxBounds; } }
		public DocumentLayoutPosition LayoutPosition { get { return position; } }
		protected internal DocumentLayoutPosition CaretBoundsPosition { get { return caretBoundsPosition; } }
		public DocumentModel DocumentModel { get { return view.DocumentModel; } }
		public int PreferredPageIndex {
			get { return preferredPageIndex; }
			set {
				Guard.ArgumentNonNegative(value, "preferredPageIndex");
				preferredPageIndex = value;
				HeaderFooterDocumentLayoutPosition pos;
				pos = this.LayoutPosition as HeaderFooterDocumentLayoutPosition;
				if (pos != null)
					pos.PreferredPageIndex = preferredPageIndex;
				pos = this.CaretBoundsPosition as HeaderFooterDocumentLayoutPosition;
				if (pos != null)
					pos.PreferredPageIndex = preferredPageIndex;
				TextBoxDocumentLayoutPosition pos2;
				pos2 = this.LayoutPosition as TextBoxDocumentLayoutPosition;
				if (pos2 != null)
					pos2.PreferredPageIndex = preferredPageIndex;
				pos2 = this.CaretBoundsPosition as TextBoxDocumentLayoutPosition;
				if (pos2 != null)
					pos2.PreferredPageIndex = preferredPageIndex;
			}
		}
		#endregion
		DocumentLogPosition GetActualLogPosition(DocumentLogPosition endPosition) {
			Selection selection = DocumentModel.Selection;
			DocumentLogPosition end;
			if (selection.Start <= selection.End)
				end = Algorithms.Max(DocumentLogPosition.Zero, endPosition - selection.ActiveSelection.RightOffset);
			else {
				int virtualEndLogPositionCompensation = 0;
				if (UsePreviousBoxBounds)
					virtualEndLogPositionCompensation += 1;
				end = endPosition + selection.ActiveSelection.LeftOffset + virtualEndLogPositionCompensation;
			}
			return Algorithms.Min(end, selection.PieceTable.DocumentEndLogPosition);
		}
		protected internal virtual void Invalidate() {
			position.Invalidate();
			caretBoundsPosition = position;
			InvalidatePageViewInfo();
		}
		protected internal virtual void InvalidatePageViewInfo() {
			this.pageViewInfo = null;
		}
		protected internal virtual void InvalidateInputPosition() {
			inputPosition = null;
		}
		public virtual bool Update(DocumentLayoutDetailsLevel detailsLevel) {
			view.CheckExecutedAtUIThread();
			position.SetLogPosition(this.VirtualLogPosition);
			UpdateModelPosition(Algorithms.Min(position.LogPosition, position.PieceTable.DocumentEndLogPosition));
			if (!position.Update(view.FormattingController.PageController.Pages, detailsLevel)) {
				if (position.DetailsLevel > DocumentLayoutDetailsLevel.None)
					this.pageViewInfo = view.LookupPageViewInfoByPage(position.Page);
				caretBoundsPosition = position;
				return false;
			}
			caretBoundsPosition = UpdateCaretDocumentLayoutPosition(position.DetailsLevel);
#if DEBUGTEST
			PageCollection pages = view.FormattingController.PageController.Pages;
			if (position.DetailsLevel >= DocumentLayoutDetailsLevel.Page)
				Debug.Assert(pages.IndexOf(position.Page) >= 0);
			if (position.DetailsLevel >= DocumentLayoutDetailsLevel.PageArea)
				Debug.Assert(position.Page.Areas.IndexOf(position.PageArea) >= 0 || position.PageArea == position.Page.Header || position.PageArea == position.Page.Footer || position.PieceTable.IsTextBox || position.PieceTable.IsComment);
			if (position.DetailsLevel >= DocumentLayoutDetailsLevel.Column)
				Debug.Assert(position.PageArea.Columns.IndexOf(position.Column) >= 0);
			if (position.DetailsLevel >= DocumentLayoutDetailsLevel.Row)
				Debug.Assert(position.Column.Rows.IndexOf(position.Row) >= 0);
			if (position.DetailsLevel >= DocumentLayoutDetailsLevel.Box)
				Debug.Assert(position.Row.Boxes.IndexOf(position.Box) >= 0);
#endif
			if (pageViewInfo != null)
				return true;
			if (position.DetailsLevel > DocumentLayoutDetailsLevel.None)
				this.pageViewInfo = view.LookupPageViewInfoByPage(position.Page);
#if DEBUGTEST
#endif
			return this.pageViewInfo != null;
		}
		protected virtual void UpdateModelPosition(DocumentLogPosition logPosition) {
			if (this.modelPosition == null || !Object.ReferenceEquals(this.modelPosition.PieceTable, DocumentModel.ActivePieceTable))
				this.modelPosition = new DocumentModelPosition(DocumentModel.ActivePieceTable);
			this.modelPosition.LogPosition = logPosition;
			if (ShouldUpdateModelPosition(logPosition))
				this.modelPosition.Update();
		}
		bool ShouldUpdateModelPosition(DocumentLogPosition logPosition) {
			ParagraphCollection paragraphs = modelPosition.PieceTable.Paragraphs;
			if (modelPosition.ParagraphIndex >= new ParagraphIndex(paragraphs.Count))
				return true;
			RunIndex runIndex = modelPosition.RunIndex;
			if(runIndex >= new RunIndex(modelPosition.PieceTable.Runs.Count))
				return true;			
			if (logPosition < this.modelPosition.RunStartLogPosition || logPosition > this.modelPosition.RunEndLogPosition)
				return true;
			Paragraph paragraph = paragraphs[modelPosition.ParagraphIndex];
			if (runIndex > paragraph.LastRunIndex || runIndex < paragraph.FirstRunIndex)
				return true;
			return false;
		}
		protected internal virtual bool UpdatePositionTrySetUsePreviousBoxBounds(DocumentLayoutDetailsLevel detailsLevel) {
			Selection selection = DocumentModel.Selection;
			if (!UsePreviousBoxBounds && selection.Length > 0 && selection.End > selection.Start) {
				if (position.IsValid(DocumentLayoutDetailsLevel.Row)) {
					if (position.Row.GetFirstPosition(position.PieceTable).LogPosition == position.LogPosition) {
						selection.UsePreviousBoxBounds = true;
						position.Invalidate();
						return Update(detailsLevel);
					}
				}
			}
			return false;
		}
		protected internal virtual DocumentLayoutPosition UpdateCaretDocumentLayoutPosition(DocumentLayoutDetailsLevel detailsLevel) {
			if (UsePreviousBoxBounds)
				return position;
			DocumentModelPosition pos = this.modelPosition;
			if (pos.RunOffset != 0)
				return position;
			if (pos.LogPosition == pos.PieceTable.Paragraphs[pos.ParagraphIndex].LogPosition)
				return position;
			IVisibleTextFilter textFilter = pos.PieceTable.VisibleTextFilter;
			DocumentLogPosition logPosition = textFilter.GetPrevVisibleLogPosition(this.VirtualLogPosition, false);
			if (logPosition < DocumentLogPosition.Zero)
				return position;
			DocumentLayoutPosition result = GetCaretBoundsDocumentLayoutPosition(logPosition);
			result.Update(view.FormattingController.PageController.Pages, detailsLevel);
			Debug.Assert(result.IsValid(detailsLevel));
			return result;
		}
		protected internal virtual DocumentLayoutPosition GetCaretBoundsDocumentLayoutPosition(DocumentLogPosition logPosition) {
			DocumentModelPosition currentFormattingPosition = CalculateCurrentFormatingPosition(logPosition);
			if (caretBoundsPosition != null && caretBoundsPosition.LogPosition == currentFormattingPosition.LogPosition)
				return caretBoundsPosition;
			if (currentFormattingPosition.LogPosition == position.LogPosition)
				return position;
			DocumentLayoutPosition result = CreateCaretDocumentLayoutPosition();
			result.SetLogPosition(currentFormattingPosition.LogPosition);
			return result;
		}
		public virtual InputPosition GetInputPosition() {
			if (inputPosition == null)
				inputPosition = CreateInputPosition();
			return inputPosition;
		}
		public virtual InputPosition TryGetInputPosition() {
			return inputPosition;
		}
		protected internal virtual DocumentModelPosition CalculateCurrentFormatingPosition(DocumentLogPosition logPosition) {
			DocumentLogPosition formattingLogPosition = Algorithms.Max(logPosition - 1, DocumentLogPosition.Zero);
			PieceTable pieceTable = DocumentModel.Selection.PieceTable;
			RunInfo runInfo = new RunInfo(pieceTable);
			pieceTable.CalculateRunInfoStart(formattingLogPosition, runInfo);
			runInfo = pieceTable.FindRunInfo(formattingLogPosition, 1);
			TextRunBase run = pieceTable.Runs[runInfo.Start.RunIndex];
			if (!(run is TextRun))
				pieceTable.CalculateRunInfoStart(this.LogPosition, runInfo);
			return runInfo.Start;
		}
		protected internal virtual InputPosition CreateInputPosition() {
			return CreateInputPosition(this.LogPosition);
		}
		protected internal virtual InputPosition CreateInputPosition(DocumentLogPosition logPosition) {
			DocumentModelPosition currentFormattingPosition = CalculateCurrentFormatingPosition(logPosition);
			RunIndex runIndex = currentFormattingPosition.RunIndex;
			TextRunBase run = DocumentModel.ActivePieceTable.Runs[runIndex];
			InputPosition pos = new InputPosition(DocumentModel.ActivePieceTable);
			pos.LogPosition = logPosition;
			pos.ParagraphIndex = run.Paragraph.Index;
			RunIndex formattingRunIndex = runIndex;
			if (run is FieldResultEndRun) {
				Field field = DocumentModel.ActivePieceTable.FindFieldByRunIndex(formattingRunIndex);
				Debug.Assert(field != null);
				run = DocumentModel.ActivePieceTable.Runs[field.FirstRunIndex];
			}
			else if (!DocumentModel.ActivePieceTable.VisibleTextFilter.IsRunVisible(formattingRunIndex)) {
				formattingRunIndex = DocumentModel.ActivePieceTable.VisibleTextFilter.GetPrevVisibleRunIndex(runIndex);
				run = DocumentModel.ActivePieceTable.Runs[formattingRunIndex];
				if (!(run is TextRun)) {
					formattingRunIndex = DocumentModel.ActivePieceTable.VisibleTextFilter.GetNextVisibleRunIndex(runIndex);
					run = DocumentModel.ActivePieceTable.Runs[formattingRunIndex];
				}
			}
			pos.CharacterStyleIndex = run.CharacterStyleIndex;
			pos.CharacterFormatting.CopyFrom(run.CharacterProperties.Info);
			pos.MergedCharacterFormatting.CopyFrom(run.MergedCharacterFormatting);
			return pos;
		}
		public virtual Rectangle CalculateCaretBounds() {
			int width = view.DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(1);
			if (UsePreviousBoxBounds) {
				Rectangle result = Rectangle.Intersect(position.Character.TightBounds, position.Row.Bounds);
				result.Offset(result.Width, 0);
				result.Width = width;
				return result;
			}
			else {
				if (caretBoundsPosition == position || caretBoundsPosition.Row != position.Row) {
					Rectangle result = Rectangle.Intersect(position.Character.TightBounds, position.Row.Bounds);
					result.Width = width;
					return result;
				}
				else {
					Rectangle result = Rectangle.Intersect(caretBoundsPosition.Character.TightBounds, caretBoundsPosition.Row.Bounds);
					result.X = position.Character.TightBounds.X;
					result.Width = width;
					return result;
				}
			}
		}
		protected internal virtual DocumentLayoutPosition CreateCaretDocumentLayoutPosition() {
			return new CaretDocumentLayoutPosition(view);
		}
		protected internal virtual CaretPosition CreateExplicitCaretPosition(DocumentLogPosition logPosition) {
			return new ExplicitCaretPosition(this.View, logPosition, PreferredPageIndex);
		}
		protected internal virtual DragCaretPosition CreateDragCaretPosition() {
			return new DragCaretPosition(View, PreferredPageIndex);
		}
		protected internal virtual Rectangle GetRowBoundsRelativeToPage(){
			return LayoutPosition.Row.Bounds;
		}
	}
	#endregion
	#region DragCaretPosition
	public class DragCaretPosition : CaretPosition {
		DocumentLogPosition logPosition;
		public DragCaretPosition(RichEditView view, int preferredPageIndex)
			: base(view, preferredPageIndex) {
		}
		protected internal override DocumentLogPosition VirtualLogPosition { get { return logPosition; } }
		protected internal override DocumentLogPosition LogPosition { get { return logPosition; } }
		protected internal override bool UsePreviousBoxBounds { get { return false; } }
		internal void SetLogPosition(DocumentLogPosition logPosition) {
			if (this.logPosition == logPosition)
				return;
			this.logPosition = logPosition;
			Invalidate();
		}
		protected internal override bool UpdatePositionTrySetUsePreviousBoxBounds(DocumentLayoutDetailsLevel detailsLevel) {
			return false;
		}
	}
	#endregion
	#region CaretDocumentLayoutPosition
	public class CaretDocumentLayoutPosition : DocumentLayoutPosition {
		readonly RichEditView view;
		public CaretDocumentLayoutPosition(RichEditView view)
			: base(view.DocumentLayout, view.DocumentModel.MainPieceTable, DocumentLogPosition.Zero) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public override PieceTable PieceTable { get { return view.DocumentModel.ActivePieceTable; } }
		protected internal override void EnsureFormattingComplete() {
			view.EnsureFormattingCompleteForSelection();
		}
		protected internal override void EnsurePageSecondaryFormattingComplete(Page page) {
			view.EnsurePageSecondaryFormattingComplete(page);
		}
		protected internal override DocumentLayoutPosition CreateEmptyClone() {
			return new CaretDocumentLayoutPosition(view);
		}
	}
	#endregion
	#region ExplicitCaretPosition
	public class ExplicitCaretPosition : CaretPosition {
		readonly DocumentLogPosition logPosition;
		public ExplicitCaretPosition(RichEditView view, DocumentLogPosition logPosition, int preferredPageIndex)
			: base(view, preferredPageIndex) {
			this.logPosition = Algorithms.Max(DocumentLogPosition.Zero, Algorithms.Min(DocumentModel.MainPieceTable.DocumentEndLogPosition, logPosition));
		}
		#region Properties
		protected internal override DocumentLogPosition LogPosition { get { return logPosition; } }
		protected internal override DocumentLogPosition VirtualLogPosition { get { return logPosition; } }
		protected internal override bool UsePreviousBoxBounds { get { return false; } }
		#endregion
		protected internal override bool UpdatePositionTrySetUsePreviousBoxBounds(DocumentLayoutDetailsLevel detailsLevel) {
			return false;
		}
	}
	#endregion
	#region HeaderFooterCaretPosition
	public class HeaderFooterCaretPosition : CaretPosition {
		public HeaderFooterCaretPosition(RichEditView view, int preferredPageIndex)
			: base(view, preferredPageIndex) {
		}
		protected internal override DocumentLayoutPosition CreateCaretDocumentLayoutPosition() {
			return new HeaderFooterCaretDocumentLayoutPosition(View, PreferredPageIndex);
		}
		protected internal override CaretPosition CreateExplicitCaretPosition(DocumentLogPosition logPosition) {
			return new ExplicitHeaderFooterCaretPosition(this.View, logPosition, PreferredPageIndex);
		}
		protected internal override DragCaretPosition CreateDragCaretPosition() {
			return new HeaderFooterDragCaretPosition(View, PreferredPageIndex);
		}
	}
	#endregion
	#region HeaderFooterDragCaretPosition
	public class HeaderFooterDragCaretPosition : DragCaretPosition {
		public HeaderFooterDragCaretPosition(RichEditView view, int preferredPageIndex)
			: base(view, preferredPageIndex) {
		}
		protected internal override DocumentLayoutPosition CreateCaretDocumentLayoutPosition() {
			return new HeaderFooterCaretDocumentLayoutPosition(View, PreferredPageIndex);
		}
		protected internal override CaretPosition CreateExplicitCaretPosition(DocumentLogPosition logPosition) {
			return new ExplicitHeaderFooterCaretPosition(this.View, logPosition, PreferredPageIndex);
		}
	}
	#endregion
	#region HeaderFooterCaretDocumentLayoutPosition
	public class HeaderFooterCaretDocumentLayoutPosition : HeaderFooterDocumentLayoutPosition {
		readonly RichEditView view;
		public HeaderFooterCaretDocumentLayoutPosition(RichEditView view, int preferredPageIndex)
			: base(view.DocumentLayout, view.DocumentModel.MainPieceTable, DocumentLogPosition.Zero, preferredPageIndex) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public override PieceTable PieceTable { get { return view.DocumentModel.ActivePieceTable; } }
		protected internal override void EnsureFormattingComplete() {
			view.EnsureFormattingCompleteForPreferredPage(PreferredPageIndex);
		}
		protected internal override void EnsurePageSecondaryFormattingComplete(Page page) {
			Debug.Assert(view.DocumentLayout.Pages[PreferredPageIndex] == page);
			view.EnsurePageSecondaryFormattingComplete(page);			
		}
		protected internal override DocumentLayoutPosition CreateEmptyClone() {
			return new HeaderFooterCaretDocumentLayoutPosition(view, PreferredPageIndex);
		}
	}
	#endregion
	#region ExplicitHeaderFooterCaretPosition
	public class ExplicitHeaderFooterCaretPosition : HeaderFooterCaretPosition {
		readonly DocumentLogPosition logPosition;
		public ExplicitHeaderFooterCaretPosition(RichEditView view, DocumentLogPosition logPosition, int preferredPageIndex)
			: base(view, preferredPageIndex) {
			this.logPosition = Algorithms.Max(DocumentLogPosition.Zero, Algorithms.Min(DocumentModel.ActivePieceTable.DocumentEndLogPosition, logPosition));
		}
		#region Properties
		protected internal override DocumentLogPosition LogPosition { get { return logPosition; } }
		protected internal override DocumentLogPosition VirtualLogPosition { get { return logPosition; } }
		protected internal override bool UsePreviousBoxBounds { get { return false; } }
		#endregion
		protected internal override bool UpdatePositionTrySetUsePreviousBoxBounds(DocumentLayoutDetailsLevel detailsLevel) {
			return false;
		}
	}
	#endregion
	#region TextBoxCaretPosition
	public class TextBoxCaretPosition : CaretPosition {
		public TextBoxCaretPosition(RichEditView view, int preferredPageIndex)
		   : base(view, preferredPageIndex) {
		}
		protected internal override DocumentLayoutPosition CreateCaretDocumentLayoutPosition() {
			return new TextBoxCaretDocumentLayoutPosition(View, PreferredPageIndex);
		}
		protected internal override CaretPosition CreateExplicitCaretPosition(DocumentLogPosition logPosition) {
			return new ExplicitTextBoxCaretPosition(this.View, logPosition, PreferredPageIndex);
		}
		protected internal override DragCaretPosition CreateDragCaretPosition() {
			return new TextBoxDragCaretPosition(View, PreferredPageIndex);
		}
	}
	#endregion
	#region TextBoxDragCaretPosition
	public class TextBoxDragCaretPosition : DragCaretPosition {
		public TextBoxDragCaretPosition(RichEditView view, int preferredPageIndex)
			: base(view, preferredPageIndex) {
		}
		protected internal override DocumentLayoutPosition CreateCaretDocumentLayoutPosition() {
			return new TextBoxCaretDocumentLayoutPosition(View, PreferredPageIndex);
		}
		protected internal override CaretPosition CreateExplicitCaretPosition(DocumentLogPosition logPosition) {
			return new ExplicitTextBoxCaretPosition(this.View, logPosition, PreferredPageIndex);
		}
	}
	#endregion
	#region TextBoxCaretDocumentLayoutPosition
	public class TextBoxCaretDocumentLayoutPosition : TextBoxDocumentLayoutPosition {
		readonly RichEditView view;
		readonly TextBoxContentType textBoxContentType;
		public TextBoxCaretDocumentLayoutPosition(RichEditView view, int preferredPageIndex)
			: this(view, (TextBoxContentType)view.DocumentModel.ActivePieceTable.ContentType, preferredPageIndex) {
		}
		TextBoxCaretDocumentLayoutPosition(RichEditView view, TextBoxContentType textBoxPieceTable, int preferredPageIndex)
			: base(view.DocumentLayout, textBoxPieceTable, DocumentLogPosition.Zero, preferredPageIndex) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.textBoxContentType = textBoxPieceTable;
		}
		public override PieceTable PieceTable { get { return AnchorPieceTable != null ? AnchorPieceTable : textBoxContentType.PieceTable; } }
		protected internal override bool Update(PageCollection pages, DocumentLayoutDetailsLevel detailsLevel) {
			if (textBoxContentType == null || textBoxContentType.AnchorRun == null)
				return false;
			return base.Update(pages, detailsLevel);
		}
		protected internal override void EnsureFormattingComplete() {
			view.EnsureFormattingCompleteForLogPosition(textBoxContentType.AnchorRun.Paragraph.EndLogPosition);
		}
		protected internal override DocumentLayoutPosition CreateEmptyClone() {
			return new TextBoxCaretDocumentLayoutPosition(view, textBoxContentType, PreferredPageIndex);
		}
		public override bool IsValid(DocumentLayoutDetailsLevel detailsLevel) {
			if (textBoxContentType == null || textBoxContentType.AnchorRun == null)
				return false;
			return base.IsValid(detailsLevel);
		}
	}
	#endregion
	#region ExplicitTextBoxCaretPosition
	public class ExplicitTextBoxCaretPosition : TextBoxCaretPosition {
		readonly DocumentLogPosition logPosition;
		public ExplicitTextBoxCaretPosition(RichEditView view, DocumentLogPosition logPosition, int preferredPageIndex)
			: base(view, preferredPageIndex) {
			this.logPosition = Algorithms.Max(DocumentLogPosition.Zero, Algorithms.Min(DocumentModel.ActivePieceTable.DocumentEndLogPosition, logPosition));
		}
		#region Properties
		protected internal override DocumentLogPosition LogPosition { get { return logPosition; } }
		protected internal override DocumentLogPosition VirtualLogPosition { get { return logPosition; } }
		protected internal override bool UsePreviousBoxBounds { get { return false; } }
		#endregion
		protected internal override bool UpdatePositionTrySetUsePreviousBoxBounds(DocumentLayoutDetailsLevel detailsLevel) {
			return false;
		}
	}
	#endregion
	#region CommentCaretPosition
	public class CommentCaretPosition : CaretPosition {
		readonly PieceTable commentPieceTable;
		CommentViewInfo comment;
		public CommentCaretPosition(RichEditView view, int preferredPageIndex, PieceTable pieceTable)
			: base(view, preferredPageIndex) {
			this.commentPieceTable = pieceTable;
		}		
		public override Rectangle CalculateCaretBounds() {
			Rectangle result = base.CalculateCaretBounds();
			comment = FindCommentViewInfo();
			if (comment != null) {
				result.X += comment.ContentBounds.X;
				result.Y += comment.ContentBounds.Y;
			}
			return result;
		}
		internal CommentViewInfo FindCommentViewInfo() {
			if ((PageViewInfo == null) || (PageViewInfo.Page == null) || (PageViewInfo.Page.Comments == null))
				return null;
			CommentViewInfoHelper helper = new CommentViewInfoHelper();
			return helper.FindCommentViewInfo(PageViewInfo.Page, this.commentPieceTable);
		}
		protected internal override DocumentLayoutPosition CreateCaretDocumentLayoutPosition() {
			return new CommentCaretDocumentLayoutPosition(View, PreferredPageIndex);
		}
		protected internal override CaretPosition CreateExplicitCaretPosition(DocumentLogPosition logPosition) {
			return new ExplicitCommentCaretPosition(this.View, logPosition, PreferredPageIndex, commentPieceTable);
		}
		protected internal override DragCaretPosition CreateDragCaretPosition() {
			return new CommentDragCaretPosition(View, PreferredPageIndex);
		}
		protected internal override Rectangle GetRowBoundsRelativeToPage() {
			DocumentLayoutUnitConverter converter = View.DocumentModel.LayoutUnitConverter;
			Rectangle result = base.GetRowBoundsRelativeToPage();
			if (comment == null) {
				CommentViewInfoHelper helper = new CommentViewInfoHelper();
				comment = helper.FindCommentViewInfo(LayoutPosition.Page, this.commentPieceTable);
			}
			result.X += comment.ContentBounds.X;
			result.Y += comment.ContentBounds.Y;
			return result;
		}
		protected internal override void Invalidate() {
			base.Invalidate();
			this.comment = null;
		}
	}
	#endregion
	#region CommentDragCaretPosition
	public class CommentDragCaretPosition : DragCaretPosition {
		readonly PieceTable commentPieceTable;
		CommentViewInfo comment;
		public CommentDragCaretPosition(RichEditView view, int preferredPageIndex)
			: base(view, preferredPageIndex) {
				commentPieceTable = View.DocumentModel.ActivePieceTable;
		}
		protected internal override DocumentLayoutPosition CreateCaretDocumentLayoutPosition() {
			return new CommentDocumentLayoutPosition(View.DocumentLayout, (CommentContentType)View.DocumentModel.ActivePieceTable.ContentType, DocumentLogPosition.Zero, PreferredPageIndex);
		}
		protected internal override CaretPosition CreateExplicitCaretPosition(DocumentLogPosition logPosition) {
			return new ExplicitCommentCaretPosition(this.View, logPosition, PreferredPageIndex, View.DocumentModel.ActivePieceTable);
		}
		public override Rectangle CalculateCaretBounds() {
			Rectangle result = base.CalculateCaretBounds();
			comment = FindCommentViewInfo();
			if (comment != null) {
				result.X += comment.ContentBounds.X;
				result.Y += comment.ContentBounds.Y;
			}
			return result;
		}
		internal CommentViewInfo FindCommentViewInfo() {
			if ((PageViewInfo == null) || (PageViewInfo.Page == null) || (PageViewInfo.Page.Comments == null))
				return null;
			CommentViewInfoHelper helper = new CommentViewInfoHelper();
			return helper.FindCommentViewInfo(PageViewInfo.Page, this.commentPieceTable);
		}
		protected internal override Rectangle GetRowBoundsRelativeToPage() {
			DocumentLayoutUnitConverter converter = View.DocumentModel.LayoutUnitConverter;
			Rectangle result = base.GetRowBoundsRelativeToPage();
			if (comment == null) {
				CommentViewInfoHelper helper = new CommentViewInfoHelper();
				comment = helper.FindCommentViewInfo(LayoutPosition.Page, this.commentPieceTable);
			}
			result.X += comment.ContentBounds.X;
			result.Y += comment.ContentBounds.Y;
			return result;
		}
		protected internal override void Invalidate() {
			base.Invalidate();
			this.comment = null;
		}
	}
	#endregion
	#region CommentCaretDocumentLayoutPosition
	public class CommentCaretDocumentLayoutPosition : CommentDocumentLayoutPosition {
		readonly RichEditView view;
		readonly CommentContentType commentContentType;
		public CommentCaretDocumentLayoutPosition(RichEditView view, int preferredPageIndex)
			: this(view, (CommentContentType)view.DocumentModel.ActivePieceTable.ContentType, preferredPageIndex) {
		}
		CommentCaretDocumentLayoutPosition(RichEditView view, CommentContentType commentPieceTable, int preferredPageIndex)
			: base(view.DocumentLayout, commentPieceTable, commentPieceTable.ReferenceComment.Start, preferredPageIndex) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.commentContentType = commentPieceTable;
		}
		public override PieceTable PieceTable { get { return AnchorPieceTable != null ? AnchorPieceTable : commentContentType.PieceTable; } }
		protected internal override bool Update(PageCollection pages, DocumentLayoutDetailsLevel detailsLevel) {
			if (commentContentType == null)
				return false;
			return base.Update(pages, detailsLevel);
		}
		protected internal override void EnsureFormattingComplete() {
			view.EnsureFormattingCompleteForLogPosition(commentContentType.ReferenceComment.End);
		}
		protected internal override void EnsurePageSecondaryFormattingComplete(Page page) {
		}
		protected internal override DocumentLayoutPosition CreateEmptyClone() {
			return new CommentCaretDocumentLayoutPosition(view, commentContentType, PreferredPageIndex);
		}
		public override bool IsValid(DocumentLayoutDetailsLevel detailsLevel) {
			return base.IsValid(detailsLevel);
		}
	}
	#endregion
	#region ExplicitCommentCaretPosition
	public class ExplicitCommentCaretPosition : CommentCaretPosition {
		readonly DocumentLogPosition logPosition;
		public ExplicitCommentCaretPosition(RichEditView view, DocumentLogPosition logPosition, int preferredPageIndex, PieceTable pieceTable)
			: base(view, preferredPageIndex, pieceTable) {
			this.logPosition = Algorithms.Max(DocumentLogPosition.Zero, Algorithms.Min(DocumentModel.ActivePieceTable.DocumentEndLogPosition, logPosition));
		}
		#region Properties
		protected internal override DocumentLogPosition LogPosition { get { return logPosition; } }
		protected internal override DocumentLogPosition VirtualLogPosition { get { return logPosition; } }
		protected internal override bool UsePreviousBoxBounds { get { return false; } }
		#endregion
		protected internal override bool UpdatePositionTrySetUsePreviousBoxBounds(DocumentLayoutDetailsLevel detailsLevel) {
			return false;
		}
	}
	#endregion
}

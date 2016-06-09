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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands.Internal;
namespace DevExpress.XtraRichEdit.Model.History {
	#region DeleteBookmarkBaseHistoryItem<T> abstract class
	public abstract class DeleteBookmarkBaseHistoryItem<T> : RichEditHistoryItem where T : BookmarkBase {
		#region Fields
		T deletedBookmark;
		int deletedBookmarkIndex = -1;
		#endregion
		protected DeleteBookmarkBaseHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int DeletedBookmarkIndex { get { return deletedBookmarkIndex; } set { deletedBookmarkIndex = value; } }
		protected T DeletedBookmark { get { return deletedBookmark; } set { deletedBookmark = value; } }
		#endregion
		protected override void RedoCore() {
			BookmarkBaseCollection<T> bookmarks = GetBookmarkCollection();
			this.deletedBookmark = bookmarks[this.deletedBookmarkIndex];
			bookmarks.RemoveAt(this.deletedBookmarkIndex);
		}
		protected override void UndoCore() {
			GetBookmarkCollection().Insert(this.deletedBookmarkIndex, this.deletedBookmark);
		}
		protected internal abstract BookmarkBaseCollection<T> GetBookmarkCollection();
	}
	#endregion
	#region DeleteBookmarkHistoryItem
	public class DeleteBookmarkHistoryItem : DeleteBookmarkBaseHistoryItem<Bookmark> {
		public DeleteBookmarkHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override BookmarkBaseCollection<Bookmark> GetBookmarkCollection() {
			return PieceTable.Bookmarks;
		}
	}
	#endregion
	#region DeleteCommentHistoryItem
	public class DeleteCommentHistoryItem : DeleteBookmarkBaseHistoryItem<Comment> {
		public DeleteCommentHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override BookmarkBaseCollection<Comment> GetBookmarkCollection() {
			return PieceTable.Comments;
		}
		public override void Execute() {			
			DocumentModel.BeginUpdate();
			base.RedoCore();
			DeletedBookmark.Content.ReferenceComment = null;
			PieceTable.OnCommentDeleted(DeletedBookmark, DeletedBookmarkIndex, true);
			DocumentModel.Selection.Start = DeletedBookmark.End;
			DocumentModel.Selection.End = DeletedBookmark.End;
			DocumentModel.EndUpdate();
		}
		protected override void RedoCore() {
			DocumentModel.BeginUpdate();
			base.RedoCore();
			DeletedBookmark.Content.ReferenceComment = null;
			PieceTable.OnCommentDeleted(DeletedBookmark, DeletedBookmarkIndex, false);
			DocumentModel.Selection.Start = DeletedBookmark.End;
			DocumentModel.Selection.End = DeletedBookmark.End;
			DocumentModel.EndUpdate();
		}
		protected override void UndoCore() {
			DocumentModel.BeginUpdate();
			base.UndoCore();
			DeletedBookmark.Content.ReferenceComment = DeletedBookmark;
			PieceTable.OnCommentInserted(DeletedBookmark, DeletedBookmarkIndex, false);
			DocumentModel.EndUpdate();
		}
	}
	#endregion
	#region InsertDocumentIntervalHistoryItem (abstract class)
	public abstract class InsertDocumentIntervalHistoryItem : RichEditHistoryItem {
		#region Fields
		int length;
		DocumentLogPosition position;
		int indexToInsert = -1;
		#endregion
		protected InsertDocumentIntervalHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int Length { get { return length; } set { length = value; } }
		public DocumentLogPosition Position { get { return position; } set { position = value; } }
		public int IndexToInsert { get { return indexToInsert; } set { indexToInsert = value; } }
		#endregion
	}
	#endregion
	#region InsertBookmarkBaseHistoryItem<T> (abstract class)
	public abstract class InsertBookmarkBaseHistoryItem<T> : InsertDocumentIntervalHistoryItem where T : BookmarkBase {
		T bookmark;
		protected InsertBookmarkBaseHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected T Bookmark { get { return bookmark; } set { bookmark = value; } }
		public override void Execute() {
			ExecuteCore();
			base.Execute();
		}
		protected virtual void ExecuteCore() {
			this.bookmark = CreateBookmark(Algorithms.Min(Position, PieceTable.DocumentEndLogPosition), Algorithms.Min(Position + Length, PieceTable.DocumentEndLogPosition));
			if (Length == 0)
				bookmark.CanExpand = false;
		}
		protected override void RedoCore() {
			IndexToInsert = GetBookmarkCollection().Add(bookmark);
		}
		protected override void UndoCore() {
			GetBookmarkCollection().RemoveAt(IndexToInsert);
		}
		protected internal abstract T CreateBookmark(DocumentLogPosition start, DocumentLogPosition end);
		protected internal abstract BookmarkBaseCollection<T> GetBookmarkCollection();
	}
	#endregion
	#region InsertBookmarkHistoryItem
	public class InsertBookmarkHistoryItem : InsertBookmarkBaseHistoryItem<Bookmark> {
		string bookmarkName = String.Empty;
		bool forceUpdateInterval;
		public InsertBookmarkHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public string BookmarkName { get { return bookmarkName; } set { bookmarkName = value; } }
		public bool ForceUpdateInterval { get { return forceUpdateInterval; } set { forceUpdateInterval = value; } }
		protected internal override Bookmark CreateBookmark(DocumentLogPosition start, DocumentLogPosition end) {
			Bookmark bookmark = new Bookmark(PieceTable, start, end, this.forceUpdateInterval);
			bookmark.Name = BookmarkName;
			return bookmark;
		}
		protected internal override BookmarkBaseCollection<Bookmark> GetBookmarkCollection() {
			return PieceTable.Bookmarks;
		}
	}
	#endregion
	#region InsertCommentHistoryItem
	public class InsertCommentHistoryItem : InsertBookmarkBaseHistoryItem<Comment> {
		public InsertCommentHistoryItem(PieceTable pieceTable) : base(pieceTable) {
			Bookmark = new Comment(pieceTable, DocumentLogPosition.Zero, DocumentLogPosition.Zero);
		}
		#region Properties
		public string Name { get { return Bookmark.Name; } set { Bookmark.Name = value; } }
		public string Author { get { return Bookmark.Author; } set { Bookmark.Author = value; } }
		public DateTime Date { get { return Bookmark.Date; } set { Bookmark.Date = value; } }
		public CommentContentType Content { get { return Bookmark.Content; } set { Bookmark.Content = value; } }
		public Comment ParentComment { get { return Bookmark.ParentComment; } set { Bookmark.ParentComment = value; } }
		#endregion
		protected internal override Comment CreateBookmark(DocumentLogPosition start, DocumentLogPosition end) {
			Comment comment= new Comment(PieceTable, start, end);
			comment.Name = Name;
			comment.Author = Author;
			comment.Date = Date;
			comment.Content = Content;
			comment.ParentComment = ParentComment;
			Bookmark = comment;						
			Bookmark.Content.ReferenceComment = Bookmark; 
			return Bookmark;
		}
		protected internal override BookmarkBaseCollection<Comment> GetBookmarkCollection() {
			return PieceTable.Comments;
		}
		public override void Execute() {
			ExecuteCore();
			DocumentModel.BeginUpdate();
			base.RedoCore();
			Bookmark.Content.ReferenceComment = Bookmark;
			PieceTable.OnCommentInserted(Bookmark, IndexToInsert, true);
			DocumentModel.EndUpdate();
		}
		protected override void RedoCore() {
			DocumentModel.BeginUpdate();
			base.RedoCore();
			Bookmark.Content.ReferenceComment = Bookmark;
			PieceTable.OnCommentInserted(Bookmark, IndexToInsert, false);
			DocumentModel.EndUpdate();
		}
		protected override void UndoCore() {
			DocumentModel.BeginUpdate();
			Bookmark.Content.ReferenceComment = null;
			base.UndoCore();
			PieceTable.OnCommentDeleted(Bookmark, IndexToInsert, false  );
			DocumentModel.EndUpdate();
		}
	}
	#endregion
	#region InsertHyperlinkInfoHistoryItem
	public class InsertHyperlinkInfoHistoryItem : RichEditHistoryItem {
		#region Fields
		int fieldIndex;
		HyperlinkInfo hyperlinkInfo;
		#endregion
		public InsertHyperlinkInfoHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int FieldIndex { get { return fieldIndex; } set { fieldIndex = value; } }
		public HyperlinkInfo HyperlinkInfo { get { return hyperlinkInfo; } set { hyperlinkInfo = value; } }
		#endregion
		protected override void RedoCore() {
			PieceTable.HyperlinkInfos.Add(fieldIndex, hyperlinkInfo);
			DocumentModel.RaiseHyperlinkInfoInserted(PieceTable, fieldIndex);
		}
		protected override void UndoCore() {
			PieceTable.HyperlinkInfos.Remove(fieldIndex);
			DocumentModel.RaiseHyperlinkInfoDeleted(PieceTable, fieldIndex);
		}
	}
	#endregion
	#region DeleteHyperlinkInfoHistoryItem
	public class DeleteHyperlinkInfoHistoryItem : RichEditHistoryItem {
		#region Fields
		HyperlinkInfo deletedHyperlinkInfo;
		int fieldIndex;
		#endregion
		public DeleteHyperlinkInfoHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int FieldIndex { get { return fieldIndex; } set { fieldIndex = value; } }
		#endregion
		protected override void RedoCore() {
			HyperlinkInfoCollection hyperlinkInfos = PieceTable.HyperlinkInfos;
			this.deletedHyperlinkInfo = hyperlinkInfos[fieldIndex];
			hyperlinkInfos.Remove(fieldIndex);
			DocumentModel.RaiseHyperlinkInfoDeleted(PieceTable, fieldIndex);
		}
		protected override void UndoCore() {
			PieceTable.HyperlinkInfos.Add(fieldIndex, deletedHyperlinkInfo);
			DocumentModel.RaiseHyperlinkInfoInserted(PieceTable, fieldIndex);
		}
	}
	#endregion
	#region ReplaceHyperlinkInfoHistoryItem
	public class ReplaceHyperlinkInfoHistoryItem : RichEditHistoryItem {
		#region Fields
		int fieldIndex = -1;
		HyperlinkInfo oldInfo;
		HyperlinkInfo newInfo;
		#endregion
		public ReplaceHyperlinkInfoHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int FieldIndex { get { return fieldIndex; } set { fieldIndex = value; } }
		public HyperlinkInfo OldInfo { get { return oldInfo; } set { oldInfo = value; } }
		public HyperlinkInfo NewInfo { get { return newInfo; } set { newInfo = value; } }
		#endregion
		protected override void UndoCore() {
			PieceTable.HyperlinkInfos[fieldIndex] = oldInfo;
		}
		protected override void RedoCore() {
			PieceTable.HyperlinkInfos[fieldIndex] = newInfo;
		}
	}
	#endregion
	#region InsertCustomMarkHistoryItem
	public class InsertCustomMarkHistoryItem : RichEditHistoryItem {
		#region Fields
		int customMarkIndex;
		CustomMark customMark;
		#endregion
		public InsertCustomMarkHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int CustomMarkIndex { get { return customMarkIndex; } set { customMarkIndex = value; } }
		public CustomMark CustomMark { get { return customMark; } set { customMark = value; } }
		#endregion
		protected override void RedoCore() {
			PieceTable.CustomMarks.Insert(customMarkIndex, CustomMark);
			DocumentModel.RaiseCustomMarkInserted(PieceTable, customMarkIndex);
		}
		protected override void UndoCore() {
			PieceTable.CustomMarks.RemoveAt(CustomMarkIndex);
			DocumentModel.RaiseCustomMarkDeleted(PieceTable, customMarkIndex);
		}
	}
	#endregion
	#region DeleteCustomMarkHistoryItem
	public class DeleteCustomMarkHistoryItem : RichEditHistoryItem {
		#region Fields
		CustomMark deletedCustomMark;
		int customMarkIndex;
		#endregion
		public DeleteCustomMarkHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int CustomMarkIndex { get { return customMarkIndex; } set { customMarkIndex = value; } }
		#endregion
		protected override void RedoCore() {
			CustomMarkCollection customMarks = PieceTable.CustomMarks;
			this.deletedCustomMark = customMarks[customMarkIndex];
			customMarks.RemoveAt(customMarkIndex);
			DocumentModel.RaiseCustomMarkDeleted(PieceTable, customMarkIndex);
		}
		protected override void UndoCore() {
			PieceTable.CustomMarks.Insert(customMarkIndex, deletedCustomMark);
			DocumentModel.RaiseCustomMarkInserted(PieceTable, customMarkIndex);
		}
	}
	#endregion
}

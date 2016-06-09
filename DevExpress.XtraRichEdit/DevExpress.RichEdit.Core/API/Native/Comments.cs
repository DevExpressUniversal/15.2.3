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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.API.Native {
	#region Comment
	[ComVisible(true)]
	public interface Comment {
		string Name { get; set; }
		string Author { get; set; }
		DateTime Date { get; set; }
		Comment ParentComment { get; }
		DocumentRange Range { get; }
		SubDocument BeginUpdate();
		void EndUpdate(SubDocument document);
	}
	#endregion
	#region ReadOnlyCommentCollection
	[ComVisible(true)]
	public interface ReadOnlyCommentCollection : ISimpleCollection<Comment> {
		ReadOnlyCommentCollection Get(DocumentRange range);
	}
	#endregion
	#region CommentCollection
	[ComVisible(true)]
	public interface CommentCollection: ReadOnlyCommentCollection {
		Comment this[string name] { get; }
		Comment Create(DocumentRange range, string author, DateTime date);
		Comment Create(DocumentRange range, string author);
		Comment Create(string author, DateTime date, Comment parentComment);
		Comment Create(string author, Comment parentComment);
		Comment Create(DocumentPosition start, int length, string author, DateTime date);
		Comment Create(DocumentPosition start, int length, string author);
		void Select(Comment comment);
		void Remove(Comment comment);
		void RemoveAt(int index);
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelComment = DevExpress.XtraRichEdit.Model.Comment;
	using ModelDocumentInterval = DevExpress.XtraRichEdit.Model.DocumentInterval;
	using ModelCommentCollection = DevExpress.XtraRichEdit.Model.CommentCollection;
	using DocumentModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	using ModelDocumentIntervalEventHandler = DevExpress.XtraRichEdit.Model.DocumentIntervalEventHandler;
	using ModelDocumentIntervalEventArgs = DevExpress.XtraRichEdit.Model.DocumentIntervalEventArgs;
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using ModelDocumentModel = DevExpress.XtraRichEdit.Model.DocumentModel;
	using ModelRunIndex = DevExpress.XtraRichEdit.Model.RunIndex;
	using DevExpress.Office.Utils;
	using DevExpress.XtraRichEdit.Utils;
	using DevExpress.XtraRichEdit.Localization;
	using DevExpress.Utils;
	using DevExpress.XtraRichEdit.Model.History;
	#region NativeComment
	public class NativeComment : NativeDocumentInterval<ModelComment>, Comment {
		readonly NativeSubDocument document;
		ModelComment comment;
		public NativeComment(NativeSubDocument document, ModelComment comment)
			: base(document, comment) {
			this.document = document;
			this.comment = comment;
		}
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		ModelDocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		public ModelComment Comment { get { return comment; } }
		#region Comment Members
		public string Name {
			get {
				CheckValid();
				return DocumentInterval.Name;
			}
			set {
				if (DocumentInterval.Name == value)
					return;
				AddCommentPropertyHistoryItem(new ChangeCommentNameHistoryItem(PieceTable, comment, value, DocumentInterval.Name));
			}
		}
		public DocumentRange Range {
			get {
				CheckValid();
				return GetRange();
			}
		}
		public string Author {
			get {
				CheckValid();
				return DocumentInterval.Author;
			}
			set {
				if (DocumentInterval.Author == value) 
					return;
				AddCommentPropertyHistoryItem(new ChangeCommentAuthorHistoryItem(PieceTable, comment, value, DocumentInterval.Author));
			}
		}
		public DateTime Date {
			get {
				CheckValid();
				return DocumentInterval.Date;
			}
			set {
				if (DocumentInterval.Date == value)
					return;
				AddCommentPropertyHistoryItem(new ChangeCommentDateHistoryItem(PieceTable, comment, Convert.ToString(value), Convert.ToString(DocumentInterval.Date)));
			}
		}
		public Comment ParentComment {
			get {
				CheckValid();
				return ((Comment)DocumentInterval).ParentComment;
			}
		}
		void AddCommentParentCommentHistoryItem(ChangeCommentParentCommentHistoryItem item) {
			DocumentModel.CaculateVisibleAuthors();
			DocumentModel.BeginUpdate();
			DocumentModel.History.Add(item);
			item.Execute();
			item.PieceTable.OnCommentChangedViaAPI(item.Comment, item.Comment.Index);
			DocumentModel.EndUpdate();
			DocumentModel.CaculateVisibleAuthors();
		}
		void AddCommentPropertyHistoryItem(ChangeCommentPropertyHistoryItem item) {
			DocumentModel.CaculateVisibleAuthors();
			DocumentModel.BeginUpdate();
			DocumentModel.History.Add(item);
			item.Execute();
			item.PieceTable.OnCommentChangedViaAPI(item.Comment, item.Comment.Index);
			DocumentModel.EndUpdate();
			DocumentModel.CaculateVisibleAuthors();
		}
		public SubDocument BeginUpdate() {
			CheckValid();
			comment.DocumentModel.BeginUpdate();
			return new NativeSubDocument(comment.Content.PieceTable, document.DocumentServer);
		}
		public void EndUpdate(SubDocument document) {
			CheckValid();
			NativeSubDocument nativeDocument = (NativeSubDocument)document;
			nativeDocument.ReferenceCount--;
			comment.DocumentModel.ActivePieceTable.OnCommentChangedViaAPI(comment, comment.Index);
			comment.DocumentModel.EndUpdate();
		}
		#endregion
		protected override void CheckValid() {
			if (!IsValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedCommentError);
		}
	}
	#endregion
	#region NativeCommentCollection
	public class NativeCommentCollection : List<NativeComment>, CommentCollection {
		readonly NativeSubDocument document;
		int lastInsertedCommentIndex = -1;
		public NativeCommentCollection(NativeSubDocument document, bool readOnly) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
			if(!readOnly)
				SubscribeCommentCollectionEvents();
		}
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		public NativeComment LastInsertedComment {
			get {
				if (lastInsertedCommentIndex < 0)
					return null;
				return this[lastInsertedCommentIndex];
			}
		}
		protected internal virtual void Invalidate() {
			UnsubscribeCommentCollectionEvents();
			Clear();
		}
		void SubscribeCommentCollectionEvents() {
			ModelCommentCollection comments = PieceTable.Comments;
			comments.Inserted += new ModelDocumentIntervalEventHandler(OnCommentInserted);
			comments.Removed += new ModelDocumentIntervalEventHandler(OnCommentRemoved);
		}
		void UnsubscribeCommentCollectionEvents() {
			ModelCommentCollection comments = PieceTable.Comments;
			comments.Inserted -= new ModelDocumentIntervalEventHandler(OnCommentInserted);
			comments.Removed -= new ModelDocumentIntervalEventHandler(OnCommentRemoved);
		}
		void OnCommentInserted(object sender, ModelDocumentIntervalEventArgs e) {
			ModelComment comment = PieceTable.Comments[e.Index];
			this.Insert(e.Index, new NativeComment(document, comment));
			this.lastInsertedCommentIndex = e.Index;
		}
		void OnCommentRemoved(object sender, ModelDocumentIntervalEventArgs e) {
			this[e.Index].IsValid = false;
			this.RemoveAt(e.Index);
		}
		NativeComment FindByName(string name) {
			for (int i = 0; i < Count; i++) {
				NativeComment comment = this[i];
				if (StringExtensions.CompareInvariantCulture(comment.Name, name) == 0)
					return comment;
			}
			return null;
		}
		#region ISimpleCollection<Comment> Members
		Comment CommentCollection.this[string name] {
			get { return FindByName(name); }
		}
		Comment ISimpleCollection<Comment>.this[int index] {
			get { return this[index]; }
		}
		#endregion
		#region IEnumerable<Comment> Members
		IEnumerator<Comment> IEnumerable<Comment>.GetEnumerator() {
			return new EnumeratorAdapter<Comment, NativeComment>(this.GetEnumerator());
		}
		#endregion
		public Comment Create(string author, DateTime date, Comment parentComment) {
			return Create(parentComment.Range.Start, parentComment.Range.Length, author, date, parentComment);
		}
		public Comment Create(DocumentRange range, string author, DateTime date) {
			return Create(range.Start, range.Length, author, date);
		}
		public Comment Create(DocumentPosition start, int length, string author, DateTime date) {
			document.CheckValid();
			Guard.ArgumentNotNull(start, "start");
			Guard.ArgumentNonNegative(length, "length");
			CommentContentType content = new CommentContentType(PieceTable.DocumentModel);
			document.CheckDocumentPosition(start);
			PieceTable.CreateComment(document.NormalizeLogPosition(start.LogPosition), length, author.Trim(), date, null, content);
			return LastInsertedComment;
		}
		public Comment Create(DocumentPosition start, int length, string author, DateTime date, Comment parentComment) {
			document.CheckValid();
			Guard.ArgumentNotNull(start, "start");
			Guard.ArgumentNonNegative(length, "length");
			Guard.ArgumentNotNull(parentComment, "parentComment");
			CommentContentType content = new CommentContentType(PieceTable.DocumentModel);
			document.CheckDocumentPosition(start);
			PieceTable.CreateComment(document.NormalizeLogPosition(start.LogPosition), length, author.Trim(), date, ((NativeComment)parentComment).Comment, content);
			return LastInsertedComment;
		}
		public Comment Create(DocumentRange range, string author) {
			return Create(range, author, DateTime.Now);
		}
		public Comment Create(string author, Comment parentComment) {
			return Create(parentComment.Range.Start, parentComment.Range.Length, author, DateTime.Now, parentComment);
		}
		public Comment Create(DocumentPosition start, int length, string author) {
			return Create(start, length, author, DateTime.Now); 
		}
		public void Select(Comment comment) {
			if (!Object.ReferenceEquals(PieceTable, document.DocumentModel.ActivePieceTable))
				throw new InvalidOperationException(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_SelectCommentError));
			document.CheckValid();
			Guard.ArgumentNotNull(comment, "comment");
			document.CheckDocumentRange(comment.Range);
			NativeDocumentRange range = (NativeDocumentRange)comment.Range;
			document.SetSelectionCore(range.Start.LogPosition, range.End.LogPosition);
		}
		public void Remove(Comment comment) {
			document.CheckValid();
			Guard.ArgumentNotNull(comment, "comment");
			document.BeginUpdate();
			CommentsHelper helper = new CommentsHelper(PieceTable);
			helper.DeleteNestedComments(((NativeComment)comment).Comment);
			PieceTable.DeleteComment(((NativeComment)comment).Comment.Index);
			document.EndUpdate();
		}
		void CommentCollection.RemoveAt(int index) {
			PieceTable.DeleteComment(index);
		}
		public ReadOnlyCommentCollection Get(DocumentRange range) {
			document.CheckValid();
			document.CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			DocumentModelPosition startRange = nativeRange.NormalizedStart.Position;
			DocumentModelPosition endRange = nativeRange.NormalizedEnd.Position;
			NativeCommentCollection result = new NativeCommentCollection(document, true);
			int count = PieceTable.Comments.Count;
			for (int i = 0; i < count; i++) {
				DocumentModelPosition startComment = PieceTable.Comments[i].Interval.Start;
				DocumentModelPosition endComment = PieceTable.Comments[i].Interval.End;
				if ((startComment >= startRange) && (endComment <= endRange)) {
					NativeComment comment = new NativeComment(document, PieceTable.Comments[i]);
					result.Add(comment);
				}
			}
			return result;
		}
	}
	#endregion        
}

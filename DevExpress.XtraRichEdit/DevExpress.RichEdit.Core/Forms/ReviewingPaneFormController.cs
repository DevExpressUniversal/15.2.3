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
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Forms {
	public class ReviewingPaneFormController {
		readonly DocumentModel documentModel;		
		readonly List<Comment> comments;
		Dictionary<string, Comment> commentsBookmarkName = new Dictionary<string,Comment>();
		public ReviewingPaneFormController(DocumentModel documentModel) {
			this.documentModel = documentModel;
			CommentCollection sourceComments = documentModel.MainPieceTable.Comments;
			int count = sourceComments.Count;
			comments = new List<Comment>(count);
			for (int i = 0; i < count; i++)
				comments.Add(sourceComments[i]);
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public List<Comment> Comments { get { return comments; } }
		public Dictionary<string, Comment> CommentsBookmarkName { get { return commentsBookmarkName; } }
		public void FillRichEditControl(DocumentModel targetModel, int rightTabPosition, CommentIdProvider idProvider, CommentColorer colorer, bool keepHistory) {
			Guard.ArgumentNotNull(targetModel, "targetModel");
			Guard.ArgumentNotNull(idProvider, "provider");
			Guard.ArgumentNotNull(colorer, "colorer");
			ReviewingPaneCommentCreator creator = new ReviewingPaneCommentCreator(idProvider, targetModel, this);
			if (!keepHistory) {
				targetModel.BeginSetContent();
				creator.CreateCommentHeadingParagraphStyle();
				creator.ChangeCommentHeadingParagraphStyle(rightTabPosition);
			}
			else {
				targetModel.BeginUpdate();
				targetModel.MainPieceTable.DeleteContent(DocumentLogPosition.Zero, targetModel.MainPieceTable.DocumentEndLogPosition - targetModel.MainPieceTable.DocumentStartLogPosition, true);
			}
			PieceTable targetPieceTable = targetModel.MainPieceTable;
			DocumentLogPosition logPosition = targetPieceTable.DocumentStartLogPosition;
			targetModel.CommentColorer = colorer;
			int count = Comments.Count;
			for (int i = 0; i < count; i++) {
				Comment sourceComment = Comments[i];
				logPosition = creator.CopyComment(sourceComment, i, logPosition, 0, idProvider.GetCommentId(sourceComment));
			}
			commentsBookmarkName = creator.CommentsBookmarkName;
			if (!keepHistory) {
				targetModel.AuthenticationOptions.UserName = string.Empty;
				targetModel.EnforceDocumentProtection(string.Empty);
				targetModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, new FieldUpdateOnLoadOptions(false, false));
			}
			else {
				targetModel.EndUpdate();
			}
		}
		public void SetCursor(DocumentModel targetModel, CommentIdProvider idProvider) {
			if ((documentModel.MainPieceTable != null) && (documentModel.MainPieceTable.Comments != null) && (documentModel.MainPieceTable.Comments.Count > 0)) {
				Comment sourceComment = documentModel.MainPieceTable.Comments[0];
				SetSelect(targetModel, sourceComment, false, idProvider, false, DocumentLogPosition.Zero, DocumentLogPosition.Zero);
			}
		}
		public void SetSelect(DocumentModel targetModel, Comment sourceComment, bool scrollToCaret, CommentIdProvider provider, bool selectParagraph, DocumentLogPosition start, DocumentLogPosition end) {
			ReviewingPaneCommentCreator creator = new ReviewingPaneCommentCreator(provider, targetModel, this);
			int shiftStart = start - DocumentLogPosition.Zero;
			int shiftEnd = end - start;
			targetModel.Selection.Start = creator.CalculateStartLogPositionCore(sourceComment, scrollToCaret) + shiftStart;
			targetModel.Selection.End = targetModel.Selection.Start + shiftEnd;
			if (selectParagraph)
				targetModel.Selection.End = targetModel.Selection.Start + CalculateParagraphLength(sourceComment);
		}
		int CalculateParagraphLength(Comment comment) {
			return comment.Content.PieceTable.Paragraphs[new ParagraphIndex(0)].EndLogPosition - comment.Content.PieceTable.DocumentStartLogPosition + 1;
		}		
		public Comment GetCommentFromBookmarkName(string bookmarkName){
			Comment result;
			if (CommentsBookmarkName.TryGetValue(bookmarkName, out result)) {
				return result;
			}
			return null;
		}		
	}
	public class CommentTextFilter : VisibleTextFilterBase {
		#region BookmarkLogPositionComparer
		class BookmarkLogPositionComparer : IComparable<Bookmark> {
			readonly DocumentLogPosition logPosition;
			public BookmarkLogPositionComparer(DocumentLogPosition logPosition) {
				this.logPosition = logPosition;
			}
			#region IComparable<Hyperlink> Members
			public int CompareTo(Bookmark other) {
				if (logPosition < other.Start)
					return 1;
				if (logPosition > other.Start)
					return -1;
				return 0;
			}
			#endregion
		}
		#endregion
		readonly IVisibleTextFilter baseFilter;
		readonly ReviewingPaneFormController controller;
		readonly CommentOptions options;
		public CommentTextFilter(CommentOptions options, ReviewingPaneFormController controller, PieceTable pieceTable, IVisibleTextFilter baseFilter)
			: base(pieceTable) {
			this.baseFilter = baseFilter;
			this.controller = controller;
			this.options = options;
		}
		public override bool IsRunVisible(RunIndex runIndex) {
			if (!IsCommentRunVisible(runIndex))
				return false;
			else
				return baseFilter.IsRunVisible(runIndex);
		}
		bool IsCommentRunVisible(RunIndex runIndex) {
			ParagraphRun paragraphRun = PieceTable.Runs[runIndex] as ParagraphRun;
			if (options.ShowAllAuthors)
				return true;
			DocumentModelPosition position = DocumentModelPosition.FromRunStart(PieceTable, runIndex);
			BookmarkCollection bookmarks = PieceTable.Bookmarks;
			int index = bookmarks.BinarySearch(new BookmarkLogPositionComparer(position.LogPosition));
			if (index < 0) {
				index = ~index - 1;
				if (index < 0)
					return true;
				Bookmark bookmark = bookmarks[index];
				if (position.LogPosition >= bookmarks[index].End)
					return true;
			}
			Comment comment = controller.GetCommentFromBookmarkName(bookmarks[index].Name);
			if (comment == null)
				comment = controller.GetCommentFromBookmarkName(bookmarks[index].Name + "comment");
			if (comment == null)
				return true;
			if (comment.ParentComment == null)
				return options.VisibleAuthors.Contains(comment.Author);
			return (options.VisibleAuthors.Contains(comment.Author) && options.VisibleAuthors.Contains(comment.ParentComment.Author));
		}
		public override IVisibleTextFilter Clone(PieceTable pieceTable) {
			return new CommentTextFilter(options, controller, pieceTable, baseFilter);
		}
	}
	public class CommentIdProvider {
		#region Fields
		Dictionary<Comment, int> commentsId;
		int nextId;
		#endregion
		public CommentIdProvider() {
			commentsId = new Dictionary<Comment, int>();
		}
		#region Properties
		internal Dictionary<Comment, int> CommentsId { get { return commentsId; } }
		internal int Id { get { return nextId; } set { nextId = value; } }
		#endregion
		public int GetCommentId(Comment comment) {
			int result;
			if (CommentsId.TryGetValue(comment, out result)) {
				return result;
			}
			CommentsId.Add(comment, nextId);
			nextId++;
			return nextId - 1;
		}
		public void Initialize(CommentCollection comments) {
			commentsId.Clear();
			int count = comments.Count;
			for (int i = 0; i < count; i++)
				commentsId.Add(comments[i], i);
			 nextId = count;
		}
	}
}

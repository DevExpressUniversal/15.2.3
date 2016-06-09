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
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region CommentContentType
	public class CommentContentType : ContentTypeBase {
		Comment referenceComment;
		public CommentContentType(DocumentModel documentModel)
			: base(documentModel) {
			documentModel.UnsafeEditor.InsertFirstParagraph(PieceTable);
			PieceTable.SpellCheckerManager = documentModel.MainPieceTable.SpellCheckerManager.CreateInstance(PieceTable);
			PieceTable.SpellCheckerManager.Initialize();
		}
		public override bool IsMain { get { return false; } }
		public override bool IsComment { get { return true; } }
		public override bool IsReferenced { get { return referenceComment != null; } }
		public Comment ReferenceComment { get { return referenceComment; } set { referenceComment = value; } }
		protected internal RunIndex CalculateCommentPieceTableStartRunIndex(RunIndex runIndex) {
			return ReferenceComment.Content.PieceTable.Paragraphs.First.FirstRunIndex;
		}
		protected internal RunIndex CalculateCommentPieceTableEndRunIndex(RunIndex runIndex) {
			return ReferenceComment.Content.PieceTable.Paragraphs.Last.LastRunIndex;
		}
		protected internal override SectionIndex LookupSectionIndexByParagraphIndex(ParagraphIndex paragraphIndex) {
			if (ReferenceComment != null) {
				DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(DocumentModel.MainPieceTable, ReferenceComment.Start);
				ParagraphIndex index = position.ParagraphIndex;
				return base.LookupSectionIndexByParagraphIndex(index);
			}
			return SectionIndex.DontCare;
		}
		protected internal override void ApplyChanges(DocumentModelChangeType changeType, RunIndex startRunIndex, RunIndex endRunIndex) {
			if (!IsReferenced)
				base.ApplyChanges(changeType, startRunIndex, endRunIndex);
			else
				base.ApplyChanges(changeType, CalculateCommentPieceTableStartRunIndex(startRunIndex), CalculateCommentPieceTableEndRunIndex(endRunIndex));
		}
		protected internal override void ApplyChangesCore(DocumentModelChangeActions actions, RunIndex startRunIndex, RunIndex endRunIndex) {
			if (!IsReferenced) {
				base.ApplyChangesCore(actions, startRunIndex, endRunIndex);
				return;
			}
			if ((actions & DocumentModelChangeActions.SplitRunByCharset) != 0) { 
				base.ApplyChangesCore(DocumentModelChangeActions.SplitRunByCharset, startRunIndex, endRunIndex);
				actions &= ~DocumentModelChangeActions.SplitRunByCharset;
			}
			if (((actions & DocumentModelChangeActions.RaiseContentChanged) != 0))
				actions |= DocumentModelChangeActions.RaiseCommentContentChange;
			base.ApplyChangesCore(actions, CalculateCommentPieceTableStartRunIndex(startRunIndex), CalculateCommentPieceTableEndRunIndex(endRunIndex));
		}
		protected internal override void FixLastParagraphOfLastSection(int originalParagraphCount) {
		}
	}
	#endregion   
	#region Comment
	public class Comment : BookmarkBase {
		public static readonly DateTime MinCommentDate = new DateTime(1900, 1, 1);
		#region Fields
		string name;
		string author = String.Empty;
		DateTime date;
		Comment parentComment;
		CommentContentType content;
		int index;
		#endregion
		public Comment(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end) 
			: this(pieceTable, start, end, new CommentContentType(pieceTable.DocumentModel)) {
		}
		public Comment(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end, CommentContentType content)
			: base(pieceTable, start, end) {
			Guid.Equals(pieceTable, pieceTable.DocumentModel.MainPieceTable);
			this.content = content;
		}
		#region Properties
		public string Name { get { return name; } set { name = value; } }
		public string Author { get { return author; } set { author = value; } }
		public DateTime Date { get { return date; } set { date = value; } }
		public Comment ParentComment { get { return parentComment; } set { parentComment = value; } }
		public CommentContentType Content { get { return content; } set { content = value; } }
		public int Index { get { return index; } set { index = value; } }
		#endregion
		public override string ToString() {
			return name;
		}
		public override void Visit(IDocumentIntervalVisitor visitor) {
			visitor.Visit(this);
		}
		protected internal override void Delete(int index) {
			PieceTable.DeleteCommentCore(index);
		}
	}
	#endregion
	#region CommentCollection
	public class CommentCollection : BookmarkBaseCollection<Comment> {
		public CommentCollection(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public virtual Comment FindByName(string name) {
			for (int i = 0; i < Count; i++) {
				if (this[i].Name == name)
					return this[i];
			}
			return null;
		}
		protected override void OnDocumentIntervalInserted(int index) {
			this[index].Index = index;
			for (int i = index + 1; i < Count; i++)
				this[i].Index++;
			base.OnDocumentIntervalInserted(index);
		}
		protected override void OnDocumentIntervalRemoved(int index) {
			for (int i = index; i < Count; i++)
				this[i].Index--;
			base.OnDocumentIntervalRemoved(index);
		}
		protected override IComparer<Comment> CreateDocumentIntervalComparer() {
			return new CommentIntervalComparer<Comment>();
		}
	}
	#endregion
	#region CommentsHelper
	internal class CommentsHelper {
		PieceTable mainPieceTable;
		protected internal CommentsHelper(PieceTable mainPieceTable){
			this.mainPieceTable = mainPieceTable;
		} 
		protected internal void DeleteNestedComments(Comment comment) {
			if (comment.ParentComment == null) {
				List<Comment> nestedComments = CreateListNestedComments(comment);
				for (int i = nestedComments.Count - 1; i >= 0; i--)
					mainPieceTable.DeleteComment(nestedComments[i].Index);
			}
		}
		protected List<Comment> CreateListNestedComments(Comment currentComment) {
			List<Comment> result = new List<Comment>();
			CommentCollection comments = mainPieceTable.Comments;
			int count = comments.Count;
			for (int i = 0; i < count; i++) {
				Comment sourceComment = comments[i];
				if (sourceComment.ParentComment == currentComment)
					result.Add(sourceComment);
			}
			return result;
		}
	}
	#endregion
}

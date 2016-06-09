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
using System.Linq;
using System.Text;
using DevExpress.Office.Layout;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.LayoutEngine {
	public class CommentSizeCalculator {
		#region Fields
		List<CommentViewInfo> comments;
		Rectangle pageClientBounds;
		Rectangle pageCommentBounds;
		CommentPadding commentPadding;
		#endregion
		protected internal CommentSizeCalculator() {  
		}
		#region Properties
		protected List<CommentViewInfo> Comments { get { return comments; }  }
		protected Rectangle PageClientBounds { get { return pageClientBounds; }  }
		protected Rectangle CommentBounds { get { return pageCommentBounds; } }
		protected internal CommentPadding CommentPadding { get { return commentPadding; } set { commentPadding = value; } }
		protected internal int DistanceBetweenComments { get { return commentPadding.DistanceBetweenComments; } }
		protected int TopMargin { get { return commentPadding.ContentTop; } }
		protected int RightMargin { get { return commentPadding.ContentRight; } }
		protected int LeftMargin { get { return commentPadding.ContentLeft; } }
		protected int BottomMargin { get { return commentPadding.ContentBottom; } }
		#endregion
		public void CalculateCommentClientBounds(List<CommentViewInfo> comments, Rectangle clientBounds, Rectangle commentBounds, CommentPadding commentPadding) {
			Guard.ArgumentNotNull(comments, "comments");
			Guard.ArgumentNotNull(commentPadding, "commentPadding");
			this.comments = comments;
			this.pageClientBounds = clientBounds;
			this.pageCommentBounds = commentBounds;
			this.commentPadding = commentPadding;
			int count = Comments.Count;
			SortComments();
			CalculateCommentContainTableInFirstRow(); 
			int totalCommentsHeight = CalculateTotalOriginalCommentsHeight() + DistanceBetweenComments * (count - 1);
			if (CommentContainTable() && (totalCommentsHeight <= PageClientBounds.Height)) {
				ResizeCommentsHeigth();
				MoveCommentsTop(commentPadding.DistanceBetweenComments, true);
			}
			if (totalCommentsHeight > PageClientBounds.Height) 
				ResizeCommentBounds();
			else
				MoveCommentsTop(commentPadding.DistanceBetweenComments, true);			
			CalculateCommentMoreButton();
			UpdateCommentContentBounds();
		}
		protected virtual void SortComments() {
			comments.Sort(CompareCommentViewInfoTightBoundsTop);
		}
		protected internal virtual int CompareCommentViewInfoTightBoundsTop(CommentViewInfo commentViewInfo1, CommentViewInfo commentViewInfo2) {
			if ((commentViewInfo1.Character.TightBounds.Top != commentViewInfo2.Character.TightBounds.Top))
				return commentViewInfo1.Character.TightBounds.Top - commentViewInfo2.Character.TightBounds.Top;
			else
				return commentViewInfo1.Comment.Index - commentViewInfo2.Comment.Index;
		}
		protected internal virtual void ResizeCommentBounds() {
			int count = Comments.Count;
			ResizeCommentsHeigth();
			if (count > 1) {
				int commentsHight = CalculateTotalCommentsHeight();
				int newTopOffset = (PageClientBounds.Height - commentsHight) / (count - 1);
				newTopOffset = Math.Max(newTopOffset, DistanceBetweenComments); 
				MoveCommentsTop(newTopOffset, false);
			}
			else
				MoveCommentsTop(DistanceBetweenComments, false);
		}
		protected internal virtual void ResizeCommentsHeigth() {
			int count = Comments.Count;
			int freeHeight = PageClientBounds.Height;
			int avgCommentHeight = CalculateCommentAvgHeight();
			int totalCommentsHeight = CalculateTotalOriginalCommentsHeight();
			for (int i = count - 1; i >= 0; i--) {
				CommentViewInfo commentViewInfo = Comments[i];
				totalCommentsHeight -= commentViewInfo.OriginalHeight;
				DocumentLogPosition lastVisiblePosition = commentViewInfo.LastVisiblePosition;
				int newCommentHeight = CalculateNewCommentHeight(commentViewInfo, avgCommentHeight, freeHeight, i, totalCommentsHeight, out lastVisiblePosition);
				commentViewInfo.LastVisiblePosition = lastVisiblePosition;
				Comments[i].Bounds = new Rectangle(commentViewInfo.Bounds.Location, new Size(commentViewInfo.Bounds.Width, newCommentHeight));
				freeHeight -= (newCommentHeight + DistanceBetweenComments);
			}
		}
		protected virtual int CalculateNewCommentHeight(CommentViewInfo commentViewInfo, int avgCommentHeight, int freeHeight, int visibleIndex, int prevCommentsHeight, out DocumentLogPosition lastVisiblePosition) {
			if (commentViewInfo.OriginalHeight < avgCommentHeight)
				return CalculateCommentHeight(commentViewInfo, commentViewInfo.OriginalHeight, out lastVisiblePosition);
			int distanceFromTop = prevCommentsHeight + DistanceBetweenComments * visibleIndex;
			int count = Comments.Count;
			int pageHeight = PageClientBounds.Height;
			if (distanceFromTop > visibleIndex * pageHeight / count)
				return CalculateCommentHeight(commentViewInfo, avgCommentHeight, out lastVisiblePosition);
			else
				return CalculateCommentHeight(commentViewInfo, freeHeight - distanceFromTop, out lastVisiblePosition);			
		}
		protected internal virtual int CalculateCommentAvgHeight() {
			int height = PageClientBounds.Height;
			int count = Comments.Count;
			int totalDistance = DistanceBetweenComments * (count - 1);
			return Math.Max((int)((height - totalDistance) / count), 1);
		}
		protected internal virtual int CalculateCommentHeight(CommentViewInfo commentViewInfo, int maxCommentHeight, out DocumentLogPosition lastVisiblePosition) {
			RowCollection rows = GetRowCollection(commentViewInfo);
			int rowsCount = rows.Count;
			int verticalMargin = TopMargin + BottomMargin;
			if (rowsCount == 1) {
				if (commentViewInfo.Comment == null) {
					lastVisiblePosition = new DocumentLogPosition(-1);
				}
				else
					lastVisiblePosition = rows[0].GetLastPosition(commentViewInfo.Comment.Content.PieceTable).LogPosition;
				return (rows[0].Bounds.Bottom + verticalMargin);
			}
			if (rows[0] is TableCellRow) {
				if (commentViewInfo.Comment == null) {
					lastVisiblePosition = new DocumentLogPosition(-1);
				}
				else
					lastVisiblePosition = new DocumentLogPosition(-1);
				return commentViewInfo.CommentHeadingBounds.Height + verticalMargin;
			}
			int maxContentHeight = maxCommentHeight - verticalMargin;
			for (int j = 1; j < rowsCount; j++) {
				if ((rows[j].Bounds.Bottom > maxContentHeight) || (rows[j] is TableCellRow)) {
					if (commentViewInfo.Comment == null) {
						lastVisiblePosition = new DocumentLogPosition(-1);
					}
					else
						lastVisiblePosition = rows[j - 1].GetLastPosition(commentViewInfo.Comment.Content.PieceTable).LogPosition;
					return (rows[j - 1].Bounds.Bottom + verticalMargin);
				}
			}
			if (commentViewInfo.Comment == null) {
				lastVisiblePosition = new DocumentLogPosition(-1);
			}
			else
				lastVisiblePosition = commentViewInfo.Comment.Content.PieceTable.DocumentEndLogPosition;
			return (rows[rowsCount - 1].Bounds.Bottom + verticalMargin);
		}
		protected virtual RowCollection GetRowCollection(CommentViewInfo commentViewInfo) {
			return commentViewInfo.CommentDocumentLayout.Pages[0].Areas[0].Columns[0].Rows;
		}
		protected internal virtual void MoveCommentsTop(int newtopOffset, bool alignmentCommentTop) {
			int count = Comments.Count;
			int commentHeight = CalculateTotalCommentsHeight() + DistanceBetweenComments * (count - 1);
			int y;
			if (commentHeight > pageClientBounds.Height)
				y = CommentBounds.Top + DistanceBetweenComments * 2;
			else
				y = PageClientBounds.Top;
			for (int i = 0; i < count; i++) {
				if (alignmentCommentTop) {
					if (commentHeight > CalculateSupposeCommentHeight(Comments[i]))
						y = pageClientBounds.Bottom - commentHeight;
					else
						y = Math.Max(y, GetTightBoundsTop(Comments[i]));
				}
				Comments[i].Bounds = new Rectangle(Comments[i].Bounds.Left, y, Comments[i].Bounds.Width, Comments[i].Bounds.Height);
				y += (Comments[i].Bounds.Height + newtopOffset);
				commentHeight -= Comments[i].Bounds.Height + DistanceBetweenComments;
			}
		}
		int CalculateSupposeCommentHeight(CommentViewInfo commentViewInfo) { 
			return pageClientBounds.Bottom - GetTightBoundsTop(commentViewInfo);
		}
		protected virtual int GetTightBoundsTop(CommentViewInfo commentViewInfo) {
			return commentViewInfo.Character.TightBounds.Top - CommentPadding.ContentTop;
		}
		protected internal virtual int CalculateTotalCommentsHeight() {
			int result = 0;
			int count = Comments.Count;
			for (int i = 0; i < count; i++) {
				result += Comments[i].Bounds.Height;
			}
			return result;
		}
		protected internal virtual int CalculateTotalOriginalCommentsHeight() {
			int result = 0;
			int count = Comments.Count;
			for (int i = 0; i < count; i++) {
				result += Comments[i].OriginalHeight;
			}
			return result;
		}
		protected void CalculateCommentContainTableInFirstRow() {
			int count = Comments.Count;
			for (int i = 0; i < count; i++) {
				Comments[i].CommentContainTableInFirstRow = CommentFirstRowInTable(Comments[i]);
			}
		}
		protected virtual bool CommentFirstRowInTable(CommentViewInfo commentViewInfo) {
			RowCollection rows = GetRowCollection (commentViewInfo);
			return (rows[0] is TableCellRow);
		}
		protected virtual bool CommentContainTable() {
			int count = Comments.Count;
			for (int i = 0; i < count; i++) {
				RowCollection rows = GetRowCollection(Comments[i]);
				int rowsCount = rows.Count;
				for (int j = 0; j < rowsCount; j++) {
					if (rows[j] is TableCellRow)
						return true;
				}
			}
			return false;
		}	   
		protected void CalculateCommentMoreButton() {
			int count = Comments.Count;
			for (int i = 0; i < count; i++) {			  
				Size size = commentPadding.MoreButtonSize;
				int top = comments[i].Bounds.Bottom - size.Height - commentPadding.MoreButtonOffsetY;
				int left = GetMoreButtonPosition(comments[i], size);
				Comments[i].CommentMoreButtonBounds = new Rectangle(new Point(left, top), size);
			}
		}
		int GetMoreButtonPosition(CommentViewInfo comment, Size moreButtonSize) {
			switch(commentPadding.MoreButtonHorizontalAlignment) {
				case MoreButtonHorizontalAlignment.Left:
					return comment.Bounds.X + commentPadding.MoreButtonOffsetX;
				case MoreButtonHorizontalAlignment.Center:
					return comment.Bounds.X + comment.Bounds.Width / 2 + commentPadding.MoreButtonOffsetX;
				default:
					return comment.Bounds.X + comment.Bounds.Width - commentPadding.MoreButtonOffsetX - moreButtonSize.Width;
			}
		}
		protected virtual void UpdateCommentContentBounds(){
			int count = Comments.Count;
			for (int i = 0; i < count; i++) {
				Rectangle contentBounds = Comments[i].Bounds;
				int verticalMargin = TopMargin + BottomMargin;
				int horizontalMargin = RightMargin + LeftMargin;
				contentBounds.Width -= horizontalMargin;
				contentBounds.Height -= verticalMargin;
				contentBounds.X += LeftMargin;
				contentBounds.Y += TopMargin;
				Comments[i].ContentBounds = contentBounds;
				Comments[i].CommentHeadingBounds = new Rectangle(new Point(contentBounds.X + Comments[i].CommentHeadingOffset, contentBounds.Y), Comments[i].CommentHeadingBounds.Size);
			}
		}
	}
}

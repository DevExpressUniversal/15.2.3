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
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	public class CommentViewInfo {
		#region Fields
		DocumentLayout commentDocumentLayout;
		Rectangle bounds;
		Rectangle contentBounds;
		int actualSize;
		Comment comment;
		CharacterBox character;
		string commentHeading;
		Rectangle commentHeadingBounds;
		FontInfo commentHeadingFontInfo;
		Rectangle commentMoreButtonBounds;
		bool commentContainTableInFirstRow;
		int commentHeadingOffset;
		DocumentLogPosition lastVisiblePosition;
		Page commentViewInfoPage;
		bool isActive;
		bool focused;
		#endregion
		#region Property
		public DocumentLayout CommentDocumentLayout { get { return commentDocumentLayout; } set { commentDocumentLayout = value; } }
		public int ActualSize { get { return actualSize; } set { actualSize = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public int OriginalHeight { get; set; }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public Comment Comment { get { return comment; } set { comment = value; } }
		public CharacterBox Character { get { return character; } set { character = value; } }
		public string CommentHeading { get { return commentHeading; } set { commentHeading = value; } }
		public Rectangle CommentHeadingBounds { get { return commentHeadingBounds; } set { commentHeadingBounds = value; } }
		public FontInfo CommentHeadingFontInfo { get { return commentHeadingFontInfo; } set { commentHeadingFontInfo = value; } }
		public Rectangle CommentMoreButtonBounds { get { return commentMoreButtonBounds; } set { commentMoreButtonBounds = value; } }
		public bool CommentContainTableInFirstRow { get { return commentContainTableInFirstRow; } set { commentContainTableInFirstRow = value; } }
		public bool IsWholeContentVisible { get { return ActualSize > Bounds.Height; } }
		public int CommentHeadingOffset { get { return commentHeadingOffset; } set { commentHeadingOffset = value; } }
		public DocumentLogPosition LastVisiblePosition { get { return lastVisiblePosition; } set { lastVisiblePosition = value; } }
		public Page CommentViewInfoPage { get { return commentViewInfoPage; } set { commentViewInfoPage = value; } }
		public bool IsActive { get { return isActive; } set { isActive = value; } }
		public bool Focused { get { return focused; } set { focused = value; } }
		#endregion
	}
	public enum MoreButtonHorizontalAlignment {
		Left,
		Right,
		Center
	}
	public class CommentPadding {
		public static CommentPadding GetDefaultCommentPadding(DocumentModel documentModel) {
			DocumentLayoutUnitConverter converter = documentModel.LayoutUnitConverter;
			return new CommentPadding(converter.DocumentsToLayoutUnits(112),
				converter.DocumentsToLayoutUnits(37),
				converter.DocumentsToLayoutUnits(20),
				converter.DocumentsToLayoutUnits(10),
				converter.DocumentsToLayoutUnits(20),
				converter.DocumentsToLayoutUnits(10),
				converter.DocumentsToLayoutUnits(6),
				new Size(converter.PixelsToLayoutUnits(23, 96), converter.PixelsToLayoutUnits(14, 96)),
				0, 0,
				MoreButtonHorizontalAlignment.Right);
		}
		public CommentPadding(int commentLeft, int commentRight, int contentLeft, int contentTop, int contentRight, int contentBottom, int distanceBetweenComments, Size moreButtonSize, int moreButtonOffsetX, int moreButtonOffsetY, MoreButtonHorizontalAlignment moreButtonHorizontalAlignment) {
			this.CommentLeft = commentLeft;
			this.CommentRight = commentRight;
			this.ContentLeft = contentLeft;
			this.ContentTop = contentTop;
			this.ContentRight = contentRight;
			this.ContentBottom = contentBottom;
			this.DistanceBetweenComments = distanceBetweenComments;
			this.MoreButtonOffsetX = moreButtonOffsetX;
			this.MoreButtonOffsetY = moreButtonOffsetY;
			this.MoreButtonHorizontalAlignment = moreButtonHorizontalAlignment;
			this.MoreButtonSize = moreButtonSize;
		}
		public int CommentLeft { get; private set; }
		public int CommentRight { get; private set; }
		public int ContentLeft { get; private set; }
		public int ContentRight { get; private set; }
		public int ContentTop { get; private set; }
		public int ContentBottom { get; private set; }
		public int DistanceBetweenComments { get; private set; }
		public int MoreButtonOffsetX { get; private set; }
		public int MoreButtonOffsetY { get; private set; }
		public MoreButtonHorizontalAlignment MoreButtonHorizontalAlignment { get; private set; }
		public Size MoreButtonSize { get; private set; }
	}
	public class CommentViewInfoHelper {
		protected internal CommentViewInfo FindCommentViewInfo(Page page, PieceTable pieceTable) {
			if ((page == null) || (page.Comments == null))
				return null;
			List<CommentViewInfo> comments = page.Comments;
			ResetIsActive(comments);
			int count = comments.Count;
			for (int i = 0; i < count; i++) {
				if (comments[i].Comment.Content.PieceTable == pieceTable) {
					comments[i].IsActive = true;
					return comments[i];
				}
			}
			return null;
		}
		internal void ResetIsActive(List<CommentViewInfo> comments) {
			for (int i = 0; i < comments.Count; i++)
				comments[i].IsActive = false;
		}
		internal void ResetFocused(List<CommentViewInfo> comments) {
			for (int i = 0; i < comments.Count; i++)
				comments[i].Focused = false;
		}
		internal void SetIsActive(List<CommentViewInfo> comments, CommentViewInfo commentViewInfo) {
			if (commentViewInfo.Comment.ParentComment == null)
				SetPropertyInNestedComment(comments, commentViewInfo.Comment, true );
			else {
				SetPropertyInParentComment(comments, commentViewInfo.Comment.ParentComment, true);
				SetPropertyInNestedComment(comments, commentViewInfo.Comment.ParentComment, true);
			}
		}
		internal void SetFocused(List<CommentViewInfo> comments, CommentViewInfo commentViewInfo) {
			if (commentViewInfo.Comment.ParentComment == null)
				SetPropertyInNestedComment(comments, commentViewInfo.Comment, false);
			else {
				SetPropertyInParentComment(comments, commentViewInfo.Comment.ParentComment, false);
				SetPropertyInNestedComment(comments, commentViewInfo.Comment.ParentComment, false);
			}
		}
		void SetPropertyInParentComment(List<CommentViewInfo> comments, Comment comment, bool isActive) {
			int index = 0;
			while (comments[index].Comment != comment)
				index++;
			if (isActive)
				comments[index].IsActive = true;
			else
				comments[index].Focused = true;
		}
		void SetPropertyInNestedComment(List<CommentViewInfo> comments, Comment comment, bool isActive){
			for (int i = 0; i < comments.Count; i++) {
				if (comments[i].Comment.ParentComment == comment)
					if (isActive)
						comments[i].IsActive = true;
					else
						comments[i].Focused = true;
			}
		 }
	} 
}

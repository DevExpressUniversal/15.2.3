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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public enum CommentType : byte
  {
	SingleLine,
	MultiLine
  }
  public class Comment : SupportElement, IFormattingElement
	{
		private const int INT_MaintainanceComplexity = 3;
		#region private fields...
		private int _TextStartOffset;
	private CommentType _CommentType;
	bool _IsUnfinished;
		int _StartPos;
		int _EndPos;
	#endregion
		#region Comment
		public Comment()
		{
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
	  Comment comment = source as Comment;
	  if (comment == null)
				return;
	  _TextStartOffset = comment._TextStartOffset;
	  _CommentType = comment._CommentType;
	  _IsUnfinished = comment._IsUnfinished;
		}
		#endregion
	public void GetFirstAndLastConnectedComments(out Comment first, out Comment last)
	{
	  first = this;
	  last = this;
	  Comment previousConnectedComment = this.PreviousConnectedComment;
	  while (previousConnectedComment != null)
	  {
		first = previousConnectedComment;
		previousConnectedComment = previousConnectedComment.PreviousConnectedComment;
	  }
	  Comment nextConnectedComment = this.NextConnectedComment;
	  while (nextConnectedComment != null)
	  {
		last = nextConnectedComment;
		nextConnectedComment = nextConnectedComment.NextConnectedComment;
	  }
	}
	#region SetCommentType
	[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetCommentType(CommentType aCommentType)
	{
		_CommentType = aCommentType;
	}
	#endregion
	#region SetTextStartOffset
	[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetTextStartOffset(int aTextStartOffset)
	{
		_TextStartOffset = aTextStartOffset;
	}
	#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Comment;
		}
		#endregion
		#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  Comment clone = options.GetClonedElement(this) as Comment;
	  if (clone == null)
	  {
		clone = new Comment();
		clone.CloneDataFrom(this, options);
		if (options != null)
		  options.AddClonedElement(this, clone);
	  }
	  if (options != null && options.SourceFile != null)
		options.SourceFile.AddComment(clone, true);
	  return clone;
	}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		public int StartPos
		{
			get
			{
				return _StartPos;
			}
			set
			{
				_StartPos = value;
			}
		}
		public int EndPos
		{
			get
			{
				return _EndPos;
			}
			set
			{
				_EndPos = value;
			}
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IsUnfinished
		{
			get
			{
		return _IsUnfinished;
			}
			set
			{
		_IsUnfinished = value;
			}
		}
		#region TextStartOffset
	public int TextStartOffset
	{
	  get
	  {
		return _TextStartOffset;
	  }
	}
		#endregion
	#region CommentType
		public CommentType CommentType
	{
	  get
	  {
		return _CommentType;
	  }
			set
			{
				_CommentType = value;
			}
	}
	#endregion
		#region NextConnectedComment
		public Comment NextConnectedComment
		{
			get
			{
				LanguageElement lNextSibling = NextSibling;
				if (lNextSibling == null)
					return null;
				if (lNextSibling.ElementType == this.ElementType)
				{
					Comment lComment = (Comment)lNextSibling;
					if (lComment.InternalRange.Start.Line == InternalRange.End.Line + 1)
						return lComment;
				}
				return null;
			}
		}
		#endregion
		#region PreviousConnectedComment
		public Comment PreviousConnectedComment
		{
			get
			{
				LanguageElement lPreviousSibling = PreviousSibling;
				if (lPreviousSibling == null)
					return null;
				if (lPreviousSibling.ElementType == this.ElementType)
				{
					Comment lComment = (Comment)lPreviousSibling;
					if (lComment.InternalRange.End.Line == InternalRange.Start.Line - 1)
						return lComment;
				}
				return null;
			}
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Comment;
			}
		}
	}
}

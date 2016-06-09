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
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.API.Native.Implementation;
namespace DevExpress.XtraRichEdit.Model.History {
	public abstract class ChangeCommentPropertyHistoryItem : RichEditHistoryItem {
		#region Fields
		readonly Comment comment;
		readonly string newValue;
		readonly string oldValue;
		#endregion
		protected ChangeCommentPropertyHistoryItem(PieceTable pieceTable, Comment comment, string newValue, string oldValue)
			: base(pieceTable) {
			this.comment = comment;
			this.newValue = newValue;
			this.oldValue = oldValue;
		}
		public Comment Comment { get { return comment; } }
		public string NewValue { get { return newValue; } }
		public string OldValue { get { return oldValue; } }
		protected override void UndoCore() {
			PieceTableApplyChanges();
		}
		protected override void RedoCore() {
			PieceTableApplyChanges();
		}
		void PieceTableApplyChanges() {
			PieceTable.ApplyChanges(DocumentModelChangeType.CommentPropertiesChanged, RunIndex.Zero, RunIndex.MaxValue);
		}
	}
	public class ChangeCommentNameHistoryItem : ChangeCommentPropertyHistoryItem {
		public ChangeCommentNameHistoryItem(PieceTable pieceTable, Comment comment, string newValue, string oldValue)
			: base(pieceTable, comment, newValue, oldValue) { }
		protected override void UndoCore() {
			Comment.Name = OldValue;
			base.UndoCore();
		}
		protected override void RedoCore() {
			Comment.Name = NewValue;
			base.RedoCore();
		} 
	}
	public class ChangeCommentAuthorHistoryItem : ChangeCommentPropertyHistoryItem {
		public ChangeCommentAuthorHistoryItem(PieceTable pieceTable, Comment comment, string newValue, string oldValue)
			: base(pieceTable, comment, newValue, oldValue) { }
		protected override void UndoCore() {
			Comment.Author = OldValue;
			base.UndoCore();
		}
		protected override void RedoCore() {
			Comment.Author = NewValue;
			base.RedoCore();
		} 
	}
	public class ChangeCommentDateHistoryItem : ChangeCommentPropertyHistoryItem {
		public ChangeCommentDateHistoryItem(PieceTable pieceTable, Comment comment, string newValue, string oldValue)
			: base(pieceTable, comment, newValue, oldValue) { }
		protected override void UndoCore() {
			Comment.Date = Convert.ToDateTime(OldValue);
			base.UndoCore();
		}
		protected override void RedoCore() {
			Comment.Date = Convert.ToDateTime(NewValue);
			base.RedoCore();
		}
	}
	public class ChangeCommentParentCommentHistoryItem : RichEditHistoryItem {
		#region Fields
		readonly Comment comment;
		readonly Comment newValue;
		readonly Comment oldValue;
		#endregion
		public ChangeCommentParentCommentHistoryItem(PieceTable pieceTable, Comment comment, Comment newValue, Comment oldValue)
			: base(pieceTable) {
			this.comment = comment;
			this.newValue = newValue;
			this.oldValue = oldValue;
		}
		public Comment Comment { get { return comment; } }
		public Comment NewValue { get { return newValue; } }
		public Comment OldValue { get { return oldValue; } }
		protected override void UndoCore() {
			Comment.ParentComment = OldValue;
			PieceTable.ApplyChanges(DocumentModelChangeType.CommentPropertiesChanged, RunIndex.Zero, RunIndex.MaxValue);
		}
		protected override void RedoCore() {
			Comment.ParentComment = NewValue;
			PieceTable.ApplyChanges(DocumentModelChangeType.CommentPropertiesChanged, RunIndex.Zero, RunIndex.MaxValue);
		}
	}
}

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
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region CommentChangeAuthorIdHistoryItem
	public class CommentChangeAuthorIdHistoryItem : SpreadsheetIntHistoryItem {
		#region Fields
		readonly Comment comment;
		#endregion
		public CommentChangeAuthorIdHistoryItem(Comment comment, int oldAuthorId, int newAuthorId) : base(comment.Workbook.ActiveSheet, oldAuthorId, newAuthorId){
			this.comment = comment;
		}
		protected override void UndoCore() {
			comment.SetAuthorIdCore(OldValue);
		}
		protected override void RedoCore() {
			comment.SetAuthorIdCore(NewValue);
		}
	}
	#endregion
	#region CommentChangeReferenceHistoryItem
	public class CommentChangeReferenceHistoryItem : SpreadsheetCellPositionHistoryItem {
		#region Fields
		readonly Comment comment;
		#endregion
		public CommentChangeReferenceHistoryItem(Comment comment, CellPosition oldReference, CellPosition newReference)
			: base(comment.Workbook.ActiveSheet, oldReference, newReference) {
			this.comment = comment;
		}
		protected override void UndoCore() {
			comment.SetReferenceCore(OldValue);
		}
		protected override void RedoCore() {
			comment.SetReferenceCore(NewValue);
		}
	}
	#endregion
	#region CommentChangeRunTextHistoryItem
	public class CommentChangeRunTextHistoryItem : SpreadsheetStringHistoryItem {
		#region Fields
		readonly CommentRun commentRun;
		#endregion
		public CommentChangeRunTextHistoryItem(CommentRun commentRun, string oldText, string newText)
			: base(commentRun.DocumentModel.ActiveSheet, oldText, newText) {
			this.commentRun = commentRun;
		}
		protected override void UndoCore() {
			commentRun.SetTextCore(OldValue);
		}
		protected override void RedoCore() {
			commentRun.SetTextCore(NewValue);
		}
	}
	#endregion
	#region CommentRunAddHistoryItem
	public class CommentRunAddHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly CommentRun commentRun;
		readonly CommentRunCollection runCollection;
		#endregion
		public CommentRunAddHistoryItem(CommentRunCollection runCollection, CommentRun commentRun)
			: base(runCollection.Worksheet) {
				this.runCollection = runCollection;
				this.commentRun = commentRun;
		}
		protected override void UndoCore() {
			runCollection.RemoveAtCore(runCollection.Count - 1);
		}
		protected override void RedoCore() {
			runCollection.AddCore(commentRun);
		}
	}
	#endregion
	#region CommentRunClearHistoryItem
	public class CommentRunClearHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly List<CommentRun> runs;
		readonly CommentRunCollection runCollection;
		#endregion
		public CommentRunClearHistoryItem(CommentRunCollection runCollection)
			: base(runCollection.Worksheet) {
			this.runCollection = runCollection;
			this.runs = new List<CommentRun>();
			this.runs.AddRange(runCollection.InnerList);
		}
		protected override void UndoCore() {
			runCollection.AddCore(runs);
		}
		protected override void RedoCore() {
			runCollection.ClearCore();
		}
	}
	#endregion
	#region CommentRunRemoveAtHistoryItem
	public class CommentRunRemoveAtHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		CommentRun commentRun;
		readonly CommentRunCollection runCollection;
		readonly int index;
		#endregion
		public CommentRunRemoveAtHistoryItem(CommentRunCollection runCollection, int index)
			: base(runCollection.Worksheet) {
				this.runCollection = runCollection;
				this.index = index;
		}
		protected override void UndoCore() {
			runCollection.InsertCore(index, commentRun);
		}
		protected override void RedoCore() {
			commentRun = runCollection[index];
			runCollection.RemoveAtCore(index);
		}
	}
	#endregion
	#region CommentRunInsertHistoryItem
	public class CommentRunInsertHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		CommentRun commentRun;
		readonly CommentRunCollection runCollection;
		readonly int index;
		#endregion
		public CommentRunInsertHistoryItem(CommentRunCollection runCollection, CommentRun commentRun, int index)
			: base(runCollection.Worksheet) {
			this.runCollection = runCollection;
			this.commentRun = commentRun;
			this.index = index;
		}
		protected override void UndoCore() {
			runCollection.RemoveAtCore(index);
		}
		protected override void RedoCore() {
			runCollection.InsertCore(index, commentRun);
		}
	}
	#endregion
}

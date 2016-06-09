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
namespace DevExpress.XtraSpreadsheet.Model {
	partial class Worksheet {
		public bool IsNotWorksheet { get; set; } 
		#region AddComment
		public void AddComment(Comment comment) {
			AddWorksheetCommentCommand command = new AddWorksheetCommentCommand(this, comment);
			command.Execute();
		}
		#endregion
		#region CreateComment
		public Comment CreateComment(CellPosition position, string authorName) {
			InsertWorksheetCommentCommand command = new InsertWorksheetCommentCommand(this, position, authorName);
			command.Execute();
			return (command.Result as Comment);
		}
		#endregion
		#region ClearComments
		public void ClearComments() {
			DeleteAllWorksheetCommentCommand command = new DeleteAllWorksheetCommentCommand(this, Comments);
			command.Execute();
		}
		#endregion
		#region RemoveCommentAt
		public void RemoveCommentAt(int index) {
			DeleteWorksheetCommentCommand command = new DeleteWorksheetCommentCommand(this, index);
			command.Execute();
		}
		#endregion
		#region RemoveComment
		public void RemoveComment(Comment comment) {
			int index = comments.IndexOf(comment);
			RemoveCommentAt(index);
		}
		#endregion
		#region RemoveComment
		public void RemoveComment(CellPosition position) {
			Comment comment = GetComment(position);
			if (comment != null)
				RemoveComment(comment);
		}
		#endregion
		#region GetComment
		public Comment GetComment(CellPosition position) {
			for (int i = 0; i < comments.Count; i++) {
				Comment comment = comments[i];
				if (comment.Reference.Equals(position))
					return comment;
			}
			return null;
		}
		#endregion
		public Comment GetComment(CellRange range) {
			for (int i = 0; i < comments.Count; i++) {
				Comment comment = comments[i];
				CellPosition reference = comment.Reference;
				if (range.ContainsCell(reference.Column, reference.Row))
					return comment;
			}
			return null;
		}
		protected internal List<Comment> GetComments(CellRange range) {
			List<Comment> result = new List<Comment>();
			for (int i = 0; i < comments.Count; i++) {
				Comment comment = comments[i];
				CellPosition reference = comment.Reference;
				if (range.ContainsCell(reference.Column, reference.Row))
					result.Add(comment);
			}
			return result;
		}
		public void MoveComment(int commentIndex, int offsetX, int offsetY) {
			MoveCommentCommand command = new MoveCommentCommand(this, commentIndex, offsetX, offsetY);
			command.Execute();
		}
		public void ResizeComment(int commentIndex, int width, int height) {
			ResizeCommentCommand command = new ResizeCommentCommand(this, commentIndex, width, height);
			command.Execute();
		}
	}
}

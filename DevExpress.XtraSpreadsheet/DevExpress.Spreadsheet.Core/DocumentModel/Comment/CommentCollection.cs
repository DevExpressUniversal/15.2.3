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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CommentCollection
	public class CommentCollection : UndoableCollection<Comment> {
		public CommentCollection(Worksheet sheet)
			: base(sheet) {
		}
		#region OnRangeInserting
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			foreach (Comment currentComment in this)
				currentComment.OnRangeInserting(notificationContext);
		}
		#endregion
		#region OnRemoving
		public void OnRemoving(RemoveRangeNotificationContext notificationContext) {
			RemoveCellMode mode = notificationContext.Mode;
			if (mode == RemoveCellMode.Default || mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange)
				return;
			for (int i = Count - 1; i >= 0; i--) {
				Comment currentComment = this[i];
				if (notificationContext.Range.ContainsCell(currentComment.Reference.Column, currentComment.Reference.Row))
					currentComment.Worksheet.RemoveCommentAt(i);
				else
					currentComment.OnRangeRemoving(notificationContext);
			}
		}
		#endregion
		#region CheckExistingComments
		public void CheckExistingComments(CellPosition position) {
			for (int i = 0; i < this.Count; i++) {
				Comment comment = this[i];
				if (comment.Reference.Equals(position))
					throw new InvalidOperationException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorCommentAlreadyExists));
			}
		}
		#endregion
		#region TryCheckExistingComments
		public bool TryCheckExistingComments(CellPosition position) {
			for (int i = 0; i < this.Count; i++) {
				Comment comment = this[i];
				if (comment.Reference.Equals(position))
					return true;
			}
			return false;
		}
		#endregion
	}
	#endregion
}

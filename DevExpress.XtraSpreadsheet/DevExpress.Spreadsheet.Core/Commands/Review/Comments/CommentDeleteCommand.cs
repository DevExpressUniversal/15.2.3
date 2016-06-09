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

using DevExpress.Office.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region CommentDeleteCommand
	public class CommentDeleteCommand : SpreadsheetSelectedRangesCommand {
		public CommentDeleteCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ReviewDeleteComment; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_DeleteComment; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_DeleteCommentDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "DeleteComment"; } }
		#endregion
		protected internal override void Modify(CellRange range) {
			ActiveSheet.ClearComments(range);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Comment.Delete, GetEnabled());
			ApplyActiveSheetProtection(state, !Protection.ObjectsLocked);
		}
		bool GetEnabled() {
			if (ActiveSheet.Selection.IsDrawingSelected || InnerControl.IsAnyInplaceEditorActive)
				return false;
			IList<CellRange> ranges = ActiveSheet.Selection.SelectedRanges;
			foreach (CellRange range in ranges) {
				if (IsCommentSelected(range))
					return true;
			}
			return false;
		}
		bool IsCommentSelected(CellRange range) {
			foreach (Comment comment in ActiveSheet.Comments) {
				CellPosition reference = comment.Reference;
				if (range.ContainsCell(reference.Column, reference.Row))
					return true;
			}
			return false;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region CommentDeleteContextMenuItemCommand
	public class CommentDeleteContextMenuItemCommand : CommentDeleteCommand {
		public CommentDeleteContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ReviewDeleteCommentContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ReviewDeleteCommentContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ReviewDeleteCommentContextMenuItemDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return false; } }
		protected override bool UseOfficeImage { get { return true; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			CellRangeBase selection = ActiveSheet.Selection.AsRange();
			state.Enabled &= !selection.IsColumnRangeInterval() && !selection.IsRowRangeInterval();
			state.Visible = state.Enabled;
		}
	}
	#endregion
}

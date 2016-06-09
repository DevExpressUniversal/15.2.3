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
namespace DevExpress.XtraSpreadsheet.Commands {
	public class CommentInsertCommand : SpreadsheetMenuItemSimpleCommand {
		public CommentInsertCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ReviewInsertComment; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_InsertComment; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_InsertCommentDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "InsertComment"; } }
		#endregion
		protected internal override void ExecuteCore() {
			Comment comment;
			DocumentModel.BeginUpdateFromUI();
			try {
				string author = DocumentModel.CurrentAuthor;
				comment = ActiveSheet.CreateComment(ActiveSheet.Selection.ActiveCell, author);
				string text = String.Format("{0}:{1}", author, Environment.NewLine);
				comment.SetPlainText(text);
				comment.Visible = true;
				SelectComment(comment);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
			InnerControl.ActivateCommentInplaceEditor(comment.Reference);
		}
		void SelectComment(Comment comment) {
			CommentEditCommand.SelectComment(comment, ActiveSheet);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Comment.Insert, GetEnabled());
			ApplyActiveSheetProtection(state, !Protection.ObjectsLocked);
			state.Visible = state.Enabled;
		}
		bool GetEnabled() {
			return !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive && !CommentShowHideCommand.IsCommentSelected(ActiveSheet);
		}
	}
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region CommentInsertContextMenuItemCommand
	public class CommentInsertContextMenuItemCommand : CommentInsertCommand {
		public CommentInsertContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ReviewInsertCommentContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ReviewInsertCommentContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ReviewInsertCommentContextMenuItemDescription; } }
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

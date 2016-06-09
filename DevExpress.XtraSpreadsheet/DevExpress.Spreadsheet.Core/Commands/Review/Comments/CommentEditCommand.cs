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
using DevExpress.XtraSpreadsheet.Model;
using System;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region CommentEditCommand
	public class CommentEditCommand : SpreadsheetMenuItemSimpleCommand {
		public CommentEditCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ReviewEditComment; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_EditComment; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_EditCommentDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "EditComment"; } }
		#endregion
		protected internal override void ExecuteCore() {
			Comment comment = CommentShowHideCommand.GetSelectedComment(ActiveSheet);
			if (comment == null)
				return;
			bool isCommentHidden = !comment.Visible;
			DocumentModel.BeginUpdateFromUI();
			try {
				comment.Visible = true;
				SelectComment(comment, ActiveSheet);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
			InnerControl.ActivateCommentInplaceEditor(comment.Reference, isCommentHidden);
		}
		internal static void SelectComment(Comment comment, Worksheet sheet) {
			int index = sheet.Comments.IndexOf(comment);
			sheet.Selection.SelectComment(index);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Comment.Edit, GetEnabled());
			ApplyActiveSheetProtection(state, !Protection.ObjectsLocked);
			state.Visible = GetVisible();
		}
		bool GetEnabled() {
			return !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive && CommentShowHideCommand.IsCommentSelected(ActiveSheet);
		}
		bool GetVisible() {
			CommentInsertCommand command = new CommentInsertCommand(Control);
			ICommandUIState insertState = command.CreateDefaultCommandUIState();
			command.UpdateUIState(insertState);
			return !insertState.Enabled;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region CommentEditContextMenuItemCommand
	public class CommentEditContextMenuItemCommand : CommentEditCommand {
		public CommentEditContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ReviewEditCommentContextMenuItem; } }
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

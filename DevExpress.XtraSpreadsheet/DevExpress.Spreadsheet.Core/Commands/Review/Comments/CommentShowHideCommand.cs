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

using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region CommentShowHideCommand
	public class CommentShowHideCommand : SpreadsheetMenuItemSimpleCommand {
		public CommentShowHideCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ReviewShowHideComment; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ReviewShowHideComment; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ReviewShowHideCommentDescription; } }
		protected override bool UseOfficeImage { get { return true; } }
		public override string ImageName { get { return "ShowHideComment"; } }
		#endregion
		protected internal override void ExecuteCore() {
			Comment comment = GetSelectedComment(ActiveSheet);
			if (comment == null)
				return;
			DocumentModel.BeginUpdateFromUI();
			try {
				comment.Visible = !comment.Visible;
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandsRestriction(state, Options.InnerBehavior.Comment.ShowHide, GetEnabled());
		}
		bool GetEnabled() {
			return !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive && IsCommentSelected(ActiveSheet);
		}
		internal static bool IsCommentSelected(Worksheet sheet) {
			return GetSelectedComment(sheet) != null;
		}
		internal static Comment GetSelectedComment(Worksheet sheet) {
			SheetViewSelection selection = sheet.Selection;
			return sheet.GetComment(selection.GetActualCellRange(selection.ActiveCell));
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region CommentShowHideContextMenuItemCommand
	public class CommentShowHideContextMenuItemCommand : CommentShowHideCommand {
		public CommentShowHideContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ReviewShowHideCommentContextMenuItem; } }
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

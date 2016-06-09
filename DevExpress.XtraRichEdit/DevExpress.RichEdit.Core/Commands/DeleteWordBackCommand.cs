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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region DeleteWordBackCoreCommand
	public class DeleteWordBackCoreCommand : DeleteCommandBase {
		public DeleteWordBackCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteWordBackCore; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteWordBackCoreDescription; } }
		#endregion
		protected internal override void ModifyModel() {
			DocumentModelPosition selectionStart = DocumentModel.Selection.Interval.NormalizedStart;
			WordsDocumentModelIterator iterator = new WordsDocumentModelIterator(ActivePieceTable);
			DocumentModelPosition newSelectionStart = iterator.MoveBack(selectionStart);
			int selectionLength = selectionStart.LogPosition - newSelectionStart.LogPosition;
			if (selectionLength > 0) {
				if (IsContentEditable && ActivePieceTable.CanEditRange(newSelectionStart.LogPosition, selectionStart.LogPosition))
					DeleteContentCore(newSelectionStart.LogPosition, selectionLength, false);
			}
		}
	}
	#endregion
	#region DeleteWordBackCommand
	public class DeleteWordBackCommand : MultiCommand {
		public DeleteWordBackCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteWordBackCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteWordBack; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteWordBackCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteWordBack; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteWordBackCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteWordBackDescription; } }
		protected internal override void CreateCommands() {
			Commands.Add(new SelectFieldPrevToCaretCommand(Control));
			Commands.Add(new DeleteWordBackCoreCommand(Control));
		}
	}
	#endregion
}

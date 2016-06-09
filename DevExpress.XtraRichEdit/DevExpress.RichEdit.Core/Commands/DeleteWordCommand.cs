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
using DevExpress.XtraRichEdit.Commands.Internal;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region DeleteWordCoreCommand
	public class DeleteWordCoreCommand : DeleteCommandBase {
		public DeleteWordCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteWordCore; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteWordCoreDescription; } }
		#endregion
		protected internal override void ModifyModel() {
			DocumentModelPosition selectionStart = DocumentModel.Selection.Interval.NormalizedStart;
			DocumentModelPosition selectionEnd = CalculateSelectionEnd(selectionStart);
			int selectionLength = selectionEnd.LogPosition - selectionStart.LogPosition;
			if (selectionLength > 0) {
				if (IsContentEditable && ActivePieceTable.CanEditRange(selectionStart.LogPosition, selectionEnd.LogPosition))
					DeleteContentCore(selectionStart.LogPosition, selectionLength, false);
			}
		}
		protected internal virtual DocumentModelPosition CalculateSelectionEnd(DocumentModelPosition selectionStart) {
			WordsDocumentModelIterator iterator = new WordsDocumentModelIterator(ActivePieceTable);
			DocumentModelPosition selectionEnd = TryCalculateSelectionEndAssumingSelectionStartAtTheEndOfWord(iterator, selectionStart);
			if (selectionEnd == null)
				return iterator.MoveForward(selectionStart);
			else
				return selectionEnd;
		}
		protected internal virtual DocumentModelPosition TryCalculateSelectionEndAssumingSelectionStartAtTheEndOfWord(WordsDocumentModelIterator iterator, DocumentModelPosition selectionStart) {
			if (iterator.IsStartOfDocument(selectionStart) || iterator.IsEndOfDocument(selectionStart))
				return null;
			if (iterator.IsInsideWord(selectionStart)) {
				CharactersDocumentModelIterator characterIterator1 = new CharactersDocumentModelIterator(ActivePieceTable);
				DocumentModelPosition result1 = selectionStart.Clone();
				iterator.SkipForward(characterIterator1, result1, iterator.IsNotNonWordsSymbols);
				return result1;
			}
			if (!iterator.IsSpace(iterator.GetCharacter(selectionStart)))
				return null;
			CharactersDocumentModelIterator characterIterator = new CharactersDocumentModelIterator(ActivePieceTable);
			DocumentModelPosition previousChar = characterIterator.MoveBack(selectionStart);
			if (!iterator.IsNotNonWordsSymbols(iterator.GetCharacter(previousChar)))
				return null;
			DocumentModelPosition result = selectionStart.Clone();
			iterator.SkipForward(characterIterator, result, iterator.IsSpace);
			iterator.SkipForward(characterIterator, result, iterator.IsNotNonWordsSymbols);
			return result;
		}
	}
	#endregion
	#region DeleteWordCommand
	public class DeleteWordCommand : MultiCommand {
		public DeleteWordCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteWordCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteWord; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteWordCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteWord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteWordCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteWordDescription; } }
		protected internal override void CreateCommands() {
			Commands.Add(new SelectFieldNextToCaretCommand(Control));
			Commands.Add(new DeleteWordCoreCommand(Control));
		}
	}
	#endregion
}

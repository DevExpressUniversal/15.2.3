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
using System.Text;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using System.ComponentModel;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	#region BackSpaceKeyCommand
	public class BackSpaceKeyCommand : RichEditCaretBasedCommand {
		public BackSpaceKeyCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("BackSpaceKeyCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.BackSpaceKey; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("BackSpaceKeyCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_BackSpaceKey; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("BackSpaceKeyCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_BackSpaceKeyDescription; } }
		#endregion
		protected internal override void ExecuteCore() {
			DecrementIndentCommand decrementIndentCommand = new DecrementIndentCommand(Control);
			if (decrementIndentCommand.SelectionBeginFirstRowStartPos() && CanDecrementParagraphIndent()) {
				DecrementParagraphIndentCommand command = CreateDecrementParagraphIndentCommand();
				command.ForceExecute(CreateDefaultCommandUIState());
				return;
			}
			if (IsForbidDeleting()) 
				return;
			DeleteBackCommand deleteBackCommand = CreateDeleteBackCommand();
			deleteBackCommand.ForceExecute(CreateDefaultCommandUIState());
		}
		protected internal virtual bool IsForbidDeleting() {
			Selection selection = DocumentModel.Selection;
			if (selection.Length > 0)
				return false;
			TableCell cell = ActivePieceTable.FindParagraph(selection.Start).GetCell();
			if (cell != null && selection.Start == ActivePieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition)
				return true;
			DocumentLogPosition previousPosition = Algorithms.Max(DocumentLogPosition.Zero, selection.Start - 1);
			TableCell previousCell = ActivePieceTable.FindParagraph(previousPosition).GetCell();
			if (cell != previousCell)
				return true;
			return false;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
		}
		protected internal virtual bool IsSelectionStartFromBeginRow() {
			SelectionLayout selectionLayout = ActiveView.SelectionLayout;
			return selectionLayout.IsSelectionStartFromBeginRow();
		}
		protected internal virtual DeleteBackCommand CreateDeleteBackCommand() {
			return new DeleteBackCommand(Control);
		}
		protected internal virtual DecrementParagraphIndentCommand CreateDecrementParagraphIndentCommand() {
			return new DecrementParagraphIndentCommand(Control);
		}
		protected internal bool CanDecrementParagraphIndent() {
			Selection selection = DocumentModel.Selection;
			ParagraphIndex paragraphIndex = selection.Interval.NormalizedStart.ParagraphIndex;
			Paragraph paragraph = ActivePieceTable.Paragraphs[paragraphIndex];
			if (!paragraph.IsInList() && (paragraph.LeftIndent != 0 || paragraph.FirstLineIndent != 0))
				return true;
			return false;
		}
	}
	#endregion
}

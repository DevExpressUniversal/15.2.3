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
using System.ComponentModel;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertParagraphCommand
	public class InsertParagraphCommand : TransactedInsertObjectCommand {
		public InsertParagraphCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertParagraphCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertParagraph; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertParagraphCoreCommand(Control);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertParagraphCoreCommand
	public class InsertParagraphCoreCommand : InsertObjectCommandBase {
		InputPosition oldInputPosition;
		public InsertParagraphCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertParagraph; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertParagraphDescription; } }
		protected internal virtual bool AllowAutoCorrect { get { return CommandSourceType != DevExpress.Utils.Commands.CommandSourceType.Unknown; } }
		#endregion
		protected internal override void BeforeUpdate() {
			base.BeforeUpdate();
			oldInputPosition = CaretPosition.GetInputPosition().Clone();
		}
		protected internal override void ModifyModel() {
			if (AllowAutoCorrect)
				ActivePieceTable.ApplyChangesCore(DocumentModelChangeActions.ApplyAutoCorrect, RunIndex.DontCare, RunIndex.DontCare);
			DocumentLogPosition pos = DocumentModel.Selection.End;
			IVisibleTextFilter filter = DocumentModel.ActivePieceTable.VisibleTextFilter;
			if (pos != new DocumentLogPosition(1) && !filter.IsRunVisible(new RunIndex(0))) {
				DocumentLogPosition prevPos = filter.GetPrevVisibleLogPosition(pos, false);
			if (prevPos == new DocumentLogPosition(0))
				pos = prevPos;
			}
			InputPosition inputPosition = (CommandSourceType == DevExpress.Utils.Commands.CommandSourceType.Keyboard) ? CaretPosition.GetInputPosition() : CaretPosition.TryGetInputPosition();
			Paragraph paragraph;
			if (inputPosition != null && DocumentModel.Selection.Length == 0) {
				paragraph = ActivePieceTable.InsertParagraph(inputPosition, pos, GetForceVisible());
			}
			else
				paragraph = ActivePieceTable.InsertParagraph(pos, GetForceVisible());
			Paragraph newParagraph = ActivePieceTable.Paragraphs[paragraph.Index + 1];
			if (newParagraph.Length <= 1) {
				ParagraphStyle paragraphStyle = paragraph.ParagraphStyle;
				ParagraphStyle nextParagraphStyle = paragraphStyle.NextParagraphStyle;
				if (nextParagraphStyle != null && nextParagraphStyle != paragraphStyle) {
					newParagraph.ParagraphProperties.ResetAllUse();
					newParagraph.ParagraphStyleIndex = DocumentModel.ParagraphStyles.IndexOf(nextParagraphStyle);
				}
			}
			TableCell cell = CalculateTableCellToAdjustIndex(paragraph, pos);
			if (cell != null) {
				DocumentModel.Selection.ActiveSelection.Start--; 
				DocumentModel.Selection.ActiveSelection.End--;
				paragraph.PieceTable.ChangeCellStartParagraphIndex(cell, paragraph.Index + 1);
				if (paragraph.IsInList())
					paragraph.PieceTable.RemoveNumberingFromParagraph(paragraph);
			}
		}
		protected internal TableCell CalculateTableCellToAdjustIndex(Paragraph paragraph, DocumentLogPosition pos) {
			if (pos != paragraph.LogPosition)
				return null;
			if (paragraph.Index == ParagraphIndex.Zero)
				return paragraph.GetCell();
			TableCell cell = paragraph.GetCell();
			if (cell == null)
				return null;
			TableCell parentCell = cell.Table.ParentCell;
			if (parentCell != null && parentCell.StartParagraphIndex == cell.StartParagraphIndex)
				return cell;
			Paragraph prevParagraph = ActivePieceTable.Paragraphs[paragraph.Index - 1];
			TableCell prevParagraphCell = prevParagraph.GetCell();
			if (prevParagraphCell == null)
				return null;
			if (!Object.ReferenceEquals(prevParagraphCell.Table, cell.Table) && prevParagraphCell.Table.NestedLevel == cell.Table.NestedLevel)
				return cell;
			return null;
		}
		protected internal override void AfterUpdate() {
			base.AfterUpdate();
			UpdateCaretPosition(DocumentLayoutDetailsLevel.Character);
			InputPosition newInputPosition = CaretPosition.GetInputPosition();
			newInputPosition.CopyFormattingFrom(oldInputPosition);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Paragraphs, state.Enabled);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region InsertParagraphIntoNonEmptyParagraphCoreCommand
	public class InsertParagraphIntoNonEmptyParagraphCoreCommand : InsertParagraphCoreCommand {
		public InsertParagraphIntoNonEmptyParagraphCoreCommand(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = state.Enabled && DocumentModel.BehaviorOptions.PageBreakInsertMode == PageBreakInsertMode.NewLine;
			if (state.Enabled) {
				ParagraphIndex paragraphIndex = this.CaretPosition.GetInputPosition().ParagraphIndex;
				state.Enabled = ActivePieceTable.Paragraphs[paragraphIndex].Length > 1;
			}
		}
	}
	#endregion
}

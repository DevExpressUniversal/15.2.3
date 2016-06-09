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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeIndentCommand (abstract class)
	public abstract class ChangeIndentCommand : RichEditMenuItemSimpleCommand {
		protected ChangeIndentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeIndent; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeIndentDescription; } }
		protected internal ParagraphIndex StartIndex { get { return DocumentModel.Selection.Interval.NormalizedStart.ParagraphIndex; } }
		protected internal ParagraphIndex EndIndex { get { return DocumentModel.Selection.Interval.NormalizedEnd.ParagraphIndex; } }
		#endregion
		protected internal ParagraphIndex GetEndParagraphIndex() {
			Selection selection = DocumentModel.Selection;
			if (selection.NormalizedEnd > new DocumentLogPosition(0) && selection.NormalizedEnd != selection.NormalizedStart)
				return ActivePieceTable.FindParagraphIndex(selection.NormalizedEnd - 1, false);
			else
				return ActivePieceTable.FindParagraphIndex(selection.NormalizedEnd, false);
		}
		protected internal virtual bool SelectedOnlyParagraphWithNumeration() {
			ParagraphCollection paragraphs = ActivePieceTable.Paragraphs;
			ParagraphIndex endIndex = GetEndParagraphIndex();
			for (ParagraphIndex i = StartIndex; i <= endIndex; i++) {
				if (!paragraphs[i].IsInList())
					return false;
			}
			return true;
		}
		protected internal virtual bool SelectedFirstParagraphInList() {
			ParagraphCollection paragraphs = ActivePieceTable.Paragraphs;
			NumberingListIndex listIndex = paragraphs[StartIndex].GetNumberingListIndex();
			if (StartIndex == new ParagraphIndex(0))
				return true;
			for (ParagraphIndex i = StartIndex - 1; i >= new ParagraphIndex(0); i--) {
				Paragraph paragraph = paragraphs[i];
				if (paragraph.IsInList() && paragraph.GetNumberingListIndex() == listIndex)
					return false;
			}
			return true;
		}
		protected internal virtual bool SelectionBeginFirstRowStartPos() {
			if (DocumentModel.Selection.Length == 0) {
				Paragraph paragraph = ActivePieceTable.Paragraphs[StartIndex];
				return DocumentModel.Selection.Start == paragraph.LogPosition;
			}
			return false;
		}
		protected internal DocumentLayoutPosition GetStartParagraphLayoutPosition() {
			Paragraph paragraph = ActivePieceTable.Paragraphs[StartIndex];
			DocumentLayoutPosition paragraphLayoutPosition = ActiveView.DocumentLayout.CreateLayoutPosition(ActivePieceTable, paragraph.LogPosition, 0);
			paragraphLayoutPosition.Update(ActiveView.DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row);
			return paragraphLayoutPosition;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.ParagraphFormatting);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
}

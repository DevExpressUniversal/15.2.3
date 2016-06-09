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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Utils;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region PreviousParagraphCommand
	public class PreviousParagraphCommand : PrevNextParagraphCommandBase {
		public PreviousParagraphCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousParagraphCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.PreviousParagraph; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousParagraphCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MovePreviousParagraph; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousParagraphCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MovePreviousParagraphDescription; } }
		#endregion
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			ParagraphIndex paragraphIndex = pos.ParagraphIndex;
			Paragraph paragraph = ActivePieceTable.Paragraphs[paragraphIndex];
			if (pos.LogPosition == paragraph.LogPosition)
				return GetPrevPosition(paragraphIndex);
			DocumentLogPosition result = GetVisibleLogPosition(paragraph);
			if (result == pos.LogPosition)
				return GetPrevPosition(paragraphIndex);
			else
				return result;
		}
		protected DocumentLogPosition GetPrevPosition(ParagraphIndex paragraphIndex) {
			ParagraphIndex prevParagraphIndex = paragraphIndex - 1;
			if (prevParagraphIndex >= new ParagraphIndex(0))
				return GetVisibleLogPosition(ActivePieceTable.Paragraphs[prevParagraphIndex]);
			else
				return GetMinPosition();
		}
		protected override DocumentLogPosition GetValidLogPosition(DocumentModelPosition newPos, Paragraph paragraph) {
			if (newPos.ParagraphIndex <= paragraph.Index)
				return newPos.LogPosition;
			return GetPrevVisiblePosition(paragraph.FirstRunIndex).LogPosition;
		}
		protected DocumentLogPosition GetMinPosition() {
			DocumentModelPosition modelPos = DocumentModelPosition.FromParagraphStart(ActivePieceTable, new ParagraphIndex(0));
			if (VisibleTextFilter.IsRunVisible(modelPos.RunIndex))
				return modelPos.LogPosition;
			return GetNextVisiblePosition(modelPos);
		}
	}
	#endregion
	#region ExtendPreviousParagraphCommand
	public class ExtendPreviousParagraphCommand : PreviousParagraphCommand {
		public ExtendPreviousParagraphCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendPreviousParagraphCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendPreviousParagraph; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override void UpdateTableSelectionAfterSelectionUpdated(DocumentLogPosition logPosition) {
			DocumentModel.Selection.UpdateTableSelectionEnd(logPosition);
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			DocumentLogPosition result = base.ChangePosition(pos);
			ExtendKeybordSelectionHorizontalDirectionCalculator calculator = new ExtendKeybordSelectionHorizontalDirectionCalculator(DocumentModel);
			return calculator.CalculatePrevPosition(result);
		}
	}
	#endregion
}

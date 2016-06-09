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
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region NextParagraphCommand
	public class NextParagraphCommand : PrevNextParagraphCommandBase {
		public NextParagraphCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextParagraphCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.NextParagraph; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextParagraphCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveNextParagraph; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextParagraphCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveNextParagraphDescription; } }
		#endregion
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			ParagraphIndex paragraphIndex = pos.ParagraphIndex + 1;
			ParagraphCollection paragraphs = ActivePieceTable.Paragraphs;
			if (paragraphIndex < new ParagraphIndex(paragraphs.Count))
				return GetVisibleLogPosition(paragraphs[paragraphIndex]);
			else
				return GetMaxPosition();
		}
		protected internal virtual DocumentLogPosition GetMaxPosition() {
			DocumentModelPosition modelPos = DocumentModelPosition.FromParagraphEnd(ActivePieceTable, new ParagraphIndex(ActivePieceTable.Paragraphs.Count - 1));
			if (VisibleTextFilter.IsRunVisible(modelPos.RunIndex))
				return modelPos.LogPosition;
			return GetPrevVisiblePosition(modelPos);
		}
	}
	#endregion
	#region ExtendNextParagraphCommand
	public class ExtendNextParagraphCommand : NextParagraphCommand {
		public ExtendNextParagraphCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendNextParagraphCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendNextParagraph; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override DocumentLogPosition GetMaxPosition() {
			return ActivePieceTable.DocumentEndLogPosition + 1;
		}
		protected internal override void UpdateTableSelectionAfterSelectionUpdated(DocumentLogPosition logPosition) {
			DocumentModel.Selection.UpdateTableSelectionEnd(logPosition);
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			DocumentLogPosition result = base.ChangePosition(pos);
			ExtendKeybordSelectionHorizontalDirectionCalculator calculator = new ExtendKeybordSelectionHorizontalDirectionCalculator(DocumentModel);
			return calculator.CalculateNextPosition(result);
		}
	}
	#endregion
}

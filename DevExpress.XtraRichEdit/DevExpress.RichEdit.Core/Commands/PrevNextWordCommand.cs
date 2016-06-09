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
using DevExpress.XtraRichEdit.SpellChecker;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region PrevNextWordCommand (abstract class)
	public abstract class PrevNextWordCommand : RichEditSelectionCommand {
		protected PrevNextWordCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return true; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected IVisibleTextFilter VisibleTextFilter { get { return ActivePieceTable.NavigationVisibleTextFilter; } }
		#endregion
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected virtual DocumentLogPosition GetNextVisibleLogPosition(DocumentModelPosition pos) {
			DocumentLogPosition result = VisibleTextFilter.GetNextVisibleLogPosition(pos, true);
			return result < ActivePieceTable.DocumentEndLogPosition ? result - 1 : result;
		}
		protected virtual DocumentLogPosition GetPrevVisibleLogPosition(DocumentModelPosition pos) {
			DocumentLogPosition result = VisibleTextFilter.GetPrevVisibleLogPosition(pos, true);
			return result > ActivePieceTable.DocumentStartLogPosition ? result + 1 : result;
		}
		protected DocumentModelPosition GetModelPosition(DocumentLogPosition pos) {
			return PositionConverter.ToDocumentModelPosition(ActivePieceTable, pos);
		}
	}
	#endregion
	#region PreviousWordCommand
	public class PreviousWordCommand : PrevNextWordCommand {
		public PreviousWordCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousWordCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.PreviousWord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousWordCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MovePreviousWord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousWordCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MovePreviousWordDescription; } }
		#endregion
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			PieceTableIterator iterator = new VisibleWordsIterator(ActivePieceTable);
			if (!ExtendSelection && DocumentModel.Selection.Length > 0)
				return pos.LogPosition;
			DocumentModelPosition result = iterator.MoveBack(pos);
			return result.LogPosition;
		}
	}
	#endregion
	#region ExtendPreviousWordCommand
	public class ExtendPreviousWordCommand : PreviousWordCommand {
		public ExtendPreviousWordCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendPreviousWordCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendPreviousWord; } }
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
	#region NextWordCommand
	public class NextWordCommand : PrevNextWordCommand {
		public NextWordCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextWordCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.NextWord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextWordCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveNextWord; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextWordCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveNextWordDescription; } }
		#endregion
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			PieceTableIterator iterator = new VisibleWordsIterator(ActivePieceTable);
			if (!ExtendSelection && DocumentModel.Selection.Length > 0)
				return pos.LogPosition;
			DocumentModelPosition result = iterator.MoveForward(pos);
			return result.LogPosition;
		}
	}
	#endregion
	#region ExtendNextWordCommand
	public class ExtendNextWordCommand : NextWordCommand {
		public ExtendNextWordCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendNextWordCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendNextWord; } }
		protected internal override bool ExtendSelection { get { return true; } }
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

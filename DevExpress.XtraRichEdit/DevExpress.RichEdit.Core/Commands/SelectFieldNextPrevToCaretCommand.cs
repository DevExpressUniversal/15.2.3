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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Native;
namespace DevExpress.XtraRichEdit.Commands {
	#region SelectFieldNextPrevToCaretCommand (abstract class)
	public abstract class SelectFieldNextPrevToCaretCommand : RichEditSelectionCommand {
		protected SelectFieldNextPrevToCaretCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal IVisibleTextFilter VisibleTextFilter { get { return ActivePieceTable.NavigationVisibleTextFilter; } }
		protected internal override void ChangeSelection(Selection selection) {
			DocumentModelPosition deletedPosition = GetDeletedPosition(selection);
			Field field = ActivePieceTable.FindFieldByRunIndex(deletedPosition.RunIndex);
			DocumentModelPosition startSelection = DocumentModelPosition.FromRunStart(ActivePieceTable, field.FirstRunIndex);
			DocumentModelPosition endSelection = DocumentModelPosition.FromRunEnd(ActivePieceTable, field.LastRunIndex);
			if (!field.IsCodeView && field.Result.Start == field.Result.End) {
				DocumentLogPosition start;
				DocumentLogPosition end;
				if ((TryGetVisibleStart(startSelection, out start) | TryGetVisibleEnd(endSelection, out end)) == true) {
					selection.Start = start;
					selection.End = end;
				}
			}
			else {
				selection.Start = startSelection.LogPosition;
				selection.End = endSelection.LogPosition;
			}
		}
		bool TryGetVisibleStart(DocumentModelPosition start, out DocumentLogPosition result) {
			result = VisibleTextFilter.GetPrevVisibleLogPosition(start, false);
			if (result >= ActivePieceTable.DocumentStartLogPosition)
				return true;
			result = ActivePieceTable.DocumentStartLogPosition;
			return false;
		}
		bool TryGetVisibleEnd(DocumentModelPosition end, out DocumentLogPosition result) {
			result = VisibleTextFilter.GetNextVisibleLogPosition(end, false);
			if (result <= ActivePieceTable.DocumentEndLogPosition)
				return true;
			result = ActivePieceTable.DocumentEndLogPosition;
			return false;
		}
		public override void UpdateUIState(DevExpress.Utils.Commands.ICommandUIState state) {
			RunIndex runIndex = GetDeletedPosition(DocumentModel.Selection).RunIndex;
			state.Enabled = DocumentModel.Selection.Length == 0 && IsFieldRun(ActivePieceTable.Runs[runIndex]);
		}
		protected virtual DocumentModelPosition GetDeletedPosition(Selection selection) {
			return selection.Interval.NormalizedEnd;
		}
		protected virtual bool IsFieldRun(TextRunBase run) {
			return run is FieldCodeRunBase || run is FieldResultEndRun;
		}
	}
	#endregion
	#region SelectFieldNextToCaretCommand
	public class SelectFieldNextToCaretCommand : SelectFieldNextPrevToCaretCommand {
		public SelectFieldNextToCaretCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectFieldNextToCaret; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectFieldNextToCaretDescription; } }
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
	}
	#endregion
	#region SelectFieldPrevToCaretCommand
	public class SelectFieldPrevToCaretCommand : SelectFieldNextPrevToCaretCommand {
		public SelectFieldPrevToCaretCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectFieldPrevToCaret; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectFieldPrevToCaretDescription; } }
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected override DocumentModelPosition GetDeletedPosition(Selection selection) {
			if (selection.NormalizedStart > ActivePieceTable.DocumentStartLogPosition)
				return DocumentModelPosition.MoveBackward(selection.Interval.NormalizedStart);
			return new DocumentModelPosition(ActivePieceTable);
		}
	}
	#endregion
}

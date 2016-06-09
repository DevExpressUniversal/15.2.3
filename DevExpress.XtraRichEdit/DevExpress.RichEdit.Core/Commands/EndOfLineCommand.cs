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
using DevExpress.Office;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
#if SL
using System.Windows.Controls.Primitives;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region EndOfLineCommand
	public class EndOfLineCommand : RichEditSelectionCommand {
		public EndOfLineCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EndOfLineCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.EndOfLine; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EndOfLineCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveToEndOfLine; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EndOfLineCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveToEndOfLineDescription; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.Row; } }
		#endregion
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return CaretPosition.LayoutPosition.Row.GetLastPosition(ActivePieceTable).LogPosition;
		}
		protected internal override bool ApplyNewPositionToSelectionEnd(Selection selection, DocumentLogPosition logPosition) {
			base.ApplyNewPositionToSelectionEnd(selection, logPosition);
			if (ExtendSelection) {
				if (selection.End < ActivePieceTable.DocumentEndLogPosition) {
					selection.End++;
					return true;
				}
				else
					return false;
			}
			DocumentModelPosition selectionIntervalEnd = selection.Interval.End;
			TextRunBase run = ActivePieceTable.Runs[selectionIntervalEnd.RunIndex];
			if (run is ParagraphRun)
				return false;
			else {
				if (ActivePieceTable.TextBuffer[run.StartIndex + selectionIntervalEnd.RunOffset] == Characters.LineBreak)
					return false;
				selection.End++;
				return true;
			}
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ExtendSelectionEndToParagraphMark(Selection selection, DocumentLogPosition logPosition) {
			return logPosition;
		}
#if SL
		protected internal override void ExecuteCore() {
			if (DocumentServer.ActualReadOnly && !Control.ShowCaretInReadOnly)
				EmulateVerticalScroll(ScrollEventType.Last);
			else
				base.ExecuteCore();
		}
#endif
	}
	#endregion
	#region ExtendEndOfLineCommand
	public class ExtendEndOfLineCommand : EndOfLineCommand {
		public ExtendEndOfLineCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendEndOfLineCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendEndOfLine; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
}

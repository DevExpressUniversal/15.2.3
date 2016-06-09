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
using DevExpress.XtraRichEdit.Tables.Native;
#if SL
using System.Windows.Controls.Primitives;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region NextCharacterCommand
	public class NextCharacterCommand : RichEditSelectionCommand {
		public NextCharacterCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextCharacterCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.NextCharacter; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextCharacterCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveForward; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextCharacterCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveForwardDescription; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		#endregion
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			if (!ExtendSelection && DocumentModel.Selection.Length > 0) {
				if(!DocumentModel.Selection.IsSelectFieldPictureResult())
				return pos.LogPosition;
			}
			if (pos.LogPosition < GetMaxPosition())
				return GetNextVisibleLogPosition(pos);
			else
				return pos.LogPosition;
		}
		protected internal virtual DocumentLogPosition GetNextVisibleLogPosition(DocumentModelPosition pos) {
			IVisibleTextFilter textFilter = ActivePieceTable.NavigationVisibleTextFilter;
			return textFilter.GetNextVisibleLogPosition(pos, true);
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal virtual DocumentLogPosition GetMaxPosition() {
			return ActivePieceTable.DocumentEndLogPosition;
		}
#if SL
		protected internal override void ExecuteCore() {
			if (DocumentServer.ActualReadOnly && !Control.ShowCaretInReadOnly)
				EmulateHorizontalScroll(ScrollEventType.SmallIncrement);
			else
				base.ExecuteCore();
		}
#endif
	}
	#endregion
	#region ExtendNextCharacterCommand
	public class ExtendNextCharacterCommand : NextCharacterCommand {
		public ExtendNextCharacterCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool ExtendSelection { get { return true; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendNextCharacterCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendNextCharacter; } }
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

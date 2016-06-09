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
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Utils;
#if SL
using System.Windows.Controls.Primitives;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region PreviousCharacterCommand
	public class PreviousCharacterCommand : RichEditSelectionCommand {
		public PreviousCharacterCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousCharacterCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.PreviousCharacter; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousCharacterCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveBackward; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousCharacterCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveBackwardDescription; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return true; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		#endregion
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			if (!ExtendSelection && DocumentModel.Selection.Length > 0) {
				if (!DocumentModel.Selection.IsSelectFieldPictureResult())
				return pos.LogPosition;
			}
			if (pos.LogPosition > ActivePieceTable.DocumentStartLogPosition)
				return GetPrevVisibleLogPosition(pos);
			else
				return pos.LogPosition;
		}
		protected internal virtual DocumentLogPosition GetPrevVisibleLogPosition(DocumentModelPosition pos) {
			IVisibleTextFilter textFilter = ActivePieceTable.NavigationVisibleTextFilter;
			DocumentLogPosition result = textFilter.GetPrevVisibleLogPosition(pos, true);
			if (result < ActivePieceTable.DocumentStartLogPosition)
				return pos.LogPosition;
			return result;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
#if SL
		protected internal override void ExecuteCore() {
			if (DocumentServer.ActualReadOnly && !Control.ShowCaretInReadOnly)
				EmulateHorizontalScroll(ScrollEventType.SmallDecrement);
			else
				base.ExecuteCore();
		}
#endif
	}
	#endregion
	#region ExtendKeybordSelectionHorizontalDirectionCalculator
	public class ExtendKeybordSelectionHorizontalDirectionCalculator {
		readonly DocumentModel documentModel;
		public ExtendKeybordSelectionHorizontalDirectionCalculator(DocumentModel documentModel) {
			this.documentModel = documentModel;
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public PieceTable ActivePieceTable { get { return documentModel.ActivePieceTable; } }
		public DocumentLogPosition CalculateNextPosition(DocumentLogPosition logPosition) {
			return logPosition;
		}
		public DocumentLogPosition CalculatePrevPosition(DocumentLogPosition logPosition) {
			return logPosition;
		}
	}
	#endregion
	#region ExtendPreviousCharacterCommand
	public class ExtendPreviousCharacterCommand : PreviousCharacterCommand {
		public ExtendPreviousCharacterCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendPreviousCharacterCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ExtendPreviousCharacter; } }
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

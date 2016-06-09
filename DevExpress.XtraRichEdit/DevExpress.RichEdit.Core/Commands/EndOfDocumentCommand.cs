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
using System.ComponentModel;
#if SL
using System.Windows.Controls.Primitives;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region EndOfDocumentCommand
	public class EndOfDocumentCommand : RichEditSelectionCommand {
		public EndOfDocumentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EndOfDocumentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.EndOfDocument; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EndOfDocumentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveToEndOfDocument; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EndOfDocumentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveToEndOfDocumentDescription; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		#endregion
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return ActivePieceTable.DocumentEndLogPosition;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
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
	#region ExtendEndOfDocumentCommand
	public class ExtendEndOfDocumentCommand : EndOfDocumentCommand {
		public ExtendEndOfDocumentCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendEndOfDocumentCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendEndOfDocument; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override void UpdateTableSelectionAfterSelectionUpdated(DocumentLogPosition logPosition) {
			DocumentModel.Selection.UpdateTableSelectionStart(logPosition);
			DocumentModel.Selection.UpdateTableSelectionEnd(logPosition);
		}
	}
	#endregion
}

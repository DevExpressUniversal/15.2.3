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
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Services;
using System.ComponentModel;
using DevExpress.Office.History;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	public class ImeUndoCommand : UndoCommand {
		public ImeUndoCommand(IRichEditControl control) 
			: base(control) { 
		}
		public override RichEditCommandId Id {
			get {
				return RichEditCommandId.ImeUndo;
			}
		}
		protected internal override void PerformHistoryOperation(DocumentHistory history) {
			history.Undo();
		}
		protected internal override void InvalidateDocumentLayout() {
			DocumentModel.InvalidateDocumentLayoutFrom(DocumentModel.Selection.Interval.NormalizedEnd.RunIndex);
		}
	}
	#region UndoCommand
	public class UndoCommand : HistoryCommandBase {
		public UndoCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UndoCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.Undo; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UndoCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_Undo; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UndoCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_UndoDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UndoCommandImageName")]
#endif
		public override string ImageName { get { return "Undo"; } }
		#endregion
		protected internal override void PerformHistoryOperation(DocumentHistory history) {
			IImeService imeService = Control.GetService(typeof(IImeService)) as IImeService;
			if (imeService != null && imeService.IsActive)
				imeService.Cancel();
			history.Undo();
		}
		protected internal override bool CanPerformHistoryOperation(DocumentHistory history) {
			return history.CanUndo;
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Undo, CanPerformHistoryOperation(DocumentModel.History));
		}
	}
	#endregion
}

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
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Commands {
	#region UpdateTableOfContentsCommand
	public class UpdateTableOfContentsCommand : FieldBasedRichEditMenuItemSimpleCommand {
		public UpdateTableOfContentsCommand(IRichEditControl control)
			: base(control, null) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateTableOfContentsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.UpdateTableOfContents; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateTableOfContentsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_UpdateTableOfContents; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateTableOfContentsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_UpdateTableOfContentsDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateTableOfContentsCommandImageName")]
#endif
		public override string ImageName { get { return "UpdateTableOfContents"; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.ResetTemporaryLayout();
			PieceTable pieceTable = DocumentModel.MainPieceTable;
			List<Field> fields = pieceTable.GetTocFields();
			int count = fields.Count;
			FieldUpdater updater = pieceTable.FieldUpdater;
			for (int i = 0; i < count; i++) {
				updater.UpdateFieldAndNestedFields(fields[i]);
				DocumentModel.ResetTemporaryLayout();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable;
			state.Visible = true;
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
	#region UpdateTableOfFiguresCommand
	public class UpdateTableOfFiguresCommand : UpdateTableOfContentsCommand {
		public UpdateTableOfFiguresCommand(IRichEditControl control)
			: base(control) { }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateTableOfFiguresCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.UpdateTableOfFigures; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateTableOfFiguresCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_UpdateTableOfFigures; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateTableOfFiguresCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_UpdateTableOfFiguresDescription; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
}

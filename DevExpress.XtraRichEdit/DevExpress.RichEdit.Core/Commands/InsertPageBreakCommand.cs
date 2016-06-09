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
using DevExpress.Utils.Commands;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertPageBreakCommand
	public class InsertPageBreakCommand : TransactedInsertObjectCommand {
		public InsertPageBreakCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertPageBreakCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertPageBreak; } }
		protected internal override RichEditCommand InsertObjectCommand { get { return (RichEditCommand)Commands[2]; } }
		protected internal override void CreateInsertObjectCommands() {
			Commands.Add(new InsertParagraphIntoNonEmptyParagraphCoreCommand(Control));
			base.CreateInsertObjectCommands();
		}
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertPageBreakCoreCommand(Control);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled)
				InsertObjectCommand.UpdateUIState(state);
		}
	}
	#endregion
	#region InsertPageBreakCommand2
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertPageBreakCommand2Id")]
#endif
	public class InsertPageBreakCommand2 : InsertPageBreakCommand {
		public InsertPageBreakCommand2(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertPageBreakCommand2Id")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertPageBreak2; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertPageBreakCoreCommand2(Control);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertPageBreakCoreCommand
	public class InsertPageBreakCoreCommand : InsertSpecialCharacterCommandBase {
		public InsertPageBreakCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertPageBreak; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertPageBreakDescription; } }
		public override string ImageName { get { return "InsertPageBreak"; } }
		protected internal override char Character { get { return Characters.PageBreak; } }
		protected override bool ResetMerging { get { return true; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled)
				state.Enabled = ActivePieceTable.IsMain && !IsSelectionEndInTableCell();
		}
	}
	#endregion
	#region InsertPageBreakCoreCommand2
	public class InsertPageBreakCoreCommand2 : InsertPageBreakCoreCommand {
		public InsertPageBreakCoreCommand2(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertPageBreak2; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertPageBreak2Description; } }
	}
	#endregion
}

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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleListCommandBase (abstract class)
	public abstract class ToggleListCommandBase : RichEditMenuItemSimpleCommand {
		readonly DeleteNumerationFromParagraphCommand deleteNumerationCommand;
		protected ToggleListCommandBase(IRichEditControl control)
			: base(control) {
			this.deleteNumerationCommand = new DeleteNumerationFromParagraphCommand(control);
		}
		#region Properties
		protected abstract NumberingListCommandBase InsertNumerationCommand { get; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleListCommandBaseMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertSimpleList; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleListCommandBaseDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertSimpleListDescription; } }
		#endregion
		public override void UpdateUIState(ICommandUIState state) {
			InsertNumerationCommand.UpdateUIState(state);
			UpdateUIStateViaService(state);
		}
		protected internal override void ExecuteCore() {
		}
		public override void ForceExecute(ICommandUIState state) {
			NotifyBeginCommandExecution(state);
			try {
				if (state.Checked)
					deleteNumerationCommand.ForceExecute(state);
				else
					InsertNumerationCommand.ForceExecute(state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	#endregion
	#region ToggleBulletedListCommand
	public class ToggleBulletedListCommand : ToggleListCommandBase {
		readonly InsertBulletListCommand insertNumerationCommand;
		public ToggleBulletedListCommand(IRichEditControl control)
			: base(control) {
			this.insertNumerationCommand = new InsertBulletListCommand(control);
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleBulletedListCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ToggleBulletedListItem; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleBulletedListCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertBulletList; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleBulletedListCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertBulletListDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleBulletedListCommandImageName")]
#endif
		public override string ImageName { get { return "ListBullets"; } }
		protected override NumberingListCommandBase InsertNumerationCommand { get { return insertNumerationCommand; } }
		#endregion
	}
	#endregion
}

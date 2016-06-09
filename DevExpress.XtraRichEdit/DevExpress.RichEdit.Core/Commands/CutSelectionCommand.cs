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
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region CutSelectionCommand
	public class CutSelectionCommand : MultiCommand {
		public CutSelectionCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CutSelectionCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.CutSelection; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CutSelectionCommandImageName")]
#endif
		public override string ImageName { get { return "Cut"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CutSelectionCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_CutSelection; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CutSelectionCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_CutSelectionDescription; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteAllAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAllAvailable; } }
		#endregion
		protected internal override void CreateCommands() {
			Commands.Add(new CopySelectionCommand(Control));
			Commands.Add(new DeleteNonEmptySelectionCommand(Control));
		}
		public override void UpdateUIState(ICommandUIState state) {
			base.UpdateUIState(state);
			ApplyCommandRestrictionOnEditableControl(state, Options.Behavior.Cut, state.Enabled);
		}
	}
	#endregion
#if SILVERLIGHT
	#region CutInternalSelectionCommand
	public class CutInternalSelectionCommand : CutSelectionCommand {
		public CutInternalSelectionCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override void CreateCommands() {
			Commands.Add(new CopyInternalSelectionCommand(Control));
			Commands.Add(new DeleteNonEmptySelectionCommand(Control));
		}
	}
	#endregion
#endif
}

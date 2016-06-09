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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeSectionLineNumberingCommand
	public class ChangeSectionLineNumberingCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public ChangeSectionLineNumberingCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeSectionLineNumbering; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeSectionLineNumberingDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeSectionLineNumbering; } }
		public override string ImageName { get { return "LineNumbering"; } }
		#endregion
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Sections, state.Enabled);
			ApplyDocumentProtectionToSelectedSections(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
	#region SetPredefinedSectionLineNumberingCommand (abstract class)
	public abstract class SetPredefinedSectionLineNumberingCommand : ToggleChangeSectionFormattingCommandBase<LineNumberingRestart> {
		protected SetPredefinedSectionLineNumberingCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal abstract LineNumberingRestart LineNumberingRestart { get; }
		#endregion
		protected internal override SectionPropertyModifier<LineNumberingRestart> CreateModifier(ICommandUIState state) {
			return new SectionLineNumberingStepAndRestartModifier(LineNumberingRestart);
		}
		protected internal override bool IsCheckedValue(LineNumberingRestart value) {
			return value.Equals(LineNumberingRestart);
		}
	}
	#endregion
	#region SetSectionLineNumberingNoneCommand
	public class SetSectionLineNumberingNoneCommand : SetPredefinedSectionLineNumberingCommand {
		public SetSectionLineNumberingNoneCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingNoneCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetSectionLineNumberingNone; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingNoneCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionLineNumberingNone; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingNoneCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionLineNumberingNoneDescription; } }
		protected internal override LineNumberingRestart LineNumberingRestart { get { return (LineNumberingRestart)(-1); } }
		#endregion
	}
	#endregion
	#region SetSectionLineNumberingContinuousCommand
	public class SetSectionLineNumberingContinuousCommand : SetPredefinedSectionLineNumberingCommand {
		public SetSectionLineNumberingContinuousCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingContinuousCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetSectionLineNumberingContinuous; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingContinuousCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionLineNumberingContinuous; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingContinuousCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionLineNumberingContinuousDescription; } }
		protected internal override LineNumberingRestart LineNumberingRestart { get { return LineNumberingRestart.Continuous; } }
		#endregion
	}
	#endregion
	#region SetSectionLineNumberingRestartNewPageCommand
	public class SetSectionLineNumberingRestartNewPageCommand : SetPredefinedSectionLineNumberingCommand {
		public SetSectionLineNumberingRestartNewPageCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingRestartNewPageCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetSectionLineNumberingRestartNewPage; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingRestartNewPageCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionLineNumberingRestartNewPage; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingRestartNewPageCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionLineNumberingRestartNewPageDescription; } }
		protected internal override LineNumberingRestart LineNumberingRestart { get { return LineNumberingRestart.NewPage; } }
		#endregion
	}
	#endregion
	#region SetSectionLineNumberingRestartNewSectionCommand
	public class SetSectionLineNumberingRestartNewSectionCommand : SetPredefinedSectionLineNumberingCommand {
		public SetSectionLineNumberingRestartNewSectionCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingRestartNewSectionCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetSectionLineNumberingRestartNewSection; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingRestartNewSectionCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionLineNumberingRestartNewSection; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionLineNumberingRestartNewSectionCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionLineNumberingRestartNewSectionDescription; } }
		protected internal override LineNumberingRestart LineNumberingRestart { get { return LineNumberingRestart.NewSection; } }
		#endregion
	}
	#endregion
}

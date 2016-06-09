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
using System.Text;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeParagraphLineSpacingCommand
	public class ChangeParagraphLineSpacingCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public ChangeParagraphLineSpacingCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeParagraphLineSpacingCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeParagraphLineSpacing; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeParagraphLineSpacingCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeParagraphLineSpacingDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeParagraphLineSpacingCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeParagraphLineSpacing; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeParagraphLineSpacingCommandImageName")]
#endif
		public override string ImageName { get { return "LineSpacing"; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.ParagraphFormatting, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
	#region ShowLineSpacingFormCommand
	public class ShowLineSpacingFormCommand : ShowParagraphFormCommand {
		public ShowLineSpacingFormCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLineSpacingFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowLineSpacingForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLineSpacingFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowLineSpacingFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLineSpacingFormCommandImageName")]
#endif
		public override string ImageName { get { return String.Empty; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLineSpacingFormCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ShowLineSpacingForm; } }
	}
	#endregion
}

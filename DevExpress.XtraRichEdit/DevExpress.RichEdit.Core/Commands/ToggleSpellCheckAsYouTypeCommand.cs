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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraSpellChecker;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleSpellCheckAsYouTypeCommand
	public class ToggleSpellCheckAsYouTypeCommand : RichEditMenuItemSimpleCommand {
		public ToggleSpellCheckAsYouTypeCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleSpellCheckAsYouTypeCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleSpellCheckAsYouType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleSpellCheckAsYouTypeCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleSpellCheckAsYouTypeDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleSpellCheckAsYouTypeCommandImageName")]
#endif
		public override string ImageName { get { return "SpellCheckAsYouType"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleSpellCheckAsYouTypeCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleSpellCheckAsYouType; } }
		ISpellChecker SpellChecker { get { return DocumentServer.SpellChecker; } }
		protected internal override void ExecuteCore() {
			if (SpellChecker != null)
				SpellChecker.SpellCheckMode = IsChecked() ? SpellCheckMode.OnDemand : SpellCheckMode.AsYouType;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentServer.Enabled && SpellChecker != null;
			if (state.Enabled)
				state.Checked = IsChecked();
		}
		bool IsChecked() {
			return SpellChecker.SpellCheckMode == SpellCheckMode.AsYouType;
		}
	}
	#endregion
}

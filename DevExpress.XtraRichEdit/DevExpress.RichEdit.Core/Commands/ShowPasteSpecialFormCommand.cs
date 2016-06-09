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
using System.Reflection;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Commands.Internal;
#if !SL
using System.Collections;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowPasteSpecialFormCommand
	public class ShowPasteSpecialFormCommand : PasteSelectionCommand {
		public ShowPasteSpecialFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowPasteSpecialFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowPasteSpecialForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowPasteSpecialFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowPasteSpecialForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowPasteSpecialFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowPasteSpecialFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowPasteSpecialFormCommandMenuCaption")]
#endif
		public override string MenuCaption { get { return XtraRichEditLocalizer.GetString(MenuCaptionStringId); } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowPasteSpecialFormCommandDescription")]
#endif
		public override string Description { get { return XtraRichEditLocalizer.GetString(DescriptionStringId); } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowPasteSpecialFormCommandImageName")]
#endif
		public override string ImageName { get { return "PasteSpecial"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowPasteSpecialFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<PasteSpecialInfo> valueBasedState = state as IValueBasedCommandUIState<PasteSpecialInfo>;
				ShowPasteSpecialForm(valueBasedState.Value, ShowPasteSpecialFormCallback, state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ShowPasteSpecialFormCallback(PasteSpecialInfo properties, object callbackData) {
			if (properties != null) {
				PasteContentCommandBase command = properties.CreateCommand(Control);
				if (command != null) {
					this.Format = command.Format;
					Commands[1] = command; 
					DefaultCommandUIState state = new DefaultCommandUIState();
					state.Enabled = true;
					state.Visible = true;
					base.ForceExecute(state);
				}
			}
		}
		protected internal virtual void ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
			Control.ShowPasteSpecialForm(properties, callback, callbackData);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<PasteSpecialInfo> state = new DefaultValueBasedCommandUIState<PasteSpecialInfo>();
			state.Value = new PasteSpecialInfo();
			return state;
		}
	}
	#endregion
}

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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Forms;
using System.ComponentModel;
#if !SL
using System.Collections;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowFontFormCommand
	public class ShowFontFormCommand : ChangeCharacterFormattingCommandBase<MergedCharacterProperties> {
		public ShowFontFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowFontFormCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ShowFontForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowFontFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowFontForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowFontFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowFontFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowFontFormCommandImageName")]
#endif
		public override string ImageName { get { return "Font"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowFontFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<MergedCharacterProperties> valueBasedState = state as IValueBasedCommandUIState<MergedCharacterProperties>;
				ShowFontForm(valueBasedState.Value, ShowFontFormCallback, state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ShowFontFormCallback(MergedCharacterProperties properties, object callbackData) {
			IValueBasedCommandUIState<MergedCharacterProperties> valueBasedState = callbackData as IValueBasedCommandUIState<MergedCharacterProperties>;
			valueBasedState.Value = properties;
			base.ForceExecute(valueBasedState);
		}
		protected internal virtual void ShowFontForm(MergedCharacterProperties characterProperties, ShowFontFormCallback callback, object callbackData) {
			Control.ShowFontForm(characterProperties, callback, callbackData);
		}
		protected internal override RunPropertyModifier<MergedCharacterProperties> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<MergedCharacterProperties> valueBasedState = state as IValueBasedCommandUIState<MergedCharacterProperties>;
			return DocumentModel.CommandsCreationStrategy.CreateFontPropertiesModifier(valueBasedState.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<MergedCharacterProperties> valueBasedState = state as IValueBasedCommandUIState<MergedCharacterProperties>;
			if (valueBasedState == null)
				return;
			MergedCharacterProperties value = null;
			GetCurrentPropertyValue(out value);
			valueBasedState.Value = value;
		}
		protected internal override bool ObtainRunsPropertyValue(DocumentModelPosition start, int length, RunPropertyModifier<MergedCharacterProperties> modifier, out MergedCharacterProperties value) {
			MergedRunPropertyModifier<MergedCharacterProperties> mergedModifier = (MergedRunPropertyModifier<MergedCharacterProperties>)modifier;
			value = ActivePieceTable.ObtainMergedRunsPropertyValue(start.LogPosition, length, mergedModifier);
			return true;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<MergedCharacterProperties> result = new DefaultValueBasedCommandUIState<MergedCharacterProperties>();
			return result;
		}
	}
	#endregion
}

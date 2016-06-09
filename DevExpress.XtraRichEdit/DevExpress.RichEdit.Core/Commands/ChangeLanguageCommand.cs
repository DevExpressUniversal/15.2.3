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
using System.Linq;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeLanguageCommand
	public class ChangeLanguageCommand : ChangeCharacterFormattingCommandBase<LangInfo> {
		public ChangeLanguageCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeLanguageCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeLanguage; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeLanguageCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeLanguageDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeLanguageCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeLanguage; } }
		#endregion
		protected internal override RunPropertyModifier<LangInfo> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<LangInfo> valueBasedState = state as IValueBasedCommandUIState<LangInfo>;
			if (valueBasedState == null)
				Exceptions.ThrowInternalException();
			return new RunLanguageTypeModifier(valueBasedState.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<LangInfo> valueBasedState = state as IValueBasedCommandUIState<LangInfo>;
			if (valueBasedState != null) {
				LangInfo value;
				if (GetCurrentPropertyValue(out value))
					valueBasedState.Value = value;
				else {
					valueBasedState.Value = new LangInfo(null,null,null);
				}
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<LangInfo> result = new DefaultValueBasedCommandUIState<LangInfo>();
			result.Value = new LangInfo(null, null, null);
			return result;
		}
		protected internal override bool ObtainRunsPropertyValue(DocumentModelPosition start, int length, RunPropertyModifier<LangInfo> modifier, out LangInfo value) {
			return (ActivePieceTable.ObtainRunsPropertyValueIgnoreParagraphRun(start.LogPosition, length, modifier, out value));
		}
	}
	#endregion
}

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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeFontNameCommand
	public class ChangeFontNameCommand : ChangeCharacterFormattingCommandBase<string> {
		public ChangeFontNameCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFontNameCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFontName; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFontNameCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFontNameDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFontNameCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeFontName; } }
		#endregion
		protected internal override RunPropertyModifier<string> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (valueBasedState == null)
				Exceptions.ThrowInternalException();
			return new RunFontNamePropertyModifier(valueBasedState.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (valueBasedState != null) {
				string value;
				if (GetCurrentPropertyValue(out value))
					valueBasedState.Value = value;
				else {
					valueBasedState.Value = null;
				}
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<string> result = new DefaultValueBasedCommandUIState<string>();
			result.Value = String.Empty;
			return result;
		}
	}
	#endregion
}

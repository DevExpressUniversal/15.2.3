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
	#region ChangeDoubleFontSizeCommand
	public class ChangeDoubleFontSizeCommand : ChangeCharacterFormattingCommandBase<int> {
		public ChangeDoubleFontSizeCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeDoubleFontSizeCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFontSize; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeDoubleFontSizeCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFontSizeDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeDoubleFontSizeCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeDoubleFontSize; } }
		#endregion
		protected internal override RunPropertyModifier<int> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<int?> valueBasedState = state as IValueBasedCommandUIState<int?>;
			if (valueBasedState == null)
				Exceptions.ThrowInternalException();
			if (valueBasedState.Value.HasValue)
				return new RunDoubleFontSizePropertyModifier(valueBasedState.Value.Value);
			else
				return new RunDoubleFontSizePropertyModifier(0);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<int?> valueBasedState = state as IValueBasedCommandUIState<int?>;
			if (valueBasedState != null) {
				int value;
				if (GetCurrentPropertyValue(out value))
					valueBasedState.Value = value;
				else {
					valueBasedState.Value = null;
				}
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<int?> result = new DefaultValueBasedCommandUIState<int?>();
			return result;
		}
	}
	#endregion
	#region ChangeFontSizeCommand
	public class ChangeFontSizeCommand : ChangeCharacterFormattingCommandBase<float> {
		public ChangeFontSizeCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFontSizeCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFontSize; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFontSizeCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFontSizeDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFontSizeCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeFontSize; } }
		#endregion
		protected internal override RunPropertyModifier<float> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<float?> valueBasedState = state as IValueBasedCommandUIState<float?>;
			if (valueBasedState == null)
				Exceptions.ThrowInternalException();
			if (valueBasedState.Value.HasValue)
				return new RunFontSizePropertyModifier(valueBasedState.Value.Value);
			else
				return new RunFontSizePropertyModifier(0);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<float?> valueBasedState = state as IValueBasedCommandUIState<float?>;
			if (valueBasedState != null) {
				float value;
				if (GetCurrentPropertyValue(out value))
					valueBasedState.Value = value;
				else {
					valueBasedState.Value = null;
				}
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<float?> result = new DefaultValueBasedCommandUIState<float?>();
			return result;
		}
	}
	#endregion
}

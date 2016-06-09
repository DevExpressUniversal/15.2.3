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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeFontBackColorCommand
	public class ChangeFontBackColorCommand : ChangeCharacterFormattingCommandBase<Color> {
		public ChangeFontBackColorCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFontBackColorCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_HighlightTextDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFontBackColorCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_HighlightText; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFontBackColorCommandImageName")]
#endif
		public override string ImageName { get { return "Highlight"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFontBackColorCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeFontBackColor; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<Color> valueBasedState = state as IValueBasedCommandUIState<Color>;
			if (valueBasedState != null) {
				Color value;
				GetCurrentPropertyValue(out value);
				valueBasedState.Value = value;
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<Color> result = new DefaultValueBasedCommandUIState<Color>();
			return result;
		}
		protected internal override RunPropertyModifier<Color> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<Color> valueBasedState = state as IValueBasedCommandUIState<Color>;
			if (valueBasedState == null)
				Exceptions.ThrowInternalException();
			return new RunBackColorModifier(valueBasedState.Value);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ChangeFontBackColorByMouseCommand
	public class ChangeFontBackColorByMouseCommand : RichEditCommand {
		#region Fields
		static Color color;
		static bool isChangeByMouse;
		ChangeFontBackColorCommand internalCommand;
		#endregion
		public ChangeFontBackColorByMouseCommand(IRichEditControl control)
			: base(control) {
			internalCommand = new ChangeFontBackColorCommand(Control);
		}
		#region Properties
		public static Color Color { get { return color; } set { color = value; } }
		public static bool IsChangeByMouse { get { return isChangeByMouse; } set { isChangeByMouse = value; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_IsNotValid; } }
		protected internal ChangeFontBackColorCommand InternalCommand { get { return internalCommand; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_IsNotValid; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			InternalCommand.UpdateUIState(state);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<Color> result = new DefaultValueBasedCommandUIState<Color>();
			return result;
		}
		public override void ForceExecute(ICommandUIState state) {
			NotifyBeginCommandExecution(state);
			try {
				ExecuteCore();
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ExecuteCore() {
			Selection selection = DocumentModel.Selection;			
			if (selection.Items.Count > 1)
				return;
			DocumentModelPosition start = InternalCommand.CalculateStartPosition(selection.ActiveSelection, false);
			DocumentModelPosition end = InternalCommand.CalculateEndPosition(selection.ActiveSelection, false);
			if ((end.LogPosition - start.LogPosition) <= 0)
				return;
			Color color;
			if (Color == DXColor.Empty)
				color = DXColor.Empty;
			else {
				if (InternalCommand.GetCurrentPropertyValue(out color) && color == Color)
					color = DXColor.Empty;
				else
					color = Color;
			}
			DefaultValueBasedCommandUIState<Color> state = new DefaultValueBasedCommandUIState<Color>();
			state.Value = color;
			state.Enabled = true;
			state.Visible = true;
			InternalCommand.ForceExecute(state);
			Command command;			
			if (selection.Start < selection.End)
				command = new NextCharacterCommand(Control);
			else
				command = new PreviousCharacterCommand(Control);
			command.Execute();
		}
	}
	#endregion
}

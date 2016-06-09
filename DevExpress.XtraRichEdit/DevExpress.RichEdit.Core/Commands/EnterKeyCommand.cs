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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using System.ComponentModel;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Data;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region EnterKeyCommand
	public class EnterKeyCommand : MultiCommand {
		public EnterKeyCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EnterKeyCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.EnterKey; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EnterKeyCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_EnterKey; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EnterKeyCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_EnterKeyDescription; } }
		protected internal override void CreateCommands() {
			Commands.Add(new OpenHyperlinkAtCaretPositionCommand(Control));
			Commands.Add(new InsertParagraphCommand(Control));
		}
	}
	#endregion
	#region OpenHyperlinkAtCaretPositionCommand
	public class OpenHyperlinkAtCaretPositionCommand : RichEditMenuItemSimpleCommand {
		Field hyperlinkField;
		public OpenHyperlinkAtCaretPositionCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_OpenHyperlinkAtCaretPosition; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_OpenHyperlinkAtCaretPositionDescription; } }
		protected Field HyperlinkField {
			get {
				if (hyperlinkField == null)
					hyperlinkField = GetHyperlinkFieldAtCaretPosition();
				return hyperlinkField;
			}
		}
		#endregion
		protected internal override void ExecuteCore() {
			if (HyperlinkField == null)
				return;
			InnerControl.OnHyperlinkClick(HyperlinkField, false);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = HyperlinkField != null;
			state.Visible = true;
		}
		protected internal virtual Field GetHyperlinkFieldAtCaretPosition() {
			DocumentLayoutPosition layoutPosition = ActiveView.CaretPosition.LayoutPosition;
			if (layoutPosition.DetailsLevel == DocumentLayoutDetailsLevel.Character) {
				RunIndex runIndex = layoutPosition.Character.StartPos.RunIndex;
				Field field = ActivePieceTable.GetHyperlinkField(runIndex);
				if (field != null && !field.IsCodeView) {
					DocumentModelPosition position = new DocumentModelPosition(ActivePieceTable);
					position.LogPosition = layoutPosition.LogPosition;
					position.Update();
					if (position.RunIndex == field.Code.Start && position.RunOffset == 0)
						return null;
					return field;
				}
			}
			return null;
		}
	}
	#endregion
}

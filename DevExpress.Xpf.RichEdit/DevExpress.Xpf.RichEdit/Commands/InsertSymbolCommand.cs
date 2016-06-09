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
using System.Windows;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Xpf.Core;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Xpf.RichEdit.Controls.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.RichEdit.Localization;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertSymbolWithFontCommand
	public class InsertSymbolWithFontCommand : RichEditCommand {
		string fontName;
		string text;
		public InsertSymbolWithFontCommand(IRichEditControl control, string text, string fontName)
			: base(control) {
			this.fontName = fontName;
			this.text = text;
		}
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_IsNotValid; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_IsNotValid; } }
		public override void ForceExecute(ICommandUIState state) {
			InsertTextCommand insertText = new InsertTextCommand(Control);
			insertText.Text = text;
			insertText.Execute();
			new ExtendPreviousCharacterCommand(Control).Execute();
			ChangeFontNameCommand changeFontCommand = new ChangeFontNameCommand(Control);
			if (changeFontCommand.CanExecute()) {
				DefaultValueBasedCommandUIState<string> changeFontCommandState = new DefaultValueBasedCommandUIState<string>();
				changeFontCommandState.Value = fontName;
				changeFontCommand.ForceExecute(changeFontCommandState);
			}
			new NextCharacterCommand(Control).Execute();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	#endregion
}

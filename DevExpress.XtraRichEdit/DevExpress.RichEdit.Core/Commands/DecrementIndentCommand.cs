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
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region DecrementIndentCommand
	public class DecrementIndentCommand : ChangeIndentCommand {
		public DecrementIndentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementIndentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.DecreaseIndent; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementIndentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DecrementIndent; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementIndentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DecrementIndentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementIndentCommandImageName")]
#endif
		public override string ImageName { get { return "IndentDecrease"; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (SelectedOnlyParagraphWithNumeration() ) {
				ProcessNumerationParagraph();
			}
			else
				DecrementParagraphIndent();
		}
		protected internal virtual void ProcessNumerationParagraph() {
			if (!SelectedFirstParagraphInList())
				DecrementNumerationFromParagraph();
			else
				DecrementNumerationParagraphIndent();
		}
		protected internal virtual void DecrementNumerationParagraphIndent() {
			DecrementNumerationParagraphIndentCommand command = new DecrementNumerationParagraphIndentCommand(Control);
			command.ForceExecute(CreateDefaultCommandUIState());
		}
		protected internal virtual void DecrementNumerationFromParagraph() {
			DecrementNumerationFromParagraphCommand command = new DecrementNumerationFromParagraphCommand(Control);
			command.ForceExecute(CreateDefaultCommandUIState());
		}
		protected internal virtual void DecrementParagraphIndent() {
			DecrementParagraphLeftIndentCommand command = new DecrementParagraphLeftIndentCommand(Control);
			command.ForceExecute(CreateDefaultCommandUIState());
		}
	}
	#endregion
	#region DecrementIndentByTheTabCommand
	public class DecrementIndentByTheTabCommand : DecrementIndentCommand {
		public DecrementIndentByTheTabCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override void DecrementParagraphIndent() {
			DecrementParagraphIndentCommand command = CreateDecrementParagraphIndentCommand();
			command.ForceExecute(CreateDefaultCommandUIState());
		}
		protected internal virtual DecrementParagraphIndentCommand CreateDecrementParagraphIndentCommand() {
			return new DecrementParagraphIndentCommand(Control);
		}
	}
	#endregion
}

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
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region IncrementIndentCommand
	public class IncrementIndentCommand : ChangeIndentCommand {
		public IncrementIndentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementIndentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.IncreaseIndent; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementIndentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementIndent; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementIndentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementIndentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementIndentCommandImageName")]
#endif
		public override string ImageName { get { return "IndentIncrease"; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (SelectedOnlyParagraphWithNumeration() )
				ProcessNumerationParagraph();
			else
				IncrementParagraphIndent();
		}
		protected internal virtual void ProcessNumerationParagraph() {
			if (!SelectedFirstParagraphInList())
				IncrementNumerationFromParagraph();
			else
				IncrementNumerationParagraphIndent();
		}
		protected internal virtual void IncrementNumerationParagraphIndent() {
			IncrementNumerationParagraphIndentCommand command = new IncrementNumerationParagraphIndentCommand(Control);
			command.Execute();
		}
		protected internal virtual void IncrementNumerationFromParagraph() {
			IncrementNumerationFromParagraphCommand command = new IncrementNumerationFromParagraphCommand(Control);
			command.Execute();
		}
		protected internal virtual void IncrementParagraphIndent() {
			IncrementParagraphLeftIndentCommand command = new IncrementParagraphLeftIndentCommand(Control);
			command.Execute();
		}
	}
	#endregion
	#region IncrementIndentByTheTabCommand
	public class IncrementIndentByTheTabCommand : IncrementIndentCommand {
		public IncrementIndentByTheTabCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementIndent; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementIndentDescription; } }
		#endregion
		protected internal override void IncrementParagraphIndent() {
			IncrementParagraphIndentCommand command = CreateIncrementParagraphIndentCommand();
			command.Execute();
		}
		protected internal virtual IncrementParagraphIndentCommand CreateIncrementParagraphIndentCommand() {
			return new IncrementParagraphIndentCommand(Control);
		}
	}
	#endregion
}

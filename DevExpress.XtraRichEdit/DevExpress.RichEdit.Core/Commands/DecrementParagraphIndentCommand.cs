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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Commands {
	#region DecrementParagraphIndentCommand
	public class DecrementParagraphIndentCommand : ChangeIndentCommand {
		public DecrementParagraphIndentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DecrementIndent; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DecrementIndentDescription; } }
		public override string ImageName { get { return "IndentDecrease"; } }
		#endregion
		protected internal override void ExecuteCore() {
			Paragraph startParagraph = ActivePieceTable.Paragraphs[StartIndex];
			if (startParagraph.LeftIndent == 0 && startParagraph.FirstLineIndent != 0)
				DecrementParagraphFirstLineIndent();
			if (SelectionBeginFirstRowStartPos() && FirstLineIndentIsPositive())
				DecrementParagraphFirstLineIndent();
			else {
				DecrementParagraphLeftIndentCommand command = CreateDecrementParagraphLeftIndentCommand();
				command.ForceExecute(CreateDefaultCommandUIState());
			}
		}
		protected internal virtual bool FirstLineIndentIsPositive() {
			Paragraph startParagraph = ActivePieceTable.Paragraphs[StartIndex];
			return startParagraph.FirstLineIndent > 0;
		}
		protected internal virtual DecrementParagraphLeftIndentCommand CreateDecrementParagraphLeftIndentCommand() {
			return new DecrementParagraphLeftIndentCommand(Control);
		}
		protected internal virtual void DecrementParagraphFirstLineIndent() {
			DecrementParagraphFirstLineIndentCommand command = CreateDecrementParagraphFirstLineIndentCommand();
			command.ForceExecute(CreateDefaultCommandUIState());
		}
		protected internal virtual DecrementParagraphFirstLineIndentCommand CreateDecrementParagraphFirstLineIndentCommand() {
			return new DecrementParagraphFirstLineIndentCommand(Control);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
}

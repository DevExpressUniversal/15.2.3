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
namespace DevExpress.XtraRichEdit.Commands {
	#region IncrementParagraphIndentCommand
	public class IncrementParagraphIndentCommand : ChangeIndentCommand {
		public IncrementParagraphIndentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementIndent; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementIndentDescription; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (SelectionBeginFirstRowStartPos() && IsFirstLineIndentLessDefaultTabSize()) {
				IncrementParagraphFirstLineIndentCommand incrementFirstLineIndent = CreateIncrementParagraphFirstLineIndentCommand();
				incrementFirstLineIndent.ForceExecute(CreateDefaultCommandUIState());
			}
			else {
				IncrementParagraphLeftIndentCommand incrementLeftIndent = CreateIncrementParagraphLeftIndentCommand();
				incrementLeftIndent.ForceExecute(CreateDefaultCommandUIState());
			}
		}
		protected internal virtual IncrementParagraphFirstLineIndentCommand CreateIncrementParagraphFirstLineIndentCommand() {
			return new IncrementParagraphFirstLineIndentCommand(Control);
		}
		protected internal virtual IncrementParagraphLeftIndentCommand CreateIncrementParagraphLeftIndentCommand() {
			return new IncrementParagraphLeftIndentCommand(Control);
		}
		protected internal virtual bool IsFirstLineIndentLessDefaultTabSize() {
			DocumentModel documentModel = DocumentModel;
			Paragraph startParagraph = ActivePieceTable.Paragraphs[StartIndex];
			return startParagraph.FirstLineIndent < documentModel.DocumentProperties.DefaultTabWidth;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
}

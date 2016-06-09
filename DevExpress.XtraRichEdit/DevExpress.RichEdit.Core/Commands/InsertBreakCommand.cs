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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Tables.Native;
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertBreakCommand
	public class InsertBreakCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public InsertBreakCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertBreak; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertBreakDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertBreak; } }
		public override string ImageName { get { return "InsertPageBreak"; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Sections);
			ApplyDocumentProtectionToSelectedSections(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
	#region InsertSectionBreakCoreCommand (abstract class)
	public abstract class InsertSectionBreakCoreCommand : InsertParagraphCoreCommand {
		protected InsertSectionBreakCoreCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract SectionStartType StartType { get; }
		protected internal override void ModifyModel() {
			SplitTableCommand splitTable = new SplitTableCommand(Control);
			if (splitTable.CanExecute())
				splitTable.PerformTableSplitBySelectionStart();
			DocumentLogPosition pos = DocumentModel.Selection.End;
			Paragraph paragraph = ActivePieceTable.FindParagraph(pos);
			if (paragraph == null)
				return;
			TableCell cell = paragraph.GetCell();
			if (cell != null) 
				return;
			DocumentModel.InsertSection(pos);
			SectionIndex sectionIndex = DocumentModel.FindSectionIndex(pos + 1);
			if (sectionIndex >= new SectionIndex(0))
				DocumentModel.Sections[sectionIndex].GeneralSettings.StartType = StartType;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled)
				state.Enabled = ActivePieceTable.IsMain && !IsSelectionEndInTableCell();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Sections, state.Enabled);
			ApplyDocumentProtectionToSelectedSections(state);
		}
	}
	#endregion
}

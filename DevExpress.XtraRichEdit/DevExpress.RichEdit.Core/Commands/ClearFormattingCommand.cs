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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.XtraRichEdit.Commands {
	public class ClearFormattingCommand : RichEditSelectionCommand {
		public ClearFormattingCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ClearFormattingCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ClearFormatting; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ClearFormattingCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ClearFormatting; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ClearFormattingCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ClearFormattingDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ClearFormattingCommandImageName")]
#endif
		public override string ImageName { get { return "ClearFormatting"; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return false;
		}
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.CharacterFormatting);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected internal override void PerformModifyModel() {
			ResetCharacterFormatting();
			ResetParagraphStyleAndProperties();
		}
		protected internal override void ChangeSelection(Selection selection) {
		}
		protected internal virtual void ResetParagraphStyleAndProperties() {
			ResetParagraphFormattingCommand resetToDefault = new ResetParagraphFormattingCommand(Control);
			resetToDefault.ModifyDocumentModelCore(resetToDefault.CreateDefaultCommandUIState()); 
		}
		protected internal virtual void ResetCharacterFormatting() {
			ResetCharacterFormattingCommand reset = new ResetCharacterFormattingCommand(Control);
			reset.ModifyDocumentModelCore(reset.CreateDefaultCommandUIState());
		}
	}
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ResetCharacterFormatting
	public class ResetCharacterFormattingCommand : ChangeCharacterFormattingCommandBase<bool> {
		public ResetCharacterFormattingCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.None; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ResetCharacterFormatting; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ResetCharacterFormattingDescription; } }
		public override string ImageName { get { return "ResetCharacterFormatting"; } }
		#endregion
		protected internal override RunPropertyModifier<bool> CreateModifier(ICommandUIState state) {
			return new RunClearCharacterFormattingModifier(true);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.CharacterFormatting);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region ResetParagraphFormattingCommand
	public class ResetParagraphFormattingCommand : ChangeParagraphStyleCommand {
		public ResetParagraphFormattingCommand(IRichEditControl control)
			: base(control, control.InnerDocumentServer.DocumentModel.ParagraphStyles.DefaultItem) {
		}
		protected internal override bool ValidateSelectionInterval(SelectionItem item) {
			bool isEmptySelection = DocumentModel.Selection.Items.Count == 1 && item.Length == 0;
			if (isEmptySelection)
				return true;
			DocumentModelPosition end = CalculateEndPosition(item, true);
			DocumentModelPosition start = CalculateStartPosition(item, true);
			bool isWholeParagraphSelected = item.NormalizedStart == start.LogPosition && item.NormalizedEnd == end.LogPosition;
			bool isSelectionCrossParagraphs = !(start.ParagraphIndex == end.ParagraphIndex - 1);
			return isWholeParagraphSelected || isSelectionCrossParagraphs;
		}
	}
	#endregion
}

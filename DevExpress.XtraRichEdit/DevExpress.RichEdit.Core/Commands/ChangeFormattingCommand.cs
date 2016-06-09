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
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ChangeFormattingCommand
	public class ChangeFormattingCommand : SelectionBasedPropertyChangeCommandBase {
		IXtraRichEditFormatting formatting;
		public ChangeFormattingCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool ValidateUIState(ICommandUIState state) {
			IValueBasedCommandUIState<IXtraRichEditFormatting> valueBasedState = state as IValueBasedCommandUIState<IXtraRichEditFormatting>;
			if (valueBasedState == null || valueBasedState.Value == null)
				return false;
			return true;
		}
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			IValueBasedCommandUIState<IXtraRichEditFormatting> valueBasedState = state as IValueBasedCommandUIState<IXtraRichEditFormatting>;
			if (valueBasedState == null || valueBasedState.Value == null)
				return DocumentModelChangeActions.None;
			valueBasedState.Value.Apply(InnerControl.DocumentModel, start, end);
			return DocumentModelChangeActions.ResetCaretInputPositionFormatting;
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeStyle; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeStyleDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeFontStyle; } }
		public override string ImageName { get { return "ChangeFontStyle"; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			IValueBasedCommandUIState<IXtraRichEditFormatting> valueBasedState = state as IValueBasedCommandUIState<IXtraRichEditFormatting>;
			state.Enabled = IsContentEditable && CanEditSelection();
			state.Visible = true;
			if (valueBasedState != null) {
				try {
					this.formatting = valueBasedState.Value;
				}
				catch { 
					this.formatting = null;
				}
				valueBasedState.Value = GetCurrentPropertyValue();
			}
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
		protected internal virtual IXtraRichEditFormatting GetCurrentPropertyValue() {
			DocumentModelPosition start = CalculateStartPosition(DocumentModel.Selection.ActiveSelection, false);
			return GetInputPositionPropertyValue(start);
		}
		protected internal override void ModifyDocumentModelCore(ICommandUIState state) {
			IValueBasedCommandUIState<IXtraRichEditFormatting> valueBasedState = state as IValueBasedCommandUIState<IXtraRichEditFormatting>;
			if (valueBasedState != null)
				this.formatting = valueBasedState.Value;
			base.ModifyDocumentModelCore(state);
			DocumentServer.OnUpdateUI();
		}
		protected internal override DocumentModelPosition CalculateStartPosition(SelectionItem selection, bool allowSelectionExpaning) {
			if (formatting != null && allowSelectionExpaning)
				allowSelectionExpaning &= formatting.AllowSelectionExpanding;
			return base.CalculateStartPosition(selection, allowSelectionExpaning);
		}
		protected internal override DocumentModelPosition CalculateEndPosition(SelectionItem selection, bool allowSelectionExpaning) {
			if (formatting != null && allowSelectionExpaning)
				allowSelectionExpaning &= formatting.AllowSelectionExpanding;
			return base.CalculateEndPosition(selection, allowSelectionExpaning);
		}
		protected virtual IXtraRichEditFormatting GetInputPositionPropertyValue(DocumentModelPosition pos) {
			TextRunBase run = ActivePieceTable.Runs[pos.RunIndex];
			if (run.CharacterStyleIndex != CharacterStyleCollection.EmptyCharacterStyleIndex) {
				if (!run.CharacterStyle.HasLinkedStyle)
					return new CharacterStyleFormatting(run.CharacterStyle.Id);
				else {
					return new ParagraphStyleFormatting(run.CharacterStyle.LinkedStyle.Id);
				}
			}
			Paragraph paragraph = run.Paragraph;
			if (paragraph.ParagraphStyle.StyleName != ParagraphStyleCollection.DefaultParagraphStyleName)
				return new ParagraphStyleFormatting(paragraph.ParagraphStyle.Id);
			ParagraphStyle defaultStyle = Control.InnerControl.DocumentModel.ParagraphStyles.DefaultItem;
			return new ParagraphStyleFormatting(defaultStyle.Id);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<IXtraRichEditFormatting> result = new DefaultValueBasedCommandUIState<IXtraRichEditFormatting>();
			return result;
		}
		public override void UpdateUIState(ICommandUIState state) {
			base.UpdateUIState(state);
			UpdateUIStateViaService(state);
		}
	}
	#endregion
}

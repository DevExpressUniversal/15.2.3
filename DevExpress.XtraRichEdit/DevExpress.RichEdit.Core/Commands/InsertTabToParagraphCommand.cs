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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils.Commands;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	public class InsertTabToParagraphCommand : SelectionBasedPropertyChangeCommandBase {
		readonly TabInfo tab;
		public InsertTabToParagraphCommand(IRichEditControl control, TabInfo tab)
			: base(control) {
			Guard.ArgumentNotNull(tab, "tab");
			this.tab = tab;
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTabToParagraph; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTabToParagraphDescription; } }
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			ParagraphCollection paragraphs = ActivePieceTable.Paragraphs;			
			ParagraphIndex from = start.ParagraphIndex;
			ParagraphIndex to = end.ParagraphIndex;
			for (ParagraphIndex i = from; i <= to; i++) {
				Paragraph paragraph = paragraphs[i];
				TabFormattingInfo tabs = paragraph.Tabs.GetTabs();
				tabs.Add(tab);
				paragraph.Tabs.SetTabs(tabs);
			}
			return DocumentModelChangeActions.None;
		}
		protected internal override DocumentModelPosition CalculateStartPosition(SelectionItem item, bool allowSelectionExpanding) {
			return DocumentModelPosition.FromParagraphStart(item.PieceTable, item.GetStartParagraphIndex());
		}
		protected internal override DocumentModelPosition CalculateEndPosition(SelectionItem item, bool allowSelectionExpanding) {
			return DocumentModelPosition.FromParagraphEnd(item.PieceTable, item.GetEndParagraphIndex());
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.ParagraphTabs);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	public class DeleteTabAtParagraphCommand : RichEditCommand {
		TabInfo tab;
		public DeleteTabAtParagraphCommand(IRichEditControl control, TabInfo tab)
			: base(control) {
			Guard.ArgumentNotNull(tab, "tab");
			this.tab = tab;
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTabToParagraph; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTabToParagraphDescription; } }
		public override void ForceExecute(ICommandUIState state) {
			Selection selection = DocumentModel.Selection;
			ParagraphList paragraphs = selection.GetSelectedParagraphs();
			foreach(Paragraph paragraph in paragraphs) {
				TabFormattingInfo tabs = paragraph.GetTabs();
				if (tabs.Contains(tab)) {
					TabFormattingInfo paragraphTabs = paragraph.Tabs.GetTabs();
					paragraphTabs.Remove(tab);
					TabFormattingInfo styleTabs = paragraph.ParagraphStyle.GetTabs();
					if (styleTabs.Contains(tab)) {
						tab = new TabInfo(tab.Position, tab.Alignment, tab.Leader, true);
						paragraphTabs.Add(tab);
					}
					paragraph.Tabs.SetTabs(paragraphTabs);
				}
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.ParagraphTabs);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
}

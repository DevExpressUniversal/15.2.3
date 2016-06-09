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
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ChangeParagraphStyleCommandBase (abstract class)
	public abstract class ChangeParagraphStyleCommandBase : SelectionBasedPropertyChangeCommandBase {
		protected ChangeParagraphStyleCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract int CalculateParagraphStyleIndex(Paragraph paragraph);
		protected internal override DocumentModelPosition CalculateStartPosition(SelectionItem selection, bool allowSelectionExpaning) {
			DocumentModelPosition result = selection.Interval.NormalizedStart;
			ParagraphsDocumentModelIterator iterator = new ParagraphsDocumentModelIterator(ActivePieceTable);
			if (iterator.IsNewElement(result))
				return result;
			else
				return iterator.MoveBack(result);
		}
		protected internal override DocumentModelPosition CalculateEndPosition(SelectionItem selection, bool allowSelectionExpaning) {
			DocumentModelPosition result = selection.Interval.NormalizedEnd;
			if (result.LogPosition > ActivePieceTable.DocumentEndLogPosition) {
				result.ParagraphIndex = ActivePieceTable.Paragraphs.Last.Index + 1;
				return result;
			}
			ParagraphsDocumentModelIterator iterator = new ParagraphsDocumentModelIterator(ActivePieceTable);
			if (iterator.IsNewElement(result))
				return result;
			else
				return iterator.MoveForward(result);
		}
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			ParagraphIndex startIndex = start.ParagraphIndex;
			ParagraphIndex endIndex = end.ParagraphIndex;
			if (startIndex==endIndex) 
				ChangeParagraphProperty(ActivePieceTable.Paragraphs[startIndex]);
			else 
				for (ParagraphIndex index = startIndex; index < endIndex; index++)
					ChangeParagraphProperty(ActivePieceTable.Paragraphs[index]);
			return DocumentModelChangeActions.None;
		}
		protected internal virtual void ChangeParagraphProperty(Paragraph paragraph) {
			paragraph.ParagraphProperties.ResetAllUse();
			paragraph.ParagraphStyleIndex = CalculateParagraphStyleIndex(paragraph);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.ParagraphStyle);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
	#region ChangeParagraphStyleCommand
	public class ChangeParagraphStyleCommand : ChangeParagraphStyleCommandBase {
		readonly ParagraphStyle paragraphStyle;
		public ChangeParagraphStyleCommand(IRichEditControl control, ParagraphStyle paragraphStyle)
			: base(control) {
			this.paragraphStyle = paragraphStyle;
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeParagraphStyle; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeParagraphStyleDescription; } }
		#endregion
		protected internal override int CalculateParagraphStyleIndex(Paragraph paragraph) {
			if (paragraphStyle == null)
				return DocumentModel.ParagraphStyles.DefaultItemIndex;
			else
				return DocumentModel.ParagraphStyles.IndexOf(paragraphStyle);
		}
	}
	#endregion
}

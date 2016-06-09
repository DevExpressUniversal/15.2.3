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
using System.ComponentModel;
using System.Text;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	#region DecrementNumerationFromParagraphCommand
	public class DecrementNumerationFromParagraphCommand : NumberingListCommandBase {
		public DecrementNumerationFromParagraphCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementNumerationFromParagraphCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.DecrementNumerationFromParagraph; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementNumerationFromParagraphCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DecrementNumerationFromParagraph; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementNumerationFromParagraphCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DecrementNumerationFromParagraphDescription; } }
		#endregion
		protected internal override void ModifyParagraphsCore(ParagraphIndex startParagraphIndex, ParagraphIndex endParagraphIndex) {
			for (ParagraphIndex i = startParagraphIndex; i <= endParagraphIndex; i++) {
				Paragraph paragraph = ActivePieceTable.Paragraphs[i];
				int listLevelIndex = paragraph.GetListLevelIndex();
				if (paragraph.IsInList() && listLevelIndex > 0) {
					NumberingListIndex numberingListIndex = paragraph.GetNumberingListIndex();
					ActivePieceTable.RemoveNumberingFromParagraph(paragraph);
					listLevelIndex = listLevelIndex - 1;
					paragraph.ParagraphProperties.ResetUse(ParagraphFormattingOptions.Mask.UseFirstLineIndent | ParagraphFormattingOptions.Mask.UseLeftIndent);
					ActivePieceTable.AddNumberingListToParagraph(paragraph, numberingListIndex, listLevelIndex);
				}
			}
		}
		protected internal override void ChangeSelection(Selection selection) {
		}
	}
	#endregion
	#region DecrementNumerationParagraphIndentCommand
	public class DecrementNumerationParagraphIndentCommand : NumerationParagraphIndentCommandBase {
		public DecrementNumerationParagraphIndentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementNumerationParagraphIndentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DecrementParagraphLeftIndent; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementNumerationParagraphIndentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DecrementParagraphLeftIndentDescription; } }
		#endregion
		protected override void AssignNewIndentCore(AbstractNumberingList abstractNumberingList, int currentNumberingOrBulletPosition) {
			int nearLeftDefaultTab = GetNearLeftDefaultTab(currentNumberingOrBulletPosition);
			int nearLeftTab = GetNearLeftTab(currentNumberingOrBulletPosition);
			if (nearLeftDefaultTab > nearLeftTab || nearLeftTab == currentNumberingOrBulletPosition)
				AssignNumberingListLeftIndentModifier(abstractNumberingList, nearLeftDefaultTab);
			else
				AssignNumberingListLeftIndentModifier(abstractNumberingList, nearLeftTab);
			}
		protected internal override ParagraphPropertyModifier<int> CreateModifier(ICommandUIState state) {
			Exceptions.ThrowInternalException();
			return null;
		}
	}
	#endregion
}

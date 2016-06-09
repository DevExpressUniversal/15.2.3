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
	#region IncrementNumerationFromParagraphCommand
	public class IncrementNumerationFromParagraphCommand : NumberingListCommandBase {
		public IncrementNumerationFromParagraphCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementNumerationFromParagraphCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.IncrementNumerationFromParagraph; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementNumerationFromParagraphCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementNumerationFromParagraph; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementNumerationFromParagraphCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementNumerationFromParagraphDescription; } }
		#endregion
		protected internal override void ModifyParagraphsCore(ParagraphIndex startParagraphIndex, ParagraphIndex endParagraphIndex) {
			for (ParagraphIndex i = startParagraphIndex; i <= endParagraphIndex; i++) {
				Paragraph paragraph = ActivePieceTable.Paragraphs[i];
				int listLevelIndex = paragraph.GetListLevelIndex();
				if (paragraph.IsInList() && listLevelIndex < 8) {
					NumberingListIndex numberingListIndex = paragraph.GetNumberingListIndex();
					ActivePieceTable.RemoveNumberingFromParagraph(paragraph);
					IncrementNumerationFromParagraph(paragraph, listLevelIndex, numberingListIndex);
				}
			}
		}
		protected internal virtual void IncrementNumerationFromParagraph(Paragraph paragraph, int listLevelIndex, NumberingListIndex numberingListIndex) {
			listLevelIndex = listLevelIndex + 1;
			paragraph.ParagraphProperties.ResetUse(ParagraphFormattingOptions.Mask.UseFirstLineIndent | ParagraphFormattingOptions.Mask.UseLeftIndent);
			ActivePieceTable.AddNumberingListToParagraph(paragraph, numberingListIndex, listLevelIndex);
		}
		protected internal override void ChangeSelection(Selection selection) {
		}
	}
	#endregion
	#region NumerationParagraphIndentCommandBase (abstract class)
	public abstract class NumerationParagraphIndentCommandBase : ChangeParagraphIndentCommandBase<int> {
		protected NumerationParagraphIndentCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override void ModifyDocumentModelCore(ICommandUIState state) {
			FillTabsList();
			Paragraph paragraph = ActivePieceTable.Paragraphs[StartParagraphIndex];
			NumberingListIndex numberingListIndex = paragraph.GetNumberingListIndex();
			if (numberingListIndex < NumberingListIndex.MinValue)
				return;
			AbstractNumberingList abstractNumberingList = DocumentModel.NumberingLists[numberingListIndex].AbstractNumberingList;
			ListLevel firstLevel = abstractNumberingList.Levels[0];			
			int currentBulletOrNumberingPosition = CalculateCurrentBulletOrNumberingPosition(firstLevel.LeftIndent, firstLevel.FirstLineIndent, firstLevel.FirstLineIndentType);
			AssignNewIndentCore(abstractNumberingList, currentBulletOrNumberingPosition);
		}
		protected abstract void AssignNewIndentCore(AbstractNumberingList abstractNumberingList, int currentNumberingOrBulletPosition);
		protected virtual int CalculateCurrentBulletOrNumberingPosition(int leftIndent, int firstLineIndent, ParagraphFirstLineIndent firstLineIndentType) {
			if (firstLineIndentType == ParagraphFirstLineIndent.Hanging)
				return leftIndent - firstLineIndent;
			else
				return leftIndent;
		}
		protected virtual int CalculateLeftIndentDelta(int taregetNumerationOrBulletPosition, int currentLeftIndent, int firstLineIndent, ParagraphFirstLineIndent firstLineIndentType) {
			return taregetNumerationOrBulletPosition - CalculateCurrentBulletOrNumberingPosition(currentLeftIndent, firstLineIndent, firstLineIndentType);
		}
		protected virtual void AssignNumberingListLeftIndentModifier(AbstractNumberingList abstractNumberingList, int targetNumberingOrBulletPosition) {
			ListLevelCollection<ListLevel> levels = abstractNumberingList.Levels;
			ListLevel firstLevel = levels[0];
			int delta = CalculateLeftIndentDelta(targetNumberingOrBulletPosition, firstLevel.LeftIndent, firstLevel.FirstLineIndent, firstLevel.FirstLineIndentType);
			int levelCount = levels.Count;
			for (int i = 0; i < levelCount; i++) {
				ListLevel level = levels[i];
				int newLeftIndent = level.LeftIndent + delta;
				if (newLeftIndent >= 0) {
					if (level.FirstLineIndentType == ParagraphFirstLineIndent.Hanging) {
						int firstLineLeftIndent = newLeftIndent - level.FirstLineIndent;
						if (firstLineLeftIndent < 0)
							newLeftIndent -= firstLineLeftIndent;
					}
					if (i == 0 && level.LeftIndent == newLeftIndent)
						break;
					level.LeftIndent = newLeftIndent;
				}
				else if (i == 0)
					break;
			}
		}
	}
	#endregion
	#region IncrementNumerationParagraphIndentCommand
	public class IncrementNumerationParagraphIndentCommand : NumerationParagraphIndentCommandBase {		
		public IncrementNumerationParagraphIndentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementNumerationParagraphIndentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementParagraphLeftIndent; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementNumerationParagraphIndentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementParagraphLeftIndentDescription; } }
		#endregion
		protected override void AssignNewIndentCore(AbstractNumberingList abstractNumberingList, int currentNumberingOrBulletPosition) {
			int nearestRightDefaultTab = GetNearRightDefaultTab(currentNumberingOrBulletPosition);
			int nearestRightTab = GetNearRightTab(currentNumberingOrBulletPosition);
			if (nearestRightDefaultTab < nearestRightTab || nearestRightTab == currentNumberingOrBulletPosition)
				AssignNumberingListLeftIndentModifier(abstractNumberingList, nearestRightDefaultTab);
			else
				AssignNumberingListLeftIndentModifier(abstractNumberingList, nearestRightTab);
		}
		protected internal override ParagraphPropertyModifier<int> CreateModifier(ICommandUIState state) {
			Exceptions.ThrowInternalException();
			return null;
		}
		protected internal override void FillTabsList() {
			Paragraph paragraph = ActivePieceTable.Paragraphs[StartParagraphIndex];
			if (paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Hanging)
				TabsList.Add(paragraph.LeftIndent + paragraph.FirstLineIndent);
			base.FillTabsList();
		}
	}
	#endregion
}

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

using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Native {
	public class SnapNumberingListIndexCalculator : NumberingListIndexCalculator {
		public SnapNumberingListIndexCalculator(DocumentModel model, NumberingType numberingListType)
			:base(model, numberingListType) {
		}
		public override NumberingListIndex GetListIndex(ParagraphIndex start, ParagraphIndex end) {
			NumberingListIndex result = base.GetListIndex(start, end);
			if (result != NumberingListIndex.ListIndexNotSetted)
				return result;
			return GetListIndexCore(start);
		}
		NumberingListIndex GetListIndexCore(ParagraphIndex startParagraphIndex) {
			Field field = FieldsHelper.FindFieldByParagraph(ActivePieceTable.Paragraphs[startParagraphIndex]);
			if (field == null)
				return NumberingListIndex.ListIndexNotSetted;
			Field parentField = field.Parent;
			if (parentField == null)
				return NumberingListIndex.ListIndexNotSetted;
			RunIndex startRunIndex = parentField.Result.Start;
			RunIndex endRunIndex = field.Code.Start;
			return CalculateNumberingListIndexByParentParagraphs(startRunIndex, endRunIndex);
		}
		NumberingListIndex CalculateNumberingListIndexByParentParagraphs(RunIndex start, RunIndex end) {
			for (RunIndex i = start; i <= end; i++) {
				Paragraph paragraph = ActivePieceTable.Runs[i].Paragraph;
				if (paragraph.IsInList()) {
					NumberingListIndex nearParagraphListIndex = paragraph.GetNumberingListIndex();
					NumberingType paragraphListType = NumberingListHelper.GetListType(DocumentModel.NumberingLists[nearParagraphListIndex].AbstractNumberingList);
					if (paragraphListType == NumberingListType) {
						ContinueList = true;
						return nearParagraphListIndex;
					}
				}
				i = paragraph.LastRunIndex;
			}
			return NumberingListIndex.ListIndexNotSetted;
		}
	}
}

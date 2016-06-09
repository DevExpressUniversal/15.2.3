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

using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Utils {
	public class NumberingListIndexCalculator {
		readonly DocumentModel model;
		readonly NumberingType numberingListType;
		public NumberingListIndexCalculator(DocumentModel model, NumberingType numberingListType) {
			Guard.ArgumentNotNull(model, "model");
			this.model = model;
			this.numberingListType = numberingListType;
		}
		protected internal DocumentModel DocumentModel { get { return model; } }
		protected internal PieceTable ActivePieceTable { get { return model.ActivePieceTable; } }
		public NumberingType NumberingListType { get { return numberingListType; } }
		public bool ContinueList { get; protected set; }
		public int NestingLevel { get; protected set; }
		public virtual NumberingListIndex GetListIndex(ParagraphIndex start, ParagraphIndex end) {
			if (start > new ParagraphIndex(0) && ActivePieceTable.Paragraphs[start - 1].IsInList()) {
				NumberingListIndex result = GetListIndexCore(start - 1);
				if (result >= NumberingListIndex.MinValue)
					return result;
			}
			if (end < new ParagraphIndex(ActivePieceTable.Paragraphs.Count - 1) && ActivePieceTable.Paragraphs[end + 1].IsInList()) {
				NumberingListIndex result = GetListIndexCore(end + 1);
				if (result >= NumberingListIndex.MinValue)
					return result;
			}
			return NumberingListIndex.ListIndexNotSetted;
		}
		NumberingListIndex GetListIndexCore(ParagraphIndex paragraphIndex) {
			NumberingListIndex nearParagraphListIndex = ActivePieceTable.Paragraphs[paragraphIndex].GetNumberingListIndex();
			NumberingType paragraphListType = NumberingListHelper.GetListType(DocumentModel.NumberingLists[nearParagraphListIndex].AbstractNumberingList);
			if (paragraphListType == NumberingListType) {
				ContinueList = true;
				return nearParagraphListIndex;
			}
			return NumberingListIndex.ListIndexNotSetted;
		}
		public virtual NumberingListIndex CreateNewList(AbstractNumberingList source) {
			AbstractNumberingListIndex abstractNumberingListIndex = CreateNewAbstractList(source);
			NumberingList newList = new NumberingList(DocumentModel, abstractNumberingListIndex);
			DocumentModel.AddNumberingListUsingHistory(newList);
			return new NumberingListIndex(DocumentModel.NumberingLists.Count - 1);
		}
		internal AbstractNumberingListIndex CreateNewAbstractList(AbstractNumberingList source) {
			AbstractNumberingList newAbstractNumberingList = new AbstractNumberingList(DocumentModel);
			newAbstractNumberingList.CopyFrom(source); 
			DocumentModel.AddAbstractNumberingListUsingHistory(newAbstractNumberingList);
			newAbstractNumberingList.SetId(DocumentModel.AbstractNumberingListIdProvider.GetNextId()); 
			return new AbstractNumberingListIndex(DocumentModel.AbstractNumberingLists.Count - 1);
		}
	}
}

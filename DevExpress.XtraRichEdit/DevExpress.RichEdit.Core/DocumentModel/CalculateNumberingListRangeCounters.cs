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
using System.Diagnostics;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Utils {
	#region NumberingListCountersCalculator
	public class NumberingListCountersCalculator {
		#region Fields
		int[] counters;
		readonly AbstractNumberingList list;
		SortedList<NumberingListIndex> usedListIndex;
		#endregion
		public NumberingListCountersCalculator(AbstractNumberingList list) {
			this.list = list;
		}
		#region Properties
		AbstractNumberingList List { get { return list; } }
		int[] Counters { get { return counters; } set { counters = value; } }
		int CountersCount { get { return List.Levels.Count; } }
		#endregion
		public void BeginCalculateCounters() {
			CreateListLevelCounters();
			this.usedListIndex = new SortedList<NumberingListIndex>();
		}
		protected internal virtual int[] CalculateCounters(Paragraph paragraph) {
			BeginCalculateCounters();
			try {
				AbstractNumberingList abstractNumberingList = paragraph.GetAbstractNumberingList();
				Debug.Assert(list != null);
				foreach (Paragraph currentParagraph in paragraph.PieceTable.Paragraphs.Range(ParagraphIndex.MinValue, paragraph.Index)) {
					if (ShouldAdvanceListLevelCounters(currentParagraph, abstractNumberingList))
						AdvanceListLevelCounters(currentParagraph, currentParagraph.GetListLevelIndex());
				}
				return GetActualRangeCounters(paragraph.GetListLevelIndex());
			}
			finally {
				EndCalculateCounters();
			}
		}
		public int[] CalculateNextCounters(Paragraph currentParagraph) {
			AbstractNumberingList abstractNumberingList = currentParagraph.GetAbstractNumberingList();
			if (ShouldAdvanceListLevelCounters(currentParagraph, abstractNumberingList))
				AdvanceListLevelCounters(currentParagraph, currentParagraph.GetListLevelIndex());
			return GetActualRangeCounters(currentParagraph.GetListLevelIndex());
		}
		public void EndCalculateCounters() {
		}
		protected internal virtual bool ShouldAdvanceListLevelCounters(Paragraph paragraph, AbstractNumberingList abstractNumberingList) {
			return paragraph.GetAbstractNumberingList() == abstractNumberingList;
		}
		public int[] GetActualRangeCounters(int listLevelIndex) {
			int[] rangeCounters = new int[listLevelIndex + 1];
			for (int i = 0; i <= listLevelIndex; i++)
				rangeCounters[i] = Counters[i];
			return rangeCounters;
		}
		public void AdvanceListLevelCounters(Paragraph paragraph, int listLevelIndex) {
			NumberingListIndex numberingListIndex = paragraph.GetNumberingListIndex();
			NumberingList numberingList = paragraph.DocumentModel.NumberingLists[numberingListIndex];
			IOverrideListLevel level = numberingList.Levels[listLevelIndex];
			if (level.OverrideStart && !usedListIndex.Contains(numberingListIndex)) {
				usedListIndex.Add(numberingListIndex);
				Counters[listLevelIndex] = level.NewStart;
			}
			else
				Counters[listLevelIndex]++;
			AdvanceSkippedLevelCounters(listLevelIndex);
			RestartNextLevelCounters(listLevelIndex);
		}
		void AdvanceSkippedLevelCounters(int listLevelIndex) {
			ListLevelCollection<ListLevel> levels = List.Levels;
			for (int i = 0; i < listLevelIndex; i++) {
				if (Counters[i] == levels[i].ListLevelProperties.Start - 1)
					Counters[i]++;
			}
		}
		void RestartNextLevelCounters(int listLevelIndex) {
			ListLevelCollection<ListLevel> levels = List.Levels;
			bool[] restartedLevels = new bool[CountersCount];
			restartedLevels[listLevelIndex] = true;
			for (int i = listLevelIndex + 1; i < CountersCount; i++) {
				ListLevelProperties listLevelProperties = levels[i].ListLevelProperties;
				if (!listLevelProperties.SuppressRestart) {
					int restartLevel = i - listLevelProperties.RelativeRestartLevel - 1;
					if (restartLevel >= 0 && restartLevel < CountersCount && restartedLevels[restartLevel]) {
						Counters[i] = listLevelProperties.Start - 1;
						restartedLevels[i] = true;
					}
				}
			}
		}
		protected internal virtual void CreateListLevelCounters() {
			Counters = new int[CountersCount];
			ListLevelCollection<ListLevel> levels = List.Levels;
			for (int i = 0; i < CountersCount; i++) {
				Counters[i] = levels[i].ListLevelProperties.Start - 1;
			}
		}
	}
	#endregion
	#region PieceTableNumberingListCountersManager
	public class PieceTableNumberingListCountersManager {
		Dictionary<AbstractNumberingList, NumberingListCountersCalculator> calculators;
		public void BeginCalculateCounters() {
			this.calculators = new Dictionary<AbstractNumberingList, NumberingListCountersCalculator>();
		}
		public int[] CalculateNextCounters(Paragraph currentParagraph) {
			AbstractNumberingList abstractNumberingList = currentParagraph.GetAbstractNumberingList();
			if (abstractNumberingList == null)
				return null;
			NumberingListCountersCalculator calculator;
			if (!calculators.TryGetValue(abstractNumberingList, out calculator)) {
				calculator = new NumberingListCountersCalculator(abstractNumberingList);
				calculator.BeginCalculateCounters();
				calculators.Add(abstractNumberingList, calculator);
			}
			return calculator.CalculateNextCounters(currentParagraph);
		}
		public void EndCalculateCounters() {
			foreach (AbstractNumberingList key in calculators.Keys)
				calculators[key].EndCalculateCounters();
			this.calculators.Clear();
		}
	}
	#endregion
}

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
using System.Linq;
using System.Text;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid.Data;
using System.Windows;
namespace DevExpress.Xpf.PivotGrid.Printing {
	class SinglePagePrintRoot : PivotPrintRoot {
		public SinglePagePrintRoot(PivotGridControl pivotGrid, PrintSizeArgs sizes)
			: base(pivotGrid, sizes) { }
		public static
#if !DEBUGTEST
			readonly
#endif
 int optimalCellCount = 3000;
		bool MergeRowFieldValues {
			get { return PivotGrid.MergeRowFieldValues; }
		}
		protected override void CreatePagesCore() {
			List<PivotPrintPageValueItemsList> Items = CreateItems();
			for(int i = 0; i < Items.Count; i++)
				CreatePage(Items[i]);
		}
		void CreatePage(PivotPrintPageValueItemsList rowItems) {
			PivotPrintPage page = CreatePrintPage();
			page.RowItems = rowItems;
			page.PageBreakAfter = false;
			Add(page);
			if(ChildNodes.Count == 1) {
				SetHeadersBorderThickness(page, ColumnFieldValuesSize.Width);
			} else {
				page.ShowColumnFieldValues = Visibility.Collapsed;
				page.ShowColumnAreaHeadersBottomBorder = false;
				page.ShowRowAreaHeadersTopBorder = false;
			}
		}
		List<PivotPrintPageValueItemsList> CreateItems() {
			List<PivotPrintPageValueItemsList> rows = new List<PivotPrintPageValueItemsList>();
			List<PivotFieldValueItem> startItems = CreateItemsLists();
			for(int i = 0; i < startItems.Count - 1; i++)
				rows.Add(CreateRowItemsListByStartEndIndexes(startItems[i].Index, startItems[i + 1].Index, startItems[i + 1].MinLastLevelIndex - 1));
			rows.Add(CreateRowItemsListByStartEndIndexes(startItems[startItems.Count - 1].Index, VisualItems.GetItemCount(false), VisualItems.GetLastLevelItemCount(false) - 1));
			return rows;
		}
		List<PivotFieldValueItem> CreateItemsLists() {
			int count = VisualItems.GetItemCount(false);
			List<PivotFieldValueItem> items = new List<PivotFieldValueItem>();
			for(int i = 0; i < count; i++) {
				PivotFieldValueItem valueItem = VisualItems.GetItem(false, i);
				if(valueItem.Level == 0)
					items.Add(valueItem);
			}
			items.Sort(PivotFieldValueItemItemsComparer.Default);
			int optimalRowCount = optimalCellCount / VisualItems.GetLastLevelItemCount(true);
			if(optimalRowCount <= 0)
				optimalRowCount = 1;
			List<PivotFieldValueItem> startItems = new List<PivotFieldValueItem>();
			int levelCount = 0;
			int start = 0;
			for(int j = 0; j < items.Count; j++) {
				PivotFieldValueItem item = items[j];
				int diff = item.MaxLastLevelIndex - item.MinLastLevelIndex + 1;
				if(diff > optimalRowCount) {
					if(start != j)
						startItems.Add(items[start]);
					startItems.Add(item);
					levelCount = 0;
					start = j + 1;
					continue;
				}
				levelCount += diff;
				if(levelCount > optimalRowCount) {
					levelCount = diff;
					startItems.Add(items[start]);
					start = j;
				}
			}
			if(levelCount != 0)
				startItems.Add(items[start]);
			return startItems;
		}
		PivotPrintPageValueItemsList CreateRowItemsListByStartEndIndexes(int startIndex, int endIndex, int endLevel) {
			PivotPrintPageValueItemsList rowItems = new PivotPrintPageValueItemsList();
			for(int k = startIndex; k < endIndex; k++)
				if(MergeRowFieldValues) {
					rowItems.Add(new PrintFieldValueItem(VisualItems.GetItem(false, k), VisualItems));
				} else {
					PivotFieldValueItem item2 = VisualItems.GetItem(false, k);
					for(int l = item2.MinLastLevelIndex; l <= item2.MaxLastLevelIndex; l++)
						rowItems.Add(new PrintFieldValueItem(item2, VisualItems, l));
				}
			rowItems.StartIndex = rowItems[0].MinLevel;
			rowItems.EndIndex = endLevel;
			rowItems.Sort(PivotFieldValueItemItemsComparer.Default);
			return rowItems;
		}
	}
}

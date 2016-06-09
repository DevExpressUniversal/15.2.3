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
namespace DevExpress.Xpf.PivotGrid.Printing {
	class MultiplePagePrintRoot : PivotPrintRoot {
		public MultiplePagePrintRoot(PivotGridControl pivotGrid, PrintSizeArgs sizes)
			: base(pivotGrid, sizes) { }
		protected override void CreatePagesCore() {
			CheckPageSize();
			List<PivotPrintPageValueItemsList> rowPages = CreateFieldValuePages(false),
				columnPages = CreateFieldValuePages(true);
			CreatePagedDocumentCore(rowPages, columnPages);
		}
		void CreatePagedDocumentCore(List<PivotPrintPageValueItemsList> rowPages, List<PivotPrintPageValueItemsList> columnPages) {
			for(int i = 0; i < rowPages.Count; i++) {
				for(int j = 0; j < columnPages.Count; j++) {
					PivotPrintPage printPage = CreatePrintPage();
					printPage.ColumnItems = columnPages[j];
					printPage.RowItems = rowPages[i];
					bool isLastPage = i == rowPages.Count - 1 && j == columnPages.Count - 1;
					if(PivotGrid.PrintInsertPageBreaks && !isLastPage) {
						printPage.Height = PrintHeight;
						if(i == 0 && j == 0 && IsReportHeaderOnFirstPage)
							printPage.Height -= ReportHeaderSize.Height;
					}
					printPage.Width = UsablePageSize.Width;
					printPage.PageBreakAfter = isLastPage ? false : PivotGrid.PrintInsertPageBreaks;
					Add(printPage);
					SetHeadersBorderThickness(printPage,
							  VisualItems.GetWidthDifference(columnPages[j].StartIndex, columnPages[j].EndIndex + 1, true));
				}
			}
		}
		List<PivotPrintPageValueItemsList> CreateFieldValuePages(bool isColumn) {
			List<PivotPrintPageValueItemsList> pages = new List<PivotPrintPageValueItemsList>();
			double availablePrintSize = isColumn ? UsablePageSize.Width : PrintHeight - 1;
			int curItem = 0, count = VisualItems.GetItemCount(isColumn);
			while(curItem < count) {
				double pageAvailablePrintSize = availablePrintSize;
				PivotPrintPageValueItemsList pageItems = new PivotPrintPageValueItemsList();
				pages.Add(pageItems);
				double usedPageSize = isColumn ? RowFieldValuesSize.Width : ColumnFieldValuesSize.Height;
				if(PivotPrintPage.GetHeadersVisibility(PivotGrid.Data, curItem) && !isColumn)
					usedPageSize += HeadersHeight;
				PivotFieldValueItem item = VisualItems.GetItem(isColumn, curItem);
				if(curItem == 0 && !isColumn && IsReportHeaderOnFirstPage)
					pageAvailablePrintSize -= ReportHeaderSize.Height;
				while(curItem < count &&
						(!item.IsLastFieldLevel || usedPageSize + GetItemSize(item, !isColumn) < pageAvailablePrintSize)) {
					pageItems.Add(new PrintFieldValueItem(item, VisualItems));
					if(item.IsLastFieldLevel) {
						usedPageSize += GetItemSize(item, !isColumn);
						pageItems.StartIndex = Math.Min(item.MinLastLevelIndex, pageItems.StartIndex);
						pageItems.EndIndex = Math.Max(item.MinLastLevelIndex, pageItems.EndIndex);
					}
					curItem++;
					item = VisualItems.GetItem(isColumn, curItem);
				}
			}
			CutMergedLevels(pages, isColumn);
			return pages;
		}
		void CutMergedLevels(List<PivotPrintPageValueItemsList> pages, bool isColumn) {
			bool cutMergedLevels = isColumn && !PivotGrid.MergeColumnFieldValues ||
				!isColumn && !PivotGrid.MergeRowFieldValues;
			for(int i = 0; i < pages.Count; i++) {
				PivotPrintPageValueItemsList page = pages[i],
					nextPage = i != pages.Count - 1 ? pages[i + 1] : null;
				for(int j = page.Count - 1; j >= 0; j--) {
					FieldValueItem item = (FieldValueItem)page[j];
					if(cutMergedLevels) {
						if(item.MinLastLevelIndex != item.MaxLastLevelIndex) {
							for(int k = Math.Max(item.MinLastLevelIndex, page.StartIndex); k <= Math.Min(item.MaxLastLevelIndex, page.EndIndex); k++)
								page.Add(new PrintFieldValueItem(item.Item, VisualItems, k));
							page.Remove(item);
							if(item.MaxLastLevelIndex > page.EndIndex)
								nextPage.Add(item);
						}
					} else {
						if(item.MaxLastLevelIndex > page.EndIndex) {
							page.Remove(item);
							if(item.MinLastLevelIndex <= page.EndIndex) {
								page.Add(new PrintFieldValueItem(item, Math.Max(item.MinLastLevelIndex, page.StartIndex), page.EndIndex));
							}
							nextPage.Add(new PrintFieldValueItem(item, nextPage.StartIndex, item.MaxLastLevelIndex));
						}
					}
				}
			}
			for(int i = 0; i < pages.Count; i++) {
				pages[i].Sort(PivotFieldValueItemItemsComparer.Default);
			}
		}
		void CheckPageSize() {
			int maxFieldValueHeight = -1;
			int maxFieldValueWidth = -1;
			for(int i = 0; i < PivotGrid.VisualItems.GetLastLevelItemCount(false); i++) {
				maxFieldValueHeight = Math.Max(maxFieldValueHeight, VisualItems.GetItemHeight(i, false));
			}
			for(int i = 0; i < PivotGrid.VisualItems.GetLastLevelItemCount(true); i++) {
				maxFieldValueWidth = Math.Max(maxFieldValueWidth, VisualItems.GetItemWidth(i, true));
			}
			if(RowFieldValuesSize.Width + maxFieldValueWidth > UsablePageSize.Width || ColumnFieldValuesSize.Height + maxFieldValueHeight + HeadersHeight > PrintHeight)
				throw new PivotPrintingException(PivotPrintingException.PageTooSmall);
		}
	}
}

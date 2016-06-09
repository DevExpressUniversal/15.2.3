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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraSpreadsheet.Model;
#if SL
using System.Windows.Controls;
#endif
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	#region FinalWebDocumentLayoutCalculator
	public class FinalWebDocumentLayoutCalculator : FinalDocumentLayoutCalculator {
		#region Fields
		WebDocumentLayout layout;
		#endregion
		public FinalWebDocumentLayoutCalculator(WebDocumentLayout layout, Worksheet sheet, RowVerticalBorderBreaks verticalBorderBreaks)
			: base(layout, sheet, verticalBorderBreaks, Rectangle.Empty, 1.0f) {
			this.layout = layout;
		}
		internal void GenerateBlocks() {
			List<Page> pages = new List<Page>();
			foreach (KeyValuePair<TilePosition, PreliminaryPage> tilePage in layout.PreliminaryPages) {
				PreliminaryPage page = tilePage.Value;
				PageGrid gridColumn = page.GridColumns.OffsetGrid(0);
				PageGrid gridRow = page.GridRows.OffsetGrid(0);
				SetPreliminaryProperties(page);
				Page currentPage = GeneratePage(gridColumn, gridRow);
				pages.Add(currentPage);
				layout.TilePages.Add(tilePage.Key, currentPage);
				GeneratePageHorizontalBorders(currentPage);
				GeneratePageVerticalBorders(currentPage);
			}
			SetOffsets(pages);
			layout.Pages.AddRange(pages);
			int pageIndex = 0;
			foreach (KeyValuePair<TilePosition, PreliminaryPage> tilePage in layout.PreliminaryPages) {
				PreliminaryPage currentPage = tilePage.Value;
				IList<ComplexCellTextBox> complexBoxes = currentPage.ComplexBoxes;
				int count = complexBoxes.Count;
				for (int i = 0; i < count; i++)
					AppendComplexBox(currentPage.GridColumns, currentPage.GridRows, complexBoxes[i], pageIndex);
				pageIndex++;
			}
			GenerateHeaderPages();
			layout.GenerateMergedCells();
		}
		void GenerateHeaderPages() {
			if (!(this.Sheet.ShowRowHeaders && this.Sheet.ShowColumnHeaders))
				return;
			Size groupItemOffset = Size.Empty;
			if (layout.GroupItemsPage != null)
				groupItemOffset = layout.GroupItemsPage.GroupItemsOffset;
			layout.GenerateHeadersContent(null, null, layout.HeaderOffset, groupItemOffset);
		}
		protected override Rectangle CalculatePageClientBounds(Rectangle pageBounds) {
			return new Rectangle(Point.Empty, pageBounds.Size);
		}
		protected override Size GetWebDrawingBoxOffset(PageGrid columnGrid, PageGrid rowGrid) {
			return new Size(columnGrid.First.Near - columnGrid.ActualFirst.Near, rowGrid.First.Near - rowGrid.ActualFirst.Near);
		}
	}
	#endregion
}

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
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region WebDocumentLayout
	public class WebDocumentLayout : DocumentLayout {
		#region Fields
		List<HeaderPage> headerPages;
		Dictionary<CellRange, List<CellRange>> rangeMergedCells;
		#endregion
		public WebDocumentLayout(DocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		public Dictionary<TilePosition, CellRange> TileRanges { get; set; }
		public Size HeaderOffset { get; set; }
		public Dictionary<TilePosition, Page> TilePages { get; set; }
		internal Dictionary<TilePosition, PreliminaryPage> PreliminaryPages { get; set; }
		#endregion
		protected internal override void GenerateHeadersContent(List<PageGrid> columnHeaderGrids, List<PageGrid> rowHeaderGrids, Size headerOffset, Size groupItemsOffset) {
			headerPages = new List<HeaderPage>();
			foreach (Page page in Pages)
				headerPages.Add(new HeaderPage(page, headerOffset, groupItemsOffset));
		}
		public List<HeaderTextBox> GetColumnHeader(int index) {
			return headerPages[index].ColumnBoxes;
		}
		public List<HeaderTextBox> GetRowHeader(int index) {
			return headerPages[index].RowBoxes;
		}
		internal void GenerateMergedCells() {
			if (Pages.Count <= 0)
				return;
			rangeMergedCells = new Dictionary<CellRange, List<CellRange>>();
			foreach (CellRange mergedCell in this.Pages[0].Sheet.MergedCells.GetEVERYMergedRangeSLOWEnumerable())
				foreach (KeyValuePair<TilePosition, CellRange> range in TileRanges) {
					if (range.Value.Intersects(mergedCell)) {
						if (!rangeMergedCells.ContainsKey(range.Value))
							rangeMergedCells.Add(range.Value, new List<CellRange>());
						rangeMergedCells[range.Value].Add(mergedCell);
					}
				}
		}
		public List<CellRange> GetMergedCells(Page page) {
			CellRange range = new CellRange(page.Sheet,
				new CellPosition(page.GridColumns.ActualFirst.ModelIndex, page.GridRows.ActualFirst.ModelIndex),
				new CellPosition(page.GridColumns.ActualLast.ModelIndex, page.GridRows.ActualLast.ModelIndex));
			if (rangeMergedCells.ContainsKey(range))
				return rangeMergedCells[range];
			return new List<CellRange>();
		}
	}
	#endregion
}

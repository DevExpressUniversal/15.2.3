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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	#region PreliminaryPageRowInfoModelRowIndexComparable
	public class PreliminaryPageRowInfoModelRowIndexComparable : IComparable<PreliminaryPageRowInfo> {
		readonly PreliminaryPage page;
		readonly int modelRowIndex;
		public PreliminaryPageRowInfoModelRowIndexComparable(PreliminaryPage page, int modelRowIndex) {
			Guard.ArgumentNotNull(page, "page");
			this.page = page;
			this.modelRowIndex = modelRowIndex;
		}
		#region IComparable<PreliminaryPageRowInfo> Members
		public int CompareTo(PreliminaryPageRowInfo other) {
			int otherModelRowIndex = page.GridRows[other.GridRowIndex].ModelIndex;
			return otherModelRowIndex - modelRowIndex;
		}
		#endregion
	}
	#endregion
	#region PreliminaryPageRowInfo
	public struct PreliminaryPageRowInfo {
		public int GridRowIndex { get; set; }
		public int LastCellIndex { get; set; }
	}
	#endregion
	#region PreliminaryPage
	public class PreliminaryPage : Page {
		readonly List<PreliminaryPageRowInfo> singleBoxRowInfos;
		readonly List<IDrawingObject> drawingObjects;
		readonly List<Comment> comments;
		readonly bool hasRightPage;
		List<ComplexCellTextBox> longTextBoxes;
		public PreliminaryPage(DocumentLayout documentLayout, Worksheet sheet, PageGrid gridColumns, PageGrid gridRows, bool hasRightPage)
			: base(documentLayout, sheet, gridColumns, gridRows) {
			this.singleBoxRowInfos = new List<PreliminaryPageRowInfo>();
			this.drawingObjects = new List<IDrawingObject>();
			this.comments = new List<Comment>();
			this.longTextBoxes = new List<ComplexCellTextBox>();
			this.hasRightPage = hasRightPage;
		}
		public List<PreliminaryPageRowInfo> SingleBoxRowInfos { get { return singleBoxRowInfos; } }
		public List<IDrawingObject> DrawingObjects { get { return drawingObjects; } }
		public List<Comment> Comments { get { return comments; } }
		public List<ComplexCellTextBox> LongTextBoxes { get { return longTextBoxes; } }
		internal bool HasRightPage { get { return hasRightPage; } }
		internal RowVerticalBorderBreaks Breaks { get; set; }
		internal void IntegratePreviousPageLongTextBoxes(PreliminaryPage previousPage) {
			List<int> breakersRows = GetBreakersRows(previousPage);
			foreach (ComplexCellTextBox box in previousPage.longTextBoxes) {
				if (box.ClipLastColumnIndex > previousPage.GridColumns.ActualLastIndex && !breakersRows.Contains(box.ClipFirstRowIndex)) {
					int lastClipModelIndex = previousPage.GridColumns[box.ClipLastColumnIndex].ModelIndex;
					int lastClipIndex = GridColumns.TryCalculateIndex(lastClipModelIndex);
					if (lastClipIndex < 0 && lastClipModelIndex < GridColumns.ActualFirst.ModelIndex)
						continue;
					if (lastClipIndex < 0)
						lastClipIndex = GridColumns.ActualLast.Index;
					ComplexCellTextBox newBox = new ComplexCellTextBox(box.GetCell(null, null, null));
					newBox.ClipFirstColumnIndex = 0;
					newBox.ClipLastColumnIndex = lastClipIndex;
					int clipFirstRowModelIndex = previousPage.GridRows[box.ClipFirstRowIndex].ModelIndex;
					int clipFirstRowIndex = GridRows.TryCalculateIndex(clipFirstRowModelIndex);
					if (clipFirstRowIndex < 0) {
						if (clipFirstRowModelIndex > GridRows.ActualLast.ModelIndex)
							continue;
						clipFirstRowIndex = GridRows.ActualFirst.Index;
					}
					newBox.ClipFirstRowIndex = clipFirstRowIndex;
					int clipLastRowModelIndex = previousPage.GridRows[box.ClipLastRowIndex].ModelIndex;
					int clipLastRowIndex = GridRows.TryCalculateIndex(clipLastRowModelIndex);
					if (clipLastRowIndex < 0) {
						if (clipLastRowModelIndex < GridRows.ActualFirst.ModelIndex)
							continue;
						clipLastRowIndex = GridRows.ActualLast.Index;
					}
					newBox.ClipLastRowIndex = clipLastRowIndex;
					if (HasRightPage) {
						int offset = previousPage.GridRows.ActualFirst.Near - GridRows.ActualFirst.Near;
						newBox.Bounds = new Rectangle(box.Bounds.X, box.Bounds.Y - offset, box.Bounds.Width, box.Bounds.Height);
					}
					else
						newBox.Bounds = box.Bounds;
					box.ClipLastColumnIndex = previousPage.GridColumns.ActualLastIndex;
					ComplexBoxes.Add(newBox);
					if (!RowComplexBoxes.ContainsKey(newBox.ClipFirstRowIndex))
						RowComplexBoxes.Add(newBox.ClipFirstRowIndex, new ComplexCellTextBoxList());
					RowComplexBoxes[clipFirstRowIndex].Add(newBox);
				}
			}
		}
		List<int> GetBreakersRows(PreliminaryPage previousPage) {
			List<int> result = new List<int>();
			if (this.GridColumns.ActualFirst.ModelIndex - 1 == previousPage.GridColumns.ActualLast.ModelIndex)
				return result;
			CellRange range = new CellRange(previousPage.Sheet, new CellPosition(previousPage.GridColumns.ActualLast.ModelIndex + 1, previousPage.GridRows.ActualFirst.ModelIndex),
				new CellPosition(this.GridColumns.ActualFirst.ModelIndex - 1, previousPage.GridRows.ActualLast.ModelIndex));
			PageGrid rowGrid = previousPage.GridRows;
			foreach (ICellBase info in range.GetLayoutVisibleCellsEnumerable(true)) {
				ICell cell = info as ICell;
				int index = rowGrid.LookupItem(cell.RowIndex);
				if (index < 0)
					index = ~index;
				result.Add(index);
				if (previousPage.Breaks != null && previousPage.Breaks.ContainsKey(cell.RowIndex)) {
					int columnIndex = previousPage.GridColumns.LookupFarItem(cell.ColumnIndex);
					if (columnIndex < 0)
						columnIndex = ~columnIndex;
					previousPage.Breaks.RemoveContinuousBreaks(cell.RowIndex, columnIndex);
				}
			}
			return result;
		}
	}
	#endregion
}

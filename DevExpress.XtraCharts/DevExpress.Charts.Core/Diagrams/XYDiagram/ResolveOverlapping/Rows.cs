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
namespace DevExpress.Charts.Native {
	public class Cell : IComparable {
		GRect2D bounds;
		int infillCount;
		public GRect2D Bounds { get { return bounds; } }
		public bool IsEmpty { get { return infillCount == 0; } }
		public int InfillCount { get { return infillCount; } }
		public Cell(GRect2D bounds) : this(bounds, 0) {
		}
		public Cell(GRect2D bounds, int infillCount) {
			this.bounds = bounds;
			this.infillCount = infillCount;
		}
		public int CompareTo(Object obj) {
			Cell cell = obj as Cell;
			if (cell == null)
				return 1;
			return Bounds.Left.CompareTo(cell.Bounds.Left);
		}
		public void Infill() {
			++infillCount;
		}
		public void Clear() {
			--infillCount;
			if (infillCount < 0)
				infillCount = 0;
		}
	}
	public class Row : IComparable {
		readonly GRect2D bounds;
		readonly List<Cell> items = new List<Cell>();
		readonly double center;
		public GRect2D Bounds { get { return bounds; } }
		public List<Cell> Items { get { return items; } }
		public Row(GRect2D bounds) {
			this.bounds = bounds;
			center = bounds.Left + bounds.Width * 0.5;
			items.Add(new Cell(bounds));
		}
		public Row(GRect2D bounds, List<Cell> items) {
			this.bounds = bounds;
			center = bounds.Left + bounds.Width * 0.5;
			this.items = items;
		}
		public int CompareTo(Object obj) {
			Row row = obj as Row;
			if (row == null)
				return 1;
			return Bounds.Top.CompareTo(row.Bounds.Top);
		}
		public void SeparateByVertical(int separator, int columnIndex) {
			Cell cell = Items[columnIndex];
			if (separator > cell.Bounds.Left && separator < cell.Bounds.Right) {
				Items[columnIndex] = new Cell(new GRect2D(separator, cell.Bounds.Top, cell.Bounds.Right - separator, cell.Bounds.Height), cell.InfillCount);
				Items.Insert(columnIndex, new Cell(new GRect2D(cell.Bounds.Left, cell.Bounds.Top, separator - cell.Bounds.Left, cell.Bounds.Height), cell.InfillCount));				
			}
		}
		public Cell FindCell(int x, out int columnIndex) {
			columnIndex = items.BinarySearch(new Cell(new GRect2D(x, 0, 1, 1)));
			if (columnIndex >= 0)
				return items[columnIndex];
			columnIndex = ~columnIndex - 1;
			if (columnIndex < 0 || columnIndex > items.Count - 1)
				return null;
			return items[columnIndex];			
		}
	}
	public class Rows {
		readonly GRect2D bounds;
		readonly double centerX;
		readonly double centerY;
		List<Row> rows = new List<Row>();
		public int Count { get { return rows.Count; } }
		public int ColumnsCount { get { return rows.Count > 0 ? rows[0].Items.Count : 0; } }
		public Rows(GRect2D bounds) {
			this.bounds = bounds;
			centerX = bounds.Left + bounds.Width * 0.5;
			centerY = bounds.Top + bounds.Height * 0.5;
			rows.Add(new Row(bounds));
		}
		List<Cell> FindIntersectionCells(GRect2D rect) {
			int rowIndex;
			int columnIndex;
			List<Cell> result = new List<Cell>();
			if (FindNearCell(rect.Left, rect.Top, out rowIndex, out columnIndex) == null) 
				return result;			
			for (int i = rowIndex; i < rows.Count; i++) {
				Row row = rows[i];
				if (row.Bounds.Top > rect.Bottom)
					return result;
				for (int j = columnIndex; j < row.Items.Count; j++) {
					Cell item = row.Items[j];
					if (item.Bounds.Left > rect.Right)
						break;
					if (GRect2D.IsIntersected(rect, item.Bounds))
						result.Add(item);
				}
			}			
			return result;
		}
		Cell FindCell(int x, int y, out int rowIndex, out int columnIndex) {
			Row row = FindRow(y, out rowIndex);
			if (row == null) {
				columnIndex = -1;
				return null;
			}
			return row.FindCell(x, out columnIndex);
		}
		Row FindRow(int y, out int rowIndex) {
			rowIndex = rows.BinarySearch(new Row(new GRect2D(0, y, 1, 1)));
			if (rowIndex < 0)
				rowIndex = ~rowIndex - 1;
			if (rowIndex < 0 || rowIndex >= rows.Count) {
				return null;
			}
			return rows[rowIndex];			
		}
		public void AddRectangle(GRect2D rect) {
			if (rect.Height == 0 || rect.Width == 0)
				return;
			SeparateByHorizontal(rect.Top);
			SeparateByHorizontal(rect.Bottom);
			SeparateByVertical(rect.Left);
			SeparateByVertical(rect.Right);
			foreach (Cell item in FindIntersectionCells(rect))
				item.Infill();
		}
		public void DeleteRectangle(GRect2D rect) {
			foreach (Cell cell in FindIntersectionCells(rect))
				cell.Clear();
		}
		public Cell GetCell(int rowIndex, int columnIndex) {
			return rows[rowIndex].Items[columnIndex];
		}
		public Cell FindNearCell(int x, int y, out int rowIndex, out int columnIndex) {
			Row row;
			if (y <= bounds.Top) {
				rowIndex = 0;
				row = rows[rowIndex];
			}
			else {
				if (y > bounds.Bottom) {
					rowIndex = Count - 1;
					row = rows[rowIndex];
				}
				else
					row = FindRow(y, out rowIndex);
			}
			if (x <= bounds.Left) {
				columnIndex = 0;
				return row.Items[columnIndex];
			}
			else {
				if (x > bounds.Right) {
					columnIndex = ColumnsCount - 1;
					return row.Items[columnIndex];
				}
				else
					return row.FindCell(x, out columnIndex);
			}
		}
		public bool IsEmptyRegion(GRect2D rect) {
			int rowIndex;
			int columnIndex;
			if (FindCell(rect.Left, rect.Top, out rowIndex, out columnIndex) == null)
				return true;
			for (int i = rowIndex; i < rows.Count; i++) {
				Row row = rows[i];
				if (row.Bounds.Top > rect.Bottom)
					return true;
				for (int j = columnIndex; j < row.Items.Count; j++) {
					Cell item = row.Items[j];
					if (item.Bounds.Left > rect.Right)
						break;
					if (!item.IsEmpty && GRect2D.IsIntersected(rect, item.Bounds))
						return false;
				}
			}
			return true;
		}
		public void SeparateByHorizontal(int separator) {
			int rowIndex;
			Row row = FindRow(separator, out rowIndex);
			if (row != null) {
				if (separator > row.Bounds.Top && separator < row.Bounds.Bottom) {
					List<Cell> topItems = new List<Cell>();
					List<Cell> bottomItems = new List<Cell>();
					foreach (Cell cell in row.Items) {
						GRect2D newBounds = new GRect2D(cell.Bounds.Left, cell.Bounds.Top, cell.Bounds.Width, separator - cell.Bounds.Top);
						bottomItems.Add(new Cell(newBounds, cell.InfillCount));
						newBounds = new GRect2D(cell.Bounds.Left, separator, cell.Bounds.Width, cell.Bounds.Bottom - separator);
						topItems.Add(new Cell(newBounds, cell.InfillCount));
					}
					rows.Insert(rowIndex, new Row(new GRect2D(row.Bounds.Left, separator, row.Bounds.Width, row.Bounds.Bottom - separator), topItems));
					rows.Insert(rowIndex, new Row(new GRect2D(row.Bounds.Left, row.Bounds.Top, row.Bounds.Width, separator - row.Bounds.Top), bottomItems));
					rows.Remove(row);
				}
			}
		}
		public void SeparateByVertical(int separator) {
			if (rows.Count <= 0)
				return;
			int columnIndex;
			if (rows[0].FindCell(separator, out columnIndex) != null) {
				foreach (Row row in rows)
					row.SeparateByVertical(separator, columnIndex);
			}
		}
	}
}

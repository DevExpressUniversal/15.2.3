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
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Bars {	
	public interface IBarLayoutTableInfo {
		Bar Bar { get; }
		bool UseWholeRow { get; }
		double Opacity { get; set; }
		int Row { get; set; }
		int Column { get; set; }	 
		int CollectionIndex { get; }   
		bool CanReduce { get; }
		double Offset { get; set; }
		void InvalidateMeasure();
		void Measure(Size constraint);
		void Arrange(Rect finalRect);
		Size DesiredSize { get; }
		Size RenderSize { get; }
		bool MakeFloating();
		event EventHandler LayoutPropertyChanged;
		bool CanDock(Dock dock);
		bool IsVisible { get; }
	}	
	public abstract class LayoutTableSizeHelper {
		public static LayoutTableSizeHelper Get(Orientation orientation) {
			if (orientation == Orientation.Horizontal)
				return new HorizontalLayoutTableSizeHelper();
			return new VerticalLayoutTableSizeHelper();
		}
		public abstract double GetWidth(Size size);
		public abstract double GetHeight(Size size);
		public abstract void SetWidth(ref Size size, double value);
		public abstract void SetHeight(ref Size size, double value);
		public abstract double GetX(Point point);
		public abstract double GetY(Point point);
		public abstract Point Translate(Point point);
		public abstract Size Translate(Size size);
		public abstract TResult Do<TResult>(Func<TResult> horizontal, Func<TResult> vertical);
		class HorizontalLayoutTableSizeHelper : LayoutTableSizeHelper {
			public override double GetHeight(Size size) { return size.Height; }
			public override double GetWidth(Size size) { return size.Width; }
			public override double GetX(Point point) { return point.X; }
			public override double GetY(Point point) { return point.Y; }
			public override TResult Do<TResult>(Func<TResult> horizontal, Func<TResult> vertical) { return horizontal(); }
			public override Point Translate(Point point) { return point; }
			public override Size Translate(Size size) { return size; }
			public override void SetHeight(ref Size size, double value) { size.Height = value; }
			public override void SetWidth(ref Size size, double value) { size.Width = value; }
		}
		class VerticalLayoutTableSizeHelper : LayoutTableSizeHelper {
			public override double GetWidth(Size size) { return size.Height; }
			public override double GetHeight(Size size) { return size.Width; }
			public override double GetY(Point point) { return point.X; }
			public override double GetX(Point point) { return point.Y; }
			public override TResult Do<TResult>(Func<TResult> horizontal, Func<TResult> vertical) { return vertical(); }
			public override Point Translate(Point point) { return new Point(point.Y, point.X); }
			public override Size Translate(Size size) { return new Size(size.Height, size.Width); }
			public override void SetHeight(ref Size size, double value) { size.Width = value; }
			public override void SetWidth(ref Size size, double value) { size.Height = value; }
		}
	}
	public class BarLayoutTable {
		List<BarLayoutTableRow> rows = new List<BarLayoutTableRow>();
		readonly List<IBarLayoutTableInfo> elements = new List<IBarLayoutTableInfo>();
		bool needsUpdate;
		Orientation orientation;
		public List<BarLayoutTableRow> Rows { get { return rows; } }
		protected List<IBarLayoutTableInfo> Elements { get { return elements; } }
		public bool IgnoreElementOffset { get; set; }
		public double RowIndent { get; private set; }
		public double ColumnIndent { get; protected set; }  
		public bool LastChildFill { get; protected set; }	  
		public Size DesiredSize { get; protected set; }
		public Size RenderSize { get { return owner.Container.RenderSize; } }
		public bool NeedsUpdate {
			get { return needsUpdate; }
			set {
				if (needsUpdate == value) return;
				needsUpdate = value;
				RaiseRequestMeasure();
			}
		}
		BaseBarLayoutCalculator owner;
		public BaseBarLayoutCalculator Owner { get { return owner; } }
		public BarLayoutTable(BaseBarLayoutCalculator owner) {
			this.owner = owner;
		}
		public Orientation Orientation {
			get { return orientation; }
			set {
				if (value == orientation) return;
				Orientation oldValue = orientation;
				orientation = value;
				OnOrientationChanged(oldValue);
			}
		}
		LayoutTableSizeHelper helper = LayoutTableSizeHelper.Get(Orientation.Horizontal);
		public LayoutTableSizeHelper Helper {
			get { return helper; }
		}
		public event EventHandler RequestMeasure;
		public virtual BarLayoutTableCell Add(IBarLayoutTableInfo element) {
			var rowIndex = Math.Max(0, element.Row);
			BarLayoutTableRow row;
			bool reorder = !GetOrCreateRow(rowIndex, out row, element.UseWholeRow);
			var cell = row.Add(element);
			elements.Add(element);
			if (reorder) {
				Reorder();
			}			
			NeedsUpdate = true;
			return cell;
		}
		public virtual void Remove(IBarLayoutTableInfo element) {
			BarLayoutTableRow row = rows.FirstOrDefault(x=>x.Contains(element));
			elements.Remove(element);
			if (row != null) {
				row.Remove(element);
				if (row.IsEmpty) {
					rows.Remove(row);
					Reorder();
				}
			}			
			NeedsUpdate = true;
		}
		protected virtual void Reorder() {			
			rows = rows.OrderBy(x => x.Index).ThenBy(x => x.WholeRowItemIndex).ToList();
		}
		public void SetRowIndent(double value) {
			if (Equals(RowIndent, value))
				return;
			RowIndent = value;
			NeedsUpdate = true;
		}
		public void SetColumnIndent(double value) {
			if (Equals(ColumnIndent, value))
				return;
			ColumnIndent = value;
			NeedsUpdate = true;
		}
		public void SetLastChildFill(bool value) {
			if (Equals(LastChildFill, value))
				return;
			LastChildFill = value;
			NeedsUpdate = true;
		}
		public virtual void Measure(Size constraint) {
			Size result = new Size();
			if (NeedsUpdate) {
				Reset();
				DesiredSize = new Size();
			}
			foreach (var value in rows) {
				value.Measure(constraint);
				AppendSize(ref result, value.DesiredSize);				
			}
			DesiredSize = result;
			NeedsUpdate = false;
		}
		protected virtual void Reset() {
			var elements = Elements.ToArray();
			foreach (var element in elements.Reverse()) {
				Remove(element);
			}
			foreach(var element in elements) {
				Add(element);
			}
		}
		public virtual void Arrange(Size finalSize) {
			Size arrangeSize = new Size();
			foreach(var value in rows) {
				if(Orientation== Orientation.Horizontal) {
					value.Arrange(new Rect(0d, arrangeSize.Height, finalSize.Width, value.DesiredSize.Height));
				} else {
					value.Arrange(new Rect(arrangeSize.Width, 0d, value.DesiredSize.Width, finalSize.Height));
				}				
				AppendSize(ref arrangeSize, value.RenderSize);
			}
		}
		protected virtual void RaiseRequestMeasure() {
			if (RequestMeasure == null)
				return;
			RequestMeasure(this, EventArgs.Empty);
		}	 
		protected virtual bool GetOrCreateRow(int index, out BarLayoutTableRow row, bool needsWholeRow) {
			if (!needsWholeRow) {
				foreach (var element in rows) {
					if (element.Index == index && element.CanAddItems) {
						row = element;
						return true;
					}
				}
			}
			row = CreateTableRow(index);
			rows.Add(row);
			return false;
		}
		protected virtual void OnOrientationChanged(Orientation oldValue) { helper = LayoutTableSizeHelper.Get(Orientation); }
		protected virtual BarLayoutTableRow CreateTableRow(int index) {
			return new BarLayoutTableRow(this) { Index = index };
		}
		public bool IsFirstRow(BarLayoutTableRow row) { return Equals(rows.FirstOrDefault(), row); }
		public bool IsLastRow(BarLayoutTableRow row) { return Equals(rows.LastOrDefault(), row); }
		protected virtual void AppendSize(ref Size target, Size source) {
			if (Orientation == Orientation.Horizontal) {
				target.Width = Math.Max(target.Width, source.Width);
				target.Height += source.Height;				
			} else {
				target.Height = Math.Max(target.Height, source.Height);
				target.Width += source.Width;
			}
		}
		public virtual void UpdateIndexes() {			
			Reorder();
			Rows.RemoveAll(x => x.IsEmpty);
			for (int i = 0; i<rows.Count; i++) {
				rows[i].Index = i;
				rows[i].UpdateIndexes();
			}
		}		
		public virtual BarLayoutTableCell FindCell(Bar bar) {
			BarLayoutTableCell cell = null;
			foreach (var row in Rows)
				if ((cell = row.FindCell(bar)) != null)
					break;
			return cell;
		}
		public virtual BarLayoutTableCell FindCell(Point mousePosition) {
			var row = FindRow(mousePosition);
			if (row == null) return null;
			return row.FindCell(mousePosition, true);
		}
		public virtual BarLayoutTableCell FindCell(IBarLayoutTableInfo layoutInfo) {
			return FindRow(layoutInfo).With(x => x.FindCell(layoutInfo));
		}
		public virtual BarLayoutTableRow FindRow(IBarLayoutTableInfo layoutInfo) {
			return Rows.FirstOrDefault(x => x.Contains(layoutInfo));
		}
		public virtual BarLayoutTableRow FindRow(Point mousePosition) {
			foreach(var row in Rows) {
				if (row.RenderRect.Contains(mousePosition))
					return row;
			}
			return null;
		}		
		public virtual BarLayoutTableCell FindClosestCell(Point mousePosition, Func<BarLayoutTableCell, bool> predicate) {
			return FindClosestRow(mousePosition).With(x => x.FindClosestCell(mousePosition, predicate));
		}
		public virtual BarLayoutTableRow FindClosestRow(Point mousePosition) {
			return Rows.OrderBy(x => (new Rect(x.RenderSize).Center() - new Point(0d, mousePosition.Y)).LengthSquared).FirstOrDefault();
		}
		public virtual void MoveCells(ref BarLayoutTableCell first, ref BarLayoutTableCell second) {
			var e1 = first.Element;
			var e2 = second.Element;
			var e1r = first.Element.Row;
			var e1c = first.Element.Column;
			Remove(e1);
			Remove(e2);
			e1.Row = e2.Row;
			e1.Column = e2.Column;
			e2.Row = e1r;
			e2.Column = e1c;
			first = Add(e1);
			second = Add(e2);			
			UpdateIndexes();
		}
		public virtual void MoveCell(ref BarLayoutTableCell cell, int rowIndex) {
			if (cell.Element.Row == rowIndex)
				return;
			int upperrows = rowIndex + 1;
			if (Rows.IsValidIndex(upperrows)) {
				Rows.GetRange(upperrows, Rows.Count - upperrows).ForEach(x => x.Index++);
			}
			rowIndex = Math.Max(0, rowIndex);
			Rows.Add(CreateTableRow(rowIndex));
			Reorder();
			var element = cell.Element;
			Remove(element);
			element.Row = rowIndex;
			cell = Add(element);
			UpdateIndexes();
		}
		public void DragCell(BarLayoutTableCell currentCell, Point mousePosition, Point delta) {
			var row = GetClosestRow(mousePosition);
			row.DragCell(currentCell, mousePosition, delta);
		}
		protected virtual BarLayoutTableRow GetClosestRow(Point mousePosition) {
			return Rows.Select(x => new { Row = x, Offset = Math.Abs(Helper.GetY(x.RenderRect.Center()) - Helper.GetY(mousePosition))}).OrderBy(x => x.Offset).FirstOrDefault().With(x => x.Row);
		}
		public virtual void DragCellToNewRow(int newRowIndex, BarLayoutTableCell currentCell, Point relativeToTablePoint, Point delta) {
			var row = CreateTableRow(newRowIndex);
			Rows.Add(row);
			Reorder();
			row.DragCell(currentCell, relativeToTablePoint, delta, true);			
		}
		public virtual void IncrementIndexes(int startWith) {
			Rows.Skip(startWith).ForEach(x => x.IncrementIndexes());			
		}
		public virtual void InsertFloatBar(Bar bar, Point point) {
			var cType = owner.With(x => x.Container).With(x => x.Owner).Return(x => x.ContainerType, () => BarContainerType.Top);
			if(cType == BarContainerType.Bottom || cType == BarContainerType.Right) {
				bar.DockInfo.Row = 0;
			} else {
				bar.DockInfo.Row = int.MaxValue;
			}
			bar.DockInfo.Offset = Helper.GetX(point);
		}
	}
	public class BarLayoutTableRow {
		readonly BarLayoutTable table;
		List<BarLayoutTableCell> cells = new List<BarLayoutTableCell>();
		protected List<BarLayoutTableCell> Cells { get { return cells; } }
		protected bool IsFirst { get { return table.IsFirstRow(this); } }
		protected bool IsLast { get { return table.IsLastRow(this); } }
		protected double RowIndent { get { return IsFirst ? 0d : table.RowIndent; } }
		public BarLayoutTable Table { get { return table; } }
		public LayoutTableSizeHelper Helper { get { return table.Helper; } }
		public Size DesiredSize { get; private set; }
		public Size RenderSize { get; private set; }
		public Rect RenderRect { get; private set; }
		public int Index { get; set; }
		public bool CanAddItems { get { return !cells.Any(x => x.UseWholeRow); } }
		public object WholeRowItemIndex { get { return CanAddItems ? 0 : cells.FirstOrDefault().Return(x => x.Element.CollectionIndex, () => 0); } }
		public bool IsEmpty { get { return !cells.Any(); } }
		public bool HasReducedCells { get { return cells.Any(x => x.CanExpand); } }
		public BarLayoutTableRow(BarLayoutTable table) {
			this.table = table;
		}
		public virtual BarLayoutTableCell Add(IBarLayoutTableInfo element) {
			var columnIndex = Math.Max(0, element.Column);
			BarLayoutTableCell cell = CreateTableCell(element);
			cells.Add(cell);
			Reorder();
			return cell;
		}		
		public virtual void Remove(IBarLayoutTableInfo element) {
			var cell = cells.FirstOrDefault(x => x.Element == element);
			cell.Element = null;
			cells.Remove(cell);
			Reorder();		   
		}
		protected virtual void Reorder() {
			cells = cells.OrderBy(x => x.Element.Column).ThenBy(x => x.Element.CollectionIndex).ToList();
		}
		public virtual bool Contains(IBarLayoutTableInfo element) {
			return cells.Any(x => x.Element == element);
		}
		public virtual void Measure(Size constraint) {									
			Size resultSize = new Size();
			bool canReduce = true;
			var oldResultSize = resultSize;
			int step = 0;
			while (canReduce) {
				canReduce = false;
				resultSize = new Size();
				var availableWidth = Helper.GetWidth(constraint);
				for (int i = 0; i < cells.Count; i++) {
					var cell = cells[i];
					var leftCells = cells.Where((c, ci) => ci >= 0 && ci < i);
					var rightCells = cells.Where((c, ci) => ci > i && ci < cells.Count);
					var left = leftCells.Sum(x => x.DesiredSize.Width);
					var right = rightCells.Sum(x => x.MinDesiredSize.Width);
					var delta = left;
					if (!rightCells.Any(x => x.CanReduce))
						delta = left + right;
					bool leftHasCollapsedItems = leftCells.Any(x => x.CanExpand);
					var newConstraint = availableWidth - delta;
					if (leftHasCollapsedItems && newConstraint > cell.Constraint)
						newConstraint = cell.Constraint;
					else {
					}
					cell.Constraint = newConstraint;
					cell.Measure();
					AppendSize(ref resultSize, cell.DesiredSize);
					canReduce |= cell.CanReduce;
				}
				if (Helper.GetWidth(resultSize) <= Helper.GetWidth(constraint) || oldResultSize == resultSize && step==cells.Count)
					canReduce = false;
				oldResultSize = resultSize;
				step++;
			}
			Helper.SetWidth(ref resultSize, Math.Max(Helper.GetWidth(resultSize), Helper.GetWidth(resultSize) + RowIndent));
			DesiredSize = resultSize;			
		}		
		public virtual void Arrange(Rect finalRect) {			
			Size arrangeSize = new Size();
			for (int i = 0; i < cells.Count; i++) {
				var cell = cells[i];				
				double width = Helper.GetWidth(cell.DesiredSize);
				if (cell.Fill)
					width = Math.Max(0, Helper.GetWidth(finalRect.Size) - Helper.GetWidth(arrangeSize));
				if(Table.Orientation == Orientation.Horizontal) {
					cell.Arrange(new Rect(arrangeSize.Width, finalRect.Top, width, DesiredSize.Height));
				} else {
					cell.Arrange(new Rect(finalRect.Left, arrangeSize.Height, DesiredSize.Width, width));
				}
				AppendSize(ref arrangeSize, cell.RenderSize);
			}
			RenderSize = arrangeSize;
			RenderRect = new Rect(new Point(finalRect.Left, finalRect.Top), arrangeSize);
		}
		public bool IsFirstColumn(BarLayoutTableCell cell) { return Equals(cells.FirstOrDefault(), cell); }
		public bool IsLastColumn(BarLayoutTableCell cell) { return Equals(cells.LastOrDefault(), cell); }				
		protected virtual void AppendSize(ref Size target, Size source) {
			if(Table.Orientation == Orientation.Horizontal) {
				target.Height = Math.Max(target.Height, source.Height);
				target.Width += source.Width;
			} else {
				target.Width = Math.Max(target.Width, source.Width);
				target.Height += source.Height;
			}
		}
		protected virtual BarLayoutTableCell CreateTableCell(IBarLayoutTableInfo element) {
			return new BarLayoutTableCell(this, table) { Element = element };
		}
		public virtual void UpdateIndexes() {
			Reorder();	
			for(int i = 0; i<cells.Count; i++) {
				cells[i].UpdateIndex(Index, i);
			}
		}
		public virtual BarLayoutTableCell FindCell(Bar bar) {
			foreach (var cell in Cells)
				if (cell.Element.With(x => x.Bar) == bar)
					return cell;
			return null;
		}
		public virtual BarLayoutTableCell FindCell(Point mousePosition, bool positionInTableCoords) {
			var searchPoint = positionInTableCoords ? mousePosition : new Point(RenderRect.Left + mousePosition.X, RenderRect.Top + mousePosition.Y);
			foreach(var cell in Cells) {
				if (cell.RenderRect.Contains(mousePosition)) {
					return cell;
				}
			}
			return null;
		}
		public virtual BarLayoutTableCell FindCell(IBarLayoutTableInfo layoutInfo) {
			return Cells.FirstOrDefault(x => x.Element == layoutInfo);
		}
		public virtual BarLayoutTableCell FindClosestCell(Point mousePosition, Func<BarLayoutTableCell, bool> predicate) {
			return Cells.Where(predicate).OrderBy(x => (x.RenderRect.Center() - mousePosition).LengthSquared).FirstOrDefault();
		}
		public double GetRightAvailableSize(BarLayoutTableCell currentCell) {
			IEnumerable<BarLayoutTableCell> rightCells = GetRightCells(currentCell);
			var right = rightCells.Sum(x => x.RenderSize.Width);
			return Table.RenderSize.Width - currentCell.RenderSize.Width - right;
		}
		IEnumerable<BarLayoutTableCell> GetRightCells(BarLayoutTableCell currentCell, bool includeSelf = false) {
			return cells.Skip(cells.IndexOf(currentCell) + (includeSelf ? 0 : 1)).Where(x=>x.IsVisible);
		}
		IEnumerable<BarLayoutTableCell> GetLeftCells(BarLayoutTableCell currentCell, bool includeSelf = false) {
			return cells.Take(cells.IndexOf(currentCell) + (includeSelf ? 1 : 0)).Where(x => x.IsVisible);
		}
		public double GetLeftAvailableSize(BarLayoutTableCell currentCell) {
			return currentCell.Element.Offset;
		}
		public BarLayoutTableCell GetNextSibling(BarLayoutTableCell cell) {
			var index = cells.IndexOf(cell);
			if (index == -1)
				return null;
			var newIndex = index + 1;
			if (cells.IsValidIndex(newIndex)) {
				var result = cells[newIndex];
				if (result.Element == null || !result.Element.IsVisible)
					return GetNextSibling(result);
				return result;
			}
			return null;
		}
		public BarLayoutTableCell GetPreviousSibling(BarLayoutTableCell cell) {
			var index = cells.IndexOf(cell);
			if (index == -1)
				return null;
			var newIndex = index - 1;
			if (cells.IsValidIndex(newIndex)) {
				var result = cells[newIndex];
				if (result.Element == null || !result.Element.IsVisible)
					return GetPreviousSibling(result);
				return result;
			}				
			return null;
		}
		protected Point GetRelativePosition(Point mousePosition) {
			return new Point(mousePosition.X - RenderRect.Left, mousePosition.Y - RenderRect.Top);
		}
		protected Point GetTablePosition(Point relativePosition) {
			return new Point(relativePosition.X + RenderRect.Left, relativePosition.Y + RenderRect.Top);
		}
		public virtual void DragCell(BarLayoutTableCell currentCell, Point relativeToTablePoint, Point delta, bool skipHeightCheck = false) {
			var relativeToSelfPoint = GetRelativePosition(relativeToTablePoint);
			var higher = Helper.GetY(relativeToSelfPoint) < Helper.GetHeight(RenderSize) * 0.1d;
			var lower = Helper.GetY(relativeToSelfPoint) > Helper.GetHeight(RenderSize) * 0.9d;			
			if (!skipHeightCheck && (higher || lower)) {
				var index = Index;
				if (lower)
					index++;
				var cType = Table.Owner.With(x => x.Container).With(x => x.Owner).Return(x => x.ContainerType, () => BarContainerType.Top);
				if (Cells.Count > 1) {
					if(cType == BarContainerType.Right || cType == BarContainerType.Bottom) {
						if (lower && Table.Rows.All(x => x.Index <= Index))
							return;
					} else {
						if (higher && Index == 0)
							return;
					}
					Table.IncrementIndexes(index);
					Table.DragCellToNewRow(index, currentCell, relativeToTablePoint, delta);
				}
				return;
			}
			if (currentCell.Row == this) {
				DragCellInsideRow(currentCell, relativeToSelfPoint, delta);
			}				
			else {
				if (Cells.Any(x => x.UseWholeRow))
					return;
				DragCellFromAnotherRow(currentCell, relativeToSelfPoint, delta);
			}
		}
		void DragCellFromAnotherRow(BarLayoutTableCell currentCell, Point relativeToSelfPoint, Point delta) {
			var oldRow = currentCell.Row;
			if (currentCell.NextSibling != null) {
				currentCell.NextSibling.Element.Offset += Helper.GetWidth(currentCell.RenderSize);
			}
			var element = currentCell.Element;
			var elementWidth = Helper.GetWidth(currentCell.RenderSize);
			var elementInfiniteSize = currentCell.InfiniteSize;
			element.Offset = 0;
			oldRow.Remove(element);
			var relativeToTablePoint = GetTablePosition(relativeToSelfPoint);
			var cellUnderMouse = Table.FindCell(relativeToTablePoint);
			if (cellUnderMouse != null) {
				var dock = cellUnderMouse.GetDock(relativeToTablePoint);
				var dockLeft = dock == Dock.Left;
				GetRightCells(cellUnderMouse, dockLeft).ForEach(x => x.Element.Column++);
				element.Column = cellUnderMouse.Element.Column + (dockLeft ? 0 : 1);				
			} else {
				var index = Cells.LastOrDefault(x => Helper.Do(()=>x.RenderRect.Right, ()=>x.RenderRect.Bottom) <= Helper.GetX(relativeToSelfPoint)).Return(x => x.Element.Column + 1, () => 0);
				var cellByIndex = Cells.If(x => x.IsValidIndex(index)).With(x => x[index]);
				if (cellByIndex != null) {
					foreach (var cell in GetRightCells(cellByIndex, true)) {
						cell.Element.Column++;
						if (elementWidth > 0) {
							var oldOffset = cell.Element.Offset;
							cell.Element.Offset -= Math.Min(oldOffset, elementWidth);
							elementWidth -= oldOffset - cell.Element.Offset;
						}
					}
				}
				element.Column = index;
			}
			currentCell = Add(element);
			element.Row = Index;
			currentCell.Element.Offset = Math.Max(0, Helper.GetX(relativeToSelfPoint) - currentCell.PreviousSibling.Return(x => Helper.Do(() => x.RenderRect.Right, () => x.RenderRect.Bottom), () => 0));
			var maxOffset = Helper.GetWidth(Table.RenderSize) - GetRightCells(currentCell, false).Sum(x => Helper.GetWidth(x.InfiniteSize)) - Helper.GetWidth(elementInfiniteSize);
			maxOffset = Math.Max(0, maxOffset);
			currentCell.Element.Offset = Math.Min(maxOffset, currentCell.Element.Offset);
			oldRow.Reorder();
			Reorder();
		}
		protected virtual void DragCellInsideRow(BarLayoutTableCell currentCell, Point relativeToSelfPoint, Point delta) {
			if (MoveCellsInsideRow(currentCell, relativeToSelfPoint, delta))
				return;
			var deltaX = delta.X;
			if (deltaX > 0 && currentCell.NextSibling == null || deltaX < 0 && currentCell.PreviousSibling == null)
				return;
			var relativeToTablePoint = GetTablePosition(relativeToSelfPoint);
			var cellUnderMouse = Table.FindCell(relativeToTablePoint);
			if (cellUnderMouse == null)
				return;
			var dock = cellUnderMouse.GetDock(relativeToTablePoint);
			if(Table.Orientation == Orientation.Horizontal) {
				if (deltaX > 0 && dock != Dock.Right || deltaX < 0 && dock != Dock.Left)
					return;
			} else {
				if (deltaX > 0 && dock != Dock.Bottom || deltaX < 0 && dock != Dock.Top)
					return;
			}
			var oldIndex = currentCell.Element.Column;
			currentCell.Element.Column = cellUnderMouse.Element.Column;
			cellUnderMouse.Element.Offset = currentCell.Element.Offset;
			cellUnderMouse.Element.Column = oldIndex;
			currentCell.Element.Offset = 0d;
			Reorder();			
		}
		protected virtual bool MoveCellsInsideRow(BarLayoutTableCell currentCell, Point relativeToSelfPoint, Point delta) {
			var deltaValue = delta.X;
			if (deltaValue > 0) {
				var rightCells = GetRightCells(currentCell);
				var maxRightOffset = Helper.GetWidth(Table.RenderSize) 
					- rightCells.Sum(x => Helper.GetWidth(x.InfiniteSize)) 
					- Helper.Do(()=>currentCell.RenderRect.Right - currentCell.Element.RenderSize.Width + currentCell.InfiniteSize.Width, ()=>currentCell.RenderRect.Bottom - currentCell.Element.RenderSize.Height + currentCell.InfiniteSize.Height);
				var coerceddeltaX = Math.Min(maxRightOffset, deltaValue);
				coerceddeltaX = Math.Max(0, coerceddeltaX);
				bool shouldSwapBars = coerceddeltaX != deltaValue;
				currentCell.Element.Offset += coerceddeltaX;
				foreach (var cell in rightCells) {
					if (coerceddeltaX <= 0)
						break;
					var oldElementOffset = cell.Element.Offset;
					cell.Element.Offset = Math.Max(0, cell.Element.Offset - coerceddeltaX);
					coerceddeltaX -= oldElementOffset - cell.Element.Offset;
				}
				if (!shouldSwapBars)
					return true;
			} else {
				deltaValue = Math.Abs(deltaValue);
				var leftCells = GetLeftCells(currentCell, true).Reverse();
				var maxLeftOffset = leftCells.Sum(x => x.Element.Offset);
				var coerceddeltaX = Math.Min(maxLeftOffset, deltaValue);
				bool shouldSwapBars = coerceddeltaX != deltaValue;
				foreach (var cell in leftCells) {
					if (coerceddeltaX <= 0)
						break;
					var oldElementOffset = cell.Element.Offset;
					cell.Element.Offset = Math.Max(0, cell.Element.Offset - coerceddeltaX);
					coerceddeltaX -= oldElementOffset - cell.Element.Offset;
				}
				var right = currentCell.NextSibling;
				if (right != null) {
					right.Element.Offset += Math.Min(maxLeftOffset, deltaValue) - coerceddeltaX;
				}
				if (!shouldSwapBars)
					return true;
			}
			return false;
		}
		public virtual void IncrementIndexes() {
			Index++;
			foreach(var cell in Cells) {
				cell.Element.Row++;
			}
		}
	}
	public class BarLayoutTableCell {
		readonly BarLayoutTableRow row;
		readonly BarLayoutTable table;
		IBarLayoutTableInfo element;
		protected bool IsFirst { get { return row.IsFirstColumn(this); } }
		protected bool IsLast { get { return row.IsLastColumn(this); } }
		protected double ColumnIndent { get { return IsFirst ? 0d : table.ColumnIndent; } }
		protected virtual double DockPercent { get { return 0.2d; } }
		public BarLayoutTable Table { get { return table; } }
		public BarLayoutTableRow Row { get { return row; } }
		public bool IsVisible { get { return Element != null && Element.IsVisible; } }
		public IBarLayoutTableInfo Element {
			get { return element; }
			set {
				if (element == value)
					return;
				var oldValue = element;
				element = value;
				OnElementChanged(oldValue, element);
			}
		}		
		public Size DesiredSize { get; private set; }
		public Size RenderSize { get; private set; }
		public Rect RenderRect { get; private set; }
		public bool CanReduce { get { return GetCanReduce(); } }		
		public bool CanExpand { get; private set; }
		protected double ActualOffset { get; set; }
		protected double Offset { get { return Table.IgnoreElementOffset ? 0d : Element.Offset; } }
		public double Constraint { get; set; }		
		public Size MinDesiredSize { get { return CanReduce ? new Size() : DesiredSize; } }
		public bool UseWholeRow { get { return Element.UseWholeRow; } }
		public bool Fill { get { return IsLast ? (Element.UseWholeRow || table.LastChildFill) : false; } }
		public BarLayoutTableCell NextSibling { get { return Row.GetNextSibling(this); } }
		public BarLayoutTableCell PreviousSibling { get { return Row.GetPreviousSibling(this); } }
		public Size InfiniteSize { get; private set; }
		public BarLayoutTableCell(BarLayoutTableRow row, BarLayoutTable table) {
			this.row = row;
			this.table = table;
			DesiredSize = new Size();
			RenderSize = new Size();
			RenderRect = new Rect();
		}
		bool forcedCanReduce = true;
		public virtual void Measure() {
			forcedCanReduce = true;
			var originConstraint = Constraint;
			Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			InfiniteSize = Element.DesiredSize;
			if (Constraint < 5d) {
				var width = InfiniteSize.Width;
				var height = InfiniteSize.Height;
				while (true) {
					if (width < 1d)
						break;
					Element.Measure(new Size(width-1, height));
					if (Element.DesiredSize.Width == width - 1)
						break;
					else width = Element.DesiredSize.Width;
				}
				Constraint = width;
			}
			var minSize = Math.Max(0, Math.Min(InfiniteSize.Width + ColumnIndent, Constraint));
			var maxSize = Math.Max(0, Constraint - Offset - ColumnIndent);
			var targetSize = Math.Max(minSize, maxSize);
			var measureSize = CreateSize(targetSize, double.PositiveInfinity);
			Element.Measure(measureSize);
			var horizontalOffset = Math.Max(0, Math.Min(Offset + ColumnIndent, Constraint - Element.DesiredSize.Width));
			var verticalOffset = Math.Max(0, Math.Min(Offset + ColumnIndent, Constraint - Element.DesiredSize.Height));
			if (Row.Table.Orientation == Orientation.Horizontal) {
				ActualOffset = horizontalOffset;
				verticalOffset = 0d;
			} else {
				ActualOffset = verticalOffset;
				horizontalOffset = 0d;
			}				
			DesiredSize = new Size(Element.DesiredSize.Width + horizontalOffset, Element.DesiredSize.Height + verticalOffset);
			if (originConstraint < DesiredSize.Width)
				forcedCanReduce = false;
			CanExpand = InfiniteSize.Width > DesiredSize.Width;
		}
		bool GetCanReduce() {
			return Element.CanReduce && forcedCanReduce;
		}
		public virtual void Arrange(Rect finalRect) {			
			var horizontalOffset = Math.Min(Offset + ColumnIndent, Math.Max(0, finalRect.Size.Width - Element.DesiredSize.Width));
			var verticalOffset = Math.Min(Offset + ColumnIndent, Math.Max(0, finalRect.Size.Height - Element.DesiredSize.Height));
			if (Row.Table.Orientation == Orientation.Horizontal) {
				verticalOffset = 0d;
			} else
				horizontalOffset = 0d;
			Point leftTop = new Point(finalRect.Left + horizontalOffset, finalRect.Top + verticalOffset);
			Size size = new Size(finalRect.Size.Width - horizontalOffset, finalRect.Size.Height - verticalOffset);
			Element.Arrange(new Rect(leftTop, size));
			RenderSize = new Size(Element.RenderSize.Width + horizontalOffset, Element.RenderSize.Height + verticalOffset);
			RenderRect = new Rect(leftTop, Element.RenderSize);
		}
		Size CreateSize(double define, double secondary) {
			if (Table.Orientation == Orientation.Horizontal) {
				return new Size(define, secondary);
			}
			return new Size(secondary, define);
		}
		Size MakeValid(Size sz) {
			return new Size(Math.Max(0, sz.Width), Math.Max(0, sz.Height));
		}		
		protected virtual void OnElementChanged(IBarLayoutTableInfo oldValue, IBarLayoutTableInfo newValue) {
			if (oldValue != null) {
				oldValue.LayoutPropertyChanged -= LayoutPropertyChanged;
			}				
			if (newValue != null) {
				newValue.LayoutPropertyChanged += LayoutPropertyChanged;
			}				
		}		
		protected virtual void LayoutPropertyChanged(object sender, EventArgs e) {
			Table.NeedsUpdate = true;
		}		
		public virtual void UpdateIndex(int rowIndex, int columnIndex) {
			Element.Row = rowIndex;
			Element.Column = columnIndex;
		}
		public Dock? GetDock(Point mousePosition) {
			var leftRect = new Rect(
				RenderRect.Left,
				RenderRect.Top + RenderSize.Height * DockPercent,
				RenderSize.Width * DockPercent,
				RenderSize.Height * (1d - DockPercent * 2));
			var left = (leftRect.Center() - mousePosition).Length;
			var topRect = new Rect(
				RenderRect.Left + RenderSize.Width * DockPercent,
				RenderRect.Top,
				RenderSize.Width * (1d - 2 * DockPercent),
				RenderSize.Height * DockPercent);
			var top = (topRect.Center() - mousePosition).Length;
			var rightRect = new Rect(
				RenderRect.Left + RenderSize.Width * (1d - DockPercent),
				RenderRect.Top + RenderSize.Height * DockPercent,
				RenderSize.Width * DockPercent,
				RenderSize.Height * (1d - DockPercent * 2));
			var right = (rightRect.Center() - mousePosition).Length;
			var bottomRect = new Rect(
				RenderRect.Left + RenderSize.Width * DockPercent,
				RenderRect.Top + RenderSize.Height * (1d - DockPercent * 2),
				RenderSize.Width * (1d - 2 * DockPercent),
				RenderSize.Height * DockPercent);
			var bottom = (bottomRect.Center() - mousePosition).Length;
			var centerRect = new Rect(
				RenderRect.Left + RenderSize.Width * DockPercent,
				RenderRect.Top + RenderSize.Height * DockPercent,
				RenderSize.Width * (1d - 2 * DockPercent),
				RenderSize.Height * (1d - 2 * DockPercent));
			var center = (centerRect.Center() - mousePosition).Length;
			var min = new double[] { left, top, right, bottom, center }.Min();
			if (min == left && Element.CanDock(Dock.Left))
				return Dock.Left;
			if (min == top && Element.CanDock(Dock.Top))
				return Dock.Top;
			if (min == right && Element.CanDock(Dock.Right))
				return Dock.Right;
			if (min == bottom && Element.CanDock(Dock.Bottom))
				return Dock.Bottom;
			return null;
		}
	}
	public abstract class BaseBarLayoutCalculator {
		Orientation orientation;
		readonly BarContainerControlPanel container;
		public BarContainerControlPanel Container { get { return container; } }
		public Orientation Orientation {
			get { return orientation; }
			set {
				if (value == orientation) return;
				Orientation oldValue = orientation;
				orientation = value;
				OnOrientationChanged(oldValue);
			}
		}
		public abstract Size ArrangeContainer(Size finalSize);
		public abstract Size MeasureContainer(Size constraint);
		public abstract void OnBarControlDrag(IBarLayoutTableInfo barLayoutTableInfo, DragDeltaEventArgs e, bool? move = null);
		public abstract void OnBarControlDragCompleted(IBarLayoutTableInfo barLayoutTableInfo, DragCompletedEventArgs e);
		public abstract void OnBarControlDragStart(IBarLayoutTableInfo barLayoutTableInfo, DragStartedEventArgs e);
		protected abstract void OnOrientationChanged(Orientation oldValue);
		public abstract bool CheckBarDocking(FloatingBarPopup popup);
		public abstract void InsertFloatBar(Bar bar, bool v);
		public BaseBarLayoutCalculator(BarContainerControlPanel container) {
			this.container = container;
		}
	}
	public class BarLayoutCalculator2 : BaseBarLayoutCalculator {		
		static double FloatBarVerticalIndent = 10;
		static double FloatBarHorizontalIndent = 10;
		readonly BarLayoutTable table;		
		public BarLayoutTable LayoutTable { get { return table; } }
		public BarLayoutCalculator2(BarContainerControlPanel container) : base(container) {
			this.table = CreateLayoutTable();
			container.VisualChildrenChanged += OnContainerChildrenChanged;
			table.RequestMeasure += OnRequestMeasure;
		}		
		protected virtual BarLayoutTable CreateLayoutTable() {
			bool ignoreOffset = Container.With(x => x.Owner as FloatingBarContainerControl).ReturnSuccess();
			return new BarLayoutTable(this) { IgnoreElementOffset = ignoreOffset };
		}
		protected override void OnOrientationChanged(Orientation oldValue) {
			LayoutTable.Orientation = Orientation;
		}
		void OnContainerChildrenChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
				OnElementAdded((IBarLayoutTableInfo)e.NewItems[0]);
			else
				OnElementRemoved((IBarLayoutTableInfo)e.OldItems[0]);
		}
		protected virtual void OnRequestMeasure(object sender, EventArgs e) {
			Container.InvalidateMeasure();
		}
		protected virtual void OnElementRemoved(IBarLayoutTableInfo element) { LayoutTable.Remove(element); }
		protected virtual void OnElementAdded(IBarLayoutTableInfo element) { LayoutTable.Add(element); }
		public override Size ArrangeContainer(Size finalSize) {
			table.Arrange(finalSize);
			return finalSize;
		}
		public override Size MeasureContainer(Size constraint) {
			var ic = Container.Children;
			table.Measure(constraint);
			return table.DesiredSize;
		}										  
		public override void OnBarControlDrag(IBarLayoutTableInfo layoutInfo, DragDeltaEventArgs e, bool? move = null) {						
			var mousePosition = Mouse.GetPosition(Container);
			LayoutTable.UpdateIndexes();
			var currentCell = LayoutTable.FindCell(layoutInfo);
			if (currentCell == null)
				return;
			if (CheckMakeBarFloating(currentCell))
				return;
			LayoutTable.DragCell(currentCell, mousePosition, Mouse.GetPosition((IInputElement)e.Source));
		}
		protected virtual bool CheckMakeBarFloating(BarLayoutTableCell currentCell) {
			var layoutInfo = currentCell.Element;
			var skipDragRect = new Rect(-FloatBarHorizontalIndent, -FloatBarVerticalIndent, Container.RenderSize.Width + FloatBarHorizontalIndent * 2, Container.RenderSize.Height + FloatBarVerticalIndent * 2);
			var width = layoutInfo.RenderSize.Width;
			var ns = currentCell.NextSibling.With(x => x.Element);
			if (ns != null) {
				ns.Offset += width;
			}
			if (!skipDragRect.Contains(Mouse.GetPosition(Container)) && MakeFloating(layoutInfo)) {
				layoutInfo.Offset = 0d;
				return true;
			}			
			if (ns != null)
				ns.Offset -= width;
			return false;						
		}
		public override void OnBarControlDragCompleted(IBarLayoutTableInfo layoutInfo, DragCompletedEventArgs e) { }
		public override void OnBarControlDragStart(IBarLayoutTableInfo layoutInfo, DragStartedEventArgs e) { }
		public override bool CheckBarDocking(FloatingBarPopup popup) {
			var floatContainer = popup.Child as FrameworkElement;
			if (floatContainer == null)
				return false;
			var floatContainerPoint = floatContainer.PointToScreen(floatContainer.IsMouseOver ? Mouse.GetPosition(floatContainer) : new Point());
			var leftTop = Container.PointToScreen(new Point());
			if (Container.FlowDirection == FlowDirection.RightToLeft)
				leftTop = new Point(leftTop.X - Container.RenderSize.Width, leftTop.Y);
			var renderSize = Container.RenderSize;
			if (Container.RenderSize.Width == 0d) {
				renderSize.Width = 10d;
				leftTop.X -= 5d;
			}
			if(Container.RenderSize.Height == 0d) {
				renderSize.Height = 10d;
				leftTop.Y -= 5d;
			}
			var layoutTableRect = new Rect(leftTop, renderSize);			
			return layoutTableRect.Contains(floatContainerPoint);
		}
		public override void InsertFloatBar(Bar bar, bool startDrag) {
			if (bar == null)
				return;			
			if (bar.DockInfo.Container != null) {
				bar.DockInfo.Container.Unlink(bar);
			}
			LayoutTable.InsertFloatBar(bar, Mouse.GetPosition(Container));
			Container.Owner.Link(bar);			
			if (startDrag) {
				LayoutTable.InsertFloatBar(bar, Mouse.GetPosition(Container));
				Container.InvalidateMeasure();				
				ContextLayoutManagerHelper.UpdateLayout();
				StartDrag(bar);
			}
		}
		void StartDrag(Bar bar) {
			if (Mouse.LeftButton == MouseButtonState.Pressed) {
				var dWidget = bar.With(x => x.DockInfo).With(x => x.BarControl).With(x => x.DragWidget);
				if (dWidget == null)
					return;
				var lInfo = GetLayoutInfo(dWidget);
				OnBarControlDrag(lInfo, new DragDeltaEventArgs(0d, 0d) { Source = dWidget }, false);
				var ltc = lInfo.With(LayoutTable.FindCell);
				if (ltc != null && ltc.NextSibling!=null) {
					ltc.NextSibling.Element.Offset -= ltc.DesiredSize.Width;
				}				
				dWidget.StartDrag();
			}
		}
		static IBarLayoutTableInfo GetLayoutInfo(object sender) {
			var bc = LayoutHelper.FindParentObject<BarControl>((DependencyObject)sender);
			if (bc == null) return null;
			return bc.TemplatedParent as IBarLayoutTableInfo ?? bc;
		}
		public static FloatingBarPopup CreateFloatingBar(BarDockInfo barDockInfo, Point floatBarOffset) {
			FloatingBarPopup popup = new FloatingBarPopup();
			popup.PlacementTarget = barDockInfo.Bar.With(BarNameScope.GetScope).With(x => x.Target as UIElement) ?? Window.GetWindow(barDockInfo.Bar);
			popup.HorizontalOffset = floatBarOffset.X;
			popup.VerticalOffset = floatBarOffset.Y;
			popup.Bar = barDockInfo.Bar;
			barDockInfo.FloatingPopup = popup;
			return popup;
		}
		protected virtual bool MakeFloating(IBarLayoutTableInfo layoutInfo) {
			return layoutInfo.MakeFloating();
		}
	}
	public class FloatingBarLayoutCalculator : BaseBarLayoutCalculator {
		public FloatingBarLayoutCalculator(BarContainerControlPanel container) : base(container) {
		}
		public override Size MeasureContainer(Size constraint) { GetChildren(); return LayoutHelper.MeasureElementWithSingleChild(Container, constraint); }
		object GetChildren() { return Container.Children; }
		public override Size ArrangeContainer(Size finalSize) { return LayoutHelper.ArrangeElementWithSingleChild(Container, finalSize); }		
		public override bool CheckBarDocking(FloatingBarPopup popup) { return false; }		
		public override void OnBarControlDrag(IBarLayoutTableInfo barLayoutTableInfo, DragDeltaEventArgs e, bool? move = default(bool?)) { }
		public override void OnBarControlDragCompleted(IBarLayoutTableInfo barLayoutTableInfo, DragCompletedEventArgs e) { }
		public override void OnBarControlDragStart(IBarLayoutTableInfo barLayoutTableInfo, DragStartedEventArgs e) { }
		protected override void OnOrientationChanged(Orientation oldValue) { }
		public override void InsertFloatBar(Bar bar, bool v) { }
	}
}

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
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.XtraPivotGrid.Selection;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Data;
using DevExpress.Xpf.Utils;
using Keys = System.Windows.Input.Key;
#endif
namespace DevExpress.XtraPivotGrid.Data {
	public class SelectionVisualItems : PivotVisualItemsBase, IMultipleSelection {
		public static Point EmptyCoord = new Point(-1000, -1000);
		readonly IPivotClipboardAccessor pivotGridClipboard;
		Point focusedCell, previousCell, startSelectionPoint;
		PivotGridSelection innerSelection;
		PivotWholeColumnSelection wholeColumnSelection;
		int selectionLock;
		bool isSelectionStartedFromSelectedCell, isCellMouseDown;
		bool isValueMouseDown;
		PivotArea valueArea;
		int valueStartIndexMin, valueStartIndexMax;
		PivotFieldValueItem valuePrevItem;
		WeakEventHandler<FocusedCellChangedEventArgs, EventHandler<FocusedCellChangedEventArgs>> focusedCellChanged;
		WeakEventHandler<EventArgs, EventHandler> selectionChanged;
		public SelectionVisualItems(PivotGridData data)
			: base(data) {
			this.pivotGridClipboard = data.pivotClipboard;
		}
		public virtual Point FocusedCell {
			get { return focusedCell; }
			set { SetFocusedCell(value, true); }
		}
		protected internal PivotGridSelection InnerSelection {
			get {
				if(innerSelection == null)
					innerSelection = new PivotGridSelection();
				return innerSelection;
			}
		}
		protected internal PivotWholeColumnSelection WholeColumnSelection {
			get {
				if(wholeColumnSelection == null)
					wholeColumnSelection = new PivotWholeColumnSelection(this);
				return wholeColumnSelection;
			}
		}
		public Rectangle Selection {
			get { return Data.OptionsSelection.CellSelection ? InnerSelection.Rectangle : Rectangle.Empty; }
			set {
				SetSelectionCore(ref value, false);
			}
		}
		void SetSelectionCore(ref Rectangle value, bool fullColumnRow) {
			if(IsSelectionLocked || !Data.OptionsSelection.CellSelection || Selection.Equals(value) || Data.DataFieldCount == 0)
				return;
			LockSelection();
			Rectangle oldSelection = InnerSelection.Rectangle;
			if(!fullColumnRow && IsSelectionEqualsFocusedCell(value)) {
				InnerSelection.Clear();
				FocusedCell = value.Location;
			} else
				InnerSelection.Rectangle = value;
			CorrectSelection(oldSelection);
			if(!Selection.IsEmpty && !IsFocusedCellSelectionCorner(InnerSelection.LastSelection.Rectangle)) {
				SetFocusedCell(InnerSelection.LastSelection.Rectangle.Location, false);
			}
			UnlockSelection();
			OnSelectionChanged();
		}
		public Rectangle FullSelection {
			get {
				Rectangle bounds = SelectedCells.Rectangle;
				return bounds;
			}
		}
		public ReadOnlyCells SelectedCells {
			get { return Data.OptionsSelection.CellSelection ? InnerSelection.Cells : ReadOnlyCells.Empty; }
		}
#if DXPORTABLE
		protected virtual Keys KeysState { get { return Keys.None; } }
		public bool IsControlDown { get { return false; } }
		public bool IsShiftDown { get { return false; } }
#elif !SL
		protected virtual Keys KeysState { get { return Control.ModifierKeys; } }
		public bool IsControlDown { get { return KeysState == Keys.Control; } }
		public bool IsShiftDown { get { return KeysState == Keys.Shift; } }
#else
		public bool IsControlDown { get { return KeyboardHelper.IsControlPressed; } }
		public bool IsShiftDown { get { return KeyboardHelper.IsShiftPressed; } }
#endif
		public bool IsMultiSelect { get { return Data.OptionsSelection.MultiSelect; } }
		public bool IsMultiSelectControlDown { get { return (IsMultiSelect && IsControlDown); } }
		public bool IsCellMouseDown { get { return isCellMouseDown; } }
		public bool IsValueMouseDown { get { return isValueMouseDown; } }
		public PivotArea ValueSelectionArea { get { return valueArea; } }
		protected virtual int ViewportHeight { get { return 1; } }
		protected Point StartSelectionPoint {
			get { return startSelectionPoint; }
			set {
				startSelectionPoint = value;
				isSelectionStartedFromSelectedCell = SelectedCells.Contains(StartSelectionPoint);
			}
		}
		protected bool? IsColumnRowSelection { get { return IsValueMouseDown ? new bool?(valueArea == PivotArea.ColumnArea) : null; } }
		public event EventHandler<FocusedCellChangedEventArgs> FocusedCellChanged {
			add { focusedCellChanged += value; }
			remove { focusedCellChanged -= value; }
		}
		public event EventHandler SelectionChanged {
			add { selectionChanged += value; }
			remove { selectionChanged -= value; }
		}
		protected override void Calculate() {
			base.Calculate();
			if(!IsDisposed && wholeColumnSelection != null)
				wholeColumnSelection.Reset();
		}
		public override void Clear() {
			base.Clear();
			if(wholeColumnSelection != null)
				wholeColumnSelection.Reset();
		}
		protected bool IsSelectionLocked { get { return selectionLock != 0; } }
		protected void LockSelection() { selectionLock++; }
		protected void UnlockSelection() {
			selectionLock--;
			if(selectionLock < 0)
				throw new Exception("unpaired UnlockSelection call");
		}
		public void SetFocusedCell(Point value, bool makeVisible) {
			SetFocusedCell(value, makeVisible, !IsMultiSelectControlDown, true);
		}
		public void SetFocusedCell(Point value, bool makeVisible, bool clearSelection, bool changeStartSelectionPoint) {
			value = CorrectFocusedCell(value);
			if(FocusedCell == value) return;
			Point oldValue = this.focusedCell;
			this.focusedCell = value;
			if(changeStartSelectionPoint)
				startSelectionPoint = value;
			if(clearSelection)
				ClearSelection();
			if(makeVisible)
				Data.MakeCellVisible(FocusedCell);
			OnFocusedCellChanged(oldValue, this.focusedCell);
		}
		void ClearSelection() {
			Selection = Rectangle.Empty;
		}
		public void AddSelection(Rectangle selection) {
			if(!Data.OptionsSelection.CellSelection) return;
			Rectangle oldSelection = Selection;
			if(isSelectionStartedFromSelectedCell) {
				InnerSelection.LoadLastState();
				if(!InnerSelection.IsEmpty)
					InnerSelection.Subtract(selection);
				else
					InnerSelection.Add(selection);
			} else
				InnerSelection.Add(selection);
			CorrectSelection(oldSelection);
			OnSelectionChanged();
		}
		public void SubtractSelection(Rectangle selection) {
			InnerSelection.Subtract(selection);
			OnSelectionChanged();
		}
		public void MoveSelectionTo(Point selectionTo) {
			if(!Data.OptionsSelection.CellSelection || startSelectionPoint == EmptyCoord) return;
			Rectangle selectionRectangle = new Rectangle(Math.Min(selectionTo.X, startSelectionPoint.X),
				Math.Min(selectionTo.Y, startSelectionPoint.Y),
				Math.Abs(selectionTo.X - startSelectionPoint.X) + 1, Math.Abs(selectionTo.Y - startSelectionPoint.Y) + 1);
			SetFocusedCell(selectionTo, false, false, false);
			AddSelection(selectionRectangle);
		}
		public void SetSelection(params Point[] points) {
			if(!Data.OptionsSelection.CellSelection) return;
			InnerSelection.SetSelection(points);
			OnSelectionChanged();
		}
		public void SetSelection() {
			if(!Data.OptionsSelection.CellSelection)
				return;
			InnerSelection.StartSelection(false);
			OnSelectionChanged();
		}
		public void SetColumnRowSelection(bool isColumn, int index, bool isExpanding) {
			PivotFieldValueItem item = GetItem(isColumn, index);
			if(item == null) return;
			if(isExpanding && item.IsTotal)
				item = GetInvisibleItemByItemTotal(isColumn, item);
			bool isTotalsFar = Data.OptionsView.IsTotalsFar(isColumn, item.ValueType);
			int count = item.MaxLastLevelIndex - item.MinLastLevelIndex + 1,
				minIndex = item.MinLastLevelIndex - (!isTotalsFar && !isExpanding ? 1 : 0);
			Selection = isColumn ? new Rectangle(minIndex, 0, count, RowCount) : new Rectangle(0, minIndex, ColumnCount, count);
		}
		public void ChangeExpanded(bool isColumn, PivotFieldValueItem item) {
			if(!item.AllowExpand || !item.AllowExpandOnDoubleClick) return;
			bool isTotal = item.IsTotal && item.ValueType != PivotGridValueType.GrandTotal;
			item = isTotal ? GetItemByItemTotal(isColumn, item) : item;
			if(item == null) return;
			ChangeExpandedCore(item, delegate(AsyncOperationResult result) {
				EnsureIsCalculated();
				SetColumnRowSelection(isColumn, item.Index, item.IsCollapsed);
			});
		}
		protected virtual void ChangeExpandedCore(PivotFieldValueItem item, AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpanded(item);
			asyncCompleted.Invoke(null);
		}
		PivotFieldValueItem GetItemByItemTotal(bool isColumn, PivotFieldValueItem itemTotal) {
			int itemCount = GetItemCount(isColumn);
			for(int i = 0; i < itemCount; i++) {
				PivotFieldValueItem item = GetItem(isColumn, i);
				if(item == itemTotal) continue;
				if(CheckIndexes(item, itemTotal) && (item.EndLevel < itemTotal.EndLevel)) {
					return item;
				}
			}
			return null;
		}
		PivotFieldValueItem GetInvisibleItemByItemTotal(bool isColumn, PivotFieldValueItem itemTotal) {
			int itemCount = GetItemCount(isColumn);
			for(int i = 0; i < itemCount; i++) {
				PivotFieldValueItem item = GetItem(isColumn, i);
				if(item == itemTotal) continue;
				if(CheckIndexes(item, itemTotal) && (!item.IsVisible || !item.IsRowTree)) {
					return item;
				}
			}
			return null;
		}
		bool CheckIndexes(PivotFieldValueItem item, PivotFieldValueItem itemTotal) {
			return item.DataIndex == itemTotal.DataIndex && item.VisibleIndex == itemTotal.VisibleIndex;
		}
		public void StartSelection(bool isFieldValueSelection) {
			InnerSelection.StartSelection(IsMultiSelect, IsControlDown, IsShiftDown, isFieldValueSelection);
			OnSelectionChanged();
		}
		public void SelectAllColumnRow(Rectangle columnRow) {
				if(IsMultiSelect && IsControlDown) {
					AddSelection(columnRow);
				}
				else {
					if(IsMultiSelect && IsShiftDown) {
						if(columnRow.Height == 1) {
							Rectangle selection = new Rectangle(
								columnRow.X, Math.Min(columnRow.Y, StartSelectionPoint.Y),
								columnRow.Width, Math.Max(columnRow.Bottom, StartSelectionPoint.Y) - Math.Min(columnRow.Y, StartSelectionPoint.Y));
							AddSelection(selection);
						}
						if(columnRow.Width == 1) {
							Rectangle selection = new Rectangle(
								Math.Min(columnRow.X, StartSelectionPoint.X), StartSelectionPoint.Y,
								Math.Max(columnRow.Right, StartSelectionPoint.X) - Math.Min(columnRow.X, StartSelectionPoint.X), columnRow.Height);
							AddSelection(selection);
						}
					}
					else
						SetSelectionCore(ref columnRow, true);
				}
		}
		public Point GetKeyDownNextFocusedCell() {
			if(!IsShiftDown) return FocusedCell;
			return GetNextFocusedCellCore();
		}
		Point GetNextFocusedCellCore() {
			if(Selection.IsEmpty) return startSelectionPoint;
			Rectangle lastSelection = InnerSelection.LastSelection.Rectangle;
			return new Point(lastSelection.X == startSelectionPoint.X ? lastSelection.Right - 1 : lastSelection.X,
				lastSelection.Y == startSelectionPoint.Y ? lastSelection.Bottom - 1 : lastSelection.Y);
		}
		public bool IsCellSelected(PivotGridCellItem cell) {
			return IsCellSelected(cell.ColumnIndex, cell.RowIndex);
		}
		public bool IsCellSelected(int columnIndex, int rowIndex) {
			return SelectedCells.Contains(new Point(columnIndex, rowIndex));
		}
		public bool IsCellFocused(PivotGridCellItem cell) {
			return IsCellFocused(cell.ColumnIndex, cell.RowIndex);
		}
		public bool IsCellFocused(int columnIndex, int rowIndex) {
			return columnIndex == FocusedCell.X && rowIndex == FocusedCell.Y;
		}
		public bool IsWholeColumnSelected(int columnIndex) {
			return WholeColumnSelection.IsWholeColumnSelected(columnIndex);
		}
		public bool IsWholeRowSelected(int rowIndex) {
			return WholeColumnSelection.IsWholeRowSelected(rowIndex);
		}
		public bool IsFieldValueSelected(PivotFieldValueItem valueItem) {
			if(Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree) {
				if(!valueItem.IsLastFieldLevel)
					return false;
				if(valueItem.IsColumn)
					return IsWholeColumnSelected(valueItem.MinLastLevelIndex);
				else
					return IsWholeRowSelected(valueItem.MinLastLevelIndex);
			} else {
				if(valueItem.IsColumn) {
					for(int i = valueItem.MinLastLevelIndex; i <= valueItem.MaxLastLevelIndex; i++)
						if(!IsWholeColumnSelected(i))
							return false;
				} else {
					for(int i = valueItem.MinLastLevelIndex; i <= valueItem.MaxLastLevelIndex; i++)
						if(!IsWholeRowSelected(i))
							return false;
				}
				return true;
			}
		}
		private static Rectangle PointsToRect(Point start, Point end) {
			int x1 = Math.Min(start.X, end.X);
			int y1 = Math.Min(start.Y, end.Y);
			int x2 = Math.Max(start.X, end.X);
			int y2 = Math.Max(start.Y, end.Y);
			return new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
		}
		public void OnCellMouseDown(Point cell) {
			if(!IsShiftDown)
				StartSelectionPoint = cell;
			if(!IsCellValid(cell)) return;
			if(!InnerSelection.IsLastSelectionIsShiftDown) {
				StartSelection(false);
				if(IsShiftDown) {
					SetSelection();
					StartSelection(false);
					StartSelectionPoint = FocusedCell;
				}
			} else
				if(!IsShiftDown)
					StartSelection(false);
			Point oldFocusedCell = FocusedCell;
			SetFocusedCell(cell, false, false, false);
			isCellMouseDown = true;
			if(!Data.OptionsSelection.CellSelection) return;
			if(IsShiftDown && oldFocusedCell != cell) {
				Point startPoint = StartSelectionPoint == EmptyCoord ? FocusedCell : StartSelectionPoint;
				AddSelection(PointsToRect(startPoint, cell));
			} else if(IsMultiSelect && IsControlDown && oldFocusedCell != cell &&
					!SelectedCells.Contains(oldFocusedCell) && SelectedCells.Count == 0) {
				AddSelection(new Rectangle(oldFocusedCell.X, oldFocusedCell.Y, 1, 1));
				StartSelection(false);
			}
			if(IsMultiSelect && IsControlDown) {
				AddSelection(new Rectangle(cell.X, cell.Y, 1, 1));
			}
			if(!IsMultiSelect && !IsShiftDown)
				ClearSelection();
			previousCell = cell;
		}
		public void OnCellMouseUp() {
			isCellMouseDown = false;
		}
		public void OnCellMouseMove(Point currentCell) {
			if(currentCell != previousCell || (previousCell == EmptyCoord && currentCell != FocusedCell))
				MoveSelectionTo(currentCell);
			previousCell = currentCell;
		}
		public void OnCellGestureTwoFingerSelection(Point cellStart, Point cellEnd) {
			if(!IsCellValid(cellStart)) return;
			StartSelection(false);
			SetFocusedCell(cellStart, false, false, false);
			if(!Data.OptionsSelection.CellSelection) return;
			AddSelection(PointsToRect(cellStart, cellEnd));
		}
		public bool OnKeyDown(int virtualKey, bool control, bool shift) {
			Keys keyCode = (Keys)virtualKey;
			Point newFocusedCell = GetKeyDownNextFocusedCell();
			bool handled = false;
			switch(keyCode) {
				case Keys.Left:
					if(newFocusedCell.X == 0 && newFocusedCell.Y != 0) 
						newFocusedCell = new Point(ColumnCount - 1, newFocusedCell.Y - 1);					  
					else
						newFocusedCell = new Point(newFocusedCell.X - 1, newFocusedCell.Y);
					handled = true;
					break;
				case Keys.Down:
					newFocusedCell = new Point(newFocusedCell.X, newFocusedCell.Y + 1);
					handled = true;
					break;
				case Keys.Right:
				case Keys.Tab:
					if(newFocusedCell.X == ColumnCount - 1)
						newFocusedCell = new Point(0, newFocusedCell.Y + 1);					  
					else
						newFocusedCell = new Point(newFocusedCell.X + 1, newFocusedCell.Y);
					handled = true;
					break;
				case Keys.Up:
					newFocusedCell = new Point(newFocusedCell.X, newFocusedCell.Y - 1);
					handled = true;
					break;
				case Keys.Home:
					if(!control)
						newFocusedCell = new Point(0, newFocusedCell.Y);
					else
						newFocusedCell = new Point(newFocusedCell.X, 0);
					handled = true;
					break;
				case Keys.End:
					if(!control)
						newFocusedCell = new Point(ColumnCount - 1, newFocusedCell.Y);
					else
						newFocusedCell = new Point(newFocusedCell.X, RowCount - 1);
					handled = true;
					break;
				case Keys.A:
					if(control) {
						FocusedCell = Point.Empty;
						Selection = new Rectangle(0, 0, ColumnCount, RowCount);
						return true;
					}
					break;
				case Keys.PageDown:
					newFocusedCell = new Point(newFocusedCell.X, Math.Min(newFocusedCell.Y + ViewportHeight, RowCount - 1));
					handled = true;
					break;
				case Keys.PageUp:
					newFocusedCell = new Point(newFocusedCell.X, Math.Max(newFocusedCell.Y - ViewportHeight, 0));
					handled = true;
					break;
			}
			if(!newFocusedCell.Equals(FocusedCell) || !Selection.IsEmpty) {
				if(shift) {
					if(IsCellValid(newFocusedCell) && !(keyCode == Keys.Shift
#if !SL
 || keyCode == Keys.ShiftKey || keyCode == Keys.LShiftKey || keyCode == Keys.RShiftKey
#else
					  || keyCode == Keys.Shift
#endif
)) {
						Data.MakeCellVisible(newFocusedCell);
						MoveSelectionTo(newFocusedCell);
						handled = true;
					}
				} else {
					if(newFocusedCell != FocusedCell) {
						StartSelection(false);
						SetFocusedCell(newFocusedCell, true, true, true);
						handled = true;
					}
				}
			}
			return handled;
		}
		public void StartColumnRowSelection(PivotFieldValueItem item, Point startPoint) {
			if(!Data.OptionsSelection.CellSelection || Data.DataFieldCount == 0) return;
			if(!IsShiftDown)
				StartSelectionPoint = startPoint;
			isValueMouseDown = true;
			valueArea = item.Area;
			valueStartIndexMin = item.MinLastLevelIndex;
			valueStartIndexMax = item.MaxLastLevelIndex;
			valuePrevItem = null;
			StartSelectionPoint = FocusedCell;
			StartSelection(true);
			if(valueArea == PivotArea.RowArea) {
				isSelectionStartedFromSelectedCell = IsWholeRowSelected(item.MinLastLevelIndex);
				if(IsShiftDown) isSelectionStartedFromSelectedCell = false;
				SetFocusedCell(new Point(0, item.MinLastLevelIndex), false, false, false);
				SelectAllColumnRow(new Rectangle(0, item.MinLastLevelIndex, ColumnCount,
					item.MaxLastLevelIndex - item.MinLastLevelIndex + 1));
			} else {
				isSelectionStartedFromSelectedCell = IsWholeColumnSelected(item.MinLastLevelIndex);
				if(IsShiftDown) isSelectionStartedFromSelectedCell = false;
				SetFocusedCell(new Point(item.MinLastLevelIndex, 0), false, false, false);
				SelectAllColumnRow(new Rectangle(item.MinLastLevelIndex, 0, item.MaxLastLevelIndex -
					item.MinLastLevelIndex + 1, RowCount));
			}
		}
		public void StopColumnRowSelection() {
			if(isValueMouseDown)
				isValueMouseDown = false;
		}
		public void PerformColumnRowSelection(PivotFieldValueItem item) {
			if(item == null || item == valuePrevItem) return;
			int minIndex = Math.Min(valueStartIndexMin, item.MinLastLevelIndex),
				maxIndex = Math.Max(valueStartIndexMax, item.MaxLastLevelIndex);
			if(item.Area == PivotArea.RowArea) {
				SetFocusedCell(new Point(0, item.MinLastLevelIndex), false, false, false);
				SelectAllColumnRow(new Rectangle(0, minIndex, ColumnCount, maxIndex - minIndex + 1));
			} else {
				SetFocusedCell(new Point(item.MinLastLevelIndex, 0), false, false, false);
				SelectAllColumnRow(new Rectangle(minIndex, 0, maxIndex - minIndex + 1, RowCount));
			}
			valuePrevItem = item;
		}
		public void SetSelectionByFieldValues(bool isColumn, object[] values) {
			SetSelectionByFieldValues(isColumn, values, null);
		}
		public void SetSelectionByFieldValues(bool isColumn, object[] values, PivotFieldItemBase dataField) {
			IList<PivotFieldValueItem> items = GetItems(isColumn, values, dataField);
			foreach(PivotFieldValueItem item in items)
				SetSelectionByItem(item);
		}
		void SetSelectionByItem(PivotFieldValueItem item) {
			if(item == null)
				return;
			int minIndex = item.MinLastLevelIndex;
			int maxIndex = item.MaxLastLevelIndex;
			Rectangle selection = (item.Area == PivotArea.RowArea) ? new Rectangle(0, minIndex, ColumnCount, maxIndex - minIndex + 1) :
				new Rectangle(minIndex, 0, maxIndex - minIndex + 1, RowCount);
			InnerSelection.AddFieldValueSelection(selection);
			CorrectSelection();
		}
		public Point CorrectFocusedCell(Point pt) {
			if(pt.X >= ColumnCount) pt.X = ColumnCount - 1;
			if(pt.Y >= RowCount) pt.Y = RowCount - 1;
			if(pt.X < 0) pt.X = 0;
			if(pt.Y < 0) pt.Y = 0;
			return pt;
		}
		public void CorrectSelection() {
			SetFocusedCell(FocusedCell, true, false, true);
			CorrectSelection(Selection);
			if(InnerSelection.IsChanged)
				OnSelectionChanged();
		}
		protected void CorrectSelection(Rectangle lastSelection) {
			InnerSelection.CorrectSelection(ColumnCount, RowCount,
				Data.OptionsSelection.MaxWidth, Data.OptionsSelection.MaxHeight, lastSelection, IsColumnRowSelection);
		}
		public bool IsCellValid(Point cell) {
			return cell.X >= 0 && cell.X < ColumnCount && cell.Y >= 0 && cell.Y < RowCount && Data.DataFieldCount > 0;
		}
		protected bool IsSelectionEqualsFocusedCell(Rectangle selection) {
			return selection.Location == FocusedCell && selection.Width == 1 && selection.Height == 1;
		}
		protected bool IsFocusedCellSelectionCorner(Rectangle selection) {
			return FocusedCell.Equals(selection.Location)
				|| FocusedCell.Equals(new Point(selection.Right - 1, selection.Y))
				|| FocusedCell.Equals(new Point(selection.X, selection.Bottom - 1))
				|| FocusedCell.Equals(new Point(selection.Right - 1, selection.Bottom - 1));
		}
		protected void OnFocusedCellChanged(Point oldValue, Point newValue) {
			Data.FocusedCellChanged(oldValue, newValue);
			RaiseFocusedCellChanged(oldValue, newValue);
		}
		protected void OnSelectionChanged() {
			if(!InnerSelection.IsChanged) return;
			InnerSelection.IsChanged = false;
			if(wholeColumnSelection != null)
				wholeColumnSelection.Reset();
			Data.CellSelectionChanged();
			RaiseSelectionChanged();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseFocusedCellChanged(Point oldValue, Point newValue) {
			if(focusedCellChanged != null)
				focusedCellChanged.Raise(this, new FocusedCellChangedEventArgs(oldValue, newValue));
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseSelectionChanged() {
			if(selectionChanged != null)
				selectionChanged.Raise(this, EventArgs.Empty);
		}
		#region Clipboard
		public void CopySelectionToClipboard() {
			pivotGridClipboard.SetDataObject(GetSelectionString());
		}
		public string GetSelectionString() {
			if(SelectedCells.IsEmpty)
				return GetCellString(FocusedCell) + Environment.NewLine;
			bool removeEmptyCellLines = Data.OptionsBehavior.CopyToClipboardRemoveEmptyLines;
			bool copySelectedRowAndColumnsOnly = Data.OptionsBehavior.ClipboardCopyMultiSelectionMode == CopyMultiSelectionMode.DiscardIntermediateColumnsAndRows;
			bool removeEmptyHeaderLines = Data.OptionsBehavior.CopyToClipboardWithFieldValues
				&& Data.OptionsBehavior.ClipboardCopyCollapsedValuesMode == CopyCollapsedValuesMode.RemoveCollapsedLevels;
			bool allowDuplicateHeaders = !removeEmptyHeaderLines && Data.OptionsBehavior.ClipboardCopyCollapsedValuesMode == CopyCollapsedValuesMode.DuplicateCollapsedValues;
			int rowHeadersCount = Data.OptionsBehavior.CopyToClipboardWithFieldValues ? GetLevelCount(false) : 0;
			int columnHeadersCount = Data.OptionsBehavior.CopyToClipboardWithFieldValues ? GetLevelCount(true) : 0;
			Rectangle bounds = FullSelection;
			List<List<Point>> sortedSelection = GetSortedSelection(bounds);
			List<List<Point>> rotatedSelection = GetRotatedSelection(bounds);
			StringBuilder result = new StringBuilder();
			if(Data.OptionsBehavior.CopyToClipboardWithFieldValues) {
				for(int i = 0; i < columnHeadersCount; i++) {
					for(int j = 0; j < rowHeadersCount; j++) result.Append('\t');
					for(int colIndex = bounds.X; colIndex < bounds.Right; colIndex++) {
						List<Point> column = rotatedSelection[colIndex - bounds.X];
						if(column.Count == 0 && copySelectedRowAndColumnsOnly) continue;
						PivotFieldValueItem valueItem = GetItem(true, colIndex, i);
						if((valueItem != null) && (allowDuplicateHeaders || (valueItem.StartLevel == i)))
							result.Append(valueItem.Text);
						if(colIndex != bounds.Right - 1)
							result.Append('\t');
					}
					result.Append(Environment.NewLine);
				}
			}
			for(int rowIndex = 0; rowIndex < bounds.Height; rowIndex++) {
				List<Point> row = sortedSelection[rowIndex];
				if(row.Count == 0 && copySelectedRowAndColumnsOnly) continue;
				if(Data.OptionsBehavior.CopyToClipboardWithFieldValues) {
					for(int i = 0; i < rowHeadersCount; i++) {
						PivotFieldValueItem valueItem = GetItem(false, rowIndex + bounds.Top, i);
						if((valueItem != null) && (allowDuplicateHeaders || (valueItem.StartLevel == i)))
							result.Append(allowDuplicateHeaders ? GetRowFieldValue(valueItem) : valueItem.Text);
						result.Append('\t');
					}
				}
				if(row.Count > 0) {
					for(int colIndex = bounds.X, i = 0; colIndex < bounds.Right; colIndex++) {
						List<Point> column = rotatedSelection[colIndex - bounds.X];
						if(column.Count == 0 && copySelectedRowAndColumnsOnly) continue;						
						if(i == row.Count || row[i].X != colIndex) result.Append('\t');
						else {
							result.Append(GetCellString(row[i]).Replace("\t", ""));
							if(colIndex != bounds.Right - 1) result.Append('\t');
							i++;
						}
					}
				}
				result.Append(Environment.NewLine);
			}
			if(!removeEmptyHeaderLines && !removeEmptyCellLines)
				return result.ToString();
			else {
				List<List<string>> stringArrayBase = GetCellsArray(result.ToString());
				if(removeEmptyCellLines)
					RemoveEmptyLines(stringArrayBase, ref rowHeadersCount, ref columnHeadersCount, false);
				if(removeEmptyHeaderLines)
					RemoveEmptyLines(stringArrayBase, ref rowHeadersCount, ref columnHeadersCount, true);
				return JoinArray(stringArrayBase);
			}
		}
		List<List<string>> GetCellsArray(string input) {
			List<List<string>> result = new List<List<string>>();
			List<string> lineArray = new List<string>();
			foreach(string line in input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)) {
				foreach(string cell in line.Split('\t')) {
					lineArray.Add(cell);
				}
				result.Add(lineArray);
				lineArray = new List<string>();
			}
			result.RemoveAt(result.Count - 1);
			return result;
		}
		void RemoveEmptyLines(List<List<string>> inputArray, ref int rowHeadersCount, ref int columnHeadersCount, bool removeHeaders) {
			RemoveEmptyRows(inputArray, ref rowHeadersCount, ref columnHeadersCount, removeHeaders);
			RemoveEmptyColumns(inputArray, ref rowHeadersCount, ref columnHeadersCount, removeHeaders);
		}
		void RemoveEmptyRows(List<List<string>> inputArray, ref int rowHeadersCount, ref int columnHeadersCount, bool removeHeaders) {
			bool lineEmpty = true;
			int startRowIndex, endRowIndex, startColumnIndex = rowHeadersCount, endColumnIndex;
			if(removeHeaders) {
				startRowIndex = 0;
				endRowIndex = columnHeadersCount - 1;
			}
			else {
				startRowIndex = columnHeadersCount;
				endRowIndex = inputArray.Count - 1;
			}
			for(int i = endRowIndex; i >= startRowIndex; i--) {
				endColumnIndex = inputArray[i].Count - 1;
				for(int j = endColumnIndex; j >= startColumnIndex; --j) {
					if(inputArray[i][j] != string.Empty) {
						lineEmpty = false;
						break;
					}
				}
				if(lineEmpty) {
					inputArray.RemoveAt(i);
					if(removeHeaders) columnHeadersCount--;
				}
				lineEmpty = true;
			}
		}
		void RemoveEmptyColumns(List<List<string>> inputArray, ref int rowHeadersCount, ref int columnHeadersCount, bool removeHeaders) {
			bool lineEmpty = true;
			int startRowIndex = columnHeadersCount, endRowIndex = inputArray.Count - 1, startColumnIndex, endColumnIndex;
			if(removeHeaders) {
				startColumnIndex = 0;
				endColumnIndex = rowHeadersCount - 1;
			}
			else {
				startColumnIndex = rowHeadersCount;
				endColumnIndex = inputArray[0].Count - 1;
			}
			for(int j = endColumnIndex; j >= startColumnIndex; --j) {
				for(int i = endRowIndex; i >= startRowIndex; --i) {
					if(inputArray[i].Count - 1 < j) continue;
					if(inputArray[i][j] != string.Empty) {
						lineEmpty = false;
						break;
					}
				}
				if(lineEmpty) {
					for(int i = 0; i <= endRowIndex; ++i) {
						if(inputArray[i].Count - 1 < j) continue;
						inputArray[i].RemoveAt(j);
					}
					if(removeHeaders) rowHeadersCount--;
				}
				lineEmpty = true;
			}
		}
		string JoinArray(List<List<string>> array) {
			string result = string.Empty;
			List<string> lines = new List<string>();
			foreach(List<string> list in array) {
				lines.Add(string.Join("\t", list.ToArray()));
			}
			return string.Join(Environment.NewLine, lines.ToArray());
		}
		string GetRowFieldValue(PivotFieldValueItem valueItem) {
			if((Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree) && (!valueItem.IsCollapsed) && (valueItem.ItemType == PivotFieldValueItemType.Cell))
				return valueItem.Value.ToString();
			else
				return valueItem.Text;
		}
		string GetCellString(Point point) {
			PivotGridCellItem item = CreateCellItem(point.X, point.Y);
			if(item == null)
				return string.Empty;
			return item.Text;
		}
		List<List<Point>> GetSortedSelection(Rectangle bounds) {
			List<List<Point>> result = new List<List<Point>>(bounds.Height);
			for(int i = 0; i < bounds.Height; i++)
				result.Add(new List<Point>());
			foreach(Point point in SelectedCells)
				result[point.Y - bounds.Y].Add(point);
			PointsComparer comparer = new PointsComparer();
			foreach(List<Point> row in result)
				row.Sort(comparer);
			return result;
		}
		List<List<Point>> GetRotatedSelection(Rectangle bounds) {
			List<List<Point>> result = new List<List<Point>>(bounds.Width);
			for(int i = 0; i < bounds.Width; i++)
				result.Add(new List<Point>());
			foreach(Point point in SelectedCells)
				result[point.X - bounds.X].Add(point);
			PointsComparer comparer = new PointsComparer();
			foreach(List<Point> column in result)
				column.Sort(comparer);
			return result;
		}
		#endregion
	}
	public class PointsComparer : IComparer<Point> {
		#region IComparer<Point> Members
		public int Compare(Point a, Point b) {
			if(a.Y < b.Y) return -1;
			if(a.Y > b.Y) return 1;
			if(a.X == b.X) return 0;
			if(a.X < b.X) return -1;
			return 1;
		}
		#endregion
	}
	public class FocusedCellChangedEventArgs : EventArgs {
		Point oldCell, newCell;
		public FocusedCellChangedEventArgs(Point oldCell, Point newCell) {
			this.oldCell = oldCell;
			this.newCell = newCell;
		}
		public Point OldCell { get { return oldCell; } }
		public Point NewCell { get { return newCell; } }
	}
}

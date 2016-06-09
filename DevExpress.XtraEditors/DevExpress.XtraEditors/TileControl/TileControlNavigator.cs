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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraEditors {
	public class TileControlNavigator {
		public TileControlNavigator(ITileControl control) {
			Control = control;
		}
		public ITileControl Control { get; private set; }
		public void SetSelectedItemCore(TileNavigationItem item) {
			SelectedItem = item;
		}
		public TileNavigationItem SelectedItem {
			get;
			protected internal set;
		}
		public TileNavigationItem LastSelectedItem { 
			get; 
			set; 
		}
		protected internal virtual TileNavigationItem GetNavigationItem(TileItemViewInfo itemInfo) {
			foreach(List<TileNavigationItem> line in NavigationGrid) {
				foreach(TileNavigationItem navItem in line) {
					if(navItem.Item == itemInfo)
						return navItem;
				}
			}
			return null;
		}
		protected internal virtual void SetSelectedItem(TileNavigationItem item) {
			SetSelectedItem(item, false);
		}
		protected internal virtual void SetSelectedItem(TileNavigationItem item, bool alignRightEdge) {
			if(item == null && SelectedItem != null)
				LastSelectedItem = SelectedItem;
			SelectedItem = item;
			Control.Invalidate(Control.ViewInfo.Bounds);
			if(item == null)
				return;
			if(ShouldUseAutoSelectFocusedItem)
				Control.SelectedItem = item.Item.Item;
			else Control.ViewInfo.MakeVisible(SelectedItem.Item, alignRightEdge);
		}
		protected virtual bool ShouldUseAutoSelectFocusedItem {
			get { return Control.AutoSelectFocusedItem && Control.AllowSelectedItem; }
		}
		List<List<TileNavigationItem>> navigationGrid;
		protected internal List<List<TileNavigationItem>> GetNavigationGridCore() { return navigationGrid; }
		protected List<List<TileNavigationItem>> NavigationGrid {
			get {
				if(navigationGrid == null)
					navigationGrid = GetNavigationGrid();
				return navigationGrid;
			}
		}
		public virtual void MoveUp() {
				MoveVertical(-1, false);
		}
		public virtual void MoveDown() {
				MoveVertical(+1, true);
		}
		public virtual void MoveLeft() { MoveHorizontal(-1, false); }
		public virtual void MoveRight() { MoveHorizontal(+1, true); }
		public virtual void MovePageLeft() { MovePage(true); }
		public virtual void MovePageRight() { MovePage(false); }
		public virtual void MoveStart() { SetSelectedItem(GetFirstItem(), false); }
		public virtual void MoveEnd() { SetSelectedItem(GetLastItem(), true); }
		protected virtual void MovePage(bool left) {
			if(SelectedItem == null) {
				MoveStart();
				return;
			}
			if(Control != null && Control.Properties.Orientation == Orientation.Horizontal)
				MovePageHorizontal(left);
			else
				MovePageVertical(left);
		}
		protected void MovePageHorizontal(bool left) {
			TileNavigationItem invisible = GetFirstInvisibleItemHorizontal(left);
			if(invisible == null) {
				if(left)
					MoveStart();
				else
					MoveEnd();
				return;
			}
			if(invisible.Column < SelectedItem.Column && !left) {
				MoveEnd();
				return;
			}
			int delta = invisible.Column - SelectedItem.Column;
			if(delta == 0)
				delta = left ? -1 : 1;
			MoveHorizontal(delta, left);
		}
		protected void MovePageVertical(bool up) {
			TileNavigationItem invisible = GetFirstInvisibleItemVertical(up);
			if(invisible == null) {
				if(up)
					MoveStart();
				else
					MoveEnd();
				return;
			}
			if(invisible.Row < SelectedItem.Row && !up) {
				MoveEnd();
				return;
			}
			int delta = invisible.Row - SelectedItem.Row;
			if(delta == 0)
				delta = up ? -1 : 1;
			MoveVertical(delta, up);
		}
		bool? ItemIsFullyVisible(int x, int y) {
			return NavigationGrid[x][y].Item == null ? null : (bool?)NavigationGrid[x][y].Item.IsFullyVisible;
		}
		protected virtual TileNavigationItem GetFirstInvisibleItemHorizontal(bool fromLeft) {
			bool foundVisibleItem = false;
			if(fromLeft) {
				for(int i = NavigationGrid[0].Count - 1; i >= 0; i--) {
					bool? result = ItemIsFullyVisible(0, i);
					if(result == null) continue;
					if(result.Value) 
						foundVisibleItem = true;
					else if(foundVisibleItem) {
						return NavigationGrid[0][i];
					}
				}
			}
			else {
				for(int i = 0; i < NavigationGrid[0].Count; i++) {
					bool? result = ItemIsFullyVisible(0, i);
					if(result == null) continue;
					if(result.Value)
						foundVisibleItem = true;
					else if(foundVisibleItem) {
						return NavigationGrid[0][i];
					}
				}
			}
			return null;
		}
		TileNavigationItem GetFirstInvisibleItemVertical(bool fromUp) {
			bool foundVisibleItem = false;
			if(fromUp) {
				for(int i = NavigationGrid.Count - 1; i >= 0; i--) {
					bool? result = ItemIsFullyVisible(i, 0);
					if(result == null) continue;
					if(result.Value)
						foundVisibleItem = true;
					else if(foundVisibleItem) {
						return NavigationGrid[i][0];
					}
				}
			}
			else {
				for(int i = 0; i < NavigationGrid.Count; i++) {
					bool? result = ItemIsFullyVisible(i, 0);
					if(result == null) continue;
					if(result.Value)
						foundVisibleItem = true;
					else if(foundVisibleItem) {
						return NavigationGrid[i][0];
					}
				}
			}
			return null;
		} 
		protected TileNavigationItem GetFirstItem() {
			return GetFirstItem(0);
		}
		public virtual void OnKeyClick() {
			if(SelectedItem == null) {
				var item = TrySyncWithSelectedItem();
				if(item != null) {
					var needClick = CheckIfClickNeeded(item);
					SetSelectedItem(item);
					if(needClick)
						SelectedItem.Item.Item.OnItemClick();
				}
				else 
					SetSelectedItem(GetFirstItem());
				return;
			}
			SelectedItem.Item.Item.OnItemClick();
		}
		bool CheckIfClickNeeded(TileNavigationItem item) {
			return item != null &&
				Control.AllowSelectedItem &&
				Control.SelectedItem == item.Item.Item;
		}
		protected virtual TileNavigationItem GetFirstItem(int row) {
			if(row >= NavigationGrid.Count)
				return null;
			if(NavigationGrid[row].Count < 1)
				return null;
			return NavigationGrid[row][0];
		}
		protected virtual TileNavigationItem GetLastItem() {
			if(Control.Properties.Orientation == System.Windows.Forms.Orientation.Horizontal)
				return GetLastItemHorizontal();
			return GetLastItemVertical();
		}
		protected TileNavigationItem GetLastItemHorizontal() {
			int row = NavigationGrid.Count - 1;
			int column = NavigationGrid[row].Count - 1;
			while(column >= 0) {
				row = NavigationGrid.Count - 1;
				while(row >= 0) {
					if(NavigationGrid[row][column].Item != null)
						return NavigationGrid[row][column];
					row--;
				}
				column--;
			}
			return null;
		}
		protected TileNavigationItem GetLastItemVertical() {
			int row = NavigationGrid.Count - 1;
			int column;
			while(row >= 0) {
				column = NavigationGrid[row].Count - 1;
				while(column >= 0) {
					if(NavigationGrid[row][column].Item != null)
						return NavigationGrid[row][column];
					column--;
				}
				row--;
			}
			return null;
		}
		protected virtual void MoveVertical(int delta, bool alignDown) {
			if(SelectedItem == null)
				SelectedItem = TrySyncWithSelectedItem();
			if(SelectedItem == null) {
				SetSelectedItem(GetFirstItem(0), false);
				return;
			}
			TileNavigationItem item;
			int row = SelectedItem.Row + delta;
			row = Math.Min(NavigationGrid.Count - 1, Math.Max(0, row));
			int column = GetFirstItemColumn(SelectedItem, SelectedItem.Column);
		cycleBegin:
			while(true) {
				item = GetItemVertical(row, column, delta);
				if(item == null) {
					if(column <= 0) return;
					column--;
					goto cycleBegin;
				}
				if((item.Item != SelectedItem.Item) &&
					(GetFirstItemRow(item, item.Row) != GetFirstItemRow(SelectedItem, SelectedItem.Row)))
					break;
				row += delta;
				if(row >= NavigationGrid.Count || row < 0) {
					SetSelectedItem(SelectedItem);
					return;
				}
			}
			if(item == null)
				return;
			SetSelectedItem(item, alignDown);
		}
		protected virtual void MoveHorizontal(int delta, bool alignRight) {
			if(SelectedItem == null)
				SelectedItem = TrySyncWithSelectedItem();
			if(SelectedItem == null) {
				SetSelectedItem(GetFirstItem(0), false);
				return;
			}
			int row = GetFirstItemRow(SelectedItem, SelectedItem.Row);
			int column = SelectedItem.Column + delta;
			column = Math.Min(NavigationGrid[row].Count - 1, Math.Max(0, column));
			while(true) {
				TileNavigationItem item = GetItem(row, column);
				if(item == null)
					return;
				if(item.Item != SelectedItem.Item)
					break;
				column += delta;
				if(column >= NavigationGrid[row].Count || column < 0) {
					SetSelectedItem(SelectedItem);
					return;
				}
			}
			SetSelectedItem(GetItem(row, column, delta), alignRight);
		}
		protected TileNavigationItem TrySyncWithSelectedItem() {
			if(Control.AllowSelectedItem && Control.SelectedItem != null) 
				return GetNavigationItem(Control.SelectedItem);
			return LastSelectedItem;
		}
		protected internal int GetFirstItemColumn(TileNavigationItem item, int currentColumn) {
			if(currentColumn <= 0) return 0;
			if(item.Item != NavigationGrid[item.Row][currentColumn - 1].Item)
				return currentColumn;
			return GetFirstItemColumn(item, currentColumn - 1);
		}
		protected internal int GetFirstItemRow(TileNavigationItem item, int currentRow) {
			if(currentRow <= 0) return 0;
			if(item.Item != NavigationGrid[currentRow - 1][item.Column].Item)
				return currentRow;
			return GetFirstItemRow(item, currentRow - 1);
		}
		protected internal TileNavigationItem GetItemVertical(int currentRow, int currentCol, int delta) {
			for(; currentRow > -1 && currentRow < NavigationGrid.Count; currentRow += delta) {
				if(NavigationGrid[currentRow][currentCol].Item != null)
					return NavigationGrid[currentRow][currentCol];
			}
			return null;
		}
		protected virtual TileNavigationItem GetItem(int row, int column) {
			return GetItem(row, column, -1);
		}
		protected virtual TileNavigationItem GetItem(int row, int column, int direction) {
			column = Math.Min(column, NavigationGrid[row].Count - 1);
			while(column >= 0 && column < NavigationGrid[row].Count) {
				if(NavigationGrid[row][column].Item != null)
					return NavigationGrid[row][column];
				column += direction;
			}
			return null;
		}
		protected virtual TileNavigationItem GetNavigationItem(TileItem item) {
			foreach(List<TileNavigationItem> row in NavigationGrid) {
				foreach(TileNavigationItem navitem in row) {
					if(navitem.Item == null)
						continue;
					if(navitem.Item.Item == item) {
						return navitem;
					}
				}
			}
			return null;
		}
		protected virtual List<List<TileNavigationItem>> GetNavigationGrid() {
			return Control.Properties.Orientation == System.Windows.Forms.Orientation.Horizontal ? GetNavigationGridHorizontal() : GetNavigationGridVertical();
		}
		protected virtual List<List<TileNavigationItem>> GetNavigationGridHorizontal() {
			List<List<TileNavigationItem>> res = new List<List<TileNavigationItem>>();
			for(int rowIndex = 0; rowIndex < Control.ViewInfo.RealRowCount * 2; rowIndex++) {
				List<TileNavigationItem> row = new List<TileNavigationItem>();
				int column = 0;
				int itemSize = Control.Properties.ItemSize;
				int indent = Control.Properties.IndentBetweenItems;
				foreach(TileGroupViewInfo groupInfo in Control.ViewInfo.Groups) {
					Point pt = new Point(groupInfo.Bounds.X + (itemSize - indent) / 4, groupInfo.Bounds.Y + ((itemSize + indent) / 2) * rowIndex + (itemSize - indent) / 4);
					for(; pt.X < groupInfo.Bounds.Right; pt.X += (itemSize + indent) / 2) {
						row.Add(new TileNavigationItem() { Item = groupInfo.Items.GetItemByPoint(pt), Column = column, Row = rowIndex });
						column++;
					}
				}
				if(row.Count > 0)
					res.Add(row);
			}
			return res;
		}
		protected virtual List<List<TileNavigationItem>> GetNavigationGridVertical() {
			List<List<TileNavigationItem>> res = new List<List<TileNavigationItem>>();
			int maxColCount = 0;
			int rowIndex = 0;
			int column = 0;
			int itemSize = Control.Properties.ItemSize;
			int indent = Control.Properties.IndentBetweenItems;
			foreach(TileGroupViewInfo groupInfo in Control.ViewInfo.Groups) {
				Point pt = new Point(groupInfo.Bounds.X + (itemSize - indent) / 4, groupInfo.Bounds.Y + (itemSize - indent) / 4);
				for(; pt.Y <= groupInfo.Bounds.Bottom; pt.Y += (itemSize + indent) / 2) {
					List<TileNavigationItem> row = new List<TileNavigationItem>();
					for(; pt.X < groupInfo.Bounds.Right; pt.X += (itemSize + indent) / 2) {
						row.Add(new TileNavigationItem() { Item = groupInfo.Items.GetItemByPoint(pt), Column = column, Row = rowIndex });
						maxColCount = Math.Max(maxColCount, column + 1);
						column++;
					}
					rowIndex++;
					column = 0;
					pt = new Point(groupInfo.Bounds.X + (itemSize - indent) / 4, pt.Y);
					if(row.Count > 0)
						res.Add(row);
				}
			}
			AlignNavigationGrid(res, maxColCount);
			return res;
		}
		protected internal void AlignNavigationGrid(List<List<TileNavigationItem>> grid, int maxColCount) {
			int rowIndex = 0;
			foreach(List<TileNavigationItem> row in grid) {
				if(row.Count < maxColCount) {
					for(int i = row.Count; i < maxColCount; i++) {
						row.Add(new TileNavigationItem() { Item = null, Column = i, Row = rowIndex });
					}
				}
				rowIndex++;
			}
		}
		protected internal virtual void UpdateNavigationGrid() {
			TileItem item = SelectedItem != null ? SelectedItem.Item.Item : null;
			TileItem lastSelectedItem = LastSelectedItem != null ? LastSelectedItem.Item.Item : null;
			this.navigationGrid = GetNavigationGrid();
			TileNavigationItem navitem = GetNavigationItem(item);
			if(navitem != null)
				SetSelectedItem(navitem, false);
			if(LastSelectedItem != null)
				UpdateLastSelectedItem(lastSelectedItem);
		}
		void UpdateLastSelectedItem(TileItem item) {
			if(item == null)
				LastSelectedItem = null;
			else
				LastSelectedItem = GetNavigationItem(item);
		}
	}
	public class TileNavigationItem {
		public TileItemViewInfo Item { get; set; }
		public int Column { get; set; }
		public int Row { get; set; }
	}
}

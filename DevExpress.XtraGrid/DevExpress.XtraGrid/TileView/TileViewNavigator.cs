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

using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraGrid.Views.Tile.Handler {
	public class TileViewNavigator : TileControlNavigator {
		public TileViewNavigator(ITileControl control) : base(control) {
			ItemsNavigationGrid = new List<List<int>>();
		}
		bool ShouldFocusLastItemAfterScroll { get; set; }
		bool ShouldFocusFirstItemAfterScroll { get; set; }
		TileView View {
			[System.Diagnostics.DebuggerStepThrough]
			get { return (Control as TileViewInfo).View as TileView; }
		}
		TileViewInfoCore ViewInfoCore {
			[System.Diagnostics.DebuggerStepThrough]
			get { return Control.ViewInfo as TileViewInfoCore; } 
		}
		protected override void MoveVertical(int delta, bool alignDown) {
			if(FocusFirstVisible())
				return;
			FocusedRowHandle = GetNewFocusedRowHandle(0, delta);
		}
		protected override void MoveHorizontal(int delta, bool alignRight) {
			if(FocusFirstVisible())
				return;
			if(Control.ViewInfo.IsRightToLeft) delta = -delta;
			FocusedRowHandle = GetNewFocusedRowHandle(delta, 0);
		}
		public virtual void MoveFocusedRow(int delta) {
			if(FocusFirstVisible())
				return;
			FocusedRowHandle = GetHandleAfterFocused(delta);
		}
		public virtual void MoveLastVisible() {
			FocusedRowHandle = GetLastVisibleHandle();
		}
		public override void OnKeyClick() {
			if(EnableFocusHighlight()) {
				ViewInfoCore.ScrollToItem(FocusedRowHandle);
				return;
			}
			var focusedItem = ViewInfoCore.GetFocusedItem();
			if(focusedItem != null) {
				focusedItem.OnItemClick();
			}
			else {
				ViewInfoCore.ScrollToItem(FocusedRowHandle);
			}
		}
		bool EnableFocusHighlight() {
			bool done = !View.CanHighlightFocusedItem;
			if(done) ViewInfoCore.ResetFocusedHighlightState(true);
			return done;
		}
		bool FocusFirstVisible() {
			if(FocusedRowHandle != GridControl.InvalidRowHandle)
				return false;
			if(ViewInfoCore.VisibleItems.Count == 0)
				FocusedRowHandle = 0;
			else
				FocusedRowHandle = GetFirstVisibleHandle();
			return true;
		}
		int GetFirstVisibleHandle() {
			int? min = null;
			foreach(var pair in ViewInfoCore.VisibleItems) {
				if(pair.Key < min || min == null)
					min = pair.Key;
			}
			return min == null ? 0 : (int)min;
		}
		int GetLastVisibleHandle() {
			var pairs = ViewInfoCore.VisibleItems.OrderByDescending(x => x.Key).ToArray();
			foreach(var pair in pairs) {
				var itemInfo = pair.Value.ItemInfo as TileViewItemInfo;
				if(itemInfo != null && itemInfo.IsFullyVisibleOnScreen)
					return pair.Key;
			}
			return 0;
		}
		bool AnimatingScroll {
			get {
				var aInfos = XtraAnimator.Current.Animations.OfType<TileViewOffsetAnimationInfo>().ToList();
				foreach(var aInfo in aInfos)
					if(aInfo.AnimationId == ViewInfoCore)
						return true;
				return false;
			}
		}
		protected override void MovePage(bool left) {
			if(AnimatingScroll)
				return;
			if(FocusFirstVisible())
				return;
			MovePageCore(left);
		}
		protected virtual void MovePageCore(bool left) {
			int boundaryItemHandle = GetBoundaryItemRowHandle(left);
			if(boundaryItemHandle != GridControl.InvalidRowHandle && boundaryItemHandle != FocusedRowHandle) {
				FocusedRowHandle = boundaryItemHandle;
				return;
			}
			else {
				ScrollPage(boundaryItemHandle, left);
			}
		}
		void ScrollPage(int rowHandle, bool left) {
			if(ViewInfoCore != null) {
				if(left)
					ShouldFocusFirstItemAfterScroll = true;
				else
					ShouldFocusLastItemAfterScroll = true;
				ViewInfoCore.ScrollPage(rowHandle, left);
			}
		}
		public void OnScrollFinish() {
			if(ShouldFocusFirstItemAfterScroll) MovePageCore(true);
			if(ShouldFocusLastItemAfterScroll) MovePageCore(false);
			ShouldFocusFirstItemAfterScroll = false;
			ShouldFocusLastItemAfterScroll = false;
		}
		int GetBoundaryItemRowHandle(bool left) {
			int resultHandle = GridControl.InvalidRowHandle;
			if(ItemIsPartiallyVisible(FocusedRowHandle)) {
				int deltaColumn = 0;
				resultHandle = FocusedRowHandle;
				bool found = false;
				while(!found) {
					deltaColumn += left ? -1 : +1;
					int handle = ViewInfoCore.IsHorizontal ? GetNewFocusedRowHandle(deltaColumn, 0) : GetNewFocusedRowHandle(0, deltaColumn);
					if(!ItemIsFullyVisible(handle) || resultHandle == handle)
						found = true;
					else
						resultHandle = handle;
				}
			}
			else {
				resultHandle = left ? GetFirstVisibleHandle() : GetLastVisibleHandle();
			}
			return resultHandle;
		}
		bool ItemIsPartiallyVisible(int rowHandle) { return ItemIsVisible(rowHandle, true); }
		bool ItemIsFullyVisible(int rowHandle) { return ItemIsVisible(rowHandle, false); }
		bool ItemIsVisible(int rowHandle, bool partially) {
			if(rowHandle != GridControl.InvalidRowHandle) {
				if(ViewInfoCore.VisibleItems.ContainsKey(rowHandle)) {
					if(partially) 
						return true;
					var itemInfo = ViewInfoCore.VisibleItems[rowHandle].ItemInfo as TileViewItemInfo;
					if(itemInfo != null) {
						return itemInfo.IsFullyVisibleOnScreen;
					}
				}
			}
			return false;
		}
		public override void MoveEnd() {
			if(FocusFirstVisible())
				return;
			View.FocusedRowHandle = Math.Max(0, View.RowCount - View.DataController.GroupRowCount - 1);
		}
		public override void MoveStart() {
			if(FocusFirstVisible())
				return;
			View.FocusedRowHandle = 0;
		}
		protected internal List<List<int>> ItemsNavigationGrid { get; set; }
		protected internal int CurrentFocusedColumn = 0;
		int FocusedRowHandle {
			get { return ViewInfoCore.FocusedRowHandle; }
			set {
				ViewInfoCore.UseOptimizedCurrentFocusColumnUpdate = true;
				ViewInfoCore.FocusedRowHandle = value;
			}
		}
		public void UpdateNavigationGridCore() {
			if(View.IsValidRowHandle(-1)) {
				int rowCount = Control.Properties.Orientation == Orientation.Horizontal ? ViewInfoCore.MaxRowCount : ViewInfoCore.GetColumnCount();
				ItemsNavigationGrid = GetNavigationGrid(rowCount, out CurrentFocusedColumn);
			}
		}
		List<List<int>> GetNavigationGrid(int rowCount, out int currentFocusColumn) {
			currentFocusColumn = -1;
			List<List<int>> grid = new List<List<int>>();
			if(View.DataController.GroupInfo.Count > 0) {
				foreach(var groupInfo in View.DataController.GroupInfo) {
					int first = groupInfo.ChildControllerRow;
					int last = groupInfo.ChildControllerRow + groupInfo.ChildControllerRowCount - 1;
					int n = 1;
					List<int> column = new List<int>();
					for(int i = first; i <= last; i++) {
						column.Add(i);
						if(i == FocusedRowHandle)
							currentFocusColumn = grid.Count;
						if(n % rowCount == 0) {
							grid.Add(column);
							column = new List<int>();
						}
						n++;
					}
					if(column.Count > 0) {
						while(column.Count < rowCount)
							column.Add(-1);
						grid.Add(column);
					}
				}
			}
			return grid;
		}
		public void UpdateCurrentFocusedColumn(bool optimized) {
			int count = ItemsNavigationGrid.Count;
			if(count == 0) return;
			int threshold = 100;
			int start = 0;
			int end = count-1;
			if(optimized) {
				start = Math.Max(0, CurrentFocusedColumn - threshold);
				end = Math.Min(count - 1, CurrentFocusedColumn + threshold);
			}
			for(int n = start; n <= end; n++) {
				if(ItemsNavigationGrid[n].Contains(FocusedRowHandle)) {
					this.CurrentFocusedColumn = n;
					return;
				}
			}
		}
		protected internal int GetNewFocusedRowHandle(int deltaX, int deltaY) {
			if(FocusedRowHandle == GridControl.InvalidRowHandle)
				return FocusedRowHandle;
			if(Control.Properties.Orientation == Orientation.Horizontal)
				return GetNewFocusedRowHandleHorz(deltaX, deltaY);
			return GetNewFocusedRowHandleVert(deltaX, deltaY);
		}
		protected virtual int GetNewFocusedRowHandleHorz(int deltaX, int deltaY) {
			if(deltaX != 0) {
				if(ViewInfoCore.AllowGroups)
					return GetHandleByMovingColumnGrouped(deltaX, ViewInfoCore.MaxRowCount);
				else
					return GetHandleByMovingColumn(deltaX, ViewInfoCore.MaxRowCount);
			}
			if(deltaY != 0)
				return GetHandleAfterFocused(deltaY);
			return FocusedRowHandle;
		}
		protected virtual int GetNewFocusedRowHandleVert(int deltaX, int deltaY) {
			if(deltaX != 0)
				return GetHandleAfterFocused(deltaX);
			if(deltaY != 0) {
				if(ViewInfoCore.AllowGroups)
					return GetHandleByMovingColumnGrouped(deltaY, ViewInfoCore.GetColumnCount());
				else
					return GetHandleByMovingColumn(deltaY, ViewInfoCore.GetColumnCount());
			}
			return FocusedRowHandle;
		}
		int GetHandleByMovingColumn(int columnDelta, int rowCount) {
			int result = FocusedRowHandle;
			result += columnDelta * rowCount;
			return Math.Min(View.RowCount - 1, Math.Max(0, result));
		}
		int GetHandleByMovingColumnGrouped(int columnDelta, int rowCount) {
			int handle = FocusedRowHandle;
			var currentGroupRow = GetGroupInfoByHandle(FocusedRowHandle);
			int currentFocusRow = (FocusedRowHandle - currentGroupRow.ChildControllerRow) % rowCount;
			int currentFocusColumn = -1;
			int prevFocusColumn = currentFocusColumn;
			currentFocusColumn = this.CurrentFocusedColumn;
			bool found = false;
			while(!found && prevFocusColumn != currentFocusColumn) {
				prevFocusColumn = currentFocusColumn;
				currentFocusColumn += columnDelta;
				currentFocusColumn = Math.Min(ItemsNavigationGrid.Count - 1, Math.Max(currentFocusColumn, 0));
				int newHandle = ItemsNavigationGrid[currentFocusColumn][currentFocusRow];
				if(newHandle >= 0) {
					handle = newHandle;
					found = true;
				}
			}
			return handle;
		}
		protected internal DevExpress.Data.GroupRowInfo GetGroupInfoByHandle(int rowHandle) {
			if(View.DataController.GroupInfo.Count == 0) return null;
			foreach(var groupInfo in View.DataController.GroupInfo) {
				int first = groupInfo.ChildControllerRow;
				int last = groupInfo.ChildControllerRow + groupInfo.ChildControllerRowCount - 1;
				if(rowHandle >= first && rowHandle <= last)
					return groupInfo;
			}
			return null;
		}
		int GetHandleAfterFocused(int delta) {
			int result = FocusedRowHandle;
			int n = Math.Abs(delta);
			while(n > 0) {
				result = delta < 0 ? GetPrevVisibleRow(result) : GetNextVisibleRow(result);
				n--;
			}
			if(result == GridControl.InvalidRowHandle)
				return FocusedRowHandle;
			return result;
		}
		public int GetNextVisibleRow(int rowVisibleIndex) {
			int rowCount = View.RowCount;
			rowCount -= View.DataController.GroupInfo.Count;
			return rowCount > rowVisibleIndex + 1 ? rowVisibleIndex + 1 : GridControl.InvalidRowHandle;
		}
		public int GetPrevVisibleRow(int rowVisibleIndex) {
			return View.GetPrevVisibleRow(rowVisibleIndex);
		}
	}
}

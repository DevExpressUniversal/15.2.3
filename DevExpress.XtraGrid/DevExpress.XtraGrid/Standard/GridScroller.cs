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
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Helpers;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Views.Grid.Customization;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Text;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraGrid.GroupSummaryEditor;
using System.Collections.Generic;
using DevExpress.Data.Summary;
using DevExpress.Utils.Serializing.Helpers;
using System.Drawing.Design;
using DevExpress.Utils.Editors;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraGrid.Frames;
using DevExpress.XtraGrid.Dragging;
using DevExpress.Utils.Gesture;
using DevExpress.XtraGrid.Internal;
namespace DevExpress.XtraGrid.Scrolling {
	public abstract class GridViewScroller {
		GridView view;
		ScrollPositionInfo scrollInfo;
		public GridViewScroller(GridView view) {
			this.view = view;
			AllowMakeRowVisible = true;
		}
		public ScrollPositionInfo ScrollInfo {
			get {
				if(scrollInfo == null) scrollInfo = CreateScrollInfo();
				return scrollInfo;
			}
		}
		public virtual void CancelSmoothScroll() {
		}
		protected internal virtual bool AllowDrawFocusRectangle { get { return true; } }
		protected virtual ScrollPositionInfo CreateScrollInfo() {
			return new ScrollPositionInfo();
		}
		public GridView View { get { return view; } }
		public GridViewInfo ViewInfo { get { return view.ViewInfo; } }
		public abstract void OnVScroll(int newPosition);
		protected internal abstract void OnMouseWheelScroll(MouseWheelScrollClientArgs e);
		protected Point touchStart = Point.Empty;
		protected Point touchOverpan = Point.Empty;
		protected virtual int GetScrollLinesCount() {
			int res = SystemInformation.MouseWheelScrollLines == -1 ? View.ScrollPageSize : SystemInformation.MouseWheelScrollLines;
			if (View.ViewInfo.ActualDataRowMinRowHeight > 40 || View.ViewInfo.RowsInfo.Count < 15) {
				if (res > 2) return res / 2;
				return 1;
			}
			return res;
		}
		protected internal virtual void OnTouchScroll(GestureArgs info, Point delta, ref Point overPan) {
			if(info.GF == GF.BEGIN) {
				touchStart.X = View.LeftCoord;
				touchStart.Y = View.TopRowIndex;
				touchOverpan = Point.Empty;
				return;
			}
			if(delta.Y != 0) {
				int totalDelta = (info.Current.Y - info.Start.Y) / (ViewInfo.ActualDataRowMinRowHeight == 0 ? 18 : ViewInfo.ActualDataRowMinRowHeight);
				int prevTopRowIndex = View.TopRowIndex;
				View.TopRowIndex = touchStart.Y - totalDelta;
				if(View.CheckTopRowIndex(View.TopRowIndex + (delta.Y > 0 ? -1 : 1)) == View.TopRowIndex) {
					if(prevTopRowIndex == View.TopRowIndex)
						touchOverpan.Y += delta.Y;
					else
						touchOverpan.Y = 0;
				}
			}
			if(delta.X != 0) {
				int prevLeftCoord = View.LeftCoord;
				View.LeftCoord -= delta.X;
				if(prevLeftCoord == View.LeftCoord)
					touchOverpan.X += delta.X;
				else
					touchOverpan.X = 0;
				if(!View.ScrollInfo.HScrollVisible) touchOverpan.X = 0;
			}
			overPan = touchOverpan;
		}
		protected internal virtual GridRowCollection LoadRowsCore(GridViewInfo viewInfo, GridRowsLoadInfo e) { return new GridRowCollection(); }
		public List<int> GetParentRowsTree(int rowHandle) {
			if(!View.AllowFixedGroups) return null;
			if(!View.IsValidRowHandle(rowHandle)) return null;
			int level = View.GetRowLevel(rowHandle);
			if(level == 0) return null;
			List<int> res = new List<int>();
			while(level >= 0) {
				int handle = View.GetParentRowHandle(rowHandle);
				if(!View.IsValidRowHandle(handle)) break;
				res.Insert(0, handle);
				rowHandle = handle;
				level--;
			}
			return res.Count > 0 ? res : null;
		}
		protected int RowCount { get { return View.RowCount; } }
		protected int ScrollPageSize { get { return View.ScrollPageSize; } }
		protected int GetVisibleRowHandle(int rowVisibleIndex) { return View.GetVisibleRowHandle(rowVisibleIndex); }
		protected int FocusedRowHandle { get { return View.FocusedRowHandle; } set { View.FocusedRowHandle = value; } }
		protected internal virtual void OnMoveNextPage() {}
		protected internal virtual void OnMovePrevPage() {}
		protected internal abstract void OnMakeRowVisibleCore(int rowHandle, bool invalidate);
		protected internal virtual void OnDoChangedFocusedRow(int newFocusedRowHandle, KeyEventArgs e) {
			View.FocusedRowHandle = newFocusedRowHandle;
		}
		protected internal virtual bool ShowEditor() { return true; }
		public bool AllowMakeRowVisible { get; set; }
		protected internal virtual void OnMoveFirst() {
			View.MoveTo(View.GetVisibleRowHandle(0), Keys.Home | Control.ModifierKeys);
		}
		protected internal virtual void OnMoveLastVisible() {
			View.MoveTo(GetVisibleRowHandle(RowCount - 1), Keys.End | Control.ModifierKeys);
		}
		protected internal virtual bool ShowEditForm(int requiredHeight) {
			if(!View.IsAllowEditForm(View.FocusedRowHandle)) return false;
			int startTopRowIndex = View.TopRowIndex;
			RowVisibleState rv =  View.IsRowVisible(View.FocusedRowHandle);
			if(rv == RowVisibleState.Hidden) {
				View.MakeRowVisible(View.FocusedRowHandle);
			}
			try {
				View.LockInvalidate();
				ViewInfo.allowEditFormHeight = false;
				int lastTopRowIndex = View.TopRowIndex;
				while(true) {
					GridDataRowInfo rowInfo = ViewInfo.GetGridRowInfo(View.FocusedRowHandle) as GridDataRowInfo;
					if(rowInfo == null) return false;
					if(rowInfo.TotalBounds.Bottom + requiredHeight < ViewInfo.ViewRects.Rows.Bottom) return true;
					int index = View.GetVisibleIndex(View.FocusedRowHandle);
					if(index > View.TopRowIndex) View.TopRowIndex++;
					if(View.TopRowIndex == lastTopRowIndex) {
						View.TopRowIndex = startTopRowIndex;
						return false;
					}
					lastTopRowIndex = View.TopRowIndex;
				}
			}
			finally {
				ViewInfo.allowEditFormHeight = true;
				View.UnlockInvalidate();
			}
		}
	}
	public class GridViewByRowScroller : GridViewScroller {
		public GridViewByRowScroller(GridView view) : base(view) {
		}
		public override void OnVScroll(int newPosition) {
			View.TopRowIndex = GetActualVScroll(newPosition);
		}
		protected internal override void OnMouseWheelScroll(MouseWheelScrollClientArgs e) {
			int delta = e.Distance;
			if(e.AllowSystemLinesCount) delta = delta < 0 ? -GetScrollLinesCount() : GetScrollLinesCount();
			if(View.IsInplaceEditFormVisible) return;
			if(!View.AllowFixedGroups || !View.ScrollInfo.VScrollVisible) {
				View.TopRowIndex += delta;
				return;
			}
			int value = View.ScrollInfo.VScroll.Value;
			if(delta > 0) {
				if(value + delta + View.ScrollInfo.VScroll.LargeChange >= View.ScrollInfo.VScroll.Maximum)
					delta = View.ScrollInfo.VScroll.Maximum - value - View.ScrollInfo.VScroll.LargeChange;
				if(delta < 0) delta = 0;
			}
			View.ScrollInfo.VScroll.Value += delta;
		}
		protected bool AllowFixedGroups { get { return View.AllowFixedGroups; } }
		int GetActualVScroll(int newTopRowIndex) {
			if(!AllowFixedGroups) return newTopRowIndex;
			if(newTopRowIndex < 0) return newTopRowIndex;
			return ConvertScrollIndexToIndex(newTopRowIndex);
		}
		int ConvertScrollIndexToIndex(int scrollIndex) {
			return View.DataController.GetVisibleIndexes().ConvertScrollIndexToIndex(scrollIndex, AllowFixedGroups);
		}
		protected internal override GridRowCollection LoadRowsCore(GridViewInfo viewInfo, GridRowsLoadInfo e) {
			int maxRowsHeight = e.MaxRowsHeight < 0 ? viewInfo.ViewRects.Rows.Height : e.MaxRowsHeight;
			int topRowIndex = e.TopVisibleRowIndex;
			if(topRowIndex < 0) topRowIndex = viewInfo.ViewTopRowIndex;
			int rowHeight;
			GridRowCollection rows = e.CalcOnly ? null : new GridRowCollection();
			GraphicsInfo ginfo = viewInfo.GInfo;
			e.VisibleRowCount = View.RowCount;
			bool showNewItemRowAtTop = viewInfo.NewItemRow == NewItemRowPosition.Top, showFilterRow = View.OptionsView.ShowAutoFilterRow;
			bool skipNextRow = false;
			if(!e.UseMinRowHeight) ginfo.AddGraphics(e.Graphics);
			try {
				int topRowHandle = GetVisibleRowHandle(topRowIndex);
				if(topRowHandle == GridControl.InvalidRowHandle)
					topRowIndex = GridControl.InvalidRowHandle;
				List<int> parentRows = GetParentRowsTree(topRowHandle);
				GridRow prevRow = null;
				int index = 0;
				while((showFilterRow || showNewItemRowAtTop || topRowIndex != GridControl.InvalidRowHandle) && e.ResultRowsHeight <= maxRowsHeight) {
					int rowHandle = 0;
					bool forcedRow = false;
					if(showFilterRow) rowHandle = GridControl.AutoFilterRowHandle;
					else {
						if(showNewItemRowAtTop) {
							rowHandle = CurrencyDataController.NewItemRow;
						}
						else {
							rowHandle = View.GetVisibleRowHandle(topRowIndex);
							if(index == 0) {
								if(parentRows != null) {
									forcedRow = true;
									rowHandle = parentRows[0];
									parentRows.RemoveAt(0);
									if(parentRows.Count == 0) {
										parentRows = null;
										index++;
									}
									skipNextRow = true;
								}
							}
						}
					}
					rowHeight = (e.UseMinRowHeight ? viewInfo.GetActualMinRowHeight(View.IsGroupRow(rowHandle)) : viewInfo.CalcTotalRowHeight(ginfo.Graphics, 0, rowHandle, topRowIndex, -1, View.IsGroupRow(rowHandle) ? (bool?)null : (bool?)true));
					if(!e.CalcOnly) {
						GridRow row = rows.Add(rowHandle, topRowIndex, forcedRow ? View.GetRowLevel(rowHandle) : View.GetVisibleRowLevel(topRowIndex), e.UseMinRowHeight ? 0 : rowHeight, View.GetRowKey(rowHandle), forcedRow);
						row.ForcedRowLight = forcedRow;
						if(prevRow != null && prevRow.RowHandle < 0) {
							prevRow.NextRowPrimaryChild = View.GetChildRowHandle(prevRow.RowHandle, 0) == rowHandle;
						}
						prevRow = row;
					}
					e.ResultRowCount++;
					e.ResultRowsHeight += rowHeight;
					if(!e.UseMinRowHeight && View.IsMasterRow(rowHandle) && View.GetMasterRowExpanded(rowHandle)) {
						e.HasDetails = true;
					}
					if(showFilterRow) {
						showFilterRow = false;
						continue;
					}
					if(showNewItemRowAtTop) {
						showNewItemRowAtTop = false;
						continue;
					}
					if(forcedRow && skipNextRow) continue;
					topRowIndex = View.GetNextVisibleRow(topRowIndex);
				}
			}
			finally {
				if(!e.UseMinRowHeight) ginfo.ReleaseGraphics();
			}
			if(topRowIndex == GridControl.InvalidRowHandle && e.ResultRowsHeight <= e.MaxRowsHeight) e.ResultAllRowsFit = true;
			e.ResultRows = rows;
			return rows;
		}
		protected int TopRowIndex { get { return View.TopRowIndex; } set { View.TopRowIndex = value; } }
		protected void MoveTopRowIndex(int delta) {
			int prevTopIndex = TopRowIndex, prevHandle = FocusedRowHandle;
			TopRowIndex = TopRowIndex + delta;
			if(prevTopIndex == TopRowIndex) {
				if(delta > 0)
					FocusedRowHandle = GetVisibleRowHandle(RowCount - 1);
				else {
					FocusedRowHandle = GetVisibleRowHandle(0);
				}
			}
			else
				FocusedRowHandle = GetVisibleRowHandle(TopRowIndex);
			View.DoAfterMoveFocusedRow(new KeyEventArgs(Keys.PageDown | Control.ModifierKeys), prevHandle, null, null);
		}
		int GetFocusedRowIndex() { return View.GetVisibleIndex(FocusedRowHandle); }
		protected internal override void OnMoveNextPage() {
			int prevHandle = FocusedRowHandle, newFocusedIndex;
			if(View.OptionsNavigation.UseOfficePageNavigation && ScrollPageSize > 0) {
				if(GetFocusedRowIndex() < TopRowIndex + ScrollPageSize - 1) {
					newFocusedIndex = TopRowIndex + ScrollPageSize - 1;
				}
				else {
					TopRowIndex = GetFocusedRowIndex() + 1;
					newFocusedIndex = TopRowIndex + ScrollPageSize - 1;
				}
				if(newFocusedIndex >= RowCount) newFocusedIndex = RowCount - 1;
				FocusedRowHandle = GetVisibleRowHandle(newFocusedIndex);
				View.DoAfterMoveFocusedRow(new KeyEventArgs(Keys.PageDown | Control.ModifierKeys), prevHandle, null, null);
				return;
			}
			MoveTopRowIndex(ScrollPageSize);
		}
		protected internal override void OnMovePrevPage() {
			int prevHandle = FocusedRowHandle;
			if(View.OptionsNavigation.UseOfficePageNavigation) {
				if(TopRowIndex < GetFocusedRowIndex()) {
					FocusedRowHandle = GetVisibleRowHandle(TopRowIndex);
					View.DoAfterMoveFocusedRow(new KeyEventArgs(Keys.PageUp | Control.ModifierKeys), prevHandle, null, null);
					return;
				}
			}
			MoveTopRowIndex(-ScrollPageSize);
		}
		protected internal override void OnMakeRowVisibleCore(int rowHandle, bool invalidate) {
			bool isReady = View.IsInitialized && ViewInfo.IsReady && !View.ViewRect.IsEmpty;
			RowVisibleState rs = View.IsRowVisible(rowHandle);
			int vIndex = View.GetVisibleIndex(rowHandle);
			if(vIndex == View.TopRowIndex && vIndex >= 0) return;
			if(!View.IsLockUpdate && isReady) {
				if(rs == RowVisibleState.Visible) {
					if(invalidate) View.InvalidateRow(rowHandle);
					return;
				}
				if(rs == RowVisibleState.Partially) {
					while(View.IsRowVisible(rowHandle) == RowVisibleState.Partially) {
						int prevTopRowIndex = TopRowIndex + 1;
						TopRowIndex++;
						if(TopRowIndex != prevTopRowIndex) break;
					}
					return;
				}
			}
			View.DataController.MakeRowVisible(rowHandle);
			if(View.IsLockUpdate || !isReady) return;
			vIndex = View.GetVisibleIndex(rowHandle);
			if(vIndex != -1)
				TopRowIndex = View.CheckTopRowIndex(vIndex);
		}
	}
	public class ScrollPositionInfoBase {
		Rectangle bottomRowBounds;
		int bottomRowHandle;
		protected int BottomRowHandle { get { return bottomRowHandle; } }
		protected Rectangle BottomRowBounds { get { return bottomRowBounds; } }
		public virtual bool IsEmpty { get { return true; } }
		public virtual void Clear() {
			this.bottomRowHandle = GridControl.InvalidRowHandle;
			this.bottomRowBounds = Rectangle.Empty;
		}
		public virtual void InvalidateNonScrollableArea(GridViewInfo viewInfo) {
		}
		public virtual Rectangle GetCurrentScrollableBounds(GridViewInfo viewInfo) {
			return GetScrollableBounds(viewInfo);
		}
		public virtual Rectangle GetScrollableBounds(GridViewInfo viewInfo) {
			Rectangle res = viewInfo.ViewRects.Rows;
			GridRowInfo row = viewInfo.RowsInfo.GetLastNonScrollableRow(viewInfo.View, false);
			int rowBottomBounds = row == null ? res.Y : row.TotalBounds.Bottom;
			if(viewInfo.View.AllowFixedGroups) {
				int index = row == null ? 0 : viewInfo.RowsInfo.IndexOf(row) + 1;
				while(index < viewInfo.RowsInfo.Count) {
					GridRowInfo checkRow = viewInfo.RowsInfo[index];
					if(checkRow.IsGroupRow && checkRow.IsGroupRowExpanded) {
						row = checkRow;
						index++;
						continue;
					}
					break;
				}
				if(row != null) rowBottomBounds = Math.Max(rowBottomBounds, row.TotalBounds.Bottom);
			}
			if(row != null && rowBottomBounds > res.Y) {
				res.Y = rowBottomBounds;
				res.Height = viewInfo.ViewRects.Rows.Bottom - res.Y;
			}
			return res;
		}
		public virtual Rectangle GetTotalRowsScrollableBounds(GridViewInfo viewInfo) {
			Rectangle res = viewInfo.ViewRects.Rows;
			GridRowInfo row = viewInfo.RowsInfo.GetLastNonScrollableRow(viewInfo.View, false);
			if(row != null) {
				res.Y = row.TotalBounds.Bottom;
				res.Height = viewInfo.ViewRects.Rows.Bottom - res.Y;
			}
			return res;
		}
		public virtual void Save(GridViewInfo viewInfo) {
			Clear();
			GridRowInfo row = GetTopScrollableRow(viewInfo), bottom = null;
			if(row == null || !viewInfo.IsReady) return;
			for(int n = viewInfo.RowsInfo.Count - 1; n >= 0; n--) {
				GridRowInfo r = viewInfo.RowsInfo[n];
				if(r == row) return;
				if(viewInfo.ViewRects.Rows.Bottom >= r.TotalBounds.Bottom) {
					bottom = r;
					break;
				}
			}
			if(bottom == null) return;
			this.bottomRowHandle = bottom.RowHandle;
			this.bottomRowBounds = bottom.Bounds;
		}
		protected GridRowInfo GetTopScrollableRow(GridViewInfo viewInfo) {
			if(!viewInfo.IsReady) return null;
			return viewInfo.RowsInfo.GetFirstScrollableRow(viewInfo.View);
		}
		public virtual Point GetDistance(GridViewInfo viewInfo) { 
			return Point.Empty;
		}
	}
	public class PixelScrollPositionInfo : ScrollPositionInfo {
		int topPixelPosition = -1, lastTopPixelPosition = -1;
		Rectangle scrollableBounds = Rectangle.Empty;
		public override bool IsEmpty { get { return topPixelPosition < 0; } }
		public override void Clear() {
			base.Clear();
			this.topPixelPosition = -1;
		}
		public override void Save(GridViewInfo viewInfo) {
			base.Save(viewInfo);
			this.lastTopPixelPosition = this.topPixelPosition = viewInfo.View.TopRowPixel;
			this.scrollableBounds = GetScrollableBounds(viewInfo);
		}
		public override Rectangle GetCurrentScrollableBounds(GridViewInfo viewInfo) {
			return scrollableBounds;
		}
		public override void InvalidateNonScrollableArea(GridViewInfo viewInfo) {
			if(!viewInfo.View.AllowFixedGroups) return;
			Rectangle rect = GetScrollableBounds(viewInfo);
			Rectangle prev = scrollableBounds;
			Rectangle inv = viewInfo.ViewRects.Rows;
			inv.Height = Math.Max(prev.Top, rect.Top) - inv.Top;
			if(inv.Height > 0) viewInfo.View.InvalidateRect(inv);
		}
		public override Point GetDistance(GridViewInfo viewInfo) { 
			Point res = Point.Empty;
			if(IsEmpty) return res;
			Rectangle rect = GetCurrentScrollableBounds(viewInfo);
			res.X = this.topPixelPosition - viewInfo.View.TopRowPixel;
			if(Math.Abs(res.X) > 0) {
				res.Y = rect.Y;
				if(res.X < 0) {
					res.Y = rect.Y + Math.Abs(res.X);
				}
			}
			return res;
		}
	}
	public class ScrollPositionInfo : ScrollPositionInfoBase {
		Rectangle topRowBounds;
		int topRowHandle;
		public ScrollPositionInfo() {
			Clear();
		}
		protected Rectangle TopRowBounds { get { return topRowBounds; } }
		public override bool IsEmpty { get { return topRowHandle == GridControl.InvalidRowHandle; } }
		public override void Clear() {
			base.Clear();
			this.topRowHandle = GridControl.InvalidRowHandle;
			this.topRowBounds = Rectangle.Empty;
		}
		public override void Save(GridViewInfo viewInfo) {
			base.Save(viewInfo);
			GridRowInfo row = GetTopScrollableRow(viewInfo);
			if(row == null || !viewInfo.IsReady) return;
			this.topRowHandle = row.RowHandle;
			this.topRowBounds = row.Bounds;
		}
		public override Point GetDistance(GridViewInfo viewInfo) { 
			Point res = Point.Empty;
			if(IsEmpty || !viewInfo.IsReady) return res;
			GridRowInfo tr = GetTopScrollableRow(viewInfo);
			if(tr == null || tr.Bounds.Y != TopRowBounds.Y) return res;
			Rectangle bounds = Rectangle.Empty;
			GridRowInfo row = viewInfo.RowsInfo.GetInfoByHandle(this.topRowHandle);
			if(row != null) {
				bounds = row.Bounds;
				if(bounds.Y > this.topRowBounds.Y) {
					res.X = bounds.Y - this.topRowBounds.Y;
					res.Y = this.topRowBounds.Y;
					return res;
				}
			}
			if(BottomRowHandle == GridControl.InvalidRowHandle) return res;
			row = viewInfo.RowsInfo.GetInfoByHandle(BottomRowHandle);
			if(row == null) return Point.Empty;
			bounds = row.Bounds;
			if(bounds.Y < BottomRowBounds.Y) {
				res.X = bounds.Y - BottomRowBounds.Y;
				res.Y = tr.Bounds.Y + Math.Abs(res.X);
			}
			return res;
		}
	}
}

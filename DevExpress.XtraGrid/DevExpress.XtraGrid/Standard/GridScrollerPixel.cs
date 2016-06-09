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
	public class GridViewByPixelScroller : GridViewScroller, ISupportAnimatedScroll {
		AnimatedScrollHelper scrollHelper;
		public GridViewByPixelScroller(GridView view)
			: base(view) {
			this.scrollHelper = new AnimatedScrollHelper(this);
		}
		bool IsTopRowFullyVisible() {
			if(!View.IsPixelScrolling) return true;
			return View.TopRowPixel - (ViewInfo.CalcPixelPositionByRow(View.TopRowIndex)) == 0;
		}
		int CalcTopRowIndex(GridViewInfo viewInfo, int topRowPixel) {
			int res = viewInfo.CalcVisibleRowByPixel(topRowPixel);
			return res;
		}
		protected internal override GridRowCollection LoadRowsCore(GridViewInfo viewInfo, GridRowsLoadInfo e) {
			int maxRowsHeight = e.MaxRowsHeight < 0 ? viewInfo.ViewRects.Rows.Height : e.MaxRowsHeight;
			int topRowIndex = 0;
			int topRowPixel = e.TopVisibleRowIndex;
			int rowHeight;
			if(!e.IsPixelIndex && e.TopVisibleRowIndex > -1) topRowPixel = viewInfo.CalcPixelPositionByRow(e.TopVisibleRowIndex);
			if(e.TopVisibleRowIndex == -1) topRowPixel = viewInfo.ViewTopRowPixel;
			topRowIndex = CalcTopRowIndex(viewInfo, topRowPixel);
			GridRowCollection rows = e.CalcOnly ? null : new GridRowCollection();
			GraphicsInfo ginfo = viewInfo.GInfo;
			e.VisibleRowCount = View.RowCount;
			bool showNewItemRowAtTop = viewInfo.NewItemRow == NewItemRowPosition.Top, showFilterRow = View.OptionsView.ShowAutoFilterRow;
			if(!e.UseMinRowHeight) ginfo.AddGraphics(e.Graphics);
			try {
				int topRowHandle = View.GetVisibleRowHandle(topRowIndex);
				if(topRowHandle == GridControl.InvalidRowHandle)
					topRowIndex = GridControl.InvalidRowHandle;
				List<int> parentRows = GetParentRowsTree(topRowHandle);
				GridRow prevRow = null;
				int index = 0;
				while((showFilterRow || showNewItemRowAtTop || topRowIndex != GridControl.InvalidRowHandle) && e.ResultRowsHeight <= maxRowsHeight) {
					int rowHandle = 0;
					bool ignoreHeight = false;
					bool forcedRow = false;
					bool forcedRowLight = false;
					bool skipNextRow = false;
					int actualVisibleIndex = topRowIndex;
					if(showFilterRow) rowHandle = GridControl.AutoFilterRowHandle;
					else {
						if(showNewItemRowAtTop) {
							rowHandle = CurrencyDataController.NewItemRow;
						}
						else {
							rowHandle = View.GetVisibleRowHandle(topRowIndex);
							if(index == 0) {
								if(parentRows != null) {
									forcedRowLight = forcedRow = true;
									int nextRow = View.GetVisibleRowHandle(View.GetNextVisibleRow(topRowIndex));
									int levelNext = View.GetRowLevel(nextRow);
									if(!View.IsValidRowHandle(nextRow) ||  levelNext < View.GetRowLevel(rowHandle)) forcedRow = false;
									rowHandle = parentRows[0];
									actualVisibleIndex = View.GetVisibleIndex(rowHandle);
									if(View.IsGroupRow(rowHandle)) {
										if(levelNext > View.GetRowLevel(rowHandle)) forcedRow = true;
									}
									parentRows.RemoveAt(0);
									if(parentRows.Count == 0) {
										parentRows = null;
										index++;
									}
									skipNextRow = true;
									if(!forcedRow) ignoreHeight = true;
								}
							}
						}
					}
					rowHeight = (e.UseMinRowHeight ? viewInfo.GetActualMinRowHeight(View.IsGroupRow(rowHandle)) : viewInfo.CalcTotalRowHeight(ginfo.Graphics, 0, rowHandle, topRowIndex, -1, View.IsGroupRow(rowHandle) ? (bool?)null : (bool?)true));
					if(!e.CalcOnly) {
						GridRow row = rows.Add(rowHandle, actualVisibleIndex, View.GetRowLevel(rowHandle), e.UseMinRowHeight ? 0 : rowHeight, View.GetRowKey(rowHandle), forcedRow);
						row.ForcedRowLight = forcedRowLight;
						if(prevRow != null && prevRow.RowHandle < 0) {
							prevRow.NextRowPrimaryChild = View.GetChildRowHandle(prevRow.RowHandle, 0) == rowHandle;
						}
						prevRow = row;
					}
					e.ResultRowCount++;
					if(forcedRow || (!forcedRow && parentRows == null)) {
						if(!ignoreHeight) e.ResultRowsHeight += rowHeight;
					}
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
					if(skipNextRow) continue;
					topRowIndex = View.GetNextVisibleRow(topRowIndex);
				}
			}
			finally {
				if(!e.UseMinRowHeight) ginfo.ReleaseGraphics();
			}
			if(topRowIndex == GridControl.InvalidRowHandle && e.ResultRowsHeight <= e.MaxRowsHeight) {
				if(ViewInfo.ViewRects.Rows.Height >= e.ResultRowsHeight) e.ResultAllRowsFit = true;
			}
			e.ResultRows = rows;
			return rows;
		}
		public virtual int GetScrollablePageSize() {
			int res = ViewInfo.RowsInfo.GetScrollableRowsHeight(View, ViewInfo.ViewRects.Rows.Bottom);
			res = Math.Max(res, ViewInfo.ActualDataRowMinRowHeight);
			return res;
		}
		DateTime lastSmoothScrollRow = DateTime.MinValue;
		bool CheckLastKeyboardScroll() {
			DateTime lastScroll = this.lastSmoothScrollRow;
			this.lastSmoothScrollRow = DateTime.Now;
			if(DateTime.Now.Subtract(lastScroll).TotalMilliseconds < 300) {
				return true;
			}
			return false;
		}
		protected bool CheckInvalidFocusedRowHandle() {
			if(FocusedRowHandle == DataController.InvalidRow) {
				FocusedRowHandle = GetVisibleRowHandle(0);
				return true;
			}
			return false;
		}
		protected internal override void OnMovePrevPage() {
			if(CheckInvalidFocusedRowHandle()) return;
			GridRowInfo row = ViewInfo.RowsInfo.GetFirstScrollableRow(View);
			while(row != null && View.IsRowVisible(row.RowHandle) == RowVisibleState.Partially) {
				row = ViewInfo.RowsInfo.NextRow(row);
			}
			if(row != null && row.RowHandle != FocusedRowHandle && View.GetVisibleIndex(FocusedRowHandle) > row.VisibleIndex) {
				OnDoChangedFocusedRow(row.RowHandle, new KeyEventArgs(Keys.PageUp | Control.ModifierKeys));
				return;
			}
			if(row == null) {
				SmoothScroll(-GetScrollablePageSize(), true);
				return;
			}
			int height = ScrollInfo.GetTotalRowsScrollableBounds(ViewInfo).Height;
			int target = ViewInfo.PositionHelper.CalcVisibleRowByPixel(row.VisibleIndex, -1, height - 1, false);
			OnDoChangedFocusedRow(GetVisibleRowHandle(target), new KeyEventArgs(Keys.PageUp | Control.ModifierKeys));
		}
		protected internal override void OnMoveFirst() {
			OnDoChangedFocusedRow(View.GetVisibleRowHandle(0), new KeyEventArgs(Keys.Home | Control.ModifierKeys));
		}
		protected internal override void OnMoveLastVisible() {
			OnDoChangedFocusedRow(GetVisibleRowHandle(RowCount - 1), new KeyEventArgs(Keys.End | Control.ModifierKeys));
		}
		protected internal override void OnMoveNextPage() {
			if(CheckInvalidFocusedRowHandle()) return;
			int lastRow = ViewInfo.RowsInfo.GetLastVisibleRowIndex();
			if(View.IsRowVisible(View.GetVisibleRowHandle(lastRow)) == RowVisibleState.Partially) lastRow = Math.Max(0, lastRow - 1);
			int prev = FocusedRowHandle;
			int currentIndex = View.GetVisibleIndex(FocusedRowHandle);
			int current = ViewInfo.CalcPixelPositionByRow(currentIndex);
			int delta = GetScrollablePageSize();
			int nextRow = ViewInfo.CalcVisibleRowByPixel(current + delta);
			if(IsScrolling) {
				OnDoChangedFocusedRow(GetVisibleRowHandle(nextRow), new KeyEventArgs(Keys.PageDown | Control.ModifierKeys));
				return;
			}
			if(currentIndex < lastRow) {
				FocusedRowHandle = GetVisibleRowHandle(lastRow);
				View.DoAfterMoveFocusedRow(new KeyEventArgs(Keys.PageDown | Control.ModifierKeys), prev, null, null);
				return;
			}
			OnDoChangedFocusedRow(GetVisibleRowHandle(nextRow), new KeyEventArgs(Keys.PageDown | Control.ModifierKeys));
		}
		public override void OnVScroll(int newPosition) {
			View.TopRowPixel = newPosition;
		}
		int counter = 0;
		void Debug(string text, params object[] args) {
			System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}", counter++, string.Format(text, args)));
		}
		protected internal override void OnTouchScroll(GestureArgs info, Point delta, ref Point overPan) {
			if(info.GF == GF.BEGIN) {
				touchStart.X = View.LeftCoord;
				touchStart.Y = View.TopRowPixel;
				touchOverpan = Point.Empty;
				return;
			}
			if(delta.Y != 0) {
				int prevTop = View.TopRowPixel;
				View.TopRowPixel -= delta.Y;
				if(prevTop == View.TopRowPixel)
					touchOverpan.Y += delta.Y;
				else
					touchOverpan.Y = 0;
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
		protected internal override void OnMouseWheelScroll(MouseWheelScrollClientArgs e) {
			if(View.IsInplaceEditFormVisible) return;
			int height = ScrollInfo.GetScrollableBounds(ViewInfo).Height;
			int wheelDistance = 0;
			bool useSmoothScroll = true;
			if (!e.InPixels) {
				wheelDistance = (e.Distance < 0 ? -1 : 1) * Math.Min(height, Math.Max(300, height / 3));
			}
			else {
				wheelDistance = e.Distance;
				useSmoothScroll = false;
			}
			if(wheelDistance + View.TopRowPixel + height > ViewInfo.VisibleRowsHeight) wheelDistance = Math.Max(0, ViewInfo.VisibleRowsHeight - View.TopRowPixel - height);
			if (useSmoothScroll) {
			SmoothScroll(wheelDistance, false);
		}
			else {
				CancelSmoothScroll();
				View.TopRowPixel += wheelDistance;
			}
		}
		protected internal override void OnMakeRowVisibleCore(int rowHandle, bool invalidate) {
			if(!AllowMakeRowVisible) return;
			bool isReady = View.IsInitialized && ViewInfo.IsReady && !View.ViewRect.IsEmpty;
			RowVisibleState rs = View.IsRowVisible(rowHandle);
			int vIndex = View.GetVisibleIndex(rowHandle);
			int pixelPosition = vIndex >= 0 ? ViewInfo.CalcPixelPositionByRow(vIndex) : -1;
			if(!View.IsLockUpdate && isReady) {
				if(rs == RowVisibleState.Visible) {
					if(invalidate) View.InvalidateRow(rowHandle);
					return;
				}
				if(rs == RowVisibleState.Partially) {
					if(vIndex > View.TopRowIndex) {
						while(View.IsRowVisible(rowHandle) == RowVisibleState.Partially) {
							int prevTopRowIndex = View.TopRowIndex + 1;
							View.TopRowIndex++;
							if(View.TopRowIndex != prevTopRowIndex) break;
						}
						if(View.IsRowVisible(rowHandle) == RowVisibleState.Visible) return;
					}
				}
			}
			View.DataController.MakeRowVisible(rowHandle);
			if(View.IsLockUpdate || !isReady) return;
			vIndex = View.GetVisibleIndex(rowHandle);
			if(vIndex != -1) {
				View.TopRowPixel = ViewInfo.CalcPixelPositionByRow(vIndex);
			}
		}
		protected override ScrollPositionInfo CreateScrollInfo() {
			return new PixelScrollPositionInfo();
		}
		protected VCrkScrollBar ScrollBar { get { return View != null && View.ScrollInfo != null ? View.ScrollInfo.VScroll : null;  } }
		public override void CancelSmoothScroll() {
			this.scrollHelper.Cancel();
		}
		protected internal override void OnDoChangedFocusedRow(int newFocusedRowHandle, KeyEventArgs e) {
			RowVisibleState visibilty = View.IsRowVisible(newFocusedRowHandle);
			RowVisibleState oldVisibilty = View.IsRowVisible(View.FocusedRowHandle);
			if(newFocusedRowHandle == FocusedRowHandle) {
				MakeFocusedRowVisibleSmooth();
				return;
			}
			int prev = FocusedRowHandle;
			int currentIndex = View.GetVisibleIndex(View.FocusedRowHandle);
			int newIndex = View.GetVisibleIndex(newFocusedRowHandle);
			int newIndexPosition = ViewInfo.CalcPixelPositionByRow(newIndex);
			int distance = newIndexPosition - ViewInfo.CalcPixelPositionByRow(currentIndex);
			if(oldVisibilty == RowVisibleState.Partially) distance *= 2;
			if(e.KeyCode == Keys.Up) {
				distance = newIndexPosition - View.TopRowPixel;
			}
			if(currentIndex < 0 || newIndex < 0 || Math.Abs(distance) > GetScrollablePageSize() || visibilty == RowVisibleState.Visible || oldVisibilty == RowVisibleState.Hidden) {
				CancelSmoothScroll();
				if(visibilty != RowVisibleState.Visible) {
					MakeRowVisibleByKey(newFocusedRowHandle, newIndex, e.KeyData);
				} else
					View.FocusedRowHandle = newFocusedRowHandle;
				View.DoAfterMoveFocusedRow(e, prev, null, null);
				return;
			}
			SmoothScroll(distance, true);
			if(IsScrolling) AllowMakeRowVisible = false;
			View.FocusedRowHandle = newFocusedRowHandle;
			View.DoAfterMoveFocusedRow(e, prev, null, null);
		}
		void MakeFocusedRowVisibleSmooth() {
			RowVisibleState visibilty = View.IsRowVisible(FocusedRowHandle);
			if(visibilty == RowVisibleState.Hidden) {
				MakeFocusedRowVisible();
				return;
			}
			if(visibilty == RowVisibleState.Visible) return;
			bool down = true;
			int distance = ViewInfo.ActualDataRowMinRowHeight;
			GridRowInfo row = ViewInfo.GetGridRowInfo(FocusedRowHandle);
			if(row != null) {
				if(ViewInfo.RowsInfo.GetFirstScrollableRow(View) == row) {
					Rectangle scrollable = ScrollInfo.GetScrollableBounds(ViewInfo);
					if(row.TotalBounds.Y < scrollable.Y) {
						distance = scrollable.Top - row.TotalBounds.Y;
						down = false;
					}
					else {
						return;
					}
				}
				else {
					distance = row.TotalBounds.Height;
					if(row.TotalBounds.Bottom < ViewInfo.ViewRects.Rows.Bottom) down = false;
				}
			}
			SmoothScroll(distance * (down ? 1 : -1), true);
		}
		void MakeRowVisibleByKey(int newFocusedRowHandle, int newVisibleIndex, Keys byKey) {
			try {
				AllowMakeRowVisible = false;
				View.FocusedRowHandle = newFocusedRowHandle;
				if(View.FocusedRowHandle != newFocusedRowHandle) return;
				if(byKey == Keys.PageDown) {
					View.TopRowPixel = ViewInfo.CalcPixelPositionByRow(newVisibleIndex + 1) - GetScrollablePageSize();
				}
				else {
					View.TopRowPixel = ViewInfo.CalcPixelPositionByRow(newVisibleIndex);
				}
			}
			finally {
				AllowMakeRowVisible = true;
			}
		}
		public virtual void SmoothScroll(int distance, bool byKeyboard) {
			if(ScrollBar == null) return;
			int target = View.CheckTopRowPixel(ScrollBar.Value + distance);
			if(byKeyboard && CheckLastKeyboardScroll()) {
				CancelSmoothScroll();
				ScrollBar.Value = target;
				return;
			}
			float time = 1.0f;
			if(Math.Abs(distance) < 30) time = 0.6f;
			this.scrollHelper.Scroll(ScrollBar.Value, target, time, true);
		}
		#region ISupportAnimatedScroll Members
		void ISupportAnimatedScroll.OnScroll(double currentScrollValue) {
			if(ScrollBar == null) return;
			int current = (int)Math.Round(currentScrollValue);
			ScrollBar.Value = current;
		}
		protected bool ShowEditorAfterScrollFinish { get; set; }
		protected bool MakeRowVisibleScrollFinish { get; set; }
		protected int ShowingEditor { get; set; }
		void ISupportAnimatedScroll.OnScrollFinish() {
			AllowMakeRowVisible = true;
			if(MakeRowVisibleScrollFinish) {
				MakeRowVisibleScrollFinish = false;
				MakeFocusedRowVisible();
			}
			ShowingEditor++;
			try {
				if(ShowEditFormAfterScrollFinish) {
					ShowEditFormAfterScrollFinish = false;
					View.ShowInplaceEditForm();
				}
				if(ShowEditorAfterScrollFinish) {
					ShowEditorAfterScrollFinish = false;
					View.ShowEditor();
				}
			}
			finally {
				ShowingEditor--;
			}
			View.InvalidateRow(FocusedRowHandle);
		}
		protected void MakeFocusedRowVisible() {
			if(!ViewInfo.IsReady) View.CheckViewInfo();
			if(!ViewInfo.IsReady || !View.IsValidRowHandle(View.FocusedRowHandle)) return;
			if(View.IsRowVisible(View.FocusedRowHandle) == RowVisibleState.Visible) return;
			View.TopRowPixel = ViewInfo.CalcPixelPositionByRow(View.GetVisibleIndex(View.FocusedRowHandle));
		}
		protected internal override bool AllowDrawFocusRectangle { get { return !IsScrolling; } }
		protected internal override bool ShowEditor() {
			if(IsScrolling) {
				ShowEditorAfterScrollFinish = true;
				return false;
			}
			if(View.IsRowVisible(FocusedRowHandle) == RowVisibleState.Partially) {
				MakeFocusedRowVisibleSmooth();
				if(IsScrolling) {
					if(ShowingEditor > 0) return false;
					ShowEditorAfterScrollFinish = true;
					return false;
				}
			}
			return true;
		}
		protected bool IsScrolling { get { return scrollHelper.Animating; } }
		protected internal override bool ShowEditForm(int requiredHeight) {
			if(!View.IsAllowEditForm(View.FocusedRowHandle)) return false;
			int pixelTop = ViewInfo.CalcPixelPositionByRow(View.GetVisibleIndex(View.FocusedRowHandle));
			if(View.TopRowPixel > pixelTop) {
				View.TopRowPixel = pixelTop;
				return true;
			}
			if(View.TopRowPixel <= pixelTop) {
				GridDataRowInfo row = ViewInfo.GetGridRowInfo(View.FocusedRowHandle) as GridDataRowInfo;
				if(row != null && row.TotalBounds.Bottom + requiredHeight < ViewInfo.ViewRects.Rows.Bottom) return true;
			}
			int spaceRequired = GetScrollablePageSize() - ViewInfo.CalcTotalRowHeight(null, 0, View.FocusedRowHandle, -1, -1, true);
			if(spaceRequired <= 0) {
				if(spaceRequired < -50) return false;
				View.TopRowPixel = pixelTop;
			}
			else {
				View.TopRowPixel += (pixelTop - View.TopRowPixel) - spaceRequired;
			}
			return true;
		}
		#endregion
		public bool ShowEditFormAfterScrollFinish { get; set; }
	}
}

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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Layout.Modes;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraLayout;
namespace DevExpress.XtraGrid.Views.Layout.Handler {
	public class LayoutViewHandler : BaseViewHandler {
		internal LayoutViewHitInfo downPointHitInfo;
		internal GridCell prevFocusedCell;
		MouseDragSelectionHelper dragSelectionHelperCore = null;
		public new LayoutView View { get { return base.View as LayoutView; } }
		public new LayoutViewInfo ViewInfo { get { return base.ViewInfo as LayoutViewInfo; } }
		public LayoutViewHandler(LayoutView view)
			: base(view) {
			downPointHitInfo = new LayoutViewHitInfo();
			prevFocusedCell = new GridCell(0, null);
			dragSelectionHelperCore = new MouseDragSelectionHelper(view);
		}
		protected MouseDragSelectionHelper DragSelectionHelper {
			get { return dragSelectionHelperCore; }
		}
		Keys[] navKeys = { Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp, Keys.End, Keys.Home, Keys.Tab };
		protected override void OnResize(Rectangle clientRect) {
			View.ScrollInfo.OnAction(XtraEditors.ScrollNotifyAction.Resize);
			View.InternalSetViewRectCore(clientRect);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			bool prevEditorOpen = false;
			DoCancelUpdateOnKey(e);   
			if(DoSwitchViewModeOnKey(e)) return;   
			if(DoRefreshDataOnKey(e)) return;   
			if(DoActivatePanModeOnKey(e)) return;   
			if(DoActivateCustomizationOnKey(e)) return;   
			if(DoCopyToClipboardOnKey(e)) return;   
			if(DoSelectAllVisibleCardsOnKey(e)) return;   
			if(DoHideCardsCaptionOnKey(e)) return;   
			if(DoSwitchArrangeStrategyCaptionOnKey(e)) return;   
			if(View.State == LayoutViewState.Editing) {
				if(e.KeyCode == Keys.Escape)
					View.HideEditor();
				else {
					prevEditorOpen = true;
					View.PostEditor();
					View.HideEditor();
					if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape) return;
				}
			}
			if(View.State == LayoutViewState.Normal) {
				if(View.CheckShowFindPanelKey(e)) return;
				if(e.KeyCode == Keys.Escape) {
					View.ResetPanModeIfNeed();
				}
				if(DoExpandCollapseOnKey(e)) return;
				if(DoInvertFocusedRowSelectionOnKey(e)) return;
				if(DoShowEditorOnKey(e)) return;
				if(Array.IndexOf(navKeys, e.KeyCode) != -1) {
					if(DoLeaveFocusOnTab(e)) return;
					DoNavigationOnKey(e);
					if(prevEditorOpen && View.IsKeyboardFocused) {
						View.ShowEditor();
					}
					return;
				}
				if(e.KeyCode == Keys.Enter) { View.ShowEditor(); }
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) { base.OnKeyUp(e); }
		static char[] keyPress = new char[] { (char)27, (char)9, (char)8, (char)3 };
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.KeyChar == 6 && View.FindPanel != null) {
				e.Handled = true;
			}
			if(e.Handled) return;
			if(!View.IsEditing && Array.IndexOf(keyPress, e.KeyChar) == -1) {
				if(e.KeyChar == '+' || e.KeyChar == '-' && View.FocusedColumn == null) return;
				if(e.KeyChar != 27)
					View.ShowEditorByKeyPress(e);
			}
		}
		public override bool ProcessKey(KeyEventArgs e) {
			if(View.ActiveEditor != null) {
				BaseEdit be = View.ActiveEditor as BaseEdit;
				if(be != null && be.IsNeededKey(e)) return false;
			}
			if(e.KeyData == (Keys.Tab | Keys.Control)) {
				return false;
			}
			if((e.KeyCode != Keys.Left && e.KeyCode != Keys.Right) &&
				Array.IndexOf(navKeys, e.KeyCode) != -1 || e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter) {
				if((Control.ModifierKeys & Keys.Alt) != 0)
					return false;
				OnKeyDown(e);
				return true;
			}
			return false;
		}
		protected virtual bool Check3DModeHitInfo(MouseEventArgs e) {
			LayoutViewCarouselMode mode = View.ViewInfo.layoutManager as LayoutViewCarouselMode;
			if(mode != null) {
				int hotCardIndex = mode.GetHotCardIndex(new Point(e.X, e.Y));
				if(hotCardIndex >= 0 && hotCardIndex != View.VisibleRecordIndex) {
					if(View.IsMultiSelect) View.ClearSelectionCore();
					View.DoNavigateRecords(hotCardIndex);
					return true;
				}
			}
			return false;
		}
		protected bool DoMouseWheel3DMode(DXMouseEventArgs e) {
			LayoutViewCarouselMode mode = View.ViewInfo.layoutManager as LayoutViewCarouselMode;
			if(mode != null) {
				int delta = e.Delta / 120;
				if(delta != 0) {
					if(View.IsEditing)
						return false;
					View.DoMoveFocusedRow(delta, new KeyEventArgs(Keys.None));
					e.Handled = true;
					return true;
				}
			}
			return false;
		}
		protected override bool AllowMouseWheelPixelScrollingHorz {
			get { return CanScrollbyPixels && View.ViewInfo.NavigationHScrollNeed; }
		}
		protected override bool AllowMouseWheelPixelScrollingVert {
			get { return CanScrollbyPixels && View.ViewInfo.NavigationVScrollNeed; }
		}
		bool CanScrollbyPixels {
			get { return !IsCarouselMode && View.OptionsBehavior.AllowMouseWheelSmoothScrolling == DefaultBoolean.True; }
		}
		bool IsCarouselMode {
			get { return View.ViewInfo.layoutManager is LayoutViewCarouselMode; } 
		}
		protected override bool OnMouseWheel(MouseWheelScrollClientArgs e) {
			try {
				if(!e.InPixels)
					return DoMouseWheelCore(e);
				else
					return DoMouseWheelPixelScrollCore(e);
			}
			finally {
				if(View != null && View.ScrollInfo != null && View.ScrollInfo.VScroll != null && View.ScrollInfo.VScroll.Visible)
					e.Handled = true;
			}
		}
		protected override bool OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ea = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseWheel(ea);
			return DoMouseWheelCore(ea);
		}
		class MouseWheelPixelScroller : Utils.Drawing.Animation.ISupportAnimatedScroll {
			LayoutView view;
			Utils.Drawing.Animation.AnimatedScrollHelper scrollHelper;
			static Point overPan = Point.Empty;
			public MouseWheelPixelScroller(LayoutView view) {
				this.view = view;
				this.scrollHelper = new Utils.Drawing.Animation.AnimatedScrollHelper(this);
			}
			void Utils.Drawing.Animation.ISupportAnimatedScroll.OnScroll(double currentScrollValue) {
				int pos = startPos + (int)Math.Round(currentScrollValue);
				int d = pos - prevPos;
				Point delta = Horizontal ? new Point(d, 0) : new Point(0, d);
				Point currentPoint = Horizontal ? new Point(pos, 0) : new Point(0, pos);
				view.OnTouchScrollDelta(Horizontal, currentPoint, delta, ref overPan);
				prevPos = pos;
			}
			void Utils.Drawing.Animation.ISupportAnimatedScroll.OnScrollFinish() {
				view.OnTouchScrollEnd(Horizontal);
			}
			public bool Horizontal { get; set; }
			int prevPos;
			int startPos;
			public void Scroll(int distance) {
				float time = Math.Abs(distance) < 30 ? 0.6f : 1.0f;
				if(!scrollHelper.Animating) {
					startPos = 0;
					prevPos = 0;
					view.OnTouchScrollBegin(Horizontal, Point.Empty);
				}
				scrollHelper.Scroll(startPos, startPos + distance, time, true);
				startPos += distance;
			}
		}
		MouseWheelPixelScroller wheelScroller;
		bool DoMouseWheelPixelScrollCore(MouseWheelScrollClientArgs ea) {
			if(!CanProcessMouseEvent(ea))
				return true;
			if(View.IsEditing || View.ViewInfo == null || View.ViewInfo.VisibleCards.Count == 0)
				return false;
			View.MouseWheelDelta = ea.Delta;
			try {
				ea.Handled = true;
				if(wheelScroller == null)
					wheelScroller = new MouseWheelPixelScroller(View);
				wheelScroller.Horizontal = ea.Horizontal;
				wheelScroller.Scroll(-ea.Distance);
			}
			finally { View.MouseWheelDelta = 0; }
			return true;
		}
		bool DoMouseWheelCore(DXMouseEventArgs ea) {
			if(!CanProcessMouseEvent(ea)) return true;
			if(!DoMouseWheel3DMode(ea)) {
				ea.Handled = true;
				if(View.IsEditing) return false;
				if(View.ViewInfo == null || View.ViewInfo.VisibleCards.Count == 0) return false;
				bool forward = ea.Delta < 0;
				int firstIndex = View.GetVisibleIndex(View.ViewInfo.GetFirstCardRowHandle());
				int lastIndex = View.GetVisibleIndex(View.ViewInfo.GetLastCardRowHandle());
				View.MouseWheelDelta = ea.Delta;
				int iSmallHChange = View.ViewInfo.CalcHSmallChange(forward);
				int iNewPos = forward ? lastIndex + iSmallHChange : firstIndex - iSmallHChange;
				View.DoNavigateRecords(iNewPos, View.OptionsBehavior.AutoFocusCardOnScrolling);
				View.MouseWheelDelta = 0;
			}
			return true;
		}
		Point ptBeginPan = Point.Empty;
		protected bool CanProcessMouseEvent(DXMouseEventArgs ea) { return !ea.Handled; }
		protected override void OnClick(MouseEventArgs e) {
			DXMouseEventArgs ea = DXMouseEventArgs.GetMouseArgs(e);
			base.OnClick(e);
			if(ea.Handled || View == null) return;
			ProcessClickEvent(ea);
		}
		protected override void OnDoubleClick(MouseEventArgs e) {
			DXMouseEventArgs ea = DXMouseEventArgs.GetMouseArgs(e);
			base.OnDoubleClick(e);
			if(ea.Handled || View == null) return;
			ProcessClickEvent(ea);
		}
		protected virtual void ProcessClickEvent(DXMouseEventArgs e) {
			LayoutViewHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(View.CanRaiseRowCellUserEvents) {
				if(hitInfo.InCard) View.RaiseCardClick(new Events.CardClickEventArgs(e, hitInfo));
				if(hitInfo.InFieldValue) View.RaiseFieldValueClick(new Events.FieldValueClickEventArgs(e, hitInfo));
			}
		}
		protected override bool OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ea = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseDown(ea);
			if(!CanProcessMouseEvent(ea)) return true;
			prevFocusedCell = new GridCell(View.FocusedRowHandle, View.FocusedColumn);
			View.GridControl.FocusViewByMouse(View);
			downPointHitInfo = ViewInfo.CalcHitInfo(e.Location);
			ViewInfo.SelectionInfo.ClearPressedInfo();
			DoRaiseMouseDownOnLayoutItem(downPointHitInfo, e);
			DoChangeFocusedRowOnMouseDown(downPointHitInfo, e);
			if((e.Button & MouseButtons.Left) != 0) { 
				View.GridControl.MouseCaptureOwner = View;
				if(ViewInfo.SelectionInfo.CheckMouseDown(downPointHitInfo)) return true;
				if(Check3DModeHitInfo(e)) return true;
				if(View.PanModeActive) {
					ptBeginPan = e.Location;
					return true;
				}
				if(this.downPointHitInfo.InCard) {
					if(DoCheckCardCollapsedOnMouseDown()) return true;
					if(DoSelectInDesignerOnMouseDown()) return true;
					DoShowEditorOnMouseDown();
					DoCancelMultiselectOnMouseDown();
					return true;
				}
				else {
					if(View.IsMultiSelect) DragSelectionHelper.DoBeginSelectCardsOnMouseDrag(e);
				}
				if(!View.IsDefaultState) View.SetDefaultState();
			}
			return false;
		}
		protected override bool OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ea = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseUp(ea);
			if(!CanProcessMouseEvent(ea)) return true;
			LayoutViewHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			DoRaiseMouseUpOnLayoutItem(hitInfo, e);
			if((e.Button & MouseButtons.Left) != 0) {
				ViewInfo.SelectionInfo.CheckMouseUp(hitInfo);
				if(View != null) {
					if(View.PanModeActive && ptBeginPan != Point.Empty) {
						ptBeginPan = Point.Empty;
						return true;
					}
					if(View.IsMultiSelect) {
						DragSelectionHelper.DoEndSelectCardsOnMouseDrag(e);
						DoCancelMultiselectOnMouseUp(hitInfo);
					}
					if(DoShowEditorOnMouseUp(hitInfo)) 
						return true;
					if(!View.IsEditing) 
						View.SetDefaultState();
				}
			}
			return false;
		}
		protected override bool OnMouseMove(MouseEventArgs e) {
			if(View != null) View.ScrollInfo.OnAction(XtraEditors.ScrollNotifyAction.MouseMove);
			DXMouseEventArgs ea = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseMove(ea);
			if(!CanProcessMouseEvent(ea)) return true;
			LayoutViewHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			View.SetHotCard(hitInfo.HitCard);
			bool resetCursor = ViewInfo.SelectionInfo.CheckHotTrackMouseMove(hitInfo);
			if(e.Button == MouseButtons.Left) {
				if(View.PanModeActive) {
					Size panOffset = new Size(e.X, e.Y) - new Size(ptBeginPan.X, ptBeginPan.Y);
					ptBeginPan = e.Location;
					View.ViewInfo.PanCardArea(panOffset.Width, panOffset.Height);
					View.Invalidate();
					return true;
				}
				if(View.IsMultiSelect) DragSelectionHelper.DoProcessCardSelectionOnMouseDrag(e);
			}
			if(resetCursor) View.ResetCursor();
			return false;
		}
		public override void DoClickAction(BaseHitInfo hitInfo) {
			LayoutViewHitInfo hi = hitInfo as LayoutViewHitInfo;
			switch(hi.HitTest) {
				case LayoutViewHitTest.CardExpandButton: DoExpandCollapseOnClick(hi); break;
				case LayoutViewHitTest.FilterPanelActiveButton: View.ActiveFilterEnabled = !View.ActiveFilterEnabled; break;
				case LayoutViewHitTest.FilterPanelCustomizeButton: View.OnFilterCustomizeClick(); break;
				case LayoutViewHitTest.FilterPanelCloseButton: View.ClearColumnsFilter(); break;
				case LayoutViewHitTest.FilterPanelMRUButton:
				case LayoutViewHitTest.FilterPanelText: View.ShowMRUFilterPopup(); break;
				case LayoutViewHitTest.FieldFilterButton: View.ShowFilterPopup(hi.Column); break;
				case LayoutViewHitTest.FieldSortButton: DoMouseSortColumn(hi.Column, Control.ModifierKeys); break;
				case LayoutViewHitTest.SingleModeButton: DoSingleModeOn(); break;
				case LayoutViewHitTest.RowModeButton: DoRowModeOn(); break;
				case LayoutViewHitTest.ColumnModeButton: DoColumnModeOn(); break;
				case LayoutViewHitTest.MultiRowModeButton: DoMultiRowModeOn(); break;
				case LayoutViewHitTest.MultiColumnModeButton: DoMultiColumnModeOn(); break;
				case LayoutViewHitTest.CarouselModeButton: DoCarouselModeOn(); break;
				case LayoutViewHitTest.PanButton: DoPanModeSwitch(); break;
				case LayoutViewHitTest.CustomizeButton: DoCustomize(); break;
				case LayoutViewHitTest.CloseZoomButton: DoZoomViewSwitch(); break;
				case LayoutViewHitTest.LayoutItem: DoLayoutItemClick(hi); break;
				default: base.DoClickAction(hitInfo); break;
			}
		}
		protected virtual void DoMouseSortColumn(GridColumn column, Keys keys) {
			View.DoMouseSortColumn(column, keys);
		}
		protected virtual void DoExpandCollapseOnClick(LayoutViewHitInfo hi) {
			if(CanExpandCollapse()) {
				View.LockAutoPanByFocusedCard();
				View.SetCardCollapsed(hi.RowHandle, !View.GetCardCollapsed(hi.RowHandle));
				LayoutViewCard cardAfterExpandCollapse = View.FindCardByRow(hi.RowHandle);
				DoChangeFocusedRowOnMouseDownCore(cardAfterExpandCollapse, hi.RowHandle, Keys.LButton);
				View.UnLockAutoPanByFocusedCard();
			}
		}
		protected virtual void DoLayoutItemClick(LayoutViewHitInfo hi) {
			if(hi.LayoutItemHitInfo.IsTabbedGroup) {
				TabbedControlGroup tg = hi.LayoutItem as TabbedControlGroup;
				try {
					DevExpress.XtraTab.ViewInfo.BaseTabHitInfo thi = tg.ViewInfo.BorderInfo.Tab.Handler.ViewInfo.CalcHitInfo(hi.HitPoint);
					if(thi.HitTest == DevExpress.XtraTab.ViewInfo.XtraTabHitTest.PageHeaderButtons) {
						MouseEventArgs ea = new MouseEventArgs(MouseButtons.Left, 0, hi.HitPoint.X, hi.HitPoint.Y, 0);
						tg.ViewInfo.BorderInfo.Tab.Handler.ProcessEvent(EventType.MouseDown, ea);
						tg.ViewInfo.BorderInfo.Tab.Handler.ProcessEvent(EventType.MouseUp, ea);
					}
					else {
						int tpIndex = hi.LayoutItemHitInfo.TabPageIndex;
						if(tpIndex != -1) {
							View.FocusedColumn = null;
							View.FocusedItemName = tg.Name;
							if(tpIndex != tg.SelectedTabPageIndex) {
								tg.SelectedTabPageIndex = tpIndex;
								View.CheckCardDifferences(hi.HitCard);
							}
						}
					}
				}
				catch { }
			}
			if(hi.LayoutItem.IsGroup) {
				DevExpress.XtraLayout.HitInfo.LayoutGroupHitInfo ghi = hi.LayoutItemHitInfo as DevExpress.XtraLayout.HitInfo.LayoutGroupHitInfo;
				LayoutControlGroup gr = hi.LayoutItem as LayoutControlGroup;
				if(ghi.IsExpandButton) {
					gr.Expanded = !gr.Expanded;
					View.CheckCardDifferences(hi.HitCard);
					View.Refresh();
				}
			}
		}
		protected virtual void DoZoomViewSwitch() {
			if(View.IsZoomedView) View.NormalView();
			else View.ZoomView();
		}
		protected virtual bool CanExpandCollapse() {
			return !View.IsCustomizationMode && !View.PanModeActive && View.OptionsBehavior.AllowExpandCollapse;
		}
		protected override void OnMouseEnter(System.EventArgs e) {
			base.OnMouseEnter(e);
			LayoutViewHitInfo hi = (LayoutViewHitInfo)View.CalcHitInfo(View.GridControl.PointToClient(Control.MousePosition));
			ViewInfo.SelectionInfo.OnHotTrackEnter(hi);
			View.SetHotCard(hi.HitCard);
		}
		protected override void OnMouseLeave(System.EventArgs e) {
			base.OnMouseLeave(e);
			ViewInfo.SelectionInfo.OnHotTrackLeave();
			View.SetHotCard(null);
		}
		protected virtual bool DoShowEditorOnMouseUp(LayoutViewHitInfo hitInfo) {
			bool result = false;
			if(View != null && View.IsDefaultState && Control.ModifierKeys == Keys.None) {
				if(View.GetShowEditorMode() != EditorShowMode.MouseDown) {
					if(hitInfo.InCardExpandButton) return true;
					if(hitInfo.InField && this.downPointHitInfo.RowHandle == hitInfo.RowHandle && View.FocusedRowHandle == hitInfo.RowHandle && this.downPointHitInfo.Column == hitInfo.Column) {
						if((View.GetShowEditorMode() == EditorShowMode.Click || View.GetShowEditorMode() == EditorShowMode.MouseDownFocused) 
							&& !this.prevFocusedCell.Equals(new GridCell(View.FocusedRowHandle, View.FocusedColumn))) return true;
						View.ShowEditor();
						result = true;
					}
				}
			}
			return result;
		}
		protected virtual void DoCancelMultiselectOnMouseUp(LayoutViewHitInfo hitInfo) {
			if(CanCancelMultiSelectOnMouseUp(hitInfo)) {
				if(View.SelectedRowsCount != 1 || !View.IsRowSelected(View.FocusedRowHandle)) {
					View.ClearSelectionCore();
					View.SelectFocusedRowCore();
				}
			}
		}
		protected virtual bool CanCancelMultiSelectOnMouseUp(LayoutViewHitInfo hitInfo) {
			bool fCheckFieldUp = downPointHitInfo.InField && View.Editable && downPointHitInfo.Column.OptionsColumn.AllowEdit;
			bool fCheckColumnUp = downPointHitInfo.Column == hitInfo.Column;
			bool fCheckRowUp = (downPointHitInfo.RowHandle == hitInfo.RowHandle) && (View.FocusedRowHandle == hitInfo.RowHandle);
			return fCheckFieldUp && fCheckColumnUp && fCheckRowUp;
		}
		protected virtual bool DoSelectInDesignerOnMouseDown() {
			bool result = false;
			if(View.IsDesignMode) {
				if(downPointHitInfo.Column != null) {
					downPointHitInfo.Column.SelectInDesigner();
					result = true;
				}
			}
			return result;
		}
		protected internal void SelectLayoutItemInDesigner(BaseLayoutItem item) {
			if(View != null && View.IsDesignMode && View.GridControl != null) {
				item = View.FindItemByName(View.TemplateCard, item.Name);
				View.GridControl.SetComponentsSelected(new object[] { item });
			}
		}
		protected virtual void DoCancelMultiselectOnMouseDown() {
			if(!View.IsMultiSelect) return;
			if((View.SelectedRowsCount > 1 || !View.IsRowSelected(View.FocusedRowHandle)) && downPointHitInfo.InCard && Control.ModifierKeys == Keys.None) {
				View.ClearSelectionCore();
				View.SelectFocusedRowCore();
			}
		}
		protected virtual void DoShowEditorOnMouseDown() {
			View.FocusedColumn = View.TryFocusCardColumn(downPointHitInfo.HitCard, downPointHitInfo.Column as LayoutViewColumn);
			GridControl ctrl = View.GridControl;
			if(downPointHitInfo.InField && View.GetShowEditorMode() == EditorShowMode.MouseDown ) {
				View.ShowEditorByMouse();
				ctrl.MouseCaptureOwner = null;
			}
		}
		protected virtual void DoRaiseMouseDownOnLayoutItem(LayoutViewHitInfo hitInfo, MouseEventArgs e) {
			if(View.IsDesignMode && downPointHitInfo.InLayoutItem) {
				SelectLayoutItemInDesigner(downPointHitInfo.LayoutItemHitInfo.Item);
			}
		}
		protected virtual void DoRaiseMouseUpOnLayoutItem(LayoutViewHitInfo hitInfo, MouseEventArgs e) {
		}
		protected virtual bool DoCheckCardCollapsedOnMouseDown() {
			return downPointHitInfo.InCardExpandButton || View.GetCardCollapsed(downPointHitInfo.RowHandle);
		}
		protected virtual void DoChangeFocusedRowOnMouseDown(LayoutViewHitInfo hitInfo, MouseEventArgs e) {
			if(hitInfo.InCard && !hitInfo.InCardExpandButton) {
				DoChangeFocusedRowOnMouseDownCore(hitInfo.HitCard, hitInfo.RowHandle, FromMouseEventArgs(e).KeyData);
			}
		}
		protected void DoChangeFocusedRowOnMouseDownCore(LayoutViewCard hitCard, int rowHandle, Keys key) {
			int prevFocusedRow = View.FocusedRowHandle;
			GridColumn prevFocusedColumn = View.FocusedColumn;
			try {
				View.LockAutoPanByScroll();
				View.LockAutoPanByPartialCard();
				View.DoNavigateRecords(rowHandle, true);
			}
			finally {
				View.UnLockAutoPanByPartialCard();
				View.UnLockAutoPanByScroll();
				View.DoAfterMoveFocusedRow(new KeyEventArgs(key), prevFocusedRow, prevFocusedColumn, null);
			}
		}
		protected virtual bool DoLeaveFocusOnTab(KeyEventArgs e) {
			if(e.Control || (e.KeyCode != Keys.Tab)) return false;
			return View.DoLeaveFocusOnTab(!e.Shift);
		}
		protected virtual bool DoShowEditorOnKey(KeyEventArgs e) {
			bool result = false;
			if(!e.Control && !e.Alt) {
				View.ResetPanModeIfNeed();
				RepositoryItem ritem = View.GetRowCellRepositoryItem(View.FocusedRowHandle, View.FocusedColumn);
				if(ritem != null && ritem.IsActivateKey(e.KeyData)) {
					View.ShowEditorByKey(e);
					result = true;
				}
			}
			return result;
		}
		protected virtual bool DoInvertFocusedRowSelectionOnKey(KeyEventArgs e) {
			bool result = false;
			if(e.KeyCode == Keys.Space && e.Control && View.IsMultiSelect) {
				View.InvertFocusedRowSelectionCore(null);
				result = true;
			}
			return result;
		}
		protected virtual bool DoCopyToClipboardOnKey(KeyEventArgs e) {
			bool result = false;
			if(View.IsDefaultState && (e.KeyData == (Keys.C | Keys.Control) || e.KeyData == (Keys.Insert | Keys.Control))) {
				View.CopyToClipboard();
				e.SuppressKeyPress = true;
				result = true;
			}
			return result;
		}
		protected virtual bool DoSelectAllVisibleCardsOnKey(KeyEventArgs e) {
			bool result = false;
			if(View.IsDefaultState && (e.KeyData == (Keys.A | Keys.Control))) {
				View.SelectAll();
				e.SuppressKeyPress = true;
				result = true;
			}
			return result;
		}
		protected virtual bool DoHideCardsCaptionOnKey(KeyEventArgs e) {
			bool result = false;
#if DEBUG
			if(View.IsDefaultState && (e.KeyData == (Keys.H | Keys.Control))) {
				View.OptionsView.ShowCardCaption = !View.OptionsView.ShowCardCaption;
				e.SuppressKeyPress = true;
				result = true;
			}
#endif
			return result;
		}
		protected virtual bool DoSwitchArrangeStrategyCaptionOnKey(KeyEventArgs e) {
			bool result = false;
#if DEBUG
			if(View.IsDefaultState && (e.KeyData == (Keys.G | Keys.Control))) {
				if(View.OptionsView.CardArrangeRule == LayoutCardArrangeRule.ShowWholeCards) {
					View.OptionsView.CardArrangeRule = LayoutCardArrangeRule.AllowPartialCards;
				}
				else
					View.OptionsView.CardArrangeRule = LayoutCardArrangeRule.ShowWholeCards;
				e.SuppressKeyPress = true;
				result = true;
			}
#endif
			return result;
		}
		protected virtual bool DoSwitchViewModeOnKey(KeyEventArgs e) {
			bool result = false;
			if(View.IsDefaultState) {
				if(e.Alt) {
					switch(e.KeyCode) {
						case Keys.D1: DoSingleModeOn(); break;
						case Keys.D2: DoRowModeOn(); break;
						case Keys.D3: DoColumnModeOn(); break;
						case Keys.D4: DoMultiRowModeOn(); break;
						case Keys.D5: DoMultiColumnModeOn(); break;
						case Keys.D6: DoCarouselModeOn(); break;
						default: return result;
					}
					e.SuppressKeyPress = true;
					result = true;
				}
			}
			return result;
		}
		protected virtual void DoSingleModeOn() {
			if(View.fInternalLockActions) return;
			if(View.OptionsHeaderPanel.EnableSingleModeButton) View.OptionsView.ViewMode = LayoutViewMode.SingleRecord;
		}
		protected virtual void DoRowModeOn() {
			if(View.fInternalLockActions) return;
			if(View.OptionsHeaderPanel.EnableRowModeButton) View.OptionsView.ViewMode = LayoutViewMode.Row;
		}
		protected virtual void DoColumnModeOn() {
			if(View.fInternalLockActions) return;
			if(View.OptionsHeaderPanel.EnableColumnModeButton) View.OptionsView.ViewMode = LayoutViewMode.Column;
		}
		protected virtual void DoMultiRowModeOn() {
			if(View.fInternalLockActions) return;
			if(View.OptionsHeaderPanel.EnableMultiRowModeButton) View.OptionsView.ViewMode = LayoutViewMode.MultiRow;
		}
		protected virtual void DoMultiColumnModeOn() {
			if(View.fInternalLockActions) return;
			if(View.OptionsHeaderPanel.EnableMultiColumnModeButton) View.OptionsView.ViewMode = LayoutViewMode.MultiColumn;
		}
		protected virtual void DoCarouselModeOn() {
			if(View.fInternalLockActions) return;
			if(View.OptionsHeaderPanel.EnableCarouselModeButton) View.OptionsView.ViewMode = LayoutViewMode.Carousel;
		}
		protected virtual void DoPanModeSwitch() {
			if(View.fInternalLockActions) return;
			if(View.OptionsHeaderPanel.EnablePanButton) View.PanModeSwitch();
		}
		protected virtual void DoCustomize() {
			if(View.fInternalLockActions) return;
			if(View.OptionsHeaderPanel.EnableCustomizeButton) View.ShowCustomizationForm();
		}
		protected virtual bool DoRefreshDataOnKey(KeyEventArgs e) {
			bool result = false;
			if(View.IsDefaultState) {
				if(e.KeyCode == Keys.F5) {
					View.RefreshData();
					result = true;
				}
			}
			return result;
		}
		protected virtual bool DoActivatePanModeOnKey(KeyEventArgs e) {
			bool result = false;
			if(View.IsDefaultState) {
				if(e.KeyCode == Keys.F10) {
					DoPanModeSwitch();
					result = true;
					e.SuppressKeyPress = true;
				}
			}
			return result;
		}
		protected virtual bool DoActivateCustomizationOnKey(KeyEventArgs e) {
			bool result = false;
			if(View.IsDefaultState) {
				if(e.KeyCode == Keys.F6) {
					DoCustomize();
					result = true;
					e.SuppressKeyPress = true;
				}
			}
			return result;
		}
		protected virtual bool DoCancelUpdateOnKey(KeyEventArgs e) {
			bool result = false;
			if(View.State == LayoutViewState.Normal && e.KeyCode == Keys.Escape) {
				View.CancelUpdateCurrentRow();
				result = true;
			}
			return result;
		}
		protected virtual bool DoExpandCollapseOnKey(KeyEventArgs e) {
			bool result = false;
			if(!CanExpandCollapse()) return result;
			if(View.FocusedColumn == null || View.GetCardCollapsed(View.FocusedRowHandle)) {
				switch(e.KeyData) {
					case Keys.Add:
						View.SetCardCollapsed(View.FocusedRowHandle, false);
						return true;
					case Keys.Subtract:
						View.SetCardCollapsed(View.FocusedRowHandle, true);
						return true;
				}
			}
			return result;
		}
		protected virtual void DoNavigationOnKey(KeyEventArgs e) {
			View.LockAutoPanByPartialCard();
			int iPageSize = View.ScrollPageSize;
			int iFocusedCardIndex = View.FocusedRowHandle;
			int iVisibleCardIndex = View.VisibleRecordIndex;
			switch(e.KeyCode) {
				case Keys.Tab:
					if(!e.Control) {
						if(string.IsNullOrEmpty(View.FocusedItemName) && !View.CanFocusColumn())
							View.DoNavigateRecords(iFocusedCardIndex + (e.Shift ? -1 : 1), true);
						else
							View.DoMoveFocusedColumn((e.Shift ? -1 : 1), Keys.Tab);
					}
					break;
				case Keys.Left:
					if(!View.ChangeTabByKey(Keys.Left)) {
						int prevPos = View.IsRightToLeft ?
							View.ViewInfo.GetNextCard(iFocusedCardIndex, true) :
							View.ViewInfo.GetPrevCard(iFocusedCardIndex, true);
						View.DoNavigateRecords(prevPos, e);
					}
					break;
				case Keys.Right:
					if(!View.ChangeTabByKey(Keys.Right)) {
						int nextPos = View.IsRightToLeft ?
							View.ViewInfo.GetPrevCard(iFocusedCardIndex, true) :
							View.ViewInfo.GetNextCard(iFocusedCardIndex, true);
						View.DoNavigateRecords(nextPos, e);
					}
					break;
				case Keys.Up:
					if(string.IsNullOrEmpty(View.FocusedItemName))
						View.DoNavigateRecords(View.ViewInfo.GetPrevCard(iFocusedCardIndex, false), e);
					else
						View.DoMoveFocusedColumn(-1, Keys.Up);
					break;
				case Keys.Down:
					if(string.IsNullOrEmpty(View.FocusedItemName) && !View.CanFocusColumn())
						View.DoNavigateRecords(View.ViewInfo.GetNextCard(iFocusedCardIndex, false), e);
					else View.DoMoveFocusedColumn(1, Keys.Down);
					break;
				case Keys.PageUp: View.DoNavigateRecords(iFocusedCardIndex - iPageSize, new KeyEventArgs(Keys.None)); break;
				case Keys.PageDown: View.DoNavigateRecords(iFocusedCardIndex + iPageSize, new KeyEventArgs(Keys.None)); break;
				case Keys.Home: View.DoNavigateRecords(0, e); break;
				case Keys.End: View.DoNavigateRecords(View.RecordCount - 1, e); break;
			}
			View.UnLockAutoPanByPartialCard();
		}
		public override bool NeedKey(NeedKeyType keyType) {
			switch(keyType) {
				case NeedKeyType.Tab: return View.OptionsBehavior.UseTabKey;
				case NeedKeyType.Escape: return View.IsEditing || View.FocusedRowModified;
				case NeedKeyType.Enter: return View.IsEditing;
				case NeedKeyType.Dialog: return View.IsEditing;
			}
			return false;
		}
	}
	public class MouseDragSelectionHelper {
		enum MouseDragSelectionState { Nothing, TryBeginSelection, SelectionInProgress }
		MouseDragSelectionState dragSelectionState = MouseDragSelectionState.Nothing;
		Point dragSelectionStartPoint;
		readonly int startDragSelectionDistance = 3;
		LayoutView viewCore = null;
		public MouseDragSelectionHelper(LayoutView view) {
			this.viewCore = view;
		}
		protected LayoutView View { get { return viewCore; } }
		public virtual void DoBeginSelectCardsOnMouseDrag(MouseEventArgs e) {
			dragSelectionState = MouseDragSelectionState.TryBeginSelection;
			dragSelectionStartPoint = e.Location;
		}
		public virtual void DoProcessCardSelectionOnMouseDrag(MouseEventArgs e) {
			Point currentPoint = e.Location;
			switch(dragSelectionState) {
				case MouseDragSelectionState.SelectionInProgress: DoSelection(currentPoint); break;
				case MouseDragSelectionState.TryBeginSelection:
					if(CanStartDragSelection(currentPoint)) {
						bool isControlPressed = (Control.ModifierKeys & Keys.Control) != 0;
						StartSelection(currentPoint, !isControlPressed);
					}
					break;
			}
		}
		public virtual void DoEndSelectCardsOnMouseDrag(MouseEventArgs e) {
			if(dragSelectionState == MouseDragSelectionState.SelectionInProgress) {
				View.EndSelection();
			}
			dragSelectionState = MouseDragSelectionState.Nothing;
			View.CardSelectionRect = Rectangle.Empty;
		}
		protected bool CanStartDragSelection(Point pt) {
			return Math.Max(Math.Abs(dragSelectionStartPoint.X - pt.X), Math.Abs(dragSelectionStartPoint.Y - pt.Y)) >= startDragSelectionDistance;
		}
		int[] selectedRowsOnStart = null;
		protected virtual void StartSelection(Point p, bool clearSelection) {
			dragSelectionState = MouseDragSelectionState.SelectionInProgress;
			View.BeginSelection();
			if(clearSelection) {
				selectedRowsOnStart = null;
				View.ClearSelectionCore();
				View.Invalidate();
			}
			else {
				selectedRowsOnStart = View.GetSelectedRows();
			}
		}
		protected virtual void DoSelection(Point p) {
			if(View.ViewInfo == null) return;
			Size dragRectSize = new Size(p.X - dragSelectionStartPoint.X, p.Y - dragSelectionStartPoint.Y);
			int left = dragRectSize.Width > 0 ? dragSelectionStartPoint.X : dragSelectionStartPoint.X + dragRectSize.Width;
			int top = dragRectSize.Height > 0 ? dragSelectionStartPoint.Y : dragSelectionStartPoint.Y + dragRectSize.Height;
			int right = dragRectSize.Width > 0 ? p.X : dragSelectionStartPoint.X;
			int bottom = dragRectSize.Height > 0 ? p.Y : dragSelectionStartPoint.Y;
			Rectangle boundRect = View.ViewInfo.ViewRects.CardsRect;
			boundRect.Inflate(-1, -1);
			left = Math.Max(boundRect.Left, left);
			top = Math.Max(boundRect.Top, top);
			right = Math.Min(boundRect.Right, right);
			bottom = Math.Min(boundRect.Bottom, bottom);
			View.CardSelectionRect = new Rectangle(left, top, right - left, bottom - top);
			List<int> list = null;
			if(selectedRowsOnStart != null) list = new List<int>(selectedRowsOnStart);
			foreach(LayoutViewCard card in View.ViewInfo.VisibleCards) {
				Rectangle cardRect = card.Bounds;
				cardRect.Inflate(-cardRect.Width / 10, -cardRect.Height / 10);
				bool needSelect = View.CardSelectionRect.IntersectsWith(cardRect);
				if(list != null && list.Contains(card.RowHandle)) {
					SelectCard(card, !needSelect);
					continue;
				}
				SelectCard(card, needSelect);
			}
		}
		private void SelectCard(LayoutViewCard card, bool select) {
			int rowHandle = card.RowHandle;
			if(select) {
				if(!View.IsRowSelected(rowHandle)) {
					View.SelectRow(rowHandle);
					View.InvalidateCard(card);
				}
			}
			else {
				if(View.IsRowSelected(rowHandle)) {
					View.UnselectRow(card.RowHandle);
					View.InvalidateCard(card);
				}
			}
		}
	}
}

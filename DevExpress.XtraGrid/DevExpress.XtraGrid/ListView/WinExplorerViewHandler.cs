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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
using System.Diagnostics;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections.Generic;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing.Animation;
namespace DevExpress.XtraGrid.Views.WinExplorer.Handler {
	public class WinExplorerViewHandler : BaseViewHandler {
		public WinExplorerViewHandler(BaseView view) : base(view) {
		}
		public bool IsInMarqueeSelection { get { return SelectionCalculator.IsInMarqueeSelection; } }
		protected WinExplorerView WinExplorerView { get { return (WinExplorerView)View; } }
		protected override void OnResize(Rectangle clientRect) {
			WinExplorerView.ScrollInfo.OnAction(XtraEditors.ScrollNotifyAction.Resize);
			WinExplorerView.InternalSetViewRectangleCore(clientRect);
		}
		protected WinExplorerViewInfo WinExplorerViewInfo { get { return (WinExplorerViewInfo)ViewInfo; } }
		WinExplorerViewMarqueeSelectionCalculator selectionCalculator;
		public WinExplorerViewMarqueeSelectionCalculator SelectionCalculator {
			get {
				if(selectionCalculator == null || !selectionCalculator.SupportItemType(WinExplorerView.OptionsView.Style))
					selectionCalculator = CreateSelectionCalculator();
				return selectionCalculator;
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			Point pt = WinExplorerView.GridControl.PointToClient(Control.MousePosition);
			ProcessMouseMove(new DXMouseEventArgs(Control.MouseButtons, 1, pt.X, pt.Y, 0));
		}
		protected virtual WinExplorerViewMarqueeSelectionCalculator CreateSelectionCalculator() {
			if(WinExplorerView.OptionsView.Style == WinExplorerViewStyle.List)
				return new WinExplorerViewMarqueeSelectionCalculatorList(this);
			return new WinExplorerViewMarqueeSelectionCalculatorIcon(this);
		}
		protected virtual bool ShouldStartDragDrop(Point pt) {
			return Math.Abs(pt.X - WinExplorerViewInfo.PressedInfo.HitPoint.X) > SystemInformation.DragSize.Width ||
				Math.Abs(pt.Y - WinExplorerViewInfo.PressedInfo.HitPoint.Y) > SystemInformation.DragSize.Height;
		}
		internal void OnMouseMoveCore(MouseEventArgs ev) {
			OnMouseMove(ev);
		}
		protected override bool OnMouseMove(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseMove(e);
			if(e.Handled)
				return true;
			return ProcessMouseMove(e);
		}
		private bool ProcessMouseMove(DXMouseEventArgs e) {
			View.OnActionScroll(XtraEditors.ScrollNotifyAction.MouseMove);
			if(WinExplorerViewInfo.PressedInfo.InItem && WinExplorerViewInfo.PressedInfo.HitTest != WinExplorerViewHitTest.ItemCheck && ShouldStartDragDrop(e.Location)) {
				StartDragDrop(e);
				return false;
			}
			if(WinExplorerView.AllowMarqueeSelection && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)) {
				SelectionCalculator.OnMouseMove(e.Location);
			}
			if(SelectionRect.Size.IsEmpty) {
				WinExplorerViewHitInfo prevInfo = WinExplorerViewInfo.HoverInfo;
				WinExplorerViewInfo.HoverInfo = WinExplorerViewInfo.CalcHitInfo(e.Location);
				OnContextButtonsMouseMove(prevInfo, WinExplorerViewInfo.HoverInfo, e);
				if(WinExplorerView.OptionsSelection.ItemSelectionMode == IconItemSelectionMode.Hover) {
					ProcessSelection(WinExplorerViewInfo.HoverInfo, prevInfo, e.Button);
				}
				if(prevInfo != null && prevInfo.ItemInfo != null) {
					WinExplorerView.InvalidateRect(prevInfo.ItemInfo.CheckBoxBounds);
				}
				if(WinExplorerViewInfo.HoverInfo != null && WinExplorerViewInfo.HoverInfo.ItemInfo != null) {
					WinExplorerView.InvalidateRect(WinExplorerViewInfo.HoverInfo.ItemInfo.CheckBoxBounds);
				}
			}
			return false;
		}
		protected virtual void OnContextButtonsMouseMove(WinExplorerViewHitInfo prevInfo, WinExplorerViewHitInfo newInfo, DXMouseEventArgs e) {
			if(WinExplorerViewInfo.ScrollHelper.Animating)
				return;
			WinExplorerItemViewInfo prevItem = prevInfo == null || prevInfo.ItemInfo == null? null: prevInfo.ItemInfo;
			WinExplorerItemViewInfo newItem = newInfo == null || newInfo.ItemInfo == null? null: newInfo.ItemInfo;
			if(prevItem != null && prevItem == newItem) { 
				newItem.ContextButtonsHandler.OnMouseMove(e);
				return;
			}
			if(prevItem != null)
				prevItem.ContextButtonsHandler.OnMouseLeave(e);
			if(newItem != null) {
				newItem.ContextButtonsHandler.OnMouseEnter(e);
				newItem.ContextButtonsHandler.OnMouseMove(e);
			}
		}
		protected virtual void StartDragDrop(MouseEventArgs e) {
			WinExplorerView.RaiseItemDrag(e);
		}
		protected virtual void ProcessSelection(WinExplorerViewHitInfo hitInfo, MouseButtons buttons) {
			ProcessSelection(hitInfo, null, buttons);
		}
		protected virtual void ProcessSelection(WinExplorerViewHitInfo hitInfo, WinExplorerViewHitInfo prevInfo, MouseButtons buttons) {
			try {
				WinExplorerView.FocusedRowChangedByHandler = true;
				if(WinExplorerView.OptionsSelection.ItemSelectionMode == IconItemSelectionMode.None ||
					hitInfo.HitTest == WinExplorerViewHitTest.GroupCaptionButton)
					return;
				if(!hitInfo.InItem && hitInfo.HitTest != WinExplorerViewHitTest.GroupCaption && WinExplorerView.OptionsSelection.ItemSelectionMode != IconItemSelectionMode.Hover) {
					WinExplorerView.DataController.Selection.Clear();
					WinExplorerView.Invalidate();
					WinExplorerViewInfo.FocusedRowHandle = GridControl.InvalidRowHandle;
					return;
				}
				bool shouldAppend = ModifierKeysHelper.IsControlPressed && WinExplorerView.IsMultiSelect;
				bool shouldAddRange = ModifierKeysHelper.IsShiftPressed && WinExplorerView.IsMultiSelect;
				if(hitInfo.InItem) {
					if(shouldAppend) {
						if(WinExplorerView.OptionsSelection.ItemSelectionMode == IconItemSelectionMode.Hover && prevInfo != null && prevInfo.RowHandle == hitInfo.RowHandle)
							return;
						WinExplorerView.DataController.Selection.SetSelected(hitInfo.ItemInfo.Row.RowHandle, !hitInfo.ItemInfo.IsSelected);
						WinExplorerViewInfo.NeedDrawFocusedRect = true;
						SelectionStart = hitInfo.ItemInfo.Row.RowHandle;
						WinExplorerView.Invalidate();
					}
					else if(shouldAddRange) {
						SelectRange(SelectionStart, hitInfo.ItemInfo.Row.RowHandle);
					}
					else {
						bool isSelected = hitInfo.ItemInfo.IsSelected;
						if(isSelected && (WinExplorerView.OptionsSelection.ItemSelectionMode == IconItemSelectionMode.Hover || buttons == MouseButtons.Right || WinExplorerView.DataController.Selection.Count == 1))
							return;
						WinExplorerView.DataController.Selection.Clear();
						WinExplorerView.DataController.Selection.SetSelected(hitInfo.ItemInfo.Row.RowHandle, true);
						WinExplorerViewInfo.NeedDrawFocusedRect = true;
						SelectionStart = hitInfo.ItemInfo.Row.RowHandle;
						WinExplorerView.Invalidate();
					}
				}
			}
			finally {
				WinExplorerView.FocusedRowChangedByHandler = false;
			}
		}
		protected virtual void SelectRange(int startHandle, int endHandle) {
			int startVisibleIndex = WinExplorerView.GetVisibleIndex(startHandle);
			int endVisibleIndex = WinExplorerView.GetVisibleIndex(endHandle);
			if(startVisibleIndex > endVisibleIndex) {
				int temp = startVisibleIndex; startVisibleIndex = endVisibleIndex; endVisibleIndex = temp;
			}
			WinExplorerView.DataController.Selection.BeginSelection();
			try {
				WinExplorerView.DataController.Selection.Clear();
				for(int i = startVisibleIndex; i <= endVisibleIndex; i++) {
					int rowHandle = WinExplorerView.GetVisibleRowHandle(i);
					if(WinExplorerView.IsGroupRow(rowHandle))
						continue;
					WinExplorerView.DataController.Selection.SetSelected(rowHandle, true);
				}
				WinExplorerViewInfo.NeedDrawFocusedRect = true;
			}
			finally {
				WinExplorerView.DataController.Selection.EndSelection();
				WinExplorerView.Invalidate();
			}
		}
		protected int SelectionStart {
			get;
			set;
		}
		protected override bool OnMouseDown(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseDown(e);
			if(e.Handled)
				return true;
			WinExplorerView.FocusedRowChangedByHandler = true;
			try {
				WinExplorerViewHitInfo hitInfo = WinExplorerViewInfo.CalcHitInfo(e.Location);;
				if(ProcessContextButtonsMouseDown(e, hitInfo))
					return false;
				WinExplorerViewInfo.AllowMakeItemVisible = WinExplorerView.OptionsBehavior.AutoScrollItemOnMouseClick;
				WinExplorerViewInfo.PressedInfo = hitInfo;
				WinExplorerViewInfo.AllowMakeItemVisible = true;
				WinExplorerView.CloseEditor();
				if(WinExplorerView.OptionsSelection.ItemSelectionMode == IconItemSelectionMode.Press) {
					ProcessSelection(WinExplorerViewInfo.PressedInfo, ev.Button);
				}
				if(WinExplorerView.AllowMarqueeSelection && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)) {
					SelectionCalculator.OnMouseDown(e);
				}
			}
			finally {
				WinExplorerView.FocusedRowChangedByHandler = false;
			}
			return false;
		}
		protected virtual bool ProcessContextButtonsMouseDown(DXMouseEventArgs e, WinExplorerViewHitInfo hitInfo) {
			if(!hitInfo.InItem)
				return false;
			return hitInfo.ItemInfo.ContextButtonsHandler.OnMouseDown(e);
		}
		protected Point SelectionStartPoint { get { return WinExplorerViewInfo.SelectionStartPoint; } set { WinExplorerViewInfo.SelectionStartPoint = value; } }
		protected Point SelectionEndPoint { get { return WinExplorerViewInfo.SelectionEndPoint; } set { WinExplorerViewInfo.SelectionEndPoint = value; } }
		protected Rectangle SelectionRect { get { return WinExplorerViewInfo.SelectionRect; } }
		protected bool DoubleClickFired { get; set; }
		protected override bool OnMouseUp(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseUp(e);
			if(e.Handled)
				return true;
			WinExplorerViewHitInfo info = WinExplorerViewInfo.CalcHitInfo(e.Location);
			if(info.InItem && !DoubleClickFired) {
				WinExplorerView.RaiseItemClick(new WinExplorerViewItemClickEventArgs(info.ItemInfo, ev));
			}
			if(ProcessContextButtonsMouseUp(ev, info))
				return true;
			if(WinExplorerViewInfo.PressedInfo.Equals(info)) {
				OnClick(info);
				if(WinExplorerView.OptionsSelection.ItemSelectionMode == IconItemSelectionMode.None) {
					SelectionCalculator.ClearSelection();
				}
				if(WinExplorerView.OptionsSelection.ItemSelectionMode == IconItemSelectionMode.Click) {
					ProcessSelection(info, ev.Button);
				}
			}
			else {
				if(WinExplorerView.OptionsSelection.ItemSelectionMode != IconItemSelectionMode.Press)
					SelectionCalculator.OnMouseUp(e);
			}
			DoubleClickFired = false;
			WinExplorerViewInfo.PressedInfo = new WinExplorerViewHitInfo();
			WinExplorerView.Invalidate();
			return false;
		}
		protected virtual bool ProcessContextButtonsMouseUp(MouseEventArgs ev, WinExplorerViewHitInfo hitInfo) {
			if(!hitInfo.InItem)
				return false;
			return hitInfo.ItemInfo.ContextButtonsHandler.OnMouseUp(ev);
		}
		protected override void OnClick(MouseEventArgs e) {
			base.OnClick(e);
		}
		WinExplorerViewHitInfo ClickInfo { get; set; }
		protected virtual void OnClick(WinExplorerViewHitInfo info) {
			if(info.HitTest == WinExplorerViewHitTest.GroupCaptionButton) {
				OnGroupCaptionButtonClick(info);
			}
			else if(info.HitTest == WinExplorerViewHitTest.GroupCaptionCheckBox) {
				OnGroupCheckBoxClick(info);
			}
			else if(info.HitTest == WinExplorerViewHitTest.GroupCaption) {
				OnGroupCaptionClick(info);
			}
			else if(info.HitTest == WinExplorerViewHitTest.ItemCheck) {
				WinExplorerView.ToggleItemCheck(info.ItemInfo);
			}
			if(info.InItem)
				WinExplorerViewInfo.ActiveEditorInfo = info;
			else
				WinExplorerViewInfo.ActiveEditorInfo = null;
		}
		protected virtual void OnGroupCheckBoxClick(WinExplorerViewHitInfo info) {
			info.GroupInfo.Clicked = true;
			WinExplorerView.ToggleGroupChecked(info.GroupInfo.Row.RowHandle);
		}
		protected virtual void OnGroupCaptionClick(WinExplorerViewHitInfo info) {
			ClickInfo = info;
			DoubleClickTimer.Start();
		}
		protected virtual void OnGroupCaptionButtonClick(WinExplorerViewHitInfo info) {
			WinExplorerView.ToggleGroupExpanded(info.GroupInfo.Row.RowHandle);
		}
		Timer doubleClickTimer;
		protected Timer DoubleClickTimer {
			get {
				if(doubleClickTimer == null)
					doubleClickTimer = CreateDoubleClickTimer();
				return doubleClickTimer;
			}
		}
		protected Timer CreateDoubleClickTimer() {
			Timer t = new Timer();
			t.Interval = SystemInformation.DoubleClickTime;
			t.Tick += OnDoubleClickTimerTick;
			return t;
		}
		void OnDoubleClickTimerTick(object sender, EventArgs e) {
			DoubleClickTimer.Stop();
			if(GroupCaptionDoubleClickFired) {
				GroupCaptionDoubleClickFired = false;
				return;
			}
			WinExplorerView.SelectGroupRowChildren(ClickInfo.GroupInfo.Row.RowHandle);
		}
		bool GroupCaptionDoubleClickFired {
			get;
			set;
		}
		protected override void OnDoubleClick(MouseEventArgs ev) {
			DoubleClickTimer.Stop();
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnDoubleClick(e);
			if(e.Handled)
				return;
			WinExplorerViewHitInfo hitInfo = WinExplorerViewInfo.CalcHitInfo(e.Location);
			WinExplorerViewInfo.ActiveEditorInfo = hitInfo;
			if(hitInfo.HitTest == WinExplorerViewHitTest.GroupCaption) {
				if(WinExplorerView.OptionsView.Style != WinExplorerViewStyle.List) {
					WinExplorerView.ToggleGroupExpanded(hitInfo.ItemInfo.Row.RowHandle);
					GroupCaptionDoubleClickFired = true;
				}
			}
			else if ((WinExplorerViewInfo.ActiveEditorInfo.HitTest == WinExplorerViewHitTest.ItemText || WinExplorerViewInfo.ActiveEditorInfo.HitTest == WinExplorerViewHitTest.ItemDescription) && WinExplorerView.OptionsBehavior.Editable)
			{
				WinExplorerViewColumnType itemEdit = WinExplorerViewInfo.ActiveEditorInfo.HitTest == WinExplorerViewHitTest.ItemText ? WinExplorerViewColumnType.Text : WinExplorerViewColumnType.Description;
				WinExplorerView.ShowEditor(itemEdit, true);
				if(WinExplorerView.ActiveEditor == null || !WinExplorerView.ActiveEditor.Visible)
					WinExplorerViewInfo.ActiveEditorInfo = null;
				e.Handled = true;
				return;
			}
			if(WinExplorerViewInfo.ActiveEditorInfo.HitTest == WinExplorerViewHitTest.ItemCheck) {
				WinExplorerView.ToggleItemCheck(WinExplorerViewInfo.ActiveEditorInfo.ItemInfo);
			}
			else if(hitInfo.InItem) {
				WinExplorerView.RaiseItemDoubleClick(new WinExplorerViewItemDoubleClickEventArgs(hitInfo.ItemInfo, ev));
				DoubleClickFired = true;
			}
			WinExplorerViewInfo.ActiveEditorInfo = null;
		}
		protected override bool OnMouseWheel(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseWheel(e);
			if(e.Handled)
				return true;
			if(WinExplorerView.IsEditing)
				return false;
			if(WinExplorerViewInfo.ShouldShowScrollBar()) {
				ScrollOnMouseWheel(WinExplorerViewInfo.CalcPositionDelta(e.Delta));
			}
			return false;
		}
		protected float AnimatedScrollTime { get { return 0.3f; } }
		protected float GetAnimatedScrollTime(int delta) {
			delta = Math.Abs(delta);
			if(delta < 500) return delta * 1.0f / 500;
			if(delta > 3000) return 2.0f;
			return (delta - 500) * 2.0f / (3000 - 500);
		}
		protected virtual void ScrollOnMouseWheel(int delta) {
			if(WinExplorerViewInfo.AllowSmoothScrollOnMouseWheel) {
				WinExplorerViewInfo.SuppressDrawContextButtons = true;
				WinExplorerViewInfo.SetContextButtonsEnabled(false);
				ViewInfo.GridControl.Invalidate();
				ViewInfo.GridControl.Update();
				WinExplorerViewInfo.ScrollHelper.Scroll(WinExplorerView.Position, WinExplorerView.Position + delta, GetAnimatedScrollTime(delta), true);
			}
			else
				WinExplorerView.Position += delta;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled)
				return;
			WinExplorerItemViewInfo itemInfo;
			foreach(WinExplorerItemViewInfo item in WinExplorerViewInfo.VisibleItems) {
				if(item.IsFocused && item.IsSelected) {
					itemInfo = item;
					WinExplorerView.RaiseItemKeyDown(new WinExplorerViewItemKeyEventArgs(itemInfo, e));
					break;
				}
			}
			if(WinExplorerView.CheckShowFindPanelKey(e))
				return;
			if(Navigator.IsNavigationKey(e)) {
				Navigator.ProcessKey(e);
			}
		}
		public override bool NeedKey(NeedKeyType keyType) {
			switch(keyType) {
				case NeedKeyType.Tab: return WinExplorerView.OptionsNavigation.UseTabKey;
				case NeedKeyType.Escape: return View.IsEditing;
				case NeedKeyType.Enter: return View.IsEditing;
				case NeedKeyType.Dialog: return View.IsEditing;
			}
			return false;
		}
		public override bool ProcessKey(KeyEventArgs keys) {
			base.ProcessKey(keys);
			if (View.ActiveEditor != null) {
				DevExpress.XtraEditors.BaseEdit be = View.ActiveEditor as DevExpress.XtraEditors.BaseEdit;
				if (be.IsNeededKey(keys)) return false;
			}
			if(keys.Handled)
				return true;
			if(Navigator.IsNavigationKey(keys)) {
				Navigator.ProcessKey(keys);
				return true;
			}
			return false;
		}
		WinExplorerViewNavigatorBase navigator;
		protected internal virtual WinExplorerViewNavigatorBase Navigator {
			get {
				if(navigator == null || !navigator.SupportItemType(WinExplorerView.OptionsView.Style))
					navigator = CreateNavigator();
				return navigator;
			}
		}
		protected virtual WinExplorerViewNavigatorBase CreateNavigator() {
			if(WinExplorerView.OptionsView.Style == WinExplorerViewStyle.List)
				return new WinExplorerViewNavigatorList(this);
			return new WinExplorerViewNavigator(this);
		}
	}
	public abstract class WinExplorerViewNavigatorBase {
		public WinExplorerViewNavigatorBase(WinExplorerViewHandler handler) {
			Handler = handler;
			PreferredColumnIndex = -1;
			StartFocusedRowHandle = GridControl.InvalidRowHandle;
		}
		public virtual bool SupportItemType(WinExplorerViewStyle itemType) { return false; }
		protected WinExplorerViewHandler Handler { get; private set; }
		protected WinExplorerViewInfo ViewInfo { get { return (WinExplorerViewInfo)Handler.ViewInfo; } }
		protected WinExplorerView View { get { return ViewInfo.WinExplorerView; } }
		protected virtual WinExplorerItemViewInfo GetItemByVisibleIndex(int visibleIndex) {
			foreach(WinExplorerItemViewInfo itemInfo in ViewInfo.VisibleItems) {
				if(itemInfo.Row.VisibleIndex == visibleIndex)
					return itemInfo;
			}
			return null;
		}
		protected internal virtual void FocusRow(int rowHandle) {
			if(!ModifierKeysHelper.IsControlPressed && (!ModifierKeysHelper.IsShiftPressed || (!View.OptionsSelection.MultiSelect && ModifierKeysHelper.IsShiftPressed))) {
				Handler.SelectionCalculator.ClearSelection();
			}
			if(View.OptionsSelection.MultiSelect && ModifierKeysHelper.IsShiftPressed && View.IsValidRowHandle(StartFocusedRowHandle)) {
				View.DataController.Selection.BeginSelection();
				try {
					View.DataController.Selection.Clear();
					int minRowHandle = Math.Min(rowHandle, StartFocusedRowHandle);
					int maxRowHandle = Math.Max(rowHandle, StartFocusedRowHandle);
					for(int i = minRowHandle; i <= maxRowHandle; i++) {
						View.DataController.Selection.SetSelected(i, true);
					}
				}
				finally {
					View.DataController.Selection.EndSelection();
				}
			}
			else if(!ModifierKeysHelper.IsControlPressed)
				View.DataController.Selection.SetSelected(rowHandle, true);
			if(View.IsGroupRow(rowHandle)) {
				View.SelectGroupRowChildren(rowHandle);
			}
			ViewInfo.AllowFadeOutAnimationWhenFocusChanged = false;
			View.FocusedRowChangedByHandler = true;
			try {
				ViewInfo.FocusedRowHandle = rowHandle;
			}
			finally {
				View.FocusedRowChangedByHandler = false;
			}
			ViewInfo.AllowFadeOutAnimationWhenFocusChanged = true;
		}
		public abstract void MoveHorizontalCore(int delta);
		protected virtual void MoveTo(int rowHandle) {
			bool prev = ViewInfo.AllowMakeItemVisible;
			try {
				ViewInfo.AllowMakeItemVisible = true;
				FocusRow(rowHandle);
			}
			finally {
				ViewInfo.AllowMakeItemVisible = prev;
			}
		}
		public virtual void MoveFirst() {
			MoveTo(View.GetVisibleRowHandle(0));
		}
		public virtual void MoveLast() {
			MoveTo(View.GetVisibleRowHandle(View.RowCount - 1));
		}
		public virtual void MoveLastVisible() {
			MoveLast();
		}
		public virtual void MoveNext() {
			MoveHorizontal(1);
		}
		public virtual void MovePrev() {
			MoveHorizontal(-1);
		}
		public virtual void MoveNextPage() {
			MovePageVertical(+1);
		}
		public virtual void MovePrevPage() {
			MovePageVertical(-1);
		}
		public virtual void DoMoveFocusedRow(int delta) {
			MoveHorizontal(delta);
		}
		public virtual void MoveHorizontal(int delta) {
		if(ViewInfo.FocusedRowHandle == GridControl.InvalidRowHandle) {
				FocusRow(View.GetVisibleRowHandle(0));
				return;
			}
			ClearPreferredColumnIndex();
			MoveHorizontalCore(delta);
		}
		public void ClearPreferredColumnIndex() {
			PreferredColumnIndex = -1;
		}
		protected virtual void MovePageUp() {
			WinExplorerItemViewInfo itemInfo = GetItemInFirstVisibleRow();
			if(itemInfo.Row.RowHandle != ViewInfo.FocusedRowHandle) {
				FocusRow(itemInfo.Row.RowHandle);
				View.Invalidate();
				return;
			}
			MovePageUpCore();
		}
		protected abstract void MovePageUpCore();
		protected virtual int GetFirstVisibleItem() {
			for(int i = 0; i < ViewInfo.VisibleItems.Count; i++) {
				if(ViewInfo.VisibleItems[i].IsFullyVisible)
					return i;
			}
			return 0;
		}
		protected virtual WinExplorerItemViewInfo GetItemInFirstVisibleRow() {
			int itemIndex = GetFirstVisibleItem();
			itemIndex = FindItemInColumn(itemIndex, PreferredColumnIndex);
			return ViewInfo.VisibleItems[itemIndex];
		}
		protected virtual void MovePageDown() {
			WinExplorerItemViewInfo itemInfo = GetItemInLastVisibleRow();
			if(itemInfo.Row.RowHandle != ViewInfo.FocusedRowHandle) {
				FocusRow(itemInfo.Row.RowHandle);
				return;
			}
			MovePageDownCore();
		}
		protected abstract void MovePageDownCore();
		public virtual void MovePageVertical(int direction) {
			if(ViewInfo.ScrollHelper.Animating)
				return;
			if(ViewInfo.FocusedRowHandle == GridControl.InvalidRowHandle) {
				FocusRow(View.GetVisibleRowHandle(0));
				ClearPreferredColumnIndex();
				return;
			}
			int visibleIndex = View.GetVisibleIndex(ViewInfo.FocusedRowHandle);
			if(PreferredColumnIndex == -1)
				PreferredColumnIndex = GetColumnIndex(visibleIndex);
			if(direction > 0)
				MovePageDown();
			else
				MovePageUp();
		}
		protected internal void FocusFirstVisibleRowItem() {
			WinExplorerItemViewInfo itemInfo = GetItemInFirstVisibleRow();
			FocusRow(itemInfo.Row.RowHandle);
		}
		protected internal void FocusLastVisibleRowItem() {
			WinExplorerItemViewInfo itemInfo = GetItemInLastVisibleRow();
			FocusRow(itemInfo.Row.RowHandle);
		}
		int GetLastVisibleItem() {
			for(int i = ViewInfo.VisibleItems.Count - 1; i >= 0; i--) {
				if(ViewInfo.VisibleItems[i].IsFullyVisible) {
					return i;
				}
			}
			return 0;
		}
		protected virtual WinExplorerItemViewInfo GetItemInLastVisibleRow() {
			int itemIndex = GetLastVisibleItem();
			itemIndex = FindFirstItemInRow(itemIndex);
			itemIndex = FindItemInColumn(itemIndex, PreferredColumnIndex);
			return ViewInfo.VisibleItems[itemIndex];
		}
		protected virtual int FindItemInColumn(int itemIndex, int columnIndex) {
			int start = Math.Min(itemIndex + columnIndex, ViewInfo.VisibleItems.Count - 1);
			for(int i = start; i >= itemIndex; i--) {
				if(ViewInfo.VisibleItems[i].Bounds.Y == ViewInfo.VisibleItems[itemIndex].Bounds.Y)
					return i;
			}
			return itemIndex;
		}
		int FindFirstItemInRow(int itemIndex) {
			int start = Math.Max(0, itemIndex - ViewInfo.AvailableColumnCount);
			for(int i = start; i <= itemIndex; i++) {
				if(ViewInfo.VisibleItems[i].Bounds.Y == ViewInfo.VisibleItems[itemIndex].Bounds.Y)
					return i;
			}
			return itemIndex;
		}
		public virtual void MoveVertical(int delta) {
			if(ViewInfo.FocusedRowHandle == GridControl.InvalidRowHandle) {
				FocusRow(View.GetVisibleRowHandle(0));
				ClearPreferredColumnIndex();
				return;
			}
			MoveVerticalCore(delta);
		}
		protected abstract void MoveVerticalCore(int delta);
		protected internal int PreferredColumnIndex { get; set; }
		protected int GetParentGroupRowHandle(int rowHandle) {
			if(View.IsGroupRow(rowHandle))
				return rowHandle;
			return View.DataController.GetParentRowHandle(rowHandle);
		}
		protected int GetFirstItemVisibleIndex(int groupRowHandle) {
			return View.GetVisibleIndex(groupRowHandle) + 1;
		}
		protected int GetItemsCount(int groupRowHandle) {
			return View.DataController.GroupInfo.GetChildCount(groupRowHandle);
		}
		protected int GetColumnIndex(int visibleIndex) {
			if(!ViewInfo.HasGroups)
				return visibleIndex % ViewInfo.AvailableColumnCount;
			int rowHandle = View.GetVisibleRowHandle(visibleIndex);
			int groupRowHandle = GetParentGroupRowHandle(rowHandle);
			int firstChildVisibleIndex = GetFirstItemVisibleIndex(groupRowHandle);
			return Math.Max(0, (visibleIndex - firstChildVisibleIndex) % ViewInfo.AvailableColumnCount);
		}
		protected virtual int MoveRow(int visibleIndex, int sign) {
			int rowHandle = View.GetVisibleRowHandle(visibleIndex);
			int newVisibleIndex = 0;
			if(View.IsGroupRow(rowHandle)) {
				if(sign > 0) {
					int lastItemIndex = visibleIndex + GetItemsCount(rowHandle);
					newVisibleIndex = Math.Min(visibleIndex + 1 + PreferredColumnIndex, lastItemIndex);
				}
				else {
					if(rowHandle == -1)
						return visibleIndex;
					int itemsCount = GetItemsCount(rowHandle + 1);
					if(itemsCount == 0)
						return visibleIndex - 1;
					int firstChildVisibleIndex = GetFirstItemVisibleIndex(rowHandle + 1);
					int rowCount = itemsCount  / ViewInfo.AvailableColumnCount;
					newVisibleIndex = firstChildVisibleIndex + Math.Min(rowCount * ViewInfo.AvailableColumnCount + PreferredColumnIndex, itemsCount - 1);
				}
			}
			else {
				int groupRowHandle = GetParentGroupRowHandle(rowHandle);
				int firstChildVisibleIndex = 0; 
				int itemsCount = View.DataRowCount;
				if(groupRowHandle != GridControl.InvalidRowHandle) {
					firstChildVisibleIndex = GetFirstItemVisibleIndex(groupRowHandle);
					itemsCount = GetItemsCount(groupRowHandle);
				}
				if(sign > 0) {
					int delta = Math.Min(itemsCount - 1 - (visibleIndex - firstChildVisibleIndex), ViewInfo.AvailableColumnCount);
					newVisibleIndex = visibleIndex + delta;
					if((newVisibleIndex - firstChildVisibleIndex) / ViewInfo.AvailableColumnCount == (visibleIndex - firstChildVisibleIndex) / ViewInfo.AvailableColumnCount)
						newVisibleIndex++;
					newVisibleIndex = firstChildVisibleIndex + Math.Min(newVisibleIndex - firstChildVisibleIndex, itemsCount - 1);
					if(newVisibleIndex == visibleIndex && groupRowHandle != GridControl.InvalidRowHandle) {
						if(View.IsValidRowHandle(groupRowHandle - 1))
							newVisibleIndex++;
					}
				}
				else {
					if(visibleIndex - firstChildVisibleIndex > ViewInfo.AvailableColumnCount - 1) {
						newVisibleIndex = visibleIndex - ViewInfo.AvailableColumnCount;
						int firstChildInRowGroup = newVisibleIndex - firstChildVisibleIndex;
						firstChildInRowGroup = firstChildInRowGroup - (firstChildInRowGroup % ViewInfo.AvailableColumnCount);
						newVisibleIndex = firstChildInRowGroup + PreferredColumnIndex;
						newVisibleIndex = Math.Min(newVisibleIndex, itemsCount - 1);
						newVisibleIndex += firstChildVisibleIndex;
					}
					else
						newVisibleIndex = View.GetVisibleIndex(groupRowHandle);
				}
			}
			if(View.GetVisibleRowHandle(newVisibleIndex) == GridControl.InvalidRowHandle)
				newVisibleIndex = visibleIndex;
			return newVisibleIndex;
		}
		protected internal virtual bool IsNavigationKey(KeyEventArgs e) {
			if(e.KeyCode == Keys.Left || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.Home || e.KeyCode == Keys.End || e.KeyCode == Keys.Space)
				return !View.IsEditing;
			return e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab;
		}
		protected int StartFocusedRowHandle { get; set; }
		protected virtual void UpdateStartFocusedRowHandle() {
			if(!ModifierKeysHelper.IsShiftPressed) {
				StartFocusedRowHandle = GridControl.InvalidRowHandle;
			}
			else if(!View.IsValidRowHandle(StartFocusedRowHandle)) {
				StartFocusedRowHandle = View.FocusedRowHandle;
			}
		}
		protected internal virtual void ProcessKey(KeyEventArgs e) {
			UpdateStartFocusedRowHandle();
			switch(e.KeyCode) {
				case Keys.Home:
					OnHome();
					break;
				case Keys.End:
					OnEnd();
					break;
				case Keys.Escape:
					OnEscape();
					break;
				case Keys.Enter:
					OnEnter();
					break;
				case Keys.Tab:
					OnTab();
					break;
				case Keys.Left:
					MoveHorizontal(-1);
					break;
				case Keys.Right:
					MoveHorizontal(1);
					break;
				case Keys.Up:
					MoveVertical(-1);
					break;
				case Keys.Down:
					MoveVertical(1);
					break;
				case Keys.PageDown:
					MovePageVertical(1);
					break;
				case Keys.PageUp:
					MovePageVertical(-1);
					break;
				case Keys.Space:
					OnSpace();
					break;
			}
		}
		protected virtual void OnSpace() {
			if(!View.OptionsSelection.MultiSelect) {
				View.DataController.Selection.Clear();
			}
			if(ModifierKeysHelper.IsControlPressed)
				View.DataController.Selection.SetSelected(View.FocusedRowHandle, !View.DataController.Selection.GetSelected(View.FocusedRowHandle));
		}
		protected virtual void OnEnd() {
			MoveLast();
		}
		protected virtual void OnHome() {
			MoveFirst();
		}
		protected bool CanNavigateToEditorBackward() {
			return View.GroupCount == 0 && View.FocusedRowHandle == 0 && ModifierKeysHelper.IsShiftPressed;
		}
		protected bool AreTextAndDescriptionColumnsSet() {
			return View.ColumnSet.DescriptionColumn != null && View.ColumnSet.TextColumn != null;
		}
		protected virtual WinExplorerViewColumnType GetNextEditColumnType() {
			if (CanNavigateToEditorBackward() || !View.IsValidRowHandle(View.FocusedRowHandle) || View.ColumnSet.DescriptionColumn == null)
				return WinExplorerViewColumnType.Text;
			if(View.ColumnSet.TextColumn == null)
				return WinExplorerViewColumnType.Description;
			if(View.IsGroupRow(View.FocusedRowHandle)) {
				return ModifierKeysHelper.IsShiftPressed ? WinExplorerViewColumnType.Description : WinExplorerViewColumnType.Text;
			}
			else if(View.EditedColumn == null || View.EditedColumn == View.ColumnSet.TextColumn)
				return WinExplorerViewColumnType.Description;
			return WinExplorerViewColumnType.Text;
		}
		protected virtual void OnTab() {
			WinExplorerViewColumnType columnType = GetNextEditColumnType();
			View.CloseEditor();
			if(View.ActiveEditor == null) {
				int delta = (ModifierKeysHelper.IsShiftPressed) ? -1 : +1;
				if (AreTextAndDescriptionColumnsSet() && 
					View.OptionsBehavior.Editable && !View.IsGroupRow(View.FocusedRowHandle) &&
					((delta == 1 && columnType == WinExplorerViewColumnType.Description) || (delta == -1 && columnType == WinExplorerViewColumnType.Text)))
					delta = 0;
				OnTabPressedCore(delta);
				if(View.IsGroupRow(View.FocusedRowHandle) || !View.OptionsBehavior.Editable)
					return;
				ViewInfo.ActiveEditorInfo = new WinExplorerViewHitInfo() { HitTest = WinExplorerViewHitTest.ItemText, ItemInfo = ViewInfo.GetItemByRow(ViewInfo.FocusedRowHandle) };
			}
			View.ShowEditor(columnType);
		}
		protected virtual void OnTabPressedCore(int delta) {
			MoveHorizontal(delta);
		}
		protected virtual void OnEnter() {
			View.CloseEditor();
		}
		protected virtual void OnEscape() {
			View.HideEditor();
		}
		public virtual void MoveBy(int delta) {
			MoveHorizontal(delta);
		}
		protected internal virtual bool GetAllowMoveItem(int delta) {
			if(ViewInfo.FocusedRowHandle == GridControl.InvalidRowHandle)
				return true;
			int visibleIndex = View.GetVisibleIndex(ViewInfo.FocusedRowHandle);
			int newVisibleIndex = Math.Max(0, Math.Min(visibleIndex + delta, View.RowCount - 1));
			return newVisibleIndex != visibleIndex;
		}
		protected internal virtual bool GetAllowMoveRow(int delta) {
			if(ViewInfo.FocusedRowHandle == GridControl.InvalidRowHandle)
				return true;
			if(PreferredColumnIndex == -1)
				PreferredColumnIndex = GetColumnIndex(View.GetVisibleIndex(ViewInfo.FocusedRowHandle));
			int prevVisibleIndex = View.GetVisibleIndex(ViewInfo.FocusedRowHandle);
			int visibleIndex = MoveRow(prevVisibleIndex, delta);
			return visibleIndex != prevVisibleIndex;
		}
		public virtual bool GetAllowMoveNextPage() {
			return GetAllowMoveRow(1);
		}
		public virtual bool GetAllowMovePrevPage() {
			return GetAllowMoveRow(-1);
		}
	}
	public class WinExplorerViewNavigator : WinExplorerViewNavigatorBase {
		public WinExplorerViewNavigator(WinExplorerViewHandler handler) : base(handler) { }
		public override bool SupportItemType(WinExplorerViewStyle itemType) {
			return itemType != WinExplorerViewStyle.List;
		}
		protected override void MovePageUpCore() {
			WinExplorerItemViewInfo itemInfo = GetItemInFirstVisibleRow();
			int newPosition = View.ConstrainPosition(View.Position - (ViewInfo.ClientBounds.Height - itemInfo.Bounds.Bottom));
			if(View.OptionsBehavior.EnableSmoothScrolling) {
				ViewInfo.CalcIconDrawInfoCore(newPosition);
				itemInfo = GetItemInFirstVisibleRow();
				ViewInfo.InternalFocusedRowHandle = itemInfo.Row.RowHandle;
				ViewInfo.ShouldFocusFirstRowItemAfterScroll = true;
				ViewInfo.ScrollHelper.Scroll(View.Position, newPosition, WinExplorerViewInfo.ScrollTime, true);
				return;
			}
			View.Position = newPosition;
			FocusFirstVisibleRowItem();
		}
		public override void MoveHorizontalCore(int delta) {
			int visibleIndex = View.GetVisibleIndex(ViewInfo.FocusedRowHandle);
			visibleIndex = Math.Max(0, Math.Min(visibleIndex + delta, View.RowCount - 1));
			FocusRow(View.GetVisibleRowHandle(visibleIndex));
		}
		protected override void MoveVerticalCore(int delta) {
			int visibleIndex = View.GetVisibleIndex(ViewInfo.FocusedRowHandle);
			if(PreferredColumnIndex == -1)
				PreferredColumnIndex = GetColumnIndex(visibleIndex);
			int rowCount = Math.Abs(delta);
			int sign = Math.Sign(delta);
			for(int i = 0; i < rowCount; i++) {
				int newVisibleIndex = MoveRow(visibleIndex, sign);
				if(newVisibleIndex == visibleIndex)
					break;
				visibleIndex = newVisibleIndex;
			}
			FocusRow(View.GetVisibleRowHandle(visibleIndex));
		}
		protected override void MovePageDownCore() {
			WinExplorerItemViewInfo itemInfo = GetItemInLastVisibleRow();
			int newPosition = View.ConstrainPosition(ViewInfo.CalcRowLocation(itemInfo.Row.RowHandle));
			if(View.OptionsBehavior.EnableSmoothScrolling) {
				ViewInfo.CalcIconDrawInfoCore(newPosition);
				itemInfo = GetItemInLastVisibleRow();
				ViewInfo.InternalFocusedRowHandle = itemInfo.Row.RowHandle;
				ViewInfo.ShouldFocusLastRowItemAfterScroll = true;
				ViewInfo.ScrollHelper.Scroll(View.Position, newPosition, WinExplorerViewInfo.ScrollTime, true);
				return;
			}
			View.Position = newPosition;
			FocusLastVisibleRowItem();
		}
	}
	public class WinExplorerViewNavigatorList : WinExplorerViewNavigatorBase {
		public WinExplorerViewNavigatorList(WinExplorerViewHandler handler) : base(handler) { }
		public override bool SupportItemType(WinExplorerViewStyle itemType) {
			return itemType == WinExplorerViewStyle.List;
		}
		protected override void OnTabPressedCore(int delta) {
			MoveVertical(delta);
		}
		public override void DoMoveFocusedRow(int delta) {
			MoveVertical(delta);
		}
		public override void MoveNext() {
			MoveVertical(+1);
		}
		public override void MovePrev() {
			MoveVertical(-1);
		}
		public override void MoveHorizontalCore(int delta) {
			int visibleIndex = View.GetVisibleIndex(ViewInfo.FocusedRowHandle);
			if(PreferredColumnIndex == -1)
				PreferredColumnIndex = GetRowIndex(visibleIndex);
			int column = GetItemColumn(ViewInfo.FocusedRowHandle);
			column += delta;
			column = Math.Max(0, column);
			column = Math.Min(column, ViewInfo.CalcScrollableAreaSize());
			if(!ViewInfo.HasGroups) {
				FocusRow(Math.Min(column * ViewInfo.AvailableRowCountHorizontal + PreferredColumnIndex, View.DataRowCount - 1));
				return;
			}
			int topVisibleGroupIndex = ViewInfo.Calculator.CalcTopVisibleGroupIndex(column);
			int groupRowHandle = -topVisibleGroupIndex - 1;
			int groupColumn = ViewInfo.Calculator.ScrollingCache[topVisibleGroupIndex];
			int firstItemRowHandle = ViewInfo.GetFirstItemInGroup(groupRowHandle);
			int itemsCount = ViewInfo.GetChildCount(groupRowHandle);
			FocusRow(firstItemRowHandle + Math.Min((column - groupColumn) * ViewInfo.AvailableRowCountHorizontal + PreferredColumnIndex, itemsCount - 1));
		}
		protected virtual int GetItemColumn(int rowHandle) {
			if(!ViewInfo.HasGroups) {
				return rowHandle / ViewInfo.AvailableRowCountHorizontal;
			}
			int groupRowHandle = View.IsGroupRow(rowHandle) ? rowHandle : View.GetParentRowHandle(rowHandle);
			int columnIndex = ViewInfo.Calculator.ScrollingCache[-groupRowHandle - 1];
			int firstRowHandle = ViewInfo.GetFirstItemInGroup(groupRowHandle);
			return columnIndex + (rowHandle - firstRowHandle) / ViewInfo.AvailableRowCountHorizontal;
		}
		protected virtual int GetRowIndex(int visibleIndex) {
			int rowHandle = View.GetVisibleRowHandle(visibleIndex);
			if(!ViewInfo.HasGroups)
				return visibleIndex % ViewInfo.AvailableRowCountHorizontal;
			int groupRowHandle = GetParentGroupRowHandle(rowHandle);
			int firstChildVisibleIndex = GetFirstItemVisibleIndex(groupRowHandle);
			return Math.Max(0, (visibleIndex - firstChildVisibleIndex) % ViewInfo.AvailableRowCountHorizontal);
		}
		protected override void MoveVerticalCore(int delta) {
			int visibleIndex = ViewInfo.FocusedRowHandle;
			visibleIndex = Math.Max(0, Math.Min(visibleIndex + delta, View.DataRowCount - 1));
			FocusRow(visibleIndex);
		}
		protected override void MovePageUpCore() {
			WinExplorerItemViewInfo focusedItem = ViewInfo.GetItemByRow(ViewInfo.FocusedRowHandle);
			if(focusedItem == null) {
				FocusRow(0);
				return;
			}
			WinExplorerItemViewInfo itemInfo = GetItemInFirstVisibleRow();
			int attemptCount = 5;
			View.SuppressInvalidate = true;
			ViewInfo.AllowDrawFocus = false;
			try {
				while(attemptCount > 0 && View.Position > 0) {
					View.Position -= 2;
					itemInfo = GetItemInLastVisibleRow();
					if(itemInfo.Column == focusedItem.Column)
						break;
					if(itemInfo.Column < focusedItem.Column) {
						View.Position += 1;
						break;
					}
					attemptCount--;
				}
			}
			finally {
				itemInfo = GetItemInFirstVisibleRow();
				FocusRow(itemInfo.Row.RowHandle);
				ViewInfo.AllowDrawFocus = true;
				View.SuppressInvalidate = false;
				View.Invalidate();
			}
		}
		protected override void MovePageDownCore() {
			WinExplorerItemViewInfo itemInfo = GetItemInLastVisibleRow();
			if(itemInfo != null) {
				ViewInfo.AllowDrawFocus = false;
				try {
					WinExplorerItemViewInfo prevItemInfo = ViewInfo.GetItemByRow(ViewInfo.FocusedRowHandle);
					if(itemInfo.Column <= prevItemInfo.Column) {
						View.Position = prevItemInfo.Column;
						itemInfo = GetItemInLastVisibleRow();
					}
					FocusRow(itemInfo.Row.RowHandle);
				}
				finally {
					ViewInfo.AllowDrawFocus = true;
					View.Invalidate();
				}
			}
		}
		protected override WinExplorerItemViewInfo GetItemInFirstVisibleRow() {
			WinExplorerItemViewInfo prevItemInfo = ViewInfo.GetItemByRow(ViewInfo.FocusedRowHandle);
			foreach(WinExplorerItemViewInfo itemInfo in ViewInfo.VisibleItems) {
				if(itemInfo.Bounds.Y == prevItemInfo.Bounds.Y && itemInfo.IsFullyVisible)
					return itemInfo;
			}
			if(ViewInfo.VisibleItems.Count > 0)
				return ViewInfo.VisibleItems[0];
			return null;
		}
		protected override WinExplorerItemViewInfo GetItemInLastVisibleRow() {
			WinExplorerItemViewInfo prevItemInfo = ViewInfo.GetItemByRow(ViewInfo.FocusedRowHandle);
			if(prevItemInfo == null)
				return null;
			WinExplorerItemViewInfo nextItemInfo = null;
			foreach(WinExplorerItemViewInfo itemInfo in ViewInfo.VisibleItems) {
				if(itemInfo == prevItemInfo)
					continue;
				if(itemInfo.Bounds.Y == prevItemInfo.Bounds.Y && itemInfo.IsFullyVisible)
					nextItemInfo = itemInfo;
			}
			if(nextItemInfo == null && ViewInfo.VisibleItems.Count > 0) 
				nextItemInfo = ViewInfo.VisibleItems[ViewInfo.VisibleItems.Count - 1];
			return nextItemInfo;
		}
	}
	public class WinExplorerViewMarqueeSelectionInfo {
		public Rectangle Bounds { get; set; }
		public int StartGroupRowHandle { get; set; }
		public int EndGroupRowHandle { get; set; }
		public int GroupRowHandle { get; set; }
		public int StartRow { get; set; }
		public int EndRow { get; set; }
		public int StartColumn { get; set; }
		public int EndColumn { get; set; }
		public int FirstRowHandle { get; set; }
		public int ItemsCount { get; set; }
		public bool IsEmpty { get; set; }
		public Point MouseStartPosition { get; set; }
		public Point MouseEndPosition { get; set; }
		public List<WinExplorerItemViewInfo> StartScreenItems { get; set; }
		public List<WinExplorerItemViewInfo> EndScreenItems { get; set; }
		public bool InvertedSelection { get; set; }
	}
	public class WinExplorerViewMarqueeSelectionCalculator {
		public WinExplorerViewMarqueeSelectionCalculator(WinExplorerViewHandler handler) {
			Handler = handler;
		}
		public WinExplorerViewHandler Handler { get; private set; }
		public WinExplorerView View { get { return (WinExplorerView)Handler.View; } }
		public WinExplorerViewInfo ViewInfo { get { return (WinExplorerViewInfo)Handler.ViewInfo; } }
		public GridControl GridControl { get { return View.GridControl; } }
		public virtual bool SupportItemType(WinExplorerViewStyle itemType) {
			return false;
		}
		Timer scrollTimer;
		protected Timer ScrollTimer {
			get {
				if(scrollTimer == null)
					scrollTimer = CreateScrollTimer();
				return scrollTimer;
			}
		}
		protected virtual Timer CreateScrollTimer() {
			Timer res = new Timer();
			res.Interval = ScrollInterval;
			res.Tick += OnScrollTimerTick;
			return res;
		}
		protected virtual bool ShouldScrollPrev(Point point) {
			return false;   
		}
		protected virtual bool ShouldScrollNext(Point point) {
			return false;
		}
		protected virtual int ScrollDelta { get { return 0; } }
		protected virtual int ScrollInterval { get { return 1; } }
		protected virtual void OnScrollTimerTick(object sender, EventArgs e) {
			Point point = GridControl.PointToClient(Control.MousePosition);
			if(ShouldScrollPrev(point)) {
				ScrollPrev();
			}
			else if(ShouldScrollNext(point)) {
				ScrollNext();
			}
			else {
				ScrollTimer.Stop();
			}
			OnMouseMove(GridControl.PointToClient(Control.MousePosition));
		}
		protected virtual void ScrollNext() {
			View.Position += ScrollDelta;
		}
		protected virtual void ScrollPrev() {
			View.Position -= ScrollDelta;
		}
		public virtual void CheckStartScrollTimer(Point pt) {
			if(ShouldScrollPrev(pt) || ShouldScrollNext(pt)) {
				ScrollTimer.Start();
			}
		}
		protected Point SelectionStartPoint { get { return ViewInfo.SelectionStartPoint; } set { ViewInfo.SelectionStartPoint = value; } }
		protected Point SelectionEndPoint { get { return ViewInfo.SelectionEndPoint; } set { ViewInfo.SelectionEndPoint = value; } }
		protected Rectangle SelectionRect { get { return ViewInfo.SelectionRect; } }
		public virtual void OnMouseDown(DXMouseEventArgs e) {
			ScrollTimer.Stop();
			SelectionStartPoint = ViewInfo.PressedInfo.IsMarqueeSelectionZone ? ViewInfo.CalcSelectionPoint(e.Location) : WinExplorerViewInfo.InvalidSelectionPoint;
			SelectionEndPoint = SelectionStartPoint;
			GridControl.MouseCaptureOwner = View;
		}
		public bool IsInMarqueeSelection { get; set; }
		public virtual void OnMouseUp(DXMouseEventArgs e) {
			if(!View.AllowMarqueeSelection)
				return;
			ScrollTimer.Stop();
			GridControl.MouseCaptureOwner = null;
			SelectionEndPoint = ViewInfo.CalcSelectionPoint(e.Location);
			if(SelectionRect.Size.IsEmpty) {
				View.DataController.Selection.Clear();
			}
			SelectionStartPoint = WinExplorerViewInfo.InvalidSelectionPoint;
			View.RaiseMarqueeSelectionCompleted(EventArgs.Empty);
		}
		public virtual void OnMouseMove(Point pt) {
			CheckStartScrollTimer(pt);
			bool notStartedYet = SelectionStartPoint == SelectionEndPoint;
			SelectionEndPoint = ViewInfo.CalcSelectionPoint(pt);
			if(notStartedYet && SelectionStartPoint != SelectionEndPoint)
				View.RaiseMarqueeSelectionStarted(EventArgs.Empty);
			if(SelectionStartPoint != WinExplorerViewInfo.InvalidSelectionPoint) {
				SelectItemsByRect(SelectionRect);
			}
		}
		protected virtual WinExplorerViewMarqueeSelectionInfo GetSelectionInfo() { return new WinExplorerViewMarqueeSelectionInfo(); }
		protected virtual void SelectItemsByRect(Rectangle rect) {
			WinExplorerViewMarqueeSelectionInfo info = GetSelectionInfo();
			info.Bounds = rect;
			View.DataController.Selection.BeginSelection();
			try {
				IsInMarqueeSelection = true;
				View.DataController.Selection.Clear();
				if(View.DataController.GroupInfo.Count > 0)
					SelectItemsByRectInGroups(info);
				else {
					SelectItemsByRectNoGroups(info);
				}
			}
			finally {
				ViewInfo.NeedDrawFocusedRect = true;
				View.DataController.Selection.EndSelection();
				IsInMarqueeSelection = false;
				View.Invalidate();
			}
		}
		protected virtual void SelectItemsByRectNoGroups(WinExplorerViewMarqueeSelectionInfo info) {
			info.GroupRowHandle = GridControl.InvalidRowHandle;
			CalcSelectionItemsInfo(info);
			CalcSelectionColumns(info);
			if(info.IsEmpty)
				return;
			CalcSelectionRows(info);
			SelectItemsByRect(info);
		}
		protected virtual void SelectItemsByRectInGroups(WinExplorerViewMarqueeSelectionInfo info) { }
		protected virtual void CalcSelectionColumns(WinExplorerViewMarqueeSelectionInfo info) { }
		protected virtual void CalcSelectionItemsInfo(WinExplorerViewMarqueeSelectionInfo info) {
			if(info.GroupRowHandle == GridControl.InvalidRowHandle) {
				info.ItemsCount = View.DataRowCount;
				info.FirstRowHandle = 0;
			}
			else {
				GroupRowInfo groupRowInfo = View.DataController.GroupInfo.GetGroupRowInfoByHandle(info.GroupRowHandle);
				info.ItemsCount = View.DataController.GroupInfo.GetChildCount(groupRowInfo);
				if(info.ItemsCount == 0) {
					info.FirstRowHandle = GridControl.InvalidRowHandle;
					return;
				}
				else
					info.FirstRowHandle = View.DataController.GroupInfo.GetChildRow(groupRowInfo, 0);
			}
		}
		protected virtual void CalcSelectionRows(WinExplorerViewMarqueeSelectionInfo info) { }
		protected virtual void SelectItemsByRect(WinExplorerViewMarqueeSelectionInfo info) { }
		public void ClearSelection() {
			View.DataController.Selection.Clear();
		}
	}
	public class WinExplorerViewMarqueeSelectionCalculatorIcon : WinExplorerViewMarqueeSelectionCalculator {
		public WinExplorerViewMarqueeSelectionCalculatorIcon(WinExplorerViewHandler handler) : base(handler) { }
		public override bool SupportItemType(WinExplorerViewStyle itemType) {
			return itemType != WinExplorerViewStyle.List;
		}
		protected override bool ShouldScrollNext(Point point) {
			return point.Y > ViewInfo.ClientBounds.Bottom;
		}
		protected override bool ShouldScrollPrev(Point point) {
			return point.Y < ViewInfo.ClientBounds.Y;
		}
		protected override void SelectItemsByRectInGroups(WinExplorerViewMarqueeSelectionInfo info) {
			CalcSelectionColumns(info);
			if(info.IsEmpty)
				return;
			info.StartGroupRowHandle = -1 - ViewInfo.CalcTopVisibleGroupIndex(info.Bounds.Y);
			info.EndGroupRowHandle = -1 - ViewInfo.CalcTopVisibleGroupIndex(info.Bounds.Bottom);
			for(int i = info.StartGroupRowHandle; i >= info.EndGroupRowHandle; i--) {
				info.GroupRowHandle = i;
				CalcSelectionItemsInfo(info);
				CalcSelectionRows(info);
				if(info.IsEmpty)
					continue;
				SelectItemsByRect(info);
			}
		}
		protected override void CalcSelectionColumns(WinExplorerViewMarqueeSelectionInfo info) {
			info.StartColumn = ((info.Bounds.X - ViewInfo.ContentBounds.X) + ViewInfo.ItemHorizontalIndent) / (ViewInfo.ItemSize.Width + ViewInfo.ItemHorizontalIndent);
			info.StartColumn = Math.Max(info.StartColumn, 0);
			int startColumnX = ViewInfo.CalcColumnLocation(info.StartColumn);
			if(info.Bounds.X < startColumnX && info.Bounds.Right < startColumnX) {
				info.IsEmpty = true;
				return;
			}
			info.EndColumn = ((info.Bounds.Right - ViewInfo.ContentBounds.X) + ViewInfo.ItemHorizontalIndent) / (ViewInfo.ItemSize.Width + ViewInfo.ItemHorizontalIndent);
			info.EndColumn = Math.Min(info.EndColumn, ViewInfo.AvailableColumnCount - 1);
			int endColumnX = ViewInfo.CalcColumnLocation(info.EndColumn);
			if(info.Bounds.Right < endColumnX)
				info.EndColumn--;
			if(info.EndColumn < info.StartColumn) {
				info.IsEmpty = true;
				return;
			}
		}
		protected override void CalcSelectionRows(WinExplorerViewMarqueeSelectionInfo info) {
			if(info.ItemsCount == 0)
				return;
			int firstRowY = 0;
			if(info.GroupRowHandle != GridControl.InvalidRowHandle)
				firstRowY = ViewInfo.CalcGroupLocationByHandle(info.GroupRowHandle) + ViewInfo.GroupCaptionHeight + ViewInfo.GroupItemIndent;
			else
				firstRowY = 0;
			if(info.GroupRowHandle != GridControl.InvalidRowHandle && info.GroupRowHandle < info.StartGroupRowHandle) {
				info.StartRow = 0;
			}
			else {
				info.StartRow = ((info.Bounds.Y - firstRowY) + ViewInfo.ItemVerticalIndent) / (ViewInfo.ItemSize.Height + ViewInfo.ItemVerticalIndent);
				info.StartRow = Math.Max(0, info.StartRow);
			}
			int startRowY = firstRowY + info.StartRow * (ViewInfo.ItemSize.Height + ViewInfo.ItemVerticalIndent);
			if(info.Bounds.Y < startRowY && info.Bounds.Bottom < startRowY) {
				info.IsEmpty = true;
				return;
			}
			int rowsCount = info.ItemsCount / ViewInfo.AvailableColumnCount;
			rowsCount += info.ItemsCount % ViewInfo.AvailableColumnCount > 0 ? 1 : 0;
			if(info.GroupRowHandle != GridControl.InvalidRowHandle && info.GroupRowHandle > info.EndGroupRowHandle) {
				info.EndRow = rowsCount;
			}
			else {
				info.EndRow = ((info.Bounds.Bottom - firstRowY) + ViewInfo.ItemVerticalIndent) / (ViewInfo.ItemSize.Height + ViewInfo.ItemVerticalIndent);
				info.EndRow = Math.Min(rowsCount, info.EndRow);
				int endRowY = firstRowY + info.EndRow * (ViewInfo.ItemSize.Height + ViewInfo.ItemVerticalIndent);
				if(info.Bounds.Bottom < endRowY)
					info.EndRow--;
			}
			if(info.EndRow < info.StartRow)
				info.IsEmpty = true;
		}
		protected override void SelectItemsByRect(WinExplorerViewMarqueeSelectionInfo info) {
			if(info.IsEmpty)
				return;
			int startRowHandle = info.FirstRowHandle + info.StartRow * ViewInfo.AvailableColumnCount + info.StartColumn;
			int endRowHandle = info.FirstRowHandle + info.EndRow * ViewInfo.AvailableColumnCount + info.StartColumn;
			int columnCount = info.EndColumn - info.StartColumn;
			endRowHandle = Math.Min(endRowHandle, info.FirstRowHandle + info.ItemsCount - 1);
			for(int i = startRowHandle; i <= endRowHandle; i += ViewInfo.AvailableColumnCount) {
				int endColumnHandle = Math.Min(info.FirstRowHandle + info.ItemsCount - 1, i + columnCount);
				for(int rowHandle = i; rowHandle <= endColumnHandle; rowHandle++) {
					View.DataController.Selection.SetSelected(rowHandle, true);
				}
			}
		}
		protected override int ScrollDelta { get { return 10; } }
	}
	public class WinExplorerViewMarqueeSelectionCalculatorList : WinExplorerViewMarqueeSelectionCalculator {
		public WinExplorerViewMarqueeSelectionCalculatorList(WinExplorerViewHandler handler) : base(handler) { }
		public override bool SupportItemType(WinExplorerViewStyle itemType) {
			return itemType == WinExplorerViewStyle.List;
		}
		protected override int ScrollDelta { get { return 1; } }
		protected override int ScrollInterval { get { return 100; } }
		protected override bool ShouldScrollNext(Point point) {
			return point.X > ViewInfo.ClientBounds.Right;
		}
		protected override bool ShouldScrollPrev(Point point) {
			return point.X < ViewInfo.ClientBounds.X;
		}
		protected virtual int GetIndentBetweenColumns(int column1, int column2) {
			WinExplorerItemViewInfo item1 = null, item2 = null;
			foreach(WinExplorerItemViewInfo itemInfo in ViewInfo.VisibleItems) {
				if(item1 == null && !itemInfo.IsGroupItem && itemInfo.Column == column1)
					item1 = itemInfo;
				if(item2 == null && !itemInfo.IsGroupItem && itemInfo.Column == column2)
					item2 = itemInfo;
				if(item1 != null && item2 != null)
					break;
			}
			if(item1 == null || item2 == null)
				return ViewInfo.ItemHorizontalIndent;
			int res = View.GetParentRowHandle(item1.Row.RowHandle) == View.GetParentRowHandle(item2.Row.RowHandle) ? ViewInfo.ItemHorizontalIndent : ViewInfo.GroupIndent;
			if(column1 == 0 || column2 == 0)
				res += ViewInfo.ContentBounds.X - ViewInfo.ClientBounds.X;
			return res;
		}
		protected override void ScrollNext() {
			if(View.Position == ViewInfo.CalcScrollableAreaSize())
				return;
			ViewInfo.SelectionStartPoint = new Point(ViewInfo.SelectionStartPoint.X - ViewInfo.GetColumnWidth(View.Position) - GetIndentBetweenColumns(View.Position, View.Position + 1), ViewInfo.SelectionStartPoint.Y);
			base.ScrollNext();
		}
		protected override void ScrollPrev() {
			if(View.Position == 0)
				return;
			base.ScrollPrev();
			ViewInfo.SelectionStartPoint = new Point(ViewInfo.SelectionStartPoint.X + ViewInfo.GetColumnWidth(View.Position) + GetIndentBetweenColumns(View.Position + 1, View.Position), ViewInfo.SelectionStartPoint.Y);
		}
		public override void OnMouseDown(DXMouseEventArgs e) {
			base.OnMouseDown(e);
			SelectionInfo.MouseStartPosition = e.Location;
			SelectionInfo.StartScreenItems = GetScreenItems(true);
		}
		protected virtual List<WinExplorerItemViewInfo> GetScreenItems(bool makeCopy) {
			List<WinExplorerItemViewInfo> res = new List<WinExplorerItemViewInfo>();
			foreach(WinExplorerItemViewInfo item in ViewInfo.VisibleItems) {
				WinExplorerItemViewInfo newItem = makeCopy? item.Clone(): item;
				res.Add(newItem);
			}
			return res;
		}
		protected override WinExplorerViewMarqueeSelectionInfo GetSelectionInfo() {
			return SelectionInfo;
		}
		public override void OnMouseMove(Point pt) {
			SelectionInfo.MouseEndPosition = pt;
			SelectionInfo.EndScreenItems = GetScreenItems(false);
			SelectionInfo.IsEmpty = false;
			base.OnMouseMove(pt);
		}
		public override void OnMouseUp(DXMouseEventArgs e) {
			base.OnMouseUp(e);
			ResetSelectionInfo();
		}
		protected virtual void ResetSelectionInfo() {
			this.selectionInfo = null;
		}
		WinExplorerViewMarqueeSelectionInfo selectionInfo;
		protected WinExplorerViewMarqueeSelectionInfo SelectionInfo {
			get {
				if(selectionInfo == null)
					selectionInfo = new WinExplorerViewMarqueeSelectionInfo();
				return selectionInfo;
			}
		}
		protected override void SelectItemsByRectInGroups(WinExplorerViewMarqueeSelectionInfo info) {
			CalcSelectionColumns(info);
			if(info.IsEmpty)
				return;
			info.StartGroupRowHandle = -1 - ViewInfo.CalcTopVisibleGroupIndex(info.StartColumn);
			info.EndGroupRowHandle = -1 - ViewInfo.CalcTopVisibleGroupIndex(info.EndColumn);
			CalcSelectionRows(info);
			for(int i = info.StartGroupRowHandle; i >= info.EndGroupRowHandle; i--) {
				info.GroupRowHandle = i;
				CalcSelectionItemsInfo(info);
				if(info.IsEmpty)
					continue;
				SelectItemsByRect(info);
			}
		}
		protected override void CalcSelectionColumns(WinExplorerViewMarqueeSelectionInfo info) {
			int startColumn, nearStartColumn, endColumn, nearEndColumn;
			CalcColumn(info.StartScreenItems, info.MouseStartPosition, out startColumn, out nearStartColumn, true);
			CalcColumn(info.EndScreenItems, info.MouseEndPosition, out endColumn, out nearEndColumn, false);
			info.InvertedSelection = false;
			if(startColumn != -1) {
				if(endColumn != -1) {
					info.InvertedSelection = startColumn > endColumn || (startColumn == endColumn && info.MouseStartPosition.X > info.MouseEndPosition.X);
					info.StartColumn = Math.Min(startColumn, endColumn);
					info.EndColumn = Math.Max(startColumn, endColumn);
				}
				else {
					if(nearEndColumn > startColumn) {
						info.StartColumn = startColumn;
						info.EndColumn = nearEndColumn - 1;
					}
					else {
						info.InvertedSelection = true;
						info.StartColumn = nearEndColumn;
						info.EndColumn = startColumn;
					}
				}
			}
			else {
				if(endColumn != -1) {
					if(nearStartColumn <= endColumn) {
						info.StartColumn = nearStartColumn;
						info.EndColumn = endColumn;
					}
					else {
						info.InvertedSelection = true;
						info.StartColumn = endColumn;
						info.EndColumn = nearStartColumn - 1;
					}
				}
				else {
					if(nearStartColumn < nearEndColumn) {
						info.StartColumn = nearStartColumn;
						info.EndColumn = nearEndColumn - 1;
					}
					else if(nearStartColumn > nearEndColumn) {
						info.InvertedSelection = true;
						info.StartColumn = nearEndColumn;
						info.EndColumn = nearStartColumn - 1;
					}
					else {
						info.IsEmpty = true;
					}
				}
			}
		}
		protected virtual void CalcColumn(List<WinExplorerItemViewInfo> items, Point pt, out int column, out int nearColumn, bool start) {
			nearColumn = -1;
			column = -1;
			foreach(WinExplorerItemViewInfo item in items) {
				if(item.IsGroupItem)
					continue;
				if(item.SelectionBounds.X < pt.X && item.SelectionBounds.Right > pt.X) {
					nearColumn = -1;
					column = item.Column;
					return;
				}
				if(item.SelectionBounds.X >= pt.X) {
					column = -1;
					nearColumn = item.Column;
					return;
				}
			}
			if(items.Count > 0) {
				if(pt.X > items[items.Count - 1].Bounds.X)
					column = items[items.Count - 1].Column;
				else if(pt.X < items[0].Bounds.X)
					column = items[0].Column;
				else
					column = start ? items[0].Column : items[items.Count - 1].Column;
			}
		}
		protected override void CalcSelectionRows(WinExplorerViewMarqueeSelectionInfo info) {
			info.StartRow = ((info.Bounds.Y - ViewInfo.ItemsTopLocation) + ViewInfo.ItemVerticalIndent) / (ViewInfo.ItemSize.Height + ViewInfo.ItemVerticalIndent);
			info.StartRow = Math.Max(info.StartRow, 0);
			int startRowY = ViewInfo.CalcRowLocationHorizontal(info.StartRow);
			if(info.Bounds.Y < startRowY && info.Bounds.Bottom < startRowY) {
				info.IsEmpty = true;
				return;
			}
			info.EndRow = ((info.Bounds.Bottom - ViewInfo.ItemsTopLocation) + ViewInfo.ItemVerticalIndent) / (ViewInfo.ItemSize.Height + ViewInfo.ItemVerticalIndent);
			info.EndRow = Math.Min(info.EndRow, ViewInfo.AvailableRowCountHorizontal - 1);
			int endRowY = ViewInfo.CalcRowLocationHorizontal(info.EndRow);
			if(info.Bounds.Bottom < endRowY)
				info.EndRow--;
			if(info.EndRow < info.StartRow) {
				info.IsEmpty = true;
			}
		}
		protected override void SelectItemsByRect(WinExplorerViewMarqueeSelectionInfo info) {
			if(info.IsEmpty)
				return;
			int startColumn, endColumn, columnIndex;
			int selectionStartColumn, selectionEndColumn;
			if(info.GroupRowHandle == GridControl.InvalidRowHandle) {
				startColumn = info.StartColumn;
				endColumn = info.EndColumn;
				selectionStartColumn = info.StartColumn;
				selectionEndColumn = info.EndColumn;
			}
			else {
				int startGroupColumn = ViewInfo.Calculator.ScrollingCache[-info.GroupRowHandle - 1];
				int groupColumnCount = ViewInfo.CalcGroupColumnCount(info.GroupRowHandle);
				selectionStartColumn = info.StartColumn - startGroupColumn;
				selectionEndColumn = info.EndColumn - startGroupColumn;
				startColumn = Math.Max(0, selectionStartColumn);
				endColumn = Math.Max(0, Math.Min(selectionEndColumn, groupColumnCount - 1));
			}
			columnIndex = selectionStartColumn;
			int startColumnHandle = info.FirstRowHandle + startColumn * ViewInfo.AvailableRowCountHorizontal + info.StartRow;
			int endColumnHandle = info.FirstRowHandle + endColumn * ViewInfo.AvailableRowCountHorizontal + info.StartRow;
			int rowCount = info.EndRow - info.StartRow;
			endColumnHandle = Math.Min(endColumnHandle, info.FirstRowHandle + info.ItemsCount - 1);
			for(int i = startColumnHandle; i <= endColumnHandle; i += ViewInfo.AvailableRowCountHorizontal, columnIndex ++) {
				int endRowHandle = Math.Min(info.FirstRowHandle + info.ItemsCount - 1, i + rowCount);
				int itemIndex = -1;
				Point location = info.InvertedSelection ? info.MouseEndPosition : info.MouseStartPosition;
				int startColIndex = info.InvertedSelection ? selectionEndColumn : selectionStartColumn;
				int startColHandle = info.InvertedSelection ? endColumnHandle : startColumnHandle;
				if(columnIndex == startColIndex) {
					itemIndex = info.StartScreenItems.IndexOf(GetItem(info.StartScreenItems, startColHandle));
					if(itemIndex == -1) { 
					}
				}
				for(int rowHandle = i; rowHandle <= endRowHandle; rowHandle++, itemIndex++) {
					if(columnIndex == startColIndex) { 
						if(itemIndex >= 0 && info.StartScreenItems[itemIndex].SelectionBounds.Right < location.X)
							continue;
					}
					View.DataController.Selection.SetSelected(rowHandle, true);
				}
			}
		}
		protected bool ContainsPoint(Rectangle rect, Point pt) {
			return rect.X < pt.X && rect.Right > pt.X;
		}
		private WinExplorerItemViewInfo GetItem(List<WinExplorerItemViewInfo> list, int rowHandle) {
			foreach(WinExplorerItemViewInfo itemInfo in list) {
				if(itemInfo.Row.RowHandle == rowHandle)
					return itemInfo;
			}
			return null;
		}
	}
	internal static class ModifierKeysHelper {
		public static bool UseTestKeys { get; set; }
		public static Keys TestModifierKeys { get; set; }
		public static Keys ModifierKeys {
			get {
				if(UseTestKeys)
					return TestModifierKeys;
				return Control.ModifierKeys; 
			}
		}
		public static bool IsControlPressed {
			get { return ModifierKeys == Keys.ControlKey || ModifierKeys == Keys.Control || ModifierKeys == Keys.LControlKey || ModifierKeys == Keys.RControlKey; }
		}
		public static bool IsShiftPressed {
			get { return ModifierKeys == Keys.ShiftKey || ModifierKeys == Keys.Shift || ModifierKeys == Keys.LShiftKey || ModifierKeys == Keys.RShiftKey; }
		}
	}
}

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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using DevExpress.XtraBars.Docking2010;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars.Utils;
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class BackstageViewControlHandler {
		BackstageViewControl control;
		public BackstageViewControlHandler(BackstageViewControl control) {
			this.control = control;
		}
		public BackstageViewControl BackstageView { get { return control; } }
		public BackstageViewInfo ViewInfo { get { return BackstageView.ViewInfo; } }
		static Point InvalidPoint = new Point(-10000, -10000);
		Point downPoint;
		public Point DownPoint {
			get { return downPoint; }
		}
		public virtual void OnMouseDown(DXMouseEventArgs e) {
			ViewInfo.PressedItem = ViewInfo.GetItemByPoint(e.Location);
			this.downPoint = e.Location;
			if(BackstageView.DesignModeCore) {
				BackstageViewItemBaseViewInfo item = ViewInfo.GetItemByPoint(e.Location, true);
				ViewInfo.SetDragItem(item);
				if(item != null)
					ViewInfo.DesignTimeManager.SelectComponent(item.Item);
				BackstageView.Invalidate(ViewInfo.ItemsBounds);
			}
		}
		public virtual void OnMouseDoubleClick(DXMouseEventArgs e) {
			if(BackstageView.DesignModeCore) {
				BackstageViewItemBaseViewInfo item = ViewInfo.GetItemByPoint(e.Location, true);
				if(item != null)
					DoDefaultAction(item);
			}
		}
		protected internal virtual void DoDefaultAction(BackstageViewItemBaseViewInfo item) {
			IDesignerHost host = ViewInfo.DesignTimeManager.DesignerHost;
			if(host != null) {
				IDesigner designer = host.GetDesigner(item.Item) as IDesigner;
				if(designer != null) designer.DoDefaultAction();
			}
		}
		public virtual void OnMouseUp(DXMouseEventArgs e) {
			BackstageViewButtonItemViewInfo buttonItem = ViewInfo.GetItemByPoint(e.Location) as BackstageViewButtonItemViewInfo;
			if(!BackstageView.DesignModeCore && buttonItem == ViewInfo.PressedItem && buttonItem != null && buttonItem.Item.Enabled)
				buttonItem.Item.RaiseItemClick();
			ViewInfo.PressedItem = null;
			ViewInfo.SetDragItem(null);
			this.downPoint = InvalidPoint;
		}
		bool ShouldStartDrag(Point pt1, Point pt2) {
			return Math.Abs(pt1.X - pt2.X) > SystemInformation.DragSize.Width || Math.Abs(pt1.Y - pt2.Y) > SystemInformation.DragSize.Height;
		}
		public virtual void OnMouseMove(DXMouseEventArgs e) {
			ViewInfo.HotItem = ViewInfo.GetItemByPoint(e.Location);
			if(DownPoint != InvalidPoint && BackstageView.DesignModeCore && ShouldStartDrag(e.Location, DownPoint) && ViewInfo.DragItem != null) {
				BackstageViewDragControl c = new BackstageViewDragControl(this);
				DragDropEffects effects = DragDropEffects.None;
				try {
					effects = c.DoDragDrop(ViewInfo.DragItem.Item, DragDropEffects.Move | DragDropEffects.Copy);
				}
				catch(Exception) {
					effects = DragDropEffects.None;
				}
				StopDragging(effects);
				c.Dispose();
			}
		}
		public virtual void OnMouseWheel(DXMouseEventArgs e) {
			if(!ViewInfo.IsVScrollVisible) return;
			int wheelChange = ViewInfo.CalcVSmallChange();
			control.ScrollerPosition += (e.Delta > 0) ? -wheelChange : wheelChange;
		}
		public virtual bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if(!BackstageView.Focused)
				return false;
			if(keyData == Keys.Up || keyData == Keys.Down) {
				if(ViewInfo.SelectedItem == null) {
					InitSelectedItem();
					return false;
				}
			}
			if(keyData == Keys.Up) {
				BackstageViewItemViewInfo itemInfo = GetPrevItem(ViewInfo.SelectedItem);
				if(itemInfo != null)
					ViewInfo.SelectedItem = itemInfo;
				BackstageView.Focus();
				return true;
			}
			else if(keyData == Keys.Down) {
				BackstageViewItemViewInfo itemInfo = GetNextItem(ViewInfo.SelectedItem);
				if(itemInfo != null)
					ViewInfo.SelectedItem = itemInfo;
				BackstageView.Focus();
				return true;
			} else if(keyData == Keys.Tab && BackstageView.SelectedTab != null) {
				BackstageView.SelectedTab.ContentControl.Focus();
				return true;
			}
			else if(msg.Msg == MSG.WM_KEYDOWN && BackstageView.ParentBackstageView != null) {
				if(BackstageView.ShouldShowKeyTips) {
					char ch = (char)msg.WParam.ToInt32();
					if(BackstageView.KeyTipManager.Show && (char.IsLetter(ch) || char.IsNumber(ch))) {
						BackstageView.KeyTipManager.AddChar(ch);
					}
				}
				return true;
			}
			return false;
		}
		public virtual void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter) {
				BackstageViewButtonItemViewInfo buttonItem = ViewInfo.SelectedItem as BackstageViewButtonItemViewInfo;
				if(buttonItem != null)
					buttonItem.Item.RaiseItemClick();
			}
		}
		BackstageViewItemViewInfo GetPrevItem(BackstageViewItemViewInfo item) {
			if(item == null)
				return null;
			for(int i = ViewInfo.Items.IndexOf(item) - 1; i >= 0; i--) {
				BackstageViewItemViewInfo itemInfo = ViewInfo.Items[i] as BackstageViewItemViewInfo;
				if(itemInfo != null && itemInfo.Item.Enabled)
					return itemInfo;
			}
			return null;
		}
		BackstageViewItemViewInfo GetNextItem(BackstageViewItemViewInfo item) {
			if(item == null)
				return null;
			for(int i = ViewInfo.Items.IndexOf(item) + 1; i < ViewInfo.Items.Count; i++) {
				BackstageViewItemViewInfo itemInfo = ViewInfo.Items[i] as BackstageViewItemViewInfo;
				if(itemInfo != null && itemInfo.Item.Enabled)
					return itemInfo;
			}
			return null;
		}
		BackstageViewItemViewInfo GetFirstItem() {
			for(int i = 0; i < ViewInfo.Items.Count; i++) {
				if(ViewInfo.Items[i] is BackstageViewItemViewInfo)
					return (BackstageViewItemViewInfo)ViewInfo.Items[i];
			}
			return null;
		}
		protected internal virtual void InitSelectedItem() {
			if(BackstageView.SelectedTab != null)
				ViewInfo.SelectedItem = (BackstageViewItemViewInfo)ViewInfo.GetItemInfo(BackstageView.SelectedTab);
			else if(ViewInfo.Items.Count > 0)
				ViewInfo.SelectedItem = GetFirstItem();
		}
		public virtual void OnMouseLeave(DXMouseEventArgs ee) {
			ViewInfo.HotItem = null;
		}
		public virtual void OnMouseEnter(DXMouseEventArgs ee) {
			ViewInfo.HotItem = ViewInfo.GetItemByPoint(ee.Location);
		}
		int dragCursorIndex = 0;
		protected int DragCursorIndex { get { return dragCursorIndex; } }
		protected virtual BackstageViewItemBaseViewInfo GetDropItem(Point pt) {
			BackstageViewItemBaseViewInfo res = ViewInfo.GetItemByPoint(pt, true);
			if(res != null)
				return res;
			if(pt.X < ViewInfo.ItemsBounds.X || pt.X > ViewInfo.ItemsBounds.Right)
				return null;
			for(int i = 0; i < ViewInfo.Items.Count - 1; i++) {
				if(ViewInfo.Items[i].Bounds.Bottom <= pt.Y && pt.Y <= ViewInfo.Items[i + 1].Bounds.Top)
					return ViewInfo.Items[i];
			}
			return null;
		}
		protected virtual void DoDragging(MouseEventArgs e) {
			if(ViewInfo.DragItem == null || !BackstageView.DesignModeCore) {
				this.dragCursorIndex = BarManager.NoDropCursor;
				Cursor.Current = (Cursor)BackstageView.GetController().DragCursors[DragCursorIndex];
				return;
			}
			Point pt = BackstageView.PointToClient(new Point(e.X, e.Y));
			ViewInfo.SetDropItem(GetDropItem(pt));
			if(ViewInfo.DropItem == null) {
				if(pt.X < ViewInfo.ItemsBounds.X || pt.X > ViewInfo.ItemsBounds.Right || !BackstageView.ClientRectangle.Contains(pt)) {
					this.dragCursorIndex = BarManager.NoDropCursor;
					Cursor.Current = (Cursor)BackstageView.GetController().DragCursors[DragCursorIndex];
					ViewInfo.Invalidate(ViewInfo.DropBounds);
					return;
				}
				if(pt.Y <= ViewInfo.ItemsBounds.Top) {
					ViewInfo.SetDropItem(ViewInfo.Items[0]);
				}
				else if(pt.Y >= ViewInfo.ItemsBounds.Bottom)
					ViewInfo.SetDropItem(ViewInfo.Items[ViewInfo.Items.Count - 1]);
			}
			if(ViewInfo.DropItem == null) {
				this.dragCursorIndex = BarManager.NoDropCursor;
				Cursor.Current = (Cursor)BackstageView.GetController().DragCursors[DragCursorIndex];
			}
			else {
				if(pt.Y < (ViewInfo.DropItem.Bounds.Y + ViewInfo.DropItem.Bounds.Height / 2))
					ViewInfo.SetDropIndicatorLocation(ItemLocation.Top);
				else
					ViewInfo.SetDropIndicatorLocation(ItemLocation.Bottom);
				this.dragCursorIndex = Control.ModifierKeys == Keys.Control ? BarManager.CopyCursor : BarManager.DragCursor;
				Cursor.Current = (Cursor)BackstageView.GetController().DragCursors[DragCursorIndex];
			}
			ViewInfo.Invalidate(ViewInfo.DropBounds);
		}
		public virtual void OnDragOver(DragEventArgs e) {
			Point pt = Control.MousePosition;
			DoDragging(new MouseEventArgs(Control.MouseButtons, 0, pt.X, pt.Y, 0));
			if(DragCursorIndex == BarManager.NoDropCursor)
				e.Effect = DragDropEffects.None;
			else if(DragCursorIndex == BarManager.CopyCursor)
				e.Effect = DragDropEffects.Copy;
			else if(DragCursorIndex == BarManager.DragCursor)
				e.Effect = DragDropEffects.Move;
		}
		int GetInsertPosition() {
			if(ViewInfo.DropItem == null)
				return BackstageView.Items.Count;
			int dropItemIndex = BackstageView.Items.IndexOf(ViewInfo.DropItem.Item);
			if(ViewInfo.DropIndicatorLocation == ItemLocation.Bottom)
				dropItemIndex++;
			return dropItemIndex;
		}
		public virtual void OnDragDrop(DragEventArgs e) {
			StopDragging(e.Effect);
		}
		protected virtual void StopDragging(DragDropEffects effects) {
			BackstageViewItemBaseViewInfo item = ViewInfo.DragItem;
			if(item == null || !BackstageView.DesignModeCore) {
				return;
			}
			if(effects == DragDropEffects.None || DragCursorIndex == BarManager.NoDropCursor) {
				if(BackstageView.Container != null)
					BackstageView.Container.Remove(ViewInfo.DragItem.Item);
				if(BackstageView.Items.Contains(ViewInfo.DragItem.Item))
					BackstageView.Items.Remove(ViewInfo.DragItem.Item);
			}
			else if(effects == DragDropEffects.Copy) {
				BackstageViewItemBase copy = (BackstageViewItemBase)item.Item.Clone();
				BackstageView.Container.Add(copy);
				BackstageView.Items.Insert(GetInsertPosition(), copy);
			}
			else if(effects == DragDropEffects.Move) {
				if(ViewInfo.DropItem == null || item.Item != ViewInfo.DropItem.Item) {
					BackstageView.Items.Remove(item.Item);
					BackstageView.Items.Insert(GetInsertPosition(), item.Item);
				}
			}
			BackstageView.FireBackstageViewChanged(BackstageView);
			ViewInfo.SetDragItem(null);
			ViewInfo.SetDropItem(null);
			ViewInfo.Invalidate(ViewInfo.DropBounds);
		}
		public virtual void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			if(BackstageView.DesignModeCore) {
				if(e.EscapePressed) {
					e.Action = DragAction.Cancel;
					ViewInfo.SetDragItem(null);
					ViewInfo.Invalidate(ViewInfo.DropBounds);
					return;				
				}
				if(FireDoDragging) {
					DoDragging(new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
				}
			}
		}
		public virtual void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if(FireDoDragging) {
				e.UseDefaultCursors = false;
				DoDragging(new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
			}
		}
		bool FireDoDragging = false;
		public virtual void OnDragEnter(DragEventArgs e) {
			const int CtrlKeyPressed = 8;
			this.FireDoDragging = false;
			if(ViewInfo.DragItem == null || !BackstageView.DesignModeCore)
				e.Effect = DragDropEffects.None;
			else
				e.Effect = (e.KeyState & CtrlKeyPressed) != 0 ? DragDropEffects.Copy : DragDropEffects.Move;
		}
		public virtual void OnDragLeave(EventArgs e) {
			ViewInfo.SetDropItem(null);
			this.FireDoDragging = true;
			ViewInfo.Invalidate(ViewInfo.DropBounds);
		}
	}
	public class Office2013BackstageViewControlHandler : BackstageViewControlHandler {
		BackstageViewMovingManager movingManager;
		public Office2013BackstageViewControlHandler(BackstageViewControl control) : base(control) {
			this.movingManager = CreateMovingManager();
		}
		protected virtual BackstageViewMovingManager CreateMovingManager() {
			return new BackstageViewMovingManager(BackstageView);
		}
		public override void OnMouseDown(DXMouseEventArgs e) {
			base.OnMouseDown(e);
			MovingManager.OnMouseDown(e);
			BackstageViewHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(hitInfo.HitTest == BackstageViewHitTest.BackButton) {
				OnBackButtonPress(e, hitInfo);
			}
			BackstageView.Focus();
		}
		public override void OnMouseUp(DXMouseEventArgs e) {
			base.OnMouseUp(e);
			MovingManager.OnMouseUp(e);
		}
		public override void OnMouseMove(DXMouseEventArgs e) {
			base.OnMouseMove(e);
			MovingManager.OnMouseMove(e);
			UpdateHotObject(e);
		}
		public virtual void UpdateHotObject(DXMouseEventArgs e) {
			BackstageViewHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(CanHotTrack(hitInfo)) ViewInfoCore.HotObject = hitInfo;
		}
		protected virtual bool CanHotTrack(BackstageViewHitInfo hitInfo) {
			if(ViewInfo.Control.DesignModeCore) return false;
			return true;
		}
		protected virtual void OnBackButtonPress(DXMouseEventArgs e, BackstageViewHitInfo hitInfo) {
			OwnerControl.OnBackButtonClick();
		}
		protected BackstageViewMovingManager MovingManager { get { return movingManager; } }
		protected BackstageViewControl OwnerControl { get { return ViewInfo.Control; } }
		protected Office2013BackstageViewInfo ViewInfoCore { get { return ViewInfo as Office2013BackstageViewInfo; } }
	}
	internal class BackstageViewDragControl : Control {
		BackstageViewControlHandler handler;
		public BackstageViewDragControl(BackstageViewControlHandler handler) {
			this.handler = handler;
		}
		public BackstageViewControlHandler Handler { get { return this.handler; } }
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			Handler.OnQueryContinueDrag(e);
		}   
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			Handler.OnGiveFeedback(e);
		}
	}
	#region Moving
	public class BackstageViewMovingManager {
		BackstageViewMovingInfo moveInfoCore;
		BackstageViewControl backstageView;
		public BackstageViewMovingManager(BackstageViewControl backstageView) {
			this.backstageView = backstageView;
		}
		public void OnMouseDown(DXMouseEventArgs e) {
			if(!CanMouseMove(e))
				return;
			moving = true;
			moveInfoCore = new BackstageViewMovingInfo(CursorPos);
		}
		public void OnMouseUp(DXMouseEventArgs e) {
			moving = false;
		}
		bool moving = false;
		public void OnMouseMove(DXMouseEventArgs e) {
			if(e.Handled || !moving) 
				return;
			MoveInfoCore.UpdateCurrentLocation(CursorPos);
			CheckFormState();			
			UpdateLocationCore();
			MoveInfoCore.MovingFinished();
		}
		protected virtual void CheckFormState() { 
			if(BackstageView == null || PopupContainer == null)
				return;
			if(TopForm.WindowState != FormWindowState.Maximized)
				return;
			if(!MoveInfoCore.ShouldMove)
				return;
			BackstageView.ViewInfo.OnBeginSizingCore();
			TopForm.WindowState = FormWindowState.Normal;
			PopupContainer.WindowState = FormWindowState.Normal;
			PopupContainer.Size = TopForm.RestoreBounds.Size;
			PopupContainer.Location = new Point(CursorPos.X - PopupContainer.Size.Width / 2, Screen.GetWorkingArea(BackstageView).Y);
			BackstageView.ViewInfo.OnEndSizingCore();
		}
		protected virtual void UpdateLocationCore() {
			if(!MoveInfoCore.ShouldMove)
				return;
			UpdateTopFormLocationCore();
			UpdatePopupLocationCore();
		}
		protected virtual void UpdateTopFormLocationCore() {
			Point loc = TopForm.Location;
			TopForm.Location = new Point(loc.X + MoveInfoCore.XOffset, loc.Y + MoveInfoCore.YOffset);
		}
		protected virtual void UpdatePopupLocationCore() {
			Point loc = PopupContainer.Location;
			PopupContainer.Location = new Point(loc.X + MoveInfoCore.XOffset, loc.Y + MoveInfoCore.YOffset);
		}
		protected virtual bool CanMouseMove(DXMouseEventArgs e) {
			if(TopForm == null)
				return false;
			return HotRegion.Contains(e.Location);
		}
		protected virtual Rectangle HotRegion {
			get {
				int left = BackstageView.IsRightToLeft ? 0 : BackstageView.ViewInfo.LeftPaneContentBounds.Right;
				return new Rectangle(left, 0, BackstageView.Width - left, TitleHeight);
			}
		}
		protected int TitleHeight {
			get { return BackstageView.ViewInfo.CalcRightPaneBounds().Y; }
		}
		protected Form TopForm {
			get {
				if(BackstageView.Ribbon == null)
					return null;
				return BackstageView.Ribbon.FindForm();
			}
		}
		protected Form PopupContainer {
			get { return BackstageView.Parent as Form; }
		}
		protected Point CursorPos { get { return Cursor.Position; } }
		public BackstageViewMovingInfo MoveInfoCore { get { return moveInfoCore; } }
		public BackstageViewControl BackstageView { get { return backstageView; } }
	}
	public class BackstageViewMovingInfo {
		public BackstageViewMovingInfo(Point mouseStartLocation) {
			SetStartPos(mouseStartLocation);
		}
		public void SetStartPos(Point pos) {
			MouseStartLocation = MouseCurrentLocation = pos;
		}
		public void UpdateCurrentLocation(Point loc) {
			MouseCurrentLocation = loc;
		}
		public void MovingFinished() {
			MouseStartLocation = MouseCurrentLocation;
		}
		public bool ShouldMove {
			get { return MouseCurrentLocation != MouseStartLocation; }
		}
		public int XOffset { get { return MouseCurrentLocation.X - MouseStartLocation.X; } }
		public int YOffset { get { return MouseCurrentLocation.Y - MouseStartLocation.Y; } }
		public Point MouseStartLocation { get; set; }
		public Point MouseCurrentLocation { get; set; }
	}
	#endregion
	#region Sizing
	public class BackstageViewSizingManager {
		ISizableControl sizableControl;
		BackstageViewSizingInfo sizingInfoCore;
		public BackstageViewSizingManager(ISizableControl sizableControl) {
			this.sizableControl = sizableControl;
			this.sizingInfoCore = new BackstageViewSizingInfo(sizableControl.Owner, Point.Empty);
		}
		public void OnMouseDown(DXMouseEventArgs e) {
			if(!CanUpdateSize(e))
				return;
			dragging = true;
			SizingInfoCore.SetStartPos(CursorPos);
			SizingInfoCore.ApplyCursor();
			SizableControl.Owner.Capture = true;
		}
		public void OnMouseUp(DXMouseEventArgs e) {
			dragging = false;
			SizableControl.Owner.Capture = false;
			if(isSizingCore)
				OnEndSizing();
		}
		bool dragging = false;
		public void OnMouseMove(DXMouseEventArgs e) {
			if(!dragging && !CanUpdateSize(e)) {
				SizingInfoCore.RestoreCursor();
				return;
			}
			SizingInfoCore.ApplyCursor();
			if(!dragging)
				return;
			if(!isSizingCore)
				OnBeginSizing();
			e.Handled = true;
			SizingInfoCore.UpdateCurrentLocation(CursorPos);
			DoSizing();
			SizingInfoCore.MovingFinished();
		}
		protected void UpdateBoundsCore(Control control, int dx, int dy, int dWidth, int dHeight) {
			if(control == null) return;
			try {
				control.SuspendLayout();
				Rectangle bounds = control.Bounds;
				control.Location = new Point(bounds.X + dx, bounds.Y + dy);
				control.Size = new Size(bounds.Width + dWidth, bounds.Height + dHeight);
			}
			finally {
				control.ResumeLayout();	
			}
		}
		protected virtual void OnBeginSizing() {
			isSizingCore = true;
			SizableControl.OnBeginSizing();
		}
		protected virtual void OnEndSizing() {
			isSizingCore = false;
			SizableControl.OnEndSizing();
		}
		bool isSizingCore = false;
		protected void UpdateBounds(int dx, int dy, int dWidth, int dHeight) {
			if(!SizableControl.CanUpdateBounds(dWidth, dHeight))
				return;
			UpdateBoundsCore(PopupContainer, dx, dy, dWidth, dHeight);
		}
		protected virtual void DoSizing() {
			if(!SizingInfoCore.ShouldMove)
				return;
			switch(SizingInfoCore.Mode) {
				case BackstageViewSizingInfo.SizingMode.Top:
					UpdateBounds(0, SizingInfoCore.YOffset, 0, -SizingInfoCore.YOffset);
					break;
				case BackstageViewSizingInfo.SizingMode.Bottom:
					UpdateBounds(0, 0, 0, SizingInfoCore.YOffset);
					break;
				case BackstageViewSizingInfo.SizingMode.Left:
					UpdateBounds(SizingInfoCore.XOffset, 0, -SizingInfoCore.XOffset, 0);
					break;
				case BackstageViewSizingInfo.SizingMode.Right:
					UpdateBounds(0, 0, SizingInfoCore.XOffset, 0);	
					break;
				case BackstageViewSizingInfo.SizingMode.LeftBottom:
					UpdateBounds(SizingInfoCore.XOffset, 0, -SizingInfoCore.XOffset, SizingInfoCore.YOffset);
					break;
				case BackstageViewSizingInfo.SizingMode.RightBottom:
					UpdateBounds(0, 0, SizingInfoCore.XOffset, SizingInfoCore.YOffset);
					break;
				case BackstageViewSizingInfo.SizingMode.RightTop:
					UpdateBounds(0, SizingInfoCore.YOffset, SizingInfoCore.XOffset, -SizingInfoCore.YOffset);
					break;
				case BackstageViewSizingInfo.SizingMode.LeftTop:
					UpdateBounds(SizingInfoCore.XOffset, SizingInfoCore.YOffset, -SizingInfoCore.XOffset, -SizingInfoCore.YOffset);
					break;
			}
		}
		protected Form PopupContainer {
			get { return SizableControl.Owner.Parent as Form; }
		}
		protected virtual bool CanUpdateSize(DXMouseEventArgs e) {
			if(!SizableControl.CanUpdateSize(e) || ((Control.MouseButtons & MouseButtons.Left) != 0) || SizableControl.OwnerForm.WindowState == FormWindowState.Maximized) return false;
			SizingInfoCore.UpdateSizingType(e.Location);
			return SizingInfoCore.Mode != BackstageViewSizingInfo.SizingMode.None;
		}
		public bool IsActive { get { return SizingInfoCore.Mode != BackstageViewSizingInfo.SizingMode.None; } }
		protected Point CursorPos { get { return Cursor.Position; } }
		protected BackstageViewSizingInfo SizingInfoCore { get { return sizingInfoCore; } }
		protected ISizableControl SizableControl { get { return sizableControl; } }
	}
	public class BackstageViewSizingInfo : BackstageViewMovingInfo {
		SizingMode mode;
		Control owner;
		Cursor sizingCursor;
		public BackstageViewSizingInfo(Control owner, Point mouseStartLocation)
			: base(mouseStartLocation) {
			this.mode = SizingMode.None;
			this.owner = owner;
			this.sizingCursor = null;
		}
		Cursor prevCursor = null;
		public void ApplyCursor() {
			if(object.ReferenceEquals(Cursor.Current, SizingCursor))
				return;
			this.prevCursor = Cursor.Current;
			Cursor.Current = SizingCursor;
		}
		public void RestoreCursor() {
			if(prevCursor == null) return;
			Cursor.Current = prevCursor;
			prevCursor = null;
		}
		public void UpdateSizingType(Point pos) {
			Mode = CalcSizingTypeCore(pos);
		}
		protected SizingMode CalcSizingTypeCore(Point pos) {
			if(IsHitInLeftEdge(pos)) {
				if(pos.Y < Offset) return SizingMode.LeftTop;
				if(pos.Y > Owner.Height - Offset) return SizingMode.LeftBottom;
				return SizingMode.Left;
			}
			if(IsHitInRightEdge(pos)) {
				if(pos.Y < Offset) return SizingMode.RightTop;
				if(pos.Y > Owner.Height - Offset) return SizingMode.RightBottom;
				return SizingMode.Right;
			}
			if(IsHitInTopEdge(pos)) {
				if(pos.X < Offset) return SizingMode.LeftTop;
				if(pos.X > Owner.Width - Offset) return SizingMode.RightTop;
				return SizingMode.Top;
			}
			if(IsHitInBottomEdge(pos)) {
				if(pos.X < Offset) return SizingMode.LeftBottom;
				if(pos.X > Owner.Width - Offset) return SizingMode.RightBottom;
				return SizingMode.Bottom;
			}
			return SizingMode.None;
		}
		protected bool IsHitInLeftEdge(Point pos) {
			return Math.Abs(pos.X) < GripSize;
		}
		protected bool IsHitInRightEdge(Point pos) {
			return Math.Abs(Owner.Width - 1 - pos.X) < GripSize;
		}
		protected bool IsHitInTopEdge(Point pos) {
			return Math.Abs(pos.Y) < GripSize;
		}
		protected bool IsHitInBottomEdge(Point pos) {
			return Math.Abs(Owner.Height - 1 - pos.Y) < GripSize;
		}
		protected void UpdateCursorCore() {
			sizingCursor = GetCursorCore();
		}
		protected Cursor GetCursorCore() {
			switch(Mode) {
				case SizingMode.Top:
				case SizingMode.Bottom:
					return Cursors.SizeNS;
				case SizingMode.Left:
				case SizingMode.Right:
					return Cursors.SizeWE;
				case SizingMode.LeftTop:
				case SizingMode.RightBottom:
					return Cursors.SizeNWSE;
				case SizingMode.LeftBottom:
				case SizingMode.RightTop:
					return Cursors.SizeNESW;
			}
			return null;
		}
		#region SizingMode
		public enum SizingMode {
			None, Top, Bottom, Left, Right, LeftBottom, LeftTop, RightTop, RightBottom
		}
		#endregion
		public SizingMode Mode {
			get { return mode; }
			protected set {
				if(mode == value) return;
				mode = value;
				UpdateCursorCore();
			}
		}
		protected virtual int Offset { get { return 40; } }
		protected virtual int GripSize { get { return 8; } }
		public Cursor SizingCursor { get { return sizingCursor; } }
		public Control Owner { get { return owner; } }
	}
	#endregion
	public class AeroSnapShortcutManager {
		ISizableControl sizableControl;
		public AeroSnapShortcutManager(ISizableControl sizableControl) {
			this.sizableControl = sizableControl;
			this.isProcessed = false;
			this.screenManager = new ScreenManager(sizableControl.Owner);
			this.currScreen = screenManager.CurrentScreen.WorkingArea;
			this.formStateController = CreateFormStateController();
			this.formState = FormState.Normal;
		}
		protected virtual FormStateController CreateFormStateController() {
			return new FormStateController();
		}
		public enum FormState { Normal, Left, Right, Max, Min };
		FormState formState;
		Rectangle currScreen;
		ScreenManager screenManager;
		FormStateController formStateController;
		bool lwinpressed = false;
		public void OnKeyDown(Keys key) {
			if(key == Keys.LWin) {
				lwinpressed = true;
			}
		}
		public void OnKeyUp(Keys key) {
			if(key == Keys.LWin) {
				lwinpressed = false;
			}
			if(!lwinpressed) return;
			UpdateState();
			if(formState == FormState.Min && SizableControl.OwnerForm.WindowState != FormWindowState.Minimized)
				formState = formStateController.CurrFormState;
			GetNextState(key);
			if(switchScreen)
				SwitchScreen();
			if(GetResult())
				DoAction();
		}
		protected internal virtual void SyncFormState() {
			var form = SizableControl.OwnerForm;
			if(RibbonForm == null) return;
			if(RibbonForm.WindowState == FormWindowState.Maximized && formState == FormState.Normal) 
				UpdateState();
			if(RibbonForm.WindowState == FormWindowState.Normal && (formState == FormState.Normal || formState == FormState.Max)) {
				formState = FormState.Normal;
				formStateController.AssignForm(form);				
				formStateController.SaveState(currScreen, Keys.Up, formState);
			}
		}
		public void OnMouseDblClk() {
			isProcessed = true;
			UpdateState();
			SyncFormState();
			if(formState == FormState.Normal) {
				formState = FormState.Max;
			} else {
				formState = FormState.Normal;
			}
			if(GetResult())
				DoAction();
		}
		bool isProcessed;
		public void ResetResult() {
			isProcessed = false;
		}
		public bool GetResult() {
			return isProcessed;
		}
		protected Rectangle GetRestoreBounds() {
			if(RibbonForm == null) return Rectangle.Empty;
			Rectangle bounds = BarUtilites.GetFormRestoreBounds(RibbonForm);
			return bounds.IsEmpty ? RibbonForm.RestoreBounds : bounds;
		}
		protected void UpdateState() {
			screenManager.RefreshScreen();
			currScreen = screenManager.CurrentScreen.WorkingArea;
			FormState newFormState = GetStartupStateCore();
			if(formState != newFormState) {
				formState = newFormState;
				formStateController.AssignForm(SizableControl.OwnerForm);
				formStateController.SaveTopFormBounds(currScreen, GetRestoreBounds());
			}
		}
		protected FormState GetStartupStateCore() {
			if(RibbonForm == null) return formState;
			if(RibbonForm.WindowState == FormWindowState.Maximized)
				return FormState.Max;
			if(formState == FormState.Normal) {
				if(IsLeftFormDocking())
					return FormState.Left;
				if(IsRightFormDocking())
					return FormState.Right;
			}
			return formState;
		}
		protected bool IsLeftFormDocking() {
			if(RibbonForm.Bounds.Width == currScreen.Width / 2 && RibbonForm.Bounds.Height == currScreen.Height)
				if(RibbonForm.Bounds.Location == currScreen.Location)
					return true;
			return false;
		}
		protected bool IsRightFormDocking() {
			if(RibbonForm.Bounds.Width == currScreen.Width / 2 && RibbonForm.Bounds.Height == currScreen.Height)
				if(RibbonForm.Bounds.Y == currScreen.Y && RibbonForm.Bounds.Right == currScreen.Right)
					return true;
			return false;
		}
		protected virtual void DoAction() {
			switch(formState) {
				case FormState.Normal:
					ResetDocking();
					break;
				case FormState.Left:
					DoLeftDock();
					break;
				case FormState.Right:
					DoRightDock();
					break;
				case FormState.Max:
					Maximize();
					break;
				case FormState.Min:
					Minimize();
					break;
			}
		}
		static Keys[] acceptableKeysCore = null;
		static Keys[] AcceptableKeys {
			get {
				if(acceptableKeysCore == null) {
					acceptableKeysCore = new Keys[] { Keys.Right, Keys.Left, Keys.Up, Keys.Down };
				}
				return acceptableKeysCore;
			}
		}
		protected virtual void GetNextState(Keys key) {
			if(!AcceptableKeys.Contains(key)) return;
			if(formState == FormState.Normal) {
				formStateController.AssignForm(SizableControl.OwnerForm);
				formStateController.SaveState(currScreen, key, formState);
			} 
			isProcessed = true;
			switch(formState) { 
				case FormState.Normal:
					if(key == Keys.Left) formState = FormState.Left;
					if(key == Keys.Right) formState = FormState.Right;
					if(key == Keys.Up) formState = FormState.Max;
					if(key == Keys.Down) formState = FormState.Min;
					break;
				case FormState.Left:
					if(key == Keys.Left) {
						switchScreen = true;
						formState = FormState.Right;
					}
					if(key == Keys.Right) formState = FormState.Normal;
					if(key == Keys.Up) formState = FormState.Max;
					if(key == Keys.Down) formState = FormState.Normal;
					break;
				case FormState.Right:
					if(key == Keys.Left) formState = FormState.Normal;
					if(key == Keys.Right) {
						switchScreen = true;
						formState = FormState.Left;
					}
					if(key == Keys.Up) formState = FormState.Max;
					if(key == Keys.Down) formState = FormState.Normal;
					break;
				case FormState.Max:
					if(key == Keys.Left) formState = FormState.Left;
					if(key == Keys.Right) formState = FormState.Right;
					if(key == Keys.Down) formState = FormState.Normal;
					break;
			}
		}
		protected virtual bool AcceptKey(Keys key) {
			var acceptableKeys = new List<Keys>() { Keys.Left, Keys.Right, Keys.Up, Keys.Down };
			return acceptableKeys.Contains(key);
		}
		bool switchScreen = false;
		protected internal void SwitchScreen() {
			if(formState == FormState.Left) {
				screenManager.NextScreen();
				currScreen = screenManager.CurrentScreen.WorkingArea;
			}
			if(formState == FormState.Right) {
				screenManager.PrevScreen();
				currScreen = screenManager.CurrentScreen.WorkingArea;
			}
			switchScreen = false;
		}
		protected internal void DoLeftDock() {
			SizableControl.OnBeginSizing();
			if(RibbonForm != null && RibbonForm.WindowState != FormWindowState.Normal)
				RibbonForm.WindowState = FormWindowState.Normal;
			SizableControl.OwnerForm.Location = new Point(currScreen.X, currScreen.Y);			
			SizableControl.OwnerForm.WindowState = FormWindowState.Normal;
			SizableControl.OwnerForm.Width = currScreen.Width / 2;
			SizableControl.OwnerForm.Height = currScreen.Height;
			SizableControl.OnEndSizing();
		}
		protected internal void DoRightDock() {
			SizableControl.OnBeginSizing();
			if(RibbonForm != null && RibbonForm.WindowState != FormWindowState.Normal)
				RibbonForm.WindowState = FormWindowState.Normal;
			SizableControl.OwnerForm.WindowState = FormWindowState.Normal;
			SizableControl.OwnerForm.Location = new Point(currScreen.Right - currScreen.Width / 2, currScreen.Y);			
			SizableControl.OwnerForm.Width = currScreen.Width / 2;
			SizableControl.OwnerForm.Height = currScreen.Height;
			SizableControl.OnEndSizing();
		}
		protected internal void Maximize() {
			if(RibbonForm == null) return;
			SizableControl.OnBeginSizing();
			RibbonForm.WindowState = FormWindowState.Maximized;
			SizableControl.OnEndSizing();
		}
		protected internal void Minimize() {
			if(RibbonForm == null) return;
			RibbonForm.WindowState = FormWindowState.Minimized;
		}
		protected internal void ResetDocking() {
			SizableControl.OnBeginSizing();
			Rectangle restoreBounds = new Rectangle();
			if(RibbonForm != null) {
				if(RibbonForm.WindowState != FormWindowState.Normal)
					RibbonForm.WindowState = FormWindowState.Normal;
				restoreBounds = RibbonForm.RestoreBounds;
			} else
				restoreBounds = SizableControl.OwnerForm.RestoreBounds;
			SizableControl.OwnerForm.WindowState = FormWindowState.Normal;
			formStateController.RestoreState(currScreen, restoreBounds);
			SizableControl.OnEndSizing();
		}
		protected ISizableControl SizableControl { get { return sizableControl; } }
		protected Form RibbonForm { get { return SizableControl.OwnerForm.Owner; } }
		protected class FormStateController {
			Form ownerForm;
			Rectangle formBounds;
			FormState formState;
			public FormStateController() {
				this.ownerForm = new Form();
				this.formBounds = new Rectangle();
			}
			public void AssignForm (Form ownerForm) {
				if(ownerForm == null) return;
				this.ownerForm = ownerForm;
			}
			public void SaveState(Rectangle currScreen, Keys key, FormState formState) {
				this.formBounds.Size = OwnerForm.Size;
				this.formBounds.Location = new Point(OwnerForm.Location.X - currScreen.X, OwnerForm.Location.Y);
				if(key != Keys.Down) {
					this.formState = formState;
				}
			}
			public void SaveTopFormBounds(Rectangle currScreen, Rectangle restoreBounds) {
				this.formBounds.Size = restoreBounds.Size;
				this.formBounds.Location = new Point(restoreBounds.Location.X - currScreen.X, restoreBounds.Location.Y);
			}
			public void RestoreState(Rectangle currScreen, Rectangle restoreBounds) {
				if(formBounds.IsEmpty)
					formBounds = restoreBounds;
				OwnerForm.Size = formBounds.Size;
				OwnerForm.Location = new Point(currScreen.Left + formBounds.Location.X, formBounds.Location.Y);
			}
			public Form OwnerForm { get { return ownerForm; } }
			public FormState CurrFormState { get { return formState; } }
		}
	}
}

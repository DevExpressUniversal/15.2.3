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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Gesture;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraBars.Navigation {
	public class AccordionControlHandler : IGestureClient {
		public AccordionControlHandler(AccordionControlViewInfo viewInfo) {
			ViewInfo = viewInfo;
		}
		protected virtual Point CalcHintPosition() {
			Point p = System.Windows.Forms.Control.MousePosition;
			if(System.Windows.Forms.Cursor.Current != null)
				p.Offset(System.Windows.Forms.Cursor.Current.Size.Width, -5);
			return p;
		}
		protected internal virtual ToolTipControlInfo GetTooltipObjectInfo() {
			if(ViewInfo.HoverInfo.IsInElement) {
				AccordionControlHintInfo hintInfo = CalcHintInfo(ViewInfo.HoverInfo);
				ToolTipControlInfo toolInfo = new ToolTipControlInfo(hintInfo, hintInfo.Show ? hintInfo.Text : string.Empty);
				toolInfo.Object = hintInfo;
				toolInfo.ForcedShow = DefaultBoolean.Default;
				toolInfo.SuperTip = ViewInfo.HoverInfo.ItemInfo.Element.SuperTip;
				return toolInfo;
			}
			return null;
		}
		protected virtual AccordionControlHintInfo CalcHintInfo(AccordionControlHitInfo hitInfo) {
			AccordionControlHintInfo hint = new AccordionControlHintInfo(AccordionControl.Appearance.Hint);
			hint.SetHint(hitInfo.ItemInfo, hitInfo.ItemInfo.Element.Hint);
			return hint;
		}
		protected internal AccordionControlViewInfo ViewInfo { get; private set; }
		protected AccordionControl AccordionControl {
			get {
				if(ViewInfo == null) return null;
				return ViewInfo.AccordionControl;
			}
		}
		public virtual void OnMouseLeave(EventArgs e) {
			if(AccordionControl.IsDesignMode)
				return;
			AccordionElementBaseViewInfo info = ViewInfo.HoverInfo.ItemInfo;
			if(HeaderControlContainsCursor(info)) return;
			ViewInfo.ResetHoverInfo();
			ViewInfo.ExpandButtonState = ObjectState.Disabled;
			if(info != null) {
				info.ContextButtonsHandler.ViewInfo = info.ContextButtonsViewInfo;
				info.ContextButtonsHandler.OnMouseLeave(e);
			}
		}
		public virtual void OnMouseEnter(EventArgs e) {
			if(AccordionControl.IsDesignMode)
				return;
			if(AccordionControl.OptionsMinimizing.AllowMinimizing) {
				ViewInfo.ExpandButtonState = ObjectState.Normal;
				AccordionControl.ControlInfo.ShowExpandButton();
			}
			AccordionElementBaseViewInfo info = ViewInfo.HoverInfo.ItemInfo;
			if(info != null) {
				info.ContextButtonsHandler.ViewInfo = info.ContextButtonsViewInfo;
				info.ContextButtonsHandler.OnMouseEnter(e);
			}
		}
		protected bool HeaderControlContainsCursor(AccordionElementBaseViewInfo info) {
			return info != null && info.Element.HeaderControl != null && info.Element.HeaderControl.Bounds.Contains(AccordionControl.PointToClient(System.Windows.Forms.Cursor.Position));
		}
		public virtual void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			AccordionControlHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(AccordionControl.IsDesignMode) {
				DragController.StartDragging(hitInfo);
				UpdateDesigner(hitInfo);
				return;
			}
			if(AccordionControl.AllowElementDragging) {
				ResetDragInfo();
				this.startDragLocation = e.Location;
			}
			AccordionElementBaseViewInfo info = hitInfo.ItemInfo;
			if(hitInfo.HitTest == AccordionControlHitTest.ExpandButton) {
				ViewInfo.ExpandButtonState = ObjectState.Pressed;
			}
			if(info != null) {
				info.ContextButtonsHandler.ViewInfo = info.ContextButtonsViewInfo;
				if(info.ContextButtonsHandler.OnMouseDown(e)) return;
			}
			ViewInfo.PressedInfo = hitInfo;
		}
		protected virtual void UpdateDesigner(AccordionControlHitInfo hitInfo) {
			if(!AccordionControl.IsDesignMode || AccordionControl.Site == null) return;
			if(hitInfo.ItemInfo == null) return;
			AccordionControlElement element = hitInfo.ItemInfo.Element;
			if(element == null) return;
			shouldProcessMouseUpDesignTime = AccordionControl.DesignManager.IsComponentSelected(element);
			AccordionControl.DesignManager.SelectComponent(element);
		}
		bool shouldProcessMouseUpDesignTime = false;
		public virtual void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
			if(ViewInfo.ExpandButtonState != ObjectState.Disabled)
				ViewInfo.ExpandButtonState = ObjectState.Normal;
			if(ViewInfo.IsInAnimation) {
				if(isInDragging)
					ForceEndDragging();
				ViewInfo.ResetPressedInfo();
				return;
			}
			if(e.Button == System.Windows.Forms.MouseButtons.Left) {
				if(ProcessEndDragging(e.Location)) return;
			}
			AccordionControlHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			AccordionElementBaseViewInfo info = hitInfo.ItemInfo;
			if(ViewInfo.PressedInfo.HitTest == AccordionControlHitTest.ExpandButton && hitInfo.HitTest == AccordionControlHitTest.ExpandButton) {
				ViewInfo.InvertAccordionExpanded();
			}
			if(info != null) {
				info.ContextButtonsHandler.ViewInfo = info.ContextButtonsViewInfo;
				if(info.ContextButtonsHandler.OnMouseUp(e)) {
					ViewInfo.ResetPressedInfo();
					return;
				}
			}
			if(ViewInfo.PressedInfo.IsInElement && ViewInfo.PressedInfo.ItemInfo.HeaderBounds.Contains(e.Location)) {
				ViewInfo.PressedInfo.ItemInfo.ProcessMouseClick();
				if(!ViewInfo.IsMinimized) ViewInfo.ResetPressedInfo();
			}
			else ViewInfo.ResetPressedInfo();
			if(AccordionControl.IsDesignMode) AccordionControl.Refresh();
			shouldProcessMouseUpDesignTime = false;
		}
		AccordionDragController dragController;
		protected internal AccordionDragController DragController {
			get {
				if(dragController == null)
					dragController = new AccordionDragController(AccordionControl);
				return dragController;
			}
		}
		public virtual void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			if(AccordionControl.IsDesignMode || (AccordionControl.AllowElementDragging)) {
				if(e.Button == System.Windows.Forms.MouseButtons.Left) {
					if(ProcessDragOver(e.Location)) return;
				}
				if(AccordionControl.IsDesignMode) return;
			}
			ViewInfo.Scrollers.VScrollBar.OnAction(DevExpress.XtraEditors.ScrollNotifyAction.MouseMove);
			if(ViewInfo.IsInAnimation) return;
			ViewInfo.HoverInfo = ViewInfo.CalcHitInfo(e.Location);
			if(ViewInfo.HoverInfo.HitTest == AccordionControlHitTest.ExpandButton) {
				ViewInfo.ExpandButtonState = ObjectState.Hot;
				return;
			}
			ViewInfo.ExpandButtonState = ObjectState.Normal;
			AccordionElementBaseViewInfo info = ViewInfo.HoverInfo.ItemInfo;
			if(info != null) {
				info.ContextButtonsHandler.ViewInfo = info.ContextButtonsViewInfo;
				info.ContextButtonsHandler.OnMouseMove(e);
			}
		}
		bool isInDragging = false;
		Point startDragLocation = Point.Empty;
		bool canStartDragging = true;
		bool ShouldStartDragging(Point location) {
			return Math.Abs(location.X - startDragLocation.X) > 20 || Math.Abs(location.Y - startDragLocation.Y) > 20;
		}
		void ResetDragInfo() {
			this.canStartDragging = true;
			this.isInDragging = false;
		}
		bool ProcessEndDragging(Point location) {
			if(!AccordionControl.IsDesignMode && !isInDragging)
				return false;
			DragController.EndDragging(location);
			if(AccordionControl.IsDesignMode)
				return !shouldProcessMouseUpDesignTime;
			ResetDragInfo();
			return true;
		}
		void ForceEndDragging() {
			DragController.ResetDropTarget();
			ResetDragInfo();
		}
		bool ProcessDragOver(Point location) {
			if(!this.canStartDragging)
				return false;
			if(DragController.DropTarget.ElementInfo != null)
				DragController.DragOver(location);
			if(AccordionControl.IsDesignMode)
				return true;
			if(!isInDragging && ShouldStartDragging(location)) {
				bool res = DragController.StartDragging(ViewInfo.PressedInfo);
				if(!res) {
					canStartDragging = false;
					return false;
				}
				isInDragging = true;
			}
			return true;
		}
		public virtual void OnMouseWheel(System.Windows.Forms.MouseEventArgs e) {
			if(ViewInfo.Scrollers != null && !ViewInfo.IsInAnimation) ViewInfo.Scrollers.OwnerMouseWheel(e);
		}
		public GestureAllowArgs[] CheckAllowGestures(Point point) {
			GestureAllowArgs p = new GestureAllowArgs() { GID = GID.PAN, AllowID = GestureHelper.GC_PAN_WITH_GUTTER | GestureHelper.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY | GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY, BlockID = GestureHelper.GC_PAN_WITH_INERTIA };
			return new GestureAllowArgs[] { p, GestureAllowArgs.PressAndTap };
		}
		public IntPtr Handle { get { return AccordionControl.IsHandleCreated ? AccordionControl.Handle : IntPtr.Zero; } }
		public void OnBegin(GestureArgs info) { }
		public void OnEnd(GestureArgs info) { }
		public void OnPressAndTap(GestureArgs info) { }
		public void OnRotate(GestureArgs info, System.Drawing.Point center, double degreeDelta) { }
		public void OnTwoFingerTap(GestureArgs info) { }
		public void OnZoom(GestureArgs info, System.Drawing.Point center, double zoomDelta) { }
		public IntPtr OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(AccordionControl); } }
		public Point PointToClient(Point p) { return AccordionControl.PointToClient(p); }
		public void OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(ViewInfo.IsInMinimizeAnimation)
				return;
			if(info.IsBegin) {
				isVerticalGesture = false;
				gestureDeltaSum = Point.Empty;
				return;
			}
			if(ProcessGestureHorizontal(delta))
				return;
			if(info.IsEnd) {
				XtraAnimator.Current.AddAnimation(new InertiaAnimationInfo(ViewInfo, ViewInfo, 1, 1000));
				return;
			}
			if(delta.Y != 0) {
				ViewInfo.GestureInertia = delta.Y;
			}
			ViewInfo.UpdateContentTop(ViewInfo.ContentTopIndent + delta.Y);
		}
		bool isVerticalGesture = false;
		Point gestureDeltaSum = Point.Empty;
		protected virtual bool ProcessGestureHorizontal(Point delta) {
			if(isVerticalGesture || !AccordionControl.OptionsMinimizing.AllowMinimizing) return false;
			this.gestureDeltaSum.X += delta.X;
			this.gestureDeltaSum.Y += delta.Y;
			if(Math.Abs(this.gestureDeltaSum.Y) > 30)
				this.isVerticalGesture = true;
			if(ShouldRunMinimizingAnimation()) {
				gestureDeltaSum.X = 0;
				ViewInfo.InvertAccordionExpanded();
				return true;
			}
			return false;
		}
		bool ShouldRunMinimizingAnimation() {
			if(Math.Abs(gestureDeltaSum.X) < 40 || isVerticalGesture)
				return false;
			bool isLeft = Math.Sign(gestureDeltaSum.X) == -1;
			if(isLeft) {
				if(!ViewInfo.IsMinimized && !IsDockRight)
					return true;
				return ViewInfo.IsMinimized && IsDockRight;
			}
			if(ViewInfo.IsMinimized && !IsDockRight)
				return true;
			return !ViewInfo.IsMinimized && IsDockRight;
		}
		bool IsDockRight {
			get {
				if(WindowsFormsSettings.RightToLeftLayout == DefaultBoolean.True)
					return AccordionControl.Dock == System.Windows.Forms.DockStyle.Left;
				return AccordionControl.Dock == System.Windows.Forms.DockStyle.Right;
			}
		}
	}
	public class AccordionControlScrollers : IDisposable {
		public AccordionControlScrollers(AccordionControlViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			CreateScrollBars();
			CheckScrollBarVisibility();
		}
		AccordionControlViewInfo viewInfo;
		internal AccordionControlViewInfo ViewInfo { get { return viewInfo; } }
		VScrollBar vScrollBar;
		internal void UpdateScrollStyles() {
			if(vScrollBar != null) vScrollBar.LookAndFeel.Assign(ViewInfo.LookAndFeel);
		}
		internal VScrollBar VScrollBar { get { return vScrollBar; } }
		public void Dispose() {
			if(vScrollBar != null) {
				vScrollBar.Dispose();
				vScrollBar = null;
			}
		}
		void CreateScrollBars() {
			vScrollBar = new VScrollBar();
			ApplyScrollBarMode();
			vScrollBar.ValueChanged += new EventHandler(OnVerticalScrollBarValueChanged);
		}
		protected internal void ApplyScrollBarMode() {
			if(ViewInfo.IsTouchScrollBarMode) {
				ScrollBarBase.ApplyUIMode(vScrollBar, ScrollUIMode.Touch);
			}
			else {
				ScrollBarBase.ApplyUIMode(vScrollBar);
			}
		}
		internal bool VScrollVisible {
			get { return vScrollBar != null && vScrollBar.ActualVisible; }
			set {
				if(vScrollBar != null) {
					vScrollBar.Parent = ViewInfo.AccordionControl;
					vScrollBar.SetVisibility(value);
				}
			}
		}
		int VScrollHeight { get { return this.ViewInfo.ContentRect.Height; } }
		int VScrollWidth { get { return (vScrollBar != null && VScrollVisible ? vScrollBar.Width : 0); } }
		protected internal void DisplayScrollBars() {
			bool prevScrollBarVisible = VScrollVisible;
			CheckScrollBarVisibility();
			UpdateScrollBarBounds();
			UpdateScrollBars();
			if(prevScrollBarVisible != VScrollVisible) ViewInfo.AccordionControl.ControlInfo.LayoutGroups();
		}
		protected void UpdateScrollBarBounds() {
			int width = VScrollBar.GetDefaultVerticalScrollBarWidth();
			int x = ViewInfo.AccordionControl.IsRightToLeft ? ViewInfo.ContentRect.X : ViewInfo.ContentRect.Right - width;
			vScrollBar.Bounds = new Rectangle(x, ViewInfo.ContentRect.Y, width, VScrollHeight);
		}
		protected void CheckScrollBarVisibility() {
			ScrollBarMode scrollMode = ViewInfo.AccordionControl.ScrollBarMode;
			if(ViewInfo.ContentRect.Size == Size.Empty || scrollMode == ScrollBarMode.Hidden || ViewInfo.IsMinimized) {
				VScrollVisible = false;
				return;
			}
			if(ViewInfo.IsTouchScrollBarMode || scrollMode == ScrollBarMode.Auto || scrollMode == ScrollBarMode.AutoCollapse) {
				VScrollVisible = ViewInfo.ContentBottom >= ViewInfo.ContentRect.Height;
				return;
			}
			VScrollVisible = true;
		}
		const int VScrollStep = 12;
		bool lockUpdateScrollBar = false;
		void UpdateScrollBars() {
			if(this.lockUpdateScrollBar) return;
			VScrollBar.BeginUpdate();
			try {
				VScrollBar.Maximum = (int)VerticalMaxScrollPosition;
				VScrollBar.LargeChange = Math.Min(ViewInfo.ContentRect.Height, ViewInfo.ContentBottom);
				if(VScrollBar.LargeChange >= VScrollBar.Maximum) {
					ResetVScrollBar();
				}
				else {
					VScrollBar.SmallChange = VScrollStep;
					VScrollBar.Value = -ViewInfo.ContentTopIndent;
				}
			}
			finally {
				VScrollBar.EndUpdate();
			}
		}
		protected void ResetVScrollBar() {
			VScrollBar.Maximum = 0;
			VScrollBar.LargeChange = 0;
			VScrollBar.SmallChange = 0;
			VScrollBar.Value = 0;
		}
		private float ConstrainValue(float value, float maxValue) {
			return Math.Min(Math.Max(0.0f, value), maxValue);
		}
		public float VerticalMaxScrollPosition {
			get {
				return Math.Max(0, -ViewInfo.ContentTopIndent + ViewInfo.ContentBottom);
			}
		}
		public bool PixelModeHorz {
			get { throw new NotImplementedException(); }
		}
		public bool PixelModeVert {
			get { throw new NotImplementedException(); }
		}
		void OnVerticalScrollBarValueChanged(object sender, EventArgs e) {
			OnScrollBarValueChangedCore(sender, e);
		}
		protected internal void LockUpdateScrollBar() {
			this.lockUpdateScrollBar = true;
		}
		protected internal void UnlockUpdateScrollBar(){
			this.lockUpdateScrollBar = false;
		}
		void OnScrollBarValueChangedCore(object sender, EventArgs e) {
			LockUpdateScrollBar();
			try {
				ViewInfo.UpdateContentTop(-VScrollBar.Value);
			}
			finally { UnlockUpdateScrollBar(); }
		}
		internal void OwnerMouseWheel(System.Windows.Forms.MouseEventArgs e) {
			VScrollBar.Value = Math.Min(VScrollBar.Value - e.Delta, VScrollBar.Maximum - VScrollBar.LargeChange);
		}
	}
}

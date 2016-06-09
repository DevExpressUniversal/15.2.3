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
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Gesture;
using System.ComponentModel.Design;
namespace DevExpress.XtraEditors {
	public class TileControlHandler : IGestureClient {
		static Point invalidHitPoint = new Point(-10000, -10000);
		public static Point InvalidHitPoint { get { return invalidHitPoint; } }
		public TileControlHandler(ITileControl control) {
			Control = control;
			HitPoint = InvalidHitPoint;
		}
		protected internal virtual void OnScrollBarValueChanged(object sender, EventArgs e) {
			OnScroll(Control.ScrollBar.Value);
		}
		public virtual void OnScroll(int value) {
			Control.Position = value;
			if(State == TileControlHandlerState.Normal && !PositionChangedByInertia) {
				StopScrollTimer();
				InertiaCalculator.Clear();
			}
			PositionChangedByInertia = false;
		}
		public ITileControl Control { get; private set; }
		TileControlHandlerState stateCore;
		public TileControlHandlerState State {
			get { return stateCore; }
			private set {
				if(State == value) return;
				stateCore = value;
				OnStateChanged();
			}
		}
		void OnStateChanged() {
			switch(State) {
				case TileControlHandlerState.Normal:
					Control.ViewInfo.UpdateVisualEffects(Utils.VisualEffects.UpdateAction.Update);
					break;
				default:
					Control.ViewInfo.UpdateVisualEffects(Utils.VisualEffects.UpdateAction.Invalidate);
					break;
			}
		}
		public Point HitPoint { get; private set; }
		public Point ContentOffset { get; private set; }
		TileControlHitInfo RightPressInfo { get; set; }
		public bool IsHorizontal {
			get { return Control.Properties.Orientation == Orientation.Horizontal; }
		}
		public bool IsVertical {
			get { return Control.Properties.Orientation == Orientation.Vertical; }
		}
		protected virtual bool ShouldProcessMouseDown(TileControlHitInfo pressedInfo) {
			if(State == TileControlHandlerState.DragMode) return false;
			if(pressedInfo.ItemInfo == null) return true;
			return pressedInfo.ItemInfo.Item.Group.Control.IsDesignMode || pressedInfo.ItemInfo.Item.Enabled;
		}
		protected bool CanProceedMouse {
			get { return TouchState == TileControlTouchState.None || TouchState == TileControlTouchState.Inactive; }
		}
		protected bool isDblClick = false;
		public virtual void OnMouseDown(MouseEventArgs e) {
			if(Control.ViewInfo.HasAnimations(TileItemAnimationType.Arrival))
				return;
			if(!CanProceedMouse) return;
			StopScrollTimer();
			InertiaCalculator.Clear();
			Control.Navigator.SetSelectedItem(null);
			Control.ViewInfo.UpdateButtonsState(e.Location, e.Button);
			if(Control.IsDesignMode) {
				OnMouseDownDesignMode(e);
				if(e.Button == MouseButtons.Right)
					return;
			}
			else Control.Focus();
			TileControlHitInfo pressedInfo = Control.ViewInfo.CalcHitInfo(e.Location);
			if(UpdateContextButtonsByMouseDown(e, pressedInfo))
				return;
			if(e.Button == MouseButtons.Right) {
				RightPressInfo = null;
				if(pressedInfo.InItem && Control.Properties.ItemCheckMode != TileItemCheckMode.None) {
					if(!ShouldProcessMouseDown(pressedInfo)) return;
					RightPressInfo = pressedInfo;
					DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = 
						CheckItemCore(pressedInfo.ItemInfo.Item);
					return;
				}
			}
			if(e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) {
				if(!ShouldProcessMouseDown(pressedInfo)) return;
				Control.ViewInfo.PressedInfo = pressedInfo;
				HitPoint = e.Location;
				if(!Control.ViewInfo.PressedInfo.InItem) {
					ScrollInfo.StartTime = DateTime.Now.Ticks;
					State = TileControlHandlerState.MouseScroll;
					return;
				}
				ContentOffset = GetDistance(pressedInfo.HitPoint, pressedInfo.ItemInfo.Bounds.Location);
			}
			if(e.Button == MouseButtons.Left && e.Clicks == 2)
				isDblClick = true;
		}
		private bool UpdateContextButtonsByMouseDown(MouseEventArgs e, TileControlHitInfo pressedInfo) {
			if(pressedInfo.ItemInfo != null) {
				return pressedInfo.ItemInfo.ContextButtonsHandler.OnMouseDown(e);
			}
			 return false;
		}
		protected virtual bool CheckItemCore(TileItem item) {
			if(!item.Enabled) return true;
			bool prev = item.Checked;
			item.Checked = !prev;
			return prev != item.Checked;
		}
		protected DateTime? DownTime { get; private set; }
		protected virtual void OnMouseDownDesignMode(MouseEventArgs e) {
			TileControlHitInfo hitInfo = Control.ViewInfo.CalcHitInfo(e.Location);
			if(hitInfo.InItem) {
				Control.ViewInfo.DesignTimeManager.SelectComponent(hitInfo.ItemInfo.Item);
				if(e.Button == MouseButtons.Right)
					ShowDesignTimeItemMenu(hitInfo);
			}
			else if(hitInfo.InGroup) {
				Control.ViewInfo.DesignTimeManager.SelectComponent(hitInfo.GroupInfo.Group);
				if(e.Button == MouseButtons.Right)
					ShowDesignTimeGroupMenu(hitInfo);
			}
			Control.Invalidate(Control.ClientRectangle);
		}
		protected virtual void ShowDesignTimeGroupMenu(TileControlHitInfo hitInfo) {
			Control.ViewInfo.DesignTimeManager.ShowGroupMenu(hitInfo);
		}
		protected virtual void ShowDesignTimeItemMenu(TileControlHitInfo hitInfo) {
			Control.ViewInfo.DesignTimeManager.ShowItemMenu(hitInfo);
		}
		protected void OnDragDrop() { OnDragDrop(false); }
		public virtual void OnMouseUp(MouseEventArgs e) {
			Control.ViewInfo.UpdateButtonsState(e.Location, e.Button);
			if(!CanProceedMouse) return;
			Control.Capture = false;
			if(State == TileControlHandlerState.DragMode) {
				OnDragDrop();
				Control.ViewInfo.SetPressedInfoCore(null);
				HitPoint = InvalidHitPoint;
				return;
			}
			if(State == TileControlHandlerState.MouseScroll) {
				State = TileControlHandlerState.Normal;
				if(Control.ViewInfo.PressedInfo.InBackArrow || Control.ViewInfo.PressedInfo.InForwardArrow) {
					Control.ViewInfo.StopScroll();					
				}
				else {
					StartMouseScrollInertia(e);
				}
			}
			else {
				TileControlHitInfo upInfo = Control.ViewInfo.CalcHitInfo(e.Location);
				if(upInfo.InItem && upInfo.ItemInfo.ContextButtonsHandler.OnMouseUp(e))
					return;
				if(Control.ViewInfo.PressedInfo.InItem && upInfo.InItem && Control.ViewInfo.PressedInfo.ItemInfo == upInfo.ItemInfo) {
					if(CanSelectItem)
						Control.SelectedItem = upInfo.ItemInfo.Item;
					ProceedPressInItem(e);
				}
				else {
					if(RightPressInfo != null && RightPressInfo.InItem && upInfo.InItem && RightPressInfo.ItemInfo == upInfo.ItemInfo)
						ProceedPressInItem(e, RightPressInfo);
				}
			}
			Control.ViewInfo.PressedInfo = TileControlHitInfo.Empty;
			RightPressInfo = null;
		}
		protected virtual bool CanSelectItem {
			get {
				return Control.Properties.AllowSelectedItem && !Control.IsDesignMode;
			}
		}
		protected virtual void ProceedPressInItem(MouseEventArgs e, TileControlHitInfo pressInfo) {
			if(pressInfo.ItemInfo == null || !pressInfo.ItemInfo.Item.Enabled) {
				isDblClick = false;
				return;
			}
			var item = pressInfo.ItemInfo.Item;
			if(e.Button == MouseButtons.Right)
				item.OnRightItemClick();
			else if(isDblClick) {
				item.OnItemPreDoubleClick();
				DoDefaultAction(item);
				isDblClick = false;
			}
			else
				item.OnItemClick();
		}
		protected virtual void ProceedPressInItem(MouseEventArgs e) {
			ProceedPressInItem(e, Control.ViewInfo.PressedInfo);
		}
		public void ResetMouseScroll() {
			if(State == TileControlHandlerState.MouseScroll) {
				State = TileControlHandlerState.Normal;
				if(Control.ViewInfo.PressedInfo.InBackArrow || Control.ViewInfo.PressedInfo.InForwardArrow) {
					Control.ViewInfo.StopScroll();
				}
			}
		}
		protected internal virtual void DoDefaultAction(TileItem item) {
			if(!Control.IsDesignMode) return;
			IDesignerHost host = Control.ViewInfo.DesignTimeManager.GetBase().DesignerHost;
			if(host != null) {
				IDesigner designer = host.GetDesigner(item) as IDesigner;
				if(designer != null) designer.DoDefaultAction();
			}
		}
		const int DecelerationRatio = 4000;
		const int InertiaScrollInterval = 10;
		const float MouseScrollActionDuration = 0.04f;
		private void StartMouseScrollInertia(MouseEventArgs e) {
			if(GetDeltaTime() > MouseScrollActionDuration) {
				Control.Invalidate(Control.ClientRectangle);
				return;
			}
			ScrollInfo.Interval = InertiaScrollInterval;
			ScrollInfo.Speed = InertiaCalculator.GetValue();
			ScrollInfo.StartTime = DateTime.Now.Ticks;
			ScrollInfo.Deceleration = -DecelerationRatio * Math.Sign(ScrollInfo.Speed);
			float sec = -ScrollInfo.Speed / ScrollInfo.Deceleration;
			ScrollInfo.EndTime = ScrollInfo.StartTime + (long)(sec * TimeSpan.TicksPerSecond);
			ScrollInfo.StartPosition = Control.Position;
			StartScrollTimer();
		}
		TileGroup GetDropGroup() {
			TileGroup dropGroup = null;
			if(Control == null || Control.ViewInfo.Calculator == null) return null;
			TileItemViewInfo itemInfo = Control.ViewInfo.Calculator.GetItemAfterDragItem();
			if(itemInfo != null)
				dropGroup = itemInfo.GroupInfo.Group;
			return dropGroup;
		}
		protected virtual bool CanDragBetweenGroupsCore {
			get {
				if(Control.IsDesignMode) return true;
				return Control.AllowDragTilesBetweenGroups;
			}
		}
		protected virtual void OnDragDrop(bool cancelDrop) {
			Control.ViewInfo.StopScroll();
			TileControlDropInfo dropInfo = Control.ViewInfo.DropInfo;
			if(dropInfo != null)
				dropInfo.DragItem = DragItem;
			DragItem.IsDragging = false;
			DragItem.ForceUseBoundsAnimation = true;
			DragItem.UseRenderImage = false;
			TileItem item = DragItem.Item;
			var targetGroup = GetTargetGroup(dropInfo);
			TileGroup newGroup = null;
			bool dropInNewGroup = dropInfo != null && dropInfo.NearestGroupInfo != null;
			if(dropInNewGroup) {
				newGroup = CreateNewGroupByDragging();
				targetGroup = newGroup;
			}
			var e = Control.RaiseEndItemDragging(item, targetGroup);
			var dropGroup = GetDropGroup();
			if(!CanDragBetweenGroupsCore && DragItem.GroupInfo != null && DragItem.Item.Group != dropGroup)
				cancelDrop = true;
			if(dropInfo == null || e.Cancel || cancelDrop) {
				UngroupGroupByDragging(newGroup);
				DropCancel();
			}
			else if(dropInNewGroup) {
				DropNewGroup(dropInfo, newGroup);
			}
			else if(dropInfo.GroupInfo != null || dropInfo.ItemInfo != null || dropInfo.NearestItemInfo != null) {
					DropItem(dropInfo);
			}
			State = TileControlHandlerState.Normal;
			Control.ViewInfo.ClearDropInfo();
			if(Control.IsDesignMode) {
				Control.ViewInfo.DesignTimeManager.FireChanged();
			}
		}
		TileGroup GetTargetGroup(TileControlDropInfo dropInfo) {
			if(dropInfo == null || dropInfo.NearestGroupInfo != null || dropInfo.DragItem == null) return null;
			if(dropInfo.GroupInfo != null)
				return dropInfo.GroupInfo.Group;
			else
				if(dropInfo.ItemInfo != null && dropInfo.ItemInfo.GroupInfo != null)
					return dropInfo.ItemInfo.GroupInfo.Group;
				return dropInfo.DragItem.GroupInfo == null ? null : dropInfo.DragItem.GroupInfo.Group;
		}
		protected virtual void DropCancel() {
			Control.BeginUpdate();
			try {
				Control.ViewInfo.PrepareItemsForExitDragMode();
				Control.ViewInfo.CacheItems();
				Control.ViewInfo.ShouldMakeTransition = true;
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected virtual void DropItem(TileControlDropInfo dropInfo) {
			TileItemViewInfo dragItem = DragItem;
			TileGroup dropGroup = null;
			Control.BeginUpdate();
			try {
				TileItemViewInfo itemInfo = Control.ViewInfo.Calculator.GetItemAfterDragItem();
				if(itemInfo != null)
					dropGroup = itemInfo.GroupInfo.Group;
				TileGroup dragGroup = dragItem.Item.Group;
				dragItem.ShouldRemoveFromCache = false;
				dragItem.ControlInfo.ShouldResetSelectedItem = false;
				dragGroup.Items.Remove(dragItem.Item);
				if(itemInfo != null) {
					InsertInGroupByDropItem(itemInfo.Item.Group, dragItem.Item, itemInfo.Item.Group.Items.IndexOf(itemInfo.Item));
				}
				else {
					TileControlDropItemInfo itemDropInfo = Control.ViewInfo.GetDropItemInfo(dropInfo);
					if(itemDropInfo == null)
						return;
					dropGroup = itemDropInfo.Group;
					AddInGroupByDropItem(itemDropInfo.Group, dragItem.Item);
				}
				if(dragGroup.Items.Count == 0) {
					UngroupGroupByDragging(dragGroup);
				}
				if(Control.IsDesignMode) {
					if(dragGroup != null) Control.ViewInfo.DesignTimeManager.ComponentChanged(dragGroup);
					if(dropGroup != null) Control.ViewInfo.DesignTimeManager.ComponentChanged(dropGroup);
				}
			}
			finally {
				Control.ViewInfo.CacheItems();
				Control.ViewInfo.ShouldMakeTransition = true;
				Control.EndUpdate();
			}
		}
		protected virtual void AddInGroupByDropItem(TileGroup group, TileItem item) {
			group.Items.Add(item);
		}
		protected virtual void InsertInGroupByDropItem(TileGroup group, TileItem item, int index) {
			group.Items.Insert(index, item);
		}
		protected virtual void DropNewGroup(TileControlDropInfo dropInfo, TileGroup newGroup) {
			if(newGroup == null) return;
			TileItemViewInfo dragItem = DragItem;
			Control.BeginUpdate();
			try {
				TileGroup group = dragItem.Item.Group;
				if(dropInfo.NearestGroupInfo != null && dropInfo.NearestGroupInfo.Group == group && group.Items.Count == 1) {
					Control.ViewInfo.ShouldMakeTransition = true;
					return;
				}
				dragItem.ShouldRemoveFromCache = false;
				group.Items.Remove(dragItem.Item);
				if(group.Items.Count == 0) {
					UngroupGroupByDragging(group);
				}
				if(Control.ViewInfo.DropTargetInfo.GroupDropSide == TileControlDropSide.Right || Control.ViewInfo.DropTargetInfo.GroupDropSide == TileControlDropSide.Bottom) {
					Control.Groups.Add(newGroup);
				}
				else {
					int groupIndex = Control.Groups.IndexOf(dropInfo.NearestGroupInfo.Group);
					Control.Groups.Insert(groupIndex, newGroup);
				}
				AddInGroupByDropItem(newGroup, dragItem.Item);
				if(Control.IsDesignMode) {
					Control.ViewInfo.DesignTimeManager.ComponentChanged(Control.Control);
				}
				Control.ViewInfo.CacheItems();
				Control.ViewInfo.ShouldMakeTransition = true;
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected virtual TileGroup CreateNewGroupByDragging() {
			TileGroup newGroup = CreateNewGroupByDraggingCore();
			if(Control.Container != null)
				Control.Container.Add(newGroup);
			else
				newGroup.Name = GenerateGroupName();
			return newGroup;
		}
		string GenerateGroupName() {
			string newName = string.Empty;
			string nameBase = "tileGroup";
			int counter = 0;
			do {
				counter++;
				newName = nameBase + counter.ToString();
			}
			while(Control.Groups[newName] != null);
			return newName;
		}
		protected virtual TileGroup CreateNewGroupByDraggingCore() {
			return new TileGroup();
		}
		protected virtual void UngroupGroupByDragging(TileGroup group) {
			if(group == null) return;
			if(Control.Groups.Contains(group))
				Control.Groups.Remove(group);
			if(Control.Container != null)
				Control.Container.Remove(group);
		}
		protected Point GetDistance(Point pt1, Point pt2) {
			return new Point(Math.Abs(pt1.X - pt2.X), Math.Abs(pt1.Y - pt2.Y));
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			if(TouchState != TileControlTouchState.Inactive)
				return;
			if(e.Button == MouseButtons.None) {
				OnMouseMoveButtonsNone(e);
				return;
			}
			if(e.Button != MouseButtons.Left)
				return;
			if(State == TileControlHandlerState.MouseScroll) {
				if(Control.Control != null)
					Control.Control.Capture = true;
				OnMouseScroll(e);
			}
			if(State == TileControlHandlerState.Normal) {
				OnMouseMoveNormal(e);
			}
			else if(State == TileControlHandlerState.DragMode) {
				OnMouseDrag(e.Location);
			}
			XtraAnimator.Current.FrameStep(this, EventArgs.Empty);
		}
		TileControlHitInfo ContextHoverInfo { get; set; }
		protected virtual void OnMouseMoveButtonsNone(MouseEventArgs e) {
			TileControlHitInfo hitInfo = Control.ViewInfo.CalcHitInfo(e.Location);
			if(Control.Properties.AllowItemHover && !Control.IsDesignMode) {
				Control.ViewInfo.HoverInfo = hitInfo;
			}
			UpdateContextButtonsByMouse(e, hitInfo);
			if(Control.ViewInfo.ScrollMode == TileControlScrollMode.ScrollButtons) {
				Control.ViewInfo.UpdateButtonsState(e.Location, e.Button);
				UpdateScrollButtons();
			}
		}
		protected virtual void UpdateContextButtonsByMouse(MouseEventArgs e, TileControlHitInfo hitInfo) {
			TileItemViewInfo prevItemInfo = ContextHoverInfo != null ? ContextHoverInfo.ItemInfo : null;
			TileItemViewInfo newItemInfo = hitInfo.ItemInfo;
			ContextHoverInfo = hitInfo;
			if(prevItemInfo != null && prevItemInfo == newItemInfo) {
				prevItemInfo.ContextButtonsHandler.OnMouseMove(e);
				return;
			}
			if(prevItemInfo != null)
				prevItemInfo.ContextButtonsHandler.OnMouseLeave(e);
			if(newItemInfo != null) {
				newItemInfo.ContextButtonsHandler.OnMouseEnter(e);
				newItemInfo.ContextButtonsHandler.OnMouseMove(e);
			}
		}
		MouseScrollInfo scrollInfo;
		protected MouseScrollInfo ScrollInfo {
			get {
				if(scrollInfo == null)
					scrollInfo = new MouseScrollInfo();
				return scrollInfo;
			}
		}
		protected internal class MouseScrollInfo {
			public float Speed { get; set; }
			public float Deceleration { get; set; }
			public float StartPosition { get; set; }
			public float Interval { get; set; }
			public long StartTime { get; set; }
			public long EndTime { get; set; }
			public float Time { get; set; }
		}
		protected Timer ScrollTimer { get; set; }
		bool PositionChangedByInertia { get; set; }
		void StartScrollTimer() {
			if(ScrollTimer == null) {
				ScrollTimer = new Timer();
				ScrollTimer.Interval = (int)ScrollInfo.Interval;
				ScrollTimer.Tick += new EventHandler(OnScrollTimerTick);
			}
			ScrollTimer.Stop();
			ScrollTimer.Start();
		}
		void OnScrollTimerTick(object sender, EventArgs e) {
			ScrollInfo.Time = (DateTime.Now.Ticks - ScrollInfo.StartTime) / (float)TimeSpan.TicksPerSecond;
			float position = ScrollInfo.StartPosition + ScrollInfo.Speed * ScrollInfo.Time + ScrollInfo.Deceleration * ScrollInfo.Time * ScrollInfo.Time / 2;
			PositionChangedByInertia = true;
			Control.Position = (int)position;
			if(DateTime.Now.Ticks >= ScrollInfo.EndTime)
				StopScrollTimer();
		}
		public void StopScrollTimer() {
			if(ScrollTimer != null)
				ScrollTimer.Stop();
		}
		protected float ScrollSpeed { get; set; }
		protected int GetScrollDelta(MouseEventArgs e) {
			return IsHorizontal ? e.Location.X - HitPoint.X : e.Location.Y - HitPoint.Y; ;
		}
		AverageValueCalculator inertiaCalculator;
		AverageValueCalculator InertiaCalculator {
			get {
				if(inertiaCalculator == null)
					inertiaCalculator = new AverageValueCalculator(5);
				return inertiaCalculator;
			}
		}
		float GetDeltaTime() {
			return (DateTime.Now.Ticks - ScrollInfo.StartTime) / (float)TimeSpan.TicksPerSecond;
		}
		protected virtual void OnMouseScroll(MouseEventArgs e) {
			int delta = GetScrollDelta(e);
			if(Control.ViewInfo.ShouldReverseScrollDueRTL) delta = -delta;
			Control.Position -= delta;
			float deltaTime = GetDeltaTime();
			if(deltaTime == 0.0f)
				return;
			float speed = -(float)delta / deltaTime;
			ScrollInfo.StartTime = DateTime.Now.Ticks;
			InertiaCalculator.AddValue(speed);
			HitPoint = e.Location;
		}
		protected void UpdateScrollButtons() {
			if(Control.ViewInfo.ScrollMode != TileControlScrollMode.ScrollButtons)
				return;
			Control.ViewInfo.UpdateScrollParamsButtons();
			Control.Invalidate(Control.ViewInfo.BackArrowBounds);
			Control.Invalidate(Control.ViewInfo.ForwardArrowBounds);
		}
		protected TileItemViewInfo DragItem { get { return Control.ViewInfo.PressedInfo.ItemInfo; } }
		protected virtual void OnMouseDrag(Point pt) {
			if(Control.ViewInfo.LeftScrollAreaBounds.Contains(pt)) {
				if(Control.Position > Control.ViewInfo.MinOffset) {
					Control.ViewInfo.StartBackScroll();
					Control.Invalidate(DragItem.Bounds);
					return;
				}
			}
			else if(Control.ViewInfo.RightScrollAreaBounds.Contains(pt)) {
				if(Control.ScrollBar != null) {
					if(Control.ScrollBar.Value < Control.ViewInfo.MaxOffset) {
						Control.ViewInfo.StartForwardScroll();
						Control.Invalidate(DragItem.Bounds);
						return;
					}
				}
			}
			else {
				Control.ViewInfo.StopScroll();
			}
			UpdateDragItemBounds(pt);
			if(!Control.ViewInfo.HasNonDragMoveAnimations) {
				Control.ViewInfo.UpdateDropItems(DragItem);
			}
			Control.Invalidate(Control.ClientRectangle);
			Control.Invalidate(DragItem.Bounds);
		}
		protected internal virtual void UpdateDragItemBounds(Point pt) {
			UpdateItemBounds(DragItem, pt);
		}
		protected internal virtual void UpdateItemBounds(TileItemViewInfo itemInfo, Point pt) {
			Point contentPoint = Control.ViewInfo.PointToContent(pt);
			itemInfo.RenderImageBounds = new Rectangle(contentPoint.X - ContentOffset.X, contentPoint.Y - ContentOffset.Y, itemInfo.RenderImageBounds.Width, itemInfo.RenderImageBounds.Height);
		}
		protected virtual Size DragSize {
			get {
				if(Control.DragSize == Size.Empty)
					return SystemInformation.DragSize;
				return Control.DragSize;
			}
		}
		protected virtual void OnMouseMoveNormal(MouseEventArgs e) {
			Point distance = GetDistance(e.Location, HitPoint);
			if(!Control.ViewInfo.AllowDragCore) {
				if(distance.X >= DragSize.Width || distance.Y >= DragSize.Height) {
					ScrollInfo.StartTime = DateTime.Now.Ticks;
					State = TileControlHandlerState.MouseScroll;
					Control.ViewInfo.PressedInfo = TileControlHitInfo.Empty;
					return;
				}
			}
			if(!Control.ViewInfo.PressedInfo.InItem || Control.ViewInfo.PressedInfo.ItemInfo.Item.Control == null ||
				!Control.ViewInfo.AllowDragCore || Control.RaiseStartItemDragging(Control.ViewInfo.PressedInfo.ItemInfo.Item).Cancel)
				return;
			if(HitPoint != InvalidHitPoint && (distance.X > DragSize.Width || distance.Y > DragSize.Height) && !Control.DebuggingState) {
				StartDragging();
			}
		}
		protected internal virtual void StartDragging() {			
			StopScrollTimer();
			Control.Capture = true;
			Control.ViewInfo.SetHoverInfoCore(null);
			Control.ViewInfo.PressedInfo.ItemInfo.IsDragging = true;
			XtraAnimator.Current.Animations.Remove(Control.AnimationHost, Control.ViewInfo.PressedInfo.ItemInfo.Item);
			State = TileControlHandlerState.DragMode;
			Control.ViewInfo.ContentLocation = Control.ViewInfo.Groups[0].TotalBounds.Location;
			Control.ViewInfo.PrepareItemsForEnterDragMode();
		}
		public virtual void OnMouseEnter(EventArgs e) { }
		public virtual void OnMouseLeave(EventArgs e) {
			if(State == TileControlHandlerState.DragMode)
				return;
			Control.ViewInfo.HoverInfo = null;
			Control.ViewInfo.PressedInfo = null;
			UpdateScrollButtons();
			State = TileControlHandlerState.Normal;
		}
		protected virtual void OnEscKeyDown() {
			if(State == TileControlHandlerState.DragMode)
				OnDragDrop(true);
			Control.ViewInfo.SetPressedInfoCore(null);
			HitPoint = InvalidHitPoint;
			Control.Navigator.SetSelectedItem(null);
		}
		protected virtual bool OnKeyDownCore(Keys keyData) {
			if(keyData == Keys.Escape) {
				OnEscKeyDown();
				return true;
			}
			if(DragItem != null)
				return false;
			switch(keyData) {
				case Keys.Up:
					Control.Navigator.MoveUp();
					return true;
				case Keys.Down:
					Control.Navigator.MoveDown();
					return true;
				case Keys.Left:
					Control.Navigator.MoveLeft();
					return true;
				case Keys.Right:
					Control.Navigator.MoveRight();
					return true;
				case Keys.PageDown:
					Control.Navigator.MovePageRight();
					return true;
				case Keys.PageUp:
					Control.Navigator.MovePageLeft();
					return true;
				case Keys.Home:
					Control.Navigator.MoveStart();
					return true;
				case Keys.End:
					Control.Navigator.MoveEnd();
					return true;
				case Keys.Enter:
				case Keys.Space:
					Control.Navigator.OnKeyClick();
					return true;
			}
			return false;
		}
		public virtual void OnKeyDown(KeyEventArgs e) {
			OnKeyDownCore(e.KeyData);
		}
		public bool ProcessCmdKey(Keys keyData) {
			return OnKeyDownCore(keyData);
		}
		#region IGestureClient Members
		protected virtual TileControlHitInfo TouchInfo { get; private set; }
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			if(TouchState != TileControlTouchState.None && TouchState != TileControlTouchState.Inactive)
				return TouchState == TileControlTouchState.Pan ? new GestureAllowArgs[] { GestureAllowArgs.Pan } : new GestureAllowArgs[] { GestureAllowArgs.Rotate };
			TouchInfo = Control.ViewInfo.CalcHitInfo(point);
			if(TouchInfo.InItem) {
				Control.ViewInfo.PressedInfo = TouchInfo;
				if(Control.ViewInfo.PressedInfo.ItemInfo == null) {
					TouchState = TileControlTouchState.Inactive;
					return new GestureAllowArgs[] { GestureAllowArgs.Rotate };
				}
				ContentOffset = GetDistance(Control.ViewInfo.PressedInfo.HitPoint, Control.ViewInfo.PressedInfo.ItemInfo.Bounds.Location);
			}
			return new GestureAllowArgs[] { GestureAllowArgs.Pan, GestureAllowArgs.PressAndTap };
		}
		IntPtr IGestureClient.Handle {
			get { return Control.IsHandleCreated ? Control.Handle : IntPtr.Zero; }
		}
		void IGestureClient.OnBegin(GestureArgs info) {
			DownTime = DateTime.Now;
			Control.ViewInfo.HoverInfo = null;
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		bool IsHorizontalMove(Point delta) {
			if(IsHorizontal)
				return Math.Abs(delta.X) > Math.Abs(delta.Y);
			return Math.Abs(delta.Y) > Math.Abs(delta.X);
		}
		bool ShouldCheckItem(Point delta) {
			if(!TouchInfo.InItem || Control.Properties.ItemCheckMode == TileItemCheckMode.None) return false;
			return !IsHorizontalMove(delta);
		}
		bool ShouldPan(Point delta) {
			return IsHorizontalMove(delta);
		}
		bool ShouldDragWithoutChecking(Point delta) {
			if(!TouchInfo.InItem || !Control.Properties.AllowDrag) return false;
			if(Control.Properties.ItemCheckMode != TileItemCheckMode.None || ShouldCheckItem(delta))
				return false;
			return !IsHorizontalMove(delta);
		}
		public enum TileControlTouchState { Inactive, None, Pan, CheckItem, Drag }
		public TileControlTouchState TouchState { get; internal set; }
		bool gestureIsEnded = false;
		int overPan = 0;
		protected internal Point TouchDragPoint = new Point(0, 0);
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPanPoint) {
			TouchDragPoint = info.Current.Point;
			XtraAnimator.Current.FrameStep(this, EventArgs.Empty);
			if(info.IsBegin) {
				gestureIsEnded = false;
				overPan = 0;
				TouchState = TileControlTouchState.None;
				if(TouchInfo == null) {
					TouchInfo = Control.ViewInfo.CalcHitInfo(info.Start.Point);
					if(TouchInfo.InItem) {
						Control.ViewInfo.PressedInfo = TouchInfo;
						ContentOffset = GetDistance(Control.ViewInfo.PressedInfo.HitPoint, Control.ViewInfo.PressedInfo.ItemInfo.Bounds.Location);
					}
				} 
				return;
			}
			if(info.IsEnd || (info.GF == GF.INERTIA && (TouchState == TileControlTouchState.Drag || TouchState == TileControlTouchState.CheckItem))) {
				gestureIsEnded = true;
				if(State == TileControlHandlerState.DragMode) {
					Control.Capture = false;
					OnDragDrop();
					Control.ViewInfo.SetPressedInfoCore(null);
				}
				else if(TouchState == TileControlTouchState.CheckItem) {
					if(TouchInfo != null && TouchInfo.ItemInfo != null) {
						if(Control.Properties.ItemCheckMode != TileItemCheckMode.None) {
							if(CheckItemCore(TouchInfo.ItemInfo.Item))
								Control.ViewInfo.AddCheckItemByTouchAnimation(TouchInfo.ItemInfo);
						}
					}
				}
				if(State == TileControlHandlerState.Normal && Control.ViewInfo.PressedInfo.InItem) {
					OnTouchReleased(info);
				}
				Control.ViewInfo.PressedInfo = null;
				TouchState = TileControlTouchState.Inactive;
				TouchInfo = null;
				return;
			}
			if(gestureIsEnded)
				return;
			if(TouchState == TileControlTouchState.None) {
				delta = new Point(info.Current.X - info.Start.X, info.Current.Y - info.Start.Y);
				if(Math.Abs(delta.X) < GetStartInteractMinValue().Width && Math.Abs(delta.Y) < GetStartInteractMinValue().Height)
					return;
				if(ShouldCheckItem(delta)) {
					TouchInfo.ItemInfo.RenderImage = Control.ViewInfo.RenderItemToBitmap(TouchInfo.ItemInfo);
					TouchInfo.ItemInfo.UseRenderImage = true;
					TouchState = TileControlTouchState.CheckItem;
				}
				else if(ShouldDragWithoutChecking(delta)) {
					TouchState = TileControlTouchState.CheckItem;
				}
				else if(ShouldPan(delta)) {
					TouchState = TileControlTouchState.Pan;
					Control.ViewInfo.PressedInfo = null;
				}
			}
			else if(TouchState == TileControlTouchState.CheckItem) {
				OnCheckItemTouch(info, delta, ref overPanPoint);
			}
			else if(TouchState == TileControlTouchState.Drag) {
				if(info.GF != GF.INERTIA) {
					OnMouseDrag(info.Current.Point);
				}
			}
			else OnTouchScroll(info, delta, ref overPanPoint);
		}
		void OnTouchReleased(GestureArgs info) {
			var touchReleaseInfo = Control.ViewInfo.CalcHitInfo(info.Current.Point);
			if(touchReleaseInfo.InItem && touchReleaseInfo.ItemInfo == Control.ViewInfo.PressedInfo.ItemInfo) {
				if(CanSelectItem)
					Control.SelectedItem = touchReleaseInfo.ItemInfo.Item;
				touchReleaseInfo.ItemInfo.Item.OnItemClick();
			}
		}
		Size GetStartInteractMinValue() {
			if(Control.DragSize.IsEmpty)
				return new Size(StartInteractionMinValue, StartInteractionMinValue);
			return new Size(DragSize.Width + StartInteractionMinValue, DragSize.Height + StartInteractionMinValue);
		}
		static int StartInteractionMinValue = 20;
		static int TouchMinDragValue = 200;
		static int TouchCheckMaxValue = 80;
		void UpdateCheckItemBounds(TileItemViewInfo itemInfo, GestureArgs info) {
			int delta = 0;
			if(IsHorizontal)
				delta = info.Current.Y - info.Start.Y;
			else
				delta = info.Current.X - info.Start.X;
			int sign = Math.Sign(delta);
			delta = (int)Math.Sqrt(Math.Abs(((double)TouchCheckMaxValue * TouchCheckMaxValue) / TouchMinDragValue * delta));
			delta *= sign;
			Point pt = info.Start.Point;
			if(IsHorizontal) {
				pt.X = itemInfo.Bounds.X + ContentOffset.X;
				pt.Y += delta;
			}
			else {
				pt.Y = itemInfo.Bounds.Y + ContentOffset.Y;
				pt.X += delta;
			}
			UpdateItemBounds(itemInfo, pt);
		}
		void OnCheckItemTouch(GestureArgs info, Point delta, ref Point overPanPoint) {
			if(TouchInfo == null || TouchInfo.ItemInfo == null) return;
			UpdateCheckItemBounds(TouchInfo.ItemInfo, info);
			Control.Invalidate(Control.ClientRectangle);
			int itemDelta = IsHorizontal ? (int)Math.Abs(info.Current.Y - TouchInfo.HitPoint.Y) : (int)Math.Abs(info.Current.X - TouchInfo.HitPoint.X);
			if((IsHorizontal && itemDelta > TouchMinDragValue) || (IsVertical && itemDelta > TouchMinDragValue)) {
				var pressedInfo = Control.ViewInfo.PressedInfo;
				if(!Control.Properties.AllowDrag || pressedInfo == null || pressedInfo.ItemInfo == null || 
					Control.RaiseStartItemDragging(pressedInfo.ItemInfo.Item).Cancel || Control.DebuggingState ||
					Control.ViewInfo.HasAnimations(TileItemAnimationType.Arrival))
					return;
				TouchState = TileControlTouchState.Drag;
				StartDragging();
			}
		}
		void OnTouchScroll(GestureArgs info, Point delta, ref Point overPanPoint) {
			int dt = IsHorizontal ? delta.X : delta.Y;
			if(dt == 0) return;
			if(Control.ViewInfo.ShouldReverseScrollDueRTL) dt = -dt;
			StopScrollTimer();
			int prevPosition = Control.Position;
			Control.Position -= dt;
			if(prevPosition == Control.Position)
				overPan += dt;
			else
				overPan = 0;
			if(IsHorizontal)
				overPanPoint.X = overPan;
			else
				overPanPoint.Y = overPan;
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
		}
		IntPtr IGestureClient.OverPanWindowHandle {
			get { return ShouldSkipOverpan(Control.Control) ? IntPtr.Zero : GestureHelper.FindOverpanWindow(Control.Control); }
		}
		Point IGestureClient.PointToClient(Point p) {
			return Control.PointToClient(p);
		}
		protected virtual bool ShouldSkipOverpan(Control control) {
			Form ownerForm = Control.Control.FindForm();
			if(ownerForm == null) return true;
			if(ownerForm.WindowState == FormWindowState.Maximized &&
				(ownerForm.FormBorderStyle == FormBorderStyle.None || ownerForm.FormBorderStyle == FormBorderStyle.FixedToolWindow))
				return true;
			return false;
		}
		#endregion
		GestureHelper gestureHelper;
		protected internal GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) {
					gestureHelper = new GestureHelper(Control.Handler);
					gestureHelper.PanWithGutter = false;
				}
				return gestureHelper;
			}
		}
		public virtual void OnMouseWheel(MouseWheelScrollClientArgs e) {
			if(ScrollTimer != null && ScrollTimer.Enabled)
				ScrollTimer.Stop();
			if(Control.ViewInfo.HasArrivalAnimation)
				return;
			int delta = -e.Delta;
			if(e.InPixels)
				delta = e.Distance;
			Control.Position += delta;
		}
	}
	public enum TileControlHandlerState { Normal, DragMode, MouseScroll }
	internal class AverageValueCalculator {
		public AverageValueCalculator(int capacity) {
			Values = new float[capacity];
			Index = 0;
		}
		public float[] Values { get; private set; }
		public int Count { get; set; }
		public int Index { get; private set; }
		public int Capacity { get { return Values.Length; } }
		public void Clear() {
			Count = 0;
			Index = 0;
		}
		public void AddValue(float value) {
			if(Count < Capacity) {
				Values[Count] = value;
				Count++;
			}
			else {
				Values[Index] = value;
				Index++;
				if(Index == Count)
					Index = 0;
			}
		}
		public float GetValue() {
			if(Count == 0)
				return 0.0f;
			float sum = 0.0f;
			for(int i = 0; i < Count; i++) {
				sum += Values[i];
			}
			return sum / Count;
		}
		public float GetLastValue(int deltaFromEnd) {
			int index = Index - deltaFromEnd < 0 ? Count + Index - deltaFromEnd : Index;
			if(index < 0)
				return float.NaN;
			return Values[index];
		}
	}
}

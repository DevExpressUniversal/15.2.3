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
using System.Collections;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraLayout.ViewInfo;
namespace DevExpress.XtraLayout.Handlers {
	public class LayoutGroupHandlerWithTabHelper : LayoutGroupHandler {
		MoveType moveType;
		InsertLocation insertLocation;
		TabbedGroup tabbedGroupinternalCore;
		int hotTabIndex;
		TabbedGroup prevTabGroup;
		public LayoutGroupHandlerWithTabHelper(BaseLayoutItem item) : base(item) { }
		protected TabbedGroup TabbedGroupinternal {
			get { return tabbedGroupinternalCore; }
			set {
				tabbedGroupinternalCore = value;
			}
		}
		internal void ResetReferences(TabbedGroup tabbedGroup) {
			if(tabbedGroupinternalCore == tabbedGroup) { tabbedGroupinternalCore = null; }
			if(prevTabGroup == tabbedGroup) { prevTabGroup = null; }
		}
		public override LayoutHandlerState State {
			get { return base.State; }
			set { base.State = value; ResetHotTracked(); }
		}
		protected internal override bool ProcessTabActions(EventType eventType, MouseEventArgs e) {
			bool processResult = false;
			TabbedGroup tGroup = null;
			if(MouseHitTest != null && MouseHitTest.Item != null && MouseHitTest.HitType == LayoutItemHitTest.Item) {
				tGroup = MouseHitTest.Item as TabbedGroup;
				if(tGroup != null) {
					if(tGroup != prevTabGroup) {
						if(TabbedGroupinternal != null) TabbedGroupinternal.ViewInfo.BorderInfo.Tab.Handler.ProcessEvent(EventType.MouseLeave, null);
						TabbedGroupinternal = tGroup;
						prevTabGroup = tGroup;
					}
					processResult = ProcessMessage(eventType, e);
				}
			} else
				ResetHotTracked();
			return processResult;
		}
		protected void ResetHotTracked() {
			if(prevTabGroup != null) {
				TabbedGroupinternal = prevTabGroup;
				TabbedGroupinternal = null;
				prevTabGroup.ViewInfo.BorderInfo.Tab.Handler.ProcessEvent(EventType.MouseLeave, null);
				prevTabGroup = null;
			}
		}
		protected internal bool ProcessMessage(EventType eventType, MouseEventArgs e) {
			Point point = e.Location;
			bool processResult = false;
			if(!TabbedGroupinternal.ViewInfo.TabsCaptionArea.Contains(point)) return false;
			if(e.Button != MouseButtons.Right) {
				EventResult res = TabbedGroupinternal.ViewInfo.BorderInfo.Tab.Handler.ProcessEvent(eventType, e);
				if(res == EventResult.Handled) {
					return true;
				}
			}
			switch(State) {
				case LayoutHandlerState.Normal:
					if(eventType == EventType.MouseDown) {
						OnMouseDown(point);
						processResult = true;
					}
					if(eventType == EventType.MouseMove) {
						OnMouseMoveNormal(eventType, e);
						processResult = ProcessNormalState(eventType, e);
					}
					break;
				case LayoutHandlerState.Sizing:
					break;
			}
			return processResult;
		}
		protected BaseLayoutItem dragItem {
			get {
				return StartHitTest.Item;
			}
		}
		protected LayoutGroup dragGroup {
			get {
				return dragItem as LayoutGroup;
			}
		}
		protected bool CalculateHitTest(Point point) {
			BaseLayoutItemHitInfo baseHi = TabbedGroupinternal.CalcHitInfo(point, false);
			TabbedGroupHitInfo hitInfo = baseHi as TabbedGroupHitInfo;
			if(hitInfo == null) return false;
			hotTabIndex = -1;
			if(hitInfo.TabbedGroupHotPageIndex >= 0) {
				hotTabIndex = hitInfo.TabbedGroupHotPageIndex;
				moveType = hitInfo.TabbedGroupMoveType;
				insertLocation = hitInfo.TabbedGroupInsertLocation;
				return true;
			}
			return false;
		}
		void UpdateActiveTabHitTest(Point point) {
			StartHitTest = new BaseLayoutItemHitInfo();
			StartHitTest.SetItem(TabbedGroupinternal.SelectedTabPage);
			StartHitTest.SetHitTestType(LayoutItemHitTest.Item);
			StartHitTest.SetHitPoint(point);
			ActivateTab(TabbedGroupinternal.TabPages.IndexOf(TabbedGroupinternal.SelectedTabPage));
			if(!Owner.Owner.OptionsFocus.AllowFocusTabbedGroups) FocusFirstControlOnTheActivePage();
		}
		protected internal void ActivateTab(TabbedGroup tg, int tabIndex) {
			TabbedGroupinternal = tg;
			ActivateTab(tabIndex);
		}
		protected internal void ActivateTab(int tabIndex) {
			if(TabbedGroupinternal == null || TabbedGroupinternal.Owner == null) return;
			if(TabbedGroupinternal.TabPages.Count > tabIndex) {
				TabbedGroup tabbedGroup = TabbedGroupinternal;
				tabbedGroup.StartChangeSelection();
				bool shouldInvalidate = false;
				if(!Owner.Owner.DesignMode && !tabbedGroup.isInSelectedTabPage) {
					tabbedGroup.Owner.FakeFocusContainer.Focus();
					shouldInvalidate = tabbedGroup.Owner.FocusHelper.SelectedComponent != tabbedGroup;
					tabbedGroup.Owner.FocusHelper.SelectedComponent = tabbedGroup;
				}
				shouldInvalidate &= tabbedGroup.SelectedTabPage == tabbedGroup.TabPages[tabIndex];
				tabbedGroup.SelectedTabPage = tabbedGroup.TabPages[tabIndex];
				if(shouldInvalidate) Owner.Invalidate();
				if(AllowChangeSelection) {
					tabbedGroup.Parent.ClearSelection();
					if(!LimitedFunctionality) tabbedGroup.SelectedTabPage.Selected = true;
				}
				tabbedGroup.EndChangeSelection();
				TabbedGroupinternal = tabbedGroup;
			} else
				throw new IndexOutOfRangeException();
		}
		protected void RemoveDragItem() {
			if(dragGroup.ParentTabbedGroup != null) {
				dragGroup.ParentTabbedGroup.RemoveTabPage(dragGroup);
			} else {
				if(dragGroup.Parent != null)
					dragGroup.Parent.Remove(dragGroup);
				else
					throw new NullReferenceException("Parent cannot be null");
			}
		}
		public void OnMouseMoveDraggingItem(Point point) {
			if(!CalculateHitTest(point)) return;
			ActivateTab(hotTabIndex);
		}
		public void OnMouseMoveNormal(EventType eventType, MouseEventArgs e) {
		}
		bool ActivateTabbedGroup(Point point) {
			if(!CalculateHitTest(point)) {
				if(Owner.Owner.EnableCustomizationMode || Owner.Owner.DesignMode) {
					TabbedGroupinternal.StartChangeSelection();
					TabbedGroupinternal.Parent.ClearSelection();
					TabbedGroupinternal.Selected = true;
					StartHitTest = new BaseLayoutItemHitInfo();
					StartHitTest.SetItem(TabbedGroupinternal);
					StartHitTest.SetHitTestType(LayoutItemHitTest.Item);
					StartHitTest.SetHitPoint(point);
					TabbedGroupinternal.EndChangeSelection();
				}
				return true;
			}
			return false;
		}
		public void OnMouseDown(Point point) {
			if(ActivateTabbedGroup(point)) {
				return;
			}
			UpdateActiveTabHitTest(point);
		}
		void FocusFirstControlOnTheActivePage() {
			Owner.Owner.FocusHelper.FocusFirstInGroup(TabbedGroupinternal.SelectedTabPage, true);
		}
	}
	public class TabbedConstraintsHelper {
		public BaseLayoutItem minWItem;
		public BaseLayoutItem minHItem;
		public TabbedConstraintsHelper() { }
		public Size CalculateMinSize(BaseItemCollection collection) {
			int MinH = int.MinValue;
			int MinW = int.MinValue;
			foreach(BaseLayoutItem item in collection) {
				if(item.MinSize.Height > MinH) {
					MinH = item.MinSize.Height;
					minHItem = item;
				}
				if(item.MinSize.Width > MinW) {
					MinW = item.MinSize.Width;
					minWItem = item;
				}
			}
			if(collection.Count == 0) return Size.Empty;
			return new Size(MinW, MinH);
		}
		public static Size CalculateMaxSize(BaseItemCollection collection) {
			int MaxH = int.MaxValue;
			int MaxW = int.MaxValue;
			foreach(BaseLayoutItem item in collection) {
				if(item.MaxSize.Height < MaxH) {
					MaxH = item.MaxSize.Height;
				}
				if(item.MaxSize.Width < MaxW) {
					MaxW = item.MaxSize.Width;
				}
			}
			if(collection.Count == 0) return Size.Empty;
			return new Size(MaxW, MaxH);
		}
	}
	public class HotTrackHelper {
		LayoutGroupHandler handlerCore;
		protected BaseLayoutItem hotTrackedItemCore = null;
		protected BaseLayoutItem pressedItemCore = null;
		ObjectState activeGroupExpandButtonState = ObjectState.Normal;
		BaseLayoutItem activeGroupWithExpandButton = null;
		int activeGroupWithExpandButtonIndex = -1;
		BaseLayoutItem prevActiveGroupWithExpandButton = null;
		ObjectState prevActiveGroupExpandButtonState = ObjectState.Normal;
		public HotTrackHelper(LayoutGroupHandler handler) {
			handlerCore = handler;
		}
		public void Reset() {
			HotTrackedItem = null;
			PressedItem = null;
			UpdateExpandButtonState();
		}
		public void ProcessMessage(EventType eventType, MouseEventArgs e) {
			if(handlerCore.MouseHitTest == null) {
				HotTrackedItem = null; PressedItem = null;
			} else {
				UpdateExpandButtonState();
				if(handlerCore.State == LayoutHandlerState.Sizing && e.Button == MouseButtons.Left && PressedItem == null && handlerCore.MouseHitTest != null) { PressedItem = handlerCore.MouseHitTest.Item; HotTrackedItem = null; };
				if(handlerCore.State != LayoutHandlerState.Sizing) { PressedItem = null; }
				if(handlerCore.State == LayoutHandlerState.Normal && PressedItem == null && handlerCore.MouseHitTest != null) HotTrackedItem = handlerCore.MouseHitTest.Item;
				if(eventType == EventType.MouseLeave || eventType == EventType.MouseUp)
					UnCaptureMouseEvents();
			}
		}
		protected void CaptureMouseEvents() {
			if(handlerCore != null && handlerCore.Owner != null && handlerCore.Owner.Owner != null)
				handlerCore.Owner.Owner.Control.Capture = true;
		}
		protected void UnCaptureMouseEvents() {
			if(handlerCore != null && handlerCore.Owner != null && handlerCore.Owner.Owner != null)
				handlerCore.Owner.Owner.Control.Capture = false;
		}
		protected void UpdateExpandButtonState() {
			if(prevActiveGroupWithExpandButton != null && prevActiveGroupWithExpandButton.IsDisposing) {
				prevActiveGroupWithExpandButton = null;
				return;
			}
			if(activeGroupWithExpandButton != null && activeGroupWithExpandButton.IsDisposing) {
				activeGroupWithExpandButton = null;
				activeGroupWithExpandButtonIndex = -1;
				return;
			}
			if(handlerCore.MouseHitTest != null && 
				(handlerCore.MouseHitTest.Item is LayoutGroup || 
				(handlerCore.MouseHitTest.Item is DashboardLayoutControlItemBase))) {
				prevActiveGroupExpandButtonState = activeGroupExpandButtonState;
				prevActiveGroupWithExpandButton = activeGroupWithExpandButton;
				activeGroupWithExpandButton = handlerCore.MouseHitTest.Item;
				activeGroupExpandButtonState = ObjectState.Normal;
				if(handlerCore.MouseHitTest.IsExpandButton) activeGroupExpandButtonState = ObjectState.Hot;
				else activeGroupExpandButtonState = ObjectState.Normal;
				DashboardHitInfo dhi = handlerCore.MouseHitTest as DashboardHitInfo;
				if(dhi != null && dhi.ButtonIndex >= 0)
					activeGroupWithExpandButtonIndex = dhi.ButtonIndex;
				if(handlerCore.MouseHitTest.IsExpandButton && Control.MouseButtons == MouseButtons.Left) {
					activeGroupExpandButtonState = ObjectState.Pressed;
					CaptureMouseEvents();
				}
			} else {
				prevActiveGroupExpandButtonState = ObjectState.Normal;
				activeGroupExpandButtonState = ObjectState.Normal;
				activeGroupWithExpandButtonIndex = -1;
				prevActiveGroupWithExpandButton = activeGroupWithExpandButton;
				activeGroupWithExpandButton = null;
			}
			if(prevActiveGroupExpandButtonState != activeGroupExpandButtonState || prevActiveGroupWithExpandButton != activeGroupWithExpandButton) {
				if(activeGroupWithExpandButton != null && !activeGroupWithExpandButton.IsDisposing) activeGroupWithExpandButton.ViewInfo.UpdateState();
				else activeGroupWithExpandButton = null;
				if(prevActiveGroupWithExpandButton != null && !prevActiveGroupWithExpandButton.IsDisposing) prevActiveGroupWithExpandButton.ViewInfo.UpdateState();
				else prevActiveGroupWithExpandButton = null;
				if(AllowTrackedPressedUpdate(activeGroupWithExpandButton, prevActiveGroupWithExpandButton)) {
					Rectangle prevActiveGroupCaption = CanGetExpandButtonRect(prevActiveGroupWithExpandButton) ?
						((GroupObjectInfoArgs)prevActiveGroupWithExpandButton.ViewInfo.BorderInfo).CaptionBounds : Rectangle.Empty;
					Rectangle activeGroupCaption = CanGetExpandButtonRect(activeGroupWithExpandButton) ?
						((GroupObjectInfoArgs)activeGroupWithExpandButton.ViewInfo.BorderInfo).CaptionBounds : Rectangle.Empty;
					HotTrackedPressedChangedUpdate(prevActiveGroupCaption, activeGroupCaption);
				}
				prevActiveGroupExpandButtonState = ObjectState.Normal;
				prevActiveGroupWithExpandButton = null;
			}
		}
		protected bool CanGetExpandButtonRect(BaseLayoutItem bli) {
			return bli != null;
		}
		bool CheckMousePosition(BaseLayoutItem item) {
			if(item.Owner == null) return false;
			if(handlerCore == null) return false;
			if(handlerCore.Owner == null) return false;
			if(handlerCore.Owner.Owner == null) return false;
			if(handlerCore.Owner.Owner.RootGroup == null) return false;
			if(Control.MouseButtons == MouseButtons.Left) return false;
			if(item is LayoutItemContainer) return false;
			LayoutControlItem cItem = item as LayoutControlItem;
			if(cItem != null && !cItem.AllowHotTrack && (handlerCore.Owner.Owner.OptionsView.DrawAdornerLayer != DefaultBoolean.True || handlerCore.Owner.Owner.DesignMode)) return false;
			if(handlerCore != null && handlerCore.MouseHitTest != null) return handlerCore.MouseHitTest.Item == item;
			Point cPos = item.Owner.Control.PointToClient(Cursor.Position);
			BaseLayoutItemHitInfo hi = handlerCore.Owner.Owner.RootGroup.CalcHitInfo(cPos, false);
			return hi.Item == item;
		}
		public virtual ObjectState GetState(BaseLayoutItem item, int index) {
			if(activeGroupWithExpandButton == item && activeGroupWithExpandButtonIndex == index)
				return activeGroupExpandButtonState;
			else
				return ObjectState.Normal;
		}
		public virtual ObjectState GetState(BaseLayoutItem item, bool returnExpandButtonState) {
			if(returnExpandButtonState) {
				if(activeGroupWithExpandButton == item)
					return activeGroupExpandButtonState;
				else
					return ObjectState.Normal;
			}
			if(item == PressedItem)
				return ObjectState.Pressed;
			if(CheckMousePosition(item))
				return ObjectState.Hot;
			return ObjectState.Normal;
		}
		protected void HotTrackedPressedChangedUpdate(Rectangle oldRegion, Rectangle newRegion) {
			if(!oldRegion.IsEmpty) {
				oldRegion.Inflate(1, 1);
				handlerCore.Owner.Owner.Control.Invalidate(oldRegion);
			}
			if(!newRegion.IsEmpty) {
				newRegion.Inflate(1, 1);
				handlerCore.Owner.Owner.Control.Invalidate(newRegion);
			}
		}
		protected bool AllowTrackedPressedUpdate(BaseLayoutItem oldItem, BaseLayoutItem newItem) {
			if(prevActiveGroupExpandButtonState != activeGroupExpandButtonState) return true;
			if(oldItem == newItem) return false;
			if(handlerCore.Owner == null) return false;
			if(handlerCore.Owner.Owner == null) return false;
			if(handlerCore.Owner.Owner.EnableCustomizationMode && (handlerCore.Owner.Owner.OptionsView.DrawAdornerLayer != DefaultBoolean.True || handlerCore.Owner.Owner.DesignMode)) return false;
			return true;
		}
		public BaseLayoutItem PressedItem {
			get { return pressedItemCore; }
			set {
				if(pressedItemCore != value) {
					BaseLayoutItem oldpressedItemCore = pressedItemCore;
					pressedItemCore = value;
					if(handlerCore.Owner != null)
						if(AllowTrackedPressedUpdate(oldpressedItemCore, pressedItemCore)) {
							HotTrackedPressedChangedUpdateCore(oldpressedItemCore, pressedItemCore);
						}
				}
			}
		}
		public BaseLayoutItem HotTrackedItem {
			get { return hotTrackedItemCore; }
			set {
				if(hotTrackedItemCore != value) {
					BaseLayoutItem oldhotTrackedItemCore = hotTrackedItemCore;
					hotTrackedItemCore = value;
					if(hotTrackedItemCore != null) XtraAnimator.Current.AddAnimation(new OpacityAnimationInfo(hotTrackedItemCore.ViewInfo));
					if(handlerCore.Owner != null)
						if((handlerCore.Owner.Owner.OptionsView.AllowHotTrack | handlerCore.Owner.Owner.DesignMode | handlerCore.Owner.Owner.EnableCustomizationMode | hotTrackedItemCore is SplitterItem | oldhotTrackedItemCore is SplitterItem) &
							AllowTrackedPressedUpdate(oldhotTrackedItemCore, hotTrackedItemCore)) {
							HotTrackedPressedChangedUpdateCore(oldhotTrackedItemCore, hotTrackedItemCore);
						}
				}
			}
		}
		protected Rectangle HotTrackedPressedChangedUpdateGetBounds(BaseLayoutItem item) {
			if(item == null || item.IsDisposing) return Rectangle.Empty;
			Rectangle bounds = item.ViewInfo.BoundsRelativeToControl;
			if(item is EmptySpaceItem && item.Parent != null && !(item is SplitterItem)) return item.Parent.ViewInfo.BoundsRelativeToControl;
			return bounds;
		}
		Rectangle HotTrackedPressedChangedUpdateGetBoundsCore(BaseLayoutItem item) {
			if(item == null || item.IsDisposing) return Rectangle.Empty;
			return item.ViewInfo.BoundsRelativeToControl;
		}
		protected void HotTrackedPressedChangedUpdateCore(BaseLayoutItem item1, BaseLayoutItem item2) {
			HotTrackedPressedChangedUpdate(
				HotTrackedPressedChangedUpdateGetBounds(item1),
				HotTrackedPressedChangedUpdateGetBounds(item2));
		}
	}
	public class LayoutGroupHandler : BaseLayoutItemHandler {
		bool fAllowProcessHotTracking = true;
		Size sizingStartGroupSize;
		Size startSize;
		Point startLocation;
		HotTrackHelper hotTrackHelper;
		BaseLayoutItemHitInfo startHitTest;
		Bitmap draggedItemImage = null;
		public LayoutGroupHandler(BaseLayoutItem item) : base(item) { }
		public override void ResetHandler() {
			base.ResetHandler();
			startHitTest = null;
			HotTrackHelper.Reset();
		}
		public bool AllowProcessHotTracking {
			get {
				if(Owner.Owner != null) return Owner.Owner.OptionsView.AllowHotTrack;
				return fAllowProcessHotTracking;
			}
			set { fAllowProcessHotTracking = value; }
		}
		public Bitmap DraggedItemImage {
			get { return draggedItemImage; }
		}
		public HotTrackHelper HotTrackHelper {
			get {
				if(hotTrackHelper == null) hotTrackHelper = new HotTrackHelper(this);
				return hotTrackHelper;
			}
		}
		public virtual bool CheckCustomizationConstraints(LayoutItemDragController controller) { return true; }
		protected internal override bool PerformControlActions(EventType eventType, MouseEventArgs e) {
			bool result = base.PerformControlActions(eventType, e);
			ResetSplitterHotTrack();
			if(AllowProcessHotTracking || CheckSplitterHotTrack() || CheckButtonHotTrack() || (Owner != null && Owner.Owner != null && Owner.Owner.OptionsView.DrawAdornerLayer == DefaultBoolean.True && Owner.Owner.EnableCustomizationMode && !Owner.Owner.DesignMode)) {
				HotTrackHelper.ProcessMessage(eventType, e);
			} else HotTrackHelper.Reset();
			return result;
		}
		bool CheckSplitterHotTrack() {
			return (MouseHitTest != null && MouseHitTest.Item is SplitterItem);
		}
		bool CheckButtonHotTrack() {
			return (MouseHitTest != null && MouseHitTest.IsExpandButton);
		}
		private void ResetSplitterHotTrack() {
			if(!AllowProcessHotTracking && HotTrackHelper.HotTrackedItem is SplitterItem) {
				HotTrackHelper.HotTrackedItem = null;
			}
		}
		protected internal BaseLayoutItemHitInfo StartHitTest {
			get { return startHitTest; }
			set { startHitTest = value; }
		}
		protected override BaseLayoutItemHitInfo CreateHitTest(int X, int Y) {
			bool shouldCalcSizing = true;
			if(Owner != null && Owner.Owner != null && !(Owner.Owner.EnableCustomizationMode | Owner.Owner.DesignMode)) shouldCalcSizing = false;
			return ((LayoutGroup)Owner).CalcHitInfo(new Point(X, Y), shouldCalcSizing, true);
		}
		protected internal virtual bool ProcessTabActions(EventType eventType, MouseEventArgs e) {
			return false;
		}
		public override string ToString() {
			string val =
				Environment.NewLine + "base:" + base.ToString() +
				Environment.NewLine + " state " + State.ToString() +
				Environment.NewLine + " mouseHitTestInternal " + (mouseHitTestInternal != null ? "{" + mouseHitTestInternal.ToString() + "}" : "") +
				Environment.NewLine + " startsize " + startSize.ToString() +
				Environment.NewLine + " startHitTest " + (startHitTest != null ? "{" + startHitTest.ToString() + "}" : "") +
				Environment.NewLine + " sizingStartGroupSize " + sizingStartGroupSize.ToString();
			return val;
		}
		protected LayoutGroup Group { get { return (LayoutGroup)Owner; } }
		protected LayoutGroup Root { get { return (LayoutGroup)Owner; } }
		protected Point GroupLocation { get { return ((LayoutGroup)Owner).Location; } }
		protected virtual bool CanStartSplitterResizing() {
			if(MouseHitTest == null) return false;
			SplitterItem si = MouseHitTest.Item as SplitterItem;
			if(si == null) return false;
			if(LimitedFunctionality) return true;
			return si.GetSizingRect().Contains(MouseHitTest.HitPoint);
		}
		protected BaseLayoutItemHitInfo GetItemBottomNeighborSplitter() {
			if(MouseHitTest.Item is SplitterItem) return null;
			if(!MouseHitTest.IsSizing || MouseHitTest.HitType!=LayoutItemHitTest.VSizing) return null;
			BaseLayoutItemHitInfo neighborHitInfo = Root.CalcHitInfo(new Point(MouseHitTest.HitPoint.X, MouseHitTest.Item.ViewInfo.BoundsRelativeToControl.Bottom + 1), true);
			if(!(neighborHitInfo.Item is SplitterItem)) return null;
			if(((SplitterItem)neighborHitInfo.Item).IsVertical) return null;
			if(neighborHitInfo.HitType != MouseHitTest.HitType) return null;
			return neighborHitInfo;
		}
		protected bool ProcessNormalState(EventType eventType, MouseEventArgs e) {
			if(e != null) {
				if(e.Button == MouseButtons.Left && eventType == EventType.Click) {
					if(MouseHitTest!=null && MouseHitTest.HitType == LayoutItemHitTest.TextArea) {
						LayoutControlItem cItem = MouseHitTest.Item as LayoutControlItem;
						if(cItem != null && Owner != null && Owner.Owner.OptionsFocus.AllowFocusControlOnLabelClick)
							Owner.Owner.FocusHelper.FocusElement(cItem, true);
					}
				}
				if(e.Button == MouseButtons.Left && eventType == EventType.MouseUp) {
					ResetHandler();
					return true;
				}
				if(mouseHitTestInternal == null) return false; 
				if(e.Button == MouseButtons.Left && eventType == EventType.MouseDown && (mouseHitTestInternal.IsSizing || (MouseHitTest.Item is SplitterItem && CanStartSplitterResizing()))) {
					if(GetItemBottomNeighborSplitter() != null) MouseHitTest = new BaseLayoutItemHitInfo(GetItemBottomNeighborSplitter());
					if(PrepareForSizing()) return true;
				}
				if(mouseHitTestInternal == null) return false;
				if(e.Button == MouseButtons.Left && eventType == EventType.MouseDown && !mouseHitTestInternal.IsSizing) {
					if(PrepareForDragging()) return false;
				}
				if(e.Button == MouseButtons.Left && eventType == EventType.MouseMove && mouseHitTestInternal.Item != null) {
					return ProcessMoving(e);
				}
				if(e.Button == MouseButtons.Left && MouseHitTest.Item != null && eventType == EventType.MouseDown) {
					if(PerformSelection()) return true;
				}
			}
			return false;
		}
		protected override bool PerformSelection() {
			BaseLayoutItem item = MouseHitTest.Item;
			bool oldSelected = item.Selected;
			bool wasSelectionModified = false;
			bool newSelected = false;
			try {
				item.StartChangeSelection();
				if(LimitedFunctionality) return true;
				if(!IsControlPressed && !IsShiftPressed) {
					item.Selected = false;
					wasSelectionModified = ((LayoutGroup)Owner).ClearSelection();
				}
				item.Selected = !item.Selected;
				newSelected = item.Selected;
			} finally {
				if(oldSelected != newSelected || wasSelectionModified) {
					item.EndChangeSelection();
				} else
					item.CancelChangeSelection();
			}
			return true;
		}
		protected bool ProcessSizingState(EventType eventType, MouseEventArgs e) {
			if(e != null) {
				if(e.Button != MouseButtons.Left) {
					EndSizing();
					return true;
				}
				if(e.Button == MouseButtons.Left && eventType == EventType.MouseUp && State == LayoutHandlerState.Sizing) {
					if(EndSizing()) return true;
				}
				if(e.Button == MouseButtons.Left && eventType == EventType.MouseMove ) {
					OnMouseMoveSizing(e); return true;
				}
			}
			return false;
		}
		protected internal bool ProcessDashboardItemActions(EventType eventType, MouseEventArgs e) {
			DashboardHitInfo hitInfo = mouseHitTestInternal as DashboardHitInfo;
			if(e == null || hitInfo == null) return false;
			bool result = false;
			if(hitInfo.ButtonIndex == -1) {
				DashboardLayoutControlItemBase item = hitInfo.Item as DashboardLayoutControlItemBase;
				if(item != null) {
					(item.ViewInfo.BorderInfo as GroupObjectInfoArgs).ButtonsPanel.Handler.Reset();
				}
				DashboardLayoutControlGroupBase group = hitInfo.Item as DashboardLayoutControlGroupBase;
				if(group != null) group.ViewInfo.BorderInfo.ButtonsPanel.Handler.Reset();
				result = true;
			}
			if(hitInfo.ButtonIndex >= 0) {
				if(eventType == EventType.Click && e.Button == MouseButtons.Left) {
					DashboardLayoutControlItemBase item = hitInfo.Item as DashboardLayoutControlItemBase;
					if(item != null) RaiseButtonMouseClick(item, hitInfo.ButtonIndex);
					DashboardLayoutControlGroupBase group = hitInfo.Item as DashboardLayoutControlGroupBase;
					if(group != null) RaiseButtonMouseClick(group, hitInfo.ButtonIndex);
				}
				if(eventType == EventType.MouseMove) {
					DashboardLayoutControlItemBase item = hitInfo.Item as DashboardLayoutControlItemBase;
					if(item != null) (item.ViewInfo.BorderInfo as GroupObjectInfoArgs).ButtonsPanel.Handler.OnMouseMove(e);
					DashboardLayoutControlGroupBase group = hitInfo.Item as DashboardLayoutControlGroupBase;
					if(group != null) group.ViewInfo.BorderInfo.ButtonsPanel.Handler.OnMouseMove(e);
				}
				if(eventType == EventType.MouseDown && e.Button == MouseButtons.Left) {
					DashboardLayoutControlItemBase item = hitInfo.Item as DashboardLayoutControlItemBase;
					if(item != null) (item.ViewInfo.BorderInfo as GroupObjectInfoArgs).ButtonsPanel.Handler.OnMouseDown(e);
					DashboardLayoutControlGroupBase group = hitInfo.Item as DashboardLayoutControlGroupBase;
					if(group != null) group.ViewInfo.BorderInfo.ButtonsPanel.Handler.OnMouseDown(e);
				}
				if(eventType == EventType.MouseUp && e.Button == MouseButtons.Left) {
					DashboardLayoutControlItemBase item = hitInfo.Item as DashboardLayoutControlItemBase;
					if(item != null) (item.ViewInfo.BorderInfo as GroupObjectInfoArgs).ButtonsPanel.Handler.OnMouseUp(e);
					DashboardLayoutControlGroupBase group = hitInfo.Item as DashboardLayoutControlGroupBase;
					if(group != null) group.ViewInfo.BorderInfo.ButtonsPanel.Handler.OnMouseUp(e);
				}
			}
			return result;
		}
		protected void RaiseButtonMouseClick(DashboardLayoutControlItemBase item, int index) {
			if(Owner != null && Owner.Owner != null && Owner.Owner.FocusHelper != null) Owner.Owner.FocusHelper.FocusElement(item, false);
			item.RaiseButtonMouseClick(index);
			ResetHandler();
		}
		protected void RaiseButtonMouseClick(DashboardLayoutControlGroupBase group, int index) {
			if(Owner != null && Owner.Owner != null && Owner.Owner.FocusHelper != null) Owner.Owner.FocusHelper.FocusElement(group, false);
			group.RaiseButtonMouseClick(index);
			ResetHandler();
		}
		protected internal bool ProcessGroupActions(EventType eventType, MouseEventArgs e) {												
			LayoutGroupHitInfo hitInfo = mouseHitTestInternal as LayoutGroupHitInfo;
			if(e == null || hitInfo == null) return false;
			if(DoExpandGroupOnDoubleClick(eventType, hitInfo)) return true;
			bool result = false;
			if(hitInfo.AdditionalHitType != LayoutGroupHitTypes.ExpandedButton) {
				LayoutGroup group = hitInfo.Item as LayoutGroup;
				if(group != null && group.ButtonsPanel != null) group.ButtonsPanel.Handler.OnMouseLeave();
				Owner.Owner.Control.Invalidate();
				return result;
			}
			if(hitInfo.Item is LayoutGroup && !result && !(mouseHitTestInternal is DashboardHitInfo)) {
				LayoutGroup group = hitInfo.Item as LayoutGroup;
				if(group.ViewInfo == null || group.ButtonsPanel == null) return result;
				switch(eventType) {
					case EventType.MouseDown:
						if(baseButtonInfo != null && e.Button == MouseButtons.Left && group.ButtonsPanel.Buttons.IndexOf(baseButtonInfo.Button) == 0) {
						   DoFocusGroupIDesignMode(group);
						   DoInvertGroupExpanded(group);
						   result = true;
					   } else {
						   group.ButtonsPanel.Handler.OnMouseDown(e);
					   }
						break;
					case EventType.MouseMove:
						group.ButtonsPanel.Handler.OnMouseMove(e);
							break;
					case EventType.MouseUp:
						group.ButtonsPanel.Handler.OnMouseUp(e);
						break;
				}
			}
			Owner.Owner.Control.Invalidate();
			return result;
		}
		protected void DoFocusGroupIDesignMode(LayoutGroup group) {
			if(Owner.Owner.DesignMode) {
				this.Owner.Owner.FakeFocusContainer.Focus();
				this.Owner.Owner.FocusHelper.SelectedComponent = group;
			}
		}
		protected virtual bool DoExpandGroupOnDoubleClick(EventType eventType, LayoutGroupHitInfo hitInfo) {
			if(hitInfo.HitType != LayoutItemHitTest.Item || eventType != EventType.DoubleClick || Owner.Owner.DesignMode) return false;
			bool result = false;
			LayoutGroup group = hitInfo.Item as LayoutGroup;
			if(group != null && group.ExpandOnDoubleClick) {
				if(hitInfo.AdditionalHitType != LayoutGroupHitTypes.ExpandedButton) {
					DoInvertGroupExpanded(group);
					result = true;
				}
			}
			return result;
		}
		protected void DoInvertGroupExpanded(LayoutGroup group) {
			if(Owner != null && Owner.Owner != null && Owner.Owner.FocusHelper != null) Owner.Owner.FocusHelper.FocusElement(group, false);
			group.Expanded = !group.Expanded;
			ResetHandler();
		}
		protected virtual void RaiseMouseEvents(EventType eventType, MouseEventArgs e) {
			if(MouseHitTest.Item != null) {
				switch(eventType) {
					case EventType.MouseDown: MouseHitTest.Item.RaiseMouseDown(e); break;
					case EventType.MouseUp: MouseHitTest.Item.RaiseMouseUp(e); break;
					case EventType.Click: MouseHitTest.Item.RaiseClick(e); break;
					case EventType.DoubleClick: MouseHitTest.Item.RaiseDoubleClick(e); break;
				}
			}
		}
		BaseButtonInfo baseButtonInfo;
		protected internal override bool PerformOwnersActions(EventType eventType, MouseEventArgs e) {
			mouseHitTestInternal = CreateHitTest(e.X, e.Y);
			baseButtonInfo = CalcBaseButtonInfo(e);
			RaiseMouseEvents(eventType, e);
			if(ProcessDashboardItemActions(eventType, e)) return true;
			if(ProcessTabActions(eventType, e)) return true;
			if(ProcessGroupActions(eventType, e)) return true;
			if(base.PerformOwnersActions(eventType, e)) return true;
			switch(State) {
				case LayoutHandlerState.Normal:
					return ProcessNormalState(eventType, e);
				case LayoutHandlerState.Sizing:
					return ProcessSizingState(eventType, e);
				default:
					return false;
			}
		}
		private BaseButtonInfo CalcBaseButtonInfo(MouseEventArgs e) {
			if(mouseHitTestInternal.Item is LayoutGroup) {
				LayoutGroup group = mouseHitTestInternal.Item as LayoutGroup;
				if(group != null && group.ViewInfo != null && group.ButtonsPanel != null && group.ButtonsPanel.ViewInfo != null) {
					return group.ButtonsPanel.ViewInfo.CalcHitInfo(e.Location);
				}
			}
			return null;
		}
		protected internal override bool PerformChildHandlerActions(EventType eventType, MouseEventArgs e) {
			return false;
		}
		protected bool PrepareForDragging() {
			startHitTest = MouseHitTest;
			return false;
		}
		protected bool PrepareForSizing() {
			LayoutHandlerState oldState = State;
			State = LayoutHandlerState.Sizing;
			wasMouseMoveSizing = false;
			if(oldState == State) return false;
			startSize = this.mouseHitTestInternal.Item.Size;
			startLocation = this.mouseHitTestInternal.Item.Location;
			startHitTest = MouseHitTest;
			sizingStartGroupSize = Root.Size;
			if(Owner.Owner != null && Owner.Owner.OptionsView.DrawAdornerLayer == DefaultBoolean.True && !Owner.Owner.DesignMode) {
				SetSizingTypeForAdorner();
				(Owner.Owner as LayoutControl).InvalidateAdornerWindowHandler();
			}
			return true;
		}
		protected bool ProcessMoving(MouseEventArgs e) {
			return false;
		}
		protected bool EndSizing() {
			if(startHitTest.Item != null && startHitTest.Item.Owner != null && wasMouseMoveSizing)
				startHitTest.Item.Owner.RootGroup.ResetResizerProportions();
			wasMouseMoveSizing = false;
			State = LayoutHandlerState.Normal;
			if(Owner.Owner != null && (Owner.Owner is LayoutControl))
				(Owner.Owner as LayoutControl).InvalidateAdornerWindowHandler();
			return true;
		}
		bool wasMouseMoveSizing = false;
		System.Collections.Generic.List<int> itemsPosition = new System.Collections.Generic.List<int>();
		protected void OnMouseMoveSizing(MouseEventArgs e) {
			if(wasMouseMoveSizing == false) {
				itemsPosition.Clear();
				foreach(BaseLayoutItem bli in startHitTest.Item.Parent.Items) {
					itemsPosition.Add(startHitTest.HitType == LayoutItemHitTest.HSizing ? bli.ViewInfo.BoundsRelativeToControl.X : bli.ViewInfo.BoundsRelativeToControl.Y);
				}
				itemsPosition.Remove(startHitTest.HitType == LayoutItemHitTest.HSizing ? startHitTest.Item.ViewInfo.BoundsRelativeToControl.X : startHitTest.Item.ViewInfo.BoundsRelativeToControl.Y);
			}
			wasMouseMoveSizing = true;
			if(NotSizingItem()) return;
			if(e != null) {
				if(startHitTest.Item is SplitterItem) {
					SplitterItem splitter = startHitTest.Item as SplitterItem;
					Point location = startLocation;
					if(!splitter.IsHorizontal)
						location.X += e.X - startHitTest.HitPoint.X; 
					else location.Y += e.Y - startHitTest.HitPoint.Y; 
					if(startHitTest.Item.Location != location) {
						if(Owner.Owner != null)
							using(new AllowSetIsModifiedHelper(Owner.Owner)) {
								Owner.Owner.AllowSetIsModified = true;
								startHitTest.Item.Parent.ResizeManager.SetSplitterSizing(splitter);
								startHitTest.Item.Parent.ChangeItemPosition(startHitTest.Item, location);
								Owner.Invalidate();
								Owner.Owner.Control.Update();
							} else {
							startHitTest.Item.Parent.ResizeManager.SetSplitterSizing(splitter);
							startHitTest.Item.Parent.ChangeItemPosition(startHitTest.Item, location);
							Owner.Invalidate();
						}
					}
				} else {
					if(Owner.Owner != null && !Owner.Owner.DesignMode && !Owner.Owner.EnableCustomizationMode) return;
					Size size = startSize;
					int delta = 0;
					if(startHitTest.HitType == LayoutItemHitTest.HSizing) {
						size.Width += e.X - startHitTest.HitPoint.X; 
						delta = ResizeHelper.GetDeltaPosition(itemsPosition, startHitTest.Item.ViewInfo.BoundsRelativeToControl.X + size.Width);
						if(Math.Abs(delta) < ResizeHelper.StuckDelta && !itemsPosition.Contains(size.Width)) size.Width += delta;
					} else {
						size.Height += e.Y - startHitTest.HitPoint.Y;
						delta = ResizeHelper.GetDeltaPosition(itemsPosition, startHitTest.Item.ViewInfo.BoundsRelativeToControl.Y + size.Height);
						if(Math.Abs(delta) < ResizeHelper.StuckDelta && !itemsPosition.Contains(size.Height)) size.Height += delta;
					}
					if(startHitTest.Item.Parent != null) {
						startHitTest.Item.Parent.ChangeItemSize(startHitTest.Item, size, this.sizingStartGroupSize);
						Owner.Invalidate();
						if(Owner.Owner != null)
							Owner.Owner.Control.Update();
					}
				}
			}
		}
		private bool NotSizingItem() {
			if(startHitTest.Item is SplitterItem) return false;
			if(startHitTest.HitType == LayoutItemHitTest.HSizing && startHitTest.Item.MaxSize.Width == startHitTest.Item.MinSize.Width) return true;
			if(startHitTest.HitType == LayoutItemHitTest.VSizing && startHitTest.Item.MaxSize.Height == startHitTest.Item.MinSize.Height) return true;
			return false;
		}
		private void SetSizingTypeForAdorner() {
			if(!(startHitTest.Item is SplitterItem) && startHitTest.HitType == LayoutItemHitTest.HSizing) {
				startHitTest.Item.sizingTypeCore = Resizing.LayoutSizingType.Horizontal;
			}
			if(!(startHitTest.Item is SplitterItem) && startHitTest.HitType == LayoutItemHitTest.VSizing) {
				startHitTest.Item.sizingTypeCore = Resizing.LayoutSizingType.Vertical;
			}
		}
		void OnMouseMoveDragging(MouseEventArgs e) { }
	}
}

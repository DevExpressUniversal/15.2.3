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

using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.XtraToolbox {
	public class ToolboxHandler {
		ToolboxControl toolbox;
		public ToolboxHandler(ToolboxControl toolbox) {
			this.toolbox = toolbox;
		}
		bool isInSplitter = false;
		ToolboxViewInfo ViewInfo {
			get { return toolbox.ViewInfo; }
		}
		ToolboxHitInfo captureHitInfo;
		ToolboxHitInfo CaptureHitInfo {
			get { return captureHitInfo; }
			set {
				if(CaptureHitInfo == value)
					return;
				captureHitInfo = value;
				if(captureHitInfo == null)
					return;
				ViewInfo.UpdateCursor(captureHitInfo.HitTest);
			}
		}
		DragController dragController;
		protected internal DragController DragController {
			get {
				if(dragController == null)
					dragController = new DragController(toolbox);
				return dragController;
			}
		}
		protected bool IsElementDragging(Point location) {
			if(ViewInfo.SelectedItems.Count == 0) return false;
			return Math.Abs(location.X - CaptureHitInfo.HitPoint.X) > 2 || Math.Abs(location.Y - CaptureHitInfo.HitPoint.Y) > 2;
		}
		protected virtual void Dragging() {
			ToolboxDragItemMoveEventArgs args = new ToolboxDragItemMoveEventArgs(ViewInfo.SelectedItems, Cursor.Position);
			toolbox.RaiseDragItemMove(args);
			toolbox.ViewInfo.UpdateCursor(args.DragDropEffects);
		}
		protected virtual void StartDragging() {
			Image dragImage = null;
			if(ViewInfo.SelectedItems.Count > 0) {
				ToolboxDragItemStartEventArgs args = new ToolboxDragItemStartEventArgs(ViewInfo.SelectedItems);
				toolbox.RaiseDragItemStart(args);
				if(args.Cancel) return;
				dragImage = args.Handled ? args.Image : GetDraggingElementBitmap();
			}
			DragController.OnStartDragging(dragImage);
			ViewInfo.InDragging = true;
			ViewInfo.UpdateCursor();
		}
		protected virtual Bitmap GetDraggingElementBitmap() {
			ToolboxElementBase draggingElement = ViewInfo.SelectedItems.FirstOrDefault();
			if(draggingElement == null || draggingElement.Image == null)
				return new Bitmap(1, 1);
			return new Bitmap(draggingElement.Image);
		}
		protected virtual void OnDragDrop() {
			if(ViewInfo.IsDesignTime) return;
			DragController.OnDragDrop();
			toolbox.RaiseDragItemDrop(new ToolboxDragItemDropEventArgs(ViewInfo.SelectedItems));
			ViewInfo.InDragging = false;
		}
		protected internal virtual void OnMouseClick(MouseEventArgs e) {
			if(ViewInfo.IsDesignTime) return;
			ToolboxHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(!hitInfo.IsInItem) return;
			toolbox.RaiseItemClick(new ToolboxItemClickEventArgs(hitInfo.ItemInfo.Item));
			if(pressedKeys != (Keys.Control | Keys.ControlKey))
				ProcessSelectedItems(CaptureHitInfo.ElementInfo.Element as ToolboxItem);
		}
		protected virtual bool CanClearSelectedItems() {
			if(pressedKeys != (Keys.ControlKey | Keys.Control)) return true;
			if(toolbox.OptionsBehavior.ItemSelectMode != ToolboxItemSelectMode.Multiple) return true;
			return false;
		}
		protected virtual void ProcessSelectedItems(ToolboxItem item) {
			if(CanClearSelectedItems())
				ViewInfo.SelectedItems.Clear();
			ViewInfo.SelectedItems.Add(item);
			ViewInfo.CalcViewInfo(ViewInfo.GInfo.Graphics);
		}
		protected internal virtual void OnMouseDoubleClick(MouseEventArgs e) {
			if(ViewInfo.IsDesignTime) return;
			ToolboxHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(!hitInfo.IsInItem) return;
			toolbox.RaiseItemDoubleClick(new ToolboxItemDoubleClickEventArgs(hitInfo.ItemInfo.Item));
		}
		protected internal virtual void OnMouseUp(MouseEventArgs e) {
			if(toolbox.IsDisposed) return;
			ViewInfo.CheckStateObject(e.Location);
			isInSplitter = false;
			if(ViewInfo.InDragging) {
				OnDragDrop();
				return;
			}
			if(CaptureHitInfo == null) return;
			ToolboxGroupInfo pressedGroup = CaptureHitInfo.ElementInfo as ToolboxGroupInfo;
			if(pressedGroup == null) return;
			toolbox.SelectedGroup = pressedGroup.Group;
			CaptureHitInfo = null;
		}
		Keys pressedKeys;
		protected internal virtual void OnKeyDown(KeyEventArgs e) {
			if(ViewInfo.IsDesignTime) return;
			pressedKeys = e.KeyData;
		}
		protected internal virtual void OnKeyUp(KeyEventArgs e) {
			if(ViewInfo.IsDesignTime) return;
			pressedKeys = Keys.None;
		}
		protected internal virtual void OnMouseDown(MouseEventArgs e) {
			CaptureHitInfo = ViewInfo.CalcHitInfo(e.Location);
			ViewInfo.CheckStateObject(e.Location, CaptureHitInfo);
			if(ViewInfo.IsDesignTime) {
				toolbox.DesignManager.OnMouseDown(e, CaptureHitInfo);
				return;
			}
			switch(CaptureHitInfo.HitTest) {
				case ToolboxHitTest.Splitter:
					isInSplitter = true;
					return;
				case ToolboxHitTest.ExpandButton:
					toolbox.InvertExpanded();
					return;
				case ToolboxHitTest.MoreItemsButton:
					ViewInfo.MoreItemsButton.Show();
					return;
				case ToolboxHitTest.MenuButton:
					ViewInfo.MenuButton.Show();
					return;
				case ToolboxHitTest.ScrollButtonUp:
					ViewInfo.Toolbox.ScrollController.ItemScroll.SmoothVScroll.SetValue(ViewInfo.Toolbox.ScrollController.ItemScroll.SmoothVScroll.SmallChange);
					return;
				case ToolboxHitTest.ScrollButtonDown:
					ViewInfo.Toolbox.ScrollController.ItemScroll.SmoothVScroll.SetValue(-ViewInfo.Toolbox.ScrollController.ItemScroll.SmoothVScroll.SmallChange);
					return;
				case ToolboxHitTest.Item:
					if(toolbox.IsDesignMode) break;
					if(pressedKeys != (Keys.ControlKey | Keys.Control)) {
						foreach(ToolboxItem i in ViewInfo.SelectedItems) {
							Rectangle rect = ViewInfo.GetItemInfo(i).Bounds;
							if(rect.Contains(ViewInfo.Toolbox.PointToClient(Cursor.Position)))
								return;
						}
					}
					ProcessSelectedItems(CaptureHitInfo.ElementInfo.Element as ToolboxItem);
					return;
				case ToolboxHitTest.ScrollBar:
				case ToolboxHitTest.None:
				case ToolboxHitTest.Group:
				default:
					break;
			}
		}
		protected internal virtual void OnMouseMove(MouseEventArgs e) {
			if(ViewInfo.IsDesignTime) return;
			ViewInfo.CheckStateObject(e.Location);
			if(e.Button == MouseButtons.Left && !ViewInfo.IsDesignTime) {
				if(ViewInfo.InDragging && ViewInfo.SelectedItems.Count > 0) {
					DragController.OnDragging(e.Location);
					Dragging();
					return;
				}
				if(!ViewInfo.InDragging && IsElementDragging(e.Location)) {
					StartDragging();
					return;
				}
			}
			if(e.Button == MouseButtons.Left && isInSplitter) {
				ViewInfo.SetGroupsRegionHeight(e.Y - ViewInfo.Rects.GroupsContentRect.Top);
				toolbox.Refresh();
				return;
			}
			if(ViewInfo.GetSplitterRectArea().Contains(e.Location))
				ViewInfo.UpdateCursor(ToolboxHitTest.Splitter);
		}
		protected internal virtual void OnMouseEnter(EventArgs e) { }
		protected internal virtual void OnMouseLeave(EventArgs e) {
			if(ViewInfo.IsDesignTime) return;
			ViewInfo.ResetStateObject();
		}
		protected internal virtual void OnMouseWheel(MouseEventArgs e) {
			if(ViewInfo.IsDesignTime) return;
			ViewInfo.CheckStateObject(e.Location);
			toolbox.ScrollController.GroupScroll.ProcessScroll(e);
			toolbox.ScrollController.ItemScroll.ProcessScroll(e);
		}
	}
	public enum ToolboxHitTest { None, Group, Item, Splitter, ExpandButton, ScrollBar, MoreItemsButton, MenuButton, ScrollButtonUp, ScrollButtonDown }
	public class ToolboxHitInfo {
		public ToolboxHitInfo() : this(Point.Empty) { }
		public ToolboxHitInfo(Point pt) {
			this.hitPoint = pt;
			this.hitTest = ToolboxHitTest.None;
			this.element = null;
		}
		protected internal bool CheckBounds(Rectangle bounds, ToolboxHitTest hitTest, ToolboxElementInfoBase element) {
			if(bounds.Contains(HitPoint)) {
				this.hitTest = hitTest;
				this.element = element;
				return true;
			}
			return false;
		}
		Point hitPoint;
		public Point HitPoint {
			get { return hitPoint; }
		}
		ToolboxHitTest hitTest;
		public ToolboxHitTest HitTest {
			get { return hitTest; }
		}
		ToolboxElementInfoBase element;
		public ToolboxElementInfoBase ElementInfo {
			get { return element; }
		}
		public void Reset() {
			this.hitPoint = Point.Empty;
			this.hitTest = ToolboxHitTest.None;
			this.element = null;
		}
		public ToolboxHitInfo Clone() {
			ToolboxHitInfo hitInfo = new ToolboxHitInfo();
			hitInfo.hitPoint = this.hitPoint;
			hitInfo.hitTest = this.hitTest;
			hitInfo.element = this.element;
			return hitInfo;
		}
		public override bool Equals(object obj) {
			ToolboxHitInfo other = obj as ToolboxHitInfo;
			if(other == null || ElementInfo == null) return false;
			if(ElementInfo.Equals(other.ElementInfo)) return false;
			if(HitTest != other.HitTest) return false;
			if(ElementInfo.Bounds != other.ElementInfo.Bounds) return false;
			return true;
		}
		public static readonly ToolboxHitInfo Empty = new ToolboxHitInfo();
		public bool IsEmpty {
			get { return Equals(Empty); }
		}
		public bool IsInGroup {
			get { return HitTest == ToolboxHitTest.Group; }
		}
		public bool IsInItem {
			get { return HitTest == ToolboxHitTest.Item; }
		}
		public bool IsInElement {
			get { return IsInGroup || IsInItem; }
		}
		public ToolboxGroupInfo GroupInfo {
			get { return this.element as ToolboxGroupInfo; }
		}
		public ToolboxItemInfo ItemInfo {
			get { return this.element as ToolboxItemInfo; }
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class ToolBoxElementInfoStateObject {
		ToolboxHitInfo stateObject;
		public ToolBoxElementInfoStateObject() {
			this.stateObject = new ToolboxHitInfo();
		}
		public ToolboxElementBase GetElement() {
			if(!this.stateObject.IsInElement) return null;
			return this.stateObject.ElementInfo.Element;
		}
		public void Reset() {
			SetStateObject(new ToolboxHitInfo());
		}
		public void SetStateObject(ToolboxHitInfo newObj) {
			if(newObj != null && newObj.Equals(stateObject)) return;
			ToolboxHitInfo prev = this.stateObject;
			this.stateObject = newObj.Clone();
			OnStateObjectChanged(new ToolBoxElementInfoStateObjectChangedEventArgs(prev, this.stateObject));
		}
		protected virtual void OnStateObjectChanged(ToolBoxElementInfoStateObjectChangedEventArgs e) {
			if(StateObjectChanged == null) return;
			StateObjectChanged(this, e);
		}
		public event ToolBoxElementInfoStateObjectChangedHandler StateObjectChanged;
	}
	public delegate void ToolBoxElementInfoStateObjectChangedHandler(object sender, ToolBoxElementInfoStateObjectChangedEventArgs e);
	public class ToolBoxElementInfoStateObjectChangedEventArgs : EventArgs {
		ToolboxHitInfo prev;
		ToolboxHitInfo next;
		public ToolBoxElementInfoStateObjectChangedEventArgs(ToolboxHitInfo prev, ToolboxHitInfo next) {
			this.prev = prev;
			this.next = next;
		}
		public ToolboxHitInfo Prev { get { return prev; } }
		public ToolboxHitInfo Next { get { return next; } }
	}
	public class ToolboxScrollController {
		ToolboxControl toolbox;
		public ToolboxScrollController(ToolboxControl toolbox) {
			this.toolbox = toolbox;
			this.groupScroll = CreateGroupsScrollBar();
			this.itemScroll = CreateItemsScrollBar();
			this.groupScroll.AddControls(toolbox);
			this.itemScroll.AddControls(toolbox);
		}
		protected virtual ToolboxGroupsScrollBar CreateGroupsScrollBar() {
			return new ToolboxGroupsScrollBar(toolbox);
		}
		protected virtual ToolboxItemsScrollBar CreateItemsScrollBar() {
			return new ToolboxItemsScrollBar(toolbox);
		}
		ToolboxGroupsScrollBar groupScroll;
		public ToolboxGroupsScrollBar GroupScroll {
			get { return groupScroll; }
		}
		ToolboxItemsScrollBar itemScroll;
		public ToolboxItemsScrollBar ItemScroll {
			get { return itemScroll; }
		}
		public void Dispose() {
			if(this.groupScroll != null) {
				this.groupScroll.RemoveControls(this.toolbox);
				this.groupScroll.Dispose();
			}
			if(this.itemScroll != null) {
				this.itemScroll.RemoveControls(this.toolbox);
				this.itemScroll.Dispose();
			}
		}
		int lockScrollUpdate = 0;
		protected internal virtual void UpdateScrolls() {
			if(this.lockScrollUpdate != 0) return;
			ToolboxViewInfo info = toolbox.ViewInfo;
			this.lockScrollUpdate++;
			try {
				GroupScroll.UpdateScroll();
				ItemScroll.UpdateScroll();
			}
			finally {
				this.lockScrollUpdate--;
			}
		}
		public void AdjustContent(bool rtl) {
			if(GroupScroll.IsVisible) {
				if(rtl)
					toolbox.ViewInfo.Rects.Offset(ToolboxRectangle.GroupsContentRect, GroupScroll.VScrollWidth, 0);
				toolbox.ViewInfo.Rects.AddWidth(ToolboxRectangle.GroupsContentRect, -GroupScroll.VScrollWidth);
			}
			if(ItemScroll.IsVisible) {
				if(rtl)
					toolbox.ViewInfo.Rects.Offset(ToolboxRectangle.ItemsContentRect, ItemScroll.VScrollWidth, 0);
				toolbox.ViewInfo.Rects.AddWidth(ToolboxRectangle.ItemsContentRect, -ItemScroll.VScrollWidth);
			}
		}
	}
	public class ToolboxGroupsScrollBar : ToolboxScrollBarBase {
		public ToolboxGroupsScrollBar(ToolboxControl toolbox) : base(toolbox) { }
		protected internal override bool IsVisible {
			get {
				if(Toolbox.ViewInfo.IsMinimized || Toolbox.ShouldDrawOnlyItems) return false;
				return Toolbox.ViewInfo.GetAllGroupsHeight() > Toolbox.ViewInfo.Rects.GroupsContentRect.Height;
			}
		}
		protected internal override Rectangle GetClientRectangle() {
			ToolboxViewInfo vi = Toolbox.ViewInfo;
			return new Rectangle(vi.Rects.GroupsClientRect.X, vi.Rects.GroupsContentRect.Y, vi.Rects.GroupsClientRect.Width - 1, vi.Rects.GroupsContentRect.Height);
		}
		protected internal override ScrollArgs GetScrollArgs() {
			ScrollArgs args = new ScrollArgs();
			args.Maximum = Toolbox.ViewInfo.GetAllGroupsHeight();
			args.LargeChange = Toolbox.ViewInfo.Rects.GroupsContentRect.Height;
			args.Value = Toolbox.ViewInfo.TopGroupIndent;
			args.SmallChange = Toolbox.ViewInfo.GetGroupHeight();
			return args;
		}
		protected override bool ShouldProcessScroll(Point p) {
			return Toolbox.ViewInfo.Rects.GroupsContentRect.Contains(p) && IsVisible;
		}
		protected override int GetDelta(MouseEventArgs e) {
			return (e.Delta < 0 ? -1 : 1) * Toolbox.ViewInfo.GetGroupHeight();
		}
	}
	public class ToolboxItemsScrollBar : ToolboxScrollBarBase {
		public ToolboxItemsScrollBar(ToolboxControl toolbox) : base(toolbox) { }
		protected internal override bool IsVisible {
			get {
				if(Toolbox.ViewInfo.IsMinimized) return false;
				return Toolbox.ViewInfo.GetAllItemsHeight() > Toolbox.ViewInfo.Rects.ItemsContentRect.Height;
			}
		}
		protected internal override Rectangle GetClientRectangle() {
			ToolboxViewInfo vi = Toolbox.ViewInfo;
			return new Rectangle(vi.Rects.ItemsClientRect.X, vi.Rects.ItemsContentRect.Y, vi.Rects.ItemsClientRect.Width - 1, vi.Rects.ItemsContentRect.Height);
		}
		protected internal override ScrollArgs GetScrollArgs() {
			ScrollArgs args = new ScrollArgs();
			args.Maximum = Toolbox.ViewInfo.GetAllItemsHeight();
			args.LargeChange = Toolbox.ViewInfo.Rects.ItemsContentRect.Height;
			args.Value = Toolbox.ViewInfo.TopItemIndent;
			args.SmallChange = Toolbox.ViewInfo.CalcItemBestSize().Height;
			return args;
		}
		protected virtual bool IsMinimizedScrollButtonsVisible {
			get { return !Toolbox.ViewInfo.ScrollButtonDown.Bounds.IsEmpty && !Toolbox.ViewInfo.ScrollButtonUp.Bounds.IsEmpty; }
		}
		protected override bool ShouldProcessScroll(Point p) {
			return Toolbox.ViewInfo.Rects.ItemsContentRect.Contains(p) && (IsMinimizedScrollButtonsVisible || IsVisible);
		}
		protected override int GetDelta(MouseEventArgs e) {
			return (e.Delta < 0 ? -1 : 1) * Toolbox.ViewInfo.CalcItemBestSize().Height * 3;
		}
	}
	public class ToolboxScrollBarBase : ScrollControllerBase {
		public ToolboxScrollBarBase(ToolboxControl toolbox) : base(toolbox) {
			this.toolbox = toolbox;
		}
		ToolboxControl toolbox;
		public ToolboxControl Toolbox {
			get { return toolbox; }
		}
		public void UpdateScroll() {
			IsRightToLeft = Toolbox.IsRightToLeft;
			VScrollVisible = IsVisible;
			ClientRect = GetClientRectangle();
			VScrollArgs = GetScrollArgs();
		}
		protected internal virtual bool IsVisible {
			get { return false; }
		}
		protected internal virtual Rectangle GetClientRectangle() {
			return Rectangle.Empty;
		}
		protected internal virtual ScrollArgs GetScrollArgs() {
			return new ScrollArgs();
		}
		protected virtual bool ShouldProcessScroll(Point p) {
			return false;
		}
		protected override XtraEditors.VScrollBar CreateVScroll() {
			return new ToolboxSmoothVScrollBar();
		}
		protected internal virtual ToolboxSmoothVScrollBar SmoothVScroll {
			get { return VScroll as ToolboxSmoothVScrollBar; }
		}
		protected virtual int GetDelta(MouseEventArgs e) {
			return e.Delta;
		}
		public void ProcessScroll(MouseEventArgs e) {
			if(!ShouldProcessScroll(e.Location)) return;
			if(Toolbox.ViewInfo.IsSmoothScrolling)
				SmoothVScroll.SetValue(GetDelta(e));
			else
				SmoothVScroll.Value -= GetDelta(e);
		}
	}
}

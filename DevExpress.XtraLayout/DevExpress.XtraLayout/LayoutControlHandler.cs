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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Dragging;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Customization.Templates;
using DevExpress.XtraLayout.Helpers;
using System.Collections.Generic;
namespace DevExpress.XtraLayout.Handlers {
	public class TabActivator : IDisposable {
		int hotTabIndexCore;
		TabbedGroup tgCore;
		Timer timer;
		public TabActivator(int hotTabIndex, TabbedGroup tg) {
			hotTabIndexCore = hotTabIndex;
			tgCore = tg;
			timer = new Timer();
			timer.Interval = 500;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
			Done = false;
		}
		void timer_Tick(object sender, EventArgs e) {
			Point cPos = tgCore.Owner.Control.PointToClient(Cursor.Position);
			BaseLayoutItemHitInfo hi = tgCore.Owner.RootGroup.CalcHitInfo(cPos, false);
			if(hi.TabPageIndex == hotTabIndexCore) tgCore.SelectedTabPageIndex = hotTabIndexCore;
			Dispose();
		}
		public void Dispose() {
			timer.Dispose();
			tgCore = null;
			Done = true;
		}
		private bool doneCore;
		public bool Done {
			get { return doneCore; }
			set { doneCore = value; }
		}
		public int HotIndex {
			get { return hotTabIndexCore; }
		}
	}
	public class LayoutControlDragDropHelper : IDisposable {
		ILayoutControl ownerControlCore = null;
		DraggingVisualizer draggingVisualizerCore = null;
		DragDropClientGroupDescriptor clientGroupCore = null;
		DragDropClientDescriptor clientDescriptorCore = null;
		protected NonClientSpaceDragVisualizer visualizer;
		readonly Size DefaultSize = new Size(DragDropItemCursor.cursorWidth, DragDropItemCursor.cursorHeight);
		public LayoutControlDragDropHelper(ILayoutControl owner) {
			this.ownerControlCore = owner;
			this.draggingVisualizerCore = CreateDraggingVisualizer();
			InitNonClientVisualizer();
		}
		protected virtual void InitNonClientVisualizer() {
			this.visualizer = NonClientSpaceDragVisualizer.QueryReference();
		}
		protected virtual DraggingVisualizer CreateDraggingVisualizer() {
			return new DraggingVisualizer(Owner);
		}
		public NonClientSpaceDragVisualizer NonClientSpaceVisualizer {
			get { return visualizer; }
		}
		public void Dispose() {
			if(tabActivator != null) {
				tabActivator.Dispose();
				tabActivator = null;
			}
			if(draggingVisualizerCore != null) {
				draggingVisualizerCore.Dispose();
				draggingVisualizerCore = null;
			}
			if(clientDescriptorCore != null) {
				DragDropDispatcherFactory.Default.UnRegisterClient(clientDescriptorCore);
				clientDescriptorCore = null;
			}
			clientGroupCore = null;
			ownerControlCore = null;
			DisposeNonclientVisualizer();
		}
		protected virtual void DisposeNonclientVisualizer() {
			NonClientSpaceDragVisualizer.Release(ref visualizer);
		}
		public virtual void Initialize() {
			clientDescriptorCore = DragDropDispatcherFactory.Default.RegisterClient(Owner as IDragDropDispatcherClient);
			clientGroupCore = DragDropDispatcherFactory.Default.GetUniqueGroupDescriptor();
			DragDropDispatcherFactory.Default.SetClientToolFrame(ClientDescriptor, GetDragDropItemCursor());
			NonClientSpaceVisualizer.Enable = true;
		}
		protected ILayoutControl Owner {
			get { return ownerControlCore; }
		}
		protected DraggingVisualizer DraggingVisualizer {
			get { return draggingVisualizerCore; }
		}
		public DragDropClientDescriptor ClientDescriptor {
			get { return clientDescriptorCore; }
		}
		public DragDropClientGroupDescriptor ClientGroup {
			get { return clientGroupCore; }
		}
		protected BaseLayoutItemHitInfo CalcHitInfo(Point p) {
			return ((LayoutControl)Owner).CalcHitInfo(p);
		}
		public bool AllowNonItemDrop {
			get { return false; }
			set { }
		}
		public bool IsPointOnItem(Point clientPoint) {
			return Owner.RootGroup.ViewInfo.BoundsRelativeToControl.Contains(clientPoint);
		}
		public bool IsPointOnItemAndNotSizing(Point clientPoint) {
			BaseLayoutItemHitInfo hi = CalcHitInfo(clientPoint);
			return Owner.RootGroup.ViewInfo.BoundsRelativeToControl.Contains(clientPoint) && !hi.IsSizing;
		}
		public BaseLayoutItem GetItemAtPoint(Point clientPoint) {
			BaseLayoutItemHitInfo hi = CalcHitInfo(clientPoint);
			TabbedGroupHitInfo tgHi = hi as TabbedGroupHitInfo;
			if(tgHi != null) {
				int index = tgHi.TabbedGroupHotPageIndex;
				if(index >= 0)
					return ((TabbedGroup)tgHi.Item).VisibleTabPages[index];
			}
			return hi.Item;
		}
		public BaseLayoutItem ProcessDragItemRequest(Point clientPoint) {
			BaseLayoutItem hitItem = GetItemAtPoint(clientPoint);
			if(hitItem.Parent != null) return hitItem;
			else return null;
		}
		public void OnDragModeKeyDown(KeyEventArgs kea) { }
		protected virtual DragDropItemCursor GetDragDropItemCursor() {
			return DragDropItemCursor.Default;
		}
		public virtual void OnDragEnter() {
			DragCursorOn(GetDragDropItemCursor());
		}
		public virtual void OnDragLeave() {
			DragCursorOff(GetDragDropItemCursor());
			DraggingVisualizer.HideDragBounds();
		}
		TabActivator tabActivator = null;
		public virtual void DoDragging(Point clientPoint) {
			LayoutItemDragController dragController = CreateLayoutItemDragController(DragDropDispatcherFactory.Default.DragItem, clientPoint);
			DragCursorOn(GetDragDropItemCursor());
			DragCursorSetPos(GetDragDropItemCursor(), clientPoint);
			if((Owner is LayoutControl) && !(Owner as LayoutControl).RaiseOnItemDragging(dragController)) DraggingVisualizer.ShowDragBounds(null);
			else DraggingVisualizer.ShowDragBounds(dragController);
			TabbedGroupHitInfo tgHi = dragController.HitInfo as TabbedGroupHitInfo;
			if(tgHi != null && tgHi.TabbedGroupHotPageIndex >= 0) {
				if((tabActivator == null || (tabActivator != null && tabActivator.Done) || (tabActivator != null && tabActivator.HotIndex != tgHi.TabbedGroupHotPageIndex))) {
					tabActivator = new TabActivator(tgHi.TabbedGroupHotPageIndex, (TabbedGroup)tgHi.Item);
				}
			}
		}
		public void DoDrop(Point clientPoint) {
			EndDragging(clientPoint);
			DragCursorOff(GetDragDropItemCursor());
			DraggingVisualizer.HideDragBounds();
		}
		protected virtual LayoutItemDragController CreateLayoutItemDragController(BaseLayoutItem dragItem, Point pt) {
			return new LayoutItemDragController(dragItem, Owner.RootGroup, pt);
		}
		public bool CheckCustomizationConstraints(LayoutItemDragController controller) {
			if(controller == null) return false;
			if(controller.DragItem == null || controller.Item == null) return false;
			if(controller.HitInfo != null && controller.HitInfo is LayoutGroupHitInfo && (controller.HitInfo as LayoutGroupHitInfo).AdditionalHitType == LayoutGroupHitTypes.TableDefinition) {
				if(controller.Item is LayoutGroup && (controller.Item as LayoutGroup).LayoutMode == Utils.LayoutMode.Table) {
					return true;
				}
			}
			if(controller.Item.Parent == null) return false;
			return true;
		}
		public bool CheckCustomizationConstraintsWildItem(LayoutItemDragController controller) {
			if(controller == null) return false;
			if(controller.DragItem == null || controller.Item == null) return false;
			return true;
		}
		protected TemplateManager GetTemplateManager(BaseLayoutItem item) {
			if(item.Tag != null && item.Tag is TemplateManager) return item.Tag as TemplateManager;
			return null;
		}
		protected virtual void EndDragging(Point clientPoint) {
			LayoutItemDragController dragController = CreateLayoutItemDragController(DragDropDispatcherFactory.Default.DragItem, clientPoint);
			if((Owner is LayoutControl) && !(Owner as LayoutControl).RaiseOnItemDragging(dragController)) return;
			TemplateManager tManager = GetTemplateManager(dragController.DragItem);
			if(dragController.DragItem != null && Owner.RootGroup != null) {
				if(Owner.RootGroup.BeginChangeUpdate()) {
					if(tManager != null) {
						Owner.RootGroup.ClearSelection();
						try {
							tManager.RestoreTemplate(null, Owner as LayoutControl, dragController);
						} catch {
							tManager = null; 
						}
					} else {
						if(dragController.DragItem.IsHidden) {
							dragController.DragItem.RestoreFromCustomization();
							dragController.Drag();
						} else {
							if(dragController.DragItem.Owner == null) {
								if(CheckCustomizationConstraintsWildItem(dragController)) dragController.DragWildItem();
							} else {
								if(CheckCustomizationConstraints(dragController)) dragController.Drag();
							}
						}
						if(dragController.DragItem.Parent != null) dragController.DragItem.Parent.ClearSelection();
						dragController.DragItem.Selected = true;
					}
					Owner.RootGroup.EndChangeUpdate();
					if(tManager != null) {
						SelectItemsAfterPasteTemplate();
					}
				}
			}
		}
		void SelectItemsAfterPasteTemplate() {
			List<BaseLayoutItem> listSelection = new List<BaseLayoutItem>();
			Owner.RootGroup.ClearSelection();
			foreach(BaseLayoutItem item in Owner.Items) {
				if(item.Name == TemplateString.LayoutGroupForRestoreName && item is LayoutGroup && item.Tag as string == TemplateString.LayoutGroupForRestoreName) {
					item.Selected = true;
					FlatItemsList FIL = new FlatItemsList();
					listSelection = FIL.GetItemsList(item);
					(item as LayoutGroup).Parent.UngroupSelected();
					break;
				}
			}
			Owner.RootGroup.UngroupSelected();
			foreach(BaseLayoutItem bli in listSelection) bli.Selected = true;
		}
		public virtual void DoBeginDrag() {
			DragCursorOn(GetDragDropItemCursor());
		}
		static void RedrawControl(Control control) {
			if(control == null) return;
			control.Invalidate();
		}
		public virtual void DoDragCancel() {
			DragCursorOff(GetDragDropItemCursor());
			DraggingVisualizer.HideDragBounds();
		}
		protected virtual void DragCursorOn(DragDropItemCursor cursor) {
			if(!cursor.Visible && !RDPHelper.IsRemoteSession) {
				cursor.DragItem = DragDropDispatcherFactory.Default.DragItem;
				cursor.Visible = true;
				cursor.Update();
				cursor.BringToFront();
				RedrawControl(Owner.Control);
			}
			if((Control.ModifierKeys & Keys.Alt) != 0) cursor.Opacity = 0.5;
			else cursor.Opacity = 1;
		}
		protected virtual void DragCursorOff(DragDropItemCursor cursor) {
			if(cursor.Visible) cursor.Visible = false;
			RedrawControl(Owner.Control);
		}
		protected virtual void DragCursorSetPos(DragDropItemCursor cursor, Point pt) {
			if(cursor.Visible) {
				Point screenPoint = pt - new Size(DefaultSize.Width / 2, DefaultSize.Height / 2);
				cursor.Location = Owner.Control.PointToScreen(screenPoint);
				cursor.Size = DefaultSize;
				cursor.BringToFront();
			}
		}
	}
	public class LayoutControlHandler : IDisposable {
		ILayoutControl ownerControlCore = null;
		Cursor sizingCursorCore = null;
		protected internal BaseLayoutItem LastSelectedItem;
		public LayoutControlHandler(ILayoutControl owner) {
			this.ownerControlCore = owner;
		}
		public void Reset() {
			Owner.RootGroup.Handler.ResetHandler();
			Owner.SetCursor(Cursors.Arrow);
		}
		public ILayoutControl Owner { get { return ownerControlCore; } }
		public LayoutGroup Group { get { return Owner.RootGroup; } }
		public LayoutHandlerState State {
			get { return Owner.RootGroup.Handler.State; }
			set { Owner.RootGroup.Handler.State = value; }
		}
		protected BaseLayoutItemHitInfo MouseHitTest {
			get { return Owner.RootGroup.Handler.MouseHitTest; }
		}
		protected LayoutGroupHandler RootHandlerHelper {
			get { return Owner.RootGroup.Handler as LayoutGroupHandler; }
		}
		protected void UpdateCursor() {
			if(IsLimitedFunctionality) return;
			if(this.State == LayoutHandlerState.Sizing) return;
		}
		protected void SetSplitterItemSizeCursor(SplitterItem item) {
			Owner.SetCursor(item.IsHorizontal ? Cursors.HSplit : Cursors.VSplit);
		}
		bool allowSetCursorCore = true;
		public bool AllowSetCursor { get { return allowSetCursorCore; } set { allowSetCursorCore = value; } }
		protected void SetCursor(LayoutItemHitTest hitTestType) {
			if(!AllowSetCursor) return;
			if(IsLimitedFunctionality) {
				if(RootHandlerHelper.HotTrackHelper.PressedItem is SplitterItem) {
					SetSplitterItemSizeCursor((SplitterItem)RootHandlerHelper.HotTrackHelper.PressedItem);
					return;
				}
				if(MouseHitTest != null && MouseHitTest.Item is SplitterItem) {
					SetSplitterItemSizeCursor((SplitterItem)MouseHitTest.Item);
					return;
				}
				Owner.SetCursor(Cursors.Arrow);
				return;
			}
			if(MouseHitTest != null) SetSizingCursor(MouseHitTest.HitType);
			if(this.State == LayoutHandlerState.Sizing) {
				Owner.SetCursor(sizingCursorCore);
			}
			if(this.State == LayoutHandlerState.Normal && Cursor.Current != Cursors.HSplit && Cursor.Current != Cursors.VSplit) {
				if(hitTestType == LayoutItemHitTest.HSizing)
					Owner.SetCursor(Cursors.SizeWE);
				else {
					if(hitTestType == LayoutItemHitTest.VSizing)
						Owner.SetCursor(Cursors.SizeNS);
					else
						Owner.SetCursor(Cursors.Arrow);
				}
				return;
			}
		}
		public virtual void Dispose() {
			ownerControlCore = null;
			sizingCursorCore = null;
		}
		protected void SetSizingCursor(LayoutItemHitTest hitTestType) {
			if(hitTestType == LayoutItemHitTest.HSizing) sizingCursorCore = Cursors.SizeWE;
			if(hitTestType == LayoutItemHitTest.VSizing) sizingCursorCore = Cursors.SizeNS;
		}
		protected void CalculateMouseHitTest(Point point) {
			Owner.RootGroup.Handler.CalculateMouseHitTest(point);
		}
		public bool ProcessMessage(EventType eventType, MouseEventArgs e) {
			return PerformControlActions(eventType, e);
		}
		protected internal bool PerformControlActions(EventType eventType, MouseEventArgs e) {
			try {
				if(!((ILayoutDesignerMethods)Owner).AllowHandleEvents) return false;
				CalculateMouseHitTest(e.Location);
				if(!PerformOwnersActions(eventType, e))
					return PerformChildHandlerActions(eventType, e);
				return false;
			} catch(LayoutControlInternalException) {
				Reset();
				Owner.ExceptionsThrown = true;
			}
			return false;
		}
		protected internal bool PerformOwnersActions(EventType eventType, MouseEventArgs e) {
			if(MouseHitTest == null) return false;
			SetCursor(MouseHitTest.HitType);
			if(e.Button == System.Windows.Forms.MouseButtons.Right && eventType == EventType.MouseUp && Owner.EnableCustomizationForm) {
				BaseLayoutItem item = Owner.RootGroup.Handler.MouseHitTest.Item;
				if(item == null) return false;
				if(Owner.AllowCustomizationMenu || Owner.DesignMode) {
					Owner.CustomizationMenuManager.ShowPopUpMenu(e.Location);
				}
			}
			return false;
		}
		protected virtual bool IsLimitedFunctionality {
			get { return !(Owner.EnableCustomizationMode || Owner.DesignMode); }
		}
		protected internal bool PerformChildHandlerActions(EventType eventType, MouseEventArgs e) {
			if(e != null) {
				UpdateChildHandlerSettings();
				if(Owner.RootGroup.ViewInfo.BoundsRelativeToControl.Contains(e.Location) || Owner.Control.Capture) return Owner.RootGroup.Handler.PerformControlActions(eventType, e);
				else return false;
			} else
				return false;
		}
		protected virtual void UpdateChildHandlerSettings() {
			Owner.RootGroup.Handler.LimitedFunctionality = IsLimitedFunctionality;
		}
		internal bool isTesting = false;
		protected bool AutoScrollByMoving() {
			if(Owner == null) return false;
			if(Owner.RootGroup == null) return false;
			if(!isTesting && DragDropDispatcherFactory.Default.State == DragDropDispatcherState.Regular) return false;
			Point mousePos = Cursor.Position;
			mousePos = Owner.Control.PointToClient(mousePos);
			Rectangle boundsRect = Owner.Bounds;
			boundsRect.X = 0;
			boundsRect.Y = 0;
			int hAS = Owner.Scroller.HorizontalAutoscrollSize;
			int vAS = Owner.Scroller.VerticalAutoscrollSize;
			Rectangle innerRect = new Rectangle(
				boundsRect.X + hAS,
				boundsRect.Y + vAS,
				boundsRect.Width - (hAS << 1),
				boundsRect.Height - (vAS << 1)
				);
			Rectangle outerLeftRect;
			Rectangle outerRightRect;
			Rectangle outerTopRect;
			Rectangle outerBottomRect;
			if(Owner.Scroller.HScrollVisible && !Owner.Scroller.VScrollVisible) {
				outerTopRect = Rectangle.Empty;
				outerBottomRect = Rectangle.Empty;
				outerLeftRect = new Rectangle(boundsRect.X, boundsRect.Y, hAS, boundsRect.Height);
				outerRightRect = new Rectangle(innerRect.Right, boundsRect.Y, hAS, boundsRect.Height);
			} else {
				if(!Owner.Scroller.HScrollVisible && Owner.Scroller.VScrollVisible) {
					outerTopRect = new Rectangle(boundsRect.X, boundsRect.Y, boundsRect.Width, vAS);
					outerBottomRect = new Rectangle(boundsRect.X, innerRect.Bottom, boundsRect.Width, vAS);
					outerLeftRect = Rectangle.Empty;
					outerRightRect = Rectangle.Empty;
				} else {
					outerLeftRect = new Rectangle(boundsRect.X, boundsRect.Y + vAS, hAS, innerRect.Height);
					outerRightRect = new Rectangle(innerRect.Right, boundsRect.Y + vAS, hAS, innerRect.Height);
					outerTopRect = new Rectangle(innerRect.X, boundsRect.Y, innerRect.Width, vAS);
					outerBottomRect = new Rectangle(innerRect.X, innerRect.Bottom, innerRect.Width, vAS);
				}
			}
			Point oldGroupPos = Owner.RootGroup.Location;
			int scrollStep = Owner.Scroller.AutoScrollStep;
			Scrolling.ScrollInfo scroller = Owner.Scroller;
			if(scroller.HScroll.Visible) {
				if(outerLeftRect.Contains(mousePos) && scroller.HScrollPos > 0) {
					scroller.HScrollPos -= scrollStep;
					Owner.SetCursor(Cursors.PanWest);
				}
				if(outerRightRect.Contains(mousePos) && scroller.HScrollPos < scroller.HScrollRange) {
					scroller.HScrollPos += scrollStep;
					Owner.SetCursor(Cursors.Arrow);
				}
			}
			if(Owner.Scroller.VScroll.Visible) {
				if(outerTopRect.Contains(mousePos) && scroller.VScrollPos > 0) {
					scroller.VScrollPos -= scrollStep;
					Owner.SetCursor(Cursors.PanNorth);
				}
				if(outerBottomRect.Contains(mousePos) && scroller.VScrollPos < scroller.VScrollRange) {
					scroller.VScrollPos += scrollStep;
					Owner.SetCursor(Cursors.PanSouth);
				}
			}
			bool changed = oldGroupPos != Owner.RootGroup.Location;
			if(changed) {
				Owner.Invalidate();
			}
			return oldGroupPos != Owner.RootGroup.Location;
		}
		public virtual void OnTimer() {
			AutoScrollByMoving();
		}
		protected virtual void InvalidateLayoutAdornerHandler() {
			if(Owner is LayoutControl && (Owner as LayoutControl).layoutAdornerWindowHandler != null) {
				(Owner as LayoutControl).layoutAdornerWindowHandler.Invalidate();
			}
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			if(e != null) {
				if(AutoScrollByMoving()) return;
				if(!PerformControlActions(EventType.MouseMove, e)) { }
			}
		}
		public virtual void OnMouseMove(object sender, MouseEventArgs e) {
			OnMouseMove(e);
		}
		public virtual void OnClick(object sender, MouseEventArgs e) {
			PerformControlActions(EventType.Click, e);
		}
		public virtual void OnDoubleClick(object sender, MouseEventArgs e) {
			if(Owner != null && Owner.RootGroup != null) PerformControlActions(EventType.DoubleClick, e);
		}
		public virtual void OnKeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Escape) { Reset(); return; }
			Owner.FocusHelper.ProcessOnKeyDown(e.KeyData);
		}
		public virtual bool OnMouseWheel(object sender, MouseEventArgs e) {
			return OnMouseWheel(e);
		}
		public virtual bool OnMouseDown(object sender, MouseEventArgs e) {
			return OnMouseDown(e);
		}
		public virtual bool OnMouseEnter(object sender, EventArgs e) {
			return OnMouseEnter(e);
		}
		public virtual bool OnMouseLeave(object sender, EventArgs e) {
			return OnMouseLeave(e);
		}
		public virtual bool OnMouseEnter(EventArgs e) {
			Owner.RootGroup.Handler.ResetHandler();
			return true;
		}
		public virtual bool OnMouseLeave(EventArgs e) {
			Reset();
			return true;
		}
		public virtual bool OnMouseDown(MouseEventArgs e) {
			PerformControlActions(EventType.MouseDown, e);
			return true;
		}
		void ProcessMouseWheel(MouseEventArgs e) {
			Owner.Scroller.VScrollPos -= e.Delta;
		}
		public virtual bool OnMouseWheel(MouseEventArgs e) {
			ProcessMouseWheel(e);
			return true;
		}
		public virtual bool OnMouseUp(object sender, MouseEventArgs e) {
			return OnMouseUp(e);
		}
		Point lastMouseUpPoint = Point.Empty;
		public virtual bool OnMouseUp(MouseEventArgs e) {
			PerformControlActions(EventType.MouseUp, e);
			lastMouseUpPoint = e.Location;
			return true;
		}
	}
}

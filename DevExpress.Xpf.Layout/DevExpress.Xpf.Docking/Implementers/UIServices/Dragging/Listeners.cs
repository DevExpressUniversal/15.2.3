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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Dragging;
using System;
using System.Windows;
#if SILVERLIGHT
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
#endif
namespace DevExpress.Xpf.Docking.Platform {
	public class LayoutViewRegularDragListener : RegularListener {
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		public override void OnEnter() {
			base.OnEnter();
			if(!KeyHelper.IsCtrlPressed) {
				ILayoutElement dragElement = View.Adapter.DragService.DragItem ?? View.Container.LinkedDragService.Return(x => x.DragItem, () => null);
				DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, lastPoint, dragElement);
				View.AdornerHelper.UpdateDockingHints(dragInfo, true);
			}
		}
		public override void OnLeave() {
			View.AdornerHelper.BeginHideAdornerWindowAndResetDockingHints();
			View.AdornerHelper.BeginHideAdornerWindowAndResetTabHeadersHints();
		}
		public override void OnCancel() {
			View.AdornerHelper.ResetDockingHints();
			View.AdornerHelper.TryHideAdornerWindow();
		}
		Point lastPoint;
		public override void OnDragging(Point point, ILayoutElement element) {
			if(!KeyHelper.IsCtrlPressed) {
				DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
				View.AdornerHelper.UpdateDockingHints(dragInfo);
			}
			lastPoint = point;
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			new DockingHelper(View).Drop(point, element);
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			return new DockingHelper(View).CanDrop(point, element);
		}
	}
	public class AutoHideViewRegularDragListener : RegularListener {
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		public override void OnLeave() {
			View.AdornerHelper.BeginHideAdornerWindowAndResetTabHeadersHints();
		}
	}
	public class LayoutViewFloatingDragListener : FloatingListener {
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		FloatingHelper helper;
		protected override IView GetFloatingView(ILayoutElement element) {
			helper = CreateFloatingHelper();
			return helper.GetFloatingView(element);
		}
		protected override void InitFloatingView(IView floatingView, Rect itemScreenRect, Rect itemContainerScreenRect) {
			helper.InitFloatingView(floatingView, itemScreenRect, itemContainerScreenRect);
		}
		protected virtual FloatingHelper CreateFloatingHelper() {
			return new FloatingHelper(View);
		}
	}
	public class AutoHideViewFloatingDragListener : LayoutViewFloatingDragListener {
		protected override FloatingHelper CreateFloatingHelper() {
			return new AutoHideFloatingHelper(View);
		}
	}
	public class FloatingViewFloatingMovingListener : FloatingMovingListener {
		LocationHelper helper;
		public FloatingView View {
			get { return ServiceProvider as FloatingView; }
		}
		public override void OnBegin(Point point, ILayoutElement element) {
			if(View.FloatGroup.IsMaximized)
				View.FloatGroup.ResetMaximized(new MarginHelper(View).Correct(point));
			helper = new LocationHelper(View, ElementHelper.GetRoot(element));
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			Point screenPoint = View.ClientToScreen(point);
			View.SetFloatLocation(helper.CalcLocation(screenPoint));
			NotificationBatch.Action(View.Container, View.FloatGroup, FloatGroup.FloatLocationProperty);
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			return true;
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			RaiseDockOperationCompleted(element);
		}
		void RaiseDockOperationCompleted(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			View.Container.RaiseDockOperationCompletedEvent(DockOperation.Move, item);
		}
	}
	public class FloatingViewNativeFloatingMovingListener : FloatingViewFloatingMovingListener {
		public override void OnBegin(Point point, ILayoutElement element) {
			base.OnBegin(point, element);
			UpdateFloatGroupBounds();
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			if(!UpdateFloatGroupBounds()) base.OnDragging(point, element);
		}
		private bool UpdateFloatGroupBounds() {
			return View.FloatGroup.RequestFloatingBoundsUpdate();
		}
	}
	public class FloatingViewFloatingResizingListener : FloatingResizingListener {
		IResizingPreviewHelper resizeHelper;
		BoundsHelper boundsHelper;
		public FloatingView View {
			get { return ServiceProvider as FloatingView; }
		}
		bool ShowPreview;
		public override void OnInitialize(Point point, ILayoutElement element) {
			Size max = LayoutItemsHelper.GetResizingMaxSize(View.FloatGroup);
			boundsHelper = new BoundsHelper(View, element, CalcResizingMinSize(View.FloatGroup), max);
			View.RootUIElement.SetValue(FrameworkElement.CursorProperty, boundsHelper.GetCursor());
			SizingAction sa = boundsHelper.GetSizingAction();
			View.Container.Win32DragService.SizingAction = sa;
		}
		public override void OnBegin(Point point, ILayoutElement element) {
			View.FloatGroup.ResetMaximized();
			ShowPreview = !View.Container.RedrawContentWhenResizing;
			if(ShowPreview) {
				resizeHelper = new FloatingResizingPreviewHelper(View) { MinSize = CalcResizingMinSize(View.FloatGroup), BoundsHelper = boundsHelper };
				resizeHelper.InitResizing(point, element);
			}
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			if(ShowPreview)
				resizeHelper.Resize(point);
			else
				ResizeCore(point);
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			return true;
		}
		public override void OnCancel() {
			if(ShowPreview)
				resizeHelper.EndResizing();
			View.RootUIElement.ClearValue(FrameworkElement.CursorProperty);
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			if(ShowPreview) {
				ResizeCore(point);
				resizeHelper.EndResizing();
			}
			RaiseDockOperationCompleted(element);
			View.RootUIElement.ClearValue(FrameworkElement.CursorProperty);
		}
		void RaiseDockOperationCompleted(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).CheckDragElement().Item;
			View.Container.RaiseDockOperationCompletedEvent(DockOperation.Resize, item);
		}
		Size CalcResizingMinSize(FloatGroup fGroup) {
			return LayoutItemsHelper.GetResizingMinSize(fGroup);
		}
		void ResizeCore(Point point) {
			Point screenPoint = View.ClientToScreen(point);
			View.FloatGroup.DisableSizeToContent();
			View.SetFloatingBounds(boundsHelper.CalcBounds(screenPoint));
			NotificationBatch.Action(View.Container, View.FloatGroup, FloatGroup.FloatLocationProperty);
			NotificationBatch.Action(View.Container, View.FloatGroup, FloatGroup.FloatSizeProperty);
		}
	}
	public class AutoHideViewResizingListener : ResizingListener {
		BoundsHelper boundsHelper;
		IResizingPreviewHelper resizeHelper;
		public AutoHideView View {
			get { return ServiceProvider as AutoHideView; }
		}
		bool ShowPreview;
		Size minSize = new Size(50, 25);
		public override bool CanDrag(Point point, ILayoutElement element) {
			Point screenPoint = View.ClientToScreen(point);
			Rect resizeBounds = View.Container.GetAutoHideResizeBounds();
			return resizeBounds.Contains(screenPoint);
		}
		public override void OnBegin(Point point, ILayoutElement element) {
			boundsHelper = new BoundsHelper(View, element, minSize);
			View.RootUIElement.SetValue(FrameworkElement.CursorProperty, boundsHelper.GetCursor());
			ShowPreview = !View.Container.RedrawContentWhenResizing;
			if(ShowPreview) {
				resizeHelper = new AutoHideResizingPreviewHelper(View) { MinSize = minSize, BoundsHelper = boundsHelper };
				resizeHelper.InitResizing(point, element);
			}
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			if(ShowPreview)
				resizeHelper.Resize(point);
			else {
				Point screenPoint = View.ClientToScreen(point);
				View.SetPanelBounds(boundsHelper.CalcBounds(screenPoint));
			}
		}
		public override void OnCancel() {
			if(ShowPreview)
				resizeHelper.EndResizing();
			View.RootUIElement.ClearValue(FrameworkElement.CursorProperty);
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			if(ShowPreview) {
				Point screenPoint = View.ClientToScreen(point);
				View.SetPanelBounds(boundsHelper.CalcBounds(screenPoint));
				resizeHelper.EndResizing();
			}
			RaiseDockOperationCompleted(element);
			View.RootUIElement.ClearValue(FrameworkElement.CursorProperty);
		}
		void RaiseDockOperationCompleted(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			View.Container.RaiseDockOperationCompletedEvent(DockOperation.Resize, item);
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			return true;
		}
	}
	public class AutoHideViewReorderingListener : LayoutViewReorderingListener {
		public override bool CanDrag(Point point, ILayoutElement element) {
			return new DockLayoutElementDragInfo(View, point, element).AcceptHide();
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			return new DockLayoutElementDragInfo(View, point, element).AcceptHide();
		}
	}
	public class LayoutViewReorderingListener : ReorderingListener {
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		InsertionHelper insertionHelper;
		public LayoutViewReorderingListener() {
			insertionHelper = new InsertionHelper();
		}
		public override void OnEnter() {
			base.OnEnter();
			ILayoutElement dragElement = View.Adapter.DragService.DragItem ?? View.Container.LinkedDragService.Return(x => x.DragItem, () => null);
			FloatingView view = View.Adapter.GetView(dragElement) as FloatingView;
			if(view != null) view.EnterReordering();
			UpdateHints(new DockLayoutElementDragInfo(View, lastPoint, dragElement), true);
			IsStartedAsReordering = false;
		}
		void UpdateHints(DockLayoutElementDragInfo dragInfo, bool forceUpdateAdornerBounds = false) {
			if(KeyHelper.IsCtrlPressed) return;
			View.AdornerHelper.UpdateTabHeadersHint(dragInfo, forceUpdateAdornerBounds);
		}
		public override void OnLeave() {
			View.AdornerHelper.BeginHideAdornerWindowAndResetTabHeadersHints();
			ILayoutElement dragElement = View.Adapter.DragService.DragItem;
			FloatingView view = View.Adapter.GetView(dragElement) as FloatingView;
			if(view != null) view.LeaveReordering();
			IsStartedAsReordering = false;
		}
		public override void OnCancel() {
			View.AdornerHelper.ResetTabHeadersHints();
			View.AdornerHelper.TryHideAdornerWindow();
			IsStartedAsReordering = false;
		}
		Point lastPoint;
		bool IsStartedAsReordering;
		public override void OnDragging(Point point, ILayoutElement element) {
			if(IsStartedAsReordering) {
				DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
				InsertTabPageCore(dragInfo);
			}
			else {
				UpdateHints(new DockLayoutElementDragInfo(View, point, element));
				UpdateDragCursor(element, point);
			}
			lastPoint = point;
		}
		protected void UpdateDragCursor(ILayoutElement element, Point point) {		   
			ICustomizationController controller = View.Container.CustomizationController;
			if(!controller.IsDragCursorVisible) return;
			Point screenPoint = View.ClientToScreen(point);
			controller.SetDragCursorPosition(screenPoint);
		}
		public override void OnBegin(Point point, ILayoutElement element) {
			base.OnBegin(point, element);
			IsStartedAsReordering = true;
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			if(!IsStartedAsReordering) {
				DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
				InsertTabPageCore(dragInfo);
				View.AdornerHelper.ResetDockingHints();
				View.AdornerHelper.ResetTabHeadersHints();
				View.AdornerHelper.ResetDragVisualization();
			}
			View.AdornerHelper.TryHideAdornerWindow();
			IsStartedAsReordering = false;
		}
		public override bool CanDrag(Point point, ILayoutElement element) {
			DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
			var hit = View.AdornerHelper.GetHitInfo(dragInfo.Point);
			return dragInfo.AcceptFill() && (hit == null || !hit.InHint);
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
			var hit = View.AdornerHelper.GetHitInfo(dragInfo.Point);
			return dragInfo.AcceptFill() && (hit == null || !hit.InButton);
		}
		protected bool CanInsert(BaseLayoutItem item, Point pt, BaseLayoutItem target, DockType type) {
			if(!insertionHelper.CanInsert()) return false;
			bool isTabHintEnabled = (IsStartedAsReordering && item.Parent == target) || View.AdornerHelper.GetTabHeadersAdorner().IsTabHeaderHintEnabled;
			return isTabHintEnabled && !RaiseItemDocking(DockLayoutManager.DockItemEndDockingEvent, item, pt, target, type, false);
		}
		protected bool RaiseItemDocking(RoutedEvent e, BaseLayoutItem item, Point pt, BaseLayoutItem target, DockType dockType, bool isHiding) {
			return View.Container.RaiseItemDockingEvent(e, item, pt, target, dockType, isHiding);
		}
		protected void InsertTabPageCore(DockLayoutElementDragInfo dragInfo) {
			IDockLayoutContainer container = dragInfo.DropTarget as IDockLayoutContainer;
			if(dragInfo.Item == null || container == null) return;
			insertionHelper.Update(dragInfo, IsStartedAsReordering);
			if(!CanInsert(dragInfo.Item, dragInfo.Point, container.Item, DockType.Fill)) {
				ILayoutElement dragElement = View.Adapter.DragService.DragItem;
				FloatingView view = View.Adapter.GetView(dragElement) as FloatingView;
				if(view != null) view.LeaveReordering();
				return;
			}
			if(View.Container.DockController.Insert((LayoutGroup)container.Item, dragInfo.Item, insertionHelper.GetInsertionIndex(), IsStartedAsReordering)) {
				View.Container.DockController.Activate(dragInfo.Item);
			}
		}
		public override void OnComplete() {
			base.OnComplete();
			View.AdornerHelper.ResetTabHeadersHints();
			View.AdornerHelper.TryHideAdornerWindow();
			IsStartedAsReordering = false;
		}
		class InsertionHelper {
			public void Update(DockLayoutElementDragInfo dragInfo, bool isStartedAsReordering) {
				IDockLayoutContainer container = dragInfo.DropTarget as IDockLayoutContainer;
				if(dragInfo.Item == null && container == null) return;
				_IsHorizontal = container.IsHorizontalHeaders;
				InsertionIndex = new TabHeaderInsertHelper(container, dragInfo.Point, dragInfo.Item.Parent != container.Item).InsertIndex;
				Point = dragInfo.Point;
				_IsStartedAsReordering = isStartedAsReordering;
				currentIndex = (container.Item as LayoutGroup).Return(x => x.TabIndexFromItem(dragInfo.Item), () => -1);
			}
			public int GetInsertionIndex() {
				return CanInsert() ? InsertionIndex : -1;
			}
			public bool CanInsert() {
				return !_IsStartedAsReordering || (Math.Sign(diffIndex) == Math.Sign(diffPoint) && Math.Abs(diffPoint) > 0) && InsertionIndex != currentIndex;
			}
			private bool _IsStartedAsReordering;
			private bool _IsHorizontal;
			private int _InsertionIndex;
			private int currentIndex;
			private Point _Point;
			double diffPoint = 0;
			double diffIndex = 0;
			public Point Point {
				get { return _Point; }
				private set {
					if(_Point == value) return;
					diffPoint = _IsHorizontal ? Point.X - value.X : Point.Y - value.Y;
					_Point = value;
				}
			}
			public int InsertionIndex {
				get { return _InsertionIndex; }
				private set {
					if(_InsertionIndex == value) return;
					diffIndex = _InsertionIndex - value;
					_InsertionIndex = value;
				}
			}
		}
	}
	public class LayoutViewMDIReorderingListener : ReorderingListener {
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		MDILocationHelper helper;
		MDIDocumentElement document;
		public override void OnBegin(Point point, ILayoutElement element) {
			document = (MDIDocumentElement)element;
			helper = new MDILocationHelper(View, document);
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			Point screenPoint = View.ClientToScreen(point);
			Point mdiLocation = helper.CalcLocation(screenPoint);
			DocumentPanel.SetMDILocation(document.Item, mdiLocation);
			if(((DocumentPanel)document.Item).IsMinimized)
				((DocumentPanel)document.Item).MinimizeLocation = mdiLocation;
			NotificationBatch.Action(View.Container, document.Item, DocumentPanel.MDILocationProperty);
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			return true;
		}
		public override void OnCancel() {
			helper = null;
			document = null;
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			RaiseDockOperationCompleted(element);
			helper = null;
			document = null;
		}
		void RaiseDockOperationCompleted(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			View.Container.RaiseDockOperationCompletedEvent(DockOperation.Move, item);
		}
	}
	public class LayoutViewMDIResizingListener : ResizingListener {
		static readonly Size DefaultMinSize = new Size(82, 42);
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		bool ShowPreview;
		MDIBoundsHelper boundsHelper;
		MDIDocumentElement document;
		IResizingPreviewHelper resizeHelper;
		public override void OnBegin(Point point, ILayoutElement element) {
			document = (MDIDocumentElement)element;
			Size minSize = MathHelper.MeasureMinSize(new Size[] { DefaultMinSize, document.Item.ActualMinSize });
			Size maxSize = document.Item.ActualMaxSize;
			boundsHelper = new MDIBoundsHelper(View, document, minSize, maxSize);
			View.RootUIElement.SetValue(FrameworkElement.CursorProperty, boundsHelper.GetCursor());
			ShowPreview = !View.Container.RedrawContentWhenResizing;
			if(ShowPreview) {
				resizeHelper = new MDIDocumentResizingPreviewHelper(View) { MinSize = minSize, MaxSize = maxSize };
				resizeHelper.InitResizing(point, element);
			}
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			if(ShowPreview)
				resizeHelper.Resize(point);
			else
				ResizeCore(point);
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			return true;
		}
		public override void OnCancel() {
			if(ShowPreview)
				resizeHelper.EndResizing();
			Reset();
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			if(ShowPreview) {
				ResizeCore(point);
				resizeHelper.EndResizing();
			}
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			View.Container.RaiseDockOperationCompletedEvent(DockOperation.Resize, item);
			Reset();
		}
		void Reset() {
			View.RootUIElement.ClearValue(FrameworkElement.CursorProperty);
			boundsHelper = null;
			document = null;
		}
		void ResizeCore(Point point) {
			Point screenPoint = View.ClientToScreen(point);
			Rect rect = boundsHelper.CalcBounds(screenPoint);
			DocumentPanel.SetMDISize(document.Item, rect.Size());
			DocumentPanel.SetMDILocation(document.Item, rect.Location());
			NotificationBatch.Action(View.Container, document.Item, DocumentPanel.MDILocationProperty);
			NotificationBatch.Action(View.Container, document.Item, DocumentPanel.MDISizeProperty);
		}
	}
	public class LayoutViewClientDraggingListener : ClientDraggingListener {
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		public override void OnBegin(Point point, ILayoutElement element) {
			Point screenPoint = View.ClientToScreen(point);
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			View.Container.CustomizationController.ShowDragCursor(screenPoint, item);
			View.Adapter.SelectionService.ClearSelection(View);
			View.AdornerHelper.TryShowAdornerWindow(true);
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			UpdateDragCursor(element, point);
			if(IsLayoutPanelWithRestrictedFloating(element)) {
				View.AdornerHelper.UpdateDockingHints(new DockLayoutElementDragInfo(View, point, element));
			}
			else {
				Customization.DragInfo info = DockLayoutElementDragInfo.CalcDragInfo(View, point, element);
				View.Container.CustomizationController.UpdateDragInfo(info);
			}
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			if(IsLayoutPanelWithRestrictedFloating(element))
				return new DockingHelper(View).CanDrop(point, element);
			else
				return new MovingHelper(View).CanMove(point, element);
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			ResetHints();
			ResetVisualization();
			if(IsLayoutPanelWithRestrictedFloating(element))
				new DockingHelper(View).Drop(point, element);
			else
				new MovingHelper(View).Move(point, element);
		}
		public override void OnEnter() {
			View.AdornerHelper.TryShowAdornerWindow(true);
		}
		public override void OnLeave() {
			View.AdornerHelper.ResetDockingHints();
			View.AdornerHelper.BeginHideAdornerWindowAndResetDockingHints();
		}
		public override void OnCancel() {
			View.Adapter.SelectionService.ClearSelection(View);
			ResetHints();
			ResetVisualization();
		}
		protected void ResetHints() {
			View.AdornerHelper.ResetDockingHints();
			View.AdornerHelper.ResetTabHeadersHints();
		}
		protected void ResetVisualization() {
			View.Container.CustomizationController.HideDragCursor();
			View.Container.CustomizationController.UpdateDragInfo(null);
		}
		protected void UpdateDragCursor(ILayoutElement element, Point point) {
			ICustomizationController controller = View.Container.CustomizationController;
			Point screenPoint = View.ClientToScreen(point);
			if(!controller.IsDragCursorVisible)
				controller.ShowDragCursor(screenPoint, ((IDockLayoutElement)element).Item);
			else controller.SetDragCursorPosition(screenPoint);
		}
		static bool IsLayoutPanelWithRestrictedFloating(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			return (item is LayoutPanel || item is TabbedGroup) && !item.AllowFloat;
		}
	}
	public class LayoutViewNonClientDraggingListener : NonClientDraggingListener {
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			Point screenPoint = View.ClientToScreen(point);
			View.Container.CustomizationController.SetDragCursorPosition(screenPoint);
		}
		public override void OnCancel() {
			View.Container.CustomizationController.HideDragCursor();
			ResetVisualization();
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			View.Container.CustomizationController.HideDragCursor();
			ResetVisualization();
		}
		protected void ResetVisualization() {
			View.Container.CustomizationController.HideDragCursor();
			View.Container.CustomizationController.UpdateDragInfo(null);
		}
	}
}

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

using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Core;
#if !SILVERLIGHT
using SWC = System.Windows.Controls;
#else
using SWC = DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Core.WPFCompatibility;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Docking.Platform {
	class LocationHelper {
		Point startLocation, startPoint;
		public LocationHelper(IView view, ILayoutElement element) {
			this.startLocation = view.ClientToScreen(element.Location);
			this.startPoint = view.Adapter.DragService.DragOrigin;
		}
		public Point CalcLocation(Point screenPoint) {
			double dx = screenPoint.X - startPoint.X;
			double dy = screenPoint.Y - startPoint.Y;
			return new Point(startLocation.X + dx, startLocation.Y + dy);
		}
	}
	class MDILocationHelper {
		Point startLocation, startPoint;
		public MDILocationHelper(IView view, IDockLayoutElement element) {
			this.startLocation = DocumentPanel.GetMDILocation(element.Item);
			this.startPoint = view.Adapter.DragService.DragOrigin;
		}
		public Point CalcLocation(Point screenPoint) {
			double dx = screenPoint.X - startPoint.X;
			double dy = screenPoint.Y - startPoint.Y;
			return new Point(startLocation.X + dx, startLocation.Y + dy);
		}
	}
	class BoundsHelper {
		internal Rect startRect;
		internal Point startPoint;
		Rect effectiveRect;
		Thickness margin;
		Size MinSize;
		Size MaxSize;
		bool rightToLeft;
		public BoundsHelper(IView view, ILayoutElement element, Size minSize, Size maxSize)
			: this(view, element, minSize) {
			MaxSize = maxSize;
			MaxSize.Width -= margin.Left + margin.Right;
			MaxSize.Height -= margin.Top + margin.Bottom;
		}
		public BoundsHelper(IView view, ILayoutElement element, Size minSize) {
			MinSize = minSize;
			rightToLeft = ((LayoutView)view).Container.FlowDirection == FlowDirection.RightToLeft;
			this.startRect = ElementHelper.GetScreenRect(view, element);
			this.startPoint = view.Adapter.DragService.DragOrigin;
			effectiveRect = GetEffectiveRect(element, startRect);
			margin = GetMargin(effectiveRect, startRect);
			MinSize.Width -= margin.Left + margin.Right;
			MinSize.Height -= margin.Top + margin.Bottom;
		}
		public Rect CalcBounds(Point screenPoint) {
			return GetRealRect(Layout.Core.ResizeHelper.CalcResizing(effectiveRect, startPoint, screenPoint, MinSize, MaxSize), margin);
		}
		public System.Windows.Input.Cursor GetCursor() {
			return Layout.Core.ResizeHelper.GetResizeCursor(effectiveRect, startPoint, rightToLeft);
		}
		public SizingAction GetSizingAction() {
			return Layout.Core.ResizeHelper.GetSizingAction(effectiveRect, startPoint, rightToLeft);
		}
		static Rect GetEffectiveRect(ILayoutElement element, Rect real) {
			if(element is FloatPanePresenterElement) 
				RectHelper.Inflate(ref real, -5, -5); 
			return real;
		}
		static Rect GetRealRect(Rect effective, Thickness margin) {
			return new Rect(effective.Left - margin.Left, effective.Top - margin.Top, 
				effective.Width + (margin.Left + margin.Right), effective.Height + (margin.Top + margin.Bottom));
		}
		static Thickness GetMargin(Rect effective, Rect real) {
			return new Thickness(effective.Left - real.Left, effective.Top - real.Top,
				real.Right - effective.Right, real.Bottom - effective.Bottom);
		}
	}
	class MDIBoundsHelper {
		Rect startRect;
		Point startPoint;
		Size MinSize;
		Size MaxSize;
		Point startLocation;
		bool rightToLeft;
		public MDIBoundsHelper(IView view, IDockLayoutElement element, Size minSize) {
			MinSize = minSize;
			rightToLeft = ((LayoutView)view).Container.FlowDirection == FlowDirection.RightToLeft;
			this.startLocation = DocumentPanel.GetMDILocation(element.Item);
			this.startRect = ElementHelper.GetScreenRect(view, element);
			this.startPoint = view.Adapter.DragService.DragOrigin;
		}
		public MDIBoundsHelper(IView view, IDockLayoutElement element, Size minSize, Size maxSize)
			: this(view, element, minSize) {
			MaxSize = maxSize;
		}
		public Rect CalcBounds(Point screenPoint) {
			Rect screenRect = Layout.Core.ResizeHelper.CalcResizing(startRect, startPoint, screenPoint, MinSize, MaxSize);
			double dx = screenRect.X - startRect.X;
			double dy = screenRect.Y - startRect.Y;
			return new Rect(new Point(startLocation.X + dx, startLocation.Y + dy), screenRect.Size());
		}
		public System.Windows.Input.Cursor GetCursor() {
			return Layout.Core.ResizeHelper.GetResizeCursor(startRect, startPoint, rightToLeft);
		}
	}
	class MarginHelper {
		Thickness margin;
		public MarginHelper(LayoutView view) {
			margin = GetContentMargin(view.RootUIElement);
		}
		public Rect Correct(Rect bounds) {
			return new Rect(
					CorrectLocation(margin, bounds.Location()),
					CorrectSize(margin, bounds.Size())
				);
		}
		public Point Correct(Point location) {
			return CorrectLocation(margin, location);
		}
		Thickness GetContentMargin(object element) {
			var presenter = element as VisualElements.FloatPanePresenter.FloatingContentPresenter;
			return (presenter != null) ? presenter.GetFloatingMargin() : new Thickness(0);
		}
		Point CorrectLocation(Thickness margins, Point floatLocation) {
			return new Point(floatLocation.X - margins.Left, floatLocation.Y - margins.Top);
		}
		Size CorrectSize(Thickness margins, Size floatSize) {
			return new Size(floatSize.Width + margins.Left + margins.Right, floatSize.Height + margins.Top + margins.Bottom);
		}
	}
	class HintRectCalculator {
		public static Rect Calc(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) {
			if(hitInfo == null || dragInfo.Item == null) return Rect.Empty;
			if(dragInfo.DropTarget is AutoHideTrayElement) return Rect.Empty;
			if(dragInfo.DropTarget is AutoHidePaneElement) return Rect.Empty;
			return SelectStrategy(hitInfo).Calc(dragInfo, hitInfo);
		}
		static BaseCalcStrategy SelectStrategy(DockHintHitInfo hitInfo) {
			if(hitInfo.IsCenter)
				return new CenterHint();
			if(hitInfo.IsHideButton)
				return new HideHint();
			return new RootHint();
		}
		abstract class BaseCalcStrategy {
			public abstract Rect Calc(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo);
		}
		class RootHint : BaseCalcStrategy {
			public override Rect Calc(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) {
				Rect rootRect = ElementHelper.GetRect(dragInfo.View.LayoutRoot);
				Size hintSize = DockPreviewCalculator.DockPreviewGroup(
						rootRect, dragInfo.Item, dragInfo.Target.GetRoot(), hitInfo.DockType
					);
				return DockHelper.GetDockRect(rootRect, hintSize, hitInfo.DockType);
			}
		}
		class CenterHint : BaseCalcStrategy {
			public override Rect Calc(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) {
				Rect targetRect;
				BaseLayoutItem target = dragInfo.Target;
				if(target.GetIsDocumentHost() && !hitInfo.IsTabButton && hitInfo.DockType != DockType.Fill && ((LayoutGroup)target).HasNotCollapsedItems && target.Parent != null) {
					target = target.Parent;
					IDockLayoutElement element = (IDockLayoutElement)dragInfo.DropTarget;
					targetRect = ElementHelper.GetRect(element.Container);
				}
				else
					targetRect = dragInfo.TargetRect;
				Size hintSize = DockPreviewCalculator.DockPreviewItem(targetRect, dragInfo.Item, target, hitInfo.DockType);
				return DockHelper.GetDockRect(targetRect, hintSize, hitInfo.DockType);
			}
		}
		class HideHint : BaseCalcStrategy {
			public override Rect Calc(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) {
				Rect rootRect = ElementHelper.GetRect(dragInfo.View.LayoutRoot);
				bool fHorz = (hitInfo.DockType.ToOrientation() != System.Windows.Controls.Orientation.Horizontal);
				Size hintSize = new Size(fHorz ? rootRect.Width : 20, fHorz ? 20 : rootRect.Height);
				return DockHelper.GetDockRect(rootRect, hintSize, hitInfo.DockType);
			}
		}
	}
	public class FloatingHelper {
		bool isTabHeader;
		BaseLayoutItem item;
		public FloatingHelper(LayoutView view) {
			View = view;
		}
		public LayoutView View { get; private set; }
		public virtual IView GetFloatingView(ILayoutElement element) {
			IDockLayoutElement dockElement = (IDockLayoutElement)element;
			item = dockElement.Item;
			isTabHeader = dockElement.IsPageHeader;
			DockLayoutManager manager = View.Container;
			try {
				manager.BeginFloating();
				FloatGroup floatGroup = manager.DockController.Float(item);
				return manager.GetView(floatGroup) as FloatingView;
			}
			finally {
				manager.EndFloating();
			}
		}
		public virtual void InitFloatingView(IView floatingView, Rect itemScreenRect, Rect itemContainerScreenRect) {
			FloatingView view = (FloatingView)floatingView;
			Size layoutSize = isTabHeader ? itemContainerScreenRect.Size() : itemScreenRect.Size();
			IView parentView = View.IsDisposing ? floatingView : View;
			Point startPoint = MathHelper.IsEmpty(parentView.Adapter.DragService.DragOrigin) ? 
				new Point() : parentView.Adapter.DragService.DragOrigin;
			Rect screenRect = new Rect(itemScreenRect.Location(), item.CheckSize(layoutSize));
			screenRect = Check(screenRect, startPoint);
			view.SetFloatingBounds(new MarginHelper(view).Correct(screenRect));
			NotificationBatch.Action(view.Container, view.FloatGroup);
			Clear();
		}
		void Clear() {
			item = null;
			View = null;
		}
		internal virtual Rect Check(Rect screenRect, Point startPoint) {
			if(!screenRect.Contains(startPoint))
				RectHelper.Offset(ref screenRect, startPoint.X - 15 - screenRect.X, startPoint.Y - 15 - screenRect.Y);
			return screenRect;
		}
	}
	public class AutoHideFloatingHelper : FloatingHelper {
		Size size;
		public AutoHideFloatingHelper(LayoutView view)
			: base(view) {
		}
		public override IView GetFloatingView(ILayoutElement element) {
			VisualElements.AutoHidePane pane = (View.RootUIElement as VisualElements.AutoHideTray).Panel;
			bool horz = (View.RootUIElement as VisualElements.AutoHideTray).IsHorizontal;
			size = new Size(horz ? pane.RenderSize.Width : pane.Size, horz ? pane.Size : pane.RenderSize.Height);
			return base.GetFloatingView(element);
		}
		public override void InitFloatingView(IView floatingView, Rect itemScreenRect, Rect itemContainerScreenRect) {
			Point location = itemScreenRect.Location();
			if(View.Adapter.DragService.DragItem is AutoHidePaneHeaderItemElement)
				location = new Point(View.Adapter.DragService.DragOrigin.X - 15, View.Adapter.DragService.DragOrigin.Y - 15);
			itemScreenRect = itemContainerScreenRect = new Rect(location, size);
			base.InitFloatingView(floatingView, itemScreenRect, itemContainerScreenRect);
		}
	}
	class MovingHelper {
		public MovingHelper(LayoutView view) {
			View = view;
		}
		public LayoutView View { get; private set; }
		public bool CanMove(Point point, ILayoutElement element) {
			DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
			return (dragInfo.Item != null) && dragInfo.AcceptDragDrop();
		}
		public void Move(Point point, ILayoutElement element) {
			View.Adapter.SelectionService.ClearSelection(View);
			DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
			MoveItemCore(dragInfo);
		}
		protected bool MoveItemCore(DockLayoutElementDragInfo dragInfo) {
			bool isFixedItem = (dragInfo.Element is HiddenItemElement && dragInfo.Item is FixedItem);
			return View.Container.LayoutController.Move(
					isFixedItem ? FixedItemFactory.CreateFixedItem(dragInfo.Item as FixedItem) : dragInfo.Item,
					dragInfo.Target, dragInfo.MoveType, dragInfo.InsertIndex
				);
		}
		public static readonly int NoBorderMarginHorizontal = -3;
		public static readonly int NoBorderMarginVertical = -3;
	}
	class DockingHelper {
		public DockingHelper(LayoutView view) {
			View = view;
		}
		public LayoutView View { get; private set; }
		public bool CanDrop(Point point, ILayoutElement element) {
			DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
			DockingHintAdorner adorner = View.AdornerHelper.GetDockingHintAdorner();
			if(adorner == null || dragInfo.Item == null) return false;
			var hit = adorner.HitTest(point);
			return (hit.InButton) && dragInfo.AcceptDocking(hit);
		}
		public void Drop(Point point, ILayoutElement element) {
			DockLayoutElementDragInfo dragInfo = new DockLayoutElementDragInfo(View, point, element);
			DockingHintAdorner adorner = View.AdornerHelper.GetDockingHintAdorner();
			DockHintHitInfo hitInfo = adorner.HitTest(point);
			if(hitInfo.IsCenter) {
				if(hitInfo.IsTabButton) {
					DockItemAsTabCore(dragInfo.Item, dragInfo.Point, dragInfo.Target, hitInfo.DockType);
				}
				else {
					DockItemCore(dragInfo.Item, dragInfo.Point, dragInfo.Target, hitInfo.DockType);
				}
			}
			else {
				bool fHideToNewGroup = (View.Type == HostType.AutoHide) && (dragInfo.DropTarget is AutoHideTrayElement);
				if(hitInfo.IsHideButton || fHideToNewGroup) {
					HideItemCore(dragInfo.Item, dragInfo.Point, fHideToNewGroup ?
					(dragInfo.DropTarget as AutoHideTrayElement).Tray.DockType : hitInfo.Dock);
				}
				else {
					DockItemCore(dragInfo.Item, dragInfo.Point, dragInfo.Target.GetRoot(), hitInfo.DockType);
				}
			}
			View.AdornerHelper.ResetDockingHints();
			View.AdornerHelper.TryHideAdornerWindow();
		}
		protected bool HideItemCore(BaseLayoutItem item, Point pt, SWC.Dock dock) {
			bool canHide = !View.Container.RaiseItemDockingEvent(
					DockLayoutManager.DockItemEndDockingEvent, item, pt, null, dock.ToDockType(), true
				);
			return canHide && View.Container.DockController.Hide(item, dock);
		}
		protected bool DockItemCore(BaseLayoutItem item, Point pt, BaseLayoutItem target, DockType type) {
			bool canDock = !View.Container.RaiseItemDockingEvent(
					DockLayoutManager.DockItemEndDockingEvent, item, pt, target, type, false
				);
			return canDock && View.Container.DockController.Dock(item, target, type);
		}
		protected bool DockItemAsTabCore(BaseLayoutItem item, Point pt, BaseLayoutItem target, DockType type) {
			bool canDock = !View.Container.RaiseItemDockingEvent(
					DockLayoutManager.DockItemEndDockingEvent, item, pt, target, type, false
				);
			return canDock && View.Container.DockController.DockAsDocument(item, target, type);
		}
	}
}

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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Base;
namespace DevExpress.Xpf.Docking.Platform {
	public interface IDragSource {
		bool AcceptDropTarget(DockLayoutElementDragInfo dragInfo);
		bool AcceptDockTarget(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo);
	}
	public interface IDropTarget {
		bool AcceptFill(DockLayoutElementDragInfo dragInfo);
		bool AcceptDockSource(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo);
		bool AcceptDragSource(DockLayoutElementDragInfo dragInfo);
	}
	public class DockLayoutElementDragInfo {
#if DEBUGTEST
		internal DockLayoutElementDragInfo(IDockLayoutElement dragElement, IDockLayoutElement dropTarget) {
			Element = CheckDragElement((IDockLayoutElement)dragElement);
			Item = Element.Item;
			DropTarget = dropTarget;
			DragSource = dragElement;
			Target = dropTarget.Item;
		}
#endif
		public DockLayoutElementDragInfo(IView view, Point point, ILayoutElement dragElement) {
			AssertionException.IsNotNull(view);
			AssertionException.Is<IDockLayoutElement>(dragElement);
			View = view;
			Element = CheckDragElement((IDockLayoutElement)dragElement);
			Item = Element.Item;
			HitInfo = View.Adapter.CalcHitInfo(view, point);
			DragSource = dragElement as IDragSource;
			DropTarget = CheckDropTarget();
			Point = CheckClientPoint(point);
			if(DropTarget != null) {
				Target = ((IDockLayoutElement)DropTarget).Item;
				DropInfo = ((IDockLayoutElement)DropTarget).CalcDropInfo(Item, Point);
			}
			if(DropInfo != null) {
				DropType = DropInfo.Type;
				MoveType = DropInfo.MoveType;
				TargetRect = DropInfo.ItemRect;
				InsertIndex = DropInfo.InsertIndex;
			}
		}
		public IView View { get; private set; }
		public Point Point { get; private set; }
		public IDockLayoutElement Element { get; private set; }
		protected LayoutElementHitInfo HitInfo { get; private set; }
		protected BaseDropInfo DropInfo { get; private set; }
		public IDragSource DragSource { get; private set; }
		public IDropTarget DropTarget { get; private set; }
		public BaseLayoutItem Item { get; private set; }
		public BaseLayoutItem Target { get; private set; }
		public DropType DropType { get; private set; }
		public MoveType MoveType { get; private set; }
		public Rect TargetRect { get; private set; }
		public int InsertIndex { get; private set; }
		public bool AcceptSelfDock() {
			return (DropTarget != DragSource) && (DropTarget != null) && Item != Target;
		}
		public bool AcceptFill() {
			if(DragSource == null || DropTarget == null || (!Item.AllowDock || !Target.AllowDockToCurrentItem)) return false;
			return AcceptSelfDock() && DropTarget.AcceptFill(this);
		}
		public bool AcceptHide() {
			if(DragSource == null || DropTarget == null || !Item.AllowHide || LayoutItemsHelper.IsLayoutItem(Item)) return false;
			return AcceptSelfDock() && DropTarget.AcceptDockSource(this, null);
		}
		public bool AcceptDockCenter() {
			if(DragSource == null || DropTarget == null || !Item.AllowDock) return false;
			return DragSource.AcceptDockTarget(this, null) && DropTarget.AcceptDockSource(this, null);
		}
		public bool AcceptDocking(DockHintHitInfo hitInfo) {
			if(hitInfo.DockType == DockType.None || DragSource == null || DropTarget == null) return false;
			if(hitInfo.DockType == DockType.Fill && !DropTarget.AcceptFill(this)) return false;
			if(!hitInfo.IsHideButton && !Item.AllowDock) return false;
			if(hitInfo.IsHideButton && !Item.AllowHide) return false;
			return DragSource.AcceptDockTarget(this, hitInfo) && DropTarget.AcceptDockSource(this, hitInfo);
		}
		public bool AcceptDragDrop() {
			if(DragSource == null || DropTarget == null || DragSource == DropTarget) return false;
			if(!Item.AllowMove || (Item.IsHidden && !Item.AllowRestore)) return false;
			return DragSource.AcceptDropTarget(this) && DropTarget.AcceptDragSource(this);
		}
		public bool CanDockToSide {
			get {
				if(!Item.AllowDock) return false;
				DockLayoutManager manager = ((LayoutView)View).Container;
				bool isFloatingDocumentHost = Item is FloatGroup && ((FloatGroup)Item).IsDocumentHost;
				if(Item.ItemType == LayoutItemType.Document && manager.DockingStyle != DockingStyle.Default || Item.GetRoot().GetIsDocumentHost()) return false;
				if(manager.DockingStyle != DockingStyle.Default && isFloatingDocumentHost) return false;
				return !LayoutItemsHelper.IsEmptyLayoutGroup(Target.GetRoot());
			}
		}
		public bool CanDockToTab {
			get {
				if(!Item.AllowDock) return false;
				return Target is LayoutGroup && Target.GetIsDocumentHost() && ((LayoutGroup)Target).HasNotCollapsedItems;
			}
		}
		public bool CanDock {
			get {
				if(!Item.AllowDock) return false;
				DockLayoutManager manager = ((LayoutView)View).Container;
				bool isDocument = Item.ItemType == LayoutItemType.Document;
				bool isFloatingDocumentHost = Item is FloatGroup && ((FloatGroup)Item).IsDocumentHost;
				if(isDocument || isFloatingDocumentHost) {
					if(manager.DockingStyle != DockingStyle.Default) return false;
					DocumentGroup documentHost = Target as DocumentGroup;
					if(documentHost!=null && !documentHost.HasNotCollapsedItems) return false;
				}
				return (!LayoutItemsHelper.IsEmptyLayoutGroup(Target) || Target.GetIsDocumentHost());
			}
		}
		public bool CanHide {
			get {
				DockLayoutManager manager = ((LayoutView)View).Container;
				bool isFloatingDocumentHost = Item is FloatGroup && ((FloatGroup)Item).IsDocumentHost;
				return Item.AllowHide && !Item.GetRoot().GetIsDocumentHost() &&
					!(isFloatingDocumentHost && manager.DockingStyle != DockingStyle.Default);
			}
		}
		public bool CanFill { get { return Item.AllowDock; } }
		bool isSplitter;
		Point splitterItemPoint;
		public Point CheckClientPoint(Point point) {
			return isSplitter ? splitterItemPoint : point;
		}
		public IDockLayoutElement CheckDragElement(IDockLayoutElement dragElement) {
			return dragElement.CheckDragElement();
		}
		public IDropTarget CheckDropTarget() {
			IDockLayoutElement element = HitInfo.Element as IDockLayoutElement;
			if(element == null) return null;
			if(element.Type == LayoutItemType.Splitter) {
				ILayoutContainer parent = element.Container;
				LayoutGroup group = element.Item as LayoutGroup;
				int index = group.ItemsInternal.IndexOf(element.Element);
				BaseLayoutItem item = group.ItemsInternal[index - 1] as BaseLayoutItem;
				foreach(IDockLayoutElement el in parent.Items) {
					if(el.Item == item) {
						element = el;
						isSplitter = true;
						if(group.Orientation == System.Windows.Controls.Orientation.Horizontal)
							splitterItemPoint = new Point(element.Location.X + element.Size.Width - 1, HitInfo.HitPoint.Y);
						else
							splitterItemPoint = new Point(HitInfo.HitPoint.X, (element.Location.Y + element.Size.Height - 1));
						break;
					}
				}
			}
			IDropTarget result = null;
			if(!IsControlItemsHost(element) || LayoutItemsHelper.IsLayoutItem(Item) ||
				(Item.ItemType == LayoutItemType.Group && Item.GetRoot().IsLayoutRoot)) {
				result = CheckNestedItem(element) as IDropTarget;
			}
			if(result == null)
				result = FindNonControlItemHostTarget(element) as IDropTarget;
			IDockLayoutElement target = result as IDockLayoutElement;
			if(target != null && target.Item != null && target.Item.Parent != null) {
				LayoutGroup targetParent = target.Item.Parent;
				if(LayoutItemsHelper.IsLayoutItem(target.Item) && targetParent.GroupBorderStyle == GroupBorderStyle.NoBorder && !targetParent.IsLayoutRoot) {
					Rect rect = ElementHelper.GetRect(result as ILayoutElement);
					RectHelper.Inflate(ref rect, MovingHelper.NoBorderMarginHorizontal, MovingHelper.NoBorderMarginVertical);
					if(!rect.Contains(HitInfo.HitPoint)) {
						result = target.Container as IDropTarget;
					}
				}
			}
			return result;
		}
		bool IsControlItemsHost(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			return item != null && (item.IsControlItemsHost || LayoutItemsHelper.IsLayoutItem(item));
		}
		ILayoutElement CheckNestedItem(ILayoutElement element) {
			if(element.Container is TabbedLayoutGroupElement)
				return element.Container;
			if(element.Container is TabbedPaneElement)
				return element.Container;
			if(element.Container is DocumentPaneElement)
				return element.Container;
			if(element.Container is AutoHideTrayHeadersGroupElement)
				return element.Container;
			return element;
		}
		ILayoutElement FindNonControlItemHostTarget(ILayoutElement element) {
			while(element.Parent != null) {
				if(!IsControlItemsHost(element.Parent)) break;
				element = element.Parent;
			}
			return CheckNestedItem(element);
		}
		public static Customization.DragInfo CalcDragInfo(IView view, Point point, ILayoutElement element) {
			DockLayoutElementDragInfo viewDragInfo = new DockLayoutElementDragInfo(view, point, element);
			if(viewDragInfo.Item != null) {
				bool allow = viewDragInfo.AcceptDragDrop();
				return new Customization.DragInfo(viewDragInfo.Item, viewDragInfo.Target, allow ? viewDragInfo.DropType : DropType.None);
			}
			return null;
		}
	}
}

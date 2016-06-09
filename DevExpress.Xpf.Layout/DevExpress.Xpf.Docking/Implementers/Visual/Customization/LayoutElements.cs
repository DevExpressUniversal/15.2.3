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
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Dragging;
namespace DevExpress.Xpf.Docking.Platform {
	public class CustomizationControlElement : DockLayoutContainer {
		public CustomizationControlElement(UIElement uiElement, UIElement view) :
			base(LayoutItemType.CustomizationControl, uiElement, view) {
		}
		public override ILayoutElementBehavior GetBehavior() { return null; }
		public override BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point) { return null; }
	}
	public class FixedItemElement : DockLayoutElement {
		public FixedItemElement(UIElement uiElement, UIElement view) :
			base(LayoutItemType.FixedItem, uiElement, view) {
		}
		public override bool AcceptDropTarget(DockLayoutElementDragInfo dragInfo) {
			if(dragInfo.Target == null) return false;
			LayoutItemType type = dragInfo.Target.ItemType;
			return LayoutItemsHelper.IsLayoutItem(dragInfo.Target) || type == LayoutItemType.Group;
		}
		public override BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point) {
			return DropTypeHelper.CalcSideDropInfo(ElementHelper.GetRect(this), point, 0.25);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new FixedItemElementHitInfo(pt, this, LayoutControllerHelper.GetLayoutController(Element));
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new FixedItemElementBehavior(this);
		}
	}
	public class LayoutSplitterElement : DockLayoutElement {
		public LayoutSplitterElement(UIElement uiElement, UIElement view) :
			base(LayoutItemType.LayoutSplitter, uiElement, view) {
		}
		public override bool AcceptDropTarget(DockLayoutElementDragInfo dragInfo) {
			if(dragInfo.Target == null) return false;
			LayoutItemType type = dragInfo.Target.ItemType;
			return LayoutItemsHelper.IsLayoutItem(dragInfo.Target) || type == LayoutItemType.Group;
		}
		public override BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point) {
			return DropTypeHelper.CalcSideDropInfo(ElementHelper.GetRect(this), point, 0.25);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new LayoutSplitterElementHitInfo(pt, this, LayoutControllerHelper.GetLayoutController(Element));
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new LayoutSplitterElementBehavior(this);
		}
		public override bool AcceptDragSource(DockLayoutElementDragInfo dragInfo) {
			return false;
		}
	}
	public class HiddenItemsListElement : DockLayoutContainer {
		public HiddenItemsListElement(UIElement uiElement, UIElement view) :
			base(LayoutItemType.HiddenItemsList, uiElement, view) {
		}
		public override ILayoutElementBehavior GetBehavior() { return null; }
		public override BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point) { return null; }
	}
	public class HiddenItemElement : DockLayoutElement {
		public HiddenItemElement(UIElement uiElement, UIElement view) :
			base(LayoutItemType.HiddenItem, uiElement, view) {
		}
		protected override BaseLayoutItem GetItem(UIElement uiElement) {
			return ((HiddenItem)uiElement).Item;
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new HiddenItemElementBehavior(this);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new HiddenItemElementHitInfo(pt, this, LayoutControllerHelper.GetLayoutController(Element));
		}
		public override bool AcceptDropTarget(DockLayoutElementDragInfo dragInfo) {
			if(dragInfo.Target == null) return false;
			LayoutItemType type = dragInfo.Target.ItemType;
			return LayoutItemsHelper.IsLayoutItem(dragInfo.Target) || type == LayoutItemType.Group;
		}
	}
	public class TreeItemElement : DockLayoutElement {
		public TreeItemElement(UIElement uiElement, UIElement view) :
			base(LayoutItemType.TreeItem, uiElement, view) {
		}
		protected override BaseLayoutItem GetItem(UIElement uiElement) {
			return ((TreeItem)uiElement).Item;
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new TreeItemElementBehavior(this);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new TreeItemElementHitInfo(pt, this, LayoutControllerHelper.GetLayoutController(Element));
		}
		public override bool AcceptDropTarget(DockLayoutElementDragInfo dragInfo) {
			if(dragInfo.Target == null) return false;
			if(dragInfo.Item is LayoutGroup && LayoutItemsHelper.IsParent(dragInfo.Target, dragInfo.Item)) return false;
			LayoutItemType type = dragInfo.Target.ItemType;
			return  LayoutItemsHelper.IsLayoutItem(dragInfo.Target) || type == LayoutItemType.Group;
		}
		public override bool AcceptDragSource(DockLayoutElementDragInfo dragInfo) {
			if(dragInfo.Item == dragInfo.Target) return false;
			return base.AcceptDragSource(dragInfo);
		}
		public override BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point) {
			switch(Item.ItemType) {
				case LayoutItemType.ControlItem:
				case LayoutItemType.FixedItem:
				case LayoutItemType.LayoutSplitter:
				case LayoutItemType.Label:
				case LayoutItemType.EmptySpaceItem:
				case LayoutItemType.Separator:
					return DropTypeHelper.CalcCenterDropInfo(ElementHelper.GetRect(this), point, 0.0);
				case LayoutItemType.Group:
					return DropTypeHelper.CalcCenterDropInfo(ElementHelper.GetRect(this), point, 0.4);
			}
			return base.CalcDropInfo(item, point);
		}
	}
	public class BaseCustomizationFormElementHitInfo : DockLayoutElementHitInfo {
		public BaseCustomizationFormElementHitInfo(Point pt, DockLayoutElement element, ILayoutController controller)
			: base(pt, element) {
			Controller = controller;
		}
		public ILayoutController Controller { get; private set; }
		public override bool InDragBounds { get { return IsCustomization && InBounds; } }
		public override bool InMenuBounds { get { return InBounds; } }
	}
	public class FixedItemElementHitInfo : BaseCustomizationFormElementHitInfo {
		public FixedItemElementHitInfo(Point pt, FixedItemElement element, ILayoutController controller)
			: base(pt, element, controller) {
		}
	}
	public class LayoutSplitterElementHitInfo : BaseCustomizationFormElementHitInfo {
		public LayoutSplitterElementHitInfo(Point pt, LayoutSplitterElement element, ILayoutController controller)
			: base(pt, element, controller) {
		}
	}
	public class HiddenItemElementHitInfo : BaseCustomizationFormElementHitInfo {
		public HiddenItemElementHitInfo(Point pt, HiddenItemElement element, ILayoutController controller)
			: base(pt, element, controller) {
		}
		public override bool InMenuBounds {
			get { return InBounds && (!InContent || InContent && IsCustomization); }
		}
	}
	public class TreeItemElementHitInfo : BaseCustomizationFormElementHitInfo {
		public TreeItemElementHitInfo(Point pt, TreeItemElement element, ILayoutController controller)
			: base(pt, element, controller) {
		}
		public override bool InMenuBounds {
			get { return InBounds && (!InContent || InContent && IsCustomization); }
		}
	}
}

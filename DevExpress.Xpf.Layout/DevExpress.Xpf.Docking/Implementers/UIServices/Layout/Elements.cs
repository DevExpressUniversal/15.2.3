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

using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking.Platform {
	public class SplitterElement : DockLayoutElement {
		public SplitterElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.Splitter, uiElement, view) {
		}
		protected override BaseLayoutItem GetItem(UIElement uiElement) {
			return ((Splitter)uiElement).LayoutGroup;
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new SplitterElementHitInfo(pt, this);
		}
		public override bool AllowActivate {
			get { return false; }
		}
	}
	public class DockPaneElement : DockLayoutContainer {
		public DockPaneElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.Panel, uiElement, view) {
		}
		protected override void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			base.CheckAndSetHitTests(hitInfo, hitType);
			hitInfo.CheckAndSetHitTest(HitTestType.PinButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.MaximizeButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.RestoreButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.ExpandButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.CollapseButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.HideButton, hitType, LayoutElementHitTest.ControlBox);
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new DockPaneElementBehavior(this);
		}
		public override ILayoutElement GetDragItem() {
			if(Item.Parent is TabbedGroup) return Container;
			return this;
		}
		public override bool AcceptFill(DockLayoutElementDragInfo dragInfo) {
			BaseLayoutItem item = dragInfo.Item;
			LayoutItemType type = item.ItemType;
			if(item is FloatGroup) return !((FloatGroup)item).IsDocumentHost;
			return type == LayoutItemType.Panel || type == LayoutItemType.TabPanelGroup;
		}
	}
	public class DocumentElement : DockLayoutContainer {
		public DocumentElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.Document, uiElement, view) {
		}
		public override bool AcceptFill(DockLayoutElementDragInfo dragInfo) {
			if(Parent is FloatPanePresenterElement) return false;
			return dragInfo.Item.ItemType == LayoutItemType.Document;
		}
		public override bool AcceptDockSource(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) {
			if(Parent is FloatPanePresenterElement) return false;
			return base.AcceptDockSource(dragInfo, hitInfo);
		}
	}
	public class FloatDocumentElement : DocumentElement {
		public FloatDocumentElement(UIElement uiElement, UIElement view)
			: base(uiElement, view) {
		}
		protected override void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			base.CheckAndSetHitTests(hitInfo, hitType);
			hitInfo.CheckAndSetHitTest(HitTestType.MaximizeButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.RestoreButton, hitType, LayoutElementHitTest.ControlBox);
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new FloatDocumentElementBehavior(this);
		}
		public override bool AcceptFill(DockLayoutElementDragInfo dragInfo) {
			if(Parent is FloatPanePresenterElement) return false;
			return dragInfo.Item.ItemType == LayoutItemType.Document;
		}
	}
	public class MDIDocumentElement : DockLayoutContainer {
		public MDIDocumentElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.Document, uiElement, view) {
		}
		protected override void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			base.CheckAndSetHitTests(hitInfo, hitType);
			hitInfo.CheckAndSetHitTest(HitTestType.MaximizeButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.MinimizeButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.RestoreButton, hitType, LayoutElementHitTest.ControlBox);
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new MDIDocumentElementBehavior(this);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new MDIDocumentElementHitInfo(pt, this);
		}
		public override bool AcceptFill(DockLayoutElementDragInfo dragInfo) {
			return dragInfo.Item.ItemType == LayoutItemType.Document;
		}
	}
	public class GroupPaneElement : DockLayoutContainer {
		public GroupPaneElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.Group, uiElement, view) {
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new GroupPaneElementBehavior(this);
		}
		protected override void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			hitInfo.CheckAndSetHitTest(HitTestType.ExpandButton, hitType, LayoutElementHitTest.ControlBox);
			base.CheckAndSetHitTests(hitInfo, hitType);
		}
		public override bool AcceptDockSource(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) {
			return base.AcceptDockSource(dragInfo, hitInfo) && ((LayoutGroup)Item).AcceptDock;
		}
		public override bool AcceptFill(DockLayoutElementDragInfo dragInfo) {
			LayoutItemType type = dragInfo.Item.ItemType;
			if(Item.IsControlItemsHost) {
				return type != LayoutItemType.Panel && type != LayoutItemType.Document && type != LayoutItemType.TabPanelGroup && type != LayoutItemType.FloatGroup;
			}
			return true;
		}
		public override bool AcceptDropTarget(DockLayoutElementDragInfo dragInfo) {
			if(dragInfo.Target == null) return false;
			LayoutItemType type = dragInfo.Target.ItemType;
			return LayoutItemsHelper.IsLayoutItem(dragInfo.Target) || type == LayoutItemType.Group;
		}
		public override BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point) {
			if(item.ItemType == LayoutItemType.Panel ||
				(!item.IsControlItemsHost && !LayoutItemsHelper.IsLayoutItem(item)))
				return base.CalcDropInfo(item, point);
			if(Items.Count == 0 || !((LayoutGroup)Item).HasVisibleItems || !((LayoutGroup)Item).HasNotCollapsedItems)
				return DropTypeHelper.CalcCenterDropInfo(Bounds, point, 0.6);
			return new GroupPaneElementDropInfo(this, point);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new GroupPaneElementHitInfo(pt, this);
		}
		class GroupPaneElementDropInfo : BaseDropInfo {
			bool isHorzontal;
			bool isVertical;
			DropType type;
			public GroupPaneElementDropInfo(GroupPaneElement group, Point pt)
				: base(group.Bounds, pt) {
				type = DropType.None;
				Rect items = GetBounds(group.Items[0]);
				for(int i = 1; i < group.Items.Count; i++) {
					items.Union(GetBounds(group.Items[i]));
				}
				if(((LayoutGroup)group.Item).GroupBorderStyle == GroupBorderStyle.NoBorder && !((LayoutGroup)group.Item).IsLayoutRoot) {
					RectHelper.Inflate(ref items, MovingHelper.NoBorderMarginHorizontal, MovingHelper.NoBorderMarginVertical);
				}
				bool fHorz = ((LayoutGroup)group.Item).Orientation == System.Windows.Controls.Orientation.Horizontal;
				if(fHorz) {
					if(pt.X > items.Right) {
						type = DropType.Right;
						isHorzontal = true;
						return;
					}
					if(pt.X < items.Left) {
						type = DropType.Left;
						isHorzontal = true;
						return;
					}
					if(pt.Y > items.Bottom) {
						type = DropType.Bottom;
						isVertical = true;
						return;
					}
					if(pt.Y < items.Top) {
						type = DropType.Top;
						isVertical = true;
						return;
					}
				}
				else {
					if(pt.X > items.Right) {
						type = DropType.Right;
						isHorzontal = true;
						return;
					}
					if(pt.X < items.Left) {
						type = DropType.Left;
						isHorzontal = true;
						return;
					}
					if(pt.Y > items.Bottom) {
						type = DropType.Bottom;
						isVertical = true;
						return;
					}
					if(pt.Y < items.Top) {
						type = DropType.Top;
						isVertical = true;
						return;
					}
				}
			}
			public override DropType Type { get { return type; } }
			public override bool Horizontal { get { return isHorzontal; } }
			public override bool Vertical { get { return isVertical; } }
			public override Rect DropRect { get { return Rect.Empty; } }
			static Rect GetBounds(ILayoutElement e) {
				return new Rect(e.Location, e.Size);
			}
		}
	}
	public class TabbedPaneElement : DockLayoutContainer {
		bool hasPageHeadersCore;
		TabbedPane pane;
		protected TabbedPane TabbedPane {
			get {
				if(pane == null)
					pane = Element is TabbedPane ? (TabbedPane)Element : LayoutItemsHelper.GetVisualChild<TabbedPane>(Element);
				return pane;
			}
		}
		TabHeadersPanel headersPanelCore;
		protected TabHeadersPanel HeadersPanel {
			get {
				if(headersPanelCore == null)
					headersPanelCore = (TabbedPane.PartContent.PartItemsContainer as PanelTabContainer).PartHeadersPanel;
				return headersPanelCore;
			}
		}
		UIElement selectedPageCore;
		protected UIElement SelectedPage {
			get {
				if(selectedPageCore == null)
					selectedPageCore = (TabbedPane.PartContent.PartItemsContainer as PanelTabContainer).PartSelectedPage;
				return selectedPageCore;
			}
		}
		public TabbedPaneElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.TabPanelGroup, uiElement, view) {
			hasPageHeadersCore = (TabbedPane != null && TabbedPane.PartContent != null) && (TabbedPane.PartContent.PartItemsContainer is PanelTabContainer);
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new TabbedPaneElementBehavior(this);
		}
		protected override void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			base.CheckAndSetHitTests(hitInfo, hitType);
			hitInfo.CheckAndSetHitTest(HitTestType.PageHeaders, hitType, LayoutElementHitTest.Bounds);
			hitInfo.CheckAndSetHitTest(HitTestType.ScrollPrevButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.ScrollNextButton, hitType, LayoutElementHitTest.ControlBox);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new TabbedPaneElementHitInfo(pt, this);
		}
		public override bool AcceptFill(DockLayoutElementDragInfo dragInfo) {
			return dragInfo.Item.ItemType == LayoutItemType.Panel || dragInfo.Item.ItemType == LayoutItemType.TabPanelGroup;
		}
		public override bool HasHeadersPanel { get { return hasPageHeadersCore; } }
		public override bool IsHorizontalHeaders {
			get { return HeadersPanel.Orientation == System.Windows.Controls.Orientation.Horizontal; }
		}
		public override Rect GetHeadersPanelBounds() {
			return new Rect(TranslateToView(CoordinateHelper.ZeroPoint, HeadersPanel), HeadersPanel.RenderSize);
		}
		public override Rect GetSelectedPageBounds() {
			return new Rect(TranslateToView(CoordinateHelper.ZeroPoint, SelectedPage), SelectedPage.RenderSize);
		}
		public override SWC.Dock TabHeaderLocation {
			get {
				var tabPane = TabbedPane.PartContent.PartItemsContainer as PanelTabContainer;
				if(tabPane.CaptionLocation == CaptionLocation.Default) return SWC.Dock.Bottom;
				return tabPane.CaptionLocation.ToDock();
			}
		}
	}
	public class DocumentPaneElement : DockLayoutContainer {
		bool hasPageHeadersCore;
		DocumentPane pane;
		protected DocumentPane DocumentPane {
			get {
				if(pane == null)
					pane = Element is DocumentPane ? (DocumentPane)Element : LayoutItemsHelper.GetVisualChild<DocumentPane>(Element);
				return pane;
			}
		}
		TabHeadersPanel headersPanelCore;
		protected TabHeadersPanel HeadersPanel {
			get {
				if(headersPanelCore == null)
					headersPanelCore = (DocumentPane.PartContent.PartItemsContainer as DocumentTabContainer).PartHeadersPanel;
				return headersPanelCore;
			}
		}
		UIElement selectedPageCore;
		protected UIElement SelectedPage {
			get {
				if(selectedPageCore == null)
					selectedPageCore = (DocumentPane.PartContent.PartItemsContainer as DocumentTabContainer).PartSelectedPage;
				return selectedPageCore;
			}
		}
		public DocumentPaneElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.DocumentPanelGroup, uiElement, view) {
			hasPageHeadersCore = (DocumentPane != null) && (DocumentPane.PartContent.PartItemsContainer is DocumentTabContainer);
		}
		protected override void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			base.CheckAndSetHitTests(hitInfo, hitType);
			hitInfo.CheckAndSetHitTest(HitTestType.ScrollPrevButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.ScrollNextButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.DropDownButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.RestoreButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.PageHeaders, hitType, LayoutElementHitTest.Bounds);
		}
		public override bool AcceptFill(DockLayoutElementDragInfo dragInfo) {
			return HasHeadersPanel ? dragInfo.Item.GetAllowDockToDocumentGroup() :
				dragInfo.Item.ItemType == LayoutItemType.Document;
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new DocumentPaneElementHitInfo(pt, this);
		}
		public override bool HasHeadersPanel { get { return hasPageHeadersCore; } }
		public override bool IsHorizontalHeaders {
			get { return HeadersPanel.Orientation == System.Windows.Controls.Orientation.Horizontal; }
		}
		public override Rect GetHeadersPanelBounds() {
			return new Rect(TranslateToView(CoordinateHelper.ZeroPoint, HeadersPanel), HeadersPanel.RenderSize);
		}
		public override Rect GetSelectedPageBounds() {
			return new Rect(TranslateToView(CoordinateHelper.ZeroPoint, SelectedPage), SelectedPage.RenderSize);
		}
		public override SWC.Dock TabHeaderLocation {
			get {
				var tabPane = DocumentPane.PartContent.PartItemsContainer as DocumentTabContainer;
				if(tabPane.CaptionLocation == CaptionLocation.Default) return SWC.Dock.Top;
				return tabPane.CaptionLocation.ToDock();
			}
		}
	}
	public class TabbedLayoutGroupElement : DockLayoutContainer {
		bool hasTabPageHeadersCore;
		TabbedLayoutGroupPane pane;
		protected TabbedLayoutGroupPane TabbedPane {
			get {
				if(pane == null)
					pane = Element is TabbedLayoutGroupPane ? (TabbedLayoutGroupPane)Element : LayoutItemsHelper.GetVisualChild<TabbedLayoutGroupPane>(Element);
				return pane;
			}
		}
		public TabbedLayoutGroupElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.Group, uiElement, view) {
			hasTabPageHeadersCore = (TabbedPane != null) && (TabbedPane.PartHeadersPanel != null);
		}
		protected override void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			base.CheckAndSetHitTests(hitInfo, hitType);
			hitInfo.CheckAndSetHitTest(HitTestType.PageHeaders, hitType, LayoutElementHitTest.Bounds);
			hitInfo.CheckAndSetHitTest(HitTestType.ScrollNextButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.ScrollPrevButton, hitType, LayoutElementHitTest.ControlBox);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new TabbedLayoutGroupElementHitInfo(pt, this);
		}
		public override bool HasHeadersPanel { get { return hasTabPageHeadersCore; } }
		public override bool IsHorizontalHeaders {
			get { return TabbedPane.PartHeadersPanel.Orientation == System.Windows.Controls.Orientation.Horizontal; }
		}
		public override Rect GetHeadersPanelBounds() {
			UIElement header = TabbedPane.PartHeadersPanel;
			return new Rect(TranslateToView(CoordinateHelper.ZeroPoint, header), header.RenderSize);
		}
		public override Rect GetSelectedPageBounds() {
			UIElement content = TabbedPane.PartSelectedPage;
			return new Rect(TranslateToView(CoordinateHelper.ZeroPoint, content), content.RenderSize);
		}
		public override SWC.Dock TabHeaderLocation {
			get {
				if(TabbedPane.CaptionLocation == CaptionLocation.Default) return SWC.Dock.Top;
				return TabbedPane.CaptionLocation.ToDock();
			}
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new TabbedLayoutGroupElementBehaviour(this);
		}
		public override bool AcceptFill(DockLayoutElementDragInfo dragInfo) {
			BaseLayoutItem dragItem = dragInfo.Item;
			LayoutGroup dragGroup = dragItem as LayoutGroup;
			return dragInfo.DragSource is TabbedLayoutGroupHeaderElement ||
				LayoutItemsHelper.IsLayoutItem(dragItem) || (dragGroup != null && dragGroup.IsControlItemsHost);
		}
		public override BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point) {
			TabHeaderInsertHelper helper = new TabHeaderInsertHelper(this, point, true);
			return helper.InsertIndex == -1 ? base.CalcDropInfo(item, point) :
				new TabbedLayoutGroupElementDropInfo(this, point, helper.InsertIndex);
		}
		class TabbedLayoutGroupElementDropInfo : BaseDropInfo {
			bool isHorzontal;
			bool isVertical;
			DropType type;
			int insertIndex;
			public TabbedLayoutGroupElementDropInfo(TabbedLayoutGroupElement group, Point pt, int insertIndex)
				: base(group.Bounds, pt) {
				isHorzontal = group.IsHorizontalHeaders;
				isVertical = !isHorzontal;
				type = DropType.Center;
				this.insertIndex = insertIndex;
			}
			public override DropType Type { get { return type; } }
			public override bool Horizontal { get { return isHorzontal; } }
			public override bool Vertical { get { return isVertical; } }
			public override Rect DropRect { get { return Rect.Empty; } }
			public override int InsertIndex { get { return insertIndex; } }
		}
	}
	public class TabbedLayoutGroupHeaderElement : DockLayoutElement {
		public TabbedLayoutGroupHeaderElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.TabItem, uiElement, view) {
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new TabbedLayoutGroupHeaderElementBehavior(this);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new TabbedLayoutGroupHeaderElementHitInfo(pt, this);
		}
		public override bool IsPageHeader { get { return true; } }
	}
	public class TabbedPaneItemElement : DockLayoutElement {
		public TabbedPaneItemElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.Panel, uiElement, view) {
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new TabbedPaneItemElementBehavior(this);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new TabbedPaneItemElementHitInfo(pt, this);
		}
		public override bool IsPageHeader { get { return true; } }
	}
	public class DocumentPaneItemElement : DockLayoutElement {
		public DocumentPaneItemElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.Document, uiElement, view) {
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new DocumentPaneItemElementBehavior(this);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new DocumentPaneItemElementHitInfo(pt, this);
		}
		public override bool IsPageHeader { get { return true; } }
		protected override void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			base.CheckAndSetHitTests(hitInfo, hitType);
			hitInfo.CheckAndSetHitTest(HitTestType.RestoreButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.PinButton, hitType, LayoutElementHitTest.ControlBox);
		}
	}
	public class AutoHideTrayElement : DockLayoutContainer {
		public AutoHideTrayElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.AutoHideContainer, uiElement, view) {
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new AutoHideTrayElementBehavior(this);
		}
		protected override void EnsureBoundsCore() {
			Size = (Element.Visibility == Visibility.Visible) ? Element.RenderSize : new Size(0, 0);
			Location = CoordinateHelper.ZeroPoint;
		}
		public AutoHideTray Tray { get { return Element as AutoHideTray; } }
		protected override UIElement CheckView(UIElement view) { return view; }
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new AutoHideTrayElementHitInfo(pt, this);
		}
	}
	public class AutoHidePaneElement : DockLayoutContainer {
		public AutoHidePaneElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.AutoHideContainer, uiElement, view) {
		}
		protected override UIElement CheckView(UIElement view) { return view; }
		public override ILayoutElementBehavior GetBehavior() {
			return new AutoHidePaneElementBehavior(this);
		}
		AutoHidePane autoHidePane;
		public AutoHidePane AutoHidePane {
			get {
				if(autoHidePane == null) {
					autoHidePane = Element as AutoHidePane;
				}
				return autoHidePane;
			}
		}
		public SWC.Dock DockType {
			get { return AutoHidePane != null ? AutoHidePane.DockType : SWC.Dock.Left; }
		}
		public override bool HitTestingEnabled {
			get {
				return AutoHidePane != null && AutoHidePane.AutoHideTray.IsExpanded;
			}
		}
		protected override bool CheckVisualHitTestCore(Point pt) {
			return HitTestHelper.CheckVisualHitTest(this, pt, IsVisualChild);
		}
		bool IsVisualChild(DependencyObject root, DependencyObject child) {
			DependencyObject parent = child;
			while(parent != null) {
				if(parent == root) return true;
				parent = VisualTreeHelper.GetParent(parent);
				if(parent is AutoHideWindowHost.AutoHideWindowRoot) parent = ((AutoHideWindowHost.AutoHideWindowRoot)parent).Pane;
			}
			return false;
		}
	}
	public class AutoHideTrayHeadersGroupElement : DockLayoutContainer {
		bool hasHeadersPanelCore;
		AutoHideTrayHeadersGroup autoHideTrayHeadersGroup;
		protected AutoHideTrayHeadersGroup AutoHideTrayHeadersGroup {
			get {
				if(autoHideTrayHeadersGroup == null) autoHideTrayHeadersGroup = LayoutItemsHelper.GetVisualChild<AutoHideTrayHeadersGroup>(Element);
				return autoHideTrayHeadersGroup;
			}
		}
		AutoHideTrayHeadersPanel headersPanelCore;
		protected AutoHideTrayHeadersPanel HeadersPanel {
			get {
				if(headersPanelCore == null)
					headersPanelCore = AutoHideTrayHeadersGroup.PartHeadersPanel;
				return headersPanelCore;
			}
		}
		public AutoHideTrayHeadersGroupElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.AutoHideGroup, uiElement, view) {
			hasHeadersPanelCore = AutoHideTrayHeadersGroup != null && AutoHideTrayHeadersGroup.PartHeadersPanel != null;
		}
		protected override UIElement CheckView(UIElement view) { return view; }
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new AutoHideTrayHeadersGroupElementHitInfo(pt, this);
		}
		public override bool HasHeadersPanel {
			get { return hasHeadersPanelCore; }
		}
		public override bool IsHorizontalHeaders {
			get { return HeadersPanel.Orientation == System.Windows.Controls.Orientation.Horizontal; }
		}
		int thickness = 3;
		public override Rect GetHeadersPanelBounds() {
			bool horz = IsHorizontalHeaders;
			var dock = GetDock();
			bool near = (dock == SWC.Dock.Right || dock == SWC.Dock.Bottom);
			Size size = Bounds.Size();
			double w = IsHorizontalHeaders ? size.Width : size.Width - thickness;
			double h = IsHorizontalHeaders ? size.Height - thickness : size.Height;
			return new Rect(horz ? Bounds.Left : (near ? Bounds.Left + thickness : Bounds.Left), horz ? (near ? Bounds.Top + thickness : Bounds.Top) : Bounds.Top, w, h);
		}
		public override Rect GetSelectedPageBounds() {
			bool horz = IsHorizontalHeaders;
			var dock = GetDock();
			bool near = (dock == SWC.Dock.Right || dock == SWC.Dock.Bottom);
			Size size = View.RenderSize;
			double w = horz ? size.Width : thickness;
			double h = horz ? thickness : size.Height;
			return new Rect(horz ? 0 : (near ? 0 : size.Width - w), horz ? (near ? 0 : size.Height - h) : 0, w, h);
		}
		public override SWC.Dock TabHeaderLocation {
			get { return GetDock(); }
		}
		SWC.Dock GetDock() {
			var result = AutoHideTrayHeadersGroup.Tray.DockType;
			switch(result) {
				case SWC.Dock.Left: return SWC.Dock.Right;
				case SWC.Dock.Top: return SWC.Dock.Bottom;
				case SWC.Dock.Right: return SWC.Dock.Left;
				case SWC.Dock.Bottom: return SWC.Dock.Top;
			}
			return result;
		}
	}
	public class AutoHidePaneHeaderItemElement : DockLayoutContainer {
		public AutoHidePaneHeaderItemElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.AutoHidePanel, uiElement, view) {
		}
		protected override UIElement CheckView(UIElement view) { return view; }
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new AutoHidePaneHeaderItemElementHitInfo(pt, this);
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new AutoHidePaneHeaderItemElementBehavior(this);
		}
		public override bool IsPageHeader {
			get { return true; }
		}
		public AutoHideTray Tray { get { return View as AutoHideTray; } }
	}
	public class FloatPanePresenterElement : DockLayoutContainer {
		public FloatPanePresenterElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.FloatGroup, uiElement, view) {
		}
		IDockLayoutElement nestedElementCore;
		protected override void OnDispose() {
			if(IsDragging)
				nestedElementCore = GetNestedElementCore();
			base.OnDispose();
		}
		public override IDockLayoutElement CheckDragElement() {
			if(((FloatGroup)Item).HasSingleItem)
				return GetNestedElement();
			return this;
		}
		protected override void OnResetIsDragging() {
			if(IsDisposing)
				nestedElementCore = null;
			base.OnResetIsDragging();
		}
		protected override void EnsureBoundsCore() {
			Location = CoordinateHelper.ZeroPoint;
			Size = ((FloatPanePresenter)Element).FloatSize;
		}
		protected override void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			base.CheckAndSetHitTests(hitInfo, hitType);
			hitInfo.CheckAndSetHitTest(HitTestType.MaximizeButton, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.RestoreButton, hitType, LayoutElementHitTest.ControlBox);
		}
		UIElement GetFloatElement() {
			return ((FloatPanePresenter)Element).Element;
		}
		protected override void ResetStateCore() {
			UIElement floatElement = GetFloatElement();
			if(floatElement != null) {
				floatElement.ClearValue(ControlBox.PressedButtonProperty);
				floatElement.ClearValue(ControlBox.HotButtonProperty);
			}
		}
		protected override object InitPressedState() {
			return ControlBox.GetPressedButton(GetFloatElement());
		}
		protected override object InitHotState() {
			return ControlBox.GetHotButton(GetFloatElement());
		}
		protected override void OnStateChanged(object hitResult, State state) {
			if(HitEquals(hitResult, HitTestType.Undefined)) return;
		}
		protected override HitTestResult GetHitResult(LayoutElementHitInfo hitInfo) {
			return HitTestHelper.GetHitResult(GetFloatElement(), DockLayoutElementHelper.GetElementPoint(hitInfo));
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new FloatPanePresenterElementBehavior(this);
		}
		public override bool AcceptDockSource(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) {
			IDockLayoutElement nestedElement = GetNestedElement();
			if(nestedElement != null) return nestedElement.AcceptDockSource(dragInfo, hitInfo);
			return false;
		}
		public override bool AcceptDockTarget(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) {
			IDockLayoutElement nestedElement = GetNestedElement();
			if(nestedElement != null) return nestedElement.AcceptDockTarget(dragInfo, hitInfo);
			return LayoutItemsHelper.IsDockItem(dragInfo.Target) || LayoutItemsHelper.IsEmptyLayoutGroup(dragInfo.Target);
		}
		public override bool AcceptFill(DockLayoutElementDragInfo dragInfo) {
			IDockLayoutElement nestedElement = GetNestedElement();
			if(nestedElement != null) return nestedElement.AcceptFill(dragInfo);
			return false;
		}
		public override bool AcceptDropTarget(DockLayoutElementDragInfo dragInfo) {
			IDockLayoutElement nestedElement = GetNestedElement();
			if(nestedElement != null) return nestedElement.AcceptDropTarget(dragInfo);
			return false;
		}
		public override bool AcceptDragSource(DockLayoutElementDragInfo dragInfo) {
			IDockLayoutElement nestedElement = GetNestedElement();
			if(nestedElement != null) return nestedElement.AcceptDragSource(dragInfo);
			return false;
		}
		protected IDockLayoutElement GetNestedElement() {
			if(IsDisposing) return nestedElementCore;
			return GetNestedElementCore();
		}
		IDockLayoutElement GetNestedElementCore() {
			return (Nodes.Length == 1) ? (IDockLayoutElement)Nodes[0] : null;
		}
	}
	public class ControlItemElement : DockLayoutContainer {
		public ControlItemElement(UIElement uiElement, UIElement view)
			: base(LayoutItemType.ControlItem, uiElement, view) {
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new ControlItemElementBehavior(this);
		}
		public override bool AcceptDropTarget(DockLayoutElementDragInfo dragInfo) {
			if(dragInfo.Target == null) return false;
			LayoutItemType type = dragInfo.Target.ItemType;
			return LayoutItemsHelper.IsLayoutItem(dragInfo.Target) || type == LayoutItemType.Group;
		}
		public override BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point) {
			return DropTypeHelper.CalcSideDropInfo(Bounds, point, 0.25);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new ControlItemElementHitInfo(pt, this);
		}
	}
}

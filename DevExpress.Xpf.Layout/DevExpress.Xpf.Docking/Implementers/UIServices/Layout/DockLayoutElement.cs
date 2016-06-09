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
using System.Windows.Media;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking.Platform {
	public interface IDockLayoutElement : ILayoutElement, IDragSource, IDropTarget {
		LayoutItemType Type { get; }
		BaseLayoutItem Item { get; }
		UIElement View { get; }
		UIElement Element { get; }
		ILayoutElementBehavior GetBehavior();
		ILayoutElement GetDragItem();
		BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point);
		IDockLayoutElement CheckDragElement();
		bool IsPageHeader { get; }
		bool AllowActivate { get; }
	}
	public interface IDockLayoutContainer : IDockLayoutElement, ILayoutContainer {
		bool HasHeadersPanel { get; }
		Rect GetHeadersPanelBounds();
		Rect GetSelectedPageBounds();
		SWC.Dock TabHeaderLocation { get;}
		bool IsHorizontalHeaders { get; }
	}
	public class DockLayoutElement : BaseLayoutElement, IDockLayoutElement, ISelectionKey {
		readonly LayoutItemType itemTypeCore;
		object viewKey;
		object ISelectionKey.Item { get { return Item; } }
		object ISelectionKey.ElementKey { get { return Element; } }
		object ISelectionKey.ViewKey { get { return viewKey; } }
		public UIElement View { get; private set; }
		public UIElement Element { get; private set; }
		public BaseLayoutItem Item { get; private set; }
		public LayoutItemType Type { get { return itemTypeCore; } }
		public DockLayoutElement(LayoutItemType itemType, UIElement uiElement, UIElement view) {
			itemTypeCore = itemType;
			Element = uiElement;
			viewKey = view;
			View = CheckView(view);
			Item = GetItem(uiElement);
		}
		public virtual ILayoutElement GetDragItem() {
			return this;
		}
		protected virtual BaseLayoutItem GetItem(UIElement uiElement) {
			return DockLayoutManager.GetLayoutItem(uiElement);
		}
		protected virtual UIElement CheckView(UIElement view) {
			FloatPanePresenter floatPresenter = view as FloatPanePresenter;
			return (floatPresenter == null) ? view : floatPresenter.Element;
		}
		protected Point TranslateToView(Point point, UIElement element = null) {
			if(element == null) element = Element;
			return element.TranslatePoint(point, View);
		}
		protected override void EnsureBoundsCore() {
			Size = Element.RenderSize;
			Location = TranslateToView(CoordinateHelper.ZeroPoint);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new DockLayoutElementHitInfo(pt, this);
		}
		protected override bool HitEquals(object prevHitResult, object hitResult) {
			return HitTestHelper.HitTestTypeEquals(prevHitResult, hitResult);
		}
		public override void ResetState() {
			base.ResetState();
			if(Element != null) {
				Element.ClearValue(ControlBox.PressedButtonProperty);
				Element.ClearValue(ControlBox.HotButtonProperty);
			}
		}
		protected override object InitPressedState() {
			return ControlBox.GetPressedButton(Element);
		}
		protected override object InitHotState() {
			return ControlBox.GetHotButton(Element);
		}
		protected override void OnStateChanged(object hitResult, State state) {
			if(HitEquals(hitResult, HitTestType.Undefined)) return;
		}
		protected override bool HitTestCore(Point pt) {
			return base.HitTestCore(pt) && HitTestHelper.CheckVisualHitTest(this, pt);
		}
		protected override void CalcHitInfoCore(LayoutElementHitInfo hitInfo) {
			HitTestResult hitResult = HitTestHelper.GetHitResult(View, hitInfo.HitPoint);
			if(hitResult == null || hitResult.VisualHit == null) return;
			hitInfo.Tag = hitResult;
			HitTestType hitType = HitTestHelper.GetHitTestType(hitResult);
			if(hitType != HitTestType.Undefined)
				CheckAndSetHitTests(hitInfo, hitType);
		}
		protected virtual void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			hitInfo.CheckAndSetHitTest(HitTestType.Border, hitType, LayoutElementHitTest.Border);
			hitInfo.CheckAndSetHitTest(HitTestType.Header, hitType, LayoutElementHitTest.Header);
			hitInfo.CheckAndSetHitTest(HitTestType.Label, hitType, LayoutElementHitTest.Header);
			hitInfo.CheckAndSetHitTest(HitTestType.Content, hitType, LayoutElementHitTest.Content);
			hitInfo.CheckAndSetHitTest(HitTestType.ControlBox, hitType, LayoutElementHitTest.ControlBox);
			hitInfo.CheckAndSetHitTest(HitTestType.CloseButton, hitType, LayoutElementHitTest.ControlBox);
		}
		public virtual ILayoutElementBehavior GetBehavior() {
			return new DockLayoutElementBehavior(this);
		}
		public virtual IDockLayoutElement CheckDragElement() {
			return this;
		}
		public virtual BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point) {
			return DropTypeHelper.CalcCenterDropInfo(new Rect(Location, Size), point, 0.0);
		}
		public virtual bool AcceptFill(DockLayoutElementDragInfo dragInfo) { return true; }
		public virtual bool AcceptDockSource(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) { return true; }
		public virtual bool AcceptDockTarget(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) { return true; }
		public virtual bool AcceptDragSource(DockLayoutElementDragInfo dragInfo) { return true; }
		public virtual bool AcceptDropTarget(DockLayoutElementDragInfo dragInfo) { return true; }
		public virtual bool IsPageHeader { get { return false; } }
		public override bool IsActive {
			get { return Item != null && Item.IsActive; }
		}
		public virtual bool AllowActivate { get { return Item != null && Item.AllowActivate; } }
	}
	public class EmptyLayoutContainer : BaseLayoutContainer {
		protected override void EnsureBoundsCore() { }
	}
	public class DockLayoutContainer : BaseLayoutContainer, IDockLayoutContainer, ISelectionKey {
		readonly LayoutItemType itemTypeCore;
		object viewKey;
		object ISelectionKey.Item { get { return Item; } }
		object ISelectionKey.ElementKey { get { return Element; } }
		object ISelectionKey.ViewKey { get { return viewKey; } }
		public UIElement View { get; private set; }
		public UIElement Element { get; private set; }
		public BaseLayoutItem Item { get; private set; }
		public LayoutItemType Type { get { return itemTypeCore; } }
		public DockLayoutContainer(LayoutItemType itemType, UIElement uiElement, UIElement view) {
			itemTypeCore = itemType;
			Element = uiElement;
			viewKey = view;
			View = CheckView(view);
			Item = uiElement as BaseLayoutItem ?? DockLayoutManager.GetLayoutItem(uiElement);
		}
		public virtual ILayoutElement GetDragItem() {
			return this;
		}
		protected virtual UIElement CheckView(UIElement view) {
			FloatPanePresenter floatPresenter = view as FloatPanePresenter;
			return (floatPresenter == null) ? view : floatPresenter.Element;
		}
		protected Point TranslateToView(Point point, UIElement element = null) {
			if(element == null) element = Element;
			return element.TranslatePoint(point, View);
		}
		protected override void EnsureBoundsCore() {
			Size = Element.RenderSize;
			Location = TranslateToView(CoordinateHelper.ZeroPoint);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new DockLayoutElementHitInfo(pt, this);
		}
		protected override bool HitTestCore(Point pt) {
			return base.HitTestCore(pt) && CheckVisualHitTestCore(pt);
		}
		protected virtual bool CheckVisualHitTestCore(Point pt) {
			return HitTestHelper.CheckVisualHitTest(this, pt);
		}
		protected override bool HitEquals(object prevHitResult, object hitResult) {
			return HitTestHelper.HitTestTypeEquals(prevHitResult, hitResult);
		}
		public sealed override void ResetState() {
			base.ResetState();
			ResetStateCore();
		}
		protected virtual void ResetStateCore() {
			if(Element != null) {
				Element.ClearValue(ControlBox.PressedButtonProperty);
				Element.ClearValue(ControlBox.HotButtonProperty);
			}
		}
		protected override object InitPressedState() {
			return ControlBox.GetPressedButton(Element);
		}
		protected override object InitHotState() {
			return ControlBox.GetHotButton(Element);
		}
		protected override void OnStateChanged(object hitResult, State state) {
			if(HitEquals(hitResult, HitTestType.Undefined)) return;
		}
		protected override void CalcHitInfoCore(LayoutElementHitInfo hitInfo) {
			HitTestResult hitResult = GetHitResult(hitInfo);
			if(hitResult == null || hitResult.VisualHit == null) return;
			hitInfo.Tag = hitResult;
			HitTestType hitType = HitTestHelper.GetHitTestType(hitResult);
			if(hitType != HitTestType.Undefined)
				CheckAndSetHitTests(hitInfo, hitType);
		}
		protected virtual HitTestResult GetHitResult(LayoutElementHitInfo hitInfo) {
			return HitTestHelper.GetHitResult(Element, DockLayoutElementHelper.GetElementPoint(hitInfo));
		}
		protected virtual void CheckAndSetHitTests(LayoutElementHitInfo hitInfo, HitTestType hitType) {
			hitInfo.CheckAndSetHitTest(HitTestType.Border, hitType, LayoutElementHitTest.Border);
			hitInfo.CheckAndSetHitTest(HitTestType.Header, hitType, LayoutElementHitTest.Header);
			hitInfo.CheckAndSetHitTest(HitTestType.Label, hitType, LayoutElementHitTest.Header);
			hitInfo.CheckAndSetHitTest(HitTestType.Content, hitType, LayoutElementHitTest.Content);
			hitInfo.CheckAndSetHitTest(HitTestType.CloseButton, hitType, LayoutElementHitTest.ControlBox);
		}
		public virtual ILayoutElementBehavior GetBehavior() {
			return new DockLayoutElementBehavior(this);
		}
		public virtual IDockLayoutElement CheckDragElement() {
			return this;
		}
		public virtual BaseDropInfo CalcDropInfo(BaseLayoutItem item, Point point) {
			return DropTypeHelper.CalcCenterDropInfo(new Rect(Location, Size), point, 0.0);
		}
		public virtual bool AcceptFill(DockLayoutElementDragInfo dragInfo) { return false; }
		public virtual bool AcceptDockSource(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) { return true; }
		public virtual bool AcceptDockTarget(DockLayoutElementDragInfo dragInfo, DockHintHitInfo hitInfo) { return true; }
		public virtual bool AcceptDragSource(DockLayoutElementDragInfo dragInfo) { return true; }
		public virtual bool AcceptDropTarget(DockLayoutElementDragInfo dragInfo) { return true; }
		public virtual bool IsPageHeader { get { return false; } }
		public virtual bool HasHeadersPanel { get { return false; } }
		public virtual bool IsHorizontalHeaders { get { return false; } }
		public virtual Rect GetHeadersPanelBounds() { return Rect.Empty; }
		public virtual Rect GetSelectedPageBounds() { return Rect.Empty; }
		public virtual SWC.Dock TabHeaderLocation { get { return SWC.Dock.Top; } }
		public override bool IsActive {
			get { return Item != null && Item.IsActive; }
		}
		public virtual bool AllowActivate { get { return Item != null && Item.AllowActivate; } }
	}
	#region Helpers
	public class DockLayoutElementHelper {
		public static Point GetElementPoint(LayoutElementHitInfo hi) {
			return new Point(
					hi.HitPoint.X - hi.Element.Location.X,
					hi.HitPoint.Y - hi.Element.Location.Y
				);
		}
	}
	#endregion Helpers
}

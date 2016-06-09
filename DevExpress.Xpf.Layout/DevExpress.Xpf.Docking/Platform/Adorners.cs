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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking.Platform {
	public class DockHintHitInfo {
		public DockHintHitInfo(DockingHintAdorner adorner, HitTestResult hitResult) {
			Adorner = adorner;
			Type = DockVisualizerElement.None;
			if(hitResult != null) {
				Element = LayoutHelper.FindParentObject<DockHintElement>(hitResult.VisualHit);
				if(Element != null && Element.Visibility == Visibility.Visible) {
					Type = Element.Type;
					IsCenter = (Type == DockVisualizerElement.Center);
					DockType = CalcDockType(hitResult.VisualHit);
					Dock = DockType.ToDock();
					HitButton = LayoutHelper.FindParentObject<DockHintButton>(hitResult.VisualHit);
					if(InButton) {
						IsHideButton = HitButton.Name == "PART_Hide" && HitButton.IsEnabled;
						IsTabButton = HitButton.Name.Contains("PART_Tab") && HitButton.IsEnabled;
						if(!HitButton.IsAvailable)
							DockType = DockType.None;
					}
					else DockType = DockType.None;
				}
			}
		}
		DockType CalcDockType(DependencyObject dObj) {
			Layout.Core.DockType result = Layout.Core.DockType.None;
			while(dObj != null) {
				Layout.Core.DockType value = DockHintElement.GetDockType(dObj);
				if(value != Layout.Core.DockType.None)
					return value;
				if(dObj is DockHintElement) break;
				dObj = VisualTreeHelper.GetParent(dObj);
			}
			return result;
		}
		protected DockHintElement Element { get; private set; }
		protected internal DockHintButton HitButton { get; private set; }
		public DockingHintAdorner Adorner { get; private set; }
		public DockVisualizerElement Type { get; private set; }
		public DockType DockType { get; private set; }
		public SWC.Dock Dock { get; private set; }
		public bool InHint { get { return Element is SideDockHintElement || Element is CenterDockHintElement; } }
		public bool InButton { get { return HitButton != null; } }
		public bool IsHideButton { get; private set; }
		public bool IsCenter { get; private set; }
		public bool IsTabButton { get; private set; }
	}
	public class DragAdorner : PlacementAdorner {
		public DragAdorner(DockLayoutManager container)
			: base(container) {
		}
		protected override BaseAdornerSurface CreateAdornerSurface() {
			return new DragAdornerSurface(this, false);
		}
		protected override void OnActivated() {
			base.OnActivated();
			WindowHelper.BindFlowDirectionIfNeeded(this, AdornedElement);
		}
		protected override void SetBoundsInContainerCore(UIElement placementElement, Rect bounds) {
			var supportAutoSize = placementElement as ISupportAutoSize;
			if(supportAutoSize != null && supportAutoSize.IsAutoSize) {
				var autoSize = supportAutoSize.FitToContent(bounds.Size);
				PlacementSurface.SetBounds(placementElement, new Rect(bounds.Location, autoSize));
			}
			else base.SetBoundsInContainerCore(placementElement, bounds);
		}
		public class DragAdornerSurface : AdornerSurface {
			public DragAdornerSurface(PlacementAdorner adorner)
				: base(adorner) {
			}
			public DragAdornerSurface(PlacementAdorner adorner, bool enableValidation)
				: base(adorner, enableValidation) {
			}
			protected override void BringToFrontCore(UIElement placementElement) {
				int count = 0;
				var elements = PlacementInfos.Keys.OrderBy(x => GetZIndex(x));
				foreach(var element in elements) {
					if(element == placementElement) continue;
					SetZIndex(element, count++);
				}
				SetZIndex(placementElement, count++);
			}
			protected override Size MeasureOverride(Size availableSize) {
				foreach(KeyValuePair<UIElement, PlacementItemInfo> pair in PlacementInfos) {
					var supportAutoSize = pair.Key as ISupportAutoSize;
					if(supportAutoSize != null && supportAutoSize.IsAutoSize) {
						Rect bounds = pair.Value.Bounds;
						var autoSize = supportAutoSize.FitToContent(bounds.Size);
						SetBounds(pair.Key, new Rect(bounds.Location, autoSize));
					}
				}
				return base.MeasureOverride(availableSize);
			}
		}
	}
	public class SelectionController : IDisposable {
		IList<SelectionHint> selectionHints;
		bool isDisposing;
		public SelectionController(DockingHintAdorner adorner) {
			selectionHints = new List<SelectionHint>();
			Adorner = adorner;
			Container = DockLayoutManager.GetDockLayoutManager(Adorner.AdornedElement);
			Container.LayoutItemSizeChanged += OnItemSizeChanged;
			Container.LayoutItemSelectionChanged += OnItemSelectionChanged;
		}
		void OnItemSelectionChanged(object sender, LayoutItemSelectionChangedEventArgs e) {
			if(Container.IsCustomization) {
				UpdateHints();
				Adorner.Update();
			}
		}
		protected virtual void OnItemSizeChanged(object sender, LayoutItemSizeChangedEventArgs e) {
			if(Container.IsCustomization) {
				UpdateHints();
				Adorner.Update();
			}
		}
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				Container.LayoutItemSelectionChanged -= OnItemSelectionChanged;
				Container.LayoutItemSizeChanged -= OnItemSizeChanged;
				Container = null;
				Ref.Clear(ref selectionHints);
			}
		}
		public virtual void UpdateHints() {
			if(EnsureSelectionHints()) {
				Selection selection = Container.LayoutController.Selection;
				for(int i = 0; i < Hints.Count; i++) {
					BaseLayoutItem item = (i < selection.Count) ? selection[i] : null;
					Hints[i].Item = Adorner.ContainsSelection(item) ? item : null;
				}
			}
		}
		protected virtual bool EnsureSelectionHints() {
			if(!Container.LayoutController.IsCustomization) return false;
			while(Hints.Count < Container.LayoutController.Selection.Count) {
				Hints.Add(CreateSelectionHint());
			}
			return true;
		}
		protected virtual SelectionHint CreateSelectionHint() {
			return (SelectionHint)Adorner.CreateDockHintElement(
					DockVisualizerElement.Selection, new Size(), DevExpress.Xpf.Layout.Core.Alignment.Fill
				);
		}
		protected DockLayoutManager Container { get; private set; }
		protected DockingHintAdorner Adorner { get; private set; }
		protected internal IList<SelectionHint> Hints { get { return selectionHints; } }
	}
	public class RenameController : IDisposable {
		RenameHint renameHint;
		bool isDisposing;
		public bool IsRenamingStarted { get { return RenameHint != null ? RenameHint.IsRenamingStarted : false; } }
		public RenameController(DockingHintAdorner adorner) {
			Adorner = adorner;
			Container = DockLayoutManager.GetDockLayoutManager(Adorner.AdornedElement);
		}
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				Container = null;
			}
		}
		internal void StartRenaming(IDockLayoutElement layoutElement) {
			if(!Adorner.ContainsSelection(layoutElement.Item)) return;
			if(EnsureRenameHint()) {
				if(renameHint.IsRenamingStarted)
					CancelRenaming();
				renameHint.StartRenaming(layoutElement);
			}
			Adorner.Update();
		}
		internal void CancelRenaming() {
			if(EnsureRenameHint() && renameHint.IsRenamingStarted) {
				renameHint.CancelRenaming();
			}
			Adorner.Update();
		}
		internal void EndRenaming() {
			if(EnsureRenameHint() && renameHint.IsRenamingStarted) {
				renameHint.EndRenaming();
			}
			Adorner.Update();
		}
		protected bool EnsureRenameHint() {
			if(renameHint == null)
				renameHint = CreateRenameHint();
			return renameHint != null;
		}
		protected RenameHint CreateRenameHint() {
			return (RenameHint)Adorner.CreateDockHintElement(
					DockVisualizerElement.RenameHint, new Size(), DevExpress.Xpf.Layout.Core.Alignment.Fill
				);
		}
		protected DockLayoutManager Container { get; private set; }
		protected DockingHintAdorner Adorner { get; private set; }
		protected internal RenameHint RenameHint { get { return renameHint; } }
	}
	public class DockingHintAdorner : BaseSurfacedAdorner {
		SelectionController selectionControllerCore;
		RenameController renameControllerCore;
		public DockingHintAdorner(UIElement container)
			: base(container) {
			selectionControllerCore = CreateSelectionController();
			renameControllerCore = CreateRenameController();
			DockHintsConfiguration = new DockHintsConfiguration();
		}
		public new bool IsDisposing { get { return base.IsDisposing; } }
		protected override void OnDispose() {
			VisualizerSurface.Clear();
			Deactivate();
			Ref.Dispose(ref hitCache);
			Ref.Dispose(ref selectionControllerCore);
			Ref.Dispose(ref renameControllerCore);
			base.OnDispose();
		}
		protected override void OnActivated() {
			Manager = DockLayoutManager.GetDockLayoutManager(AdornedElement);
			CreateVisualizerElements();
			InvalidateArrange();
		}
		protected DockLayoutManager Manager { get; private set; }
		protected internal SelectionController SelectionController {
			get { return selectionControllerCore; }
		}
		protected virtual SelectionController CreateSelectionController() {
			return new SelectionController(this);
		}
		protected internal RenameController RenameController {
			get { return renameControllerCore; }
		}
		protected virtual RenameController CreateRenameController() {
			return new RenameController(this);
		}
		internal HostType HostType { get; set; }
		bool ShouldCreateSideElements { get { return HostType == Layout.Core.HostType.Layout; } }
		protected virtual void CreateVisualizerElements() {
			Size measureSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
			CreateDockHintElement(DockVisualizerElement.DockZone, new Size(), Layout.Core.Alignment.Fill);
			if(ShouldCreateSideElements) {
				CreateDockHintElement(DockVisualizerElement.Left, measureSize, Layout.Core.Alignment.MiddleLeft);
				CreateDockHintElement(DockVisualizerElement.Right, measureSize, Layout.Core.Alignment.MiddleRight);
				CreateDockHintElement(DockVisualizerElement.Top, measureSize, Layout.Core.Alignment.TopCenter);
				CreateDockHintElement(DockVisualizerElement.Bottom, measureSize, Layout.Core.Alignment.BottomCenter);
			}
			CreateDockHintElement(DockVisualizerElement.Center, measureSize, Layout.Core.Alignment.MiddleCenter);
		}
		public DockHintElement CreateDockHintElement(DockVisualizerElement type, Size size, Layout.Core.Alignment alignment) {
			DockHintElement element = CreateDockHintElement(type);
			if(element != null) {
				VisualizerSurface.AddElement(element, size, alignment, type);
			}
			return element;
		}
		protected virtual DockHintElement CreateDockHintElement(DockVisualizerElement type) {
			return DockHintElementFactory.Make(type);
		}
		protected override BaseAdornerSurface CreateAdornerSurface() {
			return new AdornerSurface(this);
		}
		protected AdornerSurface VisualizerSurface {
			get { return Surface as AdornerSurface; }
		}
		HitTestHelper.HitCache hitCache;
		public DockHintHitInfo HitTest(Point point) {
			double indent = Indent;
			Point p = new Point(point.X + indent, point.Y + indent);
			return new DockHintHitInfo(this, HitTestHelper.HitTest(Surface, p, ref hitCache));
		}
		public void ResetDocking() {
			DockHintsConfiguration.Invalidate();
			TargetRect = Rect.Empty;
			HintRect = Rect.Empty;
		}
		public void ClearHotTrack() {
			VisualizerSurface.ClearHotTrack();
		}
		public void UpdateHotTrack(DockHintHitInfo hitInfo) {
			VisualizerSurface.UpdateHotTrack(hitInfo);
		}
		public void UpdateState() {
			VisualizerSurface.UpdateState();
		}
		public void UpdateEnabledState() {
			VisualizerSurface.UpdateEnabledState();
		}
		public void UpdateIsAvailable() {
			VisualizerSurface.UpdateIsAvailable();
		}
		public void UpdateSelection() {
			SelectionController.UpdateHints();
		}
		public bool ContainsSelection(BaseLayoutItem item) {
			if(item == null) return false;
			LayoutGroup itemRoot = item.GetRoot();
			if(AdornedElement is AdornerWindowContent)
				return ((AdornerWindowContent)AdornedElement).View == Manager.GetView(itemRoot);
			IUIElement rootElement = AdornedElement as IUIElement;
			if(AdornedElement is FloatPanePresenter.FloatingContentPresenter)
				rootElement = ((FloatPanePresenter.FloatingContentPresenter)AdornedElement).Container as IUIElement;
			return (itemRoot != null) && Manager.GetView(itemRoot) == Manager.GetView(rootElement);
		}
		internal void SetDockHintsConfiguration(DockLayoutElementDragInfo dragInfo) {
			DockHintsConfiguration.Invalidate();
			DockHintsConfiguration.SetConfiguration(this.Manager, dragInfo);
		}
		internal double Indent {
			get { return VisualizerAdornerHelper.GetAdornerWindowIndent(this); }
		}
		public bool ShowSelectionHints { get; set; }
		public Rect TargetRect { get; set; }
		public Rect HintRect { get; set; }
		public Rect SurfaceRect { get; set; }
		internal DockHintsConfiguration DockHintsConfiguration;
		protected class AdornerSurface : BaseAdornerSurface {
			Dictionary<DockHintElement, ElementInfo> elementsCore;
			public AdornerSurface(DockingHintAdorner adorner)
				: base(adorner) {
				this.elementsCore = new Dictionary<DockHintElement, ElementInfo>();
			}
			protected DockingHintAdorner Adorner {
				get { return BaseAdorner as DockingHintAdorner; }
			}
			public Dictionary<DockHintElement, ElementInfo> ElementInfos {
				get { return elementsCore; }
			}
			public void AddElement(DockHintElement element, Size size, Layout.Core.Alignment alignment, DockVisualizerElement type) {
				if(ElementInfos.ContainsKey(element)) return;
				Children.Add(element);
				RemoveLogicalChild(element);
				Adorner.Manager.DockHintsContainer.Add(element);
				element.Measure(size);
				ElementInfos.Add(element, new ElementInfo(element.DesiredSize, alignment, type));
				if(type != DockVisualizerElement.Selection && type != DockVisualizerElement.DockZone) 
					psvPanel.SetZIndex(element, 10);
			}
			public void RemoveElement(DockHintElement element) {
				if(!ElementInfos.ContainsKey(element)) return;
				Children.Remove(element);
				ElementInfos.Remove(element);
				Adorner.Manager.DockHintsContainer.Remove(element);
			}
			public void Clear() {
				DockHintElement[] uiElements = new DockHintElement[ElementInfos.Count];
				ElementInfos.Keys.CopyTo(uiElements, 0);
				Array.ForEach(uiElements, RemoveElement);
			}
			public void ClearHotTrack() {
				foreach(KeyValuePair<DockHintElement, ElementInfo> pair in ElementInfos) {
					pair.Key.UpdateHotTrack(null);
				}
			}
			public void UpdateHotTrack(DockHintHitInfo hitInfo) {
				foreach(KeyValuePair<DockHintElement, ElementInfo> pair in ElementInfos) {
					pair.Key.UpdateHotTrack(hitInfo.HitButton);
				}
			}
			public void UpdateState() {
				foreach(KeyValuePair<DockHintElement, ElementInfo> pair in ElementInfos) {
					pair.Key.UpdateState(Adorner);
				}
			}
			public void UpdateEnabledState() {
				foreach(KeyValuePair<DockHintElement, ElementInfo> pair in ElementInfos) {
					pair.Key.UpdateEnabledState(Adorner);
				}
			}
			public void UpdateIsAvailable() {
				foreach(KeyValuePair<DockHintElement, ElementInfo> pair in ElementInfos) {
					pair.Key.UpdateAvailableState(Adorner);
				}
			}
			protected override Size ArrangeOverride(Size finalSize) {
				Adorner.SurfaceRect = new Rect(CoordinateHelper.ZeroPoint, finalSize);
				foreach(KeyValuePair<DockHintElement, ElementInfo> pair in ElementInfos) {
					pair.Key.Arrange(Adorner, pair.Value);
				}
				return finalSize;
			}
		}
		public class ElementInfo {
			public ElementInfo(Size size, DevExpress.Xpf.Layout.Core.Alignment alignment, DockVisualizerElement type) {
				Size = size;
				Alignment = alignment;
				Type = type;
			}
			public Size Size { get; private set; }
			public DevExpress.Xpf.Layout.Core.Alignment Alignment { get; private set; }
			public DockVisualizerElement Type { get; private set; }
			public Rect CalcPlacement(Rect container, double Indent) {
				double indent = Indent;
				switch(Alignment) {
					case Layout.Core.Alignment.Fill:
						if(container.IsEmpty) return container;
						container = new Rect(container.X + indent, container.Y + indent, container.Width, container.Height);
						return PlacementHelper.Arrange(Size, container, Alignment);
					case Layout.Core.Alignment.TopCenter:
						return PlacementHelper.Arrange(Size, container, Alignment, new Point(0, indent));
					case Layout.Core.Alignment.MiddleLeft:
						return PlacementHelper.Arrange(Size, container, Alignment, new Point(indent, 0));
					case Layout.Core.Alignment.MiddleRight:
						return PlacementHelper.Arrange(Size, container, Alignment, new Point(-indent, 0));
					case Layout.Core.Alignment.BottomCenter:
						return PlacementHelper.Arrange(Size, container, Alignment, new Point(0, -indent));
					default:
						return PlacementHelper.Arrange(Size, container, Alignment, new Point(indent, indent));
				}
			}
		}
	}
	public class AutoHideDockHintAdorner : DockingHintAdorner {
		public AutoHideDockHintAdorner(UIElement container)
			: base(container) {
		}
		protected override void CreateVisualizerElements() { }
	}
	public interface IAdornerWindowClient {
		Rect Bounds { get; }
	}
	public interface ISupportAutoSize {
		bool IsAutoSize { get; }
		Size FitToContent(Size availableSize);
	}
	public class TabHeadersAdorner : PlacementAdorner {
		public TabHeadersAdorner(UIElement container)
			: base(container) {
			IsHitTestVisible = false;
		}
		public TabHint Tab { get; private set; }
		public TabHeaderHint Header { get; private set; }
		public DockLayoutManager Manager { get; private set; }
		TabHeadersAdornerSurface TabHeadersSurface { get { return PlacementSurface as TabHeadersAdornerSurface; } }
		protected override void OnActivated() {
			base.OnActivated();
			Tab = new TabHint();
			Header = new TabHeaderHint();
			Manager = DockLayoutManager.GetDockLayoutManager(AdornedElement);
			RegisterEx(Tab);
			RegisterEx(Header);
		}
		protected override void OnDispose() {
			Deactivate();
			base.OnDispose();
		}
		protected override void OnDeactivated() {
			Unregister(Tab);
			Unregister(Header);
			base.OnDeactivated();
		}
		public void RegisterEx(UIElement element) {
			Register(element);
			TabHeadersSurface.RemoveFromLogicalTree(element);
			Manager.DockHintsContainer.Add(element);
		}
		protected override BaseAdornerSurface CreateAdornerSurface() {
			return new TabHeadersAdornerSurface(this) { Opacity = 0.75 };
		}
		public void ResetElements() {
			if(IsDisposing) return;
			SetVisible(Tab, false);
			SetVisible(Header, false);
			IsTabHeaderHintEnabled = false;
		}
		public bool IsTabHeaderHintEnabled { get; private set; }
		double Indent {
			get { return VisualizerAdornerHelper.GetAdornerWindowIndent(this); }
		}
		public void ShowElements(SWC.Dock headerLocation, Rect tab, Rect header) {
			Tab.TabHeaderLocation = headerLocation;
			Header.TabHeaderLocation = headerLocation;
			SetVisible(Tab, !tab.IsEmpty);
			SetVisible(Header, !header.IsEmpty);
			double indent = Indent;
			if(!tab.IsEmpty) {
				tab.X += indent;
				tab.Y += indent;
				SetBoundsInContainer(Tab, tab);
			}
			if(!header.IsEmpty) {
				header.X += indent;
				header.Y += indent;
				SetBoundsInContainer(Header, header);
			}
			IsTabHeaderHintEnabled = !header.IsEmpty;
		}
		class TabHeadersAdornerSurface : AdornerSurface {
			public TabHeadersAdornerSurface(PlacementAdorner adorner)
				: base(adorner, false) {
			}
			public void RemoveFromLogicalTree(UIElement element) {
				RemoveLogicalChild(element);
			}
		}
	}
	public class ShadowResizeAdorner : PlacementAdorner {
		public ShadowResizeAdorner(UIElement container)
			: base(container) {
			IsHitTestVisible = false;
		}
		Rect savedBounds;
		UIElement savedControl;
		UIElement ResizeBackground;
		protected override void OnActivated() {
			base.OnActivated();
			ResizeBackground = new ShadowResizeBackground();
			Register(ResizeBackground);
		}
		protected override void OnDeactivated() {
			Unregister(ResizeBackground);
			if(savedControl != null) {
				Unregister(savedControl);
			}
			base.OnDeactivated();
		}
		protected override BaseAdornerSurface CreateAdornerSurface() {
			return new AdornerSurface(this, false);
		}
		void ResetElements() {
			SetVisible(ResizeBackground, false);
			if(savedControl != null) {
				SetVisible(savedControl, false);
				Unregister(savedControl);
			}
		}
		internal void EndResizing() {
			ResetElements();
		}
		double Indent {
			get { return VisualizerAdornerHelper.GetAdornerWindowIndent(this); }
		}
		internal void StartResizing(UIElement control, Rect bounds) {
			savedControl = control;
			savedBounds = bounds;
			Register(control);
			SetVisible(control, !bounds.IsEmpty);
			UpdateBounds(control, bounds);
		}
		internal void ShowBackground(Rect bounds) {
			SetVisible(ResizeBackground, !bounds.IsEmpty);
			UpdateBounds(ResizeBackground, bounds);
		}
		internal void Resize(Rect bounds) {
			UpdateBounds(savedControl, bounds);
		}
		void UpdateBounds(UIElement control, Rect bounds) {
			if(!bounds.IsEmpty) {
				double indent = Indent;
				bounds.X += indent;
				bounds.Y += indent;
				SetBoundsInContainer(control, bounds);
			}
		}
	}
}

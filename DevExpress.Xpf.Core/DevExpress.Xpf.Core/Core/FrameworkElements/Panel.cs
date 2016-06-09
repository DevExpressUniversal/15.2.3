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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core {
	public interface IPanel : IControl {
		UIElement ChildAt(Point p, bool ignoreInternalElements, bool ignoreTempChildren, bool useBounds);
		FrameworkElements GetChildren(bool includeInternalElements, bool includeTempChildren, bool visibleOnly);
		FrameworkElements GetLogicalChildren(bool visibleOnly);
		Size ActualDesiredSize { get; }
		UIElementCollection Children { get; }
		Rect ChildrenBounds { get; }
		Rect ClientBounds { get; }
		Rect ContentBounds { get; }
	}
	public abstract class PanelBase : Panel, IPanel, IScrollBarUpdated {
		#region Z Indexes
		public const int LowZIndex = -1000;
		public const int NormalLowZIndex = -500;
		public const int NormalMediumLowZIndex = -100;
		public const int NormalMediumHighZIndex = 100;
		public const int NormalHighZIndex = 500;
		public const int HighZIndex = 1000;
		#endregion Z Indexes
		#region Dependency Properties
		protected static void OnAttachedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			var element = o as FrameworkElement;
			if (element == null)
				return;
			var parent = element.GetParent() as PanelBase;
			if (parent != null)
				parent.OnAttachedPropertyChanged(element, e.Property, e.OldValue, e.NewValue);
		}
		protected static DependencyProperty RegisterPropertyListener(string propertyName) {
			return DependencyProperty.Register(propertyName + "Listener", typeof(object), typeof(PanelBase),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (PanelBase)o;
						if (!control._IsAttachingPropertyListener)
							control.OnPropertyChanged(e.Property, e.OldValue, e.NewValue);
					}));
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		private static readonly DependencyProperty ClipListener = RegisterPropertyListener("Clip");
		#endregion Dependency Properties
		private bool _IsAttachingPropertyListener;
#if SILVERLIGHT
		private bool _IsLoaded;
#endif
		private bool _IsUpdatingClip;
		public PanelBase() {
			ScrollViewerTouchBehavior.SetIsEnabled(this, true);
#if SILVERLIGHT
			AttachToEvents();
#endif
			Controller = CreateController();
#if !SILVERLIGHT
			AttachToEvents();
#endif
			AttachPropertyListener("Clip", ClipListener);
			if (NeedsChildChangeNotifications)
				InitializeChildChangeNotificationSystem();
		}
		public UIElement ChildAt(Point p) {
			return ChildAt(p, true, true);
		}
		public UIElement ChildAt(Point p, bool ignoreInternalElements, bool ignoreTempChildren) {
#if SILVERLIGHT
			var hitChildren = new List<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(this.MapPoint(p, null), this));
			if (hitChildren.Count > 1) {
				foreach (FrameworkElement element in hitChildren)
					if (element.GetParent() == this &&
						(!ignoreInternalElements || !IsInternalElement(element)) && (!ignoreTempChildren || !IsTempChild(element)))
						return element;
				return null;
			}
			else
				return null;
#else
			UIElement result = null;
			VisualTreeHelper.HitTest(this,
				delegate(DependencyObject hitObject) {
					var hitElement = hitObject as UIElement;
					if (hitElement == this)
						return HitTestFilterBehavior.Continue;
					else
						if (hitElement.IsVisible &&
							(!ignoreInternalElements || !IsInternalElement(hitElement)) && (!ignoreTempChildren || !IsTempChild(hitElement)) &&
							VisualTreeHelper.HitTest(hitElement, TranslatePoint(p, hitElement)) != null) {
							result = hitElement;
							return HitTestFilterBehavior.Stop;
						}
						else
							return HitTestFilterBehavior.ContinueSkipChildren;
				},
				(hitTestResult) => HitTestResultBehavior.Continue,
				new PointHitTestParameters(p));
			return result;
#endif
		}
		public UIElement ChildAt(Point p, bool ignoreInternalElements, bool ignoreTempChildren, bool useBounds) {
			UIElement result = ChildAt(p, ignoreInternalElements, ignoreTempChildren);
			if (useBounds && result == null && RectHelper.New(Size).Contains(p) && Visibility == Visibility.Visible)
				foreach (FrameworkElement child in GetChildren(!ignoreInternalElements, !ignoreTempChildren, true))
					if (child.Visibility == Visibility.Visible && child.GetVisualBounds().Contains(p))
						return child;
			return result;
		}
		public void DeleteChildren() {
			for(int i = Children.Count - 1; i >= 0; i--)
				if(!IsInternalElement(Children[i]))
					Children.RemoveAt(i);
		}
		public FrameworkElements GetChildren(bool visibleOnly) {
			return GetChildren(false, true, visibleOnly);
		}
		public FrameworkElements GetChildren(bool includeInternalElements, bool includeTempChildren, bool visibleOnly) {
			var result = new FrameworkElements();
			foreach(FrameworkElement element in Children)
				if((!visibleOnly || element.Visibility != Visibility.Collapsed) &&
					(includeInternalElements || !IsInternalElement(element)) &&
					(includeTempChildren || !IsTempChild(element)))
					result.Add(element);
			return result;
		}
		public FrameworkElements GetLogicalChildren(bool visibleOnly) {
			return GetChildren(false, false, visibleOnly);
		}
		public FrameworkElements GetVisibleChildren() {
			return GetVisibleChildren(false, true);
		}
		public FrameworkElements GetVisibleChildren(bool includeInternalElements, bool includeTempChildren) {
			return GetChildren(includeInternalElements, includeTempChildren, true);
		}
		public Rect AbsoluteBounds { get { return this.GetBounds(null); } }
		public Point AbsolutePosition { get { return this.GetPosition(null); } }
		public Rect Bounds { get { return this.GetBounds(); } }
		public Rect ChildrenBounds { get; private set; }
		public Rect ClientBounds { get { return CalculateClientBounds(Size); } }
		public Rect ContentBounds { get { return CalculateContentBounds(ClientBounds); } }
		public PanelControllerBase Controller { get; private set; }
#if SILVERLIGHT
		public bool IsLoaded {
			get { return _IsLoaded && Parent != null; }
		}
#endif
		public event EventHandler StartDrag {
			add { Controller.StartDrag += value; }
			remove { Controller.StartDrag -= value; }
		}
		public event EventHandler EndDrag {
			add { Controller.EndDrag += value; }
			remove { Controller.EndDrag -= value; }
		}
		#region Children
		private static bool _IsInitializingChildPropertyListener;
		protected static DependencyProperty RegisterChildPropertyListener(string propertyName, Type ownerType) {
			return DependencyProperty.RegisterAttached("Child" + propertyName + "Listener", typeof(object), ownerType,
				new PropertyMetadata(OnChildPropertyChanged));
		}
		protected static void AttachChildPropertyListener(FrameworkElement child, string propertyName, DependencyProperty propertyListener) {
			_IsInitializingChildPropertyListener = true;
			try {
				child.SetBinding(propertyListener, new Binding(propertyName) { Source = child });
			}
			finally {
				_IsInitializingChildPropertyListener = false;
			}
		}
		protected static void DetachChildPropertyListener(FrameworkElement child, DependencyProperty propertyListener) {
			_IsInitializingChildPropertyListener = true;
			try {
				child.ClearValue(propertyListener);
			}
			finally {
				_IsInitializingChildPropertyListener = false;
			}
		}
		private static void OnChildPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			if (_IsInitializingChildPropertyListener)
				return;
			var element = o as FrameworkElement;
			if (element == null)
				return;
			var parent = element.GetParent() as PanelBase;
			if (parent != null)
				parent.OnChildPropertyChanged(element, e.Property, e.OldValue, e.NewValue);
		}
		protected virtual void AttachChildPropertyListeners(FrameworkElement child) {
		}
		protected virtual void DetachChildPropertyListeners(FrameworkElement child) {
		}
		protected void CheckInternalElementsParent() {
#if !SILVERLIGHT
			if (!IsInitialized)
				return;
#endif
			foreach (UIElement element in GetInternalElements())
				if (((FrameworkElement)element).GetParent() == null)
					Children.Add(element);
		}
		protected virtual ChildInfo CreateChildInfo(FrameworkElement child) {
			return new ChildInfo(child);
		}
		protected virtual Rect GetChildBounds(FrameworkElement child) {
			return LayoutInformation.GetLayoutSlot(child);
		}
		protected virtual Rect GetChildrenBounds() {
			var result = Rect.Empty;
			foreach(var child in GetLogicalChildren(true))
				result.Union(GetChildBounds(child));
			return result;
		}
		protected virtual IEnumerable<UIElement> GetInternalElements() {
			foreach (UIElement element in Controller.GetInternalElements())
				yield return element;
		}
		protected bool IsInternalElement(UIElement element) {
			foreach (UIElement internalElement in GetInternalElements())
				if (element == internalElement)
					return true;
			return false;
		}
		protected bool IsLogicalChild(UIElement child) {
			return !IsInternalElement(child) && !IsTempChild(child);
		}
		protected virtual bool IsTempChild(UIElement child) {
			return false;
		}
		protected virtual void OnChildAdded(FrameworkElement child) {
			AttachChildPropertyListeners(child);
		}
		protected virtual void OnChildRemoved(FrameworkElement child) {
			DetachChildPropertyListeners(child);
		}
		protected virtual void OnChildPropertyChanged(FrameworkElement child, DependencyProperty propertyListener, object oldValue, object newValue) {
		}
		protected double ChildrenMinWidth {
			get { return GetChildrenSize(0.0, child => child.MinWidth, Math.Max); }
		}
		protected double ChildrenMinHeight {
			get { return GetChildrenSize(0.0, child => child.MinHeight, Math.Max); }
		}
		protected Size ChildrenMinSize { get { return new Size(ChildrenMinWidth, ChildrenMinHeight); } }
		protected double ChildrenMaxWidth {
			get { return GetChildrenSize(double.PositiveInfinity, child => child.MaxWidth, Math.Min); }
		}
		protected double ChildrenMaxHeight {
			get { return GetChildrenSize(double.PositiveInfinity, child => child.MaxHeight, Math.Min); }
		}
		protected Size ChildrenMaxSize { get { return new Size(ChildrenMaxWidth, ChildrenMaxHeight); } }
		protected virtual bool NeedsChildChangeNotifications { get { return false; } }
		private double GetChildrenSize(double defaultValue, Func<FrameworkElement, double> getChildSize, Func<double, double, double> compareSizes) {
			var result = defaultValue;
			foreach(var child in GetLogicalChildren(false))
				result = compareSizes(result, getChildSize(child));
			return result;
		}
		private List<ChildInfo> _ChildInfos;
		private void InitializeChildChangeNotificationSystem() {
			_ChildInfos = new List<ChildInfo>();
		}
		private void CheckChildChanges() {
			if(_ChildInfos != null)
				UpdateChildInfos(_ChildInfos, GetLogicalChildren(false));
		}
		private void UpdateChildInfos(List<ChildInfo> childInfos, FrameworkElements children) {
			var i = 0;
			while(i < childInfos.Count) {
				var childInfo = childInfos[i];
				var index = children.IndexOf(childInfo.Instance);
				if(index == -1) {
					OnChildRemoved(childInfo.Instance);
					childInfos.RemoveAt(i);
				}
				else {
					childInfo.Update();
					children.RemoveAt(index);
					i++;
				}
			}
			foreach(var child in children) {
				childInfos.Add(CreateChildInfo(child));
				OnChildAdded(child);
			}
		}
		protected class ChildInfo {
			public ChildInfo(FrameworkElement instance) {
				Instance = instance;
				Initialize();
			}
			public void Update() {
				OnUpdate();
				Initialize();
			}
			public FrameworkElement Instance { get; private set; }
			protected virtual void Initialize() {
			}
			protected virtual void OnUpdate() {
			}
		}
		#endregion Children
		#region Layout
		protected override Size MeasureOverride(Size availableSize) {
			IsMeasuring = true;
			CheckInternalElementsParent();
			OnBeforeMeasure(availableSize);
			Size result;
			bool needsRemeasuring;
			Controller.ResetScrollBarsVisibility();
			do {
				needsRemeasuring = false;
				Rect contentBounds = CalculateContentBounds(availableSize);
				Size contentSize = contentBounds.Size();
				result = OnMeasure(contentSize);
				UpdateOriginalDesiredSize(ref result);
				OriginalDesiredSize = result;
				if (Controller.IsScrollable()) {
					result.Width = Math.Min(result.Width, contentSize.Width);
					result.Height = Math.Min(result.Height, contentSize.Height);
				}
				bool isContentEmpty = result.Width == 0 || result.Height == 0;
				Size = availableSize;
				ChildrenBounds = new Rect(contentBounds.Location(), OriginalDesiredSize);
				Controller.UpdateScrollBarsVisibility();
				result = CalculateSizeFromContentSize(result);
				if (isContentEmpty) {
					result.Width = Math.Min(result.Width, availableSize.Width);
					result.Height = Math.Min(result.Height, availableSize.Height);
					break;
				}
				else
					if (Controller.IsScrollable()) {
						needsRemeasuring = result.Width - availableSize.Width >= 1 || result.Height - availableSize.Height >= 1;
						if (OriginalDesiredSize.Width - contentSize.Width >= 1 && availableSize.Width - result.Width >= 1)
							needsRemeasuring = true;
						if (OriginalDesiredSize.Height - contentSize.Height >= 1 && availableSize.Height - result.Height >= 1)
							needsRemeasuring = true;
					}
			}
			while (needsRemeasuring);
			var controllerAvailableSize = new Size(
				double.IsInfinity(availableSize.Width) ? result.Width : availableSize.Width,
				double.IsInfinity(availableSize.Height) ? result.Height : availableSize.Height);
			Controller.OnMeasure(controllerAvailableSize);
			IsMeasuring = false;
			return result;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			IsArranging = true;
			OnBeforeArrange(finalSize);
			Size result;
			do {
				result = OnArrange(CalculateContentBounds(finalSize));
				result = CalculateSizeFromContentSize(result);
				result.Width = Math.Min(result.Width, finalSize.Width);
				result.Height = Math.Min(result.Height, finalSize.Height);
				Size = result;
				ChildrenBounds = GetChildrenBounds();
			} while (Controller.UpdateScrolling());
			Controller.OnArrange(finalSize);
			IsArranging = false;
			return result;
		}
		protected virtual void OnBeforeMeasure(Size availableSize) {
			Controller.MeasureScrollBars();
		}
		protected virtual void OnBeforeArrange(Size finalSize) {
		}
		protected virtual Size OnMeasure(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
		protected virtual Size OnArrange(Rect bounds) {
			return base.ArrangeOverride(bounds.Size());
		}
		protected virtual Thickness GetClientPadding() {
			var result = new Thickness(0);
			Controller.GetClientPadding(ref result);
			return result;
		}
		protected virtual Thickness GetContentPadding() {
			return new Thickness(0);
		}
		protected virtual void UpdateOriginalDesiredSize(ref Size originalDesiredSize) {
		}
		protected Rect CalculateClientBounds(Size size) {
			var result = RectHelper.New(size);
			RectHelper.Deflate(ref result, ClientPadding);
			return result;
		}
		protected Rect CalculateContentBounds(Rect clientBounds) {
			RectHelper.Deflate(ref clientBounds, ContentPadding);
			return clientBounds;
		}
		protected Rect CalculateContentBounds(Size size) {
			return CalculateContentBounds(CalculateClientBounds(size));
		}
		protected Size CalculateSizeFromClientSize(Size clientSize) {
			SizeHelper.Inflate(ref clientSize, ClientPadding);
			return clientSize;
		}
		protected Size CalculateSizeFromContentSize(Size contentSize) {
			SizeHelper.Inflate(ref contentSize, TotalPadding);
			return contentSize;
		}
		protected Thickness ClientPadding { get { return GetClientPadding(); } }
		protected Thickness ContentPadding { get { return GetContentPadding(); } }
		protected bool IsArranging { get; private set; }
		protected bool IsMeasuring { get; private set; }
		protected Size OriginalDesiredSize { get; private set; }
		protected Size Size { get; private set; }
		protected Thickness TotalPadding {
			get {
				Thickness result = ClientPadding;
				ThicknessHelper.Inc(ref result, ContentPadding);
				return result;
			}
		}
		#endregion Layout
		protected virtual PanelControllerBase CreateController() {
			return new PanelControllerBase(this);
		}
		protected void AttachPropertyListener(string propertyName, DependencyProperty propertyListener) {
			_IsAttachingPropertyListener = true;
			try {
				SetBinding(propertyListener, new Binding(propertyName) { Source = this });
			}
			finally {
				_IsAttachingPropertyListener = false;
			}
		}
		protected virtual void AttachToEvents() {
			LayoutUpdated += delegate {
				if (!LayoutUpdatedHelper.GlobalLocker.IsLocked)
					OnLayoutUpdated();
			};
			Loaded += (sender, e) => OnLoaded();
			SizeChanged += (sender, e) => OnSizeChanged(e);
			Unloaded += (sender, e) => OnUnloaded();
		}
		protected virtual Geometry GetGeometry() {
			return new RectangleGeometry { Rect = RectHelper.New(Size) };
		}
		protected virtual void OnAttachedPropertyChanged(FrameworkElement child, DependencyProperty property, object oldValue, object newValue) {
		}
#if !SILVERLIGHT
		protected override void OnInitialized(EventArgs e) {
			Controller.OnInitialized();
			base.OnInitialized(e);
		}
#endif
		protected virtual void OnLayoutUpdated() {
			CheckInternalElementsParent();
			CheckChildChanges();
		}
		protected virtual void OnLoaded() {
#if SILVERLIGHT
			_IsLoaded = true;
#endif
			CheckInternalElementsParent();
			CheckChildChanges();
		}
		protected virtual void OnPropertyChanged(DependencyProperty propertyListener, object oldValue, object newValue) {
			if (propertyListener == ClipListener && newValue == null)
				UpdateClip();
		}
		protected virtual void OnSizeChanged(SizeChangedEventArgs e) {
			Size = e.NewSize;
			UpdateClip();
		}
		protected virtual void OnUnloaded() {
#if SILVERLIGHT
			_IsLoaded = false;
#endif
		}
		protected void UpdateClip() {
			if (!IsClipped || _IsUpdatingClip)
				return;
			_IsUpdatingClip = true;
			try {
				var geometry = GetGeometry();
				if (Clip == null || Clip.GetType() == geometry.GetType())
					Clip = geometry;
			}
			finally {
				_IsUpdatingClip = false;
			}
		}
		protected void Changed() {
			InvalidateMeasure();
		}
		protected virtual bool IsClipped { get { return Controller.IsScrollable(); } }
		#region IControl
		FrameworkElement IControl.Control { get { return this; } }
		Controller IControl.Controller { get { return Controller; } }
		#endregion IControl
		#region IPanel
		Size IPanel.ActualDesiredSize {
			get {
				Size result = CalculateSizeFromContentSize(OriginalDesiredSize);
				if (!double.IsNaN(Width))
					result.Width = this.GetRealWidth();
				if (!double.IsNaN(Height))
					result.Height = this.GetRealHeight();
				SizeHelper.UpdateMinSize(ref result, this.GetMaxSize());
				SizeHelper.UpdateMaxSize(ref result, this.GetMinSize());
				SizeHelper.Inflate(ref result, Margin);
				return result;
			}
		}
		#endregion IPanel
		#region IScrollBarUpdated
		event EventHandler IScrollBarUpdated.ScrollBarUpdated {
			add { Controller.ScrollParamsChanged += value; }
			remove { Controller.ScrollParamsChanged -= value; }
		}
		#endregion IScrollBarUpdated
	}
	public class PanelControllerBase : Controller {
		public PanelControllerBase(IPanel control) : base(control) { }
		public virtual void GetClientPadding(ref Thickness padding) {
			if (HasScrollBars() && !FloatingScrollBars)
				GetScrollBarPadding(ref padding);
		}
#if !SILVERLIGHT
		public virtual void OnInitialized() {
			if (HorzScrollBar == null)
				return;
			Children.Add(HorzScrollBar);
			Children.Add(VertScrollBar);
			Children.Add(CornerBox);
		}
#endif
		public virtual void OnMeasure(Size availableSize) {
			if(DragAndDropController != null)
				DragAndDropController.OnMeasure(availableSize);
		}
		public virtual void OnArrange(Size finalSize) {
			if(DragAndDropController != null)
				DragAndDropController.OnArrange(finalSize);
		}
		public Rect ClientBounds { get { return IPanel.ClientBounds; } }
		public Rect ContentBounds { get { return IPanel.ContentBounds; } }
		public IPanel IPanel { get { return IControl as IPanel; } }
		protected UIElementCollection Children { get { return IPanel.Children; } }
		#region Scrolling
		private Style _HorzScrollBarStyle;
		private Style _VertScrollBarStyle;
		private Style _CornerBoxStyle;
		public virtual void MeasureScrollBars() {
			if (!HasScrollBars())
				return;
			var availableSize = SizeHelper.Infinite;
			HorzScrollBar.Measure(availableSize);
			VertScrollBar.Measure(availableSize);
			CornerBox.Measure(availableSize);
		}
		public Style HorzScrollBarStyle {
			get { return _HorzScrollBarStyle; }
			set {
				if (HorzScrollBarStyle == value)
					return;
				_HorzScrollBarStyle = value;
				if (HorzScrollBar != null) {
					HorzScrollBar.SetValueIfNotDefault(ScrollBar.StyleProperty, HorzScrollBarStyle);
#if SILVERLIGHT
					if (HorzScrollBarStyle == null)
						StyleManager.UpdateApplicationTheme(HorzScrollBar);
#endif
				}
			}
		}
		public Style VertScrollBarStyle {
			get { return _VertScrollBarStyle; }
			set {
				if (VertScrollBarStyle == value)
					return;
				_VertScrollBarStyle = value;
				if (VertScrollBar != null) {
					VertScrollBar.SetValueIfNotDefault(ScrollBar.StyleProperty, VertScrollBarStyle);
#if SILVERLIGHT
					if (VertScrollBarStyle == null)
						StyleManager.UpdateApplicationTheme(VertScrollBar);
#endif
				}
			}
		}
		public Style CornerBoxStyle {
			get { return _CornerBoxStyle; }
			set {
				if (CornerBoxStyle == value)
					return;
				_CornerBoxStyle = value;
				if (CornerBox != null)
					CornerBox.SetValueIfNotDefault(CornerBox.StyleProperty, CornerBoxStyle);
			}
		}
		protected override void CheckScrollBars() {
			if(HasScrollBars())
				CreateScrollBars();
			else
				DestroyScrollBars();
		}
		private void CreateScrollBars() {
			if (HorzScrollBar != null)
				return;
			HorzScrollBar = new ScrollBar { Orientation = Orientation.Horizontal };
			InitScrollBar(HorzScrollBar);
			HorzScrollBar.SetValueIfNotDefault(ScrollBar.StyleProperty, HorzScrollBarStyle);
			VertScrollBar = new ScrollBar { Orientation = Orientation.Vertical };
			InitScrollBar(VertScrollBar);
			VertScrollBar.SetValueIfNotDefault(ScrollBar.StyleProperty, VertScrollBarStyle);
			CornerBox = new CornerBox();
			CornerBox.SetValueIfNotDefault(CornerBox.StyleProperty, CornerBoxStyle);
			CornerBox.SetZIndex(PanelBase.HighZIndex);
#if !SILVERLIGHT
			if (!Control.IsInitialized)
				return;
#endif
			Children.Add(HorzScrollBar);
			Children.Add(VertScrollBar);
			Children.Add(CornerBox);
		}
		private void DestroyScrollBars() {
			if (HorzScrollBar == null)
				return;
			if (Children.Contains(HorzScrollBar))
				Children.Remove(HorzScrollBar);
			if (Children.Contains(VertScrollBar))
				Children.Remove(VertScrollBar);
			if (Children.Contains(CornerBox))
				Children.Remove(CornerBox);
			HorzScrollBar.Scroll -= ScrollBarScroll;
			VertScrollBar.Scroll -= ScrollBarScroll;
			HorzScrollBar = null;
			VertScrollBar = null;
			CornerBox = null;
		}
		private void InitScrollBar(ScrollBar scrollBar) {
#if SILVERLIGHT
			StyleManager.SetApplyApplicationTheme(scrollBar, true);
#endif
			scrollBar.SetBinding(ScrollBarExtensions.ScrollViewerMouseMovedProperty,
				new Binding { Source = Control, Path = new PropertyPath(ScrollBarExtensions.ScrollViewerMouseMovedProperty) });
			scrollBar.SetBinding(ScrollBarExtensions.ScrollViewerSizeChangedProperty,
				new Binding { Source = Control, Path = new PropertyPath(ScrollBarExtensions.ScrollViewerSizeChangedProperty) });
			scrollBar.SetZIndex(PanelBase.HighZIndex);
			scrollBar.Visibility = Visibility.Collapsed;
			scrollBar.Scroll += ScrollBarScroll;
		}
		protected override void UpdateScrollBars() {
			HorzScrollBar.SetVisible(IsHorzScrollBarVisible);
			VertScrollBar.SetVisible(IsVertScrollBarVisible);
			HorzScrollBar.Arrange(IsHorzScrollBarVisible ? GetHorzScrollBarBounds() : UIElementExtensions.InvisibleBounds);
			VertScrollBar.Arrange(IsVertScrollBarVisible ? GetVertScrollBarBounds() : UIElementExtensions.InvisibleBounds);
			bool isCornerBoxVisible = IsHorzScrollBarVisible && IsVertScrollBarVisible && !FloatingScrollBars;
			CornerBox.Arrange(isCornerBoxVisible ? GetCornerBoxBounds() : UIElementExtensions.InvisibleBounds);
		}
		protected virtual Rect GetHorzScrollBarBounds() {
			var result = ScrollBarAreaBounds;
			result.Y = result.Bottom;
			result.Height = HorzScrollBar.DesiredSize.Height;
			return result;
		}
		protected virtual Rect GetVertScrollBarBounds() {
			var result = ScrollBarAreaBounds;
			result.X = result.Right;
			result.Width = VertScrollBar.DesiredSize.Width;
			return result;
		}
		protected virtual Rect GetCornerBoxBounds() {
			return new Rect(ScrollBarAreaBounds.BottomRight, new Size(VertScrollBar.DesiredSize.Width, HorzScrollBar.DesiredSize.Height));
		}
		private void GetScrollBarPadding(ref Thickness padding) {
			if (IsHorzScrollBarVisible)
				padding.Bottom += HorzScrollBar.DesiredSize.Height;
			if (IsVertScrollBarVisible)
				padding.Right += VertScrollBar.DesiredSize.Width;
		}
		protected virtual bool FloatingScrollBars {
			get { return ScrollBarExtensions.GetScrollBarMode(Control) == ScrollBarMode.TouchOverlap; }
		}
		protected override Rect ScrollableAreaBounds { get { return ClientBounds; } }
		protected virtual Rect ScrollBarAreaBounds {
			get {
				Rect result = ClientBounds;
				if (FloatingScrollBars) {
					var scrollBarPadding = new Thickness(0);
					GetScrollBarPadding(ref scrollBarPadding);
					RectHelper.Deflate(ref result, scrollBarPadding);
				}
				return result;
			}
		}
		#endregion Scrolling
	}
	public static class LayoutUpdatedHelper {
		[ThreadStatic]
		static Locker globalLocker;
		public static Locker GlobalLocker {
			get {
				if (globalLocker == null)
					globalLocker = new Locker();
				return globalLocker;
			}
		}
	}
}

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
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_PaneContent", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "PART_Sizer", Type = typeof(ElementSizer))]
	public class AutoHidePane : psvContentControl, IDisposable, IUIElement {
		#region static
		public static readonly DependencyProperty PanelSizeProperty;
		public static readonly DependencyProperty DockTypeProperty;
		static readonly DependencyPropertyKey DockTypePropertyKey;
		public static readonly DependencyProperty AutoHideTrayProperty;
		static readonly DependencyPropertyKey IsCollapsedPropertyKey;
		public static readonly DependencyProperty IsCollapsedProperty;
		static readonly DependencyPropertyKey IsSizerVisiblePropertyKey;
		public static readonly DependencyProperty IsSizerVisibleProperty;
		public static readonly DependencyProperty DisplayModeProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty AutoHideSizeProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty SizeToContentProperty;
		static AutoHidePane() {
			var dProp = new DependencyPropertyRegistrator<AutoHidePane>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("PanelSize", ref PanelSizeProperty, (double)0.0, null,
				(dObj, value) => ((AutoHidePane)dObj).CoercePanelSize((double)value));
			dProp.RegisterReadonly("DockType", ref DockTypePropertyKey, ref DockTypeProperty, SWC.Dock.Left);
			dProp.Register("AutoHideTray", ref AutoHideTrayProperty, (AutoHideTray)null,
				(dObj, e) => ((AutoHidePane)dObj).OnAutoHideTrayChanged(e.OldValue as AutoHideTray, e.NewValue as AutoHideTray));
			dProp.RegisterReadonly("IsCollapsed", ref IsCollapsedPropertyKey, ref IsCollapsedProperty, true,
				(dObj, e) => ((AutoHidePane)dObj).OnIsCollapsedChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.RegisterReadonly("IsSizerVisible", ref IsSizerVisiblePropertyKey, ref IsSizerVisibleProperty, false);
			dProp.Register("DisplayMode", ref DisplayModeProperty, AutoHideMode.Default,
				(dObj, e) => ((AutoHidePane)dObj).OnDisplayModeChanged((AutoHideMode)e.OldValue, (AutoHideMode)e.NewValue));
			dProp.Register("AutoHideSize", ref AutoHideSizeProperty, new Size(),
				(d, e) => ((AutoHidePane)d).OnAutoHideSizeChanged((Size)e.OldValue, (Size)e.NewValue));
			dProp.Register("SizeToContent", ref SizeToContentProperty, SizeToContent.Manual,
				(d, e) => ((AutoHidePane)d).OnSizeToContentChanged((SizeToContent)e.OldValue, (SizeToContent)e.NewValue));
		}
		#endregion static
		public AutoHidePane() {
			ContentControlHelper.SetContentIsNotLogical(this, true);
		}
		protected override void OnDispose() {
			CancelAnimation();
			UnSubscribe(AutoHideTray);
			if(PartAutoHidePresenter != null) {
				PartAutoHidePresenter.Dispose();
				PartAutoHidePresenter = null;
			}
			if(Container != null) Container.SizeChanged -= OnContainerSizeChanged;
			base.OnDispose();
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return AutoHideTray; } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get {
				if(uiChildren == null) uiChildren = new UIChildren();
				return uiChildren;
			}
		}
		#endregion IUIElement
		public double PanelSize {
			get { return (double)GetValue(PanelSizeProperty); }
			set { SetValue(PanelSizeProperty, value); }
		}
		public SWC.Dock DockType {
			get { return (SWC.Dock)GetValue(DockTypeProperty); }
			internal set { this.SetValue(DockTypePropertyKey, value); }
		}
		public AutoHideTray AutoHideTray {
			get { return (AutoHideTray)GetValue(AutoHideTrayProperty); }
			set { SetValue(AutoHideTrayProperty, value); }
		}
		public bool IsCollapsed {
			get { return (bool)GetValue(IsCollapsedProperty); }
			internal set { SetValue(IsCollapsedPropertyKey, value); }
		}
		public bool IsSizerVisible {
			get { return (bool)GetValue(IsSizerVisibleProperty); }
			private set { SetValue(IsSizerVisiblePropertyKey, value); }
		}
		public AutoHideMode DisplayMode {
			get { return (AutoHideMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		SizeToContent SizeToContent {
			get { return (SizeToContent)GetValue(SizeToContentProperty); }
			set { SetValue(SizeToContentProperty, value); }
		}
		Size AutoHideSize {
			get { return (Size)GetValue(AutoHideSizeProperty); }
			set { SetValue(AutoHideSizeProperty, value); }
		}
		bool IsHorz { get { return Orientation == Orientation.Vertical; } }
		protected Orientation Orientation { get { return AutoHideTray.GetOrientation(this); } }
		protected internal double Size { get; private set; }
		LayoutPanel LayoutPanel { get { return LayoutItem as LayoutPanel; } }
		AnimationContext _Context;
		AnimationContext Context {
			get {
				if(_Context == null) _Context = new AnimationContext();
				return _Context;
			}
		}
		protected virtual void OnAutoHideTrayChanged(AutoHideTray oldTray, AutoHideTray newTray) {
			UnSubscribe(oldTray);
			Subscribe(newTray);
		}
		void Subscribe(AutoHideTray tray) {
			if(tray == null) return;
			((IUIElement)tray).Children.Add(this);
			tray.Collapsed += OnCollapsed;
			tray.Expanded += OnExpanded;
			tray.PanelClosed += OnPanelClosed;
			tray.HotItemChanged += OnHotItemChanged;
			tray.PanelResizing += OnPanelResizing;
			tray.PanelMaximized += OnPanelMaximized;
			tray.PanelRestored += OnPanelRestored;
		}
		void UnSubscribe(AutoHideTray tray) {
			if(tray == null) return;
			((IUIElement)tray).Children.Remove(this);
			tray.Collapsed -= OnCollapsed;
			tray.Expanded -= OnExpanded;
			tray.PanelClosed -= OnPanelClosed;
			tray.HotItemChanged -= OnHotItemChanged;
			tray.PanelResizing -= OnPanelResizing;
			tray.PanelMaximized -= OnPanelMaximized;
			tray.PanelRestored -= OnPanelRestored;
		}
		public bool CanCollapse {
			get { return CanCollapseCore(); }
		}
		public bool CanHideCurrentItem {
			get { return CanHideCurrentItemCore(); }
		}
		protected virtual object CoercePanelSize(double size) {
			if(isClosing > 0) size = 0.0;
			if(IsSizing) size = Size;
			return size;
		}
		protected virtual bool CanHideCurrentItemCore() {
			if(KeyboardFocusHelper.IsKeyboardFocusWithin(this)) {
				DependencyObject child = KeyboardFocusHelper.FocusedElement;
				if(child != null) {
					DependencyObject root = LayoutHelper.FindRoot(child);
					return (root != null) && !DockLayoutManagerHelper.IsPopupRoot(root) && !Core.DragManager.GetIsDragging(root);
				}
			}
			return true;
		}
		protected virtual bool CanCollapseCore() {
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(this);
			bool fActive = (item != null) && item.IsActive;
			return !fActive && !(KeyboardFocusHelper.IsKeyboardFocused(this) || KeyboardFocusHelper.IsKeyboardFocusWithin(this));
		}
		DoubleAnimation CollapseAnimation { get; set; }
		protected virtual void OnCollapsed(object sender, RoutedEventArgs e) {
			AutoHideGroup aGroup = GetAutoHideGroup();
			if(aGroup == null) return;
			animatedGroup = aGroup;
			CompletePreviousAnimation();
			aGroup.IsAnimated = true;
			bool fHorz = Orientation == Orientation.Vertical;
			double actualSize = GetActualSize(fHorz);
			double from = !double.IsNaN(PanelSize) ? Math.Min(PanelSize, actualSize) : actualSize;
			CollapseAnimation = new DoubleAnimation() { From = from, To = 0.0, Duration = new Duration(TimeSpan.FromMilliseconds(aGroup.AutoHideSpeed)) };
			CollapseAnimation.Completed += OnCollapseAnimationCompleted;
			AutoHideTray.IsAnimated = true;
			BeginAnimation(PanelSizeProperty, CollapseAnimation);
		}
		void OnCollapseAnimationCompleted(object sender, EventArgs e) {
			CollapseAnimation.Completed -= OnCollapseAnimationCompleted;
			CollapseAnimation = null;
			AutoHideTray.IsAnimated = false;
			ClosePaneCore();
			animatedGroup = null;
			IsCollapsed = true;
			Size = 0d;
			Visibility = System.Windows.Visibility.Collapsed;
		}
		AutoHideGroup animatedGroup = null;
		DoubleAnimation ExpandAnimation { get; set; }
		double GetActualSize(bool isHorz) {
			if(DisplayMode != AutoHideMode.Inline || PartPaneContent == null)
				return isHorz ? ActualWidth : ActualHeight;
			return isHorz ? PartPaneContent.ActualWidth : PartPaneContent.ActualHeight;
		}
		double CalcPanelSize(PanelSizeAction action, AutoHideGroup aGroup, BaseLayoutItem LayoutItem, double desiredSize = double.NaN) {
			bool fHorz = IsHorz;
			bool max = LayoutPanel.Return(x => x.AutoHideExpandState == AutoHideExpandState.Expanded, () => false);
			if(max) action = PanelSizeAction.Maximize;
			double availableAutoHideSize = GetAvailableAutoHideSize(fHorz);
			bool hasAutoHideSize = !double.IsNaN(availableAutoHideSize);
			double Size = 0d;
			switch(action) {
				case PanelSizeAction.Expand:
					Size ahSize = aGroup.GetActualAutoHideSize(LayoutItem);
					Size = MathHelper.MeasureDimension(
						fHorz ? aGroup.ActualMinSize.Width : aGroup.ActualMinSize.Height,
						fHorz ? aGroup.ActualMaxSize.Width : aGroup.ActualMaxSize.Height,
						fHorz ? ahSize.Width : ahSize.Height
					);
					if(hasAutoHideSize) Size = Math.Min(Size, availableAutoHideSize);
					break;
				case PanelSizeAction.Maximize:
					if(hasAutoHideSize) Size = availableAutoHideSize;
					Size = MathHelper.MeasureDimension(
						fHorz ? aGroup.ActualMinSize.Width : aGroup.ActualMinSize.Height,
						fHorz ? aGroup.ActualMaxSize.Width : aGroup.ActualMaxSize.Height,
						Size
					);
					break;
				case PanelSizeAction.Resize:
					Size = MathHelper.MeasureDimension(
							fHorz ? LayoutItem.ActualMinSize.Width : LayoutItem.ActualMinSize.Height,
							fHorz ? LayoutItem.ActualMaxSize.Width : LayoutItem.ActualMaxSize.Height,
							desiredSize);
					Size = MathHelper.MeasureDimension(
						fHorz ? aGroup.ActualMinSize.Width : aGroup.ActualMinSize.Height,
						fHorz ? aGroup.ActualMaxSize.Width : aGroup.ActualMaxSize.Height,
						Size
					);
					if(hasAutoHideSize) Size = Math.Min(Size, availableAutoHideSize);
					break;
			}
			return Size;
		}
		void ExpandWithAnimation(AutoHideGroup aGroup, PanelSizeAction action, bool isAnimationAllowed = true) {
			Visibility = System.Windows.Visibility.Visible;
			Context.IsAnimationEnabled = isAnimationAllowed;
			bool fHorz = IsHorz;
			bool isAnimating = ExpandAnimation != null || CollapseAnimation != null;
			if(isAnimating) Size = GetActualSize(fHorz); 
			CompletePreviousAnimation();
			animatedGroup = aGroup;
			bool fAutoSize = GetSizeToContent(fHorz);
			if(fAutoSize) {
				InvalidateMeasure();
				LayoutUpdated += AutoHidePane_LayoutUpdated;
			}
			else {
				double currentSize = Size;
				Size = CalcPanelSize(action, aGroup, LayoutItem);
				ExpandWithAnimation(Size, currentSize);
			}
			LayoutPanel.With(x => x.ExpandAnimationLocker).Do(x => x.Lock());
			LayoutPanel.
				If(x => x.AutoHideExpandState == AutoHideExpandState.Hidden).
				Do(x => x.AutoHideExpandState = action == PanelSizeAction.Maximize ? AutoHideExpandState.Expanded : AutoHideExpandState.Visible);
			LayoutPanel.With(x => x.ExpandAnimationLocker).Do(x => x.Unlock());
			IsCollapsed = false;
		}
		protected virtual void OnExpanded(object sender, RoutedEventArgs e) {
			AutoHideGroup aGroup = GetAutoHideGroup();
			if(aGroup != null) ExpandWithAnimation(aGroup, PanelSizeAction.Expand);
		}
		bool GetSizeToContent(bool fhorz) {
			if(LayoutItem == null) return false;
			SizeToContent sizeToContent = AutoHideGroup.GetSizeToContent(LayoutItem);
			bool fAutoSize = sizeToContent == SizeToContent.WidthAndHeight || (fhorz ? sizeToContent == SizeToContent.Width : sizeToContent == SizeToContent.Height);
			return fAutoSize && (LayoutItem as LayoutPanel).Return(x => x.AutoHideExpandState != AutoHideExpandState.Expanded, () => false);
		}
		void AutoHidePane_LayoutUpdated(object sender, EventArgs e) {
			LayoutUpdated -= AutoHidePane_LayoutUpdated;
			if(double.IsNaN(Size)) Size = 0d;
			ExpandWithAnimation(IsHorz ? arrangeSize.Width : arrangeSize.Height, Size);
		}
		void ExpandWithAnimation(double to, double from = 0d) {
			AutoHideGroup aGroup = GetAutoHideGroup();
			aGroup.IsAnimated = true;
			ExpandAnimation = new DoubleAnimation() { From = from, To = to, Duration = new Duration(TimeSpan.FromMilliseconds(Context.IsAnimationEnabled ? aGroup.AutoHideSpeed : 0)) };
			ExpandAnimation.Completed += OnExpandAnimationCompleted;
			AutoHideTray.IsAnimated = true;
			BeginAnimation(PanelSizeProperty, ExpandAnimation);
		}
		Size arrangeSize;
		protected override Size ArrangeOverride(Size arrangeBounds) {
			arrangeSize = base.ArrangeOverride(arrangeBounds);
			return arrangeSize;
		}
		protected override Size MeasureOverride(Size constraint) {
			Size measureSize = constraint;
			bool fHorz = Orientation == Orientation.Vertical;
			bool fAutoSize = GetSizeToContent(fHorz);
			if(fAutoSize) {
				PanelSize = double.NaN;
				measureSize = fHorz ? new Size(double.PositiveInfinity, measureSize.Height) : new Size(measureSize.Width, double.PositiveInfinity);
			}
			Size baseSize = base.MeasureOverride(measureSize);
			return new Size(Math.Min(baseSize.Width, constraint.Width), Math.Min(baseSize.Height, constraint.Height));
		}
		void OnExpandAnimationCompleted(object sender, EventArgs e) {
			double to = ExpandAnimation.To ?? Size;
			PanelSize = to;
			Size = to;
			BeginAnimation(PanelSizeProperty, null);
			bool fAutoSize = GetSizeToContent(IsHorz);
			if(fAutoSize) PanelSize = double.NaN;
			AutoHideTray.IsAnimated = false;
			ExpandAnimation.Completed -= OnExpandAnimationCompleted;
			ExpandAnimation = null;
			AutoHideGroup aGroup = GetAutoHideGroup();
			if(aGroup != null) {
				aGroup.SelectedTabIndex = aGroup.Items.IndexOf(LayoutItem);
				aGroup.IsAnimated = false;
				aGroup.IsExpanded = true;
			}
			animatedGroup = null;
		}
		void CompletePreviousAnimation() {
			bool collapsing = CollapseAnimation != null;
			bool expanding = ExpandAnimation != null;
			CancelAnimation();
			AutoHideGroup aGroup = animatedGroup;
			if(aGroup == null) return;
			if(expanding) {
				aGroup.SelectedTabIndex = aGroup.Items.IndexOf(LayoutItem);
				aGroup.IsAnimated = false;
				aGroup.IsExpanded = true;
			}
			if(collapsing) {
				aGroup.IsAnimated = false;
				aGroup.IsExpanded = false;
				aGroup.ClearValue(LayoutGroup.SelectedTabIndexProperty);
			}
		}
		void CancelAnimation() {
			BeginAnimation(PanelSizeProperty, null);
			if(ExpandAnimation != null || CollapseAnimation != null) {
				if(ExpandAnimation != null) {
					ExpandAnimation.Completed -= OnExpandAnimationCompleted;
					ExpandAnimation = null;
				}
				if(CollapseAnimation != null) {
					CollapseAnimation.Completed -= OnCollapseAnimationCompleted;
					CollapseAnimation = null;
				}
			}
		}
		protected virtual void OnHotItemChanged(object sender, HotItemChangedEventArgs e) {
			SetValue(DockLayoutManager.LayoutItemProperty, e.Hot);
			if(e.Hot != null) {
				e.Hot.SelectTemplateIfNeeded();
			}
			SetValue(ContentProperty, e.Hot);
			if(ExpandAnimation != null) RecalculatePanelSize();
		}
		int isClosing = 0;
		protected virtual void OnPanelClosed(object sender, RoutedEventArgs e) {
			isClosing++;
			CoerceValue(PanelSizeProperty);
			ClearValue(ContentProperty);
			CancelAnimation();
			ClosePaneCore();
			isClosing--;
		}
		void ClosePaneCore(BaseLayoutItem item = null) {
			AutoHideGroup aGroup = GetAutoHideGroup() ?? item.Return(x => x.Parent as AutoHideGroup, () => null);
			if(aGroup != null) {
				aGroup.IsAnimated = false;
				aGroup.IsExpanded = false;
				aGroup.ClearValue(LayoutGroup.SelectedTabIndexProperty);
			}
			(item as LayoutPanel).
				If(x => x.AutoHideExpandState != AutoHideExpandState.Hidden).
				Do(x => x.AutoHideExpandState = AutoHideExpandState.Hidden);
		}
		int isSizing = 0;
		bool IsSizing { get { return isSizing > 0; } }
		protected virtual void OnPanelResizing(object sender, PanelResizingEventArgs e) {
			AutoHideGroup aGroup = GetAutoHideGroup();
			if(LayoutItem == null || aGroup == null) return;
			isSizing++;
			LayoutItem.ResizeLockHelper.Lock();
			bool fHorz = IsHorz;
			Size = CalcPanelSize(PanelSizeAction.Resize, aGroup, LayoutItem, e.Size);
			if(LayoutItem != null) {
				Size ahSize = AutoHideGroup.GetAutoHideSize(LayoutItem);
				AutoHideGroup.SetAutoHideSize(LayoutItem, fHorz ? new Size(Size, ahSize.Height) : new Size(ahSize.Width, Size));
				AutoHideGroup.SetSizeToContent(LayoutItem, SizeToContent.Manual);
				(LayoutItem as LayoutPanel).If(x => x.AutoHideExpandState == AutoHideExpandState.Expanded).Do(x => x.AutoHideExpandState = AutoHideExpandState.Visible);
			}
			CoerceValue(PanelSizeProperty);
			isSizing--;
		}
		void OnPanelRestored(object sender, RoutedEventArgs e) {
			AutoHideGroup aGroup = GetAutoHideGroup();
			if(LayoutItem == null || aGroup == null || IsSizing) return;
			ExpandWithAnimation(aGroup, PanelSizeAction.Expand);
		}
		void OnPanelMaximized(object sender, RoutedEventArgs e) {
			AutoHideGroup aGroup = GetAutoHideGroup();
			if(LayoutItem == null || aGroup == null) return;
			ExpandWithAnimation(aGroup, PanelSizeAction.Maximize);
		}
		double GetAvailableAutoHideSize(bool isHorz) {
			if(Container == null) return 0d;
			double containerSize = Container.GetAvailableAutoHideSize(isHorz);
			if(double.IsNaN(containerSize)) return containerSize;
			double sizerSize = DisplayMode == AutoHideMode.Inline ? PartSizer.Return(x => x.Thickness, () => 0d) : 0d;
			return Math.Max(0, containerSize - sizerSize);
		}
		AutoHideGroup GetAutoHideGroup() {
			BaseLayoutItem item = DockLayoutManager.GetLayoutItem(this);
			return (item != null) ? item.Parent as AutoHideGroup : null;
		}
		public AutoHidePanePresenter PartAutoHidePresenter { get; private set; }
		public FrameworkElement PartPaneContent { get; private set; }
		public Border PartPaneBorder { get; private set; }
		ElementSizer PartSizer;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartAutoHidePresenter = GetTemplateChild("PART_Presenter") as AutoHidePanePresenter;
			PartPaneContent = GetTemplateChild("PART_PaneContent") as FrameworkElement;
			PartPaneBorder = GetTemplateChild("PART_PaneBorder") as Border;
			PartSizer = GetTemplateChild("PART_Sizer") as ElementSizer;
			if(PartPaneContent != null)
				BindPanelSizeToContentSize();
			if(PartPaneBorder != null)
				UpdatePaneBorder();
			if(Container != null) Container.SizeChanged += OnContainerSizeChanged;
		}
		Locker containerSizeLocker = new Locker();
		private void OnContainerSizeChanged(object sender, SizeChangedEventArgs e) {
			if(!DockLayoutManagerParameters.AutoHidePanelsFitToContainer || CollapseAnimation != null || IsSizing || containerSizeLocker) return;
			if(ExpandAnimation != null) {
				containerSizeLocker.LockOnce();
				return;
			}
			if(containerSizeLocker) {
				containerSizeLocker.Unlock();
				return;
			}
			double diff = IsHorz ? e.NewSize.Width - e.PreviousSize.Width : e.NewSize.Height - e.PreviousSize.Height;
			if(diff != 0d) {
				Container.InvalidateView(Container.LayoutRoot);
				RecalculatePanelSize();
			}
		}
		void BindPanelSizeToContentSize() {
			bool fHorz = (AutoHideTray.GetOrientation(this) == Orientation.Vertical);
			BindingHelper.SetBinding(PartPaneContent, fHorz ? WidthProperty : HeightProperty, this, "PanelSize");
		}
		protected override void OnLayoutItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			base.OnLayoutItemChanged(item, oldItem);
			if(oldItem != null) {
				oldItem.ClearTemplate();
				ClearValue(AutoHideSizeProperty);
				ClearValue(SizeToContentProperty);
			}
			if(item != null) {
				BindBorderCursor();
				item.SelectTemplateIfNeeded();
				DockLayoutManager.SetUIScope(item, this);
				SetBinding(AutoHideSizeProperty, new Binding() { Path = new PropertyPath(AutoHideGroup.AutoHideSizeProperty), Source = item });
				SetBinding(SizeToContentProperty, new Binding() { Path = new PropertyPath(AutoHideGroup.SizeToContentProperty), Source = item });
			}
			else {
				ClosePaneCore(oldItem);
				IsCollapsed = true;
			}
		}
		void UpdatePaneBorder() {
			if(LayoutItem != null)
				BindBorderCursor();
			PartPaneBorder.HorizontalAlignment = DockExtensions.ToHorizontalAlignment(DockType, true);
			PartPaneBorder.VerticalAlignment = DockExtensions.ToVerticalAlignment(DockType, true);
		}
		void BindBorderCursor() {
			Cursor cursor = DockType.ToCursor();
			BindingHelper.SetBinding(PartPaneBorder, CursorProperty, LayoutItem, BaseLayoutItem.AllowSizingProperty, new ConditionalCursorConverter() { Cursor = cursor });
		}
		void OnIsCollapsedChanged(bool oldValue, bool newValue) {
			UpdateIsSizerVisible();
		}
		void OnDisplayModeChanged(AutoHideMode oldValue, AutoHideMode newValue) {
			UpdateIsSizerVisible();
		}
		void UpdateIsSizerVisible() {
			IsSizerVisible = DisplayMode == AutoHideMode.Inline && !IsCollapsed && LayoutItem != null;
		}
		void RecalculatePanelSize() {
			AutoHideGroup aGroup = GetAutoHideGroup();
			if(aGroup != null && aGroup.IsExpanded)
				ExpandWithAnimation(aGroup, PanelSizeAction.Expand, false);
		}
		void OnSizeToContentChanged(SizeToContent oldValue, SizeToContent newValue) {
			bool isHorz = IsHorz;
			bool oldIsAuto = (isHorz ? oldValue == SizeToContent.Width : oldValue == SizeToContent.Height) || oldValue == SizeToContent.WidthAndHeight;
			bool newIsAuto = (isHorz ? newValue == SizeToContent.Width : newValue == SizeToContent.Height) || newValue == SizeToContent.WidthAndHeight;
			if(oldIsAuto != newIsAuto)
				RecalculatePanelSize();
		}
		void OnAutoHideSizeChanged(Size oldValue, Size newValue) {
			bool isAuto = (IsHorz ? SizeToContent == SizeToContent.Width : SizeToContent == SizeToContent.Height) || SizeToContent == SizeToContent.WidthAndHeight;
			if(!isAuto && !IsSizing)
				RecalculatePanelSize();
		}
		enum PanelSizeAction {
			Expand, Maximize, Resize
		}
		class AnimationContext {
			public bool IsAnimationEnabled { get; set; }
		}
	}
	public class AutoHideTrayHeadersGroup : psvItemsControl {
		#region static
		public static readonly DependencyProperty LayoutItemProperty;
		static AutoHideTrayHeadersGroup() {
			var dProp = new DependencyPropertyRegistrator<AutoHideTrayHeadersGroup>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("LayoutItem", ref LayoutItemProperty, (BaseLayoutItem)null,
				(dObj, ea) => ((AutoHideTrayHeadersGroup)dObj).OnLayoutItemChanged((BaseLayoutItem)ea.OldValue, (BaseLayoutItem)ea.NewValue));
		}
		#endregion static
		public BaseLayoutItem LayoutItem {
			get { return (BaseLayoutItem)GetValue(LayoutItemProperty); }
			set { SetValue(LayoutItemProperty, value); }
		}
		AutoHideTray trayCore;
		public AutoHideTrayHeadersGroup(AutoHideTray tray)
			: this() {
			trayCore = tray;
		}
		public AutoHideTrayHeadersGroup() {
		}
		protected override void OnDispose() {
			UnSubscribeTray();
			ClearValue(AutoHideTray.OrientationProperty);
			ClearValue(LayoutItemProperty);
			base.OnDispose();
		}
		protected internal virtual void EnsureLayoutItem(AutoHideGroup aGroup) {
			DockLayoutManager.SetLayoutItem(this, aGroup);
			ItemsSource = aGroup.Items;
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			SubscribeTray();
		}
		protected override void OnUnloaded() {
			UnSubscribeTray();
			base.OnUnloaded();
		}
		protected void SubscribeTray() {
			Tray.HotItemChanged += OnTrayHotItemChanged;
			Tray.Expanded += OnTrayExpanded;
			Tray.Collapsed += OnTrayCollapsed;
			Tray.PanelClosed += OnTrayPanelClosed;
		}
		protected void UnSubscribeTray() {
			Tray.HotItemChanged -= OnTrayHotItemChanged;
			Tray.Expanded -= OnTrayExpanded;
			Tray.Collapsed -= OnTrayCollapsed;
			Tray.PanelClosed -= OnTrayPanelClosed;
		}
		public AutoHideTray Tray {
			get {
				if(trayCore == null) trayCore = LayoutHelper.FindParentObject<AutoHideTray>(this);
				return trayCore;
			}
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return (item is AutoHidePaneHeaderItem);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new AutoHidePaneHeaderItem(this);
		}
		protected override void ClearContainer(DependencyObject element) {
			AutoHidePaneHeaderItem headerItem = element as AutoHidePaneHeaderItem;
			if(headerItem != null) {
				headerItem.LayoutItem.ClearTemplate();
				headerItem.Dispose();
			}
			base.ClearContainer(element);
		}
		protected override void PrepareContainer(DependencyObject element, object item) {
			AutoHidePaneHeaderItem headerItem = element as AutoHidePaneHeaderItem;
			if(headerItem != null && DockLayoutManagerExtension.IsInContainer(this)) {
				headerItem.SetValue(AutoHideTray.OrientationProperty, AutoHideTray.GetOrientation(Tray));
				headerItem.Location = Tray.DockType;
			}
			BaseLayoutItem layoutItem = item as BaseLayoutItem;
			if(layoutItem != null) {
				layoutItem.ParentLockHelper.Lock();
				if(Tray != null && Tray.HotItem == layoutItem)
					layoutItem.SelectTemplateIfNeeded();
				layoutItem.ParentLockHelper.Unlock();
			}
		}
		void OnTrayCollapsed(object sender, RoutedEventArgs e) {
			AcceptHeaderItems(
					(headerItem, item) => headerItem.IsSelected = false
				);
		}
		void OnTrayExpanded(object sender, RoutedEventArgs e) {
			AcceptHeaderItems(
					(headerItem, item) => headerItem.IsSelected = (Tray.HotItem == item)
				);
		}
		void OnTrayHotItemChanged(object sender, HotItemChangedEventArgs e) {
			AcceptHeaderItems(
					(headerItem, item) => headerItem.IsSelected = (e.Hot == item)
				);
		}
		void OnTrayPanelClosed(object sender, RoutedEventArgs e) {
			AcceptHeaderItems(
					(headerItem, item) => headerItem.IsSelected = false
				);
		}
		bool IsHeaderForItem(FrameworkElement element, BaseLayoutItem item) {
			AutoHidePaneHeaderItem headerItem = element as AutoHidePaneHeaderItem;
			return (headerItem != null) && headerItem.LayoutItem == item;
		}
		protected void AcceptHeaderItems(Action<AutoHidePaneHeaderItem, BaseLayoutItem> action) {
			foreach(BaseLayoutItem item in Items) {
				AutoHidePaneHeaderItem headerItem = LayoutHelper.FindElement(this,
						(element) => IsHeaderForItem(element, item)
					) as AutoHidePaneHeaderItem;
				if(headerItem != null) action(headerItem, item);
			}
		}
		public AutoHideTrayHeadersPanel PartHeadersPanel { get; private set; }
		protected override void EnsureItemsPanelCore(Panel itemsPanel) {
			base.EnsureItemsPanelCore(itemsPanel);
			PartHeadersPanel = itemsPanel as AutoHideTrayHeadersPanel;
			if(PartHeadersPanel != null) {
				PartHeadersPanel.Orientation = AutoHideTray.GetOrientation(Tray);
				PartHeadersPanel.UpdateActualMargin();
			}
		}
		protected virtual void OnLayoutItemChanged(BaseLayoutItem oldValue, BaseLayoutItem newValue) {
			if(newValue is AutoHideGroup)
				EnsureLayoutItem(newValue as AutoHideGroup);
		}
	}
	[TemplatePart(Name = "PART_ControlBox", Type = typeof(BaseControlBoxControl))]
	[TemplatePart(Name = "PART_CaptionControl", Type = typeof(CaptionControl))]
	public class AutoHidePaneHeaderItem : psvContentControl, IUIElement, ITabHeader {
		#region static
		static readonly DependencyPropertyKey IsSelectedPropertyKey;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty LocationProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ItemIsVisibleProperty;
		static AutoHidePaneHeaderItem() {
			var dProp = new DependencyPropertyRegistrator<AutoHidePaneHeaderItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.RegisterReadonly("IsSelected", ref IsSelectedPropertyKey, ref IsSelectedProperty, false,
				(dObj, ea) => ((AutoHidePaneHeaderItem)dObj).OnIsSelectedChanged((bool)ea.NewValue));
			dProp.Register("Location", ref LocationProperty, SWC.Dock.Left,
				(dObj, ea) => ((AutoHidePaneHeaderItem)dObj).OnLocationChanged((Dock)ea.NewValue));
			dProp.Register("ItemIsVisible", ref ItemIsVisibleProperty, true,
				(dObj, ea) => ((AutoHidePaneHeaderItem)dObj).OnItemIsVisibleChanged((bool)ea.NewValue));
		}
		#endregion static
		public AutoHidePaneHeaderItem(AutoHideTrayHeadersGroup headersGroup) {
			HeadersGroup = headersGroup;
		}
		protected override void OnDispose() {
			ClearValue(AutoHideTray.OrientationProperty);
			base.OnDispose();
		}
		protected override void Subscribe(BaseLayoutItem item) {
			base.Subscribe(item);
			BindingHelper.SetBinding(this, ItemIsVisibleProperty, LayoutItem, "IsVisible");
		}
		public AutoHideTrayHeadersGroup HeadersGroup { get; private set; }
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			internal set { this.SetValue(IsSelectedPropertyKey, value); }
		}
		public bool ItemIsVisible {
			get { return (bool)GetValue(ItemIsVisibleProperty); }
		}
		public SWC.Dock Location {
			get { return (SWC.Dock)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		public BaseControlBoxControl PartControlBox { get; private set; }
		public TemplatedCaptionControl PartCaptionControlPresenter { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartControlBox = GetTemplateChild("PART_ControlBox") as BaseControlBoxControl;
			PartCaptionControlPresenter = GetTemplateChild("PART_CaptionControlPresenter") as TemplatedCaptionControl;
			UpdateVisualState();
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateVisualState();
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			UpdateVisualState();
		}
		protected virtual void OnLocationChanged(Dock location) {
			UpdateVisualState();
		}
		public void OnIsSelectedChanged(bool newValue) {
			UpdateVisualState();
		}
		public void OnItemIsVisibleChanged(bool newValue) {
			Recalc();
		}
		void Recalc() {
			BaseHeadersPanel.Invalidate(this);
		}
		void UpdateVisualState() {
			if(IsSelected)
				VisualStateManager.GoToState(this, "Selected", false);
			else {
				if(IsMouseOver)
					VisualStateManager.GoToState(this, "MouseOver", false);
				else
					VisualStateManager.GoToState(this, "Normal", false);
			}
			switch(Location) {
				case SWC.Dock.Top:
					VisualStateManager.GoToState(this, "Top", false);
					break;
				case SWC.Dock.Bottom:
					VisualStateManager.GoToState(this, "Bottom", false);
					break;
				case SWC.Dock.Right:
					VisualStateManager.GoToState(this, "Right", false);
					break;
				default:
					VisualStateManager.GoToState(this, "Left", false);
					break;
			}
		}
		#region IHeader Members
		public TabHeaderPinLocation PinLocation { get { return TabHeaderPinLocation.Default; } }
		public bool IsPinned { get { return false; } }
		public Rect ArrangeRect { get; private set; }
		public void Apply(ITabHeaderInfo info) {
			EnsureItemCaptionElementsVisibility();
			Visibility = VisibilityHelper.Convert(ItemIsVisible && info.IsVisible);
			if(PartCaptionControl != null)
				Measure(info.Rect.GetSize());
			ArrangeRect = info.Rect;
		}
		public CaptionControl PartCaptionControl { get { return PartCaptionControlPresenter != null ? PartCaptionControlPresenter.PartCaption : null; } }
		public ITabHeaderInfo CreateInfo(Size size) {
			EnsureItemCaptionElementsVisibility();
			Visibility = VisibilityHelper.Convert(ItemIsVisible);
			Measure(size);
			return new BaseHeaderInfo(this, PartCaptionControl, PartControlBox, IsSelected);
		}
		void EnsureItemCaptionElementsVisibility() {
			if(LayoutItem != null) {
				LayoutItem.CoerceValue(BaseLayoutItem.IsCaptionVisibleProperty);
				LayoutItem.CoerceValue(BaseLayoutItem.IsCaptionImageVisibleProperty);
			}
		}
		#endregion
		#region IUIElement
		IUIElement IUIElement.Scope { get { return DockLayoutManager.GetUIScope(this); } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion IUIElement
	}
	public class AutoHideTrayHeadersPanel : BaseHeadersPanel {
		#region static
		public static readonly DependencyProperty LeftMarginProperty;
		public static readonly DependencyProperty TopMarginProperty;
		public static readonly DependencyProperty RightMarginProperty;
		public static readonly DependencyProperty BottomMarginProperty;
		static AutoHideTrayHeadersPanel() {
			var dProp = new DependencyPropertyRegistrator<AutoHideTrayHeadersPanel>();
			dProp.Register("LeftMargin", ref LeftMarginProperty, new Thickness(0),
				(dObj, e) => ((AutoHideTrayHeadersPanel)dObj).OnSideMarginChanged());
			dProp.Register("TopMargin", ref TopMarginProperty, new Thickness(0),
				(dObj, e) => ((AutoHideTrayHeadersPanel)dObj).OnSideMarginChanged());
			dProp.Register("RightMargin", ref RightMarginProperty, new Thickness(0),
				(dObj, e) => ((AutoHideTrayHeadersPanel)dObj).OnSideMarginChanged());
			dProp.Register("BottomMargin", ref BottomMarginProperty, new Thickness(0),
				(dObj, e) => ((AutoHideTrayHeadersPanel)dObj).OnSideMarginChanged());
		}
		#endregion static
		public Thickness LeftMargin {
			get { return (Thickness)GetValue(LeftMarginProperty); }
			set { SetValue(LeftMarginProperty, value); }
		}
		public Thickness TopMargin {
			get { return (Thickness)GetValue(TopMarginProperty); }
			set { SetValue(TopMarginProperty, value); }
		}
		public Thickness RightMargin {
			get { return (Thickness)GetValue(RightMarginProperty); }
			set { SetValue(RightMarginProperty, value); }
		}
		public Thickness BottomMargin {
			get { return (Thickness)GetValue(BottomMarginProperty); }
			set { SetValue(BottomMarginProperty, value); }
		}
		public AutoHideTrayHeadersGroup HeadersGroup { get; private set; }
		protected internal void UpdateActualMargin() {
			HeadersGroup = LayoutItemsHelper.GetTemplateParent<AutoHideTrayHeadersGroup>(this);
			if(HeadersGroup != null)
				UpdateActualMargin(HeadersGroup.Tray);
		}
		protected virtual void OnSideMarginChanged() {
			UpdateActualMargin();
		}
		protected void UpdateActualMargin(AutoHideTray tray) {
			if(AutoHideTray.GetIsLeft(tray))
				Margin = LeftMargin;
			if(AutoHideTray.GetIsTop(tray))
				Margin = TopMargin;
			if(AutoHideTray.GetIsRight(tray))
				Margin = RightMargin;
			if(AutoHideTray.GetIsBottom(tray))
				Margin = BottomMargin;
		}
	}
	public class AutoHidePanePresenter : psvContentControl {
		#region static
		public static readonly DependencyProperty Win32CompatibleTemplateProperty;
		static AutoHidePanePresenter() {
			var dProp = new DependencyPropertyRegistrator<AutoHidePanePresenter>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Win32CompatibleTemplate", ref Win32CompatibleTemplateProperty, (ControlTemplate)null);
		}
		#endregion
		public AutoHidePanePresenter() {
			DefaultStyleKey = typeof(AutoHidePanePresenter);
		}
		protected override void OnContentChanged(object content, object oldContent) {
			base.OnContentChanged(content, oldContent);
			Template = SelectTemplate();
		}
		private ControlTemplate SelectTemplate() {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(this);
			if(manager != null && manager.EnableWin32Compatibility) return Win32CompatibleTemplate;
			return DefaultTemplate;
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			Template = SelectTemplate();
		}
		public ControlTemplate Win32CompatibleTemplate {
			get { return (ControlTemplate)GetValue(Win32CompatibleTemplateProperty); }
			set { SetValue(Win32CompatibleTemplateProperty, value); }
		}
		const string DefaultTemplateXAML =
			@"<ControlTemplate TargetType='local:AutoHidePanePresenter' " +
				"xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
				"xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
				"xmlns:local='clr-namespace:DevExpress.Xpf.Docking.VisualElements;assembly=DevExpress.Xpf.Docking" + AssemblyInfo.VSuffix + "'>" +
				"<ContentPresenter Content='{TemplateBinding Content}' ContentTemplate='{TemplateBinding ContentTemplate}'/>" +
			"</ControlTemplate>";
		static ControlTemplate _DefaultTemplate;
		static ControlTemplate DefaultTemplate {
			get {
				if(_DefaultTemplate == null)
					_DefaultTemplate = (ControlTemplate)System.Windows.Markup.XamlReader.Parse(DefaultTemplateXAML);
				return _DefaultTemplate;
			}
		}
	}
}

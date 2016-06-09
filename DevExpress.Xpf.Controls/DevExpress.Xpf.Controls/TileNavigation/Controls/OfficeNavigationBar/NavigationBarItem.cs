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

using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.WindowsUI;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using DevExpress.Xpf.WindowsUI.Internal.Flyout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
namespace DevExpress.Xpf.Navigation {
	interface IDragItem {
		bool IsDragStarted { get; set; }
	}
	public class NavigationBarItem : veViewSelectorItem, IFlyoutEventListener, IDragItem {
		#region static
		public static readonly DependencyProperty DisplayModeProperty;
		public static readonly DependencyProperty GlyphAlignmentProperty;
		public static readonly DependencyProperty GlyphProperty;
		static readonly DependencyPropertyKey IsContentActuallyVisiblePropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsContentActuallyVisibleProperty;
		static readonly DependencyPropertyKey IsGlyphActuallyVisiblePropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsGlyphActuallyVisibleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FlyoutProperty;
		public static readonly DependencyProperty HasPeekFormProperty;
		static readonly DependencyPropertyKey HasPeekFormPropertyKey;
		public static readonly DependencyProperty CustomizationCaptionProperty;
		public static readonly DependencyProperty CustomizationCaptionTemplateProperty;
		public static readonly DependencyProperty CustomizationCaptionTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty PeekFormTemplateProperty;
		public static readonly DependencyProperty PeekFormTemplateSelectorProperty;
		public static readonly DependencyProperty IsCompactProperty;
		internal static readonly DependencyPropertyKey IsCompactPropertyKey;
		static NavigationBarItem() {
			Type ownerType = typeof(NavigationBarItem);
			DisplayModeProperty = DependencyProperty.Register("DisplayMode", typeof(ItemDisplayMode), ownerType, new PropertyMetadata(ItemDisplayMode.Default, new PropertyChangedCallback(OnDisplayModeChanged)));
			GlyphAlignmentProperty = DependencyProperty.Register("GlyphAlignment", typeof(Dock), ownerType, new UIPropertyMetadata(Dock.Left));
			GlyphProperty = DependencyProperty.Register("Glyph", typeof(ImageSource), ownerType);
			IsGlyphActuallyVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsGlyphActuallyVisible", typeof(bool), ownerType, new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsGlyphActuallyVisibleChanged), new CoerceValueCallback(OnCoerceIsGlyphActuallyVisible)));
			IsGlyphActuallyVisibleProperty = IsGlyphActuallyVisiblePropertyKey.DependencyProperty;
			IsContentActuallyVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsContentActuallyVisible", typeof(bool), ownerType, new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsContentActuallyVisibleChanged), new CoerceValueCallback(OnCoerceIsContentActuallyVisible)));
			IsContentActuallyVisibleProperty = IsContentActuallyVisiblePropertyKey.DependencyProperty;
			FlyoutProperty = DependencyProperty.Register("Flyout", typeof(FlyoutBase), ownerType, new PropertyMetadata(null, OnFlyoutChanged));
			HasPeekFormPropertyKey = DependencyProperty.RegisterReadOnly("HasPeekForm", typeof(bool), ownerType, new PropertyMetadata(false));
			HasPeekFormProperty = HasPeekFormPropertyKey.DependencyProperty;
			CustomizationCaptionProperty = DependencyProperty.Register("CustomizationCaption", typeof(object), ownerType);
			CustomizationCaptionTemplateProperty = DependencyProperty.Register("CustomizationCaptionTemplate", typeof(DataTemplate), ownerType);
			CustomizationCaptionTemplateSelectorProperty = DependencyProperty.Register("CustomizationCaptionTemplateSelector", typeof(DataTemplateSelector), ownerType);
			OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), ownerType, new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));
			PeekFormTemplateProperty = DependencyProperty.Register("PeekFormTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, OnPeekFormTemplateChanged));
			PeekFormTemplateSelectorProperty = DependencyProperty.Register("PeekFormTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, OnPeekFormTemplateSelectorChanged));
			IsCompactPropertyKey = DependencyProperty.RegisterReadOnly("IsCompact", typeof(bool), ownerType, new PropertyMetadata(false, OnIsCompactChanged));
			IsCompactProperty = IsCompactPropertyKey.DependencyProperty;
		}
		private static void OnDisplayModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationBarItem tileNavigatorItem = o as NavigationBarItem;
			if(tileNavigatorItem != null)
				tileNavigatorItem.OnDisplayModeChanged((ItemDisplayMode)e.OldValue, (ItemDisplayMode)e.NewValue);
		}
		private static object OnCoerceIsContentActuallyVisible(DependencyObject o, object value) {
			NavigationBarItem tileNavigatorItem = o as NavigationBarItem;
			if(tileNavigatorItem != null)
				return tileNavigatorItem.OnCoerceIsContentActuallyVisible((bool)value);
			else
				return value;
		}
		private static void OnIsContentActuallyVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationBarItem tileNavigatorItem = o as NavigationBarItem;
			if(tileNavigatorItem != null)
				tileNavigatorItem.OnIsContentActuallyVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		private static object OnCoerceIsGlyphActuallyVisible(DependencyObject o, object value) {
			NavigationBarItem tileNavigatorItem = o as NavigationBarItem;
			if(tileNavigatorItem != null)
				return tileNavigatorItem.OnCoerceIsGlyphActuallyVisible((bool)value);
			else
				return value;
		}
		private static void OnIsGlyphActuallyVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationBarItem tileNavigatorItem = o as NavigationBarItem;
			if(tileNavigatorItem != null)
				tileNavigatorItem.OnIsGlyphActuallyVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		private static void OnFlyoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			NavigationBarItem tileNavigatorItem = d as NavigationBarItem;
			if(tileNavigatorItem != null)
				tileNavigatorItem.OnFlyoutChanged((FlyoutBase)e.OldValue, (FlyoutBase)e.NewValue);
		}
		private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			NavigationBarItem tileNavigatorItem = d as NavigationBarItem;
			if(tileNavigatorItem != null)
				tileNavigatorItem.OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue);
		}
		private static void OnPeekFormTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationBarItem)d).OnPeekFormTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
		}
		private static void OnPeekFormTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationBarItem)d).OnPeekFormTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);
		}
		private static void OnIsCompactChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationBarItem)d).OnIsCompactChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		#endregion
		public NavigationBarItem() {
			DefaultStyleKey = typeof(NavigationBarItem);
			CoerceValue(IsContentActuallyVisibleProperty);
			CoerceValue(IsGlyphActuallyVisibleProperty);
		}
		protected override void OnIsSelectedChanged(bool isSelected) {
			StartPeekFormHide(true);
			base.OnIsSelectedChanged(isSelected);
		}
		protected virtual void OnDisplayModeChanged(ItemDisplayMode oldValue, ItemDisplayMode newValue) {
			CoerceValue(IsContentActuallyVisibleProperty);
			CoerceValue(IsGlyphActuallyVisibleProperty);
		}
		protected virtual bool OnCoerceIsContentActuallyVisible(bool value) {
			return DisplayMode != ItemDisplayMode.Glyph && !IsCompact;
		}
		protected virtual void OnIsContentActuallyVisibleChanged(bool oldValue, bool newValue) { }
		protected virtual bool OnCoerceIsGlyphActuallyVisible(bool value) {
			return DisplayMode != ItemDisplayMode.Content || IsCompact;
		}
		protected virtual void OnIsGlyphActuallyVisibleChanged(bool oldValue, bool newValue) { }
		internal void ShowPeekForm() {
			ShowPeekTimer.Stop();
			EnsurePeekForm();
			if(!HasPeekForm) return;
			OfficeNavigationBar bar = Owner as OfficeNavigationBar;
			if(bar == null || !bar.ShowPeekFormOnItemHover) return;
			IFlyoutProvider provider = LayoutHelper.FindParentObject<IFlyoutProvider>(this);
			if(provider == null) {
			}
			if(provider != null) {
				Flyout.FlyoutControl = provider.FlyoutControl;
				Flyout.PlacementTarget = this;
				Flyout.Placement = provider.Placement;
				IFlyoutEventListener prevListener = provider.FlyoutEventListener;
				if(prevListener != null && prevListener != this) {
					prevListener.Flyout.FlyoutControl = null;
					prevListener.Flyout.IsOpen = false;
				}
				provider.FlyoutEventListener = this;
			}
			Flyout.IsOpen = true;
		}
		void OnShowPeekTimerTick(object sender, EventArgs e) {
			ShowPeekForm();
		}
		private void EnsurePeekForm() {
			if(!HasPeekForm) {
				DataTemplate peekFormTemplate = null;
				peekFormTemplate = PeekFormTemplate;
				if(PeekFormTemplateSelector != null)
					peekFormTemplate = PeekFormTemplateSelector.SelectTemplate(Content, this);
				if(peekFormTemplate != null) {
					object peekContent = peekFormTemplate.LoadContent();
					FlyoutBase flyout = peekContent as FlyoutBase;
					if(flyout == null)
						flyout = new Flyout() { ContentTemplate = peekFormTemplate };
					Flyout = flyout;
				}
			}
		}
		private void HidePeekForm() {
			if(HasPeekForm) {
				IFlyoutProvider provider = LayoutHelper.FindParentObject<IFlyoutProvider>(this);
				if(provider != null && provider.FlyoutEventListener == this) {
					Flyout.IsOpen = false;
				}
				else {
					Flyout.FlyoutControl = null;
					Flyout.IsOpen = false;
				}
			}
		}
		void OnHidePeekTimerTick(object sender, EventArgs e) {
			HidePeekTimer.Stop();
			HidePeekForm();
		}
		private void ShowPeekOnMouseEnter() {
			ShowPeekTimer.Stop();
			HidePeekTimer.Stop();
			OfficeNavigationBar bar = Owner as OfficeNavigationBar;
			if(bar == null || !bar.ShowPeekFormOnItemHover) return;
			int peekShowDelay = bar.NavigationClient != null ? bar.NavigationClient.PeekFormShowDelay : bar.PeekFormShowDelay;
			ShowPeekTimer.Interval = TimeSpan.FromMilliseconds(peekShowDelay);
			ShowPeekTimer.Start();
		}
		private void HidePeekOnMouseLeave() {
			StartPeekFormHide();
		}
		private void StartPeekFormHide(bool immediately = false) {
			ShowPeekTimer.Stop();
			HidePeekTimer.Stop();
			OfficeNavigationBar bar = Owner as OfficeNavigationBar;
			if(bar == null || !bar.ShowPeekFormOnItemHover) return;
			if(immediately) HidePeekForm();
			else {
				int peekShowDelay = bar.NavigationClient != null ? bar.NavigationClient.PeekFormHideDelay : bar.PeekFormHideDelay;
				HidePeekTimer.Interval = TimeSpan.FromMilliseconds(peekShowDelay);
				HidePeekTimer.Start();
			}
		}
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			ShowPeekOnMouseEnter();
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			if(!IsMouseOver && !IsKeyboardFocused && !IsKeyboardFocusWithin)
				HidePeekOnMouseLeave();
		}
		protected virtual void OnFlyoutChanged(FlyoutBase oldValue, FlyoutBase newValue) {
			SetValue(HasPeekFormPropertyKey, newValue != null);
			if(oldValue != null) {
				RemoveLogicalChild(oldValue);
				logicalChildren.Remove(oldValue);
			}
			if(newValue != null) {
				AddLogicalChild(newValue);
				logicalChildren.Add(newValue);
				newValue.AllowMouseCapture = false;
			}
		}
		protected virtual void OnOrientationChanged(Orientation oldValue, Orientation newValue) {
		}
		protected virtual void OnPeekFormTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			HidePeekForm();
			Flyout = null;
		}
		protected virtual void OnPeekFormTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) {
			HidePeekForm();
			Flyout = null;
		}
		protected virtual void OnIsCompactChanged(bool oldValue, bool newValue) {
			CoerceValue(IsContentActuallyVisibleProperty);
			CoerceValue(IsGlyphActuallyVisibleProperty);
		}
		protected override Core.ControlControllerBase CreateController() {
			return new NavigationBarItemController(this);
		}
		[TypeConverter(typeof(StringConverter))]
		public object CustomizationCaption {
			get { return GetValue(CustomizationCaptionProperty); }
			set { SetValue(CustomizationCaptionProperty, value); }
		}
		public DataTemplate CustomizationCaptionTemplate {
			get { return (DataTemplate)GetValue(CustomizationCaptionTemplateProperty); }
			set { SetValue(CustomizationCaptionTemplateProperty, value); }
		}
		public DataTemplateSelector CustomizationCaptionTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CustomizationCaptionTemplateSelectorProperty); }
			set { SetValue(CustomizationCaptionTemplateSelectorProperty, value); }
		}
		public ItemDisplayMode DisplayMode {
			get { return (ItemDisplayMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		public Dock GlyphAlignment {
			get { return (Dock)GetValue(GlyphAlignmentProperty); }
			set { SetValue(GlyphAlignmentProperty, value); }
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			internal set { SetValue(OrientationProperty, value); }
		}
		public DataTemplate PeekFormTemplate {
			get { return (DataTemplate)GetValue(PeekFormTemplateProperty); }
			set { SetValue(PeekFormTemplateProperty, value); }
		}
		public DataTemplateSelector PeekFormTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PeekFormTemplateSelectorProperty); }
			set { SetValue(PeekFormTemplateSelectorProperty, value); }
		}
		public bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			internal set { SetValue(IsCompactPropertyKey, value); }
		}
		DispatcherTimer showPeekTimer;
		DispatcherTimer ShowPeekTimer {
			get {
				if(showPeekTimer == null) {
					showPeekTimer = new DispatcherTimer();
					showPeekTimer.Tick += OnShowPeekTimerTick;
				}
				return showPeekTimer;
			}
		}
		DispatcherTimer hidePeekTimer;
		DispatcherTimer HidePeekTimer {
			get {
				if(hidePeekTimer == null) {
					hidePeekTimer = new DispatcherTimer();
					hidePeekTimer.Tick += OnHidePeekTimerTick;
				}
				return hidePeekTimer;
			}
		}
		internal FlyoutBase Flyout {
			get { return (FlyoutBase)GetValue(FlyoutProperty); }
			set { SetValue(FlyoutProperty, value); }
		}
		List<object> logicalChildren = new List<object>();
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, logicalChildren.GetEnumerator()); }
		}
		public bool HasPeekForm {
			get { return (bool)GetValue(HasPeekFormProperty); }
		}
		protected internal override ClickMode SelectionMode {
			get { return ClickMode.Release; }
		}
		#region IFlyoutEventListener Members
		void IFlyoutEventListener.OnFlyoutClosed() { }
		void IFlyoutEventListener.OnFlyoutOpened() { }
		void IFlyoutEventListener.OnMouseLeave() {
			HidePeekOnMouseLeave();
		}
		void IFlyoutEventListener.OnMouseEnter() {
			ShowPeekOnMouseEnter();
		}
		void IFlyoutEventListener.OnFlyoutClosed(bool onClickThrough) {
			throw new NotImplementedException();
		}
		WindowsUI.Internal.Flyout.FlyoutBase IFlyoutEventListener.Flyout { get { return Flyout; } }
		#endregion
		#region IDragItem Members
		bool IsDragStarted;
		bool IDragItem.IsDragStarted {
			get { return IsDragStarted; }
			set { IsDragStarted = value; }
		}
		#endregion
		class NavigationBarItemController : veViewSelectorItemController {
			public NavigationBarItemController(NavigationBarItem control)
				: base(control) {
			}
			public new NavigationBarItem Control { get { return (NavigationBarItem)base.Control; } }
			protected override void OnMouseLeftButtonUp(DevExpress.Xpf.Core.DXMouseButtonEventArgs e) {
				var isClick = IsMouseLeftButtonDown && IsMouseEntered;
				base.OnMouseLeftButtonUp(e);
				if(isClick && Control.IsMouseOver && !Control.IsDragStarted) {
					Control.InvokeSelectInParentContainer();
				}
			}
		}
	}
}

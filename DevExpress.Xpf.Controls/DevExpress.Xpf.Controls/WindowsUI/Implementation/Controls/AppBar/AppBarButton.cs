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
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using DevExpress.Xpf.WindowsUI.Navigation;
using DevExpress.Xpf.WindowsUI.UIAutomation;
#if !SILVERLIGHT
using DevExpress.Xpf.WindowsUI.Internal.Flyout;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
#else
using DevExpress.Xpf.ComponentModel;
using DevExpress.Xpf.Core.WPFCompatibility;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.WindowsUI {
	public abstract class AppBarButtonBase : CommandButton, IFlyoutEventListener {
		#region static
#if !SILVERLIGHT
		public static readonly DependencyProperty FlyoutProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty HasFlyoutProperty;
#endif
		static AppBarButtonBase() {
			var dProp = new DependencyPropertyRegistrator<AppBarButtonBase>();
			dProp.Register("Flyout", ref FlyoutProperty, (FlyoutBase)null,
				(d, e) => ((AppBarButtonBase)d).OnFlyoutChanged((FlyoutBase)e.OldValue, (FlyoutBase)e.NewValue));
			dProp.Register("HasFlyout", ref HasFlyoutProperty, false,
				(d, e) => ((AppBarButtonBase)d).OnHasFlyoutChanged((bool)e.OldValue, (bool)e.NewValue));
			HorizontalAlignmentProperty.AddOwner(typeof(AppBarButtonBase), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, OnHorizontalAlignmentChanged));
		}
		private static void OnHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AppBarButtonBase button = d as AppBarButtonBase;
			if(button != null) button.OnHorizontalAlignmentChanged((HorizontalAlignment)e.OldValue, (HorizontalAlignment)e.NewValue);
		}
		#endregion
		protected AppBarButtonBase() {
			Loaded += OnLoaded;
		}
		protected virtual void OnLoaded() {
			IFlyoutProvider provider = LayoutHelper.FindParentObject<IFlyoutProvider>(this);
			if(provider == null) {
			}
			if(provider != null) {
				IFlyoutServiceProvider serviceProvider = provider as IFlyoutServiceProvider;
				FlyoutService service = serviceProvider.FlyoutService;
				if(serviceProvider != null) {
					serviceProvider.FlyoutService.RegisterListener(Flyout);
				}
			}
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			OnLoaded();			
		}
		bool IgnoreNextClick;
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			if(!e.Handled)
				e.Handled = IgnoreMouseEvents.IsLocked;
			IgnoreNextClick = IgnoreMouseEvents.IsLocked;
			base.OnMouseLeftButtonDown(e);
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			if(!e.Handled)
				e.Handled = IgnoreMouseEvents.IsLocked;
			base.OnMouseLeftButtonUp(e);
		}
		ShapeContentPresenter PartPresenter;
		public override void OnApplyTemplate() {
			if(PartPresenter != null) PartPresenter.ClearValue(ShapeContentPresenter.AllowAccentProperty);
			base.OnApplyTemplate();
#if !SILVERLIGHT
			UpdateFlyoutVisualState();
#endif
			PartPresenter = LayoutTreeHelper.GetTemplateChild<ShapeContentPresenter, AppBarButtonBase>(this) as ShapeContentPresenter;
			if(PartPresenter != null) {
				PartPresenter.SetBinding(ShapeContentPresenter.AllowAccentProperty, new Binding("AllowGlyphTheming") { Source = this });
			}
		}
#if !SILVERLIGHT
		protected virtual void OnHasFlyoutChanged(bool oldValue, bool newValue) {
			UpdateFlyoutVisualState();
		}
		protected virtual void OnFlyoutChanged(FlyoutBase oldValue, FlyoutBase newValue) {
			SetValue(HasFlyoutProperty, newValue != null);
			if(oldValue != null) {
				RemoveLogicalChild(oldValue);
				logicalChildren.Remove(oldValue);
				oldValue.Opened -= OnFlyoutOpened;
				oldValue.Closed -= OnFlyoutClosed;
				oldValue.Listener = null;
			}
			if(newValue != null) {
				AddLogicalChild(newValue);
				logicalChildren.Add(newValue);
				newValue.Opened += OnFlyoutOpened;
				newValue.Closed += OnFlyoutClosed;
				newValue.Listener = this;
			}
		}
		void OnFlyoutClosed(object sender, EventArgs e) {
			UpdateFlyoutVisualState();
		}
		void OnFlyoutOpened(object sender, EventArgs e) {
			UpdateFlyoutVisualState();
		}
		protected virtual void UpdateFlyoutVisualState() {
			if(HasFlyout) {
				VisualStateManager.GoToState(this, "FlyoutEnabled", false);
				VisualStateManager.GoToState(this, Flyout.IsOpen ? "FlyoutOpened" : "FlyoutClosed", false);
			}
			else VisualStateManager.GoToState(this, "FlyoutDisabled", false);
		}
		protected virtual bool ShowFlyoutOnClick { get { return true; } }
		protected virtual void PrepareFlyout() {
			IFlyoutProvider provider = LayoutHelper.FindParentObject<IFlyoutProvider>(this);
			if(provider == null) {
			}
			if(provider != null) {
				Flyout.FlyoutControl = provider.FlyoutControl;
				Flyout.PlacementTarget = this;
				Flyout.Placement = provider.Placement;
			}
		}
		protected virtual void ProcessFlyoutClick() {
			if(HasFlyout) {
				IFlyoutProvider provider = LayoutHelper.FindParentObject<IFlyoutProvider>(this);
				if(provider == null) {
				}
				if(provider != null) {
					IFlyoutServiceProvider serviceProvider = provider as IFlyoutServiceProvider;
					FlyoutService service = serviceProvider.FlyoutService;
					if(serviceProvider != null) {
						serviceProvider.FlyoutService.RegisterListener(Flyout);
					}
					Flyout.FlyoutControl = provider.FlyoutControl;
					Flyout.PlacementTarget = this;
					Flyout.Placement = provider.Placement;
					Flyout.Service = serviceProvider.FlyoutService;
					if(Flyout.IsOpen || Flyout.FlyoutControl.IsOpen)
						serviceProvider.FlyoutService.Do((x) => x.Hide(Flyout));
					else {
						serviceProvider.FlyoutService.Do((x) => x.Show(Flyout));
					}
				}
			}
		}
		protected override void OnClick() {
			base.OnClick();
			if(ShowFlyoutOnClick && !IgnoreMouseEvents.IsLocked) 
				if(!IgnoreNextClick)
				ProcessFlyoutClick();
		}
		private void OnHorizontalAlignmentChanged(HorizontalAlignment oldValue, HorizontalAlignment newValue) {
			AppBar bar = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<AppBar>(this);
			if(bar != null) bar.Invalidate();
		}
#endif
		List<object> logicalChildren = new List<object>();
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, logicalChildren.GetEnumerator()); }
		}
		public FlyoutBase Flyout {
			get { return (FlyoutBase)GetValue(FlyoutProperty); }
			set { SetValue(FlyoutProperty, value); }
		}
		public bool HasFlyout {
			get { return (bool)GetValue(HasFlyoutProperty); }
		}
		Locker IgnoreMouseEvents = new Locker();
		#region IFlyoutEventListener Members
		void IFlyoutEventListener.OnFlyoutClosed(bool onClickThrough) {
			IFlyoutProvider provider = LayoutHelper.FindParentObject<IFlyoutProvider>(this);
				if(provider == null) {
				}
				if(provider != null) {
					IFlyoutServiceProvider serviceProvider = provider as IFlyoutServiceProvider;
					FlyoutService service = serviceProvider.FlyoutService;
					if(serviceProvider != null) {
						if(serviceProvider.FlyoutService.IsFlyoutShown)
							if(onClickThrough) {
								IgnoreMouseEvents.LockOnce();
								Dispatcher.BeginInvoke(new Action(() => IgnoreMouseEvents.Unlock()));
							}
					}
				}
		}
		void IFlyoutEventListener.OnFlyoutClosed() {
			throw new NotImplementedException();
		}
		void IFlyoutEventListener.OnFlyoutOpened() {
			throw new NotImplementedException();
		}
		void IFlyoutEventListener.OnMouseLeave() {
			throw new NotImplementedException();
		}
		void IFlyoutEventListener.OnMouseEnter() {
			throw new NotImplementedException();
		}
		FlyoutBase IFlyoutEventListener.Flyout {
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
#if !SILVERLIGHT
#endif
	public class AppBarButton : AppBarButtonBase, IAppBarElement {
		#region static
		public static readonly DependencyProperty LabelProperty;
		public static readonly DependencyProperty LabelTemplateProperty;
		public static readonly DependencyProperty IsCompactProperty;
		public static readonly DependencyProperty ContentFontFamilyProperty;
		public static readonly DependencyProperty ContentFontSizeProperty;
#if !SILVERLIGHT
		public static readonly DependencyProperty LabelTemplateSelectorProperty;
#endif
		static AppBarButton() {
			var dProp = new DependencyPropertyRegistrator<AppBarButton>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Label", ref LabelProperty, (object)null, FrameworkPropertyMetadataOptions.AffectsMeasure);
			dProp.Register("LabelTemplate", ref LabelTemplateProperty, (DataTemplate)null, FrameworkPropertyMetadataOptions.AffectsMeasure);
			dProp.Register("IsCompact", ref IsCompactProperty, false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((AppBarButton)d).OnIsCompactChanged((bool)e.NewValue));
			dProp.Register("ContentFontFamily", ref ContentFontFamilyProperty, new FontFamily("Segoe UI Symbol"), FrameworkPropertyMetadataOptions.AffectsMeasure);
			dProp.Register("ContentFontSize", ref ContentFontSizeProperty, 20, FrameworkPropertyMetadataOptions.AffectsMeasure);
#if !SILVERLIGHT
			dProp.Register("LabelTemplateSelector", ref LabelTemplateSelectorProperty, (DataTemplateSelector)null);
#endif
		}
		#endregion
		internal bool Clickable { get; set; }
		[TypeConverter(typeof(StringConverter))]
		public object Label {
			get { return GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}
		public DataTemplate LabelTemplate {
			get { return (DataTemplate)GetValue(LabelTemplateProperty); }
			set { SetValue(LabelTemplateProperty, value); }
		}
		public bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { SetValue(IsCompactProperty, value); }
		}
		public FontFamily ContentFontFamily {
			get { return (FontFamily)GetValue(ContentFontFamilyProperty); }
			set { SetValue(ContentFontFamilyProperty, value); }
		}
		public int ContentFontSize {
			get { return (int)GetValue(ContentFontSizeProperty); }
			set { SetValue(ContentFontSizeProperty, value); }
		}
#if !SILVERLIGHT
		public DataTemplateSelector LabelTemplateSelector {
			get { return (DataTemplateSelector)GetValue(LabelTemplateSelectorProperty); }
			set { SetValue(LabelTemplateSelectorProperty, value); }
		}
#endif
		public AppBarButton() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(AppBarButton);
#endif
			Clickable = true;
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			AppBarContentPresenter presenter = LayoutHelper.FindParentObject<AppBarContentPresenter>(this);
			if(presenter != null) {
				presenter.SetBinding(HorizontalAlignmentProperty, new System.Windows.Data.Binding("HorizontalAlignment") { Source = this });
			}
		}
		UIElement PartContentGrid;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartContentGrid = GetTemplateChild("PART_ContentGrid") as UIElement;
			AppBarContentPresenter presenter = LayoutHelper.FindParentObject<AppBarContentPresenter>(this);
			if(presenter != null) {
				presenter.SetBinding(HorizontalAlignmentProperty, new System.Windows.Data.Binding("HorizontalAlignment") { Source = this });
			}
			UpdateCompactState();
		}
		internal Size NormalSize {
			get { return DesiredSize; }
		}
		internal Size CompactSize {
			get { return DesiredSize; }
		}
		protected virtual void OnIsCompactChanged(bool newValue) {
			Dispatcher.BeginInvoke(new Action(() =>
			{
				UpdateCompactState();
			}));
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			if(!Clickable) {
				e.Handled = true;
				return;
			}
			base.OnMouseLeftButtonDown(e);
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			if(!Clickable) {
				e.Handled = true;
				return;
			}
			base.OnMouseLeftButtonUp(e);
		}
		protected virtual void UpdateCompactState() {
			VisualStateManager.GoToState(this, IsCompact ? "Compact" : "FullSize", false);
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new AppBarButtonAutomationPeer(this);
		}
	}
}
namespace DevExpress.Xpf.WindowsUI {
	public class AppBarBackButton : AppBarButton {
		static AppBarBackButton() {
			var dProp = new DependencyPropertyRegistrator<AppBarBackButton>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		public AppBarBackButton() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(AppBarBackButton);
#endif
		}
		protected override void OnClick() {
			base.OnClick();
			AppBar bar = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<AppBar>(this);
			if(bar == null) return;
			if(bar.BackCommand != null) {
				bar.BackCommand.Execute(null);
			}
			else {
				if(NavigationHelper.CanGoBack(this))
					NavigationHelper.GoBack(this);
			}
		}
	}
	public class AppBarExitButton : AppBarButton {
		static AppBarExitButton() {
			var dProp = new DependencyPropertyRegistrator<AppBarExitButton>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		public AppBarExitButton() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(AppBarExitButton);
#endif
		}
		protected override void OnClick() {
			base.OnClick();
			AppBar bar = LayoutHelper.FindParentObject<AppBar>(this);
			if(bar == null) return;
			if(bar.ExitCommand != null) {
				if(bar.ExitCommand.CanExecute(null))
					bar.ExitCommand.Execute(null);
			}
			else {
				Window window = LayoutHelper.FindRoot(this) as Window;
				if(window != null) {
					window.Close();
				}
			}
		}
	}
}

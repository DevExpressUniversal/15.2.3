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
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using System.Windows.Interop;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
using DevExpress.Data;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public class FloatingBarPopup : Popup {
		#region static 
		public static readonly DependencyProperty BarProperty;
		static readonly DependencyPropertyKey OwnerPopupPropertyKey;
		public static readonly DependencyProperty OwnerPopupProperty;
		static FloatingBarPopup() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingBarPopup), new FrameworkPropertyMetadata(typeof(FloatingBarPopup)));
			IsOpenProperty.OverrideMetadata(typeof(FloatingBarPopup), new FrameworkPropertyMetadata(false, null, new CoerceValueCallback(CoerceIsOpen)));
			BarProperty = DependencyPropertyManager.Register("Bar", typeof(Bar), typeof(FloatingBarPopup), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnBarPropertyChanged)));
			OwnerPopupPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("OwnerPopup", typeof(FloatingBarPopup), typeof(FloatingBarPopup), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));
			OwnerPopupProperty = OwnerPopupPropertyKey.DependencyProperty;
		}
		public static FloatingBarPopup GetOwnerPopup(DependencyObject obj) { return (FloatingBarPopup)obj.GetValue(OwnerPopupProperty); }
		internal static void SetOwnerPopup(DependencyObject obj, FloatingBarPopup value) { obj.SetValue(OwnerPopupPropertyKey, value); }
		protected static void OnBarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((FloatingBarPopup)obj).OnBarChanged(e);
		}
		public static object CoerceIsOpen(DependencyObject obj, object value) {
			return ((FloatingBarPopup)obj).CoerceIsOpen(value);
		}
		#endregion
		public FloatingBarPopup() {
		}
		public Bar Bar {
			get { return (Bar)GetValue(BarProperty); }
			set { SetValue(BarProperty, value); }
		}
		bool showHeader = true;
		public bool ShowHeader {
			get { return showHeader; }
			set {
				if(showHeader == value)
					return;
				showHeader = value;
				if(Child is FloatingBarPopupContentControl)
					((FloatingBarPopupContentControl)Child).UpdateHeaderState();
			}
		}
		bool showCloseButton = true;
		public bool ShowCloseButton {
			get { return showCloseButton; }
			set {
				if(showCloseButton == value) return;
				showCloseButton = value;
				if(Child is FloatingBarPopupContentControl)
					((FloatingBarPopupContentControl)Child).UpdateCloseButtonState();
			}
		}
		protected internal bool ShouldStartDrag {
			get;
			set;
		}
		protected virtual void OnBarChanged(DependencyPropertyChangedEventArgs e) {
			UnsubscribeBarEvents((Bar)e.OldValue);
			SubscribeBarEvents(Bar);					 
			UpdateContent();
		}
		protected virtual void UnsubscribeBarEvents(Bar bar) {
			if (bar == null)
				return;
			bar.VisibleChanged -= OnBarVisibleChanged;
		}
		protected virtual void SubscribeBarEvents(Bar bar) {
			if (bar == null)
				return;
			UnsubscribeBarEvents(bar);
			bar.VisibleChanged += OnBarVisibleChanged;
		}
		protected virtual void OnBarVisibleChanged(object sender, BarVisibileChangedEventArgs e) {
			IsOpen = Bar.Visible;
		}
		protected internal virtual void UpdateContent() {
			if(BlendHelper.IsInBlend) return;
			if(Bar == null) {
				FloatingBarPopupContentControl oldContainer = Child as FloatingBarPopupContentControl;
				if(oldContainer != null) {
					oldContainer.CancelDrag();
					FloatingBarPopup.SetOwnerPopup(oldContainer, null);
					oldContainer.DataContext = null;
				}
				Child = null;
				IsOpen = false;
				return;
			}
			FloatingBarPopupContentControl container = new FloatingBarPopupContentControl();
			FloatingBarPopup.SetOwnerPopup(container, this);
			container.Bar = Bar;
			Child = container;
		}
		protected override void OnOpened(EventArgs e) {
			base.OnOpened(e);			
		}
		protected virtual object CoerceIsOpen(object value) {
			if (Bar != null && !Bar.Visible)
				return false;
			if (Child != null)
				Child.Opacity = 0d;
			Dispatcher.BeginInvoke(new Action(() => { if (Child != null)Child.Opacity = 1d; }));
			return value;
		}		
	}
	public class FloatingBarPopupContentControl : ContentControl {
		#region static
		public static readonly DependencyProperty DragWidgetStyleProperty;
		public static readonly DependencyProperty SizeGripStyleProperty;
		public static readonly DependencyProperty CloseButtonStyleProperty;
		public static readonly DependencyProperty CaptionStyleProperty;
		public static readonly DependencyProperty QuickCustomizationButtonStyleProperty;
		public static readonly DependencyProperty BarProperty;						
		static FloatingBarPopupContentControl() {
			BarProperty = DependencyPropertyManager.Register("Bar", typeof(Bar), typeof(FloatingBarPopupContentControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((FloatingBarPopupContentControl)d).OnBarChanged((Bar)e.OldValue))));
			DragWidgetStyleProperty = DependencyPropertyManager.Register("DragWidgetStyle", typeof(Style), typeof(FloatingBarPopupContentControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			SizeGripStyleProperty = DependencyPropertyManager.Register("SizeGripStyle", typeof(Style), typeof(FloatingBarPopupContentControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			CloseButtonStyleProperty = DependencyPropertyManager.Register("CloseButtonStyle", typeof(Style), typeof(FloatingBarPopupContentControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			QuickCustomizationButtonStyleProperty = DependencyPropertyManager.Register("QuickCustomizationButtonStyle", typeof(Style), typeof(FloatingBarPopupContentControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			CaptionStyleProperty = DependencyPropertyManager.Register("CaptionStyle", typeof(Style), typeof(FloatingBarPopupContentControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		#endregion
		public FloatingBarPopupContentControl() {
			DefaultStyleKey = typeof(FloatingBarPopupContentControl);
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}		
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateBorder();
			UpdateHeaderState();
			UpdateCloseButtonState();
			SubscribeEvents();
			if (Popup != null && Popup.Bar != null)
				Popup.Width = Popup.Bar.DockInfo.FloatBarWidth;
		}
		private void UpdateBorder() {
			if(Border != null) {
				VisualStateManager.GoToState(Border, BrowserInteropHelper.IsBrowserHosted ? "BrowserHosted" : "Normal", false);
			}
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeEvents();
		}
		protected internal FloatingBarPopup Popup { get { return FloatingBarPopup.GetOwnerPopup(this); } }
		protected internal BarContainerControl Container { get; set; }
		protected internal Thumb SizeGrip { get; set; }
		protected internal Button CloseButton { get; set; }
		protected internal Control Border { get; set; }
		protected internal Control Caption { get; set; }
		public Bar Bar {
			get { return (Bar)GetValue(BarProperty); }
			set { SetValue(BarProperty, value); }
		}
		DragWidget dragWidget;
		protected internal DragWidget DragWidget {
			get { return dragWidget;}
			set {
				if(dragWidget == value) return;
				OnDragWidgetChanging();
				dragWidget = value;
				OnDragWidgetChanged();
			}
		}
		protected internal Button QuickCustomizationButton { get; set; }
		public Style DragWidgetStyle {
			get { return (Style)GetValue(DragWidgetStyleProperty); }
			set { SetValue(DragWidgetStyleProperty, value); }
		}
		public Style SizeGripStyle {
			get { return (Style)GetValue(SizeGripStyleProperty); }
			set { SetValue(SizeGripStyleProperty, value); }
		}
		public Style CloseButtonStyle {
			get { return (Style)GetValue(CloseButtonStyleProperty); }
			set { SetValue(CloseButtonStyleProperty, value); }
		}
		public Style QuickCustomizationButtonStyle {
			get { return (Style)GetValue(QuickCustomizationButtonStyleProperty); }
			set { SetValue(QuickCustomizationButtonStyleProperty, value); }
		}
		public Style CaptionStyle {
			get { return (Style)GetValue(CaptionStyleProperty); }
			set { SetValue(CaptionStyleProperty, value); }
		}
		protected virtual void SubscribeEvents() {
			UnsubscribeEvents();
			if(CloseButton != null) CloseButton.Click += OnCloseButtonClick;
			if(SizeGrip != null) {
				SizeGrip.DragDelta += OnSizeGrip;
			}
			if(DragWidget != null) {
				DragWidget.DragDelta += OnDragWidgetDelta;
				DragWidget.MouseDoubleClick += OnDragWidgetDoubleClick;
				CheckStartDrag();
			}
			if(QuickCustomizationButton != null) QuickCustomizationButton.Click += OnQuickCustomizationButtonClick;
		}
		protected virtual void UnsubscribeEvents() {
			if(CloseButton != null) CloseButton.Click -= OnCloseButtonClick;
			if(SizeGrip != null) SizeGrip.DragDelta -= OnSizeGrip;
			if(DragWidget != null) {
				DragWidget.DragDelta -= OnDragWidgetDelta;
				DragWidget.MouseDoubleClick -= OnDragWidgetDoubleClick;
			}
			if(QuickCustomizationButton != null) QuickCustomizationButton.Click -= OnQuickCustomizationButtonClick;
		}
		protected virtual void OnDragWidgetChanging() {
			if(DragWidget != null) {
				DragWidget.PreviewMouseDown -= OnPreviewDragWidgetMouseDown;
			}
		}
		protected virtual void OnDragWidgetChanged() {
			if(DragWidget != null) {
				DragWidget.PreviewMouseDown += OnPreviewDragWidgetMouseDown;
			}
		}
		protected virtual void OnDragWidgetDoubleClick(object sender, MouseButtonEventArgs e) {
			if (Bar.DockInfo.Data.Restore()) {
				Popup.Do(x => x.Bar = null);
				return;
			}				
			Bar.Visible = false;			
		}
		delegate void DragDelegate();
		protected void CheckStartDrag() {
			if(Popup != null && Popup.ShouldStartDrag) {
				Dispatcher.BeginInvoke(new DragDelegate(DragWidget.StartDrag), DispatcherPriority.Normal, new object[] { });
				Popup.ShouldStartDrag = false;
			}
		}
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonDown(e);
			if(Popup != null)
				RefreshPopup();
		}
		bool popupRefreshed = false;
		void RefreshPopup() {
			if(popupRefreshed || Bar == null || Popup==null || !Popup.IsOpen)
				return;
			popupRefreshed = true;
			Popup.IsOpen = false;
			Popup.IsOpen = true;
		}
		protected virtual void OnQuickCustomizationButtonClick(object sender, RoutedEventArgs e) {
			BarNameScope.GetService<ICustomizationService>(this).ShowCustomizationForm();
		}
		protected virtual void OnDragWidgetDelta(object sender, DragDeltaEventArgs e) {
			if(Popup == null) return;
			Point dragOffset = DragWidget.DragOffset;
			Popup.HorizontalOffset = dragOffset.X + (SystemParameters.MenuDropAlignment ? ActualWidth : 0);
			Popup.VerticalOffset = dragOffset.Y;
			if(Popup.Bar != null)
				Popup.Bar.DockInfo.FloatBarOffset = new Point(Popup.HorizontalOffset, Popup.VerticalOffset);
			var containers = BarNameScope
				.GetService<IElementRegistratorService>(Popup.Bar)
				.GetElements<IFrameworkInputElement>(ScopeSearchSettings.Local)
				.OfType<BarContainerControl>()
				.Where(x => !(x is FloatingBarContainerControl));
			var layoutCalculators = containers.Select(x => x.With(c => c.ClientPanel).With(c => c.LayoutCalculator)).Where(x => x != null);
			var container = layoutCalculators.FirstOrDefault(x => x.CheckBarDocking(Popup));
			if (container != null || Mouse.LeftButton == MouseButtonState.Released)
				CancelDrag();
			if (container != null) {
				var bar = Bar;
				Popup.Bar = null;
				container.InsertFloatBar(bar, true);				
			}			
			RefreshPopup();
		}
		protected internal virtual void OnSizeGrip(object sender, DragDeltaEventArgs e) {
			if(Popup == null) return;
			var dockInfo = Popup.Bar.DockInfo;
			if(double.IsNaN(dockInfo.FloatBarWidth)) {
				dockInfo.FloatBarWidth = Popup.Child is UIElement ? ((UIElement)Popup.Child).RenderSize.Width : double.NaN;
			} else {
				double horzChange = e.HorizontalChange;
				if(horzChange > 0)
					horzChange = Math.Max(0, Math.Min(e.HorizontalChange, dockInfo.BarControl.ItemsPresenter.MaxWidth - dockInfo.BarControl.ItemsPresenter.ActualWidth));
				else
					horzChange = - Math.Max(0, Math.Min(-e.HorizontalChange, dockInfo.BarControl.ItemsPresenter.ActualWidth - dockInfo.BarControl.ItemsPresenter.MinWidth));
				if (SystemParameters.MenuDropAlignment) {
					if (CloseButton != null) {
						if (dockInfo.FloatBarWidth + horzChange < CloseButton.DesiredSize.Width * 2) {
							horzChange = CloseButton.DesiredSize.Width * 2 - dockInfo.FloatBarWidth;
						}
					}
					var offset = dockInfo.FloatBarOffset;
					dockInfo.FloatBarOffset = new Point(offset.X + horzChange, offset.Y);
				}
				dockInfo.FloatBarWidth += horzChange;
			}			
		}
		protected internal virtual void UpdateHeaderState() {
			if(Caption == null || Popup == null)
				return;
			Caption.Visibility = Popup.ShowHeader ? Visibility.Visible : Visibility.Collapsed;
		}
		protected internal virtual void UpdateCloseButtonState() {
			if(CloseButton == null || Popup == null)
				return;
			CloseButton.Visibility = Popup.ShowCloseButton ? Visibility.Visible : Visibility.Collapsed;
		}
		protected virtual void OnCloseButtonClick(object sender, RoutedEventArgs e) {
			if(Popup == null) return;
			Popup.Bar.Visible = false;
		}
		protected virtual void OnPreviewDragWidgetMouseDown(object sender, MouseButtonEventArgs e) {
			PopupMenuManager.CloseAllPopups();
		}
		public BarManager Manager { get { return Bar != null && Bar.Manager != null ? Bar.Manager : BarManagerHelper.GetOrFindBarManager(this); } }
		public override void OnApplyTemplate() {
			int oldContainerIndex = -1;
			if(Container != null) {
				FloatingBarPopup.SetOwnerPopup(Container, null);				
				if(Manager != null) oldContainerIndex = Manager.Containers.IndexOf(Container);
			}
			base.OnApplyTemplate();
			UnsubscribeEvents();
			Border = (Control)GetTemplateChild("PART_Border");
			Unlink(Container, Bar);
			Container = (BarContainerControl)GetTemplateChild(BarDockInfo.FloatingContainerName);
			SizeGrip = (Thumb)GetTemplateChild("PART_SizeGrip");
			CloseButton = (Button)GetTemplateChild("PART_CloseButton");
			Caption = (Control)GetTemplateChild("PART_Caption");
			DragWidget.Do(x => x.TopElement = null);
			DragWidget = (DragWidget)GetTemplateChild("PART_DragWidget");
			UpdateDragWidgetTopElement();
			QuickCustomizationButton = (Button)GetTemplateChild("PART_QuickCustomizationButton");
			Link(Container, Bar);
			SubscribeEvents();
			UpdateHeaderState();
			UpdateCloseButtonState();
			if(Border!=null)
				Border.Loaded += new RoutedEventHandler(OnBorderLoadedAfterApplyTemplate);			
		}
		protected virtual void UpdateDragWidgetTopElement() {
			if (DragWidget == null)
				return;
			DragWidget.TopElement = Bar.With(BarNameScope.FindScope).With(x => x.Target) as FrameworkElement;
		}
		protected virtual void OnBarChanged(Bar oldValue) {
			Unlink(Container, oldValue);
			DataContext = Bar;
			Link(Container, Bar);
			UpdateDragWidgetTopElement();
		}
		protected virtual void Link(BarContainerControl container, Bar bar) {
			if (container == null || bar == null)
				return;
			container.Link(bar);
		}
		protected virtual void Unlink(BarContainerControl container, Bar bar) {
			if (container == null || bar == null)
				return;
			container.Unlink(bar);
		}
		void OnBorderLoadedAfterApplyTemplate(object sender, RoutedEventArgs e) {
			Border.Loaded -= OnBorderLoadedAfterApplyTemplate;
			UpdateBorder();
		}
		protected internal virtual void OnManagerChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected internal void CancelDrag() {
			if (DragWidget == null)
				return;
			DragWidget.CancelDrag();
		}
	}
	[DXToolboxBrowsableAttribute(false)]
	public class FloatingBarContainerControl : BarContainerControl {
		public FloatingBarContainerControl() {
			this.SetValue(ContainerTypeProperty, BarContainerType.Floating);
			this.SetValue(IsFloatingPropertyKey, true);
		}
		protected internal override bool CanBind { get { return false; } }		
	}
}
namespace DevExpress.Xpf.Bars.Native {
	public static class FloatingBarPopupHelper {
		public static FloatingBarPopup GetPopup(BarContainerControl control) {
			return control.OwnerPopup;
		}
	}	
}

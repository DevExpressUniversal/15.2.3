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
using System.Windows.Input;
using System;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using System.Linq;
using System.Windows.Controls;
namespace DevExpress.Xpf.Ribbon {
	internal class RibbonWindowHelper {
		public Button MinimizeWindowButton {
			get { return minimizeWindowButton; }
			set {
				if(value == minimizeWindowButton)
					return;
				var oldValue = minimizeWindowButton;
				minimizeWindowButton = value;
				OnMinimizeButtonChanged(oldValue);
			}
		}
		public Button RestoreWindowButton {
			get { return restoreWindowButton; }
			set {
				if(value == restoreWindowButton)
					return;
				var oldValue = restoreWindowButton;
				restoreWindowButton = value;
				OnRestoreButtonChanged(oldValue);
			}
		}
		public Button CloseWindowButton {
			get { return closeWindowButton; }
			set {
				if(value == closeWindowButton)
					return;
				var oldValue = closeWindowButton;
				closeWindowButton = value;
				OnCloseButtonChanged(oldValue);
			}
		}
		Button closeWindowButton;
		Button restoreWindowButton;
		Button minimizeWindowButton;
		public RibbonWindowHelper(RibbonControl ribbon) {
			Ribbon = ribbon;
		}
		public virtual bool IsAeroMode {
			get {
				if(RibbonWindowContainer != null)
					return RibbonWindowContainer.IsAeroMode;
				if(RibbonWindow != null)
					return RibbonWindow.IsAeroMode;
				return false;
			}
		}
		public virtual bool HasRibbonWindowAsParent {
			get { return (RibbonWindow != null || RibbonWindowContainer != null) && !IsInMDIChildHost(); }
		}
		RibbonControl Ribbon { get; set; }
		DXRibbonWindow eventsTargetWindow;
		bool ShouldSearchRibbonWindow {
			get {
				return !BarNameScope
				  .GetService<IElementRegistratorService>(Ribbon)
				  .GetElements<IFrameworkInputElement>(Bars.Native.ScopeSearchSettings.Ancestors)
				  .OfType<RibbonControl>()
				  .Any();
			}
		}
		Window window;
		Window Window {
			get {
				if (window == null)
					 window = Window.GetWindow(Ribbon);
				return window;
			}
		}
		internal DXRibbonWindow RibbonWindow {
			get {
				if(eventsTargetWindow != null)
					return eventsTargetWindow;
				if(!ShouldSearchRibbonWindow)
					return null;
				DXRibbonWindow window = Window as DXRibbonWindow;
				if(window != null && window.Ribbon != null && window.Ribbon != Ribbon)
					return null;
				eventsTargetWindow = window;
				return eventsTargetWindow;
			}
		}
		DXRibbonWindowContainer RibbonWindowContainer {
			get { return ShouldSearchRibbonWindow ? LayoutHelper.FindParentObject<DXRibbonWindowContainer>(Ribbon) : null; }
		}
		public virtual void SubscribeRibbonWindowEvents() {
			if (Window == null || !Ribbon.IsVisible)
				return;			
			Window.Activated += new EventHandler(OnWindowActivated);
			Window.Deactivated += new EventHandler(OnWindowDeactivated);
			if(RibbonWindow == null)
				return;			
			RibbonWindow.Ribbon = Ribbon;
			InitializeRibbonWindow();
			RibbonWindow.IsAeroModeChanged += new EventHandler(OnRibbonWindowIsAeroModeChanged);			
		}
		public virtual void UnsubscribeRibbonWindowEvents() {
			 if(Window == null)
				return;
			Window.Activated -= OnWindowActivated;
			Window.Deactivated -= OnWindowDeactivated;
			window = null;
			if(RibbonWindow == null)
				return;
			RibbonWindow.Ribbon = null;
			RibbonWindow.IsAeroModeChanged -= OnRibbonWindowIsAeroModeChanged;
			eventsTargetWindow = null;
		}
		void OnWindowDeactivated(object sender, EventArgs e) {
			OnActivateWindowStatusChanged();
		}
		void OnWindowActivated(object sender, EventArgs e) {
			OnActivateWindowStatusChanged();
		}
		void OnRibbonWindowIsAeroModeChanged(object sender, EventArgs e) {
			Ribbon.IsAeroMode = ((DXRibbonWindow)sender).IsAeroMode;
		}
		protected virtual void OnActivateWindowStatusChanged() {
			Ribbon.KeyboardNavigationManager.IsNavigationMode = false;
			if (Ribbon.HeaderBorder == null || !Ribbon.IsInRibbonWindow)
				return;
			if (RibbonWindow != null)
				FloatingContainer.SetIsActive(Ribbon.HeaderBorder, RibbonWindow.IsActive);
		}
		public virtual void InitializeRibbonWindow() {
			if(!ShouldSearchRibbonWindow || !Ribbon.IsInRibbonWindow) {
				return;
			}
			if(RibbonWindowContainer != null) {
				Ribbon.IsHeaderBorderVisible = true;
				BindingOperations.SetBinding(Ribbon, RibbonControl.ApplicationIconProperty, new Binding() { Source = RibbonWindowContainer, Path = new PropertyPath(DXRibbonWindowContainer.IconProperty) });
				return;
			}
			if(RibbonWindow != null) {
				Ribbon.IsHeaderBorderVisible = true;
				OnActivateWindowStatusChanged();
				BindingOperations.SetBinding(Ribbon, RibbonControl.ApplicationIconProperty, new Binding() { Source = RibbonWindow, Path = new PropertyPath(DXWindow.SmallIconProperty) });
			}
		}
		public virtual void UnInitializeRibbonWindow() {
			Ribbon.IsHeaderBorderVisible = false;
			Ribbon.ClearValue(RibbonControl.ApplicationIconProperty);
		}
		public virtual void DragOrMaximizeWindow(object sender, MouseButtonEventArgs e) {
			if(RibbonWindow == null)
				return;
			RibbonWindow.DragOrMaximizeWindow(sender, e);
		}
		public virtual UIElement GetRibbonWindowContent() {
			if(RibbonWindow == null)
				return null;
			return RibbonWindow.GetContentContainer();
		}
		public virtual Size GetRibbonWindowSize() {
			return RibbonWindow == null ? new Size() : RibbonWindow.RenderSize;
		}
		public virtual void UpdateRibbonVisibility() {
			if(RibbonWindow == null || !Ribbon.IsInRibbonWindow) return;
			if(Ribbon.MinWidth == 0d) {
				RibbonWindow.IsCaptionVisible = Ribbon.RibbonHeaderVisibility == RibbonHeaderVisibility.Collapsed;
				return;
			}
			if(RibbonWindowContainer != null)
				return;
			if(Ribbon.MergedParent != null)
				return;
			if(GetRibbonWindowContent().RenderSize.Width < Ribbon.MinWidth) {
				Ribbon.Visibility = Visibility.Collapsed;
				Ribbon.Toolbar.Control.Visibility = Visibility.Collapsed;
				RibbonWindow.IsRibbonCaptionVisible = false;
				RibbonWindow.IsCaptionVisible = true;
			} else {
				Ribbon.Visibility = Visibility.Visible;
				Ribbon.Toolbar.Control.Visibility = Visibility.Visible;
				var isHeaderVisible = Ribbon.RibbonHeaderVisibility == RibbonHeaderVisibility.Visible;
				RibbonWindow.IsCaptionVisible = !isHeaderVisible;
				RibbonWindow.IsRibbonCaptionVisible = isHeaderVisible;
			}
		}
		public virtual FrameworkElement GetTitleContainer() {
			if(RibbonWindowContainer != null)
				return RibbonWindowContainer.GetCaptionContainer();
			if(RibbonWindow == null)
				return null;
			return RibbonWindow.GetTitleContainer();
		}
		public virtual FrameworkElement GetControlBoxContainer() {
			if(RibbonWindowContainer != null)
				return RibbonWindowContainer.GetControlBoxContainer();
			if(RibbonWindow == null)
				return null;
			return RibbonWindow.GetControlBoxContainer();
		}
		public virtual void DragOrMaximizeWindow(MouseButtonEventArgs e) {
			if(RibbonWindow == null)
				return;
			RibbonWindow.DragOrMaximizeWindow(this, e);
		}
		public virtual void ShowSystemMenu(MouseButtonEventArgs e) {
			if(RibbonWindow == null) return;
			RibbonWindow.ShowSystemMenu(e);
		}
		public virtual void ShowSystemMenu(UIElement relativeObject, Point relativeOffset) {
			if(RibbonWindow == null)
				return;
			RibbonWindow.ShowSystemMenu(relativeObject, relativeOffset);
		}
		public virtual void SetAeroContentHorizontalOffset(double offset) {
			if(RibbonWindowContainer != null) {
				RibbonWindowContainer.SetAeroContentHorizontalOffset(offset);
				return;
			}
			if(RibbonWindow != null) {
				RibbonWindow.SetAeroContentHorizontalOffset(offset);
				return;
			}
		}
		public virtual void SetRibbonHeaderBorderHeight(double height) {
			if(Ribbon.MergedParent != null) return;
			if(RibbonWindowContainer != null) {
				RibbonWindowContainer.SetRibbonHeaderBorderHeight(height);
				return;
			}
			if(RibbonWindow != null) {
				RibbonWindow.SetRibbonHeaderBorderHeight(height);
				return;
			}
		}
		public virtual string GetTitle() {
			if(RibbonWindowContainer != null)
				return RibbonWindowContainer.Caption;
			if(RibbonWindow == null)
				return null;
			return RibbonWindow.Title;
		}
		public virtual void Close() {
			if(RibbonWindow != null)
				RibbonWindow.Close();
		}
		protected virtual void OnCloseButtonChanged(Button oldValue) {
			if(oldValue != null)
				oldValue.Click -= OnCloseButtonClick;
			if(CloseWindowButton != null)
				CloseWindowButton.Click += OnCloseButtonClick;
		}
		protected virtual void OnRestoreButtonChanged(Button oldValue) {
			if(oldValue != null)
				oldValue.Click -= OnRestoreButtonClick;
			if(RestoreWindowButton != null)
				RestoreWindowButton.Click += OnRestoreButtonClick;
		}
		protected virtual void OnMinimizeButtonChanged(Button oldValue) {
			if(oldValue != null)
				oldValue.Click -= OnMinimizeButtonClick;
			if(MinimizeWindowButton != null)
				MinimizeWindowButton.Click += OnMinimizeButtonClick;
		}
		void OnCloseButtonClick(object sender, RoutedEventArgs e) {
			Close();
		}
		void OnRestoreButtonClick(object sender, RoutedEventArgs e) {
			RibbonWindow.ForceRestore();
		}
		void OnMinimizeButtonClick(object sender, EventArgs e) {
			RibbonWindow.ForceMinimize();
		}
		bool IsInMDIChildHost() {
			return LayoutHelper.FindParentObject<Bars.IMDIChildHost>(Ribbon) != null;
		}
	}	
	public class NegativeDoubleToThicknessTopConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return new Thickness(0, -(double)value, 0, 0);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}

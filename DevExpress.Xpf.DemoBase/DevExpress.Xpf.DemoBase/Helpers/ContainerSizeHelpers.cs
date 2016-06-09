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
using System.Windows;
using System.Windows.Interop;
using DevExpress.Xpf.Core;
using System.Reflection;
#if DEMOCENTER
namespace DevExpress.DemoCenter.Xpf.Helpers {
#else
namespace DevExpress.Xpf.DemoBase.Helpers {
#endif
	class HostSizeBinder {
		static ContainerInterface container;
		public static readonly DependencyProperty BindToHostProperty;
		public static readonly DependencyProperty PreviousSizeProperty;
		public static readonly DependencyProperty AllowForceBindProperty;
		public static readonly DependencyProperty HeightGlutProperty;
		public static readonly DependencyProperty WidthGlutProperty;
		static readonly DependencyProperty BindedBeforeProperty;
		static HostSizeBinder() {
			Type ownerType = typeof(HostSizeBinder);
			container = new ContainerInterface();
			BindToHostProperty = DependencyProperty.RegisterAttached("BindToHost", typeof(bool), ownerType, new PropertyMetadata(false, OnBindToHostChanged));
			PreviousSizeProperty = DependencyProperty.RegisterAttached("PreviousSize", typeof(Size), ownerType, new PropertyMetadata(null));
			AllowForceBindProperty = DependencyProperty.RegisterAttached("AllowForceBind", typeof(bool), ownerType, new PropertyMetadata(true));
			HeightGlutProperty = DependencyProperty.RegisterAttached("HeightGlut", typeof(double), ownerType, new PropertyMetadata(0.0));
			WidthGlutProperty = DependencyProperty.RegisterAttached("WidthGlut", typeof(double), ownerType, new PropertyMetadata(0.0));
			BindedBeforeProperty = DependencyProperty.RegisterAttached("BindedBefore", typeof(bool), ownerType, new PropertyMetadata(false));
		}
		protected static ContainerInterface Container { get { return container; } private set { container = value; } }
		public static void SetBindToHost(DependencyObject element, bool value) { element.SetValue(BindToHostProperty, value); }
		public static bool GetBindToHost(DependencyObject element) { return (bool)element.GetValue(BindToHostProperty); }
		static void SetPreviousSize(DependencyObject element, Size value) { element.SetValue(PreviousSizeProperty, value); }
		static Size GetPreviousSize(DependencyObject element) { return (Size)element.GetValue(PreviousSizeProperty); }
		static void SetHeightGlut(DependencyObject element, double value) { element.SetValue(HeightGlutProperty, value); }
		static double GetHeightGlut(DependencyObject element) { return (double)element.GetValue(HeightGlutProperty); }
		static void SetWidthGlut(DependencyObject element, double value) { element.SetValue(WidthGlutProperty, value); }
		static double GetWidthGlut(DependencyObject element) { return (double)element.GetValue(WidthGlutProperty); }
		public static void SetAllowForceBind(DependencyObject element, bool value) { element.SetValue(AllowForceBindProperty, value); }
		public static bool GetAllowForceBind(DependencyObject element) { return (bool)element.GetValue(AllowForceBindProperty); }
		static void SetBindedBefore(DependencyObject element, bool value) { element.SetValue(BindedBeforeProperty, value); }
		static bool GetBindedBefore(DependencyObject element) { return (bool)element.GetValue(BindedBeforeProperty); }
		static void OnBindToHostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if((bool)e.NewValue && d is FrameworkElement && !GetBindedBefore(d)) {
				Reset(d);
				if(Container == null) return;
				Container.AddSizeChangedEventHandler((s, a) => OnMainWindowSizeChanged(d as FrameworkElement, ((SizeChangedEventArgs)a).NewSize));
				if(GetAllowForceBind(d)) {
					OnMainWindowSizeChanged(d as FrameworkElement, new Size(Container.ActualWidth, Container.ActualHeight));
				}
			}
		}
		public static object RetrieveRootElement() {
			return RootElementHelper.Container;
		}
		static void OnMainWindowSizeChanged(FrameworkElement d, Size newSize) {
			if(!GetBindToHost(d)) return;
			Size oldSize = GetPreviousSize(d);
			if(!BrowserInteropHelper.IsBrowserHosted) {
				if(newSize.Height > RootElementHelper.WindowHeightAdjustment) {
					newSize.Height -= RootElementHelper.WindowHeightAdjustment;
				}
			}
			double heightGlut = GetHeightGlut(d);
			double widthGlut = GetWidthGlut(d);
			d.Height = CorrectDimension(CalculateDimension(d.ActualHeight, newSize.Height, oldSize.Height, heightGlut), d.MinHeight, d.MaxHeight, out heightGlut);
			d.Width = CorrectDimension(CalculateDimension(d.ActualWidth, newSize.Width, oldSize.Width, widthGlut), d.MinWidth, d.MaxWidth, out widthGlut);
			oldSize.Height = newSize.Height;
			oldSize.Width = newSize.Width;
			SetPreviousSize(d, oldSize);
			SetHeightGlut(d, heightGlut);
			SetWidthGlut(d, widthGlut);
			d.HorizontalAlignment = HorizontalAlignment.Left;
			d.VerticalAlignment = VerticalAlignment.Top;
		}
		static double CalculateDimension(double actualValue, double actualHostDimesionValue, double prevHostDimensionValue, double glut) {
			return actualValue + actualHostDimesionValue - prevHostDimensionValue + glut;
		}
		static double CorrectDimension(double value, double minValue, double maxValue, out double glut) {
			glut = 0.0;
			if(!double.IsNegativeInfinity(minValue) && !double.IsNaN(minValue) && value < minValue) {
				glut = value - minValue;
				return minValue;
			}
			if(!double.IsPositiveInfinity(minValue) && !double.IsNaN(maxValue) && value > maxValue) {
				glut = value - maxValue;
				return maxValue;
			}
			return value;
		}
		static void Reset(DependencyObject d) {
			SetBindedBefore(d, true);
			SetPreviousSize(d, new Size(0.0, 0.0));
			SetWidthGlut(d, 0.0);
			SetHeightGlut(d, 0.0);
		}
	}
	static class RootElementHelper {
		static object contentElement;
		static UIElement rootVisual;
		static Application CurrentApplication { get { return Application.Current; } }
		public static double WindowHeightAdjustment { get { return Container != null && ((Window)Container).WindowState == WindowState.Maximized ? 43.0 : 37.0; } }
		public static object Container {
			get {
				if(contentElement == null) {
					contentElement = RetrieveContainer();
				}
				return contentElement;
			}
		}
		public static T GetContainer<T>() where T : class {
			return Container as T;
		}
		public static UIElement RootVisual {
			get {
				if(rootVisual == null) {
					rootVisual = RetrieveRootVisual();
				}
				return rootVisual;
			}
		}
		public static object RetrieveContainer() {
			if(CurrentApplication == null) return null;
			return CurrentApplication.Windows.Count > 0 ? CurrentApplication.Windows[CurrentApplication.Windows.Count - 1] : null;
		}
		public static UIElement RetrieveRootVisual() {
			return Container as UIElement;
		}
	}
	class ContainerInterface {
		object container;
		public ContainerInterface() {
			container = RootElementHelper.Container;
		}
		public double ActualHeight {
			get {
				return container == null ? 0.0 : TryGetWindowSize().Height;
			}
		}
		public double ActualWidth {
			get {
				return container == null ? 0.0 : TryGetWindowSize().Width;
			}
		}
		Size TryGetWindowSize() {
			Window window = container as Window;
			if(window == null) return Size.Empty;
			Size result;
			if(window is DXWindow) {
				PropertyInfo windowRectInfo = (typeof(DXWindow).GetProperty("WindowRect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
				Rect windowRect = (Rect)windowRectInfo.GetValue(window, null);
				result = new Size(windowRect.Width, windowRect.Height - RootElementHelper.WindowHeightAdjustment);
			} else {
				result = new Size(window.ActualWidth, window.ActualHeight);
			}
			return result;
		}
		public void AddSizeChangedEventHandler(SizeChangedEventHandler handler) {
			if(container == null) return;
			((FrameworkElement)container).SizeChanged += handler;
		}
		public void RemoveSizeChangedEventHandler(SizeChangedEventHandler handler) {
			if(container == null) return;
			((FrameworkElement)container).SizeChanged -= handler;
		}
	}
}

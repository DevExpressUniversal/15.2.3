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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using global::System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DevExpress.XtraPrinting.Native;
using Microsoft.Win32;
namespace DevExpress.Xpf.Core.Design {
	public partial class SmartTagAdornerButton : UserControl {
		static double popupWidth = double.NaN;
		public static readonly DependencyProperty IsPressedProperty =
			DependencyProperty.Register("IsPressed", typeof(bool), typeof(SmartTagAdornerButton), new PropertyMetadata(false, (d,e)=>((SmartTagAdornerButton)d).OnIsPressedChanged((bool)e.OldValue, (bool)e.NewValue)));
		public bool IsPressed {
			get { return (bool)GetValue(IsPressedProperty); }
			set { SetValue(IsPressedProperty, value); }
		}
		public Popup Popup { get { return popup; } }
		public event EventHandler IsPressedChanged;
		PlacementMode placementMode;
		public SmartTagAdornerButton() {
			InitializeComponent();
			placementMode = !SystemParameters.MenuDropAlignment ? PlacementMode.Right : PlacementMode.Left;
#if !SL
			ThemeManager.SetThemeName(popupGrid, DevExpress.Xpf.Core.Theme.MetropolisLightName);
#endif
			popupGrid.Effect = new DropShadowEffect() { Color = Colors.Black, ShadowDepth = 2, Direction = 270, BlurRadius = 6, Opacity = 0.15 };
			popupGrid.IsVisibleChanged += SmartTagAdornerButton_IsVisibleChanged;
			Loaded += OnLoaded;
		}
		HwndSource source;
		void OnLoaded(object sender, RoutedEventArgs e) {
			source = HwndSource.FromVisual(this) as HwndSource;
			source.AddHook(CustomHwndSourceHook);
			popupGrid.Width = GetPopupWidth();
		}
		double GetPopupWidth() {
			if (popupWidth.Equals(double.NaN)) {
				popupWidth = SmartTagSettingsHelper.GetPopupWidth();
			}
			return popupWidth;
		}
		void SavePopupWidth() {
			popupWidth = popupGrid.ActualWidth;
			SmartTagSettingsHelper.SavePopupWidth(popupWidth);
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			source.RemoveHook(CustomHwndSourceHook);
		}
		IntPtr CustomHwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if (msg == 8)
				HidePopup();
			return IntPtr.Zero;
		}
		private void HidePopup() {
			if (Popup != null)
				Popup.SetCurrentValue(Popup.IsOpenProperty, false);
		}
		void SmartTagAdornerButton_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			var child = sender as UIElement;
			var decorator = VisualTreeHelper.GetParent(child);
			if(decorator != null) {
				decorator = VisualTreeHelper.GetParent(decorator) as Decorator;
				if(decorator != null)
					decorator.ClearValue(FrameworkElement.LayoutTransformProperty);
			}
		}
		void OnIsPressedChanged(bool oldValue, bool newValue) {
			if (!newValue)
				SavePopupWidth();
			if(IsPressedChanged != null)
				IsPressedChanged(this, EventArgs.Empty);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			popup.Placement = PlacementMode.Custom;
			popup.Placement = placementMode;
		}
		void OnGridMouseDown(object sender, MouseButtonEventArgs e) {
			e.Handled = true;
		}
		void OnGridMouseUp(object sender, MouseButtonEventArgs e) {
			e.Handled = true;
		}
		private void OnDragDelta(object sender, DragDeltaEventArgs e) {
			double newWidth = popupGrid.ActualWidth + e.HorizontalChange;
			popupGrid.Width = Math.Max(newWidth, 0d);
		}
	}
	static class SmartTagSettingsHelper {
		const string popupWidthKeyName = "SmartTagWidth";
		const string popupWidthKeyPath = @"SOFTWARE\DevExpress\Components\";
		public static double GetPopupWidth() {
			double popupWidth = double.NaN;
			using (var key = GetRegistryKey()) {
				if (key != null) {
					object value = key.GetValue(popupWidthKeyName);
					if (value != null) {
						double.TryParse(value.ToString(), out popupWidth);
					}
				}
			}
			return popupWidth;
		}
		public static void SavePopupWidth(double popupWidth) {
			using (RegistryKey key = GetRegistryKey()) {
				key.SetValue(popupWidthKeyName, popupWidth);
			}
		}
		static RegistryKey GetRegistryKey() {
			RegistryKey key = null;
			try {
				key = Registry.CurrentUser.OpenSubKey(popupWidthKeyPath, true);
				if (key == null)
					key = Registry.CurrentUser.CreateSubKey(popupWidthKeyPath);
			} catch { }
			return key;
		}
	}
}

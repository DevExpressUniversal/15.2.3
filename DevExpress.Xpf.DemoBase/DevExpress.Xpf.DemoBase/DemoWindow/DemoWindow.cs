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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.DemoData.Helpers;
using DevExpress.DemoData.Utils;
using System.Drawing.Printing;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Mvvm.UI;
using System.Linq;
using System.Windows.Threading;
namespace DevExpress.DemoData.DemoParts {
	public class DemoWindow : DXWindow, IBackButton {
		internal static DemoWindow MainDemoWindow { get; private set; }
		PrimaryScreen screen = new PrimaryScreen();
		public event EventHandler CtrlFClicked;
		public DemoWindow() {
			sizingTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.1) };
			sizingTimer.Tick +=sizingTimer_Tick;
			MainDemoWindow = this;
			this.BorderEffect = BorderEffect.Default;
			SetUseLayoutRounding();
			var dictionary = new ResourceDictionary {
				Source = new Uri(string.Format("pack://application:,,,/DevExpress.Xpf.DemoBase.{0};component/DemoWindow/DemoWindow.xaml", AssemblyInfo.VSuffixWithoutSeparator), UriKind.Absolute)
			};
			Template = (ControlTemplate)dictionary["DXWindowTemplate"];
			WindowTemplate = (DataTemplate)dictionary["DemoWindowFloatingContainerTemplate"];
			ShowTitle = false;
			ShowIcon = false;
			ResizeMode = System.Windows.ResizeMode.CanResize;
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			ResizeBorderThickness = new Thickness(5);
			this.BorderEffectActiveColor = this.BorderEffectActiveColor;
			this.BorderEffectBottomMargins = this.BorderEffectBottomMargins;
			this.BorderEffectImagesUri = this.BorderEffectImagesUri;
			this.BorderEffectInactiveColor = this.BorderEffectInactiveColor;
			this.BorderEffectLeftMargins = this.BorderEffectLeftMargins;
			this.BorderEffectOffset = this.BorderEffectOffset;
			this.BorderEffectRightMargins = this.BorderEffectRightMargins;
			this.BorderEffectTopMargins = this.BorderEffectTopMargins;
			Loaded += DemoWindow_Loaded;
			screen.WorkingAreaChanged += UpdateSize;
			SizeChanged += DemoWindow_SizeChanged;
		}
		void sizingTimer_Tick(object sender, EventArgs e) {
			if(Win32.GetAsyncKeyState(System.Windows.Forms.Keys.LButton) == 0) {
				sizingTimer.Stop();
				isSizing = false;
				this.BorderEffect = Xpf.Core.BorderEffect.Default;
			}
		}
		bool isSizing = false;
		DispatcherTimer sizingTimer;
		void DemoWindow_SizeChanged(object sender, SizeChangedEventArgs e) {
			if(isSizing) return;
			isSizing = true;
			this.BorderEffect = Xpf.Core.BorderEffect.None;
			sizingTimer.Start();
		}
		void UpdateSize() {
			var point = this.PointToScreen(new Point());
			point.X += Width / 2;
			point.Y += Height / 2;
			var workingArea = screen.GetWorkingArea(point);
			MinWidth = Math.Min(workingArea.Width, MinWidth);
			MinHeight = Math.Min(workingArea.Height, MinHeight);
			Width = Math.Min(workingArea.Width, Width);
			Height = Math.Min(workingArea.Height, Height);
			Left = workingArea.Left + (workingArea.Width - Width) / 2;
			Top = workingArea.Top + (workingArea.Height - Height) / 2;
		}
		void DemoWindow_Loaded(object sender, RoutedEventArgs e) {
			UpdateSize();
			var command = new RoutedCommand();
			command.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));
			CommandBindings.Add(new CommandBinding(command, (s, _) => {
				if (CtrlFClicked != null)
					CtrlFClicked(this, EventArgs.Empty);
			}));
		}
		void SetUseLayoutRounding() {
			System.Reflection.PropertyInfo useLayoutRoundingInfo = typeof(DemoWindow).GetProperty("UseLayoutRounding", typeof(bool));
			if(useLayoutRoundingInfo != null) {
				useLayoutRoundingInfo.SetValue(this, true, null);
			}
		}
		bool IBackButton.DesiredVisibility { get { return false; } }
		event EventHandler IBackButton.DesiredVisibilityChanged {
			add { }
			remove { }
		}
		bool IBackButton.ActualVisibility {
			get {
				return false;
			}
			set {
			}
		}
		void IBackButton.DoClick() { }
		event EventHandler IBackButton.Click {
			add { }
			remove { }
		}
		internal void SetMainBackButton(ButtonExt backButton) {
			DemosIntercallHelper.RegisterMainBackButton(this);
		}
		class Win32 {
			[DllImport("user32.dll")]
			public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
		}
	}
	public abstract class DemoStartupBase : StartupBase {
		protected override bool DoStartup() {
			if(!base.DoStartup()) return false;
			MinWidth = 800.0;
			MinHeight = 550.0;
			MainElementMinHeight = 450.0;
			MainElementMinWidth = 560.0;
			Size size = new Size(1280.0, 860.0);
			size = GetSize(size, out this.maximizeWindow);
			MinWidth = Math.Min(MinWidth, size.Width);
			MinHeight = Math.Min(MinHeight, size.Height);
			Width = size.Width;
			Height = size.Height;
			return true;
		}
		bool maximizeWindow;
		protected override Window CreateMainWindow() {
			DemoWindow w = new DemoWindow();
			w.Loaded += OnWindowLoaded;
			return w;
		}
		void OnWindowLoaded(object sender, RoutedEventArgs e) {
			Window w = (Window)sender;
			if(this.maximizeWindow)
				w.WindowState = WindowState.Maximized;
		}
	}
}

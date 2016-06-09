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
using System.Windows.Data;
using System.ComponentModel;
using System;
using System.Reflection;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Runtime.CompilerServices;
using DevExpress.Xpf.Core.Internal;
using System.Linq;
namespace DevExpress.Xpf.Utils.Themes {
	public interface IThemeManager {
		void SetThemeName(DependencyObject d, string value);
		void ClearThemeName(DependencyObject d);
		string GetThemeName(DependencyObject d);
		BindingExpression GetThemeNameBindingExpression(DependencyObject d);
	}
	public class GlobalThemeHelper : DependencyObject {
		public static readonly DependencyProperty IsGlobalThemeNameProperty;
		static GlobalThemeHelper instance = new GlobalThemeHelper();
		string applicationThemeName;
		static GlobalThemeHelper() {
			IsGlobalThemeNameProperty = DependencyProperty.RegisterAttached("IsGlobalThemeName", typeof(bool), typeof(GlobalThemeHelper), new FrameworkPropertyMetadata(false));
		}
		protected GlobalThemeHelper() { }
		public string ApplicationThemeName {
			get { return applicationThemeName; }
			set { SetApplicationThemeName(value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static bool GetIsGlobalThemeName(DependencyObject d) {
			return (bool)d.GetValue(IsGlobalThemeNameProperty);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static void SetIsGlobalThemeName(DependencyObject d, bool value) {
			d.SetValue(IsGlobalThemeNameProperty, value);
		}
		public static GlobalThemeHelper Instance {
			get {
				if (instance == null)
					instance = new GlobalThemeHelper();
				return instance;
			}
		}
		IThemeManager ThemeManager { get { return DevExpress.Xpf.Core.ThemeManager.Instance; } }
		internal static bool IsAppInitialized {
			get { return Application.Current != null && (Application.Current.StartupUri != null || Application.Current.MainWindow != null); }
		}
		bool AppliedUserTheme(FrameworkElement window) {
			return !string.IsNullOrEmpty(ThemeManager.GetThemeName(window)) && !GetIsGlobalThemeName(window) || HasBinding(window);
		}
		bool HasBinding(FrameworkElement window) {
			BindingExpression expression = ThemeManager.GetThemeNameBindingExpression(window);
			if (expression == null)
				return false;
			return expression.Status != BindingStatus.Detached;
		}
		void SetApplicationThemeName(string value) {
			if (applicationThemeName == value)
				return;
			applicationThemeName = value;
			InitializeWindowTracker();
			SetWindowsApplicationThemeNameInThread();
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void InitializeWindowTracker() {
			WindowTracker.Initialize();
		}
		internal void SetWindowsApplicationThemeNameInThread() {
			if (Application.Current == null)
				return;
			if (!IsAppInitialized) {
				Application.Current.Dispatcher.BeginInvoke((Action)SetWindowsApplicationThemeName);
			}
			else {
				Application.Current.Dispatcher.Invoke((Action)SetWindowsApplicationThemeName);
			}
		}
		void SetWindowsApplicationThemeName() {
			lock (locker) {
				SetApplicationWindows();
				SetNonApplicationWindows();
			}
		}
		object locker = new object();
		void SetApplicationWindows() {
			if (Application.Current == null)
				return;
			foreach (Window window in Application.Current.Windows) {
				AssignApplicationThemeName(window);
			}
		}
		void SetNonApplicationWindows() {
			if (SecurityHelper.IsPartialTrust || Application.Current == null)
				return;
			foreach (Window window in (WindowCollection)ReflectorHelper.GetValue(Application.Current, "NonAppWindowsInternal", BindingFlags.NonPublic | BindingFlags.Instance)) {
				System.Threading.ThreadState windowThreadState = window.Dispatcher.Thread.ThreadState;
				if (!CheckThreadExecutable(windowThreadState))
					continue;
				if (CheckThreadInDirectAccess(windowThreadState))
					window.Dispatcher.Invoke((Action<Window>)AssignApplicationThemeName, window);
				else
					window.Dispatcher.BeginInvoke((Action<Window>)AssignApplicationThemeName, window);
			}
		}
		bool CheckThreadInDirectAccess(System.Threading.ThreadState state) {
			return (state & System.Threading.ThreadState.WaitSleepJoin) == 0;
		}
		bool CheckThreadExecutable(System.Threading.ThreadState state) {
			if ((state & System.Threading.ThreadState.Running) != 0 ||
				(state & System.Threading.ThreadState.Background) != 0)
				return true;
			return false;
		}
		public void AssignApplicationThemeName(FrameworkElement window) {
			if (!AppliedUserTheme(window)) {
				SetGlobalTheme(window);
			}
		}
		void SetGlobalTheme(FrameworkElement window) {
			if (string.IsNullOrEmpty(ApplicationThemeName)) {
				ThemeManager.ClearThemeName(window);
				SetIsGlobalThemeName(window, false);
			}
			else {
				ThemeManager.SetThemeName(window, ApplicationThemeName);
				SetIsGlobalThemeName(window, true);
			}
		}
	}
	static class WindowTracker {
		static WindowTracker() {
			Window.ContentProperty.AddPropertyChangedCallback(typeof(Window), (d, e) => { DoWork(d); });			
			EventManager.RegisterClassHandler(typeof(Window), Window.LoadedEvent, new RoutedEventHandler(OnWindowLoaded));
			EventManager.RegisterClassHandler(typeof(System.Windows.Documents.AdornerDecorator), FrameworkElement.LoadedEvent, new RoutedEventHandler(OnAdornerLoaded));
			foreach(var source in PresentationSource.CurrentSources.OfType<PresentationSource>().ToList()) {
				var rv = (source.RootVisual as Window);
				if (rv == null || !rv.Dispatcher.CheckAccess())
					continue;
				DoWork(rv);
			}
		}		
		static void DoWork(object sender) {
			Window window = (Window)sender;
			GlobalThemeHelper.Instance.AssignApplicationThemeName(window);
			Bars.BarNameScope.EnsureRegistrator(window);
		}
		static void OnWindowLoaded(object sender, RoutedEventArgs e) {
			DoWork(sender);
		}
		static void OnAdornerLoaded(object sender, RoutedEventArgs e) { Bars.BarNameScope.EnsureRegistrator(sender as DependencyObject); }
		internal static void Initialize() { }
	}
	public class ColorSchemeInfo {
		public string Name = string.Empty;
		public string TargetThemeName = string.Empty;
		public string TargetThemeDisplayName = string.Empty;
	}
}

﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#if !SILVERLIGHT
using WindowBase = System.Windows.Window;
using System.Windows.Input;
#else
using WindowBase = DevExpress.Xpf.Core.DXWindowBase;
using System.Reflection;
#endif
namespace DevExpress.Mvvm.UI {
	public enum WindowShowMode { Dialog, Default }
	public class WindowService : ViewServiceBase, IWindowService, IDocumentOwner {
#if !FREE
		static Type DefaultWindowType = typeof(DevExpress.Xpf.Core.DXWindow);
#else
#if !SILVERLIGHT
		static Type DefaultWindowType = typeof(Window);
#endif
#endif
#if !SILVERLIGHT
		const string WindowTypeException = "WindowType show be derived from the Window type";
#else
		const string WindowTypeException = "WindowType show be derived from the DXWindowBase type";
#endif
#if !SILVERLIGHT
		public static readonly DependencyProperty WindowStartupLocationProperty =
			DependencyProperty.Register("WindowStartupLocation", typeof(WindowStartupLocation), typeof(WindowService),
			new PropertyMetadata(WindowStartupLocation.CenterScreen));
		public static readonly DependencyProperty AllowSetWindowOwnerProperty =
			DependencyProperty.Register("AllowSetWindowOwner", typeof(bool), typeof(WindowService), new PropertyMetadata(true));
#endif
		public static readonly DependencyProperty WindowStyleProperty =
			DependencyProperty.Register("WindowStyle", typeof(Style), typeof(WindowService), new PropertyMetadata(null));
		public static readonly DependencyProperty WindowTypeProperty =
			DependencyProperty.Register("WindowType", typeof(Type), typeof(WindowService),
			new PropertyMetadata(DefaultWindowType, (d, e) => ((WindowService)d).OnWindowTypeChanged()));
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(WindowService),
			new PropertyMetadata(string.Empty, (d, e) => ((WindowService)d).OnTitleChanged()));
		public static readonly DependencyProperty WindowShowModeProperty =
			DependencyProperty.Register("WindowShowMode", typeof(WindowShowMode), typeof(WindowService),
			new PropertyMetadata(WindowShowMode.Default));
#if !SILVERLIGHT
		public WindowStartupLocation WindowStartupLocation {
			get { return (WindowStartupLocation)GetValue(WindowStartupLocationProperty); }
			set { SetValue(WindowStartupLocationProperty, value); }
		}
		public bool AllowSetWindowOwner {
			get { return (bool)GetValue(AllowSetWindowOwnerProperty); }
			set { SetValue(AllowSetWindowOwnerProperty, value); }
		}
#endif
		public Type WindowType {
			get { return (Type)GetValue(WindowTypeProperty); }
			set { SetValue(WindowTypeProperty, value); }
		}
		public Style WindowStyle {
			get { return (Style)GetValue(WindowStyleProperty); }
			set { SetValue(WindowStyleProperty, value); }
		}
		public string Title {
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public WindowShowMode WindowShowMode {
			get { return (WindowShowMode)GetValue(WindowShowModeProperty); }
			set { SetValue(WindowShowModeProperty, value); }
		}
		protected virtual IWindowSurrogate CreateWindow(object view) {
			IWindowSurrogate window = WindowProxy.GetWindowSurrogate(Activator.CreateInstance(WindowType ?? DefaultWindowType));
			UpdateThemeName(window.RealWindow);
			window.RealWindow.Content = view;
			InitializeDocumentContainer(window.RealWindow, Window.ContentProperty, WindowStyle);
#if !SILVERLIGHT
			window.RealWindow.WindowStartupLocation = this.WindowStartupLocation;
			if(AllowSetWindowOwner && AssociatedObject != null)
				window.RealWindow.Owner = Window.GetWindow(AssociatedObject);
#endif
			return window;
		}
		void OnWindowTypeChanged() {
			if(WindowType == null) return;
			if(!typeof(Window).IsAssignableFrom(WindowType))
				throw new ArgumentException(WindowTypeException);
		}
		void OnTitleChanged() {
			if(window != null)
				window.RealWindow.Title = Title ?? string.Empty;
		}
		void SetTitleBinding() {
			if(string.IsNullOrEmpty(Title))
				DocumentUIServiceBase.SetTitleBinding(window.RealWindow.Content, WindowBase.TitleProperty, window.RealWindow, true);
		}
		void OnWindowClosing(object sender, CancelEventArgs e) {
			DocumentViewModelHelper.OnClose(GetViewModel(window.RealWindow), e);
		}
		void OnWindowClosed(object sender, EventArgs e) {
			window.Closing -= OnWindowClosing;
			window.Closed -= OnWindowClosed;
			DocumentViewModelHelper.OnDestroy(GetViewModel(window.RealWindow));
			window = null;
		}
		object GetViewModel(WindowBase window) {
			return ViewHelper.GetViewModelFromView(window.Content);
		}
		IWindowSurrogate window;
		bool IWindowService.IsWindowAlive {
			get { return window != null; }
		}
		void IWindowService.Show(string documentType, object viewModel, object parameter, object parentViewModel) {
			if(window != null) {
				window.Show();
				return;
			}
			object view = CreateAndInitializeView(documentType, viewModel, parameter, parentViewModel, this);
			window = CreateWindow(view);
			window.RealWindow.Title = Title ?? string.Empty;
			SetTitleBinding();
			window.Closing += OnWindowClosing;
			window.Closed += OnWindowClosed;
			if(WindowShowMode == WindowShowMode.Dialog)
				window.ShowDialog();
			else
				window.Show();
		}
		void IWindowService.Activate() {
			if(window != null) {
				window.Activate();
			}
		}
		void IWindowService.Restore() {
			if(window != null) {
#if !SILVERLIGHT
				window.Show();
#else
				window.RealWindow.Visibility = Visibility.Visible;
#endif
			}
		}
		void IWindowService.Hide() {
			if(window != null) {
#if !SILVERLIGHT
				window.Hide();
#else
				window.RealWindow.Visibility = Visibility.Collapsed;
#endif
			}
		}
		void IWindowService.Close() {
			if(window != null)
				window.Close();
		}
		void IDocumentOwner.Close(IDocumentContent documentContent, bool force) {
			if(window == null || GetViewModel(window.RealWindow) != documentContent) return;
			if(force)
				window.Closing -= OnWindowClosing;
			window.Close();
		}
#if !SILVERLIGHT
		void IWindowService.SetWindowState(WindowState state) {
			if(window != null)
				window.RealWindow.WindowState = state;
		}
#endif
	}
}

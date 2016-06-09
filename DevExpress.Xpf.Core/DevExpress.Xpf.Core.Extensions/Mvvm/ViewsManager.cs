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
using System.Windows.Controls;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Core.MvvmSample.Helpers;
namespace DevExpress.Xpf.Core.MvvmSample {
	public interface IViewsListProvider {
		Type GetViewType(Type moduleType);
	}
	public interface IModuleView : IView {
		void SetViewIsReadyToAppear(bool v);
	}
	public class ViewsManager : IViewsManager {
		IViewsListProvider viewListProvider;
		public ViewsManager(IViewsListProvider viewListProvider) {
			this.viewListProvider = viewListProvider;
		}
		public void CreateView(IModule module) {
			FrameworkElement view = (FrameworkElement)module.View;
			if(view == null) {
				Type viewType = viewListProvider.GetViewType(module.GetType());
				view = (FrameworkElement)Activator.CreateInstance(viewType);
			}
			view.Opacity = 0.0;
			IModuleView viewAsIModuleView = view as IModuleView;
			if(viewAsIModuleView != null) {
				viewAsIModuleView.SetViewIsReadyToAppear(false);
			}
			module.SetView(view);
			view.DataContext = module;
		}
		public void ShowView(IModule module) {
			FrameworkElement view = (FrameworkElement)module.View;
			IModuleView viewAsIModuleView = view as IModuleView;
			if(viewAsIModuleView != null) {
				viewAsIModuleView.BeforeViewDisappear += OnViewBeforeViewDisappear;
				viewAsIModuleView.AfterViewDisappear += OnViewAfterViewDisappear;
				viewAsIModuleView.ViewIsVisibleChanged += OnViewViewIsVisibleChanged;
			}
			view.Opacity = 1.0;
			if(viewAsIModuleView != null) {
				viewAsIModuleView.SetViewIsReadyToAppear(true);
			}
		}
		public IModule GetModule(object view) {
			FrameworkElement viewAsFrameworkElement = view as FrameworkElement;
			return viewAsFrameworkElement == null ? null : viewAsFrameworkElement.DataContext as IModule;
		}
		void OnViewViewIsVisibleChanged(object sender, EventArgs e) {
			FrameworkElement view = (FrameworkElement)sender;
			IModuleView viewAsIModuleView = view as IModuleView;
			IModule module = view.DataContext as IModule;
			if(module != null && viewAsIModuleView != null)
				module.SetIsVisible(viewAsIModuleView.ViewIsVisible);
		}
		void OnViewBeforeViewDisappear(object sender, EventArgs e) {
			FrameworkElement view = (FrameworkElement)sender;
			IModule module = view.DataContext as IModule;
			if(module != null) {
				foreach(IModule submodule in module.GetSubmodules()) {
					if(submodule == null) continue;
					submodule.RaiseBeforeDisappear();
				}
				module.RaiseBeforeDisappear();
			}
		}
		void OnViewAfterViewDisappear(object sender, EventArgs e) {
			FrameworkElement view = (FrameworkElement)sender;
			IModuleView viewAsIModuleView = view as IModuleView;
			IModule module = view.DataContext as IModule;
			if(viewAsIModuleView != null) {
				viewAsIModuleView.ViewIsVisibleChanged -= OnViewViewIsVisibleChanged;
				viewAsIModuleView.BeforeViewDisappear -= OnViewBeforeViewDisappear;
				viewAsIModuleView.AfterViewDisappear -= OnViewAfterViewDisappear;
			}
			if(module != null && module.IsPersistentModule) return;
			view.DataContext = null;
			if(module != null) {
				foreach(IModule submodule in module.GetSubmodules()) {
					if(submodule == null) continue;
					IModuleView subviewAsIModuleView = submodule.View as IModuleView;
					if(subviewAsIModuleView != null)
						subviewAsIModuleView.RaiseAfterViewDisappear();
				}
				module.SetView(null);
				module.Dispose();
			}
			ContentControl cc = view.Parent as ContentControl;
			if(cc != null)
				cc.Content = null;
			ContentPresenter cp = view.Parent as ContentPresenter;
			if(cp != null)
				cp.Content = null;
		}
	}
#if SL
	public class UIElementSplashScreenProvider : DependencyObject, ISplashScreenProvider {
		#region Dependency Properties
		public static readonly DependencyProperty ElementProperty;
		static UIElementSplashScreenProvider() {
			Type ownerType = typeof(UIElementSplashScreenProvider);
			ElementProperty = DependencyProperty.Register("Element", typeof(UIElement), ownerType, new PropertyMetadata(null, RaiseElementChanged));
		}
		static void RaiseElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((UIElementSplashScreenProvider)d).RaiseElementChanged(e);
		}
		#endregion
		bool showSplashScreen;
		public UIElement Element { get { return (UIElement)GetValue(ElementProperty); } set { SetValue(ElementProperty, value); } }
		public bool ShowSplashScreen {
			get { return showSplashScreen; }
			set {
				showSplashScreen = value;
				UpdateElementVisibility();
			}
		}
		void RaiseElementChanged(DependencyPropertyChangedEventArgs e) {
			UpdateElementVisibility();
		}
		void UpdateElementVisibility() {
			if(Element == null) return;
			Element.Visibility = showSplashScreen ? Visibility.Visible : Visibility.Collapsed;
		}
	}
#else
	public class DXSplashScreenProvider : ISplashScreenProvider, ISupportInitialize {
		Type splashScreenType;
		public Type SplashScreenType {
			get { return splashScreenType; }
			set {
				splashScreenType = value;
			}
		}
		public bool ShowSplashScreen {
			get { return DXSplashScreen.IsActive; }
			set {
				if(value && !DXSplashScreen.IsActive) {
					DXSplashScreen.Show(SplashScreenType);
				} else if(!value && DXSplashScreen.IsActive) {
					DXSplashScreen.Close();
					Window mainWindow = Application.Current == null ? null : Application.Current.MainWindow;
					if(mainWindow != null)
						mainWindow.Activate();
				}
			}
		}
		public virtual void BeginInit() { }
		public virtual void EndInit() { }
	}
#endif
}

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
using System.Windows.Controls;
using DevExpress.Xpf.WindowsUI.Navigation;
using System.Windows;
using DevExpress.Xpf.WindowsUI.Base;
using System.ComponentModel;
namespace DevExpress.Xpf.WindowsUI {
#if !SILVERLIGHT
#endif
	public class NavigationPage : ContentControl, INavigationAware {
		public NavigationPage() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(NavigationPage);
#endif
		}
		static NavigationPage() {
			var dProp = new DependencyPropertyRegistrator<NavigationPage>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.RegisterAttached("NavigationCacheMode", ref NavigationCacheModeProperty, NavigationCacheMode.Disabled);
		}
		public static NavigationCacheMode GetNavigationCacheMode(DependencyObject obj) {
			return (NavigationCacheMode)obj.GetValue(NavigationCacheModeProperty);
		}
		public static void SetNavigationCacheMode(DependencyObject obj, NavigationCacheMode value) {
			obj.SetValue(NavigationCacheModeProperty, value);
		}
		public static readonly DependencyProperty NavigationCacheModeProperty;
		protected virtual void OnNavigatedTo(NavigationEventArgs e) { }
		protected virtual void OnNavigatingFrom(NavigatingEventArgs e) { }
		protected virtual void OnNavigatedFrom(NavigationEventArgs e) { }
		#region INavigationAware Members
		void INavigationAware.NavigatedTo(NavigationEventArgs e) {
			OnNavigatedTo(e);
		}
		void INavigationAware.NavigatingFrom(NavigatingEventArgs e) {
			OnNavigatingFrom(e);
		}
		void INavigationAware.NavigatedFrom(NavigationEventArgs e) {
			OnNavigatedFrom(e);
		}
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
		}
		public AppBar TopAppBar {
			get { return (AppBar)GetValue(TopAppBarProperty); }
			set { SetValue(TopAppBarProperty, value); }
		}
		public AppBar BottomAppBar {
			get { return (AppBar)GetValue(BottomAppBarProperty); }
			set { SetValue(BottomAppBarProperty, value); }
		}
		public static readonly DependencyProperty TopAppBarProperty =
			DependencyProperty.Register("TopAppBar", typeof(AppBar), typeof(NavigationPage),
				new PropertyMetadata(null, (d, e) => ((NavigationPage)d).OnAppBarChanged(e.OldValue as AppBar, e.NewValue as AppBar, false)));
		public static readonly DependencyProperty BottomAppBarProperty =
			DependencyProperty.Register("BottomAppBar", typeof(AppBar), typeof(NavigationPage),
				new PropertyMetadata(null, (d, e) => ((NavigationPage)d).OnAppBarChanged(e.OldValue as AppBar, e.NewValue as AppBar, true)));
		void OnAppBarChanged(AppBar oldValue, AppBar newValue, bool isBottom) {
			if (newValue != null) {
				newValue.Alignment = isBottom ? AppBarAlignment.Bottom : AppBarAlignment.Top;
			}
		}
	}
}

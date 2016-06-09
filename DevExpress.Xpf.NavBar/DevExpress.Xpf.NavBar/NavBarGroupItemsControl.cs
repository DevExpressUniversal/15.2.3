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
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.NavBar.Platform;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.NavBar {
	public partial class NavBarGroupItemsControl : ItemsControl {
		public static readonly DependencyProperty ViewKindProperty;
		public static readonly DependencyProperty ShowViewBorderProperty;		 
		public static readonly DependencyProperty ItemsSourceCoreProperty;
		static NavBarGroupItemsControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NavBarGroupItemsControl), new FrameworkPropertyMetadata(typeof(NavBarGroupItemsControl)));
			FocusableProperty.OverrideMetadata(typeof(NavBarGroupItemsControl), new FrameworkPropertyMetadata(false));
			ItemsSourceCoreProperty = DependencyProperty.Register("ItemsSourceCore", typeof(INotifyCollectionChanged), typeof(NavBarGroupItemsControl), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceCorePropertyChanged)));
			ViewKindProperty = DependencyPropertyManager.Register("ViewKind", typeof(NavBarViewKind), typeof(NavBarGroupItemsControl), new FrameworkPropertyMetadata(NavBarViewKind.ExplorerBar, (d, e) => ((NavBarGroupItemsControl)d).UpdateBorderState()));
			ShowViewBorderProperty = DependencyPropertyManager.Register("ShowViewBorder", typeof(bool), typeof(NavBarGroupItemsControl), new FrameworkPropertyMetadata(true, (d, e) => ((NavBarGroupItemsControl)d).UpdateBorderState()));	
		}
		protected static void OnItemsSourceCorePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarGroupItemsControl)d).ItemsSource = ((NavBarGroupItemsControl)d).ItemsSourceCore as IEnumerable;
		}
		public INotifyCollectionChanged ItemsSourceCore {
			get { return (INotifyCollectionChanged)GetValue(ItemsSourceCoreProperty); }
			set { SetValue(ItemsSourceCoreProperty, value); }
		}
		public NavBarViewKind ViewKind {
			get { return (NavBarViewKind)GetValue(ViewKindProperty); }
			set { SetValue(ViewKindProperty, value); }
		}
		public bool ShowViewBorder {
			get { return (bool)GetValue(ShowViewBorderProperty); }
			set { SetValue(ShowViewBorderProperty, value); }
		}
		public NavBarGroupItemsControl() {
			Loaded += OnLoaded;  
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateBorderState();
		}
		protected virtual void UpdateBorderState() {
			if (!ShowViewBorder) {
				VisualStateManager.GoToState(this, "Hidden", false);
			} else {
				VisualStateManager.GoToState(this, ViewKind.ToString(), false);
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			BindingOperations.SetBinding(this, NavBarGroupItemsControl.ItemsSourceCoreProperty, new Binding("Groups"));
			BindingOperations.SetBinding(this, NavBarGroupItemsControl.ShowViewBorderProperty, new Binding("View.ShowBorder"));
		}
		NavBarControl navBar = null;
		NavBarControl NavBar {
			get {
				if (navBar == null)
					navBar = LayoutHelper.FindParentObject<NavBarControl>(this);
				return navBar;
			}
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			if (element is NavBarGroupControl) {
				((NavBarGroupControl)element).NavBar = NavBar;
			}
			BindingOperations.SetBinding(element, NavBarGroupControl.VisibilityProperty, new Binding("IsVisible") { Source = item, Converter = new BooleanToVisibilityConverter() });
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);			
			(element as NavBarGroupControl).Do(x => x.NavBar = null);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			NavBarGroupControl group = new NavBarGroupControl();
			group.SetValue(NavigationPaneView.ElementProperty, this.GetValue(NavigationPaneView.ElementProperty));
			return group;
		}
	}
}

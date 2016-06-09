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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI.Base;
#if SILVERLIGHT
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.WindowsUI.Navigation {
	public class Navigation {
		public static readonly DependencyProperty NavigateToProperty;
		public static readonly DependencyProperty NavigationParameterProperty;
		public static readonly DependencyProperty NavigationTargetProperty;
		public static readonly DependencyProperty NavigationHandlerProperty;
		static NavigationHandlerFactory NavigationHandlerFactory;
		static Navigation() {
			NavigationHandlerFactory = new NavigationHandlerFactory();
			var dProp = new DependencyPropertyRegistrator<Navigation>();
			dProp.RegisterAttached("NavigateTo", ref NavigateToProperty, (object)null, OnNavigateToChanged);
			dProp.RegisterAttached("NavigationParameter", ref NavigationParameterProperty, (object)null, OnNavigationParameterChanged);
			dProp.RegisterAttached("NavigationTarget", ref NavigationTargetProperty, (object)null, OnNavigationTargetChanged);
			dProp.RegisterAttached("NavigationHandler", ref NavigationHandlerProperty, (INavigationHandler)null, OnNavigationHandlerChanged);
		}
		public static object GetNavigateTo(DependencyObject target) {
			return (object)target.GetValue(NavigateToProperty);
		}
		public static void SetNavigateTo(DependencyObject target, object value) {
			target.SetValue(NavigateToProperty, value);
		}
		static void OnNavigateToChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			OnNavigateToChanged(o, (object)e.OldValue, (object)e.NewValue);
		}
		static void OnNavigateToChanged(DependencyObject o, object oldValue, object newValue) {
			INavigationHandler handler = GetNavigationHandler(o);
			if(handler == null) {
				handler = NavigationHandlerFactory.Resolve(o);
				SetNavigationHandler(o, handler);
			}
			RegisterSender(o, handler);
		}
		public static object GetNavigationParameter(DependencyObject target) {
			return (object)target.GetValue(NavigationParameterProperty);
		}
		public static void SetNavigationParameter(DependencyObject target, object value) {
			target.SetValue(NavigationParameterProperty, value);
		}
		static void OnNavigationParameterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			OnNavigationParameterChanged(o, (object)e.OldValue, (object)e.NewValue);
		}
		static void OnNavigationParameterChanged(DependencyObject o, object oldValue, object newValue) {
			INavigationHandler handler = GetNavigationHandler(o);
			if(handler == null) {
				handler = NavigationHandlerFactory.Resolve(o);
				SetNavigationHandler(o, handler);
			}
			RegisterSender(o, handler);
		}
		public static object GetNavigationTarget(DependencyObject target) {
			return target.GetValue(NavigationTargetProperty);
		}
		public static void SetNavigationTarget(DependencyObject target, object value) {
			target.SetValue(NavigationTargetProperty, value);
		}
		static void OnNavigationTargetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			OnNavigationTargetChanged(o, (object)e.OldValue, (object)e.NewValue);
		}
		static void OnNavigationTargetChanged(DependencyObject o, object oldValue, object newValue) {
			INavigationHandler handler = GetNavigationHandler(o);
			if(handler == null) {
				handler = NavigationHandlerFactory.Resolve(o);
				SetNavigationHandler(o, handler);
			}
			if(oldValue != null)
				handler.UnregisterTarget(oldValue);
			if(newValue != null)
				handler.RegisterTarget(newValue);
		}
		public static INavigationHandler GetNavigationHandler(DependencyObject target) {
			return (INavigationHandler)target.GetValue(NavigationHandlerProperty);
		}
		public static void SetNavigationHandler(DependencyObject target, INavigationHandler value) {
			target.SetValue(NavigationHandlerProperty, value);
		}
		static void OnNavigationHandlerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			OnNavigationHandlerChanged(o, (INavigationHandler)e.OldValue, (INavigationHandler)e.NewValue);
		}
		static void OnNavigationHandlerChanged(DependencyObject o, INavigationHandler oldValue, INavigationHandler newValue) {
			if(oldValue != null) {
				oldValue.UnregisterSender(o);
				oldValue.UnregisterTarget(GetNavigationTarget(o));
			}
			INavigationHandler handler = newValue;
			if(handler == null) return;
			RegisterSender(o, handler);
		}
		private static void RegisterSender(DependencyObject sender, INavigationHandler handler) {
			AssertionException.IsNotNull(sender);
			AssertionException.IsNotNull(handler);
			handler.UnregisterSender(sender);
			object navigateTo = GetNavigateTo(sender);
			object param = GetNavigationParameter(sender);
			handler.RegisterSender(sender, navigateTo, param);
		}
	}
}

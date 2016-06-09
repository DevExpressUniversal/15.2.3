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
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.WindowsUI.Navigation {
	static class NavigationHelper {
		static void NavigateTo(FrameworkElement element, string targetType, object dataContext, BindingBase targetParameterBinding, BindingBase targetTypeBinding = null) {
			object targetParameter = null;
			if(dataContext != null) {
				ValueProvider valueProvider = new ValueProvider() { DataContext = dataContext };
				if(targetParameterBinding != null) {
					BindingOperations.SetBinding(valueProvider, ValueProvider.ValueProperty, targetParameterBinding);
					targetParameter = valueProvider.Value;
				}
				if(targetTypeBinding != null) {
					BindingOperations.SetBinding(valueProvider, ValueProvider.ValueProperty, targetTypeBinding);
					targetType = (valueProvider.Value as string) ?? targetType;
				}
			}
			NavigateTo(element, targetType, targetParameter);
		}
		static void NavigateTo(FrameworkElement element, string targetType, object targetParameter, bool clearNavigationHistory = false) {
			if(!string.IsNullOrEmpty(targetType)) {
				INavigationContainer frame = GetNavigationContainer(element);
				frame.Navigate(targetType, targetParameter.ToString());
			}
		}
		static object ResolveView(FrameworkElement element, string targetType, object dataContext, BindingBase targetParameterBinding, BindingBase targetTypeBinding = null) {
			object targetParameter = null;
			if(dataContext != null) {
				ValueProvider valueProvider = new ValueProvider() { DataContext = dataContext };
				if(targetParameterBinding != null) {
					BindingOperations.SetBinding(valueProvider, ValueProvider.ValueProperty, targetParameterBinding);
					targetParameter = valueProvider.Value;
				}
				if(targetTypeBinding != null) {
					BindingOperations.SetBinding(valueProvider, ValueProvider.ValueProperty, targetTypeBinding);
					targetType = (valueProvider.Value as string) ?? targetType;
				}
			}
			object content = ResolveViewHardWay(targetType);
			return content;
		}
		static object ResolveViewHardWay(string documentType) {
			Application app = Application.Current;
			if(app == null)
				return null;
			Type type = app.GetType().Assembly.GetTypes().FirstOrDefault(t => t.Name == documentType);
			if(type == null)
				return null;
			return Activator.CreateInstance(type);
		}
		public static void GoBack(FrameworkElement element, object param = null) {
			INavigationContainer frame = GetNavigationContainer(element);
			if(frame != null)
				frame.GoBack(param);
		}
		public static void GoForward(FrameworkElement element) {
			INavigationContainer frame = GetNavigationContainer(element);
			if(frame != null)
				frame.GoForward();
		}
		public static bool CanGoBack(FrameworkElement element) {
			INavigationContainer frame = GetNavigationContainer(element);
			return frame != null ? frame.CanGoBack : false;
		}
		public static bool CanGoForward(FrameworkElement element) {
			INavigationContainer frame = GetNavigationContainer(element);
			return frame != null ? frame.CanGoForward : false;
		}
		internal static INavigationContainer GetNavigationContainer(FrameworkElement element) {
			if(element == null) return null;
			if(element is INavigationContainer) return (INavigationContainer)element;
			return LayoutHelper.FindParentObject<INavigationContainer>(element);
		}
#if !SILVERLIGHT
		internal static Frame GetFrame(FrameworkElement element) {
			if(element == null) return null;
			if(element is Frame) return (Frame)element;
			return LayoutHelper.FindParentObject<Frame>(element);
		}
#endif
		class ValueProvider : FrameworkElement {
			public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(ValueProvider), new PropertyMetadata(null));
			public object Value {
				get { return GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}
		}
	}
}

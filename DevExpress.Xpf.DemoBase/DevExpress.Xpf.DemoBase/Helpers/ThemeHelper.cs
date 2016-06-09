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
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using DevExpress.DemoData.Helpers;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public static class ThemeHelper {
		const string ThemeXamlPath = ";component/Theme.xaml";
		#region Dependency Properties
		public static readonly DependencyProperty ThemeNameProperty;
		public static readonly DependencyProperty ContentThemeNameProperty;
		public static readonly DependencyProperty ApplyTextForegroundToContentProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentPropertyInfoProperty;
		static ThemeHelper() {
			Type ownerType = typeof(ThemeHelper);
			ThemeNameProperty = DependencyProperty.RegisterAttached("ThemeName", typeof(string), ownerType, new PropertyMetadata(null, OnThemeNameChanged));
			ContentThemeNameProperty = DependencyProperty.RegisterAttached("ContentThemeName", typeof(string), ownerType, new PropertyMetadata(null, OnReusablePropertiesChanged));
			ApplyTextForegroundToContentProperty = DependencyProperty.RegisterAttached("ApplyTextForegroundToContent", typeof(bool), ownerType, new PropertyMetadata(false, OnReusablePropertiesChanged));
			ContentProperty = DependencyProperty.RegisterAttached("Content", typeof(object), ownerType, new PropertyMetadata(null, OnContentChanged));
			ContentPropertyInfoProperty = DependencyProperty.RegisterAttached("ContentPropertyInfo", typeof(PropertyInfo), ownerType, new PropertyMetadata(null));
		}
		#endregion
		class ThemeInstance {
			public static string GetThemeName(string themeInstanceName) {
				return themeInstanceName.Split(',')[0];
			}
			public static string GetGroupName(string themeInstanceName) {
				return themeInstanceName.Split(':')[1];
			}
		}
		static Dictionary<string, string> actualThemeNames = new Dictionary<string, string>();
		public static void SetActualThemeName(string themeName, string actualThemeName) {
			if(actualThemeNames.ContainsKey(themeName))
				actualThemeNames[themeName] = actualThemeName;
			else
				actualThemeNames.Add(themeName, actualThemeName);
		}
		public static void SetThemeName(DependencyObject element, string value) { element.SetValue(ThemeNameProperty, value); }
		public static string GetThemeName(DependencyObject element) { return (string)element.GetValue(ThemeNameProperty); }
		public static void SetContentThemeName(DependencyObject element, string value) { element.SetValue(ContentThemeNameProperty, value); }
		public static string GetContentThemeName(DependencyObject element) { return (string)element.GetValue(ContentThemeNameProperty); }
		public static void SetApplyTextForegroundToContent(DependencyObject element, bool value) { element.SetValue(ApplyTextForegroundToContentProperty, value); }
		public static bool GetApplyTextForegroundToContent(DependencyObject element) { return (bool)element.GetValue(ApplyTextForegroundToContentProperty); }
		public static void SetContent(DependencyObject element, object value) { element.SetValue(ContentProperty, value); }
		public static object GetContent(DependencyObject element) { return element.GetValue(ContentProperty); }
		public static void SetContentPropertyInfo(DependencyObject element, PropertyInfo value) { element.SetValue(ContentPropertyInfoProperty, value); }
		public static PropertyInfo GetContentPropertyInfo(DependencyObject element) { return (PropertyInfo)element.GetValue(ContentPropertyInfoProperty); }
		public static string GetActualThemeName(string themeName) {
			string actualThemeName;
			if(!actualThemeNames.TryGetValue(themeName, out actualThemeName))
				actualThemeName = themeName;
			if(string.IsNullOrEmpty(actualThemeName))
				actualThemeName = Theme.DefaultThemeName;
			return actualThemeName;
		} 
		static void OnThemeNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ApplyTheme(d as FrameworkElement, e.NewValue as string);
		}
		static void OnReusablePropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if((e.Property == ContentThemeNameProperty && e.OldValue == null && !GetApplyTextForegroundToContent(d)) || (e.Property == ApplyTextForegroundToContentProperty && !(bool)e.OldValue && GetContentThemeName(d) == null)) {
				BindingOperations.SetBinding(d, ContentProperty, new Binding("Content") { Mode = BindingMode.OneWay, Source = d });
			} else {
				ApplyReusableProperties(d, GetContent(d) as DependencyObject);
			}
		}
		static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ApplyReusableProperties(d, e.NewValue as DependencyObject);
		}
		static void ApplyReusableProperties(DependencyObject d, DependencyObject newContentValue) {
			ApplyThemeToContent(d, GetContentThemeName(d));
			ApplyForeground(d, newContentValue);
		}
		static void ApplyThemeToContent(DependencyObject d, string newValue) {
			if(d == null || string.IsNullOrEmpty(newValue)) return;
			PropertyInfo attachedInfo = GetContentPropertyInfo(d);
			if(attachedInfo == null) {
				attachedInfo = d.GetType().GetProperty("Content");
				if(attachedInfo == null) return;
				SetContentPropertyInfo(d, attachedInfo);
			}
			ApplyTheme(attachedInfo.GetValue(d, null) as FrameworkElement, newValue);
		}
		static void ApplyForeground(DependencyObject d, DependencyObject e) {
			if(GetApplyTextForegroundToContent(d) && e != null && !d.IsInDesignTool()) {
				TextElement.SetForeground(e, TextElement.GetForeground(d));
			}
		}
		static void ApplyTheme(FrameworkElement owner, string name) {
			if(string.IsNullOrEmpty(name) || owner == null || owner.IsInDesignTool()) return;
			string themeName = ThemeInstance.GetThemeName(name);
			ThemeManager.SetThemeName(owner, GetActualThemeName(themeName));
		}
	}
}

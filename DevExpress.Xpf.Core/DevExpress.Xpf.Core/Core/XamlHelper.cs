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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Data;
#if !SILVERLIGHT
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Threading;
#else
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
#endif
namespace DevExpress.Xpf.Core {
	public static class XamlHelper {
		public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached("Name", typeof(string), typeof(XamlHelper), new PropertyMetadata(null, OnNameChanged));
		public static string GetName(FrameworkContentElement d) { return (string)d.GetValue(NameProperty); }
		public static void SetName(FrameworkContentElement d, string name) { d.SetValue(NameProperty, name); }
		static void OnNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!d.IsPropertySet(FrameworkContentElement.NameProperty))
				d.SetValue(FrameworkContentElement.NameProperty, e.NewValue);
		}
		const string DefaultNamespaces = 
#if SILVERLIGHT
			@"xmlns=""http://schemas.microsoft.com/client/2007"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""";
#else
			@"xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""";
#endif
		static string namespaces = DefaultNamespaces;
		static string GetDefaultTemplateFormatText(string templateTypeName) {
			return GetDefaultTemplateFormatText(templateTypeName, string.Empty);
		}
		static string GetDefaultTemplateFormatText(string templateTypeName, string additionalAttributes) {
			return "<" + templateTypeName + " " + namespaces + " " + additionalAttributes + ">{0}</" + templateTypeName + ">";
		}
		static string GetDefaultDisplayTemplateText(string template, string templateTypeName, string additionalAttributes) {
			return string.Format(GetDefaultTemplateFormatText(templateTypeName, additionalAttributes), template);
		}
		static string GetDefaultDisplayTemplateText(string template, string templateTypeName) {
			return GetDefaultDisplayTemplateText(template, templateTypeName, string.Empty);
		}
		public static ControlTemplate GetControlTemplate(string template) {
			return LoadObjectCore(GetDefaultDisplayTemplateText(template, "ControlTemplate")) as ControlTemplate;
		}
		public static ItemsPanelTemplate GetItemsPanelTemplate(string template) {
			return LoadObjectCore(GetDefaultDisplayTemplateText(template, "ItemsPanelTemplate")) as ItemsPanelTemplate;
		}
		public static T LoadObject<T>(string xamlContent, string additionalAttributes) {
			return (T)LoadObjectCore(GetDefaultDisplayTemplateText(xamlContent, typeof(T).Name, additionalAttributes));
		}
		public static object GetObject(string template) {
			return LoadObjectCore(String.Format("<{0} {1}/>", template, namespaces));
		}
		public static DataTemplate GetTemplate(string template) {
			return LoadObjectCore(GetDefaultDisplayTemplateText(template, "DataTemplate")) as DataTemplate;
		}
		public static void SetLocalNamespace(string name) {
			namespaces = DefaultNamespaces + @" xmlns:local=""" + name + @"""";
		}
		public static object LoadObjectCore(string xaml) {
#if SILVERLIGHT
			return XamlReader.Load(xaml);
#else
			return XamlReader.Parse(xaml);
#endif
		}
		public static T LoadContent<T>(string template) where T:DependencyObject {
			return GetTemplate(template).LoadContent() as T;
		}
	}
}

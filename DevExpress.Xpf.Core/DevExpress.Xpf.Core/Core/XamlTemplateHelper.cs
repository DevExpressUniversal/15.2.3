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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.IO;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.Xpf.Core {
	public static class XamlTemplateHelper {
		const string DefaultNamespaces = " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" ";
		public const string RootName = "Root";
		public static DataTemplate CreateDataTemplate(Type controlClass) {
			StringBuilder namespaces = null;
			string controlClassString = GetTypeString("ddd", controlClass, ref namespaces);
			string xaml = "<DataTemplate" + namespaces.ToString() + "><" + controlClassString + " /></DataTemplate>";
			return (DataTemplate)XamlReader.Parse(xaml);
		}
		public static ControlTemplate CreateControlTemplate(Type targetType, Type controlClass) {
			StringBuilder namespaces = null;
			string targetTypeString = GetTypeString("ddd1", targetType, ref namespaces);
			string controlClassString = GetTypeString("ddd2", controlClass, ref namespaces);
			string xaml = "<ControlTemplate" + namespaces.ToString() + "TargetType=\"" + targetTypeString + "\"><" + controlClassString + " x:Name=\"" + RootName + "\" /></ControlTemplate>";
			return (ControlTemplate)XamlReader.Parse(xaml);
		}
		public static T CreateObjectFromTemplate<T>(DataTemplate template) where T : class {
			if(template == null) return null;
			DependencyObject loadedContent = template.LoadContent();
			if(loadedContent == null) return null;
			T content = loadedContent as T;
			if(content != null) return content;
			ContentControl contentControl = loadedContent as ContentControl;
			if(contentControl != null) return contentControl.Content as T;
			ContentPresenter contentPresenter = loadedContent as ContentPresenter;
			if(contentPresenter != null) return contentPresenter.Content as T;
			return null;
		}
		public static FrameworkElement CreateVisualTree(object content, DataTemplate contentTemplate) {
			if(contentTemplate == null) return content as FrameworkElement;
			FrameworkElement visualTree = contentTemplate.LoadContent() as FrameworkElement;
			if(visualTree == null) return null;
			visualTree.DataContext = content;
			return visualTree;
		}
		public static object CreateObjectFromXaml(Type targetType, object source) {
			if(source == null) return null;
			if(source.GetType().IsSubclassOf(targetType)) return source;
			try {
#if SL
				StringBuilder namespaces = null;
				string typeString = GetTypeString("ddd", targetType, ref namespaces);
				string xaml = "<" + typeString + namespaces + ">" + source.ToString() + "</" + typeString + ">";
				return Parse(xaml);
#else
				TypeConverter converter = TypeDescriptor.GetConverter(targetType);
				return converter.ConvertFromInvariantString(source.ToString());
#endif
			} catch (Exception) {
				return source;
			}
		}
		public static string GetTypeString(string prefix, Type type, ref StringBuilder namespaces) {
			if(namespaces == null)
				namespaces = new StringBuilder(DefaultNamespaces);
			namespaces.Append(string.Format(" xmlns:" + prefix + "=\"clr-namespace:{0};assembly={1}\" ", type.Namespace, AssemblyHelper.GetPartialName(type.Assembly)));
			return prefix + ":" + type.Name;
		}
		public static string GetXamlPath(string path) {
#if SL
			return path.Replace(".xaml", ".SL.xaml");
#else
			return path;
#endif
		}
	}
}

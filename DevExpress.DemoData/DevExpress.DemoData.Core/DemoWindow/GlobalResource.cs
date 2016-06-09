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
using System.Reflection;
using DevExpress.DemoData.Utils;
namespace DevExpress.DemoData.Helpers {
	public class GlobalResource : DependencyObjectExt {
		#region Dependency Properties
		public static readonly DependencyProperty AssemblyProperty;
		public static readonly DependencyProperty XamlPathProperty;
		public static readonly DependencyProperty KeyProperty;
		public static readonly DependencyProperty ResourceProperty;
		static GlobalResource() {
			Type ownerType = typeof(GlobalResource);
			AssemblyProperty = DependencyProperty.Register("Assembly", typeof(string), ownerType, new PropertyMetadata(null, RaiseAssemblyChanged));
			XamlPathProperty = DependencyProperty.Register("XamlPath", typeof(string), ownerType, new PropertyMetadata(null, RaiseXamlPathChanged));
			KeyProperty = DependencyProperty.Register("Key", typeof(object), ownerType, new PropertyMetadata(null, RaiseKeyChanged));
			ResourceProperty = DependencyProperty.Register("Resource", typeof(Style), ownerType, new PropertyMetadata(null, RaiseResourceChanged));
		}
		string assemblyValue = null;
		string xamlPathValue = null;
		object keyValue = null;
		object resourceValue = null;
		static void RaiseAssemblyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GlobalResource)d).assemblyValue = (string)e.NewValue;
			((GlobalResource)d).RaiseAssemblyChanged(e);
		}
		static void RaiseXamlPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GlobalResource)d).xamlPathValue = (string)e.NewValue;
			((GlobalResource)d).RaiseXamlPathChanged(e);
		}
		static void RaiseKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GlobalResource)d).keyValue = e.NewValue;
			((GlobalResource)d).RaiseKeyChanged(e);
		}
		static void RaiseResourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GlobalResource)d).resourceValue = e.NewValue;
			((GlobalResource)d).RaiseResourceChanged(e);
		}
		#endregion
		static Dictionary<string, ResourceDictionary> resources = new Dictionary<string, ResourceDictionary>();
		public string Assembly { get { return assemblyValue; } set { SetValue(AssemblyProperty, value); } }
		public string XamlPath { get { return xamlPathValue; } set { SetValue(XamlPathProperty, value); } }
		public object Key { get { return keyValue; } set { SetValue(KeyProperty, value); } }
		public object Resource { get { return resourceValue; } private set { SetValue(ResourceProperty, value); } }
		void RaiseAssemblyChanged(DependencyPropertyChangedEventArgs e) {
			UpdateResource();
		}
		void RaiseXamlPathChanged(DependencyPropertyChangedEventArgs e) {
			UpdateResource();
		}
		void RaiseKeyChanged(DependencyPropertyChangedEventArgs e) {
			UpdateResource();
		}
		void RaiseResourceChanged(DependencyPropertyChangedEventArgs e) {
			UpdateResource();
		}
		void UpdateResource() {
			Assembly assembly = Assembly == null ? null : AssemblyHelper.GetLoadedAssembly(Assembly);
			if(assembly == null || XamlPath == null || Key == null) {
				Resource = null;
				return;
			}
			ResourceDictionary rd = GetResourceDictionary(assembly, XamlPath);
			Resource = rd == null ? null : rd[Key];
		}
		static ResourceDictionary GetResourceDictionary(Assembly assembly, string xamlPath) {
			string uri = ResourceUri.GetUriString(assembly, xamlPath, true);
			ResourceDictionary rd;
			lock(resources) {
				if(!resources.TryGetValue(uri, out rd)) {
					try {
						if(Application.Current != null) {
							rd = new ResourceDictionary() { Source = AssemblyHelper.GetResourceUri(assembly, xamlPath) };
							Application.Current.Resources.MergedDictionaries.Add(rd);
						} else {
							rd = null;
						}
					} catch {
						rd = null;
					}
					resources.Add(uri, rd);
				}
			}
			return rd;
		}
	}
}

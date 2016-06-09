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
#if SILVERLIGHT
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using System.Windows.Resources;
using DevExpress.Utils;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.Xpf.WindowsUI.Base {
	static class XamlLoaderHelper {
#if SILVERLIGHT
			internal static class UriHelper {
				const string PathSeparator = "/";
				const string ShemeAndHost = "http://localhost";
				const string PartComponentWithSlash = ";component/";
				const string PartComponentWithoutSlash = ";component";
				static Uri MakeAbsolute(Uri uri) {
					if(uri == null || uri.OriginalString.StartsWith(PathSeparator, StringComparison.Ordinal)) {
						return new Uri(ShemeAndHost + uri, UriKind.Absolute);
					}
					else {
						return new Uri(ShemeAndHost + PathSeparator + uri, UriKind.Absolute);
					}
				}
				static string GetEntryPointAssembly() {
					string assemblyPartSource = null;
					foreach(AssemblyPart ap in Deployment.Current.Parts) {
						if(AssemblyHelper.IsEntryAssembly(ap.Source.Substring(0, ap.Source.Length - 4))) {
							assemblyPartSource = ap.Source;
							break;
						}
					}
					return assemblyPartSource.Substring(0, assemblyPartSource.Length - 4);
				}
				internal static string GetUriPath(Uri uri) {
					UriComponents components = UriComponents.Path;
					if(uri.OriginalString.StartsWith("/", StringComparison.Ordinal)) {
						components |= UriComponents.KeepDelimiter;
					}
					return MakeAbsolute(uri).GetComponents(components, UriFormat.SafeUnescaped);
				}
				internal static Uri GetResourceUri(string path) {
					string assembly = null;
					string pathWithouAssembly = null;
					if(path.Contains(UriHelper.PartComponentWithSlash)) {
						string[] pathSplitted = path.Split(new string[] { UriHelper.PartComponentWithoutSlash }, StringSplitOptions.RemoveEmptyEntries);
						if(pathSplitted.Length != 2) {
							return null;
						}
						assembly = pathSplitted[0];
						pathWithouAssembly = pathSplitted[1];
					}
					else {
						assembly = GetEntryPointAssembly();
						pathWithouAssembly = path;
					}
					if(String.IsNullOrEmpty(assembly)) {
						return null;
					}
					pathWithouAssembly = Uri.EscapeUriString(pathWithouAssembly);
					assembly = Uri.EscapeUriString(assembly);
					return new Uri(assembly + UriHelper.PartComponentWithoutSlash + pathWithouAssembly, UriKind.RelativeOrAbsolute);
				}
			}
			static Type GetTypeFromString(string typeName) {
				Type type = null;
				foreach(AssemblyPart ap in Deployment.Current.Parts) {
					StreamResourceInfo sri = Application.GetResourceStream(new Uri(ap.Source, UriKind.Relative));
					if(sri != null) {
						Assembly assembly = new AssemblyPart().Load(sri.Stream);
						if(assembly != null) {
							type = Type.GetType(typeName + "," + assembly, false);
							if(type != null) break;
						}
					}
				}
				return type;
			}
			static readonly Regex XClassRegex = new Regex(".*x:Class=\"(.*?)\"", RegexOptions.CultureInvariant);
			static string GetXClassString(string xaml) {
				Match m = XClassRegex.Match(xaml);
				return m != Match.Empty ? m.Groups[1].Value : null;
			}
			static string LoadXamlString(string path) {
				Uri resourceUri = UriHelper.GetResourceUri(path);
				string xamlString = null;
				if(resourceUri != null) {
					StreamResourceInfo sri = Application.GetResourceStream(resourceUri);
					if(sri != null) {
						using(StreamReader reader = new StreamReader(sri.Stream)) {
							xamlString = reader.ReadToEnd();
						}
					}
				}
				return xamlString;
			}
			public static object LoadContentFromUri(Uri uri) {
				object content = null;
				try {
					string path = UriHelper.GetUriPath(uri);
					string xaml = LoadXamlString(path);
					if(string.IsNullOrEmpty(xaml)) return content;
					string classString = GetXClassString(xaml);
					if(string.IsNullOrEmpty(classString)) {
						content = XamlReader.Load(xaml);
					}
					else {
						Type type = GetTypeFromString(classString);
						if(type != null) content = Activator.CreateInstance(type);
					}
				}
				catch(Exception) { }
				return content;
			}
#else
		static void TryPreventWindowShowing(ContentControl control) {
			Window w = control as Window;
			if(w != null) w.Close();
		}
		public static object LoadContentFromUri(Uri uri) {
			object content = null;
			object component = Application.LoadComponent(uri);
			ContentControl contentControl = component as ContentControl;
			if(contentControl != null) {
				TryPreventWindowShowing(contentControl);
				if(contentControl is Window) {
					content = contentControl.Content;
					contentControl.Content = null;
				}
				else
					content = contentControl;
			}
			Page page = component as Page;
			if(page != null) {
				content = page.Content;
				page.Content = null;
			}
			if(content != component && content is DependencyObject && component is DependencyObject)
				NameScope.SetNameScope((DependencyObject)content, NameScope.GetNameScope((DependencyObject)component));
			return content;
		}
#endif
	}
}

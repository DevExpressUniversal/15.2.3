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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Linq;
#if !DEMO && !FREE
using DevExpress.Utils;
#endif
#if !SILVERLIGHT
using System.Windows.Interop;
#endif
#if DEMO
namespace DevExpress.Internal {
#elif MVVM
namespace DevExpress.Mvvm.UI.Native {
#else
namespace DevExpress.Xpf.Utils {
#endif
#if !FREE
	static class ApplicationHelper {
#if SILVERLIGHT
		const int ExtensionLenth = 4;
		public const string RelativeManifestName = "AppManifest.xaml";
#else
		public const string ManifestExtension = ".manifest";
		const int ExtensionLenth = 0;
		static string relativeManifestName;
		static string relativePath;
		static string EntryManifestName { get { return AssemblyHelper.EntryAssembly.ManifestModule + ManifestExtension; } }
#if !DEMO
		readonly static EnvironmentStrategy environmentStrategy;
		static ApplicationHelper() {
			environmentStrategy = GetEnvironmentStrategy();
			Environment.Initialize();
		}
		static EnvironmentStrategy Environment { get { return environmentStrategy; } }
		static EnvironmentStrategy GetEnvironmentStrategy() {
			if(BrowserInteropHelper.IsBrowserHosted) {
				if(DevExpress.Xpf.Core.ThemeManager.IgnoreManifest) {
					return new IgnoreManifestStrategy();
				}
				return new BrowserStrategy();
			}
			return new IgnoreManifestStrategy();
		}
		public static List<string> GetAvailableAssemblies() {
			List<string> assemblies = new List<string>(AppDomain.CurrentDomain.GetAssemblies().Length);
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				assemblies.Add(assembly.FullName);
			}
			Assembly entryAssembly = AssemblyHelper.EntryAssembly;
			if(entryAssembly == null)
				return assemblies;
			AssemblyName[] assemblyNames = entryAssembly.GetReferencedAssemblies();
			foreach(AssemblyName asmName in assemblyNames) {
				assemblies.Add(asmName.FullName);
			}
			assemblies.AddRange(DevExpress.Xpf.Core.ThemeManager.PluginAssemblies);
			Environment.AddAvailableAssemblies(ref assemblies);
			return assemblies;
		}
#endif
		public static string RelativeManifestName {
			get {
				if(relativeManifestName == null) {
					relativeManifestName = GetRelativeManifestName();
				}
				return relativeManifestName;
			}
		}
		public static string RelativePath {
			get {
				if(relativePath == null)
					relativePath = GetRelativePath();
				return relativePath;
			}
		}
		static string GetRelativePath() {
			string escapedName = RelativeManifestName.Replace('\\', '/');
			return escapedName.Substring(0, escapedName.LastIndexOf('/') + 1);
		}
		static string GetRelativeManifestName() {
			if(BrowserInteropHelper.Source == null)
				return EntryManifestName;
			try {
				string xbapFileName = BrowserInteropHelper.Source.Segments[BrowserInteropHelper.Source.Segments.Length - 1];
				StreamResourceInfo info = Application.GetRemoteStream(new Uri(xbapFileName, UriKind.Relative));
				Func<XElement, bool> predicate = delegate(XElement xElement) {
					return xElement.Name.LocalName == "dependentAssembly";
				};
				Func<XElement, string> selectCondition = delegate(XElement element) {
					return element.Attribute("codebase").Value;
				};
				using(Stream stream = info.Stream) {
					using(StreamReader reader = new StreamReader(stream)) {
						return XmlHelper.GetElements<string>(predicate, selectCondition, reader.ReadToEnd()).Single();
					}
				}
			} catch {
				return EntryManifestName;
			}
		}
#endif
		static string[] dependentAssembliesCache = null;
		public static bool IsManifestAvailable { get { return GetDependentAssemblies().Length > 0; } }
		public static string[] GetDependentAssemblies() {
			if(dependentAssembliesCache == null)
				dependentAssembliesCache = PopulateDependentAssemblies(RelativeManifestName);
			return dependentAssembliesCache;
		}
		internal static string[] GetDependentAssemblies(string manifestString) {
			return GetDependentAssemblies(manifestString, false);
		}
		internal static string[] GetDependentAssemblies(string manifestString, bool extensionOnly) {
			Func<XElement, bool> predicate = delegate(XElement xElement) {
#if SILVERLIGHT
				if(extensionOnly) {
					return xElement.Name.LocalName == "ExtensionPart";
				} else {
					return xElement.Name.LocalName == "ExtensionPart" || xElement.Name.LocalName == "AssemblyPart";
				}
#else
				return xElement.Name.LocalName == "assemblyIdentity";
#endif
			};
			Func<XElement, string> selectCondition = delegate(XElement element) {
#if SILVERLIGHT
				return element.Attribute("Source").Value;
#else
				return element.Attribute("name").Value;
#endif
			};
			try {
#if SILVERLIGHT
				IEnumerable<string> enumerable = XmlHelper.GetElements<string>(predicate, selectCondition, manifestString);
#else
				IEnumerable<string> enumerable = XmlHelper.GetDescendants<string>(predicate, selectCondition, manifestString);
#endif
				List<string> result = new List<string>();
				foreach(string asmName in enumerable) {
					if(asmName.Length > ExtensionLenth)
						result.Add(asmName.Substring(0, asmName.Length - ExtensionLenth));
				}
				return result.ToArray();
			} catch {
				return new string[0];
			}
		}
		internal static string[] PopulateDependentAssemblies(string manifestRelativeFileName) {
			StreamResourceInfo manifestStreamInfo = null;
			try {
#if SILVERLIGHT
				manifestStreamInfo = Application.GetResourceStream(new Uri(manifestRelativeFileName, UriKind.Relative));
#else
				manifestStreamInfo = Application.GetRemoteStream(new Uri(manifestRelativeFileName, UriKind.Relative));
#endif
			} catch { }
			if(manifestStreamInfo == null || manifestStreamInfo.Stream == null)
				return new string[0];
			string manifestString = manifestStreamInfo.Stream.ToStringWithDispose();
			return GetDependentAssemblies(manifestString);
		}
#if !SILVERLIGHT
		abstract class EnvironmentStrategy {
			const string ResourceString = ".resource";
			public abstract void AddAvailableAssemblies(ref List<string> assemblies);
			public virtual void Initialize() { }
			protected static void Add(IEnumerable<string> addingAssemblies, ref List<string> targetAssemblies) {
				foreach(string asmName in addingAssemblies) {
					if(!targetAssemblies.Contains(asmName)) {
						targetAssemblies.Add(asmName);
					}
				}
			}
		}
		class BrowserStrategy : EnvironmentStrategy {
			public override void AddAvailableAssemblies(ref List<string> assemblies) {
				Add(ApplicationHelper.GetDependentAssemblies(), ref assemblies);
			}
		}
		class IgnoreManifestStrategy : BrowserStrategy {
			public override void AddAvailableAssemblies(ref List<string> assemblies) { }
		}
#endif
	}
	static class Extenstions {
		internal static Queue<T> AsQueue<T>(this IEnumerable<T> collecetion) {
			return new Queue<T>(collecetion);
		}
	}
	internal static class XmlHelper {
		public static List<T> GetElements<T>(Func<XElement, bool> predicate, Func<XElement, T> selectCondition, string str) {
			List<XElement> source = System.Xml.Linq.Extensions.Elements<XElement>(XDocument.Parse(str).Root.Elements()).ToList<XElement>();
			return source.Where<XElement>(predicate).Select<XElement, T>(selectCondition).ToList<T>();
		}
		public static List<T> GetDescendants<T>(Func<XElement, bool> predicate, Func<XElement, T> selectCondition, string str) {
			List<XElement> source = System.Xml.Linq.Extensions.Elements<XElement>(XDocument.Parse(str).Root.Descendants()).ToList<XElement>();
			return source.Where<XElement>(predicate).Select<XElement, T>(selectCondition).ToList<T>();
		}
	}
#endif
	public static class StreamHelper {
		public static string ToStringWithDispose(this Stream stream) {
			using(stream) {
				using(StreamReader reader = new StreamReader(stream)) {
					return reader.ReadToEnd();
				}
			}
		}
#if !SILVERLIGHT
		public static byte[] CopyAllBytes(this Stream stream) {
			if (!stream.CanRead)
				return null;
			if (stream.CanSeek)
				stream.Seek(0L, SeekOrigin.Begin);
			List<byte> list = new List<byte>();
			byte[] buffer = new byte[1024];
			int num;
			while ((num = stream.Read(buffer, 0, 1024)) > 0) {
				for (int index = 0; index < num; ++index)
					list.Add(buffer[index]);
			}
			return list.ToArray();
		}
#endif
	}
}

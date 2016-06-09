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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.DemoData.Helpers;
#if DEMO
using DevExpress.Internal;
using DemoLauncher.Shared;
#else
using DevExpress.Utils;
#endif
using System.Data;
using System.Data.OleDb;
using System.Deployment.Application;
using System.Windows.Interop;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer;
using System.Windows;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public static class DemoHelper {
		const string SLAndXbapSysString = "#";
		const string CoSysString = "{0}/{1}.application?&";
		static readonly string CommonXamlPath = XamlReaderHelper.GetXamlPath("Themes/Common.xaml");
		static Dictionary<Assembly, DemoAssemblyInfo> demoAssemblies = new Dictionary<Assembly, DemoAssemblyInfo>();
		public static string GetSystemString(string platformLink, string clickOnceApplicationName) {
			return EnvironmentHelper.IsSL || EnvironmentHelper.IsXBAP ? SLAndXbapSysString : string.Format(CoSysString, platformLink, clickOnceApplicationName);
		}
		static DemoAssemblyInfo GetDemoAssemblyInfo(Assembly assembly) {
			lock(demoAssemblies) {
				DemoAssemblyInfo res;
				if(!demoAssemblies.TryGetValue(assembly, out res)) {
					res = new DemoAssemblyInfo();
					demoAssemblies[assembly] = res;
				}
				return res;
			}
		}
		public static Type GetStartup(Assembly demoAssembly) {
			DemoAssemblyInfo info = GetDemoAssemblyInfo(demoAssembly);
			if(info.Startup == null)
				info.Startup = GetStartupCore(demoAssembly);
			return info.Startup;
		}
		public static string GetPath(string path, Assembly assembly) {
			return GetDemoLanguage(assembly) == CodeLanguage.VB ? string.Empty : path;
		}
		public static string GetCodeSuffix(Assembly assembly) {
			return GetDemoLanguage(assembly) == CodeLanguage.VB ? ".vb" : ".cs";
		}
		public static CodeLanguage GetDemoLanguage(Assembly assembly) {
			DemoAssemblyInfo info = GetDemoAssemblyInfo(assembly);
			if(info.Language == null)
				info.Language = GetDemoLanguageCore(assembly);
			return info.Language.Value;
		}
		internal static List<CodeTextDescription> GetCodeTexts(Assembly demo, IList<string> codeFiles) {
			List<CodeTextDescription> codeTexts = new List<CodeTextDescription>();
			foreach(string codeFile in codeFiles) {
				AddCodeTextsForFile(codeTexts, demo, codeFile);
			}
			return codeTexts;
		}
		static void AddCodeTextsForFile(List<CodeTextDescription> codeTexts, Assembly demo, string codeFile) {
			foreach(CodeFileString source in CodeFileString.GetCodeFileStrings(codeFile)) {
				DefferableValue<string> codeText = new DefferableValue<string>(() => {
					string code = DemoHelper.GetCodeText(source.Prefix == "demobase" ? typeof(DemoHelper).Assembly : demo, source.FilePath);
					return code == null ? null : code.Replace("\t", "    ");
				});
				codeTexts.Add(new CodeTextDescription() { FileName = source.FileName, CodeText = codeText });
			}
		}
		public static string GetCodeText(Assembly demo, string codeFileName) {
			StringBuilder fileContent = new StringBuilder(GetText(GetCodeTextStream(demo, codeFileName)));
			RemoveAttribute(fileContent, "[CodeFile", "]");
			RemoveAttribute(fileContent, "[DevExpress.Xpf.DemoBase.CodeFile", "]");
			RemoveAttribute(fileContent, "<CodeFile", "> _");
			RemoveAttribute(fileContent, "<DevExpress.Xpf.DemoBase.CodeFile", "> _");
			return fileContent.ToString();
		}
		static void RemoveAttribute(StringBuilder destinationText, string attributeBegin, string attributeEnd) {
			while(true) {
				int indexBegin = destinationText.ToString().IndexOf(attributeBegin);
				if(indexBegin < 0) break;
				int indexEnd = destinationText.ToString().IndexOf(attributeEnd, indexBegin);
				if(indexEnd < 0) break;
				indexEnd += attributeEnd.Length;
				while(true) {
					if(indexEnd >= destinationText.Length) break;
					char c = destinationText[indexEnd];
					if(c != ' ' && c != '\n' && c != '\r' && c != '\t') break;
					++indexEnd;
				}
				destinationText.Remove(indexBegin, indexEnd - indexBegin);
			}
		}
		public static Stream GetCodeTextStream(Assembly demo, string codeFileName) {
			Stream s = GetModuleTextFromResources(demo, codeFileName);
			if(s != null) return s;
			return GetModuleTextFromFile(demo, codeFileName);
		}
		static string GetModuleCodeBaseFolder(string codeFileName) {
			return codeFileName.EndsWith(".vb") ? "VB" : "CS";
		}
		static string GetText(Stream s) {
			if(s == null) return string.Empty;
			using(StreamReader reader = new StreamReader(s))
				return reader.ReadToEnd();
		}
		static Stream GetModuleTextFromResources(Assembly assembly, string codeFileName) {
			return AssemblyHelper.GetResourceStream(assembly, codeFileName, false);
		}
		static Stream GetModuleTextFromFile(Assembly demo, string codeFileName) {
			if(EnvironmentHelper.IsClickOnce) return null;
			string baseFolderName = GetModuleCodeBaseFolder(codeFileName);
			Stream s = GetModuleTextFromFileCore(demo, codeFileName, baseFolderName);
			if(s == null)
				s = GetModuleTextFromFileCore(demo, codeFileName, Path.Combine("Reporting", baseFolderName));
			return s;
		}
		static Stream GetModuleTextFromFileCore(Assembly demo, string filename, string baseFolderName) {
			try {
				string baseDir = DataFilesHelper.FindDirectory(baseFolderName);
				string demoName = AssemblyHelper.GetPartialName(demo);
				if(demoName == "ReportWpfDemo")
					demoName = "ReportDemo.Wpf";
				string[] demoDirs = Directory.GetDirectories(baseDir, demoName + "*", SearchOption.TopDirectoryOnly);
				if(demoDirs.Length == 0) return null;
				string[] files = Directory.GetFiles(demoDirs[0], filename, SearchOption.AllDirectories);
				if(files.Length == 0) return null;
				return new FileStream(files[0], FileMode.Open, FileAccess.Read);
			} catch {
				return null;
			}
		}
		static CodeLanguage GetDemoLanguageCore(Assembly assembly) {
			const string EmbeddedDataName = "EmbeddedData.txt";
			const string EmbeddedDataPath = "Data";
			if(AssemblyHelper.GetEmbeddedResourceStream(assembly, AssemblyHelper.CombineUri(EmbeddedDataPath, EmbeddedDataName), true) != null)
				return CodeLanguage.CS;
			return CodeLanguage.VB;
		}
		static Type GetStartupCore(Assembly demoAssembly) {
			Type appType = null;
			Type[] types = demoAssembly.GetExportedTypes();
			foreach(Type type in types) {
				if(type.IsSubclassOf(typeof(StartupBase))) {
					appType = type;
					break;
				}
			}
			return appType;
		}
		public static void InitDemo(Assembly demoAssembly) {
			ResourceDictionary demoCommonXaml = GetDemoCommonXaml(demoAssembly);
			if(demoCommonXaml != null)
				Application.Current.Resources.MergedDictionaries.Add(demoCommonXaml);
			Type appType = DemoHelper.GetStartup(demoAssembly);
			MethodInfo mi = appType.GetMethod("InitDemo", BindingFlags.Static | BindingFlags.Public);
			if(mi != null)
				mi.Invoke(null, new object[] { });
		}
		static ResourceDictionary GetDemoCommonXaml(Assembly demoAssembly) {
			DemoAssemblyInfo info = GetDemoAssemblyInfo(demoAssembly);
			if(!info.CommonXamlInitialized) {
				info.CommonXaml = GetDemoCommonXamlCore(demoAssembly);
			}
			return info.CommonXaml;
		}
		static ResourceDictionary GetDemoCommonXamlCore(Assembly demoAssembly) {
			if(AssemblyHelper.GetResourceStream(demoAssembly, CommonXamlPath, true) == null)
				return null;
			return new ResourceDictionary() { Source = AssemblyHelper.GetResourceUri(demoAssembly, CommonXamlPath) };
		}
		public static void ClearDemo(Assembly demoAssembly) {
			if(demoAssembly != null) {
				Type appType = DemoHelper.GetStartup(demoAssembly);
				MethodInfo mi = appType.GetMethod("ClearDemo", BindingFlags.Static | BindingFlags.Public);
				if(mi != null)
					mi.Invoke(null, new object[] { });
			}
			ResourceDictionary demoCommonXaml = GetDemoCommonXaml(demoAssembly);
			if(demoCommonXaml != null)
				Application.Current.Resources.MergedDictionaries.Remove(demoCommonXaml);
		}
		public static string GetProductName(Assembly assembly) {
			object[] attrList = assembly.GetCustomAttributes(false);
			foreach(Attribute attr in attrList) {
				ProductIDAttribute productIDAttribute = attr as ProductIDAttribute;
				if(productIDAttribute != null)
					return productIDAttribute.ProductID;
			}
			return null;
		}
		class DemoAssemblyInfo {
			object locker = new object();
			ResourceDictionary commonXaml;
			bool commonXamlInitialized = false;
			public CodeLanguage? Language { get; set; }
			public Type Startup { get; set; }
			public bool CommonXamlInitialized { get { return commonXamlInitialized; } }
			public ResourceDictionary CommonXaml {
				get { return commonXaml; }
				set {
					lock(locker) {
						commonXaml = value;
						commonXamlInitialized = true;
					}
				}
			}
		}
	}
	class CodeFileString {
		public static IEnumerable<CodeFileString> GetCodeFileStrings(string fileName) {
			int prefixDelimiterIndex = fileName.IndexOf(':');
			string prefix;
			string path;
			if(prefixDelimiterIndex < 0) {
				prefix = string.Empty;
				path = fileName;
			} else {
				prefix = fileName.SafeRemove(prefixDelimiterIndex);
				path = fileName.SafeSubstring(prefixDelimiterIndex + 1);
			}
			path = path.Replace("(.SL)", "");
			string csPath = path.Replace("(cs)", "cs");
			if(csPath == path) {
				return new CodeFileString[] { new CodeFileString(prefix, csPath) };
			} else {
				string vbPath = path.Replace("(cs)", "vb");
				return new CodeFileString[] { new CodeFileString(prefix, csPath), new CodeFileString(prefix, vbPath) };
			}
		}
		CodeFileString(string prefix, string filePath) {
			Prefix = prefix;
			FilePath = filePath;
			FileName = filePath.Split('/').LastOrDefault();
		}
		public string Prefix { get; private set; }
		public string FilePath { get; private set; }
		public string FileName { get; private set; }
	}
}

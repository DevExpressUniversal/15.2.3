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
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using DevExpress.Utils;
namespace DevExpress.Web.Internal {
	public enum ThemeExtractMode { All, SkinFiles };
	public delegate void ThemesProviderLogMethod(string text);
	public class ThemesProvider {
		public static List<string> StandardThemes = new List<string>(
			new string[] { 
				"Aqua", "BlackGlass", "DevEx", "Glass", "iOS", "Metropolis", "MetropolisBlue", "Moderno", "Mulberry",
				"Office2003Blue", "Office2003Olive", "Office2003Silver", "Office2010Black", "Office2010Blue", "Office2010Silver", 
				"PlasticBlue", "RedWine", "SoftOrange", "Youthful" 
			}
		);
		public static List<string> NonPublicThemes = new List<string>(
			new string[] {
				"XafTheme"
			}
		);
		public static string DefaultTheme = "Default";
		public static string InitialTheme = "DevEx";
		public static string InitialMobileTheme = "Moderno";
		public static string DefaultTagPrefix = "dx";
		public static string ParametersConfigName = "Parameters.config";
		public static string DefaultSkinFileTemplate =
			"<%TAGPREFIXES%>" + Environment.NewLine +
			"<" + DefaultTagPrefix + ":<%TYPENAME%> runat=\"server\" <%COMPONENTATTRIBUTES%>>" + Environment.NewLine +
			"  <%COMPONENTCONTENT%>" + Environment.NewLine +
			"</" + DefaultTagPrefix + ":<%TYPENAME%>>";
		public static string DefaultParametersTemplate =
			"<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine +
			"<parameters>" + Environment.NewLine +
			"  <parameter Name=\"Font\" Value=\"12px Tahoma, Geneva, sans-serif\" Type=\"String\"></parameter>" + Environment.NewLine +
			"</parameters>";
		public static string NonSystemCssClassPattern = @"(\.dx[a-z]*?[A-Z]+[a-z][_A-Za-z0-9-]*)";
		public static string SystemCssClassPostfix = "Sys";
		public static string[] SystemCssClasses = new string[] { ".dxRtl", ".dxtvRtl", ".dxgvIndentCell", ".dxscViewNavigatorGotoTodayButton", ".dxmLiteReportToolbarPane" };
		public static string[] PartialSystemCssClasses = new string[] { ".dxwscDlg", ".dxheDlg" };
		public static string WholeCssClassPattern = @"(\{0}(?=([^_A-Za-z0-9-]|$)))";
		public static string[] NonSystemCssClasses = new string[] { ".dxvsRC", ".dxvsE", ".dxvsHT", ".dxvsH", ".dxvsHL", ".dxvsT", ".dxvsE", ".dxvsETC", ".dxvsL" };
		private static bool allowReplaceThemesUrls = true;
		private static List<string> cachedThemes = null;
		private static string rootThemesFolder = ResourceManager.ThemesResourcesRootFolder;
		private static bool overwriteExistingFiles = true;
		private static bool useSiteRelatedUrls = false;
		private static ThemesProviderLogMethod logMethod = null;
		private static bool extractingCanceled = false;
		private static object lockCachedThemes = new object();
		public static bool AllowReplaceThemesUrls {
			get { return allowReplaceThemesUrls;  }
		}
		public static string RootThemesFolder {
			get { return rootThemesFolder; }
			set { rootThemesFolder = value; }
		}
		public static bool OverwriteExistingFiles {
			get { return overwriteExistingFiles; }
			set { overwriteExistingFiles = value; }
		}
		public static bool UseSiteRelatedUrls {
			get { return useSiteRelatedUrls; }
			set { useSiteRelatedUrls = value; }
		}
		public static ThemesProviderLogMethod LogMethod {
			get { return logMethod; }
			set { logMethod = value; }
		}
		public static bool ExtractingCanceled {
			get { return extractingCanceled; }
		}
		static ThemesProvider() {
			LoadCustomThemeAssemblies(ConfigurationSettings.CustomThemeAssemblies);
		}
		public static bool IsThemePublic(string theme) {
			return theme != DefaultTheme;
		}
		public static List<string> GetThemes() {
			return GetThemes(HttpContext.Current != null, false);
		}
		public static List<string> GetThemes(bool publicOnly) {
			return GetThemes(HttpContext.Current != null, publicOnly);
		}
		static List<string> GetThemes(bool useCachedThemes, bool publicOnly) {
			lock (lockCachedThemes) {
				if(!useCachedThemes)
					cachedThemes = null;
				if(cachedThemes == null) {
					cachedThemes = new List<string>();
					foreach (string key in ResourceManager.ThemesEmbeddedResourcesHash.Keys) {
						EmbeddedResourceData resource = ResourceManager.ThemesEmbeddedResourcesHash[key];
						string[] nameParts = resource.ThemesRootedUrl.Split('/');
						if(nameParts.Length >= 1 && !cachedThemes.Contains(nameParts[1])) {
							if(!publicOnly || !NonPublicThemes.Contains(nameParts[1]))
								cachedThemes.Add(nameParts[1]);
						}
					}
					cachedThemes.Sort();
				}
				return cachedThemes;
			}
		}
		public static void Initialize() {
		}
		public static void LoadCustomThemeAssemblies(string assemblies) {
			if(string.IsNullOrEmpty(assemblies)) return;
			bool assembliesLoaded = false;
			string[] assembliesList = assemblies.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
			foreach(string assembly in assembliesList) {
				try {
					Assembly.Load(assembly);
					assembliesLoaded = true;
				}
				catch {
				}
			}
			if(assembliesLoaded)
				ResourceManager.FindAllWebResources();
		}
		public static void LoadCustomThemeAssemblyByPath(string filePath) {
			Assembly.LoadFile(filePath);
			ResourceManager.FindAllWebResources();
		}
		public static string GetSkinFileContent(string theme, string themeRootedFilePath) {
			string themeKey = ResourceManager.GetThemesResourceKey(theme, themeRootedFilePath);
			if(ResourceManager.ThemesEmbeddedResourcesHash.ContainsKey(themeKey))
				return ((SkinFileResourceData)ResourceManager.ThemesEmbeddedResourcesHash[themeKey]).GetContent();
			return string.Empty;
		}
		public static string GetParametersConfigContent(string theme) {
			if(theme == DefaultTheme)
				return DefaultParametersTemplate;
			else {
				string themeKey = ResourceManager.GetThemesResourceKey(theme, ParametersConfigName);
				if(ResourceManager.ThemesEmbeddedResourcesHash.ContainsKey(themeKey))
					return ((StringResourceData)ResourceManager.ThemesEmbeddedResourcesHash[themeKey]).GetContent();
				return string.Empty;
			}
		}
		public static string[] GetFolders(ThemedProduct[] products) {
			List<string> folders = new List<string>();
			foreach(ThemedProduct product in products)
				GetFoldersList(product, folders);
			return folders.ToArray();
		}
		public static void GetFoldersList(ThemedProduct product, List<string> folders) {
			for(int i = 0; i < product.Folders.Length; i++) {
				if(!folders.Contains(product.Folders[i]))
					folders.Add(product.Folders[i]);
			}
			for(int i = 0; i < product.SubProducts.Length; i++)
				GetFoldersList(product.SubProducts[i], folders);
		}
		public static string[] GetSkinFiles(ThemedProduct[] products) {
			List<string> skinFiles = new List<string>();
			foreach(ThemedProduct product in products)
				GetSkinFilesList(product, skinFiles);
			return skinFiles.ToArray();
		}
		public static void GetSkinFilesList(ThemedProduct product, List<string> skinFiles) {
			for(int i = 0; i < product.SkinFiles.Length; i++) {
				if(!skinFiles.Contains(product.SkinFiles[i]))
					skinFiles.Add(product.SkinFiles[i]);
			}
			if(product.ExtractSubProductsSkinFiles) {
				for(int i = 0; i < product.SubProducts.Length; i++)
					GetSkinFilesList(product.SubProducts[i], skinFiles);
			}
		}
		public static void ExtractThemes(string rootPath, string[] themes, ThemedProduct[] products, ThemeExtractMode mode) {
			if(!Directory.Exists(rootPath)) return;
			ClearCancelExtracting();
			foreach(string theme in themes) {
				if(ExtractingCanceled) break;
				ExtractTheme(rootPath, theme, products, mode);
			}
		}
		public static void ExtractTheme(string rootPath, string theme, ThemedProduct[] products, ThemeExtractMode mode) {
			ExtractTheme(rootPath, theme, string.Empty, products, mode);
		}
		public static void ExtractTheme(string rootPath, string theme, string newThemeName, ThemedProduct[] products, ThemeExtractMode mode) {
			if(!Directory.Exists(rootPath) || products.Length == 0) return;
			string[] skinFiles = GetSkinFiles(products);
			string[] productFolders = GetFolders(products);
			Function<string, string> replaceMethod = null;
			if(!string.IsNullOrEmpty(newThemeName)) {
				replaceMethod = delegate(string content) {
					return GetPatchedResourceContent(content, theme, newThemeName);
				};
			}
			BeginExtractTheme();
			try {
				List<string> skinFilesList = new List<string>(skinFiles);
				List<string> productFoldersList = new List<string>(productFolders);
				foreach(string key in ResourceManager.ThemesEmbeddedResourcesHash.Keys) {
					if(ExtractingCanceled) break;
					EmbeddedResourceData resource = ResourceManager.ThemesEmbeddedResourcesHash[key];
					string[] nameParts = resource.ThemesRootedUrl.Split('/');
					if(nameParts.Length >= 3 && nameParts[0] == ResourceManager.ThemesResourcesRootFolder) {
						if(theme == nameParts[1]) {
							string fileName = nameParts[nameParts.Length - 1];
							if(fileName.EndsWith(".skin") || mode == ThemeExtractMode.All) {
								string path = Path.Combine(rootPath, RootThemesFolder);
								if(CheckFolderCreated(path)) {
									bool canCreateFile = true;
									string productFolder = nameParts[2];
									for(int i = 1; i < nameParts.Length - 1; i++) {
										if(nameParts[i] == productFolder) {
											if(productFoldersList.Count > 0 && !productFoldersList.Contains(productFolder)) {
												canCreateFile = false;
												break;
											}
										}
										if(nameParts[i] == theme && !string.IsNullOrEmpty(newThemeName))
											path = Path.Combine(path, newThemeName);
										else
											path = Path.Combine(path, nameParts[i]);
										if(!CheckFolderCreated(path)) {
											canCreateFile = false;
											break;
										}
									}
									if(canCreateFile) {
										if(!fileName.EndsWith(".skin") || skinFilesList.Count == 0 || skinFilesList.Contains(fileName))
											CreateResourceFile(path + "\\" + fileName, resource, replaceMethod);
									}
								}
							}
						}
					}
				}
			}
			finally {
				EndExtractTheme();
			}
		}
		public static string GetPatchedResourceContent(string content, string theme, string newThemeName) {
			content = content.Replace("\"" + theme + "\"", "\"" + newThemeName + "\"");
			content = content.Replace("/" + theme + "/", "/" + newThemeName + "/");
			content = content.Replace("_" + theme, "_" + newThemeName);
			return content;
		}
		internal static string GetCssTextWithPostfix(string sourceCssText, string postfix) {
			string replaceFormatString = string.IsNullOrEmpty(postfix) ? "{0}" : "{0}_" + postfix;
			string result = sourceCssText;
			result = Regex.Replace(result, NonSystemCssClassPattern, 
				delegate(Match match) {
					string className = match.Value;
					bool skipAddPostFix = className.LastIndexOf(SystemCssClassPostfix) == className.Length - SystemCssClassPostfix.Length;
					if(!skipAddPostFix) {
						foreach(string markerClass in SystemCssClasses) {
							if(markerClass == className) {
								skipAddPostFix = true;
								break;
							}
						}
					}
					if(!skipAddPostFix) {
						foreach(string markerPartialClass in PartialSystemCssClasses) {
							if(className.Contains(markerPartialClass)) {
								skipAddPostFix = true;
								break;
							}
						}
					}
					return skipAddPostFix ? className : string.Format(replaceFormatString, className);
				}, RegexOptions.Singleline);
			foreach(string nonSystemCssClass in NonSystemCssClasses) {
				result = Regex.Replace(result, string.Format(WholeCssClassPattern, nonSystemCssClass),
					delegate(Match match) {
						return string.Format(replaceFormatString, match.Value);
					}, RegexOptions.Singleline);
			}
			return result;
		}
		public static void ExtractDefaultTheme(string rootPath, string newThemeName, ThemedProduct[] products, ThemeExtractMode mode) {
			if(!Directory.Exists(rootPath) || products.Length == 0) return;
			string[] skinFiles = GetSkinFiles(products);
			string[] productFolders = GetFolders(products);
			Function<string, string> replaceMethod = null;
			if(!string.IsNullOrEmpty(newThemeName)) {
				replaceMethod = delegate(string content) {
					content = GetCssTextWithPostfix(content, newThemeName);
					content = content.Replace("url('Images/", "url('");
					return content;
				};
			}
			BeginExtractTheme();
			try {
				string themePath = Path.Combine(Path.Combine(rootPath, RootThemesFolder), !string.IsNullOrEmpty(newThemeName) ? newThemeName : DefaultTheme);
				if(CheckFolderCreated(themePath)) {
					foreach(string skinFile in skinFiles) {
						if(ExtractingCanceled) break;
						CreateDefaultSkinFile(themePath, newThemeName, skinFile);
					}
					if(mode == ThemeExtractMode.All) {
						CreateDefaultParameters(themePath);
						foreach(string folder in productFolders) {
							if(ExtractingCanceled) break;
							CreateDefaultResources(themePath, replaceMethod, folder);
						}
					}
				}
			}
			finally {
				EndExtractTheme();
			}
		}
		static void BeginExtractTheme() {
			ClearCancelExtracting();
			allowReplaceThemesUrls = false;
		}
		static void EndExtractTheme() {
			allowReplaceThemesUrls = true;
		}
		public static void CancelExtracting() {
			extractingCanceled = true;
		}
		private static void ClearCancelExtracting() {
			extractingCanceled = false;
		}
		static bool CheckFolderCreated(string path) {
			if(Directory.Exists(path)) return true;
			bool failed = false;
			try {
				Directory.CreateDirectory(path);
			}
			catch {
				failed = true;
			}
			LogDirectoryEvent(path, failed);
			return !failed;
		}
		static void CreateResourceFile(string filePath, EmbeddedResourceData resource, Function<string, string> replaceMethod){
			CreateFile(filePath, delegate(Stream stream) {
				if(resource is SkinFileResourceData)
					((SkinFileResourceData)resource).WriteContent(stream, UseSiteRelatedUrls, replaceMethod);
				else if(resource is StringResourceData)
					((StringResourceData)resource).WriteContent(stream, true, replaceMethod);
				else
					resource.WriteContent(stream);
			});
		}
		static void CreateDefaultSkinFile(string themePath, string newThemeName, string skinFileName) {
			ThemedProduct product = GetProductBySkinFile(skinFileName, true);
			if(product != null){
				Assembly assembly = GetAssemblyByProduct(product);
				if(assembly != null) {
					string controlTypeName = skinFileName.Split('.')[0];
					Type controlType = GetTypeByName(assembly, controlTypeName);
					if(controlType == null)
						controlType = GetTypeByName(assembly, controlTypeName.Replace("MVCx", "ASPx")); 
					if(controlType == null) 
						controlType = GetTypeByName(assembly, controlTypeName.Replace("ASPx", "Web")); 
					if(controlType != null) {
						CreateFile(Path.Combine(themePath, skinFileName), delegate(Stream stream) {
							using(StreamWriter writer = new StreamWriter(stream)) {
								writer.Write(GetDefaultSkinFileContent(controlType, controlTypeName, skinFileName, newThemeName));
							}
						});
					}
				}
			}
		}
		static void CreateDefaultParameters(string themePath) {
			CreateFile(Path.Combine(themePath, ParametersConfigName), delegate(Stream stream) {
				using(StreamWriter writer = new StreamWriter(stream)) {
					writer.Write(DefaultParametersTemplate);
				}
			});
		}
		static void CreateDefaultResources(string themePath, Function<string, string> replaceMethod, string folder) {
			ThemedProduct product = GetProductByFolder(folder);
			if(product != null) {
				string path = Path.Combine(themePath, folder);
				if(CheckFolderCreated(path)) {
					Assembly assembly = GetAssemblyByProduct(product);
					if(assembly != null && ResourceManager.ResourcesStorages.ContainsKey(assembly)) {
						AssemblyResourcesStorage storage = ResourceManager.ResourcesStorages[assembly];
						foreach(EmbeddedResourceData resource in storage.EmbeddedResources) {
							string[] nameParts = resource.Name.Split('.');
							if(nameParts.Length >= 2) {
								if(resource.Name.Contains("Images.") ||
									(resource.Name.Contains("Css.") && !resource.Name.Contains("Frameworks.") && !resource.Name.Contains("DevExtreme."))) {
									string fileName = string.Join(".", nameParts, nameParts.Length - 2, 2);
									if(fileName.ToLower().Contains("system"))
										continue;
									else if(fileName.ToLower() == "default.css")
										fileName = "styles.css";
									if(nameParts[nameParts.Length - 3] == "GridView" && folder != "GridView") continue;
									if(nameParts[nameParts.Length - 3] == "CardView" && folder != "CardView") continue;
									if(nameParts[nameParts.Length - 3] == "Editors" && folder != "Editors") continue;
									if(nameParts[nameParts.Length - 3] != "GridView" && nameParts[nameParts.Length - 3] != "CardView" && nameParts[nameParts.Length - 3] != "Editors" &&
										(folder == "GridView" || folder == "CardView" || folder == "Editors"))
										continue;
									string filePath = Path.Combine(path, fileName);
									CreateResourceFile(filePath, resource, replaceMethod);
								}
							}
						}
					}
				}
			}
		}
		static string GetDefaultSkinFileContent(Type controlType, string skinFileName, string newThemeName) {
			return GetDefaultSkinFileContent(controlType, controlType.Name, skinFileName, newThemeName);
		}
		static string GetDefaultSkinFileContent(Type controlType, string controlTypeName, string skinFileName, string newThemeName) {
			string content = DefaultSkinFileTemplate;
			content = content.Replace("<%TAGPREFIXES%>", GetTagPrefixDirectives(skinFileName));
			content = content.Replace("<%TYPENAME%>", controlTypeName);
			string componentAttributes = string.Empty;
			string componentContent = string.Empty;
			if(!string.IsNullOrEmpty(newThemeName)) {
				if(GetProperty(controlType, "CssPostfix") != null)
					componentAttributes += "CssPostfix=\"" + newThemeName + "\" ";
				if(GetProperty(controlType, "CssFilePath") != null)
					componentAttributes += "CssFilePath=\"~/App_Themes/" + newThemeName + "/{0}/styles.css\" ";
				if(GetProperty(controlType, "SpriteCssFilePath") != null)
					componentAttributes += "SpriteCssFilePath=\"~/App_Themes/" + newThemeName + "/{0}/sprite.css\" ";
				PropertyInfo stylesProp = GetProperty(controlType, "Styles");
				if(stylesProp != null) {
					string stylesAttributes = string.Empty;
					if(GetProperty(stylesProp.PropertyType, "CssPostfix") != null)
						stylesAttributes += "CssPostfix=\"" + newThemeName + "\" ";
					if(GetProperty(stylesProp.PropertyType, "CssFilePath") != null)
						stylesAttributes += "CssFilePath=\"~/App_Themes/" + newThemeName + "/{0}/styles.css\" ";
					componentContent += "<Styles " + stylesAttributes + "/>" + Environment.NewLine;
				}
				PropertyInfo imagesProp = GetProperty(controlType, "Images");
				if(imagesProp != null) {
					string imagesAttributes = string.Empty;
					if(GetProperty(imagesProp.PropertyType, "SpriteCssFilePath") != null)
						imagesAttributes += "SpriteCssFilePath=\"~/App_Themes/" + newThemeName + "/{0}/sprite.css\" ";
					componentContent += "<Images " + imagesAttributes + "/>" + Environment.NewLine;
				}
			}
			content = content.Replace("<%COMPONENTATTRIBUTES%>", componentAttributes);
			content = content.Replace("<%COMPONENTCONTENT%>", componentContent);
			return content;
		}
		static string GetTagPrefixDirectives(string skinFileName) {
			string sampleResourceName = ("App_Themes." + InitialTheme + "." + skinFileName).ToLower();
			if(ResourceManager.ThemesEmbeddedResourcesHash.ContainsKey(sampleResourceName)) {
				SkinFileResourceData resource = (SkinFileResourceData)ResourceManager.ThemesEmbeddedResourcesHash[sampleResourceName];
				string content = ((SkinFileResourceData)resource).GetContent();
				string directives = string.Empty;
				AspxCodeUtils.SeparateContent(ref content, ref directives);
				return AspxCodeUtils.GetDirectives(directives, DefaultTagPrefix);
			}
			return string.Empty;
		}
		static PropertyInfo GetProperty(Type type, string name) {
			while(type.Name.StartsWith("MVCx"))
				type = type.BaseType; 
			PropertyInfo prop = type.GetProperty(name);
			if(prop != null) {
				object[] attrs = prop.GetCustomAttributes(typeof(DesignerSerializationVisibilityAttribute), true);
				if(attrs.Length > 0) {
					DesignerSerializationVisibilityAttribute attr = (DesignerSerializationVisibilityAttribute)attrs[0];
					if(attr.Visibility == DesignerSerializationVisibility.Hidden)
						return null;
				}
				attrs = prop.GetCustomAttributes(typeof(BrowsableAttribute), true);
				if(attrs.Length > 0) {
					BrowsableAttribute attr = (BrowsableAttribute)attrs[0];
					if(!attr.Browsable)
						return null;
				}
			}
			return prop;
		}
		internal static ThemedProduct GetProductBySkinFile(string skinFile) {
			return GetProductBySkinFile(skinFile, false);
		}
		internal static ThemedProduct GetProductBySkinFile(string skinFile, bool skipMVC) {
			ThemedProduct product = GetProductBySkinFileCore(skinFile);
			if(skipMVC && product == ThemedProducts.MVCExtensions) {
				product = GetProductBySkinFileCore(skinFile.Replace("MVCx", "ASPx"));
				if(product == null)
					product = GetProductBySkinFileCore(skinFile.Replace("MVCx", "")); 
				if(product == null)
					product = GetProductBySkinFileCore(skinFile.Replace("MVCx", "Web")); 
			}
			return product;
		}
		static ThemedProduct GetProductBySkinFileCore(string skinFile) {
			ThemedProduct[] products = ThemedProducts.GetList();
			foreach(ThemedProduct product in products) {
				List<string> skinFiles = new List<string>(product.SkinFiles);
				if(skinFiles.Contains(skinFile))
					return product;
			}
			return null;
		}
		internal static ThemedProduct GetProductByFolder(string folder) {
			ThemedProduct[] products = ThemedProducts.GetList();
			foreach(ThemedProduct product in products) {
				List<string> folders = new List<string>(product.Folders);
				if(folders.Contains(folder))
					return product;
			}
			return null;
		}
		internal static Assembly GetAssemblyByProduct(ThemedProduct product) {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly assembly in assemblies) {
				if(assembly.FullName == product.AssemblyName || assembly.FullName.StartsWith(product.AssemblyName + ","))
					return assembly;
			}
			return null;
		}
		internal static Type GetTypeByName(Assembly assembly, string typeName) {
			Type type = GetTypeByNameCore(assembly, typeName);
			if(type == null)
				type = GetTypeByNameCore(assembly, typeName.Replace("MVCx", "ASPx"));
			if(type == null)
				type = GetTypeByNameCore(assembly, typeName.Replace("MVCx", "")); 
			if(type == null)
				type = GetTypeByNameCore(assembly, typeName.Replace("MVCx", "Web")); 
			return type;
		}
		static Type GetTypeByNameCore(Assembly assembly, string typeName) {
			Type[] types = assembly.GetTypes();
			foreach(Type type in types) {
				if(type.Name == typeName)
					return type;
			}
			return null;
		}
		static void CreateFile(string filePath, Action<Stream> writeMethod) {
			bool fileExists = File.Exists(filePath);
			if(fileExists && !OverwriteExistingFiles) return;
			bool failed = false;
			Stream stream = null;
			try {
				stream = File.Open(filePath, FileMode.Create, FileAccess.Write);
				writeMethod(stream);
			}
			catch {
				failed = true;
			}
			finally {
				if(stream != null)
					stream.Close();
			}
			LogFileEvent(filePath, failed);
		}
		static void LogEvent(string path, bool failed, string action) {
			if(LogMethod == null) return;
			int pos = path.IndexOf(RootThemesFolder);
			if(pos > -1) {
				string restrictedPath = path.Substring(pos);
				LogMethod("# " + action + " ..\\" + restrictedPath + " - " + (failed ? "Failed" : "OK") + "\r\n");
			}
		}
		static void LogFileEvent(string path, bool failed) {
			LogEvent(path, failed, "Copying");
		}
		static void LogDirectoryEvent(string path, bool failed) {
			LogEvent(path, failed, "Creating");
		}
	}
}

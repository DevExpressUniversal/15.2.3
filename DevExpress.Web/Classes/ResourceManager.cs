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
using System.Web;
using System.Web.UI;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using System.ComponentModel;
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class WebResourceAssemblyAttribute : Attribute {
		private int assemblyID;
		public WebResourceAssemblyAttribute(int assemblyID) {
			this.assemblyID = assemblyID;
		}
#if !SL
	[DevExpressWebLocalizedDescription("WebResourceAssemblyAttributeAssemblyID")]
#endif
		public int AssemblyID {
			get { return assemblyID; }
		}
	}
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class ThemesAssemblyAttribute : Attribute {
		public ThemesAssemblyAttribute()
			: base() {
		}
	}
}
namespace DevExpress.Web.Internal {
	public class AssemblyResourcesStorage {
		private Assembly assembly = null;
		private int assemblyID = -1;
		private DateTime assemblyModificationUniversalDate = DateTime.MinValue;
		private List<EmbeddedResourceData> embeddedResources = new List<EmbeddedResourceData>();
		public AssemblyResourcesStorage(Assembly assembly) {
			this.assembly = assembly;
		}
		public EmbeddedResourceData this[int index] {
			get { return EmbeddedResources[index]; }
		}
		public Assembly Assembly {
			get { return assembly; }
		}
		public int AssemblyID {
			get {
				if(assemblyID == -1) {
					string assemblyName = GetAssemblyShortName();
					if(ResourceManager.KnownAssembliesIDs.ContainsKey(assemblyName))
						assemblyID = ResourceManager.KnownAssembliesIDs[assemblyName];
					else {
						object[] attributes = Assembly.GetCustomAttributes(typeof(WebResourceAssemblyAttribute), false);
						if(attributes.Length > 0)
							assemblyID = ((WebResourceAssemblyAttribute)attributes[0]).AssemblyID;
					}
				}
				return assemblyID;
			}
		}
		public DateTime AssemblyModificationUniversalDate {
			get {
				if(assemblyModificationUniversalDate == DateTime.MinValue)
					assemblyModificationUniversalDate = GetAssemblyModificationUniversalDate();
				return assemblyModificationUniversalDate;
			}
		}
		public int Count {
			get { return EmbeddedResources.Count; }
		}
		protected internal List<EmbeddedResourceData> EmbeddedResources {
			get { return embeddedResources; }
		}
		public EmbeddedResourceData AddResource(WebResourceAttribute attribute) {
			EmbeddedResourceData resource = CreateResource(attribute, EmbeddedResources.Count);
			EmbeddedResources.Add(resource);
			return resource;
		}
		protected EmbeddedResourceData CreateResource(WebResourceAttribute attribute, int index) {
			if(attribute.WebResource.EndsWith(".skin"))
				return new SkinFileResourceData(index, Assembly, attribute.WebResource, attribute.ContentType, false);
			if(attribute.ContentType.StartsWith("text"))
				return new StringResourceData(index, Assembly, attribute.WebResource, attribute.ContentType, attribute.PerformSubstitution);
			return new BinaryResourceData(index, Assembly, attribute.WebResource, attribute.ContentType);
		}
		protected DateTime GetAssemblyModificationUniversalDate() {
			return ResourceManager.GetAssemblyModificationUniversalDate(Assembly);
		}
		protected string GetAssemblyShortName() {
			return ResourceManager.GetAssemblyShortName(Assembly);
		}
		public EmbeddedResourceData FindByName(string name) {
			return EmbeddedResources.Find(delegate(EmbeddedResourceData resource) {
				return resource.Name == name;
			});
		}
	}
	public static class ResourceManager {
		public const string ResourceHandlerName = "DXR.axd";
		public const string ResourceIDsParam = "r";
		public const string ThemesResourcesRootFolder = "App_Themes";
		public const string IconsResourcesRootFolder = "Icons";
		public const string HandlerRegistrationFlag = "ASPxHttpHandlerModuleRegistered";
#if DebugTest
		private static DefaultBoolean designMode = DefaultBoolean.Default;
#endif
		private static bool assembliesLoading = false;
		private static bool themesAssemblyLoaded = false;
		private static MvcAssemblyVersion mvcAssemblyVersion = Internal.MvcAssemblyVersion.Default;
		private static Dictionary<string, int> knownAssembliesIDs = new Dictionary<string, int>();
		private static Dictionary<string, Assembly> loadedAssembliesNames = new Dictionary<string, Assembly>();
		private static List<Assembly> themesAssemblies = new List<Assembly>();
		private static Dictionary<Assembly, AssemblyResourcesStorage> resourcesStorages = new Dictionary<Assembly, AssemblyResourcesStorage>();
		private static Dictionary<int, Assembly> assembliesIDsHash = new Dictionary<int, Assembly>();
		private static Dictionary<string, Assembly> assembliesNameHash = new Dictionary<string, Assembly>();
		private static Dictionary<string, EmbeddedResourceData> embeddedResourcesHash = new Dictionary<string, EmbeddedResourceData>();
		private static Dictionary<string, EmbeddedResourceData> themesEmbeddedResourcesHash = new Dictionary<string, EmbeddedResourceData>();
		private static Dictionary<string, EmbeddedResourceData> iconsEmbeddedResourcesHash = new Dictionary<string, EmbeddedResourceData>();
		private static Dictionary<string, string> resourceUrlCache = new Dictionary<string, string>();
		private static ScriptBlocksRegistrator scriptBlocksRegistrator = new ScriptBlocksRegistrator("ScriptBlocks");
		private static ScriptResourceRegistrator scriptRegistrator = new ScriptResourceRegistrator("Script");
		private static CssResourceRegistrator cssRegistrator = new CssResourceRegistrator("Css");
		static ResourceManager() {
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyASPxThemes, 0);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyWeb, 1);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyHtmlEditorWeb, 4);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblySpellCheckerWeb, 5);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyTreeListWeb, 6);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyASPxPivotGrid, 7);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblySchedulerWeb, 8);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyReportsWeb, 9);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyChartsWeb, 10);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyASPxGauges, 11);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyPivotGridCore, 12);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyExpressAppWeb, 13);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyMVC, 14);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyDashboardWeb, 15);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyWebSpreadsheet, 16);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyMVC5, 17);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyWebRichEdit, 18);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyDashboardWebMVC, 19);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyDashboardWebMVC5, 20);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyExpressAppNotificationsWeb, 21);
			KnownAssembliesIDs.Add(AssemblyInfo.SRAssemblyExpressAppMapsWeb, 22);
			FindAllWebResources();
			ThemesProvider.Initialize();
		}
		public static Dictionary<string, int> KnownAssembliesIDs {
			get { return knownAssembliesIDs; }
		}
		public static Dictionary<string, Assembly> LoadedAssembliesNames {
			get { return loadedAssembliesNames; }
		}
		public static List<Assembly> ThemesAssemblies {
			get { return themesAssemblies; }
		}
		public static Dictionary<Assembly, AssemblyResourcesStorage> ResourcesStorages {
			get { return resourcesStorages; }
		}
		public static Dictionary<int, Assembly> AssembliesIDsHash {
			get { return assembliesIDsHash; }
		}
		public static Dictionary<string, Assembly> AssembliesNameHash {
			get { return assembliesNameHash; }
		}
		public static Dictionary<string, EmbeddedResourceData> EmbeddedResourcesHash {
			get { return embeddedResourcesHash; }
		}
		static object lockOnThemesEmbeddedResourcesHash = new Object();
		public static Dictionary<string, EmbeddedResourceData> ThemesEmbeddedResourcesHash {
			get {
				lock(lockOnThemesEmbeddedResourcesHash) {
					if(!assembliesLoading)
						EnsureThemesAssemblyLoaded();
					return themesEmbeddedResourcesHash;
				}
			}
		}
		static object lockOnIconsEmbeddedResourcesHash = new Object();
		public static Dictionary<string, EmbeddedResourceData> IconsEmbeddedResourcesHash {
			get {
				lock(lockOnIconsEmbeddedResourcesHash) {
					if(!assembliesLoading)
						EnsureThemesAssemblyLoaded();
					return iconsEmbeddedResourcesHash;
				}
			}
		}
		public static ScriptBlocksRegistrator ScriptBlocksRegistrator {
			get { return scriptBlocksRegistrator; }
		}
		public static ScriptResourceRegistrator ScriptRegistrator {
			get { return scriptRegistrator; }
		}
		public static CssResourceRegistrator CssRegistrator {
			get { return cssRegistrator; }
		}
		public static Dictionary<string, string> ResourceUrlCache {
			get { return resourceUrlCache; }
		}
		public static MvcAssemblyVersion MvcAssemblyVersion {
			get { return mvcAssemblyVersion; }
			set { mvcAssemblyVersion = value; }
		}
		public static bool DesignMode {
			get {
#if DebugTest
				if(designMode != DefaultBoolean.Default)
					return designMode == DefaultBoolean.True;
				else
#endif
					return HttpContext.Current == null;
			}
		}
#if DebugTest
		internal static void SetDesignMode(DefaultBoolean mode) {
			designMode = mode;
		}
		internal static void ClearCache() {
			ResourceUrlCache.Clear();
			ScriptRegistrator.Clear();
			CssRegistrator.Clear();
		}
#endif
		static object lockOnAssemblyNames = new Object();
		public static void FindAllWebResources() {
			lock(lockOnAssemblyNames) {
				assembliesLoading = true;
				try {
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					foreach(Assembly assembly in assemblies) {
						var assemblyName = GetAssemblyShortName(assembly);
						if(LoadedAssembliesNames.ContainsKey(assemblyName)) continue;
						FindAllWebResources(assembly);
						LoadedAssembliesNames.Add(assemblyName, assembly);
						if(assemblyName == AssemblyInfo.SRAssemblyMVC5)
							MvcAssemblyVersion = Internal.MvcAssemblyVersion.Mvc5;
					}
				}
				finally {
					assembliesLoading = false;
				}
			}
		}
		static object lockOnFindAllWebResources = new Object();
		private static void FindAllWebResources(Assembly assembly) {
			object[] webResourceAttributes;
			try {
				webResourceAttributes = assembly.GetCustomAttributes(typeof(WebResourceAttribute), false);
			} catch { return; }
			if(webResourceAttributes.Length == 0) return;
			Array.Sort<object>(webResourceAttributes, WebResourceAttributeSorter);
			if(!ResourcesStorages.ContainsKey(assembly)) {
				AssemblyResourcesStorage resourceStorage = new AssemblyResourcesStorage(assembly);
				if(resourceStorage.AssemblyID > -1) {
					ResourcesStorages.Add(assembly, resourceStorage);
					lock(lockOnFindAllWebResources) {
						if(AssembliesIDsHash.ContainsKey(resourceStorage.AssemblyID))
							throw new Exception(string.Format("It is impossible to load the {0} assembly. An assembly with the same AssemblyID {1} has already been loaded.", GetAssemblyShortName(assembly), resourceStorage.AssemblyID));
						AssembliesIDsHash.Add(resourceStorage.AssemblyID, assembly);
						AssembliesNameHash.Add(GetAssemblyShortName(assembly), assembly);
					}
					object[] themesAssemblyAttributes = assembly.GetCustomAttributes(typeof(ThemesAssemblyAttribute), false);
					if(themesAssemblyAttributes.Length > 0)
						ThemesAssemblies.Add(assembly);
					for(int j = 0; j < webResourceAttributes.Length; j++) {
						WebResourceAttribute attribute = (WebResourceAttribute)webResourceAttributes[j];
						EmbeddedResourceData resource = resourceStorage.AddResource(attribute);
						lock(lockOnFindAllWebResources) {
							EmbeddedResourcesHash.Add(resource.Key, resource);
						}
						lock(lockOnFindAllWebResources) {
							if(ThemesAssemblies.Contains(assembly)) {
								string key = GetThemesResourceKey(resource);
								if(!string.IsNullOrEmpty(key) && !ThemesEmbeddedResourcesHash.ContainsKey(key))
									ThemesEmbeddedResourcesHash.Add(key, resource);
								else {
									key = GetIconsResourceKey(resource);
									if(!string.IsNullOrEmpty(key) && !IconsEmbeddedResourcesHash.ContainsKey(key))
										IconsEmbeddedResourcesHash.Add(key, resource);
								}
							}
						}
					}
				}
			}
		}
		private static int WebResourceAttributeSorter(object x, object y) {
			WebResourceAttribute aX = x as WebResourceAttribute;
			WebResourceAttribute aY = y as WebResourceAttribute;
			return StringComparer.InvariantCulture.Compare(aX.WebResource, aY.WebResource);
		}
		internal static void EnsureAssemblyLoaded(string assemblyName) {
			if(KnownAssembliesIDs.ContainsKey(assemblyName))
				EnsureKnownAssemblyLoaded(assemblyName);
			else
				FindAllWebResources();
		}
		internal static void EnsureThemesAssemblyLoaded() {
			if(themesAssemblyLoaded) return;
			if(EnsureKnownAssemblyLoaded(0))
				themesAssemblyLoaded = true;
		}
		internal static bool EnsureKnownAssemblyLoaded(int id) {
			string assemblyName = GetKnownAssemblyNameByID(id);
			if(!string.IsNullOrEmpty(assemblyName))
				return EnsureKnownAssemblyLoaded(assemblyName);
			return false;
		}
		static object lockOnLoadAssembly = new Object();
		internal static bool EnsureKnownAssemblyLoaded(string assemblyName) {
			try {
				lock(lockOnLoadAssembly) {
					if(!LoadedAssembliesNames.ContainsKey(assemblyName)) {
						DevExpress.Data.Utils.Helpers.LoadWithPartialName(assemblyName);
						FindAllWebResources();
					}
				}
			}
			catch {
				return false;
			}
			return true;
		}
		internal static string GetKnownAssemblyNameByID(int id) {
			foreach(string assemblyName in KnownAssembliesIDs.Keys) {
				if(KnownAssembliesIDs[assemblyName] == id)
					return assemblyName;
			}
			return null;
		}
		public static string GetResourceKey(ResourceData resource, string rootFolder) {
			int pos = resource.Key.IndexOf(rootFolder);
			if(pos > -1)
				return resource.Key.Substring(pos).ToLower(System.Globalization.CultureInfo.InvariantCulture);
			return string.Empty;
		}
		public static string GetThemesResourceKey(ResourceData resource) {
			return GetResourceKey(resource, ThemesResourcesRootFolder);
		}
		public static string GetIconsResourceKey(ResourceData resource) {
			return GetResourceKey(resource, IconsResourcesRootFolder);
		}
		public static string GetThemesResourceKey(string url) {
			int pos = url.IndexOf(ThemesResourcesRootFolder);
			if(pos > -1)
				return HttpUtility.UrlDecode(url.Substring(pos).Replace("/", ".").Replace("\\", ".")).ToLower(System.Globalization.CultureInfo.InvariantCulture);
			return string.Empty;
		}
		public static string GetThemesResourceKey(string theme, string themeRootedFilePath) {
			return (ThemesResourcesRootFolder + "." + theme + "." + themeRootedFilePath).ToLower(System.Globalization.CultureInfo.InvariantCulture);
		}
		public static string GetIconsResourceKey(string url) {
			int pos = url.IndexOf(IconsResourcesRootFolder);
			if(pos > -1)
				return HttpUtility.UrlDecode(url.Substring(pos).Replace("/", ".").Replace("\\", ".")).ToLower(System.Globalization.CultureInfo.InvariantCulture);
			return string.Empty;
		}
		internal static EmbeddedResourceData FindResourceByUrl(string url) {
			if(!ConfigurationSettings.EnableThemesAssembly) return null;
			if(url.Contains(ResourceHandlerName)) return null;
			string key = GetThemesResourceKey(url);
			if(ThemesEmbeddedResourcesHash.ContainsKey(key))
				return ThemesEmbeddedResourcesHash[key];
			else {
				key = GetIconsResourceKey(url);
				if(IconsEmbeddedResourcesHash.ContainsKey(key))
					return IconsEmbeddedResourcesHash[key];
			}
			return null;
		}
		static object findResourceLockObj = new object();
		internal static EmbeddedResourceData FindResourceByName(Assembly assembly, string resourceName) {
			EmbeddedResourceData resource = FindResourceByNameCore(assembly, resourceName);
			if(resource == null) {
				lock(findResourceLockObj) {
					EnsureAssemblyLoaded(GetAssemblyShortName(assembly));
					resource = FindResourceByNameCore(assembly, resourceName);
				}
			}
			if(resource == null)
				throw new Exception("Internal error. Unable to find the '" + resourceName + "' resource in the '" + GetAssemblyShortName(assembly) + "' assembly.");
			return resource;
		}
		public static string GetResourceKey(Assembly assembly, string resourceName) {
			return GetAssemblyShortName(assembly) + "_" + resourceName;
		}
		private static EmbeddedResourceData FindResourceByNameCore(Assembly assembly, string resourceName) {
			string key = GetResourceKey(assembly, resourceName);
			if(EmbeddedResourcesHash.ContainsKey(key))
				return EmbeddedResourcesHash[key];
			return null;
		}
		public static string GetResourceID(Assembly assembly, int resourceIndex) {
			if(ResourcesStorages.ContainsKey(assembly))
				return ResourcesStorages[assembly].AssemblyID + "_" + resourceIndex;
			return "";
		}
		internal static EmbeddedResourceData FindResourceByID(string id) {
			string[] idParts = GetIdParts(id);
			if(idParts.Length == 2) {
				int assemblyID;
				if(int.TryParse(idParts[0], out assemblyID)) {
					int resourceIndex;
					if(int.TryParse(idParts[1], out resourceIndex)) {
						if(!AssembliesIDsHash.ContainsKey(assemblyID))
							EnsureKnownAssemblyLoaded(assemblyID);
						if(AssembliesIDsHash.ContainsKey(assemblyID)) {
							Assembly assembly = AssembliesIDsHash[assemblyID];
							if(0 <= resourceIndex && resourceIndex < ResourcesStorages[assembly].Count)
								return ResourcesStorages[assembly][resourceIndex];
						}
					}
				}
				else if(UsePhysicalResources)
					return FindResourceByName(idParts[0], idParts[1]);
			}
			return null;
		}
		static string[] GetIdParts(string id) {
			int index = id.IndexOf("_");
			if(index < 0)
				return new string[] {};
			List<string> result = new List<string>();
			return new string[] { id.Substring(0, index), id.Substring(index + 1) };
		}
		static EmbeddedResourceData FindResourceByName(string assemblyName, string name) {
			if(!AssembliesNameHash.ContainsKey(assemblyName))
				EnsureKnownAssemblyLoaded(assemblyName);
			if(AssembliesNameHash.ContainsKey(assemblyName)) {
				Assembly assembly = AssembliesNameHash[assemblyName];
				return ResourcesStorages[assembly].FindByName(name);
			}
			return null;
		}
		public static string ResolveClientUrl(IUrlResolutionService resolutionService, string url) {
			Page page = null;
			if(resolutionService is Page)
				page = (Page)resolutionService;
			else if(resolutionService is Control)
				page = ((Control)resolutionService).Page;
			string resourceUrl = GetResourceUrl(page, url);
			return (resolutionService != null) ? resolutionService.ResolveClientUrl(resourceUrl) : resourceUrl;
		}
		public static string GetResourceUrl(Page page, string url) {
			EmbeddedResourceData resource = FindResourceByUrl(url);
			if(resource != null)
				return GetResourceUrl(page, resource.Assembly, resource.Name);
			return url;
		}
		public static string GetResourceUrl(Page page, Type type, string resourceName) {
			return GetResourceUrl(page, type.Assembly, resourceName);
		}
		static object lockOnResourceUrlCache = new Object();
		internal static string GetResourceUrl(Page page, Assembly assembly, string resourceName) {
			string resourceKey = GetResourceKey(assembly, resourceName);
			string resourceUrl = string.Empty;
			lock(lockOnResourceUrlCache) {
				if(ResourceUrlCache.ContainsKey(resourceKey))
					resourceUrl = ResourceUrlCache[resourceKey];
				else {
					resourceUrl = GetResourceUrlInternal(page, assembly, resourceName);
					if(!DesignMode)
						ResourceUrlCache.Add(resourceKey, resourceUrl);
				}
			}
			return resourceUrl;
		}
		private static string GetResourceUrlInternal(Page page, Assembly assembly, string resourceName) {
			string resourceUrl = string.Empty;
			if(DesignMode) {
				Type[] types = assembly.GetTypes();
				Type type = (types.Length > 0) ? types[0] : null;
				resourceUrl = (page != null && type != null) ? page.ClientScript.GetWebResourceUrl(type, resourceName) : string.Empty;
			} else {
				EmbeddedResourceData resource = ResourceManager.FindResourceByName(assembly, resourceName);
				if(resource != null)
					resourceUrl = GetResourcesUrlString(page, new EmbeddedResourceData[] { resource });
			}
			return resourceUrl;
		}
		[SecuritySafeCritical]
		public static DateTime GetAssemblyModificationUniversalDate(Assembly assembly) {
			DateTime date = assembly.GlobalAssemblyCache
				? GetAssemblyLastWriteTimeGac(assembly)
				: GetAssemblyLastWriteTime(assembly);
			if(date > DateTime.UtcNow) date = DateTime.UtcNow;
			return date;
		}
		private static DateTime GetAssemblyLastWriteTime(Assembly assembly) {
			return File.GetLastWriteTime(new Uri(assembly.EscapedCodeBase).LocalPath).ToUniversalTime();
		}
		[SecurityCritical, FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		private static DateTime GetAssemblyLastWriteTimeGac(Assembly assembly) {
			return GetAssemblyLastWriteTime(assembly);
		}
		public static string GetAssemblyShortName(Assembly assembly) {
			string[] nameParts = assembly.FullName.Split(',');
			if(nameParts.Length > 0)
				return nameParts[0].Trim();
			else
				return assembly.FullName;
		}
		public static void RegisterScriptBlock(Page page, string name, string script) {
			RegisterScriptBlock(page, name, script, false);
		}
		public static void RegisterScriptBlock(Page page, string name, string script, bool useStandardRegistration) {
			ScriptBlocksRegistrator.RegisterScriptBlock(page, name, script, useStandardRegistration);
		}
		public static void RenderScriptBlocks(Page page, HtmlTextWriter writer) {
			ScriptBlocksRegistrator.RenderScriptBlocks(page, writer);
		}
		public static void RegisterScriptResource(Page page, Type type, string resourceName) {
			RegisterScriptResource(page, type, resourceName, false);
		}
		public static void RegisterScriptResource(Page page, Type type, string resourceName, bool useStandardRegistration) {
			ScriptRegistrator.RegisterResource(page, type.Assembly, resourceName, useStandardRegistration);
		}
		public static void RenderScriptResources(Page page, HtmlTextWriter writer) {
			ScriptRegistrator.RenderResources(page, writer);
		}
		public static void RegisterCssResource(Page page, Type type, string resourceName) {
			CssRegistrator.RegisterResource(page, type.Assembly, resourceName, false);
		}
		public static void RegisterCssResource(Page page, string resourceUrl) {
			CssRegistrator.RegisterResource(page, resourceUrl, false);
		}
		public static void RenderCssResources(Page page, HtmlTextWriter writer) {
			CssRegistrator.RenderResources(page, writer);
		}
		public static void RenderCssResources(Page page, HtmlTextWriter writer, bool renderAsStyleImports) {
			CssRegistrator.RenderResources(page, writer, renderAsStyleImports);
		}
		public static void CreateControlForCssResourcesInHeader(Page page) {
			CssRegistrator.CreateControlForResourcesInHeader(page);
		}
		public static void RenderCssResourcesInHeader(Page page) {
			CssRegistrator.RenderResourcesInHeader(page);
		}
		public static string GetCssResourcesDesignHtml(Page page, bool renderAsStyleImports) {
			return CssRegistrator.GetResourcesDesignHtml(page, renderAsStyleImports);
		}
		internal static List<ResourceData> GetResourcesByListString(string list, bool onlyEmbeddedResources) {
			List<ResourceData> result = new List<ResourceData>();
			if(!string.IsNullOrEmpty(list)) {
				string[] ids = list.Split(',');
				for(int i = 0; i < ids.Length; i++) {
					EmbeddedResourceData resource = FindResourceByID(ids[i]);
					if(resource != null)
						result.Add(resource);
					else if(!onlyEmbeddedResources)
						result.Add(new FileResourceData(ids[i]));
				}
			}
			return result;
		}
		public static bool UsePhysicalResources { get { return !string.IsNullOrEmpty(ResourcesPhysicalPath); } }
		public static string ResourcesPhysicalPath { get { return ConfigurationSettings.ResourcesPhysicalPath; } }
		internal static string GetResourcesListString(IEnumerable<EmbeddedResourceData> resources) {
			string list = string.Empty;
			foreach(EmbeddedResourceData resource in resources)
				list += (UsePhysicalResources ? resource.Key : resource.ID) + ",";
			return list.Remove(list.Length - 1);
		}
		internal static string GetResourcesUrlString(Page page, IEnumerable<EmbeddedResourceData> resources) {
			string url = HttpUtils.GetApplicationUrl(page);
			url += ResourceHandlerName + "?" + ResourceIDsParam + "=";
			url += GetResourcesListString(resources);
			url += "-" + GetDateToken(GetResourcesModificationUniversalDate(resources));
			return url;
		}
		private static bool IsResourcesBinary(IEnumerable<EmbeddedResourceData> resources) {
			foreach(EmbeddedResourceData resource in resources) {
				if(resource is BinaryResourceData)
					return true;
			}
			return false;
		}
		private static string GetResourcesContentType(IEnumerable<EmbeddedResourceData> resources) {
			string contentType = string.Empty;
			foreach(EmbeddedResourceData resource in resources) {
				if(string.IsNullOrEmpty(contentType))
					contentType = resource.ContentType;
				else {
					if(contentType != resource.ContentType)
						throw new Exception("Internal error. There are different content types in a one request.");
				}
			}
			return contentType;
		}
		static Dictionary<DateTime, string> dateTokens = new Dictionary<DateTime, string>();
		static readonly object dateTokensLock = new object();
		public static string GetDateToken(DateTime date) {
			lock (dateTokensLock) {
				if(date.Year >= 2009)
					date = date.AddYears(-2009);
				if (!dateTokens.ContainsKey(date)) {
					long seconds = date.Ticks / TimeSpan.TicksPerSecond;
					StringBuilder builder = new StringBuilder(7);
					while (seconds > 0) {
						long digit = seconds % 63;
						seconds /= 63;
						builder.Append(GetBase64Digit(digit));
					}
					dateTokens.Add(date, builder.ToString());
				}
				return dateTokens[date];
			}
		}
		static char GetBase64Digit(long digit) {
			if(digit < 10)
				return (char)((int)'0' + digit);
			if(digit < 36)
				return (char)((int)'a' + digit - 10);
			if(digit < 62)
				return (char)((int)'A' + digit - 36);
			if(digit == 62)
				return '_';
			throw new ArgumentOutOfRangeException();
		}
		private static DateTime GetResourcesModificationUniversalDate(IEnumerable<EmbeddedResourceData> resources) {
			DateTime date = DateTime.MinValue;
			foreach(EmbeddedResourceData resource in resources) {
				DateTime resourceDate = DateTime.MinValue;
				if(ResourcesStorages.ContainsKey(resource.Assembly))
					resourceDate = ResourcesStorages[resource.Assembly].AssemblyModificationUniversalDate;
				if(resourceDate > date)
					date = resourceDate;
			}
			return date;
		}
		private static List<EmbeddedResourceData> GetRequestedResources() {
			string s = HttpUtils.GetRequest().QueryString[ResourceIDsParam];
			if(!string.IsNullOrEmpty(s) && s.Contains("-"))
				s = s.Substring(0, s.LastIndexOf("-")); 
			List<EmbeddedResourceData> result = new List<EmbeddedResourceData>();
			List<ResourceData> allResources = GetResourcesByListString(s, true);
			foreach(ResourceData resource in allResources) {
				if(resource is SkinFileResourceData) continue;
				result.Add((EmbeddedResourceData)resource);
			}
			return result;
		}
		private static void MakeStatus304(HttpResponse response) {
			response.StatusCode = 304;
			response.AddHeader("content-length", "0");
			HttpUtils.EndResponse();
		}
		internal static bool ProcessRequest() {
			HttpRequest request = HttpUtils.GetRequest();
			HttpResponse response = HttpUtils.GetResponse();
			List<EmbeddedResourceData> requestedResources = GetRequestedResources();
			if(requestedResources.Count == 0) {
				MakeStatus304(response);
				return true;
			}
			DateTime modificationDate = GetResourcesModificationUniversalDate(requestedResources);
			string cacheDateStr = request.Headers["If-Modified-Since"];
			if(!UsePhysicalResources && modificationDate.ToString("R") == cacheDateStr) {
				response.ContentType = GetResourcesContentType(requestedResources);
				MakeStatus304(response);
				return true;
			}
			response.Clear();
			try {
				foreach(EmbeddedResourceData resource in requestedResources)
					resource.WriteContent(response.OutputStream);
				response.Cache.SetCacheability(HttpCacheability.Public);
				if(!UsePhysicalResources) {
					response.Cache.SetLastModified(modificationDate);
					response.Cache.SetExpires(modificationDate.AddDays(365));
					response.Cache.SetMaxAge(new TimeSpan(365, 0, 0, 0));
				}
				else
					response.Cache.SetExpires(DateTime.Now.AddYears(-1));
				response.ContentType = GetResourcesContentType(requestedResources);
				if(ConfigurationSettings.EnableResourceCompression && !IsResourcesBinary(requestedResources))
					HttpUtils.MakeResponseCompressed(true);
			} 
			catch(Exception e) {
				response.ClearHeaders();
				response.ClearContent();
				response.StatusCode = 500;
				response.StatusDescription = e.Message;
			}
			HttpUtils.EndResponse();
			return true;
		}
		public static void ClearRegisteredCssResources() {
			CssRegistrator.RegisteredResources.Clear();
		}
		public static void ClearCssResources() {
			CssRegistrator.Clear();
		}
		public static void ClearScriptResources() {
			ScriptRegistrator.Clear();
		}
		static Dictionary<string, Function<string, string>> customWebResourceHandlers = new Dictionary<string, Function<string, string>>();
		internal static string GetCustomWebResource(string resourceName) {
			if(resourceName.Contains(":")) {
				string[] parts = resourceName.Split(':');
				return GetCustomWebResource(parts[0], parts[1]);
			}
			return GetCustomWebResource(resourceName, "");
		}
		internal static string GetCustomWebResource(string key, string resourceName) {
			lock(customWebResourceHandlers) {
				if(customWebResourceHandlers.ContainsKey(key))
					return customWebResourceHandlers[key](resourceName);
			}
			return "";
		}
		public static void RegisterCustomWebResourceHandler(string key, Function<string, string> handler) {
			lock(customWebResourceHandlers) {
				if(!customWebResourceHandlers.ContainsKey(key))
					customWebResourceHandlers.Add(key, handler);
			}
		}
	}
	public class ResourceRegistrator {
		private string name;
		private Dictionary<string, ResourceData> registeredResources = new Dictionary<string, ResourceData>();
		private List<ResourceData> resourcesForRender = new List<ResourceData>();
		private List<ResourceData> mvcWayRegisteredResourcesForRender = new List<ResourceData>();
		private bool registeredResourcesSynchronized;
		public ResourceRegistrator(string name) {
			this.name = name;
		}
		public string Name {
			get { return name; }
		}
		public string ResourceTypeClientName {
			get { return "DX" + Name; }
		}
		public Dictionary<string, ResourceData> RegisteredResources {
			get {
				if(HttpContext.Current == null)
					return registeredResources;
				return HttpUtils.GetContextObject<Dictionary<string, ResourceData>>(Name + "RegisteredResources");
			}
		}
		public List<ResourceData> ResourcesForRender {
			get {
				if(HttpContext.Current == null)
					return resourcesForRender;
				return HttpUtils.GetContextObject<List<ResourceData>>(Name + "ResourcesForRender");
			}
		}
		List<ResourceData> MvcWayRegisteredResourcesForRender {
			get {
				if(HttpContext.Current == null)
					return mvcWayRegisteredResourcesForRender;
				return HttpUtils.GetContextObject<List<ResourceData>>(Name + "MvcWayRegisteredResourcesForRender");
			}
		}
		public bool RegisteredResourcesSynchronized {
			get {
				if(HttpContext.Current == null)
					return registeredResourcesSynchronized;
				return HttpUtils.GetContextValue<bool>(Name + "RegisteredResourcesSynchronized", false);
			}
			set {
				if(HttpContext.Current == null)
					registeredResourcesSynchronized = value;
				else
					HttpUtils.SetContextValue(Name + "RegisteredResourcesSynchronized", value);
			}
		}
		protected void SynchronizeResources(Page page) {
			HttpRequest request = HttpUtils.GetRequest(page);
			if(request != null) {
				string list = GetResourcesListByRequest(request);
				List<ResourceData> clientLoadedResources = ResourceManager.GetResourcesByListString(list, false);
				foreach(ResourceData resource in clientLoadedResources) {
					if(!RegisteredResources.ContainsKey(resource.Key))
						RegisteredResources.Add(resource.Key, resource);
				}
			}
		}
		protected string GetResourcesListByRequest(HttpRequest request) {
			string headerResourcesList = null;
			if(MvcUtils.RenderMode != MvcRenderMode.None)
				headerResourcesList = request.Headers[ResourceTypeClientName];
			return headerResourcesList ?? request.Params[ResourceTypeClientName];
		}
		public void EnsureResourcesSynchronized(Page page) {
			if(!RegisteredResourcesSynchronized && !ResourceManager.DesignMode && RenderUtils.IsAnyCallback(page)) {
				SynchronizeResources(page);
				RegisteredResourcesSynchronized = true;
			}
		}
		public void RegisterResource(Page page, string url, bool useStandardRegistration) {
			EmbeddedResourceData resource = ResourceManager.FindResourceByUrl(url);
			if(resource != null)
				RegisterResource(page, resource.Assembly, resource.Name, useStandardRegistration);
			else
				RegisterResource(page, new FileResourceData(url), useStandardRegistration);
		}
		public void RegisterResource(Page page, Assembly assembly, string resourceName, bool useStandardRegistration) {
			EmbeddedResourceData resource = ResourceManager.FindResourceByName(assembly, resourceName);
			if(resource != null)
				RegisterResource(page, resource, useStandardRegistration);
		}
		public void RegisterResource(Page page, ResourceData resource, bool useStandardRegistration) {
			EnsureResourcesSynchronized(page);
			if(RenderUtils.IsAnyCallback(page))
				useStandardRegistration = false;
			if(!RegisteredResources.ContainsKey(resource.Key)) {
				RegisteredResources.Add(resource.Key, resource);
				if(!useStandardRegistration) {
					ResourcesForRender.Add(resource);
				} else
					PerformStandardRegistration(page, resource);
			} else if(useStandardRegistration && ResourcesForRender.Contains(resource)) {
				ResourcesForRender.Remove(resource);
				PerformStandardRegistration(page, resource);
			}
			if(MvcUtils.RenderMode == MvcRenderMode.RenderResources && !MvcWayRegisteredResourcesForRender.Contains(resource))
				MvcWayRegisteredResourcesForRender.Add(resource);
		}
		public void RenderResources(Page page, HtmlTextWriter writer) {
			List<ResourceData> resourcesForRender = GetResourcesForRender();
			if(resourcesForRender.Count > 0) {
				writer.Write(GetResourcesHtml(page, resourcesForRender));
				ResourcesForRender.Clear();
				MvcWayRegisteredResourcesForRender.Clear();
			}
		}
		public void Clear() {
			RegisteredResources.Clear();
			ResourcesForRender.Clear();
			MvcWayRegisteredResourcesForRender.Clear();
		}
		protected List<ResourceData> GetResourcesForRender() {
			if(MvcWayRegisteredResourcesForRender.Count == 0)
				return ResourcesForRender;
			List<ResourceData> resourcesForRender = new List<ResourceData>(MvcWayRegisteredResourcesForRender);
			foreach(ResourceData resource in ResourcesForRender) {
				if(!resourcesForRender.Contains(resource))
					resourcesForRender.Add(resource);
			}
			return resourcesForRender;
		}
		protected string GetResourcesHtml(Page page, List<ResourceData> resources) {
			string fileResourcesHtml = string.Empty, embeddedResourcesHtml = string.Empty;
			List<List<EmbeddedResourceData>> mergedResourcesLists = new List<List<EmbeddedResourceData>>();
			List<EmbeddedResourceData> mergedResourcesList = new List<EmbeddedResourceData>();
			mergedResourcesLists.Add(mergedResourcesList);
			foreach(ResourceData resource in resources) {
				if(resource is FileResourceData)
					fileResourcesHtml += GetResourceHtml(((FileResourceData)resource).Url);
				else if(resource is EmbeddedResourceData) {
					if(ConfigurationSettings.EnableResourceMerging && !ResourceManager.DesignMode) {
						if(resource is StringResourceData) {
							if(HasContentLengthRestriction() && GetStringResourcesSize(mergedResourcesList, (EmbeddedResourceData)resource) > 280000) {
								mergedResourcesList = new List<EmbeddedResourceData>();
								mergedResourcesLists.Add(mergedResourcesList);
							}
							else if(HasUrlLengthRestriction() && GetResourcesUrlLength(page, mergedResourcesList, (EmbeddedResourceData)resource) > 2000) {
								mergedResourcesList = new List<EmbeddedResourceData>();
								mergedResourcesLists.Add(mergedResourcesList);
							}
						}
						mergedResourcesList.Add((EmbeddedResourceData)resource);
					}
					else
						embeddedResourcesHtml += GetResourceHtml(ResourceManager.GetResourceUrl(page, ((EmbeddedResourceData)resource).Assembly, ((EmbeddedResourceData)resource).Name));
				}
			}
			foreach(List<EmbeddedResourceData> list in mergedResourcesLists) {
				if(list.Count > 0)
					embeddedResourcesHtml += GetResourceHtml(ResourceManager.GetResourcesUrlString(page, list));
			}
			return embeddedResourcesHtml + fileResourcesHtml;
		}
		protected int GetStringResourcesSize(List<EmbeddedResourceData> list, EmbeddedResourceData additionalResource) {
			int size = 0;
			foreach(EmbeddedResourceData resource in list) {
				if(resource is StringResourceData)
					size += ((StringResourceData)resource).GetContentLength();
			}
			if(additionalResource is StringResourceData)
				size += ((StringResourceData)additionalResource).GetContentLength();
			return size;
		}
		protected int GetResourcesUrlLength(Page page, List<EmbeddedResourceData> list, EmbeddedResourceData additionalResource) {
			List<EmbeddedResourceData> resourcesList = new List<EmbeddedResourceData>(list);
			resourcesList.Add(additionalResource);
			return ResourceManager.GetResourcesUrlString(page, resourcesList).Length;
		}
		protected virtual bool HasContentLengthRestriction() {
			return false;
		}
		protected virtual bool HasUrlLengthRestriction() {
			return true;
		}
		protected virtual string GetResourceHtml(string resourceUrl) {
			return string.Empty;
		}
		protected virtual void PerformStandardRegistration(Page page, ResourceData resource) {
		}
	}
	public class ScriptResourceRegistrator : ResourceRegistrator {
		public ScriptResourceRegistrator(string name)
			: base(name) {
		}
		protected override string GetResourceHtml(string resourceUrl) {
			return !string.IsNullOrEmpty(resourceUrl) ? RenderUtils.GetIncludeScriptHtml(resourceUrl) : string.Empty;
		}
		protected override void PerformStandardRegistration(Page page, ResourceData resource) {
			if(page == null) return;
			List<ResourceData> resources = new List<ResourceData>();
			resources.Add(resource);
			page.ClientScript.RegisterClientScriptBlock(typeof(Page), resource.Key, GetResourcesHtml(page, resources));
		}
	}
	public class CssResourceRegistrator : ResourceRegistrator {
		private bool renderAsStyleImports = false;
		private readonly object lockCreateResourcesLink = new object();
		public CssResourceRegistrator(string name)
			: base(name) {
		}
		public void RenderResources(Page page, HtmlTextWriter writer, bool renderAsStyleImports) {
			this.renderAsStyleImports = renderAsStyleImports;
			try {
				RenderResources(page, writer);
			} finally {
				this.renderAsStyleImports = false;
			}
		}
		public string GetResourcesDesignHtml(Page page, bool renderAsStyleImports) {
			string html = string.Empty;
			this.renderAsStyleImports = renderAsStyleImports;
			try {
				if(ResourcesForRender.Count > 0) {
					html = GetResourcesHtml(page, ResourcesForRender);
					ResourcesForRender.Clear();
				}
			} finally {
				this.renderAsStyleImports = false;
			}
			return html;
		}
		protected override string GetResourceHtml(string resourceUrl) {
			if(!string.IsNullOrEmpty(resourceUrl)) {
				if(this.renderAsStyleImports)
					return RenderUtils.GetStyleImportHtml(resourceUrl);
				else
					return RenderUtils.GetLinkHtml(resourceUrl);
			}
			return string.Empty;
		}
		protected override bool HasContentLengthRestriction() {
			return RenderUtils.Browser.IsIE;
		}
		public LiteralControl CreateControlForResourcesInHeader(Page page) {
			lock(lockCreateResourcesLink) {
				LiteralControl control = page.Header.FindControl(RenderUtils.StyleSheetsContainerControlID) as LiteralControl;
				if(control == null) {
					control = RenderUtils.CreateLiteralControl();
					control.ID = RenderUtils.StyleSheetsContainerControlID;
					control.EnableViewState = false;
					Control meta = page.Header.FindControl(RenderUtils.IECompatibilityMetaID);
					if(meta == null)
						meta = page.Header.FindControl(RenderUtils.IECompatibilityMetaObsoleteID);
					int index = (meta != null) ? page.Header.Controls.IndexOf(meta) + 1 : 0;
					page.Header.Controls.AddAt(index, control);
				}
				return control;
			}
		}
		public void RenderResourcesInHeader(Page page) {
			if(ResourcesForRender.Count > 0) {
				LiteralControl referenceControl = CreateControlForResourcesInHeader(page);
				referenceControl.Text += GetResourcesHtml(page, ResourcesForRender);
				ResourcesForRender.Clear();
			}
		}
	}
	public class ScriptBlocksRegistrator {
		private string name;
		private Dictionary<string, string> registeredScriptBlocks = new Dictionary<string, string>();
		private List<string> scriptBlocksForRender = new List<string>();
		public ScriptBlocksRegistrator(string name) {
			this.name = name;
		}
		public string Name {
			get { return name; }
		}
		public Dictionary<string, string> RegisteredScriptBlocks {
			get {
				if(HttpContext.Current == null)
					return registeredScriptBlocks;
				return HttpUtils.GetContextObject<Dictionary<string, string>>(Name + "RegisteredScriptBlocks");
			}
		}
		public List<string> ScriptBlocksForRender {
			get {
				if(HttpContext.Current == null)
					return scriptBlocksForRender;
				return HttpUtils.GetContextObject<List<string>>(Name + "ScriptBlocksForRender");
			}
		}
		public void RegisterScriptBlock(Page page, string name, string script, bool useStandardRegistration) {
			if(!RegisteredScriptBlocks.ContainsKey(name)) {
				RegisteredScriptBlocks.Add(name, script);
				if(!useStandardRegistration) {
					ScriptBlocksForRender.Add(script);
				}
				else
					PerformStandardRegistration(page, name, script);
			}
			else if(useStandardRegistration && ScriptBlocksForRender.Contains(script)) {
				ScriptBlocksForRender.Remove(script);
				PerformStandardRegistration(page, name, script);
			} 
		}
		public void RenderScriptBlocks(Page page, HtmlTextWriter writer) {
			if(ScriptBlocksForRender.Count > 0) {
				writer.Write(GetBlocksHtml(page, ScriptBlocksForRender));
				ScriptBlocksForRender.Clear();
			}
		}
		protected string GetBlocksHtml(Page page, List<string> blocks) {
			string html = string.Empty;
			foreach(string block in blocks) 
				html += block;
			return html;
		}
		protected void PerformStandardRegistration(Page page, string name, string script) {
			if(page != null)
				page.ClientScript.RegisterClientScriptBlock(typeof(Page), name, script);
		}
	}
}

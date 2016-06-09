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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Xml;
using DevExpress.Utils;
using System.Security;
namespace DevExpress.Web.Internal {
	public static class ThemesHelper {
		private static ControlSerializer 
			controlSerializer = null;
		static ThemesHelper() {
			controlSerializer = new ControlSerializer();
		}
		public static void ApplyTheme(Control control, string themeName, bool designMode) {
			if (!IsThemeExist(themeName)) {
				if(!designMode)
					throw new ArgumentException(string.Format("Cannot find the '{0}' theme.", themeName), ThemableControlBuilder.ThemeAttributeName);
				return;
			}
			SkinFileParser.ApplySkinToControl(control,
				delegate() { return GetSkinControl(control, themeName); },
				delegate() { return GetSkinFileContent(control.GetType(), themeName); },
				themeName
			);
		}
		public static void ClearProperties(object destination, bool needClearCannotBeEmptyProperty) {
			PropertyDescriptorCollection desinationProperties = TypeDescriptor.GetProperties(destination);
			foreach(PropertyDescriptor desinationProperty in desinationProperties) {
				if(IsClearableProperty(desinationProperty)) {
					if(IsCollectionProperty(desinationProperty))
						(desinationProperty.GetValue(destination) as IAssignableCollection).Clear();
					else if(IsObjectProperty(desinationProperty))
						ClearProperties(desinationProperty.GetValue(destination), needClearCannotBeEmptyProperty);
					else if(IsCannotBeEmptyProperty(desinationProperty)) {
						if(needClearCannotBeEmptyProperty)
							desinationProperty.ResetValue(destination);
					}
					else if(IsTemplateProperty(desinationProperty)) {
					}
					else {
						if(!desinationProperty.IsReadOnly && (desinationProperty.ShouldSerializeValue(destination)))
							desinationProperty.ResetValue(destination);
						if(destination is FontInfo)
							(destination as FontInfo).ClearDefaults();
					}
				}
			}
		}
		public static void CopyProperties(object source, object destination) {
			PropertyInfo[] properties = source.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
				CopyProperty(source, destination, property.Name);
		}
		public static void CopyProperty(object source, object destination, string propertyName) {
			PropertyDescriptor sourceProperty = TypeDescriptor.GetProperties(source)[propertyName];
			PropertyDescriptor destinationProperty = TypeDescriptor.GetProperties(destination)[propertyName];
			if (destinationProperty != null && IsSerializableProperty(destinationProperty)) {
				if (IsCollectionProperty(destinationProperty))
					(destinationProperty.GetValue(destination) as IAssignableCollection).Assign(sourceProperty.GetValue(source) as IAssignableCollection);
				else if (IsObjectProperty(destinationProperty))
					CopyProperties(sourceProperty.GetValue(source), destinationProperty.GetValue(destination));
				else if (IsTemplateProperty(destinationProperty)) {
					object sourceValue = sourceProperty.GetValue(source);
					if (!destinationProperty.IsReadOnly && sourceValue != null)
						destinationProperty.SetValue(destination, sourceValue);
				} else if (IsCannotBeEmptyProperty(destinationProperty)) {
					object sourcePropertyValue = sourceProperty.GetValue(source);
					if (sourcePropertyValue is Unit) {
						Unit unitSourceProperty = (Unit)sourcePropertyValue;
						if (!unitSourceProperty.IsEmpty)
							destinationProperty.SetValue(destination, sourcePropertyValue);
					} else
						destinationProperty.SetValue(destination, sourcePropertyValue);
				} else {
					bool isFontInfoPropertySet = false;
					bool isFontInfo = source is FontInfo;
					if (isFontInfo)
						isFontInfoPropertySet = IsFontInfoPropertySetInAutoFormat(source as FontInfo, sourceProperty);
					object sourceValue = sourceProperty.GetValue(source);
					DefaultValueAttribute defaultValueAttribute = sourceProperty.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
					if (!isFontInfo && defaultValueAttribute != null &&
						object.Equals(defaultValueAttribute.Value, sourceValue))
						destinationProperty.ResetValue(destination);
					if (!destinationProperty.IsReadOnly && (isFontInfoPropertySet || sourceProperty.ShouldSerializeValue(source)))
						destinationProperty.SetValue(destination, sourceValue);
				}
			}
		}
		private static Control GetSkinControl(Control control, string themeName) {
			if(string.Equals(themeName, ThemesProvider.DefaultTheme))
				return Activator.CreateInstance(control.GetType()) as Control;
			string skinFileContent = GetSkinFileContent(control.GetType(), themeName);
			if(!string.IsNullOrEmpty(skinFileContent)) {
				Control[] skinControls = null;
				try {
					skinControls = controlSerializer.GetControls(skinFileContent);
				}
				catch {
				}
				if(skinControls != null && skinControls.Length > 0)
					return GetTargetSkinControl(control, new List<Control>(skinControls));
			}
			return null;
		}
		private static Control GetTargetSkinControl(Control control, List<Control> skinControls) {
			skinControls.RemoveAll(delegate(Control ctl) { 
				return !string.Equals(ctl.GetType().FullName, control.GetType().FullName); 
			});
			foreach(Control skinControl in skinControls) {
				if(skinControl.SkinID == control.SkinID)
					return skinControl;
			}
			return skinControls[0];
		}
		private static string GetSkinFileContent(Type controlType, string themeName) {
			string fileName = string.Format("{0}.skin", controlType.Name);
			return ThemesProvider.GetSkinFileContent(themeName, fileName);
		}
		private static bool IsClearableProperty(PropertyDescriptor property) {
			AutoFormatDisableClearAttribute attribute =
				property.Attributes[typeof(AutoFormatDisableClearAttribute)] as AutoFormatDisableClearAttribute;
			return IsSerializableProperty(property) && (attribute == null);
		}
		private static bool IsCannotBeEmptyProperty(PropertyDescriptor property) {
			AutoFormatCannotBeEmptyAttribute attribute =
				property.Attributes[typeof(AutoFormatCannotBeEmptyAttribute)] as AutoFormatCannotBeEmptyAttribute;
			return attribute != null;
		}
		private static bool IsSerializableProperty(PropertyDescriptor property) {
			AutoFormatDisableAttribute attribute =
				property.Attributes[typeof(AutoFormatDisableAttribute)] as AutoFormatDisableAttribute;
			return (!SkinFileParser.ExcludePropertyNames.Contains(property.Name) && property.SerializationVisibility != DesignerSerializationVisibility.Hidden);
		}
		private static bool IsCollectionProperty(PropertyDescriptor property) {
			return IsObjectProperty(property) && typeof(IAssignableCollection).IsAssignableFrom(property.PropertyType);
		}
		private static bool IsObjectProperty(PropertyDescriptor property) {
			if(IsTemplateProperty(property)) return false;
			PersistenceModeAttribute persistenceModeAttribute = property.Attributes[typeof(PersistenceModeAttribute)] as PersistenceModeAttribute;
			return (property.SerializationVisibility == DesignerSerializationVisibility.Content) ||
				(persistenceModeAttribute != null &&
				(persistenceModeAttribute.Mode == PersistenceMode.InnerProperty ||
				persistenceModeAttribute.Mode == PersistenceMode.InnerDefaultProperty ||
				persistenceModeAttribute.Mode == PersistenceMode.EncodedInnerDefaultProperty));
		}
		private static bool IsTemplateProperty(PropertyDescriptor property) {
			return property.PropertyType == typeof(ITemplate);
		}
		private static bool IsThemeExist(string themeName) {
			bool isThemeExist = ThemesProvider.GetThemes().FindIndex(
				delegate(string theme) {
					return theme.Equals(themeName, StringComparison.OrdinalIgnoreCase);
				}
			) != -1;
			return isThemeExist || ThemesProvider.DefaultTheme.Equals(themeName, StringComparison.OrdinalIgnoreCase);
		}
		private static bool IsFontInfoPropertySetInAutoFormat(FontInfo fontInfo, PropertyDescriptor property) {
			PropertyInfo ownerPropertyInfo = typeof(FontInfo).GetProperty("Owner", BindingFlags.Instance | BindingFlags.NonPublic);
			Style style = ownerPropertyInfo.GetValue(fontInfo, null) as Style;
			int arg1 = 0x800;
			switch(property.Name.ToLower()) {
				case "bold":
					arg1 = 0x800;
					break;
				case "italic":
					arg1 = 0x1000;
					break;
				case "overline":
					arg1 = 0x4000;
					break;
				case "strikeout":
					arg1 = 0x8000;
					break;
				case "underline":
					arg1 = 0x2000;
					break;
				default:
					arg1 = 0x800;
					break;
			}
			return (bool)typeof(Style).InvokeMember("IsSet", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, style, new object[] { arg1 });
		}
	}
	public class ControlSerializer : FakeDesignerHost {
		private string
			content = string.Empty,
			directives = string.Empty;
		public string Content {
			get { return content; }
		}
		public string Directives {
			get { return directives; }
		}
		[SecuritySafeCritical]
		public Control[] GetControls(string content) {
			AssignContent(content);
			DesignTimeParseData parseData = new DesignTimeParseData(this, Directives + Content);				
			try {
				return DesignTimeTemplateParser.ParseControls(parseData); 
			}
			catch {
				return null;
			}
		}
		[SecuritySafeCritical]
		public string GetContent(Control[] controls) {
			string content = AspxCodeUtils.GetDirectives(Directives, ThemesProvider.DefaultTagPrefix);
			foreach(Control control in controls)
				content +=  Environment.NewLine + ControlPersister.PersistControl(control, this);
			return content;
		}
		void AssignContent(string content) {
			string directives = string.Empty;
			AspxCodeUtils.SeparateContent(ref content, ref directives);
			this.content = content;
			this.directives = directives;
		}
#pragma warning disable
		[SecuritySafeCritical]
		public override object GetService(Type serviceType) {
			if(serviceType == typeof(IWebFormReferenceManager) || serviceType == typeof(ITypeResolutionService) || serviceType == typeof(IFilterResolutionService))
				return new ControlSerializerServices(content, directives);
			return null;
		}
		[SecurityCritical]
		private class ControlSerializerServices : IWebFormReferenceManager, ITypeResolutionService, IFilterResolutionService {
			string content;
			string directives;
			public ControlSerializerServices(string content, string directives) {
				this.content = content;
				this.directives = directives;
			}
			#region IWebFormReferenceManager Members
			[SecurityCritical]
			Type IWebFormReferenceManager.GetObjectType(string tagPrefix, string typeName) {
				return null;
			}
			[SecurityCritical]
			string IWebFormReferenceManager.GetRegisterDirectives() {
				return this.directives;
			}
			[SecurityCritical]
			string IWebFormReferenceManager.GetTagPrefix(Type objectType) {
				return ThemesProvider.DefaultTagPrefix;
			}
			#endregion
			#region ITypeResolutionService Members
			[SecuritySafeCritical]
			Assembly ITypeResolutionService.GetAssembly(System.Reflection.AssemblyName name, bool throwOnError) {
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach(Assembly assembly in assemblies) {
					if(assembly.FullName == name.FullName)
						return assembly;
				}
				return null;
			}
			[SecuritySafeCritical]
			Assembly ITypeResolutionService.GetAssembly(System.Reflection.AssemblyName name) {
				return ((ITypeResolutionService)this).GetAssembly(name, false);
			}
			[SecuritySafeCritical]
			string ITypeResolutionService.GetPathOfAssembly(System.Reflection.AssemblyName name) {
				return string.Empty;
			}
			[SecuritySafeCritical]
			Type ITypeResolutionService.GetType(string name, bool throwOnError, bool ignoreCase) {
				return Type.GetType(name, throwOnError, ignoreCase);
			}
			[SecuritySafeCritical]
			Type ITypeResolutionService.GetType(string name, bool throwOnError) {
				return Type.GetType(name, throwOnError);
			}
			[SecuritySafeCritical]
			Type ITypeResolutionService.GetType(string name) {
				return Type.GetType(name);
			}
			[SecuritySafeCritical]
			void ITypeResolutionService.ReferenceAssembly(System.Reflection.AssemblyName name) {
			}
			#endregion
			#region IFilterResolutionService Members
			[SecuritySafeCritical]
			int IFilterResolutionService.CompareFilters(string filter1, string filter2) {
				return string.Compare(filter1, filter2);
			}
			[SecuritySafeCritical]
			bool IFilterResolutionService.EvaluateFilter(string filterName) {
				return false;
			}
			#endregion
		}
#pragma warning restore
	}
	internal static class UrlPathHelper {
		public static string AppDomainAppVirtualPath = HttpRuntime.AppDomainAppVirtualPath;
		private static string Combine(string basepath, string relative) {
			string appPath = AppDomainAppVirtualPath;
			string str;
			if ((basepath[0] == '~') && (basepath.Length == 1))
				basepath = "~/";
			else {
				int num = basepath.LastIndexOf('/');
				if (num < (basepath.Length - 1))
					basepath = basepath.Substring(0, num + 1);
			}
			if (IsRooted(relative)) {
				str = relative;
			} else {
				if ((relative.Length == 1) && (relative[0] == '~'))
					return appPath;
				if (IsAppRelativePath(relative)) {
					if (appPath.Length > 1)
						str = appPath + "/" + relative.Substring(2);
					else
						str = "/" + relative.Substring(2);
				} else
					str = SimpleCombine(basepath, relative);
			}
			return Reduce(str);
		}
		private static string FixVirtualPathSlashes(string virtualPath) {
			virtualPath = virtualPath.Replace('\\', '/');
			while (true) {
				string str = virtualPath.Replace("//", "/");
				if (str == virtualPath) {
					return virtualPath;
				}
				virtualPath = str;
			}
		}
		private static bool HasScheme(string virtualPath) {
			int index = virtualPath.IndexOf(':');
			if (index == -1) {
				return false;
			}
			int num2 = virtualPath.IndexOf('/');
			if (num2 != -1) {
				return (index < num2);
			}
			return true;
		}
		private static bool HasTrailingSlash(string virtualPath) {
			return (virtualPath[virtualPath.Length - 1] == '/');
		}
		private static bool IsAbsolutePhysicalPath(string path) {
			if ((path == null) || (path.Length < 3)) {
				return false;
			}
			return (((path[1] == ':') && IsDirectorySeparatorChar(path[2])) || IsUncSharePath(path));
		}
		private static bool IsAppRelativePath(string path) {
			if (path == null) {
				return false;
			}
			int length = path.Length;
			if (length == 0) {
				return false;
			}
			if (path[0] != '~') {
				return false;
			}
			if ((length != 1) && (path[1] != '\\')) {
				return (path[1] == '/');
			}
			return true;
		}
		private static bool IsDirectorySeparatorChar(char ch) {
			if (ch != '\\') {
				return (ch == '/');
			}
			return true;
		}
		private static bool IsRelativeUrl(string virtualPath) {
			if (HasScheme(virtualPath))
				return false;
			return !IsRooted(virtualPath);
		}
		private static bool IsRooted(string basepath) {
			if (!string.IsNullOrEmpty(basepath) && (basepath[0] != '/')) {
				return (basepath[0] == '\\');
			}
			return true;
		}
		private static bool IsUncSharePath(string path) {
			return (((path.Length > 2) && IsDirectorySeparatorChar(path[0])) && IsDirectorySeparatorChar(path[1]));
		}
		private static string MakeVirtualPathAppAbsolute(string virtualPath) {
			string applicationPath = AppDomainAppVirtualPath;
			if ((virtualPath.Length == 1) && (virtualPath[0] == '~')) {
				return applicationPath;
			}
			if (((virtualPath.Length >= 2) && (virtualPath[0] == '~')) && ((virtualPath[1] == '/') || (virtualPath[1] == '\\'))) {
				if (applicationPath.Length > 1) {
					return (applicationPath + virtualPath.Substring(2));
				}
				return ("/" + virtualPath.Substring(2));
			}
			if (!IsRooted(virtualPath)) {
				throw new ArgumentOutOfRangeException("virtualPath");
			}
			return virtualPath;
		}
		private static string MakeVirtualPathAppRelative(string virtualPath) {
			if(string.IsNullOrEmpty(AppDomainAppVirtualPath)) return virtualPath;
			string applicationPath = AppDomainAppVirtualPath;
			int length = applicationPath.Length;
			int num2 = virtualPath.Length;
			if ((num2 == (length - 1)) && StringStartsWithIgnoreCase(applicationPath, virtualPath)) {
				return "~/";
			}
			if (!VirtualPathStartsWithVirtualPath(virtualPath, applicationPath)) {
				return virtualPath;
			}
			if (num2 == length) {
				return "~/";
			}
			if (length == 1) {
				return ('~' + virtualPath);
			}
			return ('~' + virtualPath.Substring(length - 1));
		}
		private static string Reduce(string path) {
			string str = null;
			if (path != null) {
				int index = path.IndexOf('?');
				if (index >= 0) {
					str = path.Substring(index);
					path = path.Substring(0, index);
				}
			}
			path = FixVirtualPathSlashes(path);
			path = ReduceVirtualPath(path);
			if (str == null) {
				return path;
			}
			return (path + str);
		}
		private static string ReduceVirtualPath(string path) {
			int length = path.Length;
			int startIndex = 0;
			while (true) {
				startIndex = path.IndexOf('.', startIndex);
				if (startIndex < 0) {
					return path;
				}
				if (((startIndex == 0) || (path[startIndex - 1] == '/')) && ((((startIndex + 1) == length) || (path[startIndex + 1] == '/')) || ((path[startIndex + 1] == '.') && (((startIndex + 2) == length) || (path[startIndex + 2] == '/'))))) {
					break;
				}
				startIndex++;
			}
			ArrayList list = new ArrayList();
			StringBuilder builder = new StringBuilder();
			startIndex = 0;
			do {
				int num3 = startIndex;
				startIndex = path.IndexOf('/', num3 + 1);
				if (startIndex < 0) {
					startIndex = length;
				}
				if ((((startIndex - num3) <= 3) && ((startIndex < 1) || (path[startIndex - 1] == '.'))) && (((num3 + 1) >= length) || (path[num3 + 1] == '.'))) {
					if ((startIndex - num3) == 3) {
						if (list.Count == 0)
							return string.Empty;	
						if ((list.Count == 1) && IsAppRelativePath(path)) {
							return ReduceVirtualPath(MakeVirtualPathAppAbsolute(path));
						}
						builder.Length = (int)list[list.Count - 1];
						list.RemoveRange(list.Count - 1, 1);
					}
				} else {
					list.Add(builder.Length);
					builder.Append(path, num3, startIndex - num3);
				}
			}
			while (startIndex != length);
			string str = builder.ToString();
			if (str.Length != 0) {
				return str;
			}
			if ((length > 0) && (path[0] == '/')) {
				return "/";
			}
			return ".";
		}
		private static string SimpleCombine(string basepath, string relative) {
			if (HasTrailingSlash(basepath))
				return (basepath + relative);
			return (basepath + "/" + relative);
		}
		private static bool StringStartsWithIgnoreCase(string s1, string s2) {
			if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
				return false;
			if (s2.Length > s1.Length)
				return false;
			return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase));
		}
		private static bool VirtualPathStartsWithVirtualPath(string virtualPath1, string virtualPath2) {
			if (!StringStartsWithIgnoreCase(virtualPath1, virtualPath2))
				return false;
			int length = virtualPath2.Length;
			if (virtualPath1.Length != length) {
				if (length == 1)
					return true;
				if (virtualPath2[length - 1] == '/')
					return true;
				if (virtualPath1[length] != '/')
					return false;
			}
			return true;
		}
		public static string PrepareUrlPath(string urlPath, string themeName) {
			if (IsRelativeUrl(urlPath) && !IsAppRelativePath(urlPath)) {
				string path = Combine("~/App_Themes/" + themeName + "/", urlPath);
				return MakeVirtualPathAppRelative(path);
			}
			return urlPath;
		}
	}
	public static class SkinFileParser {
		public static List<string> ExcludePropertyNames = new List<string>(new string[] { 
			"runat", "SkinID", "EnableTheming", "ValueCheckedString", "ValueUncheckedString", "ValueGrayedString" });
		public delegate Control GetSkinControlDelegate();
		public delegate string GetSkinFileContentDelegate();
		private static string
			CurrentThemeName = string.Empty,
			DirectiveRegEx = "<%[^>]*%>",
			NameSpaceRegEx = "Namespace=\"[^\"]+\"",
			AssemblyRegEx = "Assembly=\"[^\"]+\"",
			SchedulerColorSchemaCollectionClassName = "SchedulerColorSchemaCollection",
			SkinIDPropertyName = "SkinID";
		private static char
			PropertyPathSeparator = '-';
		private static Hashtable
			skinXMLCache = new Hashtable();
		private static object
			lockSkinXMLCache = new object();
		#region Cache
		private static Hashtable SkinXMLCache {
			get { return skinXMLCache; }
		}
		#endregion
		#region Property Utils
		private static object GetCompositePropertyValue(object obj, string propertyName) {
			List<string> propertyNames = new List<string>(propertyName.Split(PropertyPathSeparator));
			propertyNames.Remove(string.Empty);
			object result = obj;
			foreach (string propName in propertyNames)
				result = GetPropertyValue(result, propName);
			return result;
		}
		private static PropertyInfo GetProperty(object obj, string propertyName) {
			PropertyInfo[] properties = obj.GetType().GetProperties();
			foreach (PropertyInfo property in properties) {
				if(string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
					return property;
			}
			return null;
		}
		private static object GetPropertyValue(object obj, string propertyName) {
			bool success = true;
			return TryGetPropertyValue(obj, propertyName, out success);
		}
		private static TypeConverter GetPropertyTypeConverter(PropertyInfo propertyInfo) {
			foreach (object attribute in propertyInfo.GetCustomAttributes(typeof(TypeConverterAttribute), false)) {
				TypeConverterAttribute attr = attribute as TypeConverterAttribute;
				if(!attr.IsDefaultAttribute()) {
					try { 
						var converter = Activator.CreateInstance(Type.GetType(attr.ConverterTypeName)) as TypeConverter;
						return converter;
					}
					catch { }
				}
			}
			return null;
		}
		private static bool IsUrlProperty(PropertyInfo propertyInfo) {
			object[] attributes = propertyInfo.GetCustomAttributes(typeof(UrlPropertyAttribute), false);
			return attributes.Length > 0;
		}
		private static object PrepareUrlPropertyValue(object value, string themeName) {
			return UrlPathHelper.PrepareUrlPath(value.ToString(), themeName); 
		}
		private static void SetCompositePropertyValue(object obj, string propertyName, string propertyValue) {
			string[] simpleProperties = propertyName.Split(PropertyPathSeparator);
			object targetObject = obj;
			for (int i = 0; i < simpleProperties.Length; i++) {
				string currentPropertyName = simpleProperties[i];
				if (i == simpleProperties.Length - 1)
					SetPropertyValue(targetObject, currentPropertyName, propertyValue);
				else
					targetObject = GetPropertyValue(targetObject, currentPropertyName);
			}
		}
		private static void SetPropertyValue(object obj, string propertyName, string propertyValue) {
			propertyValue = HttpUtility.HtmlDecode(propertyValue); 
			PropertyInfo propertyInfo = GetProperty(obj, propertyName);
			if (propertyInfo == null)
				throw new ArgumentException(string.Format("An error has occurred during applying the theme. The property with the '{0}' name does not exist in {1}.", propertyName, obj.GetType().Name));
			object value = null;
			TypeConverter typeConverter = GetPropertyTypeConverter(propertyInfo);
			if(typeConverter != null)
				value = typeConverter.ConvertFrom(null, CultureInfo.CurrentCulture, propertyValue);
			if (propertyInfo.PropertyType.IsEnum)
				value = Enum.Parse(propertyInfo.PropertyType, propertyValue);
			if (value == null) {
				typeConverter  = TypeDescriptor.GetConverter(propertyInfo.PropertyType); 
				value = typeConverter.ConvertFromString(null, CultureInfo.GetCultureInfo("en-US"), propertyValue);
			}
			if (IsUrlProperty(propertyInfo))
				value = PrepareUrlPropertyValue(value, CurrentThemeName);
			propertyInfo.SetValue(obj, value, null);
		}
		private static object TryGetPropertyValue(object obj, string propertyName, out bool success) {
			PropertyInfo propertyInfo = GetProperty(obj, propertyName);
			success = propertyInfo != null;
			return success ? propertyInfo.GetValue(obj, null) : null;
		}
		#endregion
		#region Prepare Skin File Content
		private static List<string> GetTabPrefixes(string skinFileContent) {
			string tagPrefixAttribute = "TagPrefix=\"";
			List<string> result = new List<string>();
			Regex reg = new Regex(DirectiveRegEx);
			foreach (Match directive in reg.Matches(skinFileContent)) {
				int startIndex = directive.Value.IndexOf(tagPrefixAttribute, StringComparison.OrdinalIgnoreCase);
				if (startIndex != -1) {
					startIndex += tagPrefixAttribute.Length;
					int endIndex = directive.Value.IndexOf("\"", startIndex);
					result.Add(directive.Value.Substring(startIndex, endIndex - startIndex));
				}
			}
			return result;
		}
		private static List<string> GetNamespaces(string skinFileContent) {
			List<string> result = new List<string>();
			Regex reg = new Regex(DirectiveRegEx);
			foreach (Match directive in reg.Matches(skinFileContent)) {
				string nameSpace = new Regex(NameSpaceRegEx, RegexOptions.IgnoreCase).Match(directive.Value).Value;
				nameSpace = nameSpace.Substring(nameSpace.IndexOf('"')).Replace("\"", string.Empty);
				string assembly = new Regex(AssemblyRegEx, RegexOptions.IgnoreCase).Match(directive.Value).Value;
				assembly = assembly.Substring(assembly.IndexOf('"')).Replace("\"", string.Empty);
				result.Add(assembly + "-" + nameSpace);
			}
			return result;
		}
		private static string RemoveComments(string skinFileContent) {
			string startCommentTag = "<%--",
				endCommentTag = "--%>";
			int startIndex = skinFileContent.IndexOf(startCommentTag);
			if (startIndex != -1) {
				int endIndex = skinFileContent.IndexOf(endCommentTag, startIndex) + endCommentTag.Length;
				if (endIndex != -1) {
					string comment = skinFileContent.Substring(startIndex, endIndex - startIndex);
					skinFileContent = skinFileContent.Replace(comment, string.Empty);
					skinFileContent = RemoveComments(skinFileContent);
				}
			}
			return skinFileContent;
		}
		private static string RemoveTabPrefix(string skinFileContent, string tabPrefix) {
			Regex reg = new Regex(string.Format("<\\s*{0}\\s*:", tabPrefix));
			skinFileContent = reg.Replace(skinFileContent, "<");
			reg = new Regex(string.Format("<\\s*/\\s*{0}\\s*:", tabPrefix));
			return reg.Replace(skinFileContent, "</");
		}
		private static string RemoveTabPrefixes(string skinFileContent) {
			List<string> tabPrefixes = GetTabPrefixes(skinFileContent);
			foreach (string tabPrefix in tabPrefixes)
				skinFileContent = RemoveTabPrefix(skinFileContent, tabPrefix);
			return skinFileContent;
		}
		private static string PrepareSkinFileContent(string skinFileContent) {
			skinFileContent = RemoveTabPrefixes(skinFileContent);
			Regex reg = new Regex(DirectiveRegEx);
			skinFileContent = reg.Replace(skinFileContent, string.Empty);
			skinFileContent = RemoveComments(skinFileContent);
			return string.Format("<root>{0}</root>", skinFileContent);
		}
		#endregion
		private static void ApplySkinXMLRecursive(object control, object currentProperty, XmlNode skinNode, string propertyPath, GetSkinControlDelegate getSkinControl, List<string> namespaces) {
			foreach (XmlAttribute attribute in skinNode.Attributes) {
				if (!ExcludePropertyNames.Contains(attribute.Name)) {
					if (attribute.Name.IndexOf(PropertyPathSeparator) != -1)
						SetCompositePropertyValue(currentProperty, attribute.Name, attribute.Value);
					else
						SetPropertyValue(currentProperty, attribute.Name, attribute.Value);
				}
			}
			foreach (XmlNode child in skinNode.ChildNodes) {
				bool isPropertyExist = false;
				object childObj = TryGetPropertyValue(currentProperty, child.Name, out isPropertyExist);
				if (isPropertyExist) {
					if(childObj is Collection || childObj.GetType().Name == SchedulerColorSchemaCollectionClassName)
						LoadCollection(control, childObj, child, namespaces);
					else {
						string newPropertyPath = string.Format("{0}{1}{2}", propertyPath, PropertyPathSeparator, child.Name);
						ApplySkinXMLRecursive(control, childObj, child, newPropertyPath, getSkinControl, namespaces);
					}
				} else {
					LoadPropertyValueFromSkinControl(control, propertyPath, skinNode.Name, getSkinControl);
					break;
				}
			}
		}
		private static void LoadCollection(object control, object collection, XmlNode xmlNode, List<string> namespaces) {
			collection.GetType().GetMethod("Clear", Type.EmptyTypes).Invoke(collection, null);
			foreach (XmlNode child in xmlNode.ChildNodes) {
				string controlNamespace = control.GetType().Namespace;
				Type itemType = null;
				foreach (string nameSpace in namespaces) {
					itemType = Assembly.Load(nameSpace.Split('-')[0]).GetType(nameSpace.Split('-')[1] + '.' + child.Name);
					if (itemType != null)
						break;
				}
				object collectionItem = Activator.CreateInstance(itemType);
				collection.GetType().GetMethod("Add", new Type[] { collectionItem.GetType() }).Invoke(collection, new object[] { collectionItem });
				ApplySkinXMLRecursive(collectionItem, collectionItem, child, string.Empty, null, namespaces);
			}
		}
		private static XmlNode GetSkinXML(Control control, string themeName, GetSkinFileContentDelegate getSkinFileContent, out List<string> namespaces) {
			string key = string.Format("{0}_{1}_{2}", themeName, control.GetType().Name, control.SkinID).ToLower();
			lock (lockSkinXMLCache) {
				if (!SkinXMLCache.ContainsKey(key)) {
					string skinFileContent = getSkinFileContent();
					namespaces = GetNamespaces(skinFileContent);
					XmlNode node = GetSkinXMLRootNode(control, skinFileContent);
					SkinXMLCache.Add(key, new object[] { node, namespaces });
				}
				object[] cachedValue = (object[])SkinXMLCache[key];
				namespaces = (List<string>)(cachedValue)[1];
				return (XmlNode)(cachedValue)[0];
			}
		}
		private static XmlNode GetSkinXMLRootNode(Control conrtol, string skinFileContent) {
			skinFileContent = PrepareSkinFileContent(skinFileContent);
			XmlDocument xmlDoc = new XmlDocument();
			skinFileContent = skinFileContent.Replace("&", "&amp;"); 
			try {
				xmlDoc.LoadXml(skinFileContent);
			} catch (Exception e) {
				throw new ArgumentException(string.Format("Skin file cannot be parsed: {0}", e.Message));
			}
			return GetTargetSkin(conrtol, xmlDoc);
		}
		private static XmlNode GetTargetSkin(Control conrtol, XmlDocument xmlDoc) {
			XmlNodeList skinList = xmlDoc.SelectNodes("/*/*");
			bool isControlSkinIDEmpty = string.IsNullOrEmpty(conrtol.SkinID);
			foreach (XmlNode skinNode in skinList) {
				XmlAttribute skinIDAttribute = skinNode.Attributes[SkinIDPropertyName];
				bool isCurrentSkinIDEmpty = skinIDAttribute == null || string.IsNullOrEmpty(skinIDAttribute.Value);
				if (isControlSkinIDEmpty && isCurrentSkinIDEmpty ||
					!isControlSkinIDEmpty && !isCurrentSkinIDEmpty && conrtol.SkinID == skinIDAttribute.Value)
					return skinNode;
			}
			return null;
		}
		private static void LoadPropertyValueFromSkinControl(object control, string propertyPath, string propertyName, GetSkinControlDelegate getSkinControl) {
			Control skinControl = getSkinControl();
			string parentPropertyPath = propertyPath.Substring(0, propertyPath.LastIndexOf(PropertyPathSeparator));
			object source = GetCompositePropertyValue(skinControl, parentPropertyPath);
			object destination = GetCompositePropertyValue(control, parentPropertyPath);
			ThemesHelper.CopyProperty(source, destination, propertyName);
		}
		public static void ApplySkinToControl(Control conrtol, GetSkinControlDelegate getSkinControl, GetSkinFileContentDelegate getSkinFileContent, string themeName) {
			CurrentThemeName = themeName;
			List<string> namespaces = new List<string>();
			XmlNode targetSkinXmlNode = GetSkinXML(conrtol, themeName, getSkinFileContent, out namespaces);
			if(targetSkinXmlNode != null)
				ApplySkinXMLRecursive(conrtol, conrtol, targetSkinXmlNode, string.Empty, getSkinControl, namespaces);
		}
	}
}

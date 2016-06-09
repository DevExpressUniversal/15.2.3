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
using System.Collections.Generic;
using System.Text;
using System.Configuration;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using DevExpress.Utils;
	public class CompressionConfigurationSection : ConfigurationSection {
		public const string EnableHtmlCompressionAttribute = "enableHtmlCompression";
		public const string EnableCallbackCompressionAttribute = "enableCallbackCompression";
		public const string EnableResourceCompressionAttribute = "enableResourceCompression";
		public const string EnableResourceMergingAttribute = "enableResourceMerging";
		[
#if !SL
	DevExpressWebLocalizedDescription("CompressionConfigurationSectionEnableHtmlCompression"),
#endif
		ConfigurationProperty(EnableHtmlCompressionAttribute, DefaultValue = false)]
		public bool EnableHtmlCompression {
			get { return (bool)this[EnableHtmlCompressionAttribute]; }
			set { this[EnableHtmlCompressionAttribute] = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CompressionConfigurationSectionEnableCallbackCompression"),
#endif
		ConfigurationProperty(EnableCallbackCompressionAttribute, DefaultValue = true)]
		public bool EnableCallbackCompression
		{
			get { return (bool)this[EnableCallbackCompressionAttribute]; }
			set { this[EnableCallbackCompressionAttribute] = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CompressionConfigurationSectionEnableResourceCompression"),
#endif
		ConfigurationProperty(EnableResourceCompressionAttribute, DefaultValue = true)]
		public bool EnableResourceCompression
		{
			get { return (bool)this[EnableResourceCompressionAttribute]; }
			set { this[EnableResourceCompressionAttribute] = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CompressionConfigurationSectionEnableResourceMerging"),
#endif
		ConfigurationProperty(EnableResourceMergingAttribute, DefaultValue = true)]
		public bool EnableResourceMerging
		{
			get { return (bool)this[EnableResourceMergingAttribute]; }
			set { this[EnableResourceMergingAttribute] = value; }
		}
		public static CompressionConfigurationSection Get() {
			string name = ConfigurationSectionNames.WebSectionGroupName + '/' + ConfigurationSectionNames.CompressionSectionName;
			return ConfigurationManager.GetSection(name) as CompressionConfigurationSection;
		}
	}
	public class ThemesConfigurationSection : ConfigurationSection {
		public const string 
			EnableThemesAssemblyAttribute = "enableThemesAssembly",
			StyleSheetThemeAttribute = "styleSheetTheme",
			ThemeAttribute = "theme",
			CustomThemeAssembliesAttribute = "customThemeAssemblies";
		[
#if !SL
	DevExpressWebLocalizedDescription("ThemesConfigurationSectionEnableThemesAssembly"),
#endif
		ConfigurationProperty(EnableThemesAssemblyAttribute, DefaultValue = true)]
		public bool EnableThemesAssembly
		{
			get { return (bool)this[EnableThemesAssemblyAttribute]; }
			set { this[EnableThemesAssemblyAttribute] = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ThemesConfigurationSectionStyleSheetTheme"),
#endif
		ConfigurationProperty(StyleSheetThemeAttribute, DefaultValue = "")]
		public string StyleSheetTheme {
			get { return this[StyleSheetThemeAttribute] as string; }
			set { this[StyleSheetThemeAttribute] = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ThemesConfigurationSectionTheme"),
#endif
		ConfigurationProperty(ThemeAttribute, DefaultValue = "")]
		public string Theme {
			get { return this[ThemeAttribute] as string; }
			set { this[ThemeAttribute] = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ThemesConfigurationSectionCustomThemeAssemblies"),
#endif
		ConfigurationProperty(CustomThemeAssembliesAttribute, DefaultValue = "")]
		public string CustomThemeAssemblies {
			get { return this[CustomThemeAssembliesAttribute] as string; }
			set { this[CustomThemeAssembliesAttribute] = value; }
		}
		public static ThemesConfigurationSection Get() {
			string name = ConfigurationSectionNames.WebSectionGroupName + '/' + ConfigurationSectionNames.ThemesSectionName;
			return ConfigurationManager.GetSection(name) as ThemesConfigurationSection;
		}
	}
	public class ErrorsConfigurationSection : ConfigurationSection {
		public const string ErrorPageUrlAttribute = "callbackErrorRedirectUrl";
		[
#if !SL
	DevExpressWebLocalizedDescription("ErrorsConfigurationSectionErrorPageUrl"),
#endif
		ConfigurationProperty(ErrorPageUrlAttribute, DefaultValue = null)]
		public string ErrorPageUrl
		{
			get { return (string)this[ErrorPageUrlAttribute]; }
			set { this[ErrorPageUrlAttribute] = value; }
		}
		public static ErrorsConfigurationSection Get() {
			string name = ConfigurationSectionNames.WebSectionGroupName + '/' + ConfigurationSectionNames.ErrorsSectionName;
			return ConfigurationManager.GetSection(name) as ErrorsConfigurationSection;
		}
	}
	public enum DoctypeMode { Xhtml, Html5 }
	public class SettingsConfigurationSection : ConfigurationSection {
		public const string RightToLeftAttribute = "rightToLeft";
		public const string DoctypeModeAttribute = "doctypeMode";
		public const string EmbedRequiredClientLibrariesAttribute = "embedRequiredClientLibraries";
		public const string IECompatibilityVersionAttribute = "ieCompatibilityVersion";
		public const string IECompatibilityVersionEdge = "edge";
		[
#if !SL
	DevExpressWebLocalizedDescription("SettingsConfigurationSectionDoctypeMode"),
#endif
		ConfigurationProperty(DoctypeModeAttribute, DefaultValue = DoctypeMode.Html5)]
		public DoctypeMode DoctypeMode {
			get { return (DoctypeMode)this[DoctypeModeAttribute]; }
			set { this[DoctypeModeAttribute] = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SettingsConfigurationSectionRightToLeft"),
#endif
		ConfigurationProperty(RightToLeftAttribute, DefaultValue = false)]
		public bool RightToLeft
		{
			get { return (bool)this[RightToLeftAttribute]; }
			set { this[RightToLeftAttribute] = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SettingsConfigurationSectionEmbedRequiredClientLibraries"),
#endif
		ConfigurationProperty(EmbedRequiredClientLibrariesAttribute, DefaultValue = false),]
		public bool EmbedRequiredClientLibraries {
			get { return (bool)this[EmbedRequiredClientLibrariesAttribute]; }
			set { this[EmbedRequiredClientLibrariesAttribute] = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SettingsConfigurationSectionIECompatibilityVersion"),
#endif
		ConfigurationProperty(IECompatibilityVersionAttribute, DefaultValue = SettingsConfigurationSection.IECompatibilityVersionEdge)]
		public string IECompatibilityVersion {
			get { return (string)this[IECompatibilityVersionAttribute]; }
			set { this[IECompatibilityVersionAttribute] = value; }
		}
		public static SettingsConfigurationSection Get() {
			string name = ConfigurationSectionNames.WebSectionGroupName + '/' + ConfigurationSectionNames.SettingsSectionName;
			return ConfigurationManager.GetSection(name) as SettingsConfigurationSection;
		}
	}
}
namespace DevExpress.Web.Internal {
	public static class ConfigurationSectionNames {
		public const string ConfigurationSectionName = "configuration";
		public const string WebSectionGroupName = "devExpress";
		public const string CompressionSectionName = "compression";
		public const string ThemesSectionName = "themes";
		public const string ErrorsSectionName = "errors";
		public const string SettingsSectionName = "settings";
		public const string HttpModuleIIS7Section = "system.webServer/modules";
		public const string HttpModuleSection = "system.web/httpModules";
		public const string HttpHandlerIIS7Section = "system.webServer/handlers";
		public const string HttpHandlerSection = "system.web/httpHandlers";
		public const string HttpHandlerIIS7ValidationSection = "system.webServer/validation";
	}
	public static class ConfigurationSettings {
		public const string EnableHtmlCompressionKey = "DXEnableHtmlCompression";
		public const string EnableCallbackCompressionKey = "DXEnableCallbackCompression";
		public const string EnableResourceCompressionKey = "DXEnableResourceCompression";
		public const string EnableResourceMergingKey = "DXEnableResourceMerging";
		public const string EnableThemesAssemblyKey = "DXEnableThemesAssembly";
		public const string StyleSheetThemeKey = "DXStyleSheetTheme";
		public const string ThemeKey = "DXTheme";
		public const string CustomThemeAssembliesKey = "DXCustomThemeAssemblies";
		public const string ErrorPageUrlKey = "DXCallbackErrorRedirectUrl";
		public const string ResourcesPhysicalPathKey = "DXResourcesPhysicalPath";
		public const string DoctypeModeKey = "DXDoctypeMode";
		public static bool EnableHtmlCompression {
			get {
				bool result = false;
				CompressionConfigurationSection section = CompressionConfigurationSection.Get();
				string strKeyValue = ConfigurationManager.AppSettings[EnableHtmlCompressionKey];
				bool.TryParse(strKeyValue, out result);
				return section != null ? result || section.EnableHtmlCompression : result;
			}
		}
		public static bool EnableCallbackCompression {
			get {
				bool result = true;
				bool keyValue = true;
				CompressionConfigurationSection section = CompressionConfigurationSection.Get();
				string strKeyValue = ConfigurationManager.AppSettings[EnableCallbackCompressionKey];
				if(bool.TryParse(strKeyValue, out keyValue))
					result &= keyValue;
				return section != null ? result && section.EnableCallbackCompression : result;
			}
		}
		public static bool EnableResourceCompression {
			get {
				bool result = true;
				bool keyValue = true;
				CompressionConfigurationSection section = CompressionConfigurationSection.Get();
				string strKeyValue = ConfigurationManager.AppSettings[EnableResourceCompressionKey];
				if(bool.TryParse(strKeyValue, out keyValue))
					result &= keyValue;
				return section != null ? result && section.EnableResourceCompression : result;
			}
		}
		public static bool EnableResourceMerging {
			get {
				bool result = false;
				CompressionConfigurationSection section = CompressionConfigurationSection.Get();
				string strKeyValue = ConfigurationManager.AppSettings[EnableResourceMergingKey];
				bool.TryParse(strKeyValue, out result);
				return section != null ? result || section.EnableResourceMerging : result;
			}
		}
		public static bool EnableThemesAssembly {
			get {
				bool result = true;
				bool keyValue = true;
				ThemesConfigurationSection section = ThemesConfigurationSection.Get();
				string strKeyValue = ConfigurationManager.AppSettings[EnableThemesAssemblyKey];
				if(bool.TryParse(strKeyValue, out keyValue))
					result &= keyValue;
				return section != null ? result && section.EnableThemesAssembly : result;
			}
		}
		static string styleSheetTheme;
		public static string StyleSheetTheme {
			get {
				if(styleSheetTheme == null) {
					ThemesConfigurationSection section = ThemesConfigurationSection.Get();
					string strKeyValue = ConfigurationManager.AppSettings[StyleSheetThemeKey];
					styleSheetTheme = section != null ? section.StyleSheetTheme : strKeyValue;
				}
				return styleSheetTheme;
			}
		}
		static string theme;
		public static string Theme {
			get {
				if(theme == null) {
					ThemesConfigurationSection section = ThemesConfigurationSection.Get();
					string strKeyValue = ConfigurationManager.AppSettings[ThemeKey];
					theme = section != null ? section.Theme : strKeyValue;
				}
				return theme;
			}
		}
		public static string CustomThemeAssemblies {
			get {
				ThemesConfigurationSection section = ThemesConfigurationSection.Get();
				string strKeyValue = ConfigurationManager.AppSettings[CustomThemeAssembliesKey];
				return section != null ? section.CustomThemeAssemblies : strKeyValue;
			}
		}
		public static string ErrorPageUrl {
			get {
				ErrorsConfigurationSection section = ErrorsConfigurationSection.Get();
				if(section != null && !string.IsNullOrEmpty(section.ErrorPageUrl))
					return section.ErrorPageUrl;
				return ConfigurationManager.AppSettings[ErrorPageUrlKey];
			}
		}
		public static DoctypeMode DoctypeMode {
			get {
				SettingsConfigurationSection section = SettingsConfigurationSection.Get();
				string strKeyValue = ConfigurationManager.AppSettings[DoctypeModeKey];
				if(section != null)
					return section.DoctypeMode;
				else if(!string.IsNullOrEmpty(strKeyValue))
					return (DoctypeMode)Enum.Parse(typeof(DoctypeMode), strKeyValue);
				return DoctypeMode.Html5;
			}
		}
		public static bool RightToLeft {
			get {
				SettingsConfigurationSection section = SettingsConfigurationSection.Get();
				if(section != null)
					return section.RightToLeft;
				return false;
			}
		}
		public static bool EmbedRequiredClientLibraries {
			get {
				SettingsConfigurationSection section = SettingsConfigurationSection.Get();
				if(section != null)
					return section.EmbedRequiredClientLibraries;
				return false;
			}
		}
		public static string IECompatibilityVersion {
			get {
				SettingsConfigurationSection section = SettingsConfigurationSection.Get();
				if(section != null)
					return section.IECompatibilityVersion;
				return SettingsConfigurationSection.IECompatibilityVersionEdge;
			}
		}
		public static string ResourcesPhysicalPath {
			get { return ConfigurationManager.AppSettings[ResourcesPhysicalPathKey]; }
		}
	}
}

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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Runtime.InteropServices;
using DevExpress.Utils;
#if SL
using System.Windows.Media;
using System.Windows.Resources;
#endif
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core {
	[TypeConverter(typeof(ThemeTypeConverter))]
	public class Theme {
		string name;
		string fullName;
		internal const string DXThemeKey = "DXThemeKey";
		internal const string AzureName = "Azure";
		static readonly Dictionary<string, Theme> ThemeNameHash = new Dictionary<string, Theme>();
		internal static List<Theme> ThemesInternal = new List<Theme>();
		public bool ShowInThemeSelector = true;
		public const string StandardCategory = "Standard Themes";
		public const string Office2007Category = "Office 2007 Themes";
		public const string Office2010Category = "Office 2010 Themes";
		public const string Office2013Category = "Office 2013 Themes";
		public const string Office2016Category = "Office 2016 Themes";
		public const string DevExpressCategory = "DevExpress Themes";
		public const string MetropolisCategory = "Metropolis Themes";
		public const string DeepBlueName = "DeepBlue";
		public const string LightGrayName = "LightGray";
		public const string Office2007BlueName = "Office2007Blue";
		public const string Office2007BlackName = "Office2007Black";
		public const string Office2007SilverName = "Office2007Silver";
		public const string SevenName = "Seven";
		public const string VS2010Name = "VS2010";
		public const string DXStyleName = "DXStyle";
		public const string Office2010BlackName = "Office2010Black";
		public const string Office2010BlueName = "Office2010Blue";
		public const string Office2010SilverName = "Office2010Silver";
		public const string MetropolisDarkName = "MetropolisDark";
		public const string TouchlineDarkName = "TouchlineDark";
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string HybridAppName = "HybridApp";
		public const string MetropolisLightName = "MetropolisLight";
		public const string Office2013Name = "Office2013";
		public const string Office2013LightGrayName = "Office2013LightGray";
		public const string Office2013DarkGrayName = "Office2013DarkGray";
		public const string Office2013TouchName = "Office2013;Touch";
		public const string Office2013LightGrayTouchName = "Office2013LightGray;Touch";
		public const string Office2013DarkGrayTouchName = "Office2013DarkGray;Touch";
		public const string Office2016WhiteName = "Office2016White";
		public const string Office2016BlackName = "Office2016Black";
		public const string Office2016ColorfulName = "Office2016Colorful";
		public const string Office2016WhiteTouchName = "Office2016White;Touch";
		public const string Office2016BlackTouchName = "Office2016Black;Touch";
		public const string Office2016ColorfulTouchName = "Office2016Colorful;Touch";
		public const string DeepBlueFullName = "Deep Blue";
		public const string LightGrayFullName = "Light Gray";
		public const string Office2007BlueFullName = "Office 2007 Blue";
		public const string Office2007BlackFullName = "Office 2007 Black";
		public const string Office2007SilverFullName = "Office 2007 Silver";
		public const string SevenFullName = "Seven";
		public const string VS2010FullName = "Visual Studio 2010";
		public const string DXStyleFullName = "DevExpress Style";
		public const string Office2010BlackFullName = "Office 2010 Black";
		public const string Office2010BlueFullName = "Office 2010 Blue";
		public const string Office2010SilverFullName = "Office 2010 Silver";
		public const string MetropolisDarkFullName = "Metropolis Dark";
		public const string TouchlineDarkFullName = "Touchline Dark";
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string HybridAppFullName = "HybridApp";
		public const string MetropolisLightFullName = "Metropolis Light";
		public const string Office2013FullName = "Office 2013";
		public const string Office2013LightGrayFullName = "Office 2013 Light Gray";
		public const string Office2013DarkGrayFullName = "Office 2013 Dark Gray";
		public const string Office2013TouchFullName = "Office 2013 Touch";
		public const string Office2013LightGrayTouchFullName = "Office 2013 Light Gray Touch";
		public const string Office2013DarkGrayTouchFullName = "Office 2013 Dark Gray Touch";
		public const string Office2016WhiteFullName = "Office 2016 White";
		public const string Office2016BlackFullName = "Office 2016 Black";
		public const string Office2016ColorfulFullName = "Office 2016 Colorful";
		public const string Office2016WhiteTouchFullName = "Office 2016 White Touch";
		public const string Office2016BlackTouchFullName = "Office 2016 Black Touch";
		public const string Office2016ColorfulTouchFullName = "Office 2016 Colorful Touch";
		public static Theme LightGray = new Theme(LightGrayName) { IsStandard = true, FullName = LightGrayFullName, Category = DevExpressCategory, SmallGlyph = GetThemeGlyphUri(LightGrayName, false), LargeGlyph = GetThemeGlyphUri(LightGrayName, true) };
		public static Theme DeepBlue = new Theme(DeepBlueName) { IsStandard = true, FullName = DeepBlueFullName, Category = DevExpressCategory, SmallGlyph = GetThemeGlyphUri(DeepBlueName, false), LargeGlyph = GetThemeGlyphUri(DeepBlueName, true) };
		public static Theme Office2007Blue = new Theme(Office2007BlueName) { IsStandard = true, FullName = Office2007BlueFullName, Category = Office2007Category, SmallGlyph = GetThemeGlyphUri(Office2007BlueName, false), LargeGlyph = GetThemeGlyphUri(Office2007BlueName, true) };
		public static Theme Office2007Black = new Theme(Office2007BlackName) { IsStandard = true, FullName = Office2007BlackFullName, Category = Office2007Category, SmallGlyph = GetThemeGlyphUri(Office2007BlackName, false), LargeGlyph = GetThemeGlyphUri(Office2007BlackName, true) };
		public static Theme Office2007Silver = new Theme(Office2007SilverName) { IsStandard = true, FullName = Office2007SilverFullName, Category = Office2007Category, SmallGlyph = GetThemeGlyphUri(Office2007SilverName, false), LargeGlyph = GetThemeGlyphUri(Office2007SilverName, true) };
		public static Theme Seven = new Theme(SevenName) { IsStandard = true, FullName = SevenFullName, Category = StandardCategory, SmallGlyph = GetThemeGlyphUri(SevenName, false), LargeGlyph = GetThemeGlyphUri(SevenName, true) };
		public static Theme VS2010 = new Theme(VS2010Name) { IsStandard = true, FullName = VS2010FullName, Category = StandardCategory, SmallGlyph = GetThemeGlyphUri(VS2010Name, false), LargeGlyph = GetThemeGlyphUri(VS2010Name, true) };
		public static Theme DXStyle = new Theme(DXStyleName) { IsStandard = true, FullName = DXStyleFullName, Category = DevExpressCategory, SmallGlyph = GetThemeGlyphUri(DXStyleName, false), LargeGlyph = GetThemeGlyphUri(DXStyleName, true) };
		public static Theme Office2010Black = new Theme(Office2010BlackName) { IsStandard = true, FullName = Office2010BlackFullName, Category = Office2010Category, SmallGlyph = GetThemeGlyphUri(Office2010BlackName, false), LargeGlyph = GetThemeGlyphUri(Office2010BlackName, true) };
		public static Theme Office2010Blue = new Theme(Office2010BlueName) { IsStandard = true, FullName = Office2010BlueFullName, Category = Office2010Category, SmallGlyph = GetThemeGlyphUri(Office2010BlueName, false), LargeGlyph = GetThemeGlyphUri(Office2010BlueName, true) };
		public static Theme Office2010Silver = new Theme(Office2010SilverName) { IsStandard = true, FullName = Office2010SilverFullName, Category = Office2010Category, SmallGlyph = GetThemeGlyphUri(Office2010SilverName, false), LargeGlyph = GetThemeGlyphUri(Office2010SilverName, true) };
		public static Theme MetropolisDark = new Theme(MetropolisDarkName) { IsStandard = true, FullName = MetropolisDarkFullName, Category = MetropolisCategory, SmallGlyph = GetThemeGlyphUri(MetropolisDarkName, false), LargeGlyph = GetThemeGlyphUri(MetropolisDarkName, true) };
		public static Theme TouchlineDark = new Theme(TouchlineDarkName) { IsStandard = true, FullName = TouchlineDarkFullName, Category = MetropolisCategory, SmallGlyph = GetThemeGlyphUri(TouchlineDarkName, false), LargeGlyph = GetThemeGlyphUri(TouchlineDarkName, true) };
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Theme HybridApp = new Theme(HybridAppName) { IsStandard = true, FullName = HybridAppFullName, Category = StandardCategory, SmallGlyph = GetThemeGlyphUri(Office2013Name, false), LargeGlyph = GetThemeGlyphUri(Office2013Name, true), ShowInThemeSelector = false };
		public static Theme MetropolisLight = new Theme(MetropolisLightName) { IsStandard = true, FullName = MetropolisLightFullName, Category = MetropolisCategory, SmallGlyph = GetThemeGlyphUri(MetropolisLightName, false), LargeGlyph = GetThemeGlyphUri(MetropolisLightName, true) };
		static bool showTouchThemes = true;
		public static Theme Office2013 = new Theme(Office2013Name) { IsStandard = true, FullName = Office2013FullName, Category = Office2013Category, SmallGlyph = GetThemeGlyphUri(Office2013Name, false), LargeGlyph = GetThemeGlyphUri(Office2013Name, true) };
		public static Theme Office2013LightGray = new Theme(Office2013LightGrayName) { IsStandard = true, FullName = Office2013LightGrayFullName, Category = Office2013Category, SmallGlyph = GetThemeGlyphUri(Office2013LightGrayName, false), LargeGlyph = GetThemeGlyphUri(Office2013LightGrayName, true) };
		public static Theme Office2013DarkGray = new Theme(Office2013DarkGrayName) { IsStandard = true, FullName = Office2013DarkGrayFullName, Category = Office2013Category, SmallGlyph = GetThemeGlyphUri(Office2013DarkGrayName, false), LargeGlyph = GetThemeGlyphUri(Office2013DarkGrayName, true) };
		public static Theme Office2013Touch = new Theme(Office2013TouchName) { IsStandard = true, FullName = Office2013TouchFullName, Category = Office2013Category, SmallGlyph = GetThemeGlyphUri(Office2013Name, false), LargeGlyph = GetThemeGlyphUri(Office2013Name, true), ShowInThemeSelector = showTouchThemes };
		public static Theme Office2013LightGrayTouch = new Theme(Office2013LightGrayTouchName) { IsStandard = true, FullName = Office2013LightGrayTouchFullName, Category = Office2013Category, SmallGlyph = GetThemeGlyphUri(Office2013LightGrayName, false), LargeGlyph = GetThemeGlyphUri(Office2013LightGrayName, true), ShowInThemeSelector = showTouchThemes };
		public static Theme Office2013DarkGrayTouch = new Theme(Office2013DarkGrayTouchName) { IsStandard = true, FullName = Office2013DarkGrayTouchFullName, Category = Office2013Category, SmallGlyph = GetThemeGlyphUri(Office2013DarkGrayName, false), LargeGlyph = GetThemeGlyphUri(Office2013DarkGrayName, true), ShowInThemeSelector = showTouchThemes };
		public static Theme Office2016White = new Theme(Office2016WhiteName) { IsStandard = true, FullName = Office2016WhiteFullName, Category = Office2016Category, SmallGlyph = GetThemeGlyphUri(Office2016WhiteName, false), LargeGlyph = GetThemeGlyphUri(Office2016WhiteName, true) };
		public static Theme Office2016Black = new Theme(Office2016BlackName) { IsStandard = true, FullName = Office2016BlackFullName, Category = Office2016Category, SmallGlyph = GetThemeGlyphUri(Office2016BlackName, false), LargeGlyph = GetThemeGlyphUri(Office2016BlackName, true) };
		public static Theme Office2016Colorful = new Theme(Office2016ColorfulName) { IsStandard = true, FullName = Office2016ColorfulFullName, Category = Office2016Category, SmallGlyph = GetThemeGlyphUri(Office2016ColorfulName, false), LargeGlyph = GetThemeGlyphUri(Office2016ColorfulName, true) };
		public static Theme Office2016WhiteTouch = new Theme(Office2016WhiteTouchName) { IsStandard = true, FullName = Office2016WhiteTouchFullName, Category = Office2016Category, SmallGlyph = GetThemeGlyphUri(Office2016WhiteName, false), LargeGlyph = GetThemeGlyphUri(Office2016WhiteName, true), ShowInThemeSelector = showTouchThemes };
		public static Theme Office2016BlackTouch = new Theme(Office2016BlackTouchName) { IsStandard = true, FullName = Office2016BlackTouchFullName, Category = Office2016Category, SmallGlyph = GetThemeGlyphUri(Office2016BlackName, false), LargeGlyph = GetThemeGlyphUri(Office2016BlackName, true), ShowInThemeSelector = showTouchThemes };
		public static Theme Office2016ColorfulTouch = new Theme(Office2016ColorfulTouchName) { IsStandard = true, FullName = Office2016ColorfulTouchFullName, Category = Office2016Category, SmallGlyph = GetThemeGlyphUri(Office2016ColorfulName, false), LargeGlyph = GetThemeGlyphUri(Office2016ColorfulName, true), ShowInThemeSelector = showTouchThemes };
#if SL
		public static Theme Default = LightGray;
#else
		public const string NoneName = "None";
		public static Theme Default = DeepBlue;
#endif
#if DEBUGTEST
		static string currentThemeName;
		public static string CurrentThemeName {
			get { return currentThemeName; }
			set { currentThemeName = value; }
		}
#endif
		public static string DefaultThemeName { get { return Default.Name; } }
		public static string DefaultThemeFullName { get { return Default.FullName; } }
		protected static string GetAssemblyGUID(System.Reflection.Assembly asm) {
			if (asm == null) return null;
			object[] attrs = asm.GetCustomAttributes(typeof(GuidAttribute), false);
			if (attrs.Length > 0) {
				GuidAttribute ga = attrs[0] as GuidAttribute;
				return ga != null ? ga.Value : null;
			}
			return null;
		}
		public static string GetBaseThemeName(string themeName) {
			Theme theme = FindTheme(themeName);
			if (theme != null) {
				string cGuid = GetAssemblyGUID(theme.Assembly);
				if (cGuid != null) {
					foreach (Tuple<string, string> tTheme in themeGuids) {
						if (cGuid == tTheme.Item2) return tTheme.Item1;
					}
				}
			}
			return null;
		}
		protected static Tuple<string, string>[] themeGuids = {
			new Tuple<string, string>(LightGrayName,		"D9DA32D6-3BED-4D2F-8CC8-D4E44F9A61BC"),
			new Tuple<string, string>(DeepBlueName,		 "D247B88A-7194-4A9E-8838-7A36C55A5F26"),
			new Tuple<string, string>(Office2007BlackName,  "8CD094E2-96D3-4770-8BBD-95AA0E9E956A"),
			new Tuple<string, string>(Office2007BlueName,   "B366DFA1-88E2-46C5-AAFE-F1EBF4AE86BB"),
			new Tuple<string, string>(Office2007SilverName, "3262723B-7344-4734-B8D4-F927A39A3148"),
			new Tuple<string, string>(SevenName,			"DB2EE31F-95D3-4EFD-B436-3F515CFEBB0F"),
			new Tuple<string, string>(Office2010BlackName,  "D1421BB8-0131-4346-B5FD-307D70E81E80"),
			new Tuple<string, string>(Office2010BlueName,   "7F6339CA-9E31-4E8D-8293-C26917B0E127"),
			new Tuple<string, string>(Office2010SilverName, "343F52C3-8844-4FBD-8F05-C6DF882FADBD"),
			new Tuple<string, string>(MetropolisDarkName,   "88DCD2D6-C501-4355-836D-AC3A438243C8"),
			new Tuple<string, string>(TouchlineDarkName,   "B4E676E4-4F37-431C-92EF-4D6F808417F1"),
			new Tuple<string, string>(Office2013Name,		 "3B40F326-8562-4B44-B46F-B2935D284057"),
			new Tuple<string, string>(HybridAppName,		 "B722B70F-BD3D-46B9-98A4-4EFCB5741B17"),
			new Tuple<string, string>(MetropolisLightName,  "82F48483-C795-416A-9307-9C6750F5DEBA"),
			new Tuple<string, string>(Office2013LightGrayName,  "672667F5-5095-4E47-A21E-220FA569B6FD"),
			new Tuple<string, string>(Office2013DarkGrayName,  "C3C62B38-CB06-4E43-B419-332B6AC59DE9"),
			new Tuple<string, string>(DXStyleName,		  "ED998E8A-6699-4665-ACB1-7CF9D6E99865"),
			new Tuple<string, string>(VS2010Name,		   "0A27036E-072F-4949-8AAF-BFB30628DEFB"),
			Tuple.Create(Office2016WhiteName, "BAEDFC31-BDBF-4DDF-98E1-A24F324EE9FC"),
			Tuple.Create(Office2016BlackName, "295E625A-2DD6-4475-B3F9-17327FD7EE53"),
			Tuple.Create(Office2016ColorfulName, "7FB73C8C-18D0-4BBC-A718-BE3A4200DD84"),
		};
		public static System.Collections.ObjectModel.ReadOnlyCollection<Theme> Themes { get { return ThemesInternal.AsReadOnly(); } }
		static Theme() {
			RegisterTheme(DXStyle);
			RegisterTheme(Office2016White);
			RegisterTheme(Office2016Black);
			RegisterTheme(Office2016Colorful);
			RegisterTheme(Office2016WhiteTouch);
			RegisterTheme(Office2016BlackTouch);
			RegisterTheme(Office2016ColorfulTouch);
			RegisterTheme(Office2013);
			RegisterTheme(Office2013DarkGray);
			RegisterTheme(Office2013LightGray);
			RegisterTheme(Office2013Touch);
			RegisterTheme(Office2013DarkGrayTouch);
			RegisterTheme(Office2013LightGrayTouch);
			RegisterTheme(Office2010Black);
			RegisterTheme(Office2010Blue);
			RegisterTheme(Office2010Silver);
			RegisterTheme(MetropolisLight);
			RegisterTheme(MetropolisDark);
			RegisterTheme(VS2010);
			RegisterTheme(Office2007Black);
			RegisterTheme(Office2007Blue);
			RegisterTheme(Office2007Silver);
			RegisterTheme(Seven);
			RegisterTheme(TouchlineDark);
			RegisterTheme(HybridApp);
			RegisterTheme(DeepBlue);
			RegisterTheme(LightGray);
		}
		static Uri GetThemeGlyphUri(string themeName, bool isLarge) {
			string smallGlyphSuffix = "_16x16.png";
			string largeGlyphSuffix = "_48x48.png";
			string glyphPrefix = string.Format(@"/{0};component/Themes/Images/", AssemblyInfo.SRAssemblyXpfCore);
			string suffix = isLarge ? largeGlyphSuffix : smallGlyphSuffix;
			return new Uri(glyphPrefix + themeName + suffix, UriKind.Relative);
		}
#if SL
		public static readonly DependencyProperty ThemeProperty =
			DependencyProperty.RegisterAttached("Theme", typeof(Theme), typeof(Theme),
				new PropertyMetadata((o, e) => OnThemeChanged(o as Style, (Theme)e.NewValue)));
		public static Theme GetTheme(Style style) {
			return (Theme)style.GetValue(ThemeProperty);
		}
		public static void SetTheme(Style style, Theme value) {
			style.SetValue(ThemeProperty, value);
		}
		protected internal List<ProductResourceDictionary> deferredProductResourceDictionaries;
#endif
		public static Theme FindTheme(string name) {
			if (name == null)
				name = "";
			Theme theme;
			if (!ThemeNameHash.TryGetValue(name, out theme)) {
				ThemeNameHash.Add(name, FindThemeCore(name));
			}
			return ThemeNameHash[name];
		}
		static Theme FindThemeCore(string name) {
			if (name == "Default")
				return Theme.Default;
			foreach (Theme theme in ThemesInternal)
				if (theme.Name == name)
					return theme;
			return null;
		}
		public static void RegisterTheme(Theme theme) {
			if (FindTheme(theme.name) != null)
				throw new ArgumentException("A theme with the same name already exists");
			ThemeNameHash.Clear();
			ThemesInternal.Add(theme);
		}
#if SL
		ResourceDictionary styles;
		ResourceDictionary addedStyles = new ResourceDictionary();
		IEnumerable<object> styleKeys;
		Uri sourceUri = null;
		public Theme(string name) : this(name, name, null) { }
		public Theme(string name, Uri sourceUri) : this(name, name, sourceUri) { }
		public Theme(string name, string fullName, Uri sourceUri, string category = StandardCategory, Uri smallGlyphUri = null, Uri largeGlyphUri = null) {
			SourceUri = sourceUri;
			Initialize(name, fullName, category, smallGlyphUri, largeGlyphUri);
		}
#else
		public Theme(string name) : this(name, name) { }
		public Theme(string name, string fullName, string category = StandardCategory, Uri smallGlyphUri = null, Uri largeGlyphUri = null) {
			Initialize(name, fullName, category, smallGlyphUri, largeGlyphUri);
		}
#endif
		Assembly assembly = null;
		string publicKeyToken;
		string version;
		string assemblyName;
		public Uri SmallGlyph { get; private set; }
		public Uri LargeGlyph { get; private set; }
		public string Category { get; private set; }
		public string AssemblyName {
			get {
				if (assemblyName == null) {
					assemblyName = GetAssemblyName();
				}
				return assemblyName;
			}
			set {
				if (assemblyName == value)
					return;
				assemblyName = value;
				ResetAssembly();
			}
		}
		public Assembly Assembly {
			get {
				if (assembly == null) {
					assembly = GetAssembly();
				}
				return assembly;
			}
		}
		public bool GlobalAssemblyCache {
			get {
				return !(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Version) || string.IsNullOrEmpty(PublicKeyToken));
			}
		}
		public string PublicKeyToken {
			get { return publicKeyToken; }
			set {
				if (publicKeyToken == value)
					return;
				publicKeyToken = value;
				ResetAssembly();
			}
		}
		public string Version {
			get { return version; }
			set {
				if (version == value)
					return;
				version = value;
				ResetAssembly();
			}
		}
		protected virtual Assembly GetAssembly() {
			if (string.IsNullOrEmpty(Name))
				return null;
#if SL
			if (this.FullName == LightGrayFullName) return null;
#endif
			return AssemblyHelper.GetAssembly(AssemblyName);
		}
		protected void ResetAssembly() {
			this.assembly = null;
		}
		protected virtual string GetAssemblyName() {
			if (Name == DeepBlue.Name) {
				return typeof(Theme).Assembly.FullName;
			}
			if (IsStandard) {
				return AssemblyHelper.GetThemeAssemblyFullName(Name);
			}
			if (GlobalAssemblyCache) {
				return AssemblyHelper.GetAssemblyFullName(Name, Version, CultureInfo.InvariantCulture, PublicKeyToken);
			}
			return Name;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsStandard { get; set; }
		public string FullName {
			get {
				if(fullName == null)
					return Name;
				return fullName;
			}
			set {
				if(fullName == value)
					return;
				fullName = value;
			}
		}
		public string Name {
			get { return name; }
			private set {
				if(string.IsNullOrEmpty(value))
					throw new ArgumentException("The Name property can not be empty");
				name = value;
			}
		}
		internal string AssemblyPartialName {
			get {
#if SL
				return GetAssemblyName(SourceUri);
#else
				return AssemblyHelper.GetPartialName(Assembly);
#endif
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void RemoveAllCustomThemes() {
			ThemeNameHash.Clear();
			for(int i = ThemesInternal.Count - 1; i >= 0; i--) {
				if(!ThemesInternal[i].IsStandard) {
					ThemesInternal.RemoveAt(i);
				}
			}
		}
		public static bool IsDefaultTheme(string themeName) {
			return string.IsNullOrEmpty(themeName) || themeName == DefaultThemeName;
		}
		public override string ToString() {
			return FullName;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void RemoveTheme(string name) {
			Theme theme = FindTheme(name);
			if(theme == null)
				return;
			ThemeNameHash.Clear();
			ThemesInternal.Remove(theme);
		}
		protected void Initialize(string name, string fullName, string category, Uri smallGlyphUri, Uri largeGlyphUri) {
			Name = name;
			FullName = fullName;
			Category = category;
			SmallGlyph = smallGlyphUri;
			LargeGlyph = largeGlyphUri;
		}
#if SL
		public Uri SourceUri {
			get {
				if(sourceUri == null) {
					sourceUri = GetThemeUri(Name);
				}
				return sourceUri;
			}
			private set { sourceUri = value; }
		}
		public ResourceDictionary Styles {
			get {
				if (styles == null) {
					styles = GetResourceDictionary(SourceUri);
					AppendCustomStyles(styles);
					styles.Add(DXThemeKey, Name);
				}
				return styles;
			}
		}
		public bool IsLoaded { get { return styles != null; } }
		ResourceDictionary AddedStyles { get { return addedStyles; } }
		static string GetAssemblyName(Uri uri){
			if(uri == null || string.IsNullOrEmpty(uri.OriginalString))
				return null;
			return uri.OriginalString.Split(';')[0].TrimStart('/');
		}
		public void AddStyle(Style style) {
			if(Styles.Contains(style.TargetType))
				Styles.Remove(style.TargetType);
			Styles.Add(style.TargetType, style);
		}
		public void Apply(FrameworkElement element) {
			ApplyCore(element, GetThemedTypesInSubTree(element));
		}
		List<Tuple<Type, object>> applicationTypes = new List<Tuple<Type, object>>();
		private void ApplyCore(FrameworkElement element) {
			ApplyCore(element, applicationTypes);
		}
		private void ApplyCore(FrameworkElement element, List<Tuple<Type, object>> types) {
			foreach(Tuple<Type, object> temp in types)
				UpdateStyle(temp.Item1, temp.Item2);
		}
		protected internal void GetThemedTypes(FrameworkElement root){
			applicationTypes = GetThemedTypesInSubTree(root);
		}
		List<Tuple<Type, object>> GetThemedTypesInSubTree(FrameworkElement root) {
			List<Tuple<Type, object>> types = new List<Tuple<Type, object>>();
			if (root == null)
				return types;
			if (ThemeManager.GetIsThemeIndependent(root))
				return types;
			types.Add(new Tuple<Type, object>(root.GetType(), GetStyleKey(root)));
			int count = VisualTreeHelper.GetChildrenCount(root);
			for (int i = 0; i < count; i++) {
				var child = VisualTreeHelper.GetChild(root, i) as FrameworkElement;
				if (child != null) GetThemedTypesInSubTree(child);
			}
			return types;
		}
		protected virtual object GetStyleKey(FrameworkElement element) {
			return DefaultStyleKeyHelper.GetControlDefaultStyleKey(element as Control) ?? element.GetType();
		}
		protected virtual void OnElementSizeChanged(object sender, SizeChangedEventArgs e) {
			FrameworkElement element = (FrameworkElement)sender;
			if(element == null)
				return;
			element.SizeChanged -= OnElementSizeChanged;
			Apply(element);
		}
		protected internal bool PrepareDeferredLoading() {
			if(styles == null) return false;
			styleKeys = null;
			return true;
		}
		protected internal void ReplaceStyles(ResourceDictionary rd) {
			foreach(object key in StyleKeys) {
				object style = Styles[key];
				if(rd.Contains(key)) {
					rd.Remove(key);
					rd.Add(key, style);
				}
			}
		}
		protected void UpdateStyle(Type elementType, object styleKey) {
			Style style = GetStyle(elementType);
			if(style == null && styleKey != elementType) {
				style = GetStyle(styleKey);
				if (style != null) {
					var newStyle = new Style(elementType);
					newStyle.BasedOn = style;
					Styles.Add(newStyle.TargetType, newStyle);
				}
			}
		}
		protected IEnumerable<object> StyleKeys { get { return styleKeys ?? (styleKeys = GetStyleKeys()); } }
		Style GetStyle(object key) {
			if(key == null)
				throw new ArgumentNullException("key");
			if(Styles != null) {
				var style = Styles[key] as Style;
				if(style != null && !AgStyleStorage.GetIsThemeIndependent(style))
					return style;
			}
			return null;
		}
		void AddStyleDelayed(Style style) {
			if(AddedStyles.Contains(style.TargetType))
				AddedStyles.Remove(style.TargetType);
			AddedStyles.Add(style.TargetType, style);
		}
		void AppendCustomStyles(ResourceDictionary styles) {
			foreach(System.Collections.DictionaryEntry keyValuePair in AddedStyles) {
				styles.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}
		IEnumerable<object> GetStyleKeys(ResourceDictionary dic) {
			List<object> res = new List<object>();
			foreach(object key in dic.Keys) {
				Style s = dic[key] as Style;
				if(s != null && !AgStyleStorage.GetIsThemeIndependent(s))
					res.Add(key);
			}
			foreach(ResourceDictionary mdic in dic.MergedDictionaries) {
				res.AddRange(GetStyleKeys(mdic));
			}
			return res;
		}
		IEnumerable<object> GetStyleKeys() {
			return GetStyleKeys(Styles);
		}
		internal static ResourceDictionary LoadingResourceDictionary;
		static ResourceDictionary GetResourceDictionary(Uri uri) {
			AgStyleStorage.Lock();
			try {
				LoadingResourceDictionary = new ResourceDictionary();
				LoadingResourceDictionary.Source = uri;
				ResourceDictionary resourceDictionary = new ResourceDictionary();
				resourceDictionary.MergedDictionaries.Add(LoadingResourceDictionary);
				return resourceDictionary;
			}
			catch (Exception exception) {
				string assemblyName = GetAssemblyName(uri) + ".dll";
				throw new Exception(
					string.Format("Theme resource was not found. Did you forget to reference a theme assembly ({0}) in your project?", assemblyName), exception);
			}
			finally {
				AgStyleStorage.Unlock();
				LoadingResourceDictionary = null;
			}
		}
		Uri GetThemeUri(string themeName) {
			if(IsStandard) {
				return new Uri(GetDefaultEntryPointString("DevExpress.Xpf.Themes.", themeName, AssemblyInfo.VSuffix), UriKind.Relative);
			}
			return new Uri(GetDefaultEntryPointString("", themeName, ""), UriKind.Relative);
		}
		string GetDefaultEntryPointString(string prefix, string themeName, string vSuffix) {
			return string.Format("/{0}{1}{2};component/theme.xaml", prefix, themeName, vSuffix);
		}
		static void OnThemeChanged(Style style, Theme theme) {
			if(style != null && theme != null)
				theme.AddStyleDelayed(style);
		}
#endif
	}
	public class ThemeTypeConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			var themeName = value as string;
			if(themeName == null)
				return base.ConvertFrom(context, culture, value);
			Theme theme = Theme.FindTheme(themeName);
			if(theme == null)
				throw new ArgumentOutOfRangeException("theme");
			return theme;
		}
	}
	public static class DefaultStyleKeyHelper {
		public static Type GetControlDefaultStyleKey(Control control) {
			if(control == null)
				return null;
			return (Type)DefaultStyleKeyControlHelper.GetDefaultStyleKey(control);
		}
#if !SL
		public static void SetDefaultStyleKey(this FrameworkElement element, object value) {
			DefaultStyleKeyFrameworkElementHelper.SetDefaultStyleKey(element, value);
		}
		public static void SetDefaultStyleKey(this FrameworkContentElement element, object value) {
			DefaultStyleKeyFrameworkContentElementHelper.SetDefaultStyleKey(element, value);
		}
		public static object GetDefaultStyleKey(this FrameworkElement element) {
			return DefaultStyleKeyFrameworkElementHelper.GetDefaultStyleKey(element);
		}
		public static object GetDefaultStyleKey(this FrameworkContentElement element) {
			return DefaultStyleKeyFrameworkContentElementHelper.GetDefaultStyleKey(element);
		}
		public static object ClearDefaultStyleKey(this FrameworkElement element) {
			return DefaultStyleKeyFrameworkElementHelper.GetDefaultStyleKey(element);
		}
		public static object ClearDefaultStyleKey(this FrameworkContentElement element) {
			return DefaultStyleKeyFrameworkContentElementHelper.GetDefaultStyleKey(element);
		}
#endif
	}
	public class DefaultStyleKeyControlHelper : Control {
		public static object GetDefaultStyleKey(Control element) {
			return element.GetValue(Control.DefaultStyleKeyProperty);
		}
	}
#if !SL
	public class ThemeNameTypeConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<string> themeNames = new List<string>(Theme.Themes.Count);
			foreach(Theme theme in Theme.Themes) {
				if (theme.ShowInThemeSelector)
					themeNames.Add(theme.Name);
			}
			themeNames.Add(Theme.NoneName);
			return new StandardValuesCollection(themeNames);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string) {
				return value;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class DefaultStyleKeyFrameworkElementHelper : FrameworkElement {
		public static void SetDefaultStyleKey(FrameworkElement element, object value) {
			element.SetValue(FrameworkElement.DefaultStyleKeyProperty, value);
		}
		public static object GetDefaultStyleKey(FrameworkElement element) {
			return element.GetValue(FrameworkElement.DefaultStyleKeyProperty);
		}
	}
	public class DefaultStyleKeyFrameworkContentElementHelper : FrameworkContentElement {
		public static void SetDefaultStyleKey(FrameworkContentElement element, object value) {
			element.SetValue(FrameworkContentElement.DefaultStyleKeyProperty, value);
		}
		public static object GetDefaultStyleKey(FrameworkContentElement element) {
			return element.GetValue(FrameworkContentElement.DefaultStyleKeyProperty);
		}
	}
	public class ThemeExtension : System.Windows.Markup.MarkupExtension {
		public ThemeExtension() { }
		public string Name { get; set; }
		public string AssemblyName { get; set; }
		public string Version { get; set; }
		public string PublicKeyToken { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			Theme existantTheme = Theme.FindTheme(Name);
			if(existantTheme != null)
				return existantTheme;
			Theme theme = new Theme(Name) { AssemblyName = this.AssemblyName, PublicKeyToken = this.PublicKeyToken, Version = this.Version };
			Theme.RegisterTheme(theme);
			return theme;
		}
	}
#endif
	public class XamlThemeProvider : INotifyPropertyChanged {
		public string ThemeName { get; private set; }
		public XamlThemeProvider() {
			ThemeName = ThemeManager.ActualApplicationThemeName;
#if !SL
			ThemeManager.AddThemeChangedHandler(Application.Current.MainWindow, ThemeManager_ThemeChanged);
#else
			ThemeManager.AddThemeChangedHandler(null, ThemeManager_ThemeChanged);
#endif
		}
		void ThemeManager_ThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
			ThemeName = ThemeManager.ActualApplicationThemeName;
			RaisePropertyChanged("ThemeName");
		}
		void RaisePropertyChanged(string name) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}

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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Core {
	[AttributeUsage(AttributeTargets.Assembly)]
	public class DXThemeInfoAttribute : Attribute { }
	public static class ApplicationThemeHelper {
		static bool isInitialized;
		public static object GetAssignApplicationThemeName(ResourceDictionary d) {
			return isInitialized;
		}
		public static void SetAssignApplicationThemeName(ResourceDictionary d, object value) {
			if (isInitialized)
				return;
			isInitialized = true;
			Application app = Application.Current;
			if (app == null)
				return;
			UpdateApplicationThemeName();
		}
		public static void UpdateApplicationThemeName() {
			if (!string.IsNullOrEmpty(ThemeManager.ApplicationThemeName))
				return;
			string themeNameFromConfig = ConfigurationHelper.GetThemeNameFromConfig();
			if (string.IsNullOrEmpty(themeNameFromConfig))
				return;
			if (DesignerProperties.GetIsInDesignMode(new Button()))
				return;
			ThemeManager.ApplicationThemeName = themeNameFromConfig;
		}
		public static void SaveApplicationThemeName() {
			try {
				if(string.IsNullOrEmpty(ThemeManager.ApplicationThemeName))
					return;
				ConfigurationHelper.SaveThemeNameToConfig(ThemeManager.ApplicationThemeName);
			} catch { }
		}
	}
	public class BlendHelper2 : DependencyObject {
		public static string GetThemeInfo(DependencyObject obj) {
			return (string)obj.GetValue(ThemeInfoProperty);
		}
		public static void SetThemeInfo(DependencyObject obj, string value) {
			obj.SetValue(ThemeInfoProperty, value);
		}
		public static readonly DependencyProperty ThemeInfoProperty =
			DependencyProperty.RegisterAttached("ThemeInfo", typeof(string), typeof(BlendHelper2), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.Inherits));
	}
	public static class StyleManager {
		public static readonly DependencyProperty ApplyApplicationThemeProperty =
			DependencyProperty.RegisterAttached("ApplyApplicationTheme", typeof(bool), typeof(StyleManager), new PropertyMetadata());
		public static bool GetApplyApplicationTheme(FrameworkElement element) {
			return (bool)element.GetValue(ApplyApplicationThemeProperty);
		}
		public static void SetApplyApplicationTheme(FrameworkElement element, bool value) {
			element.SetValue(ApplyApplicationThemeProperty, value);
		}
	}
	public class ResourceFinder { }
	public class ThemeTreeWalker {
		readonly WeakReference wRef;
		readonly InplaceResourceProvider inplaceResourceProvider;
		protected internal ThemeTreeWalker(string themeName, bool isTouch, DependencyObject owner) {
			this.wRef = new WeakReference(owner);
			ThemeName = themeName;
			IsTouch = isTouch;
			inplaceResourceProvider = new InplaceResourceProvider(ThemeHelper.GetTreewalkerThemeName(this, false));
		}
		public DependencyObject Owner { get { return wRef.Target as DependencyObject; } }
		public string ThemeName { get; protected set; }
		public InplaceResourceProvider InplaceResourceProvider { get { return inplaceResourceProvider; } }
		public override string ToString() {
			return ThemeName;
		}
		public bool IsTouch { get; private set; }
		public bool IsDefault { get { return Theme.IsDefaultTheme(ThemeName); } }
		public bool IsRegistered { get { return Theme.FindTheme(ThemeName) != null; } }
	}
	public static class DependencyPropertyHelper2 {
		volatile static bool methodsCreated = false;
		static readonly object lockObject = new object();
		static Func<object, object> getSynchronized;
		static Func<object, object> get_metadataMap;
		static Action<object, int, object> set_Item;
		static Func<object, int, object> get_Item;
		static Action<object, object> set_propertyChangedCallback;
		static void CheckMethodsCreated(DependencyProperty property) {
			if (methodsCreated)
				return;
			lock (lockObject) {
				if (methodsCreated)
					return;
				var dPropType = typeof(DependencyProperty);
				getSynchronized = ReflectionHelper.CreateFieldGetter<object, object>(dPropType, "Synchronized", BindingFlags.Static | BindingFlags.NonPublic);
				get_metadataMap = ReflectionHelper.CreateFieldGetter<object, object>(dPropType, "_metadataMap", BindingFlags.NonPublic | BindingFlags.Instance);
				var mmap = get_metadataMap(property);
				set_Item = ReflectionHelper.CreateInstanceMethodHandler<Action<object, int, object>>(mmap, "set_Item", BindingFlags.Public | BindingFlags.Instance, mmap.GetType());
				get_Item = ReflectionHelper.CreateInstanceMethodHandler<Func<object, int, object>>(mmap, "get_Item", BindingFlags.Public | BindingFlags.Instance, mmap.GetType());
				set_propertyChangedCallback = ReflectionHelper.CreateFieldSetter<object, object>(typeof(PropertyMetadata), "_propertyChangedCallback", BindingFlags.Instance | BindingFlags.NonPublic);
				methodsCreated = true;
			}
		}
		public static void UnOverrideMetadata(this DependencyProperty property, Type ownerType) {
			CheckMethodsCreated(property);
			lock (getSynchronized(null))
				set_Item(get_metadataMap(property), DependencyObjectType.FromSystemType(ownerType).Id, DependencyProperty.UnsetValue);
		}
		public static bool IsMetadataOverriden(this DependencyProperty property, Type ownerType) {
			CheckMethodsCreated(property);
			lock (getSynchronized(null))
				return get_Item(get_metadataMap(property), DependencyObjectType.FromSystemType(ownerType).Id) != DependencyProperty.UnsetValue;
		}
		public static void AddPropertyChangedCallback(this DependencyProperty property, Type ownerType, PropertyChangedCallback callback) {
			if (!property.IsMetadataOverriden(ownerType)) {
				property.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(callback));
			} else {
				var metadata = property.GetMetadata(ownerType);
				lock (metadata) {
					var newCallback = metadata.PropertyChangedCallback == null ? callback : Delegate.Combine(metadata.PropertyChangedCallback, callback);
					set_propertyChangedCallback(metadata, newCallback);
				}				
			}
		}
	}
	public class ThemeManager : DependencyObject, IThemeManager {
		public const string TraceSwitchName = "ThemeManagerTracing";
		public const string TouchDelimiter = ";";
		public const string TouchDefinition = "touch";
		public const string TouchDelimiterAndDefinition = ";touch";
		internal const string MethodNameString = "MethodName";
		internal const string ThemeNameTraceString = "ThemeName";
		internal const string AssemblyNameTraceString = "AssemblyName";
		internal const string KeyTraceString = "Key";
		internal const string ObjectTraceString = "Object";
		[IgnoreDependencyPropertiesConsistencyChecker()]
		public static readonly DependencyProperty ThemeProperty;
		[IgnoreDependencyPropertiesConsistencyChecker()]
		public static readonly DependencyProperty ThemeNameProperty;
		[IgnoreDependencyPropertiesConsistencyChecker()]
		public static readonly DependencyProperty TreeWalkerProperty;
		public static readonly DependencyProperty IsTouchEnabledProperty;
		static readonly DependencyPropertyKey IsTouchEnabledPropertyKey;
		public static readonly RoutedEvent ThemeChangedEvent;
		public static readonly RoutedEvent ThemeChangingEvent;
		public static event ThemeChangingRoutedEventHandler ApplicationThemeChanging;
		public static event ThemeChangedRoutedEventHandler ApplicationThemeChanged;
		public static event ThemeChangedRoutedEventHandler ThemeChanged;
		public static event ThemeChangingRoutedEventHandler ThemeChanging;
		public static bool? EnableDPICorrection { get; set; }
		static readonly BooleanSwitch traceSwitch;
		static readonly List<string> pluginAssemblies = new List<string>();
		static bool ignoreManifest = false;
		static ThemeManager instance;
		static AssemblyLoaderProxy loader;
		static double defaultTouchPaddingScale = 2;
		public static double DefaultTouchPaddingScale {
			get { return defaultTouchPaddingScale; }
			set { defaultTouchPaddingScale = Math.Max(0.1, value); }
		}
		static AssemblyLoaderProxy Loader {
			get {
				if (loader == null) {
					loader = AssemblyLoaderProxy.Instance;
					loader.CompletedCallback = ThemeLoaded;
					loader.ProgressChangedCallback = ThemeLoadingProgress;
				}
				return loader;
			}
		}
		static void ThemeLoaded(object assembly) { }
		static void ThemeLoadingProgress(double progress) { }
		[SecuritySafeCritical]
		static ThemeManager() {
			Type ownerType = typeof(ThemeManager);
			ThemeNameProperty = DependencyProperty.RegisterAttached("ThemeName", typeof(string), ownerType, new FrameworkPropertyMetadata(null, ThemeNamePropertyChanged));
			ThemeProperty = DependencyProperty.RegisterAttached("Theme", typeof(Theme), ownerType, new FrameworkPropertyMetadata(null, ThemePropertyChanged));
			DependencyProperty dp = TextBlock.ForegroundProperty;
			IsTouchEnabledPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsTouchEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
			IsTouchEnabledProperty = IsTouchEnabledPropertyKey.DependencyProperty;
			TreeWalkerProperty = DependencyProperty.RegisterAttached("TreeWalker", typeof(ThemeTreeWalker), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure, TreeWalkerChanged));
			ThemeChangedEvent = EventManager.RegisterRoutedEvent("ThemeChanged", RoutingStrategy.Direct, typeof(ThemeChangedRoutedEventHandler), ownerType);
			ThemeChangingEvent = EventManager.RegisterRoutedEvent("ThemeChanging", RoutingStrategy.Direct, typeof(ThemeChangingRoutedEventHandler), ownerType);
			traceSwitch = new BooleanSwitch(TraceSwitchName, "");
			if (!System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted) {
				AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
			}
		}
		public static bool GetIsTouchEnabled(DependencyObject d) {
			return (bool)d.GetValue(IsTouchEnabledProperty);
		}
		internal static void SetIsTouchEnabled(DependencyObject d, bool value) {
			d.SetValue(IsTouchEnabledPropertyKey, value);
		}
		protected ThemeManager() { }
		internal static ThemeManager Instance {
			get {
				if (instance == null) {
					instance = new ThemeManager();
				}
				return instance;
			}
		}
		static void EnableResourcesForLoadedAssemblies() {
			if (System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted) {
				IEnumerable<string> assemblies = ApplicationHelper.GetAvailableAssemblies();
				foreach (string assemblyName in assemblies) {
					if (AssemblyHelper.IsDXProductAssembly(assemblyName))
						EnableResource(AssemblyHelper.GetShortNameWithoutVersion(assemblyName));
				}
			}
			else {
				IEnumerable<Assembly> assemblies = AssemblyHelper.GetLoadedAssemblies();
				foreach (Assembly assembly in assemblies) {
					if (AssemblyHelper.IsDXProductAssembly(assembly))
						EnableResource(AssemblyHelper.GetShortNameWithoutVersion(assembly));
				}
			}
		}
		static bool EnableResource(string assemblyName) {
			return ThemePartResourceDictionary.EnableSource(new ThemePartKeyExtension() { AssemblyName = assemblyName });
		}
		[SecuritySafeCritical]
		static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args) {
			if (AssemblyHelper.IsDXProductAssembly(args.LoadedAssembly)) {
				EnableResource(AssemblyHelper.GetShortNameWithoutVersion(args.LoadedAssembly));
			}
			if (ThemedElementsDictionary.IsCustomThemeAssembly(args.LoadedAssembly)) {
				ThemedElementsDictionary.ForceThemeKeysLoadingForAssembly(Theme.Default.Name, args.LoadedAssembly.FullName);
			}
		}
		internal static BooleanSwitch TraceSwitch { get { return traceSwitch; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ThemeManagerApplicationThemeName")]
#endif
		[TypeConverter(typeof(ThemeNameTypeConverter))]
		public static string ApplicationThemeName {
			get { return GlobalThemeHelper.Instance.ApplicationThemeName; }
			set {
				if (ApplicationThemeChanging != null)
					ApplicationThemeChanging(null, new ThemeChangingRoutedEventArgs(value));
				GlobalThemeHelper.Instance.ApplicationThemeName = value;
				if (ApplicationThemeChanged != null)
					ApplicationThemeChanged(null, new ThemeChangedRoutedEventArgs(value));
			}
		}
		static bool enableDefaultThemeLoadingCore = false;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ThemeManagerEnableDefaultThemeLoading")]
#endif
		public static bool EnableDefaultThemeLoading {
			get { return enableDefaultThemeLoadingCore; }
			set {
				enableDefaultThemeLoadingCore = value;
			}
		}
		[Browsable(false)]
		public static List<string> PluginAssemblies { get { return pluginAssemblies; } }
		[Browsable(false)]
		public static bool IgnoreManifest {
			get { return ignoreManifest; }
			set { ignoreManifest = value; }
		}
		[Browsable(false)]
		public static string ActualApplicationThemeName { get { return ApplicationThemeName; } }
		public static ThemeTreeWalker GetTreeWalker(DependencyObject obj) {
			return (ThemeTreeWalker)obj.GetValue(TreeWalkerProperty);
		}
		static void SetTreeWalker(DependencyObject d, ThemeTreeWalker value) {
			d.SetValue(TreeWalkerProperty, value);
		}
		public static Theme GetTheme(DependencyObject obj) {
			return (Theme)obj.GetValue(ThemeProperty);
		}
		public static void SetTheme(DependencyObject obj, Theme value) {
			obj.SetValue(ThemeProperty, value);
		}
		[TypeConverter(typeof(ThemeNameTypeConverter))]
		public static string GetThemeName(DependencyObject obj) {
			return (string)obj.GetValue(ThemeNameProperty);
		}
		public static void SetThemeName(DependencyObject obj, string value) {
			TraceHelper.Write(TraceSwitch,
				MethodNameString, MethodInfo.GetCurrentMethod().Name,
				ThemeNameTraceString, value,
				ObjectTraceString, obj);
			obj.SetValue(ThemeNameProperty, value);
		}
		public static void AddThemeChangedHandler(DependencyObject d, ThemeChangedRoutedEventHandler handler) {
			DependencyObjectWrapper wrapper = new DependencyObjectWrapper(d);
			wrapper.AddHandler(ThemeChangedEvent, handler);
		}
		public static void RemoveThemeChangedHandler(DependencyObject d, ThemeChangedRoutedEventHandler handler) {
			DependencyObjectWrapper wrapper = new DependencyObjectWrapper(d);
			wrapper.RemoveHandler(ThemeChangedEvent, handler);
		}
		public static void AddThemeChangingHandler(DependencyObject d, ThemeChangingRoutedEventHandler handler) {
			DependencyObjectWrapper wrapper = new DependencyObjectWrapper(d);
			wrapper.AddHandler(ThemeChangingEvent, handler);
		}
		public static void RemoveThemeChangingHandler(DependencyObject d, ThemeChangingRoutedEventHandler handler) {
			DependencyObjectWrapper wrapper = new DependencyObjectWrapper(d);
			wrapper.RemoveHandler(ThemeChangingEvent, handler);
		}
		static void RaiseThemeNameChanging(DependencyObject obj, string themeName) {
			DependencyObjectWrapper wrapper = new DependencyObjectWrapper(obj);
			ThemeChangingRoutedEventArgs eventArgs = new ThemeChangingRoutedEventArgs(themeName) { RoutedEvent = ThemeChangingEvent };
			wrapper.RaiseEvent(eventArgs);
		}
		static void RaiseThemeNameChanged(DependencyObject obj, string themeName) {
			DependencyObjectWrapper wrapper = new DependencyObjectWrapper(obj);
			ThemeChangedRoutedEventArgs eventArgs = new ThemeChangedRoutedEventArgs(themeName) { RoutedEvent = ThemeChangedEvent };
			wrapper.RaiseEvent(eventArgs);
		}
		static void TreeWalkerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			ThemeTreeWalker walker = e.NewValue as ThemeTreeWalker;
			if (walker == null)
				return;
			DependencyObjectWrapper wrapper = new DependencyObjectWrapper(obj);
			if (ShouldExcludeFromTheming(wrapper.Object)) {
				ExcludeFromTheming(wrapper.Object);
				return;
			}
			ThemeTreeWalker oldWalker = e.OldValue as ThemeTreeWalker;
			string oldThemeName = oldWalker != null ? oldWalker.ThemeName : string.Empty;
			string themeName = walker != null ? walker.ThemeName : string.Empty;
			if (string.IsNullOrEmpty(oldThemeName)) {
				string typeName = GetTypeNameFromKey(wrapper);
				if (typeName != null) {
					object key = wrapper.GetDefaultStyleKey();
					ThemedElementsDictionary.RegisterThemeType(string.Empty, typeName, key);
				}
			}
			UpdateDefaultStyleKey(wrapper, themeName);
		}
		static void ExcludeFromTheming(DependencyObject obj) {
			SetTreeWalker(obj, null);
		}
		static bool ShouldExcludeFromTheming(DependencyObject obj) {
			return obj.GetType() == typeof(DataGrid);
		}
		static void ThemePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			Theme newValue = e.NewValue as Theme;
			SetThemeName(obj, newValue != null ? newValue.Name : null);
		}
		static void ThemeNamePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			string oldThemeName = (string)e.OldValue;
			string rawThemeName = (string)e.NewValue;
			string themeName = GetFromAlias(rawThemeName);
			bool isTouch = IsTouch(rawThemeName);
			obj.ClearValue(GlobalThemeHelper.IsGlobalThemeNameProperty);
			BeforeThemeNameChanged(obj, themeName);
			if (string.IsNullOrEmpty(themeName)) {
				ClearTreeWalker(obj);
				AfterThemeNameChanged(obj, themeName);
				return;
			}
			if (themeName == Theme.NoneName) {
				themeName = string.Empty;
			}
			ChangeTheme(obj, themeName, isTouch, oldThemeName);
			AfterThemeNameChanged(obj, themeName);
		}
		static void BeforeThemeNameChanged(DependencyObject obj, string themeName) {
			if (ThemeChanging == null)
				return;
			ThemeChangingRoutedEventArgs eventArgs = new ThemeChangingRoutedEventArgs(themeName) { RoutedEvent = ThemeChangingEvent };
			ThemeChanging(obj, eventArgs);
		}
		static void AfterThemeNameChanged(DependencyObject obj, string themeName) {
			if (ThemeChanged == null)
				return;
			ThemeChangedRoutedEventArgs eventArgs = new ThemeChangedRoutedEventArgs(themeName) { RoutedEvent = ThemeChangedEvent };
			ThemeChanged(obj, eventArgs);
		}
		static void ClearTreeWalker(DependencyObject obj) {
			SetTreeWalker(obj, new ThemeTreeWalker("", false, obj));
			obj.ClearValue(TreeWalkerProperty);
			if (obj is Window) {
				GlobalThemeHelper.Instance.SetWindowsApplicationThemeNameInThread();
			}
		}
		static string GetFromAlias(string themeName) {
			if (themeName == Theme.AzureName)
				return Theme.DeepBlueName;
			if (string.IsNullOrEmpty(themeName) || themeName.IndexOf(TouchDelimiter, System.StringComparison.Ordinal) < 0)
				return themeName;
			return themeName.Split(new[] { TouchDelimiter }, StringSplitOptions.RemoveEmptyEntries).First();
		}
		static readonly object LockObject = new object();
		internal static void ChangeTheme(DependencyObject obj, string themeName, bool isTouch, string oldThemeName) {
			lock (LockObject) {
				ForceThemeKeyCreating(themeName);
				SetTreeWalker(obj, new ThemeTreeWalker(themeName, isTouch, obj));
				SetIsTouchEnabled(obj, isTouch);
			}
		}
		static bool IsTouch(string themeName) {
			if (string.IsNullOrEmpty(themeName))
				return false;
			var tokens = themeName.Split(new[] { TouchDelimiter }, StringSplitOptions.RemoveEmptyEntries);
			return tokens.Skip(1).Any(token => token.ToLower() == TouchDefinition);
		}
		static void ForceThemeKeyCreating(string themeName) {
			ThemedElementsDictionary.ForceThemeKeysLoading(themeName);
			EnableResourcesForLoadedAssemblies();
		}
		static void UpdateDefaultStyleKey(DependencyObjectWrapper wrapper, string themeName) {
			object themeKey = GetThemeOrDefaultKey(themeName, wrapper);
			if (themeKey == null)
				return;
			RaiseThemeNameChanging(wrapper.Object, themeName);
			TraceHelper.Write(TraceSwitch,
				MethodNameString, MethodInfo.GetCurrentMethod().Name,
				ObjectTraceString, wrapper.Object,
				ThemeNameTraceString, themeName,
				KeyTraceString, themeKey);
			wrapper.SetDefaultStyleKey(themeKey);
			RaiseThemeNameChanged(wrapper.Object, themeName);
		}
		static object GetThemeOrDefaultKey(string themeName, DependencyObjectWrapper wrapper) {
			if (wrapper.OverridesDefaultStyleKey)
				return null;
			string typeName = GetTypeNameFromKey(wrapper);
			if (typeName == null)
				return null;
			object registeredThemeKey = ThemedElementsDictionary.GetCachedResourceKey(themeName, typeName);
			if (registeredThemeKey != null) {
				return registeredThemeKey;
			}
			return ThemedElementsDictionary.GetCachedResourceKey(string.Empty, typeName);
		}
		static string GetTypeNameFromKey(DependencyObjectWrapper wrapper) {
			object defaultStyleKey = wrapper.GetDefaultStyleKey();
			string typeName = GetTypeName(defaultStyleKey as Type);
			if (typeName != null)
				return typeName;
			typeName = GetTypeName(defaultStyleKey as DefaultStyleThemeKeyExtension);
			return typeName;
		}
		static string GetTypeName(DefaultStyleThemeKeyExtension key) {
			if (key == null)
				return null;
			return key.FullName;
		}
		static string GetTypeName(Type key) {
			if (key == null)
				return null;
			return key.FullName;
		}
		#region IThemeManager Members
		void IThemeManager.SetThemeName(DependencyObject d, string value) {
			SetThemeName(d, value);
		}
		void IThemeManager.ClearThemeName(DependencyObject d) {
			d.ClearValue(ThemeNameProperty);
		}
		string IThemeManager.GetThemeName(DependencyObject d) {
			return GetThemeName(d);
		}
		System.Windows.Data.BindingExpression IThemeManager.GetThemeNameBindingExpression(DependencyObject d) {
			return new DependencyObjectWrapper(d).GetBindingExpression(d, ThemeManager.ThemeNameProperty);
		}
		#endregion
	}
	public class TestClass2 : DependencyObject {
		static readonly DependencyPropertyKey TestPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Test", typeof(int), typeof(TestClass2),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits));
		public static readonly DependencyProperty TestProperty = TestPropertyKey.DependencyProperty;
		static internal void SetTest(DependencyObject d, int test) {
			d.SetValue(TestPropertyKey, test);
		}
		public static int GetTest(DependencyObject d) {
			return (int)d.GetValue(TestProperty);
		}
	}
	public class TouchPaddingInfoExtension : MarkupExtension {
		[IgnoreDependencyPropertiesConsistencyChecker()]
		DependencyProperty dp;
		public DependencyProperty TargetProperty { get { return dp; } set { dp = value; } }
		public Thickness Value { get; set; }
		public Thickness TouchValue { get; set; }
		public double? TouchScale { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new TouchInfo() { Value = Value, TouchValue = TouchValue, Scale = TouchScale, TargetProperty = TargetProperty };
		}
	}
	public class TouchMarginInfoExtension : TouchPaddingInfoExtension {
		public TouchMarginInfoExtension() {
			TargetProperty = FrameworkElement.MarginProperty;
		}
	}
	public class TouchInfo : DependencyObject {
		public static readonly DependencyProperty MarginProperty;
		public static readonly DependencyProperty PaddingProperty;
		public Thickness Value { get; set; }
		public Thickness TouchValue { get; set; }
		public double? Scale { get; set; }
		[IgnoreDependencyPropertiesConsistencyChecker]
		private DependencyProperty targetProperty;
		public DependencyProperty TargetProperty {
			get { return targetProperty; }
			set { targetProperty = value; }
		}
		public static void SetMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(MarginProperty, value);
		}
		public static TouchInfo GetMargin(DependencyObject d) {
			return (TouchInfo)d.GetValue(MarginProperty);
		}
		public static void SetPadding(DependencyObject d, TouchInfo value) {
			d.SetValue(PaddingProperty, value);
		}
		public static TouchInfo GetPadding(DependencyObject d) {
			return (TouchInfo)d.GetValue(PaddingProperty);
		}
		static TouchInfo() {
			Type ownerType = typeof(TouchInfo);
			MarginProperty = DependencyPropertyManager.RegisterAttached("Margin", typeof(TouchInfo), ownerType, new PropertyMetadata(null, OnTouchPropertyChanged));
			PaddingProperty = DependencyPropertyManager.RegisterAttached("Padding", typeof(TouchInfo), ownerType, new PropertyMetadata(null, OnTouchPropertyChanged));
		}
		public static void OnTouchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
	}
	public class TouchPropertyExtension : MarkupExtension {
		static readonly Dictionary<DependencyProperty, DependencyProperty> Properties;
		[IgnoreDependencyPropertiesConsistencyChecker]
		private DependencyProperty key;
		public DependencyProperty Key {
			get { return key; }
			set { key = value; }
		}
		static TouchPropertyExtension() {
			Properties = new Dictionary<DependencyProperty, DependencyProperty>();
		}
		public TouchPropertyExtension() {
		}
		public TouchPropertyExtension(DependencyProperty key) {
			Key = key;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			DependencyProperty result = null;
			if (!Properties.TryGetValue(Key, out result)) {
				result = DependencyProperty.RegisterAttached("TouchProperty" + Key.OwnerType.FullName + Key.Name, typeof(TouchInfo), typeof(TouchInfo), new PropertyMetadata(OnTouchPropertyChanged));
				Properties[Key] = result;
			}
			return result;
		}
		static void OnTouchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
	}
	public class AssemblyPriorityComparer : IComparer<string> {
		#region IComparer<string> Members
		public int Compare(string x, string y) {
			int xLevel = GetLevel(x),
				yLevel = GetLevel(y);
			return Math.Sign(yLevel - xLevel);
		}
		int GetLevel(string assemblyFullName) {
			if (AssemblyHelper.IsDXThemeAssembly(assemblyFullName)) {
				return 1;
			}
			if (!AssemblyHelper.IsDXProductAssembly(assemblyFullName)) {
				if (AssemblyHelper.IsEntryAssembly(assemblyFullName)) {
					return 3;
				}
				else {
					return 2;
				}
			}
			return 0;
		}
		#endregion
	}
}

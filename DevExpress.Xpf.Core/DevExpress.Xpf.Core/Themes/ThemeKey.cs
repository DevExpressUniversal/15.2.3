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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Utils.Themes {
#if SILVERLIGHT
	public abstract class ThemeKeyExtensionGeneric {
#else
	public abstract class ThemeKeyExtensionGeneric : ResourceKey {
		static ThemeKeyExtensionGeneric() {
			if(System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted && !OptionsXBAP.SuppressNotSupportedException)
				throw new XbapNotSupportedException(XbapNotSupportedException.Text);
		}
#endif
		protected static readonly object defaultResourceCore = new object();
		protected ThemeKeyExtensionGeneric() {
			resourceKeyCore = defaultResourceCore;
		}
		int hash;
		object resourceKeyCore;
		string themeName;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Type TypeInTargetAssembly { get; set; }
		public string ThemeName {
			get { return themeName; }
			set { SetThemeName(value); }
		}
		protected object ResourceKeyCore { get { return resourceKeyCore; } set { resourceKeyCore = value; } }
		void SetThemeName(string value) {
			themeName = value;
			SetHashCode();
		}
		public override int GetHashCode() {
			return hash;
		}
		protected virtual bool Equals(ThemeKeyExtensionGeneric other) {
			if (IsSameTheme(other)) 
				return true;
			return false;
		}
		protected virtual bool IsSameTheme(ThemeKeyExtensionGeneric other) {
			bool equals = object.Equals(ThemeName, other.ThemeName);
			if (@equals)
				return true;
			if (CompareWithAlias(ThemeName, other.ThemeName) || CompareWithAlias(other.ThemeName, ThemeName))
				return true;
			return false;
		}
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((ThemeKeyExtensionGeneric)obj);
		}
		bool CompareWithAlias(string themeName0, string themeName1) {
			return string.IsNullOrEmpty(themeName0) && (string.IsNullOrEmpty(themeName1) || themeName1 == Theme.DeepBlueName);
		}
		public virtual string ResourceKeyToString() {
			return resourceKeyCore.ToString();
		}
#if !SILVERLIGHT
		public override System.Reflection.Assembly Assembly {
			get {
				if(TypeInTargetAssembly != null) {
					return TypeInTargetAssembly.Assembly;
				}
				return string.IsNullOrEmpty(ThemeName) ? GetType().Assembly : GetAssembly();
			}
		}
		protected virtual System.Reflection.Assembly GetAssembly() {
			Theme theme = Theme.FindTheme(ThemeName);
			if(theme == null)
				return null;
			return theme.Assembly;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if(string.IsNullOrEmpty(ThemeName)) {
				string themeName = ThemeNameHelper.GetThemeName(serviceProvider);
				if(!string.IsNullOrEmpty(themeName))
					ThemeName = themeName;
			}
			return this;
		}
#endif
		protected virtual void SetHashCode() {
			hash = GenerateHashCode();
		}
		protected virtual int GenerateHashCode() {
			return 0;
		}
		bool ShouldSerializeThemeName() {
			return !String.IsNullOrEmpty(ThemeName);
		}		
	}
	public abstract class ThemeKeyExtensionBase<T> : ThemeKeyExtensionInternalBase<T> {
		protected ThemeKeyExtensionBase() {
			IsThemeIndependent = false;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsThemeIndependent { get; set; }
		protected override bool IsSameTheme(ThemeKeyExtensionGeneric other) {
			var key = (ThemeKeyExtensionBase<T>)other;
			bool isThemeIndependent = IsThemeIndependent || key.IsThemeIndependent;
			bool isSameAssembly = true;
#if !SL
			isSameAssembly = isThemeIndependent || (base.IsSameTheme(other) && Equals(Assembly, key.Assembly));
#else
			isSameAssembly = true;
#endif
			return isSameAssembly && Equals(ResourceKeyCore, key.ResourceKeyCore);
		}
	}
	public abstract class ThemeKeyExtensionInternalBase<T> : ThemeKeyExtensionGeneric, IDisposable {
		bool isVisibleInBlendCore = true;
		public T ResourceKey {
			get { return (T)ResourceKeyCore; }
			set { SetResourceKey(value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsVisibleInBlend {
			get { return isVisibleInBlendCore; }
			set { isVisibleInBlendCore = value; }
		}
		protected ThemeKeyExtensionInternalBase()
			: base() {
		}
		protected override bool Equals(ThemeKeyExtensionGeneric other) {
			var baseResult = base.Equals(other);
			if (!baseResult)
				return false;
			var key = (ThemeKeyExtensionInternalBase<T>)other;
			return object.Equals(ResourceKeyCore, key.ResourceKeyCore);
		}
		void SetResourceKey(T value) {
			if(value == null)
				throw new NullReferenceException("ThemeKeyExtensionBase");
			if(typeof(T).IsEnum && !Enum.IsDefined(typeof(T), value))
				throw new ArgumentException("Resource key isn`t defined");
			ResourceKeyCore = value;
			SetHashCode();
		}
		public override string ToString() {
			return GetType().Name + "_" + ResourceKeyCore;
		}
		protected override int GenerateHashCode() {
			return base.GenerateHashCode() ^ typeof(T).GetHashCode() ^ ResourceKeyCore.GetHashCode();
		}
		#region IDisposable Members
		void IDisposable.Dispose() {
		}
		#endregion
	}
	public class BlendVisibilityAttribute : Attribute {
		public bool IsVisible { get; set; }
		public BlendVisibilityAttribute()
			: this(true) {
		}
		public BlendVisibilityAttribute(bool isVisible) {
			IsVisible = isVisible;
		}
	}
#if !SL
	public class DefaultStyleThemeKeyExtension : ThemeKeyExtensionInternalBase<string> {
		string fullName;
		string traceString = null; 
		public DefaultStyleThemeKeyExtension() { }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string FullName {
			get { return fullName; }
			set { SetFullName(value); }
		}
		public Type Type { set { SetFullName(value != null ? value.FullName : null); } }
		internal protected string AssemblyName { get; set; }
		protected virtual bool IsValid {
			get {
				return !string.IsNullOrEmpty(FullName) && ThemeName != null;
			}
		}
		protected override int GenerateHashCode() {
			return base.GenerateHashCode() ^ (FullName == null ? 0 : FullName.GetHashCode());
		}
		protected override bool Equals(ThemeKeyExtensionGeneric other) {
			var baseResult = base.Equals(other);
			if (!baseResult)
				return false;
			var themeKey = (DefaultStyleThemeKeyExtension)other;
			return FullName == themeKey.FullName && object.Equals(Assembly, themeKey.Assembly);
		}
		void SetFullName(string value) {
			this.fullName = value;
			SetHashCode();
		}
		public override Assembly Assembly {
			get {
				if(AssemblyName != null) {
					Assembly assembly = AssemblyHelper.GetLoadedAssembly(AssemblyName);
					if(assembly != null)
						return assembly;
				}
				return base.Assembly;
			}
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if(IsInitialized(serviceProvider)) {
				RegisterThemeType();
				return this;
			}
			return base.ProvideValue(serviceProvider);
		}
		void RegisterThemeType() {
			SetHashCode();
			if(!IsValid)
				return;
			GenerateTrace();
			TraceHelper.Write(ThemeManager.TraceSwitch, "Registering ThemedType", this);
			ThemedElementsDictionary.RegisterThemeType(ThemeName, FullName, this);
		}
		bool IsInitialized(IServiceProvider serviceProvider) {
			IProvideValueTarget provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
			return provideValueTarget != null && provideValueTarget.TargetObject != null;
		}
		[System.Diagnostics.Conditional("DEBUG")]
		void GenerateTrace() {
			StringBuilder traceStringBuilder = new StringBuilder();
			traceStringBuilder.Append(ThemeName);
			traceStringBuilder.Append(" ");
			traceStringBuilder.Append(FullName);
			traceStringBuilder.Append(" ");
			traceStringBuilder.Append((Assembly != null ? Assembly.FullName : null));
			traceStringBuilder.Append(" ");
			traceStringBuilder.Append((ResourceKeyCore != defaultResourceCore ? ResourceKeyCore.ToString() : null));
			traceStringBuilder.ToString();
			this.traceString = string.Intern(traceStringBuilder.ToString());
		}
		public override string ToString() {
			return traceString ?? base.ToString();
		}
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class ThemeKeyAttribute : Attribute {
		public Type TargetType { get; set; }
		public ThemeKeyAttribute(Type targetType) {
			TargetType = targetType;
		}
	}
	public static class ThemeNameHelper {
		public static readonly string ThemeAssemblyPrefix = "/DevExpress.Xpf.Themes.";
		public static string GetThemeName(IServiceProvider serviceProvider) {
			IUriContext uriContext = (IUriContext)serviceProvider.GetService(typeof(IUriContext));
			if(uriContext != null && uriContext.BaseUri != null && uriContext.BaseUri.IsAbsoluteUri)
				return EnsureThemeName(uriContext.BaseUri);
			return null;
		}
		public static string GetAssemblyName(IServiceProvider serviceProvider) {
			IUriContext uriContext = (IUriContext)serviceProvider.GetService(typeof(IUriContext));
			if (uriContext != null && uriContext.BaseUri != null && uriContext.BaseUri.IsAbsoluteUri && uriContext.BaseUri.LocalPath.Contains(';'))
				return uriContext.BaseUri.LocalPath.Split(';')[0];
			return null;
		}
		static string EnsureThemeName(Uri baseUri) {
			try {
				string localPath = baseUri.OriginalString;
				if(string.IsNullOrEmpty(localPath)) return null;
				int startIndex = localPath.IndexOf(ThemeAssemblyPrefix);
				if(startIndex >= 0) {
					startIndex += ThemeAssemblyPrefix.Length;
					int endIndex = localPath.IndexOf('.', startIndex);
					if(endIndex > startIndex) return localPath.Substring(startIndex, endIndex - startIndex);
				}
			}
			catch {
			}
			return null;
		}
	}
	public class ThemePartLoaderExtension : MarkupExtension {
		public string AssemblyName { get; set; }
		public string Path { get; set; }
		public string PathCore { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			string path = Path;
			string themeName = ThemeNameHelper.GetThemeName(serviceProvider);
			if(String.IsNullOrEmpty(themeName))
				return new Uri(string.Format(ThemeNameHelper.GetAssemblyName(serviceProvider) + ";component{0}", PathCore), UriKind.RelativeOrAbsolute);
			return new Uri(string.Format(ThemeNameHelper.ThemeAssemblyPrefix + "{0}" + AssemblyInfo.VSuffix + ";component{1}", themeName, path), UriKind.RelativeOrAbsolute);
		}
	}
#endif
}
namespace DevExpress.Xpf.Utils.Themes.Diagnostics {
	public enum RefCounterAction { Increase, Decrease, GetValue }
	public class ThemeKeyCounter {
		protected static Dictionary<Type, int> instancesStorage;
		static ThemeKeyCounter() {
			CreateInstancesStorage();
		}
		private static void CreateInstancesStorage() {
			instancesStorage = new Dictionary<Type, int>();
		}
		public static void Reset() { CreateInstancesStorage(); }
		public static Dictionary<Type, int> Info { get { return instancesStorage; } }
		public static int ChangeRefCounter(object obj, RefCounterAction action) {
			Type type = obj.GetType();
			int newVal = 0;
			if(DevExpress.Xpf.Utils.Themes.Diagnostics.ThemeKeyCounter.Info.ContainsKey(type)) {
				int diff = 0;
				switch(action) {
					case RefCounterAction.Increase:
						diff = 1;
						break;
					case RefCounterAction.Decrease:
						diff = -1;
						break;
				}
				newVal = DevExpress.Xpf.Utils.Themes.Diagnostics.ThemeKeyCounter.Info[type] + diff;
				if(newVal < 0) newVal = 0;
				DevExpress.Xpf.Utils.Themes.Diagnostics.ThemeKeyCounter.Info[type] = newVal;
			}
			else
				if(action != RefCounterAction.GetValue) DevExpress.Xpf.Utils.Themes.Diagnostics.ThemeKeyCounter.Info.Add(type, 1);
			return newVal;
		}
	}
}

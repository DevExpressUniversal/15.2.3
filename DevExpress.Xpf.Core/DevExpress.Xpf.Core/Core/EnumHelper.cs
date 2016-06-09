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

using System.Diagnostics;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Linq;
using System.Globalization;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Utils.Design;
#if SL
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TypeConverter = System.ComponentModel.TypeConverter;
#endif
namespace DevExpress.Xpf.Core.Native {
	public interface INotifyPositionChanged {
		void OnPositionChanged(Rect newRect);
	}
}
namespace DevExpress.Xpf.Core {
	public class FilterCriteriaConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			return (value as string).Return(x => CriteriaOperator.Parse(x), () => base.ConvertFrom(context, culture, value));
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			return (value as CriteriaOperator).Return(x => x.ToString(), () => base.ConvertTo(context, culture, value, destinationType));
		}
	}
	public class DXDockPanel : DockPanel {
		protected override Size ArrangeOverride(Size arrangeSize) {
			Size result = base.ArrangeOverride(arrangeSize);
			foreach (UIElement child in Children) {
				if (child != null && child is INotifyPositionChanged) {
					((INotifyPositionChanged)child).OnPositionChanged(LayoutHelper.GetRelativeElementRect(child, this));
				}
			}
			return result;
		}
	}
	public enum EditorShowMode {
		Default,
		MouseDown,
		MouseDownFocused,
		MouseUp,
		MouseUpFocused,
	}
	[Obsolete("Use Theme instead")]
	public abstract class DefaultThemeInfo : MarkupExtension {
		public const string DeepBlue = "DeepBlue";
		public const string LightGray = "LightGray";
		public const string Office2007Blue = "Office2007Blue";
		public const string Office2007Black = "Office2007Black";
		public const string Office2007Silver = "Office2007Silver";
		public const string DeepBlueFullName = "Deep Blue";
		public const string LightGrayFullName = "Light Gray";
		public const string Office2007BlueFullName = "Office 2007 Blue";
		public const string Office2007BlackFullName = "Office 2007 Black";
		public const string Office2007SilverFullName = "Office 2007 Silver";
		static string currentThemeName = string.Empty;
#if DEBUGTEST
		public static string CurrentThemeName {
			get { return currentThemeName; }
			set { currentThemeName = value; }
		}
#endif
		public static string DefaultThemeName {
			get {
#if !SL
				return DeepBlue;
#else
				return LightGray;
#endif
			}
		}
		public static string DefaultThemeFullName {
			get {
#if !SL
				return DeepBlueFullName;
#else
				return LightGrayFullName;
#endif
			}
		}
		public static List<string> Themes = new List<string>(new string[] { DefaultThemeName, Office2007Blue, Office2007Black, Office2007Silver,
#if !SL
				LightGray
#else
				DeepBlue
#endif
		});
		public static bool IsDefaultTheme(string themeName) {
			return string.IsNullOrEmpty(themeName) || themeName == DefaultThemeName;
		}
	}
	public static class RightToLeftHelper {
		public static bool IsLeftKey(Key key, bool isRightToLeft) {
			return key == GetLeftKey(isRightToLeft);
		}
		public static bool IsRightKey(Key key, bool isRightToLeft) {
			return key == GetLeftKey(!isRightToLeft);
		}
		public static Key TransposeKey(Key key, bool isRightToLeft = true) {
			if (!isRightToLeft)
				return key;
			if (key == Key.Left)
				return Key.Right;
			if (key == Key.Right)
				return Key.Left;
			return key;
		}
		public static Key TransposeKey(Key key, FrameworkElement control) {
			if (control == null)
				return key;
			return TransposeKey(key, control.FlowDirection == FlowDirection.RightToLeft);
		}
		static Key GetLeftKey(bool isRightToLeft) {
			return isRightToLeft ? Key.Right : Key.Left;
		}
	}
	public static class EnumHelper {
		public static int GetEnumCount(Type enumType) {
			return EnumSourceHelperCore.GetEnumCount(enumType);
		}
		public static IEnumerable<EnumMemberInfo> GetEnumSource(Type enumType, bool useUnderlyingEnumValue = true, IValueConverter nameConverter = null, bool splitNames = false, EnumMembersSortMode sortMode = EnumMembersSortMode.Default, bool allowImages = true, bool allowText = true) {
			return EnumSourceHelperCore.GetEnumSource(enumType, useUnderlyingEnumValue, nameConverter, splitNames, sortMode, ViewModelMetadataSource.GetKnownImageCallback(ImageType.Colored), allowImages, allowText);
		}
	}
	public abstract class CustomPropertyDescriptor : PropertyDescriptor {
		protected CustomPropertyDescriptor(string name)
			: base(name, null) {
		}
		public override Type ComponentType { get { return typeof(object); } }
		public override bool CanResetValue(object component) { return false; }
		public override bool ShouldSerializeValue(object component) { return false; }
		public override void ResetValue(object component) { }
	}
#if !SL
	public class AssemblyVersionExtension : MarkupExtension {
		public AssemblyVersionExtension() { }
		public AssemblyVersionExtension(bool showShortVersion) { ShowShortVersion = showShortVersion; }
		public bool ShowShortVersion { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return !ShowShortVersion ? string.Concat("version ", AssemblyInfo.Version) : AssemblyInfo.Version;
		}
	}
	public class ObjectConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if (value != null && value is string)
				return value;
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class TimeConsumingOperationHelper {
		public static readonly DependencyProperty IsBusyProperty;
		static readonly DependencyPropertyKey IsBusyPropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty BusyLockerProperty;
		static readonly DependencyPropertyKey BusyLockerPropertyKey;
		static TimeConsumingOperationHelper() {
			IsBusyPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsBusy", typeof(bool), typeof(TimeConsumingOperationHelper), new UIPropertyMetadata(false));
			IsBusyProperty = IsBusyPropertyKey.DependencyProperty;
			BusyLockerPropertyKey = DependencyProperty.RegisterAttachedReadOnly("BusyLocker", typeof(Locker), typeof(TimeConsumingOperationHelper), new UIPropertyMetadata(null));
			BusyLockerProperty = BusyLockerPropertyKey.DependencyProperty;
		}
		public static bool GetIsBusy(DependencyObject obj) {
			return (bool)obj.GetValue(IsBusyProperty);
		}
		static void SetIsBusy(DependencyObject obj, bool value) {
			obj.SetValue(IsBusyPropertyKey, value);
		}
		static Locker GetBusyLocker(DependencyObject obj) {
			return (Locker)obj.GetValue(BusyLockerProperty);
		}
		static void SetBusyLocker(DependencyObject obj, Locker value) {
			obj.SetValue(BusyLockerPropertyKey, value);
		}
		public static void LockElement(DependencyObject d) {
			Locker locker = GetLocker(d);
			locker.Lock();
			UpdateIsBusy(d, locker);
		}
		public static void UnlockElement(DependencyObject d) {
			Locker locker = GetLocker(d);
			locker.Unlock();
			UpdateIsBusy(d, locker);
		}
		public static void DoBusyAction(DependencyObject d, Action action) {
			LockElement(d);
			try {
				action();
			}
			finally {
				UnlockElement(d);
			}
		}
		static Locker GetLocker(DependencyObject d) {
			Locker locker = GetBusyLocker(d);
			if (locker == null) {
				locker = new Locker();
				SetBusyLocker(d, locker);
			}
			return locker;
		}
		static void UpdateIsBusy(DependencyObject d, Locker locker) {
			SetIsBusy(d, locker.IsLocked);
		}
	}
#endif
	public class EmbeddedResourceImage : MarkupExtension {
		public object Source { get; set; }
		object value;
		public EmbeddedResourceImage() { }
		public EmbeddedResourceImage(object source) {
			this.Source = source;
		}
		public static object ConvertSource(object source) {
			object result = null;
			using (var stream = GetResourceStream(source)) {
				result = ImageDataConverter.CreateImageFromStream(stream);
			}
			return result;
		}
		static Stream GetResourceStream(object value) {
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(System.Convert.ToString(value));
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return value ?? (value = ConvertSource(Source));
		}
	}
	public class EmbeddedResourceImageConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return EmbeddedResourceImage.ConvertSource(value);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
#if !SL
	[Obsolete]
	public class SchedulerDefaultThemeInfo : DefaultThemeInfo {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return (Uri)(new SchedulerResourceExtension(("Themes/" + DefaultThemeName + ".xaml")).ProvideValue(null));
		}
	}
	[Obsolete]
	public class NavBarDefaultThemeInfo : DefaultThemeInfo {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return (Uri)(new NavBarResourceExtension(("Themes/" + DefaultThemeName + ".xaml")).ProvideValue(null));
		}
	}
	[Obsolete]
	public class DockingDefaultThemeInfo : DefaultThemeInfo {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return (Uri)(new DockingResourceExtension(("Themes/" + DefaultThemeName + ".xaml")).ProvideValue(null));
		}
	}
	public static class ReraiseEventHelper {
		public static void ReraiseEvent<T>(T sourceEventArgs, UIElement element, RoutedEvent tunnelingEvent, RoutedEvent bubblingEvent, Func<T, T> cloneFunc) where T : RoutedEventArgs {
			if (element == null)
				return;
			T newEventArgs = cloneFunc(sourceEventArgs);
			newEventArgs.RoutedEvent = tunnelingEvent;
			element.RaiseEvent(newEventArgs);
			if (newEventArgs.Handled)
				return;
			newEventArgs = cloneFunc(sourceEventArgs);
			newEventArgs.RoutedEvent = bubblingEvent;
			element.RaiseEvent(newEventArgs);
		}
		public static MouseButtonEventArgs CloneMouseButtonEventArgs(MouseButtonEventArgs eventArgs) {
			return new MouseButtonEventArgs(eventArgs.MouseDevice, eventArgs.Timestamp, eventArgs.ChangedButton, eventArgs.StylusDevice);
		}
		public static MouseEventArgs CloneMouseEventArgs(MouseEventArgs eventArgs) {
			return new MouseEventArgs(eventArgs.MouseDevice, eventArgs.Timestamp, eventArgs.StylusDevice);
		}
	}
#endif
	[DebuggerStepThrough]
	public class Locker : IDisposable {
		int lockCount;
		public bool IsLocked { get { return lockCount > 0; } }
		public event EventHandler Unlocked;
		public Locker LockOnce() {
			if (!IsLocked)
				Lock();
			return this;
		}
		public Locker Lock() {
			lockCount++;
			return this;
		}
		public void Unlock() {
			if (IsLocked) {
				lockCount--;
				if (!IsLocked)
					RaiseOnUnlock();
			}
		}
		public void Reset() {
			lockCount = 0;
		}
		public void DoLockedAction(Action action) {
			Lock();
			try {
				action();
			}
			finally {
				Unlock();
			}
		}
		public void DoIfNotLocked(Action action) {
			if (!IsLocked)
				action();
		}
		public void DoLockedActionIfNotLocked(Action action) {
			DoIfNotLocked(() => DoLockedAction(action));
		}
		public void DoIfNotLocked(Action action, Action lockedAction = null) {
			if (!IsLocked) {
				action();
			}
			else {
				if (lockedAction != null)
					lockedAction();
			}
		}
		void RaiseOnUnlock() {
			if (Unlocked != null)
				Unlocked(this, EventArgs.Empty);
		}
		#region for using directive usage
		void IDisposable.Dispose() {
			Unlock();
		}
		#endregion
		#region implicit convert to bool
		public static implicit operator bool(Locker locker) {
			return locker.IsLocked;
		}
		#endregion
	}
}

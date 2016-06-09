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
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Printing {
	public static class ExportSettings {
		public static readonly DependencyProperty TargetTypeProperty;
		public static readonly DependencyProperty BackgroundProperty;
		public static readonly DependencyProperty ForegroundProperty;
		public static readonly DependencyProperty BorderColorProperty;
		public static readonly DependencyProperty BorderThicknessProperty;
		public static readonly DependencyProperty UrlProperty;
		public static readonly DependencyProperty OnPageUpdaterProperty;
		public static readonly DependencyProperty PropertiesHintMaskProperty;
		public static readonly DependencyProperty ElementTagProperty;
		public static readonly DependencyProperty BookmarkProperty;
		public static readonly DependencyProperty BookmarkParentNameProperty;
		public static readonly DependencyProperty BorderDashStyleProperty;
		public static readonly DependencyProperty MergeValueProperty;
		static ExportSettings() {
			Type ownerType = typeof(ExportSettings);
			TargetTypeProperty = DependencyPropertyManager.RegisterAttached("TargetType", typeof(TargetType), ownerType, new PropertyMetadata(ExportSettingDefaultValue.TargetType));
#if SL
			BackgroundProperty = DependencyPropertyManager.RegisterAttached("Background", typeof(Color), ownerType, new PropertyMetadata(ExportSettingDefaultValue.Background));
#else
			BackgroundProperty = DependencyPropertyManager.RegisterAttached("Background", typeof(Color), ownerType, new FrameworkPropertyMetadata(ExportSettingDefaultValue.Background, FrameworkPropertyMetadataOptions.Inherits));
#endif
			ForegroundProperty = DependencyPropertyManager.RegisterAttached("Foreground", typeof(Color), ownerType, new PropertyMetadata(ExportSettingDefaultValue.Foreground));
			BorderColorProperty = DependencyPropertyManager.RegisterAttached("BorderColor", typeof(Color), ownerType, new PropertyMetadata(ExportSettingDefaultValue.BorderColor));
			BorderThicknessProperty = DependencyPropertyManager.RegisterAttached("BorderThickness", typeof(Thickness), ownerType, new PropertyMetadata(ExportSettingDefaultValue.BorderThickness));
			UrlProperty = DependencyPropertyManager.RegisterAttached("Url", typeof(string), ownerType, new PropertyMetadata(ExportSettingDefaultValue.Url));
			OnPageUpdaterProperty = DependencyPropertyManager.RegisterAttached("OnPageUpdater", typeof(IOnPageUpdater), ownerType, new PropertyMetadata(ExportSettingDefaultValue.OnPageUpdater));
			PropertiesHintMaskProperty = DependencyPropertyManager.RegisterAttached("PropertiesHintMask", typeof(ExportSettingsProperties), ownerType, new PropertyMetadata(ExportSettingsProperties.None));
			ElementTagProperty = DependencyPropertyManager.RegisterAttached("ElementTag", typeof(string), typeof(ExportSettings), new PropertyMetadata(null));
			BookmarkProperty = DependencyPropertyManager.RegisterAttached("Bookmark", typeof(string), typeof(ExportSettings), new PropertyMetadata(null));
			BookmarkParentNameProperty = DependencyPropertyManager.RegisterAttached("BookmarkParentName", typeof(string), typeof(ExportSettings), new PropertyMetadata(null));
			BorderDashStyleProperty = DependencyPropertyManager.RegisterAttached("BorderDashStyle", typeof(BorderDashStyle), typeof(ExportSettings), new PropertyMetadata(ExportSettingDefaultValue.BorderDashStyle));
			MergeValueProperty = DependencyPropertyManager.RegisterAttached("MergeValue", typeof(object), typeof(ExportSettings), new PropertyMetadata(ExportSettingDefaultValue.MergeValue));
		}
		public static TargetType GetTargetType(DependencyObject obj) {
			return (TargetType)obj.GetValue(TargetTypeProperty);
		}
		public static void SetTargetType(DependencyObject obj, TargetType value) {
			obj.SetValue(TargetTypeProperty, value);
		}
		public static Color GetBackground(DependencyObject obj) {
			return (Color)obj.GetValue(BackgroundProperty);
		}
		public static void SetBackground(DependencyObject obj, Color value) {
			obj.SetValue(BackgroundProperty, value);
		}
		public static Color GetForeground(DependencyObject obj) {
			return (Color)obj.GetValue(ForegroundProperty);
		}
		public static void SetForeground(DependencyObject obj, Color value) {
			obj.SetValue(ForegroundProperty, value);
		}
		public static Color GetBorderColor(DependencyObject obj) {
			return (Color)obj.GetValue(BorderColorProperty);
		}
		public static void SetBorderColor(DependencyObject obj, Color value) {
			obj.SetValue(BorderColorProperty, value);
		}
		public static Thickness GetBorderThickness(DependencyObject obj) {
			return (Thickness)obj.GetValue(BorderThicknessProperty);
		}
		public static void SetBorderThickness(DependencyObject obj, Thickness value) {
			obj.SetValue(BorderThicknessProperty, value);
		}
		public static string GetUrl(DependencyObject obj) {
			return (string)obj.GetValue(UrlProperty);
		}
		public static void SetUrl(DependencyObject obj, string value) {
			obj.SetValue(UrlProperty, value);
		}
		public static IOnPageUpdater GetOnPageUpdater(DependencyObject obj) {
			return (IOnPageUpdater)obj.GetValue(OnPageUpdaterProperty);
		}
		public static void SetOnPageUpdater(DependencyObject obj, IOnPageUpdater value) {
			obj.SetValue(OnPageUpdaterProperty, value);
		}
		public static ExportSettingsProperties GetPropertiesHintMask(DependencyObject obj) {
			return (ExportSettingsProperties)obj.GetValue(PropertiesHintMaskProperty);
		}
		public static void SetPropertiesHintMask(DependencyObject obj, ExportSettingsProperties value) {
			obj.SetValue(PropertiesHintMaskProperty, value);
		}
		public static string GetElementTag(DependencyObject obj) {
			return (string)obj.GetValue(ElementTagProperty);
		}
		public static void SetElementTag(DependencyObject obj, string value) {
			obj.SetValue(ElementTagProperty, value);
		}
		public static string GetBookmark(DependencyObject obj) {
			return (string)obj.GetValue(BookmarkProperty);
		}
		public static void SetBookmark(DependencyObject obj, string value) {
			obj.SetValue(BookmarkProperty, value);
		}
		public static string GetBookmarkParentName(DependencyObject obj) {
			return (string)obj.GetValue(BookmarkParentNameProperty);
		}
		public static void SetBookmarkParentName(DependencyObject obj, string value) {
			obj.SetValue(BookmarkParentNameProperty, value);
		}
		public static BorderDashStyle GetBorderDashStyle(DependencyObject obj) {
			return (BorderDashStyle)obj.GetValue(BorderDashStyleProperty);
		}
		public static void SetBorderDashStyle(DependencyObject obj, BorderDashStyle value) {
			obj.SetValue(BorderDashStyleProperty, value);
		}
		public static object GetMergeValue(DependencyObject obj) {
			return obj.GetValue(MergeValueProperty);
		}
		public static void SetMergeValue(DependencyObject obj, object value) {
			obj.SetValue(MergeValueProperty, value);
		}
	}
	[Flags]
	public enum ExportSettingsProperties {
		None				= 0,
		TargetType		  = 1 << 0,
		Background		  = 1 << 1,
		Foreground		  = 1 << 2,
		BorderColor		 = 1 << 3,
		BorderThickness	 = 1 << 4,
		Url				 = 1 << 5,
		OnPageUpdater	   = 1 << 6,
		BorderDashStyle	 = 1 << 7,
		MergeValue		  = 1 << 8,
	}
}

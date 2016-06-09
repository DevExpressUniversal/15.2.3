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
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Printing {
	public static class TextExportSettings {
		public static readonly DependencyProperty HorizontalAlignmentProperty;
		public static readonly DependencyProperty VerticalAlignmentProperty;
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty TextValueProperty;
		public static readonly DependencyProperty TextValueFormatStringProperty;
		public static readonly DependencyProperty FontFamilyProperty;
		public static readonly DependencyProperty FontStyleProperty;
		public static readonly DependencyProperty FontWeightProperty;
		public static readonly DependencyProperty FontSizeProperty;
		public static readonly DependencyProperty PaddingProperty;
		public static readonly DependencyProperty TextWrappingProperty;
		public static readonly DependencyProperty NoTextExportProperty;
		public static readonly DependencyProperty XlsExportNativeFormatProperty;
		public static readonly DependencyProperty XlsxFormatStringProperty;
		public static readonly DependencyProperty TextDecorationsProperty;
		public static readonly DependencyProperty TextTrimmingProperty;
		static TextExportSettings() {
			HorizontalAlignmentProperty = RegisterAttached("HorizontalAlignment", typeof(HorizontalAlignment), ExportSettingDefaultValue.HorizontalAlignment);
			VerticalAlignmentProperty = RegisterAttached("VerticalAlignment", typeof(VerticalAlignment), ExportSettingDefaultValue.VerticalAlignment);
			TextProperty = RegisterAttached("Text", typeof(string), ExportSettingDefaultValue.Text);
			TextValueProperty = RegisterAttached("TextValue", typeof(object), ExportSettingDefaultValue.TextValue);
			TextValueFormatStringProperty = RegisterAttached("TextValueFormatString", typeof(string), ExportSettingDefaultValue.TextValueFormatString);
			FontFamilyProperty = RegisterAttached("FontFamily", typeof(FontFamily), ExportSettingDefaultValue.FontFamily);
			FontStyleProperty = RegisterAttached("FontStyle", typeof(FontStyle), ExportSettingDefaultValue.FontStyle);
			FontWeightProperty = RegisterAttached("FontWeight", typeof(FontWeight), ExportSettingDefaultValue.FontWeight);
			FontSizeProperty = RegisterAttached("FontSize", typeof(double), ExportSettingDefaultValue.FontSize);
			PaddingProperty = RegisterAttached("Padding", typeof(Thickness), ExportSettingDefaultValue.Padding);
			TextWrappingProperty = RegisterAttached("TextWrapping", typeof(TextWrapping), ExportSettingDefaultValue.TextWrapping);
			NoTextExportProperty = RegisterAttached("NoTextExport", typeof(bool), ExportSettingDefaultValue.NoTextExport);
			XlsExportNativeFormatProperty = RegisterAttached("XlsExportNativeFormat", typeof(bool?), ExportSettingDefaultValue.XlsExportNativeFormat);
			XlsxFormatStringProperty = RegisterAttached("XlsxFormatString", typeof(string), ExportSettingDefaultValue.XlsxFormatString);
			TextDecorationsProperty = RegisterAttached("TextDecorations", typeof(TextDecorationCollection), ExportSettingDefaultValue.TextDecorations);
			TextTrimmingProperty = RegisterAttached("TextTrimming", typeof(TextTrimming), TextTrimming.None);
		}
		static DependencyProperty RegisterAttached(string name, Type propertyType, object defaultValue) {
			return DependencyPropertyManager.RegisterAttached(name, propertyType, typeof(TextExportSettings), new PropertyMetadata(defaultValue));
		}
		public static TextWrapping GetTextWrapping(DependencyObject obj) {
			return (TextWrapping)obj.GetValue(TextWrappingProperty);
		}
		public static void SetTextWrapping(DependencyObject obj, TextWrapping value) {
			obj.SetValue(TextWrappingProperty, value);
		}
		public static HorizontalAlignment GetHorizontalAlignment(DependencyObject obj) {
			return (HorizontalAlignment)obj.GetValue(HorizontalAlignmentProperty);
		}
		public static void SetHorizontalAlignment(DependencyObject obj, HorizontalAlignment value) {
			obj.SetValue(HorizontalAlignmentProperty, value);
		}
		public static VerticalAlignment GetVerticalAlignment(DependencyObject obj) {
			return (VerticalAlignment)obj.GetValue(VerticalAlignmentProperty);
		}
		public static void SetVerticalAlignment(DependencyObject obj, VerticalAlignment value) {
			obj.SetValue(VerticalAlignmentProperty, value);
		}
		public static string GetText(DependencyObject obj) {
			return (string)obj.GetValue(TextProperty);
		}
		public static void SetText(DependencyObject obj, string value) {
			obj.SetValue(TextProperty, value);
		}
		public static object GetTextValue(DependencyObject obj) {
			return obj.GetValue(TextValueProperty);
		}
		public static void SetTextValue(DependencyObject obj, object value) {
			obj.SetValue(TextValueProperty, value);
		}
		public static string GetTextValueFormatString(DependencyObject obj) {
			return (string)obj.GetValue(TextValueFormatStringProperty);
		}
		public static void SetTextValueFormatString(DependencyObject obj, string value) {
			obj.SetValue(TextValueFormatStringProperty, value);
		}
		public static FontFamily GetFontFamily(DependencyObject obj) {
			return (FontFamily)obj.GetValue(FontFamilyProperty);
		}
		public static void SetFontFamily(DependencyObject obj, FontFamily value) {
			obj.SetValue(FontFamilyProperty, value);
		}
		public static FontStyle GetFontStyle(DependencyObject obj) {
			return (FontStyle)obj.GetValue(FontStyleProperty);
		}
		public static void SetFontStyle(DependencyObject obj, FontStyle value) {
			obj.SetValue(FontStyleProperty, value);
		}
		public static FontWeight GetFontWeight(DependencyObject obj) {
			return (FontWeight)obj.GetValue(FontWeightProperty);
		}
		public static void SetFontWeight(DependencyObject obj, FontWeight value) {
			obj.SetValue(FontWeightProperty, value);
		}
		public static double GetFontSize(DependencyObject obj) {
			return (double)obj.GetValue(FontSizeProperty);
		}
		public static void SetFontSize(DependencyObject obj, double value) {
			obj.SetValue(FontSizeProperty, value);
		}
		public static Thickness GetPadding(DependencyObject obj) {
			return (Thickness)obj.GetValue(PaddingProperty);
		}
		public static void SetPadding(DependencyObject obj, Thickness value) {
			obj.SetValue(PaddingProperty, value);
		}
		public static bool GetNoTextExport(DependencyObject obj) {
			return (bool)obj.GetValue(NoTextExportProperty);
		}
		public static void SetNoTextExport(DependencyObject obj, bool value) {
			obj.SetValue(NoTextExportProperty, value);
		}
		public static bool? GetXlsExportNativeFormat(DependencyObject obj) {
			return (bool?)obj.GetValue(XlsExportNativeFormatProperty);
		}
		public static void SetXlsExportNativeFormat(DependencyObject obj, bool? value) {
			obj.SetValue(XlsExportNativeFormatProperty, value);
		}
		public static string GetXlsxFormatString(DependencyObject obj) {
			return (string)obj.GetValue(XlsxFormatStringProperty);
		}
		public static void SetXlsxFormatString(DependencyObject obj, string value) {
			obj.SetValue(XlsxFormatStringProperty, value);
		}
		public static TextDecorationCollection GetTextDecorations(DependencyObject obj) {
			return (TextDecorationCollection)obj.GetValue(TextDecorationsProperty);
		}
		public static void SetTextDecorations(DependencyObject obj, TextDecorationCollection value) {
			obj.SetValue(TextDecorationsProperty, value);
		}
		public static TextTrimming GetTextTrimming(DependencyObject obj) {
			return (TextTrimming)obj.GetValue(TextTrimmingProperty);
		}
		public static void SetTextTrimming(DependencyObject obj, TextTrimming value) {
			obj.SetValue(TextTrimmingProperty, value);
		}
	}
}

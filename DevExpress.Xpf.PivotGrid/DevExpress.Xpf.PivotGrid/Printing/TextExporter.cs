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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.Native;
using System;
using DevExpress.XtraPrinting;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.PivotGrid.Printing {
	public interface IPivotTextExportSettings {
		HorizontalAlignment HorizontalContentAlignment { get; }
		string ValueFormat { get; }
		bool? UseNativeFormat { get; }
		Color Foreground { get; }
		Color Background { get; }
		string DisplayText { get; }
		Thickness Border { get; }
		object Value { get; }
		PivotGridField Field { get; }
	}
	public class TextExporter : Control, ITextExportSettings, IPivotTextExportSettings {
		#region static
		public static readonly DependencyProperty ExportSettingsProperty;
		static TextExporter() {
			Type ownerType = typeof(TextExporter);
			ExportSettingsProperty = DependencyProperty.Register("ExportSettings", typeof(IPivotTextExportSettings), ownerType, new PropertyMetadata(null));
		}
		#endregion
		public TextExporter() {
			this.SetDefaultStyleKey(typeof(TextExporter));
		}
		public IPivotTextExportSettings ExportSettings {
			get { return (IPivotTextExportSettings)GetValue(ExportSettingsProperty); }
			set { SetValue(ExportSettingsProperty, value); }
		}
		protected IPivotTextExportSettings ActualExportSettings {
			get { return ExportSettings ?? this; }
		}
		protected
#if !DEBUGTEST 
			sealed
#endif
			override Size MeasureOverride(Size constraint) {
			return constraint;
		}
		protected sealed override Size ArrangeOverride(Size arrangeBounds) {
			return arrangeBounds;
		}
		#region ITextExportSettings Members
		FontFamily ITextExportSettings.FontFamily {
			get { return FontFamily; }
		}
		double ITextExportSettings.FontSize {
			get { return FontSize; }
		}
		FontStyle ITextExportSettings.FontStyle {
			get { return FontStyle; }
		}
		FontWeight ITextExportSettings.FontWeight {
			get { return FontWeight; }
		}
		HorizontalAlignment ITextExportSettings.HorizontalAlignment {
			get { return ActualExportSettings.HorizontalContentAlignment; }
		}
		bool ITextExportSettings.NoTextExport {
			get { return false; }
		}
		Thickness ITextExportSettings.Padding {
			get { return Padding; }
		}
		string ITextExportSettings.Text {
			get { return ActualExportSettings.DisplayText; }
		}
		object ITextExportSettings.TextValue {
			get { return ActualExportSettings.Value; }
		}
		string ITextExportSettings.TextValueFormatString {
			get { return ActualExportSettings.ValueFormat; }
		}
		TextWrapping ITextExportSettings.TextWrapping {
			get { return TextWrapping.NoWrap; }
		}
		VerticalAlignment ITextExportSettings.VerticalAlignment {
			get { return VerticalAlignment; }
		}
		bool? ITextExportSettings.XlsExportNativeFormat {
			get { return ActualExportSettings.UseNativeFormat; }
		}
		string ITextExportSettings.XlsxFormatString {
			get { return ExportSettingDefaultValue.XlsxFormatString; }
		}
		TextDecorationCollection ITextExportSettings.TextDecorations {
			get { return ExportSettingDefaultValue.TextDecorations; }
		}
		TextTrimming ITextExportSettings.TextTrimming {
			get { return TextTrimming.None; }
		}
		#endregion
		#region IExportSettings Members
		Color IExportSettings.Background {
			get { return ActualExportSettings.Background; }
		}
		Color IExportSettings.BorderColor {
			get {
				if(BorderBrush != null)
					return (Color)BorderBrush.GetValue(SolidColorBrush.ColorProperty);
				return Colors.Black;
			}
		}
		Thickness IExportSettings.BorderThickness {
			get { return ActualExportSettings.Border; }
		}
		Color IExportSettings.Foreground {
			get { return ActualExportSettings.Foreground; }
		}
		IOnPageUpdater IExportSettings.OnPageUpdater {
			get { return null; }
		}
		string IExportSettings.Url {
			get { return string.Empty; }
		}
		BorderDashStyle IExportSettings.BorderDashStyle {
			get { return ExportSettingDefaultValue.BorderDashStyle; }
		}
		object IExportSettings.MergeValue {
			get { return null; }
		}
		#endregion
		#region IPivotTextExportSettings Members
		HorizontalAlignment IPivotTextExportSettings.HorizontalContentAlignment {
			get { return HorizontalContentAlignment; }
		}
		string IPivotTextExportSettings.ValueFormat {
			get { return string.Empty; ; }
		}
		bool? IPivotTextExportSettings.UseNativeFormat {
			get { return false; }
		}
		Color IPivotTextExportSettings.Foreground {
			get {
				if(Foreground != null)
					return (Color)Foreground.GetValue(SolidColorBrush.ColorProperty);
				return Colors.Black;
			}
		}
		Color IPivotTextExportSettings.Background {
			get {
				if(Background != null)
					return (Color)Background.GetValue(SolidColorBrush.ColorProperty);
				return Colors.Black;
			}
		}
		string IPivotTextExportSettings.DisplayText {
			get { return string.Empty; }
		}
		Thickness IPivotTextExportSettings.Border {
			get { return BorderThickness; }
		}
		object IPivotTextExportSettings.Value {
			get { return null; }
		}
		PivotGridField IPivotTextExportSettings.Field {
			get { return null; }
		}
		#endregion
	}
}

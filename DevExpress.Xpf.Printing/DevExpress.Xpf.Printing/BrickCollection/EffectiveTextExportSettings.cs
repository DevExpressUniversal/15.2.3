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
namespace DevExpress.Xpf.Printing.BrickCollection {
	class EffectiveTextExportSettings : EffectiveExportSettings, ITextExportSettings {
		public EffectiveTextExportSettings(DependencyObject source)
			: base(source) {
		}
		#region ITextExportSettings Members
		public FontFamily FontFamily {
			get { return GetEffectiveTextExportValue<FontFamily>(TextExportSettings.FontFamilyProperty, o => o.FontFamily); }
		}
		public double FontSize {
			get { return GetEffectiveTextExportValue<double>(TextExportSettings.FontSizeProperty, o => o.FontSize); }
		}
		public FontStyle FontStyle {
			get { return GetEffectiveTextExportValue<FontStyle>(TextExportSettings.FontStyleProperty, o => o.FontStyle); }
		}
		public FontWeight FontWeight {
			get { return GetEffectiveTextExportValue<FontWeight>(TextExportSettings.FontWeightProperty, o => o.FontWeight); }
		}
		public HorizontalAlignment HorizontalAlignment {
			get { return GetEffectiveTextExportValue<HorizontalAlignment>(TextExportSettings.HorizontalAlignmentProperty, o => o.HorizontalAlignment); }
		}
		public bool NoTextExport {
			get { return GetEffectiveTextExportValue<bool>(TextExportSettings.NoTextExportProperty, o => o.NoTextExport); }
		}
		public Thickness Padding {
			get { return GetEffectiveTextExportValue<Thickness>(TextExportSettings.PaddingProperty, o => o.Padding); }
		}
		public string Text {
			get { return GetEffectiveTextExportValue<string>(TextExportSettings.TextProperty, o => o.Text); }
		}
		public object TextValue {
			get { return GetEffectiveTextExportValue<object>(TextExportSettings.TextValueProperty, o => o.TextValue); }
		}
		public string TextValueFormatString {
			get { return GetEffectiveTextExportValue<string>(TextExportSettings.TextValueFormatStringProperty, o => o.TextValueFormatString); }
		}
		public TextWrapping TextWrapping {
			get { return GetEffectiveTextExportValue<TextWrapping>(TextExportSettings.TextWrappingProperty, o => o.TextWrapping); }
		}
		public VerticalAlignment VerticalAlignment {
			get { return GetEffectiveTextExportValue<VerticalAlignment>(TextExportSettings.VerticalAlignmentProperty, o => o.VerticalAlignment); }
		}
		public bool? XlsExportNativeFormat {
			get { return GetEffectiveTextExportValue<bool?>(TextExportSettings.XlsExportNativeFormatProperty, o => o.XlsExportNativeFormat); }
		}
		public string XlsxFormatString {
			get { return GetEffectiveTextExportValue<string>(TextExportSettings.XlsxFormatStringProperty, o => o.XlsxFormatString); }
		}
		public TextDecorationCollection TextDecorations {
			get { return GetEffectiveTextExportValue<TextDecorationCollection>(TextExportSettings.TextDecorationsProperty, o => o.TextDecorations); }
		}
		public TextTrimming TextTrimming {
			get { return GetEffectiveTextExportValue<TextTrimming>(TextExportSettings.TextTrimmingProperty, o => o.TextTrimming); }
		}
		#endregion
		T GetEffectiveTextExportValue<T>(DependencyProperty property, Func<ITextExportSettings, T> getValue) {
			return GetEffectiveValue<T, ITextExportSettings>(property, getValue);
		}
	}
}

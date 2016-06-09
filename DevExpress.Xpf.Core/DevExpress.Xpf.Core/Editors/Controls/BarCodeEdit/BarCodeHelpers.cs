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
using System.Windows;
using PlatformMedia = System.Windows.Media;
using PlatformUI = System.Windows.Media;
using PlatformWindows = System.Windows;
using PlatformXaml = System.Windows;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using System.Windows.Data;
namespace DevExpress.Xpf.Editors.Internal {
	public class AligmentHelper {
		static List<Tuple<XtraPrinting.TextAlignment, PlatformWindows.VerticalAlignment, PlatformWindows.HorizontalAlignment>> list = new List<Tuple<XtraPrinting.TextAlignment, VerticalAlignment, HorizontalAlignment>>();
		static AligmentHelper() {
			AddRow(XtraPrinting.TextAlignment.BottomCenter, VerticalAlignment.Bottom, HorizontalAlignment.Center);
			AddRow(XtraPrinting.TextAlignment.BottomJustify, VerticalAlignment.Bottom, HorizontalAlignment.Stretch);
			AddRow(XtraPrinting.TextAlignment.BottomLeft, VerticalAlignment.Bottom, HorizontalAlignment.Left);
			AddRow(XtraPrinting.TextAlignment.BottomRight, VerticalAlignment.Bottom, HorizontalAlignment.Right);
			AddRow(XtraPrinting.TextAlignment.MiddleCenter, VerticalAlignment.Center, HorizontalAlignment.Center);
			AddRow(XtraPrinting.TextAlignment.MiddleJustify, VerticalAlignment.Center, HorizontalAlignment.Stretch);
			AddRow(XtraPrinting.TextAlignment.MiddleLeft, VerticalAlignment.Center, HorizontalAlignment.Left);
			AddRow(XtraPrinting.TextAlignment.MiddleRight, VerticalAlignment.Center, HorizontalAlignment.Right);
			AddRow(XtraPrinting.TextAlignment.MiddleCenter, VerticalAlignment.Stretch, HorizontalAlignment.Center);
			AddRow(XtraPrinting.TextAlignment.MiddleJustify, VerticalAlignment.Stretch, HorizontalAlignment.Stretch);
			AddRow(XtraPrinting.TextAlignment.MiddleLeft, VerticalAlignment.Stretch, HorizontalAlignment.Left);
			AddRow(XtraPrinting.TextAlignment.MiddleRight, VerticalAlignment.Stretch, HorizontalAlignment.Right);
			AddRow(XtraPrinting.TextAlignment.TopCenter, VerticalAlignment.Top, HorizontalAlignment.Center);
			AddRow(XtraPrinting.TextAlignment.TopJustify, VerticalAlignment.Top, HorizontalAlignment.Stretch);
			AddRow(XtraPrinting.TextAlignment.TopLeft, VerticalAlignment.Top, HorizontalAlignment.Left);
			AddRow(XtraPrinting.TextAlignment.TopRight, VerticalAlignment.Top, HorizontalAlignment.Right);
		}
		static void AddRow(XtraPrinting.TextAlignment textAlignment, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment) {
			list.Add(new Tuple<XtraPrinting.TextAlignment, VerticalAlignment, HorizontalAlignment>(textAlignment, verticalAlignment, horizontalAlignment));
		}
		public static XtraPrinting.TextAlignment GetFullAligment(PlatformWindows.VerticalAlignment verticalContentAlignment, PlatformWindows.HorizontalAlignment horizontalContentAlignment) {
			foreach(var tuple in list) {
				if(tuple.Item2 == verticalContentAlignment && tuple.Item3 == horizontalContentAlignment)
					return tuple.Item1;
			}
			throw new ArgumentException();
		}
	}
	public class XamlEnumTypeConverter<T> : TypeConverter where T : struct {
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			return Enum.Parse(typeof(T), (string)value);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
	}
	public static class WindowsFormsHelper {
		public static PlatformMedia.Brush ConvertBrush(System.Drawing.Brush brush) {
			var color = ((System.Drawing.SolidBrush)brush).Color;
			return new PlatformMedia.SolidColorBrush(ConvertColor(color));
		}
		public static PlatformMedia.Brush ConvertColorToBrush(System.Drawing.Color color) {
			return new PlatformMedia.SolidColorBrush(ConvertColor(color));
		}
		public static PlatformUI.Color ConvertColor(System.Drawing.Color color) {
			return PlatformUI.Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);
		}
		public static PlatformXaml.TextAlignment ConvertStringToTextAlignment(System.Drawing.StringAlignment stringAlignment) {
			switch(stringAlignment) {
				case System.Drawing.StringAlignment.Near:
					return PlatformXaml.TextAlignment.Left;
				case System.Drawing.StringAlignment.Center:
					return PlatformXaml.TextAlignment.Center;
				case System.Drawing.StringAlignment.Far:
					return PlatformXaml.TextAlignment.Right;
			}
			throw new ArgumentException();
		}
		public static System.Drawing.Color ConvertBrushToColor(PlatformMedia.Brush brush) {
			if(brush == null || !(brush is PlatformMedia.SolidColorBrush))
				return System.Drawing.Color.FromArgb(0);
			var color = (brush as PlatformMedia.SolidColorBrush).Color;
			return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
		public static PlatformUI.Color ConvertBrushToColor(System.Drawing.Brush brush) {
			var defaultColor = PlatformUI.Color.FromArgb(0, 0, 0, 0);
			if(brush == null || !(brush is System.Drawing.SolidBrush))
				return defaultColor;
			var color = (brush as System.Drawing.SolidBrush).Color;
			return PlatformUI.Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);
		}
	}
	public class BarCodeData : IBarCodeData {
		public BarCodeData(IBarCodeData barCodeData) {
			Module = barCodeData.Module;
			AutoModule = barCodeData.AutoModule;
			ShowText = barCodeData.ShowText;
			Text = barCodeData.Text;
			Alignment = barCodeData.Alignment;
			Orientation = barCodeData.Orientation;
			Style = barCodeData.Style;
		}
		public double Module { get; set; }
		public bool AutoModule { get; set; }
		public bool ShowText { get; set; }
		public string Text { get; set; }
		public XtraPrinting.TextAlignment Alignment { get; set; }
		public BarCodeOrientation Orientation { get; set; }
		public BrickStyle Style { get; set; }
	}
	public class BaseEditStyleSettingsToBarCodeStyleSettingsConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value as BarCodeStyleSettings;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value as BaseEditStyleSettings;
		}
	}
	public class BaseEditToBarCodeEditConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value as BarCodeEdit;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value as BaseEdit;
		}
	}
}

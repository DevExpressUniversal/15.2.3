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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Localization;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Diagram {
	public class DiagramStringIdConverter : StringIdConverter<DiagramControlStringId> {
		protected override XtraLocalizer<DiagramControlStringId> Localizer { get { return DiagramControlLocalizer.Active; } }
	}
	public class TextAlignmentLocalizationConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			IValueConverter converter = new DiagramStringIdConverter();
			switch (value.ToString()) {
				case "Left": return converter.Convert(value, targetType, "Paragraph_AlignLeft", culture);
				case "Right": return converter.Convert(value, targetType, "Paragraph_AlignRight", culture);
				case "Center": return converter.Convert(value, targetType, "Paragraph_AlignCenter", culture);
				case "Justify": return converter.Convert(value, targetType, "Paragraph_Justify", culture);
				case "Top": return converter.Convert(value, targetType, "Paragraph_AlignTop", culture);
				case "Bottom": return converter.Convert(value, targetType, "Paragraph_AlignBottom", culture);
			}
			return string.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class MeasureUnitLocalizationConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			IValueConverter converter = new DiagramStringIdConverter();
			return converter.Convert(value, targetType, string.Format("MeasureUnit_{0}", value.ToString()), culture);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class HeaderVisibilityConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			ListCollectionView collectionView = value as ListCollectionView;
			if (collectionView == null || collectionView.SourceCollection == null)
				return Visibility.Visible;
			int i = 0;
			foreach (var item in collectionView.SourceCollection)
				if (item is ItemTool && ((ItemTool)item).IsQuick)
					i++;
			return (i == 0) ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public static class DiagramValuesConverter {
		public static double SizeToDouble(Size size) {
			return size.Width;
		}
		public static Size DoubleToSize(double val) {
			return new Size(Math.Round(val), Math.Round(val));
		}
		public static bool ResizingModeToBool(ResizingMode mode) {
			return mode == ResizingMode.Live;
		}
		public static ResizingMode BoolToResizingMode(bool allowLiveResizing) {
			return allowLiveResizing ? ResizingMode.Live : ResizingMode.Preview;
		}
		public static object TextAlignmentToImage(TextAlignment alignment) {
			string name = string.Format("DevExpress.Diagram.Core.Images.Menu.Align{0}_16x16.png", alignment);
			return DiagramImageExtension.GetDiagramImage(name);
		}
		public static object VerticalAlignmentToImage(VerticalAlignment alignment) {
			string name = string.Format("DevExpress.Diagram.Core.Images.Menu.AlignHorizontal{0}_16x16.png", alignment);
			return DiagramImageExtension.GetDiagramImage(name);
		}
		public static object PageSizeToImage(string sizeId) {
			switch (sizeId) {
				case "Letter": case "A4": return DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.PageSizes.SizeMiddle_16x16.png");
				case "A3": return DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.PageSizes.SizeLarge_16x16.png");
				case "A5": return DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.PageSizes.SizeSmall_16x16.png");
			}
			return DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.PageSizes.SizeEmpty_16x16.png");
		}
		public static object CanvasSizeModeToImage(CanvasSizeMode mode) {
			if (mode == CanvasSizeMode.AutoSize)
				return DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.AutoSize_16x16.png");
			else if (mode == CanvasSizeMode.None)
				return DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.NoneAutoSize_16x16.png");
			return null;
		}
		static ImageSource GetImageByName(string name) {
			return (ImageSource)new DXImageExtension() { Image = (DXImageInfo)new DXImageConverter().ConvertFromString(name) }.ProvideValue(null);
		}
	}
	public class ToolIdToImageConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			string name = string.Empty;
			switch (value.ToString()) {
				case "PointerTool": name = "DevExpress.Diagram.Core.Images.Menu.PointerTool_16x16.png"; break;
				case "ConnectorTool": name = "DevExpress.Diagram.Core.Images.Menu.ConnectorTool_16x16.png"; break;
				case "Rectangle": name = "DevExpress.Diagram.Core.Images.Menu.RectangleTool_16x16.png"; break;
				case "Ellipse": name = "DevExpress.Diagram.Core.Images.Menu.EllipseTool_16x16.png"; break;
				case "RightTriangle": name = "DevExpress.Diagram.Core.Images.Menu.RightTriangleTool_16x16.png"; break;
				case "Hexagon": name = "DevExpress.Diagram.Core.Images.Menu.HexagonTool_16x16.png"; break;
			}
			return string.IsNullOrEmpty(name) ? null : DiagramImageExtension.GetDiagramImage(name);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class ToolIdToShortCutConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			string shortCutGroupId = parameter.ToString();
			if (shortCutGroupId == "Tools")
				switch (value.ToString()) {
					case "PointerTool": return "Ctrl+1";
					case "ConnectorTool": return "Ctrl+3";
					case "Rectangle": return "Ctrl+8";
					case "Ellipse": return "Ctrl+9";
				}
			if (shortCutGroupId == "VerticalAlignment")
				switch (value.ToString()) {
					case "Top": return "Ctrl+Shift+T";
					case "Center": return "Ctrl+Shift+M";
					case "Bottom": return "Ctrl+Shift+V";
				}
			if (shortCutGroupId == "HorizontalAlignment")
				switch (value.ToString()) {
					case "Left": return "Ctrl+Shift+L";
					case "Center": return "Ctrl+Shift+C";
					case "Right": return "Ctrl+Shift+R";
					case "Justify": return "Ctrl+Shift+J";
				}
			return string.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class StyleIdToGroupNameConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Regex regex = new Regex(@"^variant[0-9]+.*", RegexOptions.IgnoreCase);
			if (regex.IsMatch(value.ToString()))
				return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonGallery_VariantStyles);
			return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonGallery_ThemeStyles);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class InvertThicknessConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is Thickness)
				return ((Thickness)value).Invert();
			else
				throw new NotSupportedException();
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class StyleInfoToStyleIdConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (value as DiagramItemEditUnit).With(styleInfo => styleInfo.StyleId);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return (value as DiagramItemStyleId).With(styleId => DiagramItemEditUnit.CreateStyleEditUnit(styleId));
		}
	}
	public class DoubleToDecimalConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (decimal)(double)value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return (double)(decimal)value;
		}
	}
	public class ItemPropertiesToStringConverter : MarkupExtension, IMultiValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			string result = string.Empty;
			if (values[0] != null && values[0] != DependencyProperty.UnsetValue) {
				Size size = (Size)values[0];
				result = string.Format("{0}: {1:0}    {2}: {3:0}", DiagramControlLocalizer.GetString(DiagramControlStringId.ShapeInfo_Width).ToUpper(), size.Width,
										DiagramControlLocalizer.GetString(DiagramControlStringId.ShapeInfo_Height).ToUpper(), size.Height);
			}
			if (values[1] != null && values[0] != DependencyProperty.UnsetValue) {
				double angle = (double)values[1];
				result += string.Format("    {0}: {1:0}\u00B0", DiagramControlLocalizer.GetString(DiagramControlStringId.ShapeInfo_Angle).ToUpper(), angle);
			}
			return result;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class IsQuickToolsConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			ObservableCollection<ItemTool> Tools = value as ObservableCollection<ItemTool>;
			if (Tools == null || Tools.Count == 0) return value;
			ItemTool firstQuickItem = Tools.FirstOrDefault(x => x.IsQuick);
			if (firstQuickItem == null || firstQuickItem == Tools.ElementAt(0)) return value;
			ObservableCollection<ItemTool> movedTools = new ObservableCollection<ItemTool>() { firstQuickItem };
			foreach (var item in Tools)
				if (item != firstQuickItem) movedTools.Add(item);
			return movedTools;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class NonlinearZoomConverter : MarkupExtension, IValueConverter {
		const double zoom0Scale = 0.1;
		const double zoomHalfScale = 1.0;
		const double zoomFullScale = 4.0;
		const double halfScaleLowBound = 0.44;
		const double halfScaleHighBound = 0.56;
		double coeff_A;
		double coeff_B;
		double coeff_C;
		public NonlinearZoomConverter() {
			coeff_A = (zoom0Scale * zoomFullScale - zoomHalfScale * zoomHalfScale) / (zoom0Scale - 2 * zoomHalfScale + zoomFullScale);
			coeff_B = (zoomHalfScale - zoom0Scale) * (zoomHalfScale - zoom0Scale) / (zoom0Scale - 2 * zoomHalfScale + zoomFullScale);
			coeff_C = 2 * Math.Log((zoomFullScale - zoomHalfScale) / (zoomHalfScale - zoom0Scale));
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double scale = Math.Log(((double)value - coeff_A) / coeff_B) / coeff_C;
			if (scale > halfScaleLowBound && scale < halfScaleHighBound) return 0.5;
			return scale;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			double scale = (double)value;
			if (scale > halfScaleLowBound && scale < halfScaleHighBound) return 1.0;
			return coeff_A + coeff_B * Math.Exp(coeff_C * scale);
		}
	}
}

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
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Office.Internal;
using DevExpress.Xpf.Spreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Xpf.Spreadsheet.Localization;
using System.Collections.Generic;
using System.Collections;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class GridLengthConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (parameter != null && parameter.ToString() == "Star") ? new GridLength((double)value, GridUnitType.Star) : new GridLength((double)value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
	public class BoolToGridLengthConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (bool)value ? GetGridLengthFromString(parameter) : new GridLength(0);
		}
		private GridLength GetGridLengthFromString(object parameter) {
			if (parameter == null) return new GridLength(0);
			string str = parameter.ToString();
			if (str.Contains("*")) {
				double legth = double.Parse(str.Substring(0, str.IndexOf('*')));
				return new GridLength(legth, GridUnitType.Star);
			}
			else if (str == "Auto")
				return new GridLength(1, GridUnitType.Auto);
			return new GridLength(double.Parse(str));
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
	public class BrushToColorConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return System.Drawing.Color.FromArgb(0, 0, 0, 0);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			SolidColorBrush brush = parameter as SolidColorBrush;
			if (brush != null) return XpfTypeConverter.ToPlatformIndependentColor(brush.Color);
			else return System.Drawing.Color.FromArgb(0, 0, 0, 0);
		}
	}
	public class RectToGeometryConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return new RectangleGeometry((Rect)value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
	public class SelectedItemIndexToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return int.Parse(value.ToString()) == int.Parse(parameter.ToString()) ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
	public class IntToHyperLinkTypeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (int)value;
		}
		private HyperlinkType GetLinkType(int value) {
			switch (value) {
				case 1:
					return HyperlinkType.PlaceInThisDocument;
				case 2:
					return HyperlinkType.EmailAddress;
				default:
					return HyperlinkType.ExistingFileOrWebPage;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return GetLinkType((int)value);
		}
	}
	public class BoolToIntConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (int)value == int.Parse(parameter.ToString());
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (bool)value == true ? int.Parse(parameter.ToString()) : -1;
		}
	}
	public class SystemColorToMediaColor : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value == null)
				return (System.Drawing.Color.Empty).ToWpfColor();
			return ((System.Drawing.Color)value).ToWpfColor();
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Color color = (Color)value;
			return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
		#endregion
	}
	public class CheckStateToBoolConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			CheckState state = (CheckState)value;
			switch (state) {
				case CheckState.Unchecked: return false;
				case CheckState.Checked: return true;
				default: return null;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			bool? isChecked = (bool?)value;
			switch (isChecked) {
				case true: return CheckState.Checked;
				case false: return CheckState.Unchecked;
				default: return CheckState.Indeterminate;
			}
		}
		#endregion
	}
	public class UnderlineTypeToTextDecorationsConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			TextDecorationCollection decoration = null;
			if ((string)value == XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_Single)
				|| (string)value == XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_SingleAccounting))
				decoration = TextDecorations.Underline;
			return decoration;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class BoolToThicknessConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			int thickness = parameter != null ? int.Parse(parameter.ToString()) : 0;
			bool applyThickness = value != null ? (bool)value : false;
			return applyThickness ? new Thickness(thickness) : new Thickness(0);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class ByteToIntConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			byte? byteValue = (byte?)value;
			if (byteValue.HasValue)
				return (int)byteValue.Value;
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class ColorToBrushConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return new SolidColorBrush((Color)value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class InitialTabPageToIntConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (int)value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class StringToFontStyleConveter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return GetFontStyle(value != null ? value.ToString() : string.Empty);
		}
		private FontStyle GetFontStyle(string value) {
			switch (value) {
				case "Italic": return FontStyles.Italic;
				case "Bold Italic": return FontStyles.Italic;
				default: return FontStyles.Normal;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class StringToFontWeightConveter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return GetFontWeight(value != null ? value.ToString() : string.Empty);
		}
		private FontWeight GetFontWeight(string value) {
			switch (value) {
				case "Bold": return FontWeights.Bold;
				case "Bold Italic": return FontWeights.Bold;
				default: return FontWeights.Normal;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class StringToFontFamilyConveter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return GetFontFamily(value != null ? value.ToString() : String.Empty);
		}
		private FontFamily GetFontFamily(string value) {
			return string.IsNullOrEmpty(value) ? new FontFamily() : new FontFamily(value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class NoneToNullFilterOperatorConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (GenericFilterOperator)value == GenericFilterOperator.None ? null : value.ToString().ToLower();
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Dictionary<string, GenericFilterOperator> filters = new Dictionary<string, GenericFilterOperator>();
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorNone), GenericFilterOperator.None);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorEquals), GenericFilterOperator.Equals);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotEqual), GenericFilterOperator.DoesNotEqual);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorGreater), GenericFilterOperator.Greater);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorGreaterOrEqual), GenericFilterOperator.GreaterOrEqual);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorLess), GenericFilterOperator.Less);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorLessOrEqual), GenericFilterOperator.LessOrEqual);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBeginsWith), GenericFilterOperator.BeginsWith);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotBeginWith), GenericFilterOperator.DoesNotBeginWith);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorEndsWith), GenericFilterOperator.EndsWith);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotEndWith), GenericFilterOperator.DoesNotEndWith);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorContains), GenericFilterOperator.Contains);
			filters.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotContain), GenericFilterOperator.DoesNotContain);
			foreach (string findString in filters.Keys)
				if (findString == value.ToString()) return filters[findString];
			return null;
		}
		#endregion
	}
	public class IsDateTimeToVisibilityConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			string visible = "Visible";
			string collapsed = "Collapsed";
			return (bool)value == true ? visible : collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class InverseBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return !(bool)value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return Convert(value, targetType, parameter, culture);
		}
	}
	public class NullToFloatHeightAndWidthConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class ProtectedToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return bool.Parse(value.ToString()) ? Visibility.Hidden : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
	public class InverseVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			bool isVisible = !(bool)value;
			if (isVisible)
				return Visibility.Visible;
			else
				return Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	#region PivotTableFieldListStartPositionConveter
	public class PivotTableFieldListStartPositionConveter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (DevExpress.Xpf.Spreadsheet.SpreadsheetPivotTableFieldListStartPosition)value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (DevExpress.XtraSpreadsheet.SpreadsheetPivotTableFieldListStartPosition)value;
		}
		#endregion
	}
	#endregion
	#region DrawingSizeToWindowsSizeConveter
	public class DrawingSizeToWindowsSizeConveter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			System.Drawing.Size size = (System.Drawing.Size)value;
			return new Size(size.Width, size.Height);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Size size = (Size)value;
			return new System.Drawing.Size((int)size.Width, (int)size.Height);
		}
		#endregion
	}
	#endregion
	#region DrawingPointToWindowsPointConveter
	public class DrawingPointToWindowsPointConveter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			System.Drawing.Point point = (System.Drawing.Point)value;
			return new Point(point.X, point.Y);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Point point = (Point)value;
			return new System.Drawing.Point((int)point.X, (int)point.Y);
		}
		#endregion
	}
	#endregion
	public class DecimalToDoubleConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return System.Convert.ToDecimal(value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return System.Convert.ToDouble(value);
		}
		#endregion
	}
}

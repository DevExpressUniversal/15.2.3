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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using SWC = System.Windows.Controls;
using CoreDock = System.Windows.Controls.Dock;
namespace DevExpress.Xpf.Docking.VisualElements {
	[ValueConversion(typeof(CoreDock), typeof(CoreDock))]
	public class DockInverseConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			CoreDock dock = (CoreDock)value;
			switch(dock) {
				case CoreDock.Left:
					return CoreDock.Right;
				case CoreDock.Bottom:
					return CoreDock.Top;
				case CoreDock.Top:
					return CoreDock.Bottom;
			}
			return CoreDock.Left;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	[ValueConversion(typeof(SWC.Dock), typeof(Thickness)), Obsolete]
	public class DockTypeToThicknessRotateConverter : IValueConverter {
		public Thickness Thickness { get; set; }
		public bool Inverted { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			SWC.Dock dock = (SWC.Dock)value;
			double left, top, right, bottom;
			switch(dock) {
				case SWC.Dock.Left:
					left = Thickness.Bottom;
					top = Thickness.Left;
					right = Thickness.Top;
					bottom = Thickness.Right;
					break;
				case SWC.Dock.Top:
					left = Thickness.Left;
					top = Thickness.Bottom;
					right = Thickness.Right;
					bottom = Thickness.Top;
					break;
				case SWC.Dock.Right:
					left = Thickness.Top;
					top = Thickness.Left;
					right = Thickness.Bottom;
					bottom = Thickness.Right;
					break;
				default:
					left = Thickness.Left;
					top = Thickness.Top;
					right = Thickness.Right;
					bottom = Thickness.Bottom;
					break;
			}
			return new Thickness(left, top, right, bottom);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return Inverted ? SWC.Dock.Right : SWC.Dock.Left;
		}
	}
	[ValueConversion(typeof(SWC.Dock), typeof(Thickness))]
	public class DockTypeToThicknessConverter : IValueConverter {
		public int All { get; set; }
		public bool Inverted { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			SWC.Dock dock = (SWC.Dock)value;
			Thickness thickness = new Thickness(Inverted ? 0 : All);
			switch(dock) {
				case SWC.Dock.Left:
					if(Inverted) thickness.Right = All;
					else thickness.Left = 0;
					break;
				case SWC.Dock.Top:
					if(Inverted) thickness.Bottom = All;
					else thickness.Top = 0;
					break;
				case SWC.Dock.Right:
					if(Inverted) thickness.Left = All;
					else thickness.Right = 0;
					break;
				case SWC.Dock.Bottom:
					if(Inverted) thickness.Top = All;
					else thickness.Bottom = 0;
					break;
			}
			return thickness;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			Thickness thickness = (Thickness)value;
			double side = Inverted ? All : 0;
			SWC.Dock dock = Inverted ? SWC.Dock.Right : SWC.Dock.Left;
			if(thickness.Top == side) dock = Inverted ? SWC.Dock.Bottom : SWC.Dock.Top;
			if(thickness.Right == side) dock = Inverted ? SWC.Dock.Left : SWC.Dock.Right;
			if(thickness.Bottom == side) dock = Inverted ? SWC.Dock.Top : SWC.Dock.Bottom;
			return dock;
		}
	}
	[ValueConversion(typeof(SWC.Dock), typeof(HorizontalAlignment))]
	public class DockToHorizontalAlignmentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			SWC.Dock dock = (SWC.Dock)value;
			HorizontalAlignment alignment;
			switch(dock) {
				case SWC.Dock.Left:
					alignment = HorizontalAlignment.Right;
					break;
				case SWC.Dock.Right:
					alignment = HorizontalAlignment.Left;
					break;
				default:
					alignment = HorizontalAlignment.Stretch;
					break;
			}
			return alignment;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	[ValueConversion(typeof(SWC.Dock), typeof(VerticalAlignment))]
	public class DockToVerticalAlignmentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			SWC.Dock dock = (SWC.Dock)value;
			VerticalAlignment alignment;
			switch(dock) {
				case SWC.Dock.Top:
					alignment = VerticalAlignment.Bottom;
					break;
				case SWC.Dock.Bottom:
					alignment = VerticalAlignment.Top;
					break;
				default:
					alignment = VerticalAlignment.Stretch;
					break;
			}
			return alignment;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	[ValueConversion(typeof(double), typeof(Thickness))]
	public class DoubleToThicknessConverter : IValueConverter {
		public bool All { get; set; }
		public SWC.Dock Direction { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double d = (double)value;
			Thickness thickness = new Thickness(All ? d : 0);
			if(!All) {
				switch(Direction) {
					case SWC.Dock.Left:
						thickness.Left = d;
						break;
					case SWC.Dock.Top:
						thickness.Top = d;
						break;
					case SWC.Dock.Right:
						thickness.Right = d;
						break;
					case SWC.Dock.Bottom:
						thickness.Bottom = d;
						break;
				}
			}
			return thickness;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	[ValueConversion(typeof(SWC.Dock), typeof(Cursor))]
	public class DockTypeToCursorConverter : IValueConverter {
		public int All { get; set; }
		public bool Inverted { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			SWC.Dock dock = (SWC.Dock)value;
			Cursor cursor = dock.ToCursor();
			return cursor;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return SWC.Dock.Left;
		}
	}
	[ValueConversion(typeof(object), typeof(bool)), Obsolete]
	public class NotNullConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value != null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	[ValueConversion(typeof(double), typeof(GridLength))]
	public class DoubleToGridLengthConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return new GridLength(Layout.Core.MathHelper.CalcRealDimension((double)value), GridUnitType.Pixel);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return ((GridLength)value).Value;
		}
	}
	[ValueConversion(typeof(CaptionLocation), typeof(CoreDock))]
	public class CaptionLocationToDockConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			CaptionLocation captionLocation = (CaptionLocation)value;
			switch(captionLocation) {
				case CaptionLocation.Right:
					return CoreDock.Right;
				case CaptionLocation.Bottom:
					return CoreDock.Bottom;
				case CaptionLocation.Top:
					return CoreDock.Top;
				default:
					return CoreDock.Left;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	[ValueConversion(typeof(CaptionLocation), typeof(CoreDock))]
	public class GroupCaptionLocationToDockConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			CaptionLocation captionLocation = (CaptionLocation)value;
			switch(captionLocation) {
				case CaptionLocation.Right:
					return CoreDock.Right;
				case CaptionLocation.Bottom:
					return CoreDock.Bottom;
				case CaptionLocation.Left:
					return CoreDock.Left;
				default:
					return CoreDock.Top;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	[ValueConversion(typeof(CaptionLocation), typeof(CoreDock))]
	public class TabHeaderCaptionLocationToDockConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			CaptionLocation captionLocation = (CaptionLocation)value;
			switch(captionLocation) {
				case CaptionLocation.Right:
				case CaptionLocation.Left:
					return CoreDock.Top;
				default:
					return CoreDock.Left;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	[ValueConversion(typeof(CaptionLocation), typeof(Orientation))]
	public class TabHeaderCaptionLocationToOrientationConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			CaptionLocation captionLocation = (CaptionLocation)value;
			switch(captionLocation) {
				case CaptionLocation.Right:
				case CaptionLocation.Left:
					return Orientation.Vertical;
				default:
					return Orientation.Horizontal;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	[ValueConversion(typeof(Orientation), typeof(Orientation))]
	public class OrientationInvertConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			Orientation orientation = (Orientation)value;
			switch(orientation) {
				case Orientation.Horizontal:
					return Orientation.Vertical;
				default:
					return Orientation.Horizontal;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	[Obsolete]
	[ValueConversion(typeof(bool), typeof(CornerRadius))]
	public class BoolToCornerRadiusConverter : IValueConverter {
		public CornerRadius FalseValue { get; set; }
		public CornerRadius TrueValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (bool)value ? TrueValue : FalseValue;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	[ValueConversion(typeof(bool), typeof(Cursor))]
	public class ConditionalCursorConverter : IValueConverter {
		public Cursor Cursor { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (bool)value ? Cursor : Cursors.Arrow;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	[ValueConversion(typeof(ImageLocation), typeof(CoreDock))]
	public class ImageLocationToDockConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (ImageLocation)value == ImageLocation.AfterText ? CoreDock.Right : CoreDock.Left;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	[ValueConversion(typeof(Size), typeof(double))]
	abstract class SizeConverter : IValueConverter {
		bool IsWidth;
		double NaNResult;
		protected SizeConverter(bool isWidth, double nanResult) {
			this.IsWidth = isWidth;
			this.NaNResult = nanResult;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Size size = (Size)value;
			double result = IsWidth ? size.Width : size.Height;
			return double.IsNaN(result) ? NaNResult : result;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	class MinSizeConverter : SizeConverter {
		public MinSizeConverter(bool isWidth)
			: base(isWidth, 0) {
		}
	}
	class MaxSizeConverter : SizeConverter {
		public MaxSizeConverter(bool isWidth)
			: base(isWidth, double.PositiveInfinity) {
		}
	}
	[Obsolete]
	public class BoolInverseConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return !(bool)value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException("BoolInverseConverter.ConvertBack");
		}
	}
	class BooleanAndConverter : IMultiValueConverter {
		public BooleanAndConverter() {
			UseNullableValues = false;
			NullValue = false;
		}
		public bool UseNullableValues { get; set; }
		public bool NullValue { get;set; }
		object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			foreach(var value in values) {
				if (value == DependencyProperty.UnsetValue)
					return DependencyProperty.UnsetValue;
				var currentValue = false;
				if (UseNullableValues) {
					if (!((bool?)value).HasValue)
						currentValue = NullValue;
					else
						currentValue = ((bool?)value).Value;
				} else
					currentValue = (bool)value;
				if (!currentValue)
					return false;
			}
			return true;
		}
		object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}

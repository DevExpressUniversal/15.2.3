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
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Data.Utils;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Markup;
using SystemMediaColor = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
using Brush = System.Windows.Media.Brush;
using LineGeometry = System.Windows.Media.LineGeometry;
using GeometryGroup = System.Windows.Media.GeometryGroup;
using System.Windows.Media;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Core;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PlatformIndependentColor = System.Windows.Media.Color;
#else
using PlatformIndependentColor = System.Drawing.Color;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.Xpf.Core {
	public static class ConverterHolder {
		static readonly BooleanToVisibilityConverter booleanToVisibilityConverter = new BooleanToVisibilityConverter();
		public static BooleanToVisibilityConverter BooleanToVisibilityConverter { get { return booleanToVisibilityConverter; } }
	}
	public static class DXColorConverter {
#if SILVERLIGHT
		public static System.Windows.Media.Color ToMediaColor(System.Windows.Media.Color color) {
			return color;
		}
		public static PlatformIndependentColor ToDrawingColor(System.Windows.Media.Color color) {
			return PlatformIndependentColor.FromArgb(color.A, color.R, color.G, color.B);
		}
#else
		public static System.Windows.Media.Color ToMediaColor(System.Drawing.Color color) {
			return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
		public static System.Drawing.Color ToDrawingColor(System.Windows.Media.Color color) {
			return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
#endif
		public static SystemMediaColor FromHSB(double h, double s, double b) {
			double hTemp = h / 60;
			int hi = (int)Math.Floor(hTemp) % 6;
			double f = hTemp - Math.Floor(hTemp);
			double p = b * (1 - s);
			double q = b * (1 - f * s);
			double t = b * (1 - (1 - f) * s);
			XtraSchedulerDebug.Assert(hi >= 0 && hi <= 5);
			switch (hi) {
				case 0:
					return SystemMediaColor.FromArgb(255, (byte)(b * 255), (byte)(t * 255), (byte)(p * 255));
				case 1:
					return SystemMediaColor.FromArgb(255, (byte)(q * 255), (byte)(b * 255), (byte)(p * 255));
				case 2:
					return SystemMediaColor.FromArgb(255, (byte)(p * 255), (byte)(b * 255), (byte)(t * 255));
				case 3:
					return SystemMediaColor.FromArgb(255, (byte)(p * 255), (byte)(q * 255), (byte)(b * 255));
				case 4:
					return SystemMediaColor.FromArgb(255, (byte)(t * 255), (byte)(t * 255), (byte)(b * 255));
				case 5:
					return SystemMediaColor.FromArgb(255, (byte)(b * 255), (byte)(p * 255), (byte)(q * 255));
				default:
					Exceptions.ThrowInternalException();
					return new SystemMediaColor();
			}
		}
		public static double GetHue(PlatformIndependentColor baseColor) {
			return baseColor.GetHue();
		}
		public static double GetSaturation(PlatformIndependentColor baseColor) {
			double min = Math.Min(Math.Min(baseColor.R, baseColor.G), baseColor.B);
			double max = Math.Max(Math.Max(baseColor.R, baseColor.G), baseColor.B);
			if (max == 0)
				return 0;
			return 1 - (min / max);
		}
		public static double GetBrightness(PlatformIndependentColor baseColor) {
			return Math.Max(Math.Max(baseColor.R, baseColor.G), baseColor.B) / 255.0;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class ColorToBrushConverter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
#if !SL
			Color color = new Color();
			if (value.GetType() == typeof(System.Drawing.Color)) {
				System.Drawing.Color mColor = (System.Drawing.Color)value;
				color = Color.FromArgb(mColor.A, mColor.R, mColor.G, mColor.B);
			}
			else
				color = (Color)value;
			return new SolidColorBrush(color);
#else
			return new SolidColorBrush((Color)value);
#endif
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return ((SolidColorBrush)value).Color;
		}
	}
#if !SL
	public class DataItemThumbnailColorToBrushConverterExtension : MarkupExtension, IValueConverter {
		DataItemThumbnailColorToBrushConverterExtension instance = null;
		public DataItemThumbnailColorToBrushConverterExtension() {
		}
		public Color DefaultColor { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null) { 
				instance = new DataItemThumbnailColorToBrushConverterExtension();
				instance.DefaultColor = DefaultColor;
			}
			return instance;
		}
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			SystemMediaColor color = new SystemMediaColor();
			if (value.GetType() == typeof(System.Drawing.Color)) {
				System.Drawing.Color mColor = (System.Drawing.Color)value;
				color = SystemMediaColor.FromArgb(mColor.A, mColor.R, mColor.G, mColor.B);
			}
			else
				color = (SystemMediaColor)value;
			AppointmentColorConverter appointmentColorConverter = new AppointmentColorConverter();
			Color convertedColor = (Color)appointmentColorConverter.Convert(color, typeof(Color), DefaultColor, System.Globalization.CultureInfo.CurrentCulture);
			return new SolidColorBrush(convertedColor);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return ((SolidColorBrush)value).Color;
		}
		#endregion
	}
#endif
	public class BoolToVisibilityConverter : IValueConverter {
		bool inverse;
		public bool Inverse { get { return inverse; } set { inverse = value; } }
		#region IValueConverter
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is bool) || targetType != typeof(Visibility))
				return null;
			return ((bool)value ^ Inverse) ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is Visibility) || targetType != typeof(bool))
				return null;
			return ((Visibility)value == Visibility.Visible) ^ Inverse;
		}
		#endregion
	}
	public class BoolToVisibilityConverterExtension : MarkupExtension, IValueConverter {
		bool inverse;
		public bool Inverse { get { return inverse; } set { inverse = value; } }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new BoolToVisibilityConverterExtension() { Inverse = this.Inverse };
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is bool) || targetType != typeof(Visibility))
				return null;
			return ((bool)value ^ Inverse) ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is Visibility) || targetType != typeof(bool))
				return null;
			return ((Visibility)value == Visibility.Visible) ^ Inverse;
		}
		#endregion
	}
	public class StringToVisibilityConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string str = value as string;
			if (str == null || targetType != typeof(Visibility))
				return null;
			return String.IsNullOrEmpty(str) ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class TimeSpanToDateTimeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is TimeSpan))
				return null;
			TimeSpan timeSpan = (TimeSpan)value;
			return new DateTime(timeSpan.Ticks);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is DateTime))
				return null;
			DateTime dateTime = (DateTime)value;
			return TimeSpan.FromTicks(dateTime.Ticks);
		}
	}
	public class SchedulerTimeCellHeightConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is int))
				return null;
			int cellHeight = (int)value;
			if (cellHeight == 0)
				return 20;
			return (cellHeight < 0) ? 0 : cellHeight;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DayViewCellBackgroundBrushConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType != typeof(Brush))
				return null;
			WorkTimeCellBase cell = value as WorkTimeCellBase;
			if (cell == null)
				return null;
			if (cell.IsWorkTime)
				return cell.Brushes.CellLight;
			else
				return cell.Brushes.Cell;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class DayViewCellBackgroundBrushSelector : ICellBrushSelector, ISchedulerDefaultCellBrushSelector {
		#region ICellBrushSelector Members
		public Brush SelectBrush(VisualResourceCellBaseContent content) {
			VisualWorkTimeCellBaseContent workTimeCell = content as VisualWorkTimeCellBaseContent;
			if (workTimeCell == null)
				return null;
			if (workTimeCell.Brushes == null)
				return null;
			if (workTimeCell.IsWorkTime)
				return workTimeCell.Brushes.CellLight;
			else
				return workTimeCell.Brushes.Cell;
		}
		#endregion
	}
	public class DayViewAllDayAreaCellBorderBrushSelector : ICellBrushSelector, ISchedulerDefaultCellBrushSelector {
		#region ICellBrushSelector Members
		public Brush SelectBrush(VisualResourceCellBaseContent content) {
			VisualWorkTimeCellBaseContent workTimeCell = content as VisualWorkTimeCellBaseContent;
			if (workTimeCell == null)
				return null;
			if (workTimeCell.Brushes == null)
				return null;
			if (workTimeCell.IsWorkTime)
				return workTimeCell.Brushes.CellLightBorderDark;
			else
				return workTimeCell.Brushes.CellBorderDark;
		}
		#endregion
	}
	public class DayViewTimeCellBorderBrushSelector : ICellBrushSelector, ISchedulerDefaultCellBrushSelector {
		#region ICellBrushSelector Members
		public Brush SelectBrush(VisualResourceCellBaseContent content) {
			VisualTimeCellContent workTimeCell = content as VisualTimeCellContent;
			if (workTimeCell == null)
				return null;
			if (workTimeCell.Brushes == null)
				return null;
			if (workTimeCell.IsWorkTime)
				return workTimeCell.EndOfHour ? workTimeCell.Brushes.CellLightBorderDark : workTimeCell.Brushes.CellLightBorder;
			else
				return workTimeCell.EndOfHour ? workTimeCell.Brushes.CellBorderDark : workTimeCell.Brushes.CellBorder;
		}
		#endregion
	}
	public class WeekViewCellBackgroundBrushSelector : ICellBrushSelector, ISchedulerDefaultCellBrushSelector {
		#region ICellBrushSelector Members
		public Brush SelectBrush(VisualResourceCellBaseContent content) {
			VisualDateCellContent dateCell = content as VisualDateCellContent;
			if (dateCell == null)
				return null;
			if (dateCell.Brushes == null)
				return null;
			return dateCell.Brushes.CellLight;
		}
		#endregion
	}
	public class WeekViewCellBorderBrushSelector : ICellBrushSelector, ISchedulerDefaultCellBrushSelector {
		#region ICellBrushSelector Members
		public Brush SelectBrush(VisualResourceCellBaseContent content) {
			VisualDateCellContent dateCell = content as VisualDateCellContent;
			if (dateCell == null)
				return null;
			if (dateCell.Brushes == null)
				return null;
			return dateCell.Brushes.CellLightBorderDark;
		}
		#endregion
	}
	public class MonthViewCellBackgroundBrushSelector : ICellBrushSelector, ISchedulerDefaultCellBrushSelector {
		#region ICellBrushSelector Members
		public Brush SelectBrush(VisualResourceCellBaseContent content) {
			VisualDateCellContent dateCell = content as VisualDateCellContent;
			if (dateCell == null)
				return null;
			if (dateCell.Brushes == null)
				return null;
			if (dateCell.IsEvenMonth)
				return dateCell.Brushes.CellLight;
			else
				return dateCell.Brushes.Cell;
		}
		#endregion
	}
	public class DayViewCellBackgroundBrushConverterExtension : MarkupExtension, IValueConverter {
		static DayViewCellBackgroundBrushConverterExtension instance = null;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new DayViewCellBackgroundBrushConverterExtension();
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType != typeof(Brush))
				return null;
			WorkTimeCellBase cell = value as WorkTimeCellBase;
			if (cell == null)
				return null;
			if (cell.IsWorkTime)
				return cell.Brushes.CellLight;
			else
				return cell.Brushes.Cell;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class SizeToHalfHeightConverterExtension : MarkupExtension, IValueConverter {
		static SizeToHalfHeightConverterExtension instance = null;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new SizeToHalfHeightConverterExtension();
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType != typeof(double) || !(value is Size))
				return null;
			return ((Size)value).Height / 2;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class DayViewCellBorderBrushConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType != typeof(Brush))
				return null;
			WorkTimeCellBase cell = value as WorkTimeCellBase;
			if (cell == null)
				return null;
			if (cell.IsWorkTime)
				return cell.Brushes.CellLightBorder;
			else
				return cell.Brushes.CellBorder;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class DayViewCellBorderBrushConverterExtension : MarkupExtension, IValueConverter {
		static DayViewCellBorderBrushConverterExtension instance;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new DayViewCellBorderBrushConverterExtension();
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType != typeof(Brush))
				return null;
			WorkTimeCellBase cell = value as WorkTimeCellBase;
			if (cell == null)
				return null;
			if (cell.IsWorkTime)
				return cell.Brushes.CellLightBorder;
			else
				return cell.Brushes.CellBorder;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}	
	public class MonthCellBorderBrushConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType != typeof(Brush))
				return null;
			DateCell cell = value as DateCell;
			if (cell == null)
				return null;
			if (cell.Interval.Start.Month % 2 != 0)
				return cell.Brushes.CellLightBorderDark;
			else
				return cell.Brushes.CellBorderDark;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class MonthCellBorderBrushConverterExtension : MarkupExtension, IValueConverter {
		static MonthCellBorderBrushConverterExtension instance;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new MonthCellBorderBrushConverterExtension();
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType != typeof(Brush))
				return null;
			DateCell cell = value as DateCell;
			if (cell == null)
				return null;
			if (cell.Interval.Start.Month % 2 != 0)
				return cell.Brushes.CellLightBorderDark;
			else
				return cell.Brushes.CellBorderDark;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class TimelineViewCellBackgroundBrushConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType != typeof(Brush))
				return null;
			SingleTimelineCell cell = value as SingleTimelineCell;
			if (cell == null)
				return null;
			if (cell.IsWorkTime)
				return cell.Brushes.CellLight;
			else
				return cell.Brushes.Cell;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class TimelineViewCellBackgroundBrushConverterExtension : MarkupExtension, IValueConverter {
		static TimelineViewCellBackgroundBrushConverterExtension instance;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new TimelineViewCellBackgroundBrushConverterExtension();
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType != typeof(Brush))
				return null;
			SingleTimelineCell cell = value as SingleTimelineCell;
			if (cell == null)
				return null;
			if (cell.IsWorkTime)
				return cell.Brushes.CellLight;
			else
				return cell.Brushes.Cell;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class TimelineViewCellBorderBrushConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType != typeof(Brush))
				return null;
			SingleTimelineCell cell = value as SingleTimelineCell;
			if (cell == null)
				return null;
			if (cell.IsWorkTime)
				return cell.Brushes.CellLightBorderDark;
			else
				return cell.Brushes.CellBorderDark;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class TimelineViewCellBackgroundBrushSelector : ICellBrushSelector, ISchedulerDefaultCellBrushSelector {
		#region ICellBrushSelector Members
		public Brush SelectBrush(VisualResourceCellBaseContent content) {
			VisualSingleTimelineCellContent cell = content as VisualSingleTimelineCellContent;
			if (cell == null)
				return null;
			if (cell.Brushes == null)
				return null;
			return cell.IsWorkTime ? cell.Brushes.CellLight : cell.Brushes.Cell;
		}
		#endregion
	}
	public class TimelineViewCellBorderBrushSelector : ICellBrushSelector, ISchedulerDefaultCellBrushSelector {
		#region ICellBrushSelector Members
		public Brush SelectBrush(VisualResourceCellBaseContent content) {
			VisualSingleTimelineCellContent cell = content as VisualSingleTimelineCellContent;
			if (cell == null)
				return null;
			if (cell.Brushes == null)
				return null;
			return cell.IsWorkTime ? cell.Brushes.CellLightBorderDark : cell.Brushes.CellBorderDark;
		}
		#endregion
	}
	public class TimelineViewCellBorderBrushConverterExtension : MarkupExtension, IValueConverter {
		static TimelineViewCellBorderBrushConverterExtension instance;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new TimelineViewCellBorderBrushConverterExtension();
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType != typeof(Brush))
				return null;
			SingleTimelineCell cell = value as SingleTimelineCell;
			if (cell == null)
				return null;
			if (cell.IsWorkTime)
				return cell.Brushes.CellLightBorderDark;
			else
				return cell.Brushes.CellBorderDark;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class ColorConverter : IValueConverter {
		#region IValueConverter Members
		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			SystemMediaColor color = new SystemMediaColor();
			PlatformIndependentColor par = GetSystemDrawingColor(parameter);
			PlatformIndependentColor val = GetSystemDrawingColor(value);
			if (par != DXColor.Empty && val != DXColor.Empty) {
				return ConvertFromHSB(par, val);
			}
			return new SolidColorBrush(color);
		}
		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
		protected internal PlatformIndependentColor GetSystemDrawingColor(object color) {
			Type type = color.GetType();
			if (type == typeof(String))
				return MarkupLanguageColorParser.ParseColor((String)color);
			else if (type == typeof(System.Windows.Media.Color))
				return DevExpress.Xpf.Core.DXColorConverter.ToDrawingColor((System.Windows.Media.Color)color);
#if !SL
			else if (type == typeof(System.Drawing.Color))
				return (System.Drawing.Color)color;
#endif
			else if (type == typeof(SolidColorBrush))
				return DevExpress.Xpf.Core.DXColorConverter.ToDrawingColor(((SolidColorBrush)color).Color);
			return DXColor.Empty;
		}
		protected internal virtual SystemMediaColor ConvertFromHSB(PlatformIndependentColor baseColor, PlatformIndependentColor targetColor) {
			double b = Math.Min(1, DevExpress.Xpf.Core.DXColorConverter.GetBrightness(baseColor));
			double h = DevExpress.Xpf.Core.DXColorConverter.GetHue(targetColor);
			if (h > 0.0) {
				double s = DevExpress.Xpf.Core.DXColorConverter.GetSaturation(baseColor) * 0.45f;
				return DevExpress.Xpf.Core.DXColorConverter.FromHSB(h, s, b);
			}
			else {
				byte colorChannel = (byte)(b * 255);
				return DevExpress.Xpf.Core.ColorHelper.OverlayColor(Color.FromArgb(baseColor.A, colorChannel, colorChannel, colorChannel), DevExpress.Xpf.Core.DXColorConverter.ToMediaColor(targetColor));
			}
		}
		protected internal SystemMediaColor ConvertFromHSB(PlatformIndependentColor targetColor, float brightness) {
			byte b = (byte)(brightness * 255);
			SystemMediaColor color1 = SystemMediaColor.FromArgb(255, b, b, b);
			SystemMediaColor color2 = DevExpress.Xpf.Core.DXColorConverter.ToMediaColor(targetColor);
			return DevExpress.Xpf.Core.ColorHelper.OverlayColor(color1, color2);
		}
		SystemMediaColor ColorFromRgb(byte r, byte g, byte b) {
#if (SL)
			return PlatformIndependentColor.FromArgb(255, r, g, b);
#else
			return SystemMediaColor.FromRgb(r, g, b);
#endif
		}
	}
	public class AppointmentColorConverter : ColorConverter {
		protected internal override SystemMediaColor ConvertFromHSB(PlatformIndependentColor baseColor, PlatformIndependentColor targetColor) {
			double h = DevExpress.Xpf.Core.DXColorConverter.GetHue(targetColor);
			double s = DevExpress.Xpf.Core.DXColorConverter.GetSaturation(targetColor);
			double b = DevExpress.Xpf.Core.DXColorConverter.GetBrightness(baseColor);
			return DevExpress.Xpf.Core.DXColorConverter.FromHSB(h, s, b);
		}
	}
	public class OfficeColorConverter : ColorConverter {
		protected internal override SystemMediaColor ConvertFromHSB(PlatformIndependentColor baseColor, PlatformIndependentColor targetColor) {
			double h = DevExpress.Xpf.Core.DXColorConverter.GetHue(targetColor);
			double s = DevExpress.Xpf.Core.DXColorConverter.GetSaturation(targetColor);
			double b = DevExpress.Xpf.Core.DXColorConverter.GetBrightness(baseColor);
			return DevExpress.Xpf.Core.DXColorConverter.FromHSB(h, s, b);
		}
	}
	public class LabelColorToBorderBrushConverter : ColorConverter {
		public float BrightnessCoefficient { get; set; }
		#region IValueConverter Members
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			PlatformIndependentColor color = GetSystemDrawingColor(value);
			if (color != DXColor.Empty) {
				float coefficient = (BrightnessCoefficient <= 0 || BrightnessCoefficient > 1) ? 1 : BrightnessCoefficient;
				return new SolidColorBrush(ConvertFromHSB(color, color.GetBrightness() * coefficient));
			}
			return null;
		}
		#endregion
	}
	#region ItemsCountToVisibilityConverter
	public class ItemsCountToVisibilityConverter : IValueConverter {
		Visibility zeroEquivalent = Visibility.Collapsed;
		Visibility oneEquivalent = Visibility.Visible;
		Visibility moreThanOneEquivalent = Visibility.Visible;
		public Visibility ZeroEquivalent { get { return zeroEquivalent; } set { zeroEquivalent = value; } }
		public Visibility OneEquivalent { get { return oneEquivalent; } set { oneEquivalent = value; } }
		public Visibility MoreThanOneEquivalent { get { return moreThanOneEquivalent; } set { moreThanOneEquivalent = value; } }
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			IList list = value as IList;
			if (targetType != typeof(Visibility) || list == null)
				return null;
			switch (list.Count) {
				case 0:
					return ZeroEquivalent;
				case 1:
					return OneEquivalent;
				default:
					return MoreThanOneEquivalent;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
		#endregion
	}
	#endregion
	public class LastItemIndexConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			IList list = value as IList;
			if (targetType != typeof(int) || list == null)
				return -1;
			int count = list.Count;
			return count > 0 ? count - 1 : 0;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class ItemToIndexConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			ContentPresenter container = value as ContentPresenter;
			if (container != null) {
				ItemsControl control = ItemsControl.ItemsControlFromItemContainer(container);
				if (control != null)
					return control.ItemContainerGenerator.IndexFromContainer(container);
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class BoolToVisibilityMultiConverter : IMultiValueConverter {
		#region IMultiValueConverter Members
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (targetType != typeof(Visibility))
				return null;
			int count = values.Length;
			bool? result = null;
			for (int i = 0; i < count; i++) {
				bool? value = values[i] as bool?;
				result = result.HasValue ? result & (value ?? false) : value;
			}
			if (!result.HasValue)
				return null;
			return result.Value ? Visibility.Visible : Visibility.Collapsed;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class BoolToVisibilityMultiConverterExtension : MarkupExtension, IMultiValueConverter {
		static BoolToVisibilityMultiConverterExtension instance = null;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new BoolToVisibilityMultiConverterExtension();
			return instance;
		}
		#region IMultiValueConverter Members
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (targetType != typeof(Visibility))
				return null;
			int count = values.Length;
			bool? result = null;
			for (int i = 0; i < count; i++) {
				bool? value = values[i] as bool?;
				result = result.HasValue ? result & (value ?? false) : value;
			}
			if (!result.HasValue)
				return null;
			return result.Value ? Visibility.Visible : Visibility.Collapsed;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class BoolMultiValueConverter : IMultiValueConverter {
		#region IMultiValueConverter Members
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (targetType != typeof(bool) || values.Length == 0)
				return null;
			int count = values.Length;
			bool? result = null;
			for (int i = 0; i < count; i++) {
				bool? value = values[i] as bool?;
				if (value == null)
					return null;
				result = result.HasValue ? (result & value) : value;
			}
			return result;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class DoubleValueToHalfConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is Double) || targetType != typeof(double))
				return null;
			return (double)value / 2;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is Double) || targetType != typeof(double))
				return null;
			return (double)value * 2;
		}
		#endregion
	}
	public class InvertedBoolConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType != typeof(bool) || !(value is bool))
				return false;
			return !(bool)value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class InvertedBoolConverterExtension : MarkupExtension, IValueConverter {
		static InvertedBoolConverterExtension instance = null;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new InvertedBoolConverterExtension();
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return Invert(value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return Invert(value);
		}
		static object Invert(object value) {
			if (!(value is bool))
				return null;
			return !(bool)value;
		}
		#endregion
	}
	public class StringToEnclosedInBracketsStringConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string str = value as string;
			if (str == null || targetType != typeof(string))
				return null;
			return !String.IsNullOrEmpty(str) ? String.Format("({0})", str) : String.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			string str = value as string;
			if (str == null || targetType != typeof(string))
				return null;
			return str.Trim('(', ')');
		}
		#endregion
	}
	public class StringToEnclosedInBracketsStringConverterExtension : MarkupExtension, IValueConverter {
		static StringToEnclosedInBracketsStringConverterExtension converter = null;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (converter == null)
				converter = new StringToEnclosedInBracketsStringConverterExtension();
			return converter;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string str = value as string;
			if (str == null || targetType != typeof(string))
				return null;
			return !String.IsNullOrEmpty(str) ? String.Format("({0})", str) : String.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			string str = value as string;
			if (str == null || targetType != typeof(string))
				return null;
			return str.Trim('(', ')');
		}
		#endregion
	}
	public class AllDayTemplateConverter : SchedulerSealableObject, IValueConverter {
		#region AllDayArea
		public static readonly DependencyProperty AllDayAreaProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AllDayTemplateConverter, DataTemplate>("AllDayArea", null, (s, e) => s.OnAllDayAreaChanged(e.OldValue, e.NewValue));
		void OnAllDayAreaChanged(DataTemplate oldValue, DataTemplate newValue) {
			SealHelper.SealIfSealable(newValue);
		}
		public DataTemplate AllDayArea { get { return (DataTemplate)GetValue(AllDayAreaProperty); } set { SetValue(AllDayAreaProperty, value); } }
		#endregion
		#region AllDayAreaWithScroll
		public static readonly DependencyProperty AllDayAreaWithScrollProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AllDayTemplateConverter, DataTemplate>("AllDayAreaWithScroll", null);
		public DataTemplate AllDayAreaWithScroll { get { return (DataTemplate)GetValue(AllDayAreaWithScrollProperty); } set { SetValue(AllDayAreaWithScrollProperty, value); } }
		#endregion
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value is Boolean) {
				if ((bool)value == false)
					return AllDayArea;
				else
					return AllDayAreaWithScroll;
			}
			return AllDayArea;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new AllDayTemplateConverter();
		}
#endif
	}
	public class TimelineTemplateConverter : DependencyObject, IValueConverter {
		#region Timeline
		public static readonly DependencyProperty TimelineProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineTemplateConverter, ControlTemplate>("Timeline", null);
		public ControlTemplate Timeline { get { return (ControlTemplate)GetValue(TimelineProperty); } set { SetValue(TimelineProperty, value); } }
		#endregion
		#region TimelineWithScroll
		public static readonly DependencyProperty TimelineWithScrollProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineTemplateConverter, ControlTemplate>("TimelineWithScroll", null);
		public ControlTemplate TimelineWithScroll { get { return (ControlTemplate)GetValue(TimelineWithScrollProperty); } set { SetValue(TimelineWithScrollProperty, value); } }
		#endregion
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value is Boolean) {
				if ((bool)value == false)
					return Timeline;
				else
					return TimelineWithScroll;
			}
			return Timeline;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class DayViewGroupByDateHeadersTemplateConverter : DependencyObject, IValueConverter {
		#region DayHeaderVisibleTemplate
		public static readonly DependencyProperty DayHeaderVisibleTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayViewGroupByDateHeadersTemplateConverter, DataTemplate>("DayHeaderVisibleTemplate", null);
		public DataTemplate DayHeaderVisibleTemplate { get { return (DataTemplate)GetValue(DayHeaderVisibleTemplateProperty); } set { SetValue(DayHeaderVisibleTemplateProperty, value); } }
		#endregion
		#region DayHeaderInvisibleTemplate
		public static readonly DependencyProperty DayHeaderInvisibleTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayViewGroupByDateHeadersTemplateConverter, DataTemplate>("DayHeaderInvisibleTemplate", null);
		public DataTemplate DayHeaderInvisibleTemplate { get { return (DataTemplate)GetValue(DayHeaderInvisibleTemplateProperty); } set { SetValue(DayHeaderInvisibleTemplateProperty, value); } }
		#endregion
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is bool) || targetType != typeof(DataTemplate))
				return null;
			return (bool)value ? DayHeaderVisibleTemplate : DayHeaderInvisibleTemplate;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class ElementPositionVerticalWeekHeaderConverter :  IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			ElementPosition elementPosition = value as ElementPosition;
			if (elementPosition == null)
				return value;
			if (targetType != typeof(ElementPosition))
				return value;
			ElementPosition result = elementPosition.Clone();
			if (result.IsLeft && (result.OwnHorizontalSeparatorPosition & OuterSeparator.NoStart) == 0) {
				result.HorizontalElementPosition.OuterSeparator &= ~OuterSeparator.NoStart;
				result.HorizontalElementPosition.OuterSeparator |= OuterSeparator.Start;
			}
			if (result.IsRight && (result.OwnHorizontalSeparatorPosition & OuterSeparator.NoEnd) == 0) {
				result.HorizontalElementPosition.OuterSeparator &= ~OuterSeparator.NoEnd;
				result.HorizontalElementPosition.OuterSeparator |= OuterSeparator.End;
				if (result.VerticalElementPosition.ElementPosition != ElementRelativePosition.NotDefined)
					result.VerticalElementPosition.ElementPosition = ElementRelativePosition.Middle;
			}
			result.VerticalElementPosition.ElementPosition &= ~ElementRelativePosition.End;
			if (result.VerticalElementPosition.ElementPosition == ElementRelativePosition.NotDefined)
				result.VerticalElementPosition.ElementPosition = ElementRelativePosition.Middle;
			result.VerticalElementPosition.OuterSeparator &= ~OuterSeparator.NoEnd;
			result.VerticalElementPosition.OuterSeparator |= OuterSeparator.End;
			if (!result.IsTop && (result.VerticalWeekVerticalPosition & ElementRelativePosition.Start) == 0) {
				result.VerticalElementPosition.OuterSeparator &= ~OuterSeparator.NoStart;
				result.VerticalElementPosition.OuterSeparator |= OuterSeparator.Start;
			}
			return result;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class ElementPositionDayViewConverter : MarkupExtension, IValueConverter {
		ElementPositionDayViewConverter converter;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (converter == null)
				converter = new ElementPositionDayViewConverter();
			return converter;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			ElementPosition elementPosition = value as ElementPosition;
			if (elementPosition == null)
				return value;
			if (targetType != typeof(ElementPosition))
				return value;
			return elementPosition;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class HorizontalPositionToMarginConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			ElementPosition elementPosition = (value as ElementPosition) ?? ElementPosition.Standalone;
			if (targetType != typeof(Thickness))
				throw new ArgumentException();
			Thickness baseMargin = (Thickness)parameter;
			if ((elementPosition.OwnHorizontalMarginPosition & MarginPosition.Start) == 0)
				baseMargin.Left = 0;
			if ((elementPosition.OwnHorizontalMarginPosition & MarginPosition.End) == 0)
				baseMargin.Right = 0;
			if ((elementPosition.OwnVerticalMarginPosition & MarginPosition.Start) == 0)
				baseMargin.Top = 0;
			if ((elementPosition.OwnVerticalMarginPosition & MarginPosition.End) == 0)
				baseMargin.Bottom = 0;
			return baseMargin;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class ToolTipVisibilityToVisibilityConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			ToolTipVisibility toolTipVisibility = (ToolTipVisibility)value;
			return toolTipVisibility == ToolTipVisibility.Never ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
		#endregion
	}
	public class DecimalToConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Type target = parameter as Type;
			if (target == null)
				return value;
			return System.Convert.ChangeType(value, target, culture);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return System.Convert.ToDecimal(value);
		}
	}
	public class ResourceNavigatorVisibilityToConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is ResourceNavigatorVisibility))
				return Visibility.Visible;
			ResourceNavigatorVisibility resourceNavigatorVisibility = (ResourceNavigatorVisibility)value;
			if (resourceNavigatorVisibility == ResourceNavigatorVisibility.Never)
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class ResourceNavigatorVisibilityToConverterExtension : MarkupExtension {
		ResourceNavigatorVisibilityToConverter converter;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (converter == null)
				converter = new ResourceNavigatorVisibilityToConverter();
			return converter;
		}
	}
	public class ResourceNavigatorVisibilityToBoolConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is ResourceNavigatorVisibility))
				return false;
			ResourceNavigatorVisibility resourceNavigatorVisibility = (ResourceNavigatorVisibility)value;
			if (resourceNavigatorVisibility == ResourceNavigatorVisibility.Never)
				return true;
			return false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class ResourceNavigatorVisibilityToBoolConverterExtension : MarkupExtension {
		ResourceNavigatorVisibilityToBoolConverter converter;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (converter == null)
				converter = new ResourceNavigatorVisibilityToBoolConverter();
			return converter;
		}
	}
	public class ResourceNavigatorVisibilityToElementPositionConverter : DependencyObject, IValueConverter {
		#region VisibleElementPosition
		public ElementPosition VisibleElementPosition {
			get { return (ElementPosition)GetValue(VisibleElementPositionProperty); }
			set { SetValue(VisibleElementPositionProperty, value); }
		}
		public static readonly DependencyProperty VisibleElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorVisibilityToElementPositionConverter, ElementPosition>("VisibleElementPosition", null, (d, e) => d.OnVisibleElementPositionChanged(e.OldValue, e.NewValue), null);
		void OnVisibleElementPositionChanged(ElementPosition oldValue, ElementPosition newValue) {
		}
		#endregion
		#region OuterSeparatorWhenHidden
		public OuterSeparatorPosition OuterSeparatorWhenHidden {
			get { return (OuterSeparatorPosition)GetValue(OuterSeparatorWhenHiddenProperty); }
			set { SetValue(OuterSeparatorWhenHiddenProperty, value); }
		}
		public static readonly DependencyProperty OuterSeparatorWhenHiddenProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorVisibilityToElementPositionConverter, OuterSeparatorPosition>("OuterSeparatorWhenHidden", null, (d, e) => d.OnOuterSeparatorWhenHiddenChanged(e.OldValue, e.NewValue), null);
		void OnOuterSeparatorWhenHiddenChanged(OuterSeparatorPosition oldValue, OuterSeparatorPosition newValue) {
		}
		#endregion
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (!(value is ResourceNavigatorVisibility))
				return value;
			ResourceNavigatorVisibility visibility = (ResourceNavigatorVisibility)value;
			return (visibility == ResourceNavigatorVisibility.Never) ? new ElementPosition(VisibleElementPosition, OuterSeparatorWhenHidden) : VisibleElementPosition;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class BrushResourcesConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			GraphicResourceContainer container = value as GraphicResourceContainer;
			if (container == null)
				return null;
			string parameterString = parameter as string;
			if (String.IsNullOrEmpty(parameterString))
				return null;
			return container.BrushResources[parameterString];
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class ColorResourcesConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			GraphicResourceContainer container = value as GraphicResourceContainer;
			if (container == null)
				return null;
			string parameterString = parameter as string;
			if (String.IsNullOrEmpty(parameterString))
				return null;
			if (!container.ColorResources.ContainsKey(parameterString))
				return null;
			return container.ColorResources[parameterString];
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class AppointmentStatusDisplayTypeToGridLengthConverter : IValueConverter {
		public double DefaultValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is AppointmentStatusDisplayType) || targetType != typeof(GridLength))
				return value;
			AppointmentStatusDisplayType statusDisplayType = (AppointmentStatusDisplayType)value;
			if (statusDisplayType == AppointmentStatusDisplayType.Never)
				return new GridLength();
			return new GridLength(DefaultValue);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class AppointmentStatusDisplayTypeToDoubleConverter : IValueConverter {
		public double DefaultValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is AppointmentStatusDisplayType) || targetType != typeof(double))
				return value;
			AppointmentStatusDisplayType statusDisplayType = (AppointmentStatusDisplayType)value;
			if (statusDisplayType == AppointmentStatusDisplayType.Never)
				return 0;
			return DefaultValue;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class AppointmentStatusDisplayTypeToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is AppointmentStatusDisplayType))
				return value;
			AppointmentStatusDisplayType statusDisplayType = (AppointmentStatusDisplayType)value;
			if (statusDisplayType == AppointmentStatusDisplayType.Never)
				return Visibility.Collapsed;
			return Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class AppointmentStatusDisplayTypeToVisibileConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is AppointmentStatusDisplayType))
				return value;
			AppointmentStatusDisplayType statusDisplayType = (AppointmentStatusDisplayType)value;
			if (statusDisplayType == AppointmentStatusDisplayType.Never)
				return false;
			else
				return true;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class BoolToThicknessConverter : IValueConverter {
		public Thickness ThicknessWhenTrue { get; set; }
		public Thickness ThicknessWhenFalse { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is bool))
				return value;
			bool boolValue = (bool)value;
			return (boolValue) ? ThicknessWhenTrue : ThicknessWhenFalse;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class BoolToDataTemplateConverter : IValueConverter {
		public DataTemplate NormalTemplate { get; set; }
		public DataTemplate AlternateTemplate { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is bool))
				return value;
			bool boolValue = (bool)value;
			return (boolValue) ? NormalTemplate : AlternateTemplate;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DoubleLimitToInfinityConverter : MarkupExtension, IValueConverter {
		static DoubleLimitToInfinityConverter instance;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new DoubleLimitToInfinityConverter();
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(double)) {
				double val = (double)value;
				if (double.IsInfinity(val))
					return 0;
			}
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class IntervalEndConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is DateTime) {
				return ((DateTime)value).AddDays(-1);
			}
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is DateTime) {
				return ((DateTime)value).AddDays(1);
			}
			return value;
		}
		#endregion
	}
	public class RecurrenceTypeToStringConverter : MarkupExtension, IValueConverter {
		static RecurrenceTypeToStringConverter instance;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new RecurrenceTypeToStringConverter();
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return null;
			RecurrenceType recurrenceType = (RecurrenceType)value;
			switch (recurrenceType) {
				case XtraScheduler.RecurrenceType.Daily:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeDaily);
				case XtraScheduler.RecurrenceType.Hourly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeHourly);
				case XtraScheduler.RecurrenceType.Minutely:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeMinutely);
				case XtraScheduler.RecurrenceType.Monthly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeMonthly);
				case XtraScheduler.RecurrenceType.Weekly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeWeekly);
				case XtraScheduler.RecurrenceType.Yearly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeYearly);
			}
			return String.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
	}
}

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
using System.Collections.Generic;
using System.Windows.Controls;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Bars;
using System.Windows.Media;
namespace DevExpress.Charts.Designer.Native {
	public sealed class ScaleTypeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			SelectDataMemberCommandBase command = parameter as SelectDataMemberCommandBase;
			return command != null && value is WpfChartSeriesModel ? command.GetCommandScaleType() : ScaleType.Auto;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class ChartTreeVisibleToOpacityConverter : IValueConverter {
		const double visibleOpacity = 1.0;
		const double hiddenOpacity = 0.5;
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if ((targetType == typeof(double) && (value is bool)))
				return ((bool)value) ? visibleOpacity : hiddenOpacity;
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
	}
	public sealed class PageCategoryVisibleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value != null && parameter != null && parameter is Type) {
				Type valueType = value.GetType();
				return valueType == (Type)parameter || valueType.IsSubclassOf((Type)parameter);
			}
			return false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class PageSelectedConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string && parameter is string)
				return value == parameter;
			else
				return false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is bool)
				return (bool)value ? parameter : String.Empty;
			return String.Empty;
		}
	}
	public sealed class LegendPositionToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return false;
			ChangeLegendPositionCommand command = (ChangeLegendPositionCommand)parameter;
			HorizontalPosition horizPos = command.HorizontalPosition;
			VerticalPosition vertPos = command.VerticalPosition;
			Legend legend = command.ChartControl.Legend;
			return legend != null && legend.HorizontalPosition == horizPos && legend.VerticalPosition == vertPos;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class LegendOrientationToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return false;
			ChangeLegendOrientationCommand command = (ChangeLegendOrientationCommand)parameter;
			return command.ChartControl.Legend.Orientation == command.Orientation;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class LegendIsReverseConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartLegendModel model = value as WpfChartLegendModel;
			return model != null ? model.ReverseItems : false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class FontFamilyConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartFontModel model = value as WpfChartFontModel;
			return model != null ? model.FontFamily : null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class FontItalicToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartFontModel model = value as WpfChartFontModel;
			return model != null ? model.FontStyle == FontStyles.Italic : false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class FontBoldToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartFontModel model = value as WpfChartFontModel;
			return model != null ? model.FontWeight == FontWeights.Bold : false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class FontSizeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartFontModel model = value as WpfChartFontModel;
			return model != null ? model.FontSize : double.NaN;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class ChartTitlePositionToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartTitleModel model = value as WpfChartTitleModel;
			if (model == null)
				return false;
			if (parameter is EmptyChartCommand)
				return false;
			ChartTitlePositionCommand command = (ChartTitlePositionCommand)parameter;
			Title title = model.Title;
			if (title != null && title.Dock == command.Dock) {
				switch (command.Dock) {
					case Dock.Top:
					case Dock.Bottom:
						return title.HorizontalAlignment == command.HorizontalAlignment ||
							title.HorizontalAlignment == HorizontalAlignment.Stretch && command.HorizontalAlignment == HorizontalAlignment.Center;
					case Dock.Left:
					case Dock.Right:
						return true;
				}
			}
			return false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class ChartTitleContentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartTitleModel model = value as WpfChartTitleModel;
			if (model == null)
				return false;
			return model.Title.Content is string ? model.Title.Content.ToString() : ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ComplexTitleContent);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedItemGlyphConverter : IMultiValueConverter {
		object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (values.Length > 0) {
				if (values.Length > 1) {
					IList<GalleryItem> checkedItems = values[0] as IList<GalleryItem>;
					if ((checkedItems != null) && (checkedItems.Count > 0) && checkedItems[0].Glyph != null)
						return checkedItems[0].Glyph;
					if (values[1] != null)
						return values[1] as ImageSource;
				}
				else {
					IList<GalleryItem> galleryItems = values[0] as IList<GalleryItem>;
					if ((galleryItems != null) && (galleryItems.Count > 0))
						return galleryItems[0].Glyph;
				}
			}
			return null;
		}
		object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			return new object[] { value };
		}
	}
	public sealed class SeriesTypeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			ChangeSeriesTypeCommand command = parameter as ChangeSeriesTypeCommand;
			return seriesModel != null && command != null ? command.IsSeriesAtCommandState(seriesModel) : false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class DataMemberConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			SelectDataMemberCommandBase command = parameter as SelectDataMemberCommandBase;
			return command != null && value is WpfChartSeriesModel ? command.GetCommandDataMember() : string.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			DataMemberInfo info = value as DataMemberInfo;
			if (info != null)
				return info;
			else if (value != null)
				return new DataMemberInfo(value.ToString(), value.ToString());
			else
				return new DataMemberInfo();
		}
	}
	public sealed class DataSourceConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			return seriesModel != null ? seriesModel.DataSource : null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedDataSourceConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			SelectDataSourceCommandBase command = parameter as SelectDataSourceCommandBase;
			return seriesModel != null && command != null ? command.IsSeriesAtCommandState(seriesModel) : false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class ConstantLineLegendTextConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartConstantLineModel constantLineModel = value as WpfChartConstantLineModel;
			if (constantLineModel == null)
				return string.Empty;
			else
				return constantLineModel.LegendText;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class ConstantLineTitleTextConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartConstantLineModel constantLineModel = value as WpfChartConstantLineModel;
			if (constantLineModel == null)
				return string.Empty;
			else
				return constantLineModel.TitleText;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class ConstantLineThicknessConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartConstantLineModel constantLineModel = value as WpfChartConstantLineModel;
			if (constantLineModel == null)
				return 0;
			else
				return constantLineModel.Thickness;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			BarThicknessEditViewModel.Item item = (BarThicknessEditViewModel.Item)value;
			return item.Thickness;
		}
	}
	public sealed class ConstantLineColorConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartConstantLineModel constantLineModel = value as WpfChartConstantLineModel;
			if (constantLineModel == null)
				return null;
			else
				return constantLineModel.LineBrush;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class ConstantLineTitleForegroundConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartConstantLineModel constantLineModel = value as WpfChartConstantLineModel;
			if (constantLineModel == null)
				return null;
			else
				return constantLineModel.TitleForeground;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class ConstantLineValueConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartConstantLineModel constantLineModel = value as WpfChartConstantLineModel;
			if (constantLineModel == null)
				return null;
			else
				return constantLineModel.Value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SeriesLabelConectorThicknessConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return 1;
			else
				return seriesModel.LabelConnectorThickness;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			BarThicknessEditViewModel.Item item = (BarThicknessEditViewModel.Item)value;
			return item.Thickness;
		}
	}
	public sealed class StripMaxLimitConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartStripModel stripModel = value as WpfChartStripModel;
			if (stripModel == null)
				return null;
			else
				return stripModel.MaxLimit;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class StripMinLimitConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartStripModel stripModel = value as WpfChartStripModel;
			if (stripModel == null)
				return null;
			else
				return stripModel.MinLimit;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class StripBrushConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartStripModel stripModel = value as WpfChartStripModel;
			if (stripModel == null)
				return null;
			else
				return stripModel.Brush;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class StripLegendTextConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartStripModel stripModel = value as WpfChartStripModel;
			if (stripModel == null)
				return string.Empty;
			else
				return stripModel.LegendText;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class StripAxisLabelTextConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartStripModel stripModel = value as WpfChartStripModel;
			if (stripModel == null)
				return string.Empty;
			else
				return stripModel.AxisLabelText;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class AxisTitleDisplayNameConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartAxisModel axisModel = value as WpfChartAxisModel;
			if (axisModel == null || axisModel.Title == null)
				return null;
			else
				return axisModel.Title.DisplayName;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class AxisGridLinesVisibleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartAxisModel axisModel = value as WpfChartAxisModel;
			if (axisModel == null)
				return false;
			else
				return axisModel.GridLinesVisible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class AxisGridLinesMinorVisibleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartAxisModel axisModel = value as WpfChartAxisModel;
			if (axisModel == null)
				return false;
			else
				return axisModel.GridLinesMinorVisible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class AxisTickMarksVisibleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartAxisModel axisModel = value as WpfChartAxisModel;
			if (axisModel == null)
				return false;
			else
				return axisModel.TickmarksVisible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class ToggleAxisTickmarksMinorVisibleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartAxisModel axisModel = value as WpfChartAxisModel;
			if (axisModel == null)
				return false;
			else
				return axisModel.TickmarksMinorVisible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SeriesNameConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return null;
			else
				return seriesModel.Name;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SeriesLabelIndentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return 0;
			else
				return seriesModel.LabelIndent;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SeriesPaneConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return null;
			else
				return seriesModel.Pane;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class IndicatorPaneConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return null;
			else 
				return indicatorModel.Pane;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SeriesAxisXConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return null;
			else
				return seriesModel.AxisX;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SeriesAxisYConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return null;
			else
				return seriesModel.AxisY;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class IndicatorAxisYConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var indicatorModel = value as WpfChartIndicatorModel;
			if (indicatorModel == null)
				return null;
			else
				return indicatorModel.AxisY;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SeriesComplexLabelPositionConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return null;
			else
				return seriesModel.ComplexLabelPosition;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToPieHoleRadiusPercent : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return 0.0;
			else
				return seriesModel.HoleRadiusPercent;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToFunnelPointDistanceConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return 0;
			else
				return seriesModel.PointDistance;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToFunnelAlignToCenterConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return false;
			else
				return seriesModel.AlignToCenter;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToRatioAutoConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return false;
			else
				return seriesModel.HeightToWidthRatioAuto;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class SelectedModelToFunnelRatioConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return 0.0;
			else
				return seriesModel.HeightToWidthRatio;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class DiagramToHintVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return Visibility.Visible;
			else
				return Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class NestedDonutGroupConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return null;
			else
				return seriesModel.Group;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class NestedDonutInnerIndentIndentConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return NestedDonutSeries2D.DefaultInnerIndent;
			else
				return seriesModel.InnerIndent;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public sealed class NestedDonutWeightConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartSeriesModel seriesModel = value as WpfChartSeriesModel;
			if (seriesModel == null)
				return NestedDonutSeries2D.DefaultWeight;
			else
				return seriesModel.Weight;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
}

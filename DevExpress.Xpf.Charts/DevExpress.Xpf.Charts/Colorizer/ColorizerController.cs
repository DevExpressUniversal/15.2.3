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
using System.Windows.Media;
using System.Collections.Generic;
using DevExpress.Charts.Native;
using System.Windows;
namespace DevExpress.Xpf.Charts.Native {
	public class ColorizerController {
		Series series;
		ColorObjectColorizer defaultColorizer;
		ColorizerBrushCache colorizerBrushCache;
		Dictionary<ISeriesPoint, SolidColorBrush> aggregatedPointsCache;
		ColorizerLegendVisibleProvider legendVisible;
		Palette SeriesPalette { get { return series.Palette; } }
		ChartColorizerBase Colorizer { get { return series.Colorizer; } }
		ChartColorizerBase ActualColorizer { get { return Colorizer != null ? Colorizer : defaultColorizer; } }
		public ColorizerController(Series series) {
			this.series = series;
			this.defaultColorizer = new ColorObjectColorizer();
			this.colorizerBrushCache = new ColorizerBrushCache();
			this.aggregatedPointsCache = new Dictionary<ISeriesPoint, SolidColorBrush>();
			this.legendVisible = new ColorizerLegendVisibleProvider(series);
		}
		public void ClearBrushCache() {
			colorizerBrushCache.Clear();
			aggregatedPointsCache.Clear();
		}
		public SolidColorBrush GetSeriesPointBrush(object argument, object[] values, object colorKey) {
			if (colorKey != null) {
				Palette palette = GetPalette();
				Color? pointColor = ActualColorizer.GetPointColor(argument, values, colorKey, palette);
				if (pointColor.HasValue)
					return colorizerBrushCache.GetBrush(pointColor.Value);
			}
			return null;
		}
		Palette GetPalette() {
			if (ActualColorizer is ISupportPalette) {
				Palette palette = ((ISupportPalette)ActualColorizer).GetPalette();
				if (palette != null)
					return palette;
			}
			return SeriesPalette;
		}
		public SolidColorBrush GetBrushForAgregatedPoint(AggregatedSeriesPoint aggregatedPoint) {
			if (aggregatedPoint != null) {
				if (aggregatedPointsCache.ContainsKey(aggregatedPoint))
					return aggregatedPointsCache[aggregatedPoint];
				List<SeriesPoint> points = new List<SeriesPoint>();
				foreach (RefinedPoint refinedPoint in aggregatedPoint.SourcePoints) {
					SeriesPoint seriesPoint = refinedPoint.SeriesPoint as SeriesPoint;
					if (seriesPoint != null)
						points.Add(refinedPoint.SeriesPoint as SeriesPoint);
				}
				List<object> values = new List<object>();
				if (aggregatedPoint.Series.ValueScaleType == Scale.Numerical)
					foreach (double value in aggregatedPoint.Values)
						values.Add(value);
				else
					foreach (DateTime value in aggregatedPoint.DateTimeValues)
						values.Add(value);
				Palette palette = GetPalette();
				Color? pointColor = ActualColorizer.GetAggregatedPointColor(aggregatedPoint.Argument, values, points, palette);
				SolidColorBrush brush = (pointColor != null) ? colorizerBrushCache.GetBrush(pointColor.Value) : null;
				aggregatedPointsCache.Add(aggregatedPoint, brush);
			}
			return null;
		}
		public List<LegendItem> GetLegendItems() {
			List<LegendItem> legendItems = new List<LegendItem>();
			ILegendItemsProvider legendItemsProvider = ActualColorizer as ILegendItemsProvider;
			if (legendItemsProvider != null && legendItemsProvider.ShowInLegend) {
				List<ColorizerLegendItem> colorrizerLegendItems = legendItemsProvider.GetLegendItems(GetPalette());
				foreach (ColorizerLegendItem item in colorrizerLegendItems)
					legendItems.Add(new LegendItem(legendVisible, null, item.Text, colorizerBrushCache.GetBrush(item.Color)));
			}
			return legendItems;
		}
	}
	public class ColorizerLegendVisibleProvider : DependencyObject, ILegendVisible {
		Series series;
		public ColorizerLegendVisibleProvider(Series series) {
			this.series = series;
			CheckedInLegend = false;
			CheckableInLegend = false;
		}
		public DataTemplate LegendMarkerTemplate { get { return series != null ? series.LegendMarkerTemplate : null; } }
		public bool CheckedInLegend { get; set; }
		public bool CheckableInLegend { get; set; }
	}
	public class ColorizerBrushCache {
		readonly Dictionary<Color, SolidColorBrush> brushCache = new Dictionary<Color, SolidColorBrush>();
		public SolidColorBrush GetBrush(Color color) {
			SolidColorBrush brush = null;
			if (!brushCache.TryGetValue(color, out brush)) {
				brush = new SolidColorBrush(color);
				brushCache.Add(color, brush);
			}
			return brush;
		}
		public void Clear() {
			brushCache.Clear();
		}
	}
}

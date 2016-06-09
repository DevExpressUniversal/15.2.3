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
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	class ColorProvider {
		Dictionary<ISeriesPoint, Color> colors;
		IColorizer colorizer;
		Palette palette = null;
		public static void SetupDrawOptions(Color color, DrawOptions drawOptions) {
			if (color.IsEmpty)
				return;
			drawOptions.Color = HitTestColors.MixAlphaChanel(color, drawOptions.Color.A);
			drawOptions.ActualColor2 = HitTestColors.MixColors(Color.FromArgb(128, 50, 50, 50), color);
		}
		public Palette Palette {
			set { palette = value; }
		}
		public ColorProvider(IColorizer colorizer) {
			colors = new Dictionary<ISeriesPoint, Color>();
			this.colorizer = colorizer;
		}
		Palette GetPalette(IOwnedElement owner) {
			if (colorizer is IPaletteProvider) {
				Palette colorizerPalette = ((IPaletteProvider)colorizer).GetPalette();
				if (colorizerPalette != null)
					return colorizerPalette;
				else {
					string colorizerPaletteName = ((IPaletteProvider)colorizer).GetPaletteName();
					if (!string.IsNullOrEmpty(colorizerPaletteName))
						return GetPaletteFromChart(owner as IOwnedElement, colorizerPaletteName);
				}
			}
			return GetPaletteFromRoot(owner as IOwnedElement);
		}
		Palette GetPaletteFromRoot(IOwnedElement owner) {
			Chart chart = GetChart(owner);
			if (chart != null)
				return chart.Palette;
			return null;
		}
		Chart GetChart(IOwnedElement owner) {
			if (owner == null)
				return null;
			if (owner is Chart)
				return owner as Chart;
			else
				return GetChart(owner.Owner);
		}
		Palette GetPaletteFromChart(IOwnedElement owner, string colorizerPaletteName) {
			Chart chart = GetChart(owner);
			if (chart != null)
				return chart.PaletteRepository.GetPaletteByName(colorizerPaletteName);
			return null;
		}
		public Color GetColor(object argument, object[] values, object colorKey, IOwnedElement owner) {
			Palette palette = GetPalette(owner);
			object[] colorKeys = colorKey as object[];
			if (colorKeys != null)
				return colorizer.GetPointColor(argument, values, colorKeys, palette);
			return colorizer.GetPointColor(argument, values, colorKey, palette);
		}
		public Color GetColor(ISeriesPoint point) {
			if (point is SeriesPoint)
				return ((SeriesPoint)point).Color;
			else {
				if (colors.ContainsKey(point))
					return colors[point];
				else {
					if (colorizer is ColorObjectColorizer)
						return Color.Empty;
					AggregatedSeriesPoint aggregatedPoint = point as AggregatedSeriesPoint;
					List<SeriesPoint> points = new List<SeriesPoint>();
					foreach (RefinedPoint refinedPoint in aggregatedPoint.SourcePoints) {
						SeriesPoint seriesPoint = refinedPoint.SeriesPoint as SeriesPoint;
						if (seriesPoint != null)
							points.Add(seriesPoint);
					}
					List<object> values = new List<object>();
					if (aggregatedPoint.Series.ValueScaleType == Scale.Numerical)
						foreach (double value in aggregatedPoint.Values)
							values.Add(value);
					else
						foreach (DateTime value in aggregatedPoint.DateTimeValues)
							values.Add(value);
					Color color = colorizer.GetAggregatedPointColor(aggregatedPoint.Argument, values.ToArray(), points.ToArray(), palette);
					colors.Add(point, color);
					return color;
				}
			}
		}
		public List<LegendItem> GetLegendItemsForColorizer(IOwnedElement owner) {
			List<LegendItem> items = new List<LegendItem>();
			ILegendItemProvider provider = colorizer as ILegendItemProvider;
			if (provider != null) {
				Legend legend = GetChart(owner).Legend;
				Palette palette = GetPalette(owner as IOwnedElement);
				items = provider.GetLegendItems(palette, legend.TextVisible, legend.ActualTextColor, legend.Font, legend.MarkerVisible, legend.MarkerSize);
				foreach (LegendItem item in items)
					item.Init(legend);
			}
			return items;
		}
		public void Reset() { colors.Clear(); }
	}
	class BindColorizerToChartPalettesHelper {
		public static void BindColorizerToChartPalettes(ChartColorizerBase colorizer, IOwnedElement element) {
			IPaletteRepositoryProvider paletteRepositoryProvider = colorizer as IPaletteRepositoryProvider;
			if (paletteRepositoryProvider != null)
				paletteRepositoryProvider.SetPaletteRepository(GetPaletteRepository(element));
		}
		static PaletteRepository GetPaletteRepository(IOwnedElement element) {
			if (element == null)
				return null;
			if (element is Chart)
				return ((Chart)element).PaletteRepository;
			return GetPaletteRepository(element.Owner);
		}
	}
}

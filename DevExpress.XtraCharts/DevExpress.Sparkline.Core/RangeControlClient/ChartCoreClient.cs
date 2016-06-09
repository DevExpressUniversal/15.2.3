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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Sparkline;
using DevExpress.Sparkline.Core;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.ChartRangeControlClient.Core {
	public class GridUnit {
		public double Spacing { get; set; }
		public double Unit { get; set; }
		public double Step {
			get { return Spacing * Unit; }
		}
		public GridUnit(double unit, double spacing) {
			this.Spacing = spacing;
			this.Unit = unit;
		}
	}
	public sealed class SeriesProviderPair {
		readonly IClientSeries series;
		readonly SparklineDataProvider dataProvider;
		public IClientSeries Series {
			get { return series; }
		}
		public SparklineDataProvider DataProvider {
			get { return dataProvider; }
		}
		public SeriesProviderPair(IClientSeries series, SparklineDataProvider provider) {
			this.series = series;
			this.dataProvider = provider;
		}
	}
	public sealed class SeriesInteraction {
		readonly List<SeriesProviderPair> dataProviders;
		readonly SparklineInteractionRanges interactionRanges;
		public int Count {
			get { return dataProviders.Count; }
		}
		public bool HasDataToPresent {
			get { return Count > 0; }
		}
		public SparklineInteractionRanges Ranges {
			get { return interactionRanges; }
		}
		public SeriesInteraction() {
			this.dataProviders = new List<SeriesProviderPair>();
			this.interactionRanges = new SparklineInteractionRanges();
		}
		void UpdateRanges(SparklineDataProvider provider) {
			List<SparklinePoint> sortedPoints = provider.SortedPoints;
			int pointCount = sortedPoints.Count;
			if (pointCount == 1) {
				interactionRanges.DataArgumentRange.Extend(sortedPoints[0].Argument - 0.5);
				interactionRanges.DataArgumentRange.Extend(sortedPoints[0].Argument + 0.5);
				interactionRanges.DataValueRange.Extend(sortedPoints[0].Value - 0.5);
				interactionRanges.DataValueRange.Extend(sortedPoints[0].Value + 0.5);
				interactionRanges.UpdateMinPointDistance(1);
			}
			else {
				for (int pointIndex = 0; pointIndex < pointCount; pointIndex++) {
					SparklinePoint point = sortedPoints[pointIndex];
					interactionRanges.DataArgumentRange.Extend(point.Argument);
					interactionRanges.DataValueRange.Extend(point.Value);
					if ((pointCount > 1) && (pointIndex > 0))
						interactionRanges.UpdateMinPointDistance(point.Argument - sortedPoints[pointIndex - 1].Argument);
				}
			}
		}
		public void Clear() {
			dataProviders.Clear();
			interactionRanges.Invalidate();
		}
		public void Add(IClientSeries series) {
			SparklineDataProvider provider = new SparklineDataProvider(series, interactionRanges.VisualArgumentRange);
			provider.AllowPaddingCorrection = false;
			UpdateRanges(provider);
			dataProviders.Add(new SeriesProviderPair(series, provider));
		}
		public SeriesProviderPair this[int index] {
			get {
				return dataProviders[index];
			}
		}
		public double NormalizeArgument(double argument) {
			SparklineRangeData argumentRange = interactionRanges.ActualDataArgumentRange;
			double relativeArgument = argument - argumentRange.Min;
			double argumentDelta = argumentRange.Delta;
			if (SparklineMathUtils.IsValidDouble(argumentDelta) && SparklineMathUtils.IsValidDouble(relativeArgument))
				return relativeArgument / argumentDelta;
			return 0.0;
		}
		public double GetArgument(double normalizedArgument) {
			if (SparklineMathUtils.IsValidDouble(normalizedArgument)) {
				SparklineRangeData argumentRange = interactionRanges.ActualDataArgumentRange;
				double argumentDelta = argumentRange.Delta;
				if (SparklineMathUtils.IsValidDouble(argumentDelta) && SparklineMathUtils.IsValidDouble(argumentRange.Min))
					return normalizedArgument * argumentDelta + argumentRange.Min;
			}
			return 0.0;
		}
	}
	public enum SnapBounds {
		None = 0,
		Minimum = 1,
		Maximum = 2,
		Both = 3
	}
	public sealed class ChartCoreClient {
		readonly SeriesInteraction interaction; 
		readonly SparklinePaintersCache sparklinePaintersCache;
		IClientDataProvider dataProvider;
		IChartCoreClientDelegate clientDelegate;
		public IClientDataProvider DataProvider {
			get { return dataProvider; }
			set {
				if (dataProvider != value) {
					if (dataProvider != null)
						dataProvider.SetDataChangedDelegate(null);
					dataProvider = value;
					if (dataProvider != null)
						dataProvider.SetDataChangedDelegate(DataProviderDataChanged);
					DataProviderDataChanged(dataProvider);
				}
			}
		}
		public bool HasDataToPresent {
			get { return interaction.HasDataToPresent; }
		}
		public SeriesInteraction Interaction {
			get { return interaction; }
		}
		public IChartCoreClientDelegate Delegate {
			get { return clientDelegate; }
			set {
				if (clientDelegate != value) {
					clientDelegate = value;
					UpdateInteraction();
				}
			}
		}
		public ChartCoreClient() {
			this.interaction = new SeriesInteraction();
			this.sparklinePaintersCache = new SparklinePaintersCache();
		}
		void DataProviderDataChanged(IClientDataProvider provider) {
			UpdateInteraction();
		}
		void UpdateInteraction() {
			interaction.Clear();
			if (dataProvider != null) {
				IList<IClientSeries> seriesList = dataProvider.Series;
				for (int seriesIndex = 0; seriesIndex < seriesList.Count; seriesIndex++)
					interaction.Add(seriesList[seriesIndex]);
			}
			if (clientDelegate != null)
				clientDelegate.InteractionUpdated();
		}
		GridUnit GetAutoGridUnit(double min, double max, double width, IChartCoreClientGridOptions gridOptions) {
			double delta = max - min;
			double wholeRange = interaction.Ranges.ActualDataArgumentRange.Delta;
			double pixelsPerUnit = gridOptions.PixelPerUnit;
			if (SparklineMathUtils.IsValidDouble(delta) && SparklineMathUtils.IsValidDouble(wholeRange) && (wholeRange > 0) && (delta > 0)) {
				double wholeLength = (width * wholeRange) / delta;
				double unitsCount = Math.Max(1, Math.Floor(wholeLength / pixelsPerUnit));
				return gridOptions.GridMapping.SelectGridUnit(wholeRange / unitsCount);
			}
			return null;
		}
		double RoundNormalValue(double normalizedValue, GridUnit unit, IChartCoreClientGridMapping gridMapping) {
			if (SparklineMathUtils.AreDoublesEqual(normalizedValue, 0.0) || SparklineMathUtils.AreDoublesEqual(normalizedValue, 1.0))
				return normalizedValue;
			double value = GetValue(normalizedValue);
			double floor = gridMapping.GetGridValue(unit, gridMapping.FloorValue(unit, value));
			double roundedValue = 0.0;
			if (((value - floor) / unit.Step) > 0.5)
				roundedValue = gridMapping.GetGridValue(unit, gridMapping.CeilValue(unit, value));
			else
				roundedValue = floor;
			return Normalize(roundedValue);
		}
		double FloorNormalValue(double normalizedValue, GridUnit unit, IChartCoreClientGridMapping gridMapping) {
			if (SparklineMathUtils.AreDoublesEqual(normalizedValue, 0.0))
				return 0.0;
			double value = GetValue(normalizedValue);
			double floorIndex = gridMapping.FloorValue(unit, value);
			return Normalize(gridMapping.GetGridValue(unit, floorIndex));
		}
		double CeilNormalValue(double normalizedValue, GridUnit unit, IChartCoreClientGridMapping gridMapping) {
			if (SparklineMathUtils.AreDoublesEqual(normalizedValue, 1.0))
				return 1.0;
			double value = GetValue(normalizedValue);
			double ceilIndex = gridMapping.CeilValue(unit, value);
			return Normalize(gridMapping.GetGridValue(unit, ceilIndex));
		}
		double ShiftNormalValue(double normalizedValue, GridUnit unit, IChartCoreClientGridMapping gridMapping, double offset) {
			double value = GetValue(normalizedValue);
			double floorIndex = gridMapping.FloorValue(unit, value) + offset;
			if (floorIndex > 0)
				return Normalize(gridMapping.GetGridValue(unit, floorIndex));
			return 0;
		}
		public void DrawContent(Graphics graphics, Rectangle bounds, Matrix normalTransform) {
			if (interaction.HasDataToPresent) {
				for (int dataIndex = 0; dataIndex < interaction.Count; dataIndex++) {
					SeriesProviderPair data = interaction[dataIndex];
					if(data.Series.View != null) {
						BaseSparklinePainter painter = sparklinePaintersCache.GetPainter(data.Series.View);
						painter.Initialize(data.DataProvider, data.Series, bounds, interaction.Ranges, normalTransform);
						painter.Draw(graphics, null);
					}
				}
			}
		}
		public double Normalize(double value) {
			double normalValue = interaction.NormalizeArgument(value);
			if (normalValue < 0.0)
				normalValue = 0.0;
			else if (normalValue > 1.0)
				normalValue = 1.0;
			return normalValue;
		}
		public double GetValue(double normalizedValue) {
			return interaction.GetArgument(normalizedValue);
		}
		public List<object> GenerateGrid(double min, double max, double width, IChartCoreClientGridOptions gridOptions) {
			List<object> grid = new List<object>();
			GridUnit unit = null;
			if (gridOptions.Auto)
				unit = GetAutoGridUnit(min, max, width, gridOptions);
			else
				unit = gridOptions.GridUnit;
			if (unit != null) {
				IChartCoreClientGridMapping gridMapping = gridOptions.GridMapping;
				if (((max - min) / unit.Step) > width)
					return grid;
				if (unit != null) {
					double gridRangeMin = gridMapping.CeilValue(unit, min);
					double gridRangeMax = gridMapping.FloorValue(unit, max);
					double gridCurrent = gridRangeMin;
					do {
						grid.Add(gridOptions.GridMapping.GetGridValue(unit, gridCurrent++));
					} while (gridCurrent <= gridRangeMax);
				}
			}
			return grid;
		}
		public void SnapNormalRange(SparklineRangeData range, SnapBounds changedBounds, IChartCoreClientGridOptions gridOptions) {
			GridUnit unit = gridOptions.SnapUnit;
			IChartCoreClientGridMapping gridMapping = gridOptions.GridMapping;
			double edgeMin = RoundNormalValue(range.Min, unit, gridMapping);
			double edgeMax = RoundNormalValue(range.Max, unit, gridMapping);
			if (SparklineMathUtils.AreDoublesEqual(edgeMin, edgeMax)) {
				double edge = edgeMin;
				if (SparklineMathUtils.AreDoublesEqual(range.Min, range.Max)) {
					switch (changedBounds) {
						case SnapBounds.None:
						case SnapBounds.Both:
							if ((edge - range.Min) > 0)
								edgeMin = ShiftNormalValue(edge, unit, gridMapping, -1);
							else
								edgeMax = ShiftNormalValue(edge, unit, gridMapping, 1);
							break;
						case SnapBounds.Minimum:
							edgeMin = ShiftNormalValue(edge, unit, gridMapping, -1);
							break;
						case SnapBounds.Maximum:
							edgeMax = ShiftNormalValue(edge, unit, gridMapping, 1);
							break;
					}
				} else {
					double diffMin = Math.Abs(edge - range.Min);
					double diffMax = Math.Abs(edge - range.Max);
					if (diffMin > diffMax)
						edgeMin = FloorNormalValue(range.Min, unit, gridMapping);
					else
						edgeMax = CeilNormalValue(range.Max, unit, gridMapping);
				}
			}
			range.Set(edgeMin, edgeMax);
		}
		public void SetCustomArgumentRange(SparklineRangeData range) {
			Interaction.Ranges.CustomDataArgumentRange.Set(range.Min, range.Max);
		}
	}
}

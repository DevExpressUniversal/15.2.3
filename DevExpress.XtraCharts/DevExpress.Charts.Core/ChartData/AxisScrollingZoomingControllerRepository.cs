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
namespace DevExpress.Charts.Native {
	public class AxisPaneContainer : Dictionary<IAxisData, IList<IPane>> {
		public AxisPaneContainer(PaneAxesContainerRepository paneAxisRepository) {
			foreach (PaneAxesContainer paneAxesData in paneAxisRepository.Values) {
				RegisterAxis(paneAxesData.PrimaryAxisX, paneAxesData.Pane);
				RegisterAxis(paneAxesData.PrimaryAxisY, paneAxesData.Pane);
				foreach (IAxisData axisX in paneAxesData.SecondaryAxesX)
					RegisterAxis(axisX, paneAxesData.Pane);
				foreach (IAxisData axisY in paneAxesData.SecondaryAxesY)
					RegisterAxis(axisY, paneAxesData.Pane);
			}
			foreach (KeyValuePair<IAxisData, IList<IPane>> pair in this) {
				AxisVisibilityInPanes visibilityInPanes = pair.Key.AxisVisibilityInPanes;
				if (visibilityInPanes != null)
					visibilityInPanes.UpdateVisibilityInPanes(pair.Value);
			}
		}
		public IList<IPane> GetPanes(IAxisData axis) {
			IList<IPane> data;
			return TryGetValue(axis, out data) ? data : null;
		}
		public IPane GetFirstPaneByAxes(IAxisData axisX, IAxisData axisY) {
			IList<IPane> axisXPanes = GetPanes(axisX);
			IList<IPane> axisYPanes = GetPanes(axisY);
			foreach (IPane axisXPane in axisXPanes)
				foreach (IPane axisYPane in axisYPanes)
					if (axisXPane == axisYPane)
						return axisXPane;
			return null;
		}
		public bool IsScrollingZoomingEnabledForAxis(IAxisData axis) {
			bool isValueAxis = !axis.IsArgumentAxis;
			IList<IPane> panes;
			if (TryGetValue(axis, out panes))
				foreach (IPane pane in panes) {
					IZoomablePane zoomablePane = pane as IZoomablePane;
					if (zoomablePane != null) {
						if (isValueAxis) {
							if (zoomablePane.ScrollingByYEnabled || zoomablePane.ZoomingByYEnabled)
								return true;
						}
						else
							if (zoomablePane.ScrollingByXEnabled || zoomablePane.ZoomingByXEnabled)
								return true;
					}
				}
			return false;
		}
		void RegisterAxis(IAxisData axis, IPane pane) {
			IList<IPane> data;
			if (!TryGetValue(axis, out data)) {
				data = new List<IPane>();
				Add(axis, data);
			}
			data.Add(pane);
		}
	}
	public class PaneAxesContainerRepository {
		class PaneComparer : IComparer<IPane> {
			public int Compare(IPane x, IPane y) {
				return x.PaneIndex.CompareTo(y.PaneIndex);
			}
		}
		class AxisUsing {
			readonly int index;
			bool used;
			public int Index { get { return index; } }
			public bool Used {
				get { return used; }
				set { used = value; }
			}
			public AxisUsing(int index) {
				this.index = index;
			}
		}
		class AxesUsingList : Dictionary<IAxisData, AxisUsing> {
			public AxesUsingList(IAxisData primaryAxis, IEnumerable<IAxisData> secondaryAxes) {
				Add(primaryAxis, new AxisUsing(-1));
				if (secondaryAxes != null) {
					int index = 0;
					foreach (IAxisData secondaryAxis in secondaryAxes)
						Add(secondaryAxis, new AxisUsing(index++));
				}
			}
		}
		SortedList<IPane, PaneAxesContainer> repository;
		public IList<PaneAxesContainer> Values { get { return repository.Values; } }
		public PaneAxesContainerRepository(IAxisData primaryAxisX, IAxisData primaryAxisY, IEnumerable<IAxisData> secondaryAxesX, IEnumerable<IAxisData> secondaryAxesY, IList<IPane> actualPanes, IEnumerable<IRefinedSeries> seriesList) {
			this.repository = new SortedList<IPane, PaneAxesContainer>(new PaneComparer());
			AxesUsingList xAxesUsing = new AxesUsingList(primaryAxisX, secondaryAxesX);
			AxesUsingList yAxesUsing = new AxesUsingList(primaryAxisY, secondaryAxesY);
			if (new List<IRefinedSeries>(seriesList).Count == 0) {
				if (actualPanes.Count > 0) {
					IPane pane = actualPanes[0];
					repository.Add(pane, new PaneAxesContainer(pane));
				}
			}
			else {
				foreach (IRefinedSeries series in seriesList) {
					IXYSeriesView view = series.SeriesView as IXYSeriesView;
					if (view != null) {
						RegisterPane(xAxesUsing, yAxesUsing, view.Pane, view.AxisXData, view.AxisYData);
						List<ISeparatePaneIndicator> indicators = view.GetSeparatePaneIndicators();
						if (indicators != null) {
							foreach (ISeparatePaneIndicator indicator in indicators) {
								if (indicator.Pane != view.Pane) {
									IAxisData axisYData = yAxesUsing.ContainsKey(indicator.AxisYData) ? indicator.AxisYData : primaryAxisY;
									RegisterPane(xAxesUsing, yAxesUsing, indicator.Pane, view.AxisXData, axisYData);
								}
							}
						}
					}
				}
			}
			foreach (IPane pane in actualPanes) {
				PaneAxesContainer data;
				if (repository.TryGetValue(pane, out data)) {
					foreach (KeyValuePair<IAxisData, AxisUsing> item in xAxesUsing) {
						AxisUsing axisUsing = item.Value;
						if (!axisUsing.Used)
							data.RegisterAxisX(axisUsing.Index, item.Key);
					}
					foreach (KeyValuePair<IAxisData, AxisUsing> item in yAxesUsing) {
						AxisUsing axisUsing = item.Value;
						if (!axisUsing.Used)
							data.RegisterAxisY(axisUsing.Index, item.Key);
					}
					break;
				}
			}
			foreach (PaneAxesContainer paneAxisData in Values)
				paneAxisData.InitializePrimaryAndSecondaryAxes();
			foreach (PaneAxesContainer paneAxisData in Values)
				paneAxisData.InitializeTotalSecondaryAxes(this);
		}
		void RegisterPane(AxesUsingList xAxesUsing, AxesUsingList yAxesUsing, IPane pane, IAxisData axisX, IAxisData axisY) {
			if (pane == null)
				return;
			PaneAxesContainer data;
			if (!repository.TryGetValue(pane, out data)) {
				data = new PaneAxesContainer(pane);
				repository.Add(pane, data);
			}
			AxisUsing axisXUsing = xAxesUsing[axisX];
			data.RegisterAxisX(axisXUsing.Index, axisX);
			axisXUsing.Used = true;
			AxisUsing axisYUsing = yAxesUsing[axisY];
			data.RegisterAxisY(axisYUsing.Index, axisY);
			axisYUsing.Used = true;
		}
		public PaneAxesContainer GetContaiter(IPane pane) {
			PaneAxesContainer container;
			return repository.TryGetValue(pane, out container) ? container : null;
		}
		public void UpdateRanges() {
			foreach (PaneAxesContainer paneAxisData in Values)
				paneAxisData.RangeLimitsUpdated();
		}
	}
	public class AxisRangeInfo {
		public double Min { get; private set; }
		public double Max { get; private set; }
		public object MinValue { get; private set; }
		public object MaxValue { get; private set; }
		public AxisRangeInfo(IAxisRangeData axisRange) {
			Min = axisRange.Min;
			Max = axisRange.Max;
			MinValue = axisRange.MinValue;
			MaxValue = axisRange.MaxValue;
		}
		public void UpdateRange(IVisualAxisRangeData axisRange) {
			axisRange.ScrollOrZoomRange(MinValue, MaxValue, Min, Max);
		}
		public override bool Equals(object obj) {
			AxisRangeInfo info = obj as AxisRangeInfo;
			if (info == null)
				return false;
			return info.Max == Max && info.Min == Min;
		}
		public override int GetHashCode() {
			return Min.GetHashCode() ^ Max.GetHashCode();
		}
	}
	public class RangesSnapshot {
		static void CorrectSecondaryAxesPositions(IList<IAxisData> secondaryAxes, IAxisRangeData visualRange, IAxisRangeData wholeRange, bool reversed) {
			double minFactor = (visualRange.Min - wholeRange.Min) / wholeRange.Delta;
			double maxFactor = (visualRange.Max - wholeRange.Min) / wholeRange.Delta;
				double actualMinFactor;
				double actualMaxFactor;
				foreach (IAxisData axis in secondaryAxes) {
					if (axis.Reverse ^ reversed) {
						actualMinFactor = 1 - maxFactor;
						actualMaxFactor = 1 - minFactor;
					}
					else {
						actualMinFactor = minFactor;
						actualMaxFactor = maxFactor;
					}
					ITransformation transformation = axis.AxisScaleTypeMap.Transformation;
					double min = transformation.TransformForward(axis.WholeRange.Min);
					double max = transformation.TransformForward(axis.WholeRange.Max);
					double delta = max - min;
					double newMin = transformation.TransformBackward(min + actualMinFactor * delta);
					double newMax = transformation.TransformBackward(min + actualMaxFactor * delta);
					PaneAxesContainer.SetMinMax(axis, newMin, newMax);
			}
		}
		readonly IPane pane;
		readonly IAxisData axisX;
		readonly IAxisData axisY;
		readonly AxisRangeInfo xAxisRange;
		readonly AxisRangeInfo yAxisRange;
		public AxisRangeInfo XRange { get { return xAxisRange; } }
		public AxisRangeInfo YRange { get { return yAxisRange; } }
		public IAxisData AxisX { get { return axisX; } }
		public IAxisData AxisY { get { return axisY; } }
		public override bool Equals(object obj) {
			RangesSnapshot snapshot = obj as RangesSnapshot;
			if (snapshot == null)
				return false;
			return xAxisRange.Equals(snapshot.xAxisRange) && yAxisRange.Equals(snapshot.yAxisRange);
		}
		public override int GetHashCode() {
			return xAxisRange.GetHashCode() ^ yAxisRange.GetHashCode();
		}
		public RangesSnapshot(IPane pane, IAxisData axisX, IAxisData axisY) {
			this.pane = pane;
			this.axisX = axisX;
			this.axisY = axisY;
			this.xAxisRange = new AxisRangeInfo(axisX.VisualRange);
			this.yAxisRange = new AxisRangeInfo(axisY.VisualRange);
		}
		internal void CorrectSecondaryAxesPositions(List<IAxisData> secondaryAxesX, bool correctAxesX, List<IAxisData> secondaryAxesY, bool correctAxesY) {
			if (correctAxesX)
				CorrectSecondaryAxesPositions(secondaryAxesX, axisX.VisualRange, axisX.WholeRange, axisX.Reverse);
			if (correctAxesY)
				CorrectSecondaryAxesPositions(secondaryAxesY, axisY.VisualRange, axisY.WholeRange, axisY.Reverse);
		}
	}
	public class OutOfBoundsCheckerEx {
		#region inner classes
		class Segment {
			public static Segment Horizontal(GRealRect2D bounds) {
				return new Segment(bounds.Left, bounds.Right);
			}
			public static Segment Vertical(GRealRect2D bounds) {
				return new Segment(bounds.Top, bounds.Bottom);
			}
			double start;
			double end;
			double length;
			public double Length { get { return length; } }
			public Segment(double start, double end) {
				this.start = start;
				this.end = end;
				this.length = end - start;
			}
			public static Segment operator |(Segment x, Segment y) {
				double start = x.start < y.start ? x.start : y.start;
				double end = x.end > y.end ? x.end : y.end;
				return new Segment(start, end);
			}
			public static MinMaxValues operator -(Segment x, Segment y) {
				return new MinMaxValues(y.start - x.start, x.end - y.end);
			}
			public static bool operator ==(Segment x, Segment y) {
				return x.Equals(y);
			}
			public static bool operator !=(Segment x, Segment y) {
				return !(x == y);
			}
			public override bool Equals(object obj) {
				if (obj == null)
					return false;
				Segment second = obj as Segment;
				if (second == null)
					return false;
				return this.start == second.start && this.end == second.end;
			}
			public override int GetHashCode() {
				return start.GetHashCode() ^ end.GetHashCode();
			}
			public Segment Clone() {
				return new Segment(this.start, this.end);
			}
		}
		#endregion
		IAxisData axis;
		bool isHorizontal;
		Segment bound;
		Segment expandedBound;
		IMinMaxValues axisInterval;
		public IAxisData Axis { get { return axis; } }
		public bool NeedCorrection { get { return expandedBound != bound; } }
		public OutOfBoundsCheckerEx(IAxisData axis, bool isHorizontal, GRect2D visualRangeBounds, IMinMaxValues axisInterval) {
			this.axisInterval = axisInterval;
			this.axis = axis;
			this.isHorizontal = isHorizontal;
			if (isHorizontal)
				bound = GetWholeRangeBound(axis, visualRangeBounds.Left, visualRangeBounds.Right);
			else
				bound = GetWholeRangeBound(axis, visualRangeBounds.Top, visualRangeBounds.Bottom);
			expandedBound = bound.Clone();
		}
		Segment GetWholeRangeBound(IAxisData axis, int minBound, int maxBound) {
			if (axis.WholeRange.Min == axis.VisualRange.Min && axis.WholeRange.Max == axis.VisualRange.Max)
				return new Segment(minBound, maxBound);
			else {
				double pixelPerAxisUnit = (maxBound - minBound) / axis.VisualRange.Delta;
				double minMarginInPixels = MinMarginInPixels(axis, pixelPerAxisUnit);
				double maxMarginInPixels = MaxMarginInPixels(axis, pixelPerAxisUnit);
				if (axis.IsVertical ^ axis.Reverse)
					return new Segment(minBound - maxMarginInPixels, maxBound + minMarginInPixels);
				else
					return new Segment(minBound - minMarginInPixels, maxBound + maxMarginInPixels);
			}
		}
		double MinMarginInPixels(IAxisData axis, double pixelPerAxisUnit) {
			return Math.Ceiling((axis.VisualRange.Min - axis.WholeRange.Min) * pixelPerAxisUnit);
		}
		double MaxMarginInPixels(IAxisData axis, double pixelPerAxisUnit) {
			return Math.Ceiling((axis.WholeRange.Max - axis.VisualRange.Max) * pixelPerAxisUnit);
		}
		MinMaxValues CalculateRange(IMinMaxValues range, bool reverse, Segment expandedBound, Segment bound) {
			double axisUnitPerPixel = range.Delta / (bound.Length - (expandedBound.Length - bound.Length));
			if (double.IsPositiveInfinity(axisUnitPerPixel) || axisUnitPerPixel < 0)
				axisUnitPerPixel = 1;
			MinMaxValues correctionInPixel = expandedBound - bound;
			MinMaxValues correctionInAxisUnit;
			if (reverse)
				correctionInAxisUnit = new MinMaxValues(correctionInPixel.Min * axisUnitPerPixel, correctionInPixel.Max * axisUnitPerPixel);
			else
				correctionInAxisUnit = new MinMaxValues(correctionInPixel.Max * axisUnitPerPixel, correctionInPixel.Min * axisUnitPerPixel);
			MinMaxValues newRange = new MinMaxValues(range.Min - correctionInAxisUnit.Min, range.Max + correctionInAxisUnit.Max);
			ChartDebug.Assert(newRange.HasValues && newRange.Delta > 0, "range delta must be > 0");
			return newRange;
		}
		public void CheckOutOfBounds(GRealRect2D bounds) {
			expandedBound = expandedBound | (isHorizontal ? Segment.Horizontal(bounds) : Segment.Vertical(bounds));
		}
		public MinMaxValues GetCorrectedWholeRange() {
			if (!NeedCorrection)
				return MinMaxValues.Empty;
			if (isHorizontal)
				return CalculateRange(axisInterval, !axis.Reverse, expandedBound, bound);
			else
				return CalculateRange(axisInterval, axis.Reverse, expandedBound, bound);
		}
	}   
	public interface IBoundsProvider {
		bool Enable { get; }
		GRealRect2D GetBounds();
	}
}

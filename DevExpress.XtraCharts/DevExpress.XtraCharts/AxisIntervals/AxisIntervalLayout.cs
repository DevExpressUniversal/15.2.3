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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class AxisIntervalLayout {
		static AxisIntervalLayout CreateIntervalLayout(AxisInterval interval, double position, double nextPosition) {
			int positionInt = MathUtils.StrongRound(position);
			int nextPositionInt = MathUtils.StrongRound(nextPosition);
			return new AxisIntervalLayout(interval, positionInt, Math.Max(nextPositionInt - positionInt, 0));
		}
		public static List<AxisIntervalLayout> CreateIntervalsLayout(IList<AxisInterval> intervals, int totalLength, int separator) {
			List<AxisIntervalLayout> intervalsLayout = new List<AxisIntervalLayout>();
			if (intervals.Count == 0) {
				ChartDebug.Fail("At least one interval expected.");
				return intervalsLayout;
			}
			if (separator < 0) {
				ChartDebug.Fail("The separator should be greater than or equal to zero.");
				separator = 0;
			}
			int actualLength = totalLength - separator * (intervals.Count - 1);
			if (actualLength < 0) {
				foreach (AxisInterval interval in intervals)
					intervalsLayout.Add(new AxisIntervalLayout(interval, 0, 0));
				return intervalsLayout;
			}
			double totalDelta = 0;
			foreach (AxisInterval interval in intervals)
				totalDelta += interval.Delta;
			double position = 0;
			for (int i = 0; i < intervals.Count - 1; i++) {
				double nextPosition = position + (double)actualLength * intervals[i].Delta / totalDelta;
				intervalsLayout.Add(CreateIntervalLayout(intervals[i], position, nextPosition));
				position = nextPosition + separator;
			}
			intervalsLayout.Add(CreateIntervalLayout(intervals[intervals.Count - 1], position, (double)totalLength));
			return intervalsLayout;
		}
		public static List<AxisIntervalLayout> CreateIntervalsLayout(IIntervalContainer intervalContainer, IAxisData axis, int totalLength) {
			return CreateIntervalsLayout(intervalContainer.Intervals, totalLength, intervalContainer.IntervalsDistance);
		}
		readonly AxisInterval axisInterval;
		readonly Interval interval;
		public AxisInterval Interval { get { return axisInterval; } }
		public Interval Bounds { get { return interval; } }
		public IMinMaxValues Range { get { return axisInterval.Range; } }
		public IMinMaxValues WholeRange { get { return axisInterval.WholeRange; } }
		public int Start { get { return interval.Start; } }
		public int End { get { return interval.End; } }
		public AxisIntervalLayout(AxisInterval axisInterval, int position, int length) {
			this.axisInterval = axisInterval;
			this.interval = new Interval(position, position + length);
		}
		public bool ValueWithinRange(double value) {
			return value >= axisInterval.Range.Min && value <= axisInterval.Range.Max;
		}
	}
	public interface IIntervalContainer {
		IList<AxisInterval> Intervals { get; }
		int IntervalsDistance { get; }
	}
	public class AxisIntervalLayoutMapping {
		readonly AxisIntervalLayout axisIntervalLayout;
		readonly ITransformation transformation;
		public ITransformation Transformation { get { return transformation; } }
		public AxisIntervalLayoutMapping(AxisIntervalLayout axisIntervalLayout, ITransformation transformation) {
			this.axisIntervalLayout = axisIntervalLayout;
			this.transformation = transformation;
		}
		public double GetInterimCoord(double value, bool clip, bool round) {
			double interimCoord = clip ? GetClampedCoord(value) : GetCoord(value);
			if (round)
				interimCoord = Math.Round(interimCoord);
			return interimCoord;
		}
		public double GetCoord(double value) {
			AxisInterval axisInterval = axisIntervalLayout.Interval;
			Interval interval = axisIntervalLayout.Bounds;
			MinMaxValues minMax = new MinMaxValues(transformation.TransformForward(axisInterval.Range.Min), transformation.TransformForward(axisInterval.Range.Max));
			return AxisCoordCalculator.GetCoord(minMax, transformation.TransformForward(value), interval.Length) + interval.Start;
		}
		public double GetClampedCoord(double value) {
			AxisInterval axisInterval = axisIntervalLayout.Interval;
			Interval interval = axisIntervalLayout.Bounds;
			MinMaxValues minMax = new MinMaxValues(transformation.TransformForward(axisInterval.Range.Min), transformation.TransformForward(axisInterval.Range.Max));
			return AxisCoordCalculator.GetClampedCoord(minMax, transformation.TransformForward(value), interval.Length) + interval.Start;
		}
	}
	public struct Interval {
		readonly int start;
		readonly int end;
		readonly int length;
		public int Start { get { return start; } }
		public int Length { get { return length; } }
		public int End { get { return end; } }
		public Interval(int start, int end) {
			this.start = start;
			this.end = end;
			this.length = end - start;
		}
		public bool IsIn(int value) {
			return start <= value && value <= end;
		}
		public static Interval operator |(Interval x, Interval y) {
			int start = Math.Min(x.start, y.start);
			int end = Math.Max(x.end, y.end);
			return new Interval(start, end);
		}
		public static Interval operator &(Interval x, Interval y) {
			if (x.IsIn(y.start) || x.IsIn(y.end)) {
				int start = Math.Max(x.start, y.start);
				int end = Math.Min(x.end, y.end);
				return new Interval(start, end);
			}
			return new Interval(0, 0);
		}
		public static MinMaxValues operator -(Interval x, Interval y) {
			return new MinMaxValues(y.start - x.start, x.end - y.end);
		}
		public static bool operator ==(Interval x, Interval y) {
			return x.Equals(y);
		}
		public static bool operator !=(Interval x, Interval y) {
			return !(x == y);
		}
		public override bool Equals(object obj) {
			if (!(obj is Interval))
				return false;
			Interval intervalBounds = (Interval)obj;
			return intervalBounds.start == start && intervalBounds.length == length;
		}
		public override int GetHashCode() {
			return start ^ length;
		}
		public Interval Clone() {
			return new Interval(this.start, this.end);
		}
	}
	public class AxisIntervalsLayoutRepository {
		readonly XYDiagramPaneBase pane;
		readonly Rectangle mappingBounds;
		readonly Dictionary<IAxisData, List<AxisIntervalLayout>> dictionary = new Dictionary<IAxisData, List<AxisIntervalLayout>>();
		public Rectangle MappingBounds { get { return mappingBounds; } }
		public AxisIntervalsLayoutRepository(XYDiagramPaneBase pane, Rectangle mappingBounds) {
			this.pane = pane;
			this.mappingBounds = mappingBounds;
		}
		public List<AxisIntervalLayout> GetIntervalsLayout(IAxisData axis) {
			if (dictionary.ContainsKey(axis))
				return dictionary[axis];
			List<AxisIntervalLayout> intervalsLayout = AxisIntervalLayout.CreateIntervalsLayout(axis as IIntervalContainer, axis, axis.IsVertical ? mappingBounds.Height : mappingBounds.Width);
			((Axis2D)axis).IntervalBoundsCache.SetAxisIntervalLayout(pane, intervalsLayout);
			dictionary.Add(axis, intervalsLayout);
			return intervalsLayout;
		}
	}
}

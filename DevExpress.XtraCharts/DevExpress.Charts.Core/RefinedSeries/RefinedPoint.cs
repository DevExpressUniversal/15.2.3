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
	public class RefinedPoint : IFullStackedPoint, IStackedPoint, IRangePoint, IFinancialPoint, ISideBySidePoint, IXYWPoint, IXYPoint, IPiePoint, IFunnelPoint, IValuePoint, IPointInteraction {
		public struct PointInteractionContainer {
			public static PointInteractionContainer InvalidContainer { get { return new PointInteractionContainer(null, -1); } }
			readonly IPointInteraction data;
			readonly int index;
			public bool IsEmpty { get { return data == null || index < 0; } }
			public IPointInteraction Data { get { return data; } }
			public int Index { get { return index; } }
			public PointInteractionContainer(IPointInteraction data, int index) {
				this.data = data;
				this.index = index;
			}
		}
		static PointInteractionContainer emptyInteraction = new PointInteractionContainer(null, -1);
		readonly ISeriesPoint point;
		PointInteractionContainer interaction;
		PointInteractionContainer seriesGroupsInteraction;
		int index = -1;
		bool isSupplyPoint;
		Scale argumentScale = Scale.Auto;
		double argument;
		double value1;
		double value2;
		double value3;
		double value4;
		internal PointInteractionContainer SideBySideInteraction { get { return seriesGroupsInteraction; } }
		public double Argument { get { return argument; } }
		internal double Value1 { get { return value1; } }
		internal double Value2 { get { return value2; } }
		internal double Value3 { get { return value3; } }
		internal double Value4 { get { return value4; } }
		internal PointInteractionContainer Interaction { get { return interaction; } }
		internal bool IsSupplyPoint { get { return isSupplyPoint; } set { isSupplyPoint = value; } }
		public IEnumerable<RefinedPoint> Children {
			get {
				AggregatedSeriesPoint aggregatedPoint = point as AggregatedSeriesPoint;
				if (aggregatedPoint != null)
					foreach (RefinedPoint refinedPoint in aggregatedPoint.SourcePoints)
						yield return refinedPoint;
			}
		}
		public ISeriesPoint SeriesPoint {
			get {
				return point;
			}
		}
		public Scale ArgumentScale {
			get {
				if (point != null && argumentScale == Scale.Auto)
					argumentScale = point.ArgumentScaleType;
				return argumentScale;
			}
		}
		public Scale CachedArgumentScale {
			get {
				return argumentScale;
			}
		}
		public bool IsEmpty { get; set; }
		public int Index { get { return index; } set { index = value; } }
		public RefinedPoint() : this(null) {
		}
		public RefinedPoint(ISeriesPoint point) : this(point, double.NaN) {
		}
		public RefinedPoint(ISeriesPoint point, double value) : this(point, double.NaN, value) {
		}
		public RefinedPoint(ISeriesPoint point, double argument, double value) {
			this.point = point;
			this.argument = argument;
			this.value1 = value;
			this.value2 = double.NaN;
			this.value3 = double.NaN;
			this.value4 = double.NaN;
			this.interaction = PointInteractionContainer.InvalidContainer;
		}
		public RefinedPoint(ISeriesPoint point, double argument, double value1, double value2) {
			this.point = point;
			this.argument = argument;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = double.NaN;
			this.value4 = double.NaN;
			this.interaction = PointInteractionContainer.InvalidContainer;
		}
		public RefinedPoint(ISeriesPoint point, double argument, double open, double close, double min, double max) {
			this.point = point;
			this.argument = argument;
			this.value1 = open;
			this.value2 = close;
			this.value3 = min;
			this.value4 = max;
			this.interaction = PointInteractionContainer.InvalidContainer;
		}
		public RefinedPoint(ISeriesPoint point, double argument, IList<double> values) {
			this.point = point;
			this.argument = argument;
			this.value1 = values.Count > 0 ? values[0] : double.NaN;
			this.value2 = values.Count > 1 ? values[1] : double.NaN;
			this.value3 = values.Count > 2 ? values[2] : double.NaN;
			this.value4 = values.Count > 3 ? values[3] : double.NaN;
			this.interaction = PointInteractionContainer.InvalidContainer;
		}
		public RefinedPoint(RefinedPoint refinedPoint, double argument) {
			this.point = refinedPoint.SeriesPoint;
			this.argument = argument;
			this.value1 = refinedPoint.value1;
			this.value2 = refinedPoint.value2;
			this.value3 = refinedPoint.value3;
			this.value4 = refinedPoint.value4;
			this.index = refinedPoint.index;
			this.interaction = PointInteractionContainer.InvalidContainer;
			this.IsEmpty = refinedPoint.IsEmpty;
			this.argumentScale = refinedPoint.argumentScale;
		}
		#region IValuePoint
		double IValuePoint.Value {
			get {
				return value1;
			}
			set {
				value1 = value;
			}
		}
		#endregion
		#region IPiePoint
		bool IPiePoint.IsMaxPoint {
			get {
				SimplePointInteraction simpleInteraction = interaction.Data as SimplePointInteraction;
				return simpleInteraction != null ? simpleInteraction.MaxValue == Value1 : false;
			}
		}
		bool IPiePoint.IsMinPoint {
			get {
				SimplePointInteraction simpleInteraction = interaction.Data as SimplePointInteraction;
				return simpleInteraction != null ? simpleInteraction.MinValue == Value1 : false;
			}
		}
		double IPiePoint.NormalizedValue {
			get {
				SimplePointInteraction simpleInteraction = interaction.Data as SimplePointInteraction;
				if (simpleInteraction != null) {
					if (simpleInteraction.PositiveValuesSum > 0)
						return Value1 > 0 ? Value1 / simpleInteraction.PositiveValuesSum : 0;
					if (simpleInteraction.NegativeValuesSum < 0)
						return Value1 / simpleInteraction.NegativeValuesSum;
					if (simpleInteraction.PositiveValuesSum == 0)
						return 0;
				}
				return double.NaN;
			}
		}
		#endregion
		#region IFunnelPoint
		double IFunnelPoint.NormalizedValue {
			get {
				SimplePointInteraction simpleInteraction = interaction.Data as SimplePointInteraction;
				if (simpleInteraction != null) {
					if (!double.IsNaN(simpleInteraction.PositiveValuesSum)) {
						if (Value1 >= 0)
							return simpleInteraction.MaxValue > 0 ? Value1 / simpleInteraction.MaxValue : 1;
						return double.NaN;
					}
					if (!double.IsNaN(simpleInteraction.NegativeValuesSum))
						return Value1 < 0 ? Value1 / simpleInteraction.MinValue : double.NaN;
				}
				return double.NaN;
			}
		}
		#endregion
		#region IXYWPoint
		double IXYWPoint.Weight {
			get { return value2; }
			set { value2 = value; }
		}
		double IXYWPoint.Size {
			get {
				XYWPointInteraction xywInteraction = interaction.Data as XYWPointInteraction;
				return xywInteraction != null ? xywInteraction.GetXYWPointSize(this) : double.NaN;
			}
		}
		#endregion
		#region IFinancialPoint
		double IFinancialPoint.Open {
			get { return value3; }
			set { value3 = value; }
		}
		double IFinancialPoint.Close {
			get { return value4; }
			set { value4 = value; }
		}
		double IFinancialPoint.Low {
			get { return value1; }
			set { value1 = value; }
		}
		double IFinancialPoint.High {
			get { return value2; }
			set { value2 = value; }
		}
		#endregion
		#region IRangePoint
		double IRangePoint.Value1 {
			get { return value1; }
			set { value1 = value; }
		}
		double IRangePoint.Value2 {
			get { return value2; }
			set { value2 = value; }
		}
		double IRangePoint.Min { get { return Math.Min(Value1, Value2); } }
		double IRangePoint.Max { get { return Math.Max(Value1, Value2); } }
		#endregion
		#region IPointInteraction
		int IPointInteraction.Count { get { return 1; } }
		double IPointInteraction.Argument { get { return argument; } }
		double IPointInteraction.GetMinValue(ISeriesView seriesView) {
			return seriesView.GetRefinedPointMin(this);
		}
		double IPointInteraction.GetMaxValue(ISeriesView seriesView) {
			return seriesView.GetRefinedPointMax(this);
		}
		double IPointInteraction.GetMinAbsValue(ISeriesView seriesView) {
			return seriesView.GetRefinedPointAbsMin(this);
		}
		bool IPointInteraction.IsEmpty { get { return this.IsEmpty; } }
		#endregion
		#region IStackedPoint
		double IStackedPoint.MinValue {
			get {
				StackedPointInteraction stackedInteraction = interaction.Data as StackedPointInteraction;
				return stackedInteraction != null ? stackedInteraction.GetStackedPointMinValue(interaction.Index) : double.NaN;
			}
		}
		double IStackedPoint.MaxValue {
			get {
				StackedPointInteraction stackedInteraction = interaction.Data as StackedPointInteraction;
				return stackedInteraction != null ? stackedInteraction.GetStackedPointMaxValue(interaction.Index) : double.NaN;
			}
		}
		double IStackedPoint.TotalValue {
			get {
				StackedPointInteraction stackedInteraction = interaction.Data as StackedPointInteraction;
				if (stackedInteraction == null)
					return double.NaN;
				return value1 < 0 ? stackedInteraction.MinValue : stackedInteraction.MaxValue;
			}
		}
		double IStackedPoint.TotalMinValue {
			get {
				StackedPointInteraction stackedInteraction = interaction.Data as StackedPointInteraction;
				return stackedInteraction != null ? stackedInteraction.MinValue : double.NaN;
			}
		}
		double IStackedPoint.TotalMaxValue {
			get {
				StackedPointInteraction stackedInteraction = interaction.Data as StackedPointInteraction;
				return stackedInteraction != null ? stackedInteraction.MaxValue : double.NaN;
			}
		}
		#endregion
		#region IFullStackedPoint
		double IFullStackedPoint.NormalizedValue {
			get {
				return ((IStackedPoint)this).MaxValue - ((IStackedPoint)this).MinValue;
			}
		}
		#endregion
		#region IArgumentPoint
		double IArgumentPoint.Argument {
			get { return argument; }
			set { argument = value; }
		}
		#endregion
		#region ISideBySidePoint
		double ISideBySidePoint.BarWidth {
			get {
				SideBySideInteractionBase sideBySideInteractionContainer = seriesGroupsInteraction.Data as SideBySideInteractionBase;
				return sideBySideInteractionContainer != null ? sideBySideInteractionContainer.GetBarWidth(seriesGroupsInteraction.Index) : double.NaN;
			}
		}
		double ISideBySidePoint.Offset {
			get {
				SideBySideInteractionBase sideBySideInteractionContainer = seriesGroupsInteraction.Data as SideBySideInteractionBase;
				return sideBySideInteractionContainer != null ? sideBySideInteractionContainer.GetBarDistance(seriesGroupsInteraction.Index) : double.NaN;
			}
		}
		int ISideBySidePoint.FixedOffset {
			get {
				SideBySideInteractionBase sideBySideInteractionContainer = seriesGroupsInteraction.Data as SideBySideInteractionBase;
				return sideBySideInteractionContainer != null ? sideBySideInteractionContainer.GetFixedOffset(seriesGroupsInteraction.Index) : 0;
			}
		}
		#endregion
		public void SetInteraction(IPointInteraction data, int index) {
			interaction = new PointInteractionContainer(data, index);
		}
		public void SetSeriesGroupsInteraction(IPointInteraction data, int index) {
			seriesGroupsInteraction = new PointInteractionContainer(data, index);
		}
		public void ResetInteraction() {
			interaction = emptyInteraction;
		}
		public void ResetSeriesGroupsInteraction() {
			seriesGroupsInteraction = emptyInteraction;
		}
		public RangeValue GetMinMax(ISeriesView seriesView) {
			if (!Interaction.IsEmpty)
				return seriesView.GetMinMax(interaction.Data, interaction.Index);
			return seriesView.GetMinMax(this, 0);
		}
		public List<RefinedSeries> GetSideBySideSeries() {
			SideBySideInteractionBase sideBySideInteractionContainer = seriesGroupsInteraction.Data as SideBySideInteractionBase;
			return sideBySideInteractionContainer != null ? sideBySideInteractionContainer.Series : null;
		}
		public double GetValue(ValueLevelInternal valueLevel) {
			switch (valueLevel) {
				case ValueLevelInternal.Value:
					return this.value1;
				case ValueLevelInternal.Value_1:
					return this.value1;
				case ValueLevelInternal.Low:
					return this.value1;
				case ValueLevelInternal.Value_2:
					return this.value2;
				case ValueLevelInternal.Weight:
					return this.value2;
				case ValueLevelInternal.High:
					return this.value2;
				case ValueLevelInternal.Open:
					return this.value3;
				case ValueLevelInternal.Close:
					return this.value4;
			}
			throw new ArgumentException("The RefinedPoint can't return value because the given ValueLevelInternal is unknown.");
		}
	}
	public class RefinedPointArgumentComparer : IComparer<RefinedPoint> {
		int IComparer<RefinedPoint>.Compare(RefinedPoint x, RefinedPoint y) {
			return SortingUtils.CompareDoubles(x.Argument, y.Argument);
		}
	}
}

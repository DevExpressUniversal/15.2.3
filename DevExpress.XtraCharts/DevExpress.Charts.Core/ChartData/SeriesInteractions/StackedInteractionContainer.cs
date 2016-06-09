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

using DevExpress.Charts.Native;
using System;
using System.Collections.Generic;
namespace DevExpress.Charts.Native{
	public class StackedInteractionContainer : SeriesInteractionContainer {
		#region Nested class: PointComparer
		class PointComparer : Comparer<RefinedPoint> {
			public override int Compare(RefinedPoint x, RefinedPoint y) {
				return x.Argument.CompareTo(y.Argument);
			}
		}
		#endregion
		readonly PointInteractionCollection sortedInteractionsByArgument;
		readonly PointInteractionCollection sortedInteractionsByMinValue;
		readonly PointInteractionCollection sortedInteractionsByMaxValue;
		readonly PointInteractionCollection sortedInteractionsByMinAbsValue;
		readonly bool isContinousView;
		bool shouldResort;
		internal bool IsContinousView { get { return isContinousView; } }
		internal PointInteractionCollection SortedInteractionsByArgument { get { return sortedInteractionsByArgument; } }
		internal PointInteractionCollection SortedInteractionsByMinValue { get { return sortedInteractionsByMinValue; } }
		internal PointInteractionCollection SortedInteractionsByMaxValue { get { return sortedInteractionsByMaxValue; } }
		internal PointInteractionCollection SortedInteractionsByMinAbsValue { get { return sortedInteractionsByMinAbsValue; } }
		public override double Max {
			get {
				if (SeriesView != null && sortedInteractionsByMaxValue.Count > 0) {
					Resort();
					return sortedInteractionsByMaxValue[0].GetMaxValue(SeriesView);
				}
				return double.NaN;
			}
		}
		public override double Min {
			get {
				if (SeriesView != null && sortedInteractionsByMinValue.Count > 0) {
					Resort();
					return sortedInteractionsByMinValue[0].GetMinValue(SeriesView);
				}
				return double.NaN;
			}
		}
		public StackedInteractionContainer(ISeriesView view)
			: this(view, false) {
		}
		public StackedInteractionContainer(ISeriesView view, bool isContinousView)
			: base(view) {
			sortedInteractionsByArgument = new PointInteractionCollection(new InteractionComparerByArgument());
			sortedInteractionsByMinValue = new PointInteractionCollection(new InteractionComparerByMinValue(view));
			sortedInteractionsByMaxValue = new PointInteractionCollection(new InteractionComparerByMaxValue(view));
			sortedInteractionsByMinAbsValue = new PointInteractionCollection(new InteractionComparerByMinAbsValue(view));
			this.isContinousView = isContinousView;
		}
		void Resort() {
			if (!shouldResort)
				return;
			sortedInteractionsByMinValue.Sort();
			sortedInteractionsByMaxValue.Sort();
			sortedInteractionsByMinAbsValue.Sort();
			shouldResort = false;
		}
		void AddNewPointInteraction(int seriesIndex, RefinedPoint point) {
			StackedPointInteraction interaction = CreatePointInteraction(seriesIndex, point);
			sortedInteractionsByArgument.Add(interaction);
			sortedInteractionsByMaxValue.Add(interaction);
			sortedInteractionsByMinValue.Add(interaction);
			sortedInteractionsByMinAbsValue.Add(interaction);
		}
		void InsertRefinedPoint(int seriesIndex, RefinedPoint point) {
			int argumentIndex = sortedInteractionsByArgument.BinarySearch(point);
			if (argumentIndex < 0)
				AddNewPointInteraction(seriesIndex, point);
			else {
				StackedPointInteraction interaction = (StackedPointInteraction)sortedInteractionsByArgument[argumentIndex];
				interaction.InsertPoint(seriesIndex, point);
				shouldResort = true;
			}
		}
		void ClearSupplyPoints() {
			for (int i = 0; i < sortedInteractionsByArgument.Count; i++) {
				StackedPointInteraction interaction = (StackedPointInteraction)sortedInteractionsByArgument[i];
				if (interaction.IsSupplyInteraction) {
					sortedInteractionsByArgument.RemoveAt(i);
					sortedInteractionsByMinValue.Remove(interaction);
					sortedInteractionsByMaxValue.Remove(interaction);
					sortedInteractionsByMinAbsValue.Remove(interaction);
					i--;
				}
				else {
					for (int j = 0; j < interaction.Count; j++) {
						if (interaction[j] != null && interaction[j].IsSupplyPoint)
							interaction[j] = null;
					}
				}
			}
		}
		void AddSupplyInteractions() {
			for (int i = 0; i < sortedInteractionsByArgument.Count; i++) {
				StackedPointInteraction interaction = (StackedPointInteraction)sortedInteractionsByArgument[i];
				if (interaction.HasEmptyPoints) {
					if (i > 0) {
						StackedPointInteraction previousInteraction = (StackedPointInteraction)sortedInteractionsByArgument[i - 1];
						if (!previousInteraction.IsSupplyInteraction) {
							if (AddSupplyInteraction(i, interaction, previousInteraction))
								i++;
							if (AddSupplyInteraction(i, previousInteraction, interaction))
								i++;
						}
					}
					if (i < sortedInteractionsByArgument.Count - 1) {
						StackedPointInteraction nextInteraction = (StackedPointInteraction)sortedInteractionsByArgument[i + 1];
						if (AddSupplyInteraction(i + 1, nextInteraction, interaction))
							i++;
						if (AddSupplyInteraction(i + 1, interaction, nextInteraction))
							i++;
					}
				}
			}
		}
		void AddSupplyPoints() {
			for (int i = 0; i < Series.Count; i++) {
				List<RangeIndexes> additionIntervals = new List<RangeIndexes>();
				int startPointIndex = -1;
				for (int j = 0; j < sortedInteractionsByArgument.Count; j++) {
					StackedPointInteraction interaction = (StackedPointInteraction)sortedInteractionsByArgument[j];
					RefinedPoint point = interaction[i];
					if (point != null) {
						if (point.IsEmpty)
							startPointIndex = -1;
						else {
							if (startPointIndex > -1 && j - startPointIndex > 1)
								additionIntervals.Add(new RangeIndexes(startPointIndex, j));
							startPointIndex = j;
						}
					}
				}
				foreach (RangeIndexes additionInterval in additionIntervals) {
					StackedPointInteraction interaction1 = (StackedPointInteraction)sortedInteractionsByArgument[additionInterval.Min];
					StackedPointInteraction interaction2 = (StackedPointInteraction)sortedInteractionsByArgument[additionInterval.Max];
					for (int j = additionInterval.Min + 1; j < additionInterval.Max; j++) {
						double argument = sortedInteractionsByArgument[j].Argument;
						double value1 = ((IValuePoint)interaction1[i]).Value;
						double argument1 = interaction1.Argument;
						double value2 = ((IValuePoint)interaction2[i]).Value;
						double argument2 = interaction2.Argument;
						double value = (argument - argument1) / (argument2 - argument1) * (value2 - value1) + value1;
						RefinedPoint point = new RefinedPoint(null, argument, value);
						point.IsSupplyPoint = true;
						((StackedPointInteraction)sortedInteractionsByArgument[j]).InsertPoint(i, point);
					}
				}
			}
		}
		void RecalculateSupplyPoints() {
			ClearSupplyPoints();
			AddSupplyPoints();
			AddSupplyInteractions();
		}
		int FindInteractionIndexByArgument(int startInteractionIndex, double exceptedArgument) {
			for (int index = startInteractionIndex; index < sortedInteractionsByArgument.Count; index++) {
				StackedPointInteraction interaction = (StackedPointInteraction)sortedInteractionsByArgument[index];
				if (interaction.IsSupplyInteraction)
					continue;
				if (interaction.Argument == exceptedArgument)
					return index;
				else if (interaction.Argument > exceptedArgument)
					return ~index;
			}
			return ~sortedInteractionsByArgument.Count;
		}
		bool AddSupplyInteraction(int index, StackedPointInteraction interaction, StackedPointInteraction supplyValues) {
			StackedPointInteraction supplyInteraction = null;
			for (int i = 0; i < interaction.Count; i++) {
				if (supplyValues[i] != null && !supplyValues[i].IsEmpty && interaction[i] != null && !interaction[i].IsEmpty) {
					RefinedPoint point = new RefinedPoint(null, supplyValues.Argument, ((IXYPoint)supplyValues[i]).Value);
					point.IsSupplyPoint = true;
					if (supplyInteraction == null) {
						supplyInteraction = CreatePointInteraction(i, point);
						supplyInteraction.IsSupplyInteraction = true;
					}
					else
						supplyInteraction.InsertPoint(i, point);
				}
			}
			if (supplyInteraction != null) {
				if (index > 0 && IsEqualInteractions((StackedPointInteraction)sortedInteractionsByArgument[index - 1], supplyInteraction)
					|| IsEqualInteractions((StackedPointInteraction)sortedInteractionsByArgument[index], supplyInteraction)) {
					return false;
				}
				sortedInteractionsByArgument.Insert(index, supplyInteraction);
				sortedInteractionsByMinValue.Add(supplyInteraction);
				sortedInteractionsByMaxValue.Add(supplyInteraction);
				sortedInteractionsByMinAbsValue.Add(interaction);
				return true;
			}
			return false;
		}
		bool IsEqualInteractions(StackedPointInteraction interaction1, StackedPointInteraction interaction2) {
			if (interaction1.Argument == interaction2.Argument && interaction1.IsEmpty == interaction2.IsEmpty) {
				for (int i = 0; i < interaction1.Count; i++) {
					IStackedPoint point1 = (IStackedPoint)interaction1[i];
					IStackedPoint point2 = (IStackedPoint)interaction2[i];
					if (point1 == null && point2 == null || point1 != null && point2 != null && (point1.Value == point2.Value || point1.IsEmpty == point2.IsEmpty))
						continue;
					return false;
				}
				return true;
			}
			return false;
		}
		protected virtual StackedPointInteraction CreatePointInteraction(int seriesIndex, RefinedPoint point) {
			return new StackedPointInteraction(Series, seriesIndex, point);
		}
		protected override void InsertRefinedPoints(int seriesIndex, RefinedSeries refinedSeries) {
			if (seriesIndex < 0)
				return;
			IList<RefinedPoint> points = refinedSeries.FinalPointsSortedByArgument;
			if (sortedInteractionsByArgument.Count == 0) {
				foreach (RefinedPoint point in points)
					InsertRefinedPoint(seriesIndex, point);
			}
			else {
				foreach (StackedPointInteraction interaction in sortedInteractionsByArgument)
					interaction.InsertSeries(refinedSeries, seriesIndex, null);
				int interactionIndex = 0;
				for (int pointIndex = 0; pointIndex < points.Count; pointIndex++) {
					RefinedPoint point = (RefinedPoint)points[pointIndex];
					interactionIndex = FindInteractionIndexByArgument(interactionIndex, points[pointIndex].Argument);
					if (interactionIndex >= 0) {
						((StackedPointInteraction)sortedInteractionsByArgument[interactionIndex]).InsertPoint(seriesIndex, point);
					}
					else {
						AddNewPointInteraction(seriesIndex, point);
						interactionIndex = ~interactionIndex + 1;
					}
				}
			}
			if (isContinousView)
				RecalculateSupplyPoints();
			shouldResort = true;
		}
		protected override void RemoveRefinedPoints(int seriesIndex, RefinedSeries refinedSeries) {
			if (seriesIndex < 0)
				return;
			for (int i = 0; i < sortedInteractionsByArgument.Count; i++) {
				StackedPointInteraction interaction = (StackedPointInteraction)sortedInteractionsByArgument[i];
				interaction.RemoveSeries(seriesIndex);
				if (interaction.IsEmpty) {
					sortedInteractionsByArgument.RemoveAt(i);
					sortedInteractionsByMinValue.Remove(interaction);
					sortedInteractionsByMaxValue.Remove(interaction);
					sortedInteractionsByMinAbsValue.Remove(interaction);
					i--;
				}
			}
			if (isContinousView)
				RecalculateSupplyPoints();
			shouldResort = true;
		}
		protected override void InsertRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point) {
			int seriesIndex = GetSeriesIndex(refinedSeries);
			if (seriesIndex < 0)
				throw new ArgumentException();
			if (isContinousView) 
				ClearSupplyPoints();
			InsertRefinedPoint(seriesIndex, point);
			if (isContinousView) {
				AddSupplyPoints();
				AddSupplyInteractions();
				shouldResort = true;
			}
		}
		protected override void RemoveRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point) {
			int seriesIndex = GetSeriesIndex(refinedSeries);
			if (seriesIndex < 0)
				throw new ArgumentException();
			int argumentIndex = sortedInteractionsByArgument.BinarySearch(point);
			if (argumentIndex >= 0) {
				StackedPointInteraction interaction = (StackedPointInteraction)sortedInteractionsByArgument[argumentIndex];
				interaction.RemovePoint(seriesIndex);
				if (interaction.IsEmpty) {
					sortedInteractionsByArgument.RemoveAt(argumentIndex);
					sortedInteractionsByMinValue.Remove(interaction);
					sortedInteractionsByMaxValue.Remove(interaction);
					sortedInteractionsByMinAbsValue.Remove(interaction);
				}
				else
					shouldResort = true;
				if (isContinousView) {
					RecalculateSupplyPoints();
					shouldResort = true;
				}
			}
		}
		public override void Recalculate() {
			sortedInteractionsByArgument.Clear();
			sortedInteractionsByMinValue.Clear();
			sortedInteractionsByMaxValue.Clear();
			sortedInteractionsByMinAbsValue.Clear();
			for (int i = 0; i < Series.Count; i++)
				InsertRefinedPoints(i, Series[i]);
		}
		public override double GetAbsMinValue() {
			if (SeriesView != null && sortedInteractionsByMinAbsValue.Count > 0)
				return sortedInteractionsByMinAbsValue[0].GetMinAbsValue(SeriesView);
			return double.NaN;
		}
		public IList<RefinedPoint> GetStackedPointsForDrawing(RefinedSeries refinedSeries) {
			List<RefinedPoint> points = new List<RefinedPoint>(sortedInteractionsByArgument.Count);
			int seriesIndex = GetSeriesIndex(refinedSeries);
			foreach (StackedPointInteraction interaction in sortedInteractionsByArgument) {
				IStackedPoint point = interaction[seriesIndex];
				IStackedPoint previousPoint = points.Count > 0 ? points[points.Count - 1] : null;
				if (point != null && (previousPoint == null || previousPoint.Argument != point.Argument || previousPoint.MaxValue != point.MaxValue || previousPoint.MinValue != point.MinValue))
					points.Add(interaction[seriesIndex]);
			}
			return points;
		}
	}
}

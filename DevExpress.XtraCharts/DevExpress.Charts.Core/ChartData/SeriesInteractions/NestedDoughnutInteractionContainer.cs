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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public class NestedDoughnutInteractionContainer : SeriesGroupsInteractionContainer {
		double[] totalGroupIndentsInPixels;
		double[] holeRadiuses;
		double[] startOffsets;
		double[] startOffsetsInPixels;
		double[] normalizedWeights;
		double[] explodedFactors;
		bool[] isExploded;
		public NestedDoughnutInteractionContainer(ISeriesView view)
			: base(view) { }
		double GetDoubleArrayValue(IList<double> array, int index) {
			if (index < 0 || index >= array.Count)
				return 0.0;
			return array[index];
		}
		void UpdateInteractionKeys() {
			for (int i = 0; i < Series.Count; i++)
				Series[i].SetGroupsInteraction(this, i);
		}
		protected override void InsertRefinedPoints(int seriesIndex, RefinedSeries refinedSeries) {
		}
		protected override void RemoveRefinedPoints(int seriesIndex, RefinedSeries refinedSeries) {
		}
		protected override void InsertRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point) {
		}
		protected override void RemoveRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point) {
		}
		protected override void RecalculateCore(int groupsCount) {
			int seriesCount = Series.Count;
			totalGroupIndentsInPixels = new double[seriesCount];
			holeRadiuses = new double[seriesCount];
			startOffsets = new double[seriesCount];
			startOffsetsInPixels = new double[seriesCount];
			normalizedWeights = new double[seriesCount];
			explodedFactors = new double[seriesCount];
			isExploded = new bool[seriesCount];
			double[] groupOffsetInPixels = new double[groupsCount];
			double[] groupTotalWeights = new double[groupsCount];
			double[] groupWeights = new double[groupsCount];
			double[] groupHoleRadiuses = new double[groupsCount];
			double[] groupIndents = new double[groupsCount];
			double[] groupInnerSeriesIndents = new double[groupsCount];
			double[] groupExplodedFactors = new double[groupsCount];
			bool[] groupIsExploded = new bool[groupsCount];
			bool[] isOuterSereisFound = new bool[groupsCount];
			for (int seriesIndex = 0; seriesIndex < seriesCount; seriesIndex++) {
				RefinedSeries series = Series[seriesIndex];
				INestedDoughnutSeriesView nestedView = series.SeriesView as INestedDoughnutSeriesView;
				int groupIndex = GetSeriesGroupIndex(series);
				if (nestedView != null) {
					groupTotalWeights[groupIndex] += nestedView.Weight;
					groupIndents[groupIndex] += nestedView.InnerIndent;
					groupInnerSeriesIndents[groupIndex] = nestedView.InnerIndent;
				}
			}
			for (int seriesIndex = 0; seriesIndex < seriesCount; seriesIndex++) {
				RefinedSeries series = Series[seriesIndex];
				INestedDoughnutSeriesView nestedView = series.SeriesView as INestedDoughnutSeriesView;
				if (nestedView != null) {
					int groupIndex = GetSeriesGroupIndex(series);
					startOffsetsInPixels[seriesIndex] = groupOffsetInPixels[groupIndex];
					groupOffsetInPixels[groupIndex] += nestedView.InnerIndent;
					double startOffset = groupWeights[groupIndex];
					startOffsets[seriesIndex] = startOffset;
					double normalizedWeight = nestedView.Weight / groupTotalWeights[groupIndex];
					normalizedWeights[seriesIndex] = normalizedWeight;
					groupWeights[groupIndex] += normalizedWeight;
					bool isOuter = false;
					if (startOffset == 0.0 && !isOuterSereisFound[groupIndex]) {
						isOuter = true;
						isOuterSereisFound[groupIndex] = true;
					}
					nestedView.IsOutside = isOuter;
					if (isOuter) {
						groupHoleRadiuses[groupIndex] = nestedView.HoleRadiusPercent / 100.0;
						groupExplodedFactors[groupIndex] = nestedView.ExplodedDistancePercentage / 100.0;
						groupIsExploded[groupIndex] = nestedView.HasExplodedPoints(series);
					}
					holeRadiuses[seriesIndex] = groupHoleRadiuses[groupIndex];
					explodedFactors[seriesIndex] = groupExplodedFactors[groupIndex];
					isExploded[seriesIndex] = groupIsExploded[groupIndex];
					totalGroupIndentsInPixels[seriesIndex] = groupIndents[groupIndex] - groupInnerSeriesIndents[groupIndex];
				}
			}
			UpdateInteractionKeys();
		}
		public double GetTotalGroupIndentInPixels(int index) {
			return GetDoubleArrayValue(totalGroupIndentsInPixels, index);
		}
		public double GetHoleRadius(int index) {
			return GetDoubleArrayValue(holeRadiuses, index);
		}
		public double GetStartOffset(int index) {
			return GetDoubleArrayValue(startOffsets, index);
		}
		public double GetStartOffsetInPixels(int index) {
			return GetDoubleArrayValue(startOffsetsInPixels, index);
		}
		public double GetNormalizedWeight(int index) {
			return GetDoubleArrayValue(normalizedWeights, index);
		}
		public double GetExplodedFactor(int index) {
			return GetDoubleArrayValue(explodedFactors, index);
		}
		public bool GetIsExploded(int index) {
			if (index < 0 || index >= isExploded.Length)
				return false;
			return isExploded[index];
		}
	}
}

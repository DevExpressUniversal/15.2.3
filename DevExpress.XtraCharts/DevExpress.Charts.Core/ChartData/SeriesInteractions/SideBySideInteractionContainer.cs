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
	public class SideBySideInteractionContainer : SeriesInteractionContainer {
		readonly PointInteractionCollection sortedInteractionsByArgument;
		bool EqualBarWidth { get { return SideBySideBarSeriesView.EqualBarWidth; } }
		ISideBySideBarSeriesView SideBySideBarSeriesView { get { return (ISideBySideBarSeriesView)SeriesView; } }
		public override double Max { get { return double.NaN; } }
		public override double Min { get { return double.NaN; } }
		public SideBySideInteractionContainer(ISeriesView view) : base(view) {
			if (!(view is ISideBySideBarSeriesView))
				throw new ArgumentException();
			sortedInteractionsByArgument = new PointInteractionCollection(new InteractionComparerByArgument());
		}
		void InsertRefinedPoint(RefinedSeries refinedSeries, RefinedPoint refinedPoint, bool changeInteractionLayout) {
			SideBySideInteractionBase pointInteraction;
			if (Series.Count > 1 || changeInteractionLayout) {
				int index = sortedInteractionsByArgument.BinarySearch(refinedPoint);
				if (index < 0) {
					index = ~index;
					pointInteraction = new SideBySidePointInteraction(refinedPoint.Argument, EqualBarWidth);
					int end = changeInteractionLayout ? GetSeriesIndex(refinedSeries) + 1 : Series.Count;
					for (int i = 0; i < end; i++)
						pointInteraction.AddSeries(Series[i]);
					sortedInteractionsByArgument.Insert(index, pointInteraction);
				}
				else
					pointInteraction = (SideBySidePointInteraction)sortedInteractionsByArgument[index];
			}
			else {
				if (sortedInteractionsByArgument.Count == 0) {
					SideBySideSeriesInteraction interaction = new SideBySideSeriesInteraction();
					interaction.AddSeries(refinedSeries);
					sortedInteractionsByArgument.Add(interaction);
				}
				pointInteraction = ((SideBySideInteractionBase)sortedInteractionsByArgument[0]);
			}
			pointInteraction.InsertPoint(refinedSeries, refinedPoint);
			pointInteraction.Invalidate();
		}
		void InsertRefinedPoints(RefinedSeries refinedSeries, bool changeInteractionLayout) {
			foreach (SideBySideInteractionBase interaction in sortedInteractionsByArgument)
				interaction.AddSeries(refinedSeries);
			foreach (RefinedPoint point in refinedSeries.FinalPoints)
				InsertRefinedPoint(refinedSeries, point, changeInteractionLayout);
		}
		protected override void InsertRefinedPoints(int seriesIndex, RefinedSeries refinedSeries) {
			if (seriesIndex > 0 && sortedInteractionsByArgument.Count > 0 && sortedInteractionsByArgument[0] is SideBySideSeriesInteraction) {
				sortedInteractionsByArgument.Clear();
				foreach(RefinedSeries series in Series)
					InsertRefinedPoints(series, true);
			}
			else
				InsertRefinedPoints(refinedSeries, false);
		}
		protected override void RemoveRefinedPoints(int seriesIndex, RefinedSeries refinedSeries) {
			for (int i = 0; i < sortedInteractionsByArgument.Count; i++) {
				SideBySideInteractionBase interaction = (SideBySideInteractionBase)sortedInteractionsByArgument[i];
				interaction.RemoveSeries(refinedSeries);
				if (interaction.IsEmpty) {
					sortedInteractionsByArgument.RemoveAt(i);
					i--;
				}
				else
					interaction.Invalidate();
			}
		}
		protected override void InsertRefinedPoint(RefinedSeries refinedSeries, RefinedPoint refinedPoint) {
			InsertRefinedPoint(refinedSeries, refinedPoint, false);
		}
		protected override void RemoveRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point) {
			if (sortedInteractionsByArgument.Count > 0 && sortedInteractionsByArgument[0] is SideBySideSeriesInteraction) {
				((SideBySideInteractionBase)sortedInteractionsByArgument[0]).RemovePoint(refinedSeries, point);
			}
			else {
				int index = sortedInteractionsByArgument.BinarySearch(point);
				if (index < 0)
					return;
				SideBySideInteractionBase pointInteraction = (SideBySideInteractionBase)sortedInteractionsByArgument[index];
				pointInteraction.RemovePoint(refinedSeries, point);
				if (pointInteraction.IsEmpty)
					sortedInteractionsByArgument.RemoveAt(index);
				else
					pointInteraction.Invalidate();
			}
		}
		public override void Recalculate() {
			IList<RefinedSeries> series = Series.GetRange(0, Series.Count);
			for (int i = 0; i < series.Count; i++)
				RemoveSeries(series[i]);
			for (int i = 0; i < series.Count; i++)
				AddSeries(series[i]);
		}
		public override double GetAbsMinValue() {
			return double.NaN;
		}
	}
}

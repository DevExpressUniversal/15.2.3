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

using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public abstract class SeriesGroupsInteractionContainer : SeriesInteractionContainer, IPointInteraction {
		readonly SeriesGroupsManager groupsManager;
		public override double Max { get { return double.NaN; } }
		public override double Min { get { return double.NaN; } }
		public SeriesGroupsInteractionContainer(ISeriesView view)
			: base(view) {
			groupsManager = new SeriesGroupsManager(this.Series);
		}
		#region IPointInteraction
		int IPointInteraction.Count { get { return 0; } }
		double IPointInteraction.Argument { get { return double.NaN; } }
		double IPointInteraction.GetMinValue(ISeriesView seriesView) {
			return double.NaN;
		}
		double IPointInteraction.GetMaxValue(ISeriesView seriesView) {
			return double.NaN;
		}
		double IPointInteraction.GetMinAbsValue(ISeriesView seriesView) {
			return double.NaN;
		}
		#endregion
		protected int GetSeriesGroupIndex(RefinedSeries series) {
			return groupsManager.GetGroupIndexBySeries(series);
		}
		protected bool IsSeriesInGroups(RefinedSeries series) {
			return groupsManager.IsSeriesInGroups(series);
		}
		protected abstract void RecalculateCore(int groupsCount);
		protected override void RemoveRefinedPoints(int seriesIndex, RefinedSeries refinedSeries) {
			IList<RefinedPoint> points = refinedSeries.FinalPoints;
			foreach (RefinedPoint point in points)
				point.ResetSeriesGroupsInteraction();
		}
		protected override void RemoveRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point) {
			point.ResetSeriesGroupsInteraction();
		}
		public override void AddSeries(RefinedSeries refinedSeries) {
			int index = GetSeriesIndex(refinedSeries);
			if (index >= 0)
				RemoveSeries(refinedSeries);
			else
				index = ~index;
			Series.Insert(index, refinedSeries);
			Recalculate();
		}
		public override void RemoveSeries(RefinedSeries refinedSeries) {
			int index = GetSeriesIndex(refinedSeries);
			if (index >= 0) {
				Series.RemoveAt(index);
				Recalculate();
			}
		}
		public override void Recalculate() {
			groupsManager.UpdateGroups();
			RecalculateCore(groupsManager.GroupsCount);
		}
		public override double GetAbsMinValue() {
			return 0;
		}
	}
}

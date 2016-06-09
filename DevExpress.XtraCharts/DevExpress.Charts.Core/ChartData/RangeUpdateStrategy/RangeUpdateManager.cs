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
	public abstract class RangeUpdateEnqueuer {
		public static void Update(RefinedSeriesGroupController groupController, IList<IAxisRangeData> rangesForReset, VisualRangeUpdateMode visualRangeUpdateMode, IXYDiagram diagram, IList<RefinedSeries> activeSeries) {
			if (diagram != null && (diagram.AxisX == null || diagram.AxisY == null))
				return;
			RangeUpdateEnqueuer manager;
			List<IPaneAxesContainer> containers;
			if (diagram != null)
				containers = diagram.GetPaneAxesContainers(activeSeries);
			else
				containers = new List<IPaneAxesContainer>();
			if (diagram == null)
				manager = new EmptyRangeUpdateEnqueuer(groupController, rangesForReset, visualRangeUpdateMode);
			else if (diagram.ScrollingEnabled || diagram is ISwiftPlotDiagram)
				manager = new RangeUpdateNonDependentEnqueuer(groupController, rangesForReset, visualRangeUpdateMode, containers);
			else
				manager = new RangeUpdateDependentEnqueuer(groupController, rangesForReset, visualRangeUpdateMode, containers);
			manager.Update();
		}
		readonly Dictionary<IAxisData, RefinedSeriesGroup> refinedSeriesGroups;
		readonly IList<IAxisRangeData> rangesForReset;
		readonly VisualRangeUpdateMode visualRangeUpdateMode;
		protected Dictionary<IAxisData, RefinedSeriesGroup> RefinedSeriesGroups { get { return refinedSeriesGroups; } }
		protected IList<IAxisRangeData> RangesForReset { get { return rangesForReset; } }
		protected VisualRangeUpdateMode VisualRangeUpdateMode { get { return visualRangeUpdateMode; } }
		public RangeUpdateEnqueuer(RefinedSeriesGroupController groupController, IList<IAxisRangeData> rangesForReset, VisualRangeUpdateMode visualRangeUpdateMode) {
			this.rangesForReset = rangesForReset;
			this.visualRangeUpdateMode = visualRangeUpdateMode;
			this.refinedSeriesGroups = new Dictionary<IAxisData, RefinedSeriesGroup>();
			foreach (RefinedSeriesGroup refinedSeriesGroup in groupController.RefinedSeriesGroups.Values) {
				IAxisData axis = refinedSeriesGroup.GroupKey.AxisData;
				if (axis != null)
					refinedSeriesGroups.Add(axis, refinedSeriesGroup);
			}
		}
		protected abstract void Update();
	}
	public class EmptyRangeUpdateEnqueuer : RangeUpdateEnqueuer, IRangeUpdater {
		public EmptyRangeUpdateEnqueuer(RefinedSeriesGroupController groupController, IList<IAxisRangeData> rangesForReset, VisualRangeUpdateMode visualRangeUpdateMode)
			: base(groupController, rangesForReset, visualRangeUpdateMode) { }
		protected override void Update() {
			foreach (RefinedSeriesGroup refinedSeriesGroup in RefinedSeriesGroups.Values) {
				IAxisData axis = refinedSeriesGroup.GroupKey.AxisData;
				Update(axis, GetInternalRange(axis));
			}
		}
		void Update(IAxisData axis, MinMaxValues minMaxInternalValues) {
			MinMaxValues refinedRange = GetRefinedRange(axis);
			ICollection<RefinedSeries> refinedSeriesCollection = GetRefinedSeries(axis);
			bool needResetVisualRange = RangesForReset.Contains(axis.VisualRange);
			RangeUpdateStrategy.UpdateRange(axis, minMaxInternalValues, refinedRange, refinedSeriesCollection, needResetVisualRange, VisualRangeUpdateMode);
		}
		ICollection<RefinedSeries> GetRefinedSeries(IAxisData axis) {
			if (RefinedSeriesGroups.ContainsKey(axis)) {
				RefinedSeriesGroup group = RefinedSeriesGroups[axis];
				return group.RefinedSeries;
			}
			return null;
		}
		MinMaxValues GetInternalRange(IAxisData axis) {
			if (RefinedSeriesGroups.ContainsKey(axis)) {
				RefinedSeriesGroup group = RefinedSeriesGroups[axis];
				return group.GetMinMaxValuesFromSeries();
			}
			return new MinMaxValues(double.NaN);
		}
		MinMaxValues GetRefinedRange(IAxisData axis) {
			if (RefinedSeriesGroups.ContainsKey(axis)) {
				RefinedSeriesGroup group = RefinedSeriesGroups[axis];
				return group.GetMinMaxRefined();
			}
			return new MinMaxValues(double.NaN);
		}
		#region IValueProvider
		MinMaxValues IRangeUpdater.GetInternalValues(IAxisData axis) {
			return GetInternalRange(axis);
		}
		void IRangeUpdater.UpdateRanges(IAxisData axis, MinMaxValues internalValue) {
			Update(axis, internalValue);
		}
		#endregion IValueProvider
	}
	public class RangeUpdateNonDependentEnqueuer : EmptyRangeUpdateEnqueuer {
		protected class UpdateGroup {
			IAxisData primaryAxis;
			List<IAxisData> axes;
			public List<IAxisData> Axes { get { return axes; } }
			public bool Auto { get { return axes[0].VisualRange.CorrectionMode == RangeCorrectionMode.Auto && axes[0].WholeRange.CorrectionMode == RangeCorrectionMode.Auto; } }
			public UpdateGroup() {
				this.axes = new List<IAxisData>();
			}
			public void SetPrimaryAxis(IAxisData axis) {
				if (primaryAxis != null)
					axes.Remove(primaryAxis);
				primaryAxis = axis;
				if (!axes.Contains(axis))
					axes.Insert(0, axis);
			}
			public void AddSecondaryAxis(IAxisData axis) {
				if (!axes.Contains(axis))
					axes.Add(axis);
			}
			public void Update(IRangeUpdater provider) {
				provider.UpdateRanges(primaryAxis, provider.GetInternalValues(primaryAxis));
				foreach (IAxisData axis in axes) {
					if (axis == primaryAxis)
						continue;
					provider.UpdateRanges(axis, provider.GetInternalValues(axis));
					CorrectSecondaryAxes(primaryAxis, axis);
				}
			}
			void CorrectSecondaryAxes(IAxisData primaryAxis, IAxisData secondaryAxes) {
				if (primaryAxis.VisualRange.CorrectionMode == RangeCorrectionMode.Auto || secondaryAxes.VisualRange.CorrectionMode == RangeCorrectionMode.Values)
					return;
				double minOffsetInPercent = (primaryAxis.VisualRange.Min - primaryAxis.WholeRange.Min) / primaryAxis.WholeRange.Delta;
				double maxOffsetInPercent = (primaryAxis.VisualRange.Max - primaryAxis.WholeRange.Min) / primaryAxis.WholeRange.Delta;
				double minOffset = secondaryAxes.WholeRange.Delta * minOffsetInPercent;
				double maxOffset = secondaryAxes.WholeRange.Delta * maxOffsetInPercent;
				double newMin = secondaryAxes.WholeRange.Min + minOffset;
				double newMax = secondaryAxes.WholeRange.Min + maxOffset;
				AxisScaleTypeMap map = secondaryAxes.AxisScaleTypeMap;
				secondaryAxes.VisualRange.UpdateRange(map.InternalToNative(newMin), map.InternalToNative(newMax), newMin, newMax);
				if (secondaryAxes is IAxisData) {
					RangeHelper.ThrowRangeData(secondaryAxes, secondaryAxes.WholeRange, secondaryAxes.ScrollingRange);
					RangeHelper.ThrowRangeData(secondaryAxes, secondaryAxes.VisualRange, secondaryAxes.Range);
				}
			}
			public override string ToString() {
				if (axes.Count > 0)
					return "main axis: " + axes[0];
				return "empty";
			}
		}
		readonly List<IPaneAxesContainer> containers;
		protected List<IPaneAxesContainer> Containers { get { return containers; } }
		public RangeUpdateNonDependentEnqueuer(RefinedSeriesGroupController groupController, IList<IAxisRangeData> rangesForReset, VisualRangeUpdateMode visualRangeUpdateMode, List<IPaneAxesContainer> containers)
			: base(groupController, rangesForReset, visualRangeUpdateMode) {
			this.containers = containers;
		}
		void CreateOrAddGroup(IAxisData pAxis, IList<IAxisData> sAxes, Dictionary<IAxisData, UpdateGroup> groups, List<IAxisData> forUpdate) {
			UpdateGroup group;
			if (groups.ContainsKey(pAxis)) {
				group = groups[pAxis];
			}
			else {
				group = new UpdateGroup();
				groups.Add(pAxis, group);
				forUpdate.Add(pAxis);
				group.SetPrimaryAxis(pAxis);
			}
			if (sAxes != null)
				foreach (IAxisData axis in sAxes)
					if (!forUpdate.Contains(axis)) {
						forUpdate.Add(axis);
						group.AddSecondaryAxis(axis);
					}
		}
		protected Dictionary<IAxisData, UpdateGroup> CreateUpdateGroups(IList<IPaneAxesContainer> containers) {
			Dictionary<IAxisData, UpdateGroup> groups = new Dictionary<IAxisData, UpdateGroup>();
			List<IAxisData> forUpdate = new List<IAxisData>();
			foreach (PaneAxesContainer paneContainer in containers) {
				CreateOrAddGroup(paneContainer.PrimaryAxisX, paneContainer.SecondaryAxesX, groups, forUpdate);
				CreateOrAddGroup(paneContainer.PrimaryAxisY, paneContainer.SecondaryAxesY, groups, forUpdate);
			}
			foreach (RefinedSeriesGroup refinedSeriesGroup in RefinedSeriesGroups.Values) {
				IAxisData axis = refinedSeriesGroup.GroupKey.AxisData;
				if (!forUpdate.Contains(axis))
					CreateOrAddGroup(axis, null, groups, forUpdate);
			}
			return groups;
		}
		protected override void Update() {
			Dictionary<IAxisData, UpdateGroup> groups = CreateUpdateGroups(Containers);
			foreach (UpdateGroup axisGroup in groups.Values)
				axisGroup.Update(this as IRangeUpdater);
		}
	}
	public class RangeUpdateDependentEnqueuer : RangeUpdateNonDependentEnqueuer {
		class DependentPair : IRangeUpdater {
			readonly UpdateGroup followers;
			readonly UpdateGroup leaders;
			readonly Dictionary<IAxisData, RefinedSeriesGroup> refinedSeriesGroups;
			readonly IRangeUpdater provider;
			public UpdateGroup Followers { get { return followers; } }
			public UpdateGroup Leaders { get { return leaders; } }
			public DependentPair(UpdateGroup followers, UpdateGroup leaders, Dictionary<IAxisData, RefinedSeriesGroup> refinedSeriesGroups, IRangeUpdater provider) {
				this.followers = followers;
				this.leaders = leaders;
				this.refinedSeriesGroups = refinedSeriesGroups;
				this.provider = provider;
			}
			MinMaxValues GetMinMaxByFilteredPoints(RefinedSeriesGroup followerGroup, IAxisData leader) {
				RefinedSeriesGroup leaderGroup = null;
				if (refinedSeriesGroups.ContainsKey(leader))
					leaderGroup = refinedSeriesGroups[leader];
				if (leaderGroup != null && followerGroup != null)
					return followerGroup.GetMinMaxValuesFromSeries(leader.VisualRange, leaderGroup.RefinedSeries);
				return MinMaxValues.Empty;
			}
			public void Update() {
				leaders.Update(provider);
				followers.Update(this as IRangeUpdater);
			}
			#region IValueProvider
			MinMaxValues IRangeUpdater.GetInternalValues(IAxisData axis) {
				if (followers.Axes.Contains(axis)) {
					MinMaxValues range = MinMaxValues.Empty;
					IEnumerable<IAxisData> axes = leaders.Axes;
					if (refinedSeriesGroups.ContainsKey(axis)) {
						RefinedSeriesGroup followerGroup = refinedSeriesGroups[axis];
						foreach (IAxisData leader in leaders.Axes)
							range = range.Union(GetMinMaxByFilteredPoints(followerGroup, leader));
						if (range.HasValues)
							return range;
					}
				}
				return provider.GetInternalValues(axis);
			}
			void IRangeUpdater.UpdateRanges(IAxisData axis, MinMaxValues internalValue) {
				provider.UpdateRanges(axis, internalValue);
			}
			#endregion IValueProvider
			public override string ToString() {
				return "leaders: [" + leaders.ToString() + "] " + "followers: [" + followers.ToString() + "] ";
			}
		}
		public RangeUpdateDependentEnqueuer(RefinedSeriesGroupController groupController, IList<IAxisRangeData> rangesForReset, VisualRangeUpdateMode visualRangeUpdateMode, List<IPaneAxesContainer> containers)
			: base(groupController, rangesForReset, visualRangeUpdateMode, containers) { }
		protected override void Update() {
			Dictionary<IAxisData, UpdateGroup> groups = CreateUpdateGroups(Containers);
			List<DependentPair> pairs = new List<DependentPair>();
			foreach (PaneAxesContainer paneContainer in Containers) {
				UpdateGroup groupX = groups[paneContainer.PrimaryAxisX];
				UpdateGroup groupY = groups[paneContainer.PrimaryAxisY];
				DependentPair pair = CreateDependentPair(groupX, groupY);
				if (pair != null)
					pairs.Add(pair);
			}
			List<UpdateGroup> updated = new List<UpdateGroup>();
			foreach (DependentPair pair in pairs) {
				pair.Update();
				updated.Add(pair.Followers);
				updated.Add(pair.Leaders);
			}
			foreach (UpdateGroup axisGroup in groups.Values)
				if (!updated.Contains(axisGroup))
					axisGroup.Update(this);
		}
		DependentPair CreateDependentPair(UpdateGroup proportionsGroupX, UpdateGroup proportionsGroupY) {
			if (proportionsGroupX.Auto ^ proportionsGroupY.Auto) {
				if (proportionsGroupX.Auto)
					return new DependentPair(proportionsGroupX, proportionsGroupY, RefinedSeriesGroups, this as IRangeUpdater);
				else
					return new DependentPair(proportionsGroupY, proportionsGroupX, RefinedSeriesGroups, this as IRangeUpdater);
			}
			return null;
		}
	}
	public interface IRangeUpdater {
		MinMaxValues GetInternalValues(IAxisData axis);
		void UpdateRanges(IAxisData axis, MinMaxValues intentalValues);
	}
}

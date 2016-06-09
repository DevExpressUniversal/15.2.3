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
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.Native {
	public abstract class AnimationEffectCalculatorBase {
		static List<AnimationEffectCalculatorBase> animationCalculatorList = new List<AnimationEffectCalculatorBase>();
		static AnimationEffectCalculatorBase() {
			animationCalculatorList.Add(new AnimationEffectSwitchViewCalculator());
			animationCalculatorList.Add(new AnimationEffectSwitchResourceCalculator());
			animationCalculatorList.Add(new AnimationEffectDayViewCalculator());
			animationCalculatorList.Add(new AnimationEffectWorkWeekViewCalculator());
			animationCalculatorList.Add(new AnimationEffectWeekViewCalculator());
			animationCalculatorList.Add(new AnimationEffectMonthCalculator());
			animationCalculatorList.Add(new AnimationEffectTimelineCalculator());
			animationCalculatorList.Add(new AnimationEffectGanttCalculator());
		}
		public static AnimationEffect Calculate(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			if (prevAnimationInfo == null || currentAnimationInfo == null)
				return null;
			int count = animationCalculatorList.Count;
			for (int i = 0; i < count; i++) {
				AnimationEffectCalculatorBase calculator = animationCalculatorList[i];
				if (calculator.CanCalculate(prevAnimationInfo, currentAnimationInfo))
					return calculator.CalculateEffect(control, prevAnimationInfo, currentAnimationInfo);
			}
			return null;
		}
		public abstract bool CanCalculate(SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo);
		public abstract AnimationEffect CalculateEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo);		
	}
	public class AnimationEffectSwitchViewCalculator : AnimationEffectCalculatorBase {
		public override bool CanCalculate(SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			if (prevAnimationInfo == null || currentAnimationInfo == null)
				return false;
			return prevAnimationInfo.ViewType != currentAnimationInfo.ViewType;
		}
		public override AnimationEffect CalculateEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			if (prevAnimationInfo == null || currentAnimationInfo == null)
				return null;
			if (prevAnimationInfo.ViewType != currentAnimationInfo.ViewType) {
				return new FadeEffect(control, prevAnimationInfo, currentAnimationInfo);
			}
			return null;
		}
	}
	public abstract class AnimationEffectSimpleFadeEffectCalculatorBase : AnimationEffectCalculatorBase {
		public override AnimationEffect CalculateEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			return new FadeEffect(control, prevAnimationInfo, currentAnimationInfo);
		}
	}
	public abstract class AnimationEffectViewBaseCalculatorBase : AnimationEffectCalculatorBase {
		public override AnimationEffect CalculateEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			bool isResourcesPerPageDiff = prevAnimationInfo.ResourcesPerPage != currentAnimationInfo.ResourcesPerPage;
			if (isResourcesPerPageDiff)
				return new FadeEffect(control, prevAnimationInfo, currentAnimationInfo);
			return null;
		}
	}
	public class AnimationEffectDayViewCalculator : AnimationEffectViewBaseCalculatorBase {
		public override bool CanCalculate(SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			if (prevAnimationInfo.ViewType != currentAnimationInfo.ViewType)
				return false;
			return currentAnimationInfo.ViewType == SchedulerViewType.Day;
		}
		public override AnimationEffect CalculateEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			AnimationEffect baseEffect = base.CalculateEffect(control, prevAnimationInfo, currentAnimationInfo);
			if (baseEffect != null)
				return baseEffect;
			int intervalDirection = Math.Sign(prevAnimationInfo.Interval.Start.Ticks - currentAnimationInfo.Interval.Start.Ticks);
			if (intervalDirection != 0)
				return CalculateIntervalChangeAnimationEffect(control, prevAnimationInfo, currentAnimationInfo, intervalDirection);
			DayViewAnimationInfo currentInfo = (DayViewAnimationInfo)currentAnimationInfo;
			DayViewAnimationInfo previousInfo = (DayViewAnimationInfo)prevAnimationInfo;
			int resourceIndexDirection = Math.Sign(previousInfo.FirstVisibleResourceIndex - currentInfo.FirstVisibleResourceIndex);
			if (resourceIndexDirection != 0 && currentInfo.GroupType != SchedulerGroupType.None)
				return CalculateVisibleResourceIndexChangeAnimationEffect(control, previousInfo, currentInfo, resourceIndexDirection);
			bool isDayCountDiff = previousInfo.DayCount != currentInfo.DayCount;
			bool isShowAllDayAreaDiff = previousInfo.ShowAllDayArea != currentInfo.ShowAllDayArea;
			bool isShowDayHeadersDiff = previousInfo.ShowDayHeaders != currentInfo.ShowDayHeaders;
			bool isShowWorkTimeOnlyDiff = previousInfo.ShowWorkTimeOnly != currentInfo.ShowWorkTimeOnly;
			bool isDiff = isDayCountDiff || isShowAllDayAreaDiff || isShowDayHeadersDiff || isShowWorkTimeOnlyDiff;
			if (isDiff)
				return new FadeEffect(control, prevAnimationInfo, currentAnimationInfo);
			return null;
		}
		AnimationEffect CalculateVisibleResourceIndexChangeAnimationEffect(SchedulerControl control, DayViewAnimationInfo previousInfo, DayViewAnimationInfo currentInfo, int resourceDirection) {
			if (currentInfo.GroupType == SchedulerGroupType.Date && currentInfo.FirstVisibleResourceIndex != previousInfo.FirstVisibleResourceIndex) {
				SlottedHorizontalScrollEffect effect = new SlottedHorizontalScrollEffect(control, previousInfo, currentInfo);
				List<Rectangle> slotsBounds = CalculateResourcesBoundsInGroupByDate(control);
				effect.SlotsBounds.AddRange(slotsBounds);
				effect.ScrollDelta = resourceDirection * CalculateScrollDeltaViaVisibleResourceIndex(control, previousInfo, currentInfo, slotsBounds[0].Width);
				effect.FadeOverlappedArea = true;
				return effect;
			}
			else if (currentInfo.GroupType == SchedulerGroupType.Resource && currentInfo.FirstVisibleResourceIndex != previousInfo.FirstVisibleResourceIndex) {
				if (control.ActiveView.ViewInfo.VisibleResources.Count <= 0)
					return null;
				SlottedHorizontalScrollEffect effect = new SlottedHorizontalScrollEffect(control, previousInfo, currentInfo);
				List<Rectangle> slotsBounds = CalculateSlotsBoundsOnResourceBasisWithHeaders(control);
				effect.SlotsBounds.AddRange(slotsBounds);
				effect.ScrollDelta = resourceDirection * slotsBounds[0].Width;
				return effect;
			}
			return null;
		}
		AnimationEffect CalculateIntervalChangeAnimationEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo, int intervalDirection) {
			if (currentAnimationInfo.GroupType == SchedulerGroupType.Resource && control.ActiveView.VisibleResources.Count > 0) {
				List<Rectangle> slotsBounds = CalculateSlotsBoundsOnResourceBasisWithoutHeaders(control);
				if (slotsBounds == null)
					return null;
				SlottedHorizontalScrollEffect effect = new SlottedHorizontalScrollEffect(control, prevAnimationInfo, currentAnimationInfo);
				effect.SlotsBounds.AddRange(slotsBounds);
				effect.ScrollDelta = intervalDirection * CalculateDayViewScrollDeltaViaVisibleInterval(control, prevAnimationInfo, currentAnimationInfo, slotsBounds[0].Width);
				effect.FadeOverlappedArea = true;
				return effect;
			}
			else if (currentAnimationInfo.GroupType == SchedulerGroupType.Date) {
				HorizontalScrollEffect effect = new HorizontalScrollEffect(control, prevAnimationInfo, currentAnimationInfo);
				effect.ScrollBounds = CalculateGroupByNoneBounds(control); ;
				effect.ScrollDelta = intervalDirection * CalculateDayViewScrollDeltaViaVisibleInterval(control, prevAnimationInfo, currentAnimationInfo, effect.ScrollBounds.Width);
				effect.FadeOverlappedArea = true;
				return effect;
			}
			else {
				if (control.ActiveView.ViewInfo.CellContainers.Count <= 0)
					return null;
				HorizontalScrollEffect effect = new HorizontalScrollEffect(control, prevAnimationInfo, currentAnimationInfo);
				effect.ScrollBounds = CalculateGroupByNoneBounds(control);
				effect.ScrollDelta = intervalDirection * CalculateDayViewScrollDeltaViaVisibleInterval(control, prevAnimationInfo, currentAnimationInfo, effect.ScrollBounds.Width);
				effect.FadeOverlappedArea = true;
				return effect;
			}
		}
		Rectangle CalculateGroupByNoneBounds(SchedulerControl control) {
			SchedulerViewBase view = control.ActiveView;
			int top = view.ViewInfo.Bounds.Top;
			SchedulerViewCellContainerCollection cellContainers = view.ViewInfo.CellContainers;
			Rectangle firstBounds = cellContainers[0].Bounds;
			Rectangle lastBounds = cellContainers[cellContainers.Count - 1].Bounds;
			return Rectangle.FromLTRB(firstBounds.Left + 1, top, lastBounds.Right, lastBounds.Bottom);
		}
		List<Rectangle> CalculateResourcesBoundsInGroupByDate(SchedulerControl control) {
			SchedulerViewBase view = control.ActiveView;
			int top = view.ViewInfo.ResourceHeaders[0].Bounds.Top;
			SchedulerViewCellContainerCollection cellContainers = view.ViewInfo.CellContainers;
			int cellContainerCount = cellContainers.Count;
			int previousContainerIndex = 0;
			TimeInterval currentInterval = cellContainers[0].Interval;
			List<Rectangle> containerBounds = new List<Rectangle>();
			for (int i = 0; i < cellContainerCount; i++) {
				SchedulerViewCellContainer container = cellContainers[i];
				if (currentInterval != container.Interval) {
					currentInterval = container.Interval;
					Rectangle firstBounds = cellContainers[previousContainerIndex].Bounds;
					Rectangle lastBounds = cellContainers[i - 1].Bounds;
					containerBounds.Add(Rectangle.FromLTRB(firstBounds.Left + 1, top, lastBounds.Right, lastBounds.Bottom));
					previousContainerIndex = i;
				}
			}
			Rectangle firstBounds1 = cellContainers[previousContainerIndex].Bounds;
			Rectangle lastBounds1 = cellContainers[cellContainerCount - 1].Bounds;
			containerBounds.Add(Rectangle.FromLTRB(firstBounds1.Left + 1, top, lastBounds1.Right, lastBounds1.Bottom));
			return containerBounds;
		}
		List<Rectangle> CalculateSlotsBoundsOnResourceBasisWithHeaders(SchedulerControl control) {
			return CalculateSlotsBoundsOnResourceBasis(control, control.ActiveView.ViewInfo.ResourceHeaders[0].Bounds.Top);
		}
		List<Rectangle> CalculateSlotsBoundsOnResourceBasisWithoutHeaders(SchedulerControl control) {
			return CalculateSlotsBoundsOnResourceBasis(control, control.ActiveView.ViewInfo.ResourceHeaders[0].Bounds.Bottom);
		}
		List<Rectangle> CalculateSlotsBoundsOnResourceBasis(SchedulerControl control, int top) {
			SchedulerViewBase view = control.ActiveView;
			SchedulerViewCellContainerCollection cellContainers = view.ViewInfo.CellContainers;
			int cellContainerCount = cellContainers.Count;
			Resource currentResource = null;
			int previousContainerIndex = 0;
			if (cellContainerCount <= 0)
				return null;
			currentResource = cellContainers[0].Resource;
			List<Rectangle> containerBounds = new List<Rectangle>();
			for (int i = 0; i < cellContainerCount; i++) {
				SchedulerViewCellContainer container = cellContainers[i];
				if (currentResource != container.Resource) {
					currentResource = container.Resource;
					Rectangle firstBounds = cellContainers[previousContainerIndex].Bounds;
					Rectangle lastBounds = cellContainers[i - 1].Bounds;
					containerBounds.Add(Rectangle.FromLTRB(firstBounds.Left + 1, top, lastBounds.Right, lastBounds.Bottom));
					previousContainerIndex = i;
				}
			}
			Rectangle firstBounds1 = cellContainers[previousContainerIndex].Bounds;
			Rectangle lastBounds1 = cellContainers[cellContainerCount - 1].Bounds;
			containerBounds.Add(Rectangle.FromLTRB(firstBounds1.Left + 1, top, lastBounds1.Right, lastBounds1.Bottom));
			return containerBounds;
		}
		int CalculateDayViewScrollDeltaViaVisibleInterval(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo, int scrollWidth) {
			DayView view = (DayView)control.ActiveView;
			int dayCount = view.GetVisibleIntervals().Count;
			TimeSpan delta = TimeSpan.FromTicks(Math.Abs(prevAnimationInfo.Interval.Start.Ticks - currentAnimationInfo.Interval.Start.Ticks));
			int deltaDays = (int)Math.Round(delta.TotalDays);
			return scrollWidth * Math.Max(1, Math.Min(dayCount, deltaDays)) / Math.Max(1, dayCount);
		}
		int CalculateScrollDeltaViaVisibleResourceIndex(SchedulerControl control, DayViewAnimationInfo prevAnimationInfo, DayViewAnimationInfo currentAnimationInfo, int scrollWidth) {
			DayView view = (DayView)control.ActiveView;
			int resourceCount = view.ResourcesPerPage;
			int delta = Math.Abs(prevAnimationInfo.FirstVisibleResourceIndex - currentAnimationInfo.FirstVisibleResourceIndex);
			return scrollWidth * Math.Max(1, Math.Min(resourceCount, delta)) / Math.Max(1, resourceCount);
		}
	}
	public class AnimationEffectWorkWeekViewCalculator : AnimationEffectDayViewCalculator {
		public override bool CanCalculate(SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			if (prevAnimationInfo.ViewType != currentAnimationInfo.ViewType)
				return false;
			return currentAnimationInfo.ViewType == SchedulerViewType.WorkWeek;
		}
		public override AnimationEffect CalculateEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			AnimationEffect effect = base.CalculateEffect(control, prevAnimationInfo, currentAnimationInfo);
			if (effect != null)
				return effect;
			WorkWeekViewAnimationInfo currentInfo = (WorkWeekViewAnimationInfo)currentAnimationInfo;
			WorkWeekViewAnimationInfo prevInfo = (WorkWeekViewAnimationInfo)prevAnimationInfo;
			if (prevInfo.Days != currentInfo.Days)
				return new FadeEffect(control, prevAnimationInfo, currentAnimationInfo);
			return null; ;
		}
	}
	public class AnimationEffectWeekViewCalculator : AnimationEffectViewBaseCalculatorBase {
		public override bool CanCalculate(SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			if (prevAnimationInfo.ViewType != currentAnimationInfo.ViewType)
				return false;
			return currentAnimationInfo is WeekViewAnimationInfo;
		}
		public override AnimationEffect CalculateEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			AnimationEffect baseEffect = base.CalculateEffect(control, prevAnimationInfo, currentAnimationInfo);
			if (baseEffect != null)
				return baseEffect;
			int direction = Math.Sign(prevAnimationInfo.Interval.Start.Ticks - currentAnimationInfo.Interval.Start.Ticks);
			if (direction != 0)
				return CalculateIntervalChangeAnimationEffect(control, prevAnimationInfo, currentAnimationInfo, direction);
			WeekViewAnimationInfo prevInfo = (WeekViewAnimationInfo)prevAnimationInfo;
			WeekViewAnimationInfo currentInfo = (WeekViewAnimationInfo)currentAnimationInfo;
			int resourceDirection = Math.Sign(prevInfo.FirstVisibleResourceIndex - currentInfo.FirstVisibleResourceIndex);
			if (resourceDirection != 0)
				return CalculateVisibleResouceIndexChangeAnimationEffect(control, prevInfo, currentInfo, resourceDirection);
			return null;
		}
		AnimationEffect CalculateVisibleResouceIndexChangeAnimationEffect(SchedulerControl control, WeekViewAnimationInfo prevInfo, WeekViewAnimationInfo currentInfo, int direction) {
			if (direction == 0)
				return null;
			if (control.GroupType == SchedulerGroupType.Date) {
				VerticalScrollEffect effect = new VerticalScrollEffect(control, prevInfo, currentInfo);
				effect.ScrollBounds = CalculateBoundsWhenLeftPlacedResources(control);
				effect.ScrollDelta = direction * CalculateDayViewScrollDeltaViaVisibleResourceIndex(control, prevInfo, currentInfo, effect.ScrollBounds.Height);
				return effect;
			}
			if (control.GroupType == SchedulerGroupType.Resource && control.ActiveView.VisibleResources.Count > 0) {
				HorizontalScrollEffect effect = new HorizontalScrollEffect(control, prevInfo, currentInfo);
				Rectangle totalBounds = CalculateBoundsWhenTopPlacedResources(control);
				effect.ScrollBounds = totalBounds;
				effect.ScrollDelta = direction * CalculateDayViewScrollDeltaViaVisibleResourceIndex(control, prevInfo, currentInfo, totalBounds.Width);
				return effect;
			}
			return null;
		}
		AnimationEffect CalculateIntervalChangeAnimationEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo, int direction) {
			if (direction == 0)
				return null;
			if (control.GroupType == SchedulerGroupType.Date) {
				HorizontalScrollEffect effect = new HorizontalScrollEffect(control, prevAnimationInfo, currentAnimationInfo);
				effect.ScrollBounds = CalculateBounds(control);
				effect.ScrollDelta = direction * effect.ScrollBounds.Width;
				return effect;
			}
			else {
				VerticalScrollEffect effect = new VerticalScrollEffect(control, prevAnimationInfo, currentAnimationInfo);
				effect.ScrollBounds = CalculateBounds(control);
				effect.ScrollDelta = direction * CalculateDayViewScrollDeltaViaVisibleInterval(control, prevAnimationInfo, currentAnimationInfo, effect.ScrollBounds.Height);
				return effect;
			}
		}
		Rectangle CalculateBounds(SchedulerControl control) {
			WeekView view = (WeekView)control.ActiveView;
			SchedulerViewCellContainerCollection cellContainers = view.ViewInfo.CellContainers;
			Rectangle firstBounds = cellContainers[0].Bounds;
			Rectangle lastBounds = cellContainers[cellContainers.Count - 1].Bounds;
			return Rectangle.FromLTRB(firstBounds.Left + 1, firstBounds.Top, lastBounds.Right, lastBounds.Bottom);
		}
		Rectangle CalculateBoundsWhenTopPlacedResources(SchedulerControl control) {
			WeekView view = (WeekView)control.ActiveView;
			SchedulerViewCellContainerCollection cellContainers = view.ViewInfo.CellContainers;
			Rectangle firstBounds = cellContainers[0].Bounds;
			Rectangle lastBounds = cellContainers[cellContainers.Count - 1].Bounds;
			int top = view.ViewInfo.ResourceHeaders[0].Bounds.Top;
			return Rectangle.FromLTRB(firstBounds.Left + 1, top, lastBounds.Right, lastBounds.Bottom);
		}
		Rectangle CalculateBoundsWhenLeftPlacedResources(SchedulerControl control) {
			WeekView view = (WeekView)control.ActiveView;
			WeekViewInfo viewInfo = view.ViewInfo;
			SchedulerViewCellContainerCollection cellContainers = viewInfo.CellContainers;
			Rectangle firstBounds = cellContainers[0].Bounds;
			Rectangle lastBounds = cellContainers[cellContainers.Count - 1].Bounds;
			int left = viewInfo.ResourceHeaders[0].Bounds.Left;
			return Rectangle.FromLTRB(left + 1, firstBounds.Top, lastBounds.Right, lastBounds.Bottom);
		}
		int CalculateDayViewScrollDeltaViaVisibleInterval(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo, int scrollHeight) {
			WeekView view = (WeekView)control.ActiveView;
			int weekCount = view.GetVisibleIntervals().Count;
			TimeSpan delta = TimeSpan.FromTicks(Math.Abs(prevAnimationInfo.Interval.Start.Ticks - currentAnimationInfo.Interval.Start.Ticks));
			int deltaWeeks = (int)Math.Round(delta.TotalDays / 7);
			return scrollHeight * Math.Max(1, Math.Min(weekCount, deltaWeeks)) / Math.Max(1, weekCount);
		}
		int CalculateDayViewScrollDeltaViaVisibleResourceIndex(SchedulerControl control, WeekViewAnimationInfo prevInfo, WeekViewAnimationInfo currentInfo, int scrollHeight) {
			WeekView view = (WeekView)control.ActiveView;
			int resourceCount = view.ResourcesPerPage;
			int delta = Math.Abs(prevInfo.FirstVisibleResourceIndex - currentInfo.FirstVisibleResourceIndex);
			return scrollHeight * Math.Max(1, Math.Min(resourceCount, delta)) / Math.Max(1, resourceCount);
		}
	}
	public class AnimationEffectMonthCalculator : AnimationEffectWeekViewCalculator {
		public override bool CanCalculate(SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			if (prevAnimationInfo.ViewType != currentAnimationInfo.ViewType)
				return false;
			return currentAnimationInfo is MonthViewAnimationInfo;
		}
		public override AnimationEffect CalculateEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			AnimationEffect effect = base.CalculateEffect(control, prevAnimationInfo, currentAnimationInfo);
			if (effect != null)
				return effect;
			MonthViewAnimationInfo prevInfo = (MonthViewAnimationInfo)prevAnimationInfo;
			MonthViewAnimationInfo currentInfo = (MonthViewAnimationInfo)currentAnimationInfo;
			bool isWeekCountDiff = prevInfo.WeekCount != currentInfo.WeekCount;
			bool isCompressWeekendDiff = prevInfo.CompressWeekend != currentInfo.CompressWeekend;
			if (isWeekCountDiff || isCompressWeekendDiff)
				return new FadeEffect(control, prevAnimationInfo, currentAnimationInfo);
			return null;
		}
	}
	public class AnimationEffectTimelineCalculator : AnimationEffectViewBaseCalculatorBase {
		public override bool CanCalculate(SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			if (prevAnimationInfo.ViewType != currentAnimationInfo.ViewType)
				return false;
			return currentAnimationInfo.ViewType == SchedulerViewType.Timeline;
		}
		public override AnimationEffect CalculateEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			AnimationEffect baseEffect = base.CalculateEffect(control, prevAnimationInfo, currentAnimationInfo);
			if (baseEffect != null)
				return baseEffect;
			int intervalDirection = Math.Sign(prevAnimationInfo.Interval.Start.Ticks - currentAnimationInfo.Interval.Start.Ticks);
			if (intervalDirection != 0) 
				return CalculateVisibleIntervalChangeAnimationEffect(control, prevAnimationInfo, currentAnimationInfo, intervalDirection);
			TimelineViewAnimationInfo currentInfo = (TimelineViewAnimationInfo)currentAnimationInfo;
			TimelineViewAnimationInfo previousInfo = (TimelineViewAnimationInfo)prevAnimationInfo;
			int resourceIndexDirection = Math.Sign(previousInfo.FirstVisibleResourceIndex - currentInfo.FirstVisibleResourceIndex);
			if (resourceIndexDirection != 0 && currentInfo.GroupType != SchedulerGroupType.None)
				return CalculateVisibleResourceIndexChangeAnimationEffect(control, previousInfo, currentInfo, resourceIndexDirection);
			return null;
		}
		AnimationEffect CalculateVisibleIntervalChangeAnimationEffect(SchedulerControl control, SchedulerAnimationInfo previousInfo, SchedulerAnimationInfo currentInfo, int direction) {
			if (direction == 0)
				return null;
			HorizontalScrollEffect effect = new HorizontalScrollEffect(control, previousInfo, currentInfo);
			effect.ScrollBounds = CalculateTimelineViewHorizontalTransitionBounds(control);
			effect.ScrollDelta = direction * CalculateTimelineViewScrollDeltaViaInterval(control, previousInfo, currentInfo, effect.ScrollBounds.Width);
			return effect;
		}
		protected virtual AnimationEffect CalculateVisibleResourceIndexChangeAnimationEffect(SchedulerControl control, TimelineViewAnimationInfo previousInfo, TimelineViewAnimationInfo currentInfo, int direction) {
			if (direction == 0)
				return null;
			VerticalScrollEffect effect = new VerticalScrollEffect(control, previousInfo, currentInfo);
			effect.ScrollBounds = CalculateTimelineViewVerticalTransitionBounds(control);
			effect.ScrollDelta = direction * CalculateTimelineViewScrollDeltaViaVisibleResourceIndex(control, previousInfo, currentInfo, effect.ScrollBounds.Height);
			return effect;
		}
		int CalculateTimelineViewScrollDeltaViaVisibleResourceIndex(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo, int scrollWidth) {
			TimelineView view = (TimelineView)control.ActiveView;
			int resourceCount = view.ResourcesPerPage;
			int delta = Math.Abs(prevAnimationInfo.FirstVisibleResourceIndex - currentAnimationInfo.FirstVisibleResourceIndex);
			return scrollWidth * Math.Max(1, Math.Min(resourceCount, delta)) / Math.Max(1, resourceCount);
		}
		int CalculateTimelineViewScrollDeltaViaInterval(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo, int scrollWidth) {
			TimelineView view = (TimelineView)control.ActiveView;
			TimeSpan delta = TimeSpan.FromTicks(Math.Abs(prevAnimationInfo.Interval.Start.Ticks - currentAnimationInfo.Interval.Start.Ticks));
			if (delta >= view.GetVisibleIntervals().Interval.Duration)
				return scrollWidth;
			TimeScale scale = view.GetBaseTimeScale();
			DateTime start = Algorithms.Min(prevAnimationInfo.Interval.Start, currentAnimationInfo.Interval.Start);
			DateTime end = Algorithms.Max(prevAnimationInfo.Interval.Start, currentAnimationInfo.Interval.Start);
			DateTime currentDate = start;
			int result = 0;
			for (; ; ) {
				if (currentDate >= end)
					break;
				result += scale.Width;
				currentDate = scale.GetNextDate(currentDate);
			}
			return Math.Min(scrollWidth, result);
		}
		Rectangle CalculateTimelineViewHorizontalTransitionBounds(SchedulerControl control) {
			SchedulerViewBase view = control.ActiveView;
			int top = view.ViewInfo.Bounds.Top;
			SchedulerViewCellContainerCollection cellContainers = view.ViewInfo.CellContainers;
			Rectangle firstBounds = cellContainers[0].Bounds;
			Rectangle lastBounds = cellContainers[cellContainers.Count - 1].Bounds;
			return Rectangle.FromLTRB(firstBounds.Left + 1, top, lastBounds.Right, lastBounds.Bottom);
		}
		Rectangle CalculateTimelineViewVerticalTransitionBounds(SchedulerControl control) {
			SchedulerViewBase view = control.ActiveView;
			SchedulerViewInfoBase viewInfo = view.ViewInfo;
			SchedulerViewCellContainerCollection cellContainers = viewInfo.CellContainers;
			Rectangle firstBounds = cellContainers[0].Bounds;
			Rectangle lastBounds = cellContainers[cellContainers.Count - 1].Bounds;
			int top = firstBounds.Top;
			int left = firstBounds.Left;
			if (viewInfo.ResourceHeaders.Count > 0)
				left = viewInfo.ResourceHeaders[0].Bounds.Left;
			return Rectangle.FromLTRB(left + 1, top, lastBounds.Right, lastBounds.Bottom);
		}
	}
	public class AnimationEffectGanttCalculator : AnimationEffectTimelineCalculator {
		public override bool CanCalculate(SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			if (prevAnimationInfo.ViewType != currentAnimationInfo.ViewType)
				return false;
			return currentAnimationInfo.ViewType == SchedulerViewType.Gantt;
		}
		protected override AnimationEffect CalculateVisibleResourceIndexChangeAnimationEffect(SchedulerControl control, TimelineViewAnimationInfo previousInfo, TimelineViewAnimationInfo currentInfo, int direction) {
			return null;
		}
	}
	public class AnimationEffectSwitchResourceCalculator : AnimationEffectSimpleFadeEffectCalculatorBase {
		public override bool CanCalculate(SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			return prevAnimationInfo.GroupType != currentAnimationInfo.GroupType;
		}
	}
}

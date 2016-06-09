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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class TimelineViewGroupHeadersLayoutCalculator : VerticalHeadersSupportedLayoutCalculator {
		TimeInterval now;
		IHeaderCaptionService captionFormatProvider;
		IHeaderToolTipService toolTipFormatProvider;
		protected TimelineViewGroupHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
			TimeZoneHelper timeZoneEngine = ViewInfo.View.Control.InnerControl.TimeZoneHelper;
			this.now = new TimeInterval(timeZoneEngine.ToClientTime(GetSystemNowTime(), TimeZoneInfo.Local.Id), TimeSpan.Zero);
			this.captionFormatProvider = (IHeaderCaptionService)viewInfo.View.Control.GetService(typeof(IHeaderCaptionService));
			this.toolTipFormatProvider = (IHeaderToolTipService)viewInfo.View.Control.GetService(typeof(IHeaderToolTipService));
		}
		#region Properties
		protected internal new TimelineView View { get { return (TimelineView)base.View; } }
		protected internal new TimelineViewInfo ViewInfo { get { return (TimelineViewInfo)base.ViewInfo; } }
		protected TimeInterval Now { get { return now; } }
		protected IHeaderCaptionService CaptionFormatProvider { get { return captionFormatProvider; } }
		protected IHeaderToolTipService ToolTipFormatProvider { get { return toolTipFormatProvider; } }
		#endregion
		public virtual int CalculateResourceHeadersWidht(Rectangle bounds) {
			return 0;
		}
		protected internal override void CalculatePreliminaryLayout() {
			TimeScaleLevelCollection levels = CalculateScaleLevels();
			ViewInfo.PreliminaryLayoutResult.Levels.AddRange(levels);
			CreateHeaderLevels(levels);
			base.CalculatePreliminaryLayout();
		}
		protected internal virtual TimeScaleLevelCollection CalculateScaleLevels() {
			TimeScaleLevelsCalculator calc = new TimeScaleLevelsCalculator();
			TimeIntervalCollection baseLevelIntervals = calc.CalculateBaseLevelIntervals(ViewInfo.Scales, View.InnerVisibleIntervals.Start, ViewInfo.PreliminaryLayoutResult.VisibleIntervalsCount);
			FillVisibleIntervals(ViewInfo, baseLevelIntervals);
			return calc.Calculate(ViewInfo.Scales, View.InnerVisibleIntervals.Start, ViewInfo.PreliminaryLayoutResult.VisibleIntervalsCount);
		}
		protected internal virtual void FillVisibleIntervals(TimelineViewInfo viewInfo, TimeIntervalCollection timeIntervals) {
			TimeIntervalCollection visibleIntervals = viewInfo.VisibleIntervals;
			visibleIntervals.BeginUpdate();
			visibleIntervals.Clear();
			visibleIntervals.SuspendSort();
			try {
				int count = timeIntervals.Count;
				for (int i = 0; i < count; i++) {
					visibleIntervals.Add(timeIntervals[i].Clone());
				}
			} finally {
				visibleIntervals.ResumeSort();
				visibleIntervals.EndUpdate();
			}
			View.InnerView.ApplyLimitInterval(View.Control.Selection);
		}
		protected virtual DateTime GetSystemNowTime() {
			return DateTime.Now;
		}
		protected internal virtual void PerformScaleHeadersLayout(Rectangle bounds) {
			CalculateBaseLevel(TopLevelHeaders, bounds, ViewInfo.PreliminaryLayoutResult.Levels);
			CalculateUpperLevels(TopLevelHeaders, bounds, ViewInfo.PreliminaryLayoutResult.UpperLevels);
			SchedulerHeaderLevelCollection headerLevels = new SchedulerHeaderLevelCollection();
			headerLevels.AddRange(ViewInfo.PreliminaryLayoutResult.UpperLevels);
			headerLevels.Add(ViewInfo.PreliminaryLayoutResult.BaseLevel);
			PerformHeaderLevelsLayout(headerLevels, bounds.Top);
			ViewInfo.ScaleLevels.AddRange(headerLevels);
		}
		protected void CreateHeaderLevels(TimeScaleLevelCollection levels) {
			ViewInfo.PreliminaryLayoutResult.BaseLevel = CreateBaseLevel(levels);
			ViewInfo.PreliminaryLayoutResult.UpperLevels.AddRange(CreateUpperLevels(levels));
		}
		protected internal virtual SchedulerHeaderLevelCollection CreateUpperLevels(TimeScaleLevelCollection levels) {
			SchedulerHeaderLevelCollection result = new SchedulerHeaderLevelCollection();
			for (int i = levels.Count - 2; i >= 0; i--) {
				SchedulerHeaderCollection upperHeaders = CreateLevelHeaders(levels[i], false);
				SchedulerHeaderLevel headerLevel = new SchedulerHeaderLevel();
				headerLevel.Headers.AddRange(upperHeaders);
				result.Insert(0, headerLevel);
			}
			return result;
		}
		protected internal virtual void PerformHeaderLevelsLayout(SchedulerHeaderLevelCollection levels, int top) {
			int count = levels.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeaderCollection levelHeaders = levels[i].Headers;
				ArrangeLevelHeadersVertically(levelHeaders, top);
				bool visible = (i == count - 1) ? ViewInfo.BaseScale.Visible : true;
				PerformLevelHeadersLayout(levelHeaders, visible);
				top = levelHeaders[0].Bounds.Bottom;
				if (visible)
					top -= Painter.VerticalOverlap;
			}
		}
		protected internal virtual void SetBaseHeadersInitialBounds(SchedulerHeaderCollection headers, Rectangle bounds, int width) {
			Rectangle[] rects = CalculateBaseHeadersInitialBounds(headers.Count, bounds, width);
			SetInitialHeadersBounds(headers, rects);
		}
		protected internal virtual void SetUpperHeadersInitialBounds(SchedulerHeaderCollection headers, SchedulerHeaderCollection baseHeaders, Rectangle bounds) {
			Rectangle[] rects = CalculateUpperHeadersInitialBounds(headers, baseHeaders, bounds.Top, bounds.Height);
			SetInitialHeadersBounds(headers, rects);
		}
		protected internal virtual void ArrangeLevelHeadersVertically(SchedulerHeaderCollection headers, int top) {
			for (int i = 0; i < headers.Count; i++) {
				Rectangle rect = headers[i].Bounds;
				rect.Y = top;
				headers[i].Bounds = rect;
			}
		}
		protected internal virtual void PerformLevelHeadersLayout(SchedulerHeaderCollection headers, bool visible) {
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = CalculateHeadersPreliminaryLayout(headers);
			CalculateInitialHeadersAnchorBounds(headers, true, true);
			int height = visible ? CalculateHeadersHeight(preliminaryResults) : 0;
			AssignHeadersHeight(headers, height);
			ApplyHeaderSeparators(headers);
			CalcFinalLayout(headers, preliminaryResults);
		}
		protected internal virtual SchedulerHeaderCollection CreateLevelHeaders(TimeScaleLevel level, bool baseLevel) {
			SchedulerHeaderCollection headers = new SchedulerHeaderCollection();
			int count = level.Intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeScaleHeader header = CreateTimeScaleHeader(level.Scale, level.Intervals[i], baseLevel);
				headers.Add(header);
			}
			return headers;
		}
		protected internal virtual TimeScaleHeader CreateTimeScaleHeader(TimeScale scale, TimeInterval interval, bool hasBottomBorder) {
			TimeScaleHeader header = new TimeScaleHeader(ViewInfo.TimelinePaintAppearance, scale);
			header.Interval = interval;
			header.Caption = CalculateHeaderCaption(header);
			header.ToolTipText = CalculateHeaderToolTip(header);
			header.Alternate = IsAlternateHeader(scale, interval.Start);
			header.HasTopBorder = true;
			header.HasBottomBorder = hasBottomBorder;
			return header;
		}
		protected internal virtual string CalculateHeaderCaption(TimeScaleHeader header) {
			if (CaptionFormatProvider != null) {
				string format = CaptionFormatProvider.GetTimeScaleHeaderCaption(header);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, header.Interval.Start, header.Interval.End);
			}
			return header.Scale.FormatCaption(header.Interval.Start, header.Interval.End);
		}
		protected internal virtual string CalculateHeaderToolTip(TimeScaleHeader header) {
			if (ToolTipFormatProvider != null) {
				string format = ToolTipFormatProvider.GetTimeScaleHeaderToolTip(header);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, header.Interval.Start, header.Interval.End);
			}
			return header.Scale.FormatCaption(header.Interval.Start, header.Interval.End);
		}
		protected internal virtual Rectangle[] CalculateBaseHeadersInitialBounds(int count, Rectangle bounds, int width) {
			Rectangle[] result = new Rectangle[count];
			int left = bounds.Left;
			for (int i = 0; i < count; i++) {
				int right = left + width;
				result[i] = Rectangle.FromLTRB(left, bounds.Top, right, bounds.Bottom);
				left = Math.Min(right, bounds.Right);
			}
			return result;
		}
		protected internal virtual Rectangle[] CalculateUpperHeadersInitialBounds(SchedulerHeaderCollection upperHeaders, SchedulerHeaderCollection baseHeaders, int top, int height) {
			List<Rectangle> results = new List<Rectangle>();
			int left = baseHeaders[0].Bounds.Left;
			int startIndex = 0;
			for (int i = 0; i < upperHeaders.Count; i++) {
				DateTime end = upperHeaders[i].Interval.End;
				startIndex = FindHeaderIndexByDate(baseHeaders, end, startIndex);
				SchedulerHeader baseHeader = baseHeaders[startIndex];
				Rectangle baseBounds = baseHeader.Bounds;
				int right = baseBounds.Right;
				if (end < baseHeader.Interval.End)
					right = EstimateHeaderRight(end, baseHeader.Interval, baseBounds.Left, baseBounds.Right);
				Rectangle rect = Rectangle.FromLTRB(left, top, right, top + height);
				left = right;
				results.Add(rect);
			}
			XtraSchedulerDebug.Assert(results.Count == upperHeaders.Count);
			return results.ToArray();
		}
		protected int FindHeaderIndexByDate(SchedulerHeaderCollection headers, DateTime date, int startIndex) {
			for (int i = startIndex; i < headers.Count; i++) {
				if (headers[i].Interval.Contains(date))
					return i;
			}
			return headers.Count - 1;
		}
		protected internal virtual int EstimateHeaderRight(DateTime date, TimeInterval baseInterval, int baseLeft, int baseRight) {
			double ratio = Convert.ToDouble(date.Ticks - baseInterval.Start.Ticks) / Convert.ToDouble(baseInterval.End.Ticks - baseInterval.Start.Ticks);
			return baseLeft + Convert.ToInt32((baseRight - baseLeft) * ratio);
		}
		protected internal virtual bool IsAlternateHeader(TimeScale scale, DateTime start) {
			DateTime scaleStart = scale.Floor(start);
			TimeInterval interval = new TimeInterval(scaleStart, scale.GetNextDate(scaleStart));
			return View.HeaderAlternateEnabled && interval.IntersectsWithExcludingBounds(now);
		}
		SchedulerHeaderLevel CreateBaseLevel(TimeScaleLevelCollection levels) {
			SchedulerHeaderCollection baseHeaders = CreateLevelHeaders(levels[levels.Count - 1], true);
			SchedulerHeaderLevel result = new SchedulerHeaderLevel();
			result.Headers.AddRange(baseHeaders);
			return result;
		}
		void CalculateBaseLevel(SchedulerHeaderCollection baseHeaders, Rectangle bounds, TimeScaleLevelCollection levels) {
			SetBaseHeadersInitialBounds(baseHeaders, bounds, levels[levels.Count - 1].Scale.Width);
		}
		void CalculateUpperLevels(SchedulerHeaderCollection baseHeaders, Rectangle bounds, SchedulerHeaderLevelCollection upperLevels) {
			 SchedulerHeaderCollection lowerHeaders = baseHeaders;
			for (int i = upperLevels.Count - 1; i >= 0; i--) {
				SchedulerHeaderCollection upperHeaders = upperLevels[i].Headers;
				SetUpperHeadersInitialBounds(upperHeaders, lowerHeaders, bounds);
				lowerHeaders = upperHeaders;
			}
		}
	}
}

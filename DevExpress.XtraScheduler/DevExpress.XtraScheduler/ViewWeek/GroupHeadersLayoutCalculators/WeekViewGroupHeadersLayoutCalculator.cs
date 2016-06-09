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
using DevExpress.Utils.Text;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Implementation;
using DevExpress.XtraScheduler.Services.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class WeekViewGroupHeadersLayoutCalculator : VerticalHeadersSupportedLayoutCalculator {
		WeekBasedViewHeaderCaptionCalculator headerCaptionCalculator;
		protected WeekViewGroupHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
			this.headerCaptionCalculator = CreateHeaderCaptionCalculator();
			this.headerCaptionCalculator.CompressWeekend = ViewInfo.CompressWeekend;
		}
		protected internal new WeekViewInfo ViewInfo { get { return (WeekViewInfo)base.ViewInfo; } }
		protected internal WeekBasedViewHeaderCaptionCalculator HeaderCaptionCalculator { get { return headerCaptionCalculator; } internal set { headerCaptionCalculator = value; } }
		protected void CalculateDateHeadersBounds(Rectangle bounds, bool groupSeparatorBeforeHeaders) {
			SchedulerHeaderCollection dateHeaders = ViewInfo.PreliminaryLayoutResult.DateHeaders;
			CalculateInitialHeadersBounds(dateHeaders, bounds);
			CalculateInitialHeadersAnchorBounds(dateHeaders, groupSeparatorBeforeHeaders, false);
			CalculateWeekDayHeadersLayout(dateHeaders);
		}
		protected internal virtual DayOfWeek[] CalcWeekDays() {
			return DateTimeHelper.GetDaysOfWeek(ViewInfo.View.InnerVisibleIntervals.Start, ViewInfo.FirstDayOfWeek, ViewInfo.ShowWeekend, ViewInfo.CompressWeekend);
		}
		protected internal virtual SchedulerHeaderCollection CreateDateHeaders(Resource resource) {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			DayOfWeek[] weekDays = ViewInfo.GetActualWeekDays(CalcWeekDays());
			int count = weekDays.Length;
			if (count <= 0)
				return result;
			DateTime[] weekDates = DateTimeHelper.GetWeekDates(ViewInfo.View.InnerVisibleIntervals.Start, ViewInfo.FirstDayOfWeek, ViewInfo.CompressWeekend, ViewInfo.ShowWeekend);
			if (weekDates.Length != count)
				weekDates = null;
			for (int i = 0; i < count; i++) {
				DayOfWeekHeader header = CreateHeader(ViewInfo.PaintAppearance, weekDays[i]);
				header.HasTopBorder = true;
				header.HasBottomBorder = true;
				header.Resource = resource;
				if (weekDates != null)
					header.Interval = new TimeInterval(weekDates[i], TimeSpan.FromDays(1));
				HeaderCaptionCalculator.CalculateFullHeaderCaption(header);
				result.Add(header);
			}
			return result;
		}
		protected virtual DayOfWeekHeader CreateHeader(WeekViewAppearance appearance, DayOfWeek dayOfWeek) {
			return new DayOfWeekHeader(appearance, dayOfWeek);
		}
		protected internal virtual void CalculateHeadersCaptions(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			CalculateFullHeadersCaptions(headers);
			CalculateShouldShowToolTip(headers, preliminaryResults);
			if ((ShouldUseAbbreviatedCaptions(headers, preliminaryResults)))
				CalculateAbbreviatedHeadersCaptions(headers);
			CalculateHeadersToolTips(headers);
		}
		protected internal virtual WeekBasedViewHeaderCaptionCalculator CreateHeaderCaptionCalculator() {
			WeekBasedViewHeaderCaptionCalculator calculator = new WeekBasedViewOptimalHeaderCaptionCalculator();
			calculator.CompressWeekend = ViewInfo.CompressWeekend;
			IHeaderCaptionService headerCaptionService = (IHeaderCaptionService)View.Control.GetService(typeof(IHeaderCaptionService));
			IHeaderToolTipService toolTipProvider = (IHeaderToolTipService)View.Control.GetService(typeof(IHeaderToolTipService));
			if (headerCaptionService == null && toolTipProvider == null)
				return calculator;
			if (headerCaptionService == null)
				headerCaptionService = new HeaderCaptionService();
			if (toolTipProvider == null)
				toolTipProvider = new HeaderToolTipService();
			HeaderCaptionFormatProviderBase captionProvider = new HeaderCaptionFormatProvider(headerCaptionService);
			WeekViewBasedFixedFormatHeaderCaptionCalculator result = new WeekViewBasedFixedFormatHeaderCaptionCalculator(captionProvider, toolTipProvider, calculator);
			result.CompressWeekend = ViewInfo.CompressWeekend;
			return result;
		}
		protected internal virtual void CalculateHeadersToolTips(SchedulerHeaderCollection headers) {
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				HeaderCaptionCalculator.CalculateHeaderToolTip((DayOfWeekHeader)headers[i]);
		}
		protected internal virtual void CalculateShouldShowToolTip(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				HeaderCaptionCalculator.CalculateShouldShowToolTip((DayOfWeekHeader)headers[i], Cache.Graphics, preliminaryResults[i].TextSize);
		}
		protected internal virtual void CalculateFullHeadersCaptions(SchedulerHeaderCollection headers) {
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				HeaderCaptionCalculator.CalculateFullHeaderCaption((DayOfWeekHeader)headers[i]);
		}
		protected internal virtual bool ShouldUseAbbreviatedCaptions(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			int totalWidth = 0;
			int totalSpaceWidth = 0;
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = headers[i];
				int availableTextWidth = preliminaryResults[i].TextSize.Width;
				totalWidth += Math.Max(availableTextWidth, CalculateHeaderCaptionWidth(header));
				totalSpaceWidth += availableTextWidth;
			}
			return totalWidth > totalSpaceWidth;
		}
		protected internal virtual int CalculateHeaderCaptionWidth(SchedulerHeader header) {
			DevExpress.Utils.Text.StringInfo stringInfo = StringPainter.Default.Calculate(Cache.Graphics, header.CaptionAppearance, header.Caption, Int32.MaxValue, Cache.Paint);
			return stringInfo.Bounds.Width;
		}
		protected internal virtual void CalculateAbbreviatedHeadersCaptions(SchedulerHeaderCollection headers) {
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				HeaderCaptionCalculator.CalculateAbbreviatedHeaderCaption((DayOfWeekHeader)headers[i]);
		}
		protected internal virtual void CalculateWeekDayHeadersLayout(SchedulerHeaderCollection headers) {
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = CalculateHeadersPreliminaryLayout(headers);
			CalculateHeadersCaptions(headers, preliminaryResults);
			int headerHeight = CalculateHeadersHeight(preliminaryResults);
			AssignHeadersHeight(headers, headerHeight);
			CalcFinalLayout(headers, preliminaryResults);
		}
	}
}

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
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class DayViewGroupHeadersLayoutCalculator : SchedulerViewHeadersLayoutCalculator {
		DayViewHeaderCaptionCalculator headerCaptionCalculator;
		protected DayViewGroupHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
			this.headerCaptionCalculator = CreateHeaderCaptionCalculator();
		}
		#region Properties
		protected internal new DayView View { get { return (DayView)base.View; } }
		protected internal new DayViewInfo ViewInfo { get { return (DayViewInfo)base.ViewInfo; } }
		protected internal virtual int GroupSeparatorsCount { get { return 0; } }
		protected internal abstract bool ShowTopLevelHeaders { get; }
		protected internal virtual bool ShowSubLevelHeaders { get { return false; } }
		protected internal abstract SchedulerHeaderPreliminaryLayoutResultCollection TopLevelHeadersPreliminaryResult { get; set; }
		protected internal virtual SchedulerHeaderPreliminaryLayoutResultCollection SubLevelHeadersPreliminaryResult {
			get { return new SchedulerHeaderPreliminaryLayoutResultCollection(); }
			set {  }
		}
		protected internal abstract int TopLevelHeadersHeight { get; set; }
		protected internal virtual int SubLevelHeadersHeight { get { return 0; } set { } }
		protected internal DayViewHeaderCaptionCalculator HeaderCaptionCalculator { get { return headerCaptionCalculator; } }
		#endregion
		public override void CalcLayout(Rectangle bounds) {
			ViewInfo.ResourceHeaders.Clear();
			ViewInfo.GroupSeparators.Clear();
			ViewInfo.DayHeaders.Clear();
			DayViewPreliminaryLayoutResult preliminaryResult = ViewInfo.PreliminaryLayoutResult;
			SchedulerHeaderCollection groupSeparators = preliminaryResult.GroupSeparators;
			SchedulerHeaderCollection resourceHeaders = preliminaryResult.ResourceHeaders;
			SchedulerHeaderCollection dateHeaders = preliminaryResult.DateHeaders;
			CalculateBounds(bounds);
			CalculateDateHeadersLayout(dateHeaders);
			CalculateGroupSeparatorsLayout(groupSeparators);
			CalculateResourceHeadersLayout(resourceHeaders);
			CacheDateHeadersSkinElementInfos(dateHeaders);
			CacheResourceHeadersSkinElementInfos(resourceHeaders);
			ViewInfo.ResourceHeaders.AddRange(resourceHeaders);
			ViewInfo.GroupSeparators.AddRange(groupSeparators);
			ViewInfo.DayHeaders.AddRange(dateHeaders);
		}
		protected internal override SchedulerHeaderCollection CreateSubLevelHeaders() {
			SchedulerHeaderCollection subHeaders = new SchedulerHeaderCollection();
			for (int i = 0; i < TopLevelHeaders.Count; i++) {
				SchedulerHeaderCollection dateResourceHeaders = CreateSubHeadersByTopHeader(TopLevelHeaders[i]);
				ApplyHeaderSeparators(dateResourceHeaders);
				subHeaders.AddRange(dateResourceHeaders);
			}
			SetLeftAndRightBorders(subHeaders);
			SubLevelHeaders.AddRange(subHeaders);
			return subHeaders;
		}
		protected internal virtual SchedulerHeaderCollection CreateDateHeaders(Resource resource) {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			TimeIntervalCollection days = ViewInfo.VisibleIntervals;
			TimeZoneHelper timeZoneEngine = ViewInfo.View.Control.InnerControl.TimeZoneHelper;
			DateTime today = timeZoneEngine.ToClientTime(DateTime.Now).Date;
			int count = days.Count;
			if (count <= 0)
				return result;
			SchedulerViewBase view = ViewInfo.View;
			for (int i = 0; i < count; i++) {
				DayHeader header = new DayHeader();
				header.Interval = days[i];
				header.Alternate = view.HeaderAlternateEnabled && (days[i].Start == today);
				header.HasTopBorder = true;
				header.Resource = resource;
				result.Add(header);
			}
			return result;
		}
		protected internal virtual void CalculateDateHeadersLayout(SchedulerHeaderCollection headers) {
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = ViewInfo.PreliminaryLayoutResult.DateHeadersPreliminaryLayoutResult;
			CalculateDateHeadersCaptions(headers, preliminaryResults);
			AssignHeadersHeight(headers, ViewInfo.PreliminaryLayoutResult.DateHeadersHeight);
			CalcFinalLayout(headers, preliminaryResults);
		}
		protected internal virtual DayViewHeaderCaptionCalculator CreateHeaderCaptionCalculator() {
			DayViewHeaderCaptionCalculator calculator = new DayViewOptimalHeaderCaptionCalculator();
			IHeaderCaptionService captionProvider = (IHeaderCaptionService)View.Control.GetService(typeof(IHeaderCaptionService));
			IHeaderToolTipService toolTipProvider = (IHeaderToolTipService)View.Control.GetService(typeof(IHeaderToolTipService));
			if (captionProvider == null && toolTipProvider == null)
				return calculator;
			if (captionProvider == null)
				captionProvider = new HeaderCaptionService();
			if (toolTipProvider == null)
				toolTipProvider = new HeaderToolTipService();
			return new DayViewFixedFormatHeaderCaptionCalculator(captionProvider, toolTipProvider, calculator);
		}
		protected internal override void CalculateResourceHeadersLayout(SchedulerHeaderCollection headers) {
			if (headers.Count == 0)
				return;
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = ViewInfo.PreliminaryLayoutResult.ResourceHeadersPreliminaryLayoutResult;
			AssignHeadersHeight(headers, ViewInfo.PreliminaryLayoutResult.ResourceHeadersHeight);
			CalcFinalLayout(headers, preliminaryResults);
		}
		protected internal virtual void CalculateDateHeadersCaptions(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				DayHeader header = (DayHeader)headers[i];
				HeaderCaptionCalculator.CalculateOptimalHeaderCaption(header, Cache.Graphics, preliminaryResults[i].TextSize);
				HeaderCaptionCalculator.CalculateHeaderToolTip(header);
			}
		}
		protected internal override void CalculatePreliminaryLayout() {
			DayViewPreliminaryLayoutResult preliminaryResult = ViewInfo.PreliminaryLayoutResult;
			SchedulerHeaderCollection groupSeparators = CreateVerticalGroupSeparators(GroupSeparatorsCount);
			SchedulerHeaderCollection topLevelHeaders = CreateTopLevelHeaders();
			CreateTopLevelHeadersFixedCaptions(topLevelHeaders);
			SchedulerHeaderCollection subHeaders = CreateSubLevelHeaders();
			CreateSubHeadersFixedCaptions(subHeaders);
			preliminaryResult.GroupSeparators.AddRange(groupSeparators);
		}
		protected virtual void CalculateBounds(Rectangle bounds) {
			DayViewPreliminaryLayoutResult preliminaryResult = ViewInfo.PreliminaryLayoutResult;
			SchedulerHeaderCollection groupSeparators = preliminaryResult.GroupSeparators;
			int groupSeparatorWidth = groupSeparators.Count > 0 ? CalculateVerticalGroupSeparatorWidth() : 0;
			CalculateInitialHeadersBounds(TopLevelHeaders, bounds, groupSeparators, groupSeparatorWidth);
			CalculateInitialHeadersAnchorBounds(TopLevelHeaders, false, false);
			CreateTopLevelHeadersFixedCaptions(TopLevelHeaders);
			TopLevelHeadersPreliminaryResult = CalculateHeadersPreliminaryLayout(TopLevelHeaders);
			int topLevelHeadersHeight = ShowTopLevelHeaders ? CalculateHeadersHeight(TopLevelHeadersPreliminaryResult) : 0;
			TopLevelHeadersHeight = topLevelHeadersHeight;
			preliminaryResult.GroupSeparatorsPreliminaryLayoutResult = CalculateHeadersPreliminaryLayout(groupSeparators);
			CalculateSubHeadres(TopLevelHeaders, TopLevelHeadersHeight);
			SubLevelHeadersPreliminaryResult = CalculateHeadersPreliminaryLayout(SubLevelHeaders);
			int subHeadersHeight = ShowSubLevelHeaders ? CalculateHeadersHeight(SubLevelHeadersPreliminaryResult) : 0;
			SubLevelHeadersHeight = subHeadersHeight;
			int totalHeadersHeight = topLevelHeadersHeight + subHeadersHeight;
			if (subHeadersHeight > 0)
				totalHeadersHeight -= Painter.VerticalOverlap;
			preliminaryResult.TotalHeadersHeight = totalHeadersHeight;
		}
		protected internal virtual void CreateDateHeadersFixedCaptions(SchedulerHeaderCollection headers) {
			IHeaderCaptionService captionProvider = (IHeaderCaptionService)View.Control.GetService(typeof(IHeaderCaptionService));
			if (captionProvider == null)
				return;
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				HeaderCaptionCalculator.CalculateFixedHeaderCaption((DayHeader)headers[i]);
		}
		protected internal abstract void CreateTopLevelHeadersFixedCaptions(SchedulerHeaderCollection headers);
		protected internal abstract void CreateSubHeadersFixedCaptions(SchedulerHeaderCollection headers);
		protected internal abstract void CacheDateHeadersSkinElementInfos(SchedulerHeaderCollection dateHeaders);
		protected abstract SchedulerHeaderCollection GetSubHeadersByTopHeader(SchedulerHeader topHeader);
		protected abstract SchedulerHeaderCollection CreateSubHeadersByTopHeader(SchedulerHeader topHeader);
		void CalculateSubHeadres(SchedulerHeaderCollection topHeaders, int topHeadersHeight) {
			int verticalOverlap = Painter.VerticalOverlap;
			for (int i = 0; i < topHeaders.Count; i++) {
				Rectangle bounds = topHeaders[i].Bounds;
				bounds.Y = bounds.Top + topHeadersHeight - verticalOverlap;
				SchedulerHeaderCollection subHeaders = GetSubHeadersByTopHeader(topHeaders[i]);
				CalculateInitialHeadersBounds(subHeaders, bounds);
				CalculateInitialHeadersAnchorBounds(subHeaders, i > 0, i < topHeaders.Count - 1);
			}
		}
	}
}

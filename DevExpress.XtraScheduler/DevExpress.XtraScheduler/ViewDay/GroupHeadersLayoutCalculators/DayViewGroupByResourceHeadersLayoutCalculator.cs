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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class DayViewGroupByResourceHeadersLayoutCalculator : DayViewGroupHeadersLayoutCalculator {
		public DayViewGroupByResourceHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
		#region Properties
		protected internal override int GroupSeparatorsCount { get { return ViewInfo.VisibleResources.Count - 1; } }
		protected internal override bool ShowTopLevelHeaders { get { return true; } }
		protected internal override bool ShowSubLevelHeaders { get { return View.ShowDayHeaders; } }
		protected override SchedulerHeaderCollection TopLevelHeaders { get { return ViewInfo.PreliminaryLayoutResult.ResourceHeaders; } }
		protected override SchedulerHeaderCollection SubLevelHeaders { get { return ViewInfo.PreliminaryLayoutResult.DateHeaders; } }
		protected internal override SchedulerHeaderPreliminaryLayoutResultCollection TopLevelHeadersPreliminaryResult {
			get { return ViewInfo.PreliminaryLayoutResult.ResourceHeadersPreliminaryLayoutResult; }
			set { ViewInfo.PreliminaryLayoutResult.ResourceHeadersPreliminaryLayoutResult = value; }
		}
		protected internal override SchedulerHeaderPreliminaryLayoutResultCollection SubLevelHeadersPreliminaryResult {
			get { return ViewInfo.PreliminaryLayoutResult.DateHeadersPreliminaryLayoutResult; }
			set { ViewInfo.PreliminaryLayoutResult.DateHeadersPreliminaryLayoutResult = value; }
		}
		protected internal override int TopLevelHeadersHeight {
			get { return ViewInfo.PreliminaryLayoutResult.ResourceHeadersHeight; }
			set { ViewInfo.PreliminaryLayoutResult.ResourceHeadersHeight = value; }
		}
		protected internal override int SubLevelHeadersHeight {
			get { return ViewInfo.PreliminaryLayoutResult.DateHeadersHeight; }
			set { ViewInfo.PreliminaryLayoutResult.DateHeadersHeight = value; }
		}
		#endregion
		protected internal override SchedulerHeaderCollection CreateTopLevelHeaders() {
			DayViewPreliminaryLayoutResult preliminaryResult = ViewInfo.PreliminaryLayoutResult;
			TimeInterval interval = new TimeInterval(ViewInfo.VisibleIntervals.Start, ViewInfo.VisibleIntervals.End);
			SchedulerHeaderCollection resourceHeaders = CreateHorizontalResourceHeaders(interval);
			SetLeftAndRightBorders(resourceHeaders);
			preliminaryResult.ResourceHeaders.AddRange(resourceHeaders);
			return resourceHeaders;
		}
		protected internal override void CreateTopLevelHeadersFixedCaptions(SchedulerHeaderCollection headers) {
		}
		protected internal override void CreateSubHeadersFixedCaptions(SchedulerHeaderCollection headers) {
			CreateDateHeadersFixedCaptions(headers);
		}
		protected override SchedulerHeaderCollection CreateSubHeadersByTopHeader(SchedulerHeader topHeader) {
			return CreateDateHeaders(topHeader.Resource);
		}
		protected override SchedulerHeaderCollection GetSubHeadersByTopHeader(SchedulerHeader topHeader) {
			SchedulerHeaderCollection subHeaders = new SchedulerHeaderCollection();
			subHeaders.AddRange(SubLevelHeaders.Where(h => h.Resource == topHeader.Resource));
			return subHeaders;
		}
		protected internal virtual int CalcDateHeaderResourceHeaderIndex(int dateHeaderIndex) {
			TimeIntervalCollection days = ViewInfo.VisibleIntervals;
			return dateHeaderIndex / Math.Max(1, days.Count);
		}
		protected internal override void CacheDateHeadersSkinElementInfos(SchedulerHeaderCollection dateHeaders) {
			CacheHeadersSkinElementInfos(dateHeaders, CalcDateHeaderResourceHeaderIndex);
		}
	}
}

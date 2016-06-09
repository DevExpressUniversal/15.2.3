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
using DevExpress.XtraScheduler.Internal.Implementations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class DayViewGroupByDateHeadersLayoutCalculator : DayViewGroupHeadersLayoutCalculator {
		public DayViewGroupByDateHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
		#region Properties
		protected internal override int GroupSeparatorsCount { get { return ViewInfo.VisibleIntervals.Count - 1; } }
		protected internal override bool ShowSubLevelHeaders { get { return true; } }
		protected internal override bool ShowTopLevelHeaders { get { return View.ShowDayHeaders; } }
		protected override SchedulerHeaderCollection TopLevelHeaders { get { return ViewInfo.PreliminaryLayoutResult.DateHeaders; } }
		protected override SchedulerHeaderCollection SubLevelHeaders { get { return ViewInfo.PreliminaryLayoutResult.ResourceHeaders; } }
		protected internal override SchedulerHeaderPreliminaryLayoutResultCollection TopLevelHeadersPreliminaryResult {
			get { return ViewInfo.PreliminaryLayoutResult.DateHeadersPreliminaryLayoutResult; }
			set { ViewInfo.PreliminaryLayoutResult.DateHeadersPreliminaryLayoutResult = value; }
		}
		protected internal override SchedulerHeaderPreliminaryLayoutResultCollection SubLevelHeadersPreliminaryResult {
			get { return ViewInfo.PreliminaryLayoutResult.ResourceHeadersPreliminaryLayoutResult; }
			set { ViewInfo.PreliminaryLayoutResult.ResourceHeadersPreliminaryLayoutResult = value; }
		}
		protected internal override int TopLevelHeadersHeight {
			get { return ViewInfo.PreliminaryLayoutResult.DateHeadersHeight; }
			set { ViewInfo.PreliminaryLayoutResult.DateHeadersHeight = value; }
		}
		protected internal override int SubLevelHeadersHeight {
			get { return ViewInfo.PreliminaryLayoutResult.ResourceHeadersHeight; }
			set { ViewInfo.PreliminaryLayoutResult.ResourceHeadersHeight = value; }
		}
		#endregion
		protected internal override SchedulerHeaderCollection CreateTopLevelHeaders() {
			SchedulerHeaderCollection dateHeaders = CreateDateHeaders(ResourceBase.Empty);
			SetLeftAndRightBorders(dateHeaders);
			ViewInfo.PreliminaryLayoutResult.DateHeaders.AddRange(dateHeaders);
			return dateHeaders;
		}
		protected internal override void CreateTopLevelHeadersFixedCaptions(SchedulerHeaderCollection headers) {
			CreateDateHeadersFixedCaptions(headers);
		}
		protected internal override void CreateSubHeadersFixedCaptions(SchedulerHeaderCollection headers) {
		}
		protected override SchedulerHeaderCollection CreateSubHeadersByTopHeader(SchedulerHeader topHeader) {
			return CreateHorizontalResourceHeaders(topHeader.Interval);
		}
		protected override SchedulerHeaderCollection GetSubHeadersByTopHeader(SchedulerHeader topHeader) {
			SchedulerHeaderCollection subHeaders = new SchedulerHeaderCollection();
			subHeaders.AddRange(SubLevelHeaders.Where(h => TimeInterval.Equals(h.Interval, topHeader.Interval)));
			return subHeaders;
		}
		protected internal override int CalcVisibleResourceIndexByResourceHeaderIndex(int headerIndex) {
			return headerIndex % ViewInfo.VisibleResources.Count;
		}
		protected internal override void CacheDateHeadersSkinElementInfos(SchedulerHeaderCollection dateHeaders) {
		}
	}
}

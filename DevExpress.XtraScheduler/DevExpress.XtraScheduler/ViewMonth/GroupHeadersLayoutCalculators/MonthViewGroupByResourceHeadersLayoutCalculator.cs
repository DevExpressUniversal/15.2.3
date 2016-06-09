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
using System.Linq;
using System.Drawing;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraScheduler.Drawing {
	public class MonthViewGroupByResourceHeadersLayoutCalculator : WeekViewGroupByResourceHeadersLayoutCalculator {
		public MonthViewGroupByResourceHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
		public override void CalcLayout(Rectangle bounds) {
			base.CalcLayout(bounds);
			CalculateDateHeaders();
			CalculateWeekDayHeadersLayout(SubLevelHeaders);
			CalcHeaderResourceIndexDelegate handler = delegate(int headerIndex) { return CalcWeekDayHeaderResourceHeaderIndex(headerIndex, SubLevelHeaders.Count / TopLevelHeaders.Count); };
			CacheHeadersSkinElementInfos(SubLevelHeaders, handler);
			ViewInfo.WeekDaysHeaders.AddRange(SubLevelHeaders);
		}
		protected internal virtual int CalcWeekDayHeaderResourceHeaderIndex(int weekDayHeaderIndex, int weekDayCount) {
			return weekDayHeaderIndex / Math.Max(1, weekDayCount);
		}
		protected internal override SchedulerHeaderCollection CreateSubLevelHeaders() {
			return CreateDateHeadersByResourceHeaders(TopLevelHeaders);
		}
		protected internal virtual SchedulerHeaderCollection CreateDateHeadersByResourceHeaders(SchedulerHeaderCollection resourceHeaders) {
			SchedulerHeaderCollection dayHeaders = new SchedulerHeaderCollection();
			int count = resourceHeaders.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeaderCollection resourceDateHeaders = CreateDateHeaders(resourceHeaders[i].Resource);
				ApplyHeaderSeparators(resourceDateHeaders);
				dayHeaders.AddRange(resourceDateHeaders);
			}
			SetLeftAndRightBorders(dayHeaders);
			return dayHeaders;
		}
		void CalculateDateHeaders() {
			SchedulerHeaderCollection dayHeaders = SubLevelHeaders;
			SchedulerHeaderCollection resourceHeaders = TopLevelHeaders;
			int verticalOverlap = Painter.VerticalOverlap;
			for (int i = 0; i < resourceHeaders.Count; i++) {
				Rectangle bounds = resourceHeaders[i].Bounds;
				bounds.Y = bounds.Bottom - verticalOverlap;
				SchedulerHeaderCollection resourceDateHeaders = new SchedulerHeaderCollection();
				resourceDateHeaders.AddRange(dayHeaders.Where(rh => rh.Resource == resourceHeaders[i].Resource));
				CalculateInitialHeadersBounds(resourceDateHeaders, bounds);
				CalculateInitialHeadersAnchorBounds(resourceDateHeaders, i > 0, i < resourceHeaders.Count - 1);
			}
		}
		protected internal override SchedulerHeaderCollection CreateDateHeaders(Resource resource) {
			SchedulerHeaderCollection result = base.CreateDateHeaders(resource);
			ApplyHeaderSeparators(result);
			return result;
		}
	}
}

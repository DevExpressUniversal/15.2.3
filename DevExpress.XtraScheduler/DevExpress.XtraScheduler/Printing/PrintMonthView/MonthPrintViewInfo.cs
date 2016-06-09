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
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Printing.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class MonthPrintViewInfo : MonthViewInfo {
		ViewPart viewPart;
		GraphicsInfo gInfo;
		WeekDaysHelper weekDaysHelper;
		public MonthPrintViewInfo(MonthPrintView view, ViewPart part, GraphicsInfo gInfo)
			: base(view) {
			if (gInfo == null)
				Exceptions.ThrowArgumentNullException("gInfo");
			this.viewPart = part;
			this.gInfo = gInfo;
			this.weekDaysHelper = CreateWeekDaysHelper();
		}
		internal GraphicsInfo GInfo { get { return gInfo; } }
		internal ViewPart ViewPart { get { return viewPart; } set { viewPart = value; } }
		internal WeekDaysHelper WeekDaysHelper { get { return weekDaysHelper; } }
		protected override void OnCompressWeekendChanged() {
			base.OnCompressWeekendChanged();
			this.weekDaysHelper = CreateWeekDaysHelper();
		}
		protected internal override void ApplySelection(SchedulerViewCellContainer cellContainer) {
		}
		protected internal virtual WeekDaysHelper CreateWeekDaysHelper() {
			return new WeekDaysHelper(this, viewPart);
		}
		public override void CalcPreliminaryLayout() {
			CalcPreliminaryLayoutCore(gInfo.Cache);
		}
		public override void CalcFinalLayout() {
			CalcFinalLayoutCore(gInfo.Cache);
		}
		protected internal override DayOfWeek[] GetActualWeekDays(DayOfWeek[] dayOfWeek) {
			return weekDaysHelper.GetActualWeekDays(dayOfWeek);
		}
		protected internal override DateTime[] GetWeekDates(DateTime start) {
			DateTime[] weekDates = base.GetWeekDates(start);
			return weekDaysHelper.GetWeekDates(weekDates);
		}
		protected internal override MoreButton CreateMoreButton() {
			return new MoreItems();
		}
		protected internal override Size CalculateMoreButtonMinSize() {
			return MoreItems.CalculateMinSize(gInfo);
		}
		protected internal override bool ShouldHideCellContent(SchedulerViewCellBase cell) {
			MonthPrintView printView = (MonthPrintView)View;
			if (!printView.SingleMonthOnly)
				return false;
			DateTime cellStart = cell.Interval.Start;
			DateTime month = printView.Month;
			return cellStart.Month != month.Month || cellStart.Year != month.Year;
		}
		protected internal override void ExecuteNavigationButtonsLayoutCalculator(GraphicsCache cache) {
		}
	}
}

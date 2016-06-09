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
using System.ComponentModel;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.XtraScheduler.Native;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.Reporting {
	[DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxBitmap(typeof(XtraSchedulerReport), DevExpress.Utils.ControlConstants.BitmapPath + "monthview.bmp"),
	Description("A View component for a monthly (multi-week) style report.")
	]
	public class ReportMonthView : ReportWeekView {
		internal const bool DefaultExactlyOneMonth = true;		
		bool exactlyOneMonth;
		public ReportMonthView() {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportMonthViewAppearance"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ReportMonthViewAppearance Appearance { get { return (ReportMonthViewAppearance)base.Appearance; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportMonthViewExactlyOneMonth"),
#endif
DefaultValue(DefaultExactlyOneMonth), Category(SRCategoryNames.Layout)]
		public bool ExactlyOneMonth { get { return exactlyOneMonth; } set { exactlyOneMonth = value; } }
		[DefaultValue(DefaultVisibleIntervalCount), Category(SRCategoryNames.Layout), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new int VisibleWeekCount { get { return base.VisibleIntervalCount; } set { base.VisibleIntervalCount = value; } }
		internal override WeekControlDataType DataType { get { return WeekControlDataType.Months; } }
		internal override bool ActualExactlyOneMonth { get { return ExactlyOneMonth; } }
		#endregion
		protected internal override void Initialize() {
			base.Initialize();
			this.exactlyOneMonth = DefaultExactlyOneMonth;
		}		
		protected internal override TimeIntervalFormatType GetDefaultTimeIntervalFormatType() {
			return TimeIntervalFormatType.Monthly;
		}
		protected override BaseViewAppearance CreateAppearance() {
			return new ReportMonthViewAppearance();
		}
		protected internal override TimeIntervalCollection CalculateVisibleIntervals(TimeIntervalCollection adapterIntervals) {
			if (adapterIntervals.Count == 0)
				return adapterIntervals;			
			return GetMonthIntervals(adapterIntervals.Interval);			
		}
		protected virtual TimeIntervalCollection GetMonthIntervals(TimeInterval interval) {
			MonthIntervalCollection monthIntervals = new MonthIntervalCollection();
			monthIntervals.Add(interval);
			return monthIntervals;
		}
		protected internal override TimeIntervalCollection CreateFakeTimeIntervalsCore(DateTime date) {
			MonthIntervalCollection monthIntervals = new MonthIntervalCollection();
			monthIntervals.Add(new TimeInterval(date, TimeSpan.FromDays(1)));
			return monthIntervals;
		}
		protected internal override ViewPainterBase CreateViewPainter() {
			return new MonthViewPainterFlat();
		}
	}
}

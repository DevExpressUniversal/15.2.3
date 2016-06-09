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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Design;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class WorkdaysControl : ChartUserControl {
		AxisBase axis;
		IChartContainer chartContainer;
		public WorkdaysControl() {
			InitializeComponent();
		}
		public void Initialize(AxisBase axis) {
			this.axis = axis;
			this.chartContainer = ((IOwnedElement)axis).ChartContainer;
			ceWorkdaysOnly.Checked = axis.DateTimeScaleOptions.WorkdaysOnly;
			WorkdaysOptions options = axis.DateTimeScaleOptions.WorkdaysOptions;
			ceSunday.Checked = (options.Workdays & Weekday.Sunday) == Weekday.Sunday;
			ceMonday.Checked = (options.Workdays & Weekday.Monday) == Weekday.Monday;
			ceTuesday.Checked = (options.Workdays & Weekday.Tuesday) == Weekday.Tuesday;
			ceWednesday.Checked = (options.Workdays & Weekday.Wednesday) == Weekday.Wednesday;
			ceThursday.Checked = (options.Workdays & Weekday.Thursday) == Weekday.Thursday;
			ceFriday.Checked = (options.Workdays & Weekday.Friday) == Weekday.Friday;
			ceSaturday.Checked = (options.Workdays & Weekday.Saturday) == Weekday.Saturday;
			UpdateControls();
		}
		void UpdateControls() {
			bool workdaysOnly = axis.DateTimeScaleOptions.WorkdaysOnly;
			grWorkdays.Enabled = workdaysOnly;
			grKnownDays.Enabled = workdaysOnly;
		}
		void UpdateWorkdays(Weekday workday, bool set) {
			if (set)
				axis.DateTimeScaleOptions.WorkdaysOptions.Workdays |= workday;
			else
				axis.DateTimeScaleOptions.WorkdaysOptions.Workdays &= (Weekday)(Int32.MaxValue ^ (int)workday);
		}
		void ceWorkdaysOnly_CheckedChanged(object sender, EventArgs e) {
			axis.DateTimeScaleOptions.WorkdaysOnly = ceWorkdaysOnly.Checked;
			UpdateControls();
		}
		void ceSunday_CheckedChanged(object sender, EventArgs e) {
			UpdateWorkdays(Weekday.Sunday, ceSunday.Checked);
		}
		void ceMonday_CheckedChanged(object sender, EventArgs e) {
			UpdateWorkdays(Weekday.Monday, ceMonday.Checked);
		}
		void ceTuesday_CheckedChanged(object sender, EventArgs e) {
			UpdateWorkdays(Weekday.Tuesday, ceTuesday.Checked);
		}
		void ceWednesday_CheckedChanged(object sender, EventArgs e) {
			UpdateWorkdays(Weekday.Wednesday, ceWednesday.Checked);
		}
		void ceThursday_CheckedChanged(object sender, EventArgs e) {
			UpdateWorkdays(Weekday.Thursday, ceThursday.Checked);
		}
		void ceFriday_CheckedChanged(object sender, EventArgs e) {
			UpdateWorkdays(Weekday.Friday, ceFriday.Checked);
		}
		void ceSaturday_CheckedChanged(object sender, EventArgs e) {
			UpdateWorkdays(Weekday.Saturday, ceSaturday.Checked);
		}
		void btnHolidays_ButtonClick(object sender, ButtonPressedEventArgs e) {
			using (HolidaysCollectionForm form = new HolidaysCollectionForm(axis.DateTimeScaleOptions.WorkdaysOptions.Holidays, chartContainer, true))
				form.ShowDialog();
		}
		void btnExactWorkdays_ButtonClick(object sender, ButtonPressedEventArgs e) {
			using (HolidaysCollectionForm form = new HolidaysCollectionForm(axis.DateTimeScaleOptions.WorkdaysOptions.ExactWorkdays, chartContainer, false))
				form.ShowDialog();
		}
	}
}

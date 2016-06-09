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
using System.Windows;
using DevExpress.XtraScheduler;
using System.Collections;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Drawing;
using System.ComponentModel;
namespace DevExpress.Xpf.Scheduler.UI {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public partial class YearlyRecurrenceControl : RecurrenceControlBase {
		public static readonly DependencyProperty RecurrenceInfoProperty;
		WeekOfMonth actualWeekOfMonth = WeekOfMonth.First;
		WeekOfMonth unboundWeekOfMonth = RecurrenceInfo.DefaultWeekOfMonth;
		static YearlyRecurrenceControl() {
			RecurrenceInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<YearlyRecurrenceControl, RecurrenceInfo>("RecurrenceInfo", null, (d, e) => d.OnRecurrenceInfoChanged(e.OldValue, e.NewValue), null);
		}
		public YearlyRecurrenceControl() {
			InitializeComponent();
			MainElement.DataContext = this;
		}
		private void OnRecurrenceInfoChanged(RecurrenceInfo oldRecInfo, RecurrenceInfo newRecInfo) {
			RaiseOnPropertyChange("WeekDays");
			RaiseOnPropertyChange("Periodicity");
			RaiseOnPropertyChange("DayNumber");
			RaiseOnPropertyChange("Month");
			RaiseOnPropertyChange("YearlyRecurrenceType");
			RaiseOnPropertyChange("WeekOfMonthNumber");
			if (newRecInfo != null)
				SetRecurrenceWeekOfMonthType(newRecInfo.WeekOfMonth);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("YearlyRecurrenceControlRecurrenceInfo")]
#endif
		public RecurrenceInfo RecurrenceInfo {
			get { return (RecurrenceInfo)GetValue(RecurrenceInfoProperty); }
			set { SetValue(RecurrenceInfoProperty, value); }
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("YearlyRecurrenceControlPeriodicity")]
#endif
		public int Periodicity {
			get { return RecurrenceInfo != null ? RecurrenceInfo.Periodicity : RecurrenceInfo.DefaultPeriodicity; }
			set {
				if (RecurrenceInfo == null) return;
				RecurrenceInfo.Periodicity = value;
				RaiseOnPropertyChange("Periodicity");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("YearlyRecurrenceControlWeekDays")]
#endif
		public WeekDays WeekDays {
			get { return RecurrenceInfo != null ? RecurrenceInfo.WeekDays : RecurrenceInfo.DefaultWeekDays; }
			set {
				if (RecurrenceInfo == null) return;
				RecurrenceInfo.WeekDays = value;
				RaiseOnPropertyChange("WeekDays");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("YearlyRecurrenceControlDayNumber")]
#endif
		public int DayNumber {
			get { return RecurrenceInfo != null ? RecurrenceInfo.DayNumber : 1; }
			set {
				if (RecurrenceInfo == null) return;
				RecurrenceInfo.DayNumber = value;
				RaiseOnPropertyChange("DayNumber");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("YearlyRecurrenceControlWeekOfMonth")]
#endif
		public WeekOfMonth WeekOfMonth {
			get { return RecurrenceInfo != null ? RecurrenceInfo.WeekOfMonth : unboundWeekOfMonth; }
			set {
				if (RecurrenceInfo == null)
					unboundWeekOfMonth = value;
				else
					RecurrenceInfo.WeekOfMonth = value;
				RaiseOnPropertyChange("WeekOfMonth");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("YearlyRecurrenceControlMonth")]
#endif
		public int Month {
			get { return RecurrenceInfo != null ? RecurrenceInfo.Month : 1; }
			set {
				if (RecurrenceInfo == null) return;
				RecurrenceInfo.Month = value;
				RaiseOnPropertyChange("Month");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("YearlyRecurrenceControlWeekOfMonthNumber")]
#endif
		public WeekOfMonth WeekOfMonthNumber { get { return GetWeekOfMonthNumber(); } set { SetWeekOfMonthNumber(value); } }
		private void SetWeekOfMonthNumber(WeekOfMonth value) {
			actualWeekOfMonth = value;
			if (YearlyRecurrenceType != WeekOfMonth.None && WeekOfMonth != WeekOfMonth.None) {
				WeekOfMonth = value;
				RaiseOnPropertyChange("WeekOfMonthNumber");
			}
		}
		private WeekOfMonth GetWeekOfMonthNumber() {
			if (WeekOfMonth == WeekOfMonth.None)
				return WeekOfMonth.First;
			else {
				actualWeekOfMonth = WeekOfMonth;
				return actualWeekOfMonth;
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("YearlyRecurrenceControlYearlyRecurrenceTypes")]
#endif
		public IList YearlyRecurrenceTypes { get { return GetRecurrenceYearlyTypes(); } }
		private IList GetRecurrenceYearlyTypes() {
			NamedElementList result = new NamedElementList();
			result.Add(new NamedElement(WeekOfMonth.None, SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_DayNumber), this));
			result.Add(new NamedElement(WeekOfMonth.First, SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_DayOfWeek), this));
			return result;
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("YearlyRecurrenceControlYearlyRecurrenceType")]
#endif
		public WeekOfMonth YearlyRecurrenceType { get { return GetYearlyRecurrenceType(); } set { SetYearlyRecurrenceType(value); } }
		private WeekOfMonth GetYearlyRecurrenceType() {
			if (WeekOfMonth == WeekOfMonth.None)
				return WeekOfMonth.None;
			else
				return WeekOfMonth.First;
		}
		private void SetYearlyRecurrenceType(WeekOfMonth value) {
			WeekOfMonth = value;
			RaiseOnPropertyChange("YearlyRecurrenceType");
		}
		protected override void ValidateValues(XtraScheduler.UI.ValidationArgs args) {
			if (YearlyRecurrenceType == XtraScheduler.WeekOfMonth.First)
				return;
			SpinEdit spinYearlyDayNumber = FindElementByName("spinYearlyDayNumber") as SpinEdit;
			MonthEdit cbYearlyMonth = FindElementByName("cbYearlyMonth") as MonthEdit;
			if (spinYearlyDayNumber == null || cbYearlyMonth == null)
				return;
			Validator.ValidateMonthDayNumber(args, spinYearlyDayNumber, spinYearlyDayNumber.EditValue, cbYearlyMonth.Month);
		}
		protected internal virtual void SetRecurrenceWeekOfMonthType(WeekOfMonth type) {
			if (type == XtraScheduler.WeekOfMonth.None)
				MonthDayRadioButton.IsChecked = true;
			else
				WeekOfMonthRadioButton.IsChecked = true;
		}
		private void MonthDayRadioButton_Checked(object sender, RoutedEventArgs e) {
			RecurrenceInfo.WeekOfMonth = XtraScheduler.WeekOfMonth.None;
		}
		private void WeekOfMonthRadioButton_Checked(object sender, RoutedEventArgs e) {
			RecurrenceInfo.WeekOfMonth = weekOfMonthEdit.WeekOfMonth;
		}
		private void WeekOfMonthRadioButton_GotFocus(object sender, RoutedEventArgs e) {
			if (!IsReadOnly)
				WeekOfMonthRadioButton.IsChecked = true;
		}
		private void MonthDayRadioButton_GotFocus(object sender, RoutedEventArgs e) {
			if (!IsReadOnly)
				MonthDayRadioButton.IsChecked = true;
		}
	}
}

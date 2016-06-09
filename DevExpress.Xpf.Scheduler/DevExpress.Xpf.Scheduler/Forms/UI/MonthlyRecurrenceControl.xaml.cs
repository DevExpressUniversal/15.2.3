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
	public partial class MonthlyRecurrenceControl : RecurrenceControlBase {
		public static readonly DependencyProperty RecurrenceInfoProperty;
		WeekOfMonth actualWeekOfMonth = WeekOfMonth.First;
		static MonthlyRecurrenceControl() {
			RecurrenceInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<MonthlyRecurrenceControl, RecurrenceInfo>("RecurrenceInfo", null, (d, e) => d.OnRecurrenceInfoChanged(e.OldValue, e.NewValue), null);
		}
		public MonthlyRecurrenceControl() {
			InitializeComponent();
			MainElement.DataContext = this;
		}
		private void OnRecurrenceInfoChanged(RecurrenceInfo oldRecInfo, RecurrenceInfo newRecInfo) {
			RaiseOnPropertyChange("WeekDays");
			RaiseOnPropertyChange("Periodicity");
			RaiseOnPropertyChange("DayNumber");
			RaiseOnPropertyChange("MonthlyRecurrenceType");
			RaiseOnPropertyChange("WeekOfMonthNumber");
			if (newRecInfo != null)
				SetRecurrenceWeekOfMonthType(newRecInfo.WeekOfMonth);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("MonthlyRecurrenceControlRecurrenceInfo")]
#endif
		public RecurrenceInfo RecurrenceInfo {
			get { return (RecurrenceInfo)GetValue(RecurrenceInfoProperty); }
			set { SetValue(RecurrenceInfoProperty, value); }
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("MonthlyRecurrenceControlPeriodicity")]
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
	[DevExpressXpfSchedulerLocalizedDescription("MonthlyRecurrenceControlWeekDays")]
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
	[DevExpressXpfSchedulerLocalizedDescription("MonthlyRecurrenceControlDayNumber")]
#endif
		public int DayNumber {
			get { return RecurrenceInfo != null ? RecurrenceInfo.DayNumber : 1; }
			set {
				if (RecurrenceInfo == null)
					return;
				RecurrenceInfo.DayNumber = value;
				RaiseOnPropertyChange("DayNumber");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("MonthlyRecurrenceControlWeekOfMonth")]
#endif
		public WeekOfMonth WeekOfMonth {
			get { return RecurrenceInfo != null ? RecurrenceInfo.WeekOfMonth : RecurrenceInfo.DefaultWeekOfMonth; }
			set {
				if (RecurrenceInfo == null) return;
				RecurrenceInfo.WeekOfMonth = value;
				RaiseOnPropertyChange("WeekOfMonth");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("MonthlyRecurrenceControlWeekOfMonthNumber")]
#endif
		public WeekOfMonth WeekOfMonthNumber { get { return GetWeekOfMonthNumber(); } set { SetWeekOfMonthNumber(value); } }
		private void SetWeekOfMonthNumber(WeekOfMonth value) {
			actualWeekOfMonth = value;
			if (MonthlyRecurrenceType != WeekOfMonth.None && WeekOfMonth != WeekOfMonth.None) {
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
	[DevExpressXpfSchedulerLocalizedDescription("MonthlyRecurrenceControlMonthlyRecurrenceTypes")]
#endif
		public IList MonthlyRecurrenceTypes { get { return GetMonthlyRecurrenceTypes(); } }
		private IList GetMonthlyRecurrenceTypes() {
			NamedElementList result = new NamedElementList();
			result.Add(new NamedElement(WeekOfMonth.None, SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_DayNumber), this));
			result.Add(new NamedElement(WeekOfMonth.First, SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_DayOfWeek), this));
			return result;
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("MonthlyRecurrenceControlMonthlyRecurrenceType")]
#endif
		public WeekOfMonth MonthlyRecurrenceType { get { return GetMonthlyRecurrenceType(); } set { SetMonthlyRecurrenceType(value); } }
		private WeekOfMonth GetMonthlyRecurrenceType() {
			if (WeekOfMonth == WeekOfMonth.None)
				return WeekOfMonth.None;
			else
				return WeekOfMonth.First;
		}
		private void SetMonthlyRecurrenceType(WeekOfMonth value) {
			WeekOfMonth = value;
			RaiseOnPropertyChange("MonthlyRecurrenceType");
		}
		protected override void ValidateValues(XtraScheduler.UI.ValidationArgs args) {
			WeekOfMonth weekOfMonth = GetMonthlyRecurrenceType();
			if (weekOfMonth == XtraScheduler.WeekOfMonth.None) {
				SpinEdit spinMonthlyDayNumberCount = FindElementByName("spinMonthlyDayNumberCount") as SpinEdit;
				SpinEdit spinMonthlyDay = FindElementByName("spinMonthlyDay") as SpinEdit;
				if (spinMonthlyDayNumberCount == null || spinMonthlyDay == null)
					return;
				Validator.ValidateMonthCount(args, spinMonthlyDayNumberCount, spinMonthlyDayNumberCount.EditValue);
				Validator.ValidateDayNumber(args, spinMonthlyDay, spinMonthlyDay.EditValue, 31);
			}
			else {
				SpinEdit spinMonthlyDayNumberCount = FindElementByName("spinMonthlyDayNumberCount") as SpinEdit;
				if (spinMonthlyDayNumberCount == null)
					return;
				Validator.ValidateMonthCount(args, spinMonthlyDayNumberCount, spinMonthlyDayNumberCount.EditValue);
			}
		}
		protected override void CheckForWarnings(XtraScheduler.UI.ValidationArgs args) {
			if (Validator.NeedToCheckLargeDayNumberWarning(WeekOfMonth)) {
				SpinEdit spinMonthlyDay = FindElementByName("spinMonthlyDay") as SpinEdit;
				if (spinMonthlyDay == null)
					return;
				Validator.CheckLargeDayNumberWarning(args, spinMonthlyDay, spinMonthlyDay.EditValue);
			}
		}
		protected internal virtual void SetRecurrenceWeekOfMonthType(WeekOfMonth type) {
			if (type == XtraScheduler.WeekOfMonth.None)
				EveryNDayRadioButton.IsChecked = true;
			else
				EveryWeekOfMonthRadioButton.IsChecked = true;
		}
		private void EveryNDayRadioButton_Checked(object sender, RoutedEventArgs e) {
			RecurrenceInfo.WeekOfMonth = XtraScheduler.WeekOfMonth.None;
		}
		private void EveryWeekOfMonthRadioButton_Checked(object sender, RoutedEventArgs e) {
			RecurrenceInfo.WeekOfMonth = weekOfMonth.WeekOfMonth;
		}
		private void EveryNDayRadioButton_GotFocus(object sender, RoutedEventArgs e) {
			if (!IsReadOnly)
				EveryNDayRadioButton.IsChecked = true;
		}
		private void EveryWeekOfMonthRadioButton_GotFocus(object sender, RoutedEventArgs e) {
			if (!IsReadOnly)
				EveryWeekOfMonthRadioButton.IsChecked = true;
		}
	}
}

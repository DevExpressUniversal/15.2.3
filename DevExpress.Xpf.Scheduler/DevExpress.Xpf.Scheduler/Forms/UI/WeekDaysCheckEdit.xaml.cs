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
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using System.ComponentModel;
using System.Globalization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Scheduler.UI {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public partial class WeekDaysCheckEdit : UserControl, INotifyPropertyChanged {
		class RowColumnInfo {
			public RowColumnInfo(int row, int column) {
				Row = row;
				Column = column;
			}
			public int Row { get; set; }
			public int Column { get; set; }
		}
		static readonly List<RowColumnInfo> indexToPositionAssociation = new List<RowColumnInfo>();
		static WeekDaysCheckEdit() {
			InitializeIndexToPositionAssociation();
		}
		static void InitializeIndexToPositionAssociation() {
			IndexToPositionAssociation.Add(new RowColumnInfo(0, 0));
			IndexToPositionAssociation.Add(new RowColumnInfo(0, 1));
			IndexToPositionAssociation.Add(new RowColumnInfo(0, 2));
			IndexToPositionAssociation.Add(new RowColumnInfo(0, 3));
			IndexToPositionAssociation.Add(new RowColumnInfo(1, 0));
			IndexToPositionAssociation.Add(new RowColumnInfo(1, 1));
			IndexToPositionAssociation.Add(new RowColumnInfo(1, 2));
		}
		static List<RowColumnInfo> IndexToPositionAssociation { get { return indexToPositionAssociation; } }
		bool useAbbreviatedDayNames;
		readonly List<CheckEdit> chkBoxes = new List<CheckEdit>();
		public WeekDaysCheckEdit() {
			InitializeComponent();
			InitializeCheckBoxesAssociations();
			UpdateDayOfWeekCheckboxesPositions(FirstDayOfWeek);
		}
		#region Properties
		List<CheckEdit> CheckBoxes { get { return chkBoxes; } }
		#region FirstDayOfWeek
		public DayOfWeek FirstDayOfWeek {
			get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
			set { SetValue(FirstDayOfWeekProperty, value); }
		}
		public static readonly DependencyProperty FirstDayOfWeekProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekDaysCheckEdit, DayOfWeek>("FirstDayOfWeek", DayOfWeek.Saturday, (d, e) => d.OnFirstDayOfWeekChanged(e.OldValue, e.NewValue));
		void OnFirstDayOfWeekChanged(DayOfWeek oldValue, DayOfWeek newValue) {
			UpdateDayOfWeekCheckboxesPositions(newValue);
		}
		#endregion
		#region WeekDays
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditWeekDays")]
#endif
		public WeekDays WeekDays { get { return (WeekDays)GetValue(WeekDaysProperty); } set { SetValue(WeekDaysProperty, value); } }
		public static readonly DependencyProperty WeekDaysProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekDaysCheckEdit, WeekDays>("WeekDays", RecurrenceInfo.DefaultWeekDays, (d, e) => d.OnWeekDaysChanged(e.OldValue, e.NewValue), null);		
		void OnWeekDaysChanged(XtraScheduler.WeekDays oldWeekDays, XtraScheduler.WeekDays newWeekDays) {
			NotifyWeekDaysControlValues();
		}
		void NotifyWeekDaysControlValues() {
			RaiseOnPropertyChange("SundayValue");
			RaiseOnPropertyChange("MondayValue");
			RaiseOnPropertyChange("TuesdayValue");
			RaiseOnPropertyChange("WednesdayValue");
			RaiseOnPropertyChange("ThursdayValue");
			RaiseOnPropertyChange("FridayValue");
			RaiseOnPropertyChange("SaturdayValue");
		}
		#endregion
		#region UseAbbreviatedDayNames
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditUseAbbreviatedDayNames")]
#endif
		public bool UseAbbreviatedDayNames {
			get { return useAbbreviatedDayNames; }
			set {
				if (useAbbreviatedDayNames == value)
					return;
				useAbbreviatedDayNames = value;
				NotifyWeekDaysControlCaptions();
			}
		}
		void NotifyWeekDaysControlCaptions() {
			RaiseOnPropertyChange("SundayCaption");
			RaiseOnPropertyChange("MondayCaption");
			RaiseOnPropertyChange("TuesdayCaption");
			RaiseOnPropertyChange("WednesdayCaption");
			RaiseOnPropertyChange("ThursdayCaption");
			RaiseOnPropertyChange("FridayCaption");
			RaiseOnPropertyChange("SaturdayCaption");
		}
		#endregion
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditSundayCaption")]
#endif
		public string SundayCaption { get { return GetDayOfWeekCaption(DayOfWeek.Sunday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditMondayCaption")]
#endif
		public string MondayCaption { get { return GetDayOfWeekCaption(DayOfWeek.Monday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditTuesdayCaption")]
#endif
		public string TuesdayCaption { get { return GetDayOfWeekCaption(DayOfWeek.Tuesday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditWednesdayCaption")]
#endif
		public string WednesdayCaption { get { return GetDayOfWeekCaption(DayOfWeek.Wednesday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditThursdayCaption")]
#endif
		public string ThursdayCaption { get { return GetDayOfWeekCaption(DayOfWeek.Thursday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditFridayCaption")]
#endif
		public string FridayCaption { get { return GetDayOfWeekCaption(DayOfWeek.Friday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditSaturdayCaption")]
#endif
		public string SaturdayCaption { get { return GetDayOfWeekCaption(DayOfWeek.Saturday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditSundayValue")]
#endif
		public bool SundayValue { get { return GetWeekDay(WeekDays.Sunday); } set { SetWeekDay(value, WeekDays.Sunday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditMondayValue")]
#endif
		public bool MondayValue { get { return GetWeekDay(WeekDays.Monday); } set { SetWeekDay(value, WeekDays.Monday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditTuesdayValue")]
#endif
		public bool TuesdayValue { get { return GetWeekDay(WeekDays.Tuesday); } set { SetWeekDay(value, WeekDays.Tuesday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditWednesdayValue")]
#endif
		public bool WednesdayValue { get { return GetWeekDay(WeekDays.Wednesday); } set { SetWeekDay(value, WeekDays.Wednesday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditThursdayValue")]
#endif
		public bool ThursdayValue { get { return GetWeekDay(WeekDays.Thursday); } set { SetWeekDay(value, WeekDays.Thursday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditFridayValue")]
#endif
		public bool FridayValue { get { return GetWeekDay(WeekDays.Friday); } set { SetWeekDay(value, WeekDays.Friday); } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysCheckEditSaturdayValue")]
#endif
		public bool SaturdayValue { get { return GetWeekDay(WeekDays.Saturday); } set { SetWeekDay(value, WeekDays.Saturday); } }
		#endregion
		void InitializeCheckBoxesAssociations() {
			CheckBoxes.Add(chkSun);
			CheckBoxes.Add(chkMon);
			CheckBoxes.Add(chkTue);
			CheckBoxes.Add(chkWed);
			CheckBoxes.Add(chkThu);
			CheckBoxes.Add(chkFri);
			CheckBoxes.Add(chkSat);
		}
		void UpdateDayOfWeekCheckboxesPositions(DayOfWeek firstDayOfWeek) {
			int dayOfWeekIndex = (int)firstDayOfWeek;
			const int totalDayOfWeekCount = 7;
			for (int checkBoxPositionIndex = 0; checkBoxPositionIndex < totalDayOfWeekCount; checkBoxPositionIndex++) {
				CheckEdit edit = CheckBoxes[dayOfWeekIndex];
				RowColumnInfo info = IndexToPositionAssociation[checkBoxPositionIndex];
				Grid.SetColumn(edit, info.Column);
				Grid.SetRow(edit, info.Row);
				dayOfWeekIndex++;
				dayOfWeekIndex %= totalDayOfWeekCount;
			}
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected virtual void RaiseOnPropertyChange(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		protected virtual string GetDayOfWeekCaption(DayOfWeek dayOfWeek) {
			DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentUIDateTimeFormat;
			return useAbbreviatedDayNames ? dtfi.GetAbbreviatedDayName(dayOfWeek) : dtfi.GetDayName(dayOfWeek);
		}
		protected virtual bool GetWeekDay(WeekDays day) {
			return (WeekDays & day) == day;
		}
		protected virtual void SetWeekDay(bool value, WeekDays day) {
			if (value)
				WeekDays |= day;
			else
				WeekDays &= (~day);
		}
	}
}

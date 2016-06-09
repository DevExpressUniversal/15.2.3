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
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Drawing;
namespace DevExpress.Xpf.Scheduler.UI {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public partial class DailyRecurrenceControl : RecurrenceControlBase {
		public static readonly DependencyProperty RecurrenceInfoProperty;
		static DailyRecurrenceControl() {
			RecurrenceInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DailyRecurrenceControl, RecurrenceInfo>("RecurrenceInfo", null, (d, e) => d.OnRecurrenceInfoChanged(e.OldValue, e.NewValue), null);
		}
		public DailyRecurrenceControl() {
			InitializeComponent();
			MainElement.DataContext = this;
		}
		void RecurrenceInfo_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			RaiseOnPropertyChange("WeekDays");
			RaiseOnPropertyChange("Periodicity");
			RaiseOnPropertyChange("RecurrenceStart");
			RaiseOnPropertyChange("RecurrenceEnd");
			RaiseOnPropertyChange("RecurrenceRange");
			RaiseOnPropertyChange("OccurenceCount");
			RecurrenceInfo.PropertyChanged += new PropertyChangedEventHandler(RecurrenceInfo_PropertyChanged);
		}
		private void OnRecurrenceInfoChanged(RecurrenceInfo oldRecInfo, RecurrenceInfo newRecInfo) {
			RaiseOnPropertyChange("WeekDays");
			RaiseOnPropertyChange("Periodicity");
			RaiseOnPropertyChange("RecurrenceStart");
			RaiseOnPropertyChange("RecurrenceEnd");
			RaiseOnPropertyChange("RecurrenceRange");
			RaiseOnPropertyChange("OccurenceCount");
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DailyRecurrenceControlRecurrenceInfo")]
#endif
		public RecurrenceInfo RecurrenceInfo {
			get { return (RecurrenceInfo)GetValue(RecurrenceInfoProperty); }
			set { SetValue(RecurrenceInfoProperty, value); }
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DailyRecurrenceControlPeriodicity")]
#endif
		public int Periodicity {
			get { return RecurrenceInfo != null ? RecurrenceInfo.Periodicity : RecurrenceInfo.DefaultPeriodicity; }
			set {
				if (RecurrenceInfo == null)
					return;
				RecurrenceInfo.Periodicity = value;
				RaiseOnPropertyChange("WeekDays");
				RaiseOnPropertyChange("Periodicity");
				RaiseOnPropertyChange("RecurrenceStart");
				RaiseOnPropertyChange("RecurrenceEnd");
				RaiseOnPropertyChange("RecurrenceRange");
				RaiseOnPropertyChange("OccurenceCount");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DailyRecurrenceControlWeekDays")]
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
	[DevExpressXpfSchedulerLocalizedDescription("DailyRecurrenceControlDailyRecurrenceTypes")]
#endif
		public IList DailyRecurrenceTypes { get { return GetDailyRecurrenceTypes(); } }
		private IList GetDailyRecurrenceTypes() {
			NamedElementList result = new NamedElementList();
			result.Add(new NamedElement(WeekDays.EveryDay, SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_Every), this));
			result.Add(new NamedElement(WeekDays.WorkDays, SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_EveryWeekday), this));
			return result;
		}
		protected override void ValidateValues(XtraScheduler.UI.ValidationArgs args) {
			if (WeekDays == XtraScheduler.WeekDays.WorkDays)
				return;
			SpinEdit spinDailyDaysCount = FindElementByName("spinDailyDaysCount") as SpinEdit;
			if (spinDailyDaysCount == null)
				return;
			Validator.ValidateDayCount(args, spinDailyDaysCount, spinDailyDaysCount.EditValue);
		}
		private void spinDailyDaysCount_GotFocus(object sender, RoutedEventArgs e) {
			EveryNDayRadioButton.IsChecked = true;
		}
	}
	public class DailyWeekDaysTemplateSelector : DataTemplateSelector {
		public DataTemplate EveryNDayTemplate { get; set; }
		public DataTemplate WeekDaysTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			NamedElement el = item as NamedElement;
			if (el == null)
				return null;
			WeekDays val = (WeekDays)el.Id;
			switch (val) {
				case WeekDays.EveryDay:
					return EveryNDayTemplate;
				case WeekDays.WorkDays:
					return WeekDaysTemplate;
			}
			return null;
		}
	}
}

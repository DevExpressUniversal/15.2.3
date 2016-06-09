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
using DevExpress.XtraScheduler.UI;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using System.ComponentModel;
namespace DevExpress.Xpf.Scheduler.UI {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public partial class WeeklyRecurrenceControl : RecurrenceControlBase {
		public WeeklyRecurrenceControl() {
			InitializeComponent();
		}
		#region RecurrenceInfo
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeeklyRecurrenceControlRecurrenceInfo")]
#endif
		public RecurrenceInfo RecurrenceInfo {
			get { return (RecurrenceInfo)GetValue(RecurrenceInfoProperty); }
			set { SetValue(RecurrenceInfoProperty, value); }
		}
		public static readonly DependencyProperty RecurrenceInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeeklyRecurrenceControl, RecurrenceInfo>("RecurrenceInfo", null, (d, e) => d.OnRecurrenceInfoChanged(e.OldValue, e.NewValue), null);
		void OnRecurrenceInfoChanged(RecurrenceInfo oldRecInfo, RecurrenceInfo newRecInfo) {
			RaiseOnPropertyChange("WeekDays");
			RaiseOnPropertyChange("Periodicity");
		}
		#endregion
		#region Periodicity
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeeklyRecurrenceControlPeriodicity")]
#endif
		public int Periodicity {
			get { return RecurrenceInfo != null ? RecurrenceInfo.Periodicity : RecurrenceInfo.DefaultPeriodicity; }
			set {
				if (RecurrenceInfo == null) return;
				RecurrenceInfo.Periodicity = value;
				RaiseOnPropertyChange("Periodicity");
			}
		}
		#endregion
		#region WeekDays
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeeklyRecurrenceControlWeekDays")]
#endif
		public WeekDays WeekDays {
			get { return RecurrenceInfo != null ? RecurrenceInfo.WeekDays : RecurrenceInfo.DefaultWeekDays; }
			set {
				if (RecurrenceInfo == null) return;
				RecurrenceInfo.WeekDays = value;
				RaiseOnPropertyChange("WeekDays");
			}
		}
		#endregion
		#region FirstDayOfWeek
		public DayOfWeek FirstDayOfWeek {
			get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
			set { SetValue(FirstDayOfWeekProperty, value); }
		}
		public static readonly DependencyProperty FirstDayOfWeekProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeeklyRecurrenceControl, DayOfWeek>("FirstDayOfWeek", DayOfWeek.Sunday, null);
		#endregion
		protected override void ValidateValues(ValidationArgs args) {
			Validator.ValidateWeekCount(args, spinWeeklyWeeksCount, spinWeeklyWeeksCount.EditValue);
			Validator.ValidateDayOfWeek(args, weekDaysCheckEdit, this.WeekDays);
		}
		protected override void OnIsReadOnlyChanged(bool oldValue, bool newValue) {
			base.OnIsReadOnlyChanged(oldValue, newValue);
			spinWeeklyWeeksCount.IsReadOnly = newValue;
			brdWeekDays.IsHitTestVisible = !newValue;
		}
	}
}

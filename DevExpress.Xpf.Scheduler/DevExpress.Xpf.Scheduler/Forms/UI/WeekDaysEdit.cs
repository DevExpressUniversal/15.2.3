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
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler;
using System.Globalization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Drawing;
using System.Windows;
using System.ComponentModel;
namespace DevExpress.Xpf.Scheduler.UI {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public class WeekDaysEdit : FixedSourceComboBoxEdit {
		#region DayOfWeek
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekDaysEditDayOfWeek")]
#endif
public WeekDays DayOfWeek {
			get { return (WeekDays)GetValue(DayOfWeekProperty); }
			set { SetValue(DayOfWeekProperty, value); }
		}
		public static readonly DependencyProperty DayOfWeekProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekDaysEdit, WeekDays>("DayOfWeek", WeekDays.EveryDay, (d, e) => d.OnDayOfWeekChanged(e.OldValue, e.NewValue), null);
		void OnDayOfWeekChanged(WeekDays oldValue, WeekDays newValue) {
			SelectedItem = Settings.GetItemFromValue(newValue);
		}
		#endregion
		protected override Editors.Settings.BaseEditSettings CreateEditorSettings() {
			return new WeekDaysEditSettings();
		}
		protected override void OnSelectedItemChanged(object oldValue, object newValue) {
			base.OnSelectedItemChanged(oldValue, newValue);
			NamedElement element = newValue as NamedElement;
			if(element != null && element.Id != null && element.Id is WeekDays) 
				DayOfWeek = (WeekDays)element.Id;
		}
	}
	public class WeekDaysEditSettings : FixedSourceComboBoxEditSettings {
		protected override void PopulateItems() {
			Items.Clear();
			Items.Add(CreateItem(WeekDays.EveryDay));
			Items.Add(CreateItem(WeekDays.WorkDays));
			Items.Add(CreateItem(WeekDays.WeekendDays));
			Items.Add(CreateItem(WeekDays.Sunday));
			Items.Add(CreateItem(WeekDays.Monday));
			Items.Add(CreateItem(WeekDays.Tuesday));
			Items.Add(CreateItem(WeekDays.Wednesday));
			Items.Add(CreateItem(WeekDays.Thursday));
			Items.Add(CreateItem(WeekDays.Friday));
			Items.Add(CreateItem(WeekDays.Saturday));
		}
		protected NamedElement CreateItem(WeekDays val) {
			return new NamedElement(val, GetDisplayName(val));
		}
		protected string GetDisplayName(WeekDays val) {
			if (val == WeekDays.EveryDay)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysEveryDay);
			else if (val == WeekDays.WeekendDays)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysWeekendDays);
			else if (val == WeekDays.WorkDays)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysWorkDays);
			else {
				DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentUIDateTimeFormat;
				DayOfWeek dayOfWeek = DateTimeHelper.ToDayOfWeek(val);
				return dtfi.GetDayName(dayOfWeek);
			}
		}
	}
}

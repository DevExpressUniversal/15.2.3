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

using System.ComponentModel;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraScheduler.Native {
	#region SchedulerViewPropertiesBase
	public class SchedulerViewPropertiesBase : SchedulerPropertiesBase, ISchedulerViewPropertiesBase {
		internal const bool defaultEnabled = true;
		public const SchedulerGroupType defaultGroupType = SchedulerGroupType.None;
		bool enabled = defaultEnabled;
		SchedulerGroupType groupType = defaultGroupType;
		ISchedulerDeferredScrollingOption deferredScrolling;
		public SchedulerViewPropertiesBase() {
			DeferredScrolling = PropertyCreator.CreateDeferredScrollingOption();
		}
		#region Properties
		#region Enabled
		public bool Enabled {
			get { return enabled; }
			set {
				if (enabled == value)
					return;
				if (RaisePropertyChanging("Enabled")) {
					enabled = value;
					RaisePropertyChanged("Enabled", !value, value);
				}
			}
		}
		#endregion
		#region GroupType
		[DefaultValue(defaultGroupType), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerGroupType GroupType {
			get { return groupType; }
			set {
				if (groupType == value)
					return;
				SchedulerGroupType oldValue = groupType;
				groupType = value;
				RaisePropertyChanged("GroupType", oldValue, value);
			}
		}
		#endregion
		#region DeferredScrolling
		public ISchedulerDeferredScrollingOption DeferredScrolling {
			get { return deferredScrolling; }
			set {
				ISchedulerDeferredScrollingOption oldValue = DeferredScrolling;
				if (oldValue == value)
					return;
				UnsubscribeDeferredScrollingEvents(oldValue);
				deferredScrolling = value;
				SubscribeDeferredScrollingEvents(value);
				RaisePropertyChanged("DeferredScrolling", oldValue, value);
			}
		}
		void OnDeferredScrollingChanged(object sender, BaseOptionChangedEventArgs e) {
			RaisePropertyChanged("DeferredScrolling", DeferredScrolling, DeferredScrolling);
		}
		#endregion
		#endregion
		void SubscribeDeferredScrollingEvents(ISchedulerDeferredScrollingOption schedulerDeferredScrollingOption) {
			if (schedulerDeferredScrollingOption == null)
				return;
			schedulerDeferredScrollingOption.Changed += OnDeferredScrollingChanged;
		}
		void UnsubscribeDeferredScrollingEvents(ISchedulerDeferredScrollingOption schedulerDeferredScrollingOption) {
			if (schedulerDeferredScrollingOption == null)
				return;
			schedulerDeferredScrollingOption.Changed -= OnDeferredScrollingChanged;
		}
	}
	#endregion
	public class DayViewProperties : SchedulerViewPropertiesBase, IDayViewProperties {
	}
	public class WorkWeekViewProperties : DayViewProperties, IWorkWeekViewProperties {
	}
	public class TimelineViewProperties : SchedulerViewPropertiesBase, ITimelineViewProperties {
	}
	public class WeekViewProperties : SchedulerViewPropertiesBase, IWeekViewProperties {
	}
	public class MonthViewProperties : WeekViewProperties, IMonthViewProperties {
	}
	public class FullWeekViewProperties : DayViewProperties, IFullWeekViewProperties {
		public FullWeekViewProperties() {
			Enabled = false;
		}
	}
}

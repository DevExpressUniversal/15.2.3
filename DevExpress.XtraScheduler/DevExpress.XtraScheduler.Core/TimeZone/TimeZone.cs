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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
#if !SL
using DevExpress.Data.Helpers;
using Microsoft.Win32;
using System.Security;
#else
#endif
namespace DevExpress.XtraScheduler {
	internal static class TimeZoneId {
		public static readonly string Custom = ""; 
		public static readonly string Afghanistan = "Afghanistan Standard Time";
		public static readonly string Alaska = "Alaskan Standard Time";
		public static readonly string Arab = "Arab Standard Time";
		public static readonly string Arabian = "Arabian Standard Time";
		public static readonly string Arabic = "Arabic Standard Time";
		public static readonly string Argentina = "Argentina Standard Time";
		public static readonly string Armenia = "Armenian Standard Time";
		public static readonly string Atlantic = "Atlantic Standard Time";
		public static readonly string CentralAustralian = "AUS Central Standard Time";
		public static readonly string EasternAustralian = "AUS Eastern Standard Time";
		public static readonly string Azerbaijan = "Azerbaijan Standard Time";
		public static readonly string Azores = "Azores Standard Time";
		public static readonly string CentralCanadian = "Canada Central Standard Time";
		public static readonly string CapeVerde = "Cape Verde Standard Time";
		public static readonly string Caucasus = "Caucasus Standard Time";
		public static readonly string Adelaide = "Cen. Australia Standard Time";
		public static readonly string CentralAmerica = "Central America Standard Time";
		public static readonly string CentralAsia = "Central Asia Standard Time";
		public static readonly string CentralBrazilian = "Central Brazilian Standard Time";
		public static readonly string CentralEurope = "Central Europe Standard Time";
		public static readonly string CentralEuropean = "Central European Standard Time";
		public static readonly string CentralPacific = "Central Pacific Standard Time";
		public static readonly string Central = "Central Standard Time";
		public static readonly string CentralMexico = "Central Standard Time (Mexico)";
		public static readonly string China = "China Standard Time";
		public static readonly string DateLine = "Dateline Standard Time";
		public static readonly string EasternAfrica = "E. Africa Standard Time";
		public static readonly string EasternAustralia = "E. Australia Standard Time";
		public static readonly string EasternEurope = "E. Europe Standard Time";
		public static readonly string EasternSouthAmerica = "E. South America Standard Time";
		public static readonly string Eastern = "Eastern Standard Time";
		public static readonly string Cairo = "Egypt Standard Time";
		public static readonly string Ekaterinburg = "Ekaterinburg Standard Time";
		public static readonly string Fiji = "Fiji Standard Time";
		public static readonly string NorthEurope = "FLE Standard Time";
		public static readonly string Georgian = "Georgian Standard Time";
		public static readonly string Lisbon = "GMT Standard Time";
		public static readonly string Greenland = "Greenland Standard Time";
		public static readonly string Greenwich = "Greenwich Standard Time";
		public static readonly string Balkan = "GTB Standard Time";
		public static readonly string Hawaii = "Hawaiian Standard Time";
		public static readonly string India = "India Standard Time";
		public static readonly string Iran = "Iran Standard Time";
		public static readonly string Israel = "Israel Standard Time";
		public static readonly string Jordan = "Jordan Standard Time";
		public static readonly string Kamchatka = "Kamchatka Standard Time";
		public static readonly string Korea = "Korea Standard Time";
		public static readonly string Mauritius = "Mauritius Standard Time";
		public static readonly string Mexico = "Mexico Standard Time";
		public static readonly string Mexico2 = "Mexico Standard Time 2";
		public static readonly string MidAtlantic = "Mid-Atlantic Standard Time";
		public static readonly string MidEast = "Middle East Standard Time";
		public static readonly string Montevideo = "Montevideo Standard Time";
		public static readonly string Morocco = "Morocco Standard Time";
		public static readonly string Mountain = "Mountain Standard Time";
		public static readonly string MountainMexico = "Mountain Standard Time (Mexico)";
		public static readonly string Myanmar = "Myanmar Standard Time";
		public static readonly string NorthCentralAsia = "N. Central Asia Standard Time";
		public static readonly string Namibia = "Namibia Standard Time";
		public static readonly string Nepal = "Nepal Standard Time";
		public static readonly string NewZealand = "New Zealand Standard Time";
		public static readonly string Newfoundland = "Newfoundland Standard Time";
		public static readonly string NorthAsiaEast = "North Asia East Standard Time";
		public static readonly string NorthAsia = "North Asia Standard Time";
		public static readonly string SouthPacific = "Pacific SA Standard Time";
		public static readonly string Pacific = "Pacific Standard Time";
		public static readonly string PacificMexico = "Pacific Standard Time (Mexico)";
		public static readonly string Pakistan = "Pakistan Standard Time";
		public static readonly string Paraguay = "Paraguay Standard Time";
		public static readonly string Romance = "Romance Standard Time";
		public static readonly string Russian = "Russian Standard Time";
		public static readonly string SouthAmericaEastern = "SA Eastern Standard Time";
		public static readonly string SouthAmericaPacific = "SA Pacific Standard Time";
		public static readonly string SouthAmericaWestern = "SA Western Standard Time";
		public static readonly string MidwayIsland = "Samoa Standard Time";
		public static readonly string SouthEasternAsia = "SE Asia Standard Time";
		public static readonly string Singapore = "Singapore Standard Time";
		public static readonly string SouthAfrica = "South Africa Standard Time";
		public static readonly string SriLanka = "Sri Lanka Standard Time";
		public static readonly string Taipei = "Taipei Standard Time";
		public static readonly string Tasmania = "Tasmania Standard Time";
		public static readonly string Tokyo = "Tokyo Standard Time";
		public static readonly string Tonga = "Tonga Standard Time";
		public static readonly string Ulaanbaatar = "Ulaanbaatar Standard Time";
		public static readonly string USEastern = "US Eastern Standard Time";
		public static readonly string USMountain = "US Mountain Standard Time";
		public static readonly string UTC = "UTC";
		public static readonly string Venezuela = "Venezuela Standard Time";
		public static readonly string Vladivostok = "Vladivostok Standard Time";
		public static readonly string WestAustralia = "W. Australia Standard Time";
		public static readonly string WestCentralAfrica = "W. Central Africa Standard Time";
		public static readonly string WestEurope = "W. Europe Standard Time";
		public static readonly string WestAsia = "West Asia Standard Time";
		public static readonly string WestPacific = "West Pacific Standard Time";
		public static readonly string Yakutsk = "Yakutsk Standard Time";
		public static readonly string Kaliningrad = "Kaliningrad Standard Time";
		public static readonly string Salvador = "Bahia Standard Time";
		public static readonly string Damascus = "Syria Standard Time";
	}
}
namespace DevExpress.XtraScheduler.Native {
	#region SchedulerTimeZoneHelperEventWrapper
	public class SchedulerTimeZoneHelperEventWrapper : GenericEventListenerWrapper<TimeZoneEngineSyncronizer, object> {
#if DEBUGTEST
		internal static bool EnableTestStub;
		static List<SchedulerTimeZoneHelperEventWrapper> instances;
		static List<SchedulerTimeZoneHelperEventWrapper> Instances {
			get {
				if (instances == null)
					instances = new List<SchedulerTimeZoneHelperEventWrapper>();
				return instances;
			}
		}
		internal static void TestRaiseSysTimeChanged() {
			int count = Instances.Count;
			for (int i = 0; i < count; i++) {
				SchedulerTimeZoneHelperEventWrapper instance = Instances[i];
				instance.OnSysTimeChanged(instance, EventArgs.Empty);
			}							
		}
		internal static void TestClear() {
			instances = null;
		}
#endif
		public SchedulerTimeZoneHelperEventWrapper(TimeZoneEngineSyncronizer instance)
			: base(instance, null) {
#if DEBUGTEST
			if (EnableTestStub)
				Instances.Add(this);
#endif
		}
		protected override void SubscribeEvents() {
#if !SL
			if (!SecurityHelper.IsPartialTrust)
				SubscribeTimeChangedEvent();
#endif
		}
		protected override void UnsubscribeEvents() {
#if !SL
			if (!SecurityHelper.IsPartialTrust)
				UnsubscribeTimeChangedEvent();
#endif
		}
#if !SL
		[SecuritySafeCritical]
		protected internal virtual void SubscribeTimeChangedEvent() {
			SystemEvents.TimeChanged += new EventHandler(OnSysTimeChanged);
		}
		[SecuritySafeCritical]
		protected internal virtual void UnsubscribeTimeChangedEvent() {
			SystemEvents.TimeChanged -= new EventHandler(OnSysTimeChanged);
		}
#endif
		protected void OnSysTimeChanged(object sender, EventArgs e) {
			if (IsAlive())
				ListenerInstance.OnSysTimeChanged(this, e);
			else
				CleanUp();
		}
	}
	#endregion
	#region TimeZoneComparer
	public class TimeZoneComparer : IComparer<TimeZoneInfo> {
		public static TimeZoneComparer Default = new TimeZoneComparer();
		public int Compare(TimeZoneInfo firstTimeZone, TimeZoneInfo lastTimeZone) {
			return Math.Sign(firstTimeZone.BaseUtcOffset.Ticks - lastTimeZone.BaseUtcOffset.Ticks);
		}
	}
	#endregion
	public static class TimeZoneInfoUtils {
		public static DateTime CalculateDateTimeFromTransitionTime(int year, System.TimeZoneInfo.TransitionTime transition) {
			Int32 daysInMonth = DateTime.DaysInMonth(year, transition.Month);
			if (transition.IsFixedDateRule) {
				if (daysInMonth >= transition.Day)
					daysInMonth = transition.Day;
				return CreateDateTime(year, daysInMonth, transition);
			}
			DateTime result;
			int dayOffset = 0;
			if (transition.Week == 5) {
				result = CreateDateTime(year, daysInMonth, transition);
				dayOffset = CalculateWeekDayOffset(result, transition);
				if (dayOffset > 0)
					dayOffset -= 7;
				if (dayOffset > 0)
					dayOffset = 0;
			} else {
				result = CreateDateTime(year, 1, transition);
				dayOffset = CalculateWeekDayOffset(result, transition);
				if (dayOffset < 0)
					dayOffset += 7;
				dayOffset += 7 * (transition.Week - 1);
				if (dayOffset < 0)
					dayOffset = 0;
			}
			return result.AddDays(dayOffset);
		}
		static int CalculateWeekDayOffset(DateTime date, System.TimeZoneInfo.TransitionTime transition) {
			return (int)transition.DayOfWeek - (int)date.DayOfWeek;
		}
		static DateTime CreateDateTime(int year, Int32 day, System.TimeZoneInfo.TransitionTime transition) {
			DateTime time = transition.TimeOfDay;
			return new DateTime(year, transition.Month, day, time.Hour, time.Minute, time.Second, time.Millisecond);
		}
	}
}

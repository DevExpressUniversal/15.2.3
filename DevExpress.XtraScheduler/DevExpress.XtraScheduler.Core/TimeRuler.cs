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
using System.Collections.Specialized;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
namespace DevExpress.XtraScheduler {
	#region TimeRuler
#if !SL
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
#endif
	public class TimeRuler : ISupportObjectChanged, IXtraSerializable {
		#region Fields
#pragma warning disable 618
		const CurrentTimeVisibility defaultShowCurrentTime = CurrentTimeVisibility.Auto;
#pragma warning restore 618
		const bool defaultShowMinutes = false;
		const bool defaultAdjustForDaylightSavingTime = true;
		const bool defaultAlwaysShowTimeDesignator = false;
		const bool defaultAlwaysShowTopRowTime = false;
		const string defaultCaption = "";
		const bool defaultUseClientTimeZone = true;
		const bool defaultVisible = true;
		TimeMarkerVisibility? timeMarkerVisibility;
		bool showMinutes = defaultShowMinutes;
		bool adjustForDaylightSavingTime = defaultAdjustForDaylightSavingTime;
		bool alwaysShowTimeDesignator = defaultAlwaysShowTimeDesignator;
		bool alwaysShowTopRowTime = defaultAlwaysShowTopRowTime;
		bool useClientTimeZone = defaultUseClientTimeZone;
		bool visible = defaultVisible;
		string caption = defaultCaption;
		string timeZoneId;
		#endregion
		public TimeRuler() {
			this.useClientTimeZone = defaultUseClientTimeZone;
			this.timeMarkerVisibility = null;
		}
		#region Properties
		#region TimeZoneId
		[NotifyParentProperty(true), AutoFormatDisable(), Localizable(false), TypeConverter(typeof(DevExpress.XtraScheduler.Design.TimeZoneIdStringTypeConverter))]
		public String TimeZoneId {
			get {
				if (UseClientTimeZone || String.IsNullOrEmpty(timeZoneId))
					return ObtainClientTimeZoneId();
				return timeZoneId;
			}
			set {
				if (Object.Equals(value, timeZoneId))
					return;
				SetTimeZoneCore(value);
				RaiseChanged();
			}
		}
		internal bool ShouldSerializeTimeZoneId() {
			return !UseClientTimeZone && !String.IsNullOrEmpty(this.timeZoneId);
		}
		internal void ResetTimeZoneId() {
			this.timeZoneId = String.Empty;
			UseClientTimeZone = true;
		}
		#endregion
		#region UseClientTimeZone
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeRulerUseClientTimeZone"),
#endif
		DefaultValue(defaultUseClientTimeZone), NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatEnable()]
		public bool UseClientTimeZone {
			get { return useClientTimeZone; }
			set {
				if (useClientTimeZone == value)
					return;
				useClientTimeZone = value;
				RaiseChanged();
			}
		}
		#endregion
		#region TimeMarkerVisibility
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeRulerTimeMarkerVisibility"),
#endif
		DefaultValue(null), NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatEnable()]
		public TimeMarkerVisibility? TimeMarkerVisibility {
			get { return timeMarkerVisibility; }
			set {
				if (timeMarkerVisibility == value)
					return;
				timeMarkerVisibility = value;
				RaiseChanged();
			}
		}
		#endregion
		#region ShowMinutes
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeRulerShowMinutes"),
#endif
DefaultValue(defaultShowMinutes), NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatEnable()]
		public bool ShowMinutes {
			get { return showMinutes; }
			set {
				if (showMinutes == value)
					return;
				showMinutes = value;
				RaiseChanged();
			}
		}
		#endregion
#pragma warning disable 618
		#region ShowCurrentTime
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeRulerShowCurrentTime"),
#endif
		DefaultValue(defaultShowCurrentTime), NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatEnable()]
		[Obsolete("Use TimeMarkerVisibility instead.", false)]
		public CurrentTimeVisibility ShowCurrentTime {
			get {
				switch (TimeMarkerVisibility) {
					case XtraScheduler.TimeMarkerVisibility.Always:
						return CurrentTimeVisibility.Always;
					case XtraScheduler.TimeMarkerVisibility.Never:
						return CurrentTimeVisibility.Never;
					default:
						return CurrentTimeVisibility.Auto;
				}
			}
			set {
				switch (value) {
					case CurrentTimeVisibility.Always:
						TimeMarkerVisibility = XtraScheduler.TimeMarkerVisibility.Always;
						break;
					case CurrentTimeVisibility.Never:
						TimeMarkerVisibility = XtraScheduler.TimeMarkerVisibility.Never;
						break;
					default:
						TimeMarkerVisibility = XtraScheduler.TimeMarkerVisibility.TodayView;
						break;
				}
			}
		}
		#endregion
#pragma warning restore 618
		#region AdjustForDaylightSavingTime
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeRulerAdjustForDaylightSavingTime"),
#endif
DefaultValue(defaultAdjustForDaylightSavingTime), NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatEnable()]
		public bool AdjustForDaylightSavingTime {
			get { return adjustForDaylightSavingTime; }
			set {
				if (adjustForDaylightSavingTime == value)
					return;
				adjustForDaylightSavingTime = value;
				RaiseChanged();
			}
		}
		#endregion
		#region AlwaysShowTimeDesignator
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeRulerAlwaysShowTimeDesignator"),
#endif
DefaultValue(defaultAlwaysShowTimeDesignator), NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatEnable()]
		public bool AlwaysShowTimeDesignator {
			get { return alwaysShowTimeDesignator; }
			set {
				if (alwaysShowTimeDesignator == value)
					return;
				alwaysShowTimeDesignator = value;
				RaiseChanged();
			}
		}
		#endregion
		#region AlwaysShowTopRowTime
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeRulerAlwaysShowTopRowTime"),
#endif
DefaultValue(defaultAlwaysShowTopRowTime), NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatEnable()]
		public virtual bool AlwaysShowTopRowTime {
			get { return alwaysShowTopRowTime; }
			set {
				if (alwaysShowTopRowTime == value)
					return;
				alwaysShowTopRowTime = value;
				RaiseChanged();
			}
		}
		#endregion
		#region Caption
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeRulerCaption"),
#endif
DefaultValue(defaultCaption), Localizable(true), NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatDisable()]
		public string Caption {
			get { return caption; }
			set {
				if (caption == value)
					return;
				caption = value;
				RaiseChanged();
			}
		}
		#endregion
		#region Visible
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeRulerVisible"),
#endif
		DefaultValue(defaultVisible), NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatEnable()]
		public bool Visible {
			get { return visible; }
			set {
				if (visible == value)
					return;
				visible = value;
				RaiseChanged();
			}
		}
		#endregion
		#endregion
		#region Events
		#region Changed
		EventHandler onChanged;
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		#region QueryClientTimeZoneId
		QueryClientTimeZoneIdEventHandler onQueryClientTimeZoneId;
		internal event QueryClientTimeZoneIdEventHandler QueryClientTimeZoneId { add { onQueryClientTimeZoneId += value; } remove { onQueryClientTimeZoneId -= value; } }
		protected internal virtual string RaiseQueryClientTimeZoneId() {
			if (onQueryClientTimeZoneId != null) {
				QueryClientTimeZoneIdEventArgs args = new QueryClientTimeZoneIdEventArgs();
				onQueryClientTimeZoneId(this, args);
				return args.TimeZoneId;
			}
			return null;
		}
		#endregion
		#endregion
		protected internal virtual void Assign(TimeRuler source) {
			if (source == null)
				return;
			TimeZoneId = source.TimeZoneId;
			TimeMarkerVisibility = source.TimeMarkerVisibility;
			ShowMinutes = source.ShowMinutes;
			AdjustForDaylightSavingTime = source.AdjustForDaylightSavingTime;
			AlwaysShowTimeDesignator = source.AlwaysShowTimeDesignator;
			alwaysShowTopRowTime = source.AlwaysShowTopRowTime;
			Caption = source.Caption;
			UseClientTimeZone = source.UseClientTimeZone;
			Visible = source.Visible;
#pragma warning disable 618
			ShowCurrentTime = source.ShowCurrentTime;
#pragma warning restore 618
		}
		protected internal virtual string ObtainClientTimeZoneId() {
			return ObtainClientTimeZone().Id;
		}
		protected internal virtual TimeZoneInfo ObtainClientTimeZone() {
			string id = RaiseQueryClientTimeZoneId();
			if (String.IsNullOrEmpty(id))
				return TimeZoneEngine.Local;
			else
				return TimeZoneInfo.FindSystemTimeZoneById(id);
		}
		protected internal virtual void SetTimeZoneCore(string tzId) {
			if (String.IsNullOrEmpty(tzId)) {
				this.useClientTimeZone = true;
			} else {
				this.timeZoneId = tzId;
				this.useClientTimeZone = false;
			}
		}
		protected internal virtual void UpdateClientTimeZone(string timeZoneId) {
			if (UseClientTimeZone)
				this.timeZoneId = timeZoneId;
		}
		#region IXtraSerializable Members
		bool isDeserializing;
		internal bool IsDeserializing { get { return isDeserializing; } }
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			isDeserializing = true;
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			isDeserializing = false;
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		#endregion
	}
	#endregion
	#region TimeRulerCollection
#if !SL
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
#endif
	[ListBindable(BindableSupport.No)]
	public class TimeRulerCollection : NotificationCollection<TimeRuler> {
		protected internal void Assign(TimeRulerCollection source) {
			if (source == null)
				return;
			Clear();
			foreach (TimeRuler ruler in source) {
				TimeRuler copyRuler = new TimeRuler();
				copyRuler.Assign(ruler);
				Add(copyRuler);
			}
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Drawing {
	#region TimeFormatInfo
	public class TimeFormatInfo {
		#region Fields
		string hourFormat;
		string halfDayHourFormat;
		string timeDesignatorOnlyFormat;
		string minutesOnlyFormat;
		string hourOnlyFormat;
		#endregion
		public TimeFormatInfo() {
		}
		#region Properties
		public string HourFormat { get { return hourFormat; } }
		public string HalfDayHourFormat { get { return halfDayHourFormat; } }
		public string TimeDesignatorOnlyFormat { get { return timeDesignatorOnlyFormat; } }
		public string MinutesOnlyFormat { get { return minutesOnlyFormat; } }
		public string HourOnlyFormat { get { return hourOnlyFormat; } }
		#endregion
		public virtual void Initialize(TimeRuler ruler, ITimeRulerFormatStringService provider) {
			string pattern = GetLongTimePattern();
			StringCollection formats = GetWholeHourTimeFormats();
			if (DateTimeFormatHelper.IsTimeDesignator(formats[1])) {
				this.hourFormat = DateTimeFormatHelper.StripSeconds(pattern);
				this.halfDayHourFormat = DateTimeFormatHelper.StripMinutes(hourFormat);
				this.hourFormat = DateTimeFormatHelper.StripTimeDesignator(hourFormat);
			} else {
				this.hourFormat = DateTimeFormatHelper.StripSeconds(pattern);
				this.hourFormat = DateTimeFormatHelper.StripTimeDesignator(hourFormat);
				this.halfDayHourFormat = hourFormat;
			}
			this.halfDayHourFormat = DateTimeFormatHelper.FixCustomFormat(halfDayHourFormat);
			this.hourFormat = DateTimeFormatHelper.FixCustomFormat(hourFormat);
			this.hourOnlyFormat = formats[0];
			this.timeDesignatorOnlyFormat = formats[1];
			this.minutesOnlyFormat = formats[2];
			if (provider != null) {
				string format;
				format = provider.GetHourFormat(ruler);
				if (!String.IsNullOrEmpty(format))
					this.hourFormat = format;
				format = provider.GetHalfDayHourFormat(ruler);
				if (!String.IsNullOrEmpty(format))
					this.halfDayHourFormat = format;
				format = provider.GetHourOnlyFormat(ruler);
				if (!String.IsNullOrEmpty(format))
					this.hourOnlyFormat = format;
				format = provider.GetTimeDesignatorOnlyFormat(ruler);
				if (!String.IsNullOrEmpty(format))
					this.timeDesignatorOnlyFormat = format;
				format = provider.GetMinutesOnlyFormat(ruler);
				if (!String.IsNullOrEmpty(format))
					this.minutesOnlyFormat = format;
			}
		}
		protected internal virtual string GetLongTimePattern() {
			return DateTimeFormatHelper.CurrentDateTimeFormat.LongTimePattern;
		}
		protected internal virtual StringCollection GetWholeHourTimeFormats() {
			return DateTimeFormatHelper.GetWholeHourTimeFormats();
		}
	}
	#endregion
	#region ScaleFormatHelper
	public static class ScaleFormatHelper {
		public static string ChooseHourFormat(DateTime time, bool useTimeDesignator, TimeFormatInfo formatInfo) {
			if (time.Hour == 0 || time.Hour == 12 || useTimeDesignator)
				return formatInfo.HalfDayHourFormat;
			else
				return formatInfo.HourFormat;
		}
		public static string ChooseMinutesFormat(DateTime time, bool useTimeDesignator, TimeFormatInfo formatInfo) {
			if (time.Minute == 0 && time.Second == 0) {
				if (time.Hour == 0 || time.Hour == 12 || useTimeDesignator)
					return formatInfo.TimeDesignatorOnlyFormat;
				else
					return formatInfo.MinutesOnlyFormat;
			} else
				return formatInfo.MinutesOnlyFormat;
		}
	}
	#endregion
}

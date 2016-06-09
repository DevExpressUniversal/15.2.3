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
using System.ComponentModel;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler.Cookies;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Web.ASPxScheduler {
	#region ASPxSchedulerOptionsCookies
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ASPxSchedulerOptionsCookies : StateManager {
		public ASPxSchedulerOptionsCookies() {
		}
		#region Enabled
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookiesEnabled"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool Enabled {
			get { return GetBoolProperty(ASPxSchedulerCookieName.Enabled, false); }
			set { SetBoolProperty(ASPxSchedulerCookieName.Enabled, false, value); }
		}
		#endregion
		#region CookiesID
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookiesCookiesID"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false)]
		public string CookiesID {
			get { return GetStringProperty(ASPxSchedulerCookieName.CookiesID, string.Empty); }
			set { SetStringProperty(ASPxSchedulerCookieName.CookiesID, string.Empty, value); }
		}
		#endregion
		#region Version
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookiesVersion"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false)]
		public string Version {
			get { return GetStringProperty(ASPxSchedulerCookieName.Version, string.Empty); }
			set { SetStringProperty(ASPxSchedulerCookieName.Version, string.Empty, value); }
		}
		#endregion
		#region StoreActiveViewType
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookiesStoreActiveViewType"),
#endif
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreActiveViewType {
			get { return GetBoolProperty(ASPxSchedulerCookieName.StoreActiveViewType, true); }
			set { SetBoolProperty(ASPxSchedulerCookieName.StoreActiveViewType, true, value); }
		}
		#endregion
		#region StoreGroupType
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookiesStoreGroupType"),
#endif
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreGroupType {
			get { return GetBoolProperty(ASPxSchedulerCookieName.StoreGroupType, true); }
			set { SetBoolProperty(ASPxSchedulerCookieName.StoreGroupType, true, value); }
		}
		#endregion
		#region StoreDayViewTimeScale
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookiesStoreDayViewTimeScale"),
#endif
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreDayViewTimeScale {
			get { return GetBoolProperty(ASPxSchedulerCookieName.StoreDayViewTimeScale, true); }
			set { SetBoolProperty(ASPxSchedulerCookieName.StoreDayViewTimeScale, true, value); }
		}
		#endregion
		#region StoreWorkWeekViewTimeScale
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookiesStoreWorkWeekViewTimeScale"),
#endif
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreWorkWeekViewTimeScale {
			get { return GetBoolProperty(ASPxSchedulerCookieName.StoreWorkWeekViewTimeScale, true); }
			set { SetBoolProperty(ASPxSchedulerCookieName.StoreWorkWeekViewTimeScale, true, value); }
		}
		#endregion
		#region StoreTimelineScalesInfo
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookiesStoreTimelineScalesInfo"),
#endif
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreTimelineScalesInfo {
			get { return GetBoolProperty(ASPxSchedulerCookieName.StoreTimelineScalesInfo, true); }
			set { SetBoolProperty(ASPxSchedulerCookieName.StoreTimelineScalesInfo, true, value); }
		}
		#endregion
		#region StoreTimeZoneId
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookiesStoreTimeZoneId"),
#endif
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreTimeZoneId {
			get { return GetBoolProperty(ASPxSchedulerCookieName.StoreTimeZoneId, true); }
			set { SetBoolProperty(ASPxSchedulerCookieName.StoreTimeZoneId, true, value); }
		}
		#endregion
		#region StoreStart
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerOptionsCookiesStoreStart"),
#endif
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreStart
		{
			get { return GetBoolProperty(ASPxSchedulerCookieName.Start, true); }
			set { SetBoolProperty(ASPxSchedulerCookieName.Start, true, value); }
		}
		#endregion       
		public virtual void Assign(ASPxSchedulerOptionsCookies source) {
			if (source == null)
				return;
			Enabled = source.Enabled;
			CookiesID = source.CookiesID;
			Version = source.Version;
			StoreActiveViewType = source.StoreActiveViewType;
			StoreGroupType = source.StoreGroupType;
			StoreDayViewTimeScale = source.StoreDayViewTimeScale;
			StoreWorkWeekViewTimeScale = source.StoreWorkWeekViewTimeScale;
			StoreTimelineScalesInfo = source.StoreTimelineScalesInfo;
			StoreTimeZoneId = source.StoreTimeZoneId;
			StoreStart = source.StoreStart;
		}
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Cookies {
	#region ASPxSchedulerCookieName
	public static class ASPxSchedulerCookieName {
		public const string Enabled = "Enabled";
		public const string CookiesID = "CookiesID";
		public const string Version = "Version";
		public const string StoreActiveViewType = "StoreActiveViewType";
		public const string StoreGroupType = "StoreGroupType";
		public const string StoreDayViewTimeScale = "StoreDayViewTimeScale";
		public const string StoreWorkWeekViewTimeScale = "StoreWorkWeekViewTimeScale";
		public const string StoreTimelineScalesInfo = "StoreTimelineScalesInfo";
		public const string StoreTimeZoneId = "StoreTimeZoneId";
		public const string Start = "Start";
	}
	#endregion
	#region ASPxSchedulerCookies
	public class ASPxSchedulerCookies : IXtraSupportShouldSerialize {
		#region Fields
		protected const string CurrentVersion = "1.0";
		protected const char Divider = '|';
		protected const char ItemDivider = ';';
		protected const char ParameterDivider = ':';
		protected const string VersionPrefix = "version";
		ASPxScheduler control;
		SchedulerViewType activeViewType = SchedulerViewType.Day;
		SchedulerGroupType groupType = SchedulerGroupType.None;
		TimeSpan dayViewTimeScale = TimeSpan.FromMinutes(30);
		TimeSpan workWeekViewTimeScale = TimeSpan.FromMinutes(30);
		string clientTimeZoneId = TimeZoneId.Custom;
		string version = string.Empty;
		string timelineScalesInfo = string.Empty;
		XtraSupportShouldSerializeHelper shouldSerializeHelper = new XtraSupportShouldSerializeHelper();
		DateTime start;
		#endregion
		public ASPxSchedulerCookies(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
			Reset();
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("ActiveViewType", XtraShouldSerializeActiveViewType);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("GroupType", XtraShouldSerializeGroupType);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("DayViewTimeScale", XtraShouldSerializeDayViewTimeScale);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("WorkWeekViewTimeScale", XtraShouldSerializeWorkWeekViewTimeScale);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("TimelineScalesInfo", XtraShouldSerializeTimelineScalesInfo);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("ClientTimeZoneId", XtraShouldSerializeClientTimeZoneId);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("Start", XtraShouldSerializeStart);
		}
		#region Properties
		public ASPxScheduler Control { get { return control; } }
		public ASPxSchedulerOptionsCookies Options { get { return Control.OptionsCookies; } }
		#region ActiveViewType
		[XtraSerializableProperty()]
		public SchedulerViewType ActiveViewType { set { activeViewType = value; } get { return activeViewType; } }
		protected virtual bool XtraShouldSerializeActiveViewType() {
			return Options.StoreActiveViewType;
		}
		#endregion
		#region GroupType
		[XtraSerializableProperty()]
		public SchedulerGroupType GroupType { set { groupType = value; } get { return groupType; } }
		protected virtual bool XtraShouldSerializeGroupType() {
			return Options.StoreGroupType;
		}
		#endregion
		#region DayViewTimeScale
		[XtraSerializableProperty()]
		public TimeSpan DayViewTimeScale { set { dayViewTimeScale = value; } get { return dayViewTimeScale; } }
		protected virtual bool XtraShouldSerializeDayViewTimeScale() {
			return Options.StoreDayViewTimeScale;
		}
		#endregion
		#region WorkWeekViewTimeScale
		[XtraSerializableProperty()]
		public TimeSpan WorkWeekViewTimeScale { set { workWeekViewTimeScale = value; } get { return workWeekViewTimeScale; } }
		protected virtual bool XtraShouldSerializeWorkWeekViewTimeScale() {
			return Options.StoreWorkWeekViewTimeScale;
		}
		#endregion
		#region TimelineScalesInfo
		[XtraSerializableProperty()]
		public string TimelineScalesInfo { set { timelineScalesInfo = value; } get { return timelineScalesInfo; } }
		protected virtual bool XtraShouldSerializeTimelineScalesInfo() {
			return Options.StoreTimelineScalesInfo;
		}
		#endregion
		#region ClientTimeZoneId
		[XtraSerializableProperty()]
		public string ClientTimeZoneId { set { clientTimeZoneId = value; } get { return clientTimeZoneId; } }
		protected virtual bool XtraShouldSerializeClientTimeZoneId() {
			return Options.StoreTimeZoneId;
		}
		#endregion
		#region Start
		[XtraSerializableProperty()]
		public DateTime Start { set { start = value; } get { return start; } }
		protected virtual bool XtraShouldSerializeStart() {
			return Options.StoreStart;
		}
		#endregion
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public string Version { set { version = value; } get { return version; } }
		public virtual bool IsEmpty { get { return CalculateIsEmpty(); } }
		#endregion
		protected internal virtual bool CalculateIsEmpty() {
			return IsActiveViewTypeEmpty() &&
					IsGroupTypeEmpty() &&
					IsDayViewTimeScaleEmpty() &&
					IsWorkWeekViewTimeScaleEmpty() &&
					IsTimelineScalesEmpty() &&
					IsClientTimeZoneIdEmpty() &&
					IsStartEmpty();
		}
		protected internal virtual bool IsActiveViewTypeEmpty() {
			return ActiveViewType == Control.ActiveViewType;
		}
		protected internal virtual bool IsGroupTypeEmpty() {
			return GroupType == Control.GroupType;
		}
		protected internal virtual bool IsDayViewTimeScaleEmpty() {
			return DayViewTimeScale == Control.DayView.TimeScale;
		}
		protected internal virtual bool IsWorkWeekViewTimeScaleEmpty() {
			return WorkWeekViewTimeScale == Control.WorkWeekView.TimeScale;
		}
		protected internal virtual bool IsTimelineScalesEmpty() {
			return TimelineScalesInfo == CreateTimelineScalesInfoString();
		}
		protected internal virtual bool IsClientTimeZoneIdEmpty() {
			return ClientTimeZoneId == Control.OptionsBehavior.ClientTimeZoneId;
		}
		protected internal virtual bool IsStartEmpty() {
			return Start == Control.Start;
		}
		public virtual string SaveState() {
			return SaveStateCore();
		}
		protected internal virtual string SaveStateCore() {
			string version = SaveVersionState();
			string content = SaveContentState();
			return string.Format("{0}{1}{2}", version, Divider, content);
		}
		protected internal virtual string SaveVersionState() {
			return VersionPrefix + Version;
		}
		protected internal virtual string CreateTimelineScalesInfoString() {
			TimeScaleCollection scales = Control.TimelineView.Scales;
			int count = scales.Count;
			string result = string.Empty;
			for (int i = 0; i < count; i++) {
				result += TimeScaleToString(scales[i], i);
				if (i < count - 1) result += ItemDivider;
			}
			return result;
		}
		protected internal virtual string TimeScaleToString(TimeScale scale, int index) {
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}{1}", scale.GetType().Name, ParameterDivider);
			sb.AppendFormat("{0}{1}", index, ParameterDivider); 
			sb.AppendFormat("{0}{1}", SchedulerWebUtils.BoolToStr(scale.Enabled), ParameterDivider);
			sb.Append(SchedulerWebUtils.BoolToStr(scale.Visible));
			return sb.ToString();
		}
		protected internal virtual void TimeScaleFromString(TimeScale scale, int index, string state) {
			string[] parts = state.Split(ParameterDivider);
			if (parts.Length <= 0) return;
			XtraSchedulerDebug.Assert(parts.Length == 4);
			if (scale.GetType().Name.ToString() == parts[0] && index == Convert.ToInt32(parts[1])) {
				scale.Enabled = SchedulerWebUtils.BoolFromStr(parts[2]);
				scale.Visible = SchedulerWebUtils.BoolFromStr(parts[3]);
			}
		}
		protected internal virtual string SaveContentState() {
			IXtraPropertyCollection propInfos = new SerializeHelper().SerializeObject(this, OptionsLayoutBase.FullLayout);
			CompressedBase64XtraSerializer serializer = new CompressedBase64XtraSerializer();
			return serializer.Serialize(propInfos);
		}
		public virtual void LoadState(string state) {
			if (string.IsNullOrEmpty(state))
				return;
			LoadStateCore(state);
			ClearUnsavedFields();
			ApplyToControl();
		}
		protected internal virtual string GetActualCookiesVersion() {
			string ver = Control.OptionsCookies.Version;
			if (String.IsNullOrEmpty(ver))
				return CurrentVersion;
			else
				return ver;
		}
		protected internal virtual void Reset() {
			Version = GetActualCookiesVersion();
			ActiveViewType = Control.ActiveViewType;
			GroupType = Control.GroupType;
			DayViewTimeScale = Control.DayView.TimeScale;
			WorkWeekViewTimeScale = Control.WorkWeekView.TimeScale;
			TimelineScalesInfo = CreateTimelineScalesInfoString();
			ClientTimeZoneId = Control.OptionsBehavior.ClientTimeZoneId;
			Start = Control.Start;
		}
		protected internal virtual void LoadStateCore(string state) {
			string[] parts = state.Split(Divider);
			if (parts.Length <= 0) return;
			string versionState = parts[0];
			string oldVersion = LoadVersionState(versionState);
			if (oldVersion != Version)
				return;
			string contentState = state.Substring(versionState.Length + 1);
			LoadContentState(contentState);
		}
		protected internal virtual void LoadContentState(string state) {
			CompressedBase64XtraSerializer serializer = new CompressedBase64XtraSerializer();
			serializer.Deserialize(this, state);
		}
		protected string LoadVersionState(string state) {
			if (state.StartsWith(VersionPrefix))
				return state.Remove(0, VersionPrefix.Length);
			return string.Empty;
		}
		protected internal virtual void ClearUnsavedFields() {
		}
		public virtual void ApplyToControl() {
			if (IsEmpty) return;
			Control.ActiveViewType = ActiveViewType;
			Control.GroupType = GroupType;
			Control.DayView.TimeScale = DayViewTimeScale;
			Control.WorkWeekView.TimeScale = WorkWeekViewTimeScale;
			Control.OptionsBehavior.ClientTimeZoneId = ClientTimeZoneId;
			ApplyToTimelineScales();
			Control.Start = Start;
		}
		protected internal virtual void ApplyToTimelineScales() {
			string[] itemParts = TimelineScalesInfo.Split(ItemDivider);
			if (itemParts.Length <= 0) return;
			TimeScaleCollection scales = Control.TimelineView.Scales;
			int count = scales.Count;
			if (itemParts.Length != count) return;
			for (int i = 0; i < count; i++)
				TimeScaleFromString(scales[i], i, itemParts[i]);
		}
		#region IXtraSupportShouldSerialize Members
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return shouldSerializeHelper.ShouldSerialize(propertyName);
		}
		#endregion
	}
	#endregion
}

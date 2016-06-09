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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public enum DateOnError { Undo, Null, Today }
	public enum EditFormat { Date, DateTime, Time, Custom }
	public interface ITimeSectionOwner {
		bool ShowTimeSection { get; }
		bool ShowOkButton { get; }
		bool ShowCancelButton { get; }
		bool ShowHourHand { get; }
		bool ShowMinuteHand { get; }
		bool ShowSecondHand { get; }
		string OkButtonText { get; }
		string CancelButtonText { get; }
		string OkButtonClickScript { get; }
		string CancelButtonClickScript { get; }
		string ClearButtonClickScript { get; }
		string TimeEditKeyDownScript { get; }
		string TimeEditLostFocusScript { get; }
		string TimeSectionFooterClassName { get; }
		string TimeSectionHeaderClassName { get; }
		DateEditTimeEditProperties TimeEditProperties{ get; }
		ImageProperties ClockFaceImage { get; }
		ImageProperties HourHandImage { get; }
		ImageProperties MinuteHandImage { get; }
		ImageProperties SecondHandImage { get; }
		DateEditTimeSectionCellStyle TimeEditCellStyle { get; }
		DateEditTimeSectionCellStyle ClockCellStyle { get; }
	}
	public interface IDateEditIDResolver {
		string GetDateEditIdByDataItemName(string dataItemName);
		string[] GetPossibleDataItemNames();
	}
	public class DateEditTimeSectionProperties : PropertiesBase {
		DateEditTimeEditProperties timeEditProperties;
		public DateEditTimeSectionProperties(DateEditProperties owner)
			: base(owner) {
		}
		protected DateEditProperties OwnerProperties { get { return Owner as DateEditProperties; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesTimeEditProperties"),
#endif
 NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DateEditTimeEditProperties TimeEditProperties {
			get {
				if(timeEditProperties == null)
					timeEditProperties = new DateEditTimeEditProperties();
				return timeEditProperties;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesVisible"),
#endif
 NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool Visible {
			get { return GetBoolProperty("Visible", false); }
			set { SetBoolProperty("Visible", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesShowOkButton"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowOkButton {
			get { return GetBoolProperty("ShowOkButton", true); }
			set { SetBoolProperty("ShowOkButton", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesShowCancelButton"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowCancelButton {
			get { return GetBoolProperty("ShowCancelButton", true); }
			set { SetBoolProperty("ShowCancelButton", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesShowHourHand"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowHourHand {
			get { return GetBoolProperty("ShowHourHand", true); }
			set { SetBoolProperty("ShowHourHand", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesShowMinuteHand"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowMinuteHand {
			get { return GetBoolProperty("ShowMinuteHand", true); }
			set { SetBoolProperty("ShowMinuteHand", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesShowSecondHand"),
#endif
 NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool ShowSecondHand {
			get { return GetBoolProperty("ShowSecondHand", false); }
			set { SetBoolProperty("ShowSecondHand", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesOkButtonText"),
#endif
 NotifyParentProperty(true), Localizable(true), DefaultValue(StringResources.Calendar_Ok), AutoFormatDisable]
		public string OkButtonText {
			get { return GetStringProperty("OkButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_OK)); }
			set { SetStringProperty("OkButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_OK), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesCancelButtonText"),
#endif
 NotifyParentProperty(true), Localizable(true), DefaultValue(StringResources.Calendar_Cancel), AutoFormatDisable]
		public string CancelButtonText {
			get { return GetStringProperty("CancelButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_Cancel)); }
			set { SetStringProperty("CancelButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_Cancel), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesClockFaceImage"),
#endif
 NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable, 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties ClockFaceImage { get { return OwnerProperties.Images.DateEditTimeSectionClockFace; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesHourHandImage"),
#endif
 NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties HourHandImage { get { return OwnerProperties.Images.DateEditTimeSectionHourHand; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesMinuteHandImage"),
#endif
 NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties MinuteHandImage { get { return OwnerProperties.Images.DateEditTimeSectionMinuteHand; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesSecondHandImage"),
#endif
 NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties SecondHandImage { get { return OwnerProperties.Images.DateEditTimeSectionSecondHand; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesTimeEditCellStyle"),
#endif
 Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public DateEditTimeSectionCellStyle TimeEditCellStyle { get { return OwnerProperties.Styles.DateEditTimeEditCell; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditTimeSectionPropertiesClockCellStyle"),
#endif
 Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public DateEditTimeSectionCellStyle ClockCellStyle { get { return OwnerProperties.Styles.DateEditClockCell; } }
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { TimeEditProperties });
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			DateEditTimeSectionProperties src = source as DateEditTimeSectionProperties;
			if(src != null) {
				Visible = src.Visible;
				ShowOkButton = src.ShowOkButton;
				ShowCancelButton = src.ShowCancelButton;
				ShowHourHand = src.ShowHourHand;
				ShowMinuteHand = src.ShowMinuteHand;
				ShowSecondHand = src.ShowSecondHand;
				OkButtonText = src.OkButtonText;
				CancelButtonText = src.CancelButtonText;
				ClockFaceImage.Assign(src.ClockFaceImage);
				HourHandImage.Assign(src.HourHandImage);
				MinuteHandImage.Assign(src.MinuteHandImage);
				SecondHandImage.Assign(src.SecondHandImage);
				TimeEditCellStyle.Assign(src.TimeEditCellStyle);
				ClockCellStyle.Assign(src.ClockCellStyle);
				TimeEditProperties.Assign(src.TimeEditProperties);
			}
		}
	}
	public class DateEditTimeEditProperties : TimeEditProperties {
		public DateEditTimeEditProperties()
			: base() { 
		}
		public DateEditTimeEditProperties(IPropertiesOwner owner)
			: base(owner) { 
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string ClientInstanceName { get { return base.ClientInstanceName; } set { base.ClientInstanceName = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new TimeEditClientSideEvents ClientSideEvents { get { return base.ClientSideEvents; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
		public new int ValueChangedDelay { get { return base.ValueChangedDelay; } set { base.ValueChangedDelay = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ConvertEmptyStringToNull { get { return base.ConvertEmptyStringToNull; } set { base.ConvertEmptyStringToNull = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool DisplayFormatInEditMode { get { return base.DisplayFormatInEditMode; } set { base.DisplayFormatInEditMode = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool EnableClientSideAPI { get { return base.EnableClientSideAPI; } set { base.EnableClientSideAPI = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string NullDisplayText { get { return base.NullDisplayText; } set { base.NullDisplayText = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ValidationSettings ValidationSettings { get { return base.ValidationSettings; } }
	}
	public class DateEditCalendarProperties : CalendarProperties {
		public DateEditCalendarProperties()
			: base() {
		}
		public DateEditCalendarProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditCalendarPropertiesControlStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Obsolete("Please use the Style property instead")]
		public AppearanceStyleBase ControlStyle {
			get { return Style; }
		}		
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string ClientInstanceName {
			get { return base.ClientInstanceName; }
			set { base.ClientInstanceName = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new CalendarClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ConvertEmptyStringToNull {
			get { return base.ConvertEmptyStringToNull; }
			set { base.ConvertEmptyStringToNull = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ShowShadow {
			get { return base.ShowShadow; }
			set { base.ShowShadow = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Localizable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string DisplayFormatString {
			get { return base.DisplayFormatString; }
			set { base.DisplayFormatString = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool EnableClientSideAPI {
			get { return base.EnableClientSideAPI; }
			set { base.EnableClientSideAPI = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Localizable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string NullDisplayText {
			get { return base.NullDisplayText; }
			set { base.NullDisplayText = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ValidationSettings ValidationSettings {
			get { return base.ValidationSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableMultiSelect {
			get { return false; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DateTime MinDate {
			get { return base.MinDate; }
			set { base.MinDate = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DateTime MaxDate {
			get { return base.MaxDate; }
			set { base.MaxDate = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DateTimeCollection DisabledDates {
			get { return base.DisabledDates; }
		}
	}
	public class DateEditRangeSettings : PropertiesBase {
		const int MillisecondsPerDay = 86400000;
		internal const string StartDateEditIDName = "StartDateEditID";
		static readonly int MaxPossibleDayCount = (int)(TimeSpan.MaxValue.TotalMilliseconds / MillisecondsPerDay);
		public DateEditRangeSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditRangeSettingsStartDateEditID"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatDisable, TypeConverter(typeof(StartDateEditIDConverter))]
		public string StartDateEditID {
			get { return GetStringProperty(StartDateEditIDName, ""); }
			set {
				SetStringProperty(StartDateEditIDName, "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditRangeSettingsCalendarColumnCount"),
#endif
		NotifyParentProperty(true), DefaultValue(2), AutoFormatDisable]
		public int CalendarColumnCount {
			get { return GetIntProperty("CalendarColumnCount", 2); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "CalendarColumnCount");
				SetIntProperty("CalendarColumnCount", 2, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditRangeSettingsMinDayCount"),
#endif
		NotifyParentProperty(true), DefaultValue(0), AutoFormatDisable]
		public int MinDayCount {
			get { return GetIntProperty("MinDayCount", 0); }
			set {
				CommonUtils.CheckValueRange(value, 0, MaxPossibleDayCount, "MinDayCount");
				SetIntProperty("MinDayCount", 0, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditRangeSettingsMaxDayCount"),
#endif
		NotifyParentProperty(true), DefaultValue(0), AutoFormatDisable]
		public int MaxDayCount {
			get { return GetIntProperty("MaxDayCount", 0); }
			set {
				CommonUtils.CheckValueRange(value, 0, MaxPossibleDayCount, "MaxDayCount");
				SetIntProperty("MaxDayCount", 0, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditRangeSettingsMinErrorText"),
#endif
 NotifyParentProperty(true),
		DefaultValue(StringResources.ASPxDateEdit_DateRange_MinValueErrorText), AutoFormatDisable, Localizable(true)]
		public string MinErrorText {
			get { return GetStringProperty("MinErrorText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.DateRangeMinErrorText)); }
			set { SetStringProperty("MinErrorText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.DateRangeMinErrorText), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditRangeSettingsRangeErrorText"),
#endif
 NotifyParentProperty(true),
		DefaultValue(StringResources.ASPxDateEdit_DateRange_RangeErrorText), AutoFormatDisable, Localizable(true)]
		public string RangeErrorText {
			get { return GetStringProperty("RangeErrorText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.DateRangeErrorText)); }
			set { SetStringProperty("RangeErrorText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.DateRangeErrorText), value); }
		}
		protected internal DateEditProperties OwnerProperties { get { return Owner as DateEditProperties; } }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			DateEditRangeSettings src = source as DateEditRangeSettings;
			if (src != null) {
				StartDateEditID = GetSourceStartDateEditID(src);
				MinDayCount = src.MinDayCount;
				MaxDayCount = src.MaxDayCount;
				MinErrorText = src.MinErrorText;
				RangeErrorText = src.RangeErrorText;
				CalendarColumnCount = src.CalendarColumnCount;
			}
		}
		protected string GetSourceStartDateEditID(DateEditRangeSettings source) {
			IDateEditIDResolver sourceDateEditIdResolver = source.OwnerProperties != null ? source.OwnerProperties.GetOwner() as IDateEditIDResolver : null;
			if (OwnerProperties.GetOwner() is ASPxDateEdit && sourceDateEditIdResolver != null && !string.IsNullOrEmpty(source.StartDateEditID))
				return sourceDateEditIdResolver.GetDateEditIdByDataItemName(source.StartDateEditID);
			return source.StartDateEditID;
		}
		protected internal TimeSpan GetMinRange() {
			return new TimeSpan(MinDayCount, 0, 0, 0);
		}
		protected internal TimeSpan GetMaxRange() {
			return MaxDayCount > 0 ? new TimeSpan(MaxDayCount, 0, 0, 0) : TimeSpan.MaxValue;
		}
		protected string GetMinErrorText() {
			return string.Format(MinErrorText, MinDayCount);
		}
		protected string GetRangeErrorText() {
			return string.Format(RangeErrorText, MinDayCount, MaxDayCount);
		}
		protected internal string GetErrorText() {
			return MaxDayCount != 0 ? GetRangeErrorText() : GetMinErrorText();
		}
		protected internal bool HasDayCountRestrictions() {
			return MinDayCount != 0 || MaxDayCount != 0;
		}
	}
	public class DateEditProperties : DropDownEditPropertiesBase {
		DateEditCalendarProperties calendarProperties;
		DateEditTimeSectionProperties timeSectionProperties;
		DateEditRangeSettings dateRangeSettings;
		private bool formatEnumTouched = false;
		public DateEditProperties()
			: base() {
		}
		public DateEditProperties(IPropertiesOwner owner)
			: base(owner) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("DateEditPropertiesAllowMouseWheel")]
#endif
		public override bool AllowMouseWheel {
			get { return MaskSettingsInternal.AllowMouseWheel; }
			set { MaskSettingsInternal.AllowMouseWheel = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesDisplayFormatString"),
#endif
		NotifyParentProperty(true), DefaultValue("d"), Localizable(true), AutoFormatDisable]
		public override string DisplayFormatString {
			get { return base.DisplayFormatString; }
			set { base.DisplayFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesShowShadow"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public override bool ShowShadow {
			get { return base.ShowShadow; }
			set {
				LayoutChanged();
				base.ShowShadow = value;
				CalendarProperties.ShowShadow = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesCalendarProperties"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DateEditCalendarProperties CalendarProperties {
			get {
				if(calendarProperties == null)
					calendarProperties = new DateEditCalendarProperties(Owner);
				return calendarProperties; 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesTimeSectionProperties"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DateEditTimeSectionProperties TimeSectionProperties {
			get {
				if(timeSectionProperties == null)
					timeSectionProperties = new DateEditTimeSectionProperties(this);
				return timeSectionProperties; 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesDateRangeSettings"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DateEditRangeSettings DateRangeSettings {
			get {
				if (dateRangeSettings == null)
					dateRangeSettings = CreateDateRangeSettings();
				return dateRangeSettings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesMinDate"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(DateTime), ""), AutoFormatDisable]
		public DateTime MinDate {
			get { return CalendarProperties.MinDate; }
			set { 
				CalendarProperties.MinDate = value;
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesMaxDate"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(DateTime), ""), AutoFormatDisable]
		public DateTime MaxDate {
			get { return CalendarProperties.MaxDate; }
			set { 
				CalendarProperties.MaxDate = value;
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesShowOutOfRangeWarning"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowOutOfRangeWarning {
			get { return GetBoolProperty("ShowOutOfRangeWarning", true); }
			set { SetBoolProperty("ShowOutOfRangeWarning", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesDisabledDates"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(DateTimeCollection), ""), AutoFormatDisable]
		public DateTimeCollection DisabledDates {
			get { return CalendarProperties.DisabledDates; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesDateOnError"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(DateOnError.Undo), AutoFormatDisable]
		public DateOnError DateOnError {
			get { return (DateOnError)GetEnumProperty("DateOnError", DateOnError.Undo); }
			set { SetEnumProperty("DateOnError", DateOnError.Undo, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesEditFormatString"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true), AutoFormatDisable, RefreshProperties(RefreshProperties.Repaint)]
		public string EditFormatString {
			get { return GetStringProperty("EditFormatString", ""); }
			set {
				SetStringProperty("EditFormatString", "", value);
				CustomEditFormatAssigned(value);
				SyncMaskAndEditFormat();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesEditFormat"),
#endif
		NotifyParentProperty(true), DefaultValue(EditFormat.Date), AutoFormatDisable]
		public EditFormat EditFormat {
			get { return (EditFormat)GetEnumProperty("EditFormat", EditFormat.Date); }
			set {
				SetEnumProperty("EditFormat", EditFormat.Date, value);
				this.formatEnumTouched = true;
				SyncMaskAndEditFormat();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesAllowNull"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool AllowNull {
			get { return GetBoolProperty("AllowNull", true); }
			set { SetBoolProperty("AllowNull", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesMaskHintStyle"),
#endif
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public MaskHintStyle MaskHintStyle {
			get { return Styles.MaskHint; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesUseMaskBehavior"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool UseMaskBehavior {
			get { return GetBoolProperty("UseMaskBehavior", false); }
			set {
				SetBoolProperty("UseMaskBehavior", false, value);
				SyncMaskAndEditFormat();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string NullText {
			get { return NullTextInternal; }
			set { NullTextInternal = value; }
		}
		[NotifyParentProperty(true), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int MaxLength {
			get { return 0; }
			set {  }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatDisable]
		public new DateEditClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as DateEditClientSideEvents; }
		}
		protected override string DefaultDisplayFormatString {
			get { return "d"; }
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxDateEdit();
		}
		protected override string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			if(args.DisplayText == null && args.EditValue is DateTime) {
				DateTime date = (DateTime)args.EditValue;
				if(date == DateTime.MinValue)
					args.EditValue = null;
			}
			return base.GetDisplayTextCore(args, encode);
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new DateEditClientSideEvents(this);
		}
		protected internal override bool IsClearButtonVisibleAuto() {
			return AllowNull && base.IsClearButtonVisibleAuto();
		}
		public override void Assign(PropertiesBase source) {
			DateEditProperties src = source as DateEditProperties;
			if(src != null) {
				CalendarProperties.Assign(src.CalendarProperties);
				TimeSectionProperties.Assign(src.TimeSectionProperties);
				DateRangeSettings.Assign(src.DateRangeSettings);
				DateOnError = src.DateOnError;
				EditFormatString = src.EditFormatString;
				EditFormat = src.EditFormat;
				AllowNull = src.AllowNull;
				UseMaskBehavior = src.UseMaskBehavior;
				ShowOutOfRangeWarning = src.ShowOutOfRangeWarning;
			}
			base.Assign(source);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { CalendarProperties, TimeSectionProperties, DateRangeSettings });
		}
		protected override MaskSettings CreateMaskSettings() {
			MaskSettings instance = new MaskSettings(this);
			instance.IsDateTimeOnly = true;			
			return instance;
		}
		protected virtual DateEditRangeSettings CreateDateRangeSettings() {
			return new DateEditRangeSettings(this);
		}
		protected internal string GetDateFormatString() {
			return DateTimeEditUtils.GetDateFormatString(EditFormat, EditFormatString, 
				CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}
		protected void SyncMaskAndEditFormat() {
			MaskSettingsInternal.Mask = UseMaskBehavior ? GetDateFormatString() : string.Empty;
		}
		protected void CustomEditFormatAssigned(string value) {
			if(!IsDesignMode() || String.IsNullOrEmpty(value) || this.formatEnumTouched)
				return;			
			EditFormat = EditFormat.Custom;
			DateTimeEditUtils.NotifyOwnerComponentChanged(Owner);
		}
		protected internal IPropertiesOwner GetOwner() {
			return Owner;
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DefaultProperty("Date"), DefaultEvent("DateChanged"),
	Designer("DevExpress.Web.Design.ASPxDateEditDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxDateEdit.bmp"),
	ValidationProperty("Text") 
	]
	public class ASPxDateEdit : ASPxDropDownEditBase, ITimeSectionOwner {
		protected internal const string DateEditScriptResourceName = EditScriptsResourcePath + "DateEdit.js";
		internal const string PopupCalendarOwnerIDName = "PopupCalendarOwnerID";
		const string OkClickHandlerName = "ASPx.DECalOkClick();";
		const string CancelClickHandlerName = "ASPx.DECalCancelClick();";
		const string ClearClickHandlerName = "ASPx.DECalClearClick();";
		const string TimeSectionTimeEditKeyDownHandlerName = "ASPx.DETimeEditKeyDown";
		const string TimeSectionTimeEditLostFocusHandlerName = "ASPx.DETimeEditLostFocus";
		const string TimeSectionFooterClassName = "dxeDETSF";
		const string TimeSectionHeaderClassName = "dxeDETSH";
		static readonly object EventDateChanged = new object();
		static readonly object EventCalendarCutomDisabledDate = new object();
		static readonly object EventCalendarDayCellInitialize = new object();
		static readonly object EventCalendarDayCellCreated = new object();
		static readonly object EventCalendarDayCellPrepared = new object();
		static readonly object EventPostBackValueSecuring = new object();
		private ASPxDateEdit popupCalendarOwner = null;
		private ASPxDateEdit startDateEdit = null;
		public ASPxDateEdit()
			: base() {
		}
		protected ASPxDateEdit(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditDateOnError"),
#endif
		Category("Behavior"), DefaultValue(DateOnError.Undo), AutoFormatDisable]
		public DateOnError DateOnError {
			get { return Properties.DateOnError; }
			set { Properties.DateOnError = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditAllowNull"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool AllowNull {
			get { return Properties.AllowNull; }
			set { Properties.AllowNull = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditPopupCalendarOwnerID"),
#endif
		Category("Behavior"), DefaultValue(""), AutoFormatDisable, Localizable(false),
		IDReferenceProperty(typeof(ASPxDateEdit)), TypeConverter(typeof(ComponentIDConverter<ASPxDateEdit>))]
		public string PopupCalendarOwnerID {
			get { return GetStringProperty(PopupCalendarOwnerIDName, string.Empty); }
			set {
				if(value == PopupCalendarOwnerClientID)
					return;
				SetStringProperty(PopupCalendarOwnerIDName, string.Empty, value);
				LayoutChanged();
				this.popupCalendarOwner = null;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditUseMaskBehavior"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool UseMaskBehavior {
			get { return Properties.UseMaskBehavior; }
			set { Properties.UseMaskBehavior = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditMinDate"),
#endif
		DefaultValue(typeof(DateTime), ""), AutoFormatDisable]
		public DateTime MinDate {
			get { return Properties.MinDate; }
			set { Properties.MinDate = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditMaxDate"),
#endif
		DefaultValue(typeof(DateTime), ""), AutoFormatDisable]
		public DateTime MaxDate {
			get { return Properties.MaxDate; }
			set { Properties.MaxDate = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditShowOutOfRangeWarning"),
#endif
		DefaultValue(true), AutoFormatDisable]
		public bool ShowOutOfRangeWarning {
			get { return Properties.ShowOutOfRangeWarning; }
			set { Properties.ShowOutOfRangeWarning = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditDisabledDates"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DefaultValue(typeof(DateTimeCollection), ""), AutoFormatDisable, Browsable(false)]
		public DateTimeCollection DisabledDates {
			get { return Properties.DisabledDates; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditEditFormat"),
#endif
		DefaultValue(EditFormat.Date), AutoFormatDisable]
		public EditFormat EditFormat {
			get { return Properties.EditFormat; }
			set { Properties.EditFormat = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditEditFormatString"),
#endif
		DefaultValue(""), Localizable(true), AutoFormatDisable]
		public string EditFormatString {
			get { return Properties.EditFormatString; }
			set { Properties.EditFormatString = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxDateEditValue")]
#endif
		public override object Value {
			get { return base.Value; }
			set { base.Value = DateTimeEditUtils.PreprocessValueProperty(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditDate"),
#endif
		DefaultValue(typeof(DateTime), ""), AutoFormatDisable]
		public DateTime Date {
			get { return DateTimeEditUtils.GetDateProperty(this); }
			set { DateTimeEditUtils.SetDateProperty(this, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Bindable(false), AutoFormatDisable, Localizable(false)]
		public override string Text {
			get { return DateTimeEditUtils.GetTextProperty(this, Properties.GetDateFormatString(), base.Text); }
			set { DateTimeEditUtils.SetTextProperty(this, value, Properties.GetDateFormatString()); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int MaxLength {
			get { return 0; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditCalendarProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DateEditCalendarProperties CalendarProperties {
			get { return Properties.CalendarProperties; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditTimeSectionProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DateEditTimeSectionProperties TimeSectionProperties {
			get { return Properties.TimeSectionProperties; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditDateRangeSettings"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DateEditRangeSettings DateRangeSettings {
			get { return Properties.DateRangeSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditMaskHintStyle"),
#endif
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public MaskHintStyle MaskHintStyle {
			get { return Properties.MaskHintStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string NullText {
			get { return Properties.NullText; }
			set { Properties.NullText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new DateEditClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditDateChanged"),
#endif
		Category("Action")]
		public event EventHandler DateChanged
		{
			add { Events.AddHandler(EventDateChanged, value); }
			remove { Events.RemoveHandler(EventDateChanged, value); }
		}
		internal event EventHandler<PostBackValueSecuringEventArgs> PostBackValueSecuring {
			add { Events.AddHandler(EventPostBackValueSecuring, value); }
			remove { Events.RemoveHandler(EventPostBackValueSecuring, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditCalendarCustomDisabledDate"),
#endif
		Category("Rendering")]
		public event EventHandler<CalendarCustomDisabledDateEventArgs> CalendarCustomDisabledDate
		{
			add { Events.AddHandler(EventCalendarCutomDisabledDate, value); }
			remove { Events.RemoveHandler(EventCalendarCutomDisabledDate, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditCalendarDayCellInitialize"),
#endif
		Category("Rendering")]
		public event EventHandler<CalendarDayCellInitializeEventArgs> CalendarDayCellInitialize
		{
			add { Events.AddHandler(EventCalendarDayCellInitialize, value); }
			remove { Events.RemoveHandler(EventCalendarDayCellInitialize, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditCalendarDayCellCreated"),
#endif
		Category("Rendering")]
		public event EventHandler<CalendarDayCellCreatedEventArgs> CalendarDayCellCreated
		{
			add { Events.AddHandler(EventCalendarDayCellCreated, value); }
			remove { Events.RemoveHandler(EventCalendarDayCellCreated, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDateEditCalendarDayCellPrepared"),
#endif
		Category("Rendering")]
		public event EventHandler<CalendarDayCellPreparedEventArgs> CalendarDayCellPrepared
		{
			add { Events.AddHandler(EventCalendarDayCellPrepared, value); }
			remove { Events.RemoveHandler(EventCalendarDayCellPrepared, value); }
		}
		internal EventHandler<CalendarCustomDisabledDateEventArgs> CalendarCutomDisabledDateHandler {
			get { return Events[EventCalendarCutomDisabledDate] as EventHandler<CalendarCustomDisabledDateEventArgs>; }
		}
		internal EventHandler<CalendarDayCellInitializeEventArgs> CalendarDayCellInitializeHandler {
			get { return Events[EventCalendarDayCellInitialize] as EventHandler<CalendarDayCellInitializeEventArgs>; }
		}
		internal EventHandler<CalendarDayCellCreatedEventArgs> CalendarDayCellCreatedHandler {
			get { return Events[EventCalendarDayCellCreated] as EventHandler<CalendarDayCellCreatedEventArgs>; }
		}
		internal EventHandler<CalendarDayCellPreparedEventArgs> CalendarDayCellPreparedHandler {
			get { return Events[EventCalendarDayCellPrepared] as EventHandler<CalendarDayCellPreparedEventArgs>; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler TextChanged {
			add { }
			remove { }
		}
		protected internal new DateEditProperties Properties {
			get { return base.Properties as DateEditProperties; }
		}
		protected ASPxDateEdit PopupCalendarOwner {
			get {
				if(DesignMode || String.IsNullOrEmpty(PopupCalendarOwnerID))
					return null;
				if(popupCalendarOwner == null)
					popupCalendarOwner = FindControlHelper.LookupControl(this, PopupCalendarOwnerID) as ASPxDateEdit;
				return popupCalendarOwner;
			}
		}
		protected virtual string PopupCalendarOwnerClientID {
			get { return (PopupCalendarOwner != null) ? PopupCalendarOwner.ClientID : string.Empty; }
		}
		protected internal override bool IsPopupControlShared {
			get { return !string.IsNullOrEmpty(PopupCalendarOwnerClientID); }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new DateEditProperties(this);
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new DateEditControl(this);
		}
		protected override string GetFormattedInputText() {
			if(Date == DateTime.MinValue)
				return "";
			return String.Format(CommonUtils.GetFormatString(DisplayFormatString), Date);
		}
		protected override bool IsPostBackValueSecure(object value) {
			bool result = base.IsPostBackValueSecure(value);
			if (value != null) {
				PostBackValueSecuringEventArgs args = new PostBackValueSecuringEventArgs();
				RaisePostBackValueSecuring(args);
				bool isValidClientDateRange = StartDateEdit == null || IsValidClientDateRange(StartDateEdit);
				result = result && ASPxCalendar.IsDateInRange((DateTime)value, MinDate, MaxDate) && args.IsValidValue && isValidClientDateRange;
			}
			return result;
		}
		protected virtual void OnPostBackValueSecuring(object sender, PostBackValueSecuringEventArgs e) {
			e.IsValidValue = IsValidClientDateRange((ASPxDateEdit)sender);
		}
		protected bool IsValidClientDateRange(ASPxDateEdit startDateEdit) {
			NameValueCollection postCollection = Request.Params;
			object clientStartDate = startDateEdit.GetPostBackValue(startDateEdit.UniqueID, postCollection);
			object clientEndDate = GetPostBackValue(UniqueID, postCollection);
			return clientStartDate == null || clientEndDate == null 
				|| DateRangeValidator.IsValueValid((DateTime)clientStartDate, (DateTime)clientEndDate, DateRangeSettings);
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			EnsureStartDateEdit(false);
		}
		protected void EnsureStartDateEdit(bool force) {
			if (NeedUpdateStartDateEdit(force)) {
				if (this.startDateEdit != null) {
					this.startDateEdit.EndDateEdit = null;
					this.startDateEdit.PostBackValueSecuring -= OnPostBackValueSecuring;
					this.startDateEdit = null;
				}
				ASPxDateEdit startDateEdit = FindControlHelper.LookupControl(this, DateRangeSettings.StartDateEditID) as ASPxDateEdit;
				if (startDateEdit != null && startDateEdit.ID != ID) {
					this.startDateEdit = startDateEdit;
					this.startDateEdit.EndDateEdit = this;
					this.startDateEdit.PostBackValueSecuring += OnPostBackValueSecuring;
				}
			}
		}
		protected ASPxDateEdit StartDateEdit {
			get {
				EnsureStartDateEdit(false);
				return this.startDateEdit;
			}
		}
		protected ASPxDateEdit EndDateEdit { get; set; }
		protected virtual string StartDateEditClientID {
			get { return StartDateEdit != null ? StartDateEdit.ClientID : ""; }
		}
		protected bool NeedUpdateStartDateEdit(bool force) {
			if (force && !string.IsNullOrEmpty(DateRangeSettings.StartDateEditID))
				return true;
			return this.startDateEdit == null && !string.IsNullOrEmpty(DateRangeSettings.StartDateEditID)
				|| this.startDateEdit != null && this.startDateEdit.ID != DateRangeSettings.StartDateEditID;
		}
		protected override void BeforeRender() {
			EnsureStartDateEdit(true);
			ApplyDateRangeCalendarColumnCount(CalendarProperties);
			base.BeforeRender();
		}
		protected internal virtual void ApplyDateRangeCalendarColumnCount(CalendarProperties targetCalendarProperties) {
			if(EndDateEdit != null)
				targetCalendarProperties.Columns = EndDateEdit.DateRangeSettings.CalendarColumnCount;
			else if(!string.IsNullOrEmpty(StartDateEditClientID))
				targetCalendarProperties.Columns = DateRangeSettings.CalendarColumnCount;
		}
		protected override bool HasValidationPatterns {
			get { return base.HasValidationPatterns
					|| !string.IsNullOrEmpty(StartDateEditClientID) && DateRangeSettings.HasDayCountRestrictions();
			}
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxDateEdit), DateEditScriptResourceName);
			if(!IsMaskCapabilitiesEnabled())
				RegisterDateFormatterScript();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientDateEdit";
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			RegisterCultureInfoScript();
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);			
			if(Date > DateTime.MinValue)
				stb.Append(localVarName + ".date = " + HtmlConvertor.ToScript(Date) + ";\n");
			if(DateOnError != DateOnError.Undo)
				stb.Append(localVarName + ".dateOnError = " + HtmlConvertor.ToScript(GetDateOnErrorCode()) + ";\n");
			if(!AllowNull)
				stb.AppendFormat("{0}.allowNull = false;\n", localVarName);
			if(!IsMaskCapabilitiesEnabled() || ShowOutOfRangeWarning) {
				stb.AppendFormat("{0}.dateFormatter = ASPx.DateFormatter.Create({1});\n", localVarName, HtmlConvertor.ToScript(Properties.GetDateFormatString()));
			}
			if(IsPopupControlShared)
				stb.AppendFormat("{0}.calendarOwnerName = '{1}';\n", localVarName, PopupCalendarOwnerClientID);
			if(TimeSectionProperties.Visible)
				stb.AppendFormat("{0}.showTimeSection = true;\n", localVarName);
			if (!string.IsNullOrEmpty(StartDateEditClientID)) {
				stb.AppendFormat("{0}.startDateEditName = '{1}';\n", localVarName, StartDateEditClientID);
				if(DateRangeSettings.MinDayCount > 0)
					stb.AppendFormat("{0}.minRange = {1};\n", localVarName, HtmlConvertor.ToScript(DateRangeSettings.GetMinRange().TotalMilliseconds));
				if(DateRangeSettings.MaxDayCount > 0)
					stb.AppendFormat("{0}.maxRange = {1};\n", localVarName, HtmlConvertor.ToScript(DateRangeSettings.GetMaxRange().TotalMilliseconds));
				if(DateRangeSettings.HasDayCountRestrictions())
					stb.AppendFormat("{0}.invalidDateRangeErrorText = '{1}';\n", localVarName, DateRangeSettings.GetErrorText());
			}
		}
		protected override bool IsOutOfRangeWarningSupported() {
			return true;
		}
		protected override bool IsOutOfRangeWarningMode() {
			return ShowOutOfRangeWarning;
		}
		protected override string GetInvalidValueRangeWarningMessage() {
			return ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.InvalidDateEditRange);
		}
		protected override string GetInvalidMinValueWarningMessage() {
			return ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.InvalidDateEditMinDate);
		}
		protected override string GetInvalidMaxValueWarningMessage() {
			return ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.InvalidDateEditMaxDate);
		}
		protected void ValidateDateRangeSettings() {
			if (StartDateEdit != null && EndDateEdit != null)
				throw new ArgumentException(StringResources.ASPxDateEdit_StartDateEditChain, DateEditRangeSettings.StartDateEditIDName);
			if (DateRangeSettings.GetMinRange() > DateRangeSettings.GetMaxRange())
				throw new ArgumentOutOfRangeException("MaxDayCount",
					string.Format(StringResources.InvalidMinimumValue, "MaxDayCount", "MinDayCount"));
		}
		protected internal override void ValidateProperties() {
			base.ValidateProperties();
			if(StartDateEdit != null)
				ValidateDateRangeSettings();
			string id = PopupCalendarOwnerID;
			if(String.IsNullOrEmpty(id))
				return;
			if(PopupCalendarOwner == null)
				throw new ArgumentNullException(PopupCalendarOwnerIDName, StringResources.ASPxDateEdit_CannotFindPopupCalendarOwner);
			if(!String.IsNullOrEmpty(PopupCalendarOwner.PopupCalendarOwnerID))
				throw new ArgumentException(StringResources.ASPxDateEdit_PopupCalendarOwnerChain, PopupCalendarOwnerIDName);
		}
		protected override object GetPostBackValue(string controlName, System.Collections.Specialized.NameValueCollection postCollection) {
			if(ClientObjectState == null)
				return Value;
			return DateTimeEditUtils.ParseRawInputValue(GetClientObjectStateValueString(RawValueKey));
		}
		protected override bool AreEditorValuesEqual(object v1, object v2, bool convertEmptyStringToNull) {
			if(convertEmptyStringToNull && CommonUtils.IsNullOrEmpty(v1) && CommonUtils.IsNullOrEmpty(v2))
				return true;
			if(!(v1 is DateTime && v2 is DateTime))
				return base.AreEditorValuesEqual(v1, v2, convertEmptyStringToNull);
			DateTime date1 = (DateTime)v1;
			DateTime date2 = (DateTime)v2;
			int timeMilliseconds1 = (int)Math.Floor(date1.TimeOfDay.TotalMilliseconds);
			int timeMilliseconds2 = (int)Math.Floor(date2.TimeOfDay.TotalMilliseconds);
			return date1.Date == date2.Date && timeMilliseconds1 == timeMilliseconds2;
		}
		protected override bool RequireRenderRawInput() {
			return true;
		}
		protected override string GetRawValue() {
			return DateTimeEditUtils.GetRawValue(Date);
		}
		protected internal string GetDateOnErrorCode() {
			switch(DateOnError) {
				case DateOnError.Null:
					return "n";
				case DateOnError.Today:
					return "t";
				default:
					return "u";
			}
		}
		protected void OnDateChanged(EventArgs e) {
			EventHandler handler = Events[EventDateChanged] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected void RaisePostBackValueSecuring(PostBackValueSecuringEventArgs e) {
			EventHandler<PostBackValueSecuringEventArgs> handler = Events[EventPostBackValueSecuring] as EventHandler<PostBackValueSecuringEventArgs>;
			if (handler != null)
				handler(this, e);
		}
		protected override void RaiseValueChanged() {
			base.RaiseValueChanged();
			OnDateChanged(EventArgs.Empty);
		}
		protected override bool IsMaskValidationPatternRequired() {
			return false;
		}
		bool ITimeSectionOwner.ShowTimeSection { get { return TimeSectionProperties.Visible; } }
		bool ITimeSectionOwner.ShowOkButton { get { return TimeSectionProperties.ShowOkButton; } }
		bool ITimeSectionOwner.ShowCancelButton { get { return TimeSectionProperties.ShowCancelButton; } }
		bool ITimeSectionOwner.ShowHourHand { get { return TimeSectionProperties.ShowHourHand; } }
		bool ITimeSectionOwner.ShowMinuteHand { get { return TimeSectionProperties.ShowMinuteHand; } }
		bool ITimeSectionOwner.ShowSecondHand { get { return TimeSectionProperties.ShowSecondHand; } }
		string ITimeSectionOwner.OkButtonText { get { return TimeSectionProperties.OkButtonText; } }
		string ITimeSectionOwner.CancelButtonText { get { return TimeSectionProperties.CancelButtonText; } }
		string ITimeSectionOwner.OkButtonClickScript { get { return OkClickHandlerName; } }
		string ITimeSectionOwner.CancelButtonClickScript { get { return CancelClickHandlerName; } }
		string ITimeSectionOwner.ClearButtonClickScript { get { return ClearClickHandlerName; } }
		string ITimeSectionOwner.TimeEditKeyDownScript { get { return TimeSectionTimeEditKeyDownHandlerName; } }
		string ITimeSectionOwner.TimeEditLostFocusScript { get { return TimeSectionTimeEditLostFocusHandlerName; } }
		string ITimeSectionOwner.TimeSectionFooterClassName { get { return TimeSectionFooterClassName; } }
		string ITimeSectionOwner.TimeSectionHeaderClassName { get { return TimeSectionHeaderClassName; } }
		DateEditTimeEditProperties ITimeSectionOwner.TimeEditProperties { get { return TimeSectionProperties.TimeEditProperties; } }
		ImageProperties ITimeSectionOwner.ClockFaceImage { get { return RenderImages.GetImageProperties(Page, EditorImages.DateEditTimeSectionClockFaceImageName); } }
		ImageProperties ITimeSectionOwner.HourHandImage { get { return RenderImages.GetImageProperties(Page, EditorImages.DateEditTimeSectionHourHandImageName); } }
		ImageProperties ITimeSectionOwner.MinuteHandImage { get { return RenderImages.GetImageProperties(Page, EditorImages.DateEditTimeSectionMinuteHandImageName); } }
		ImageProperties ITimeSectionOwner.SecondHandImage { get { return RenderImages.GetImageProperties(Page, EditorImages.DateEditTimeSectionSecondHandImageName); } }
		DateEditTimeSectionCellStyle ITimeSectionOwner.TimeEditCellStyle { get { return GetTimeEditCellStyle(); } }
		DateEditTimeSectionCellStyle ITimeSectionOwner.ClockCellStyle { get { return GetClockCellStyle(); } }
		protected internal DateEditTimeSectionCellStyle GetTimeEditCellStyle() {
			return GetTimeSectionCellStyle(EditorStyles.DateEditTimeEditCellStyleName, RenderStyles.DateEditTimeEditCell);
		}
		protected internal DateEditTimeSectionCellStyle GetClockCellStyle() {
			return GetTimeSectionCellStyle(EditorStyles.DateEditClockCellStyleName, RenderStyles.DateEditClockCell);
		}
		protected DateEditTimeSectionCellStyle GetTimeSectionCellStyle(string styleName, DateEditTimeSectionCellStyle renderStyle) {
			DateEditTimeSectionCellStyle style = new DateEditTimeSectionCellStyle();
			style.CopyFrom(RenderStyles.CreateStyleByName(styleName));
			style.CopyFrom(renderStyle);
			MergeDisableStyle(style);
			return style;
		}
	}
}
namespace DevExpress.Web.Internal {
	public class DateRangeValidator {
		public static bool IsValueValid(DateTime startDate, DateTime endDate, DateEditRangeSettings settings) {
			return startDate == null || endDate == null ||
				   endDate - startDate >= settings.GetMinRange() && endDate - startDate <= settings.GetMaxRange();
		}
	}
}

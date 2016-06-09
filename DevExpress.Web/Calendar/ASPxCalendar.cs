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

using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	public class CalendarProperties : EditProperties {
		private CalendarFastNavProperties fastNavProperties = null;
		private DateTimeCollection disabledDates = null;
		public CalendarProperties()
			: this(null) {
		}
		public CalendarProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesDisplayFormatString"),
#endif
		NotifyParentProperty(true), DefaultValue("d"), AutoFormatDisable]
		public override string DisplayFormatString {
			get { return base.DisplayFormatString; }
			set { base.DisplayFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesClearButtonText"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.Calendar_Clear),
		AutoFormatDisable, Localizable(true)]
		public virtual string ClearButtonText {
			get { return GetStringProperty("ClearButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_Clear)); }
			set {
				SetStringProperty("ClearButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_Clear), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesTodayButtonText"),
#endif
		NotifyParentProperty(true), DefaultValue(StringResources.Calendar_Today),
		AutoFormatDisable, Localizable(true)]
		public string TodayButtonText {
			get { return GetStringProperty("TodayButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_Today)); }
			set {
				SetStringProperty("TodayButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_Today), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesShowClearButton"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public virtual bool ShowClearButton {
			get { return GetBoolProperty("ShowClearButton", true); }
			set {
				SetBoolProperty("ShowClearButton", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesShowTodayButton"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowTodayButton {
			get { return GetBoolProperty("ShowTodayButton", true); }
			set {
				SetBoolProperty("ShowTodayButton", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesShowHeader"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowHeader {
			get { return GetBoolProperty("ShowHeader", true); }
			set {
				if(ShowHeader != value) {
					SetBoolProperty("ShowHeader", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesShowDayHeaders"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowDayHeaders {
			get { return GetBoolProperty("ShowDayHeaders", true); }
			set {
				if(value != ShowDayHeaders) {
					SetBoolProperty("ShowDayHeaders", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesShowWeekNumbers"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ShowWeekNumbers {
			get { return GetBoolProperty("ShowWeekNumbers", true); }
			set {
				if(value != ShowWeekNumbers) {
					SetBoolProperty("ShowWeekNumbers", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesDayNameFormat"),
#endif
		NotifyParentProperty(true), DefaultValue(DayNameFormat.Short), AutoFormatDisable]
		public DayNameFormat DayNameFormat {
			get { return (DayNameFormat)GetEnumProperty("DayNameFormat", DayNameFormat.Short); }
			set {
				if(DayNameFormat != value) {
					SetEnumProperty("DayNameFormat", DayNameFormat.Short, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesHighlightWeekends"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public bool HighlightWeekends {
			get { return GetBoolProperty("HighlightWeekends", true); }
			set {
				SetBoolProperty("HighlightWeekends", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesHighlightToday"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public bool HighlightToday {
			get { return GetBoolProperty("HighlightToday", true); }
			set {
				SetBoolProperty("HighlightToday", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesShowShadow"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(true)]
		public bool ShowShadow {
			get { return GetBoolProperty("ShowShadow", true); }
			set {
				SetBoolProperty("ShowShadow", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesEnableMonthNavigation"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool EnableMonthNavigation {
			get { return GetBoolProperty("EnableMonthNavigation", true); }
			set {
				if(EnableMonthNavigation != value) {
					SetBoolProperty("EnableMonthNavigation", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesEnableYearNavigation"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool EnableYearNavigation {
			get { return GetBoolProperty("EnableYearNavigation", true); }
			set {
				if(EnableYearNavigation != value) {
					SetBoolProperty("EnableYearNavigation", true, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFirstDayOfWeek"),
#endif
		NotifyParentProperty(true), DefaultValue(FirstDayOfWeek.Default), AutoFormatDisable]
		public FirstDayOfWeek FirstDayOfWeek {
			get { return (FirstDayOfWeek)GetEnumProperty("FirstDayOfWeek", FirstDayOfWeek.Default); }
			set {
				if(FirstDayOfWeek != value) {
					SetEnumProperty("FirstDayOfWeek", FirstDayOfWeek.Default, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesEnableMultiSelect"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public virtual bool EnableMultiSelect {
			get { return GetBoolProperty("EnableMultiSelect", false); }
			set { SetBoolProperty("EnableMultiSelect", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesMinDate"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(DateTime), "")]
		public virtual DateTime MinDate {
			get { return (DateTime)GetObjectProperty("MinDate", DateTime.MinValue); }
			set { SetObjectProperty("MinDate", DateTime.MinValue, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesMaxDate"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(DateTime), "")]
		public virtual DateTime MaxDate {
			get { return (DateTime)GetObjectProperty("MaxDate", DateTime.MinValue); }
			set { SetObjectProperty("MaxDate", DateTime.MinValue, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesDisabledDates"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(typeof(DateTimeCollection), "")]
		public virtual DateTimeCollection DisabledDates {
			get {
				if(disabledDates == null)
					disabledDates = new DateTimeCollection();
				return disabledDates;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesChangeVisibleDateAnimationType"),
#endif
		NotifyParentProperty(true), DefaultValue(AnimationType.Auto), AutoFormatDisable]
		public AnimationType ChangeVisibleDateAnimationType {
			get { return (AnimationType)GetEnumProperty("ChangeVisibleDateAnimationType", AnimationType.Auto); }
			set {
				if(ChangeVisibleDateAnimationType != value) {
					SetEnumProperty("ChangeVisibleDateAnimationType", AnimationType.Auto, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesEnableChangeVisibleDateGestures"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean EnableChangeVisibleDateGestures {
			get { return (DefaultBoolean)GetEnumProperty("EnableChangeVisibleDateGestures", DefaultBoolean.Default); }
			set {
				if(EnableChangeVisibleDateGestures != value) {
					SetEnumProperty("EnableChangeVisibleDateGestures", DefaultBoolean.Default, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesMonthGridPaddings"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings MonthGridPaddings {
			get { return Styles.CalendarMonthGridPaddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesColumns"),
#endif
		NotifyParentProperty(true), DefaultValue(1), AutoFormatDisable]
		public virtual int Columns {
			get { return GetIntProperty("Columns", 1); }
			set {
				value = GetCorrectedColRowValue(value);
				if(Columns != value) {
					SetIntProperty("Columns", 1, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesRows"),
#endif
		NotifyParentProperty(true), DefaultValue(1), AutoFormatDisable]
		public virtual int Rows {
			get { return GetIntProperty("Rows", 1); }
			set {
				value = GetCorrectedColRowValue(value);
				if(Rows != value) {
					SetIntProperty("Rows", 1, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesPrevYearImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImagePropertiesEx PrevYearImage {
			get { return Images.CalendarPrevYear; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesPrevMonthImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImagePropertiesEx PrevMonthImage {
			get { return Images.CalendarPrevMonth; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesNextMonthImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImagePropertiesEx NextMonthImage {
			get { return Images.CalendarNextMonth; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesNextYearImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImagePropertiesEx NextYearImage {
			get { return Images.CalendarNextYear; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFastNavPrevYearImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties FastNavPrevYearImage {
			get { return Images.CalendarFastNavPrevYear; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFastNavNextYearImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties FastNavNextYearImage {
			get { return Images.CalendarFastNavNextYear; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesLoadingPanelImage"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties LoadingPanelImage {
			get { return Images.LoadingPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesDayHeaderStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayHeaderStyle {
			get { return Styles.CalendarDayHeader; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesWeekNumberStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle WeekNumberStyle {
			get { return Styles.CalendarWeekNumber; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesDayStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayStyle {
			get { return Styles.CalendarDay; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesDaySelectedStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DaySelectedStyle {
			get { return Styles.CalendarDaySelected; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesDayOtherMonthStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayOtherMonthStyle {
			get { return Styles.CalendarDayOtherMonth; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesDayWeekendStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayWeekendStyle {
			get { return Styles.CalendarDayWeekEnd; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesDayOutOfRangeStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayOutOfRangeStyle {
			get { return Styles.CalendarDayOutOfRange; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesDayDisabledStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayDisabledStyle {
			get { return Styles.CalendarDayDisabled; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesTodayStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle TodayStyle {
			get { return Styles.CalendarToday; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesButtonStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditButtonStyle ButtonStyle {
			get { return Styles.CalendarButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesHeaderStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarHeaderFooterStyle HeaderStyle {
			get { return Styles.CalendarHeader; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFooterStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarHeaderFooterStyle FooterStyle {
			get { return Styles.CalendarFooter; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFastNavStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarFastNavStyle FastNavStyle {
			get { return Styles.CalendarFastNav; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFastNavMonthAreaStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle FastNavMonthAreaStyle {
			get { return Styles.CalendarFastNavMonthArea; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFastNavYearAreaStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle FastNavYearAreaStyle {
			get { return Styles.CalendarFastNavYearArea; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFastNavMonthStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarFastNavItemStyle FastNavMonthStyle {
			get { return Styles.CalendarFastNavMonth; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFastNavYearStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarFastNavItemStyle FastNavYearStyle {
			get { return Styles.CalendarFastNavYear; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFastNavFooterStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarHeaderFooterStyle FastNavFooterStyle {
			get { return Styles.CalendarFastNavFooter; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesClientSideEvents"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), AutoFormatDisable]
		public new CalendarClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as CalendarClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarPropertiesFastNavProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), AutoFormatDisable]
		public CalendarFastNavProperties FastNavProperties {
			get {
				if(fastNavProperties == null)
					fastNavProperties = new CalendarFastNavProperties(this);
				return fastNavProperties;
			}
		}
		protected override string DefaultDisplayFormatString {
			get { return "d"; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				CalendarProperties src = source as CalendarProperties;
				if (src != null) {
					ClearButtonText = src.ClearButtonText;
					TodayButtonText = src.TodayButtonText;
					ShowClearButton = src.ShowClearButton;
					ShowTodayButton = src.ShowTodayButton;
					EnableMonthNavigation = src.EnableMonthNavigation;
					EnableYearNavigation = src.EnableYearNavigation;
					EnableChangeVisibleDateGestures = src.EnableChangeVisibleDateGestures;
					ShowHeader = src.ShowHeader;
					ShowDayHeaders = src.ShowDayHeaders;
					ShowWeekNumbers = src.ShowWeekNumbers;
					ChangeVisibleDateAnimationType = src.ChangeVisibleDateAnimationType;
					DayNameFormat = src.DayNameFormat;
					FirstDayOfWeek = src.FirstDayOfWeek;
					MonthGridPaddings.Assign(src.MonthGridPaddings);
					Columns = src.Columns;
					Rows = src.Rows;
					EnableMultiSelect = src.EnableMultiSelect;
					FastNavProperties.Assign(src.FastNavProperties);
					MinDate = src.MinDate;
					MaxDate = src.MaxDate;
					DisabledDates.Assign(src.DisabledDates);
					HighlightWeekends = src.HighlightWeekends;
					HighlightToday = src.HighlightToday;
					ShowShadow = src.ShowShadow;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new CalendarClientSideEvents(this);
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxCalendar();
		}
		protected override string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			if (args.DisplayText == null && args.EditValue is DateTime) {
				DateTime date = (DateTime)args.EditValue;
				if (date == DateTime.MinValue)
					args.EditValue = null;
			}
			return base.GetDisplayTextCore(args, encode);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { FastNavProperties, DisabledDates });
		}
		protected internal bool HasVisibleButtons() {
			return ShowTodayButton || ShowClearButton;
		}
		protected int GetCorrectedColRowValue(int value) {
			if (value < 1)
				value = 1;
			return value;
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DefaultProperty("SelectedDate"), DefaultEvent("SelectionChanged"),
	Designer("DevExpress.Web.Design.ASPxCalendarDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxCalendar.bmp")
	]
	public class ASPxCalendar : ASPxEdit {
		protected internal const string CalendarScriptResourceName = EditScriptsResourcePath + "Calendar.js";
		private const string NavigationClickHandlerName = "ASPx.CalShiftMonth('{0}', {1});";
		private const string TodayClickHandlerName = "ASPx.CalTodayClick('{0}');";
		private const string ClearClickHandlerName = "ASPx.CalClearClick('{0}');";
		private const string TitleClickHandlerName = "ASPx.CalTitleClick('{0}', {1}, {2})";
		private const string FastNavYearShuffleHandlerName = "ASPx.CalFNYShuffle('{0}', {1})";
		private const string FastNavButtonClickHandlerName = "ASPx.CalFNBClick('{0}', '{1}')";
		protected internal static string PrevYearCellIdSufix = "PYC";
		protected internal static string PrevMonthCellIdSufix = "PMC";
		protected internal static string NextMonthCellIdSufix = "NMC";
		protected internal static string NextYearCellIdSufix = "NYC";
		protected internal static string ButtonImageIdPostfix = "Img";
		private CalendarSelection selectedDates;
		private bool visibleMonthChanged = false;
		private bool selectionChanged = false;
		private bool isDateEditCalendar = false;
		ITimeSectionOwner timeSectionOwner;
		static readonly object EventVisibleMonthChanged = new object();
		static readonly object EventSelectionChanged = new object();
		static readonly object EventCustomDisabledDate = new object();
		static readonly object EventDayCellInitialize = new object();
		static readonly object EventDayCellCreated = new object();
		static readonly object EventDayCellPrepared = new object();
		public ASPxCalendar()
			: base() {
		}
		protected internal ASPxCalendar(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarVisibleDate"),
#endif
		DefaultValue(typeof(DateTime), ""), Bindable(true), AutoFormatDisable]
		public DateTime VisibleDate {
			get { return (DateTime)GetObjectProperty("VisibleDate", DateTime.MinValue); }
			set {
				SetObjectProperty("VisibleDate", DateTime.MinValue, value.Date);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarSelectedDate"),
#endif
		DefaultValue(typeof(DateTime), ""), AutoFormatDisable]
		public DateTime SelectedDate {
			get { return SelectedDates.GetFirstDate(); }
			set {
				value = value.Date;
				SelectedDates.Clear();
				if(value > DateTime.MinValue) {
					if(DesignMode)
						value = FitDateToRange(value);
					SelectedDates.Add(value);
				}
				LayoutChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CalendarSelection SelectedDates {
			get {
				if (selectedDates == null)
					selectedDates = new CalendarSelection(this);
				return selectedDates;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxCalendarValue")]
#endif
		public override object Value {
			get {
				if(SelectedDate == DateTime.MinValue)
					return null;
				return SelectedDate;
			}
			set {
				if(CommonUtils.IsNullValue(value))
					SelectedDate = DateTime.MinValue;
				else if(value is DateTime)
					SelectedDate = (DateTime)value;
				else if(value is String) {
					string str = (String)value;
					if(str == "")
						SelectedDate = DateTime.MinValue;
					else
						SelectedDate = DateTime.ParseExact(str, "d", CultureInfo.InvariantCulture);
				}
				else
					throw new ArgumentException();
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarShowClearButton"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool ShowClearButton {
			get { return Properties.ShowClearButton; }
			set { Properties.ShowClearButton = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarShowTodayButton"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool ShowTodayButton {
			get { return Properties.ShowTodayButton; }
			set { Properties.ShowTodayButton = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarShowHeader"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool ShowHeader {
			get { return Properties.ShowHeader; }
			set { Properties.ShowHeader = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarShowDayHeaders"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool ShowDayHeaders {
			get { return Properties.ShowDayHeaders; }
			set { Properties.ShowDayHeaders = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarShowWeekNumbers"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool ShowWeekNumbers {
			get { return Properties.ShowWeekNumbers; }
			set { Properties.ShowWeekNumbers = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDayNameFormat"),
#endif
		Category("Appearance"), DefaultValue(DayNameFormat.Short), AutoFormatDisable]
		public DayNameFormat DayNameFormat {
			get { return Properties.DayNameFormat; }
			set { Properties.DayNameFormat = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarHighlightWeekends"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool HighlightWeekends {
			get { return Properties.HighlightWeekends; }
			set { Properties.HighlightWeekends = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarHighlightToday"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool HighlightToday {
			get { return Properties.HighlightToday; }
			set { Properties.HighlightToday = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarShowShadow"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool ShowShadow {
			get { return Properties.ShowShadow; }
			set { Properties.ShowShadow = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarEnableMonthNavigation"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableMonthNavigation {
			get { return Properties.EnableMonthNavigation; }
			set { Properties.EnableMonthNavigation = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarEnableYearNavigation"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableYearNavigation {
			get { return Properties.EnableYearNavigation; }
			set { Properties.EnableYearNavigation = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarEncodeHtml"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public override bool EncodeHtml {
			get { return Properties.EncodeHtml; }
			set { Properties.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFirstDayOfWeek"),
#endif
		Category("Behavior"), DefaultValue(FirstDayOfWeek.Default), AutoFormatDisable]
		public FirstDayOfWeek FirstDayOfWeek {
			get { return Properties.FirstDayOfWeek; }
			set { Properties.FirstDayOfWeek = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarEnableMultiSelect"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableMultiSelect {
			get { return Properties.EnableMultiSelect; }
			set { Properties.EnableMultiSelect = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxCalendarReadOnly")]
#endif
		public override bool ReadOnly {
			get { return base.ReadOnly; }
			set {
				base.ReadOnly = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarChangeVisibleDateAnimationType"),
#endif
		Category("Behavior"), DefaultValue(AnimationType.Auto), AutoFormatDisable]
		public AnimationType ChangeVisibleDateAnimationType {
			get { return Properties.ChangeVisibleDateAnimationType; }
			set { Properties.ChangeVisibleDateAnimationType = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarEnableChangeVisibleDateGestures"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean EnableChangeVisibleDateGestures {
			get { return Properties.EnableChangeVisibleDateGestures; }
			set { Properties.EnableChangeVisibleDateGestures = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarPrevYearImage"),
#endif
		Category("Images"), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImagePropertiesEx PrevYearImage {
			get { return (ImagePropertiesEx)Properties.PrevYearImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarPrevMonthImage"),
#endif
		Category("Images"), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImagePropertiesEx PrevMonthImage {
			get { return (ImagePropertiesEx)Properties.PrevMonthImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarNextMonthImage"),
#endif
		Category("Images"), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImagePropertiesEx NextMonthImage {
			get { return (ImagePropertiesEx)Properties.NextMonthImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarNextYearImage"),
#endif
		Category("Images"), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImagePropertiesEx NextYearImage {
			get { return (ImagePropertiesEx)Properties.NextYearImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFastNavPrevYearImage"),
#endif
		Category("Images"), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties FastNavPrevYearImage {
			get { return Properties.FastNavPrevYearImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFastNavNextYearImage"),
#endif
		Category("Images"), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties FastNavNextYearImage {
			get { return Properties.FastNavNextYearImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarLoadingPanelImage"),
#endif
		Category("Images"), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new ImageProperties LoadingPanelImage {
			get { return Properties.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarMonthGridPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings MonthGridPaddings {
			get { return Properties.MonthGridPaddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[Browsable(false)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarColumns"),
#endif
		Category("Layout"), DefaultValue(1), AutoFormatDisable]
		public int Columns {
			get { return Properties.Columns; }
			set { Properties.Columns = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarRows"),
#endif
		Category("Layout"), DefaultValue(1), AutoFormatDisable]
		public int Rows {
			get { return Properties.Rows; }
			set { Properties.Rows = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarClearButtonText"),
#endif
		DefaultValue(StringResources.Calendar_Clear), Localizable(true), AutoFormatDisable]
		public string ClearButtonText {
			get { return Properties.ClearButtonText; }
			set { Properties.ClearButtonText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarTodayButtonText"),
#endif
		DefaultValue(StringResources.Calendar_Today), Localizable(true), AutoFormatDisable]
		public string TodayButtonText {
			get { return Properties.TodayButtonText; }
			set { Properties.TodayButtonText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarMinDate"),
#endif
		DefaultValue(typeof(DateTime), ""), AutoFormatDisable]
		public DateTime MinDate {
			get { return Properties.MinDate; }
			set { Properties.MinDate = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarMaxDate"),
#endif
		DefaultValue(typeof(DateTime), ""), AutoFormatDisable]
		public DateTime MaxDate {
			get { return Properties.MaxDate; }
			set { Properties.MaxDate = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDisabledDates"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DefaultValue(typeof(DateTimeCollection), ""), Browsable(false), AutoFormatDisable]
		public DateTimeCollection DisabledDates {
			get { return Properties.DisabledDates; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarRenderIFrameForPopupElements"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), AutoFormatEnable]
		public DefaultBoolean RenderIFrameForPopupElements {
			get { return RenderIFrameForPopupElementsInternal; }
			set {
				RenderIFrameForPopupElementsInternal = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDayHeaderStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayHeaderStyle {
			get { return Properties.DayHeaderStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarWeekNumberStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle WeekNumberStyle {
			get { return Properties.WeekNumberStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDayStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayStyle {
			get { return Properties.DayStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDaySelectedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DaySelectedStyle {
			get { return Properties.DaySelectedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDayOtherMonthStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayOtherMonthStyle {
			get { return Properties.DayOtherMonthStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDayWeekendStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayWeekendStyle {
			get { return Properties.DayWeekendStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDayOutOfRangeStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayOutOfRangeStyle {
			get { return Properties.DayOutOfRangeStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDayDisabledStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle DayDisabledStyle {
			get { return Properties.DayDisabledStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarTodayStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle TodayStyle {
			get { return Properties.TodayStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditButtonStyle ButtonStyle {
			get { return Properties.ButtonStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarHeaderStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarHeaderFooterStyle HeaderStyle {
			get { return Properties.HeaderStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFooterStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarHeaderFooterStyle FooterStyle {
			get { return Properties.FooterStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFastNavStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarFastNavStyle FastNavStyle {
			get { return Properties.FastNavStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFastNavMonthAreaStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle FastNavMonthAreaStyle {
			get { return Properties.FastNavMonthAreaStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFastNavYearAreaStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarElementStyle FastNavYearAreaStyle {
			get { return Properties.FastNavYearAreaStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFastNavMonthStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarFastNavItemStyle FastNavMonthStyle {
			get { return Properties.FastNavMonthStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFastNavYearStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarFastNavItemStyle FastNavYearStyle {
			get { return Properties.FastNavYearStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFastNavFooterStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarHeaderFooterStyle FastNavFooterStyle {
			get { return Properties.FastNavFooterStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarLoadingDivStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingDivStyle LoadingDivStyle {
			get { return base.LoadingDivStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public CalendarClientSideEvents ClientSideEvents {
			get { return ClientSideEventsInternal as CalendarClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarFastNavProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable]
		public CalendarFastNavProperties FastNavProperties {
			get { return Properties.FastNavProperties; }
		}
		protected internal DateTime EffectiveVisibleDate {
			get {
				if (VisibleDate == DateTime.MinValue) {
					if(SelectedDate == DateTime.MinValue) {
						DateTime result = DateTime.Now.Date;
						if(MaxDate > DateTime.MinValue && result > MaxDate)
							result = MaxDate.AddMonths(1 - Columns * Rows);
						if(MinDate > DateTime.MinValue && result < MinDate)
							result = MinDate;
						return result.Date;
					}
					return GetCorrectedDate(SelectedDate);
				}
				return GetCorrectedDate(VisibleDate.Date);
			}
		}
		protected internal new CalendarProperties Properties {
			get { return base.Properties as CalendarProperties; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarSelectionChanged"),
#endif
		Category("Action")]
		public event EventHandler SelectionChanged
		{
			add { Events.AddHandler(EventSelectionChanged, value); }
			remove { Events.RemoveHandler(EventSelectionChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarVisibleMonthChanged"),
#endif
		Category("Action")]
		public event EventHandler VisibleMonthChanged
		{
			add { Events.AddHandler(EventVisibleMonthChanged, value); }
			remove { Events.RemoveHandler(EventVisibleMonthChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarCustomDisabledDate"),
#endif
		Category("Rendering")]
		public event EventHandler<CalendarCustomDisabledDateEventArgs> CustomDisabledDate
		{
			add { Events.AddHandler(EventCustomDisabledDate, value); }
			remove { Events.RemoveHandler(EventCustomDisabledDate, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDayCellInitialize"),
#endif
		Category("Rendering")]
		public event EventHandler<CalendarDayCellInitializeEventArgs> DayCellInitialize
		{
			add { Events.AddHandler(EventDayCellInitialize, value); }
			remove { Events.RemoveHandler(EventDayCellInitialize, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDayCellCreated"),
#endif
		Category("Rendering")]
		public event EventHandler<CalendarDayCellCreatedEventArgs> DayCellCreated
		{
			add { Events.AddHandler(EventDayCellCreated, value); }
			remove { Events.RemoveHandler(EventDayCellCreated, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCalendarDayCellPrepared"),
#endif
		Category("Rendering")]
		public event EventHandler<CalendarDayCellPreparedEventArgs> DayCellPrepared
		{
			add { Events.AddHandler(EventDayCellPrepared, value); }
			remove { Events.RemoveHandler(EventDayCellPrepared, value); }
		}
		protected internal bool IsDateEditCalendar {
			get { return isDateEditCalendar; }
			set { isDateEditCalendar = value; }
		}
		protected internal ITimeSectionOwner TimeSectionOwner {
			get { return timeSectionOwner; }
			set { timeSectionOwner = value; }
		}
		protected internal EventHandler<CalendarCustomDisabledDateEventArgs> CustomDisabledDateEventHandler {
			get { return Events[EventCustomDisabledDate] as EventHandler<CalendarCustomDisabledDateEventArgs>; }
		}
		protected internal EventHandler<CalendarDayCellInitializeEventArgs> DayCellInitializeEventHanlder {
			get { return Events[EventDayCellInitialize] as EventHandler<CalendarDayCellInitializeEventArgs>; }
		}
		protected internal EventHandler<CalendarDayCellCreatedEventArgs> DayCellCreatedEventHanlder {
			get { return Events[EventDayCellCreated] as EventHandler<CalendarDayCellCreatedEventArgs>; }
		}
		protected internal EventHandler<CalendarDayCellPreparedEventArgs> DayCellPreparedEventHanlder {
			get { return Events[EventDayCellPrepared] as EventHandler<CalendarDayCellPreparedEventArgs>; }
		}
		internal void RaiseCustomDisabledDate(DayData dayData) {
			if(CustomDisabledDateEventHandler != null)
				CustomDisabledDateEventHandler(this, new CalendarCustomDisabledDateEventArgs(dayData));
		}
		internal void RaiseDayCellInitialize(DayData dayData) {
			if(DayCellInitializeEventHanlder == null)
				return;
			var e = new CalendarDayCellInitializeEventArgs(dayData);
			var displayText = e.DisplayText;
			DayCellInitializeEventHanlder(this, e);
			if(e.EncodeHtml && e.DisplayText != displayText)
				e.DisplayText = System.Web.HttpUtility.HtmlEncode(e.DisplayText);
		}
		internal void RaiseDayCellCreated(DayData dayData, TableCell cell) {
			if(DayCellCreatedEventHanlder != null)
				DayCellCreatedEventHanlder(this, new CalendarDayCellCreatedEventArgs(dayData, cell));
		}
		internal void RaiseDayCellPrepared(DayData dayData, TableCell cell, WebControl textControl) {
			if(DayCellPreparedEventHanlder != null)
				DayCellPreparedEventHanlder(this, new CalendarDayCellPreparedEventArgs(dayData, cell, textControl));
		}
		protected override EditPropertiesBase CreateProperties() {
			return new CalendarProperties(this);
		}
		protected override bool IsDesignTimeDataBindingRequired() {
			return false;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(!DesignMode)
				Controls.Add(new CalendarEtalonCells(this));
			Controls.Add(CreateCalendarControl());
		}
		protected virtual ASPxInternalWebControl CreateCalendarControl() {
			return new CalendarControl(this);
		}
		protected internal string GetMonthGridClassName() {
			string className = "dxMonthGrid";
			if(ShowWeekNumbers) {
				className += "WithWeekNumbers";
				if(IsRightToLeft())
					className += "Rtl";
			}
			return className;
		}
		protected internal DayOfWeek GetFirstDayOfWeek() {
			if (FirstDayOfWeek != FirstDayOfWeek.Default)
				return (DayOfWeek)FirstDayOfWeek;
			return CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
		}
		protected internal int GetISO8601WeekOfYear(DateTime date) {
			int firstDayOfWeek = (int)(new DateTime(date.Year, 1, 1)).DayOfWeek;
			if (firstDayOfWeek == 0)
				firstDayOfWeek = 7;
			int daysInFirstWeek = 8 - firstDayOfWeek;
			int lastDayOfWeek = (int)(new DateTime(date.Year, 12, 31)).DayOfWeek;
			if (lastDayOfWeek == 0)
				lastDayOfWeek = 7;
			int daysInLastWeek = 8 - lastDayOfWeek;
			int fullWeeks = (int)Math.Ceiling((date.DayOfYear - daysInFirstWeek) / 7.0);
			int result = fullWeeks;
			if (daysInFirstWeek > 3)
				result++;
			bool isThursday = firstDayOfWeek == 4 || lastDayOfWeek == 4;
			if (result > 52 && !isThursday)
				result = 1;
			if (result == 0)
				return GetISO8601WeekOfYear(new DateTime(date.Year - 1, 12, 31));
			return result;
		}
		protected internal bool IsDateSelected(DateTime date) {
			foreach (DateTime selectedDate in SelectedDates)
				if (date.Date == selectedDate.Date)
					return true;
			return false;
		}
		protected internal virtual bool IsDateDefaultWeekend(DateTime date) {
			return date.DayOfWeek == DayOfWeek.Saturday
				|| date.DayOfWeek == DayOfWeek.Sunday;
		}
		protected internal bool AreDatesOfSameMonth(DateTime date1, DateTime date2) {
			return date1.Month == date2.Month && date1.Year == date2.Year;
		}
		protected internal bool IsFastNavEnabled() {
			return !DesignMode && IsEnabled() && ShowHeader &&
				FastNavProperties.Enabled && (EnableYearNavigation || EnableMonthNavigation);
		}
		protected internal bool IsMultiView() {
			return Columns > 1 || Rows > 1;
		}
		protected internal bool IsInDisabledDates(DateTime dateTime) {
			foreach(DateTime date in DisabledDates)
				if(dateTime.Date == date.Date)
					return true;
			return false;
		}
		protected internal bool IsDateInRange(DateTime date) {
			return IsDateInRange(date, MinDate, MaxDate);
		}
		internal static bool IsDateInRange(DateTime date, DateTime minDate, DateTime maxDate) {
			return (minDate == DateTime.MinValue || date >= minDate) &&
				(maxDate == DateTime.MinValue || date <= maxDate);
		}
		protected internal DateTime FitDateToRange(DateTime date) {
			if (MinDate > DateTime.MinValue && date < MinDate)
				date = MinDate;
			else if (MaxDate > DateTime.MinValue && date > MaxDate)
				date = MaxDate;
			return date;
		}
		protected internal override void ValidateProperties() {
			if (MinDate > DateTime.MinValue && MaxDate > DateTime.MinValue) {
				if (MaxDate < MinDate)
					throw new ArgumentException(String.Format(StringResources.ASPxEdit_InvalidRange, "MaxDate", "MinDate"));
			}
		}
		protected internal DateTime GetCorrectedDate(DateTime date) {
			if(date.Year < 100) {
				if(date.Year < 28)
					date = date.AddYears(2000);
				else
					date = date.AddYears(1900);
			}
			return date;
		}
		protected internal IEnumerable<DateTime> GetCorrectedSelectedDates() {
			return SelectedDates.Select(d => GetCorrectedDate(d));
		}
		#region Styles
		protected override LoadingPanelStyle GetLoadingPanelStyle() {
			LoadingPanelStyle style = RenderStyles.GetDefaultLoadingPanelWithContentStyle(true);
			style.CopyFrom(RenderStyles.LoadingPanel);
			return style;
		}
		protected override LoadingDivStyle GetLoadingDivStyle() {
			LoadingDivStyle style = RenderStyles.GetDefaultLoadingDivWithContentStyle(true);
			style.CopyFrom(RenderStyles.LoadingDiv);
			return style;
		}
		protected override AppearanceStyle GetDefaultEditStyle() {
			AppearanceStyle defaultStyle = new AppearanceStyle();
			defaultStyle.CopyFrom(RenderStyles.GetDefaultCalendarStyle());
			return defaultStyle;
		}
		protected override AppearanceStyle GetEditStyleFromStylesStorage() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.Calendar);
			return style;
		}
		protected internal CalendarElementStyle GetDayHeaderStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarDayHeaderStyle());
			style.CopyFrom(RenderStyles.CalendarDayHeader);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarElementStyle GetWeekNumberStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarWeekNumberStyle());
			style.CopyFrom(RenderStyles.CalendarWeekNumber);
			MergeDisableStyle(style);
			return style;
		}
		static object dayStyleKey = new object();
		protected internal CalendarElementStyle GetDayStyle() {
			return (CalendarElementStyle)CreateStyle(delegate() {
				CalendarElementStyle style = new CalendarElementStyle();
				style.CopyFrom(RenderStyles.GetDefaultCalendarDayStyle());
				style.CopyFrom(RenderStyles.CalendarDay);
				MergeDisableStyle(style);
				return style;
			}, dayStyleKey);
		}
		protected internal CalendarElementStyle GetDayOtherMonthStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarOtherMonthStyle());
			style.CopyFrom(RenderStyles.CalendarDayOtherMonth);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarElementStyle GetDaySelectedStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarSelectedStyle());
			style.CopyFrom(RenderStyles.CalendarDaySelected);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarElementStyle GetDayWeekendStyle() {
			if (!HighlightWeekends)
				return new CalendarElementStyle();
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarWeekendStyle());
			style.CopyFrom(RenderStyles.CalendarDayWeekEnd);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarElementStyle GetDayOutOfRangeStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarOutOfRangeStyle());
			style.CopyFrom(RenderStyles.CalendarDayOutOfRange);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarElementStyle GetDayDisabledStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarDayDisabledStyle());
			style.CopyFrom(RenderStyles.CalendarDayDisabled);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarElementStyle GetTodayStyle() {
			if (!HighlightToday)
				return new CalendarElementStyle();
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarTodayStyle());
			style.CopyFrom(RenderStyles.CalendarToday);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarHeaderFooterStyle GetHeaderStyle() {
			CalendarHeaderFooterStyle style = new CalendarHeaderFooterStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarHeaderStyle());
			style.CopyFrom(RenderStyles.CalendarHeader);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarHeaderFooterStyle GetFooterStyle() {
			CalendarHeaderFooterStyle style = new CalendarHeaderFooterStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarFooterStyle());
			style.CopyFrom(RenderStyles.CalendarFooter);
			MergeDisableStyle(style);
			if(TimeSectionOwner != null && TimeSectionOwner.ShowTimeSection)
				style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, TimeSectionOwner.TimeSectionFooterClassName);
			return style;
		}
		protected internal CalendarFastNavStyle GetFastNavStyle() {
			CalendarFastNavStyle style = new CalendarFastNavStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarFastNavStyle());
			style.CopyFrom(RenderStyles.CalendarFastNav);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarElementStyle GetFastNavMonthAreaStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarFastNavMonthAreaStyle());
			style.CopyFrom(RenderStyles.CalendarFastNavMonthArea);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarElementStyle GetFastNavYearAreaStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(RenderStyles.GetDefatultCalendarFastNavYearAreaStyle());
			style.CopyFrom(RenderStyles.CalendarFastNavYearArea);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarHeaderFooterStyle GetFastNavFooterStyle() {
			CalendarHeaderFooterStyle style = new CalendarHeaderFooterStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarFastNavFooterStyle());
			style.CopyFrom(RenderStyles.CalendarFastNavFooter);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarFastNavItemStyle GetFastNavMonthStyle() {
			CalendarFastNavItemStyle style = new CalendarFastNavItemStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarFastNavMonthStyle());
			style.CopyFrom(RenderStyles.CalendarFastNavMonth);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarFastNavItemStyle GetFastNavMonthSelectedStyle() {
			CalendarFastNavItemStyle style = new CalendarFastNavItemStyle();
			style.CopyFrom(GetFastNavMonthStyle().SelectedStyle);
			return style;
		}
		protected internal CalendarFastNavItemStyle GetFastNavMonthHoverStyle() {
			CalendarFastNavItemStyle style = GetFastNavMonthStyle();
			style.CopyFrom(style.HoverStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetFastNavMonthHoverCssStyle() {
			AppearanceStyle style = new AppearanceStyle();
			CalendarFastNavItemStyle itemStyle = GetFastNavMonthStyle();
			style.CopyFrom(itemStyle.HoverStyle);
			style.Paddings.CopyFrom(UnitUtils.GetSelectedCssStylePaddings(itemStyle, itemStyle.HoverStyle, itemStyle.Paddings));
			return style;
		}
		protected internal CalendarFastNavItemStyle GetFastNavYearStyle() {
			CalendarFastNavItemStyle style = new CalendarFastNavItemStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarFastNavYearStyle());
			style.CopyFrom(RenderStyles.CalendarFastNavYear);
			MergeDisableStyle(style);
			return style;
		}
		protected internal CalendarFastNavItemStyle GetFastNavYearSelectedStyle() {
			CalendarFastNavItemStyle style = new CalendarFastNavItemStyle();
			style.CopyFrom(GetFastNavYearStyle().SelectedStyle);
			return style;
		}
		protected internal CalendarFastNavItemStyle GetFastNavYearHoverStyle() {
			CalendarFastNavItemStyle style = GetFastNavYearStyle();
			style.CopyFrom(style.HoverStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetFastNavYearHoverCssStyle() {
			AppearanceStyle style = new AppearanceStyle();
			CalendarFastNavItemStyle itemStyle = GetFastNavYearStyle();
			style.CopyFrom(itemStyle.HoverStyle);
			style.Paddings.CopyFrom(UnitUtils.GetSelectedCssStylePaddings(itemStyle, itemStyle.HoverStyle, itemStyle.Paddings));
			return style;
		}
		protected internal EditButtonStyle GetButtonStyle() {
			EditButtonStyle style = GetButtonStyleInternal();
			MergeDisableStyle(style, IsEnabled(), GetButtonDisabledStyle(), false);
			return style;
		}
		protected virtual DisabledStyle GetButtonDisabledStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetDisabledStyle());
			style.CopyFrom(RenderStyles.GetDefaultButtonDisabledStyle());
			style.CopyFrom(RenderStyles.CalendarButton.DisabledStyle);
			return style;
		}
		protected DisabledStyle GetButtonDisabledCssStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetButtonDisabledStyle());
			return style;
		}
		protected internal EditButtonStyle GetFastNavButtonStyle() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarButtonStyle());
			style.CopyFrom(RenderStyles.CalendarButton);
			MergeDisableStyle(style);
			return style;
		}
		protected EditButtonStyle GetButtonStyleInternal() {
			EditButtonStyle style = new EditButtonStyle();
			style.CopyFrom(RenderStyles.GetDefaultCalendarButtonStyle());
			style.CopyFrom(RenderStyles.CalendarButton);
			return style;
		}
		protected AppearanceStyleBase GetButtonHoverCssStyle() {
			AppearanceStyle style = new AppearanceStyle();
			EditButtonStyle buttonStyle = GetButtonStyleInternal();
			style.CopyFrom(buttonStyle.HoverStyle);
			style.Paddings.CopyFrom(UnitUtils.GetSelectedCssStylePaddings(buttonStyle, buttonStyle.HoverStyle, buttonStyle.Paddings));
			return style;
		}
		protected AppearanceStyleBase GetButtonPressedCssStyle() {
			AppearanceStyle style = new AppearanceStyle();
			EditButtonStyle buttonStyle = GetButtonStyleInternal();
			style.CopyFrom(buttonStyle.PressedStyle);
			style.Paddings.CopyFrom(UnitUtils.GetSelectedCssStylePaddings(buttonStyle, buttonStyle.PressedStyle, buttonStyle.Paddings));
			return style;
		}
		#endregion Styles
		protected virtual bool AllowClientSideRendering() {
			return DayCellInitializeEventHanlder == null && DayCellCreatedEventHanlder == null && DayCellPreparedEventHanlder == null && Legacy_DayRenderEventHandler == null && !CustomDisabledDatesViaCallback();
		}
		protected bool CustomDisabledDatesViaCallback() {
			return CustomDisabledDateEventHandler != null && String.IsNullOrEmpty(ClientSideEvents.CustomDisabledDate);
		}
		protected override bool IsServerSideEventsAssigned() {
			return !AllowClientSideRendering();
		}
		protected internal override bool IsCallBacksEnabled() {
			return !AllowClientSideRendering() && !AutoPostBack;
		}
		protected internal override bool IsCallbackAnimationEnabled() {
			return GetActualChangeVisibleDateAnimationType() == AnimationType.Fade;
		}
		protected internal override bool IsSlideCallbackAnimationEnabled() {
			return GetActualChangeVisibleDateAnimationType() == AnimationType.Slide || IsSwipeGesturesEnabled();
		}
		protected internal override bool IsSwipeGesturesEnabled() {
			return EnableChangeVisibleDateGestures == DefaultBoolean.True || (EnableChangeVisibleDateGestures == DefaultBoolean.Default && Browser.Platform.IsTouchUI);
		}
		protected override bool HasLoadingPanel() {
			return IsCallBacksEnabled();
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		protected virtual AnimationType GetActualChangeVisibleDateAnimationType() {
			if(ChangeVisibleDateAnimationType == AnimationType.Auto)
				return AnimationType.None;
			return ChangeVisibleDateAnimationType;
		}
		protected internal string GetTodayButtonOnClick() {
			return string.Format(TodayClickHandlerName, ClientID);
		}
		protected internal string GetClearButtonOnClick() {
			return string.Format(ClearClickHandlerName, ClientID);
		}
		protected internal string GetNavigationOnClick(int monthOffset) {
			return string.Format(NavigationClickHandlerName, ClientID, monthOffset);
		}
		protected internal string GetTitleOnClick(int row, int column) {
			return string.Format(TitleClickHandlerName, ClientID, row, column);
		}
		protected internal string GetFastNavYearShuffleOnClick(int offset) {
			return string.Format(FastNavYearShuffleHandlerName, ClientID, offset);
		}
		protected internal string GetFastNavButtonOnClick(string action) {
			return string.Format(FastNavButtonClickHandlerName, ClientID, action);
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxCalendar), StateControllerScriptResourceName);
			RegisterDateFormatterScript();
			RegisterIncludeScript(typeof(ASPxCalendar), CalendarScriptResourceName);
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			RegisterCultureInfoScript();
			if(IsAccessibilityCompliantRender())
				RegisterAccessibilityDescription();
		}
		protected void RegisterAccessibilityDescription() {
			string script = string.Format("ASPx.AccessibilitySR.CalendarMultiSelectText = {0};", HtmlConvertor.ToScript(AccessibilityUtils.ClientCalendarMultiSelectText));
			RegisterScriptBlock("AccessibilitySR_Calendar", RenderUtils.GetScriptHtml(script));
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendLine(string.Format("{0}.currentDate={1};", localVarName, HtmlConvertor.ToScript(DateTime.Now.Date)));
			stb.AppendLine(localVarName + ".visibleDate = " + HtmlConvertor.ToScript(EffectiveVisibleDate) + ";");
			if(SelectedDates.Count > 0)
				stb.AppendLine(localVarName + ".selection.AddArray(" + HtmlConvertor.ToJSON(GetCorrectedSelectedDates()) + ");");
			int firstDayOfWeek = (int)GetFirstDayOfWeek();
			if (firstDayOfWeek != 0)
				stb.AppendLine(localVarName + ".firstDayOfWeek = " + firstDayOfWeek + ";");
			if (Columns > 1)
				stb.AppendLine(localVarName + ".columns = " + Columns.ToString() + ";");
			if (Rows > 1)
				stb.AppendLine(localVarName + ".rows = " + Rows.ToString() + ";");
			if (IsEnabled()) {
				if (!IsFastNavEnabled())
					stb.AppendLine(localVarName + ".enableFast = false;");
				if (EnableMultiSelect)
					stb.AppendLine(localVarName + ".enableMulti = true;");
			}
			if (MinDate > DateTime.MinValue)
				stb.AppendLine(localVarName + ".minDate = " + HtmlConvertor.ToScript(MinDate) + ";");
			if (MaxDate > DateTime.MinValue)
				stb.AppendLine(localVarName + ".maxDate = " + HtmlConvertor.ToScript(MaxDate) + ";");
			if (!AllowClientSideRendering())
				stb.AppendLine(localVarName + ".customDraw = true;");
			if (!ShowWeekNumbers)
				stb.AppendLine(localVarName + ".showWeekNumbers = false;");
			if (!ShowDayHeaders)
				stb.AppendLine(localVarName + ".showDayHeaders = false;");
			if(IsDateEditCalendar)
				stb.AppendFormat("{0}.isDateEditCalendar = true;\n", localVarName);
			if(DisabledDates.Count > 0 || CustomDisabledDatesViaCallback()) {
				DateTimeCollection disabledDates = new DateTimeCollection();
				disabledDates.AddRange(DisabledDates);
				disabledDates.AddRange(GetCustomDisabledDates());
				stb.AppendFormat("{0}.disabledDates = {1};\n", localVarName, HtmlConvertor.ToJSON(disabledDates));
			}
		}
		protected override string GetClientObjectStateInputID() {
			return UniqueID;
		}
		protected DateTimeCollection GetCustomDisabledDates() {
			DateTimeCollection disabledDates = new DateTimeCollection();
			if(CustomDisabledDatesViaCallback()) {
				for(DateTime date = this.GetFirstVisibleDate().Date; date.Date <= this.GetLastVisibleDate().Date; date = date.AddDays(1)) {
					if(IsCustomDisabledDate(date))
						disabledDates.Add(date);
				}
			}
			return disabledDates;
		}
		protected bool IsCustomDisabledDate(DateTime date) {
			DayData dayData = new DayData(new CalendarViewInfo(this, VisibleDate), date);
			RaiseCustomDisabledDate(dayData);
			return dayData.IsDisabled;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientCalendar";
		}
		protected override bool HasHoverScripts() {
			return IsStateScriptEnabled() && (Properties.HasVisibleButtons() || IsFastNavEnabled());
		}
		protected override bool HasPressedScripts() {
			return IsStateScriptEnabled() && Properties.HasVisibleButtons();
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			AddStateItems(helper, GetButtonHoverCssStyle());
			if (IsFastNavEnabled()) {
				AddHoverFastNavMonthItems(helper, GetFastNavMonthHoverCssStyle());
				AddHoverFastNavYearItems(helper, GetFastNavYearHoverCssStyle());
			}
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			AddStateItems(helper, GetButtonPressedCssStyle());
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			base.AddDisabledItems(helper);
			AddStateItems(helper, GetButtonDisabledCssStyle());
			AppearanceStyleBase style = new AppearanceStyleBase();
			helper.AddStyle(style, PrevYearCellIdSufix, GetPrevNextImagesPostfixes(),
				GetPrevYearImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
			helper.AddStyle(style, PrevMonthCellIdSufix, GetPrevNextImagesPostfixes(),
				GetPrevMonthImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
			helper.AddStyle(style, NextMonthCellIdSufix, GetPrevNextImagesPostfixes(),
				GetNextMonthImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
			helper.AddStyle(style, NextYearCellIdSufix, GetPrevNextImagesPostfixes(),
				GetNextYearImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
		}
		protected void AddStateItems(StateScriptRenderHelper helper, AppearanceStyleBase style) {
			if (ShowTodayButton)
				helper.AddStyle(style, CalendarFooter.TodayButtonSuffix, IsEnabled());
			if (ShowClearButton)
				helper.AddStyle(style, CalendarFooter.ClearButtonSuffix, IsEnabled());
			if(TimeSectionOwner != null) {
				if(TimeSectionOwner.ShowOkButton)
					helper.AddStyle(style, TimeSectionCalendarFooter.OkButtonSuffix, IsEnabled());
				if(TimeSectionOwner.ShowCancelButton)
					helper.AddStyle(style, TimeSectionCalendarFooter.CancelButtonSuffix, IsEnabled());
			}
			if (IsFastNavEnabled()) {
				helper.AddStyle(style, CalendarFastNav.PopupSuffix + "_" + CalendarFastNav.OkButtonSuffix, IsEnabled());
				helper.AddStyle(style, CalendarFastNav.PopupSuffix + "_" + CalendarFastNav.CancelButtonSuffix, IsEnabled());
			}
		}
		protected string[] GetPrevNextImagesPostfixes() {
			return new string[0];
		}
		protected void AddHoverFastNavMonthItems(StateScriptRenderHelper helper, AppearanceStyleBase style) {
			for (int i = 0; i < 12; i++)
				helper.AddStyle(style, GetFastMonthNavItemID(i), IsEnabled());
		}
		protected void AddHoverFastNavYearItems(StateScriptRenderHelper helper, AppearanceStyleBase style) {
			for (int i = 0; i < 10; i++)
				helper.AddStyle(style, GetFastYearNavItemID(i), IsEnabled());
		}
		protected string GetFastMonthNavItemID(int month) {
			return CalendarFastNav.PopupSuffix + "_M" + month.ToString();
		}
		protected string GetFastYearNavItemID(int index) {
			return CalendarFastNav.PopupSuffix + "_Y" + index.ToString();
		}
		protected internal override string GetAssociatedControlID() {
			return ClientID + "_" + (IsAccessibilityCompliantRender() ? AccessibilityUtils.AssistantID : GetKBSupportInputId());
		}
		protected internal override bool IsAccessibilityAssociatingSupported() {
			return RenderUtils.IsHtml5Mode(this) && IsAccessibilityCompliantRender() && !IsNativeRender();
		}
		protected override bool IsAccessibilityCompliant() {
			return IsAriaSupported() && base.IsAccessibilityCompliant();
		}
		protected internal ImageProperties GetPrevYearImage() {
			return RenderImages.GetImageProperties(Page, IsRightToLeft() ? EditorImages.CalendarNextYearImageName : EditorImages.CalendarPrevYearImageName);
		}
		protected internal ImageProperties GetPrevMonthImage() {
			return RenderImages.GetImageProperties(Page, IsRightToLeft() ? EditorImages.CalendarNextMonthImageName : EditorImages.CalendarPrevMonthImageName);
		}
		protected internal ImageProperties GetNextMonthImage() {
			return RenderImages.GetImageProperties(Page, IsRightToLeft() ? EditorImages.CalendarPrevMonthImageName : EditorImages.CalendarNextMonthImageName);
		}
		protected internal ImageProperties GetNextYearImage() {
			return RenderImages.GetImageProperties(Page, IsRightToLeft() ? EditorImages.CalendarPrevYearImageName : EditorImages.CalendarNextYearImageName);
		}
		protected internal ImageProperties GetFastNavPrevYearImage() {
			return RenderImages.GetImageProperties(Page, IsRightToLeft() ? EditorImages.CalendarFastNavNextYearImageName : EditorImages.CalendarFastNavPrevYearImageName);
		}
		protected internal ImageProperties GetFastNavNextYearImage() {
			return RenderImages.GetImageProperties(Page, IsRightToLeft() ? EditorImages.CalendarFastNavPrevYearImageName : EditorImages.CalendarFastNavNextYearImageName);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { SelectedDates });
		}
		protected override object GetCallbackResult() {
			BeginRendering();
			try {
				return GetCallbackRenderResult();
			}
			finally {
				EndRendering();
			}
		}
		protected object GetCallbackRenderResult() {
			List<string> result = new List<string>();
			CalendarControl control = GetCallbackResultControl();
			foreach (TableCell cell in control.ViewCells) {
				StringBuilder builder = new StringBuilder();
				foreach (Control child in cell.Controls)
					builder.Append(RenderUtils.GetRenderResult(child));
				result.Add(builder.ToString());
			}
			if(result.Count > 0 && CustomDisabledDatesViaCallback())
				result.Add(HtmlConvertor.ToJSON(GetCustomDisabledDates()));
			return result;
		}
		protected CalendarControl GetCallbackResultControl() {
			ApplyDateRangeCalendarColumnCount();
			EnsureChildControls();
			foreach(Control control in Controls) {
				if(control is CalendarControl)
					return (CalendarControl)control;
			}
			return null;
		}
		protected void ApplyDateRangeCalendarColumnCount() {
			ASPxDateEdit dateEdit = OwnerControl as ASPxDateEdit;
			if(dateEdit != null)
				dateEdit.ApplyDateRangeCalendarColumnCount(Properties);
		}
		protected override void RaisePostDataChangedEvent() {
			if (this.visibleMonthChanged) {
				OnDateEvent(EventArgs.Empty, EventVisibleMonthChanged);
				this.visibleMonthChanged = false;
			}
			if (this.selectionChanged) {
				base.RaisePostDataChangedEvent();
				this.selectionChanged = false;
			}
		}
		protected override void RaiseValueChanged() {
			base.RaiseValueChanged();
			OnDateEvent(EventArgs.Empty, EventSelectionChanged);
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			string visibleDateStr = GetClientObjectStateValueString("visibleDate");
			ArrayList selectedDates = GetClientObjectStateValue<ArrayList>("selectedDates");
			ParseClientStateString(visibleDateStr, selectedDates);
			return this.visibleMonthChanged || this.selectionChanged;
		}
		protected internal void ParseClientStateString(string visibleDateStr, ArrayList selectedDates) {
			if(!string.IsNullOrEmpty(visibleDateStr)) {
				DateTime newVisibleDate = DateTime.ParseExact(visibleDateStr, "d", CultureInfo.InvariantCulture);
				this.visibleMonthChanged = !AreDatesOfSameMonth(newVisibleDate, EffectiveVisibleDate);
				VisibleDate = newVisibleDate;
			}
			if(selectedDates != null) {
				CalendarSelection newSelectedDates = new CalendarSelection(this, selectedDates.Count);
				bool newSelectedDatesAreValid = true;
				for(int i = 0; i < selectedDates.Count; i++) {
					DateTime newDate = DateTime.ParseExact((string)selectedDates[i], "d", CultureInfo.InvariantCulture);
					if(!IsDateInRange(newDate)) {
						newSelectedDatesAreValid = false;
						break;
					}
					if(!IsCustomDisabledDate(newDate) && !IsInDisabledDates(newDate))
						newSelectedDates.Add(DateTime.ParseExact((string)selectedDates[i], "d", CultureInfo.InvariantCulture));
				}
				this.selectionChanged = newSelectedDatesAreValid && !newSelectedDates.Equals(SelectedDates);
				if(this.selectionChanged)
					SelectedDates.Assign(newSelectedDates);
			} else if(SelectedDates.Count > 0) {
				SelectedDates.Clear();
			}
		}
		protected void OnDateEvent(EventArgs e, object eventKey) {
			EventHandler handler = Events[eventKey] as EventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			Legacy_UseDayRenderOnPreRender();
		}
		#region For ASPxScheduler
		public DateTime GetFirstVisibleDate() {
			DateTime date = EffectiveVisibleDate;
			return CommonUtils.GetFirstDateOfMonthView(date.Year, date.Month, GetFirstDayOfWeek());
		}
		public DateTime GetLastVisibleDate() {
			DateTime date = EffectiveVisibleDate;
			date = date.AddMonths(Columns * Rows - 1);			
			return CommonUtils.GetFirstDateOfMonthView(date.Year, date.Month, GetFirstDayOfWeek()).AddDays(7 * CalendarView.WeekCount - 1);
		}
		#endregion
		#region Legacy
		static readonly object Legacy_EventDayRender = new object();   
		protected internal DayRenderEventHandler Legacy_DayRenderEventHandler {
			get { return Events[Legacy_EventDayRender] as DayRenderEventHandler; }
		}
		void Legacy_UseDayRenderOnPreRender() {
			if(Legacy_DayRenderEventHandler != null)
				RecreateControlHierarchy();
		}
		#endregion
		protected internal virtual DateTime GetActualTodayDate() {
			return DateTime.Today.Date;
		}
	}
	public class CalendarSelection : ICollection<DateTime>, IStateManager {
		private Dictionary<long, DateTime> dates;
		private IWebControlObject owner;
		public CalendarSelection(IWebControlObject owner, int capacity) {
			this.owner = owner;
			this.dates = new Dictionary<long, DateTime>(capacity);
		}
		public CalendarSelection(IWebControlObject owner)
			: this(owner, 5) {
		}
		public CalendarSelection(IWebControlObject owner, DateTime[] dates)
			: this(owner, dates.Length) {
			Add(dates);
		}
		protected Dictionary<long, DateTime> Dates {
			get { return dates; }
		}
		protected IWebControlObject Owner {
			get { return owner; }
		}
		public bool Equals(CalendarSelection selection) {
			if (object.ReferenceEquals(this, selection))
				return true;
			if (selection.Count != Count)
				return false;
			foreach (DateTime date in selection)
				if (!Contains(date))
					return false;
			return true;
		}
		public void Assign(CalendarSelection source) {
			Clear();
			foreach (DateTime date in source)
				Add(date);
		}
		public void Add(DateTime[] dates) {
			foreach (DateTime date in dates)
				Add(date);
		}
		public void Remove(DateTime[] dates) {
			foreach (DateTime date in dates)
				Remove(date);
		}
		public void AddRange(DateTime start, DateTime end) {
			if (end < start) {
				AddRange(end, start);
				return;
			}
			start = start.Date;
			end = end.Date;
			do {
				Add(start);
				start = start.AddDays(1);
			} while (start <= end);
		}
		public void RemoveRange(DateTime start, DateTime end) {
			if (end < start) {
				RemoveRange(end, start);
				return;
			}
			start = start.Date;
			end = end.Date;
			do {
				Remove(start);
				start = start.AddDays(1);
			} while (start <= end);
		}
		protected internal DateTime GetFirstDate() {
			if (Count > 0) {
				IEnumerator<DateTime> enumerator = GetEnumerator();
				enumerator.MoveNext();
				return enumerator.Current;
			}
			return DateTime.MinValue;
		}
		private long GetKey(DateTime date) {
			return date.Date.Ticks / TimeSpan.TicksPerDay;
		}
		protected void Changed() {
			if (Owner != null)
				Owner.LayoutChanged();
		}
#if !SL
	[DevExpressWebLocalizedDescription("CalendarSelectionCount")]
#endif
		public int Count {
			get { return Dates.Count; }
		}
		bool ICollection<DateTime>.IsReadOnly {
			get { return false; }
		}
		public void Add(DateTime date) {
			Dates[GetKey(date)] = date.Date;
			Changed();
		}
		public void Clear() {
			Dates.Clear();
			Changed();
		}
		public bool Contains(DateTime date) {
			return Dates.ContainsKey(GetKey(date));
		}
		public void CopyTo(DateTime[] array, int index) {
			Dates.Values.CopyTo(array, index);
		}
		public bool Remove(DateTime date) {
			if (!Contains(date))
				return false;
			Dates.Remove(GetKey(date));
			Changed();
			return true;
		}
		public IEnumerator<DateTime> GetEnumerator() {
			return Dates.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		bool IStateManager.IsTrackingViewState {
			get { return false; }
		}
		void IStateManager.LoadViewState(object state) {
			Dates.Clear();
			Int32[] data = (Int32[])state;
			DateTime date;
			for (int i = 0; i < data.Length; i++) {
				date = new DateTime(TimeSpan.TicksPerDay * data[i]);
				Add(date);
			}
		}
		object IStateManager.SaveViewState() {
			Int32[] result = new Int32[Count];
			int index = 0;
			foreach (DateTime date in this)
				result[index++] = (Int32)(date.Ticks / TimeSpan.TicksPerDay);
			return result;
		}
		void IStateManager.TrackViewState() { }
	}
	public class CalendarFastNavProperties : PropertiesBase {
		public CalendarFastNavProperties()
			: base() {
		}
		public CalendarFastNavProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarFastNavPropertiesEnabled"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set {
				SetBoolProperty("Enabled", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarFastNavPropertiesOkButtonText"),
#endif
		NotifyParentProperty(true), Localizable(true), DefaultValue(StringResources.Calendar_Ok)]
		public string OkButtonText {
			get { return GetStringProperty("OkButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_OK)); }
			set { SetStringProperty("OkButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_OK), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarFastNavPropertiesCancelButtonText"),
#endif
		NotifyParentProperty(true), Localizable(true), DefaultValue(StringResources.Calendar_Cancel)]
		public string CancelButtonText {
			get { return GetStringProperty("CancelButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_Cancel)); }
			set { SetStringProperty("CancelButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.Calendar_Cancel), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarFastNavPropertiesEnablePopupAnimation"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool EnablePopupAnimation {
			get { return GetBoolProperty("EnablePopupAnimation", true); }
			set { SetBoolProperty("EnablePopupAnimation", true, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				CalendarFastNavProperties src = source as CalendarFastNavProperties;
				if (src != null) {
					Enabled = src.Enabled;
					OkButtonText = src.OkButtonText;
					CancelButtonText = src.CancelButtonText;
					EnablePopupAnimation = src.EnablePopupAnimation;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class DateTimeCollection : CustomCollection<DateTime> {
		public DateTimeCollection() : base() { }
		protected override bool IsNullOrEmpty(DateTime value) {
			return value == null;
		}
		protected override void Changed() { }
	}
}
namespace DevExpress.Web.Internal {
	class DayData {
		CalendarViewInfo viewInfo;
		DateTime date;
		bool hiddenDay;
		bool disabledDay;
		public string DisplayText;
		public string NavigateUrl;
		public string NavigateUrlTarget;
		public bool Weekend;
		public DayData(CalendarViewInfo viewInfo, DateTime date)
			: this(viewInfo, date, false) {
		}
		public DayData(CalendarViewInfo viewInfo, DateTime date, bool hiddenDay) {
			this.viewInfo = viewInfo;
			this.date = date;
			NavigateUrl = NavigateUrlTarget = null;
			DisplayText = date.Day.ToString();
			Weekend = viewInfo.Calendar.IsDateDefaultWeekend(date);
			this.hiddenDay = hiddenDay;
		}
		public CalendarViewInfo ViewInfo {
			get { return viewInfo; }
		}
		public ASPxCalendar Calendar {
			get { return ViewInfo.Calendar; }
		}
		public DateTime Date {
			get { return date; }
		}
		public bool IsDisabled {
			get { return disabledDay; }
			set { disabledDay = value; }
		}
		public bool IsSelected {
			get { return Calendar.IsDateSelected(Date); }
		}
		public bool IsOtherMonthDay {
			get { return !Calendar.AreDatesOfSameMonth(ViewInfo.VisibleDate, Date); }
		}
		public bool IsDisplayed {
			get {
				if(this.hiddenDay)
					return false;
				return !Calendar.IsMultiView() || !IsOtherMonthDay ||
					ViewInfo.IsLowBoundary && Date <= ViewInfo.VisibleDate ||
					ViewInfo.IsHighBoundary && Date >= ViewInfo.VisibleDate;
			}
		}
	}
}

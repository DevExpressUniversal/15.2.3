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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Calendar;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.Accessibility;
using System.Drawing.Imaging;
using DevExpress.Skins;
using System.Globalization;
using DevExpress.Data.Mask;
using System.Collections.Generic;
using DevExpress.XtraEditors.Popups;
namespace DevExpress.XtraEditors.Mask {
	public class DateEditMaskProperties : DateTimeMaskProperties {
		public DateEditMaskProperties() : base() {
			this.fEditMask = "d";
		}
		[CategoryAttribute("Mask"),
		DefaultValue("d"),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public override string EditMask {
			get { return base.EditMask; }
			set { base.EditMask = value; }
		}
	}
}
namespace DevExpress.XtraEditors.Repository {
	public enum CalendarView { Default, Vista, Classic, TouchUI }
	public enum CalendarSelectionMode { Single, Multiple }
	public interface ICalendarAppearancesOwner {
		void OnAppearanceChanged();
		bool IsLoading { get; }
	}
	public class CalendarAppearances : BaseAppearanceCollection {
		public CalendarAppearances(ICalendarAppearancesOwner owner) {
			Owner = owner;
			this.calendarHeader = CreateAppearance("CalendarHeader");
			this.button = CreateAppearance("Button");
			this.buttonHighlighted = CreateAppearance("ButtonHighlight");
			this.buttonPressed = CreateAppearance("ButtonPressed");
			this.header = CreateAppearance("Header");
			this.highlightHeader = CreateAppearance("HeaderHighlighted");
			this.pressedHeader = CreateAppearance("HeaderPressed");
			this.weekNumber = CreateAppearance("WeekNumber");
			this.dayCell = CreateAppearance("DayCell");
			this.highlightDayCell = CreateAppearance("DayCellHighlighted");
			this.pressedDayCell = CreateAppearance("DayCellPressed");
			this.selectedDayCell = CreateAppearance("DayCellSelected");
			this.inactiveDayCell = CreateAppearance("DayCellInactive");
			this.disabledDayCell = CreateAppearance("DayCellDisabled");
			this.specialDayCell = CreateAppearance("DayCellSpecial");
			this.highlightSpecialDayCell = CreateAppearance("DayCellSpecialHighlighted");
			this.pressedSpecialDayCell = CreateAppearance("DayCellSpecialPressed");
			this.specialDayCellSelected = CreateAppearance("DayCellSpecialSelected");
			this.inactiveSpecialDayCell = CreateAppearance("DayCellSpecialInactive");
			this.disabledSpecialDayCell = CreateAppearance("DayCellSpecialDisabled");
			this.todayDayCell = CreateAppearance("DayCellToday");
			this.holidayDayCell = CreateAppearance("DayCellHoliday");
			this.weekDay = CreateAppearance("WeekDay");
		}
		public override bool IsLoading {
			get { return Owner.IsLoading; }
		}
		protected override AppearanceObject CreateNullAppearance() {
			return null;
		}
		protected override void OnChanged() {
			base.OnChanged();
			Owner.OnAppearanceChanged();
		}
		AppearanceObject 
			calendarHeader,
			button,
			buttonHighlighted,
			buttonPressed,
			header,
			highlightHeader,
			pressedHeader,
			weekNumber,
			dayCell,
			disabledDayCell,
			specialDayCell,
			specialDayCellSelected,
			highlightSpecialDayCell,
			pressedSpecialDayCell,
			disabledSpecialDayCell,
			inactiveSpecialDayCell,
			inactiveDayCell,
			selectedDayCell,
			todayDayCell,
			highlightDayCell,
			pressedDayCell,
			holidayDayCell,
			weekDay;
		void ResetCalendarHeader() { CalendarHeader.Reset(); }
		bool ShouldSerializeCalendarHeader() { return CalendarHeader.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject CalendarHeader {
			get { return calendarHeader; }
		}
		void ResetHeader() { Header.Reset(); }
		bool ShouldSerializeHeader() { return Header.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Header {
			get { return header; }
		}
		void ResetHeaderHighlighted() { HeaderHighlighted.Reset(); }
		bool ShouldSerializeHeaderHighlighted() { return HeaderHighlighted.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderHighlighted {
			get { return highlightHeader; }
		}
		void ResetHeaderPressed() { HeaderPressed.Reset(); }
		bool ShouldSerializeHeaderPressed() { return HeaderPressed.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderPressed {
			get { return pressedHeader; }
		}
		void ResetButton() { Button.Reset(); }
		bool ShouldSerializeButton() { return Button.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Button {
			get { return button; }
		}
		void ResetButtonHighlighted() { ButtonHighlighted.Reset(); }
		bool ShouldSerializeButtonHighlighted() { return ButtonHighlighted.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ButtonHighlighted {
			get { return buttonHighlighted; }
		}
		void ResetButtonPressed() { ButtonPressed.Reset(); }
		bool ShouldSerializeButtonPressed() { return ButtonPressed.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ButtonPressed {
			get { return buttonPressed; }
		}
		void ResetWeekNumber() { WeekNumber.Reset(); }
		bool ShouldSerializeWeekNumber() { return WeekNumber.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject WeekNumber {
			get { return weekNumber; }
		}
		void ResetDayCell() { DayCell.Reset(); }
		bool ShouldSerializeDayCell() { return DayCell.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCell {
			get { return dayCell; }
		}
		void ResetDayCellSpecial() { DayCellSpecial.Reset(); }
		bool ShouldSerializeDayCellSpecial() { return DayCellSpecial.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellSpecial {
			get { return specialDayCell; }
		}
		void ResetDayCellSpecialDisabled() { DayCellSpecialDisabled.Reset(); }
		bool ShouldSerializeDayCellSpecialDisabled() { return DayCellSpecialDisabled.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellSpecialDisabled {
			get { return specialDayCell; }
		}
		void ResetDayCellSpecialInactive() { DayCellSpecialInactive.Reset(); }
		bool ShouldSerializeDayCellSpecialInactive() { return DayCellSpecialInactive.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellSpecialInactive {
			get { return specialDayCell; }
		}
		void ResetDayCellSpecialSelected() { DayCellSpecialSelected.Reset(); }
		bool ShouldSerializeDayCellSpecialSelected() { return DayCellSpecialSelected.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellSpecialSelected {
			get { return specialDayCellSelected; }
		}
		void ResetDayCellSpecialHighlighted() { DayCellSpecialHighlighted.Reset(); }
		bool ShouldSerializeDayCellSpecialHighlighted() { return DayCellSpecialHighlighted.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellSpecialHighlighted {
			get { return highlightSpecialDayCell; }
		}
		void ResetDayCellSpecialPressed() { DayCellSpecialPressed.Reset(); }
		bool ShouldSerializeDayCellSpecialPressed() { return DayCellSpecialPressed.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellSpecialPressed {
			get { return pressedSpecialDayCell; }
		}
		void ResetDayCellDisabled() { DayCellDisabled.Reset(); }
		bool ShouldSerializeDayCellDisabled() { return DayCellDisabled.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellDisabled {
			get { return disabledDayCell; }
		}
		void ResetDayCellInactive() { DayCellInactive.Reset(); }
		bool ShouldSerializeDayCellInactive() { return DayCellInactive.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellInactive {
			get { return inactiveDayCell; }
		}
		void ResetDayCellSelected() { DayCellSelected.Reset(); }
		bool ShouldSerializeDayCellSelected() { return DayCellSelected.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellSelected {
			get { return selectedDayCell; }
		}
		void ResetDayCellToday() { DayCellToday.Reset(); }
		bool ShouldSerializeDayCellToday() { return DayCellToday.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellToday {
			get { return todayDayCell; }
		}
		void ResetDayCellHighlighted() { DayCellHighlighted.Reset(); }
		bool ShouldSerializeDayCellHighlighted() { return DayCellHighlighted.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellHighlighted {
			get { return highlightDayCell; }
		}
		void ResetDayCellPressed() { DayCellPressed.Reset(); }
		bool ShouldSerializeDayCellPressed() { return DayCellPressed.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellPressed {
			get { return pressedDayCell; }
		}
		void ResetDayCellHoliday() { DayCellHoliday.Reset(); }
		bool ShouldSerializeDayCellHoliday() { return DayCellHoliday.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DayCellHoliday {
			get { return holidayDayCell; }
		}
		void ResetWeekDay() { WeekDay.Reset(); }
		bool ShouldSerializeWeekDay() { return WeekDay.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject WeekDay {
			get { return weekDay; }
		}
		public ICalendarAppearancesOwner Owner { get; private set; }
	}
	public class RepositoryItemDateEdit : RepositoryItemPopupBase, IDateTimeOwner, ICalendarAppearancesOwner {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemDateEdit Properties { get { return this; } }
		private static object customDrawDayNumberCell = new object();
		private static object disabledCalendarDateCell = new object();
		private static object specialCalendarDateCell = new object();
		private static object dateTimeChanged = new object();
		private static object customWeekNumber = new object();
		bool _showToday, fShowWeekNumbers, fHighlightHolidays;
		object _nullDate;
		DateTime _minValue, _maxValue, calendarNullDateValue;
		AppearanceObject appearanceDropDownHeader;
		AppearanceObject appearanceDropDownHeaderHighlight, appearanceDropDownHighlight;
		AppearanceObject appearanceDropDownDisabledCalendarDate;
		AppearanceObject appearanceWeekNumber;
		WeekNumberRule weekNumberRule;
		DefaultBoolean vistaDisplayMode, vistaEditTime, highlightTodayCell;
		RepositoryItemTimeEdit timeEditProperties;
		int touchUISecondIncrement, touchUIMinuteIncrement;
		VistaCalendarViewStyle vistaCalendarViewStyle = VistaCalendarViewStyle.Default;
		VistaCalendarInitialViewStyle vistaCalendarInitialViewStyle = VistaCalendarInitialViewStyle.MonthView;
		public RepositoryItemDateEdit() {
			this._showToday = true;
			this.fShowWeekNumbers = false;
			this.fHighlightHolidays = true;
			this._nullDate = null;
			this.appearanceDropDownHeader = CreateAppearance("DropDownHeader");
			this.appearanceDropDownHeaderHighlight = CreateAppearance("DropDownHeaderHighlight");
			this.appearanceWeekNumber = CreateAppearance("WeekNumber");
			this.appearanceDropDownHighlight = CreateAppearance("DropDownHighlight");
			this.appearanceDropDownDisabledCalendarDate = CreateAppearance("DropDownDisabledCalendarDate");
			this.weekNumberRule = WeekNumberRule.Default;
			this._minValue = new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Local);
			this._maxValue = new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Local);
			this.calendarNullDateValue = new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Local);
			this.vistaDisplayMode = DefaultBoolean.Default;
			this.vistaEditTime = DefaultBoolean.Default;
			this.timeEditProperties = CreateVistaTimeProperties();
			this.timeEditProperties.SetOwnerItem(this);
			this.calendarView = Repository.CalendarView.Default;
			this.highlightTodayCell = DefaultBoolean.Default;
			this.touchUISecondIncrement = this.touchUIMinuteIncrement = 1;
			this.firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
			AllowAnimatedContentChange = true;
		}
		private bool calendarDateEditing = true;
		[DefaultValue(true)]
		public bool CalendarDateEditing {
			get { return calendarDateEditing; }
			set {
				if(CalendarDateEditing == value)
					return;
				calendarDateEditing = value;
				OnCalendarDateEditingChanged();
			}
		}
		protected virtual void OnCalendarDateEditingChanged() {
			OnPropertiesChanged();
		}
		private bool allowClickInactiveDays = true;
		[DefaultValue(true)]
		public bool AllowClickInactiveDays {
			get { return allowClickInactiveDays; }
			set {
				if(AllowClickInactiveDays == value)
					return;
				allowClickInactiveDays = value;
				OnAllowClickInactiveDaysChanged();
			}
		}
		protected virtual void OnAllowClickInactiveDaysChanged() { }
		private DefaultBoolean showMonthNavigationButtons = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowMonthNavigationButtons {
			get { return showMonthNavigationButtons; }
			set {
				if(ShowMonthNavigationButtons == value)
					return;
				showMonthNavigationButtons = value;
				OnShowMonthNavigationButtonsChanged();
			}
		}
		protected virtual void OnShowMonthNavigationButtonsChanged() {
			OnPropertiesChanged();
		}
		private DefaultBoolean showYearNavigationButtons = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowYearNavigationButtons {
			get { return showYearNavigationButtons; }
			set {
				if(ShowYearNavigationButtons == value)
					return;
				showYearNavigationButtons = value;
				OnShowYearNavigationButtonsChanged();
			}
		}
		protected virtual void OnShowYearNavigationButtonsChanged() {
			OnPropertiesChanged();
		}
		private DateTime todayDate = DateTime.MinValue;
		void ResetTodayDate() { TodayDate = DateTime.MinValue; }
		bool ShouldSerializeTodayDate() { return TodayDate != DateTime.MinValue; }
		public DateTime TodayDate {
			get { return todayDate; }
			set {
				if(TodayDate == value)
					return;
				todayDate = value;
				OnTodayDateChanged();
			}
		}
		protected virtual void OnTodayDateChanged() { }
		private int weekDayAbbreviationLength = 0;
		[DefaultValue(0)]
		public int WeekDayAbbreviationLength {
			get { return weekDayAbbreviationLength; }
			set {
				if(WeekDayAbbreviationLength == value)
					return;
				weekDayAbbreviationLength = value;
				OnPropertiesChanged();
			}
		}
		private bool drawCellLines;
		[DefaultValue(false)]
		public bool DrawCellLines {
			get { return drawCellLines; }
			set {
				if(DrawCellLines == value)
					return;
				drawCellLines = value;
				OnPropertiesChanged();
			}
		}
		private Size cellSize = Size.Empty;
		bool ShouldSerializeCellSize() { return !CellSize.IsEmpty; }
		void ResetCellSize() { CellSize = Size.Empty; }
		public Size CellSize {
			get { return cellSize; }
			set {
				if(CellSize == value)
					return;
				cellSize = value;
				OnPropertiesChanged();
			}
		}
		private ICalendarCellStyleProvider cellAppearanceProvider;
		[DefaultValue(null)]
		public ICalendarCellStyleProvider CellStyleProvider {
			get { return cellAppearanceProvider; }
			set {
				if(CellStyleProvider == value)
					return;
				cellAppearanceProvider = value;
				OnPropertiesChanged();
			}
		}
		private ICalendarDisabledDateProvider disabledDateProvider;
		[DefaultValue(null)]
		public ICalendarDisabledDateProvider DisabledDateProvider {
			get { return disabledDateProvider; }
			set {
				if(DisabledDateProvider == value)
					return;
				disabledDateProvider = value;
				OnPropertiesChanged();
			}
		}
		private ICalendarSpecialDateProvider specialDateProvider;
		[DefaultValue(null)]
		public ICalendarSpecialDateProvider SpecialDateProvider {
			get { return specialDateProvider; }
			set {
				if(SpecialDateProvider == value)
					return;
				specialDateProvider = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowAnimatedContentChange { get; set; }
		private CalendarAppearances appearanceCalendar;
		void ResetAppearanceCalendar() { AppearanceCalendar.Reset(); }
		bool ShouldSerializeAppearanceCalendar() { return AppearanceCalendar.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public CalendarAppearances AppearanceCalendar {
			get {
				if(appearanceCalendar == null)
					appearanceCalendar = CreateCalendarAppearance();
				return appearanceCalendar; 
			}
		}
		protected virtual CalendarAppearances CreateCalendarAppearance() {
			return new CalendarAppearances(this);
		}
		protected override FormatInfo CreateDisplayFormat() {
			return new DateEditFormatInfo();
		}
		protected override FormatInfo CreateEditFormat() {
			return new DateEditFormatInfo();
		}
		bool ShouldSerializeNullDateCalendarValue() { return NullDateCalendarValue != DateTime.MinValue; }
		[ DXCategory(CategoryName.Behavior), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public DateTime NullDateCalendarValue {
			get { return calendarNullDateValue; }
			set {
				if(NullDateCalendarValue == value) return;
				calendarNullDateValue = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(VistaCalendarInitialViewStyle.MonthView)]
		public VistaCalendarInitialViewStyle VistaCalendarInitialViewStyle {
			get { return vistaCalendarInitialViewStyle; }
			set {
				if(VistaCalendarInitialViewStyle == value)
					return;
				vistaCalendarInitialViewStyle = value;
				OnPropertiesChanged();
			}
		}
		protected override PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection collection) {
			VistaTimeProperties.SetRequireFakeDesigner(RequireFakeDesigner || DesignMode);
			return base.FilterProperties(collection);
		}
		protected virtual RepositoryItemTimeEdit CreateVistaTimeProperties() {
			return new RepositoryItemTimeEdit();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				timeEditProperties.Dispose();
			}
			base.Dispose(disposing);
		}
		protected new DateEdit OwnerEdit { get { return base.OwnerEdit as DateEdit; } }
		protected override MaskProperties CreateMaskProperties() {
			return new DateEditMaskProperties();
		}
		protected override void DestroyAppearances() {
			base.DestroyAppearances();
			AppearanceCalendar.Changed -= OnAppearanceChanged;
			AppearanceCalendar.PaintChanged -= OnAppearancePaintChanged;
			AppearanceCalendar.SizeChanged -= OnAppearanceSizeChanged;
			AppearanceCalendar.Dispose();
		}
		CalendarSelectionMode selectionMode = CalendarSelectionMode.Single;
		[DefaultValue(CalendarSelectionMode.Single)]
		public CalendarSelectionMode SelectionMode {
			get { return selectionMode; }
			set { selectionMode = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size PopupFormMinSize {
			get { return Size.Empty; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size PopupFormSize {
			get { return Size.Empty; }
			set { }
		}
		[DefaultValue(VistaCalendarViewStyle.Default),
		System.ComponentModel.Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public VistaCalendarViewStyle VistaCalendarViewStyle {
			get { return vistaCalendarViewStyle; }
			set { vistaCalendarViewStyle = value; }
		}
		protected internal VistaCalendarViewStyle GetVistaCalendarViewStyle() {
			if(VistaCalendarViewStyle == VistaCalendarViewStyle.Default)
				return VistaCalendarViewStyle.MonthView | VistaCalendarViewStyle.YearView | VistaCalendarViewStyle.YearsGroupView | VistaCalendarViewStyle.CenturyView;
			return VistaCalendarViewStyle;
		}
		void ResetAppearanceDropDownHeaderHighlight() { AppearanceDropDownHeaderHighlight.Reset(); }
		bool ShouldSerializeAppearanceDropDownHeaderHighlight() { return AppearanceDropDownHeaderHighlight.ShouldSerialize(); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RepositoryItemTimeEdit VistaTimeProperties { get { return CalendarTimeProperties; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditCalendarTimeProperties"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual RepositoryItemTimeEdit CalendarTimeProperties { get { return timeEditProperties;  } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditAppearanceDropDownHeaderHighlight"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceDropDownHeaderHighlight {
			get {
				return AppearanceCalendar.HeaderHighlighted;
			}
		}
		void ResetAppearanceWeekNumber() { AppearanceWeekNumber.Reset(); }
		bool ShouldSerializeAppearanceWeekNumber() { return AppearanceWeekNumber.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditAppearanceWeekNumber"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceWeekNumber {
			get {
				return AppearanceCalendar.WeekNumber;
			}
		}
		void ResetAppearanceDropDownHighlight() { AppearanceDropDownHighlight.Reset(); }
		bool ShouldSerializeAppearanceDropDownHighlight() { return AppearanceDropDownHighlight.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditAppearanceDropDownHighlight"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceDropDownHighlight {
			get {
				return AppearanceCalendar.DayCellHighlighted;
			}
		}
		void ResetAppearanceDropDownDisabledDate() { AppearanceDropDownDisabledDate.Reset(); }
		bool ShouldSerializeAppearanceDropDownDisabledDate() { return AppearanceDropDownDisabledDate.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditAppearanceDropDownDisabledDate"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceDropDownDisabledDate {
			get {
				return AppearanceCalendar.DayCellDisabled;
			}
		}
		void ResetAppearanceDropDownHeader() { AppearanceDropDownHeader.Reset(); }
		bool ShouldSerializeAppearanceDropDownHeader() { return AppearanceDropDownHeader.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditAppearanceDropDownHeader"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceDropDownHeader {
			get {
				return AppearanceCalendar.Header;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditVistaDisplayMode"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean VistaDisplayMode {
			get {
				if(CalendarView == Repository.CalendarView.Default)
					return DefaultBoolean.Default;
				return CalendarView == Repository.CalendarView.Vista ? DefaultBoolean.True : DefaultBoolean.False;
			}
			set {
				if(VistaDisplayMode == value) return;
				if(CalendarView == Repository.CalendarView.TouchUI)
					return;
				if(value == DefaultBoolean.Default)
					CalendarView = Repository.CalendarView.Default;
				else
					CalendarView = value == DefaultBoolean.False ? Repository.CalendarView.Classic : Repository.CalendarView.Vista;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditHighlightTodayCell"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean HighlightTodayCell {
			get { return highlightTodayCell; }
			set {
				if(HighlightTodayCell == value) return;
				highlightTodayCell = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual DefaultBoolean VistaEditTime {
			get { return CalendarTimeEditing; }
			set {
				CalendarTimeEditing = value;
			}
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean CalendarTimeEditing {
			get { return vistaEditTime; }
			set {
				if(CalendarTimeEditing == value) return;
				vistaEditTime = value;
				OnVistaEditTimeChanged();
				if(OwnerEdit!= null && OwnerEdit.PopupForm is TouchPopupDateEditForm)
					((TouchPopupDateEditForm)OwnerEdit.PopupForm).TouchCalendar.CreateProviders();
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditTouchUISecondIncrement"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(1)]
		public int TouchUISecondIncrement {
			get { return touchUISecondIncrement; }
			set {
				if(TouchUISecondIncrement == value)
					return;
				touchUISecondIncrement = CheckTimeIncrement(value);
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditTouchUIMinuteIncrement"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(1)]
		public int TouchUIMinuteIncrement {
			get { return touchUIMinuteIncrement; }
			set {
				if(TouchUIMinuteIncrement == value)
					return;
				touchUIMinuteIncrement = CheckTimeIncrement(value);
				OnPropertiesChanged();
			}
		}
		protected int CheckTimeIncrement(int desiredValue){
			return (desiredValue < 1 || desiredValue > 60 || 60 % desiredValue != 0) ? 1 : desiredValue;
		}
		CalendarView calendarView;
		[DefaultValue(CalendarView.Default), SmartTagProperty("Calendar View", "", SmartTagActionType.RefreshAfterExecute)]
		public CalendarView CalendarView {
			get { return calendarView; }
			set {
				if(CalendarView == value)
					return;
				calendarView = value;
				OnPropertiesChanged();
			}
		}
		protected virtual void OnVistaEditTimeChanged() {
		}
		protected internal virtual bool GetVistaEditTime() { return IsVistaDisplayMode() && VistaEditTime == DefaultBoolean.True; }
		protected internal virtual bool IsVistaDisplayMode() {
			if(VistaDisplayMode == DefaultBoolean.False) return false;
			if(VistaDisplayMode == DefaultBoolean.True) return true;
			return NativeVista.IsVista;
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "DateEdit"; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditShowToday"),
#endif
 DefaultValue(true)]
		public virtual bool ShowToday {
			get { return _showToday; }
			set {
				if(ShowToday == value) return;
				_showToday = value;
				OnPropertiesChanged();
			}
		}
		int rowCount = 1;
		[DXCategory(CategoryName.Behavior),  DefaultValue(1)]
		public virtual int RowCount {
			get { return rowCount; }
			set {
				if(value <= 0)
					value = 1;
				if(RowCount == value)
					return;
				rowCount = value;
				OnPropertiesChanged();
			}
		}
		int columnCount = 1;
		[DXCategory(CategoryName.Behavior),  DefaultValue(1)]
		public virtual int ColumnCount {
			get { return columnCount; }
			set {
				if(value <= 0)
					value = 1;
				if(ColumnCount == value)
					return;
				columnCount = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditShowWeekNumbers"),
#endif
 DefaultValue(false)]
		public virtual bool ShowWeekNumbers {
			get { return fShowWeekNumbers; }
			set {
				if(ShowWeekNumbers == value) return;
				fShowWeekNumbers = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditHighlightHolidays"),
#endif
 DefaultValue(true)]
		public virtual bool HighlightHolidays {
			get { return fHighlightHolidays; }
			set {
				if(HighlightHolidays == value) return;
				fHighlightHolidays = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditNullDate"),
#endif
		DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public virtual object NullDate {
			get { return _nullDate; }
			set {
				if(NullDate == value) return;
				_nullDate = value;
				OnPropertiesChanged();
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditDateOnError")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Obsolete(ObsoleteText.SRObsoleteDateOnError)]
		public virtual DateOnError DateOnError {
			get { return DateOnError.NullDate; }
			set { }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditWeekNumberRule"),
#endif
 DefaultValue(WeekNumberRule.Default)]
		public virtual WeekNumberRule WeekNumberRule {
			get { return weekNumberRule; }
			set {
				if(WeekNumberRule == value) return;
				weekNumberRule = value;
				OnPropertiesChanged();
			}
		}
		public event CustomWeekNumberEventHandler CustomWeekNumber {
			add { Events.AddHandler(customWeekNumber, value); }
			remove { Events.RemoveHandler(customWeekNumber, value); }
		}
		protected internal virtual void RaiseCustomWeekNumber(CustomWeekNumberEventArgs e) {
			CustomWeekNumberEventHandler handler = Events[customWeekNumber] as CustomWeekNumberEventHandler;
			if(handler != null)
				handler(this, e);
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditShowClear"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(true), RefreshProperties(RefreshProperties.All)]
		public virtual bool ShowClear {
			get { return IsNullInputAllowed; }
			set {
				AllowNullInput = value ? DefaultBoolean.Default : DefaultBoolean.False;
			}
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemDateEdit source = item as RepositoryItemDateEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this._nullDate = source.NullDate;
				this._showToday = source.ShowToday;
				this.fShowWeekNumbers = source.ShowWeekNumbers;
				this.fHighlightHolidays = source.HighlightHolidays;
				this.weekNumberRule = source.WeekNumberRule;
				this._minValue = source.MinValue;
				this._maxValue = source.MaxValue;
				this.AppearanceDropDownHeader.Assign(source.AppearanceDropDownHeader);
				this.AppearanceDropDownHighlight.Assign(source.AppearanceDropDownHighlight);
				this.AppearanceDropDownDisabledDate.Assign(source.AppearanceDropDownDisabledDate);
				this.AppearanceDropDownHeaderHighlight.Assign(source.AppearanceDropDownHeaderHighlight);
				this.AppearanceWeekNumber.Assign(source.AppearanceWeekNumber);
				this.vistaDisplayMode = source.VistaDisplayMode;
				this.vistaEditTime = source.VistaEditTime;
				this.timeEditProperties.Assign(source.VistaTimeProperties);
				this.CalendarView = source.CalendarView;
				this.VistaCalendarViewStyle = source.VistaCalendarViewStyle;
				this.TouchUISecondIncrement = source.TouchUISecondIncrement;
				this.TouchUIMinuteIncrement = source.TouchUIMinuteIncrement;
				this.weekDayAbbreviationLength = source.WeekDayAbbreviationLength;
				this.specialDateProvider = source.SpecialDateProvider;
				this.disabledDateProvider = source.DisabledDateProvider;
				this.cellAppearanceProvider = source.CellStyleProvider;
				this.drawCellLines = source.DrawCellLines;
				this.cellSize = source.CellSize;
				this.todayDate = source.TodayDate;
				this.showMonthNavigationButtons = source.ShowMonthNavigationButtons;
				this.showYearNavigationButtons = source.ShowYearNavigationButtons;
				this.calendarDateEditing = source.CalendarDateEditing;
				this.allowClickInactiveDays = source.AllowClickInactiveDays;
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(customDrawDayNumberCell, source.Events[customDrawDayNumberCell]);
			Events.AddHandler(dateTimeChanged, source.Events[dateTimeChanged]);
			Events.AddHandler(disabledCalendarDateCell,source.Events[disabledCalendarDateCell]);
			Events.AddHandler(specialCalendarDateCell, source.Events[specialCalendarDateCell]);
			Events.AddHandler(customWeekNumber, source.Events[customWeekNumber]);
		}
		bool ShouldSerializeMinValue() { return MinValue != DateTime.MinValue; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditMinValue"),
#endif
 DXCategory(CategoryName.Behavior), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public DateTime MinValue {
			get { return _minValue; }
			set {
				if(MinValue == value) return;
				_minValue = value;
				if(MaxValue != DateTime.MinValue && MaxValue < MinValue) MaxValue = MinValue;
				OnMinMaxChanged();
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeMaxValue() { return MaxValue != DateTime.MinValue; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditMaxValue"),
#endif
 DXCategory(CategoryName.Behavior), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public DateTime MaxValue {
			get { return _maxValue; }
			set {
				if(MaxValue == value) return;
				_maxValue = value;
				if(MaxValue != DateTime.MinValue && MaxValue < MinValue) MinValue = MaxValue;
				OnMinMaxChanged();
				OnPropertiesChanged();
			}
		}
		protected virtual void OnMinMaxChanged() {
			if(OwnerEdit != null) OwnerEdit.OnMinMaxChanged();
		}
		protected internal DateTime CorrectValue(DateTime value) {
			return Helpers.DateTimeHelper.FitDateTime(value, MinValue, MaxValue);
		}
		protected internal override void RaiseEditValueChanged(EventArgs e) {
			if(OwnerEdit != null && !IsNullValue(OwnerEdit.EditValue)) {
				DateTime currentValue = OwnerEdit.DateTime;
				DateTime checkedValue = CorrectValue(currentValue);
				if(currentValue != checkedValue)
					return;
			}
			base.RaiseEditValueChanged(e);
		}
		protected internal override bool CancelPopupInputOnButtonClose { get { return true; } }
		protected internal override bool AllowInputOnOpenPopup { get { return false; } }
		bool IsDateTimeFormat(FormatInfo info) {
			if(info.FormatType == FormatType.DateTime && info.FormatString == "d") return true;
			return false;
		}
		bool ShouldSerializeDisplayFormat() { return !IsDateTimeFormat(DisplayFormat); }
		[DXCategory(CategoryName.Format), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditDisplayFormat"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override FormatInfo DisplayFormat {
			get { return base.DisplayFormat; }
		}
		bool ShouldSerializeEditFormat() { return !IsDateTimeFormat(EditFormat); }
		[DXCategory(CategoryName.Format), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditEditFormat"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override FormatInfo EditFormat {
			get { return base.EditFormat; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditEditMask"),
#endif
		DXCategory(CategoryName.Format),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		DefaultValue("d"),
		]
		public string EditMask {
			get { return Mask.EditMask; }
			set { Mask.EditMask = value; }
		}
		protected override bool NeededKeysPopupContains(Keys key) {
			switch(key) {
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.Home:
				case Keys.PageDown:
				case Keys.PageUp:
				case Keys.End:
				case Keys.Enter:
					return true;
			}
			return base.NeededKeysPopupContains(key);
		}
		protected internal override bool IsReadOnlyAllowsDropDown {
			get {
				if(OwnerEdit == null || OwnerEdit.InplaceType != InplaceType.Standalone)
					return AllowDropDownWhenReadOnly == DefaultBoolean.True;
				return AllowDropDownWhenReadOnly != DefaultBoolean.False;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteDrawItem)]
		public event CustomDrawDayNumberCellEventHandler CustomDrawDayNumberCell {
			add { DrawItem += value; }
			remove { DrawItem -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditDrawItem"),
#endif
 DXCategory(CategoryName.Events)]
		public event CustomDrawDayNumberCellEventHandler DrawItem {
			add { Events.AddHandler(customDrawDayNumberCell, value); }
			remove { Events.RemoveHandler(customDrawDayNumberCell, value); }
		}
		[ DXCategory(CategoryName.Events)]
		public event DisableCalendarDateEventHandler DisableCalendarDate {
			add { Events.AddHandler(disabledCalendarDateCell, value); }
			remove { Events.RemoveHandler(disabledCalendarDateCell, value); }
		}
		[ DXCategory(CategoryName.Events)]
		public event SpecialCalendarDateEventHandler SpecialCalendarDate {
			add { Events.AddHandler(specialCalendarDateCell, value); }
			remove { Events.RemoveHandler(specialCalendarDateCell, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditDateTimeChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler DateTimeChanged {
			add { this.Events.AddHandler(dateTimeChanged, value); }
			remove { this.Events.RemoveHandler(dateTimeChanged, value); }
		}
		protected override void RaiseEditValueChangedCore(EventArgs e) {
			base.RaiseEditValueChangedCore(e);
			RaiseDateTimeChanged(e);
		}
		protected internal virtual void RaiseDateTimeChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[dateTimeChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseCustomDrawDayNumberCell(CustomDrawDayNumberCellEventArgs e) {
			CustomDrawDayNumberCellEventHandler handler = (CustomDrawDayNumberCellEventHandler)this.Events[customDrawDayNumberCell];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseDisableCalendarDate(DisableCalendarDateEventArgs e) {
			DisableCalendarDateEventHandler handler = (DisableCalendarDateEventHandler)this.Events[disabledCalendarDateCell];
			if (handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseSpecialCalendarDate(SpecialCalendarDateEventArgs e) {
			SpecialCalendarDateEventHandler handler = (SpecialCalendarDateEventHandler)this.Events[specialCalendarDateCell];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal override bool IsNullValue(object editValue) {
			if(base.IsNullValue(editValue))
				return true;
			if(ConvertToDateTime(editValue).Equals(NullDate))
				return true;
			return editValue != null && editValue.Equals(NullDate);
		}
		DateEditValueConverter converter;
		protected DateEditValueConverter Converter {
			get {
				if(converter == null)
					converter = CreateConverter();
				return converter;
			}
		}
		protected virtual DateEditValueConverter CreateConverter() {
			return new DateEditValueConverter(this);
		}
		protected internal virtual DateTime ConvertToDateTime(object val) {
			return Converter.ConvertToDateTime(val);
		}
		protected internal override bool IsNullInputAllowed {
			get {
				return AllowNullInput != DefaultBoolean.False;
			}
		}
		protected internal virtual CalendarView ActualCalendarView {
			get {
				if(CalendarView == Repository.CalendarView.Default)
					return NativeVista.IsVista? CalendarView.Vista: CalendarView.Classic;
				return CalendarView;
			}
		}
		protected internal virtual bool ShowTimeEdit {
			get { return ActualCalendarView == Repository.CalendarView.Vista && VistaEditTime == DefaultBoolean.True; }
		}
		ConvertEditValueEventArgs IDateTimeOwner.DoParseEditValue(object value) {
			return DoParseEditValue(value);
		}
		ConvertEditValueEventArgs IDateTimeOwner.DoFormatEditValue(object value) {
			return DoFormatEditValue(value);
		}
		void ICalendarAppearancesOwner.OnAppearanceChanged() {
			OnAppearanceSizeChanged(AppearanceCalendar, EventArgs.Empty);
		}
		bool ICalendarAppearancesOwner.IsLoading {
			get { return IsLoading; }
		}
		DayOfWeek firstDayOfWeek;
		public DayOfWeek FirstDayOfWeek {
			get { return firstDayOfWeek; }
			set {
				if(FirstDayOfWeek == value)
					return;
				firstDayOfWeek = value;
				OnPropertiesChanged();
			}
		}
		private bool showMonthNames;
		[DefaultValue(false)]
		public bool ShowMonthHeaders {
			get { return showMonthNames; }
			set {
				if(ShowMonthHeaders == value)
					return;
				showMonthNames = value;
				OnShowMonthNamesChanged();
			}
		}
		protected virtual void OnShowMonthNamesChanged() {
			OnPropertiesChanged();
		}
	}
	public interface IDateTimeOwner {
		object NullDate { get; }
		ConvertEditValueEventArgs DoParseEditValue(object value);
		ConvertEditValueEventArgs DoFormatEditValue(object value);
	}
	public class DateEditValueConverter {
		public DateEditValueConverter(IDateTimeOwner owner) {
			Owner = owner;
		}
		public IDateTimeOwner Owner {
			get;
			private set;
		}
		public DateTime ConvertToDateTime(object value) {
			object obj = ConvertToObject(Owner.DoParseEditValue(value));
			if(obj is DateTime)
				return (DateTime)obj;
			ConvertEditValueEventArgs args = Owner.DoFormatEditValue(obj);
			if(args.Value is DateTime)
				return (DateTime)args.Value;
			if(Owner.NullDate is DateTime)
				return (DateTime)Owner.NullDate;
			else
				return DateTime.MinValue;
		}
		protected internal virtual object ConvertToObject(ConvertEditValueEventArgs args) {
			object val = args.Value;
			if(args.Handled)
				return val;
			if(val == null || val == DBNull.Value)
				return null;
			if(val.Equals(Owner.NullDate))
				return null;
			if(val is string && ((string)val).Length == 0)
				return null;
			if(val is DateTime)
				return val;
			try {
				DateTime date;
				if(DateTime.TryParse(val.ToString(), out date))
					return date;
				else return null;
			}
			catch {
				return null;	
			}
		}
	}
	public class CustomWeekNumberEventArgs {
		public DateTime Date { get; internal set; }
		public int? WeekNumber { get; set; }
	}
	public delegate void CustomWeekNumberEventHandler(object sender, CustomWeekNumberEventArgs e);
}
namespace DevExpress.XtraEditors.Controls {
	public enum WeekNumberRule { Default = -1, FirstDay, FirstFullWeek, FirstFourDayWeek }
}
namespace DevExpress.Accessibility {
	public class DateEditAccessible : TextEditAccessible {
		public DateEditAccessible(RepositoryItemDateEdit dateEdit)
			: base(dateEdit) {
		}
		protected override AccessibleRole GetRole() {
			return AccessibleRole.DropList;
		}
		public DateEdit DateEdit { get { return Edit as DateEdit; } }
		public override string Value {
			get {
				PopupDateEditForm form = DateEdit.PopupForm as PopupDateEditForm;
				if(form != null && form.Calendar != null) return form.Calendar.DateTime.ToLongDateString();
				if(DateEdit != null && DateEdit.Text != string.Empty) return DateEdit.DateTime.ToLongDateString();
				return base.Value;
			}
		}
		protected override string GetName() {
			return Value;
		}
		protected override TextAccessible CreateTextAccessible() {
			return new DateEditTextAccessible(this, Item);
		}
	}
	public class DateEditTextAccessible : TextAccessible {
		DateEditAccessible dateEdit;
		public DateEditTextAccessible(DateEditAccessible dateEdit, RepositoryItemTextEdit textEdit) : base(textEdit) {
			this.dateEdit = dateEdit;
		}
		public DateEdit DateEdit { get { return Edit as DateEdit; } }
		public DateEditAccessible AccessibleDateEdit { get { return dateEdit; } }
		public override string Value { get { return AccessibleDateEdit.Value; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.DropList; }
		protected override string GetName() { return AccessibleDateEdit.Value; }
		protected override AccessibleStates GetState() {
			AccessibleStates st = AccessibleStates.Focusable;
			if(DateEdit != null && DateEdit.MaskBox != null && DateEdit.MaskBox.Focused) st |= AccessibleStates.Focused;
				return st;
		}
	}
}
namespace DevExpress.XtraEditors {
	[Flags]
	public enum VistaCalendarViewStyle { Default = 0, MonthView = 1, YearView = 2, QuarterView = 4, YearsGroupView = 8, CenturyView = 16, All = MonthView | QuarterView | YearView | YearsGroupView | CenturyView }
	public enum VistaCalendarInitialViewStyle { MonthView = 1, YearView = 2, QuarterView = 4, YearsGroupView = 8, CenturyView = 16 }
	public class DateEditFormatInfo : FormatInfo {
		public DateEditFormatInfo(IComponentLoading componentLoading) : base(componentLoading) {
			FormatType = Utils.FormatType.DateTime;
			FormatString = "d";
		}
		public DateEditFormatInfo() : base() {
			FormatType = Utils.FormatType.DateTime;
			FormatString = "d";
		}
		protected override void ResetFormatType() {
			FormatType = Utils.FormatType.DateTime;
		}
		public override bool ShouldSerialize() {
			return FormatType != Utils.FormatType.DateTime;
		}
		protected override bool ShouldSerializeFormatString() {
			return FormatString != "d";
		}
		protected override void ResetFormatString() {
			FormatString = "d";
		}
	}
	[DefaultBindingPropertyEx("DateTime"),
	 Designer("DevExpress.XtraEditors.Design.DateEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows an end-user to edit date/time values and select a date and time via a dropdown calendar."),
	 ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "DateEdit"),
	 ToolboxTabName(AssemblyInfo.DXTabCommon)
	]
	public class DateEdit : PopupBaseEdit {
		public DateEdit() {
			this.fOldEditValue = this.fEditValue = DateTime.Today;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "DateEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DateEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemDateEdit Properties { get { return base.Properties as RepositoryItemDateEdit; } }
		protected override PopupBaseForm CreatePopupForm() {
			if(Properties.CalendarView == CalendarView.TouchUI) 
				return new TouchPopupDateEditForm(this);
			return new PopupDateEditForm(this);
		}
		[Bindable(ControlConstants.NonObjectBindable), DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DateEditDateTime"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime DateTime {
			get {
				FlushPendingEditActions();
				return Properties.ConvertToDateTime(EditValue);
			}
			set { EditValue = value; }
		}
		protected internal virtual void AccessibleNotifyClients(AccessibleEvents events, int childId) {
			AccessibilityNotifyClients(events, childId);
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DateEditAccessible(Properties);
		}
		public override object EditValue {
			get { return base.EditValue; }
			set { base.EditValue = value; }
		}
		bool skipPressedSpace = false;
		protected override void OnKeyDown(KeyEventArgs e) {
			skipPressedSpace = false;
			if(Properties.IsVistaDisplayMode() && IsPopupOpen && e.KeyCode == Keys.Space) skipPressedSpace = true;
			if(IsPopupOpen) PopupForm.OnKeyDownCore(e);
			base.OnKeyDown(e);
		}
		[Browsable(false), Bindable(false)]
		public override string Text {
			get { return base.Text; }
		}
		protected override bool AcceptsSpace {
			get { return !skipPressedSpace; }
		}
		protected override object ExtractParsedValue(ConvertEditValueEventArgs e) {
			if(e.Handled)
				return e.Value;
			if(Properties.IsNullInputAllowed && Properties.IsNullValue(e.Value))
				return this.Properties.NullDate;
			return Properties.CorrectValue(Properties.ConvertToDateTime(e.Value));
		}
		protected override void OnValidatingCore(CancelEventArgs e) {
			base.OnValidatingCore(e);
			if(EditValue is DateTime) {
				DisableCalendarDateEventArgs calArgs = new DisableCalendarDateEventArgs(Convert.ToDateTime(EditValue), false, DateEditCalendarViewType.MonthInfo);
				Properties.RaiseDisableCalendarDate(calArgs);
				if(!e.Cancel)
					e.Cancel = calArgs.IsDisabled;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteDrawItem)]
		public event CustomDrawDayNumberCellEventHandler CustomDrawDayNumberCell {
			add { DrawItem += value; }
			remove { DrawItem -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DateEditDrawItem"),
#endif
 DXCategory(CategoryName.Events)]
		public event CustomDrawDayNumberCellEventHandler DrawItem {
			add { Properties.DrawItem += value; }
			remove { Properties.DrawItem -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DateEditDateTimeChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler DateTimeChanged {
			add { Properties.DateTimeChanged += value; }
			remove { Properties.DateTimeChanged -= value; }
		}
		[ DXCategory(CategoryName.Events)]
		public event DisableCalendarDateEventHandler DisableCalendarDate {
			add { Properties.DisableCalendarDate += value; }
			remove { Properties.DisableCalendarDate -= value; }
		}
		protected internal virtual void OnMinMaxChanged() {
			if(IsLoading) return;
			if(Properties.IsNullInputAllowed && Properties.IsNullValue(this.EditValue)) {
			} else {
				this.DateTime = Properties.CorrectValue(this.DateTime);
			}
		}
		protected override void OnMaskBox_MouseWheel(object sender, MouseEventArgs e) {
			DoMouseWheel(e);
		   base.OnMaskBox_MouseWheel(sender, e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			DoMouseWheel(e);
			base.OnMouseWheel(e);
		}
		void DoMouseWheel(MouseEventArgs e) {
			if(IsPopupOpen) {
				if(e is DXMouseEventArgs)
					((DXMouseEventArgs)e).Handled = true;
				if(PopupForm is TouchPopupForm)
					((TouchPopupForm)PopupForm).TouchCalendar.OnMouseWheelCore(e);
			}
		}
	}
	[ToolboxItem(false)]
	public class DateControl : BaseDateControl {
		bool showTodayButton;
		public DateControl() { }
		[DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public override bool ShowTodayButton {
			get { return showTodayButton; }
			set {
				if(ShowTodayButton == value) return;
				showTodayButton = value;
				LayoutChanged();
			}
		}
		protected override void Init() {
			this.showTodayButton = true;
			base.Init();
		}
	}
	[ToolboxItem(false)]
	public class BaseDateControl : CalendarControl { }
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class DateEditViewInfo : PopupBaseEditViewInfo {
		public DateEditViewInfo(RepositoryItem item) : base(item) {
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public class TouchPopupDateEditForm : TouchPopupForm {
		public TouchPopupDateEditForm(PopupBaseEdit ownerEdit)
			: base(ownerEdit) {
		}
		public DateEdit DateEdit { get { return (DateEdit)OwnerEdit; } }
		protected override void CreateTouchCalendar() {
			TouchCalendar = new DateEditTouchCalendar(this);
		}
		public override void SetDate(object val) {
			DateTime date = IsNull(val) ? DateTime.Now : DateEdit.Properties.ConvertToDateTime(val);
			if(TouchCalendar != null)
				TouchCalendar.SelectedDate = date;
		}
		protected override PopupBaseFormViewInfo CreateViewInfo() {
			return new TouchPopupDateEditFormViewInfo(this);
		}
		protected internal virtual bool RaiseDisableCalendarDate(DisableCalendarDateEventArgs e) {
			if(Properties is RepositoryItemDateEdit) {
				((RepositoryItemDateEdit)Properties).RaiseDisableCalendarDate(e);
			}
			return e.IsDisabled;
		}
	}
	public class TouchPopupDateEditFormViewInfo : CustomBlobPopupFormViewInfo {
		public TouchPopupDateEditFormViewInfo(PopupBaseForm form) : base(form) { }
		protected override int SizeBarIndent {
			get {
				return 21;
			}
		}
		protected override int SizeBarSideIndent {
			get {
				return 12;
			}
		}
		protected override int GetButtonXIndent() {
			return Form.AllowSizing ? 8 : 0;
		}
	}
	public class DateEditTouchCalendar : TouchCalendar {
		protected TouchPopupDateEditForm Form { get; set; }
		public DateEditTouchCalendar(TouchPopupDateEditForm form)
			: base() {
			Form = form;
			IsTimeProviderAdded = false;
		}
		protected internal DateEdit OwnerEdit { get { return Form.DateEdit; } }
		protected override bool ShouldUpdatePanels {
			get {
				if(OwnerEdit == null) return false;
				return OwnerEdit.Properties.MinValue != DateTime.MinValue || OwnerEdit.Properties.MaxValue != DateTime.MinValue;
			}
		}
		protected internal override int GetSecondIncrement() {
			return OwnerEdit.Properties.TouchUISecondIncrement;
		}
		protected internal override int GetMinuteIncrement() {
			return OwnerEdit.Properties.TouchUIMinuteIncrement;
		}
		protected override void CheckContainer() {
			if(PickContainer.IsReady)
				OnDateChanged();
		}
		public override bool ShowTime() {
			if(Form == null || OwnerEdit == null) {
				return false;
			}
			return OwnerEdit.Properties.VistaEditTime == DefaultBoolean.True || OwnerEdit.Properties.IsVistaDisplayMode();
		}
		public override void UpdateMaskManager() {
			if(Form == null || Form.DateEdit == null)
				return;
			MaskManager = new DateTimeMaskManager(Form.DateEdit.Properties.Mask.EditMask, true, CultureInfo.CurrentCulture, false);
		}
		protected bool IsTimeProviderAdded { get; set; }
		public override void AddNewProvider(DateTimeMaskFormatElementEditable editableFormat) {
			if(Form == null || Form.DateEdit == null || !ShowTime()) {
				if(IsTimeProvider(editableFormat)) return;
			}
			IItemsProvider provider = CreateNewProvider(editableFormat);
			if(provider != null) {
				if(ShouldInsertProvider(provider))
					Providers.Insert(firstTimeProviderIndex, provider);
				else Providers.Add(provider);
				if(IsTimeProvider(editableFormat)) {
					if(firstTimeProviderIndex == -1) firstTimeProviderIndex = Providers.Count - 1;
					IsTimeProviderAdded = true;
				}
				TotalProviders += 1;
			}
		}
		protected virtual bool ShouldInsertProvider(IItemsProvider provider) {
			return provider is MeridiemItemsProvider && this.firstTimeProviderIndex != -1 && OwnerEdit != null && OwnerEdit.IsRightToLeft;
		}
		int firstTimeProviderIndex = -1;
		protected override void InitializeItemsControl() {
			this.firstTimeProviderIndex = -1;
			base.InitializeItemsControl();
		}
		protected override void AddCustomProviders() {
			if(ShowTime() && !IsTimeProviderAdded) {
				this.firstTimeProviderIndex = -1;
				foreach(var format in TimeFormats) {
					var editableFormat = format as DateTimeMaskFormatElementEditable;
					if(editableFormat == null || !IsTimeProvider(editableFormat)) continue;
					AddNewProvider(editableFormat);
				}
			}
		}
		DateTimeMaskManager timeMaskManager;
		IEnumerable<DateTimeMaskFormatElement> TimeFormats {
			get {
				if(timeMaskManager == null)
					timeMaskManager = new DateTimeMaskManager("t", true, CultureInfo.CurrentCulture, false);
				return DateTimeMaskManagerHelper.GetFormatInfo(timeMaskManager);
			}
		}
		protected virtual bool IsTimeProvider(DateTimeMaskFormatElementEditable editableFormat) {
			if(editableFormat is DateTimeMaskFormatElement_AmPm) return true;
			if(editableFormat is DateTimeMaskFormatElement_h12) return true;
			if(editableFormat is DateTimeMaskFormatElement_H24) return true;
			if(editableFormat is DateTimeMaskFormatElement_Min) return true;
			if(editableFormat is DateTimeMaskFormatElement_s) return true;
			return false;
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			SetDate();
		}
		public override bool RaiseDisableCalendarDate(DateTime date) {
			if(Form == null) return base.RaiseDisableCalendarDate(date);
			return Form.RaiseDisableCalendarDate(new DisableCalendarDateEventArgs(date, false, DateEditCalendarViewType.MonthInfo));
		}
	}
	[ToolboxItem(false)]
	public class VistaPopupDateEditForm : PopupDateEditForm {
		public VistaPopupDateEditForm(DateEdit ownerEdit) : base(ownerEdit) { }
	}
	[ToolboxItem(false)]
	public class PopupCalendarControl : CalendarControl {
		protected internal override bool IsPopupCalendar {
			get {
				return true;
			}
		}
	}
	[ToolboxItem(false)]
	public class PopupDateEditForm : PopupBaseForm {
		CalendarControl calendar;
		public PopupDateEditForm(DateEdit ownerEdit) : base(ownerEdit) {
			this.calendar = CreateCalendar();
			calendar.DateTime = OwnerEdit.DateTime;
			AssignCalendarProperties();
			SubscribeCalendarEvents();
			this.Controls.Add(Calendar);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Calendar != null) {
					UnsubscribeCalendarEvents();
					Calendar.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected virtual void UnsubscribeCalendarEvents() {
			Calendar.DateTimeCommit -= OnCalendar_EditDateModified;
			Calendar.CustomDrawDayNumberCell -= OnCalendar_CustomDrawDayNumberCell;
			Calendar.OkClick -= OnCalendarOkClick;
		}
		protected virtual void AssignCalendarProperties() {
			RepositoryItemDateEdit item = (RepositoryItemDateEdit)Properties;
			Calendar.Appearance.Assign(Properties.AppearanceDropDown);
			Calendar.CalendarAppearance.Assign(item.AppearanceCalendar);
			Calendar.CalendarTimeEditing = item.CalendarTimeEditing;
			Calendar.CalendarTimeProperties.Assign(item.CalendarTimeProperties);
			Calendar.CalendarView = item.CalendarView;
			Calendar.ColumnCount = item.ColumnCount;
			Calendar.TodayDate = item.TodayDate;
			Calendar.EditValue = OwnerEdit.EditValue == null? Calendar.GetTodayDate(): OwnerEdit.EditValue;
			Calendar.AllowAnimatedContentChange = item.AllowAnimatedContentChange;
			Calendar.FirstDayOfWeek = item.FirstDayOfWeek;
			Calendar.HighlightHolidays = item.HighlightHolidays;
			Calendar.HighlightTodayCell = item.HighlightTodayCell;
			Calendar.MaxValue = item.MaxValue != item.MinValue? item.MaxValue: DateTime.MaxValue;
			Calendar.MinValue = item.MinValue;
			Calendar.NullDate = item.NullDate;
			Calendar.ReadOnly = item.ReadOnly;
			Calendar.RightToLeft = WindowsFormsSettings.GetRightToLeft(OwnerEdit);
			Calendar.RowCount = item.RowCount;
			Calendar.SelectionMode = item.SelectionMode;
			Calendar.ShowClearButton = item.ShowClear;
			Calendar.ShowTodayButton = item.ShowToday;
			Calendar.ShowWeekNumbers = item.ShowWeekNumbers;
			Calendar.VistaCalendarInitialViewStyle = item.VistaCalendarInitialViewStyle;
			Calendar.VistaCalendarViewStyle = item.VistaCalendarViewStyle;
			Calendar.WeekNumberRule = item.WeekNumberRule;
			Calendar.DrawCellLines = item.DrawCellLines;
			Calendar.CellSize = item.CellSize;
			Calendar.WeekDayAbbreviationLength = item.WeekDayAbbreviationLength;
			Calendar.SpecialDateProvider = item.SpecialDateProvider;
			Calendar.DisabledDateProvider = item.DisabledDateProvider;
			Calendar.CellStyleProvider = item.CellStyleProvider;
			Calendar.ShowMonthNavigationButtons = item.ShowMonthNavigationButtons;
			Calendar.ShowYearNavigationButtons = item.ShowYearNavigationButtons;
			Calendar.CalendarDateEditing = item.CalendarDateEditing;
			Calendar.AllowClickInactiveDays = item.AllowClickInactiveDays;
			Calendar.ReadOnly = item.ReadOnly;
			Calendar.ShowMonthHeaders = item.ShowMonthHeaders;
		}
		protected virtual void SubscribeCalendarEvents() {
			Calendar.DateTimeCommit += OnCalendar_EditDateModified;
			Calendar.CustomDrawDayNumberCell += OnCalendar_CustomDrawDayNumberCell;
			Calendar.DisableCalendarDate += Calendar_DisableCalendarDate;
			Calendar.OkClick += OnCalendarOkClick;
		}
		void Calendar_DisableCalendarDate(object sender, DisableCalendarDateEventArgs e) {
		}		
		protected virtual void OnCalendarOkClick(object sender, EventArgs e) {
			if(OwnerEdit != null)
				OwnerEdit.ClosePopup();
		}
		protected virtual void OnCalendar_EditDateModified(object sender, EventArgs e) {
			if(OwnerEdit.Properties.IsVistaDisplayMode() && OwnerEdit.Properties.GetVistaEditTime()) return;
			if(OwnerEdit.Properties.ReadOnly)
				OwnerEdit.CancelPopup();
			else
				OwnerEdit.ClosePopup();
		}
		protected virtual CalendarControl CreateCalendar() {
			return new PopupCalendarControl();
		}
		protected virtual void OnCalendar_CustomDrawDayNumberCell(object sender, CustomDrawDayNumberCellEventArgs e) {
			OwnerEdit.Properties.RaiseCustomDrawDayNumberCell(e);
		}
		protected virtual void ResetState() {
			AssignCalendarProperties();
			DateTime now = GetTodayDateTime();
			DateTime date = OwnerEdit.EditValue == null || object.Equals(OwnerEdit.EditValue, ((RepositoryItemDateEdit)Properties).NullDate)? OwnerEdit.Properties.NullDateCalendarValue: OwnerEdit.DateTime;
			if(date == DateTime.MinValue) date = now;
			Calendar.ResetState(OwnerEdit.EditValue !=null? OwnerEdit.EditValue: date, date);
		}
		protected virtual DateTime GetTodayDateTime() {
			if(Calendar.TodayDate != DateTime.MinValue)
				return new DateTime(Calendar.TodayDate.Ticks, OwnerEdit.Properties.NullDateCalendarValue.Kind);
			return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, OwnerEdit.Properties.NullDateCalendarValue.Kind);
		}
		public override void ShowPopupForm() {
			ResetState();
			base.ShowPopupForm();
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Tab && OwnerEdit.AllowPopupTabOut) {
				Calendar.ProcessTabPopupKey();
				e.Handled = true;
			}
			Calendar.ProcessKeyDown(e);
			base.ProcessKeyDown(e);
		}
		protected override Control EmbeddedControl { get { return Calendar; } }
		protected override Size CalcFormSizeCore() {
			ResetState();
			return CalcFormSize(Calendar.CalcBestSize());
		}
		[DXCategory(CategoryName.Appearance)]
		public override object ResultValue {
			get {
				if(Calendar.IsDateCleared) return ((RepositoryItemDateEdit)Properties).NullDate;
				return Calendar.EditValue;
			}
		}
		[Browsable(false)]
		public CalendarControl Calendar { get { return calendar; } }
		[Browsable(false)]
		public new DateEdit OwnerEdit { get { return base.OwnerEdit as DateEdit; } }
	}
}
namespace DevExpress.XtraEditors.Helpers {
	public sealed class DateTimeHelper {
		private DateTimeHelper() { }
		static DateTime AssignDatePart(DateTime target, DateTime source) {
			return new DateTime(source.Year, source.Month, source.Day, target.Hour, target.Minute, target.Second, target.Millisecond);
		}
		public static DateTime FitDateTime(DateTime value, DateTime minValue, DateTime maxValue) {
			if(value.Date < minValue.Date) value = AssignDatePart(value, minValue);
			if(maxValue != DateTime.MinValue && value.Date > maxValue.Date) value = AssignDatePart(value, maxValue);
			if(value < minValue) value = minValue;
			if(maxValue != DateTime.MinValue && value > maxValue) value = maxValue;
			return value;
		}
		public static bool HasCollision(params Rectangle[] rects) {
			for(int i = 0; i < rects.Length; i++) {
				for(int j = 0; j < rects.Length && i != j; j++) {
					Rectangle r = Rectangle.Intersect(rects[i], rects[j]);
					if(!r.IsEmpty) return true;
				}
			}
			return false;
		}
	}
}

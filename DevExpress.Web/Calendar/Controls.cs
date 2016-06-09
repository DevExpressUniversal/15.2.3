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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using System.ComponentModel;
namespace DevExpress.Web.Internal {
	public class CalendarViewInfo {
		private ASPxCalendar calendar = null;
		private DateTime visibleDate = DateTime.MinValue;
		private int column, row;
		private bool isLowBoundary;
		private bool isHightBoundary;
		public CalendarViewInfo(ASPxCalendar calendar)
			: this(calendar, calendar.EffectiveVisibleDate, 0, 0) {
		}
		public CalendarViewInfo(ASPxCalendar calendar, DateTime visibleDate)
			: this(calendar, visibleDate, 0, 0) {
		}
		public CalendarViewInfo(ASPxCalendar calendar, DateTime visibleDate, int row, int column) {
			this.calendar = calendar;
			this.visibleDate = visibleDate;
			this.column = column;
			this.row = row;
			int temp = column + row;
			this.isLowBoundary = temp == 0;
			this.isHightBoundary = temp == calendar.Columns + calendar.Rows - 2;
		}
		public ASPxCalendar Calendar {
			get { return calendar; }
		}
		public DateTime VisibleDate {
			get { return visibleDate; }
			set { visibleDate = value; }
		}
		public int Column {
			get { return column; }
		}
		public int Row {
			get { return row; }
		}
		public bool IsLowBoundary {
			get { return isLowBoundary; }
		}
		public bool IsHighBoundary {
			get { return isHightBoundary; }
		}
		public DateTime GetStartDate() {
			return CommonUtils.GetFirstDateOfMonthView(VisibleDate.Year, VisibleDate.Month, Calendar.GetFirstDayOfWeek());
		}
		public string GetIDPostfix() {
			return string.Format("_{0}x{1}", Row, Column);
		}
	}
	public abstract class CalendarViewTable : InternalTable {
		private CalendarViewInfo viewInfo = null;
		public CalendarViewTable(CalendarViewInfo viewInfo)
			: base() {
			this.viewInfo = viewInfo;
		}
		protected ASPxCalendar Calendar {
			get { return ViewInfo.Calendar; }
		}
		protected CalendarViewInfo ViewInfo {
			get { return viewInfo; }
		}
	}
	public abstract class CalendarViewCell : InternalTableCell {
		private CalendarViewInfo viewInfo = null;
		public CalendarViewCell(CalendarViewInfo viewInfo)
			: base() {
			this.viewInfo = viewInfo;
		}
		protected ASPxCalendar Calendar {
			get { return ViewInfo.Calendar; }
		}
		protected CalendarViewInfo ViewInfo {
			get { return viewInfo; }
		}
		protected bool IsRightToLeft {
			get { return (Calendar as ISkinOwner).IsRightToLeft(); }
		}
	}
	public class CalendarEtalonCells : InternalTable {
		private Dictionary<string, AppearanceStyleBase> etalonInfo;
		private ASPxCalendar calendar = null;
		private TableRow mainRow = RenderUtils.CreateTableRow();
		public CalendarEtalonCells(ASPxCalendar calendar) {
			this.calendar = calendar;
		}
		protected ASPxCalendar Calendar {
			get { return calendar; }
		}
		const int EtalonInfoCount = 12; 
		protected Dictionary<string, AppearanceStyleBase> EtalonInfo {
			get {
				if(etalonInfo == null) {
					etalonInfo = new Dictionary<string, AppearanceStyleBase>();
					etalonInfo.Add("D", Calendar.GetDayStyle());
					etalonInfo.Add("DS", Calendar.GetDaySelectedStyle());
					etalonInfo.Add("DA", Calendar.GetDayOtherMonthStyle());
					etalonInfo.Add("DW", Calendar.GetDayWeekendStyle());
					etalonInfo.Add("DO", Calendar.GetDayOutOfRangeStyle());
					etalonInfo.Add("DDD", Calendar.GetDayDisabledStyle());
					etalonInfo.Add("DT", Calendar.GetTodayStyle());
					etalonInfo.Add("DD", Calendar.GetDisabledCssStyle());
					etalonInfo.Add("FNM", Calendar.GetFastNavMonthStyle());
					etalonInfo.Add("FNMS", Calendar.GetFastNavMonthSelectedStyle());
					etalonInfo.Add("FNY", Calendar.GetFastNavYearStyle());
					etalonInfo.Add("FNYS", Calendar.GetFastNavYearSelectedStyle());
					if(EtalonInfoCount != etalonInfo.Count)
						throw new InvalidOperationException();
				}
				return etalonInfo;
			}
		}
		protected TableRow MainRow {
			get { return mainRow; }
		}
		protected override void CreateControlHierarchy() {
			for(int i = 0; i < EtalonInfoCount; i++) {
				TableCell cell = RenderUtils.CreateTableCell();
				MainRow.Cells.Add(cell);
			}
			Rows.Add(MainRow);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.SetStyleStringAttribute(this, "display", "none");
			int i = 0;
			TableCell cell;
			foreach (string id in EtalonInfo.Keys) {
				cell = MainRow.Cells[i++];
				cell.ID = "EC_" + id;
				EtalonInfo[id].AssignToControl(cell, true);
			}
		}
	}
	public class CalendarHeaderButton : CalendarViewCell {
		private string idSuffix;
		private int offset;
		private Image imageControl = RenderUtils.CreateImage();
		ImageCreator imageCreator;
		public delegate ImageProperties ImageCreator();
		public CalendarHeaderButton(CalendarViewInfo viewInfo, string idSuffix, ImageCreator imageCreator, int offset)
			: base(viewInfo) {
			this.idSuffix = idSuffix;
			this.offset = offset;
			this.imageCreator = imageCreator;
		}
		public Image ImageControl { get { return imageControl; } }
		protected override void CreateControlHierarchy() {
			ID = GetId();
			Controls.Add(ImageControl);
			ImageControl.ID = GetImageId();
		}
		protected override void PrepareControlHierarchy() {
			bool isVisible = ViewInfo.Column == 0 && this.offset < 0
				|| ViewInfo.Column == Calendar.Columns - 1 && this.offset > 0;
			if (IsEnabled && isVisible) {
				string handler = Calendar.GetNavigationOnClick(this.offset);
				RenderUtils.SetStringAttribute(this, "onclick", handler);
				if (RenderUtils.Browser.IsIE && RenderUtils.Browser.Version < 9)
					RenderUtils.SetStringAttribute(this, "ondblclick", handler);
			}
			RenderUtils.AppendDefaultDXClassName(this, Calendar.GetCssClassNamePrefix());
			this.imageCreator().AssignToControl(ImageControl, DesignMode, !ViewInfo.Calendar.Enabled);
			if (!isVisible)
				RenderUtils.SetStyleStringAttribute(ImageControl, "visibility", "hidden");
		}
		protected string GetId() {
			return this.idSuffix + (Calendar.IsMultiView() ? ViewInfo.GetIDPostfix() : string.Empty);
		}
		protected string GetImageId() {
			return GetId() + ASPxCalendar.ButtonImageIdPostfix;
		}
	}
	public class CalendarHeaderTitle : CalendarViewCell {
		private string idSuffix;
		private string labelIdSuffix;
		private Label titleLabel = RenderUtils.CreateLabel();
		public CalendarHeaderTitle(CalendarViewInfo viewInfo, string idSuffix, string labelIdSuffix)
			: base(viewInfo) {
			this.idSuffix = idSuffix;
			this.labelIdSuffix = labelIdSuffix;
		}
		protected Label TitleLabel {
			get { return titleLabel; }
		}
		protected virtual bool AssignClickHandler { get { return Calendar.IsFastNavEnabled(); } }
		protected override void CreateControlHierarchy() {
			ID = GetId();
			Controls.Add(TitleLabel);
			TitleLabel.ID = GetLabelId();
		}
		protected override void PrepareControlHierarchy() {			
			Width = Unit.Percentage(100);
			RenderUtils.SetCursor(this, RenderUtils.GetDefaultCursor());
			TitleLabel.Text = GetTitleText();
			if(AssignClickHandler) {
				RenderUtils.SetCursor(TitleLabel, RenderUtils.GetPointerCursor());
				RenderUtils.SetStringAttribute(TitleLabel, "onclick", Calendar.GetTitleOnClick(ViewInfo.Row, ViewInfo.Column));
			}
			CalendarHeaderFooterStyle style = Calendar.GetHeaderStyle();
			style.AssignToControl(this, AttributesRange.Font);
			RenderUtils.SetHorizontalAlign(this, style.HorizontalAlign);
			RenderUtils.AppendDefaultDXClassName(this, Calendar.GetCssClassNamePrefix());
		}
		protected virtual string GetTitleText() {
			return ViewInfo.VisibleDate.ToString(IsRightToLeft ? "MMMM yyyy" : CultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern); 
		}
		protected string GetLabelId() {
			return this.labelIdSuffix + (Calendar.IsMultiView() ? ViewInfo.GetIDPostfix() : string.Empty);
		}
		protected string GetId() {
			return this.idSuffix + (Calendar.IsMultiView() ? ViewInfo.GetIDPostfix() : string.Empty);
		}
	}
	public class TimeSectionCalendarHeaderTitle : CalendarHeaderTitle {
		public TimeSectionCalendarHeaderTitle(CalendarViewInfo info, string idSuffix, string labelIdSuffix)
			: base(info, idSuffix, labelIdSuffix) {
		}
		protected override bool AssignClickHandler { get { return false; } }
		protected override string GetTitleText() {
			return "&nbsp;"; 
		}
	}
	public class CalendarHeader : CalendarViewTable {
		private TableRow mainRow = RenderUtils.CreateTableRow();
		private List<TableCell> spacers = new List<TableCell>();
		public CalendarHeader(CalendarViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected TableRow MainRow {
			get { return mainRow; }
		}
		protected List<TableCell> Spacers { get { return spacers; } }
		protected override void CreateControlHierarchy() {
			CreateTitle();
			if (ViewInfo.Row == 0) {
				if (Calendar.EnableMonthNavigation)
					CreateMonthNavigation();
				if (Calendar.EnableYearNavigation)
					CreateYearNavigation();
			}
			Rows.Add(MainRow);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.CollapseAndRemovePadding(this);
			this.Width = Unit.Percentage(100);
			PrepareSpacers();
		}
		protected void CreateTitle() {
			MainRow.Cells.Add(CreateCalendarHeaderTitle());
		}
		protected virtual CalendarHeaderTitle CreateCalendarHeaderTitle() {
			return new CalendarHeaderTitle(ViewInfo, "TC", "T");
		}
		protected void CreateMonthNavigation() {
			CalendarHeaderButton prevButton = CreateButton(ASPxCalendar.PrevMonthCellIdSufix, Calendar.GetPrevMonthImage, -1);
			CalendarHeaderButton nextButton = CreateButton(ASPxCalendar.NextMonthCellIdSufix, Calendar.GetNextMonthImage, 1);
			MainRow.Cells.AddAt(0, prevButton);
			MainRow.Cells.Add(nextButton);
		}
		protected void CreateYearNavigation() {
			CalendarHeaderButton prevButton = CreateButton(ASPxCalendar.PrevYearCellIdSufix, Calendar.GetPrevYearImage, -12);
			CalendarHeaderButton nextButton = CreateButton(ASPxCalendar.NextYearCellIdSufix, Calendar.GetNextYearImage, 12);
			if (Calendar.EnableMonthNavigation)
				MainRow.Cells.AddAt(0, CreateSpacer());
			MainRow.Cells.AddAt(0, prevButton);
			if (Calendar.EnableMonthNavigation)
				MainRow.Cells.Add(CreateSpacer());
			MainRow.Cells.Add(nextButton);
		}
		protected virtual CalendarHeaderButton CreateButton(string idSuffix, CalendarHeaderButton.ImageCreator imageCreator, int offset) {
			return new CalendarHeaderButton(ViewInfo, idSuffix, imageCreator, offset);
		}
		protected TableCell CreateSpacer() {
			TableCell cell = RenderUtils.CreateIndentCell();
			Spacers.Add(cell);
			return cell;
		}
		protected void PrepareSpacers() {
			Unit spacing = Calendar.GetHeaderStyle().Spacing;
			foreach(TableCell spacer in Spacers) {
				RenderUtils.PrepareIndentCell(spacer, spacing, false);
				RenderUtils.AppendDefaultDXClassName(spacer, EditorStyles.CalendarHeaderSpacerClassName);
			}
		}
	}
	public class TimeSectionCalendarHeader : CalendarHeader {
		List<CalendarHeaderButton> buttons;
		public TimeSectionCalendarHeader(CalendarViewInfo info)
			: base(info) {
			this.buttons = new List<CalendarHeaderButton>();
		}
		protected List<CalendarHeaderButton> Buttons { get { return buttons; } }
		protected override CalendarHeaderButton CreateButton(string idSuffix, CalendarHeaderButton.ImageCreator imageCreator, int offset) {
			CalendarHeaderButton button = base.CreateButton("TS" + idSuffix, imageCreator, 0);
			Buttons.Add(button);
			return button;
		}
		protected override CalendarHeaderTitle CreateCalendarHeaderTitle() {
			return new TimeSectionCalendarHeaderTitle(ViewInfo, "TSTC", "TST");
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			foreach(CalendarHeaderButton button in Buttons)
				RenderUtils.SetStyleStringAttribute(button.ImageControl, "width", "1px", true);
			RenderUtils.SetStyleStringAttribute(this, "visibility", "hidden");
		}
	}
	public class CalendarDayCell : CalendarViewCell {
		DayData DayData;
		InternalHyperLink HyperLink;
		public CalendarDayCell(CalendarViewInfo viewInfo, DateTime date)
			: this(new DayData(viewInfo, date)) {
		}
		internal CalendarDayCell(DayData dayData)
			: base(dayData.ViewInfo) {
			DayData = dayData;
		}
		public DateTime DateTime { get { return DayData.Date; } }
		public bool IsOtherMonthDay { get { return DayData.IsOtherMonthDay; } }
		public bool IsSelected { get { return DayData.IsSelected; } }
		public bool IsWeekend {
			get { return DayData.Weekend; }
			set { DayData.Weekend = value; }
		}
		protected override void CreateControlHierarchy() {
			if(DayData.IsDisplayed) {
				Calendar.RaiseDayCellCreated(DayData, this);
				if(Controls.Count < 1) {
					HyperLink = new InternalHyperLink();
					HyperLink.EnableViewState = false;
					Legacy_UseDayRenderOnCreate();
					Controls.Add(HyperLink);
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			GetMergedStyle().AssignToControl(this, true);
			if(HyperLink != null) {
				HyperLink.Text = DayData.DisplayText;
				HyperLink.NavigateUrl = DayData.NavigateUrl;
				HyperLink.Target = DayData.NavigateUrlTarget;
				Legacy_UseDayRenderOnPrepare();
			}
			if(DayData.IsDisplayed)
				Calendar.RaiseDayCellPrepared(DayData, this, HyperLink);
			if(Controls.Count < 1 && String.IsNullOrEmpty(Text)) {
				Text = "&nbsp;";
			}
		}
		CalendarElementStyle GetMergedStyle() {
			CalendarElementStyle style = new CalendarElementStyle();
			style.CopyFrom(Calendar.GetDayStyle());
			if(DayData.IsDisplayed) {
				if(IsWeekend)
					style.CopyFrom(Calendar.GetDayWeekendStyle());
				if(IsOtherMonthDay)
					style.CopyFrom(Calendar.GetDayOtherMonthStyle());
				if(!Calendar.IsDateInRange(DateTime))
					style.CopyFrom(Calendar.GetDayOutOfRangeStyle());
				if(Calendar.IsInDisabledDates(DateTime) || DayData.IsDisabled)
					style.CopyFrom(Calendar.GetDayDisabledStyle());
				if (DateTime.Date == GetActualTodayDate())
					style.CopyFrom(Calendar.GetTodayStyle());
				if(IsSelected && (DesignMode || !Calendar.IsEnabled()))
					style.CopyFrom(Calendar.GetDaySelectedStyle());
				Legacy_ApplyDayRenderStyle(style);
			}
			return style;
		}
		protected virtual DateTime GetActualTodayDate() {
			return Calendar.GetActualTodayDate();
		}
		#region Legacy
		DayRenderEventArgs Legacy_RenderEventArgs;
		void Legacy_UseDayRenderOnCreate() {
			DayRenderEventHandler handler = Calendar.Legacy_DayRenderEventHandler;
			bool shouldRaise = Calendar.PreRendered || Page != null && Page.IsCallback;
			if(shouldRaise && handler != null) {
				Legacy_RenderEventArgs = new DayRenderEventArgs(HyperLink.Controls, this, DayData.DisplayText);
				handler(Calendar, Legacy_RenderEventArgs);
			}
		}
		void Legacy_ApplyDayRenderStyle(AppearanceStyleBase target) {
			if(Legacy_RenderEventArgs != null)
				target.CopyFrom(Legacy_RenderEventArgs.Style);
		}
		void Legacy_UseDayRenderOnPrepare() {
			if(Legacy_RenderEventArgs == null || Legacy_RenderEventArgs.Controls.Count > 0)
				return;
			HyperLink.Text = Legacy_RenderEventArgs.Text;
			HyperLink.NavigateUrl = Legacy_RenderEventArgs.NavigateUrl;
			HyperLink.Target = Legacy_RenderEventArgs.Target;
			Legacy_RenderEventArgs.Style.AssignToHyperLink(HyperLink);
		}
		#endregion
	}
	public class CalendarWeek : InternalTableRow {
		private CalendarViewInfo viewInfo = null;
		private int number;
		private List<CalendarDayCell> days = new List<CalendarDayCell>(7);
		private TableCell weekNumberCell = null;
		public CalendarWeek(CalendarViewInfo viewInfo, int number)
			: base() {
			this.viewInfo = viewInfo;
			this.number = number;
		}
		protected ASPxCalendar Calendar {
			get { return ViewInfo.Calendar; }
		}
		protected CalendarViewInfo ViewInfo {
			get { return viewInfo; }
		}
		public int Number {
			get { return number; }
		}
		public List<CalendarDayCell> Days {
			get { return days; }
		}
		protected TableCell WeekNumberCell {
			get { return weekNumberCell; }
		}
		protected override void CreateControlHierarchy() {
			if (Calendar.ShowWeekNumbers) {
				this.weekNumberCell = Calendar.IsAccessibilityCompliantRender() ? RenderUtils.CreateTableHeaderCell("row") : RenderUtils.CreateTableCell();
				Cells.Add(WeekNumberCell);
			}
			foreach (CalendarDayCell day in Days)
				Cells.Add(day);
		}
		protected override void PrepareControlHierarchy() {
			if (WeekNumberCell != null) {
				WeekNumberCell.Text = (Number < 10 ? "0" : "") + Number.ToString();
				Calendar.GetWeekNumberStyle().AssignToControl(WeekNumberCell, true);
			}
		}
	}
	public class CalendarMonth : CalendarViewTable {
		private List<CalendarWeek> weeks = null;
		private TableRow dayNamesRow = null;
		public CalendarMonth(CalendarViewInfo viewInfo)
			: base(viewInfo) {
		}
		public List<CalendarWeek> Weeks {
			get {
				if (weeks == null)
					weeks = new List<CalendarWeek>(CalendarView.WeekCount);
				return weeks;
			}
		}
		protected TableRow DayNamesRow {
			get { return dayNamesRow; }
		}
		protected override void CreateControlHierarchy() {
			ID = "mt";
			if(Calendar.IsMultiView())
				ID += ViewInfo.GetIDPostfix();
			Fill();
			if (Calendar.ShowDayHeaders) {
				this.dayNamesRow = RenderUtils.CreateTableRow();
				if (Calendar.ShowWeekNumbers)
					DayNamesRow.Cells.Add(RenderUtils.CreateTableCell());
				for (int i = 0; i < 7; i++) {
					TableCell cell = Calendar.IsAccessibilityCompliantRender() ? RenderUtils.CreateTableHeaderCell("col") : RenderUtils.CreateTableCell();
					DayNamesRow.Cells.Add(cell);
				}
				Rows.Add(DayNamesRow);
			}
			foreach (CalendarWeek week in Weeks)
				Rows.Add(week);
		}
		protected override void PrepareControlHierarchy() {			
			CellPadding = 0;
			CellSpacing = 0;
			BorderCollapse = BorderCollapse.Separate; 
			Width = Unit.Percentage(100);
			if(DayNamesRow != null) {
				DayNamesRow.HorizontalAlign = HorizontalAlign.Center;
				CalendarElementStyle dayHeaderStyle = Calendar.GetDayHeaderStyle();
				DayOfWeek day = Calendar.GetFirstDayOfWeek();
				for(int i = 0; i < DayNamesRow.Cells.Count; i++) {
					if(Calendar.ShowWeekNumbers && i == 0)
						continue;
					DayNamesRow.Cells[i].Text = CommonUtils.GetDayName(day, Calendar.DayNameFormat);
					dayHeaderStyle.AssignToControl(DayNamesRow.Cells[i], true);
					if((int)(++day) > 6)
						day = (DayOfWeek)0;
				}
			}
		}
		protected internal void Fill() {
			Weeks.Clear();
			DateTime date = ViewInfo.GetStartDate();
			int offset = (int)Calendar.GetFirstDayOfWeek() - 1;
			if(offset < 0)
				offset += 7;
			int weekNumber = Calendar.GetISO8601WeekOfYear(date.AddDays(offset));
			bool hiddenDay = false;
			for(int i = 0; i < CalendarView.WeekCount; i++) {
				CalendarWeek week = new CalendarWeek(ViewInfo, weekNumber);
				for(int j = 0; j < 7; j++) {
					DayData dayData = new DayData(ViewInfo, date, hiddenDay);
					if(dayData.IsDisplayed) {
						Calendar.RaiseCustomDisabledDate(dayData);
						Calendar.RaiseDayCellInitialize(dayData);
					}
					CalendarDayCell day = new CalendarDayCell(dayData);
					if(date < DateTime.MaxValue.Date)
						date = date.AddDays(1);
					else
						hiddenDay = true;
					week.Days.Add(day);
				}
				Weeks.Add(week);
				if(++weekNumber > 52) {
					try {
						weekNumber = Calendar.GetISO8601WeekOfYear(date.AddDays(offset));
					} catch(ArgumentOutOfRangeException) {
						weekNumber = 53; 
					}
				}
			}
		}
	}
	public class CalendarView : CalendarViewTable {
		public const int WeekCount = 6;
		private TableCell headerCell = null;
		private TableCell monthCell = null;
		public CalendarView(CalendarViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected TableCell HeaderCell {
			get { return headerCell; }
		}
		protected TableCell MonthCell {
			get { return monthCell; }
		}
		protected override void CreateControlHierarchy() {
			if (Calendar.ShowHeader)
				CreateHeader();
			CreateMonthGrid();
		}
		protected override void PrepareControlHierarchy() {
			Width = Unit.Percentage(100);
			RenderUtils.CollapseAndRemovePadding(this);
			if (HeaderCell != null)
				PrepareHeader();
			PrepareMonthGrid();
		}
		protected void CreateHeader() {
			TableRow row = RenderUtils.CreateTableRow();
			this.headerCell = RenderUtils.CreateTableCell();
			HeaderCell.Controls.Add(new CalendarHeader(ViewInfo));
			row.Cells.Add(HeaderCell);
			Rows.Add(row);
		}
		protected void CreateMonthGrid() {
			TableRow row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			this.monthCell = RenderUtils.CreateTableCell();			
			row.Cells.Add(MonthCell);
			MonthCell.ID = "mc";
			if(Calendar.IsMultiView())
				MonthCell.ID += ViewInfo.GetIDPostfix();
			MonthCell.Controls.Add(new CalendarMonth(ViewInfo));
		}
		protected void PrepareHeader() {
			if(HeaderCell != null) {
				CalendarHeaderFooterStyle style = Calendar.GetHeaderStyle();
				style.AssignToControl(HeaderCell, true);
				if(ViewInfo.Row == 0 || !Calendar.ShowHeader)
					RenderUtils.SetStyleStringAttribute(HeaderCell, "border-top", "0");
			}
		}
		protected void PrepareMonthGrid() {
			RenderUtils.SetPaddings(MonthCell, Calendar.MonthGridPaddings);
			RenderUtils.SetPreventSelectionAttribute(MonthCell);			
			RenderUtils.AppendDefaultDXClassName(MonthCell, Calendar.GetMonthGridClassName());
		}
	}
	public class CalendarButton : ButtonCell {
		public CalendarButton()
			: base() {
		}
		public CalendarButton(string text)
			: base(text) {
		}
	}
	public class CalendarFooter : InternalTable {
		protected internal static readonly string TodayButtonSuffix = "BT";
		protected internal static readonly string ClearButtonSuffix = "BC";
		List<TableCell> spacers;
		public CalendarFooter(ASPxCalendar calendar) {
			Calendar = calendar;
		}
		protected ASPxCalendar Calendar { get; private set; }
		protected CalendarButton TodayButton { get; private set; }
		protected CalendarButton ClearButton { get; private set; }
		protected TableRow FooterRow { get; private set; }
		protected List<TableCell> Spacers {
			get {
				if(spacers == null)
					spacers = new List<TableCell>();
				return spacers;
			}
		}
		protected override void CreateControlHierarchy() {
			FooterRow = RenderUtils.CreateTableRow();
			Rows.Add(FooterRow);
			if(Calendar.ShowTodayButton) 
				CreateTodayButton();
			if(Calendar.ShowTodayButton && Calendar.ShowClearButton) 
				CreateSpacer();
			if(Calendar.ShowClearButton) 
				CreateClearButton();
		}
		protected void CreateTodayButton() {
			TodayButton = new CalendarButton(Calendar.TodayButtonText);
			TodayButton.ID = TodayButtonSuffix;
			FooterRow.Controls.Add(TodayButton);
		}
		protected void CreateClearButton() {
			ClearButton = new CalendarButton(Calendar.ClearButtonText);
			ClearButton.ID = ClearButtonSuffix;
			FooterRow.Controls.Add(ClearButton);
		}
		protected void CreateSpacer() {
			TableCell cell = RenderUtils.CreateIndentCell();
			Spacers.Add(cell);
			FooterRow.Cells.Add(cell);
		}
		protected override void PrepareControlHierarchy() {
			PrepareTable();
			PrepareSpacers();
			if(TodayButton != null) 
				PrepareTodayButton();
			if(ClearButton != null) 
				PrepareClearButton();
			if(RenderUtils.IsHtml5Mode(this) && Calendar.IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(this, "role", "presentation");
		}
		protected virtual void PrepareTable() {
			RenderUtils.CollapseAndRemovePadding(this);
			if(RenderUtils.Browser.IsFirefox) {
				this.BorderWidth = Unit.Pixel(0);
				this.BorderStyle = BorderStyle.Solid;
			}
		}
		protected void PrepareTodayButton() {
			PrepareButton(TodayButton, Calendar.GetTodayButtonOnClick());
		}
		protected virtual void PrepareClearButton() {
			PrepareButton(ClearButton, Calendar.GetClearButtonOnClick());
		}
		protected void PrepareButton(CalendarButton button, string onClick) {
			EditButtonStyle buttonStyle = Calendar.GetButtonStyle();
			button.ButtonStyle.Assign(buttonStyle);
			button.Width = buttonStyle.Width;
			if(IsEnabled)
				RenderUtils.SetStringAttribute(button, "onclick", onClick);
			button.EncodeHtml = Calendar.EncodeHtml;
		}
		protected void PrepareSpacers() {
			Unit spacing = Calendar.GetFooterStyle().Spacing;
			foreach(TableCell spacer in Spacers) {
				RenderUtils.PrepareIndentCell(spacer, spacing, false);
				RenderUtils.AppendDefaultDXClassName(spacer, EditorStyles.CalendarFooterSpacerClassName);
			}
		}
	}
	public class TimeSectionCalendarFooter : CalendarFooter {
		protected internal static readonly string OkButtonSuffix = "BO";
		protected internal static readonly string CancelButtonSuffix = "BCN";
		CalendarButton okButton, cancelButton;
		TableCell strutCell;
		public TimeSectionCalendarFooter(ASPxCalendar calendar)
			: base(calendar) {
		}
		protected ITimeSectionOwner TimeSectionOwner { get { return Calendar.TimeSectionOwner; } }
		protected CalendarButton OkButton { get { return okButton; } }
		protected CalendarButton CancelButton { get { return cancelButton; } }
		protected TableCell StrutCell { get { return strutCell; } }
		protected override void CreateControlHierarchy()  {
			base.CreateControlHierarchy();
			CreateStrutCell();
			if(TimeSectionOwner.ShowOkButton)
				CreateOkButton();
			if(TimeSectionOwner.ShowOkButton && TimeSectionOwner.ShowCancelButton)
				CreateSpacer();
			if(TimeSectionOwner.ShowCancelButton)
				CreateCancelButton();
		}
		protected void CreateOkButton() {
			this.okButton = new CalendarButton(Calendar.FastNavProperties.OkButtonText);
			OkButton.ID = OkButtonSuffix;
			FooterRow.Controls.Add(OkButton);
		}
		protected void CreateCancelButton() {
			this.cancelButton = new CalendarButton(Calendar.FastNavProperties.CancelButtonText);
			CancelButton.ID = CancelButtonSuffix;
			FooterRow.Controls.Add(CancelButton);
		}
		protected void CreateStrutCell() {
			this.strutCell = RenderUtils.CreateTableCell();
			FooterRow.Cells.Add(StrutCell);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Width = Unit.Percentage(100);
			StrutCell.Width = Unit.Percentage(100);
			if(OkButton != null)
				PrepareOkButton();
			if(CancelButton != null)
				PrepareCancelButton();
		}
		protected void PrepareOkButton() {
			PrepareButton(OkButton, TimeSectionOwner.OkButtonClickScript);
			OkButton.Text = TimeSectionOwner.OkButtonText;
		}			   
		protected void PrepareCancelButton() {
			PrepareButton(CancelButton, TimeSectionOwner.CancelButtonClickScript);
			CancelButton.Text = TimeSectionOwner.CancelButtonText;
		}
		protected override void PrepareClearButton() {
			PrepareButton(ClearButton, TimeSectionOwner.ClearButtonClickScript);
		}
	}
	public class CalendarControl : ASPxInternalWebControl {
		private ASPxCalendar calendar = null;
		private Table mainTable = null;
		private TableCell footerCell = null;
		private TableCell[] viewCells;
		private DropDownPopupControl fastNavPopup = null;
		private KeyboardSupportInputHelper kbInput = null;
		TimeSectionLayout timeSectionLayout;
		static DateTime MaxDate;
		static CalendarControl() {
			MaxDate = DateTime.MaxValue.Date.AddMonths(-1);
		}
		public CalendarControl(ASPxCalendar calendar) {
			this.calendar = calendar;
		}
		protected ASPxCalendar Calendar {
			get { return calendar; }
		}
		protected bool IsRightToLeft {
			get { return (Calendar as ISkinOwner).IsRightToLeft(); }
		}
		protected Table MainTable {
			get { return mainTable; }
		}
		protected TableCell FooterCell {
			get { return footerCell; }
		}
		protected internal TableCell[] ViewCells { get { return viewCells; } }
		internal DropDownPopupControl FastNavPopup {
			get { return fastNavPopup; }
		}
		protected KeyboardSupportInputHelper KbInput {
			get { return this.kbInput; }
		}
		protected bool ShowTimeSection { get { return Calendar.TimeSectionOwner != null && Calendar.TimeSectionOwner.ShowTimeSection; } }
		protected bool HasVisibleButtons {
			get {
				if(Calendar.Properties.HasVisibleButtons())
					return true;
				if(ShowTimeSection)
					return Calendar.TimeSectionOwner.ShowOkButton || Calendar.TimeSectionOwner.ShowCancelButton;
				return false;
			}
		}
		protected TimeSectionLayout TimeSectionLayout { get { return timeSectionLayout; } }
		protected override void ClearControlFields() {
			this.mainTable = null;
			this.kbInput = null;
			this.footerCell = null;
			this.viewCells = null;
			this.fastNavPopup = null;
		}
		protected override void CreateControlHierarchy() {
			this.mainTable = RenderUtils.CreateTable();
			Table viewsContainer = MainTable;
			if(ShowTimeSection) {
				CreateTimeSectionLayout();
				viewsContainer = RenderUtils.CreateTable();
				TimeSectionLayout.CalendarCell.Controls.Add(viewsContainer);
			}
			CreateViews(viewsContainer);
			if(!Calendar.ReadOnly && HasVisibleButtons)
				CreateFooter();
			Controls.Add(MainTable);
			if (Calendar.IsFastNavEnabled())
				CreateFastNav();
			if(!Calendar.IsDateEditCalendar) {
				if(RenderUtils.IsHtml5Mode(this) && Calendar.IsAccessibilityCompliantRender())
					CreateAccessibilityHelper();
				else
					CreateKBSupportInput();
			}
		}
		void CreateAccessibilityHelper() {
			WebControl accessibilityHelper = new WebControl(HtmlTextWriterTag.Div);
			accessibilityHelper.ID = AccessibilityUtils.AssistantID;
			RenderUtils.SetStringAttribute(accessibilityHelper, "role", Browser.IsIE ? "application" : "row");
			RenderUtils.SetStringAttribute(accessibilityHelper, "tabindex", "0");
			RenderUtils.SetStringAttribute(accessibilityHelper, "aria-label", AccessibilityUtils.CalendarDescriptionText);
			accessibilityHelper.CssClass = AccessibilityUtils.InvisibleFocusableElementCssClassName;
			for(int i = 0; i < 2; i++) {
				WebControl child = new WebControl(HtmlTextWriterTag.Span);
				child.ID = accessibilityHelper.ID + i.ToString();
				RenderUtils.SetStringAttribute(child, "role", "gridcell");
				RenderUtils.SetStringAttribute(child, "aria-selected", "true");
				accessibilityHelper.Controls.Add(child);
			}
			Controls.Add(accessibilityHelper);
		}
		protected void CreateTimeSectionLayout() {
			this.timeSectionLayout = new TimeSectionLayout();
			TableRow row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(row);
			CreateCalendarCell(row);
			CreateTimeSectionCell(row);
		}
		protected void CreateTimeSectionCell(TableRow row) {
			TableCell timeSectionCell = RenderUtils.CreateTableCell();
			timeSectionCell.VerticalAlign = VerticalAlign.Top;
			row.Cells.Add(timeSectionCell);
			timeSectionCell.Controls.Add(CreateTimeSectionTable());
		}
		protected Table CreateTimeSectionTable(){
			Table table = RenderUtils.CreateTable();
			table.Width = Unit.Percentage(100);
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			if(Calendar.ShowHeader && Calendar.Rows < 2) {
				CreateTimeSectionHeader(row);
				row = RenderUtils.CreateTableRow();
				table.Rows.Add(row);
			}
			CreateClockCell(row);
			row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			CreateTimeEditCell(row);
			return table;
		}
		protected void CreateTimeSectionHeader(TableRow row) {
			TimeSectionLayout.TimeSectionHeaderCell = RenderUtils.CreateTableCell();
			row.Cells.Add(TimeSectionLayout.TimeSectionHeaderCell);
			CalendarViewInfo info = new CalendarViewInfo(Calendar, Calendar.EffectiveVisibleDate, 0, Calendar.Columns);
			TimeSectionLayout.TimeSectionHeaderCell.Controls.Add(new TimeSectionCalendarHeader(info));
		}
		protected void CreateCalendarCell(TableRow row) {
			TimeSectionLayout.CalendarCell = RenderUtils.CreateTableCell();
			row.Cells.Add(TimeSectionLayout.CalendarCell);
			TimeSectionLayout.CalendarCell.VerticalAlign = VerticalAlign.Top; 
		}
		protected void CreateClockCell(TableRow row) {
			TimeSectionLayout.ClockCell = RenderUtils.CreateTableCell();
			row.Cells.Add(TimeSectionLayout.ClockCell);
			CreateClock();
		}
		protected void CreateClock() {
			InternalClock clock = new InternalClock();
			clock.ID = TimeSectionLayout.ClockID;
			TimeSectionLayout.ClockCell.Controls.Add(clock);
			clock.ShowHourHand = Calendar.TimeSectionOwner.ShowHourHand;
			clock.ShowMinuteHand = Calendar.TimeSectionOwner.ShowMinuteHand;
			clock.ShowSecondHand = Calendar.TimeSectionOwner.ShowSecondHand;
			clock.ClockFaceImage.Assign(Calendar.TimeSectionOwner.ClockFaceImage);
			clock.HourHandImage.Assign(Calendar.TimeSectionOwner.HourHandImage);
			clock.MinuteHandImage.Assign(Calendar.TimeSectionOwner.MinuteHandImage);
			clock.SecondHandImage.Assign(Calendar.TimeSectionOwner.SecondHandImage);
		}
		protected void CreateTimeEditCell(TableRow row) {
			TimeSectionLayout.TimeEditCell = RenderUtils.CreateTableCell();
			row.Cells.Add(TimeSectionLayout.TimeEditCell);
			TimeSectionLayout.TimeEdit = CreateTimeEdit();
			TimeSectionLayout.TimeEdit.ID = TimeSectionLayout.TimeEditID;
			TimeSectionLayout.TimeEdit.ReadOnly = Calendar.ReadOnly;
			TimeSectionLayout.TimeEditCell.Controls.Add(TimeSectionLayout.TimeEdit);
			TimeSectionLayout.TimeEdit.Properties.ParentSkinOwner = Calendar;
			TimeSectionLayout.TimeEdit.Properties.ParentStyles = Calendar.RenderStyles;
			TimeSectionLayout.TimeEdit.Properties.ParentImages = Calendar.RenderImages;
			TimeSectionLayout.TimeEdit.Properties.Assign(Calendar.TimeSectionOwner.TimeEditProperties);
			TimeSectionLayout.TimeEdit.ClientSideEvents.KeyDown = Calendar.TimeSectionOwner.TimeEditKeyDownScript;
			TimeSectionLayout.TimeEdit.ClientSideEvents.LostFocus = Calendar.TimeSectionOwner.TimeEditLostFocusScript;
			if(TimeSectionLayout.TimeEdit.Width.IsEmpty)
				TimeSectionLayout.TimeEdit.Width = Unit.Percentage(100);
		}
		protected virtual ASPxTimeEdit CreateTimeEdit() {
			return new ASPxTimeEdit();
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(Calendar, MainTable);
			Calendar.RemoveImportedStyleAttrsFromMainElement(MainTable);
			RenderUtils.SetVisibility(MainTable, Calendar.IsClientVisible(), true);
			Calendar.GetControlStyle().AssignToControl(MainTable);
			MainTable.Height = Unit.Empty;
			if (FooterCell != null) {
				Calendar.GetFooterStyle().AssignToControl(FooterCell, true);
				int colSpan = Calendar.Columns;
				if(TimeSectionLayout != null)
					colSpan = 2;
				if(colSpan > 1)
					FooterCell.ColumnSpan = colSpan;
			}
			if(TimeSectionLayout != null) {
				ITimeSectionOwner owner = Calendar.TimeSectionOwner;
				if(TimeSectionLayout.TimeEditCell != null) {
					owner.TimeEditCellStyle.AssignToControl(TimeSectionLayout.TimeEditCell, true);
					if(Browser.IsIE)
						RenderUtils.SetStyleStringAttribute(TimeSectionLayout.TimeEditCell, "border-collapse", "separate");
				}
				if(TimeSectionLayout.ClockCell != null)
					owner.ClockCellStyle.AssignToControl(TimeSectionLayout.ClockCell, true);
				TableCell headerCell = TimeSectionLayout.TimeSectionHeaderCell;
				if(headerCell != null) {
					Calendar.GetHeaderStyle().AssignToControl(headerCell, true);
					RenderUtils.SetStyleStringAttribute(headerCell, "border-top", "0");
					headerCell.CssClass = RenderUtils.CombineCssClasses(headerCell.CssClass, owner.TimeSectionHeaderClassName);
				}
			}
			if (FastNavPopup != null)
				PrepareFastNav();
			foreach (TableCell cell in ViewCells)
				cell.VerticalAlign = VerticalAlign.Top;
		}
		protected void CreateKBSupportInput() {
			if(Calendar.IsEnabled()) {
				this.kbInput = new KeyboardSupportInputHelper();
				MainTable.Rows[0].Cells[0].Controls.Add(kbInput);
			}
		}
		protected void CreateViews(Table container) {
			this.viewCells = new InternalTableCell[Calendar.Columns * Calendar.Rows];
			DateTime viewDate = Calendar.EffectiveVisibleDate;
			int index = 0;
			for (int viewRowNumber = 0; viewRowNumber < Calendar.Rows; viewRowNumber++) {
				TableRow row = RenderUtils.CreateTableRow();
				for (int viewColumnNumber = 0; viewColumnNumber < Calendar.Columns; viewColumnNumber++) {
					CalendarViewInfo info = new CalendarViewInfo(Calendar, viewDate, viewRowNumber, viewColumnNumber);
					TableCell cell = RenderUtils.CreateTableCell();
					cell.Controls.Add(new CalendarView(info));
					row.Cells.Add(cell);
					ViewCells[index] = cell;
					if(viewDate < MaxDate)
						viewDate = viewDate.AddMonths(1);
					++index;
				}
				container.Rows.Add(row);
			}
		}
		protected void CreateFooter() {
			TableRow row = RenderUtils.CreateTableRow();
			this.footerCell = RenderUtils.CreateTableCell();
			CalendarFooter footer = ShowTimeSection ? new TimeSectionCalendarFooter(Calendar) : new CalendarFooter(Calendar);
			FooterCell.Controls.Add(footer);
			FooterCell.HorizontalAlign = HorizontalAlign.Center;
			row.Cells.Add(FooterCell);
			MainTable.Rows.Add(row);
		}
		protected void CreateFastNav() {
			this.fastNavPopup = new DropDownPopupControl(false);
			FastNavPopup.RenderIFrameForPopupElements = Calendar.RenderIFrameForPopupElements;
			FastNavPopup.ShowShadow = Calendar.ShowShadow;
			FastNavPopup.ParentSkinOwner = Calendar;
			FastNavPopup.Controls.Add(new CalendarFastNav(Calendar));
			Controls.Add(FastNavPopup);
			FastNavPopup.ID = CalendarFastNav.PopupSuffix;
		}
		protected void PrepareFastNav() {			
			FastNavPopup.Width = 0;
			FastNavPopup.ShowHeader = false;
			FastNavPopup.ShowFooter = false;
			FastNavPopup.PopupAnimationType = Calendar.FastNavProperties.EnablePopupAnimation ? AnimationType.Fade : AnimationType.None;
			FastNavPopup.EnableViewState = false;
		}
	}
	public class CalendarFastNav : ASPxInternalWebControl {
		protected internal const string OkButtonSuffix = "BO";
		protected internal const string CancelButtonSuffix = "BC";
		protected internal const string PopupSuffix = "FNP";
		public CalendarFastNav(ASPxCalendar calendar) {
			Calendar = calendar;
		}
		protected ASPxCalendar Calendar { get; private set; }
		protected bool IsRightToLeft {
			get { return (Calendar as ISkinOwner).IsRightToLeft(); }
		}
		protected WebControl MainArea { get; private set; }
		protected WebControl MonthArea { get; private set; }
		protected WebControl YearArea { get; private set; }
		protected Table MonthTable { get; private set; }
		protected Table YearTable { get; private set; }
		protected TableCell[] YearCells { get; private set; }
		protected TableCell PrevCell { get; private set; }
		protected TableCell NextCell { get; private set; }
		protected Image PrevImage { get; private set; }
		protected Image NextImage { get; private set; }
		protected WebControl ButtonArea { get; private set; }
		protected Table ButtonTable { get; private set; }
		protected CalendarButton OkButton { get; private set; }
		protected CalendarButton CancelButton { get; private set; }
		protected TableCell SpacerCell { get; private set; }
		protected override void CreateControlHierarchy() {
			MainArea = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(MainArea);
			if (Calendar.EnableMonthNavigation)
				CreateMonthTable();
			if (Calendar.EnableYearNavigation)
				CreateYearTable();
			ButtonArea = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(ButtonArea);
			ButtonTable = RenderUtils.CreateTable();
			ButtonArea.Controls.Add(ButtonTable);
			TableRow row = RenderUtils.CreateTableRow();
			ButtonTable.Rows.Add(row);
			CreateOkButton(row);
			CreateSpacer(row);
			CreateCancelButton(row);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			MainArea = null;
			MonthArea = null;
			YearArea = null;
			MonthTable = null;
			YearTable = null;
			YearCells = null;
			PrevCell = null;
			NextCell = null;
			PrevImage = null;
			NextImage = null;
			ButtonArea = null;
			ButtonTable = null;
			OkButton = null;
			CancelButton = null;
			SpacerCell = null;
		}
		protected override void PrepareControlHierarchy() {
			Calendar.GetFastNavStyle().AssignToControl(MainArea, true);
			if (MonthArea != null)
				Calendar.GetFastNavMonthAreaStyle().AssignToControl(MonthArea, true);
			if (MonthTable != null)
				PrepareMonthTable();
			if (YearArea != null)
				Calendar.GetFastNavYearAreaStyle().AssignToControl(YearArea, true);
			if (YearTable != null)
				PrepareYearTable();
			CalendarHeaderFooterStyle buttonAreaStyle = Calendar.GetFastNavFooterStyle();
			buttonAreaStyle.AssignToControl(ButtonArea, true);
			RenderUtils.SetStringAttribute(ButtonArea, "align", "center");
			RenderUtils.PrepareSpaceControl(SpacerCell, buttonAreaStyle.Spacing, false);
			RenderUtils.AppendDefaultDXClassName(SpacerCell, EditorStyles.CalendarFastNavFooterSpacerClassName);
			EditButtonStyle buttonStyle = Calendar.GetFastNavButtonStyle();
			OkButton.ButtonStyle.Assign(buttonStyle);			
			OkButton.OnClickScript = Calendar.GetFastNavButtonOnClick("ok");
			OkButton.EncodeHtml = Calendar.EncodeHtml;
			CancelButton.ButtonStyle.Assign(buttonStyle);
			CancelButton.OnClickScript = Calendar.GetFastNavButtonOnClick("cancel");
			CancelButton.EncodeHtml = Calendar.EncodeHtml;
		}
		protected void CreateOkButton(TableRow parent) {
			OkButton = new CalendarButton(Calendar.FastNavProperties.OkButtonText);
			OkButton.ID = OkButtonSuffix;
			parent.Controls.Add(OkButton);
		}
		protected void CreateCancelButton(TableRow parent) {
			CancelButton = new CalendarButton(Calendar.FastNavProperties.CancelButtonText);
			CancelButton.ID = CancelButtonSuffix;
			parent.Controls.Add(CancelButton);
		}
		protected void CreateSpacer(TableRow parent) {
			SpacerCell = RenderUtils.CreateTableCell();
			parent.Cells.Add(SpacerCell);
		}
		protected void CreateMonthTable() {
			MonthArea = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			MonthTable = RenderUtils.CreateTable(true);			
			int rowCount = 3;
			int columnCount = 12 / rowCount;
			for (int rowNumber = 0; rowNumber < rowCount; rowNumber++) {
				TableRow row = RenderUtils.CreateTableRow();
				for (int colNumber = 0; colNumber < columnCount; colNumber++) {
					TableCell cell = RenderUtils.CreateTableCell();
					row.Cells.Add(cell);
				}
				MonthTable.Rows.Add(row);
			}
			MonthArea.Controls.Add(MonthTable);
			MainArea.Controls.Add(MonthArea);
			MonthTable.ID = "m";
		}
		protected void CreateYearTable() {
			YearArea = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			YearTable = RenderUtils.CreateTable(true);
			YearCells = new InternalTableCell[10];
			PrevCell = RenderUtils.CreateTableCell();
			NextCell = RenderUtils.CreateTableCell();
			PrevImage = RenderUtils.CreateImage();
			NextImage = RenderUtils.CreateImage();
			PrevCell.Controls.Add(PrevImage);
			NextCell.Controls.Add(NextImage);
			int index = 0;
			for (int rowNumber = 0; rowNumber < 2; rowNumber++) {
				TableRow row = RenderUtils.CreateTableRow();
				if (rowNumber == 0)
					row.Cells.Add(PrevCell);
				for (int colNumber = 0; colNumber < 5; colNumber++) {
					TableCell cell = RenderUtils.CreateTableCell();
					YearCells[index++] = cell;
					row.Cells.Add(cell);
				}
				if (rowNumber == 0)
					row.Cells.Add(NextCell);
				YearTable.Rows.Add(row);
			}
			YearArea.Controls.Add(YearTable);
			MainArea.Controls.Add(YearArea);
			YearTable.ID = "y";
		}
		protected void PrepareMonthTable() {			
			MonthTable.Width = Unit.Percentage(100);
			CalendarFastNavItemStyle style = Calendar.GetFastNavMonthStyle();
			int month = 0;
			foreach (TableRow row in MonthTable.Rows) {
				foreach (TableCell cell in row.Cells) {
					style.AssignToControl(cell, true);
					cell.Text = CommonUtils.Capitalize(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(1 + month));
					month++;
				}
			}
		}
		protected void PrepareYearTable() {
			CalendarFastNavStyle fastNavStyle = Calendar.GetFastNavStyle();
			if(MonthTable != null && !fastNavStyle.MonthYearSpacing.IsEmpty)
				RenderUtils.SetStyleStringAttribute(YearTable, "margin-top", fastNavStyle.MonthYearSpacing.ToString());
			YearTable.Width = Unit.Percentage(100);
			CalendarFastNavItemStyle style = Calendar.GetFastNavYearStyle();
			foreach (TableCell cell in YearCells)
				style.AssignToControl(cell, true);
			Unit spacing = fastNavStyle.ImageSpacing;
			if (!spacing.IsEmpty) {
				RenderUtils.SetHorizontalMargins(IsRightToLeft ? NextImage : PrevImage, Unit.Empty, spacing);
				RenderUtils.SetHorizontalMargins(IsRightToLeft ? PrevImage : NextImage, spacing, Unit.Empty);
			}
			PrevCell.RowSpan = NextCell.RowSpan = 2;
			Calendar.GetFastNavPrevYearImage().AssignToControl(PrevImage, DesignMode);
			Calendar.GetFastNavNextYearImage().AssignToControl(NextImage, DesignMode);
			RenderUtils.SetCursor(PrevCell, RenderUtils.GetPointerCursor());
			RenderUtils.SetCursor(NextCell, RenderUtils.GetPointerCursor());
			string prevHandler = Calendar.GetFastNavYearShuffleOnClick(-10);
			string nextHandler = Calendar.GetFastNavYearShuffleOnClick(10);
			RenderUtils.SetStringAttribute(PrevCell, "onclick", prevHandler);
			RenderUtils.SetStringAttribute(NextCell, "onclick", nextHandler);
			if (Browser.IsIE) {
				RenderUtils.SetStringAttribute(PrevCell, "ondblclick", prevHandler);
				RenderUtils.SetStringAttribute(NextCell, "ondblclick", nextHandler);
			}
		}
	}
	public class TimeSectionLayout {
		public const string TimeEditID = "TE";
		public const string ClockID = "CL";
		public TableCell CalendarCell { get; set; }
		public TableCell TimeSectionHeaderCell { get; set; }
		public TableCell ClockCell { get; set; }
		public TableCell TimeEditCell { get; set; }
		public ASPxTimeEdit TimeEdit {get; set; }
	}
}

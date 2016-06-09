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
using System.Drawing;
using System.Text;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxScheduler {
	[
	DXWebToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(ToolboxBitmapAccess),
	ToolboxBitmapAccess.BitmapPath + "ASPxDateNavigator.bmp")
]
	public class ASPxDateNavigator : ASPxSchedulerRelatedControl, IASPxDateNavigatorControllerOwner {
		#region Fields
		protected internal const string ScriptResourceName = "Scripts.DateNavigator.js";
		readonly DateNavigatorProperties properties;
		ASPxDateNavigatorCalendar calendar;
		DateNavigatorController controller;
		List<DateTime> clientSelection;
		DateTime visibleDate;
		#endregion
		public ASPxDateNavigator() {
			ClientIDHelper.SetClientIDModeToAutoID(this);
			this.properties = new DateNavigatorProperties(this);
		}
		#region Properties
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxDateNavigatorProperties"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DateNavigatorProperties Properties { get { return properties; } }
		protected internal override ASPxSchedulerChangeAction RenderActions {
			get {
				ASPxSchedulerChangeAction result = ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged;
				if (Properties.BoldAppointmentDates)
					result |= ASPxSchedulerChangeAction.RenderAppointments;
				return result;
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxDateNavigatorImageFolder"),
#endif
		Category("Images"), DefaultValue(""), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxDateNavigatorSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("ASPxDateNavigatorSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		#endregion
		protected internal override void CreateControlContentHierarchy() {
			LoadClientStateFromCalendar(this.calendar);
			this.calendar = new ASPxDateNavigatorCalendar();
			ApplyClientStateToCalendar(this.calendar);
			this.calendar.ParentSkinOwner = this;
			this.calendar.ID = "cal";
			calendar.Properties.Assign(properties);
			MainCell.Controls.Add(calendar);
		}
		protected internal override void PrepareControlContentHierarchy() {
			if (SchedulerControl != null) {
				GetControlStyle().AssignToControl(MainTable);
				this.calendar.ParentStyles = SchedulerControl.Styles.FormEditors;
				this.calendar.ParentImages = SchedulerControl.Images.FormEditors;
				this.calendar.ClientEnabled = true;
				this.calendar.FirstDayOfWeek = (System.Web.UI.WebControls.FirstDayOfWeek)SchedulerControl.FirstDayOfWeek;
				this.calendar.MinDate = SchedulerControl.LimitInterval.Start;
				this.calendar.MaxDate = SchedulerControl.LimitInterval.End.TimeOfDay == TimeSpan.Zero ? SchedulerControl.LimitInterval.End.AddDays(-1) : SchedulerControl.LimitInterval.End.Date.AddDays(1);
				this.calendar.Width = Width;
				if (this.controller == null)
					this.controller = new ASPxDateNavigatorController(this);
				SubscribeControllerEvents(this.controller);
				this.controller.BoldAppointmentDates = this.Properties.BoldAppointmentDates;
				this.controller.ConnectToControl(SchedulerControl.InnerControl);
				this.calendar.DayCellInitialize += OnCalendarDayCellInitialize;
				this.calendar.DayCellPrepared += OnCalendarDayCellPrepared;
				this.calendar.TodayDate = SchedulerControl.InnerControl.TimeZoneHelper.ToClientTime(DateTime.Now).Date;
			}
			this.calendar.ClientSideEvents.VisibleMonthChanged = String.Format("function(s,e) {{ ASPx.VisibleMonthChanged('{0}', e.offsetInternal); }}", ClientID);
			this.calendar.ClientSideEvents.SelectionChanged = String.Format("function(s,e) {{ ASPx.DateNavigatorSelectionChanged('{0}'); }}", ClientID);
		}
		protected virtual void SubscribeControllerEvents(DateNavigatorController controller) {
			controller.DateNavigatorQueryActiveViewType += OnControllerSchedulerViewAutoAdjusting;
		}
		void OnControllerSchedulerViewAutoAdjusting(object sender, DateNavigatorQueryActiveViewTypeEventArgs e) {
			if (SchedulerControl == null)
				return;
			e.NewViewType = SchedulerControl.RaiseDateNavigatorQueryActiveViewType(e.OldViewType, e.NewViewType, e.SelectedDays);
		}
		void OnCalendarDayCellInitialize(object sender, CalendarDayCellInitializeEventArgs e) {
			e.IsWeekend = !controller.IsWorkDay(e.Date.Date);
		}
		void OnCalendarDayCellPrepared(object sender, CalendarDayCellPreparedEventArgs e) {
			bool value;
			if (!controller.AppointmentDatesMap.TryGetValue(e.Date.Date, out value))
				value = false;
			e.Cell.Font.Bold = value;
		}
		protected override StylesBase CreateStyles() {
			return new DateNavigatorStyles(this);
		}
		protected override string GetSkinControlName() {
			return "Editors";
		}
		#region IDateNavigatorControllerOwner Members
		DateTime IDateNavigatorControllerOwner.GetFirstDayOfMonth(DateTime date) {
			return new DateTime(date.Year, date.Month, 1);
		}
		DayIntervalCollection IDateNavigatorControllerOwner.GetSelection() {
			DayIntervalCollection result = new DayIntervalCollection();
			CalendarSelection selection = calendar.SelectedDates;
			foreach (DateTime date in selection)
				result.Add(new TimeInterval(date.Date, TimeSpan.Zero));
			return result;
		}
		void IDateNavigatorControllerOwner.SetSelection(DayIntervalCollection days) {
			calendar.SelectedDates.Clear();
			int count = days.Count;
			for (int i = 0; i < count; i++)
				calendar.SelectedDates.Add(days[i].Start);
		}
		DateTime IDateNavigatorControllerOwner.SelectionStart {
			get {
				long result = DateTime.MaxValue.Ticks;
				foreach (DateTime date in calendar.SelectedDates)
					result = Math.Min(date.Ticks, result);
				return new DateTime(result);
			}
		}
		DateTime IDateNavigatorControllerOwner.SelectionEnd {
			get {
				long result = DateTime.MinValue.Ticks;
				foreach (DateTime date in calendar.SelectedDates)
					result = Math.Max(date.Ticks, result);
				return new DateTime(result);
			}
		}
		DateTime IDateNavigatorControllerOwner.StartDate {
			get { return calendar.GetFirstVisibleDate(); }
			set {
				calendar.VisibleDate = value;
			}
		}
		DateTime IDateNavigatorControllerOwner.EndDate {
			get { return calendar.GetLastVisibleDate(); }
		}
		#endregion
		#region Client scripts support
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxDateNavigator.ScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder sb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(sb, localVarName, clientName);
			if (SchedulerControl != null)
				sb.AppendFormat("{0}.schedulerControlId='{1}';", localVarName, SchedulerControl.ClientID);
			sb.AppendFormat("{0}.calendarId='{1}';", localVarName, calendar.ClientID);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientDateNavigator";
		}
		#endregion
		#region ViewState
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { properties });
		}
		#endregion        
		void LoadClientStateFromCalendar(ASPxDateNavigatorCalendar calendar) {
			if (calendar == null)
				return;
			this.clientSelection = new List<DateTime>();
			this.clientSelection.AddRange(calendar.SelectedDates);
			this.visibleDate = calendar.VisibleDate;
		}
		void ApplyClientStateToCalendar(ASPxDateNavigatorCalendar calendar) {
			if (calendar == null || this.clientSelection == null)
				return;
			foreach (var date in this.clientSelection)
				calendar.SelectedDates.Add(date);
			calendar.VisibleDate = this.visibleDate;
			this.clientSelection = null;
		}
		TimeInterval IASPxDateNavigatorControllerOwner.GetCalendarInterval() {
			return new TimeInterval(this.calendar.GetFirstDate(), this.calendar.GetLastDate());
		}
		TimeInterval IASPxDateNavigatorControllerOwner.GetCalendarVisibleInterval() {
			return new TimeInterval(this.calendar.GetFirstVisibleDate(), this.calendar.GetLastVisibleDate().AddDays(1));
		}
	}
	public class DateNavigatorProperties : CalendarProperties {
		public DateNavigatorProperties()
			: base() {
		}
		public DateNavigatorProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		#region Properties
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DateNavigatorPropertiesBoldAppointmentDates"),
#endif
NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public virtual bool BoldAppointmentDates {
			get { return GetBoolProperty("BoldAppointmentDates", true); }
			set {
				SetBoolProperty("BoldAppointmentDates", true, value);
				Changed();
			}
		}
		#endregion
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			DateNavigatorProperties properties = source as DateNavigatorProperties;
			if (properties != null)
				BoldAppointmentDates = properties.BoldAppointmentDates;
		}
		#region Hide unused properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableMultiSelect { get { return true; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public override bool ShowClearButton {
			get { return false; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NotifyParentProperty(true), DefaultValue(""),
		AutoFormatDisable, Localizable(true)]
		public override string ClearButtonText {
			get { return String.Empty; }
			set { }
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
		#endregion
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public interface IASPxDateNavigatorControllerOwner : IDateNavigatorControllerOwner {
		TimeInterval GetCalendarInterval();
		TimeInterval GetCalendarVisibleInterval();
	}
	public class ASPxDateNavigatorController : DateNavigatorController {
		public ASPxDateNavigatorController(IASPxDateNavigatorControllerOwner owner)
			: base(owner) {
		}
		protected IASPxDateNavigatorControllerOwner DateNavigator { get { return (IASPxDateNavigatorControllerOwner)Owner;} }
		protected internal override void OnInnerControlChanged() {
			if (InnerControl == null)
				return;
			InnerSchedulerViewBase view = InnerControl.ActiveView;
			TimeInterval selectionInterval = view.GetVisibleIntervals().Interval;
			TimeInterval calendarInterval = DateNavigator.GetCalendarInterval();
			TimeInterval intersection = calendarInterval.Intersect(selectionInterval);
			if (intersection.Duration == TimeSpan.Zero)
				Owner.StartDate = CalculateCalendarStartDate();
			else if (InnerControl.ActiveViewType == SchedulerViewType.Month) {
				TimeInterval visibleInterval2 = DateNavigator.GetCalendarVisibleInterval();
				TimeInterval intersection2 = visibleInterval2.Intersect(selectionInterval);
				if (intersection2.Duration != selectionInterval.Duration)
				Owner.StartDate = CalculateCalendarStartDate();
			}
			if (!IsIntervalsEquals(Owner.GetSelection(), view.GetVisibleIntervals())) {
				UpdateSelection();
			}
			UpdateAppointmentDatesMapCore();
			RaiseChanged();
		}		
		DateTime CalculateCalendarStartDate() {
			InnerSchedulerViewBase view = InnerControl.ActiveView;
			DateTime someDate = view.InnerVisibleIntervals.Start;
			if (InnerControl.ActiveViewType == SchedulerViewType.Month)
				someDate = someDate.AddDays(7);
			return Owner.GetFirstDayOfMonth(someDate.Date);
		}
		bool IsIntervalsEquals(DayIntervalCollection selection, TimeIntervalCollection schedulerVisibleIntervals) {
			if (selection.IsContinuous())
				return selection.Interval.Equals(schedulerVisibleIntervals.Interval);
			int selectionCount = selection.Count;
			if (selectionCount != schedulerVisibleIntervals.Count)
				return false;
			for (int i = 0; i < selectionCount; i++) {
				if (!selection[i].Equals(schedulerVisibleIntervals[i]))
					return false;
			}
			return true;
		}
		void UpdateSelection() {
			DayIntervalCollection days = new DayIntervalCollection();
			if (InnerControl != null)
				days.AddRange(InnerControl.ActiveView.GetVisibleIntervals());
			Owner.SetSelection(days);
		}
	}
	[ToolboxItem(false)]
	public class ASPxDateNavigatorCalendar : ASPxCalendar {
		protected internal const string SchedulerCalendarScriptResourceName = "Scripts.ClientCalendar.js";
		DateTime todayDate;
		public ASPxDateNavigatorCalendar() {
			TodayDate = DateTime.Now.Date;
		}
		protected internal new DateNavigatorProperties Properties { get { return (DateNavigatorProperties)base.Properties; } }
		internal DateTime TodayDate {
			get { return todayDate; }
			set { todayDate = value; }
		}
		#region For ASPxScheduler
		public DateTime GetFirstDate() {
			DateTime date = EffectiveVisibleDate;
			return new DateTime(date.Year, date.Month, 1);
		}
		public DateTime GetLastDate() {
			DateTime date = GetFirstDate();
			return date.AddMonths(Columns * Rows).AddDays(-1);
		}
		#endregion
		protected override EditPropertiesBase CreateProperties() {
			return new DateNavigatorProperties(this);
		}
		protected override bool AllowClientSideRendering() {
			return true;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxScheduler), SchedulerCalendarScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientSchedulerCalendar";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.actualTodayDate = {1};\n", localVarName, HtmlConvertor.ToScript(TodayDate));
		}
		protected override DateTime GetActualTodayDate() {
			return todayDate;
		}
	}
}

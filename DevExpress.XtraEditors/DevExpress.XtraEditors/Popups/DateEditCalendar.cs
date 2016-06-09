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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Calendar;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing.Animation;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraTab.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.XtraEditors.Drawing {
	public class VistaClockPainter : CalendarRightAreaObjectPainter {
		public override void Draw(CalendarControlInfoArgs info) {
			if(info.ViewInfo.Calendar.CalendarTimeEditing != DefaultBoolean.True)
				return ;
			DrawBackground(info);
			DrawClockText(info);
			DrawClockFace(info);
			DrawArrows(info);
			DrawClockGlass(info);
		}
		VistaCalendarRightAreaViewInfo RightArea(CalendarControlInfoArgs e) { return (VistaCalendarRightAreaViewInfo)e.ViewInfo.RightArea; }
		protected virtual void DrawClockFace(CalendarControlInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, RightArea(e).GetClockFaceInfo());
		}
		protected virtual void DrawClockGlass(CalendarControlInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, RightArea(e).GetClockGlassInfo());
		}
		protected virtual Color GetSecArrowColor(CalendarControlInfoArgs e) {
			return EditorsSkins.GetSkin(e.ViewInfo.Calendar.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinDateEditClockFace].Color.GetForeColor();
		}
		protected virtual Color GetMinArrowColor(CalendarControlInfoArgs e) {
			return EditorsSkins.GetSkin(e.ViewInfo.Calendar.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinDateEditClockFace].Color.GetForeColor();
		}
		protected virtual Color GetHourArrowColor(CalendarControlInfoArgs e) {
			return EditorsSkins.GetSkin(e.ViewInfo.Calendar.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinDateEditClockFace].Color.GetForeColor();
		}
		protected virtual int GetSecArrowThickness(CalendarControlInfoArgs e) { return 1; }
		protected virtual int GetMinArrowThickness(CalendarControlInfoArgs e) { return 2; }
		protected virtual int GetHourArrowThickness(CalendarControlInfoArgs e) { return 2; }
		protected virtual void DrawArrows(CalendarControlInfoArgs e) {
			SmoothingMode mode = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			VistaCalendarRightAreaViewInfo vi = RightArea(e);
			e.Graphics.DrawLine(e.Cache.GetPen(GetSecArrowColor(e), GetSecArrowThickness(e)), vi.SecArrow[0], vi.SecArrow[1]);
			e.Graphics.DrawLine(e.Cache.GetPen(GetMinArrowColor(e), GetMinArrowThickness(e)), vi.MinArrow[0], vi.MinArrow[1]);
			e.Graphics.DrawLine(e.Cache.GetPen(GetHourArrowColor(e), GetHourArrowThickness(e)), vi.HourArrow[0], vi.HourArrow[1]);
			e.Graphics.SmoothingMode = mode;
		}
		protected virtual void DrawBackground(CalendarControlInfoArgs e) {
			e.Cache.FillRectangle(e.ViewInfo.PaintAppearance.GetBackBrush(e.Cache), RightArea(e).ClockBounds);
		}
		protected virtual void DrawClockText(CalendarControlInfoArgs e) {
		}
	}
	public class VistaCalendarCellPainter : CalendarCellPainter {
		public VistaCalendarCellPainter() { }
	}
	public class CalendarControlInfoArgs : ControlGraphicsInfoArgs {
		public CalendarControlInfoArgs(BaseControlViewInfo viewInfo, GraphicsCache cache, Rectangle bounds) : base(viewInfo, cache, bounds) { }
		public new CalendarViewInfoBase ViewInfo { get { return (CalendarViewInfoBase)base.ViewInfo; } }
	}
	public class NewCalendarObjectPainter : CalendarPainter {
		protected override void DrawHeader(CalendarControlInfoArgs info) {
			NewClassicCalendarViewInfo viewInfo = (NewClassicCalendarViewInfo)info.ViewInfo;
			viewInfo.PaintStyle.HeaderAppearance.DrawString(info.Cache, viewInfo.MonthCaption, viewInfo.CaptionMonthBounds);
			viewInfo.PaintStyle.HeaderAppearance.DrawString(info.Cache, viewInfo.YearCaption, viewInfo.CaptionYearBounds);
		}
	}
	public class CalendarRightAreaObjectPainter {
		public virtual void Draw(CalendarControlInfoArgs info) { }
	}
	public class CalendarHeaderObjectPainter { 
		public virtual void Draw(CalendarControlInfoArgs info) { 
			CalendarHeaderViewInfoBase header = (CalendarHeaderViewInfoBase)info.ViewInfo.Header;
			if(header.ViewInfo.SwitchState) {
				DrawSwitchState(info);
				return;
			}
			DrawContent(info);
		}
		protected virtual void DrawCaptions(CalendarControlInfoArgs info) {
			CalendarHeaderViewInfo header = (CalendarHeaderViewInfo)info.ViewInfo.Header;
			AppearanceObject appearance = header.ViewInfo.PaintStyle.HeaderAppearance;
			if(header.ViewInfo.SwitchState) {
				info.Cache.DrawString(header.MonthCaption, appearance.Font, info.Cache.GetSolidBrush(ColorAnimationHelper.BlendColor(appearance, 1.0f - info.ViewInfo.Opacity)), header.MonthCaptionBounds, appearance.GetStringFormat());
				info.Cache.DrawString(header.YearCaption, appearance.Font, info.Cache.GetSolidBrush(ColorAnimationHelper.BlendColor(appearance, 1.0f - info.ViewInfo.Opacity)), header.YearCaptionBounds, appearance.GetStringFormat());
			}
			else {
				info.ViewInfo.PaintStyle.HeaderAppearance.DrawString(info.Cache, header.MonthCaption, header.MonthCaptionBounds);
				info.ViewInfo.PaintStyle.HeaderAppearance.DrawString(info.Cache, header.YearCaption, header.YearCaptionBounds);
			}
		}
		protected virtual void DrawContent(CalendarControlInfoArgs info) {
			CalendarHeaderViewInfo header = (CalendarHeaderViewInfo)info.ViewInfo.Header;
			DrawCaptions(info);
			if(header.ShowMonthNavigationButtons) {
				DrawNavigationButton(info, header.MonthIncreaseButton);
				DrawNavigationButton(info, header.MonthDecreaseButton);
			}
			if(header.ShowYearNavigationButtons) {
				DrawNavigationButton(info, header.YearDecreaseButton);
				DrawNavigationButton(info, header.YearIncreaseButton);
			}
		}
		protected virtual void DrawSwitchState(CalendarControlInfoArgs info) {
			DrawContent(info);
		}
		protected virtual void DrawNavigationButton(CalendarControlInfoArgs info, CalendarNavigationButtonViewInfo button) {
			BitmapAnimationInfo animInfo = (BitmapAnimationInfo)XtraAnimator.Current.Get(info.ViewInfo.Calendar, button);
			if(animInfo != null && XtraAnimator.Current.DrawFrame(info.Cache, animInfo))
				return;
			info.ViewInfo.PaintStyle.NavigationButtonPainter.DrawObject(new CalendarNavigationButtonInfoArgs(info.Cache, button));
		}
	}
	public class VistaDateEditCalendarFooterObjectPainter : CalendarFooterObjectPainter {
		public override void Draw(CalendarControlInfoArgs info) {
			base.Draw(info);
			VistaCalendarFooterViewInfo footer = (VistaCalendarFooterViewInfo)((CalendarViewInfo)info.ViewInfo).Footer;
			if(footer.ShowOkButton) {
				DrawButton(info, footer.OkButtonInfo);
			}
			if(footer.ShowCancelButton) {
				DrawButton(info, footer.CancelButtonInfo);
			}
		}
	}
	public class CalendarFooterObjectPainter {
		public virtual void Draw(CalendarControlInfoArgs info) {
			CalendarFooterViewInfo footer = (CalendarFooterViewInfo)((CalendarViewInfo)info.ViewInfo).Footer;
			if(footer.ShowTodayButton) {
				DrawButton(info, footer.TodayButtonInfo);
			}
			if(footer.ShowClearButton) {
				DrawButton(info, footer.ClearButtonInfo);
			}
		}
		protected virtual void DrawButton(CalendarControlInfoArgs info, EditorButtonObjectInfoArgs buttonInfo) {
			CalendarFooterViewInfo footer = (CalendarFooterViewInfo)((CalendarViewInfo)info.ViewInfo).Footer;
			try {
				buttonInfo.Cache = info.Cache;
				BitmapAnimationInfo animInfo = (BitmapAnimationInfo)XtraAnimator.Current.Get(info.ViewInfo.Calendar, buttonInfo);
				if(animInfo != null && XtraAnimator.Current.DrawFrame(info.Cache, animInfo))
					return;
				footer.ViewInfo.PaintStyle.ButtonPainter.DrawObject(buttonInfo);
			}
			finally {
				buttonInfo.Cache = null;
			}
		}
	}
	public class VistaDateEditCalendarHeaderObjectPainter : CalendarHeaderObjectPainter {
		protected override void DrawContent(CalendarControlInfoArgs info) {
			DrawTodayButton(info);
			DrawCaptionButton(info);
			if(info.ViewInfo.Header.ShowMonthNavigationButtons) {
				DrawBackwardArrow(info);
				DrawForwardArrow(info);
			}
			if(info.ViewInfo.Header.ShowYearNavigationButtons && info.ViewInfo.Calendar.View == DateEditCalendarViewType.MonthInfo) {
				DrawBackwardArrow2(info);
				DrawForwardArrow2(info);
			}
		}
		protected virtual void DrawForwardArrow2(CalendarControlInfoArgs info) {
			VistaCalendarHeaderViewInfo header = (VistaCalendarHeaderViewInfo)info.ViewInfo.Header;
			if(XtraAnimator.Current.DrawFrame(info.Cache, header.Calendar, header.ForwardArrow2))
				return;
			info.ViewInfo.PaintStyle.NavigationButtonPainter.DrawObject(new CalendarNavigationButtonInfoArgs(info.Cache, header.ForwardArrow2));
		}
		protected virtual void DrawBackwardArrow2(CalendarControlInfoArgs info) {
			VistaCalendarHeaderViewInfo header = (VistaCalendarHeaderViewInfo)info.ViewInfo.Header;
			if(XtraAnimator.Current.DrawFrame(info.Cache, header.Calendar, header.BackwardArrow2))
				return;
			info.ViewInfo.PaintStyle.NavigationButtonPainter.DrawObject(new CalendarNavigationButtonInfoArgs(info.Cache, header.BackwardArrow2));
		}
		protected override void DrawSwitchState(CalendarControlInfoArgs info) {
			DrawContent(info);
		}
		protected virtual void DrawCaptionButton(CalendarControlInfoArgs info) {
			VistaCalendarHeaderViewInfo header = (VistaCalendarHeaderViewInfo)info.ViewInfo.Header;
			if(XtraAnimator.Current.DrawFrame(info.Cache, header.Calendar, header.CaptionButton))
				return;
			info.ViewInfo.PaintStyle.NavigationButtonPainter.DrawObject(new CalendarNavigationButtonInfoArgs(info.Cache, header.CaptionButton));
		}
		protected virtual void DrawTodayButton(CalendarControlInfoArgs info) {
			VistaCalendarHeaderViewInfo header = (VistaCalendarHeaderViewInfo)info.ViewInfo.Header;
			if(XtraAnimator.Current.DrawFrame(info.Cache, header.Calendar, header.TodayButton))
				return;
			info.ViewInfo.PaintStyle.NavigationButtonPainter.DrawObject(new CalendarNavigationButtonInfoArgs(info.Cache, header.TodayButton));
		}
		protected virtual void DrawForwardArrow(CalendarControlInfoArgs info) {
			VistaCalendarHeaderViewInfo header = (VistaCalendarHeaderViewInfo)info.ViewInfo.Header;
			if(XtraAnimator.Current.DrawFrame(info.Cache, header.Calendar, header.ForwardArrow))
				return;
			info.ViewInfo.PaintStyle.NavigationButtonPainter.DrawObject(new CalendarNavigationButtonInfoArgs(info.Cache, header.ForwardArrow));
		}
		protected virtual void DrawBackwardArrow(CalendarControlInfoArgs info) {
			VistaCalendarHeaderViewInfo header = (VistaCalendarHeaderViewInfo)info.ViewInfo.Header;
			if(XtraAnimator.Current.DrawFrame(info.Cache, header.Calendar, header.BackwardArrow))
				return;
			info.ViewInfo.PaintStyle.NavigationButtonPainter.DrawObject(new CalendarNavigationButtonInfoArgs(info.Cache, header.BackwardArrow));
		}
	}
	public class VistaDateEditCalendarObjectPainter : CalendarPainter {
		public override CalendarHeaderObjectPainter HeaderPainter {
			get {
				return new VistaDateEditCalendarHeaderObjectPainter();
			}
		}
		public override CalendarFooterObjectPainter FooterPaitner {
			get {
				return new VistaDateEditCalendarFooterObjectPainter();
			}
		}
	}
	public class CalendarPainter : BaseControlPainter {
		public override void Draw(ControlGraphicsInfoArgs info) {
			base.Draw(info);
			CalendarControlInfoArgs dinfo = (CalendarControlInfoArgs)info;
			DrawBackground(dinfo);
			DrawBorder(dinfo);
			DrawHeader(dinfo);
			DrawFooter(dinfo);
			DrawRightArea(dinfo);
			DrawCalendars(dinfo);
		}
		public virtual CalendarRightAreaObjectPainter RightAreaPainter {
			get {
				return new VistaClockPainter();
			}
		}
		protected virtual void DrawRightArea(CalendarControlInfoArgs dinfo) {
			RightAreaPainter.Draw(dinfo);
		}
		public virtual CalendarHeaderObjectPainter HeaderPainter { get { return new CalendarHeaderObjectPainter(); } }
		public virtual CalendarFooterObjectPainter FooterPaitner { get { return new CalendarFooterObjectPainter(); } }
		protected virtual void DrawFooter(CalendarControlInfoArgs info) {
			if(info.ViewInfo.ShowFooter)
				FooterPaitner.Draw(info);
		}
		protected virtual void DrawHeader(CalendarControlInfoArgs info) {
			if(info.ViewInfo.ShowHeader)
				HeaderPainter.Draw(info);
		}
		protected virtual void DrawBorder(CalendarControlInfoArgs info) {
			info.ViewInfo.BorderPainter.DrawObject(new BorderObjectInfoArgs(info.Cache, info.ViewInfo.Bounds, info.ViewInfo.PaintAppearance, ObjectState.Normal));
		}
		protected virtual CalendarObjectPainterBase GetCalendarPainter() {
			return new CalendarObjectPainter();
		}
		public virtual void DrawCalendarsSwitchState(CalendarControlInfoArgs e) {
			GraphicsClipState state = e.Cache.ClipInfo.SaveAndSetClip(e.ViewInfo.CalendarsClientBounds);
			try {
				if(e.ViewInfo.SwitchType == SwitchStateType.SwitchToLeft || e.ViewInfo.SwitchType == SwitchStateType.SwitchToRight)
					DrawHorizontalSwitchState(e);
				else
					DrawSwitchInOut(e);
			}
			finally {
				e.Cache.ClipInfo.RestoreClip(state);
			}
		}
		protected virtual void DrawTwoImages(CalendarControlInfoArgs e, Image image1, Image image2, Rectangle srcRect1, Rectangle destRect1, Rectangle srcRect2, Rectangle destRect2, float opacity) {
			float[][] array = new float[5][];
			array[0] = new float[5] { 1.00f, 0.00f, 0.00f, 0.00f, 0.00f };
			array[1] = new float[5] { 0.00f, 1.00f, 0.00f, 0.00f, 0.00f };
			array[2] = new float[5] { 0.00f, 0.00f, 1.00f, 0.00f, 0.00f };
			array[3] = new float[5] { 0.00f, 0.00f, 0.00f, opacity, 0.00f };
			array[4] = new float[5] { 0.00f, 0.00f, 0.00f, 0.00f, 1.00f };
			ColorMatrix matrix = new ColorMatrix(array);
			using(ImageAttributes imAttr = new ImageAttributes()) {
				imAttr.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				e.Graphics.DrawImage(image1, destRect1, srcRect1.X, srcRect1.Y, srcRect1.Width, srcRect1.Height, GraphicsUnit.Pixel, imAttr);
				matrix.Matrix33 = 1.0f - opacity;
				imAttr.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				e.Graphics.DrawImage(image2, destRect2, srcRect2.X, srcRect2.Y, srcRect2.Width, srcRect2.Height, GraphicsUnit.Pixel, imAttr);
			}
		}
		protected virtual void DrawSwitchInOut(CalendarControlInfoArgs e) {
			e.Graphics.FillRectangle(e.Cache.GetSolidBrush(e.ViewInfo.PaintAppearance.BackColor), e.ViewInfo.Bounds);
			Rectangle srcRect = new Rectangle(0, 0, e.ViewInfo.CurrStateImage.Width, e.ViewInfo.CurrStateImage.Height);
			Rectangle destRect = e.ViewInfo.CurrStateBounds;
			DrawTwoImages(e, e.ViewInfo.CurrStateImage, e.ViewInfo.PrevStateImage, srcRect, e.ViewInfo.NextStateCurrBounds, srcRect, e.ViewInfo.CurrStateBounds, 1.0f - e.ViewInfo.Opacity);
		}
		protected virtual void DrawHorizontalSwitchState(CalendarControlInfoArgs e) {
			RectangleF srcRect = new RectangleF(0.0f, 0.0f, e.ViewInfo.CalendarsClientBounds.Width, e.ViewInfo.CalendarsClientBounds.Height);
			RectangleF destRect = new RectangleF(e.ViewInfo.CalendarsClientBounds.X, e.ViewInfo.CalendarsClientBounds.Y, e.ViewInfo.CalendarsClientBounds.Width, e.ViewInfo.CalendarsClientBounds.Height);
			float offset = e.ViewInfo.AnimatedContentProgress * srcRect.Width;
			if(e.ViewInfo.SwitchType == SwitchStateType.SwitchToLeft)
				destRect.Offset(-offset, 0);
			else
				destRect.Offset(offset, 0);
			e.Cache.Graphics.DrawImage(e.ViewInfo.PrevStateImage, destRect, srcRect, GraphicsUnit.Pixel);
			if(e.ViewInfo.SwitchType == SwitchStateType.SwitchToLeft)
				destRect.Offset(e.ViewInfo.PrevStateImage.Width, 0);
			else
				destRect.Offset(-e.ViewInfo.PrevStateImage.Width, 0);
			e.Cache.Graphics.DrawImage(e.ViewInfo.CurrStateImage, destRect, srcRect, GraphicsUnit.Pixel);
		}
		protected virtual void DrawCalendars(CalendarControlInfoArgs info) {
			CalendarViewInfoBase viewInfo = (CalendarViewInfoBase)info.ViewInfo;
			if(viewInfo.SwitchState) {
				DrawCalendarsSwitchState(info);
				return;
			}
			ObjectPainter calendarPainter = GetCalendarPainter();
			foreach(CalendarObjectViewInfo cal in viewInfo.Calendars) {
				calendarPainter.DrawObject(new CalendarControlObjectInfoArgs(info, cal, info.Cache));
			}
		}
		protected virtual void DrawBackground(CalendarControlInfoArgs info) {
			CalendarViewInfoBase viewInfo = (CalendarViewInfoBase)info.ViewInfo;
			viewInfo.PaintAppearance.FillRectangle(info.Cache, info.ViewInfo.Bounds);
		}
	}
	public class CalendarObjectPainter : CalendarObjectPainterBase {
		protected override void DrawHeader(CalendarControlObjectInfoArgs e) {
			CalendarObjectViewInfo vi = (CalendarObjectViewInfo)e.ViewInfo;
			vi.CalendarInfo.PaintStyle.CalendarHeaderAppearance.DrawString(e.Cache, vi.Header.Caption, vi.Header.ContentBounds);
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class CalendarObjectViewInfo : CalendarObjectViewInfoBase {
		public CalendarObjectViewInfo(CalendarControlBase calendar)
			: base(calendar) {
		}
		public override UserLookAndFeel LookAndFeel { get { return Calendar.LookAndFeel; } }
		public override bool ShowWeekNumbers { 
			get { return Calendar.ShowWeekNumbers; } 
		}
		public override DateTime DateTime { get { return Calendar.Handler.SelectedDate; } }
		public override DateTimeFormatInfo DateFormat { get { return Calendar.DateFormat; } }
		public override DayOfWeek FirstDayOfWeek { get { return Calendar.FirstDayOfWeek; } }
		public override DateTime MinValue { 
			get {
				DateTime dt = CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime;
				return dt > Calendar.MinValue ? dt : Calendar.MinValue;
			} 
		}
		public override DateTime MaxValue { 
			get {
				DateTime dt = CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime;
				return dt < Calendar.MaxValue? dt: Calendar.MaxValue; 
			} 
		}
		protected internal override void UpdateContent() {
			if(ShowHeader)
				Header.UpdateContent();
		}
		protected override Rectangle CalcContentBounds() {
			Rectangle res = base.CalcContentBounds();
			if(ShowHeader) {
				res.Y += Header.Size.Height;
				res.Height -= Header.Size.Height;
			}
			return res;
		}
		public override void LayoutCalendar(Rectangle bounds) {
			base.LayoutCalendar(bounds);
			if(ShowHeader)
				Header.CalcViewInfo(bounds);
		}
		protected IStyleController StyleController { get { return Calendar.StyleController; } }
		protected internal bool AllowAddInactiveDatesFromLeft { get; set; }
		protected internal bool AllowAddInacitveDatesFromRight { get; set; }
		protected override bool CanAddDate(DateTime date) {
			if(!base.CanAddDate(date))
				return false;
			DateTime startDate = CalcCalendarStartDate();
			DateTime endDate = CalcCalendarEndDate();
			if(!AllowAddInactiveDatesFromLeft && date < startDate)
				return false;
			if(!AllowAddInacitveDatesFromRight && date > endDate)
				return false;
			return true;
		}
		protected virtual DateTime CalcCalendarEndDate() {
			switch(Calendar.View) { 
				case DateEditCalendarViewType.MonthInfo:
				case DateEditCalendarViewType.QuarterInfo:
					return (new DateTime(CurrentDate.Year, CurrentDate.Month, DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month), 23, 59, 59, CurrentDate.Kind));
				case DateEditCalendarViewType.YearInfo:
					return (new DateTime(CurrentDate.Year, 12, 31, 23, 59, 59, CurrentDate.Kind));
				case DateEditCalendarViewType.YearsInfo:
					return (new DateTime(Math.Min(DateTime.MaxValue.Year, CurrentDate.Year + 9), 12, 31, 23, 59, 59, CurrentDate.Kind));
				case DateEditCalendarViewType.YearsGroupInfo:
					return (new DateTime(Math.Min(DateTime.MaxValue.Year, CurrentDate.Year + 99), 12, 31, 23, 59, 59, CurrentDate.Kind));
			}
			return Calendar.MaxValue;
		}
		private DateTime CalcCalendarStartDate() {
			return CurrentDate;
		}
		public override void Reset() {
			base.Reset();
		}
		protected virtual bool IsMonthSelected(DateTime dateTime) {
			DateTime startMonth = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 1);
			DateTime endMonth = (new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month), 23, 59, 59));
			return Calendar.SelectedRanges.IsIntersect(startMonth, endMonth);
		}
		protected virtual bool IsYearSelected(DateTime dateTime) {
			DateTime startYear = new DateTime(dateTime.Year, 1, 1, 0, 0, 1);
			DateTime endYear = new DateTime(dateTime.Year, 12, DateTime.DaysInMonth(dateTime.Year, 12));
			return Calendar.SelectedRanges.IsIntersect(startYear, endYear);
		}
		protected virtual bool IsYearsGroupSelected(DateTime dateTime) {
			DateTime startYear = new DateTime(Math.Max((dateTime.Year / 10) * 10, 1), 1, 1, 0, 0, 1);
			DateTime endYear = new DateTime(startYear.Year + 9, 12, DateTime.DaysInMonth(startYear.Year, 12));
			return Calendar.SelectedRanges.IsIntersect(startYear, endYear);
		}
		protected virtual bool IsQuarterSelected(DateTime dateTime) {
			DateTime startMonth = new DateTime(dateTime.Year, (dateTime.Month / 3) * 3, 1, 0, 0, 1);
			DateTime endMonth = new DateTime(startMonth.Year, startMonth.Month + 3, DateTime.DaysInMonth(startMonth.Year, startMonth.Month + 3));
			return Calendar.SelectedRanges.IsIntersect(startMonth, endMonth);   
		}
		protected internal override bool IsDateSelected(CalendarCellViewInfo cell) {
			switch(Calendar.View) {
				case DateEditCalendarViewType.MonthInfo:
					return Calendar.SelectedRanges.IsDateSelected(cell.Date);
				case DateEditCalendarViewType.YearInfo:
					return IsMonthSelected(cell.Date);
				case DateEditCalendarViewType.QuarterInfo:
					return IsQuarterSelected(cell.Date);
				case DateEditCalendarViewType.YearsInfo:
					return IsYearSelected(cell.Date);
				case DateEditCalendarViewType.YearsGroupInfo:
					return IsYearsGroupSelected(cell.Date);
			}
			return false;
		}
		protected override CalendarWeekRule GetWeekNumberRule() {
			if(Calendar.WeekNumberRule == WeekNumberRule.Default) 
				return base.GetWeekNumberRule();
			return (CalendarWeekRule)Calendar.WeekNumberRule;
		}
		public virtual CalendarCellViewInfo GetCellByDate(DateTime dateTime) {
			foreach(CalendarCellViewInfo cell in DayCells) {
				if(cell.Date == dateTime)
					return cell;
			}
			return null;
		}
		public void FillSelectedCells(List<CalendarCellViewInfo> list) {
			foreach(CalendarCellViewInfo cell in DayCells) {
				if(cell.Selected)
					list.Add(cell);
			}
		}
		protected internal virtual void ResetContextButtons() {
			foreach(CalendarCellViewInfo cell in DayCells) {
				cell.ContextButtonsViewInfo.InvalidateViewInfo();
			}
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	public enum SwitchStateType { SwitchToLeft, SwitchToRight, SwitchInOut }
	public enum DateEditCalendarSwitchObject { MonthCaption, YearCaption }
	internal static class SizeFHelper { 
		public static Size ToSizeCeiling(this SizeF val) { return new Size((int)Math.Ceiling(val.Width + 0.1f), (int)Math.Ceiling(val.Height)); }
	}
	public interface ICalendarDisabledDateProvider {
		bool IsDisabledDate(DateTime date, DateEditCalendarViewType view);
	}
	public interface ICalendarSpecialDateProvider {
		bool IsSpecialDate(DateTime date, DateEditCalendarViewType view);
	}
	public interface ICalendarCellStyleProvider {
		void UpdateAppearance(CalendarCellStyle cell);
	}
	public class CalendarCellStyle {
		public DateEditCalendarViewType View { get; internal set; }
		public DateTime Date { get; internal set; }
		public AppearanceObject Appearance { get; set; }
		public bool Holiday { get; internal set; }
		public bool IsSpecial { get; internal set; }
		public bool Active { get; internal set; }
		public ObjectState State { get; internal set; }
		public CalendarPaintStyle PaintStyle { get; internal set; }
		public Image Image { get; set; }
		public ImageScaleMode ImageSizeMode { get; set; }
		public ImageAlignmentMode ImageAlignment { get; set; }
		public Rectangle Bounds { get; internal set; }
		public SkinPaddingEdges ImageMargins { get; set; }
		public string Description { get; set; }
		public AppearanceObject DescriptionAppearance { get; set; }
	}
	[DXToolboxItem(DXToolboxItemKind.Regular), ToolboxTabName(AssemblyInfo.DXTabCommon), 
	Description("Displays a monthly calendar and allows an end-user to select a date.")]
	public class CalendarControl : CalendarControlBase {
		public CalendarControl() {
			ResetViewInfo();
			ResetPainter();
			ResetHandler();
			Init();
		}
		protected internal virtual void IncView() {
			if(View == GetMaxView()) return;
			DateEditCalendarViewType nextView = View + 1;
			while(nextView < GetMaxView()) {
				if(IsAllowedView(nextView))
					break;
				nextView += 1;
			}
			View = nextView;
		}
		protected internal DateEditCalendarViewType GetMinView() {
			VistaCalendarViewStyle style = GetVistaCalendarViewStyle();
			if(style == VistaCalendarViewStyle.Default || style == VistaCalendarViewStyle.All)
				return DateEditCalendarViewType.MonthInfo;
			if((style & VistaCalendarViewStyle.MonthView) == VistaCalendarViewStyle.MonthView)
				return DateEditCalendarViewType.MonthInfo;
			if((style & VistaCalendarViewStyle.YearView) == VistaCalendarViewStyle.YearView)
				return DateEditCalendarViewType.YearInfo;
			if((style & VistaCalendarViewStyle.QuarterView) == VistaCalendarViewStyle.QuarterView)
				return DateEditCalendarViewType.QuarterInfo;
			if((style & VistaCalendarViewStyle.YearsGroupView) == VistaCalendarViewStyle.YearsGroupView)
				return DateEditCalendarViewType.YearsInfo;
			return DateEditCalendarViewType.YearsGroupInfo;
		}
		protected DateEditCalendarViewType GetMaxView() {
			VistaCalendarViewStyle style = GetVistaCalendarViewStyle();
			if(style == VistaCalendarViewStyle.Default || style == VistaCalendarViewStyle.All)
				return DateEditCalendarViewType.YearsGroupInfo;
			if((style & VistaCalendarViewStyle.CenturyView) == VistaCalendarViewStyle.CenturyView)
				return DateEditCalendarViewType.YearsGroupInfo;
			if((style & VistaCalendarViewStyle.YearsGroupView) == VistaCalendarViewStyle.YearsGroupView)
				return DateEditCalendarViewType.YearsInfo;
			if((style & VistaCalendarViewStyle.QuarterView) == VistaCalendarViewStyle.QuarterView)
				return DateEditCalendarViewType.QuarterInfo;
			if((style & VistaCalendarViewStyle.YearView) == VistaCalendarViewStyle.YearView)
				return DateEditCalendarViewType.YearInfo;
			return DateEditCalendarViewType.MonthInfo;
		}
		protected internal virtual bool IsAllowedView(DateEditCalendarViewType view) {
			VistaCalendarViewStyle style = GetVistaCalendarViewStyle();
			if(view == DateEditCalendarViewType.MonthInfo)
				return (style & VistaCalendarViewStyle.MonthView) != 0;
			if(view == DateEditCalendarViewType.QuarterInfo)
				return (style & VistaCalendarViewStyle.QuarterView) != 0;
			if(view == DateEditCalendarViewType.YearInfo)
				return (style & VistaCalendarViewStyle.YearView) != 0;
			if(view == DateEditCalendarViewType.YearsInfo)
				return (style & VistaCalendarViewStyle.YearsGroupView) != 0;
			if(view == DateEditCalendarViewType.YearsGroupInfo)
				return (style & VistaCalendarViewStyle.CenturyView) != 0;
			return false;
		}
		protected internal virtual void DecView() {
			if(View == GetMinView()) return;
			DateEditCalendarViewType nextView = View - 1;
			while(nextView > GetMinView()) {
				if(IsAllowedView(nextView))
					break;
				nextView -= 1;
			}
			View = nextView;
		}
		private static readonly object okClick = new object();
		protected internal virtual void RaiseOkClick() {
			EventHandler handler = Events[okClick] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		public event EventHandler OkClick {
			add { Events.AddHandler(okClick, value); }
			remove { Events.RemoveHandler(okClick, value); }
		}
		private static readonly object cancelClick = new object();
		public event EventHandler CancelClick {
			add { Events.AddHandler(cancelClick, value); }
			remove { Events.RemoveHandler(cancelClick, value); }
		}
		protected internal virtual void RaiseCancelClick() {
			EventHandler handler = Events[cancelClick] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal override bool ClockIsLeft {
			get { return IsRightToLeft; }
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			if(ActualCalendarView == CalendarView.Vista)
				return new VistaCalendarViewInfo(this);
			return new CalendarViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			if(ActualCalendarView == CalendarView.Vista)
				return new VistaDateEditCalendarObjectPainter();
			return new CalendarPainter();
		}
		protected virtual bool IsInOpenedPopup { get { return false; } }
		protected override DateTime ChangeSelectedDay(DateTime value) {
			if(RowCount > 1 || ColumnCount > 1)
				return value;
			if(value.Month != DateTime.Month && IsInOpenedPopup)
				return value.AddDays(1 - value.Day);
			return value;
		}
		protected internal virtual void AccessibleNotifyClients(AccessibleEvents events, int childId) {
			AccessibilityNotifyClients(events, childId);
		}
		protected internal virtual void ProcessTabPopupKey() {
			OnDateTimeCommit(DateTime);
		}
		public virtual void ResetState(object editDate, DateTime dt) {
			EditValue = editDate;
		}
		protected virtual void UpdateStartView() { }
		protected override Color GetBackColorCore() { return Color.Empty; }
		protected internal virtual System.DateTime GetTodayDateTime() {
			DateTime today = GetTodayDate();
			if(ShowTimeEdit)
				return new DateTime(today.Year, today.Month, today.Day, TimeEdit.Time.Hour, TimeEdit.Time.Minute, TimeEdit.Time.Second);
			return today;
		}
		protected internal void CheckViewInfo() {
			CheckViewInfo((Graphics)null);
		}
		protected internal virtual bool IsPopupCalendar { get { return false; } }
		internal void ForceUpdateStartDate() {
			UpdateStartDate();
			OnStartDateChanged();
		}
	}
	public class CalendarCollection : CollectionBase {
		public CalendarObjectViewInfo this[int index] { get { return (CalendarObjectViewInfo)InnerList[index]; } }
		public int Add(CalendarObjectViewInfo val) {
			return InnerList.Contains(val) ? InnerList.IndexOf(val) : InnerList.Add(val);
		}
		public int IndexOf(CalendarObjectViewInfo val) {
			return InnerList.IndexOf(val);
		}
		public bool IsFirstCalendar(int index) { return index == 0; }
		public bool IsLastCalendar(int index) { return index == Count - 1; }
		public bool IsFirstCalendar(CalendarObjectViewInfo calendar) { return IsFirstCalendar(IndexOf(calendar)); }
		public bool IsLastCalendar(CalendarObjectViewInfo calendar) { return IsLastCalendar(IndexOf(calendar)); }
	}
	public enum DateEditCalendarViewType { MonthInfo, YearInfo, QuarterInfo, YearsInfo, YearsGroupInfo };
	public enum CalendarInactiveDaysVisibility { FirstLast, Visible, Hidden }
	public enum CalendarSelectionBehavior { Simple, OutlookStyle }
	public class CalendarAreaViewInfoBase {
		public CalendarAreaViewInfoBase(CalendarViewInfoBase viewInfo) {
			ViewInfo = viewInfo;
		}
		protected internal virtual void UpdateContentAnimationParams() {
		}
		protected internal virtual void UpdateVisualInfo() {
		}
		protected internal virtual void Clear() { }
		public CalendarControlBase Calendar { get { return ViewInfo.Calendar; } }
		protected internal void UpdateButtonAppearance(EditorButtonObjectInfoArgs button) {
			if(button.State == ObjectState.Hot)
				button.Button.Appearance.Assign(ViewInfo.PaintStyle.HighlightButtonAppearance);
			else if(button.State == ObjectState.Pressed)
				button.Button.Appearance.Assign(ViewInfo.PaintStyle.PressedButtonAppearance);
			else
				button.Button.Appearance.Assign(ViewInfo.PaintStyle.ButtonAppearance);
			button.UpdateButtonAppearance();
		}
		protected virtual void CheckButtonState(CalendarHitInfoType calendarHitTest, EditorButtonObjectInfoArgs button) {
			ObjectState prevState = button.State;
			button.State = ViewInfo.CalcObjectState(calendarHitTest, button);
			UpdateButtonAppearance(button);
			if(prevState != button.State) {
				OnButtonStateChanged(button, prevState, button.State);
			}
		}
		protected virtual void CheckButtonState(CalendarHitInfoType calendarHitTest, CalendarNavigationButtonViewInfo button) {
			ObjectState prevState = button.State;
			button.State = ViewInfo.CalcObjectState(calendarHitTest, button);
			if(prevState != button.State) {
				OnButtonStateChanged(button, prevState, button.State);
			}
		}
		protected virtual void OnButtonStateChanged(CalendarNavigationButtonViewInfo button, ObjectState prevState, ObjectState newState) {
			CalendarControlInfoArgs info = new CalendarControlInfoArgs(ViewInfo, ViewInfo.GInfo.Cache, button.Bounds);
			CalendarAnimationFramePainter painter = new CalendarAnimationFramePainter(info, ViewInfo.PaintStyle.NavigationButtonPainter, new CalendarNavigationButtonInfoArgs(ViewInfo.GInfo.Cache, button));
			button.State = prevState;
			Bitmap from = XtraAnimator.Current.CreateBitmap(painter, new CalendarNavigationButtonInfoArgs(ViewInfo.GInfo.Cache, button), button.Bounds, true);
			button.State = newState;
			Bitmap to = XtraAnimator.Current.CreateBitmap(painter, new CalendarNavigationButtonInfoArgs(ViewInfo.GInfo.Cache, button), button.Bounds, true);
			XtraAnimator.Current.AddAnimation(new BitmapAnimationInfo(ViewInfo.Calendar, button, from, to, button.Bounds, CalendarViewInfoBase.StateChangeAnimationLength));
		}
		protected virtual void OnButtonStateChanged(EditorButtonObjectInfoArgs button, ObjectState prevState, ObjectState newState) {
			CalendarControlInfoArgs info = new CalendarControlInfoArgs(ViewInfo, ViewInfo.GInfo.Cache, button.Bounds);
			CalendarAnimationFramePainter painter = new CalendarAnimationFramePainter(info, ViewInfo.PaintStyle.ButtonPainter, button);
			button.State = prevState;
			Bitmap from = XtraAnimator.Current.CreateBitmap(painter, button, button.Bounds, true);
			button.State = newState;
			Bitmap to = XtraAnimator.Current.CreateBitmap(painter, button, button.Bounds, true);
			XtraAnimator.Current.AddAnimation(new BitmapAnimationInfo(ViewInfo.Calendar, button, from, to, button.Bounds, CalendarViewInfoBase.StateChangeAnimationLength));
		}
		public CalendarViewInfoBase ViewInfo { get; private set; }
		protected internal virtual bool UpdateVisualInfo(CalendarHitInfo info) { return false; }
		public Size CalcBestSize() {
			Size = CalcBestSizeCore();
			return Size;
		}
		protected virtual Size CalcBestSizeCore() {
			return Size.Empty;
		}
		public virtual void CalcViewInfo(Rectangle bounds) {
			Bounds = bounds;
			ContentBounds = CalcContentBounds(Bounds);
		}
		protected virtual Rectangle CalcContentBounds(Rectangle bounds) {
			return bounds;
		}
		protected virtual Size CalcSizeFromContentSize(Size contentSize) {
			return contentSize;
		}
		public Size Size { get; protected set; }
		public Rectangle Bounds { get; private set; }
		public Rectangle ContentBounds { get; private set; }
		public virtual void UpdateContent() { 
		}
		protected internal virtual string FormatString(string str) {
			if(!str.Contains("&"))
				return str;
			int ampIndex = str.IndexOf('&');
			string res = str.Substring(0, ampIndex);
			if(ampIndex != str.Length - 1) res += str.Substring(ampIndex + 1);
			return res;
		}
		public virtual void CalcHitInfo(CalendarHitInfo hitInfo) { }
	}
	public class CalendarAnimationFramePainter : ObjectPainter {
		public CalendarAnimationFramePainter(CalendarControlInfoArgs info, ObjectPainter objectPainter, ObjectInfoArgs objectInfoArgs) {
			CalendarInfo = info;
			ObjectPainter = objectPainter;
			ObjectInfoArgs = objectInfoArgs;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			CalendarInfo.Cache = e.Cache;
			CalendarInfo.ViewInfo.PaintStyle.CalendarPainter.DrawBackground(CalendarInfo, ObjectInfoArgs.Bounds);
			ObjectInfoArgs.Cache = e.Cache;
			ObjectPainter.DrawObject(ObjectInfoArgs);
		}
		public CalendarControlInfoArgs CalendarInfo { get; private set; }
		public ObjectPainter ObjectPainter { get; private set; }
		public ObjectInfoArgs ObjectInfoArgs { get; private set; }
	}
	public class CalendarObjectHeaderViewInfoBase : CalendarAreaViewInfoBase {
		public CalendarObjectHeaderViewInfoBase(CalendarObjectViewInfoBase viewInfo) : base(viewInfo.CalendarInfo) {
			CalendarObjectInfo = viewInfo;
		}
		public CalendarObjectViewInfoBase CalendarObjectInfo { get; private set; }
		protected DateTime CurrentMonth { get { return CalendarObjectInfo.CurrentDate; } }
		public string Caption { get; protected set; }
		public override void UpdateContent() {
			base.UpdateContent();
			Caption = GetHeaderText();
		}
		protected virtual string GetHeaderText() {
			return GetHeaderText(CurrentMonth);
		}
		protected bool IsLocalizedDateValid(DateTime date) {
			return date >= CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime && date <= CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime;
		}
		protected virtual string GetHeaderText(DateTime dateTime) {
			if(!IsLocalizedDateValid(dateTime))
				return "";
			string res = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month) + " " + dateTime.ToString("yyyy", CultureInfo.CurrentCulture);
			return ViewInfo.GetTextCase(res, Calendar.CaseMonthNames);
		}
		protected virtual Size CalcHeaderTextSize() {
			Size res = Size.Empty;
			for(int i = 1; i <= 12; i++) {
				DateTime dt = new DateTime(CurrentMonth.Year, i, 1);
				Size sz = ViewInfo.PaintStyle.CalendarHeaderAppearance.CalcTextSize(ViewInfo.GInfo.Graphics, GetHeaderText(dt), 0).ToSizeCeiling();
				res.Width = Math.Max(sz.Width, res.Width);
				res.Height = Math.Max(sz.Height, res.Height);
			}
			return res;
		}
		protected virtual SkinPaddingEdges GetHeaderContentPaddings() {
			return new SkinPaddingEdges(5);
		}
		protected virtual Size CalcHeaderSize() {
			Size res = CalcHeaderTextSize();
			SkinPaddingEdges p = GetHeaderContentPaddings();
			res.Width += p.Width;
			res.Height += p.Height;
			return res;
		}
		protected override Size CalcBestSizeCore() {
			return CalcHeaderSize();
		}
		protected override Rectangle CalcContentBounds(Rectangle bounds) {
			bounds.Height = Size.Height;
			SkinPaddingEdges e = GetHeaderContentPaddings();
			return new Rectangle(bounds.X + e.Left, bounds.Y + e.Top, bounds.Width - e.Width, bounds.Height - e.Height);
		}
	}
	public class CalendarObjectHeaderViewInfo : CalendarObjectHeaderViewInfoBase {
		public CalendarObjectHeaderViewInfo(CalendarObjectViewInfoBase viewInfo) : base(viewInfo) { }
		protected override SkinPaddingEdges GetHeaderContentPaddings() {
			return new SkinPaddingEdges(0, 5, 0, 5);
		}
	}
	public class CalendarHeaderViewInfoBase : CalendarAreaViewInfoBase {
		public CalendarHeaderViewInfoBase(CalendarViewInfoBase viewInfo) : base(viewInfo) { }
		protected virtual int ButtonIndent { get { return 0; } }
		protected virtual SkinPaddingEdges CaptionPadding { get { return new SkinPaddingEdges(5); } }
		protected virtual Size CalcCaptionSize(Size textSize) {
			textSize.Width += CaptionPadding.Width;
			textSize.Height += CaptionPadding.Height;
			return textSize;
		}
		protected internal virtual bool ShowMonthNavigationButtons { get { return false; } }
		protected internal virtual bool ShowYearNavigationButtons { get { return false; } }
		protected virtual CalendarNavigationButtonViewInfo CreateNavigationButton(CalendarNavigationButtonPredefines kind) {
			CalendarNavigationButtonViewInfo res = new CalendarNavigationButtonViewInfo(this) { Kind = kind };
			UpdateSkinNavigationButton(res);
			return res;
		}
		protected virtual void UpdateSkinNavigationButton(CalendarNavigationButtonViewInfo vi) {
			if(ViewInfo.PaintStyle.IsSkinned) {
				vi.BackgroundElement = EditorsSkins.GetSkin(ViewInfo.PaintStyle.SkinProvider)[EditorsSkins.SkinCalendarNavigationButton];
				if(vi.BackgroundElement == null || vi.BackgroundElement.Image == null)
					vi.BackgroundElement = EditorsSkins.GetSkin(ViewInfo.PaintStyle.SkinProvider)[EditorsSkins.SkinEditorButton];
			}
		}
		protected internal virtual void OnStateChanging(DateEditCalendarViewType prevView, DateEditCalendarViewType currView) {
		}
		protected internal virtual void OnStateChanged(DateEditCalendarViewType prevView, DateEditCalendarViewType currView) {
		}
	}
	public enum CalendarNavigationButtonPredefines { Right, Left, Text }
	public class CalendarNavigationButtonInfoArgs : StyleObjectInfoArgs {
		public CalendarNavigationButtonInfoArgs(GraphicsCache cache, CalendarNavigationButtonViewInfo viewInfo) : base(cache) {
			ViewInfo = viewInfo;
			Bounds = ViewInfo.Bounds;
		}
		public SkinElementInfo GetSkinElementInfo() { 
			SkinElementInfo info = ViewInfo.GetSkinElementInfo();
			if(info != null)
				info.Cache = Cache;
			return info;
		}
		public CalendarNavigationButtonViewInfo ViewInfo { get; private set; }
		public override ObjectState State {
			get { return ViewInfo.State; }
			set { }
		}
		public override AppearanceObject Appearance {
			get { return ViewInfo.PaintAppearance; }
		}
	}
	public class CalendarNavigationButtonPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			CalendarNavigationButtonInfoArgs info = e as CalendarNavigationButtonInfoArgs;
			if(info == null) {
				base.DrawObject(e);
				return;
			}
			UpdateButtonPainter(info);
			SkinElementInfo sinfo = info.GetSkinElementInfo();
			if(sinfo != null) {
				DrawSkinElement(info, sinfo);
			}
			else {
				if(info.ViewInfo.AllowDrawBackground && info.State != ObjectState.Normal)
					DrawBackground(info);
				DrawContent(info);
			}
		}
		protected virtual void DrawSkinElement(CalendarNavigationButtonInfoArgs info, SkinElementInfo sinfo) {
			if(info.ViewInfo.SkinElementRotateFlip == RotateFlipType.RotateNoneFlipNone)
				ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, sinfo);
			else
				new RotateObjectPaintHelper().DrawRotated(info.Cache, sinfo, SkinElementPainter.Default, info.ViewInfo.SkinElementRotateFlip);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			CalendarNavigationButtonInfoArgs info = e as CalendarNavigationButtonInfoArgs;
			if(info == null)
				return base.CalcObjectMinBounds(e);
			UpdateButtonPainter(info);
			Rectangle rect = new Rectangle(Point.Empty, info.ViewInfo.CalcContentSize());
			rect.Width += info.ViewInfo.ContentPadding.Width;
			rect.Height += info.ViewInfo.ContentPadding.Height;
			if(!info.ViewInfo.AllowDrawBackground)
				return rect;
			return GetButtonPainter(info).CalcBoundsByClientRectangle(info, rect);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			CalendarNavigationButtonInfoArgs info = e as CalendarNavigationButtonInfoArgs;
			UpdateButtonPainter(info);
			Rectangle rect = new Rectangle(Point.Empty, info.ViewInfo.CalcContentSize());
			rect.Width += info.ViewInfo.ContentPadding.Width;
			rect.Height += info.ViewInfo.ContentPadding.Height;
			if(!info.ViewInfo.AllowDrawBackground)
				return rect;
			return GetButtonPainter(info).CalcBoundsByClientRectangle(info, rect);
		}
		private void UpdateButtonPainter(CalendarNavigationButtonInfoArgs info) {
			this.buttonPainter = null;
		}
		ObjectPainter buttonPainter;
		protected virtual void DrawContent(CalendarNavigationButtonInfoArgs info) {
			if(info.ViewInfo.Kind == CalendarNavigationButtonPredefines.Text) {
				DrawText(info);
				return;
			}
			DrawArrow(info, info.ViewInfo.Kind == CalendarNavigationButtonPredefines.Left);
		}
		protected virtual void DrawArrow(CalendarNavigationButtonInfoArgs info, bool isLeft) {
			PointF[] points = new PointF[3];
			if(isLeft) {
				points[0] = new PointF(info.ViewInfo.ContentBounds.Right - 1, info.ViewInfo.ContentBounds.Top - 1);
				points[1] = new PointF(info.ViewInfo.ContentBounds.Right - 1, info.ViewInfo.ContentBounds.Bottom - 1);
				points[2] = new PointF(info.ViewInfo.ContentBounds.Left - 1, (info.ViewInfo.ContentBounds.Top + info.ViewInfo.ContentBounds.Bottom) / 2.0f - 1);
			}
			else {
				points[0] = new PointF(info.ViewInfo.ContentBounds.Left, info.ViewInfo.ContentBounds.Top - 1);
				points[1] = new PointF(info.ViewInfo.ContentBounds.Left, info.ViewInfo.ContentBounds.Bottom - 1);
				points[2] = new PointF(info.ViewInfo.ContentBounds.Right, (info.ViewInfo.ContentBounds.Top + info.ViewInfo.ContentBounds.Bottom) / 2.0f - 1);
			}
			SmoothingMode mode = info.Graphics.SmoothingMode;
			try {
				info.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				info.Graphics.FillPolygon(info.Cache.GetSolidBrush(info.ViewInfo.PaintAppearance.ForeColor), points);
			}
			finally {
				info.Graphics.SmoothingMode = mode;
			}
		}
		protected virtual void DrawText(CalendarNavigationButtonInfoArgs info) {
			if(info.ViewInfo.Opacity == 1.0f)
				info.ViewInfo.PaintAppearance.DrawString(info.Cache, info.ViewInfo.Text, info.ViewInfo.ContentBounds);
			else { 
				info.Cache.DrawString(info.ViewInfo.Text, info.ViewInfo.PaintAppearance.Font, info.Cache.GetSolidBrush(ColorAnimationHelper.BlendColor(info.ViewInfo.PaintAppearance, info.ViewInfo.Opacity)), info.ViewInfo.Bounds, info.ViewInfo.PaintAppearance.GetStringFormat());
			}
		}
		protected virtual ObjectPainter GetButtonPainter(CalendarNavigationButtonInfoArgs info) {
			if(this.buttonPainter != null)
				return buttonPainter;
			switch(info.ViewInfo.CalendarInfo.LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Flat: this.buttonPainter = new FlatButtonObjectPainter(); break;
				case ActiveLookAndFeelStyle.Office2003: this.buttonPainter = new Office2003ButtonObjectPainter(); break;
				case ActiveLookAndFeelStyle.Skin: this.buttonPainter = new SkinCalendarNavigationButtonPainter(info.ViewInfo.CalendarInfo.LookAndFeel.ActiveLookAndFeel); break;
				case ActiveLookAndFeelStyle.Style3D: this.buttonPainter = new Style3DButtonObjectPainter(); break;
				case ActiveLookAndFeelStyle.UltraFlat: this.buttonPainter = new UltraFlatButtonObjectPainter(); break;
				case ActiveLookAndFeelStyle.WindowsXP: this.buttonPainter = new UltraFlatButtonObjectPainter(); break;
				default: this.buttonPainter = new FlatButtonObjectPainter(); break;
			}
			return this.buttonPainter;
		}
		protected virtual void DrawBackground(CalendarNavigationButtonInfoArgs info) {
			GetButtonPainter(info).DrawObject(info);
		}
	}
	public class CalendarNavigationButtonViewInfo : ICalendarClickableObject {
		public CalendarNavigationButtonViewInfo(CalendarAreaViewInfoBase viewInfo) {
			ViewInfo = viewInfo;
			Text = string.Empty;
			Opacity = 1.0f;
			SkinElementRotateFlip = RotateFlipType.RotateNoneFlipNone;
			AllowDrawBackground = true;
		}
		public SkinElement SkinElement { get; set; }
		public SkinElement BackgroundElement { get; set; }
		public bool AllowDrawBackground { get; set; }
		public SkinElementInfo GetSkinElementInfo() { return SkinElement == null || !AllowSkining ? null : new SkinElementInfo(SkinElement, Bounds) { State = State, ImageIndex = -1 }; }
		protected bool AllowSkining {
			get {
				if(CalendarInfo == null)
					return false;
				return CalendarInfo.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin;
			}
		}
		public RotateFlipType SkinElementRotateFlip { get; set; }
		public CalendarAreaViewInfoBase ViewInfo { get; private set; }
		public CalendarViewInfoBase CalendarInfo { get { return ViewInfo.ViewInfo; } }
		public float Opacity { get; set; }
		public string Text { get; set; }
		public virtual AppearanceObject PaintAppearance { 
			get {
				if(State == ObjectState.Pressed)
					return CalendarInfo.PaintStyle.PressedHeaderAppearance;
				else if(State == ObjectState.Hot)
					return CalendarInfo.PaintStyle.HighlightHeaderAppearance;
				return CalendarInfo.PaintStyle.HeaderAppearance;
			} 
		}
		public virtual SkinPaddingEdges ContentPadding {
			get { return new SkinPaddingEdges(3); }
		}
		public Rectangle Bounds { get; set; }
		public Rectangle ContentBounds { get; set; }
		public CalendarNavigationButtonPredefines Kind { get; set; }
		public ObjectState State { get; set; }
		protected virtual int ArrowHeight { get { return 9; } }
		protected virtual Size ArrowSize { 
			get { return new Size((int)(ArrowHeight / 2 / Math.Tan(Math.PI / 6)), ArrowHeight); } 
		}
		protected internal virtual Size CalcContentSize() {
			switch(Kind) { 
				case CalendarNavigationButtonPredefines.Left:
				case CalendarNavigationButtonPredefines.Right:
					return ArrowSize;
				case CalendarNavigationButtonPredefines.Text:
					return PaintAppearance.CalcTextSize(CalendarInfo.GInfo.Graphics, Text, 0).ToSizeCeiling();
			}
			return Size.Empty;
		}
		public virtual Size CalcBestSize() {
			SkinElementInfo info = GetSkinElementInfo();
			if(info != null)
				return ObjectPainter.CalcObjectMinBounds(CalendarInfo.GInfo.Graphics, SkinElementPainter.Default, info).Size;
			ContentBounds = new Rectangle(ContentBounds.Location, CalcContentSize());
			Bounds = CalcBoundsByContentBounds(ContentBounds);
			return Bounds.Size;
		}
		protected virtual Rectangle CalcBoundsByContentBounds(Rectangle content) {
			return CalendarInfo.PaintStyle.NavigationButtonPainter.CalcBoundsByClientRectangle(new CalendarNavigationButtonInfoArgs(CalendarInfo.GInfo.Cache, this) { Bounds = content});
		}
		public virtual void CalcViewInfo(Rectangle bounds) {
			Bounds = bounds;
			ContentBounds = CalcContentBounds(Bounds);
		}
		protected virtual Rectangle CalcContentBounds(Rectangle Bounds) {
			return new Rectangle(Bounds.X + (Bounds.Width - ContentBounds.Size.Width) / 2, Bounds.Y + (Bounds.Height - ContentBounds.Size.Height) / 2, ContentBounds.Size.Width, ContentBounds.Size.Height);
		}
		public event EventHandler Click;
		void ICalendarClickableObject.OnClick() {
			if(Click != null)
				Click(this, EventArgs.Empty);
		}
		public CalendarNavigationButtonViewInfo Clone() {
			CalendarNavigationButtonViewInfo res = new CalendarNavigationButtonViewInfo(ViewInfo);
			res.Text = Text;
			res.Bounds = Bounds;
			res.Kind = Kind;
			res.State = State;
			return res;
		}
	}
	public class CalendarEditorButtonObjectInfoArgs : EditorButtonObjectInfoArgs, ICalendarClickableObject {
		public CalendarEditorButtonObjectInfoArgs(EditorButton button, AppearanceObject backStyle) : base(button, backStyle, true) { }
		public void OnClick() {
			Button.RaiseClick();
		}
	}
	public class CalendarHeaderViewInfo : CalendarHeaderViewInfoBase {
		public CalendarHeaderViewInfo(CalendarViewInfoBase viewInfo) : base(viewInfo) { }
		public string MonthCaption { get; private set; }
		public string YearCaption { get; private set; }
		public Size MonthTextSize { get; private set; }
		public Size YearTextSize { get; private set; }
		public Rectangle MonthCaptionBounds { get; private set; }
		public Rectangle YearCaptionBounds { get; private set; }
		protected internal override bool UpdateVisualInfo(CalendarHitInfo info) {
			if(info.HitObject == MonthDecreaseButton) {
				CheckButtonState(CalendarHitInfoType.DecMonth, MonthDecreaseButton);
			}
			else if(info.HitObject == MonthIncreaseButton) {
				CheckButtonState(CalendarHitInfoType.IncMonth, MonthIncreaseButton);
			}
			else if(info.HitObject == YearDecreaseButton) {
				CheckButtonState(CalendarHitInfoType.DecYear, YearDecreaseButton);
			}
			else if(info.HitObject == YearIncreaseButton) {
				CheckButtonState(CalendarHitInfoType.IncYear, YearIncreaseButton);
			}
			else {
				return false;
			}
			return true;
		}
		protected CalendarControl DateEditCalendar { get { return (CalendarControl)Calendar; } }
		protected CalendarControlHandler Handler { get { return (CalendarControlHandler)DateEditCalendar.Handler; } }
		protected internal override void Clear() {
			base.Clear();
			MonthDecreaseButton.Click -= Handler.OnMonthDecreaseButtonClick;
			MonthIncreaseButton.Click -= Handler.OnMonthIncreaseButtonClick;
			YearDecreaseButton.Click -= Handler.OnYearDecreaseButtonClick;
			YearIncreaseButton.Click -= Handler.OnYearIncreaseButtonClick;
		}
		CalendarNavigationButtonViewInfo monthDecreaseButton;
		public CalendarNavigationButtonViewInfo MonthDecreaseButton {
			get {
				if(monthDecreaseButton == null) {
					monthDecreaseButton = CreateNavigationButton(CalendarNavigationButtonPredefines.Left);
					monthDecreaseButton.Click += Handler.OnMonthDecreaseButtonClick;
				}
				return monthDecreaseButton;
			}
		}
		CalendarNavigationButtonViewInfo monthIncreaseButton;
		public CalendarNavigationButtonViewInfo MonthIncreaseButton {
			get {
				if(monthIncreaseButton == null) {
					monthIncreaseButton = CreateNavigationButton(CalendarNavigationButtonPredefines.Right);
					monthIncreaseButton.Click += Handler.OnMonthIncreaseButtonClick;
				}
				return monthIncreaseButton;
			}
		}
		CalendarNavigationButtonViewInfo yearDecreaseButton;
		public CalendarNavigationButtonViewInfo YearDecreaseButton {
			get {
				if(yearDecreaseButton == null) {
					yearDecreaseButton = CreateNavigationButton(CalendarNavigationButtonPredefines.Left);
					yearDecreaseButton.Click += Handler.OnYearDecreaseButtonClick;
				}
				return yearDecreaseButton;
			}
		}
		CalendarNavigationButtonViewInfo yearIncreaseButton;
		public CalendarNavigationButtonViewInfo YearIncreaseButton {
			get {
				if(yearIncreaseButton == null) {
					yearIncreaseButton = CreateNavigationButton(CalendarNavigationButtonPredefines.Right);
					yearIncreaseButton.Click += Handler.OnYearIncreaseButtonClick;
				}
				return yearIncreaseButton;
			}
		}
		public override void CalcHitInfo(CalendarHitInfo hitInfo) {
			if(ShowMonthNavigationButtons && hitInfo.ContainsSet(MonthDecreaseButton.Bounds, CalendarHitInfoType.DecMonth)) {
				hitInfo.HitObject = MonthDecreaseButton;
			}
			else if(ShowMonthNavigationButtons && hitInfo.ContainsSet(MonthIncreaseButton.Bounds, CalendarHitInfoType.IncMonth)) {
				hitInfo.HitObject = MonthIncreaseButton;
			}
			else if(ShowYearNavigationButtons && hitInfo.ContainsSet(YearDecreaseButton.Bounds, CalendarHitInfoType.DecYear)) {
				hitInfo.HitObject = YearDecreaseButton;
			}
			else if(ShowYearNavigationButtons && hitInfo.ContainsSet(YearIncreaseButton.Bounds, CalendarHitInfoType.IncYear)) {
				hitInfo.HitObject = YearIncreaseButton;
			}
		}
		public override void UpdateContent() {
			MonthCaption = GetMonthCaption();
			YearCaption = GetYearCaption();
		}
		protected virtual string GetYearCaption() {
			return ViewInfo.DateTime.ToString("yyyy", ViewInfo.Calendar.DateFormat);
		}
		protected virtual string GetMonthCaption() {
			return ViewInfo.DateTime.ToString("MMMM", ViewInfo.Calendar.DateFormat);
		}
		protected internal override bool ShowMonthNavigationButtons {
			get { return Calendar.ShowMonthNavigationButtons != DefaultBoolean.False; }
		}
		protected internal override bool ShowYearNavigationButtons {
			get { return Calendar.ShowYearNavigationButtons != DefaultBoolean.False; }
		}
		protected override Size CalcBestSizeCore() {
			MonthTextSize = CalcMonthTextSize();
			YearTextSize = CalcYearTextSize();
			MonthTextSize = new Size(Math.Max(MonthTextSize.Width, YearTextSize.Width), MonthTextSize.Height);
			YearTextSize = new Size(Math.Max(MonthTextSize.Width, YearTextSize.Width), YearTextSize.Height);
			Size maxSize = ViewInfo.GetMaxSize(new Size[] { MonthTextSize, YearTextSize });
			MonthCaptionBounds = new Rectangle(MonthCaptionBounds.Location, CalcCaptionSize(maxSize));
			YearCaptionBounds = new Rectangle(YearCaptionBounds.Location, CalcCaptionSize(maxSize));
			UpdateSkinInfo();
			MonthIncreaseButton.Bounds = new Rectangle(MonthIncreaseButton.Bounds.Location, CalcNavigationButtonSize(MonthIncreaseButton));
			MonthDecreaseButton.Bounds = new Rectangle(MonthDecreaseButton.Bounds.Location, CalcNavigationButtonSize(MonthDecreaseButton));
			YearIncreaseButton.Bounds = new Rectangle(YearIncreaseButton.Bounds.Location, CalcNavigationButtonSize(YearIncreaseButton));
			YearDecreaseButton.Bounds = new Rectangle(YearDecreaseButton.Bounds.Location, CalcNavigationButtonSize(YearDecreaseButton));
			Size contentSize = CalcContentBestSize(MonthCaptionBounds.Size, YearCaptionBounds.Size, MonthIncreaseButton.Bounds.Size);
			return CalcSizeFromContentSize(contentSize);
		}
		protected virtual void UpdateSkinInfo() {
			if(ViewInfo.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				MonthIncreaseButton.SkinElement = EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinCalendarNavigationButton];
				MonthDecreaseButton.SkinElement = EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinCalendarNavigationButton];
				YearIncreaseButton.SkinElement = EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinCalendarNavigationButton];
				YearDecreaseButton.SkinElement = EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinCalendarNavigationButton];
				MonthIncreaseButton.SkinElementRotateFlip = RotateFlipType.RotateNoneFlipX;
				YearIncreaseButton.SkinElementRotateFlip = RotateFlipType.RotateNoneFlipX;
			}
			else {
				MonthDecreaseButton.SkinElement = null;
				MonthIncreaseButton.SkinElement = null;
				YearDecreaseButton.SkinElement = null;
				YearIncreaseButton.SkinElement = null;
			}
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			UpdateItemsSize();
			MonthDecreaseButton.CalcViewInfo(CalcMonthDecreaseButtonBounds());
			MonthIncreaseButton.CalcViewInfo(CalcMonthIncreaseButtonBounds());
			MonthCaptionBounds = CalcMonthCaptionBounds();
			YearDecreaseButton.CalcViewInfo(CalcYearDecreaseButtonBounds());
			YearIncreaseButton.CalcViewInfo(CalcYearIncreaseButtonBounds());
			YearCaptionBounds = CalcYearCaptionBounds();
		}
		protected virtual void UpdateItemsSize() {
			if(Bounds.Width > Size.Width)
				return;
			int delta = YearIncreaseButton.Bounds.Width * 4 + RealButtonToCaptionIndent * 4 + ButtonIndent + MonthCaptionBounds.Width + YearCaptionBounds.Width - ContentBounds.Width;
			if(delta <= 0)
				return;
			MonthCaptionBounds = new Rectangle(0, 0, MonthCaptionBounds.Width - delta / 2, MonthCaptionBounds.Height);
			YearCaptionBounds = new Rectangle(0, 0, YearCaptionBounds.Width - delta / 2, YearCaptionBounds.Height);
		}
		protected virtual Rectangle CalcYearCaptionBounds() {
			return new Rectangle(ContentBounds.Right - YearIncreaseButton.Bounds.Width - RealButtonToCaptionIndent - YearCaptionBounds.Width,
				ContentBounds.Y + (ContentBounds.Height - YearCaptionBounds.Height) / 2, YearCaptionBounds.Width, YearCaptionBounds.Height);
		}
		protected virtual Rectangle CalcYearIncreaseButtonBounds() {
			return new Rectangle(ContentBounds.Right - YearIncreaseButton.Bounds.Width,
				ContentBounds.Y + (ContentBounds.Height - YearIncreaseButton.Bounds.Height) / 2,
				YearIncreaseButton.Bounds.Width, YearIncreaseButton.Bounds.Height);
		}
		protected virtual Rectangle CalcYearDecreaseButtonBounds() {
			return new Rectangle(ContentBounds.Right - YearDecreaseButton.Bounds.Width - RealButtonToCaptionIndent * 2 - YearCaptionBounds.Width - YearDecreaseButton.Bounds.Width,
				ContentBounds.Y + (ContentBounds.Height - YearDecreaseButton.Bounds.Height) / 2,
				YearDecreaseButton.Bounds.Width, YearDecreaseButton.Bounds.Height);
		}
		protected virtual int RealButtonToCaptionIndent {
			get {
				if(Bounds.Width < Size.Width)
					return Math.Max(0, (ContentBounds.Width - MonthDecreaseButton.Bounds.Width * 4 - MonthCaptionBounds.Width - YearCaptionBounds.Width - ButtonIndent) / 4);
				return ButtonToCaptionIndent;
			}
		}
		protected virtual Rectangle CalcMonthCaptionBounds() {
			return new Rectangle(ContentBounds.X + MonthDecreaseButton.Bounds.Width + RealButtonToCaptionIndent, ContentBounds.Y + (ContentBounds.Height - MonthCaptionBounds.Height) / 2, MonthCaptionBounds.Width, MonthCaptionBounds.Height);
		}
		protected virtual Rectangle CalcMonthIncreaseButtonBounds() {
			return new Rectangle(ContentBounds.X + MonthDecreaseButton.Bounds.Width + RealButtonToCaptionIndent * 2 + MonthCaptionBounds.Width, ContentBounds.Y + (ContentBounds.Height - MonthIncreaseButton.Bounds.Height) / 2, MonthIncreaseButton.Bounds.Width, MonthIncreaseButton.Bounds.Height);
		}
		protected virtual Rectangle CalcMonthDecreaseButtonBounds() {
			return new Rectangle(ContentBounds.X, ContentBounds.Y + (ContentBounds.Height - MonthDecreaseButton.Bounds.Height) / 2, MonthDecreaseButton.Bounds.Width, MonthDecreaseButton.Bounds.Height);
		}
		protected virtual int ButtonToCaptionIndent { get { return 0; } }
		protected virtual int CaptionIndent { get { return 10; } }
		protected virtual Size CalcContentBestSize(Size monthCaptionSize, Size yearCaptionSize, Size buttonSize) {
			Size res = Size.Empty;
			res.Width = monthCaptionSize.Width + buttonSize.Width * 2 + ButtonToCaptionIndent * 2;
			res.Width += yearCaptionSize.Width + buttonSize.Width * 2 + ButtonToCaptionIndent * 2;
			res.Width += CaptionIndent;
			res.Height = Math.Max(monthCaptionSize.Height, Math.Max(yearCaptionSize.Height, buttonSize.Height));
			return res;
		}
		protected virtual Size CalcNavigationButtonSize(CalendarNavigationButtonViewInfo button) {
			return button.CalcBestSize();
		}
		protected virtual Size CalcYearTextSize() {
			return ViewInfo.PaintStyle.HeaderAppearance.CalcTextSize(ViewInfo.GInfo.Graphics, YearCaption, 0).ToSize();
		}
		protected virtual Size CalcMonthTextSize() {
			Size maxSize = Size.Empty;
			foreach(string month in CultureInfo.CurrentCulture.DateTimeFormat.MonthNames) {
				Size monthSize = ViewInfo.PaintStyle.HeaderAppearance.CalcTextSize(ViewInfo.GInfo.Graphics, month, 0).ToSize();
				maxSize.Width = Math.Max(monthSize.Width, maxSize.Width);
				maxSize.Height = Math.Max(monthSize.Height, maxSize.Height);
			}
			return maxSize;
		}
	}
	public class CalendarFooterViewInfo : CalendarFooterViewInfoBase {
		public CalendarFooterViewInfo(CalendarViewInfoBase viewInfo) : base(viewInfo) { }
		protected override Size CalcBestSizeCore() {
			CalcButtonsBestSize();
			Size contentSize = CalcContentSize();
			return CalcSizeFromContentSize(contentSize);
		}
		protected virtual int MinButtonWidth { get { return 70; } }
		protected virtual int ButtonHorizontalIndent { get { return 10; } }
		public virtual bool ShowTodayButton { get { return Calendar.ShowTodayButton; } }
		public virtual bool ShowClearButton { get { return Calendar.ShowClearButton; } }
		protected internal override bool UpdateVisualInfo(CalendarHitInfo info) {
			if(info.HitObject == TodayButtonInfo) {
				CheckButtonState(CalendarHitInfoType.Today, TodayButtonInfo);
				return true;
			}
			if(info.HitObject == ClearButtonInfo) {
				CheckButtonState(CalendarHitInfoType.Clear, ClearButtonInfo);
				return true;
			}
			return false;
		}
		protected virtual void CalcButtonsBestSize() {
			UpdateButtonAppearance(TodayButtonInfo);
			UpdateButtonAppearance(ClearButtonInfo);
			TodayButtonInfo.Bounds = new Rectangle(TodayButtonInfo.Bounds.Location, CalcTodayButtonSize());
			ClearButtonInfo.Bounds = new Rectangle(ClearButtonInfo.Bounds.Location, CalcClearButtonSize());
			if(ShowTodayButton && ShowClearButton) {
				int maxWidth = Math.Max(MinButtonWidth, Math.Max(TodayButtonInfo.Bounds.Width, ClearButtonInfo.Bounds.Width));
				int maxHeight = Math.Max(TodayButtonInfo.Bounds.Height, ClearButtonInfo.Bounds.Height);
				TodayButtonInfo.Bounds = new Rectangle(TodayButtonInfo.Bounds.X, TodayButtonInfo.Bounds.Y, maxWidth, maxHeight);
				ClearButtonInfo.Bounds = new Rectangle(ClearButtonInfo.Bounds.X, ClearButtonInfo.Bounds.Y, maxWidth, maxHeight);
			}
		}
		protected virtual Size CalcContentSize() {
			Size res = Size.Empty;
			if(TodayButtonInfo.Bounds.Width > 0) {
				res = AddButtonSize(res, TodayButtonInfo);
			}
			if(ClearButtonInfo.Bounds.Width > 0) {
				res.Width += ButtonHorizontalIndent;
				res = AddButtonSize(res, ClearButtonInfo);
			}
			return res;
		}
		protected virtual Size AddButtonSize(Size size, EditorButtonObjectInfoArgs button) {
			size.Width += button.Bounds.Width;
			size.Height = Math.Max(size.Height, button.Bounds.Height);
			return size;
		}
		public override void UpdateContent() {
			base.UpdateContent();
			TodayButton.Caption = TodayButtonCaption;
			ClearButton.Caption = ClearButtonCaption;
		}
		string todayButtonCaption;
		public string TodayButtonCaption { 
			get { 
				if(string.IsNullOrEmpty(todayButtonCaption))
					todayButtonCaption = FormatString(Localizer.Active.GetLocalizedString(StringId.DateEditToday));
				return todayButtonCaption; 
			} 
		}
		string clearButtonCaption;
		public string ClearButtonCaption { 
			get { 
				if(string.IsNullOrEmpty(clearButtonCaption))
					clearButtonCaption = FormatString(Localizer.Active.GetLocalizedString(StringId.DateEditClear));
				return clearButtonCaption; 
			} 
		}
		public Rectangle ButtonPanelBounds { get; private set; }
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			ButtonPanelBounds = CalcButtonPanelBounds();
			UpdateButtonAppearance(TodayButtonInfo);
			UpdateButtonAppearance(ClearButtonInfo);
			TodayButtonInfo.Bounds = CalcTodayButtonBounds();
			ClearButtonInfo.Bounds = CalcClearButtonBounds();
		}
		protected virtual Rectangle CalcButtonPanelBounds() {
			int width = CalcButtonsTotalWidth();
			int height = CalcButtonsTotalHeight();
			return new Rectangle(ContentBounds.X + (ContentBounds.Width - width) / 2, ContentBounds.Y + (ContentBounds.Height - height) / 2, width, height);
		}
		protected virtual int CalcButtonsTotalWidth() {
			int width = 0;
			if(ShowTodayButton)
				width += TodayButtonInfo.Bounds.Width;
			if(ShowClearButton) {
				if(width > 0)
					width += ButtonHorizontalIndent;
				width += ClearButtonInfo.Bounds.Width;
			}
			return width;
		}
		protected virtual int CalcButtonsTotalHeight() {
			return Math.Max(ClearButtonInfo.Bounds.Height, TodayButtonInfo.Bounds.Height);
		}
		protected virtual Rectangle CalcClearButtonBounds() {
			if(!ShowClearButton)
				return Rectangle.Empty;
			return new Rectangle(ButtonPanelBounds.Right - ClearButtonInfo.Bounds.Width, ButtonPanelBounds.Y, ClearButtonInfo.Bounds.Width, ClearButtonInfo.Bounds.Height);
		}
		protected virtual Rectangle CalcTodayButtonBounds() {
			if(!ShowTodayButton)
				return Rectangle.Empty;
			return new Rectangle(ButtonPanelBounds.X, ButtonPanelBounds.Y, TodayButtonInfo.Bounds.Width, TodayButtonInfo.Bounds.Height);
		}
		public override void CalcHitInfo(CalendarHitInfo hitInfo) {
			if(hitInfo.ContainsSet(TodayButtonInfo.Bounds, CalendarHitInfoType.Today)) {
				hitInfo.HitObject = TodayButtonInfo;
			}
			else if(hitInfo.ContainsSet(ClearButtonInfo.Bounds, CalendarHitInfoType.Clear)) {
				hitInfo.HitObject = ClearButtonInfo;
			}
		}
		protected CalendarControlHandler Handler { get { return (CalendarControlHandler)Calendar.Handler; } }
		EditorButton todayButton;
		public EditorButton TodayButton {
			get {
				if(todayButton == null) {
					todayButton = CreateEditorButton();
					todayButton.Click += Handler.OnTodayButtonClick;
				}
				return todayButton;
			}
		}
		protected internal override void Clear() {
			base.Clear();
			ClearButton.Click -= Handler.OnClearButtonClick;
			TodayButton.Click -= Handler.OnTodayButtonClick;
		}
		EditorButton clearButton;
		public EditorButton ClearButton {
			get {
				if(clearButton == null) {
					clearButton = CreateEditorButton();
					clearButton.Click += Handler.OnClearButtonClick;
				}
				return clearButton;
			}
		}
		EditorButtonObjectInfoArgs todayButtonInfo;
		public EditorButtonObjectInfoArgs TodayButtonInfo {
			get {
				if(todayButtonInfo == null)
					todayButtonInfo = CreateButtonInfo(TodayButton);
				return todayButtonInfo;
			}
		}
		EditorButtonObjectInfoArgs clearButtonInfo;
		public EditorButtonObjectInfoArgs ClearButtonInfo {
			get {
				if(clearButtonInfo == null)
					clearButtonInfo = CreateButtonInfo(ClearButton);
				return clearButtonInfo;
			}
		}
		protected virtual EditorButtonObjectInfoArgs CreateButtonInfo(EditorButton button) {
			return new CalendarEditorButtonObjectInfoArgs(button, ViewInfo.PaintAppearance);
		}
		protected virtual EditorButton CreateEditorButton() {
			return new EditorButton() { Kind = ButtonPredefines.Glyph };
		}
		protected virtual Size CalcButtonSize(EditorButtonObjectInfoArgs buttonInfo) {
			buttonInfo.Cache = ViewInfo.GInfo.Cache;
			try {
				return ViewInfo.PaintStyle.ButtonPainter.CalcObjectMinBounds(buttonInfo).Size;
			}
			finally {
				buttonInfo.Cache = null;
			}
		}
		protected virtual Size CalcClearButtonSize() {
			if(!ShowClearButton)
				return Size.Empty;
			return CalcButtonSize(ClearButtonInfo);
		}
		protected virtual Size CalcTodayButtonSize() {
			if(!ShowTodayButton)
				return Size.Empty;
			return CalcButtonSize(TodayButtonInfo);
		}
		public new CalendarControl Calendar { get { return (CalendarControl)ViewInfo.Calendar; } }
	}
	public class CalendarFooterViewInfoBase : CalendarAreaViewInfoBase {
		public CalendarFooterViewInfoBase(CalendarViewInfoBase viewInfo) : base(viewInfo) { }
	}
	public class CalendarNavigationGrid {
		public CalendarNavigationGrid(int rowCount, int columnCount) {
			NavigationGrid = new CalendarCellViewInfo[rowCount, columnCount];
		}
		public CalendarCellViewInfo[,] NavigationGrid { get; set; }
		public void Add(int row, int column, CalendarCellViewInfo cell) {
			NavigationGrid[row, column] = cell;
		}
		Rectangle[] rowsBounds;
		public Rectangle[] RowsBounds {
			get {
				if(rowsBounds == null)
					rowsBounds = CalcRowsBounds();
				return rowsBounds;
			}
		}
		Rectangle[] columnBounds;
		public Rectangle[] ColumnBounds {
			get {
				if(columnBounds == null)
					columnBounds = CalcColumnsBounds();
				return columnBounds;
			}
		}
		public void UpdateBounds() {
			this.rowsBounds = null;
		}
		public int RowCount { get { return NavigationGrid.GetLength(0); } }
		public int ColumnCount { get { return NavigationGrid.GetLength(1); } }
		protected virtual Rectangle[] CalcRowsBounds() {
			Rectangle[] res = new Rectangle[RowCount];
			for(int i = 0; i < RowCount; i++) {
				res[i] = CalcRowBounds(i);	
			}
			return res;
		}
		public CalendarCellViewInfo this[int row, int column] { get { return NavigationGrid[row, column]; } set { NavigationGrid[row, column] = value; } }
		protected virtual Rectangle CalcRowBounds(int row) {
			CalendarCellViewInfo start = null;
			CalendarCellViewInfo end = null;
			for(int i = 0; i < ColumnCount; i++) {
				if(start == null && this[row, i] != null)
					start = this[row, i];
				if(end == null && this[row, ColumnCount - 1 - i] != null)
					end = this[row, ColumnCount - 1 - i];
				if(start != null && end != null)
					break;
			}
			if(start == null)
				return Rectangle.Empty;
			return new Rectangle(start.Bounds.X, start.Bounds.Y, end.Bounds.Right - start.Bounds.X, end.Bounds.Bottom - start.Bounds.Y);
		}
		protected virtual Rectangle[] CalcColumnsBounds() {
			Rectangle[] res = new Rectangle[ColumnCount];
			for(int j = 0; j < ColumnCount; j++) {
				res[j] = CalcColumnBounds(j);
			}
			return res;
		}
		protected virtual Rectangle CalcColumnBounds(int column) {
			CalendarCellViewInfo start = null;
			CalendarCellViewInfo end = null;
			for(int i = 0; i < RowCount; i++) {
				if(start == null && this[i, column] != null)
					start = this[i, column];
				if(end == null && this[RowCount - 1 - i, column] != null)
					end = this[RowCount - 1 - i, column];
				if(start != null && end != null)
					break;
			}
			if(start == null)
				return Rectangle.Empty;
			return new Rectangle(start.Bounds.X, start.Bounds.Y, end.Bounds.Right - start.Bounds.X, end.Bounds.Bottom - start.Bounds.Y);
		}
		public Point GetPosition(CalendarCellViewInfo cell) {
			if(cell == null)
				return new Point(-1, -1);
			for(int row = 0; row < RowCount; row++) {
				for(int column = 0; column < ColumnCount; column++) {
					if(this[row, column] == cell)
						return new Point(row, column);
				}
			}
			return new Point(-1, -1);
		}
	}
	public class CalendarViewInfoBase : BaseStyleControlViewInfo {
		internal static readonly int StateChangeAnimationLength = 100;
		public CalendarViewInfoBase(BaseStyleControl owner) : base(owner) { }
		public virtual bool HighlightHolidays { get { return Calendar.HighlightHolidays; } }
		public virtual bool HighlightTodayCell { get { return Calendar.HighlightTodayCell != DefaultBoolean.False; } }
		public override bool IsReady {
			get {
				return base.IsReady;
			}
			set {
				base.IsReady = value;
			}
		}
		protected internal virtual CalendarCellViewInfo GetFirstVisibleCellInfo() {
			if(!IsReady) return null;
			foreach(CalendarObjectViewInfo calendar in Calendars) {
				if(calendar.DayCells.Count > 0)
					return calendar.DayCells[0];
			}
			return null;
		}
		protected internal virtual CalendarCellViewInfo GetLastVisibleCellInfo() {
			if(!IsReady) return null;
			for(int calendarIndex = Calendars.Count - 1; calendarIndex >= 0; calendarIndex--) {
				if(Calendars[calendarIndex].DayCells.Count > 0)
					return Calendars[calendarIndex].DayCells[Calendars[calendarIndex].DayCells.Count - 1];
			}
			return null;
		}
		protected internal virtual CalendarCellViewInfo GetFirstActiveVisibleCellInfo() {
			if(!IsReady) return null;
			foreach(CalendarObjectViewInfo calendar in Calendars) {
				foreach(CalendarCellViewInfo cell in calendar.DayCells) {
					if(cell.IsActive)
						return cell;
				}
			}
			return null;
		}
		protected internal virtual CalendarCellViewInfo GetLastActiveVisibleCellInfo() {
			if(!IsReady) return null;
			for(int calendarIndex = Calendars.Count - 1; calendarIndex >= 0; calendarIndex--) {
				for(int cellIndex = Calendars[calendarIndex].DayCells.Count - 1; cellIndex >= 0; cellIndex--) {
					if(Calendars[calendarIndex].DayCells[cellIndex].IsActive)
						return Calendars[calendarIndex].DayCells[cellIndex];
				}
			}
			return null;
		}
		CalendarHeaderViewInfoBase header;
		public CalendarHeaderViewInfoBase Header {
			get {
				if(header == null)
					header = CreateHeaderInfo();
				return header;
			}
		}
		CalendarFooterViewInfoBase footer;
		public CalendarFooterViewInfoBase Footer {
			get {
				if(footer == null)
					footer = CreateFooterInfo();
				return footer;
			}
		}
		CalendarAreaViewInfoBase rightArea;
		public CalendarAreaViewInfoBase RightArea {
			get {
				if(rightArea == null)
					rightArea = CreateRightAreaInfo();
				return rightArea;
			}
		}
		protected virtual CalendarAreaViewInfoBase CreateRightAreaInfo() {
			return new CalendarAreaViewInfoBase(this);
		}
		protected virtual CalendarFooterViewInfoBase CreateFooterInfo() {
			return new CalendarFooterViewInfoBase(this);
		}
		protected virtual CalendarHeaderViewInfoBase CreateHeaderInfo() {
			return new CalendarHeaderViewInfoBase(this);
		}
		public CalendarControlBase Calendar { get { return (CalendarControlBase)OwnerControl; } }
		public DateTime DateTime { get { return Calendar.Handler.SelectedDate; } }
		string maxMonthAbbreviatedName;
		protected string MaxMonthAbbreviatedName {
			get {
				if(string.IsNullOrEmpty(maxMonthAbbreviatedName))
					maxMonthAbbreviatedName = GetMaxAbbreviatedMonthName();
				return maxMonthAbbreviatedName;
			}
		}
		protected virtual string DayCellContentTextTemplate { 
			get {
				if(Calendar.View == DateEditCalendarViewType.MonthInfo)
					return "99";
				if(Calendar.View == DateEditCalendarViewType.QuarterInfo)
					return "9";
				if(Calendar.View == DateEditCalendarViewType.YearInfo)
					return GetMaxAbbreviatedMonthName();
				if(Calendar.View == DateEditCalendarViewType.YearsInfo)
					return "9999";
				if(Calendar.View == DateEditCalendarViewType.YearsGroupInfo)
					return "9999-\n9999";
				return "99";
			}
		}
		protected virtual string GetMaxAbbreviatedMonthName() {
			string name = string.Empty;
			for(int i = 1; i <= 12; i++) {
				if(name.Length < Calendar.DateFormat.GetAbbreviatedMonthName(i).Length)
					name = Calendar.DateFormat.GetAbbreviatedMonthName(i);
			}
			StringBuilder builder = new StringBuilder(name.Length);
			builder.Append('W');
			for(int i = 1; i < name.Length; i++)
				builder.Append('w');
			return builder.ToString();
		}
		protected virtual Size CalcDayCellTextSize() {
			return CalcDayCellTextSizeCore(DayCellContentTextTemplate);
		}
		protected virtual Size CalcMonthInfoCellTextSize() {
			return CalcDayCellTextSizeCore("99");
		}
		protected virtual Size CalcDayCellTextSizeCore(string cellTextTemplate) {
			Size normal = PaintStyle.DayCellAppearance.CalcTextSize(GInfo.Cache, cellTextTemplate, 0).ToSizeCeiling();
			Size highlight = PaintStyle.HighlightDayCellAppearance.CalcTextSize(GInfo.Cache, cellTextTemplate, 0).ToSizeCeiling();
			Size inactive = PaintStyle.InactiveDayCellAppearance.CalcTextSize(GInfo.Cache, cellTextTemplate, 0).ToSizeCeiling();
			Size disabled = PaintStyle.DisabledDayCellAppearance.CalcTextSize(GInfo.Cache, cellTextTemplate, 0).ToSizeCeiling();
			Size holiday = PaintStyle.HolidayCellAppearance.CalcTextSize(GInfo.Cache, cellTextTemplate, 0).ToSizeCeiling();
			Size today = PaintStyle.TodayCellAppearance.CalcTextSize(GInfo.Cache, cellTextTemplate, 0).ToSizeCeiling();
			Size selected = PaintStyle.SelectedDayCellAppearance.CalcTextSize(GInfo.Cache, cellTextTemplate, 0).ToSizeCeiling();
			Size special = PaintStyle.DayCellSpecialAppearance.CalcTextSize(GInfo.Cache, cellTextTemplate, 0).ToSizeCeiling();
			return GetMaxSize(new Size[] { normal, highlight, inactive, disabled, holiday, today, selected, special });
		}
		protected internal Size GetMaxSize(Size[] sizes) {
			Size max = Size.Empty;
			foreach(Size size in sizes) {
				max.Width = Math.Max(max.Width, size.Width);
				max.Height = Math.Max(max.Height, size.Height);
			}
			return max;
		}
		protected virtual SkinPaddingEdges ContentPadding {
			get {
				if(Calendar.Padding != new Padding(-1))
					return new SkinPaddingEdges(Calendar.Padding.Left, Calendar.Padding.Top, Calendar.Padding.Right, Calendar.Padding.Bottom);
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
					return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).Properties.GetPadding(EditorsSkins.OptNewCalendarPadding, new SkinPaddingEdges(10));
				return new SkinPaddingEdges(10);
			}
		}
		protected virtual int TextIndent {
			get {
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
					return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).Properties.GetInteger(EditorsSkins.OptNewCalendarTextIndent, 5);
				return 5;
			}
		}
		protected virtual int CalendarIndent {
			get {
				if(Calendar.CalendarIndent > -1)
					return Calendar.CalendarIndent;
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
					return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).Properties.GetInteger(EditorsSkins.OptNewCalendarIndent, 20);
				return 10;
			}
		}
		CalendarPaintStyle paintStyle;
		public CalendarPaintStyle PaintStyle {
			get {
				if(paintStyle == null)
					paintStyle = CreatePaintStyle();
				return paintStyle;
			}
		}
		protected virtual CalendarPaintStyle CreatePaintStyle() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new CalendarPaintStyleSkin(this);
			return new CalendarPaintStyle(this);
		}
		protected internal override void ResetAppearanceDefault() {
			base.ResetAppearanceDefault();
			this.paintStyle = null;
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			PaintStyle.UpdatePaintAppearance();
		}
		private Size CalcSizeByContentSize(Size contentSize) {
			return new Size(contentSize.Width + ContentPadding.Width, contentSize.Height + ContentPadding.Height);
		}
		private Size CalcItemsGroupSize(Size size, int row, int columnt, int indent) {
			return new Size(size.Width * row + indent * (row - 1), size.Height * columnt + (columnt - 1) * indent);
		}
		public virtual CalendarCollection Calendars {
			get;
			private set;
		}
		protected virtual void CreateCalendars() {
			Calendars = new CalendarCollection();
			if(!Calendar.CalendarDateEditing)
				return;
			int count = ActualRowCount * ActualColumnCount;
			for(int i = 0; i < count; i++) {
				CalendarObjectViewInfo info = CreateCalendar(i);
				info.CalendarInfo = this;
				Calendars.Add(info);
			}
		}
		protected virtual bool ShowCalendarHeader(int index) {
			return Calendar.ShowMonthHeaders;
		}
		protected virtual CalendarObjectViewInfo CreateCalendar(int index) {
			return new CalendarObjectViewInfo(Calendar) { ShowHeader = ShowCalendarHeader(index), View = Calendar.View };
		}
		protected virtual Size CalcHeaderSize() {
			if(!ShowHeader)
				return Size.Empty;
			return Header.CalcBestSize();
		}
		protected virtual Size CalcCalendarMonthSize() {
			if(!Calendar.CalendarDateEditing)
				return Size.Empty;
			CalendarObjectViewInfo info = CreateCalendar(0);
			info.CalendarInfo = this;
			info.View = DateEditCalendarViewType.MonthInfo;
			return info.CalcBestSize(GInfo.Cache);
		}
		protected virtual Size CalcCalendarContentSize() {
			return CalcCalendarsGroupSize(CalendarMonthSize, RowCount, ColumnCount);
		}
		public Size ContentSize { get; protected set; }
		public Size HeaderSize { get; protected set; }
		public Size FooterSize { get; protected set; }
		public Size RightAreaSize { get; protected set; }
		protected internal Size CalendarMonthSize { get; set; }
		protected virtual Size CalcFooterSize() {
			if(!ShowFooter)
				return Size.Empty;
			return Footer.CalcBestSize();
		}
		protected virtual Size CalcTotalContentSize(Size headerSize, Size contentSize, Size footerSize, Size rightAreaSize) {
			return new Size(Math.Max(headerSize.Width, Math.Max(footerSize.Width, contentSize.Width + rightAreaSize.Width)), headerSize.Height + Math.Max(rightAreaSize.Height, contentSize.Height) + footerSize.Height);
		}
		protected internal virtual bool ShowHeader { 
			get { return Calendar.CalendarDateEditing && Calendar.ShowHeader; } 
		}
		protected internal virtual bool ShowFooter {
			get {
				if(Calendar.CalendarTimeEditing == DefaultBoolean.True)
					return true;
				return Calendar.CalendarDateEditing && Calendar.ShowFooter; }
		}
		protected virtual void CalcBestSizeCore() {
			CalendarMonthSize = CalcCalendarMonthSize();
			ContentSize = CalcCalendarContentSize();
			HeaderSize = CalcHeaderSize();
			FooterSize = CalcFooterSize();
			RightAreaSize = CalcRightAreaSize();
		}
		public override Size CalcBestFit(Graphics g) {
			if(IsReady || LockCalcBestSize)
				return BestSize;
			GInfo.AddGraphics(g);
			try {
				UpdateContent();
				CalcConstants();
				CalcElementsSize();
				CalcBestSizeCore();
				Size size = CalcTotalContentSize(HeaderSize, ContentSize, FooterSize, RightAreaSize);
				BestSize = CalcSizeFromContentSize(size);
				return BestSize;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual Size CalcRightAreaSize() {
			return RightArea.CalcBestSize();
		}
		protected virtual Size CalcSizeFromContentSize(Size size) {
			size.Width += ContentPadding.Width;
			size.Height += ContentPadding.Height;
			return BorderPainter.CalcBoundsByClientRectangle(new BorderObjectInfoArgs(GInfo.Cache, new Rectangle(Point.Empty, size), PaintAppearance, ObjectState.Normal)).Size;
		}
		protected virtual Size CalcCalendarsGroupSize(Size calendarSize, int rowCount, int colCount) {
			return new Size(colCount * calendarSize.Width + (colCount - 1) * CalendarIndent, rowCount * calendarSize.Height + (rowCount - 1) * CalendarIndent);
		}
		protected virtual int RowCount { get { return 1; } }
		protected virtual int ColumnCount { get { return 1; } }
		public int ActualRowCount { get; protected set; }
		public int ActualColumnCount { get; protected set; }
		protected internal Size CellSize { get; private set; }
		protected internal Size MonthInfoCellSize { get; private set; }
		protected internal Size DayCellTextSize { get; private set; }
		protected internal Size MonthInfoDayCellTextSize { get; private set; }
		protected internal Size WeekDayTextSize { get; private set; }
		protected internal Size WeekNumberSize { get; private set; }
		protected internal Size MaxCellTextSize { get; set; }
		protected internal int WeekDayVertIndent { get; set; }
		protected virtual Size CalcWeekNumberWidth() {
			return PaintStyle.WeekNumberAppearance.CalcTextSize(GInfo.Graphics, "00", 0).ToSizeCeiling();
		}
		protected virtual int DefaultWeekDayAbbreviationLength { get { return 2; } }
		protected int WeekDayAbbreviationLength {
			get {
				if(Calendar.WeekDayAbbreviationLength == 0)
					return DefaultWeekDayAbbreviationLength;
				return Calendar.WeekDayAbbreviationLength;
			}
		}
		protected internal string GetTextCase(string text, TextCaseMode textCase) { 
			switch(textCase) {
				case TextCaseMode.System: return text;
				case TextCaseMode.UpperCase: return text.ToUpper();
				case TextCaseMode.LowerCase: return text.ToLower();
				case TextCaseMode.SentenceCase: return text.Substring(0, 1).ToUpper() + text.Substring(1, text.Length - 1).ToLower();
			}
			return text;
		}
		protected virtual string GetAbbreviatedDayNameCore(int day) {
			string abbrDayName = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(Convert.ToInt32(Calendar.FirstDayOfWeek) + day) % 7];
			if(Calendar.CaseWeekDayAbbreviations == TextCaseMode.System)
				return abbrDayName;
			if(abbrDayName.Length < WeekDayAbbreviationLength)
				return abbrDayName;
			if(CultureInfo.CurrentCulture.TextInfo.IsRightToLeft)
				return abbrDayName.Substring(abbrDayName.Length - WeekDayAbbreviationLength, WeekDayAbbreviationLength).ToUpper();
			return abbrDayName.Substring(0, WeekDayAbbreviationLength).ToUpper();
		}
		protected internal string GetAbbreviatedDayName(int day) {
			string abbrDayName = GetAbbreviatedDayNameCore(day);
			if(Calendar.CaseWeekDayAbbreviations == TextCaseMode.Default)
				return abbrDayName;
			return GetTextCase(abbrDayName, Calendar.CaseWeekDayAbbreviations);
		}
		private Size CalcWeekDayCellTextSize() {
			Size res = Size.Empty;
			for(int i = 0; i < 7; i++) {
				SizeF sz = PaintStyle.WeekDayAppearance.CalcTextSize(GInfo.Cache, GetAbbreviatedDayName(i), 0);
				res.Width = Math.Max(res.Width, (int)Math.Ceiling(sz.Width));
				res.Height = Math.Max(res.Height, (int)Math.Ceiling(sz.Height));
			}
			return res;
		}
		protected virtual Size CalcMaxCellTextSize() {
			Size res = Size.Empty;
			res = GetMaxSize(res, DayCellTextSize);
			res = GetMaxSize(res, WeekDayTextSize);
			return res;
		}
		private Size GetMaxSize(Size size1, Size size2) {
			return new Size(Math.Max(size1.Width, size2.Width), Math.Max(size1.Height, size2.Height));
		}
		protected internal virtual SkinPaddingEdges GetCellPadding() {
			SkinPaddingEdges res = new SkinPaddingEdges();
			if(!Calendar.IsDefaultCellPadding)
				res = new SkinPaddingEdges(Calendar.CellPadding.Left, Calendar.CellPadding.Top, Calendar.CellPadding.Right, Calendar.CellPadding.Bottom);
			else 
				res = GetCellPaddingCore();
			return ScalePadding(res);
		}
		protected virtual SkinPaddingEdges ScalePadding(SkinPaddingEdges res) {
			if(!LookAndFeel.GetTouchUI())
				return res;
			float touchScale = LookAndFeel.GetTouchScaleFactor();
			return new SkinPaddingEdges((int)(res.Left * touchScale), (int)(res.Top * touchScale), (int)(res.Right * touchScale), (int)(res.Bottom * touchScale));
		}
		protected internal virtual SkinPaddingEdges GetCellPaddingCore() {
			return new SkinPaddingEdges(5);
		}
		protected virtual Size CalcCellSize() {
			if(!Calendar.CellSize.IsEmpty)
				return Calendar.CellSize;
			SkinPaddingEdges pe = GetCellPadding();
			return new Size(MaxCellTextSize.Width + pe.Width, MaxCellTextSize.Height + pe.Height);
		}
		protected virtual Size CalcMonthInfoCellSize() {
			if(!Calendar.CellSize.IsEmpty)
				return Calendar.CellSize;
			SkinPaddingEdges pe = GetCellPadding();
			return new Size(MonthInfoDayCellTextSize.Width + pe.Width, MonthInfoDayCellTextSize.Height + pe.Height);
		}
		protected override void CalcConstants() {
			base.CalcConstants();
			WeekDayVertIndent = 2;
		}
		protected virtual void CalcElementsSize() {
			DayCellTextSize = CalcDayCellTextSize();
			MonthInfoDayCellTextSize = CalcMonthInfoCellTextSize();
			WeekDayTextSize = CalcWeekDayCellTextSize();
			WeekNumberSize = CalcWeekNumberWidth();
			MaxCellTextSize = CalcMaxCellTextSize();
			MonthInfoDayCellTextSize = new Size((int)Math.Max(MonthInfoDayCellTextSize.Width, WeekDayTextSize.Width), MonthInfoDayCellTextSize.Height);
			CellSize = CalcCellSize();
			MonthInfoCellSize = CalcMonthInfoCellSize();
		}
		public Rectangle CalendarBounds { get; protected set; }
		public Rectangle HeaderBounds { get; protected set; }
		public Rectangle FooterBounds { get; protected set; }
		public Rectangle RightAreaBounds { get; protected set; }
		public Rectangle CalendarsClientBounds { get; protected set; }
		public Rectangle CalendarsContentBounds { get; protected set; }
		protected virtual Rectangle CalcCalendarsClientBounds(Rectangle bounds) {
			Rectangle rect = bounds;
			rect.Width -= RightAreaBounds.Width;
			rect.Y += HeaderBounds.Height;
			rect.Height -= HeaderBounds.Height + FooterBounds.Height;
			if(IsRightToLeftLayout)
				rect.X += RightAreaBounds.Width;
			return rect;
		}
		protected virtual bool IsRightToLeftLayout { get { return WindowsFormsSettings.GetIsRightToLeftLayout(Calendar); } }
		protected virtual Rectangle CalcHeaderBounds() {
			if(!ShowHeader)
				return Rectangle.Empty;
			if(IsRightToLeftLayout)
				return new Rectangle(CalendarBounds.X + RightAreaSize.Width, CalendarBounds.Y, CalendarBounds.Width - RightAreaSize.Width, HeaderSize.Height);
			return new Rectangle(CalendarBounds.X, CalendarBounds.Y, CalendarBounds.Width - RightAreaSize.Width, HeaderSize.Height);
		}
		protected virtual Rectangle CalcFooterBounds() {
			if(!ShowFooter)
				return Rectangle.Empty;
			if(IsRightToLeftLayout)
				return new Rectangle(CalendarBounds.X + RightAreaSize.Width, CalendarBounds.Bottom - FooterSize.Height, CalendarBounds.Width - RightAreaSize.Width, FooterSize.Height);
			return new Rectangle(CalendarBounds.X, CalendarBounds.Bottom - FooterSize.Height, CalendarBounds.Width - RightAreaSize.Width, FooterSize.Height);
		}
		protected virtual int GetActualColumnCount(Rectangle calendarsRect, Size calendarSize) {
			if(!Calendar.CalendarDateEditing)
				return 0;
			int columnCount = Math.Max((calendarsRect.Width + CalendarIndent) / (calendarSize.Width + CalendarIndent), 1);
			if(Calendar.ColumnCount != 0)
				return Math.Min(Calendar.ColumnCount, columnCount);
			return columnCount;
		}
		protected virtual int GetActualRowCount(Rectangle calendarsRect, Size calendarSize) {
			if(!Calendar.CalendarDateEditing)
				return 0;
			int rowCount = Math.Max((calendarsRect.Height + CalendarIndent) / (calendarSize.Height + CalendarIndent), 1);
			if(Calendar.RowCount != 0)
				return Math.Min(Calendar.RowCount, rowCount);
			return rowCount;
		}
		protected Size BestSize { get; set; }
		protected override void CalcRects() {
			base.CalcRects();
			UpdateContent();
			CalcConstants();
			CalcElementsSize();
			if(!Calendar.AutoSize || BestSize.IsEmpty)
				CalcBestFit(GInfo.Graphics);
			CalendarBounds = CalcCalendarBounds(ContentRect);
			CalendarMonthSize = CalcCalendarMonthSize();
			HeaderBounds = CalcHeaderBounds();
			FooterBounds = CalcFooterBounds();
			RightAreaBounds = CalcRightAreaBounds();
			CalendarsClientBounds = CalcCalendarsClientBounds(CalendarBounds);
			ActualColumnCount = GetActualColumnCount(CalendarsClientBounds, CalendarMonthSize);
			ActualRowCount = GetActualRowCount(CalendarsClientBounds, CalendarMonthSize);
			CalendarsContentBounds = CalcCalendarsContentBounds(CalendarsClientBounds);
			CalendarsClientBounds = CalendarsContentBounds;
			FooterBounds = UpdateFooterBounds();
			LayoutHeader();
			LayoutFooter();
			LayoutRightArea();
			CreateNavigationGrid();
			CreateCalendars();
			UpdateCalendarsContent();
			LayoutCalendars();
		}
		protected virtual Rectangle UpdateRightAreaBounds() {
			return new Rectangle(RightAreaBounds.X, CalendarsContentBounds.Y, RightAreaBounds.Width, CalendarsContentBounds.Height);
		}
		protected virtual Rectangle UpdateFooterBounds() {
			if(!ShowFooter)
				return Rectangle.Empty;
			return new Rectangle(FooterBounds.X, CalendarsContentBounds.Top + Math.Max(RightAreaSize.Height, CalendarsContentBounds.Height), FooterBounds.Width, FooterBounds.Height);
		}
		public CalendarNavigationGrid NavigationGrid { get; protected set; }
		protected virtual void CreateNavigationGrid() {
			int cellColumnCount = GetCalendarCellColumnCount();
			int cellRowCount = GetCalendarCellRowCount();
			NavigationGrid = new CalendarNavigationGrid(cellRowCount * ActualRowCount, cellColumnCount * ActualColumnCount);
		}
		public void AddCellToNavigationGrid(CalendarCellViewInfo cell, int calendarRow, int calendarColumn, int cellRow, int cellColumn) {
			int cellRowCount = GetCalendarCellRowCount();
			int cellColumnCount = GetCalendarCellColumnCount();
			NavigationGrid.Add(calendarRow * cellRowCount + cellRow, calendarColumn * cellColumnCount + cellColumn, cell);
		}
		protected virtual void UpdateCalendarsContent() {
			DateTime date = Calendar.StartDate;
			foreach(CalendarObjectViewInfo vi in Calendars) {
				vi.CurrentDate = date;
				vi.UpdateContent();
				date = IncrementDate(date);
			}
		}
		protected virtual DateTime IncrementDate(DateTime date) {
			switch(Calendar.View) { 
				case DateEditCalendarViewType.MonthInfo:
				case DateEditCalendarViewType.QuarterInfo:
					return date.AddMonths(1);
				case DateEditCalendarViewType.YearInfo:
					return date.AddMonths(12);
				case DateEditCalendarViewType.YearsInfo:
					return date.AddYears(10);
				case DateEditCalendarViewType.YearsGroupInfo:
					return date.AddYears(100);
			}
			return date.AddMonths(1);
		}
		protected virtual Rectangle CalcRightAreaBounds() {
			if(IsRightToLeftLayout)
				return new Rectangle(CalendarBounds.X, HeaderBounds.Bottom, RightAreaSize.Width, CalendarBounds.Height - HeaderBounds.Height - FooterBounds.Height); 
			return new Rectangle(CalendarBounds.Right - RightAreaSize.Width, HeaderBounds.Bottom, RightAreaSize.Width, CalendarBounds.Height - HeaderBounds.Height - FooterBounds.Height); 
		}
		protected virtual void LayoutFooter() {
			Footer.CalcBestSize();
			Footer.CalcViewInfo(FooterBounds);
		}
		protected virtual void LayoutHeader() {
			Header.CalcBestSize();
			Header.CalcViewInfo(HeaderBounds);
		}
		protected virtual void LayoutRightArea() {
			RightArea.CalcBestSize();
			RightArea.CalcViewInfo(RightAreaBounds);
		}
		protected virtual void LayoutCalendars() {
			int index = 0;
			for(int row = 0; row < ActualRowCount; row++) {
				for(int column = 0; column < ActualColumnCount; column++, index++) {
					int calendarPosition = WindowsFormsSettings.GetIsRightToLeftLayout(Calendar) ? (ActualColumnCount - column - 1) : column;
					Rectangle calendarBounds = new Rectangle(CalendarsContentBounds.X + calendarPosition * (CalendarMonthSize.Width + CalendarIndent),
						CalendarsContentBounds.Y + row * (CalendarMonthSize.Height + CalendarIndent),
						CalendarMonthSize.Width, CalendarMonthSize.Height);
					LayoutCalendar(column, row, calendarBounds, Calendars[index]);
					if(Calendars[index].CurrentDate.Year == DateTime.MaxValue.Year && Calendars[index].CurrentDate.Month == DateTime.MaxValue.Month) {
						column = ActualColumnCount;
						break;
					}
				}
			}
		}
		protected internal virtual int GetCalendarCellColumnCount() {
			return GetCalendarCellColumnCount(Calendar.View);
		}
		protected internal virtual int GetCalendarCellColumnCount(DateEditCalendarViewType view) {
			switch(view) {
				case DateEditCalendarViewType.MonthInfo:
					return 7;
				case DateEditCalendarViewType.QuarterInfo:
					return 2;
				case DateEditCalendarViewType.YearInfo:
					return 4;
				case DateEditCalendarViewType.YearsInfo:
					return 4;
				case DateEditCalendarViewType.YearsGroupInfo:
					return 4;
			}
			return 7;
		}
		protected internal virtual int GetCalendarCellRowCount() {
			return GetCalendarCellRowCount(Calendar.View);
		}
		protected internal virtual int GetCalendarCellRowCount(DateEditCalendarViewType view) {
			switch(view) {
				case DateEditCalendarViewType.MonthInfo:
					return 6;
				case DateEditCalendarViewType.QuarterInfo:
					return 2;
				case DateEditCalendarViewType.YearInfo:
				case DateEditCalendarViewType.YearsGroupInfo:
				case DateEditCalendarViewType.YearsInfo:
					return 3;
			}
			return 6;
		}
		private Rectangle LayoutCalendar(int column, int row, Rectangle bounds, CalendarObjectViewInfo calendar) {
			calendar.Row = row;
			calendar.Column = column;
			if(Calendar.InactiveDaysVisibility == CalendarInactiveDaysVisibility.FirstLast) {
				calendar.AllowAddInactiveDatesFromLeft = Calendars.IsFirstCalendar(calendar);
				calendar.AllowAddInacitveDatesFromRight = Calendars.IsLastCalendar(calendar);
			}
			else {
				calendar.AllowAddInactiveDatesFromLeft = Calendar.InactiveDaysVisibility == CalendarInactiveDaysVisibility.Visible;
				calendar.AllowAddInacitveDatesFromRight = Calendar.InactiveDaysVisibility == CalendarInactiveDaysVisibility.Visible;
			}
			calendar.LayoutCalendar(bounds);
			return bounds;
		}
		protected virtual Rectangle CalcCalendarsContentBounds(Rectangle bounds) {
			Rectangle rect = bounds;
			Size sz = CalcCalendarsGroupSize(CalendarMonthSize, ActualRowCount, ActualColumnCount);
			if(Calendar.CenterHorizontally) {
				rect.X += (rect.Width - sz.Width) / 2;
			}
			if(Calendar.CenterVertically && rect.Height > sz.Height) {
				rect.Y += (rect.Height - sz.Height) / 2;
			}
			rect.Size = sz;
			return rect;
		}
		protected bool DockedHorizontally { get { return Calendar.Dock == DockStyle.Left || Calendar.Dock == DockStyle.Right || Calendar.Dock == DockStyle.Fill; } }
		protected bool DockedVertically { get { return Calendar.Dock == DockStyle.Top || Calendar.Dock == DockStyle.Bottom || Calendar.Dock == DockStyle.Fill; } }
		protected virtual Rectangle CalcCalendarBounds(Rectangle bounds) {
			bounds.X += ContentPadding.Left;
			bounds.Y += ContentPadding.Top;
			bounds.Width -= ContentPadding.Width;
			bounds.Height -= ContentPadding.Height;
			if(!Calendar.AutoSize)
				return bounds;
			if(bounds.Width > BestSize.Width || bounds.Height > BestSize.Height)
				bounds = new Rectangle(bounds.X + (bounds.Width - BestSize.Width) / 2, bounds.Y + (bounds.Height - BestSize.Height) / 2, BestSize.Width, BestSize.Height);
			return bounds;
		}
		protected virtual void UpdateContent() {
			Header.UpdateContent();
			Footer.UpdateContent();
			RightArea.UpdateContent();
		}
		public bool PrepareAnimation { get; set; }
		protected internal SwitchStateType SwitchType { get; set; }
		protected internal virtual bool SwitchState { get; set; }
		protected internal Image PrevStateImage { get; set; }
		protected internal Image CurrStateImage { get; set; }
		protected internal virtual float Opacity { get; set; }
		protected internal Rectangle BeginStateBounds { get; set; }
		protected internal Rectangle EndStateBounds { get; set; }
		protected internal Rectangle CurrStateBounds { get; set; }
		protected internal Rectangle NextStateBeginBounds { get; set; }
		protected internal Rectangle NextStateCurrBounds { get; set; }
		protected internal Rectangle NextStateEndBounds { get; set; }
		protected internal virtual Size ImageSize {
			get { return CalendarsClientBounds.Size; }
		}
		protected virtual Rectangle GetCurrentRectangle(Rectangle begin, Rectangle end, float koeff) {
			int x = (int)(begin.X + (end.X - begin.X) * koeff);
			int y = (int)(begin.Y + (end.Y - begin.Y) * koeff);
			int width = (int)(begin.Width + (end.Width - begin.Width) * koeff);
			int height = (int)(begin.Height + (end.Height - begin.Height) * koeff);
			return new Rectangle(x, y, width, height);
		}
		protected virtual void CreatePrevImage() {
			PrevStateImage = Calendar.RenderContentToImage(PrevStateImage);
		}
		protected virtual void CreateCurrImage() {
			CurrStateImage = Calendar.RenderContentToImage(CurrStateImage);
		}
		protected virtual void DrawSwitchTo(BaseAnimationInfo animInfo) {
			SwitchState = true;
			AnimatedContentProgress = (float)SplineHelper.CalcSpline(((float)animInfo.CurrentFrame) / animInfo.FrameCount);
			UpdateContentAnimationParams();
			Calendar.Invalidate(Bounds);
			if(animInfo.IsFinalFrame) 
				SwitchState = false;
		}
		protected virtual void UpdateContentAnimationParams() {
			Opacity = 1.0f - AnimatedContentProgress;
			CurrStateBounds = GetCurrentRectangle(BeginStateBounds, EndStateBounds, AnimatedContentProgress);
			NextStateCurrBounds = GetCurrentRectangle(NextStateBeginBounds, NextStateEndBounds, AnimatedContentProgress);
			if(Header != null)
				Header.UpdateContentAnimationParams();
		}
		CalendarHitInfo pressedInfo, hoverInfo;
		protected internal virtual CalendarHitInfo PressedInfo {
			get {
				if(pressedInfo == null)
					pressedInfo = new CalendarHitInfo(Point.Empty, ObjectState.Pressed);
				return pressedInfo; 
			}
			set {
				if(PressedInfo != null && (PressedInfo.HitTest != value.HitTest || PressedInfo.HitObject != value.HitObject)) {
					CalendarHitInfo hi = PressedInfo;
					OnPressedObjectChanging(hi, value);
					value.HitInfoType = ObjectState.Pressed;
					pressedInfo = value;
					OnPressedObjectChanged(hi, PressedInfo);
				}
			}
		}
		protected virtual void OnPressedObjectChanged(CalendarHitInfo prev, CalendarHitInfo curr) {
			prev.HitInfoType = ObjectState.Normal;
			UpdateVisualInfo(prev);
			UpdateVisualInfo(curr);
			Calendar.UpdateVisual();
		}
		protected virtual void OnPressedObjectChanging(CalendarHitInfo prev, CalendarHitInfo curr) {
			Calendar.UpdateVisual();
		}
		protected internal virtual CalendarHitInfo HoverInfo {
			get {
				if(hoverInfo == null)
					hoverInfo = new CalendarHitInfo(Point.Empty, ObjectState.Hot);
				return hoverInfo; 
			}
			set {
				if(HoverInfo != null && HoverInfo.IsEquals(value))
					return;
				CalendarHitInfo hi = HoverInfo;
				value.HitInfoType = ObjectState.Hot;
				hoverInfo = value;
				OnHotObjectChanged(hi, HoverInfo);
			}
		}
		protected virtual void OnHotObjectChanging(CalendarHitInfo prevInfo, CalendarHitInfo currInfo) { }
		public virtual void UpdateVisualInfo() {
			if(Header != null)
				Header.UpdateVisualInfo();
			if(Footer != null)
				Footer.UpdateVisualInfo();
			if(RightArea != null)
				RightArea.UpdateVisualInfo();
			if(Calendars != null) {
				foreach(CalendarObjectViewInfo calendar in Calendars) {
					calendar.UpdateVisualInfo();
				}
			}
		}
		protected virtual bool UpdateVisualInfo(CalendarHitInfo info) {
			if(info == null)
				return false;
			if(info.IsInHeader)
				return info.Header.UpdateVisualInfo(info);
			if(info.IsInFooter)
				return info.Footer.UpdateVisualInfo(info);
			if(info.IsInRightArea)
				return info.RightArea.UpdateVisualInfo(info);
			if(info.IsInCell)
				return info.Cell.UpdateVisualInfo(info);
			return false;
		}
		protected virtual void OnHotObjectChanged(CalendarHitInfo prevInfo, CalendarHitInfo currInfo) {
			if(prevInfo.IsInCell)
				prevInfo.Cell.ContextButtonsHandler.OnMouseLeave(EventArgs.Empty);
			if(currInfo.IsInCell)
				currInfo.Cell.ContextButtonsHandler.OnMouseEnter(EventArgs.Empty);
			prevInfo.HitInfoType = ObjectState.Normal;
			UpdateVisualInfo(prevInfo);
			UpdateVisualInfo(currInfo);
			Calendar.UpdateVisual();
		}
		protected internal int LockAnimation { get; set; }
		protected internal float AnimatedContentProgress { get; set; }
		protected virtual Rectangle SelectedItemBounds {
			get {
				foreach(CalendarObjectViewInfo calendar in Calendars) {
					foreach(CalendarCellViewInfo cell in calendar.DayCells) {
						if(cell.Selected) return cell.Bounds;
					}	
				}
				return Rectangle.Empty;
			}
		}
		protected internal virtual void OnViewChanging(DateEditCalendarViewType prevView, DateEditCalendarViewType currView, bool shouldUpdateLayout) {
			OnStateChanging(prevView, currView, shouldUpdateLayout);
		}
		protected internal virtual void OnStateChanging(DateEditCalendarViewType prevView, DateEditCalendarViewType currView, bool shouldUpdateLayout) {
			if(shouldUpdateLayout)
				Calendar.LayoutChangedCore(false);
			OnCalendarsStateChanging(prevView, currView);
			if(Header != null)
				Header.OnStateChanging(prevView, currView);
		}
		protected virtual void OnCalendarsStateChanging(DateEditCalendarViewType prevView, DateEditCalendarViewType currView) {
			CreatePrevImage();
			if(prevView < currView)
				BeginStateBounds = CalendarsClientBounds;
			else
				BeginStateBounds = SelectedItemBounds;
			CurrStateBounds = BeginStateBounds;
		}
		internal bool suppressPaint = false;
		private static readonly int SwitchInOutAnimationLength = 75000;
		protected SplineAnimationHelper SplineHelper;
		void ResetBestSize() { BestSize = Size.Empty; }
		bool? ForcedAllowAnimation { get; set; }
		public override bool AllowAnimation {
			get {
				if(ForcedAllowAnimation.HasValue)
					return ForcedAllowAnimation.Value;
				return base.AllowAnimation;
			}
		}
		protected internal virtual void OnStateChanged(DateEditCalendarViewType prevView, DateEditCalendarViewType currView, bool shouldUpdateLayout) {
			if(shouldUpdateLayout) {
				ResetBestSize();
				Calendar.LayoutChangedCore(false);
			}
			OnCalendarsStateChanged(prevView, currView);
			if(Header != null)
				Header.OnStateChanged(prevView, currView);
			if(Calendar.AllowAnimatedContentChange) {
				ForcedAllowAnimation = true;
				SwitchState = true;
				SplineHelper = new SplineAnimationHelper();
				SplineHelper.Init(0.0, 1.0, 1.0);
				XtraAnimator.Current.AddObject(Calendar, Calendar.ContentChangedAnimationId, SwitchInOutAnimationLength, 50, new CustomAnimationInvoker(DrawSwitchTo));
				ForcedAllowAnimation = null;
			}
			else
				Calendar.UpdateVisual();
		}
		protected internal bool IsAnimated { get { return XtraAnimator.Current.Get(Calendar, Calendar.ContentChangedAnimationId) != null; } }
		protected virtual void OnCalendarsStateChanged(DateEditCalendarViewType prevView, DateEditCalendarViewType currView) {
			CreateCurrImage();
			AnimatedContentProgress = 0;
			Opacity = 1.0f;
			if(prevView < currView) 
				EndStateBounds = SelectedItemBounds;
			else
				EndStateBounds = CalendarsClientBounds;
			if(SwitchType == SwitchStateType.SwitchInOut) CalcNextStateRects(prevView, currView);
		}
		protected internal virtual void OnViewChanged(DateEditCalendarViewType prevView, DateEditCalendarViewType currView, bool shouldUpdateLayout) {
			SwitchType = SwitchStateType.SwitchInOut;
			Calendar.UpdateStartDateCore();
			OnStateChanged(prevView, currView, shouldUpdateLayout);
		}
		protected virtual void SwitchStateRects() {
			Rectangle tmp = NextStateBeginBounds; 
			NextStateBeginBounds = NextStateEndBounds; 
			NextStateEndBounds = tmp;
			tmp = BeginStateBounds; 
			BeginStateBounds = NextStateBeginBounds; 
			NextStateBeginBounds = tmp;
			tmp = EndStateBounds; 
			EndStateBounds = NextStateEndBounds; 
			NextStateEndBounds = tmp;
		}
		protected virtual void CalcNextStateRects(DateEditCalendarViewType prevView, DateEditCalendarViewType currView) {
			Rectangle stateBegin = prevView < currView ? BeginStateBounds : EndStateBounds;
			Rectangle stateEnd = prevView < currView ? EndStateBounds : BeginStateBounds;
			int left = stateBegin.Left + (stateBegin.Left - stateEnd.Left) * 2;
			int top = stateBegin.Top + (stateBegin.Top - stateEnd.Top) * 2;
			int right = stateBegin.Right + (stateBegin.Right - stateEnd.Right) * 2;
			int bottom = stateBegin.Bottom + (stateBegin.Bottom - stateEnd.Bottom) * 2;
			NextStateBeginBounds = new Rectangle(left, top, right - left, bottom - top);
			NextStateEndBounds = stateBegin;
			if(prevView < currView) return;
			SwitchStateRects();
		}
		protected internal virtual void UpdateClock() {
		}
		protected virtual int ScrollAreaWidth { get { return 10; } }
		protected virtual Rectangle LeftScrollArea {
			get { return new Rectangle(CalendarsClientBounds.X - ScrollAreaWidth, CalendarsClientBounds.Y, ScrollAreaWidth, CalendarsClientBounds.Height); } 
		}
		protected virtual Rectangle RightScrollArea {
			get { return new Rectangle(CalendarsClientBounds.Right, CalendarsClientBounds.Y, ScrollAreaWidth, CalendarsClientBounds.Height); } 
		}
		protected virtual void CalcHitInfo(CalendarHitInfo hitInfo) {
			if(hitInfo.ContainsSet(LeftScrollArea, CalendarHitInfoType.LeftScrollArea))
				return;
			else if(hitInfo.ContainsSet(RightScrollArea, CalendarHitInfoType.RightScrollArea))
				return;
			else if(Header != null && ShowHeader && Header.Bounds.Contains(hitInfo.HitPoint)) {
				hitInfo.Header = Header;
				Header.CalcHitInfo(hitInfo);
			}
			else if(Footer != null && ShowFooter && Footer.Bounds.Contains(hitInfo.HitPoint)) {
				hitInfo.Footer = Footer;
				Footer.CalcHitInfo(hitInfo);
			}
			else if(RightArea != null && RightArea.Bounds.Contains(hitInfo.HitPoint)) {
				hitInfo.RightArea = RightArea;
				RightArea.CalcHitInfo(hitInfo);
			}
			else {
				foreach(CalendarObjectViewInfo calendar in Calendars) {
					if(calendar.Bounds.Contains(hitInfo.HitPoint)) {
						calendar.CalcHitInfo(hitInfo);
						break;
					}
				}
			}
		}
		public virtual CalendarHitInfo CalcHitInfo(Point point) {
			CalendarHitInfo hitInfo = new CalendarHitInfo(point);
			CalcHitInfo(hitInfo);
			return hitInfo;
		}
		protected internal virtual ObjectState CalcObjectState(CalendarHitInfoType hitTest, object hitObject) {
			if(PressedInfo.HitTest == hitTest && PressedInfo.HitObject == hitObject)
				return ObjectState.Pressed;
			if(HoverInfo.HitTest == hitTest && HoverInfo.HitObject == hitObject)
				return ObjectState.Hot;
			return ObjectState.Normal;
		}
		protected internal bool SuppressStateChangeAnimation { get; set; }
		public CalendarCellViewInfo GetCellByDate(System.DateTime dateTime) {
			if(Calendars == null)
				return null;
			foreach(CalendarObjectViewInfo vi in Calendars) {
				CalendarCellViewInfo cell = vi.GetCellByDate(dateTime);
				if(cell != null)
					return cell;
			}
			return null;
		}
		public List<CalendarCellViewInfo> GetSelectedCells() {
			List<CalendarCellViewInfo> res = new List<CalendarCellViewInfo>();
			foreach(CalendarObjectViewInfo vi in Calendars) {
				vi.FillSelectedCells(res);
			}
			return res;
		}
		protected internal bool LockCalcBestSize { get; set; }
		public Color TodayFrameColor {
			get {
				if(Calendar.TodayDayCellFrameColor != Color.Empty)
					return Calendar.TodayDayCellFrameColor;
				return PaintStyle.TodayFrameColor;
			}
		}
		protected internal virtual void ResetContextButtons() {
			if(Calendars == null)
				return;
			foreach(CalendarObjectViewInfo calendar in Calendars) {
				calendar.ResetContextButtons();
			}
		}
	}
	public class CalendarViewInfo : CalendarViewInfoBase {
		public CalendarViewInfo(CalendarControlBase owner) : base(owner) { }
		protected new CalendarControl Calendar { get { return (CalendarControl)base.Calendar; } }
		protected override int RowCount { 
			get {
				if(Calendar.RowCount == 0)
					return 1;
				return Calendar.RowCount; 
			} 
		}
		protected override int ColumnCount { 
			get {
				if(Calendar.ColumnCount == 0)
					return 1;
				return Calendar.ColumnCount; 
			} 
		}
		protected override CalendarHeaderViewInfoBase CreateHeaderInfo() {
			return new CalendarHeaderViewInfo(this);
		}
		protected override CalendarFooterViewInfoBase CreateFooterInfo() {
			return new CalendarFooterViewInfo(this);
		}
		protected override bool ShowCalendarHeader(int index) {
			return base.ShowCalendarHeader(index) && Calendar.View == DateEditCalendarViewType.MonthInfo;
		}
		protected override CalendarAreaViewInfoBase CreateRightAreaInfo() {
			return new VistaCalendarRightAreaViewInfo(this);
		}
		protected internal override void UpdateClock() {
			((VistaCalendarRightAreaViewInfo)RightArea).UpdateClock();
		}
	}
	public class NewClassicCalendarViewInfo : CalendarViewInfoBase {
		public NewClassicCalendarViewInfo(CalendarControlBase owner) : base(owner) { }
		public Size CaptionMonthTextSize { get; protected set; }
		public Size CaptionYearTextSize { get; protected set; }
		public Rectangle CaptionMonthBounds { get; protected set; }
		public Rectangle CaptionYearBounds { get; protected set; }
		protected virtual Size CalcCaptionTextSize() {
			return PaintStyle.HeaderAppearance.CalcTextSize(GInfo.Cache, MonthCaption, 0).ToSizeCeiling();
		}
		protected virtual Size CalcCaptionYearTextSize() {
			return PaintStyle.HeaderAppearance.CalcTextSize(GInfo.Cache, YearCaption, 0).ToSizeCeiling();
		}
		protected override void UpdateContent() {
			MonthCaption = DateTime.ToString(HeaderFormatString);
			YearCaption = DateTime.Year.ToString();
		}
		protected override Size CalcHeaderSize() {
			return new Size(0, CaptionMonthBounds.Height + CaptionYearBounds.Height);
		}
		protected virtual Size CalcCaptionYearSize() {
			return new Size(CaptionYearTextSize.Width + TextIndent * 2, CaptionYearTextSize.Height + TextIndent * 2);
		}
		protected virtual Size CalcCaptionMonthSize() {
			return new Size(CaptionMonthTextSize.Width + TextIndent * 2, CaptionMonthTextSize.Height + TextIndent * 2);
		}
		protected override void CalcElementsSize() {
			base.CalcElementsSize();
			CaptionMonthTextSize = CalcCaptionTextSize();
			CaptionYearTextSize = CalcCaptionYearTextSize();
			CaptionMonthBounds = new Rectangle(CaptionMonthBounds.Location, CalcCaptionMonthSize());
			CaptionYearBounds = new Rectangle(CaptionYearBounds.Location, CalcCaptionYearSize());
		}
		protected internal virtual string HeaderFormatString { get { return "MMMM, dd"; } }
		public string MonthCaption { get; set; }
		public string YearCaption { get; set; }
		protected virtual Rectangle CalcCaptionYearBounds() {
			return new Rectangle(HeaderBounds.X, CaptionMonthBounds.Bottom, CaptionYearBounds.Width, CaptionYearBounds.Height);
		}
		protected virtual Rectangle CalcCaptionMonthBounds() {
			return new Rectangle(HeaderBounds.X, HeaderBounds.Y, CaptionMonthBounds.Width, CaptionMonthBounds.Height);
		}
		protected override Rectangle CalcHeaderBounds() {
			return new Rectangle(CalendarBounds.Location, HeaderBounds.Size);
		}
	}
	public class VistaCalendarHeaderViewInfo : CalendarHeaderViewInfoBase {
		public VistaCalendarHeaderViewInfo(CalendarViewInfoBase viewInfo) : base(viewInfo) { }
		protected internal override bool UpdateVisualInfo(CalendarHitInfo info) {
			if(info.HitObject == TodayButton) {
				CheckButtonState(CalendarHitInfoType.CurrentDate, TodayButton);
				return true;
			}
			else if(info.HitObject == CaptionButton) {
				CheckButtonState(CalendarHitInfoType.Caption, CaptionButton);
				return true;
			}
			else if(info.HitObject == BackwardArrow) {
				CheckButtonState(CalendarHitInfoType.LeftArrow, BackwardArrow);
				return true;
			}
			else if(info.HitObject == ForwardArrow) {
				CheckButtonState(CalendarHitInfoType.RightArrow, ForwardArrow);
				return true;
			}
			else if(info.HitObject == BackwardArrow2) {
				CheckButtonState(CalendarHitInfoType.LeftArrow2, BackwardArrow2);
				return true;
			}
			else if(info.HitObject == ForwardArrow2) {
				CheckButtonState(CalendarHitInfoType.RightArrow2, ForwardArrow2);
				return true;
			}
			return false;
		}
		protected internal override void OnStateChanging(DateEditCalendarViewType prevView, DateEditCalendarViewType currView) {
		}
		protected internal override void OnStateChanged(DateEditCalendarViewType prevView, DateEditCalendarViewType currView) {
			base.OnStateChanged(prevView, currView);
			CaptionButton.Opacity = 0.0f;
		}
		protected internal override void UpdateContentAnimationParams() {
			CaptionButton.Opacity = ViewInfo.AnimatedContentProgress;
		}
		CalendarNavigationButtonViewInfo backwardArrow;
		public CalendarNavigationButtonViewInfo BackwardArrow {
			get {
				if(backwardArrow == null) {
					backwardArrow = CreateNavigationButton(CalendarNavigationButtonPredefines.Left);
					backwardArrow.Click += Handler.OnBackwardArrowClick;
				}
				return backwardArrow;
			}
		}
		CalendarNavigationButtonViewInfo backwardArrow2;
		public CalendarNavigationButtonViewInfo BackwardArrow2 {
			get {
				if(backwardArrow2 == null) {
					backwardArrow2 = CreateNavigationButton(CalendarNavigationButtonPredefines.Left);
					backwardArrow2.Click += Handler.OnBackwardArrow2Click;
				}
				return backwardArrow2;
			}
		}
		protected CalendarControl DateEditCalendar { get { return (CalendarControl)Calendar; } }
		protected VistaCalendarControlHandler Handler { get { return (VistaCalendarControlHandler)DateEditCalendar.Handler; } }
		CalendarNavigationButtonViewInfo forwardArrow;
		public CalendarNavigationButtonViewInfo ForwardArrow {
			get {
				if(forwardArrow == null) {
					forwardArrow = CreateNavigationButton(CalendarNavigationButtonPredefines.Right);
					forwardArrow.Click += Handler.OnForwardArrowClick;
				}
				return forwardArrow;
			}
		}
		CalendarNavigationButtonViewInfo forwardArrow2;
		public CalendarNavigationButtonViewInfo ForwardArrow2 {
			get {
				if(forwardArrow2 == null) {
					forwardArrow2 = CreateNavigationButton(CalendarNavigationButtonPredefines.Right);
					forwardArrow2.Click += Handler.OnForwardArrow2Click;
				}
				return forwardArrow2;
			}
		}
		CalendarNavigationButtonViewInfo todayButton;
		public CalendarNavigationButtonViewInfo TodayButton {
			get {
				if(todayButton == null) {
					todayButton = CreateTodayButton();
					todayButton.Click += Handler.OnTodayCaptionButtonClick;
				}
				return todayButton;
			}
		}
		protected virtual CalendarNavigationButtonViewInfo CreateTodayButton() {
			CalendarNavigationButtonViewInfo res = CreateNavigationButton(CalendarNavigationButtonPredefines.Text);
			res.Text = GetTodayCaption();
			return res;
		}
		CalendarNavigationButtonViewInfo captionButton;
		public CalendarNavigationButtonViewInfo CaptionButton {
			get {
				if(captionButton == null) {
					captionButton = CreateCaptionButton();
					captionButton.Click += Handler.OnCaptionButtonClick;
				}
				return captionButton;
			}
		}
		protected virtual CalendarNavigationButtonViewInfo CreateCaptionButton() {
			CalendarNavigationButtonViewInfo res = CreateNavigationButton(CalendarNavigationButtonPredefines.Text);
			res.Text = GetCurrentDateCaption();
			return res;
		}
		protected virtual int ArrowHorizontalIndent { get { return 20; } }
		public override void CalcHitInfo(CalendarHitInfo hitInfo) {
			base.CalcHitInfo(hitInfo);
			if(hitInfo.ContainsSet(TodayButton.Bounds, CalendarHitInfoType.CurrentDate)) {
				hitInfo.HitObject = TodayButton;
			}
			else if(hitInfo.ContainsSet(CaptionButton.Bounds, CalendarHitInfoType.Caption)) {
				hitInfo.HitObject = CaptionButton;
			}
			else if(ShowMonthNavigationButtons && hitInfo.ContainsSet(BackwardArrow.Bounds, CalendarHitInfoType.LeftArrow)) { 
				hitInfo.HitObject = BackwardArrow;
			}
			else if(ShowMonthNavigationButtons && hitInfo.ContainsSet(ForwardArrow.Bounds, CalendarHitInfoType.RightArrow)) {
				hitInfo.HitObject = ForwardArrow;
			}
			else if(ShowYearNavigationButtons && Calendar.View == DateEditCalendarViewType.MonthInfo && hitInfo.ContainsSet(BackwardArrow2.Bounds, CalendarHitInfoType.LeftArrow2)) {
				hitInfo.HitObject = BackwardArrow2;
			}
			else if(ShowYearNavigationButtons && Calendar.View == DateEditCalendarViewType.MonthInfo && hitInfo.ContainsSet(ForwardArrow2.Bounds, CalendarHitInfoType.RightArrow2)) {
				hitInfo.HitObject = ForwardArrow2;
			}
		}
		public override void UpdateContent() {
			base.UpdateContent();
			TodayButton.Text = GetTodayCaption();
			CaptionButton.Text = GetCurrentDateCaption();
			UpdateSkinNavigationButton(TodayButton);
			UpdateSkinNavigationButton(CaptionButton);
		}
		public new CalendarControl Calendar { get { return (CalendarControl)ViewInfo.Calendar; } }
		protected virtual string GetTodayCaption() {
			return Calendar.GetTodayDate().ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern, CultureInfo.CurrentCulture);
		}
		protected bool IsLocalizedDateTimeValid(DateTime dt) {
			return dt >= CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime && dt <= CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime;
		}
		protected virtual string GetCurrentDateCaption() {
			if(Calendar.DateTime < CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime)
				return "";
			if(Calendar.View == DateEditCalendarViewType.MonthInfo) {
				return Calendar.DateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern, CultureInfo.CurrentCulture);
			}
			if(Calendar.View == DateEditCalendarViewType.YearInfo || Calendar.View == DateEditCalendarViewType.QuarterInfo) {
				if(!IsLocalizedDateTimeValid(ViewInfo.DateTime))
					return "";
				return ViewInfo.DateTime.ToString("yyyy", CultureInfo.CurrentCulture);
			}
			if(Calendar.View == DateEditCalendarViewType.YearsInfo) {
				int year = Math.Max(1, (ViewInfo.DateTime.Year / 10) * 10);
				DateTime dt = new DateTime(year, 1, 1);
				DateTime dt2 = new DateTime(((ViewInfo.DateTime.Year / 10) * 10) + 9, 1, 1);
				if(!IsLocalizedDateTimeValid(dt) || !IsLocalizedDateTimeValid(dt2))
					return "";
				return dt.ToString("yyyy", CultureInfo.CurrentCulture) + "-" + dt2.ToString("yyyy", CultureInfo.CurrentCulture);
			}
			if(Calendar.View == DateEditCalendarViewType.YearsGroupInfo) {
				int year = Math.Max(1, (ViewInfo.DateTime.Year / 100) * 100);
				DateTime dt = new DateTime(year, 1, 1);
				DateTime dt2 = new DateTime(((ViewInfo.DateTime.Year / 100) * 100) + 99, 1, 1);
				if(!IsLocalizedDateTimeValid(dt) || !IsLocalizedDateTimeValid(dt2))
					return "";
				return dt.ToString("yyyy", CultureInfo.CurrentCulture) + "-" + dt2.ToString("yyyy", CultureInfo.CurrentCulture);
			}
			return string.Empty;
		}
		protected override Size CalcBestSizeCore() {
			TodayButton.CalcBestSize();
			CaptionButton.CalcBestSize();
			UpdateSkinInfo();
			BackwardArrow.Bounds = new Rectangle(BackwardArrow.Bounds.Location, CalcBackwardArrowSize());
			ForwardArrow.Bounds = new Rectangle(ForwardArrow.Bounds.Location, CalcForwardArrowSize());
			BackwardArrow2.Bounds = new Rectangle(BackwardArrow2.Bounds.Location, CalcBackwardArrow2Size());
			ForwardArrow2.Bounds = new Rectangle(ForwardArrow2.Bounds.Location, CalcForwardArrow2Size());
			return CalcContentSize(CaptionButton.Bounds.Size, TodayButton.Bounds.Size, BackwardArrow.Bounds.Size, ForwardArrow.Bounds.Size, BackwardArrow2.Bounds.Size, ForwardArrow2.Bounds.Size);
		}
		protected virtual Size CalcContentSize(Size captionSize, Size todayCaptionSize, Size backArrowSize, Size forwardArrowSize, Size backArrow2Size, Size forwardArrow2Size) {
			Size res = Size.Empty;
			res.Width = Math.Max(captionSize.Width, todayCaptionSize.Width);
			res.Height = captionSize.Height + CaptionVerticalIndent + todayCaptionSize.Height;
			if(ShowMonthNavigationButtons) {
				res.Width += backArrowSize.Width + forwardArrowSize.Width;
				res.Height = Math.Max(res.Height, forwardArrowSize.Height);
			}
			if(ShowYearNavigationButtons) {
				res.Width += backArrow2Size.Width + forwardArrow2Size.Width;
				res.Height = Math.Max(res.Height, forwardArrow2Size.Height);
			}
			if(ShowYearNavigationButtons || ShowMonthNavigationButtons)
				res.Width += ArrowHorizontalIndent * 2;
			return res;
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			TodayButton.CalcViewInfo(CalcTodayCaptionBounds());
			CaptionButton.CalcViewInfo(CalcCaptionBounds());
			UpdateSkinInfo();
			BackwardArrow2.CalcViewInfo(CalcBackwardArrow2Bounds());
			ForwardArrow2.CalcViewInfo(CalcForwardArrow2Bounds());
			BackwardArrow.CalcViewInfo(CalcBackwardArrowBounds());
			ForwardArrow.CalcViewInfo(CalcForwardArrowBounds());
		}
		protected virtual void UpdateSkinInfo() {
			BackwardArrow.SkinElement = EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinCalendarNavigationButton];
			ForwardArrow.SkinElement = EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinCalendarNavigationButton];
			ForwardArrow.SkinElementRotateFlip = RotateFlipType.RotateNoneFlipX;
			BackwardArrow2.SkinElement = EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinCalendarNavigationButton];
			ForwardArrow2.SkinElement = EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinCalendarNavigationButton];
			ForwardArrow2.SkinElementRotateFlip = RotateFlipType.RotateNoneFlipX;
		}
		protected virtual int CaptionVerticalIndent { get { return 2; } }
		protected virtual Rectangle CalcForwardArrow2Bounds() {
			if(!ShowYearNavigationButtons)
				return Rectangle.Empty;
			return new Rectangle(ContentBounds.Right - ForwardArrow2.Bounds.Width,
				ContentBounds.Y + TodayButton.Bounds.Height + CaptionVerticalIndent + (TodayButton.Bounds.Height - ForwardArrow2.Bounds.Height) / 2,
				ForwardArrow2.Bounds.Width, ForwardArrow2.Bounds.Height);
		}
		protected virtual Rectangle CalcBackwardArrow2Bounds() {
			if(!ShowYearNavigationButtons)
				return Rectangle.Empty;
			return new Rectangle(ContentBounds.X,
				ContentBounds.Y + TodayButton.Bounds.Height + CaptionVerticalIndent + (CaptionButton.Bounds.Height - BackwardArrow2.Bounds.Height) / 2,
				BackwardArrow2.Bounds.Width, BackwardArrow2.Bounds.Height);
		}
		protected internal override bool ShowYearNavigationButtons {
			get { return Calendar.ShowYearNavigationButtons == DefaultBoolean.True; }
		}
		protected internal override bool ShowMonthNavigationButtons {
			get { return Calendar.ShowMonthNavigationButtons != DefaultBoolean.False; }
		}
		protected virtual Rectangle CalcForwardArrowBounds() {
			if(!ShowMonthNavigationButtons)
				return Rectangle.Empty;
			int x = ShowYearNavigationButtons ? ForwardArrow2.Bounds.X - ForwardArrow.Bounds.Width : ContentBounds.Right - ForwardArrow.Bounds.Width; ;
			return new Rectangle(x, 
				ContentBounds.Y + TodayButton.Bounds.Height + CaptionVerticalIndent + (TodayButton.Bounds.Height - ForwardArrow.Bounds.Height) / 2, 
				ForwardArrow.Bounds.Width, ForwardArrow.Bounds.Height);
		}
		protected virtual Rectangle CalcBackwardArrowBounds() {
			if(!ShowMonthNavigationButtons)
				return Rectangle.Empty;
			int x = ShowYearNavigationButtons? BackwardArrow2.Bounds.Right: ContentBounds.X;
			return new Rectangle(x,
				ContentBounds.Y + TodayButton.Bounds.Height + CaptionVerticalIndent + (CaptionButton.Bounds.Height - BackwardArrow.Bounds.Height) / 2,
				BackwardArrow.Bounds.Width, BackwardArrow.Bounds.Height);
		}
		protected virtual Rectangle CalcCaptionBounds() {
			return new Rectangle(ContentBounds.X + (ContentBounds.Width - CaptionButton.Bounds.Width) / 2, 
				ContentBounds.Y + TodayButton.Bounds.Height + CaptionVerticalIndent, 
				CaptionButton.Bounds.Width, CaptionButton.Bounds.Height);
		}
		protected virtual Rectangle CalcTodayCaptionBounds() {
			return new Rectangle(ContentBounds.X + (ContentBounds.Width - TodayButton.Bounds.Width) / 2, 
				ContentBounds.Y,
				TodayButton.Bounds.Width, TodayButton.Bounds.Height);
		}
		protected virtual Size CalcForwardArrowSize() {
			return ForwardArrow.CalcBestSize();
		}
		protected virtual Size CalcBackwardArrowSize() {
			return BackwardArrow.CalcBestSize();
		}
		protected virtual Size CalcBackwardArrow2Size() {
			return BackwardArrow2.CalcBestSize();
		}
		protected virtual Size CalcForwardArrow2Size() {
			return ForwardArrow2.CalcBestSize();
		}
		protected virtual Size CalcCaptionTextSize() {
			return ViewInfo.PaintStyle.HeaderAppearance.CalcTextSize(ViewInfo.GInfo.Graphics, CaptionButton.Text, 0).ToSizeCeiling();
		}
		protected virtual Size CalcTodayCaptionTextSize() {
			return ViewInfo.PaintStyle.HeaderAppearance.CalcTextSize(ViewInfo.GInfo.Graphics, TodayButton.Text, 0).ToSizeCeiling();
		}
	}
	public class VistaCalendarFooterViewInfo : CalendarFooterViewInfo {
		public VistaCalendarFooterViewInfo(CalendarViewInfoBase viewInfo) : base(viewInfo) { }
		public override void CalcHitInfo(CalendarHitInfo hitInfo) {
			base.CalcHitInfo(hitInfo);
			if(hitInfo.ContainsSet(CancelButtonInfo.Bounds, CalendarHitInfoType.Cancel)) {
				hitInfo.HitObject = CancelButtonInfo;
			}
			else if(hitInfo.ContainsSet(OkButtonInfo.Bounds, CalendarHitInfoType.Ok)) {
				hitInfo.HitObject = OkButtonInfo;
			}
		}
		protected override Size CalcContentSize() {
			Size res = base.CalcContentSize();
			if(OkButtonInfo.Bounds.Width > 0) {
				res = AddButtonSize(res, OkButtonInfo);
			}
			if(CancelButtonInfo.Bounds.Width > 0) {
				res.Width += ButtonHorizontalIndent;
				res = AddButtonSize(res, CancelButtonInfo);
			}
			return res;
		}
		protected internal override bool UpdateVisualInfo(CalendarHitInfo info) {
			bool res = base.UpdateVisualInfo(info);
			if(res)
				return res;
			if(info.HitObject == OkButtonInfo) {
				CheckButtonState(CalendarHitInfoType.Ok, OkButtonInfo);
				return true;
			}
			if(info.HitObject == CancelButtonInfo) {
				CheckButtonState(CalendarHitInfoType.Cancel, CancelButtonInfo);
				return true;
			}
			return false;
		}
		string cancelButtonCaption;
		public string CancelButtonCaption { 
			get {
				if(string.IsNullOrEmpty(cancelButtonCaption))
					cancelButtonCaption = FormatString(Localizer.Active.GetLocalizedString(StringId.Cancel));
				return cancelButtonCaption; 
			} 
		}
		string okButtonCaption;
		public string OkButtonCaption { 
			get { 
				if(string.IsNullOrEmpty(okButtonCaption))
					okButtonCaption = FormatString(Localizer.Active.GetLocalizedString(StringId.OK));
				return okButtonCaption; 
			} 
		}
		protected new VistaCalendarControlHandler Handler { get { return (VistaCalendarControlHandler)base.Handler; } }
		EditorButton cancelButton;
		public EditorButton CancelButton {
			get {
				if(cancelButton == null) {
					cancelButton = CreateEditorButton();
					cancelButton.Click += Handler.OnCancelButtonClick;
				}
				return cancelButton;
			}
		}
		EditorButton okButton;
		public EditorButton OkButton {
			get {
				if(okButton == null) {
					okButton = CreateEditorButton();
					okButton.Click += Handler.OnOkButtonClick;
				}
				return okButton;
			}
		}
		EditorButtonObjectInfoArgs okButtonInfo;
		public EditorButtonObjectInfoArgs OkButtonInfo {
			get {
				if(okButtonInfo == null)
					okButtonInfo = CreateButtonInfo(OkButton);
				return okButtonInfo;
			}
		}
		EditorButtonObjectInfoArgs cancelButtonInfo;
		public EditorButtonObjectInfoArgs CancelButtonInfo {
			get {
				if(cancelButtonInfo == null)
					cancelButtonInfo = CreateButtonInfo(CancelButton);
				return cancelButtonInfo;
			}
		}
		public override void UpdateContent() {
			base.UpdateContent();
			OkButton.Caption = OkButtonCaption;
			CancelButton.Caption = CancelButtonCaption;
		}
		protected override Size CalcTodayButtonSize() {
			return Size.Empty;
		}
		protected override Rectangle CalcTodayButtonBounds() {
			return Rectangle.Empty;
		}
		public virtual bool ShowOkButton {
			get { return Calendar.IsPopupCalendar && Calendar.CalendarTimeEditing == DefaultBoolean.True; }
		}
		public virtual bool ShowCancelButton {
			get { return Calendar.IsPopupCalendar && Calendar.CalendarTimeEditing == DefaultBoolean.True; }
		}
		public override bool ShowTodayButton {
			get { return false; }
		}
		protected override void CalcButtonsBestSize() {
			base.CalcButtonsBestSize();
			UpdateButtonAppearance(OkButtonInfo);
			UpdateButtonAppearance(CancelButtonInfo);
			OkButtonInfo.Bounds = new Rectangle(OkButtonInfo.Bounds.Location, CalcOkButtonSize());
			CancelButtonInfo.Bounds = new Rectangle(CancelButtonInfo.Bounds.Location, CalcCancelButtonSize());
			int maxWidth = 0;
			int maxHeight = 0;
			if(ShowTodayButton) {
				maxWidth = Math.Max(MinButtonWidth, TodayButtonInfo.Bounds.Width);
				maxHeight = TodayButtonInfo.Bounds.Height;
			}
			if(ShowClearButton) {
				maxWidth = Math.Max(maxWidth, ClearButtonInfo.Bounds.Width);
				maxHeight = Math.Max(maxHeight, ClearButtonInfo.Bounds.Height);
			}
			if(ShowOkButton) {
				maxWidth = Math.Max(maxWidth, OkButtonInfo.Bounds.Width);
				maxHeight = Math.Max(maxHeight, OkButtonInfo.Bounds.Height);
			}
			if(ShowCancelButton) {
				maxWidth = Math.Max(maxWidth, CancelButtonInfo.Bounds.Width); 
				maxHeight = Math.Max(maxHeight, CancelButtonInfo.Bounds.Height);
			}
			if(ShowTodayButton)
				TodayButtonInfo.Bounds = new Rectangle(TodayButtonInfo.Bounds.X, TodayButtonInfo.Bounds.Y, maxWidth, maxHeight);
			if(ShowClearButton)
				ClearButtonInfo.Bounds = new Rectangle(ClearButtonInfo.Bounds.X, ClearButtonInfo.Bounds.Y, maxWidth, maxHeight);
			if(ShowOkButton)
				OkButtonInfo.Bounds = new Rectangle(OkButtonInfo.Bounds.X, OkButtonInfo.Bounds.Y, maxWidth, maxHeight);
			if(ShowCancelButton)
				CancelButtonInfo.Bounds = new Rectangle(CancelButtonInfo.Bounds.X, CancelButtonInfo.Bounds.Y, maxWidth, maxHeight);
		}
		protected override int CalcButtonsTotalHeight() {
			return Math.Max(TodayButtonInfo.Bounds.Height, Math.Max(ClearButtonInfo.Bounds.Height, Math.Max(OkButtonInfo.Bounds.Height, CancelButtonInfo.Bounds.Height)));
		}
		protected override int CalcButtonsTotalWidth() {
			int res = TodayButtonInfo.Bounds.Width;
			if(ShowClearButton)
				res += ButtonHorizontalIndent + ClearButtonInfo.Bounds.Width;
			if(ShowOkButton)
				res += ButtonHorizontalIndent + OkButtonInfo.Bounds.Width;
			if(ShowCancelButton)
				res += ButtonHorizontalIndent + CancelButtonInfo.Bounds.Width;
			return res;
		}
		protected virtual Size CalcCancelButtonSize() {
			return CalcButtonSize(CancelButtonInfo);
		}
		protected virtual Size CalcOkButtonSize() {
			return CalcButtonSize(OkButtonInfo);
		}
		protected override Rectangle CalcClearButtonBounds() {
			if(!ShowClearButton)
				return Rectangle.Empty;
			return new Rectangle(ContentBounds.X + (ContentBounds.Width - ClearButtonInfo.Bounds.Width) / 2, ContentBounds.Y + (ContentBounds.Height - ClearButtonInfo.Bounds.Height) / 2, ClearButtonInfo.Bounds.Width, ClearButtonInfo.Bounds.Height);
		}
		protected virtual Rectangle CalcOkButtonBounds() {
			if(!ShowOkButton)
				return Rectangle.Empty;
			return new Rectangle(ContentBounds.X, ContentBounds.Y + (ContentBounds.Height - OkButtonInfo.Bounds.Height) / 2, OkButtonInfo.Bounds.Width, OkButtonInfo.Bounds.Height);
		}
		protected virtual Rectangle CalcCancelButtonBounds() {
			if(!ShowCancelButton)
				return Rectangle.Empty;
			return new Rectangle(ContentBounds.Right - CancelButtonInfo.Bounds.Width, ContentBounds.Y + (ContentBounds.Height - CancelButtonInfo.Bounds.Height) / 2, CancelButtonInfo.Bounds.Width, CancelButtonInfo.Bounds.Height);
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			UpdateButtonAppearance(OkButtonInfo);
			UpdateButtonAppearance(CancelButtonInfo);
			OkButtonInfo.Bounds = CalcOkButtonBounds();
			CancelButtonInfo.Bounds = CalcCancelButtonBounds();
			OkButtonInfo.Bounds = OkButtonInfo.Bounds;
			CancelButtonInfo.Bounds = CancelButtonInfo.Bounds;
		}
	}
	public class VistaCalendarRightAreaViewInfo : CalendarAreaViewInfoBase {
		public VistaCalendarRightAreaViewInfo(CalendarViewInfoBase viewInfo) : base(viewInfo) {
			SecArrow = new PointF[2];
			MinArrow = new PointF[2];
			HourArrow = new PointF[2];
		}
		public DateTime DateTime { get { return Calendar.Handler.SelectedDate; } }
		protected bool IsRightToLeft { get { return WindowsFormsSettings.GetIsRightToLeft(Calendar); } }
		public virtual Rectangle ClockTextBounds { get; private set; }
		public virtual PointF ClockOrigin { get; private set; }
		public virtual PointF[] SecArrow { get; private set; }
		public virtual PointF[] MinArrow { get; private set; }
		public virtual PointF[] HourArrow { get; private set; }
		public virtual Rectangle DateContentRect { get; private set; }
		public virtual Rectangle ClockBounds { get; private set; }
		protected internal virtual SkinElementInfo GetClockFaceInfo() {
			return new SkinElementInfo(EditorsSkins.GetSkin(Calendar.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinDateEditClockFace], ClockBounds);
		}
		protected internal virtual SkinElementInfo GetClockGlassInfo() {
			return new SkinElementInfo(EditorsSkins.GetSkin(Calendar.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinDateEditClockGlass], ClockBounds);
		}
		protected virtual PointF[] CalcArrow(float angle, float length, float offset) {
			SizeF sz = new SizeF((float)Math.Cos(Math.PI * 0.5f - angle), (float)Math.Sin(Math.PI * 0.5f - angle));
			PointF[] pt = new PointF[2];
			pt[0].X = ClockOrigin.X - offset * sz.Width;
			pt[0].Y = ClockOrigin.Y + offset * sz.Height;
			pt[1].X = pt[0].X + length * sz.Width;
			pt[1].Y = pt[0].Y - length * sz.Height;
			return pt;
		}
		protected virtual float CalcAngle(float val) {
			return val * 2.0f * (float)Math.PI / 60;
		}
		protected virtual float CalcHourAngle(float val) {
			return val * 2.0f * (float)Math.PI / 12;
		}
		protected virtual float CalcAngleOffset(float val) {
			return val * 6.0f * (float)Math.PI / 180 / 60;
		}
		protected virtual float CalcHourAngleOffset(float val) {
			return val * 30.0f * (float)Math.PI / 180 / 60;
		}
		protected virtual float SecArrowLength { get { return 55.0f; } }
		protected virtual float MinArrowLength { get { return 48.0f; } }
		protected virtual float HourArrowLength { get { return 35.0f; } }
		protected virtual PointF[] CalcSecArrow(DateTime dt) {
			float angle = CalcAngle(dt.Second);
			return CalcArrow(angle, SecArrowLength, 8.0f);
		}
		protected virtual PointF[] CalcMinArrow(DateTime dt) {
			float angle = CalcAngle(dt.Minute);
			angle += CalcAngleOffset(dt.Second);
			return CalcArrow(angle, MinArrowLength, 2.0f);
		}
		protected virtual PointF[] CalcHourArrow(DateTime dt) {
			int hour = dt.Hour % 12;
			float angle = CalcHourAngle(hour);
			angle += CalcHourAngleOffset(dt.Minute);
			return CalcArrow(angle, HourArrowLength, 2.0f);
		}
		protected internal virtual void CalcClockPoints() {
			ClockOrigin = new PointF(ClockBounds.X + ClockBounds.Width / 2, ClockBounds.Y + ClockBounds.Height / 2);
			SecArrow = CalcSecArrow(DateTime);
			MinArrow = CalcMinArrow(DateTime);
			HourArrow = CalcHourArrow(DateTime);
		}
		protected virtual int GetClockWidth() {
			SkinElementInfo info = GetClockFaceInfo();
			if(info.Element == null || info.Element.Image == null) return 0;
			return info.Element.Image.Image.Width;
		}
		protected virtual Rectangle CalcClockBounds() {
			return new Rectangle(ContentBounds.X, ContentBounds.Y + (ContentBounds.Height - ClockBounds.Height) / 2, ClockBounds.Width, ClockBounds.Height);
		}
		public override void UpdateContent() {
		}
		public virtual bool ShowTime { get { return Calendar.CalendarTimeEditing == DefaultBoolean.True; } }
		protected virtual int Clock2TimeIndent { get { return 8; } }
		public Size TimeEditSize { get; protected set; }
		protected override Size CalcBestSizeCore() {
			if(!ShowTime)
				return Size.Empty;
			ClockBounds = new Rectangle(ClockBounds.Location, CalcClockSize());
			TimeEditSize = CalcTimeSize();
			return new Size(Math.Max(ClockBounds.Width, TimeEditSize.Width), ClockBounds.Height + TimeEditSize.Height + Clock2TimeIndent);
		}
		protected CalendarControl DateEditCalendar { get { return (CalendarControl)Calendar; } }
		protected virtual Size CalcTimeSize() {
			return new Size(100, DateEditCalendar.TimeEdit.CalcBestSize().Height);
		}
		public bool ShowClock { get { return Calendar.CalendarTimeEditing == DefaultBoolean.True; } }
		protected virtual Size CalcClockSize() {
			if(!ShowClock)
				return Size.Empty;
			SkinElementInfo info = GetClockFaceInfo();
			if(info.Element == null || info.Element.Image == null) 
				return Size.Empty;
			return info.Element.Image.Image.Size;
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			if(!ShowClock)
				return;
			ClockBounds = CalcClockBounds();
			CalcClockPoints();
			UpdateTimeEditProperties();
		}
		protected virtual void UpdateTimeEditProperties() {
			if(!ShowTime)
				return;
			ViewInfo.LockCalcBestSize = true;
			try {
				if(!DateEditCalendar.Controls.Contains(DateEditCalendar.TimeEdit))
					DateEditCalendar.Controls.Add(DateEditCalendar.TimeEdit);
				DateEditCalendar.TimeEdit.Bounds = CalcTimeEditBounds();
				DateEditCalendar.TimeEdit.ReadOnly = Calendar.ReadOnly;
			}
			finally {
				ViewInfo.LockCalcBestSize = false;
			}
		}
		protected virtual Rectangle CalcTimeEditBounds() {
			if(ViewInfo.ActualRowCount == 1)
				return new Rectangle(ContentBounds.X + (ContentBounds.Width - TimeEditSize.Width) / 2, ViewInfo.Footer.ContentBounds.Y + (ViewInfo.Footer.ContentBounds.Height - TimeEditSize.Height) / 2, TimeEditSize.Width, TimeEditSize.Height);
			return new Rectangle(ContentBounds.X + (ContentBounds.Width - TimeEditSize.Width) / 2, ClockBounds.Bottom + Clock2TimeIndent, TimeEditSize.Width, TimeEditSize.Height);
		}
		protected internal virtual void UpdateClock() {
			CalcClockPoints();
		}
	}
	public class VistaCalendarViewInfo : CalendarViewInfo {
		public VistaCalendarViewInfo(CalendarControlBase owner) : base(owner) { }
		protected override CalendarHeaderViewInfoBase CreateHeaderInfo() {
			return new VistaCalendarHeaderViewInfo(this);
		}
		protected override CalendarFooterViewInfoBase CreateFooterInfo() {
			return new VistaCalendarFooterViewInfo(this);
		}
		protected internal override SkinPaddingEdges GetCellPaddingCore() {
			return new SkinPaddingEdges(7, 2, 7, 2);
		}
		protected override bool ShowCalendarHeader(int index) {
			return Calendar.ShowMonthHeaders;
		}
		protected override CalendarPaintStyle CreatePaintStyle() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new VistaCalendarPaintStyleSkin(this);
			return new VistaCalendarPaintStyle(this);
		}
		public new VistaCalendarHeaderViewInfo Header { get { return (VistaCalendarHeaderViewInfo)base.Header; } }
	}
	public class CalendarContextButtonCustomizeEventArgs {
		public CalendarCellStyle Cell { get; internal set; }
		public ContextItem Item { get; internal set; }
	}
	public delegate void CalendarContextButtonCustomizeEventHandler(object sender, CalendarContextButtonCustomizeEventArgs e);
	public abstract class CalendarControlBase : BaseStyleControl, ICalendarAppearancesOwner, IDateTimeOwner, IContextItemCollectionOptionsOwner, IContextItemCollectionOwner, IDatesCollectionOwner {
		private static readonly object selectionChanged = new object();
		private static readonly object customDrawDayNumberCell = new object();
		private static readonly object disabledCalendarDateCell = new object();
		private static readonly object specialCalendarDateCell = new object();
		private static readonly object dateTimeChanged = new object();
		private static readonly object dateTimeCommit = new object();
		private static readonly object customWeekNumber = new object();
		private static readonly object parseEditValue = new object();
		private static readonly object formatEditValue = new object();
		private static readonly object editValueChanged = new object();
		private static readonly object contextButtonClick = new object();
		private static readonly object customContextButtonToolTip = new object();
		private static readonly object contextButtonValueChanged = new object();
		private static readonly object contextButtonCustomize = new object();
		protected DateTimeFormatInfo fFormat;
		int lockStartDateCounter;
		DateEditCalendarViewType view;
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			LayoutChanged();
		}
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
		private DefaultBoolean showYearNavigationButton = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowYearNavigationButtons {
			get { return showYearNavigationButton; }
			set {
				if(ShowYearNavigationButtons == value)
					return;
				showYearNavigationButton = value;
				OnShowYearNavigationButtonsChanged();
			}
		}
		protected virtual void OnShowYearNavigationButtonsChanged() {
			OnPropertiesChanged();
		}
		ContextItemCollection contextButtons;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContextItemCollection ContextButtons {
			get {
				if(contextButtons == null) {
					contextButtons = CreateContextButtonsCollection();
					contextButtons.Options = ContextButtonOptions;
				}
				return contextButtons;
			}
		}
		ContextItemCollectionOptions contextButtonOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public ContextItemCollectionOptions ContextButtonOptions {
			get {
				if(contextButtonOptions == null) {
					contextButtonOptions = CreateContextButtonOptions();
				}
				return contextButtonOptions;
			}
		}
		protected virtual ContextItemCollectionOptions CreateContextButtonOptions() {
			return new ContextItemCollectionOptions(this);
		}
		protected virtual ContextItemCollection CreateContextButtonsCollection() {
			return new ContextItemCollection(this);
		}
		void IContextItemCollectionOwner.OnCollectionChanged() {
			ViewInfo.ResetContextButtons();
			OnPropertiesChanged();
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			if(propertyName == "Checked" || propertyName == "Value" || propertyName == "Rating")
				RaiseContextButtonValueChanged(new ContextButtonValueEventArgs(item, newValue));
			if(propertyName == "Visibility" || propertyName == "Value") {
				Invalidate();
				Update();
				return;
			}
			OnPropertiesChanged();
		}
		bool IContextItemCollectionOwner.IsDesignMode { get { return IsDesignMode; } }
		bool IContextItemCollectionOwner.IsRightToLeft {
			get {
				return WindowsFormsSettings.GetIsRightToLeft(this);
			}
		}
		void IContextItemCollectionOptionsOwner.OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			OnPropertiesChanged();
		}
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType { get { return ContextAnimationType.OpacityAnimation; } }
		private DateTime todayDate = DateTime.MinValue;
		bool ShouldSerializeTodayDate() { return TodayDate != DateTime.MinValue; }
		void ResetTodayDate() { TodayDate = DateTime.MinValue; }
		public DateTime TodayDate {
			get { return todayDate; }
			set {
				if(TodayDate == value)
					return;
				todayDate = value;
				OnTodayDateChanged();
			}
		}
		protected virtual void OnTodayDateChanged() { 
			OnPropertiesChanged();
		}
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		protected internal void RaiseContextButtonClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		public event ContextButtonToolTipEventHandler CustomContextButtonToolTip {
			add { Events.AddHandler(customContextButtonToolTip, value); }
			remove { Events.RemoveHandler(customContextButtonToolTip, value); }
		}
		protected internal void RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
			ContextButtonToolTipEventHandler handler = Events[customContextButtonToolTip] as ContextButtonToolTipEventHandler;
			if(handler != null)
				handler(this, e);
		}
		public event ContextButtonValueChangedEventHandler ContextButtonValueChanged {
			add { Events.AddHandler(contextButtonValueChanged, value); }
			remove { Events.RemoveHandler(contextButtonValueChanged, value); }
		}
		public event CalendarContextButtonCustomizeEventHandler ContextButtonCustomize {
			add { Events.AddHandler(contextButtonCustomize, value); }
			remove { Events.RemoveHandler(contextButtonCustomize, value); }
		}
		protected internal void RaiseContextButtonCustomize(CalendarContextButtonCustomizeEventArgs e) {
			CalendarContextButtonCustomizeEventHandler handler = Events[contextButtonCustomize] as CalendarContextButtonCustomizeEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseContextButtonValueChanged(ContextButtonValueEventArgs e) {
			ContextButtonValueChangedEventHandler handler = Events[contextButtonValueChanged] as ContextButtonValueChangedEventHandler;
			if(handler != null)
				handler(this, e);
		}
		private TextCaseMode weekDayCase = TextCaseMode.Default;
		[DefaultValue(TextCaseMode.Default)]
		public TextCaseMode CaseWeekDayAbbreviations {
			get { return weekDayCase; }
			set {
				if(CaseWeekDayAbbreviations == value)
					return;
				weekDayCase = value;
				OnWeekDayCaseChanged();
			}
		}
		protected virtual void OnWeekDayCaseChanged() {
			OnPropertiesChanged();
		}
		private TextCaseMode monthNameCase = TextCaseMode.Default;
		[DefaultValue(TextCaseMode.Default)]
		public TextCaseMode CaseMonthNames {
			get { return monthNameCase; }
			set {
				if(CaseMonthNames == value)
					return;
				monthNameCase = value;
				OnMonthNameCaseChanged();
			}
		}
		protected virtual void OnMonthNameCaseChanged() {
			OnPropertiesChanged();
		}
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
				Invalidate();
				Update();
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
		protected override Padding DefaultPadding {
			get { return new Padding(-1); }
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			LayoutChanged();
		}
		public CalendarControlBase() {
			AutoSize = true;
			SetStyle(ControlStyles.Selectable, true);
			AllowAnimatedContentChange = true;
			this.editValue = GetTodayDate();
			this.firstDayOfWeek = DateFormat.FirstDayOfWeek;
			AllowClickInactiveDays = true;
		}
		[DefaultValue(true)]
		public bool AllowClickInactiveDays { get; set; }
		bool inDefaultSize = false;
		protected override Size DefaultSize {
			get {
				if(inDefaultSize)
					return new Size(128, 128);
				try {
					inDefaultSize = true;
					return CalcBestSize();
				}
				finally {
					inDefaultSize = false;
				}
			}
		}
		[Obsolete, Browsable(false)]
		public int FullButtonsHeight { get { return 0; } }
		private CalendarInactiveDaysVisibility drawInactiveDays = CalendarInactiveDaysVisibility.Visible;
		[DefaultValue(CalendarInactiveDaysVisibility.Visible)]
		public CalendarInactiveDaysVisibility InactiveDaysVisibility {
			get { return drawInactiveDays; }
			set {
				if(InactiveDaysVisibility == value)
					return;
				drawInactiveDays = value;
				OnPropertiesChanged();
			}
		}
		private CalendarSelectionBehavior selectionBehavior = CalendarSelectionBehavior.Simple;
		[DefaultValue(CalendarSelectionBehavior.Simple)]
		public CalendarSelectionBehavior SelectionBehavior {
			get { return selectionBehavior; }
			set {
				if(SelectionBehavior == value)
					return;
				selectionBehavior = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowAnimatedContentChange { get; set; }
		private bool showCalendarHeader = false;
		public bool ShowMonthHeaders {
			get { return showCalendarHeader; }
			set {
				if(ShowMonthHeaders == value)
					return;
				showCalendarHeader = value;
				OnPropertiesChanged();
			}
		}
		protected int LockVisualCount { get; private set; }
		protected internal bool IsLockVisual { get { return LockVisualCount > 0; } }
		protected internal void LockVisual() {
			LockVisualCount++;
		}
		protected internal void InvalidateVisual() {
			if(IsLockVisual)
				return;
			Invalidate();
		}
		protected internal void UpdateVisual() {
			if(IsLockVisual)
				return;
			Invalidate();
			Update();
		}
		protected internal void UnlockVisual() {
			if(LockVisualCount > 0)
				LockVisualCount--;
			InvalidateVisual();
		}
		protected internal void CancelVisual() {
			if(LockVisualCount > 0)
				LockVisualCount--;
		}
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		protected virtual void Init() {
			AddSelection(new DateRange(DateTime));
			this.fFormat = null;
			this.readOnly = false;
			this.lockStartDateCounter = 0;
			this.view = GetStartView();
			SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
			UpdateStartDate();
			LayoutChangedCore(false);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		public virtual DateTime GetStartDate() {
			CheckViewInfo((Graphics)null);
			CalendarCellViewInfo cell = ViewInfo.GetFirstActiveVisibleCellInfo();
			return cell == null? MinValue: cell.Date;
		}
		public virtual DateTime GetEndDate() { 
			CheckViewInfo((Graphics)null);
			CalendarCellViewInfo cell = ViewInfo.GetLastActiveVisibleCellInfo();
			return cell == null? MinValue: cell.Date;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		protected internal virtual VistaCalendarViewStyle GetVistaCalendarViewStyle() {
			if(VistaCalendarViewStyle == VistaCalendarViewStyle.Default)
				return VistaCalendarViewStyle.MonthView | VistaCalendarViewStyle.YearView | VistaCalendarViewStyle.YearsGroupView | VistaCalendarViewStyle.CenturyView;
			return VistaCalendarViewStyle;
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(true)]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}
		protected override void OnAutoSizeChanged(EventArgs e) {
			base.OnAutoSizeChanged(e);
		}
		public override Size GetPreferredSize(Size proposedSize) {
			Size res = base.GetPreferredSize(proposedSize);
			Size bestSize = ViewInfo.CalcBestFit(null);
			return bestSize;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetHandler() { this.handler = null; }
		protected internal virtual bool ShouldDrawCellFocusRect { get { return true; } }
		protected virtual DateTime GetStartSelectionByState(DateTime date) {
			if(View == DateEditCalendarViewType.MonthInfo) return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
			if(View == DateEditCalendarViewType.YearInfo) return new DateTime(date.Year, date.Month, 1, 0, 0, 0);
			if(View == DateEditCalendarViewType.YearsInfo) return new DateTime(date.Year, 1, 1, 0, 0, 0);
			int year = (date.Year / 10) * 10;
			if(year == 0) year = 1;
			return new DateTime(year, 1, 1, 0, 0, 0);
		}
		protected virtual DateTime GetEndSelectionByState(DateTime dt) {
			if(View == DateEditCalendarViewType.MonthInfo) return new DateTime(dt.Year, dt.Month, dt.Day).AddDays(1);
			if(View == DateEditCalendarViewType.YearInfo) return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month)).AddDays(1);
			if(View == DateEditCalendarViewType.YearsInfo) return new DateTime(dt.Year, 12, DateTime.DaysInMonth(dt.Year, dt.Month)).AddDays(1);
			return new DateTime((dt.Year / 10) * 10 + 9, 12, DateTime.DaysInMonth(dt.Year, dt.Month)).AddDays(1);
		}
		CalendarSelectionManager selectionManager;
		protected internal CalendarSelectionManager SelectionManager {
			get {
				if(selectionManager == null)
					selectionManager = CreateSelectionManager();
				return selectionManager;
			}
		}
		protected virtual CalendarSelectionManager CreateSelectionManager() {
			return new CalendarSelectionManager(this);
		}
		public virtual void SetSelection(DateTime date) {
			SetSelection(new DateRange(date));
		}
		public virtual void SetSelection(DateRange range) {
			SelectionManager.SetSelection(range);
		}
		public virtual void SetSelection(DateTime start, DateTime end) {
			SetSelection(new DateRange(start, end));
		}
		public virtual void AddSelection(DateTime date) {
			AddSelection(new DateRange(date));
		}
		public virtual void AddSelection(DateTime start, DateTime end) {
			AddSelection(new DateRange(start, end));
		}
		public virtual void AddSelection(DateRange range) {
			SelectionManager.AddSelection(range);
		}
		public event EventHandler SelectionChanged {
			add { Events.AddHandler(selectionChanged, value); }
			remove { Events.RemoveHandler(selectionChanged, value); }
		}
		public event CustomDrawDayNumberCellEventHandler CustomDrawDayNumberCell {
			add { Events.AddHandler(customDrawDayNumberCell, value); }
			remove { Events.RemoveHandler(customDrawDayNumberCell, value); }
		}
		public event DisableCalendarDateEventHandler DisableCalendarDate {
			add { Events.AddHandler(disabledCalendarDateCell, value); }
			remove { Events.RemoveHandler(disabledCalendarDateCell, value); }
		}
		public event SpecialCalendarDateEventHandler SpecialCalendarDate {
			add { Events.AddHandler(specialCalendarDateCell, value); }
			remove { Events.RemoveHandler(specialCalendarDateCell, value); }
		}
		public event EventHandler DateTimeChanged {
			add { this.Events.AddHandler(dateTimeChanged, value); }
			remove { this.Events.RemoveHandler(dateTimeChanged, value); }
		}
		public event EventHandler DateTimeCommit {
			add { this.Events.AddHandler(dateTimeCommit, value); }
			remove { this.Events.RemoveHandler(dateTimeCommit, value); }
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
		protected internal virtual void RaiseDateTimeCommit(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[dateTimeCommit];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDateTimeChanged() {
			EventHandler handler = (EventHandler)this.Events[dateTimeChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseCustomDrawDayNumberCell(CustomDrawDayNumberCellEventArgs e) {
			CustomDrawDayNumberCellEventHandler handler = (CustomDrawDayNumberCellEventHandler)this.Events[customDrawDayNumberCell];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDisableCalendarDate(DisableCalendarDateEventArgs e) {
			DisableCalendarDateEventHandler handler = (DisableCalendarDateEventHandler)this.Events[disabledCalendarDateCell];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseSpecialCalendarDate(SpecialCalendarDateEventArgs e) {
			SpecialCalendarDateEventHandler handler = (SpecialCalendarDateEventHandler)this.Events[specialCalendarDateCell];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseSelectionChanged() {
			EventHandler handler = Events[selectionChanged] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual CalendarViewInfoBase CalendarViewInfo { get { return (CalendarViewInfoBase)base.ViewInfo; } }
		protected internal new CalendarViewInfoBase ViewInfo { get { return (CalendarViewInfoBase)base.ViewInfo; } }
		protected override BaseControlPainter CreatePainter() {
			return new CalendarPainter();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new CalendarViewInfoBase(this);
		}
		protected internal virtual bool ClockIsLeft { get { return false; } }
		protected internal virtual DateEditCalendarViewType GetStartView() {
			return DateEditCalendarViewType.MonthInfo;
		}
		protected internal virtual void SetViewCore(DateEditCalendarViewType v) { view = v; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual DateEditCalendarViewType View {
			get { return view; }
			set {
				DateEditCalendarViewType prevView = View;
				if(View == value) return;
				OnViewChanging(prevView, value);
				SetViewCore(value);
				OnViewChanged(prevView, View);
			}
		}
		protected virtual void OnViewChanged(DateEditCalendarViewType prevView, DateEditCalendarViewType currView, bool shouldUpdateLayout) {
			ViewInfo.OnViewChanged(prevView, currView, shouldUpdateLayout);
		}
		protected virtual void OnViewChanging(DateEditCalendarViewType prevView, DateEditCalendarViewType currView, bool shouldUpdateLayout) {
			ViewInfo.OnViewChanging(prevView, currView, shouldUpdateLayout);
		}
		protected virtual void OnViewChanged(DateEditCalendarViewType prevView, DateEditCalendarViewType currView) {
			OnViewChanged(prevView, currView, true);
		}
		protected virtual void OnViewChanging(DateEditCalendarViewType prevView, DateEditCalendarViewType currView) {
			OnViewChanging(prevView, currView, true);
		}
		private DefaultBoolean calendarTimeEditing = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean CalendarTimeEditing {
			get { return calendarTimeEditing; }
			set {
				if(CalendarTimeEditing == value)
					return;
				calendarTimeEditing = value;
				OnCalendarTimeEditingChanged();
			}
		}
		protected internal virtual void OnCalendarTimeEditingChanged() {
			if(CalendarTimeEditing == DefaultBoolean.False) {
				Controls.Remove(TimeEdit);
			}
			else if(CalendarTimeEditing == DefaultBoolean.True) {
				Controls.Add(TimeEdit);
			}
			OnPropertiesChanged();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDateEditCalendarTimeProperties"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual RepositoryItemTimeEdit CalendarTimeProperties { 
			get { return TimeEdit.Properties; } 
		}
		[Browsable(false)]
		public virtual bool IsDateCleared { get; protected set; }
		protected int SuppressCommitOnTimeChanged { get; set; }
		protected virtual void OnTimeEditTextChanged(object sender, EventArgs e) {
			if(IsDateCleared) return;
			DateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, TimeEdit.Time.Hour, TimeEdit.Time.Minute, TimeEdit.Time.Second, DateTime.Kind);
			((VistaCalendarRightAreaViewInfo)ViewInfo.RightArea).CalcClockPoints();
			Invalidate(((VistaCalendarRightAreaViewInfo)ViewInfo.RightArea).ClockBounds);
			ViewInfo.UpdateClock();
		}
		TimeEdit timeEdit;
		protected internal virtual TimeEdit TimeEdit {
			get {
				if(timeEdit == null)
					timeEdit = CreateTimeEditor();
				return timeEdit;
			}
		}
		protected virtual TimeEdit CreateTimeEditor() {
			TimeEdit res = new TimeEdit();
			SuppressCommitOnTimeChanged++;
			try { res.EditValue = DateTime; }
			finally {
				SuppressCommitOnTimeChanged--;
			}
			res.TextChanged += new EventHandler(OnTimeEditTextChanged);
			if(!res.Properties.ReadOnly)
				res.Properties.ReadOnly = ReadOnly;
			return res;
		}
		private CalendarView calendarView = CalendarView.Default;
		protected internal virtual CalendarView ActualCalendarView { 
			get {
				if(CalendarView == Repository.CalendarView.Default)
					return NativeVista.IsVistaOrLater? CalendarView.Vista: CalendarView.Classic;
				return CalendarView;
			} 
		}
		[DefaultValue(CalendarView.Default)]
		public virtual CalendarView CalendarView {
			get { return calendarView; }
			set {
				value = ConstrainView(value);
				if(CalendarView == value)
					return;
				calendarView = value;
				OnCalendarViewChanged();
			}
		}
		private void OnCalendarViewChanged() {
			if(ActualCalendarView == Repository.CalendarView.Classic)
				this.view = DateEditCalendarViewType.MonthInfo;
			ResetViewInfo();
			ResetPainter();
			ResetHandler();
			OnPropertiesChanged();
		}
		protected internal override void OnPropertiesChanged() {
			ViewInfo.IsReady = false;
			CheckUpdateAutoSize();
			base.OnPropertiesChanged();
		}
		protected virtual void CheckUpdateAutoSize() {
			if(AutoSize) Size = CalcBestSize();
		}
		protected virtual CalendarView ConstrainView(CalendarView value) {
			if(value == CalendarView.TouchUI)
				return CalendarView.Default;
			return value;
		}
		int rowCount = 0;
		[DXCategory(CategoryName.Behavior), DefaultValue(0)]
		public virtual int RowCount {
			get { return rowCount; }
			set {
				value = ConstrainRowCount(value);
				if(RowCount == value)
					return;
				rowCount = value;
				OnPropertiesChanged();
			}
		}
		protected virtual int ConstrainRowCount(int value) {
			return Math.Max(0, value);
		}
		int columnCount = 0;
		[DXCategory(CategoryName.Behavior), DefaultValue(0)]
		public virtual int ColumnCount {
			get { return columnCount; }
			set {
				value = ConstrainColumnCount(value);
				if(ColumnCount == value)
					return;
				columnCount = value;
				OnPropertiesChanged();
			}
		}
		protected virtual int ConstrainColumnCount(int value) {
			return Math.Max(0, value);
		}
		private bool highlightSelection = true;
		[DefaultValue(true)]
		public bool HighlightSelection {
			get { return highlightSelection; }
			set {
				if(HighlightSelection == value)
					return;
				highlightSelection = value;
				OnPropertiesChanged();
			}
		}
		bool highlightHolidays = true;
		[DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public virtual bool HighlightHolidays {
			get { return highlightHolidays; }
			set {
				if(HighlightHolidays == value) return;
				highlightHolidays = value;
				OnPropertiesChanged();
			}
		}
		private Color todayDayCellFrameColor = Color.Empty;
		bool ShouldSerializeTodayDayCellFrameColor() { return TodayDayCellFrameColor != Color.Empty; }
		void ResetTodayDayCellFrameColor() { TodayDayCellFrameColor = Color.Empty; }
		public Color TodayDayCellFrameColor {
			get { return todayDayCellFrameColor; }
			set {
				if(TodayDayCellFrameColor == value)
					return;
				todayDayCellFrameColor = value;
				OnPropertiesChanged();
			}
		}
		DefaultBoolean highlightTodayCell = DefaultBoolean.Default;
		[DXCategory(CategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean HighlightTodayCell {
			get { return highlightTodayCell; }
			set {
				if(HighlightTodayCell == value) return;
				highlightTodayCell = value;
				OnPropertiesChanged();
			}
		}
		private DateTime minValue = DateTime.MinValue;
		void ResetMinValue() { MinValue = DateTime.MinValue; }
		bool ShouldSerializeMinValue() { return MinValue != DateTime.MinValue; }
		public virtual DateTime MinValue {
			get { return minValue; }
			set {
				value = ConstrainMinValue(value);
				if(MinValue == value)
					return;
				minValue = value;
				OnMinValueChanged();
			}
		}
		protected virtual void OnMinValueChanged() {
			EditValue = ConstrainValue(EditValue);
			OnPropertiesChanged();
		}
		protected virtual DateTime ConstrainMinValue(System.DateTime value) {
			if(value > MaxValue)
				return MaxValue;
			return value;
		}
		private DateTime maxValue = DateTime.MaxValue;
		void ResetMaxValue() { MaxValue = DateTime.MaxValue; }
		bool ShouldSerializeMaxValue() { return MaxValue != DateTime.MaxValue; }
		public virtual DateTime MaxValue {
			get { return maxValue; }
			set {
				value = ConstrainMaxValue(value);
				if(MaxValue == value)
					return;
				maxValue = value;
				OnMaxValueChanged();
			}
		}
		protected virtual void OnMaxValueChanged() {
			EditValue = ConstrainValue(EditValue);
			OnPropertiesChanged();
		}
		protected virtual DateTime ConstrainMaxValue(System.DateTime value) {
			if(value < MinValue)
				return MinValue;
			return value;
		}
		CalendarSelectionMode selectionMode = CalendarSelectionMode.Single;
		[DefaultValue(CalendarSelectionMode.Single)]
		public CalendarSelectionMode SelectionMode {
			get { return selectionMode; }
			set {
				if(SelectionMode == value)
					return;
				selectionMode = value;
				OnSelectionModeChanged();
			}
		}
		protected virtual void OnSelectionModeChanged() {
			if(SelectionMode == CalendarSelectionMode.Single)
				SelectedRanges.Clear();
		}
		private bool showClearButton = false;
		[DefaultValue(false)]
		public virtual bool ShowClearButton {
			get { return showClearButton; }
			set {
				if(ShowClearButton == value)
					return;
				showClearButton = value;
				OnPropertiesChanged();
			}
		}
		private bool sycnSelectionWithEditValue = true;
		[DefaultValue(true)]
		public bool SyncSelectionWithEditValue {
			get { return sycnSelectionWithEditValue; }
			set {
				if(SyncSelectionWithEditValue == value)
					return;
				sycnSelectionWithEditValue = value;
			}
		}
		private bool updateSelectionWhenNavigate;
		[DefaultValue(false)]
		public bool UpdateSelectionWhenNavigating {
			get { return updateSelectionWhenNavigate; }
			set {
				if(UpdateSelectionWhenNavigating == value)
					return;
				updateSelectionWhenNavigate = value;
			}
		}
		private bool changeDateWhenNavigate = true;
		[DefaultValue(true)]
		public bool UpdateDateTimeWhenNavigating {
			get { return changeDateWhenNavigate; }
			set {
				if(UpdateDateTimeWhenNavigating == value)
					return;
				changeDateWhenNavigate = value;
				OnChangeDateWhenNavigateChanged();
			}
		}
		protected virtual void OnChangeDateWhenNavigateChanged() {
		}
		private bool showTodayButton = true;
		[DefaultValue(true)]
		public virtual bool ShowTodayButton {
			get { return showTodayButton; }
			set {
				if(ShowTodayButton == value)
					return;
				showTodayButton = value;
				OnPropertiesChanged();
			}
		}
		private bool showWeekNumbers;
		[DefaultValue(false)]
		public virtual bool ShowWeekNumbers {
			get { return showWeekNumbers; }
			set {
				if(ShowWeekNumbers == value)
					return;
				showWeekNumbers = value;
				OnPropertiesChanged();
			}
		}
		private VistaCalendarInitialViewStyle vistaCalendarInitialViewStyle = VistaCalendarInitialViewStyle.MonthView;
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
		private VistaCalendarViewStyle vistaCalendarViewStyle = VistaCalendarViewStyle.Default;
		[DefaultValue(VistaCalendarViewStyle.Default)]
		public VistaCalendarViewStyle VistaCalendarViewStyle {
			get { return vistaCalendarViewStyle; }
			set {
				if(VistaCalendarViewStyle == value)
					return;
				vistaCalendarViewStyle = value;
				OnPropertiesChanged();
			}
		}
		private WeekNumberRule weekNumberRule = WeekNumberRule.Default;
		[DefaultValue(WeekNumberRule.Default)]
		public WeekNumberRule WeekNumberRule {
			get { return weekNumberRule; }
			set {
				if(WeekNumberRule == value)
					return;
				weekNumberRule = value;
				OnPropertiesChanged();
			}
		}
		private BorderStyles buttonStyle = BorderStyles.Default;
		[DefaultValue(BorderStyles.Default)]
		public BorderStyles ButtonsStyle {
			get { return buttonStyle; }
			set {
				if(ButtonsStyle == value)
					return;
				buttonStyle = value;
				OnPropertiesChanged();
			}
		}
		[Obsolete,
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime? HotDate {
			get { 
				if(ViewInfo == null || !ViewInfo.HoverInfo.IsInCell)
					return null;
				return ViewInfo.HoverInfo.HitDate;
			}
			set { }
		}
		[Obsolete,
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Multiselect {
			get { return SelectionMode == CalendarSelectionMode.Multiple; }
			set { SelectionMode = value ? CalendarSelectionMode.Multiple : CalendarSelectionMode.Single; }
		}
		private CalendarAppearances appearanceCalendar;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalendarAppearances CalendarAppearance {
			get {
				if(appearanceCalendar == null)
					appearanceCalendar = CreateAppearanceCalendar();
				return appearanceCalendar;
			}
		}
		protected virtual CalendarAppearances CreateAppearanceCalendar() {
			return new CalendarAppearances(this);
		}
		[Obsolete(ObsoleteText.SRCalendarControl_AppearanceCalendar),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceObject AppearanceCalendar {
			get { return Appearance; }
		}
		[Obsolete(ObsoleteText.SRCalendarControl_AppearanceCalendar_DayCellDisabled),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceObject AppearanceDisabledCalendarDate {
			get { return CalendarAppearance.DayCellDisabled; }
		}
		[Obsolete(ObsoleteText.SRCalendarControl_AppearanceCalendar_CalendarHeader),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceObject AppearanceHeader {
			get { return CalendarAppearance.CalendarHeader; }
		}
		[Obsolete(ObsoleteText.SRCalendarControl_AppearanceCalendar_WeekNumber),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceObject AppearanceWeekNumber {
			get { return CalendarAppearance.WeekNumber; }
		}
		protected DateTime GetFirstDayOfMonth(DateTime date) {
			return CalculateFirstMonthDate(date);
		}
		protected internal virtual Image RenderContentToImage(Image img) {
			if(img == null || img.Width != ViewInfo.CalendarsClientBounds.Width || img.Height != ViewInfo.CalendarsClientBounds.Height) {
				if(img != null)
					img.Dispose();
				img = new Bitmap(ViewInfo.CalendarsClientBounds.Width, ViewInfo.CalendarsContentBounds.Height);
			}
			try {
				using(Graphics g = Graphics.FromImage(img)) {
					using(var buffered = BufferedGraphicsManager.Current.Allocate(g, new Rectangle(Point.Empty, img.Size))) {
						PaintEventArgs e = new PaintEventArgs(buffered.Graphics, new Rectangle(Point.Empty, img.Size));
						GraphicsCache cache = new GraphicsCache(e);
						e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0x01, ViewInfo.PaintAppearance.BackColor)), Bounds);
						e.Graphics.TranslateTransform(-ViewInfo.CalendarsClientBounds.X, -ViewInfo.CalendarsClientBounds.Y);
						Painter.Draw(new CalendarControlInfoArgs(ViewInfo, cache, ClientRectangle));
						buffered.Render();
					}
				}
			}
			catch { }
			return img;
		}
		protected override void OnPaint(PaintEventArgs e) {
			CheckViewInfo(e);
			CalendarCellViewInfo vi = ViewInfo.GetCellByDate(new DateTime(2015, 10, 20));
			using(GraphicsCache cache = new GraphicsCache(e)) {
				CalendarControlInfoArgs infoArgs = new CalendarControlInfoArgs(ViewInfo, cache, e.ClipRectangle);
				Painter.Draw(infoArgs);
			}
		}
		protected virtual void CheckViewInfo(PaintEventArgs e) {
			CheckViewInfo(e.Graphics);
		}
		protected virtual void CheckViewInfo(Graphics graphics) {
			if(!ViewInfo.IsReady)
				ViewInfo.CalcViewInfo(graphics);
		}
		protected internal virtual bool CenterHorizontally { get { return true; } }
		protected internal virtual bool CenterVertically { get { return false; } }
		public virtual CalendarHitInfo GetHitInfo(Point location) {
			int count = ViewInfo.Calendars.Count;
			for(int i = 0; i < count; i++) {
				CalendarHitInfo hi = ViewInfo.Calendars[i].GetHitInfo(location);
				if(hi.HitTest != CalendarHitInfoType.Unknown)
					return hi;
			}
			if(ViewInfo.Calendars.Count == 0)
				return new CalendarHitInfo(location);
			return ViewInfo.Calendars[0].GetHitInfo(location);
		}
		public virtual CalendarHitInfo GetHitInfo(MouseEventArgs e) {
			return GetHitInfo(e.Location);
		}
		public virtual void ProcessKeyDown(KeyEventArgs e) {
			if(e.Handled == true)
				return;
			if(e.KeyData == Keys.Enter) {
				if (SkipChangingDate(DateTime)) return;
				OnDateTimeCommit(DateTime);
				e.Handled = true;
				return;
			}
			OnKeyDown(e);
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			OnKeyDown(new KeyEventArgs(keyData));
			return base.ProcessCmdKey(ref msg, keyData);
		}
		protected internal virtual bool SkipChangingDate(DateTime newDate) {
			for (int i = 0; i < ViewInfo.Calendars.Count; i++) {
				DayNumberCellInfoCollection dayCells = ViewInfo.Calendars[i].DayCells;
				for (int j = 0; j < dayCells.Count; j++)
					if (dayCells[j].IsDisabled && newDate == dayCells[j].Date)
						return true;
			}
			return false;
		}
		protected internal virtual bool ShowTimeEdit { get { return false; } }
		protected internal int GetWeekDayNumber(DateTime date) {
			DateTime startOfWeek = GetStartOfWeekCore(date, FirstDayOfWeek);
			if(startOfWeek > date)
				return 7 - (startOfWeek - date).Days;
			else
				return (date - startOfWeek).Days;
		}
		protected internal static DateTime GetStartOfWeekCore(DateTime date, DayOfWeek firstDayOfWeek) {
			DateTime start = date;
			DateTime sevenDays = DateTime.MinValue + TimeSpan.FromDays(7);
			TimeSpan daySpan = TimeSpan.FromDays(1);
			TimeSpan signedDaySpan = (start < sevenDays) ? daySpan : -daySpan;
			for(int i = 0; i <= 7; i++) {
				if(start.DayOfWeek == firstDayOfWeek)
					break;
				start += signedDaySpan;
			}
			return start;
		}
		protected internal virtual DateTime FilterDate(DateTime dt) {
			dt = ChangeSelectedDay(dt);
			if(dt < MinValue) return MinValue;
			if(dt > MaxValue) return MaxValue;
			return dt;
		}
		protected internal virtual void OnDateTimeCommit(object value) {
			EditValue = value;
			RaiseDateTimeCommit(EventArgs.Empty);
		}
		protected virtual DateTime CalculateFirstMonthDate(DateTime dateTime) {
			return CalendarObjectViewInfo.GetFirstMonthDate(dateTime);
		}
		internal void LockRecalcStartDate() { lockStartDateCounter++; }
		internal void UnlockRecalcStartDate() { lockStartDateCounter--; }
		protected virtual bool RecalcStartDate {
			get {
				if(lockStartDateCounter != 0) return false;
				return (ViewInfo.Calendars.Count == 1);
			}
		}
		[Bindable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		internal bool ShouldSerializeDateTime() { return DateTime != GetTodayDate(); }
		internal void ResetDateTime() { DateTime = GetTodayDate(); }
		[DXCategory(CategoryName.Appearance), RefreshProperties(RefreshProperties.All)]
		public virtual DateTime DateTime {
			get { return ConvertToDateTime(EditValue); }
			set {
				value = FilterDate(value);
				EditValue = value;
			}
		}
		private DateEditValueConverter converter;
		public DateEditValueConverter Converter {
			get {
				if(converter == null)
					converter = CreateConverter();
				return converter;
			}
		}
		protected virtual DateEditValueConverter CreateConverter() {
			return new DateEditValueConverter(this);
		}
		protected virtual System.DateTime ConvertToDateTime(object value) {
			return Converter.ConvertToDateTime(value);
		}
		internal bool ShouldSerializeEditValue() { return !object.Equals(EditValue, GetTodayDate()); }
		internal void ResetEditValue() { EditValue = GetTodayDate(); }
		object editValue;
		[Bindable(true), Localizable(true), DXCategory(CategoryName.Data),
		RefreshProperties(RefreshProperties.All),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object EditValue {
			get { return editValue; }
			set {
				if(SuppressChangeEditValue)
					return;
				value = DoParseEditValue(value).Value;
				value = ConstrainValue(value);
				if(object.Equals(EditValue, value))
					return;
				DateTime prevDate = DateTime;
				editValue = value;
				OnEditValueChanged();
				CheckDateTimeChanged(prevDate);
			}
		}
		private object ConstrainValue(object value) {
			if(!(value is DateTime))
				return value;
			DateTime date = (DateTime)value;
			if(date < MinValue)
				date = MinValue;
			if(date > MaxValue)
				date = MaxValue;
			return date;
		}
		protected virtual void CheckDateTimeChanged(System.DateTime prevDate) {
			if(!object.Equals(prevDate, DateTime))
				RaiseDateTimeChanged();
		}
		protected virtual internal ConvertEditValueEventArgs DoParseEditValue(object val) {
			ConvertEditValueEventArgs args = new ConvertEditValueEventArgs(val);
			RaiseParseEditValue(args);
			return args;
		}
		protected virtual internal ConvertEditValueEventArgs DoFormatEditValue(object val) {
			ConvertEditValueEventArgs args = new ConvertEditValueEventArgs(val);
			RaiseFormatEditValue(args);
			return args;
		}
		protected internal virtual void RaiseParseEditValue(ConvertEditValueEventArgs e) {
			ConvertEditValueEventHandler handler = (ConvertEditValueEventHandler)this.Events[parseEditValue];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseFormatEditValue(ConvertEditValueEventArgs e) {
			ConvertEditValueEventHandler handler = (ConvertEditValueEventHandler)this.Events[formatEditValue];
			if(handler != null) handler(this, e);
		}
		[DXCategory(CategoryName.Events)]
		public event ConvertEditValueEventHandler ParseEditValue {
			add { this.Events.AddHandler(parseEditValue, value); }
			remove { this.Events.RemoveHandler(parseEditValue, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event ConvertEditValueEventHandler FormatEditValue {
			add { this.Events.AddHandler(formatEditValue, value); }
			remove { this.Events.RemoveHandler(formatEditValue, value); }
		}
		[DXCategory(CategoryName.Events)]
		public event EventHandler EditValueChanged {
			add { this.Events.AddHandler(editValueChanged, value); }
			remove { this.Events.RemoveHandler(editValueChanged, value); }
		}
		[Obsolete(ObsoleteText.SRCalendarControl_EditValueChanged), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler EditDateModified {
			add { EditValueChanged += value; }
			remove { EditValueChanged -= value; }
		}
		bool SuppressChangeEditValue { get; set; }
		protected internal virtual void OnEditValueChanged() {
			SuppressChangeEditValue = true;
			try {
				if(EditValue == null) {
					SelectedRanges.Clear();
					if(NullDate == null)
						Handler.SetSelectedDateCore(GetTodayDate());
					else
						Handler.SetSelectedDateCore(DateTime);
				}
				else {
					if(SyncSelectionWithEditValue)
						SetSelection(DateTime);
					Handler.SelectedDate = DateTime;
				}
			}
			finally {
				SuppressChangeEditValue = false;
			}
			CalendarCellViewInfo start = ViewInfo.GetFirstActiveVisibleCellInfo();
			CalendarCellViewInfo end = ViewInfo.GetLastActiveVisibleCellInfo();
			bool recalcLayout = start == null || end == null || DateTime < start.Date || DateTime > end.Date;
			if(recalcLayout) {
				UpdateStartDateCore();
			}
			if(recalcLayout || Handler.ShouldUpdateOnDateChange) {
				LayoutChanged();
			}
			RaiseEditValueChanged();
		}
		DateTime startDate;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime StartDate {
			get { return startDate; }
			set {
				if(StartDate == value)
					return;
				startDate = UpdateStartDate(value);
				OnStartDateChanged();
			}
		}
		protected virtual void OnStartDateChanged() {
			Handler.SetSelectedDateCore(StartDate);
			LayoutChanged();
		}
		protected internal virtual void UpdateStartDateCore(DateTime dt) {
			this.startDate = UpdateStartDate(dt);
		}
		protected internal virtual void UpdateStartDateCore() {
			UpdateStartDateCore(DateTime);
		}
		protected internal virtual void UpdateStartDate() {
			StartDate = UpdateStartDate(DateTime);
		}
		protected virtual DateTime UpdateStartDate(DateTime dateTime) {
			switch(View) {
				case DateEditCalendarViewType.MonthInfo:
				case DateEditCalendarViewType.QuarterInfo:
					return new DateTime(dateTime.Year, dateTime.Month, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
				case DateEditCalendarViewType.YearInfo:
					return new DateTime(dateTime.Year, 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
				case DateEditCalendarViewType.YearsInfo:
					 return new DateTime(Math.Max((dateTime.Year / 10) * 10, 1), 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
				case DateEditCalendarViewType.YearsGroupInfo:
					return new DateTime(Math.Max((dateTime.Year / 100 ) * 100, 1), 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
			}
			DateTime res = new DateTime(dateTime.Year, dateTime.Month, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
			return res.AddDays(GetFirstDayOffset(res));
		}
		protected int GetFirstDayOffset(DateTime firstMonthDate) {
			return (FirstDayOfWeek == firstMonthDate.DayOfWeek ? 7 : (7 + firstMonthDate.DayOfWeek - FirstDayOfWeek) % 7);
		}
		protected virtual void RaiseEditValueChanged() {
			EventHandler handler = (EventHandler)Events[editValueChanged];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected virtual DateTime ChangeSelectedDay(DateTime value) {
			return value;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual DateTime SelectionStart {
			get {
				if(SelectedRanges.Count > 0)
					return SelectedRanges.Start;
				return DateTime; 
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual DateTime SelectionEnd {
			get {
				if(SelectedRanges.Count > 0)
					return SelectedRanges.End;
				return DateTime; 
			}
		}
		DatesCollection selection;
		[Obsolete("Use the SelectedRanges property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual DatesCollection Selection { 
			get {
				return SelectionCore;
			} 
		}
		protected internal DatesCollection SelectionCore {
			get {
				if(selection == null)
					selection = CreateSelection();
				return selection; 
			}
		}
		protected virtual DatesCollection CreateSelection() {
			return new DatesCollection(this);
		}
		DateRangeCollection selectedRanges;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public DateRangeCollection SelectedRanges {
			get {
				if(selectedRanges == null)
					selectedRanges = CreateSelectedRanges();
				return selectedRanges;
			}
		}
		protected virtual DateRangeCollection CreateSelectedRanges() {
			return new DateRangeCollection(this);
		}
		private bool readOnly;
		[DefaultValue(false)]
		public bool ReadOnly {
			get { return readOnly; }
			set {
				if(ReadOnly == value)
					return;
				readOnly = value;
				OnPropertiesChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual DateTimeFormatInfo DateFormat {
			get {
				if(fFormat == null)
					return DevExpress.Data.Mask.DateTimeMaskManager.GetGoodCalendarDateTimeFormatInfo(CultureInfo.CurrentCulture);
				return fFormat;
			}
			set { fFormat = value; }
		}
		DayOfWeek firstDayOfWeek;
		bool ShouldSerializeFirstDayOfWeek() { return FirstDayOfWeek != DateFormat.FirstDayOfWeek; }
		void ResetFirstDayOfWeek() { FirstDayOfWeek = DateFormat.FirstDayOfWeek; }
		public virtual DayOfWeek FirstDayOfWeek { 
			get { return firstDayOfWeek; }
			set {
				if(FirstDayOfWeek == value)
					return;
				firstDayOfWeek = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor { get { return GetBackColorCore(); } set { SetBackColorCore(value); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor { get { return GetForeColorCore(); } set { SetForeColorCore(value); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }
		protected virtual Color GetBackColorCore() { return base.BackColor; }
		protected virtual void SetBackColorCore(Color color) { base.BackColor = color; }
		protected virtual Color GetForeColorCore() { return base.ForeColor; }
		protected virtual void SetForeColorCore(Color color) { base.ForeColor = color; }
		CalendarControlHandlerBase handler;
		protected internal virtual CalendarControlHandlerBase Handler {
			get {
				if(handler == null)
					handler = CreateHandler();
				return handler;
			}
		}
		protected virtual CalendarControlHandlerBase CreateHandler() {
			if(ActualCalendarView == CalendarView.Vista)
				return new VistaCalendarControlHandler(this);
			return new CalendarControlHandler(this);
		}
		protected override void OnMouseEnter(EventArgs e) {
			CheckViewInfo((Graphics)null);
			base.OnMouseEnter(e);
			Handler.OnMouseEnter(e);
		}
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			Handler.OnDoubleClick(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = new DXMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta);
			try {
				base.OnMouseDown(ee);
				if(ee.Handled)
					return;
				Handler.OnMouseDown(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = new DXMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta);
			try {
				base.OnMouseUp(ee);
				if(ee.Handled)
					return;
				Handler.OnMouseUp(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = new DXMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta);
			try {
				CheckViewInfo((Graphics)null);
				base.OnMouseMove(ee);
				if(ee.Handled)
					return;
				Handler.OnMouseMove(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Handler.OnMouseLeave(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			Handler.OnKeyDown(e);
		}
		protected internal virtual void OnSelectionChanged() {
			RaiseSelectionChanged();
			ViewInfo.UpdateVisualInfo();
			UpdateVisual();
			SyncSelectionWithSelectedRanges();
		}
		protected virtual bool AllowSyncSelectionWithSelectedRanges { get { return false; } }
		protected virtual void SyncSelectionWithSelectedRanges() {
			if(!AllowSyncSelectionWithSelectedRanges)
				return;
			SuppressSyncSelectedRangeWidthSelection = true;
			try {
				SelectionCore.BeginUpdate();
				SelectionCore.Clear();
				foreach(DateRange range in SelectedRanges) {
					for(int i = 0; ; i++) {
						DateTime date = range.StartDate.AddDays(i);
						if(date >= range.EndDate)
							break;
						SelectionCore.Add(date);
					}
				}	  
			}
			finally {
				SelectionCore.EndUpdate();
				SuppressSyncSelectedRangeWidthSelection = false;
			}
		}
		object nullDate = null;
		internal bool ShouldSerializeNullDate() { return NullDate != null; }
		internal void ResetNullDate() { NullDate = null; }
		[DXCategory(CategoryName.Behavior),
		DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public virtual object NullDate {
			get { return nullDate; }
			set {
				if(NullDate == value) return;
				nullDate = value;
				OnPropertiesChanged();
			}
		}
		protected internal readonly object ContentChangedAnimationId = new object();
		void ICalendarAppearancesOwner.OnAppearanceChanged() {
			OnPropertiesChanged();
		}
		bool ICalendarAppearancesOwner.IsLoading {
			get { return IsLoading; }
		}
		object IDateTimeOwner.NullDate {
			get { return NullDate; }
		}
		ConvertEditValueEventArgs IDateTimeOwner.DoParseEditValue(object value) {
			return DoParseEditValue(value);
		}
		ConvertEditValueEventArgs IDateTimeOwner.DoFormatEditValue(object value) {
			return DoFormatEditValue(value);
		}
		protected Padding DefaultCellPadding = new Padding(-1);
		private Padding cellPadding = new Padding(-1);
		bool ShouldSerializeCellPadding() { return !IsDefaultCellPadding; }
		void ResetCellPadding() { CellPadding = DefaultCellPadding; }
		protected internal bool IsDefaultCellPadding { get { return object.Equals(CellPadding, DefaultCellPadding); } }
		public Padding CellPadding {
			get { return cellPadding; }
			set {
				if(CellPadding == value)
					return;
				cellPadding = value;
				OnPropertiesChanged();
			}
		}
		private bool showHeader = true;
		[DefaultValue(true)]
		public bool ShowHeader {
			get { return showHeader; }
			set {
				if(ShowHeader == value)
					return;
				showHeader = value;
				OnPropertiesChanged();
			}
		}
		private bool showFooter = true;
		[DefaultValue(true)]
		public bool ShowFooter {
			get { return showFooter; }
			set {
				if(ShowFooter == value)
					return;
				showFooter = value;
				OnPropertiesChanged();
			}
		}
		private int calendarIndent =- 1;
		[DefaultValue(-1)]
		public int CalendarIndent {
			get { return calendarIndent; }
			set {
				if(CalendarIndent == value)
					return;
				calendarIndent = value;
				OnPropertiesChanged();
			}
		}
		protected bool SuppressSyncSelectedRangeWidthSelection { get; set; }
		protected bool SuppressSyncSelectionWithSelectedRange { get; set; }
		protected void SyncSelectedRangeWithSelection() {
			if(SuppressSyncSelectedRangeWidthSelection)
				return;
			SuppressSyncSelectionWithSelectedRange = true;
			try {
				SelectedRanges.BeginUpdate();
				SelectedRanges.Clear();
				foreach(DateTime date in SelectionCore) {
					if(SelectedRanges.Contains(date))
						continue;
					AddSelection(date);
				}
			}
			finally {
				SelectedRanges.EndUpdate();
				SuppressSyncSelectionWithSelectedRange = false;
			}
		}
		void IDatesCollectionOwner.OnCollectionChanged() {
			SyncSelectedRangeWithSelection();
		}
		protected internal System.DateTime GetTodayDate() {
			return TodayDate != DateTime.MinValue ? TodayDate: DateTime.Today;
		}
	}
	public class CalendarPaintStyle {
		public CalendarPaintStyle(CalendarViewInfoBase viewInfo) {
			ViewInfo = viewInfo;
		}
		internal CalendarViewInfoBase ViewInfo {
			get;
			private set;
		}
		public CalendarControlBase Calendar { get { return ViewInfo.Calendar; } }
		public UserLookAndFeel LookAndFeel { get { return ViewInfo.LookAndFeel.ActiveLookAndFeel; } }
		AppearanceDefault
			calendarHeaderAppearanceDefault,
			buttonAppearanceDefault,
			buttonHighlightedAppearanceDefault,
			buttonPressedAppearanceDefault,
			headerAppearanceDefault,
			highlightHeaderAppearanceDefault,
			pressedHeaderAppearanceDefault,
			weekNumberAppearanceDefault,
			dayCellAppearanceDefault,
			dayCellSpecialAppearanceDefault,
			highlightedDayCellSpecialAppearanceDefault,
			pressedDayCellSpecialAppearanceDefault,
			disabledDayCellAppearanceDefault,
			disabledDayCellSpecialAppearanceDefault,
			inactiveDayCellAppearanceDefault,
			inactiveDayCellSpecialAppearanceDefault,
			selectedDayCellAppearanceDefault,
			selectedDayCellSpecialAppearanceDefault,
			todayDayCellAppearanceDefault,
			highlightDayCellAppearanceDefault,
			pressedDayCellAppearanceDefault,
			holidayCellAppearanceDefault,
			weekDayAppearanceDefault;
		Font dayCellFont;
		public Font DayCellFont {
			get {
				if(dayCellFont == null)
					dayCellFont = CreateDayCellFont();
				return dayCellFont;
			}
		}
		Font dayCellSpecialFont;
		public Font DayCellSpecialFont {
			get {
				if(dayCellSpecialFont == null)
					dayCellSpecialFont = CreateDayCellSpecialFont();
				return dayCellSpecialFont;
			}
		}
		protected virtual Font CreateDayCellSpecialFont() {
			return new Font(DayCellFont, FontStyle.Bold);
		}
		protected virtual Font CreateDayCellFont() {
			return AppearanceObject.DefaultFont;
		}
		Font weekNumberFont;
		public Font WeekNumberFont {
			get {
				if(weekNumberFont == null)
					weekNumberFont = CreateWeekNumberFont();
				return weekNumberFont;
			}
		}
		protected virtual Font CreateWeekNumberFont() {
			return new Font(AppearanceObject.DefaultFont.FontFamily.Name, 6);
		}
		Font weekDayFont;
		public Font WeekDayAbbreviationFont {
			get {
				if(weekDayFont == null)
					weekDayFont = CreateWeekDayAbbreviationFont();
				return weekDayFont;
			}
		}
		protected virtual Font CreateWeekDayAbbreviationFont() {
			return AppearanceObject.DefaultFont;
		}
		Font headerFont;
		public Font HeaderFont {
			get {
				if(headerFont == null)
					headerFont = CreateHeaderFont();
				return headerFont;
			}
		}
		Font calendarHeaderFont;
		public Font CalendarHeaderFont {
			get {
				if(calendarHeaderFont == null)
					calendarHeaderFont = CreateCalendarHeaderFont();
				return calendarHeaderFont;
			}
		}
		private Font CreateCalendarHeaderFont() {
			return new Font(AppearanceObject.DefaultFont.FontFamily, 10);
		}
		protected virtual Font CreateHeaderFont() {
			return AppearanceObject.DefaultFont;
		}
		protected internal virtual CalendarHeaderObjectPainter HeaderPainter { get { return new CalendarHeaderObjectPainter(); } }
		protected internal virtual CalendarFooterObjectPainter FooterPainter { get { return new CalendarFooterObjectPainter(); } }
		protected internal virtual CalendarRightAreaObjectPainter RightAreaPainter { get { return new CalendarRightAreaObjectPainter(); } }
		protected internal virtual CalendarObjectPainter CalendarPainter { get { return new CalendarObjectPainter(); } }
		protected internal virtual CalendarCellPainter DayCellPainter { get { return new CalendarCellPainter(); } }
		protected internal virtual ObjectPainter NavigationButtonPainter { 
			get { return new CalendarNavigationButtonPainter(); } 
		}
		protected internal virtual EditorButtonPainter ButtonPainter {
			get { return EditorButtonHelper.GetPainter(Calendar.ButtonsStyle, ViewInfo.LookAndFeel.ActiveLookAndFeel); }
		}
		#region Appearance
		protected virtual HorzAlignment DayCellTextAlignment { get { return HorzAlignment.Center; } }
		protected virtual HorzAlignment HeaderTextAlignment { get { return HorzAlignment.Center; } }
		protected virtual HorzAlignment WeekDayTextAlignment { get { return HorzAlignment.Center; } }
		protected virtual HorzAlignment WeekNumberTextAlignment { get { return HorzAlignment.Far; } }
		protected virtual Color CalendarHeaderForeColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color HeaderForeColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color HeaderForeColorHighlight { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color HeaderForeColorPressed { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color ButtonForeColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color ButtonForeColorHighlight { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color ButtonForeColorPressed { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color HeaderBackColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ButtonFace); } }
		protected virtual Color HeaderBackColorHighlight { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ButtonFace); } }
		protected virtual Color HeaderBackColorPressed { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ButtonFace); } }
		protected virtual Color DayCellForeColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color DayCellBackColor { get { return Color.Empty; } }
		protected virtual Color DayCellForeColorHighlight { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.HighlightText); } }
		protected virtual Color DayCellBackColorHighlight { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Highlight); } }
		protected virtual Color DayCellForeColorPressed { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.HighlightText); } }
		protected virtual Color DayCellBackColorPressed { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Highlight); } }
		protected virtual Color DayCellForeColorSelected { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.HighlightText); } }
		protected virtual Color DayCellBackColorSelected { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Highlight); } }
		protected virtual Color DayCellForeColorDisabled { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.GrayText); } }
		protected virtual Color DayCellBackColorDisabled { get { return Color.Empty; } }
		protected virtual Color DayCellForeColorInactive { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.GrayText); } }
		protected virtual Color DayCellBackColorInactive { get { return Color.Empty; } }
		protected virtual Color DayCellSpecialForeColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color DayCellSpecialForeColorHighlight { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.HighlightText); } }
		protected virtual Color DayCellSpecialForeColorPressed { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.HighlightText); } }
		protected virtual Color DayCellSpecialForeColorSelected { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.HighlightText); } }
		protected virtual Color DayCellSpecialForeColorDisabled { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.GrayText); } }
		protected virtual Color DayCellSpecialForeColorInactive { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.GrayText); } }
		protected virtual Color DayCellForeColorToday { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color DayCellBackColorToday { get { return Color.Empty; } }
		protected virtual Color DayCellForeColorHoliday { get { return Color.Red; } }
		protected virtual Color DayCellBackColorHoliday { get { return Color.Empty; } }
		protected virtual Color WeekDayForeColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected virtual Color WeekNumberForeColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText); } }
		protected internal virtual AppearanceDefault CalendarHeaderAppearanceDefault {
			get {
				if(calendarHeaderAppearanceDefault == null)
					calendarHeaderAppearanceDefault = new AppearanceDefault(CalendarHeaderForeColor, Color.Transparent, HorzAlignment.Center, VertAlignment.Center, CalendarHeaderFont);
				return calendarHeaderAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault ButtonAppearanceDefault {
			get {
				if(buttonAppearanceDefault == null)
					buttonAppearanceDefault = new AppearanceDefault(ButtonForeColor, Color.Transparent, HeaderTextAlignment, VertAlignment.Center, HeaderFont);
				return buttonAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault HighlightButtonAppearanceDefault {
			get {
				if(buttonHighlightedAppearanceDefault == null)
					buttonHighlightedAppearanceDefault = new AppearanceDefault(ButtonForeColorHighlight, Color.Transparent, HeaderTextAlignment, VertAlignment.Center, HeaderFont);
				return buttonHighlightedAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault PressedButtonAppearanceDefault {
			get {
				if(buttonPressedAppearanceDefault == null)
					buttonPressedAppearanceDefault = new AppearanceDefault(ButtonForeColorPressed, Color.Transparent, HeaderTextAlignment, VertAlignment.Center, HeaderFont);
				return buttonPressedAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault HeaderAppearanceDefault {
			get {
				if(headerAppearanceDefault == null) 
					headerAppearanceDefault = new AppearanceDefault(HeaderForeColor, HeaderBackColor, HeaderTextAlignment, VertAlignment.Center, HeaderFont);
				return headerAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault HighlightHeaderAppearanceDefault {
			get {
				if(highlightHeaderAppearanceDefault == null) 
					highlightHeaderAppearanceDefault = new AppearanceDefault(HeaderForeColorHighlight, HeaderBackColorHighlight, HeaderTextAlignment, VertAlignment.Center, HeaderFont);
				return highlightHeaderAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault PressedHeaderAppearanceDefault {
			get {
				if(pressedHeaderAppearanceDefault == null)
					pressedHeaderAppearanceDefault = new AppearanceDefault(HeaderForeColorPressed, HeaderBackColorPressed, HeaderTextAlignment, VertAlignment.Center, HeaderFont);
				return pressedHeaderAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault DayCellAppearanceDefault {
			get {
				if(dayCellAppearanceDefault == null) {
					dayCellAppearanceDefault = new AppearanceDefault(DayCellForeColor, DayCellBackColor, DayCellTextAlignment, VertAlignment.Center, DayCellFont);
				}
				return dayCellAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault HighlightDayCellAppearanceDefault {
			get {
				if(highlightDayCellAppearanceDefault == null) {
					highlightDayCellAppearanceDefault = new AppearanceDefault(DayCellForeColorHighlight, DayCellBackColorHighlight, DayCellTextAlignment, VertAlignment.Center, DayCellFont);
				}
				return highlightDayCellAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault PressedDayCellAppearanceDefault {
			get {
				if(pressedDayCellAppearanceDefault == null) {
					pressedDayCellAppearanceDefault = new AppearanceDefault(DayCellForeColorPressed, DayCellBackColorPressed, DayCellTextAlignment, VertAlignment.Center, DayCellFont);
				}
				return pressedDayCellAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault SelectedDayCellAppearanceDefault {
			get {
				if(selectedDayCellAppearanceDefault == null) {
					selectedDayCellAppearanceDefault = new AppearanceDefault(DayCellForeColorSelected, DayCellBackColorSelected, DayCellTextAlignment, VertAlignment.Center, DayCellFont);
				}
				return selectedDayCellAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault DisabledDayCellAppearanceDefault {
			get {
				if(disabledDayCellAppearanceDefault == null) {
					disabledDayCellAppearanceDefault = new AppearanceDefault(DayCellForeColorDisabled, DayCellBackColorDisabled, DayCellTextAlignment, VertAlignment.Center, DayCellFont);
				}
				return disabledDayCellAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault InactiveDayCellAppearanceDefault {
			get {
				if(inactiveDayCellAppearanceDefault == null) {
					inactiveDayCellAppearanceDefault = new AppearanceDefault(DayCellForeColorInactive, DayCellBackColorInactive, DayCellTextAlignment, VertAlignment.Center, DayCellFont);
				}
				return inactiveDayCellAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault DayCellSpecialAppearanceDefault {
			get {
				if(dayCellSpecialAppearanceDefault == null) {
					dayCellSpecialAppearanceDefault = new AppearanceDefault(DayCellSpecialForeColor, DayCellBackColor, DayCellTextAlignment, VertAlignment.Center, DayCellSpecialFont);
				}
				return dayCellSpecialAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault HighlightDayCellSpecialAppearanceDefault {
			get {
				if(highlightedDayCellSpecialAppearanceDefault == null) {
					highlightedDayCellSpecialAppearanceDefault = new AppearanceDefault(DayCellSpecialForeColorHighlight, DayCellBackColorHighlight, DayCellTextAlignment, VertAlignment.Center, DayCellSpecialFont);
				}
				return highlightedDayCellSpecialAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault PressedDayCellSpecialAppearanceDefault {
			get {
				if(pressedDayCellSpecialAppearanceDefault == null) {
					pressedDayCellSpecialAppearanceDefault = new AppearanceDefault(DayCellSpecialForeColorPressed, DayCellBackColorPressed, DayCellTextAlignment, VertAlignment.Center, DayCellSpecialFont);
				}
				return pressedDayCellSpecialAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault SelectedDayCellSpecialAppearanceDefault {
			get {
				if(selectedDayCellSpecialAppearanceDefault == null) {
					selectedDayCellSpecialAppearanceDefault = new AppearanceDefault(DayCellSpecialForeColorSelected, DayCellBackColorSelected, DayCellTextAlignment, VertAlignment.Center, DayCellSpecialFont);
				}
				return selectedDayCellSpecialAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault DisabledDayCellSpecialAppearanceDefault {
			get {
				if(disabledDayCellSpecialAppearanceDefault == null) {
					disabledDayCellSpecialAppearanceDefault = new AppearanceDefault(DayCellSpecialForeColorDisabled, DayCellBackColorDisabled, DayCellTextAlignment, VertAlignment.Center, DayCellSpecialFont);
				}
				return disabledDayCellSpecialAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault InactiveDayCellSpecialAppearanceDefault {
			get {
				if(inactiveDayCellSpecialAppearanceDefault == null) {
					inactiveDayCellSpecialAppearanceDefault = new AppearanceDefault(DayCellSpecialForeColorInactive, DayCellBackColorInactive, DayCellTextAlignment, VertAlignment.Center, DayCellSpecialFont);
				}
				return inactiveDayCellSpecialAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault TodayCellAppearanceDefault {
			get {
				if(todayDayCellAppearanceDefault == null) {
					todayDayCellAppearanceDefault = new AppearanceDefault(DayCellForeColorToday, DayCellBackColorToday, DayCellTextAlignment, VertAlignment.Center, DayCellFont);
				}
				return todayDayCellAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault HolidayCellAppearanceDefault {
			get {
				if(holidayCellAppearanceDefault == null) 
					holidayCellAppearanceDefault = new AppearanceDefault(DayCellForeColorHoliday, DayCellBackColorHoliday, DayCellTextAlignment, VertAlignment.Center, DayCellFont);
				return holidayCellAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault WeekDayAppearanceDefault {
			get {
				if(weekDayAppearanceDefault == null) {
					weekDayAppearanceDefault = new AppearanceDefault(WeekDayForeColor, Color.Empty, WeekDayTextAlignment, VertAlignment.Center, WeekDayAbbreviationFont);
				}
				return weekDayAppearanceDefault;
			}
		}
		protected internal virtual AppearanceDefault WeekNumberAppearanceDefault {
			get {
				if(weekNumberAppearanceDefault == null)
					weekNumberAppearanceDefault = new AppearanceDefault(WeekNumberForeColor, Color.Empty, WeekNumberTextAlignment, VertAlignment.Center, WeekNumberFont);
				return weekNumberAppearanceDefault;
			}
		}
		protected CalendarAppearances AppearanceCalendar { get { return Calendar.CalendarAppearance; } }
		AppearanceObject calendarHeaderAppearance;
		public virtual AppearanceObject CalendarHeaderAppearance {
			get {
				if(calendarHeaderAppearance == null) {
					calendarHeaderAppearance = new AppearanceObject();
					AppearanceHelper.Combine(calendarHeaderAppearance, new AppearanceObject[] { AppearanceCalendar.CalendarHeader }, CalendarHeaderAppearanceDefault);
				}
				return calendarHeaderAppearance;
			}
		}
		AppearanceObject buttonAppearance;
		public virtual AppearanceObject ButtonAppearance {
			get {
				if(buttonAppearance == null) {
					buttonAppearance = new AppearanceObject();
					AppearanceHelper.Combine(buttonAppearance, new AppearanceObject[] { AppearanceCalendar.Button }, ButtonAppearanceDefault);
				}
				return buttonAppearance;
			}
		}
		AppearanceObject highlightButtonAppearance;
		public virtual AppearanceObject HighlightButtonAppearance {
			get {
				if(highlightButtonAppearance == null) {
					highlightButtonAppearance = new AppearanceObject();
					AppearanceHelper.Combine(highlightButtonAppearance, new AppearanceObject[] { AppearanceCalendar.ButtonHighlighted }, HighlightButtonAppearanceDefault);
				}
				return highlightButtonAppearance;
			}
		}
		AppearanceObject pressedButtonAppearance;
		public virtual AppearanceObject PressedButtonAppearance {
			get {
				if(pressedButtonAppearance == null) {
					pressedButtonAppearance = new AppearanceObject();
					AppearanceHelper.Combine(pressedButtonAppearance, new AppearanceObject[] { AppearanceCalendar.ButtonPressed }, PressedButtonAppearanceDefault);
				}
				return pressedButtonAppearance;
			}
		}
		AppearanceObject headerAppearance;
		public virtual AppearanceObject HeaderAppearance {
			get {
				if(headerAppearance == null) {
					headerAppearance = new AppearanceObject();
					AppearanceHelper.Combine(headerAppearance, new AppearanceObject[] { AppearanceCalendar.Header }, HeaderAppearanceDefault);
				}
				return headerAppearance;
			}
		}
		AppearanceObject highlightHeaderAppearance;
		public virtual AppearanceObject HighlightHeaderAppearance {
			get {
				if(highlightHeaderAppearance == null) {
					highlightHeaderAppearance = new AppearanceObject();
					AppearanceHelper.Combine(highlightHeaderAppearance, new AppearanceObject[] { AppearanceCalendar.HeaderHighlighted }, HighlightHeaderAppearanceDefault);
				}
				return highlightHeaderAppearance;
			}
		}
		AppearanceObject pressedHeaderAppearance;
		public virtual AppearanceObject PressedHeaderAppearance {
			get {
				if(pressedHeaderAppearance == null) {
					pressedHeaderAppearance = new AppearanceObject();
					AppearanceHelper.Combine(pressedHeaderAppearance, new AppearanceObject[] { AppearanceCalendar.HeaderPressed }, PressedHeaderAppearanceDefault);
				}
				return pressedHeaderAppearance;
			}
		}
		AppearanceObject dayCellAppearance;
		public virtual AppearanceObject DayCellAppearance {
			get {
				if(dayCellAppearance == null) {
					dayCellAppearance = new AppearanceObject();
					AppearanceHelper.Combine(dayCellAppearance, new AppearanceObject[] { AppearanceCalendar.DayCell }, DayCellAppearanceDefault);
				}
				return dayCellAppearance;
			}
		}
		AppearanceObject dayCellSpecialAppearance;
		public virtual AppearanceObject DayCellSpecialAppearance {
			get {
				if(dayCellSpecialAppearance == null) {
					dayCellSpecialAppearance = new AppearanceObject();
					AppearanceHelper.Combine(dayCellSpecialAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellSpecial }, DayCellSpecialAppearanceDefault);
				}
				return dayCellSpecialAppearance;
			}
		}
		AppearanceObject holidayCellAppearance;
		public virtual AppearanceObject HolidayCellAppearance {
			get {
				if(holidayCellAppearance == null) {
					holidayCellAppearance = new AppearanceObject();
					AppearanceHelper.Combine(holidayCellAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellHoliday }, HolidayCellAppearanceDefault);
				}
				return holidayCellAppearance;
			}
		}
		AppearanceObject inactiveDayCellAppearance;
		public virtual AppearanceObject InactiveDayCellAppearance {
			get {
				if(inactiveDayCellAppearance == null) {
					inactiveDayCellAppearance = new AppearanceObject();
					AppearanceHelper.Combine(inactiveDayCellAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellInactive }, InactiveDayCellAppearanceDefault);
				}
				return inactiveDayCellAppearance;
			}
		}
		AppearanceObject todayDayCellAppearance;
		public virtual AppearanceObject TodayCellAppearance {
			get {
				if(todayDayCellAppearance == null) {
					todayDayCellAppearance = new AppearanceObject();
					AppearanceHelper.Combine(todayDayCellAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellToday }, TodayCellAppearanceDefault);
				}
				return todayDayCellAppearance;
			}
		}
		AppearanceObject selectedDayCellAppearance;
		public virtual AppearanceObject SelectedDayCellAppearance {
			get {
				if(selectedDayCellAppearance == null) {
					selectedDayCellAppearance = new AppearanceObject();
					AppearanceHelper.Combine(selectedDayCellAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellSelected }, SelectedDayCellAppearanceDefault);
				}
				return selectedDayCellAppearance;
			}
		}
		AppearanceObject selectedDayCellSpecialAppearance;
		public virtual AppearanceObject SelectedDayCellSpecialAppearance {
			get {
				if(selectedDayCellSpecialAppearance == null) {
					selectedDayCellSpecialAppearance = new AppearanceObject();
					AppearanceHelper.Combine(selectedDayCellSpecialAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellSpecialSelected }, SelectedDayCellSpecialAppearanceDefault);
				}
				return selectedDayCellSpecialAppearance;
			}
		}
		AppearanceObject inactiveDayCellSpecialAppearance;
		public virtual AppearanceObject InactiveDayCellSpecialAppearance {
			get {
				if(inactiveDayCellSpecialAppearance == null) {
					inactiveDayCellSpecialAppearance = new AppearanceObject();
					AppearanceHelper.Combine(inactiveDayCellSpecialAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellSpecialInactive }, InactiveDayCellSpecialAppearanceDefault);
				}
				return inactiveDayCellSpecialAppearance;
			}
		}
		AppearanceObject disabledDayCellSpecialAppearance;
		public virtual AppearanceObject DisabledDayCellSpecialAppearance {
			get {
				if(disabledDayCellSpecialAppearance == null) {
					disabledDayCellSpecialAppearance = new AppearanceObject();
					AppearanceHelper.Combine(disabledDayCellSpecialAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellSpecialDisabled }, DisabledDayCellSpecialAppearanceDefault);
				}
				return disabledDayCellSpecialAppearance;
			}
		}
		AppearanceObject highlightDayCellAppearance;
		public virtual AppearanceObject HighlightDayCellAppearance {
			get {
				if(highlightDayCellAppearance == null) {
					highlightDayCellAppearance = new AppearanceObject();
					AppearanceHelper.Combine(highlightDayCellAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellHighlighted }, HighlightDayCellAppearanceDefault);
				}
				return highlightDayCellAppearance;
			}
		}
		AppearanceObject highlightDayCellSpecialAppearance;
		public virtual AppearanceObject HighlightDayCellSpecialAppearance {
			get {
				if(highlightDayCellSpecialAppearance == null) {
					highlightDayCellSpecialAppearance = new AppearanceObject();
					AppearanceHelper.Combine(highlightDayCellSpecialAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellSpecialHighlighted }, HighlightDayCellSpecialAppearanceDefault);
				}
				return highlightDayCellSpecialAppearance;
			}
		}
		AppearanceObject pressedDayCellAppearance;
		public virtual AppearanceObject PressedDayCellAppearance {
			get {
				if(pressedDayCellAppearance == null) {
					pressedDayCellAppearance = new AppearanceObject();
					AppearanceHelper.Combine(pressedDayCellAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellPressed }, PressedDayCellAppearanceDefault);
				}
				return pressedDayCellAppearance;
			}
		}
		AppearanceObject pressedDayCellSpecialAppearance;
		public virtual AppearanceObject PressedDayCellSpecialAppearance {
			get {
				if(pressedDayCellSpecialAppearance == null) {
					pressedDayCellSpecialAppearance = new AppearanceObject();
					AppearanceHelper.Combine(pressedDayCellSpecialAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellSpecialPressed }, PressedDayCellSpecialAppearanceDefault);
				}
				return pressedDayCellSpecialAppearance;
			}
		}
		AppearanceObject disabledDayCellAppearance;
		public virtual AppearanceObject DisabledDayCellAppearance {
			get {
				if(disabledDayCellAppearance == null) {
					disabledDayCellAppearance = new AppearanceObject();
					AppearanceHelper.Combine(disabledDayCellAppearance, new AppearanceObject[] { AppearanceCalendar.DayCellDisabled }, DisabledDayCellAppearanceDefault);
				}
				return disabledDayCellAppearance;
			}
		}
		AppearanceObject weekDayAppearance;
		public virtual AppearanceObject WeekDayAppearance {
			get {
				if(weekDayAppearance == null) {
					weekDayAppearance = new AppearanceObject();
					AppearanceHelper.Combine(weekDayAppearance, new AppearanceObject[] { AppearanceCalendar.WeekDay }, WeekDayAppearanceDefault);
				}
				return weekDayAppearance;
			}
		}
		AppearanceObject weekNumberAppearance;
		public virtual AppearanceObject WeekNumberAppearance {
			get {
				if(weekNumberAppearance == null) {
					weekNumberAppearance = new AppearanceObject();
					AppearanceHelper.Combine(weekNumberAppearance, new AppearanceObject[] { AppearanceCalendar.WeekNumber }, WeekNumberAppearanceDefault);
				}
				return weekNumberAppearance;
			}
		}
		public virtual void ResetAppearanceDefault() {
			this.calendarHeaderAppearanceDefault = null;
			this.buttonAppearanceDefault = null;
			this.buttonHighlightedAppearanceDefault = null;
			this.buttonPressedAppearanceDefault = null;
			this.headerAppearanceDefault = null;
			this.highlightHeaderAppearanceDefault = null;
			this.pressedHeaderAppearanceDefault = null;
			this.dayCellAppearanceDefault = null;
			this.highlightDayCellAppearanceDefault = null;
			this.inactiveDayCellAppearanceDefault = null;
			this.pressedDayCellAppearanceDefault = null;
			this.disabledDayCellAppearanceDefault = null;
			this.todayDayCellAppearanceDefault = null;
			this.weekDayAppearanceDefault = null;
			this.selectedDayCellAppearanceDefault = null;
			this.weekNumberAppearanceDefault = null;
			this.highlightDayCellAppearanceDefault = null;
			this.dayCellSpecialAppearanceDefault = null;
			this.highlightedDayCellSpecialAppearanceDefault = null;
			this.pressedDayCellSpecialAppearanceDefault = null;
			this.selectedDayCellSpecialAppearanceDefault = null;
			this.inactiveDayCellSpecialAppearanceDefault = null;
			this.disabledDayCellSpecialAppearanceDefault = null;
		}
		public virtual void UpdatePaintAppearance() {
			this.calendarHeaderAppearance = null;
			this.buttonAppearance = null;
			this.highlightButtonAppearance = null;
			this.pressedButtonAppearance = null;
			this.headerAppearance = null;
			this.highlightHeaderAppearance = null;
			this.pressedHeaderAppearance = null;
			this.dayCellAppearance = null;
			this.disabledDayCellAppearance = null;
			this.weekDayAppearance = null;
			this.weekNumberAppearance = null;
			this.todayDayCellAppearance = null;
			this.selectedDayCellAppearance = null;
			this.inactiveDayCellAppearance = null;
			this.highlightDayCellAppearance = null;
			this.pressedDayCellAppearance = null;
			this.dayCellSpecialAppearance = null;
			this.highlightDayCellSpecialAppearance = null;
			this.pressedDayCellSpecialAppearance = null;
			this.selectedDayCellSpecialAppearance = null;
			this.inactiveDayCellSpecialAppearance = null;
			this.disabledDayCellSpecialAppearance = null;
		}
		#endregion
		public virtual Color WeekNumbersSeparatorColor { get { return SystemColors.ControlLight; } }
		public virtual Color WeekDaySeparatorColor { get { return SystemColors.ControlLight; } }
		public virtual Color TodayFrameColor { get { return Color.Red; } }
		public bool IsSkinned { get { return LookAndFeel.ActiveLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin; } }
		public ISkinProvider SkinProvider { get { return LookAndFeel.ActiveLookAndFeel; } }
	}
	public class CalendarPaintStyleSkin : CalendarPaintStyle {
		public CalendarPaintStyleSkin(CalendarViewInfoBase viewInfo) : base(viewInfo) { }
		protected internal override CalendarCellPainter DayCellPainter {
			get { return new SkinCalendarCellPainter(); }
		}
		public override Color TodayFrameColor {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarTodayFrameColor, base.TodayFrameColor);
			}
		}
		protected internal virtual Color GetCellColor(string name, Color defaultColor) {
			if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) 
				return defaultColor;
			return EditorsSkins.GetSkin(LookAndFeel).Colors.GetColor(name, defaultColor);
		}
		protected override Color DayCellForeColor {
			get { return GetCellColor(EditorsSkins.SkinCalendarNormalCellColor, base.DayCellForeColor); }
		}
		protected override Color DayCellForeColorHighlight {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarHighlightedCellColor, base.DayCellForeColorHighlight);
			}
		}
		protected override Color DayCellForeColorPressed {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarPressedCellColor, base.DayCellForeColorHighlight);
			}
		}
		protected override Color DayCellForeColorSelected {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarSelectedCellColor, base.DayCellForeColorSelected);
			}
		}
		protected override Color DayCellForeColorDisabled {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarDisabledCellColor, base.DayCellForeColorDisabled);
			}
		}
		protected override Color DayCellForeColorInactive {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarInactiveCellColor, base.DayCellForeColorInactive);
			}
		}
		protected override Color DayCellSpecialForeColor {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarSpecialCellColor, base.DayCellSpecialForeColor);
			}
		}
		protected override Color DayCellSpecialForeColorHighlight {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarHighlightedSpecialCellColor, base.DayCellSpecialForeColorHighlight);
			}
		}
		protected override Color DayCellSpecialForeColorPressed {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarPressedSpecialCellColor, base.DayCellSpecialForeColorPressed);
			}
		}
		protected override Color DayCellSpecialForeColorSelected {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarSelectedSpecialCellColor, base.DayCellSpecialForeColorSelected);
			}
		}
		protected override Color DayCellSpecialForeColorInactive {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarInactiveSpecialCellColor, base.DayCellSpecialForeColorInactive);
			}
		}
		protected override Color DayCellSpecialForeColorDisabled {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarDisabledSpecialCellColor, base.DayCellSpecialForeColorDisabled);
			}
		}
		protected override Color DayCellForeColorToday {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarTodayCellColor, base.DayCellForeColorToday);
			}
		}
		protected override Color DayCellForeColorHoliday {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarHolidayCellColor, base.DayCellForeColorHoliday);
			}
		}
		protected override Color WeekDayForeColor {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarNormalCellColor, base.WeekDayForeColor);
			}
		}
		protected override Color WeekNumberForeColor {
			get {
				return GetCellColor(EditorsSkins.SkinCalendarNormalCellColor, base.WeekNumberForeColor);
			}
		}
		protected override Color HeaderForeColor {
			get {
				return GetCellColor("VistaDateEditHeaderForeColor", base.HeaderForeColor);
			}
		}
	}
	public class VistaCalendarPaintStyle : CalendarPaintStyle {
		public VistaCalendarPaintStyle(CalendarViewInfoBase viewInfo) : base(viewInfo) { }
		protected override HorzAlignment DayCellTextAlignment {
			get {
				if(Calendar.View == DateEditCalendarViewType.MonthInfo)
					return HorzAlignment.Far;
				if(Calendar.View == DateEditCalendarViewType.YearsGroupInfo)
					return HorzAlignment.Near;
				return HorzAlignment.Center; 
			}
		}   
	}
	public class VistaCalendarPaintStyleSkin : CalendarPaintStyleSkin {
		public VistaCalendarPaintStyleSkin(CalendarViewInfoBase viewInfo) : base(viewInfo) { }
		protected override HorzAlignment DayCellTextAlignment {
			get {
				if(Calendar.View == DateEditCalendarViewType.MonthInfo)
					return HorzAlignment.Far;
				if(Calendar.View == DateEditCalendarViewType.YearsGroupInfo)
					return HorzAlignment.Near;
				return HorzAlignment.Center; 
			}
		}
	}
	public class DateRange {
		protected internal DateRangeCollection Collection { get; set; }
		public DateRange() { }
		public DateRange(DateTime dateTime) {
			this.startDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
			this.endDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(1);
		}
		public DateRange(DateTime startDate, DateTime endDate) {
			if(startDate > endDate) {
				this.startDate = endDate;
				this.endDate = startDate;
			}
			else {
				this.startDate = startDate;
				this.endDate = endDate;
			}
		}
		DateTime startDate;
		public DateTime StartDate {
			get { return startDate; }
			set {
				if(StartDate == value)
					return;
				startDate = value;
				OnRangeChanged();
			}
		}
		protected virtual void OnRangeChanged() {
			if(Collection != null)
				Collection.OnSelectionChanged();
		}
		DateTime endDate;
		public DateTime EndDate {
			get { return endDate; }
			set {
				if(EndDate == value)
					return;
				endDate = value;
				OnRangeChanged();
			}
		}
		public bool Contains(DateRange range) {
			return range.StartDate >= StartDate && range.EndDate <= EndDate;
		}
		public override bool Equals(object obj) {
			if(!(obj is DateRange))
				return false;
			DateRange range = (DateRange)obj;
			return range.StartDate == StartDate && range.EndDate == EndDate;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return StartDate.ToShortDateString() + " - " + EndDate.ToShortDateString();
		}
	}
	public class DateRangeCollection : Collection<DateRange> {
		public DateRangeCollection(CalendarControlBase calendar) {
			Calendar = calendar;
		}
		public CalendarControlBase Calendar { get; private set; }
		public bool IsDateSelected(DateTime date) {
			DateTime endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, date.Kind);
			foreach(DateRange range in this) {
				if(range.StartDate <= date && range.EndDate >= endDate)
					return true;
			}
			return false;
		}
		protected int UpdateCount { get; set; }
		public bool IsLockUpdate { get { return UpdateCount > 0; } }
		public void BeginUpdate() {
			UpdateCount++;
		}
		public void CancelUpdate() {
			if(UpdateCount > 0)
				UpdateCount--;
		}
		public void EndUpdate() {
			CancelUpdate();
			if(UpdateCount == 0)
				OnSelectionChanged();
		}
		protected internal virtual void OnSelectionChanged() {
			if(IsLockUpdate)
				return;
			Calendar.OnSelectionChanged();
		}
		protected override void InsertItem(int index, DateRange item) {
			base.InsertItem(index, item);
			item.Collection = this;
			OnSelectionChanged();
		}
		protected override void RemoveItem(int index) {
			DateRange item = this[index];
			base.RemoveItem(index);
			item.Collection = null;
			OnSelectionChanged();
		}
		protected override void ClearItems() {
			BeginUpdate();
			try {
				base.ClearItems();
			}
			finally {
				EndUpdate();
			}
		}
		protected override void SetItem(int index, DateRange item) {
			base.SetItem(index, item);
			OnSelectionChanged();
		}
		public bool IsIntersect(DateTime start, DateTime end) {
			if(IsDateSelected(start) || IsDateSelected(end))
				return true;
			foreach(DateRange range in this) {
				if(range.StartDate >= start && range.StartDate <= end)
					return true;
				if(range.EndDate >= start && range.EndDate <= end)
					return true;
			}
			return false;
		}
		public virtual DateTime Start { 
			get {
				DateTime min = DateTime.MaxValue;
				foreach(DateRange range in this) {
					if(range.StartDate < min)
						min = range.StartDate;
				}
				return min;
			} 
		}
		public virtual DateTime End { 
			get {
				DateTime max = DateTime.MinValue;
				foreach(DateRange range in this) {
					if(range.EndDate > max)
						max = range.EndDate;
				}
				return max;
			} 
		}
		public bool Contains(DateTime date) {
			foreach(DateRange range in this) {
				if(range.StartDate <= date && range.EndDate >= date)
					return true;
			}
			return false;
		}
	}
	public interface ICalendarClickableObject {
		void OnClick();
	}
	public class CalendarControlHandlerBase {
		public CalendarControlHandlerBase(CalendarControlBase calendar) {
			Calendar = calendar;
			this.selectedDate = calendar.DateTime;
		}
		public virtual CalendarControlBase Calendar { get; private set; }
		protected internal virtual bool ShouldUpdateOnDateChange { get; internal set; }
		DateTime selectedDate = DateTime.Today;
		public void SetSelectedDateCore(DateTime dateTime) {
			if(SuppressChangeSelectedDate)
				return;
			this.selectedDate = dateTime;
		}
		public DateTime SelectedDate {
			get { return selectedDate; }
			set {
				if(SuppressChangeSelectedDate)
					return;
				if(SelectedDate == value)
					return;
				selectedDate = value;
				OnSelectedDateChanged();
			}
		}
		bool SuppressChangeSelectedDate { get; set; }
		protected virtual void OnSelectedDateChanged() {
			if(Calendar.ReadOnly)
				return;
			SuppressChangeSelectedDate = true;
			try {
				if(Calendar.UpdateDateTimeWhenNavigating)
					Calendar.DateTime = SelectedDate;
				else
					Calendar.OnEditValueChanged();
			}
			finally {
				SuppressChangeSelectedDate = false;
			}
		}
		public virtual void OnMouseEnter(EventArgs e) { }
		public virtual void OnMouseLeave(EventArgs e) { }
		public virtual void OnMouseDown(MouseEventArgs e) { }
		public virtual void OnMouseUp(MouseEventArgs e) { }
		public virtual void OnMouseMove(MouseEventArgs e) { }
		public virtual void OnKeyDown(KeyEventArgs e) { }
		public virtual void OnKeyUp(KeyEventArgs e) { }
		public virtual void OnDoubleClick(EventArgs e) { }
	}
	public class CalendarSelectionManager {
		public CalendarSelectionManager(CalendarControlBase calendar) {
			Calendar = calendar;
		}
		public CalendarControlBase Calendar { get; private set; }
		public CalendarViewInfoBase ViewInfo { get { return Calendar.ViewInfo; } }
		protected virtual CalendarCellViewInfo GetFirstActiveVisibleCellInfo() { return ViewInfo.GetFirstActiveVisibleCellInfo(); }
		protected CalendarCellViewInfo SelectionStartCell { get; set; }
		protected DateTime GetEndDateTime(DateTime date) {
			return new DateTime(date.Year, date.Month, date.Day).AddDays(1);
		}
		public virtual void AddSelection(DateRange range) {
			range = AdjustRange(range);
			if(ContainsRange(range))
				return;
			CheckRemoveContainedRange(range);
			Calendar.SelectedRanges.Add(range);			
		}
		protected virtual DateRange AdjustRange(DateRange range) {
			if(Calendar.SelectionBehavior == CalendarSelectionBehavior.Simple)
				return AdjustRangeSimple(range);
			if(Calendar.SelectionBehavior == CalendarSelectionBehavior.OutlookStyle)
				return AdjustRangeOutlookStyle(range);
			return range;
		}
		protected virtual DateTime AdjustSelectionStart(DateTime start, DateTime end) {
			TimeSpan weekSpan = TimeSpan.FromDays(6);
			DayOfWeek actualFirstDayOfWeek = Calendar.FirstDayOfWeek;
			if (end >= start) {
				if (end - start < weekSpan)
					return start;
				else {
					return CalendarControlBase.GetStartOfWeekCore(start, actualFirstDayOfWeek);
				}
			} else {
				if (start - end < weekSpan)
					return start;
				else
					return CalendarControlBase.GetStartOfWeekCore(start, actualFirstDayOfWeek) + weekSpan;
			}
		}
		protected virtual DateTime AdjustSelectionEnd(DateTime start, DateTime end) {
			TimeSpan weekSpan = TimeSpan.FromDays(7);
			DayOfWeek actualFirstDayOfWeek = Calendar.FirstDayOfWeek;
			if (end >= start) {
				if (end - start < weekSpan)
					return end;
				DateTime adjustedEnd = CalendarControlBase.GetStartOfWeekCore(end, actualFirstDayOfWeek);
				if (end != adjustedEnd)
					return adjustedEnd + weekSpan;
				return end;
			} else {
				if (start - end < weekSpan)
					return end;
				return CalendarControlBase.GetStartOfWeekCore(end, actualFirstDayOfWeek);
			}
		}
		protected virtual DateRange AdjustRangeOutlookStyle(DateRange range) {
			DateTime startDate = AdjustSelectionStart(range.StartDate, range.EndDate);
			DateTime endDate = AdjustSelectionEnd(range.StartDate, range.EndDate);
			return new DateRange(startDate, endDate);
		}
		protected virtual DateRange AdjustRangeSimple(DateRange range) {
			return range;
		}
		private void CheckRemoveContainedRange(DateRange range) {
			for(int rangeIndex = 0; rangeIndex < Calendar.SelectedRanges.Count; ) {
				if(range.Contains(Calendar.SelectedRanges[rangeIndex])) {
					Calendar.SelectedRanges.BeginUpdate();
					Calendar.SelectedRanges.RemoveAt(rangeIndex);
					Calendar.SelectedRanges.CancelUpdate();
				}
				else
					rangeIndex++;
			}
		}
		protected internal virtual bool ContainsRange(DateRange range) {
			foreach(DateRange r in Calendar.SelectedRanges) {
				if(r.Contains(range))
					return true;
			}
			return false;
		}
		protected internal DateRangeCollection Selection { get { return Calendar.SelectedRanges; } }
		protected bool IsMultiselect { get { return Calendar.SelectionMode == CalendarSelectionMode.Multiple; } }
		DateRange LastRangeSelectedByMouse { get; set; }
		public virtual void UpdateSelectionByMouse(DateRange range) {
			Calendar.SelectedRanges.BeginUpdate();
			try {
				if((!IsControlKeyPressed && !IsShiftKeyPressed) || !IsMultiselect)
					Calendar.SelectedRanges.Clear();
				if(LastRangeSelectedByMouse != null)
					Calendar.SelectedRanges.Remove(LastRangeSelectedByMouse);
				AddSelection(range);
				LastRangeSelectedByMouse = range;
			}
			finally {
				Calendar.SelectedRanges.EndUpdate();
			}
		}
		public virtual void UpdateSelection(CalendarCellViewInfo cell) {
			if(cell == null) {
				Calendar.SelectedRanges.Clear();
				return;
			}
			Calendar.SelectedRanges.BeginUpdate();
			try {
				if(!IsControlKeyPressed || !IsMultiselect)
					Calendar.SelectedRanges.Clear();
				if(IsShiftKeyPressed && IsMultiselect) {
					if(SelectionStartCell == null)
						SelectionStartCell = GetFirstActiveVisibleCellInfo();
					if(SelectionStartCell != null && cell != null)
						AddSelection(new DateRange(SelectionStartCell.Date.Date, GetEndDateTime(cell.Date.Date)));
				}
				else {
					AddSelection(new DateRange(cell.Date));
				}
			}
			finally {
				if(!IsShiftKeyPressed || !IsMultiselect)
					SelectionStartCell = cell;
				Calendar.SelectedRanges.EndUpdate();
			}
		}
		protected internal virtual bool IsShiftKeyPressed {
			get { return Control.ModifierKeys.HasFlag(Keys.Shift); }
		}
		protected internal virtual bool IsControlKeyPressed {
			get {
				return Control.ModifierKeys.HasFlag(Keys.Control);
			}
		}
		public virtual void SetSelection(DateRange range) {
			if(SuppressSetSelection)
				return;
			Selection.BeginUpdate();
			try {
				Selection.Clear();
				AdjustRange(range);
				Selection.Add(range);
			}
			finally {
				Selection.EndUpdate();
			}
		}
		protected internal bool SuppressSetSelection { get; set; }
		protected Point DownPoint { get; set; }
		protected CalendarHitInfo DownInfo { get; set; }
		protected Point StartNavigationPoint { get; set; }
		protected DateTime DownStartDateTime { get; set; }
		public bool IsMarqueeSelection { get; private set; }
		public virtual void OnMouseDown(MouseEventArgs e) {
			if(Calendar.IsDesignMode || Calendar.ReadOnly)
				return;
			if(e.Button != MouseButtons.Left)
				return;
			ViewInfo.SuppressStateChangeAnimation = true;
			LastRangeSelectedByMouse = null;
			DownPoint = e.Location;
			DownInfo = Calendar.GetHitInfo(e);
			EndCell = DownInfo.Cell;
			DownStartDateTime = Calendar.StartDate;
			StartNavigationPoint = NavigationGrid.GetPosition(DownInfo.Cell);
			if(!Calendar.ReadOnly && DownInfo.IsInCell && Calendar.View == DateEditCalendarViewType.MonthInfo)
				UpdateSelection(DownInfo.Cell);
			IsMarqueeSelection = false;
		}
		public virtual void Clear() {
			ViewInfo.SuppressStateChangeAnimation = false;
			DownPoint = Point.Empty;
			EndCell = null;
			LastRangeSelectedByMouse = null;
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			if(Calendar.IsDesignMode || Calendar.ReadOnly)
				return;
			Clear();
		}
		protected Rectangle GetSelectionBounds(Point pt1, Point pt2) {
			return new Rectangle(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y), Math.Abs(pt1.X - pt2.X), Math.Abs(pt1.Y - pt2.Y));
		}
		public CalendarCellViewInfo EndCell { get; private set; }
		public virtual void OnMouseMove(MouseEventArgs e) {
			if(Calendar.IsDesignMode || Calendar.ReadOnly)
				return;
			if(e.Button != MouseButtons.Left || DownInfo == null || !DownInfo.IsInCell) 
				return;
			if(Calendar.SelectionMode == CalendarSelectionMode.Single) {
				CalendarHitInfo hitInfo = Calendar.GetHitInfo(e.Location);
				if(hitInfo.IsInCell && !hitInfo.Cell.IsDisabled)
					UpdateSelection(hitInfo.Cell);
				return;
			}
			if(!IsMarqueeSelection && !ShouldStartMarqueeSelection(e.Location))
				return;
			IsMarqueeSelection = true;
			CalendarCellViewInfo startCell = DownInfo.Cell;
			CalendarCellViewInfo endCell = CalcEndCell(e);
			EndCell = endCell;
			if(endCell == null)
				return;
			DateTime startDate = startCell.Date < endCell.Date ? startCell.Date : endCell.Date;
			DateTime endDate = startCell.Date < endCell.Date ? endCell.Date : startCell.Date;
			endDate = endDate.AddDays(1);
			DateRange range = new DateRange(startDate.Date, endDate.Date);
			UpdateSelectionByMouse(range);
		}
		protected double MarqueeSelectionDelta { get { return 3.0f; } }
		protected virtual bool ShouldStartMarqueeSelection(Point point) {
			Point pt = new Point(point.X - DownPoint.X, point.Y - DownPoint.Y);
			return Math.Sqrt(pt.X * pt.X + pt.Y * pt.Y) > MarqueeSelectionDelta;
		}
		protected CalendarNavigationGrid NavigationGrid { get { return ViewInfo.NavigationGrid; } }
		protected virtual CalendarCellViewInfo CalcEndCell(MouseEventArgs e) {
			CalendarHitInfo hitInfo = Calendar.GetHitInfo(e);
			if(hitInfo.IsInCell)
				return hitInfo.Cell;
			int xDirection = Math.Sign(e.Location.X - DownPoint.X);
			int yDirection = Math.Sign(e.Location.Y - DownPoint.Y);
			if(xDirection == 0) xDirection = 1;
			if(yDirection == 0) yDirection = 1;
			Point start = CalcStartCell();
			Point end = new Point(-1,-1);
			for(int row = start.X; row >= 0 && row < NavigationGrid.RowCount; row += yDirection) {
				if(NavigationGrid.RowsBounds[row].IsEmpty)
					continue;
				if((yDirection < 0 && NavigationGrid.RowsBounds[row].Bottom < e.Location.Y) ||
					(yDirection > 0 && NavigationGrid.RowsBounds[row].Top > e.Location.Y))
					break;
				for(int column = start.Y; column >= 0 && column < NavigationGrid.ColumnCount; column += xDirection) {
					if((xDirection < 0 && NavigationGrid.ColumnBounds[column].Right < e.X) ||
						(xDirection > 0 && NavigationGrid.ColumnBounds[column].Left > e.X))
						break;
					end = new Point(row, column);
				}
			}
			if(end.X == -1 || end.Y == -1)
				return null;
			if(NavigationGrid[end.X, end.Y] == null) {
				xDirection = -xDirection;
				for(int column = end.Y; column >= 0 && column < NavigationGrid.ColumnCount; column += xDirection) {
					if(NavigationGrid[end.X, column] != null)
						return NavigationGrid[end.X, column];
				}
			}
			return NavigationGrid[end.X, end.Y];
		}
		private Point CalcStartCell() {
			if(StartNavigationPoint.X == -1 || StartNavigationPoint.Y == -1 || DownStartDateTime == Calendar.StartDate) {
				return StartNavigationPoint;
			}
			if(DownStartDateTime < Calendar.StartDate)
				return new Point(StartNavigationPoint.X, 0);
			else if(DownStartDateTime > Calendar.StartDate)
				return new Point(StartNavigationPoint.X, NavigationGrid.ColumnCount - 1);
			return StartNavigationPoint;
		}
	}
	public class CalendarControlHandler : CalendarControlHandlerBase {
		public CalendarControlHandler(CalendarControlBase calendar) : base(calendar) {
			ShouldUpdateOnDateChange = true;
		}
		public new CalendarControl Calendar { get { return (CalendarControl)base.Calendar; } }
		public DateEditCalendarViewType View { get { return Calendar.View; } }
		public bool ReadOnly { get { return Calendar.ReadOnly; } }
		public CalendarViewInfoBase ViewInfo { get { return Calendar.ViewInfo; } }
		public bool IsRightToLeft { get { return WindowsFormsSettings.GetIsRightToLeft(Calendar); } }
		protected CalendarSelectionManager SelectionManager { get { return Calendar.SelectionManager; } }
		public override void OnMouseEnter(EventArgs e) { }
		public override void OnMouseLeave(EventArgs e) { }
		protected CalendarCellViewInfo DownCell { get; set; }
		public override void OnMouseDown(MouseEventArgs e) {
			if(ViewInfo.IsAnimated)
				return;
			if(e.Button == MouseButtons.Left) {
				ViewInfo.PressedInfo = ViewInfo.CalcHitInfo(e.Location);
				DownCell = ViewInfo.PressedInfo.Cell;
				if(ViewInfo.PressedInfo.IsInCell) 
					if(ViewInfo.PressedInfo.Cell.ContextButtonsHandler.OnMouseDown(e))
						return;
				SelectionManager.OnMouseDown(e);
			}
		}
		public override void OnMouseUp(MouseEventArgs e) {
			bool stopScrolling = StopScrolling();
			CalendarHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(hitInfo.IsInCell && hitInfo.Cell.ContextButtonsHandler.OnMouseUp(e)) {
				SelectionManager.Clear();
				ViewInfo.PressedInfo = new CalendarHitInfo(Point.Empty);
				return;
			}
			CalendarCellViewInfo endCell = SelectionManager.EndCell;
			SelectionManager.OnMouseUp(e);
			if(ViewInfo.IsAnimated || stopScrolling) {
				ViewInfo.PressedInfo = new CalendarHitInfo(Point.Empty);
				return;
			}
			if(CanProcessCellClick(hitInfo.Cell)) {
				ViewInfo.PressedInfo = new CalendarHitInfo(Point.Empty);
				ProcessCellClick(hitInfo);
				SelectionManager.Clear();
				return;
			}
			if(hitInfo.HitTest == CalendarHitInfoType.Unknown && Calendar.SelectionMode == CalendarSelectionMode.Multiple) {
				if(endCell != null)
					ProcessCellClick(endCell);
			}
			else if(hitInfo.IsEquals(ViewInfo.PressedInfo)) {
				ViewInfo.PressedInfo = new CalendarHitInfo(Point.Empty);
				ProcessClick(hitInfo);
			}
			SelectionManager.Clear();
			ViewInfo.PressedInfo = new CalendarHitInfo(Point.Empty);
		}
		protected virtual bool CanProcessCellClick(CalendarCellViewInfo cell) {
			if(cell == null) return false;
			if(Calendar.SelectionMode == CalendarSelectionMode.Single)
				return true;
			if(SelectionManager.IsControlKeyPressed || SelectionManager.IsShiftKeyPressed)
				return false;
			if(SelectionManager.IsMarqueeSelection)
				return false;
			return true;
		}
		protected virtual void ProcessClick(CalendarHitInfo hitInfo) {
			if(ViewInfo.SwitchState)
				return;
			ICalendarClickableObject clickable = hitInfo.HitObject as ICalendarClickableObject;
			if(clickable != null) {
				clickable.OnClick();
				return;
			}
			if(CanProcessCellClick(hitInfo.Cell)) {
				ProcessCellClick(hitInfo);
				return;
			}
		}
		protected internal virtual DateTime CorrectDateTime(DateTime dt) {
			DateTime offsetDate = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, SelectedDate.Hour, SelectedDate.Minute, SelectedDate.Second, SelectedDate.Millisecond);
			long offset = SelectedDate.Ticks - offsetDate.Ticks;
			long newThicks = (new DateTime(dt.Year, dt.Month, dt.Day, SelectedDate.Hour, SelectedDate.Minute, SelectedDate.Second, SelectedDate.Millisecond)).Ticks;
			return new DateTime(newThicks + offset, dt.Kind);
		}
		protected virtual int CorrectDay(int year, int month, int day) {
			return Math.Max(1, Math.Min(day,System.DateTime.DaysInMonth(year, month)));
		}
		protected virtual DateTime ExtractDateTimeFromCellDate(DateTime date) { 
			if(View == DateEditCalendarViewType.MonthInfo)
				return CorrectDateTime(new DateTime(date.Year, date.Month, CorrectDay(SelectedDate.Year, date.Month, date.Day), SelectedDate.Hour, SelectedDate.Minute, SelectedDate.Second, SelectedDate.Millisecond));
			else if(View == DateEditCalendarViewType.YearInfo)
				return CorrectDateTime(new DateTime(SelectedDate.Year, date.Month, CorrectDay(SelectedDate.Year, date.Month, SelectedDate.Day), SelectedDate.Hour, SelectedDate.Minute, SelectedDate.Second, SelectedDate.Millisecond));
			else if(View == DateEditCalendarViewType.YearsInfo)
				return CorrectDateTime(new DateTime(date.Year, SelectedDate.Month, CorrectDay(date.Year, SelectedDate.Month, SelectedDate.Day), SelectedDate.Hour, SelectedDate.Minute, SelectedDate.Second, SelectedDate.Millisecond));
			else if(View == DateEditCalendarViewType.YearsGroupInfo) {
				int year = date.Year == 1 ? 0 : date.Year;
				return CorrectDateTime(new DateTime(year + SelectedDate.Year % 10, SelectedDate.Month, CorrectDay(year + SelectedDate.Year % 10, SelectedDate.Month, SelectedDate.Day), SelectedDate.Hour, SelectedDate.Minute, SelectedDate.Second, SelectedDate.Millisecond));
			}
			return date;
		}
		protected internal virtual void ProcessCellClick(CalendarCellViewInfo cell) {
			if(cell.IsDisabled || (!cell.IsActive && !Calendar.AllowClickInactiveDays))
				return;
			SetEditDate(cell.Date, false);
		}
		protected internal virtual void ProcessCellClick(CalendarHitInfo hitInfo) {
			ProcessCellClick(hitInfo.Cell);
		}
		protected virtual void SetEditDate(DateTime date) {
			date = Calendar.FilterDate(date);
			SetEditDate(date, false);
		}
		protected virtual void SetEditDate(DateTime date, bool allowSetSelection) {
			DateTime newDate = ExtractDateTimeFromCellDate(date);
			if(View != Calendar.GetMinView()) {
				try {
					ShouldUpdateOnDateChange = false;
					SelectionManager.SuppressSetSelection = !allowSetSelection;
					SelectedDate = ExtractDateTimeFromCellDate(date);
				}
				finally {
					SelectionManager.SuppressSetSelection = false;
					ShouldUpdateOnDateChange = true;
					Calendar.DecView();
				}
				return;
			}
			bool shouldScrollCalendar = ShouldScrollCalendar(date);
			bool isLeftArrow = ScrollToLeft(date);
			ShouldUpdateOnDateChange = false;
			try {
				if(shouldScrollCalendar) {
					ViewInfo.SwitchType = isLeftArrow ? SwitchStateType.SwitchToLeft : SwitchStateType.SwitchToRight;
					ViewInfo.OnStateChanging(View, View, false);
				}
				SelectionManager.SuppressSetSelection = !allowSetSelection;
				SelectedDate = newDate;
				Calendar.OnDateTimeCommit(newDate);
				if(shouldScrollCalendar)
					ViewInfo.OnStateChanged(View, View, true);
			}
			finally {
				SelectionManager.SuppressSetSelection = false;
				ShouldUpdateOnDateChange = true;
			}
		}
		private void ScrollCalendarByMonth(CalendarHitInfo hitInfo) {
			if(ScrollToLeft(hitInfo.Cell.Date))
				OnArrowClick(-1, false);
			else
				OnArrowClick(+1, false);
		}
		private bool ScrollToLeft(DateTime date) {
			return date.Day < 7;
		}
		protected virtual bool ShouldScrollCalendar(DateTime date) {
			return date < ViewInfo.GetFirstActiveVisibleCellInfo().Date || date > ViewInfo.GetLastActiveVisibleCellInfo().Date;
		}
		protected internal virtual void OnTodayButtonClick(object sender, EventArgs e) {
			SetEditDate(Calendar.GetTodayDate(), true);
			Calendar.UpdateStartDateCore();
		}
		protected internal virtual void OnClearButtonClick(object sender, EventArgs e) {
			Calendar.OnDateTimeCommit(null);
			Calendar.ForceUpdateStartDate();
		}
		protected virtual bool ShouldChangeState(DateTime prev, DateTime current) {
			if(View == DateEditCalendarViewType.MonthInfo)
				return prev.Year != current.Year || prev.Month != current.Month;
			if(View == DateEditCalendarViewType.QuarterInfo)
				return prev.Year != current.Year;
			if(View == DateEditCalendarViewType.YearInfo)
				return prev.Year != current.Year;
			if(View == DateEditCalendarViewType.YearsInfo)
				return (prev.Year / 10) != (current.Year / 10);
			if(View == DateEditCalendarViewType.YearsGroupInfo)
				return (prev.Year / 100) != (current.Year / 100);
			return false;
		}
		protected internal virtual void OnMonthDecreaseButtonClick(object sender, EventArgs e) {
			OnArrowClick(IsRightToLeft ? +1 : -1, false);
		}
		protected internal virtual void OnMonthIncreaseButtonClick(object sender, EventArgs e) {
			OnArrowClick(IsRightToLeft ? -1 : +1, false);
		}
		protected virtual DateTime GetDateTimeAfterMonthNavigating(int dir) {
			return UpdateDateTime(0, dir, 0);
		}
		protected virtual DateTime GetDateTimeAfterYearNavigating(int dir) {
			return UpdateDateTime(dir, 0, 0);
		}
		protected virtual void NavigateNextMonthView(int dir) {
			ChangeSelectedDate(0, dir, 0, false);
		}
		protected virtual void NavigateNextYearView(int dir) {
			ChangeSelectedDate(dir, 0, 0, false);
		}
		protected internal virtual void ChangeSelectedDate(int yearOffset, int monthOffset, int daysOffset, bool updateLayout) {
			if(yearOffset == 0 && monthOffset == 0 && daysOffset == 0)
				return;
			try {
				ChangeSelectedDateCore(yearOffset, monthOffset, daysOffset, updateLayout);
			}
			catch {
			}
		}
		protected virtual DateTime GetTime() { return new DateTime(1, 1, 1); }
		protected virtual System.DateTime ApplyTime(DateTime date) {
			if(!Calendar.ShowTimeEdit)
				return date;
			DateTime time = GetTime();
			return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
		}
		protected internal virtual DateTime UpdateDateTime(DateTime dateTime, int yearOffset, int monthOffset, int daysOffset) {
			try {
				dateTime = dateTime.AddYears(yearOffset);
				dateTime = dateTime.AddMonths(monthOffset);
				dateTime = dateTime.AddDays(daysOffset);
				dateTime = ApplyTime(dateTime);
				dateTime = Calendar.FilterDate(dateTime);
			}
			catch(Exception) {
				if(yearOffset < 0 || monthOffset < 0 || daysOffset < 0)
					dateTime = Calendar.MinValue;
				else
					dateTime = Calendar.MaxValue;
			}
			return dateTime;
		}
		protected internal virtual DateTime UpdateDateTime(int yearOffset, int monthOffset, int daysOffset) {
			return UpdateDateTime(SelectedDate, yearOffset, monthOffset, daysOffset);
		}
		protected internal bool IsScrollingHitInfoType(CalendarHitInfoType infoType) {
			return (infoType == CalendarHitInfoType.DecMonth || infoType == CalendarHitInfoType.IncMonth ||
				infoType == CalendarHitInfoType.DecYear || infoType == CalendarHitInfoType.IncYear);
		}
		protected internal virtual void ChangeSelectedDate(CalendarHitInfoType infoType) {
			if(!IsScrollingHitInfoType(infoType)) return;
			if(infoType == CalendarHitInfoType.DecMonth) ChangeSelectedDate(0, -1, 0, true);
			if(infoType == CalendarHitInfoType.IncMonth) ChangeSelectedDate(0, 1, 0, true);
			if(infoType == CalendarHitInfoType.DecYear) ChangeSelectedDate(-1, 0, 0, true);
			if(infoType == CalendarHitInfoType.IncYear) ChangeSelectedDate(1, 0, 0, true);
		}
		protected virtual void ChangeSelectedDateCore(int yearOffset, int monthOffset, int daysOffset, bool updateLayout) {
			DateTime t = UpdateDateTime(yearOffset, monthOffset, daysOffset);
			DateTime startDate = UpdateDateTime(Calendar.StartDate, yearOffset, monthOffset, daysOffset);
			if(ReadOnly) {
				if(yearOffset == 0 && monthOffset == 0) return;
				if(updateLayout)
					Calendar.LayoutChanged();
			}
			else {
				if(Calendar.UpdateSelectionWhenNavigating)
					UpdateSelection(yearOffset, monthOffset, daysOffset);
				SelectedDate = t;
			}
			Calendar.UpdateStartDateCore(startDate);
		}
		protected virtual void UpdateSelection(int yearOffset, int monthOffset, int daysOffset) {
			Calendar.SelectedRanges.BeginUpdate();
			try {
				foreach(DateRange range in Calendar.SelectedRanges) {
					range.StartDate = UpdateDateTime(range.StartDate, yearOffset, monthOffset, daysOffset);
					range.EndDate = UpdateDateTime(range.EndDate, yearOffset, monthOffset, daysOffset);
				}
			}
			finally {
				Calendar.SelectedRanges.EndUpdate();
			}
		}
		protected internal virtual void OnYearDecreaseButtonClick(object sender, EventArgs e) {
			OnArrowClick(IsRightToLeft ? +1 : -1, true);
		}
		protected internal virtual void OnYearIncreaseButtonClick(object sender, EventArgs e) {
			OnArrowClick(IsRightToLeft ? -1 : +1, true);
		}
		protected virtual void OnArrowClick(int dir, bool yearNavigation) {
			if(ViewInfo.IsAnimated)
				return;
			try {
				Calendar.LockVisual();
				Calendar.CheckViewInfo();
				DateTime prev = SelectedDate;
				DateTime current = yearNavigation ? GetDateTimeAfterYearNavigating(dir) : GetDateTimeAfterMonthNavigating(dir);
				if(!ShouldChangeState(prev, current))
					return;
				ViewInfo.SwitchType = dir < 0 ? SwitchStateType.SwitchToRight : SwitchStateType.SwitchToLeft;
				ViewInfo.OnStateChanging(View, View, false);
				if(yearNavigation)
					NavigateNextYearView(dir);
				else
					NavigateNextMonthView(dir);
			}
			finally {
				Calendar.CancelVisual();
			}   
			ViewInfo.OnStateChanged(View, View, true);
		}
		Timer scrollTimer;
		protected Timer ScrollTimer {
			get {
				if(scrollTimer == null)
					scrollTimer = CreateScrollTimer();
				return scrollTimer;
			}
		}
		protected virtual Timer CreateScrollTimer() {
			Timer timer = new Timer();
			timer.Interval = 200;
			timer.Tick += OnScrollTimerTick;
			return timer;
		}
		void OnScrollTimerTick(object sender, EventArgs e) {
			if(ViewInfo.IsAnimated)
				return;
			if(IsScrollLeft)
				OnScrollLeft();
			else
				OnScrollRight();
		}
		protected bool IsScrollLeft { get; set; }
		public override void OnMouseMove(MouseEventArgs e) {
			if(ViewInfo.IsAnimated)
				return;
			ViewInfo.HoverInfo = ViewInfo.CalcHitInfo(e.Location);
			if(e.Button == MouseButtons.Left) {
				ViewInfo.PressedInfo = ViewInfo.CalcHitInfo(e.Location);
				if(ViewInfo.HoverInfo.HitTest == CalendarHitInfoType.LeftScrollArea)
					StartScrollLeft();
				else if(ViewInfo.HoverInfo.HitTest == CalendarHitInfoType.RightScrollArea)
					StartScrollRight();
				else {
					StopScrolling();
					SelectionManager.OnMouseMove(e);
				}
			}
			else {
				if(ViewInfo.HoverInfo.IsInCell) {
					ViewInfo.HoverInfo.Cell.ContextButtonsHandler.OnMouseMove(e);
				}
			}
		}
		private bool StopScrolling() {
			if(!ScrollTimer.Enabled)
				return false;
			ScrollTimer.Stop();
			return true;
		}
		protected void StartScrollRight() {
			IsScrollLeft = false;
			ScrollTimer.Start();
		}
		protected void StartScrollLeft() {
			IsScrollLeft = true;
			ScrollTimer.Start();
		}
		protected virtual void OnScrollRight() {
			NavigateNextMonthView(+1);
		}
		protected virtual void OnScrollLeft() {
			NavigateNextMonthView(-1);
		}
		public override void OnKeyDown(KeyEventArgs e) {
			if(e.Handled)
				return;
			switch(e.KeyCode) { 
				case Keys.Enter:
				case Keys.Space:
					SetEditDate(SelectedDate);
					e.Handled = true;
					break;
				case Keys.Right:
					OnNavigateRight();
					e.Handled = true;
					break;
				case Keys.Left:
					OnNavigateLeft();
					e.Handled = true;
					break;
				case Keys.Up:
					OnNavigateUp();
					e.Handled = true;
					break;
				case Keys.Down:
					OnNavigateDown();
					e.Handled = true;
					break;
			}
		}
		protected virtual void OnNavigateHorizontal(int dir) {
			int yearOffset = GetYearOffsetHorizontal(dir);
			int monthOffset = GetMonthOffsetHorizontal(dir);
			int dayOffset = GetDayOffsetHorizontal(dir);
			ChangeSelectedDate(yearOffset, monthOffset, dayOffset, false);
		}
		protected virtual void OnNavigateVertical(int dir) {
			int yearOffset = GetYearOffsetVertical(dir);
			int monthOffset = GetMonthOffsetVertical(dir);
			int dayOffset = GetDayOffsetVertical(dir);
			ChangeSelectedDate(yearOffset, monthOffset, dayOffset, false);
		}
		protected virtual void OnNavigateRight() {
			OnNavigateHorizontal(+1);
		}
		protected virtual void OnNavigateLeft() {
			OnNavigateHorizontal(-1);
		}
		protected virtual void OnNavigateUp() {
			OnNavigateVertical(-1);
		}
		protected virtual void OnNavigateDown() {
			OnNavigateVertical(1);
		}
		private int GetDayOffsetHorizontal(int dir) {
			if(View == DateEditCalendarViewType.MonthInfo) {
				if(Control.ModifierKeys.HasFlag(Keys.Control) || Control.ModifierKeys.HasFlag(Keys.Shift))
					return 0;
				return dir;
			}
			return 0;
		}
		private int GetMonthOffsetHorizontal(int dir) {
			if(View == DateEditCalendarViewType.YearInfo)
				return dir;
			if(View == DateEditCalendarViewType.QuarterInfo)
				return dir * 3;
			if(Calendar.SelectionMode == CalendarSelectionMode.Single && View == DateEditCalendarViewType.MonthInfo && Control.ModifierKeys.HasFlag(Keys.Control))
				return dir;
			return 0;
		}
		private int GetYearOffsetHorizontal(int dir) {
			if(View == DateEditCalendarViewType.YearsInfo)
				return dir;
			if(View == DateEditCalendarViewType.YearsGroupInfo)
				return dir * 10;
			if(Calendar.SelectionMode == CalendarSelectionMode.Single && View == DateEditCalendarViewType.MonthInfo && Control.ModifierKeys.HasFlag(Keys.Shift))
				return dir;
			return 0;
		}
		private int GetDayOffsetVertical(int dir) {
			if(View == DateEditCalendarViewType.MonthInfo)
				return dir * 7;
			return 0;
		}
		private int GetMonthOffsetVertical(int dir) {
			if(View == DateEditCalendarViewType.YearInfo)
				return dir * 4;
			if(View == DateEditCalendarViewType.QuarterInfo)
				return dir * 6;
			return 0;
		}
		private int GetYearOffsetVertical(int dir) {
			if(View == DateEditCalendarViewType.YearsInfo)
				return dir * 4;
			if(View == DateEditCalendarViewType.YearsGroupInfo)
				return dir * 10 * 4;
			return 0;
		}
		public override void OnKeyUp(KeyEventArgs e) { }
		public override void OnDoubleClick(EventArgs e) { }
	}
	public class VistaCalendarControlHandler : CalendarControlHandler {
		public VistaCalendarControlHandler(CalendarControlBase calendar) : base(calendar) { }
		public TimeEdit TimeEdit { get { return ((CalendarControl)Calendar).TimeEdit; } }
		bool GetDateChangeDirection(DateTime dt, DateTime today) {
			if(dt.Year > today.Year) return true;
			else if(dt.Year == today.Year && dt.Month > today.Month) return true;
			return false;
		}
		protected internal virtual void OnTodayCaptionButtonClick(object sender, EventArgs e) {
			if(View == DateEditCalendarViewType.MonthInfo) {
				if(SelectedDate.Month == Calendar.GetTodayDate().Month) {
					SelectedDate = Calendar.GetTodayDateTime();
					return;
				}
				bool leftArrow = GetDateChangeDirection(SelectedDate, Calendar.GetTodayDate());
				if(leftArrow)
					ViewInfo.SwitchType = SwitchStateType.SwitchToLeft;
				else
					ViewInfo.SwitchType = SwitchStateType.SwitchToRight;
				ViewInfo.OnStateChanging(View, View, true);
				SelectedDate = Calendar.GetTodayDateTime();
				ViewInfo.OnStateChanged(View, View, true);
			}
			else { 
				if(Calendar.IsAllowedView(DateEditCalendarViewType.MonthInfo)) {
					ViewInfo.OnViewChanging(View, DateEditCalendarViewType.MonthInfo, false);
					SelectedDate = Calendar.GetTodayDateTime();
					Calendar.SetViewCore(DateEditCalendarViewType.MonthInfo);
					ViewInfo.OnViewChanged(View, DateEditCalendarViewType.MonthInfo, true);
				}
			}
		}
		protected internal virtual void OnCancelButtonClick(object sender, EventArgs e) {
			Calendar.RaiseCancelClick();
		}
		protected internal virtual void OnOkButtonClick(object sender, EventArgs e) {
			if(TimeEdit != null) TimeEdit.DoValidate();
			Calendar.RaiseOkClick();
			Calendar.OnDateTimeCommit(SelectedDate);
		}
		protected internal virtual void OnForwardArrowClick(object sender, EventArgs e) {
			OnArrowClick(IsRightToLeft ? -1 : +1, 0);
		}
		protected internal virtual void OnForwardArrow2Click(object sender, EventArgs e) {
			OnArrowClick(0, IsRightToLeft ? -1 : +1);
		}
		protected internal virtual void OnBackwardArrowClick(object sender, EventArgs e) {
			OnArrowClick(IsRightToLeft ? +1 : -1, 0);
		}
		protected internal virtual void OnBackwardArrow2Click(object sender, EventArgs e) {
			OnArrowClick(0, IsRightToLeft ? +1 : -1);
		}
		protected internal virtual void OnCaptionButtonClick(object sender, EventArgs e) {
			Calendar.CheckViewInfo();
			Calendar.IncView();
		}
		protected virtual DateTime GetDateTimeAfterNavigating(int dir, int dir2) {
			if(View == DateEditCalendarViewType.MonthInfo)
				return UpdateDateTime(dir2, dir, 0);
			else if(View == DateEditCalendarViewType.QuarterInfo)
				return UpdateDateTime(dir, 0, 0);
			else if(View == DateEditCalendarViewType.YearInfo)
				return UpdateDateTime(dir, 0, 0);
			else if(View == DateEditCalendarViewType.YearsInfo)
				return UpdateDateTime(dir * 10, 0, 0);
			else if(View == DateEditCalendarViewType.YearsGroupInfo)
				return UpdateDateTime(dir * 100, 0, 0);
			return SelectedDate;
		}
		protected virtual void NavigateNextView(int dir, int dir2) {
			if(View == DateEditCalendarViewType.MonthInfo)
				ChangeSelectedDate(dir2, dir, 0, false);
			else if(View == DateEditCalendarViewType.QuarterInfo)
				ChangeSelectedDate(dir, 0, 0, false);
			else if(View == DateEditCalendarViewType.YearInfo)
				ChangeSelectedDate(dir, 0, 0, false);
			else if(View == DateEditCalendarViewType.YearsInfo)
				ChangeSelectedDate(dir * 10, 0, 0, false);
			else if(View == DateEditCalendarViewType.YearsGroupInfo)
				ChangeSelectedDate(dir * 100, 0, 0, false);
		}
		protected virtual void OnArrowClick(int dir, int dir2) {
			if(ViewInfo.IsAnimated)
				return;
			try {
				Calendar.LockVisual();
				Calendar.CheckViewInfo();
				DateTime prev = SelectedDate;
				DateTime current = GetDateTimeAfterNavigating(dir, dir2);
				if(!ShouldChangeState(prev, current))
					return;
				int direction = dir == 0 ? dir2 : dir;
				ViewInfo.SwitchType = direction < 0 ? SwitchStateType.SwitchToRight : SwitchStateType.SwitchToLeft;
				ViewInfo.OnStateChanging(View, View, false);
				NavigateNextView(dir, dir2);
			}
			finally {
				Calendar.CancelVisual();
			}
			ViewInfo.OnStateChanged(View, View, true);
		}
		protected override void OnScrollLeft() {
			OnArrowClick(-1, 0);
		}
		protected override void OnScrollRight() {
			OnArrowClick(+1, 0);
		}
	}
	public enum TextCaseMode { Default, UpperCase, LowerCase, SentenceCase, System }
	internal static class ColorAnimationHelper {
		public static Color BlendColor(AppearanceObject appearance, float opacity) {
			return BlendColor(appearance.BackColor, appearance.ForeColor, opacity);
		}
		public static Color BlendColor(Color backColor, Color foreColor, float opacity) {
			return Color.FromArgb(255,
					(byte)(backColor.R + (foreColor.R - backColor.R) * opacity),
					(byte)(backColor.G + (foreColor.G - backColor.G) * opacity),
					(byte)(backColor.B + (foreColor.B - backColor.B) * opacity));		
		}
	}
}

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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.XtraScheduler;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.XtraScheduler.Native;
using System.Runtime.Serialization;
using System.Collections;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Drawing {
	public enum ProcessAppointmentType {
		All = 0,
		Selected = 1,
		NotSelected = 2
	}
	public abstract class DayViewPainter : ViewPainterBase {
		DayViewColumnPainter dayViewColumnPainter;
		TimeRulerPainter timeRulerPainter;
		TimeIndicatorPainter timeIndicatorPainter;
		DayViewDispatchAppointmentPainter dispatchAppointmentPainter;
		AppointmentPainter timeCellsAppointmentPainter;
		int hiddenAllDayAreaHeight = 0;
		protected DayViewPainter() {
		}
		public override bool HideSelection {
			get { return base.HideSelection; }
			set {
				base.HideSelection = value;
				dayViewColumnPainter.CellPainter.HideSelection = value;
			}
		}
		public AppointmentPainter TimeCellsAppointmentPainter { get { return timeCellsAppointmentPainter; } }
		public AppointmentPainter AllDayAppointmentPainter { get { return AppointmentPainter; } }
		protected internal DayViewDispatchAppointmentPainter DispatchAppointmentPainter { get { return dispatchAppointmentPainter; } }
		protected internal TimeRulerPainter TimeRulerPainter { get { return timeRulerPainter; } }
		protected internal TimeIndicatorPainter TimeIndicatorPainter { get { return timeIndicatorPainter; } }
		protected internal int HiddenAllDayAreaHeight { get { return hiddenAllDayAreaHeight; } }
		public DayViewColumnPainter DayViewColumnPainter { get { return dayViewColumnPainter; } }
		protected abstract DayViewColumnPainter CreateColumnPainter();
		protected abstract TimeRulerPainter CreateTimeRulerPainter();
		protected abstract TimeIndicatorPainter CreateTimeIndicatorPainter();
		public override void Initialize() {
			base.Initialize();
			this.dayViewColumnPainter = CreateColumnPainter();
			this.dayViewColumnPainter.Initialize();
			this.timeRulerPainter = CreateTimeRulerPainter();
			this.timeIndicatorPainter = CreateTimeIndicatorPainter();
			this.timeCellsAppointmentPainter = CreateTimeCellsAppointmentPainter();
			this.dispatchAppointmentPainter = new DayViewDispatchAppointmentPainter(timeCellsAppointmentPainter, AppointmentPainter);
		}
		protected internal override ViewInfoPainterBase SelectCellsLayoutPainter() {
			return this.dayViewColumnPainter;
		}
		protected internal override AppointmentPainter CreateAppointmentPainter() {
			return CreateAllDayAppointmentPainter();
		}
		protected internal virtual AppointmentPainter CreateTimeCellsAppointmentPainter() {
			return new DayViewTimeCellsAppointmentPainter();
		}
		protected internal virtual AppointmentPainter CreateAllDayAppointmentPainter() {
			return new AppointmentPainter();
		}
		public override void DrawHeaders(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			DayViewInfo dayViewInfo = (DayViewInfo)viewInfo;
			SchedulerControl control = viewInfo.View.Control;
			TimeRulerPainter.DrawTimeRulers(cache, dayViewInfo.TimeRulers, control);
			DrawViewAndScrollBarSeparator(cache, dayViewInfo.GapBetweenTimeRulerAndCellsAreaBounds);
			HorizontalHeaderPainter.DrawHeaders(cache, dayViewInfo.DayHeaders, control);
			HorizontalHeaderPainter.DrawHeaders(cache, dayViewInfo.ResourceHeaders, control);
			HorizontalHeaderPainter.DrawHeaders(cache, dayViewInfo.GroupSeparators, control);
		}
		protected override void DrawTimeIndicator(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			DayViewInfo dayViewInfo = (DayViewInfo)viewInfo;
			SchedulerControl control = viewInfo.View.Control;
			TimeIndicatorPainter.DrawTimeIndicator(cache, dayViewInfo.TimeIndicator, control);
		}
		protected internal override void DrawCellContainers(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			dayViewColumnPainter.DrawColumns(cache, viewInfo.CellContainers, viewInfo.View.Control);
		}
		protected internal override void DrawAppointments(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			DrawAllDayAppointmentStatuses(cache, ((DayViewInfo)viewInfo).AllDayAppointmentsStatuses);
			DispatchAppointmentPainter.DrawAppointments(cache, viewInfo);
		}
		protected internal override void DrawMoreButtons(SchedulerViewInfoBase viewInfo, GraphicsCache cache) {
			base.DrawMoreButtons(viewInfo, cache);
			DayViewInfo dayViewInfo = (DayViewInfo)viewInfo;
			DrawMoreButtonsCore(cache, dayViewInfo.ScrollMoreButtons);
		}
		protected internal virtual void DrawAllDayAppointmentStatuses(GraphicsCache cache, AppointmentStatusViewInfoCollection statusViewInfos) {
			int count = statusViewInfos.Count;
			for (int i = 0; i < count; i++)
				AppointmentPainter.StatusPainter.DrawRectangleStatus(cache, statusViewInfos[i]);
		}
	}
	public class DayViewDispatchAppointmentPainter : AppointmentPainter {
		AppointmentPainter timeCellsAppointmentPainter;
		AppointmentPainter allDayAppointmentPainter;
		public DayViewDispatchAppointmentPainter(AppointmentPainter timeCellsAppointmentPainter, AppointmentPainter allDayAppointmentPainter) {
			if (timeCellsAppointmentPainter == null)
				Exceptions.ThrowArgumentException("timeCellsAppointmentPainter", timeCellsAppointmentPainter);
			if (allDayAppointmentPainter == null)
				Exceptions.ThrowArgumentException("allDayAppointmentPainter", allDayAppointmentPainter);
			this.timeCellsAppointmentPainter = timeCellsAppointmentPainter;
			this.allDayAppointmentPainter = allDayAppointmentPainter;
		}
		internal AppointmentPainter TimeCellsAppointmentPainter { get { return timeCellsAppointmentPainter; } }
		internal AppointmentPainter AllDayAppointmentPainter { get { return allDayAppointmentPainter; } }
		protected internal override void DrawAppointments(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			DrawAppointments(cache, viewInfo, ProcessAppointmentType.NotSelected);
			DrawAppointments(cache, viewInfo, ProcessAppointmentType.Selected);
		}
		protected internal override void DrawAppointments(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ProcessAppointmentType selection) {
			DrawAllDayAppointments(cache, viewInfo, selection);
			DrawTimeCellAppointments(cache, viewInfo, selection);
		}
		protected internal virtual void DrawAllDayAppointments(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ProcessAppointmentType selection) {
			AllDayAppointmentPainter.DrawAppointments(cache, viewInfo, selection);
		}
		protected internal virtual void DrawTimeCellAppointments(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ProcessAppointmentType selection) {
			TimeCellsAppointmentPainter.DrawAppointments(cache, viewInfo, selection);
		}
		protected internal override void DrawAppointmentsWithoutScrolling(GraphicsCache cache, AppointmentViewInfoCollection viewInfos, ISupportCustomDraw customDrawProvider) {
			AppointmentViewInfoCollection allDayAppointments = FilterAppointments(viewInfos, true);
			AppointmentViewInfoCollection timeCellAppointments = FilterAppointments(viewInfos, false);
			AllDayAppointmentPainter.DrawAppointmentsWithoutScrolling(cache, allDayAppointments, customDrawProvider);
			TimeCellsAppointmentPainter.DrawAppointmentsWithoutScrolling(cache, timeCellAppointments, customDrawProvider);
		}
		protected internal virtual AppointmentViewInfoCollection FilterAppointments(AppointmentViewInfoCollection viewInfos, bool allDay) {
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				AppointmentViewInfo viewInfo = viewInfos[i];
				if (viewInfo.LongerThanADay == allDay)
					result.Add(viewInfo);
			}
			return result;
		}
	}
	public abstract class DayViewColumnPainter : ViewInfoPainterBase {
		TimeCellPainter cellPainter;
		ExtendedTimeCellPainter extendedCellPainter;
		public virtual int StatusLineWidth { get { return 7; } }
		public virtual int AllDayAreaSeparatorVerticalMargin { get { return 1; } }
		public TimeCellPainter CellPainter { get { return cellPainter; } }
		public virtual void Initialize() {
			this.cellPainter = CreateCellPainter();
			this.extendedCellPainter = CreateExtendedCellPainter();
		}
		protected internal virtual TimeCellPainter CreateCellPainter() {
			return new TimeCellPainter();
		}
		protected internal virtual ExtendedTimeCellPainter CreateExtendedCellPainter() {
			return new ExtendedTimeCellPainter();
		}
		public virtual void DrawColumns(GraphicsCache cache, SchedulerViewCellContainerCollection columns, ISupportCustomDraw customDrawProvider) {
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				DrawColumn(cache, (DayViewColumn)columns[i], customDrawProvider);
		}
		protected virtual void DrawColumn(GraphicsCache cache, DayViewColumn column, ISupportCustomDraw customDrawProvider) {
			DrawAllDayArea(cache, column, customDrawProvider);
			DrawStatusLine(cache, column);
			DrawCells(cache, column.Cells, customDrawProvider);
			DrawExtendedCells(cache, column.ExtendedCells, customDrawProvider);
		}
		protected virtual void DrawCells(GraphicsCache cache, SchedulerViewCellBaseCollection cells, ISupportCustomDraw customDrawProvider) {
			cellPainter.DrawCells(cache, cells, customDrawProvider);
		}
		protected virtual void DrawExtendedCells(GraphicsCache cache, SchedulerViewCellBaseCollection extendedCells, ISupportCustomDraw customDrawProvider) {
			this.extendedCellPainter.DrawCells(cache, extendedCells, customDrawProvider);
		}
		public virtual void DrawStatusLine(GraphicsCache cache, DayViewColumn column) {
			Brush statusLineBrush = GetStatusLineBrush(cache, column);
			Brush statusLineBorderBrush = GetStatusLineBorderBrush(cache, column);
			DrawStatusLine(cache, column.StatusLineBounds, statusLineBrush, statusLineBorderBrush, true);
		}
		protected internal virtual Brush GetStatusLineBorderBrush(GraphicsCache cache, DayViewColumn column) {
			return cache.GetSolidBrush(column.StatusLineAppearance.BorderColor);
		}
		protected internal virtual Brush GetStatusLineBrush(GraphicsCache cache, DayViewColumn column) {
			return cache.GetSolidBrush(column.StatusLineAppearance.BackColor);
		}
		protected internal virtual void DrawStatusLine(GraphicsCache cache, Rectangle bounds, Brush br, Brush borderBrush, bool drawLeftBorder) {
			cache.FillRectangle(br, bounds);
			if (drawLeftBorder)
				cache.FillRectangle(borderBrush, RectUtils.GetLeftSideRect(bounds));
			cache.FillRectangle(borderBrush, RectUtils.GetRightSideRect(bounds));
		}
		public virtual void DrawAllDayArea(GraphicsCache cache, DayViewColumn column, ISupportCustomDraw customDrawProvider) {
			AllDayAreaCell cell = column.AllDayAreaCell;
			if (cell == null)
				return;
			DefaultDrawDelegate defaultDraw = delegate() { DrawAllDayAreaCell(cache, column, cell); };
			if (cell.RaiseCustomDrawEvent(cache, customDrawProvider, defaultDraw))
				return;
			defaultDraw();
		}
		public virtual void DrawAllDayAreaCell(GraphicsCache cache, DayViewColumn column, AllDayAreaCell cell) {
			bool isSelected = cell.Selected; 
			AppearanceObject appearance = isSelected ? cell.SelectionAppearance : cell.Appearance;
			appearance.FillRectangle(cache, cell.Bounds);
			DrawAllDayAreaCellBorders(cache, column, cell);
			if (cell.DrawLeftSeparator) {
				appearance = column.AllDayAreaSeparatorAppearance;
				appearance.FillRectangle(cache, cell.LeftSeparatorBounds);
			}
		}
		protected internal virtual void DrawAllDayAreaCellBorders(GraphicsCache cache, DayViewColumn column, AllDayAreaCell cell) {
			AppearanceObject appearance = column.AllDayAreaSeparatorAppearance;
			if (cell.HasLeftBorder)
				appearance.FillRectangle(cache, cell.LeftBorderBounds);
			if (cell.HasRightBorder)
				appearance.FillRectangle(cache, cell.RightBorderBounds);
			if (cell.HasTopBorder)
				appearance.FillRectangle(cache, cell.TopBorderBounds);
			if (cell.HasBottomBorder)
				appearance.FillRectangle(cache, cell.BottomBorderBounds);
		}
		protected internal virtual void PrepareCachedSkinElementInfo(AllDayAreaCell cell, Color color) {
		}
	}
	public class DayViewColumnFlatPainter : DayViewColumnPainter {
	}
	public class DayViewColumnUltraFlatPainter : DayViewColumnPainter {
	}
	public class DayViewColumnWindowsXPPainter : DayViewColumnPainter {
	}
	public class DayViewColumnOffice2003Painter : DayViewColumnPainter {
	}
	public class DayViewColumnSkinPainter : DayViewColumnPainter {
		UserLookAndFeel lookAndFeel;
		public DayViewColumnSkinPainter(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		public override int AllDayAreaSeparatorVerticalMargin { get { return 0; } }
		public override int StatusLineWidth {
			get {
				return ObjectPainter.CalcObjectMinBounds(null, SkinElementPainter.Default, SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinDefaultTimeLine)).Width;
			}
		}
		bool ShouldPaintAllDayAreaWithResourceColor {
			get {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				return skin.Properties.GetBoolean(SchedulerSkins.OptPaintResourceHeaderWithResourceColor);
			}
		}
		public override void DrawStatusLine(GraphicsCache cache, DayViewColumn info) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinDefaultTimeLine, info.StatusLineBounds));
		}
		public override void DrawAllDayAreaCell(GraphicsCache cache, DayViewColumn column, AllDayAreaCell cell) {
			SkinElementInfo el;
			if (cell.Selected)
				el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinAllDayAreaSelected, cell.Bounds);
			else if (cell.CachedSkinElementInfo != null)
				el = cell.CachedSkinElementInfo;
			else
				el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinAllDayArea, cell.Bounds);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
			if (cell.DrawLeftSeparator)
				cache.FillRectangle(cache.GetSolidBrush(el.Element.Color.GetForeColor()), cell.LeftSeparatorBounds);
		}
		protected internal override void PrepareCachedSkinElementInfo(AllDayAreaCell cell, Color color) {
			if (!ShouldPaintAllDayAreaWithResourceColor)
				return;
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinAllDayArea, cell.Bounds);
			cell.CachedSkinElementInfo = SkinElementColorer.PaintElementWithColor(el, color);
		}
	}
	#region Paint styles
	public class DayViewPainterFlat : DayViewPainter {
		public DayViewPainterFlat() {
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderFlatPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalFlatPainter();
		}
		protected override DayViewColumnPainter CreateColumnPainter() {
			return new DayViewColumnFlatPainter();
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			return new TimeRulerFlatPainter();
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorFlatPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonFlatPainter();
		}
		protected internal override void DrawCellContainers(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			base.DrawCellContainers(cache, viewInfo);
		}
		protected virtual void DrawExtendedRow(GraphicsCache cache, DayViewInfo viewInfo) {
			int count = viewInfo.ExtendedRows.Count;
			for (int i = 0; i < count; i++) {
				DayViewRow row = viewInfo.ExtendedRows[i];
				cache.DrawRectangle(new Pen(Color.Black), row.Bounds);
			}
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRuler, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, SystemColors.ControlDark, SystemColors.Control)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerHourLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowLine, new AppearanceDefault(SystemColors.ControlText, Color.Red, Color.Transparent, Color.Red)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowArea, new AppearanceDefault(SystemColors.ControlText, Color.LightGoldenrodYellow, Color.Transparent, Color.Orange, System.Drawing.Drawing2D.LinearGradientMode.Vertical)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayArea, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayAreaSeparator, new AppearanceDefault(SystemColors.ControlText, SystemColors.WindowText, Color.Transparent, SystemColors.WindowText)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.SelectedAllDayArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, Color.Transparent, SystemColors.Window)),
												   HeaderAppearanceFlatHelper.CreateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateSelectionAppearance(HorzAlignment.Center),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class DayViewPainterUltraFlat : DayViewPainter {
		public DayViewPainterUltraFlat() {
		}
		public override int ViewAndScrollbarVerticalSeparatorWidth { get { return 1; } }
		public override int ViewAndScrollbarHorizontalSeparatorHeight { get { return 1; } }
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderUltraFlatPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalUltraFlatPainter();
		}
		protected override DayViewColumnPainter CreateColumnPainter() {
			return new DayViewColumnUltraFlatPainter();
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			return new TimeRulerUltraFlatPainter();
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorUltraFlatPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonUltraFlatPainter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRuler, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, SystemColors.ControlDark, SystemColors.Control)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerHourLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowLine, new AppearanceDefault(SystemColors.ControlText, Color.Red, Color.Transparent, Color.Red)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowArea, new AppearanceDefault(SystemColors.ControlText, Color.LightGoldenrodYellow, Color.Transparent, Color.Orange, System.Drawing.Drawing2D.LinearGradientMode.Vertical)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayArea, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayAreaSeparator, new AppearanceDefault(SystemColors.ControlText, SystemColors.WindowText, Color.Transparent, SystemColors.WindowText)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.SelectedAllDayArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, Color.Transparent, SystemColors.Window)),
												   HeaderAppearanceUltraFlatHelper.CreateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceUltraFlatHelper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceUltraFlatHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceUltraFlatHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceUltraFlatHelper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceUltraFlatHelper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceUltraFlatHelper.CreateSelectionAppearance(HorzAlignment.Center),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonUltraFlatAppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonUltraFlatAppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class DayViewPainterOffice2003 : DayViewPainter {
		public DayViewPainterOffice2003() {
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderOffice2003Painter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalOffice2003Painter();
		}
		protected override DayViewColumnPainter CreateColumnPainter() {
			return new DayViewColumnOffice2003Painter();
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			return new TimeRulerOffice2003Painter();
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorOffice2003Painter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonOffice2003Painter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRuler, new AppearanceDefault(SystemColors.WindowText, ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Header]), ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]), ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Header2]))),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerLine, new AppearanceDefault(SystemColors.ControlText, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]), Color.Transparent, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]))),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerHourLine, new AppearanceDefault(SystemColors.ControlText, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]), Color.Transparent, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]))),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowLine, new AppearanceDefault(SystemColors.ControlText, Color.Red, Color.Transparent, Color.Red)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowArea, new AppearanceDefault(SystemColors.ControlText, Color.LightGoldenrodYellow, Color.Transparent, Color.Orange, System.Drawing.Drawing2D.LinearGradientMode.Vertical)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayArea, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayAreaSeparator, new AppearanceDefault(SystemColors.ControlText, SystemColors.WindowText, Color.Transparent, SystemColors.WindowText)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.SelectedAllDayArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, Color.Transparent, SystemColors.Window)),
												   HeaderAppearanceOffice2003Helper.CreateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceOffice2003Helper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceOffice2003Helper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceOffice2003Helper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceOffice2003Helper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceOffice2003Helper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceOffice2003Helper.CreateSelectionAppearance(HorzAlignment.Center),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonOffice2003AppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonOffice2003AppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class DayViewPainterWindowsXP : DayViewPainter {
		public DayViewPainterWindowsXP() {
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderWindowsXPPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalWindowsXPPainter();
		}
		protected override DayViewColumnPainter CreateColumnPainter() {
			return new DayViewColumnWindowsXPPainter();
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			return new TimeRulerWindowsXPPainter();
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorWindowsXPPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonWindowsXPPainter();
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRuler, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, SystemColors.ControlDark, SystemColors.Control)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerHourLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowLine, new AppearanceDefault(SystemColors.ControlText, Color.Red, Color.Transparent, Color.Red)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowArea, new AppearanceDefault(SystemColors.ControlText, Color.LightGoldenrodYellow, Color.Transparent, Color.Orange, System.Drawing.Drawing2D.LinearGradientMode.Vertical)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayArea, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayAreaSeparator, new AppearanceDefault(SystemColors.ControlText, SystemColors.WindowText, Color.Transparent, SystemColors.WindowText)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.SelectedAllDayArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, Color.Transparent, SystemColors.Window)),
												   HeaderAppearanceFlatHelper.CreateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateSelectionAppearance(HorzAlignment.Center),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		#endregion
	}
	public class DayViewPainterSkin : DayViewPainter {
		UserLookAndFeel lookAndFeel;
		public DayViewPainterSkin(UserLookAndFeel lookAndFeel) {
			if (lookAndFeel == null)
				Exceptions.ThrowArgumentException("lookAndFeel", lookAndFeel);
			this.lookAndFeel = lookAndFeel;
		}
		public override int ViewAndScrollbarVerticalSeparatorWidth { get { return SkinPainterHelper.GetViewAndScrollbarVerticalSeparatorWidth(lookAndFeel); } }
		public override int ViewAndScrollbarHorizontalSeparatorHeight { get { return SkinPainterHelper.GetViewAndScrollbarHorizontalSeparatorHeight(lookAndFeel); } }
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new SchedulerHeaderSkinPainter(lookAndFeel);
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new SchedulerHeaderVerticalSkinPainter(lookAndFeel);
		}
		protected override DayViewColumnPainter CreateColumnPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new DayViewColumnSkinPainter(lookAndFeel);
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new TimeRulerSkinPainter(lookAndFeel);
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new TimeIndicatorSkinPainter(lookAndFeel);
		}
		protected internal override AppointmentPainter CreateAllDayAppointmentPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new AppointmentSkinPainter(lookAndFeel);
		}
		protected internal override AppointmentPainter CreateTimeCellsAppointmentPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new DayViewTimeCellAppointmentSkinPainter(lookAndFeel);
		}
		protected override MoreButtonPainter CreateMoreButtonPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new MoreButtonSkinPainter(lookAndFeel);
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			XtraSchedulerDebug.Assert(lookAndFeel != null);
			return new NavigationButtonSkinPainter(lookAndFeel);
		}
		#region GetDefaultAppearances()
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRuler, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinRuler, new AppearanceDefault(SystemColors.WindowText, SystemColors.Control, Color.Transparent, SystemColors.Control), lookAndFeel)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerLine, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinRulerMinLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark), lookAndFeel)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerHourLine, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinRulerHourLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark), lookAndFeel)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowLine, new AppearanceDefault(SystemColors.ControlText, Color.Red, Color.Transparent, Color.Red)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.TimeRulerNowArea, new AppearanceDefault(SystemColors.ControlText, Color.LightGoldenrodYellow, Color.Transparent, Color.Orange, System.Drawing.Drawing2D.LinearGradientMode.Vertical)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayArea, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinAllDayArea, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Transparent, SystemColors.ControlDark), lookAndFeel)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.AllDayAreaSeparator, new AppearanceDefault(SystemColors.ControlText, SystemColors.WindowText, Color.Transparent, SystemColors.WindowText)),
												   new AppearanceDefaultInfo(DayViewAppearanceNames.SelectedAllDayArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, Color.Transparent, SystemColors.Window)),
												   HeaderAppearanceSkinHelper.CreateHeaderCaptionAppearance(HorzAlignment.Center, lookAndFeel),
												   HeaderAppearanceSkinHelper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceSkinHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center, lookAndFeel),
												   HeaderAppearanceSkinHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceSkinHelper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center, lookAndFeel),
												   HeaderAppearanceSkinHelper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceSkinHelper.CreateSelectionAppearance(lookAndFeel, HorzAlignment.Center),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonSkinAppearancesHelper.CreateNavigationButtonAppearance(HorzAlignment.Center, lookAndFeel),
												   NavigationButtonSkinAppearancesHelper.CreateNavigationButtonDisabledAppearance(HorzAlignment.Center, lookAndFeel)
											   };
		}
		#endregion
		public override void DrawViewAndScrollBarSeparator(GraphicsCache cache, Rectangle bounds) {
			SkinPainterHelper.DrawViewAndDateTimeSeparator(cache, lookAndFeel, bounds);
		}
	}
	#endregion
	#region ShadowImageIndexes
	public enum ShadowImageIndexes {
		BottomLeftCorner = 0,
		BottomSide = 1,
		BottomRightCorner = 2,
		RightSide = 3,
		TopRightCorner = 4
	}
	#endregion
	public class DayViewTimeCellsAppointmentPainter : AppointmentPainter {
		#region Fields
		readonly ImageCollection shadowImages;
		ImageAttributes imageAttributes;
		#endregion
		public DayViewTimeCellsAppointmentPainter()
			: base() {
			shadowImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraScheduler.Images.shadows.png", System.Reflection.Assembly.GetExecutingAssembly(), new Size(5, 5));
			imageAttributes = new ImageAttributes();
			imageAttributes.SetWrapMode(WrapMode.Tile);
		}
		#region Properties
		protected internal virtual ImageCollection ShadowImages { get { return shadowImages; } }
		protected internal override int RightShadowSize { get { return shadowImages.ImageSize.Width; } }
		protected internal override int BottomShadowSize { get { return shadowImages.ImageSize.Width; } }
		public override int LeftPadding { get { return 0; } }
		public override int RightPadding { get { return 0; } }
		#endregion
		protected internal override void DrawSelection(GraphicsCache cache, AppointmentViewInfo viewInfo, Rectangle viewClipBounds, ISupportCustomDraw customDrawProvider) {
			if (viewInfo.LongerThanADay)
				base.DrawSelection(cache, viewInfo, viewClipBounds, customDrawProvider);
			else {
				SchedulerControl control = (SchedulerControl)customDrawProvider;
				DayViewInfo dayViewInfo = (DayViewInfo)control.ActiveView.ViewInfo;
				DrawSelectionCore(cache, viewInfo, viewClipBounds, dayViewInfo.SelectionBorderWidth);
			}
		}
		protected internal virtual void DrawSelectionCore(GraphicsCache cache, AppointmentViewInfo viewInfo, Rectangle viewClipBounds, int selectionWidth) {
			Rectangle bounds = viewInfo.Bounds;
			Rectangle selectionBounds = Rectangle.Inflate(bounds, 0, selectionWidth);
			selectionBounds = Rectangle.Intersect(selectionBounds, viewClipBounds);
			using (IntersectClipper clipper = new IntersectClipper(cache, selectionBounds, IntersectClipperOptions.SkipCurrentClip)) {
				AppointmentStatusViewInfo topSideSelection = CalcaulteTopSideSelection(viewInfo, selectionWidth);
				AppointmentStatusViewInfo bottomSideSelection = CalcaulteBottomSideSelection(viewInfo, selectionWidth);
				StatusPainter.DrawRectangleStatus(cache, topSideSelection);
				StatusPainter.DrawRectangleStatus(cache, bottomSideSelection);
			}
		}
		protected internal virtual AppointmentStatusViewInfo CalcaulteTopSideSelection(AppointmentViewInfo viewInfo, int selectionWidth) {
			Rectangle bounds = viewInfo.Bounds;
			AppointmentStatusViewInfo selection = new AppointmentStatusViewInfo(viewInfo);
			selection.Bounds = new Rectangle(bounds.Left, bounds.Top - selectionWidth, bounds.Width, selectionWidth);
			selection.HasLeftBorder = true;
			selection.HasTopBorder = true;
			selection.HasRightBorder = true;
			selection.HasBottomBorder = false;
			return selection;
		}
		protected internal virtual AppointmentStatusViewInfo CalcaulteBottomSideSelection(AppointmentViewInfo viewInfo, int selectionWidth) {
			Rectangle bounds = viewInfo.Bounds;
			AppointmentStatusViewInfo selection = new AppointmentStatusViewInfo(viewInfo);
			selection.Bounds = new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, selectionWidth);
			selection.HasLeftBorder = true;
			selection.HasTopBorder = false;
			selection.HasRightBorder = true;
			selection.HasBottomBorder = true;
			return selection;
		}
		protected internal override SchedulerViewCellContainerCollection GetScrollContainers(SchedulerViewInfoBase viewInfo) {
			return viewInfo.CellContainers;
		}
		protected internal override Rectangle CalculateContainerClipBounds(SchedulerViewCellContainer container) {
			DayViewColumn column = (DayViewColumn)container;
			Rectangle cellsBounds = column.CellsBounds;
			Rectangle clipBounds = Rectangle.FromLTRB(column.Bounds.Left, cellsBounds.Top, cellsBounds.Right, cellsBounds.Bottom);
			return clipBounds;
		}
		protected internal override void DrawShadow(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
			AppointmentShadowViewInfo shadowViewInfo = new AppointmentShadowViewInfo();
			shadowViewInfo.CalculateBounds(aptViewInfo.Bounds, RightShadowSize);
			using (IntersectClipper clipper = new IntersectClipper(cache, shadowViewInfo.Bounds)) {
				DrawShadow(shadowViewInfo, cache);
			}
		}
		private void DrawShadow(AppointmentShadowViewInfo shadowViewInfo, GraphicsCache cache) {
			DrawShadowElement(cache, ShadowImageIndexes.BottomLeftCorner, shadowViewInfo.BottomLeftCorner);
			DrawShadowElement(cache, ShadowImageIndexes.BottomSide, shadowViewInfo.BottomRect);
			DrawShadowElement(cache, ShadowImageIndexes.BottomRightCorner, shadowViewInfo.BottomRightCorner);
			DrawShadowElement(cache, ShadowImageIndexes.RightSide, shadowViewInfo.RightRect);
			DrawShadowElement(cache, ShadowImageIndexes.TopRightCorner, shadowViewInfo.TopRightCorner);
		}
		protected internal virtual void DrawShadowElement(GraphicsCache cache, ShadowImageIndexes index, Rectangle bounds) {
			cache.Paint.DrawImage(cache.Graphics, shadowImages.Images[(int)index], bounds, 0, 0, shadowImages.ImageSize.Width, shadowImages.ImageSize.Height, GraphicsUnit.Pixel, imageAttributes);
		}
	}
	public class ExpandedAppointmentViewInfoState {
		Rectangle bounds;
		AppointmentViewInfoSkinElementInfoCache skinElements;
		Hashtable statusItemBounds = new Hashtable();
		public ExpandedAppointmentViewInfoState(AppointmentViewInfo aptViewInfo) {
			this.bounds = aptViewInfo.Bounds;
			this.skinElements = aptViewInfo.CachedSkinElementInfos;
			PopulateStatusItemBounds(aptViewInfo);
		}
		protected internal virtual void PopulateStatusItemBounds(AppointmentViewInfo aptViewInfo) {
			int count = aptViewInfo.StatusItems.Count;
			for (int i = 0; i < count; i++) {
				ViewInfoItem statusItem = aptViewInfo.StatusItems[i];
				statusItemBounds.Add(statusItem, statusItem.Bounds);
			}
		}
		protected internal virtual void LoadState(AppointmentViewInfo aptViewInfo) {
			aptViewInfo.CachedSkinElementInfos = this.skinElements;
			aptViewInfo.Bounds = this.bounds;
			int count = statusItemBounds.Count;
			for (int i = 0; i < count; i++) {
				object bounds = statusItemBounds[aptViewInfo.StatusItems[i]];
				XtraSchedulerDebug.Assert(bounds != null);
				aptViewInfo.StatusItems[i].Bounds = (Rectangle)bounds;
			}
		}
	}
	public class DayViewTimeCellAppointmentSkinPainter : AppointmentSkinPainter {
		const int AdditionalAppointmentHeight = 10;
		public DayViewTimeCellAppointmentSkinPainter(UserLookAndFeel lookAndFeel)
			: base(lookAndFeel) {
		}
		public override int LeftPadding {
			get {
				Skin skin = SchedulerSkins.GetSkin(LookAndFeel);
				return skin.Properties.GetInteger(SchedulerSkins.OptDayViewAppointmentLeftPadding);
			}
		}
		public override int RightPadding { get { return 0; } }
		protected internal override SchedulerViewCellContainerCollection GetScrollContainers(SchedulerViewInfoBase viewInfo) {
			return viewInfo.CellContainers;
		}
		protected internal override Rectangle CalculateContainerClipBounds(SchedulerViewCellContainer container) {
			DayViewColumn column = (DayViewColumn)container;
			Rectangle cellsBounds = column.CellsBounds;
			Rectangle clipBounds = Rectangle.FromLTRB(column.Bounds.Left, cellsBounds.Top, cellsBounds.Right, cellsBounds.Bottom);
			return clipBounds;
		}
		protected internal override int RightShadowSize {
			get {
				SkinElementInfo info = SkinPainterHelper.UpdateObjectInfoArgs(LookAndFeel, SchedulerSkins.SkinAppointmentRightShadow);
				SkinElement el = info.Element;
				if (el == null)
					return 0;
				SkinImage skinImage = el.Image;
				if (skinImage == null)
					return 0;
				return skinImage.SizingMargins.Right;
			}
		}
		protected internal override int BottomShadowSize {
			get {
				SkinElementInfo info = SkinPainterHelper.UpdateObjectInfoArgs(LookAndFeel, SchedulerSkins.SkinAppointmentBottomShadow);
				SkinElement el = info.Element;
				if (el == null)
					return 0;
				SkinImage skinImage = el.Image;
				if (skinImage == null)
					return 0;
				return skinImage.SizingMargins.Bottom;
			}
		}
		public override void DrawAppointment(GraphicsCache cache, AppointmentViewInfo viewInfo, ISupportCustomDraw customDrawProvider) {
			SchedulerControl control = (SchedulerControl)customDrawProvider;
			if (viewInfo.Bounds.Width < 5 || viewInfo.Bounds.Height < 5) {
				DrawAppointmentSimple(cache, viewInfo, customDrawProvider);
				return;
			}
			if (IsEntireAppointmentVisible(viewInfo))
				DrawEntireAppointment(cache, viewInfo, control);
			else
				DrawAppointmentWithoutCorners(cache, viewInfo, control);
		}
		protected internal virtual void DrawEntireAppointment(GraphicsCache cache, AppointmentViewInfo viewInfo, SchedulerControl control) {
			base.DrawAppointment(cache, viewInfo, control);
		}
		protected internal virtual void DrawAppointmentWithoutCorners(GraphicsCache cache, AppointmentViewInfo viewInfo, SchedulerControl control) {
			ExpandedAppointmentViewInfoState prevState = SaveState(viewInfo);
			ExpandViewInfoBounds(viewInfo, control.ColoredSkinElementCache);
			base.DrawAppointment(cache, viewInfo, control);
			RestoreState(prevState, viewInfo);
		}
		protected internal virtual ExpandedAppointmentViewInfoState SaveState(AppointmentViewInfo viewInfo) {
			return new ExpandedAppointmentViewInfoState(viewInfo);
		}
		protected internal virtual void RestoreState(ExpandedAppointmentViewInfoState state, AppointmentViewInfo viewInfo) {
			state.LoadState(viewInfo);
		}
		protected internal virtual bool IsEntireAppointmentVisible(AppointmentViewInfo viewInfo) {
			return viewInfo.HasBottomBorder && viewInfo.HasTopBorder;
		}
		protected internal virtual void ExpandViewInfoBounds(AppointmentViewInfo viewInfo, ColoredSkinElementCache skinCache) {
			if (!viewInfo.HasTopBorder)
				ExpandViewInfoTopBound(viewInfo);
			if (!viewInfo.HasBottomBorder)
				ExpandViewInfoBottomBound(viewInfo);
			viewInfo.CalcBorderBounds(this);
			viewInfo.PrepareSkinElementInfoCache(this, skinCache);
		}
		protected internal virtual void ExpandViewInfoTopBound(AppointmentViewInfo viewInfo) {
			viewInfo.Bounds = RectUtils.GetBottomSideRect(viewInfo.Bounds, viewInfo.Bounds.Height + AdditionalAppointmentHeight);
			int count = viewInfo.StatusItems.Count;
			for (int i = 0; i < count; i++) {
				ViewInfoItem statusItem = viewInfo.StatusItems[i];
				statusItem.Bounds = RectUtils.GetBottomSideRect(statusItem.Bounds, statusItem.Bounds.Height + AdditionalAppointmentHeight);
			}
		}
		protected internal virtual void ExpandViewInfoBottomBound(AppointmentViewInfo viewInfo) {
			viewInfo.Bounds = RectUtils.GetTopSideRect(viewInfo.Bounds, viewInfo.Bounds.Height + AdditionalAppointmentHeight);
			int count = viewInfo.StatusItems.Count;
			for (int i = 0; i < count; i++) {
				ViewInfoItem statusItem = viewInfo.StatusItems[i];
				statusItem.Bounds = RectUtils.GetTopSideRect(statusItem.Bounds, statusItem.Bounds.Height + AdditionalAppointmentHeight);
			}
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected internal override AppointmentViewInfoSkinElementInfoCache PrepareSkinElementInfoCache(AppointmentViewInfo viewInfo, ColoredSkinElementCache coloredSkinElementCache) {
			AppointmentViewInfoSkinElementInfoCache cache = new AppointmentViewInfoSkinElementInfoCache();
			Color color = viewInfo.Appearance.BackColor;
			cache.Content = coloredSkinElementCache.GetAppointmentSkinElementInfo(viewInfo.ContentSkinElementName, color, LookAndFeel, viewInfo.Bounds);
			return cache;
		}
		protected internal override bool ShouldShowShadow(AppointmentViewInfo viewInfo) {
			return viewInfo.ShowShadow;
		}
		protected internal override void DrawShadow(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
			int rightShadowSize = this.RightShadowSize;
			int bottomShadowSize = this.BottomShadowSize;
			Rectangle viewInfoBounds = aptViewInfo.Bounds;
			SkinElementInfo info = SkinPainterHelper.UpdateObjectInfoArgs(LookAndFeel, SchedulerSkins.SkinAppointmentBottomShadow);
			Rectangle minBounds = ObjectPainter.CalcObjectMinBounds(cache.Graphics, SkinElementPainter.Default, info);
			Rectangle bottomShadowBounds = new Rectangle(viewInfoBounds.X, viewInfoBounds.Bottom - minBounds.Height + bottomShadowSize, viewInfoBounds.Width + rightShadowSize, minBounds.Height);
			info.Bounds = bottomShadowBounds;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			info = SkinPainterHelper.UpdateObjectInfoArgs(LookAndFeel, SchedulerSkins.SkinAppointmentRightShadow);
			minBounds = ObjectPainter.CalcObjectMinBounds(cache.Graphics, SkinElementPainter.Default, info);
			Rectangle rightShadowBounds = new Rectangle(viewInfoBounds.Right - minBounds.Width + rightShadowSize, viewInfoBounds.Y, minBounds.Width, bottomShadowBounds.Top - viewInfoBounds.Y);
			info.Bounds = rightShadowBounds;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
	}
	public class AppointmentShadowViewInfo {
		#region Fields
		Rectangle bounds;
		Rectangle bottomRect;
		Rectangle rightRect;
		Rectangle bottomLeftCorner;
		Rectangle bottomRightCorner;
		Rectangle topRightCorner;
		#endregion
		public AppointmentShadowViewInfo() {
		}
		#region Properties
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle BottomRect { get { return bottomRect; } }
		public Rectangle RightRect { get { return rightRect; } }
		public Rectangle BottomLeftCorner { get { return bottomLeftCorner; } }
		public Rectangle BottomRightCorner { get { return bottomRightCorner; } }
		public Rectangle TopRightCorner { get { return topRightCorner; } }
		#endregion
		protected internal virtual void CalculateBounds(Rectangle ownerBounds, int shadowSize) {
			const int w2 = 0;
			bottomRect = new Rectangle(ownerBounds.X + 2 * shadowSize, ownerBounds.Bottom, ownerBounds.Width - 2 * shadowSize, shadowSize);
			rightRect = new Rectangle(ownerBounds.Right, ownerBounds.Top + shadowSize + w2, shadowSize, ownerBounds.Height - shadowSize - w2);
			bottomLeftCorner = new Rectangle(ownerBounds.X + shadowSize, ownerBounds.Bottom, shadowSize, shadowSize);
			bottomRightCorner = new Rectangle(ownerBounds.Right, ownerBounds.Bottom, shadowSize, shadowSize);
			topRightCorner = new Rectangle(ownerBounds.Right, ownerBounds.Top + w2, shadowSize, shadowSize);
			bounds = new Rectangle(bottomLeftCorner.X, topRightCorner.Y, rightRect.Right - bottomLeftCorner.X, bottomRect.Bottom - topRightCorner.Y);
		}
	}
}

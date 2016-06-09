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
using System.Text;
using DevExpress.Utils;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Operations;
using DevExpress.Utils.Drawing;
using DevExpress.Services.Internal;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using System.Linq;
namespace DevExpress.XtraScheduler.Native {
	#region SplitIndicatorType
	public enum SplitIndicatorType {
		Vertical,
		Horizontal
	}
	#endregion
	#region SplitIndicatorPainter
	public class SplitIndicatorPainter : IDisposable {
		static Cursor cutCursor;
		static SplitIndicatorPainter() {
			cutCursor = ResourceImageHelper.CreateCursorFromResources("DevExpress.XtraScheduler.Images.cut_cursor.cur", typeof(SchedulerControl).Assembly);
		}
		public static Cursor CutCursor { get { return cutCursor; } }
		Brush outerSplitIndicatorRectangleBrush;
		Brush innerSplitIndicatorRectangleBrush;
		Brush outerSplitIndicatorMarkerRectangleBrush;
		Brush innerSplitIndicatorMarkerRectangleBrush;
		Brush shadowBrush;
		public SplitIndicatorPainter() {
			this.outerSplitIndicatorRectangleBrush = new SolidBrush(Color.FromArgb(0x62, 0x62, 0x62));
			this.innerSplitIndicatorRectangleBrush = new SolidBrush(Color.FromArgb(0xff, 0xff, 0xff));
			this.outerSplitIndicatorMarkerRectangleBrush = new SolidBrush(Color.FromArgb(0xb9, 0x00, 0x00));
			this.innerSplitIndicatorMarkerRectangleBrush = new SolidBrush(Color.FromArgb(0xff, 0xb3, 0xb3));
			this.shadowBrush = new SolidBrush(Color.FromArgb(35, 0, 0, 0));
		}
		public Brush OuterSplitIndicatorRectangleBrush { get { return outerSplitIndicatorRectangleBrush; } }
		public Brush InnerSplitIndicatorRectangleBrush { get { return innerSplitIndicatorRectangleBrush; } }
		public Brush OuterSplitIndicatorMarkerRectangleBrush { get { return outerSplitIndicatorMarkerRectangleBrush; } }
		public Brush InnerSplitIndicatorMarkerRectangleBrush { get { return innerSplitIndicatorMarkerRectangleBrush; } }
		public Brush ShadowBrush { get { return shadowBrush; } }
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.outerSplitIndicatorRectangleBrush != null) {
					this.outerSplitIndicatorRectangleBrush.Dispose();
					this.outerSplitIndicatorRectangleBrush = null;
				}
				if (this.innerSplitIndicatorRectangleBrush != null) {
					this.innerSplitIndicatorRectangleBrush.Dispose();
					this.innerSplitIndicatorRectangleBrush = null;
				}
				if (shadowBrush != null) {
					this.shadowBrush.Dispose();
					this.shadowBrush = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SplitIndicatorPainter() {
			Dispose(false);
		}
		#endregion
		internal void DrawSplitIndicator(SplitIndicatorInfo splitIndicatorInfo, Graphics graphics) {
			if (splitIndicatorInfo == null)
				return;
			graphics.FillRectangle(ShadowBrush, splitIndicatorInfo.ShadowRectangle);
			graphics.FillRectangle(OuterSplitIndicatorRectangleBrush, splitIndicatorInfo.OuterRectangle);
			graphics.FillRectangle(InnerSplitIndicatorRectangleBrush, splitIndicatorInfo.InnerRectangle);
		}
		internal void DrawSplitIndicatorMarkers(SplitIndicatorMarkersInfo splitIndicatorMarkerInfo, Graphics graphics) {
			if (splitIndicatorMarkerInfo == null)
				return;
			List<SplitIndicatorInfo> infos = splitIndicatorMarkerInfo.SplitIndicatorMarkers;
			int count = infos.Count;
			for (int i = 0; i < count; i++) {
				SplitIndicatorInfo info = infos[i];
				graphics.FillRectangle(OuterSplitIndicatorMarkerRectangleBrush, info.OuterRectangle);
				graphics.FillRectangle(InnerSplitIndicatorMarkerRectangleBrush, info.InnerRectangle);
			}
		}
	}
	#endregion
	#region SplitIndicatorMarkersInfo
	public class SplitIndicatorMarkersInfo {
		#region Fields
		List<SplitIndicatorInfo> splitIndicatorMarkers;
		SplitIndicatorType indicatorType;
		#endregion
		public SplitIndicatorMarkersInfo(SplitIndicatorType indicatorType) {
			this.indicatorType = indicatorType;
			this.splitIndicatorMarkers = new List<SplitIndicatorInfo>();
		}
		#region Properties
		public SplitIndicatorType IndicatorType { get { return indicatorType; } }
		public List<SplitIndicatorInfo> SplitIndicatorMarkers { get { return splitIndicatorMarkers; } }
		#endregion
		#region AddPrimaryMarkers
		public void AddPrimaryMarkers(List<Rectangle> primaryRectangles) {
			int count = primaryRectangles.Count;
			for (int i = 0; i < count; i++) {
				SplitIndicatorInfo info = SplitIndicatorInfo.Create(IndicatorType);
				info.PrimaryRectangle = primaryRectangles[i];
				SplitIndicatorMarkers.Add(info);
			}
		}
		#endregion
		#region Reset
		public void Reset() {
			SplitIndicatorMarkers.Clear();
		}
		#endregion
	}
	#endregion
	#region SplitIndicatorInfo
	public abstract class SplitIndicatorInfo {
		public static SplitIndicatorInfo Create(SplitIndicatorType SplitIndicatorType) {
			if (SplitIndicatorType == SplitIndicatorType.Horizontal)
				return new HorizontalSplitIndicatorInfo();
			else if (SplitIndicatorType == SplitIndicatorType.Vertical)
				return new VerticalSplitIndicatorInfo();
			return null;
		}
		Rectangle primaryRectangle;
		public Rectangle PrimaryRectangle {
			get { return primaryRectangle; }
			set {
				if (primaryRectangle == value)
					return;
				primaryRectangle = value;
			}
		}
		public abstract Rectangle InnerRectangle { get; }
		public abstract Rectangle OuterRectangle { get; }
		public abstract Rectangle ShadowRectangle { get; }
		public abstract SplitIndicatorType Type { get; }
	}
	#endregion
	#region VerticalSplitIndicatorInfo
	public class VerticalSplitIndicatorInfo : SplitIndicatorInfo {
		public override Rectangle OuterRectangle {
			get { return new Rectangle(PrimaryRectangle.X - 1, PrimaryRectangle.Y, PrimaryRectangle.Width + 2, PrimaryRectangle.Height); }
		}
		public override Rectangle InnerRectangle { get { return PrimaryRectangle; } }
		public override Rectangle ShadowRectangle { get { return new Rectangle(PrimaryRectangle.X - 2, PrimaryRectangle.Y, PrimaryRectangle.Width + 5, PrimaryRectangle.Height); } }
		public override SplitIndicatorType Type { get { return SplitIndicatorType.Vertical; } }
	}
	#endregion
	#region HorizontalSplitIndicatorInfo
	public class HorizontalSplitIndicatorInfo : SplitIndicatorInfo {
		public override Rectangle OuterRectangle { get { return new Rectangle(PrimaryRectangle.X, PrimaryRectangle.Y - 1, PrimaryRectangle.Width, PrimaryRectangle.Height + 2); } }
		public override Rectangle InnerRectangle { get { return PrimaryRectangle; } }
		public override Rectangle ShadowRectangle { get { return new Rectangle(PrimaryRectangle.X, PrimaryRectangle.Y - 2, PrimaryRectangle.Width, PrimaryRectangle.Height + 5); } }
		public override SplitIndicatorType Type { get { return SplitIndicatorType.Horizontal; } }
	}
	#endregion
	#region SchedulerAppointmentOperationBase (abstract class)
	public abstract class SchedulerAppointmentOperationBase : SchedulerInputOperation, IAppointmentVisualStateCalculator {
		#region Fields
		ICommandUIStateManagerService oldCommandUIStateManagerService;
		IAppointmentVisualStateCalculator baseAppointmentVisualStateCalculator;
		#endregion
		protected SchedulerAppointmentOperationBase(SchedulerControl schedulerControl)
			: base(schedulerControl) {
		}
		#region Properties
		protected internal SchedulerViewInfoBase ViewInfo { get { return SchedulerControl.ActiveView.ViewInfo; } }
		#endregion
		public override void Start() {
			base.Start();
			try {
				InitializeOperation();
				StartCore();
			} catch {
				Finish();
			}
		}
		public override void Finish() {
			base.Finish();
			if (this.baseAppointmentVisualStateCalculator != null) {
				SchedulerControl.InnerControl.RestoreAppointmentVisualStateFactory();
				System.Diagnostics.Debug.Assert(SchedulerControl.InnerControl.AppointmentVisualStateCalculator == this.baseAppointmentVisualStateCalculator);
			}
		}
		protected internal virtual void InitializeOperation() {
			ReplaceAppointmentVisualStateCalculator();
			SubstituteCommandUIStateManagerService();
			SubscribeSchedulerControlEvents();
			PrepareSchedulerProperties();
			InitializeOperationCore();
		}
		void ReplaceAppointmentVisualStateCalculator() {
			this.baseAppointmentVisualStateCalculator = SchedulerControl.InnerControl.AppointmentVisualStateCalculator;
			SchedulerControl.InnerControl.SetNewAppointmentVisualStateCalculator(this);
			foreach (SchedulerViewCellContainer cellContainer in ViewInfo.CellContainers) {
				ViewInfo.UpdateAppointmentsDisableDropState(cellContainer);
				ViewInfo.UpdateAppointmentsSelection(cellContainer);
			}
		}
		public void SubstituteCommandUIStateManagerService() {
			this.oldCommandUIStateManagerService = (ICommandUIStateManagerService)ServiceContainer.GetService(typeof(ICommandUIStateManagerService));
			ServiceContainer.RemoveService(typeof(ICommandUIStateManagerService));
			ServiceContainer.AddService(typeof(ICommandUIStateManagerService), new SplitAppointmentOperationCommandUIStateManagerService());
		}
		public void RestoreCommandUIStateManagerService() {
			ServiceContainer.RemoveService(typeof(ICommandUIStateManagerService));
			ServiceContainer.AddService(typeof(ICommandUIStateManagerService), this.oldCommandUIStateManagerService);
		}
		protected internal virtual void SubscribeSchedulerControlEvents() {
			SchedulerControl.Paint += OnSchedulerControlPaint;
			SchedulerControl.PopupMenuShowing += OnPopupMenuShowing;
		}
		protected internal virtual void UnsubscribeSchedulerControlEvents() {
			SchedulerControl.Paint -= OnSchedulerControlPaint;
			SchedulerControl.PopupMenuShowing -= OnPopupMenuShowing;
		}
		void OnPopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			e.Menu = null;
		}
		protected internal virtual AppointmentViewInfo GetActiveAppointmentViewInfo(IEnumerable<AppointmentViewInfo> appointmentViewInfos, Point mousePosition) {
			SchedulerHitInfo schedulerHitInfo = ViewInfo.CalcHitInfo(mousePosition, true);
			SelectableIntervalViewInfo viewCell = schedulerHitInfo.FindHitInfo(SchedulerHitTest.Cell | SchedulerHitTest.AllDayArea).ViewInfo;
			foreach (AppointmentViewInfo vi in appointmentViewInfos) {
				XtraSchedulerDebug.Assert(!vi.IsDisposed);
				if (IsAppointmentViewInfoHit(vi, mousePosition, viewCell))
					return vi;
			}
			return null;
		}
		protected internal virtual bool IsAppointmentViewInfoHit(AppointmentViewInfo appointmentViewInfo, Point point, SelectableIntervalViewInfo cellViewInfo) {
			bool isAppointmentViewInfoIntersectCellViewInfo = true;
			if (cellViewInfo is SingleWeekCellBase)
				isAppointmentViewInfoIntersectCellViewInfo = appointmentViewInfo.Interval.IntersectsWithExcludingBounds(cellViewInfo.Interval);
			if (!isAppointmentViewInfoIntersectCellViewInfo)
				return false;
			Rectangle bounds = appointmentViewInfo.GetVisualBounds();
			return IsAppointmentViewInfoHitCore(appointmentViewInfo, point, bounds);
		}
		protected internal override void OnMouseDown(MouseEventArgs e) {
			try {
				Point mousePosition = new Point(e.X, e.Y);
				if (!IsMouseOver(SchedulerControl, mousePosition))
					return;
				if (e.Button == MouseButtons.Left) {
					AppointmentViewInfo appointmentViewInfo = GetActiveAppointmentViewInfo(GetActualAppointmentViewInfos(), mousePosition);
					if (appointmentViewInfo == null)
						return;
					Execute(mousePosition, appointmentViewInfo);
				}
			} catch {
				Finish();
			}
		}
		protected internal virtual bool IsMouseOver(Control control, Point mousePosition) {
			Rectangle newControlBounds = new Rectangle(Point.Empty, control.Size);
			return newControlBounds.Contains(mousePosition);
		}
		protected internal override void OnMouseUp(MouseEventArgs e) {
			if (e.Button == MouseButtons.Right)
				Finish();
		}
		public virtual bool IsDisabled(Appointment apt) {
			if (this.baseAppointmentVisualStateCalculator == null)
				return false;
			return this.baseAppointmentVisualStateCalculator.IsDisabled(apt);
		}
		public virtual bool IsSelected(Appointment apt) {
			if (this.baseAppointmentVisualStateCalculator == null)
				return false;
			return this.baseAppointmentVisualStateCalculator.IsSelected(apt);
		}
		protected internal abstract void StartCore();
		protected internal abstract void InitializeOperationCore();
		protected internal abstract void PrepareSchedulerProperties();
		protected internal abstract ICommandUIStateManagerService GetNewService();
		protected internal abstract void OnSchedulerControlPaint(object sender, PaintEventArgs e);
		protected internal abstract bool IsAppointmentViewInfoHitCore(AppointmentViewInfo appointmentViewInfo, Point point, Rectangle bounds);
		protected internal abstract void Execute(Point mousePosition, AppointmentViewInfo appointmentViewInfo);
		protected internal abstract IEnumerable<AppointmentViewInfo> GetActualAppointmentViewInfos();
	}
	#endregion
	#region SplitAppointmentOperation
	public class SplitAppointmentOperation : SchedulerAppointmentOperationBase {
		#region Fields
		Appointment appointment;
		TimeScaleCollection timeScales;
		AppointmentViewInfoCollection appointmentViewInfos;
		TimeScale timeScale;
		SplitIndicatorInfo splitIndicatorInfo;
		SplitIndicatorMarkersInfo splitIndicatorMarkersInfo;
		SplitIndicatorType splitIndicatorType;
		String toolTipStringFormat;
		SplitAppointmentOperationToolTipController toolTipController;
		bool isMinimumTimeScaleMode;
		bool dateTimeScrollBarEnabledOldState;
		SplitIndicatorPainter splitPainter;
		Cursor oldCursor;
		SplitIndicatorInfoCalculator splitIndicatorInfoCalculator;
		MarkerCalculator markerCalculator;
		#endregion
		public SplitAppointmentOperation(SchedulerControl schedulerControl, TimeScaleCollection timeScales, Appointment appointment)
			: base(schedulerControl) {
			if (appointment == null)
				Exceptions.ThrowArgumentNullException("appointment");
			if (timeScales == null)
				Exceptions.ThrowArgumentNullException("timeScales");
			this.appointment = appointment;
			this.timeScales = timeScales;
			this.toolTipController = new SplitAppointmentOperationToolTipController();
			this.toolTipStringFormat = SchedulerLocalizer.GetString(SchedulerStringId.DefaultToolTipStringFormat_SplitAppointment);
			this.splitPainter = null;
			this.oldCursor = null;
		}
		#region Properties
		internal Appointment Appointment { get { return appointment; } }
		public TimeScaleCollection TimeScales { get { return timeScales; } set { timeScales = value; } }
		internal TimeScale TimeScale { get { return timeScale; } set { timeScale = value; } }
		public String ToolTipStringFormat { get { return toolTipStringFormat; } set { toolTipStringFormat = value; } }
		internal SplitIndicatorType SplitIndicatorType { get { return splitIndicatorType; } }
		internal bool IsMinimumTimeScaleMode { get { return isMinimumTimeScaleMode; } set { isMinimumTimeScaleMode = value; } }
		internal SplitAppointmentOperationToolTipController ToolTipController { get { return toolTipController; } }
		internal AppointmentViewInfoCollection AppointmentViewInfos { get { return appointmentViewInfos; } }
		public SplitIndicatorInfo SplitIndicatorInfo { get { return splitIndicatorInfo; } }
		public SplitIndicatorMarkersInfo SplitIndicatorMarkersInfo { get { return splitIndicatorMarkersInfo; } }
		public SplitIndicatorPainter SplitPainter { get { return splitPainter; } }
		public Cursor OldCursor { get { return oldCursor; } }
		public SplitIndicatorInfoCalculator SplitIndicatorInfoCalculator { get { return splitIndicatorInfoCalculator; } }
		public MarkerCalculator MarkerCalculator { get { return markerCalculator; } }
		#endregion
		protected internal override IEnumerable<AppointmentViewInfo> GetActualAppointmentViewInfos() {
			return AppointmentViewInfos;
		}
		#region OnMouseMove
		protected internal override void OnMouseMove(MouseEventArgs e) {
			XtraSchedulerDebug.Assert(!Appointment.IsDisposed);
			try {
				Point mousePosition = new Point(e.X, e.Y);
				Recalculate(mousePosition);
				SchedulerControl.Invalidate();
				SchedulerControl.Update();
			} catch {
				Finish();
			}
		}
		#endregion
		#region Recalculate
		protected internal virtual void Recalculate(Point mousePosition) {
			if (!IsMouseOver(SchedulerControl, mousePosition)) {
				HideToolTip();
				return;
			}
			this.appointmentViewInfos = GetAppointmentViewInfos();
			AppointmentViewInfo activeAppointmentViewInfo = GetActiveAppointmentViewInfo(AppointmentViewInfos, mousePosition);
			DateTime splitTime = CalculateSplitTime(activeAppointmentViewInfo, ViewInfo, mousePosition);
			SplitIndicatorInfoCalculator.Calculate(activeAppointmentViewInfo, splitTime, mousePosition);
			if (splitTime != DateTime.MinValue)
				markerCalculator.Calculate(SplitIndicatorMarkersInfo, activeAppointmentViewInfo, SplitIndicatorInfo, mousePosition);
			else
				SplitIndicatorMarkersInfo.Reset();
			TimeScale currentTimeScale = GetTimeScale(TimeScales);
			ProcessToolTip(mousePosition, activeAppointmentViewInfo, splitTime, currentTimeScale);
		}
		#endregion
		#region CalculateSplitTime
		protected internal virtual DateTime CalculateSplitTime(AppointmentViewInfo appointmentViewInfo, SchedulerViewInfoBase viewInfo, Point position) {
			if (appointmentViewInfo == null)
				return DateTime.MinValue;
			DateTime dateTimeMousePosition = GetDateTimeForPosition(appointmentViewInfo, viewInfo, position);
			if (dateTimeMousePosition == DateTime.MinValue)
				return DateTime.MinValue;
			DayView dayView = ViewInfo.View as DayView;
			if (dayView != null && appointmentViewInfo is VerticalAppointmentViewInfo) { 
				if (dateTimeMousePosition.TimeOfDay <= dayView.ScrollStartTime)
					return appointmentViewInfo.Interval.Start.Date.Add(dayView.ScrollStartTime);
			}
			TimeSpan offset = CalculateSplitOffsetForAppointmentViewInfo(appointmentViewInfo, dateTimeMousePosition);
			return appointmentViewInfo.Interval.Start + offset;
		}
		#endregion
		#region OnMouseWheel
		protected internal override void OnMouseWheel(MouseEventArgs e) {
			Point mousePosition = new Point(e.X, e.Y);
			AppointmentViewInfo appointmentViewInfo = GetActiveAppointmentViewInfo(AppointmentViewInfos, mousePosition);
			if (appointmentViewInfo == null)
				return;
			if (e.Delta > 0)
				IncrementTimeScale(appointmentViewInfo);
			else
				DecrementTimeScale();
			Recalculate(mousePosition);
			SchedulerControl.Invalidate();
			SchedulerControl.Update();
		}
		#endregion
		#region IncrementTimeScale
		protected internal virtual void IncrementTimeScale(AppointmentViewInfo appointmentViewInfo) {
			int currentIndex = TimeScales.IndexOf(TimeScale);
			TimeScale augmentTimeScale;
			if (currentIndex + 1 < TimeScales.Count) {
				augmentTimeScale = TimeScales[currentIndex + 1];
				TimeInterval appointmentViewInfoInterval = appointmentViewInfo.Interval;
				int numberOfSteps = CalculateNumberOfSteps(appointmentViewInfoInterval.Start, appointmentViewInfoInterval.Duration, augmentTimeScale);
				if (numberOfSteps > 0)
					TimeScale = augmentTimeScale;
			}
		}
		#endregion
		#region DecrementTimeScale
		protected internal virtual void DecrementTimeScale() {
			int currentIndex = TimeScales.IndexOf(TimeScale);
			if (currentIndex - 1 >= 0)
				TimeScale = TimeScales[currentIndex - 1];
		}
		#endregion
		#region OnKeyDown
		protected internal override void OnKeyDown(KeyEventArgs e) {
			if (e.KeyCode == Keys.Escape) {
				Finish();
				return;
			}
			if (e.KeyCode == (Keys.ControlKey) && !IsMinimumTimeScaleMode) {
				Point mousePosition = SchedulerControl.MousePosition;
				Recalculate(mousePosition);
			}
		}
		#endregion
		#region OnKeyUp
		protected internal override void OnKeyUp(KeyEventArgs e) {
			IsMinimumTimeScaleMode = false;
			if (e.KeyCode == (Keys.ControlKey)) {
				Point mousePosition = SchedulerControl.MousePosition;
				Recalculate(mousePosition);
			}
		}
		#endregion
		#region IsAppointmentViewInfoIntersectsWithIndicatorRectangle
		protected internal virtual bool IsAppointmentViewInfoIntersectsWithIndicatorRectangle(AppointmentViewInfo activeAppointmentViewInfo, AppointmentViewInfo appointmentViewInfo, Rectangle indicator) {
			return appointmentViewInfo.Interval.Equals(activeAppointmentViewInfo.Interval);
		}
		#endregion
		#region GetSingleSplitMarkerRect
		protected internal virtual Rectangle GetSingleSplitMarkerRect(AppointmentViewInfo appointmentViewInfo, Rectangle indicatorRectangle) {
			Rectangle bounds = appointmentViewInfo.Bounds;
			if (SplitIndicatorType == SplitIndicatorType.Horizontal)
				return new Rectangle(bounds.X, indicatorRectangle.Y, bounds.Width, 1);
			return new Rectangle(indicatorRectangle.X, bounds.Y, 1, bounds.Height);
		}
		#endregion
		protected internal override void StartCore() {
			Point mousePosition = SchedulerControl.PointToClient(Control.MousePosition);
			if (!IsEmptyViewCell(mousePosition))
				Recalculate(mousePosition);
		}
		protected internal override void InitializeOperationCore() {
			this.splitPainter = new SplitIndicatorPainter();
			this.appointmentViewInfos = GetAppointmentViewInfos();
			Point mousePosition = SchedulerControl.PointToClient(Control.MousePosition);
			AppointmentViewInfo appointmentViewInfo = GetCurrentAppointmentViewInfo(AppointmentViewInfos, mousePosition);
			this.timeScale = CalculateOptimalTimeScale(appointmentViewInfo);
			this.splitIndicatorType = CalculateSplitIndicatorType(Appointment, ViewInfo.GetCellContainers().SelectMany(c => c.AppointmentViewInfos));
			this.splitIndicatorInfo = SplitIndicatorInfo.Create(SplitIndicatorType);
			this.splitIndicatorMarkersInfo = new SplitIndicatorMarkersInfo(SplitIndicatorType);
			this.splitIndicatorInfoCalculator = CreateSplitIndicatorInfoCalculator(SplitIndicatorInfo, ViewInfo);
			this.markerCalculator = CreateMarkerCalculator(ViewInfo, SchedulerControl.GroupType, AppointmentViewInfos, SplitIndicatorType);
		}
		protected internal virtual SplitIndicatorInfoCalculator CreateSplitIndicatorInfoCalculator(SplitIndicatorInfo splitIndicatorInfo, SchedulerViewInfoBase viewInfo) {
			return SplitIndicatorInfoCalculatorFactory.Create(splitIndicatorInfo, viewInfo);
		}
		#region CreateMarkerCalculator
		protected internal virtual MarkerCalculator CreateMarkerCalculator(SchedulerViewInfoBase viewInfo, SchedulerGroupType groupType, AppointmentViewInfoCollection appointmentViewInfos, SplitIndicatorType splitIndicatorType) {
			return MarkerCalculator.CreateConverter(viewInfo, groupType, appointmentViewInfos, splitIndicatorType);
			;
		}
		#endregion
		#region CalculateOptimalTimeScale
		protected internal virtual TimeScale CalculateOptimalTimeScale(AppointmentViewInfo appointmentViewInfo) {
			TimeInterval appointmentViewInfoInterval = appointmentViewInfo.Interval;
			return GetOptimalTimeScale(TimeScales, appointmentViewInfoInterval.Start, appointmentViewInfoInterval.Duration, 13);
		}
		#endregion
		protected internal override void PrepareSchedulerProperties() {
			this.oldCursor = SchedulerControl.Cursor;
			SchedulerControl.Capture = true;
			SchedulerControl.Cursor = SplitIndicatorPainter.CutCursor;
			this.dateTimeScrollBarEnabledOldState = SchedulerControl.DateTimeScrollBar.Enabled;
			SchedulerControl.DateTimeScrollBar.Enabled = false;
		}
		#region IsEmptyViewCell
		protected internal virtual bool IsEmptyViewCell(Point position) {
			SchedulerHitInfo hitInfo = ViewInfo.CalcHitInfo(position, true);
			SelectableIntervalViewInfo viewCell = hitInfo.FindHitInfo(SchedulerHitTest.Cell | SchedulerHitTest.AllDayArea).ViewInfo;
			return viewCell == SelectableIntervalViewInfo.Empty;
		}
		#endregion
		#region GetCurrentAppointmentViewInfo
		protected internal virtual AppointmentViewInfo GetCurrentAppointmentViewInfo(AppointmentViewInfoCollection appointmentViewInfos, Point mousePosition) {
			AppointmentViewInfo activAppointmentViewInfo = GetActiveAppointmentViewInfo(appointmentViewInfos, mousePosition);
			if (activAppointmentViewInfo != null)
				return activAppointmentViewInfo;
			else
				return GetFirstAppointmentViewInfo(Appointment, appointmentViewInfos);
		}
		#endregion
		protected internal override void SubscribeSchedulerControlEvents() {
			base.SubscribeSchedulerControlEvents();
			SchedulerControl.Resize += OnResize;
		}
		protected internal override void UnsubscribeSchedulerControlEvents() {
			base.UnsubscribeSchedulerControlEvents();
			SchedulerControl.Resize -= OnResize;
		}
		#region OnResize
		protected internal virtual void OnResize(object sender, EventArgs e) {
			Finish();
		}
		#endregion
		#region Finish
		public override void Finish() {
			base.Finish();
			SchedulerControl.BeginUpdate();
			try {
				SchedulerControl.DateTimeScrollBar.Enabled = this.dateTimeScrollBarEnabledOldState;
				SchedulerControl.Capture = false;
				RestoreCommandUIStateManagerService();
				UnsubscribeSchedulerControlEvents();
				SchedulerControl.Refresh();
				HideToolTip();
				if (OldCursor != null)
					SchedulerControl.Cursor = OldCursor;
				if (SplitPainter != null)
					SplitPainter.Dispose();
			} finally {
				SchedulerControl.EndUpdate();
			}
		}
		#endregion
		#region CalculateSplitIndicatorType
		protected internal virtual SplitIndicatorType CalculateSplitIndicatorType(Appointment appointment, IEnumerable<AppointmentViewInfo> appointmentViewInfos) {
			AppointmentViewInfo appointmentViewInfo = GetFirstAppointmentViewInfo(appointment, appointmentViewInfos);
			if (appointmentViewInfo is HorizontalAppointmentViewInfo)
				return SplitIndicatorType.Vertical;
			return SplitIndicatorType.Horizontal;
		}
		#endregion
		#region GetFirstAppointmentViewInfo
		protected internal virtual AppointmentViewInfo GetFirstAppointmentViewInfo(Appointment appointment, IEnumerable<AppointmentViewInfo> appointmentViewInfos) {
			foreach (AppointmentViewInfo vi in appointmentViewInfos) {
				if (vi.Appointment == appointment)
					return vi;
			}
			Exceptions.ThrowInternalException();
			return null;
		}
		#endregion
		#region ProcessToolTip
		protected internal virtual void ProcessToolTip(Point mousePosition, AppointmentViewInfo appointmentViewInfo, DateTime splitTime, TimeScale timeScale) {
			if (appointmentViewInfo == null || splitTime == DateTime.MinValue) {
				HideToolTip();
				return;
			}
			Point toolTipPoint = new Point(mousePosition.X, mousePosition.Y + 14);
			ShowToolTip(toolTipPoint, splitTime, timeScale);
		}
		#endregion
		#region Execute
		protected internal override void Execute(Point mousePosition, AppointmentViewInfo appointmentViewInfo) {
			TimeSpan splitOffsetAppointment = CalculateSplitOffsetForAppointment(appointmentViewInfo, mousePosition);
			if (splitOffsetAppointment.Ticks > 0 && splitOffsetAppointment != Appointment.Duration) {
				SchedulerControl.BeginUpdate();
				try {
					ExecuteCore(splitOffsetAppointment);
					Finish();
				} finally {
					SchedulerControl.EndUpdate();
				}
			}
		}
		#endregion
		#region CalculateSplitOffsetForAppointment
		protected internal virtual TimeSpan CalculateSplitOffsetForAppointment(AppointmentViewInfo appointmentViewInfo, Point mousePosition) {
			DateTime dateTimeMousePosition = GetDateTimeForPosition(appointmentViewInfo, ViewInfo, mousePosition);
			if (dateTimeMousePosition == DateTime.MinValue)
				return TimeSpan.MinValue;
			TimeSpan splitOffsetForAppointmentViewInfo = CalculateSplitOffsetForAppointmentViewInfo(appointmentViewInfo, dateTimeMousePosition);
			DateTime splitDate = appointmentViewInfo.Interval.Start + splitOffsetForAppointmentViewInfo;
			return splitDate - appointmentViewInfo.Appointment.Start;
		}
		#endregion
		#region ExecuteCore
		protected internal virtual void ExecuteCore(TimeSpan splitOffset) {
			if (Appointment.IsRecurring) {
				RecurrentAppointmentAction action;
				if (Appointment.IsException)
					action = RecurrentAppointmentAction.Occurrence;
				else
					action = SchedulerControl.ShowEditRecurrentAppointmentForm(appointment);
				switch (action) {
					case RecurrentAppointmentAction.Cancel:
						return;
					case RecurrentAppointmentAction.Series:
						{
							SplitAppointmentCore(appointment.RecurrencePattern, splitOffset);
							break;
						}
					case RecurrentAppointmentAction.Occurrence:
						SplitAppointmentCore(appointment, splitOffset);
						break;
					case RecurrentAppointmentAction.Ask:
						return;
				}
			} else
				SplitAppointmentCore(appointment, splitOffset);
		}
		#endregion
		#region SplitAppointmentCore
		void SplitAppointmentCore(Appointment apt, TimeSpan splitOffset) {
			SchedulerControl.DataStorage.BeginUpdate();
			try {
				SchedulerControl.BeginUpdate();
				try {
					AppointmentSplitter splitter = new AppointmentSplitter();
					splitter.Splitted += new AppointmentSplittedEventHandler(SplittedEventHandler);
					try {
						splitter.SplitAppointment(apt, apt.Start + splitOffset, ChangeOccurrenceSplitMode.Auto);
					} finally {
						splitter.Splitted -= new AppointmentSplittedEventHandler(SplittedEventHandler);
					}
				} finally {
					SchedulerControl.EndUpdate();
				}
			} finally {
				SchedulerControl.DataStorage.EndUpdate();
			}
		}
		#endregion
		#region SplittedEventHandler
		void SplittedEventHandler(object sender, AppointmentSplittedEventArgs e) {
			if (e.Copy.Type != AppointmentType.ChangedOccurrence)
				SchedulerControl.DataStorage.Appointments.Add(e.Copy);
		}
		#endregion
		protected internal override void OnSchedulerControlPaint(object sender, PaintEventArgs e) {
			DrawSplitIndicator(e.Graphics, SplitIndicatorInfo);
			DrawSplitMarkers(e.Graphics, SplitIndicatorMarkersInfo);
		}
		#region DrawSplitIndicator
		protected internal virtual void DrawSplitIndicator(Graphics graphics, SplitIndicatorInfo splitIndicatorInfo) {
			SplitPainter.DrawSplitIndicator(SplitIndicatorInfo, graphics);
		}
		#endregion
		#region DrawSplitMarkers
		protected internal virtual void DrawSplitMarkers(Graphics graphics, SplitIndicatorMarkersInfo markersInfo) {
			SplitPainter.DrawSplitIndicatorMarkers(markersInfo, graphics);
		}
		#endregion
		#region GetAppointmentViewInfos
		protected internal virtual AppointmentViewInfoCollection GetAppointmentViewInfos() {
			AppointmentViewInfoCollection resultAppointmentViewInfos = new AppointmentViewInfoCollection();
			resultAppointmentViewInfos.AddRange(ViewInfo.CopyAllAppointmentViewInfos().Where(vi => vi.Appointment == Appointment));
			return resultAppointmentViewInfos;
		}
		#endregion
		#region CalculateSplitOffsetForAppointmentViewInfo
		protected internal virtual TimeSpan CalculateSplitOffsetForAppointmentViewInfo(AppointmentViewInfo appointmentViewInfo, DateTime dateTimeMousePosition) {
			XtraSchedulerDebug.Assert(appointmentViewInfo != null);
			TimeSpan splitOffset = dateTimeMousePosition - appointmentViewInfo.Interval.Start;
			DateTime start = appointmentViewInfo.Interval.Start;
			DateTime dateTime = start + splitOffset;
			TimeSpan result = GetTimeInterval(dateTime) - start;
			if (result.Ticks < 0)
				return TimeSpan.Zero;
			if (result > appointmentViewInfo.Interval.Duration)
				return appointmentViewInfo.Interval.Duration;
			return result;
		}
		#endregion
		#region GetDateTimeForPosition
		protected internal virtual DateTime GetDateTimeForPosition(AppointmentViewInfo appointmentViewInfo, SchedulerViewInfoBase viewInfo, Point point) {
			AppointmentViewInfoPositionTimeConverter converter = AppointmentViewInfoPositionTimeConverter.CreateConverter(appointmentViewInfo, viewInfo);
			return converter.ConvertPositionToTime(point);
		}
		#endregion
		#region GetTimeInterval
		protected internal virtual DateTime GetTimeInterval(DateTime dateTime) {
			TimeScale timeScale = GetTimeScale(TimeScales);
			DateTime floorDateTime = timeScale.Floor(dateTime);
			DateTime nextDateTime = timeScale.GetNextDate(floorDateTime);
			if (dateTime - floorDateTime <= nextDateTime - dateTime)
				return floorDateTime;
			else
				return nextDateTime;
		}
		#endregion
		#region GetTimeScale
		protected internal virtual TimeScale GetTimeScale(TimeScaleCollection timeScales) {
			XtraSchedulerDebug.Assert(timeScales.Count > 0);
			if (GetModifierKeys() == Keys.Control) {
				IsMinimumTimeScaleMode = true;
				return timeScales[0];
			}
			return TimeScale;
		}
		#endregion
		#region GetModifierKeys
		protected internal virtual Keys GetModifierKeys() {
			return Control.ModifierKeys;
		}
		#endregion
		#region GetOptimalTimeScale
		protected internal virtual TimeScale GetOptimalTimeScale(TimeScaleCollection timeScales, DateTime startTime, TimeSpan duration, int optimalNumber) {
			XtraSchedulerDebug.Assert(timeScales.Count > 0);
			TimeScale result = timeScales[0];
			int resultNumberOfSteps = int.MaxValue;
			foreach (TimeScale timeScale in timeScales) {
				int numberOfSteps = CalculateNumberOfSteps(startTime, duration, timeScale);
				if (Math.Abs(optimalNumber - numberOfSteps) <= (Math.Abs(optimalNumber - resultNumberOfSteps)) && (numberOfSteps != 0)) {
					resultNumberOfSteps = numberOfSteps;
					result = timeScale;
				}
			}
			return result;
		}
		#endregion
		#region CalculateNumberOfSteps
		protected internal virtual int CalculateNumberOfSteps(DateTime startTime, TimeSpan duration, TimeScale timeScale) {
			DateTime stepStartTime = timeScale.Floor(startTime);
			DateTime end = stepStartTime + duration;
			stepStartTime = timeScale.GetNextDate(stepStartTime);
			int stepNumber = 0;
			while (stepStartTime <= end) {
				stepNumber++;
				stepStartTime = timeScale.GetNextDate(stepStartTime);
			}
			return stepNumber;
		}
		#endregion
		protected internal override bool IsAppointmentViewInfoHitCore(AppointmentViewInfo appointmentViewInfo, Point point, Rectangle bounds) {
			if (appointmentViewInfo is VerticalAppointmentViewInfo)
				return RectUtils.ContainsY(bounds, point);
			else
				return RectUtils.ContainsX(bounds, point);
		}
		#region ShowToolTip
		protected internal virtual void ShowToolTip(Point mousePosition, DateTime splitTime, TimeScale timeScale) {
			Point point = new Point(Control.MousePosition.X, Control.MousePosition.Y - 2);
			ToolTipController.ShowHint(String.Format(ToolTipStringFormat, splitTime, timeScale), ToolTipLocation.TopRight, point);
		}
		#endregion
		#region HideToolTip
		protected internal virtual void HideToolTip() {
			ToolTipController.HideHint();
		}
		#endregion
		protected internal override ICommandUIStateManagerService GetNewService() {
			return new SplitAppointmentOperationCommandUIStateManagerService();
		}
		public override bool IsDisabled(Appointment apt) {
			return apt != Appointment;
		}
	}
	#endregion
	#region SplitAppointmentOperationCommandUIStateManagerService
	public class SplitAppointmentOperationCommandUIStateManagerService : ICommandUIStateManagerService {
		public void UpdateCommandUIState(Command command, ICommandUIState state) {
			state.Enabled = false;
		}
	}
	#endregion
	#region MarkerCalculator
	public abstract class MarkerCalculator {
		#region Fields
		SchedulerViewInfoBase viewInfo;
		AppointmentViewInfoCollection appointmentViewInfos;
		SplitIndicatorType splitIndicatorType;
		#endregion
		protected MarkerCalculator(SchedulerViewInfoBase viewInfo, AppointmentViewInfoCollection appointmentViewInfos, SplitIndicatorType splitIndicatorType) {
			this.viewInfo = viewInfo;
			this.appointmentViewInfos = appointmentViewInfos;
			this.splitIndicatorType = splitIndicatorType;
		}
		#region Properties
		public SchedulerViewInfoBase ViewInfo { get { return viewInfo; } }
		public AppointmentViewInfoCollection AppointmentViewInfos { get { return appointmentViewInfos; } }
		public SplitIndicatorType SplitIndicatorType { get { return splitIndicatorType; } }
		#endregion
		#region CreateConverter
		public static MarkerCalculator CreateConverter(SchedulerViewInfoBase viewInfo, SchedulerGroupType groupType, AppointmentViewInfoCollection appointmentViewInfos, SplitIndicatorType splitIndicatorType) {
			if (groupType == SchedulerGroupType.None)
				return new SingleMarkerCalculator(viewInfo, appointmentViewInfos, splitIndicatorType);
			if (viewInfo is DayViewInfo) {
				if (splitIndicatorType == SplitIndicatorType.Vertical)
					return new SingleMarkerCalculator(viewInfo, appointmentViewInfos, splitIndicatorType);
				else
					return new MultipleMarkerCalculator(viewInfo, appointmentViewInfos, splitIndicatorType);
			}
			if (viewInfo is WeekViewInfo) {
				if (groupType == SchedulerGroupType.Date)
					return new MultipleMarkerCalculator(viewInfo, appointmentViewInfos, splitIndicatorType);
				else
					return new SingleMarkerCalculator(viewInfo, appointmentViewInfos, splitIndicatorType);
			}
			if (viewInfo is TimelineViewInfo)
				return new MultipleMarkerCalculator(viewInfo, appointmentViewInfos, splitIndicatorType);
			System.Diagnostics.Debug.Assert(true, "MarkerCalculator.CreateConverter()");
			return null;
		}
		#endregion
		protected internal abstract void Calculate(SplitIndicatorMarkersInfo splitIndicatorMarkersInfo, AppointmentViewInfo appointmentViewInfo, SplitIndicatorInfo splitIndicatorInfo, Point position);
	}
	#endregion
	#region SingleMarkerCalculator
	public class SingleMarkerCalculator : MarkerCalculator {
		public SingleMarkerCalculator(SchedulerViewInfoBase viewInfo, AppointmentViewInfoCollection appointmentViewInfos, SplitIndicatorType splitIndicatorType)
			: base(viewInfo, appointmentViewInfos, splitIndicatorType) {
		}
		#region Calculate
		protected internal override void Calculate(SplitIndicatorMarkersInfo splitIndicatorMarkersInfo, AppointmentViewInfo appointmentViewInfo, SplitIndicatorInfo splitIndicatorInfo, Point position) {
			splitIndicatorMarkersInfo.Reset();
			List<Rectangle> markerRectangles = new List<Rectangle>();
			if (appointmentViewInfo == null)
				return;
			Rectangle bounds = appointmentViewInfo.Bounds;
			Rectangle indicatorRectangle = splitIndicatorInfo.PrimaryRectangle;
			if (appointmentViewInfo is VerticalAppointmentViewInfo)
				markerRectangles.Add(new Rectangle(bounds.X, indicatorRectangle.Y, bounds.Width, 1));
			else
				markerRectangles.Add(new Rectangle(indicatorRectangle.X, bounds.Y, 1, bounds.Height));
			splitIndicatorMarkersInfo.AddPrimaryMarkers(markerRectangles);
		}
		#endregion
	}
	#endregion
	#region MultipleMarkerCalculator
	public class MultipleMarkerCalculator : MarkerCalculator {
		public MultipleMarkerCalculator(SchedulerViewInfoBase viewInfo, AppointmentViewInfoCollection appointmentViewInfos, SplitIndicatorType splitIndicatorType)
			: base(viewInfo, appointmentViewInfos, splitIndicatorType) {
		}
		#region Calculate
		protected internal override void Calculate(SplitIndicatorMarkersInfo splitIndicatorMarkersInfo, AppointmentViewInfo appointmentViewInfo, SplitIndicatorInfo splitIndicatorInfo, Point position) {
			splitIndicatorMarkersInfo.Reset();
			List<Rectangle> markerRectangles = new List<Rectangle>();
			if (AppointmentViewInfos.Count == 0 || appointmentViewInfo == null)
				return;
			Rectangle indicatorRectangle = splitIndicatorInfo.PrimaryRectangle;
			SelectableIntervalViewInfo viewCell = CalculateCurrentViewCell(AppointmentViewInfos, position);
			int count = AppointmentViewInfos.Count;
			for (int i = 0; i < count; i++) {
				AppointmentViewInfo appointmentViewInfoItem = AppointmentViewInfos[i];
				if (IsIntervalsIntersectsWithoutBorders(viewCell.Interval, appointmentViewInfoItem.Interval)) {
					Rectangle marker = GetSingleSplitMarkerRect(appointmentViewInfoItem, indicatorRectangle);
					markerRectangles.Add(marker);
				}
			}
			splitIndicatorMarkersInfo.AddPrimaryMarkers(markerRectangles);
		}
		#endregion
		#region IsIntervalsIntersectsWithoutBorders
		protected internal virtual bool IsIntervalsIntersectsWithoutBorders(TimeInterval intervalContainer, TimeInterval interval) {
			return ((intervalContainer.End > interval.Start) && (intervalContainer.Start < interval.End));
		}
		#endregion
		#region CalculateCurrentViewCell
		protected internal virtual SelectableIntervalViewInfo CalculateCurrentViewCell(AppointmentViewInfoCollection appointmentViewInfos, Point position) {
			List<SelectableIntervalViewInfo> viewCells = GetViewCellsIntersectedWidthVirtualSplitIndicator(appointmentViewInfos, position);
			int count = viewCells.Count;
			if (count == 0)
				return SelectableIntervalViewInfo.Empty;
			SelectableIntervalViewInfo firstViewCell = viewCells[0];
			TimeInterval firstCellInterval = firstViewCell.Interval;
			for (int i = 1; i < count; i++) {
				TimeInterval verifiableCellInterval = viewCells[i].Interval;
				if (!firstCellInterval.Equals(verifiableCellInterval)) {
					SchedulerHitInfo hitInfo = ViewInfo.CalcHitInfo(position, true);
					SchedulerHitInfo cellHitInfo = hitInfo.FindHitInfo(SchedulerHitTest.Cell);
					return cellHitInfo.ViewInfo;
				}
			}
			return firstViewCell;
		}
		#endregion
		#region GetViewCellsIntersectedWidthVirtualSplitIndicator
		protected internal virtual List<SelectableIntervalViewInfo> GetViewCellsIntersectedWidthVirtualSplitIndicator(AppointmentViewInfoCollection appointmentViewInfos, Point position) {
			SelectableIntervalViewInfo viewCell = SelectableIntervalViewInfo.Empty;
			List<SelectableIntervalViewInfo> selectableIntervalViewInfos = new List<SelectableIntervalViewInfo>();
			int count = appointmentViewInfos.Count;
			for (int i = 0; i < count; i++) {
				AppointmentViewInfo appointmentViewInfo = appointmentViewInfos[i];
				Point correctedPoint = CalculateCorrectedPoint(appointmentViewInfo, position);
				SchedulerHitInfo hitInfo = ViewInfo.CalcHitInfo(correctedPoint, true);
				SchedulerHitInfo cellHitInfo = hitInfo.FindHitInfo(SchedulerHitTest.Cell);
				viewCell = cellHitInfo.ViewInfo;
				if (viewCell != SelectableIntervalViewInfo.Empty)
					selectableIntervalViewInfos.Add(viewCell);
			}
			return selectableIntervalViewInfos;
		}
		#endregion
		#region GetSingleSplitMarkerRect
		protected internal virtual Rectangle GetSingleSplitMarkerRect(AppointmentViewInfo appointmentViewInfo, Rectangle indicatorRectangle) {
			Rectangle bounds = appointmentViewInfo.Bounds;
			if (appointmentViewInfo is VerticalAppointmentViewInfo)
				return new Rectangle(bounds.X, indicatorRectangle.Y, bounds.Width, 1);
			return new Rectangle(indicatorRectangle.X, bounds.Y, 1, bounds.Height);
		}
		#endregion
		#region CalculateCorrectedPoint
		protected internal virtual Point CalculateCorrectedPoint(AppointmentViewInfo appointmentViewInfo, Point position) {
			Rectangle appointmentViewInfoBounds = appointmentViewInfo.Bounds;
			if (SplitIndicatorType == SplitIndicatorType.Horizontal) {
				int x = appointmentViewInfoBounds.X + appointmentViewInfoBounds.Width / 2;
				return new Point(x, position.Y);
			}
			int y = appointmentViewInfoBounds.Y + appointmentViewInfoBounds.Height / 2;
			return new Point(position.X, y);
		}
		#endregion
	}
	#endregion
	#region AppointmentViewInfoPositionTimeConverter
	public abstract class AppointmentViewInfoPositionTimeConverter {
		#region CreateConverter
		public static AppointmentViewInfoPositionTimeConverter CreateConverter(AppointmentViewInfo appointmentViewInfo, SchedulerViewInfoBase viewInfo) {
			if (viewInfo is DayViewInfo) {
				if (appointmentViewInfo is VerticalAppointmentViewInfo)
					return new DayViewVerticalAppointmentViewInfoPositionTimeConverter(appointmentViewInfo, viewInfo);
				else
					return new DayViewHorizontalAppointmentViewInfoPositionTimeConverter(appointmentViewInfo, viewInfo);
			}
			if (viewInfo is WeekViewInfo)
				return new AppointmentViewInfoPositionTimeConverterWeekView(appointmentViewInfo, viewInfo);
			if (viewInfo is TimelineViewInfo)
				return new AppointmentViewInfoPositionTimeConverterTimelineView(appointmentViewInfo, viewInfo);
			return null;
		}
		#endregion
		#region Fields
		AppointmentViewInfo appointmentViewInfo;
		SchedulerViewInfoBase viewInfo;
		#endregion
		protected AppointmentViewInfoPositionTimeConverter(AppointmentViewInfo appointmentViewInfo, SchedulerViewInfoBase viewInfo) {
			if (appointmentViewInfo == null)
				Exceptions.ThrowArgumentNullException("appointmentViewInfo");
			if (viewInfo == null)
				Exceptions.ThrowArgumentNullException("viewInfo");
			this.appointmentViewInfo = appointmentViewInfo;
			this.viewInfo = viewInfo;
		}
		#region Properties
		public AppointmentViewInfo AppointmentViewInfo { get { return appointmentViewInfo; } }
		public SchedulerViewInfoBase SchedulerViewInfo { get { return viewInfo; } }
		#endregion
		#region ConvertTimeToPosition
		protected internal virtual Point ConvertTimeToPosition(DateTime spliTime) {
			return new Point();
		}
		#endregion
		#region ConvertPositionToTime
		protected internal virtual DateTime ConvertPositionToTime(Point mousePosition) {
			return new DateTime();
		}
		#endregion
	}
	#endregion
	#region DayViewHorizontalAppointmentViewInfoPositionTimeConverter
	public class DayViewHorizontalAppointmentViewInfoPositionTimeConverter : AppointmentViewInfoPositionTimeConverter {
		List<AllDayAreaCell> viewCells;
		public DayViewHorizontalAppointmentViewInfoPositionTimeConverter(AppointmentViewInfo appointmentViewInfo, SchedulerViewInfoBase viewInfo)
			: base(appointmentViewInfo, viewInfo) {
			this.viewCells = GetViewCells(appointmentViewInfo, SchedulerViewInfo);
		}
		#region Properties
		public new DayViewInfo SchedulerViewInfo { get { return (DayViewInfo)base.SchedulerViewInfo; } }
		public List<AllDayAreaCell> ViewCells { get { return viewCells; } }
		#endregion
		#region GetViewCells
		protected internal virtual List<AllDayAreaCell> GetViewCells(AppointmentViewInfo appointmentViewInfo, DayViewInfo viewInfo) {
			List<AllDayAreaCell> result = new List<AllDayAreaCell>();
			int countColumns = viewInfo.Columns.Count;
			for (int i = 0; i < countColumns; i++) {
				if (((DayViewColumn)viewInfo.Columns[i]).AllDayAreaCell.Bounds.IntersectsWith(appointmentViewInfo.Bounds))
					result.Add(((DayViewColumn)viewInfo.Columns[i]).AllDayAreaCell);
			}
			return result;
		}
		#endregion
		#region GetActiveViewcell
		protected internal virtual SchedulerViewCellBase GetActiveViewCell(Point mousePosition) {
			int count = ViewCells.Count;
			for (int i = 0; i < count; i++) {
				if (RectUtils.ContainsX(ViewCells[i].Bounds, mousePosition))
					return ViewCells[i];
			}
			return null;
		}
		#endregion
		#region ConvertPositionToTime
		protected internal override DateTime ConvertPositionToTime(Point mousePosition) {
			SchedulerViewCellBase viewCell = GetActiveViewCell(mousePosition);
			Rectangle viewCellBounds = viewCell.Bounds;
			double proportionalityRatio = (mousePosition.X - viewCellBounds.Left) / (double)viewCellBounds.Width;
			TimeSpan offset = new TimeSpan((long)(proportionalityRatio * viewCell.Interval.Duration.Ticks));
			DateTime result = viewCell.Interval.Start + offset;
			return result;
		}
		#endregion
		#region GetViewCell
		protected internal virtual SchedulerViewCellBase GetViewCell(DateTime splitDateTime) {
			int count = ViewCells.Count;
			for (int i = 0; i < count; i++) {
				if (ViewCells[i].Interval.Contains(splitDateTime))
					return ViewCells[i];
			}
			return null;
		}
		#endregion
		#region ConvertTimeToPosition
		protected internal override Point ConvertTimeToPosition(DateTime splitTime) {
			SchedulerViewCellBase viewCell = GetViewCell(splitTime);
			Rectangle viewCellBounds = viewCell.Bounds;
			TimeSpan offset = splitTime - viewCell.Interval.Start;
			double proportionalityRatio = (double)offset.Ticks / viewCell.Interval.Duration.Ticks;
			int x = (int)(Math.Round(proportionalityRatio * (double)viewCellBounds.Width + viewCellBounds.Left));
			Rectangle bounds = SchedulerViewInfo.Bounds;
			return new Point(x - 1, bounds.Top);
		}
		#endregion
	}
	#endregion
	#region DayViewVerticalAppointmentViewInfoPositionTimeConverter
	public class DayViewVerticalAppointmentViewInfoPositionTimeConverter : AppointmentViewInfoPositionTimeConverter {
		SchedulerViewCellBaseCollection viewCells;
		public DayViewVerticalAppointmentViewInfoPositionTimeConverter(AppointmentViewInfo appointmentViewInfo, SchedulerViewInfoBase viewInfo)
			: base(appointmentViewInfo, viewInfo) {
			this.viewCells = GetViewCells(appointmentViewInfo, SchedulerViewInfo);
		}
		#region Properties
		public new DayViewInfo SchedulerViewInfo { get { return (DayViewInfo)base.SchedulerViewInfo; } }
		public SchedulerViewCellBaseCollection ViewCells { get { return viewCells; } }
		#endregion
		#region GetViewCells
		protected internal virtual SchedulerViewCellBaseCollection GetViewCells(AppointmentViewInfo appointmentViewInfo, DayViewInfo viewInfo) {
			SchedulerViewCellBaseCollection result = new SchedulerViewCellBaseCollection();
			int countColumns = viewInfo.Columns.Count;
			for (int i = 0; i < countColumns; i++) {
				if (viewInfo.Columns[i].Interval.IntersectsWith(appointmentViewInfo.Interval)) {
					int countCells = viewInfo.Columns[i].Cells.Count;
					SchedulerViewCellContainer currentColumn = viewInfo.Columns[i];
					for (int j = 0; j < countCells; j++) {
						if (currentColumn.Cells[j].Interval.IntersectsWith(appointmentViewInfo.Interval))
							result.Add(currentColumn.Cells[j]);
					}
				}
			}
			return result;
		}
		#endregion
		#region ConvertPositionToTime
		protected internal override DateTime ConvertPositionToTime(Point mousePosition) {
			Point correctedPoint = CalculateCorrectedPoint(mousePosition);
			SelectableIntervalViewInfo viewCell = GetViewCell(correctedPoint);
			if (viewCell == SelectableIntervalViewInfo.Empty)
				return DateTime.MinValue;
			Rectangle viewCellBounds = viewCell.Bounds;
			double proportionalityRatio = (mousePosition.Y - viewCellBounds.Top) / (double)viewCellBounds.Height;
			TimeSpan offset = new TimeSpan((long)(proportionalityRatio * viewCell.Interval.Duration.Ticks));
			DateTime result = viewCell.Interval.Start + offset;
			return result;
		}
		#endregion
		#region GetViewCell
		protected internal virtual SelectableIntervalViewInfo GetViewCell(Point correctedPoint) {
			SchedulerHitInfo hitInfo = SchedulerViewInfo.CalcHitInfo(correctedPoint, true);
			SelectableIntervalViewInfo viewCell = hitInfo.ViewInfo;
			if (viewCell is AllDayAreaCell)
				return SelectableIntervalViewInfo.Empty;
			return viewCell;
		}
		#endregion
		#region CalculateCorrectedPoint
		protected internal Point CalculateCorrectedPoint(Point mousePosition) {
			Rectangle appointmentViewInfoBounds = AppointmentViewInfo.Bounds;
			int x = appointmentViewInfoBounds.X + appointmentViewInfoBounds.Width / 2;
			Point correctedPoint = new Point(x, mousePosition.Y);
			return correctedPoint;
		}
		#endregion
		#region GetViewCell
		protected internal virtual SchedulerViewCellBase GetViewCell(DateTime splitDateTime) {
			int count = ViewCells.Count;
			for (int i = 0; i < count; i++) {
				if (ViewCells[i].Interval.Contains(splitDateTime))
					return ViewCells[i];
			}
			return null;
		}
		#endregion
		#region ConvertTimeToPosition
		protected internal override Point ConvertTimeToPosition(DateTime splitTime) {
			SchedulerViewCellBase viewCell = GetViewCell(splitTime);
			if (viewCell == null)
				return Point.Empty;
			Rectangle viewCellBounds = viewCell.Bounds;
			TimeSpan offset = splitTime - viewCell.Interval.Start;
			double proportionalityRatio = (double)offset.Ticks / viewCell.Interval.Duration.Ticks;
			int y = (int)(Math.Round(proportionalityRatio * (double)viewCellBounds.Height + viewCellBounds.Top));
			Rectangle bounds = SchedulerViewInfo.Bounds;
			return new Point(bounds.Left, y - 1);
		}
		#endregion
	}
	#endregion
	#region AppointmentViewInfoPositionTimeConverterTimelineView
	public class AppointmentViewInfoPositionTimeConverterTimelineView : AppointmentViewInfoPositionTimeConverter {
		SchedulerViewCellBaseCollection viewCells;
		public AppointmentViewInfoPositionTimeConverterTimelineView(AppointmentViewInfo appointmentViewInfo, SchedulerViewInfoBase viewInfo)
			: base(appointmentViewInfo, viewInfo) {
			this.viewCells = GetViewCells(appointmentViewInfo, SchedulerViewInfo);
		}
		#region Properties
		public new TimelineViewInfo SchedulerViewInfo { get { return (TimelineViewInfo)base.SchedulerViewInfo; } }
		public SchedulerViewCellBaseCollection ViewCells { get { return viewCells; } }
		#endregion
		#region GetViewCells
		protected internal virtual SchedulerViewCellBaseCollection GetViewCells(AppointmentViewInfo appointmentViewInfo, TimelineViewInfo viewInfo) {
			SchedulerViewCellBaseCollection result = new SchedulerViewCellBaseCollection();
			int countTimelines = viewInfo.Timelines.Count;
			for (int i = 0; i < countTimelines; i++) {
				if (viewInfo.Timelines[i].Bounds.IntersectsWith(appointmentViewInfo.Bounds)) {
					int countCells = viewInfo.Timelines[i].Cells.Count;
					SchedulerViewCellContainer currentTimeline = viewInfo.Timelines[i];
					for (int j = 0; j < countCells; j++) {
						if (currentTimeline.Cells[j].Bounds.IntersectsWith(appointmentViewInfo.Bounds))
							result.Add(currentTimeline.Cells[j]);
					}
				}
			}
			return result;
		}
		#endregion
		#region GetActiveViewcell
		protected internal virtual SchedulerViewCellBase GetActiveViewCell(Point mousePosition) {
			int count = ViewCells.Count;
			for (int i = 0; i < count; i++) {
				if (RectUtils.ContainsX(ViewCells[i].Bounds, mousePosition))
					return ViewCells[i];
			}
			return null;
		}
		#endregion
		#region ConvertPositionToTime
		protected internal override DateTime ConvertPositionToTime(Point mousePosition) {
			SchedulerViewCellBase viewCell = GetActiveViewCell(mousePosition);
			if (viewCell == null)
				return new DateTime();
			Rectangle viewCellBounds = viewCell.Bounds;
			double proportionalityRatio = (mousePosition.X - viewCellBounds.Left) / (double)viewCellBounds.Width;
			TimeSpan offset = new TimeSpan((long)(proportionalityRatio * viewCell.Interval.Duration.Ticks));
			DateTime result = viewCell.Interval.Start + offset;
			return result;
		}
		#endregion
		#region GetViewCell
		protected internal virtual SchedulerViewCellBase GetViewCell(DateTime splitDateTime) {
			int count = ViewCells.Count;
			for (int i = 0; i < count; i++) {
				if (ViewCells[i].Interval.Contains(splitDateTime))
					return ViewCells[i];
			}
			return null;
		}
		#endregion
		#region ConvertTimeToPosition
		protected internal override Point ConvertTimeToPosition(DateTime splitTime) {
			SchedulerViewCellBase viewCell = GetViewCell(splitTime);
			Rectangle viewCellBounds = viewCell.Bounds;
			TimeSpan offset = splitTime - viewCell.Interval.Start;
			double proportionalityRatio = (double)offset.Ticks / viewCell.Interval.Duration.Ticks;
			int x = (int)(Math.Round(proportionalityRatio * (double)viewCellBounds.Width + viewCellBounds.Left));
			Rectangle bounds = SchedulerViewInfo.Bounds;
			return new Point(x, bounds.Top);
		}
		#endregion
	}
	#endregion
	#region AppointmentViewInfoPositionTimeConverterWeekView
	public class AppointmentViewInfoPositionTimeConverterWeekView : AppointmentViewInfoPositionTimeConverter {
		SchedulerViewCellBaseCollection viewCells;
		public AppointmentViewInfoPositionTimeConverterWeekView(AppointmentViewInfo appointmentViewInfo, SchedulerViewInfoBase viewInfo)
			: base(appointmentViewInfo, viewInfo) {
			this.viewCells = GetViewCells(appointmentViewInfo, SchedulerViewInfo);
		}
		#region Properties
		public new WeekViewInfo SchedulerViewInfo { get { return (WeekViewInfo)base.SchedulerViewInfo; } }
		public SchedulerViewCellBaseCollection ViewCells { get { return viewCells; } }
		#endregion
		#region GetViewCells
		protected internal virtual SchedulerViewCellBaseCollection GetViewCells(AppointmentViewInfo appointmentViewInfo, WeekViewInfo viewInfo) {
			SchedulerViewCellBaseCollection result = new SchedulerViewCellBaseCollection();
			int countWeeks = viewInfo.Weeks.Count;
			for (int i = 0; i < countWeeks; i++) {
				if (viewInfo.Weeks[i].Bounds.IntersectsWith(appointmentViewInfo.Bounds)) {
					int countCells = viewInfo.Weeks[i].Cells.Count;
					SchedulerViewCellContainer currentWeek = viewInfo.Weeks[i];
					for (int j = 0; j < countCells; j++) {
						if (currentWeek.Cells[j].Bounds.IntersectsWith(appointmentViewInfo.Bounds))
							result.Add(currentWeek.Cells[j]);
					}
				}
			}
			return result;
		}
		#endregion
		#region GetActiveViewcell
		protected internal virtual SchedulerViewCellBase GetActiveViewCell(Point mousePosition) {
			int count = ViewCells.Count;
			for (int i = 0; i < count; i++) {
				if (RectUtils.ContainsX(ViewCells[i].Bounds, mousePosition))
					return ViewCells[i];
			}
			return null;
		}
		#endregion
		#region ConvertPositionToTime
		protected internal override DateTime ConvertPositionToTime(Point mousePosition) {
			SchedulerViewCellBase viewCell = GetActiveViewCell(mousePosition);
			if (viewCell == null)
				return new DateTime();
			Rectangle viewCellBounds = viewCell.Bounds;
			int viewCellBoundsWidth = viewCell.Bounds.Width;
			if (!((SchedulerViewCellBase)viewCell).HasLeftBorder && !((SchedulerViewCellBase)viewCell).HasRightBorder) {
				int overlap = SchedulerViewInfo.Painter.HorizontalHeaderPainter.HorizontalOverlap;
				viewCellBoundsWidth = viewCellBounds.Width - overlap;
			}
			double proportionalityRatio = (mousePosition.X - viewCellBounds.Left) / (double)viewCellBoundsWidth;
			TimeSpan offset = new TimeSpan((long)(proportionalityRatio * viewCell.Interval.Duration.Ticks));
			DateTime result = viewCell.Interval.Start + offset;
			return result;
		}
		#endregion
		#region GetViewCell
		protected internal virtual SchedulerViewCellBase GetViewCell(DateTime splitDateTime) {
			int count = ViewCells.Count;
			for (int i = 0; i < count; i++) {
				if (ViewCells[i].Interval.Contains(splitDateTime))
					return ViewCells[i];
			}
			return null;
		}
		#endregion
		#region ConvertTimeToPosition
		protected internal override Point ConvertTimeToPosition(DateTime splitTime) {
			SchedulerViewCellBase viewCell = GetViewCell(splitTime);
			Rectangle viewCellBounds = viewCell.ContentBounds;
			int viewCellBoundsWidth = viewCell.Bounds.Width;
			if (!((SchedulerViewCellBase)viewCell).HasLeftBorder && !((SchedulerViewCellBase)viewCell).HasRightBorder) {
				int overlap = SchedulerViewInfo.Painter.HorizontalHeaderPainter.HorizontalOverlap;
				viewCellBoundsWidth = viewCellBounds.Width - overlap;
			}
			TimeSpan offset = splitTime - viewCell.Interval.Start;
			double proportionalityRatio = (double)offset.Ticks / viewCell.Interval.Duration.Ticks;
			int x = (int)(Math.Round(proportionalityRatio * (double)viewCellBoundsWidth + viewCellBounds.Left));
			Rectangle bounds = SchedulerViewInfo.Bounds;
			return new Point(x, bounds.Top);
		}
		#endregion
	}
	#endregion
	#region SplitAppointmentOperationToolTipController
	[System.ComponentModel.DXToolboxItem(false)]
	public class SplitAppointmentOperationToolTipController : ToolTipController {
		#region ShowHintCore
		protected override void ShowHintCore(ToolTipControllerShowEventArgs eShow) {
			eShow.AutoHide = false;
			base.ShowHintCore(eShow);
			this.ToolWindow.Update();
		}
		#endregion
	}
	#endregion
	#region SplitIndicatorInfoCalculatorFactory
	public static class SplitIndicatorInfoCalculatorFactory {
		public static SplitIndicatorInfoCalculator Create(SplitIndicatorInfo splitIndicatorInfo, SchedulerViewInfoBase viewInfo) {
			if (viewInfo is DayViewInfo)
				return new DayViewSplitIndicatorInfoCalculator(splitIndicatorInfo, viewInfo);
			else
				return new SplitIndicatorInfoCalculator(splitIndicatorInfo, viewInfo);
		}
	}
	#endregion
	#region SplitIndicatorInfoCalculator
	public class SplitIndicatorInfoCalculator {
		#region Fields
		SplitIndicatorInfo splitIndicatorInfo;
		SchedulerViewInfoBase viewInfo;
		SplitIndicatorType splitIndicatorType;
		#endregion
		public SplitIndicatorInfoCalculator(SplitIndicatorInfo splitIndicatorInfo, SchedulerViewInfoBase viewInfo) {
			if (splitIndicatorInfo == null)
				Exceptions.ThrowArgumentNullException("splitIndicatorInfo");
			if (viewInfo == null)
				Exceptions.ThrowArgumentNullException("viewInfo");
			this.splitIndicatorInfo = splitIndicatorInfo;
			this.viewInfo = viewInfo;
			this.splitIndicatorType = SplitIndicatorInfo.Type;
		}
		#region Properties
		public SplitIndicatorInfo SplitIndicatorInfo { get { return splitIndicatorInfo; } }
		public SchedulerViewInfoBase ViewInfo { get { return viewInfo; } }
		public SplitIndicatorType SplitIndicatorType { get { return splitIndicatorType; } }
		#endregion
		#region Calculate
		public virtual void Calculate(AppointmentViewInfo appointmentViewInfo, DateTime splitTime, Point mousePosition) {
			SplitIndicatorInfo.PrimaryRectangle = GetSplitIndicatorRect(appointmentViewInfo, splitTime, mousePosition);
		}
		#endregion
		#region GetSplitIndicatorRect
		protected internal virtual Rectangle GetSplitIndicatorRect(AppointmentViewInfo appointmentViewInfo, DateTime splitTime, Point mousePosition) {
			Rectangle viewInfoBounds = ViewInfo.Bounds;
			Point startPoint = CalcSplitIndicatorStartPoint(mousePosition, appointmentViewInfo, splitTime);
			Size size = (SplitIndicatorType == SplitIndicatorType.Vertical) ? new Size(1, viewInfoBounds.Height) : new Size(viewInfoBounds.Width, 1);
			return new Rectangle(startPoint, size);
		}
		#endregion
		#region CalcSplitIndicatorStartPoint
		protected internal virtual Point CalcSplitIndicatorStartPoint(Point mousePosition, AppointmentViewInfo appointmentViewInfo, DateTime splitTime) {
			if (appointmentViewInfo != null && splitTime != DateTime.MinValue)
				return GetPositionForDateTime(appointmentViewInfo, ViewInfo, splitTime);
			Rectangle bounds = ViewInfo.Bounds;
			if (SplitIndicatorType == SplitIndicatorType.Vertical)
				return new Point(mousePosition.X, bounds.Y);
			return new Point(bounds.Y, mousePosition.Y);
		}
		#endregion
		#region GetPositionForDateTime
		protected internal virtual Point GetPositionForDateTime(AppointmentViewInfo appointmentViewInfo, SchedulerViewInfoBase viewInfo, DateTime splitTime) {
			AppointmentViewInfoPositionTimeConverter converter = AppointmentViewInfoPositionTimeConverter.CreateConverter(appointmentViewInfo, viewInfo);
			return converter.ConvertTimeToPosition(splitTime);
		}
		#endregion
	}
	#endregion
	#region DayViewSplitIndicatorInfoCalculator
	public class DayViewSplitIndicatorInfoCalculator : SplitIndicatorInfoCalculator {
		public DayViewSplitIndicatorInfoCalculator(SplitIndicatorInfo splitIndicatorInfo, SchedulerViewInfoBase viewInfo) : base(splitIndicatorInfo, viewInfo) { }
		public new DayViewInfo ViewInfo { get { return base.ViewInfo as DayViewInfo; } }
		public override void Calculate(AppointmentViewInfo appointmentViewInfo, DateTime splitTime, Point mousePosition) {
			base.Calculate(appointmentViewInfo, splitTime, mousePosition);
			if (ViewInfo == null || SplitIndicatorType == SplitIndicatorType.Vertical)
				return;
			int width = CalcTimeRulersWidth(ViewInfo.TimeRulers);
			Rectangle bounds = SplitIndicatorInfo.PrimaryRectangle;
			Rectangle newBounds = new Rectangle(bounds.X + width, bounds.Y, bounds.Width - width, bounds.Height);
			SplitIndicatorInfo.PrimaryRectangle = newBounds;
		}
		#region CalcTimeRulersWidth
		protected internal virtual int CalcTimeRulersWidth(TimeRulerViewInfoCollection timeRulers) {
			int width = 0;
			int count = timeRulers.Count;
			for (int i = 0; i < count; i++)
				width += timeRulers[i].ContentBounds.Width;
			return width;
		}
		#endregion
	}
	#endregion
}

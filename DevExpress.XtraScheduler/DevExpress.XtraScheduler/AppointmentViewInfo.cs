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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler {
	#region AppointmentViewInfoTabOrderComparer
	public class AppointmentViewInfoTabOrderComparer : IComparer<AppointmentViewInfo> {
		public int Compare(AppointmentViewInfo x, AppointmentViewInfo y) {
			Appointment aptX = x.Appointment;
			Appointment aptY = y.Appointment;
			int result = Comparer.Default.Compare(aptX.Start.Date, aptY.Start.Date);
			if (result != 0)
				return result;
			result = SeveralDaysAppointmentCore(aptY) - SeveralDaysAppointmentCore(aptX);
			if (result != 0)
				return result;
			result = Comparer.Default.Compare(aptX.Start.TimeOfDay, aptY.Start.TimeOfDay);
			if (result != 0)
				return result;
			result = -Comparer.Default.Compare(aptX.End, aptY.End);
			if (result != 0)
				return result;
			result = Comparer.Default.Compare(aptX.RowHandle, aptY.RowHandle);
			if (result != 0)
				return result;
			aptX = aptX.RecurrencePattern;
			aptY = aptY.RecurrencePattern;
			if (aptX == null)
				return (aptY == null) ? 0 : 1;
			else {
				if (aptY == null)
					return -1;
				else
					return Comparer.Default.Compare(aptX.RowHandle, aptY.RowHandle);
			}
		}
		protected internal virtual int SeveralDaysAppointmentCore(Appointment apt) {
			return apt.LongerThanADay ? 1 : 0;
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Drawing {
	#region FitIntoCellResult
	public enum FitIntoCellResult {
		Fitted = 0,
		Truncated = 1,
		NotFitted = 2
	}
	#endregion
	#region AppointmentStatusViewInfo
	public class AppointmentStatusViewInfo : BorderObjectViewInfo {
		Brush brush;
		readonly IAppointmentStatus status;
		Color borderColor;
		public AppointmentStatusViewInfo(IAppointmentStatus status, Color borderColor) {
			Guard.ArgumentNotNull(status, "status");
			this.status = status;
			this.borderColor = borderColor;
		}
		public AppointmentStatusViewInfo(AppointmentViewInfo viewInfo) {
			Guard.ArgumentNotNull(viewInfo, "viewInfo");
			this.status = viewInfo.Status;
			this.borderColor = viewInfo.Appearance.GetBorderColor();
		}
		public Brush Brush { 
			get {
				if (brush == null)
					brush = Status.GetBrush();
				return brush;
			}
			set { brush = value; }
		}
		public IAppointmentStatus Status { get { return status; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.None; } }
		protected internal Brush GetBrush(GraphicsCache cache) {
			if (brush != null)
				return brush;
			return Status.GetBrush();
		}
		protected internal Brush GetBrush() {
			if (brush != null)
				return brush;
			return Status.GetBrush();
		}
		protected internal virtual void SetBrush(Brush brush) {
			this.brush = brush;
		}
	}
	#endregion
	#region AppointmentStatusViewInfoCollection
	public class AppointmentStatusViewInfoCollection : DXCollection<AppointmentStatusViewInfo> {
	}
	#endregion
	#region AppointmentIntermediateViewInfo
	public class AppointmentIntermediateViewInfo : AppointmentIntermediateViewInfoCore {
		#region Fields
		IVisuallyContinuousCellsInfo cellsInfo;
		int relativePosition;
		Size size;
		#endregion
		#region Properties
		public int Height { get { return size.Height; } set { size.Height = value; } }
		public int Width { get { return size.Width; } set { size.Width = value; } }
		public Rectangle Bounds { get { return ViewInfo.Bounds; } set { ViewInfo.Bounds = value; } }
		public int RelativePosition { get { return relativePosition; } set { relativePosition = value; } }
		public new AppointmentViewInfo ViewInfo { get { return (AppointmentViewInfo)base.ViewInfo; } }
		public IVisuallyContinuousCellsInfo CellsInfo { get { return cellsInfo; } }
		#endregion
		public AppointmentIntermediateViewInfo(AppointmentViewInfo viewInfo, IVisuallyContinuousCellsInfo cellsInfo)
			: base(viewInfo) {
			Guard.ArgumentNotNull(cellsInfo, "cellsInfo");
			this.cellsInfo = cellsInfo;
		}
	}
	#endregion
	#region AppointmentIntermediateViewInfoComparer
	public class AppointmentIntermediateViewInfoComparer : AppointmentIntermediateViewInfoCoreComparer<AppointmentIntermediateViewInfo> {
		public AppointmentIntermediateViewInfoComparer(AppointmentBaseComparer aptComparer)
			: base(aptComparer) {
		}
	}
	#endregion
	#region AppointmentIntermediateViewInfoCollection
	public class AppointmentIntermediateViewInfoCollection : DXCollection<AppointmentIntermediateViewInfo>, IAppointmentIntermediateViewInfoCoreCollection, IAppointmentIntermediateLayoutViewInfoCoreCollection {
		Resource resource;
		TimeInterval interval;
		List<IAppointmentIntermediateLayoutViewInfoCoreCollection> groupedViewInfos;
		ManualResetEventSlim signal;
		public AppointmentIntermediateViewInfoCollection(Resource resource, TimeInterval interval) {
			this.resource = resource;
			this.interval = interval;
			this.groupedViewInfos = new List<IAppointmentIntermediateLayoutViewInfoCoreCollection>();
			signal = new ManualResetEventSlim();
		}
		public AppointmentIntermediateViewInfoCollection() {
		}
		public void SetResourceAndInterval(Resource resource, TimeInterval interval) {
			this.resource = resource;
			this.interval = interval;
		}
		#region IAppointmentIntermediateViewInfoCoreCollection Members
		AppointmentIntermediateViewInfoCore IAppointmentIntermediateViewInfoCoreCollection.this[int index] { get { return base[index]; } }
		bool IAppointmentIntermediateViewInfoCoreCollection.Remove(AppointmentIntermediateViewInfoCore value) {
			return Remove((AppointmentIntermediateViewInfo)value);
		}
		int IAppointmentIntermediateViewInfoCoreCollection.Add(AppointmentIntermediateViewInfoCore value) {
			return Add((AppointmentIntermediateViewInfo)value);
		}
		void IAppointmentIntermediateViewInfoCoreCollection.Sort(AppointmentBaseComparer aptComparer) {
			Sort(new AppointmentIntermediateViewInfoComparer(aptComparer));
		}
		public Resource Resource {
			get { return this.resource; }
		}
		public TimeInterval Interval {
			get { return this.interval; }
		}
		public List<IAppointmentIntermediateLayoutViewInfoCoreCollection> GroupedViewInfos {
			get { return this.groupedViewInfos; }
		}
		public ManualResetEventSlim Signal {
			get { return this.signal; }
		}
		#endregion
		#region IAppointmentIntermediateLayoutViewInfoCoreCollection Members
		IAppointmentIntermediateLayoutViewInfo IAppointmentIntermediateLayoutViewInfoCoreCollection.this[int index] {
			get { return this[index]; }
		}
		#endregion
	}
	#endregion
	#region HorizontalAppointmentIntermediateLayoutCalculator
	public class HorizontalAppointmentIntermediateLayoutCalculator : HorizontalAppointmentIntermediateLayoutCalculatorCore {
		public HorizontalAppointmentIntermediateLayoutCalculator(TimeScale scale, AppointmentSnapToCellsMode appointmentsSnapToCells, TimeZoneHelper timeZoneEngine)
			: base(scale, appointmentsSnapToCells, timeZoneEngine) {
		}
		protected internal override AppointmentIntermediateViewInfoCore CreateAppointmentIntermediateViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo) {
			HorizontalAppointmentViewInfo viewInfo = new HorizontalAppointmentViewInfo(apt, TimeZoneHelper);
			return new AppointmentIntermediateViewInfo(viewInfo, (IVisuallyContinuousCellsInfo)cellsInfo);
		}
		protected internal override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection() {
			return new AppointmentIntermediateViewInfoCollection();
		}
		public override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection(Resource resource, TimeInterval interval) {
			return new AppointmentIntermediateViewInfoCollection(resource, interval);
		}
	}
	#endregion
	#region VerticalAppointmentLayoutCalculator
	public class VerticalAppointmentIntermediateLayoutCalculator : VerticalAppointmentIntermediateLayoutCalculatorCore {
		public VerticalAppointmentIntermediateLayoutCalculator(TimeScaleFixedInterval scale, AppointmentSnapToCellsMode appointmentsSnapToCells, TimeZoneHelper timeZoneEngine)
			: base(scale, appointmentsSnapToCells, timeZoneEngine) {
		}
		protected internal override AppointmentIntermediateViewInfoCore CreateAppointmentIntermediateViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo) {
			VerticalAppointmentViewInfo viewInfo = new VerticalAppointmentViewInfo(apt, TimeZoneHelper);
			return new AppointmentIntermediateViewInfo(viewInfo, (IVisuallyContinuousCellsInfo)cellsInfo);
		}
		protected internal override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection() {
			return new AppointmentIntermediateViewInfoCollection();
		}
		public override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection(Resource resource, TimeInterval interval) {
			return new AppointmentIntermediateViewInfoCollection(resource, interval);
		}
	}
	#endregion
	#region TimeLineAppointmentIntermediateLayoutCalculator
	public class TimeLineAppointmentIntermediateLayoutCalculator : HorizontalAppointmentIntermediateLayoutCalculator {
		public TimeLineAppointmentIntermediateLayoutCalculator(TimeScale scale, AppointmentSnapToCellsMode appointmentsSnapToCells, TimeZoneHelper timeZoneEngine)
			: base(scale, appointmentsSnapToCells, timeZoneEngine) {
		}
		protected internal override AppointmentIntermediateViewInfoCore CreateAppointmentIntermediateViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo) {
			TimeLineAppointmentViewInfo viewInfo = new TimeLineAppointmentViewInfo(apt, TimeZoneHelper);
			return new AppointmentIntermediateViewInfo(viewInfo, (IVisuallyContinuousCellsInfo)cellsInfo);
		}
	}
	#endregion
	#region BusyPosition
	public class BusyPosition {
		static readonly BusyPosition empty = new BusyPosition();
		public static BusyPosition Empty { get { return empty; } }
		int start;
		int end;
		internal BusyPosition() {
		}
		public BusyPosition(int start, int end) {
			if (start >= end)
				Exceptions.ThrowArgumentException("start", start);
			if (start < 0)
				Exceptions.ThrowArgumentException("start", start);
			this.start = start;
			this.end = end;
		}
		public int Start { get { return start; } }
		public int End { get { return end; } }
		public bool ContainsExcludeEndBound(int value) {
			return this.start <= value && this.end > value;
		}
		public bool ContainsExcludeStartBound(int value) {
			return this.start < value && this.end >= value;
		}
		public bool IntersectsWithExcludingBounds(int positionStart, int positionEnd) {
			XtraSchedulerDebug.Assert(positionStart <= positionEnd);
			if (positionEnd <= this.Start)
				return false;
			if (positionStart >= this.End)
				return false;
			return true;
		}
	}
	#endregion
	#region BusyPositionsCollection
	public class BusyPositionsCollection : DXCollection<BusyPosition> {
	}
	#endregion
	#region HorizontalAppointmentFixedHeightRelativePositionsCalculator
	public class HorizontalAppointmentFixedHeightRelativePositionsCalculator {
		int gapBetweenAppointments;
		public HorizontalAppointmentFixedHeightRelativePositionsCalculator(int gapBetweenAppointments) {
			this.gapBetweenAppointments = gapBetweenAppointments;
		}
		protected internal int GapBetweenAppointments { get { return gapBetweenAppointments; } }
		protected internal virtual void CalculateAppointmentRelativePositions(AppointmentIntermediateViewInfoCollection viewInfos, CancellationToken token) {
			AppointmentIndexesCalculator indexCalculator = new AppointmentIndexesCalculator();
			indexCalculator.CalculateAppointmentIndexes(token, viewInfos);
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				AppointmentIntermediateViewInfo viewInfo = viewInfos[i];
				viewInfo.RelativePosition = (viewInfo.Height + GapBetweenAppointments) * viewInfo.FirstIndexPosition;
			}
		}
	}
	#endregion
	#region HorizontalAppointmentAutoHeightRelativePositionsCalculator
	public class HorizontalAppointmentAutoHeightRelativePositionsCalculator {
		int gapBetweenAppointments;
		public HorizontalAppointmentAutoHeightRelativePositionsCalculator(int gapBetweenAppointments) {
			this.gapBetweenAppointments = gapBetweenAppointments;
		}
		protected internal int GapBetweenAppointments { get { return gapBetweenAppointments; } }
		public virtual void CalculateAppointmentRelativePositions(CancellationToken token, AppointmentIntermediateViewInfoCollection infos, int cellsMaxHeight) {
			CalculateAppointmentRelativePositions(token, infos, 0, cellsMaxHeight);
		}
		public virtual void CalculateAppointmentRelativePositions(CancellationToken token, AppointmentIntermediateViewInfoCollection viewInfos, int startIndex, int cellsMaxHeight) {
			AppointmentIndexesCalculator indexCalculator = new AppointmentIndexesCalculator();
			AppointmentCellIndexesCollection previousCellIndexes = indexCalculator.CreateAppointmentCellIndexesCollection(viewInfos);
			indexCalculator.AdjustAppointmentCellIndexes(token, viewInfos);
			CalculateAppointmentRelativePositionsCore(viewInfos, startIndex, cellsMaxHeight);
			indexCalculator.RestoreCellIndexes(token, viewInfos, previousCellIndexes);
		}
		protected internal virtual void CalculateAppointmentRelativePositionsCore(AppointmentIntermediateViewInfoCollection viewInfos, int startIndex, int cellsMaxHeight) {
			BusyPositionsCollection[] busyIntervals = PrepareBusyPositions(viewInfos, startIndex);
			int count = viewInfos.Count;
			for (int i = startIndex; i < count; i++) {
				AppointmentIntermediateViewInfo viewInfo = viewInfos[i];
				int relativePosition = FindAvailableRelativePosition(viewInfo, busyIntervals);
				if (relativePosition < cellsMaxHeight) {
					viewInfo.RelativePosition = relativePosition;
					MakePositionBusy(viewInfo, busyIntervals);
				} else
					viewInfo.RelativePosition = -1;
			}
		}
		protected internal virtual BusyPositionsCollection[] PrepareBusyPositions(AppointmentIntermediateViewInfoCollection viewInfos, int startIndex) {
			BusyPositionsCollection[] busyIntervals = CreateBusyPositions(2 * viewInfos.Count);
			for (int i = 0; i < startIndex; i++)
				MakePositionBusy(viewInfos[i], busyIntervals);
			return busyIntervals;
		}
		protected internal virtual BusyPositionsCollection[] CreateBusyPositions(int cellsCount) {
			BusyPositionsCollection[] result = new BusyPositionsCollection[cellsCount];
			for (int i = 0; i < cellsCount; i++)
				result[i] = new BusyPositionsCollection();
			return result;
		}
		protected internal virtual int FindAvailableRelativePosition(AppointmentIntermediateViewInfo viewInfo, BusyPositionsCollection[] cellsBusyIntervals) {
			int relativePosition = 0;
			int from = viewInfo.FirstCellIndex;
			int to = viewInfo.LastCellIndex;
			int i = from;
			while (i <= to) {
				BusyPositionsCollection busyIntervals = cellsBusyIntervals[i];
				BusyPosition interval = FindPossibleIntersectionInterval(busyIntervals, relativePosition);
				if ((interval == BusyPosition.Empty) || (interval.Start >= relativePosition + viewInfo.Height))
					i++;
				else {
					relativePosition = interval.End;
					i = from;
				}
			}
			return relativePosition;
		}
		public BusyPosition FindPossibleIntersectionInterval(BusyPositionsCollection busyIntervals, int value) {
			for (int i = 0; i < busyIntervals.Count; i++) {
				BusyPosition interval = busyIntervals[i];
				if ((interval.ContainsExcludeEndBound(value)) || (interval.Start > value))
					return new BusyPosition(interval.Start, interval.End);
			}
			return BusyPosition.Empty;
		}
		protected internal virtual void MakePositionBusy(AppointmentIntermediateViewInfo info, BusyPositionsCollection[] busyIntervals) {
			for (int i = info.FirstCellIndex; i <= info.LastCellIndex; i++)
				AddBusyPosition(busyIntervals[i], new BusyPosition(info.RelativePosition, info.RelativePosition + info.Height + GapBetweenAppointments));
		}
		protected internal virtual void AddBusyPosition(BusyPositionsCollection busyIntervals, BusyPosition busyInterval) {
			int count = busyIntervals.Count;
			int i = 0;
			while (i < count) {
				if (busyIntervals[i].Start > busyInterval.Start)
					break;
				i++;
			}
			busyIntervals.Insert(i, busyInterval);
		}
	}
	#endregion
	#region AppointmentViewInfoSkinElementInfoCache
	public class AppointmentViewInfoSkinElementInfoCache {
		SkinElementInfo content;
		SkinElementInfo leftBorder;
		SkinElementInfo rightBorder;
		SkinElementInfo progress;
		public SkinElementInfo Content { get { return content; } set { content = value; } }
		public SkinElementInfo LeftBorder { get { return leftBorder; } set { leftBorder = value; } }
		public SkinElementInfo RightBorder { get { return rightBorder; } set { rightBorder = value; } }
		public SkinElementInfo Progress { get { return progress; } set { progress = value; } }
	}
	#endregion
	[Flags]
	public enum AppointmentViewInfoVisibilitySet {
		Visible = 0x00,
		InvisibleTopResource = 0x01,
		InvisibleBottomResource = 0x02,
		InvisibleInterval = 0x04,
		InvisibleMoreButton = 0x08
	}
	#region AppointmentViewInfoVisibility
	public class AppointmentViewInfoVisibility {
		AppointmentViewInfoVisibilitySet visibility;
		public bool Visible {
			get { return visibility == AppointmentViewInfoVisibilitySet.Visible; }
		}
		public bool InvisibleTopResource {
			get { return GetValue(AppointmentViewInfoVisibilitySet.InvisibleTopResource); }
			set { SetValue(AppointmentViewInfoVisibilitySet.InvisibleTopResource, value); }
		}
		public bool InvisibleBottomResource {
			get { return GetValue(AppointmentViewInfoVisibilitySet.InvisibleBottomResource); }
			set { SetValue(AppointmentViewInfoVisibilitySet.InvisibleBottomResource, value); }
		}
		public bool InvisibleResource {
			get { return GetValue(AppointmentViewInfoVisibilitySet.InvisibleBottomResource) || GetValue(AppointmentViewInfoVisibilitySet.InvisibleTopResource); }
		}
		public bool InvisibleInterval {
			get { return GetValue(AppointmentViewInfoVisibilitySet.InvisibleInterval); }
			set { SetValue(AppointmentViewInfoVisibilitySet.InvisibleInterval, value); }
		}
		public bool InvisibleMoreButton {
			get { return GetValue(AppointmentViewInfoVisibilitySet.InvisibleMoreButton); }
			set { SetValue(AppointmentViewInfoVisibilitySet.InvisibleMoreButton, value); }
		}
		protected internal bool GetValue(AppointmentViewInfoVisibilitySet mask) {
			return (visibility & mask) != 0;
		}
		protected internal void SetValue(AppointmentViewInfoVisibilitySet mask, bool value) {
			if (value)
				visibility |= mask;
			else
				visibility &= ~mask;
		}
		public void MakeVisible() {
			visibility = AppointmentViewInfoVisibilitySet.Visible;
		}
	}
	#endregion
	#region AppointmentViewInfo
	public abstract class AppointmentViewInfo : ViewInfoItemContainer, IAppointmentViewInfo, IGanttAppointmentViewInfo {
		protected internal static readonly int SelectedBorderWidth = 6;
		#region Fields
		Appointment appointment;
		AppointmentViewInfoOptions options;
		AppearanceObject appearance;
		ViewInfoItemCollection statusItems;
		Rectangle innerBounds;
		bool disableDrop;
		bool showShadow;
		AppointmentViewInfoVisibility visibility;
		IAppointmentStatus status = AppointmentStatus.Empty;
		ViewInfoTextItem displayTextItem;
		ViewInfoTextItem descriptionItem;
		AppointmentViewInfoSkinElementInfoCache cachedSkinElementInfos;
		TimeZoneHelper timeZoneEngine;
		TimeInterval appointmentInterval;
		SchedulerViewCellContainer scrollContainer;
		#endregion
		protected AppointmentViewInfo(Appointment appointment, TimeZoneHelper timeZoneEngine) {
			if (appointment == null)
				Exceptions.ThrowArgumentNullException("appointment");
			if (timeZoneEngine == null)
				Exceptions.ThrowArgumentNullException("timeZoneEngine");
			this.appointment = appointment;
			this.appearance = new AppearanceObject();
			this.statusItems = new ViewInfoItemCollection();
			this.options = new AppointmentViewInfoOptions();
			this.timeZoneEngine = timeZoneEngine;
			this.appointmentInterval = GetAppointmentInterval();
			this.visibility = new AppointmentViewInfoVisibility();
		}
		#region Properties
		public Appointment Appointment { get { return appointment; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoAppointmentInterval")]
#endif
		public TimeInterval AppointmentInterval { get { return appointmentInterval; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoHitTestType")]
#endif
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.AppointmentContent; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoAppearance")]
#endif
		public AppearanceObject Appearance { get { return appearance; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoStatusItems")]
#endif
		public ViewInfoItemCollection StatusItems { get { return statusItems; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoInnerBounds")]
#endif
		public Rectangle InnerBounds { get { return innerBounds; } set { innerBounds = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoDisableDrop")]
#endif
		public bool DisableDrop { get { return disableDrop; } set { disableDrop = value; } }
		[Obsolete("You should use the 'Appearance.BackColor' instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public Color BackColor { get { return appearance.BackColor; } set { appearance.BackColor = value; } }
		public IAppointmentStatus Status { get { return status; } set { status = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoOptions")]
#endif
		public AppointmentViewInfoOptions Options { get { return options; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoShowBell")]
#endif
		public bool ShowBell { get { return Options.ShowBell; } set { Options.ShowBell = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoShowEndTime")]
#endif
		public bool ShowEndTime { get { return Options.ShowEndTime; } set { Options.ShowEndTime = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoShowRecurrence")]
#endif
		public bool ShowRecurrence { get { return Options.ShowRecurrence; } set { Options.ShowRecurrence = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoShowStartTime")]
#endif
		public bool ShowStartTime { get { return Options.ShowStartTime; } set { Options.ShowStartTime = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoShowTimeAsClock")]
#endif
		public bool ShowTimeAsClock { get { return Options.ShowTimeAsClock; } set { Options.ShowTimeAsClock = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoStatusDisplayType")]
#endif
		public AppointmentStatusDisplayType StatusDisplayType { get { return Options.StatusDisplayType; } set { Options.StatusDisplayType = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoDisplayText")]
#endif
		public string DisplayText {
			get {
				if (displayTextItem != null)
					return displayTextItem.Text;
				else
					return appointment.Subject;
			}
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentViewInfoDescription")]
#endif
		public string Description {
			get {
				if (descriptionItem != null)
					return descriptionItem.Text;
				else
					return appointment.Description;
			}
		}
		protected internal bool ShowShadow { get { return showShadow; } set { showShadow = value; } }
		protected internal abstract string ContentSkinElementName { get; }
		protected internal abstract string StartBorderSkinElementName { get; }
		protected internal abstract string EndBorderSkinElementName { get; }
		protected internal virtual bool SameDay { get { return AppointmentInterval.SameDay; } }
		protected internal virtual bool LongerThanADay { get { return AppointmentInterval.LongerThanADay; } }
		protected internal TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } }
		protected internal virtual bool CanScroll {
			get {
				XtraSchedulerDebug.Assert(ScrollContainer != null);
				return ScrollContainer.ScrollController.CanScroll;
			}
		}
		internal AppointmentViewInfoSkinElementInfoCache CachedSkinElementInfos { get { return cachedSkinElementInfos; } set { cachedSkinElementInfos = value; } }
		internal AppointmentViewInfoVisibility Visibility { get { return visibility; } }
		internal ViewInfoTextItem DisplayTextItem { get { return displayTextItem; } set { displayTextItem = value; } }
		internal ViewInfoTextItem DescriptionItem { get { return descriptionItem; } set { descriptionItem = value; } }
		protected internal SchedulerViewCellContainer ScrollContainer { get { return scrollContainer; } set { scrollContainer = value; } }
		protected internal virtual Rectangle PercentCompleteBounds { get { return new Rectangle(); } set { } }
		protected internal virtual Color PercentCompleteColor { get { return Appearance.BackColor; } set { } }
#if DEBUGTEST
		internal bool Painted { get; set; }
#endif
		#endregion
		public virtual Rectangle GetVisualBounds() {
			return Bounds;
		}
		protected internal virtual TimeInterval GetAppointmentInterval() {
			return TimeZoneHelper.ToClientTime(((IInternalAppointment)Appointment).CreateInterval(), true);
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (appearance != null) {
						appearance.Dispose();
						appearance = null;
					}
					if (displayTextItem != null) {
						DisposeItemCore(displayTextItem);
						displayTextItem = null;
					}
					if (descriptionItem != null) {
						DisposeItemCore(descriptionItem);
						descriptionItem = null;
					}
					if (statusItems != null) {
						DisposeStatusItems();
						statusItems = null;
					}
					cachedSkinElementInfos = null;
					appointment = null;
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal virtual void DisposeStatusItems() {
			DisposeItemsCore(StatusItems);
		}
		public virtual bool IsLongTime() {
			return LongerThanADay || !SameDay;
		}
		protected internal virtual void CalcInnerBounds(AppointmentPainter painter) {
			int left = LeftBorderBounds.Right + painter.GetLeftContentPadding(this);
			int top = TopBorderBounds.Bottom + painter.GetTopContentPadding(this);
			int right = RightBorderBounds.Left - painter.GetRightContentPadding(this);
			int bottom = BottomBorderBounds.Top - painter.GetBottomContentPadding(this);
			this.innerBounds = Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal virtual int CalcAppointmenStatusMaskImageIndex() {
			if (HasLeftBorder) {
				if (HasRightBorder)
					return 0;
				else
					return 1;
			} else {
				if (HasRightBorder)
					return 2;
				else
					return 3;
			}
		}
		public override SchedulerHitInfo CalculateHitInfo(Point pt, SchedulerHitInfo nextHitInfo) {
			SchedulerHitInfo hitInfo = base.CalculateHitInfo(pt, nextHitInfo);
			if (hitInfo == nextHitInfo)
				return hitInfo;
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				AppointmentViewInfoStatusItem item = Items[i] as AppointmentViewInfoStatusItem;
				if (item != null && item.Bounds.Contains(pt))
					return new SchedulerHitInfo(this, SchedulerHitTest.AppointmentMoveEdge, nextHitInfo);
			}
			return hitInfo;
		}
		protected internal virtual void PrepareSkinElementInfoCache(AppointmentPainter painter, ColoredSkinElementCache coloredSkinElementCache) {
			this.CachedSkinElementInfos = painter.PrepareSkinElementInfoCache(this, coloredSkinElementCache);
		}
		#region IGanttAppointmentViewInfo implementation
		AppointmentViewInfoVisibility IGanttAppointmentViewInfo.Visibility { get { return Visibility; } }
		#endregion
	}
	#endregion
	#region HorizontalAppointmentViewInfo
	public class HorizontalAppointmentViewInfo : AppointmentViewInfo {
		public HorizontalAppointmentViewInfo(Appointment appointment, TimeZoneHelper timeZoneEngine)
			: base(appointment, timeZoneEngine) {
		}
		protected internal override string ContentSkinElementName {
			get {
				if (SameDay)
					return SchedulerSkins.SkinAppointmentSameDayContent;
				else
					return SchedulerSkins.SkinAppointmentContent;
			}
		}
		protected internal override string StartBorderSkinElementName {
			get {
				if (SameDay) {
					return SchedulerSkins.SkinAppointmentSameDayLeftBorder;
				} else {
					if (HasLeftBorder)
						return SchedulerSkins.SkinAppointmentLeftBorder;
					else
						return SchedulerSkins.SkinAppointmentNoLeftBorder;
				}
			}
		}
		protected internal override string EndBorderSkinElementName {
			get {
				if (SameDay) {
					return SchedulerSkins.SkinAppointmentSameDayRightBorder;
				} else {
					if (HasRightBorder)
						return SchedulerSkins.SkinAppointmentRightBorder;
					else
						return SchedulerSkins.SkinAppointmentNoRightBorder;
				}
			}
		}
		protected internal override int CalcAppointmenStatusMaskImageIndex() {
			if (SameDay)
				return 0;
			else
				return base.CalcAppointmenStatusMaskImageIndex();
		}
		public override SchedulerHitInfo CalculateHitInfo(Point pt, SchedulerHitInfo nextHitInfo) {
			if (IsLeftBorderHit(pt))
				return new SchedulerHitInfo(this, SchedulerHitTest.AppointmentResizingLeftEdge, nextHitInfo);
			if (IsRightBorderHit(pt))
				return new SchedulerHitInfo(this, SchedulerHitTest.AppointmentResizingRightEdge, nextHitInfo);
			return base.CalculateHitInfo(pt, nextHitInfo);
		}
		bool IsLeftBorderHit(Point pt) {
			return HasLeftBorder && Rectangle.Inflate(LeftBorderBounds, SelectedBorderWidth / 2, 0).Contains(pt);
		}
		bool IsRightBorderHit(Point pt) {
			return HasRightBorder && Rectangle.Inflate(RightBorderBounds, SelectedBorderWidth / 2, 0).Contains(pt);
		}
	}
	#endregion
	#region VerticalAppointmentViewInfo
	public class VerticalAppointmentViewInfo : AppointmentViewInfo {
		public VerticalAppointmentViewInfo(Appointment appointment, TimeZoneHelper timeZoneEngine)
			: base(appointment, timeZoneEngine) {
		}
		#region Properties
		protected internal override string ContentSkinElementName { get { return SchedulerSkins.SkinAppointment; } }
		protected internal override string StartBorderSkinElementName { get { return SchedulerSkins.SkinAppointmentLeftBorder; } }
		protected internal override string EndBorderSkinElementName { get { return SchedulerSkins.SkinAppointmentRightBorder; } }
		#endregion
		public override Rectangle GetVisualBounds() {
			int offset = 0;
			if (ScrollContainer != null)
				offset = ScrollContainer.CalculateScrollOffset();
			return Rectangle.FromLTRB(Bounds.Left, Bounds.Top - offset, Bounds.Right, Bounds.Bottom - offset);
		}
		public override SchedulerHitInfo CalculateHitInfo(Point pt, SchedulerHitInfo nextHitInfo) {
			if (IsBottomBorderHit(pt))
				return new SchedulerHitInfo(this, SchedulerHitTest.AppointmentResizingBottomEdge, nextHitInfo);
			if (IsTopBorderHit(pt))
				return new SchedulerHitInfo(this, SchedulerHitTest.AppointmentResizingTopEdge, nextHitInfo);
			return base.CalculateHitInfo(pt, nextHitInfo);
		}
		bool IsTopBorderHit(Point pt) {
			return HasTopBorder && Rectangle.Inflate(TopBorderBounds, 0, SelectedBorderWidth / 2).Contains(pt);
		}
		bool IsBottomBorderHit(Point pt) {
			return HasBottomBorder && Rectangle.Inflate(BottomBorderBounds, 0, SelectedBorderWidth / 2).Contains(pt);
		}
	}
	#endregion
	#region TimeLineAppointmentViewInfo
	public class TimeLineAppointmentViewInfo : HorizontalAppointmentViewInfo {
		Rectangle progressBounds;
		Color progressColor;
		public TimeLineAppointmentViewInfo(Appointment appointment, TimeZoneHelper timeZoneEngine)
			: base(appointment, timeZoneEngine) {
		}
		protected internal override bool SameDay { get { return false; } }
		protected internal override bool LongerThanADay { get { return true; } }
		protected internal override Rectangle PercentCompleteBounds { get { return progressBounds; } set { progressBounds = value; } }
		protected internal override Color PercentCompleteColor { get { return progressColor; } set { progressColor = value; } }
	}
	#endregion
	#region AppointmentViewInfoCollection
	public class AppointmentViewInfoCollection : List<AppointmentViewInfo>, IAppointmentViewInfoCollection {
		public AppointmentViewInfoCollection() {
		}
		public AppointmentViewInfoCollection(IEnumerable<AppointmentViewInfo> collection)
			: base(collection) {
		}
		public AppointmentViewInfoCollection(int capacity)
			: base(capacity) {
		}
		public void AddRange(IAppointmentViewInfoCollection value) {
			base.AddRange((AppointmentViewInfoCollection)value);
		}
		public bool Contains(Appointment apt) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (Object.ReferenceEquals(this[i].Appointment, apt))
					return true;
			}
			return false;
		}
		public bool ContainsId(object appointmentId) {
			if (appointmentId == null)
				return false;
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (Object.Equals(appointmentId, this[i].Appointment.Id))
					return true;
			}
			return false;
		}
		public AppointmentViewInfoCollection GetAppointmentViewInfosById(object appointmentId) {
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (Object.Equals(appointmentId, this[i].Appointment.Id))
					result.Add(this[i]);
			}
			return result;
		}
	}
	#endregion
	#region AppointmentViewInfosContainer
	public class AppointmentViewInfosContainer {
		AppointmentViewInfoCollection viewInfos;
		SchedulerViewCellContainer container;
		public AppointmentViewInfosContainer(AppointmentViewInfoCollection viewInfos, SchedulerViewCellContainer container) {
			if (viewInfos == null)
				Exceptions.ThrowArgumentNullException("viewInfos");
			if (container == null)
				Exceptions.ThrowArgumentNullException("container");
			this.viewInfos = viewInfos;
			this.container = container;
		}
		public AppointmentViewInfoCollection ViewInfos { get { return viewInfos; } }
		public SchedulerViewCellContainer Container { get { return container; } }
	}
	#endregion
	#region AppointmentViewInfosContainerCollection
	public class AppointmentViewInfosContainerCollection : DXCollectionBase<AppointmentViewInfosContainer> {
	}
	#endregion
	#region AppointmentsLayoutResult
	public class AppointmentsLayoutResult : IAppointmentsLayoutResult {
		MoreButtonCollection moreButtons = new MoreButtonCollection();
		AppointmentViewInfoCollection appointmentViewInfos = new AppointmentViewInfoCollection();
		public AppointmentViewInfoCollection AppointmentViewInfos {
			get { return this.appointmentViewInfos; }
			set { this.appointmentViewInfos = value; }
		}
		IAppointmentViewInfoCollection IAppointmentsLayoutResult.AppointmentViewInfos { get { return AppointmentViewInfos; } }
		public MoreButtonCollection MoreButtons {
			get { return moreButtons; }
			set { this.moreButtons = value; }
		}
		public virtual void Merge(IAppointmentsLayoutResult baseLayoutResult) {
			AppointmentsLayoutResult layoutResult = baseLayoutResult as AppointmentsLayoutResult;
			if (layoutResult != null) {
				MoreButtons.AddRange(layoutResult.MoreButtons);
				AppointmentViewInfos.AddRange(layoutResult.AppointmentViewInfos);
			}
		}
		public virtual void Clear() {
			MoreButtons.Clear();
			AppointmentViewInfos.Clear();
		}
	}
	#endregion
	#region DayViewAppointmentsLayoutResult
	public class DayViewAppointmentsLayoutResult : AppointmentsLayoutResult {
		AppointmentStatusViewInfoCollection appointmentStatusViewInfos = new AppointmentStatusViewInfoCollection();
		public AppointmentStatusViewInfoCollection AppointmentStatusViewInfos { get { return appointmentStatusViewInfos; } }
		public override void Merge(IAppointmentsLayoutResult baseLayoutResult) {
			base.Merge(baseLayoutResult);
			DayViewAppointmentsLayoutResult layoutResult = baseLayoutResult as DayViewAppointmentsLayoutResult;
			if (layoutResult != null)
				this.AppointmentStatusViewInfos.AddRange(layoutResult.AppointmentStatusViewInfos);
		}
		public override void Clear() {
			base.Clear();
			AppointmentStatusViewInfos.Clear();
		}
	}
	#endregion
	#region AppointmentBaseLayoutCalculator
	public abstract class AppointmentBaseLayoutCalculator : AppointmentLayoutCalculatorCore<AppointmentIntermediateViewInfoCollection, IVisuallyContinuousCellsInfo, AppointmentsLayoutResult, AppointmentViewInfoCollection> {
		#region Fields
		AppointmentPainter painter;
		AppointmentContentLayoutCalculator contentCalculator;
		object appointmentImages;
		Size moreButtonSize;
		GraphicsCache cache;
		int intermediateViewInfosToCalculateCount;
		int calculatedViewInfosCount;
		#endregion
		protected AppointmentBaseLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo) {
			if (contentCalculator == null)
				Exceptions.ThrowArgumentException("contentCalculator", contentCalculator);
			if (cache == null)
				Exceptions.ThrowArgumentException("cache", cache);
			if (painter == null)
				Exceptions.ThrowArgumentNullException("painter");
			this.contentCalculator = contentCalculator;
			this.painter = painter;
			object appointmentImages = viewInfo.AppointmentImages;
			if (appointmentImages == null)
				this.appointmentImages = this.painter.DefaultAppointmentImages;
			else
				this.appointmentImages = appointmentImages;
			this.moreButtonSize = viewInfo.CalculateMoreButtonMinSize();
			this.cache = cache;
		}
		#region Properties
		protected internal new ISupportAppointments ViewInfo { get { return (ISupportAppointments)base.ViewInfo; } }
		protected internal GraphicsCache Cache { get { return cache; } }
		protected internal AppearanceObject DefaultAppointmentAppearance { get { return ViewInfo.AppointmentAppearance; } }
		protected internal AppointmentPainter Painter { get { return painter; } }
		protected internal object AppointmentImages { get { return appointmentImages; } }
		protected internal AppointmentContentLayoutCalculator ContentCalculator { get { return contentCalculator; } }
		protected internal Size MoreButtonSize { get { return moreButtonSize; } set { moreButtonSize = value; } }
		#endregion
		#region Abstract methods
		protected internal abstract FitIntoCellResult FitIntoLastCell(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo);
		protected internal abstract FitIntoCellResult FitIntoFirstCell(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo);
		protected internal abstract void CalculateIntermediateViewInfoBounds(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo);
		protected internal abstract AppointmentViewInfoCollection CreateViewInfoCollection(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo, FitIntoCellResult fitResult);
		protected internal abstract void FinalContentLayoutCore(AppointmentViewInfoCollection viewInfos, GraphicsCache cache);
		protected internal abstract int RecalcEndOffset(Rectangle viewInfoBounds, Rectangle lastCellBounds);
		protected internal abstract FitIntoCellResult FitIntoCells(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo);
		internal abstract AppointmentIntermediateViewInfoCollection FilterInvisibleAppointments(AppointmentIntermediateViewInfoCollection viewInfos, IVisuallyContinuousCellsInfo cellsInfo);
		#endregion
		protected virtual void CalculateLayoutCore(AppointmentIntermediateViewInfoCollection intermediateViewInfos, VisuallyContinuousCellsInfoCollection cellInfos, AppointmentsLayoutResult layoutResult) {
			IVisuallyContinuousCellsInfo cellInfo = (IVisuallyContinuousCellsInfo)cellInfos.FirstOrDefault(ci => ci.Resource == intermediateViewInfos.Resource && ci.Interval.Equals(intermediateViewInfos.Interval));
			if (cellInfo == null || cellInfo.VisibleCellsCount <= 0 || ViewInfo.CancellationToken.IsCancellationRequested) {
				Interlocked.Increment(ref calculatedViewInfosCount);
				return;
			}
			IAppointmentsLayoutResult result = CalculateFinalLayoutForCellInfo(intermediateViewInfos, cellInfo);
			AppointmentsLayoutResult finalLayoutResult = result as AppointmentsLayoutResult;
			if (finalLayoutResult == null) {
				Interlocked.Increment(ref calculatedViewInfosCount);
				return;
			}
			lock (layoutResult.MoreButtons)
				layoutResult.MoreButtons.AddRange(finalLayoutResult.MoreButtons);
			SchedulerViewInfoBase schedulerViewInfo = ViewInfo as SchedulerViewInfoBase;
			if (schedulerViewInfo != null)
				schedulerViewInfo.ApplySelection(cellInfo.ScrollContainer);
			Interlocked.Increment(ref calculatedViewInfosCount);
		}
		public override void CalculateLayout(AppointmentsLayoutResult result, AppointmentBaseCollection appointments, ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			SchedulerViewInfoBase viewInfoBase = base.ViewInfo as SchedulerViewInfoBase;
			if (viewInfoBase == null) {
				PreparedAppointmentsCollection preparedAppointments = PrepareAppointments(appointments, cellsInfos);
				CalculateLayoutCore(result, preparedAppointments, false);
				return;
			}
			viewInfoBase.View.Control.SubscribeAppointmentContentLayoutCalculatorEvents(ContentCalculator);
			VisuallyContinuousCellsInfoCollection cellsInfoCollection = new VisuallyContinuousCellsInfoCollection();
			cellsInfoCollection.AddRange(cellsInfos.SelectMany(ci => ci.CellsInfoCollection));
			intermediateViewInfosToCalculateCount = viewInfoBase.PreliminaryLayoutResult.PreliminaryAppointmentResult.Count;
			calculatedViewInfosCount = 0;
			IViewAsyncAccessor viewAccessor = ViewInfo as IViewAsyncAccessor;
			if (viewAccessor == null)
				return;
			foreach (AppointmentIntermediateViewInfoCollection intermediateViewInfoCollection in viewInfoBase.PreliminaryLayoutResult.PreliminaryAppointmentResult) {
				viewAccessor.View.ThreadManager.Run((intermediateViewInfos, cellInfos, mainResult) => {
					try {
						ThreadUtils.WaitFor(intermediateViewInfos.Signal.WaitHandle);
						CalculateLayoutCore(intermediateViewInfos, cellInfos, mainResult);
						if (calculatedViewInfosCount == intermediateViewInfosToCalculateCount && !ViewInfo.CancellationToken.IsCancellationRequested)
							AfterFinalLayoutCalculation(cellInfos, mainResult);
					} catch (Exception e) {
						throw e;
					}
				}, intermediateViewInfoCollection, cellsInfoCollection, result);
			}
			if (!ViewInfo.UseAsyncMode)
				viewAccessor.View.ThreadManager.WaitForAllThreads();
		}
		public virtual void RecalcAppointmentsVisible(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
		}
		protected virtual void CalculateViewInfosSizeAndPosition(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfoCollection intermediateResult) {
			LayoutViewInfos(cellsInfo, intermediateResult, true);
		}
		protected virtual void AfterFinalLayoutCalculation(VisuallyContinuousCellsInfoCollection cellsInfos, AppointmentsLayoutResult layoutResult) {
			SchedulerViewInfoBase schedulerViewInfo = ViewInfo as SchedulerViewInfoBase;
			if (schedulerViewInfo == null)
				return;
			schedulerViewInfo.View.Control.UnsubscribeAppointmentContentLayoutCalculatorEvents(ContentCalculator);
			if (!schedulerViewInfo.View.Control.IsDesignMode)
				schedulerViewInfo.View.Invalidate();
		}
		protected internal void ClearMoreButtons(IVisuallyContinuousCellsInfo cellsInfo) {
			int count = cellsInfo.MoreButtons.Length;
			for (int i = 0; i < count; i++)
				cellsInfo.MoreButtons[i] = MoreButton.Empty;
		}
		protected override IEnumerable<PreparedAppointments> GetPreparedAppointmentsToCalculation(PreparedAppointmentsCollection preparedAppointments) {
			SchedulerViewInfoBase viewInfoBase = base.ViewInfo as SchedulerViewInfoBase;
			if (viewInfoBase == null)
				return base.GetPreparedAppointmentsToCalculation(preparedAppointments);
			IEnumerable<Resource> resources = viewInfoBase.PreliminaryLayoutResult.PreliminaryAppointmentResult.Select(par => par.Resource);
			IEnumerable<TimeInterval> intervals = viewInfoBase.PreliminaryLayoutResult.PreliminaryAppointmentResult.Select(par => par.Interval);
			return preparedAppointments.Where(pa => (!resources.Contains(pa.CellsInfo.Resource) && pa.CellsInfo.Resource != ResourceBase.Empty) || !intervals.Contains(pa.CellsInfo.Interval));
		}
		protected internal override TimeInterval GetAppointmentInterval(Appointment appointment) {
			TimeZoneHelper helper = ViewInfo.TimeZoneHelper;
			return helper.ToClientTime(((IInternalAppointment)appointment).CreateInterval(), true);
		}
		protected internal virtual void CacheAppointmentViewInfosSkinElementInfos(AppointmentViewInfoCollection viewInfos) {
			if (!Painter.ShouldCacheSkinElementInfos)
				return;
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (ViewInfo.CancellationToken.IsCancellationRequested)
					return;
				viewInfos[i].PrepareSkinElementInfoCache(Painter, ViewInfo.ColoredSkinElementCache);
			}
		}
		protected internal virtual void AddMoreButtons(MoreButtonCollection target, MoreButton[] moreButtons) {
			int count = moreButtons.Length;
			for (int i = 0; i < count; i++) {
				MoreButton moreButton = moreButtons[i];
				if (moreButton != MoreButton.Empty)
					target.Add(moreButton);
			}
		}
		protected internal override void FinalContentLayout(AppointmentViewInfoCollection viewInfos) {
			FinalContentLayoutCore(viewInfos, GraphicsCachePool.GetCache());
			CacheAppointmentViewInfosSkinElementInfos(viewInfos);
		}
		protected internal virtual AppointmentViewInfoCollection SnapToCellsCore(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			CalculateIntermediateViewInfoBounds(intermediateResult, cellsInfo);
			InitializeViewInfoScrollContainer(intermediateResult, cellsInfo);
			FitIntoCellResult fitResult = FitIntoCells(intermediateResult, cellsInfo);
			AppointmentViewInfoCollection viewInfos = CreateViewInfoCollection(intermediateResult, cellsInfo, fitResult);
			return viewInfos;
		}
		protected internal virtual void InitializeViewInfoScrollContainer(AppointmentIntermediateViewInfo intermediateViewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			intermediateViewInfo.ViewInfo.ScrollContainer = cellsInfo.ScrollContainer;
		}
		protected internal override void CalculateIntermediateViewInfos(IAppointmentIntermediateViewInfoCoreCollection intermediateResult, AppointmentBaseCollection appointments, IVisuallyContinuousCellsInfo cellsInfo) {
			AppointmentIntermediateLayoutCalculatorCore intermediateCalculator = CreateIntermediateLayoutCalculator();
			intermediateCalculator.CalculateIntermediateAppointmentViewInfos(ViewInfo.CancellationToken.Token, intermediateResult, appointments, cellsInfo);
			intermediateResult.Sort(CreateAppointmentComparer());
		}
		protected internal virtual FitIntoCellResult FitIntoNonScrollingCells(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			FitIntoCellResult fitIntoFirstCellResult = FitIntoFirstCell(intermediateResult, cellsInfo);
			if (fitIntoFirstCellResult == FitIntoCellResult.NotFitted)
				return fitIntoFirstCellResult;
			else
				return FitIntoLastCell(intermediateResult, cellsInfo);
		}
		protected internal override AppointmentsLayoutResult SnapToCells(AppointmentIntermediateViewInfoCollection intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			AppointmentsLayoutResult result = new AppointmentsLayoutResult();
			ClearMoreButtons(cellsInfo);
			AppointmentIntermediateViewInfoCollection intermediateViewInfos = FilterInvisibleAppointments(intermediateResult, cellsInfo);
			int i = 0;
			while (i < intermediateViewInfos.Count) {
				if (ViewInfo.CancellationToken.IsCancellationRequested)
					return result;
				AppointmentViewInfoCollection snap = SnapToCellsCore(intermediateViewInfos[i], cellsInfo);
				if (ShouldRecalcSnapToCells(snap)) {
					snap = RecalcSnapToCells(intermediateViewInfos, cellsInfo, i);
					intermediateViewInfos = FilterInvisibleAppointments(intermediateViewInfos, cellsInfo);
				}
				if (!ViewInfo.DrawMoreButtonsOverAppointments)
					HideAppointmentsOverMoreButtons(snap, cellsInfo);
				result.AppointmentViewInfos.AddRange(snap);
				i++;
			}
			AddMoreButtons(result.MoreButtons, cellsInfo.MoreButtons);
			return result;
		}
		protected internal virtual bool ShouldRecalcSnapToCells(AppointmentViewInfoCollection snapResult) {
			return false;
		}
		protected internal virtual AppointmentViewInfoCollection RecalcSnapToCells(AppointmentIntermediateViewInfoCollection intermediateViewInfos, IVisuallyContinuousCellsInfo cellsInfo, int i) {
			return new AppointmentViewInfoCollection();
		}
		protected internal virtual void HideAppointmentsOverMoreButtons(AppointmentViewInfoCollection viewInfos, IVisuallyContinuousCellsInfo cellsInfo) {
			int count = cellsInfo.MoreButtons.Length;
			for (int i = 0; i < count; i++) {
				MoreButton button = cellsInfo.MoreButtons[i];
				if ((button != MoreButton.Empty))
					HideAppointmentsOverMoreButtonsCore(button, viewInfos);
			}
		}
		internal virtual void HideAppointmentsOverMoreButtonsCore(MoreButton button, AppointmentViewInfoCollection viewInfos) {
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				AppointmentViewInfo viewInfo = viewInfos[i];
				if (viewInfo.Bounds.IntersectsWith(button.Bounds))
					HideUnfittedAppointment(viewInfo, button.Bounds);
			}
		}
		internal virtual void HideUnfittedAppointment(AppointmentViewInfo viewInfo, Rectangle moreButtonBounds) {
			viewInfo.Visibility.InvisibleMoreButton = true;
		}
		protected internal virtual bool ShouldUpdateMoreButton(MoreButton moreButton, DateTime viewStart ) {
			if (moreButton == MoreButton.Empty)
				return true;
			if (moreButton.GoUp)
				return ShouldUpdateUpMoreButton(moreButton, viewStart);
			else
				return ShouldUpdateDownMoreButton(moreButton, viewStart);
		}
		protected internal virtual bool ShouldUpdateDownMoreButton(MoreButton moreButton, DateTime viewStart ) {
			return viewStart.Ticks < moreButton.TargetViewStart.Ticks;
		}
		protected internal virtual bool ShouldUpdateUpMoreButton(MoreButton moreButton, DateTime viewStart ) {
			return viewStart.Ticks > moreButton.TargetViewStart.Ticks;
		}
		protected internal virtual void RecalcExtendedViewInfoProperties(AppointmentIntermediateViewInfo intermediateViewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			intermediateViewInfo.LastCellIndex = RecalcLastCellIndex(intermediateViewInfo, cellsInfo);
			Rectangle lastCellBounds = cellsInfo.GetContentBounds(intermediateViewInfo.LastCellIndex);
			TimeInterval lastCellInterval = cellsInfo.GetIntervalByIndex(intermediateViewInfo.LastCellIndex);
			intermediateViewInfo.EndRelativeOffset = RecalcEndOffset(intermediateViewInfo.Bounds, lastCellBounds);
			RecalcViewInfoInterval(intermediateViewInfo, lastCellInterval);
		}
		protected virtual IAppointmentsLayoutResult CalculateFinalLayoutForCellInfo(AppointmentIntermediateViewInfoCollection intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			AppointmentIntermediateLayoutCalculatorCore intermediateCalculator = CreateIntermediateLayoutCalculator();
			AppointmentIntermediateViewInfoCollection filteredResult = (AppointmentIntermediateViewInfoCollection)FilterAppointmentIntermediateViewInfoResult(intermediateResult, cellsInfo, intermediateCalculator);
			PreliminaryContentLayout(filteredResult, cellsInfo);
			CalculateViewInfosSizeAndPosition(cellsInfo, filteredResult);
			AppointmentsLayoutResult result = SnapToCells(filteredResult, cellsInfo);
			FinalContentLayout(result.AppointmentViewInfos);
			if (ViewInfo.CancellationToken.IsCancellationRequested)
				return result;
			lock (cellsInfo.ScrollContainer.AppointmentViewInfos)
				cellsInfo.ScrollContainer.AppointmentViewInfos.AddRange(result.AppointmentViewInfos);
			return result;
		}
		protected override void AfterPreliminaryLayoutCalculation() {
			base.AfterPreliminaryLayoutCalculation();
			SchedulerViewInfoBase viewInfoBase = base.ViewInfo as SchedulerViewInfoBase;
			if (viewInfoBase != null)
				viewInfoBase.PreliminaryLayoutResult.Calculated = true;
		}
		internal virtual int RecalcLastCellIndex(AppointmentIntermediateViewInfo viewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			int count = cellsInfo.Count;
			for (int i = viewInfo.FirstCellIndex + 1; i < count; i++) {
				Rectangle cellBounds = cellsInfo.GetContentBounds(i);
				if (!viewInfo.Bounds.IntersectsWith(cellBounds))
					return i - 1;
			}
			return count - 1;
		}
		internal virtual void RecalcViewInfoInterval(AppointmentIntermediateViewInfo intermediateViewInfo, TimeInterval lastCellInterval) {
			DateTime start = intermediateViewInfo.ViewInfo.Interval.Start;
			DateTime adjustedEnd = AppointmentTimeScaleHelper.CalculateEndTimeByOffset(lastCellInterval, intermediateViewInfo.EndRelativeOffset);
			DateTime actualEnd = DateTimeHelper.Max(start, adjustedEnd);
			intermediateViewInfo.ViewInfo.Interval = new TimeInterval(start, actualEnd);
		}
	}
	#endregion
	#region VerticalAppointmentLayoutCalculatorHelper
	public class VerticalAppointmentLayoutCalculatorHelper : VerticalAppointmentLayoutCalculatorHelperCore<IVisuallyContinuousCellsInfo> {
		AppointmentPainter painter;
		SchedulerColumnPadding columnPadding;
		public VerticalAppointmentLayoutCalculatorHelper(AppointmentPainter painter, SchedulerColumnPadding columnPadding) {
			if (painter == null)
				Exceptions.ThrowArgumentNullException("painter");
			if (columnPadding == null)
				Exceptions.ThrowArgumentNullException("columnPadding");
			this.painter = painter;
			this.columnPadding = columnPadding;
		}
		public AppointmentPainter Painter { get { return painter; } }
		public SchedulerColumnPadding ColumnPadding { get { return columnPadding; } }
		protected internal override void LayoutGroupedViewInfosHorizontally(CancellationToken token, IAppointmentIntermediateLayoutViewInfoCoreCollection groupedViewInfos, IVisuallyContinuousCellsInfo cellsInfo) {
			base.LayoutGroupedViewInfosHorizontally(token, groupedViewInfos, cellsInfo);
			CalculateViewInfosWidthAndPosition(token, groupedViewInfos, cellsInfo);
		}
		protected internal virtual void CalculateViewInfosWidthAndPosition(CancellationToken token, IAppointmentIntermediateLayoutViewInfoCoreCollection groupedViewInfos, IVisuallyContinuousCellsInfo cellsInfo) {
			int count = groupedViewInfos.Count;
			if (count == 0)
				return;
			int leftPadding = Painter.LeftPadding + ColumnPadding.Left;
			int rightPadding = Painter.RightPadding + ColumnPadding.Right;
			int availableWidth = Math.Max(0, cellsInfo.GetContentBounds(0).Width - leftPadding - rightPadding);
			int maxIndexInGroup = groupedViewInfos[0].MaxIndexInGroup;
			Rectangle[] columnsBounds = RectUtils.SplitHorizontally(new Rectangle(leftPadding, 0, availableWidth, 0), maxIndexInGroup + 1);
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested)
					return;
				AppointmentIntermediateViewInfo viewInfo = (AppointmentIntermediateViewInfo)groupedViewInfos[i];
				int viewInfoWidth = columnsBounds[viewInfo.LastIndexPosition].Right - columnsBounds[viewInfo.FirstIndexPosition].Left - Painter.RightShadowSize;
				if (viewInfo.LastIndexPosition != maxIndexInGroup)
					viewInfoWidth -= Painter.HorizontalInterspacing;
				viewInfo.Width = Math.Max(1, viewInfoWidth);
				viewInfo.RelativePosition = columnsBounds[viewInfo.FirstIndexPosition].X;
			}
		}
	}
	#endregion
	#region VerticalAppointmentLayoutCalculatorBase
	public abstract class VerticalAppointmentLayoutCalculatorBase : AppointmentBaseLayoutCalculator {
		protected VerticalAppointmentLayoutCalculatorBase(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal abstract bool ShowShadow { get; }
		internal override AppointmentIntermediateViewInfoCollection FilterInvisibleAppointments(AppointmentIntermediateViewInfoCollection viewInfos, IVisuallyContinuousCellsInfo cellsInfo) {
			return viewInfos;
		}
		protected internal override AppointmentViewInfoCollection CreateViewInfoCollection(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo, FitIntoCellResult fitResult) {
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			AppointmentViewInfo viewInfo = intermediateResult.ViewInfo;
			if (fitResult == FitIntoCellResult.NotFitted)
				viewInfo.Visibility.InvisibleMoreButton = true;
			else if (!viewInfo.Visibility.Visible)
				viewInfo.Visibility.MakeVisible();
			CalculateViewInfoProperties(intermediateResult, cellsInfo);
			result.Add(viewInfo);
			return result;
		}
		protected internal override void CalculateViewInfoProperties(AppointmentIntermediateViewInfoCore intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			base.CalculateViewInfoProperties(intermediateResult, cellsInfo);
			((AppointmentIntermediateViewInfo)intermediateResult).ViewInfo.ShowShadow = this.ShowShadow;
		}
		protected internal override void FinalContentLayoutCore(AppointmentViewInfoCollection viewInfos, GraphicsCache cache) {
			ContentCalculator.CalculateContentLayout(cache, viewInfos);
		}
		protected internal virtual AppointmentViewInfoCollection GetVisibleViewInfos(AppointmentViewInfoCollection viewInfos) {
			int count = viewInfos.Count;
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			for (int i = 0; i < count; i++) {
				AppointmentViewInfo viewInfo = viewInfos[i];
				if (viewInfo.Visibility.Visible)
					result.Add(viewInfo);
			}
			return result;
		}
		protected internal override void PreliminaryContentLayout(AppointmentIntermediateViewInfoCollection intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			int count = intermediateResult.Count;
			for (int i = 0; i < count; i++) {
				if (ViewInfo.CancellationToken.IsCancellationRequested)
					return;
				CalculateViewInfoVerticalPosition(intermediateResult[i], cellsInfo);
			}
		}
		protected internal virtual void CalculateViewInfoVerticalPosition(AppointmentIntermediateViewInfo intermediateViewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle firstCellBounds = cellsInfo.GetContentBounds(intermediateViewInfo.FirstCellIndex);
			Rectangle lastCellBounds = cellsInfo.GetContentBounds(intermediateViewInfo.LastCellIndex);
			int top = firstCellBounds.Top + firstCellBounds.Height * intermediateViewInfo.StartRelativeOffset / 100;
			int bottom = lastCellBounds.Bottom - lastCellBounds.Height * intermediateViewInfo.EndRelativeOffset / 100;
			Rectangle bounds = Rectangle.FromLTRB(0, top, 0, bottom);
			intermediateViewInfo.Bounds = bounds;
			if (ShouldExtendViewInfoBounds(intermediateViewInfo)) {
				ExtendIntermediateViewInfoBounds(intermediateViewInfo, cellsInfo);
				RecalcExtendedViewInfoProperties(intermediateViewInfo, cellsInfo);
			}
		}
		internal virtual bool ShouldExtendViewInfoBounds(AppointmentIntermediateViewInfo intermediateViewInfo) {
			if (AppointmentsSnapToCells == AppointmentSnapToCellsMode.Never)
				return intermediateViewInfo.Bounds.Height < GetMinViewInfoHeight(intermediateViewInfo.ViewInfo);
			return false;
		}
		protected internal virtual void ExtendIntermediateViewInfoBounds(AppointmentIntermediateViewInfo intermediateViewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle totalBounds = cellsInfo.GetTotalBounds();
			int minHeight = GetMinViewInfoHeight(intermediateViewInfo.ViewInfo);
			int actualHeight = Math.Min(totalBounds.Bottom - intermediateViewInfo.Bounds.Top, minHeight);
			intermediateViewInfo.Bounds = RectUtils.GetTopSideRect(intermediateViewInfo.Bounds, actualHeight);
		}
		protected internal override void LayoutViewInfos(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfoCollection intermediateResult, bool isTwoPassLayout) {
			DayViewAppointmentDisplayOptions dayViewAppointmentDisplayOptions = (DayViewAppointmentDisplayOptions)ViewInfo.AppointmentDisplayOptions;
			VerticalAppointmentLayoutCalculatorHelper helper = new VerticalAppointmentLayoutCalculatorHelper(Painter, dayViewAppointmentDisplayOptions.ColumnPadding);
			intermediateResult.GroupedViewInfos.AddRange(helper.LayoutViewInfos(ViewInfo.CancellationToken.Token, cellsInfo, intermediateResult));
		}
		protected internal override void CalculateIntermediateViewInfoBounds(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle firstCellBounds = cellsInfo.GetContentBounds(intermediateResult.FirstCellIndex);
			int left = firstCellBounds.Left + intermediateResult.RelativePosition;
			intermediateResult.Bounds = new Rectangle(left, intermediateResult.Bounds.Top, intermediateResult.Width, intermediateResult.Bounds.Height);
		}
		protected internal virtual int GetMinViewInfoHeight(AppointmentViewInfo viewInfo) {
			return Painter.GetTopBorderWidth(viewInfo) + Painter.GetBottomBorderWidth(viewInfo) + AppointmentViewInfo.SelectedBorderWidth + 2;
		}
		protected internal override int RecalcEndOffset(Rectangle viewInfoBounds, Rectangle lastCellBounds) {
			if (lastCellBounds.Height == 0)
				return 0;
			else {
				int endOffset = (int)((lastCellBounds.Bottom - viewInfoBounds.Bottom) * 100 / lastCellBounds.Height);
				return Math.Max(0, endOffset);
			}
		}
		protected internal override FitIntoCellResult FitIntoCells(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			return FitIntoNonScrollingCells(intermediateResult, cellsInfo);
		}
		protected internal override FitIntoCellResult FitIntoFirstCell(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			int visibleCellsTopBorder = cellsInfo.VisibleCells[0].Bounds.Top;
			Rectangle bounds = GetIntermediateResultBounds(intermediateResult, cellsInfo);
			if (bounds.Bottom <= visibleCellsTopBorder) {
				AddUpMoreButton(cellsInfo, intermediateResult);
				return FitIntoCellResult.NotFitted;
			}
			if (bounds.Top < visibleCellsTopBorder)
				return FitIntoCellResult.Truncated;
			return FitIntoCellResult.Fitted;
		}
		protected virtual Rectangle GetIntermediateResultBounds(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			return intermediateResult.Bounds;
		}
		protected internal override FitIntoCellResult FitIntoLastCell(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			SchedulerViewCellBaseCollection visibleCells = cellsInfo.VisibleCells;
			int visibleCellsBottomBorder = visibleCells[visibleCells.Count - 1].Bounds.Bottom;
			Rectangle bounds = GetIntermediateResultBounds(intermediateResult, cellsInfo);
			if (bounds.Top >= visibleCellsBottomBorder) {
				AddDownMoreButton(cellsInfo, intermediateResult);
				return FitIntoCellResult.NotFitted;
			}
			if (bounds.Bottom > visibleCellsBottomBorder)
				return FitIntoCellResult.Truncated;
			return FitIntoCellResult.Fitted;
		}
		protected internal override void CalculateViewInfoBorders(AppointmentIntermediateViewInfoCore intermediateResult) {
			IAppointmentViewInfo viewInfo = intermediateResult.ViewInfo;
			viewInfo.HasRightBorder = true;
			viewInfo.HasLeftBorder = true;
			viewInfo.HasTopBorder = intermediateResult.HasStartBorder;
			viewInfo.HasBottomBorder = intermediateResult.HasEndBorder;
		}
		protected internal abstract void AddUpMoreButton(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfo intermediateResult);
		protected internal abstract void AddDownMoreButton(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfo intermediateResult);
	}
	#endregion
	#region VerticalAppointmentLayoutCalculator
	public abstract class VerticalAppointmentLayoutCalculator : VerticalAppointmentLayoutCalculatorBase {
		protected VerticalAppointmentLayoutCalculator(DayViewInfo viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal new DayViewInfo ViewInfo { get { return (DayViewInfo)base.ViewInfo; } }
		protected internal DayView View { get { return ViewInfo.View; } }
		protected override void AfterFinalLayoutCalculation(VisuallyContinuousCellsInfoCollection cellsInfos, AppointmentsLayoutResult layoutResult) {
			base.AfterFinalLayoutCalculation(cellsInfos, layoutResult);
			FilterMoreButtons(layoutResult);
		}
		protected internal virtual void FilterMoreButtons(AppointmentsLayoutResult layoutResult) {
			MoreButtonCollection filteredButtons = GetFilteredMoreButtons(layoutResult.MoreButtons);
			layoutResult.MoreButtons = filteredButtons;
		}
		protected internal virtual MoreButtonCollection GetFilteredMoreButtons(MoreButtonCollection moreButtons) {
			if (View.ShowMoreButtonsOnEachColumn)
				return moreButtons;
			else
				return GetFilteredMoreButtonsCore(moreButtons);
		}
		protected internal virtual MoreButtonCollection GetFilteredMoreButtonsCore(MoreButtonCollection moreButtons) {
			MoreButtonCollection result = new MoreButtonCollection();
			MoreButton upButton = GetUpMoreButton(moreButtons);
			MoreButton downButton = GetDownMoreButton(moreButtons);
			if (upButton != MoreButton.Empty)
				result.Add(upButton);
			if (downButton != MoreButton.Empty)
				result.Add(downButton);
			RecalcMoreButtonsProperties(result);
			return result;
		}
		protected internal virtual MoreButton GetUpMoreButton(MoreButtonCollection moreButtons) {
			MoreButtonCollection upMoreButtons = FilterUpMoreButtons(moreButtons);
			MoreButton button = FindMaxViewTimeButton(upMoreButtons);
			return button;
		}
		protected internal virtual MoreButton GetDownMoreButton(MoreButtonCollection moreButtons) {
			MoreButtonCollection downMoreButtons = FilterDownMoreButtons(moreButtons);
			MoreButton button = FindMinViewTimeButton(downMoreButtons);
			return button;
		}
		protected internal virtual MoreButtonCollection FilterUpMoreButtons(MoreButtonCollection moreButtons) {
			return FilterMoreButtons(moreButtons, true);
		}
		protected internal virtual MoreButtonCollection FilterDownMoreButtons(MoreButtonCollection moreButtons) {
			return FilterMoreButtons(moreButtons, false);
		}
		protected internal virtual MoreButtonCollection FilterMoreButtons(MoreButtonCollection moreButtons, bool goUp) {
			MoreButtonCollection result = new MoreButtonCollection();
			int count = moreButtons.Count;
			for (int i = 0; i < count; i++) {
				MoreButton button = moreButtons[i];
				if (button.GoUp == goUp)
					result.Add(button);
			}
			return result;
		}
		protected internal virtual MoreButton FindMaxViewTimeButton(MoreButtonCollection moreButtons) {
			int count = moreButtons.Count;
			if (count == 0)
				return MoreButton.Empty;
			else {
				MoreButton result = moreButtons[0];
				for (int i = 1; i < count; i++)
					result = GetMaxViewTimeButton(result, moreButtons[i]);
				return result;
			}
		}
		protected internal virtual MoreButton FindMinViewTimeButton(MoreButtonCollection moreButtons) {
			int count = moreButtons.Count;
			if (count == 0)
				return MoreButton.Empty;
			else {
				MoreButton result = moreButtons[0];
				for (int i = 1; i < count; i++)
					result = GetMinViewTimeButton(result, moreButtons[i]);
				return result;
			}
		}
		protected internal virtual MoreButton GetMaxViewTimeButton(MoreButton button1, MoreButton button2) {
			if (button1.TargetViewStart.Ticks > button2.TargetViewStart.Ticks)
				return button1;
			else
				return button2;
		}
		protected internal virtual MoreButton GetMinViewTimeButton(MoreButton button1, MoreButton button2) {
			if (button1.TargetViewStart.Ticks < button2.TargetViewStart.Ticks)
				return button1;
			else
				return button2;
		}
		protected internal virtual void RecalcMoreButtonsProperties(MoreButtonCollection moreButtons) {
			int count = moreButtons.Count;
			for (int i = 0; i < count; i++) {
				moreButtons[i].Resource = ResourceBase.Empty;
				RecalcMoreButtonInterval(moreButtons[i]);
			}
		}
		protected internal virtual void RecalcMoreButtonInterval(MoreButton moreButton) {
			XtraSchedulerDebug.Assert(ViewInfo.CellContainers.Count != 0);
			SchedulerViewCellBaseCollection firstColumnCells = ViewInfo.CellContainers[0].Cells;
			int cellsCount = firstColumnCells.Count;
			XtraSchedulerDebug.Assert(cellsCount != 0);
			if (moreButton.GoUp)
				moreButton.Interval = firstColumnCells[0].Interval;
			else
				moreButton.Interval = firstColumnCells[cellsCount - 1].Interval;
		}
		protected internal override void AddUpMoreButton(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfo intermediateResult) {
			DayViewVisuallyContinuousCellsInfo verticalCellsInfo = (DayViewVisuallyContinuousCellsInfo)cellsInfo;
			if (!View.ShowMoreButtons)
				return;
			DateTime targetViewStart = CalculateTargetViewStart(verticalCellsInfo, intermediateResult.FirstCellIndex);
			MoreButton currentMoreButton = cellsInfo.MoreButtons[0];
			if (ShouldUpdateMoreButton(currentMoreButton, targetViewStart))
				cellsInfo.MoreButtons[0] = CreateUpMoreButton(verticalCellsInfo, targetViewStart);
		}
		protected internal override void AddDownMoreButton(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfo intermediateResult) {
			DayViewVisuallyContinuousCellsInfo verticalCellsInfo = (DayViewVisuallyContinuousCellsInfo)cellsInfo;
			if (!View.ShowMoreButtons)
				return;
			TimeInterval lastAptCellInterval = verticalCellsInfo.GetIntervalByIndex(intermediateResult.LastCellIndex);
			DateTime newCellsStart = lastAptCellInterval.End - verticalCellsInfo.VisibleInterval.Duration;
			int firstVisibleCellIndex = verticalCellsInfo.GetIndexByStartDate(newCellsStart);
			DateTime viewStart = CalculateTargetViewStart(verticalCellsInfo, firstVisibleCellIndex);
			MoreButton currentMoreButton = cellsInfo.MoreButtons[1];
			if (ShouldUpdateMoreButton(currentMoreButton, viewStart))
				cellsInfo.MoreButtons[1] = CreateDownMoreButton(verticalCellsInfo, viewStart);
		}
		protected internal virtual DateTime CalculateTargetViewStart(DayViewVisuallyContinuousCellsInfo cellsInfo, int firstVisibleCellIndex) {
			XtraSchedulerDebug.Assert(cellsInfo.Rows.Count > firstVisibleCellIndex);
			TimeSpan offset = cellsInfo.Rows[firstVisibleCellIndex].Interval.Start;
			return ViewInfo.VisibleIntervals.Start.Add(offset);
		}
		protected internal virtual MoreButton CreateUpMoreButton(DayViewVisuallyContinuousCellsInfo cellsInfo, DateTime targetViewStart  ) {
			XtraSchedulerDebug.Assert(cellsInfo.VisibleCells.Count != 0);
			MoreButton moreButton = ViewInfo.CreateMoreButton();
			moreButton.GoUp = true;
			moreButton.Bounds = CalculateUpMoreButtonBounds(cellsInfo);
			moreButton.Interval = cellsInfo.VisibleCells[0].Interval.Clone();
			moreButton.TargetViewStart = targetViewStart;
			moreButton.Resource = cellsInfo.Resource;
			return moreButton;
		}
		protected internal virtual MoreButton CreateDownMoreButton(DayViewVisuallyContinuousCellsInfo cellsInfo, DateTime viewStart  ) {
			int visibleCellsCount = cellsInfo.VisibleCells.Count;
			XtraSchedulerDebug.Assert(visibleCellsCount != 0);
			MoreButton moreButton = ViewInfo.CreateMoreButton();
			moreButton.GoUp = false;
			moreButton.Bounds = CalculateDownMoreButtonBounds(cellsInfo);
			moreButton.Interval = cellsInfo.VisibleCells[visibleCellsCount - 1].Interval.Clone();
			moreButton.TargetViewStart = viewStart;
			moreButton.Resource = cellsInfo.Resource;
			return moreButton;
		}
		protected internal virtual Rectangle CalculateUpMoreButtonBounds(IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle availableBounds = CalculateMoreButtonAvailableBounds(cellsInfo);
			return new Rectangle(availableBounds.Right - MoreButtonSize.Width - 2, availableBounds.Top + 2, MoreButtonSize.Width, MoreButtonSize.Height);
		}
		protected internal virtual Rectangle CalculateDownMoreButtonBounds(IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle availableBounds = CalculateMoreButtonAvailableBounds(cellsInfo);
			Rectangle bounds = new Rectangle(availableBounds.Right - MoreButtonSize.Width - 2, availableBounds.Bottom - MoreButtonSize.Height - 2, MoreButtonSize.Width, MoreButtonSize.Height);
			return bounds;
		}
		protected internal virtual Rectangle CalculateMoreButtonAvailableBounds(IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle availableBounds;
			DayViewInfo dayViewInfo = (DayViewInfo)ViewInfo;
			if (dayViewInfo.View.ShowMoreButtonsOnEachColumn)
				availableBounds = CalculateMoreButtonBoundsAtColumn(cellsInfo);
			else
				availableBounds = CalculateMoreButtonBoundsAtTimeRuler();
			return availableBounds;
		}
		protected internal virtual Rectangle CalculateMoreButtonBoundsAtColumn(IVisuallyContinuousCellsInfo cellsInfo) {
			SchedulerViewCellBaseCollection visibleCells = cellsInfo.VisibleCells;
			Rectangle firstCellBoudns = visibleCells[0].Bounds;
			Rectangle lastCellBounds = visibleCells[visibleCells.Count - 1].Bounds;
			return Rectangle.FromLTRB(firstCellBoudns.Left, firstCellBoudns.Top, firstCellBoudns.Right, lastCellBounds.Bottom);
		}
		protected internal virtual Rectangle CalculateMoreButtonBoundsAtTimeRuler() {
			Rectangle bounds;
			DayViewInfo dayViewInfo = (DayViewInfo)ViewInfo;
			int width = 4 + MoreButtonSize.Width;
			Rectangle rowsBounds = dayViewInfo.CellsPreliminaryLayoutResult.RowsBounds;
			if (dayViewInfo.TimeRulers.Count == 0)
				bounds = RectUtils.GetLeftSideRect(rowsBounds, width);
			else
				bounds = new Rectangle(rowsBounds.Left - width, rowsBounds.Top, width, rowsBounds.Height);
			return bounds;
		}
	}
	#endregion
	#region HorizontalAppointmentLayoutCalculator
	public abstract class HorizontalAppointmentLayoutCalculator : AppointmentBaseLayoutCalculator {
		protected HorizontalAppointmentLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal abstract void CalculateAppointmentRelativePositions(AppointmentIntermediateViewInfoCollection viewInfos, int maxHeight);
		protected internal abstract FitIntoCellResult FitIntoFirstCellCore(AppointmentIntermediateViewInfo intermediateResult, Rectangle firstCellBounds);
		protected internal abstract int CalculateViewInfoContentHeight(AppointmentIntermediateViewInfo intermediateViewInfo);
		protected internal virtual bool DrawMoreButtonsOverAppointments { get { return ViewInfo.DrawMoreButtonsOverAppointments; } }
		public int AppointmentVerticalInterspacing { get { return ViewInfo.AppointmentDisplayOptions.InnerAppointmentVerticalInterspacing; } }
		public void CalculateViewInfosVerticalBounds(AppointmentIntermediateViewInfoCollection intermidiateViewInfos, IVisuallyContinuousCellsInfo cellsInfo) {
			foreach (AppointmentIntermediateViewInfo intermediateResult in intermidiateViewInfos)
				CalculateViewInfoVerticalBounds(intermediateResult, cellsInfo);
		}
		void CalculateViewInfoVerticalBounds(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle firstCellBounds = cellsInfo.GetContentBounds(intermediateResult.FirstCellIndex);
			int top = CalculateTop(firstCellBounds, intermediateResult);
			intermediateResult.Bounds = Rectangle.FromLTRB(0, top, 0, top + intermediateResult.Height);
		}
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new HorizontalAppointmentIntermediateLayoutCalculator(Scale, AppointmentsSnapToCells, ViewInfo.TimeZoneHelper);
		}
		protected internal override void LayoutViewInfos(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfoCollection intermediateResult, bool isTwoPassLayout) {
			if (!isTwoPassLayout) {
				CalculateIntermediateViewInfosHeight(intermediateResult);
				int maxHeight = cellsInfo.ScrollContainer.ScrollController.CanScroll ? Int32.MaxValue : cellsInfo.GetTotalBounds().Height;
				CalculateAppointmentRelativePositions(intermediateResult, maxHeight);
			}
		}
		protected override void CalculateViewInfosSizeAndPosition(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfoCollection intermediateResult) {
			CalculateIntermediateViewInfosHeight(intermediateResult);
			int maxHeight = cellsInfo.ScrollContainer.ScrollController.CanScroll ? Int32.MaxValue : cellsInfo.GetTotalBounds().Height;
			CalculateAppointmentRelativePositions(intermediateResult, maxHeight);
		}
		protected internal virtual void CalculateIntermediateViewInfosHeight(AppointmentIntermediateViewInfoCollection viewInfos) {
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (ViewInfo.CancellationToken.IsCancellationRequested)
					return;
				CalculateIntermediateViewInfoHeight(viewInfos[i]);
			}
		}
		protected internal virtual int CalculateViewInfoFrameHeight(AppointmentViewInfo viewInfo) {
			int contentPaddings = Painter.GetTopContentPadding(viewInfo) + Painter.GetBottomContentPadding(viewInfo);
			int bordersWidth = Painter.GetBottomBorderWidth(viewInfo) + Painter.GetTopBorderWidth(viewInfo);
			int borderHeight = bordersWidth + contentPaddings;
			int appointmentStatusSize = ContentCalculator.CalculateAppointmentStatusSize(viewInfo);
			return borderHeight + appointmentStatusSize;
		}
		protected internal virtual void CalculateIntermediateViewInfoHeight(AppointmentIntermediateViewInfo intermediateViewInfo) {
			int contentHeight = CalculateViewInfoContentHeight(intermediateViewInfo);
			int frameHeight = CalculateViewInfoFrameHeight(intermediateViewInfo.ViewInfo);
			intermediateViewInfo.Height = contentHeight + frameHeight;
		}
		protected internal override void CalculateIntermediateViewInfoBounds(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle firstCellBounds = cellsInfo.GetContentBounds(intermediateResult.FirstCellIndex);
			Rectangle lastCellBounds = cellsInfo.GetContentBounds(intermediateResult.LastCellIndex);
			int left = firstCellBounds.Left + firstCellBounds.Width * intermediateResult.StartRelativeOffset / 100;
			int right = lastCellBounds.Right - lastCellBounds.Width * intermediateResult.EndRelativeOffset / 100;
			if (ShouldApplyPaddings()) {
				left += Painter.LeftPadding;
				right -= Painter.RightPadding;
			}
			int top = CalculateTop(firstCellBounds, intermediateResult);
			intermediateResult.Bounds = Rectangle.FromLTRB(left, top, right, top + intermediateResult.Height);
			if (ShouldExtendViewInfoBounds(intermediateResult)) {
				ExtendIntermediateViewInfoBounds(intermediateResult, cellsInfo);
				RecalcExtendedViewInfoProperties(intermediateResult, cellsInfo);
			}
		}
		protected virtual int CalculateTop(Rectangle firstCellBounds, AppointmentIntermediateViewInfo intermediateResult) {
			return firstCellBounds.Top + Painter.TopPadding + intermediateResult.RelativePosition;
		}
		internal bool ShouldApplyPaddings() {
			AppointmentSnapToCellsMode snapToCells = ViewInfo.AppointmentDisplayOptions.SnapToCellsMode;
			return (snapToCells == AppointmentSnapToCellsMode.Always || snapToCells == AppointmentSnapToCellsMode.Auto);
		}
		internal virtual bool ShouldExtendViewInfoBounds(AppointmentIntermediateViewInfo intermediateResult) {
			if (AppointmentsSnapToCells == AppointmentSnapToCellsMode.Never)
				return intermediateResult.Bounds.Width < GetMinViewInfoWidth(intermediateResult.ViewInfo);
			return false;
		}
		protected internal virtual int GetMinViewInfoWidth(AppointmentViewInfo viewInfo) {
			int result = AppointmentViewInfo.SelectedBorderWidth + 2;
			if (viewInfo.HasLeftBorder)
				result += Painter.GetLeftBorderWidth(viewInfo);
			if (viewInfo.HasRightBorder)
				result += Painter.GetRightBorderWidth(viewInfo);
			return result;
		}
		protected internal virtual void ExtendIntermediateViewInfoBounds(AppointmentIntermediateViewInfo intermediateViewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle totalBounds = cellsInfo.GetTotalBounds();
			int minWidth = GetMinViewInfoWidth(intermediateViewInfo.ViewInfo);
			int actualWidth = Math.Min(totalBounds.Right - intermediateViewInfo.Bounds.Left, minWidth);
			intermediateViewInfo.Bounds = RectUtils.GetLeftSideRect(intermediateViewInfo.Bounds, actualWidth);
		}
		protected internal override int RecalcEndOffset(Rectangle viewInfoBounds, Rectangle lastCellBounds) {
			if (lastCellBounds.Width == 0)
				return 0;
			else {
				int endOffset = (int)((lastCellBounds.Right - viewInfoBounds.Right) * 100 / lastCellBounds.Width);
				return Math.Max(0, endOffset);
			}
		}
		protected internal override void PreliminaryContentLayout(AppointmentIntermediateViewInfoCollection intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			int count = intermediateResult.Count;
			for (int i = 0; i < count; i++) {
				if (ViewInfo.CancellationToken.IsCancellationRequested)
					return;
				AppointmentIntermediateViewInfo intermediateViewInfo = intermediateResult[i];
				PreliminaryContentLayoutCore(intermediateViewInfo, cellsInfo);
			}
		}
		protected internal virtual void PreliminaryContentLayoutCore(AppointmentIntermediateViewInfo intermediateViewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			CalculatePreliminaryViewInfoProperties(intermediateViewInfo, cellsInfo);
			ContentCalculator.CalculatePreliminaryContentLayout(GraphicsCachePool.GetCache(), intermediateViewInfo.ViewInfo);
		}
		protected internal virtual void CalculatePreliminaryViewInfoProperties(AppointmentIntermediateViewInfo intermediateViewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			CalculateViewInfoProperties(intermediateViewInfo, cellsInfo);
			CalculateIntermediateViewInfoBounds(intermediateViewInfo, cellsInfo);
			intermediateViewInfo.Bounds = RectUtils.GetTopSideRect(intermediateViewInfo.Bounds, Int32.MaxValue);
		}
		protected internal override void FinalContentLayoutCore(AppointmentViewInfoCollection viewInfos, GraphicsCache cache) {
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (ViewInfo.CancellationToken.IsCancellationRequested)
					return;
				AppointmentViewInfo viewInfo = viewInfos[i];
				CalculateFinalViewInfoProperties(viewInfo);
				ContentCalculator.CalculateFinalContentLayout(cache, viewInfo);
			}
		}
		protected internal virtual void CalculateFinalViewInfoProperties(AppointmentViewInfo viewInfo) {
			viewInfo.CalcBorderBounds(Painter);
			viewInfo.CalcInnerBounds(Painter);
			ContentCalculator.RecalcInnerBounds(viewInfo);
		}
		protected internal virtual void AddMoreButtons(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfo intermediateResult) {
			if (!ViewInfo.ShowMoreButtons)
				return;
			TimeInterval aptInterval = ((IInternalAppointment)intermediateResult.Appointment).CreateInterval();
			for (int i = intermediateResult.FirstCellIndex; i <= intermediateResult.LastCellIndex; i++) {
				SchedulerViewCellBase cell = cellsInfo.VisibleCells[i];
				DateTime viewStart = CalculateViewStart(aptInterval, cell);
				MoreButton currentMoreButton = cellsInfo.MoreButtons[i];
				if (ShouldUpdateMoreButton(currentMoreButton, viewStart))
					cellsInfo.MoreButtons[i] = CreateMoreButton(cell, viewStart);
			}
		}
		protected internal virtual DateTime CalculateViewStart(TimeInterval aptInterval, SchedulerViewCellBase cell) {
			return TimeInterval.Intersect(aptInterval, cell.Interval).Start;
		}
		protected internal virtual void AddMoreButtonAtLastCell(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfo intermediateResult) {
			int firstCellIndex = intermediateResult.FirstCellIndex;
			intermediateResult.FirstCellIndex = intermediateResult.LastCellIndex;
			AddMoreButtons(cellsInfo, intermediateResult);
			intermediateResult.FirstCellIndex = firstCellIndex;
		}
		protected internal virtual MoreButton CreateMoreButton(SchedulerViewCellBase cell, DateTime viewStart) {
			MoreButton moreButton = ViewInfo.CreateMoreButton();
			moreButton.Interval = cell.Interval.Clone();
			moreButton.Bounds = CalculateMoreButtonBounds(cell);
			moreButton.Resource = cell.Resource;
			moreButton.GoUp = false;
			moreButton.TargetViewStart = viewStart;
			return moreButton;
		}
		private Rectangle CalculateMoreButtonBounds(SchedulerViewCellBase cell) {
			int buttonLeftBound = cell.Bounds.Right - MoreButtonSize.Width - Painter.RightPadding;
			int cellLeftBound = cell.Bounds.Left + Painter.LeftPadding;
			int cellRightBound = cell.Bounds.Right - Painter.RightPadding;
			int left = Math.Max(cellLeftBound, buttonLeftBound);
			int width = Math.Min(MoreButtonSize.Width, cellRightBound - cellLeftBound);
			return new Rectangle(left, cell.Bounds.Bottom - MoreButtonSize.Height - 1, width, MoreButtonSize.Height);
		}
		protected internal virtual bool CanFitViewInfoIntoCell(AppointmentIntermediateViewInfo intermediateResult, Rectangle cellContentBounds) {
			return (intermediateResult.Bounds.Bottom + Painter.BottomPadding <= cellContentBounds.Bottom);
		}
		protected internal virtual void RecalcTruncatedViewInfoProperties(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			intermediateResult.LastCellIndex--;
			intermediateResult.HasEndBorder = false;
			intermediateResult.ViewInfo.Interval.End = cellsInfo.GetIntervalByIndex(intermediateResult.LastCellIndex).End;
			CalculateIntermediateViewInfoBounds(intermediateResult, cellsInfo);
		}
		protected internal override FitIntoCellResult FitIntoLastCell(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle lastCellBounds = cellsInfo.GetContentBounds(intermediateResult.LastCellIndex);
			if ((intermediateResult.FirstCellIndex != intermediateResult.LastCellIndex) && !CanFitViewInfoIntoCell(intermediateResult, lastCellBounds)) {
				AddMoreButtonAtLastCell(cellsInfo, intermediateResult);
				RecalcTruncatedViewInfoProperties(intermediateResult, cellsInfo);
				return FitIntoCellResult.Truncated;
			}
			XtraSchedulerDebug.Assert(intermediateResult.ViewInfo.Bounds == intermediateResult.Bounds);
			return FitIntoCellResult.Fitted;
		}
		protected internal override AppointmentViewInfoCollection CreateViewInfoCollection(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo, FitIntoCellResult fitResult) {
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			if (fitResult == FitIntoCellResult.Truncated)
				return result;
			if (fitResult == FitIntoCellResult.NotFitted)
				return result;
			CalculateViewInfoProperties(intermediateResult, cellsInfo);
			result.Add(intermediateResult.ViewInfo);
			return result;
		}
		protected internal virtual FitIntoCellResult FitIntoScrollingCells(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			FitIntoFirstCell(intermediateResult, cellsInfo);
			return FitIntoCellResult.Fitted;
		}
		protected internal override FitIntoCellResult FitIntoCells(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			if (intermediateResult.ViewInfo.CanScroll)
				return FitIntoScrollingCells(intermediateResult, cellsInfo);
			return FitIntoNonScrollingCells(intermediateResult, cellsInfo);
		}
		protected internal override FitIntoCellResult FitIntoFirstCell(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			Rectangle firstCellBounds = cellsInfo.GetContentBounds(intermediateResult.FirstCellIndex);
			FitIntoCellResult fitResult = FitIntoFirstCellCore(intermediateResult, firstCellBounds);
			if (fitResult == FitIntoCellResult.NotFitted)
				AddMoreButtons(cellsInfo, intermediateResult);
			XtraSchedulerDebug.Assert(intermediateResult.ViewInfo.Bounds == intermediateResult.Bounds);
			return fitResult;
		}
		protected internal override void CalculateViewInfoBorders(AppointmentIntermediateViewInfoCore intermediateResult) {
			IAppointmentViewInfo viewInfo = intermediateResult.ViewInfo;
			bool showBorders = ShouldShowBorders(viewInfo);
			viewInfo.HasTopBorder = true;
			viewInfo.HasBottomBorder = true;
			viewInfo.HasLeftBorder = intermediateResult.HasStartBorder && showBorders;
			viewInfo.HasRightBorder = intermediateResult.HasEndBorder && showBorders;
		}
		protected internal virtual bool ShouldShowBorders(IAppointmentViewInfo viewInfo) {
			return true;
		}
		internal override AppointmentIntermediateViewInfoCollection FilterInvisibleAppointments(AppointmentIntermediateViewInfoCollection viewInfos, IVisuallyContinuousCellsInfo cellsInfo) {
			AppointmentIntermediateViewInfoCollection result = new AppointmentIntermediateViewInfoCollection();
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++)
				if (viewInfos[i].RelativePosition >= 0)
					result.Add(viewInfos[i]);
				else
					AddMoreButtons(cellsInfo, viewInfos[i]);
			return result;
		}
		protected internal override AppointmentViewInfoCollection RecalcSnapToCells(AppointmentIntermediateViewInfoCollection intermediateViewInfos, IVisuallyContinuousCellsInfo cellsInfo, int startIndex) {
			AppointmentIntermediateViewInfo intermediateViewInfo = intermediateViewInfos[startIndex];
			PreliminaryContentLayoutCore(intermediateViewInfo, cellsInfo);
			CalculateIntermediateViewInfoHeight(intermediateViewInfo);
			HorizontalAppointmentAutoHeightRelativePositionsCalculator relativePositionCalculator = new HorizontalAppointmentAutoHeightRelativePositionsCalculator(AppointmentVerticalInterspacing);
			relativePositionCalculator.CalculateAppointmentRelativePositions(ViewInfo.CancellationToken.Token, intermediateViewInfos, startIndex, cellsInfo.GetTotalBounds().Height);
			return SnapToCellsCore(intermediateViewInfos[startIndex], cellsInfo);
		}
	}
	#endregion
	#region HorizontalAppointmentFixedHeightLayoutCalculator
	public abstract class HorizontalAppointmentFixedHeightLayoutCalculator : HorizontalAppointmentLayoutCalculator {
		protected internal HorizontalAppointmentFixedHeightLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal int AppointmentHeight { get { return ViewInfo.AppointmentDisplayOptions.AppointmentHeight; } }
		protected internal override void CalculateAppointmentRelativePositions(AppointmentIntermediateViewInfoCollection viewInfos, int maxHeight) {
			HorizontalAppointmentFixedHeightRelativePositionsCalculator calculator = new HorizontalAppointmentFixedHeightRelativePositionsCalculator(AppointmentVerticalInterspacing);
			calculator.CalculateAppointmentRelativePositions(viewInfos, ViewInfo.CancellationToken.Token);
		}
		protected internal override void CalculateIntermediateViewInfoHeight(AppointmentIntermediateViewInfo viewInfo) {
			if (AppointmentHeight != 0)
				viewInfo.Height = AppointmentHeight;
			else
				base.CalculateIntermediateViewInfoHeight(viewInfo);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected internal override int CalculateViewInfoContentHeight(AppointmentIntermediateViewInfo intermediateViewInfo) {
			SizeF textSize = DefaultAppointmentAppearance.CalcTextSize(GraphicsCachePool.GetCache(), " ", Int32.MaxValue);
			int contentHeight = (int)Math.Ceiling(textSize.Height);
			contentHeight = Math.Max(contentHeight, ImageCollection.GetImageListSize(AppointmentImages).Height);
			return contentHeight;
		}
		protected internal override FitIntoCellResult FitIntoFirstCellCore(AppointmentIntermediateViewInfo intermediateResult, Rectangle firstCellBounds) {
			if (CanFitViewInfoIntoCell(intermediateResult, firstCellBounds))
				return FitIntoCellResult.Fitted;
			else
				return FitIntoCellResult.NotFitted;
		}
		protected override int CalculateMinAppointmentHeight(IAppointmentIntermediateViewInfoCoreCollection appointmenstIntermediateViewInfo) {
			if (ViewInfo.ShouldShowContainerScrollBar())
				return 0;
			bool snapToCells = ViewInfo.AppointmentDisplayOptions.SnapToCellsMode == AppointmentSnapToCellsMode.Always || ViewInfo.AppointmentDisplayOptions.SnapToCellsMode == AppointmentSnapToCellsMode.Auto;
			if (appointmenstIntermediateViewInfo.Count <= 0 || !snapToCells)
				return 0;
			CalculateIntermediateViewInfoHeight(((AppointmentIntermediateViewInfo)appointmenstIntermediateViewInfo[0]));
			return ((AppointmentIntermediateViewInfo)appointmenstIntermediateViewInfo[0]).Height;
		}
	}
	#endregion
	#region HorizontalAppointmentAutoHeightLayoutCalculator
	public abstract class HorizontalAppointmentAutoHeightLayoutCalculator : HorizontalAppointmentLayoutCalculator {
		protected internal HorizontalAppointmentAutoHeightLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal override void CalculateAppointmentRelativePositions(AppointmentIntermediateViewInfoCollection viewInfos, int maxHeight) {
			HorizontalAppointmentAutoHeightRelativePositionsCalculator calculator = new HorizontalAppointmentAutoHeightRelativePositionsCalculator(AppointmentVerticalInterspacing);
			calculator.CalculateAppointmentRelativePositions(ViewInfo.CancellationToken.Token, viewInfos, maxHeight);
		}
		protected internal override int CalculateViewInfoContentHeight(AppointmentIntermediateViewInfo intermediateViewInfo) {
			ViewInfoItemCollection items = intermediateViewInfo.ViewInfo.Items;
			int count = items.Count;
			int height = 0;
			for (int i = 0; i < count; i++)
				height = Math.Max(height, items[i].Bounds.Height);
			return height;
		}
		protected internal override FitIntoCellResult FitIntoFirstCellCore(AppointmentIntermediateViewInfo intermediateResult, Rectangle firstCellBounds) {
			if (CanFitViewInfoIntoCell(intermediateResult, firstCellBounds))
				return FitIntoCellResult.Fitted;
			if (intermediateResult.ViewInfo.CanScroll)
				return FitIntoCellResult.NotFitted;
			else
				return RestrictAppointmentHeight(intermediateResult.ViewInfo, firstCellBounds.Bottom);
		}
		protected internal virtual FitIntoCellResult RestrictAppointmentHeight(AppointmentViewInfo viewInfo, int bottomBound) {
			Rectangle bounds = viewInfo.Bounds;
			int restrictedHeight = bottomBound - bounds.Y - Painter.BottomPadding;
			int minContentHeight = DefaultAppointmentAppearance.CalcDefaultTextSize(GraphicsCachePool.GetCache().Graphics).Height;
			int minViewInfoHeight = minContentHeight + CalculateViewInfoFrameHeight(viewInfo);
			if (restrictedHeight < minViewInfoHeight)
				return FitIntoCellResult.NotFitted;
			else {
				viewInfo.Bounds = new Rectangle(bounds.Location, new Size(bounds.Width, restrictedHeight));
				return FitIntoCellResult.Fitted;
			}
		}
		internal override void HideUnfittedAppointment(AppointmentViewInfo viewInfo, Rectangle moreButtonBounds) {
			FitIntoCellResult fitResult = RestrictAppointmentHeight(viewInfo, moreButtonBounds.Top);
			if (fitResult == FitIntoCellResult.NotFitted)
				viewInfo.Visibility.InvisibleMoreButton = true;
		}
	}
	#endregion
	#region WeekViewAppointmentFixedHeightLayoutCalculator
	public class WeekViewAppointmentFixedHeightLayoutCalculator : HorizontalAppointmentFixedHeightLayoutCalculator {
		public WeekViewAppointmentFixedHeightLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal override AppointmentBaseComparer CreateAppointmentComparer() {
			return ViewInfo.AppointmentComparerProvider.CreateWeekViewAppointmentComparer();
		}
		protected internal override TimeScale CreateScale() {
			return new TimeScaleDay();
		}
		protected internal override bool ShouldShowBorders(IAppointmentViewInfo viewInfo) {
			if (viewInfo.Appointment.SameDay)
				return ((WeekViewAppointmentDisplayOptions)ViewInfo.AppointmentDisplayOptions).ShowBordersForSameDayAppointments;
			else
				return true;
		}
	}
	#endregion
	#region WeekViewAppointmentAutoHeightLayoutCalculator
	public class WeekViewAppointmentAutoHeightLayoutCalculator : HorizontalAppointmentAutoHeightLayoutCalculator {
		public WeekViewAppointmentAutoHeightLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal override TimeScale CreateScale() {
			return new TimeScaleDay();
		}
		protected internal override AppointmentBaseComparer CreateAppointmentComparer() {
			return ViewInfo.AppointmentComparerProvider.CreateWeekViewAppointmentComparer();
		}
		protected internal override bool ShouldShowBorders(IAppointmentViewInfo viewInfo) {
			if (viewInfo.Appointment.SameDay)
				return ((WeekViewAppointmentDisplayOptions)ViewInfo.AppointmentDisplayOptions).ShowBordersForSameDayAppointments;
			else
				return true;
		}
	}
	#endregion
	#region MonthViewAppointmentFixedHeightLayoutCalculator
	public class MonthViewAppointmentFixedHeightLayoutCalculator : WeekViewAppointmentFixedHeightLayoutCalculator {
		bool actualCompressWeekEnd;
		public MonthViewAppointmentFixedHeightLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter, bool actualCompressWeekEnd)
			: base(viewInfo, contentCalculator, cache, painter) {
			this.actualCompressWeekEnd = actualCompressWeekEnd;
		}
		internal bool ActualCompressWeekEnd { get { return actualCompressWeekEnd; } }
		protected internal override bool ShouldRecalcSnapToCells(AppointmentViewInfoCollection snapResult) {
			return (snapResult.Count == 0) && ActualCompressWeekEnd;
		}
	}
	#endregion
	#region MonthViewAppointmentAutoHeightLayoutCalculator
	public class MonthViewAppointmentAutoHeightLayoutCalculator : WeekViewAppointmentAutoHeightLayoutCalculator {
		bool actualCompressWeekEnd;
		public MonthViewAppointmentAutoHeightLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter, bool actualCompressWeekEnd)
			: base(viewInfo, contentCalculator, cache, painter) {
			this.actualCompressWeekEnd = actualCompressWeekEnd;
		}
		internal bool ActualCompressWeekEnd { get { return actualCompressWeekEnd; } }
		protected internal override bool ShouldRecalcSnapToCells(AppointmentViewInfoCollection snapResult) {
			return (snapResult.Count == 0) && ActualCompressWeekEnd;
		}
	}
	#endregion
	#region AppointmentLayoutAutoHeightCellsHelper
	public abstract class AppointmentLayoutAutoHeightCellsHelper {
		readonly HorizontalAppointmentLayoutCalculator layoutCalculator;
		protected AppointmentLayoutAutoHeightCellsHelper(HorizontalAppointmentLayoutCalculator layoutCalculator) {
			Guard.ArgumentNotNull(layoutCalculator, "layoutCalculator");
			this.layoutCalculator = layoutCalculator;
		}
		internal ISupportAppointments ViewInfo { get { return layoutCalculator.ViewInfo; } }
		internal HorizontalAppointmentLayoutCalculator LayoutCalculator { get { return layoutCalculator; } }
		internal AppointmentContentLayoutCalculator ContentCalculator { get { return layoutCalculator.ContentCalculator; } }
		public virtual CellsLayerInfos CalculatePreliminaryLayout(AppointmentBaseCollection appointments, ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			CellsLayerInfos result = new CellsLayerInfos();
			PreparedAppointmentsCollection preparedAppointments = layoutCalculator.PrepareAppointments(appointments, cellsInfos);
			int count = preparedAppointments.Count;
			for (int i = 0; i < count; i++) {
				PreparedAppointments preparedApt = preparedAppointments[i];
				VisuallyContinuousCellsInfo cellsInfo = (VisuallyContinuousCellsInfo)preparedApt.CellsInfo;
				CellsLayerInfo layerInfo = CreateLayerInfo(cellsInfo);
				AppointmentIntermediateViewInfoCollection aptViewInfos = CalculatePreliminaryLayoutCore(preparedApt.Appointments, cellsInfo);
				layerInfo.AppointmentViewInfos.AddRange(aptViewInfos);
				result.Add(layerInfo);
			}
			return result;
		}
		protected internal virtual CellsLayerInfo CreateLayerInfo(VisuallyContinuousCellsInfo cellsInfo) {
			return new CellsLayerInfo(LayoutCalculator.MoreButtonSize.Height, LayoutCalculator.Painter.BottomPadding, cellsInfo);
		}
		protected internal virtual AppointmentIntermediateViewInfoCollection CalculatePreliminaryLayoutCore(AppointmentBaseCollection appointments, IVisuallyContinuousCellsInfo cellsInfo) {
			AppointmentIntermediateViewInfoCollection result = (AppointmentIntermediateViewInfoCollection)layoutCalculator.CreateIntermediateLayoutCalculator().CreateIntermediateViewInfoCollection(cellsInfo.Resource, cellsInfo.Interval);
			layoutCalculator.CalculateIntermediateViewInfos(result, appointments, cellsInfo);
			layoutCalculator.PreliminaryContentLayout(result, cellsInfo);
			layoutCalculator.CalculateIntermediateViewInfosHeight(result);
			layoutCalculator.CalculateAppointmentRelativePositions(result, Int32.MaxValue);
			PreliminarySnapToCells(result, cellsInfo);
			return result;
		}
		protected internal virtual void PreliminarySnapToCells(AppointmentIntermediateViewInfoCollection intermediateViewInfos, IVisuallyContinuousCellsInfo cellsInfo) {
			int count = intermediateViewInfos.Count;
			for (int i = 0; i < count; i++) {
				PreliminarySnapToCellsCore(intermediateViewInfos[i], cellsInfo);
			}
		}
		protected internal virtual void PreliminarySnapToCellsCore(AppointmentIntermediateViewInfo intermediateViewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			SchedulerViewCellBase firstCell = cellsInfo.VisibleCells[intermediateViewInfo.FirstCellIndex];
			SchedulerViewCellBase lastCell = cellsInfo.VisibleCells[intermediateViewInfo.LastCellIndex];
			intermediateViewInfo.ViewInfo.Interval = new TimeInterval(firstCell.Interval.Start, lastCell.Interval.End);
			layoutCalculator.CalculateIntermediateViewInfoBounds(intermediateViewInfo, cellsInfo);
			layoutCalculator.InitializeViewInfoScrollContainer(intermediateViewInfo, cellsInfo);
		}
		public abstract AppointmentsLayoutResult CalculateFinalLayout(CellsLayerInfos cellsLayerInfos, SchedulerViewCellContainerCollection containers, ResourceVisuallyContinuousCellsInfosCollection cellsInfos);
		public virtual void CalculateHeight(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) { }
	}
	#endregion
	#region AllDayAppointmentLayoutHelper
	public class AllDayAppointmentLayoutHelper : AppointmentLayoutAutoHeightCellsHelper {
		public AllDayAppointmentLayoutHelper(HorizontalAppointmentLayoutCalculator layoutCalculator)
			: base(layoutCalculator) {
		}
		public override AppointmentsLayoutResult CalculateFinalLayout(CellsLayerInfos cellLayers, SchedulerViewCellContainerCollection containers, ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			Rectangle allDayAreaBounds = CalculateAllDayAreaBounds(containers);
			AppointmentIntermediateViewInfoCollection viewInfos = cellLayers.GetAppointmentViewInfos();
			DayViewAppointmentsLayoutResult layoutResult = new DayViewAppointmentsLayoutResult();
			AppointmentViewInfoCollection aptViewInfos = FinalSnapToCells(viewInfos, allDayAreaBounds);
			layoutResult.AppointmentViewInfos.AddRange(aptViewInfos);
			LayoutCalculator.FinalContentLayout(aptViewInfos);
			AppointmentStatusViewInfoCollection appointmentStatuses = CreateLongAppointmentStatuses(viewInfos, containers);
			layoutResult.AppointmentStatusViewInfos.AddRange(appointmentStatuses);
			MoreButtonCollection moreButtons = CalculateScrollMoreButtons(containers);
			layoutResult.MoreButtons.AddRange(moreButtons);
			return layoutResult;
		}
		protected internal virtual Rectangle CalculateAllDayAreaBounds(SchedulerViewCellContainerCollection columns) {
			int count = columns.Count;
			if (count == 0)
				return Rectangle.Empty;
			Rectangle firstCellBounds = ((DayViewColumn)columns[0]).AllDayAreaCell.Bounds;
			Rectangle lastCellBounds = ((DayViewColumn)columns[count - 1]).AllDayAreaCell.Bounds;
			return Rectangle.FromLTRB(firstCellBounds.Left, firstCellBounds.Top, lastCellBounds.Right, lastCellBounds.Bottom);
		}
		protected internal virtual AppointmentStatusViewInfoCollection CreateLongAppointmentStatuses(AppointmentIntermediateViewInfoCollection aptViewInfos, SchedulerViewCellContainerCollection columns) {
			AppointmentStatusViewInfoCollection result = new AppointmentStatusViewInfoCollection();
			int count = aptViewInfos.Count;
			for (int i = 0; i < count; i++) {
				result.AddRange(CreateLongAppointmentStatusesCore(aptViewInfos[i], columns));
			}
			return result;
		}
		protected internal virtual AppointmentStatusViewInfoCollection CreateLongAppointmentStatusesCore(AppointmentIntermediateViewInfo intermediateViewInfo, SchedulerViewCellContainerCollection columns) {
			AppointmentStatusViewInfoCollection result = new AppointmentStatusViewInfoCollection();
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(CreateLongAppointmentStatusSingleColumn(intermediateViewInfo, (DayViewColumn)columns[i]));
			return result;
		}
		protected internal virtual AppointmentStatusViewInfoCollection CreateLongAppointmentStatusSingleColumn(AppointmentIntermediateViewInfo intermediateViewInfo, DayViewColumn column) {
			AppointmentStatusViewInfoCollection result = new AppointmentStatusViewInfoCollection();
			Appointment appointment = intermediateViewInfo.Appointment;
			if (!ResourceBase.InternalMatchIdToResourceIdCollection(appointment.ResourceIds, column.Resource.Id))
				return result;
			TimeInterval statusInterval = TimeInterval.Intersect(column.VisibleInterval, ((IInternalAppointment)appointment).CreateInterval());
			if (statusInterval.Duration != TimeSpan.Zero) {
				AppointmentStatusViewInfo aptStatus = CreateLongAppointmentStatus(intermediateViewInfo, column, statusInterval);
				result.Add(aptStatus);
			}
			return result;
		}
		private void SetStatusBorders(AppointmentStatusViewInfo aptStatus, Appointment appointment, DayViewColumn column) {
			aptStatus.HasLeftBorder = false;
			aptStatus.HasTopBorder = false;
			aptStatus.HasTopBorder = !(appointment.Start < column.VisibleInterval.Start);
			aptStatus.HasBottomBorder = !(appointment.End > column.VisibleInterval.End);
		}
		protected internal virtual AppointmentStatusViewInfo CreateLongAppointmentStatus(AppointmentIntermediateViewInfo intermediateViewInfo, DayViewColumn column, TimeInterval statusInterval) {
			Appointment appointment = intermediateViewInfo.Appointment;
			Color borderColor = intermediateViewInfo.ViewInfo.Appearance.BorderColor;
			AppointmentStatusViewInfo aptStatus = new AppointmentStatusViewInfo(LayoutCalculator.ViewInfo.GetStatus(appointment.StatusKey), borderColor);
			Rectangle statusBounds = AppointmentTimeScaleHelper.ScaleRectangleVertically(column.StatusLineBounds, column.VisibleInterval, statusInterval);
			statusBounds.Inflate(-1, 0);
			aptStatus.Bounds = statusBounds;
			SetStatusBorders(aptStatus, intermediateViewInfo.Appointment, column);
			return aptStatus;
		}
		protected internal virtual MoreButtonCollection CalculateScrollMoreButtons(SchedulerViewCellContainerCollection columns) {
			if (ShouldCreateScrollMoreButtons()) {
				AllDayScrollMoreButtonsCalculator calculator = new AllDayScrollMoreButtonsCalculator((DayViewInfo)ViewInfo);
				SchedulerViewCellBaseCollection allDayCells = GetAllDayCells(columns);
				return calculator.CalculateMoreButtons(allDayCells);
			}
			return new MoreButtonCollection();
		}
		protected internal virtual SchedulerViewCellBaseCollection GetAllDayCells(SchedulerViewCellContainerCollection columns) {
			SchedulerViewCellBaseCollection result = new SchedulerViewCellBaseCollection();
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				AllDayAreaCell cell = ((DayViewColumn)columns[i]).AllDayAreaCell;
				result.Add(cell);
			}
			return result;
		}
		protected internal virtual bool ShouldCreateScrollMoreButtons() {
			return (ViewInfo.ShowMoreButtons) && (ViewInfo.ShouldShowContainerScrollBar());
		}
		protected internal virtual AppointmentViewInfoCollection FinalSnapToCells(AppointmentIntermediateViewInfoCollection intermediateViewInfos, Rectangle cellsBounds) {
			AppointmentViewInfoCollection aptViewInfos = new AppointmentViewInfoCollection();
			int count = intermediateViewInfos.Count;
			for (int i = 0; i < count; i++) {
				AppointmentViewInfoCollection snap = FinalSnapToCellsCore(intermediateViewInfos[i], cellsBounds);
				aptViewInfos.AddRange(snap);
			}
			return aptViewInfos;
		}
		protected internal virtual AppointmentViewInfoCollection FinalSnapToCellsCore(AppointmentIntermediateViewInfo intermediateViewInfo, Rectangle cellsBounds) {
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			FitIntoCellResult fitResult = FinalFitIntoCells(intermediateViewInfo, cellsBounds);
			if (fitResult == FitIntoCellResult.NotFitted)
				return result;
			AppointmentViewInfo viewInfo = intermediateViewInfo.ViewInfo;
			LayoutCalculator.CalculateViewInfoBorders(intermediateViewInfo);
			result.Add(viewInfo);
			return result;
		}
		protected internal virtual FitIntoCellResult FinalFitIntoCells(AppointmentIntermediateViewInfo intermediateResult, Rectangle allDayAreaBounds) {
			CalculateActualPosition(intermediateResult, allDayAreaBounds);
			if (intermediateResult.ViewInfo.CanScroll)
				return FitIntoCellResult.Fitted;
			else
				return LayoutCalculator.FitIntoFirstCellCore(intermediateResult, allDayAreaBounds);
		}
		protected internal virtual void CalculateActualPosition(AppointmentIntermediateViewInfo intermediateResult, Rectangle allDayAreaBounds) {
			Rectangle bounds = intermediateResult.Bounds;
			bounds.Y += allDayAreaBounds.Top;
			intermediateResult.Bounds = bounds;
		}
	}
	#endregion
	#region HorizontalAppointmentLayoutAutoHeightCellsHelper
	public class HorizontalAppointmentLayoutAutoHeightCellsHelper : AppointmentLayoutAutoHeightCellsHelper {
		public HorizontalAppointmentLayoutAutoHeightCellsHelper(HorizontalAppointmentLayoutCalculator layoutCalculator)
			: base(layoutCalculator) {
		}
		public override AppointmentsLayoutResult CalculateFinalLayout(CellsLayerInfos cellsLayers, SchedulerViewCellContainerCollection containers, ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			AppointmentsLayoutResult result = new AppointmentsLayoutResult();
			int count = cellsLayers.Count;
			for (int i = 0; i < count; i++) {
				AppointmentsLayoutResult currentResult = CalculateFinalLayoutCore(cellsLayers[i]);
				result.Merge(currentResult);
			}
			LayoutCalculator.FinalContentLayout(result.AppointmentViewInfos);
			return result;
		}
		protected internal AppointmentsLayoutResult CalculateFinalLayoutCore(CellsLayerInfoBase cellsLayer) {
			AppointmentsLayoutResult result = new AppointmentsLayoutResult();
			VisuallyContinuousCellsInfoCollection cellsInfos = GetCellsInfos(cellsLayer);
			int count = cellsInfos.Count;
			for (int i = 0; i < count; i++) {
				IVisuallyContinuousCellsInfo cellsInfo = (IVisuallyContinuousCellsInfo)cellsInfos[i];
				AppointmentIntermediateViewInfoCollection aptViewInfos = GetAppointmentViewInfos(cellsLayer, cellsInfo);
				AppointmentsLayoutResult currentResult = LayoutCalculator.SnapToCells(aptViewInfos, cellsInfo);
				result.Merge(currentResult);
				lock (cellsInfo.ScrollContainer.AppointmentViewInfos)
					cellsInfo.ScrollContainer.AppointmentViewInfos.AddRange(result.AppointmentViewInfos);
			}
			return result;
		}
		protected internal VisuallyContinuousCellsInfoCollection GetCellsInfos(CellsLayerInfoBase cellLayer) {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			int count = cellLayer.AppointmentViewInfos.Count;
			for (int i = 0; i < count; i++) {
				IVisuallyContinuousCellsInfo cellInfo = cellLayer.AppointmentViewInfos[i].CellsInfo;
				if (!result.Contains(cellInfo))
					result.Add(cellInfo);
			}
			return result;
		}
		protected internal AppointmentIntermediateViewInfoCollection GetAppointmentViewInfos(CellsLayerInfoBase cellLayer, IVisuallyContinuousCellsInfo cellsInfo) {
			AppointmentIntermediateViewInfoCollection result = new AppointmentIntermediateViewInfoCollection();
			int count = cellLayer.AppointmentViewInfos.Count;
			for (int i = 0; i < count; i++) {
				AppointmentIntermediateViewInfo aptViewInfo = cellLayer.AppointmentViewInfos[i];
				if (aptViewInfo.CellsInfo == cellsInfo)
					result.Add(aptViewInfo);
			}
			return result;
		}
	}
	#endregion
	#region TimelineAppointmentLayoutAutoHeightCellsHelper
	public class TimelineAppointmentLayoutAutoHeightCellsHelper : HorizontalAppointmentLayoutAutoHeightCellsHelper {
		public TimelineAppointmentLayoutAutoHeightCellsHelper(HorizontalAppointmentLayoutCalculator layoutCalculator)
			: base(layoutCalculator) {
		}
		protected internal override CellsLayerInfo CreateLayerInfo(VisuallyContinuousCellsInfo cells) {
			TimelineViewInfo viewInfo = ((TimelineViewInfo)LayoutCalculator.ViewInfo);
			int minHeight = viewInfo.View.CellsAutoHeightOptions.MinHeight;
			if (minHeight == 0)
				minHeight = LayoutCalculator.MoreButtonSize.Height;
			int padding = LayoutCalculator.Painter.BottomPadding + viewInfo.Painter.VerticalHeaderPainter.VerticalOverlap;
			return ((IInternalResource)cells.Resource).IsExpanded ? new CellsLayerInfo(minHeight, padding, cells) : new CollapsedCellsLayerInfo(minHeight, padding, cells);  
		}
	}
	#endregion
	#region DayViewTimeCellAppointmentLayoutCalculator
	public class DayViewTimeCellAppointmentLayoutCalculator : VerticalAppointmentLayoutCalculator {
		public DayViewTimeCellAppointmentLayoutCalculator(DayViewInfo viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal override bool ShowShadow {
			get {
				DayView view = (DayView)View;
				return view.AppointmentDisplayOptions.ShowShadows;
			}
		}
		public override void RecalcAppointmentsVisible(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			foreach (IVisuallyContinuousCellsInfo cellsInfo in cellsInfos.SelectMany(ci => ci.CellsInfoCollection)) {
				DayViewColumn column = cellsInfo.ScrollContainer as DayViewColumn;
				foreach (AppointmentViewInfo vi in column.AppointmentViewInfos)
					RecalcAppointmentVisible(vi, cellsInfo);
			}
		}
		public void RecalcAppointmentVisible(AppointmentViewInfo viewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			DayViewColumn column = (DayViewColumn)cellsInfo.ScrollContainer;
			int visibleCellsTopBorder = cellsInfo.VisibleCells[0].Bounds.Top + column.ScrollOffset;
			int visibleCellsBottomBorder = cellsInfo.VisibleCells[cellsInfo.VisibleCells.Count - 1].Bounds.Bottom + column.ScrollOffset;
			if ((viewInfo.Bounds.Top < visibleCellsTopBorder && viewInfo.Bounds.Bottom < visibleCellsTopBorder) || (viewInfo.Bounds.Bottom > visibleCellsBottomBorder && viewInfo.Bounds.Top > visibleCellsBottomBorder))
				viewInfo.Visibility.InvisibleMoreButton = true;
			else if (!viewInfo.Visibility.Visible)
				viewInfo.Visibility.MakeVisible();
		}
		protected internal override TimeScale CreateScale() {
			return new TimeScaleFixedInterval(((DayView)View).TimeScale);
		}
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new VerticalAppointmentIntermediateLayoutCalculator((TimeScaleFixedInterval)Scale, AppointmentsSnapToCells, ViewInfo.TimeZoneHelper);
		}
		protected override void CalculateViewInfosSizeAndPosition(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfoCollection intermediateResult) {
			DayViewAppointmentDisplayOptions dayViewAppointmentDisplayOptions = (DayViewAppointmentDisplayOptions)ViewInfo.AppointmentDisplayOptions;
			VerticalAppointmentLayoutCalculatorHelper helper = new VerticalAppointmentLayoutCalculatorHelper(Painter, dayViewAppointmentDisplayOptions.ColumnPadding);
			foreach (IAppointmentIntermediateLayoutViewInfoCoreCollection groupedViewInfo in intermediateResult.GroupedViewInfos) {
				if (ViewInfo.CancellationToken.IsCancellationRequested)
					return;
				helper.CalculateViewInfosWidthAndPosition(ViewInfo.CancellationToken.Token, groupedViewInfo, cellsInfo);
			}
		}
		protected internal override void CalculateViewInfoVerticalPosition(AppointmentIntermediateViewInfo intermediateViewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			base.CalculateViewInfoVerticalPosition(intermediateViewInfo, cellsInfo);
			int offset = ((DayViewColumn)cellsInfo.ScrollContainer).ScrollOffset;
			Rectangle bounds = intermediateViewInfo.Bounds;
			intermediateViewInfo.Bounds = Rectangle.FromLTRB(bounds.Left, bounds.Top + offset, bounds.Right, bounds.Bottom + offset);
		}
		protected override Rectangle GetIntermediateResultBounds(AppointmentIntermediateViewInfo intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			return GetBoundsWithoutOffset(intermediateResult, cellsInfo);
		}
		protected override IAppointmentsLayoutResult CalculateFinalLayoutForCellInfo(AppointmentIntermediateViewInfoCollection intermediateResult, IVisuallyContinuousCellsInfo cellsInfo) {
			AppointmentIntermediateViewInfoCollection visibleIntermediateResult = new AppointmentIntermediateViewInfoCollection(intermediateResult.Resource, intermediateResult.Interval);
			visibleIntermediateResult.GroupedViewInfos.AddRange(intermediateResult.GroupedViewInfos);
			DayViewVisuallyContinuousCellsInfo dayViewCellsInfo = (DayViewVisuallyContinuousCellsInfo)cellsInfo;
			int firstVisibleCellIndex = dayViewCellsInfo.GetIndexByStartDate(cellsInfo.VisibleCells[0].Interval.Start);
			int lastVisibleCellIndex = dayViewCellsInfo.GetIndexByEndDate(cellsInfo.VisibleCells[cellsInfo.VisibleCells.Count - 1].Interval.End);
			foreach (AppointmentIntermediateViewInfo intermediateViewInfo in intermediateResult) {
				if (intermediateViewInfo.FirstCellIndex <= lastVisibleCellIndex && intermediateViewInfo.LastCellIndex >= firstVisibleCellIndex && !cellsInfo.ScrollContainer.AppointmentViewInfos.Contains(intermediateViewInfo.ViewInfo))
					visibleIntermediateResult.Add(intermediateViewInfo);
			}
			AppointmentsLayoutResult result = (AppointmentsLayoutResult)base.CalculateFinalLayoutForCellInfo(visibleIntermediateResult, cellsInfo);
			RecalcMoreButtons(intermediateResult, cellsInfo, firstVisibleCellIndex, lastVisibleCellIndex);
			AddMoreButtons(result.MoreButtons, cellsInfo.MoreButtons);
			return result;
		}
		void RecalcMoreButtons(AppointmentIntermediateViewInfoCollection intermediateResult, IVisuallyContinuousCellsInfo cellsInfo, int firstVisibleCellIndex, int lastVisibleCellIndex) {
			AppointmentIntermediateViewInfo viewInfoForUpMoreButton = GetIntermediateViewInfoForUpMoreButton(intermediateResult, firstVisibleCellIndex);
			if (viewInfoForUpMoreButton != null)
				AddUpMoreButton(cellsInfo, viewInfoForUpMoreButton);
			AppointmentIntermediateViewInfo viewInfoForDownMoreButton = GetIntermediateViewInfoForDownMoreButton(intermediateResult, lastVisibleCellIndex);
			if (viewInfoForDownMoreButton != null)
				AddDownMoreButton(cellsInfo, viewInfoForDownMoreButton);
		}
		AppointmentIntermediateViewInfo GetIntermediateViewInfoForUpMoreButton(AppointmentIntermediateViewInfoCollection intermediateResult, int firstVisibleCellIndex) {
			AppointmentIntermediateViewInfo viewInfoForUpMoreButton = null;
			if (firstVisibleCellIndex == 0)
				return viewInfoForUpMoreButton;
			foreach (AppointmentIntermediateViewInfo appViewInfo in intermediateResult.Where(aivi => aivi.LastCellIndex <= firstVisibleCellIndex && aivi.FirstCellIndex != firstVisibleCellIndex)) {
				if (viewInfoForUpMoreButton == null || viewInfoForUpMoreButton.FirstCellIndex < appViewInfo.FirstCellIndex)
					viewInfoForUpMoreButton = appViewInfo;
			}
			return viewInfoForUpMoreButton;
		}
		AppointmentIntermediateViewInfo GetIntermediateViewInfoForDownMoreButton(AppointmentIntermediateViewInfoCollection intermediateResult, int lasVisibleCellIndex) {
			AppointmentIntermediateViewInfo viewInfoForDownMoreButton = null;
			foreach (AppointmentIntermediateViewInfo appViewInfo in intermediateResult.Where(aivi => aivi.FirstCellIndex > lasVisibleCellIndex)) {
				if (viewInfoForDownMoreButton == null || viewInfoForDownMoreButton.LastCellIndex > appViewInfo.LastCellIndex)
					viewInfoForDownMoreButton = appViewInfo;
			}
			return viewInfoForDownMoreButton;
		}
		Rectangle GetBoundsWithoutOffset(AppointmentIntermediateViewInfo intermediateViewInfo, IVisuallyContinuousCellsInfo cellsInfo) {
			int offset = ((DayViewColumn)cellsInfo.ScrollContainer).ScrollOffset;
			Rectangle bounds = intermediateViewInfo.Bounds;
			return Rectangle.FromLTRB(bounds.Left, bounds.Top - offset, bounds.Right, bounds.Bottom - offset);
		}
	}
	#endregion
	#region DayViewAllDayAppointmentFixedHeightLayoutCalculator
	public class DayViewAllDayAppointmentFixedHeightLayoutCalculator : HorizontalAppointmentFixedHeightLayoutCalculator {
		public DayViewAllDayAppointmentFixedHeightLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal override AppointmentSnapToCellsMode AppointmentsSnapToCells { get { return AppointmentSnapToCellsMode.Always; } }
		protected internal override TimeScale CreateScale() {
			return new TimeScaleDay();
		}
		protected override int CalculateTop(Rectangle firstCellBounds, AppointmentIntermediateViewInfo intermediateResult) {
			return Painter.TopPadding + intermediateResult.RelativePosition;
		}
	}
	#endregion
	#region DayViewAllDayAppointmentAutoHeightLayoutCalculator
	public class DayViewAllDayAppointmentAutoHeightLayoutCalculator : HorizontalAppointmentAutoHeightLayoutCalculator {
		public DayViewAllDayAppointmentAutoHeightLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal override AppointmentSnapToCellsMode AppointmentsSnapToCells { get { return AppointmentSnapToCellsMode.Always; } }
		protected internal override TimeScale CreateScale() {
			return new TimeScaleDay();
		}
		protected override int CalculateTop(Rectangle firstCellBounds, AppointmentIntermediateViewInfo intermediateResult) {
			return Painter.TopPadding + intermediateResult.RelativePosition;
		}
	}
	#endregion
	#region FullWeekViewTimeCellAppointmentLayoutCalculator
	public class FullWeekViewTimeCellAppointmentLayoutCalculator : DayViewTimeCellAppointmentLayoutCalculator {
		public FullWeekViewTimeCellAppointmentLayoutCalculator(FullWeekViewInfo viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
	}
	#endregion
	#region WorkWeekViewTimeCellAppointmentLayoutCalculator
	public class WorkWeekViewTimeCellAppointmentLayoutCalculator : DayViewTimeCellAppointmentLayoutCalculator {
		public WorkWeekViewTimeCellAppointmentLayoutCalculator(WorkWeekViewInfo viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
	}
	#endregion
	#region WorkWeekViewAllDayAppointmentFixedHeightLayoutCalculator
	public class WorkWeekViewAllDayAppointmentFixedHeightLayoutCalculator : DayViewAllDayAppointmentFixedHeightLayoutCalculator {
		public WorkWeekViewAllDayAppointmentFixedHeightLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
	}
	#endregion
	#region WorkWeekViewAllDayAppointmentAutoHeightLayoutCalculator
	public class WorkWeekViewAllDayAppointmentAutoHeightLayoutCalculator : DayViewAllDayAppointmentAutoHeightLayoutCalculator {
		public WorkWeekViewAllDayAppointmentAutoHeightLayoutCalculator(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
	}
	#endregion
	#region TimelineViewAppointmentFixedHeightLayoutCalculatorBase
	public abstract class TimelineViewAppointmentFixedHeightLayoutCalculatorBase : HorizontalAppointmentFixedHeightLayoutCalculator {
		protected TimelineViewAppointmentFixedHeightLayoutCalculatorBase(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new TimeLineAppointmentIntermediateLayoutCalculator(Scale, AppointmentsSnapToCells, ViewInfo.TimeZoneHelper);
		}
	}
	#endregion
	#region TimelineViewAppointmentAutoHeightLayoutCalculatorBase
	public abstract class TimelineViewAppointmentAutoHeightLayoutCalculatorBase : HorizontalAppointmentAutoHeightLayoutCalculator {
		protected TimelineViewAppointmentAutoHeightLayoutCalculatorBase(ISupportAppointments viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new TimeLineAppointmentIntermediateLayoutCalculator(Scale, AppointmentsSnapToCells, ViewInfo.TimeZoneHelper);
		}
	}
	#endregion
	#region TimelineViewAppointmentFixedHeightLayoutCalculator
	public class TimelineViewAppointmentFixedHeightLayoutCalculator : TimelineViewAppointmentFixedHeightLayoutCalculatorBase {
		public TimelineViewAppointmentFixedHeightLayoutCalculator(TimelineViewInfo viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal TimelineView View { get { return ((TimelineViewInfo)ViewInfo).View; } }
		protected internal override TimeScale CreateScale() {
			return View.GetBaseTimeScale();
		}
	}
	#endregion
	#region TimelineViewAppointmentAutoHeightLayoutCalculator
	public class TimelineViewAppointmentAutoHeightLayoutCalculator : TimelineViewAppointmentAutoHeightLayoutCalculatorBase {
		public TimelineViewAppointmentAutoHeightLayoutCalculator(TimelineViewInfo viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		TimelineView View { get { return ((TimelineViewInfo)ViewInfo).View; } }
		protected internal override TimeScale CreateScale() {
			return View.GetBaseTimeScale();
		}
	}
	#endregion
	#region GanttViewAppointmentFixedHeightLayoutCalculator
	public class GanttViewAppointmentFixedHeightLayoutCalculator : TimelineViewAppointmentFixedHeightLayoutCalculator {
		public GanttViewAppointmentFixedHeightLayoutCalculator(GanttViewInfo viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal override void CacheAppointmentViewInfosSkinElementInfos(AppointmentViewInfoCollection viewInfos) {
			base.CacheAppointmentViewInfosSkinElementInfos(viewInfos);
		}
	}
	#endregion
	#region GanttViewAppointmentAutoHeightLayoutCalculator
	public class GanttViewAppointmentAutoHeightLayoutCalculator : TimelineViewAppointmentAutoHeightLayoutCalculator {
		public GanttViewAppointmentAutoHeightLayoutCalculator(GanttViewInfo viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		protected internal override void CacheAppointmentViewInfosSkinElementInfos(AppointmentViewInfoCollection viewInfos) {
			base.CacheAppointmentViewInfosSkinElementInfos(viewInfos);
		}
	}
	#endregion
}

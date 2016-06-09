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
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Controls;
#if !SL
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#else
using DevExpress.Data;
#endif
namespace DevExpress.XtraScheduler.Native {
	public interface ITimelineViewProperties : ISchedulerViewPropertiesBase {
	}
	#region InnerTimelineView
	public class InnerTimelineView : InnerSchedulerViewBase {
		internal static readonly WorkTimeInterval defaultWorkTime = WorkTimeInterval.WorkTime;
		internal const bool defaultShowResourceHeaders = true;
		#region Fields
		TimeOfDayInterval workTime;
		TimeScaleCollection scales;
		TimeScaleCollection actualScales;
		NotificationCollectionChangedListener<TimeScale> timeScaleCollectionChangedListener;
		OptionsSelectionBehavior optionsSelectionBehavior;
		SelectionBarOptions selectionBar;
		CellsAutoHeightOptions cellsAutoHeightOptions;
		bool showResourceHeaders = defaultShowResourceHeaders;
		#endregion
		public InnerTimelineView(IInnerSchedulerViewOwner owner, ITimelineViewProperties properties)
			: base(owner, properties) {
			this.scales = CreateTimeScaleCollection();
			this.actualScales = CreateTimeScaleCollection();
			this.timeScaleCollectionChangedListener = CreateTimeScaleCollectionListener();
			this.optionsSelectionBehavior = new OptionsSelectionBehavior();
			this.selectionBar = CreateSelectionBarOptions();
			this.cellsAutoHeightOptions = new CellsAutoHeightOptions();
		}
		#region Properties
		[Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public CellsAutoHeightOptions CellsAutoHeightOptions { get { return cellsAutoHeightOptions; } }
		internal NotificationCollectionChangedListener<TimeScale> TimeScaleCollectionChangedListener { get { return timeScaleCollectionChangedListener; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Timeline; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override Keys Shortcut { get { return Keys.Control | Keys.Alt | Keys.D5; } }
		#region WorkTime
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public TimeOfDayInterval WorkTime {
			get { return workTime; }
			set {
				if (workTime.IsEqual(value))
					return;
				UnsubscribeWorkTimeEvents();
				workTime = value.Clone();
				SubscribeWorkTimeEvents();
				RaiseChanged(SchedulerControlChangeType.WorkTimeChanged); 
			}
		}
		internal bool ShouldSerializeWorkTime() {
			return !WorkTime.IsEqual(defaultWorkTime);
		}
		internal void ResetWorkTime() {
			WorkTime = defaultWorkTime;
		}
		internal bool XtraShouldSerializeWorkTime() {
			return ShouldSerializeWorkTime();
		}
		#endregion
		#region Scales
#if !SL
		[Editor("DevExpress.XtraScheduler.Design.TimeScaleCollectionEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor))]
#endif
		[Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public TimeScaleCollection Scales { get { return InnerScales; } }
		internal bool ShouldSerializeScales() {
			return !Scales.HasDefaultContent();
		}
		internal bool XtraShouldSerializeScales() {
			return ShouldSerializeScales();
		}
		internal void ResetScales() {
			Scales.LoadDefaults();
		}
		internal virtual void XtraClearScales(XtraItemEventArgs e) {
			scales.Clear();
		}
		internal virtual object XtraCreateScalesItem(XtraItemEventArgs e) {
			if (e.Item.ChildProperties == null)
				return null;
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo propertyInfo;
			propertyInfo = e.Item.ChildProperties["SerializationTypeName"];
			if (propertyInfo == null || propertyInfo.Value == null)
				return null;
			string typeName = propertyInfo.Value.ToString();
			if (String.IsNullOrEmpty(typeName))
				return null;
			Type type = TypeSerializationHelper.CreateTypeFromSerializationTypeName(typeName);
			if (type == null)
				return null;
			TimeScale scale = Activator.CreateInstance(type) as TimeScale;
			return scale;
		}
		internal void XtraSetIndexScalesItem(XtraSetItemIndexEventArgs e) {
			TimeScale scale = e.Item.Value as TimeScale;
			if (scale == null)
				return;
			Scales.Add(scale);
		}
		#endregion
		#region ShowResourceHeaders
		[DefaultValue(defaultShowResourceHeaders)]
		public bool ShowResourceHeaders {
			get { return showResourceHeaders; }
			set {
				if (showResourceHeaders == value)
					return;
				showResourceHeaders = value;
				RaiseChanged(SchedulerControlChangeType.ShowResourceHeadersChanged);
			}
		}
		#endregion
		protected internal TimeScaleCollection ActualScales { get { return actualScales; } }
		protected internal TimeScaleCollection InnerScales {
			get { return scales; }
			set {
				if (scales == value)
					return;
				SetInnerScales(value);
			}
		}
		protected internal SelectionBarOptions SelectionBar { get { return selectionBar; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToTimelineView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.ViewDisplayName_Timeline); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultShortDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.ViewShortDisplayName_Timeline); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_SwitchToTimelineView); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string Description { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_TimelineViewDescription); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal OptionsSelectionBehavior OptionsSelectionBehavior { get { return optionsSelectionBehavior; } }
		#endregion
		protected internal virtual SelectionBarOptions CreateSelectionBarOptions() {
			return new SelectionBarOptions();
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DayIntervalCollection days) {
			return days;
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DateTime visibleStart, DateTime visibleEnd) {
			TimeIntervalCollection result = CreateTimeIntervalCollection();
			result.Add(new TimeInterval(visibleStart, visibleEnd));
			return result;
		}
		public TimeScale GetBaseTimeScale() {
			int count = ActualScales.Count;
			XtraSchedulerDebug.Assert(count > 0);
			return ActualScales[count - 1];
		}
		protected internal virtual TimeScaleCollection CreateTimeScaleCollection() {
			return new TimeScaleCollection();
		}
		public override void Initialize(SchedulerViewSelection selection) {
			UpdateActualScales();
			base.Initialize(selection);
			workTime = WorkTimeInterval.WorkTime;
			SubscribeScalesEvents();
			SubscribeWorkTimeEvents();
			SubscribeSelectionBarEvents();
			SubscribeCellsAutoHeightOptionsEvents();
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (selectionBar != null) {
						UnsubscribeSelectionBarEvents();
						selectionBar = null;
					}
					if (timeScaleCollectionChangedListener != null) {
						UnsubscribeScalesEvents();
						timeScaleCollectionChangedListener.Dispose();
						timeScaleCollectionChangedListener = null;
					}
					if (scales != null) {
						scales = null;
					}
					if (workTime != null) {
						UnsubscribeWorkTimeEvents();
						workTime = null;
					}
					if (cellsAutoHeightOptions != null) {
						UnsubscribeCellsAutoHeightOptionsEvents();
						this.cellsAutoHeightOptions = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		#region SubscribeCellsAutoHeightOptionsEvents
		protected internal virtual void SubscribeCellsAutoHeightOptionsEvents() {
			CellsAutoHeightOptions.Changed += new BaseOptionChangedEventHandler(OnCellsAutoHeightOptionsChanged);
		}
		#endregion
		#region UnsubscribeCellsAutoHeightOptionsEvents
		protected internal virtual void UnsubscribeCellsAutoHeightOptionsEvents() {
			CellsAutoHeightOptions.Changed -= new BaseOptionChangedEventHandler(OnCellsAutoHeightOptionsChanged);
		}
		#endregion
		protected internal virtual void OnCellsAutoHeightOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			RaiseChanged(SchedulerControlChangeType.CellsAutoHeightChanged);
		}
		protected internal virtual void SubscribeSelectionBarEvents() {
			selectionBar.Changed += new BaseOptionChangedEventHandler(OnSelectionBarOptionsChanged);
		}
		protected internal virtual void UnsubscribeSelectionBarEvents() {
			selectionBar.Changed -= new BaseOptionChangedEventHandler(OnSelectionBarOptionsChanged);
		}
		protected internal virtual void OnSelectionBarOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			RaiseChanged(SchedulerControlChangeType.SelectionBarOptionsChanged);
		}
		protected internal virtual void SubscribeWorkTimeEvents() {
			workTime.Changed += new EventHandler(OnWorkTimeChanged);
		}
		protected internal virtual void UnsubscribeWorkTimeEvents() {
			workTime.Changed -= new EventHandler(OnWorkTimeChanged);
		}
		protected internal virtual void SubscribeScalesEvents() {
			timeScaleCollectionChangedListener.Changed += new EventHandler(OnScalesChanged);
		}
		protected internal virtual void UnsubscribeScalesEvents() {
			timeScaleCollectionChangedListener.Changed -= new EventHandler(OnScalesChanged);
		}
		protected internal virtual void OnScalesChanged(object sender, EventArgs e) {
			UpdateActualScales();
			DateTime oldStart = InnerVisibleIntervals.Start;
			RecreateTimeIntervalCollection();
			CreateVisibleIntervalsCore(oldStart);
			RaiseChanged(SchedulerControlChangeType.TimelineScalesChanged);
			RaiseUIChanged("Scales", null, null);
		}
		protected internal virtual void UpdateActualScales() {
			ActualScales.BeginUpdate();
			try {
				ActualScales.Clear();
				ActualScales.AddRange(TimeScaleCollectionHelper.SelectVisibleScales(Scales));
			} finally {
				ActualScales.EndUpdate();
			}
		}
		protected internal virtual void UpdateFilteredAppointments() {
			FilteredAppointments.BeginUpdate();
			try {
				FilteredAppointments.Clear();
				bool reloaded;
				FilteredAppointments.AddRange(Owner.GetFilteredAppointments(InnerVisibleIntervals.Interval, VisibleResources, out reloaded));
			} finally {
				FilteredAppointments.EndUpdate();
			}
		}
		protected internal virtual void OnWorkTimeChanged(object sender, EventArgs e) {
			RaiseChanged(SchedulerControlChangeType.WorkTimeChanged);
		}
		protected internal override TimeIntervalCollection CreateTimeIntervalCollection() {
			return new TimeScaleIntervalCollection(GetBaseTimeScale());
		}
		protected internal virtual void UpdateScalesFirstDayOfWeek(DayOfWeek firstDayOfWeek) {
			int count = Scales.Count;
			for (int i = 0; i < count; i++)
				Scales[i].SetFirstDayOfWeek(firstDayOfWeek);
		}
		protected internal override void CreateVisibleIntervalsCore(DateTime date) {
			PopulateVisibleIntervalsCore(date);
			UpdateScalesFirstDayOfWeek(Owner.FirstDayOfWeek);
		}
		protected internal virtual void PopulateVisibleIntervalsCore(DateTime date) {
			TimeScaleIntervalCollection collection = (TimeScaleIntervalCollection)InnerVisibleIntervals;
			InnerVisibleIntervals.Add(new TimeInterval(collection.Scale.Floor(date), TimeSpan.Zero));
		}
		protected internal override void SetStartCore(DateTime date, SchedulerViewSelection selection) {
			CreateVisibleIntervals(date, selection);
		}
		protected internal override TimeInterval RoundLimitInterval(TimeInterval interval) {
			TimeScaleIntervalCollection collection = (TimeScaleIntervalCollection)InnerVisibleIntervals;
			TimeScale scale = collection.Scale;
			DateTime start = scale.Floor(interval.Start);
			DateTime end = scale.Ceil(interval.End);
			return new TimeInterval(start, end);
		}
		protected internal override TimeInterval RoundSelectionInterval(TimeInterval interval) {
			TimeScaleIntervalCollection collection = (TimeScaleIntervalCollection)InnerVisibleIntervals;
			TimeScale scale = collection.Scale;
			DateTime start = scale.Floor(interval.Start);
			DateTime end = scale.Ceil(interval.End);
			return new TimeInterval(start, end);
		}
		protected internal override TimeInterval CreateDefaultSelectionInterval(DateTime date) {
			return CreateFirstSelectedInterval(date);
		}
		protected internal override ChangeActions SynchronizeSelectionInterval(SchedulerViewSelection selection, bool activeViewChanged) {
			return TryToAdjustSelectionToOldValue(selection);
		}
		protected internal override ChangeActions SynchronizeOrResetSelectionInterval(SchedulerViewSelection selection, bool activeViewChanged) {
			if (OptionsSelectionBehavior.UpdateSelectionDurationAction == UpdateSelectionDurationAction.Adjust)
				return TryToAdjustSelectionToOldValue(selection);
			return ResetSelection(selection);
		}
		protected internal virtual ChangeActions TryToAdjustSelectionToOldValue(SchedulerViewSelection selection) {
			selection.FirstSelectedInterval = CreateFirstSelectedInterval(selection.Interval.Start);
			return ValidateSelectionIntervalCore(selection);
		}
		protected internal virtual ChangeActions ResetSelection(SchedulerViewSelection selection) {
			TimeInterval newSelectionInterval = CreateDefaultSelectionInterval(selection.Interval.Start);
			ChangeActions result = newSelectionInterval.Equals(selection.Interval) ? ChangeActions.None : ChangeActions.RaiseSelectionChanged;
			selection.Interval = newSelectionInterval;
			TimeScaleIntervalCollection collection = (TimeScaleIntervalCollection)InnerVisibleIntervals;
			selection.FirstSelectedInterval = new TimeInterval(newSelectionInterval.Start, collection.Scale.GetNextDate(newSelectionInterval.Start));
			return result;
		}
		protected internal virtual TimeInterval CreateFirstSelectedInterval(DateTime date) {
			TimeScaleIntervalCollection collection = (TimeScaleIntervalCollection)InnerVisibleIntervals;
			TimeScale scale = collection.Scale;
			DateTime start = scale.Floor(date);
			return new TimeInterval(start, scale.GetNextDate(start));
		}
		protected internal override ChangeActions ValidateSelectionInterval(SchedulerViewSelection selection) {
			if (OptionsSelectionBehavior.UpdateSelectionDurationAction != UpdateSelectionDurationAction.Adjust)
				return ChangeActions.None;
			return ValidateSelectionIntervalCore(selection);
		}
		protected virtual ChangeActions ValidateSelectionIntervalCore(SchedulerViewSelection selection) {
			TimeScale scale = ((TimeScaleIntervalCollection)InnerVisibleIntervals).Scale;
			TimeInterval visibleInterval = InnerVisibleIntervals[0];
			DateTime newStart = scale.Floor(visibleInterval.Start);
			DateTime newEnd = scale.Ceil(visibleInterval.End);
			TimeInterval newSelectionInterval = new TimeInterval(newStart, newEnd);
			ChangeActions result = newSelectionInterval.Equals(selection.Interval) ? ChangeActions.None : ChangeActions.RaiseSelectionChanged;
			selection.Interval = newSelectionInterval;
			selection.FirstSelectedInterval = CreateFirstSelectedInterval(selection.Interval.Start);
			return result;
		}
		public override void Reset() {
			base.Reset();
			WorkTime = WorkTimeInterval.WorkTime;
			ResetScales();
		}
		protected internal override DateTime CalculateNewStartDateForward() {
			TimeIntervalCollection visibleIntervals = InnerVisibleIntervals;
			int count = visibleIntervals.Count;
			if (count <= 0)
				return visibleIntervals.Start;
			if (count == 1)
				return visibleIntervals[0].End;
			else {
				TimeScale scale = GetBaseTimeScale();
				return scale.GetNextDate(visibleIntervals[count - 1].Start);
			}
		}
		protected internal override DateTime CalculateNewStartDateBackward() {
			TimeIntervalCollection visibleIntervals = InnerVisibleIntervals;
			TimeScale scale = GetBaseTimeScale();
			int count = Math.Max(1, visibleIntervals.Count);
			DateTime date = visibleIntervals.Start;
			for (int i = 0; i < count; i++)
				date = scale.GetPrevDate(date);
			return date;
		}
		protected internal override TimeOfDayIntervalCollection CalcResourceWorkTimeInterval(TimeInterval interval, Resource resource) {
			return RaiseQueryWorkTime(interval, resource);
		}
		public override void ZoomIn() {
			TimeScaleCollection availableScales = TimeScaleCollectionHelper.CreateSortedAvailableScales(scales);
			if (!CanZoomInCore(availableScales))
				return;
			Scales.BeginUpdate();
			try {
				int count = ActualScales.Count;
				for (int i = count - 1; i >= 0; i--) {
					int index = availableScales.IndexOf(ActualScales[i]);
					availableScales[index].Enabled = false;
					availableScales[index + 1].Visible = availableScales[index].Visible;
					availableScales[index + 1].Enabled = true;
				}
			} finally {
				Scales.EndUpdate();
			}
		}
		protected internal override bool CanZoomIn() {
			return CanZoomInCore(TimeScaleCollectionHelper.CreateSortedAvailableScales(scales));
		}
		protected internal virtual bool CanZoomInCore(TimeScaleCollection availableScales) {
			int maxScaleIndex = availableScales.IndexOf(ActualScales[ActualScales.Count - 1]);
			if (maxScaleIndex < 0)
				return false;
			return maxScaleIndex < availableScales.Count - 1;
		}
		public override void ZoomOut() {
			TimeScaleCollection availableScales = TimeScaleCollectionHelper.CreateSortedAvailableScales(scales);
			if (!CanZoomOutCore(availableScales))
				return;
			Scales.BeginUpdate();
			try {
				int count = ActualScales.Count;
				for (int i = 0; i < count; i++) {
					int index = availableScales.IndexOf(ActualScales[i]);
					availableScales[index].Enabled = false;
					availableScales[index - 1].Visible = availableScales[index].Visible;
					availableScales[index - 1].Enabled = true;
				}
			} finally {
				Scales.EndUpdate();
			}
		}
		protected internal override bool CanZoomOut() {
			return CanZoomOutCore(TimeScaleCollectionHelper.CreateSortedAvailableScales(scales));
		}
		protected internal virtual bool CanZoomOutCore(TimeScaleCollection availableScales) {
			int minScaleIndex = availableScales.IndexOf(ActualScales[0]);
			return (minScaleIndex > 0);
		}
		protected internal override ChangeActions ApplyLimitIntervalToSelection(bool isNeedMoveSelection, SchedulerViewSelection selection, TimeSpan delta) {
			if (!isNeedMoveSelection)
				return ChangeActions.None;
			return base.ApplyLimitIntervalToSelection(isNeedMoveSelection, selection, delta);
		}
		protected internal virtual void SetInnerScales(TimeScaleCollection value) {
			UnsubscribeScalesEvents();
			this.scales = value;
			RecreateTimeScaleCollectionListener();
			SubscribeScalesEvents();
			OnScalesChanged(this, EventArgs.Empty);
		}
		protected internal virtual NotificationCollectionChangedListener<TimeScale> CreateTimeScaleCollectionListener() {
			return new NotificationCollectionChangedListener<TimeScale>(Scales);
		}
		protected internal virtual void RecreateTimeScaleCollectionListener() {
			if (this.timeScaleCollectionChangedListener != null)
				this.timeScaleCollectionChangedListener.Dispose();
			this.timeScaleCollectionChangedListener = CreateTimeScaleCollectionListener();
		}
		#region pseudo IXtraSupportDeserializeCollection Members
		protected internal virtual void BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			if (propertyName == "Scales")
				Scales.BeginUpdate();
		}
		protected internal virtual void ClearCollection(string propertyName, XtraItemEventArgs e) {
			if (propertyName == "Scales")
				Scales.Clear();
		}
		protected internal virtual void AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			if (propertyName == "Scales")
				Scales.EndUpdate();
		}
		#endregion
		protected internal override ResourceBaseCollection GetFilteredResources() {
			ResourceBaseCollection rootResources = Owner.GetResourcesTree();
			ResourceHierarchyHelper helper = new ResourceHierarchyHelper();
			return helper.GetExpandedResources(rootResources);
		}
	}
	#endregion
	#region TimelineWorkTimeCalculatorBase (abstract class)
	public abstract class TimelineWorkTimeCalculatorBase : WorkTimeInfoCalculatorBase {
		#region Fields
		WorkDaysCollection workDays;
		#endregion
		protected TimelineWorkTimeCalculatorBase(WorkDaysCollection workDays) {
			this.workDays = workDays;
		}
		#region Properties
		public override WorkDaysCollection WorkDays { get { return workDays; } }
		#endregion
	}
	#endregion
	#region TimelineWorkTimeCalculator
	public class TimelineWorkTimeCalculator : TimelineWorkTimeCalculatorBase {
		#region Fields
		InnerTimelineView innerView;
		#endregion
		public TimelineWorkTimeCalculator(InnerTimelineView innerView, WorkDaysCollection workDays)
			: base(workDays) {
			this.innerView = innerView;
		}
		#region Properties
		public InnerTimelineView InnerView { get { return innerView; } }
		protected internal override TimeOfDayInterval WorkTime {
			get { return InnerView.WorkTime; }
		}
		#endregion
		protected internal override TimeOfDayIntervalCollection CalcResourceWorkTimeInterval(TimeInterval interval, Resource resource) {
			return InnerView.CalcResourceWorkTimeInterval(interval, resource);
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Drawing {
	#region TimeScaleLevel
	public class TimeScaleLevel {
		TimeIntervalCollection intervals;
		TimeScale scale;
		public TimeScaleLevel(TimeScale scale) {
			this.scale = scale;
			this.intervals = new TimeIntervalCollection();
		}
		public TimeIntervalCollection Intervals { get { return intervals; } }
		public TimeScale Scale { get { return scale; } }
	}
	#endregion
	#region TimeScaleLevelCollection
	public class TimeScaleLevelCollection : List<TimeScaleLevel> {
	}
	#endregion
	#region TimeScaleLevelsCalculator
	public class TimeScaleLevelsCalculator {
		public TimeScaleLevelsCalculator() {
		}
		public virtual TimeScaleLevelCollection Calculate(TimeScaleCollection scales, DateTime start, int intervalCount) {
			TimeScaleLevelCollection levels = new TimeScaleLevelCollection();
			int count = scales.Count;
			if (count == 0)
				return levels;
			TimeScale baseScale = scales[count - 1];
			DateTime visibleStart = baseScale.Floor(start);
			TimeScaleLevel baseLevel = new TimeScaleLevel(baseScale);
			CalculateBaseLevelIntervals(baseLevel, visibleStart, intervalCount);
			levels.Add(baseLevel);
			for (int i = count - 1; i > 0; i--) {
				TimeScaleLevel upperLevel = new TimeScaleLevel(scales[i - 1]);
				CalculateUpperLevelIntervals(levels[0], upperLevel, visibleStart);
				levels.Insert(0, upperLevel);
			}
			return levels;
		}
		protected internal virtual void CalculateBaseLevelIntervals(TimeScaleLevel level, DateTime start, int intervalCount) {
			XtraSchedulerDebug.Assert(start == level.Scale.Floor(start));
			DateTime lastDate = start;
			TimeIntervalCollection levelIntervals = level.Intervals;
			levelIntervals.SuspendSort();
			for (int intervalIndex = 0; intervalIndex < intervalCount; intervalIndex++) {
				TimeInterval interval = CalculateScaleInterval(level.Scale, lastDate);
				levelIntervals.Add(interval);
				lastDate = interval.End;
			}
			levelIntervals.ResumeSort();
		}
		public TimeIntervalCollection CalculateBaseLevelIntervals(TimeScaleCollection scales, DateTime start, int intervalCount) {
			TimeIntervalCollection result = new TimeIntervalCollection(DXCollectionUniquenessProviderType.None);
			int count = scales.Count;
			TimeScale baseScale = scales[count - 1];
			XtraSchedulerDebug.Assert(start == baseScale.Floor(start));
			DateTime lastDate = start;
			result.SuspendSort();
			for (int intervalIndex = 0; intervalIndex < intervalCount; intervalIndex++) {
				TimeInterval interval = CalculateScaleInterval(baseScale, lastDate);
				result.Add(interval);
				lastDate = interval.End;
			}
			result.ResumeSort();
			return result;
		}
		protected internal virtual void CalculateUpperLevelIntervals(TimeScaleLevel baseLevel, TimeScaleLevel upperLevel, DateTime start) {
			TimeScale scale = upperLevel.Scale;
			DateTime endofInterval = scale.GetNextDate(scale.Floor(start));
			DateTime startOfInterval = scale.GetPrevDate(endofInterval);
			TimeInterval interval = new TimeInterval(startOfInterval, endofInterval);
			int count = baseLevel.Intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval baseInterval = baseLevel.Intervals[i];
				if (baseInterval.IntersectsWithExcludingBounds(interval) && !upperLevel.Intervals.Contains(interval)) {
					upperLevel.Intervals.Add(interval);
					interval = CalculateScaleInterval(scale, interval.End);
				}
			}
		}
		protected TimeInterval CalculateScaleInterval(TimeScale scale, DateTime start) {
			return new TimeInterval(start, scale.GetNextDate(start));
		}
	}
	#endregion
}

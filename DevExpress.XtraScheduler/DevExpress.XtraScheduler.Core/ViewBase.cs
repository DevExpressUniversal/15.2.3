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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Internal.Implementations;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#else
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Data;
#endif
namespace DevExpress.XtraScheduler {
	#region SchedulerViewType
	public enum SchedulerViewType {
		Day = 0,
		Week = 1,
		Month = 2,
		WorkWeek = 3,
		Timeline = 4,
		Gantt = 5,
		FullWeek = 6
	}
	#endregion
	#region SchedulerGroupType
	public enum SchedulerGroupType {
		None = 0,
		Date = 1,
		Resource = 2
	}
	#endregion
	#region AppointmentTimeDisplayType
	public enum AppointmentTimeDisplayType {
		Auto = 0,
		Clock = 1,
		Text = 2
	}
	#endregion
	#region AppointmentTimeVisibility
	public enum AppointmentTimeVisibility {
		Auto = 0,
		Always = 1,
		Never = 2
	}
	#endregion
	#region NavigationButtonVisibility
	public enum NavigationButtonVisibility {
		Auto = 0,
		Always = 1,
		Never = 2
	}
	#endregion
	#region SchedulerMenuItemId
	public enum SchedulerMenuItemId {
		Custom = -1,
		OpenAppointment = 0,
		PrintAppointment = 1,
		DeleteAppointment = 2,
		EditSeries = 3,
		NewAppointment = 4,
		NewAllDayEvent = 5,
		NewRecurringAppointment = 6,
		NewRecurringEvent = 7,
		GotoThisDay = 8,
		GotoToday = 9,
		GotoDate = 10,
		OtherSettings = 11,
		CustomizeCurrentView = 12,
		CustomizeTimeRuler = 13,
		AppointmentDragMove = 14,
		AppointmentDragCopy = 15,
		AppointmentDragCancel = 16,
		StatusSubMenu = 17,
		LabelSubMenu = 18,
		RulerMenu = 19,
		AppointmentMenu = 20,
		DefaultMenu = 21,
		AppointmentDragMenu = 22,
		RestoreOccurrence = 23,
		SwitchViewMenu = 24,
		SwitchToDayView = 25,
		SwitchToWorkWeekView = 26,
		SwitchToWeekView = 27,
		SwitchToMonthView = 28,
		SwitchToTimelineView = 29,
		TimeScaleEnable = 30,
		TimeScaleVisible = 31,
		SwitchTimeScale = 32,
		SplitAppointment = 33,
		SwitchToGroupByNone = 34,
		SwitchToGroupByDate = 35,
		SwitchToGroupByResource = 36,
		SwitchToGanttView = 37,
		AppointmentDependencyCreation = 38,
		CollapseResource = 39,
		ExpandResource = 40,
		OpenAppointmentDependency = 41,
		AppointmentDependencyMenu = 42,
		DeleteAppointmentDependency = 43,
		PrintPreview = 44,
		Print = 45,
		PrintPageSetup = 46,
		SwitchTimeScalesTo = 47,
		SwitchTimeScalesTo5MinutesSlot = 48,
		SwitchTimeScalesTo6MinutesSlot = 49,
		SwitchTimeScalesTo10MinutesSlot = 50,
		SwitchTimeScalesTo15MinutesSlot = 51,
		SwitchTimeScalesTo30MinutesSlot = 52,
		SwitchTimeScalesTo60MinutesSlot = 53,
		SwitchShowWorkTimeOnly = 54,
		SwitchCompressWeekend = 55,
		SwitchCellsAutoHeight = 56,
		ToggleRecurrence = 57,
		ChangeAppointmentStatusUI = 58,
		ChangeAppointmentLabelUI = 59,
		ChangeAppointmentReminderUI = 60,
		ChangeTimelineScaleWidthUI = 61,
		OpenSchedule = 62,
		SaveSchedule = 63,
		ChangeSnapToCellsUI = 64,
		EditOccurrenceUI = 65,
		EditSeriesUI = 66,
		DeleteSeriesUI = 67,
		DeleteOccurrenceUI = 68,
		SetSnapToCells = 69,
		SwitchToFullWeekView = 70
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	public interface ISchedulerPropertiesBase : INotifyPropertyChanging, INotifyPropertyChanged {
	}
	public interface ISchedulerViewPropertiesBase : ISchedulerPropertiesBase {
		bool Enabled { get; set; }
		SchedulerGroupType GroupType { get; set; }
		ISchedulerDeferredScrollingOption DeferredScrolling { get; set; }
	}
	public class SchedulerPropertiesBase : ISchedulerPropertiesBase, IDisposable {
		bool isDisposed;
		internal bool IsDisposed { get { return isDisposed; } }
		#region INotifyPropertyChanged Members
		#region PropertyChanging
		PropertyChangingEventHandler onPropertyChanging;
		public event PropertyChangingEventHandler PropertyChanging { add { onPropertyChanging += value; } remove { onPropertyChanging -= value; } }
		protected internal virtual bool RaisePropertyChanging(string propertyName) {
			if (onPropertyChanging != null) {
				PropertyChangingEventArgsEx args = new PropertyChangingEventArgsEx(propertyName);
				onPropertyChanging(this, args);
				return !args.Cancel;
			}
			else
				return true;
		}
		#endregion
		#region PropertyChanged
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected internal virtual void RaisePropertyChanged<T>(string propertyName, T oldValue, T newValue) {
			if (onPropertyChanged != null) {
				PropertyChangedEventArgsEx args = new PropertyChangedEventArgsEx(propertyName, oldValue, newValue);
				onPropertyChanged(this, args);
			}
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerPropertiesBase() {
			Dispose(false);
		}
		#endregion
	}
	public class PropertyChangingEventArgsEx : PropertyChangingEventArgs {
		bool cancel;
		public PropertyChangingEventArgsEx(string propertyName)
			: base(propertyName) {
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class PropertyChangedEventArgsEx : PropertyChangedEventArgs {
		readonly object oldValue;
		readonly object newValue;
		public PropertyChangedEventArgsEx(string propertyName, object oldValue, object newValue)
			: base(propertyName) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public object OldValue { get { return oldValue; } }
		public object NewValue { get { return newValue; } }
	}
	#region ISchedulerViewRepositoryItem
	public interface ISchedulerViewRepositoryItem : IDisposable {
		SchedulerViewType Type { get; }
		SchedulerGroupType GroupType { get; set; }
		InnerSchedulerViewBase InnerView { get; }
		void Initialize(InnerSchedulerControl control);
		void Reset();
	}
	#endregion
	#region IInnerSchedulerViewOwner
	public interface IInnerSchedulerViewOwner {
		WorkDaysCollection WorkDays { get; }
		DayOfWeek FirstDayOfWeek { get; }
		string ClientTimeZoneId { get; }
		AppointmentDisplayOptions CreateAppointmentDisplayOptions();
		ResourceBaseCollection GetFilteredResources();
		AppointmentBaseCollection GetFilteredAppointments(TimeInterval interval, ResourceBaseCollection resources, out bool appointmentsAreReloaded);
		AppointmentBaseCollection GetNonFilteredAppointments();
		void UpdateSelection(SchedulerViewSelection selection);
		ResourceBaseCollection GetResourcesTree();
	}
	#endregion
	#region InnerSchedulerViewBase
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public abstract class InnerSchedulerViewBase : UserInterfaceObject, IDisposable {
		#region Fields
		internal const bool defaultShowMoreButtons = true;
		internal const NavigationButtonVisibility defaultNavigationButtonVisibility = NavigationButtonVisibility.Auto;
		internal static readonly TimeSpan defaultNavigationButtonAppointmentSearchInterval = TimeSpan.Zero;
		IInnerSchedulerViewOwner owner;
		TimeInterval limitInterval;
		TimeIntervalCollection visibleIntervals;
		ResourceBaseCollection filteredResources;
		AppointmentBaseCollection filteredAppointments;
		ResourceBaseCollection visibleResources;
		int firstVisibleResourceIndex;
		int resourcesPerPage;
		AppointmentDisplayOptions appointmentDisplayOptions;
		bool showMoreButtons = defaultShowMoreButtons;
		NavigationButtonVisibility navigationButtonVisibility = defaultNavigationButtonVisibility;
		string shortDisplayName = String.Empty;
		TimeSpan navigationButtonAppointmentSearchInterval = defaultNavigationButtonAppointmentSearchInterval;
		bool isDisposed;
		string displayName = string.Empty;
		string menuCaption = string.Empty;
		ISchedulerViewPropertiesBase properties;
		#endregion
		protected InnerSchedulerViewBase(IInnerSchedulerViewOwner owner, ISchedulerViewPropertiesBase properties)
			: base(null, string.Empty) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNull(properties, "properties");
			this.owner = owner;
			this.properties = properties;
			this.appointmentDisplayOptions = owner.CreateAppointmentDisplayOptions();
			ResetDisplayName();
			ResetMenuCaption();
			SubscribePropertiesEvents();
			ExtendedVisibleInterval = TimeInterval.Empty;
		}
		#region Properties
		internal IInnerSchedulerViewOwner Owner { get { return owner; } }
		public abstract SchedulerViewType Type { get; }
		public TimeInterval ExtendedVisibleInterval { get; set; } 
		protected internal abstract Keys Shortcut { get; }
		internal TimeIntervalCollection InnerVisibleIntervals { get { return visibleIntervals; } }
		#region Enabled
		[DefaultValue(true)]
		public virtual bool Enabled { get { return Properties.Enabled; } set { Properties.Enabled = value; } }
		#endregion
		#region GroupType
		[DefaultValue(SchedulerGroupType.None), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerGroupType GroupType { get { return Properties.GroupType; } set { Properties.GroupType = value; } }
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ResourceBaseCollection FilteredResources { get { return filteredResources; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal AppointmentBaseCollection FilteredAppointments { get { return filteredAppointments; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ResourceBaseCollection VisibleResources { get { return visibleResources; } }
		protected internal DateTime VisibleStart { get { return InnerVisibleIntervals.Start; } }
		#region FirstVisibleResourceIndex
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FirstVisibleResourceIndex {
			get { return firstVisibleResourceIndex; }
			set {
				int oldValue = firstVisibleResourceIndex;
				if (oldValue == value)
					return;
				if (value < 0)
					Exceptions.ThrowArgumentException("FirstVisibleResourceIndex", value);
				SetFirstVisibleResourceIndexCore(value);
				RaiseChanged(SchedulerControlChangeType.FirstVisibleResourceIndexChanged, oldValue, value);
			}
		}
		#endregion
		#region ResourcesPerPage
		[DefaultValue(0), XtraSerializableProperty()]
		public int ResourcesPerPage {
			get { return resourcesPerPage; }
			set {
				int oldValue = resourcesPerPage;
				if (oldValue == value)
					return;
				if (value < 0)
					Exceptions.ThrowArgumentException("ResourcesPerPage", value);
				SetResourcesPerPageCore(value);
				RaiseChanged(SchedulerControlChangeType.ResourcesPerPageChanged, oldValue, value);
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal int ActualFirstVisibleResourceIndex {
			get { return Math.Min(FirstVisibleResourceIndex, FilteredResources.Count - ActualResourcesPerPage); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal int ActualResourcesPerPage {
			get {
				int resourceCount = FilteredResources.Count;
				int result = Math.Min(ResourcesPerPage, resourceCount);
				if (result == 0)
					result = resourceCount;
				return result;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual bool CanShowResources() {
			return FilteredResources.Count > 0;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal TimeInterval LimitInterval {
			get { return limitInterval; }
			set {
				if (Object.Equals(limitInterval, value))
					return;
				limitInterval = RoundLimitInterval(value);
			}
		}
		[Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public AppointmentDisplayOptions AppointmentDisplayOptions { get { return appointmentDisplayOptions; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ISchedulerDeferredScrollingOption DeferredScrolling { get { return Properties.DeferredScrolling; } }
		#region ShowMoreButtons
		[DefaultValue(defaultShowMoreButtons), XtraSerializableProperty()]
		public bool ShowMoreButtons {
			get { return showMoreButtons; }
			set {
				if (showMoreButtons == value)
					return;
				this.showMoreButtons = value;
				RaiseChanged(SchedulerControlChangeType.ShowMoreButtonsChanged);
			}
		}
		#endregion
		#region NavigationButtonVisibility
		[DefaultValue(defaultNavigationButtonVisibility), XtraSerializableProperty()]
		public NavigationButtonVisibility NavigationButtonVisibility {
			get { return navigationButtonVisibility; }
			set {
				if (NavigationButtonVisibility == value)
					return;
				this.navigationButtonVisibility = value;
				RaiseChanged(SchedulerControlChangeType.NavigationButtonVisibilityChanged);
			}
		}
		#endregion
		#region NavigationButtonAppointmentSearchInterval
		[XtraSerializableProperty()]
		public TimeSpan NavigationButtonAppointmentSearchInterval {
			get { return navigationButtonAppointmentSearchInterval; }
			set {
				if (NavigationButtonAppointmentSearchInterval == value)
					return;
				this.navigationButtonAppointmentSearchInterval = value;
				RaiseChanged(SchedulerControlChangeType.NavigationButtonAppointmentSearchIntervalChanged);
			}
		}
		protected internal virtual bool ShouldSerializeNavigationButtonAppointmentSearchInterval() {
			return NavigationButtonAppointmentSearchInterval != defaultNavigationButtonAppointmentSearchInterval;
		}
		protected internal virtual void ResetNavigationButtonAppointmentSearchInterval() {
			NavigationButtonAppointmentSearchInterval = defaultNavigationButtonAppointmentSearchInterval;
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract SchedulerMenuItemId MenuItemId { get; }
		protected internal virtual bool HeaderAlternateEnabled { get { return true; } }
		#region IUserInterfaceObject implementation
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object Id {
			get { return Type; }
		}
		#region DisplayName
		public override string DisplayName {
			get { return displayName; }
			set {
				if (value == null)
					value = String.Empty;
				if (displayName != value) {
					string oldValue = displayName;
					displayName = value;
					RaiseUIChanged("DisplayName", oldValue, value);
				}
			}
		}
		protected internal override bool ShouldSerializeDisplayName() {
			return DisplayName != DefaultDisplayName;
		}
		#endregion
		#region MenuCaption
		public override string MenuCaption {
			get { return menuCaption; }
			set {
				if ( value == null )
					value = String.Empty;
				if ( menuCaption != value ) {
					string oldValue = menuCaption;
					menuCaption = value;
					RaiseUIChanged("MenuCaption", oldValue, value);
				}
			}
		}
		protected internal override bool ShouldSerializeMenuCaption() {
			return MenuCaption != DefaultMenuCaption;
		}
		#endregion
		#endregion
		#region ShortDisplayName
		[NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true), AutoFormatDisable()]
		public string ShortDisplayName {
			get { return shortDisplayName; }
			set {
				if (value == null)
					value = String.Empty;
				if (shortDisplayName != value) {
					string oldValue = shortDisplayName;
					shortDisplayName = value;
					RaiseUIChanged("ShortDisplayName", oldValue, value);
				}
			}
		}
		protected internal bool ShouldSerializeShortDisplayName() {
			return ShortDisplayName != DefaultShortDisplayName;
		}
		protected internal virtual bool XtraShouldSerializeShortDisplayName() {
			return ShouldSerializeShortDisplayName();
		}
		protected internal virtual void ResetShortDisplayName() {
			ShortDisplayName = DefaultShortDisplayName;
		}
		protected internal abstract string DefaultShortDisplayName { get; }
		#endregion
		protected internal abstract string Description { get; }
		protected ISchedulerViewPropertiesBase Properties { get { return properties; } }
		#endregion
		protected internal abstract TimeIntervalCollection CreateTimeIntervalCollection();
		protected internal abstract void CreateVisibleIntervalsCore(DateTime date);
		protected internal abstract TimeInterval RoundLimitInterval(TimeInterval interval);
		protected internal abstract TimeInterval RoundSelectionInterval(TimeInterval interval);
		protected internal abstract TimeInterval CreateDefaultSelectionInterval(DateTime date);
		public virtual void Initialize(SchedulerViewSelection selection) {
			this.limitInterval = new TimeInterval(DateTime.MinValue, DateTime.MaxValue);
			RecreateTimeIntervalCollection();
			ResetDisplayName();
			ResetMenuCaption();
			ResetShortDisplayName();
			this.filteredResources = new ResourceBaseCollection();
			this.filteredAppointments = new AppointmentBaseCollection();
			this.visibleResources = new ResourceBaseCollection();
			AppointmentDisplayOptions.Reset(); 
			CreateVisibleIntervals(DateTime.Now, selection);
			SubscribeAppointmentDisplayOptionsEvents();
		}
		protected internal virtual ChangeActions CreateVisibleIntervals(DateTime date, SchedulerViewSelection selection) {
			InnerVisibleIntervals.BeginUpdate();
			try {
				InnerVisibleIntervals.Clear();
				CreateVisibleIntervalsCore(date);
				return ChangeActions.RaiseVisibleIntervalChanged | ApplyLimitInterval(selection);
			}
			finally {
				InnerVisibleIntervals.EndUpdate();
			}
		}
		protected internal virtual void RecreateTimeIntervalCollection() {
			this.visibleIntervals = CreateTimeIntervalCollection();
		}
		protected internal virtual ChangeActions ApplyLimitInterval(SchedulerViewSelection selection) {
			TimeInterval interval = new TimeInterval(InnerVisibleIntervals.Start, InnerVisibleIntervals.End);
			if (LimitInterval.Contains(interval))
				return ChangeActions.None; 
			if (interval.Contains(LimitInterval))
				return ChangeActions.None; 
			ChangeActions result = ChangeActions.None;
			if (interval.Start < LimitInterval.Start) {
				TimeSpan delta = LimitInterval.Start - interval.Start;
				InnerVisibleIntervals.Shift(delta);
				bool isNeedMoveSelection = (selection.Interval.Start < LimitInterval.Start);
				result |= ApplyLimitIntervalToSelection(isNeedMoveSelection, selection, delta);
				result |= ChangeActions.RaiseVisibleIntervalChanged;
			}
			if (interval.End > LimitInterval.End) {
				bool isNeedMoveSelection = (selection.Interval.Start > LimitInterval.End);
				TimeSpan delta;
				delta = LimitInterval.End - interval.End;
				if (interval.Start.Ticks + delta.Ticks < LimitInterval.Start.Ticks) {
					delta = LimitInterval.Start - interval.Start;
					InnerVisibleIntervals.Shift(delta);
					result |= ApplyLimitIntervalToSelection(isNeedMoveSelection, selection, delta);
				}
				else {
					InnerVisibleIntervals.Shift(delta);
					result |= ApplyLimitIntervalToSelection(isNeedMoveSelection, selection, delta);
				}
				result |= ChangeActions.RaiseVisibleIntervalChanged;
			}
			return result;
		}
		protected internal virtual ChangeActions ApplyLimitIntervalToSelection(bool isNeedMoveSelection, SchedulerViewSelection selection, TimeSpan delta) {
			selection.Interval.Start += delta;
			selection.FirstSelectedInterval.Start += delta;
			return ChangeActions.RaiseVisibleIntervalChanged;
		}
		protected internal virtual void SetStart(DateTime date, SchedulerViewSelection selection) {
			CreateVisibleIntervals(date, selection);
		}
		protected internal virtual void SetStartCore(DateTime date, SchedulerViewSelection selection) {
			DateTime oldVisibleStart = VisibleStart;
			CreateVisibleIntervals(date, selection);
			TimeSpan delta = VisibleStart - oldVisibleStart;
			selection.Interval.Start += delta;
			selection.FirstSelectedInterval.Start += delta;
		}
		protected internal virtual void SetFirstVisibleResourceIndexCore(int value) {
			firstVisibleResourceIndex = Math.Max(0, value);
		}
		protected internal virtual void SetResourcesPerPageCore(int value) {
			resourcesPerPage = value;
			if (resourcesPerPage == 0)
				SetFirstVisibleResourceIndexCore(0);
		}
		#region Events
		#region EnabledChanging
		CancelEventHandler onEnabledChanging;
		internal event CancelEventHandler EnabledChanging { add { onEnabledChanging += value; } remove { onEnabledChanging -= value; } }
		protected internal virtual bool RaiseEnabledChanging() {
			if (onEnabledChanging != null) {
				CancelEventArgs args = new CancelEventArgs();
				onEnabledChanging(this, args);
				return !args.Cancel;
			}
			return true;
		}
		#endregion
		#region Changed
		SchedulerControlStateChangedEventHandler onChanged;
		internal event SchedulerControlStateChangedEventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged(SchedulerControlChangeType changeType) {
			RaiseChanged(changeType, false);
		}
		protected internal virtual void RaiseChanged(SchedulerControlChangeType changeType, object oldValue, object newValue) {
			RaiseChanged(changeType, false, oldValue, newValue);
		}
		protected internal virtual void RaiseChanged(SchedulerControlChangeType changeType, bool ignoreApplyChange) {
			RaiseChanged(changeType, ignoreApplyChange, null, null);
		}
		protected internal virtual void RaiseChanged(SchedulerControlChangeType changeType, bool ignoreApplyChange, object oldValue, object newValue) {
			if (onChanged != null) {
				SchedulerControlStateChangedEventArgs args = new SchedulerControlStateChangedEventArgs(changeType, oldValue, newValue);
				args.IgnoreApplyChanges = ignoreApplyChange;
				onChanged(this, args);
			}
		}
		#endregion
		#region LowLevelChanged
		SchedulerControlLowLevelStateChangedEventHandler onLowLevelChanged;
		internal event SchedulerControlLowLevelStateChangedEventHandler LowLevelChanged { add { onLowLevelChanged += value; } remove { onLowLevelChanged -= value; } }
		protected internal virtual void RaiseLowLevelChanged(SchedulerControlChangeType changeType, ChangeActions actions) {
			if (onLowLevelChanged != null) {
				SchedulerControlLowLevelStateChangedEventArgs args = new SchedulerControlLowLevelStateChangedEventArgs(changeType, actions);
				onLowLevelChanged(this, args);
			}
		}
		#endregion
		#region UIChanged
		SchedulerViewUIChangedEventHandler onUIChanged;
		internal event SchedulerViewUIChangedEventHandler UIChanged { add { onUIChanged += value; } remove { onUIChanged -= value; } }
		protected internal virtual void RaiseUIChanged(string propertyName, object oldValue, object newValue) {
			if (onUIChanged != null) {
				SchedulerViewUIChangedEventArgs args = new SchedulerViewUIChangedEventArgs(Type, propertyName, oldValue, newValue);
				onUIChanged(this, args);
			}
		}
		#endregion
		#region GroupTypeChanged
		EventHandler onGroupTypeChanged;
		internal event EventHandler GroupTypeChanged {
			add { onGroupTypeChanged += value; }
			remove { onGroupTypeChanged -= value; }
		}
		protected internal virtual void RaiseGroupTypeChanged() {
			if (onGroupTypeChanged != null)
				onGroupTypeChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (visibleIntervals != null)
					this.visibleIntervals.Clear();
				if (filteredResources != null)
					this.filteredResources.Clear();
				if (filteredAppointments != null)
					this.filteredAppointments.Clear();
				if (visibleResources != null)
					this.visibleResources.Clear();
				this.limitInterval = null;
				if (appointmentDisplayOptions != null) {
					UnsubscribeAppointmentDisplayOptionsEvents();
					this.appointmentDisplayOptions = null;
				}
				this.owner = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		internal bool IsDisposed { get { return isDisposed; } }
		#endregion
		#region SubscribeAppointmentDisplayOptionsEvents
		protected internal virtual void SubscribeAppointmentDisplayOptionsEvents() {
			appointmentDisplayOptions.Changed += new BaseOptionChangedEventHandler(OnAppointmentDisplayOptionsChanged);
		}
		#endregion
		#region UnsubscribeAppointmentDisplayOptionsEvents
		protected internal virtual void UnsubscribeAppointmentDisplayOptionsEvents() {
			appointmentDisplayOptions.Changed -= new BaseOptionChangedEventHandler(OnAppointmentDisplayOptionsChanged);
		}
		#endregion
		protected internal virtual void OnAppointmentDisplayOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			RaiseChanged(SchedulerControlChangeType.AppointmentDisplayOptionsChanged);
		}
		#region QueryVisibleResources
		protected internal virtual ChangeActions QueryVisibleResources() {
			this.VisibleResources.Clear();
			if (GroupType == SchedulerGroupType.None || ResourcesPerPage <= 0)
				this.VisibleResources.AddRange(FilteredResources);
			else {
				XtraSchedulerDebug.Assert(ActualFirstVisibleResourceIndex >= 0);
				XtraSchedulerDebug.Assert(ActualFirstVisibleResourceIndex + ActualResourcesPerPage <= FilteredResources.Count);
				int startIndex = ActualFirstVisibleResourceIndex;
				int maxIndex = startIndex + ActualResourcesPerPage;
				this.VisibleResources.BeginUpdate();
				try {
					for (int i = startIndex; i < maxIndex; i++)
						this.VisibleResources.Add(FilteredResources[i]);
				}
				finally {
					this.VisibleResources.EndUpdate();					
				}
			}
			return ChangeActions.ValidateSelectionResource;
		}
		#endregion
		protected internal virtual ChangeActions QueryResources() {
			int oldResourceCount = this.FilteredResources.Count;
			this.filteredResources = GetFilteredResources();
			QueryVisibleResources();
			if (oldResourceCount != this.FilteredResources.Count)
				return ChangeActions.ValidateSelectionResource | ChangeActions.UpdateResourceScrollBarValue | ChangeActions.MayChangeResourceScrollBarOrientation | ChangeActions.MayChangeDateTimeScrollBarOrientation | ChangeActions.MayChangeResourceScrollBarVisibility | ChangeActions.MayChangeDateTimeScrollBarVisibility;
			else
				return ChangeActions.ValidateSelectionResource;
		}
		protected internal virtual ResourceBaseCollection GetFilteredResources() {
			return owner.GetFilteredResources();
		}
		protected internal virtual ChangeActions QueryAppointments() {
			TimeInterval queryInterval = InnerVisibleIntervals.Interval;
			if (!TimeInterval.Empty.Equals(ExtendedVisibleInterval))
				queryInterval = queryInterval.Merge(ExtendedVisibleInterval);
			TimeInterval adjustedQueryInterval = ApplyVisibleTime(queryInterval);
			bool reloaded;
			this.filteredAppointments = owner.GetFilteredAppointments(adjustedQueryInterval, VisibleResources, out reloaded);
			if (reloaded)
				return ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout;
			else
				return ChangeActions.None;
		}
		protected internal virtual TimeInterval ApplyVisibleTime(TimeInterval interval) {
			return new TimeInterval(interval.Start, interval.Duration);
		}
		protected internal virtual ChangeActions UpdateVisibleIntervals(SchedulerViewSelection selection) {
			return CreateVisibleIntervals(VisibleStart, selection);
		}
		protected internal virtual void SynchronizeVisibleStart(DateTime previousViewStart, SchedulerViewSelection selection) {
			CreateVisibleIntervals(previousViewStart, selection);
		}
		protected internal virtual ChangeActions SynchronizeSelectionResource(SchedulerViewSelection selection) {
			ViewSelectionResourceSynchronizer synchronizer = new ViewSelectionResourceSynchronizer(this);
			return synchronizer.Synchronize(selection);
		}
		protected internal virtual ChangeActions ValidateSelectionResource(SchedulerViewSelection selection) {
			ViewSelectionResourceValidator validator = new ViewSelectionResourceValidator(this);
			return validator.Synchronize(selection);
		}
		protected internal abstract ChangeActions SynchronizeSelectionInterval(SchedulerViewSelection selection, bool activeViewChanged);
		protected internal abstract ChangeActions ValidateSelectionInterval(SchedulerViewSelection selection);
		protected internal abstract TimeIntervalCollection CreateValidIntervals(DayIntervalCollection days);
		protected internal abstract TimeIntervalCollection CreateValidIntervals(DateTime visibleStart, DateTime visibleEnd);
		protected internal virtual ChangeActions SynchronizeOrResetSelectionInterval(SchedulerViewSelection selection, bool activeViewChanged) {
			return SynchronizeSelectionInterval(selection, activeViewChanged);
		}
		protected internal virtual ChangeActions InitializeSelection(SchedulerViewSelection selection) {
			return InitializeSelectionCore(selection, InnerVisibleIntervals.Start);
		}
		protected internal virtual ChangeActions InitializeSelectionCore(SchedulerViewSelection selection, DateTime date) {
			TimeInterval newSelectionInterval = CreateDefaultSelectionInterval(date);
			ChangeActions result = newSelectionInterval.Equals(selection.Interval) ? ChangeActions.None : ChangeActions.RaiseSelectionChanged | ChangeActions.RecalcViewLayout;
			selection.Interval = newSelectionInterval;
			selection.FirstSelectedInterval = newSelectionInterval.Clone();
			return result;
		}
		protected internal virtual TimeIntervalCollection GetVisibleIntervals() {
			return visibleIntervals.Clone();
		}
		protected internal virtual ChangeActions SetVisibleIntervals(TimeIntervalCollection intervals, SchedulerViewSelection selection) {
			ChangeActions actions = ChangeActionsCalculator.CalculateChangeActions(SchedulerControlChangeType.VisibleIntervalsChanged);
			visibleIntervals.BeginUpdate();
			try {
				actions |= SetVisibleIntervalsCore(intervals);
				actions |= ApplyLimitInterval(selection);
			}
			finally {
				visibleIntervals.EndUpdate();
			}
			RaiseLowLevelChanged(SchedulerControlChangeType.VisibleIntervalsChanged, actions);
			return actions;
		}
		protected internal virtual ChangeActions ApplyVisibleInterval(int oldVisibleIntervalsCount) {
			return ChangeActions.None;		
		}
		protected internal virtual ChangeActions SetVisibleIntervalsCore(TimeIntervalCollection intervals) {
			visibleIntervals.Clear();
			visibleIntervals.AssignProperties(intervals);
			visibleIntervals.AddRange(intervals);
			return ChangeActions.None;
		}
		protected internal virtual ChangeActions SetVisibleDays(DayIntervalCollection days, SchedulerViewSelection selection) {
			TimeIntervalCollection newVisibleIntervals = CreateValidIntervals(days);
			return SetVisibleIntervals(newVisibleIntervals, selection);
		}
		protected internal virtual ChangeActions SetVisibleInterval(DateTime start, DateTime end, SchedulerViewSelection selection) {
			TimeIntervalCollection newVisibleIntervals = CreateValidIntervals(start, end);
			return SetVisibleIntervals(newVisibleIntervals, selection);
		}
		public virtual void Reset() {
			GroupType = SchedulerGroupType.None;
			ShowMoreButtons = defaultShowMoreButtons;
			NavigationButtonVisibility = defaultNavigationButtonVisibility;
			appointmentDisplayOptions.Reset();
		}
		protected internal virtual void UpdateSelection(SchedulerViewSelection selection) {
			Owner.UpdateSelection(selection);
		}
		protected internal virtual DateTime CalculateNewStartDateForward() {
			return CalculateNewStartDateCore(1);
		}
		protected internal virtual DateTime CalculateNewStartDateBackward() {
			return CalculateNewStartDateCore(-1);
		}
		protected internal virtual DateTime CalculateNewStartDateCore(int direction) {
			TimeIntervalCollection visibleIntervals = InnerVisibleIntervals;
			int count = visibleIntervals.Count;
			if (count <= 0)
				return visibleIntervals.Start;
			return visibleIntervals.Start + TimeSpan.FromTicks(direction * visibleIntervals[0].Duration.Ticks * count);
		}
		protected internal abstract TimeOfDayIntervalCollection CalcResourceWorkTimeInterval(TimeInterval interval, Resource resource);
		#region QueryWorkTime
		QueryWorkTimeEventHandler onQueryWorkTime;
		internal event QueryWorkTimeEventHandler QueryWorkTime {
			add { onQueryWorkTime += value; }
			remove { onQueryWorkTime -= value; }
		}
		protected internal virtual TimeOfDayIntervalCollection RaiseQueryWorkTime(TimeInterval interval, Resource resource) {
			if (onQueryWorkTime != null) {
				QueryWorkTimeEventArgs args = new QueryWorkTimeEventArgs(interval, resource);
				onQueryWorkTime(this, args);
				ValidateWorkTimes(args.WorkTimes);
				return args.WorkTimes;
			}
			else
				return new TimeOfDayIntervalCollection();
		}
		#endregion
		protected internal virtual void ValidateWorkTimes(TimeOfDayIntervalCollection workTimes) {
			if (!IsWorkTimesValid(workTimes))
				MakeValidWorkTimes(workTimes);
		}
		protected internal virtual bool IsWorkTimesValid(TimeOfDayIntervalCollection workTimes) {
			if (workTimes.Count <= 1)
				return true;
			TimeSpan date = workTimes[0].Start;
			int count = workTimes.Count;
			for (int i = 1; i < count; i++) {
				TimeSpan value = workTimes[i].Start;
				if (value < date)
					return false;
				date = value;
				value = workTimes[i].End;
				if (value < date)
					return false;
				date = value;
			}
			return true;
		}
		protected internal virtual void MakeValidWorkTimes(TimeOfDayIntervalCollection workTimes) {
			TimeIntervalCollectionEx intervals = new TimeIntervalCollectionEx();
			int count = workTimes.Count;
			for (int i = 0; i < count; i++) {
				TimeOfDayInterval workTime = workTimes[i];
				TimeInterval interval = new TimeInterval(new DateTime(workTime.Start.Ticks), workTime.Duration);
				intervals.Add(interval);
			}
			workTimes.Clear();
			count = intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = intervals[i];
				workTimes.Add(new TimeOfDayInterval(TimeSpan.FromTicks(interval.Start.Ticks), TimeSpan.FromTicks(interval.End.Ticks)));
			}
		}
		public virtual ResourceBaseCollection GetResources() {
			ResourceBaseCollection result = new ResourceBaseCollection();
			result.AddRange(VisibleResources);
			return result;
		}
		public virtual AppointmentBaseCollection GetAppointments() {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			result.AddRange(FilteredAppointments);
			return result;
		}
		public abstract void ZoomIn();
		protected internal abstract bool CanZoomIn();
		public abstract void ZoomOut();
		protected internal abstract bool CanZoomOut();
		protected internal virtual Image LoadSmallImage() {
			return ResourceImageLoader.LoadSmallImage(GetImageName(), Assembly.GetExecutingAssembly());
		}
		protected internal virtual Image LoadLargeImage() {
			return ResourceImageLoader.LoadLargeImage(GetImageName(), Assembly.GetExecutingAssembly());
		}
		protected internal virtual string GetImageName() {
			return String.Format("{0}View", Type);
		}
		protected internal virtual void SetOwner(IInnerSchedulerViewOwner owner) {
			this.owner = owner;
		}
		protected internal virtual void SetProperties(ISchedulerViewPropertiesBase properties) {
			UnsubscribePropertiesEvents();
			this.properties = properties;
			SubscribePropertiesEvents();
		}
		protected internal virtual void SubscribePropertiesEvents() {
			Properties.PropertyChanging += OnPropertiesChanging;
			Properties.PropertyChanged += OnPropertiesChanged;
		}
		protected internal virtual void UnsubscribePropertiesEvents() {
			Properties.PropertyChanging -= OnPropertiesChanging;
			Properties.PropertyChanged -= OnPropertiesChanged;
		}
		protected internal virtual void OnPropertiesChanging(object sender, PropertyChangingEventArgs e) {
			if (e.PropertyName == "Enabled") {
				bool ok = RaiseEnabledChanging();
				PropertyChangingEventArgsEx args = e as PropertyChangingEventArgsEx;
				if (args != null)
					args.Cancel = !ok;
			}
		}
		protected internal virtual void OnPropertiesChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "Enabled") {
				RaiseChanged(SchedulerControlChangeType.ViewEnabledChanged);
			}
			if (e.PropertyName == "GroupType") {
				RaisePropertyChanged("GroupType");
				RaiseChanged(SchedulerControlChangeType.GroupTypeChanged);
				RaiseGroupTypeChanged();
				return;
			}
			PropertyChangedEventArgsEx args = e as PropertyChangedEventArgsEx;
			if (args != null)
				RaiseUIChanged(args.PropertyName, args.OldValue, args.NewValue);
			else
				RaiseUIChanged(e.PropertyName, null, null);
		}
	}
	#endregion
	#region ViewSelectionResourceSynchronizer
	public class ViewSelectionResourceSynchronizer {
		#region Fields
		InnerSchedulerViewBase view;
		ChangeActions changeActions = ChangeActions.None;
		#endregion
		public ViewSelectionResourceSynchronizer(InnerSchedulerViewBase view) {
			if (view == null)
				Exceptions.ThrowArgumentException("view", view);
			this.view = view;
		}
		#region Properties
		protected internal InnerSchedulerViewBase View { get { return view; } }
		protected internal ChangeActions ChangeActions { get { return changeActions; } }
		#endregion
		public virtual ChangeActions Synchronize(SchedulerViewSelection selection) {
			Resource newSelectionResource = CalculateNewSelectionResource(selection.Resource);
			if (!Object.Equals(selection.Resource.Id, newSelectionResource.Id))
				this.changeActions |= ChangeActions.RaiseSelectionChanged;
			selection.Resource = newSelectionResource;
			return changeActions;
		}
		protected internal virtual Resource CalculateNewSelectionResource(Resource oldSelectionResource) {
			if (View.CanShowResources() && View.GroupType != SchedulerGroupType.None) {
				Resource currentResource = View.VisibleResources.GetResourceById(oldSelectionResource.Id);
				if (currentResource == ResourceBase.Empty)
					return ValidateSelectionResource(oldSelectionResource);
				else
					return currentResource;
			}
			else
				return ResourceBase.Empty;
		}
		protected internal virtual Resource ValidateSelectionResource(Resource oldSelectionResource) {
			Resource resource = View.FilteredResources.GetResourceById(oldSelectionResource.Id);
			if (resource == ResourceBase.Empty)
				return View.VisibleResources[0];
			else
				return ValidateSelectionForExistingNonVisibleResource(resource);
		}
		protected internal virtual Resource ValidateSelectionForExistingNonVisibleResource(Resource existingNonVisibleResource) {
			int resourceIndex = View.FilteredResources.IndexOf(existingNonVisibleResource);
			if (resourceIndex < View.ActualFirstVisibleResourceIndex)
				View.SetFirstVisibleResourceIndexCore(resourceIndex);
			else 
				View.SetFirstVisibleResourceIndexCore(resourceIndex - View.ActualResourcesPerPage + 1);
			View.QueryVisibleResources();
			this.changeActions |= ChangeActions.UpdateResourceScrollBarValue | ChangeActions.RaiseSelectionChanged;
			return existingNonVisibleResource;
		}
	}
	#endregion
	#region ViewSelectionResourceValidator
	public class ViewSelectionResourceValidator : ViewSelectionResourceSynchronizer {
		public ViewSelectionResourceValidator(InnerSchedulerViewBase view)
			: base(view) {
		}
		protected internal override Resource ValidateSelectionForExistingNonVisibleResource(Resource existingNonVisibleResource) {
			int resourceIndex = View.FilteredResources.IndexOf(existingNonVisibleResource);
			if (resourceIndex < View.ActualFirstVisibleResourceIndex)
				return View.VisibleResources[0];
			else
				return View.VisibleResources[View.VisibleResources.Count - 1];
		}
	}
	#endregion
	#region SchedulerViewCollection
	public class SchedulerViewCollection<T> : DXCollection<T> where T : ISchedulerViewRepositoryItem {
		#region Fields
		Dictionary<SchedulerViewType, T> viewTypeHash;
		#endregion
		public SchedulerViewCollection() {
			viewTypeHash = new Dictionary<SchedulerViewType, T>();
		}
		#region Properties
		public T this[SchedulerViewType type] {
			get {
				T result;
				if (ViewTypeHash.TryGetValue(type, out result))
					return result;
				else
					return default(T);
			}
		}
		protected internal Dictionary<SchedulerViewType, T> ViewTypeHash { get { return viewTypeHash; } }
		#endregion
		protected override void OnInsertComplete(int position, T obj) {
			base.OnInsertComplete(position, obj);
			ViewTypeHash.Add(obj.Type, obj);
		}
		protected override void OnRemoveComplete(int position, T obj) {
			base.OnRemoveComplete(position, obj);
			if (ViewTypeHash.ContainsKey(obj.Type))
				ViewTypeHash.Remove(obj.Type);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			ViewTypeHash.Clear();
		}
	}
	#endregion
	#region SchedulerViewRepositoryBase
	public abstract class SchedulerViewRepositoryBase : IDisposable {
		#region Fields
		bool isDisposed;
		#endregion
		#region Properties
		internal bool IsDisposed { get { return isDisposed; } }
		protected internal InnerDayView InnerDayView { get { return (InnerDayView)GetInnerView(SchedulerViewType.Day); } }
		protected internal InnerWorkWeekView InnerWorkWeekView { get { return (InnerWorkWeekView)GetInnerView(SchedulerViewType.WorkWeek); } }
		protected internal InnerWeekView InnerWeekView { get { return (InnerWeekView)GetInnerView(SchedulerViewType.Week); } }
		protected internal InnerMonthView InnerMonthView { get { return (InnerMonthView)GetInnerView(SchedulerViewType.Month); } }
		protected internal InnerTimelineView InnerTimelineView { get { return (InnerTimelineView)GetInnerView(SchedulerViewType.Timeline); } }
		protected internal InnerFullWeekView InnerFullWeekView { get { return (InnerFullWeekView)GetInnerView(SchedulerViewType.FullWeek); } }
		protected internal InnerGanttView InnerGanttView { get { return (InnerGanttView)GetInnerView(SchedulerViewType.Gantt); } }
		public abstract int Count { get; }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				DisposeViews();
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerViewRepositoryBase() {
			Dispose(false);
		}
		#endregion
		protected internal abstract void DisposeViews();
		protected internal abstract void Reset();
		protected internal abstract void SetGroupType(SchedulerGroupType type);
		protected internal abstract InnerSchedulerViewBase GetInnerView(SchedulerViewType viewType);
		protected internal abstract InnerSchedulerViewBase GetInnerView(int index);
		protected internal abstract void CreateViews(InnerSchedulerControl control);
		protected internal abstract void InitializeViews(InnerSchedulerControl control);
	}
	#endregion
	#region SchedulerViewTypedRepositoryBase (absract class)
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public abstract class SchedulerViewTypedRepositoryBase<T> : SchedulerViewRepositoryBase where T : ISchedulerViewRepositoryItem {
		#region Fields
		SchedulerViewCollection<T> views;
		#endregion
		protected SchedulerViewTypedRepositoryBase() {
			this.views = new SchedulerViewCollection<T>();
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal SchedulerViewCollection<T> Views { get { return views; } }
		public T this[SchedulerViewType type] { get { return views[type]; } }
		public T this[int index] { get { return views[index]; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int Count { get { return views.Count; } }
		#endregion
		protected internal override void InitializeViews(InnerSchedulerControl control) {
			int count = views.Count;
			for (int i = 0; i < count; i++)
				views[i].Initialize(control);
		}
		protected internal override void DisposeViews() {
			int count = views.Count;
			for (int i = count - 1; i >= 0; i--) {
				T view = views[i];
				UnregisterView(view);
				view.Dispose();
			}
		}
		protected internal override void Reset() {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].Reset();
		}
		protected internal override void SetGroupType(SchedulerGroupType type) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].GroupType = type;
		}
		protected internal virtual void RegisterView(T view) {
			views.Add(view);
		}
		protected internal virtual void UnregisterView(T view) {
			views.Remove(view);
		}
		protected internal override InnerSchedulerViewBase GetInnerView(SchedulerViewType viewType) {
			T view = this[viewType];
			if (view != null)
				return view.InnerView;
			else
				return null;
		}
		protected internal override InnerSchedulerViewBase GetInnerView(int index) {
			return views[index].InnerView;
		}
	}
	#endregion
	#region ISupportAppointmentsBase
	public interface ISupportAppointmentsBase {
		AppointmentDisplayOptions AppointmentDisplayOptions { get; }
		IAppointmentComparerProvider AppointmentComparerProvider { get; }
		bool UseAsyncMode { get; }
		SchedulerCancellationTokenSource CancellationToken { get; }
		TimeInterval GetVisibleInterval();
	}
	#endregion
	#region IWorkTimeInfo
	public interface IWorkTimeInfo {
		TimeOfDayIntervalCollection WorkTimes { get; }
		bool IsWorkDay { get; }
	}
	#endregion
	#region WorkTimeInfo
	public class WorkTimeInfo : IWorkTimeInfo {
		TimeOfDayIntervalCollection workTimes;
		bool isWorkDay;
		public WorkTimeInfo(TimeOfDayIntervalCollection workTimes, bool isWorkDay) {
			Guard.ArgumentNotNull(workTimes, "workTimes");
			this.workTimes = workTimes;
			this.isWorkDay = isWorkDay;
		}
		#region IWorkTimeInfo Members
		public TimeOfDayIntervalCollection WorkTimes { get { return workTimes; } }
		public bool IsWorkDay { get { return isWorkDay; } }
		#endregion
	}
	#endregion
	#region InnerSchedulerViewComparer
	public class InnerSchedulerViewComparer : IComparer<InnerSchedulerViewBase> {
		Hashtable correctOrder;
		void fillCorrectOrders() {
			correctOrder = new Hashtable();
			correctOrder.Add(SchedulerViewType.Day, 1);
			correctOrder.Add(SchedulerViewType.WorkWeek, 2);
			correctOrder.Add(SchedulerViewType.FullWeek, 3);
			correctOrder.Add(SchedulerViewType.Week, 4);
			correctOrder.Add(SchedulerViewType.Month, 5);
			correctOrder.Add(SchedulerViewType.Timeline, 6);
			correctOrder.Add(SchedulerViewType.Gantt, 7);
		}
		#region IComparer<InnerSchedulerViewBase> Members
		public int Compare(InnerSchedulerViewBase x, InnerSchedulerViewBase y) {
			if (correctOrder == null)
				fillCorrectOrders();
			if (Convert.ToInt16(correctOrder[x.Type]) > Convert.ToInt16(correctOrder[y.Type]))
				return 1;
			else if (Convert.ToInt16(correctOrder[x.Type]) == Convert.ToInt16(correctOrder[y.Type]))
				return 0;
			return -1;
		}
		#endregion
	}
	#endregion
}

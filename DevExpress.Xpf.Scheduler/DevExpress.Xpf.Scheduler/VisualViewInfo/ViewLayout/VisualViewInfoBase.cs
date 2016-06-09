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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Automation;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public abstract class VisualViewInfoBase : Control, ISupportCopyFrom<ISchedulerViewInfoBase> {
		static VisualViewInfoBase() {
		}
		protected VisualViewInfoBase() {
			DefaultStyleKey = typeof(VisualViewInfoBase);
			Loaded += new RoutedEventHandler(OnVisualViewInfoBaseLoaded);
			AddHandler(ThemeManager.ThemeChangedEvent, new ThemeChangedRoutedEventHandler(OnThemeManagerThemeChanged));
		}
		protected virtual void OnThemeManagerThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
			this.Style = View.ContentStyleSelector.SelectStyle(this, this);
		}
		void OnVisualViewInfoBaseLoaded(object sender, RoutedEventArgs e) {
			if (Control.IsKeyboardFocusWithin)
				Focus();
		}
		#region View
		static readonly DependencyPropertyKey ViewPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualViewInfoBase, SchedulerViewBase>("View", null);
		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualViewInfoBaseView")]
#endif
		public SchedulerViewBase View { get { return (SchedulerViewBase)GetValue(ViewProperty); } protected set { this.SetValue(ViewPropertyKey, value); } }
		#endregion
		#region Control
		static readonly DependencyPropertyKey ControlPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualViewInfoBase, SchedulerControl>("Control", null);
		public static readonly DependencyProperty ControlProperty = ControlPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualViewInfoBaseControl")]
#endif
		public SchedulerControl Control { get { return (SchedulerControl)GetValue(ControlProperty); } protected set { this.SetValue(ControlPropertyKey, value); } }
		#endregion
		#region ISupportCopyFrom<ISchedulerViewInfoBase> Members
		void ISupportCopyFrom<ISchedulerViewInfoBase>.CopyFrom(ISchedulerViewInfoBase source) {
			CopyFromCore(source);
		}
		#endregion
		internal void CopyAppointmentsViewInfo(ISchedulerViewInfoBase source) {
			CopyAppointmentsViewInfoCore(source);
		}
		protected virtual void CopyAppointmentsViewInfoCore(ISchedulerViewInfoBase source) {
		}
		protected virtual void CopyFromCore(ISchedulerViewInfoBase source) {
			View = source.View;
			Control = source.View.Control;
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new VisualViewInfoBaseAutomationPeer(this);
		}
		protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			if (Control.IsKeyboardFocusWithin) {
				Keyboard.ClearFocus();
				Focus();
			}
		}
	}
	public abstract class VisualResourcesBasedViewInfo : VisualViewInfoBase {
		#region ResourceContainers
		static readonly DependencyPropertyKey ResourceContainersPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualResourcesBasedViewInfo, VisualResourcesCollection>("ResourceContainers", null);
		public static readonly DependencyProperty ResourceContainersProperty = ResourceContainersPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourcesBasedViewInfoResourceContainers")]
#endif
		public VisualResourcesCollection ResourceContainers { get { return (VisualResourcesCollection)GetValue(ResourceContainersProperty); } protected set { this.SetValue(ResourceContainersPropertyKey, value); } }
		internal const string ResourceContainersPropertyName = "ResourceContainers";
		#endregion
		protected abstract VisualResource CreateVisualResource();
	}
	public abstract class VisualIntervalsBasedViewInfo : VisualViewInfoBase {
		#region Intervals
		static readonly DependencyPropertyKey IntervalsPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualIntervalsBasedViewInfo, VisualIntervalsCollection>("Intervals", null);
		public static readonly DependencyProperty IntervalsProperty = IntervalsPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualIntervalsBasedViewInfoIntervals")]
#endif
		public VisualIntervalsCollection Intervals { get { return (VisualIntervalsCollection)GetValue(IntervalsProperty); } protected set { this.SetValue(IntervalsPropertyKey, value); } }
		#endregion
		protected abstract VisualInterval CreateVisualInterval();
	}
	public abstract class VisualResource : DependencyObject, ISupportCopyFrom<SingleResourceViewInfo> {
		#region View
		static readonly DependencyPropertyKey ViewPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualResource, SchedulerViewBase>("View", null);
		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourceView")]
#endif
		public SchedulerViewBase View { get { return (SchedulerViewBase)GetValue(ViewProperty); } protected set { this.SetValue(ViewPropertyKey, value); } }
		#endregion
		#region SimpleIntervals
		static readonly DependencyPropertyKey SimpleIntervalsPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualResource, VisualSimpleResourceIntervalCollection>("SimpleIntervals", null);
		public static readonly DependencyProperty SimpleIntervalsProperty = SimpleIntervalsPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourceSimpleIntervals")]
#endif
		public VisualSimpleResourceIntervalCollection SimpleIntervals { get { return (VisualSimpleResourceIntervalCollection)GetValue(SimpleIntervalsProperty); } protected set { this.SetValue(SimpleIntervalsPropertyKey, value); } }
		internal const string SimpleIntervalsPropertyName = "SimpleIntervals";
		#endregion
		#region ResourceHeader
		public static readonly DependencyProperty ResourceHeaderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResource, VisualResourceHeaderBaseContent>("ResourceHeader", null);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourceResourceHeader")]
#endif
		public VisualResourceHeaderBaseContent ResourceHeader { get { return (VisualResourceHeaderBaseContent)GetValue(ResourceHeaderProperty); } set { SetValue(ResourceHeaderProperty, value); } }
		#endregion
		#region PrevNavButtonInfo
		public static readonly DependencyProperty PrevNavButtonInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResource, NavigationButtonViewModel>("PrevNavButtonInfo", null);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourcePrevNavButtonInfo")]
#endif
		public NavigationButtonViewModel PrevNavButtonInfo { get { return (NavigationButtonViewModel)GetValue(PrevNavButtonInfoProperty); } set { SetValue(PrevNavButtonInfoProperty, value); } }
		#endregion
		#region PrevNavButtonVisibility
		public static readonly DependencyProperty PrevNavButtonVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResource, Visibility>("PrevNavButtonVisibility", Visibility.Collapsed);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourcePrevNavButtonVisibility")]
#endif
		public Visibility PrevNavButtonVisibility { get { return (Visibility)GetValue(PrevNavButtonVisibilityProperty); } set { SetValue(PrevNavButtonVisibilityProperty, value); } }
		#endregion
		#region PrevNavButtonEnabled
		public static readonly DependencyProperty PrevNavButtonEnabledProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResource, bool>("PrevNavButtonEnabled", false);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourcePrevNavButtonEnabled")]
#endif
		public bool PrevNavButtonEnabled { get { return (bool)GetValue(PrevNavButtonEnabledProperty); } set { SetValue(PrevNavButtonEnabledProperty, value); } }
		#endregion
		#region NextNavButtonInfo
		public static readonly DependencyProperty NextNavButtonInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResource, NavigationButtonViewModel>("NextNavButtonInfo", null);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourceNextNavButtonInfo")]
#endif
		public NavigationButtonViewModel NextNavButtonInfo { get { return (NavigationButtonViewModel)GetValue(NextNavButtonInfoProperty); } set { SetValue(NextNavButtonInfoProperty, value); } }
		#endregion
		#region NextNavButtonVisibility
		public static readonly DependencyProperty NextNavButtonVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResource, Visibility>("NextNavButtonVisibility", Visibility.Collapsed);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourceNextNavButtonVisibility")]
#endif
		public Visibility NextNavButtonVisibility { get { return (Visibility)GetValue(NextNavButtonVisibilityProperty); } set { SetValue(NextNavButtonVisibilityProperty, value); } }
		#endregion
		#region NextNavButtonEnabled
		public static readonly DependencyProperty NextNavButtonEnabledProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualResource, bool>("NextNavButtonEnabled", false);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualResourceNextNavButtonEnabled")]
#endif
		public bool NextNavButtonEnabled { get { return (bool)GetValue(NextNavButtonEnabledProperty); } set { SetValue(NextNavButtonEnabledProperty, value); } }
		#endregion        
		#region ISupportCopyFrom<SingleResourceViewInfo> Members
		void ISupportCopyFrom<SingleResourceViewInfo>.CopyFrom(SingleResourceViewInfo source) {
			CopyFromCore(source);
		}
		#endregion
		protected virtual void CopyFromCore(SingleResourceViewInfo source) {
			if (SimpleIntervals == null)
				SimpleIntervals = new VisualSimpleResourceIntervalCollection();
			CollectionCopyHelper.Copy(SimpleIntervals, GetCellContainers(source), CreateVisualSimpleResourceInterval);
			View = source.View;
			if (source.ResourceHeader != null) {
				if (ResourceHeader == null)
					ResourceHeader = CreateResourceHeader();
				if (ResourceHeader != null)
					ResourceHeader.CopyFrom(source.ResourceHeader);
			} else
				ResourceHeader = null;
			if (PrevNavButtonInfo == null)
				PrevNavButtonInfo = new NavigationButtonViewModel();
			PrevNavButtonInfo.CopyFrom(source.PrevNavButtonInfo);
			if (NextNavButtonInfo == null)
				NextNavButtonInfo = new NavigationButtonViewModel();
			NextNavButtonInfo.CopyFrom(source.NextNavButtonInfo);
		}
		protected virtual CellContainerCollection GetCellContainers(SingleResourceViewInfo source) {
			return source.CellContainers;
		}
		protected virtual VisualResourceHeaderBaseContent CreateResourceHeader() {
			VisualResourceHeaderBaseContent result = CreateResourceHeaderCore();
			return result;
		}
		protected abstract VisualResourceHeaderBaseContent CreateResourceHeaderCore();
		public virtual void CopyAppointmentsFrom(SingleResourceViewInfo source) {
			if (SimpleIntervals == null)
				return;
			CellContainerCollection cellContainers = GetCellContainers(source);
			int count = cellContainers.Count;
			for (int i = 0; i < count; i++)
				SimpleIntervals[i].CopyAppointmentsFrom(cellContainers[i]);
		}
		protected abstract VisualSimpleResourceInterval CreateVisualSimpleResourceInterval();
	}
	public abstract class VisualInterval : DependencyObject, ISupportCopyFrom<SingleDayViewInfo> {
		#region View
		static readonly DependencyPropertyKey ViewPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualInterval, SchedulerViewBase>("View", null);
		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;
		public SchedulerViewBase View { get { return (SchedulerViewBase)GetValue(ViewProperty); } protected set { this.SetValue(ViewPropertyKey, value); } }
		#endregion
		#region SimpleIntervals
		static readonly DependencyPropertyKey SimpleIntervalsPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualInterval, VisualSimpleResourceIntervalCollection>("SimpleIntervals", null);
		public static readonly DependencyProperty SimpleIntervalsProperty = SimpleIntervalsPropertyKey.DependencyProperty;
		public VisualSimpleResourceIntervalCollection SimpleIntervals { get { return (VisualSimpleResourceIntervalCollection)GetValue(SimpleIntervalsProperty); } protected set { this.SetValue(SimpleIntervalsPropertyKey, value); } }
		#endregion
		#region ResourceHeaders
		static readonly DependencyPropertyKey ResourceHeadersPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualInterval, VisualResourceHeaderBaseContentCollection>("ResourceHeaders", null);
		public static readonly DependencyProperty ResourceHeadersProperty = ResourceHeadersPropertyKey.DependencyProperty;
		public VisualResourceHeaderBaseContentCollection ResourceHeaders { get { return (VisualResourceHeaderBaseContentCollection)GetValue(ResourceHeadersProperty); } protected set { this.SetValue(ResourceHeadersPropertyKey, value); } }
		#endregion
		#region NavigationButtons
		static readonly DependencyPropertyKey NavigationButtonsPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualInterval, VisualNavigationButtonsCollection>("NavigationButtons", null);
		public static readonly DependencyProperty NavigationButtonsProperty = NavigationButtonsPropertyKey.DependencyProperty;
		public VisualNavigationButtonsCollection NavigationButtons { get { return (VisualNavigationButtonsCollection)GetValue(NavigationButtonsProperty); } protected set { this.SetValue(NavigationButtonsPropertyKey, value); } }
		#endregion
		protected abstract VisualSimpleResourceInterval CreateVisualSimpleResourceInterval();
		#region ISupportCopyFrom<SingleDayViewInfo> Members
		void ISupportCopyFrom<SingleDayViewInfo>.CopyFrom(SingleDayViewInfo source) {
			CopyFromCore(source);
		}
		#endregion
		protected abstract void CopyFromCore(SingleDayViewInfo source);
		public abstract void CopyAppointmentsFrom(SingleDayViewInfo source);
	}
	public class ObservableCollectionWithFirstAndLast<T> : ObservableCollection<T> where T : class {
		T prevLast;
		T prevFirst;
		#region Last
		public T Last {
			get {
				prevLast = GetLastItemCore();
				return prevLast;
			}
		}
		#endregion
		#region First
		public T First {
			get {
				prevFirst = GetFirstItemCore();
				return prevFirst;
			}
		}
		#endregion
		protected virtual T GetFirstItemCore() {
			if (Count > 0)
				return this[0];
			else
				return null;
		}
		protected virtual T GetLastItemCore() {
			int count = Count;
			if (count > 0)
				return this[count - 1];
			else
				return null;
		}
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			T prevReturnedLast = prevLast;
			if (Last != prevReturnedLast)
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Last"));
			T prevReturnedFirst = prevFirst;
			if (First != prevReturnedFirst)
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("First"));
		}
	}
	public class VisualResourcesCollection : ObservableCollectionWithFirstAndLast<VisualResource> {
	}
	public class VisualIntervalsCollection : ObservableCollectionWithFirstAndLast<VisualInterval> {
	}
	public abstract class VisualSimpleResourceInterval : DependencyObject, ISupportCopyFrom<ICellContainer> {
		protected VisualSimpleResourceInterval() {
			IntervalCells = new TimeInterval(DateTime.MinValue, DateTime.MaxValue);
		}
		#region View
		static readonly DependencyPropertyKey ViewPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualSimpleResourceInterval, SchedulerViewBase>("View", null);
		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;
		public SchedulerViewBase View { get { return (SchedulerViewBase)GetValue(ViewProperty); } protected set { this.SetValue(ViewPropertyKey, value); } }
		#endregion
		#region IntervalStart
		public static readonly DependencyProperty IntervalStartProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualSimpleResourceInterval, DateTime>("IntervalStart", DateTime.MinValue, (d, e) => d.OnIntervalStartChanged(e.OldValue, e.NewValue));
		public DateTime IntervalStart { get { return (DateTime)GetValue(IntervalStartProperty); } set { SetValue(IntervalStartProperty, value); } }
		protected internal virtual void OnIntervalStartChanged(DateTime oldValue, DateTime newValue) {
		}
		#endregion
		#region IntervalEnd
		public static readonly DependencyProperty IntervalEndProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualSimpleResourceInterval, DateTime>("IntervalEnd", DateTime.MinValue);
		public DateTime IntervalEnd { get { return (DateTime)GetValue(IntervalEndProperty); } set { SetValue(IntervalEndProperty, value); } }
		#endregion
		#region IntervalCells
		public static readonly DependencyProperty IntervalCellsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualSimpleResourceInterval, TimeInterval>("IntervalCells", null, (d, e) => d.OnIntervalCellsChanged(e.OldValue, e.NewValue));
		public TimeInterval IntervalCells { get { return (TimeInterval)GetValue(IntervalCellsProperty); } set { SetValue(IntervalCellsProperty, value); } }
		protected internal virtual void OnIntervalCellsChanged(TimeInterval oldValue, TimeInterval newValue) {
		}
		#endregion
		#region Brushes
		public static readonly DependencyProperty BrushesProperty = VisualResourceCellBaseContent.BrushesProperty.AddOwner(typeof(VisualSimpleResourceInterval));
		public VisualResourceBrushes Brushes { get { return (VisualResourceBrushes)GetValue(BrushesProperty); } set { SetValue(BrushesProperty, value); } }
		#endregion
		#region ISupportCopyFrom<ICellContainer> Members
		void ISupportCopyFrom<ICellContainer>.CopyFrom(ICellContainer source) {
			CopyFromCore(source);
		}
		#endregion
		protected abstract void CopyFromCore(ICellContainer source);
		public abstract void CopyAppointmentsFrom(ICellContainer cellContainer);
	}
	public class VisualSimpleResourceIntervalCollection : ObservableCollection<VisualSimpleResourceInterval> {
	}
	public class VisualNavigationButtons : DependencyObject, ISupportCopyFrom<SingleResourceViewInfo> {
		#region View
		static readonly DependencyPropertyKey ViewPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualNavigationButtons, SchedulerViewBase>("View", null);
		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;
		public SchedulerViewBase View { get { return (SchedulerViewBase)GetValue(ViewProperty); } protected set { this.SetValue(ViewPropertyKey, value); } }
		#endregion
		#region PrevNavButtonInfo
		public static readonly DependencyProperty PrevNavButtonInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualNavigationButtons, NavigationButtonViewModel>("PrevNavButtonInfo", null);
		public NavigationButtonViewModel PrevNavButtonInfo { get { return (NavigationButtonViewModel)GetValue(PrevNavButtonInfoProperty); } set { SetValue(PrevNavButtonInfoProperty, value); } }
		#endregion
		#region NextNavigationButton
		public static readonly DependencyProperty NextNavButtonInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualNavigationButtons, NavigationButtonViewModel>("NextNavButtonInfo", null);
		public NavigationButtonViewModel NextNavButtonInfo { get { return (NavigationButtonViewModel)GetValue(NextNavButtonInfoProperty); } set { SetValue(NextNavButtonInfoProperty, value); } }
		#endregion
		#region ISupportCopyFrom<SingleResourceViewInfo> Members
		void ISupportCopyFrom<SingleResourceViewInfo>.CopyFrom(SingleResourceViewInfo source) {
			if (PrevNavButtonInfo == null)
				PrevNavButtonInfo = new NavigationButtonViewModel();
			PrevNavButtonInfo.CopyFrom(source.PrevNavButtonInfo);
			if (NextNavButtonInfo == null)
				NextNavButtonInfo = new NavigationButtonViewModel();
			NextNavButtonInfo.CopyFrom(source.NextNavButtonInfo);
			View = source.View;
		}
		#endregion
	}
	public class VisualNavigationButtonsCollection : ObservableCollection<VisualNavigationButtons> {
	}
	public abstract class VisualCellContainer : DependencyObject, ISupportCopyFrom<ICellContainer> {
		Dictionary<Appointment, FrameworkElement> appointmentPresentersCache;
		#region View
		static readonly DependencyPropertyKey ViewPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualCellContainer, SchedulerViewBase>("View", null);
		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;
		public SchedulerViewBase View { get { return (SchedulerViewBase)GetValue(ViewProperty); } protected set { this.SetValue(ViewPropertyKey, value); } }
		#endregion
		#region Cells
		static readonly DependencyPropertyKey CellsPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualCellContainer, VisualTimeCellContentCollection>("Cells", null);
		public static readonly DependencyProperty CellsProperty = CellsPropertyKey.DependencyProperty;
		public VisualTimeCellContentCollection Cells { get { return (VisualTimeCellContentCollection)GetValue(CellsProperty); } protected set { this.SetValue(CellsPropertyKey, value); } }
		internal const string CellsPropertyName = "Cells";
		#endregion
		#region AppointmentControlCollection
		public AppointmentControlObservableCollection AppointmentControlCollection {
			get { return (AppointmentControlObservableCollection)GetValue(AppointmentControlCollectionProperty); }
			set { SetValue(AppointmentControlCollectionProperty, value); }
		}
		internal const string AppointmentControlCollectionPropertyName = "AppointmentControlCollection";
		public static readonly DependencyProperty AppointmentControlCollectionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualCellContainer, AppointmentControlObservableCollection>("AppointmentControlCollection", null, (d, e) => d.OnAppointmentControlCollectionChanged(e.OldValue, e.NewValue), null);
		void OnAppointmentControlCollectionChanged(AppointmentControlObservableCollection oldValue, AppointmentControlObservableCollection newValue) {
			RaiseAppointmentCollectionChanged();
		}
		#endregion        
		#region DraggedAppointmentControlCollection
		public AppointmentControlObservableCollection DraggedAppointmentControlCollection {
			get { return (AppointmentControlObservableCollection)GetValue(DraggedAppointmentControlCollectionProperty); }
			set { SetValue(DraggedAppointmentControlCollectionProperty, value); }
		}
		public static readonly DependencyProperty DraggedAppointmentControlCollectionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualCellContainer, AppointmentControlObservableCollection>("DraggedAppointmentControlCollection", null, (d, e) => d.OnDraggedAppointmentControlCollectionChanged(e.OldValue, e.NewValue), null);
		void OnDraggedAppointmentControlCollectionChanged(AppointmentControlObservableCollection oldValue, AppointmentControlObservableCollection newValue) {
		}
		#endregion
		#region SelectedCells
		public static readonly DependencyProperty SelectedCellsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualCellContainer, SelectedCellIndexesInterval>("SelectedCells", null);
		public SelectedCellIndexesInterval SelectedCells { get { return (SelectedCellIndexesInterval)GetValue(SelectedCellsProperty); } set { SetValue(SelectedCellsProperty, value); } }
		#endregion
		#region OnAppointmentCollectionChanged
		public event EventHandler AppointmentControlCollectionChanged;
		protected virtual void RaiseAppointmentCollectionChanged() {
			if (AppointmentControlCollectionChanged != null)
				AppointmentControlCollectionChanged(this, EventArgs.Empty);
		}
		#endregion
		protected virtual VisualTimeCellBaseContent CreateVisualTimeCell() {
			return new VisualTimeCellContent();
		}
		#region ISupportCopyFrom<ICellContainer> Members
		void ISupportCopyFrom<ICellContainer>.CopyFrom(ICellContainer source) {
			CopyFromCore(source);
		}
		#endregion
		protected virtual void CopyFromCore(ICellContainer source) {
			View = source.View;
			if (Cells == null)
				Cells = new VisualTimeCellContentCollection();
			CollectionCopyHelper.Copy(Cells, source, CreateVisualTimeCell);
			if (AppointmentControlCollection == null)
				AppointmentControlCollection = new AppointmentControlObservableCollection();
			AppointmentControlCollection.SetRange(source.AppointmentControls);
			if (DraggedAppointmentControlCollection == null)
				DraggedAppointmentControlCollection = new AppointmentControlObservableCollection();
			DraggedAppointmentControlCollection.SetRange(source.DraggedAppointmentControls);
			this.SelectedCells = source.SchedulerControl.SelectedAppointments.Count == 0 ? source.SelectedCells : null;
			this.appointmentPresentersCache = source.AppointmentPresentersCache;
		}
		public virtual void CopyAppointmentsFrom(ICellContainer source) {
			ISchedulerStateService service = View.Control.GetService<ISchedulerStateService>();
			SchedulerAutoScroller autoScroller = View.Control.MouseHandler.AutoScroller as SchedulerAutoScroller;
			if ((service != null && !service.AreAppointmentsDragged) || autoScroller.IsActivated)
				AppointmentControlCollection.SetRange(source.AppointmentControls);
			DraggedAppointmentControlCollection.SetRange(source.DraggedAppointmentControls);
		}
		protected abstract VisualAppointmentControl CreateXpfAppointment();
		protected virtual VisualAppointmentControl CreateXpfDraggedAppointment() {
			return new VisualDraggedAppointmentControl();
		}
		public void ClearAppointmentPresentersCache() {
			appointmentPresentersCache.Clear();
		}
		public void AddAppointmentPresenterToCache(Appointment apt, FrameworkElement presenter) {
			if (!appointmentPresentersCache.ContainsKey(apt)) {
				appointmentPresentersCache[apt] = presenter;
			}
		}
	}
	public class VisualHorizontalCellContainer : VisualCellContainer {
		protected override VisualAppointmentControl CreateXpfAppointment() {
			return new VisualHorizontalAppointmentControl();
		}
		protected override void CopyFromCore(ICellContainer source) {
			base.CopyFromCore(source);
			View = source.View;
		}
		protected override VisualTimeCellBaseContent CreateVisualTimeCell() {
			return new VisualSingleTimelineCellContent();
		}
	}
	public class VisualTimeCellCollection : ObservableCollection<VisualTimeCellBase> {
	}
	public class VisualTimeCellContentCollection : ObservableCollectionWithFirstAndLast<VisualTimeCellBaseContent> {
	}
}

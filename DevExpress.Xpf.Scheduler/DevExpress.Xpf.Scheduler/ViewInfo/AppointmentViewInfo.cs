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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Implementations;
using System.Threading;
#if WPF || SL
using DevExpress.XtraScheduler.Services;
using System.Collections.ObjectModel;
#endif
#if SL
using DevExpress.Xpf.Scheduler.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region AppointmentViewInfo
	public class AppointmentViewInfo : IAppointmentViewInfo, ISelectableIntervalViewInfo, INotifyPropertyChanged {
		#region Fields
		bool hasTopBorder;
		bool hasBottomBorder;
		bool hasLeftBorder;
		bool hasRightBorder;
		TimeInterval interval;
		Appointment appointment;
		Resource resource;
		AppointmentViewInfoOptions options;
		IAppointmentStatus status;
		IAppointmentLabel label;
		TimeInterval appointmentInterval;
		bool selected;
		double relativePosition;
		int firstCellIndex;
		int lastCellIndex;
		bool visible;
		AppointmentContentCalculatorHelper contentCalculatorHelper;
		string subject;
		string description;
		string location;
		bool isStartVisible;
		bool isEndVisible;
		Object customViewInfo;
		#endregion
		public AppointmentViewInfo(Appointment appointment, TimeZoneHelper timeZoneEngine) {
			if (appointment == null)
				Exceptions.ThrowArgumentException("appointment", appointment);
			this.appointment = appointment;
			this.subject = Appointment.Subject;
			this.description = Appointment.Description;
			this.location = GetLocation(Appointment);
			Initialize(timeZoneEngine);
		}
		#region Properties
		public Appointment Appointment {
			get { return appointment; }
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoCustomViewInfo")]
#endif
		public Object CustomViewInfo {
			get { return customViewInfo; }
			set {
				if (Object.Equals(customViewInfo, value))
					return;
				customViewInfo = value;
				RaiseOnPropertyChanged("CustomViewInfo");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoSubject")]
#endif
		public virtual string Subject {
			get { return subject; }
			set {
				if (subject == value)
					return;
				subject = value;
				RaiseOnPropertyChanged("Subject");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoDescription")]
#endif
		public virtual string Description {
			get { return description; }
			set {
				if (description == value)
					return;
				description = value;
				RaiseOnPropertyChanged("Description");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoLocation")]
#endif
		public virtual string Location {
			get { return location; }
			set {
				if (location == value)
					return;
				location = value;
				RaiseOnPropertyChanged("Location");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoLabelColor")]
#endif
		public virtual Color LabelColor { get { return Label != null ? Label.GetColor() : Colors.Black; } }
		public virtual Brush LabelBrush { get { return Label != null ? ((AppointmentLabel)Label).Brush : Brushes.Black; } }
		public virtual Brush StatusBrush { get { return Status != null ? Status.GetBrush() : Brushes.Black; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoIsStartVisible")]
#endif
		public bool IsStartVisible {
			get { return isStartVisible; }
			set {
				if (isStartVisible == value)
					return;
				isStartVisible = value;
				RaiseOnPropertyChanged("IsStartVisible");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoIsEndVisible")]
#endif
		public bool IsEndVisible {
			get { return isEndVisible; }
			set {
				if (isEndVisible == value)
					return;
				isEndVisible = value;
				RaiseOnPropertyChanged("IsEndVisible");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoHitTestType")]
#endif
		public SchedulerHitTest HitTestType { get { return SchedulerHitTest.AppointmentContent; } }
		protected internal int FirstCellIndex {
			get { return firstCellIndex; }
			set {
				if (FirstCellIndex == value)
					return;
				firstCellIndex = value;
				RaiseOnPropertyChanged("FirstCellIndex");
			}
		}
		protected internal int LastCellIndex {
			get { return lastCellIndex; }
			set {
				if (LastCellIndex == value)
					return;
				lastCellIndex = value;
				RaiseOnPropertyChanged("LastCellIndex");
			}
		}
		protected internal bool Visible {
			get { return visible; }
			set {
				if (Visible == value)
					return;
				visible = value;
				RaiseOnPropertyChanged("Visible");
			}
		}
		protected internal double RelativePosition {
			get { return relativePosition; }
			set {
				if (RelativePosition == value)
					return;
				relativePosition = value;
				RaiseOnPropertyChanged("RelativePosition");
			}
		}
		protected internal double Height { get; set; }
		protected internal AppointmentContentCalculatorHelper ContentCalculatorHelper {
			get { return contentCalculatorHelper; }
			set {
				if (ContentCalculatorHelper == value)
					return;
				contentCalculatorHelper = value;
				RaiseOnPropertyChanged("ContentCalculatorHelper");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoAppointmentInterval")]
#endif
		public TimeInterval AppointmentInterval { get { return appointmentInterval; } }
		public Resource Resource {
			get { return resource; }
			set {
				if (Resource == value)
					return;
				if (value == null)
					Exceptions.ThrowArgumentException("resource", resource);
				else
					resource = value;
				RaiseOnPropertyChanged("Resource");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoInterval")]
#endif
		public TimeInterval Interval {
			get { return interval; }
			set {
				if (interval == value)
					return;
				if (value == null)
					Exceptions.ThrowArgumentException("interval", interval);
				else
					interval = value;
				RaiseOnPropertyChanged("Interval");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoSelected")]
#endif
		public bool Selected {
			get { return selected; }
			set {
				if (selected == value)
					return;
				selected = value;
				RaiseOnPropertyChanged("Selected");
			}
		}
		#region Border Options
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoHasTopBorder")]
#endif
		public bool HasTopBorder {
			get { return hasTopBorder; }
			set {
				if (hasTopBorder == value)
					return;
				hasTopBorder = value;
				RaiseOnPropertyChanged("HasTopBorder");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoHasBottomBorder")]
#endif
		public bool HasBottomBorder {
			get { return hasBottomBorder; }
			set {
				if (hasBottomBorder == value)
					return;
				hasBottomBorder = value;
				RaiseOnPropertyChanged("HasBottomBorder");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoHasLeftBorder")]
#endif
		public bool HasLeftBorder {
			get { return hasLeftBorder; }
			set {
				if (HasLeftBorder == value)
					return;
				hasLeftBorder = value;
				RaiseOnPropertyChanged("HasLeftBorder");
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoHasRightBorder")]
#endif
		public bool HasRightBorder {
			get { return hasRightBorder; }
			set {
				if (hasRightBorder == value)
					return;
				hasRightBorder = value;
				RaiseOnPropertyChanged("HasRightBorder");
			}
		}
		#endregion
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentViewInfoOptions")]
#endif
		public AppointmentViewInfoOptions Options { get { return options; } }
		public IAppointmentStatus Status {
			get { return status; }
			set {
				if (Status == value)
					return;
				status = value;
				RaiseOnPropertyChanged("Status");
				RaiseOnPropertyChanged("StatusColor");
				RaiseOnPropertyChanged("StatusBrush");
			}
		}
		public IAppointmentLabel Label {
			get {
				return label;
			}
			set {
				if (Label == value)
					return;
				label = value;
				RaiseOnPropertyChanged("Label");
				RaiseOnPropertyChanged("LabelColor");
				RaiseOnPropertyChanged("LabelBrush");
			}
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected void RaiseOnPropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		protected internal virtual bool SameDay { get { return AppointmentInterval.SameDay; } }
		protected internal virtual bool LongerThanADay { get { return AppointmentInterval.LongerThanADay; } }
		#endregion
		protected internal virtual void Initialize(TimeZoneHelper engine) {
			this.resource = ResourceBase.Empty;
			this.interval = TimeInterval.Empty;
			this.options = new AppointmentViewInfoOptions();
			this.status = AppointmentStatus.Empty;
			this.appointmentInterval = GetAppointmentInterval(engine);
		}
		public virtual bool IsLongTime() {
			return LongerThanADay || !SameDay;
		}
		protected internal virtual TimeInterval GetAppointmentInterval(TimeZoneHelper engine) {
			return ((IInternalAppointment)Appointment).GetInterval();
		}
		public string GetStartTimeText() {
			if (ContentCalculatorHelper == null)
				return String.Empty;
			return ContentCalculatorHelper.GetStartTimeText(this);
		}
		public string GetEndTimeText() {
			if (ContentCalculatorHelper == null)
				return String.Empty;
			return ContentCalculatorHelper.GetEndTimeText(this);
		}
		string GetLocation(Appointment appointment) {
			if (String.IsNullOrEmpty(appointment.Location))
				return String.Empty;
			return String.Format("({0})", appointment.Location);
		}
	}
	#endregion
	#region TimeLineAppointmentViewInfo
	public class TimeLineAppointmentViewInfo : AppointmentViewInfo {
		public TimeLineAppointmentViewInfo(Appointment appointment, TimeZoneHelper timeZoneEngine)
			: base(appointment, timeZoneEngine) {
		}
		public override bool IsLongTime() {
			return true;
		}
	}
	#endregion
	#region AppointmentControl
	public abstract class AppointmentControl {
		readonly SchedulerViewBase view;
		AppointmentIntermediateViewInfoCore intermediateViewInfo;
		bool shouldCopyFrom = true;
		bool isPermanentAppointment = true;
		IAppointmentFormatStringService appointmentFormatStringService = null;
		bool isStartInvisible;
		bool isEndInvisible;
		bool isDraggedAppointment;
		protected AppointmentControl(SchedulerViewBase view, AppointmentIntermediateViewInfoCore aptViewInfo, IAppointmentFormatStringService appointmentFormatStringService) {
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(aptViewInfo, "aptViewInfo");
			Guard.ArgumentNotNull(appointmentFormatStringService, "appointmentFormatStringService");
			this.view = view;
			this.intermediateViewInfo = aptViewInfo;
			this.appointmentFormatStringService = appointmentFormatStringService;
			CalculateIsStartInvisible();
			CalculateIsEndInvisible();
			IntermediateViewInfo.PropertyChanged += new PropertyChangedEventHandler(OnIntermediateViewInfoPropertyChanged);
		}
		public bool ShouldCopyFrom { get { return shouldCopyFrom; } set { shouldCopyFrom = value; } }
		public SchedulerViewBase View { get { return view; } }
		public bool IsStartInvisible {
			get { return isStartInvisible; }
			private set {
				if (IsStartInvisible == value)
					return;
				isStartInvisible = value;
				RaisePropertyChanged("IsStartInvisible");
			}
		}
		public bool IsEndInvisible {
			get { return isEndInvisible; }
			private set {
				if (IsEndInvisible == value)
					return;
				isEndInvisible = value;
				RaisePropertyChanged("IsEndInvisible");
			}
		}
		public bool ContainsStart { get { return ViewInfo.Interval.Start <= ViewInfo.Appointment.Start; } }
		public bool ContainsEnd { get { return ViewInfo.Interval.End >= ViewInfo.Appointment.End; } }
		public AppointmentViewInfo ViewInfo { get { return (AppointmentViewInfo)intermediateViewInfo.ViewInfo; } }
		internal AppointmentIntermediateViewInfoCore IntermediateViewInfo { get { return intermediateViewInfo; } }
		internal Appointment Appointment { get { return ViewInfo.Appointment; } }
		internal bool IsPermanentAppointment { get { return isPermanentAppointment; } set { isPermanentAppointment = value; } }
		internal bool IsDraggedAppointment { get { return isDraggedAppointment; } set { isDraggedAppointment = value; } }
		internal IAppointmentFormatStringService AppointmentFormatStringService { get { return appointmentFormatStringService; } }
		void OnIntermediateViewInfoPropertyChanged(object sender, PropertyChangedEventArgs e) {
			ShouldCopyFrom = true;
		}
		void CalculateIsStartInvisible() {
			DateTime visibleIntervalStart = View.InnerVisibleIntervals.Interval.Start;
			this.isStartInvisible = ViewInfo.Appointment.Start < visibleIntervalStart && ViewInfo.Interval.Start == visibleIntervalStart;
		}
		void CalculateIsEndInvisible() {
			DateTime visibleIntervalEnd = View.InnerVisibleIntervals.Interval.End;
			this.isEndInvisible = ViewInfo.Appointment.End > visibleIntervalEnd && ViewInfo.Interval.End == visibleIntervalEnd;
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected virtual void RaisePropertyChanged(string propertyName) {
			ShouldCopyFrom = true;
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	#endregion
	public class VerticalAppointmentControl : AppointmentControl {
		public VerticalAppointmentControl(SchedulerViewBase view, AppointmentIntermediateViewInfoCore aptViewInfo, IAppointmentFormatStringService appointmentFormatStringService)
			: base(view, aptViewInfo, appointmentFormatStringService) {
		}
	}
	#region HorizontalAppointmentControl
	public class HorizontalAppointmentControl : AppointmentControl {
		public HorizontalAppointmentControl(SchedulerViewBase view, AppointmentIntermediateViewInfoCore aptViewInfo, IAppointmentFormatStringService appointmentFormatStringService)
			: base(view, aptViewInfo, appointmentFormatStringService) {
		}
	}
	#endregion
	#region AppointmentControlCollection
	public class AppointmentControlCollection : AssignableCollection<AppointmentControl>, IAppointmentViewInfoCollection {
		public AppointmentControlCollection() {
		}
		public void AddRange(IAppointmentViewInfoCollection value) {
			base.AddRange((AppointmentControlCollection)value);
		}
	}
	#endregion
	#region AppointmentControlObservableCollection
	public class AppointmentControlObservableCollection : ObservableCollection<AppointmentControl>, IBatchUpdateable, IBatchUpdateHandler {
		bool deferredOnCollectionChanged = false;
		public AppointmentControlObservableCollection() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		public void SetRange(AppointmentControlCollection appointmentControlCollection) {
#if !SL
			CheckReentrancy();
#endif
			if (appointmentControlCollection.Count == 0 && Count == 0)
				return;
			BeginUpdate();
			try {
				Clear();
				SetRangeCore(appointmentControlCollection);
			}
			finally {
				EndUpdate();
			}
		}
		void SetRangeCore(AppointmentControlCollection appointmentControlCollection) {
			int count = appointmentControlCollection.Count;
			for (int i = 0; i < count; i++)
				Add(appointmentControlCollection[i]);
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (IsUpdateLocked)
				deferredOnCollectionChanged = true;
			else
				base.OnCollectionChanged(e);
		}
		#region IBatchUpdateHandler
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			this.deferredOnCollectionChanged = false;
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (this.deferredOnCollectionChanged)
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		#endregion
		#region IBatchUpdateable
		BatchUpdateHelper batchUpdateHelper;
		public BatchUpdateHelper BatchUpdateHelper {
			get { return batchUpdateHelper; }
		}
		public bool IsUpdateLocked {
			get { return batchUpdateHelper.IsUpdateLocked; }
		}
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		#endregion
	}
	#endregion
	#region VisualAppointmentControlCollection
	public class VisualAppointmentControlCollection : ObservableCollection<VisualAppointmentControl> {
	}
	#endregion
	#region AppointmentsLayoutResult
	public class AppointmentsLayoutResult : IAppointmentsLayoutResult {
		AppointmentControlCollection appointmentControls = new AppointmentControlCollection();
		public AppointmentControlCollection AppointmentControls { get { return appointmentControls; } }
		IAppointmentViewInfoCollection IAppointmentsLayoutResult.AppointmentViewInfos { get { return AppointmentControls; } }
		public virtual void Merge(IAppointmentsLayoutResult baseLayoutResult) {
			AppointmentsLayoutResult layoutResult = (AppointmentsLayoutResult)baseLayoutResult;
			AppointmentControls.AddRange(layoutResult.AppointmentControls);
		}
	}
	#endregion
	#region AppointmentIntermediateViewInfo
	public class AppointmentIntermediateViewInfo : AppointmentIntermediateViewInfoCore {
		public AppointmentIntermediateViewInfo(AppointmentViewInfo viewInfo)
			: base(viewInfo) {
			viewInfo.PropertyChanged += new PropertyChangedEventHandler(OnViewInfoPropertyChanged);
		}
		void OnViewInfoPropertyChanged(object sender, PropertyChangedEventArgs e) {
			RaisePropertyChanged(e.PropertyName);
		}
	}
	#endregion
	#region AppointmentIntermediateViewInfoComparer
	public class AppointmentIntermediateViewInfoComparer : AppointmentIntermediateViewInfoCoreComparer<AppointmentIntermediateViewInfoCore> {
		public AppointmentIntermediateViewInfoComparer(AppointmentBaseComparer aptComparer)
			: base(aptComparer) {
		}
	}
	#endregion
	public abstract class AppointmentBaseLayoutCalculator : AppointmentLayoutCalculatorCore<AppointmentIntermediateViewInfoCoreCollection, VisuallyContinuousCellsInfo, AppointmentsLayoutResult, AppointmentControlCollection> {
		protected AppointmentBaseLayoutCalculator(ISchedulerViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal new ISchedulerViewInfoBase ViewInfo { get { return (ISchedulerViewInfoBase)base.ViewInfo; } }
		protected internal abstract AppointmentControl CreateAppointmentControlInstance(AppointmentIntermediateViewInfoCore aptViewInfo);
		protected internal override TimeInterval GetAppointmentInterval(Appointment appointment) {
			return ViewInfo.View.Control.InnerControl.TimeZoneHelper.ToClientTime(((IInternalAppointment)appointment).CreateInterval(), appointment.TimeZoneId, true);
		}
		protected internal override void PreliminaryContentLayout(AppointmentIntermediateViewInfoCoreCollection intermediateResul, VisuallyContinuousCellsInfo cellsInfo) {
		}
		protected internal override AppointmentsLayoutResult CalculateLayoutCoreSingleCellsInfo(AppointmentBaseCollection appointments, VisuallyContinuousCellsInfo cellsInfo, bool isTwoPassLayout) {
			AppointmentIntermediateLayoutCalculatorCore intermediateCalculator = CreateIntermediateLayoutCalculator();
			AppointmentIntermediateViewInfoCoreCollection intermediateResult = (AppointmentIntermediateViewInfoCoreCollection)intermediateCalculator.CreateIntermediateViewInfoCollection(cellsInfo.Resource, cellsInfo.Interval);
			CalculateIntermediateViewInfos(intermediateResult, appointments, cellsInfo);
			PreliminaryContentLayout(intermediateResult, cellsInfo);
			LayoutViewInfos(cellsInfo, intermediateResult, isTwoPassLayout);
			AppointmentsLayoutResult result = SnapToCells(intermediateResult, cellsInfo);
			FinalContentLayout(result.AppointmentControls);
			return result;
		}
		protected internal override AppointmentsLayoutResult SnapToCells(AppointmentIntermediateViewInfoCoreCollection intermediateResult, VisuallyContinuousCellsInfo cellsInfo) {
			AppointmentsLayoutResult result = new AppointmentsLayoutResult();
			int count = intermediateResult.Count;
			for (int i = 0; i < count; i++) {
				AppointmentControl aptControl = SnapToCellsCore(intermediateResult[i], cellsInfo);
				result.AppointmentControls.Add(aptControl);
			}
			return result;
		}
		protected internal virtual AppointmentControl SnapToCellsCore(AppointmentIntermediateViewInfoCore intermediateViewInfo, VisuallyContinuousCellsInfo cellsInfo) {
			CalculateViewInfoProperties(intermediateViewInfo, cellsInfo);
			return CreateAppointmentControlInstance(intermediateViewInfo);
		}
		protected internal override void CalculateViewInfoBorders(AppointmentIntermediateViewInfoCore intermediateResult) {
			throw new NotImplementedException();
		}
		protected internal override void FinalContentLayout(AppointmentControlCollection aptControls) {
			int count = aptControls.Count;
			for (int i = 0; i < count; i++) {
				AppointmentControl aptControl = aptControls[i];
				SchedulerStorage storage = this.ViewInfo.View.Control.Storage;
				Appointment apt = aptControl.ViewInfo.Appointment;
				aptControl.ViewInfo.Label = storage != null ? storage.GetLabel(apt.LabelKey) : AppointmentLabel.Empty;
			}
		}
	}
	public abstract class HorizontalAppointmentLayoutCalculator : AppointmentBaseLayoutCalculator {
		protected HorizontalAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal abstract bool ShouldShowBorders(IAppointmentViewInfo aptViewInfo);
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new HorizontalAppointmentIntermediateLayoutCalculator(Scale, AppointmentsSnapToCells, ViewInfo.View.Control.InnerControl.TimeZoneHelper);
		}
		protected internal override AppointmentControl CreateAppointmentControlInstance(AppointmentIntermediateViewInfoCore aptViewInfo) {
			return new HorizontalAppointmentControl(ViewInfo.View, aptViewInfo, ViewInfo.GetFormatStringProvider());
		}
		protected internal override TimeScale CreateScale() {
			return new TimeScaleDay();
		}
		protected internal override void LayoutViewInfos(VisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfoCoreCollection intermediateResult, bool isTwoPassLayout) {
		}
		protected internal override void CalculateViewInfoBorders(AppointmentIntermediateViewInfoCore intermediateResult) {
			IAppointmentViewInfo viewInfo = intermediateResult.ViewInfo;
			bool showBorders = ShouldShowBorders(viewInfo);
			viewInfo.HasTopBorder = true;
			viewInfo.HasBottomBorder = true;
			viewInfo.HasLeftBorder = intermediateResult.HasStartBorder && showBorders;
			viewInfo.HasRightBorder = intermediateResult.HasEndBorder && showBorders;
		}
		protected internal override AppointmentsLayoutResult CalculateLayoutCoreSingleCellsInfo(AppointmentBaseCollection appointments, VisuallyContinuousCellsInfo cellsInfo, bool isTwoPassLayout) {
			AppointmentsLayoutResult result = base.CalculateLayoutCoreSingleCellsInfo(appointments, cellsInfo, isTwoPassLayout);
			AppointmentControlCollection appointmentControls = result.AppointmentControls;
			int count = appointmentControls.Count;
			int originalFirstCellIndex = cellsInfo.OriginalFirstCellIndex;
			for (int i = 0; i < count; i++) {
				appointmentControls[i].IntermediateViewInfo.FirstCellIndex += originalFirstCellIndex;
				appointmentControls[i].IntermediateViewInfo.LastCellIndex += originalFirstCellIndex;
			}
			return result;
		}
	}
	public class VerticalAppointmentLayoutCalculator : AppointmentBaseLayoutCalculator {
		public VerticalAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new VerticalAppointmentIntermediateLayoutCalculator((TimeScaleFixedInterval)Scale, AppointmentsSnapToCells, ViewInfo.View.Control.InnerControl.TimeZoneHelper);
		}
		protected internal override AppointmentControl CreateAppointmentControlInstance(AppointmentIntermediateViewInfoCore aptViewInfo) {
			return new VerticalAppointmentControl(ViewInfo.View, aptViewInfo, ViewInfo.GetFormatStringProvider());
		}
		protected internal override TimeScale CreateScale() {
			return new TimeScaleFixedInterval(((InnerDayView)ViewInfo.View.InnerView).TimeScale);
		}
		protected internal override void LayoutViewInfos(VisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfoCoreCollection intermediateResult, bool isTwoPassLayout) {
			VerticalAppointmentLayoutCalculatorHelper helper = new VerticalAppointmentLayoutCalculatorHelper();
			helper.LayoutViewInfos(CancellationToken.None, cellsInfo, intermediateResult);
		}
		protected internal override void CalculateViewInfoBorders(AppointmentIntermediateViewInfoCore intermediateResult) {
			IAppointmentViewInfo viewInfo = intermediateResult.ViewInfo;
			viewInfo.HasRightBorder = true;
			viewInfo.HasLeftBorder = true;
			viewInfo.HasTopBorder = intermediateResult.HasStartBorder;
			viewInfo.HasBottomBorder = intermediateResult.HasEndBorder;
		}
	}
	#region DayViewShortAppointmentLayoutCalculator
	public class DayViewShortAppointmentLayoutCalculator : VerticalAppointmentLayoutCalculator {
		public DayViewShortAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo)
			: base(viewInfo) {
		}
	}
	#endregion
	#region DayViewLongAppointmentLayoutCalculator
	public class DayViewLongAppointmentLayoutCalculator : HorizontalAppointmentLayoutCalculator {
		public DayViewLongAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal override AppointmentSnapToCellsMode AppointmentsSnapToCells { get { return AppointmentSnapToCellsMode.Always; } }
		protected internal override bool ShouldShowBorders(IAppointmentViewInfo aptViewInfo) {
			return true;
		}
	}
	#endregion
	#region WeekViewAppointmentLayoutCalculator
	public class WeekViewAppointmentLayoutCalculator : HorizontalAppointmentLayoutCalculator {
		public WeekViewAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal override bool ShouldShowBorders(IAppointmentViewInfo aptViewInfo) {
			return true;
		}
		protected internal override AppointmentBaseComparer CreateAppointmentComparer() {
			return new WeekViewAppointmentComparer();
		}
		protected override int CalculateMaxAppointmentsLevel(IAppointmentIntermediateViewInfoCoreCollection intermediateResult, VisuallyContinuousCellsInfo cellsInfo) {
			VisualizatoinStatistics visualizationStatistics = ViewInfo.View.Control.VisualizatoinStatistics;
			if (!visualizationStatistics.IsActual())
				return -1;			
			return visualizationStatistics.MaxCellHeight / visualizationStatistics.MinAppointmentHeight + 1;
		}		
	}
	#endregion
	#region MonthViewAppointmentLayoutCalculator
	public class MonthViewAppointmentLayoutCalculator : WeekViewAppointmentLayoutCalculator {
		public MonthViewAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo)
			: base(viewInfo) {
		}
	}
	#endregion
	#region HorizontalAppointmentIntermediateLayoutCalculator
	public class HorizontalAppointmentIntermediateLayoutCalculator : HorizontalAppointmentIntermediateLayoutCalculatorCore {
		public HorizontalAppointmentIntermediateLayoutCalculator(TimeScale scale, AppointmentSnapToCellsMode snapToCells, TimeZoneHelper timeZoneEngine)
			: base(scale, snapToCells, timeZoneEngine) {
		}
		protected internal override AppointmentIntermediateViewInfoCore CreateAppointmentIntermediateViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo) {
			return new AppointmentIntermediateViewInfo(new AppointmentViewInfo(apt, TimeZoneHelper));
		}
		protected internal override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection() {
			return new AppointmentIntermediateViewInfoCoreCollection();
		}
		public override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection(Resource resource, TimeInterval interval) {
			return new AppointmentIntermediateViewInfoCoreCollection(resource, interval);
		}
		protected internal override void AssignViewInfoIntervalAndOffset(AppointmentIntermediateViewInfoCore intermediateViewInfo, TimeInterval viewInfoInterval, int startRelativeOffset, int endRelativeOffset) {
			base.AssignViewInfoIntervalAndOffset(intermediateViewInfo, viewInfoInterval, startRelativeOffset, endRelativeOffset);
		}
	}
	#endregion
	#region VerticalAppointmentIntermediateLayoutCalculator
	public class VerticalAppointmentIntermediateLayoutCalculator : VerticalAppointmentIntermediateLayoutCalculatorCore {
		public VerticalAppointmentIntermediateLayoutCalculator(TimeScaleFixedInterval scale, AppointmentSnapToCellsMode snapToCells, TimeZoneHelper timeZoneEngine)
			: base(scale, snapToCells, timeZoneEngine) {
		}
		protected internal override AppointmentIntermediateViewInfoCore CreateAppointmentIntermediateViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo) {
			return new AppointmentIntermediateViewInfo(new AppointmentViewInfo(apt, TimeZoneHelper));
		}
		protected internal override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection() {
			return new AppointmentIntermediateViewInfoCoreCollection();
		}
		public override IAppointmentIntermediateViewInfoCoreCollection CreateIntermediateViewInfoCollection(Resource resource, TimeInterval interval) {
			return new AppointmentIntermediateViewInfoCoreCollection(resource, interval);
		}
	}
	#endregion
	#region VerticalAppointmentLayoutCalculatorHelper
	public class VerticalAppointmentLayoutCalculatorHelper : VerticalAppointmentLayoutCalculatorHelperCore<VisuallyContinuousCellsInfo> {
	}
	#endregion
	#region BusyInterval
	public class BusyInterval {
		static readonly BusyInterval empty = new BusyInterval();
		public static BusyInterval Empty { get { return empty; } }
		double start;
		double end;
		internal BusyInterval() {
		}
		public BusyInterval(double start, double end) {
			if (start >= end)
				Exceptions.ThrowArgumentException("start", start);
			if (start < 0)
				Exceptions.ThrowArgumentException("start", start);
			this.start = start;
			this.end = end;
		}
		public double Start { get { return start; } }
		public double End { get { return end; } }
		public bool ContainsExcludeEndBound(double value) {
			return this.start <= value && this.end > value;
		}
		public bool ContainsExcludeStartBound(double value) {
			return this.start < value && this.end >= value;
		}
		public BusyInterval Clone() {
			return new BusyInterval(Start, End);
		}
	}
	#endregion
	#region BusyIntervalCollection
	public class BusyIntervalCollection : DXCollection<BusyInterval> {
		public DateTime DateTime { get; set; }
		public void Assign(BusyIntervalCollection otherCollection) {
			foreach (BusyInterval interval in otherCollection)
				Add(interval.Clone());
		}
	}
	#endregion
	#region HorizontalAppointmentRelativePositionsCalculatorBase
	public abstract class HorizontalAppointmentRelativePositionsCalculatorBase {
		public abstract void CalculateAppointmentRelativePositions(HorizontalAppointmentLayoutInfoCollection viewInfos, int index);
	}
	#endregion
	#region HorizontalAppointmentAutoHeightRelativePositionsCalculator
	public class HorizontalAppointmentAutoHeightRelativePositionsCalculator : HorizontalAppointmentRelativePositionsCalculatorBase {
		double gapBetweenAppointments;
		public HorizontalAppointmentAutoHeightRelativePositionsCalculator(double gapBetweenAppointments) {
			this.gapBetweenAppointments = gapBetweenAppointments;
		}
		protected internal double GapBetweenAppointments { get { return gapBetweenAppointments; } }
		public virtual void CalculateAppointmentRelativePositions(HorizontalAppointmentLayoutInfoCollection infos) {
			CalculateAppointmentRelativePositions(infos, 0);
		}
		public override void CalculateAppointmentRelativePositions(HorizontalAppointmentLayoutInfoCollection viewInfos, int startIndex) {
			AppointmentIndexesCalculator indexCalculator = new AppointmentIndexesCalculator();
			AppointmentCellIndexesCollection previousCellIndexes = indexCalculator.CreateAppointmentCellIndexesCollection(viewInfos);
			indexCalculator.AdjustAppointmentCellIndexes(CancellationToken.None, viewInfos);
			CalculateAppointmentRelativePositionsCore(viewInfos, startIndex);
			indexCalculator.RestoreCellIndexes(CancellationToken.None, viewInfos, previousCellIndexes);
		}
		protected internal virtual void CalculateAppointmentRelativePositionsCore(HorizontalAppointmentLayoutInfoCollection collection, int startIndex) {
			BusyIntervalCollection[] busyIntervals = PrepareBusyIntervals(collection, startIndex);
			int count = collection.Count;
			for (int i = startIndex; i < count; i++) {
				HorizontalAppointmentLayoutInfo intermediateViewInfo = collection[i];
				double relativePosition = FindAvailableRelativePosition(intermediateViewInfo, busyIntervals);
				intermediateViewInfo.RelativePosition = relativePosition;
				MakeIntervalBusy(intermediateViewInfo, busyIntervals);
			}
		}
		protected internal virtual BusyIntervalCollection[] PrepareBusyIntervals(HorizontalAppointmentLayoutInfoCollection viewInfos, int startIndex) {
			BusyIntervalCollection[] busyIntervals = CreateBusyIntervals(2 * viewInfos.Count);
			for (int i = 0; i < startIndex; i++)
				MakeIntervalBusy(viewInfos[i], busyIntervals);
			return busyIntervals;
		}
		protected internal virtual BusyIntervalCollection[] CreateBusyIntervals(int cellsCount) {
			BusyIntervalCollection[] result = new BusyIntervalCollection[cellsCount];
			for (int i = 0; i < cellsCount; i++)
				result[i] = new BusyIntervalCollection();
			return result;
		}
		protected internal virtual double FindAvailableRelativePosition(HorizontalAppointmentLayoutInfo intermediateViewInfo, BusyIntervalCollection[] cellsBusyIntervals) {
			double relativePosition = 0;
			int from = intermediateViewInfo.CellIndexes.FirstCellIndex;
			int to = intermediateViewInfo.CellIndexes.LastCellIndex;
			int i = from;
			double height = GetHeight(intermediateViewInfo);
			while (i <= to) {
				BusyIntervalCollection busyIntervals = cellsBusyIntervals[i];
				BusyInterval interval = FindPossibleIntersectionInterval(busyIntervals, relativePosition);
				if ((interval == BusyInterval.Empty) || (interval.Start >= relativePosition + height))
					i++;
				else {
					relativePosition = interval.End;
					i = from;
				}
			}
			return relativePosition;
		}
		protected virtual double GetHeight(HorizontalAppointmentLayoutInfo intermediateViewInfo) {
			return intermediateViewInfo.Height;
		}
		public BusyInterval FindPossibleIntersectionInterval(BusyIntervalCollection busyIntervals, double value) {
			for (int i = 0; i < busyIntervals.Count; i++) {
				BusyInterval interval = busyIntervals[i];
				if ((interval.ContainsExcludeEndBound(value)) || (interval.Start > value))
					return new BusyInterval(interval.Start, interval.End);
			}
			return BusyInterval.Empty;
		}
		protected internal virtual void MakeIntervalBusy(HorizontalAppointmentLayoutInfo info, BusyIntervalCollection[] busyIntervals) {
			double top = info.RelativePosition;
			double bottom = GetBottom(info);
			for (int i = info.CellIndexes.FirstCellIndex; i <= info.CellIndexes.LastCellIndex; i++)
				AddBusyInterval(busyIntervals[i], new BusyInterval(top, bottom));
		}
		protected virtual double GetBottom(HorizontalAppointmentLayoutInfo info) {
			return info.RelativePosition + GetHeight(info) + GapBetweenAppointments;
		}
		protected internal virtual void AddBusyInterval(BusyIntervalCollection busyIntervals, BusyInterval busyInterval) {
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
	#region HorizontalAppointmentDragRelativePositionsCalculator
	public class HorizontalAppointmentDragRelativePositionsCalculator : HorizontalAppointmentAutoHeightRelativePositionsCalculator {
		double maxAppointmentBottom;
		public HorizontalAppointmentDragRelativePositionsCalculator()
			: base(0) {
		}
		protected override double GetHeight(HorizontalAppointmentLayoutInfo intermediateViewInfo) {
			return 1;
		}
		protected internal override void MakeIntervalBusy(HorizontalAppointmentLayoutInfo info, BusyIntervalCollection[] busyIntervals) {
			base.MakeIntervalBusy(info, busyIntervals);
			double bottom = GetBottom(info);
			if (bottom > maxAppointmentBottom)
				maxAppointmentBottom = bottom;
		}
		public override void CalculateAppointmentRelativePositions(HorizontalAppointmentLayoutInfoCollection viewInfos, int startIndex) {
			maxAppointmentBottom = 0;
			base.CalculateAppointmentRelativePositions(viewInfos, startIndex);
			int count = viewInfos.Count;
			double c = 1.0 / maxAppointmentBottom;
			for (int i = 0; i < count; i++) {
				HorizontalAppointmentLayoutInfo info = viewInfos[i];
				info.RelativePosition = info.RelativePosition * c;
				info.Height = GetHeight(info) * c;
			}
		}
	}
	#endregion
	#region TimeLineAppointmentLayoutCalculator
	public class TimelineViewAppointmentLayoutCalculator : HorizontalAppointmentLayoutCalculator {
		public TimelineViewAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected internal override AppointmentIntermediateLayoutCalculatorCore CreateIntermediateLayoutCalculator() {
			return new TimeLineAppointmentIntermediateLayoutCalculator(Scale, AppointmentsSnapToCells, ViewInfo.View.Control.InnerControl.TimeZoneHelper);
		}
		protected internal override TimeScale CreateScale() {
			return ((TimelineView)ViewInfo.View).GetBaseTimeScale();
		}
		protected internal override bool ShouldShowBorders(IAppointmentViewInfo viewInfo) {
			return true;
		}
	}
	#endregion
	#region TimeLineAppointmentIntermediateLayoutCalculator
	public class TimeLineAppointmentIntermediateLayoutCalculator : HorizontalAppointmentIntermediateLayoutCalculator {
		public TimeLineAppointmentIntermediateLayoutCalculator(TimeScale scale, AppointmentSnapToCellsMode snapToCells, TimeZoneHelper timeZoneEngine)
			: base(scale, snapToCells, timeZoneEngine) {
		}
		protected internal override AppointmentIntermediateViewInfoCore CreateAppointmentIntermediateViewInfo(Appointment apt, IVisuallyContinuousCellsInfoCore cellsInfo) {
			return new AppointmentIntermediateViewInfo(new TimeLineAppointmentViewInfo(apt, TimeZoneHelper));
		}
	}
	#endregion
}

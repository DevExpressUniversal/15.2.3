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
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Windows;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Internal.Implementations;
#if WPF
using DevExpress.Xpf.Scheduler;
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using System.Collections.Specialized;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public enum CellContainerType { Horizontal, Vertical };
	public class SelectedCellIndexesInterval {
		int startCellIndex;
		int endCellIndex;
		bool visibleFirstSelectedCell;
		bool visibleLastSelectedCell;
		public SelectedCellIndexesInterval(int startCellIndex, bool visibleFirstSelectedCell, int endCellIndex, bool visibleLastSelectedCell) {
			Guard.ArgumentNonNegative(startCellIndex, "startCellIndex");
			Guard.ArgumentNonNegative(startCellIndex, "endCellIndex");
			this.startCellIndex = startCellIndex;
			this.endCellIndex = endCellIndex;
			this.visibleFirstSelectedCell = visibleFirstSelectedCell;
			this.visibleLastSelectedCell = visibleLastSelectedCell;
		}
		public Resource Resource { get; set; }
		public TimeInterval Interval { get; set; }
		public int StartCellIndex { get { return startCellIndex; } }
		public int EndCellIndex { get { return endCellIndex; } }
		public bool VisibleFirstSelectedCell { get { return visibleFirstSelectedCell; } }
		public bool VisibleLastSelectedCell { get { return visibleLastSelectedCell; } }
		public override bool Equals(object obj) {
			SelectedCellIndexesInterval interval2 = obj as SelectedCellIndexesInterval;
			if (Object.ReferenceEquals(interval2, null))
				return false;
			return StartCellIndex == interval2.StartCellIndex &&
				EndCellIndex == interval2.EndCellIndex &&
				VisibleFirstSelectedCell == interval2.VisibleFirstSelectedCell &&
				VisibleLastSelectedCell == interval2.VisibleLastSelectedCell;
		}
		public override int GetHashCode() {
			return startCellIndex + (endCellIndex << 16);
		}
		public static bool operator ==(SelectedCellIndexesInterval interval1, SelectedCellIndexesInterval interval2) {
			if(Object.ReferenceEquals(interval1, interval2))
				return true;
			if(Object.ReferenceEquals(interval1, null) || Object.ReferenceEquals(interval2, null))
				return false;
			return interval1.Equals(interval2);
		}
		public static bool operator !=(SelectedCellIndexesInterval interval1, SelectedCellIndexesInterval interval2) {
			return !(interval1 == interval2);
		}
	}
	public interface ICellContainer {
		int CellCount { get; }
		Resource Resource { get; }
		CellContainerType ContainerType { get; }
		ITimeCell this[int index] { get; }		
		AppointmentControlCollection AppointmentControls { get; }
		AppointmentControlCollection DraggedAppointmentControls { get; }
		void FillAppointmentControls(AppointmentControlCollection allAppointments);
		void FillDraggedAppointmentControls(AppointmentControlCollection allAppointments);
		SelectedCellIndexesInterval SelectedCells { get; }
		SchedulerViewBase View { get; }
		void CalculateSelectionLayout(SchedulerViewSelection schedulerViewSelection);
		SchedulerControl SchedulerControl { get; }
		Dictionary<Appointment, FrameworkElement> AppointmentPresentersCache { get; }
		FrameworkElement GetAppointmentPresenter(Appointment apt);
	}	
	public interface ISchedulerViewInfoBase : ISupportAppointmentsBase {
		SchedulerViewBase View { get; }
		AppointmentControlCollection AppointmentControls { get; }
		AppointmentControlCollection DraggedAppointmentControls { get; }
		CellContainerCollection GetContainers();
		int GetResourceColorIndex(Resource resource);
		void Create();
		void UpdateSelection(SchedulerViewSelection newSelection);
		void UpdateAppointmentsSelection();
		event EventHandler SelectionChanged;
		event EventHandler AppointmentsSelectionChanged;
		event EventHandler Initialized;
		void EndCreate();
		IAppointmentFormatStringService GetFormatStringProvider();		
	}
	public abstract class ResourceCellBase : ITimeCell {
		readonly Resource resource;
		readonly ResourceBrushes brushes;
		protected ResourceCellBase(Resource resource, ResourceBrushes brushes) {
			Guard.ArgumentNotNull(resource, "resource");
			Guard.ArgumentNotNull(brushes, "brushes");
			this.resource = resource;
			this.brushes = brushes;
		}
		TimeInterval ITimeCell.Interval { get { return TimeInterval.Empty ; } }
		public Resource Resource { get { return resource; } }
		public ResourceBrushes Brushes { get { return brushes; } }
	}
	public class TimeCellBase : ResourceCellBase, ITimeCell {
		TimeInterval interval;
		public TimeCellBase(TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(resource, brushes) {
			Guard.ArgumentNotNull(interval, "interval");
			this.interval = interval;
		}
		TimeInterval ITimeCell.Interval { get { return Interval; } }
		public virtual TimeInterval Interval {
			get { return interval; }
			protected  set {
				interval = value;
			}
		}
	}
	public abstract class TimeCellWithContentBase : TimeCellBase {
		TimeCellContent content;
		protected TimeCellWithContentBase(TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(interval, resource, brushes) {
			this.content = new TimeCellContent(Interval, Resource);
		}
		public TimeCellContent Content {
			get { return content; }
			private set {
				content = value;
			}
		}
	}
	public class SingleDayViewInfo : IViewInfo {
		AssignableCollection<SingleResourceViewInfo> singleResourceViewInfoCollection;
		readonly DayView view;
		public SingleDayViewInfo(DayView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			TimeIndicatorVisibility = TimeIndicatorVisibility.TodayView;
		}
		public DayView View { get { return view; } }
		public AssignableCollection<SingleResourceViewInfo> SingleResourceViewInfoCollection {
			get { return singleResourceViewInfoCollection; }
			set {
				singleResourceViewInfoCollection = value;
			}
		}
		public DayViewColumn FirstCellContainer {
			get {
				return ((DayBasedSingleResourceViewInfo)singleResourceViewInfoCollection[0]).FirstCellContainer;
			}
		}
		public DayBasedSingleResourceViewInfo LastSingleResourceView {
			get {
				if (singleResourceViewInfoCollection.Count > 0) 
					return ((DayBasedSingleResourceViewInfo)singleResourceViewInfoCollection[singleResourceViewInfoCollection.Count -1]);
				return null;
			}
		}
		public TimeIndicatorVisibility TimeIndicatorVisibility { get; set; }
	}
	public class SingleResourceViewInfo : IViewInfo {
		#region Fields
		readonly SchedulerViewBase view;
		ResourceHeaderBase resourceHeader;
		CellContainerCollection cellContainers;
		NavigationButtonInfo prevNavButtonInfo;
		NavigationButtonInfo nextNavButtonInfo;
		#endregion
		public SingleResourceViewInfo(SchedulerViewBase view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.prevNavButtonInfo = new NavigationButtonInfo(view.Control);
			this.nextNavButtonInfo = new NavigationButtonInfo(view.Control);
		}
		#region Properties
		public SchedulerViewBase View { get { return view; } }
		public SchedulerControl Control { get { return view.Control; } }
		public NavigationButtonInfo PrevNavButtonInfo { get { return prevNavButtonInfo; } set { prevNavButtonInfo = value; } }
		public NavigationButtonInfo NextNavButtonInfo { get { return nextNavButtonInfo; } set { nextNavButtonInfo = value; } }
		public CellContainerCollection CellContainers {
			get { return cellContainers; }
			set {
				if(CellContainers == value)
					return;
				this.cellContainers = value;
			}
		}
		public ResourceHeaderBase ResourceHeader { 
			get {
				return resourceHeader;
			} 
			internal set {
				if(Object.Equals(ResourceHeader, value))
					return;
				this.resourceHeader = value;
			}  
		}
		#endregion
	}
	public class WeekBasedSingleResourceViewInfo : SingleResourceViewInfo {
		DayOfWeekHeaderCollection dayOfWeekHeaders;
		public WeekBasedSingleResourceViewInfo(SchedulerViewBase view)
			: base(view) {
		}
		public DayOfWeekHeaderCollection DayOfWeekHeaders {
			get { return dayOfWeekHeaders; }
			set {
				if(DayOfWeekHeaders == value)
					return;
				dayOfWeekHeaders = value;
			}
		}
		public override bool Equals(object obj) {
			WeekBasedSingleResourceViewInfo other = obj as WeekBasedSingleResourceViewInfo;
			if(Object.ReferenceEquals(other, null) || this.ResourceHeader == null || other.ResourceHeader == null)
				return false;
			return object.Equals((object)this.ResourceHeader.Resource.Id, (object)other.ResourceHeader.Resource.Id);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}	   
	}
	public class DayBasedSingleResourceViewInfo : SingleResourceViewInfo {
		ICellContainer horizontalCellContainer;
		public DayBasedSingleResourceViewInfo(SchedulerViewBase view)
			: base(view) {
		}
		public TimeIndicatorVisibility TimeIndicatorVisibility { get; set; }
		public CellContainerCollection VerticalCellContainers { get; set; }
		public ICellContainer HorizontalCellContainer {
			get { return horizontalCellContainer; }
			set {
				if(horizontalCellContainer == value)
					return;
				horizontalCellContainer = value;				
			} 
		}
		public ITimeCell LastHorizontalCell { get { return HorizontalCellContainer[HorizontalCellContainer.CellCount - 1]; } }
		public DayViewColumn FirstCellContainer { get; set; }
	}
	public class GanttViewSingleResourceViewInfo : SingleResourceViewInfo {
		public GanttViewSingleResourceViewInfo(SchedulerViewBase view)
			: base(view) {
		}
		public bool IsExpanded { get; set; }
		public int NestingLevel { get; set; }
		public SchedulerUICommandBase ExpandCommand { get; set; }
		public SchedulerUICommandBase CollapseCommand { get; set; }
	}
	public interface IViewInfo {
	}
	public abstract class ResourceHeaderBase : ResourceCellBase {
		TimeIntervalCollection intervals;
		protected ResourceHeaderBase(TimeInterval interval, Resource resource, ResourceBrushes brushes) : base(resource, brushes) {
			TimeIntervalCollection intervals = new TimeIntervalCollection();
			intervals.Add(interval);
			Initialize(intervals);
		}
		protected ResourceHeaderBase(TimeIntervalCollection intervals, Resource resource, ResourceBrushes brushes) : base(resource, brushes) {;
			Initialize(intervals);		   
		}
		void Initialize(TimeIntervalCollection intervals) {
			Guard.ArgumentNotNull(intervals, "intervals");
			this.intervals = intervals;
		}
		public TimeIntervalCollection Intervals { 
			get { return intervals; } 
			private set {
				if(object.Equals(Intervals, value))
					return;
				intervals = value;
			}
		}
		public TimeInterval Interval {
			get {
				if(Intervals.Count <= 0)
					return TimeInterval.Empty;
				return Intervals[0];
			}
			protected set {
			}
		}
	}
	public class HorizontalResourceHeader : ResourceHeaderBase {
		public HorizontalResourceHeader(TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(interval, resource, brushes) {
		}
		public HorizontalResourceHeader(TimeIntervalCollection intervals, Resource resource, ResourceBrushes brushes)
			: base(intervals, resource, brushes) {
		}
	}
	#region LeftTopCornerBase
	public abstract class LeftTopCornerBase : IViewInfo {
		#region Fields
		string caption;
		#endregion
		protected LeftTopCornerBase()
			: this(String.Empty) {
		}
		protected LeftTopCornerBase(string caption) {
			this.caption = caption;
		}
		#region Properties
		public string Caption { get { return caption; } }
		#endregion
	}
	#endregion
	#region LeftTopCorner
	public class LeftTopCorner : LeftTopCornerBase {
		public LeftTopCorner() {
		}
		public LeftTopCorner(string caption)
			: base(caption) {
		}
	}
	#endregion
	#region TimeRulerHeader
	public class TimeRulerHeader : LeftTopCornerBase {
		TimeRuler timeRuler;
		public TimeRulerHeader(TimeRuler timeRuler)
			: base(timeRuler.Caption) {
			this.timeRuler = timeRuler;
		}
		public TimeRuler TimeRuler { get { return timeRuler; } }
	}
	#endregion
	public abstract class SimpleTimeCellContainerBase<T> : ICellContainer where T : TimeCellBase {
		SelectedCellIndexesInterval selectedCells;
		AppointmentControlCollection appointmentControls = new AppointmentControlCollection();		
		AppointmentControlCollection draggedAppointmentControls = new AppointmentControlCollection();
		AssignableSchedulerViewCellBaseCollection<T> cells;
		Dictionary<Appointment, FrameworkElement> appointmentPresentersCache = new Dictionary<Appointment, FrameworkElement>();
		readonly SchedulerViewBase view;
		readonly SchedulerControl schedulerControl;
		protected SimpleTimeCellContainerBase(SchedulerViewBase view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.schedulerControl = view.Control;
			this.cells = CreateCellContainerCells();
		}
		public SchedulerControl SchedulerControl { get { return schedulerControl; } }
		public SchedulerViewBase View { get { return view; } }
		public abstract Resource Resource {
			get;
			internal set;
		}
		public abstract TimeInterval Interval {
			get;
			internal set;
		}
		public virtual AssignableSchedulerViewCellBaseCollection<T> Cells {
			get { return cells; }
			protected set {
				if(object.Equals(Cells, value))
					return;
				cells = value;				
			}
		}
		public int CellCount { get { return Cells.Count; } }
		#region ICellContainer Members
		public virtual CellContainerType ContainerType { get { return CellContainerType.Vertical; } }
		public AppointmentControlCollection AppointmentControls { 
			get { return appointmentControls; }
			set {
				if(object.Equals(AppointmentControls, value))
					return;
				appointmentControls = value;				
			}
		}
		public AppointmentControlCollection DraggedAppointmentControls {
			get { return draggedAppointmentControls; }
			set {
				if (object.Equals(DraggedAppointmentControls, value))
					return;
				draggedAppointmentControls = value;				
			}
		}
		public SelectedCellIndexesInterval SelectedCells {
			get { return selectedCells; }
			internal set {
				if(selectedCells == value)
					return;
				selectedCells = value;
			}
		}
		public void FillAppointmentControls(AppointmentControlCollection allAppointments) {
			AppointmentControls.Clear();			
			foreach(AppointmentControl control in allAppointments) {
				if(FitAppointmentControl(control)) {
					AppointmentControls.Add(control);					
				}
			}
		}
		public void FillDraggedAppointmentControls(AppointmentControlCollection draggedAppointments) {
			DraggedAppointmentControls.Clear();
			foreach (AppointmentControl control in draggedAppointments) {
				if (FitAppointmentControl(control)) {
					control.IsDraggedAppointment = true;
					DraggedAppointmentControls.Add(control);
				}
			}
		}
		public virtual void CalculateSelectionLayout(SchedulerViewSelection schedulerViewSelection) {
			if(!ResourceBase.MatchIds(schedulerViewSelection.Resource, (XtraScheduler.Resource)this.Resource) || this.schedulerControl.SelectedAppointments.Count > 0) {
				this.SelectedCells = null;
				return;
			}
			CalculateSelectionLayoutCore(schedulerViewSelection);
		}
		protected internal virtual void CalculateSelectionLayoutCore(SchedulerViewSelection schedulerViewSelection) {
			int firstSelectedCellIndex = FindFirstSelectedCellIndex(schedulerViewSelection.Interval.Start);
			int lastSelectedCellIndex = FindLastSelectedCellIndex(schedulerViewSelection.Interval.End);	   
			SetSelectedCells(schedulerViewSelection, firstSelectedCellIndex, lastSelectedCellIndex);
		}
		protected virtual void SetSelectedCells(SchedulerViewSelection schedulerViewSelection, int firstSelectedCellIndex, int lastSelectedCellIndex) {
			if (firstSelectedCellIndex < 0 || lastSelectedCellIndex < 0)
				SelectedCells = null;
			else {
				bool visibleFirstSelectedCell = Cells[firstSelectedCellIndex].Interval.Start <= schedulerViewSelection.Interval.Start;
				bool visibleLastSelectedCell = Cells[lastSelectedCellIndex].Interval.End >= schedulerViewSelection.Interval.End;
				SelectedCellIndexesInterval selectedCells = new SelectedCellIndexesInterval(firstSelectedCellIndex, visibleFirstSelectedCell, lastSelectedCellIndex, visibleLastSelectedCell);
				selectedCells.Interval = schedulerViewSelection.Interval;
				selectedCells.Resource = schedulerViewSelection.Resource;
				SelectedCells = selectedCells;
			}
		}
		protected int FindFirstSelectedCellIndex(DateTime dateTime) {
			int count = Cells.Count;
			for(int i = 0; i < count; i++) {
				if(Cells[i].Interval.End > dateTime)
					return i;
			}
			return -1;	
		}
		protected int FindLastSelectedCellIndex(DateTime dateTime) {
			int count = Cells.Count;
			for(int i = count - 1; i >= 0; i--) {
				if(Cells[i].Interval.Start < dateTime)
					return i;
			}
			return -1;
		}
		#endregion
		#region ICellContainer Members
		CellContainerType ICellContainer.ContainerType { get { return ContainerType; } }
		int ICellContainer.CellCount { get { return Cells.Count; } }
		Resource ICellContainer.Resource { get { return Resource; } }
		ITimeCell ICellContainer.this[int index] { get { return Cells[index]; } }
		AppointmentControlCollection ICellContainer.AppointmentControls { get { return AppointmentControls; } }
		void ICellContainer.FillAppointmentControls(AppointmentControlCollection allAppointments) {
			FillAppointmentControls(allAppointments);
		}
		protected virtual bool FitAppointmentControl(AppointmentControl control) {
			if(!(control is VerticalAppointmentControl))
				return false;
			return this.FitAppointmentControlInterval(control.ViewInfo.Interval) && ResourceBase.InternalMatchIds((object)control.ViewInfo.Resource.Id, (object)this.Resource.Id)
				&& this.FitAppointmentControlDuration(control);
		}
		protected virtual bool FitAppointmentControlInterval(TimeInterval appointmentInterval) {
			return Interval.Contains(appointmentInterval);
		}
		protected virtual bool FitAppointmentControlDuration(AppointmentControl control) {
			return control.ViewInfo.Appointment.Duration < DateTimeHelper.DaySpan;
		}
		SelectedCellIndexesInterval ICellContainer.SelectedCells {
			get { return SelectedCells; }
		}
		void ICellContainer.CalculateSelectionLayout(SchedulerViewSelection schedulerViewSelection) {
			CalculateSelectionLayout(schedulerViewSelection);
		}
		Dictionary<Appointment, FrameworkElement> ICellContainer.AppointmentPresentersCache { get { return appointmentPresentersCache; } }
		FrameworkElement ICellContainer.GetAppointmentPresenter(Appointment apt) {
			return GetAppointmentPresenterCore(apt);
		}
		#endregion
		protected internal void ResetSelectedCells() {
			SelectedCells = null;
		}
		protected virtual FrameworkElement GetAppointmentPresenterCore(Appointment apt) {
			return appointmentPresentersCache.ContainsKey(apt) ? appointmentPresentersCache[apt] : null;
		}
		protected void SetCells(AssignableSchedulerViewCellBaseCollection<T> cells) {
			this.cells = cells;
		}
		public abstract AssignableSchedulerViewCellBaseCollection<T> CreateCellContainerCells();
	}
	public abstract class CommonTimeCellContainerBase<T> : SimpleTimeCellContainerBase<T> where T : TimeCellWithContentBase {
		TimeInterval interval;
		Resource resource;
		ResourceBrushes brushes;
		protected CommonTimeCellContainerBase(SchedulerViewBase view, TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(view) {
			Guard.ArgumentNotNull(interval, "interval");
			Guard.ArgumentNotNull(resource, "resource");
			Guard.ArgumentNotNull(brushes, "brushes");
			this.interval = interval;
			this.resource = resource;
			this.brushes = brushes;
		}
		public override TimeInterval Interval {
			get { return interval; }
			internal set {
				if(object.Equals(interval, value))
					return;
				interval = value;
			}
		}
		public override Resource Resource {
			get { return resource; }
			internal set {
				if(object.Equals(resource, value))
					return;
				resource = value;
			}
		}
		public ResourceBrushes Brushes {
			get { return brushes; }
			set {
				if(object.Equals(Brushes, value))
					return;
				brushes = value;
			}
		}
	}
	public abstract class TimeCellContainerBase : CommonTimeCellContainerBase<TimeCell>, IAppointmentViewInfoContainer<AppointmentControlCollection> {
		WorkTimeInfoCalculatorBase workTimeCalculator;
		protected TimeCellContainerBase(SchedulerViewBase view, TimeInterval interval, Resource resource, ResourceBrushes brushes) 
			: base(view, interval, resource, brushes){
		}
		#region Properties
		public new TimeCellCollection Cells { 
			get { return (TimeCellCollection)base.Cells; } 
		}
		public WorkTimeInfoCalculatorBase WorkTimeCalculator { get { return workTimeCalculator; } }
		public AppointmentControlCollection AppointmentViewInfos {
			get { throw new NotImplementedException(); }
		}
		#endregion
		public virtual void Initialize() {
			this.workTimeCalculator = CreateWorkTimeCalculator();
			PopulateCellCollection(View);
		}
		protected abstract WorkTimeInfoCalculatorBase CreateWorkTimeCalculator();
		protected abstract internal void PopulateCellCollection(SchedulerViewBase view);
		public override AssignableSchedulerViewCellBaseCollection<TimeCell> CreateCellContainerCells() {
			return new TimeCellCollection();
		}
		#region IAppointmentViewInfoContainer<AppointmentControlCollection> Members
		#endregion
	}
}

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
using System.Collections.Specialized;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Implementations;
#if WPF || SL
using DevExpress.Xpf.Scheduler.Native;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class DateHeader : ISelectableIntervalViewInfo {
		#region Fields
		string caption;
		TimeInterval interval;
		ResourceBaseCollection resources;
		bool isAlternate;
		StringCollection dateFormats;
		SchedulerViewBase view;
		#endregion
		public DateHeader(SchedulerViewBase view, TimeInterval interval, Resource resource) {
			Guard.ArgumentNotNull(resource, "resource");
			ResourceBaseCollection resources = new ResourceBaseCollection();
			resources.Add(resource);
			Initialize(view, interval, resources);
		}
		public DateHeader(SchedulerViewBase view, TimeInterval interval, ResourceBaseCollection resources) {
			Initialize(view, interval, resources);
		}
		#region Properties
		public TimeInterval Interval { 
			get { return interval; }
			private set {
				interval = value;
			}
		}
		public ResourceBaseCollection Resources { 
			get { return resources;}
			private set {
				resources = value;
			} 
		}
		public Resource Resource { get { return Resources[0]; } }
		public virtual bool IsAlternate { 
			get { return isAlternate; }
			private set {
				isAlternate = value;
			}
		}
		public StringCollection DateFormats {
			get { return dateFormats; }
			set {
				dateFormats = value;
			}
		}
		public string Caption {
			get { return caption; }
			set {
				if(value == null)
					return;
				if (caption == value)
					return;
				this.caption = value;
			}
		}
		public virtual SchedulerHitTest HitTestType { get { return SchedulerHitTest.DayHeader; } }
		public virtual bool Selected { get { return false; } }
		#endregion
		void Initialize(SchedulerViewBase view, TimeInterval interval, ResourceBaseCollection resources) {
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(interval, "interval");
			Guard.ArgumentNotNull(resources, "resources");
			Guard.ArgumentPositive(resources.Count, "resources.Count");
			this.view = view;
			this.interval = interval;
			this.resources = resources;
			this.isAlternate = IsAlternateHeader();
			DateFormats = CalculateDateFormats(interval);
		}
		protected virtual StringCollection CalculateDateFormats(TimeInterval interval) {
			if (interval.Start.Month == 1 && interval.Start.Day == 1)
				return DateTimeFormatHelper.GenerateFormatsWithYearForJanuary();
			else
				return DateTimeFormatHelper.GenerateFormatsWithoutYearAndWeekDay();
		}
		public SchedulerViewBase View { get { return view; } }
		protected internal virtual bool IsAlternateHeader() {
			if (View == null)
				return false;
			return XpfSchedulerUtils.IsTodayDate(View.Control.InnerControl.TimeZoneHelper, Interval.Start);
		}		
	}
	public class DateCellHeader : DateHeader {
		public DateCellHeader(SchedulerViewBase view, TimeInterval interval, Resource resource)
			: base(view, interval, resource) {
		}
		public DateCellHeader(SchedulerViewBase view, TimeInterval interval, ResourceBaseCollection resources)
			: base(view, interval, resources) {
		}
	}
	public class VerticalDateCellHeader : DateCellHeader {
		public VerticalDateCellHeader(SchedulerViewBase view, TimeInterval interval, Resource resource)
			: base(view, interval, resource) {
		}
		public VerticalDateCellHeader(SchedulerViewBase view, TimeInterval interval, ResourceBaseCollection resources)
			: base(view, interval, resources) {
		}
		protected override StringCollection CalculateDateFormats(TimeInterval interval) {
			return DateTimeFormatHelper.GenerateFormatsWithoutYear();
		}
	}
	public class TimeCellContent : ITimeCell, ISelectableIntervalViewInfo {
		#region Fields
		TimeInterval interval;
		Resource resource;
		#endregion
		public TimeCellContent(TimeInterval interval, Resource resource) {
			Guard.ArgumentNotNull(interval, "interval");
			Guard.ArgumentNotNull(resource, "resource");
			this.interval = interval;
			this.resource = resource;
		}
		#region Properties
		public TimeInterval Interval { 
			get { return interval; }
			private set {
				interval = value;
			}
		}
		public Resource Resource { 
			get { return resource; }
			private set {
				resource = value;
			}
		}
		#endregion
		#region ISelectableIntervalViewInfo Members
		bool ISelectableIntervalViewInfo.Selected { get { return false; } }
		SchedulerHitTest ISelectableIntervalViewInfo.HitTestType { get { return SchedulerHitTest.Cell; } }
		#endregion
	}
	public class DateCell : TimeCellWithContentBase {
		#region Fields
		DateCellHeader header;
		readonly SchedulerViewBase view;
		#endregion
		public DateCell(SchedulerViewBase view, TimeInterval interval, Resource resource, ResourceBrushes brushes)
			: base(interval, resource, brushes) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.header = CreateHeader(view);
		}
		#region Properties
		public SchedulerViewBase View { get { return view; } }
		public DateCellHeader Header {
			get { return header; }
			private set {
				header = value;
			}
		}
		#endregion
		protected virtual DateCellHeader CreateHeader(SchedulerViewBase view) {
			return new DateCellHeader(view, Interval, Resource);
		}				
	}
	public class VerticalDateCell : DateCell {
		public VerticalDateCell(SchedulerViewBase view, TimeInterval interval, Resource resource, ResourceBrushes brushes) 
			: base(view, interval, resource, brushes) { 
		}
		protected override DateCellHeader CreateHeader(SchedulerViewBase view) {
			return new VerticalDateCellHeader(view, Interval, Resource);
		}				
	}
	public class DateCellCollection : AssignableSchedulerViewCellBaseCollection<DateCell> {
	}
	#region WeekBase (abstract class)
	public abstract class WeekBase : CommonTimeCellContainerBase<DateCell> {
		#region Fields
		bool compressWeekEnd;
		bool showWeekend;
		int compressedIndex = -1;
		DateTime[] dates;
		WeekBase selectedCellsSource;
		bool isCompressed;
		#endregion
		protected WeekBase(SchedulerViewBase view, TimeInterval interval, Resource resource, DateTime[] dates, bool compressWeekend, bool showWeekend, ResourceBrushes brushes) 
			: base(view, interval, resource, brushes) {
			Guard.ArgumentNotNull(dates, "dates");
			this.dates = dates;
			this.compressWeekEnd = compressWeekend;
			this.showWeekend = showWeekend;
			this.selectedCellsSource = this;
			this.isCompressed = compressWeekend && showWeekend;
			PopulateCells();
			CalculateCompressedIndex();
		}
		#region Properties
		public int CompressedIndex {
			get { return compressedIndex; }
			private set {
				if (CompressedIndex == value)
					return;
				compressedIndex = value;
				ResetSelectedCells(); 
			}
		}
		public new DateCellCollection Cells { get { return (DateCellCollection)base.Cells; } }
		public bool IsCompressed {
			get { return isCompressed; }
			set {
				if (IsCompressed == value)
					return;
				isCompressed = value;
				ResetSelectedCells(); 
			}
		}
		protected AppointmentControlCollection InternalAppointmentControls {
			get { return base.AppointmentControls; }
		}
		protected SelectedCellIndexesInterval InternalSelectedCells {
			get { return base.SelectedCells; }
		}
		#endregion
		private void CalculateCompressedIndex() {
			this.compressedIndex = DateTimeHelper.FindDayOfWeekIndex(dates, DayOfWeek.Saturday);
		}
		protected override bool FitAppointmentControl(AppointmentControl control) {
			return base.Interval.Contains(control.ViewInfo.Interval) && ResourceBase.InternalMatchIds((object)control.ViewInfo.Resource.Id, (object)this.Resource.Id);	
		}
		public override AssignableSchedulerViewCellBaseCollection<DateCell> CreateCellContainerCells() {
			return new DateCellCollection();
		}
		void PopulateCells() {
			int count = dates.Length;
			for(int i = 0; i < count; i++) {
				TimeInterval interval = new TimeInterval(dates[i], DateTimeHelper.DaySpan);
				Cells.Add(CreateDateCell(interval, Resource));
			}
		}
		protected internal abstract DateCell CreateDateCell(TimeInterval interval, Resource resource);
	}
	#endregion
	#region HorizontalWeek
	public class HorizontalWeek : WeekBase {
		public HorizontalWeek(SchedulerViewBase view, TimeInterval interval, Resource resource, DateTime[] dates, bool compressWeekend, bool showWeekend, ResourceBrushes brushes)
			: base(view, interval, resource, dates, compressWeekend, showWeekend, brushes) {
		}
		protected internal override DateCell CreateDateCell(TimeInterval interval, Resource resource) {
			return new DateCell(this.View, interval, resource, Brushes);
		}
		public override bool Equals(object obj) {
			HorizontalWeek other = obj as HorizontalWeek;
			if (Object.ReferenceEquals(other, null))
				return false;
			return Interval.Equals(other.Interval) && Resource.Id == other.Resource.Id;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
	#region WeekViewInfoBase (abstract class)
	public abstract class WeekViewInfoBase : ViewInfoBase<WeekBase> {
		#region Fields
		DayOfWeek[] weekDays;
		DayOfWeekHeaderCollection dayOfWeekHeaders;
		#endregion
		protected WeekViewInfoBase(WeekViewBase view)
			: base(view) {
		}
		#region Properties
		public new WeekViewBase View { get { return (WeekViewBase)base.View; } }
		protected internal DayOfWeek[] WeekDays { get { return weekDays; } }
		public DayOfWeekHeaderCollection DayOfWeekHeaders {
			get { return dayOfWeekHeaders; }
			private set {
				dayOfWeekHeaders = value;
			}
		}
		protected virtual bool CreateTopDayOfWeekHeader { get { return false; } }
		#endregion
		protected internal override IViewInfo CreateAdditionalViewElements() {
			return null;
		}
		protected internal override void Initialize() {
			base.Initialize();
			this.weekDays = CreateWeekDays();
		}
		public override void Create() {
			base.Create();
			if(CreateTopDayOfWeekHeader) {
				SchedulerColorSchema schema = new SchedulerColorSchema();
				ResourceBrushes brushes = new ResourceBrushes(schema);
				this.dayOfWeekHeaders = CreateDayOfWeekHeaders(brushes, ResourceBase.Empty);
			}
		}
		protected internal virtual DayOfWeek[] CreateWeekDays() {
			DateTime[] dates = GetVisibleDates(VisibleIntervals[0]);
			int count = dates.Length;
			DayOfWeek[] result = new DayOfWeek[count];
			for (int i = 0; i < count; i++)
				result[i] = dates[i].DayOfWeek;
			return result;
		}
		protected internal virtual DateTime[] GetVisibleDates(TimeInterval interval) {
			List<DateTime> dates = new List<DateTime>();
			DateTime date = interval.Start;
			while (date < interval.End) {
				if (IsDayOfWeekVisible(date.DayOfWeek))
					dates.Add(date);
				date += DateTimeHelper.DaySpan;
			}
			return dates.ToArray();
		}
		protected internal virtual bool IsDayOfWeekVisible(DayOfWeek dayOfWeek) {
			bool showWeekend = ((InnerWeekView)View.InnerView).ShowWeekendInternal;
			if (showWeekend)
				return true;
			else
				return dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
		}
		protected internal override WeekBase CreateCellContainer(TimeInterval interval, Resource resource, ResourceBrushes brushes) {
			DateTime[] dates = GetVisibleDates(interval);
			WeekBase week = CreateWeekCore(interval, resource, dates, brushes);
			return week;
		}
		protected internal abstract WeekBase CreateWeekCore(TimeInterval interval, Resource resource, DateTime[] dates, ResourceBrushes brushes);
		protected internal virtual DayOfWeekHeaderCollection CreateDayOfWeekHeaders(ResourceBrushes brushes, Resource resource) {
			DayHeaderContainer headers = new DayHeaderContainer(WeekDays, View, resource, brushes);
			return headers.DayHeaders;
		}
		protected internal override SingleResourceViewInfo CreateSingleResourceViewInfo() {
			return new WeekBasedSingleResourceViewInfo(this.View);
		}		
	}
	#endregion
	#region WeekViewGroupByNone
	public class WeekViewGroupByNone : WeekViewInfoBase {
		public WeekViewGroupByNone(WeekViewBase view)
			: base(view) {
		}
		public override ResourceBaseCollection Resources { get { return WeekViewInfoBase.EmptyResourceCollection; } }
		protected internal override ResourceHeaderBase CreateResourceHeader(Resource resource, ResourceBrushes brushes) {
			return null;
		}
		protected internal override WeekBase CreateWeekCore(TimeInterval interval, Resource resource, DateTime[] dates, ResourceBrushes brushes) {
			return new VerticalWeek(this.View, interval, resource, dates, brushes);
		}
		public override int GetResourceColorIndex(Resource resource) {
			return 0;
		}
	}
	#endregion
	#region WeekViewGroupByResource
	public class WeekViewGroupByResource : WeekViewInfoBase {
		public WeekViewGroupByResource(WeekViewBase view)
			: base(view) {
		}
		protected internal override WeekBase CreateWeekCore(TimeInterval interval, Resource resource, DateTime[] dates, ResourceBrushes brushes) {
			return new VerticalWeek(this.View, interval, resource, dates, brushes);
		}
	}
	#endregion
	#region WeekViewGroupByDate
	public class WeekViewGroupByDate : WeekViewInfoBase {
		public WeekViewGroupByDate(WeekViewBase view)
			: base(view) {
		}
		protected override bool CreateTopDayOfWeekHeader { get { return true; } }
		protected internal override WeekBase CreateWeekCore(TimeInterval interval, Resource resource, DateTime[] dates, ResourceBrushes brushes) {
			return new HorizontalWeek(this.View, interval, resource, dates, true, true, brushes);
		}
	}
	#endregion
	#region MonthViewGroupByNone
	public class MonthViewGroupByNone : WeekViewGroupByNone {
		public MonthViewGroupByNone(MonthView view)
			: base(view) {
		}
		protected override bool CreateTopDayOfWeekHeader {
			get { return true; }
		}
		public new MonthView View { get { return (MonthView)base.View; } }
		protected internal override WeekBase CreateWeekCore(TimeInterval interval, Resource resource, DateTime[] dates, ResourceBrushes brushes) {
			InnerMonthView innerView = (InnerMonthView)View.InnerView;
			return new HorizontalWeek(this.View, interval, resource, dates, innerView.CompressWeekendInternal, innerView.ShowWeekendInternal, brushes);
		}
	}
	#endregion
	#region MonthViewGroupByResource
	public class MonthViewGroupByResource : WeekViewGroupByResource {
		public MonthViewGroupByResource(MonthView view)
			: base(view) {
		}
		protected internal override WeekBase CreateWeekCore(TimeInterval interval, Resource resource, DateTime[] dates, ResourceBrushes brushes) {
			InnerMonthView innerView = (InnerMonthView)View.InnerView;
			return new HorizontalWeek(this.View, interval, resource, dates, innerView.CompressWeekendInternal, innerView.ShowWeekendInternal, brushes);
		}
		protected internal override SingleResourceViewInfo CreateSingleResourceView(Resource resource, TimeIntervalCollection visibleIntervals, ResourceBrushes brushes) {
			WeekBasedSingleResourceViewInfo viewInfo = (WeekBasedSingleResourceViewInfo)base.CreateSingleResourceView(resource, visibleIntervals, brushes);
			viewInfo.DayOfWeekHeaders = CreateDayOfWeekHeaders(brushes, resource);
			return viewInfo;
		}
	}
	#endregion
	#region MonthViewGroupByDate
	public class MonthViewGroupByDate : WeekViewGroupByDate {
		public MonthViewGroupByDate(MonthView view)
			: base(view) {
		}
		protected internal override WeekBase CreateWeekCore(TimeInterval interval, Resource resource, DateTime[] dates, ResourceBrushes brushes) {
			InnerMonthView innerView = (InnerMonthView)View.InnerView;
			return new HorizontalWeek(this.View, interval, resource, dates, innerView.CompressWeekendInternal, innerView.ShowWeekendInternal, brushes);
		}
		protected override bool CreateTopDayOfWeekHeader {
			get {
				return true;
			}
		}
	}
	#endregion
	#region VerticalWeek
	public class VerticalWeek : WeekBase {
		public VerticalWeek(SchedulerViewBase view, TimeInterval interval, Resource resource, DateTime[] dates, ResourceBrushes brushes)
			: base(view, interval, resource, dates, true, true, brushes) {
		}
		protected internal override DateCell CreateDateCell(TimeInterval interval, Resource resource) {
			return new VerticalDateCell(this.View, interval, resource, Brushes);
		}
	}
	#endregion
}

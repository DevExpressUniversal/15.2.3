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
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Reporting.Native;
using System.Drawing;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Reporting {
	public enum ColumnArrangementMode { Ascending, Descending };
	#region ReportViewCollection
	public class ReportViewCollection : NotificationCollection<ReportViewBase> {
		public ReportViewCollection() {
		}
		public void AddRange(ReportViewBase[] items) {
			base.AddRange(items);
		}
	}
	#endregion
	#region ReportViewManager
	public class ReportViewManager : IDisposable {
		ReportViewCollection views;
		XtraSchedulerReport report;
		public ReportViewManager(XtraSchedulerReport report) {
			if (report == null)
				Exceptions.ThrowArgumentNullException("report");
			this.report = report;
			this.views = CreateViewCollection();
			SubscribeViewCollectionEvents();
		}
		#region Properties
		protected internal XtraSchedulerReport Report { get { return report; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ReportViewCollection Views { get { return views; } }
		#endregion
		protected virtual ReportViewCollection CreateViewCollection() {
			return new ReportViewCollection();
		}
		#region SubscribeViewCollectionEvents
		protected internal virtual void SubscribeViewCollectionEvents() {
			if (Views != null)
				Views.CollectionChanged += new CollectionChangedEventHandler<ReportViewBase>(OnViewCollectionChanged);
		}
		#endregion
		#region UnsubscribeViewsEvents
		protected internal virtual void UnsubscribeViewCollectionEvents() {
			if (Views != null)
				Views.CollectionChanged -= new CollectionChangedEventHandler<ReportViewBase>(OnViewCollectionChanged);
		}
		#endregion
		protected virtual void OnViewCollectionChanged(object sender, CollectionChangedEventArgs<ReportViewBase> e) {
			if (e.Action == CollectionChangedAction.EndBatchUpdate) {
				Views.ForEach(SubscribeViewEvents);
				UpdateSchedulerAdapterForAll();
			}
			if (e.Action == CollectionChangedAction.Add) {
				SubscribeViewEvents(e.Element);
				UpdateViewAdapter(e.Element);
			}
			if (e.Action == CollectionChangedAction.Remove) {
				UnsubscribeViewEvents(e.Element);
				ClearViewAdapter(e.Element);
			}
		}
		#region SubscribeViewEvents
		protected virtual void SubscribeViewEvents(ReportViewBase view) {
			if (view != null) {
				view.Changed += new ReportControlStateChangedEventHandler(OnViewChanged);
				view.Disposed += new EventHandler(OnViewDisposed);
			}
		}
		void OnViewDisposed(object sender, EventArgs e) {
			ReportViewBase view = (ReportViewBase)sender;
			Views.Remove(view);
		}
		#endregion
		#region UnsubscribeViewEvents
		protected virtual void UnsubscribeViewEvents(ReportViewBase view) {
			if (view != null) {
				view.Changed -= new ReportControlStateChangedEventHandler(OnViewChanged);
				view.Disposed -= new EventHandler(OnViewDisposed);
			}
		}
		#endregion
		protected internal void ApplyChangesForAll(ReportControlChangeType change) {
			int count = Views.Count;
			for (int i = 0; i < count; i++) {
				ApplyChanges(Views[i], change);
			}
		}
		protected void ApplyChanges(IReportControlChangeTarget target, ReportControlChangeType change) {
			Report.ApplyChanges(target, change);
		}
		protected virtual void OnViewChanged(object sender, ReportControlStateChangedEventArgs e) {
			ApplyChanges((IReportControlChangeTarget)sender, e.Change);
		}
		protected void AssignViewProperties(ReportViewBase view, XtraSchedulerReport report) {
			view.SchedulerAdapter = report != null ? report.ActualSchedulerAdapter : null;
		}
		public void ClearViews() {
			int count = views.Count;
			for (int i = 0; i < count; i++) {
				ReportViewBase view = Views[i];
				UnsubscribeViewEvents(view);
				ClearViewAdapter(view);
			}
			Views.Clear();
		}
		#region Dispose
		public void Dispose() {
			ClearViews();
			UnsubscribeViewCollectionEvents();
			views = null;
		}
		#endregion
		protected virtual void UpdateViewAdapter(ReportViewBase view) {
			view.SchedulerAdapter = Report.ActualSchedulerAdapter;
		}
		protected virtual void UpdateVisibleIntervals(ReportViewBase view) {
			view.CreateVisibleIntervals();
		}
		protected virtual void ClearViewAdapter(ReportViewBase view) {
			view.SchedulerAdapter = null;
		}
		internal void UpdateSchedulerAdapterForAll() {
			Views.ForEach(UpdateViewAdapter);
		}
		internal void UpdateAllowFakeDataForAll() {
			Views.ForEach(UpdateAllowFakeData);
		}
		internal void UpdateVisibleIntervalsForAll() {
			Views.ForEach(UpdateVisibleIntervals);
		}
		protected virtual void UpdateAllowFakeData(ReportViewBase view) {
			view.AllowFakeData = Report.IsCloned;
		}
	}
	#endregion
	#region ReportViewBase    
	[Designer("DevExpress.XtraScheduler.Reporting.Design.ReportViewBaseDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions),
	DXToolboxItem(false),
	ToolboxTabName(AssemblyInfo.DXTabSchedulerReporting),
	ToolboxItemFilter(DevExpress.XtraReports.Design.AttributeSR.SchedulerToolboxItem, ToolboxItemFilterType.Require),
	]
	public abstract class ReportViewBase : Component, ISupportInitialize, ISchedulerAppointmentProvider, ISchedulerResourceProvider, ISchedulerTimeIntervalProvider, IServiceProvider, IReportControlChangeTarget {
		#region Fields
		internal const ColumnArrangementMode DefaultColumnArrangement = ColumnArrangementMode.Ascending;
		internal const int DefaultVisibleIntervalColumnCount = 1;
		internal const int DefaultVisibleIntervalCount = 1;
		internal const int DefaultVisibleResourceCount = 1;
		internal const int AllResourcesVisibleConst = 0;
		internal const SchedulerGroupType DefaultGroupType = SchedulerGroupType.Resource;
		internal const int DefaultFirstVisibleResourceIndex = 0; 
		internal static DateTime DefaultStart = DateTime.Today;
		SchedulerPrintAdapter schedulerAdapter;
		BaseViewAppearance appearance;
		BaseViewAppearance printAppearance;
		ViewPainterBase painter;
		TimeIntervalCollection visibleIntervals;
		SchedulerGroupType groupType;
		int visibleResourceCount;
		int visibleIntervalCount;
		int visibleIntervalColumnCount;
		ColumnArrangementMode columnArrangement;
		bool allowFakeData = false;
		bool isDisposed;
		#endregion
		protected ReportViewBase()
			: base() {
			Initialize();
		}
		#region Properties
		internal SchedulerPrintAdapter SchedulerAdapter {
			get {
				return schedulerAdapter;
			}
			set {
				if (this.schedulerAdapter == value)
					return;
				UnsubscribeSchedulerAdapterEvents();
				this.schedulerAdapter = value;
				SubscribeSchedulerAdapterEvents();
				OnSchedulerAdapterChanged();
			}
		}
		protected internal bool AllowFakeData {
			get { return allowFakeData; }
			set {
				if (allowFakeData == value)
					return;
				allowFakeData = value;
				RaiseChanged(ReportControlChangeType.AllowFakeDataChanged);
			}
		}
		protected internal virtual bool CanCreateFakeData() {
			return AllowFakeData || DesignMode;
		}
		protected virtual void OnSchedulerAdapterChanged() {
			RaiseChanged(ReportControlChangeType.SchedulerAdapterChanged);
		}
		protected internal ViewPainterBase Painter { get { return painter; } }
		internal bool IsDisposed { get { return isDisposed; } }
		protected internal TimeIntervalCollection VisibleIntervals {
			get {
				return visibleIntervals;
			}
		}
		protected internal int ActualVisibleResourceCount {
			get { return VisibleResourceCount == AllResourcesVisibleConst ? ((ISchedulerResourceProvider)this).ResourceCount : VisibleResourceCount; }
		}
#if !SL
	[DevExpressXtraSchedulerReportingLocalizedDescription("ReportViewBaseAppearance")]
#endif
		public BaseViewAppearance Appearance {
			get { return appearance; }
			set {
				if (appearance == value)
					return;
				UnsubscribeAppearanceEvents();
				appearance = value;
				SubscribeAppearanceEvents();
			}
		}
		protected internal BaseViewAppearance PrintAppearance { get { return printAppearance; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportViewBaseGroupType"),
#endif
DefaultValue(DefaultGroupType), Category(SRCategoryNames.Layout)]
		public SchedulerGroupType GroupType {
			get { return groupType; }
			set {
				if (groupType == value)
					return;
				groupType = value;
				RaiseChanged(ReportControlChangeType.VisibleResourceCountChanged); 
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportViewBaseVisibleResourceCount"),
#endif
DefaultValue(DefaultVisibleResourceCount), Category(SRCategoryNames.Layout)]
		public int VisibleResourceCount {
			get { return visibleResourceCount; }
			set {
				int newVal = Math.Max(0, value);
				if (visibleResourceCount == newVal)
					return;
				visibleResourceCount = newVal;
				RaiseChanged(ReportControlChangeType.VisibleResourceCountChanged);
			}
		}
		protected internal int VisibleIntervalCount {
			get { return visibleIntervalCount; }
			set {
				int newVal = Math.Max(1, value);
				if (visibleIntervalCount == newVal)
					return;
				visibleIntervalCount = newVal;
				RaiseChanged(ReportControlChangeType.VisibleIntervalCountChanged);
			}
		}
		protected internal int VisibleIntervalColumnCount {
			get { return visibleIntervalColumnCount; }
			set {
				int newVal = Math.Max(1, value);
				if (visibleIntervalColumnCount == newVal)
					return;
				visibleIntervalColumnCount = newVal;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("ReportViewBaseColumnArrangement"),
#endif
DefaultValue(DefaultColumnArrangement), Category(SRCategoryNames.Layout)]
		public ColumnArrangementMode ColumnArrangement {
			get { return columnArrangement; }
			set {
				if (columnArrangement == value)
					return;
				columnArrangement = value;
			}
		}
		#endregion
		#region Events
		#region Changed
		ReportControlStateChangedEventHandler onChanged;
		internal event ReportControlStateChangedEventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged(ReportControlChangeType changeType) {
			if (onChanged != null) {
				ReportControlStateChangedEventArgs args = new ReportControlStateChangedEventArgs(changeType);
				onChanged(this, args);
			}
		}
		#endregion
		#region SubscribeAppearanceEvents
		protected internal virtual void SubscribeAppearanceEvents() {
			if (appearance == null)
				return;
			appearance.Changed += new EventHandler(OnAppearanceChanged);
		}
		#endregion
		#region UnsubscribeAppearanceEvents
		protected internal virtual void UnsubscribeAppearanceEvents() {
			if (appearance != null)
				appearance.Changed -= new EventHandler(OnAppearanceChanged);
		}
		#endregion
		protected internal virtual void OnAppearanceChanged(object sender, EventArgs e) {
			CalculatePrintAppearance(Painter);
			RaiseChanged(ReportControlChangeType.AppearanceChanged);
		}
		#endregion
		protected internal virtual void Initialize() {
			this.visibleIntervalCount = CalculateDefaultVisibleIntervalCount();
			this.visibleResourceCount = DefaultVisibleResourceCount;
			this.visibleIntervalColumnCount = DefaultVisibleIntervalColumnCount;
			this.groupType = DefaultGroupType;
			this.appearance = CreateAppearance();
			this.printAppearance = CreateAppearance();
			this.painter = CreateViewPainter();
			CalculatePrintAppearance(painter);
			visibleIntervals = new TimeIntervalCollection();
			CreateVisibleIntervals();
			SubscribeEvents();
		}
		protected virtual int CalculateDefaultVisibleIntervalCount() {
			return DefaultVisibleIntervalCount;
		}
		#region AfterApplyChanges
		AfterApplyReportControlChangesEventHandler onAfterApplyChanges;
		internal event AfterApplyReportControlChangesEventHandler AfterApplyChanges { add { onAfterApplyChanges += value; } remove { onAfterApplyChanges -= value; } }
		protected internal virtual void RaiseAfterApplyChanges(ReportControlChangeActions actions) {
			if (onAfterApplyChanges != null) {
				AfterApplyReportControlChangesEventArgs args = new AfterApplyReportControlChangesEventArgs(actions);
				onAfterApplyChanges(this, args);
			}
		}
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					UnsubscribeEvents();
					if (painter != null) {
					}
					if (appearance != null) {
						appearance.Dispose();
						appearance = null;
					}
					if (printAppearance != null) {
						printAppearance.Dispose();
						printAppearance = null;
					}
					isDisposed = true;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal virtual void SubscribeEvents() {
			SubscribeAppearanceEvents();
			SubscribeSchedulerAdapterEvents();
		}
		protected internal virtual void UnsubscribeEvents() {
			UnsubscribeAppearanceEvents();
			UnsubscribeSchedulerAdapterEvents();
		}
		#region IReportControlChangeTarget members
		ReportControlChangeActions IReportControlChangeTarget.UpdateVisibleIntervals() {
			return CreateVisibleIntervals();
		}
		void IReportControlChangeTarget.NotifyDependents(ReportControlChangeActions actions) {
			RaiseAfterApplyChanges(actions);
		}
		#endregion
		protected internal virtual void CalculatePrintAppearance(ViewPainterBase viewPainter) {
			AppearanceDefaultInfo[] defaultAppearances = viewPainter.GetDefaultAppearances();
			BaseViewAppearance viewAppearanceCollection = Appearance;
			int count = defaultAppearances.Length;
			for (int i = 0; i < count; i++) {
				AppearanceDefaultInfo defaultAppearance = defaultAppearances[i];
				string appearanceName = defaultAppearance.Name;
				AppearanceObject printAppearance = PrintAppearance.GetAppearance(appearanceName);
				AppearanceObject viewAppearance = viewAppearanceCollection.GetAppearance(appearanceName);
				AppearanceHelper.Combine(printAppearance, new AppearanceObject[] { viewAppearance }, defaultAppearance.DefaultAppearance);
			}
		}
		#region ISchedulerAppointmentProvider Members
		public AppointmentBaseCollection GetAppointments(TimeInterval timeInterval, ResourceBaseCollection resources) {
			return SchedulerAdapter != null ? SchedulerAdapter.GetAppointments(timeInterval, resources) : new AppointmentBaseCollection();
		}
		public IAppointmentStatus GetStatus(object statusId) {
			return SchedulerAdapter != null ? SchedulerAdapter.GetStatus(statusId) : AppointmentStatus.Empty;
		}
		public Color GetLabelColor(object labelId) {
			return SchedulerAdapter != null ? SchedulerAdapter.GetLabelColor(labelId) : Color.White;
		}
		#endregion
		#region ISchedulerResourceProvider Members
		SchedulerGroupType ISchedulerResourceProvider.GetGroupType() {
			if (SchedulerAdapter != null && SchedulerAdapter.EnableSmartSync)
				return ((ISchedulerResourceProvider)SchedulerAdapter).GetGroupType();
			return GroupType;
		}
		ResourceBaseCollection ISchedulerResourceProvider.GetResources() {
			ResourceBaseCollection resources = CalculateActualResources();
			return resources;
		}
		internal static ResourceBaseCollection CreateFakeResources() {
			ResourceBaseCollection stubResources = new ResourceBaseCollection();
			int resourceCount = 5;
			for (int i = 1; i <= resourceCount; i++)
				stubResources.Add(new ResourceBase(i, "Res " + i.ToString()));
			return stubResources;
		}
		protected internal virtual TimeIntervalCollection CreateFakeTimeIntervals() {
			DateTime date = GetFakeIntervalsStart();
			return CreateFakeTimeIntervalsCore(date);
		}
		protected internal virtual DateTime GetFakeIntervalsStart() {
			if (SchedulerAdapter == null)
				return DateTime.Now;
			DateTime start = SchedulerAdapter.GetTimeIntervals().Interval.Start;
			return start == DateTime.MinValue ? DateTime.Now : start;
		}
		protected internal abstract TimeIntervalCollection CreateFakeTimeIntervalsCore(DateTime date);
		ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> ISchedulerResourceProvider.GetResourceColorSchemas() {
			if (SchedulerAdapter != null)
				return SchedulerAdapter.GetResourceColorSchemas();
			else {
				SchedulerColorSchemaCollection schemas = new SchedulerColorSchemaCollection();
				return schemas;
			}
		}
		int ISchedulerResourceProvider.ResourceCount {
			get { return CalculateResourceCount(); }
		}
		protected internal virtual ResourceBaseCollection CalculateActualResources() {
			SchedulerGroupType actualGroupType = ((ISchedulerResourceProvider)this).GetGroupType();
			if (SchedulerAdapter != null) {
				if (actualGroupType == SchedulerGroupType.None) {
					return CreateEmptyResourceCollection();
				}
				ResourceBaseCollection resources = SchedulerAdapter.GetResources();
				if (resources.Count == 0) {
					if (CanCreateFakeData())
						return CreateFakeResources();
					else 
						return CreateEmptyResourceCollection();
				}
				return resources;
			}
			return new ResourceBaseCollection();
		}
		protected ResourceBaseCollection CreateEmptyResourceCollection() {
			ResourceBaseCollection result = new ResourceBaseCollection();
			result.Add(ResourceBase.Empty);
			return result;
		}
		protected internal virtual int CalculateResourceCount() {
			if (GroupType == SchedulerGroupType.None)
				return 0;
			return SchedulerAdapter != null ? SchedulerAdapter.ResourceCount : 0;
		}
		#endregion
		#region ISchedulerTimeIntervalProvider Members
		TimeIntervalCollection ISchedulerTimeIntervalProvider.GetTimeIntervals() {
			return GetTimeIntervals();
		}
		TimeOfDayIntervalCollection ISchedulerTimeIntervalProvider.GetWorkTime(TimeInterval interval, Resource resource) {
			return GetWorkTime(interval, resource);
		}
		WorkDaysCollection ISchedulerTimeIntervalProvider.GetWorkDays() {
			return GetWorkDays();
		}
		string ISchedulerTimeIntervalProvider.GetClientTimeZoneId() {
			return SchedulerAdapter != null ? SchedulerAdapter.GetClientTimeZoneId() : SchedulerPrintAdapter.GetDefaultTimeZoneId();
		}
		TimeInterval ISchedulerTimeIntervalProvider.GetAdapterInterval() {
			TimeIntervalCollection intervals = GetAdapterTimeIntervals();
			if ((intervals.Count == 0) && CanCreateFakeData())
				intervals = CreateFakeTimeIntervals();
			return intervals.Interval;
		}
		DayOfWeek ISchedulerTimeIntervalProvider.GetFirstDayOfWeek() {
			return GetFirstDayOfWeek();
		}
		#endregion
		protected internal WorkDaysCollection GetWorkDays() {
			return SchedulerAdapter != null ? SchedulerAdapter.GetWorkDays() : SchedulerPrintAdapter.GetDefaultWorkDays();
		}
		protected internal virtual DayOfWeek GetFirstDayOfWeek() {
			return SchedulerAdapter != null ? SchedulerAdapter.GetFirstDayOfWeek() : SchedulerPrintAdapter.GetDefaultFirstDayOfWeek();
		}
		protected internal TimeOfDayIntervalCollection GetWorkTime(TimeInterval interval, Resource resource) {
			if (SchedulerAdapter != null)
				return SchedulerAdapter.GetWorkTime(interval, resource);
			else {
				TimeOfDayIntervalCollection result = new TimeOfDayIntervalCollection();
				result.Add(SchedulerPrintAdapter.DefaultWorkTime.Clone());
				return result;
			}
		}
		protected internal virtual TimeIntervalCollection GetTimeIntervals() {
			return VisibleIntervals;
		}
		#region IServiceProvider Members
		public virtual new object GetService(Type serviceType) {
			return SchedulerAdapter != null ? SchedulerAdapter.GetService(serviceType) : null;
		}
		protected internal virtual HeaderCaptionFormatProviderBase GetHeaderCaptionFormatProvider() {
			return SchedulerAdapter != null ? SchedulerAdapter.GetHeaderCaptionFormatProvider() : null;			
		}		
		#endregion
		protected virtual void SubscribeSchedulerAdapterEvents() {
			if (SchedulerAdapter == null)
				return;
			SchedulerAdapter.Disposed += new EventHandler(OnSchedulerAdapterDisposed);
		}
		protected virtual void OnSchedulerAdapterDisposed(object sender, EventArgs e) {
			this.schedulerAdapter = null;
		}
		protected virtual void UnsubscribeSchedulerAdapterEvents() {
			if (SchedulerAdapter == null)
				return;
			SchedulerAdapter.Disposed -= new EventHandler(OnSchedulerAdapterDisposed);
		}
		protected abstract BaseViewAppearance CreateAppearance();
		protected internal virtual ReportControlChangeActions CreateVisibleIntervals() {
			VisibleIntervals.BeginUpdate();
			try {
				VisibleIntervals.Clear();
				CreateVisibleIntervalsCore();
				return ReportControlChangeActions.RaiseVisibleIntervalChanged | ReportControlChangeActions.NotifyDependents;
			}
			finally {
				VisibleIntervals.EndUpdate();
			}
		}
		protected internal void CreateVisibleIntervalsCore() {
			TimeIntervalCollection adapterIntervals = GetAdapterTimeIntervals();
			TimeIntervalCollection intervals = CalculateVisibleIntervals(adapterIntervals);
			if ((intervals.Count == 0) && CanCreateFakeData())
				intervals = CreateFakeTimeIntervals();
			VisibleIntervals.AddRange(intervals);
		}
		protected internal virtual TimeIntervalCollection CalculateVisibleIntervals(TimeIntervalCollection adapterIntervals) {
			TimeIntervalCollection intervals = CreateTimeIntervalCollection();			
			intervals.AddRange(adapterIntervals);
			return intervals;
		}
		protected internal virtual TimeIntervalCollection GetAdapterTimeIntervals() {
			return SchedulerAdapter != null ? SchedulerAdapter.GetTimeIntervals() : new TimeIntervalCollection();
		}
		protected internal abstract TimeIntervalCollection CreateTimeIntervalCollection();
		protected internal abstract ViewPainterBase CreateViewPainter();
		protected internal abstract TimeIntervalFormatType GetDefaultTimeIntervalFormatType();
		#region ISupportInitialize Members
		public virtual void BeginInit() {
		}
		public virtual void EndInit() {
		}
		#endregion
	}
	#endregion
}

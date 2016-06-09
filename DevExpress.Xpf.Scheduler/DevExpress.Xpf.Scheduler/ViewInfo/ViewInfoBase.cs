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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public abstract class ViewInfoBase<T> : ISchedulerViewInfoBase, ISupportAppointmentsBase
	where T : ICellContainer {
		internal static readonly ResourceBaseCollection EmptyResourceCollection = CreateResourceCollection(ResourceBase.Empty);
		static ResourceBaseCollection CreateResourceCollection(Resource resource) {
			ResourceBaseCollection result = new ResourceBaseCollection();
			result.Add(resource);
			return result;
		}
		#region Fields
		SchedulerViewBase view;
		AssignableCollection<AssignableCollection<T>> resourcesCellContainers;
		AssignableCollection<SingleResourceViewInfo> resourceContainers;
		AppointmentControlCollection appointmentControls = new AppointmentControlCollection();
		AppointmentControlCollection draggedAppointmentControls = new AppointmentControlCollection();
		AssignableCollection<SingleDayViewInfo> daysContainers;
		IAppointmentComparerProvider appointmentComparerProvider;
		SchedulerCancellationTokenSource tokenSource;
		#endregion
		protected ViewInfoBase(SchedulerViewBase view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.tokenSource = new SchedulerCancellationTokenSource();
			Initialize();
		}
		#region Properties
		public AssignableCollection<SingleDayViewInfo> DaysContainers { get { return daysContainers; } set { daysContainers = value; } }
		public SingleDayViewInfo LastSingleDayViewInfo {
			get {
				if (daysContainers.Count > 0)
					return daysContainers[daysContainers.Count - 1];
				return null;
			}
		}
		public SchedulerViewBase View { get { return view; } }
		public SchedulerControl Control { get { return View.Control; } }
		public TimeIntervalCollection VisibleIntervals { get { return View.InnerView.InnerVisibleIntervals; } }
		public AppointmentDisplayOptions AppointmentDisplayOptions { get { return View.InnerView.AppointmentDisplayOptions; } }
		public virtual ResourceBaseCollection Resources { get { return View.InnerView.VisibleResources; } }
		public AssignableCollection<AssignableCollection<T>> ResourcesCellContainers { get { return resourcesCellContainers; } }
		public AssignableCollection<SingleResourceViewInfo> ResourcesContainers { get { return resourceContainers; } }
		public SingleResourceViewInfo LastResourcesViewInfo {
			get {
				if (resourceContainers.Count > 0)
					return resourceContainers[resourceContainers.Count - 1];
				return null;
			}
		}
		public SingleResourceViewInfo FirstResourcesViewInfo {
			get {
				if (resourceContainers.Count > 0)
					return resourceContainers[0];
				return null;
			}
		}
		public AppointmentControlCollection AppointmentControls { get { return appointmentControls; } }
		public AppointmentControlCollection DraggedAppointmentControls { get { return draggedAppointmentControls; } }
		IAppointmentComparerProvider ISupportAppointmentsBase.AppointmentComparerProvider {
			get {
				if (appointmentComparerProvider == null) {
					appointmentComparerProvider = View.Control.GetService<IAppointmentComparerProvider>();
				}
				if (appointmentComparerProvider == null)
					appointmentComparerProvider = new AppointmentComparerProvider(View.Control);
				return appointmentComparerProvider;
			}
		}
		bool ISupportAppointmentsBase.UseAsyncMode {
			get { throw new NotImplementedException(); }
		}
		SchedulerCancellationTokenSource ISupportAppointmentsBase.CancellationToken {
			get { return this.tokenSource; }
		}
		#endregion
		#region Events
		#region SelectionChaged
		EventHandler onSelectionChanged;
		public event EventHandler SelectionChanged {
			add { onSelectionChanged += value; }
			remove { onSelectionChanged -= value; }
		}
		protected virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null)
				onSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region AppointmentsSelectionChanged
		EventHandler onAppointmentsSelectionChanged;
		public event EventHandler AppointmentsSelectionChanged {
			add { onAppointmentsSelectionChanged += value; }
			remove { onAppointmentsSelectionChanged -= value; }
		}
		protected virtual void RaiseAppointmentsSelectionChanged() {
			if (onAppointmentsSelectionChanged != null)
				onAppointmentsSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region Initialized
		EventHandler onInitialized;
		public event EventHandler Initialized {
			add { onInitialized += value; }
			remove { onInitialized -= value; }
		}
		protected virtual void RaiseInitialized() {
			if (onInitialized != null)
				onInitialized(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		public virtual void EndCreate() {
			RaiseInitialized();
		}
		public virtual void UpdateSelection(SchedulerViewSelection newSelection) {
			CellContainerCollection containers = GetContainersForUpdateSelection();
			foreach (ICellContainer container in containers) {
				container.CalculateSelectionLayout(newSelection);
			}
			RaiseSelectionChanged();
		}
		public virtual void UpdateAppointmentsSelection() {
			int count = AppointmentControls.Count;
			AppointmentSelectionController selectionController = View.Control.AppointmentSelectionController;
			for (int i = 0; i < count; i++) {
				AppointmentControl appointmentControl = AppointmentControls[i];
				Appointment apt = appointmentControl.Appointment; 
				appointmentControl.ViewInfo.Selected = selectionController.IsAppointmentSelected(apt);
			}
			RaiseAppointmentsSelectionChanged();
		}
		protected internal virtual void Initialize() {
			this.resourcesCellContainers = new AssignableCollection<AssignableCollection<T>>();
			this.resourceContainers = new AssignableCollection<SingleResourceViewInfo>();
			this.daysContainers = new AssignableCollection<SingleDayViewInfo>();
		}
		public virtual void Create() {
			ResourcesContainers.Clear();
			CreateResourcesView(VisibleIntervals);
			CreateAdditionalViewElements();
			CreateNavigationButtons();
		}
		protected internal virtual void CreateNavigationButtons() {
			SchedulerControl control = View.Control;
			SchedulerNavigationButtonCalculator calc = new SchedulerNavigationButtonCalculator(control, View);
			PrevNextAppointmentIntervalPairCollection data = calc.Calculate();
			if (data == null)
				return;
			int count = data.Count;
			for (int i = 0; i < count; i++) {
				PrevNextAppointmentIntervalPair item = data[i];
				Resource resource = item.Resource;
				List<SingleResourceViewInfo> resourceViewInfos = GetResourceViewInfos(resource);
				int resourceCount = resourceViewInfos.Count;
				if (resourceCount <= 0)
					continue;
				SingleResourceViewInfo firstResource = resourceViewInfos[0];
				firstResource.PrevNavButtonInfo = CreateNavigationButtonInfo(control, item.PrevAppointmentInterval, resource);
				firstResource.PrevNavButtonInfo.Visibility = Visibility.Visible;
				firstResource.PrevNavButtonInfo.IsEnabled = CalculateNavigationButtonEnabled(item.PrevAppointmentInterval);
				SingleResourceViewInfo lastResource = resourceViewInfos[resourceCount - 1];
				lastResource.NextNavButtonInfo = CreateNavigationButtonInfo(control, item.NextAppointmentInterval, resource);
				lastResource.NextNavButtonInfo.Visibility = Visibility.Visible;
				lastResource.NextNavButtonInfo.IsEnabled = CalculateNavigationButtonEnabled(item.NextAppointmentInterval);
			}
		}
		protected virtual bool CalculateNavigationButtonEnabled(TimeInterval interval) {
			return interval != null && !interval.Equals(TimeInterval.Empty);
		}
		protected virtual NavigationButtonInfo CreateNavigationButtonInfo(SchedulerControl control, TimeInterval interval, Resource resource) {
			return new NavigationButtonInfo(control, interval, resource);
		}
		private List<SingleResourceViewInfo> GetResourceViewInfos(Resource resource) {
			List<SingleResourceViewInfo> result = new List<SingleResourceViewInfo>();
			int count = ResourcesContainers.Count;
			for (int i = 0; i < count; i++) {
				SingleResourceViewInfo resourceViewInfo = ResourcesContainers[i];
				ResourceHeaderBase resourceHeader = resourceViewInfo.ResourceHeader;
				if ((resourceHeader == null && resource == ResourceBase.Empty) || resourceHeader.Resource == resource)
					result.Add(ResourcesContainers[i]);
			}
			return result;
		}
		protected internal virtual AssignableCollection<SingleResourceViewInfo> CreateResourcesView(TimeIntervalCollection visibleIntervals) {
			AssignableCollection<SingleResourceViewInfo> result = new AssignableCollection<SingleResourceViewInfo>();
			int count = Resources.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = this.Resources[i];
				int resourceIndex = GetResourceColorIndex(resource);
				SchedulerColorSchema schema = GetResourceColorSchema(resource, resourceIndex);
				ResourceBrushes brushes = new ResourceBrushes(schema);
				SingleResourceViewInfo resourceView = CreateSingleResourceView(Resources[i], visibleIntervals, brushes);
				result.Add(resourceView);
			}
			return result;
		}
		protected virtual SchedulerColorSchema GetResourceColorSchema(Resource resource, int resourceIndex) {
			SchedulerControl control = View.Control;
			QueryResourceColorSchemaEventArgs args = new QueryResourceColorSchemaEventArgs(resource, resourceIndex);
			control.RaiseQueryResourceColorSchema(args);
			if (args.ResourceColorSchema != null) {
				return (SchedulerColorSchema)args.ResourceColorSchema;
			}
			SchedulerColorSchema schema = control.ResourceColorSchemas.GetSchema(resource.GetColor(), resourceIndex);
			return schema;
		}
		protected internal virtual SingleResourceViewInfo CreateSingleResourceView(Resource resource, TimeIntervalCollection visibleIntervals, ResourceBrushes brushes) {
			AssignableCollection<T> cellContainers = CreateResourceCellContainers(resource, visibleIntervals, brushes);
			ResourcesCellContainers.Add(cellContainers);
			SingleResourceViewInfo viewInfo = CreateSingleResourceViewInfo();
			viewInfo.ResourceHeader = CreateResourceHeader(resource, brushes);
			viewInfo.CellContainers = GetCellContainers(cellContainers);
			ResourcesContainers.Add(viewInfo);
			return viewInfo;
		}
		protected internal virtual ResourceHeaderBase CreateResourceHeader(Resource resource, ResourceBrushes brushes) {
			return new HorizontalResourceHeader(VisibleIntervals, resource, brushes);
		}
		protected internal virtual AssignableCollection<T> CreateResourceCellContainers(Resource resource, TimeIntervalCollection visibleIntervals, ResourceBrushes brushes) {
			AssignableCollection<T> result = new AssignableCollection<T>();
			int count = visibleIntervals.Count;
			for (int i = 0; i < count; i++) {
				T container = CreateCellContainer(visibleIntervals[i], resource, brushes);
				result.Add(container);
			}
			return result;
		}
		protected internal abstract IViewInfo CreateAdditionalViewElements();
		protected internal abstract SingleResourceViewInfo CreateSingleResourceViewInfo();
		protected internal abstract T CreateCellContainer(TimeInterval interval, Resource resource, ResourceBrushes brushes);
		#region ISchedulerViewInfoBase Members
		public virtual CellContainerCollection GetContainers() {
			CellContainerCollection result = new CellContainerCollection();
			int resourceContainersCount = resourcesCellContainers.Count;
			for (int i = 0; i < resourceContainersCount; i++) {
				AssignableCollection<T> resourceContainer = resourcesCellContainers[i];
				result.AddRange(GetCellContainers(resourceContainer));
			}
			return result;
		}
		protected internal virtual CellContainerCollection GetContainersForUpdateSelection() {
			return GetContainers();
		}
		protected internal virtual CellContainerCollection GetCellContainers(AssignableCollection<T> resourceCellContainers) {
			CellContainerCollection result = new CellContainerCollection();
			int count = resourceCellContainers.Count;
			for (int j = 0; j < count; j++)
				result.Add(resourceCellContainers[j]);
			return result;
		}
		IAppointmentFormatStringService ISchedulerViewInfoBase.GetFormatStringProvider() {
			return (IAppointmentFormatStringService)View.Control.GetService(typeof(IAppointmentFormatStringService));
		}
		#endregion
		public virtual int GetResourceColorIndex(Resource resource) {
			int index = CalculateVisibleResourceIndex(resource);
			return CalculateResourceColorIndex(index);
		}
		protected internal virtual int CalculateVisibleResourceIndex(Resource resource) {
			int count = Resources.Count;
			for (int i = 0; i < count; i++) {
				if (ResourceBase.MatchIds(resource, Resources[i]))
					return i;
			}
			return 0;
		}
		protected internal virtual int CalculateResourceColorIndex(int visibleIndex) {
			return View.InnerView.ActualFirstVisibleResourceIndex + visibleIndex;
		}
		#region ISupportAppointmentsBase Members
		TimeInterval ISupportAppointmentsBase.GetVisibleInterval() {
			return VisibleIntervals.Interval;
		}
		#endregion
	}
	public class AssignableCollection<T> : DXCollectionWithSetItem<T> {
		public AssignableCollection() {
			UniquenessProviderType = DXCollectionUniquenessProviderType.None;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Native {
	#region ViewGroupTypeStrategy
	public abstract class ViewGroupTypeStrategy {
		public abstract IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate);
		public abstract ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view);
	}
	#endregion
	#region ViewFactoryHelper
	public abstract class ViewFactoryHelper {
		#region Fields
		ViewGroupTypeStrategy groupByNoneStrategy;
		ViewGroupTypeStrategy groupByDateStrategy;
		ViewGroupTypeStrategy groupByResourceStrategy;
		#endregion
		protected ViewFactoryHelper() {
			this.groupByNoneStrategy = CreateGroupByNoneStrategy();
			this.groupByDateStrategy = CreateGroupByDateStrategy();
			this.groupByResourceStrategy = CreateGroupByResourceStrategy();
		}
		#region Properties
		protected internal ViewGroupTypeStrategy GroupByNoneStrategy { get { return groupByNoneStrategy; } }
		protected internal ViewGroupTypeStrategy GroupByDateStrategy { get { return groupByDateStrategy; } }
		protected internal ViewGroupTypeStrategy GroupByResourceStrategy { get { return groupByResourceStrategy; } }
		protected internal virtual bool CanShowResources(InnerSchedulerViewBase view) {
			return view.CanShowResources();
		}
		#endregion
		public virtual IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(InnerSchedulerViewBase view, bool alternate) {
			ViewGroupTypeStrategy strategy = SelectStrategy(view);
			return strategy.CreateVisuallyContinuousCellsInfosCalculator(alternate);
		}
		protected internal virtual ViewGroupTypeStrategy SelectStrategy(InnerSchedulerViewBase view) {
			SchedulerGroupType groupType = CalcActualGroupType(view);
			switch (groupType) {
				case SchedulerGroupType.None:
					return groupByNoneStrategy;
				case SchedulerGroupType.Date:
					return groupByDateStrategy;
				case SchedulerGroupType.Resource:
					return groupByResourceStrategy;
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
		protected internal virtual SchedulerGroupType CalcActualGroupType(InnerSchedulerViewBase view) {
			return CanShowResources(view) ? view.GroupType : SchedulerGroupType.None;
		}
		public abstract ViewGroupTypeStrategy CreateGroupByNoneStrategy();
		public abstract ViewGroupTypeStrategy CreateGroupByDateStrategy();
		public abstract ViewGroupTypeStrategy CreateGroupByResourceStrategy();
		public abstract AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo, bool alternate);
	}
	#endregion
}

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
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentPredicate (abstract class)
	public abstract class AppointmentPredicate : SchedulerPredicate<Appointment> {
	}
	#endregion
	#region EmptyAppointmentPredicate
	public class EmptyAppointmentPredicate : SchedulerEmptyPredicate<Appointment> {
	}
	#endregion
	#region CompositeAppointmentPredicate
	public class CompositeAppointmentPredicate : SchedulerCompositePredicate<Appointment> {
	}
	#endregion
	#region AppointmentIntersectsIntervalPredicate
	public class AppointmentIntersectsIntervalPredicate : AppointmentPredicate {
		readonly TimeInterval interval;
		public AppointmentIntersectsIntervalPredicate(TimeInterval interval) {
			this.interval = interval;
		}
		public override bool Calculate(Appointment obj) {
			return interval.IntersectsWithExcludingBounds(((IInternalAppointment)obj).GetInterval());
		}
	}
	#endregion
	#region AppointmentsProcessorBase
	public abstract class AppointmentsProcessorBase : ProcessorBase<Appointment> {
		protected internal override NotificationCollection<Appointment> CreateDestinationCollection() {
			return new AppointmentBaseCollection();
		}
	}
	#endregion
	#region AppointmentsSimpleProcessorBase (abstract class)
	public abstract class AppointmentsSimpleProcessorBase : SimpleProcessorBase<Appointment> {
		protected AppointmentsSimpleProcessorBase(IPredicate<Appointment> predicate)
			: base(predicate) {
		}
		protected internal override NotificationCollection<Appointment> CreateDestinationCollection() {
			return new AppointmentBaseCollection();
		}
	}
	#endregion
	#region ResourcesAppointmentPredicate
	public class ResourcesAppointmentPredicate : AppointmentPredicate {
		ResourceBaseCollection resources;
		public ResourcesAppointmentPredicate(ResourceBaseCollection resources) {
			if (resources == null)
				Exceptions.ThrowArgumentNullException("resources");
			this.resources = resources;
		}
		public ResourcesAppointmentPredicate(Resource resource) {
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.resources = new ResourceBaseCollection();
			this.resources.Add(resource);
		}
		protected internal ResourceBaseCollection Resources { get { return resources; } }
		public override bool Calculate(Appointment apt) {
			return ResourceBase.IsEmptyResourceId(apt.ResourceId) || ResourceBase.InternalAreResourceIdsCollectionsIntersect(apt.ResourceIds, resources);
		}
	}
	#endregion
	#region ResourcesAppointmentsFilter
	public class ResourcesAppointmentsFilter : AppointmentsSimpleProcessorBase {
		public ResourcesAppointmentsFilter(ResourceBaseCollection resources)
			: base(new ResourcesAppointmentPredicate(resources)) {
		}
		public ResourcesAppointmentsFilter(Resource resource)
			: base(new ResourcesAppointmentPredicate(resource)) {
		}
	}
	#endregion
	#region NonEmptyResourceAppointmentPredicate
	public class NonEmptyResourceAppointmentPredicate : AppointmentPredicate {
		public override bool Calculate(Appointment apt) {
			return !ResourceBase.IsEmptyResourceId(apt.ResourceId);
		}
	}
	#endregion
	#region NonEmptyResourceAppointmentsFilter
	public class NonEmptyResourceAppointmentsFilter : AppointmentsSimpleProcessorBase {
		public NonEmptyResourceAppointmentsFilter()
			: base(new NonEmptyResourceAppointmentPredicate()) {
		}
	}
	#endregion
	#region DayViewTimeCellAppointmentPredicate
	public class DayViewTimeCellAppointmentPredicate : AppointmentPredicate {
		public override bool Calculate(Appointment apt) {
			return !apt.LongerThanADay;
		}
	}
	#endregion
	#region DayViewTimeCellAppointmentsFilter
	public class DayViewTimeCellAppointmentsFilter : AppointmentsSimpleProcessorBase {
		public DayViewTimeCellAppointmentsFilter()
			: base(new DayViewTimeCellAppointmentPredicate()) {
		}
	}
	#endregion
	#region LongTimeAppointmentPredicate
	public class DayViewAllDayAppointmentPredicate : AppointmentPredicate {
		public override bool Calculate(Appointment apt) {
			return apt.LongerThanADay;
		}
	}
	#endregion
	#region DayViewAllDayAppointmentsFilter
	public class DayViewAllDayAppointmentsFilter : AppointmentsSimpleProcessorBase {
		public DayViewAllDayAppointmentsFilter()
			: base(new DayViewAllDayAppointmentPredicate()) {
		}
	}
	#endregion
	#region RecurringAppointmentsSeparator
	public class RecurringAppointmentsSeparator : AppointmentsProcessorBase {
		AppointmentBaseCollection destinationPatterns;
		public RecurringAppointmentsSeparator() {
			this.destinationPatterns = new AppointmentBaseCollection();
			this.destinationPatterns.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
		}
		public AppointmentBaseCollection DestinationPatterns { get { return destinationPatterns; } }
		protected internal override void InitializeProcess() {
			base.InitializeProcess();
			DestinationPatterns.Clear();
		}
		protected internal override void ProcessCore(NotificationCollection<Appointment> sourceCollection) {
			SeparateAppointments((AppointmentBaseCollection)sourceCollection, (AppointmentBaseCollection)DestinationCollection, DestinationPatterns);
		}
		protected internal virtual void SeparateAppointments(AppointmentBaseCollection sourceAppointments, AppointmentBaseCollection appointments, AppointmentBaseCollection patterns) {
			int count = sourceAppointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = sourceAppointments[i];
				if (apt.Type == AppointmentType.Pattern) {
					patterns.Add(apt);
					if (apt.HasExceptions)
						appointments.AddRange(GetPatternChangedOccurrences(apt));
				} else
					appointments.Add(apt);
			}
		}
		protected internal virtual AppointmentBaseCollection GetPatternChangedOccurrences(Appointment pattern) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			if (!pattern.HasExceptions)
				return result;
			AppointmentBaseCollection exceptions = ((IInternalAppointment)pattern).PatternExceptions;
			int count = exceptions.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = exceptions[i];
				if (apt.Type == AppointmentType.ChangedOccurrence)
					result.Add(apt);
			}
			return result;
		}
	}
	#endregion
	#region CompositeAppointmentsProcessor
	public class CompositeAppointmentsProcessor : AppointmentsProcessorBase {
		AppointmentsProcessorCollection items;
		public CompositeAppointmentsProcessor() {
			this.items = new AppointmentsProcessorCollection();
		}
		public AppointmentsProcessorCollection Items { get { return items; } }
		protected internal override void ProcessCore(NotificationCollection<Appointment> sourceCollection) {
			DestinationCollection.AddRange(ProcessItems(sourceCollection));
		}
		protected internal virtual NotificationCollection<Appointment> ProcessItems(NotificationCollection<Appointment> sourceCollection) {
			NotificationCollection<Appointment> result = sourceCollection;
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				ProcessorBase<Appointment> item = Items[i];
				item.Process(result);
				result = item.DestinationCollection;
			}
			return result;
		}
	}
	#endregion
	#region FilterAppointmentPredicate
	public class FilterAppointmentPredicate : FilterObjectViaFilterCriteriaPredicate<Appointment> {
		public FilterAppointmentPredicate(IAppointmentStorageBase appointmentStorage)
			: base(appointmentStorage) {
		}
	}
	#endregion
	#region FilterAppointmentViaStorageEventPredicate
	public class FilterAppointmentViaStorageEventPredicate : FilterObjectViaStorageEventPredicate<Appointment> {
		public FilterAppointmentViaStorageEventPredicate(ISchedulerStorageBase storage)
			: base(storage) {
		}
		#region Calculate
		public override bool Calculate(Appointment apt) {
			return ((IInternalSchedulerStorageBase)Storage).RaiseFilterAppointment(apt);
		}
		#endregion
	}
	#endregion
	#region AppointmentFilter
	public class AppointmentsFilter : AppointmentsSimpleProcessorBase {
		public AppointmentsFilter(IPredicate<Appointment> appointmentPredicate)
			: base(appointmentPredicate) {
		}
	}
	#endregion
	#region AppointmentsProcessorCollection
	public class AppointmentsProcessorCollection : List<ProcessorBase<Appointment>> {
	}
	#endregion
	#region AppointmentPredicateCollection
	public class AppointmentPredicateCollection : List<AppointmentPredicate> {
	}
	#endregion
	#region DeleteAppointmentsCommandFilter
	public class DeleteAppointmentsCommandFilter : AppointmentsProcessorBase {
		AppointmentBaseCollection recurringAppointments;
		AppointmentBaseCollection nonRecurringAppointments;
		DeleteAppointmentsCommand command;
		public DeleteAppointmentsCommandFilter(DeleteAppointmentsCommand command) {
			Guard.ArgumentNotNull(command, "command");
			this.command = command;
			this.recurringAppointments = CreateCollection();
			this.nonRecurringAppointments = CreateCollection();
		}
		public AppointmentBaseCollection RecurringAppointments { get { return recurringAppointments; } }
		public AppointmentBaseCollection NonRecurringAppointments { get { return nonRecurringAppointments; } }
		internal DeleteAppointmentsCommand Command { get { return command; } }
		internal AppointmentBaseCollection CreateCollection() {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			result.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
			return result;
		}
		protected internal override void InitializeProcess() {
			base.InitializeProcess();
			RecurringAppointments.Clear();
			NonRecurringAppointments.Clear();
		}
		protected internal override void ProcessCore(NotificationCollection<Appointment> sourceCollection) {
			int count = sourceCollection.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = (Appointment)sourceCollection[i];
				if (!command.CanDeleteAppointment(apt))
					continue;
				if (apt.IsBase) {
					NonRecurringAppointments.Add(apt);
					continue;
				}
				Appointment pattern = apt.RecurrencePattern;
				if (pattern != null)
					RecurringAppointments.Add(apt);
			}
		}
	}
	#endregion
	#region VisibleIntervalAppointmentsSeparator
	public class VisibleIntervalAppointmentsSeparator : AppointmentsProcessorBase {
		#region Fields
		AppointmentBaseCollection invisibleAppointmentsBefore;
		AppointmentBaseCollection invisibleAppointmentsAfter;
		TimeInterval visibleInterval;
		TimeZoneHelper timeZoneHelper;
		#endregion
		public VisibleIntervalAppointmentsSeparator(TimeInterval visibleInterval, TimeZoneHelper timeZoneHelper) {
			Guard.ArgumentNotNull(visibleInterval, "visibleInterval");
			Guard.ArgumentNotNull(timeZoneHelper, "timeZoneEngine");
			this.visibleInterval = visibleInterval;
			this.timeZoneHelper = timeZoneHelper;
			this.invisibleAppointmentsBefore = new AppointmentBaseCollection();
			this.invisibleAppointmentsBefore.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
			this.invisibleAppointmentsAfter = new AppointmentBaseCollection();
			this.invisibleAppointmentsAfter.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
		}
		public AppointmentBaseCollection InvisibleAppointmentsBefore { get { return invisibleAppointmentsBefore; } }
		public AppointmentBaseCollection InvisibleAppointmentsAfter { get { return invisibleAppointmentsAfter; } }
		public new AppointmentBaseCollection DestinationCollection { get { return (AppointmentBaseCollection)base.DestinationCollection; } }
		protected internal TimeInterval VisibleInterval { get { return visibleInterval; } }
		internal TimeZoneHelper TimeZoneHelper { get { return timeZoneHelper; } }
		protected internal override void InitializeProcess() {
			base.InitializeProcess();
			InvisibleAppointmentsBefore.Clear();
			InvisibleAppointmentsAfter.Clear();
		}
		protected internal override void ProcessCore(NotificationCollection<Appointment> sourceCollection) {
			SeparateAppointments((AppointmentBaseCollection)sourceCollection, (AppointmentBaseCollection)DestinationCollection);
		}
		protected internal virtual void SeparateAppointments(AppointmentBaseCollection sourceAppointments, AppointmentBaseCollection visibleAppointments) {
			int count = sourceAppointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = sourceAppointments[i];
				TimeInterval aptInterval = TimeZoneHelper.ToClientTime(((IInternalAppointment)apt).GetInterval(), true);
				if (VisibleInterval.IntersectsWithExcludingBounds(aptInterval)) {
					visibleAppointments.Add(apt);
					continue;
				}
				if (aptInterval.End <= VisibleInterval.Start)
					InvisibleAppointmentsBefore.Add(apt);
				else
					invisibleAppointmentsAfter.Add(apt);
			}
		}
	}
	#endregion
	#region ResourceIdAppointmentPredicateDictionary
	public class ResourceIdAppointmentPredicateDictionary : Dictionary<object, SchedulerPredicate<Appointment>> {
	}
	#endregion
	#region AppointmentResourcesMatchFilter
	public class AppointmentResourcesMatchFilter {
		readonly static AppointmentResourcesMatchFilter empty = new EmptyAppointmentResourceFilterInfo();
		public static AppointmentResourcesMatchFilter Empty { get { return empty; } }
		public IPredicate<Appointment> AppointmentExternalFilter { get; set; }
		public ResourceBaseCollection Resources { get; set; }
		public bool ShowOnlyResourceAppointments { get; set; }
		public TimeZoneHelper TimeZoneHelper { get; set; }
		public ResourceIdAppointmentPredicateDictionary ResourceAppointmentPredicateDictionary { get; set; }
		public virtual bool MatchAppointmentResourceIds(Appointment apt) {
			if (AppointmentExternalFilter != null) {
				if (!AppointmentExternalFilter.Calculate(apt))
					return false;
			}
			bool isEmptyResource = ResourceBase.IsEmptyResourceId(apt.ResourceId);
			if (isEmptyResource && ShowOnlyResourceAppointments)
				return false;
			return isEmptyResource || ResourceBase.InternalAreResourceIdsCollectionsIntersect(apt.ResourceIds, Resources);
		}
	}
	#endregion
	public class EmptyAppointmentResourceFilterInfo : AppointmentResourcesMatchFilter {
		public override bool MatchAppointmentResourceIds(Appointment apt) {
			return true;
		}
	}
	#region AppointmenPerCellReductionPredicate
	public class AppointmenPerCellReductionPredicate {
		int appointmentCountPerCell;
		int[] days;
		IVisuallyContinuousCellsInfoCore cellsInfo;
		public AppointmenPerCellReductionPredicate(IVisuallyContinuousCellsInfoCore cellsInfo, int appointmentCountPerCell) {
			Guard.ArgumentNotNull(cellsInfo, "cellsInfo");
			this.cellsInfo = cellsInfo;
			this.appointmentCountPerCell = appointmentCountPerCell;
			this.days = new int[this.cellsInfo.Count];
			int count = this.days.Length;
			for (int i = 0; i < count; i++) {
				this.days[i] = 0;
			}
		}
		public bool Calculate(TimeInterval clientAppointmentInterval) {
			int start = GetIndexFromStartDate(clientAppointmentInterval.Start);
			int end = (clientAppointmentInterval.Start == clientAppointmentInterval.End) ? start : GetIndexFromEndDate(clientAppointmentInterval.End);
			if (!cellsInfo.Interval.IntersectsWith(clientAppointmentInterval) && start < 0 && end < 0)
				return false;
			start = Math.Max(start, 0);
			if (end < 0)
				end = this.cellsInfo.Count - 1;
			bool isFit = false;
			for (int i = start; i <= end; i++) {
				if (this.days[i] < appointmentCountPerCell)
					isFit = true;
			}
			if (!isFit)
				return false;
			if (this.days[start] >= appointmentCountPerCell)
				return false;
			for (int i = start; i <= end; i++) {
				this.days[i]++;
			}
			return true;
		}
		int GetIndexFromStartDate(DateTime date) {
			int count = this.cellsInfo.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = cellsInfo.GetIntervalByIndex(i);
				if (date < interval.End && interval.Contains(date))
					return i;
			}
			return -1;
		}
		int GetIndexFromEndDate(DateTime date) {
			int count = this.cellsInfo.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = cellsInfo.GetIntervalByIndex(i);
				if (interval.Start < date && interval.Contains(date))
					return i;
			}
			return -1;
		}
	}
	#endregion
}

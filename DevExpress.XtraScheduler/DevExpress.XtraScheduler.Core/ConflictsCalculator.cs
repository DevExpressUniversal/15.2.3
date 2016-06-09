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
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler {
	#region PrevNextAppointmentIntervalPair
	public class PrevNextAppointmentIntervalPair {
		#region Fields
		TimeInterval prevAppointmentInterval;
		TimeInterval nextAppointmentInterval;
		Resource resource = ResourceBase.Empty;
		#endregion
		public PrevNextAppointmentIntervalPair(Resource resource, TimeInterval prev, TimeInterval next) {
			this.resource = resource;
			this.prevAppointmentInterval = prev;
			this.nextAppointmentInterval = next;
		}
		#region Properties
		public TimeInterval PrevAppointmentInterval { get { return prevAppointmentInterval; } }
		public TimeInterval NextAppointmentInterval { get { return nextAppointmentInterval; } }
		public Resource Resource { get { return resource; } }
		#endregion
	}
	#endregion
	#region PrevNextAppointmentIntervalPairCollection
	public class PrevNextAppointmentIntervalPairCollection : List<PrevNextAppointmentIntervalPair> {
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentConflictsCalculator
	public class AppointmentConflictsCalculator {
		AppointmentBaseCollection appointments;
		AppointmentBaseComparer appointmentComparer;
		public AppointmentConflictsCalculator(AppointmentBaseCollection appointments) {
			if (appointments == null)
				Exceptions.ThrowArgumentException("appointments", appointments);
			this.appointments = appointments;
			this.appointmentComparer = AppointmentComparerProvider.CreateDefaultAppointmentComparer();  
		}
		protected internal AppointmentBaseCollection Appointments { get { return appointments; } }
		protected internal AppointmentBaseComparer AppointmentComparer { get { return appointmentComparer; } }
		public virtual AppointmentBaseCollection CalculateConflicts(Appointment appointment, TimeInterval interval) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (apt.Type != AppointmentType.Pattern && !((IInternalAppointment)apt).GetInterval().IntersectsWith(interval))
					continue;
				if (AreAppointmentResourcesIntersect(appointment, apt) && AreIntersecting(apt, appointment)) {
					if (apt.Type == AppointmentType.Pattern)
						result.AddRange(GetConflictedOccurrences(apt, appointment, interval));
					else
						result.Add(apt);
				}
			}
			return result;
		}
		protected internal virtual bool AreAppointmentResourcesIntersect(Appointment apt1, Appointment apt2) {
			return ResourceBase.InternalAreResourceIdsCollectionsIntersect(apt1.ResourceIds, apt2.ResourceIds);
		}
		protected internal virtual bool AreIntersecting(Appointment apt1, Appointment apt2) {
			if (Object.ReferenceEquals(apt1, apt2))
				return false;
			TimeInterval apt2Interval = ((IInternalAppointment)apt2).GetInterval();
			if (apt1.Type == AppointmentType.Pattern && apt2.Type != AppointmentType.Pattern)
				return IsIntervalIntersectingPattern(apt1, apt2Interval);
			TimeInterval apt1Interval = ((IInternalAppointment)apt1).GetInterval();
			if (apt1.Type != AppointmentType.Pattern && apt2.Type == AppointmentType.Pattern)
				return IsIntervalIntersectingPattern(apt2, apt1Interval);
			if (apt1.Type == AppointmentType.Pattern && apt2.Type == AppointmentType.Pattern)
				return ArePatternsIntersecting(apt1, apt2);
			return apt1Interval.IntersectsWithExcludingBounds(apt2Interval);
		}
		protected internal virtual AppointmentBaseCollection GetPatternOccurrences(Appointment pattern, TimeInterval interval) {
			AppointmentPatternExpander expander = new AppointmentPatternExpander(pattern);
			return expander.Expand(interval);
		}
		protected internal virtual bool IsIntervalIntersectingPattern(Appointment pattern, TimeInterval interval) {
			AppointmentBaseCollection occurrences = GetPatternOccurrences(pattern, interval);
			return occurrences.Count != 0;
		}
		protected internal virtual bool ArePatternsIntersecting(Appointment pattern1, Appointment pattern2) {
			if (pattern1.RecurrenceInfo.Range == RecurrenceRange.NoEndDate && pattern2.RecurrenceInfo.Range == RecurrenceRange.NoEndDate)
				return false; 
			if (pattern1.RecurrenceInfo.Range == RecurrenceRange.NoEndDate && pattern2.RecurrenceInfo.Range == RecurrenceRange.NoEndDate) {
				if (pattern1.RecurrenceInfo.Duration < pattern2.RecurrenceInfo.Duration)
					return IsPatternIntersectingFinitePattern(pattern2, pattern1);
				else
					return IsPatternIntersectingFinitePattern(pattern1, pattern2);
			}
			else {
				if (pattern1.RecurrenceInfo.Range == RecurrenceRange.NoEndDate)
					return IsPatternIntersectingFinitePattern(pattern1, pattern2);
				else
					return IsPatternIntersectingFinitePattern(pattern2, pattern1);
			}
		}
		protected internal virtual bool IsPatternIntersectingFinitePattern(Appointment pattern, Appointment finitePattern) {
			TimeInterval interval = CalcPatternInterval(finitePattern);
			AppointmentBaseCollection occurrences1 = PreparePatternOccurrences(pattern, interval);
			AppointmentBaseCollection occurrences2 = PreparePatternOccurrences(finitePattern, interval);
			return AreAppointmentSequencesIntersect(occurrences1, occurrences2);
		}
		protected internal virtual AppointmentBaseCollection PreparePatternOccurrences(Appointment pattern, TimeInterval interval) {
			AppointmentBaseCollection result = GetPatternOccurrences(pattern, interval);
			result.Sort(appointmentComparer);
			return result;
		}
		protected internal virtual bool AreAppointmentSequencesIntersect(AppointmentBaseCollection occurrences1, AppointmentBaseCollection occurrences2) {
			int count1 = occurrences1.Count;
			int count2 = occurrences2.Count;
			int index1 = 0;
			int index2 = 0;
			while (index1 < count1 && index2 < count2) {
				Appointment apt1 = occurrences1[index1];
				Appointment apt2 = occurrences2[index2];
				if (((IInternalAppointment)apt1).GetInterval().IntersectsWithExcludingBounds(((IInternalAppointment)apt2).GetInterval()))
					return true;
				int compareResult = appointmentComparer.Compare(apt1, apt2);
				XtraSchedulerDebug.Assert(compareResult != 0);
				if (compareResult < 0)
					index1++;
				else
					index2++;
			}
			return false;
		}
		protected internal virtual TimeInterval CalcPatternInterval(Appointment pattern) {
			OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			IndexInterval indexInterval = new IndexInterval(0, calc.CalcLastOccurrenceIndex(pattern));
			int aptsCount = indexInterval.End + 1;
			AppointmentBaseCollection exceptions = pattern.GetExceptions();
			InternalTimeInterval result = CalcTotalExceptionsInterval(exceptions);
			TrimIndexInterval(exceptions, indexInterval);
			if (indexInterval.Start < aptsCount || aptsCount == 0) {
				DateTime firstOccurrenceStart = calc.CalcOccurrenceStartTime(indexInterval.Start);
				result.Start = DateTimeHelper.Min(result.Start, firstOccurrenceStart);
			}
			if (indexInterval.End != -1) {
				DateTime lastOccurrenceEnd = calc.CalcOccurrenceStartTime(indexInterval.End).Add(pattern.Duration);
				result.End = DateTimeHelper.Max(result.End, lastOccurrenceEnd);
			}
			if (result.End == DateTime.MinValue)
				result.End = DateTime.MaxValue;
			return new TimeInterval(result.Start, result.End);
		}
		protected internal virtual void TrimIndexInterval(AppointmentBaseCollection exceptions, IndexInterval indexInterval) {
			int count = exceptions.Count;
			if (count <= 0)
				return;
			int[] exceptionRecurrenceIndexes = new int[count];
			for (int i = 0; i < count; i++)
				exceptionRecurrenceIndexes[i] = exceptions[i].RecurrenceIndex;
			Array.Sort(exceptionRecurrenceIndexes);
			indexInterval.Start = TrimIndexFromStart(exceptionRecurrenceIndexes, indexInterval.Start);
			indexInterval.End = TrimIndexFromEnd(exceptionRecurrenceIndexes, indexInterval.End);
		}
		protected internal virtual int TrimIndexFromStart(int[] exceptionRecurrenceIndexes, int startIndex) {
			int count = exceptionRecurrenceIndexes.Length;
			while (startIndex < count && exceptionRecurrenceIndexes[startIndex] == startIndex)
				startIndex++;
			return startIndex;
		}
		protected internal virtual int TrimIndexFromEnd(int[] exceptionRecurrenceIndexes, int endIndex) {
			int i = exceptionRecurrenceIndexes.Length - 1;
			while (i >= 0 && exceptionRecurrenceIndexes[i] == endIndex) {
				i--;
				endIndex--;
			}
			return endIndex;
		}
		protected internal virtual InternalTimeInterval CalcTotalExceptionsInterval(AppointmentBaseCollection exceptions) {
			int count = exceptions.Count;
			if (count <= 0)
				return new InternalTimeInterval(DateTime.MaxValue, DateTime.MinValue);
			else {
				DateTime start = DateTime.MaxValue;
				DateTime end = DateTime.MinValue;
				for (int i = 0; i < count; i++) {
					Appointment exception = exceptions[i];
					if (exception.Type != AppointmentType.DeletedOccurrence) {
						start = DateTimeHelper.Min(start, exception.Start);
						end = DateTimeHelper.Max(end, exception.End);
					}
				}
				return new InternalTimeInterval(start, end);
			}
		}
		protected internal virtual AppointmentBaseCollection GetConflictedOccurrences(Appointment pattern, Appointment appointment, TimeInterval interval) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			AppointmentBaseCollection occurrences = GetPatternOccurrences(pattern, interval);
			int count = occurrences.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = occurrences[i];
				if (AreIntersecting(apt, appointment))
					result.Add(apt);
			}
			return result;
		}
		public virtual bool IsIntersecting(TimeInterval interval, Resource resource) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (AreIntersecting(apt, interval) && ResourceBase.InternalMatchIdToResourceIdCollection(apt.ResourceIds, resource.Id))
					return true;
			}
			return false;
		}
		protected internal virtual bool AreIntersecting(Appointment apt, TimeInterval interval) {
			if (apt.Type == AppointmentType.Pattern)
				return IsIntervalIntersectingPattern(apt, interval);
			return ((IInternalAppointment)apt).GetInterval().IntersectsWithExcludingBounds(interval);
		}
	}
	#endregion
	#region InternalTimeInterval
	public class InternalTimeInterval {
		DateTime start;
		DateTime end;
		public InternalTimeInterval(DateTime start, DateTime end) {
			this.start = start;
			this.end = end;
		}
		public DateTime Start { get { return start; } set { start = value; } }
		public DateTime End { get { return end; } set { end = value; } }
	}
	#endregion
	#region IndexInterval
	public class IndexInterval {
		int start;
		int end;
		public IndexInterval(int start, int end) {
			this.start = start;
			this.end = end;
		}
		public int Start { get { return start; } set { start = value; } }
		public int End { get { return end; } set { end = value; } }
	}
	#endregion
	#region ResourceAppointmentsPredicatePair
	public class ResourceAppointmentsPredicatePair {
		Resource resource;
		SchedulerPredicate<Appointment> predicate;
		public ResourceAppointmentsPredicatePair(Resource resource, SchedulerPredicate<Appointment> predicate) {
			Guard.ArgumentNotNull(resource, "resource");
			Guard.ArgumentNotNull(predicate, "predicate");
			this.resource = resource;
			this.predicate = predicate;
		}
		public Resource Resource { get { return resource; } }
		public SchedulerPredicate<Appointment> Predicate { get { return predicate; } }
	}
	#endregion
	#region ResourcePredicatePairCollection
	public class ResourcePredicatePairCollection : List<ResourceAppointmentsPredicatePair> {
	}
	#endregion
	#region PrevNextAppointmentCalculator
	public class PrevNextAppointmentCalculator {
		ISchedulerStorageBase storage;
		IPredicate<Appointment> externalAppointmentFilter;
		IPredicate<OccurrenceInfo> externalOccurrenceInfoFilter;
		TimeInterval limitInterval = NotificationTimeInterval.FullRange.Clone();
		public PrevNextAppointmentCalculator(IPredicate<Appointment> externalFilter, IPredicate<OccurrenceInfo> externalOccurrenceInfoFilter) {
			Guard.ArgumentNotNull(externalFilter, "externalAppointmentFilter");
			Guard.ArgumentNotNull(externalOccurrenceInfoFilter, "externalOccurrenceInfoFilter");
			this.externalAppointmentFilter = externalFilter;
			this.externalOccurrenceInfoFilter = externalOccurrenceInfoFilter;
		}
		public PrevNextAppointmentCalculator(ISchedulerStorageBase storage, IPredicate<Appointment> externalFilter, IPredicate<OccurrenceInfo> externalOccurrenceInfoFilter) {
			Guard.ArgumentNotNull(externalFilter, "externalAppointmentFilter");
			Guard.ArgumentNotNull(externalOccurrenceInfoFilter, "externalOccurrenceInfoFilter");
			this.externalAppointmentFilter = externalFilter;
			this.externalOccurrenceInfoFilter = externalOccurrenceInfoFilter;
			this.storage = storage;
		}
		protected internal IPredicate<Appointment> ExternalAppointmentFilter { get { return externalAppointmentFilter; } }
		protected internal IPredicate<OccurrenceInfo> ExternalOccurrenceInfoFilter { get { return externalOccurrenceInfoFilter; } }
		protected internal TimeInterval LimitInterval { get { return limitInterval; } set { limitInterval = value; } }
		protected DateTime MinDateTime { get { return LimitInterval.Start; } }
		protected DateTime MaxDateTime { get { return LimitInterval.End; } }
		protected ISchedulerStorageBase Storage { get { return storage; } }
		public virtual List<PrevNextAppointmentIntervalPair> CalculatePrevNextAppointments(AppointmentResourcesMatchFilter filter, TimeInterval interval) {
			List<PrevNextAppointmentIntervalPair> result = new List<PrevNextAppointmentIntervalPair>();
			ResourceBaseCollection resources = filter.Resources;
			if (resources == null || resources.Count == 0)
				return result;
			Dictionary<object, Appointment> prevApts = FindNearestAppointmentIntervalBefore(interval.Start, filter);
			Dictionary<object, Appointment> nextApts = FindNearestAppointmentIntervalAfter(interval.End, filter);
			for (int i = 0; i < resources.Count; i++) {
				object resourceId = resources[i].Id;
				TimeInterval prevInterval = null;
				TimeInterval nextInterval = null;
				if (resourceId != null) {
					if (prevApts != null && prevApts[resourceId] != null)
						prevInterval = ((IInternalAppointment)prevApts[resourceId]).GetInterval();
					if (nextApts != null && nextApts[resourceId] != null)
						nextInterval = ((IInternalAppointment)nextApts[resourceId]).GetInterval();
				}
				PrevNextAppointmentIntervalPair item = new PrevNextAppointmentIntervalPair(resources[i], prevInterval, nextInterval);
				result.Add(item);
			}
			return result;
		}
		protected internal virtual Dictionary<object, Appointment> FindNearestAppointmentIntervalBefore(DateTime date, AppointmentResourcesMatchFilter filter) {
			return FindNearestAppointmentIntervalBase(date, filter, true);
		}
		protected internal virtual Dictionary<object, Appointment> FindNearestAppointmentIntervalAfter(DateTime date, AppointmentResourcesMatchFilter filter) {
			return FindNearestAppointmentIntervalBase(date, filter, false);
		}
		Dictionary<object, Appointment> FindNearestAppointmentIntervalBase(DateTime date, AppointmentResourcesMatchFilter filter, bool findPrevApt) {
			ResourceBaseCollection resources = filter.Resources;
			if (storage == null || resources == null || resources.Count == 0)
				return null;
			if (!LimitInterval.Contains(date))
				return null;
			TimeInterval searchInterval = (findPrevApt) ? new TimeInterval(LimitInterval.Start, date) : new TimeInterval(date, LimitInterval.End);
			return Storage.FindNearestAppointmentInterval(searchInterval, filter, findPrevApt);
		}
		protected internal virtual TimeInterval FindNearestAppointmentIntervalBefore(AppointmentBaseCollection appointments, AppointmentBaseCollection patterns, DateTime date) {
			TimeInterval nearestAppointmentInterval = FindNearestAppointmentIntervalBeforeCore(appointments, date);
			if (nearestAppointmentInterval.End > date) 
				return null;
			TimeInterval interval = new TimeInterval(nearestAppointmentInterval.End, date);
			TimeInterval nearestOccurrenceInterval = FindNearestOccurrenceIntervalBeforeCore(patterns, interval);
			if (nearestAppointmentInterval.End > nearestOccurrenceInterval.End)
				interval = nearestAppointmentInterval;
			else
				interval = nearestOccurrenceInterval;
			if (interval.Start > MinDateTime)
				return interval;
			else
				return null;
		}
		protected internal virtual TimeInterval FindNearestAppointmentIntervalAfter(AppointmentBaseCollection appointments, AppointmentBaseCollection patterns, DateTime date) {
			TimeInterval nearestAppointmentInterval = FindNearestAppointmentIntervalAfterCore(appointments, date);
			if (nearestAppointmentInterval.Start < date) 
				return null;
			TimeInterval interval = new TimeInterval(date, nearestAppointmentInterval.Start);
			TimeInterval nearestOccurrenceInterval = FindNearestOccurrenceIntervalAfterCore(patterns, interval);
			if (nearestAppointmentInterval.Start < nearestOccurrenceInterval.Start)
				interval = nearestAppointmentInterval;
			else
				interval = nearestOccurrenceInterval;
			if (interval.Start < MaxDateTime)
				return interval;
			else
				return null;
		}
		protected internal virtual bool IsAppointmentBelongsToIntervalBefore(Appointment apt, TimeInterval interval) {
			DateTime appointmentEnd = apt.End;
			return appointmentEnd > interval.Start && (appointmentEnd < interval.End || (appointmentEnd <= interval.End && apt.Duration.Ticks > 0));
		}
		protected internal virtual TimeInterval FindPrevAppointmentInAppointmentTree(AppointmentBaseCollection appointments, DateTime date) {
			return new TimeInterval(MinDateTime, TimeSpan.Zero);
		}
		protected internal virtual TimeInterval FindNearestAppointmentIntervalBeforeCore(AppointmentBaseCollection appointments, DateTime date) {
			int count = appointments.Count;
			if (count <= 0)
				return new TimeInterval(MinDateTime, TimeSpan.Zero);
			date = Algorithms.Max(MinDateTime, date);
			TimeInterval interval = new TimeInterval(MinDateTime, date);
			Appointment result = null;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (IsAppointmentBelongsToIntervalBefore(apt, interval)) {
					if (externalAppointmentFilter.Calculate(apt)) {
						interval = new TimeInterval(apt.End, date);
						result = apt;
					}
				}
			}
			if (result != null)
				return ((IInternalAppointment)result).GetInterval().Clone();
			else
				return new TimeInterval(MinDateTime, TimeSpan.Zero);
		}
		protected internal virtual TimeInterval FindNearestAppointmentIntervalAfterCore(AppointmentBaseCollection appointments, DateTime date) {
			int count = appointments.Count;
			if (count <= 0)
				return new TimeInterval(MaxDateTime, TimeSpan.Zero);
			DateTime minStartDateAfter = MaxDateTime;
			Appointment result = null;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				DateTime start = apt.Start;
				if (start < minStartDateAfter && start >= date) {
					if (externalAppointmentFilter.Calculate(apt)) {
						minStartDateAfter = start;
						result = apt;
					}
				}
			}
			if (result != null)
				return ((IInternalAppointment)result).GetInterval().Clone();
			else
				return new TimeInterval(MaxDateTime, TimeSpan.Zero);
		}
		protected internal virtual TimeInterval FindNextAppointmentInAppointmentTree(AppointmentBaseCollection appointments, DateTime date) {
			return new TimeInterval(MaxDateTime, TimeSpan.Zero);
		}
		protected internal virtual TimeInterval FindNearestOccurrenceIntervalBeforeCore(AppointmentBaseCollection patterns, TimeInterval interval) {
			TimeInterval result = new TimeInterval(MinDateTime, TimeSpan.Zero);
			int count = patterns.Count;
			for (int i = 0; i < count; i++) {
				Appointment pattern = patterns[i];
				if (ShouldSearchPatternOccurrencesAtInterval(pattern, interval)) {
					OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
					OccurrenceInfo occurrenceInfo = calc.FindMaxTimeOfOccurrenceInsideInterval(interval, pattern, externalOccurrenceInfoFilter);
					if (occurrenceInfo.Index >= 0) {
						if (occurrenceInfo.Start + pattern.Duration > result.End) {
							result.Start = occurrenceInfo.Start;
							result.Duration = pattern.Duration;
							interval = new TimeInterval(result.End, interval.End);
						}
					}
				}
			}
			return result;
		}
		protected internal virtual TimeInterval FindNearestOccurrenceIntervalAfterCore(AppointmentBaseCollection patterns, TimeInterval interval) {
			TimeInterval result = new TimeInterval(MaxDateTime, TimeSpan.Zero);
			int count = patterns.Count;
			for (int i = 0; i < count; i++) {
				Appointment pattern = patterns[i];
				if (ShouldSearchPatternOccurrencesAtInterval(pattern, interval)) {
					OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
					OccurrenceInfo occurrenceInfo = calc.FindMinTimeOfOccurrenceInsideInterval(interval, pattern, externalOccurrenceInfoFilter);
					if (occurrenceInfo.Start < interval.End) {
						result.Start = occurrenceInfo.Start;
						result.Duration = pattern.Duration;
						interval = new TimeInterval(interval.Start, result.Start);
					}
				}
			}
			return result;
		}
		protected internal virtual bool ShouldSearchPatternOccurrencesAtInterval(Appointment pattern, TimeInterval interval) {
			IRecurrenceInfo recurrenceInfo = pattern.RecurrenceInfo;
			if (recurrenceInfo.Start >= interval.End)
				return false;
			return (recurrenceInfo.Range != RecurrenceRange.EndByDate || recurrenceInfo.End > interval.Start);
		}
	}
	#endregion
	#region NavigationButtonsDataCriteriaCalculator
	public class NavigationButtonsDataCriteriaCalculator {
		InnerSchedulerControl innerControl;
		public NavigationButtonsDataCriteriaCalculator(InnerSchedulerControl innerControl) {
			if (innerControl == null)
				Exceptions.ThrowArgumentNullException("innerControl");
			this.innerControl = innerControl;
		}
		public InnerSchedulerControl InnerControl { get { return innerControl; } }
		public virtual ResourcePredicatePairCollection Calculate(ResourceBaseCollection visibleResources, ResourceBaseCollection filteredResources, AppointmentBaseCollection visibleAppointments, NavigationButtonVisibility visibility) {
			XtraSchedulerDebug.Assert(visibleResources.Count > 0);
			XtraSchedulerDebug.Assert(visibility != NavigationButtonVisibility.Never);
			XtraSchedulerDebug.Assert(!visibleResources.Contains(ResourceBase.Empty));
			XtraSchedulerDebug.Assert(!filteredResources.Contains(ResourceBase.Empty));
			ResourcePredicatePairCollection result = new ResourcePredicatePairCollection();
			int count = visibleResources.Count;
			for (int i = 0; i < count; i++) {
				Resource res = visibleResources[i];
				if (CanCreateCriteria(visibility, res, visibleAppointments))
					result.Add(CreateCriteria(res));
			}
			return result;
		}
		protected internal virtual bool CanCreateCriteria(NavigationButtonVisibility visibility, Resource resource, AppointmentBaseCollection appointments) {
			return visibility == NavigationButtonVisibility.Always || !HasAppointmentsMatchResource(resource, appointments);
		}
		protected internal virtual bool HasAppointmentsMatchResource(Resource resource, AppointmentBaseCollection appointments) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				if (ResourceBase.InternalMatchIdToResourceIdCollection(appointments[i].ResourceIds, resource.Id))
					return true;
			}
			return false;
		}
		protected internal virtual ResourceAppointmentsPredicatePair CreateCriteria(Resource resource) {
			ResourceBaseCollection resources = new ResourceBaseCollection();
			resources.Add(resource);
			SchedulerPredicate<Appointment> predicate = InnerControl.CreateResourcesAppointmentPredicate(resources);
			return new ResourceAppointmentsPredicatePair(resource, predicate);
		}
	}
	#endregion
	#region NavigationButtonsViewGroupByNoneDataCriteriaCalculator
	public class NavigationButtonsViewGroupByNoneDataCriteriaCalculator : NavigationButtonsDataCriteriaCalculator {
		public NavigationButtonsViewGroupByNoneDataCriteriaCalculator(InnerSchedulerControl innerControl)
			: base(innerControl) {
		}
		public override ResourcePredicatePairCollection Calculate(ResourceBaseCollection visibleResources, ResourceBaseCollection filteredResources, AppointmentBaseCollection visibleAppointments, NavigationButtonVisibility visibility) {
			XtraSchedulerDebug.Assert(visibility != NavigationButtonVisibility.Never);
			XtraSchedulerDebug.Assert(!visibleResources.Contains(ResourceBase.Empty));
			XtraSchedulerDebug.Assert(!filteredResources.Contains(ResourceBase.Empty));
			ResourcePredicatePairCollection result = new ResourcePredicatePairCollection();
			if (CanCreateCriteria(visibility, visibleResources, visibleAppointments)) {
				SchedulerPredicate<Appointment> predicate = InnerControl.CreateResourcesAppointmentPredicate(filteredResources);
				result.Add(new ResourceAppointmentsPredicatePair(ResourceBase.Empty, predicate));
			}
			return result;
		}
		protected internal virtual bool CanCreateCriteria(NavigationButtonVisibility visibility, ResourceBaseCollection resources, AppointmentBaseCollection appointments) {
			if (visibility == NavigationButtonVisibility.Always)
				return true;
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (apt.ResourceId == EmptyResourceId.Id || ResourceBase.InternalAreResourceIdsCollectionsIntersect(apt.ResourceIds, resources))
					return false;
			}
			return true;
		}
	}
	#endregion
	#region NavigationButtonCalculator (abstract class)
	public abstract class NavigationButtonCalculator {
		#region Fields
		InnerSchedulerControl innerControl;
		InnerSchedulerViewBase view;
		#endregion
		protected NavigationButtonCalculator(InnerSchedulerControl innerControl, InnerSchedulerViewBase view) {
			Guard.ArgumentNotNull(innerControl, "innerControl");
			Guard.ArgumentNotNull(view, "view");
			this.innerControl = innerControl;
			this.view = view;
		}
		#region Properties
		protected internal InnerSchedulerControl InnerControl { get { return innerControl; } }
		protected internal InnerSchedulerViewBase View { get { return view; } }
		#endregion
		public PrevNextAppointmentIntervalPairCollection Calculate() {
			if (!CanShowNavigationButtons())
				return null;
			AppointmentBaseCollection appointments = GetVisibleAppointments();
			NavigationButtonsDataCriteriaCalculator criteriaCalc = CreateCriteriaCalculator();
			NavigationButtonVisibility actualVisibility = CalculateActualNavigationButtonVisibility(); 
			ResourcePredicatePairCollection criteria = criteriaCalc.Calculate(View.VisibleResources, View.FilteredResources , appointments, actualVisibility);
			if (criteria.Count <= 0)
				return null;
			return CalculatePairs(criteria);
		}
		protected internal virtual NavigationButtonsDataCriteriaCalculator CreateCriteriaCalculator() {
			if (UseGroupByNoneCriteriaCalculator())
				return new NavigationButtonsViewGroupByNoneDataCriteriaCalculator(innerControl);
			else
				return new NavigationButtonsDataCriteriaCalculator(innerControl);
		}
		protected internal virtual IPredicate<Appointment> CreateAppointmentExternalFilter() {
			if (innerControl.Storage != null)
				return innerControl.Storage.CreateAppointmentExternalFilter();
			else
				return new EmptyAppointmentPredicate();
		}
		protected internal virtual IPredicate<OccurrenceInfo> CreateOccurrenceInfoExternalFilter() {
			if (innerControl.Storage != null)
				return innerControl.Storage.CreateOccurrenceInfoExternalFilter();
			else
				return new EmptyOccurrenceInfoPredicate();
		}
		protected internal virtual PrevNextAppointmentIntervalPairCollection CalculatePairs(ResourcePredicatePairCollection criteria) {
			TimeInterval visibleInterval = ApplyTimeZone(View.InnerVisibleIntervals.Interval);
			PrevNextAppointmentIntervalPairCollection result = new PrevNextAppointmentIntervalPairCollection();
			PrevNextAppointmentCalculator calc = CreatePrevNextAppointmentCalculator();
			AppointmentResourcesMatchFilter filter = InnerControl.CreateAppointmentResourcesMatchFilter(criteria);
			CompositeAppointmentPredicate predicate = new CompositeAppointmentPredicate();
			predicate.Items.Add(new NotPredicate<Appointment>(new AppointmentIntersectsIntervalPredicate(visibleInterval)));
			predicate.Items.Add(calc.ExternalAppointmentFilter);
			filter.AppointmentExternalFilter = predicate;
			List<PrevNextAppointmentIntervalPair> intervals = calc.CalculatePrevNextAppointments(filter, visibleInterval);
			for (int i = 0; i < intervals.Count; i++)
				result.Add(intervals[i]);
			return result;
		}
		protected internal virtual TimeInterval ApplyTimeZone(TimeInterval interval) {
			return innerControl.ApplyTimeZone(interval);
		}
		protected internal virtual PrevNextAppointmentCalculator CreatePrevNextAppointmentCalculator() {
			IPredicate<Appointment> externalFilter = CreateAppointmentExternalFilter();
			IPredicate<OccurrenceInfo> externalOccurrenceInfoFilter = CreateOccurrenceInfoExternalFilter();
			PrevNextAppointmentCalculator calc = new PrevNextAppointmentCalculator(InnerControl.Storage, externalFilter, externalOccurrenceInfoFilter);
			calc.LimitInterval = CalculateSearchInterval();
			return calc;
		}
		protected internal virtual TimeInterval CalculateSearchInterval() {
			TimeSpan searchSpan = InnerControl.OptionsView.NavigationButtons.AppointmentSearchInterval;
			if (view.NavigationButtonAppointmentSearchInterval.Ticks > 0)
				searchSpan = view.NavigationButtonAppointmentSearchInterval;
			TimeInterval limitInterval = view.LimitInterval;
			TimeInterval visibleInterval = view.InnerVisibleIntervals.Interval;
			TimeInterval searchInterval = new TimeInterval(visibleInterval.Start - searchSpan, visibleInterval.End + searchSpan);
			return TimeInterval.Intersect(searchInterval, limitInterval);
		}
		protected internal virtual NavigationButtonVisibility CalculateActualNavigationButtonVisibility() {
			NavigationButtonVisibility result = View.NavigationButtonVisibility;
			if (result == NavigationButtonVisibility.Auto)
				result = InnerControl.OptionsView.NavigationButtons.Visibility;
			return result;
		}
		protected internal virtual bool CanShowNavigationButtons() {
			NavigationButtonVisibility visibility = CalculateActualNavigationButtonVisibility();
			if (visibility == NavigationButtonVisibility.Always)
				return true;
			else if (visibility == NavigationButtonVisibility.Auto) {
				if (View.GroupType == SchedulerGroupType.None)
					return View.FilteredAppointments.Count == 0;
				else
					return true;
			}
			else
				return false;
		}
		protected internal abstract AppointmentBaseCollection GetVisibleAppointments();
		protected internal abstract bool UseGroupByNoneCriteriaCalculator();
	}
	#endregion
}

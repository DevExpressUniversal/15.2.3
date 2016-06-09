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
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentTreeController
	public class AppointmentTreeController : IDisposable {
		internal const int MaxSearchDayCount = 256;
		IAppointmentTreeNodeEnumeratorFactory treeEnumerableFactory;
		AppointmentTree tree;
		AppointmentBaseCollection patterns;
		AppointmentBaseCollection changedOccurrences;
		Dictionary<Appointment, TimeInterval> appointmentIntervals = new Dictionary<Appointment, TimeInterval>();
		bool isLoaded;
		public AppointmentTreeController( AppointmentTree tree) {
			Guard.ArgumentNotNull(tree, "tree");
			this.tree = tree;
			this.patterns = new AppointmentBaseCollection(DXCollectionUniquenessProviderType.None);
			this.changedOccurrences = new AppointmentBaseCollection(DXCollectionUniquenessProviderType.None);
			Initialize();
		}
		protected void Initialize() {
		}
		protected AppointmentTree Tree { get { return tree; } }
		protected AppointmentBaseCollection Patterns { get { return patterns; } }
		protected AppointmentBaseCollection ChangedOccurrences { get { return changedOccurrences; } }
		protected IAppointmentTreeNodeEnumeratorFactory TreeEnumerableFactory { get { return treeEnumerableFactory; } }
		protected internal Dictionary<Appointment, TimeInterval> AppointmentIntervals { get { return appointmentIntervals; } }
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.tree != null) {
					this.tree.ClearAll();
					this.tree = null;
				}
				if (patterns != null) {
					patterns.Clear();
					patterns = null;
				}
				if (changedOccurrences != null) {
					changedOccurrences.Clear();
					changedOccurrences = null;
				}
				if (treeEnumerableFactory != null)
					treeEnumerableFactory = null;
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~AppointmentTreeController() {
			Dispose(false);
		}
		#endregion
		public virtual DateCollection FindAppointmentDates(TimeInterval clientInterval, AppointmentResourcesMatchFilter filter) {
			QueryAppointmentDatesTreeNodeVisitor visitor = CreateQueryAppointmentDatesVisitor(clientInterval, filter);
			EnumerateTreeNodes(visitor);
			visitor.PrepareResult();
			return visitor.Result;
		}
		public virtual DateCollection FindRecurringAppointmentDates(TimeInterval clientInterval, AppointmentResourcesMatchFilter filter) {
			DateCollection result = new DateCollection(DXCollectionUniquenessProviderType.MaximizePerformance);
			TimeZoneEngine tze = (filter.TimeZoneHelper != null) ? filter.TimeZoneHelper.StorageTimeZoneEngine : null;
			AppointmentBaseCollection recurring = FindAppointmentsExpandingPatterns(clientInterval, tze);
			int count = recurring.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = recurring[i];
				if (filter.MatchAppointmentResourceIds(apt)) {
					List<DateTime> aptDates = DateTimeHelper.CreateRoundedDateList(apt.Start, apt.End);
					result.AddRange(aptDates);
				}
			}
			return result;
		}
		public Dictionary<object, Appointment> FindPrevOccurrenceAppointment(TimeInterval searchInterval, AppointmentResourcesMatchFilter filter) {
			PrevOccurrenceCalculator prevOccurrenceCalculator = new PrevOccurrenceCalculator(Patterns);
			return prevOccurrenceCalculator.Find(searchInterval, filter);
		}
		public Dictionary<object, Appointment> FindNextOccurrenceAppointment(TimeInterval searchInterval, AppointmentResourcesMatchFilter filter) {
			NextOccurrenceCalculator nextOccurrenceCalculator = new NextOccurrenceCalculator(Patterns);
			return nextOccurrenceCalculator.Find(searchInterval, filter);
		}
		public AppointmentBaseCollection FindAppointmentsExpandingPatterns(TimeInterval interval, TimeZoneEngine timeZoneEngine) {
			FindRecurringAppointmentsCalculator calculator = new FindRecurringAppointmentsCalculator(Patterns, timeZoneEngine);
			return calculator.FindAppointmentsExpandingPatterns(interval);
		}
		QueryAppointmentDatesTreeNodeVisitor CreateQueryAppointmentDatesVisitor(TimeInterval clientInterval, AppointmentResourcesMatchFilter filter) {
			bool isServerTimeZone = CalculateIsServerTimeZone(filter.TimeZoneHelper);
			if (isServerTimeZone)
				return new QueryAppointmentDatesTreeNodeVisitor(clientInterval, filter);
			return new QueryClientAppointmentDatesTreeNodeVisitor(clientInterval, filter);
		}
		protected internal bool CalculateIsServerTimeZone(TimeZoneHelper timeZoneHelper) {
			if (timeZoneHelper == null)
				return false;			
			return timeZoneHelper.OperationTimeZone.Id == timeZoneHelper.ClientTimeZone.Id;
		}
		public Dictionary<object, Appointment> FindPrevAppointment(TimeInterval searchInterval, AppointmentResourcesMatchFilter filter) {
			PrevNextAppointmentTreeNodeVisitor visitor = CreateFindPrevNextAppointmentsVisitor(searchInterval, filter, true);
			visitor.EnumeratorFactory = new PrevAppointmentTreeNodeEnumerableFactory();
			AppointmentPrevTreeEnumerator en = new AppointmentPrevTreeEnumerator(Tree);
			en.Enumerate(visitor, AppointmentTreeNodeIntersectionType.Unknown);
			return visitor.Result;
		}
		public Dictionary<object, Appointment> FindNextAppointment(TimeInterval searchInterval, AppointmentResourcesMatchFilter filter) {
			PrevNextAppointmentTreeNodeVisitor visitor = CreateFindPrevNextAppointmentsVisitor(searchInterval, filter, false);
			visitor.EnumeratorFactory = new NextAppointmentTreeNodeEnumerableFactory();
			AppointmentNextTreeEnumerator en = new AppointmentNextTreeEnumerator(Tree);
			en.Enumerate(visitor, AppointmentTreeNodeIntersectionType.Unknown);
			return visitor.Result;
		}
		protected virtual PrevNextAppointmentTreeNodeVisitor CreateFindPrevNextAppointmentsVisitor(TimeInterval interval, AppointmentResourcesMatchFilter filter, bool prevApt) {
			return new PrevNextAppointmentTreeNodeVisitor(interval, filter, prevApt);
		}
		public AppointmentBaseCollection FindAppointments(TimeInterval interval) {
			QueryAppointmentsTreeNodeVisitor visitor = CreateFindAppointmentsVisitor(interval);
			EnumerateTreeNodes(visitor);
			return visitor.Result;
		}
		public virtual Dictionary<TimeInterval, AppointmentBaseCollection> FindAppointmentsByIntervals(TimeIntervalCollection intervals, AppointmentResourcesMatchFilter filter) {
			Dictionary<TimeInterval, AppointmentBaseCollection> result = new Dictionary<TimeInterval, AppointmentBaseCollection>();
			int count = intervals.Count;
			TimeZoneEngine tze = (filter.TimeZoneHelper != null) ? filter.TimeZoneHelper.StorageTimeZoneEngine : null;
			FindRecurringAppointmentsCalculator findRecurringAppointmentsCalculator = new FindRecurringAppointmentsCalculator(Patterns, tze);
			for (int i = 0; i < count; i++) {
				TimeInterval interval = intervals[i];
				AppointmentBaseCollection founded = FindAppointmentByIntervalCore(interval, filter);
				AppointmentBaseCollection foundedRecurringAppointments = findRecurringAppointmentsCalculator.FindViaIntervalAndFilter(interval, filter);
				founded.AddRange(foundedRecurringAppointments);
				result[interval] = founded;
			}
			return result;
		}
		internal virtual AppointmentBaseCollection FindAppointmentByIntervalCore(TimeInterval interval, AppointmentResourcesMatchFilter filter) {
			QueryClientAppointmentByIntervalTreeNodeVisitor visitor = CreateFindClientAppointmentByIntervalVisitor(interval, filter);
			EnumerateTreeNodes(visitor);
			return visitor.Result;
		}
		protected virtual void EnumerateTreeNodes(IAppointmentTreeNodeVisitor visitor) {
			AppointmentTreeNodeEnumeratorBase en = visitor.EnumeratorFactory.CreateTreeEnumerator(Tree);
			en.Enumerate(visitor, AppointmentTreeNodeIntersectionType.Unknown);
		}
		protected internal void UpdateAppointment(Appointment apt) {
			if (apt.Type == AppointmentType.DeletedOccurrence) { 
				if (ChangedOccurrences.Contains(apt))
					ChangedOccurrences.Remove(apt);
			} else if (apt.Type == AppointmentType.Normal) {
				DeleteAppointment(apt);
				if (Patterns.Contains(apt))
					Patterns.Remove(apt);
				InsertAppointment(apt);
			} else if (apt.Type == AppointmentType.Pattern && !Patterns.Contains(apt)) {
				DeleteNormalAppointmentFromTree(apt);
				Patterns.Add(apt);
			}
		}		
		protected internal void InsertAppointment(Appointment apt) {
			if (apt.Type == AppointmentType.Pattern && !Patterns.Contains(apt)) 
				Patterns.Add(apt);
			if (apt.Type == AppointmentType.ChangedOccurrence && !ChangedOccurrences.Contains(apt))
				ChangedOccurrences.Add(apt);
			if (apt.Type == AppointmentType.Normal) {
				Tree.Insert(apt);
			}
		}
		private void DeleteNormalAppointmentFromTree(Appointment apt) {
			UnregisterAppointmentInterval(apt);
			IAppointmentTreeNodeVisitor visitor = CreateDeleteAppointmentVisitor(apt, new TimeInterval(DateTime.MinValue, DateTime.MaxValue));
			EnumerateTreeNodes(visitor);
		}
		protected internal void DeleteAppointment(Appointment apt) {
			if (apt.Type == AppointmentType.Pattern && Patterns.Contains(apt)) {
				Patterns.Remove(apt);
			}
			if (apt.Type == AppointmentType.ChangedOccurrence && ChangedOccurrences.Contains(apt)) { 
				ChangedOccurrences.Remove(apt);
			}
			if (apt.Type == AppointmentType.Normal) {
				DeleteNormalAppointmentFromTree(apt);
			}
		}
		protected void RegisterAppointmentInterval(Appointment apt, TimeInterval interval) {
			if (!AppointmentIntervals.ContainsKey(apt))
				AppointmentIntervals[apt] = interval;
		}
		protected void UnregisterAppointmentInterval(Appointment apt) {
			if (AppointmentIntervals.ContainsKey(apt))
				AppointmentIntervals.Remove(apt);
		}
		protected TimeInterval GetOriginalAppointmentInterval(Appointment apt) {
			if (AppointmentIntervals.ContainsKey(apt))
				return AppointmentIntervals[apt];
			return ((IInternalAppointment)apt).CreateInterval();
		}
		protected virtual QueryAppointmentsTreeNodeVisitor CreateFindAppointmentsVisitor(TimeInterval interval) {
			return new QueryAppointmentsTreeNodeVisitor(interval);
		}
		protected virtual QueryClientAppointmentByIntervalTreeNodeVisitor CreateFindClientAppointmentByIntervalVisitor(TimeInterval interval, AppointmentResourcesMatchFilter filter) {
			return new QueryClientAppointmentByIntervalTreeNodeVisitor(interval, filter);
		}
		protected virtual DeleteAppointmentTreeNodeVisitor CreateDeleteAppointmentVisitor(Appointment apt, TimeInterval appointmentInterval) {
			return new DeleteAppointmentTreeNodeVisitor(apt, appointmentInterval);
		}
		public virtual void OnAppointmentCollectionCleared() {
			ClearAppointments();
		}
		protected virtual void ClearAppointments() {
			Tree.ClearAll();
			Patterns.Clear();
			AppointmentIntervals.Clear();
		}
		public virtual void LoadAppointments(AppointmentBaseCollection appointments) {
			ClearAppointments();
			OnAppointmentsInserted(appointments);
			isLoaded = true;
		}
		public virtual void OnAppointmentCollectionLoaded(AppointmentBaseCollection appointments) {
			if (!isLoaded) {
				ClearAppointments();
				OnAppointmentsInserted(appointments);
			}
			isLoaded = false;
		}
		public virtual void OnAppointmentsInserted(AppointmentBaseCollection appointments) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
#if DEBUGTEST
				System.Diagnostics.Debug.Assert(appointments[i].IsDisposed == false);
#endif
				InsertAppointment(appointments[i]);
			}
		}
		public virtual void OnAppointmentsChanging(Appointment appointment, AppointmentChangeStateData data) {
			XtraSchedulerDebug.Assert(data != null);
			RegisterAppointmentInterval(appointment, data.Interval);
		}
		public virtual void OnAppointmentsChanged(AppointmentBaseCollection appointments) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				UpdateAppointment(appointments[i]);
			}
		}
		public virtual void OnAppointmentsDeleted(AppointmentBaseCollection appointments) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				DeleteAppointment(appointments[i]);
			}
		}
	}
	#endregion
	public static class AppointmentClientIntervalMatchHelper {
		public static bool MatchAppointmentClientInterval(TimeInterval searchInterval, DateTime start, DateTime end, Appointment appointment, TimeZoneHelper tzHelper) {
			TimeInterval appointmentInterval = ((IInternalAppointment)appointment).GetInterval();
			TimeInterval clientInterval = tzHelper.ToClientTime(appointmentInterval, true); 
			if (!clientInterval.IntersectsWith(searchInterval)) {
				return false;
			}
			if (clientInterval.Contains(searchInterval))
				clientInterval = searchInterval;
			else if (clientInterval.Start < searchInterval.Start)
				clientInterval = new TimeInterval(searchInterval.Start, clientInterval.End);
			else if (clientInterval.End > searchInterval.End)
				clientInterval = new TimeInterval(clientInterval.Start, searchInterval.End);
			if (searchInterval.Duration == TimeSpan.Zero)
				return AppointmentTreeHelper.IntersectsWith(searchInterval, clientInterval.Start, clientInterval.End);
			bool result = AppointmentTreeHelper.IntersectsWithExcludingBounds(searchInterval, clientInterval.Start, clientInterval.End);
			if (appointmentInterval.Duration == TimeSpan.Zero)
				return result || (searchInterval.Start == clientInterval.Start);
			else
				return result;
		}
	}
	#region AppointmentTreeHelper
	public static class AppointmentTreeHelper {
		public static TimeInterval CalculateActualSearchInterval(TimeInterval interval) {
			if (TimeInterval.Empty.Equals(interval))
				return interval;
			if (interval.Duration == TimeSpan.Zero)
				return new TimeInterval(interval.Start.AddTicks(-1), interval.End);
			return interval;
		}
		public static AppointmentTreeNodeIntersectionType CalculateNodeIntersectionType(TimeInterval interval, AppointmentTreeNode node) {
			if (TimeInterval.Empty.Equals(interval))
				return AppointmentTreeNodeIntersectionType.Unknown; 
			bool isIntersects = IntersectsWith(interval, node.Start, node.End);
			if (!isIntersects) {
				if (node.End < interval.Start)
					return AppointmentTreeNodeIntersectionType.BeyondAtLeft;
				if (node.Start > interval.End)
					return AppointmentTreeNodeIntersectionType.BeyondAtRight;
			}
			if (IntervalContainsRange(interval, node.Start, node.End))
				return AppointmentTreeNodeIntersectionType.Full;
			if (NodeIntersectsInterval(interval, node)) {
				return AppointmentTreeNodeIntersectionType.Partial;
			}
			if (!TimeInterval.Empty.Equals(interval)) 
				return AppointmentTreeNodeIntersectionType.None;
			return AppointmentTreeNodeIntersectionType.Unknown;
		}
		public static bool IntervalContainsRange(TimeInterval baseInterval, DateTime start, DateTime end) {
			return baseInterval.Start <= start && end <= baseInterval.End;
		}
		public static bool IntervalIntersectsRange(TimeInterval baseInterval, DateTime start, DateTime end) {
			if (baseInterval.Duration == TimeSpan.Zero)
				return IntersectsWith(baseInterval, start, end);
			bool result = IntersectsWithExcludingBounds(baseInterval, start, end);
			if (start == end)
				return result || (baseInterval.Start == start);
			else
				return result;
		}
		internal static bool IntersectsWithExcludingBounds(TimeInterval baseInterval, DateTime start, DateTime end) {
			if (baseInterval.Duration == TimeSpan.Zero && start == end && baseInterval.Start == start)
				return true; 
			return baseInterval.End > start && baseInterval.Start < end;
		}
		internal static bool IntersectsWith(TimeInterval baseInterval, DateTime start, DateTime end) {
			return baseInterval.End >= start && baseInterval.Start <= end;
		}
		public static bool NodeIntersectsInterval(TimeInterval baseInterval, AppointmentTreeNode node) {
			return IntervalIntersectsRange(baseInterval, node.Start, node.End);
		}
		public static bool Intersects(TimeInterval interval, TimeInterval appointmentInterval) {
			return interval.IntersectsWith(appointmentInterval);
		}
		public static bool IsNoneIntersection(AppointmentTreeNodeIntersectionType intersection) {
			return intersection == AppointmentTreeNodeIntersectionType.None ||
				intersection == AppointmentTreeNodeIntersectionType.BeyondAtLeft ||
				intersection == AppointmentTreeNodeIntersectionType.BeyondAtRight;
		}
		public static bool ShouldCheckNodeInterval(AppointmentTreeNodeIntersectionType intersection) {
			return intersection == AppointmentTreeNodeIntersectionType.Partial;
		}
		public static Dictionary<object, Appointment> CreateResourceAppointmentCache(ResourceBaseCollection resources) {
			Dictionary<object, Appointment> result = new Dictionary<object, Appointment>();
			if (resources == null)
				return result;
			for (int i = 0; i < resources.Count; i++) {
				object resourceId = resources[i].Id;
				if (resourceId != null && !result.ContainsKey(resourceId))
					result.Add(resourceId, null);
			}
			return result;
		}
	}
	#endregion
	public class NextOccurrenceCalculator : PrevNexOccurrenceCalculatorBase {
		public NextOccurrenceCalculator(AppointmentBaseCollection patterns)
			: base(patterns) {
		}
		protected override bool IsCurrentAptNearest(Dictionary<object, Appointment> result, AppointmentBaseCollection apts, int i, object aptResourceId) {
			return result[aptResourceId].Start > apts[i].Start;
		}
		protected override AppointmentBaseCollection ExpandOccurrences(AppointmentPatternExpander expander, TimeInterval interval) {
			return expander.ExpandFirstOccurrences(interval);
		}
	}
	public class PrevOccurrenceCalculator : PrevNexOccurrenceCalculatorBase {
		public PrevOccurrenceCalculator(AppointmentBaseCollection patterns)
			: base(patterns) {
		}
		protected override bool IsCurrentAptNearest(Dictionary<object, Appointment> result, AppointmentBaseCollection apts, int i, object aptResourceId) {
			return result[aptResourceId].End < apts[i].End;
		}
		protected override AppointmentBaseCollection ExpandOccurrences(AppointmentPatternExpander expander, TimeInterval interval) {
			return expander.ExpandLastOccurrences(interval);
		}
	}
	public abstract class PrevNexOccurrenceCalculatorBase : FindRecurringAppointmentsCalculatorBase {
		protected PrevNexOccurrenceCalculatorBase(AppointmentBaseCollection patterns)
			: base(patterns) {
		}
		public Dictionary<object, Appointment> Find(TimeInterval searchInterval, AppointmentResourcesMatchFilter filter) {
			Dictionary<object, Appointment> result = AppointmentTreeHelper.CreateResourceAppointmentCache(filter.Resources);
			AppointmentBaseCollection apts = FindViaIntervalAndFilter(searchInterval, filter);
			if (apts.Count > 0)
				result[ResourceBase.Empty] = apts[0];
			for (int i = 0; i < apts.Count; i++) {
				if (IsCurrentAptNearest(result, apts, i, ResourceBase.Empty))
					result[ResourceBase.Empty] = apts[i];
				for (int k = 0; k < apts[i].ResourceIds.Count; k++) {
					object aptResourceId = apts[i].ResourceIds[k];
					if (result.ContainsKey(aptResourceId)) {
						if (result[aptResourceId] == null)
							result[aptResourceId] = apts[i];
						else {
							if (IsCurrentAptNearest(result, apts, i, aptResourceId))
								result[aptResourceId] = apts[i];
						}
					}
				}
			}
			return result;
		}
		protected abstract bool IsCurrentAptNearest(Dictionary<object, Appointment> result, AppointmentBaseCollection apts, int i, object aptResourceId);
	}
	public abstract class FindRecurringAppointmentsCalculatorBase {
		readonly AppointmentBaseCollection patterns;
		protected FindRecurringAppointmentsCalculatorBase(AppointmentBaseCollection patterns) {
			Guard.ArgumentNotNull(patterns, "patterns");
			this.patterns = patterns;
		}
		public AppointmentBaseCollection Patterns { get { return patterns; } }
		public AppointmentBaseCollection FindViaIntervalAndFilter(TimeInterval clientInterval, AppointmentResourcesMatchFilter filter) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			AppointmentBaseCollection recurring = FindAppointmentsExpandingPatterns(clientInterval);
			int count = recurring.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = recurring[i];
				if (filter.MatchAppointmentResourceIds(apt))
					result.Add(apt);
			}
			return result;
		}
		public AppointmentBaseCollection FindAppointmentsExpandingPatterns(TimeInterval interval) {
			AppointmentBaseCollection result = new AppointmentBaseCollection(DXCollectionUniquenessProviderType.None);
			AppointmentBaseCollection appointments = Patterns;
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				AppointmentPatternExpander expander = new AppointmentPatternExpander(apt);
				AppointmentBaseCollection expandedOccurrences = ExpandOccurrences(expander, interval);
				result.AddRange(expandedOccurrences);
			}
			return result;
		}		
		protected abstract AppointmentBaseCollection ExpandOccurrences(AppointmentPatternExpander expander, TimeInterval interval);
	}
	public class FindRecurringAppointmentsCalculator : FindRecurringAppointmentsCalculatorBase {
		public FindRecurringAppointmentsCalculator(AppointmentBaseCollection patterns, TimeZoneEngine timeZoneEngine)
			: base(patterns) {
		}
		protected override AppointmentBaseCollection ExpandOccurrences(AppointmentPatternExpander expander, TimeInterval interval) {
			return expander.Expand(interval);
		}
	}
}

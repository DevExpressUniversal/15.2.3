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
using System.Collections;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Native {
	public enum AppointmentTreeNodeIntersectionType { Unknown, None, Partial, Full, BeyondAtLeft, BeyondAtRight };
	public interface IAppointmentTreeNodeVisitor {
		TimeInterval SearchInterval { get; }
		bool VisitCompleted { get; }
		IAppointmentTreeNodeEnumeratorFactory EnumeratorFactory { get; }
		void VisitDayNode(AppointmentDayTreeNode node, bool checkInterval);
		void VisitCompositeNode(AppointmentTreeCompositeNode node, bool checkInterval);
	}
	#region AppointmentTreeNodeVisitor
	public abstract class AppointmentTreeNodeVisitor : IAppointmentTreeNodeVisitor {
		IAppointmentTreeNodeEnumeratorFactory enumeratorFactory;
		readonly TimeInterval seachInterval;
		protected AppointmentTreeNodeVisitor(TimeInterval seachInterval) {
			this.seachInterval = seachInterval ?? TimeInterval.Empty;
		}
		public IAppointmentTreeNodeEnumeratorFactory EnumeratorFactory {
			get {
				if (enumeratorFactory == null)
					enumeratorFactory = new AppointmentTreeNodeEnumerableFactory();
				return enumeratorFactory;
			}
			set {
				enumeratorFactory = value;
			}
		}
		public TimeInterval SearchInterval { get { return seachInterval; } }
		public bool VisitCompleted { get; set; }
		public abstract void VisitDayNode(AppointmentDayTreeNode node, bool checkInterval);
		public abstract void VisitCompositeNode(AppointmentTreeCompositeNode node, bool checkInterval);
	}
	#endregion
	public abstract class BaseQueryAppointmentTreeNodeVisitor : AppointmentTreeNodeVisitor {
		protected BaseQueryAppointmentTreeNodeVisitor(TimeInterval seachInterval)
			: base(seachInterval) {
		}
		public override void VisitDayNode(AppointmentDayTreeNode node, bool checkInterval) {
			SelectAppointmentsFromCollection(node, node.PartiallyIntersectedAppointments, SearchInterval, checkInterval);
			SelectAppointmentsFromCollection(node, node.IncludedAppointments, SearchInterval, checkInterval);
			SelectAppointmentsFromCollection(node, node.Appointments, SearchInterval, checkInterval);
		}
		public override void VisitCompositeNode(AppointmentTreeCompositeNode node, bool checkInterval) {
			QueryCompositeNodeAppointments(node);
		}
		protected virtual void QueryCompositeNodeAppointments(AppointmentTreeCompositeNode node) {
			SelectAppointmentsFromCollection(node, node.IncludedAppointments, SearchInterval, false);
		}
		protected virtual void SelectAppointmentsFromCollection(AppointmentTreeNode node, IList<Appointment> appointments, TimeInterval interval, bool checkInterval) {
			for (int i = 0; i < appointments.Count; i++) {
				Appointment apt = appointments[i];
				if (CanAddAppointment(node, apt, interval, checkInterval))
					AddAppointment(apt);
			}
		}
		protected abstract bool CanAddAppointment(AppointmentTreeNode node, Appointment apt, TimeInterval interval, bool checkInterval);
		protected abstract void AddAppointment(Appointment apt);
	}
	#region FilteredAppointmentTreeNodeVisitor
	public abstract class FilteredAppointmentTreeNodeVisitor : AppointmentTreeNodeVisitor {
		readonly AppointmentResourcesMatchFilter filter;
		protected FilteredAppointmentTreeNodeVisitor(TimeInterval interval, AppointmentResourcesMatchFilter filter)
			: base(interval) {
			Guard.ArgumentNotNull(filter, "filter");
			this.filter = filter;
		}
		protected internal AppointmentResourcesMatchFilter Filter { get { return filter; } }
	}
	#endregion
	#region QueryAppointmentsTreeNodeVisitor
	public class QueryAppointmentsTreeNodeVisitor : BaseQueryAppointmentTreeNodeVisitor {
		readonly AppointmentBaseCollection result;
		public QueryAppointmentsTreeNodeVisitor(TimeInterval interval)
			: base(interval) {
			this.result = new AppointmentBaseCollection(DXCollectionUniquenessProviderType.MaximizePerformance);
		}
		public AppointmentBaseCollection Result { get { return result; } }
		public override void VisitDayNode(AppointmentDayTreeNode node, bool checkInterval) {
			XtraSchedulerDebug.Assert(AppointmentTreeHelper.NodeIntersectsInterval(SearchInterval, node));
			base.VisitDayNode(node, checkInterval);
		}
		protected override void QueryCompositeNodeAppointments(AppointmentTreeCompositeNode node) {
			XtraSchedulerDebug.Assert(AppointmentTreeHelper.NodeIntersectsInterval(SearchInterval, node));
			base.QueryCompositeNodeAppointments(node);
		}
		protected override void AddAppointment(Appointment apt) {
			Result.Add(apt);
		}
		protected override bool CanAddAppointment(AppointmentTreeNode node, Appointment apt, TimeInterval interval, bool checkInterval) {
			if (checkInterval) {
				return AppointmentTreeHelper.IntervalIntersectsRange(interval, apt.Start, apt.End);
			}
			return true;
		}
	}
	#endregion
	public class QueryClientAppointmentByIntervalTreeNodeVisitor : QueryAppointmentsTreeNodeVisitor {
		public QueryClientAppointmentByIntervalTreeNodeVisitor(TimeInterval interval, AppointmentResourcesMatchFilter filter)
			: base(interval) {
			Guard.ArgumentNotNull(filter, "filter");
			Filter = filter;
		}
		protected internal AppointmentResourcesMatchFilter Filter { get; private set; }
		protected override bool CanAddAppointment(AppointmentTreeNode node, Appointment apt, TimeInterval interval, bool checkInterval) {
			if (checkInterval) {
				return MatchAppointmentResources(apt) && MatchAppointmentInterval(apt, node);
			}
			return MatchAppointmentResources(apt);
		}
		protected virtual bool MatchAppointmentResources(Appointment apt) {
			return Filter.MatchAppointmentResourceIds(apt);
		}
		protected virtual bool MatchAppointmentInterval(Appointment apt, AppointmentTreeNode node) {
			return AppointmentClientIntervalMatchHelper.MatchAppointmentClientInterval(SearchInterval, node.Start, node.End, apt, Filter.TimeZoneHelper);
		}
	}
	#region QueryAppointmentDatesTreeNodeVisitor
	public class QueryAppointmentDatesTreeNodeVisitor : FilteredAppointmentTreeNodeVisitor {
		readonly DateCollection result;
		public QueryAppointmentDatesTreeNodeVisitor(TimeInterval interval, AppointmentResourcesMatchFilter filter)
			: base(interval, filter) {
			this.result = new DateCollection(DXCollectionUniquenessProviderType.MaximizePerformance);
		}
		public DateCollection Result { get { return result; } }
		protected virtual void AddNodeDateRange(DateTime start, DateTime end) {
			List<DateTime> dates = DateTimeHelper.CreateRoundedDateList(start, end);
			Result.AddRange(dates);
		}
		public override void VisitDayNode(AppointmentDayTreeNode node, bool checkInterval) {
			XtraSchedulerDebug.Assert(AppointmentTreeHelper.NodeIntersectsInterval(SearchInterval, node));
			bool hasIncludedAppointments = HasFilteredAppointments(node.IncludedAppointments);
			bool hasAppointments = HasFilteredAppointments(node.Appointments);
			bool hasPartiallyIntersectedAppointments = HasFilteredAppointments(node.PartiallyIntersectedAppointments);
			if (hasIncludedAppointments || hasAppointments || hasPartiallyIntersectedAppointments) {
				AddNodeDateRange(node.Start, node.End);
				return;
			}
		}
		bool HasFilteredAppointments(IList<Appointment> appointments) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (MatchAppointmentResources(apt) ) {
					return true;
				}
			}
			return false;
		}
		protected bool FindAppointmentInCollection(IList<Appointment> appointments, AppointmentDayTreeNode node) {
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (MatchAppointmentResources(apt) && MatchAppointmentInterval(apt, node)) {
					AddNodeDateRange(node.Start, node.End);
					return true;
				}
			}
			return false;
		}
		public override void VisitCompositeNode(AppointmentTreeCompositeNode node, bool checkInterval) {
			VisitCompleted = false;
			XtraSchedulerDebug.Assert(AppointmentTreeHelper.NodeIntersectsInterval(SearchInterval, node));
			if (node.IncludedAppointments.Count > 0) {
				VisitCompleted = true;
				AddNodeDateRange(node.Start, node.End); 
				return;
			}
		}
		protected virtual bool MatchAppointmentResources(Appointment apt) {
			return Filter.MatchAppointmentResourceIds(apt);
		}
		protected virtual bool MatchAppointmentInterval(Appointment apt, AppointmentDayTreeNode node) {
			return AppointmentClientIntervalMatchHelper.MatchAppointmentClientInterval(SearchInterval, node.Start, node.End, apt, Filter.TimeZoneHelper);
		}
		protected internal virtual void PrepareResult() {
		}
	}
	#endregion
	public class QueryClientAppointmentDatesTreeNodeVisitor : QueryAppointmentDatesTreeNodeVisitor {
		readonly TimeIntervalCollectionEx appointmentsIntervals;
		public QueryClientAppointmentDatesTreeNodeVisitor(TimeInterval interval, AppointmentResourcesMatchFilter filter)
			: base(interval, filter) {
			appointmentsIntervals = new TimeIntervalCollectionEx();
		}
		protected TimeIntervalCollectionEx AppointmentsIntervals { get { return appointmentsIntervals; } }
		public override void VisitDayNode(AppointmentDayTreeNode node, bool checkInterval) {
			XtraSchedulerDebug.Assert(AppointmentTreeHelper.NodeIntersectsInterval(SearchInterval, node));
			AddTotalAppointmentsIntervalsAtInterval(node.IncludedAppointments, SearchInterval);
			AddTotalAppointmentsIntervalsAtInterval(node.Appointments, SearchInterval);
			AddTotalAppointmentsIntervalsAtInterval(node.PartiallyIntersectedAppointments, SearchInterval);
		}
		public override void VisitCompositeNode(AppointmentTreeCompositeNode node, bool checkInterval) {
			XtraSchedulerDebug.Assert(AppointmentTreeHelper.NodeIntersectsInterval(SearchInterval, node));
			AddTotalAppointmentsIntervalsAtInterval(node.IncludedAppointments, SearchInterval);
		}
		protected virtual void AddTotalAppointmentsIntervalsAtInterval(IList<Appointment> appointments, TimeInterval filterInterval) {
			TimeZoneHelper timeZoneHelper = Filter.TimeZoneHelper;
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (!MatchAppointmentResources(apt))
					continue;
				TimeInterval interval = ((IInternalAppointment)apt).GetInterval();
				if (interval.Contains(filterInterval))
					interval = filterInterval;
				else if (interval.Start < filterInterval.Start)
					interval = new TimeInterval(filterInterval.Start, interval.End);
				else if (interval.End > filterInterval.End)
					interval = new TimeInterval(interval.Start, filterInterval.End);
				TimeInterval clientInterval = timeZoneHelper.ToClientTime(interval);
				AppointmentsIntervals.Add(new TimeInterval(clientInterval.Start.Date, clientInterval.CalculateAllDayDuration()));
			}
		}
		protected internal override void PrepareResult() {
			DayIntervalCollection days = new DayIntervalCollection();
			days.AddRange(AppointmentsIntervals);
			int count = days.Count;
			for (int i = 0; i < count; i++)
				Result.Add(days[i].Start);
		}
	}
	#region DeleteAppointmentTreeNodeVisitor
	public class DeleteAppointmentTreeNodeVisitor : AppointmentTreeNodeVisitor {
		readonly Appointment appointment;
		public DeleteAppointmentTreeNodeVisitor(Appointment appointment, TimeInterval appointmentInterval)
			: base(appointmentInterval) {
			Guard.ArgumentNotNull(appointment, "appointment");
			this.appointment = appointment;
		}
		public Appointment Appointment { get { return appointment; } }
		public override void VisitDayNode(AppointmentDayTreeNode node, bool checkInterval) {
			RemoveAppointmentFromList(node.Appointments, Appointment);
			RemoveAppointmentFromList(node.IncludedAppointments, Appointment);
			RemoveAppointmentFromList(node.PartiallyIntersectedAppointments, Appointment);
		}
		public override void VisitCompositeNode(AppointmentTreeCompositeNode node, bool checkInterval) {
			RemoveAppointmentFromList(node.IncludedAppointments, Appointment);
		}
		protected void RemoveAppointmentFromList(IList<Appointment> list, Appointment apt) {
			if (list.Contains(apt)) list.Remove(apt);
		}
	}
	#endregion
	#region PrevNextAppointmentTreeNodeVisitor
	public class PrevNextAppointmentTreeNodeVisitor : BaseQueryAppointmentTreeNodeVisitor {
		internal Dictionary<object, Appointment> result;
		readonly AppointmentResourcesMatchFilter filter;
		readonly bool prevApt;
		readonly Dictionary<object, List<Appointment>> matchingApts;
		public PrevNextAppointmentTreeNodeVisitor(TimeInterval interval, AppointmentResourcesMatchFilter filter)
			: this(interval, filter, false) {
		}
		public PrevNextAppointmentTreeNodeVisitor(TimeInterval interval, AppointmentResourcesMatchFilter filter, bool prevApt)
			: base(interval) {
			Guard.ArgumentNotNull(filter, "filter");
			this.filter = filter;
			this.prevApt = prevApt;
			InitResult(filter.Resources);
			matchingApts = new Dictionary<object, List<Appointment>>();
		}
		protected internal AppointmentResourcesMatchFilter Filter { get { return filter; } }
		public Dictionary<object, Appointment> Result { get { return result; } }
		private void InitResult(ResourceBaseCollection resources) {
			this.result = AppointmentTreeHelper.CreateResourceAppointmentCache(resources);
		}
		void AddAppointmentIfNull(Appointment apt, object id) {
			if (id == null)
				return;
			List<Appointment> apts;
			if (matchingApts.TryGetValue(id, out apts))
				apts.Add(apt);
			else {
				apts = new List<Appointment>() { apt };
				matchingApts[id] = apts;
			}
		}
		void FindResultingAppointments() {
			foreach (KeyValuePair<object, List<Appointment>> entry in matchingApts) {
				List<Appointment> apts = entry.Value;
				result[entry.Key] = GetFirstOrLastAppointment(apts);
			}
		}
		Appointment GetFirstOrLastAppointment(List<Appointment> apts) {
			Appointment result = apts[0];
			for (int i = 1; i < apts.Count; i++) {
				if (prevApt) {
					if (apts[i].Start > result.Start || (apts[i].Start == result.Start && apts[i].Duration < result.Duration))
						result = apts[i];
				} else {
					if (apts[i].Start < result.Start || (apts[i].Start == result.Start && apts[i].Duration > result.Duration))
						result = apts[i];
				}
			}
			return result;
		}
		protected override void AddAppointment(Appointment apt) {
			XtraSchedulerDebug.Assert(result != null);
			XtraSchedulerDebug.Assert(Filter.ResourceAppointmentPredicateDictionary != null);
			ResourceBaseCollection resources = filter.Resources;
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				object resourceId = resource.Id;
				SchedulerPredicate<DevExpress.XtraScheduler.Appointment> isFitPredicate = Filter.ResourceAppointmentPredicateDictionary[resourceId];
				if (isFitPredicate.Calculate(apt)) 
					AddAppointmentIfNull(apt, resourceId);
			}
		}
		protected override bool CanAddAppointment(AppointmentTreeNode node, Appointment apt, TimeInterval interval, bool checkInterval) {
			TimeInterval aptInterval = ((IInternalAppointment)apt).GetInterval();
			if (interval.Start >= aptInterval.End || interval.End <= aptInterval.Start)
				return false;
			return Filter.MatchAppointmentResourceIds(apt);
		}
		protected override void SelectAppointmentsFromCollection(AppointmentTreeNode node, IList<Appointment> appointments, TimeInterval interval, bool checkInterval) {
			for (int i = 0; i < appointments.Count; i++) {
				Appointment apt = appointments[i];
				if (CanAddAppointment(node, apt, interval, checkInterval))
					AddAppointment(apt);
			}
			FindResultingAppointments();
		}
	}
	#endregion
	public class AppointmentsPerDayTreeNodeVisitor : AppointmentTreeNodeVisitor {
		public AppointmentsPerDayTreeNodeVisitor(TimeInterval interval)
			: base(interval) {
			Result = new Dictionary<DateTime, int>();
		}
		protected Dictionary<DateTime, int> Result { get; private set; }
		public override void VisitCompositeNode(AppointmentTreeCompositeNode node, bool checkInterval) {
			DateTime start = node.Start;
			DateTime end = node.End;
			int count = (end - start).Days;
			for (int i = 0; i < count; i++) {
				RegisterAppointments(start.AddDays(i), node.IncludedAppointments.Count);
			}
		}
		public override void VisitDayNode(AppointmentDayTreeNode node, bool checkInterval) {
			RegisterAppointments(node.Start, node.Appointments.Count);
			RegisterAppointments(node.Start, node.IncludedAppointments.Count);
			RegisterAppointments(node.Start, node.PartiallyIntersectedAppointments.Count);
		}
		protected virtual void RegisterAppointments(DateTime key, int count) {
			int registered = 0;
			Result.TryGetValue(key, out registered);
			Result[key] = registered + count;
		}
	}
}

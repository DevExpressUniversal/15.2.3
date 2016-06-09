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
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentTreeNode (abstract class)
	public abstract class AppointmentTreeNode {
		protected static readonly long dayTicks = TimeSpan.FromDays(1).Ticks;
		readonly long DateTimeMaxTicks = DateTime.MaxValue.Ticks;
		readonly AppointmentTreeNode parent;
		int startDayIndex;
		protected AppointmentTreeNode(AppointmentTreeNode parent) {
			this.parent = parent;
		}
		public AppointmentTreeNode Parent { get { return parent; } }
		public int StartDayIndex { get { return startDayIndex; } set { startDayIndex = value; } }
		public abstract int DayCount { get; }
		public virtual DateTime Start { get { return new DateTime(dayTicks * StartDayIndex); } }
		public virtual DateTime End { get { return new DateTime(Math.Min(dayTicks * (StartDayIndex + DayCount), DateTimeMaxTicks)); } }
		protected internal abstract AppointmentTreeNode CreateChildNode();
		public abstract void Insert(Appointment apt);
		protected abstract AppointmentTreeNode GetChild(int index);
		protected abstract int CalculateChildIndex(DateTime dateTime);
		protected internal virtual int CalculateSearchRangeStartIndex(DateTime date) {
			return CalculateChildIndex(date);
		}
		protected internal virtual int CalculateSearchRangeEndIndex(DateTime date) {
			return CalculateChildIndex(date);
		}
		protected void InsertChildrenRange(Appointment apt, int startChildIndex, int endChildIndex) {
			for (int i = startChildIndex; i <= endChildIndex; i++)
				GetChild(i).Insert(apt);
		}
		protected internal abstract void Accept(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType intersection);
	}
	#endregion
	#region AppointmentTreeCompositeNode (abstract class)
	public abstract class AppointmentTreeCompositeNode : AppointmentTreeNode {
		List<Appointment> includedAppointments;
		AppointmentTreeNode[] children;
		protected AppointmentTreeCompositeNode(AppointmentTreeNode parent)
			: base(parent) {
		}
		#region Properties
		public IList<Appointment> IncludedAppointments {
			get {
				if (includedAppointments == null)
					includedAppointments = new List<Appointment>();
				return includedAppointments;
			}
		}
		public AppointmentTreeNode[] Children {
			get {
				if (children == null)
					children = new AppointmentTreeNode[32];
				return children;
			}
		}
		public abstract int ChildDayCount { get; }
		protected int Count { get { return children != null ? children.Length : 0; } }
		public bool HasChildren { get { return GetHasChildren(); } }
		#endregion
		protected bool GetHasChildren() {
			if (Count == 0)
				return false;
			for (int i = 0; i < Count; i++) {
				if (Children[i] != null)
					return true;
			}
			return false;
		}
		public override void Insert(Appointment apt) {
			if (apt.Start <= Start && apt.End >= End) {
				IncludedAppointments.Add(apt);
				return;
			}
			int startChildIndex = Math.Max(0, CalculateChildIndex(apt.Start));
			int maxChildIndex = DayCount / ChildDayCount;
			int endChildIndex = Math.Min( maxChildIndex - 1, CalculateChildIndex(apt.End)); 
			InsertChildrenRange(apt, startChildIndex, endChildIndex);
		}
		protected internal override int CalculateSearchRangeStartIndex(DateTime date) {
			return Math.Max(0, CalculateChildIndex(date));
		}
		protected internal override int CalculateSearchRangeEndIndex(DateTime date) {
			return Math.Min(31, CalculateChildIndex(date));
		}
		protected override AppointmentTreeNode GetChild(int index) {
			AppointmentTreeNode child = Children[index];
			if (child == null) {
				child = CreateChildNode();
				child.StartDayIndex = this.StartDayIndex + index * ChildDayCount;
				Children[index] = child;
			}
			return child;
		}
		protected override int CalculateChildIndex(DateTime dateTime) {
			return (int)(dateTime - Start).TotalDays / ChildDayCount;
		}
		protected internal override void Accept(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType intersection) {
			visitor.VisitCompositeNode(this, AppointmentTreeHelper.ShouldCheckNodeInterval(intersection));
			if (CanVisitChildren(visitor))
				VisitChildren(visitor, intersection);
		}
		protected virtual bool CanVisitChildren(IAppointmentTreeNodeVisitor visitor) {
			return !visitor.VisitCompleted;
		}
		protected virtual void VisitChildren(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType parentIntersection) {
			AppointmentTreeNodeEnumeratorBase en = visitor.EnumeratorFactory.CreateCompositeNodeEnumerator(this);
			en.Enumerate(visitor, parentIntersection);
		}
	}
	#endregion
	#region AppointmentTreeNodeLeaf (abstract class)
	public abstract class AppointmentTreeNodeLeaf : AppointmentTreeNode {
		protected AppointmentTreeNodeLeaf(AppointmentTreeNode parent)
			: base(parent) {
		}		
	}
	#endregion
	#region AppointmentDayTreeNode
	public class AppointmentDayTreeNode : AppointmentTreeNodeLeaf {
		List<Appointment> includedAppointments;
		List<Appointment> partiallyIntersectedAppointments;
		List<Appointment> appointments;
		public AppointmentDayTreeNode(AppointmentTreeNode parent)
			: base(parent) {
		}
		#region Properties
		public override int DayCount { get { return 1; } }
		public IList<Appointment> IncludedAppointments {
			get {
				if (includedAppointments == null)
					includedAppointments = new List<Appointment>();
				return includedAppointments;
			}
		}
		public IList<Appointment> PartiallyIntersectedAppointments {
			get {
				if (partiallyIntersectedAppointments == null)
					partiallyIntersectedAppointments = new List<Appointment>();
				return partiallyIntersectedAppointments;
			}
		}
		public IList<Appointment> Appointments {
			get {
				if (appointments == null)
					appointments = new List<Appointment>();
				return appointments;
			}
		}
		#endregion
		protected override AppointmentTreeNode GetChild(int index) {
			return null;
		}
		protected override int CalculateChildIndex(DateTime dateTime) {
			return -1;
		}
		protected internal override int CalculateSearchRangeEndIndex(DateTime date) {
			return -1;
		}
		protected internal override int CalculateSearchRangeStartIndex(DateTime date) {
			return -1;
		}		
		public override void Insert(Appointment apt) {
			TimeSpan intersection = Algorithms.Min(End, apt.End) - Algorithms.Max(Start, apt.Start);
			if (intersection < TimeSpan.Zero)
				return;
			if (intersection < apt.Duration) {
				if (apt.Start <= Start && apt.End >= End)
					IncludedAppointments.Add(apt);
				else if (intersection > TimeSpan.Zero)
					PartiallyIntersectedAppointments.Add(apt);
			} else {
				if (apt.Duration == TimeSpan.Zero) {
					if (apt.Start >= Start && apt.Start < End)
						Appointments.Add(apt);
				} else if (apt.Duration == intersection)
					Appointments.Add(apt);
		}
		}
		protected internal override void Accept(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType intersection) {
			visitor.VisitDayNode(this, AppointmentTreeHelper.ShouldCheckNodeInterval(intersection));
		}
		protected internal override AppointmentTreeNode CreateChildNode() {
			return null;
		}
	}
	#endregion
	#region Appointment32DayTreeNode
	public class Appointment32DayTreeNode : AppointmentTreeCompositeNode {
		public Appointment32DayTreeNode(AppointmentTreeNode parent)
			: base(parent) {
		}
		public override int DayCount { get { return 32; } }
		public override int ChildDayCount { get { return 1; } }
		protected internal override AppointmentTreeNode CreateChildNode() {
			return new AppointmentDayTreeNode(this);
		}
	}
	#endregion
	#region Appointment1024DayTreeNode
	public class Appointment1024DayTreeNode : AppointmentTreeCompositeNode {
		public Appointment1024DayTreeNode(AppointmentTreeNode parent)
			: base(parent) {
		}
		public override int DayCount { get { return 1024; } }
		public override int ChildDayCount { get { return 32; } }
		protected internal override AppointmentTreeNode CreateChildNode() {
			return new Appointment32DayTreeNode(this);
		}
	}
	#endregion
	#region AppointmentTree
	public class AppointmentTree : AppointmentTreeNode {
		static readonly long childDurationTicks = TimeSpan.FromDays(1024).Ticks;
		Dictionary<int, AppointmentTreeNode> childrenTable = new Dictionary<int, AppointmentTreeNode>();
		public AppointmentTree()
			: base(null) {
			Initialize();
		}
		protected AppointmentTree(AppointmentTreeNode parent)
			: base(parent) {
			Initialize();
		}
		#region Properties
		public override DateTime Start { get { return DateTime.MinValue; } }
		public override DateTime End { get { return DateTime.MaxValue; } }
		public override int DayCount { get { return (int)(End - Start).TotalDays; } }
		public virtual int ChildDayCount { get { return 1024; } }
		public bool HasChildren { get { return GetHasChildren(); } }
		protected internal Dictionary<int, AppointmentTreeNode> Children { get { return childrenTable; } }
		#endregion
		public List<AppointmentTreeCompositeNode> GetChildrenNodes() {
			List<AppointmentTreeCompositeNode> result = new List<AppointmentTreeCompositeNode>();
			int[] keys = GetSortedChildrenKeys();
			int count = keys.Length;
			for (int i = 0; i < count; i++) {
				AppointmentTreeCompositeNode item = Children[keys[i]] as AppointmentTreeCompositeNode;
				XtraSchedulerDebug.Assert(item != null);
				result.Add(item);
			}
			return result;
		}
		protected internal int[] GetSortedChildrenKeys() { 
			List<int> sortedList = new List<int>(Children.Keys);
			sortedList.Sort();
			return sortedList.ToArray();
		}
		protected bool GetHasChildren() {
			return Children.Count > 0;
		}
		protected virtual void Initialize() {
		}
		protected internal override AppointmentTreeNode CreateChildNode() {
			return new Appointment1024DayTreeNode(this);
		}
		public void ClearAll() {
			Children.Clear();
		}
		public override void Insert(Appointment apt) {
			XtraSchedulerDebug.Assert(apt.Type == AppointmentType.Normal);
			int startChildIndex = CalculateChildIndex(apt.Start);
			int endChildIndex = CalculateChildIndex(apt.End);
			InsertChildrenRange(apt, startChildIndex, endChildIndex);
		}
		protected override int CalculateChildIndex(DateTime dateTime) {
			return (int)(dateTime.Ticks / childDurationTicks);
		}
		protected override AppointmentTreeNode GetChild(int index) {
			AppointmentTreeNode child;
			if (childrenTable.TryGetValue(index, out child))
				return child;
			child = CreateChildNode();
			child.StartDayIndex = index * ChildDayCount;
			childrenTable[index] = child;
			return child;
		}
		protected internal override void Accept(IAppointmentTreeNodeVisitor visitor, AppointmentTreeNodeIntersectionType intersection) {
			Exceptions.ThrowInternalException();
		}
	}
	#endregion
}

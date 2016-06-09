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
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using System.Collections;
using DevExpress.XtraScheduler.Drawing.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using System.Threading;
namespace DevExpress.XtraScheduler.Drawing {
	#region DependencyViewInfosCalculator
	public class DependencyViewInfosCalculator {
		#region Fields
		GanttDependenciesPainter painter;
		Rectangle visibleBounds;
		DependencyGroupingHelper groupingHelper;
		DependencyIntermediateViewInfoCollection totalGroupsDependencyViewInfos;
		DependencyIntermediateViewInfoCollection currentGroupDependencyViewInfos;
		GanttAppointmentViewInfoCollection visibleAppointments;
		MoreButtonCollection moreButtons;
		#endregion
		public DependencyViewInfosCalculator(GanttDependenciesPainter painter, Rectangle visibleBounds, MoreButtonCollection moreButtons, AppointmentDependencyCollection dependencies) {
			this.totalGroupsDependencyViewInfos = new DependencyIntermediateViewInfoCollection();
			this.currentGroupDependencyViewInfos = new DependencyIntermediateViewInfoCollection();
			this.visibleAppointments = new GanttAppointmentViewInfoCollection();
			Guard.ArgumentNotNull(painter, "painter");
			Guard.ArgumentNotNull(moreButtons, "moreButtons");
			this.groupingHelper = new DependencyGroupingHelper(painter.GetArrowWidth(), dependencies);
			this.painter = painter;
			this.visibleBounds = visibleBounds;
			this.moreButtons = moreButtons;
		}
		#region Properties
		internal Rectangle VisibleBounds { get { return visibleBounds; } }
		internal DependencyIntermediateViewInfoCollection TotalGroupsDependencyViewInfos { get { return totalGroupsDependencyViewInfos; } }
		internal DependencyIntermediateViewInfoCollection CurrentGroupDependencyViewInfos { get { return currentGroupDependencyViewInfos; } set { currentGroupDependencyViewInfos = value; } }
		internal DependencyGroupingHelper GroupingHelper { get { return groupingHelper; } }
		internal GanttAppointmentViewInfoCollection VisibleAppointments { get { return visibleAppointments; } }
		internal GanttDependenciesPainter Painter { get { return painter; } }
		internal MoreButtonCollection MoreButtons { get { return moreButtons; } }
		#endregion
		public DependencyViewInfoCollection GetDependencyViewInfos(CancellationToken token, DependencyIntermediateViewInfoCollection intermediateViewInfos) {
			DependencyViewInfoCollection result = new DependencyViewInfoCollection();
			int intermediateCount = intermediateViewInfos.Count;
			for (int i = 0; i < intermediateCount; i++) {
				if (token.IsCancellationRequested) {
					result.Clear();
					return result;
				}
				result.AddRange(intermediateViewInfos[i].DependencyViewInfos);
			}
			return result;
		}
		public DependencyViewInfoCollection CalculateDependencyViewInfos(CancellationToken token, GanttAppointmentViewInfoCollection aptViewInfos) {
			GroupingHelper.Calculate(aptViewInfos, VisibleBounds);
			CollectVisibleAppointments(aptViewInfos);
			int count = GroupingHelper.Groups.Count;
			for (int i = 0; i < count; i++) {
				if (token.IsCancellationRequested) {
					TotalGroupsDependencyViewInfos.Clear();
					return GetDependencyViewInfos(token, TotalGroupsDependencyViewInfos);
				}
				TotalGroupsDependencyViewInfos.AddRange(CalculateSingleGroupDependencies(GroupingHelper.Groups[i]));
			}
			return GetDependencyViewInfos(token, TotalGroupsDependencyViewInfos);
		}
		protected internal virtual DependencyIntermediateViewInfoCollection CalculateSingleGroupDependencies(DependencyGroup group) {
#if DEBUG || DEBUGTEST
			XtraSchedulerDebug.Assert(group.Dependencies.Count > 0);
			XtraSchedulerDebug.Assert(group.DependentViewInfos.Count > 0);
			XtraSchedulerDebug.Assert(group.ParentViewInfos.Count > 0);
#endif
			CurrentGroupDependencyViewInfos = new DependencyIntermediateViewInfoCollection();
			CalculateViewInfosTableType(group);
			GanttAppointmentViewInfoWrapperCollection processedApts = CalculateFirstDependency(group);
			CalculateDependenciesForParentAppointments(group, processedApts);
			CalculateDependenciesForDependentAppointments(group, processedApts);
			AdjacentDependencyViewInfoCalculator relatedViewInfosCalculator = new AdjacentDependencyViewInfoCalculator(CurrentGroupDependencyViewInfos);
			relatedViewInfosCalculator.Calculate();
			DependencyViewInfosDependenciesCalculator dependenciesCalculator = new DependencyViewInfosDependenciesCalculator(CurrentGroupDependencyViewInfos, group);
			dependenciesCalculator.Calculate();
			return CurrentGroupDependencyViewInfos;
		}
		protected internal virtual GanttAppointmentViewInfoWrapperCollection CalculateFirstDependency(DependencyGroup group) {
			GanttAppointmentViewInfoWrapperCollection processed = new GanttAppointmentViewInfoWrapperCollection();
			GanttAppointmentViewInfoWrapperCollection apts = GetFirstDependencyAppointments(group);
			if (apts.Count == 0)
				return processed;
			XtraSchedulerDebug.Assert(apts.Count == 2);
			GanttAppointmentViewInfoWrapper startViewInfo = apts[0];
			GanttAppointmentViewInfoWrapper endViewInfo = apts[1];
			ConnectAppointments(startViewInfo, endViewInfo);
			processed.Add(startViewInfo);
			processed.Add(endViewInfo);
			return processed;
		}
		protected internal virtual GanttAppointmentViewInfoWrapperCollection GetFirstDependencyAppointments(DependencyGroup group) {
			GanttAppointmentViewInfoWrapperCollection result = GetFirstDependencyAppointmentsCore(group, false);
			if (result.Count == 0)
				result = GetFirstDependencyAppointmentsCore(group, true);
			return result;
		}
		protected internal virtual GanttAppointmentViewInfoWrapperCollection GetFirstDependencyAppointmentsCore(DependencyGroup group, bool allowMoreButtonAppointments) {
			GanttAppointmentViewInfoWrapperCollection result = new GanttAppointmentViewInfoWrapperCollection();
			int count = group.ParentViewInfos.Count;
			for (int i = 0; i < count; i++) {
				GanttAppointmentViewInfoWrapper parent = group.ParentViewInfos[i];
				if (parent.Visibility.InvisibleMoreButton && !allowMoreButtonAppointments)
					continue;
				GanttAppointmentViewInfoWrapper dependent = GetFirstDependentAppointment(parent, group.DependentViewInfos);
				if (dependent == null)
					continue;
				result.Add(parent);
				result.Add(dependent);
				break;
			}
			return result;
		}
		protected internal virtual GanttAppointmentViewInfoWrapper GetFirstDependentAppointment(GanttAppointmentViewInfoWrapper parent, GanttAppointmentViewInfoWrapperCollection dependentApts) {
			Point dependencyStart = parent.GetNextDependencyConnectionPoint();
			dependentApts.Sort(new GanttAppointmentViewInfoDependentPositionComparer(dependencyStart));
			int count = dependentApts.Count;
			for (int i = 0; i < count; i++) {
				GanttAppointmentViewInfoWrapper dependentApt = dependentApts[i];
				if (IsDependencyVisible(parent.Visibility, dependentApt.Visibility, false))
					return dependentApt;
			}
			return null;
		}
		protected internal virtual bool IsDependencyVisible(AppointmentViewInfoVisibility parent, AppointmentViewInfoVisibility dependent, bool ignoreMoreButtons) {
			if (parent.InvisibleTopResource && dependent.InvisibleTopResource)
				return false;
			if (parent.InvisibleBottomResource && dependent.InvisibleBottomResource)
				return false;
			if (parent.InvisibleMoreButton && dependent.InvisibleMoreButton && !ignoreMoreButtons)
				return false;
			return true;
		}
		protected internal virtual bool IsDependencyVisible(GanttAppointmentViewInfoWrapper aptViewInfo, DependencyGroup group) {
			if (!aptViewInfo.Visibility.InvisibleResource)
				return true;
			return aptViewInfo.IsParent ? IsDependencyVisible(aptViewInfo, group.DependentViewInfos) : IsDependencyVisible(aptViewInfo, group.ParentViewInfos);
		}
		protected internal virtual bool IsDependencyVisible(GanttAppointmentViewInfoWrapper aptViewInfo, GanttAppointmentViewInfoWrapperCollection groupViewInfos) {
			int count = groupViewInfos.Count;
			for (int i = 0; i < count; i++) {
				GanttAppointmentViewInfoWrapper currentViewInfo = groupViewInfos[i];
				if (currentViewInfo == aptViewInfo)
					continue;
				if (IsDependencyVisible(aptViewInfo.Visibility, currentViewInfo.Visibility, true))
					return true;
			}
			return false;
		}
		protected internal virtual void ConnectAppointments(GanttAppointmentViewInfoWrapper parentApt, GanttAppointmentViewInfoWrapper dependentApt) {
			DependencyConnectorCalculator connectorCalculator = CreateConnectorCalculator();
			AppointmentToAppointmentPathBuilder builder = new AppointmentToAppointmentPathBuilder(parentApt, dependentApt, connectorCalculator);
			DependencyIntermediateViewInfoCollection result = builder.BuildPath();
			AddDependencyViewInfos(result);
		}
		protected internal virtual DependencyConnectorCalculator CreateConnectorCalculator() {
			IntersectionObjectsInfo intersectionObjects = new IntersectionObjectsInfo(VisibleAppointments, TotalGroupsDependencyViewInfos, painter.GetDistanceFromAppointment());
			return new DependencyConnectorCalculator(intersectionObjects, painter.GetConnectorIndent(), VisibleBounds, MoreButtons);
		}
		protected internal virtual void CalculateDependenciesForParentAppointments(DependencyGroup group, GanttAppointmentViewInfoWrapperCollection processedApts) {
			int count = group.ParentViewInfos.Count;
			for (int i = 0; i < count; i++) {
				GanttAppointmentViewInfoWrapper currentViewInfo = group.ParentViewInfos[i];
				if (processedApts.Contains(currentViewInfo))
					continue;
				currentViewInfo.TableType = GroupingHelper.GetViewInfoTableTypeByGroupIndex(group.Index, currentViewInfo, true);
				ConnectWithCurrentGroupDependencies(currentViewInfo, group);
			}
		}
		protected internal virtual void CalculateDependenciesForDependentAppointments(DependencyGroup group, GanttAppointmentViewInfoWrapperCollection processedApts) {
			int count = group.DependentViewInfos.Count;
			for (int i = 0; i < count; i++) {
				GanttAppointmentViewInfoWrapper currentViewInfo = group.DependentViewInfos[i];
				if (processedApts.Contains(currentViewInfo))
					continue;
				currentViewInfo.TableType = GroupingHelper.GetViewInfoTableTypeByGroupIndex(group.Index, currentViewInfo, false);
				ConnectWithCurrentGroupDependencies(currentViewInfo, group);
			}
		}
		protected internal virtual void ConnectWithCurrentGroupDependencies(GanttAppointmentViewInfoWrapper aptViewInfo, DependencyGroup group) {
			if (!IsDependencyVisible(aptViewInfo, group))
				return;
			NearestDependencySearchResult searchResult = GetNearestDependencyViewInfo(aptViewInfo);
			ConnectToDependencyViewInfo(aptViewInfo, searchResult);
		}
		protected internal void ConnectToDependencyViewInfo(GanttAppointmentViewInfoWrapper aptViewInfo, NearestDependencySearchResult nearestDependency) {
			if (nearestDependency.ViewInfo == null)
				return;
			AppointmentToDependencyPathBuilderBase builder = CreateAppointmentToDependencyPathBuilder(aptViewInfo, nearestDependency);
			DependencyIntermediateViewInfoCollection result = builder.BuildPath();
			AddDependencyViewInfos(result);
		}
		protected internal AppointmentToDependencyPathBuilderBase CreateAppointmentToDependencyPathBuilder(GanttAppointmentViewInfoWrapper aptViewInfo, NearestDependencySearchResult nearestDependency) {
			DependencyConnectorCalculator connectorCalculator = CreateConnectorCalculator();
			if (nearestDependency.ViewInfo.Direction == DependencyDirection.Vertical)
				return new AppointmentToVerticalDependencyPathBuilder(aptViewInfo, nearestDependency, connectorCalculator);
			return new AppointmentToHorizontalDependencyPathBuilder(aptViewInfo, nearestDependency, connectorCalculator);
		}
		protected internal virtual ConnectorItemBase CreateConnectorItem(GanttAppointmentViewInfoWrapper viewInfo) {
			ConnectorLocation location = CalculateConnectorLocation(viewInfo);
			Point aptPoint = viewInfo.GetNextDependencyConnectionPoint();
			Point indentPoint = CreatePointWithIndent(aptPoint, location);
			if (aptPoint.Y == indentPoint.Y)
				return new HorizontalConnectorItem(aptPoint, indentPoint, viewInfo.ConnectorPosition, location);
			return new VerticalConnectorItem(aptPoint, indentPoint, viewInfo.ConnectorPosition, location);
		}
		protected internal virtual ConnectorLocation CalculateConnectorLocation(GanttAppointmentViewInfoWrapper viewInfo) {
			ConnectorLocation location = ConnectorLocation.None;
			bool isFinish = viewInfo.ConnectorPosition == ConnectorPosition.Finish;
			if (CanCreateHorizontalConnector(viewInfo))
				return isFinish ? ConnectorLocation.Right : ConnectorLocation.Left;
			location = ConnectorLocation.BottomRight;
			if (viewInfo.IsParent)
				location = ConnectorLocation.BottomLeft;
			return location;
		}
		protected internal virtual bool CanCreateHorizontalConnector(GanttAppointmentViewInfoWrapper from) {
			Point aptPoint = from.GetNextDependencyConnectionPoint();
			bool isFinish = from.ConnectorPosition == ConnectorPosition.Finish;
			ConnectorLocation location = isFinish ? ConnectorLocation.Right : ConnectorLocation.Left;
			Point indentPoint = CreatePointWithIndent(aptPoint, location);
			if (IsPointInvisible(indentPoint))
				return false;
			IntersectionObjectsInfo intersectionObjects = new IntersectionObjectsInfo(VisibleAppointments, TotalGroupsDependencyViewInfos, painter.GetDistanceFromAppointment());
			Rectangle intersection = intersectionObjects.CalculateConnectorIntersections(aptPoint, indentPoint, from.ViewInfo);
			return intersection == Rectangle.Empty;
		}
		protected internal virtual bool IsPointInvisible(Point indentPoint) {
			return (indentPoint.X <= VisibleBounds.Left) || (indentPoint.X >= VisibleBounds.Right);
		}
		protected internal Point CreatePointWithIndent(Point source, ConnectorLocation location) {
			int indent = Painter.GetConnectorIndent();
			switch (location) {
				case ConnectorLocation.Left:
					return new Point(source.X - indent, source.Y);
				case ConnectorLocation.Right:
					return new Point(source.X + indent, source.Y);
				case ConnectorLocation.BottomLeft:
					return new Point(source.X, source.Y + indent);
				case ConnectorLocation.BottomRight:
					return new Point(source.X, source.Y + indent);
				default:
					return Point.Empty;
			}
		}
		protected internal int CalcDistance(Point pt1, Point pt2) {
			return Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y);
		}
		void CollectVisibleAppointments(GanttAppointmentViewInfoCollection aptViewInfos) {
			VisibleAppointments.Clear();
			int count = aptViewInfos.Count;
			for (int i = 0; i < count; i++)
				if (aptViewInfos[i].Visibility.Visible)
					VisibleAppointments.Add(aptViewInfos[i]);
		}
		void CalculateViewInfosTableType(DependencyGroup group) {
			CalculateViewInfosTableType(group.ParentViewInfos, group.Index, true);
			CalculateViewInfosTableType(group.DependentViewInfos, group.Index, false);
		}
		void CalculateViewInfosTableType(GanttAppointmentViewInfoWrapperCollection viewInfos, int groupIndex, bool isParent) {
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				GanttAppointmentViewInfoWrapper viewInfo = viewInfos[i];
				viewInfo.TableType = GroupingHelper.GetViewInfoTableTypeByGroupIndex(groupIndex, viewInfo, isParent);
			}
		}
		void AddDependencyViewInfos(DependencyIntermediateViewInfoCollection viewInfos) {
			CurrentGroupDependencyViewInfos.AddRange(viewInfos);
			TotalGroupsDependencyViewInfos.AddRange(viewInfos);
		}
		NearestDependencySearchResult GetNearestDependencyViewInfo(GanttAppointmentViewInfoWrapper aptViewInfo) {
			int count = CurrentGroupDependencyViewInfos.Count;
			ConnectorItemBase item = CreateConnectorItem(aptViewInfo);
			int minDistance = Int32.MaxValue;
			NearestDependencySearchResult result = new NearestDependencySearchResult();
			for (int i = 0; i < count; i++) {
				DependencyIntermediateViewInfo current = CurrentGroupDependencyViewInfos[i];
				DependencyIntermediateViewInfo prev = (i != 0) ? CurrentGroupDependencyViewInfos[i - 1] : null;
				DependencyIntermediateViewInfo next = (i != count - 1) ? CurrentGroupDependencyViewInfos[i + 1] : null;
				DependencyConnectionPointInfo pt = current.GetPointToConnect(item, prev, next);
				int currentDistance = CalcDistance(item.PointPath, pt.Point);
				if (currentDistance < minDistance) {
					minDistance = currentDistance;
					result = new NearestDependencySearchResult(current, pt);
				}
			}
			return result;
		}
	}
	#endregion
	#region NearestDependencySearchResult
	public class NearestDependencySearchResult {
		#region Fields
		DependencyIntermediateViewInfo viewInfo;
		DependencyConnectionPointInfo pointInfo;
		#endregion
		public NearestDependencySearchResult() {
		}
		public NearestDependencySearchResult(DependencyIntermediateViewInfo viewInfo, DependencyConnectionPointInfo pointInfo) {
			this.viewInfo = viewInfo;
			this.pointInfo = pointInfo;
		}
		#region Properties
		public DependencyIntermediateViewInfo ViewInfo { get { return viewInfo; } }
		public DependencyConnectionPointInfo PointInfo { get { return pointInfo; } }
		public Point PointToConnect { get { return PointInfo.Point; } }
		#endregion
	}
	#endregion
	#region DependencyViewInfoRelatedViewInfosCalculator
	public class AdjacentDependencyViewInfoCalculator {
		DependencyViewInfoCollection dependencyViewInfos;
		public AdjacentDependencyViewInfoCalculator(DependencyIntermediateViewInfoCollection intermediateViewInfos) {
			this.dependencyViewInfos = GetDependencyViewInfos(intermediateViewInfos);
		}
		internal DependencyViewInfoCollection DependencyViewInfos { get { return dependencyViewInfos; } }
		public void Calculate() {
			int count = DependencyViewInfos.Count;
			for (int i = 0; i < count; i++)
				CalculateCore(DependencyViewInfos[i]);
		}
		protected internal virtual DependencyViewInfoCollection GetDependencyViewInfos(DependencyIntermediateViewInfoCollection intermediateViewInfos) {
			DependencyViewInfoCollection result = new DependencyViewInfoCollection();
			int count = intermediateViewInfos.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(intermediateViewInfos[i].DependencyViewInfos);
			return result;
		}
		void CalculateCore(DependencyViewInfo viewInfo) {
			int count = DependencyViewInfos.Count;
			for (int i = 0; i < count; i++) {
				DependencyViewInfo current = DependencyViewInfos[i];
				if (current == viewInfo)
					continue;
				if (AreAdjacent(current.Start, viewInfo)) {
					current.StartCorner.AdjacentDependencyViewInfos.Add(viewInfo);
					current.StartCorner.ConnectionPoint = current.Start;
					current.StartCorner.Type = DependencyViewInfoCornerTypeCalculator.GetCornerType(current, current.StartCorner);
				}
				if (AreAdjacent(current.End, viewInfo)) {
					current.EndCorner.AdjacentDependencyViewInfos.Add(viewInfo);
					current.EndCorner.ConnectionPoint = current.End;
					current.EndCorner.Type = DependencyViewInfoCornerTypeCalculator.GetCornerType(current, current.EndCorner);
				}
			}
		}
		bool AreAdjacent(Point pt, DependencyViewInfo viewInfo) {
			return pt == viewInfo.Start || pt == viewInfo.End;
		}
	}
	#endregion
	#region DependencyViewInfosDependenciesCalculator
	public class DependencyViewInfosDependenciesCalculator {
		#region Fields
		DependencyIntermediateViewInfoCollection dependencyViewInfos;
		DependencyGroup group;
		#endregion
		public DependencyViewInfosDependenciesCalculator(DependencyIntermediateViewInfoCollection dependencyViewInfos, DependencyGroup group) {
			this.dependencyViewInfos = new DependencyIntermediateViewInfoCollection();
			this.dependencyViewInfos.AddRange(dependencyViewInfos);
			this.group = group;
		}
		#region Properties
		internal DependencyIntermediateViewInfoCollection DependencyViewInfos { get { return dependencyViewInfos; } }
		internal DependencyGroup Group { get { return group; } }
		#endregion
		protected internal virtual void Calculate() {
			int count = group.Dependencies.Count;
			for (int i = 0; i < count; i++)
				AddDependency(Group.Dependencies[i], Group.ParentViewInfos, Group.DependentViewInfos);
		}
		protected internal virtual void AddDependency(AppointmentDependency dependency, GanttAppointmentViewInfoWrapperCollection parents, GanttAppointmentViewInfoWrapperCollection dependents) {
			GanttAppointmentViewInfoWrapperCollection parentsByDependency = GetAppointmentViewInfosById(parents, dependency.ParentId);
			GanttAppointmentViewInfoWrapperCollection dependentsByDependency = GetAppointmentViewInfosById(dependents, dependency.DependentId);
			int parentsCount = parentsByDependency.Count;
			int dependentsCount = dependentsByDependency.Count;
			for (int parentIndex = 0; parentIndex < parentsCount; parentIndex++) {
				GanttAppointmentViewInfoWrapper parentViewInfo = parentsByDependency[parentIndex];
				for (int dependentsIndex = 0; dependentsIndex < dependentsCount; dependentsIndex++)
					AddDependencyCore(dependency, parentViewInfo, dependentsByDependency[dependentsIndex]);
			}
		}
		protected internal virtual void AddDependencyCore(AppointmentDependency dependency, GanttAppointmentViewInfoWrapper parentViewInfo, GanttAppointmentViewInfoWrapper dependentViewInfo) {
			DependencyViewInfo start = GetStartDependencyViewInfo(parentViewInfo);
			if (start == null)
				return;
			DependencyViewInfoPathTracer calculator = new DependencyViewInfoPathTracer(start, dependentViewInfo);
			List<DependencyViewInfo> path = calculator.FindPath();
			int count = path.Count;
			for (int i = 0; i < count; i++)
				path[i].Dependencies.Add(dependency);
		}
		GanttAppointmentViewInfoWrapperCollection GetAppointmentViewInfosById(GanttAppointmentViewInfoWrapperCollection collection, object id) {
			GanttAppointmentViewInfoWrapperCollection result = new GanttAppointmentViewInfoWrapperCollection();
			int count = collection.Count;
			for (int i = 0; i < count; i++) {
				if (Object.Equals(collection[i].Id, id))
					result.Add(collection[i]);
			}
			return result;
		}
		DependencyViewInfo GetStartDependencyViewInfo(GanttAppointmentViewInfoWrapper parentViewInfo) {
			int intemediateCount = DependencyViewInfos.Count;
			for (int intermediateIndex = 0; intermediateIndex < intemediateCount; intermediateIndex++) {
				DependencyViewInfoCollection collection = DependencyViewInfos[intermediateIndex].DependencyViewInfos;
				int viewInfosCount = collection.Count;
				for (int viewInfoIndex = 0; viewInfoIndex < viewInfosCount; viewInfoIndex++) {
					DependencyViewInfo currentDependencyViewInfo = collection[viewInfoIndex];
					if (currentDependencyViewInfo.StartGanttViewInfo == parentViewInfo)
						return currentDependencyViewInfo;
				}
			}
			return null;
		}
	}
	#endregion
	#region GanttAppointmentViewInfoParentPositionComparer
	public class GanttAppointmentViewInfoParentPositionComparer : IComparer<GanttAppointmentViewInfoWrapper> {
		#region IComparer<GanttAppointmentViewInfo> Members
		public int Compare(GanttAppointmentViewInfoWrapper x, GanttAppointmentViewInfoWrapper y) {
			Point xPoint = GetPoint(x);
			Point yPoint = GetPoint(y);
			int result = Comparer.Default.Compare(xPoint.X, yPoint.X);
			if (result == 0)
				result = Comparer.Default.Compare(xPoint.Y, yPoint.Y);
			return result;
		}
		Point GetPoint(GanttAppointmentViewInfoWrapper apt) {
			return apt.GetNextDependencyConnectionPoint();
		}
		#endregion
	}
	#endregion
	#region GanttAppointmentViewInfoDependentPositionComparer
	public class GanttAppointmentViewInfoDependentPositionComparer : IComparer<GanttAppointmentViewInfoWrapper> {
		Point basePoint;
		public GanttAppointmentViewInfoDependentPositionComparer(Point basePoint) {
			this.basePoint = basePoint;
		}
		#region IComparer<GanttAppointmentViewInfo> Members
		public int Compare(GanttAppointmentViewInfoWrapper x, GanttAppointmentViewInfoWrapper y) {
			Point xPoint = x.GetNextDependencyConnectionPoint();
			Point yPoint = y.GetNextDependencyConnectionPoint();
			int distanceX = Math.Abs(xPoint.X - this.basePoint.X) + Math.Abs(xPoint.Y - this.basePoint.Y);
			int distanceY = Math.Abs(yPoint.X - this.basePoint.X) + Math.Abs(yPoint.Y - this.basePoint.Y);
			return Comparer.Default.Compare(distanceX, distanceY);
		}
		#endregion
	}
	#endregion
	#region DependencyLayoutUtils
	public static class DependencyLayoutUtils {
		public static bool IsIntersectsRectangle(Rectangle aptRectangle, Point start, Point end) {
			if (start.Y == end.Y)
				return IsIntersectsRectangleHorizontally(aptRectangle, start, end);
			return IsIntersectsRectangleVertically(aptRectangle, start, end);
		}
		public static bool ShouldRoundAppointment(ConnectorItemBase aptConnector, Point pathEnd) {
			if (aptConnector.Direction == DependencyDirection.Vertical)
				return false;
			if (aptConnector.Position == ConnectorPosition.Finish)
				return pathEnd.X < aptConnector.PointPath.X;
			else
				return pathEnd.X > aptConnector.PointPath.X;
		}
		public static bool ContainsPointX(Point edge1, Point edge2, int x) {
			int start = Math.Min(edge1.X, edge2.X);
			int end = Math.Max(edge1.X, edge2.X);
			return (start <= x) && (x <= end);
		}
		public static bool ContainsPointX(Point edge1, Point edge2, Point pt) {
			return ContainsPointX(edge1, edge2, pt.X);
		}
		public static bool ContainsPointY(Point edge1, Point edge2, int y) {
			int start = Math.Min(edge1.Y, edge2.Y);
			int end = Math.Max(edge1.Y, edge2.Y);
			return (start <= y) && (y <= end);
		}
		public static bool ContainsPointY(Point edge1, Point edge2, Point pt) {
			return ContainsPointY(edge1, edge2, pt.Y);
		}
		public static Point ContainsPoint(PathItemBase startPath, PathItemBase endPath) {
			Point point = Point.Empty;
			if (DependencyLayoutUtils.ContainsPointY(startPath.Start, startPath.End, endPath.Start.Y))
				point.Y = endPath.Start.Y;
			if (DependencyLayoutUtils.ContainsPointY(endPath.Start, endPath.End, startPath.Start.Y))
				point.Y = startPath.Start.Y;
			if (DependencyLayoutUtils.ContainsPointX(startPath.Start, startPath.End, endPath.Start.X))
				point.X = endPath.Start.X;
			if (DependencyLayoutUtils.ContainsPointX(endPath.Start, endPath.End, startPath.Start.X))
				point.X = startPath.Start.X;
			if (point.X != 0 && point.Y != 0)
				return point;
			return Point.Empty;
		}
		public static Point ContainsPoint(DependencyViewInfo startPath, DependencyViewInfo endPath) {
			Point point = Point.Empty;
			if (DependencyLayoutUtils.ContainsPointY(startPath.Start, startPath.End, endPath.Start.Y))
				point.Y = endPath.Start.Y;
			if (DependencyLayoutUtils.ContainsPointY(endPath.Start, endPath.End, startPath.Start.Y))
				point.Y = startPath.Start.Y;
			if (DependencyLayoutUtils.ContainsPointX(startPath.Start, startPath.End, endPath.Start.X))
				point.X = endPath.Start.X;
			if (DependencyLayoutUtils.ContainsPointX(endPath.Start, endPath.End, startPath.Start.X))
				point.X = startPath.Start.X;
			if (point.X != 0 && point.Y != 0)
				return point;
			return Point.Empty;
		}
		static bool IsIntersectsRectangleHorizontally(Rectangle aptRectangle, Point start, Point end) {
			if (start.Y <= aptRectangle.Top || start.Y >= aptRectangle.Bottom)
				return false;
			return AreIntersectedSegments(Math.Min(start.X, end.X), Math.Max(start.X, end.X), aptRectangle.Left, aptRectangle.Right);
		}
		static bool IsIntersectsRectangleVertically(Rectangle aptRectangle, Point start, Point end) {
			if (start.X <= aptRectangle.Left || start.X >= aptRectangle.Right)
				return false;
			return AreIntersectedSegments(Math.Min(start.Y, end.Y), Math.Max(start.Y, end.Y), aptRectangle.Top, aptRectangle.Bottom);
		}
		static bool AreIntersectedSegments(int firstSegmentStart, int firstSegmentEnd, int secondSegmentStart, int secondSegmentEnd) {
			return (firstSegmentStart == secondSegmentStart && firstSegmentEnd == secondSegmentEnd) ||
				(firstSegmentStart > secondSegmentStart && firstSegmentStart < secondSegmentEnd) ||
				(firstSegmentEnd > secondSegmentStart && firstSegmentEnd < secondSegmentEnd) ||
				(secondSegmentStart > firstSegmentStart && secondSegmentStart < firstSegmentEnd) ||
				(secondSegmentEnd > firstSegmentStart && secondSegmentEnd < firstSegmentEnd);
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Drawing.Native {
	public class DependencyViewInfoPathTracer {
		class DependencyViewInfoTreeItem {
			public DependencyViewInfoTreeItem(int rootIndex, DependencyViewInfo info) {
				RootIndex = rootIndex;
				DependencyViewInfo = info;
			}
			public int RootIndex { get; set; }
			public DependencyViewInfo DependencyViewInfo { get; set; }
		}
		public DependencyViewInfoPathTracer(DependencyViewInfo startDependencyViewInfo, GanttAppointmentViewInfoWrapper endAppointmentViewInfoWrapper) {
			StartDependencyViewInfo = startDependencyViewInfo;
			EndAppointmentViewInfoWrapper = endAppointmentViewInfoWrapper;
		}
		public DependencyViewInfo StartDependencyViewInfo { get; private set; }
		public GanttAppointmentViewInfoWrapper EndAppointmentViewInfoWrapper { get; private set; }
		public List<DependencyViewInfo> FindPath() {
			List<DependencyViewInfoTreeItem> map = new List<DependencyViewInfoTreeItem>();
			DependencyViewInfoTreeItem item = new DependencyViewInfoTreeItem(-1, StartDependencyViewInfo);
			map.Add(item);
			return FindDependencyPathRecursive(map, EndAppointmentViewInfoWrapper, 0);
		}
		List<DependencyViewInfo> FindDependencyPathRecursive(List<DependencyViewInfoTreeItem> map, GanttAppointmentViewInfoWrapper endAppointmentViewInfoWrapper, int startIndx) {
			List<DependencyViewInfoTreeItem> newGenerationMap = new List<DependencyViewInfoTreeItem>();
			for (int i = startIndx; i < map.Count; i++) {
				DependencyViewInfo currentPathEnd = map[i].DependencyViewInfo;
				if (currentPathEnd.EndGanttViewInfo != null) {
					if (currentPathEnd.EndGanttViewInfo == endAppointmentViewInfoWrapper)
						return GetPath(map, i);
				}
				List<DependencyViewInfo> adjacentDependencies = GetAdjacentDependencies(currentPathEnd);
				int mapRootIndx = map[i].RootIndex;
				DependencyViewInfo prevRootDependencyViewInfo = (mapRootIndx < 0) ? null : map[mapRootIndx].DependencyViewInfo;
				foreach (DependencyViewInfo child in adjacentDependencies) {
					if (prevRootDependencyViewInfo == child)
						continue;
					DependencyViewInfoTreeItem existedItem = map.Find(a => a.DependencyViewInfo == child);
					if (existedItem != null)
						continue;
					DependencyViewInfoTreeItem item = new DependencyViewInfoTreeItem(i, child);
					newGenerationMap.Add(item);
				}
			}
			if (newGenerationMap.Count == 0)
				return new List<DependencyViewInfo>();
			startIndx = map.Count;
			map.AddRange(newGenerationMap);
			return FindDependencyPathRecursive(map, endAppointmentViewInfoWrapper, startIndx);
		}
		List<DependencyViewInfo> GetPath(List<DependencyViewInfoTreeItem> map, int startIndx) {
			List<DependencyViewInfo> result = new List<DependencyViewInfo>();
			for (int i = startIndx; i > -1; i = map[i].RootIndex)
				result.Add(map[i].DependencyViewInfo);
			result.Reverse(0, result.Count);
			return result;
		}
		List<DependencyViewInfo> GetAdjacentDependencies(DependencyViewInfo currentPathEnd) {
			List<DependencyViewInfo> result = new List<DependencyViewInfo>();
			result.AddRange(currentPathEnd.EndCorner.AdjacentDependencyViewInfos);
			result.AddRange(currentPathEnd.StartCorner.AdjacentDependencyViewInfos);
			return result;
		}
	}
}

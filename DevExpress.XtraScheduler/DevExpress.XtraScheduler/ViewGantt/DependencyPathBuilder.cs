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
using System.Drawing;
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Drawing {
	public class PathPreliminaryResult {
		PathItemCollection path;
		IntersectionsInfo intersections;
		public PathPreliminaryResult(PathItemCollection path, IntersectionsInfo intersections) {
			this.path = path;
			this.intersections = intersections;
		}
		public PathItemCollection Path { get { return path; } }
		public IntersectionsInfo Intersections { get { return intersections; } }
	}
	public class PathPreliminaryResultComparer : IComparer<PathPreliminaryResult> {
		#region IComparer<PathPreliminaryResult> Members
		public int Compare(PathPreliminaryResult x, PathPreliminaryResult y) {
			IntersectionsInfo xIntersections = x.Intersections;
			IntersectionsInfo yIntersections = y.Intersections;
			if (xIntersections.AppointmentsIntersectionCount < yIntersections.AppointmentsIntersectionCount)
				return -1;
			if (xIntersections.AppointmentsIntersectionCount > yIntersections.AppointmentsIntersectionCount)
				return 1;
			if (xIntersections.DependenciesIntersectionCount < yIntersections.DependenciesIntersectionCount)
				return -1;
			if (xIntersections.DependenciesIntersectionCount > yIntersections.DependenciesIntersectionCount)
				return 1;
			int comparer = x.Path.Count.CompareTo(y.Path.Count);
			if (comparer == 0)
				comparer = x.Path.CalcPathLength().CompareTo(y.Path.CalcPathLength());
			return comparer;
		}
		#endregion
	}
	#region DependencyPathBuilderBase
	public abstract class DependencyPathBuilderBase {
		#region Fields
		bool inverseResult = false;
		DependencyConnectorCalculator connectorCalculator;
		GanttAppointmentViewInfoCollection pathOwners;
		#endregion
		protected DependencyPathBuilderBase(DependencyConnectorCalculator connectorCalculator) {
			this.connectorCalculator = connectorCalculator;
			this.pathOwners = new GanttAppointmentViewInfoCollection();
		}
		#region Properties
		protected internal IntersectionObjectsInfo IntersectionObjects { get { return connectorCalculator.IntersectionObjects; } }
		protected internal bool InverseResult { get { return inverseResult; } set { inverseResult = value; } }
		protected internal Rectangle VisibleBounds { get { return connectorCalculator.VisibleBounds; } }
		protected internal DependencyConnectorCalculator ConnectorCalculator { get { return connectorCalculator; } }
		protected internal GanttAppointmentViewInfoCollection PathOwners { get { return pathOwners; } }
		#endregion
		public abstract DependencyIntermediateViewInfoCollection BuildPath();
		protected internal PathItemCollection BuildPathCore() {
			PathItemCollection path = BuildOptimalPathCore();
			if (path == null)
				return new PathItemCollection();
			if (InverseResult)
				path.Inverse();
			return path;
		}
		protected internal virtual PathItemCollection BuildOptimalPathCore() {
			RoundElementType type = RoundElementType.All;
			RoundElementType[] ignoreObjects = new RoundElementType[] { RoundElementType.None, RoundElementType.AppointmentVertical, RoundElementType.AppointmentHorizontal };
			List<PathPreliminaryResult> preliminaryPaths = new List<PathPreliminaryResult>();
			for (int i = 0; i < ignoreObjects.Length; i++) {
				type &= ~ignoreObjects[i];
				PathPreliminaryResult path = CalculatePreliminaryPath(type);
				if (path == null)
					continue;
				if (path.Intersections.IntersectionType == RoundElementType.None)
					return path.Path;
				preliminaryPaths.Add(path);
			}
			preliminaryPaths.Sort(new PathPreliminaryResultComparer());
			if (preliminaryPaths == null || preliminaryPaths.Count <= 0)
				return null;
			return preliminaryPaths[0].Path;
		}
		protected internal virtual PathPreliminaryResult CalculatePreliminaryPath(RoundElementType roundType) {
			List<PathPreliminaryResult> result = new List<PathPreliminaryResult>();
			List<PathItemCollection> paths = CalculatePreliminaryPathCore(roundType);
			if (paths == null || paths.Count == 0)
				return null;
			for (int i = 0; i < paths.Count; i++) {
				PathItemCollection path = paths[i];
				path = RemoveEmptyItems(path);
				if (path == null || path.Count == 0)
					continue;
				IntersectionsInfo intersections = CalculateIntersections(path);
				result.Add(new PathPreliminaryResult(path, intersections));
			}
			if (result == null || result.Count == 0)
				return null;
			result.Sort(new PathPreliminaryResultComparer());
			return result[0];
		}
		protected internal virtual IntersectionsInfo CalculateIntersections(PathItemCollection path) {
			int count = path.Count;
			RoundElementType intersectionType = RoundElementType.None;
			int appointmentsIntersectionCount = 0;
			int dependenciesIntersectionCount = 0;
			for (int i = 0; (i < count) && (intersectionType != RoundElementType.All); i++) {
				PathItemBase item = path[i];
				IntersectionsInfo intersectionsInfo = IntersectionObjects.CalculateIntersectionsType(item.Start, item.End, ~intersectionType);
				intersectionType |= intersectionsInfo.IntersectionType;
				appointmentsIntersectionCount += intersectionsInfo.AppointmentsIntersectionCount;
				dependenciesIntersectionCount += intersectionsInfo.DependenciesIntersectionCount;
			}
			return new IntersectionsInfo(intersectionType, appointmentsIntersectionCount, dependenciesIntersectionCount);
		}
		protected internal PathItemCollection ClonePath(PathItemCollection path) {
			PathItemCollection result = new PathItemCollection();
			int count = path.Count;
			for (int i = 0; i < count; i++)
				result.Add(path[i].Clone());
			return result;
		}
		protected internal abstract List<PathItemCollection> CalculatePreliminaryPathCore(RoundElementType roundType);
		protected void AddItemToPath(PathItemCollection path, PathItemBase item) {
			path.Add(item);
		}
		protected void AddItemsRangeToPath(PathItemCollection destination, PathItemCollection source) {
			int count = source.Count;
			for (int i = 0; i < count; i++) {
				AddItemToPath(destination, (PathItem)source[i].Clone());
			}
		}
		protected internal virtual List<PathItemCollection> CalculateOptimalPaths(List<PathItemCollection> initialPaths, RoundElementType roundType) {
			int count = initialPaths.Count;
			for (int i = 0; i < count; i++)
				OptimizePath(initialPaths[i], roundType);
			return initialPaths;
		}
		protected virtual void OptimizePath(PathItemCollection initialPath, RoundElementType roundType) {
			int currentIndex = 0;
			int count = initialPath.Count;
			while (currentIndex < count) {
				PathItem item = (PathItem)initialPath[currentIndex];
				if (currentIndex == 0 || item.Validate(roundType)) {
					if (item.Length != 0 && item.Length < 6 && currentIndex + 1 < count) {
						PathItem itemNext = (PathItem)initialPath[currentIndex + 1];
						itemNext.SetStart(item.Start);
						if (currentIndex + 2 < count) {
							item.SetEnd(item.Start);
							PathItem itemNext2 = (PathItem)initialPath[currentIndex + 2];
							Point point = CalculateOptimalPointIntersection(item, itemNext2);
							itemNext.SetEnd(point);
							itemNext2.SetStart(itemNext.End);
						} else {
							Point point = CalculateOptimalPointIntersection(item, itemNext);
							item.SetEnd(itemNext.End);
							item.SetStart(point);
							itemNext.SetEnd(point);
							initialPath.RemoveAt(currentIndex);
							initialPath.RemoveAt(currentIndex);
							initialPath.Insert(currentIndex, itemNext);
							initialPath.Insert(currentIndex + 1, item);
						}
						currentIndex++;
					}
					currentIndex++;
				} else {
					UpdateBounds(initialPath, currentIndex);
					if (currentIndex > 0)
						currentIndex--;
				}
			}
		}
		Point CalculateOptimalPointIntersection(PathItem item, PathItem itemNext) {
			if (item.Direction == itemNext.Direction && itemNext.Start == item.End)
				return itemNext.Start;
			if (item.Direction == DependencyDirection.Vertical)
				return new Point(itemNext.End.X, item.Start.Y);
			return new Point(item.Start.X, itemNext.End.Y);
		}
		protected internal virtual int CalculateAvailableHorizontalBound(ConnectorItemBase aptConnector, Point pathEnd, RoundElementType roundType) {
			Point pathStart = aptConnector.PointPath;
			if (!DependencyLayoutUtils.ShouldRoundAppointment(aptConnector, pathEnd))
				return pathStart.X;
			return CalculateAvailableHorizontalBoundCore(aptConnector, roundType);
		}
		protected internal virtual int CalculateAvailableHorizontalBoundCore(ConnectorItemBase aptConnector, RoundElementType roundType) {
			Point pathStart = aptConnector.PointPath;
			Rectangle intersections;
			if (aptConnector.Position == ConnectorPosition.Finish) {
				intersections = IntersectionObjects.CalculateIntersections(pathStart, new Point(VisibleBounds.Right, pathStart.Y), PathOwners, roundType);
				return intersections == Rectangle.Empty ? VisibleBounds.Right : intersections.Left;
			}
			intersections = IntersectionObjects.CalculateIntersections(new Point(VisibleBounds.Left, pathStart.Y), pathStart, PathOwners, roundType);
			return (intersections == Rectangle.Empty) ? VisibleBounds.Left : intersections.Right;
		}
		protected internal virtual DependencyIntermediateViewInfoCollection CalculateIntermediateViewInfosByPath(PathItemCollection path) {
			int count = path.Count;
			XtraSchedulerDebug.Assert(count > 0);
			DependencyIntermediateViewInfoCollection result = new DependencyIntermediateViewInfoCollection();
			result.Add(CreateViewInfo(path[0]));
			for (int i = 1; i < count; i++) {
				DependencyIntermediateViewInfo prevViewInfo = result[result.Count - 1];
				PathItemBase pathItem = path[i];
				XtraSchedulerDebug.Assert(prevViewInfo.DependencyViewInfos[0].End == pathItem.Start);
				XtraSchedulerDebug.Assert(pathItem.Length >= 0);
				if (prevViewInfo.Direction == pathItem.Direction)
					prevViewInfo.DependencyViewInfos[0].End = pathItem.End;
				else
					result.Add(CreateViewInfo(pathItem));
			}
			return result;
		}
		protected internal virtual DependencyIntermediateViewInfo CreateViewInfo(PathItemBase item) {
			return new DependencyIntermediateViewInfo(new DependencyViewInfo(item.Start, item.End));
		}
		protected internal void InitializeFirstViewInfo(DependencyIntermediateViewInfoCollection dependencyViewInfos, GanttAppointmentViewInfoWrapper aptViewInfo) {
			dependencyViewInfos[0].DependencyViewInfos[0].StartGanttViewInfo = aptViewInfo;
		}
		protected internal void InitializeLastViewInfo(DependencyIntermediateViewInfoCollection dependencyViewInfos, GanttAppointmentViewInfoWrapper aptViewInfo) {
			DependencyViewInfoCollection last = dependencyViewInfos[dependencyViewInfos.Count - 1].DependencyViewInfos;
			last[last.Count - 1].EndGanttViewInfo = aptViewInfo;
		}
		protected abstract int CalculatePathLength(PathItemCollection path);
		void UpdateBounds(PathItemCollection path, int index) {
			PathItemBase current = path[index];
			int previousIndex = index - 1;
			if (previousIndex >= 0)
				path[previousIndex].SetEnd(current.Start);
			int nextIndex = index + 1;
			if (nextIndex < path.Count)
				path[nextIndex].SetStart(current.End);
		}
		PathItemCollection RemoveEmptyItems(PathItemCollection path) {
			PathItemCollection result = new PathItemCollection();
			int count = path.Count;
			for (int i = 0; i < count; i++)
				if (path[i].Length > 0)
					result.Add(path[i]);
			return result;
		}
	}
	#endregion
	#region AppointmetnToAppointmentPathBuilder
	public class AppointmentToAppointmentPathBuilder : DependencyPathBuilderBase {
		internal const int MaxFootstep = 10;
		#region Fields
		Point geometricStart;
		Point geometricEnd;
		ConnectorItemBase startConnector;
		ConnectorItemBase endConnector;
		GanttAppointmentViewInfoWrapper parentApt;
		GanttAppointmentViewInfoWrapper dependentApt;
		ConnectorItemBase geometricStartConnector;
		ConnectorItemBase geometricEndConnector;
		#endregion
		public AppointmentToAppointmentPathBuilder(GanttAppointmentViewInfoWrapper parentApt, GanttAppointmentViewInfoWrapper dependentApt, DependencyConnectorCalculator connectorCalculator)
			: base(connectorCalculator) {
			InitializeAppointments(parentApt, dependentApt);
			CreateConnectors();
			InitializePoints();
			PopulatePathOwners();
		}
		#region Properties
		protected internal Point GeometricStart { get { return geometricStart; } set { geometricStart = value; } }
		protected internal Point GeometricEnd { get { return geometricEnd; } set { geometricEnd = value; } }
		protected internal ConnectorItemBase EndConnector { get { return endConnector; } }
		protected internal ConnectorItemBase StartConnector { get { return startConnector; } }
		protected internal GanttAppointmentViewInfoWrapper ParentApt { get { return parentApt; } }
		protected internal GanttAppointmentViewInfoWrapper DependentApt { get { return dependentApt; } }
		protected internal ConnectorItemBase GeometricStartConnector { get { return geometricStartConnector; } }
		protected internal ConnectorItemBase GeometricEndConnector { get { return geometricEndConnector; } }
		#endregion
		public override DependencyIntermediateViewInfoCollection BuildPath() {
#if DEBUG || DEBUGTEST
			XtraSchedulerDebug.Assert(ParentApt.IsParent);
			XtraSchedulerDebug.Assert(!DependentApt.IsParent);
#endif
			PathItemCollection path = BuildPathCore();
			AddConnectorItems(path);
			return CalculateIntermediateViewInfos(path);
		}
		protected internal void CreateConnectors() {
			this.startConnector = ConnectorCalculator.CreateConnectorItem(ParentApt, DependentApt);
			this.endConnector = ConnectorCalculator.CreateConnectorItem(DependentApt, ParentApt, this.startConnector);
			Point containsPoint = DependencyLayoutUtils.ContainsPoint(startConnector, endConnector);
			if (containsPoint != Point.Empty) {
				this.startConnector.PointPath = containsPoint;
				this.endConnector.PointPath = containsPoint;
			}
			this.endConnector.IsPathEndItem = true;
			ConnectorCalculator.ValidateConnectors(this.startConnector, this.endConnector, ParentApt.ConnectionPointsInfo, DependentApt.ConnectionPointsInfo);
		}
		protected internal void PopulatePathOwners() {
			PathOwners.Add(ParentApt.ViewInfo);
			PathOwners.Add(DependentApt.ViewInfo);
		}
		protected internal virtual void AddConnectorItems(PathItemCollection path) {
			path.Insert(0, StartConnector);
			path.Add(EndConnector);
		}
		protected internal virtual DependencyIntermediateViewInfoCollection CalculateIntermediateViewInfos(PathItemCollection path) {
			DependencyIntermediateViewInfoCollection result = CalculateIntermediateViewInfosByPath(path);
			InitializeViewInfos(result);
			return result;
		}
		protected internal virtual void InitializeViewInfos(DependencyIntermediateViewInfoCollection dependencyViewInfos) {
			InitializeFirstViewInfo(dependencyViewInfos, ParentApt);
			InitializeLastViewInfo(dependencyViewInfos, DependentApt);
		}
		protected internal override List<PathItemCollection> CalculatePreliminaryPathCore(RoundElementType roundType) {
			List<PathItemCollection> result = new List<PathItemCollection>();
			Rectangle verticalIntersections = CalculateVerticalIntersections(roundType);
			Rectangle availableBounds = CalculateAvailableBounds(roundType);
			PathHorizontalFixedStartItem horFixedStartItem = new PathHorizontalFixedStartItem(GeometricStart, new Point(verticalIntersections.Left, GeometricStart.Y), IntersectionObjects, PathOwners);
			if (!horFixedStartItem.Validate(roundType))
				verticalIntersections = RectUtils.SetLeft(verticalIntersections, horFixedStartItem.End.X);
			horFixedStartItem.SkipValidation = true;
			PathHorizontalFixedEndItem horFixedEndItem = new PathHorizontalFixedEndItem(new Point(verticalIntersections.Right, GeometricEnd.Y), GeometricEnd, IntersectionObjects, PathOwners);
			if (!horFixedEndItem.Validate(roundType))
				verticalIntersections = RectUtils.SetRight(verticalIntersections, horFixedEndItem.Start.X);
			horFixedEndItem.SkipValidation = true;
			if (verticalIntersections.Width == 0) {
				if ((horFixedStartItem.Length + horFixedEndItem.Length < MaxFootstep)) {
					if (GeometricStartConnector.ConnectorLocation == ConnectorLocation.BottomRight && GeometricEndConnector.ConnectorLocation == ConnectorLocation.Left) {
						result.Add(CreateDefaultVerticalToHorizontalLShapedPath(horFixedStartItem.Start, horFixedEndItem.End, availableBounds));
						return result;
					} else if (GeometricStartConnector.ConnectorLocation == ConnectorLocation.Right && GeometricEndConnector.ConnectorLocation == ConnectorLocation.TopLeft) {
						result.Add(CreateDefaultHorizontalToVerticalLShapedPath(horFixedStartItem.Start, horFixedEndItem.End, availableBounds));
						return result;
					}
				}
				result.Add(CreateDefaultPath(horFixedStartItem, horFixedEndItem, availableBounds));
				return result;
			}
			result = BuildPathsWithRound(horFixedStartItem, horFixedEndItem, verticalIntersections, availableBounds, roundType);
			return result;
		}
		protected internal virtual Rectangle CalculateVerticalIntersections(RoundElementType roundType) {
			int halfX;
			if (Math.Abs(GeometricStart.X - GeometricEnd.X) < ConnectorCalculator.ConnectorIndent * 2)
				halfX = Math.Min(GeometricStart.X, GeometricEnd.X);
			else if (GeometricStart.X <= VisibleBounds.Left)
				halfX = GeometricEnd.X;
			else if (GeometricEnd.X >= VisibleBounds.Right)
				halfX = GeometricStart.X;
			else
				halfX = (GeometricStart.X + GeometricEnd.X) / 2;
			Rectangle verticalIntersectionRect = IntersectionObjects.CalculateIntersections(new Point(halfX, GeometricStart.Y), new Point(halfX, GeometricEnd.Y), PathOwners, roundType);
			if (verticalIntersectionRect == Rectangle.Empty)
				return Rectangle.FromLTRB(halfX, Math.Min(GeometricStart.Y, GeometricEnd.Y), halfX, Math.Max(GeometricStart.Y, GeometricEnd.Y));
			int left = Math.Max(GeometricStart.X, verticalIntersectionRect.Left);
			int right = Math.Min(GeometricEnd.X, verticalIntersectionRect.Right);
			return Rectangle.FromLTRB(left, verticalIntersectionRect.Top, right, verticalIntersectionRect.Bottom);
		}
		protected internal virtual PathItemCollection CreateDefaultPath(PathHorizontalFixedStartItem startItem, PathHorizontalFixedEndItem endItem, Rectangle availableBounds) {
			PathItemCollection result = new PathItemCollection();
			PathVerticalItem verticalItem = new PathVerticalItem(startItem.End, endItem.Start, IntersectionObjects, PathVerticalItemType.MoveLeft, availableBounds, PathOwners);
			AddItemToPath(result, startItem);
			AddItemToPath(result, verticalItem);
			AddItemToPath(result, endItem);
			return result;
		}
		protected internal virtual PathItemCollection CreateDefaultHorizontalToVerticalLShapedPath(Point start, Point end, Rectangle availableBounds) {
			PathItemCollection result = new PathItemCollection();
			PathHorizontalItem horizontalItem = new PathHorizontalItem(start, new Point(end.X, start.Y), IntersectionObjects, PathHorizontalItemType.MoveDown, availableBounds, PathOwners);
			PathVerticalItem verticalItem = new PathVerticalItem(new Point(end.X, start.Y), end, IntersectionObjects, PathVerticalItemType.MoveLeft, availableBounds, PathOwners);
			AddItemToPath(result, horizontalItem);
			AddItemToPath(result, verticalItem);
			return result;
		}
		protected internal virtual PathItemCollection CreateDefaultVerticalToHorizontalLShapedPath(Point start, Point end, Rectangle availableBounds) {
			PathItemCollection result = new PathItemCollection();
			PathVerticalItem verticalItem = new PathVerticalItem(start, new Point(start.X, end.Y), IntersectionObjects, PathVerticalItemType.MoveLeft, availableBounds, PathOwners);
			PathHorizontalItem horizontalItem = new PathHorizontalItem(new Point(start.X, end.Y), end, IntersectionObjects, PathHorizontalItemType.MoveDown, availableBounds, PathOwners);
			AddItemToPath(result, verticalItem);
			AddItemToPath(result, horizontalItem);
			return result;
		}
		protected override int CalculatePathLength(PathItemCollection path) {
			return path.CalcPathLength();
		}
		protected virtual Rectangle CalculateAvailableBounds(RoundElementType roundType) {
			int horBound1 = CalculateAvailableHorizontalBound(StartConnector, EndConnector.PointPath, roundType);
			int horBound2 = CalculateAvailableHorizontalBound(EndConnector, StartConnector.PointPath, roundType);
			Rectangle result = Rectangle.FromLTRB(Math.Min(horBound1, horBound2), VisibleBounds.Top, Math.Max(horBound1, horBound2), VisibleBounds.Bottom);
			XtraSchedulerDebug.Assert(RectUtils.ContainsX(result, GeometricStart));
			XtraSchedulerDebug.Assert(RectUtils.ContainsX(result, GeometricEnd));
			return result;
		}
		void InitializeAppointments(GanttAppointmentViewInfoWrapper parentApt, GanttAppointmentViewInfoWrapper dependentApt) {
			this.parentApt = parentApt;
			this.dependentApt = dependentApt;
		}
		void InitializePoints() {
			Point pathStart = StartConnector.PointPath;
			Point pathEnd = EndConnector.PointPath;
			if (pathStart.X < pathEnd.X) {
				this.geometricStart = pathStart;
				this.geometricEnd = pathEnd;
				this.geometricStartConnector = StartConnector;
				this.geometricEndConnector = EndConnector;
			} else {
				this.geometricStart = pathEnd;
				this.geometricEnd = pathStart;
				this.geometricStartConnector = EndConnector;
				this.geometricEndConnector = StartConnector;
			}
			InverseResult = pathStart != GeometricStart;
		}
		List<PathItemCollection> BuildPathsWithRound(PathHorizontalFixedStartItem startItem, PathHorizontalFixedEndItem endItem, Rectangle verticalIntersections, Rectangle availableBounds, RoundElementType roundType) {
			PathItemCollection lowerPath = CreateRoundPathWithoutOptimization(startItem, endItem, verticalIntersections, PathHorizontalItemType.MoveDown, availableBounds);
			PathItemCollection upperPath = CreateRoundPathWithoutOptimization(startItem, endItem, verticalIntersections, PathHorizontalItemType.MoveUp, availableBounds);
			List<PathItemCollection> paths = new List<PathItemCollection>();
			paths.Add(lowerPath);
			paths.Add(upperPath);
			AddVerticalPaths(paths, availableBounds, roundType);
			if (paths.Count <= 0)
				return null;
			return CalculateOptimalPaths(paths, roundType);
		}
		PathItemCollection CreateRoundPathWithoutOptimization(PathHorizontalFixedStartItem startItem, PathHorizontalFixedEndItem endItem, Rectangle verticalIntersections, PathHorizontalItemType type, Rectangle availableBounds) {
			PathItemCollection result = new PathItemCollection();
			int horItemY = (type == PathHorizontalItemType.MoveDown) ? verticalIntersections.Bottom : verticalIntersections.Top;
			Point left = new Point(startItem.End.X, horItemY);
			Point right = new Point(endItem.Start.X, horItemY);
			AddItemToPath(result, startItem.Clone());
			AddItemToPath(result, new PathVerticalItem(startItem.End, left, IntersectionObjects, PathVerticalItemType.MoveLeft, availableBounds, PathOwners));
			AddItemToPath(result, new PathHorizontalItem(left, right, IntersectionObjects, type, availableBounds, PathOwners));
			AddItemToPath(result, new PathVerticalItem(right, endItem.Start, IntersectionObjects, PathVerticalItemType.MoveRight, availableBounds, PathOwners));
			AddItemToPath(result, endItem.Clone());
			return result;
		}
		void AddVerticalPaths(List<PathItemCollection> paths, Rectangle availableBounds, RoundElementType roundType) {
			int halfY;
			if (Math.Abs(GeometricStart.Y - GeometricEnd.Y) < ConnectorCalculator.ConnectorIndent * 2)
				halfY = Math.Max(GeometricStart.Y, GeometricEnd.Y);
			else
				halfY = (GeometricStart.Y + GeometricEnd.Y) / 2;
			Point horItemStart = new Point(GeometricStart.X, halfY);
			Point horItemEnd = new Point(GeometricEnd.X, halfY);
			PathVerticalFixedStartItem vertStartItem1 = new PathVerticalFixedStartItem(GeometricStart, horItemStart, IntersectionObjects, PathOwners);
			PathVerticalFixedEndItem vertStartItem2 = new PathVerticalFixedEndItem(horItemEnd, GeometricEnd, IntersectionObjects, PathOwners);
			if (!vertStartItem1.Validate(roundType)) {
				vertStartItem2.SetStart(new Point(GeometricEnd.X, vertStartItem1.End.Y));
			}
			if (!vertStartItem2.Validate(roundType))
				return;
			vertStartItem1.SkipValidation = true;
			vertStartItem2.SkipValidation = true;
			paths.Add(CreateVerticalPath(vertStartItem1, vertStartItem2, PathHorizontalItemType.MoveDown, availableBounds));
			paths.Add(CreateVerticalPath(vertStartItem1, vertStartItem2, PathHorizontalItemType.MoveUp, availableBounds));
		}
		PathItemCollection CreateVerticalPath(PathVerticalFixedStartItem pathStart, PathVerticalFixedEndItem pathEnd, PathHorizontalItemType type, Rectangle availableBounds) {
			PathItemCollection result = new PathItemCollection();
			AddItemToPath(result, pathStart.Clone());
			AddItemToPath(result, new PathHorizontalItem(pathStart.End, pathEnd.Start, IntersectionObjects, type, availableBounds, PathOwners));
			AddItemToPath(result, pathEnd.Clone());
			return result;
		}
	}
	#endregion
	public abstract class AppointmentToDependencyPathBuilderBase : DependencyPathBuilderBase {
		#region Fields
		DependencyIntermediateViewInfo dependencyViewInfo;
		GanttAppointmentViewInfoWrapper aptViewInfo;
		ConnectorItemBase connector;
		#endregion
		protected AppointmentToDependencyPathBuilderBase(GanttAppointmentViewInfoWrapper aptViewInfo, DependencyIntermediateViewInfo dependencyViewInfo, DependencyConnectorCalculator connectorCalculator)
			: base(connectorCalculator) {
			this.dependencyViewInfo = dependencyViewInfo;
			this.aptViewInfo = aptViewInfo;
			this.connector = ConnectorCalculator.CreateConnectorItem(aptViewInfo, dependencyViewInfo);
			PathOwners.Add(aptViewInfo.ViewInfo);
		}
		#region Properties
		internal DependencyIntermediateViewInfo DependencyViewInfo { get { return dependencyViewInfo; } }
		internal ConnectorItemBase Connector { get { return connector; } }
		internal Point PathStart { get { return Connector.PointPath; } }
		internal GanttAppointmentViewInfoWrapper AppointmentViewInfo { get { return aptViewInfo; } }
		#endregion
		public override DependencyIntermediateViewInfoCollection BuildPath() {
			PathItemCollection path = BuildPathCore();
			if (path.Count > 0)
				if (Connector.PointPath != path[0].Start && Connector.PointPath != path[0].End) {
					Point point = DependencyLayoutUtils.ContainsPoint(Connector, path[0]);
					if (point != Point.Empty)
						Connector.PointPath = point;
					else
						return new DependencyIntermediateViewInfoCollection();
				}
			path.Insert(0, Connector);
			if (path.Count > 0)
				DependencyViewInfo.AddConnectionPoint(path[path.Count - 1].End);
			if (!aptViewInfo.IsParent)
				path.Inverse();
			return CalculateIntermediateViewInfos(path, aptViewInfo);
		}
		protected internal virtual DependencyIntermediateViewInfoCollection CalculateIntermediateViewInfos(PathItemCollection path, GanttAppointmentViewInfoWrapper aptViewInfo) {
			DependencyIntermediateViewInfoCollection result = CalculateIntermediateViewInfosByPath(path);
			if (aptViewInfo.IsParent)
				InitializeFirstViewInfo(result, aptViewInfo);
			else
				InitializeLastViewInfo(result, aptViewInfo);
			return result;
		}
	}
	#region AppointmentToVerticalDependencyPathBuilder
	public class AppointmentToVerticalDependencyPathBuilder : AppointmentToDependencyPathBuilderBase {
		#region Fields
		PathVerticalItemType verticalItemType;
		DependencyConnectionPointInfo connectionPointInfo;
		#endregion
		public AppointmentToVerticalDependencyPathBuilder(GanttAppointmentViewInfoWrapper aptViewInfo, NearestDependencySearchResult dependencySearchResult, DependencyConnectorCalculator connectorCalculator)
			: base(aptViewInfo, dependencySearchResult.ViewInfo, connectorCalculator) {
			XtraSchedulerDebug.Assert(DependencyViewInfo.Direction == DependencyDirection.Vertical);
			this.verticalItemType = (PathStart.X < DependencyViewInfo.Start.X) ? PathVerticalItemType.MoveRight : PathVerticalItemType.MoveLeft;
			this.connectionPointInfo = dependencySearchResult.PointInfo;
		}
		#region Properties
		internal PathVerticalItemType VerticalItemType { get { return verticalItemType; } }
		internal Point PointToConnect { get { return ConnectionPointInfo.Point; } }
		public DependencyConnectionPointInfo ConnectionPointInfo { get { return connectionPointInfo; } }
		#endregion
		protected internal override List<PathItemCollection> CalculatePreliminaryPathCore(RoundElementType roundType) {
			List<PathItemCollection> result = new List<PathItemCollection>();
			if (DependencyLayoutUtils.ContainsPointY(DependencyViewInfo.Start, DependencyViewInfo.End, Connector.PointPath.Y)) {
				if (Math.Abs(Connector.PointPath.X - DependencyViewInfo.End.X) > IntersectionObjectsInfo.DistanceFromDependency) {
					PathItemCollection simplePath = new PathItemCollection();
					Point endPoint = new Point(DependencyViewInfo.End.X, Connector.PointPath.Y);
					AddItemToPath(simplePath, new PathHorizontalFixedStartItem(PathStart, endPoint, IntersectionObjects, PathOwners));
					result.Add(simplePath);
				}
			}
			PathItemCollection path = new PathItemCollection();
			Rectangle availableBounds = CalculateAvailableBounds(roundType);
			ConnectStartPointToAvailableBounds(path, availableBounds);
			if (path.Count > 1) {
#if DEBUGTEST
				DebuggerGantHelper.CheckPath(path, IntersectionObjects);
#endif
				result.Add(path);
				return result;
			}
			Point horStart = (path.Count > 0) ? path[0].End : PathStart;
			Point horEnd = new Point(DependencyViewInfo.Start.X, horStart.Y);
			Rectangle horizontalIntersections = IntersectionObjects.CalculateIntersections(horStart, horEnd, PathOwners, roundType);
			if (horizontalIntersections == Rectangle.Empty) {
				Point horProjection = DependencyViewInfo.GetPointProjection(horStart);
				if (horProjection == Point.Empty) {
					PathItemCollection pathToConnectionPoint = BuildPathToConnectionPoint(horStart);
					path.AddRange(pathToConnectionPoint);
				} else
					AddItemToPath(path, new PathHorizontalFixedStartItem(horStart, horEnd, IntersectionObjects, PathOwners));
				result.Add(path);
			} else {
				result.AddRange(BuildPathWithRound(path, horStart, horizontalIntersections, availableBounds, roundType));
			}
#if DEBUGTEST
			DebuggerGantHelper.CheckPath(path, IntersectionObjects);
#endif
			return result;
		}
		protected override int CalculatePathLength(PathItemCollection path) {
			Point pathEnd = path[path.Count - 1].End;
			return path.CalcPathLength() + DependencyViewInfo.CalcDistanceToPoint(pathEnd);
		}
		protected internal virtual Rectangle CalculateAvailableBounds(RoundElementType roundType) {
			Rectangle verticalBounds = CalculateAvailableVerticalBounds(DependencyViewInfo, roundType);
			Point dependencyStart = DependencyViewInfo.Start;
			int horizBound = CalculateAvailableHorizontalBound(Connector, dependencyStart, roundType);
			int left = Math.Min(horizBound, dependencyStart.X);
			int right = Math.Max(horizBound, dependencyStart.X);
			if ((right - left) < Connector.Length) {
				right = left + Connector.Length;
			}
			return Rectangle.FromLTRB(left, verticalBounds.Top, right, verticalBounds.Bottom);
		}
		protected internal virtual Rectangle CalculateAvailableVerticalBounds(DependencyIntermediateViewInfo viewInfo, RoundElementType roundType) {
			XtraSchedulerDebug.Assert(viewInfo.Direction == DependencyDirection.Vertical);
			int x = viewInfo.Start.X;
			Rectangle intersections = IntersectionObjects.CalculateIntersections(new Point(x, viewInfo.GeometricStart.Y), new Point(x, VisibleBounds.Top), PathOwners, roundType);
			int top = (intersections == Rectangle.Empty) ? VisibleBounds.Top : intersections.Bottom;
			top = Math.Min(top, viewInfo.GeometricStart.Y);
			intersections = IntersectionObjects.CalculateIntersections(new Point(x, viewInfo.GeometricEnd.Y), new Point(x, VisibleBounds.Bottom), PathOwners, roundType);
			int bottom = (intersections == Rectangle.Empty) ? VisibleBounds.Bottom : intersections.Top - Connector.Length;
			bottom = Math.Max(bottom, viewInfo.GeometricStart.Y);
			if (bottom < top)
				bottom = VisibleBounds.Bottom;
			return Rectangle.FromLTRB(0, top, 0, bottom);
		}
		PathItemCollection BuildPathToConnectionPoint(Point start) {
			RectangularPathBuilder builder = new RectangularPathBuilder(IntersectionObjects, PathOwners);
			return builder.Build(start, ConnectionPointInfo, Connector.Length);
		}
		void ConnectStartPointToAvailableBounds(PathItemCollection path, Rectangle availableBounds) {
			if (PathStart.Y < availableBounds.Top)
				CalcConnectionToAvailableBounds(path, availableBounds, true);
			if (PathStart.Y > availableBounds.Bottom)
				CalcConnectionToAvailableBounds(path, availableBounds, false);
		}
		void CalcConnectionToAvailableBounds(PathItemCollection path, Rectangle availableBounds, bool top) {
			int y = top ? availableBounds.Top : availableBounds.Bottom;
			Point pt = new Point(PathStart.X, y);
			if (!top && DependencyViewInfo.Direction == DependencyDirection.Vertical && pt == ConnectionPointInfo.Point) {
				RectangularPathBuilder builder = new RectangularPathBuilder(IntersectionObjects, PathOwners);
				PathItemCollection pathItems = builder.Build(PathStart, connectionPointInfo, Connector.Length);
				path.AddRange(pathItems);
			} else {
				PathVerticalItem item = new PathVerticalItem(PathStart, pt, IntersectionObjects, VerticalItemType, availableBounds, PathOwners);
				item.SkipValidation = true;
				if (item.End == Connector.Start)
					item.SetEnd(Connector.End);
				AddItemToPath(path, item);
			}
		}
		List<PathItemCollection> BuildPathWithRound(PathItemCollection path, Point start, Rectangle horizontalIntersections, Rectangle availableBounds, RoundElementType roundType) {
			List<PathItemCollection> paths = new List<PathItemCollection>();
			PathItemCollection upperPath = ClonePath(path);
			PathItemCollection lowerPath = ClonePath(path);
			upperPath.AddRange(CreatePathWithoutOptimization(start, horizontalIntersections, PathHorizontalItemType.MoveUp, availableBounds));
			lowerPath.AddRange(CreatePathWithoutOptimization(start, horizontalIntersections, PathHorizontalItemType.MoveDown, availableBounds));
			paths.Add(upperPath);
			paths.Add(lowerPath);
			return CalculateOptimalPaths(paths, roundType);
		}
		PathItemCollection CreatePathWithoutOptimization(Point start, Rectangle intersection, PathHorizontalItemType type, Rectangle availableBounds) {
			PathItemCollection result = new PathItemCollection();
			int rectX = (VerticalItemType == PathVerticalItemType.MoveLeft) ? intersection.Right : intersection.Left;
			int rectY = (type == PathHorizontalItemType.MoveDown) ? intersection.Bottom : intersection.Top;
			Point hItem1End = new Point(rectX, start.Y);
			Point vItemEnd = new Point(hItem1End.X, rectY);
			Point hItem2End = new Point(DependencyViewInfo.Start.X, vItemEnd.Y);
			Point projectionToDependency = DependencyViewInfo.GetPointProjection(hItem2End);
			if (projectionToDependency == Point.Empty) {
				RectangularPathBuilder builder = new RectangularPathBuilder(IntersectionObjects, PathOwners);
				return builder.Build(start, ConnectionPointInfo, Connector.Length);
			} else {
				AddItemToPath(result, new PathHorizontalFixedStartItem(start, hItem1End, IntersectionObjects, PathOwners, true));
				AddItemToPath(result, new PathVerticalItem(hItem1End, vItemEnd, IntersectionObjects, VerticalItemType, availableBounds, PathOwners));
				AddItemToPath(result, new PathHorizontalItem(vItemEnd, hItem2End, IntersectionObjects, type, availableBounds, PathOwners));
			}
			return result;
		}
	}
	#endregion
	#region AppointmentToHorizontalDependencyPathBuilder
	public class AppointmentToHorizontalDependencyPathBuilder : AppointmentToDependencyPathBuilderBase {
		#region Fields
		Point pointToConnect;
		#endregion
		public AppointmentToHorizontalDependencyPathBuilder(GanttAppointmentViewInfoWrapper aptViewInfo, NearestDependencySearchResult dependencySearchResult, DependencyConnectorCalculator connectorCalculator)
			: base(aptViewInfo, dependencySearchResult.ViewInfo, connectorCalculator) {
			XtraSchedulerDebug.Assert(DependencyViewInfo.Direction == DependencyDirection.Horizontal);
			this.pointToConnect = dependencySearchResult.PointToConnect;
			XtraSchedulerDebug.Assert(DependencyViewInfo.GetPointProjection(pointToConnect) != Point.Empty);
		}
		#region Properties
		public Point PointToConnect { get { return pointToConnect; } }
		#endregion
		protected override int CalculatePathLength(PathItemCollection path) {
			Point pathEnd = path[path.Count - 1].End;
			return path.CalcPathLength() + DependencyViewInfo.CalcDistanceToPoint(pathEnd);
		}
		protected internal AppointmentToHorizontalDependencyPathStrategyBase CreateStrategy() {
			if (DependencyLayoutUtils.ShouldRoundAppointment(Connector, PointToConnect))
				return new AppointmentToHorizontalDependencyRoundPathStrategy(this);
			return new AppointmentToHorizontalDependencyNoRoundPathStrategy(this);
		}
		protected internal override List<PathItemCollection> CalculatePreliminaryPathCore(RoundElementType roundType) {
			AppointmentToHorizontalDependencyPathStrategyBase strategy = CreateStrategy();
			List<PathItemCollection> result = new List<PathItemCollection>();
			if (DependencyLayoutUtils.ContainsPointX(Connector.Start, Connector.End, DependencyViewInfo.End.X)) {
				if (Math.Abs(Connector.PointPath.Y - DependencyViewInfo.End.Y) > IntersectionObjectsInfo.DistanceFromDependency) {
					PathItemCollection simplePath = new PathItemCollection();
					Point startPoint = new Point(DependencyViewInfo.End.X, Connector.PointPath.Y);
					AddItemToPath(simplePath, new PathVerticalFixedStartItem(startPoint, DependencyViewInfo.End, IntersectionObjects, PathOwners));
					result.Add(simplePath);
				}
			}
			result.AddRange(strategy.BuildPaths(roundType));
			return result;
		}
	}
	#endregion
	#region AppointmentToHorizontalDependencyStrategyBase
	public abstract class AppointmentToHorizontalDependencyPathStrategyBase {
		readonly AppointmentToHorizontalDependencyPathBuilder pathBuilder;
		protected AppointmentToHorizontalDependencyPathStrategyBase(AppointmentToHorizontalDependencyPathBuilder pathBuilder) {
			this.pathBuilder = pathBuilder;
		}
		internal AppointmentToHorizontalDependencyPathBuilder PathBuilder { get { return pathBuilder; } }
		internal Point PointToConnect { get { return PathBuilder.PointToConnect; } }
		internal ConnectorItemBase Connector { get { return PathBuilder.Connector; } }
		internal Point PathStart { get { return PathBuilder.PathStart; } }
		internal GanttAppointmentViewInfoCollection PathOwners { get { return PathBuilder.PathOwners; } }
		internal Rectangle VisibleBounds { get { return PathBuilder.VisibleBounds; } }
		internal IntersectionObjectsInfo IntersectionObjects { get { return PathBuilder.IntersectionObjects; } }
		internal DependencyIntermediateViewInfo DependencyViewInfo { get { return PathBuilder.DependencyViewInfo; } }
		internal ConnectorPosition Position { get { return Connector.Position; } }
		public abstract List<PathItemCollection> BuildPaths(RoundElementType roundType);
		protected internal Rectangle CalculateAppointmentAvailableBounds(RoundElementType roundType) {
			int aptHorizontalBound = PathBuilder.CalculateAvailableHorizontalBoundCore(Connector, roundType);
			int aptLeft = Math.Min(Connector.PointPath.X, aptHorizontalBound);
			int aptRight = Math.Max(Connector.PointPath.X, aptHorizontalBound);
			return Rectangle.FromLTRB(aptLeft, VisibleBounds.Top, aptRight, VisibleBounds.Bottom);
		}
		protected internal Rectangle CalculateDependencyAvailableBounds(RoundElementType roundType) {
			Rectangle dependcyHorizontalBounds = CalculateAvailableHorizontalBounds(DependencyViewInfo, roundType);
			return Rectangle.FromLTRB(dependcyHorizontalBounds.Left, VisibleBounds.Top, dependcyHorizontalBounds.Right, VisibleBounds.Bottom);
		}
		protected internal virtual Rectangle CalculateAvailableHorizontalBounds(DependencyIntermediateViewInfo viewInfo, RoundElementType roundType) {
			XtraSchedulerDebug.Assert(viewInfo.Direction == DependencyDirection.Horizontal);
			int y = viewInfo.GeometricStart.Y;
			Rectangle intersections = IntersectionObjects.CalculateIntersections(viewInfo.GeometricStart, new Point(VisibleBounds.Left, y), PathOwners, roundType);
			int left = (intersections == Rectangle.Empty) ? VisibleBounds.Left : intersections.Right;
			intersections = IntersectionObjects.CalculateIntersections(viewInfo.GeometricEnd, new Point(VisibleBounds.Right, y), PathOwners, roundType);
			int right = (intersections == Rectangle.Empty) ? VisibleBounds.Right : intersections.Left;
			return Rectangle.FromLTRB(left, 0, right, 0);
		}
		protected internal List<PathItemCollection> CreateInitialPathsCollection() {
			List<PathItemCollection> result = new List<PathItemCollection>();
			result.Add(new PathItemCollection());
			return result;
		}
		protected internal void AddItemToPaths(List<PathItemCollection> paths, PathItem item) {
			int count = paths.Count;
			for (int i = 0; i < count; i++)
				paths[i].Add(item.Clone());
		}
		protected internal void AddItemsToPaths(List<PathItemCollection> paths, PathItem item1, PathItem item2) {
			List<PathItemCollection> newCollection = new List<PathItemCollection>();
			int count = paths.Count;
			for (int i = 0; i < count; i++) {
				PathItemCollection currentPath = paths[i];
				PathItemCollection newPath = ClonePath(currentPath);
				currentPath.Add(item1.Clone());
				newPath.Add(item2.Clone());
				newCollection.Add(currentPath);
				newCollection.Add(newPath);
			}
			paths.Clear();
			paths.AddRange(newCollection);
		}
		protected internal PathItemCollection ClonePath(PathItemCollection path) {
			PathItemCollection result = new PathItemCollection();
			int count = path.Count;
			for (int i = 0; i < count; i++)
				result.Add(path[i].Clone());
			return result;
		}
		protected internal void AddHorizontalItemsToPaths(List<PathItemCollection> paths, Point start, Point end, Rectangle availableBounds) {
			PathHorizontalItem item1 = new PathHorizontalItem(start, end, IntersectionObjects, PathHorizontalItemType.MoveUp, availableBounds, PathOwners);
			PathHorizontalItem item2 = new PathHorizontalItem(start, end, IntersectionObjects, PathHorizontalItemType.MoveDown, availableBounds, PathOwners);
			AddItemsToPaths(paths, item1, item2);
		}
		protected internal void AddVerticalItemsToPaths(List<PathItemCollection> paths, Point start, Point end, Rectangle availableBounds) {
			PathVerticalItem item1 = new PathVerticalItem(start, end, IntersectionObjects, PathVerticalItemType.MoveLeft, availableBounds, PathOwners);
			PathVerticalItem item2 = new PathVerticalItem(start, end, IntersectionObjects, PathVerticalItemType.MoveRight, availableBounds, PathOwners);
			AddItemsToPaths(paths, item1, item2);
		}
	}
	#endregion
	#region AppointmentToHorizontalDependencyNoRoundPathStrategy
	public class AppointmentToHorizontalDependencyNoRoundPathStrategy : AppointmentToHorizontalDependencyPathStrategyBase {
		public AppointmentToHorizontalDependencyNoRoundPathStrategy(AppointmentToHorizontalDependencyPathBuilder pathBuilder)
			: base(pathBuilder) {
		}
		public override List<PathItemCollection> BuildPaths(RoundElementType roundType) {
			PathHorizontalFixedStartItem item = new PathHorizontalFixedStartItem(PathStart, new Point(PointToConnect.X, PathStart.Y), IntersectionObjects, PathOwners);
			Rectangle dependencyAvailableBounds = CalculateDependencyAvailableBounds(roundType);
			Rectangle aptAvailableBounds = CalculateAppointmentAvailableBounds(roundType);
			item.SkipValidation = true;
			if (RectUtils.ContainsX(dependencyAvailableBounds, item.End))
				return CalculateSimplePaths(item, dependencyAvailableBounds, aptAvailableBounds, roundType);
			return CalculateIntersectionPath(item, dependencyAvailableBounds, aptAvailableBounds, roundType);
		}
		internal List<PathItemCollection> CalculateSimplePaths(PathHorizontalFixedStartItem startItem, Rectangle dependencyAvailableBounds, Rectangle aptAvailableBounds, RoundElementType roundType) {
			PathItemCollection path1 = CreateInitialSimplePath(startItem, PathVerticalItemType.MoveLeft, dependencyAvailableBounds, aptAvailableBounds);
			PathItemCollection path2 = CreateInitialSimplePath(startItem, PathVerticalItemType.MoveRight, dependencyAvailableBounds, aptAvailableBounds);
			List<PathItemCollection> initialPaths = new List<PathItemCollection>();
			initialPaths.Add(path1);
			initialPaths.Add(path2);
			return PathBuilder.CalculateOptimalPaths(initialPaths, roundType);
		}
		protected internal virtual PathItemCollection CreateInitialSimplePath(PathHorizontalFixedStartItem startItem, PathVerticalItemType type, Rectangle dependencyAvailableBounds, Rectangle aptAvailableBounds) {
			Rectangle bounds = CalculateSimplePathAvailableBounds(dependencyAvailableBounds, aptAvailableBounds);
			Point vItemEnd = new Point(startItem.End.X, PointToConnect.Y);
			PathVerticalItem vItem = new PathVerticalItem(startItem.End, vItemEnd, IntersectionObjects, type, bounds, PathOwners);
			PathItemCollection result = new PathItemCollection();
			result.Add(startItem.Clone());
			result.Add(vItem);
			return result;
		}
		protected internal virtual Rectangle CalculateSimplePathAvailableBounds(Rectangle dependencyAvailableBounds, Rectangle aptAvailableBounds) {
			int top = Math.Min(PathStart.Y, PointToConnect.Y);
			int bottom = Math.Max(PathStart.Y, PointToConnect.Y);
			int left, right;
			if (Position == ConnectorPosition.Start) {
				left = Math.Max(dependencyAvailableBounds.Left, aptAvailableBounds.Left);
				right = Math.Min(PathStart.X, dependencyAvailableBounds.Right);
			} else {
				left = Math.Max(PathStart.X, dependencyAvailableBounds.Left);
				right = Math.Min(dependencyAvailableBounds.Right, aptAvailableBounds.Right);
			}
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal virtual List<PathItemCollection> CalculateIntersectionPath(PathHorizontalFixedStartItem startItem, Rectangle dependencyAvailableBounds, Rectangle aptAvailableBounds, RoundElementType roundType) {
			List<PathItemCollection> result = CreateInitialPathsCollection();
			AddItemToPaths(result, startItem);
			PathVerticalItemType vitemType = Position == ConnectorPosition.Finish ? PathVerticalItemType.MoveLeft : PathVerticalItemType.MoveRight;
			PathVerticalItem vItem1 = new PathVerticalItem(startItem.End, startItem.End, IntersectionObjects, vitemType, aptAvailableBounds, PathOwners); 
			AddItemToPaths(result, vItem1);
			Point hItemEnd = new Point(PointToConnect.X, vItem1.End.Y);
			AddHorizontalItemsToPaths(result, vItem1.End, hItemEnd, aptAvailableBounds); 
			AddVerticalItemsToPaths(result, hItemEnd, PointToConnect, dependencyAvailableBounds);
			return PathBuilder.CalculateOptimalPaths(result, roundType);
		}
	}
	#endregion
	#region AppointmentToHorizontalDependencyRoundPathStrategy
	public class AppointmentToHorizontalDependencyRoundPathStrategy : AppointmentToHorizontalDependencyPathStrategyBase {
		public AppointmentToHorizontalDependencyRoundPathStrategy(AppointmentToHorizontalDependencyPathBuilder pathBuilder)
			: base(pathBuilder) {
		}
		#region Properties
		internal int MiddleY { get { return (PathStart.Y + PointToConnect.Y) / 2; } }
		#endregion
		public override List<PathItemCollection> BuildPaths(RoundElementType roundType) {
			XtraSchedulerDebug.Assert(PathStart.X > DependencyViewInfo.GeometricEnd.X || PathStart.X < DependencyViewInfo.GeometricEnd.X);
			List<PathItemCollection> initPaths = CreateInitialPathsCollection();
			PathHorizontalFixedStartItem itemStart = new PathHorizontalFixedStartItem(PathStart, PathStart, IntersectionObjects, PathOwners, true);
			AddItemToPaths(initPaths, itemStart);
			Rectangle dependencyAvailableBounds = CalculateDependencyAvailableBounds(roundType);
			Rectangle aptAvailableBounds = CalculateAppointmentAvailableBounds(roundType);
			AddVerticalItem1(initPaths, aptAvailableBounds);
			AddHorizontalItem(initPaths, aptAvailableBounds);
			AddVerticalItem2(initPaths, dependencyAvailableBounds);
			return PathBuilder.CalculateOptimalPaths(initPaths, roundType);
		}
		protected internal void AddVerticalItem1(List<PathItemCollection> initPaths, Rectangle aptAvailableBounds) {
			Point itemEnd = new Point(PathStart.X, MiddleY);
			PathVerticalItem vItem1;
			if (PathStart.X > PointToConnect.X)
				vItem1 = new PathVerticalItem(PathStart, itemEnd, IntersectionObjects, PathVerticalItemType.MoveRight, aptAvailableBounds, PathOwners);
			else
				vItem1 = new PathVerticalItem(PathStart, itemEnd, IntersectionObjects, PathVerticalItemType.MoveLeft, aptAvailableBounds, PathOwners);
			AddItemToPaths(initPaths, vItem1);
		}
		protected internal void AddVerticalItem2(List<PathItemCollection> initPaths, Rectangle dependencyAvailableBounds) {
			Point start = new Point(PointToConnect.X, MiddleY);
			AddVerticalItemsToPaths(initPaths, start, PointToConnect, dependencyAvailableBounds);
		}
		protected internal void AddHorizontalItem(List<PathItemCollection> initPaths, Rectangle aptAvailableBounds) {
			Point itemStart = new Point(PathStart.X, MiddleY);
			Point itemEnd = new Point(PointToConnect.X, MiddleY);
			AddHorizontalItemsToPaths(initPaths, itemStart, itemEnd, aptAvailableBounds);
		}
	}
	#endregion
	#region RectangleWorkaroundDirection
	public enum RectangleWorkaroundDirection {
		Clockwise,
		Counterclockwise
	}
	#endregion
	#region DirectedRectangle
	public class DirectedRectangle {
		public Rectangle Rectangle { get; set; }
		public RectangleWorkaroundDirection Direction { get; set; }
	}
	#endregion
	#region DirectedRectangleCalculator
	public class DirectedRectangleCalculator {
		readonly DependencyConnectionPointInfo endPointInfo;
		readonly Point startPoint;
		readonly int connectorLength;
		public DirectedRectangleCalculator(Point startPoint, DependencyConnectionPointInfo endPointInfo, int connectorLength) {
			Guard.ArgumentNotNull(endPointInfo, "pointInfo");
			this.startPoint = startPoint;
			this.endPointInfo = endPointInfo;
			this.connectorLength = connectorLength;
		}
		public DependencyConnectionPointInfo EndPointInfo { get { return endPointInfo; } }
		public Point StartPoint { get { return startPoint; } }
		public Point EndPoint { get { return EndPointInfo.Point; } }
		public int ConnectorLength { get { return connectorLength; } }
		protected internal DirectedRectangle CalculateBounds() {
			Rectangle bounds = CalculateRectangleFromPoint(StartPoint, EndPoint);
			return FitBoundsToAvailableDirection(bounds, EndPointInfo.AvailableDirections);
		}
		Rectangle CalculateRectangleFromPoint(Point StartPoint, Point EndPoint) {
			Rectangle bounds = new Rectangle();
			bounds.X = Math.Min(StartPoint.X, EndPoint.X);
			bounds.Y = Math.Min(StartPoint.Y, EndPoint.Y);
			bounds.Width = Math.Abs(StartPoint.X - EndPoint.X);
			bounds.Height = Math.Abs(StartPoint.Y - EndPoint.Y);
			return bounds;
		}
		DirectedRectangle FitBoundsToAvailableDirection(Rectangle bounds, DependencyConnectionDirectionType availableDirections) {
			DependencyConnectionDirectionType expandToDirection = DependencyConnectionDirectionType.None;
			RectangleWorkaroundDirection direction = RectangleWorkaroundDirection.Clockwise;
			if ((availableDirections & DependencyConnectionDirectionType.Top) != 0) {
				if (StartPoint.Y >= EndPoint.Y) {
					expandToDirection |= DependencyConnectionDirectionType.Top;
					if (StartPoint.X == EndPoint.X)
						expandToDirection |= DependencyConnectionDirectionType.Left;
				}
				if (StartPoint.X > EndPoint.X)
					direction = RectangleWorkaroundDirection.Counterclockwise;
			} else if ((availableDirections & DependencyConnectionDirectionType.Bottom) != 0) {
				if (StartPoint.Y <= EndPoint.Y) {
					expandToDirection |= DependencyConnectionDirectionType.Bottom;
					if (StartPoint.X == EndPoint.X)
						expandToDirection |= DependencyConnectionDirectionType.Left;
				}
				if (StartPoint.X <= EndPoint.X)
					direction = RectangleWorkaroundDirection.Counterclockwise;
			}
			if ((availableDirections & DependencyConnectionDirectionType.Left) != 0) {
				if (StartPoint.X >= EndPoint.X) {
					expandToDirection |= DependencyConnectionDirectionType.Left;
					if (StartPoint.Y == EndPoint.Y)
						expandToDirection |= DependencyConnectionDirectionType.Top;
				}
				if (StartPoint.Y <= EndPoint.Y)
					direction = RectangleWorkaroundDirection.Counterclockwise;
			} else if ((availableDirections & DependencyConnectionDirectionType.Right) != 0) {
				if (StartPoint.X <= EndPoint.X) {
					expandToDirection |= DependencyConnectionDirectionType.Right;
					if (StartPoint.Y == EndPoint.Y)
						expandToDirection |= DependencyConnectionDirectionType.Top;
				}
				if (StartPoint.Y > EndPoint.Y)
					direction = RectangleWorkaroundDirection.Counterclockwise;
			}
			return CreateDirctedRectangle(bounds, direction, expandToDirection);
		}
		DirectedRectangle CreateDirctedRectangle(Rectangle bounds, RectangleWorkaroundDirection direction, DependencyConnectionDirectionType expandToDirection) {
			DirectedRectangle result = new DirectedRectangle();
			result.Direction = direction;
			result.Rectangle = ValidateBounds(bounds);
			return result;
		}
		Rectangle ValidateBounds(Rectangle bounds) {
			if (bounds.Width != 0 && bounds.Width < ConnectorLength) {
				int delta = ConnectorLength - bounds.Width;
				if (StartPoint.X < EndPoint.X)
					bounds = RectUtils.ExpandToLeft(bounds, delta);
				else
					bounds = RectUtils.ExpandToRight(bounds, delta);
			} else if (bounds.Height != 0 && bounds.Height < ConnectorLength) {
				int delta = ConnectorLength - bounds.Height;
				if (StartPoint.Y < EndPoint.Y)
					bounds = RectUtils.ExpandToTop(bounds, delta);
				else
					bounds = RectUtils.ExpandToBottom(bounds, delta);
			}
			return bounds;
		}
	}
	#endregion
	#region RectangularPathBuilder
	public class RectangularPathBuilder {
		readonly IntersectionObjectsInfo intersection;
		readonly GanttAppointmentViewInfoCollection pathOwners;
		public RectangularPathBuilder(IntersectionObjectsInfo intersection, GanttAppointmentViewInfoCollection pathOwners) {
			Guard.ArgumentNotNull(intersection, "intersection");
			Guard.ArgumentNotNull(pathOwners, "pathOwners");
			this.intersection = intersection;
			this.pathOwners = pathOwners;
		}
		public IntersectionObjectsInfo Intersection { get { return intersection; } }
		public GanttAppointmentViewInfoCollection PathOwners { get { return pathOwners; } }
		public PathItemCollection Build(Point startPoint, DependencyConnectionPointInfo endPointInfo, int connectorLength, bool skipValidation) {
			DirectedRectangleCalculator calculator = new DirectedRectangleCalculator(startPoint, endPointInfo, connectorLength);
			DirectedRectangle directedBounds = calculator.CalculateBounds();
			DirectedRectanglePathBuilderBase pathBuilder = CreatePathBuilder(directedBounds, startPoint, endPointInfo);
			return pathBuilder.BuildPath(Intersection, PathOwners, skipValidation);
		}
		public PathItemCollection Build(Point startPoint, DependencyConnectionPointInfo endPointInfo, int connectorLength) {
			return Build(startPoint, endPointInfo, connectorLength, true);
		}
		DirectedRectanglePathBuilderBase CreatePathBuilder(DirectedRectangle directedBounds, Point startPoint, DependencyConnectionPointInfo endPointInfo) {
			if (directedBounds.Direction == RectangleWorkaroundDirection.Clockwise)
				return new ClockwiseDirectedRectanglePathBuilder(startPoint, endPointInfo, directedBounds);
			return new CounterclockwiseDirectedRectanglePathBuilder(startPoint, endPointInfo, directedBounds);
		}
	}
	#endregion
	#region ClockwiseDirectedRectanglePathBuilder
	public class ClockwiseDirectedRectanglePathBuilder : DirectedRectanglePathBuilderBase {
		public ClockwiseDirectedRectanglePathBuilder(Point startPoint, DependencyConnectionPointInfo endPointInfo, DirectedRectangle directedBounds)
			: base(endPointInfo, startPoint, directedBounds) {
		}
		protected internal override int CalcNextIndex(Point point, DirectedRectangle bounds) {
			if (point.X == bounds.Rectangle.X) {
				if (point.Y == bounds.Rectangle.Y)
					return 1;
				else if (point.Y == bounds.Rectangle.Bottom)
					return 0;
				return 0;
			}
			if (point.X == bounds.Rectangle.Right) {
				if (point.Y == bounds.Rectangle.Y)
					return 2;
				else if (point.Y == bounds.Rectangle.Bottom)
					return 3;
				return 2;
			}
			if (point.Y == bounds.Rectangle.Y)
				return 1;
			if (point.Y == bounds.Rectangle.Bottom)
				return 3;
			return -1;
		}
		protected internal override int CalcNextIndex(int currentIndex) {
			return (currentIndex + 1) % 4;
		}
	}
	#endregion
	#region CounterclockwiseDirectedRectanglePathBuilder
	public class CounterclockwiseDirectedRectanglePathBuilder : DirectedRectanglePathBuilderBase {
		public CounterclockwiseDirectedRectanglePathBuilder(Point startPoint, DependencyConnectionPointInfo endPointInfo, DirectedRectangle directedBounds)
			: base(endPointInfo, startPoint, directedBounds) {
		}
		protected internal override int CalcNextIndex(Point point, DirectedRectangle bounds) {
			if (point.X == bounds.Rectangle.X) {
				if (point.Y == bounds.Rectangle.Y)
					return 3;
				else if (point.Y == bounds.Rectangle.Bottom)
					return 2;
				return 3;
			}
			if (point.X == bounds.Rectangle.Right) {
				if (point.Y == bounds.Rectangle.Y)
					return 0;
				else if (point.Y == bounds.Rectangle.Bottom)
					return 1;
				return 1;
			}
			if (point.Y == bounds.Rectangle.Y)
				return 0;
			if (point.Y == bounds.Rectangle.Bottom)
				return 2;
			return -1;
		}
		protected internal override int CalcNextIndex(int currentIndex) {
			int result = currentIndex - 1;
			if (result < 0)
				result = 3;
			return result;
		}
	}
	#endregion
	#region DirectedRectanglePathBuilderBase
	public abstract class DirectedRectanglePathBuilderBase {
		readonly DirectedRectangle directedBounds;
		readonly DependencyConnectionPointInfo endPointInfo;
		readonly Point startPoint;
		int lastPointIndex;
		protected DirectedRectanglePathBuilderBase(DependencyConnectionPointInfo endPointInfo, Point startPoint, DirectedRectangle directedBounds) {
			Guard.ArgumentNotNull(directedBounds, "directedBounds");
			this.directedBounds = directedBounds;
			this.startPoint = startPoint;
			this.endPointInfo = endPointInfo;
			this.lastPointIndex = CalcNextIndex(EndPoint, DirectedBounds);
		}
		public DirectedRectangle DirectedBounds { get { return directedBounds; } }
		public DependencyConnectionPointInfo EndPointInfo { get { return endPointInfo; } }
		public Point StartPoint { get { return startPoint; } }
		public Point EndPoint { get { return EndPointInfo.Point; } }
		public PathItemCollection BuildPath(IntersectionObjectsInfo intersection, GanttAppointmentViewInfoCollection pathOwners) {
			return BuildPath(intersection, pathOwners);
		}
		public PathItemCollection BuildPath(IntersectionObjectsInfo intersection, GanttAppointmentViewInfoCollection pathOwners, bool skipValidation) {
			PathItemCollection path = new PathItemCollection();
			int currentPointIndex = CalcNextIndex(StartPoint, DirectedBounds);
			Point currentPoint = StartPoint;
			Point nextPoint = GetPoint(currentPointIndex, DirectedBounds);
			int pathItemCount = 0;
			while (pathItemCount < 5) {
				PathItemBase pathItem = null;
				if (nextPoint.Y == currentPoint.Y)
					pathItem = new PathHorizontalFixedStartItem(currentPoint, nextPoint, intersection, pathOwners, skipValidation);
				else
					pathItem = new PathVerticalFixedStartItem(currentPoint, nextPoint, intersection, pathOwners, skipValidation);
				path.Add(pathItem);
				if (nextPoint == EndPoint)
					break;
				currentPoint = nextPoint;
				currentPointIndex = CalcNextIndex(currentPointIndex);
				if (currentPointIndex == this.lastPointIndex)
					nextPoint = EndPoint;
				else
					nextPoint = GetPoint(currentPointIndex, DirectedBounds);
				pathItemCount++;
			}
			return path;
		}
		protected internal abstract int CalcNextIndex(Point point, DirectedRectangle bounds);
		protected internal abstract int CalcNextIndex(int currentIndex);
		Point GetPoint(int index, DirectedRectangle directedRectangle) {
			Rectangle bounds = directedRectangle.Rectangle;
			switch (index) {
				case 0:
					return new Point(bounds.X, bounds.Y);
				case 1:
					return new Point(bounds.Right, bounds.Y);
				case 2:
					return new Point(bounds.Right, bounds.Bottom);
				case 3:
					return new Point(bounds.X, bounds.Bottom);
			}
			return Point.Empty;
		}
	}
	#endregion
}

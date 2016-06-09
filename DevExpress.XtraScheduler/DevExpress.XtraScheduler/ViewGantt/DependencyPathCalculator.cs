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
using System.Windows.Forms;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Drawing {
	#region RoundElementType
	[Flags]
	public enum RoundElementType {
		None = 0x0,
		AppointmentVertical = 0x1,
		AppointmentHorizontal = 0x2,
		Dependency = 0x4,
		All = 0x7
	}
	#endregion
	public enum OwnerAppointmentsAction {
		Skip,
		Process
	}
	#region IntersectionsInfo
	public class IntersectionsInfo {
		RoundElementType intersectionType;
		int appointmentsIntersectionCount;
		int dependenciesIntersectionCount;
		public IntersectionsInfo(RoundElementType intersectionType, int appointmentsIntersectionCount, int dependenciesIntersectionCount) {
			this.intersectionType = intersectionType;
			this.appointmentsIntersectionCount = appointmentsIntersectionCount;
			this.dependenciesIntersectionCount = dependenciesIntersectionCount;
		}
		public RoundElementType IntersectionType { get { return intersectionType; } }
		public int AppointmentsIntersectionCount { get { return appointmentsIntersectionCount; } }
		public int DependenciesIntersectionCount { get { return dependenciesIntersectionCount; } }
	}
	#endregion
	#region IntersectionObjectsInfo
	public class IntersectionObjectsInfo {
		#region Fields
		public const int DistanceFromDependency = 3;
		GanttAppointmentViewInfoCollection visibleAppointments;
		DependencyIntermediateViewInfoCollection dependencyViewInfos;
		int distanceFromAppointment;
		#endregion
		public IntersectionObjectsInfo(GanttAppointmentViewInfoCollection appointments, DependencyIntermediateViewInfoCollection dependencyViewInfos, int distanceFromAppointment) {
			this.distanceFromAppointment = distanceFromAppointment;
			this.visibleAppointments = new GanttAppointmentViewInfoCollection();
			this.visibleAppointments.AddRange(appointments);
			this.dependencyViewInfos = new DependencyIntermediateViewInfoCollection();
			this.dependencyViewInfos.AddRange(dependencyViewInfos);
		}
		#region Properties
		internal GanttAppointmentViewInfoCollection VisibleAppointments { get { return visibleAppointments; } }
		internal DependencyIntermediateViewInfoCollection DependencyViewInfos { get { return dependencyViewInfos; } }
		protected internal int DistanceFromAppointment { get { return distanceFromAppointment; } }
		#endregion
		public IntersectionsInfo CalculateIntersectionsType(Point start, Point end, RoundElementType searchPattern) {
			RoundElementType intersectionType = RoundElementType.None;
			int appointmentsIntersectionCount = 0;
			int dependenciesIntersectionCount = 0;
			XtraSchedulerDebug.Assert(start.Y == end.Y || start.X == end.X);
			if (ShouldCalculateAppointmentsIntersection(start, end, searchPattern)) {
				IntersectionsInfo appointmentsIntersectionsInfo = CalculateAppointmentsIntersectionType(start, end);
				intersectionType |= appointmentsIntersectionsInfo.IntersectionType;
				appointmentsIntersectionCount = appointmentsIntersectionsInfo.AppointmentsIntersectionCount;
			}
			if (ShouldCalculateDependenciesIntersection(searchPattern)) {
				IntersectionsInfo dependenciesIntersectionsInfo = CalculateDependenciesIntersectionType(start, end);
				intersectionType |= dependenciesIntersectionsInfo.IntersectionType;
				dependenciesIntersectionCount = dependenciesIntersectionsInfo.DependenciesIntersectionCount;
			}
			return new IntersectionsInfo(intersectionType, appointmentsIntersectionCount, dependenciesIntersectionCount);
		}
		public Rectangle CalculateIntersections(Point start, Point end, GanttAppointmentViewInfoCollection pathOwners, RoundElementType roundType) {
			return CalculateIntersectionsCore(start, end, pathOwners, roundType, OwnerAppointmentsAction.Process);
		}
		public Rectangle CalculateConnectorIntersections(Point start, Point end, IGanttAppointmentViewInfo owner) {
			GanttAppointmentViewInfoCollection owners = new GanttAppointmentViewInfoCollection();
			owners.Add(owner);
			return CalculateIntersectionsCore(start, end, owners, RoundElementType.All, OwnerAppointmentsAction.Skip);
		}
		public Rectangle CalculateIntersectionsCore(Point start, Point end, GanttAppointmentViewInfoCollection pathOwners, RoundElementType roundType, OwnerAppointmentsAction action) {
			XtraSchedulerDebug.Assert(start.Y == end.Y || start.X == end.X);
			Rectangle result = CalculateAppointmentsIntersection(start, end, pathOwners, roundType, action);
			if (ShouldCalculateDependenciesIntersection(roundType)) {
				Rectangle dependencyIntersectRectangle = CalculateDependenciesIntersection(start, end);
				result = RectUtils.UnionNonEmpty(result, dependencyIntersectRectangle);
			}
			return CropRectangleByPoints(result, start, end);
		}
		protected internal IntersectionsInfo CalculateAppointmentsIntersectionType(Point start, Point end) {
			int count = VisibleAppointments.Count;
			RoundElementType intersectionType = RoundElementType.None;
			int appointmentsIntersectionCount = 0;
			for (int i = 0; i < count; i++) {
				Rectangle aptRectangle = CreateRectangleFromAppointmentBoundsFromIntersections(VisibleAppointments[i].Bounds);
				if (!DependencyLayoutUtils.IsIntersectsRectangle(aptRectangle, start, end))
					continue;
				intersectionType |= start.Y == end.Y ? RoundElementType.AppointmentVertical : RoundElementType.AppointmentHorizontal;
				appointmentsIntersectionCount++;
			}
			return new IntersectionsInfo(intersectionType, appointmentsIntersectionCount, 0);
		}
		protected internal IntersectionsInfo CalculateDependenciesIntersectionType(Point start, Point end) {
			int count = DependencyViewInfos.Count;
			RoundElementType intersectionType = RoundElementType.None;
			int dependenciesIntersectionCount = 0;
			for (int i = 0; i < count; i++) {
				DependencyIntermediateViewInfo current = DependencyViewInfos[i];
				if (!IsOverlayedWithDependency(current, start, end))
					continue;
				intersectionType = RoundElementType.Dependency;
				dependenciesIntersectionCount++;
			}
			return new IntersectionsInfo(intersectionType, 0, dependenciesIntersectionCount);
		}
		protected internal bool ShouldCalculateAppointmentsIntersection(Point start, Point end, RoundElementType roundType) {
			if (start.Y == end.Y && (roundType & RoundElementType.AppointmentHorizontal) != 0)
				return true;
			if (start.X == end.X && (roundType & RoundElementType.AppointmentVertical) != 0)
				return true;
			return false;
		}
		protected internal bool ShouldCalculateDependenciesIntersection(RoundElementType roundType) {
			return (roundType & RoundElementType.Dependency) != 0;
		}
		protected internal bool ShouldCalculateAppointmentIntersection(Point start, Point end, bool isPathOwner, RoundElementType roundType, OwnerAppointmentsAction action) {
			if (isPathOwner)
				return action == OwnerAppointmentsAction.Skip ? false : true;
			return ShouldCalculateAppointmentsIntersection(start, end, roundType);
		}
		internal Rectangle CalculateAppointmentsIntersection(Point start, Point end, GanttAppointmentViewInfoCollection pathOwners, RoundElementType roundType, OwnerAppointmentsAction action) {
			Rectangle result = Rectangle.Empty;
			int count = VisibleAppointments.Count;
			for (int i = 0; i < count; i++) {
				IGanttAppointmentViewInfo current = VisibleAppointments[i];
				if (!ShouldCalculateAppointmentIntersection(start, end, pathOwners.Contains(current), roundType, action))
					continue;
				Padding correctionPadding = CalculateNeighborAppointmentsPaddingCorrection(pathOwners, current);
				Rectangle aptRectangle = CreateRectangleFromAppointmentBounds(current.Bounds, correctionPadding);
				if (DependencyLayoutUtils.IsIntersectsRectangle(aptRectangle, start, end))
					result = RectUtils.UnionNonEmpty(result, aptRectangle);
			}
			return result;
		}
		internal Rectangle CalculateDependenciesIntersection(Point start, Point end) {
			Rectangle result = Rectangle.Empty;
			int count = DependencyViewInfos.Count;
			for (int i = 0; i < count; i++) {
				DependencyIntermediateViewInfo current = DependencyViewInfos[i];
				if (IsOverlayedWithDependency(current, start, end)) {
					result = RectUtils.UnionNonEmpty(result, CreateRectangleFromDependencyViewInfo(current));
				}
			}
			return result;
		}
		internal bool IsOverlayedWithDependency(DependencyIntermediateViewInfo dependency, Point start, Point end) {
			if (start.X == end.X && dependency.Direction == DependencyDirection.Horizontal)
				return false;
			if (start.Y == end.Y && dependency.Direction == DependencyDirection.Vertical)
				return false;
			Rectangle dependencyRect = CreateRectangleFromDependencyViewInfo(dependency);
			return DependencyLayoutUtils.IsIntersectsRectangle(dependencyRect, start, end);
		}
		internal Rectangle CreateRectangleFromAppointmentBounds(Rectangle aptBounds, Padding padding) {
			aptBounds.Inflate(DistanceFromAppointment, DistanceFromAppointment);
			aptBounds.X += padding.Left;
			aptBounds.Width -= padding.Left + padding.Right;
			return aptBounds;
		}
		internal Rectangle CreateRectangleFromAppointmentBounds(Rectangle aptBounds) {
			aptBounds.Inflate(DistanceFromAppointment, DistanceFromAppointment);
			return aptBounds;
		}
		internal Rectangle CreateRectangleFromAppointmentBoundsFromIntersections(Rectangle aptBounds) {
			aptBounds.Inflate(DistanceFromDependency, DistanceFromDependency);
			return aptBounds;
		}
		internal Rectangle CreateRectangleFromDependencyViewInfo(DependencyIntermediateViewInfo info) {
			Rectangle bounds = Rectangle.FromLTRB(info.GeometricStart.X, info.GeometricStart.Y, info.GeometricEnd.X, info.GeometricEnd.Y);
			int width = (info.Direction == DependencyDirection.Horizontal) ? 0 : DistanceFromDependency;
			int height = (info.Direction == DependencyDirection.Vertical) ? 0 : DistanceFromDependency;
			bounds.Inflate(width, height);
			return bounds;
		}
		internal Rectangle CropRectangleByPoints(Rectangle rect, Point start, Point end) {
			if (rect == Rectangle.Empty)
				return rect;
			if (start.Y == end.Y)
				return CropRectangleHorizontally(rect, start, end);
			return CropRectangleVertically(rect, start, end);
		}
		Padding CalculateNeighborAppointmentsPaddingCorrection(GanttAppointmentViewInfoCollection pathOwners, IGanttAppointmentViewInfo current) {
			Padding result = new Padding();
			if (pathOwners.Contains(current))
				return result;
			int count = pathOwners.Count;
			for (int i = 0; i < count; i++) {
				Rectangle ownerBounds = pathOwners[i].Bounds;
				Rectangle currentBounds = current.Bounds;
				if (!(currentBounds.Bottom > ownerBounds.Top || currentBounds.Top < ownerBounds.Bottom))
					continue;
				int horizontalDistance = Math.Abs(ownerBounds.Left - currentBounds.Right);
				if (horizontalDistance < DistanceFromAppointment) {
					result.Right = DistanceFromAppointment - horizontalDistance;
				}
				horizontalDistance = Math.Abs(ownerBounds.Right - currentBounds.Left);
				if (horizontalDistance < DistanceFromAppointment)
					result.Left = DistanceFromAppointment - horizontalDistance;
				break;
			}
			return result;
		}
		Rectangle CropRectangleHorizontally(Rectangle rect, Point start, Point end) {
			int left = Math.Min(start.X, end.X);
			int right = Math.Max(start.X, end.X);
			if (left > rect.Left)
				rect = RectUtils.CutFromLeft(rect, left - rect.Left);
			if (right < rect.Right)
				rect = RectUtils.CutFromRight(rect, rect.Right - right);
			return rect;
		}
		Rectangle CropRectangleVertically(Rectangle rect, Point start, Point end) {
			int top = Math.Min(start.Y, end.Y);
			int bottom = Math.Max(start.Y, end.Y);
			if (top > rect.Top)
				rect = RectUtils.CutFromTop(rect, top - rect.Top);
			if (bottom < rect.Bottom)
				rect = RectUtils.CutFromBottom(rect, rect.Bottom - bottom);
			return rect;
		}
	}
	#endregion
}

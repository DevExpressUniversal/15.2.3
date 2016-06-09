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
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraScheduler.Native;
using System.ComponentModel;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using System.Threading;
namespace DevExpress.XtraScheduler.Drawing {
	#region PointStack
	public class PointStack {
		#region Fields
		Stack<Point> points;
		#endregion
		public PointStack() {
			this.points = new Stack<Point>();
		}
		#region Properties
		protected internal int Count { get { return points.Count; } }
		#endregion
		protected internal virtual void Push(Point item) {
			this.points.Push(item);
		}
		protected internal virtual Point PeekNext() {
			if (this.points.Count == 0)
				return Point.Empty;
			return this.points.Peek();
		}
		protected internal virtual void MoveNext() {
			if (this.points.Count > 1)
				this.points.Pop();
		}
		protected internal virtual Point GetNext() {
			if (this.points.Count == 1)
				return this.points.Peek();
			return this.points.Pop();
		}
		protected internal virtual Point GetNext(Point point, ConnectorLocation connectorLocation) {
			if (this.points.Count == 1)
				return this.points.Peek();
			Point result = this.points.Peek();
			Stack<Point> tempPoints = new Stack<Point>();
			while (this.points.Count > 0) {
				Point location = this.points.Pop();
				if (connectorLocation == ConnectorLocation.TopLeft && location.X - point.X > 0) {
					result = location;
					break;
				} else if (connectorLocation == ConnectorLocation.BottomRight && point.X - location.X > 0) {
					result = location;
					break;
				} else if (connectorLocation == ConnectorLocation.TopRight && point.X - location.X > 0) {
					result = location;
					break;
				}
				tempPoints.Push(location);
			}
			int count = tempPoints.Count;
			for (int i = 0; i < count; i++)
				this.points.Push(tempPoints.Pop());
			return result;
		}
	}
	#endregion
	#region ConnectionPointsInfo
	public class ConnectionPointsInfo {
		#region Fields
		PointStack topLeft;
		PointStack left;
		PointStack bottomLeft;
		PointStack topRight;
		PointStack right;
		PointStack bottomRight;
		#endregion
		public ConnectionPointsInfo() {
			topLeft = new PointStack();
			left = new PointStack();
			bottomLeft = new PointStack();
			topRight = new PointStack();
			right = new PointStack();
			bottomRight = new PointStack();
		}
		#region Properties
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ConnectionPointsInfoTopLeft")]
#endif
		public PointStack TopLeft { get { return topLeft; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ConnectionPointsInfoLeft")]
#endif
		public PointStack Left { get { return left; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ConnectionPointsInfoBottomLeft")]
#endif
		public PointStack BottomLeft { get { return bottomLeft; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ConnectionPointsInfoTopRight")]
#endif
		public PointStack TopRight { get { return topRight; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ConnectionPointsInfoRight")]
#endif
		public PointStack Right { get { return right; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ConnectionPointsInfoBottomRight")]
#endif
		public PointStack BottomRight { get { return bottomRight; } }
		#endregion
		public void ReturnNextPoint(ConnectorLocation location, Point point) {
			PointStack stack = GetStack(location);
			if (stack.Count == 0)
				return;
			stack.Push(point);
		}
		public Point GetNextPoint(ConnectorLocation location, Point point) {
			PointStack stack = GetStack(location);
			if (stack.Count == 0)
				return Point.Empty;
			return stack.GetNext(point, location);
		}
		protected internal Point GetNextPoint(ConnectorLocation location) {
			PointStack stack = GetStack(location);
			return stack.Count == 0 ? Point.Empty : stack.GetNext();
		}
		protected internal Point GetPoint(ConnectorLocation location) {
			PointStack stack = GetStack(location);
			return stack.Count == 0 ? Point.Empty : stack.PeekNext();
		}
		protected internal virtual PointStack GetStack(ConnectorLocation location) {
			switch (location) {
				case ConnectorLocation.TopLeft:
					return TopLeft;
				case ConnectorLocation.Left:
					return Left;
				case ConnectorLocation.BottomLeft:
					return BottomLeft;
				case ConnectorLocation.TopRight:
					return TopRight;
				case ConnectorLocation.Right:
					return Right;
				case ConnectorLocation.BottomRight:
					return BottomRight;
				default: {
						XtraSchedulerDebug.Assert(false);
						return Right;
					}
			}
		}
	}
	#endregion
	#region DependencyConnectionDirectionType
	[Flags]
	public enum DependencyConnectionDirectionType {
		None = 0,
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8,
		All = Left | Right | Top | Bottom
	}
	#endregion
	public class DependencyConnectionPointInfo {
		public Point Point { get; set; }
		public DependencyConnectionDirectionType AvailableDirections { get; set; }
	}
	#region DependencyViewInfo
	public class DependencyViewInfo : SelectableIntervalViewInfo, IAppointmentDependencyView, IDisposable {
		#region Fields
		const int selectionDelta = 2;
		AppearanceObject appearance;
		AppearanceObject selectedAppearance;
		Point start;
		Point end;
		Point lineStart;
		Point lineEnd;
		AppointmentDependencyCollection dependencies;
		GanttAppointmentViewInfoWrapper startGanttViewInfo;
		GanttAppointmentViewInfoWrapper endGanttViewInfo;
		DXCollection<DependencyViewInfo> relatedViewInfos;
		ViewInfoItemCollection items;
		DependencyCornerViewInfo startCorner;
		DependencyCornerViewInfo endCorner;
		bool isDisposed;
		bool lineVisible;
		#endregion
		#region Ctors
		public DependencyViewInfo() {
			this.appearance = new AppearanceObject();
			this.selectedAppearance = new AppearanceObject();
			this.items = new ViewInfoItemCollection();
			this.dependencies = new AppointmentDependencyCollection();
			this.relatedViewInfos = new DXCollection<DependencyViewInfo>();
			this.startCorner = new DependencyCornerViewInfo();
			this.endCorner = new DependencyCornerViewInfo();
			this.lineVisible = true;
		}
		public DependencyViewInfo(Point start, Point end)
			: this() {
			this.start = start;
			this.end = end;
			this.lineStart = start;
			this.lineEnd = end;
		}
		#endregion
		#region Public Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override TimeInterval Interval { get { return TimeInterval.Empty; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Resource Resource { get { return ResourceBase.Empty; } set { } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("DependencyViewInfoHitTestType")]
#endif
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.AppointmentDependency; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("DependencyViewInfoAppearance")]
#endif
		public AppearanceObject Appearance { get { return appearance; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("DependencyViewInfoSelectedAppearance")]
#endif
		public AppearanceObject SelectedAppearance { get { return selectedAppearance; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("DependencyViewInfoStart")]
#endif
		public Point Start { get { return start; } set { start = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("DependencyViewInfoEnd")]
#endif
		public Point End { get { return end; } set { end = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("DependencyViewInfoLineStart")]
#endif
		public Point LineStart { get { return lineStart; } set { lineStart = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("DependencyViewInfoLineEnd")]
#endif
		public Point LineEnd { get { return lineEnd; } set { lineEnd = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("DependencyViewInfoDependencies")]
#endif
		public AppointmentDependencyCollection Dependencies { get { return dependencies; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("DependencyViewInfoItems")]
#endif
		public ViewInfoItemCollection Items { get { return items; } }
		public AppointmentViewInfo StartAppointmentViewInfo { get { return StartGanttViewInfo != null ? (AppointmentViewInfo)startGanttViewInfo.ViewInfo : null; } }
		public AppointmentViewInfo EndAppointmentViewInfo { get { return EndGanttViewInfo != null ? (AppointmentViewInfo)endGanttViewInfo.ViewInfo : null; } }
		#endregion
		#region Internal Properties
		internal DependencyCornerViewInfo StartCorner { get { return startCorner; } set { startCorner = value; } }
		internal DependencyCornerViewInfo EndCorner { get { return endCorner; } set { endCorner = value; } }
		internal DependencyDirection Direction { get { return Start.Y == End.Y ? DependencyDirection.Horizontal : DependencyDirection.Vertical; } }
		internal GanttAppointmentViewInfoWrapper StartGanttViewInfo {
			get { return startGanttViewInfo; }
			set {
				startGanttViewInfo = value;
				if (startGanttViewInfo != null)
					XtraSchedulerDebug.Assert(startGanttViewInfo.IsParent);
			}
		}
		internal GanttAppointmentViewInfoWrapper EndGanttViewInfo {
			get { return endGanttViewInfo; }
			set {
				endGanttViewInfo = value;
				if (endGanttViewInfo != null)
					XtraSchedulerDebug.Assert(!endGanttViewInfo.IsParent);
			}
		}
		internal DXCollection<DependencyViewInfo> RelatedViewInfos { get { return relatedViewInfos; } }
		internal bool IsDisposed { get { return isDisposed; } }
		internal bool LineVisible { get { return lineVisible; } set { lineVisible = value; } }
		#endregion
		#region IAppointmentDependencyView
		AppointmentDependencyCollection IAppointmentDependencyView.Dependencies { get { return Dependencies; } }
		#endregion
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DependencyViewInfo() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (appearance != null) {
					appearance.Dispose();
					appearance = null;
				}
				if (selectedAppearance != null) {
					selectedAppearance.Dispose();
					selectedAppearance = null;
				}
				if (items != null) {
					DisposeItems();
					items = null;
				}
			}
			this.isDisposed = true;
		}
		protected internal virtual void DisposeItems() {
			int count = items.Count;
			for (int i = 0; i < count; i++)
				DisposeItemCore(items[i]);
		}
		protected internal virtual void DisposeItemCore(ViewInfoItem item) {
			if (item != null)
				item.Dispose();
		}
		#endregion
		public override string ToString() {
			return String.Format("{0} - {1}", Start.ToString(), End.ToString());
		}
		protected internal virtual void CalculateBounds() {
			if (Start.X == End.X)
				Bounds = CalculateVerticalBounds();
			else
				Bounds = CalculateHorizontalBounds();
		}
		protected internal virtual Rectangle CalculateVerticalBounds() {
			int top = Math.Min(Start.Y, End.Y);
			int bottom = Math.Max(Start.Y, End.Y);
			return new Rectangle(Start.X - selectionDelta, top, selectionDelta * 2, bottom - top + 1);
		}
		protected internal virtual Rectangle CalculateHorizontalBounds() {
			int left = Math.Min(Start.X, End.X);
			int right = Math.Max(Start.X, End.X);
			return new Rectangle(left, Start.Y - selectionDelta, right - left + 1, selectionDelta * 2);
		}
		protected internal virtual int GetLineLenght() {
			if (LineStart.X == LineEnd.X)
				return Math.Abs(LineStart.Y - LineEnd.Y) + 1;
			return Math.Abs(LineStart.X - LineEnd.X) + 1;
		}
		protected internal virtual DependencyDirection GetDirection() {
			return Start.X == End.X ? DependencyDirection.Vertical : DependencyDirection.Horizontal;
		}
	}
	#endregion
	#region DependencyViewInfoCollection
	public class DependencyViewInfoCollection : List<DependencyViewInfo> {
		public DependencyViewInfoCollection() {
		}
		public DependencyViewInfoCollection(int capacity)
			: base(capacity) {
		}
		public DependencyViewInfoCollection(IEnumerable<DependencyViewInfo> collection)
			: base(collection) {
		}
		protected internal virtual void CalculateBounds(CancellationToken token) {
			for (int i = 0; i < Count; i++) {
				if (token.IsCancellationRequested)
					return;
				this[i].CalculateBounds();
			}
		}
	}
	#endregion
	#region DependencyViewInfoDirection
	public enum DependencyDirection {
		Horizontal,
		Vertical
	}
	#endregion
	#region DependencyCornerType
	public enum DependencyCornerType {
		None,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}
	#endregion
	#region ConnectorPosition
	public enum ConnectorPosition {
		Start,
		Finish
	}
	#endregion
	#region ConnectorLocation
	public enum ConnectorLocation {
		TopLeft,
		Left,
		BottomLeft,
		TopRight,
		Right,
		BottomRight,
		None
	}
	#endregion
	#region DependencyIntermediateViewInfo
	public class DependencyIntermediateViewInfo {
		#region Fields
		DependencyViewInfoCollection dependencyViewInfos;
		ConnectionPointCalculatorBase connectionPointCalculator;
		#endregion
		public DependencyIntermediateViewInfo(DependencyViewInfo info) {
			Guard.ArgumentNotNull(info, "info");
			this.dependencyViewInfos = new DependencyViewInfoCollection();
			this.dependencyViewInfos.Add(info);
			this.connectionPointCalculator = CreateConnectionPointCalculator();
		}
		#region Properties
		public Point Start { get { return dependencyViewInfos[0].Start; } }
		public Point End { get { return dependencyViewInfos[dependencyViewInfos.Count - 1].End; } }
		public Point GeometricStart {
			get {
				return (Start.X < End.X || Start.Y < End.Y) ? Start : End;
			}
		}
		public Point GeometricEnd {
			get {
				return (Start.X < End.X || Start.Y < End.Y) ? End : Start;
			}
		}
		internal DependencyViewInfoCollection DependencyViewInfos { get { return dependencyViewInfos; } }
		protected internal DependencyDirection Direction { get { return Start.X == End.X ? DependencyDirection.Vertical : DependencyDirection.Horizontal; } }
		internal ConnectionPointCalculatorBase ConnectionPointCalculator { get { return connectionPointCalculator; } }
		#endregion
		public DependencyConnectionPointInfo GetPointToConnect(ConnectorItemBase aptConnector, DependencyIntermediateViewInfo prev, DependencyIntermediateViewInfo next) {
			if (aptConnector.PointAppointment == Start || aptConnector.PointAppointment == End) {
				DependencyConnectionPointInfo connectionPointInfo = new DependencyConnectionPointInfo();
				connectionPointInfo.Point = aptConnector.PointPath;
				connectionPointInfo.AvailableDirections = DependencyConnectionDirectionType.All;
				return connectionPointInfo;
			}
			return ConnectionPointCalculator.Calculate(aptConnector, prev, next);
		}
		public Point GetPointProjection(Point pt) {
			if (Direction == DependencyDirection.Vertical)
				return GetPointProjectionVertical(pt);
			return GetPointProjectionHorizontal(pt);
		}
		public void AddConnectionPoint(Point pt) {
			if (Direction == DependencyDirection.Horizontal)
				XtraSchedulerDebug.Assert(pt.Y == Start.Y);
			else
				XtraSchedulerDebug.Assert(pt.X == Start.X);
			Point projection = GetPointProjection(pt);
			if (projection == Point.Empty)
				AddConnectionPointCore(pt);
			else
				SplitDependencyViewInfo(pt);
		}
		public override string ToString() {
			return String.Format("{0} - {1}", Start.ToString(), End.ToString());
		}
		protected internal ConnectionPointCalculatorBase CreateConnectionPointCalculator() {
			if (Direction == DependencyDirection.Horizontal)
				return new HorizontalDependencyConnectionPointCalculator(this);
			return new VerticalDependencyConnectionPointCalculator(this);
		}
		protected internal Point GetPointProjectionVertical(Point pt) {
			if ((GeometricStart.Y <= pt.Y) && (pt.Y <= GeometricEnd.Y))
				return new Point(Start.X, pt.Y);
			return Point.Empty;
		}
		protected internal Point GetPointProjectionHorizontal(Point pt) {
			if ((GeometricStart.X <= pt.X) && (pt.X <= GeometricEnd.X))
				return new Point(pt.X, Start.Y);
			return Point.Empty;
		}
		protected internal int CalcDistanceToPoint(Point pt) {
			Point projection = GetPointProjection(pt);
			if (projection != Point.Empty)
				return 0;
			return CalcDistanceToPointCore(pt);
		}
		void AddConnectionPointCore(Point pt) {
			if (ShouldInsertBeforeStart(pt))
				DependencyViewInfos.Insert(0, new DependencyViewInfo(pt, Start));
			else
				DependencyViewInfos.Add(new DependencyViewInfo(End, pt));
		}
		internal bool ShouldInsertBeforeStart(Point pt) {
			if ((Start.X < End.X || Start.Y < End.Y) && (pt.X < Start.X || pt.Y < Start.Y))
				return true;
			if ((Start.X > End.X || Start.Y > End.Y) && (pt.X > End.X || pt.Y > End.Y))
				return true;
			return false;
		}
		void SplitDependencyViewInfo(Point pt) {
			DependencyViewInfo info = GetDependencyViewInfoByPoint(pt);
#if DEBUG || DEBUGTEST
			Guard.ArgumentNotNull(info, "info");
#endif
			if (info.Start == pt || info.End == pt)
				return;
			SplitDependencyViewInfoCore(pt, info);
		}
		void SplitDependencyViewInfoCore(Point pt, DependencyViewInfo info) {
			DependencyViewInfo splittedInfo = new DependencyViewInfo(pt, info.End);
			info.End = pt;
			splittedInfo.EndGanttViewInfo = info.EndGanttViewInfo;
			info.EndGanttViewInfo = null;
			DependencyViewInfos.Insert(DependencyViewInfos.IndexOf(info) + 1, splittedInfo);
		}
		DependencyViewInfo GetDependencyViewInfoByPoint(Point pt) {
			int count = DependencyViewInfos.Count;
			for (int i = 0; i < count; i++) {
				DependencyViewInfo current = DependencyViewInfos[i];
				if (IsPointCorrespondsToViewInfo(current, pt))
					return current;
			}
			return null;
		}
		bool IsPointCorrespondsToViewInfo(DependencyViewInfo info, Point pt) {
			if (Direction == DependencyDirection.Horizontal)
				return DependencyLayoutUtils.ContainsPointX(info.Start, info.End, pt);
			return DependencyLayoutUtils.ContainsPointY(info.Start, info.End, pt);
		}
		int CalcDistanceToPointCore(Point pt) {
			if (pt.X < GeometricStart.X || pt.Y < GeometricStart.Y)
				return (GeometricStart.X - pt.X) + (GeometricStart.Y - pt.Y);
			return (pt.X - GeometricEnd.X) + (pt.Y - GeometricEnd.Y);
		}
	}
	#endregion
	#region DependencyIntermediateViewInfoCollection
	public class DependencyIntermediateViewInfoCollection : DXCollection<DependencyIntermediateViewInfo> {
	}
	#endregion
	#region ConnectionPointCalculatorBase (abstract class)
	public abstract class ConnectionPointCalculatorBase {
		readonly DependencyIntermediateViewInfo viewInfo;
		protected ConnectionPointCalculatorBase(DependencyIntermediateViewInfo dependencyViewInfo) {
			this.viewInfo = dependencyViewInfo;
		}
		protected internal DependencyIntermediateViewInfo ViewInfo { get { return viewInfo; } }
		protected Point Start { get { return ViewInfo.Start; } }
		protected Point End { get { return ViewInfo.End; } }
		public abstract DependencyConnectionPointInfo Calculate(ConnectorItemBase aptConnector, DependencyIntermediateViewInfo prev, DependencyIntermediateViewInfo next);
	}
	#endregion
	#region VerticalDependencyConnectionPointCalculator
	public class VerticalDependencyConnectionPointCalculator : ConnectionPointCalculatorBase {
		public VerticalDependencyConnectionPointCalculator(DependencyIntermediateViewInfo dependencyViewInfo)
			: base(dependencyViewInfo) {
		}
		public override DependencyConnectionPointInfo Calculate(ConnectorItemBase aptConnector, DependencyIntermediateViewInfo prev, DependencyIntermediateViewInfo next) {
			Point projection = ViewInfo.GetPointProjection(aptConnector.End);
			DependencyConnectionPointInfo result = new DependencyConnectionPointInfo();
			if (projection != Point.Empty) {
				result.Point = projection;
				result.AvailableDirections = DependencyConnectionDirectionType.Left | DependencyConnectionDirectionType.Right;
				return result;
			}
			int count = ViewInfo.DependencyViewInfos.Count;
			DependencyViewInfo dependencyInfo = ViewInfo.DependencyViewInfos[count - 1];
			int endY = (dependencyInfo.EndAppointmentViewInfo == null) ? End.Y : End.Y - aptConnector.Length;
			int nearestY = GetNearestPoint(Start.Y, endY, aptConnector.End.Y);
			result.Point = new Point(Start.X, nearestY);
			result.AvailableDirections = CalculateAvailableConnectionDirection(prev, next, nearestY);
			return result;
		}
		protected internal int GetNearestPoint(int point1, int point2, int target) {
			if (Math.Abs(target - point1) < Math.Abs(target - point2))
				return point1;
			return point2;
		}
		DependencyConnectionDirectionType CalculateAvailableConnectionDirection(DependencyIntermediateViewInfo prev, DependencyIntermediateViewInfo next, int nearestY) {
			DependencyConnectionDirectionType result = DependencyConnectionDirectionType.None;
			if (Start.Y == nearestY) {
				if (prev == null)
					return result;
				result = DependencyConnectionDirectionType.Top;
				if (prev.Start.X < Start.X)
					result |= DependencyConnectionDirectionType.Right;
				else
					result |= DependencyConnectionDirectionType.Left;
			} else if (End.Y == nearestY) {
				if (next == null)
					return result;
				result = DependencyConnectionDirectionType.Bottom;
				if (next.End.X > End.X)
					result |= DependencyConnectionDirectionType.Left;
				else
					result |= DependencyConnectionDirectionType.Right;
			} else {
				result |= DependencyConnectionDirectionType.Left | DependencyConnectionDirectionType.Right;
			}
			return result;
		}
	}
	#endregion
	#region HorizontalDependencyConnectionPointCalculator
	public class HorizontalDependencyConnectionPointCalculator : ConnectionPointCalculatorBase {
		public HorizontalDependencyConnectionPointCalculator(DependencyIntermediateViewInfo dependencyViewInfo)
			: base(dependencyViewInfo) {
		}
		DependencyViewInfoCollection DependencyViewInfos { get { return ViewInfo.DependencyViewInfos; } }
		public override DependencyConnectionPointInfo Calculate(ConnectorItemBase aptConnector, DependencyIntermediateViewInfo prev, DependencyIntermediateViewInfo next) {
			DXCollection<Point> availablePoints = GetAvailableConnectPoints();
			XtraSchedulerDebug.Assert(availablePoints.Count > 0);
			if (availablePoints.Count == 1) {
				return CalculateSingleAvailablePoint(availablePoints[0], aptConnector, prev, next);
			}
			DXCollection<Point> noRoundPoints = GetNoRoundPoints(availablePoints, aptConnector);
			if (noRoundPoints.Count > 0)
				return GetNearestPoint(noRoundPoints, aptConnector.End);
			return GetNearestPoint(availablePoints, aptConnector.End);
		}
		protected internal DependencyConnectionPointInfo CalculateSingleAvailablePoint(Point middlePoint, ConnectorItemBase aptConnector, DependencyIntermediateViewInfo perv, DependencyIntermediateViewInfo next) {
			DependencyConnectionPointInfo result = new DependencyConnectionPointInfo();
			result.Point = middlePoint;
			if (!ShouldRoundAppointment(aptConnector, middlePoint))
				return result;
			Point projection = ViewInfo.GetPointProjection(aptConnector.PointPath);
			if (projection == Point.Empty)
				return result;
			if (!ShouldRoundAppointment(aptConnector, projection))
				result.Point = projection;
			return result;
		}
		protected internal DXCollection<Point> GetNoRoundPoints(DXCollection<Point> availablePoints, ConnectorItemBase aptConnector) {
			DXCollection<Point> result = new DXCollection<Point>();
			int count = availablePoints.Count;
			for (int i = 0; i < count; i++) {
				Point point = availablePoints[i];
				if (!ShouldRoundAppointment(aptConnector, point))
					result.Add(point);
			}
			return result;
		}
		protected internal DependencyConnectionPointInfo GetNearestPoint(DXCollection<Point> points, Point point) {
			DependencyConnectionPointInfo pointInfo = new DependencyConnectionPointInfo();
			int count = points.Count;
			XtraSchedulerDebug.Assert(count > 0);
			Point result = points[0];
			for (int i = 0; i < count; i++) {
				Point currentPoint = points[i];
				if (CalcDistance(currentPoint, point) < CalcDistance(result, point))
					result = currentPoint;
			}
			pointInfo.Point = result;
			pointInfo.AvailableDirections = DependencyConnectionDirectionType.Top | DependencyConnectionDirectionType.Bottom;
			return pointInfo;
		}
		protected internal int CalcDistance(Point pt1, Point pt2) {
			return Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y);
		}
		protected internal DXCollection<Point> GetAvailableConnectPoints() {
			DXCollection<Point> result = new DXCollection<Point>();
			int count = DependencyViewInfos.Count;
			for (int i = 0; i < count; i++) {
				DependencyViewInfo viewInfo = DependencyViewInfos[i];
				if (viewInfo.StartGanttViewInfo == null)
					result.Add(viewInfo.Start);
				if (viewInfo.EndGanttViewInfo == null)
					result.Add(viewInfo.End);
			}
			if (result.Count == 0)
				result.Add(GetMiddlePoint(DependencyViewInfos[0]));
			return result;
		}
		protected internal Point GetMiddlePoint(DependencyViewInfo info) {
			return new Point((info.Start.X + info.End.X) / 2, info.Start.Y);
		}
		bool ShouldRoundAppointment(ConnectorItemBase aptConnector, Point pathEnd) {
			return DependencyLayoutUtils.ShouldRoundAppointment(aptConnector, pathEnd);
		}
	}
	#endregion
	#region DependencyArrowViewInfoImageItem
	public class DependencyViewInfoArrowItem : ViewInfoImageItem {
		public DependencyViewInfoArrowItem(Image image)
			: base(image) {
		}
	}
	#endregion
	#region DependencyCornerViewInfoImageItem
	public class DependencyViewInfoCornerItem : ViewInfoImageItem {
		#region Fields
		DependencyCornerType cornerType;
		#endregion
		public DependencyViewInfoCornerItem(Image image, DependencyCornerType cornerType)
			: base(image) {
			this.cornerType = cornerType;
		}
		#region Properties
		public DependencyCornerType CornerType { get { return cornerType; } set { cornerType = value; } }
		#endregion
	}
	#endregion
}

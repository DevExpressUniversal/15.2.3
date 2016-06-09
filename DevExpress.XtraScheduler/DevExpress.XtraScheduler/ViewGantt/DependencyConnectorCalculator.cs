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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Drawing {
	public class DependencyConnectorCalculator {
		IntersectionObjectsInfo intersectionObjects;
		int connectorIndent;
		Rectangle visibleBounds;
		MoreButtonCollection moreButtons;
		public DependencyConnectorCalculator(IntersectionObjectsInfo intersectionObjects, int connectorIndent, Rectangle visibleBounds, MoreButtonCollection moreButtons) {
			Guard.ArgumentNotNull(intersectionObjects, "intersectionObjects");
			this.intersectionObjects = intersectionObjects;
			this.connectorIndent = connectorIndent;
			this.visibleBounds = visibleBounds;
			this.moreButtons = moreButtons;
		}
		#region Properties
		protected internal IntersectionObjectsInfo IntersectionObjects { get { return intersectionObjects; } }
		protected internal int ConnectorIndent { get { return connectorIndent; } }
		protected internal Rectangle VisibleBounds { get { return visibleBounds; } }
		protected internal MoreButtonCollection MoreButtons { get { return moreButtons; } }
		#endregion
		public void ValidateConnectors(ConnectorItemBase startConnector, ConnectorItemBase endConnector, ConnectionPointsInfo fromInfo, ConnectionPointsInfo toInfo) {
			if (startConnector.ConnectorLocation == ConnectorLocation.BottomRight && endConnector.ConnectorLocation == ConnectorLocation.Left) {
				if (startConnector.PointPath.X > endConnector.PointPath.X) {
					fromInfo.ReturnNextPoint(startConnector.ConnectorLocation, startConnector.PointAppointment);
					Point newStartConnectorAptPoint = fromInfo.GetNextPoint(startConnector.ConnectorLocation, endConnector.PointPath);
					Point newStartConnectorIndentPoint = CreatePointWithIndent(newStartConnectorAptPoint, startConnector.ConnectorLocation);
					startConnector.PointPath = newStartConnectorIndentPoint;
					startConnector.PointAppointment = newStartConnectorAptPoint;
				}
			} else if (startConnector.ConnectorLocation == ConnectorLocation.TopRight && endConnector.ConnectorLocation == ConnectorLocation.Left) {
				if (startConnector.PointPath.X > endConnector.PointPath.X) {
					fromInfo.ReturnNextPoint(startConnector.ConnectorLocation, startConnector.PointAppointment);
					Point newStartConnectorAptPoint = fromInfo.GetNextPoint(startConnector.ConnectorLocation, endConnector.PointPath);
					Point newStartConnectorIndentPoint = CreatePointWithIndent(newStartConnectorAptPoint, startConnector.ConnectorLocation);
					startConnector.PointPath = newStartConnectorIndentPoint;
					startConnector.PointAppointment = newStartConnectorAptPoint;
				}
			}
		}
		public virtual ConnectorItemBase CreateConnectorItem(GanttAppointmentViewInfoWrapper from, GanttAppointmentViewInfoWrapper to) {
			if (!from.Visibility.Visible)
				return CreateConnectorInvisibleAppointment(from);
			if (!from.Bounds.IntersectsWith(VisibleBounds))
				return CreateConnectorInvisibleIntervalAppointment(from);
			ConnectorLocation location = CalculateConnectorLocation(from, to, null);
			return CreateConnectorItem(from, location);
		}
		public virtual ConnectorItemBase CreateConnectorItem(GanttAppointmentViewInfoWrapper from, GanttAppointmentViewInfoWrapper to, ConnectorItemBase connector) {
			if (!from.Visibility.Visible)
				return CreateConnectorInvisibleAppointment(from);
			if (!from.Bounds.IntersectsWith(VisibleBounds))
				return CreateConnectorInvisibleIntervalAppointment(from);
			ConnectorLocation location = CalculateConnectorLocation(from, to, connector);
			return CreateConnectorItem(from, location, connector);
		}
		public virtual ConnectorItemBase CreateConnectorItem(GanttAppointmentViewInfoWrapper from, DependencyIntermediateViewInfo to) {
			if (!from.Visibility.Visible)
				return CreateConnectorInvisibleAppointment(from);
			if (!from.Bounds.IntersectsWith(VisibleBounds))
				return CreateConnectorInvisibleIntervalAppointment(from);
			ConnectorLocation location = CalculateConnectorLocation(from, to);
			return CreateConnectorItem(from, location);
		}
		protected internal virtual ConnectorLocation CalculateConnectorLocation(GanttAppointmentViewInfoWrapper from, GanttAppointmentViewInfoWrapper to, ConnectorItemBase connector) {
			bool isFinish = from.ConnectorPosition == ConnectorPosition.Finish;
			if (CanCreateHorizontalConnector(from))
				return isFinish ? ConnectorLocation.Right : ConnectorLocation.Left;
			Rectangle boundsFrom = from.Bounds;
			Rectangle boundsTo = to.Bounds;
			ConnectorLocation testLocation = isFinish ? ConnectorLocation.BottomRight : ConnectorLocation.BottomLeft;
			if (connector != null && (boundsTo.Left == boundsFrom.Right || boundsFrom.Left == boundsTo.Right)) {
				ConnectorLocation connectorLocation = connector.ConnectorLocation;
				if (connectorLocation == ConnectorLocation.TopRight || connectorLocation == ConnectorLocation.TopLeft)
					return isFinish ? ConnectorLocation.TopRight : ConnectorLocation.TopLeft;
				if (connectorLocation == ConnectorLocation.BottomRight || connectorLocation == ConnectorLocation.BottomLeft)
					return isFinish ? ConnectorLocation.BottomRight : ConnectorLocation.BottomLeft;
			} else if (boundsFrom.Top > boundsTo.Bottom)
				testLocation = isFinish ? ConnectorLocation.TopRight : ConnectorLocation.TopLeft;
			else if (boundsFrom.Bottom < boundsTo.Top)
				testLocation = isFinish ? ConnectorLocation.BottomRight : ConnectorLocation.BottomLeft;
			else if (Math.Abs(boundsFrom.Top - boundsTo.Top) < Math.Abs(boundsFrom.Bottom - boundsTo.Bottom))
				testLocation = isFinish ? ConnectorLocation.TopRight : ConnectorLocation.TopLeft;
			if (IsConnectorIntersectWithAppointments(from, to, testLocation)) {
				ConnectorLocation alternateLocation = GetAlternateConnectorLocation(testLocation);
				if (!IsConnectorIntersectWithAppointments(from, to, alternateLocation))
					return alternateLocation;
			}
			return testLocation;
		}
		protected internal virtual ConnectorLocation CalculateConnectorLocation(GanttAppointmentViewInfoWrapper from, DependencyIntermediateViewInfo to) {
			bool isFinish = from.ConnectorPosition == ConnectorPosition.Finish;
			if (CanCreateHorizontalConnector(from))
				return isFinish ? ConnectorLocation.Right : ConnectorLocation.Left;
			Rectangle boundsFrom = from.Bounds;
			int y = CalculateConnectionYCoord(to);
			if (Math.Abs(boundsFrom.Top - y) < Math.Abs(boundsFrom.Bottom - y))
				return isFinish ? ConnectorLocation.TopRight : ConnectorLocation.TopLeft;
			return isFinish ? ConnectorLocation.BottomRight : ConnectorLocation.BottomLeft;
		}
		protected internal virtual bool CanCreateHorizontalConnector(GanttAppointmentViewInfoWrapper from) {
			Point aptPoint = from.GetNextDependencyConnectionPoint();
			bool isFinish = from.ConnectorPosition == ConnectorPosition.Finish;
			ConnectorLocation location = isFinish ? ConnectorLocation.Right : ConnectorLocation.Left;
			Point indentPoint = CreatePointWithIndent(aptPoint, location);
			if (IsPointInvisible(indentPoint))
				return false;
			Rectangle intersection = intersectionObjects.CalculateConnectorIntersections(aptPoint, indentPoint, from.ViewInfo);
			return intersection == Rectangle.Empty;
		}
		protected internal virtual int CalculateConnectionYCoord(DependencyIntermediateViewInfo info) {
			if (info.Direction == DependencyDirection.Horizontal)
				return info.Start.Y;
			return (info.Start.Y + info.End.Y) / 2;
		}
		protected internal virtual ConnectorItemBase CreateConnectorItem(GanttAppointmentViewInfoWrapper info, ConnectorLocation location) {
			Point aptPoint = info.ConnectionPointsInfo.GetNextPoint(location);
			Point indentPoint = CreatePointWithIndent(aptPoint, location);
			if (aptPoint.Y == indentPoint.Y)
				return new HorizontalConnectorItem(aptPoint, indentPoint, info.ConnectorPosition, location);
			return new VerticalConnectorItem(aptPoint, indentPoint, info.ConnectorPosition, location);
		}
		protected internal virtual ConnectorItemBase CreateConnectorItem(GanttAppointmentViewInfoWrapper info, ConnectorLocation location, ConnectorItemBase connector) {
			Point aptPoint = info.ConnectionPointsInfo.GetNextPoint(location, connector.PointPath);
			Point indentPoint = CreatePointWithIndent(aptPoint, location);
			if (aptPoint.Y == indentPoint.Y)
				return new HorizontalConnectorItem(aptPoint, indentPoint, info.ConnectorPosition, location);
			return new VerticalConnectorItem(aptPoint, indentPoint, info.ConnectorPosition, location);
		}
		protected internal virtual ConnectorItemBase CreateConnectorInvisibleAppointment(GanttAppointmentViewInfoWrapper info) {
			AppointmentViewInfoVisibility visibility = info.Visibility;
			if (visibility.InvisibleInterval)
				return CreateConnectorInvisibleIntervalAppointment(info);
			if (visibility.InvisibleResource)
				return CreateConnectorInvisibleResourceAppointment(info);
			return CreateConnectorMoreButtonInvisibleAppointment(info);
		}
		protected internal virtual ConnectorItemBase CreateConnectorInvisibleIntervalAppointment(GanttAppointmentViewInfoWrapper info) {
			int y = info.Bounds.Bottom;
			Point connectorStart, connectorEnd;
			if (info.Bounds.X >= VisibleBounds.Right) {
				connectorStart = new Point(info.Bounds.Right, y);
				connectorEnd = new Point(VisibleBounds.Right, y);
			} else {
				connectorStart = new Point(VisibleBounds.Left, y);
				connectorEnd = new Point(VisibleBounds.Left + IntersectionObjectsInfo.DistanceFromDependency, y);
			}
			return new HorizontalConnectorItem(connectorStart, connectorEnd, info.ConnectorPosition, ConnectorLocation.None);
		}
		protected internal virtual ConnectorItemBase CreateConnectorInvisibleResourceAppointment(GanttAppointmentViewInfoWrapper info) {
			int x = info.ConnectorPosition == ConnectorPosition.Start ? info.Bounds.Left + ConnectorIndent : info.Bounds.Right - ConnectorIndent;
			Point connectorStart, connectorEnd;
			if (info.Bounds.Top <= VisibleBounds.Top) {
				connectorStart = new Point(x, VisibleBounds.Top);
				connectorEnd = new Point(x, VisibleBounds.Top + ConnectorIndent);
			} else {
				connectorStart = new Point(x, visibleBounds.Bottom);
				connectorEnd = new Point(x, visibleBounds.Bottom - ConnectorIndent);
			}
			return new VerticalConnectorItem(connectorStart, connectorEnd, info.ConnectorPosition, ConnectorLocation.None);
		}
		protected internal virtual ConnectorItemBase CreateConnectorMoreButtonInvisibleAppointment(GanttAppointmentViewInfoWrapper wrapper) {
			TimeInterval interval = wrapper.ViewInfo.Interval;
			DateTime connectorPosition = wrapper.ConnectorPosition == ConnectorPosition.Start ? interval.Start : interval.End;
			MoreButton moreButton = GetMoreButton(connectorPosition, wrapper.ViewInfo.Resource);
			Point ptStart = GetConnectorStart(moreButton, wrapper);
			Point ptEnd = new Point(ptStart.X, ptStart.Y - ConnectorIndent);
			return new VerticalConnectorItem(ptStart, ptEnd, wrapper.ConnectorPosition, ConnectorLocation.None);
		}
		protected internal virtual MoreButton GetMoreButton(DateTime connectorPosition, Resource resource) {
			int count = MoreButtons.Count;
			for (int i = 0; i < count; i++) {
				MoreButton button = moreButtons[i];
				if (ResourceBase.MatchIds(button.Resource, resource) && button.Interval.Contains(connectorPosition))
					return button;
			}
			return MoreButton.Empty;
		}
		protected internal virtual Point GetConnectorStart(MoreButton moreButton, GanttAppointmentViewInfoWrapper info) {
			if (moreButton != MoreButton.Empty)
				return new Point(moreButton.Bounds.X + moreButton.Bounds.Width / 2, moreButton.Bounds.Top);
			int x = info.ConnectorPosition == ConnectorPosition.Start ? info.Bounds.Left : info.Bounds.Right;
			return new Point(x, info.Bounds.Y);
		}
		protected internal virtual Point CreatePointWithIndent(Point source, ConnectorLocation location) {
			switch (location) {
				case ConnectorLocation.TopLeft:
				case ConnectorLocation.TopRight:
					return new Point(source.X, source.Y - ConnectorIndent);
				case ConnectorLocation.Left:
					return new Point(source.X - ConnectorIndent, source.Y);
				case ConnectorLocation.Right:
					return new Point(source.X + ConnectorIndent, source.Y);
				case ConnectorLocation.BottomLeft:
				case ConnectorLocation.BottomRight:
					return new Point(source.X, source.Y + ConnectorIndent);
				default:
					return Point.Empty;
			}
		}
		protected internal virtual bool IsPointInvisible(Point indentPoint) {
			return (indentPoint.X <= VisibleBounds.Left) || (indentPoint.X >= VisibleBounds.Right);
		}
		protected internal virtual bool IsAppointmentInvisible(GanttAppointmentViewInfoWrapper wrapper) {
			Rectangle aptBounds = wrapper.ViewInfo.Bounds;
			return aptBounds.Right < VisibleBounds.Left || aptBounds.Left > VisibleBounds.Right;
		}
		ConnectorLocation GetAlternateConnectorLocation(ConnectorLocation location) {
			if (location == ConnectorLocation.BottomRight)
				return ConnectorLocation.TopRight;
			if (location == ConnectorLocation.TopRight)
				return ConnectorLocation.BottomRight;
			if (location == ConnectorLocation.BottomLeft)
				return ConnectorLocation.TopLeft;
			if (location == ConnectorLocation.TopLeft)
				return ConnectorLocation.BottomLeft;
			return location;
		}
		bool IsConnectorIntersectWithAppointments(GanttAppointmentViewInfoWrapper from, GanttAppointmentViewInfoWrapper to, ConnectorLocation testLocation) {
			Point aptPoint = from.ConnectionPointsInfo.GetPoint(testLocation);
			Point indentPoint = CreatePointWithIndent(aptPoint, testLocation);
			if (IsPointInvisible(indentPoint))
				return false;
			Rectangle intersection = intersectionObjects.CalculateConnectorIntersections(aptPoint, indentPoint, from.ViewInfo);
			return intersection != Rectangle.Empty;
		}
	}
}

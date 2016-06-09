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
using System.Linq;
using System.Windows;
using DevExpress.Internal;
namespace DevExpress.Diagram.Core.Native {
	public static class ConnectorIntersectionHelper {
		public static IEnumerable<IntersectionInfo> GetIntersections(this IEnumerable<IDiagramConnector> connectors) {
			return connectors.SelectMany((c, i) => c.GetIntersections(connectors.Skip(i + 1)));
		}
		public static IEnumerable<IntersectionInfo> GetIntersections(this IDiagramConnector connector, IEnumerable<IDiagramConnector> connectors) {
			return connectors.Except(connector.Yield()).SelectMany(c => connector.GetIntersections(c));
		}
		public static IEnumerable<IntersectionInfo> GetIntersections(this IDiagramConnector connector, IDiagramConnector with) {
			if(connector.Type == ConnectorType.Curved || with.Type == ConnectorType.Curved)
				return Enumerable.Empty<IntersectionInfo>();
			var connector1Segments = connector.Segments();
			var connector2Segments = with.Segments();
			var intersections = connector1Segments.SelectMany(segment => connector2Segments.Select(segment2 => new IntersectionInfo(segment, segment2)));
			return intersections.Where(info => info.IntersectionPoint.HasValue).ToArray();
		}
	}
	public class IntersectionInfo {
		public ConnectorSegment Segment1 { get; private set; }
		public ConnectorSegment Segment2 { get; private set; }
		public Point? IntersectionPoint { get { return intersectionPoint; } }
		public IntersectionInfo(ConnectorSegment segment1, ConnectorSegment segment2) {
			Segment1 = segment1;
			Segment2 = segment2;
			intersectionPoint = Segment1.GetIntersectPoint(Segment2);
		}
		public override bool Equals(object obj) {
			var intersection = obj as IntersectionInfo;
			if(intersection == null)
				return false;
			if (!intersectionPoint.Equals(intersection.IntersectionPoint))
				return false;
			return Segment1.Equals(intersection.Segment1) && Segment2.Equals(intersection.Segment2);
		}
		public override int GetHashCode() {
			return Segment1.GetHashCode() ^ Segment2.GetHashCode() ^ intersectionPoint.GetHashCode();
		}
		readonly Point? intersectionPoint;
	}
	public class ConnectorSegment {
		public double AngleRad { get; private set; }
		public bool IsHorizontal { get { return Start.Y == End.Y; } }
		public bool IsVertical { get { return Start.X == End.X; } }
		public Point Start { get; private set; }
		public Point End { get; private set; }
		public IDiagramConnector Connector { get; private set; }
		public ConnectorSegment(IDiagramConnector connector, Point start, Point end) {
			Connector = connector;
			Start = start;
			End = end;
			AngleRad = End.GetAngleRad(Start);
		}
		public Point? GetIntersectPoint(ConnectorSegment segment) {
			return MathHelper.GetIntersectionPoint(Start, End, segment.Start, segment.End);
		}
		public bool IntersectsWith(IDiagramItem item) {
			return item.Controller.GetIntersectionPoints(Start, End).Any();
		}
		public override bool Equals(object obj) {
			var segment = obj as ConnectorSegment;
			if(segment == null)
				return false;
			return object.ReferenceEquals(Connector, segment.Connector) && Start.Equals(segment.Start) && End.Equals(segment.End);
		}
		public override int GetHashCode() {
			return Connector.GetHashCode() ^ Start.GetHashCode() ^ End.GetHashCode();
		}
	}
}

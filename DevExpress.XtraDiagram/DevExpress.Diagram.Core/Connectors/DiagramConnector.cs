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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Native;
using DevExpress.Diagram.Core.TypeConverters;
using DevExpress.Internal;
using DevExpress.Utils.Serializing;
namespace DevExpress.Diagram.Core {
	[TypeConverter(typeof(ConnectorTypeTypeConverter))]
	public sealed class ConnectorType {
		public static ConnectorType Curved { get; private set; }
		public static ConnectorType RightAngle { get; private set; }
		public static ConnectorType Straight { get; private set; }
		public static IEnumerable<ConnectorType> RegisteredTypes { get { return registeredTypes.Values; } }
		static readonly Dictionary<string, ConnectorType> registeredTypes;
		static ConnectorType() {
			registeredTypes = new Dictionary<string, ConnectorType>();
			Curved = Create("Curved", () => DiagramControlLocalizer.GetString(DiagramControlStringId.ConnectorType_Curved));
			RightAngle = Create("RightAngle", () => DiagramControlLocalizer.GetString(DiagramControlStringId.ConnectorType_RightAngle));
			Straight = Create("Straight", () => DiagramControlLocalizer.GetString(DiagramControlStringId.ConnectorType_Straight));
		}
		public static ConnectorType Create(string id, Func<string> getTypeName) {
			return registeredTypes.GetOrAdd(id, () => new ConnectorType(id, getTypeName));
		}
		public string Id { get; private set; }
		public string TypeName { get { return getTypeName(); } }
		readonly Func<string> getTypeName;
		ConnectorType(string id, Func<string> getTypeName) {
			Id = id;
			this.getTypeName = getTypeName;
		}
		public override string ToString() {
			return TypeName;
		}
	}
	public interface IDiagramConnector : IDiagramItem {
		[XtraSerializableProperty]
		ArrowDescription BeginArrow { get; set; }
		[XtraSerializableProperty]
		ArrowDescription EndArrow { get; set; }
		[XtraSerializableProperty]
		Size BeginArrowSize { get; set; }
		[XtraSerializableProperty]
		Size EndArrowSize { get; set; }
		[XtraSerializableProperty]
		int BeginItemPointIndex { get; set; }
		[XtraSerializableProperty]
		int EndItemPointIndex { get; set; }
		Point BeginPoint { get; set; }
		Point EndPoint { get; set; }
		[XtraSerializableProperty]
		ConnectorPointsCollection Points { get; set; }
		[XtraSerializableProperty]
		ConnectorType Type { get; set; }
		[XtraSerializableProperty]
		string Text { get; set; }
		IDiagramItem BeginItem { get; set; }
		IDiagramItem EndItem { get; set; }
		void InvalidateAppearance();
		void UpdateRoute();
	}
	public struct ConnectorPointInfo {
		public readonly int ItemPointIndex;
		public readonly Point Point;
		public readonly IDiagramItem Item;
		public ConnectorPointInfo(int itemPointIndex, Point point, IDiagramItem item) {
			ItemPointIndex = itemPointIndex;
			Point = point;
			Item = item;
		}
	}
	public class ConnectorProxy {
		public readonly ConnectorPointInfo Begin, End;
		public readonly ConnectorType Type;
		public readonly ConnectorPointsCollection Points;
		public ConnectorProxy(ConnectorPointInfo begin, ConnectorPointInfo end, ConnectorType type, ConnectorPointsCollection points) {
			Begin = begin;
			End = end;
			Type = type;
			Points = points;
		}
		public Point GetPoint(ConnectorPointType pointType) {
			return PointInfo(pointType).Point;
		}
		public ConnectorProxy SetPoint(Point value, ConnectorPointType pointType) {
			return SetPointInfo(new ConnectorPointInfo(-1, value, null), pointType);
		}
		public ConnectorProxy SetConnectionPoint(IDiagramItem item, int pointIndex, ConnectorPointType pointType) {
			return SetPointInfo(new ConnectorPointInfo(pointIndex, GetPoint(pointType), item), pointType);
		}
		public ConnectorProxy SetPointAtIndex(Point value, int index) {
			return new ConnectorProxy(Begin, End, Type, Points.SetElementAt(index, value));
		}
		public ConnectorProxy SetMiddlePoints(IEnumerable<Point> points) {
			return new ConnectorProxy(Begin, End, Type, new ConnectorPointsCollection(points));
		}
		public ConnectorPoints ActualPoints() {
			return new ConnectorPoints(ActualPoint(ConnectorPointType.Begin), ActualPoint(ConnectorPointType.End), Points);
		}
		public Point ActualPoint(ConnectorPointType pointType) {
			Point? definedPoint = GetDefinedPoint(PointInfo(pointType));
			if(definedPoint.HasValue)
				return definedPoint.Value;
			if(Type == ConnectorType.Straight)
				return GetStraightConnectorActualPoint(pointType);
			return GetRigthAngleActualPoint(pointType);
		}
		Point GetRigthAngleActualPoint(ConnectorPointType pointType) {
			var pointInfo = PointInfo(pointType);
			var item = pointInfo.Item;
			var fromPointInfo = PointInfo(pointType.Opposite());
			var fromPoint = GetDefinedPoint(fromPointInfo);
			var connectionPoints = item.GetDiagramConnectionPoints();
			Point center = item.ActualDiagramBounds().GetCenter();
			if(fromPoint.HasValue)
				return connectionPoints.Any() ? fromPoint.Value.GetNearestPoint(connectionPoints) : center;
			var fromConnectionPoints = fromPointInfo.Item.GetDiagramConnectionPoints();
			if(pointType == ConnectorPointType.Begin)
				return connectionPoints.Any() ?
					GetNearestPoint(connectionPoints, fromConnectionPoints).GetNearestPoint(connectionPoints)
					: center;
			return fromConnectionPoints.Any() ? GetNearestPoint(fromConnectionPoints, connectionPoints) : center;
		}
		Point GetStraightConnectorActualPoint(ConnectorPointType pointType) {
			var pointInfo = PointInfo(pointType);
			var item = pointInfo.Item;
			Point fromPoint = GetFromPoint(pointType.Opposite());
			var center = item.ActualDiagramBounds().GetCenter();
			var intersectionPoints = item.Controller.GetIntersectionPoints(fromPoint, center);
			if(!intersectionPoints.Any())
				return center;
			return fromPoint.GetNearestPoint(intersectionPoints);
		}
		Point GetFromPoint(ConnectorPointType pointType) {
			if(Points.Count > 0)
				return pointType.Last(Points);
			var pointInfo = PointInfo(pointType);
			var definedPoint = GetDefinedPoint(pointInfo);
			if(definedPoint.HasValue)
				return definedPoint.Value;
			return pointInfo.Item.ActualDiagramBounds().GetCenter();
		}
		static Point GetNearestPoint(IEnumerable<Point> connectionPoints, IEnumerable<Point> toConnectionPoints) {
			return toConnectionPoints.MinBy(p => p.Distance(p.GetNearestPoint(connectionPoints)));
		}
		static Point? GetDefinedPoint(ConnectorPointInfo pointInfo) {
			if(pointInfo.Item == null)
				return pointInfo.Point;
			if(IsIndexValid(pointInfo))
				return pointInfo.Item.GetDiagramConnectionPoints()[pointInfo.ItemPointIndex];
			return null;
		}
		public bool IsIndexValid(ConnectorPointType pointType) {
			return IsIndexValid(PointInfo(pointType));
		}
		static bool IsIndexValid(ConnectorPointInfo pointInfo) {
			if(pointInfo.Item == null)
				return false;
			return pointInfo.ItemPointIndex >= 0 && pointInfo.ItemPointIndex < pointInfo.Item.Controller.GetConnectionPoints().Count();
		}
		ConnectorPointInfo PointInfo(ConnectorPointType pointType) {
			return pointType == ConnectorPointType.Begin ? Begin : End;
		}
		ConnectorProxy SetPointInfo(ConnectorPointInfo value, ConnectorPointType pointType) {
			var begin = Begin;
			var end = End;
			if(pointType == ConnectorPointType.Begin)
				begin = value;
			else
				end = value;
			return new ConnectorProxy(begin, end, Type, Points);
		}
	}
	public static class DiagramConnectorItemExtensions {
		static readonly PropertyDescriptor BeginPointProperty = ExpressionHelper.GetProperty((IDiagramConnector x) => x.BeginPoint);
		static readonly PropertyDescriptor EndPointProperty = ExpressionHelper.GetProperty((IDiagramConnector x) => x.EndPoint);
		static readonly PropertyDescriptor BeginItemProperty = ExpressionHelper.GetProperty((IDiagramConnector x) => x.BeginItem);
		static readonly PropertyDescriptor EndItemProperty = ExpressionHelper.GetProperty((IDiagramConnector x) => x.EndItem);
		static readonly PropertyDescriptor BeginItemPointIndexProperty = ExpressionHelper.GetProperty((IDiagramConnector x) => x.BeginItemPointIndex);
		static readonly PropertyDescriptor EndItemPointIndexProperty = ExpressionHelper.GetProperty((IDiagramConnector x) => x.EndItemPointIndex);
		public static PropertyDescriptor GetPointProperty(ConnectorPointType pointType) {
			return pointType == ConnectorPointType.Begin ? BeginPointProperty : EndPointProperty;
		}
		public static PropertyDescriptor GetItemPointIndexProperty(ConnectorPointType pointType) {
			return pointType == ConnectorPointType.Begin ? BeginItemPointIndexProperty : EndItemPointIndexProperty;
		}
		public static PropertyDescriptor GetItemProperty(ConnectorPointType pointType) {
			return pointType == ConnectorPointType.Begin ? BeginItemProperty : EndItemProperty;
		}
		public static Point ActualPoint(this IDiagramConnector connector, ConnectorPointType pointType) {
			var controller = connector.Controller();
			return pointType == ConnectorPointType.Begin ? controller.ActualBeginPoint : controller.ActualEndPoint;
		}
		public static Point Point(this IDiagramConnector connector, ConnectorPointType pointType) {
			return pointType == ConnectorPointType.Begin ? connector.BeginPoint : connector.EndPoint;
		}
		public static void SetPoint(this IDiagramConnector connector, Point value, ConnectorPointType pointType) {
			if(pointType == ConnectorPointType.Begin)
				connector.BeginPoint = value;
			else
				connector.EndPoint = value;
		}
		public static IDiagramItem Item(this IDiagramConnector connector, ConnectorPointType pointType) {
			return pointType == ConnectorPointType.Begin ? connector.BeginItem: connector.EndItem;
		}
		public static void SetItem(this IDiagramConnector connector, IDiagramItem item, ConnectorPointType pointType) {
			if(pointType == ConnectorPointType.Begin)
				connector.BeginItem = item;
			else
				connector.EndItem = item;
		}
		public static void SetItemPointIndex(this IDiagramConnector connector, int index, ConnectorPointType pointType) {
			if(pointType == ConnectorPointType.Begin)
				connector.BeginItemPointIndex = index;
			else
				connector.EndItemPointIndex = index;
		}
		public static int ItemPointIndex(this IDiagramConnector connector, ConnectorPointType pointType) {
			return pointType == ConnectorPointType.Begin ? connector.BeginItemPointIndex : connector.EndItemPointIndex;
		}
		public static ArrowDescription Arrow(this IDiagramConnector connector, ConnectorPointType pointType) {
			return pointType == ConnectorPointType.Begin ? connector.BeginArrow: connector.EndArrow;
		}
		public static Size ArrowSize(this IDiagramConnector connector, ConnectorPointType pointType) {
			return pointType == ConnectorPointType.Begin ? connector.BeginArrowSize : connector.EndArrowSize;
		}
		internal static Rect CalcContainingBounds(this IDiagramConnector connector) {
			return new Rect(connector.ActualPoint(ConnectorPointType.Begin), connector.ActualPoint(ConnectorPointType.End));
		}
		internal static ConnectorProxy Proxy(this IDiagramConnector connector) {
			return new ConnectorProxy(
				begin: new ConnectorPointInfo(connector.BeginItemPointIndex, connector.BeginPoint, connector.BeginItem),
				end: new ConnectorPointInfo(connector.EndItemPointIndex, connector.EndPoint, connector.EndItem),
				type: connector.Type,
				points: connector.Points ?? ConnectorPointsCollection.Empty
			);
		}
		public static Point ProxyActualPoint(this IDiagramConnector connector, ConnectorPointType pointType) {
			return connector.Proxy().ActualPoint(pointType);
		}
		public static Point LocalActualPoint(this IDiagramConnector connector, ConnectorPointType pointType) {
			return connector.GetItemRelativePosition(connector.ActualPoint(pointType));
		}
		public static IEnumerable<Point> LocalPoints(this IDiagramConnector connector) {
			var points = connector.Points ?? ConnectorPointsCollection.Empty;
			return points.Select(x => connector.GetItemRelativePosition(x));
		}
		public static double ArrowAngle(this IDiagramConnector connector, ConnectorPointType pointType) {
			var points = connector.Points ?? ConnectorPointsCollection.Empty;
			return connector.LocalActualPoint(pointType)
				.GetAngle(points.Any() ? GetIntermediatePoint(connector, pointType) : connector.LocalActualPoint(pointType.Opposite()));
		}
		static Point GetIntermediatePoint(IDiagramConnector connector, ConnectorPointType pointType) {
			return pointType == ConnectorPointType.Begin ? connector.LocalPoints().First() : connector.LocalPoints().Last();
		}
		public static ShapeGeometry ArrowShape(this IDiagramConnector connector, ConnectorPointType pointType) {
			return GetArrowShape(connector.Arrow(pointType), connector.ArrowSize(pointType), connector.LocalActualPoint(pointType), connector.ArrowAngle(pointType));
		}
		public static Point GetLinePoint(this IDiagramConnector connector, ConnectorPointType pointType) {
			var position = connector.ActualPoint(pointType);
			return connector.Arrow(pointType)
				.Return(arrow => arrow.GetConnectionPoint(connector.ArrowSize(pointType), position, connector.ArrowAngle(pointType)),
				() => position);
		}
		static ShapeGeometry GetArrowShape(ArrowDescription arrow, Size arrowSize, Point position, double angle) {
			return arrow.Return(ar => ar.GetShapePoints(arrowSize, position, angle), () => new ShapeGeometry());
		}
		public static void MakeFree(this IDiagramConnector connector, ConnectorPointType pointType) {
			if(connector.Item(pointType) != null) {
				connector.SetPoint(connector.ActualPoint(pointType), pointType);
				connector.SetItem(null, pointType);
			}
		}
		public static DiagramConnectorController Controller(this IDiagramConnector connector) {
			return (DiagramConnectorController)connector.Controller;
		}
		public static IEnumerable<ConnectorSegment> Segments(this IDiagramConnector connector) {
			var points = new List<Point>();
			points.Add(connector.Controller().ActualBeginPoint);
			points.AddRange(connector.Points());
			points.Add(connector.Controller().ActualEndPoint);
			return points.Take(points.Count - 1).Select((p, i) => new ConnectorSegment(connector, p, points.ElementAt(i + 1)));
		}
		public static Point GetRoutingBeginEndPoint(this IDiagramConnector item, ConnectorPointType type, double margin) {
			Point position = item.ActualPoint(type);
			IDiagramItem diagramNode = item.Item(type);
			if(diagramNode == null)
				return position;
			Rect itemBounds = diagramNode.ActualDiagramBounds();
			Rect boundingBox = itemBounds.InflateRect(Math.Max(0, margin));
			if(itemBounds.X == position.X)
				return new Point(boundingBox.Left, position.Y);
			else if(itemBounds.Y == position.Y)
				return new Point(position.X, boundingBox.Top);
			else if(itemBounds.Right == position.X)
				return new Point(boundingBox.Right, position.Y);
			else
				return new Point(position.X, boundingBox.Bottom);
		}
		public static ConnectorPointsCollection Points(this IDiagramConnector item) {
			return item.Points ?? ConnectorPointsCollection.Empty;
		}
	}
	[TypeConverter(typeof(ConnectorPointsCollectionConverter))]
	public class ConnectorPointsCollection : ReadOnlyCollection<Point> {
		static ConnectorPointsCollection empty;
		public static ConnectorPointsCollection Empty {
			get { return empty ?? (empty = new ConnectorPointsCollection()); }
		}
		ConnectorPointsCollection()
			: base(new List<Point>()) { }
		public ConnectorPointsCollection(IEnumerable<Point> points)
			: base(points.ToList()) { }
		public ConnectorPointsCollection SetElementAt(int index, Point value) {
			return new ConnectorPointsCollection(this.ToArray().Do(x => x[index] = value));
		}
		public ConnectorPointsCollection Translate(Func<Point, Point> selector) {
			return new ConnectorPointsCollection(this.Select(selector));
		}
		public ConnectorPointsCollection Translate(Func<Point, int, Point> selector) {
			return new ConnectorPointsCollection(this.Select(selector));
		}
	}
}

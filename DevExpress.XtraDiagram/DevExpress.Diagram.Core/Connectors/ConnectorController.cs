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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using DevExpress.Diagram.Core.Native;
using DevExpress.Internal;
using System.ComponentModel;
using RoutingStrategy = DevExpress.Diagram.Core.Routing.RoutingStrategy;
using System.Collections.ObjectModel;
using DevExpress.Utils.Serializing;
namespace DevExpress.Diagram.Core {
	public interface IConnectorAdorner {
		Point BeginPoint { get; set; }
		Point EndPoint { get; set; }
		bool IsBeginPointConnected { get; set; }
		bool IsEndPointConnected { get; set; }
		bool IsConnectorCurved { get; set; }
		void SetPoints(Point[] points);
	}
	public interface IConnectorSelectionPartAdorner {
		ShapeGeometry Shape { get; set; }
	}
	public interface IConnectionPointsAdorner {
		void SetPoints(Point[] points);
	}
	public struct ConnectorPoints {
		public readonly Point BeginPoint, EndPoint;
		public readonly ConnectorPointsCollection Points;
		public ConnectorPoints(Point beginPoint, Point endPoint, ConnectorPointsCollection points) {
			BeginPoint = beginPoint;
			EndPoint = endPoint;
			Points = points;
		}
		public ConnectorPoints Translate(Func<Point, Point> translate) {
			return new ConnectorPoints(translate(BeginPoint), translate(EndPoint), new ConnectorPointsCollection(Points.Select(translate)));
		}
	}
	public class DiagramConnectorController : DiagramItemController {
		public const double SelectionStrokeThickness = 7;
		const double contentPositionCoef = 0.5;
		public static readonly Size BridgeSize = new Size(10, 5);
		protected override void OnPositionChangedCore(Point oldValue) {
			MakeFree();
			var offset = MathHelper.GetOffset(oldValue, Connector.Position);
			ActualBeginPoint = ActualBeginPoint.OffsetPoint(offset);
			ActualEndPoint = ActualEndPoint.OffsetPoint(offset);
			Connector.Points = Connector.Points.With(points => points.Translate(x => x.OffsetPoint(offset)));
			NotifyConnectorInvalidated();
			base.OnPositionChangedCore(oldValue);
		}
		protected override void OnSizeChangedCore(Size oldValue) {
			MakeFree();
			var oldBoundsBottomRight = new Rect(Connector.ActualDiagramBounds().Location, oldValue).BottomRight;
			var offset = MathHelper.GetDifference(oldValue, Connector.Size);
			ActualBeginPoint = MathHelper.MoveCornerPoint(ActualBeginPoint, oldBoundsBottomRight, offset);
			ActualEndPoint = MathHelper.MoveCornerPoint(ActualEndPoint, oldBoundsBottomRight, offset);
			NotifyConnectorInvalidated();
			base.OnSizeChangedCore(oldValue);
		}
		#region serialization
		ConnectotTraits traits;
		public class ConnectotTraits {
			readonly DiagramConnectorController controller;
			IDiagramConnector Connector { get { return controller.Connector; } }
			public ConnectotTraits(DiagramConnectorController controller) {
				this.controller = controller;
			}
			DiagramItemFinderPath beginItemPath;
			DiagramItemFinderPath endItemPath;
			[XtraSerializableProperty, DefaultValue(null)]
			public DiagramItemFinderPath BeginItem {
				get { return beginItemPath; }
				set { beginItemPath = value; }
			}
			[XtraSerializableProperty, DefaultValue(null)]
			public DiagramItemFinderPath EndItem {
				get { return endItemPath; }
				set { endItemPath = value; }
			}
			[XtraSerializableProperty]
			public Point BeginPoint {
				get { return GetPoint(beginItemPath, ConnectorPointType.Begin); }
				set { Connector.BeginPoint = value; }
			}
			[XtraSerializableProperty]
			public Point EndPoint {
				get { return GetPoint(endItemPath, ConnectorPointType.End); }
				set { Connector.EndPoint = value; }
			}
			Point GetPoint(DiagramItemFinderPath path, ConnectorPointType pointType) {
				return (path == null || path.IsEmpty) && Connector.Item(pointType) != null
					? Connector.ActualPoint(pointType)
					: Connector.Point(pointType);
			}
			public void PrepareRelations(Func<IDiagramItem, DiagramItemFinderPath> getPath) {
				PrepareRelation(out beginItemPath, ConnectorPointType.Begin, getPath);
				PrepareRelation(out endItemPath, ConnectorPointType.End, getPath);
			}
			public void RestoreRelations(Func<DiagramItemFinderPath, IDiagramItem> getItem) {
				UpdateRelation(ref beginItemPath, ConnectorPointType.Begin, getItem);
				UpdateRelation(ref endItemPath, ConnectorPointType.End, getItem);
			}
			void PrepareRelation(out DiagramItemFinderPath path, ConnectorPointType pointType, Func<IDiagramItem, DiagramItemFinderPath> getPath) {
				path = Connector.Item(pointType).With(x => getPath(x));
			}
			void UpdateRelation(ref DiagramItemFinderPath path, ConnectorPointType pointType, Func<DiagramItemFinderPath, IDiagramItem> getItem) {
				if(path == null)
					return;
				var item = getItem(path);
#if DEBUGTEST
				if(item == null || item is IDiagramRoot)
					throw new InvalidOperationException();
#endif
				Connector.SetItem(item, pointType);
				path = null;
			}
		}
		public override IEnumerable<PropertyDescriptor> GetItemProperties() {
			var sizeProperty = ExpressionHelper.GetPropertyName((IDiagramItem x) => x.Size);
			var positionProperty = ExpressionHelper.GetPropertyName((IDiagramItem x) => x.Position);
			var result = base.GetItemProperties()
				.Where(x => x.Name != sizeProperty && x.Name != positionProperty)
				.Concat(ProxyPropertyDescriptor.GetProxyDescriptors(Connector, x => x.Controller().traits))
				;
			return result;
		}
		public override void PrepareRelations(Func<IDiagramItem, DiagramItemFinderPath> getPath) {
			base.PrepareRelations(getPath);
			traits.PrepareRelations(getPath);
		}
		public override void RestoreRelations(Func<DiagramItemFinderPath, IDiagramItem> getItem) {
			base.RestoreRelations(getItem);
			traits.RestoreRelations(getItem);
		}
		#endregion
		public void UpdateRoute() {
			if(!Connector.IsInDiagram())
				return;
			var diagram = Connector.GetRootDiagram();
			diagram.ExecuteWithSelectionRestore(transaction => DiagramConnectorActions.UpdateRouteCore(diagram, Connector, transaction.SetItemProperty));
		}
		void MakeFree() {
			Connector.MakeFree(ConnectorPointType.Begin);
			Connector.MakeFree(ConnectorPointType.End);
		}
		void OnActualPointChanged(ConnectorPointType pointType) {
			if(Connector.Item(pointType) == null)
				Connector.SetPoint(Connector.ActualPoint(pointType), pointType);
			SetActualPoint(pointType.Opposite(), Connector.ProxyActualPoint(pointType.Opposite()));
			InvalidateLine();
			UpdateBoundaryProperties(() => {
				UpdateBounds();
				NotifyConnectorInvalidated();
			});
		}
		public void OnPointChanged(ConnectorPointType pointType) {
			if(Connector.Item(pointType) != null)
				return;
			SetActualPoint(pointType, pointType == ConnectorPointType.Begin ? Connector.BeginPoint : Connector.EndPoint);
		}
		public void OnPointsChanged() {
			InvalidateBeginEndPoints();
			InvalidateLine();
			Connector.NotifyInteractionChanged();
		}
		public void OnItemChanged(ConnectorPointType pointType) {
			InvalidateBeginEndPoints();
			Connector.NotifyInteractionChanged();
		}
		public void OnItemPointIndexChanged(ConnectorPointType pointType) {
			if(Connector.Item(pointType) == null)
				return;
			InvalidateBeginEndPoints();
		}
		public void OnAppearancePropertyChanged() {
			InvalidateLine();
		}
		public void OnTypeChanged() {
			InvalidateLine();
			InvalidateBeginEndPoints();
			Connector.NotifyInteractionChanged();
		}
		public void Update() {
			InvalidateBeginEndPoints();
		}
		public override IEnumerable<Point> GetIntersectionPoints(Point point1, Point point2) {
			return new ShapeGeometry(GetLineSegments().ToArray()).GetIntersectionPoints(Item.Position, point1, point2);
		}
		void SetActualPoint(ConnectorPointType pointType, Point value) {
			if(pointType == ConnectorPointType.Begin)
				ActualBeginPoint = value;
			else
				ActualEndPoint = value;
		}
		void InvalidateBeginEndPoints() {
			UpdateBeginEndPoints(Connector.Proxy());
			NotifyConnectorInvalidated();
		}
		void UpdateBounds() {
			var bounds = Connector.CalcContainingBounds();
			if(bounds.Equals(Item.ActualDiagramBounds()))
				return;
			InvalidateLine();
			Item.SetBounds(bounds);
			base.OnBoundsChangedCore();
		}
		void NotifyConnectorInvalidated() {
			var diagram = Connector.GetRootDiagram();
			if(diagram != null)
				diagram.Controller.Connectors.Invalidate(Connector);
		}
		void InvalidateShape() {
			connectorShape = null;
			Connector.InvalidateAppearance();
		}
		void InvalidateLine() {
			lineSegments = null;
			InvalidateShape();
		}
		Point actualBeginPoint, actualEndPoint;
		public Point ActualBeginPoint {
			get { return actualBeginPoint; }
			private set {
				if(actualBeginPoint == value)
					return;
				actualBeginPoint = value;
				OnActualPointChanged(ConnectorPointType.Begin);
			}
		}
		public Point ActualEndPoint {
			get { return actualEndPoint; }
			private set {
				if(actualEndPoint == value)
					return;
				actualEndPoint = value;
				OnActualPointChanged(ConnectorPointType.End);
			}
		}
		IEnumerable<Point> LocalPoints {
			get { return Connector.LocalPoints(); }
		}
		public IEnumerable<IntersectionInfo> Intersections { get; private set; }
		IEnumerable<Point> LocalIntersections {
			get { return Intersections.Select(i => ToLocalPoint(i.IntersectionPoint.Value)); }
		}
		IDiagramConnector Connector {
			get { return (IDiagramConnector)Item; }
		}
		readonly IFontTraits fontTraits;
		public DiagramConnectorController(IDiagramConnector connector)
			: base(connector) {
			fontTraits = new FontTraits(Item);
			traits = new ConnectotTraits(this);
			Intersections = Enumerable.Empty<IntersectionInfo>();
		}
		ShapeGeometry connectorShape;
		public ShapeGeometry GetShape() {
			Diagram.Do(x => x.Controller.Connectors.UpdateIntersectionsForConnectors());
			if(connectorShape == null) {
				var lineWithBridges = GetLineSegments().GetConnectorShapeWithBridges(LocalIntersections);
				connectorShape = Connector.ArrowShape(ConnectorPointType.Begin)
					.Concat(lineWithBridges)
					.Concat(Connector.ArrowShape(ConnectorPointType.End));
#if DEBUGTEST
				CreateShapeCountForTests++;
#endif
			}
			return connectorShape;
		}
		public Point GetTextPosition() {
			return GetLineSegments().GetPointPosition(contentPositionCoef);
		}
		IEnumerable<ShapeSegment> lineSegments;
		IEnumerable<ShapeSegment> GetLineSegments() {
			if(lineSegments == null) {
				var points = new ConnectorPoints(Connector.GetLinePoint(ConnectorPointType.Begin),
												 Connector.GetLinePoint(ConnectorPointType.End),
												 Connector.Points()).Translate(ToLocalPoint);
				lineSegments = GetLineSegments(points);
#if DEBUGTEST
				CreateLineCountForTests++;
#endif
			}
			return lineSegments;
		}
		void UpdateBeginEndPoints(ConnectorProxy proxy) {
			UpdateBoundaryProperties(() => {
				ActualBeginPoint = proxy.ActualPoint(ConnectorPointType.Begin);
				ActualEndPoint = proxy.ActualPoint(ConnectorPointType.End);
				UpdateBounds();
			});
		}
		internal IEnumerable<ShapeSegment> GetLineSegments(ConnectorPoints points) {
			return DiagramConnectorExtensions.GetLineSegments(
				Connector.Type,
				points.BeginPoint,
				points.Points,
				points.EndPoint
			);
		}
#if DEBUGTEST
		public int CreateLineCountForTests;
		public int CreateShapeCountForTests;
#endif
		public override IFontTraits GetFontTraits() {
			return fontTraits;
		}
		internal void SetIntersections(IEnumerable<IntersectionInfo> intersections) {
			if(Intersections.SequenceEqual(intersections))
				return;
			Intersections = intersections;
			InvalidateShape();
		}
		Point ToLocalPoint(Point point) {
			return Item.GetItemRelativePosition(point);
		}
		public override Rect GetInplaceEditAdornerBounds() {
			var position = Item.GetDiagramPoint(new Point_Angle(GetTextPosition(), 0));
			return new Rect(position.Point, new Size(double.NaN, double.NaN));
		}
		internal override Func<IDiagramControl, DefaultSelectionLayerHandler, IUpdatableAdorner> SingleSelectionAdornerFactory {
			get { return (diagram, handler) => diagram.AdornerFactory().CreateConnectorSelectionAdorner(Connector).AsUpdatableAdorner(UpdateSingleSelectionAdorner); }
		}
		internal override Func<IAdornerFactory, bool, IUpdatableAdorner> SelectionPartAdornerFactory {
			get { return (factory, isPrimarySelection) => factory.CreateConnectorSelectionPartAdorner(Connector, isPrimarySelection).AsUpdatableAdorner(UpdateSelectionPartAdorner); }
		}
		protected internal override IAdorner CreateDragPreviewAdorner(IDiagramControl diagram) {
			return diagram.AdornerFactory().CreateConnectorDragPreview(GetShape());
		}
		void UpdateSingleSelectionAdorner(IConnectorAdorner adorner) {
			adorner.BeginPoint = Connector.LocalActualPoint(ConnectorPointType.Begin);
			adorner.EndPoint = Connector.LocalActualPoint(ConnectorPointType.End);
			adorner.IsBeginPointConnected = Connector.BeginItem != null;
			adorner.IsEndPointConnected = Connector.EndItem != null;
			adorner.SetPoints(LocalPoints.ToArray());
			adorner.IsConnectorCurved = Connector.Type == ConnectorType.Curved;
		}
		void UpdateSelectionPartAdorner(IConnectorSelectionPartAdorner adorner) {
			adorner.Shape = new ShapeGeometry(GetLineSegments(Connector.Proxy().ActualPoints().Translate(ToLocalPoint)).ToArray());
		}
		public override PropertyDescriptor GetTextProperty() {
			return ExpressionHelper.GetProperty((IDiagramConnector item) => item.Text);
		}
		internal override AdjustmentBase CreateLayoutAdjustment(Item_Owner_Bounds moveInfo, Func<IDiagramItem, bool> isMovingItem) {
			var diagramBounds = Item.ActualDiagramBounds();
			var topLeftOffset = MathHelper.GetOffset(diagramBounds.TopLeft, moveInfo.DiagramBounds.TopLeft);
			var bottomRightOffset = MathHelper.GetOffset(diagramBounds.BottomRight, moveInfo.DiagramBounds.BottomRight);
			Func<ConnectorPointType, Point?> getPoint = pointType =>
				(Connector.Item(pointType) == null || !isMovingItem(Connector.Item(pointType)))
					? (Point?)Connector.ActualPoint(pointType)
						.MoveCornerPoint(diagramBounds.TopLeft, topLeftOffset)
						.MoveCornerPoint(diagramBounds.BottomRight, bottomRightOffset)
					: null;
			var points = Connector.Points.With(x => x.Translate(point => point.OffsetPoint(topLeftOffset)));
			return new ConnectorAdjustment(Connector, getPoint(ConnectorPointType.Begin), getPoint(ConnectorPointType.End), points, moveInfo.Owner, moveInfo.Index);
		}
		#region Themes
		public override DiagramItemStyleId GetDefaultStyleId() {
			return DefaultDiagramStyleId.Subtle1;
		}
		public override ReadOnlyCollection<DiagramItemStyleId> GetDiagramItemStylesId() {
			return DiagramConnectorStyleId.Styles;
		}
		protected override bool GetAllowLightForeground() {
			return false;
		}
		protected override bool GetAllowLightStroke() {
			return false;
		}
		#endregion
	}
}

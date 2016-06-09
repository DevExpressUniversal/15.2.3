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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Diagram.Core.Routing;
namespace DevExpress.Diagram.Core {
	internal static class DiagramConnectorActions {
		internal static readonly PropertyDescriptor PointsProperty = ExpressionHelper.GetProperty((IDiagramConnector x) => x.Points);
		static readonly PropertyDescriptor TypeProperty = ExpressionHelper.GetProperty((IDiagramConnector x) => x.Type);
		public static MouseMoveFeedbackHelper CreateMoveConnectorPointFeedbackHelper(this IDiagramControl diagram) {
			var factory = diagram.AdornerFactory();
			var glueToItemAdornerHelper = SharedAdornersHelper.Create(
				() => factory.CreateGlueToItemAdorner()
			);
			var connectionPointsAdornerHelper = SharedAdornersHelper.Create(
				() => factory.CreateConnectionPointsAdorner().MakeTopmostEx(),
				(adorner, item) => adorner.Model.SetPoints(item.Controller.GetConnectionPoints().ToArray())
			);
			var glueToPointAdornerHelper = SharedAdornersHelper.Create(
				() => factory.CreateGlueToPointAdorner(),
				(IAdorner adorner, Point point) => adorner.Bounds = new Rect(point, default(Size)));
			return new MouseMoveFeedbackHelper(
					provideMouseMoveFeedback: (d, position) => {
						var connectionPointItems = d.GetConnectionPointItems(position);
						var minDistanceConnectionPoint = d.GetMinDistanceConnectionPoint(position, connectionPointItems);
						if(minDistanceConnectionPoint != null && minDistanceConnectionPoint.PointIndex >= 0) {
							glueToPointAdornerHelper.Update(minDistanceConnectionPoint.DiagramPoint);
							glueToItemAdornerHelper.Update();
						} else {
							glueToPointAdornerHelper.Update();
							glueToItemAdornerHelper.Update(minDistanceConnectionPoint.With(x => x.Item).YieldIfNotNull().ToArray());
						}
						connectionPointsAdornerHelper.Update(connectionPointItems);
					},
					clearMouseMoveFeedback: LinqExtensions.CombineActions(
						glueToItemAdornerHelper.Clear,
						connectionPointsAdornerHelper.Clear,
						glueToPointAdornerHelper.Clear)
				);
		}
		public static void MoveConnectorBeginEndPoint(this IDiagramControl diagram, IDiagramConnector connector, Point position, ConnectorPointType pointType) {
			diagram.ExecuteWithSelectionRestore(transaction => {
				if(!connector.IsInDiagram())
					transaction.AddItem(connector, diagram.RootItem());
				diagram.MoveConnectorBeginEndPointCore(connector, position, pointType, (c, value, property) => transaction.SetItemProperty(c, value, property));
			}, restoreSelectionItems: connector.Yield().ToArray(), allowMerge: false);
		}
		public static void MoveStandaloneConnectorBeginEndPoint(this IDiagramControl diagram, IDiagramConnector connector, Point position, ConnectorPointType pointType) {
			diagram.MoveConnectorBeginEndPointCore(connector, position, pointType, (c, value, property) => property.SetValue(c, value));
		}
		static void MoveConnectorBeginEndPointCore(this IDiagramControl diagram, IDiagramConnector connector, Point position, ConnectorPointType pointType,
			Action<IDiagramConnector, object, PropertyDescriptor> setValue) {
			var connectionPointItems = diagram.GetConnectionPointItems(position);
			var connectionPointInfo = diagram.GetMinDistanceConnectionPoint(position, connectionPointItems);
			if(connectionPointInfo != null) {
				setValue(connector, connectionPointInfo.Item, DiagramConnectorItemExtensions.GetItemProperty(pointType));
				setValue(connector, connectionPointInfo.PointIndex, DiagramConnectorItemExtensions.GetItemPointIndexProperty(pointType));
			} else {
				setValue(connector, SnapConnectorPointToGrid(diagram, connector, position), DiagramConnectorItemExtensions.GetPointProperty(pointType));
				setValue(connector, null, DiagramConnectorItemExtensions.GetItemProperty(pointType));
				setValue(connector, -1, DiagramConnectorItemExtensions.GetItemPointIndexProperty(pointType));
			}
			UpdateRouteCore(diagram, connector, setValue);
		}
		public static void MoveConnectorMiddlePoint(this IDiagramControl diagram, IDiagramConnector connector, Point mousePosition, int pointIndex) {
			diagram.ExecuteWithSelectionRestore(transaction => {
				transaction.SetItemProperty(connector, connector.Points.SetElementAt(pointIndex, SnapConnectorPointToGrid(diagram, connector, mousePosition)), PointsProperty);
			}, allowMerge: false);
		}
		public static ConnectorProxy TransformProxyBeginEndPoint(this IDiagramControl diagram, IDiagramConnector connector, ConnectorPointType pointType, ConnectorProxy proxy, Point mousePosition) {
			var transformedProxy = proxy.SetPoint(mousePosition, pointType);
			var position = transformedProxy.GetPoint(pointType);
			var connectionPointItems = diagram.GetConnectionPointItems(position);
			var minDistanceConnectionPoint = diagram.GetMinDistanceConnectionPoint(position, connectionPointItems);
			if(minDistanceConnectionPoint != null) {
				var snappedPoint =  connector.Proxy()
					.SetConnectionPoint(minDistanceConnectionPoint.Item, minDistanceConnectionPoint.PointIndex, pointType)
					.ActualPoint(pointType);
				transformedProxy = transformedProxy.SetPoint(snappedPoint, pointType)
								   .SetConnectionPoint(minDistanceConnectionPoint.Item, minDistanceConnectionPoint.PointIndex, pointType);
			} else {
				transformedProxy = transformedProxy.SetPoint(SnapConnectorPointToGrid(diagram, connector, transformedProxy.GetPoint(pointType)), pointType);
			}
			return transformedProxy.SetMiddlePoints(diagram.RouteConnector(transformedProxy).Points);
		}
		public static ConnectorProxy TransformProxyMiddlePoint(this IDiagramControl diagram, IDiagramConnector connector, int pointIndex, ConnectorProxy proxy, Point mousePosition) {
			return proxy.SetPointAtIndex(SnapConnectorPointToGrid(diagram, connector, mousePosition), pointIndex);
		}
		static Point SnapConnectorPointToGrid(IDiagramControl diagram, IDiagramConnector connector, Point point) {
			return diagram.GetSnapInfo(snapScopeItem: null, isSnappingEnabled: true).SnapPoint(point);
		}
		class ConnectionPointIndexInfo {
			public readonly IDiagramItem Item;
			public readonly int PointIndex;
			public Point DiagramPoint { get { return Item.GetDiagramConnectionPoints()[PointIndex]; } }
			public ConnectionPointIndexInfo(IDiagramItem item, int pointIndex) {
				Item = item;
				PointIndex = pointIndex;
			}
		}
		static ConnectionPointIndexInfo GetMinDistanceConnectionPoint(this IDiagramControl diagram, Point position, IDiagramItem[] connectionPointItems) {
			var pointResult = connectionPointItems.Any()
				? connectionPointItems
					.SelectMany(x => x.Controller.GetConnectionPoints().Select((p, i) => new ConnectionPointIndexInfo(x, i)))
					.MinBy(x => x.DiagramPoint.Distance(position))
					.If(x => x.DiagramPoint.Distance(position) <= diagram.GlueToConnectionPointDistance)
				: null;
			if(pointResult != null)
				return pointResult;
			var glueToItem = diagram.RootItem()
				.FindItemAtPoint(position, x => x.HasConnectionPoints())
				.If(x => x != diagram.RootItem());
			return glueToItem.With(x => new ConnectionPointIndexInfo(x, -1));
		}
		public static void UpdateMoveAdorner(this IDiagramConnector connector, IAdorner<IConnectorMovePointAdorner> adorner, Func<ConnectorProxy, ConnectorProxy> changeProxy) {
			var points = changeProxy(connector.Proxy()).ActualPoints();
			adorner.Bounds = new Rect(points.BeginPoint, points.EndPoint);
			var movedOffset = adorner.Bounds.Location.InvertPoint();
			adorner.Model.Shape = new ShapeGeometry(connector.Controller().GetLineSegments(
				points.Translate(x => x.OffsetPoint(movedOffset))
			).ToArray());
		}
		static IDiagramItem[] GetConnectionPointItems(this IDiagramControl diagram, Point position) {
			return diagram.RootItem().GetChildren()
				.Where(x => {
					var rotatedPoint = position.Rotate(-x.Angle, x.GetDiagramRotationCenter());
					return x.HasConnectionPoints() && x.ActualDiagramBounds().InflateRect(diagram.GlueToConnectionPointDistance).Contains(rotatedPoint);
					})
				.ToArray();
		}
		static bool HasConnectionPoints(this IDiagramItem item) {
			return item.Controller.GetConnectionPoints().Any();
		}
		public static bool CanDrawConnectedConnector(this IDiagramControl diagram, IMouseArgs mouse) {
			var connectionPointItems = diagram.GetConnectionPointItems(mouse.Position);
			return null != diagram.GetMinDistanceConnectionPoint(mouse.Position, connectionPointItems);
		}
		public static ConnectorPoints RouteConnector(this IDiagramControl diagram, ConnectorProxy proxy) {
			var routingStrategy = diagram.GetRoutingStrategy(proxy.Type);
			var items = diagram.RootItem().GetChildren().Where(item => !(item is IDiagramConnector));
			return routingStrategy.RouteConnector(items, proxy);
		}
		public static void UpdateConnectorsRoute(this IDiagramControl diagram) {
			diagram.Items().OfType<IDiagramConnector>().ForEach(c => c.UpdateRoute());
		}
		internal static void UpdateRouteCore(IDiagramControl diagram, IDiagramConnector connector, Action<IDiagramConnector, object, PropertyDescriptor> setValue) {
			var route = diagram.RouteConnector(connector.Proxy());
			setValue(connector, route.BeginPoint, DiagramConnectorItemExtensions.GetPointProperty(ConnectorPointType.Begin));
			setValue(connector, new ConnectorPointsCollection(route.Points), PointsProperty);
			setValue(connector, route.EndPoint, DiagramConnectorItemExtensions.GetPointProperty(ConnectorPointType.End));
		}
		public static void ChangeConnectorType(this IDiagramControl diagram, ConnectorType type) {
			diagram.ExecuteWithSelectionRestore(transaction => {
				diagram.SelectedItems().OfType<IDiagramConnector>().ForEach(connector => {
					transaction.SetItemProperty(connector, type, TypeProperty);
					UpdateRouteCore(diagram, connector, transaction.SetItemProperty);
				});
			}, allowMerge: false);
		}
	}
}

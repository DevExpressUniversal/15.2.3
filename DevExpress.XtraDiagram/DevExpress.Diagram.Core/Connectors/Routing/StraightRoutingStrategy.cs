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
namespace DevExpress.Diagram.Core.Routing {
	public abstract class RoutingStrategy {
		#region static
		public static RoutingStrategy Straight { get { return straight ?? (straight = new StraightRoutingStrategy()); } }
		public static RoutingStrategy RightAngle { get { return rightangle ?? (rightangle = new RightAngleRoutingStrategy()); } }
		static RoutingStrategy OrgChart { get { return orgChart ?? (orgChart = new OrgChartRoutingStrategy()); } }
		static RoutingStrategy Curved { get { return curved ?? (curved = new CurvedRoutingStrategy()); } }
		static RoutingStrategy straight, rightangle, curved, orgChart;
		#endregion
		public ConnectorPoints RouteConnector(IEnumerable<IDiagramItem> items, ConnectorProxy proxy) {
			return OnRouteConnector(items, proxy);
		}
		protected abstract IEnumerable<Point> GetRoutePoints(IEnumerable<IDiagramItem> items, ConnectorProxy proxy);
		protected ConnectorProxy GetActualProxy(IEnumerable<IDiagramItem> items, ConnectorProxy proxy) {
			if(!items.Any())
				return proxy;
			var diagram = items.First().GetRootDiagram();
			var connectorsProxy = diagram.Items().OfType<IDiagramConnector>().Select(c => c.Proxy()).Except(proxy.Yield());
			return GetActualProxyCore(proxy, connectorsProxy);
		}
		protected virtual ConnectorPoints OnRouteConnector(IEnumerable<IDiagramItem> items, ConnectorProxy proxy) {
			var actualProxy = GetActualProxy(items, proxy);
			var beginPoint = actualProxy.ActualPoint(ConnectorPointType.Begin);
			var endPoint = actualProxy.ActualPoint(ConnectorPointType.End);
			var points = new ConnectorPointsCollection(GetRoutePoints(items, actualProxy));
			return new ConnectorPoints(beginPoint, endPoint, points);
		}
		protected virtual ConnectorProxy GetActualProxyCore(ConnectorProxy proxy, IEnumerable<ConnectorProxy> connectorsProxy) {
			return proxy;
		}
	}
	public class StraightRoutingStrategy : RoutingStrategy {
		public StraightRoutingStrategy() { }
		protected override IEnumerable<Point> GetRoutePoints(IEnumerable<IDiagramItem> items, ConnectorProxy proxy) {
			return Enumerable.Empty<Point>();
		}
	}
}

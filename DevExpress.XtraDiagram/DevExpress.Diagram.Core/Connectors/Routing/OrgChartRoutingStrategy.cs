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
using System.Text;
using System.Windows;
using DevExpress.Internal;
namespace DevExpress.Diagram.Core.Routing {
	public class OrgChartRoutingStrategy : RightAngleRoutingStrategy {
		public Direction Direction { get; set; }
		public OrgChartRoutingStrategy() {
			Direction = Direction.Down;
		}
		protected override ConnectorProxy GetActualProxyCore(ConnectorProxy proxy, IEnumerable<ConnectorProxy> connectorsProxy) {
			if(proxy.Begin.Item == null || proxy.End.Item == null)
				return proxy;
			if(proxy.IsIndexValid(ConnectorPointType.Begin) || proxy.IsIndexValid(ConnectorPointType.End))
				return proxy;
			switch(Direction) {
				case Direction.Left:
					return GetLeftDirectionProxy(proxy);
				case Direction.Up:
					return GetUpDirectionProxy(proxy);
				case Direction.Right:
					return GetRightDirectionProxy(proxy);
				case Direction.Down:
					return GetDownDirectionProxy(proxy);
			}
			throw new ArgumentException();
		}
		ConnectorProxy GetLeftDirectionProxy(ConnectorProxy proxy) {
			return GetDirectionProxy(proxy, points => points.MinBy(p => p.X), points => points.MaxBy(p => p.X));
		}
		ConnectorProxy GetUpDirectionProxy(ConnectorProxy proxy) {
			return GetDirectionProxy(proxy, points => points.MinBy(p => p.Y), points => points.MaxBy(p => p.Y));
		}
		ConnectorProxy GetRightDirectionProxy(ConnectorProxy proxy) {
			return GetDirectionProxy(proxy, points => points.MaxBy(p => p.X), points => points.MinBy(p => p.X));
		}
		ConnectorProxy GetDownDirectionProxy(ConnectorProxy proxy) {
			return GetDirectionProxy(proxy, points => points.MaxBy(p => p.Y), points => points.MinBy(p => p.Y));
		}
		ConnectorProxy GetDirectionProxy(ConnectorProxy proxy, Func<IEnumerable<Point>, Point> getBeginPointFunc, Func<IEnumerable<Point>, Point> getEndPointFunc) {
			var beginConnectionPoints = proxy.Begin.Item.GetDiagramConnectionPoints();
			var endConnectionPoints = proxy.End.Item.GetDiagramConnectionPoints();
			var beginPoint = getBeginPointFunc(beginConnectionPoints);
			var endPoint = getEndPointFunc(endConnectionPoints);
			var actualProxy = proxy.SetConnectionPoint(proxy.Begin.Item, beginConnectionPoints.FindIndex(p => p.Equals(beginPoint)), ConnectorPointType.Begin);
			return actualProxy.SetConnectionPoint(proxy.End.Item, endConnectionPoints.FindIndex(p => p.Equals(endPoint)), ConnectorPointType.End);
		}
	}
}

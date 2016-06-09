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
using System.ComponentModel;
using System.Linq;
using DevExpress.Diagram.Core.Native;
using DevExpress.Internal;
using DevExpress.Utils;
namespace DevExpress.Diagram.Core {
	public class ConnectorsController {
		readonly IDiagramControl diagram;
		readonly HashSet<IDiagramConnector> InvalidatedConnectors;
		public IEnumerable<IDiagramConnector> Connectors { get { return diagram.Items().OfType<IDiagramConnector>(); } }
		bool IntersectionsIsValid { get; set; }
		public ConnectorsController(IDiagramControl diagram) {
			Guard.ArgumentNotNull(diagram, "diagram");
			this.diagram = diagram;
			InvalidatedConnectors = new HashSet<IDiagramConnector>();
		}
		public void Invalidate(IDiagramConnector connector) {
			InvalidatedConnectors.Add(connector);
			connector.InvalidateAppearance();
			IntersectionsIsValid = false;
		}
		public void UpdateIntersectionsForConnectors() {
			if(IntersectionsIsValid)
				return;
#if DEBUGTEST
			UpdateIntersectionsCountForTests++;
#endif
			var intersections = Connectors.GetIntersections();
			var lookup = intersections.ToLookup(info => GetHorizontalSegment(info).Connector);
			Connectors.ForEach(c => c.Controller().SetIntersections(lookup[c].ToArray()));
			IntersectionsIsValid = true;
		}
		internal void ItemChanged(IDiagramItem item, ItemChangedKind kind) {
			if(diagram.RootItem() == null || kind == ItemChangedKind.ZOrderChanged)
				return;
			if(kind == ItemChangedKind.Added && item is IDiagramConnector) {
				var connector = (IDiagramConnector)item;
				if(diagram.Controller.IsInitializing)
					Invalidate(connector);
				else if(connector.Points == null)
					UpdateRouteWithoutUndo(connector);
			}
			var connectors = GetAttachedConnectors(item);
			if(kind == ItemChangedKind.Removed) {
				if(item is IDiagramConnector) {
					InvalidateIntersectedConnectors(item);
				} else
					OnAttachedItemRemoved(item, connectors);
				return;
			}
			connectors.ForEach(c => c.Controller().Update());
		}
		internal void UpdateRoute(Item_Owner_Bounds[] moveInfo, Action<IDiagramItem, object, PropertyDescriptor> setPropertyValue) {
			var connectorsToUpdate = Connectors.Where(x => moveInfo.Any(item => item.Item == x.BeginItem || item.Item == x.EndItem));
			connectorsToUpdate.ForEach(connector => DiagramConnectorActions.UpdateRouteCore(diagram, connector, setPropertyValue));
		}
#if DEBUGTEST
		public int UpdateIntersectionsCountForTests;
#endif
		void OnAttachedItemRemoved(IDiagramItem item, IEnumerable<IDiagramConnector> connectors) {
			foreach(var connector in connectors) {
				item.Controller.DetachFromConnector(connector);
			}
		}
		void InvalidateIntersectedConnectors(IDiagramItem item) {
			var connector = (IDiagramConnector)item;
			Connectors.SelectMany(c => c.Controller().Intersections.Select(x => x.Segment1.Connector == connector ? x.Segment2 : x.Segment1)).
				Distinct().ForEach(s => Invalidate(s.Connector));
			InvalidatedConnectors.Remove(connector);
		}
		static ConnectorSegment GetHorizontalSegment(IntersectionInfo info) {
			if(Math.Abs(Math.Sin(info.Segment1.AngleRad)) < Math.Abs(Math.Sin(info.Segment2.AngleRad)))
				return info.Segment1;
			return info.Segment2;
		}
		static ConnectorSegment GetVerticalSegment(IntersectionInfo info) {
			if(GetHorizontalSegment(info) == info.Segment2)
				return info.Segment1;
			return info.Segment2;
		}
		internal void UpdateRouteForAttachedConnectors(Transaction transaction, IDiagramItem item) {
			var connectors = GetAttachedConnectors(item);
			connectors.ForEach(x => DiagramConnectorActions.UpdateRouteCore(diagram, x, transaction.SetItemProperty));
		}
		IEnumerable<IDiagramConnector> GetAttachedConnectors(IDiagramItem item) {
			return Connectors.Where(c => c.BeginItem == item || c.EndItem == item);
		}
		internal void RouteConnectorsWithoutMiddlePoints() {
			Connectors.Where(c => c.Points == null).ForEach(c => UpdateRouteWithoutUndo(c));
		}
		void UpdateRouteWithoutUndo(IDiagramConnector c) {
			c.Points = diagram.RouteConnector(c.Proxy()).Points;
		}
	}
}

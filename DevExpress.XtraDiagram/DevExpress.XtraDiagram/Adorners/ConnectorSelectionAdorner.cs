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
using System.Drawing;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Paint;
using PlatformPoint = System.Windows.Point;
using DevExpress.XtraDiagram.Utils;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.ViewInfo;
namespace DevExpress.XtraDiagram.Adorners {
	public class DiagramConnectorSelectionAdorner : DiagramItemAdornerBase, IConnectorAdorner, IAdorner<IConnectorAdorner>, IDiagramSelectionSupports {
		ConnectorSelection rects;
		Size pointSize;
		bool isMultipleSelection;
		PlatformPoint beginPoint, endPoint;
		bool beginPointConnected, endPointConnected;
		PlatformPoint[] points;
		public DiagramConnectorSelectionAdorner(IDiagramConnector connector, Size pointSize, bool isMultipleSelection) : base(connector) {
			this.pointSize = pointSize;
			this.isMultipleSelection = isMultipleSelection;
		}
		#region IConnectorAdorner
		PlatformPoint IConnectorAdorner.BeginPoint {
			get { return beginPoint; }
			set {
				if(beginPoint != value) {
					beginPoint = value;
					UpdateSelection();
				}
			}
		}
		PlatformPoint IConnectorAdorner.EndPoint {
			get { return endPoint; }
			set {
				if(endPoint != value) {
					endPoint = value;
					UpdateSelection();
				}
			}
		}
		bool IConnectorAdorner.IsBeginPointConnected {
			get { return beginPointConnected; }
			set {
				if(beginPointConnected != value) {
					beginPointConnected = value;
					UpdateSelection();
				}
			}
		}
		bool IConnectorAdorner.IsEndPointConnected {
			get { return endPointConnected; }
			set {
				if(endPointConnected != value) {
					this.endPointConnected = value;
					UpdateSelection();
				}
			}
		}
		void IConnectorAdorner.SetPoints(PlatformPoint[] points) {
			this.points = points;
			UpdateSelection();
		}
		bool IConnectorAdorner.IsConnectorCurved {
			get; set;
		}
		#endregion
		#region IAdorner<IConnectorAdorner>
		IConnectorAdorner IAdorner<IConnectorAdorner>.Model { get { return this; } }
		#endregion
		public override void Update() {
			base.Update();
			UpdateSelection();
		}
		public override bool AffectsSelection { get { return true; } }
		protected void UpdateSelection() {
			this.rects = CalcSelection();
		}
		protected virtual ConnectorSelection CalcSelection() {
			DiagramElementBounds beginPoint = GetDisplayRect(this.beginPoint, this.pointSize);
			DiagramElementBounds endPoint = GetDisplayRect(this.endPoint, this.pointSize);
			return new ConnectorSelection(beginPoint, endPoint, CalcIntermediatePointRects());
		}
		protected DiagramElementBounds[] CalcIntermediatePointRects() {
			if(this.points == null) return new DiagramElementBounds[0];
			return this.points.Select(point => GetDisplayRect(point, this.pointSize)).ToArray();
		}
		protected DiagramElementBounds GetDisplayRect(PlatformPoint platformPoint, Size rectSize) {
			Point point = Connector.Controller.Bounds.Location.ToWinPoint();
			DiagramElementPoint displayPoint = PlatformPointToDisplayPoint(PointUtils.ApplyOffset(point, platformPoint));
			return displayPoint.CreateRect(rectSize);
		}
		public ConnectorSelection Rects { get { return rects; } }
		public bool BeginPointConnected { get { return beginPointConnected; } }
		public bool EndPointConnected { get { return endPointConnected; } }
		public bool IsMultiSelection { get { return isMultipleSelection; } }
		public DiagramConnectorPointItem GetConnectorBeginPointItem() {
			return new DiagramConnectorPointItem(Connector);
		}
		public DiagramConnectorPointItem GetConnectorEndPointItem() {
			return new DiagramConnectorPointItem(Connector);
		}
		public DiagramConnectorPointItem GetConnectorIntermediatePointItem(Point point) {
			if(this.points == null) return null;
			return new DiagramConnectorPointItem(Connector, Rects.GetPointIndex(point));
		}
		public override DiagramAdornerPainterBase GetPainter() {
			return new DiagramConnectorSelectionAdornerPainter();
		}
		public override DiagramAdornerObjectInfoArgsBase GetDrawArgs() {
			return new DiagramConnectorSelectionAdornerObjectInfoArgs();
		}
		public DiagramConnector Connector { get { return (DiagramConnector)base.Item; } }
	}
}

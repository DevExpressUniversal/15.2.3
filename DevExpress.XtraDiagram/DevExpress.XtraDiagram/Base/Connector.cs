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

using DevExpress.Diagram.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Base;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.XtraEditors;
namespace DevExpress.XtraDiagram {
	public partial class DiagramConnector : DiagramItem, IDiagramConnector, IXtraPathView {
		DiagramConnectorPointCollection intermediatePoints;
		public DiagramConnector() {
			this._BeginArrowSize = this._EndArrowSize = DefaultArrowSize;
			this._BeginArrow = null;
			this._EndArrow = null;
			this.intermediatePoints = new DiagramConnectorPointCollection();
			this.intermediatePoints.ListChanged += OnIntermediatePointListChanged;
		}
		public DiagramConnector(ConnectorType connectorType) : this() {
			SetConnectorType(connectorType);
		}
		public DiagramConnector(DiagramItem beginItem, DiagramItem endItem) : this() {
			SetItems(beginItem, endItem);
		}
		public DiagramConnector(DiagramItem beginItem, DiagramItem endItem, params PointF[] intermediatePoints) : this(beginItem, endItem) {
			SetPointCollection(intermediatePoints);
		}
		public DiagramConnector(ConnectorType connectorType, DiagramItem beginItem, DiagramItem endItem) : this(beginItem, endItem) {
			SetConnectorType(connectorType);
		}
		public DiagramConnector(PointF beginPoint, PointF endPoint) : this() {
			SetPoints(beginPoint, endPoint);
		}
		public DiagramConnector(PointFloat beginPoint, PointFloat endPoint) : this() {
			SetPoints(beginPoint, endPoint); 
		}
		public DiagramConnector(ConnectorType connectorType, PointF beginPoint, PointF endPoint) : this(beginPoint, endPoint) {
			SetConnectorType(connectorType);
		}
		public DiagramConnector(ConnectorType connectorType, PointFloat beginPoint, PointFloat endPoint) : this(beginPoint, endPoint) {
			SetConnectorType(connectorType);
		}
		public DiagramConnector(Point beginPoint, Point endPoint) : this() {
			SetPoints(beginPoint, endPoint);
		}
		public DiagramConnector(ConnectorType connectorType, Point beginPoint, Point endPoint) : this(beginPoint, endPoint) {
			SetConnectorType(connectorType);
		}
		public DiagramConnector(PointF beginPoint, PointF endPoint, params PointF[] intermediatePoints) : this(beginPoint, endPoint) {
			SetPointCollection(intermediatePoints);
		}
		public DiagramConnector(PointFloat beginPoint, PointFloat endPoint, params PointFloat[] intermediatePoints) : this(beginPoint, endPoint) {
			SetPointCollection(intermediatePoints);
		}
		public DiagramConnector(ConnectorType connectorType, PointF beginPoint, PointF endPoint, params PointF[] intermediatePoints) : this(beginPoint, endPoint, intermediatePoints) {
			SetConnectorType(connectorType);
		}
		public DiagramConnector(ConnectorType connectorType, PointFloat beginPoint, PointFloat endPoint, params PointFloat[] intermediatePoints) : this(beginPoint, endPoint, intermediatePoints) {
			SetConnectorType(connectorType);
		}
		public DiagramConnector(Point beginPoint, Point endPoint, params Point[] intermediatePoints) : this(beginPoint, endPoint) {
			SetPointCollection(intermediatePoints);
		}
		public DiagramConnector(ConnectorType connectorType, Point beginPoint, Point endPoint, params Point[] intermediatePoints) : this(beginPoint, endPoint, intermediatePoints) {
			SetConnectorType(connectorType);
		}
		internal static SizeF DefaultArrowSize = new SizeF(10, 7);
		protected override sealed DiagramItemController CreateItemController() {
			return CreateDiagramConnectorController();
		}
		protected virtual DiagramConnectorController CreateDiagramConnectorController() {
			return new XtraDiagramConnectorController(this);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Layout)]
		public DiagramConnectorPointCollection IntermediatePoints {
			get { return intermediatePoints; }
			internal set { intermediatePoints = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanSnapToThisItem {
			get { return false; }
			set { }
		}
		protected override bool ShouldSerializeCanSnapToThisItem() { return false; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanSnapToOtherItems {
			get { return false; }
			set { }
		}
		protected override bool ShouldSerializeCanSnapToOtherItems() { return false; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override double Angle {
			get { return 0; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanRotate {
			get { return false; }
			set { }
		}
		protected override bool ShouldSerializeCanRotate() { return false; }
		protected internal override string EditValue {
			get { return Text; }
			set { Text = value; }
		}
		protected internal ShapeGeometry GetShape() {
			return Controller.GetShape();
		}
		protected void SetItems(DiagramItem beginItem, DiagramItem endItem) {
			this._BeginItem = beginItem;
			this._EndItem = endItem;
			Controller.OnItemChanged(ConnectorPointType.Begin);
			Controller.OnItemChanged(ConnectorPointType.End);
		}
		protected void SetPoints(Point beginPoint, Point endPoint) {
			SetPoints(beginPoint.ToPointFloat(), endPoint.ToPointFloat());
		}
		protected void SetPoints(PointF beginPoint, PointF endPoint) {
			SetPoints(beginPoint.ToPointFloat(), endPoint.ToPointFloat());
		}
		protected void SetPoints(PointFloat beginPoint, PointFloat endPoint) {
			this._BeginPoint = beginPoint;
			this._EndPoint = endPoint;
			Controller.OnPointChanged(ConnectorPointType.Begin);
			Controller.OnPointChanged(ConnectorPointType.End);
		}
		protected void SetPointCollection(PointF[] intermediatePoints) {
			IntermediatePoints.AddRange(intermediatePoints);
		}
		protected void SetPointCollection(Point[] intermediatePoints) {
			IntermediatePoints.AddRange(intermediatePoints);
		}
		protected void SetPointCollection(PointFloat[] intermediatePoints) {
			IntermediatePoints.AddRange(intermediatePoints);
		}
		protected void SetConnectorType(ConnectorType connectorType) {
			this._Type = connectorType;
			this.Controller().OnTypeChanged();
		}
		protected virtual void OnIntermediatePointListChanged(object sender, ListChangedEventArgs e) {
			this.Controller().OnPointsChanged();
			OnPropertiesChanged();
		}
		protected virtual PointFloat CoerceBeginPoint(PointFloat value) {
			return Controller.CoercePoint(value, ConnectorPointType.Begin);
		}
		protected virtual PointFloat CoerceEndPoint(PointFloat value) {
			return Controller.CoercePoint(value, ConnectorPointType.Begin);
		}
		protected virtual void OnBeginItemChanged(IDiagramItem oldItem) {
			Controller.OnItemChanged(ConnectorPointType.Begin);
		}
		protected virtual void OnEndItemChanged(IDiagramItem oldItem) {
			Controller.OnItemChanged(ConnectorPointType.End);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override PointFloat Position {
			get { return base.Position; }
			set { base.Position = value; }
		}
		protected internal override bool IsRoutable { get { return true; } }
		#region IXtraPathView
		int IXtraPathView.ItemId { get { return ItemId; } }
		ShapeGeometry IXtraPathView.Shape {
			get { return Controller.GetShape(); }
		}
		Rectangle IXtraPathView.TextBounds {
			get { return new Rectangle(Controller.GetTextPosition().ToWinPoint(), Size.Empty); }
		}
		#endregion
		protected internal new XtraDiagramConnectorController Controller { get { return base.Controller as XtraDiagramConnectorController; } }
	}
}

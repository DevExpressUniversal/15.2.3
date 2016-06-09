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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.Parser;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.ViewInfo {
	public class DiagramItemViewControllers {
		readonly DiagramContainerViewController containerItemViewController;
		readonly DiagramShapeViewController shapeViewController;
		readonly DiagramConnectorViewController connectorViewController;
		DiagramControlViewInfo diagramViewInfo;
		public DiagramItemViewControllers(DiagramControlViewInfo diagramViewInfo) {
			this.diagramViewInfo = diagramViewInfo;
			this.containerItemViewController = CreateContainerItemViewController();
			this.shapeViewController = CreateShapeViewController();
			this.connectorViewController = CreateConnectorViewController();
		}
		public DiagramItemViewControllerBase GetController(Type diagramItemType) {
			if(typeof(DiagramShape).IsAssignableFrom(diagramItemType)) {
				return shapeViewController;
			}
			if(typeof(DiagramContainer).IsAssignableFrom(diagramItemType)) {
				return containerItemViewController;
			}
			if(typeof(DiagramConnector).IsAssignableFrom(diagramItemType)) {
				return connectorViewController;
			}
			throw new ArgumentException(string.Format("{0} item type is not supported", diagramItemType.GetType().Name));
		}
		public DiagramItemViewControllerBase GetController(DiagramItem item) {
			return GetController(item.GetType());
		}
		protected virtual DiagramContainerViewController CreateContainerItemViewController() {
			return new DiagramContainerViewController(this.diagramViewInfo);
		}
		protected virtual DiagramShapeViewController CreateShapeViewController() {
			return new DiagramShapeViewController(this.diagramViewInfo);
		}
		protected virtual DiagramConnectorViewController CreateConnectorViewController() {
			return new DiagramConnectorViewController(this.diagramViewInfo);
		}
	}
	public abstract class DiagramItemViewControllerBase {
		DiagramControlViewInfo diagramViewInfo;
		public DiagramItemViewControllerBase(DiagramControlViewInfo diagramViewInfo) {
			this.diagramViewInfo = diagramViewInfo;
		}
		protected virtual Rectangle CalcItemBounds(DiagramItem item, Rectangle ownerBounds) {
			return new Rectangle(CalcItemLocation(item, ownerBounds), item.Size);
		}
		protected Point CalcItemLocation(DiagramItem item, Rectangle ownerBounds) {
			return CalcItemLocation(item.Position, ownerBounds);
		}
		protected Point CalcItemLocation(PointFloat point, Rectangle ownerBounds) {
			return CalcItemLocation(point.ToPoint(), ownerBounds);
		}
		protected Point CalcItemLocation(Point point, Rectangle ownerBounds) {
			return new Point(ownerBounds.X + point.X, ownerBounds.Y + point.Y);
		}
		public abstract DiagramItemPaintControllerBase PaintController { get; }
		public DiagramItemInfo CalcItemInfo(DiagramItem item, Rectangle ownerBounds, DiagramAppearanceObject appearance) {
			DiagramItemInfo itemInfo = CreateInstance(item);
			itemInfo.SetBounds(CalcItemBounds(item, ownerBounds));
			itemInfo.SetOwnerBounds(ownerBounds);
			itemInfo.PaintAppearance = appearance;
			itemInfo.SetPaintController(PaintController);
			Initialize(itemInfo, item, ownerBounds, appearance);
			return itemInfo;
		}
		protected abstract DiagramItemInfo CreateInstance(DiagramItem item);
		protected abstract void Initialize(DiagramItemInfo itemInfo, DiagramItem item, Rectangle ownerBounds, DiagramAppearanceObject appearance);
		protected DiagramControlViewInfo DiagramViewInfo { get { return diagramViewInfo; } }
	}
	public abstract class DiagramPathViewItemControllerBase : DiagramItemViewControllerBase {
		public DiagramPathViewItemControllerBase(DiagramControlViewInfo diagramViewInfo)
			: base(diagramViewInfo) {
		}
		protected override void Initialize(DiagramItemInfo itemInfo, DiagramItem item, Rectangle ownerBounds, DiagramAppearanceObject appearance) {
			IXtraPathView view = (IXtraPathView)item;
			((DiagramPathViewItemInfo)itemInfo).SetView(CalcItemView(view, itemInfo.Bounds, appearance));
		}
		protected DiagramItemView CalcItemView(IXtraPathView view, Rectangle bounds, DiagramAppearanceObject appearance) {
			return GetParser().Parse(view, bounds, appearance);
		}
		protected abstract DiagramPathViewParserBase GetParser();
	}
	public class DiagramContainerViewController : DiagramItemViewControllerBase {
		public DiagramContainerViewController(DiagramControlViewInfo diagramViewInfo)
			: base(diagramViewInfo) {
		}
		protected override void Initialize(DiagramItemInfo itemInfo, DiagramItem item, Rectangle ownerBounds, DiagramAppearanceObject appearance) {
			DiagramContainer containerItem = (DiagramContainer)item;
			DiagramContainerInfo containerInfo = (DiagramContainerInfo)itemInfo;
			if(containerItem.HasChildren) {
				containerInfo.CalcItems(childItem => DiagramViewInfo.ItemViewControllers.GetController(childItem).CalcItemInfo(childItem, containerInfo.Bounds, appearance));
			}
		}
		protected override DiagramItemInfo CreateInstance(DiagramItem item) {
			return new DiagramContainerInfo((DiagramContainer)item);
		}
		public override DiagramItemPaintControllerBase PaintController { get { return new DiagramContainerPaintController(DiagramViewInfo); } }
	}
	public class DiagramShapeViewController : DiagramPathViewItemControllerBase {
		public DiagramShapeViewController(DiagramControlViewInfo diagramViewInfo)
			: base(diagramViewInfo) {
		}
		protected override DiagramItemInfo CreateInstance(DiagramItem item) {
			return new DiagramShapeInfo((DiagramShape)item);
		}
		protected override DiagramPathViewParserBase GetParser() {
			return DiagramViewInfo.ShapeParser;
		}
		public override DiagramItemPaintControllerBase PaintController { get { return new DiagramShapePaintController(DiagramViewInfo); } }
	}
	public class DiagramConnectorViewController : DiagramPathViewItemControllerBase {
		public DiagramConnectorViewController(DiagramControlViewInfo diagramViewInfo)
			: base(diagramViewInfo) {
		}
		protected override void Initialize(DiagramItemInfo itemInfo, DiagramItem item, Rectangle ownerBounds, DiagramAppearanceObject appearance) {
			base.Initialize(itemInfo, item, ownerBounds, appearance);
			DiagramConnectorInfo connectorInfo = (DiagramConnectorInfo)itemInfo;
			connectorInfo.SetOutlineBounds(CalcConnectorOutlineBounds(connectorInfo.Item, ownerBounds));
		}
		protected virtual Rectangle CalcConnectorOutlineBounds(DiagramConnector item, Rectangle ownerBounds) {
			Rectangle rect = CalcConnectorBounds(item, ownerBounds);
			return Rectangle.Inflate(rect, DiagramViewInfo.ConnectorOutlineSize / 2, DiagramViewInfo.ConnectorOutlineSize / 2);
		}
		protected Rectangle CalcConnectorBounds(DiagramConnector item, Rectangle ownerBounds) {
			Rectangle rect = CalcItemBounds(item, ownerBounds);
			DiagramConnectorPointCollection intermediatePoints = item.IntermediatePoints;
			if(intermediatePoints == null) return rect;
			for(int i = 0; i < intermediatePoints.Count; i++) {
				Point point = CalcItemLocation(intermediatePoints[i], ownerBounds);
				rect = RectangleUtils.EnsureContains(rect, point);
			}
			return rect;
		}
		protected override DiagramItemInfo CreateInstance(DiagramItem item) {
			return new DiagramConnectorInfo((DiagramConnector)item, DiagramViewInfo.ConnectorOutlinePen);
		}
		protected override DiagramPathViewParserBase GetParser() {
			return DiagramViewInfo.ConnectorParser;
		}
		public override DiagramItemPaintControllerBase PaintController { get { return new DiagramConnectorPaintController(DiagramViewInfo); } }
	}
}

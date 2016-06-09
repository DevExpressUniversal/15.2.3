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
using System.Linq;
using System.Text;
using DevExpress.XtraDiagram.ViewInfo;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraDiagram.Paint {
	public abstract class DiagramItemPaintControllerBase {
		DiagramControlViewInfo diagramViewInfo;
		public DiagramItemPaintControllerBase(DiagramControlViewInfo diagramViewInfo) {
			this.diagramViewInfo = diagramViewInfo;
		}
		public virtual void DrawItem(ControlGraphicsInfoArgs info, DiagramItemInfo itemInfo) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			DiagramItemObjectInfoArgs drawArgs = GetDrawArgs();
			drawArgs.Cache = info.Cache;
			drawArgs.Initialize(itemInfo, viewInfo);
			try {
				GetItemPainter().DrawObject(drawArgs);
			}
			finally {
				drawArgs.Cache = null;
				drawArgs.Clear();
			}
		}
		protected DiagramControlViewInfo DiagramViewInfo { get { return diagramViewInfo; } }
		protected abstract DiagramItemObjectInfoArgs GetDrawArgs();
		protected abstract DiagramItemPainterBase GetItemPainter();
	}
	public class DiagramContainerPaintController : DiagramItemPaintControllerBase {
		public DiagramContainerPaintController(DiagramControlViewInfo diagramViewInfo)
			: base(diagramViewInfo) {
		}
		public override void DrawItem(ControlGraphicsInfoArgs info, DiagramItemInfo itemInfo) {
			base.DrawItem(info, itemInfo);
			DrawChildren(info, itemInfo);
		}
		protected virtual void DrawChildren(ControlGraphicsInfoArgs info, DiagramItemInfo itemInfo) {
			ICollection visibleItems = ((DiagramContainerInfo)itemInfo).VisibleItems;
			if(visibleItems.Count == 0) return;
			foreach(DiagramItemInfo childItemInfo in visibleItems) {
				childItemInfo.PaintController.DrawItem(info, childItemInfo);
			}
		}
		protected override DiagramItemObjectInfoArgs GetDrawArgs() {
			return DiagramViewInfo.ContainerItemDrawArgs;
		}
		protected override DiagramItemPainterBase GetItemPainter() {
			return DiagramViewInfo.ContainerItemPainter;
		}
	}
	public class DiagramShapePaintController : DiagramItemPaintControllerBase {
		public DiagramShapePaintController(DiagramControlViewInfo diagramViewInfo)
			: base(diagramViewInfo) {
		}
		protected override DiagramItemObjectInfoArgs GetDrawArgs() {
			return DiagramViewInfo.ShapeDrawArgs;
		}
		protected override DiagramItemPainterBase GetItemPainter() {
			return DiagramViewInfo.ShapePainter;
		}
	}
	public class DiagramConnectorPaintController : DiagramItemPaintControllerBase {
		public DiagramConnectorPaintController(DiagramControlViewInfo diagramViewInfo)
			: base(diagramViewInfo) {
		}
		protected override DiagramItemObjectInfoArgs GetDrawArgs() {
			return DiagramViewInfo.ConnectorDrawArgs;
		}
		protected override DiagramItemPainterBase GetItemPainter() {
			return DiagramViewInfo.ConnectorPainter;
		}
	}
}

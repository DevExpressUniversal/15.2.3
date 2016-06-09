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
using System.Drawing;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.ViewInfo;
namespace DevExpress.XtraDiagram.Adorners {
	public class DiagramAdornerFactory : IAdornerFactory {
		DiagramControl diagram;
		public DiagramAdornerFactory(DiagramControl diagram) {
			this.diagram = diagram;
		}
		#region IAdorner
		IAdorner<ISelectionAdorner> IAdornerFactory.CreateSelectionAdorner(DefaultSelectionLayerHandler handler, double? selectionMargin) {
			return GetController().Create(() => new DiagramSelectionAdorner(handler, selectionMargin, GetViewInfo().SelectionSizeGripSize, Diagram.IsMultipleSelection(), GetViewInfo().GetRotationGrip()));
		}
		IAdorner IAdornerFactory.CreateItemDragPreview(IDiagramItem item) {
			return GetController().Create(() => new DiagramItemDragPreviewAdorner(item));
		}
		IAdorner IAdornerFactory.CreateBackground() {
			return GetController().Create(() => new DiagramBackgroundAdorner());
		}
		IAdorner IAdornerFactory.CreateHRuler() {
			return GetController().Create(() => new DiagramHRulerAdorner());
		}
		IAdorner IAdornerFactory.CreateVRuler() {
			return GetController().Create(() => new DiagramVRulerAdorner());
		}
		IAdorner IAdornerFactory.CreateHRulerShadow() {
			return GetController().Create(() => new DiagramHRulerShadowAdorner());
		}
		IAdorner IAdornerFactory.CreateVRulerShadow() {
			return GetController().Create(() => new DiagramVRulerShadowAdorner());
		}
		IAdorner IAdornerFactory.CreateHBoundsSnapLine(BoundsSnapLine snapLine) {
			return GetController().Create(() => new DiagramHBoundsSnapLineAdorner(snapLine));
		}
		IAdorner IAdornerFactory.CreateVBoundsSnapLine(BoundsSnapLine snapLine) {
			return GetController().Create(() => new DiagramVBoundsSnapLineAdorner(snapLine));
		}
		IAdorner IAdornerFactory.CreateHSizeSnapLine(SizeSnapLine snapLine) {
			return GetController().Create(() => new DiagramHSizeSnapLineAdorner(snapLine));
		}
		IAdorner IAdornerFactory.CreateVSizeSnapLine(SizeSnapLine snapLine) {
			return GetController().Create(() => new DiagramVSizeSnapLineAdorner(snapLine));
		}
		IAdorner IAdornerFactory.CreateSelectionPreview() {
			return GetController().Create(() => new DiagramSelectionPreviewAdorner());
		}
		IAdorner<ISelectionPartAdorner> IAdornerFactory.CreateSelectionPartAdorner(IDiagramItem item, bool isPrimarySelection) {
			return GetController().Create(() => new DiagramSelectionPartAdorner(item, isPrimarySelection));
		}
		IAdorner<IShapeParametersAdorner> IAdornerFactory.CreateShapeParametersAdorner(IDiagramShape shape) {
			return GetController().Create(() => new DiagramShapeParametersAdorner(shape, GetViewInfo().SelectionSizeGripSize));
		}
		IAdorner IAdornerFactory.CreateInplaceEditor(IDiagramItem item, Action onDestroy) {
			return GetController().Create(() => new DiagramInplaceEditorAdorner(item, GetViewInfo().CalcDiagramEditorBestSize((DiagramItem)item), onDestroy));
		}
		IAdorner<IConnectorAdorner> IAdornerFactory.CreateConnectorSelectionAdorner(IDiagramConnector connector) {
			return GetController().Create(() => new DiagramConnectorSelectionAdorner(connector, GetViewInfo().SelectionSizeGripSize, Diagram.IsMultipleSelection()));
		}
		IAdorner<IConnectorSelectionPartAdorner> IAdornerFactory.CreateConnectorSelectionPartAdorner(IDiagramConnector item, bool isPrimarySelection) {
			return GetController().Create(() => new DiagramConnectorSelectionPartAdorner(item, isPrimarySelection));
		}
		IAdorner IAdornerFactory.CreateConnectorDragPreview(ShapeGeometry shape) {
			return GetController().Create(() => new DiagramConnectorDragPreviewAdorner(shape));
		}
		IAdorner<IConnectorMovePointAdorner> IAdornerFactory.CreateConnectorMovePointPreview() {
			return GetController().Create(() => new DiagramConnectorPointDragPreviewAdorner());
		}
		IAdorner IAdornerFactory.CreateGlueToItemAdorner() {
			return GetController().Create(() => new DiagramGlueToItemAdorner());
		}
		IAdorner IAdornerFactory.CreateGlueToPointAdorner() {
			return GetController().Create(() => new DiagramGlueToPointAdorner(GetViewInfo().HotConnectionPointSize));
		}
		IAdorner<IConnectionPointsAdorner> IAdornerFactory.CreateConnectionPointsAdorner() {
			return GetController().Create(() => new DiagramConnectionPointsAdorner(GetViewInfo().ConnectionPointSize));
		}
		#endregion
		protected DiagramControlViewInfo GetViewInfo() {
			return Diagram.DiagramViewInfo;
		}
		protected DiagramAdornerController GetController() {
			return Diagram.DiagramViewInfo.AdornerController;
		}
		public DiagramControl Diagram { get { return diagram; } }
	}
}

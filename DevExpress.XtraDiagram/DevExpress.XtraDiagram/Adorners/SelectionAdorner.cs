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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.Utils;
using DevExpress.XtraDiagram.ViewInfo;
using PlatformRect = System.Windows.Rect;
namespace DevExpress.XtraDiagram.Adorners {
	public class DiagramSelectionAdorner : DiagramAdornerBase, IAdorner<ISelectionAdorner>, ISelectionAdorner, IDiagramSelectionSupports {
		bool canResize;
		bool canRotate;
		bool showFullUI;
		bool isMultiSelection;
		double? margins;
		Size gripSize;
		RotationGrip rotationGrip;
		DiagramItemSelection selection;
		DefaultSelectionLayerHandler layerHandler;
		public DiagramSelectionAdorner(DefaultSelectionLayerHandler layerHandler, double? margins, Size gripSize, bool isMultiSelection, RotationGrip rotationGrip) {
			this.canResize = false;
			this.canRotate = false;
			this.showFullUI = true;
			this.gripSize = gripSize;
			this.layerHandler = layerHandler;
			this.margins = margins;
			this.isMultiSelection = isMultiSelection;
			this.rotationGrip = rotationGrip;
		}
		protected override PlatformRect AdjustRect(PlatformRect rect) {
			PlatformRect newRect = rect;
			if(Margins.HasValue) {
				double inflateVal = -Margins.Value;
				newRect.Inflate(inflateVal, inflateVal);
			}
			return newRect;
		}
		public Size GripSize { get { return gripSize; } }
		public DiagramItemSelection Selection { get { return selection; } }
		public double? Margins { get { return margins; } }
		public bool IsMultiSelection { get { return isMultiSelection; } }
		protected override void OnChanged(DiagramAdornerChangedEventArgs e) {
			base.OnChanged(e);
			if(e.ChangeKind != DiagramAdornedChangeKind.Destroy) {
				UpdateSelection();
			}
		}
		public DefaultSelectionLayerHandler LayerHandler { get { return layerHandler; } }
		protected virtual void UpdateSelection() {
			this.selection = DiagramSelectionUtils.CalcSelection(Bounds, GripSize, RotationGrip);
		}
		public RotationGrip RotationGrip { get { return rotationGrip; } }
		protected override void UpdateBounds() {
			base.UpdateBounds();
			UpdateSelection();
		}
		public override bool AffectsSelection { get { return true; } }
		public override bool IsRotationSupports { get { return true; } }
		public bool CanResize { get { return canResize; } }
		public bool CanRotate { get { return canRotate; } }
		public bool InSizeGrip(Point pt) {
			return Selection.InSizeGrip(AdjustLogicalPoint(pt));
		}
		public bool InRotationGrip(Point pt) {
			Point point = LogicalPointToDisplayPoint(pt).DisplayPoint;
			return Selection.InRotationGrip(AdjustDisplayPoint(point));
		}
		public DiagramSizeGripItem GetSizeGripItem(Point pt) {
			return new DiagramSizeGripItem(selection.GetGripKind(AdjustLogicalPoint(pt)));
		}
		#region ISelectionAdorner
		bool ISelectionAdorner.CanResize {
			get { return canResize; }
			set { canResize = value; }
		}
		bool ISelectionAdorner.CanRotate {
			get { return canRotate; }
			set { canRotate = value; }
		}
		bool ISelectionAdorner.ShowFullUI {
			get { return this.showFullUI; }
			set { this.showFullUI = value; }
		}
		#endregion
		#region IAdorner<ISelectionAdorner>
		ISelectionAdorner IAdorner<ISelectionAdorner>.Model { get { return this; } }
		#endregion
		public override DiagramAdornerPainterBase GetPainter() {
			return new DiagramSelectionAdornerPainter();
		}
		public override DiagramAdornerObjectInfoArgsBase GetDrawArgs() {
			return new DiagramSelectionAdornerObjectInfoArgs();
		}
	}
}

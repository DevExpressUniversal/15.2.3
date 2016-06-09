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

using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.Layers {
	public class XtraLayersHostController : LayersHostController {
		public XtraLayersHostController(ILayersHost layersHost)
			: base(layersHost) {
		}
		public new Matrix CreateLogicToDisplayTransform() {
			Matrix matrix = base.CreateLogicToDisplayTransform().ToWinMatrix();
			return matrix.ApplyOffset(CalcXCoerceOffset(matrix), CalcYCoerceOffset(matrix));
		}
		public void SetOffset(Point scrollPos) {
			base.SetOffset(scrollPos.ToPlatformPoint());
		}
		public Point TransformToLogicPoint(Point pt) {
			return TransformToLogicPoint(pt.ToPlatformPoint()).ToWinPoint();
		}
		public Point TransformToDisplayPoint(Point point) {
			return TransformToDisplayPoint(point.ToPlatformPoint()).ToWinPoint();
		}
		protected int CalcXCoerceOffset(Matrix matrix) {
			return CalcCoerceOffsetCore(matrix, GetDiagramContentRect().X);
		}
		protected int CalcYCoerceOffset(Matrix matrix) {
			return CalcCoerceOffsetCore(matrix, GetDiagramContentRect().Y);
		}
		protected int CalcCoerceOffsetCore(Matrix matrix, int value) {
			Rectangle rect = new Rectangle(0, 0, value, 1);
			Point[] points = rect.GetPoints();
			matrix.TransformPoints(points);
			return RectangleUtils.FromPoints(points).Width - value;
		}
		protected int CalcValue(Matrix matrix, int value) {
			Rectangle rect = new Rectangle(0, 0, value, 1);
			Point[] points = rect.GetPoints();
			matrix.TransformPoints(points);
			return RectangleUtils.FromPoints(points).Width;
		}
		protected Rectangle GetDiagramContentRect() {
			return DiagramControl.DiagramViewInfo.ContentRect;
		}
		public DiagramControl DiagramControl { get { return (DiagramControl)Diagram; } }
	}
}

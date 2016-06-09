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
using System.Linq;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.Utils;
using PlatformPoint = System.Windows.Point;
namespace DevExpress.XtraDiagram.Adorners {
	public class DiagramConnectionPointsAdorner : DiagramAdornerBase, IAdorner<IConnectionPointsAdorner>, IConnectionPointsAdorner {
		Size pointSize;
		ShapedSelection selection;
		public DiagramConnectionPointsAdorner(Size rectSize) {
			this.selection = null;
			this.pointSize = rectSize;
		}
		public override bool IsRotationSupports { get { return true; } }
		#region IAdorner<IConnectionPointsAdorner>
		IConnectionPointsAdorner IAdorner<IConnectionPointsAdorner>.Model { get { return this; } }
		#endregion
		#region IConnectionPointsAdorner
		void IConnectionPointsAdorner.SetPoints(PlatformPoint[] points) {
			UpdateSelection(points);
		}
		#endregion
		protected void UpdateSelection(PlatformPoint[] points) {
			this.selection = CalcSelection(points);
		}
		protected virtual ShapedSelection CalcSelection(PlatformPoint[] points) {
			return new ShapedSelection(points.Select(point => GetDisplayRect(point, this.pointSize)));
		}
		protected DiagramElementBounds GetDisplayRect(PlatformPoint point, Size rectSize) {
			DiagramElementPoint displayPoint = LogicalPointToDisplayPoint(PointUtils.ApplyOffset(point.ToWinPoint(), LogicalBounds.Location));
			displayPoint.OffsetDisplay(-RotationOrigin.X, -RotationOrigin.Y);
			return displayPoint.CreateRect(rectSize);
		}
		public override DiagramAdornerObjectInfoArgsBase GetDrawArgs() {
			return new DiagramConnectionPointsAdornerObjectInfoArgs();
		}
		public override DiagramAdornerPainterBase GetPainter() {
			return new DiagramConnectionPointsAdornerPainter();
		}
		public ShapedSelection Selection { get { return selection; } }
	}
}

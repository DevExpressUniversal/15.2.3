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
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
namespace DevExpress.XtraDiagram.Parser {
	public class DiagramItemParseStrategy : ShapeParseStrategyBase {
		Point currentPt;
		Action<DiagramGraphicsPath> pathHandler;
		public DiagramItemParseStrategy() {
			this.currentPt = new Point();
		}
		public void BeginParse(Action<DiagramGraphicsPath> pathHandler) {
			this.pathHandler = pathHandler;
		}
		public void EndParse() {
			this.pathHandler = null;
		}
		protected Action<DiagramGraphicsPath> PathHandler { get { return pathHandler; } }
		DiagramGraphicsPath path = null;
		protected override void BeginFigureCore(Point startPt, bool isFilled, bool isClosed, bool isNewShape, StartSegmentStyle style) {
			if(isNewShape) {
				this.path = CreateDiagramGraphicsPath(isClosed, isFilled);
			}
			else {
				if(this.path != null && this.path.Closed) this.path.CloseFigure();
			}
			this.currentPt = startPt;
		}
		public override void LineTo(Point pt, bool isStroked) {
			this.path.AddLine((float)this.currentPt.X, (float)this.currentPt.Y, (float)pt.X, (float)pt.Y);
			this.currentPt = pt;
		}
		public override void ArcTo(Point pt, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked) {
			if(!pt.Equals(currentPt)) this.path.AddArc(this.currentPt, pt, size, sweepDirection.ToSpinDirection());
			this.currentPt = pt;
		}
		public override void BezierTo(Point point1, Point point2, Point point3, bool isStroked) {
			this.path.AddBezier(this.currentPt, point1, point2, point3);
			this.currentPt = point3;
		}
		public override void QuadraticBezierTo(Point point1, Point point2, bool isStroked) {
			this.path.AddQuadraticBezier(this.currentPt, point1, point2);
			this.currentPt = point2;
		}
		public override void EndFigure() {
			if(this.path == null) return;
			if(this.path.Closed) {
				this.path.CloseFigure();
			}
			if(this.path != null) {
				PathHandler(this.path);
			}
			this.path = null;
		}
		protected DiagramGraphicsPath CreateDiagramGraphicsPath(bool closed, bool filled) {
			return new DiagramGraphicsPath(new XtraGraphicsPath(), closed, filled);
		}
	}
}

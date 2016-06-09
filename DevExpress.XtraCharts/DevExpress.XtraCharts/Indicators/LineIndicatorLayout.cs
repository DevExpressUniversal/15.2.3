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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class LineIndicatorLayout : IndicatorLayout {
		readonly DiagramPoint startPoint;
		readonly DiagramPoint endPoint;
		public LineIndicatorLayout(Indicator indicator, GRealLine2D line) : base(indicator) {
			this.startPoint = DiagramPoint.Round((DiagramPoint)line.Start);
			this.endPoint = DiagramPoint.Round((DiagramPoint)line.End);
		}
		public override void Render(IRenderer renderer) {
			renderer.EnableAntialiasing(startPoint.X != endPoint.X && startPoint.Y != endPoint.Y);
			renderer.DrawLine((Point)startPoint, (Point)endPoint, Color, Indicator.LineStyle);
			renderer.RestoreAntialiasing();
		}
		public override GraphicsPath CalculateHitTestGraphicsPath() {
			if (startPoint == endPoint)
				return null;
			GraphicsPath path = new GraphicsPath();
			path.AddLine((PointF)startPoint, (PointF)endPoint);
			using (Pen pen = new Pen(Color.Empty, Thickness)) {
				if (Indicator.LineStyle != null)
					pen.LineJoin = Indicator.LineStyle.LineJoin;
				path.Widen(pen);
			}
			return path;
		}
	}
}

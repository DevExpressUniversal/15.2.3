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
using System.Collections.Generic;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public class GridLineViewData {
		public static void RenderGridLines(IRenderer renderer, IEnumerable<GridLineViewData> gridLinesViewData, Color color, LineStyle lineStyle) {
			foreach (GridLineViewData viewData in gridLinesViewData)
				viewData.Render(renderer, color, lineStyle);
		}
		DiagramPoint point1;
		DiagramPoint point2;
		public DiagramPoint Point1 { get { return point1; } }
		public DiagramPoint Point2 { get { return point2; } }
		public GridLineViewData(DiagramPoint point1, DiagramPoint point2) {
			this.point1 = point1;
			this.point2 = point2;
		}
		public void Render(IRenderer renderer, Color color, LineStyle lineStyle) {
			if (point1.IsZero || point2.IsZero)
				return;
			renderer.EnableAntialiasing(lineStyle.AntiAlias);
			renderer.DrawLine((Point)point1, (Point)point2, color, lineStyle);
			renderer.RestoreAntialiasing();
		}
	}
	public class RadarAxisYGridLineViewData {
		IPolygon polygon;
		public IPolygon Polygon { get { return polygon; } }
		public RadarAxisYGridLineViewData(IPolygon polygon) {
			this.polygon = polygon;
		}
	}
}

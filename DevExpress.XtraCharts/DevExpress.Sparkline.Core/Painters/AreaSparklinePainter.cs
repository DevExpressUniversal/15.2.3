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

using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.Sparkline.Core {
	public class AreaSparklinePainter : LineSparklinePainter {
		AreaSparklineView AreaView { get { return (AreaSparklineView)View; } }
		public override SparklineViewType SparklineType { get { return SparklineViewType.Area; } }
		protected override void DrawWholeGeometry(Graphics graphics, Color color, List<Point> points) {
			if (points.Count == 0)
				return;
			Color areaColor = Color.FromArgb(AreaView.AreaOpacity, color);
			if (points.Count == 1)
				graphics.DrawLine(GetPen(areaColor, AreaView.LineWidth), points[0], new Point(points[0].X, Mapping.ScreenYZeroValue));
			else {
				List<Point> areaPoints = new List<Point>(points);
				for (int i = points.Count - 1; i >= 0; i--)
					areaPoints.Add(new Point(points[i].X, Mapping.ScreenYZeroValue));
				SetAntialiasingMode(graphics, SmoothingMode.None);
				graphics.FillPolygon(GetSolidBrush(areaColor), areaPoints.ToArray());
				RestoreAntialiasingMode(graphics);
				base.DrawWholeGeometry(graphics, color, points);
			}
		}
	}
}

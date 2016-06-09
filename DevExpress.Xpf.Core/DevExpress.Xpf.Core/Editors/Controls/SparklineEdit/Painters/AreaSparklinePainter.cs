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
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors.Internal {
	public class AreaSparklinePainter : LineSparklinePainter {
		AreaSparklineControl AreaView { get { return (AreaSparklineControl)View; } }
		public override SparklineViewType SparklineType { get { return SparklineViewType.Area; } }
		private byte GetOpacity() {
			byte opacity;
			if (AreaView.ActualAreaOpacity > 1)
				opacity = 255;
			else if (AreaView.ActualAreaOpacity < 0)
				opacity = 0;
			else
				opacity = Convert.ToByte(AreaView.ActualAreaOpacity * 255);
			return opacity;
		}
		protected override void DrawWholeGeometry(DrawingContext drawingContext, SolidColorBrush brush, List<Point> points) {
			if (points.Count == 0)
				return;
			Color areaColor = Color.FromArgb(GetOpacity(), brush.Color.R, brush.Color.G, brush.Color.B);
			if (points.Count == 1)
				drawingContext.DrawLine(GetPen(areaColor, AreaView.ActualLineWidth), points[0], new Point(points[0].X, Mapping.ScreenYZeroValue));
			else {
				if (points.Count > 1) {
					SolidColorBrush solidColorBrush = GetSolidBrush(areaColor);
					Pen pen = GetPen(areaColor, 1);
					var yZeroValue = Math.Round(Mapping.ScreenYZeroValue) + 1;
					double correction = 1;
					List<LineSegment> lineSegments = new List<LineSegment>();
					double startX = Math.Round(points[0].X) - correction;
					lineSegments.Add(new LineSegment(new Point(startX, yZeroValue), false));
					lineSegments.Add(new LineSegment(new Point(startX, points[0].Y), false));
					for (int i = 0; i < points.Count; i++)
						lineSegments.Add(new LineSegment(points[i], false));
					double endX = Math.Round(points[points.Count - 1].X) + correction;
					lineSegments.Add(new LineSegment(new Point(endX, points[points.Count - 1].Y), false));
					lineSegments.Add(new LineSegment(new Point(endX, yZeroValue), false));
					lineSegments.Add(new LineSegment(new Point(startX, yZeroValue), false));
					PathGeometry pathGeometry = new PathGeometry();
					pathGeometry.Figures.Add(new PathFigure(new Point(startX, yZeroValue), lineSegments, true) { IsFilled = true });
					drawingContext.DrawGeometry(solidColorBrush, pen, pathGeometry);
				}
				base.DrawWholeGeometry(drawingContext, brush, points);
			}
		}
	}
}

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
using DevExpress.Compatibility.System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.Sparkline.Core {
	public class LineSparklinePainter : BaseSparklinePainter {
		LineSparklineView LineView { get { return (LineSparklineView)View; } }
		protected override bool EnableAntialiasing { get { return LineView.EnableAntialiasing; } }
		public override SparklineViewType SparklineType { get { return SparklineViewType.Line; } }
		void DrawMarkers(Graphics graphics, List<Point> points, List<int> pointsIndexes) {
			for (int i = 0; i < points.Count; i++) {
				PointPresentationType pointType = GetPointPresentationType(pointsIndexes[i]);
				if (pointType != PointPresentationType.SimplePoint || LineView.ShowMarkers) {
					Color pointColor = GetPointColor(pointType);
					float markerSize = GetMarkerSize(pointType);
					float halfMarkerSize = 0.5f * markerSize;
					graphics.FillEllipse(GetSolidBrush(pointColor), points[i].X - halfMarkerSize, points[i].Y - halfMarkerSize, markerSize, markerSize);
				}
			}
		}
		protected override Padding GetMarkersPadding() {
			double maxMarkerWidth = LineView.LineWidth;
			if (LineView.HighlightStartPoint)
				maxMarkerWidth = Math.Max(maxMarkerWidth, LineView.StartPointMarkerSize);
			if (LineView.HighlightEndPoint)
				maxMarkerWidth = Math.Max(maxMarkerWidth, LineView.EndPointMarkerSize);
			if (LineView.HighlightMaxPoint)
				maxMarkerWidth = Math.Max(maxMarkerWidth, LineView.MaxPointMarkerSize);
			if (LineView.HighlightMinPoint)
				maxMarkerWidth = Math.Max(maxMarkerWidth, LineView.MinPointMarkerSize);
			if (LineView.HighlightNegativePoints)
				maxMarkerWidth = Math.Max(maxMarkerWidth, LineView.NegativePointMarkerSize);
			if (LineView.ShowMarkers)
				maxMarkerWidth = Math.Max(maxMarkerWidth, LineView.MarkerSize);
			return new Padding((int)Math.Ceiling(0.5 * maxMarkerWidth));
		}
		protected virtual void DrawWholeGeometry(Graphics graphics, Color color, List<Point> points) {
			if (points.Count > 1)
				graphics.DrawLines(GetPen(color, LineView.LineWidth), points.ToArray());
		}
		protected override void DrawInternal(Graphics graphics) {
			List<Point> points = new List<Point>();
			List<int> pointsIndexes = new List<int>();
			int startIndex = GetIndexOfFirstPointForDrawing();
			int endIndex = GetIndexOfLastPointForDrawing();
			for (int i = startIndex; i <= endIndex; i++) {
				SparklinePoint point = DataProvider.SortedPoints[i];
				if (SparklineMathUtils.IsValidDouble(point.Value) && SparklineMathUtils.IsValidDouble(point.Argument)) {
					points.Add(Mapping.MapPoint(point.Argument, point.Value));
					pointsIndexes.Add(i);
				}
				else {
					DrawWholeGeometry(graphics, View.ActualColor, points);
					DrawMarkers(graphics, points, pointsIndexes);
					points.Clear();
					pointsIndexes.Clear();
				}
			}
			DrawWholeGeometry(graphics, View.ActualColor, points);
			DrawMarkers(graphics, points, pointsIndexes);
		}
		protected internal float GetMarkerSize(PointPresentationType pointType) {
			switch (pointType) {
				case PointPresentationType.HighPoint:
					return LineView.MaxPointMarkerSize;
				case PointPresentationType.LowPoint:
					return LineView.MinPointMarkerSize;
				case PointPresentationType.StartPoint:
					return LineView.StartPointMarkerSize;
				case PointPresentationType.EndPoint:
					return LineView.EndPointMarkerSize;
				case PointPresentationType.NegativePoint:
					return LineView.NegativePointMarkerSize;
				default:
					return LineView.MarkerSize;
			}
		}
		protected internal override Color GetPointColor(PointPresentationType pointType) {
			if (pointType == PointPresentationType.SimplePoint)
				return LineView.ActualMarkerColor;
			return base.GetPointColor(pointType);
		}
	}
}

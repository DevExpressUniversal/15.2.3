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
using System.Windows.Forms;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors.Internal {
	public class LineSparklinePainter : BaseSparklinePainter {
		LineSparklineControl LineView { get { return (LineSparklineControl)View; } }
		protected override bool EnableAntialiasing { get { return true; } }
		public override SparklineViewType SparklineType { get { return SparklineViewType.Line; } }
		void DrawMarkers(DrawingContext drawingContext, List<Point> points, List<int> pointsIndexes) {
			for(int i = 0; i < points.Count; i++) {
				PointPresentationType pointType = GetPointPresentationType(pointsIndexes[i]);
				if(pointType != PointPresentationType.SimplePoint || LineView.ActualShowMarkers) {
					float markerSize = GetMarkerSize(pointType);
					float halfMarkerSize = 0.5f * markerSize;
					SolidColorBrush solidColorBrush = GetPointBrush(pointType);
					Pen pen = GetPen(solidColorBrush, 1);
					drawingContext.DrawEllipse(solidColorBrush, pen, points[i], (double)halfMarkerSize, (double)halfMarkerSize);
				}
			}
		}
		float GetMarkerSize(PointPresentationType pointType) {
			switch(pointType) {
				case PointPresentationType.HighPoint:
					return LineView.ActualMaxPointMarkerSize;
				case PointPresentationType.LowPoint:
					return LineView.ActualMinPointMarkerSize;
				case PointPresentationType.StartPoint:
					return LineView.ActualStartPointMarkerSize;
				case PointPresentationType.EndPoint:
					return LineView.ActualEndPointMarkerSize;
				case PointPresentationType.NegativePoint:
					return LineView.ActualNegativePointMarkerSize;
				default:
					return LineView.ActualMarkerSize;
			}
		}
		protected virtual void DrawWholeGeometry(DrawingContext drawingContext, SolidColorBrush brush, List<Point> points) {
			if (points.Count > 1) {
				Pen pen = GetPen(brush, LineView.ActualLineWidth);
				for (int i = 0; i < points.Count - 1; i++)
					drawingContext.DrawLine(pen, points[i], points[i + 1]);
			}
		}
		protected override SolidColorBrush GetPointBrush(PointPresentationType pointType) {
			if(pointType == PointPresentationType.SimplePoint)
				return LineView.ActualMarkerBrush;
			return base.GetPointBrush(pointType);
		}
		protected override void DrawInternal(DrawingContext drawingContext) {
			List<Point> points = new List<Point>();
			List<int> pointsIndexes = new List<int>();
			Point lastInvisible = new Point();
			int lastInvisibleIndex = -1;
			for(int i = 0; i < Data.Count; i++) {
				double value = Data[i].Value;
				if(SparklineMathUtils.IsValidDouble(value)) {
					double x = Mapping.MapXValueToScreen(Data[i].Argument);
					double y = Mapping.MapYValueToScreen(value);
					if (!Mapping.isPointVisible(Data[i].Argument) && points.Count == 0) {
						lastInvisible = new Point(x, y);
						lastInvisibleIndex = i;
						continue;
					}
					if (lastInvisibleIndex >= 0) {
						points.Add(lastInvisible);
						pointsIndexes.Add(lastInvisibleIndex);
						lastInvisibleIndex = -1;
					}
					points.Add(new Point(x, y));
					pointsIndexes.Add(i);
					if (!Mapping.isPointVisible(Data[i].Argument))
						break;
				}
				else {
					DrawWholeGeometry(drawingContext, View.ActualBrush, points);
					DrawMarkers(drawingContext, points, pointsIndexes);
					points.Clear();
					pointsIndexes.Clear();
				}
			}
			DrawWholeGeometry(drawingContext, View.ActualBrush, points);
			DrawMarkers(drawingContext, points, pointsIndexes);
		}
	}
}

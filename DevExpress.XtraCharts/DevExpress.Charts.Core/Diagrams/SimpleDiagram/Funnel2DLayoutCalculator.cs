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

using DevExpress.Charts.Native;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.Charts.Native {
	public class FunnelPointInfo {
		public GRealPoint2D TopLeftPoint { get; set; }
		public GRealPoint2D TopRightPoint { get; set; }
		public GRealPoint2D BottomLeftPoint { get; set; }
		public GRealPoint2D BottomRightPoint { get; set; }
	}
	public class FunnelLableInfo {
		readonly RefinedPoint refinedPoint;
		readonly GRealSize2D labelSize;
		readonly FunnelPointInfo pointInfo;
		public RefinedPoint RefinedPoint { get { return refinedPoint; } }
		public GRealSize2D LabelSize { get { return labelSize; } }
		public FunnelPointInfo PointInfo { get { return pointInfo; } }
		public FunnelLableInfo(RefinedPoint refinedPoint, GRealSize2D labelSize, FunnelPointInfo pointInfo) {
			this.refinedPoint = refinedPoint;
			this.labelSize = labelSize;
			this.pointInfo = pointInfo;
		}
	}
	public class Funnel2DLayoutCalculator {
		readonly GRealSize2D bounds;
		readonly IList<RefinedPoint> points;
		readonly double centerX;
		readonly double actualPointDistance;
		readonly double heightOfPolygon;
		public static GRealPoint2D? CalcIntersectionPoint(GRealPoint2D p1, GRealPoint2D p2, double y) {
			if (p1 == p2)
				return null;
			double x1 = p1.X;
			double x2 = p2.X;
			double y1 = p1.Y;
			double y2 = p2.Y;
			if (y1 == y2)
				return null;
			if (x2 == x1)
				return new GRealPoint2D(x1, y);
			else {
				double x = (y - y1) / (y2 - y1) * (x2 - x1) + x1;
				return new GRealPoint2D(x, y);
			}
		}
		public Funnel2DLayoutCalculator(GRealSize2D bounds, IList<RefinedPoint> points, double pointDistance) {
			this.points = points;
			this.bounds = bounds;
			this.centerX = bounds.Width / 2;
			if (points.Count > 0) {
				double maxAvailibleSumOfPointDistances = bounds.Height - points.Count;
				double sumOfPointDistances = (points.Count - 1) * pointDistance;
				if (pointDistance != 0 && maxAvailibleSumOfPointDistances < sumOfPointDistances) {
					this.actualPointDistance = maxAvailibleSumOfPointDistances / (points.Count - 1);
					this.heightOfPolygon = 1;
				}
				else {
					this.heightOfPolygon = (bounds.Height - sumOfPointDistances) / points.Count;
					this.actualPointDistance = pointDistance;
				}
			}
		}
		FunnelPointInfo CalculateSeriesPointLayout(IFunnelPoint point, IFunnelPoint bottomPoint, double topY) {
			double bottomY = topY + heightOfPolygon;
			if (point == null)
				return null;
			double topWidth = point.NormalizedValue * bounds.Width;
			double bottomWidth = (bottomPoint == null) ? topWidth : bottomPoint.NormalizedValue * bounds.Width;
			return new FunnelPointInfo() { 
				TopLeftPoint = new GRealPoint2D(centerX - topWidth / 2, topY),
				TopRightPoint = new GRealPoint2D(centerX + topWidth / 2, topY),
				BottomRightPoint = CalcBottomPoint(topY, bottomY, topWidth, bottomWidth, false),
				BottomLeftPoint = CalcBottomPoint(topY, bottomY, topWidth, bottomWidth, true)
			};
		}
		GRealPoint2D CalcBottomPoint(double topY, double bottomY, double topWidth, double bottomWidth, bool isLeftPoint) {
			int sign = isLeftPoint ? -1 : 1;
			GRealPoint2D topPoint = new GRealPoint2D(centerX + sign * topWidth / 2, topY);
			GRealPoint2D bottomPoint = new GRealPoint2D(centerX + sign * bottomWidth / 2, bottomY + actualPointDistance);
			GRealPoint2D? realPoint = CalcIntersectionPoint(topPoint, bottomPoint, bottomY);
			if (realPoint.HasValue)
				return realPoint.Value;
			return new GRealPoint2D(centerX - topWidth / 2, topY);
		}
		public List<FunnelPointInfo> Calculate() {
			if (points.Count == 0 || bounds.Height < points.Count)
				return null;
			double topY = 0;
			List<FunnelPointInfo> layouts = new List<FunnelPointInfo>();
			for (int i = 0; i < points.Count; i++) {
				RefinedPoint bottomPoint = (i + 1) < points.Count ? points[i + 1] : null;
				layouts.Add(CalculateSeriesPointLayout(points[i], bottomPoint, topY));
				topY += heightOfPolygon + actualPointDistance;
			}
			return layouts;
		}
	}
}

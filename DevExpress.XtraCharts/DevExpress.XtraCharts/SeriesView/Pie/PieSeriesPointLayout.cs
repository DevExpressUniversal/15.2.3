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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public class PieSeriesPointLayout : SeriesPointLayout {
		Rectangle bounds;
		Pie pie;
		PointF basePoint;
		RectangleF pieBounds;
		int borderThickness;
		bool negativeValuesPresents;
		int positiveValuesCount;
		public Rectangle Bounds {
			get { return bounds; }
		}
		public Pie Pie {
			get { return pie; }
		}
		public PointF BasePoint {
			get { return basePoint; }
		}
		public RectangleF PieBounds {
			get { return pieBounds; }
		}
		public bool NegativeValuesPresents {
			get { return negativeValuesPresents; }
		}
		public int PositiveValuesCount {
			get { return positiveValuesCount; }
		}
		public PieSeriesPointLayout(RefinedPointData pointData, Rectangle bounds, Pie pie, PointF basePoint, RectangleF pieBounds, int borderThickness, bool negativeValuesPresents, int positiveValuesCount)
			: base(pointData) {
			this.bounds = bounds;
			this.pie = pie;
			this.basePoint = basePoint;
			this.pieBounds = pieBounds;
			this.borderThickness = borderThickness;
			this.negativeValuesPresents = negativeValuesPresents;
			this.positiveValuesCount = positiveValuesCount;
		}
		SizeF CalculatePathSize(GraphicsPath path) {
			if (path.PointCount == 0)
				return SizeF.Empty;
			float minX, maxX;
			float minY, maxY;
			minX = maxX = path.PathPoints[0].X;
			minY = maxY = path.PathPoints[0].Y;
			for (int i = 1; i < path.PointCount; i++) {
				PointF point = path.PathPoints[i];
				minX = Math.Min(minX, point.X);
				maxX = Math.Max(maxX, point.X);
				minY = Math.Min(minY, point.Y);
				maxY = Math.Max(maxY, point.Y);
			}
			return new SizeF(maxX - minX, maxY - minY);
		}
		public override HitRegionContainer CalculateHitRegion() {
			HitRegionContainer region = base.CalculateHitRegion();
			if (pie.HolePercent == 100) {
				GraphicsPath path = new GraphicsPath();
				path.AddArc(pieBounds, pie.StartAngleInDegrees, pie.SweepAngleInDegrees);
				SizeF pathSize = CalculatePathSize(path);
				if (pathSize.Width > 1 || pathSize.Height > 1) {
					using (Pen pen = new Pen(Color.Empty, 1))
						path.Widen(pen);
					region.Union(new HitRegion(path));
				}
				else
					path.Dispose();
			}
			else {
				GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(
					GraphicUtils.CalcCenter((ZPlaneRectangle)pieBounds, false),
					pie.MajorSemiaxis,
					pie.MinorSemiaxis,
					pie.HoleFraction,
					pie.StartAngleInDegrees,
					pie.SweepAngleInDegrees);
				region.Union(new HitRegion(path));
			}
			return region;
		}
	}
}

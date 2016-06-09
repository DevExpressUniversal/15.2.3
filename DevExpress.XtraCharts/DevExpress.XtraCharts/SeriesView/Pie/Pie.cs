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
	public class Pie {
		double startAngle;
		double finishAngle;
		Ellipse ellipse;
		int depthPercent;
		double holePercent;
		double sweepAngle;
		double halfAngle;
		GRealPoint2D finishPoint;
		RectangleF anchorBounds;
		float depth;
		float holeFraction;
		public double StartAngle { 
			get { return startAngle; } 
		}
		public double HalfAngle { 
			get { return halfAngle; } 
		}
		public double FinishAngle { 
			get { return finishAngle; } 
		}
		public GRealPoint2D FinishPoint { 
			get { return finishPoint; } 
		}
		public GRealPoint2D CenterPoint { 
			get { return ellipse.Center; } 
		}
		public float StartAngleInDegrees { 
			get { return -(float)MathUtils.Radian2Degree(startAngle); } 
		}
		public float SweepAngleInDegrees { 
			get { return -(float)MathUtils.Radian2Degree(sweepAngle); } 
		}
		public Ellipse Ellipse { 
			get { return ellipse; } 
		}
		public float MajorSemiaxis { 
			get { return (float)ellipse.MajorSemiaxis; } 
		}
		public float MinorSemiaxis { 
			get { return (float)ellipse.MinorSemiaxis; } 
		}
		public float HoleFraction { 
			get { return holeFraction; } 
		}
		public int DepthPercent { 
			get { return depthPercent; } 
		}
		public double HolePercent { 
			get { return holePercent; } 
		}
		public Pie(double startAngle, double finishAngle, Ellipse ellipse, int depthPercent, double holePercent) {
			this.startAngle = startAngle;
			this.finishAngle = finishAngle;
			this.ellipse = ellipse;
			this.depthPercent = depthPercent;
			this.holePercent = holePercent;
			sweepAngle = finishAngle - startAngle;
			halfAngle = ellipse.CalcEllipseSectorFinishAngle(ellipse.CalcEllipseSectorArea(startAngle, finishAngle) / 2, startAngle);
			finishPoint = ellipse.CalcEllipsePoint(finishAngle);
			depth = (float)(MajorSemiaxis * depthPercent / 50.0f);
			holeFraction = (float)(holePercent / 100.0);
			bool isDoughnut = holePercent > 0;
			LineStrip vertices = GraphicUtils.FillAnchorPoints(ellipse, startAngle, sweepAngle, !isDoughnut);
			if (isDoughnut) {
				Ellipse innerEllipse = new Ellipse(ellipse.Center,
					ellipse.MajorSemiaxis * holeFraction, ellipse.MinorSemiaxis * holeFraction);
				vertices.AddRange(GraphicUtils.FillAnchorPoints(innerEllipse, startAngle, sweepAngle, false));
			}
			anchorBounds = GraphicUtils.BoundsFromPointsArray(vertices);
		}
		public bool ShouldCreateGraphicsCommand() {
			return MajorSemiaxis >= 0.5f && MinorSemiaxis >= 0.5f &&
				SweepAngleInDegrees != 0 && anchorBounds.Width > 0.0f && anchorBounds.Height > 0.0f;
		}
		public GRealPoint2D CalculateCenter(PointF basePoint) {
			return new GRealPoint2D(ellipse.Center.X + basePoint.X, ellipse.Center.Y + basePoint.Y);
		}
		public GraphicsCommand CreateGraphicsCommand(Color color, Color color2, FillStyleBase fillStyle, PointF basePoint) {
			RectangleF bounds = anchorBounds;
			bounds.Offset(basePoint);
			GraphicsCommand command = new SimpleAntialiasingGraphicsCommand();
			command.AddChildCommand(fillStyle.Options.CreatePieGraphicsCommand(CalculateCenter(basePoint),
				MajorSemiaxis, MinorSemiaxis, StartAngleInDegrees, SweepAngleInDegrees, depth, holeFraction, bounds, color, color2));
			return command;
		}
		public void Render(IRenderer renderer, Color color, Color color2, FillStyleBase fillStyle, PointF basePoint) {
			RectangleF bounds = anchorBounds;
			bounds.Offset(basePoint);
			renderer.EnableAntialiasing(true);
			GRealPoint2D pieCenter = CalculateCenter(basePoint);
			fillStyle.Options.RenderPie(renderer, new PointF((float)pieCenter.X, (float)pieCenter.Y), MajorSemiaxis, MinorSemiaxis,
			StartAngleInDegrees, SweepAngleInDegrees, depth, holeFraction, bounds, color, color2);
			renderer.RestoreAntialiasing();
		}
		public GraphicsCommand CreateBorderGraphicsCommand(Color borderColor, int thickness, PointF basePoint) {
			GraphicsCommand command = new SimpleAntialiasingGraphicsCommand();
			GRealPoint2D centerPoint = CalculateCenter(basePoint);
			if (thickness <= 1) {
				command.AddChildCommand(new BoundedPieGraphicsCommand(centerPoint, MajorSemiaxis, MinorSemiaxis,
					StartAngleInDegrees, SweepAngleInDegrees, depth, holeFraction, borderColor, thickness));
				return command;
			}
			Region region;
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(centerPoint, MajorSemiaxis, MinorSemiaxis,
				holeFraction, StartAngleInDegrees, SweepAngleInDegrees)) {
				region = new Region(path);
			}
			GraphicsCommand clipping = new ClippingGraphicsCommand(region);
			clipping.AddChildCommand(new BoundedPieGraphicsCommand(centerPoint, MajorSemiaxis, MinorSemiaxis,
					StartAngleInDegrees, SweepAngleInDegrees, depth, holeFraction, borderColor, thickness));
			clipping.AddChildCommand(new BoundedPieGraphicsCommand(centerPoint, MajorSemiaxis, MinorSemiaxis,
				0, -360, depth, holeFraction, borderColor, thickness * 2));
			command.AddChildCommand(clipping);
			command.AddChildCommand(new BoundedPieGraphicsCommand(centerPoint, MajorSemiaxis, MinorSemiaxis,
				StartAngleInDegrees, SweepAngleInDegrees, depth, holeFraction, borderColor, 1));
			return command;
		}
		public void RenderBorder(IRenderer renderer, Color borderColor, int thickness, PointF basePoint) {
			renderer.EnableAntialiasing(true);
			GRealPoint2D centerPoint = CalculateCenter(basePoint);
			if (thickness <= 1)
				renderer.DrawPie(this, basePoint, borderColor, thickness);
			else {
				using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(centerPoint, MajorSemiaxis, MinorSemiaxis, holeFraction, StartAngleInDegrees, SweepAngleInDegrees)) {
					renderer.SetClipping(path);
					renderer.DrawPie(this, basePoint, borderColor, thickness * 2);
					renderer.RestoreClipping();
					renderer.DrawPie(this, basePoint, borderColor, 1);
				}
			}
			renderer.RestoreAntialiasing();
		}
		public GraphicsCommand CreateHatchGraphicsCommand(Color color, PointF basePoint) {
			return new HatchedPieGraphicsCommand(CalculateCenter(basePoint), MajorSemiaxis, MinorSemiaxis,
				StartAngleInDegrees, SweepAngleInDegrees, depth, holeFraction, HatchStyle.WideUpwardDiagonal, color, Color.Empty);
		}
		public void RenderHatch(IRenderer renderer, Color color, PointF basePoint) {
			renderer.FillPie(this, basePoint, HatchStyle.WideUpwardDiagonal, color, Color.Empty);
		}
	}
}

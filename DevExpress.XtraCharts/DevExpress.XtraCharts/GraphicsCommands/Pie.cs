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
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {
	public abstract class PieGraphicsCommand : GraphicsCommand {
		public const float FacetPercent = 0.02f;
		GRealPoint2D center;
		float majorSemiAxis;
		float minorSemiAxis;
		float startAngle;
		float sweepAngle;
		float depth;
		float holePercent;
		float innerMajorSemiAxis;
		float innerMinorSemiAxis;
		float radius;
		float innerRadius;
		float facetSize;
		float correctedDepth;
		bool isDoughnut;
		protected GRealPoint2D Center { get { return center; } }
		protected float MajorSemiAxis { get { return majorSemiAxis; } }
		protected float MinorSemiAxis { get { return minorSemiAxis; } }
		protected float StartAngle { get { return startAngle; } }
		protected float SweepAngle { get { return sweepAngle; } }
		protected float Depth { get { return depth; } }
		protected float HolePercent { get { return holePercent; } }
		protected float InnerMajorSemiAxis { get { return innerMajorSemiAxis; } }
		protected float InnerMinorSemiAxis { get { return innerMinorSemiAxis; } }
		protected float FacetSize { get { return facetSize; } }
		protected float Radius { get { return radius; } }
		protected float InnerRadius { get { return innerRadius; } }
		protected float CorrectedDepth { get { return correctedDepth; } }
		protected bool IsDoughnut { get { return isDoughnut; } }
		public PieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent) {
			this.center = center;
			this.majorSemiAxis = majorSemiAxis;
			this.minorSemiAxis = minorSemiAxis;
			this.startAngle = startAngle;
			this.sweepAngle = sweepAngle;
			this.depth = depth;
			this.holePercent = holePercent;
			if (holePercent > 0.0f) {
				isDoughnut = true;
				float doughnutThickness = minorSemiAxis * (1.0f - holePercent);
				innerMajorSemiAxis = majorSemiAxis - doughnutThickness;
				innerMinorSemiAxis = minorSemiAxis - doughnutThickness;
			}
			if (depth > 0.0f) {
				facetSize = majorSemiAxis * FacetPercent;
				if (facetSize > depth / 2.0f)
					facetSize = depth / 2.0f;
				radius = majorSemiAxis - facetSize;
				float diff = radius - innerMajorSemiAxis;
				if (diff < 0.0f) {
					radius -= diff;
					facetSize += diff;
				}
				correctedDepth = depth - facetSize * 2;
				if (isDoughnut) {
					innerRadius = innerMajorSemiAxis + facetSize;
					if (innerRadius > radius)
						innerRadius = radius;
				}
			}
		}
		public void Execute(Graphics gr, Brush brush) {
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(Center, majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				gr.FillPath(brush, path);
		}
		void PerformPieDrawing() {
			ExecutePie();
			if (depth != 0.0) {
				GL.PushMatrix();
				try {
					GL.Translated(0.0, 0.0, depth);
					ExecutePie();
				}
				finally {
					GL.PopMatrix();
				}
			}
			if (facetSize > 0.0f) {
				ExecuteFacet(Radius, MajorSemiAxis);
				GL.PushMatrix();
				try {
					GL.Translated(0.0, 0.0, facetSize);
					if (correctedDepth > 0) {
						ExecuteCylinder(MajorSemiAxis);
						GL.Translated(0.0, 0.0, correctedDepth);
					}
					ExecuteFacet(MajorSemiAxis, Radius);
				}
				finally {
					GL.PopMatrix();
				}
			}
			else
				ExecuteCylinder(MajorSemiAxis);
			ExecuteSections(0.0f, MajorSemiAxis);
		}
		void PerformDoughnutDrawing() {
			if (innerRadius < radius) {
				ExecuteDoughnut();
				if (depth != 0.0) {
					GL.PushMatrix();
					try {
						GL.Translated(0.0, 0.0, depth);
						ExecuteDoughnut();
					}
					finally {
						GL.PopMatrix();
					}
				}
			}
			if (facetSize > 0.0f) {
				ExecuteFacet(Radius, MajorSemiAxis);
				ExecuteFacet(InnerRadius, InnerMajorSemiAxis);
				GL.PushMatrix();
				try {
					GL.Translated(0.0, 0.0, facetSize);
					if (correctedDepth > 0) {
						ExecuteCylinder(MajorSemiAxis);
						ExecuteCylinder(InnerMajorSemiAxis);
						GL.Translated(0.0, 0.0, correctedDepth);
					}
					ExecuteFacet(MajorSemiAxis, Radius);
					ExecuteFacet(InnerMajorSemiAxis, InnerRadius);
				}
				finally {
					GL.PopMatrix();
				}
			}
			else {
				ExecuteCylinder(MajorSemiAxis);
				ExecuteCylinder(InnerMajorSemiAxis);
			}
			ExecuteSections(InnerMajorSemiAxis, MajorSemiAxis);
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.MatrixMode(GL.MODELVIEW);
			GL.PushMatrix();
			try {
				GL.Translated(center.X, -center.Y, -depth * 0.5);
				if (isDoughnut)
					PerformDoughnutDrawing();
				else
					PerformPieDrawing();
			}
			finally {
				GL.PopMatrix();
			}	   
		}
		protected abstract void ExecuteSections(float innerRadius, float outerRadius);
		protected abstract void ExecutePie();
		protected abstract void ExecuteDoughnut();
		protected abstract void ExecuteCylinder(float radius);
		protected abstract void ExecuteFacet(float radius1, float radius2);
	}
	public class BoundedPieGraphicsCommand : PieGraphicsCommand {
		Color color;
		int thickness;
		public BoundedPieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, Color color, int thickness) : base(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent) {
			this.color = color;
			this.thickness = thickness;
		}
		protected override void ExecuteSections(float innerRadius, float outerRadius) {
		}
		protected override void ExecutePie() {
		}
		protected override void ExecuteDoughnut() {
		}
		protected override void ExecuteCylinder(float radius) {
		}
		protected override void ExecuteFacet(float radius1, float radius2) {
		}
	}
	public class SolidPieGraphicsCommand : PieGraphicsCommand {
		Color color;
		public SolidPieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, Color color) : base(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent) {
			this.color = color;
		}
		protected override void ExecuteSections(float innerRadius, float outerRadius) {
			GL.Color4ub(color.R, color.G, color.B, color.A);
			GLHelper.PieSections(Depth, innerRadius, outerRadius, FacetSize, -StartAngle, -(StartAngle + SweepAngle));
		}
		protected override void ExecutePie() {
			GL.Color4ub(color.R, color.G, color.B, color.A);
			GLHelper.PartialDisk(Radius, -StartAngle, -SweepAngle);
		}
		protected override void ExecuteDoughnut() {
			GL.Color4ub(color.R, color.G, color.B, color.A);
			GLHelper.PartialDisk(InnerRadius, Radius, -StartAngle, -SweepAngle);
		}
		protected override void ExecuteCylinder(float radius) {
			GL.Color4ub(color.R, color.G, color.B, color.A);
			GLHelper.PartialCylinder(CorrectedDepth, radius, -StartAngle, -SweepAngle);
		}
		protected override void ExecuteFacet(float radius1, float radius2) {
			GL.Color4ub(color.R, color.G, color.B, color.A);
			GLHelper.PartialCone(FacetSize, radius1, radius2, -StartAngle, -SweepAngle);
		}
	}
	public class GradientPieGraphicsCommand : PieGraphicsCommand {
		RectangleF gradientBounds;
		Color color;
		Color color2;
		LinearGradientMode gradientMode;
		public GradientPieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2, LinearGradientMode gradientMode) : base(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent) {
			this.gradientBounds = gradientBounds;
			this.color = color;
			this.color2 = color2;
			this.gradientMode = gradientMode;
		}
		protected override void ExecuteSections(float innerRadius, float outerRadius) {
			GLHelper.PieSections(Depth, innerRadius, outerRadius, FacetSize, -StartAngle, -(StartAngle + SweepAngle), 
				GLHelper.Create1DTexture(color, color2), new GLHelper.Gen1DTextureCoord(TextureGenerator));
		}
		protected override void ExecutePie() {
			GLHelper.PartialDisk(Radius, -StartAngle, -SweepAngle, 
				GLHelper.Create1DTexture(color, color2), new GLHelper.Gen1DTextureCoord(TextureGenerator));
		}
		protected override void ExecuteDoughnut() {
			GLHelper.PartialDisk(InnerRadius, Radius, -StartAngle, -SweepAngle, 
				GLHelper.Create1DTexture(color, color2), new GLHelper.Gen1DTextureCoord(TextureGenerator));
		}
		protected override void ExecuteCylinder(float radius) {
			GLHelper.PartialCylinder(CorrectedDepth, radius, -StartAngle, -SweepAngle, 
				GLHelper.Create1DTexture(color, color2), new GLHelper.Gen1DTextureCoord(TextureGenerator));
		}
		protected override void ExecuteFacet(float radius1, float radius2) {
			GLHelper.PartialCone(FacetSize, radius1, radius2, -StartAngle, -SweepAngle, 
				GLHelper.Create1DTexture(color, color2), new GLHelper.Gen1DTextureCoord(TextureGenerator));
		}
		int TextureGenerator(double x, double y, double z) {
			float coord;
			RectangleF relativeBounds = gradientBounds;
			relativeBounds.Offset(-(float)Center.X, -(float)Center.Y);
			switch (gradientMode) {
			case LinearGradientMode.Horizontal:
				coord = (float)(-(relativeBounds.Left - x) / relativeBounds.Width);
				break;
			case LinearGradientMode.Vertical:
				coord = (float)(-(relativeBounds.Top + y) / relativeBounds.Height);
				break;
			case LinearGradientMode.ForwardDiagonal:
				coord = (float)((-(relativeBounds.Top + y) * relativeBounds.Height - (relativeBounds.Left - x) * relativeBounds.Width)) / (relativeBounds.Width * relativeBounds.Width + relativeBounds.Height * relativeBounds.Height);
				break;
			case LinearGradientMode.BackwardDiagonal:
				coord = 1.0f - (float)(((relativeBounds.Height + relativeBounds.Top + y) * relativeBounds.Height - (relativeBounds.Left - x) * relativeBounds.Width)) / (relativeBounds.Width * relativeBounds.Width + relativeBounds.Height * relativeBounds.Height);
				break;
			default:
				coord = 0.0f;
				break;
			}
			try {
				return GLHelper.ConvertTexCoord(coord);
			}
			catch {
				return GLHelper.MidTexCoord;
			}
		}
	}
	public class CenterGradientPieGraphicsCommand : PieGraphicsCommand {
		Color color;
		Color color2;
		float textureDiff;
		public CenterGradientPieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, Color color, Color color2) : base(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent) {
			this.color = color;
			this.color2 = color2;
			if (IsDoughnut)
				textureDiff = MajorSemiAxis - InnerMajorSemiAxis;
		}
		protected override void ExecuteSections(float innerRadius, float outerRadius) {
			GLHelper.PieSections(Depth, innerRadius, outerRadius, FacetSize, -StartAngle, -(StartAngle + SweepAngle), 
				GLHelper.Create1DTexture(color, color2), CreateTextureGenerator());
		}
		protected override void ExecutePie() {
			GLHelper.PartialDisk(Radius, -StartAngle, -SweepAngle, 
				GLHelper.Create1DTexture(color, color2), CreateTextureGenerator());
		}
		protected override void ExecuteDoughnut() {
			GLHelper.PartialDisk(InnerRadius, Radius, -StartAngle, -SweepAngle, 
				GLHelper.Create1DTexture(color, color2), CreateTextureGenerator());
		}
		protected override void ExecuteCylinder(float radius) {
			GLHelper.PartialCylinder(CorrectedDepth, radius, -StartAngle, -SweepAngle, 
				GLHelper.Create1DTexture(color, color2), CreateTextureGenerator());
		}
		protected override void ExecuteFacet(float radius1, float radius2) {
			GLHelper.PartialCone(FacetSize, radius1, radius2, -StartAngle, -SweepAngle, 
				GLHelper.Create1DTexture(color, color2), CreateTextureGenerator());
		}
		int PieTextureGenerator(double x, double y, double z) {
			return GLHelper.ConvertTexCoord((float)(Math.Sqrt(x * x + y * y) / MajorSemiAxis));
		}
		int DoughnutTextureGenerator(double x, double y, double z) {
			return GLHelper.ConvertTexCoord((float)((Math.Sqrt(x * x + y * y) - InnerMajorSemiAxis) / textureDiff));
		}
		GLHelper.Gen1DTextureCoord CreateTextureGenerator() {
			return IsDoughnut ? new GLHelper.Gen1DTextureCoord(DoughnutTextureGenerator) : new GLHelper.Gen1DTextureCoord(PieTextureGenerator);
		}
	}
	public class HatchedPieGraphicsCommand : PieGraphicsCommand {
		Color color;
		Color backColor;
		HatchStyle hatchStyle;
		public HatchedPieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, HatchStyle hatchStyle, Color color, Color backColor) : base(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent) {
			this.hatchStyle = hatchStyle;
			this.color = color;
			this.backColor = backColor;
		}
		protected override void ExecuteSections(float innerRadius, float outerRadius) {
		}
		protected override void ExecutePie() {
		}
		protected override void ExecuteDoughnut() {
		}
		protected override void ExecuteCylinder(float radius) {
		}
		protected override void ExecuteFacet(float radius1, float radius2) {
		}
	}
}

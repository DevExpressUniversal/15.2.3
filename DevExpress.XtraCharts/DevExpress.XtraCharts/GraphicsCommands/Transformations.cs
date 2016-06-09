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
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {
	public class SaveStateGraphicsCommand : GraphicsCommand {
		public SaveStateGraphicsCommand() {
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			GL.MatrixMode(GL.MODELVIEW);
			GL.PushMatrix();
			GL.MatrixMode(GL.PROJECTION);
			GL.PushMatrix();
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.MatrixMode(GL.MODELVIEW);
			GL.PopMatrix();
			GL.MatrixMode(GL.PROJECTION);
			GL.PopMatrix();
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
	}
	public enum TransformMatrix {
		ModelView,
		Projection
	}
	public class IdentityTransformGraphicsCommand : GraphicsCommand {
		TransformMatrix matrix;
		public IdentityTransformGraphicsCommand(TransformMatrix matrix) : base() {
			this.matrix = matrix;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			bool isProjectionMatrix = matrix == TransformMatrix.ModelView;
			GL.MatrixMode(isProjectionMatrix ?  GL.PROJECTION :	GL.MODELVIEW);			
			GL.LoadIdentity();
		}
	}
	public class TranslateGraphicsCommand : GraphicsCommand {
		double xTranslate;
		double yTranslate;
		double zTranslate;
		public double XTranslate { get { return xTranslate; } }
		public double YTranslate { get { return yTranslate; } }
		public double ZTranslate { get { return zTranslate; } }
		public TranslateGraphicsCommand(double xTranslate, double yTranslate) : this(xTranslate, yTranslate, 0.0) {
		}
		public TranslateGraphicsCommand(double xTranslate, double yTranslate, double zTranslate) {
			this.xTranslate = xTranslate;
			this.yTranslate = yTranslate;
			this.zTranslate = zTranslate;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.MatrixMode(GL.MODELVIEW);
			GL.Translated(xTranslate, yTranslate, zTranslate);
		}
	}
	public class RotateGraphicsCommand : GraphicsCommand {
		float angle;
		DiagramVector rotateVector;
		public float Angle { get { return angle; } }
		public DiagramVector RotateVector { get { return rotateVector; } }
		public RotateGraphicsCommand(float angle) : this(angle, new DiagramVector(0.0, 0.0, 1.0)) {
		}
		public RotateGraphicsCommand(float angle, DiagramVector rotateVector) : base() {
			this.angle = angle;
			this.rotateVector = rotateVector;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.MatrixMode(GL.MODELVIEW);
			GL.Rotated(angle, rotateVector.DX, rotateVector.DY, rotateVector.DZ);
		}
	}
	public class ScaleGraphicsCommand : GraphicsCommand {
		double xScale, yScale, zScale;
		public ScaleGraphicsCommand(float xScale, float yScale, float zScale) {
			this.xScale = xScale;
			this.yScale = yScale;
			this.zScale = zScale;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.MatrixMode(GL.MODELVIEW);
			GL.Scaled(xScale, yScale, zScale);
		}
	}
	public class LookAtGraphicsCommand : GraphicsCommand {
		DiagramPoint eye;
		DiagramPoint center;
		DiagramPoint up;
		public LookAtGraphicsCommand(DiagramPoint eye, DiagramPoint center, DiagramPoint up) {
			this.eye = eye;
			this.center = center;
			this.up = up;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.MatrixMode(GL.PROJECTION);
			GLU.LookAt(eye.X, eye.Y, eye.Z, center.X, center.Y, center.Z, up.X, up.Y, up.Z);
		}	
	}
	public class OrthographicProjectionGraphicsCommand : GraphicsCommand {
		DiagramPoint p1;
		DiagramPoint p2;
		public DiagramPoint P1 { get { return p1; } }
		public DiagramPoint P2 { get { return p2; } }
		public OrthographicProjectionGraphicsCommand(DiagramPoint p1, DiagramPoint p2) {
			this.p1 = p1;
			this.p2 = p2;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.MatrixMode(GL.PROJECTION);
			GL.Ortho(p1.X, p2.X, p1.Y, p2.Y, p1.Z, p2.Z);
		}
	}
	public class FrustumProjectionGraphicsCommand : GraphicsCommand {
		DiagramPoint p1;
		DiagramPoint p2;
		public DiagramPoint P1 { get { return p1; } }
		public DiagramPoint P2 { get { return p2; } }
		public FrustumProjectionGraphicsCommand(DiagramPoint p1, DiagramPoint p2) {
			this.p1 = p1;
			this.p2 = p2;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.MatrixMode(GL.PROJECTION);
			GL.Frustum(p1.X, p2.X, p1.Y, p2.Y, p1.Z, p2.Z);
		}
	}
	public class PerspectiveGraphicsCommand : GraphicsCommand {
		double angle;
		double aspect;
		double zNear;
		double zFar;
		public PerspectiveGraphicsCommand(double angle, double aspect, double zNear, double zFar) {
			this.angle = angle;
			this.aspect = aspect;
			this.zNear = zNear;
			this.zFar = zFar;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.MatrixMode(GL.PROJECTION);
			GLU.Perspective(angle, aspect, zNear, zFar);
		}
	}
	public class TransformGraphicsCommand : GraphicsCommand {
		double[] transformMatrix;
		public double[] TransformMatrix { get { return transformMatrix; } }
		public TransformGraphicsCommand(double[] transformMatrix) : base() {
			this.transformMatrix = transformMatrix;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.MatrixMode(GL.MODELVIEW);
			GL.MultMatrixd(transformMatrix);
		}
	}
	public class ViewPortGraphicsCommand : GraphicsCommand {
		Rectangle viewPort;
		Rectangle defaultViewPort;
		internal Rectangle ViewPort { get { return viewPort; } }
		internal Rectangle DefaultViewPort { get { return defaultViewPort; } }
		public ViewPortGraphicsCommand(Rectangle viewPort) {
			this.viewPort = viewPort;
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			int[] res = new int[4];
			GL.GetIntegerv(GL.VIEWPORT, res);
			defaultViewPort = new Rectangle(res[0], res[1], res[2], res[3]);
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.Viewport(gr.Bounds.X, gr.Bounds.Y, gr.Bounds.Width, gr.Bounds.Height);
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.Viewport(gr.Bounds.X + viewPort.Left, gr.Bounds.Y + gr.Size.Height - viewPort.Bottom, viewPort.Width, viewPort.Height);
		}
	}
}

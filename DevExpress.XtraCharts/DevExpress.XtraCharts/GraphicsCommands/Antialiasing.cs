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
	public class SimpleAntialiasingGraphicsCommand : GraphicsCommand {
		PixelOffsetMode pixelOffsetMode;
		public SimpleAntialiasingGraphicsCommand() : this(PixelOffsetMode.Default) {
		}
		public SimpleAntialiasingGraphicsCommand(PixelOffsetMode pixelOffsetMode) : base() {
			this.pixelOffsetMode = pixelOffsetMode;
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
	}
	public class LineAntialiasingGraphicsCommand : GraphicsCommand {
		public LineAntialiasingGraphicsCommand() : base() {
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			GL.Hint(GL.LINE_SMOOTH_HINT, GL.NICEST);
			GL.Enable(GL.LINE_SMOOTH);
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			GL.Disable(GL.LINE_SMOOTH);
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
	}
	public class PolygonAntialiasingGraphicsCommand : GraphicsCommand {
		double[] projectionMatrix = new double[16];
		byte[] pixelData;
		int[] viewport = new int[4];
		double[] shifts;
		int shiftCount;
		public PolygonAntialiasingGraphicsCommand() : base() {
			shiftCount = 4;
			shifts = new double[shiftCount];
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			if (gr.GraphicsQuality < GraphicsQuality.Highest)
				return;
			GL.ClearAccum(0.0f, 0.0f, 0.0f, 0.0f);
			GL.Clear(GL.ACCUM_BUFFER_BIT);
			GL.GetIntegerv(GL.VIEWPORT, viewport);
			GL.GetDoublev(GL.PROJECTION_MATRIX, projectionMatrix);
			for (int i = 0; i < shiftCount; i++)
				shifts[i] = ((double)i) / shiftCount;
			pixelData = new byte[gr.Size.Width * gr.Size.Height * 4];
			GL.Finish();
			GL.PixelStorei(GL.UNPACK_ALIGNMENT, 1);						
			GL.ReadPixels(gr.Bounds.X, gr.Bounds.Y, gr.Bounds.Width, gr.Bounds.Height, GL.RGBA, GL.BYTE, pixelData);
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			if (gr.GraphicsQuality < GraphicsQuality.Highest)
				return;
			GL.Accum(GL.RETURN, 1.0f);
			GL.MatrixMode(GL.PROJECTION);
			GL.LoadMatrixd(projectionMatrix);
			pixelData = null;
		}
		public override void Execute(OpenGLGraphics gr) {
			if (gr.NativeDrawing) {
				ExecuteNative(gr);
				return;
			}
			BeforeExecute(gr);
			try {
				if (gr.GraphicsQuality >= GraphicsQuality.Highest) {
   					GL.PixelStorei(GL.PACK_ALIGNMENT, 1);						
					float val = 0.5f / shiftCount;
					for (int i = 0; i < shiftCount; i++) {
						RestoreImage(gr);
						GL.MatrixMode(GL.PROJECTION);
						GL.LoadIdentity();
						GL.Translated(2.0 / (viewport[2]) * shifts[i], 2.0 / (viewport[3]) * shifts[i], 0);
						GL.MultMatrixd(projectionMatrix);
						ExecuteChildrenCommands(gr);
						GL.Accum(GL.ACCUM, val);
					}
					for (int i = 0; i < shiftCount; i++) {
						RestoreImage(gr);
						GL.MatrixMode(GL.PROJECTION);
						GL.LoadIdentity();
						GL.Translated(2.0 / (viewport[2]) * shifts[i], 2.0 / (viewport[3]) * (1 - shifts[i]), 0);
						GL.MultMatrixd(projectionMatrix);
						ExecuteChildrenCommands(gr);
						GL.Accum(GL.ACCUM, val);
					}
				}
				else
					ExecuteChildrenCommands(gr);
			}
			finally {
				AfterExecute(gr);
			}
		}
		void RestoreImage(OpenGLGraphics graphics) {
			GL.Viewport(graphics.Bounds.X, graphics.Bounds.Y, graphics.Bounds.Width, graphics.Bounds.Height);
			GL.MatrixMode(GL.MODELVIEW);
			GL.PushMatrix();
			GL.LoadIdentity();
			GL.MatrixMode(GL.PROJECTION);
			GL.PushMatrix();
			GL.LoadIdentity();						
			GL.Ortho(0, viewport[2], 0, viewport[3], -1, 1);
			GL.RasterPosi(graphics.Bounds.X, graphics.Bounds.Y);
			GL.DrawPixels(graphics.Bounds.Width, graphics.Bounds.Height, GL.RGBA, GL.BYTE, pixelData);
			GL.Viewport(viewport[0], viewport[1], viewport[2], viewport[3]);
			GL.MatrixMode(GL.MODELVIEW);
			GL.PopMatrix();
			GL.MatrixMode(GL.PROJECTION);
			GL.PopMatrix();
			GL.Clear(GL.DEPTH_BUFFER_BIT | GL.STENCIL_BUFFER_BIT);
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
	}
}

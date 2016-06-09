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
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {
	public class SolidLineGraphicsCommand : GraphicsCommand {
		DiagramPoint p1;
		DiagramPoint p2;
		DiagramVector normal = DiagramVector.Zero;
		Color color;
		int thickness;
		LineCap lineCap = LineCap.Flat;
		protected DiagramPoint P1 { get { return p1; } }
		protected DiagramPoint P2 { get { return p2; } }
		protected int Thickness { get { return thickness; } }
		public SolidLineGraphicsCommand(DiagramPoint p1, DiagramPoint p2, Color color, int thickness) : base() {
			this.p1 = p1;
			this.p2 = p2;
			this.color = color;
			this.thickness = thickness;
		}
		public SolidLineGraphicsCommand(DiagramPoint p1, DiagramPoint p2, Color color, int thickness, LineCap lineCap) : this(p1, p2, color, thickness) {
			this.lineCap = lineCap;
		}
		public SolidLineGraphicsCommand(DiagramPoint p1, DiagramPoint p2, DiagramVector normal, Color color, int thickness, LineCap lineCap) : this(p1, p2, color, thickness, lineCap) {
			normal.Normalize();
			this.normal = normal;
		}
		protected virtual Pen CreatePen() {
			Pen pen = new Pen(color, thickness);
			pen.StartCap = lineCap;
			pen.EndCap = lineCap;
			return pen;
		}
		protected virtual void InitializeGL() {
			float red, green, blue, alpha;
			OpenGLGraphics.CalculateColorComponents(this.color, out red, out green, out blue, out alpha);
			GL.Color4f(red, green, blue, alpha);
			GL.LineWidth(thickness);
			if (lineCap == LineCap.Round) {
				GL.PointSize(thickness);
				GL.Enable(GL.POINT_SMOOTH);
				GL.Hint(GL.POINT_SMOOTH_HINT, GL.NICEST);
			}
		}
		protected virtual void FinalizeGL() {
			if (lineCap == LineCap.Round)
				GL.Disable(GL.POINT_SMOOTH);
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			InitializeGL();
			if (!normal.IsZero)
				GL.Normal3d(normal.DX, normal.DY, normal.DZ);
			GL.Begin(GL.LINES);
				GL.Vertex3d(p1.X, p1.Y, p1.Z);
				GL.Vertex3d(p2.X, p2.Y, p2.Z);
			GL.End();
			if (lineCap == LineCap.Round) {
				GL.Begin(GL.POINTS);
					GL.Vertex3d(p1.X, p1.Y, p1.Z);
					GL.Vertex3d(p2.X, p2.Y, p2.Z);
				GL.End();
			}
			FinalizeGL();
		}
	}
	public class DashedLineGraphicsCommand : SolidLineGraphicsCommand {
		DashStyle dashStyle;
		public DashedLineGraphicsCommand(DiagramPoint p1, DiagramPoint p2, Color color, int thickness, LineCap lineCap, DashStyle dashStyle) : this(p1, p2, DiagramVector.Zero, color, thickness, lineCap, dashStyle) {
		}
		public DashedLineGraphicsCommand(DiagramPoint p1, DiagramPoint p2, DiagramVector normal, Color color, int thickness, LineCap lineCap, DashStyle dashStyle) : base(p1, p2, normal, color, thickness, lineCap) {
			this.dashStyle = dashStyle;
		}
		protected override Pen CreatePen() {
			Pen pen = base.CreatePen();
			DashStyleHelper.ApplyDashStyle(pen, dashStyle);
			return pen;
		}
		protected override void InitializeGL() {
			base.InitializeGL();
			if (dashStyle != DashStyle.Solid) {
				GL.LineStipple(Thickness, GetGLDashStyle(dashStyle));
				GL.Enable(GL.LINE_STIPPLE);
			}
		}
		protected override void FinalizeGL() {
			if (dashStyle != DashStyle.Solid)
				GL.Disable(GL.LINE_STIPPLE);
			base.FinalizeGL();
		}
		ushort GetGLDashStyle(DashStyle dashStyle){
			switch (dashStyle) {
				case DashStyle.Solid :
					return (ushort)0xffffu;
				case DashStyle.Dash  :
					return (ushort)0xeeeeu;
				case DashStyle.Dot :
					return (ushort)0xaaaau;
				case DashStyle.DashDot :
					return (ushort)0xfafau;
				case DashStyle.DashDotDot :
					return (ushort)0xeaeau;
			}
			return (ushort)0xffffu;
		}
	}
}

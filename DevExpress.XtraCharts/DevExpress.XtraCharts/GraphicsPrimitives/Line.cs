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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Native {
	public class Line : PlanePrimitive {
		public static Line CreateFromLine(Vertex[] vertices, Line baseLine) {
			if (vertices.Length == 2) {
				Line line = new Line();
				line.Assign(baseLine);
				line.Vertices = vertices;
				return line;
			}
			return null;
		}
		DashStyle dashStyle = DashStyle.Solid;
		int thickness = 1;
		LineCap lineCap = LineCap.Flat;
		public Vertex V1 { get { return Vertices[0]; } }
		public Vertex V2 { get { return Vertices[1]; } }
		protected Line() {
		}
		public Line(DiagramPoint p1, DiagramPoint p2) : base(new DiagramPoint[] { p1, p2 }) {
		}
		public Line(DiagramPoint p1, DiagramPoint p2, bool sameNormals, bool sameColors, DiagramVector normal, Color color, LineCap lineCap) : base(new DiagramPoint[] { p1, p2 }, sameNormals, sameColors, normal, color) {
			this.lineCap = lineCap;
		}
		public Line(DiagramPoint p1, DiagramPoint p2, bool sameNormals, bool sameColors, DiagramVector normal, Color color) : this(p1, p2, sameNormals, sameColors, normal, color, LineCap.Flat) {
		}
		public Line(DiagramPoint p1, DiagramPoint p2, DiagramVector normal) : this(p1, p2, true, false, normal, Color.Empty) {
		}
		public Line(Vertex v1, Vertex v2) : base(new Vertex[] { v1, v2 }) {
		}
		public Line(Vertex v1, Vertex v2, bool sameNormals, bool sameColors, DiagramVector normal, Color color) : base(new Vertex[] { v1, v2 }, sameNormals, sameColors, normal, color) {
		}
		public Line(Vertex v1, Vertex v2, DiagramVector normal) : this(v1, v2, true, false, normal, Color.Empty) {
		}
		public void SetParameters(DashStyle dashStyle, int thickness) {
			this.dashStyle = dashStyle;
			this.thickness = thickness;
		}
		protected override bool IsVerticesCountValid(int count) {
			return count == 2;
		}
		protected override PlanePrimitive CreateInstance() {
			return new Line();
		}
		public override void AssignProperties(PlanePrimitive primitive) {
			base.AssignProperties(primitive);
			Line line = primitive as Line;
			if (line != null) {
				dashStyle = line.dashStyle;
				thickness = line.thickness;
				lineCap = line.lineCap;
			}
		}
		protected internal override GraphicsCommand CreateGraphicsCommand() {
			return dashStyle == DashStyle.Solid ? new SolidLineGraphicsCommand(V1, V2, Normal, Color, thickness, lineCap) :
				new DashedLineGraphicsCommand(V1, V2, Normal, Color, thickness, lineCap, dashStyle);
		}
	}
}

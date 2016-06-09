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
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraCharts.Native {
	public class PlaneTriangle : PlanePolygon {
		public PlaneTriangle() {
		}
		public PlaneTriangle(DiagramPoint p1, DiagramPoint p2, DiagramPoint p3) : base(new DiagramPoint[] { p1, p2, p3 }) {
		}
		public PlaneTriangle(DiagramPoint p1, DiagramPoint p2, DiagramPoint p3, bool sameNormals, bool sameColors, DiagramVector normal, Color color)
			: base(new DiagramPoint[] { p1, p2, p3 }, sameNormals, sameColors, normal, color) {
		}
		public PlaneTriangle(Vertex v1, Vertex v2, Vertex v3) : base(new Vertex[] { v1, v2, v3 }) {
		}
		protected override bool IsVerticesCountValid(int count) {
			return count == 3;
		}
		protected override PlanePrimitive CreateInstance() {
			return new PlaneTriangle();
		}
		PlaneTriangle CreateInstance(Vertex v1, Vertex v2, Vertex v3) {
			PlaneTriangle result = (PlaneTriangle)CreateInstance();
			result.AssignProperties(this);
			result.SetVertices(new Vertex[] { v1, v2, v3 });
			return result;
		}
		public PlaneTriangle CalcLeftHalf() {
			Vertex center = MathUtils.CalcIntervalCenter3D(Vertices[0], Vertices[1], Diagram3D.Epsilon);
			return CreateInstance(center, Vertices[1], Vertices[2]);
		}
		public PlaneTriangle CalcRightHalf() {
			Vertex center = MathUtils.CalcIntervalCenter3D(Vertices[0], Vertices[1], Diagram3D.Epsilon);
			return CreateInstance(Vertices[0], center, Vertices[2]);
		}
	}
}

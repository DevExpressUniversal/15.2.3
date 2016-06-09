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
using System.Text;
namespace DevExpress.XtraCharts.Native {
	public class PlaneQuadrangle : PlanePolygon {
		BoxPlane boxPlane;
		public virtual BoxPlane BoxPlane { get { return boxPlane; } }
		protected PlaneQuadrangle() {
		}
		public PlaneQuadrangle(DiagramPoint p1, DiagramPoint p2, DiagramPoint p3, DiagramPoint p4, BoxPlane boxPlane)
			: base(new DiagramPoint[] { p1, p2, p3, p4 }) {
			this.boxPlane = boxPlane;
		}
		public PlaneQuadrangle(DiagramPoint p1, DiagramPoint p2, DiagramPoint p3, DiagramPoint p4)
			: base(new DiagramPoint[] { p1, p2, p3, p4 }) {
		}
		public PlaneQuadrangle(Vertex v1, Vertex v2, Vertex v3, Vertex v4, BoxPlane boxPlane)
			: base(new Vertex[] { v1, v2, v3, v4 }) {
			this.boxPlane = boxPlane;
		}
		public PlaneQuadrangle(Vertex v1, Vertex v2, Vertex v3, Vertex v4)
			: base(new Vertex[] { v1, v2, v3, v4 }) {
		}
		protected override bool IsVerticesCountValid(int count) {
			return count == 4;
		}
		protected override PlanePrimitive CreateInstance() {
			return new PlaneQuadrangle();
		}
		void CalcCenterVerticalVertices(out Vertex v1, out Vertex v2) {
			v1 = MathUtils.CalcIntervalCenter3D(Vertices[0], Vertices[1], Diagram3D.Epsilon);
			v2 = MathUtils.CalcIntervalCenter3D(Vertices[2], Vertices[3], Diagram3D.Epsilon);
		}
		void CalcCenterHorizontalVertices(out Vertex v1, out Vertex v2) {
			v1 = MathUtils.CalcIntervalCenter3D(Vertices[0], Vertices[3], Diagram3D.Epsilon);
			v2 = MathUtils.CalcIntervalCenter3D(Vertices[1], Vertices[2], Diagram3D.Epsilon);
		}
		PlaneQuadrangle CreateInstance(Vertex v1, Vertex v2, Vertex v3, Vertex v4) {
			PlaneQuadrangle result = (PlaneQuadrangle)CreateInstance();
			result.AssignProperties(this);
			result.SetVertices(new Vertex[] { v1, v2, v3, v4 });
			return result;
		}
		void CreateHorizontalHalves(out PlaneQuadrangle leftHalf, out PlaneQuadrangle rightHalf) {
			leftHalf = CalcLeftHalf();
			rightHalf = CalcRightHalf();
		}
		void CreateVerticalHalves(out PlaneQuadrangle topHalf, out PlaneQuadrangle bottomHalf) {
			bottomHalf = CalcBottomHalf();
			topHalf = CalcTopHalf();
		}
		public PlaneQuadrangle CalcLeftHalf() {
			Vertex v1, v2;
			CalcCenterVerticalVertices(out v1, out v2);
			return CreateInstance(Vertices[0], v1, v2, Vertices[3]);
		}
		public PlaneQuadrangle CalcRightHalf() {
			Vertex v1, v2;
			CalcCenterVerticalVertices(out v1, out v2);
			return CreateInstance(v1, Vertices[1], Vertices[2], v2);
		}
		public PlaneQuadrangle CalcBottomHalf() {
			Vertex v1, v2;
			CalcCenterHorizontalVertices(out v1, out v2);
			return CreateInstance(Vertices[0], Vertices[1], v2, v1);
		}
		public PlaneQuadrangle CalcTopHalf() {
			Vertex v1, v2;
			CalcCenterHorizontalVertices(out v1, out v2);
			return CreateInstance(v1, v2, Vertices[2], Vertices[3]);
		}
		public PlaneQuadrangle[] SeparateByCenter() {
			PlaneQuadrangle leftHalf, rightHalf;
			CreateHorizontalHalves(out leftHalf, out rightHalf);
			PlaneQuadrangle leftTop, leftBottom, rightTop, rightBottom;
			leftHalf.CreateVerticalHalves(out leftTop, out leftBottom);
			rightHalf.CreateVerticalHalves(out rightTop, out rightBottom);
			return new PlaneQuadrangle[] { leftTop, leftBottom, rightTop, rightBottom };
		}
	}
}

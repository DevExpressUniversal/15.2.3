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
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Native {
	public class Prizm {
		PlanePolygon[] polygons;
		PlanePolygon[] bases;
		PlanePolygon[] laterals;
		public PlanePolygon[] Polygons { get { return polygons; } }
		public PlanePolygon[] Bases { get { return bases; } }
		public PlanePolygon[] Laterals { get { return laterals; } }
		public Prizm(PlanePolygon polygon, double height) {
#if DEBUGTEST
			if (polygon.Vertices.Length < 3)
				throw new ArgumentException();
			DiagramVector normal = MathUtils.CalcNormal(polygon.Vertices[0], polygon.Vertices[1], polygon.Vertices[2]);
			if (ComparingUtils.CompareDoubles(normal.DX, 0, Diagram3D.Epsilon) != 0 ||
				ComparingUtils.CompareDoubles(normal.DY, 0, Diagram3D.Epsilon) != 0 ||
				ComparingUtils.CompareDoubles(Math.Abs(normal.DZ), 1, Diagram3D.Epsilon) != 0)
					throw new ArgumentException();
#endif
			DiagramPoint[] points1 = new DiagramPoint[polygon.Vertices.Length];
			DiagramPoint[] points2 = new DiagramPoint[polygon.Vertices.Length];
			double offset1 = -height / 2;
			double offset2 = height / 2;
			for (int i = 0; i < polygon.Vertices.Length; i++) {
				points1[i] = DiagramPoint.Offset(polygon.Vertices[i], 0, 0, offset1);
				points2[i] = DiagramPoint.Offset(polygon.Vertices[i], 0, 0, offset2);
			}
			bases = new PlanePolygon[2];
			bases[0] = new PlanePolygon(points1);
			bases[1] = new PlanePolygon(points2);
			laterals = new PlanePolygon[polygon.Vertices.Length];
			for (int i = 0; i < polygon.Vertices.Length; i++) {
				int index1 = i;
				int index2 = i == polygon.Vertices.Length - 1 ? 0 : i + 1;
				laterals[i] = new PlanePolygon(new DiagramPoint[] { points1[index1], points1[index2], points2[index2], points2[index1] });
				laterals[i].Normal = MathUtils.CalcNormal(laterals[i]);
				laterals[i].SameNormals = true;
			}
			bases[0].InvertVerticesDirection();
			bases[0].Normal = new DiagramVector(0, 0, -1);
			bases[0].SameNormals = true;
			bases[1].Normal = new DiagramVector(0, 0, 1);
			bases[1].SameNormals = true;
			polygons = new PlanePolygon[bases.Length + laterals.Length];
			Array.Copy(bases, 0, polygons, 0, bases.Length);
			Array.Copy(laterals, 0, polygons, 2, laterals.Length);
		}
	}
}

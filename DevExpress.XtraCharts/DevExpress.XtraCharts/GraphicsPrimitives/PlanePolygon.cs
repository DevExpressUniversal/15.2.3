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
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Native {
	public class PlanePolygon : PlanePrimitive {
		protected PlanePolygon() {
		}
		public PlanePolygon(IList<DiagramPoint> points) : base(points) {
		}
		public PlanePolygon(DiagramPoint[] points, bool sameNormals, bool sameColors, DiagramVector normal, Color color)
			: base(points, sameNormals, sameColors, normal, color) {
		}
		public PlanePolygon(Vertex[] vertices)
			: base(vertices) {
		}
		public PlanePolygon(Vertex[] vertices, bool sameNormals, bool sameColors, DiagramVector normal, Color color)
			: base(vertices, sameNormals, sameColors, normal, color) {
		}
		public PlanePolygon CreateInstance(Vertex[] vertices) {
			if (vertices.Length < 3)
				return null;
			PlanePolygon result = new PlanePolygon();
			result.AssignProperties(this);
			result.SetVertices(vertices);
			return result;
		}
		public PlaneTriangle CreateTriangle(Vertex v1, Vertex v2, Vertex v3) {
			PlaneTriangle triangle = new PlaneTriangle();
			triangle.AssignProperties(this);
			triangle.SetVertices(new Vertex[] { v1, v2, v3 });
			return triangle;
		}
		protected override bool IsVerticesCountValid(int count) {
			return count > 2;
		}
		protected override PlanePrimitive CreateInstance() {
			return new PlanePolygon();
		}
		protected internal override GraphicsCommand CreateGraphicsCommand() {
			return new PolygonGraphicsCommand(this);
		}
	}
}

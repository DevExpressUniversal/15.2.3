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
using System.Runtime.InteropServices;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {
	public class Tessellator : IDisposable {
		#region inner classes
		[StructLayout(LayoutKind.Sequential)]
		struct VertexData {
			DiagramPoint point;
			bool doubled;
			public DiagramPoint Point { get { return point; } }
			public bool Doubled { get { return doubled; } }
			public VertexData(DiagramPoint point, bool doubled) {
				this.point = point;
				this.doubled = doubled;
			}
			public VertexData(DiagramPoint point)
				: this(point, false) {
			}
		}
		#endregion
		IntPtr tess;
		delegate void ErrorCallbackDelegate(int errno);
		delegate void BeginCallbackDelegate(int type);
		delegate void EndCallbackDelegate();
		delegate void VertexCallbackDelegate(IntPtr vertex_data);
		delegate void CombineCallbackDelegate(IntPtr coords, IntPtr[] vertex_data, IntPtr weight, ref IntPtr outData);
		Delegate errorDlg, beginDlg, endDlg, vertexDlg, combineDlg;
		List<PlaneTriangle> resultTriangles;
		List<IList<DiagramPoint>> resultContours;
		List<DiagramPoint> points = new List<DiagramPoint>();
		List<IntPtr> memory = new List<IntPtr>();
		DiagramVector normal;
		Color color;
		int type;
		bool doubleCombined;
		public Tessellator() {
		}
		~Tessellator() {
			Dispose();
		}
		[System.Security.SecuritySafeCritical]
		void EnsureInitialization() {
			if (tess == IntPtr.Zero) {
				errorDlg = new ErrorCallbackDelegate(Error);
				beginDlg = new BeginCallbackDelegate(Begin);
				endDlg = new EndCallbackDelegate(End);
				vertexDlg = new VertexCallbackDelegate(Vertex);
				combineDlg = new CombineCallbackDelegate(Combine);
				tess = GLU.NewTess();
				GLU.TessCallback(tess, GLU.TESS_ERROR, Marshal.GetFunctionPointerForDelegate(errorDlg));
				GLU.TessCallback(tess, GLU.TESS_BEGIN, Marshal.GetFunctionPointerForDelegate(beginDlg));
				GLU.TessCallback(tess, GLU.TESS_END, Marshal.GetFunctionPointerForDelegate(endDlg));
				GLU.TessCallback(tess, GLU.TESS_VERTEX, Marshal.GetFunctionPointerForDelegate(vertexDlg));
				GLU.TessCallback(tess, GLU.TESS_COMBINE, Marshal.GetFunctionPointerForDelegate(combineDlg));
				GLU.TessProperty(tess, GLU.TESS_TOLERANCE, 0);
			}
		}
		void Init(bool boundaryOnly, int windingRule, bool doubleCombined, DiagramVector normal, Color color) {
			EnsureInitialization();
			GLU.TessProperty(tess, GLU.TESS_BOUNDARY_ONLY, boundaryOnly ? GL.TRUE : GL.FALSE);
			GLU.TessProperty(tess, GLU.TESS_WINDING_RULE, windingRule);
			this.doubleCombined = doubleCombined;
			this.normal = normal;
			this.color = color;
			resultContours = new List<IList<DiagramPoint>>();
			resultTriangles = new List<PlaneTriangle>();
		}
		void Error(int errno) {
			throw new OpenGLException("tessellator");
		}
		void Begin(int type) {
			points.Clear();
			this.type = type;
		}
		void End() {
			if (type == GL.LINE_LOOP && points.Count > 0) {
				List<DiagramPoint> contour = new List<DiagramPoint>(points);
				resultContours.Add(contour);
			}
		}
		[System.Security.SecuritySafeCritical]
		void Vertex(IntPtr vertex_data) {
			VertexData data = (VertexData)Marshal.PtrToStructure(vertex_data, typeof(VertexData));
			points.Add(data.Point);
			if (data.Doubled)
				points.Add(data.Point);
			if (points.Count < 3)
				return;
			int i1, i2, i3;
			switch (type) {
				case GL.TRIANGLE_FAN:
					i1 = 0;
					i2 = points.Count - 2;
					i3 = points.Count - 1;
					break;
				case GL.TRIANGLE_STRIP:
					i3 = points.Count - 1;
					if (points.Count % 2 == 0) {
						i1 = points.Count - 2;
						i2 = i1 - 1;
					}
					else {
						i1 = points.Count - 3;
						i2 = i1 + 1;
					}
					break;
				case GL.TRIANGLES:
					if (points.Count % 3 == 0) {
						i1 = points.Count - 3;
						i2 = i1 + 1;
						i3 = i1 + 2;
					}
					else
						return;
					break;
				default:
					return;
			}
			resultTriangles.Add(new PlaneTriangle(points[i1], points[i2], points[i3], true, true, normal, color));
		}
		VertexData Combine(DiagramPoint point, List<VertexData> verticesData) {
			return new VertexData(point, doubleCombined);
		}
		[System.Security.SecuritySafeCritical]
		void Combine(IntPtr coords, IntPtr[] vertex_data, IntPtr weight, ref IntPtr outData) {
			List<VertexData> verticesData = new List<VertexData>();
			foreach (IntPtr dataPtr in vertex_data) {
				if (dataPtr != IntPtr.Zero) {
					VertexData data = (VertexData)Marshal.PtrToStructure(vertex_data[0], typeof(VertexData));
					verticesData.Add(data);
				}
			}
			DiagramPoint point = (DiagramPoint)Marshal.PtrToStructure(coords, typeof(DiagramPoint));
			VertexData combined = Combine(point, verticesData);
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(combined));
			Marshal.StructureToPtr(combined, ptr, false);
			memory.Add(ptr);
			outData = ptr;
		}
		IList<IList<DiagramPoint>> CalcAdjacentEdges(IList<DiagramPoint> contour, double epsilon) {
			List<IList<DiagramPoint>> result = new List<IList<DiagramPoint>>();
			if (contour != null && contour.Count >= 3) {
				for (int i = 0; i < contour.Count - 1; i++) {
					DiagramPoint p1 = contour[i];
					DiagramPoint p2 = contour[i + 1];
					if (MathUtils.ArePointsEquals2D(p1, p2, epsilon))
						continue;
					for (int k = i + 1; k < contour.Count; k++) {
						DiagramPoint p3 = contour[k];
						DiagramPoint p4 = contour[k == contour.Count - 1 ? 0 : k + 1];
						if (MathUtils.ArePointsEquals2D(p3, p4, epsilon))
							continue;
						Intersection intersection = IntersectionUtils.CalcLinesIntersection2D(p1, p2, p3, p4, epsilon, out points);
						if (intersection == Intersection.Match && points.Count == 2) {
							List<DiagramPoint> line = new List<DiagramPoint>(points);
							result.Add(line);
						}
					}
				}
			}
			return result;
		}
		IList<IList<DiagramPoint>> CalcAdjacentContours(IList<DiagramPoint> contour, double epsilon) {
			List<IList<DiagramPoint>> result = new List<IList<DiagramPoint>>();
			IList<IList<DiagramPoint>> edges = CalcAdjacentEdges(contour, epsilon);
			foreach (IList<DiagramPoint> edge in edges) {
				IList<DiagramPoint> minContour = MathUtils.MakeRectangle(edge[0], edge[1], true, epsilon * 3);
				result.Add(minContour);
			}
			return result;
		}
		List<VertexData> GetVerticesData(IList<DiagramPoint> contour) {
			List<VertexData> result = new List<VertexData>();
			foreach (DiagramPoint point in contour)
				result.Add(new VertexData(point));
			return result;
		}
		void ProcessPolygon(params IList<DiagramPoint>[] contours) {
			ProcessPolygon((IList<IList<DiagramPoint>>)contours);
		}
		[System.Security.SecuritySafeCritical]
		void ProcessPolygon(IList<IList<DiagramPoint>> contours) {
			try {
				GLU.TessNormal(tess, normal.DX, normal.DY, normal.DZ);
				GLU.TessBeginPolygon(tess, IntPtr.Zero);
				foreach (IList<DiagramPoint> contour in contours) {
					List<VertexData> verticesData = GetVerticesData(contour);
					GLU.TessBeginContour(tess);
					foreach (VertexData data in verticesData) {
						IntPtr dataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(data));
						Marshal.StructureToPtr(data, dataPtr, false);
						memory.Add(dataPtr);
						GLU.TessVertex(tess, new double[] { data.Point.X, data.Point.Y, data.Point.Z }, dataPtr);
					}
					GLU.TessEndContour(tess);
				}
				GLU.TessEndPolygon(tess);
			}
			finally {
				foreach (IntPtr ptr in memory)
					Marshal.FreeHGlobal(ptr);
				memory.Clear();
			}
		}
		void DeleteTess() {
			GLU.DeleteTess(tess);
			tess = IntPtr.Zero;
		}
		public void Dispose() {
			if (tess != IntPtr.Zero)
				DeleteTess();
		}
		public List<PlaneTriangle> Triangulate(IList<DiagramPoint> contour, DiagramVector normal, Color color) {
			if (contour == null || contour.Count < 3)
				return null;
			Init(false, GLU.TESS_WINDING_NONZERO, false, normal, color);
			ProcessPolygon(new List<DiagramPoint>(contour));
			return resultTriangles;
		}
		public IList<IList<DiagramPoint>> Prepare(IList<DiagramPoint> contour, DiagramVector normal, bool adjacentEdges, double epsilon) {
			if (contour == null || contour.Count < 3)
				return null;
			Init(true, GLU.TESS_WINDING_NONZERO, true, normal, Color.Empty);
			ProcessPolygon(new List<DiagramPoint>(contour));
			if (adjacentEdges) {
				IList<IList<DiagramPoint>> adjacent = CalcAdjacentContours(contour, epsilon);
				resultContours.AddRange(adjacent);
			}
			return resultContours;
		}
		public IList<IList<DiagramPoint>> Union(IList<IList<DiagramPoint>> contours, DiagramVector normal) {
			if (contours == null || contours.Count == 0)
				return null;
			Init(true, GLU.TESS_WINDING_NONZERO, false, normal, Color.Empty);
			ProcessPolygon(contours);
			return resultContours;
		}
		public IList<IList<DiagramPoint>> Difference(IList<DiagramPoint> minuend, IList<IList<DiagramPoint>> subtrahend, DiagramVector normal) {
			if (minuend == null || subtrahend == null)
				return null;
			List<IList<DiagramPoint>> contours = new List<IList<DiagramPoint>>();
			contours.Add(minuend);
			contours.AddRange(subtrahend);
			return Difference(contours, normal);
		}
		public IList<IList<DiagramPoint>> Difference(IList<IList<DiagramPoint>> contours, DiagramVector normal) {
			if (contours == null || contours.Count == 0)
				return null;
			Init(true, GLU.TESS_WINDING_POSITIVE, false, normal, Color.Empty);
			ProcessPolygon(contours);
			return resultContours;
		}
	}
}

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

using D3D;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing.DirectX;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
namespace DevExpress.XtraMap.Drawing {
	[CLSCompliant(false)]
	public class BufferParam : IDisposable {
		public uint PrimitiveCount { get; set; }
		public IntPtr VerticesPtr { get; set; }
		public bool IsStatic { get; set; }
		protected virtual void Dispose(bool disposing) {
			if (VerticesPtr != IntPtr.Zero) {
				MarshalHelper.FreeHGlobal(VerticesPtr);
				VerticesPtr = IntPtr.Zero;
			}
		}
		~BufferParam() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct Vertex {
		Vector3 position;
		Vector2 uv;
		public Vector3 Position { get { return position; } set { position = value; } }
		public Vector2 UV { get { return uv; } set { uv = value; } }
		public Vertex(LiteVertex liteVertex) {
			position = new Vector3(liteVertex.Position);
			uv = new Vector2();
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct LiteVertex {
		Vector2 position;
		Vector2 positionPrecision;
		Vector2 normal;
		public Vector2 Position { get { return position; } set { position = value; } }
		public Vector2 PositionPrecision { get { return positionPrecision; } set { positionPrecision = value; } }
		public Vector2 Normal { get { return normal; } set { normal = value; } }
	}
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public struct D3DBuffer {
		public IDirect3DVertexBuffer9 Buffer { get; set; }
		public uint PrimitiveCount { get; set; }
		public uint Size { get; set; }
	}
	[CLSCompliant(false)]
	public class D3DResourceHolder : MapDisposableObject, IRenderItemResourceHolder {
		static Dictionary<string, IList<BufferParam>> StaticAreasBuffers = new Dictionary<string, IList<BufferParam>>();
		static Dictionary<string, IList<BufferParam>> StaticCountourBuffers = new Dictionary<string, IList<BufferParam>>();
		readonly IRenderItemProvider provider;
		readonly IRenderItem owner;
		readonly IList<LiteVertex[]> contourVertices = new List<LiteVertex[]>();
		readonly IList<BufferParam> areas = new List<BufferParam>();
		readonly IList<BufferParam> contours = new List<BufferParam>();
		ITesselator tesselator;
		LiteVertex[] areaVertices;
		protected IMapItemGeometry Geometry { get { return owner.Geometry; } }
		protected int ContourWidth { get { return owner.Style.StrokeWidth; } }
		protected internal IList<LiteVertex[]> ContourVertices { get { return contourVertices; } }
		public IList<BufferParam> Areas { get { return areas; } }
		public IList<BufferParam> Contours { get { return contours; } }
		public D3DResourceHolder(ITesselator tesselator, IRenderItemProvider provider, IRenderItem owner) {
			Guard.ArgumentNotNull(provider, "provider");
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.provider = provider;
			this.tesselator = tesselator;
		}
		protected override void DisposeOverride() {
			StaticAreasBuffers.Clear();
			StaticCountourBuffers.Clear();
			ClearBufferParams();
			base.DisposeOverride();
		}
		MapPoint[] ClosingPolygon(MapPoint[] points) {
			if (points[0] != points[points.Length - 1]) {
				Array.Resize(ref points, points.Length + 1);
				points[points.Length - 1] = points[0];
			}
			return points;
		}
		LiteVertex[] CreateLine(MapPoint[] points) {
			LiteVertex[] line = new LiteVertex[6];
			Vector2d p0 = new Vector2d(points[0].X, -points[0].Y);
			Vector2d p1 = new Vector2d(points[1].X, -points[1].Y);
			Vector2d normal = D3DMath.CalcNormal(p0, p1);
			line[0] = new LiteVertex() { Position = p0.ToVector2(), Normal = normal.ToVector2(), PositionPrecision = p0.GetFloatPrecision() };
			line[1] = new LiteVertex() { Position = p0.ToVector2(), Normal = normal.Inversion().ToVector2(), PositionPrecision = p0.GetFloatPrecision() };
			line[2] = new LiteVertex() { Position = p1.ToVector2(), Normal = normal.ToVector2(), PositionPrecision = p1.GetFloatPrecision() };
			line[3] = line[1];
			line[4] = line[2];
			line[5] = new LiteVertex() { Position = p1.ToVector2(), Normal = normal.Inversion().ToVector2(), PositionPrecision = p1.GetFloatPrecision() };
			return line;
		}
		void CreateFilledArea(IList<MapPoint[]> polygon) {
			if (this.tesselator == null)
				return;
			Vector3d[] resPoints = this.tesselator.Tesselate(polygon);
			if (resPoints == null)
				return;
			int vertexCount = resPoints.Length;
			areaVertices = new LiteVertex[vertexCount];
			for (int i = 0; i < vertexCount; i++) {
				areaVertices[i] = new LiteVertex();
				float x = (float)(resPoints[i].X - (float)resPoints[i].X);
				float y = (float)(resPoints[i].Y - (float)resPoints[i].Y);
				areaVertices[i].PositionPrecision = new Vector2() { X = x, Y = -y };
				areaVertices[i].Position = new Vector2() { X = (float)resPoints[i].X, Y = -(float)(resPoints[i].Y) };
			}
		}
		void InitShapeGeometry(IList<MapPoint[]> contours) {
			foreach (MapPoint[] contour in contours)
				AddContour(contour, true);
			CreateFilledArea(contours);
		}
		void CalculateData() {
			ClearGeometryVerties();
			IList<MapPoint[]> contours = new List<MapPoint[]>();
			IUnitGeometry geometry = Geometry as IUnitGeometry;
			MultiLineUnitGeometry lineGeometry = Geometry as MultiLineUnitGeometry;
			PathUnitGeometry pathGeometry = Geometry as PathUnitGeometry;
			if (pathGeometry != null) {
				contours.Add(provider.GeometryToScreenPoints(pathGeometry.Points));
				foreach (MapUnit[] contour in pathGeometry.InnerContours)
					contours.Add(provider.GeometryToScreenPoints(contour));
			} else if(lineGeometry != null) {
				foreach(MapUnit[] segment in lineGeometry.Segments)
					contours.Add(provider.GeometryToScreenPoints(segment));
			} else if (geometry != null)
				contours.Add(provider.GeometryToScreenPoints(geometry.Points));
			InitGeometry(contours, geometry);
		}
		void InitGeometry(IList<MapPoint[]> contours, IUnitGeometry geometry) {
			if (geometry != null && contours.Count > 0) {
				if (geometry.Type == UnitGeometryType.Areal)
					InitShapeGeometry(contours);
				else if (geometry.Type == UnitGeometryType.Linear) {
					foreach (MapPoint[] contour in contours)
						if (contour.Length > 1)
							InitLineGeometry(contour);
				}
			}
		}
		void Update() {
			ITemplateGeometryItem templItem = owner as ITemplateGeometryItem;
			bool staticBuffer = templItem != null;
			ClearBufferParams();
			if (staticBuffer && FindInStaticBuffers(templItem))
				return;
			CalculateData();
			CreateBufferParams(templItem, staticBuffer);
			ClearGeometryVerties();
		}
		bool FindInStaticBuffers(ITemplateGeometryItem templItem) {
			string key = GenerateKey(templItem);
			bool res = false;
			IList<BufferParam> staticAreas = new List<BufferParam>();
			if(StaticAreasBuffers.TryGetValue(key, out staticAreas)) {
				foreach (BufferParam item in staticAreas)
					Areas.Add(item);
				res = true;
			}
			IList<BufferParam> staticCountours = new List<BufferParam>();
			if (StaticCountourBuffers.TryGetValue(key, out staticCountours)) {
				foreach (BufferParam item in staticCountours)
					Contours.Add(item);
				res = true;
			}
			return res;
		}
		string GenerateKey(ITemplateGeometryItem templItem) {
			object key = templItem == null ? owner.GetHashCode() : (object)templItem.Type;
			return string.Format("Item_{0}", key);
		}
		void CreateBufferParams(ITemplateGeometryItem templItem, bool isStatic) {
			CreateAreasBufferParam(isStatic);
			CreateContourBufferParam(isStatic);
			if (isStatic)
				AddStaticBufferParam(templItem);
		}
		void AddStaticBufferParam(ITemplateGeometryItem templItem) {
			string key = GenerateKey(templItem);
			if (Areas.Count > 0) {
				List<BufferParam> staticAreas = new List<BufferParam>();
				staticAreas.Add(Areas[0]);
				StaticAreasBuffers.Add(key, staticAreas);
			}
			List<BufferParam> staticCountours = new List<BufferParam>();
			foreach (BufferParam item in Contours)
				staticCountours.Add(item);
			StaticCountourBuffers.Add(key, staticCountours);
		}
		void CreateContourBufferParam(bool isStatic) {
			for (int i = 0; i < ContourVertices.Count; i++) {
				LiteVertex[] countour = ContourVertices[i];
				BufferParam param = new BufferParam();
				param.PrimitiveCount = Math.Min((uint)(countour.Length / 3), Direct3DHelper.D3DParameters.MaxPrimitiveCount);
				param.VerticesPtr = CreateVerticesPtr(countour);
				param.IsStatic = isStatic; ;
				Contours.Add(param);
			}
		}
		void CreateAreasBufferParam(bool isStatic) {
			if (areaVertices != null && areaVertices.Length > 0) {
				uint areaVerticesCount = (uint)areaVertices.Length;
				BufferParam param = new BufferParam();
				param.PrimitiveCount = Math.Min(areaVerticesCount / 3, Direct3DHelper.D3DParameters.MaxPrimitiveCount);
				param.VerticesPtr = CreateVerticesPtr(areaVertices);
				param.IsStatic = isStatic;
				Areas.Add(param);
			}
		}
		[SecuritySafeCritical]
		IntPtr CreateVerticesPtr(LiteVertex[] vertices) {
			uint size = (uint)(vertices.Length * D3DRenderer.LiteVertexStride);
			IntPtr data = MarshalHelper.AllocHGlobal((int)(size + D3DRenderer.LiteVertexStride));
			GCHandle handle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
			IntPtr source = handle.AddrOfPinnedObject();
			MarshalHelper.CopyMemory(data, source, size);
			handle.Free();
			return data;
		}
		void ClearGeometryVerties() {
			ContourVertices.Clear();
			areaVertices = null;
		}
		void ClearBufferParams() {
			foreach (BufferParam param in Areas) {
				if(!param.IsStatic)
					param.Dispose();
			}
			foreach (BufferParam param in Contours) {
				if(!param.IsStatic)
					param.Dispose();
			}
			Areas.Clear();
			Contours.Clear();
		}
		void AddContour(MapPoint[] points, bool closing) {
			LiteVertex[] contour = CreateContour(points, closing);
			if (contour.Length > 0)
				ContourVertices.Add(contour);
		}
		void AddLine(MapPoint[] points) {
			LiteVertex[] line = CreateLine(points);
			if (line.Length > 0)
				ContourVertices.Add(line);
		}
		LiteVertex[] CreateSimpleContour(MapPoint[] points) {
			LiteVertex[] contour = new LiteVertex[(points.Length - 1) * 6];
			PopulateLineContour(contour, points);
			return contour;
		}
		void PopulateLineContour(LiteVertex[] contour, MapPoint[] points) {
			for(int i = 0, ind = 0; i < points.Length - 1; ++i, ind += 6) {
				Vector2d p = new Vector2d(points[i].X, -points[i].Y);
				Vector2d pNext = new Vector2d(points[i + 1].X, -points[i + 1].Y);
				Vector2d normal = D3DMath.CalcNormal(p, pNext);
				contour[ind] = new LiteVertex() { Position = p.ToVector2(), Normal = normal.ToVector2(), PositionPrecision = p.GetFloatPrecision() };
				contour[ind + 1] = new LiteVertex() { Position = p.ToVector2(), Normal = normal.Inversion().ToVector2(), PositionPrecision = p.GetFloatPrecision() };
				contour[ind + 2] = new LiteVertex() { Position = pNext.ToVector2(), Normal = normal.ToVector2(), PositionPrecision = pNext.GetFloatPrecision() };
				contour[ind + 3] = contour[ind + 1];
				contour[ind + 4] = contour[ind + 2];
				contour[ind + 5] = new LiteVertex() { Position = pNext.ToVector2(), Normal = normal.Inversion().ToVector2(), PositionPrecision = pNext.GetFloatPrecision() };
			}
		}
		LiteVertex[] CreateComplexContour(MapPoint[] points, bool closing) {
			LiteVertex[] contour = new LiteVertex[(points.Length - 1) * 6 + (points.Length - (closing ? 1 : 2)) * 6];
			PopulateLineContour(contour, points);
			int ind = (points.Length - 1) * 6;
			for(int i = 1; i < points.Length - 1; i++) {
				contour[ind] = contour[(i - 1) * 6 + 4];
				contour[ind + 1] = contour[(i - 1) * 6 + 5];
				contour[ind + 2] = contour[i * 6];
				contour[ind + 3] = contour[ind];
				contour[ind + 4] = contour[ind + 1];
				contour[ind + 5] = contour[i * 6 + 1];
				ind += 6;
			}
			if(closing) {
				contour[ind] = contour[0];
				contour[ind + 1] = contour[1];
				contour[ind + 2] = contour[(points.Length - 1) * 6 - 1];
				contour[ind + 3] = contour[0];
				contour[ind + 4] = contour[1];
				contour[ind + 5] = contour[(points.Length - 1) * 6 - 2];
			}
			return contour;
		}
		void IRenderItemResourceHolder.Update() {
			Update();
		}
		internal void InitLineGeometry(MapPoint[] points) {
			if (points.Length < 3)
				AddLine(points);
			else {
				bool closing = points[0] == points[points.Length - 1];
				AddContour(points, closing);
			}
		}
		internal LiteVertex[] CreateContour(MapPoint[] points, bool closing) {
			if (points.Length < 3)
				return new LiteVertex[0];
			if (closing)
				points = ClosingPolygon(points);
			if(ContourWidth < 3)
				return CreateSimpleContour(points);
			return CreateComplexContour(points, closing);
		}
	}
}

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
using DevExpress.XtraMap.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.XtraMap.Drawing {
	public interface ITesselator : IDisposable {
		void Create();
		Vector3d[] Tesselate(IList<MapPoint[]> polygon);
	}
	public class GLUTesselator : MapDisposableObject, ITesselator {
		static int PointSize = MarshalHelper.SizeOf(typeof(double)) * 3;
		readonly object tesLocker = new object();
		IntPtr tess;
		IList<Vector3d> outPoints = new List<Vector3d>(1000);
		bool invalidPolygon;
		IntPtr[] unmanagedBuffer;
		List<IntPtr> combinedVerties;
		GLU.tessBeginCallback BeginCallback;
		GLU.tessEndCallback EndCallback;
		GLU.tessErrorCallback ErrorCallback;
		GLU.tessVertexCallback VertexCallback;
		GLU.tessCombineCallback CombineCallback;
		GLU.tessEdgeCallback EdgeCallback;
		void CreateGLUTess() {
			this.tess = GLU.gluNewTess();
			BeginCallback = new GLU.tessBeginCallback(BeginProc);
			EndCallback = new GLU.tessEndCallback(EndProc);
			ErrorCallback = new GLU.tessErrorCallback(ErrorProc);
			VertexCallback = new GLU.tessVertexCallback(VertexProc);
			CombineCallback = new GLU.tessCombineCallback(CombineProc);
			EdgeCallback = new GLU.tessEdgeCallback(EDGEFlagProc);
			GLU.gluTessCallback(tess, GLU.GLU_TESS_BEGIN, BeginCallback);
			GLU.gluTessCallback(tess, GLU.GLU_TESS_END, EndCallback);
			GLU.gluTessCallback(tess, GLU.GLU_TESS_ERROR, ErrorCallback);
			GLU.gluTessCallback(tess, GLU.GLU_TESS_VERTEX, VertexCallback);
			GLU.gluTessCallback(tess, GLU.GLU_TESS_COMBINE, CombineCallback);
			GLU.gluTessCallback(tess, GLU.GLU_TESS_EDGE_FLAG, EdgeCallback);
			GLU.gluTessProperty(tess, GLU.GLU_TESS_WINDING_RULE, GLU.GLU_TESS_WINDING_ODD);
		}
		Vector3d[] RunTesselate(IList<MapPoint[]> polygon) {
			Vector3d[] result = null;
			lock(tesLocker) {
				if (IsDisposed)
					return null;
				this.outPoints.Clear();
				this.invalidPolygon = false;
				this.combinedVerties = new List<IntPtr>();
				int intbufCount = polygon.Count;
				this.unmanagedBuffer = new IntPtr[intbufCount];
				GLU.gluTessBeginPolygon(tess, IntPtr.Zero);
				for(int c = 0; c < intbufCount; c++) {
					unmanagedBuffer[c] = MarshalHelper.AllocHGlobal(PointSize * polygon[c].Length);
					GLU.gluTessBeginContour(tess);
					for(int i = 0; i < polygon[c].Length; i++) {
						double[] tessP = new double[] { polygon[c][i].X, polygon[c][i].Y, .0 };
						long indexOfPoint = (long)unmanagedBuffer[c] + (PointSize * i);
						IntPtr pointPtr = new IntPtr(indexOfPoint);
						MarshalHelper.Copy(tessP, 0, pointPtr, 3);
						GLU.gluTessVertex(tess, pointPtr, pointPtr);
					}
					GLU.gluTessEndContour(tess);
				}
				GLU.gluTessEndPolygon(tess);
				FreeUnmanagedMemory();
				if (invalidPolygon) {
					outPoints.Clear();
					return null;
				}
				result = outPoints.ToArray();
				outPoints.Clear();
			}
			return result;
		}
		void FreeUnmanagedMemory() {
			for(int i = 0; i < this.unmanagedBuffer.Count(); i++)
				MarshalHelper.FreeHGlobal(this.unmanagedBuffer[i]);
			for(int i = 0; i < this.combinedVerties.Count(); i++)
				MarshalHelper.FreeHGlobal(this.combinedVerties[i]);
		}
		# region Tessalator Callbacks
		double[] static_point = new double[3];
		void VertexProc(IntPtr data) {
			MarshalHelper.Copy(data, static_point, 0, static_point.Length);
			outPoints.Add(new Vector3d(static_point[0], static_point[1], static_point[2]));
		}
		void CombineProc(IntPtr d1, IntPtr d2, IntPtr d3, IntPtr dataOut) {
			IntPtr ptr = MarshalHelper.AllocHGlobal(PointSize); 
			this.combinedVerties.Add(ptr);
			MarshalHelper.CopyMemory(ptr, d1, (uint)PointSize);
			MarshalHelper.WriteIntPtr(dataOut, ptr);
		}
		void BeginProc(int which) {
		}
		void EndProc() {
		}
		void EDGEFlagProc(bool val) {
		}
		void ErrorProc(int errorCode) {
			this.invalidPolygon = true;
		}
		#endregion
		protected override void DisposeOverride() {
			lock (tesLocker) {
				GLU.gluDeleteTess(tess);
			}
			base.DisposeOverride();
		}
		void ITesselator.Create() {
			CreateGLUTess();
		}
		Vector3d[] ITesselator.Tesselate(IList<MapPoint[]> polygon) {
			return RunTesselate(polygon);
		}
	}
}

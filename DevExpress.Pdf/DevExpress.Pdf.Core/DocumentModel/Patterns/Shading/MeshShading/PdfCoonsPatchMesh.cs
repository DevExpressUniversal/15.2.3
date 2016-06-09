#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfCoonsPatchMesh : PdfMeshShading {
		struct VertexData {
			readonly PdfPoint point5;
			readonly PdfPoint point6;
			readonly PdfPoint point7;
			readonly PdfPoint point8;
			readonly PdfPoint point9;
			readonly PdfPoint point10;
			readonly PdfPoint point11;
			readonly PdfPoint point12;
			public PdfPoint Point5 { get { return point5; } }
			public PdfPoint Point6 { get { return point6; } }
			public PdfPoint Point7 { get { return point7; } }
			public PdfPoint Point8 { get { return point8; } }
			public PdfPoint Point9 { get { return point9; } }
			public PdfPoint Point10 { get { return point10; } }
			public PdfPoint Point11 { get { return point11; } }
			public PdfPoint Point12 { get { return point12; } }
			public VertexData(PdfIntegerStreamReader streamReader) { 
				point5 = streamReader.ReadPoint();
				point6 = streamReader.ReadPoint();
				point7 = streamReader.ReadPoint();
				point8 = streamReader.ReadPoint();
				point9 = streamReader.ReadPoint();
				point10 = streamReader.ReadPoint();
				point11 = streamReader.ReadPoint();
				point12 = streamReader.ReadPoint();
			}		   
		}
		internal const int Type = 6;
		readonly IList<PdfCoonsPatch> patches = new List<PdfCoonsPatch>();
		public IList<PdfCoonsPatch> Patches { get { return patches; } }
		protected override int ShadingType { get { return Type; } }
		protected override bool HasBitsPerFlag { get { return true; } }
		internal PdfCoonsPatchMesh(PdfReaderStream stream) : base(stream) {
			if (Data.Length > 0) { 
				PdfIntegerStreamReader streamReader = CreateIntegerStreamReader();
				PdfCoonsPatch patch = null;
				for (;;) {
					int edgeFlag = streamReader.ReadEdgeFlag();
					if (edgeFlag > 3)
						PdfDocumentReader.ThrowIncorrectDataException();
					else {
						VertexData vertexData;
						PdfBezierCurve curve;
						if (edgeFlag == 0) {
							PdfPoint point1 = streamReader.ReadPoint();
							PdfPoint point2 = streamReader.ReadPoint();
							PdfPoint point3 = streamReader.ReadPoint();
							PdfPoint point4 = streamReader.ReadPoint();
							vertexData = new VertexData(streamReader);
							PdfColor color1 = streamReader.ReadColor();
							PdfColor color2 = streamReader.ReadColor();
							curve = new PdfBezierCurve(new PdfVertex(point1, color1), point2, point3, new PdfVertex(point4, color2));
						} 
						else {
							if (patch == null)
								PdfDocumentReader.ThrowIncorrectDataException();
							vertexData = new VertexData(streamReader);
							switch (edgeFlag) {
								case 1:
									curve = patch.Top;
									break;
								case 2:
									curve = patch.Right;
									break;
								default:
									curve = patch.Bottom;
									break;
							}
						}
						PdfColor color3 = streamReader.ReadColor();
						PdfColor color4 = streamReader.ReadColor();
						patch = new PdfCoonsPatch(curve, 
							new PdfBezierCurve(curve.Vertex2, vertexData.Point5, vertexData.Point6, new PdfVertex(vertexData.Point7, color3)),
							new PdfBezierCurve(new PdfVertex(vertexData.Point7, color3), vertexData.Point8, vertexData.Point9, new PdfVertex(vertexData.Point10, color4)),
							new PdfBezierCurve(new PdfVertex(vertexData.Point10, color4), vertexData.Point11, vertexData.Point12, curve.Vertex1));
						patches.Add(patch);
					}
					if (!streamReader.IgnoreExtendedBits())
						break;
				}
			}
		}
	}
}

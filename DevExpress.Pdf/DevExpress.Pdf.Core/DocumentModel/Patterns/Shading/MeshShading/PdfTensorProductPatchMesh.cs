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
	public class PdfTensorProductPatchMesh : PdfMeshShading {
		struct VertexData { 
			readonly PdfPoint p13;
			readonly PdfPoint p23;
			readonly PdfPoint p33;
			readonly PdfPoint p32;
			readonly PdfPoint p31;
			readonly PdfPoint p30;
			readonly PdfPoint p20;
			readonly PdfPoint p10;
			readonly PdfPoint p11;
			readonly PdfPoint p12;
			readonly PdfPoint p22;
			readonly PdfPoint p21;
			public PdfPoint P13 { get { return p13; } }
			public PdfPoint P23 { get { return p23; } }
			public PdfPoint P33 { get { return p33; } }
			public PdfPoint P32 { get { return p32; } }
			public PdfPoint P31 { get { return p31; } }
			public PdfPoint P30 { get { return p30; } }
			public PdfPoint P20 { get { return p20; } }
			public PdfPoint P10 { get { return p10; } }
			public PdfPoint P11 { get { return p11; } }
			public PdfPoint P12 { get { return p12; } }
			public PdfPoint P22 { get { return p22; } }
			public PdfPoint P21 { get { return p21; } }
			public VertexData(PdfIntegerStreamReader streamReader) {
				p13 = streamReader.ReadPoint();
				p23 = streamReader.ReadPoint();
				p33 = streamReader.ReadPoint();
				p32 = streamReader.ReadPoint();
				p31 = streamReader.ReadPoint();
				p30 = streamReader.ReadPoint();
				p20 = streamReader.ReadPoint();
				p10 = streamReader.ReadPoint();
				p11 = streamReader.ReadPoint();
				p12 = streamReader.ReadPoint();
				p22 = streamReader.ReadPoint();
				p21 = streamReader.ReadPoint();			  
			}
		}
		internal const int Type = 7;
		readonly IList<PdfTensorProductPatch> patches = new List<PdfTensorProductPatch>();
		public IList<PdfTensorProductPatch> Patches { get { return patches; } }
		protected override int ShadingType { get { return Type; } }
		protected override bool HasBitsPerFlag { get { return true; } }
		internal PdfTensorProductPatchMesh(PdfReaderStream stream) : base(stream) {
			if (Data.Length > 0) { 
				PdfIntegerStreamReader streamReader = CreateIntegerStreamReader();
				PdfTensorProductPatch patch = null;
				for (;;) {
					int edgeFlag = streamReader.ReadEdgeFlag();
					if (edgeFlag > 3) 
						PdfDocumentReader.ThrowIncorrectDataException();
					PdfPoint point1;
					PdfPoint point2;
					PdfPoint point3;
					PdfPoint point4;
					VertexData vertexData;
					PdfColor color1;
					PdfColor color2;
					if (edgeFlag == 0) {
						point1 = streamReader.ReadPoint();
						point2 = streamReader.ReadPoint();
						point3 = streamReader.ReadPoint();
						point4 = streamReader.ReadPoint();
						vertexData = new VertexData(streamReader);
						color1 = streamReader.ReadColor();
						color2 = streamReader.ReadColor();
					} 
					else {
						if (patch == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						PdfPoint[,] controlPoints = patch.ControlPoints;
						PdfColor[] colors = patch.Colors;
						switch (edgeFlag) {
							case 1:
								point1 = controlPoints[0, 3];
								point2 = controlPoints[1, 3];
								point3 = controlPoints[2, 3];
								point4 = controlPoints[3, 3];
								color1 = colors[1];
								color2 = colors[2];
								break;
							case 2:
								point1 = controlPoints[3, 3];
								point2 = controlPoints[3, 2];
								point3 = controlPoints[3, 1];
								point4 = controlPoints[3, 0];
								color1 = colors[2];
								color2 = colors[3];
								break;
							default:
								point1 = controlPoints[3, 0];
								point2 = controlPoints[2, 0];
								point3 = controlPoints[1, 0];
								point4 = controlPoints[0, 0];
								color1 = colors[3];
								color2 = colors[0];
								break;
						}
						vertexData = new VertexData(streamReader);
					}
					PdfPoint[,] patchControlPoints = new PdfPoint[,] { 
						{ point1, point2, point3, point4 },
						{ vertexData.P10, vertexData.P11, vertexData.P12, vertexData.P13 }, 
						{ vertexData.P20, vertexData.P21, vertexData.P22, vertexData.P23 },
						{ vertexData.P30, vertexData.P31, vertexData.P32, vertexData.P33 } 
					};
					PdfColor color3 = streamReader.ReadColor();
					PdfColor color4 = streamReader.ReadColor();
					patch = new PdfTensorProductPatch(patchControlPoints, color1, color2, color3, color4); 
					patches.Add(patch);
					if (!streamReader.IgnoreExtendedBits())
						break;   
				}
			}
		}
	}
}

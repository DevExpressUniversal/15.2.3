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
	public class PdfFreeFormGourandShadedTriangleMesh : PdfGourandShadedTriangleMesh  {
		internal const int Type = 4;
		protected override int ShadingType { get { return Type; } }
		protected override bool HasBitsPerFlag { get { return true; } }
		internal PdfFreeFormGourandShadedTriangleMesh(PdfReaderStream stream) : base(stream) {
			PdfIntegerStreamReader streamReader = CreateIntegerStreamReader();
			int vertexCount = Data.Length / streamReader.BytesPerVertex;
			if (vertexCount > 0 && vertexCount < 3) 
				PdfDocumentReader.ThrowIncorrectDataException();
			IList<PdfTriangle> triangles = Triangles;
			PdfTriangle previousTriangle = null;
			for (int remainVertexCount = vertexCount; remainVertexCount > 0;) {
				PdfTriangle triangle;
				switch (streamReader.ReadEdgeFlag()) {
					case 0: 
						if (remainVertexCount < 3)
							PdfDocumentReader.ThrowIncorrectDataException();
						PdfVertex vertex1 = streamReader.ReadVertex();
						streamReader.ReadEdgeFlag();
						PdfVertex vertex2 = streamReader.ReadVertex();
						streamReader.ReadEdgeFlag();
						PdfVertex vertex3 = streamReader.ReadVertex();
						triangle = new PdfTriangle(vertex1, vertex2, vertex3);
						remainVertexCount -= 3;
						break;
					case 1: 
						if (previousTriangle == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						triangle = new PdfTriangle(previousTriangle.Vertex2, previousTriangle.Vertex3, streamReader.ReadVertex());
						remainVertexCount--;
						break;			   
					case 2: 
						if (previousTriangle == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						triangle = new PdfTriangle(previousTriangle.Vertex1, previousTriangle.Vertex3, streamReader.ReadVertex());
						remainVertexCount--;
						break;
					default: 
						PdfDocumentReader.ThrowIncorrectDataException();
						triangle = null;
						break;
				}
				triangles.Add(triangle);
				previousTriangle = triangle;
			}
		}
		internal PdfFreeFormGourandShadedTriangleMesh(IList<PdfTriangle> triangles, int bitsPerFlag, int bitsPerComponent, int bitsPerCoordinate, PdfDecodeRange decodeX, PdfDecodeRange decodeY, PdfDecodeRange[] decodeC, PdfObjectList<PdfCustomFunction> functions)
			: base(triangles, bitsPerFlag, bitsPerComponent, bitsPerCoordinate, decodeX, decodeY, decodeC, functions){
		}
	}
}

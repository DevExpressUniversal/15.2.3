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
	public class PdfLatticeFormGourandShadedTriangleMesh : PdfGourandShadedTriangleMesh  {
		internal const int Type = 5;
		const string verticesPerRowDictionaryKey = "VerticesPerRow";
		readonly int verticesPerRow;
		protected override int ShadingType { get { return Type; } }
		internal PdfLatticeFormGourandShadedTriangleMesh(PdfReaderStream stream) : base(stream) {
			int? valueVerticesPerRowDictionaryKey = stream.Dictionary.GetInteger(verticesPerRowDictionaryKey);
			if (!valueVerticesPerRowDictionaryKey.HasValue || valueVerticesPerRowDictionaryKey.Value < 2)
				PdfDocumentReader.ThrowIncorrectDataException();
			verticesPerRow = valueVerticesPerRowDictionaryKey.Value;
			PdfIntegerStreamReader streamReader = CreateIntegerStreamReader();
			int rowByteCount = streamReader.BytesPerVertex * verticesPerRow;
			int dataLength = Data.Length;
			int dataRowCount = dataLength / rowByteCount;
			if (dataRowCount == 1 || dataLength % rowByteCount > 0) 
				PdfDocumentReader.ThrowIncorrectDataException();
			if (dataRowCount != 0) { 
				IList<PdfTriangle> triangles = Triangles;
				PdfVertex[] previousVertexRow = new PdfVertex[verticesPerRow];
				for (int i = 0; i < verticesPerRow; i++) 
					previousVertexRow[i] = streamReader.ReadVertex();
				for (int i = 1; i < dataRowCount; i++) {
					PdfVertex upLeftVertex = previousVertexRow[0];
					PdfVertex leftVertex = streamReader.ReadVertex();
					previousVertexRow[0] = leftVertex;
					for (int j = 1; j < verticesPerRow; j++) { 
						PdfVertex upVertex = previousVertexRow[j];
						PdfVertex vertex = streamReader.ReadVertex();
						previousVertexRow[j] = vertex;
						triangles.Add(new PdfTriangle(upLeftVertex, upVertex, leftVertex)); 
						triangles.Add(new PdfTriangle(upVertex, leftVertex, vertex)); 
						leftVertex = vertex;
						upLeftVertex = upVertex;
					}
				}
			}
		}
		protected override PdfDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(verticesPerRowDictionaryKey, verticesPerRow);
			return dictionary;
		}
	}
}

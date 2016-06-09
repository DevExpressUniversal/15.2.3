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

using System.IO;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public abstract class PdfGourandShadedTriangleMesh : PdfMeshShading {
		readonly IList<PdfTriangle> triangles = new List<PdfTriangle>();
		public IList<PdfTriangle> Triangles { get { return triangles; } }
		protected PdfGourandShadedTriangleMesh(PdfReaderStream stream)
			: base(stream) {
		}
		protected PdfGourandShadedTriangleMesh(IList<PdfTriangle> triangles, int bitsPerFlag, int bitsPerComponent, int bitsPerCoordinate, PdfDecodeRange decodeX, PdfDecodeRange decodeY, PdfDecodeRange[] decodeC, PdfObjectList<PdfCustomFunction> functions)
			: base(bitsPerFlag, bitsPerComponent, bitsPerCoordinate, decodeX, decodeY, decodeC, functions) {
			this.triangles = triangles;
		}
		protected override byte[] GetData() {
			int vertexSize = BitsPerFlag + BitsPerCoordinate * 2 + BitsPerComponent * DecodeC.Length;
			int vertexSizeInBytes = vertexSize / 8 + (vertexSize % 8 != 0 ? 1 : 0);
			byte[] data = new byte[vertexSizeInBytes * 3 * triangles.Count];
			using (PdfTriangleWriter writer = new PdfTriangleWriter(new MemoryStream(data), DecodeX, DecodeY, DecodeC, BitsPerCoordinate, BitsPerFlag, BitsPerComponent)) {
				foreach (PdfTriangle triangle in triangles)
					writer.Write(triangle);
			}
			return data;
		}
	}
}

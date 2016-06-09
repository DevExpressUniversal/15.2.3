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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfIntegerStreamReader {
		readonly int bitsPerFlag;
		readonly int bitsPerCoordinate;
		readonly int bitsPerComponent;
		readonly PdfDecodeRange decodeX;
		readonly PdfDecodeRange decodeY;
		readonly PdfDecodeRange[] decodeC;
		readonly PdfIntegerStream integerStream;
		public int BytesPerVertex { 
			get {
				int bitsPerVertex = bitsPerFlag + bitsPerCoordinate * 2 + bitsPerComponent * decodeC.Length;
				int bytesPerVertex = bitsPerVertex / 8;
				if (bitsPerVertex % 8 > 0)
					bytesPerVertex++;
				return bytesPerVertex;
			}
		}
		public PdfIntegerStreamReader(int bitsPerFlag, int bitsPerCoordinate, int bitsPerComponent, PdfDecodeRange decodeX, PdfDecodeRange decodeY, PdfDecodeRange[] decodeC, byte[] data) { 
			this.bitsPerFlag = bitsPerFlag;
			this.bitsPerCoordinate = bitsPerCoordinate;
			this.bitsPerComponent = bitsPerComponent;
			this.decodeX = decodeX;
			this.decodeY = decodeY;
			this.decodeC = decodeC;
			integerStream = new PdfIntegerStream(data);
		}
		public PdfPoint ReadPoint() { 
			double x = integerStream.GetInteger(bitsPerCoordinate);
			double y = integerStream.GetInteger(bitsPerCoordinate);
			return new PdfPoint(decodeX.DecodeValue(x), decodeY.DecodeValue(y));
		}
		public PdfColor ReadColor() { 
			int colorComponentsCount = decodeC.Length;
			double[] colorComponents = new double[colorComponentsCount];
			for (int i = 0; i < colorComponentsCount; i++)
				colorComponents[i] = decodeC[i].DecodeValue(integerStream.GetInteger(bitsPerComponent));
			return new PdfColor(colorComponents);
		}
		public int ReadEdgeFlag() { 
			return integerStream.GetInteger(bitsPerFlag);
		}
		public PdfVertex ReadVertex() { 
			PdfPoint point = ReadPoint();
			PdfColor color = ReadColor();
			PdfVertex vertex = new PdfVertex(point, color); 
			integerStream.IgnoreExtendedBits();
			return vertex;
		}
		public bool IgnoreExtendedBits() {
			return integerStream.IgnoreExtendedBits();
		}
	}
}

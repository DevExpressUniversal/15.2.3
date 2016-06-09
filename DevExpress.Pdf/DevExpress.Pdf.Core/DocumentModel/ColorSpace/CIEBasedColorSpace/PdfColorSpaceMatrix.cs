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
	public class PdfColorSpaceMatrix {
		readonly double xa = 1;
		readonly double ya = 0;
		readonly double za = 0;
		readonly double xb = 0;
		readonly double yb = 1;
		readonly double zb = 0;
		readonly double xc = 0;
		readonly double yc = 0;
		readonly double zc = 1;
		public double Xa { get { return xa; } }
		public double Ya { get { return ya; } }
		public double Za { get { return za; } }
		public double Xb { get { return xb; } }
		public double Yb { get { return yb; } }
		public double Zb { get { return zb; } }
		public double Xc { get { return xc; } }
		public double Yc { get { return yc; } }
		public double Zc { get { return zc; } }
		public bool IsIdentity { get { return xa == 1 && ya == 0 && za == 0 && xb == 0 && yb == 1 && zb == 0 && xc == 0 && yc == 0 && zc == 1; } }
		internal PdfColorSpaceMatrix() {
		}
		internal PdfColorSpaceMatrix(IList<object> array) {
			if (array.Count != 9)
				PdfDocumentReader.ThrowIncorrectDataException();
			xa = PdfDocumentReader.ConvertToDouble(array[0]);
			ya = PdfDocumentReader.ConvertToDouble(array[1]);
			za = PdfDocumentReader.ConvertToDouble(array[2]);
			xb = PdfDocumentReader.ConvertToDouble(array[3]);
			yb = PdfDocumentReader.ConvertToDouble(array[4]);
			zb = PdfDocumentReader.ConvertToDouble(array[5]);
			xc = PdfDocumentReader.ConvertToDouble(array[6]);
			yc = PdfDocumentReader.ConvertToDouble(array[7]);
			zc = PdfDocumentReader.ConvertToDouble(array[8]);
		}
		internal double[] ToArray() {
			return new double[] { xa, ya, za, xb, yb, zb, xc, yc, zc };
		}
	}
}

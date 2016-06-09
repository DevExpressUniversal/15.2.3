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
	public class PdfQuadrilateral {
		const string dictionaryKey = "QuadPoints";
		internal static IList<PdfQuadrilateral> ParseArray(PdfReaderDictionary dictionary) {
			IList<object> quadPoints = dictionary.GetArray(dictionaryKey);
			if (quadPoints == null)
				return null;
			int count = quadPoints.Count;
			if (count % 8 != 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			count /= 8;
			List<PdfQuadrilateral> region = new List<PdfQuadrilateral>(count);
			for (int i = 0, index = 0; i < count; i++, index += 2) {
				PdfPoint p1 = PdfDocumentReader.CreatePoint(quadPoints, index);
				index += 2;
				PdfPoint p2 = PdfDocumentReader.CreatePoint(quadPoints, index);
				index += 2;
				PdfPoint p3 = PdfDocumentReader.CreatePoint(quadPoints, index);
				index += 2;
				PdfPoint p4 = PdfDocumentReader.CreatePoint(quadPoints, index);
				region.Add(new PdfQuadrilateral(p1, p2, p3, p4));
			}
			return region;
		}
		internal static void Write(PdfWriterDictionary dictionary, IList<PdfQuadrilateral> region) {
			if (region != null) {
				List<double> array = new List<double>();
				foreach (PdfQuadrilateral q in region)
					array.AddRange(q.Data);
				dictionary.Add(dictionaryKey, array);
			}
		}
		readonly PdfPoint p1;
		readonly PdfPoint p2;
		readonly PdfPoint p3;
		readonly PdfPoint p4;
		public PdfPoint P1 { get { return p1; } }
		public PdfPoint P2 { get { return p2; } }
		public PdfPoint P3 { get { return p3; } }
		public PdfPoint P4 { get { return p4; } }
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1025")]
#endif
		public PdfQuadrilateral(PdfPoint p1, PdfPoint p2, PdfPoint p3, PdfPoint p4) {
			this.p1 = p1;
			this.p2 = p2;
			this.p3 = p3;
			this.p4 = p4;
		}
		internal double[] Data { get { return new double[] { p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y }; } }
	}
}

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
	public class PdfFunctionBasedShading : PdfShading {
		internal const int Type = 1;
		const string domainDictionaryKey = "Domain";
		const string matrixDictionaryKey = "Matrix";
		readonly PdfRange domainX;
		readonly PdfRange domainY;
		readonly PdfTransformationMatrix matrix = new PdfTransformationMatrix();
		public PdfRange DomainX { get { return domainX; } }
		public PdfRange DomainY { get { return domainY; } }
		public PdfTransformationMatrix Matrix { get { return matrix; } }
		protected override int ShadingType { get { return Type; } }
		protected override int FunctionDomainDimension { get { return 2; } }
		internal PdfFunctionBasedShading(PdfReaderDictionary dictionary) : base(dictionary) {
			IList<object> domainValues = dictionary.GetArray(domainDictionaryKey);
			if (domainValues == null) {
				domainX = new PdfRange(0.0, 1.0);
				domainY = new PdfRange(0.0, 1.0);
			}
			else {
				if (domainValues.Count != 4) 
					PdfDocumentReader.ThrowIncorrectDataException();
				domainX = PdfDocumentReader.CreateDomain(domainValues, 0);
				domainY = PdfDocumentReader.CreateDomain(domainValues, 2);
			}
			IList<object> matrixValues = dictionary.GetArray(matrixDictionaryKey);
			matrix = matrixValues == null ? new PdfTransformationMatrix() : new PdfTransformationMatrix(matrixValues);
		}
		protected override PdfDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(domainDictionaryKey, new object[] { domainX.Min, domainX.Max, domainY.Min, domainY.Max });
			dictionary.Add(matrixDictionaryKey, matrix.Data);
			return dictionary;
		}
	}
}

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
	public class PdfAxialShading : PdfGradientShading {
		internal const int Type = 2;
		readonly PdfPoint axisStart;
		readonly PdfPoint axisEnd;
		public PdfPoint AxisStart { get { return axisStart; } }
		public PdfPoint AxisEnd { get { return axisEnd; } }
		protected override int ShadingType { get { return Type; } }
		internal PdfAxialShading(PdfReaderDictionary dictionary) : base(dictionary) {
			IList<object> coords = dictionary.GetArray(CoordsDictionaryKey);
			if (coords == null || coords.Count != 4)
				PdfDocumentReader.ThrowIncorrectDataException();
			axisStart = PdfDocumentReader.CreatePoint(coords, 0);
			axisEnd = PdfDocumentReader.CreatePoint(coords, 2);
		}
		internal PdfAxialShading(PdfPoint axisStart, PdfPoint axisEnd, PdfObjectList<PdfCustomFunction> blendFunctions) : base(blendFunctions) {
			this.axisStart = axisStart;
			this.axisEnd = axisEnd;
		}
		protected override PdfDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(CoordsDictionaryKey, new object[] { axisStart.X, axisStart.Y, axisEnd.X, axisEnd.Y });
			return dictionary;
		}
	}
}

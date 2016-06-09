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
	public abstract class PdfGradientShading : PdfShading {
		protected const string CoordsDictionaryKey = "Coords";
		const string domainDictionaryKey = "Domain";
		const string extendDictionaryKey = "Extend";
		readonly PdfRange domain;
		readonly bool extendX;
		readonly bool extendY;
		public PdfRange Domain { get { return domain; } }
		public bool ExtendX { get { return extendX; } }
		public bool ExtendY { get { return extendY; } }
		protected PdfGradientShading(PdfReaderDictionary dictionary) : base(dictionary) {
			IList<object> domainArray = dictionary.GetArray(domainDictionaryKey);
			if (domainArray == null) 
				domain = new PdfRange(0.0, 1.0);
			else {
				if (domainArray.Count != 2)
					PdfDocumentReader.ThrowIncorrectDataException();
				domain = PdfDocumentReader.CreateDomain(domainArray, 0);
			}
			IList<object> extendArray = dictionary.GetArray(extendDictionaryKey);
			if (extendArray != null) {
				if (extendArray.Count != 2)
					PdfDocumentReader.ThrowIncorrectDataException();
				object extendXValue = extendArray[0];
				object extendYValue = extendArray[1];
				if (!(extendXValue is bool) || !(extendYValue is bool))
					PdfDocumentReader.ThrowIncorrectDataException();
				extendX = (bool)extendXValue;
				extendY = (bool)extendYValue;
			}
		}
		protected PdfGradientShading(PdfObjectList<PdfCustomFunction> blendFunctions) : base(blendFunctions) {
			domain = new PdfRange(0, 1);
		}
		protected override PdfDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfDictionary dictionary = base.CreateDictionary(objects);
			if (domain.Min != 0.0 || domain.Max != 1.0)
				dictionary.Add(domainDictionaryKey, new object[] { domain.Min, domain.Max });
			if (extendX || extendY)
				dictionary.Add(extendDictionaryKey, new object[] { extendX, extendY });
			return dictionary;
		}
	}
}

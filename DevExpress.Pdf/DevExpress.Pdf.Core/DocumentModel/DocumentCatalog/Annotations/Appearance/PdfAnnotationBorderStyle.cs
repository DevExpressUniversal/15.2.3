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
	public class PdfAnnotationBorderStyle : PdfObject {
		internal const string DictionaryKey = "BS";
		const string widthDictionaryKey = "W";
		const string styleDictionaryKey = "S";
		const string lineStyleDictionaryLey = "D";
		const string solidStyleName = "S";
		const string dashedStyleName = "D";
		const string beveledStyleName = "B";
		const string insetStyleName = "I";
		const string underlineStyleName = "U";
		internal static PdfAnnotationBorderStyle Parse(PdfReaderDictionary dictionary) {
			PdfReaderDictionary bs = dictionary.GetDictionary(DictionaryKey);
			return bs == null ? null : new PdfAnnotationBorderStyle(bs);
		}
		internal static PdfLineStyle ParseLineStyle(IList<object> array) {
			if (array != null) {
				int count = array.Count;
				if (count > 1 || (count == 1 && PdfDocumentReader.ConvertToDouble(array[0]) != 0))
					return PdfLineStyle.CreateDashed(PdfLineStyle.ParseDashPattern(array), 0);
			}
			return PdfLineStyle.CreateSolid();
		}
		readonly double width;
		readonly string styleName;
		readonly PdfLineStyle lineStyle;
		public double Width { get { return width; } }
		public string StyleName { get { return styleName; } }
		public PdfLineStyle LineStyle { get { return lineStyle; } }
		PdfAnnotationBorderStyle(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			width = dictionary.GetNumber(widthDictionaryKey) ?? 0;
			if ((type != null && type != "Border") || width < 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			styleName = dictionary.GetName(styleDictionaryKey) ?? solidStyleName;
			lineStyle = ParseLineStyle(dictionary.GetArray(lineStyleDictionaryLey));
			if (!lineStyle.IsDashed && styleName == dashedStyleName)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.Add(widthDictionaryKey, width, 0);
			result.AddName(styleDictionaryKey, styleName, solidStyleName);
			result.Add(lineStyleDictionaryLey, lineStyle.Data[0]);
			return result;
		}
	}
}

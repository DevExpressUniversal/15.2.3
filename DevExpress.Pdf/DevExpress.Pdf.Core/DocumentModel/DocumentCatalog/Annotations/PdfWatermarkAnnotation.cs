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
	public class PdfWatermarkAnnotation : PdfAnnotation {
		internal const string Type = "Watermark";
		const string fixedPrintDictionaryKey = "FixedPrint";
		const string horizontalTranslationDictionaryKey = "H";
		const string verticalTranslationDictionaryKey = "V";
		readonly double horizontalTranslationPercent;
		readonly double verticalTranslationPercent;
		public double HorizontalTranslationPercent { get { return horizontalTranslationPercent; } }
		public double VerticalTranslationPercent { get { return verticalTranslationPercent; } }
		protected override string AnnotationType { get { return Type; } }
		internal PdfWatermarkAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			PdfReaderDictionary fixedPrintDictionary = dictionary.GetDictionary(fixedPrintDictionaryKey);
			if (fixedPrintDictionary != null) {
				if (fixedPrintDictionary.GetName(PdfDictionary.DictionaryTypeKey) != fixedPrintDictionaryKey)
					PdfDocumentReader.ThrowIncorrectDataException();
				horizontalTranslationPercent = (fixedPrintDictionary.GetNumber(horizontalTranslationDictionaryKey) ?? 0) * 100;
				verticalTranslationPercent = (fixedPrintDictionary.GetNumber(verticalTranslationDictionaryKey) ?? 0) * 100;
				if (horizontalTranslationPercent < 0 || horizontalTranslationPercent > 100 || verticalTranslationPercent < 0 || verticalTranslationPercent > 100)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			if (horizontalTranslationPercent != 0 || verticalTranslationPercent != 0) {
				PdfWriterDictionary fixedPrintDictionary = new PdfWriterDictionary(collection);
				fixedPrintDictionary.AddName(PdfDictionary.DictionaryTypeKey, fixedPrintDictionaryKey);
				fixedPrintDictionary.Add(horizontalTranslationDictionaryKey, horizontalTranslationPercent / 100, 0.0);
				fixedPrintDictionary.Add(verticalTranslationDictionaryKey, verticalTranslationPercent / 100, 0.0);
				dictionary.Add(fixedPrintDictionaryKey, fixedPrintDictionary);
			}
			return dictionary;
		}
	}
}

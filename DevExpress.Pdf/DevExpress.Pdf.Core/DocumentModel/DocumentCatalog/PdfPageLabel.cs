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

using System;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfPageNumberingStyle.None)]
	public enum PdfPageNumberingStyle {
		None,
		[PdfFieldName("D")]
		DecimalArabic,
		[PdfFieldName("R")]
		UppercaseRoman,
		[PdfFieldName("r")]
		LowercaseRoman,
		[PdfFieldName("A")]
		UppercaseLetters,
		[PdfFieldName("a")]
		LowercaseLetters
	}
	public class PdfPageLabel : PdfObject {
		const string dictionaryName = "PageLabel";
		const string styleKey = "S";
		const string prefixKey = "P";
		const string firstNumberKey = "St";
		readonly PdfPageNumberingStyle numberingStyle;
		readonly string prefix;
		readonly int firstNumber = 1;
		public PdfPageNumberingStyle NumberingStyle { get { return numberingStyle; } }
		public string Prefix { get { return prefix; } }
		public int FirstNumber { get { return firstNumber; } }
		internal PdfPageLabel(PdfObjectCollection objects, object value) {
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary == null) {
				PdfObjectReference reference = value as PdfObjectReference;
				if (reference == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				dictionary = objects.GetDictionary(reference.Number);
				if (dictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (type != null && type != dictionaryName)
				PdfDocumentReader.ThrowIncorrectDataException();
			numberingStyle = dictionary.GetEnumName<PdfPageNumberingStyle>(styleKey);
			prefix = dictionary.GetString(prefixKey) ?? String.Empty;
			firstNumber = dictionary.GetInteger(firstNumberKey) ?? 1;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary result = new PdfWriterDictionary(objects);
			result.Add(PdfDictionary.DictionaryTypeKey, new PdfName(dictionaryName));
			result.AddEnumName(styleKey, numberingStyle);
			result.AddNotNullOrEmptyString(prefixKey, prefix);
			result.Add(firstNumberKey, firstNumber, 1);
			return result;
		}
	}
}

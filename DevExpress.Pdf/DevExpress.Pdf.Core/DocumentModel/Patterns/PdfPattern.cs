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
	public abstract class PdfPattern : PdfObject {
		const string patternTypeDictionaryKey = "PatternType";
		const string matrixDictionaryKey = "Matrix";
		internal static PdfPattern Parse(object value) {
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary != null)
				return new PdfShadingPattern(dictionary);
			PdfReaderStream stream = value as PdfReaderStream;
			if (stream == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return new PdfTilingPattern(stream);
		}
		readonly PdfTransformationMatrix matrix;
		public PdfTransformationMatrix Matrix { get { return matrix; } }
		protected abstract int PatternType { get; }
		protected PdfPattern(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			int? patternType = dictionary.GetInteger(patternTypeDictionaryKey);
			if ((type != null && type != "Pattern") || !patternType.HasValue || patternType.Value != PatternType)
				PdfDocumentReader.ThrowIncorrectDataException();
			matrix = new PdfTransformationMatrix(dictionary.GetArray(matrixDictionaryKey));
		}
		protected PdfPattern(PdfTransformationMatrix matrix) {
			this.matrix = matrix;
		}
		protected virtual PdfWriterDictionary GetDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(patternTypeDictionaryKey, PatternType);
			if (!matrix.IsDefault)
				dictionary.Add(matrixDictionaryKey, matrix.Data);
			return dictionary;
		}
	}
}

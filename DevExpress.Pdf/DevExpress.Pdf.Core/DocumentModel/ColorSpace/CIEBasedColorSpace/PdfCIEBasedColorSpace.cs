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
	public abstract class PdfCIEBasedColorSpace : PdfCustomColorSpace {
		protected const string GammaDictionaryKey = "Gamma";
		const string writePointDictionaryKey = "WhitePoint";
		const string blackPointDictionaryKey = "BlackPoint";
		protected static PdfReaderDictionary ResolveColorSpaceDictionary(PdfObjectCollection collection, IList<object> array) {
			if (array.Count != 2)
				PdfDocumentReader.ThrowIncorrectDataException();
			object value = array[1];
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary != null)
				return dictionary;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			dictionary = collection.GetDictionary(reference.Number);
			if (dictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return dictionary;
		}
		protected static int FillResult(byte[] result, double[] components, int position) {
			result[position++] = PdfMathUtils.ToByte(components[0] * 255.0);
			result[position++] = PdfMathUtils.ToByte(components[1] * 255.0);
			result[position++] = PdfMathUtils.ToByte(components[2] * 255.0);
			return position;
		}
		readonly PdfCIEColor whitePoint;
		readonly PdfCIEColor blackPoint;
		public PdfCIEColor WhitePoint { get { return whitePoint; } }
		public PdfCIEColor BlackPoint { get { return blackPoint; } }
		protected abstract string Name { get; }
		protected PdfCIEBasedColorSpace(PdfReaderDictionary dictionary) {
			IList<object> whitePointArray = dictionary.GetArray(writePointDictionaryKey);
			if (whitePointArray == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			whitePoint = new PdfCIEColor(whitePointArray);
			if (whitePoint.X <= 0 || whitePoint.Y != 1 || whitePoint.Z <= 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			IList<object> blackPointArray = dictionary.GetArray(blackPointDictionaryKey);
			blackPoint = blackPointArray == null ? new PdfCIEColor() : new PdfCIEColor(blackPointArray);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			return new object[] { new PdfName(Name), CreateDictionary(collection) };
		}
		protected virtual PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.Add(writePointDictionaryKey, whitePoint.ToArray());
			if (!blackPoint.IsEmpty)
				dictionary.Add(blackPointDictionaryKey, blackPoint.ToArray());
			return dictionary;
		}
	}
}

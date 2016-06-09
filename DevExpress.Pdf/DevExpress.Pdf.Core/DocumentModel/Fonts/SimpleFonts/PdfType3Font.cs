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
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfType3Font : PdfSimpleFont {
		internal const string Name = "Type3";
		const string fontBBoxDictionaryKey = "FontBBox";
		const string fontMatrixDictionaryKey = "FontMatrix";
		const string charProcsDictionaryKey = "CharProcs";
		const string resourcesDictionaryKey = "Resources";
		readonly PdfRectangle fontBBox;
		readonly PdfTransformationMatrix fontMatrix;
		readonly Dictionary<string, PdfCommandList> charProcs = new Dictionary<string, PdfCommandList>();
		readonly PdfResources resources;
		public PdfRectangle FontBBox { get { return fontBBox; } }
		public PdfTransformationMatrix FontMatrix { get { return fontMatrix; } }
		public IDictionary<string, PdfCommandList> CharProcs { get { return charProcs; } }
		protected internal override string Subtype { get { return Name; } }
		protected internal override bool HasSizeAttributes { get { return false; } }
		internal PdfType3Font(PdfReaderStream toUnicode, PdfReaderDictionary fontDescriptor, PdfSimpleFontEncoding encoding, int firstChar, int lastChar, double[] widths, PdfReaderDictionary dictionary)
				: base(String.Empty, toUnicode, fontDescriptor, encoding, firstChar, lastChar, widths) {
			IList<object> bboxArray = dictionary.GetArray(fontBBoxDictionaryKey);
			IList<object> matrixArray = dictionary.GetArray(fontMatrixDictionaryKey);
			PdfReaderDictionary charProcsDictionary = dictionary.GetDictionary(charProcsDictionaryKey);
			if (bboxArray == null || matrixArray == null || charProcsDictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			fontBBox = new PdfRectangle(bboxArray);
			fontMatrix = new PdfTransformationMatrix(matrixArray);
			resources = dictionary.GetResources(resourcesDictionaryKey, null, false);
			PdfObjectCollection objects = charProcsDictionary.Objects;
			foreach (KeyValuePair<string, object> pair in charProcsDictionary) {
				object value = pair.Value;
				PdfReaderStream stream = value as PdfReaderStream;
				if (stream == null) {
					PdfObjectReference reference = value as PdfObjectReference;
					if (reference == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					stream = objects.GetStream(reference.Number);
				}
				if (stream != null)
					charProcs.Add(pair.Key, PdfType3FontContentStreamParser.ParseGlyph(resources, stream.GetData(true)));
			}
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(fontBBoxDictionaryKey, FontBBox);
			dictionary.Add(fontMatrixDictionaryKey, fontMatrix.Data);
			dictionary.Add(charProcsDictionaryKey, PdfElementsDictionaryWriter.Write(charProcs, value => objects.AddStream(((PdfCommandList)value).ToStream(resources))));
			if (resources != null)
				dictionary.Add(resourcesDictionaryKey, resources);
			return dictionary;
		}
	}
}

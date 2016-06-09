﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	public class PdfRichMediaInstance: PdfObject {
		const string mediaInstanceDictionaryName = "RichMediaInstance";
		const string assetRefKey = "Asset";
		const string paramsKey = "Params";
		readonly PdfFileSpecification asset;
		readonly PdfRichMediaContentType contentType;
		readonly PdfRichMediaParams parameters;
		public PdfFileSpecification Asset { get { return asset; } }
		public PdfRichMediaContentType ContentType { get { return contentType; } }
		public PdfRichMediaParams Parameters { get { return parameters; } }
		internal PdfRichMediaInstance(IDictionary<string, PdfFileSpecification> assets, PdfReaderDictionary dictionary) : base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			PdfObjectReference reference = dictionary.GetObjectReference(assetRefKey);
			if ((type != null && type != mediaInstanceDictionaryName) || assets == null || reference == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			int assetNumber = reference.Number;
			foreach (KeyValuePair<string, PdfFileSpecification> pair in (IEnumerable<KeyValuePair<string, PdfFileSpecification>>)assets) {
				PdfFileSpecification specification = pair.Value;
				if (specification.ObjectNumber == assetNumber) {
					asset = specification;
					break;
				}
			}
			if (asset == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			contentType = dictionary.GetRichMediaContentType() ?? PdfRichMediaContentType.Flash;
			PdfReaderDictionary paramsDictionary = dictionary.GetDictionary(paramsKey);
			if (paramsDictionary != null) {
				if (contentType != PdfRichMediaContentType.Flash)
					PdfDocumentReader.ThrowIncorrectDataException();
				parameters = new PdfRichMediaParams(paramsDictionary);
			}
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.Add(PdfDictionary.DictionaryTypeKey, new PdfName(mediaInstanceDictionaryName));
			result.Add(assetRefKey, asset);
			if(contentType != PdfRichMediaContentType.Flash)
				result.AddEnumName(PdfDictionary.DictionarySubtypeKey, contentType);
			result.Add(paramsKey, parameters);
			return result;
		}
	}
}

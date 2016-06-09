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
namespace DevExpress.Pdf.Native {
	[PdfDefaultField(PdfSignatureDigestMethod.MD5)]
	public enum PdfSignatureDigestMethod { MD5, SHA1 }
	public enum PdfSignatureTransformMethod { 
		DocMDP, FieldMDP,
		[PdfFieldName("UR", "UR3")]
		UR
	}
	public class PdfSignatureReference: PdfObject {
		const string typeDictionary = "SigRef";
		const string transformMethodDictionaryKey = "TransformMethod";
		const string transformParamsDictionaryKey = "TransformParams";
		const string dataDictionaryKey = "Data";
		const string digestMethodDictionaryKey = "DigestMethod";
		internal static PdfSignatureReference Parse(PdfObjectCollection objects, object value) {
			if (value == null)
				return null;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				return objects.ResolveObject<PdfSignatureReference>(reference.Number, () => Parse(objects, objects.GetObjectData(reference.Number)));
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return new PdfSignatureReference(dictionary);
		}
		readonly PdfSignatureTransformMethod transformMethod;
		readonly PdfPrivateData transformParams;
		readonly PdfSignatureDigestMethod digestMethod;
		readonly byte[] data = null;
		public PdfSignatureTransformMethod TransformMethod { get { return transformMethod; } }
		public PdfPrivateData TransformParams { get { return transformParams; } }
		public PdfSignatureDigestMethod DigestMethod { get { return digestMethod; } }
		public byte[] Data { get { return data; } }
		PdfSignatureReference(PdfReaderDictionary dictionary) : base (dictionary.Number){
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (!String.IsNullOrEmpty(type) && type != typeDictionary)
				PdfDocumentReader.ThrowIncorrectDataException();
			transformMethod = dictionary.GetEnumName<PdfSignatureTransformMethod>(transformMethodDictionaryKey);
			PdfReaderDictionary parametersDictionary = dictionary.GetDictionary(transformParamsDictionaryKey);
			if (parametersDictionary != null)
				transformParams = new PdfPrivateData(null, parametersDictionary);
			if (transformMethod == PdfSignatureTransformMethod.FieldMDP && data == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			digestMethod = dictionary.GetEnumName<PdfSignatureDigestMethod>(digestMethodDictionaryKey);
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return new PdfWriterDictionary(objects);
		}
	}
}

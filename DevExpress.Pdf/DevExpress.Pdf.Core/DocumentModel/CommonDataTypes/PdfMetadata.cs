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
using System.Text;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfMetadata : PdfObject {
		internal const string Name = "Metadata";
		const string subtypeValue = "XML";
		readonly string data;
		public virtual string Data { get { return data; } }
		internal PdfMetadata(string data) {
			this.data = data;
		}
		internal PdfMetadata(PdfReaderStream stream)
			: base(stream.Dictionary.Number) {
			PdfReaderDictionary dictionary = stream.Dictionary;
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			string subtype = dictionary.GetName(PdfDictionary.DictionarySubtypeKey);
			if ((type != null && type != Name) || (subtype != null && subtype != subtypeValue))
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfEncryptionInfo encryptionInfo = dictionary.Objects.EncryptionInfo;
			byte[] streamData = stream.GetData(encryptionInfo != null && encryptionInfo.EncryptMetadata);
			if (streamData == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			data = Encoding.UTF8.GetString(streamData, 0, streamData.Length);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfDictionary dictionary = new PdfDictionary();
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(Name));
			dictionary.Add(PdfDictionary.DictionarySubtypeKey, new PdfName(subtypeValue));
			return new PdfStream(dictionary, Encoding.UTF8.GetBytes(data));
		}
	}
}

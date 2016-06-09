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
namespace DevExpress.Pdf.Native {
	public class PdfReaderStream : PdfStream {
		public const string FilterDictionaryKey = "Filter";
		public const string DecodeParametersDictionaryKey = "DecodeParms";
		readonly PdfReaderDictionary dictionary;
		readonly IList<PdfFilter> filters;
		readonly PdfEncryptionInfo encryptionInfo;
		byte[] data;
		public PdfReaderDictionary Dictionary { get { return dictionary; } }
		public IList<PdfFilter> Filters { get { return filters; } }
		public byte[] DecryptedData {
			get {
				byte[] result = RawData;
				if (result.Length > 0 && encryptionInfo != null)
					result = encryptionInfo.DecryptData(result, dictionary.Number, dictionary.Generation);
				return result;
			}
		}
		public PdfReaderStream(PdfReaderDictionary dictionary, byte[] data, PdfEncryptionInfo encriptionInfo)
			: base(dictionary, data) {
			this.encryptionInfo = encriptionInfo;
			this.dictionary = dictionary;
			filters = dictionary.GetFilters(FilterDictionaryKey, DecodeParametersDictionaryKey) ?? new PdfFilter[0];
		}
		public PdfReaderStream(PdfReaderDictionary dictionary, byte[] data)
			: this(dictionary, data, dictionary.Objects.EncryptionInfo) {
		}
		public byte[] GetData(bool shouldDecrypt) {
			if (data == null) {
				data = shouldDecrypt ? DecryptedData : RawData;
				if (data.Length > 0)
					foreach (PdfFilter filter in filters)
						data = filter.Decode(data);
			}
			return data;
		}
	}
}

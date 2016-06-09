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
using System.IO;
using System.Linq;
using System.Text;
namespace DevExpress.Pdf.Native {
	internal class FdfDocumentReader : PdfDocumentStructureReader {
		internal static void Read(Stream stream, PdfFormData root) {
			BufferedStream bufferedStream = new BufferedStream(stream);
			new FdfDocumentReader(new PdfDocumentStream(bufferedStream)).Read(root);
		}
		FdfDocumentReader(PdfDocumentStream streamReader) : base(streamReader) {
			string header = streamReader.ReadString();
			long length = streamReader.Length;
			long pos = streamReader.Position;
			streamReader.SkipSpaces();
			streamReader.Position = pos;
			long offset = streamReader.FindLastToken(TrailerToken);
			streamReader.Position = pos;
			do {
				pos = streamReader.Position;
				if (streamReader.ReadToken(TrailerToken))
					break;
				Objects.AddItem(streamReader.ReadObject(pos), true);
			} while (streamReader.Position < length);
			streamReader.Position = offset;
			UpdateTrailer(PdfDocumentParser.ParseDictionary(Objects, 0, 0, new PdfArrayData(ReadFdfTrailerData())), Objects);
		}
		void Read(PdfFormData root) {
			Objects.ResolveAllSlots();
			PdfReaderDictionary documentCatalogDictionary = Objects.GetDictionary(RootObjectNumber);
			if (documentCatalogDictionary == null)
				ThrowIncorrectDataException();
			PdfReaderDictionary fdfDictionary = documentCatalogDictionary.GetDictionary("FDF");
			if (fdfDictionary == null)
				ThrowIncorrectDataException();
			IList<object> fields = fdfDictionary.GetArray("Fields");
			if (fields != null) {
				foreach (PdfReaderDictionary item in fields) {
					ParseItem(root, null, item);
				}
			}
		}
		void ParseItem(PdfFormData result, string parentName, PdfReaderDictionary item) {
			string key = (parentName == null ? "" : (parentName + ".")) + item.GetString("T");
			object value;
			if (item.TryGetValue("V", out value)) {
				IList<object> values = value as IList<object>;
				if (values != null) {
					List<string> strings = new List<string>();
					foreach (byte[] str in values)
						strings.Add(PdfDocumentReader.ConvertToString(str));
					result[key].Value = strings;
				}
				else {
					byte[] bytes = value as byte[];
					if (bytes != null) {
						string strValue = PdfDocumentReader.ConvertToString(bytes);
						if (strValue != null)
							result[key].Value = strValue;
						else
							ThrowIncorrectDataException();
					}
					else {
						PdfName name = value as PdfName;
						if (name != null)
							result[key].Value = name.Name;
						else
							ThrowIncorrectDataException();
					}
				}
			}
			else {
				IList<object> kids = item.GetArray("Kids");
				if(kids != null)
					foreach (PdfReaderDictionary kid in kids)
						ParseItem(result, key, kid);
			}
		}
		byte[] ReadFdfTrailerData() {
			List<byte> data = new List<byte>();
			PdfTokenDescription token = PdfTokenDescription.BeginCompare(EofToken);
			PdfDocumentStream streamReader = DocumentStream;
			for (; ; ) {
				int next = streamReader.ReadByte();
				if (next == -1)
					return data.ToArray();
				byte symbol = (byte)next;
				data.Add(symbol);
				if (token.Compare(symbol)) {
					int tokenLength = token.Length;
					data.RemoveRange(data.Count - tokenLength, tokenLength);
					return data.ToArray();
				}
			}
		}
	}
}

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
	public class PdfPieceInfoEntry {
		const string lastModifiedKey = "LastModified";
		const string privateKey = "Private";
		const string pieceInfoKey = "PieceInfo";
		internal static Dictionary<string, PdfPieceInfoEntry> Parse(PdfReaderDictionary dictionary) {
			PdfReaderDictionary pieceInfoDictionary = dictionary.GetDictionary(pieceInfoKey);
			if (pieceInfoDictionary == null)
				return null;
			Dictionary<string, PdfPieceInfoEntry> result = new Dictionary<string, PdfPieceInfoEntry>();
			foreach (KeyValuePair<string, object> pair in pieceInfoDictionary) {
				PdfReaderDictionary subDictionary = pieceInfoDictionary.Objects.TryResolve(pair.Value) as PdfReaderDictionary;
				if (subDictionary != null)
					result.Add(pair.Key, new PdfPieceInfoEntry(subDictionary));
			}
			return result.Count == 0 ? null : result;
		}
		internal static void WritePieceInfo(PdfWriterDictionary dictionary, Dictionary<string, PdfPieceInfoEntry> pieceInfo) {
			dictionary.AddIfPresent(pieceInfoKey, PdfElementsDictionaryWriter.Write(pieceInfo, value => ((PdfPieceInfoEntry)value).Write(dictionary.Objects)));
		}
		readonly PdfDocumentCatalog catalog;
		readonly DateTimeOffset? lastModified;
		object data;
		object dataValue;
		public DateTimeOffset? LastModified { get { return lastModified; } }
		public object Data {
			get {
				if (dataValue != null) {
					data = PdfPrivateData.TryResolve(null, catalog.Objects, dataValue);
					dataValue = null;
				}
				return data;
			}
		}
		internal PdfPieceInfoEntry(PdfReaderDictionary dictionary) {
			lastModified = dictionary.GetDate(lastModifiedKey);
			dictionary.TryGetValue(privateKey, out dataValue);
			catalog = dictionary.Objects.DocumentCatalog;
		}
		internal PdfDictionary Write(PdfObjectCollection objects) {
			PdfWriterDictionary result = new PdfWriterDictionary(objects);
			result.AddNullable(lastModifiedKey, lastModified);
			result.AddIfPresent(privateKey, WriteObject(objects, Data));
			return result;
		}
		object WriteObject(PdfObjectCollection objects, object obj) {
			PdfPrivateData privateData = obj as PdfPrivateData;
			if (privateData != null)
				return objects.AddObject(privateData);
			IEnumerable<object> list = obj as IEnumerable<object>;
			return list == null ? obj : new PdfWritableConvertableArray(list, value => WriteObject(objects, value));
		}
	}
}

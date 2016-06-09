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
	public class PdfPrivateData : PdfObject {
		internal static object TryResolve(PdfPrivateData parent, PdfObjectCollection collection, object value) {
			if (collection != null) {
				PdfObjectReference reference = value as PdfObjectReference;
				if (reference != null) {
					if (parent != null) {
						int objectNumber = reference.Number;
						for (PdfPrivateData data = parent; data != null; data = data.parent)
							if (data.objectNumber == objectNumber)
								return data;
					}
					object resolvedObject = TryResolve(parent, collection, collection.TryResolvePrivateDataObject(value));
					collection.ResolveObject<PdfObject>(reference.Number, () => resolvedObject as PdfObject);
					return resolvedObject;
				}
			}
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary != null)
				return dictionary.Count == 0 ? null : new PdfPrivateData(parent, dictionary);
			PdfReaderStream stream = value as PdfReaderStream;
			if (stream != null)
				return new PdfPrivateData(parent, stream);
			if (value is double)
				return new PdfDouble(value);
			byte[] bytes = value as byte[];
			if (bytes != null)
				return bytes;
			IList<object> list = value as IList<object>;
			if (list == null)
				return value;
			List<object> result = new List<object>(list.Count);
			foreach (object o in list)
				result.Add(TryResolve(parent, collection, o));
			return result;
		}
		readonly PdfPrivateData parent;
		readonly int objectNumber;
		readonly byte[] rawData = null;
		readonly PdfDocumentCatalog catalog;
		readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();
		public byte[] RawData { get { return rawData; } }
		public object this[string key] {
			get {
				return TryResolve(this, catalog.Objects, dictionary[key]);
			}
		}
		internal PdfPrivateData(PdfPrivateData parent, PdfReaderDictionary readerDictionary) : base(readerDictionary.Number) {
			this.parent = parent;
			objectNumber = readerDictionary.Number;
			catalog = readerDictionary.Objects.DocumentCatalog;
			dictionary = readerDictionary;
		}
		internal PdfPrivateData(PdfPrivateData parent, PdfReaderStream readerStream) : this(parent, readerStream.Dictionary) {
			rawData = readerStream.RawData;
		}
		internal PdfDictionary CreateWriterDictionary(PdfObjectCollection collection) {
			return PdfElementsDictionaryWriter.Write(dictionary, value => ToWritableObject(collection, TryResolve(this, catalog.Objects, value)));
		}
		internal void Remove(string key) {
			dictionary.Remove(key);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfDictionary dictionary = CreateWriterDictionary(collection);
			return rawData == null ? (object)dictionary : (object)new PdfStream(dictionary, rawData);
		}
		object ToWritableObject(PdfObjectCollection collection, object obj) {
			PdfPrivateData privateData = obj as PdfPrivateData;
			if (privateData != null)
				return collection.AddObject(privateData);
			IList<object> list = obj as IList<object>;
			return list == null ? obj : new PdfWritableConvertableArray(list, value => ToWritableObject(collection, TryResolve(this, catalog.Objects, value)));
		}
	}
}

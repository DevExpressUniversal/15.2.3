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
	public abstract class PdfSpiderSet : PdfObject {
		const string dictionaryName = "SpiderContentSet";
		const string idKey = "ID";
		const string contentTypeKey = "CT";
		const string timeStampKey = "TS";
		const string sourceInformationKey = "SI";
		const string pagesKey = "O";
		internal static PdfSpiderSet Create(PdfObjectCollection objects, object value) {
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				return objects.ResolveObject<PdfSpiderSet>(reference.Number, () => Create(objects, objects.GetObjectData(reference.Number)));
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			string subtype = dictionary.GetName("S");
			if (subtype == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			switch (subtype) {
				case "SPS":
					return new PdfPageSet(dictionary);
				case "SIS":
					return new PdfImageSet(dictionary);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					break;
			}
			return null;
		}
		readonly byte[] id;
		readonly List<PdfPage> pageSet = new List<PdfPage>();
		readonly List<PdfSourceInformation> sourceInformation = new List<PdfSourceInformation>();
		readonly string contentType;
		readonly DateTimeOffset? timeStamp;
		public byte[] ID { get { return id; } }
		public IEnumerable<PdfPage> PageSet { get { return pageSet; } }
		public IEnumerable<PdfSourceInformation> SourceInformation { get { return sourceInformation; } }
		public string ContentType { get { return contentType; } }
		public DateTimeOffset? TimeStamp { get { return timeStamp; } }
		protected abstract string SubType { get; }
		protected PdfSpiderSet(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			id = dictionary.GetBytes(idKey);
			IList<object> o = dictionary.GetArray(pagesKey);
			object si = null;
			if ((type != null && type != dictionaryName) || id == null || o == null || !dictionary.TryGetValue(sourceInformationKey, out si))
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfObjectCollection objects = dictionary.Objects;
			PdfDocumentCatalog documentCatalog = objects.DocumentCatalog;
			int prevPageIndex = -1;
			IList<PdfPage> pages = documentCatalog.Pages;
			foreach (object obj in o) {
				PdfPage page = documentCatalog.FindPage(obj);
				if (page != null) {
					int index = pages.IndexOf(page);
					if (index <= prevPageIndex)
						PdfDocumentReader.ThrowIncorrectDataException();
					prevPageIndex = index;
					pageSet.Add(page);
				}
			}
			PdfObjectReference reference = si as PdfObjectReference;
			if (reference == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			object value = objects.GetObjectData(reference.Number);
			PdfReaderDictionary siDictionary = value as PdfReaderDictionary;
			if (siDictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			sourceInformation.Add(new PdfSourceInformation(siDictionary));
			contentType = dictionary.GetString(contentTypeKey);
			timeStamp = dictionary.GetDate(timeStampKey);
		}
		protected virtual PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.Add(PdfDictionary.DictionaryTypeKey, new PdfName(dictionaryName));
			result.Add("S", new PdfName(SubType));
			result.Add(idKey, id, null);
			result.Add(contentTypeKey, contentType, null);
			result.Add(timeStampKey, timeStamp, null);
			if (sourceInformation.Count > 0) {
				object siWritten;
				if (sourceInformation.Count == 1)
					siWritten = collection.AddObject(sourceInformation[0]);
				else {
					siWritten = new PdfWritableObjectArray(sourceInformation, collection);
				}
				result.Add(sourceInformationKey, siWritten);
			}
			List<object> pages = new List<object>();
			foreach (PdfPage page in pageSet)
				pages.Add(collection.AddObject(page));
			result.Add(pagesKey, pages);
			return result;
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			return CreateDictionary(collection);
		}
	}
}

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
	public abstract class PdfDestination : PdfObject, IPdfDeferredSavedObject {
		static object GetSingleValue(IList<object> array) {
			switch (array.Count) {
				case 2:
					return null;
				case 3:
					return array[2];
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		protected static void AddParameter(IList<object> parameters, double? parameter) {
			parameters.Add(parameter.HasValue ? (object)parameter.Value : null);
		}
		internal static PdfDestination Parse(PdfDocumentCatalog documentCatalog, object value) {
			IList<object> array = value as IList<object>;
			if (array == null) {
				PdfReaderDictionary dictionary = value as PdfReaderDictionary;
				if (dictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				array = dictionary.GetArray("D");
				if (array == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			int arrayLength = array.Count;
			if (arrayLength < 1)
				PdfDocumentReader.ThrowIncorrectDataException();
			object pageObject = array[0];
			PdfName name;
			if (arrayLength == 1)
				name = new PdfName(PdfXYZDestination.Name);
			else {
				name = array[1] as PdfName;
				if (name == null)
					name = new PdfName(PdfXYZDestination.Name);
			}
			switch (name.Name) {
				case PdfXYZDestination.Name: {
						object left;
						object top;
						object zoom = null;
						switch (arrayLength) {
							case 1:
							case 2:
								left = null;
								top = null;
								break;
							case 3:
								left = null;
								top = array[2];
								break;
							case 4:
								left = array[2];
								top = array[3];
								break;
							case 5:
								zoom = array[4];
								goto case 4;
							default:
								PdfDocumentReader.ThrowIncorrectDataException();
								return null;
						}
						return new PdfXYZDestination(documentCatalog, pageObject, left == null ? (double?)null : PdfDocumentReader.ConvertToDouble(left),
							top == null ? (double?)null : PdfDocumentReader.ConvertToDouble(top), zoom == null ? (double?)null : PdfDocumentReader.ConvertToDouble(zoom));
					}
				case PdfFitDestination.Name:
					if (arrayLength != 2)
						PdfDocumentReader.ThrowIncorrectDataException();
					return new PdfFitDestination(documentCatalog, pageObject);
				case PdfFitHorizontallyDestination.Name: {
						object top = GetSingleValue(array);
						return new PdfFitHorizontallyDestination(documentCatalog, pageObject, top == null ? (double?)null : PdfDocumentReader.ConvertToDouble(top));
					}
				case PdfFitVerticallyDestination.Name: {
						object left = GetSingleValue(array);
						return new PdfFitVerticallyDestination(documentCatalog, pageObject, left == null ? (double?)null : PdfDocumentReader.ConvertToDouble(left));
					}
				case PdfFitRectangleDestination.Name:
					if (arrayLength != 6)
						PdfDocumentReader.ThrowIncorrectDataException();
					return new PdfFitRectangleDestination(documentCatalog, pageObject, new PdfRectangle(new object[] { array[2], array[3], array[4], array[5] }));
				case PdfFitBBoxDestination.Name:
					if (arrayLength != 2)
						PdfDocumentReader.ThrowIncorrectDataException();
					return new PdfFitBBoxDestination(documentCatalog, pageObject);
				case PdfFitBBoxHorizontallyDestination.Name: {
						object top = GetSingleValue(array);
						return new PdfFitBBoxHorizontallyDestination(documentCatalog, pageObject, top == null ? (double?)null : PdfDocumentReader.ConvertToDouble(top));
					}
				case PdfFitBBoxVerticallyDestination.Name: {
						object left = GetSingleValue(array);
						return new PdfFitBBoxVerticallyDestination(documentCatalog, pageObject, left == null ? (double?)null : PdfDocumentReader.ConvertToDouble(left));
					}
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		internal static PdfDeferredSortedDictionary<string, PdfDestination> Parse(PdfReaderDictionary dictionary) {
			PdfObjectCollection objects = dictionary.Objects;
			try {
				object value;
				if (dictionary.Count == 1 && dictionary.TryGetValue(PdfDocumentCatalog.NamesDictionaryKey, out value)) {
					IList<object> namesList = value as IList<object>;
					if (namesList != null) {
						int count = namesList.Count;
						if (count > 0 && count % 2 == 0) {
							PdfDeferredSortedDictionary<string, PdfDestination> list = new PdfDeferredSortedDictionary<string, PdfDestination>();
							for (int i = 0; i < count; ) {
								byte[] name = namesList[i++] as byte[];
								if (name == null)
									PdfDocumentReader.ThrowIncorrectDataException();
								list.Add(PdfDocumentReader.ConvertToString(name), objects.GetDestination(namesList[i++]));
							}
							return list;
						}
					}
				}
			}
			catch {
			}
			PdfDeferredSortedDictionary<string, PdfDestination> result = new PdfDeferredSortedDictionary<string, PdfDestination>();
			foreach (KeyValuePair<string, object> pair in dictionary)
				result.AddDeferred(pair.Key, pair.Value, objects.GetDestination);
			return result;
		}
		internal static PdfDestination Create(PdfPage page, float x, float y, float dpiX, float dpiY, float? zoomFactor) {
			return new PdfXYZDestination(page, x / dpiX * 72, page.CropBox.Height - y / dpiY * 72, zoomFactor);
		}
		readonly PdfDocumentCatalog documentCatalog;
		readonly Tuple<int, Guid> pageId;
		object pageObject;
		PdfPage page;
		int pageIndex = -1;
		public PdfPage Page {
			get {
				ResolvePage();
				return page;
			}
		}
		public int PageIndex {
			get {
				ResolvePage();
				return pageIndex;
			}
		}
		Guid IPdfDeferredSavedObject.CollectionId { get { return pageId == null ? Guid.Empty : pageId.Item2; } }
		protected PdfDestination(PdfPage page) {
			if (page == null)
				throw new ArgumentNullException("page");
			this.page = page;
			this.documentCatalog = page.DocumentCatalog;
			this.pageIndex = CalculatePageIndex(documentCatalog.Pages);
		}
		protected PdfDestination(PdfDocumentCatalog documentCatalog, object pageObject) {
			this.documentCatalog = documentCatalog;
			this.pageObject = pageObject;
		}
		protected PdfDestination(PdfDestination destination) : base(destination.ObjectNumber) {
			if (destination.pageId == null) {
				PdfPage page = destination.page;
				if (page != null) {
					PdfDocumentCatalog catalog = destination.documentCatalog;
					if (catalog != null) {
						PdfObjectCollection objects = catalog.Objects;
						int number = page.ObjectNumber;
						if (number < 1)
							number = ++objects.LastObjectNumber;
						pageId = new Tuple<int, Guid>(number, objects.Id);
					}
				}
			}
			else
				pageId = destination.pageId;
			pageIndex = destination.pageIndex;
		}
		protected int CalculatePageIndex(IList<PdfPage> pages) {
			return Page == null ? pageIndex : pages.IndexOf(page);
		}
		protected double? ValidateVerticalCoordinate(double? top) {
			if (top.HasValue && page != null)
				top = PdfMathUtils.Min(top.Value, page.CropBox.Height);
			return top;
		}
		void ResolvePage() {
			if (pageObject != null) {
				PdfObjectReference reference = pageObject as PdfObjectReference;
				if (reference != null)
					page = documentCatalog.Objects.GetPage(reference.Number);
				else if (pageObject is int)
					pageIndex = (int)pageObject;
				pageObject = null;
			}
		}
		internal void ResolveInternalPage() {
			ResolvePage();
			if (page == null && documentCatalog != null && pageIndex >= 0 && pageIndex < documentCatalog.Pages.Count) {
				page = documentCatalog.Pages[pageIndex];
				pageIndex = -1;
			}
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			List<object> parameters = new List<object>();
			ResolvePage();
			if (pageId != null) {
				PdfObjectReference pageReference = collection.GetSavedObjectReference(pageId.Item1, pageId.Item2);
				if (pageReference == null)
					return null;
				parameters.Add(pageReference);
			}
			else if (page == null)
				parameters.Add(pageIndex == -1 ? null : (object)pageIndex);
			else
				parameters.Add(collection.AddObject(page));
			AddWriteableParameters(parameters);
			return new PdfWritableArray(parameters);
		}
		PdfObject IPdfDeferredSavedObject.CreateDuplicate() {
			ResolvePage();
			return CreateDuplicate();
		}
		protected internal abstract PdfTarget CreateTarget(IList<PdfPage> pages);
		protected abstract void AddWriteableParameters(IList<object> parameters);
		protected internal abstract PdfDestination CreateDuplicate();
	}
}

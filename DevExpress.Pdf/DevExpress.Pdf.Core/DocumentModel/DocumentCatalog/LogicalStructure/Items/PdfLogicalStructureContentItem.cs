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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfLogicalStructureContentItem : PdfLogicalStructureItem {
		internal const string Type = "OBJR";
		const string contentPageDictionaryKey = "Pg";
		const string contentObjectDictionaryKey = "Obj";
		readonly PdfPage page;
		readonly PdfObject content;
		public PdfPage Page { get { return page; } }
		public object Content { get { return content; } }
		protected internal override PdfPage ContainingPage { get { return page; } }
		internal PdfLogicalStructureContentItem(PdfPage elementPage, PdfReaderDictionary dictionary) : base(dictionary.Number) {
			PdfObjectCollection objects = dictionary.Objects;
			PdfDocumentCatalog documentCatalog = objects.DocumentCatalog;
			PdfObjectReference pageReference = dictionary.GetObjectReference(contentPageDictionaryKey);
			if (pageReference != null) {
				page = documentCatalog.FindPage(pageReference);
				page.EnsureAnnotations();
				if (page == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			PdfObjectReference contentReference = dictionary.GetObjectReference(contentObjectDictionaryKey);
			if (contentReference != null) {
				content = objects.GetResolvedObject<PdfAnnotation>(contentReference.Number, false) ?? objects.GetResolvedObject<PdfObject>(contentReference.Number, false);
				if (content == null) {
					object item = objects.TryResolve(contentReference);
					if (item != null) {
						PdfReaderDictionary contentDictionary = item as PdfReaderDictionary;
						if (contentDictionary != null && contentDictionary.GetName(PdfDictionary.DictionaryTypeKey) == PdfAnnotation.DictionaryType)
							content = objects.GetAnnotation(page ?? elementPage, contentReference);
						else
							PdfDocumentReader.ThrowIncorrectDataException();
					}
				}
			}
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.AddName(PdfDictionary.DictionaryTypeKey, Type);
			dictionary.Add(contentPageDictionaryKey, page);
			dictionary.Add(contentObjectDictionaryKey, content);
			return dictionary;
		}
	}
}

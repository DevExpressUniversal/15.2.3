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
	public abstract class PdfLogicalStructureEntry : PdfLogicalStructureItem {
		const string kidsDictionaryKey = "K";
		readonly PdfLogicalStructure logicalStructure;
		readonly PdfDocumentCatalog documentCatalog;
		IList<PdfLogicalStructureItem> kids;
		object kidsValue;
		public IList<PdfLogicalStructureItem> Kids {
			get {
				Resolve();
				return kids;
			}
		}
		protected PdfLogicalStructure LogicalStructure { get { return logicalStructure; } }
		internal PdfDocumentCatalog DocumentCatalog { get { return documentCatalog; } }
		protected PdfLogicalStructureEntry(PdfLogicalStructure logicalStructure, PdfReaderDictionary dictionary) : base(dictionary.Number) {
			this.documentCatalog = dictionary.Objects.DocumentCatalog;
			this.logicalStructure = logicalStructure ?? (PdfLogicalStructure)this;
			dictionary.TryGetValue(kidsDictionaryKey, out kidsValue);
		}
		protected void WriteKids(PdfWriterDictionary dictionary, PdfObjectCollection collection) {
			if (kids != null)
				dictionary.Add(kidsDictionaryKey, kids.Count == 1 ? kids[0].Write(collection) : new PdfWritableConvertableArray<PdfLogicalStructureItem>(kids, value => value.Write(collection)));
		}
		protected internal override void Resolve() {
			if (kidsValue != null) {
				PdfObjectCollection objects = documentCatalog.Objects;
				kids = new List<PdfLogicalStructureItem>();
				object obj = objects.TryResolve(kidsValue);
				PdfReaderDictionary dictionary = obj as PdfReaderDictionary;
				if (dictionary == null) {
					IList<object> valueList = obj as IList<object>;
					if (valueList == null) {
						PdfLogicalStructureItem item = objects.GetLogicalStructureItem(logicalStructure, this, obj);
						if (item != null)
							kids.Add(item);
					}
					else
						foreach (object listObject in valueList) {
							PdfLogicalStructureItem item = objects.GetLogicalStructureItem(logicalStructure, this, listObject);
							if (item != null)
								kids.Add(item);
						}
				}
				else {
					PdfLogicalStructureItem item = objects.GetLogicalStructureItem(logicalStructure, this, kidsValue);
					if (item != null)
						kids.Add(item);
				}
				if (kids.Count == 0)
					kids = null;
				else 
					foreach (PdfLogicalStructureItem kid in kids)
						kid.Resolve();
				kidsValue = null;
			}
		}
	}
}

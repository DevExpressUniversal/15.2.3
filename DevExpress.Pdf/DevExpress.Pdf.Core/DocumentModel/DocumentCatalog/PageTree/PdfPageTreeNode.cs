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

using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfPageTreeNode : PdfPageTreeObject, IEnumerable<PdfPage> {
		class PdfPageTreeObjectList : PdfDeferredList<PdfPageTreeObject> {
			readonly PdfObjectCollection objects;
			readonly PdfPageTreeNode parent;
			public PdfPageTreeObjectList(IList<object> kids, PdfObjectCollection objects, PdfPageTreeNode parent) : base(kids.GetEnumerator(), kids.Count) {
				this.objects = objects;
				this.parent = parent;
			}
			protected override PdfPageTreeObject ParseObject(object value) {
				PdfObjectReference kidReference = value as PdfObjectReference;
				if (kidReference == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				PdfPageTreeObject result = objects.ResolveObject<PdfPageTreeObject>(kidReference.Number, () => CreatePage(kidReference.Number));
				return result;
			}
			PdfPageTreeObject CreatePage(int number) {
				PdfReaderDictionary kidDictionary = objects.GetDictionary(number);
				if (kidDictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				switch (kidDictionary.GetName(PdfDictionary.DictionaryTypeKey)) {
					case PdfPageTreeNode.pageTreeNodeType:
						return new PdfPageTreeNode(parent, kidDictionary);
					case PdfPage.PageTreeNodeType:
						return new PdfPage(parent, kidDictionary);
					default:
						PdfDocumentReader.ThrowIncorrectDataException();
						return null;
				}
			}
		}
		internal const string pageTreeNodeType = "Pages";
		const string kidsDictionaryKey = "Kids";
		const string countDictionaryKey = "Count";
		IList<PdfPageTreeObject> kids;
		readonly int count;
		public int Count { get { return count; } }
		internal PdfPageTreeNode(PdfDocumentCatalog documentCatalog, PdfRectangle mediaBox, PdfRectangle cropBox, int rotate, IEnumerable<PdfPage> pages)
			: base(documentCatalog, null, mediaBox, cropBox, rotate) {
			this.kids = new List<PdfPageTreeObject>(pages);
			foreach (PdfPage page in pages)
				page.Parent = this;
			count = kids.Count;
		}
		internal PdfPageTreeNode(PdfPageTreeNode parent, PdfReaderDictionary dictionary)
			: base(parent, dictionary) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			IList<object> kidsArray = dictionary.GetArray(kidsDictionaryKey);
			int? count = dictionary.GetInteger(countDictionaryKey);
			if ((type != null && type != pageTreeNodeType) || kidsArray == null || !count.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			this.count = count.Value;
			PdfObjectCollection objects = dictionary.Objects;
			kids = new PdfPageTreeObjectList(kidsArray, objects, this);
		}
		internal void RemovePage(PdfPage page) {
			kids.Remove(page);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.AddName(PdfDictionary.DictionaryTypeKey, pageTreeNodeType);
			dictionary.Add(countDictionaryKey, kids.Count);
			dictionary.Add(kidsDictionaryKey, new PdfWritableObjectArray(kids, objects));
			return dictionary;
		}
		IEnumerator<PdfPage> IEnumerable<PdfPage>.GetEnumerator() {
			foreach (PdfPageTreeObject obj in kids) {
				PdfPage page = obj as PdfPage;
				if (page == null) {
					PdfPageTreeNode treeNode = obj as PdfPageTreeNode;
					if (treeNode == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					foreach (PdfPage childPage in treeNode)
						yield return childPage;
				}
				else
					yield return page;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<PdfPage>)this).GetEnumerator();
		}
	}
}

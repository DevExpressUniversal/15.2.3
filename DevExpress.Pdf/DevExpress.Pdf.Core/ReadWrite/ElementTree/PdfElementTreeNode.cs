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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public delegate K PdfCreateTreeKeyAction<K>(object value);
	public delegate T PdfCreateTreeElementAction<T>(PdfObjectCollection objects, object value);
	public delegate object PdfConvertToKeyTreeElementAction<T>(T value);
	public abstract class PdfElementTreeNode<K, T> : IEnumerable<KeyValuePair<K, T>> where T : class {
		internal static PdfDeferredSortedDictionary<K, T> Parse(PdfReaderDictionary dictionary, PdfCreateTreeKeyAction<K> createKey, PdfCreateTreeElementAction<T> createElement, string nodeName, bool checkElementCount) {
			if (dictionary == null || dictionary.Count == 0)
				return null;
			PdfObjectCollection objects = dictionary.Objects;
			IList<object> elements = null;
			object elementsValue;
			if (dictionary.TryGetValue(nodeName, out elementsValue)) {
				elementsValue = objects.TryResolve(elementsValue);
				if (elementsValue != null) {
					elements = elementsValue as IList<object>;
					if (elements == null)
						return null;
				}
			}
			IList<object> kids = dictionary.GetArray("Kids");
			if (elements == null) {
				if (kids == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return new PdfElementTreeBranch<K, T>(objects, kids, createKey, createElement, nodeName).Value;
			}
			else {
				if (kids != null)
					PdfDocumentReader.ThrowIncorrectDataException();
				if (!checkElementCount && elements.Count == 1)
					return null;
				return new PdfElementTreeLeaf<K, T>(objects, elements, createKey, createElement).Value;
			}
		}
		internal static PdfWriterDictionary Write(PdfObjectCollection objects, string key, PdfDeferredSortedDictionary<K, T> dictionary, PdfConvertToKeyTreeElementAction<K> convertToKeyAction, Func<PdfObjectCollection, T, object> writeAction) {
			if (dictionary == null)
				return null;
			List<object> namesArray = new List<object>();
			foreach (KeyValuePair<K, PdfDeferredItem<T>> pair in ((IEnumerable<KeyValuePair<K, PdfDeferredItem<T>>>)dictionary)) {
				namesArray.Add(convertToKeyAction(pair.Key));
				if (writeAction == null) {
					PdfObjectReference reference = pair.Value.Value as PdfObjectReference;
					reference = objects.AddObject(reference == null ? PdfObject.DirectObjectNumber : reference.Number, () => pair.Value.Item as PdfObject);
					namesArray.Add(reference);
				}
				else
					namesArray.Add(writeAction(objects, pair.Value.Item));
			}
			PdfWriterDictionary result = new PdfWriterDictionary(objects);
			result.Add(key, new PdfWritableArray(namesArray));
			return result;
		}
		protected abstract PdfDeferredSortedDictionary<K, T> Value { get; }
		protected PdfElementTreeNode() {
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerator<KeyValuePair<K, T>> IEnumerable<KeyValuePair<K, T>>.GetEnumerator() {
			return GetEnumerator();
		}
		internal abstract IEnumerator<KeyValuePair<K, T>> GetEnumerator();
	}
}

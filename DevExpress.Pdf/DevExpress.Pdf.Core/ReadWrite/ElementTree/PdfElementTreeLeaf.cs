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
	public class PdfElementTreeLeaf<K, T> : PdfElementTreeNode<K, T> where T : class {
		readonly PdfDeferredSortedDictionary<K, T> values = new PdfDeferredSortedDictionary<K, T>();
		protected override PdfDeferredSortedDictionary<K, T> Value { get { return values; } }
		internal PdfElementTreeLeaf(PdfObjectCollection objects, IList<object> elements, PdfCreateTreeKeyAction<K> createKey, PdfCreateTreeElementAction<T> createElement) {
			int count = elements.Count / 2;
			for (int i = 0, index = 0; i < count; i++) {
				object value = elements[index++];
				PdfObjectReference reference = value as PdfObjectReference;
				if (reference != null)
					value = objects.GetObjectData(reference.Number);
				K key = createKey(value);
				values.AddDeferred(key, elements[index++], v => createElement(objects, v));
			}
		}
		internal override IEnumerator<KeyValuePair<K, T>> GetEnumerator() {
			return values.GetEnumerator();
		}
	}
}

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
	public class PdfElementTreeBranch<K, T> : PdfElementTreeNode<K, T> where T : class {
		readonly PdfDeferredSortedDictionary<K, T> kids = new PdfDeferredSortedDictionary<K, T>();
		protected override PdfDeferredSortedDictionary<K, T> Value { get { return kids; } }
		internal PdfElementTreeBranch(PdfObjectCollection objects, IList<object> children, PdfCreateTreeKeyAction<K> createKey, PdfCreateTreeElementAction<T> createElement, string nodeName) {
			foreach (object child in children) {
				PdfReaderDictionary dictionary = child as PdfReaderDictionary;
				if (dictionary == null) {
					PdfObjectReference reference = child as PdfObjectReference;
					if (reference == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					dictionary = objects.GetDictionary(reference.Number);
					if (dictionary == null)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				kids.AddRange(PdfElementTreeNode<K, T>.Parse(dictionary, createKey, createElement, nodeName, true));
			}
		}
		internal override IEnumerator<KeyValuePair<K, T>> GetEnumerator() {
			foreach (KeyValuePair<K, T> pair in (IEnumerable<KeyValuePair<K, T>>)kids)
				yield return pair;
		}
	}
}

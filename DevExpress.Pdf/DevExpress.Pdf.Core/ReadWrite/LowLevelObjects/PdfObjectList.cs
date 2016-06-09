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
	public class PdfObjectList<T> : PdfObject, IList<T> where T : PdfObject {
		readonly List<T> objectList;
		public PdfObjectList(PdfObjectCollection objects) {
			objectList = new List<T>();
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return new PdfWritableObjectArray(objectList, objects);
		}
		public T this[int index] {
			get { return objectList[index]; }
			set { objectList[index] = value; }
		}
		public int Count { get { return objectList.Count; } }
		public void Add(T item) {
			objectList.Add(item);
		}
		int IList<T>.IndexOf(T item) {
			return objectList.IndexOf(item);
		}
		void IList<T>.Insert(int index, T item) {
			objectList.Insert(index, item);
		}
		void IList<T>.RemoveAt(int index) {
			objectList.RemoveAt(index);
		}
		bool ICollection<T>.IsReadOnly {
			get { return ((IList<T>)objectList).IsReadOnly; }
		}
		void ICollection<T>.Clear() {
			objectList.Clear();
		}
		bool ICollection<T>.Contains(T item) {
			return objectList.Contains(item);
		}
		void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
			objectList.CopyTo(array, arrayIndex);
		}
		bool ICollection<T>.Remove(T item) {
			return objectList.Remove(item);
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return objectList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)objectList).GetEnumerator();
		}
	}
}

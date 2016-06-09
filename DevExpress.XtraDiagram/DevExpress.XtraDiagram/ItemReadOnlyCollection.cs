#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
namespace DevExpress.XtraDiagram {
	public class DiagramItemReadOnlyCollection : ReadOnlyCollectionBase, IList<IDiagramItem> {
		public DiagramItemReadOnlyCollection() {
		}
		void ICollection<IDiagramItem>.Add(IDiagramItem item) {
			throw new NotSupportedException();
		}
		void ICollection<IDiagramItem>.Clear() {
			throw new NotSupportedException();
		}
		bool ICollection<IDiagramItem>.Contains(IDiagramItem item) {
			return InnerList.Contains(item);
		}
		void ICollection<IDiagramItem>.CopyTo(IDiagramItem[] array, int arrayIndex) {
			InnerList.CopyTo(array, arrayIndex);
		}
		bool ICollection<IDiagramItem>.IsReadOnly { get { return true; } }
		int IList<IDiagramItem>.IndexOf(IDiagramItem item) {
			return InnerList.IndexOf(item);
		}
		void IList<IDiagramItem>.Insert(int index, IDiagramItem item) {
			throw new NotSupportedException();
		}
		bool ICollection<IDiagramItem>.Remove(IDiagramItem item) {
			throw new NotSupportedException();
		}
		void IList<IDiagramItem>.RemoveAt(int index) {
			throw new NotSupportedException();
		}
		int ICollection<IDiagramItem>.Count { get { return InnerList.Count; } }
		IDiagramItem IList<IDiagramItem>.this[int index] {
			get { return (IDiagramItem)InnerList[index]; }
			set { InnerList[index] = value; }
		}
		IEnumerator<IDiagramItem> IEnumerable<IDiagramItem>.GetEnumerator() {
			foreach(IDiagramItem item in InnerList) yield return item;
		}
	}
}

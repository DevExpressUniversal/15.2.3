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
using System.Linq;
namespace DevExpress.Xpf.Editors.Helpers {
	public class DefaultDataViewAsListWrapper : IList {
		readonly DefaultDataView view;
		public DefaultDataViewAsListWrapper(DefaultDataView view) {
			this.view = view;
		}
		public object this[int index] {
			get { return view.GetItemByIndex(index); }
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return this.view.Select(item => item.f_component).GetEnumerator();
		}
		void ICollection.CopyTo(Array array, int index) {
			throw new NotImplementedException();
		}
		int ICollection.Count {
			get { return view.VisibleRowCount; }
		}
		object ICollection.SyncRoot {
			get { return this; }
		}
		bool ICollection.IsSynchronized {
			get { return true; }
		}
		int IList.Add(object value) {
			throw new NotImplementedException();
		}
		bool IList.Contains(object value) {
			throw new NotImplementedException();
		}
		void IList.Clear() {
			throw new NotImplementedException();
		}
		int IList.IndexOf(object value) {
			throw new NotImplementedException();
		}
		void IList.Insert(int index, object value) {
			throw new NotImplementedException();
		}
		void IList.Remove(object value) {
			throw new NotImplementedException();
		}
		void IList.RemoveAt(int index) {
			throw new NotImplementedException();
		}
		object IList.this[int index] {
			get { return view[index].f_component; }
			set { throw new NotImplementedException(); }
		}
		bool IList.IsReadOnly {
			get { return true; }
		}
		bool IList.IsFixedSize {
			get { return true; }
		}
	}
}

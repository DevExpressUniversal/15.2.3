#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using System.Collections;
namespace DevExpress.Persistent.Base {
	public class ListConverter<OfType, FromType> : IList<OfType>, IList where FromType : OfType {
		private IList<FromType> list;
		public ListConverter(IList<FromType> list) {
			this.list = list;
		}
		#region IList<OfType> Members
		public int IndexOf(OfType item) {
			return list.IndexOf((FromType)item);
		}
		public void Insert(int index, OfType item) {
			list.Insert(index, (FromType)item);
		}
		public void RemoveAt(int index) {
			list.RemoveAt(index);
		}
		public OfType this[int index] {
			get {
				return (OfType)list[index];
			}
			set {
				list[index] = (FromType)value;
			}
		}
		#endregion
		#region ICollection<OfType> Members
		public void Add(OfType item) {
			list.Add((FromType)item);
		}
		public void Clear() {
			list.Clear();
		}
		public bool Contains(OfType item) {
			return list.Contains((FromType)item);
		}
		public void CopyTo(OfType[] array, int arrayIndex) {
			FromType[] temp = new FromType[array.Length];
			array.CopyTo(temp, arrayIndex);
			list.CopyTo(temp, arrayIndex);
		}
		public bool Remove(OfType item) {
			return list.Remove((FromType)item);
		}
		public int Count {
			get { return list.Count; }
		}
		public bool IsReadOnly {
			get { return list.IsReadOnly; }
		}
		#endregion
		#region IEnumerable<OfType> Members
		public IEnumerator<OfType> GetEnumerator() {
			return new DevExpress.ExpressApp.Utils.EnumeratorConverter<OfType, FromType>(list.GetEnumerator());
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((System.Collections.IEnumerable)list).GetEnumerator();
		}
		#endregion
		#region IList Members
		private IList ToIList() {
			if(!(list is IList)) {
				throw new Exception("Source list does not implement IList");
			}
			return list as IList;
		}
		public int Add(object value) {
			return ToIList().Add(value);
		}
		public bool Contains(object value) {
			return ToIList().Contains(value);
		}
		public int IndexOf(object value) {
			return ToIList().IndexOf(value);
		}
		public void Insert(int index, object value) {
			ToIList().Insert(index, value);
		}
		public void Remove(object value) {
			ToIList().Remove(value);
		}
		public bool IsFixedSize {
			get { return ToIList().IsFixedSize; }
		}
		object IList.this[int index] {
			get {
				return ToIList()[index];
			}
			set {
				ToIList()[index] = value;
			}
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			ToIList().CopyTo(array, index);
		}
		public bool IsSynchronized {
			get { return ToIList().IsSynchronized; }
		}
		public object SyncRoot {
			get { return ToIList().SyncRoot; }
		}
		#endregion
	}
}

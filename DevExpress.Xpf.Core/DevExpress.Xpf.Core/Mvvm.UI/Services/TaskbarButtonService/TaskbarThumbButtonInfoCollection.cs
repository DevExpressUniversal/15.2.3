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
using System.Windows.Shell;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils;
using DevExpress.Mvvm.Native;
namespace DevExpress.Mvvm.UI {
	public class TaskbarThumbButtonInfoCollection : IList<TaskbarThumbButtonInfo>, IList {
		public TaskbarThumbButtonInfoCollection() : this(new ThumbButtonInfoCollection()) { }
		public TaskbarThumbButtonInfoCollection(ThumbButtonInfoCollection collection) {
			GuardHelper.ArgumentNotNull(collection, "collection");
			this.InternalCollection = collection;
		}
		internal ThumbButtonInfoCollection InternalCollection { get; private set; }
		public int IndexOf(TaskbarThumbButtonInfo item) {
			return InternalCollection.IndexOf(TaskbarThumbButtonInfoWrapper.Wrap(item));
		}
		public void Insert(int index, TaskbarThumbButtonInfo item) {
			InternalCollection.Insert(index, TaskbarThumbButtonInfoWrapper.Wrap(item));
		}
		public void RemoveAt(int index) {
			InternalCollection.RemoveAt(index);
		}
		public TaskbarThumbButtonInfo this[int index] {
			get { return TaskbarThumbButtonInfoWrapper.UnWrap(InternalCollection[index]); }
			set { InternalCollection[index] = TaskbarThumbButtonInfoWrapper.Wrap(value); }
		}
		public void Add(TaskbarThumbButtonInfo item) {
			InternalCollection.Add(TaskbarThumbButtonInfoWrapper.Wrap(item));
		}
		public void Clear() {
			InternalCollection.Clear();
		}
		public bool Contains(TaskbarThumbButtonInfo item) {
			return InternalCollection.Contains(TaskbarThumbButtonInfoWrapper.Wrap(item));
		}
		public void CopyTo(TaskbarThumbButtonInfo[] array, int arrayIndex) {
			foreach(TaskbarThumbButtonInfo item in this) {
				array[arrayIndex] = item;
				++arrayIndex;
			}
		}
		public int Count { get { return InternalCollection.Count; } }
		public bool IsReadOnly { get { return false; } }
		public bool Remove(TaskbarThumbButtonInfo item) {
			return InternalCollection.Remove(TaskbarThumbButtonInfoWrapper.Wrap(item));
		}
		public IEnumerator<TaskbarThumbButtonInfo> GetEnumerator() {
			foreach(ThumbButtonInfo item in InternalCollection) {
				yield return TaskbarThumbButtonInfoWrapper.UnWrap(item);
			}
		}
		#region IList
		object syncRoot = new object();
		int IList.Add(object value) {
			Add((TaskbarThumbButtonInfo)value);
			return Count - 1;
		}
		bool IList.Contains(object value) {
			return Contains((TaskbarThumbButtonInfo)value);
		}
		int IList.IndexOf(object value) {
			return IndexOf((TaskbarThumbButtonInfo)value);
		}
		void IList.Insert(int index, object value) {
			Insert(index, (TaskbarThumbButtonInfo)value);
		}
		bool IList.IsFixedSize {
			get { return false; }
		}
		void IList.Remove(object value) {
			Remove((TaskbarThumbButtonInfo)value);
		}
		object IList.this[int index] {
			get { return this[index];}
			set { this[index] = (TaskbarThumbButtonInfo)value;}
		}
		void ICollection.CopyTo(Array array, int arrayIndex) {
			CopyTo((TaskbarThumbButtonInfo[])array, arrayIndex);
		}
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return syncRoot; } }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		#endregion
	}
}

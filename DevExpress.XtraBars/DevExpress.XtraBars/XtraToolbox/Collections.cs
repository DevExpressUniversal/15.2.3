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

using DevExpress.Utils.Design;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.XtraToolbox {
	[TypeConverter(typeof(UniversalCollectionTypeConverter))]
	public class ToolboxElementCollection<T> : CollectionBase, IEnumerable<T> where T : ToolboxElementBase {
		int lockUpdate;
		public ToolboxElementCollection() {
			this.lockUpdate = 0;
		}
		public virtual void Add(T item) {
			List.Add(item);
		}
		public virtual void AddRange(IEnumerable<T> items) {
			BeginUpdate();
			foreach(T item in items) {
				Add(item);
			}
			EndUpdate();
		}
		public virtual T this[int index] {
			get { return List[index] as T; }
			set {
				if(value == null) return;
				List[index] = value;
			}
		}
		public virtual bool Remove(T item) {
			if(!Contains(item)) return false;
			List.Remove(item);
			return true;
		}
		public virtual void Insert(int position, T item) {
			if(Contains(item)) return;
			List.Insert(position, item);
		}
		public virtual bool Contains(T item) {
			return List.Contains(item);
		}
		public virtual int IndexOf(T item) {
			return List.IndexOf(item);
		}
		public virtual void BeginUpdate() {
			lockUpdate++;
		}
		public virtual void EndUpdate() {
			if(--lockUpdate != 0) return;
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		protected override void OnInsert(int position, object value) {
			base.OnInsert(position, value);
		}
		protected override void OnInsertComplete(int position, object value) {
			T item = (T)value;
			base.OnInsertComplete(position, value);
			OnInsertCompleteCore(item);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, position));
		}
		protected virtual void OnInsertCompleteCore(T item) { }
		protected override void OnRemoveComplete(int position, object value) {
			T item = (T)value;
			base.OnRemoveComplete(position, value);
			OnRemoveCompleteCore(item);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, position));
		}
		protected virtual void OnRemoveCompleteCore(T item) { }
		protected override void OnClearComplete() {
			base.OnClearComplete();
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}
		protected virtual void RaiseListChanged(ListChangedEventArgs e) {
			if(lockUpdate != 0) return;
			if(ListChanged != null) ListChanged(this, e);
		}
		public event ListChangedEventHandler ListChanged;
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			foreach(ToolboxElementBase item in List) yield return item as T;
		}
	}
	[TypeConverter(typeof(UniversalCollectionTypeConverter))]
	public class ToolboxItemCollection : ToolboxElementCollection<ToolboxItem> {
		public ToolboxItemCollection(ToolboxGroup ownerGroup) {
			this.ownerGroup = ownerGroup;
		}
		ToolboxGroup ownerGroup;
		public ToolboxGroup OwnerGroup {
			get { return ownerGroup; }
		}
		protected override void OnInsertCompleteCore(ToolboxItem item) {
			base.OnInsertCompleteCore(item);
			if(OwnerGroup == null) return;
			item.SetOwner(OwnerGroup.Owner);
			item.SetOwnerGroup(OwnerGroup);
		}
		protected override void OnRemoveCompleteCore(ToolboxItem item) {
			base.OnRemoveCompleteCore(item);
			item.ResetOwner();
			item.ResetOwnerGroup();
		}
	}
	[TypeConverter(typeof(UniversalCollectionTypeConverter))]
	public class ToolboxGroupCollection : ToolboxElementCollection<ToolboxGroup> {
		IToolboxControl owner;
		public ToolboxGroupCollection(IToolboxControl owner) {
			this.owner = owner;
		}
		public IToolboxControl Owner {
			get { return owner; }
		}
		protected override void OnInsertCompleteCore(ToolboxGroup item) {
			base.OnInsertCompleteCore(item);
			item.SetOwner(Owner);
			if(item.Items == null || item.Items.Count <= 0) return;
			item.SetItemsOwner(Owner);
		}
		protected override void OnRemoveCompleteCore(ToolboxGroup item) {
			base.OnRemoveCompleteCore(item);
			item.ResetOwner();
			if(item.Items == null || item.Items.Count <= 0) return;
			item.ResetItemsOwner();
		}
	}
}

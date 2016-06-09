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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.Utils.Design;
using DevExpress.XtraDiagram.Base;
namespace DevExpress.XtraDiagram {
	[TypeConverter(typeof(UniversalCollectionTypeConverter))]
	public class DiagramItemCollection : CollectionBase, IEnumerable<DiagramItem>, IList<IDiagramItem> {
		IDiagramItem owner;
		int lockUpdate;
		public DiagramItemCollection(IDiagramItem owner) {
			this.lockUpdate = 0;
			this.owner = owner;
		}
		public virtual void Add(DiagramItem item) {
			List.Add(item);
		}
		public virtual void AddRange(params DiagramItem[] items) {
			BeginUpdate();
			try {
				Array.ForEach(items, item => Add(item));
			}
			finally {
				EndUpdate();
			}
		}
		public virtual DiagramItem this[int index] {
			get { return List[index] as DiagramItem; }
			set {
				if(value == null) return;
				List[index] = value;
			}
		}
		public virtual bool Remove(DiagramItem item) {
			if(!Contains(item)) return false;
			List.Remove(item);
			return true;
		}
		public virtual void Insert(int position, DiagramItem item) {
			if(Contains(item)) return;
			List.Insert(position, item);
		}
		public virtual bool Contains(DiagramItem item) {
			return List.Contains(item);
		}
		public virtual int IndexOf(DiagramItem item) {
			return List.IndexOf(item);
		}
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}
		protected internal virtual void DestroyItems() {
			BeginUpdate();
			try {
				for(int i = Count - 1; i >= 0; i--) {
					this[i].Dispose();
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		protected override void OnInsert(int position, object value) {
			Controller.InsertItem(position, (IDiagramItem)value, () => base.OnInsert(position, value));
		}
		protected override void OnInsertComplete(int position, object value) {
			base.OnInsertComplete(position, value);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, position));
		}
		protected override void OnRemove(int index, object value) {
			DiagramItem item = (DiagramItem)value;
			if(item.IsDisposing) return;
			Controller.RemoveItem(index, () => base.OnRemove(index, value));
		}
		protected override void OnRemoveComplete(int position, object value) {
			base.OnRemoveComplete(position, value);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, position));
		}
		protected override void OnClearComplete() {
			Controller.ClearItems(() => base.OnClearComplete());
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			Controller.SetItem(index, (IDiagramItem)newValue, () => base.OnSetComplete(index, oldValue, newValue));
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}
		protected virtual void RaiseListChanged(ListChangedEventArgs e) {
			if(lockUpdate != 0) return;
			if(ListChanged != null) ListChanged(this, e);
		}
		public event ListChangedEventHandler ListChanged;
		#region IList<IDiagramItem>
		IDiagramItem IList<IDiagramItem>.this[int index] {
			get { return this[index]; }
			set { this[index] = (DiagramItem)value; }
		}
		int IList<IDiagramItem>.IndexOf(IDiagramItem item) {
			return IndexOf((DiagramItem)item);
		}
		bool ICollection<IDiagramItem>.IsReadOnly { get { return List.IsReadOnly; } }
		void IList<IDiagramItem>.Insert(int index, IDiagramItem item) {
			Insert(index, (DiagramItem)item);
		}
		void ICollection<IDiagramItem>.Add(IDiagramItem item) {
			Add((DiagramItem)item);
		}
		bool ICollection<IDiagramItem>.Contains(IDiagramItem item) {
			return Contains((DiagramItem)item);
		}
		void ICollection<IDiagramItem>.CopyTo(IDiagramItem[] array, int arrayIndex) {
			List.CopyTo(array, arrayIndex);
		}
		bool ICollection<IDiagramItem>.Remove(IDiagramItem item) {
			return Remove((DiagramItem)item);
		}
		IEnumerator<IDiagramItem> IEnumerable<IDiagramItem>.GetEnumerator() {
			foreach(IDiagramItem item in List) yield return item;
		}
		IEnumerator<DiagramItem> IEnumerable<DiagramItem>.GetEnumerator() {
			foreach(DiagramItem item in List) yield return item;
		}
		#endregion
		protected IDiagramItem Owner { get { return owner; } }
		ItemCollectionController<IDiagramItem> controller = null;
		protected ItemCollectionController<IDiagramItem> Controller {
			get {
				if(controller == null) controller = CreateCollectionController();
				return controller;
			}
		}
		protected virtual ItemCollectionController<IDiagramItem> CreateCollectionController() {
			return new ItemCollectionController<IDiagramItem>(Owner, this);
		}
	}
}

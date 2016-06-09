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
using System.Windows;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
namespace DevExpress.Xpf.Gauges {
	public partial class GaugeElementCollection<T> : ObservableCollection<T>, IOwnedElement {
		object owner;
		ILogicalParent logicalParent;
		protected object Owner { get { return owner; } }
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				RemoveChildren(this);
				owner = value;
				logicalParent = owner as ILogicalParent;
				AddChildren(this);
			}
		}
		#endregion
		void SetOwnerForChildren(IList children, object owner) {
			foreach (object child in children) {
				IOwnedElement element = child as IOwnedElement;
				if (element != null)
					element.Owner = owner;
			}
		}
		void AddChildren(IList children) {
			SetOwnerForChildren(children, owner);
			if (logicalParent != null) {
				foreach (object child in children) {
					if (child != null)
						logicalParent.AddChild(child);
				}
			}
		}
		void RemoveChildren(IList children) {
			SetOwnerForChildren(children, null);
			if (logicalParent != null) {
				foreach (object child in children) {
					if (child != null)
						logicalParent.RemoveChild(child);
				}
			}
		}
		protected override void ClearItems() {
			RemoveChildren(this);
			base.ClearItems();
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (e.OldItems != null && (e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace))
				RemoveChildren(e.OldItems);
			if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace))
				AddChildren(e.NewItems);
		}
	}
	[NonCategorized]
	public abstract partial class GaugeDependencyObjectCollectionBase<T> : FreezableCollection<T> where T : DependencyObject {
		public new T this[int index] { get { return base[index]; } set { SetItem(index, value); } }
		public GaugeDependencyObjectCollectionBase() {
			((INotifyCollectionChanged)this).CollectionChanged += new NotifyCollectionChangedEventHandler(BaseCollectionChanged);
		}
		void BaseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			OnCollectionChanged(e);
		}
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
		}
		protected virtual void ClearItems() {
			base.Clear();
		}
		protected virtual void InsertItem(int index, T item) {
			base.Insert(index, item);
		}
		protected virtual void MoveItem(int oldIndex, int newIndex) {
			T item = this[oldIndex];
			base.RemoveAt(oldIndex);
			base.Insert(newIndex, item);
		}
		protected virtual void RemoveItem(int index) {
			base.RemoveAt(index);
		}
		protected virtual void SetItem(int index, T item) {
			base[index] = item;
		}
		public new void Add(T item) {
			InsertItem(Count, item);
		}
		public new void Insert(int index, T item) {
			InsertItem(index, item);
		}
		public new bool Remove(T item) {
			int index = IndexOf(item);
			if (index >= 0) {
				RemoveItem(index);
				return true;
			}
			return false;
		}
		public new void RemoveAt(int index) {
			RemoveItem(index);
		}
		public new void Clear() {
			ClearItems();
		}
		public void Move(int oldIndex, int newIndex) {
			MoveItem(oldIndex, newIndex);
		}
	}
	public partial class GaugeDependencyObjectCollection<T> : GaugeDependencyObjectCollectionBase<T>, IOwnedElement where T : DependencyObject {
		void SetOwnerForChildren(IList children, object owner) {
			foreach (object child in children) {
				IOwnedElement element = child as IOwnedElement;
				if (element != null)
					element.Owner = owner;
			}
		}
		object owner;
		protected object Owner { get { return owner; } }
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				RemoveChildren(this);
				owner = value;
				AddChildren(this);
			}
		}
		#endregion
		protected virtual void AddChildren(IList children) {
			SetOwnerForChildren(children, owner);
		}
		protected virtual void RemoveChildren(IList children) {
			SetOwnerForChildren(children, null);
		}
		protected override void ClearItems() {
			RemoveChildren(this);
			base.ClearItems();
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (e.OldItems != null && (e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace))
				RemoveChildren(e.OldItems);
			if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace))
				AddChildren(e.NewItems);
			IModelSupported modelHolder = owner as IModelSupported;
			if (modelHolder != null)
				modelHolder.UpdateModel();
		}
	}
	public abstract class WeakEventListenerCollection<T> : GaugeDependencyObjectCollection<T>, IWeakEventListener where T : DependencyObject {
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		protected abstract bool PerformWeakEvent(Type managerType, object sender, EventArgs e);
		protected override void AddChildren(IList children) {
			base.AddChildren(children);
			foreach (object child in children)
				if (child is INotifyPropertyChanged)
					PropertyChangedWeakEventManager.AddListener(child as INotifyPropertyChanged, this);
		}
		protected override void RemoveChildren(IList children) {
			base.RemoveChildren(children);
			foreach (object child in children)
				if (child is INotifyPropertyChanged)
					PropertyChangedWeakEventManager.RemoveListener(child as INotifyPropertyChanged, this);
		}									
	}
}

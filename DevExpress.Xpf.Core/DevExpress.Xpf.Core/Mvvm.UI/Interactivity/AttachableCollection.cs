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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
#if NETFX_CORE
using Windows.UI.Xaml;
#endif
namespace DevExpress.Mvvm.UI.Interactivity {
#if !SILVERLIGHT && !NETFX_CORE
	public abstract class AttachableCollection<T> : FreezableCollection<T>, IAttachableObject where T : DependencyObject, IAttachableObject {
#else
	public abstract class AttachableCollection<T> : DependencyObjectCollection<T>, IAttachableObject where T : DependencyObject, IAttachableObject {
#endif
		internal AttachableCollection() {
			((INotifyCollectionChanged)this).CollectionChanged += this.OnCollectionChanged;
		}
		public void Attach(DependencyObject obj) {
			if(obj == this.AssociatedObject) return;
			if(AssociatedObject != null)
				throw new InvalidOperationException("This AttachableCollection is already attached");
			AssociatedObject = obj;
			OnAttached();
		}
		public void Detach() {
			OnDetaching();
			AssociatedObject = null;
		}
		DependencyObject _associatedObject;
		DependencyObject IAttachableObject.AssociatedObject { get { return AssociatedObject; } }
		protected internal DependencyObject AssociatedObject {
			get {
				VerifyRead();
				return _associatedObject;
			}
			private set {
				VerifyWrite();
				_associatedObject = value;
				NotifyChanged();
			}
		}
		protected virtual void OnAttached() {
			foreach(T item in this) {
				if(ShouldAttachItem(item))
					item.Attach(AssociatedObject);
			}
		}
		protected virtual void OnDetaching() {
			foreach(T item in this)
				item.Detach();
		}
		bool ShouldAttachItem(T item) {
			if(!InteractionHelper.IsInDesignMode(AssociatedObject))
				return true;
			return !InteractionHelper.IsInDesignMode(item);
		}
		void VerifyRead() {
#if !SILVERLIGHT && !NETFX_CORE
			ReadPreamble();
#endif
		}
		void VerifyWrite() {
#if !SILVERLIGHT && !NETFX_CORE
			WritePreamble();
#endif
		}
		void NotifyChanged() {
#if !SILVERLIGHT && !NETFX_CORE
			WritePostscript();
#endif
		}
		internal virtual void ItemAdded(T item) {
			if(ShouldAttachItem(item))
				AssociatedObject.Do(x => item.Attach(x));
		}
		internal virtual void ItemRemoved(T item) {
			if(item.AssociatedObject == null) return;
			item.Detach();
		}
		List<T> snapshot = new List<T>();
		void AddItem(T item) {
			if(snapshot.Contains(item))
				return;
			ItemAdded(item);
			snapshot.Insert(IndexOf(item), item);
		}
		void RemoveItem(T item) {
			ItemRemoved(item);
			snapshot.Remove(item);
		}
		void ClearItems() {
			foreach(T item in snapshot)
				ItemRemoved(item);
			snapshot.Clear();
		}
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
#if !SILVERLIGHT && !NETFX_CORE
			if(e.Action == NotifyCollectionChangedAction.Move)
				return;
#endif
			if(e.Action == NotifyCollectionChangedAction.Reset) {
				ClearItems();
				foreach(T item in this)
					AddItem(item);
				return;
			}
			if(e.OldItems != null) {
				foreach(T item in e.OldItems)
					RemoveItem(item);
			}
			if(e.NewItems != null) {
				foreach(T item in e.NewItems)
					AddItem(item);
			}
		}
#if !SILVERLIGHT && !NETFX_CORE
		protected override sealed Freezable CreateInstanceCore() {
			return (Freezable)Activator.CreateInstance(GetType());
		}
#endif
	}
	public sealed class BehaviorCollection : AttachableCollection<Behavior> { }
	public sealed class TriggerCollection : AttachableCollection<TriggerBase> { }
}

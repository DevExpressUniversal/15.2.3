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
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Windows;
#if SL
using DXFrameworkContentElement = System.Windows.FrameworkElement;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class PivotChildCollection<T> : ObservableCollection<T> where T : DXFrameworkContentElement {
		ILogicalOwner owner;
#if SL
		internal ResourceCollectionSyncronizer<FrameworkElement> resourcesSyncronizer;
#endif
		public PivotChildCollection(ILogicalOwner owner) : this(owner, false, null) { }
		public PivotChildCollection(ILogicalOwner owner, bool syncInOwnerResources) : this(owner, syncInOwnerResources, null) { }
		public PivotChildCollection(ILogicalOwner owner, bool syncInOwnerResources, DXFrameworkContentElement syncResourcesOwner) {
			this.owner = owner;
#if SL
			syncResourcesOwner = syncResourcesOwner ?? Owner as FrameworkElement;
			if(syncInOwnerResources && syncResourcesOwner != null) {
				resourcesSyncronizer = new ResourceCollectionSyncronizer<FrameworkElement>(syncResourcesOwner, this, pivot => true, Guid.NewGuid().ToString());
			}
#endif
		}
		public void AddRange(IEnumerable<T> collection) {
			foreach(T item in collection)
				Add(item);
		}
		protected internal ILogicalOwner Owner { 
			get { return owner; }
			set {
				if(owner == value) return;
				OnPivotGridChanging(owner);
				owner = value;
				OnPivotGridChanged(owner);
			}
		}
		protected virtual void OnPivotGridChanging(ILogicalOwner oldPivot) {
			if(oldPivot != null)
				RemoveLogicalChilds(this);
		}
		protected virtual void OnPivotGridChanged(ILogicalOwner newPivot) {
			if(newPivot != null)
				AddLogicalChilds(this);
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
#if SL
			if(resourcesSyncronizer != null) {
				Action action = delegate() {
					resourcesSyncronizer.SynchronizeResources(e);
				};
				PivotGridControl.InvokeAction(action, Owner as PivotGridControl);
			}
#endif
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					OnItemsAdded(e.NewStartingIndex, e.NewItems);
					break;
#if !SL
				case NotifyCollectionChangedAction.Move:
					OnItemMoved(e.OldStartingIndex, e.NewStartingIndex, (T)e.OldItems[0]);
					break;
#endif
				case NotifyCollectionChangedAction.Remove:
					OnItemRemoved(e.OldStartingIndex, (T)e.OldItems[0]);
					break;
				case NotifyCollectionChangedAction.Replace:
					OnItemReplaced(e.NewStartingIndex, (T)e.OldItems[0], (T)e.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Reset:
					break;
				default:
					throw new ArgumentException("Unsupported action (" + e.Action.ToString() + ")");
			}
		}
		protected override void ClearItems() {
			OnItemsClearing(new List<T>(this));
			base.ClearItems();
		}
		protected void AddLogicalChild(object child) {
			if(Owner == null)
				return;
			Owner.AddChild(child);
		}
		protected void RemoveLogicalChild(object child) {
			if(Owner == null)
				return;
			Owner.RemoveChild(child);
		}
		void AddLogicalChilds(IList list) {
			for(int i = 0; i < list.Count; i++) {
				OnItemAdded(i, (T)list[i]);
			}
		}
		void RemoveLogicalChilds(IList<T> list) {
			for(int i = 0; i < list.Count; i++) {
				RemoveLogicalChild(list[i]);
			}
		}
		void OnItemsAdded(int index, IList list) {
			for(int i = 0; i < list.Count; i++) {
				OnItemAdded(index + i, (T)list[i]);
			}
		}	   
		protected virtual void OnItemAdded(int index, T item) {
			AddLogicalChild(item);
		}
		protected virtual void OnItemMoved(int oldIndex, int newIndex, T item) { }
		protected virtual void OnItemRemoved(int index, T item) {
			RemoveLogicalChild(item);
		}
		protected virtual void OnItemReplaced(int index, T oldItem, T newItem) {
			RemoveLogicalChild(oldItem);
		}
		protected virtual void OnItemsClearing(IList<T> oldItems) {
			RemoveLogicalChilds(oldItems);
		}
	}
}

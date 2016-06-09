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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
namespace DevExpress.Xpf.Docking {
	public abstract class BaseItemsCollection<T> : ObservableCollection<T>, IDisposable, IWeakEventListener {
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDisposing() {
			ClearItemsSource();
			ClearItems();
		}
		#region ItemsSource
		public void ClearItemsSource() {
			if(IsUsingItemsSource) {
				UnsubscribeSource(itemsSource);
				itemsSource = null;
				InvalidateItemsSource();
			}
		}
		protected IEnumerable ItemsSource { get { return itemsSource; } }
		IEnumerable itemsSource;
		internal bool IsUsingItemsSource {
			get { return itemsSource != null; }
		}
		public void SetItemsSource(IEnumerable value) {
			if(!IsUsingItemsSource && (Count > 0)) {
				throw new InvalidOperationException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.ItemCollectionMustBeEmpty));
			}
			SetItemsSourceCore(value);
		}
		void SetItemsSourceCore(IEnumerable value) {
			UnsubscribeSource(itemsSource);
			itemsSource = value;
			SubscribeSource(itemsSource);
			InvalidateItemsSource();
			OnAddToItemsSource(value);
		}
		ICollectionView collectionViewCore;
		public object CurrentItem { get { return collectionViewCore != null ? collectionViewCore.CurrentItem : null; } }
		void collectionView_CurrentChanged(object sender, EventArgs e) {
			OnCurrentChanged(sender);
		}
		public void MoveCurrentTo(object item) {
			if(collectionViewCore!= null)
				collectionViewCore.MoveCurrentTo(item);
		}
		void SubscribeSource(IEnumerable Source) {
			INotifyCollectionChanged collection = Source as INotifyCollectionChanged;
			if(collection != null) {
				CollectionChangedEventManager.AddListener(collection, this);
			}
			collectionViewCore = System.Windows.Data.CollectionViewSource.GetDefaultView(Source);
			if(collectionViewCore != null) {
				CurrentChangedEventManager.AddListener(collectionViewCore, this);
			}
		}
		void UnsubscribeSource(IEnumerable Source) {
			INotifyCollectionChanged collection = Source as INotifyCollectionChanged;
			if(collection != null) {
				CollectionChangedEventManager.RemoveListener(collection, this);
			}
			if(collectionViewCore != null) {
				CurrentChangedEventManager.RemoveListener(collectionViewCore, this);
			}
		}
		protected virtual void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					OnAddToItemsSource(e.NewItems, e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Remove:
					OnRemoveFromItemsSource(e.OldItems);
					break;
				case NotifyCollectionChangedAction.Replace:
					OnItemReplacedInItemsSource(e.OldItems, e.NewItems, e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Move:
					OnItemMovedInItemsSource(e.OldStartingIndex, e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Reset:
					OnResetItemsSource();
					break;
			}
		}
		protected virtual void OnItemReplacedInItemsSource(IList oldItems, IList newItems, int newStartingIndex) { }
		protected virtual void OnItemMovedInItemsSource(int oldStartingIndex, int newStartingIndex) { }
		protected abstract void OnAddToItemsSource(IEnumerable newItems, int startingIndex = 0);
		protected abstract void OnRemoveFromItemsSource(IEnumerable oldItems);
		protected abstract void OnResetItemsSource();
		protected abstract void OnCurrentChanged(object sender);
		protected virtual void InvalidateItemsSource() { }
		#endregion
		#region IWeakEventListener Members
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(CollectionChangedEventManager)) {
				NotifyCollectionChangedEventArgs args = (NotifyCollectionChangedEventArgs)e;
				OnItemsSourceCollectionChanged(sender, args);
			}
			else if(managerType == typeof(CurrentChangedEventManager)) {
				collectionView_CurrentChanged(sender, e);
			}
			else {
				return false;
			}
			return true;
		}
		#endregion
	}
	public class DockLayoutManagerItemsCollection : BaseItemsCollection<BaseLayoutItem> {
		protected readonly DockLayoutManager Owner;
		public DockLayoutManagerItemsCollection(DockLayoutManager owner) {
			Owner = owner;
		}
		protected override void OnAddToItemsSource(IEnumerable newItems, int startingIndex = 0) {
			Owner.OnAddToItemsSource(newItems, startingIndex);
		}
		protected override void OnRemoveFromItemsSource(IEnumerable oldItems) {
			Owner.OnRemoveFromItemsSource(oldItems);
		}
		protected override void OnItemReplacedInItemsSource(IList oldItems, IList newItems, int newStartingIndex) {
			Owner.OnItemReplacedInItemsSource(oldItems, newItems, newStartingIndex);
		}
		protected override void InvalidateItemsSource() {
			Owner.OnResetItemsSource(null);
		}
		protected override void OnResetItemsSource() {
			Owner.OnResetItemsSource(ItemsSource);
		}
		protected override void OnCurrentChanged(object sender) {
			Owner.OnCurrentChanged(sender);
		}
	}
}

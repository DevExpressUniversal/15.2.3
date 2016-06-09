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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core {
	public interface IItemsHost {
		void Add(IDiagramItem item);
		void Remove(IDiagramItem item);
		void Insert(int index, IDiagramItem item);
	}
	public class ItemCollectionController<TItem> where TItem : IDiagramItem {
		internal readonly TItem ownerItem;
		internal IItemsHost itemsHost;
#if DEBUGTEST
		public IItemsHost ItemsHostsForTests { get { return itemsHost; } }
#endif
		readonly IList<TItem> collection;
		public ItemCollectionController(TItem ownerItem, IList<TItem> collection) {
			this.ownerItem = ownerItem;
			this.collection = collection;
		}
		void InsertItemCore(int index, TItem item) {
			if(itemsHost != null) {
				itemsHost.Insert(index, item);
			}
		}
		void NotifyChanged(TItem item, ItemChangedKind itemChangedKind) {
			ownerItem.GetRootDiagram().Do(x => {
				item.IterateSelfAndChildren(y => y.NotifyChanged(itemChangedKind));
			});
		}
		void DoInsertItem(int index, TItem item) {
			InsertItemCore(index, item);
			item.Controller.SetOwner(ownerItem);
			NotifyChanged(item, ItemChangedKind.Added);
		}
		void RemoveItems(IEnumerable<TItem> items) {
			if(itemsHost == null)
				return;
			foreach(var item in items) {
				itemsHost.Remove(item);
			}
		}
		void DetachItem(TItem item) {
			RemoveItems(item.Yield());
			NotifyChanged(item, ItemChangedKind.Removed);
			item.Controller.SetOwner(null);
		}
		public void SetHost(IItemsHost itemsHost) {
			RemoveItems(collection);
			this.itemsHost = itemsHost;
			if(itemsHost != null) {
				foreach(var item in collection) {
					itemsHost.Add(item);
				}
			}
		}
		public void InsertItem(int index, TItem item, Action baseAction) {
			baseAction();
			DoInsertItem(index, item);
		}
		public void MoveItem(int oldIndex, int newIndex, Action baseAction) {
			var item = collection[oldIndex];
			baseAction();
			RemoveItems(item.Yield());
			InsertItemCore(newIndex, item);
			item.NotifyChanged(ItemChangedKind.ZOrderChanged);
		}
		public void SetItem(int index, TItem item, Action baseAction) {
			var itemToRemove = collection[index];
			baseAction();
			DetachItem(itemToRemove);
			DoInsertItem(index, item);
		}
		public void RemoveItem(int index, Action baseAction) {
			var item = collection[index];
			baseAction();
			DetachItem(item);
		}
		public void ClearItems(Action baseAction) {
			var snapshot = this.collection.ToArray();
			baseAction();
			foreach(var item in snapshot) {
				DetachItem(item);
			}
		}
	}
}

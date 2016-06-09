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
using DevExpress.Internal;
using System.Linq;
namespace DevExpress.Diagram.Core {
	public struct AddItemInfo {
		public readonly IDiagramItem Item, Owner;
		public int? Index;
		public AddItemInfo(IDiagramItem item, IDiagramItem owner, int? index) {
			Item = item;
			Owner = owner;
			Index = index;
		}
	}
	public static class AddRemoveActions {
		class AddItemsState {
			public readonly Func<IDiagramItem[]> GetItems;
			public readonly IDiagramControl Diagram;
			public readonly IItemFinder<IDiagramItem>[] Owners;
			public readonly int[] Indexes;
			public AddItemsState(IDiagramControl diagram, Func<IDiagramItem[]> getItems, IEnumerable<IDiagramItem> owners, int[] indexes) {
				GetItems = getItems;
				Owners = owners.Select(x => x.GetFinder()).ToArray();
				Indexes = indexes;
				Diagram = diagram;
			}
		}
		class RemoveItemsState {
			public readonly IDiagramControl Diagram;
			public readonly IItemFinder<IDiagramItem>[] Items;
			public RemoveItemsState(IDiagramControl diagram, IDiagramItem[] items) {
				Diagram = diagram;
				Items = items.Select(x => x.GetFinder()).ToArray();
			}
		}
		public static void RemoveItem(this Transaction transaction, IDiagramItem item) {
			transaction.RemoveItems(item.GetRootDiagram(), new[] { item });
		}
		public static void RemoveItems(this Transaction transaction, IDiagramControl diagram, IDiagramItem[] items) {
			transaction.Execute(new RemoveItemsState(diagram, items), RemoveItems, AddItems);
		}
		public static void AddItem(this Transaction transaction, IDiagramItem item, IDiagramItem owner, int? index = null) {
			transaction.AddItems(owner.GetRootDiagram(), new AddItemInfo(item, owner, index).Yield());
		}
		public static void AddItems(this Transaction transaction, IDiagramControl diagram, IEnumerable<AddItemInfo> addInfos) {
			addInfos = addInfos.Reverse().ToArray();
			if(!addInfos.Any())
				return;
			transaction.Execute(
				new AddItemsState(
					diagram, 
					() => addInfos.Select(x => x.Item).ToArray(), 
					addInfos.Select(x => x.Owner), 
					addInfos.Select(x => x.Index ?? x.Owner.NestedItems.Count).ToArray()
				), 
				AddItems, RemoveItems);
		}
		static AddItemsState RemoveItems(RemoveItemsState removeState) {
			var itemsToRemove = removeState.Items
				.Select(x => x.FindItem())
				.OrderBy(x => x.GetFinder().Path)
				.ToArray();
			var serialized = removeState.Diagram.SerializeItems(itemsToRemove, StoreRelationsMode.RelativeToDiagram);
			var addState = new AddItemsState(
				removeState.Diagram, 
				() => removeState.Diagram.DeserializeItems(serialized, storeRelationsMode: null).ToArray(), 
				itemsToRemove.Select(x => x.Owner()),
				itemsToRemove.Select(x => x.GetIndexInOwnerCollection()).ToArray());
			foreach(var itemToRemove in itemsToRemove) {
				itemToRemove.OwnerCollection().RemoveAt(itemToRemove.GetIndexInOwnerCollection());
			}
			return addState;
		}
		static RemoveItemsState AddItems(AddItemsState addState) {
			var items = addState.GetItems();
			if(items.Length != addState.Owners.Length || items.Length != addState.Owners.Length)
				throw new InvalidOperationException();
			for(int i = 0; i < items.Length; i++) {
				addState.Owners[i].FindItem().NestedItems.Insert(addState.Indexes[i], items[i]);
			}
			items.SelectMany(x => x.GetSelfAndChildren())
				.ForEach(x => {
					x.Controller.RestoreRelations(path => path.FindItem(addState.Diagram, addState.Diagram.Items()));
				});
			return new RemoveItemsState(addState.Diagram, items.ToArray());
		}
	}
	public static class AddRemoveActions2 {
		class AddItemState<T> {
			internal readonly T Item;
			internal readonly IItemFinder<IList<T>> Owner;
			internal readonly int Index;
			public AddItemState(T item, IItemFinder<IList<T>> owner, int index) {
				Item = item;
				Owner = owner;
				Index = index;
			}
		}
		class RemoveItemState<T> {
			internal readonly IItemFinder<IList<T>> Owner;
			internal readonly int Index;
			public RemoveItemState(IItemFinder<IList<T>> owner, int index) {
				this.Owner = owner;
				this.Index = index;
			}
		}
		public static void RemoveItem<T>(this Transaction transaction, IItemFinder<IList<T>> owner, int index) {
			transaction.Execute(new RemoveItemState<T>(owner, index), RemoveItem, AddItem);
		}
		public static void InsertItem<T>(this Transaction transaction, IItemFinder<IList<T>> owner, T item, int index) {
			transaction.Execute(new AddItemState<T>(item, owner, index), AddItem, RemoveItem);
		}
		static AddItemState<T> RemoveItem<T>(RemoveItemState<T> x) {
			var owner = x.Owner.FindItem();
			var itemToRemove = owner[x.Index];
			var state = new AddItemState<T>(itemToRemove, x.Owner, x.Index);
			if(state.Index < 0)
				throw new InvalidOperationException();
			owner.RemoveAt(x.Index);
			return state;
		}
		static RemoveItemState<T> AddItem<T>(AddItemState<T> x) {
			var restoredItem = x.Item;
			var owner = x.Owner.FindItem();
			owner.Insert(x.Index, restoredItem);
			return new RemoveItemState<T>(x.Owner, x.Index);
		}
	}
}

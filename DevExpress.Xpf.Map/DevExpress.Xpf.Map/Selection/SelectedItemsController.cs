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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Collections;
namespace DevExpress.Xpf.Map {
	public enum ElementSelectionMode { None, Single, Multiple, Extended }
	public delegate void MapItemSelectionChangedEventHandler(object sender, MapItemSelectionChangedEventArgs e);
	[NonCategorized]
	public class MapItemSelectionChangedEventArgs : EventArgs {
		public MapItemSelectionChangedEventArgs(List<object> selection) {
			Selection = new List<object>();
			Selection.AddRange(selection);
		}
		public List<object> Selection { get; private set; }
	}
}
namespace DevExpress.Xpf.Map.Native {
	public enum SelectionAction { None, Clear, Add, Remove, ReplaceAll }
	public static class SelectionHelper {
		public static SelectionAction CalculateSelectionAction(ElementSelectionMode selectionMode, ModifierKeys keyModifiers, bool isItemSelected, bool layerEnabledSelection) {
			if (!layerEnabledSelection)
				return SelectionAction.Clear;
			switch (selectionMode) {
				case ElementSelectionMode.Single:
					return isItemSelected ? SelectionAction.None : SelectionAction.ReplaceAll;
				case ElementSelectionMode.Multiple:
					return isItemSelected ? SelectionAction.Remove : SelectionAction.Add;
				case ElementSelectionMode.Extended: {
						if (keyModifiers == ModifierKeys.Shift)
							return isItemSelected ? SelectionAction.None : SelectionAction.Add;
						if (keyModifiers == ModifierKeys.Control)
							return isItemSelected ? SelectionAction.Remove : SelectionAction.Add;
						return isItemSelected ? SelectionAction.None : SelectionAction.ReplaceAll;
					}
			}
			return SelectionAction.None;
		}
	}
	public class SelectedItemsController {
		VectorLayerBase layer;
		IList currentList = null;
		Locker updateSelectedItemLocker = new Locker();
		Locker updateSelectedItemsLocker = new Locker();
		public object SelectedItem { get { return layer.SelectedItem; } set { layer.SelectedItem = value; } }
		public IList SelectedItems { get { return layer.SelectedItems; } set { layer.SelectedItems = value; } }
		protected bool IsEventsSuspended { get { return updateSelectedItemsLocker.IsLocked; } }
		public SelectedItemsController(VectorLayerBase layer) {
			this.layer = layer;
		}
		void SafeSetSelectedItems(IList newList) {
			updateSelectedItemLocker.Lock();
			try {
				SelectedItems = newList;
			}
			finally {
				updateSelectedItemLocker.Unlock();
			}
		}
		void UpdateSelectedItemByList(IList list) {
			SelectedItem = list != null && list.Count > 0 ? list[0] : default(object);
		}
		IList CopySelectedItems() {
			IList collection = CreateNewSelectedItems();
			if (SelectedItems != null)
				foreach (object item in SelectedItems)
					collection.Add(item);
			return collection;
		}
		IList CreateNewSelectedItems() {
			return new ObservableCollection<object>();
		}
		void ClearAllSelectedItems() {
			foreach (LayerBase item in layer.Map.Layers) {
				VectorLayerBase vectorLayer = item as VectorLayerBase;
				if (vectorLayer != null && vectorLayer != layer)
					vectorLayer.ClearSelectionWithoutEvent();
			}
		}
		void EnsureSelectedItems() {
			if (SelectedItems == null)
				SelectedItems = CreateNewSelectedItems();
		}
		void AddSelectedItem(object item) {
			EnsureSelectedItems();
			IList newList = CopySelectedItems();
			newList.Add(item);
			SafeSetSelectedItems(newList);
		}
		void RemoveSelectedItem(object item) {
			EnsureSelectedItems();
			IList newList = CopySelectedItems();
			newList.Remove(item);
			SafeSetSelectedItems(newList);
		}
		void SuspendEvents() {
			updateSelectedItemsLocker.Lock();
		}
		void ResumeEvents() {
			updateSelectedItemsLocker.Unlock();
		}
		void SetSelectedItem(object item) {
			EnsureSelectedItems();
			if (updateSelectedItemsLocker.IsLocked)
				return;
			IList newList = CreateNewSelectedItems();
			newList.Add(item);
			SafeSetSelectedItems(newList);
		}
		internal void UpdateActualItemsSelection() {
			foreach (MapItem item in layer.DataItems) {
				if (item.Info != null) {
					object obj = item.Info.Source;
					item.SetIsSelected(SelectedItems != null && obj != null ? SelectedItems.Contains(obj) : false);
				}
			}
		}
		public void UpdateItemSelection(ElementSelectionMode selectionMode, ModifierKeys keyModifiers, MapItemInfo item) {
			SelectionAction action = SelectionHelper.CalculateSelectionAction(selectionMode, keyModifiers, item.IsSelected, layer.EnableSelection);
			object selectedItem = item.Source;
			switch (action) {
				case SelectionAction.None:
					return;
				case SelectionAction.ReplaceAll:
					ClearAllSelectedItems();
					SetSelectedItem(selectedItem);
					break;
				case SelectionAction.Add:
					AddSelectedItem(selectedItem);
					break;
				case SelectionAction.Remove:
					RemoveSelectedItem(selectedItem);
					break;
			}
		}
		public void ClearSelectedItems() {
			if (SelectedItems == null)
				return;
			IList newList = CreateNewSelectedItems();
			SafeSetSelectedItems(newList);
		}
		public void OnUpdateSelectedItem(object item) {
			if (updateSelectedItemLocker.IsLocked)
				return;
			if (item == null) {
				SelectedItems = null;
			}
			else {
				SetSelectedItem(item);
			}
			UpdateActualItemsSelection();
		}
		public void OnUpdateSelectedItems(IList list) {
			SuspendEvents();
			try {
				UpdateSelectedItemByList(list);
				SetCurrentList(list);
			}
			finally {
				ResumeEvents();
				layer.OnSelectionChanged();
			}
			UpdateActualItemsSelection();
		}
		void SetCurrentList(IList list) {
			UnsubscribeListEvents(currentList);
			currentList = list;
			SubscribeListEvents(currentList);
		}
		void SubscribeListEvents(object list) {
			if (list != null) {
				INotifyCollectionChanged coll = list as INotifyCollectionChanged;
				if (coll != null)
					coll.CollectionChanged += OnCollectionChanged;
			}
		}
		void UnsubscribeListEvents(object list) {
			if (list != null) {
				INotifyCollectionChanged coll = list as INotifyCollectionChanged;
				if (coll != null)
					coll.CollectionChanged -= OnCollectionChanged;
			}
		}
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateActualItemsSelection();
		}
	}
}

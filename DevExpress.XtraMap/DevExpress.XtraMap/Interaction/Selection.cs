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
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System.Windows.Forms;
namespace DevExpress.XtraMap {
	public class SelectedItemCollection : NotificationCollection<object> {
		MapItemsLayerBase layer;
		public SelectedItemCollection(MapItemsLayerBase layer) {
			this.layer = layer;
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			Select(value);
		}
		protected override void OnRemoveComplete(int index, object value) {
			Unselect(value);
			base.OnRemoveComplete(index, value);
		}
		protected override bool OnClear() {
			int count = Count;
			for(int i = 0; i < count; i++)
				Unselect(this[i]);
			return base.OnClear();
		}
		protected virtual void Select(object item) {
			UpdateSelection(item, true);
		}
		protected virtual void Unselect(object item) {
			UpdateSelection(item, false);
		}
		protected void UpdateSelection(object item, bool selected) {
			IInteractiveElement element = item as IInteractiveElement;
			if(element != null)
				SetElementSelection(element, selected);
			else {
				element = FindIInteractiveElement(item);
				SetElementSelection(element, selected);
			}
		}
		protected void SetElementSelection(IInteractiveElement element, bool selected) {
			if(element != null && element.IsSelected != selected) {
				element.IsSelected = selected;
			}
		}
		IInteractiveElement FindIInteractiveElement(object sourceObject) {
			if (sourceObject == null) return null;
			ILayerDataManagerProvider provider = (ILayerDataManagerProvider)layer;
			return provider != null ? provider.DataManager.GetMapItemBySourceObject(sourceObject) as IInteractiveElement : null;
		}
		internal void ApplySelection() {
			int count = Count;
			for(int i = 0; i < count; i++)
				Select(this[i]);
		}
		internal void ClearSelection() {
			int count = Count;
			for(int i = 0; i < count; i++)
				Unselect(this[i]);
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public enum SelectionAction { None, Clear, Add, Remove, ReplaceAll }
	public class SelectedItemsController {
		delegate void ItemsAction(object[] items);
		MapItemsLayerBase layer;
		bool shouldProcessEvents = true;
		public SelectedItemsController(MapItemsLayerBase layer) {
			this.layer = layer;
		}
		protected SelectedItemCollection SelectedItems { get { return layer.SelectedItems; } }
		public bool ShouldProcessEvents {
			get { return shouldProcessEvents; }
			set { shouldProcessEvents = value; }
		}
		void ProcessItemsAction(ItemsAction action, object[] items) {
			if (ShouldProcessEvents)
				if (!NotifySelectionChanging(items))
					return;
			action(items);
			if (ShouldProcessEvents)
				NotifySelectionChanged();
		}
		internal void InternalSetSelectedItems(IEnumerable<object> items) {
			SelectedItems.BeginUpdate();
			try {
				SelectedItems.Clear();
				foreach(object item in items)
					if(item != null)
						SelectedItems.Add(item);
			} finally {
				SelectedItems.EndUpdate();
			}
		}
		void AddSelectedItemCore(object[] items) {
			SelectedItems.AddRange(items);
		}
		void RemoveSelectedItemCore(object[] items) {
			foreach(object obj in items)
				SelectedItems.Remove(obj);
		}
		void ClearSelectedItems(object[] items) {
			InternalClearSelectedItems();
		}
		void SetSelectedItemsCore(object[] items) {
			if(items != null)
				InternalSetSelectedItems(items);
		}
		public bool IsItemSelected(object item) {
			return SelectedItems != null ? SelectedItems.Contains(item) : false;
		}
		public void SetSelectedItem(object item) {
			ProcessItemsAction(SetSelectedItemsCore, new object[] { item });
		}
		public void AddSelectedItem(object item) {
			ProcessItemsAction(AddSelectedItemCore, new object[] { item });
		}
		public void RemoveSelectedItem(object item) {
			ProcessItemsAction(RemoveSelectedItemCore, new object[] { item });
		}
		public void ClearSelectedItems() {
			if(layer.SelectedItems.Count == 0)
				return;
			ProcessItemsAction(ClearSelectedItems, new object[] { });
		}
		internal void InternalClearSelectedItems() {
			SelectedItems.Clear();
		}
		void NotifySelectionChanged() {
			layer.RaiseItemSelectionChanged();
		}
		bool NotifySelectionChanging(IEnumerable<object> newValue) {
			return layer.RaiseItemSelectionChanging(newValue);
		}
		internal void ApplySelection() {
			SelectedItems.ApplySelection();
		}
		internal void ClearSelection() {
			SelectedItems.ClearSelection();
		}
	}
	public static class SelectionHelper {
		public static SelectionAction CalculateSelectionAction(ElementSelectionMode selectionMode, Keys keyModifiers, bool isItemSelected, bool layerEnabledSelection) {
			if(!layerEnabledSelection)
				return SelectionAction.Clear;
			switch(selectionMode) {
				case ElementSelectionMode.Single:
					return isItemSelected ? SelectionAction.None : SelectionAction.ReplaceAll;
				case ElementSelectionMode.Multiple:
					return isItemSelected ? SelectionAction.Remove : SelectionAction.Add;
				case ElementSelectionMode.Extended: {
						if(keyModifiers == Keys.Shift)
							return isItemSelected ? SelectionAction.None: SelectionAction.Add;
						if(keyModifiers == Keys.Control)
							return isItemSelected ? SelectionAction.Remove : SelectionAction.Add;
						return isItemSelected ? SelectionAction.None : SelectionAction.ReplaceAll;
					}
			}
			return SelectionAction.None;
		}
	}
}

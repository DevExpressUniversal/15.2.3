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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Mvvm.UI.Interactivity;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using System.Windows.Controls.Primitives;
using System.Collections;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Diagram {
	public class ListBoxMenuBehavior : Behavior<ListBox> {
		MouseButton previewClickedButton;
		Locker selectionChangeLock = new Locker();
		bool popupMenuEnable = false;
		public ListBoxMenuBehavior() {
			selectionChangeLock.Unlock();
		}
		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.Register("ViewModel", typeof(DiagramToolboxControlViewModel), typeof(ListBoxMenuBehavior),
				new PropertyMetadata(null, (d, e) => ((ListBoxMenuBehavior)d).OnViewModelChanged(e)));
		public static readonly DependencyProperty IsCompactProperty =
			DependencyProperty.Register("IsCompact", typeof(bool), typeof(ListBoxMenuBehavior), new PropertyMetadata(false, (d, e) => ((ListBoxMenuBehavior)d).UpdateListBoxCollection()));
		public static readonly DependencyProperty MenuViewModeProperty =
			DependencyProperty.Register("MenuViewMode", typeof(ToolboxMenuViewMode), typeof(ListBoxMenuBehavior), new PropertyMetadata(ToolboxMenuViewMode.ViewSelector));
		public static readonly DependencyProperty PopupMenuProperty =
			DependencyProperty.Register("PopupMenu", typeof(PopupMenu), typeof(ListBoxMenuBehavior), new PropertyMetadata(null, (d, e) => ((ListBoxMenuBehavior)d).OnContextMenuChanged(e)));
		public DiagramToolboxControlViewModel ViewModel {
			get { return (DiagramToolboxControlViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}
		public bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { SetValue(IsCompactProperty, value); }
		}
		public ToolboxMenuViewMode MenuViewMode {
			get { return (ToolboxMenuViewMode)GetValue(MenuViewModeProperty); }
			set { SetValue(MenuViewModeProperty, value); }
		}
		public PopupMenu PopupMenu {
			get { return (PopupMenu)GetValue(PopupMenuProperty); }
			set { SetValue(PopupMenuProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.Items.Add(new MenuStencilInfo(DiagramControlLocalizer.GetString(DiagramControlStringId.More_Shapes_Name), "MoreShapes"));
			AssociatedObject.SelectionMode = SelectionMode.Multiple;
			AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
			AssociatedObject.PreviewMouseDown += PreviewMouseDown;
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
			AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
			AssociatedObject.PreviewMouseDown -= PreviewMouseDown;
		}
		void OnViewModelChanged(DependencyPropertyChangedEventArgs e) {
			if (e.OldValue != null) {
				((DiagramToolboxControlViewModel)e.OldValue).PropertyChanged -= ViewModel_PropertyChanged;
				((DiagramToolboxControlViewModel)e.OldValue).CheckedStencils.CollectionChanged -= CheckedStencils_CollectionChanged;
			}
			if (e.NewValue != null) {
				((DiagramToolboxControlViewModel)e.NewValue).PropertyChanged += ViewModel_PropertyChanged;
				((DiagramToolboxControlViewModel)e.NewValue).CheckedStencils.CollectionChanged += CheckedStencils_CollectionChanged;
				CheckedStencils_CollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}
		void OnContextMenuChanged(DependencyPropertyChangedEventArgs e) {
			if (e.OldValue != null) {
				((PopupMenu)e.NewValue).Opening -= MenuOpening;
				((PopupMenu)e.OldValue).Closed -= MenuClosed;
			}
			if (e.NewValue != null) {
				((PopupMenu)e.NewValue).Opening += MenuOpening;
				((PopupMenu)e.NewValue).Closed += MenuClosed;
			}
		}
		void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (!IsCompact && e.PropertyName == "SelectedStencil") {
				selectionChangeLock.Lock();
				RemoveSelectedMenuItems(null);
				SelectMenuItem();
				selectionChangeLock.Unlock();
			}
		}
		void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (selectionChangeLock.IsLocked || e.AddedItems.Count > 1 || e.RemovedItems.Count > 1) return;
			string removeItemId = (e.RemovedItems.Count != 0) ? ((MenuStencilInfo)e.RemovedItems[0]).Id : null;
			string addedItemId = (e.AddedItems.Count != 0) ? ((MenuStencilInfo)e.AddedItems[0]).Id : null;
			if (removeItemId == "MoreShapes") {
				ListBoxItem removedItem = AssociatedObject.ItemContainerGenerator.ContainerFromItem((MenuStencilInfo)e.RemovedItems[0]) as ListBoxItem;
				if (removedItem != null && removedItem.IsFocused)
					AssociatedObject.Focus();
				return;
			}
			if (addedItemId == "MoreShapes") {
				MenuViewMode = (IsCompact) ? ToolboxMenuViewMode.StensilsCompactSelector : ToolboxMenuViewMode.StencilsSelector;
				OpenPopupMenu(PlacementMode.Relative, AssociatedObject.ActualHeight * (AssociatedObject.Items.Count - 1) / AssociatedObject.Items.Count, AssociatedObject.ActualWidth);
				return;
			}
			MenuViewMode = (removeItemId == "QuickShapes" || addedItemId == "QuickShapes") ? ToolboxMenuViewMode.ViewSelector : ToolboxMenuViewMode.ViewAndOrderSelector;
			SelectStencil(removeItemId != null ? removeItemId : addedItemId);
			if (previewClickedButton == MouseButton.Right) {
				OpenPopupMenu(PlacementMode.MousePoint);
				RemoveMenuItems(new List<string>() { "MoreShapes" }, AssociatedObject.SelectedItems);
			} else
				popupMenuEnable = false;
		}
		void CheckedStencils_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateListBoxCollection();
		}
		void PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			previewClickedButton = e.ChangedButton;
		}
		void MenuOpening(object sender, CancelEventArgs e) {
			if (!popupMenuEnable)
				e.Cancel = true;
			popupMenuEnable = false;
		}
		void MenuClosed(object sender, EventArgs e) {
			RemoveMenuItems(new List<string>() { "MoreShapes" }, AssociatedObject.SelectedItems);
		}
		void UpdateListBoxCollection() {
			if (AssociatedObject != null) {
				selectionChangeLock.Lock();
				RemoveSelectedMenuItems(null);
				List<string> ids = new List<string>();
				foreach (var item in AssociatedObject.Items) {
					if (((MenuStencilInfo)item).Id != "MoreShapes")
						ids.Add(((MenuStencilInfo)item).Id);
				}
				RemoveMenuItems(ids, AssociatedObject.Items);
				if (!IsCompact) {
					foreach (var item in ViewModel.CheckedStencils.Reverse())
						AssociatedObject.Items.Insert(0, new MenuStencilInfo(item.Name, item.Id));
					SelectMenuItem();
				}
				selectionChangeLock.Unlock();
			}
		}
		void SelectMenuItem() {
			if (ViewModel.SelectedStencil != null && AssociatedObject != null) {
				foreach (var item in AssociatedObject.Items)
					if (((MenuStencilInfo)item).Id == ViewModel.SelectedStencil.Id) {
						AssociatedObject.SelectedItems.Add(item);
						return;
					}
			}
		}
		void SelectStencil(string id) {
			if (id == "MoreShapes") return;
			selectionChangeLock.Lock();
			RemoveSelectedMenuItems(id);
			foreach (var item in AssociatedObject.Items)
				if (((MenuStencilInfo)item).Id == id) {
					AssociatedObject.SelectedItems.Add(item);
					break;
				}
			ViewModel.SelectedStencil = ViewModel.CheckedStencils.Single(x => x.Id == id);
			selectionChangeLock.Unlock();
		}
		void RemoveSelectedMenuItems(string savedItemId) {
			if(AssociatedObject == null)
				return;
			List<string> ids = new List<string>();
			foreach (var item in AssociatedObject.SelectedItems) {
				string itemId = ((MenuStencilInfo)item).Id;
				if (itemId != "MoreShapes") {
					if (string.IsNullOrEmpty(savedItemId))
						ids.Add(itemId);
					else
						if (itemId != savedItemId)
						ids.Add(itemId);
				}
			}
			RemoveMenuItems(ids, AssociatedObject.SelectedItems);
		}
		void RemoveMenuItems(List<string> idsCollection, IList itemsCollection) {
			foreach (string id in idsCollection) {
				object item = null;
				foreach (var selectedItem in itemsCollection)
					if (((MenuStencilInfo)selectedItem).Id == id)
						item = selectedItem;
				if (item != null)
					itemsCollection.Remove(item);
			}
		}
		void OpenPopupMenu(PlacementMode mode, double verticalOffset = 0, double horizontalOffset = 0) {
			PopupMenu.Placement = mode;
			PopupMenu.VerticalOffset = verticalOffset;
			PopupMenu.HorizontalOffset = horizontalOffset;
			popupMenuEnable = true;
			PopupMenu.IsOpen = true;
		}
	}
	public class MenuStencilInfo : ObservableObject {
		string id;
		string name;
		public string Id {
			get { return id; }
			set { SetPropertyValue("Id", ref id, value); }
		}
		public string Name {
			get { return name; }
			set { SetPropertyValue("Name", ref name, value); }
		}
		public MenuStencilInfo(string name, string id) {
			Name = name;
			Id = id;
		}
	}
}

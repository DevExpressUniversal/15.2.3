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
using System.Linq;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Controls.Design.Features.TileNavPaneDesigner.Commands;
using DevExpress.Xpf.Navigation;
using Microsoft.Windows.Design.Model;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.Windows;
namespace DevExpress.Xpf.Controls.Design.Features.TileNavPaneDesigner.ViewModel {
	public abstract class TileNavPaneBarViewModelBase<T> : ViewModelBase where T: NavElementBase {
		public TileNavPaneBarViewModelBase() {
			Items = new ObservableCollection<T>();
		}
		private ObservableCollection<T> items;
		public ObservableCollection<T> Items {
			get { return items; }
			set { SetProperty(ref items, value, () => Items); }
		}
		private object selectedItem;
		public object SelectedItem {
			get { return selectedItem; }
			set {
				SetProperty(ref selectedItem, value, () => SelectedItem); }
		}
		private int selectedIndex = -1;
		public int SelectedIndex {
			get { return selectedIndex; }
			set {
				var oldValue = selectedIndex;
				SetProperty(ref selectedIndex, value, () => SelectedIndex);
				if (oldValue != value)
					OnSelectedIndexChanged();
			}
		}
		public virtual void OnSelectedIndexChanged() {}
		public abstract ICommand AddItemCommand { get; }
		public abstract ICommand RemoveItemCommand { get; }
		public abstract ICommand MoveLeftCommand { get; }
		public abstract ICommand MoveRightCommand { get; }
		public abstract ICommand ChangePropertyCommand { get; }
		public abstract string Name { get; }
	}
	public class CategoriesViewModel : TileNavPaneBarViewModelBase<TileNavCategory> {
		private ItemsViewModel itemsVM;
		private IModelItem modelItem;
		public ItemsViewModel ItemsVM {
			get { return itemsVM; }
			private set { SetProperty(ref itemsVM, value, () => ItemsVM); }
		}
		public CategoriesViewModel(IModelItem modelItem)
		{
			this.modelItem = modelItem;
			Items = modelItem.View<TileNavPane>().Categories;
			ItemsVM = new ItemsViewModel();
			SelectedIndex = -1;
		}
		public override void OnSelectedIndexChanged() {
			ItemsVM = SelectedIndex >= 0 ? new ItemsViewModel(modelItem.At("Categories", SelectedIndex)) : null;
		}
		public override ICommand AddItemCommand {
			get { return modelItem != null ? new AddNewTileNavCategoryCommand(modelItem) : null; }
		}
		public override ICommand RemoveItemCommand {
			get { return modelItem != null ? new RemoveTileNavCategoryCommand(modelItem) : null; }
		}
		public override ICommand MoveLeftCommand {
			get { return modelItem != null ? new MoveTileNavCategoryLeftCommand(modelItem) { AfterExecute = param => SelectedIndex = param - 1 } : null; }
		}
		public override ICommand MoveRightCommand {
			get { return modelItem != null ? new MoveTileNavCategoryRightCommand(modelItem) { AfterExecute = param => SelectedIndex = param + 1 } : null; }
		}
		public override ICommand ChangePropertyCommand {
			get { return modelItem != null ? new ChangeCategoryPropertyCommand(modelItem) : null; }
		}
		public override string Name {
			get { return "Categories"; }
		}
	}
	public class ItemsViewModel : TileNavPaneBarViewModelBase<TileNavItem> {
		private SubItemsViewModel subItemsVM;
		private readonly IModelItem modelItem;
		public SubItemsViewModel SubItemsVM {
			get { return subItemsVM; }
			private set { SetProperty(ref subItemsVM, value, () => SubItemsVM); }
		}
		public ItemsViewModel() {
			SubItemsVM = new SubItemsViewModel();
		}
		public ItemsViewModel(IModelItem modelItem)
		{
			this.modelItem = modelItem;
			Items = modelItem.View<TileNavCategory>().Items;
			SubItemsVM = new SubItemsViewModel();
			SelectedIndex = -1;
		}
		public override void OnSelectedIndexChanged() {
			SubItemsVM = SelectedIndex >= 0 ? new SubItemsViewModel(modelItem.At("Items", SelectedIndex)) : null;
		}
		public override ICommand AddItemCommand {
			get { return modelItem != null ? new AddNewTileNavItemCommand(modelItem) : null; }
		}
		public override ICommand RemoveItemCommand {
			get { return modelItem != null ? new RemoveTileNavItemCommand(modelItem) : null; }
		}
		public override ICommand MoveLeftCommand {
			get { return modelItem != null ? new MoveTileNavItemLeftCommand(modelItem) { AfterExecute = param => SelectedIndex = param - 1 } : null; }
		}
		public override ICommand MoveRightCommand {
			get { return modelItem != null ? new MoveTileNavItemRightCommand(modelItem) { AfterExecute = param => SelectedIndex = param + 1 } : null; }
		}
		public override ICommand ChangePropertyCommand {
			get { return modelItem != null ? new ChangeItemPropertyCommand(modelItem) : null; }
		}
		public override string Name {
			get { return "Items"; }
		}
	}
	public class SubItemsViewModel : TileNavPaneBarViewModelBase<TileNavSubItem> {
		private readonly IModelItem modelItem;
		public SubItemsViewModel() { }
		public SubItemsViewModel(IModelItem modelItem) {
			this.modelItem = modelItem;
			Items = modelItem.View<TileNavItem>().Items;
		}
		public override ICommand AddItemCommand {
			get { return modelItem != null ? new AddNewTileNavSubItemCommand(modelItem) : null; }
		}
		public override ICommand RemoveItemCommand {
			get { return modelItem != null ? new RemoveTileNavSubItemCommand(modelItem) : null; }
		}
		public override ICommand MoveLeftCommand {
			get { return modelItem != null ? new MoveTileNavSubItemLeftCommand(modelItem) { AfterExecute = param => SelectedIndex = param - 1 } : null; }
		}
		public override ICommand MoveRightCommand {
			get { return modelItem != null ? new MoveTileNavSubItemRightCommand(modelItem) { AfterExecute = param => SelectedIndex = param + 1 } : null; }
		}
		public override ICommand ChangePropertyCommand {
			get { return modelItem != null ? new ChangeSubItemPropertyCommand(modelItem) : null; }
		}
		public override string Name {
			get { return "SubItems"; }
		}
	}
}

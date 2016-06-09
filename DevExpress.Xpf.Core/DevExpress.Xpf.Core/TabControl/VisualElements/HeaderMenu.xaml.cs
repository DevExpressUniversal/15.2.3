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

using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Core.Native {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class DXTabControlHeaderMenuTemplateSelectorBase : DataTemplateSelector {
		public DataTemplate ItemTemplate { get; set; }
		[Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
		public DataTemplate OldTemplate { get; set; }
		public DataTemplate SeparatorTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
#pragma warning disable
			DXTabItem tab = item as DXTabItem;
			if(tab == null) return SeparatorTemplate;
			if(tab.HeaderMenuIcon != null && tab.HeaderMenuGlyph == null)
				return OldTemplate;
			return ItemTemplate;
#pragma warning restore
		}
	}
	public partial class DXTabControlHeaderMenuTemplateSelector : DXTabControlHeaderMenuTemplateSelectorBase {
		public DXTabControlHeaderMenuTemplateSelector() {
			InitializeComponent();
		}
	}
	[POCOViewModel]
	public class DXTabControlHeaderMenuInfo {
		public virtual bool IsMenuOpen { get; set; }
		public virtual ObservableCollection<DependencyObject> Items { get; protected set; }
		public virtual bool HasItems { get; protected set; }
		protected bool ShowHeaderMenu { get { return View != null && View.ShowHeaderMenu; } }
		protected bool ShowVisibleTabItemsInHeaderMenu { get { return View != null && View.ShowVisibleTabItemsInHeaderMenu; } }
		protected bool ShowHiddenItemsInHeaderMenu { get { return View != null && View.ShowHiddenTabItemsInHeaderMenu; } }
		protected bool ShowDisabledTabItemsInHeaderMenu { get { return View != null && View.ShowDisabledTabItemsInHeaderMenu; } }
		protected readonly DXTabControl Owner;
		protected TabControlViewBase View { get { return Owner.View; } }
		protected IEnumerable<DXTabItem> TabItems { get { return GetTabItems(); } }
		public DXTabControlHeaderMenuInfo(DXTabControl owner) {
			System.Diagnostics.Contracts.Contract.Requires(owner != null);
			Owner = owner;
			Items = new ObservableCollection<DependencyObject>();
			UpdateHasItems();
		}
		public void SelectTab(DXTabItem tabItem) {
			Owner.ShowTabItem(tabItem);
		}
		public void UpdateHasItems() {
			if(!ShowHeaderMenu) {
				HasItems = false;
				return;
			}
			bool hasCustomItems = Owner.IsInitialized && Owner.View.HeaderMenuCustomizations.Count > 0;
			HasItems = hasCustomItems || GetItems().Count() > 0;
		}
		protected virtual void GenerateItems() {
			Items.Clear();
			IEnumerable<DependencyObject> result = GetItems();
			foreach (var item in result) {
				Items.Add(item);
			}
		}
		protected virtual IEnumerable<DependencyObject> GetItems() {
			IEnumerable<DXTabItem> items = TabItems.Where(tabItem => tabItem.IsEnabled || ShowDisabledTabItemsInHeaderMenu);
			IEnumerable<DXTabItem> hiddenItems = items.Where(tabItem => tabItem.Visibility == Visibility.Collapsed);
			IEnumerable<DependencyObject> result = items.Where(tabItem => ShowVisibleTabItemsInHeaderMenu && tabItem.Visibility == Visibility.Visible);
			if (!ShowVisibleTabItemsInHeaderMenu)
				result = TabItems.Where(tabItem => !tabItem.IsEnabled && ShowDisabledTabItemsInHeaderMenu);
			if (ShowHiddenItemsInHeaderMenu && hiddenItems.Count() > 0) {
				if (result.Count() > 0)
					result = result.Union(new[] { new Separator() });
				result = result.Union(hiddenItems);
			}
			return result;
		}
		protected void OnIsMenuOpenChanged() {
			if(IsMenuOpen) {
				AddCustomItems();
				GenerateItems();
			} else {
				Clear();
			}
		}
		void AddCustomItems() {
			PopupMenu menu = Owner.GetLayoutChild("PART_HeaderMenuPopup") as PopupMenu;
			if(menu == null) return;
			Owner.View.HeaderMenuCustomizationController.Execute(menu);
		}
		void Clear() {
			Items.Clear();
			PopupMenu menu = Owner.GetLayoutChild("PART_HeaderMenuPopup") as PopupMenu;
			if(menu == null) return;
			var itemsToRemove = menu.Items.OfType<FrameworkContentElement>().Where(x => x.Name != "PART_HeaderMenuPopupItemsContainer").ToList();
			foreach(var item in itemsToRemove)
				menu.Items.Remove((IBarItem)item);
		}
		IEnumerable<DXTabItem> GetTabItems() {
			return Owner.Items.Cast<object>().Select(item => Owner.GetTabItem(item)).Where(item => item != null && item.VisibleInHeaderMenu);
		}
	}
}

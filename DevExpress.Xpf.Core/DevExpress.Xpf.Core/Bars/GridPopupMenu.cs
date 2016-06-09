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
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using DevExpress.Xpf.Utils.Themes;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Bars {
	public abstract class MenuInfoBase {
		CustomizablePopupMenuBase menu;
		protected MenuInfoBase(CustomizablePopupMenuBase menu) {
			this.menu = menu;
		}
		public CustomizablePopupMenuBase Menu { get { return menu; } }
		public ILogicalOwner Owner { get { return menu.LogicalOwner; } }
		public IInputElement TargetElement { get; set; }
		public virtual bool Initialize(IInputElement value) {			
			TargetElement = value;
			if(!CanCreateItems)
				return false;
			CreateItems();
			return true;
		}
		public virtual void Uninitialize() {
			Menu.ClearItems();
			TargetElement = null;
		}
		protected abstract void CreateItems();
		public abstract bool CanCreateItems { get; }
		public abstract BarManagerMenuController MenuController { get; }
		protected internal virtual void ExecuteMenuController() {
			MenuController.Execute();
		}
	}
	public abstract class CustomizablePopupMenuBase : PopupMenu {
		static CustomizablePopupMenuBase() {
			BarNameScope.IsScopeOwnerProperty.OverrideMetadata(typeof(CustomizablePopupMenuBase), new FrameworkPropertyMetadata(true));
		}
		protected internal override bool AttachToPlacementTargetWhenClosed { get { return false; } }
		public override DependencyObject Owner { get { return (DependencyObject)ownerReference.Target; } }
		public ILogicalOwner LogicalOwner { get { return Owner as ILogicalOwner; } }
		public MenuInfoBase MenuInfo { get; private set; }
		public BarManagerActionCollection Customizations { get { return MenuController.ActionContainer.Actions; } }
		protected virtual bool ShouldClearItemsOnClose { get { return false; } }
		public string GetItemName() { return string.Empty; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BarButtonItem CreateBarButtonItem(string name, object content, bool beginGroup, ImageSource glyph, BarItemLinkCollection links) {
			BarButtonItem barItem = new BarButtonItem() { Name = name, Content = content, Glyph = glyph };
			AddItemCore(barItem, beginGroup, links);
			return barItem;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BarSplitButtonItem CreateBarSplitButtonItem(string name, object content, bool beginGroup, ImageSource glyph, BarItemLinkCollection links) {
			BarSplitButtonItem barItem = new BarSplitButtonItem() { Name = name, Content = content, Glyph = glyph };
			AddItemCore(barItem, beginGroup, links);
			return barItem;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BarCheckItem CreateBarCheckItem(string name, object content, bool? isChecked, bool beginGroup, ImageSource glyph, BarItemLinkCollection links) {
			BarCheckItem barItem = new BarCheckItem() { Name = name, Content = content, IsChecked = isChecked, Glyph = glyph };
			AddItemCore(barItem, beginGroup, links);
			return barItem;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BarSubItem CreateBarSubItem(string name, object content, bool beginGroup, ImageSource glyph, BarItemLinkCollection links) {
			BarSubItem barItem = new BarSubItem() { Name = name, Content = content, Glyph = glyph };
			AddItemCore(barItem, beginGroup, links);
			return barItem;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ProcessCustomizationActions() {
			MenuController.Execute();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ForceCreateItems() {
			if(CreateItems()) ProcessCustomizationActions();
			UpdateBarItemsIsEnabled();
			BarDragProvider.RemoveUnnesessarySeparators(ItemLinks);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool CreateItems() {
			UninitializeMenuInfo();
			if(PlacementTarget == null) return false;
			MenuInfo = CreateMenuInfo(PlacementTarget);
			DataContext = MenuInfo;
			if(MenuInfo == null) return false;
			if(MenuInfo != null && !MenuInfo.Initialize(PlacementTarget))
				return false;
			if(MenuInfo.MenuController != null) {
				MenuInfo.ExecuteMenuController();
			}
			return true;
		}
		protected void UninitializeMenuInfo() {
			if(MenuInfo != null) {
				UpdateMenuInfoAttachedProperty(null, null);
				MenuInfo.Uninitialize();
				MenuInfo = null;
				DataContext = null;
			}
		}
		public void ClearItems() {
			Items.Clear();
			ItemLinks.Clear();			
		}
		public ReadOnlyCollection<BarItem> GetItems(BarItemLinkCollection itemLinks = null) {
			List<BarItem> list = new List<BarItem>();
			if(itemLinks == null) itemLinks = ItemLinks;
			foreach(BarItemLinkBase link in itemLinks) {
				if(!(link is BarItemLink)) continue;
				BarItem item = ((BarItemLink)link).Item;
				if(item == null) continue;
				if(itemLinks.Holder.Items.Contains(item) || itemLinks.Holder.Items.Contains(link))
					list.Add(item);
				if(item is ILinksHolder) {
					ReadOnlyCollection<BarItem> subItems = GetItems(((ILinksHolder)item).Links);
					foreach(BarItem subItem in subItems)
						list.Add(subItem);
				}
			}
			return new ReadOnlyCollection<BarItem>(list);
		}
		public CustomizablePopupMenuBase(ILogicalOwner owner)
			: base() {
			this.ownerReference = new WeakReference(owner);
			Dispatcher.BeginInvoke(new Action(() => { var lOwner = Owner as ILogicalOwner; if (lOwner != null) lOwner.AddChild(this); }));
			MenuController = new BarManagerMenuController(this);
			this.ItemClickBehaviour = PopupItemClickBehaviour.CloseCurrentBranch;
		}
		public void Reset() {
			IsOpen = false;
			UpdateMenuInfoAttachedProperty(null, null);
			if(MenuInfo != null) MenuInfo.Uninitialize();
			MenuInfo = null;
			DataContext = null;			
		}
		public void Init() {			
		}
		public virtual void Destroy() {
			Reset();
			var lOwner = Owner as ILogicalOwner;
			if (lOwner != null) {
				lOwner.RemoveChild(this);
			}
			ownerReference = new WeakReference(null);
		}
		public BarItem GetBarItemByName(string barItemName) {
			BarItemLinkBase link = BarManagerHelper.GetLinkByItemName(barItemName, this.ItemLinks);
			return link is BarItemLink ? ((BarItemLink)link).Item : null;
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, new SingleLogicalChildEnumerator(MenuController.If(x => x.Parent == this))); }
		}
		protected internal BarManagerMenuController MenuController { 
			get { return menuController; }
			internal set {				
				if(menuController == value) return;
				var oldValue = menuController;
				menuController = value;
				oldValue.Do(RemoveLogicalChild);
				if (value != null && value.Parent == null)
					value.Do(AddLogicalChild);
			} 
		}
		protected abstract MenuInfoBase CreateMenuInfo(UIElement placementTarget);		
		protected virtual void UpdateMenuInfoAttachedProperty(BarManager manager, MenuInfoBase menuInfo) {
		}
		protected void AddItemCore(BarItem barItem, bool isBeginGroup, BarItemLinkCollection links) {			
			barItem.IsPrivate = true;
			var items = links.Holder.Items;			
			if(isBeginGroup)
				items.Add(new BarItemLinkSeparator());
			items.Add(barItem);
		}
		protected override bool RaiseOpening() {
			bool result = base.RaiseOpening();
			if(result) {
				Customizations.Clear();
				this.DataContext = null;
				LockItemLinks();
				if(!CreateItems()) {
					UnlockItemLinks();
					return false;
				}
				result = RaiseShowMenu();
				if(result) ProcessCustomizationActions();
				UnlockItemLinks();
				UpdateMenuInfoAttachedProperty(null, MenuInfo);
				BarDragProvider.RemoveUnnesessarySeparators(ItemLinks);
				BarItemLink.UpdateSeparatorsVisibility(this, true);
				ForceClose();				
			}
			return result;
		}		
		protected virtual bool RaiseShowMenu() {
			return true;
		}
		protected override BarItemLinkCollection GetNewItemLinksCollection() {
			return new CustomizablePopupMenuItemLinkCollection(this);
		}
		protected override void OnIsOpenChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsOpenChanged(e);
			if(!((bool)e.NewValue)) {
				if(ShouldClearItemsOnClose) {
					Dispatcher.BeginInvoke(new Action(() => { UninitializeMenuInfo(); }), System.Windows.Threading.DispatcherPriority.Render);
				}				
			} 
		}
		protected override void OnIsOpenPropertyCoerced(bool value) {
			base.OnIsOpenPropertyCoerced(value);
			if(!value) return;
			UpdateBarItemsIsEnabled();
		}
		protected override bool HasVisibleItems() {
			foreach(BarItemLinkBase link in ItemLinks) {
				if(link.ActualIsVisible) return true;
			}
			return false;
		}
		internal void LockItemLinks() {
			((CustomizablePopupMenuItemLinkCollection)ItemLinks).LockUpdateLinkControl();
		}
		internal void UnlockItemLinks() {
			((CustomizablePopupMenuItemLinkCollection)ItemLinks).UnlockUpdateLinkControl();
		}
		void UpdateBarItemsIsEnabled() {
			ReadOnlyCollection<BarItem> items = GetItems();
			foreach(BarItem item in items) item.ForceUpdateCanExecute();
		}
		WeakReference ownerReference;
		BarManagerMenuController menuController = null;
		class CustomizablePopupMenuItemLinkCollection : BarItemLinkCollection {			
			public CustomizablePopupMenuItemLinkCollection(ILinksHolder holder)
				: base(holder) {
			}
			public void LockUpdateLinkControl() {
				lockUpdateLinkControl = true;
			}
			public void UnlockUpdateLinkControl() {
				lockUpdateLinkControl = false;
				foreach(BarItemLinkBase link in Items)
					link.lockUpdateLinkControl = false;
			}
			protected override void InsertItem(int index, BarItemLinkBase itemLink) {
				itemLink.lockUpdateLinkControl = lockUpdateLinkControl;
				base.InsertItem(index, itemLink);
			}
			protected override void SetItem(int index, BarItemLinkBase item) {
				Items[index].lockUpdateLinkControl = false;
				item.lockUpdateLinkControl = lockUpdateLinkControl;
				base.SetItem(index, item);
			}
			protected override void RemoveItem(int index) {
				Items[index].lockUpdateLinkControl = false;
				base.RemoveItem(index);
			}
			protected override void ClearItems() {
				foreach(BarItemLinkBase link in Items)
					link.lockUpdateLinkControl = false;
				base.ClearItems();
			}
			bool lockUpdateLinkControl = false;
		}		
	}
	public abstract class GridPopupMenuBase : CustomizablePopupMenuBase {
		#region static
		public static readonly DependencyProperty GridMenuInfoProperty;
		static GridPopupMenuBase() {
			Type ownerType = typeof(GridPopupMenuBase);
			GridMenuInfoProperty = DependencyPropertyManager.RegisterAttached("GridMenuInfo", typeof(GridMenuInfoBase),
				ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		}
		public static void SetGridMenuInfo(DependencyObject element, GridMenuInfoBase value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(GridMenuInfoProperty, value);
		}
		public static GridMenuInfoBase GetGridMenuInfo(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (GridMenuInfoBase)element.GetValue(GridMenuInfoProperty);
		}
		#endregion
		public GridPopupMenuBase(ILogicalOwner owner)
			: base(owner) {
		}
		protected override void UpdateMenuInfoAttachedProperty(BarManager manager, MenuInfoBase menuInfo) {
			GridMenuInfoBase gridMenuInfo = menuInfo as GridMenuInfoBase;
			foreach(BarItem item in GetItems()) {
				SetGridMenuInfo(item, gridMenuInfo);
			}
		}
	}
	public abstract class GridMenuInfoBase : MenuInfoBase {
		protected GridMenuInfoBase(GridPopupMenuBase menu) : base(menu) {
		}
		protected virtual BarCheckItem CreateBarCheckItem(string name, string content, bool? isChecked, bool beginGroup, ImageSource image, ICommand command) {
			BarCheckItem item = Menu.CreateBarCheckItem(name, content, isChecked, beginGroup, image, Menu.ItemLinks);
			AssignCommand(item, command, TargetElement);
			return item;
		}
		protected virtual BarButtonItem CreateBarButtonItem(string name, string content, bool beginGroup, ImageSource image, ICommand command, object commandParameter = null) {
			return CreateBarButtonItem(Menu.ItemLinks, name, content, beginGroup, image, command, commandParameter);
		}
		protected virtual BarButtonItem CreateBarButtonItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image, ICommand command, object commandParameter = null) {
			BarButtonItem item = Menu.CreateBarButtonItem(name, content, beginGroup, image, links);
			AssignCommand(item, command, TargetElement, commandParameter);
			return item;
		}
		protected virtual BarSplitButtonItem CreateBarSplitButtonItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image) {
			return Menu.CreateBarSplitButtonItem(name, content, beginGroup, image, links);
		}
		protected virtual BarSubItem CreateBarSubItem(string name, string content, bool beginGroup, ImageSource image, ICommand command = null) {
			return CreateBarSubItem(Menu.ItemLinks, name, content, beginGroup, image);
		}
		protected virtual BarSubItem CreateBarSubItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image, ICommand command = null) {
			BarSubItem item = Menu.CreateBarSubItem(name, content, beginGroup, image, links);
			AssignCommand(item, command, TargetElement);
			return item;
		}
		void AssignCommand(BarItem item, ICommand command, IInputElement commandTarget, object commandParameter = null) {
			if(item != null) {
				item.CommandTarget = commandTarget;
				item.Command = command;
				if(commandParameter != null)
					item.CommandParameter = commandParameter;
			}
		}
		public static BarManagerMenuController CreateMenuController(PopupMenu menu) {
			return menu is CustomizablePopupMenuBase ? new BarManagerCustomizableMenuController((CustomizablePopupMenuBase)menu) : new BarManagerMenuController(menu);
		}
	}
	public class BarManagerCustomizableMenuController : BarManagerMenuController {
		public new CustomizablePopupMenuBase Menu {
			get { return (CustomizablePopupMenuBase)base.Menu; }
			set { base.Menu = (CustomizablePopupMenuBase)value; }
		}
		public BarManagerCustomizableMenuController(CustomizablePopupMenuBase menu)
			: base(menu) {
			DataContext = null;
			var ilo = Menu.LogicalOwner;
			if (ilo == null)
				return;
			ilo.AddChild(this);
		}
		BarManagerMenuController controller = null;
		protected override void OnBeforeExecute() {
			controller = Menu.MenuController;
			Menu.MenuController = this;
			base.OnBeforeExecute();
		}
		protected override void OnAfterExecute() {
			base.OnAfterExecute();
			Menu.MenuController = controller;
			controller = null;
		}
	}
}

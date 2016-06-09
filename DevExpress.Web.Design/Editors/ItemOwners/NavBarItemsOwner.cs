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
using DevExpress.Web;
using DevExpress.Web.Design;
namespace DevExpress.Web.Design {
	class NavBarItemsOwner : ItemsEditorOwner {
		public NavBarItemsOwner(object component, IServiceProvider provider)
			: base(component, "Groups", provider, ((ASPxNavBar)component).Groups) {
		}
		internal NavBarItemsOwner(Collection items)
			: base(null, null, null, items) {
		}
		protected ASPxNavBar NavBar { get { return Component as ASPxNavBar; } }
		protected override void FillItemTypes() {
			AddItemType(typeof(NavBarGroup), "Group", NavBarGroupImageResource);
			AddItemType(typeof(NavBarItem), "Item", NavBarItemImageResource);
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.AddItem,
				DesignEditorMenuRootItemActionType.InsertBefore,
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp,
				DesignEditorMenuRootItemActionType.MoveDown
			};
		}
		protected override List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp,
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.SelectAll
			};
		}
		public override bool CanMoveItem(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(!base.CanMoveItem(source, target, direction))
				return false;
			if(direction == InsertDirection.Inside)
				return CanAssociate(source, target);
			return source.GetType() == target.GetType();
		}
		protected bool CanAssociate(IDesignTimeCollectionItem child, IDesignTimeCollectionItem parent) {
			var childType = child != null ? child.GetType() : null;
			if(parent == null && childType == typeof(NavBarGroup))
				return true;
			var parentType = parent != null ? parent.GetType() : null;
			return parentType == typeof(NavBarGroup) && childType == typeof(NavBarItem);
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			return false;
		}
		protected override DesignEditorDescriptorItem CreateAddItemMenuItem() {
			var item = base.CreateAddItemMenuItem();
			item.EditorType = DesignEditorDescriptorItemType.Button;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateInsertBeforeMenuItem() {
			var item = base.CreateInsertBeforeMenuItem();
			item.EditorType = DesignEditorDescriptorItemType.Button;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateInsertChildMenuItem(bool isToolbar) {
			var item = base.CreateInsertChildMenuItem(isToolbar);
			item.EditorType = DesignEditorDescriptorItemType.Button;
			if(isToolbar)
				return item;
			var focusedItem = FocusedItemForAction;
			if(focusedItem == null) {
				item.Caption = "Add Group";
				item.EditorType = DesignEditorDescriptorItemType.Button;
				item.ItemType = FindDesignTimeItem(typeof(NavBarGroup));
			} else if(focusedItem.GetType() == typeof(NavBarGroup)) {
				item.Caption = "Add Item";
				item.EditorType = DesignEditorDescriptorItemType.Button;
				item.ItemType = FindDesignTimeItem(typeof(NavBarItem));
			}
			return item;
		}
		protected override void FillMenuItemResouceImages() {
			base.FillMenuItemResouceImages();
			AddResourceImage(AddChildItemImageResource);
		}
		Type defaultItemType;
		protected override void AddItemCore(IDesignTimeColumnAndEditorItem designTimeItem, IDesignTimeCollectionItem target, InsertDirection direction) {
			IDesignTimeCollectionItem parentItem;
			if(direction == InsertDirection.Inside)
				parentItem = target as IDesignTimeCollectionItem;
			else
				parentItem = target != null ? target.Parent : null;
			this.defaultItemType = parentItem == null ? typeof(NavBarGroup) : typeof(NavBarItem);
			base.AddItemCore(designTimeItem, target, direction);
		}
		protected override System.Collections.IList FindItemCollection(IDesignTimeCollectionItem item) {
			if(item is NavBarItem)
				return item.Parent != null ? item.Parent.Items : null;
			return base.FindItemCollection(item);			
		}
		public override Type GetDefaultItemType() {
			return this.defaultItemType;
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Groups";
		}
		protected internal override string GetItemPropertiesTabCaption() {
			if(FocusedItemForAction == null)
				return string.Empty;
			return string.Format("{0} Properties", FocusedItemForAction.GetType().Name);
		}
		public override List<string> GetViewDependedProperties() {
			var list = base.GetViewDependedProperties();
			list.AddRange(new string[] { 
				"Text", 
				"Caption", 
				"Visible", 
				"VisibleIndex"
			});
			return list;
		}
	}
}

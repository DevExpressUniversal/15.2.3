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

extern alias Platform;
using System;
using Microsoft.Windows.Design.Interaction;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Bars;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design;
using System.Reflection;
using System.Collections.Generic;
using System.Windows;
using System.Resources;
using System.Collections;
using System.Linq;
#if SILVERLIGHT
using AssemblyInfo = Platform::AssemblyInfo;
#endif
namespace DevExpress.Xpf.Core.Design {
	public class ContainerContextMenuProvider : PrimarySelectionContextMenuProvider {
		public MenuGroup AddBarItemGroup { get; set; }
		public MenuGroup AddEditItemGroup { get; set; }
		public MenuGroup AddBarItemLinkGroup { get; set; }
		protected AddBarItemMenuAction AddBarButtonItem { get; set; }
		protected AddBarItemMenuAction AddBarCheckItem { get; set; }
		protected AddBarItemMenuAction AddBarSplitButtonItem { get; set; }
		protected AddBarItemMenuAction AddBarSplitCheckItem { get; set; }
		protected AddBarItemMenuAction AddBarSubItem { get; set; }
		protected AddBarItemMenuAction AddBarStaticItem { get; set; }
		protected List<AddBarItemMenuAction> BarEditItemActions { get; set; }
		protected MenuAction DeleteAction { get; set; }
		public ContainerContextMenuProvider() {
			InitializeMenuGroups();
			InitializeMenuActions();
			AddItemsForGroup();
			AddGroups();
			UpdateItemStatus += OnUpdateItemStatus;
		}
		protected virtual void AddGroups() {
			AddBarItemGroup.Items.Add(AddEditItemGroup);
			Items.Add(AddBarItemGroup);
			Items.Add(AddBarItemLinkGroup);
#if !SL
			Items.Add(DeleteAction);
#endif
		}
		protected virtual void AddItemsForGroup() {
			AddBarItemGroup.Items.Add(AddBarButtonItem);
			AddBarItemGroup.Items.Add(AddBarCheckItem);
			AddBarItemGroup.Items.Add(AddBarSplitButtonItem);
			AddBarItemGroup.Items.Add(AddBarSplitCheckItem);
			AddBarItemGroup.Items.Add(AddBarSubItem);
			AddBarItemGroup.Items.Add(AddBarStaticItem);
			foreach(MenuAction item in BarEditItemActions) {
				AddEditItemGroup.Items.Add(item);
			}
		}
		protected virtual void InitializeAddBarItemLinkGroup(ModelItem barManager) {
			AddBarItemLinkGroup.Items.Clear();
			if(barManager == null) return;
			foreach(ModelItem item in BarManagerDesignTimeHelper.GetBarManagerItems(barManager)) {
				if(SkipAddingToAddBarItemLink(item)) continue;
				AddBarItemLinkMenuAction menuAction = new AddBarItemLinkMenuAction(item);
				menuAction.Execute += new EventHandler<MenuActionEventArgs>(OnAddBarItemLinkMenuActionExecute);
				menuAction.ImageUri = GetBarItemGlyphUri((BarItem)item.GetCurrentValue());
				AddBarItemLinkGroup.Items.Add(menuAction);
			}
			AddBarItemLinkGroup.HasDropDown = AddBarItemLinkGroup.Items.Count > 0;
		}
		protected virtual Uri GetBarItemGlyphUri(BarItem barItem) {
			string uri = string.Empty;
			uri = GetGlyphUri(barItem);
			if(string.IsNullOrEmpty(uri)) return null;
			string assemblyName = Application.ResourceAssembly.GetName().Name;
			ResourceManager resourceManager = new ResourceManager(assemblyName + ".g", Application.ResourceAssembly);
			ResourceSet rs = resourceManager.GetResourceSet(System.Globalization.CultureInfo.InvariantCulture, true, false);
			if(rs == null || rs.GetEnumerator() == null) return null;
			IDictionaryEnumerator enumerator = rs.GetEnumerator();
			while(enumerator.MoveNext()) {
				string resourceName = (string)enumerator.Key;
				if(uri.ToLower().EndsWith(resourceName.ToLower())) {
					uri = resourceName;
					break;
				}
			}
			uri = "pack://application:,,,/" + assemblyName + ";component/" + uri;
			return new Uri(uri, UriKind.Absolute);
		}
		protected virtual string GetGlyphUri(BarItem barItem) {
			var glyph = barItem.Glyph ?? barItem.LargeGlyph;
			if(glyph == null) return string.Empty;
			string result = string.Empty;
#if SL
			if(glyph is Platform::System.Windows.Media.Imaging.BitmapImage)
				result = ((Platform::System.Windows.Media.Imaging.BitmapImage)glyph).UriSource.ToString();
#else
			result = glyph.ToString();
#endif
			return result;
		}
		protected virtual bool SkipAddingToAddBarItemLink(ModelItem barItem) {
			return (bool)barItem.Properties["IsPrivate"].ComputedValue;
		}
		protected virtual void InitializeMenuActions() {
			AddBarButtonItem = new AddBarItemMenuAction("BarButtonItem", typeof(BarButtonItem));
			AddBarButtonItem.Execute += OnAddBarItemExecute;
			AddBarCheckItem = new AddBarItemMenuAction("BarCheckItem", typeof(BarCheckItem));
			AddBarCheckItem.Execute += OnAddBarItemExecute;
			AddBarSplitButtonItem = new AddBarItemMenuAction("BarSplitButtonItem", typeof(BarSplitButtonItem));
			AddBarSplitButtonItem.Execute += OnAddBarItemExecute;
			AddBarSplitCheckItem = new AddBarItemMenuAction("BarSplitCheckItem", typeof(BarSplitCheckItem));
			AddBarSplitCheckItem.Execute += OnAddBarItemExecute;
			AddBarSubItem = new AddBarItemMenuAction("BarSubItem", typeof(BarSubItem));
			AddBarSubItem.Execute += OnAddBarItemExecute;
			AddBarStaticItem = new AddBarItemMenuAction("BarStaticItem", typeof(BarStaticItem));
			AddBarStaticItem.Execute += OnAddBarItemExecute;
			InitializeBarEditItems();
			DeleteAction = new MenuAction("Delete");
			DeleteAction.Execute += OnDeleteActionExecute;
		}
		protected virtual void InitializeBarEditItems() {
			BarEditItemActions = new List<AddBarItemMenuAction>();
			foreach(Type editSettingsType in GetEditSettings()) {
				string name = editSettingsType.Name;
				AddBarItemMenuAction item = new AddBarItemMenuAction(name, editSettingsType);
				item.ImageUri = GetImageUri(editSettingsType.Name);
				item.Execute += OnAddBarItemExecute;
				BarEditItemActions.Add(item);
			}
		}
		protected virtual IEnumerable<Type> GetEditSettings() {
			return TypeLists.EditSettingsTypes.Where(t => (t.Item2 & PropertyTarget.Bar) != 0).Select(t => t.Item1).ToArray();
		}
		protected virtual void InitializeMenuGroups() {
			AddBarItemGroup = new MenuGroup("Add BarItem") { HasDropDown = true };
			AddEditItemGroup = new MenuGroup("BarEditItem") { HasDropDown = true };
			AddBarItemLinkGroup = new MenuGroup("Add BarItemLink");
		}
		protected virtual ModelProperty GetParentItemLinksCollection(MenuAction menuAction, ModelItem container) {
			if(container.IsItemOfType(typeof(ILinksHolder)))
				return container.Properties["ItemLinks"];
			return null;
		}
		protected virtual ModelProperty GetParentCommonItemsCollection(MenuAction menuAction, ModelItem container) {
			if(container.IsItemOfType(typeof(ILinksHolder)))
				return container.Properties["Items"];
			return null;
		}
		protected virtual Uri GetImageUri(string imageUri) {
			return new Uri(string.Format("pack://application:,,,/{0}.Design;component/Images/EditSettings/{1}.png", AssemblyInfo.SRAssemblyXpfCore, imageUri));
		}
		protected virtual void OnAddBarItemExecute(object sender, MenuActionEventArgs e) {
			AddBarItemMenuAction action = sender as AddBarItemMenuAction;
			if(action == null) return;
			ModelItem targetItem = e.Selection.PrimarySelection;
			ModelItem item = CreateNewBarItem(e.Context, action.IsBarEditItem, action.BarItemType);
			using(ModelEditingScope scope = targetItem.BeginEdit(string.Format("Add {0}", item.ItemType.Name))) {
				var property = GetTargetProperty(targetItem);
				if(property != null)
					property.Collection.Add(item);
				scope.Complete();
			}
		}
		protected virtual ModelProperty GetTargetProperty(ModelItem targetItem) {
			if(targetItem.Content.IsCollection && targetItem.Content.IsPropertyOfType(typeof(CommonBarItemCollection)))
				return targetItem.Content;
			string propertyName = "Items";
			if(targetItem.ItemType.Name.Equals("RibbonGalleryBarItem")) {
				propertyName = "DropDownItems";
			}
			var property = targetItem.Properties.Find(propertyName);
			return property;
		}
		protected virtual void OnAddBarItemLinkMenuActionExecute(object sender, MenuActionEventArgs e) {
			AddBarItemLinkMenuAction menuAction = sender as AddBarItemLinkMenuAction;
			if(menuAction == null) return;
			ModelItem barItemLink = BarManagerDesignTimeHelper.CreateBarItemLink(menuAction.BarItem);
			ModelItem container = e.Selection.PrimarySelection;
			ModelProperty parentCollection = GetParentCommonItemsCollection(menuAction, container);
			if(parentCollection != null) {
				using(ModelEditingScope scope = parentCollection.Parent.BeginEdit(string.Format("Add {0}", barItemLink.ItemType.Name))) {
					BarManagerDesignTimeHelper.AddBarItemLink(parentCollection, barItemLink);
					scope.Complete();
				}
			}
		}
		protected virtual void OnDeleteActionExecute(object sender, MenuActionEventArgs e) {
			ModelItem bar = e.Selection.PrimarySelection;
			using(var scope = bar.BeginEdit("Remove the Bar")) {
				BarManagerDesignTimeHelper.RemoveBar(bar);
				scope.Complete();
			}
		}
		protected virtual void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			DeleteAction.DisplayName = e.Selection.PrimarySelection != null ? string.Format("Delete {0}", e.Selection.PrimarySelection.ItemType.Name) : "Delete";
			InitializeAddBarItemLinkGroup(BarManagerDesignTimeHelper.FindBarManagerInParent(e.Selection.PrimarySelection));
		}
		protected ModelItem CreateNewBarItem(EditingContext editingContext, bool isBarEditItem, Type barItemType) {
			return isBarEditItem ? BarManagerDesignTimeHelper.CreateBarEditItem(editingContext, barItemType) : BarManagerDesignTimeHelper.CreateBarItem(editingContext, barItemType);
		}
	}
	class BarContextMenuProvider : ContainerContextMenuProvider {
		protected MenuGroup SetContainerTypeGroup { get; set; }
		protected MenuAction NoneDockInfo { get; set; }
		protected MenuAction LeftDockInfo { get; set; }
		protected MenuAction TopDockInfo { get; set; }
		protected MenuAction RightDockInfo { get; set; }
		protected MenuAction BottomDockInfo { get; set; }
		public BarContextMenuProvider() {
			UpdateItemStatus += OnUpdateItemStatus;
		}
		protected override void InitializeMenuGroups() {
			base.InitializeMenuGroups();
			SetContainerTypeGroup = new MenuGroup("Set ContainerType") { HasDropDown = true };
		}
		protected override void InitializeMenuActions() {
			base.InitializeMenuActions();
			NoneDockInfo = new MenuAction("None") { Checkable = true };
			NoneDockInfo.Execute += OnDockInfoMenuItemExecute;
			LeftDockInfo = new MenuAction("Left") { Checkable = true };
			LeftDockInfo.Execute += OnDockInfoMenuItemExecute;
			TopDockInfo = new MenuAction("Top") { Checkable = true };
			TopDockInfo.Execute += OnDockInfoMenuItemExecute;
			RightDockInfo = new MenuAction("Right") { Checkable = true };
			RightDockInfo.Execute += OnDockInfoMenuItemExecute;
			BottomDockInfo = new MenuAction("Bottom") { Checkable = true };
			BottomDockInfo.Execute += OnDockInfoMenuItemExecute;
		}
		protected override void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			base.OnUpdateItemStatus(sender, e);
			UpdateContainerTypeGroup(e);
		}
		protected override void AddGroups() {
			AddBarItemGroup.Items.Add(AddEditItemGroup);
			Items.Add(AddBarItemGroup);
			Items.Add(AddBarItemLinkGroup);
			Items.Add(SetContainerTypeGroup);
#if !SL
			Items.Add(DeleteAction);
#endif
		}
		protected override void AddItemsForGroup() {
			base.AddItemsForGroup();
			SetContainerTypeGroup.Items.Add(LeftDockInfo);
			SetContainerTypeGroup.Items.Add(TopDockInfo);
			SetContainerTypeGroup.Items.Add(RightDockInfo);
			SetContainerTypeGroup.Items.Add(BottomDockInfo);
			SetContainerTypeGroup.Items.Add(NoneDockInfo);
		}
		void OnDockInfoMenuItemExecute(object sender, MenuActionEventArgs e) {
			MenuAction item = sender as MenuAction;
			ModelItem bar = e.Selection.PrimarySelection;
			if(item == null || bar == null || !bar.IsItemOfType(typeof(Bar))) return;
			BarContainerType type = (BarContainerType)Enum.Parse(typeof(BarContainerType), item.DisplayName);
			BarManagerDesignTimeHelper.SetBarContainerType(bar, type);
		}
		void UpdateContainerTypeGroup(MenuActionEventArgs e) {
			ModelItem barManager = BarManagerDesignTimeHelper.FindBarManagerInParent(e.Selection.PrimarySelection);
			bool isCreateStandardLayout = BarManagerDesignTimeHelper.IsCreateStandardLayout(barManager);
			if(isCreateStandardLayout) {
				InitCheckForContainerTypeGroup(e.Selection.PrimarySelection);
			}
			SetContainerTypeGroup.HasDropDown = isCreateStandardLayout;
			foreach(MenuAction item in SetContainerTypeGroup.Items) {
				item.Visible = isCreateStandardLayout;
			}
		}
		void InitCheckForContainerTypeGroup(ModelItem bar) {
			BarContainerType type = (BarContainerType)bar.Properties["DockInfo"].Value.Properties["ContainerType"].ComputedValue;
			BottomDockInfo.Checked = type == BarContainerType.Bottom;
			LeftDockInfo.Checked = type == BarContainerType.Left;
			NoneDockInfo.Checked = type == BarContainerType.None;
			RightDockInfo.Checked = type == BarContainerType.Right;
			TopDockInfo.Checked = type == BarContainerType.Top;
		}
	}
	public class AddBarItemMenuAction : MenuAction, ICloneable {
		public Type BarItemType { get; protected set; }
		public bool IsBarEditItem { get { return BarItemType.IsSubclassOf(typeof(BaseEditSettings)); } }
		public object Tag { get; set; }
		public AddBarItemMenuAction(string displayName, Type barItemType)
			: base(displayName) {
			if(barItemType.IsSubclassOf(typeof(BarItem)) || barItemType.IsSubclassOf(typeof(BaseEditSettings)))
				BarItemType = barItemType;
			else throw new ArgumentException("barItemType parameter must be of BarItem or BaseEditSettings type");
		}
		public object Clone() {
			return new AddBarItemMenuAction(DisplayName, BarItemType) { Tag = this.Tag };
		}
	}
	public class AddBarItemLinkMenuAction : MenuAction, ICloneable {
		public ModelItem BarItem { get; protected set; }
		public object Tag { get; set; }
		public AddBarItemLinkMenuAction(ModelItem barItem) : base(barItem.Name) {
			BarItem = barItem;
			if(BarItem.Properties["Content"].ComputedValue is string)
				DisplayName = string.Format("{0} <{1}>", BarItem.Properties["Content"].ComputedValue, BarItem.Name);
		}
		public object Clone() {
			return new AddBarItemLinkMenuAction(BarItem);
		}
	}
}

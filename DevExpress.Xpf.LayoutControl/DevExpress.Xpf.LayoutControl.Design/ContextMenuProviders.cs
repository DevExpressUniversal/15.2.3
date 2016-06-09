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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.LayoutControl;
namespace DevExpress.Xpf.LayoutControl.Design {
	using LayoutControl = Platform::DevExpress.Xpf.LayoutControl.LayoutControl;
	class LayoutGroupContextMenuProviderBase : ContextMenuProviderBase {
		public LayoutGroupContextMenuProviderBase() {
			MainMenuGroup = new MenuGroup("LayoutGroupMenuItems") { HasDropDown = false };
			Items.Add(MainMenuGroup);
			OrientationMenuGroup = CreateEnumMenuGroup(MainMenuGroup.Items, "Orientation",
				typeof(Platform::System.Windows.Controls.Orientation), OnOrientationExecute);
			InsertAvailableItemMenuGroup = new MenuGroup("InsertAvailableItem", "Insert From Available Items") { HasDropDown = true };
			MainMenuGroup.Items.Add(InsertAvailableItemMenuGroup);
		}
		protected override void OnUpdateItemStatus(MenuActionEventArgs e) {
			base.OnUpdateItemStatus(e);
			SetEnumMenuGroupValue(OrientationMenuGroup,
				GetProperty<Platform::System.Windows.Controls.Orientation>(e.Selection.SelectedObjects, "Orientation"));
			AddAvailableItems(InsertAvailableItemMenuGroup.Items, e.Selection.PrimarySelection);
			if (InsertAvailableItemMenuGroup.Items.Count == 0)
				MainMenuGroup.Items.Remove(InsertAvailableItemMenuGroup);
		}
		protected MenuGroup MainMenuGroup { get; private set; }
		void OnInsertAvailableItemExecute(object sender, MenuActionEventArgs e) {
			using (ModelEditingScope editingScope = e.Selection.PrimarySelection.BeginEdit("Insert From Available Items")) {
				ModelItem item = InsertAvailableItem(e.Selection.PrimarySelection, InsertAvailableItemMenuGroup.Items.IndexOf((MenuAction)sender));
				e.Context.Items.SetValue(new Selection(item));
				editingScope.Complete();
			}
		}
		void OnOrientationExecute(object sender, MenuActionEventArgs e) {
			var orientation = (Platform::System.Windows.Controls.Orientation)GetEnumMenuGroupValue(OrientationMenuGroup, (MenuAction)sender);
			SetProperty<Platform::System.Windows.Controls.Orientation>(e.Selection, "Orientation", orientation);
		}
		void AddAvailableItems(ObservableCollection<MenuBase> menuItems, ModelItem group) {
			menuItems.Clear();
			ModelItem layoutControlItem = group.GetLayoutControl(true);
			foreach (ModelItem item in layoutControlItem.Properties["AvailableItems"].Collection) {
				var menuItem = new MenuAction(GetAvailableItemDisplayName(item));
				menuItem.Execute += OnInsertAvailableItemExecute;
				menuItems.Add(menuItem);
			}
		}
		string GetAvailableItemDisplayName(ModelItem item) {
			string result = item.ItemType.Name;
			if (!string.IsNullOrEmpty(item.Name))
				result += " (" + item.Name + ")";
			var element = item.GetCurrentValue() as Platform::System.Windows.UIElement;
			if (element != null) {
				string label = LayoutControl.GetCustomizationLabel(element) as string;
				if (string.IsNullOrEmpty(label))
					label = LayoutControl.GetCustomizationDefaultLabel(element);
				result += " \"" + label + "\"";
			}
			return result;
		}
		ModelItem InsertAvailableItem(ModelItem group, int availableItemIndex) {
			ModelItem layoutControlItem = group.GetLayoutControl(true);
			ModelItemCollection availableItems = layoutControlItem.Properties["AvailableItems"].Collection;
			ModelItem availableItem = availableItems[availableItemIndex];
			availableItems.RemoveAt(availableItemIndex);
			group.Properties["Children"].Collection.Add(availableItem);
			return availableItem;
		}
		MenuGroup InsertAvailableItemMenuGroup { get; set; }
		MenuGroup OrientationMenuGroup { get; set; }
	}
	class LayoutGroupSelectionPolicy : ConditionalSelectionPolicy {
		protected override bool Condition(ModelItem item) {
			return item.IsLayoutGroup();
		}
	}
	[UsesItemPolicy(typeof(LayoutGroupSelectionPolicy))]
	class LayoutGroupContextMenuProvider : LayoutGroupContextMenuProviderBase {
		public LayoutGroupContextMenuProvider() {
			ViewMenuGroup = CreateEnumMenuGroup(MainMenuGroup.Items, "View", typeof(LayoutGroupView), OnViewExecute);
			MainMenuGroup.Items.Move(MainMenuGroup.Items.IndexOf(ViewMenuGroup), 0);
			TabMenuGroup = new MenuGroup("LayoutGroupTabMenuItems") { HasDropDown = false };
			CreateMenuItem(TabMenuGroup.Items, "Add Tab", false, OnAddTabExecute);
			MoveTabBackwardMenuItem = CreateMenuItem(TabMenuGroup.Items, "Move Active Tab Backward", false, OnMoveTabExecute);
			MoveTabForwardMenuItem = CreateMenuItem(TabMenuGroup.Items, "Move Active Tab Forward", false, OnMoveTabExecute);
			Items.Add(TabMenuGroup);
		}
		protected override bool IsActive(Selection selection) {
			return selection.SelectedObjects.All(item => item.IsLayoutGroup());
		}
		protected override void OnUpdateItemStatus(MenuActionEventArgs e) {
			base.OnUpdateItemStatus(e);
			SetEnumMenuGroupValue(ViewMenuGroup, GetProperty<LayoutGroupView>(e.Selection.SelectedObjects, "View"));
			if (AreTabActionsSupported(e.Selection)) {
				MoveTabBackwardMenuItem.Enabled = IsMoveTabEnabled(e.Selection.PrimarySelection, false);
				MoveTabForwardMenuItem.Enabled = IsMoveTabEnabled(e.Selection.PrimarySelection, true);
			}
			else
				Items.Remove(TabMenuGroup);
		}
		void OnAddTabExecute(object sender, MenuActionEventArgs e) {
			ModelItem group = e.Selection.PrimarySelection;
			using (ModelEditingScope editingScope = group.BeginEdit("Add Tab")) {
				ModelItem tab = ModelFactory.CreateItem(e.Context, typeof(LayoutGroup), CreateOptions.InitializeDefaults, null);
				LayoutGroupParentAdapter.InitializeChild(group, tab);
				group.Properties["Children"].Collection.Add(tab);
				e.Context.Items.SetValue(new Selection(tab));
				editingScope.Complete();
			}
		}
		void OnMoveTabExecute(object sender, MenuActionEventArgs e) {
			ModelItem group = e.Selection.PrimarySelection;
			using (ModelEditingScope editingScope = group.BeginEdit("Move Tab")) {
				ModelItemCollection groupChildren = group.Properties["Children"].Collection;
				ModelItem tab = GetActiveTab(group);
				int tabIndex = groupChildren.IndexOf(tab);
				int tabIndexChange = sender == MoveTabForwardMenuItem ? 1 : -1;
				groupChildren.Move(tabIndex, tabIndex + tabIndexChange);
				e.Context.Items.SetValue(new Selection(tab));
				editingScope.Complete();
			}
		}
		void OnViewExecute(object sender, MenuActionEventArgs e) {
			var view = (LayoutGroupView)GetEnumMenuGroupValue(ViewMenuGroup, (MenuAction)sender);
			SetProperty<LayoutGroupView>(e.Selection, "View", view, SetView);
		}
		bool AreTabActionsSupported(Selection selection) {
			return selection.SelectionCount == 1 && selection.PrimarySelection.Properties["View"].ComputedValue.Equals(LayoutGroupView.Tabs);
		}
		ModelItem GetActiveTab(ModelItem group) {
			Platform::System.Windows.FrameworkElement selectedTabChild = ((LayoutGroup)group.GetCurrentValue()).SelectedTabChild;
			if (selectedTabChild == null)
				return null;
			ModelService service = group.Context.Services.GetService<ModelService>();
			return service.Find(group, selectedTabChild);
		}
		bool IsMoveTabEnabled(ModelItem group, bool forward) {
			ModelItem activeTab = GetActiveTab(group);
			if (activeTab == null)
				return false;
			ModelItemCollection groupChildren = group.Properties["Children"].Collection;
			int tabIndex = groupChildren.IndexOf(activeTab);
			if (forward)
				return tabIndex < groupChildren.Count - 1;
			else
				return tabIndex > 0;
		}
		void SetView(ModelItem item, LayoutGroupView view) {
			item.Properties["View"].SetValue(view);
			if (view == LayoutGroupView.GroupBox && !item.Properties["Header"].IsSet)
				item.Properties["Header"].SetValue(LayoutGroupParentAdapter.GetDefaultGroupHeader(item));
		}
		MenuAction MoveTabBackwardMenuItem { get; set; }
		MenuAction MoveTabForwardMenuItem { get; set; }
		MenuGroup TabMenuGroup { get; set; }
		MenuGroup ViewMenuGroup { get; set; }
	}
	[UsesItemPolicy(typeof(PrimarySelectionPolicy))]
	class LayoutControlContextMenuProvider : LayoutGroupContextMenuProviderBase {
		public LayoutControlContextMenuProvider() {
			CustomizationModeMenuItem = CreateMenuItem(MainMenuGroup.Items, "Customization Mode", true, OnCustomizationModeExecute);
			InplaceLayoutBuilderMenuItem = CreateMenuItem(MainMenuGroup.Items, "Inplace Layout Builder", true, OnInplaceLayoutBuilderExecute);
			MenuAction optimizeLayoutMenuItem = CreateMenuItem(MainMenuGroup.Items, "Optimize Layout...", false, OnOptimizeLayoutExecute);
			MainMenuGroup.Items.Move(MainMenuGroup.Items.IndexOf(CustomizationModeMenuItem), 0);
			MainMenuGroup.Items.Move(MainMenuGroup.Items.IndexOf(InplaceLayoutBuilderMenuItem), 1);
			MainMenuGroup.Items.Move(MainMenuGroup.Items.IndexOf(optimizeLayoutMenuItem), 2);
		}
		protected override bool IsActive(Selection selection) {
			return selection.PrimarySelection != null &&
				selection.PrimarySelection.IsItemOfType(typeof(LayoutControl)) &&
				!selection.PrimarySelection.IsItemOfType(typeof(DataLayoutControl));
		}
		protected override void OnUpdateItemStatus(MenuActionEventArgs e) {
			base.OnUpdateItemStatus(e);
			LayoutControlDesignService service = e.Context.Services.GetService<LayoutControlDesignService>();
			CustomizationModeMenuItem.Checked = service.GetIsInCustomizationMode(e.Selection.PrimarySelection);
			InplaceLayoutBuilderMenuItem.Checked = service.GetIsInplaceLayoutBuilderVisible(e.Selection.PrimarySelection);
		}
		void OnCustomizationModeExecute(object sender, MenuActionEventArgs e) {
			LayoutControlDesignService service = e.Context.Services.GetService<LayoutControlDesignService>();
			ModelItem modelItem = e.Selection.PrimarySelection;
			service.SetIsInCustomizationMode(modelItem, !service.GetIsInCustomizationMode(modelItem));
		}
		void OnInplaceLayoutBuilderExecute(object sender, MenuActionEventArgs e) {
			LayoutControlDesignService service = e.Context.Services.GetService<LayoutControlDesignService>();
			ModelItem modelItem = e.Selection.PrimarySelection;
			service.SetIsInplaceLayoutBuilderVisible(modelItem, !service.GetIsInplaceLayoutBuilderVisible(modelItem));
		}
		void OnOptimizeLayoutExecute(object sender, MenuActionEventArgs e) {
			MessageBoxResult result = MessageBox.Show(
				"Unneeded LayoutGroups will be deleted. Do you also want to delete empty tabbed LayoutGroups?",
				"Layout Optimization", MessageBoxButton.YesNoCancel);
			if (result == MessageBoxResult.Cancel)
				return;
			ModelItem modelItem = e.Selection.PrimarySelection;
			var control = (LayoutControl)modelItem.GetCurrentValue();
			if (control.OptimizeLayout(result == MessageBoxResult.No))
				using (ModelEditingScope editingScope = modelItem.BeginEdit("Optimize Layout")) {
					new LayoutControlModelUpdater(modelItem, control).UpdateModel();
					editingScope.Complete();
				}
		}
		MenuAction CustomizationModeMenuItem { get; set; }
		MenuAction InplaceLayoutBuilderMenuItem { get; set; }
	}
	class LayoutGroupParentSelectionPolicy : SameParentTypeSelectionPolicy {
		protected override Type GetParentType() {
			return typeof(ILayoutGroup);
		}
	}
	[UsesItemPolicy(typeof(LayoutGroupParentSelectionPolicy))]
	class LayoutGroupChildContextMenuProvider : ChildContextMenuProvider {
		public LayoutGroupChildContextMenuProvider() {
			var mainGroup = new MenuGroup("LayoutGroupChildMainMenuItems") { HasDropDown = false };
			HorizontalAlignmentMenuGroup = CreateAlignmentMenuGroup("Horizontal Alignment",
				typeof(Platform::System.Windows.HorizontalAlignment), OnHorizontalAlignmentExecute, 
				(menuItem) => AutoHorizontalAlignmentMenuItem = menuItem, (menuGroup) => HorizontalAlignmentValueMenuGroup = menuGroup);
			mainGroup.Items.Add(HorizontalAlignmentMenuGroup);
			VerticalAlignmentMenuGroup = CreateAlignmentMenuGroup("Vertical Alignment",
				typeof(Platform::System.Windows.VerticalAlignment), OnVerticalAlignmentExecute,
				(menuItem) => AutoVerticalAlignmentMenuItem = menuItem, (menuGroup) => VerticalAlignmentValueMenuGroup = menuGroup);
			mainGroup.Items.Add(VerticalAlignmentMenuGroup);
			Items.Add(mainGroup);
			var group = new MenuGroup("LayoutGroupChildAdditionalMenuItems") { HasDropDown = false };
			MoveIntoLayoutItemMenuItem = CreateMenuItem(group.Items, "Move Into LayoutItem", false, OnMoveIntoLayoutItemExecute);
			RemoveLayoutItemMenuItem = CreateMenuItem(group.Items, "Remove LayoutItem", false, OnRemoveLayoutItemExecute);
			CreateMenuItem(group.Items, "Move To Available Items", false, OnMoveToAvailableItemsExecute);
			Items.Add(group);
		}
		protected override Type GetParentType() {
			return typeof(ILayoutGroup);
		}
		protected override void OnUpdateItemStatus(MenuActionEventArgs e) {
			base.OnUpdateItemStatus(e);
			UpdateAlignmentMenu<Platform::System.Windows.HorizontalAlignment>(e.Selection, "HorizontalAlignment",
				HorizontalAlignmentMenuGroup, AutoHorizontalAlignmentMenuItem, HorizontalAlignmentValueMenuGroup);
			UpdateAlignmentMenu<Platform::System.Windows.VerticalAlignment>(e.Selection, "VerticalAlignment",
				VerticalAlignmentMenuGroup, AutoVerticalAlignmentMenuItem, VerticalAlignmentValueMenuGroup);
			MoveIntoLayoutItemMenuItem.Enabled = IsMoveIntoLayoutItemEnabled(e.Selection.SelectedObjects);
			RemoveLayoutItemMenuItem.Enabled = IsRemoveLayoutItemEnabled(e.Selection.SelectedObjects);
		}
		MenuGroup CreateAlignmentMenuGroup(string propertyDisplayName, Type propertyType, EventHandler<MenuActionEventArgs> execute,
			Action<MenuAction> setAutoAlignmentMenuItem, Action<MenuGroup> setAlignmentValueMenuGroup) {
			var alignmentMenuGroup = new MenuGroup(propertyDisplayName) { HasDropDown = true };
			setAutoAlignmentMenuItem(CreateMenuItem(alignmentMenuGroup.Items, "Auto", true, execute));
			MenuGroup alignmentValueMenuGroup = CreateEnumMenuGroup(alignmentMenuGroup.Items, propertyDisplayName + " Value", propertyType, execute);
			alignmentValueMenuGroup.HasDropDown = false;
			setAlignmentValueMenuGroup(alignmentValueMenuGroup);
			return alignmentMenuGroup;
		}
		void UpdateAlignmentMenu<T>(Selection selection, string propertyName,
			MenuGroup alignmentMenuGroup, MenuAction autoAlignmentMenuItem, MenuGroup alignmentValueMenuGroup) where T : struct {
			bool isAutoAlignment = false;
			if (selection.SelectionCount == 1 && selection.PrimarySelection.IsLayoutGroup()) {
				isAutoAlignment = !selection.PrimarySelection.Properties[propertyName].IsSet;
				autoAlignmentMenuItem.Checked = isAutoAlignment;
			}
			else
				alignmentMenuGroup.Items.Remove(autoAlignmentMenuItem);
			if (isAutoAlignment)
				SetEnumMenuGroupValue(alignmentValueMenuGroup, -1);
			else
				SetEnumMenuGroupValue(alignmentValueMenuGroup, GetProperty<T>(selection.SelectedObjects, propertyName));
		}
		void OnAlignmentExecute<T>(object sender, MenuActionEventArgs e, string propertyName,
#if !SILVERLIGHT
			Action<ILayoutGroup, FrameworkElement> propertyChanged,
#endif
			MenuAction autoAlignmentMenuItem, MenuGroup alignmentValueMenuGroup) {
			object alignment;
			if (sender == autoAlignmentMenuItem)
				alignment = Platform::System.Windows.DependencyProperty.UnsetValue;
			else
				alignment = (T)GetEnumMenuGroupValue(alignmentValueMenuGroup, (MenuAction)sender);
#if SILVERLIGHT
			SetProperty<object>(e.Selection, propertyName, alignment);
#else
			SetProperty<object>(e.Selection, propertyName, alignment,
				(item, value) => SetAlignment(item, propertyName, value, propertyChanged));
#endif
		}
		void OnHorizontalAlignmentExecute(object sender, MenuActionEventArgs e) {
			OnAlignmentExecute<Platform::System.Windows.HorizontalAlignment>(sender, e, "HorizontalAlignment",
#if !SILVERLIGHT
				(parent, element) => parent.ChildHorizontalAlignmentChanged(element),
#endif
				AutoHorizontalAlignmentMenuItem, HorizontalAlignmentValueMenuGroup);
		}
		void OnVerticalAlignmentExecute(object sender, MenuActionEventArgs e) {
			OnAlignmentExecute<Platform::System.Windows.VerticalAlignment>(sender, e, "VerticalAlignment",
#if !SILVERLIGHT
				(parent, element) => parent.ChildVerticalAlignmentChanged(element),
#endif
				AutoVerticalAlignmentMenuItem, VerticalAlignmentValueMenuGroup);
		}
		void OnMoveIntoLayoutItemExecute(object sender, MenuActionEventArgs e) {
			using (ModelEditingScope editingScope = e.Selection.PrimarySelection.BeginEdit("Move Into LayoutItem")) {
				var layoutItems = new List<ModelItem>();
				foreach (ModelItem item in e.Selection.SelectedObjects)
					layoutItems.Add(MoveIntoLayoutItem(item));
				e.Context.Items.SetValue(new Selection(layoutItems));
				editingScope.Complete();
			}
		}
		void OnRemoveLayoutItemExecute(object sender, MenuActionEventArgs e) {
			using (ModelEditingScope editingScope = e.Selection.PrimarySelection.BeginEdit("Remove LayoutItem")) {
				var contentItems = new List<ModelItem>();
				foreach (ModelItem item in e.Selection.SelectedObjects) {
					ModelItem content = RemoveLayoutItem(item);
					if (content != null)
						contentItems.Add(content);
				}
				e.Context.Items.SetValue(new Selection(contentItems));
				editingScope.Complete();
			}
		}
		void OnMoveToAvailableItemsExecute(object sender, MenuActionEventArgs e) {
			using (ModelEditingScope editingScope = e.Selection.PrimarySelection.BeginEdit("Move to Available Items")) {
				foreach (ModelItem item in e.Selection.SelectedObjects)
					MoveToAvailableItems(item);
				e.Context.Items.SetValue(e.Selection);
				editingScope.Complete();
			}
		}
		bool IsMoveIntoLayoutItemEnabled(IEnumerable<ModelItem> items) {
			foreach (ModelItem item in items)
				if (!IsMoveIntoLayoutItemEnabled(item))
					return false;
			return true;
		}
		bool IsMoveIntoLayoutItemEnabled(ModelItem item) {
			return !(item.IsLayoutGroup() || item.IsItemOfType(typeof(LayoutItem)));
		}
		bool IsRemoveLayoutItemEnabled(IEnumerable<ModelItem> items) {
			foreach (ModelItem item in items)
				if (!IsRemoveLayoutItemEnabled(item))
					return false;
			return true;
		}
		bool IsRemoveLayoutItemEnabled(ModelItem item) {
			return item.IsItemOfType(typeof(LayoutItem)) && !item.IsItemOfType(typeof(DataLayoutItem));
		}
		ModelItem MoveIntoLayoutItem(ModelItem item) {
			ModelItemCollection itemParentChildren = item.Parent.Properties["Children"].Collection;
			ModelItem layoutItem = ModelFactory.CreateItem(item.Context, typeof(LayoutItem), CreateOptions.InitializeDefaults, null);
			itemParentChildren.Insert(itemParentChildren.IndexOf(item), layoutItem);
			itemParentChildren.Remove(item);
			layoutItem.Properties["Content"].SetValue(item);
			return layoutItem;
		}
		void MoveToAvailableItems(ModelItem item) {
			ModelItem layoutControlItem = item.GetLayoutControl(false);
			item.Parent.Properties["Children"].Collection.Remove(item);
			layoutControlItem.Properties["AvailableItems"].Collection.Add(item);
		}
		ModelItem RemoveLayoutItem(ModelItem item) {
			ModelItemCollection itemParentChildren = item.Parent.Properties["Children"].Collection;
			ModelItem content = item.Properties["Content"].Value;
			if (content != null) {
				item.Properties["Content"].SetValue(null);
				itemParentChildren.Insert(itemParentChildren.IndexOf(item), content);
			}
			itemParentChildren.Remove(item);
			return content;
		}
#if !SILVERLIGHT
		void SetAlignment(ModelItem item, string propertyName, object value, Action<ILayoutGroup, FrameworkElement> propertyChanged) {
			ModelProperty property = item.Properties[propertyName];
			bool isAlignmentAssigned = property.IsSet;
			object alignment = property.ComputedValue;
			if (value == Platform::System.Windows.DependencyProperty.UnsetValue)
				property.ClearValue();
			else
				property.SetValue(value);
			var element = (FrameworkElement)item.GetCurrentValue();
			if (element.IsLayoutGroup() && property.ComputedValue.Equals(alignment) && property.IsSet != isAlignmentAssigned)
				propertyChanged((ILayoutGroup)element.Parent, element);
		}
#endif
		MenuGroup HorizontalAlignmentMenuGroup { get; set; }
		MenuAction AutoHorizontalAlignmentMenuItem { get; set; }
		MenuGroup HorizontalAlignmentValueMenuGroup { get; set; }
		MenuGroup VerticalAlignmentMenuGroup { get; set; }
		MenuAction AutoVerticalAlignmentMenuItem { get; set; }
		MenuGroup VerticalAlignmentValueMenuGroup { get; set; }
		MenuAction MoveIntoLayoutItemMenuItem { get; set; }
		MenuAction RemoveLayoutItemMenuItem { get; set; }
	}
	class FlowLayoutControlParentSelectionPolicy : SameParentTypeSelectionPolicy {
		protected override Type GetParentType() {
			return typeof(FlowLayoutControl);
		}
	}
	[UsesItemPolicy(typeof(FlowLayoutControlParentSelectionPolicy))]
	class FlowLayoutControlChildContextMenuProvider : ChildContextMenuProvider {
		public FlowLayoutControlChildContextMenuProvider() {
			var group = new MenuGroup("FlowLayoutControlChildMenuItems") { HasDropDown = false };
			IsFlowBreakMenuItem = CreateMenuItem(group.Items, "Flow Break", true, OnFlowBreakExecute);
			Items.Add(group);
		}
		protected override Type GetParentType() {
			return typeof(FlowLayoutControl);
		}
		protected override void OnUpdateItemStatus(MenuActionEventArgs e) {
			base.OnUpdateItemStatus(e);
			IsFlowBreakMenuItem.Checked = IsFlowBreak(e.Selection.SelectedObjects);
		}
		bool IsFlowBreak(IEnumerable<ModelItem> items) {
			return GetAttachedProperty<bool>(items, typeof(FlowLayoutControl), "IsFlowBreak") ?? false;
		}
		void OnFlowBreakExecute(object sender, MenuActionEventArgs e) {
			SetAttachedProperty<bool>(e.Selection, typeof(FlowLayoutControl), "IsFlowBreak", !IsFlowBreak(e.Selection.SelectedObjects));
		}
		MenuAction IsFlowBreakMenuItem { get; set; }
	}
	class DockLayoutControlParentSelectionPolicy : SameParentTypeSelectionPolicy {
		protected override Type GetParentType() {
			return typeof(DockLayoutControl);
		}
	}
	[UsesItemPolicy(typeof(DockLayoutControlParentSelectionPolicy))]
	class DockLayoutControlChildContextMenuProvider : ChildContextMenuProvider {
		public DockLayoutControlChildContextMenuProvider() {
			var mainGroup = new MenuGroup("DockLayoutControlChildMenuItems") { HasDropDown = false };
			DockMenuGroup = CreateEnumMenuGroup(mainGroup.Items, "Dock", typeof(Dock), OnDockExecute);
			var group = new MenuGroup("AllowSizing", "Allow Sizing") { HasDropDown = true };
			AllowHorizontalSizingMenuItem = CreateMenuItem(group.Items, "Horizontal", true, OnAllowHorizontalSizingExecute);
			AllowVerticalSizingMenuItem = CreateMenuItem(group.Items, "Vertical", true, OnAllowVerticalSizingExecute);
			mainGroup.Items.Add(group);
			group = new MenuGroup("UseDesiredSizeAsMaxSize", "Use Desired Size As Max Size") { HasDropDown = true };
			UseDesiredWidthAsMaxWidthMenuItem = CreateMenuItem(group.Items, "Width", true, OnUseDesiredWidthAsMaxWidthExecute);
			UseDesiredHeightAsMaxHeightMenuItem = CreateMenuItem(group.Items, "Height", true, OnUseDesiredHeightAsMaxHeightExecute);
			mainGroup.Items.Add(group);
			Items.Add(mainGroup);
		}
		protected override Type GetParentType() {
			return typeof(DockLayoutControl);
		}
		protected override void OnUpdateItemStatus(MenuActionEventArgs e) {
			base.OnUpdateItemStatus(e);
			SetEnumMenuGroupValue(DockMenuGroup, GetAttachedProperty<Dock>(e.Selection.SelectedObjects, typeof(DockLayoutControl), "Dock"));
			AllowHorizontalSizingMenuItem.Checked = IsAllowHorizontalSizing(e.Selection.SelectedObjects);
			AllowVerticalSizingMenuItem.Checked = IsAllowVerticalSizing(e.Selection.SelectedObjects);
			UseDesiredWidthAsMaxWidthMenuItem.Checked = IsUseDesiredWidthAsMaxWidth(e.Selection.SelectedObjects);
			UseDesiredHeightAsMaxHeightMenuItem.Checked = IsUseDesiredHeightAsMaxHeight(e.Selection.SelectedObjects);
		}
		bool IsAllowHorizontalSizing(IEnumerable<ModelItem> items) {
			return GetAttachedProperty<bool>(items, typeof(DockLayoutControl), "AllowHorizontalSizing") ?? false;
		}
		bool IsAllowVerticalSizing(IEnumerable<ModelItem> items) {
			return GetAttachedProperty<bool>(items, typeof(DockLayoutControl), "AllowVerticalSizing") ?? false;
		}
		bool IsUseDesiredWidthAsMaxWidth(IEnumerable<ModelItem> items) {
			return GetAttachedProperty<bool>(items, typeof(DockLayoutControl), "UseDesiredWidthAsMaxWidth") ?? false;
		}
		bool IsUseDesiredHeightAsMaxHeight(IEnumerable<ModelItem> items) {
			return GetAttachedProperty<bool>(items, typeof(DockLayoutControl), "UseDesiredHeightAsMaxHeight") ?? false;
		}
		void OnDockExecute(object sender, MenuActionEventArgs e) {
			var dock = (Dock)GetEnumMenuGroupValue(DockMenuGroup, (MenuAction)sender);
			SetAttachedProperty<Dock>(e.Selection, typeof(DockLayoutControl), "Dock", dock);
		}
		void OnAllowHorizontalSizingExecute(object sender, MenuActionEventArgs e) {
			SetAttachedProperty<bool>(e.Selection, typeof(DockLayoutControl), "AllowHorizontalSizing",
				!IsAllowHorizontalSizing(e.Selection.SelectedObjects));
		}
		void OnAllowVerticalSizingExecute(object sender, MenuActionEventArgs e) {
			SetAttachedProperty<bool>(e.Selection, typeof(DockLayoutControl), "AllowVerticalSizing",
				!IsAllowVerticalSizing(e.Selection.SelectedObjects));
		}
		void OnUseDesiredWidthAsMaxWidthExecute(object sender, MenuActionEventArgs e) {
			SetAttachedProperty<bool>(e.Selection, typeof(DockLayoutControl), "UseDesiredWidthAsMaxWidth",
				!IsUseDesiredWidthAsMaxWidth(e.Selection.SelectedObjects));
		}
		void OnUseDesiredHeightAsMaxHeightExecute(object sender, MenuActionEventArgs e) {
			SetAttachedProperty<bool>(e.Selection, typeof(DockLayoutControl), "UseDesiredHeightAsMaxHeight",
				!IsUseDesiredHeightAsMaxHeight(e.Selection.SelectedObjects));
		}
		MenuAction AllowHorizontalSizingMenuItem { get; set; }
		MenuAction AllowVerticalSizingMenuItem { get; set; }
		MenuGroup DockMenuGroup { get; set; }
		MenuAction UseDesiredWidthAsMaxWidthMenuItem { get; set; }
		MenuAction UseDesiredHeightAsMaxHeightMenuItem { get; set; }
	}
}

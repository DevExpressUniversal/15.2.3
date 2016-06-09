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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class LayoutItemsGroup : GroupBase { };
	public class FormLayoutItemsOwner : DataItemsEditorOwner {
		internal const string ThreeDots = ", ...";
		Type layoutItemsGroupType = typeof(LayoutItemsGroup);
		RootLayoutGroupWrapper rootItemWrapper;
		LayoutGroup rootLayoutGroup;
		ASPxFormLayout formLayout;
		System.Collections.IList formLayoutItems;
		public FormLayoutItemsOwner(ASPxFormLayout formLayout)
			: base(formLayout, "Items", ((IComponent)formLayout).Site, formLayout.Items) {
			OriginalItems = formLayout.Items;
			SaveUndo();
		}
		public FormLayoutItemsOwner(ASPxWebControl component, FormLayoutProperties layoutProperties)
			: base(component, "Items", ((IComponent)component).Site, null) {
			LayoutProperties = layoutProperties;
			OriginalItems = LayoutProperties.Items;
			InitializeOwner(component);
		}
		public override bool SupportDataSource { get { return DesignerSchema != null; } }
		protected virtual FormLayoutProperties LayoutProperties { get; private set; }
		internal virtual ASPxFormLayout FormLayout {
			get {
				if(formLayout == null)
					formLayout = Component as ASPxFormLayout;
				return formLayout;
			}
		}
		Type LayoutItemsGroupType { get { return layoutItemsGroupType; } }
		protected override string KeyFieldName { get { return string.Empty; } set { } }
		internal RootLayoutGroupWrapper RootItemWrapper {
			get {
				if(rootItemWrapper == null)
					rootItemWrapper = new RootLayoutGroupWrapper(RootLayoutGroup);
				return rootItemWrapper;
			}
			set { rootItemWrapper = value; }
		}
		internal virtual LayoutGroup RootLayoutGroup {
			get {
				if(rootLayoutGroup == null)
					rootLayoutGroup = FormLayout.Root;
				return rootLayoutGroup;
			}
		}
		internal LayoutGroup FocusedRootLayoutGroup {
			get {
				if(FocusedItem == null)
					return null;
				if(!TreeListItemsInvertedHash.ContainsKey(FocusedItem) || !TreeListItemsInvertedHash.ContainsKey(RootItemWrapper))
					return null;
				return TreeListItemsInvertedHash[FocusedItem] == TreeListItemsInvertedHash[RootItemWrapper] ? RootLayoutGroup : null;
			}
		}
		protected internal override System.Collections.IList Items {
			get {
				if(formLayoutItems == null || formLayoutItems.Count == 0)
					formLayoutItems = new Collection() { RootItemWrapper };
				return formLayoutItems;
			}
			internal set { }
		}
		public override int FocusedNodeID {
			get { return base.FocusedNodeID; }
			set {
				var item = TreeListItemsHash.ContainsKey(value) ? TreeListItemsHash[value] : null;
				if(item != FocusedItem) {
					FocusedItem = item;
					UpdateView();
				}
			}
		}
		internal virtual Type LayoutItemType { get { return typeof(LayoutItem); } }
		internal IDesignTimeCollectionItem TopNeighbourItem { get; set; }
		internal IDesignTimeCollectionItem BottomNeighbourItem { get; set; }
		IList OriginalItems { get; set; }
		Dictionary<LayoutItemBase, List<Control>> undoNestedControls;
		Dictionary<LayoutItemBase, List<Control>> UndoNestedControls {
			get {
				if(undoNestedControls == null)
					undoNestedControls = new Dictionary<LayoutItemBase, List<Control>>();
				return undoNestedControls;
			}
		}
		public override void SaveUndo() {
			CloneListTo(UndoItems, OriginalItems);
		}
		protected override IDesignTimeCollectionItem CloneItemTo(IList target, object item) {
			var result = base.CloneItemTo(target, item);
			var layoutItem = result as LayoutItem;
			if(layoutItem != null) {
				UndoNestedControls[layoutItem] = new List<Control>();
				CloneListTo(UndoNestedControls[layoutItem], layoutItem.Controls);
			}
			return result;
		}
		public override void UndoChanges() {
			ReplaceList(OriginalItems, UndoItems);
		}
		protected override void UndoItem(IList target, object item) {
			var layoutItem = item as LayoutItem;
			if(layoutItem != null) {
				if(UndoNestedControls.ContainsKey(layoutItem)) {
					layoutItem.Controls.Clear();
					UndoNestedControls[layoutItem].ForEach(c => layoutItem.Controls.Add(c));
				}
			}
			base.UndoItem(target, item);
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.AddItem,
				DesignEditorMenuRootItemActionType.InsertBefore, 
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp,
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.MoveLeft,
				DesignEditorMenuRootItemActionType.MoveRight,
				DesignEditorMenuRootItemActionType.IncreaseColumn,
				DesignEditorMenuRootItemActionType.DecreaseColumn,
				DesignEditorMenuRootItemActionType.IncreaseColSpan,
				DesignEditorMenuRootItemActionType.DecreaseColSpan,
				DesignEditorMenuRootItemActionType.IncreaseRowSpan,
				DesignEditorMenuRootItemActionType.DecreaseRowSpan,
				DesignEditorMenuRootItemActionType.RemoveInnerItems,
				DesignEditorMenuRootItemActionType.ChangeTo,
				DesignEditorMenuRootItemActionType.RetriveFields
			};
		}
		protected override List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp,
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.MoveLeft,
				DesignEditorMenuRootItemActionType.MoveRight,
				DesignEditorMenuRootItemActionType.IncreaseColumn,
				DesignEditorMenuRootItemActionType.DecreaseColumn,
				DesignEditorMenuRootItemActionType.IncreaseColSpan,
				DesignEditorMenuRootItemActionType.DecreaseColSpan,
				DesignEditorMenuRootItemActionType.IncreaseRowSpan,
				DesignEditorMenuRootItemActionType.DecreaseRowSpan,
				DesignEditorMenuRootItemActionType.RemoveInnerItems,
				DesignEditorMenuRootItemActionType.ChangeTo,
				DesignEditorMenuRootItemActionType.SelectAll
			};
		}
		public override List<string> GetViewDependedProperties() {
			var result = base.GetViewDependedProperties();
			result.Add("ColCount");
			result.Add("ColSpan");
			result.Add("RowSpan");
			result.Add("TextBoxDecoration");
			return result;
		}
		protected override bool CanCreateRootMenuItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(actionType == DesignEditorMenuRootItemActionType.RemoveInnerItems ||
			   actionType == DesignEditorMenuRootItemActionType.IncreaseColumn ||
			   actionType == DesignEditorMenuRootItemActionType.DecreaseColumn ||
			   actionType == DesignEditorMenuRootItemActionType.IncreaseColSpan ||
			   actionType == DesignEditorMenuRootItemActionType.DecreaseColSpan||
			   actionType == DesignEditorMenuRootItemActionType.IncreaseRowSpan ||
			   actionType == DesignEditorMenuRootItemActionType.DecreaseRowSpan)
				return true;
			return base.CanCreateRootMenuItem(actionType, isToolbarMenu);
		}
		protected override IDesignTimeCollectionItem CreateDataItemCore(string fieldName) {
			return new LayoutItem() { FieldName = fieldName };
		}
		protected override DesignEditorDescriptorItem CreateAddItemMenuItem() {
			var item = base.CreateAddItemMenuItem();
			item.Enabled = FocusedItemForAction == null || FocusedItemForAction.GetType() != RootItemWrapper.GetType();
			return item;
		}
		protected override DesignEditorDescriptorItem CreateInsertBeforeMenuItem() {
			var item = base.CreateInsertBeforeMenuItem();
			item.Enabled = FocusedItemForAction != null && FocusedItemForAction.GetType() != RootItemWrapper.GetType();
			return item;
		}
		protected virtual bool CanCreateNestedTypes { get { return FocusedItem != null && LayoutItemType.IsAssignableFrom(FocusedItem.GetType()); } }
		protected override DesignEditorDescriptorItem CreateInsertChildMenuItem(bool isToolbar) {
			var item = base.CreateInsertChildMenuItem(isToolbar);
			if(CanCreateNestedTypes) {
				if(isToolbar)
					item.EditorType = DesignEditorDescriptorItemType.DropDown;
				item.Caption = "Add Nested Control";
				item.Enabled = ((LayoutItem)FocusedItem).GetNestedControl() == null;
				PopulateChildItems(item, isToolbar);
			}
			return item;
		}
		protected override DesignEditorDescriptorItem CreateRemoveMenuItem(bool isToolbarMenu) {
			var item = base.CreateRemoveMenuItem(isToolbarMenu);
			item.Enabled = IsSelectionChangingEnabled();
			return item;
		}
		protected override DesignEditorDescriptorItem CreateRemoveInnerItems(bool isToolbarMenu) {
			var item = base.CreateRemoveInnerItems(isToolbarMenu);
			item.Caption = "Remove Nested controls";
			item.Enabled = SelectedItems.Any(i => i is LayoutItem && ((LayoutItem)i).GetNestedControl() != null);
			item.BeginGroup = true;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateIncreaseColumnGroupMenuItem() {
			var item = base.CreateIncreaseColumnGroupMenuItem();
			item.BeginGroup = true;
			item.ImageIndex = GetResourceImageIndex(AddColumnImageResource);
			item.Enabled = FocusedItem is LayoutGroup || FocusedRootLayoutGroup != null;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateDecreaseColumnGroupMenuItem() {
			var item = base.CreateDecreaseColumnGroupMenuItem();
			var layoutGroup = FocusedRootLayoutGroup != null ? FocusedRootLayoutGroup : FocusedItem as LayoutGroup;
			item.ImageIndex = GetResourceImageIndex(RemoveColumnImageResource);
			item.Enabled = layoutGroup != null && layoutGroup.ColCount > 1;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateIncreaseColSpanGroupMenuItem() {
			var item = base.CreateIncreaseColSpanGroupMenuItem();
			item.Enabled = GetFocusedItemIncreaseDecreaseColSpanEnabled(true);
			item.BeginGroup = true;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateDecreaseColSpanGroupMenuItem() {
			var item = base.CreateDecreaseColSpanGroupMenuItem();
			item.Enabled = GetFocusedItemIncreaseDecreaseColSpanEnabled(false);
			return item;
		}
		protected override DesignEditorDescriptorItem CreateDecreaseRowSpanGroupMenuItem() {
			var item = base.CreateDecreaseRowSpanGroupMenuItem();
			item.Enabled = GetFocusedItemDecreaseRowSpanEnabled();
			return item;
		}
		bool GetFocusedItemDecreaseRowSpanEnabled() {
			var item = FocusedItem as LayoutItemBase;
			return item != null && item.RowSpan > 1;
		}
		bool GetFocusedItemIncreaseDecreaseColSpanEnabled(bool increase) {
			if(FocusedItem == null)
				return false;
			var item = FocusedItem as LayoutItemBase;
			var rootGroup = FocusedRootLayoutGroup;
			if(rootGroup != null)
				return increase ? true : rootGroup.ColSpan > 1;
			var group = FocusedItem.Parent as LayoutGroup;
			if(group == null)
				return false;
			return increase ? group.ColCount > item.ColSpan : item.ColSpan > 1;
		}
		protected override DesignEditorDescriptorItem CreateChangeToMenuItem(bool isToolbar) {
			var item = base.CreateChangeToMenuItem(isToolbar);
			item.Enabled = IsSelectionChangingEnabled();
			return item;
		}
		protected override DesignEditorDescriptorItem CreateRetriveFieldsMenuItem() {
			var item = base.CreateRetriveFieldsMenuItem();
			item.Enabled = true;
			if(!SupportDataSource)
				item.EditorType = DesignEditorDescriptorItemType.Button;
			return item;
		}
		public virtual bool ShowConfirmCreateDefaultLayout() {
			return RootLayoutGroup.Items.Count > 0;
		}
		public virtual void CreateDefaultLayout() { }
		internal void AddEditorToLayoutItem(LayoutItem layoutItem, Type editorType) {
			AddControlToLayoutItem(layoutItem, editorType);
		}
		void AddControlToLayoutItem(LayoutItem layoutItem, Type controlType) {
			if(!TypeIsControl(controlType))
				return;
			var control = Activator.CreateInstance(controlType) as System.Web.UI.Control;
			if(control == null)
				return;
			control.ID = FormLayout.GetVacantItemNestedControlID();
			if(FormLayout.DesignTimeEditingMode)
				FormLayout.DesignTimeNestedControlsStorage[layoutItem].Add(control);
			else
				layoutItem.Controls.Add(control);
			DevExpress.Web.Internal.NestedControlHelper.PrepareControl(control, layoutItem.DataType);
		}
		protected override void FillItemTypes() {
			AddItemType(LayoutItemType, "Layout Item", string.Empty);
			AddItemType(typeof(EmptyLayoutItem), "Empty Layout Item", string.Empty);
			AddItemType(typeof(LayoutGroup), "Layout Group", string.Empty);
			AddItemType(typeof(TabbedLayoutGroup), "Tabbed Layout Group", string.Empty);
			AddItemType(LayoutItemsGroupType, "Layout Item with", string.Empty);
			LayoutItem.GetAllowedNestedControlTypes().ForEach(t => { AddGroupItemType(LayoutItemsGroupType, t, t.Name); });
		}
		public override Type GetDefaultItemType() {
			return LayoutItemType;
		}
		protected override IDesignTimeCollectionItem ChangeItemToCore(IDesignTimeColumnAndEditorItem designTimeItem, IDesignTimeCollectionItem oldItem, IDesignTimeCollectionItem neighbour, InsertDirection direction) {
			var result = base.ChangeItemToCore(designTimeItem, oldItem, neighbour, direction);
			if(TypeIsControl(designTimeItem.ColumnType) && result is LayoutItem) {
				Type nestedControlType = designTimeItem.ColumnType;
				var layoutItem = (LayoutItem)result;
				var newNewstedControl = layoutItem.GetNestedControl();
				if(newNewstedControl == null || newNewstedControl.GetType() != nestedControlType) {
					layoutItem.NestedControlContainer.Controls.Clear();
					AddControlToLayoutItem(layoutItem, nestedControlType);
				}
			}
			return result;
		}
		protected override DesignEditorDescriptorItem CreateSubmenuItem(DesignEditorDescriptorItem parent, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			var menuItem = base.CreateSubmenuItem(parent, designTimeItem, isToolbar);
			menuItem.BeginGroup = designTimeItem.ColumnType == typeof(LayoutItemsGroup);
			if(typeof(GroupBase).IsAssignableFrom(designTimeItem.ColumnType)) {
				menuItem.EditorType = DesignEditorDescriptorItemType.DropDown;
				menuItem.ActionType = parent.ActionType;
				PopulateChildItems(menuItem, isToolbar);
			}
			return menuItem;
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(!parentMenuItem.Enabled)
				return false;
			var focusedItem = FocusedItemForAction;
			if(parentMenuItem.ActionType == DesignEditorMenuRootItemActionType.InsertChild) {
				if(focusedItem != null && !IsSupportHierarchy(focusedItem))
					return TypeIsControl(designTimeItem.ColumnType);
			}
			if(IsGroupItemType(designTimeItem.ColumnType))
				return parentMenuItem.ItemType != null && GroupItemTypes.ContainsKey(parentMenuItem.ItemType.ColumnType);
			if(parentMenuItem.ItemType != null && GroupItemTypes.ContainsKey(parentMenuItem.ItemType.ColumnType))
				return GroupItemTypes[parentMenuItem.ItemType.ColumnType].Contains(designTimeItem.ColumnType);
			if(parentMenuItem.ActionType == DesignEditorMenuRootItemActionType.ChangeTo)
				return GetSelectedItemsType() != designTimeItem.ColumnType;
			return true;
		}
		public override IDesignTimeCollectionItem CreateNewItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			var type = designTimeItem != null ? designTimeItem.ColumnType : GetDefaultItemType();
			if(TypeIsControl(type)) {
				var item = Activator.CreateInstance(GetDefaultItemType()) as IDesignTimeCollectionItem;
				AddEditorToLayoutItem(item as LayoutItem, type);
				return item;
			}
			return base.CreateNewItem(designTimeItem);
		}
		protected override void AddItemCore(IDesignTimeColumnAndEditorItem designTimeItem, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(target == null)
				target = RootLayoutGroup;
			if(direction == InsertDirection.Inside && !IsSupportHierarchy(target)) {
				if(designTimeItem == null)
					return;
				var type = designTimeItem.ColumnType;
				if(TypeIsControl(type))
					SelectedItems.Where(i => i is LayoutItem).OfType<LayoutItem>().ToList().ForEach(i => AddEditorToLayoutItem(i as LayoutItem, type));
			} else {
				base.AddItemCore(designTimeItem, target, direction);
			}
		}
		public void MoveFocusedItemTo(IDesignTimeCollectionItem target, InsertDirection direction) {
			BeginUpdate();
			MoveItemTo(FocusedItem, target, direction);
			EndUpdate();
		}
		protected internal override void MoveItemTo(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			UpdateLayoutItemColSpan(source, target, direction);
			base.MoveItemTo(source, target, direction);
		}
		void UpdateLayoutItemColSpan(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(source == null)
				return;
			LayoutGroup group = null;
			if(direction == InsertDirection.Inside)
				group = DetermineLayoutGroup(target);
			else
				group = target.Parent as LayoutGroup;
			if(group != null && group != source.Parent) {
				var item = (LayoutItemBase)source;
				if(group.ColCount < item.ColSpan)
					item.ColSpan = group.ColCount;
			}
		}
		LayoutGroup DetermineLayoutGroup(IDesignTimeCollectionItem item) { 
			return item == null || item is RootLayoutGroupWrapper ? RootLayoutGroup : item as LayoutGroup;
		}
		protected override void RemoveInnerSelectedItemsCore() {
			SelectedItems.ForEach(i => {
				var layoutItem = i as LayoutItem;
				if(layoutItem != null && layoutItem.GetNestedControl() != null)
					layoutItem.NestedControlContainer.Controls.Clear();
			});
		}
		protected override IList FindItemCollection(IDesignTimeCollectionItem item) {
			return FindParentItem(item, RootItemWrapper).Items;
		}
		internal bool TypeIsControl(Type type) {
			return typeof(System.Web.UI.Control).IsAssignableFrom(type);
		}
		public void IncreaseDecreaseFocusedGroupColCount(bool increase) {
			var group = DetermineLayoutGroup(FocusedItem);
			if(group == null)
				return;
			BeginUpdate();
			if(increase) {
				++group.ColCount;
			} else {
				var newColCount = group.ColCount - 1;
				foreach(var item in group.Items) {
					var layoutItem = (LayoutItemBase)item;
					if(layoutItem.ColSpan > newColCount)
						layoutItem.ColSpan = newColCount;
				}
				group.ColCount = newColCount;
			}
			EndUpdate();
		}
		public void IncreaseDecreaseFocusedItemColSpan(bool increase) {
			var item = FocusedItem as LayoutItemBase;
			if(item == null)
				return;
			BeginUpdate();
			if(increase)
				++item.ColSpan;
			else
				--item.ColSpan;
			EndUpdate();
		}
		public void IncreaseDecreaseFocusedItemRowSpan(bool increase) {
			var item = FocusedItem as LayoutItemBase;
			if(item == null)
				return;
			BeginUpdate();
			if(increase)
				++item.RowSpan;
			else
				--item.RowSpan;
			EndUpdate();
		}
		public bool MovingBetweenNeighbours(IDesignTimeCollectionItem movingItem, InsertDirection direction) {
			var neighbour = direction == InsertDirection.After ? TopNeighbourItem : BottomNeighbourItem;
			return neighbour != null && TreeListItemsInvertedHash[neighbour] == TreeListItemsInvertedHash[movingItem];
		}
		public void FillNeighbourItems(IDesignTimeCollectionItem item) {
			var parent = item != null && item.Parent != null ? item.Parent : null;
			if(parent == null) {
				ClearNeighbourItems();
				return;
			}
			var items = parent.Items;
			var orderedItems = items.OfType<IDesignTimeCollectionItem>().OrderBy(i => i.VisibleIndex).ToList();
			var lastIndex = orderedItems.Count - 1;
			var itemIndex = orderedItems.IndexOf(item);
			TopNeighbourItem = itemIndex != 0 ? orderedItems[itemIndex - 1] : null;
			BottomNeighbourItem = itemIndex < lastIndex ? orderedItems[itemIndex + 1] : null;
		}
		protected override void EndUpdate(bool isChanged = true) {
			ClearNeighbourItems();
			base.EndUpdate(isChanged);
		}
		public void ClearNeighbourItems() {
			TopNeighbourItem = null;
			BottomNeighbourItem = null;
		}
		public bool CanMoveInside(IDesignTimeCollectionItem item) {
			return IsSupportHierarchy(item) && item.Items.Count == 0;
		}
		public void GenerateItemsByDataType(Type dataType) { 
				FormLayout.DataType = dataType;
				FormLayout.Items.Clear();
				if(FormLayout.DesignTimeEditingMode)
					FormLayout.DesignTimeNestedControlsStorage.Clear();
				CreateItemsByType(ServiceProvider, formLayout);
				RecreateTreeListItems(true);
		}
		void CreateItemsByType(IServiceProvider serviceProvider, ASPxFormLayout formLayout) {
			var dataType = formLayout.DataType;
			foreach(PropertyInfo propertyInfo in dataType.GetProperties())
				TryCreateLayoutItem(serviceProvider, formLayout, propertyInfo.Name, propertyInfo.PropertyType);
		}
		void TryCreateLayoutItem(IServiceProvider serviceProvider, ASPxFormLayout formLayout, string fieldName, Type dataType) {
			if(NestedControlHelper.IsAllowedDataType(dataType)) {
				LayoutItem layoutItem = new LayoutItem();
				if(formLayout.DesignTimeEditingMode)
					formLayout.DesignTimeNestedControlsStorage[layoutItem] = new List<System.Web.UI.Control>();
				layoutItem.FieldName = fieldName;
				layoutItem.DataType = dataType;
				AddControlToLayoutItemEditorsTempStorage(serviceProvider, formLayout, layoutItem, NestedControlHelper.GetControlTypeByDataType(dataType));
				formLayout.Items.Add(layoutItem);
			}
		}
		void AddControlToLayoutItemEditorsTempStorage(IServiceProvider serviceProvider, ASPxFormLayout formLayout, LayoutItem layoutItem, Type controlType) {
			if(typeof(System.Web.UI.Control).IsAssignableFrom(controlType)) {
				System.Web.UI.Control control = null;
				try {
					object[] attrs = controlType.GetCustomAttributes(typeof(ToolboxDataAttribute), false);
					if(attrs.Length > 0) {
						ToolboxDataAttribute toolboxData = attrs[0] as ToolboxDataAttribute;
						string data = string.Format(toolboxData.Data, ThemesProvider.DefaultTagPrefix);
						control = ControlParser.ParseControl(Designer.DesignerHost, data) as Control;
					}
				} catch {
				}
				if(control == null)
					control = Activator.CreateInstance(controlType) as System.Web.UI.Control;
				if(control != null) {
					if(!string.IsNullOrEmpty(formLayout.ID))
						control.ID = formLayout.GetVacantItemNestedControlID();
					if(formLayout.DesignTimeEditingMode)
						formLayout.DesignTimeNestedControlsStorage[layoutItem].Add(control);
					else
						layoutItem.Controls.Add(control);
					NestedControlHelper.PrepareControl(control, layoutItem.DataType);
				}
			}
		}
	}
	public class RootLayoutGroupWrapper : CollectionItem, IDesignTimeCollectionItem {
		private LayoutGroup layoutGroup;
		public RootLayoutGroupWrapper(LayoutGroup layoutGroup) {
			this.layoutGroup = layoutGroup;
		}
		public int ColCount {
			get { return layoutGroup.ColCount; }
			set { layoutGroup.ColCount = value; }
		}
		public LayoutItemCaptionSettings SettingsItemCaptions {
			get { return layoutGroup.SettingsItemCaptions; }
		}
		public LayoutGroupItemSettings SettingsItems {
			get { return layoutGroup.SettingsItems; }
		}
		public LayoutItemHelpTextSettings SettingsItemHelpTexts {
			get { return layoutGroup.SettingsItemHelpTexts; }
		}
		public RequiredMarkMode RequiredMarkDisplayMode {
			get { return layoutGroup.Owner.RequiredMarkDisplayMode; }
			set { layoutGroup.Owner.RequiredMarkDisplayMode  = value; }
		}
		public string RequiredMark {
			get { return layoutGroup.Owner.RequiredMark; }
			set { layoutGroup.Owner.RequiredMark = value; }
		}
		public string OptionalMark {
			get { return layoutGroup.Owner.OptionalMark; }
			set { layoutGroup.Owner.OptionalMark = value; }
		}
		public bool AlignItemCaptionsInAllGroups {
			get { return layoutGroup.Owner.AlignItemCaptionsInAllGroups; }
			set { layoutGroup.Owner.AlignItemCaptionsInAllGroups = value; }
		}
		public bool ShowItemCaptionColon {
			get { return layoutGroup.Owner.ShowItemCaptionColon; }
			set { layoutGroup.Owner.ShowItemCaptionColon = value; }
		}
		public FormLayoutAdaptivityMode AdaptivityMode {
			get { return layoutGroup.Owner.SettingsAdaptivity.AdaptivityMode; }
			set { layoutGroup.Owner.SettingsAdaptivity.AdaptivityMode = value; }
		}
		void IDesignTimeCollectionItem.Assign(IDesignTimeCollectionItem item) {
			((IDesignTimeCollectionItem)layoutGroup).Assign(item);
		}
		string IDesignTimeCollectionItem.Caption { get { return "Control Layout"; } }
		PropertiesBase IDesignTimeCollectionItem.EditorProperties { get { return null; } }
		string IDesignTimeCollectionItem.FieldName { get { return string.Empty; } set { } }
		string[] IDesignTimeCollectionItem.GetHiddenPropertyNames() { return new string[] { }; }
		IList IDesignTimeCollectionItem.Items { get { return layoutGroup.Items; } }
		IDesignTimeCollectionItem IDesignTimeCollectionItem.Parent { get { return null; } }
		bool IDesignTimeCollectionItem.ReadOnly { get { return true; } }
		bool IDesignTimeCollectionItem.Visible { get { return true; } set { } }
		int IDesignTimeCollectionItem.VisibleIndex { get { return -1; } set { } }
	}
}

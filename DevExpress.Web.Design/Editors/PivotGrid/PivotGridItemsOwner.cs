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
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.Design;
using DevExpress.XtraPivotGrid;
namespace DevExpress.Web.ASPxPivotGrid.Design {
	public class PivotGridItemsOwner : DataItemsEditorOwner, IOwnerEditingProperty {
		IList items;
		List<IDesignTimeCollectionItem> undoGroups;
		PivotGridDesignTimeArea hiddenArea;
		public PivotGridItemsOwner(object pivotGrid, IServiceProvider provider)
			: base(pivotGrid, "Fields", provider, null) {
		}
		internal PivotGridItemsOwner(ASPxPivotGrid pivotGrid)
			: base(pivotGrid, "Fields", null, null) {
		}
		protected internal override IList Items {
			get {
				if(items == null)
					RegenerateItems();
				return items;
			}
		}
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		protected internal PivotGridDesignTimeArea HiddenArea {
			get {
				if(hiddenArea == null)
					hiddenArea = Items.OfType<PivotGridDesignTimeArea>().First(a => a.DesignTimeArea == PivotGridDesignTimeAreaTypes.HiddenArea);
				return hiddenArea;
			}
		}
		public ASPxPivotGrid PivotGrid { get { return (ASPxPivotGrid)Component; } }
		bool CanMoveSelection { get { return !IsReadOnlySelection && SelectedItems.Count(i => DenyedMoveTypes.Contains(i.GetType()) == true) == 0; } }
		protected internal List<IDesignTimeCollectionItem> UndoGroups {
			get {
				if(undoGroups == null)
					undoGroups = new List<IDesignTimeCollectionItem>();
				return undoGroups;
			}
		}
		object IOwnerEditingProperty.PropertyInstance { get { return PivotGrid.Fields; } }
		public bool HasOLAPDataSource { get { return !string.IsNullOrEmpty(PivotGrid.OLAPConnectionString); } }
		protected override void PopulateFieldInfoList(List<DesignTimeFieldInfo> list) {
			if(string.IsNullOrEmpty(PivotGrid.OLAPConnectionString))
				base.PopulateFieldInfoList(list);
		}
		public object GetOLAPDataSource() {
			if(string.IsNullOrEmpty(PivotGrid.OLAPConnectionString))
				return null;
			Data.OLAPConnectionString = PivotGrid.OLAPConnectionString;
			return Data.GetDesignOLAPDataSourceObject();
		}
		protected override DesignEditorDescriptorItem CreateRetriveFieldsMenuItem() {
			var item = base.CreateRetriveFieldsMenuItem();
			item.Enabled |= HasOLAPDataSource;
			return item;
		}
		IList PopulateItems() {
			var result = new List<IDesignTimeCollectionItem>();
			foreach(PivotGridDesignTimeAreaTypes area in Enum.GetValues(typeof(PivotGridDesignTimeAreaTypes)))
				result.Add(new PivotGridDesignTimeArea(area, PivotGrid));
			return (IList)result;
		}
		void RegenerateItems() {
			items = PopulateItems();
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(PivotGridDesignTimeArea), "PivotGrid Area", PivotGridAreaItemImageResource);
			AddItemType(typeof(PivotGridWebGroup), "PivotGrid Group", PivotGridGroupItemImageResource);
			AddItemType(typeof(PivotGridField), "PivotGrid Field", PivotGridFieldItemImageResource);
		}
		protected override void FillDenyMoveTypes() {
			DenyedMoveTypes.Add(typeof(PivotGridDesignTimeArea));
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Fields and Groups";
		}
		public override void SaveUndo() {
			var grouppedFields = PivotGrid.Fields.OfType<PivotGridField>().GroupBy(f => f.Group);
			foreach(var group in grouppedFields) {
				var undoFields = group.OfType<PivotGridField>().ToArray();
				var undoList = CloneListTo(UndoItems, undoFields, false);
				if(group.Key != null) {
					var undoGroup = (PivotGridWebGroup)CloneItemTo(UndoGroups, group.Key);
					undoGroup.AddRange(undoList.OfType<PivotGridField>().ToArray());
				}
			}
		}
		public override void UndoChanges() {
			ReplaceList(PivotGrid.Fields, UndoItems);
			ReplaceList(PivotGrid.Groups, UndoGroups);
		}
		public override void SelectedItemsPropertyChanged(string propertyName) {
			if(propertyName == "Area")
				SynchronizeSelectedFieldsArea();
		}
		void SynchronizeSelectedFieldsArea() {
			var fields = SelectedItems.OfType<PivotGridField>().ToList();
			fields.ForEach(f => {
				var area = FindArea(f.Area);
				if(!area.HasItem(f))
					MoveItemTo(f, area, InsertDirection.Inside);
			});
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(!base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar))
				return false;
			var focusedItem = FocusedItemForAction;
			if(focusedItem == null)
				return false;
			switch(parentMenuItem.ActionType) {
				case DesignEditorMenuRootItemActionType.AddItem:
				case DesignEditorMenuRootItemActionType.InsertBefore:
					var parentFocusedItem = FindParentItem(focusedItem);
					var parentType = parentFocusedItem != null ? parentFocusedItem.GetType() : null;
					return CanAssociate(designTimeItem.ColumnType, parentType, true);
				case DesignEditorMenuRootItemActionType.InsertChild:
					return CanAssociate(designTimeItem.ColumnType, focusedItem.GetType(), true);
			}
			return false;
		}
		public override Type GetDefaultItemType() {
			return typeof(PivotGridField);
		}
		public override List<string> GetViewDependedProperties() {
			var list = base.GetViewDependedProperties();
			list.AddRange(new string[] { 
				"Area",
				"AreaIndex",
				"FieldName", 
				"Caption", 
				"Visible", 
				"VisibleIndex"
			});
			return list;
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.AddItem,
				DesignEditorMenuRootItemActionType.InsertBefore, 
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.MoveToNewGroup,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp, 
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.MoveLeft,
				DesignEditorMenuRootItemActionType.MoveRight,
				DesignEditorMenuRootItemActionType.RetriveFields
			};
		}
		protected override List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveToNewGroup,
				DesignEditorMenuRootItemActionType.MoveUp, 
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.MoveLeft, 
				DesignEditorMenuRootItemActionType.MoveRight
			};
		}
		public override bool CanMoveItem(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(!base.CanMoveItem(source, target, direction) || !CanChangeLocationTo(source, target))
				return false;
			if(direction == InsertDirection.Inside)
				return CanAssociate(source, target);
			var sourceType = source.GetType();
			var targetType = target != null ? target.GetType() : null;
			if(sourceType == typeof(PivotGridWebGroup) && targetType == typeof(PivotGridField))
				return FindFieldParent(target).GetType() == typeof(PivotGridDesignTimeArea);
			return sourceType != typeof(PivotGridDesignTimeArea) && targetType != typeof(PivotGridDesignTimeArea);
		}
		bool CanChangeLocationTo(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target) {
			if(ItemIsHiddenArea(FindArea(source)) && ItemIsHiddenArea(FindArea(target)))
				return false;
			if(!HasOLAPDataSource)
				return true;
			var sourceField = source as PivotGridField;
			if(sourceField != null)
				return sourceField.CanChangeLocationTo(FindArea(target).Area, GetAreaIndex(target));
			var group = source as PivotGridWebGroup;
			return group != null ? group.CanChangeAreaTo(FindArea(target).Area, GetAreaIndex(target)) : true;
		}
		int GetAreaIndex(IDesignTimeCollectionItem target) {
			if(target is PivotGridField) return ((PivotGridField)target).AreaIndex;
			if(target is PivotGridWebGroup) return ((PivotGridWebGroup)target).AreaIndex;
			return -1;
		}
		IDesignTimeCollectionItem FindFieldParent(IDesignTimeCollectionItem field) {
			return field.Parent != null ? field.Parent : FindArea(field);
		}
		protected override bool CanPopulateDecreaseIndentLevel(IDesignTimeCollectionItem item, IDesignTimeCollectionItem parent) {
			if(!IsSupportHierarchy(parent))
				return false;
			return parent.GetType() == typeof(PivotGridWebGroup) && item.GetType() == typeof(PivotGridField);
		}
		protected bool CanAssociate(IDesignTimeCollectionItem child, IDesignTimeCollectionItem parent) {
			var childType = child != null ? child.GetType() : null;
			var parentType = parent != null ? parent.GetType() : null;
			return CanAssociate(childType, parentType);
		}
		protected bool CanAssociate(Type childType, Type parentType, bool fromMenu = false) {
			if(parentType == null || childType == typeof(PivotGridDesignTimeArea))
				return false;
			if(fromMenu)
				return childType == typeof(PivotGridField);
			if(parentType == typeof(PivotGridWebGroup))
				return childType == typeof(PivotGridField);
			if(parentType == typeof(PivotGridDesignTimeArea))
				return childType == typeof(PivotGridField) || childType == typeof(PivotGridWebGroup);
			return false;
		}
		protected override void ResetItemVisibleIndex(IDesignTimeCollectionItem item, IList itemCollection) {
			item.VisibleIndex = itemCollection.Count;
		}
		protected override DesignEditorDescriptorItem CreateAddItemMenuItem() {
			var item = base.CreateAddItemMenuItem();
			item.Enabled = !IsReadOnlySelection;
			item.EditorType = DesignEditorDescriptorItemType.Button;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateInsertBeforeMenuItem() {
			var item = base.CreateInsertBeforeMenuItem();
			item.Enabled = !IsReadOnlySelection;
			item.EditorType = DesignEditorDescriptorItemType.Button;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateMoveUpMenuItem(bool isToolbarMenu) {
			var item = base.CreateMoveUpMenuItem(isToolbarMenu);
			if(item.Enabled)
				item.Enabled = CanMoveSelection;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateMoveDownMenuItem(bool isToolbarMenu) {
			var item = base.CreateMoveDownMenuItem(isToolbarMenu);
			if(item.Enabled)
				item.Enabled = CanMoveSelection;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateIncreaseIndentMenuItem() {
			var item = base.CreateIncreaseIndentMenuItem();
			if(item.Enabled)
				item.Enabled = IsSelectedOnlyThisType(typeof(PivotGridField));
			return item;
		}
		protected override DesignEditorDescriptorItem CreateDecreaseIndentMenuItem() {
			var item = base.CreateDecreaseIndentMenuItem();
			if(item.Enabled)
				item.Enabled = IsSelectedOnlyThisType(typeof(PivotGridField));
			return item;
		}
		protected override DesignEditorDescriptorItem CreateChangeToMenuItem(bool isToolbar) {
			var item = base.CreateChangeToMenuItem(isToolbar);
			item.Enabled = !IsReadOnlySelection;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateRemoveMenuItem(bool isToolbarMenu) {
			var item = base.CreateRemoveMenuItem(isToolbarMenu);
			item.Enabled = !IsReadOnlySelection;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateInsertChildMenuItem(bool isToolbar) {
			var item = base.CreateInsertChildMenuItem(isToolbar);
			if(isToolbar) {
				item.EditorType = DesignEditorDescriptorItemType.Button;
				return item;
			}
			item.Enabled = item.Enabled && FocusedItemForAction != null;
			if(item.ChildItems.Count == 1) {
				var childItem = item.ChildItems[0];
				childItem.Caption = string.Format("Add {0}", childItem.ItemType.ColumnType.Name);
				item = childItem;
			}
			return item;
		}
		public override IDesignTimeCollectionItem CreateNewItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			var item = base.CreateNewItem(designTimeItem);
			var itemType = item.GetType();
			if(itemType == typeof(PivotGridField))
				AddPivotGridField((PivotGridField)item);
			else if(itemType == typeof(PivotGridWebGroup))
				PivotGrid.Groups.Add((PivotGridWebGroup)item);
			return item;
		}
		protected PivotGridField AddPivotGridField(PivotGridField field) {
			field.ID = PivotGrid.Fields.GenerateName(field.FieldName);
			PivotGrid.Fields.Add(field);
			return field;
		}
		List<PivotGridField> fieldsForGrouping;
		List<PivotGridField> FieldsForGrouping {
			get {
				if(fieldsForGrouping == null)
					fieldsForGrouping = new List<PivotGridField>();
				return fieldsForGrouping;
			}
		}
		protected override bool IsEnabledMoveToNewGroupMenuItem() {
			FieldsForGrouping.Clear();
			SelectedItems.OfType<PivotGridField>().ToList().ForEach(f => {
				var itemArea = GetItemDesignTimeArea(f);
				if(itemArea == null || !itemArea.IsHiddenArea)
					FieldsForGrouping.Add(f);
			});
			var count = FieldsForGrouping.Count;
			if(count == 0)
				return false;
			var field = FieldsForGrouping[0];
			var group = field.Group as PivotGridWebGroup;
			if(group != null) {
				var groupCount = FieldsForGrouping.Count(f => f.Group == group);
				if(groupCount == group.Count && groupCount == count)
					return false;
			}
			return true;
		}
		protected override void RemoveItem(IDesignTimeCollectionItem item) {
			base.RemoveItem(item);
			if(item is PivotGridField)
				RemovePivotField(item);
			else if(item is PivotGridWebGroup)
				RemovePivotGroup((PivotGridWebGroup)item);
		}
		protected void RemovePivotGroup(PivotGridWebGroup group) {
			var fieldsToRemove = new List<PivotGridField>();
			fieldsToRemove.AddRange(group.Fields.OfType<PivotGridField>());
			fieldsToRemove.ForEach(f => RemovePivotField(f));
			if(PivotGrid.Groups.Contains(group))
				PivotGrid.Groups.Remove(group);
		}
		protected void RemovePivotField(IDesignTimeCollectionItem item) {
			var field = (PivotGridField)item;
			if(PivotGrid.Fields.Contains(field))
				PivotGrid.Fields.Remove(field);
		}
		public void MoveSelectedFieldsToNewGroup() {
			if(FieldsForGrouping.Count == 0)
				return;
			BeginUpdate();
			var group = (PivotGridWebGroup)CreateNewItem(FindDesignTimeItem(typeof(PivotGridWebGroup)));
			var insertDirection = InsertDirection.After;
			var neighbor = FindNextNotSelectedItem();
			if(neighbor is PivotGridField) {
				var neigborGroup = ((PivotGridField)neighbor).Group as PivotGridWebGroup;
				if(neigborGroup != null)
					neighbor = neigborGroup;
			} else if(neighbor is PivotGridDesignTimeArea) {
				neighbor = AreaForDataItem;
				insertDirection = InsertDirection.Inside;
			}
			MoveItemTo(group, neighbor, insertDirection);
			FieldsForGrouping.ForEach(i => MoveItemTo(i, group, InsertDirection.Inside));
			FieldsForGrouping.Clear();
			EndUpdate();
		}
		protected override void EndUpdate(bool isChanged = true) {
			ProcessHiddenItems();
			base.EndUpdate(isChanged);
		}
		void ProcessHiddenItems() {
			Items.OfType<PivotGridDesignTimeArea>().ToList().ForEach(a => a.Items.ToList().ForEach(i => {
				var moveToHiddenArea = !i.Visible;
				if(a.IsHiddenArea && !moveToHiddenArea)
					MoveFromHiddenArea(i);
				else if(moveToHiddenArea)
					MoveToHiddenArea(i);
			}));
		}
		void MoveFromHiddenArea(IDesignTimeCollectionItem source) {
			var targetArea = GetItemDesignTimeArea(source);
			if(targetArea != null)
				MoveItemTo(source, targetArea, InsertDirection.Inside);
		}
		void MoveToHiddenArea(IDesignTimeCollectionItem item) {
			var visibleIndex = item.VisibleIndex;
			MoveItemToCore(item, HiddenArea, InsertDirection.Inside);
			item.VisibleIndex = visibleIndex;
			SetPivotGridItemVisible(item, false);
		}
		protected void SetPivotGridItemVisible(IDesignTimeCollectionItem item, bool visible) {
			var group = item as PivotGridWebGroup;
			if(group != null && group.Fields.Count != 0)
				group.Fields.OfType<IDesignTimeCollectionItem>().OrderBy(i => i.VisibleIndex).ToArray()[0].Visible = visible;
			else if(item is PivotGridField)
				((PivotGridField)item).Visible = visible;
		}
		protected void SetPivotGridItemVisibleIndex(IDesignTimeCollectionItem item, int visibleIndex) {
			var group = item as PivotGridWebGroup;
			if(group != null && group.Fields.Count != 0)
				((IDesignTimeCollectionItem)group.Fields[0]).VisibleIndex = visibleIndex;
			else if(item is PivotGridField)
				item.VisibleIndex = visibleIndex;
		}
		protected void MoveDataItemTo(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(!CanChangeLocationTo(source, target))
				return;
			Data.BeginUpdate();
			MoveItemTo(source, AreaForDataItem, InsertDirection.Inside);
			Data.EndUpdate();
			RegenerateItems();
		}
		protected internal override void MoveItemTo(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(!CanMoveItem(source, target, direction))
				return;
			if(ItemIsHiddenArea(target)) {
				MoveToHiddenArea(source);
				return;
			}
			if(source is PivotGridWebGroup)
				MovePivotGridGroupTo((PivotGridWebGroup)source, target, direction);
			else if(source is PivotGridField)
				MovePivotGridFieldTo((PivotGridField)source, target, direction);
		}
		protected void MovePivotGridFieldTo(PivotGridField field, IDesignTimeCollectionItem target, InsertDirection direction) {
			var fieldVisibility = field.Visible;
			var group = GetPivotGroupFromDesignTimeItem(field);
			var needUpdateGroupVisibility = group != null && group.Fields.Count > 1 && group.Fields[0] == field;
			if(needUpdateGroupVisibility) {
				SetPivotGridItemVisible(field, !ItemIsHiddenArea(GetItemDesignTimeArea(target)));
				MoveItemToCore(field, target, direction);
				UpdateMovingItemGroupVisibility(group, target, fieldVisibility);
				return;
			}
			var targetArea = GetItemDesignTimeArea(target);
			fieldVisibility = targetArea != null && targetArea != HiddenArea;
			MoveItemToCore(field, target, direction);
			field.Visible = fieldVisibility;
		}
		protected void MovePivotGridGroupTo(PivotGridWebGroup group, IDesignTimeCollectionItem target, InsertDirection direction) {
			var visible = group.Visible;
			SetPivotGridItemVisible(group, !ItemIsHiddenArea(GetItemDesignTimeArea(target)));
			MoveItemToCore(group, target, direction);
		}
		void UpdateMovingItemGroupVisibility(PivotGridWebGroup group, IDesignTimeCollectionItem target, bool visible) {
			if(target == null || group == null || group.Fields.Count == 0)
				return;
			if(GetPivotGroupFromDesignTimeItem(target) == group)
				return;
			SetPivotGridItemVisible(group, visible);
		}
		PivotGridWebGroup GetPivotGroupFromDesignTimeItem(IDesignTimeCollectionItem item) {
			var field = item as PivotGridField;
			if(field != null)
				return (PivotGridWebGroup)field.Group;
			var group = item as PivotGridWebGroup;
			return group != null ? group : null;
		}
		protected void MoveItemToCore(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			SetPivotGridArea(source, target);
			var savedIndex = source.VisibleIndex;
			if(source is PivotGridField && target is PivotGridField)
				MoveFields(source, target, direction);
			else
				base.MoveItemTo(source, target, direction);
			var itemArea = GetItemDesignTimeArea(source);
			if(itemArea != null && itemArea.IsHiddenArea)
				SetPivotGridItemVisibleIndex(source, savedIndex);
		}
		protected bool ItemIsHiddenArea(IDesignTimeCollectionItem item) {
			return item != null && item is PivotGridDesignTimeArea && ((PivotGridDesignTimeArea)item).DesignTimeArea == PivotGridDesignTimeAreaTypes.HiddenArea;
		}
		protected void MoveFields(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(source == target)
				return;
			var sourceGroup = ((PivotGridField)source).Group as IDesignTimeCollectionItem;
			var targetGroup = ((PivotGridField)target).Group as IDesignTimeCollectionItem;
			bool movingInGroup = sourceGroup != null && sourceGroup == targetGroup;
			List<IDesignTimeCollectionItem> groupItems = new List<IDesignTimeCollectionItem>();
			if(movingInGroup)
				groupItems = DecreaseGroupFieldsLevel(targetGroup);
			base.MoveItemTo(source, target, direction);
			if(movingInGroup)
				MoveFieldsToGroup(groupItems, targetGroup);
		}
		List<IDesignTimeCollectionItem> DecreaseGroupFieldsLevel(IDesignTimeCollectionItem group) {
			List<IDesignTimeCollectionItem> groupItems = new List<IDesignTimeCollectionItem>();
			groupItems.AddRange(group.Items.OfType<IDesignTimeCollectionItem>().OrderBy(i => i.VisibleIndex));
			var lastItem = groupItems[groupItems.Count - 1];
			MoveItemTo(lastItem, group, InsertDirection.Before);
			for(int i = groupItems.Count - 2; i >= 0; --i) {
				MoveItemTo(groupItems[i], lastItem, InsertDirection.Before);
				lastItem = groupItems[i];
			}
			return groupItems;
		}
		void MoveFieldsToGroup(List<IDesignTimeCollectionItem> fields, IDesignTimeCollectionItem group) {
			var orderedList = fields.OrderBy(i => i.VisibleIndex).ToList();
			var groupVisibleIndex = orderedList[0].VisibleIndex;
			var lastItem = orderedList[0];
			var borderItem = FindPreviosItemInArea(lastItem);
			MoveItemTo(lastItem, group, InsertDirection.Inside);
			for(int i = 1; i < orderedList.Count; ++i)
				base.MoveItemTo(orderedList[i], lastItem, InsertDirection.After);
			if(borderItem != null)
				MoveItemTo(group, borderItem, InsertDirection.After);
		}
		protected IDesignTimeCollectionItem FindPreviosItemInArea(IDesignTimeCollectionItem item) {
			var orderedAreaItems = FindArea(item).Items.OrderBy(i => i.VisibleIndex).ToList();
			var index = orderedAreaItems.IndexOf(item) - 1;
			return index < 0 ? null : orderedAreaItems[index];
		}
		PivotGridDesignTimeArea GetItemDesignTimeArea(IDesignTimeCollectionItem item) {
			if(item is PivotGridDesignTimeArea) return (PivotGridDesignTimeArea) item;
			if(!item.Visible && HiddenArea.HasItem(item)) return HiddenArea;
			if(item is PivotGridField) return FindArea(((PivotGridField)item).Area);
			if(item is PivotGridWebGroup) return FindArea(((PivotGridWebGroup)item).Area);
			return null;
		}
		PivotGridDesignTimeArea FindArea(PivotArea areaType) {
			return Items.OfType<PivotGridDesignTimeArea>().FirstOrDefault(a => a.Area == areaType);
		}
		PivotGridDesignTimeArea FindArea(IDesignTimeCollectionItem targetItem) {
			while(targetItem != null) {
				if(targetItem is PivotGridDesignTimeArea)
					return (PivotGridDesignTimeArea)targetItem;
				targetItem = FindParentItem(targetItem);
			}
			return null;
		}
		protected override IList FindItemCollection(IDesignTimeCollectionItem item) {
			var result = FindInnerItemCollection(item, null);
			return result != null ? result : base.FindItemCollection(item);
		}
		protected override IDesignTimeCollectionItem FindParentItem(IDesignTimeCollectionItem itemToSearch, IDesignTimeCollectionItem parent = null) {
			return IterateCollection(itemToSearch, parent, (item, compareWith, parentItem, items) => { return item == compareWith ? parentItem : null; });
		}
		IList FindInnerItemCollection(IDesignTimeCollectionItem itemToSearch, IDesignTimeCollectionItem parent) {
			return IterateCollection(itemToSearch, parent, (item, compareWith, parentItem, items) => { return item == compareWith ? items : null; });
		}
		T IterateCollection<T>(IDesignTimeCollectionItem itemToSearch, IDesignTimeCollectionItem parent, Func<IDesignTimeCollectionItem, IDesignTimeCollectionItem, IDesignTimeCollectionItem, IList, T> compareFunc) {
			var items = parent == null ? Items : parent.Items;
			if(items == null)
				return default(T);
			T result = default(T);
			foreach(IDesignTimeCollectionItem item in items) {
				result = compareFunc(itemToSearch, item, parent, items);
				if(result == null)
					result = IterateCollection(itemToSearch, item, compareFunc);
				if(result != null)
					break;
			}
			return result;
		}
		void SetPivotGridArea(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target) {
			var area = FindArea(target);
			if(area == null || area.DesignTimeArea == PivotGridDesignTimeAreaTypes.HiddenArea)
				return;
			var group = source as PivotGridWebGroup;
			if(group != null) {
				SetPivotGroupArea(group, area.Area);
			} else {
				var field = source as PivotGridField;
				if(field != null)
					field.Area = area.Area;
			}
		}
		void SetPivotGroupArea(PivotGridWebGroup group, PivotArea area) {
			var fields = new List<PivotGridField>();
			fields.AddRange(group.Fields.OfType<PivotGridField>());
			group.Clear();
			foreach(var field in fields)
				field.Area = area;
			group.AddRange(fields.ToArray());
		}
		protected PivotGridDesignTimeArea AreaForDataItem {
			get {
				var result = FindArea(FocusedItem);
				if(result == null)
					result = Items.OfType<PivotGridDesignTimeArea>().First(a => a.Area == PivotArea.DataArea);
				return result;
			}
		}
		protected override string KeyFieldName { get { return string.Empty; } set { } }
		public void CreateDataFields(List<string> fieldNames) {
			if(fieldNames.Count != 0) {
				BeginUpdate();
				fieldNames.ForEach(f => MoveDataItemTo(CreateDataItemCore(f), AreaForDataItem, InsertDirection.Inside));
				EndUpdate();
			}
		}
		public override void CreateDataItemWithoutUpdate(string fieldName) {
			MoveDataItemTo(CreateDataItemCore(fieldName), AreaForDataItem, InsertDirection.Inside);
		}
		protected override IDesignTimeCollectionItem CreateDataItemCore(string fieldName) {
			return AddPivotGridField(new PivotGridField(fieldName, AreaForDataItem.Area));
		}
		public void RemoveOlapDataItem(string fieldName) {
			var field = PivotGrid.Fields.OfType<PivotGridField>().FirstOrDefault(f => f.FieldName == fieldName);
			if(field != null) {
				BeginUpdate();			
				var group = (PivotGridWebGroup)PivotGrid.Groups.GetGroupByField(field);
				if(group != null && group.Count == 1)
					RemoveItem(group);
				else
					RemoveItem(field);
				EndUpdate();
			}
		}
	}
	public enum PivotGridDesignTimeAreaTypes {
		RowArea = 0,
		ColumnArea = 1,
		FilterArea = 2,
		DataArea = 3,
		HiddenArea = 4
	}
	public class PivotGridDesignTimeArea : IDesignTimeCollectionItem {
		List<IDesignTimeCollectionItem> items;
		public PivotGridDesignTimeArea(PivotGridDesignTimeAreaTypes area, ASPxPivotGrid pivotGrid) {
			DesignTimeArea = area;
			PivotGrid = pivotGrid;
			FillItems();
		}
		[Browsable(false)]
		public PivotGridDesignTimeAreaTypes DesignTimeArea { get; set; }
		[Browsable(false)]
		public PivotArea Area { get { return DesignTimeArea != PivotGridDesignTimeAreaTypes.HiddenArea ? (PivotArea)DesignTimeArea : PivotArea.DataArea; } }
		[Browsable(false)]
		public List<IDesignTimeCollectionItem> Items {
			get {
				if(items == null)
					items = new List<IDesignTimeCollectionItem>();
				return items;
			}
		}
		[Browsable(false)]
		public List<IDesignTimeCollectionItem> OrderedItems { get { return Items.OrderBy(i => i.VisibleIndex).ToList(); } }
		[Browsable(false)]
		public bool IsHiddenArea { get { return DesignTimeArea == PivotGridDesignTimeAreaTypes.HiddenArea; } }
		ASPxPivotGrid PivotGrid { get; set; }
		string IDesignTimeCollectionItem.Caption { get { return DesignTimeArea.ToString(); } }
		PropertiesBase IDesignTimeCollectionItem.EditorProperties { get { return null; } }
		string IDesignTimeCollectionItem.FieldName { get { return string.Empty; } set { } }
		IList IDesignTimeCollectionItem.Items { get { return this.Items; } }
		IDesignTimeCollectionItem IDesignTimeCollectionItem.Parent { get { return null; } }
		bool IDesignTimeCollectionItem.ReadOnly { get { return true; } }
		bool IDesignTimeCollectionItem.Visible { get { return true; } set { } }
		int IDesignTimeCollectionItem.VisibleIndex { get { return GetVisibleIndex(); } set { } }
		void IDesignTimeCollectionItem.Assign(IDesignTimeCollectionItem item) { }
		string[] IDesignTimeCollectionItem.GetHiddenPropertyNames() {
			return new string[] { };
		}
		public bool HasItem(IDesignTimeCollectionItem target) {
			return FindItem(Items, target) != null;
		}
		int GetVisibleIndex() {
			int areaOrder = 0;
			switch(DesignTimeArea) { 
				case PivotGridDesignTimeAreaTypes.DataArea:
					areaOrder = 0;
					break;
				case PivotGridDesignTimeAreaTypes.RowArea:
					areaOrder = 1;
					break;
				case PivotGridDesignTimeAreaTypes.ColumnArea:
					areaOrder = 2;
					break;
				case PivotGridDesignTimeAreaTypes.FilterArea:
					areaOrder = 3;
					break;
				case PivotGridDesignTimeAreaTypes.HiddenArea:
					areaOrder = 4;
					break;
			}
			return -1 * Enum.GetValues(typeof(PivotArea)).Length + areaOrder;
		}
		IDesignTimeCollectionItem FindItem(IList items, IDesignTimeCollectionItem target) {
			if(items == null)
				return null;
			foreach(IDesignTimeCollectionItem item in items) {
				if(item == target)
					return item;
				var result = FindItem(item.Items, target);
				if(result != null)
					return result;
			}
			return null;
		}
		void FillItems() {
			var itemVisibility = DesignTimeArea != PivotGridDesignTimeAreaTypes.HiddenArea;
			Items.AddRange(PivotGrid.Fields.OfType<PivotGridField>().Where(f => (!itemVisibility || f.Area == Area) && f.Visible == itemVisibility &&  PivotGrid.Groups.GetGroupByField(f) == null));
			Items.AddRange(PivotGrid.Groups.OfType<PivotGridWebGroup>().Where(g => (!itemVisibility || g.Area == Area) && g.Visible == itemVisibility));
		}
	}
}

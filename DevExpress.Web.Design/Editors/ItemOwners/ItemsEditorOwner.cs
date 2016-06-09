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
using System.ComponentModel.Design;
using System.Linq;
using System.Web.UI.Design;
using DevExpress.Web.Internal;
using System.Web.UI;
using System.Reflection;
namespace DevExpress.Web.Design {
	public delegate TreeListNodesState OnBeginDataOwnerUpdate();
	public delegate void OnEndDataOwnerUpdate(TreeListNodesState validatedState);
	public delegate void OnUpdateResourceImageMap(string resourceUrl);
	public delegate void OnComponentChanged(ComponentChangedEventArgs e);
	public delegate void OnUpdateView();
	public class HierarchicalItemOwner<T> : ItemsEditorOwner {
		public HierarchicalItemOwner(object component, IServiceProvider provider, Collection items)
			: base(component, "Items", provider, items) {
		}
		internal HierarchicalItemOwner(Collection items)
			: base(null, null, null, items) {
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(T), "Item");
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
				DesignEditorMenuRootItemActionType.MoveRight
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
				DesignEditorMenuRootItemActionType.SelectAll
			};
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
			return item;
		}
		protected override void FillMenuItemResouceImages() {
			base.FillMenuItemResouceImages();
			AddResourceImage(AddChildItemImageResource);
		}
		public override Type GetDefaultItemType() {
			return typeof(T);
		}
	}
	public class FlatCollectionItemsOwner<T> : FlatCollectionOwner {
		public FlatCollectionItemsOwner(object component, IServiceProvider provider, Collection items)
			: this(component, provider, items, "Items") {
		}
		public FlatCollectionItemsOwner(object component, IServiceProvider provider, Collection items, string navBarGroupItemsName)
			: base(component, "Items", provider, items) {
			NavBarItemsGroupName = navBarGroupItemsName;
		}
		string NavBarItemsGroupName { get; set; }
		protected override void FillItemTypes() {
			AddItemType(typeof(T), "Items", string.Empty);
		}
		public override Type GetDefaultItemType() {
			return typeof(T);
		}
		protected internal override string GetNavBarItemsGroupName() {
			return !string.IsNullOrEmpty(NavBarItemsGroupName) ? NavBarItemsGroupName : base.GetNavBarItemsGroupName();
		}
	}
	public abstract class FlatCollectionOwner : ItemsEditorOwner {
		public FlatCollectionOwner(object component, string collectionPropertyName, IServiceProvider provider, Collection items)
			: base(component, collectionPropertyName, provider, items) {
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.AddItem,
				DesignEditorMenuRootItemActionType.InsertBefore, 
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
		protected override void FillMenuItemResouceImages() {
			base.FillMenuItemResouceImages();
			AddResourceImage(AddChildItemImageResource);
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Items";
		}
		protected internal override string GetNavBarItemsGroupDescription() {
			return string.Format("Edit {0} displayed in the {1}", GetNavBarItemsGroupName(), Component.GetType().Name);
		}
	}
	public abstract class DataItemsEditorOwner : ItemsEditorOwner {
		List<DesignTimeFieldInfo> fieldInfoList;
		IDataSourceViewSchema designerSchema;
		public DataItemsEditorOwner(object component, string collectionPropertyName, IServiceProvider provider, IList items)
			: base(component, collectionPropertyName, provider, items) {
		}
		public override bool SupportDataSource { get { return true; } }
		public List<DesignTimeFieldInfo> FieldInfoList {
			get {
				if(fieldInfoList == null) {
					fieldInfoList = new List<DesignTimeFieldInfo>();
					PopulateFieldInfoList(fieldInfoList);
				}
				return fieldInfoList;
			}
			private set { fieldInfoList = value; }
		}
		protected abstract string KeyFieldName { get; set; }
		protected abstract IDesignTimeCollectionItem CreateDataItemCore(string fieldName);
		protected IDataSourceViewSchema DesignerSchema {
			get {
				if(designerSchema != null)
					return designerSchema;
				var dataDesigner = Designer as ASPxDataWebControlDesigner;
				if(dataDesigner == null)
					return null;
				var designerView = dataDesigner.DesignerView;
				if(designerView != null) {
					try {
						if(designerView.Schema == null && designerView.DataSourceDesigner != null)
							designerView.DataSourceDesigner.RefreshSchema(true);
						designerSchema = designerView.Schema;
					} catch {
						designerSchema = null;
					}
				}
				return designerSchema;
			}
		}
		protected virtual void PopulateFieldInfoList(List<DesignTimeFieldInfo> list) {
			if(DesignerSchema == null)
				return;
			foreach(var field in DesignerSchema.GetFields()) {
				list.Add(new DesignTimeFieldInfo(field.Name, field.DataType) {
					IsBindableType = GridViewDesignerHelper.IsBindableType(field.DataType),
					IsPrimaryKey = field.PrimaryKey,
					IsReadOnly = field.IsReadOnly,
					Identity = field.Identity
				});
			}
		}
		public virtual void CreateDataItems(List<string> fields) { 
			BeginUpdate();
			fields.ForEach(f => CreateDataItemWithoutUpdate(f));
			EndUpdate();
		}
		public virtual void CreateDataItem(string fieldName) {
			BeginUpdate();
			CreateDataItemWithoutUpdate(fieldName);
			EndUpdate();
		}
		public virtual void CreateDataItemWithoutUpdate(string fieldName) {
			var column = CreateDataItemCore(fieldName);
			MoveItemTo(column, null, InsertDirection.Inside);
			UpdateKeyFieldName();
		}
		public virtual void RemoveDataItem(string fieldName) {
			BeginUpdate();
			var column = TreeListItemsHash.Values.OfType<IDesignTimeCollectionItem>().FirstOrDefault(c => c.FieldName == fieldName);
			if(column != null)
				RemoveItem(column);
			EndUpdate();
		}
		void UpdateKeyFieldName() {
			if(TreeListItemsHash.Values.OfType<IDesignTimeCollectionItem>().Any(c => c.FieldName == KeyFieldName))
				return;
			var keyFieldName = FieldInfoList.Where(f => f.IsPrimaryKey).Select(f => f.Name).FirstOrDefault();
			if(!string.IsNullOrEmpty(keyFieldName))
				KeyFieldName = KeyFieldName;
		}
		public void ResetDataSourceFieldInfo() {
			FieldInfoList = null;
		}
		protected override DesignEditorDescriptorItem CreateEditorDescriptorItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(actionType == DesignEditorMenuRootItemActionType.RetriveFields)
				return CreateRetriveFieldsMenuItem();
			return base.CreateEditorDescriptorItem(actionType, isToolbarMenu);
		}
		protected virtual DesignEditorDescriptorItem CreateRetriveFieldsMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.RetriveFields };
			item.Caption = "Retrieve Fields";
			item.EditorType = DesignEditorDescriptorItemType.DropDown;
			item.Enabled = FieldInfoList.Count > 0;
			item.ImageIndex = GetResourceImageIndex(RetrieveFieldsItemImageResource);
			return item;
		}
		public List<string> GetUsedFieldNames() {
			return TreeListItemsInvertedHash.Keys.OfType<IDesignTimeCollectionItem>().Select(c => c.FieldName).ToList();
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Columns";
		}
	}
	public abstract class ItemsEditorOwner : IOwnerEditingProperty {
		protected internal const string ImageResourcePrefix = "DevExpress.Web.Design.Images";
		protected const string
			AddItemImageResource = "FrameToolbarItems.png, 0",
			InsertItemImageResource = "FrameToolbarItems.png, 1",
			AddChildItemImageResource = "FrameToolbarItems.png, 2",
			RemoveItemImageResource = "FrameToolbarItems.png, 3",
			MoveUpItemImageResource = "FrameToolbarItems.png, 4",
			MoveDownItemImageResource = "FrameToolbarItems.png, 5",
			DecreaseIndentItemImageResource = "FrameToolbarItems.png, 6",
			IncreaseIndentItemImageResource = "FrameToolbarItems.png, 7",
			ChangeToItemImageResource = "FrameToolbarItems.png, 8",
			RetrieveFieldsItemImageResource = "FrameToolbarItems.png, 9",
			AddColumnImageResource = "FrameToolbarItems.png, 10",
			RemoveColumnImageResource = "FrameToolbarItems.png, 11",
			IncreaseColSpanImageResource = "FrameToolbarItems.png, 12",
			DecreaseColSpanImageResource = "FrameToolbarItems.png, 13",
			IncreaseRowSpanImageResource = "FrameToolbarItems.png, 14",
			DecreaseRowSpanImageResource = "FrameToolbarItems.png, 15",
			CommandColumnImageResource = "ColumnsEditor.CommandColumn.bmp",
			BandColumnImageResource = "ColumnsEditor.BandColumn.bmp",
			BinaryImageColumnImageResource = "ColumnsEditor.ASPxBinaryImage.bmp",
			ButtonEditColumnImageResource = "ColumnsEditor.ASPxButtonEdit.bmp",
			CheckColumnImageResource = "ColumnsEditor.ASPxCheckBox.bmp",
			ColorEditImageResource = "ColumnsEditor.ASPxColorEdit.bmp",
			ComboboxImageResource = "ColumnsEditor.ASPxComboBox.bmp",
			ListEditImageResource = "ColumnsEditor.ListItem.png",
			DateEditImageResource = "ColumnsEditor.ASPxDateEdit.bmp",
			DropDownEditImageResource = "ColumnsEditor.ASPxDropDownEdit.bmp",
			DropDownButtonImageResource = "ColumnsEditor.DropDownButton.png",
			DropDownToggleButtonImageResource = "ColumnsEditor.DropDownToggleButton.png",
			HyperlinkImageResource = "ColumnsEditor.ASPxHyperLink.bmp",
			ImageImageResource = "ColumnsEditor.ASPxImage.bmp",
			MemoImageResource = "ColumnsEditor.ASPxMemo.bmp",
			ProgressBarImageResource = "ColumnsEditor.ASPxProgressBar.bmp",
			SpinEditImageResource = "ColumnsEditor.ASPxSpinEdit.bmp",
			TextImageResource = "ColumnsEditor.ASPxTextBox.bmp",
			TimeEditImageResource = "ColumnsEditor.ASPxTimeEdit.bmp",
			TokenBoxImageResource = "ColumnsEditor.ASPxTokenBox.bmp",
			DataColumnImageResource = "ColumnsEditor.DataColumn.bmp",
			SummaryItemImageResource = "ColumnsEditor.SummaryItem.bmp",
			TabControlItemImageResource = "ColumnsEditor.ASPxTabControl.png",
			RibbonButtonItemImageResource = "ColumnsEditor.RibbonButtonItem.png",
			RibbonGroupItemImageResource = "ColumnsEditor.RibbonGroupItem.png",
			RibbonOptionButtonItemImageResource = "ColumnsEditor.RibbonOptionButtonItem.png",
			RibbonTemplateItemImageResource = "ColumnsEditor.RibbonTemplateItem.png",
			RibbonToggleButtonItemImageResource = "ColumnsEditor.RibbonToggleButtonItem.png",
			RibbonGalleryDropDownResource = "ColumnsEditor.RibbonGalleryDropDown.png",
			RibbonGalleryBarResource = "ColumnsEditor.RibbonGalleryBar.png",
			RibbonGalleryGroupResource = "ColumnsEditor.RibbonGalleryGroup.png",
			RibbonGalleryItemResource = "ColumnsEditor.RibbonGalleryItem.png",
			RibbonContextTabImageResource = "ColumnsEditor.RibbonContextTabs.png",
			PivotGridAreaItemImageResource = "ColumnsEditor.PivotGridArea.bmp",
			PivotGridGroupItemImageResource = "ColumnsEditor.PivotGridGroup.bmp",
			PivotGridFieldItemImageResource = "ColumnsEditor.PivotGridField.bmp",
			PivotGridGroupFieldsItemImageResource = "ColumnsEditor.PivotGridGroupFields.png",
			RankPropertiesItemImageResource = "ColumnsEditor.RankProperties.bmp",
			ItemImageResource = "ColumnsEditor.Item.png",
			NavBarGroupImageResource = "ColumnsEditor.NavBarGroup.bmp",
			NavBarItemImageResource = "ColumnsEditor.NavBarItem.bmp",
			FileManagerFileAccessRuleItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 0",
			FileManagerFolderAccessRuleItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 1",
			FileManagerCreateButtonItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 2",
			FileManagerRenameButtonItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 3",
			FileManagerMoveButtonItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 4",
			FileManagerCopyButtonItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 5",
			FileManagerDeleteButtonItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 6",
			FileManagerRefreshButtonItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 7",
			FileManagerDownloadButtonItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 8",
			FileManagerCustomButtonItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 9",
			FileManagerCustomDropdownButtonItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 10",
			FileManagerDetailsColumnItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 11",
			FileManagerDetailsCustomColumnItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 12",
			FileManagerUploadButtonItemImageResource = "ColumnsEditor.ASPxFileManagerItems.png, 13",
			SpellCheckerDictionaryImageResource = "ColumnsEditor.ASPxSpellCheckerDictionary.png";
		int treeListNodeIDGenerator;
		List<Type> denyedMoveTypes;
		bool itemsChanged;
		List<IDesignTimeCollectionItem> undoItems;
		IComponentChangeService componentChangedService;
		PropertyEditorItems topLevelEditorInfo;
		public ItemsEditorOwner(object component, string collectionPropertyName, IServiceProvider provider, IList items) {
			ServiceProvider = provider;
			CollectionPropertyName = collectionPropertyName;
			Items = items;
			InitializeOwner(component);
		}
		public bool HasTopLevelEditor { get { return !string.IsNullOrEmpty(GetDependendPropertyName()); } }
		public bool ItemsChanged { get { return itemsChanged; } set { itemsChanged = value; } }
		public IServiceProvider ServiceProvider { get; private set; }
		public ITypeDescriptorContext Context {
			get {
				if(Designer == null || string.IsNullOrEmpty(CollectionPropertyName))
					return null;
				return Designer.GetTypeDescriptorContext(CollectionPropertyName);
			}
		}
		protected internal object Component { get; private set; }
		public IComponentChangeService ComponentChangedService {
			get {
				if(ServiceProvider == null)
					return null;
				if(componentChangedService == null)
					componentChangedService = (IComponentChangeService)ServiceProvider.GetService(typeof(IComponentChangeService));
				return componentChangedService;
			}
		}
		public string CollectionPropertyName { get; private set; }
		public ASPxWebControlDesigner Designer { get; private set; }
		public bool SelectedItemsHasChilds { get { return SelectedItems.OfType<IDesignTimeCollectionItem>().Where(i => !i.ReadOnly && i.Items != null && i.Items.Count > 0).Count() > 0; } }
		public bool NeedConfirmRemove {
			get {
				if(SelectedItemsHasChilds)
					return true;
				return TreeListItemsInvertedHash.Keys.Where(i => i.Parent == null).Except(SelectedItems).Count() == 0;
			}
		}
		public List<TreeListItemNode> TreeListItems { get; private set; }
		public Dictionary<int, IDesignTimeCollectionItem> TreeListItemsHash { get; private set; }
		public Dictionary<IDesignTimeCollectionItem, int> TreeListItemsInvertedHash { get; private set; }
		public virtual int FocusedNodeID {
			get {
				if(FocusedItem != null && TreeListItemsInvertedHash.ContainsKey(FocusedItem))
					return TreeListItemsInvertedHash[FocusedItem];
				return -1;
			}
			set { FocusedItem = TreeListItemsHash.ContainsKey(value) ? TreeListItemsHash[value] : null; }
		}
		public object PropertyInstance { get { return Items; } }
		protected internal IDesignTimeCollectionItem FocusedItem { get; set; }
		public IDesignTimeCollectionItem FocusedItemForActionParent { get { return FindParentItem(FocusedItemForAction); } }
		protected IDesignTimeCollectionItem FocusedItemForAction {
			get {
				if(FocusedItem == null)
					return null;
				return TreeListItemsInvertedHash.ContainsKey(FocusedItem) && SelectedItems.Contains(FocusedItem) ? FocusedItem : null;
			}
		}
		protected bool IsReadOnlySelection { get { return SelectedItems.FirstOrDefault(i => i.ReadOnly) != null || FocusedItem == null || FocusedItem.ReadOnly; } }
		Dictionary<Type, List<Type>> groupItemTypes;
		public Dictionary<Type, List<Type>> GroupItemTypes {
			get {
				if(groupItemTypes == null)
					groupItemTypes = new Dictionary<Type, List<Type>>();
				return groupItemTypes;
			}
			private set {
				groupItemTypes = value;
			}
		}
		protected List<Type> DenyedMoveTypes {
			get {
				if(denyedMoveTypes == null)
					denyedMoveTypes = new List<Type>();
				return denyedMoveTypes;
			}
		}
		PropertyEditorItems TopLevelEditorInfo {
			get {
				if(topLevelEditorInfo == null)
					topLevelEditorInfo = new PropertyEditorItems(GetTopLevelEditorType(), GetDependendPropertyName());
				return topLevelEditorInfo;
			}
		}
		public virtual bool SupportDataSource { get { return false; } }
		public virtual string CreateDefaultItemsConfirmMessage { get { return string.Empty; } }
		public virtual string CreateDefaultItemsCaption { get { return string.Empty; } }
		protected virtual void InitializeOwner(object component) {
			InitializeComponent(component);
			Designer = FindDesigner();
			SelectedItems = new List<IDesignTimeCollectionItem>();
			TreeListItems = new List<TreeListItemNode>();
			TreeListItemsHash = new Dictionary<int, IDesignTimeCollectionItem>();
			TreeListItemsInvertedHash = new Dictionary<IDesignTimeCollectionItem, int>();
			ColumnsAndEditors = new List<IDesignTimeColumnAndEditorItem>();
			SaveUndo();
			FillItemTypes();
			FillDenyMoveTypes();
			FillMenuItemResouceImages();
			ValidateVisibleIndices();
			CreateTreeListItems();
			InitFocusedItem();
		}
		void InitializeComponent(object component) {
			Component = component;
			if(ComponentChangedService != null)
				ComponentChangedService.ComponentChanged += ComponentChangedService_ComponentChanged;
		}
		void ComponentChangedService_ComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(OnComponentChanged != null)
				OnComponentChanged(e);
		}
		public virtual string GetDependendPropertyName() {
			return string.Empty;
		}
		public virtual string GetTopLevelEditorName() {
			var properties = GetDependendPropertyName().Split('.');
			var result = properties[properties.Length - 1];
			return !string.IsNullOrEmpty(result) ? string.Format("{0}:", result) : string.Empty;
		}
		public virtual PropertyEditorType GetTopLevelEditorType() {
			return PropertyEditorType.TextEdit;
		}
		public object GetDependendPropertyValue() {
			return FeatureBrowserHelper.GetPropertyValue(Component, GetDependendPropertyName());
		}
		public void SetDependendPropertyValue(object propertyValue) {
			FeatureBrowserHelper.SetPropertyValue(Component, GetDependendPropertyName(), propertyValue);
		}
		protected bool IsGroupItemType(Type type) {
			return GroupItemTypes.Keys.Any(i => GroupItemTypes[i].Contains(type));
		}
		protected bool IsSelectedOnlyThisType(Type type) {
			return !SelectedItems.Exists(i => i.GetType() != type);
		}
		protected void AddGroupItemType(Type groupType, Type itemType, string caption) {
			AddGroupItemType(groupType, itemType, caption, string.Empty, string.Empty);
		}
		protected void AddGroupItemType(Type groupType, Type itemType, string caption, string resourceImageUrl) {
			AddGroupItemType(groupType, itemType, caption, string.Empty, resourceImageUrl);
		}
		protected void AddGroupItemType(Type groupType, Type itemType, string caption, string editorName, string resourceImageUrl) {
			if(!GroupItemTypes.ContainsKey(groupType))
				GroupItemTypes[groupType] = new List<Type>(new Type[] { itemType });
			else
				GroupItemTypes[groupType].Add(itemType);
			AddItemType(itemType, caption, editorName, resourceImageUrl);
		}
		protected virtual void FillDenyMoveTypes() { }
		ASPxWebControlDesigner FindDesigner() {
			var host = ServiceProvider != null ? ServiceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost : null;
			return host != null ? host.GetDesigner((Component as IComponent)) as ASPxWebControlDesigner : null;
		}
		void CreateTreeListItems() {
			TreeListItems = new List<TreeListItemNode>();
			TreeListItemsHash.Clear();
			TreeListItemsInvertedHash.Clear();
			FillTreeListItems(Items, -1);
		}
		void InitFocusedItem() {
			if(Context == null)
				return;
			var item = Context.Instance as IDesignTimeCollectionItem;
			if(item == null && ServiceProvider is ITypeDescriptorContext)
				item = ((ITypeDescriptorContext)ServiceProvider).Instance as IDesignTimeCollectionItem;
			if(item == null && TreeListItemsHash.Count > 0)
				item = TreeListItemsHash[0];
			SetFocusedItem(item, true);
		}
		protected internal void SetFocusedItem(IDesignTimeCollectionItem item, bool forAction) {
			SelectedItems.Remove(FocusedItem);
			FocusedItem = item;
			if(forAction && item != null && !SelectedItems.Contains(item))
				SelectedItems.Add(item);
		}
		public void ChangeFocusedItem(IDesignTimeCollectionItem item) {
			SetFocusedItem(item, true);
			UpdateView();
		}
		public virtual void SelectedItemsPropertyChanged(string propertyName) {  }
		public void RecreateTreeListItems(bool isChanged = true) {
			try {
				BeginUpdate();
				EndUpdate(isChanged);
			} catch {
			}
		}
		void FillTreeListItems(IList items, int parentID) {
			if(items == null || items.Count == 0)
				return;
			var sortedItems = items.OfType<IDesignTimeCollectionItem>().OrderBy(i => i.VisibleIndex).ToList();
			foreach(var item in sortedItems) {
				var designTimeItem = FindDesignTimeItem(item.GetType());
				if(designTimeItem == null) {
					designTimeItem = GetDesignTimeItem(item.GetType());
					if(designTimeItem != null)
						ColumnsAndEditors.Add(designTimeItem);
				}
				var imageIndex = designTimeItem != null ? designTimeItem.ImageIndex : -1;
				var node = CreateTreeListItemNode(treeListNodeIDGenerator++, parentID, item, imageIndex);
				TreeListItems.Add(node);
				TreeListItemsHash[node.ID] = item;
				TreeListItemsInvertedHash[item] = node.ID;
				FillTreeListItems(item.Items, node.ID);
			}
		}
		protected virtual TreeListItemNode CreateTreeListItemNode(int nodeID, int parentNodeID, IDesignTimeCollectionItem item, int imageIndex) {
			return new TreeListItemNode(nodeID, parentNodeID, item.Visible, false, item.Caption, imageIndex);
		}
		protected virtual IDesignTimeColumnAndEditorItem GetDesignTimeItem(Type itemType) {
			return null;
		}
		public virtual string GetDesignTimeItemText(Type itemType) {
			var item = FindDesignTimeItem(itemType);
			return item != null ? item.Text : string.Empty;
		}
		public virtual IDesignTimeColumnAndEditorItem FindDesignTimeColumnType(int hashItemID) {
			if(!TreeListItemsHash.ContainsKey(hashItemID))
				return null;
			return FindDesignTimeItem(TreeListItemsHash[hashItemID].GetType());
		}
		public OnBeginDataOwnerUpdate OnBeginDataUpdate { get; set; }
		public OnEndDataOwnerUpdate OnEndDataUpdate { get; set; }
		public OnUpdateResourceImageMap OnUpdateImageMap { get; set; }
		public OnComponentChanged OnComponentChanged { get; set; }
		public OnUpdateView OnUpdateView { get; set; }
		TreeListNodesState TreeListSavedState { get; set; }
		protected void UpdateView() {
			if(OnUpdateView != null)
				OnUpdateView();
		}
		protected void BeginUpdate() {
			if(OnBeginDataUpdate == null)
				return;
			TreeListSavedState = OnBeginDataUpdate();
		}
		protected virtual void EndUpdate(bool isChanged = true) {
			if(OnEndDataUpdate == null)
				return;
			if(isChanged)
				ItemsChanged = true;
			var isFocusedItemSelected = SelectedItems.Contains(FocusedItem);
			SelectedItems.Clear();
			var savedTreeListItemsHash = new Dictionary<int, IDesignTimeCollectionItem>(TreeListItemsHash);
			CreateTreeListItems();
			var newExpandedState = GetValidatedTreeListIDs(TreeListSavedState.Expanded, savedTreeListItemsHash, TreeListItemsInvertedHash);
			var newSelectedState = GetValidatedTreeListIDs(TreeListSavedState.Selected, savedTreeListItemsHash, TreeListItemsInvertedHash);
			if(isFocusedItemSelected)
				newSelectedState.Add(FocusedNodeID);
			TreeListSavedState.Expanded.Clear();
			TreeListSavedState.Selected.Clear();
			TreeListSavedState.Expanded.AddRange(newExpandedState);
			TreeListSavedState.Selected.AddRange(newSelectedState);
			OnEndDataUpdate(TreeListSavedState);
		}
		List<int> GetValidatedTreeListIDs(List<int> source, Dictionary<int, IDesignTimeCollectionItem> savedHash, Dictionary<IDesignTimeCollectionItem, int> invertedNewHash) {
			var result = new List<int>();
			foreach(var oldID in source) {
				var column = savedHash[oldID];
				if(invertedNewHash.ContainsKey(column))
					result.Add(invertedNewHash[column]);
			}
			return result;
		}
		Dictionary<string, int> resourceImageMap;
		public Dictionary<string, int> ResourceImageMap {
			get {
				if(resourceImageMap == null) {
					resourceImageMap = new Dictionary<string, int>();
					GetEditorDescriptorItems(true);
					GetEditorDescriptorItems(false);
				}
				return resourceImageMap;
			}
		}
		public List<IDesignTimeColumnAndEditorItem> ColumnsAndEditors { get; private set; }
		protected internal virtual IList Items { get; internal set; }
		protected internal List<IDesignTimeCollectionItem> UndoItems {
			get {
				if(undoItems == null)
					undoItems = new List<IDesignTimeCollectionItem>();
				return undoItems;
			}
		}
		public virtual void SaveUndo() {
			CloneListTo(UndoItems, Items);
		}
		public virtual void SaveChanges() {
		}
		public void BeforeClosed() { }
		public virtual void UndoChanges() {
			ReplaceList(Items, UndoItems);
		}
		protected IList CloneListTo(IList target, IEnumerable source, bool clearTarget = true) {
			if(clearTarget)
				target.Clear();
			var result = new List<IDesignTimeCollectionItem>();
			if(target == null || source == null)
				return result;
			foreach(var item in source) {
				var clone = CloneItemTo(target, item);
				if(clone != null)
					result.Add(clone);
			}
			return result;
		}
		protected virtual IDesignTimeCollectionItem CloneItemTo(IList target, object item) {
			var type = item.GetType();
			if(!typeof(IDesignTimeCollectionItem).IsAssignableFrom(type))
				return null;
			var constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
			if(constructor == null)
				return null;
			var newItem = (IDesignTimeCollectionItem)Activator.CreateInstance(type);
			newItem.Assign((IDesignTimeCollectionItem)item);
			target.Add(newItem);
			return newItem;
		}
		protected void ReplaceList(IList target, IList source) {
			target.Clear();
			CopyList(target, source);
		}
		protected void CopyList(IList target, IList source) {
			if(target == null || source == null)
				return;
			foreach(var item in source)
				UndoItem(target, item);
		}
		protected virtual void UndoItem(IList target, object item) {
			target.Add(item);
		}
		protected abstract void FillItemTypes();
		protected virtual void FillMenuItemResouceImages() {
			AddResourceImage(AddItemImageResource);
			AddResourceImage(InsertItemImageResource);
			AddResourceImage(RemoveItemImageResource);
			AddResourceImage(MoveDownItemImageResource);
			AddResourceImage(MoveUpItemImageResource);
			AddResourceImage(DecreaseIndentItemImageResource);
			AddResourceImage(IncreaseIndentItemImageResource);
		}
		public virtual List<string> GetViewDependedProperties() {
			var list = new List<string>();
			list.AddRange(new string[] { 
				"Text", 
				"HeaderText",
				"Caption",
				"Name",
				"FieldName",
				"Visible", 
				"VisibleIndex"
			});
			return list;
		}
		public virtual string[] GetVisibleProperties() {
			return new string[] { };
		}
		protected void AddItemType(Type itemType, string caption) {
			AddItemType(itemType, caption, string.Empty, string.Empty);
		}
		protected void AddItemType(Type itemType, string caption, string resourceImageUrl) {
			AddItemType(itemType, caption, string.Empty, resourceImageUrl);
		}
		protected void AddItemType(Type itemType, string caption, string editorName, string resourceImageUrl) {
			ColumnsAndEditors.Add(new DesignTimeColumnType(itemType, caption, editorName, GetResourceImageIndex(resourceImageUrl)));
		}
		void ValidateVisibleIndices() {
			ValidateVisibleIndicesCore(Items as WebColumnCollectionBase);
		}
		void ValidateVisibleIndicesCore(WebColumnCollectionBase webColumns) {
			if(webColumns == null)
				return;
			var sortedWebColumns = webColumns.OfType<WebColumnBase>().OrderBy(c => c.VisibleIndex).ToList();
			int prevIndex = int.MinValue;
			foreach(var column in sortedWebColumns) {
				if(column.VisibleIndex <= prevIndex)
					column.SetColVisibleIndex(++prevIndex);
				else
					prevIndex = column.VisibleIndex;
				var columnsOwner = column as IWebColumnsOwner;
				if(columnsOwner != null)
					ValidateVisibleIndicesCore(columnsOwner.Columns);
			}
		}
		protected internal virtual void MoveItemTo(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			direction = TranslateMovingDirection(direction);
			if(!CanMoveItem(source, target, direction))
				return;
			var sourceCollection = FindItemCollection(source);
			var targetCollection = FindItemCollection(target);
			if(direction == InsertDirection.Inside && target is IDesignTimeCollectionItem)
				targetCollection = target.Items;
			var visible = source.Visible;
			if(targetCollection.IndexOf(source) == -1) {
				RemoveDesignTimeItem(sourceCollection, source);
				ResetItemVisibleIndex(source, targetCollection);
				targetCollection.Add(source);
			}
			source.VisibleIndex = GetMovingItemVisibleIndex(targetCollection, source, target, direction);
			source.Visible = visible;
		}
		InsertDirection TranslateMovingDirection(InsertDirection direction) {
			if(direction == InsertDirection.Left)
				return InsertDirection.Before;
			if(direction == InsertDirection.Right)
				return InsertDirection.After;
			return direction;
		}
		protected virtual void ResetItemVisibleIndex(IDesignTimeCollectionItem item, IList itemCollection) {
			item.VisibleIndex = -1;
		}
		protected void RemoveDesignTimeItem(IList itemsCollection, IDesignTimeCollectionItem item) {
			if(itemsCollection == null)
				return;
			var items = itemsCollection.OfType<IDesignTimeCollectionItem>();
			if(items.Contains(item))
				itemsCollection.Remove(item);
		}
		protected virtual IDesignTimeCollectionItem FindParentItem(IDesignTimeCollectionItem itemToSearch, IDesignTimeCollectionItem parent = null) {
			return itemToSearch != null && itemToSearch.Parent != null ? itemToSearch.Parent : parent;
		}
		protected virtual IList FindItemCollection(IDesignTimeCollectionItem item) {
			return item == null || item.Parent == null ? Items : item.Parent.Items;
		}
		protected virtual IDesignTimeCollectionItem FindNextNotSelectedItem() {
			if(SelectedItems.Count == 0)
				return null;
			var firstSelectedItem = SelectedItems.OrderBy(i => i.VisibleIndex).FirstOrDefault();
			var items = FindItemCollection(firstSelectedItem).OfType<IDesignTimeCollectionItem>().ToList();
			int index = items.IndexOf(firstSelectedItem);
			for(int i = index + 1; i < items.Count && index > -1; i++)
				if(!SelectedItems.Contains(items[i]))
					return items[i];
			return index > 0 ? items[--index] : FindParentItem(firstSelectedItem);
		}
		int GetMovingItemVisibleIndex(IList items, IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			var sortedItems = items.OfType<IDesignTimeCollectionItem>().OrderBy(c => c.VisibleIndex).ToList();
			if(direction == InsertDirection.Inside || target == null) {
				if(sortedItems.Count > 1)
					return sortedItems[sortedItems.Count - 2].VisibleIndex + 1;
				return 0;
			}
			var sourceIndex = sortedItems.IndexOf(source);
			var targetIndex = sortedItems.IndexOf(target);
			var diff = sourceIndex - targetIndex;
			if(direction == InsertDirection.Before)
				return diff > 0 ? target.VisibleIndex : target.VisibleIndex - 1;
			return diff > 0 ? target.VisibleIndex + 1 : target.VisibleIndex;
		}
		public virtual string GetFormCaption() {
			return Designer != null ? Designer.GetFormCaption(EditingPropertyName) : string.Empty;
		}
		protected string EditingPropertyName {
			get {
				if(Context != null && Context.PropertyDescriptor != null)
					return Context.PropertyDescriptor.Name;
				return string.Empty;
			}
		}
		public DesignTimeEditorDescriptorItems GetEditorDescriptorItems(bool isToolbarMenu) {
			var result = new DesignTimeEditorDescriptorItems();
			var actionTypes = isToolbarMenu ? GetToolbarActionTypes() : GetContextMenuActionTypes();
			foreach(DesignEditorMenuRootItemActionType actionType in actionTypes) {
				var menuItem = CreateEditorDescriptorItem(actionType, isToolbarMenu);
				if(menuItem != null)
					result.Add(menuItem);
			}
			return result;
		}
		protected virtual List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.AddItem,
				DesignEditorMenuRootItemActionType.InsertBefore, 
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp, 
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.ChangeTo, 
				DesignEditorMenuRootItemActionType.RetriveFields,
				DesignEditorMenuRootItemActionType.CreateDefaultItems
			};
		}
		protected virtual List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.ChangeTo, 
				DesignEditorMenuRootItemActionType.MoveUp, 
				DesignEditorMenuRootItemActionType.MoveDown, 
				DesignEditorMenuRootItemActionType.SelectAll
			};
		}
		protected virtual DesignEditorDescriptorItem CreateEditorDescriptorItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(!CanCreateRootMenuItem(actionType, isToolbarMenu))
				return null;
			switch(actionType) {
				case DesignEditorMenuRootItemActionType.InsertChild:
					return CreateInsertChildMenuItem(isToolbarMenu);
				case DesignEditorMenuRootItemActionType.Remove:
					return CreateRemoveMenuItem(isToolbarMenu);
				case DesignEditorMenuRootItemActionType.RemoveInnerItems:
					return CreateRemoveInnerItems(isToolbarMenu);
				case DesignEditorMenuRootItemActionType.ChangeTo:
					return CreateChangeToMenuItem(isToolbarMenu);
				case DesignEditorMenuRootItemActionType.MoveUp:
					return CreateMoveUpMenuItem(isToolbarMenu);
				case DesignEditorMenuRootItemActionType.MoveDown:
					return CreateMoveDownMenuItem(isToolbarMenu);
				case DesignEditorMenuRootItemActionType.AddItem:
					return CreateAddItemMenuItem();
				case DesignEditorMenuRootItemActionType.MoveLeft:
					return CreateDecreaseIndentMenuItem();
				case DesignEditorMenuRootItemActionType.MoveRight:
					return CreateIncreaseIndentMenuItem();
				case DesignEditorMenuRootItemActionType.InsertBefore:
					return CreateInsertBeforeMenuItem();
				case DesignEditorMenuRootItemActionType.SelectAll:
					return CreateSelectAllMenuItem();
				case DesignEditorMenuRootItemActionType.SetItemsAmount:
					return CreateSetItemsAmount();
				case DesignEditorMenuRootItemActionType.MoveToNewGroup:
					return CreateMoveToNewGroupMenuItem();
				case DesignEditorMenuRootItemActionType.IncreaseColumn:
					return CreateIncreaseColumnGroupMenuItem();
				case DesignEditorMenuRootItemActionType.DecreaseColumn:
					return CreateDecreaseColumnGroupMenuItem();
				case DesignEditorMenuRootItemActionType.IncreaseColSpan:
					return CreateIncreaseColSpanGroupMenuItem();
				case DesignEditorMenuRootItemActionType.DecreaseColSpan:
					return CreateDecreaseColSpanGroupMenuItem();
				case DesignEditorMenuRootItemActionType.IncreaseRowSpan:
					return CreateIncreaseRowSpanGroupMenuItem();
				case DesignEditorMenuRootItemActionType.DecreaseRowSpan:
					return CreateDecreaseRowSpanGroupMenuItem();
				case DesignEditorMenuRootItemActionType.CreateDefaultItems:
					return CreateDefaultItemsMenuItem();
			}
			return null;
		}
		protected virtual DesignEditorDescriptorItem CreateIncreaseColumnGroupMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.IncreaseColumn };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Caption = "Add Column";
			item.Enabled = true;
			item.ImageIndex = -1; 
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateDecreaseColumnGroupMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.DecreaseColumn };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Caption = "Remove Column";
			item.Enabled = true;
			item.ImageIndex = -1; 
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateIncreaseColSpanGroupMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.IncreaseColSpan };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Caption = "Increase ColSpan";
			item.Enabled = true;
			item.ImageIndex = GetResourceImageIndex(IncreaseColSpanImageResource);
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateDecreaseColSpanGroupMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.DecreaseColSpan };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Caption = "Decrease ColSpan";
			item.Enabled = true;
			item.ImageIndex = GetResourceImageIndex(DecreaseColSpanImageResource);
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateIncreaseRowSpanGroupMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.IncreaseRowSpan };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Caption = "Increase RowSpan";
			item.Enabled = true;
			item.ImageIndex = GetResourceImageIndex(IncreaseRowSpanImageResource);
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateDecreaseRowSpanGroupMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.DecreaseRowSpan };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Caption = "Decrease RowSpan";
			item.Enabled = true;
			item.ImageIndex = GetResourceImageIndex(DecreaseRowSpanImageResource);
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateAddItemMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.AddItem };
			item.EditorType = DesignEditorDescriptorItemType.DropDownButton;
			item.Caption = "Add";
			item.Enabled = true;
			item.ImageIndex = GetResourceImageIndex(AddItemImageResource);
			PopulateChildItems(item);
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateMoveToNewGroupMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.MoveToNewGroup };
			item.Caption = "Move to a new group";
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.ImageIndex = GetResourceImageIndex(PivotGridGroupFieldsItemImageResource);
			item.Enabled = IsEnabledMoveToNewGroupMenuItem();
			return item;
		}
		protected virtual bool IsEnabledMoveToNewGroupMenuItem() {
			return true;
		}
		protected virtual DesignEditorDescriptorItem CreateInsertBeforeMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.InsertBefore };
			item.Caption = "Insert";
			item.EditorType = DesignEditorDescriptorItemType.DropDownButton;
			item.Enabled = FocusedItemForAction != null;
			item.ImageIndex = GetResourceImageIndex(InsertItemImageResource);
			PopulateChildItems(item);
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateInsertChildMenuItem(bool isToolbar) {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.InsertChild };
			item.EditorType = isToolbar ? DesignEditorDescriptorItemType.DropDownButton : DesignEditorDescriptorItemType.DropDown;
			item.Caption = isToolbar ? "Add Child" : "Add";
			if(isToolbar)
				item.Enabled = IsSupportHierarchy(FocusedItemForAction);
			else
				item.Enabled = FocusedItemForAction == null || IsSupportHierarchy(FocusedItemForAction);
			item.ImageIndex = GetResourceImageIndex(AddChildItemImageResource);
			PopulateChildItems(item, isToolbar);
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateRemoveMenuItem(bool isToolbarMenu) {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.Remove };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = SelectedItems.Count > 0;
			item.ImageIndex = GetResourceImageIndex(RemoveItemImageResource);
			item.Caption = "Remove";
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateRemoveInnerItems(bool isToolbarMenu) {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.RemoveInnerItems };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = false;
			item.Caption = "RemoveInnerItems";
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateMoveUpMenuItem(bool isToolbar) {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.MoveUp };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.BeginGroup = true;
			item.Enabled = IsMenuMoveItemEnabled(false);
			item.ImageIndex = GetResourceImageIndex(MoveUpItemImageResource);
			item.Caption = "Move Up";
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateMoveDownMenuItem(bool isToolbarMenu) {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.MoveDown };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = IsMenuMoveItemEnabled(true);
			item.ImageIndex = GetResourceImageIndex(MoveDownItemImageResource);
			item.Caption = "Move Down";
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateDecreaseIndentMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.MoveLeft };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Caption = "Move Out";
			item.Enabled = CanDecreaseLevelSelectedItems();
			item.ImageIndex = GetResourceImageIndex(DecreaseIndentItemImageResource);
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateIncreaseIndentMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.MoveRight };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Caption = "Move In";
			item.Enabled = CanIncreaseLevelSelectedItems();
			item.ImageIndex = GetResourceImageIndex(IncreaseIndentItemImageResource);
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateChangeToMenuItem(bool isToolbar) {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.ChangeTo };
			item.EditorType = DesignEditorDescriptorItemType.DropDown;
			item.Caption = "Change To";
			item.Enabled = IsSelectionChangingEnabled();
			item.BeginGroup = true;
			item.ImageIndex = GetResourceImageIndex(ChangeToItemImageResource);
			PopulateChildItems(item, isToolbar);
			if(item.ChildItems.Count == 0)
				item.Enabled = false;
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateSelectAllMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.SelectAll };
			item.Caption = "Select All";
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = TreeListItemsHash.Count > 0 && TreeListItemsHash.Values.Any(i => !i.ReadOnly);
			item.BeginGroup = true;
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateSetItemsAmount() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.SetItemsAmount };
			item.Caption = string.Format("Set {0} count", GetNavBarItemsGroupName());
			item.EditorType = DesignEditorDescriptorItemType.SpinEdit;
			item.BeginGroup = true;
			item.Value = Items.Count;
			item.Enabled = true;
			item.Parameters.Add(0);
			item.Parameters.Add(255);
			return item;
		}
		protected virtual DesignEditorDescriptorItem CreateDefaultItemsMenuItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.CreateDefaultItems };
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Caption = "Create Default Items";
			return item;
		}
		protected void PopulateChildItems(DesignEditorDescriptorItem parent) {
			PopulateChildItems(parent, false);
		}
		protected virtual void PopulateChildItems(DesignEditorDescriptorItem parent, bool isToolbar) {
			if(!parent.Enabled)
				return;
			foreach(var designTimeItem in ColumnsAndEditors) {
				if(CanCreateSubmenuItem(parent, designTimeItem, isToolbar)) {
					var menuItem = CreateSubmenuItem(parent, designTimeItem, isToolbar);
					parent.ChildItems.Add(menuItem);
				}
			}
		}
		protected virtual DesignEditorDescriptorItem CreateSubmenuItem(DesignEditorDescriptorItem parent, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			return new DesignEditorDescriptorItem() {
				Caption = designTimeItem.Text,
				ImageIndex = designTimeItem.ImageIndex,
				ItemType = designTimeItem,
				ParentItem = parent,
				Enabled = true
			};
		}
		protected virtual bool CanCreateRootMenuItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			switch(actionType) {
				case DesignEditorMenuRootItemActionType.AddItem:
				case DesignEditorMenuRootItemActionType.InsertBefore:
				case DesignEditorMenuRootItemActionType.RetriveFields:
				case DesignEditorMenuRootItemActionType.SetItemsAmount:
					return isToolbarMenu;
				case DesignEditorMenuRootItemActionType.CreateDefaultItems:
					return isToolbarMenu && !string.IsNullOrEmpty(CreateDefaultItemsConfirmMessage);
				case DesignEditorMenuRootItemActionType.InsertChild:
				case DesignEditorMenuRootItemActionType.Remove:
				case DesignEditorMenuRootItemActionType.ChangeTo:
				case DesignEditorMenuRootItemActionType.MoveUp:
				case DesignEditorMenuRootItemActionType.MoveDown:
				case DesignEditorMenuRootItemActionType.MoveLeft:
				case DesignEditorMenuRootItemActionType.MoveRight:
				case DesignEditorMenuRootItemActionType.MoveToNewGroup:
					return true;
				case DesignEditorMenuRootItemActionType.SelectAll:
					return !isToolbarMenu;
			}
			return false;
		}
		protected virtual bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(!parentMenuItem.Enabled)
				return false;
			if(parentMenuItem.ActionType == DesignEditorMenuRootItemActionType.ChangeTo)
				return GetSelectedItemsType() != designTimeItem.ColumnType;
			return true;
		}
		bool IsMenuMoveItemEnabled(bool down) {
			foreach(var group in GetSelectedGroups()) {
				if(GetNextItemFromSelectionGroup(group, down) != null)
					return true;
			}
			return false;
		}
		protected internal IDesignTimeColumnAndEditorItem FindDesignTimeItem(Type columnType) {
			return ColumnsAndEditors.FirstOrDefault(i => i.ColumnType == columnType);
		}
		protected int GetResourceImageIndex(string imageUrl) {
			return imageUrl != string.Empty ? AddResourceImage(imageUrl) : -1;
		}
		protected int AddResourceImage(string imagePath) {
			if(string.IsNullOrEmpty(imagePath))
				return -1;
			var fullPath = string.Format("{0}.{1}", ImageResourcePrefix, imagePath);			
			if(!ResourceImageMap.ContainsKey(fullPath)) {
				ResourceImageMap[fullPath] = ResourceImageMap.Count;
				if(OnUpdateImageMap != null)
					OnUpdateImageMap(fullPath);
			}
			return ResourceImageMap[fullPath];
		}
		protected internal virtual string GetItemPropertiesTabCaption() {
			return "Item Properties";
		}
		protected internal virtual string GetNavBarItemsGroupName() {
			return CollectionPropertyName;
		}
		protected internal virtual string GetNavBarItemsGroupDescription() {
			return string.Format("Manage and create {0}.", GetNavBarItemsGroupName().ToLower());
		}
		protected internal virtual string GetNavBarClientSideEventsGroupName() {
			return "Client-Side Events";
		}
		public string GetEditorTabName() {
			var items = SelectedItems.OfType<IDesignTimeCollectionItem>().ToList();
			if(items.Count == 0)
				return string.Empty;
			var first = items[0];
			var itemType = first.GetType();
			if(!items.All(i => i.GetType() == itemType))
				return string.Empty;
			var designTimeItem = FindEditorDesignTimeItem(first);
			if(designTimeItem != null)
				return designTimeItem.EditorName;
			return string.Empty;
		}
		protected virtual IDesignTimeColumnAndEditorItem FindEditorDesignTimeItem(IDesignTimeCollectionItem item) {
			return FindDesignTimeItem(item.GetType());
		}
		public virtual IDesignTimeCollectionItem CreateNewItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			return Activator.CreateInstance(GetDesignTimeItemType(designTimeItem)) as IDesignTimeCollectionItem;
		}
		protected Type GetDesignTimeItemType(IDesignTimeColumnAndEditorItem designTimeItem) { 
			return designTimeItem != null ? designTimeItem.ColumnType : GetDefaultItemType();
		}
		public abstract Type GetDefaultItemType();
		public void SetItemsAmount(int amount) {
			if(amount < 0)
				amount = 0;
			int delta = amount - Items.Count;
			int counter = Math.Abs(delta);
			if(delta < 0)
				RemoveRange(amount, counter);
			else
				AddRange(FindDesignTimeItem(GetDefaultItemType()), counter);
		}
		public void AddRange(IDesignTimeColumnAndEditorItem designTimeItem, int amount) {
			BeginUpdate();
			while(--amount >= 0)
				AddItemCore(designTimeItem, FocusedItemForActionParent, InsertDirection.Inside);
			EndUpdate();
		}
		public void AddItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			BeginUpdate();
			AddItemCore(designTimeItem, FocusedItemForActionParent, InsertDirection.Inside);
			EndUpdate();
		}
		public void InsertItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			BeginUpdate();
			AddItemCore(designTimeItem, FocusedItemForAction, InsertDirection.Before);
			EndUpdate();
		}
		public void AddChildItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			BeginUpdate();
			AddItemCore(designTimeItem, FocusedItemForAction, InsertDirection.Inside);
			EndUpdate();
		}
		protected virtual void AddItemCore(IDesignTimeColumnAndEditorItem designTimeItem, IDesignTimeCollectionItem target, InsertDirection direction) {
			var newItem = CreateNewItem(designTimeItem);
			MoveItemTo(newItem, target, direction);
			SetFocusedItem(newItem, true);
		}
		public virtual void CreateDefaultItems(bool deleteExistingItems) { }
		public void RemoveSelectedItems() {
			BeginUpdate();
			var neighbourItem = FindNextNotSelectedItem();
			var items = SelectedItems.OfType<IDesignTimeCollectionItem>().Where(i => !i.ReadOnly).ToList();
			if(items.Count != 0) {
				foreach(var item in items)
					RemoveItem(item);
				if(neighbourItem == null)
					neighbourItem = Items.OfType<IDesignTimeCollectionItem>().FirstOrDefault();
				SetFocusedItem(neighbourItem, true);
			}
			EndUpdate();
		}
		public void RemoveInnerSelectedItems() {
			BeginUpdate();
			RemoveInnerSelectedItemsCore();
			EndUpdate();
		}
		protected virtual void RemoveInnerSelectedItemsCore() {
		}
		public void MoveSelectedItems(bool down) {
			if(!IsMenuMoveItemEnabled(down))
				return;
			BeginUpdate();
			var direction = down ? InsertDirection.After : InsertDirection.Before;
			foreach(var group in GetSelectedGroups()) {
				var target = GetNextItemFromSelectionGroup(group, down);
				if(target != null)
					MoveItemGroup(group, target, direction);
			}
			EndUpdate();
		}
		public void MoveSelectedItemsTo(int targetID, InsertDirection direction) {
			BeginUpdate();
			var target = TreeListItemsHash[targetID];
			foreach(var group in GetSelectedGroups())
				MoveItemGroup(group, target, direction);
			EndUpdate();
		}
		public void ChangeSelectionIndent(bool increase) {
			BeginUpdate();
			var newIndentsList = increase ? IncreaseIndentSelectionLevels : DecreaseIndentSelectionLevels;
			var direction = increase ? InsertDirection.Inside : InsertDirection.After;
			for(int i = 0; i < SelectedGroups.Count; ++i) {
				if(newIndentsList.ContainsKey(i))
					MoveItemGroup(SelectedGroups[i], newIndentsList[i], direction);
			}
			EndUpdate();
		}
		protected void MoveItemGroup(List<IDesignTimeCollectionItem> group, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(direction == InsertDirection.After)
				group.Reverse();
			foreach(var item in group)
				MoveItemTo(item, target, direction);
		}
		public void ChangeSelectedItemsTo(IDesignTimeColumnAndEditorItem designTimeItem) {
			BeginUpdate();
			IDesignTimeCollectionItem newItem = null;
			foreach(var item in SelectedItems) {
				IDesignTimeCollectionItem neighbour = null;
				InsertDirection direction = InsertDirection.Inside;
				var parentItems = GetParentItems(item).OrderBy(i => i.VisibleIndex).ToList();
				var itemIndex = parentItems.IndexOf(item);
				if(itemIndex >= 0 && itemIndex < parentItems.Count - 1) {
					neighbour = parentItems[itemIndex + 1];
					direction = InsertDirection.Before;
				} else {
					neighbour = item.Parent;
				}
				newItem = ChangeItemToCore(designTimeItem, item, neighbour, direction);
			}
			SetFocusedItem(newItem, true);
			EndUpdate();
		}
		protected virtual IDesignTimeCollectionItem ChangeItemToCore(IDesignTimeColumnAndEditorItem designTimeItem, IDesignTimeCollectionItem oldItem, IDesignTimeCollectionItem neighbour, InsertDirection direction) {
			var result = CreateNewItem(designTimeItem);
			result.Assign(oldItem);
			RemoveItem(oldItem);
			MoveItemTo(result, neighbour, direction);
			return result;
		}
		protected virtual void RemoveItem(IDesignTimeCollectionItem item) {
			if(item.ReadOnly)
				return;
			RemoveDesignTimeItem(FindItemCollection(item), item);
		}
		protected virtual void RemoveRange(int startIndex, int amount) {
			BeginUpdate();
			int endIndex = amount + startIndex - 1;
			for(int i = endIndex; i >= startIndex; --i)
				RemoveItem((IDesignTimeCollectionItem)Items[i]);
			EndUpdate();
		}
		protected virtual List<IDesignTimeCollectionItem> GetParentItems(IDesignTimeCollectionItem item) {
			return FindItemCollection(item).OfType<IDesignTimeCollectionItem>().ToList();
		}
		public bool IsItemSupportHierarchy(int itemID) {
			return IsSupportHierarchy(TreeListItemsHash[itemID]);
		}
		public bool IsSupportHierarchy(IDesignTimeCollectionItem item) {
			return item != null && item.Items != null;
		}
		List<List<IDesignTimeCollectionItem>> SelectedGroups { get; set; }
		protected internal void PopulateSelectedGroupsNextIndentLevel() {
			IncreaseIndentSelectionLevels.Clear();
			DecreaseIndentSelectionLevels.Clear();
			SelectedGroups = GetSelectedGroups();
			if(SelectedGroups.Count == 0)
				return;
			var indentLevels = new List<IDesignTimeCollectionItem>();
			for(int groupIndex = 0; groupIndex < SelectedGroups.Count; ++groupIndex) {
				PopulateIncreaseIndentLevel(groupIndex);
				PopulateDecreaseIndentLevel(groupIndex);
			}
		}
		protected void PopulateIncreaseIndentLevel(int groupIndex) {
			var items = SelectedGroups[groupIndex].OrderBy(i => i.VisibleIndex).OfType<IDesignTimeCollectionItem>().ToList();
			if(items.Count == 0)
				return;
			var edgeItem = items.First();
			var parentItem = FindParentItem(edgeItem);
			var currentItems = (parentItem != null ? parentItem.Items : Items).OfType<IDesignTimeCollectionItem>().OrderBy(i => i.VisibleIndex).ToList();
			var edgeItemIndex = currentItems.IndexOf(edgeItem);
			if(--edgeItemIndex < 0)
				return;
			var itemBeforeEdge = currentItems[edgeItemIndex];
			if(IsSupportHierarchy(itemBeforeEdge))
				IncreaseIndentSelectionLevels.Add(groupIndex, itemBeforeEdge);
		}
		protected void PopulateDecreaseIndentLevel(int groupIndex) {
			var items = SelectedGroups[groupIndex];
			if(items.Count == 0)
				return;
			var firstItem = items.First();
			var parent = FindParentItem(firstItem);
			if(CanPopulateDecreaseIndentLevel(firstItem, parent))
				DecreaseIndentSelectionLevels.Add(groupIndex, parent);
		}
		protected virtual bool CanPopulateDecreaseIndentLevel(IDesignTimeCollectionItem firstItem, IDesignTimeCollectionItem parent) {
			return IsSupportHierarchy(parent) && (parent == null || parent.VisibleIndex >= 0);
		}
		protected internal List<IDesignTimeCollectionItem> SelectedItems { get; private set; }
		Dictionary<int, IDesignTimeCollectionItem> increaseIndentSelectionLevels;
		protected internal Dictionary<int, IDesignTimeCollectionItem> IncreaseIndentSelectionLevels {
			get {
				if(increaseIndentSelectionLevels == null)
					increaseIndentSelectionLevels = new Dictionary<int, IDesignTimeCollectionItem>();
				return increaseIndentSelectionLevels;
			}
		}
		Dictionary<int, IDesignTimeCollectionItem> decreaseIndentSelectionLevels;
		protected internal Dictionary<int, IDesignTimeCollectionItem> DecreaseIndentSelectionLevels {
			get {
				if(decreaseIndentSelectionLevels == null)
					decreaseIndentSelectionLevels = new Dictionary<int, IDesignTimeCollectionItem>();
				return decreaseIndentSelectionLevels;
			}
		}
		public bool CanIncreaseLevelSelectedItems() {
			return IncreaseIndentSelectionLevels.Count != 0;
		}
		public bool CanDecreaseLevelSelectedItems() {
			return DecreaseIndentSelectionLevels.Count != 0;
		}
		public void SetSelection(List<int> selection) {
			SelectedItems.Clear();
			var focuseID = FocusedNodeID;
			bool setHeadFocus = focuseID != -1 && selection.Count != 0 && !selection.Contains(focuseID);
			foreach(var id in selection) {
				if(TreeListItemsHash.ContainsKey(id))
					SelectedItems.Add(TreeListItemsHash[id]);
			}
			if(setHeadFocus)
				SetFocusedItem(TreeListItemsHash[selection[0]], true);
			PopulateSelectedGroupsNextIndentLevel();
		}
		protected internal IDesignTimeCollectionItem GetNextItemFromSelectionGroup(List<IDesignTimeCollectionItem> selectedItems, bool down) {
			var parentItems = GetParentItems(selectedItems[0]).OrderBy(i => i.VisibleIndex).ToList();
			var sortedSelectedItems = selectedItems.OrderBy(i => i.VisibleIndex).ToList();
			var boundSelectItemIndex = down ? sortedSelectedItems.Count - 1 : 0;
			var boundParentItemIndex = parentItems.IndexOf(sortedSelectedItems[boundSelectItemIndex]);
			var nextParentItemIndex = boundParentItemIndex + (down ? 1 : -1);
			if(nextParentItemIndex < 0 || nextParentItemIndex >= parentItems.Count)
				return null;
			var result = parentItems[nextParentItemIndex];
			return !DenyedMoveTypes.Contains(result.GetType()) ? result : null;
		}
		protected internal List<List<IDesignTimeCollectionItem>> GetSelectedGroups() {
			var result = new List<List<IDesignTimeCollectionItem>>();
			foreach(var selectionGroup in SelectedItems.GroupBy(i => FindItemCollection(i))) {
				var parentItems = selectionGroup.Key.OfType<IDesignTimeCollectionItem>().OrderBy(i => i.VisibleIndex).ToList();
				var selectedItems = selectionGroup.OrderBy(i => i.VisibleIndex).ToList();
				List<List<IDesignTimeCollectionItem>> groups = new List<List<IDesignTimeCollectionItem>>();
				for(var i = selectedItems.Count - 1; i >= 0; i--) {
					var startItem = selectedItems[i];
					var list = new List<IDesignTimeCollectionItem>() { startItem };
					groups.Add(list);
					var prevIndex = parentItems.IndexOf(startItem);
					for(var j = i - 1; j >= 0; j--) {
						var item = selectedItems[j];
						var itemIndex = parentItems.IndexOf(item);
						if(itemIndex + 1 == prevIndex) {
							list.Add(item);
							prevIndex = itemIndex;
						} else {
							i = j + 1;
							break;
						}
						if(j == 0)
							i = 0;
					}
					list.Reverse();
				}
				groups.Reverse();
				result.AddRange(groups);
			}
			return result;
		}
		public bool CanMoveSelectedItems(int targetID, InsertDirection direction) {
			var target = TreeListItemsHash[targetID];
			if(SelectedItems.Contains(target))
				return false;
			foreach(var item in SelectedItems) {
				if(!CanMoveItem(item, target, direction))
					return false;
			}
			return true;
		}
		public virtual bool CanMoveItem(IDesignTimeCollectionItem source, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(direction == InsertDirection.None)
				return false;
			if(direction == InsertDirection.Inside && target != null && !IsSupportHierarchy(target))
				return false;
			while(target != null) {
				if(target == source)
					return false;
				target = target.Parent;
			}
			return true;
		}
		protected virtual bool IsSelectionChangingEnabled() {
			return SelectedItems.Count > 0 && !SelectedItems.Exists(i => i.ReadOnly);
		}
		protected virtual Type GetSelectedItemsType() {
			var types = new HashSet<Type>();
			SelectedItems.ForEach(i => types.Add(i.GetType()));
			return types.Count == 1 ? types.First() : null;
		}
		public virtual bool CanCreatePopupMenuForDesignEditorDescriptorItem(DesignEditorDescriptorItem descriptorItem) {
			return descriptorItem.ActionType != DesignEditorMenuRootItemActionType.RetriveFields;
		}
	}
	public enum DesignEditorMenuRootItemActionType {
		AddItem,
		InsertBefore,
		InsertChild,
		Remove,
		RemoveInnerItems,
		MoveUp,
		MoveDown,
		MoveLeft,
		MoveRight,
		ChangeTo,
		RetriveFields,
		SelectAll,
		CreateDefaultItems,
		SetItemsAmount,
		MoveToNewGroup,
		IncreaseColumn,
		DecreaseColumn,
		IncreaseColSpan,
		DecreaseColSpan,
		IncreaseRowSpan,
		DecreaseRowSpan
	}
	public enum DesignEditorDescriptorItemType { Button, DropDown, DropDownButton, SpinEdit }
	public class DesignEditorDescriptorItem {
		List<object> parameters;
		public DesignEditorDescriptorItem() {
			ImageIndex = -1;
			ChildItems = new List<DesignEditorDescriptorItem>();
		}
		public string Caption { get; set; }
		public int ImageIndex { get; set; }
		public List<object> Parameters {
			get {
				if(parameters == null)
					parameters = new List<object>();
				return parameters;
			}
		}
		public object Value { get; set; }
		public bool BeginGroup { get; set; }
		public bool Enabled { get; set; }
		public bool HasChildItems { get { return ChildItems.Count > 0; } }
		public bool IsButtonItem { get { return EditorType == DesignEditorDescriptorItemType.Button || EditorType == DesignEditorDescriptorItemType.DropDown || EditorType == DesignEditorDescriptorItemType.DropDownButton; } }
		public DesignEditorDescriptorItem ParentItem { get; set; }
		public List<DesignEditorDescriptorItem> ChildItems { get; private set; }
		public DesignEditorDescriptorItemType EditorType { get; set; }
		public IDesignTimeColumnAndEditorItem ItemType { get; set; }
		public DesignEditorMenuRootItemActionType ActionType { get; set; }
	}
	public class TreeListItemNode {
		public TreeListItemNode(int id, int parentID, bool visible, bool selected, string caption, int imageIndex) {
			ID = id;
			ParentID = parentID;
			Visible = visible;
			Selected = selected;
			Caption = caption;
			ImageIndex = imageIndex;
		}
		public bool Visible { get; private set; }
		public bool Selected { get; private set; }
		public int ID { get; private set; }
		public int ParentID { get; private set; }
		public string Caption { get; private set; }
		public int ImageIndex { get; private set; }
	}
	public interface IDesignTimeColumnAndEditorItem {
		string Text { get; }
		string EditorName { get; }
		string Category { get; }
		Type ColumnType { get; }
		int ImageIndex { get; }
	}
	public class DesignTimeFieldInfo {
		string name;
		Type dataType;
		public DesignTimeFieldInfo(string name, Type dataType) {
			this.name = name;
			this.dataType = dataType;
			IsBindableType = true;
		}
		public string Name { get { return name; } }
		public Type DataType { get { return dataType; } }
		public bool IsPrimaryKey { get; set; }
		public bool IsBindableType { get; set; }
		public bool IsReadOnly { get; set; }
		public bool Identity { get; set; }
		public override string ToString() { return Name; }
	}
	public enum InsertDirection { Inside, Before, After, Left, Right, None };
	public class DesignTimeColumnType : IDesignTimeColumnAndEditorItem {
		Type columnType;
		string text;
		int imageIndex;
		string editorName;
		public DesignTimeColumnType(Type columnType, string text) :
			this(columnType, text, string.Empty, -1) {
		}
		public DesignTimeColumnType(Type columnType, string text, string editorName, int imageIndex) {
			this.columnType = columnType;
			this.text = text;
			this.imageIndex = imageIndex;
			this.editorName = editorName;
		}
		string IDesignTimeColumnAndEditorItem.Category { get { return string.Empty; } }
		Type IDesignTimeColumnAndEditorItem.ColumnType { get { return this.columnType; } }
		string IDesignTimeColumnAndEditorItem.Text { get { return this.text; } }
		int IDesignTimeColumnAndEditorItem.ImageIndex { get { return this.imageIndex; } }
		string IDesignTimeColumnAndEditorItem.EditorName { get { return this.editorName; } }
	}
	public class DesignTimeEditorDescriptorItems : List<DesignEditorDescriptorItem> {
		public DesignTimeEditorDescriptorItems()
			: base() {
		}
		public DesignEditorDescriptorItem this[DesignEditorMenuRootItemActionType type] { get { return Find(i => i.ActionType == type); } }
		public new DesignEditorDescriptorItem this[int index] { get { return base[index]; } set { base[index] = value; } }
	}
	public enum PropertyEditorType { TextEdit, ComboBox }
	public class PropertyEditorItems : List<PropertyEditorItemInfo> {
		public PropertyEditorItems(PropertyEditorType editorType, string dependendPropertyName) {
			EditorType = editorType;
			DependendPropertyName = dependendPropertyName;
		}
		public PropertyEditorType EditorType { get; private set; }
		public string DependendPropertyName { get; private set; }
		public bool HasDependendProperty { get { return !string.IsNullOrEmpty(DependendPropertyName); } }
		public void AddViewSelectorInfoItem(string viewName, string[] tabNames) {
			Add(new PropertyEditorItemInfo(viewName, tabNames));
		}
		public void AddViewSelectorInfoItem(string viewName, string[] tabNames, object propertyValueToSet) {
			Add(new PropertyEditorItemInfo(viewName, tabNames, propertyValueToSet));
		}
		public PropertyEditorItemInfo FindSelectorInfoByPropertyValue(object propertyValue) {
			return Find(vi => Object.Equals(vi.PropertyValueToSet, propertyValue));
		}
	}
	public class PropertyEditorItemInfo {
		public PropertyEditorItemInfo(string viewName, string[] tabNames)
			: this(viewName, tabNames, null) {
		}
		public PropertyEditorItemInfo(string viewName, string[] tabNames, object propertyValueToSet) {
			ViewName = viewName;
			TabNames = tabNames;
			PropertyValueToSet = propertyValueToSet;
		}
		public string ViewName { get; private set; }
		public string[] TabNames { get; private set; }
		public object PropertyValueToSet { get; private set; }
		public override string ToString() {
			return ViewName;
		}
	}
	public class GroupBase { }
	public interface IOwnerEditingProperty {
		object PropertyInstance { get; }
		bool ItemsChanged { get; set; }
		void BeforeClosed();
		void SaveUndo();
		void SaveChanges();
		void UndoChanges();
	}
}

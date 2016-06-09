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
using DevExpress.Utils;
namespace DevExpress.Web.Design {
	public class GridViewColumnPropertiesOwner : GridViewColumnsOwner {
		public GridViewColumnPropertiesOwner(object grid, IList columns, string[] filteredProperties)
			: base(grid, ((IComponent)grid).Site, columns) {
			FilteredProperties = filteredProperties;
		}
		string[] FilteredProperties { get; set; }
		public override string[] GetVisibleProperties() {
			return FilteredProperties;
		}
		public void CreateFirstAndUniqColumn(Type columnType) {
			if(!CanCreateUniqColumn(columnType))
				return;
			var designTimeItem = FindDesignTimeItem(columnType);
			if(Items.Count == 0) {
				AddItem(designTimeItem);
				return;
			}
			var firstColumn = GetOrderedItems().First();
			MoveItemTo(CreateNewItem(designTimeItem), firstColumn, InsertDirection.Before);
		}
		public bool CanCreateUniqColumn(Type columnType) {
			return FindColumnByType(columnType) == null;
		}
		public GridViewColumn FindColumnByType(Type columnType) {
			foreach(GridViewColumn column in Items) {
				if(column.GetType() == columnType)
					return column;
			}
			return null;
		}
		public bool CommandColumn_ShowNewButtonChecked() {
			return GetCommandColumnPropertyValue("ShowNewButton");
		}
		public bool CommandColumn_ShowEditButtonChecked() {
			return GetCommandColumnPropertyValue("ShowEditButton");
		}
		public bool CommandColumn_ShowDeleteButtonChecked() {
			return GetCommandColumnPropertyValue("ShowDeleteButton");
		}
		public bool CommandColumn_ShowSelectCheckboxChecked() {
			return GetCommandColumnPropertyValue("ShowSelectCheckbox");
		}
		public void CommandColumn_CheckNewButton() {
			CheckCommandColumnProperty("ShowNewButton");
		}
		public void CommandColumn_CheckEditButton() {
			CheckCommandColumnProperty("ShowEditButton");
		}
		public void CommandColumn_CheckDeleteButton() {
			CheckCommandColumnProperty("ShowDeleteButton");
		}
		public void CommandColumn_CheckShowSelectCheckbox() {
			CheckCommandColumnProperty("ShowSelectCheckbox");
		}
		protected override List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.ChangeTo, 
				DesignEditorMenuRootItemActionType.MoveUp, 
				DesignEditorMenuRootItemActionType.MoveDown, 
				DesignEditorMenuRootItemActionType.MoveLeft,
				DesignEditorMenuRootItemActionType.MoveRight,
				DesignEditorMenuRootItemActionType.SelectAll
			};
		}
		IOrderedEnumerable<IDesignTimeCollectionItem> GetOrderedItems() {
			return Items.OfType<IDesignTimeCollectionItem>().OrderBy(i => i.VisibleIndex);
		}
		bool GetCommandColumnPropertyValue(string propertyName) {
			var commandColumn = GetCommandColumn();
			return commandColumn != null ? Convert.ToBoolean(FeatureBrowserHelper.GetPropertyValue(commandColumn, propertyName)) : false;
		}
		void CheckCommandColumnProperty(string propertyName) {
			var commandColumn = GetOrCreateCommandColumn();
			var value = !Convert.ToBoolean(FeatureBrowserHelper.GetPropertyValue(commandColumn, propertyName));
			FeatureBrowserHelper.SetPropertyValue(commandColumn, propertyName, value);
		}
		GridViewCommandColumn GetOrCreateCommandColumn() {
			var commandColumn = GetCommandColumn();
			if(commandColumn == null) {
				CreateFirstAndUniqColumn(typeof(GridViewCommandColumn));
				commandColumn = (GridViewCommandColumn)GetOrderedItems().First();
			}
			return commandColumn;
		}
		GridViewCommandColumn GetCommandColumn() {
			return FindColumnByType(typeof(GridViewCommandColumn)) as GridViewCommandColumn;
		}
	}
	public class GridViewColumnsOwner : DataItemsEditorOwner {
		ASPxGridBase grid;
		public GridViewColumnsOwner(object grid, IServiceProvider provider, IList columns)
			: base(grid, "Columns", provider, columns) {
		}
		internal GridViewColumnsOwner(Collection items)
			: base(null, null, null, items) {
		}
		protected virtual ASPxGridBase Grid {
			get {
				if(grid == null)
					grid = GetGrid();
				return grid;
			}
		}
		protected override string KeyFieldName { get { return Grid.KeyFieldName; } set { Grid.KeyFieldName = value; } }
		protected virtual ASPxGridBase GetGrid() {
			return Designer != null ? Designer.Component as ASPxGridBase : null;
		}
		public override IDesignTimeCollectionItem CreateNewItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			ResetAutogenerateColumns();
			var item = base.CreateNewItem(designTimeItem);
			var binaryImageColumn = item as GridViewDataBinaryImageColumn;
			if(binaryImageColumn != null)
				binaryImageColumn.PropertiesBinaryImage.EditingSettings.Enabled = true;
			return item;
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(GridViewDataBinaryImageColumn), "Binary Image Column", "Binary Image", BinaryImageColumnImageResource);
			AddItemType(typeof(GridViewDataButtonEditColumn), "Button Edit Column", "Button Edit", ButtonEditColumnImageResource);
			AddItemType(typeof(GridViewDataCheckColumn), "Check Column", "Check Box", CheckColumnImageResource);
			AddItemType(typeof(GridViewDataColorEditColumn), "Color Edit Column", "Color Edit", ColorEditImageResource);
			AddItemType(typeof(GridViewDataComboBoxColumn), "Combo Box Column", "Combo Box", ComboboxImageResource);
			AddItemType(typeof(GridViewDataDateColumn), "Date Column", "Date Edit", DateEditImageResource);
			AddItemType(typeof(GridViewDataDropDownEditColumn), "DropDown Edit Column", "DropDown Edit", DropDownEditImageResource);
			AddItemType(typeof(GridViewDataHyperLinkColumn), "Hyperlink Column", "Hyperlink", HyperlinkImageResource);
			AddItemType(typeof(GridViewDataImageColumn), "Image Column", "Image", ImageImageResource);
			AddItemType(typeof(GridViewDataMemoColumn), "Memo Column", "Memo", MemoImageResource);
			AddItemType(typeof(GridViewDataProgressBarColumn), "Progress Bar Column", "Progress Bar", ProgressBarImageResource);
			AddItemType(typeof(GridViewDataSpinEditColumn), "Spin Edit Column", "Spin Edit", SpinEditImageResource);
			AddItemType(typeof(GridViewDataTextColumn), "Text Column", "Text Box", TextImageResource);
			AddItemType(typeof(GridViewDataTimeEditColumn), "Time Edit Column", "Time Edit", TimeEditImageResource);
			AddItemType(typeof(GridViewDataTokenBoxColumn), "Token Box Column", "Token Box", TokenBoxImageResource);
			AddItemType(typeof(GridViewCommandColumn), "Command Column", CommandColumnImageResource);
			AddItemType(typeof(GridViewBandColumn), "Band Column", BandColumnImageResource);
			AddItemType(typeof(GridViewDataColumn), "Data Column", DataColumnImageResource);
		}
		protected override void FillMenuItemResouceImages() {
			base.FillMenuItemResouceImages();
			AddResourceImage(AddChildItemImageResource);
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
				DesignEditorMenuRootItemActionType.ChangeTo, 
				DesignEditorMenuRootItemActionType.RetriveFields,
				DesignEditorMenuRootItemActionType.CreateDefaultItems
			};
		}
		protected override List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.ChangeTo, 
				DesignEditorMenuRootItemActionType.MoveUp, 
				DesignEditorMenuRootItemActionType.MoveDown, 
				DesignEditorMenuRootItemActionType.MoveLeft,
				DesignEditorMenuRootItemActionType.MoveRight,
				DesignEditorMenuRootItemActionType.SelectAll
			};
		}
		protected override IDesignTimeCollectionItem CreateDataItemCore(string fieldName) {
			ResetAutogenerateColumns();
			return CreteDataColumn(fieldName);
		}
		void ResetAutogenerateColumns() { 
			if(Grid != null)
				Grid.AutoGenerateColumns = false;
		}
		GridViewDataColumn CreteDataColumn(string fieldName) {
			var fieldInfo = FieldInfoList.FirstOrDefault(f => f.Name == fieldName);
			if(fieldInfo == null)
				return new GridViewDataTextColumn() { FieldName = fieldName };
			var column = GridViewEditDataColumn.CreateColumn(fieldInfo.DataType);
			column.FieldName = fieldInfo.Name;
			column.ReadOnly = fieldInfo.IsPrimaryKey || fieldInfo.IsReadOnly;
			if(fieldInfo.Identity) {
				column.EditFormSettings.Visible = DefaultBoolean.False;
			}
			return column;
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(designTimeItem.ColumnType == typeof(GridViewDataColumn))
				return false;
			return base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar);
		}
		protected override DesignEditorDescriptorItem CreateSubmenuItem(DesignEditorDescriptorItem parent, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			var menuItem = base.CreateSubmenuItem(parent, designTimeItem, isToolbar);
			if(menuItem.ItemType.ColumnType == typeof(GridViewCommandColumn))
				menuItem.BeginGroup = true;
			return menuItem;
		}
		protected internal override string GetItemPropertiesTabCaption() {
			return "Column Properties";
		}
		public override Type GetDefaultItemType() {
			return typeof(GridViewDataTextColumn);
		}
	}
	public class GridViewFormLayoutItemsOwner : FormLayoutItemsOwner {
		ASPxFormLayout formLayout;
		public GridViewFormLayoutItemsOwner(ASPxWebControl component, FormLayoutProperties layoutProperties, ItemsEditorOwner editorItemsOwner)
			: base(component, layoutProperties) {
			EditorItemsOwner = editorItemsOwner;
		}
		internal override ASPxFormLayout FormLayout {
			get {
				if(formLayout == null) {
					formLayout = new ASPxFormLayout();
					AssignFormLayoutProperties(LayoutProperties);
				}
				return formLayout;
			}
		}
		protected ASPxGridBase Grid { get { return (ASPxGridBase)Component; } }
		ItemsEditorOwner EditorItemsOwner { get; set; }
		internal override Type LayoutItemType { get { return typeof(GridViewColumnLayoutItem); } }
		internal override LayoutGroup RootLayoutGroup { get { return LayoutProperties.Root; } }
		protected override bool CanCreateNestedTypes { get { return false; } }
		protected override void FillItemTypes() {
			AddItemType(LayoutItemType, "Column Layout Item", string.Empty);
			AddItemType(typeof(EditModeCommandLayoutItem), "Command Layout Item", string.Empty);
			AddItemType(typeof(EmptyLayoutItem), "Empty Layout Item", string.Empty);
			AddItemType(typeof(GridViewLayoutGroup), "Layout Group", string.Empty);
			AddItemType(typeof(GridViewTabbedLayoutGroup), "Tabbed Layout Group", string.Empty);
		}
		protected override void InitializeOwner(object component) {
			if(LayoutProperties != null) {
				FormLayout.Properties.Assign(LayoutProperties);
				base.InitializeOwner(component);
			}
		}
		protected void AssignFormLayoutProperties(FormLayoutProperties properties) {
			if(properties != null) {
				FormLayout.Properties.Assign(properties);
				formLayout.Properties.DataOwner = properties.DataOwner;
			}
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
				DesignEditorMenuRootItemActionType.CreateDefaultItems
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
		protected override IDesignTimeColumnAndEditorItem FindEditorDesignTimeItem(IDesignTimeCollectionItem item) {
			var columnLayoutItem = item as ColumnLayoutItem;
			if(columnLayoutItem == null)
				return null;
			if(columnLayoutItem.ColumnInternal != null)
				return EditorItemsOwner.FindDesignTimeItem(columnLayoutItem.ColumnInternal.GetType());
			return EditorItemsOwner.FindDesignTimeItem(columnLayoutItem.GetType());
		}
		public override List<string> GetViewDependedProperties() {
			var result = base.GetViewDependedProperties();
			result.Add("ColumnName");
			return result;
		}
		protected override bool CanCreateRootMenuItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(actionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return isToolbarMenu;
			return base.CanCreateRootMenuItem(actionType, isToolbarMenu);
		}
		protected override DesignEditorDescriptorItem CreateDefaultItemsMenuItem() {
			var item = base.CreateDefaultItemsMenuItem();
			item.Enabled = Grid.Columns.Count > 0;
			return item;
		}
		public override void CreateDefaultLayout() {
			Items.Clear();
			LayoutProperties.Assign(GenerateDefaultLayout());
			FormLayout.Properties.Assign(LayoutProperties);
			RootItemWrapper = new RootLayoutGroupWrapper(RootLayoutGroup);
			RecreateTreeListItems(true);
		}
		protected virtual FormLayoutProperties GenerateDefaultLayout() {
			LayoutProperties.DataOwner = Grid;
			return ((IFormLayoutOwner)LayoutProperties).GenerateDefaultLayout(true);
		}
	}
	public class SummaryItemsOwner : FlatCollectionItemsOwner<ASPxSummaryItem> {
		public SummaryItemsOwner(ASPxGridView gridView, IServiceProvider provider, string summaryName, ASPxSummaryItemCollection summaryCollection)
			: base(gridView, provider, summaryCollection, summaryName) {
		}
	}
	public class GridViewAdaptiveDetailsFormLayoutItemsOwner : GridViewFormLayoutItemsOwner {
		public GridViewAdaptiveDetailsFormLayoutItemsOwner(ASPxGridView gridView, ItemsEditorOwner editorItemsOwner)
			: base(gridView, gridView.SettingsAdaptivity.AdaptiveDetailLayoutProperties, editorItemsOwner) {
		}
		protected override void FillItemTypes() {
			AddItemType(LayoutItemType, "Column Layout Item", string.Empty);
			AddItemType(typeof(EmptyLayoutItem), "Empty Layout Item", string.Empty);
			AddItemType(typeof(GridViewLayoutGroup), "Layout Group", string.Empty);
			AddItemType(typeof(GridViewTabbedLayoutGroup), "Tabbed Layout Group", string.Empty);
		}
		protected override FormLayoutProperties GenerateDefaultLayout() {
			LayoutProperties.DataOwner = Grid;
			return ((ASPxGridView)Grid).GenerateAdaptiveDefaultLayout(true);
		}
	}
	public class FormatConditionItemsOwner: ItemsEditorOwner {
		public FormatConditionItemsOwner(ASPxGridView gridView, IServiceProvider provider, GridViewFormatConditionCollection formatConditions)
			: base(gridView, "Format Conditions", provider, formatConditions) {
		}
		public override Type GetDefaultItemType() {
			return typeof(GridViewFormatConditionHighlight);
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(GridViewFormatConditionHighlight), "Highlight condition");
			AddItemType(typeof(GridViewFormatConditionTopBottom), "Top/Bottom condition");
			AddItemType(typeof(GridViewFormatConditionColorScale), "Color scale condition");
			AddItemType(typeof(GridViewFormatConditionIconSet), "Icon set condition");
		}
	}
}

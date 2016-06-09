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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraTreeList;
namespace DevExpress.Web.ASPxTreeList.Design {
	public class TreeListColumnsOwner : DataItemsEditorOwner {
		public TreeListColumnsOwner(object component, IServiceProvider provider)
			: base(component, "Columns", provider,  ((ASPxTreeList)component).Columns) {
		}
		internal TreeListColumnsOwner(Collection items)
			: base(null, null, null, items) {
		}
		protected ASPxTreeList TreeList { get { return Component as ASPxTreeList; } }
		protected override string KeyFieldName { get { return TreeList.KeyFieldName; } set { TreeList.KeyFieldName = value; } }
		public override IDesignTimeCollectionItem CreateNewItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			var item = base.CreateNewItem(designTimeItem);
			var binaryImageColumn = item as TreeListBinaryImageColumn;
			if(binaryImageColumn != null)
				binaryImageColumn.PropertiesBinaryImage.EditingSettings.Enabled = true;
			return item;
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(TreeListBinaryImageColumn), "Binary Image Column", "Binary Image", BinaryImageColumnImageResource);
			AddItemType(typeof(TreeListButtonEditColumn), "Button Edit Column", "Button Edit", ButtonEditColumnImageResource);
			AddItemType(typeof(TreeListCheckColumn), "Check Column", "Check Box", CheckColumnImageResource);
			AddItemType(typeof(TreeListColorEditColumn), "Color Edit Column", "Color Edit", ColorEditImageResource);
			AddItemType(typeof(TreeListComboBoxColumn), "Combo Box Column", "Combo Box", ComboboxImageResource);
			AddItemType(typeof(TreeListDateTimeColumn), "Date Column", "Date Edit", DateEditImageResource);
			AddItemType(typeof(TreeListDropDownEditColumn), "DropDown Edit Column", "DropDown Edit", DropDownEditImageResource);
			AddItemType(typeof(TreeListHyperLinkColumn), "Hyperlink Column", "Hyperlink", HyperlinkImageResource);
			AddItemType(typeof(TreeListImageColumn), "Image Column", "Image", ImageImageResource);
			AddItemType(typeof(TreeListMemoColumn), "Memo Column", "Memo", MemoImageResource);
			AddItemType(typeof(TreeListProgressBarColumn), "Progress Bar Column", "Progress Bar", ProgressBarImageResource);
			AddItemType(typeof(TreeListSpinEditColumn), "Spin Edit Column", "Spin Edit", SpinEditImageResource);
			AddItemType(typeof(TreeListTextColumn), "Text Column", "Text Box", TextImageResource);
			AddItemType(typeof(TreeListTimeEditColumn), "Time Edit Column", "Time Edit", TimeEditImageResource);
			AddItemType(typeof(TreeListTokenBoxColumn), "Token Box Column", "Token Box", TokenBoxImageResource);
			AddItemType(typeof(TreeListDataColumn), "Data column", DataColumnImageResource);
			AddItemType(typeof(TreeListCommandColumn), "Command Column", CommandColumnImageResource);
		}
		protected override IDesignTimeCollectionItem CreateDataItemCore(string fieldName) {
			TreeList.AutoGenerateColumns = false;
			return CreateTreeListDataColumn(fieldName);
		}
		TreeListDataColumn CreateTreeListDataColumn(string fieldName) {
			var fieldInfo = FieldInfoList.FirstOrDefault(f => f.Name == fieldName);
			if(fieldInfo == null)
				return new TreeListTextColumn() { FieldName = fieldName };
			var column = new TreeListDataColumn(fieldInfo.Name);
			column.FieldName = fieldInfo.Name;
			column.ReadOnly = fieldInfo.IsPrimaryKey || fieldInfo.IsReadOnly;
			if(fieldInfo.Identity)
				column.EditFormSettings.Visible = DefaultBoolean.False;
			return column;
		}
		protected override bool CanCreateRootMenuItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(isToolbarMenu && actionType == DesignEditorMenuRootItemActionType.InsertChild)
				return false;
			return base.CanCreateRootMenuItem(actionType, isToolbarMenu);
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(designTimeItem.ColumnType == typeof(TreeListDataColumn))
				return false;
			return base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar);
		}
		protected override DesignEditorDescriptorItem CreateSubmenuItem(DesignEditorDescriptorItem parent, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			var menuItem = base.CreateSubmenuItem(parent, designTimeItem, isToolbar);
			if(menuItem.ItemType.ColumnType == typeof(TreeListCommandColumn))
				menuItem.BeginGroup = true;
			return menuItem;
		}
		public override Type GetDefaultItemType() {
			return typeof(TreeListTextColumn);
		}
		protected internal override string GetItemPropertiesTabCaption() {
			return "Column Properties";
		}
	}
}

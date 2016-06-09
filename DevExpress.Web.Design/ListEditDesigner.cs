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
using System.Windows.Forms;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxListEditDesigner : ASPxEditDesigner {
		public override bool ShowValueTypeProperty {
			get { return true; }
		}
		protected ASPxListEdit ListEdit {
			get { return Component as ASPxListEdit; }
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Items", "Items");
		}
		protected override string GetBaseProperty() {
			return "Properties.Items";
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new ListEditItemsOwner(ListEdit, DesignerHost)));
		}
		protected override string GetEmptyDesignTimeHtmlInternal() {
			return base.GetEmptyDesignTimeHtmlInternalBase();
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxListEdit edit = dataControl as ASPxListEdit;
			if(!string.IsNullOrEmpty(edit.DataSourceID) || edit.DataSource != null || edit.Items.Count < 1) {
				edit.Items.Clear();
				base.DataBind(edit);
			}
		}
		protected override IEnumerable GetSampleDataSource() {
			return new ListEditSampleDataSource(Component);
		}
	}
	public class ListEditItemsOwner : FlatCollectionItemsOwner<ListEditItem> {
		public ListEditItemsOwner(object control, IServiceProvider provider) 
			: base(control, provider, ((ASPxListEdit)control).Items) {
		}
		public ListEditItemsOwner(object component, IServiceProvider provider, ListEditItemCollection items)
			: base(component, provider, items, "Items") {
		}
		protected override IDesignTimeCollectionItem FindParentItem(IDesignTimeCollectionItem itemToSearch, IDesignTimeCollectionItem parent = null) {
			return null;
		}		
		protected override IList FindItemCollection(IDesignTimeCollectionItem item) {
			return Items;
		}
	}
	public class ASPxListBoxDesigner : ASPxListEditDesigner {
		protected ASPxListBox ListBox {
			get { return Component as ASPxListBox; }
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Columns", "Columns");
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new ListBoxCommonFormDesigner(ListBox, DesignerHost)));
		}
	}
	public class ListEditSampleDataItem {
		public string Text {
			get { return "Unbound"; }
		}
		public object Value {
			get { return null; }
		}
		public override string ToString() {
			return null;
		}
	}
	public class ListEditSampleDataSource : IEnumerable {
		private IComponent component;
		public ListEditSampleDataSource(IComponent component) {
			this.component = component;
		}
		private bool IsMultiColumn {
			get {
				ASPxListBox listBox = component as ASPxListBox;
				if(listBox != null)
					return listBox.IsMultiColumn;
				else {
					ASPxComboBox comboBox = component as ASPxComboBox;
					if(comboBox != null)
						return comboBox.IsMultiColumn;
				}
				return false;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			List<ListEditSampleDataItem> items = new List<ListEditSampleDataItem>();
			if(!IsMultiColumn)
				items.Add(new ListEditSampleDataItem());
			return items.GetEnumerator();
		}
	}
	public static class ListBoxDesignerHelper {
		public static object GetDataSourceOwner(ITypeDescriptorContext context) {
			object propertyOwner = context.Instance;
			object dataSourceOwner = GetDataSourceOwnerFromColumn(propertyOwner);
			if(dataSourceOwner == null)
				dataSourceOwner = GetDataSourceOwnerFromProperties(propertyOwner);
			return dataSourceOwner;
		}
		static object GetDataSourceOwnerFromColumn(object propertyOwner) {
			ListBoxColumn column = propertyOwner as ListBoxColumn;
			if(column != null) {
				object columnOwner = column.Collection.Owner;
				return GetDataSourceOwnerFromProperties(columnOwner);
			}
			return null;
		}
		static object GetDataSourceOwnerFromProperties(object propertyOwner) {
			ComboBoxListBoxProperties comboBoxListBoxProps = propertyOwner as ComboBoxListBoxProperties;
			if(comboBoxListBoxProps != null) {
				if(comboBoxListBoxProps.ComboBox != null)
					return comboBoxListBoxProps.ComboBox;
				else
					return comboBoxListBoxProps.ComboBoxProperties;
			} else {
				ListBoxProperties listBoxProps = propertyOwner as ListBoxProperties;
				if(listBoxProps.ListBox != null)
					return listBoxProps.ListBox;
				else
					return listBoxProps;
			}
		}
	}
	public class ListBoxCommonFormDesigner : CommonFormDesigner {
		ASPxListBox listBox;
		public ListBoxCommonFormDesigner(ASPxListBox listBox, IServiceProvider provider)
			: base(listBox, provider) { 
		}
		ASPxListBox ListBox {
			get {
				if(listBox == null)
					listBox = (ASPxListBox)Control;
				return listBox;
			}
		}
		protected override void CreateMainGroupItems() {
			AddItemsItem();
			AddColumnsItem();
			CreateClientSideEventsItem();
		}
		protected void AddItemsItem() {
			MainGroup.Add(CreateDesignerItem(new FlatCollectionItemsOwner<ListEditItem>(ListBox, Provider, ListBox.Items), typeof(ItemsEditorFrame), ItemsItemImageIndex));
		}
		protected void AddColumnsItem() {
			MainGroup.Add(CreateDesignerItem(new ListBoxColumnsOwner(ListBox, Provider), typeof(ItemsEditorFrame), ColumnsItemImageIndex));
		}
	}
	class ListBoxColumnsOwner : FlatCollectionItemsOwner<ListBoxColumn> {
		public ListBoxColumnsOwner(IListBoxColumnsOwner listBoxControl, IServiceProvider provider)
			: base(listBoxControl, provider, listBoxControl.Columns, "Columns") {
		}
		public ListBoxColumnsOwner(object component, IServiceProvider provider, ListBoxColumnCollection columns)
			: base(component, provider, columns, "Columns") {
		}
	}
}

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
using System.Windows.Forms;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxComboBoxDesigner : ASPxButtonEditDesigner {
		protected ASPxComboBox ComboBox {
			get { return Component as ASPxComboBox; }
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Items", "Items");
			propertyNameToCaptionMap.Add("Columns", "Columns");
		}
		public override bool ShowValueTypeProperty {
			get { return true; }
		}
		public override void RunDesigner() {
			var comboBoxFormDesigner = new ComboBoxCommonFormDesigner(ComboBox, DesignerHost);
			ShowDialog(new WrapperEditorForm(comboBoxFormDesigner));
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ComboBoxDesignerActionList(this);
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxComboBox edit = dataControl as ASPxComboBox;
			if(!string.IsNullOrEmpty(edit.DataSourceID) || edit.DataSource != null || edit.Items.Count < 1) {
				edit.Items.Clear();
				base.DataBind(edit);
			}
		}
		protected override IEnumerable GetSampleDataSource() {
			return new ListEditSampleDataSource(Component);
		}
	}
	public class ComboBoxDesignerActionList : ButtonEditDesignerActionList {
		public ComboBoxDesignerActionList(ASPxComboBoxDesigner designer) 
			: base(designer) {
		}
		protected override string GetClearButtonHint() {
			return StringResources.ASPxComboBoxActionList_ClearButtonVisibilityHint;
		}
	}
	public sealed class TextFormatStringUIEditor : DropDownUITypeEditorBase {
		protected override void ApplySelectedValue(ListBox valueList, ITypeDescriptorContext context) {
			ComboBoxProperties props = GetComboBoxProperties(context);
			props.TextFormatString = valueList.SelectedItem.ToString();
		}
		protected override void FillValueList(ListBox valueList, ITypeDescriptorContext context) {
			ComboBoxProperties props = GetComboBoxProperties(context);
			valueList.Items.Add(props.DefaultTextFormatString);
		}
		protected override PropertyDescriptor GetChangedPropertyDescriptor(object component) {
			return null;
		}
		protected override object GetComponent(ITypeDescriptorContext context) {
			return GetComboBoxProperties(context).ComboBox;
		}
		protected override void SetInitiallySelectedValue(ListBox valueList, ITypeDescriptorContext context) {
			ComboBoxProperties props = GetComboBoxProperties(context);
			valueList.SelectedItem = props.TextFormatString;
		}
		private ComboBoxProperties GetComboBoxProperties(ITypeDescriptorContext context) {
			ASPxComboBox comboBox = context.Instance as ASPxComboBox;
			if(comboBox != null)
				return comboBox.Properties;
			else {
				ComboBoxProperties props = context.Instance as ComboBoxProperties;
				if(props != null)
					return props;
				else
					throw new ArgumentException("Unable to extract the ComboBoxProperties from the type descriptor context.");
			}
		}
	}
	public class ComboBoxCommonFormDesigner : CommonFormDesigner {
		ASPxComboBox comboBox;
		public ComboBoxCommonFormDesigner(ASPxComboBox comboBox, IServiceProvider provider)
			: base(comboBox, provider) { 
		}
		ASPxComboBox ComboBox {
			get {
				if(comboBox == null)
					comboBox = (ASPxComboBox)Control;
				return comboBox;
			}
		}
		protected override void CreateMainGroupItems() {
			AddItemsItem();
			AddButtonsItem();
			AddColumnsItem();
			CreateClientSideEventsItem();
		}
		protected void AddItemsItem() {
			MainGroup.Add(CreateDesignerItem(new ComboBoxItemsOwner(ComboBox, Provider, ComboBox.Items), typeof(ItemsEditorFrame), ItemsItemImageIndex));
		}
		protected void AddButtonsItem() {
			MainGroup.Add(CreateDesignerItem(new ButtonEditButtonsOwner((ASPxButtonEditBase)ComboBox, Provider), typeof(ItemsEditorFrame), ButtonsItemImageIndex));
		}
		protected void AddColumnsItem() {
			MainGroup.Add(CreateDesignerItem(new ListBoxColumnsOwner(ComboBox, Provider), typeof(ItemsEditorFrame), ColumnsItemImageIndex));
		}
	}
	public class ComboBoxItemsOwner : FlatCollectionItemsOwner<ListEditItem> {
		public ComboBoxItemsOwner(ASPxComboBox comboBox) 
			: base(null, null, comboBox.Items) { 
		}
		public ComboBoxItemsOwner(object component, IServiceProvider provider, Collection items) 
			: base(component, provider, items) {
		}
		public override IDesignTimeCollectionItem CreateNewItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			var item = (ListEditItem)base.CreateNewItem(designTimeItem);
			var orderedCollection = Items.OfType<IDesignTimeCollectionItem>().OrderBy(i => i.VisibleIndex);
			item.Value = orderedCollection.Count();
			item.Text = item.GetType().Name;
			return item;
		}
		protected override TreeListItemNode CreateTreeListItemNode(int nodeID, int parentNodeID, IDesignTimeCollectionItem item, int imageIndex) {
			return new TreeListItemNode(nodeID, parentNodeID, item.Visible, ((ListEditItem)item).Selected, item.Caption, imageIndex);
		}
		public override List<string> GetViewDependedProperties() {
			var result = base.GetViewDependedProperties();
			result.Add("Selected");
			result.Add("Value");
			return result;
		}
	}
}

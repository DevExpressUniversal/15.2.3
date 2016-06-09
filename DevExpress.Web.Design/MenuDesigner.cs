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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public enum MenuDesignerViewType { Static, Dynamic }
	public class ASPxMenuDesignerBase : ASPxHierarchicalDataWebControlDesigner {
		private static string fItemTemplateCaption = "Item[{0}]";
		private static string fItemNamePrefix = "Item[{0}].";
		private static string[] fItemTemplateNames = new string[] { "Template", "TextTemplate", "SubMenuTemplate" };
		private static string[] fControlTemplateNames = new string[] { "ItemTemplate", "ItemTextTemplate", "RootItemTemplate", "RootItemTextTemplate", "SubMenuTemplate" };
		protected ASPxMenuBase Menu {
			get { return (ASPxMenuBase)Component; }
		}
		private MenuDesignerViewType view = MenuDesignerViewType.Static;
		public MenuDesignerViewType View {
			get { return view; }
			set {
				view = value;
				TypeDescriptor.Refresh(Component);
				UpdateDesignTimeHtml();
			}
		}
		public virtual bool ShowDesignViewType {
			get { return true; }
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			view = GetDefaultViewType();
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected virtual MenuDesignerViewType GetDefaultViewType() {
			return MenuDesignerViewType.Static;
		}
		protected override void FillPropertyNameToCaptionMap(System.Collections.Generic.Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Items", "Items");
		}
		protected override string GetBaseProperty() {
			return "Items";
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new MenuDesignerActionListBase(this);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new MenuItemsOwner(Menu, DesignerHost)));
		}
		protected override string GetDesignTimeHtmlInternal() {
			((IControlDesignerAccessor)base.ViewControl).UserData["ViewType"] = (int)View;
			((IControlDesignerAccessor)base.ViewControl).GetDesignModeState();
			return base.GetDesignTimeHtmlInternal();
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxMenuBase menu = dataControl as ASPxMenuBase;
			if (!string.IsNullOrEmpty(menu.DataSourceID) || (menu.DataSource != null) || !menu.HasVisibleItems()) {
				menu.Items.Clear();
				base.DataBind(menu);
			}
		}
		protected override IHierarchicalEnumerable GetSampleDataSource() {
			return new MenuHierarchicalSampleData(0, string.Empty);
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			CreateItemTemplateDefinitions(templateGroups, Menu.RootItem, "");
			for(int i = 0; i < fControlTemplateNames.Length; i++) {
				TemplateGroup templateGroup = new TemplateGroup(fControlTemplateNames[i]);
				TemplateDefinition templateDefinition = new TemplateDefinition(this, fControlTemplateNames[i],
					Component, fControlTemplateNames[i],
					GetTemplateStyle(null, fControlTemplateNames[i]));
				templateDefinition.SupportsDataBinding = true;
				templateGroup.AddTemplateDefinition(templateDefinition);
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
		protected void CreateItemTemplateDefinitions(TemplateGroupCollection templateGroups, MenuItem item, string namePrefix) {
			MenuItemCollection items = item.Items;
			for (int i = 0; i < items.Count; i++) {
				TemplateGroup templateGroup = new TemplateGroup(namePrefix + string.Format(fItemTemplateCaption, i));
				for (int j = 0; j < fItemTemplateNames.Length; j++) {
					TemplateDefinition templateDefinition = new TemplateDefinition(this, fItemTemplateNames[j],
						item.Items[i], fItemTemplateNames[j],
						GetTemplateStyle(item.Items[i], fItemTemplateNames[j]));
					templateDefinition.SupportsDataBinding = true;
					templateGroup.AddTemplateDefinition(templateDefinition);
				}
				templateGroups.Add(templateGroup);
				CreateItemTemplateDefinitions(templateGroups, item.Items[i], namePrefix + string.Format(fItemNamePrefix, i));
			}
		}
		protected Style GetTemplateStyle(MenuItem item, string templateName) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Menu.GetMenuStyle(item));
			if (templateName == "TextTemplate" || templateName == "ItemTextTemplate")
				style.CopyFrom(Menu.GetItemStyle(item));
			return style;
		}
		protected internal override string[] GetDataBindingSchemaFields() {
			return new string[] { "Enabled", "Name", "NavigateUrl", "Target", "Text", "ToolTip" };
		}
		protected internal override Type GetDataBindingSchemaItemType() {
			return typeof(MenuItem); 
		}
	}
	public class MenuDesignerActionListBase : ASPxWebControlDesignerActionList {
		protected ASPxMenuDesignerBase MenuDesigner { get { return Designer as ASPxMenuDesignerBase; } }
		public MenuDesignerViewType View {
			get { return MenuDesigner.View; }
			set { MenuDesigner.View = value; }
		}
		public MenuDesignerActionListBase(ASPxMenuDesignerBase menuDesigner)
			: base(menuDesigner) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			if(MenuDesigner.ShowDesignViewType)
				collection.Add(new DesignerActionPropertyItem("View",
					StringResources.MenuActionList_View,
					StringResources.ActionList_MiscCategory,
					StringResources.MenuActionList_ViewDescription));
			return collection;
		}
	}
	public class MenuItemsOwner : HierarchicalItemOwner<MenuItem> {
		public MenuItemsOwner(object control, IServiceProvider provider) 
			: base(control, provider, ((ASPxMenuBase)control).Items) {
		}
		public MenuItemsOwner(MenuItemCollection items) 
			: base(items) {
		}
	}
}

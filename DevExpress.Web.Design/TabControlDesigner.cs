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
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Utils.About;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxTabControlDesignerBase : ASPxHierarchicalDataWebControlDesigner {
		private ASPxTabControlBase TabControl { get; set; }
		private static string[] fTabTemplateNames = new string[] { "TabTemplate", "ActiveTabTemplate" };
		private static string[] fControlTemplateNames = new string[] { "TabTemplate", "ActiveTabTemplate", "SpaceBeforeTabsTemplate", "SpaceAfterTabsTemplate" };
		protected internal ASPxTabControlBase TabControlBase {
			get { return TabControl; }
		}
		public override void Initialize(IComponent component) {
			TabControl = (ASPxTabControlBase)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		public TabPosition TabPosition {
			get { return TabControlBase.TabPosition; }
			set {
				TabControlBase.TabPosition = value;
				PropertyChanged("TabPosition");
			}
		}
		public bool EnableTabScrolling {
			get { return TabControlBase.EnableTabScrolling; }
			set {
				TabControlBase.EnableTabScrolling = value;
				PropertyChanged("EnableTabScrolling");
			}
		}
		public override void ShowAbout() {
			TabControlBaseAboutDialogHelper.ShowAbout(Component.Site);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new TabControlDesignerActionList(this);
		}
		public virtual string GetEditTabsAction() {
			return "";
		}
		public virtual string GetEditTabsActionDescription() {
			return "";
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxTabControlBase tabControl = dataControl as ASPxTabControlBase;
			if(!string.IsNullOrEmpty(tabControl.DataSourceID) || (tabControl.DataSource != null) || !tabControl.HasVisibleTabs()) {
				tabControl.TabItems.Clear();
				base.DataBind(tabControl);
			}
		}
		protected override IHierarchicalEnumerable GetSampleDataSource() {
			return new TabControlHierarchicalSampleData(0, string.Empty);
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			TabCollectionBase tabs = TabControl.TabItems;
			if(tabs.Count > 0) {
				for(int i = 0; i < tabs.Count; i++) {
					TabBase tab = tabs[i] as TabBase;
					TemplateGroup templateGroup = new TemplateGroup(string.Format("Tab[{0}]", i));
					for(int j = 0; j < fTabTemplateNames.Length; j++) {
						TemplateDefinition templateDefinition = new TemplateDefinition(this,
							fTabTemplateNames[j], tab, fTabTemplateNames[j],
							GetTemplateStyle(tab, fTabTemplateNames[j]));
						templateDefinition.SupportsDataBinding = true;
						templateGroup.AddTemplateDefinition(templateDefinition);
					}
					templateGroups.Add(templateGroup);
				}
			}
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
		private Style GetTemplateStyle(TabBase tab, string templateName) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(TabControl.GetControlStyle());
			switch (templateName) {
				case "TabTemplate":
					style.CopyFrom(TabControl.GetTabStyle(tab, false));
					break;
				case "ActiveTabTemplate":
					style.CopyFrom(TabControl.GetTabStyle(tab, true));
					break;
			}
			return style;
		}
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			if(TabControl.ShowTabsInternal) {
				int regionCount = regions.Count;
				for(int i = 0; i < TabControl.TabItems.GetVisibleTabItemCount(); i++) {
					TabBase tab = TabControl.TabItems.GetVisibleTabItem(i);
					regions.Add(new TabControlSelectableRegion(this, "Activate " + this.GetRegionName(tab), tab));
					WebControl tabItemControl = ViewControl.FindControl(TabControlBase.GetTabElementID(tab, false)) as WebControl;
					tabItemControl.Attributes[DesignerRegion.DesignerRegionAttributeName] = (regionCount + i).ToString();
				}
			} else
				base.AddDesignerRegions(regions);
		}
		protected string GetRegionName(TabBase tab) {
			if (tab.Text != "")
				return tab.Text;
			else if (tab.Name != "") 
				return tab.Name;
			else
				return ("[Tab (" + tab.Index + ")]");
		}
		protected override void OnClick(DesignerRegionMouseEventArgs e) {
			base.OnClick(e);
			TabControlSelectableRegion region = e.Region as TabControlSelectableRegion;
			if (region != null) {
				TabControl.ActiveTabIndex = region.Tab.Index;
				PropertyChanged("ActiveTabIndex");
			}
		}
		protected internal override string[] GetDataBindingSchemaFields() {
			return new string[] { "Enabled", "Text", "ToolTip" };
		}
		protected internal override Type GetDataBindingSchemaItemType() {
			return typeof(TabBase);
		}
	}
	public class TabControlBaseAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxTabControlBase), new ProductKind[] { ProductKind.FreeOffer, ProductKind.DXperienceASP });
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if (ShouldShowTrialAbout(typeof(ASPxTabControlBase)))
				ShowAbout(provider);
		}
	}
	public class TabControlSelectableRegion: DesignerRegion {
		private TabBase fTab = null;
		public TabBase Tab {
			get { return fTab; }
		}
		public TabControlSelectableRegion(ASPxTabControlDesignerBase designer, string name, TabBase tab)
			: base(designer, name, true) {
			fTab = tab;
		}
	}
	public class TabControlDesignerActionList: ASPxWebControlDesignerActionList {
		protected ASPxTabControlDesignerBase TabControlDesigner { get { return (ASPxTabControlDesignerBase)Designer; } }
		public int ActiveTabIndex {
			get { return TabControlDesigner.TabControlBase.ActiveTabIndex; }
			set {
				IComponent component = TabControlDesigner.Component;
				TypeDescriptor.GetProperties(component)["ActiveTabIndex"].SetValue(component, value);
			}
		}
		public TabControlDesignerActionList(ASPxTabControlDesignerBase tabControlDesigner)
			: base(tabControlDesigner) {
		}
		public TabPosition TabPosition {
			get { return TabControlDesigner.TabPosition; }
			set { TabControlDesigner.TabPosition = value; }
		}
		public bool EnableTabScrolling {
			get { return TabControlDesigner.EnableTabScrolling; }
			set { TabControlDesigner.EnableTabScrolling = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Insert(1, new DesignerActionPropertyItem("ActiveTabIndex",
				StringResources.TabControlActionList_ActiveTabIndex,
				StringResources.ActionList_MiscCategory,
				StringResources.TabControlActionList_ActiveTabIndexDescription));
			collection.Add(new DesignerActionPropertyItem("EnableTabScrolling", "Enable Tab Scrolling"));
			collection.Add(new DesignerActionPropertyItem("TabPosition", "Tab Position"));
			return collection;
		}
	}
	public class ASPxTabControlDesigner: ASPxTabControlDesignerBase {
		private ASPxTabControl TabControl { get; set; }
		public override void Initialize(IComponent component) {
			TabControl = (ASPxTabControl)component;
			base.Initialize(component);
		}
		protected override void FillPropertyNameToCaptionMap(System.Collections.Generic.Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Tabs", "Tabs");
		}
		protected override string GetBaseProperty() {
			return "Tabs";
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new TabControlCommonDesigner(TabControl, DesignerHost)));
		}
		public override string GetEditTabsAction() {
			return StringResources.TabControlActionList_EditTabs;
		}
		public override string GetEditTabsActionDescription() {
			return StringResources.TabControlActionList_EditTabsDescription;
		}
		protected internal override string[] GetDataBindingSchemaFields() {
			return new string[] { "Enabled", "NavigateUrl", "Target", "Text", "ToolTip" };
		}
		protected internal override Type GetDataBindingSchemaItemType() {
			return typeof(Tab); 
		}
	}
	public class TabControlCommonDesigner : CommonFormDesigner {
		public TabControlCommonDesigner(ASPxTabControl tabControl, IServiceProvider provider)
			: base(new TabControlTabsOwner(tabControl, provider)) {
			ItemsImageIndex = TabPagesImageIndex;
		}
	}
	public class TabControlTabsOwner : FlatCollectionItemsOwner<Tab> {
		public TabControlTabsOwner(ASPxTabControl tabControl, IServiceProvider provider) 
			: base(tabControl, provider, tabControl.Tabs, "Tabs") {
		}
	}
}

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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxRibbonDesigner : ASPxHierarchicalDataWebControlDesigner {
		protected internal ASPxRibbon Ribbon { get; private set; }
		public override void Initialize(IComponent component) {
			Ribbon = (ASPxRibbon)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Tabs", "Items");
		}
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			int regionCount = regions.Count;
			var ribbonViewControl = (ASPxRibbon)ViewControl;
			for(int i = 0; i < Ribbon.Tabs.GetVisibleItemCount(); i++) {
				RibbonTab tab = Ribbon.Tabs.GetVisibleItem(i);
				TabBase tabControlTab = ribbonViewControl.RibbonControl.TabControl.TabItems.GetVisibleTabItem(Ribbon.ShowFileTab ? (i + 1) : i);
				regions.Add(new RibbonTabSelectableRegion(this, "Activate " + "[Tab (" + tab.Index + ")]", tab));
				WebControl tabItemControl = ribbonViewControl.RibbonControl.TabControl.FindControl(ribbonViewControl.RibbonControl.TabControl.GetTabElementID(tabControlTab, false)) as WebControl;
				tabItemControl.Attributes[DesignerRegion.DesignerRegionAttributeName] = (regionCount + i).ToString();
			}
		}
		protected override void OnClick(DesignerRegionMouseEventArgs e) {
			base.OnClick(e);
			RibbonTabSelectableRegion region = e.Region as RibbonTabSelectableRegion;
			if(region != null) {
				Ribbon.ActiveTabIndex = region.Tab.Index;
				PropertyChanged("ActiveTabIndex");
			}
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new RibbonDesignerActionList(this);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new RibbonCommonDesigner(Ribbon, DesignerHost)));
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			TemplateGroup fileTabTmplGrp = new TemplateGroup("FileTabTemplate");
			fileTabTmplGrp.AddTemplateDefinition(new TemplateDefinition(this, "FileTabTemplate", Ribbon, "FileTabTemplate", Ribbon.Styles.GetFileTabStyle()));
			templateGroups.Add(fileTabTmplGrp);
			var templateItems = Ribbon.Tabs.SelectMany(t => t.Groups).SelectMany(g => g.Items).Where(i => i is RibbonTemplateItem).Select(i => (RibbonTemplateItem)i).ToList();
			foreach(RibbonTab tab in Ribbon.Tabs) {
				foreach(RibbonGroup group in tab.Groups.Where(g => g.Items.Any(i => i is RibbonTemplateItem))) {
					var items = group.Items.Where(i => i is RibbonTemplateItem).ToList();
					string tmplGroupName = string.Format("Tab{0}.Group{1}", GetTemplateSectionSubName(tab.Index, tab.Name), GetTemplateSectionSubName(group.Index, group.Name));
					if(items.Count == 1)
						tmplGroupName += string.Format(".Item{0}", GetTemplateSectionSubName(items[0].Index, items[0].Name));
					TemplateGroup tmplItemsGroup = new TemplateGroup(tmplGroupName);
					foreach(RibbonItemBase item in group.Items.Where(i => i is RibbonTemplateItem)) {
						TemplateDefinition templateDefinition = new TemplateDefinition(this, items.Count == 1 ? "Template" : ("Item" + GetTemplateSectionSubName(item.Index, item.Name)), item, "Template", Ribbon.Styles.GetItemStyle(item));
						templateDefinition.SupportsDataBinding = true;
						tmplItemsGroup.AddTemplateDefinition(templateDefinition);
					}
					templateGroups.Add(tmplItemsGroup);
				}
			}
			return templateGroups;
		}
		string GetTemplateSectionSubName(int index, string name) {
			var sb = new StringBuilder();
			sb.AppendFormat("[{0}]", index);
			if(!string.IsNullOrEmpty(name))
				sb.AppendFormat("({0})", name);
			return sb.ToString();
		}
		protected override IHierarchicalEnumerable GetSampleDataSource() {
			return new RibbonHierarchicalSampleData(0, string.Empty);
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxRibbon ribbon = dataControl as ASPxRibbon;
			if(!string.IsNullOrEmpty(ribbon.DataSourceID) || (ribbon.DataSource != null) || ribbon.Tabs.GetVisibleItemCount() == 0) {
				ribbon.Tabs.Clear();
				base.DataBind(ribbon);
			}
		}
	}
	public class RibbonTabSelectableRegion : DesignerRegion {
		public RibbonTabSelectableRegion(ASPxRibbonDesigner designer, string name, RibbonTab tab)
			: base(designer, name, true) {
				Tab = tab;
		}
		public RibbonTab Tab { get; private set; }
	}
	public class RibbonDesignerActionList : ASPxWebControlDesignerActionList {
		private ASPxRibbonDesigner designer = null;
		public RibbonDesignerActionList(ASPxRibbonDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		public new ASPxRibbonDesigner Designer {
			get { return designer; }
		}
		public ASPxRibbon Ribbon {
			get { return Designer.Ribbon; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("ShowFileTab", "Show File Tab"));
			collection.Add(new DesignerActionPropertyItem("AllowMinimize", "Allow Minimize"));
			return collection;
		}
		public bool ShowFileTab {
			get { return Ribbon.ShowFileTab; }
			set { 
				Ribbon.ShowFileTab = value;
				Designer.PropertyChanged("ShowFileTab");
			}
		}
		public bool AllowMinimize {
			get { return Ribbon.AllowMinimize; }
			set {
				Ribbon.AllowMinimize = value;
				Designer.PropertyChanged("ShowMinimizeButton");
			}
		}
	}
	public class RibbonDesignerHelper {
		public static void FireRibbonControlPropertyChanged(IServiceProvider provider, ASPxRibbon control, string propertyName, IComponent component) {
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(component)[propertyName];
			System.Web.UI.Design.ControlDesigner.InvokeTransactedChange(provider, component, delegate(object arg) {
				IComponentChangeService service = control.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				if(service != null)
					service.OnComponentChanged(control.Site.Component, descriptor, null, null);
				return true;
			}, null, string.Format("{0} changed", propertyName), descriptor);
		}
		protected static ASPxRibbon GetRibbonByControlID(string controlID, System.Web.UI.Control owner) {
			Dictionary<string, ASPxRibbon> dictionary = RibbonControlIDConverter.GetRibbonControlsDictionary(owner.Site.Component);
			ASPxRibbon ribbon = dictionary[controlID];
			if(ribbon == null)
				ribbon = FindControlHelper.LookupControl(owner, controlID) as ASPxRibbon;
			return ribbon;
		}
		public static void AddTabCollectionToRibbonControl(string controlID, RibbonTab[] ribbonTabs, System.Web.UI.Control owner) {
			AddTabCollectionToRibbonControl(controlID, ribbonTabs, null, owner);
		}
		public static void AddTabCollectionToRibbonControl(string controlID, RibbonTab[] ribbonTabs, RibbonContextTabCategory[] contextTabCategories, System.Web.UI.Control owner) {
			var ribbon = GetRibbonByControlID(controlID, owner);
			if(ribbon == null) {
				DesignUtils.ShowError(owner.Site, string.Format(StringResources.RibbonExceptionText_ControlNotFound, controlID));
				return;
			}
			bool isSuccessfully = TryAddTabsToRibbon(ribbon, ribbonTabs, contextTabCategories, owner);
			if(isSuccessfully) {
				FireRibbonControlPropertyChanged(ribbon.Site, ribbon, "Tabs", ribbon.Site.Component);
				if(contextTabCategories != null)
					FireRibbonControlPropertyChanged(ribbon.Site, ribbon, "ContextTabCategories", ribbon.Site.Component);
			}
		}
		static bool TryAddTabsToRibbon(ASPxRibbon ribbon, RibbonTab[] ribbonTabs, RibbonContextTabCategory[] contextTabCategories, System.Web.UI.Control owner) {
			bool clearExistingTabs = false;
			if(RibbonHasExistingTabs(ribbon, ribbonTabs, contextTabCategories)) {
				var dialogResult = ShowDeleteTabsConfirm(owner, ribbon.ID);
				if(dialogResult == DialogResult.Cancel)
					return false;
				clearExistingTabs = dialogResult == DialogResult.Yes;
			}
			RibbonHelper.AddTabCollectionToControl(ribbon, ribbonTabs, clearExistingTabs);
			if(contextTabCategories != null)
				AddContextTabsToRibbon(ribbon, contextTabCategories, clearExistingTabs);
			return true;
		}
		static bool RibbonHasExistingTabs(ASPxRibbon ribbon, RibbonTab[] ribbonTabs, RibbonContextTabCategory[] contextTabCategories) {
			foreach(RibbonTab tab in ribbonTabs) {
				if(ribbon.Tabs.Find(i => i.GetType() == tab.GetType()) != null)
					return true;
			}
			if(contextTabCategories != null) {
				foreach(RibbonContextTabCategory tabCategory in contextTabCategories) {
					if(ribbon.ContextTabCategories.Find(i => i.GetType() == tabCategory.GetType()) != null)
						return true;
				}
			}
			return false;
		}
		static DialogResult ShowDeleteTabsConfirm(System.Web.UI.Control owner, string ribbonID) {
			return DesignUtils.ShowMessage(owner.Site, string.Format(string.Format("Do you want to delete the existing {0} ribbon tabs??", owner.GetType().Name)),
							string.Format("Create default tabs for '{0}'", ribbonID), MessageBoxButtons.YesNoCancel);
		}
		static void AddContextTabsToRibbon(ASPxRibbon ribbon, RibbonContextTabCategory[] contextTabCategories, bool clearExistingTabs) {
			if(clearExistingTabs)
				ribbon.ContextTabCategories.Clear();
			ribbon.ContextTabCategories.AddRange(contextTabCategories);
		}
	}
	public class RibbonCommonDesigner : CommonFormDesigner {
		public RibbonCommonDesigner(ASPxRibbon ribbon, IServiceProvider provider) 
			: base(new RibbonItemsOwner(ribbon, provider)) {
			ItemsImageIndex = RibbonItemsImageIndex;
		}
		protected override void CreateMainGroupItems() {
			AddRibbonItems();
			AddRibbonContextTabCategoriesItems();
			base.CreateClientSideEventsItem();
		}
		protected void AddRibbonItems() {
			MainGroup.Add(CreateDesignerItem(new RibbonItemsOwner((ASPxRibbon)Control, Provider), typeof(ItemsEditorFrame), RibbonItemsImageIndex));
		}
		protected void AddRibbonContextTabCategoriesItems() {
			MainGroup.Add(CreateDesignerItem(new RibbonItemsOwner((ASPxRibbon)Control, "Context Tabs", Provider, ((ASPxRibbon)Control).ContextTabCategories), typeof(ItemsEditorFrame), RibbonItemsImageIndex));
		}
	}
}

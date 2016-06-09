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
using System.Linq;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Core.Design.SmartTags;
using System.ComponentModel;
using DevExpress.Xpf.Design;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Ribbon.Design {
	internal
	class RegisterMetadata : MetadataProviderBase {
		protected override System.Reflection.Assembly RuntimeAssembly { get { return typeof(RibbonControl).Assembly; } }
		protected override string ToolboxCategoryPath {
			get { return AssemblyInfo.DXTabWpfNavigation; }
		}
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			base.PrepareAttributeTable(builder);
			HideProperties(builder);
			PrepareAttributeTableCore(builder);
			RibbonPropertyLineRegistrator.RegisterPropertyLines();
		}
		protected virtual void PrepareAttributeTableCore(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(RibbonControl), new FeatureAttribute(typeof(HookAdornerProvider)));
			builder.AddCustomAttributes(typeof(RibbonStatusBarControl), new FeatureAttribute(typeof(HookAdornerProvider)));
			builder.AddCustomAttributes(typeof(RibbonControl), new FeatureAttribute(typeof(RibbonControlInitializer)));
			builder.AddCustomAttributes(typeof(RibbonControl), new FeatureAttribute(typeof(RibbonControlContextMenuProvider)));
			builder.AddCustomAttributes(typeof(RibbonControl), new FeatureAttribute(typeof(RibbonControlSelectionAdornerProvider)));
			builder.AddCustomAttributes(typeof(RibbonControl), new FeatureAttribute(typeof(RibbonControlAdornerProvider)));
			builder.AddCustomAttributes(typeof(RibbonControl), new FeatureAttribute(typeof(RibbonControlParentAdapter)));
			builder.AddCustomAttributes(typeof(RibbonControl), "ApplicationMenu", new NewItemTypesAttribute(typeof(ApplicationMenu), typeof(BackstageViewControl)));
			TypeDescriptor.AddAttributes(typeof(RibbonPageGroupControl), new DesignerHitTestInfoAttribute(typeof(RibbonModelItemProvider)));
			TypeDescriptor.AddAttributes(typeof(RibbonPageGroupControl), new DesignerHitTestInfoAttribute(typeof(RibbonModelItemProvider)));
			TypeDescriptor.AddAttributes(typeof(RibbonSelectedPageControl), new DesignerHitTestInfoAttribute(typeof(RibbonModelItemProvider)));
			TypeDescriptor.AddAttributes(typeof(RibbonPageCaptionControl), new DesignerHitTestInfoAttribute(typeof(RibbonModelItemProvider)));
			TypeDescriptor.AddAttributes(typeof(RibbonPageCategoryHeaderControl), new DesignerHitTestInfoAttribute(typeof(RibbonModelItemProvider)));
			TypeDescriptor.AddAttributes(typeof(RibbonPageCategoryBase), new DesignTimeParentAttribute(typeof(RibbonControl), typeof(RibbonViewProvider)));
			TypeDescriptor.AddAttributes(typeof(RibbonPage), new DesignTimeParentAttribute(typeof(RibbonControl), typeof(RibbonViewProvider)));
			TypeDescriptor.AddAttributes(typeof(RibbonPageGroup), new DesignTimeParentAttribute(typeof(RibbonControl), typeof(RibbonViewProvider)));
			TypeDescriptor.AddAttributes(typeof(BarItemLink), new DesignTimeParentAttribute(typeof(RibbonControl), typeof(BarsViewProvider)), new DesignTimeParentAttribute(typeof(RibbonStatusBarControl), typeof(BarsViewProvider)));
			TypeDescriptor.AddAttributes(typeof(BarItem), new DesignTimeParentAttribute(typeof(RibbonControl), typeof(BarsViewProvider)), new DesignTimeParentAttribute(typeof(RibbonStatusBarControl), typeof(BarsViewProvider)));
			builder.AddCustomAttributes(typeof(RibbonControl), new FeatureAttribute(typeof(RibbonControlTaskProvider)));
			builder.AddCustomAttributes(typeof(RibbonControl), new FeatureAttribute(typeof(BarManagerSelectionAdornerProvider)));
			builder.AddCustomAttributes(typeof(RibbonStatusBarControl), new FeatureAttribute(typeof(BarManagerSelectionAdornerProvider)));
			builder.AddCustomAttributes(typeof(RibbonStatusBarControl), new FeatureAttribute(typeof(RibbonStatusBarControlContextMenuProvider)));
			builder.AddCustomAttributes(typeof(RibbonPage), new FeatureAttribute(typeof(RibbonPageInitializer)));
			builder.AddCustomAttributes(typeof(RibbonControl), new FeatureAttribute(typeof(RibbonControlDeleteItemTaskProvider)));
			builder.AddCustomAttributes(typeof(RibbonControl), "ApplicationMenu", new AlternateContentPropertyAttribute());
			builder.AddCustomAttributes(typeof(RibbonPage), new FeatureAttribute(typeof(RibbonPageContextMenuProvider)));
			builder.AddCustomAttributes(typeof(RibbonPage), "Caption", new AlternateContentPropertyAttribute());
			builder.AddCustomAttributes(typeof(RibbonPageCategoryBase), new FeatureAttribute(typeof(RibbonPageCategoryContextMenuProvider)));
			builder.AddCustomAttributes(typeof(RibbonPageGroup), new FeatureAttribute(typeof(RibbonPageGroupInitializer)));
			builder.AddCustomAttributes(typeof(RibbonPageGroup), new FeatureAttribute(typeof(RibbonPageGroupContextMenuProvider)));
			builder.AddCustomAttributes(typeof(RibbonPageGroup), new FeatureAttribute(typeof(RibbonPageGroupParentAdapter)));
			builder.AddCustomAttributes(typeof(RibbonPageGroup), new FeatureAttribute(typeof(RibbonPageGroupDefaultValueProvider)));
			builder.AddCustomAttributes(typeof(RibbonPageGroup), "Caption", new AlternateContentPropertyAttribute());
			builder.AddCustomAttributes(typeof(BarButtonGroup), new FeatureAttribute(typeof(BarButtonGroupContextMenuProvider)));
			builder.AddCustomAttributes(typeof(BarButtonGroupLink), new FeatureAttribute(typeof(BarButtonGroupLinkParentAdapter)));
			builder.AddCustomAttributes(typeof(RibbonGalleryBarItem), new FeatureAttribute(typeof(RibbonGalleryBarItemDefaultInitializer)));
			builder.AddCustomAttributes(typeof(BarItem), new FeatureAttribute(typeof(BarItemContextMenuProvider)));
			builder.AddCustomAttributes(typeof(BackstageViewControl), new FeatureAttribute(typeof(BackstageViewDefaultInitializer)));
			builder.AddCustomAttributes(typeof(BackstageButtonItem), new FeatureAttribute(typeof(BackstageItemInitializer)));
			builder.AddCustomAttributes(typeof(BackstageTabItem), new FeatureAttribute(typeof(BackstageItemInitializer)));
			builder.AddCustomAttributes(typeof(RibbonControl), "Categories", new NewItemTypesAttribute(typeof(RibbonPageCategory)));
			builder.AddCustomAttributes(typeof(RibbonControl), "PageHeaderItemLinks", GetBarItemLinkCollectionTypesAttribute());
			builder.AddCustomAttributes(typeof(RibbonControl), "ToolbarItemLinks", GetBarItemLinkCollectionTypesAttribute());
			builder.AddCustomAttributes(typeof(RibbonStatusBarControl), "LeftItemLinks", GetBarItemLinkCollectionTypesAttribute());
			builder.AddCustomAttributes(typeof(RibbonStatusBarControl), "RightItemLinks", GetBarItemLinkCollectionTypesAttribute());
			builder.AddCustomAttributes(typeof(RibbonDefaultPageCategory), "Pages", new NewItemTypesAttribute(typeof(RibbonPage)));
			builder.AddCustomAttributes(typeof(RibbonPageCategory), "Pages", new NewItemTypesAttribute(typeof(RibbonPage)));
			builder.AddCustomAttributes(typeof(RibbonPageCategory), "Caption", new AlternateContentPropertyAttribute());
			builder.AddCustomAttributes(typeof(RibbonPage), "Groups", new NewItemTypesAttribute(typeof(RibbonPageGroup)));
			builder.AddCustomAttributes(typeof(RibbonPageGroup), "ItemLinks", new NewItemTypesAttribute(
				typeof(BarItemLink), typeof(BarButtonItemLink), typeof(BarSplitButtonItemLink), typeof(BarSubItemLink), typeof(BarSplitCheckItemLink),
				typeof(BarEditItemLink), typeof(BarItemLinkSeparator), typeof(BarStaticItemLink), typeof(RibbonGalleryBarItemLink),
				typeof(BarButtonGroupLink)));
			builder.AddCustomAttributes(typeof(RibbonPageGroup), "Items", new NewItemTypesAttribute(TypeLists.BarItemTypes.ToArray()));
			builder.AddCustomAttributes(typeof(RibbonPageGroup), RibbonPageGroup.SuperTipProperty.Name, new NewItemTypesAttribute(typeof(SuperTip)));
			builder.AddCustomAttributes(typeof(RibbonControl), "PageHeaderItems", new NewItemTypesAttribute(TypeLists.BarItemTypes.Where(t => !string.Equals(t.Name, "BarButtonGroup")).ToArray()));
			builder.AddCustomAttributes(typeof(RibbonControl), "ToolbarItems", new NewItemTypesAttribute(TypeLists.BarItemTypes.Where(t => !string.Equals(t.Name, "BarButtonGroup")).ToArray()));
			builder.AddCustomAttributes(typeof(RibbonStatusBarControl), "LeftItems", new NewItemTypesAttribute(TypeLists.BarItemTypes.Where(t => !string.Equals(t.Name, "BarButtonGroup")).ToArray()));
			builder.AddCustomAttributes(typeof(RibbonStatusBarControl), "RightItems", new NewItemTypesAttribute(TypeLists.BarItemTypes.Where(t => !string.Equals(t.Name, "BarButtonGroup")).ToArray()));
			builder.AddCustomAttributes(typeof(BackstageViewControl), "Items", new NewItemTypesAttribute(
				typeof(BackstageButtonItem), typeof(BackstageTabItem), typeof(BackstageSeparatorItem)));
		}
		NewItemTypesAttribute GetBarItemLinkCollectionTypesAttribute() {
			return new NewItemTypesAttribute(
				typeof(BarItemLink), typeof(BarButtonItemLink), typeof(BarSplitButtonItemLink), typeof(BarSubItemLink), typeof(BarSplitCheckItemLink),
				typeof(BarEditItemLink), typeof(BarItemLinkSeparator), typeof(ToolbarListItemLink), typeof(BarStaticItemLink));
		}
		void HideProperties(AttributeTableBuilder builder) {
		}
	}
	internal
	class RibbonControlInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			using(ModelEditingScope scope = item.BeginEdit("Initializing the RibbonControl")) {
				ModelItem pageCategory = ModelFactory.CreateItem(item.Context, typeof(RibbonDefaultPageCategory), CreateOptions.InitializeDefaults);
				pageCategory.Properties["Caption"].SetValue("defaultCategory");
				pageCategory.ResetLayout();
				item.Properties["Categories"].Collection.Add(pageCategory);
				ModelItem ribbonPage = ModelFactory.CreateItem(item.Context, typeof(RibbonPage), CreateOptions.InitializeDefaults);
				ribbonPage.Properties["Caption"].SetValue("Home");
				ribbonPage.ResetLayout();
				pageCategory.Properties["Pages"].Collection.Add(ribbonPage);
				ModelItem ribbonPageGroup = ModelFactory.CreateItem(ribbonPage.Context, typeof(RibbonPageGroup), CreateOptions.InitializeDefaults);
				ribbonPageGroup.Properties["Caption"].SetValue("Tools");
				ribbonPageGroup.ResetLayout();
				ribbonPage.Properties["Groups"].Collection.Add(ribbonPageGroup);
				item.ResetLayout();
				item.Properties["Width"].SetValue(400d);
				item.Properties["Height"].SetValue(100d);
				InitializerHelper.Initialize(item);
				scope.Complete();
			}
		}
	}
	class RibbonPageInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			string name = string.IsNullOrEmpty(item.Name) ? "Ribbon Page" : item.Name.Replace("ribbonPage", "Ribbon Page ");
			item.Properties["Caption"].SetValue(name);
		}
	}
	class RibbonPageGroupInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			string name = string.IsNullOrEmpty(item.Name) ? "Ribbon Page Group" : item.Name.Replace("ribbonPageGroup", "Ribbon Page Group ");
			item.Properties["Caption"].SetValue(name);
		}
	}
	class RibbonPageGroupDefaultValueProvider : DesignModeValueProvider {
		public RibbonPageGroupDefaultValueProvider() {
			Properties.Add(new PropertyIdentifier(typeof(RibbonPageGroup), "AllowCollapse"));
		}
		public override object TranslatePropertyValue(ModelItem item, PropertyIdentifier identifier, object value) {
			if(identifier.DeclaringType == typeof(RibbonPageGroup) && identifier.Name == "AllowCollapse")
				return false;
			return base.TranslatePropertyValue(item, identifier, value);
		}
	}
	class RibbonGalleryBarItemDefaultInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item, EditingContext context) {
			base.InitializeDefaults(item, context);
			using(var scope = item.BeginEdit("Initialize "+ typeof(RibbonGalleryBarItem).Name)) {
				ModelItem gallery = ModelFactory.CreateItem(context, typeof(Gallery), CreateOptions.InitializeDefaults);
				gallery.Properties["ColCount"].SetValue(4);
				item.Properties["Gallery"].SetValue(gallery);
				scope.Complete();
			}
		}
	}
}

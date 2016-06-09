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

extern alias Platform;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Xpf.Design;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.Editors.DateNavigator;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::System.Windows;
using Platform::System.Windows.Controls;
using DevExpress.Xpf.Core.Design.SmartTags;
#if SILVERLIGHT
using System.Collections;
using System.Globalization;
using DevExpress.Utils.About;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.Core.ServerMode;
using PLinqInstantFeedbackDataSource = Platform::DevExpress.Xpf.Core.ServerMode.LinqToObjectsInstantFeedbackDataSource;
using ToolboxTabNameAttribute = Platform::DevExpress.Utils.ToolboxTabNameAttribute;
using AssemblyInfo = Platform::AssemblyInfo;
using Platform::System.Windows.Media;
using System.IO;
using DevExpress.Utils.Design;
using EnvDTE;
#else
using DevExpress.Xpf.Core.Native;
using System.Windows;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Media;
using DevExpress.Xpf.Editors.RangeControl;
using DependencyObject = System.Windows.DependencyObject;
using DependencyPropertyChangedEventArgs = System.Windows.DependencyPropertyChangedEventArgs;
using FrameworkContentElement = System.Windows.FrameworkContentElement;
using FrameworkElement = System.Windows.FrameworkElement;
using FrameworkPropertyMetadata = System.Windows.FrameworkPropertyMetadata;
using Rect = System.Windows.Rect;
using Window = System.Windows.Window;
using ToolboxTabNameAttribute = DevExpress.Utils.ToolboxTabNameAttribute;
using DevExpress.Xpf.Core.Design.Services;
using DevExpress.Mvvm.UI;
using System.Collections;
#endif
namespace DevExpress.Xpf.Core.Design {
	public abstract class MetadataProviderBase : IProvideAttributeTable {
		public virtual AttributeTable AttributeTable {
			get {
				var builder = new AttributeTableBuilder();
				MetadataProvider.ForceRegister(builder);
				BuildAttributes(builder);
				return builder.CreateTable();
			}
		}
		public void BuildAttributes(AttributeTableBuilder builder) {
			PrepareAttributeTable(builder);
			if (RuntimeAssembly != null)
				RegisterToolboxBrowsableControls(builder, RuntimeAssembly, ToolboxCategoryPath);
		}
		protected virtual void PrepareAttributeTable(AttributeTableBuilder builder) {
		}
		bool IsDXToolboxBrowsableAttribute(CustomAttributeData toolboxItemAttr) {
			if (toolboxItemAttr == null || toolboxItemAttr.Constructor == null || toolboxItemAttr.Constructor.DeclaringType == null) return false;
			return toolboxItemAttr.Constructor.DeclaringType == typeof(DXToolboxBrowsableAttribute);
		}
		string GetTabName(IList<CustomAttributeData> attributes) {
			foreach (CustomAttributeData attribute in attributes)
				if (attribute.Constructor.DeclaringType == typeof(ToolboxTabNameAttribute))
					return (string)attribute.ConstructorArguments[0].Value;
			return null;
		}
		private void RegisterToolboxBrowsableControls(AttributeTableBuilder builder, Assembly assembly, string categoryPath) {
			foreach (Type type in assembly.GetExportedTypes()) {
				if (type.IsSubclassOf(typeof(FrameworkElement))
#if !SL
					|| type.IsSubclassOf(typeof(FrameworkContentElement))
#endif
					) {
					IList<CustomAttributeData> attributes = CustomAttributeData.GetCustomAttributes(type);
					CustomAttributeData dxToolboxBrowsableAttribute = FindDXToolboxBrowsableAttribute(attributes);
					if (IsVisible(dxToolboxBrowsableAttribute)) {
						string tabName = null;
						tabName = GetTabName(attributes);
						if (tabName == null)
							tabName = categoryPath;
						if(tabName.StartsWith(ToolboxTabPrefix))
							tabName = tabName.Remove(0, ToolboxTabPrefix.Length);
						builder.RegisterControls(tabName, type);
					}
					else {
						builder.HideControls(type);
					}
				}
			}
		}
		protected virtual string ToolboxTabPrefix { get { return "DX." + AssemblyInfo.VersionShort + ": "; } }
		bool IsVisible(CustomAttributeData attributeData) {
			if (!IsDXToolboxBrowsableAttribute(attributeData))
				return false;
			bool? hideItem = null;
			if (attributeData.ConstructorArguments.Count == 0) {
				hideItem = false;
			}
			else {
				object kind = attributeData.ConstructorArguments[0].Value;
				if (kind is bool) {
					hideItem = !(bool)attributeData.ConstructorArguments[0].Value;
				}
				else {
					hideItem = (int)(kind) == 2;
				}
			}
			return (hideItem != null && !hideItem.Value);
		}
		CustomAttributeData FindDXToolboxBrowsableAttribute(IList<CustomAttributeData> attributes) {
			foreach (CustomAttributeData attr in attributes) {
				if (IsDXToolboxBrowsableAttribute(attr))
					return attr;
			}
			return null;
		}
		protected virtual Assembly RuntimeAssembly { get { return null; } }
		protected virtual string ToolboxCategoryPath { get { return ""; } }
	}
	public class MetadataProviderConsts {
		public static readonly string[] ScrollableControlCoreProperties =
			new string[] { "AnimateScrolling", "HorizontalOffset", "VerticalOffset", "ScrollBars" };
		public static readonly string[] ScrollableControlStyleProperties =
			new string[] { "HorizontalScrollBarStyle", "VerticalScrollBarStyle", "CornerBoxStyle" };
		public static readonly string[] ScrollableControlProperties;
		static MetadataProviderConsts() {
			ScrollableControlProperties = new string[ScrollableControlCoreProperties.Length + ScrollableControlStyleProperties.Length];
			ScrollableControlCoreProperties.CopyTo(ScrollableControlProperties, 0);
			ScrollableControlStyleProperties.CopyTo(ScrollableControlProperties, ScrollableControlCoreProperties.Length);
		}
	}
	public class MetadataProvider : MetadataProviderBase {
		static bool CoreMetadataRegistered;
		static MetadataProvider() {
			Platform::DevExpress.Xpf.Core.Native.UIElementExtensions.InvisibleBounds = new Rect(0, 0, 0, 0);
		}
		public static void ForceRegister(AttributeTableBuilder builder) {
#if !SL
			new MetadataProvider().BuildAttributes(builder);
#endif
		}
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			if (CoreMetadataRegistered == true)
				return;
			CoreMetadataRegistered = true;
			base.PrepareAttributeTable(builder);
			HideProperties(builder);
			AddTypeConverters(builder);
			PutPropertiesInCategories(builder);
			PositionProperties(builder);
			RegisterNewItemTypes(builder);
			RegisterFeatures(builder);
#if !SILVERLIGHT
			builder.AddCustomAttributes(typeof(FrameworkElement), new FeatureAttribute(typeof(DesignTimeThemeSelectorAdornerProvider)));
#endif
			builder.AddCustomAttributes(typeof(FrameworkElement), new FeatureAttribute(typeof(FrameworkElementSmartTagAdorner)));
			RegisterConditionalFormatting(builder);
		}
		protected override Assembly RuntimeAssembly { get { return typeof(BaseEdit).Assembly; } }
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabWpfCommon; } }
		void HideProperties(AttributeTableBuilder builder) {
			builder.ShowPropertiesAsAdvanced(typeof(ScrollControl), MetadataProviderConsts.ScrollableControlCoreProperties);
#if SILVERLIGHT
			builder.HideProperties(typeof(BarItem), "Visibility");
			builder.HideProperties(typeof(BarItemLinkBase), "Visibility");
#else
			builder.HideProperties(typeof(BarItem), "ToolTip");
#endif
		}
		void PutPropertiesInCategories(AttributeTableBuilder builder) {
			builder.PutPropertiesInCategory(Categories.Layout, typeof(ScrollControl),
				"HorizontalOffset", "ScrollBars", "VerticalOffset");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(ScrollControl),
				"AnimateScrolling", "DragScrolling");
		}
		void AddTypeConverters(AttributeTableBuilder builder) {
			builder.AddStringConverter(typeof(DXWindow), "Title");
			builder.AddCustomAttributes(typeof(BarItemLink), "BarItemName", new TypeConverterAttribute(typeof(BarItemNameTypeConverter)));
		}
		void PositionProperties(AttributeTableBuilder builder) {
			builder.PositionProperties(PropertyOrder.Early, typeof(ScrollControl), "ScrollBars");
			builder.PositionProperties(PropertyOrder.Late, typeof(ScrollControl), "HorizontalOffset", "VerticalOffset");
			var order = PropertyOrder.CreateAfter(PropertyOrder.Default);
			builder.PositionProperties(order, typeof(ScrollControl), "HorizontalScrollBarStyle", "VerticalScrollBarStyle");
			builder.PositionProperties(PropertyOrder.CreateAfter(order), typeof(ScrollControl), "CornerBoxStyle");
		}
		void BarsRegisterNewItemTypes(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(BarManager), "Bars", new NewItemTypesAttribute(typeof(Bar)));
			builder.AddCustomAttributes(typeof(BarManager), "Categories", new NewItemTypesAttribute(typeof(BarManagerCategory)));
			builder.AddCustomAttributes(typeof(BarManager), "Items", new NewItemTypesAttribute(TypeLists.BarItemTypes.Where(t => !string.Equals(t.Name, "BarButtonGroup")).ToArray()));
			builder.AddCustomAttributes(typeof(Bar), "ItemLinks", new NewItemTypesAttribute(typeof(BarButtonItemLink)));
			builder.AddCustomAttributes(typeof(Bar), "ItemLinks", new NewItemTypesAttribute(typeof(BarCheckItemLink)));
			builder.AddCustomAttributes(typeof(Bar), "ItemLinks", new NewItemTypesAttribute(typeof(BarEditItemLink)));
			builder.AddCustomAttributes(typeof(Bar), "ItemLinks", new NewItemTypesAttribute(typeof(BarSplitButtonItemLink)));
			builder.AddCustomAttributes(typeof(Bar), "ItemLinks", new NewItemTypesAttribute(typeof(BarSplitCheckItemLink)));
			builder.AddCustomAttributes(typeof(Bar), "ItemLinks", new NewItemTypesAttribute(typeof(BarStaticItemLink)));
			builder.AddCustomAttributes(typeof(Bar), "ItemLinks", new NewItemTypesAttribute(typeof(BarSubItemLink)));
			builder.AddCustomAttributes(typeof(Bar), "ItemLinks", new NewItemTypesAttribute(typeof(BarItemLinkSeparator)));
			builder.AddCustomAttributes(typeof(Bar), "ItemLinks", new NewItemTypesAttribute(typeof(BarLinkContainerItemLink)));
			builder.AddCustomAttributes(typeof(Bar), "ItemLinks", new NewItemTypesAttribute(typeof(ToolbarListItemLink)));
			builder.AddCustomAttributes(typeof(BarItem), BarItem.SuperTipProperty.Name, new NewItemTypesAttribute(typeof(SuperTip)));
			builder.AddCustomAttributes(typeof(SuperTip), "Items", new NewItemTypesAttribute(typeof(SuperTipHeaderItem), typeof(SuperTipItem), typeof(SuperTipItemSeparator)));
			builder.AddCustomAttributes(typeof(BarSubItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarButtonItemLink)));
			builder.AddCustomAttributes(typeof(BarSubItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarCheckItemLink)));
			builder.AddCustomAttributes(typeof(BarSubItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarEditItemLink)));
			builder.AddCustomAttributes(typeof(BarSubItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarSplitButtonItemLink)));
			builder.AddCustomAttributes(typeof(BarSubItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarSplitCheckItemLink)));
			builder.AddCustomAttributes(typeof(BarSubItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarStaticItemLink)));
			builder.AddCustomAttributes(typeof(BarSubItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarSubItemLink)));
			builder.AddCustomAttributes(typeof(BarSubItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarItemLinkSeparator)));
			builder.AddCustomAttributes(typeof(BarSubItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarLinkContainerItemLink)));
			builder.AddCustomAttributes(typeof(BarSubItem), "ItemLinks", new NewItemTypesAttribute(typeof(ToolbarListItemLink)));
			builder.AddCustomAttributes(typeof(BarSplitButtonItem), "PopupControl", new NewItemTypesAttribute(typeof(PopupMenu)));
			builder.AddCustomAttributes(typeof(BarSplitButtonItem), "PopupControl", new NewItemTypesAttribute(typeof(PopupControlContainer)));
			builder.AddCustomAttributes(typeof(BarSplitButtonItem), "PopupControl", new NewItemTypesAttribute(typeof(PopupControlContainerInfo)));
			builder.AddCustomAttributes(typeof(BarEditItem), "EditSettings", new NewItemTypesAttribute(TypeLists.EditSettingsTypes.Where(t => (t.Item2 & PropertyTarget.Bar) != 0).Select(t => t.Item1).ToArray()));
			builder.AddCustomAttributes(typeof(BarLinkContainerItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarButtonItemLink)));
			builder.AddCustomAttributes(typeof(BarLinkContainerItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarCheckItemLink)));
			builder.AddCustomAttributes(typeof(BarLinkContainerItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarEditItemLink)));
			builder.AddCustomAttributes(typeof(BarLinkContainerItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarSplitButtonItemLink)));
			builder.AddCustomAttributes(typeof(BarLinkContainerItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarStaticItemLink)));
			builder.AddCustomAttributes(typeof(BarLinkContainerItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarLinkContainerItemLink)));
			builder.AddCustomAttributes(typeof(BarLinkContainerItem), "ItemLinks", new NewItemTypesAttribute(typeof(BarLinkContainerItemLink)));
			builder.AddCustomAttributes(typeof(BarLinkContainerItem), "ItemLinks", new NewItemTypesAttribute(typeof(ToolbarListItemLink)));
			builder.AddCustomAttributes(typeof(PopupMenu), "ItemLinks", new NewItemTypesAttribute(typeof(BarButtonItemLink)));
			builder.AddCustomAttributes(typeof(PopupMenu), "ItemLinks", new NewItemTypesAttribute(typeof(BarCheckItemLink)));
			builder.AddCustomAttributes(typeof(PopupMenu), "ItemLinks", new NewItemTypesAttribute(typeof(BarEditItemLink)));
			builder.AddCustomAttributes(typeof(PopupMenu), "ItemLinks", new NewItemTypesAttribute(typeof(BarSplitButtonItemLink)));
			builder.AddCustomAttributes(typeof(PopupMenu), "ItemLinks", new NewItemTypesAttribute(typeof(BarStaticItemLink)));
			builder.AddCustomAttributes(typeof(PopupMenu), "ItemLinks", new NewItemTypesAttribute(typeof(BarSubItemLink)));
			builder.AddCustomAttributes(typeof(PopupMenu), "ItemLinks", new NewItemTypesAttribute(typeof(BarLinkContainerItemLink)));
			builder.AddCustomAttributes(typeof(PopupMenu), "ItemLinks", new NewItemTypesAttribute(typeof(ToolbarListItemLink)));
		}
		void RegisterNewItemTypes(AttributeTableBuilder builder) {
			BarsRegisterNewItemTypes(builder);
			builder.AddCustomAttributes(typeof(DXTabControl), "Items", new NewItemTypesAttribute(typeof(DXTabItem)));
			builder.AddCustomAttributes(typeof(DXTabControl), "View", new NewItemTypesAttribute(typeof(TabControlMultiLineView), typeof(TabControlScrollView), typeof(TabControlStretchView)));
			builder.AddCustomAttributes(typeof(ComboBoxEdit), "Items", new NewItemTypesAttribute(typeof(ComboBoxEditItem)));
			builder.AddCustomAttributes(typeof(ListBoxEdit), "Items", new NewItemTypesAttribute(typeof(ListBoxEditItem)));
			builder.AddCustomAttributes(typeof(ButtonEdit), "Buttons", new NewItemTypesAttribute(typeof(ButtonInfo), typeof(SpinButtonInfo)));
			builder.AddCustomAttributes(typeof(ButtonEditSettings), "Buttons", new NewItemTypesAttribute(typeof(ButtonInfo), typeof(SpinButtonInfo)));
			builder.AddCustomAttributes(typeof(DateNavigator), DesignHelper.GetPropertyName(BaseEdit.StyleSettingsProperty), new NewItemTypesAttribute(typeof(DateNavigatorStyleSettings)));
			builder.AddCustomAttributes(typeof(ComboBoxEdit), DesignHelper.GetPropertyName(BaseEdit.StyleSettingsProperty), new NewItemTypesAttribute(typeof(ComboBoxStyleSettings), typeof(CheckedComboBoxStyleSettings), typeof(RadioComboBoxStyleSettings)));
			builder.AddCustomAttributes(typeof(ListBoxEdit), DesignHelper.GetPropertyName(BaseEdit.StyleSettingsProperty), new NewItemTypesAttribute(typeof(ListBoxEditStyleSettings), typeof(CheckedListBoxEditStyleSettings), typeof(RadioListBoxEditStyleSettings)));
			builder.AddCustomAttributes(typeof(TrackBarEdit), DesignHelper.GetPropertyName(BaseEdit.StyleSettingsProperty), new NewItemTypesAttribute(typeof(TrackBarStyleSettings), typeof(TrackBarZoomStyleSettings), typeof(TrackBarRangeStyleSettings), typeof(TrackBarZoomRangeStyleSettings)));
			builder.AddCustomAttributes(typeof(ProgressBarEdit), DesignHelper.GetPropertyName(BaseEdit.StyleSettingsProperty), new NewItemTypesAttribute(typeof(ProgressBarStyleSettings), typeof(ProgressBarMarqueeStyleSettings)));
			builder.AddCustomAttributes(typeof(Gallery), "Groups", new NewItemTypesAttribute(typeof(GalleryItemGroup)));
			builder.AddCustomAttributes(typeof(GalleryItemGroup), "Items", new NewItemTypesAttribute(typeof(GalleryItem)));
#if !SL
			builder.AddCustomAttributes(typeof(RangeControl), "Client", new NewItemTypesAttribute(typeof(IRangeControlClient)));
#endif
		}
		void RegisterFeatures(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(BaseEdit), new FeatureAttribute(typeof(EditorInitializer)));
			builder.AddCustomAttributes(typeof(ListBoxEdit), new FeatureAttribute(typeof(ListBoxEditInitializer)));
			builder.AddCustomAttributes(typeof(ColorEdit), new FeatureAttribute(typeof(ColorEditInitializer)));
#if !SL
			builder.AddCustomAttributes(typeof(FrameworkElement), new FeatureAttribute(typeof(DXContextMenuProvider)));
			builder.AddCustomAttributes(typeof(FrameworkContentElement), new FeatureAttribute(typeof(DXContextMenuProvider)));
			builder.AddCustomAttributes(typeof(RangeControl), new FeatureAttribute(typeof(RangeControlInitializer)));
			builder.AddCustomAttributes(typeof(SparklineEdit), new FeatureAttribute(typeof(SparklineEditInitializer)));
			builder.AddCustomAttributes(typeof(BarCodeEdit), new FeatureAttribute(typeof(BarCodeControlInitializer)));
			builder.AddCustomAttributes(typeof(BarCodeEdit), BarCodeEdit.StyleSettingsProperty.Name, new NewItemTypesAttribute(BarCodeStyleSettingsStorage.GetSymbologyTypes().ToArray()));
#endif
			builder.AddCustomAttributes(typeof(BarManager), new FeatureAttribute(typeof(HookAdornerProvider)));
			builder.AddCustomAttributes(typeof(BarContainerControl), new FeatureAttribute(typeof(HookAdornerProvider)));
			builder.AddCustomAttributes(typeof(ToolBarControlBase), new FeatureAttribute(typeof(HookAdornerProvider)));
			builder.AddCustomAttributes(typeof(BarManager), new FeatureAttribute(typeof(BarManagerInitializer)));
			builder.AddCustomAttributes(typeof(BarManager), new FeatureAttribute(typeof(BarManagerContextMenuProvider)));
			builder.AddCustomAttributes(typeof(BarManager), new FeatureAttribute(typeof(BarManagerAdornerProvider)));
			builder.AddCustomAttributes(typeof(BarManager), new FeatureAttribute(typeof(BarManagerParentAdapter)));
			builder.AddCustomAttributes(typeof(BarManager), new FeatureAttribute(typeof(BarManagerSelectionAdornerProvider)));
			builder.AddCustomAttributes(typeof(ToolBarControlBase), new FeatureAttribute(typeof(BarManagerSelectionAdornerProvider)));
			TypeDescriptor.AddAttributes(typeof(BarControl), new DesignerHitTestInfoAttribute(typeof(BarsModelItemProvider)));
			TypeDescriptor.AddAttributes(typeof(BarItemLinkInfo), new DesignerHitTestInfoAttribute(typeof(BarsModelItemProvider)));
			TypeDescriptor.AddAttributes(typeof(BarItem),
				new DesignTimeParentAttribute(typeof(BarManager), typeof(BarsViewProvider)),
				new DesignTimeParentAttribute(typeof(ToolBarControlBase), typeof(BarsViewProvider)));
			TypeDescriptor.AddAttributes(typeof(BarItemLinkBase), new DesignTimeParentAttribute(typeof(BarManager), typeof(BarsViewProvider)));
			TypeDescriptor.AddAttributes(typeof(Bar), new DesignTimeParentAttribute(typeof(BarManager), typeof(BarsViewProvider)));
			builder.AddCustomAttributes(typeof(BarManager), "Bars", new AlternateContentPropertyAttribute());
			builder.AddCustomAttributes(typeof(BarManager), "Items", new AlternateContentPropertyAttribute());
			builder.AddCustomAttributes(typeof(BarContainerControl), new FeatureAttribute(typeof(BarManagerAdornerProvider)));
			builder.AddCustomAttributes(typeof(Bar), new FeatureAttribute(typeof(BarContextMenuProvider)));
			builder.AddCustomAttributes(typeof(Bar), new FeatureAttribute(typeof(BarParentAdapter)));
			builder.AddCustomAttributes(typeof(Bar), "DockInfo", new AlternateContentPropertyAttribute());
			builder.AddCustomAttributes(typeof(PopupMenu), "ItemLinks", new AlternateContentPropertyAttribute());
			builder.AddCustomAttributes(typeof(BarItemLink), new FeatureAttribute(typeof(BarItemLinkContextMenuProvider)));
			builder.AddCustomAttributes(typeof(BarItemLink), new FeatureAttribute(typeof(BarItemLinkDesignModeValue)));
			RegisterBarItemsInitializers(builder);
			builder.AddCustomAttributes(typeof(DXTabControl), new FeatureAttribute(typeof(DXTabControlInitializer)));
			builder.AddCustomAttributes(typeof(DXTabControl), new FeatureAttribute(typeof(DXTabControlContextMenuProvider)));
			builder.AddCustomAttributes(typeof(DXTabControl), new FeatureAttribute(typeof(DXTabControlParentAdapter)));
			builder.AddCustomAttributes(typeof(DXTabControl), new ComplexBindingPropertiesAttribute("", ""),				
				new LookupBindingPropertiesAttribute("", "", "", ""));
			builder.AddCustomAttributes(typeof(DXTabItem), new DefaultPropertyAttribute("Header"));
			builder.AddCustomAttributes(typeof(DXTabItem), new FeatureAttribute(typeof(DXTabItemInitializer)));			 
			builder.AddCustomAttributes(typeof(DXTabItem), new FeatureAttribute(typeof(DXTabControlContextMenuProvider)));  
			builder.AddCustomAttributes(typeof(DXTabItem), new FeatureAttribute(typeof(DXTabItemParentAdapter)));		   
			builder.AddCustomAttributes(typeof(DXTabItem), new FeatureAttribute(typeof(DXTabItemAdornerProvider)));		 
#if SILVERLIGHT
#if !DEBUG
#endif
#endif
			SmartTagPropertyLineRegistrator.RegisterPropertyLines();
		}
		void RegisterBarItemsInitializers(AttributeTableBuilder builder) {
#if !SL
			builder.AddCustomAttributes(typeof(BarItem), new FeatureAttribute(typeof(BarItemContextMenuProvider)));
			builder.AddCustomAttributes(typeof(FrameworkElement), new FeatureAttribute(typeof(FrameworkElementInitializer)));
			builder.AddCustomAttributes(typeof(FrameworkContentElement), new FeatureAttribute(typeof(FrameworkElementInitializer)));
#endif
			builder.AddCustomAttributes(typeof(BarButtonItem), new FeatureAttribute(typeof(BarItemInitializer)));
			builder.AddCustomAttributes(typeof(BarCheckItem), new FeatureAttribute(typeof(BarItemInitializer)));
			builder.AddCustomAttributes(typeof(BarSplitButtonItem), new FeatureAttribute(typeof(BarItemInitializer)));
			builder.AddCustomAttributes(typeof(BarSplitCheckItem), new FeatureAttribute(typeof(BarItemInitializer)));
			builder.AddCustomAttributes(typeof(BarEditItem), new FeatureAttribute(typeof(BarItemInitializer)));
			builder.AddCustomAttributes(typeof(BarStaticItem), new FeatureAttribute(typeof(BarItemInitializer)));
			builder.AddCustomAttributes(typeof(BarSubItem), new FeatureAttribute(typeof(BarItemInitializer)));
		}
		void RegisterConditionalFormatting(AttributeTableBuilder builder) {
			DevExpress.Xpf.Core.Design.ConditionalFormatting.RegistrationHelper.Register(builder);
		}
	}
#if !SL
	class BarCodeControlInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			ModelItem newItem = ModelFactory.CreateItem(item.Context, typeof(Code128StyleSettings), CreateOptions.None, null);
			item.Properties["Symbology"].SetValue(newItem);
			InitializerHelper.Initialize(item);
		}
	}
	public class FrameworkElementInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
		}
	}
	class RangeControlInitializer : DefaultInitializer {
		readonly double defaultWidth = 500d;
		readonly double defaultheight = 150d;
		public override void InitializeDefaults(ModelItem item) {
			item.Properties["Width"].SetValue(defaultWidth);
			item.Properties["Height"].SetValue(defaultheight);
			InitializerHelper.Initialize(item);
		}
	}
#endif
	class EditorInitializer : DefaultInitializer {
		readonly double defaultWidth = 150d;
		public override void InitializeDefaults(ModelItem item) {
			if (item.Parent == null || item.Parent.ItemType.FullName != "DevExpress.Xpf.LayoutControl.LayoutItem")
				item.Properties["Width"].SetValue(defaultWidth);
			InitializerHelper.Initialize(item);
		}
	}
#if !SL
	class SparklineEditInitializer : EditorInitializer {
		readonly double defaultHeight = 100d;
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			item.Properties["Height"].SetValue(defaultHeight);
		}
	}
#endif
	class ListBoxEditInitializer : EditorInitializer {
		readonly double defaultHeight = 150d;
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			item.Properties["Height"].SetValue(defaultHeight);
		}
	}
	class ColorEditInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			item.Properties["Height"].SetValue("Auto");
			item.Properties["Width"].SetValue("Auto");
			InitializerHelper.Initialize(item);
		}
	}
	class DXTabControlInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item, EditingContext context) {
			base.InitializeDefaults(item, context);
			using (var scope = item.BeginEdit("Initialize DXTabControl")) {
				item.Properties["HorizontalAlignment"].ClearValue();
				item.Properties["VerticalAlignment"].ClearValue();
				ModelItem newTabItem = ModelFactory.CreateItem(context, typeof(DXTabItem), CreateOptions.InitializeDefaults);
				item.Content.Collection.Add(newTabItem);
				scope.Complete();
			}
			InitializerHelper.Initialize(item);
		}
	}
	class DXTabItemInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item, EditingContext context) {
			base.InitializeDefaults(item, context);
			using (ModelEditingScope scope = item.BeginEdit("Initialize DXTabItem")) {
				string header = !string.IsNullOrEmpty(item.Name) ? item.Name : "dxTabItem";
				header = header.Replace("dxTabItem", "Tab");
				item.Properties["Header"].SetValue(header);
				ModelItem grid = ModelFactory.CreateItem(context, typeof(Grid), CreateOptions.None);
				grid.Properties["Background"].SetValue(new SolidColorBrush(Colors.Transparent));
				item.Content.SetValue(grid);
				item.Properties["Height"].ClearValue();
				item.Properties["Width"].ClearValue();
				item.Properties["HorizontalAlignment"].ClearValue();
				item.Properties["VerticalAlignment"].ClearValue();
				if (item.View != null) {
					item.View.UpdateLayout();
				}
				scope.Complete();
			}
		}
	}
	class BarItemNameTypeConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context != null;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			ICollection barItemNames = new List<string>();
			var service = context.GetService(typeof(ModelService)) as ModelService;
			if(service != null && service.Root != null && service.Root.Context != null) {
				var selection = service.Root.Context.Items.GetValue<Microsoft.Windows.Design.Interaction.Selection>();
				if(selection != null)
					barItemNames = GetBarItemNames(BarManagerDesignTimeHelper.FindBarManagerInParent(selection.PrimarySelection), selection.PrimarySelection);
			}
			return new StandardValuesCollection(barItemNames);
		}
		ICollection GetBarItemNames(ModelItem barManager, ModelItem link) {
			if(barManager == null)
				return new List<string>();
			if(link.IsItemOfType(typeof(BarItemLink)) && link.ItemType != typeof(BarItemLink)) {
				return barManager.Properties["Items"].Collection
					.Where(barItem => BarItemLinkCreator.Default.GetItemType(barItem.ItemType) == link.ItemType)
					.Select(barItem => barItem.Name).ToList();
			}
			return barManager.Properties["Items"].Collection.Select(barItem => barItem.Name).ToList();
		}
	}
}

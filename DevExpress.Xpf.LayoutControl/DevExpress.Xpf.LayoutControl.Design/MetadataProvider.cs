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

#if SL
extern alias Platform;
#endif
using System.ComponentModel;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using System.Reflection;
namespace DevExpress.Xpf.LayoutControl.Design {
	class MetadataProvider : MetadataProviderBase {
		protected override Assembly RuntimeAssembly { get { return typeof(LayoutControl).Assembly; } }
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabNameLayoutControl; } }
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {	  
			base.PrepareAttributeTable(builder);
#if !SILVERLIGHT
			DataLayoutControlPropertyLinesRegistrator.RegisterPropertyLines();
#endif
			HideProperties(builder);
			RegisterAttachedProperties(builder);
			AddTypeConverters(builder);
			PutPropertiesInCategories(builder);
			PositionProperties(builder);
			RegisterFeatures(builder);
		}
		void HideProperties(AttributeTableBuilder builder) {
			builder.HideProperties(typeof(DockLayoutControl), DevExpress.Xpf.Core.Design.MetadataProviderConsts.ScrollableControlProperties);
			builder.ShowPropertiesAsAdvanced(typeof(DockLayoutControl), "GetUseDesiredWidthAsMaxWidth", "GetUseDesiredHeightAsMaxHeight");
			builder.HideProperties(typeof(FlowLayoutControl), "LayerWidth", "MaximizedElementOriginalSize");
			builder.HideProperties(typeof(LayoutGroup), DevExpress.Xpf.Core.Design.MetadataProviderConsts.ScrollableControlProperties);
			builder.ShowProperties(typeof(LayoutControl), DevExpress.Xpf.Core.Design.MetadataProviderConsts.ScrollableControlStyleProperties);
			builder.ShowPropertiesAsAdvanced(typeof(LayoutControl), DevExpress.Xpf.Core.Design.MetadataProviderConsts.ScrollableControlCoreProperties);
			builder.ShowPropertiesAsAdvanced(typeof(LayoutControl),
				"AllowItemMovingDuringCustomization", "AllowItemSizingDuringCustomization", "IsCustomization", "LayoutUri",
				"GetUseDesiredWidthAsMaxWidth", "GetUseDesiredHeightAsMaxHeight");
			builder.HideProperties(typeof(LayoutControl),
				"Header", "HeaderTemplate", "IsCollapsed", "IsCollapsible", "ItemLabelsAlignment", "SelectedTabIndex", "View",
				"Collapsed", "Expanded", "SelectedTabChildChanged");
			builder.ShowPropertiesAsAdvanced(typeof(LayoutItem), "ElementSpace");
			builder.HideProperties(typeof(ScrollBox), "ItemSpace");
			builder.HideProperties(typeof(TileLayoutControl), "AllowLayerSizing", "LayerSeparatorStyle", "LayerSizingCoverBrush",
				"ShowLayerSeparators", "StretchContent");
			builder.HideProperties(typeof(DataLayoutItem), "Content", "Label");
		}
		void RegisterAttachedProperties(AttributeTableBuilder builder) {
			builder.RegisterAttachedPropertiesForChildren(typeof(DockLayoutControl), false,
				"GetAllowHorizontalSizing", "GetAllowVerticalSizing", "GetDock", "GetUseDesiredWidthAsMaxWidth", "GetUseDesiredHeightAsMaxHeight");
			builder.RegisterAttachedPropertiesForChildren(typeof(FlowLayoutControl), false, "GetIsFlowBreak");
			builder.RegisterAttachedPropertiesForChildren(typeof(LayoutControl), true,
				"GetAllowHorizontalSizing", "GetAllowVerticalSizing", "GetCustomizationLabel",
				"GetUseDesiredWidthAsMaxWidth", "GetUseDesiredHeightAsMaxHeight",
				"GetTabHeader", "GetTabHeaderTemplate");
			builder.RegisterAttachedPropertiesForChildren(typeof(ScrollBox), false, "GetLeft", "GetTop");
			builder.RegisterAttachedPropertiesForChildren(typeof(TileLayoutControl), false, "GetGroupHeader");
		}
		void AddTypeConverters(AttributeTableBuilder builder) {
			builder.AddStringConverter(typeof(GroupBox), "Header");
			builder.AddStringConverter(typeof(LayoutControl), "GetCustomizationLabel");
			builder.AddStringConverter(typeof(LayoutControl), "GetTabHeader");
			builder.AddStringConverter(typeof(LayoutGroup), "Header");
			builder.AddStringConverter(typeof(LayoutItem), "Label");
			builder.AddStringConverter(typeof(TileLayoutControl), "GetGroupHeader");
			builder.AddStringConverter(typeof(Tile), "Content", "Header");
		}
		void PutPropertiesInCategories(AttributeTableBuilder builder) {
			builder.PutPropertiesInCategory(Categories.Layout, typeof(LayoutControlBase),
				"ItemSpace", "Padding");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(DockLayoutControl),
				"AllowItemSizing", "GetAllowHorizontalSizing", "GetAllowVerticalSizing");
			builder.PutPropertiesInCategory(Categories.Layout, typeof(DockLayoutControl),
				"GetDock", "GetUseDesiredWidthAsMaxWidth", "GetUseDesiredHeightAsMaxHeight");
			builder.PutPropertiesInCategory(Categories.Layout, typeof(FlowLayoutControl),
				"BreakFlowToFit", "LayerSpace", "MaximizedElementPosition", "Orientation", "ShowLayerSeparators", "StretchContent", "GetIsFlowBreak");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(FlowLayoutControl),
				"AllowAddFlowBreaksDuringItemMoving", "AllowItemMoving", "AllowLayerSizing", "AllowMaximizedElementMoving",
				"AnimateItemMaximization", "AnimateItemMoving",
				"ItemsSource", "ItemTemplate", "ItemTemplateSelector", "MaximizedElement");
			builder.PutPropertiesInCategory(Categories.Appearance, typeof(GroupBox),
				"CornerRadius", "ShadowOffset", "ShowShadow");
			builder.PutPropertiesInCategory(Categories.Layout, typeof(GroupBox),
				"MaximizeElementVisibility", "MinimizeElementVisibility", "TitleVisibility");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(GroupBox),
				"Content", "Header", "State");
			builder.PutPropertiesInCategory(Categories.Layout, typeof(LayoutGroup),
#if !SILVERLIGHT
				"MeasureSelectedTabChildOnly",
#endif
				"Orientation");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(LayoutGroup),
				"Header", "IsCollapsed", "IsCollapsible", "IsLocked", "ItemLabelsAlignment", "SelectedTabIndex", "View");
			builder.PutPropertiesInCategory(Categories.Layout, typeof(LayoutControl),
				"StretchContentHorizontally", "StretchContentVertically", "GetUseDesiredWidthAsMaxWidth", "GetUseDesiredHeightAsMaxHeight");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(LayoutControl),
				"AllowAvailableItemsDuringCustomization", "AllowItemMovingDuringCustomization", "AllowItemRenamingDuringCustomization",
				"AllowItemSizing", "AllowItemSizingDuringCustomization", "AllowNewItemsDuringCustomization",
				"AvailableItems", "IsCustomization", "LayoutUri",
				"GetAllowHorizontalSizing", "GetAllowVerticalSizing", "GetCustomizationLabel", "GetTabHeader");
			builder.PutPropertiesInCategory(Categories.Layout, typeof(LayoutItem),
				"ElementSpace", "LabelHorizontalAlignment", "LabelPosition", "LabelVerticalAlignment");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(LayoutItem),
				"AddColonToLabel", "Content", "IsRequired", "Label");
			builder.PutPropertiesInCategory(Categories.Layout, typeof(ScrollBox),
				"GetLeft", "GetTop");
			builder.PutPropertiesInCategory(Categories.Layout, typeof(TileLayoutControl),
				"GroupHeaderSpace");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(TileLayoutControl),
				"AllowGroupHeaderEditing", "GetGroupHeader", "GroupHeaderTemplate", "TileClickCommand");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(Tile),
				"AnimateContentChange", "Command", "CommandParameter", "Content", "ContentChangeInterval", "ContentSource", "Header");
			builder.PutPropertiesInCategory(Categories.Layout, typeof(Tile),
				"HorizontalHeaderAlignment", "Size", "VerticalHeaderAlignment");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(DataLayoutControl),
				"AddColonToItemLabels", "AutoGenerateItems", "AutoGeneratedItemsLocation", "CurrentItem", "IsReadOnly");
			builder.PutPropertiesInCategory(Categories.CommonProperties, typeof(DataLayoutItem),
				"Binding", "IsReadOnly");
		}
		void PositionProperties(AttributeTableBuilder builder) {
			builder.PositionProperties(PropertyOrder.CreateBefore(PropertyOrder.Default), typeof(LayoutItem), "LabelPosition");
		}
		void RegisterFeatures(AttributeTableBuilder builder) {
			builder.AddAttributes(typeof(GroupBox),
				new FeatureAttribute(typeof(GroupBoxDefaultInitializer)),
				new FeatureAttribute(typeof(GroupBoxParentAdapter)),
				new FeatureAttribute(typeof(SimplePlacementAdapter)));
			builder.AddAttributes(typeof(LayoutGroup),
				new FeatureAttribute(typeof(LayoutGroupDefaultInitializer)),
				new FeatureAttribute(typeof(LayoutGroupDesignModeValueProvider)),
				new FeatureAttribute(typeof(LayoutGroupParentAdapter)),
				new FeatureAttribute(typeof(LayoutGroupAdornerProvider)),
				new FeatureAttribute(typeof(LayoutGroupContextMenuProvider)));
			builder.AddAttributes(typeof(LayoutControl),
				new FeatureAttribute(typeof(LayoutControlAdornerProvider)),
				new FeatureAttribute(typeof(LayoutControlContextMenuProvider)));
			builder.AddAttributes(typeof(LayoutItem),
				new FeatureAttribute(typeof(LayoutItemDefaultInitializer)),
				new FeatureAttribute(typeof(LayoutItemParentAdapter)),
				new FeatureAttribute(typeof(SimplePlacementAdapter)));
			builder.AddAttributes(typeof(TileLayoutControl),
				new DefaultEventAttribute("TileClick"),
				new FeatureAttribute(typeof(TileLayoutControlDefaultInitializer)));
			builder.AddAttributes(typeof(Tile),
				new DefaultEventAttribute("Click"),
				new FeatureAttribute(typeof(TileDefaultInitializer)),
				new FeatureAttribute(typeof(TileParentAdapter)),
				new FeatureAttribute(typeof(SimplePlacementAdapter)));
			builder.AddAttributes(typeof(DataLayoutControl),
				new FeatureAttribute(typeof(DisabledParentAdapter)));
			builder.AddAttributes(typeof(DataLayoutItem),
				new FeatureAttribute(typeof(DisabledParentAdapter)));
			builder.AddAttributes(typeof(System.Windows.FrameworkElement),
				new FeatureAttribute(typeof(LayoutGroupChildAdornerProvider)),
				new FeatureAttribute(typeof(LayoutGroupChildContextMenuProvider)),
				new FeatureAttribute(typeof(FlowLayoutControlChildContextMenuProvider)),
				new FeatureAttribute(typeof(DockLayoutControlChildContextMenuProvider)));
		}
	}
}

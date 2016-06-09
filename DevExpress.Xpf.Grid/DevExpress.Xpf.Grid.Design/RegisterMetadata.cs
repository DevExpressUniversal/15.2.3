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

#if SILVERLIGHT
extern alias Platform;
#endif
using System;
using System.Linq;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Features;
#if SL
using DependencyObject = Platform::System.Windows.DependencyObject;
using DependencyProperty = Platform::System.Windows.DependencyProperty;
using PropertyMetadata = Platform::System.Windows.PropertyMetadata;
using Point = Platform::System.Windows.Point;
using RoutedEventHandler = Platform::System.Windows.RoutedEventHandler;
using RoutedEventArgs = Platform::System.Windows.RoutedEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using FrameworkElement = Platform::System.Windows.FrameworkElement;
using ToggleStateButton = DevExpress.Xpf.Core.Design.CoreUtils.ToggleStateButton;
using UIElement = Platform::System.Windows.FrameworkElement;
using LayoutHelper = Platform::DevExpress.Xpf.Core.Native.LayoutHelper;
using HitTestResult = Platform::DevExpress.Xpf.Core.HitTestResult;
using HitTestResultBehavior = Platform::DevExpress.Xpf.Core.HitTestResultBehavior;
using HitTestFilterCallback = Platform::DevExpress.Xpf.Core.HitTestFilterCallback;
using HitTestResultCallback = Platform::DevExpress.Xpf.Core.HitTestResultCallback;
using HitTestFilterBehavior = Platform::DevExpress.Xpf.Core.HitTestFilterBehavior;
using PointHitTestParameters = Platform::DevExpress.Xpf.Core.PointHitTestParameters;
using DesignTimeSelectionControl = Platform::DevExpress.Xpf.Grid.DesignTimeSelectionControl;
using DesignerSerializationVisibility = Platform::System.ComponentModel.DesignerSerializationVisibility;
using DesignerSerializationVisibilityAttribute = Platform::System.ComponentModel.DesignerSerializationVisibilityAttribute;
using PropertyDescriptor = Platform::DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Xpf.Core.Design.CoreUtils;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Grid;
using Platform::DevExpress.Xpf.Grid.Native;
using Platform::DevExpress.Data;
using Platform::DevExpress.Xpf.Bars.Helpers;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Utils;
using Platform::DevExpress.Xpf.Core.Commands;
using Platform::DevExpress.Xpf.Core.WPFCompatibility;
using Platform::DevExpress.Xpf.Data.Native;
using AssemblyInfo = Platform::AssemblyInfo;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Grid.LookUp;
using System.ComponentModel;
using DevExpress.Xpf.Design;
using DevExpress.Xpf.Core.Design.SmartTags;
using Platform::System.Windows;
using GridControlHelper = Platform::DevExpress.Xpf.Grid.Native.GridControlHelper;
using GridColumnHeader = Platform::DevExpress.Xpf.Grid.GridColumnHeader;
using BandHeaderControl = Platform::DevExpress.Xpf.Grid.BandHeaderControl;
using Button = Platform::System.Windows.Controls.Button;
using ButtonBase = Platform::System.Windows.Controls.Primitives.ButtonBase;
using ContentControl = Platform::System.Windows.Controls.ContentControl;
using Control = Platform::System.Windows.Controls.Control;
using SummaryItemBase = Platform::DevExpress.Xpf.Grid.SummaryItemBase;
#else
using DevExpress.Xpf.Design;
using DevExpress.Xpf.Grid.Native;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Grid.LookUp;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.Design.SmartTags;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using Microsoft.Windows.Design.Interaction;
using System.Windows.Input;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
#endif
namespace DevExpress.Xpf.Grid.Design {
	internal abstract class DataControlInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			item.Properties["AutoGenerateColumns"].SetValue(AutoGenerateColumnsMode.AddNew);
			item.Properties["EnableSmartColumnsGeneration"].SetValue(true);
			ModelItem viewItem = GridDesignTimeHelper.CreateModelItem(item.Context, ViewType);
			item.Properties["View"].SetValue(viewItem);
			InitializerHelper.Initialize(item);
		}
		void AddColumn(ModelItem item, string fieldName) {
			ColumnBase column = CreateColumn();
			column.FieldName = fieldName;
			GridDesignTimeHelper.GetGridColumnsCollection(item).Add(column);
		}
		protected abstract Type ViewType { get; }
		protected abstract ColumnBase CreateColumn();
	}
	internal class GridControlInitializer : DataControlInitializer {
		protected override Type ViewType { get { return typeof(TableView); } }
		protected override ColumnBase CreateColumn() {
			return new GridColumn();
		}
	}
	internal class TreeListControlInitializer : DataControlInitializer {
		protected override Type ViewType { get { return typeof(TreeListView); } }
		protected override ColumnBase CreateColumn() {
			return new TreeListColumn();
		}
	}
	internal class DataControlViewInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			item.Properties["ShowTotalSummary"].SetValue(true);
		}
	}
	internal class TableViewInitializer : DataControlViewInitializer {
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			item.Properties["AllowPerPixelScrolling"].SetValue(true);
		}
	}
	internal class RegisterHelper {
		public static void PrepareAttributeTable(AttributeTableBuilder builder) {
			builder.AddCustomAttributes(typeof(GridControl), "View", new NewItemTypesAttribute(GridDesignTimeHelper.GridAvailableViewTypes));
			builder.AddCustomAttributes(typeof(GridColumn), "EditSettings", new NewItemTypesAttribute(TypeLists.EditSettingsTypes.Where(t => (t.Item2 & PropertyTarget.Grid) != 0).Select(t => t.Item1).ToArray()));
			builder.AddCustomAttributes(typeof(GridControl), "DetailDescriptor", new NewItemTypesAttribute(GridDesignTimeHelper.GridDetailDescriptorTypes));
			builder.AddCustomAttributes(typeof(DataControlDetailDescriptor), "DataControl", new NewItemTypesAttribute(typeof(GridControl)));
			builder.AddCustomAttributes(typeof(TabViewDetailDescriptor), "DetailDescriptors", new NewItemTypesAttribute(GridDesignTimeHelper.GridDetailDescriptorTypes));
			builder.AddCustomAttributes(typeof(GridControl), new FeatureAttribute(typeof(GridControlInitializer)));
			builder.AddCustomAttributes(typeof(GridControl), new FeatureAttribute(typeof(GridControlAdornerProvider)));
			builder.AddCustomAttributes(typeof(TreeListControl), "View", new NewItemTypesAttribute(GridDesignTimeHelper.TreeListAvailableViewTypes));
			builder.AddCustomAttributes(typeof(TreeListColumn), "EditSettings", new NewItemTypesAttribute(TypeLists.EditSettingsTypes.Where(t => (t.Item2 & PropertyTarget.Grid) != 0).Select(t => t.Item1).ToArray()));
			builder.AddCustomAttributes(typeof(TreeListControl), new FeatureAttribute(typeof(TreeListControlInitializer)));
			builder.AddCustomAttributes(typeof(TreeListControl), new FeatureAttribute(typeof(TreeListControlAdornerProvider)));
			builder.AddCustomAttributes(typeof(TableView), new FeatureAttribute(typeof(TableViewInitializer)));
			builder.AddCustomAttributes(typeof(TreeListView), new FeatureAttribute(typeof(TableViewInitializer)));
#if !SILVERLIGHT
			builder.AddCustomAttributes(typeof(CardView), new FeatureAttribute(typeof(DataControlViewInitializer)));
#endif      
			builder.AddCustomAttributes(typeof(DataViewBase), new FeatureAttribute(typeof(DataViewBaseAdornerProvider)));
#if !SILVERLIGHT
			RegisterCustomEditor(builder, typeof(UnboundExpressionEditor), ColumnBase.UnboundExpressionProperty);
			RegisterCustomEditor(builder, typeof(FilterStringEditor), DataControlBase.FilterStringProperty);
#endif
			builder.AddCustomAttributes(typeof(ColumnBase), DesignHelper.GetPropertyName(ColumnBase.FieldNameProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FieldNameEditor)));
			builder.AddCustomAttributes(typeof(SummaryItemBase), DesignHelper.GetPropertyName(SummaryItemBase.FieldNameProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FieldNameEditor)));
			builder.AddCustomAttributes(typeof(SummaryItemBase), DesignHelper.GetPropertyName(SummaryItemBase.ShowInColumnProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FieldNameEditor)));
			builder.AddCustomAttributes(typeof(DataControlBase), DesignHelper.GetPropertyName(GridControl.DesignTimeDataObjectTypeProperty), new TypeConverterAttribute(typeof(TypeTypeConverter)));
			builder.AddCustomAttributes(typeof(LookUpEdit), DesignHelper.GetPropertyName(LookUpEdit.StyleSettingsProperty), new NewItemTypesAttribute(typeof(LookUpEditStyleSettings), typeof(SearchLookUpEditStyleSettings)));
			builder.AddCustomAttributes(typeof(BaseColumn), new FeatureAttribute(typeof(RemoveHeaderTaskProvider)));
#if !SILVERLIGHT
			builder.AddCustomAttributes(typeof(TableView), "FormatConditions", new NewItemTypesAttribute(typeof(ColorScaleFormatCondition)));
			builder.AddCustomAttributes(typeof(TableView), "FormatConditions", new NewItemTypesAttribute(typeof(DataBarFormatCondition)));
			builder.AddCustomAttributes(typeof(TableView), "FormatConditions", new NewItemTypesAttribute(typeof(FormatCondition)));
			builder.AddCustomAttributes(typeof(TableView), "FormatConditions", new NewItemTypesAttribute(typeof(IconSetFormatCondition)));
			builder.AddCustomAttributes(typeof(TableView), "FormatConditions", new NewItemTypesAttribute(typeof(TopBottomRuleFormatCondition)));
			builder.AddCustomAttributes(typeof(FormatConditionBase), DesignHelper.GetPropertyName(FormatConditionBase.FieldNameProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FieldNameEditor)));
			builder.AddCustomAttributes(typeof(FormatConditionBase), DesignHelper.GetPropertyName(FormatConditionBase.PredefinedFormatNameProperty), PropertyValueEditor.CreateEditorAttribute(typeof(ConditionFormatNameEditor)));
			builder.AddCustomAttributes(typeof(ColorScaleFormatCondition), DesignHelper.GetPropertyName(ColorScaleFormatCondition.ExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionEditor)));
			builder.AddCustomAttributes(typeof(DataBarFormatCondition), DesignHelper.GetPropertyName(DataBarFormatCondition.ExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionEditor)));
			builder.AddCustomAttributes(typeof(FormatCondition), DesignHelper.GetPropertyName(FormatCondition.ExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionFilterEditor)));
			builder.AddCustomAttributes(typeof(IconSetFormatCondition), DesignHelper.GetPropertyName(IconSetFormatCondition.ExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionEditor)));
			builder.AddCustomAttributes(typeof(TopBottomRuleFormatCondition), DesignHelper.GetPropertyName(TopBottomRuleFormatCondition.ExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionEditor)));
			builder.AddCustomAttributes(typeof(IndicatorFormatConditionBase), DesignHelper.GetPropertyName(IndicatorFormatConditionBase.SelectiveExpressionProperty), PropertyValueEditor.CreateEditorAttribute(typeof(FormatConditionExpressionFilterEditor)));
			builder.AddCustomAttributes(typeof(ColumnBase), new FeatureAttribute(typeof(ColumnContextMenuProvider)));
#endif
		}
#if !SILVERLIGHT
		static void RegisterCustomEditor(AttributeTableBuilder builder, Type editorType, DependencyProperty property) {
			builder.AddCustomAttributes(property.OwnerType, property.Name,
				PropertyValueEditor.CreateEditorAttribute(editorType));
		}
#endif
	}
	internal class RegisterMetadata : MetadataProviderBase {
		protected override void PrepareAttributeTable(AttributeTableBuilder builder) {
			RegisterHelper.PrepareAttributeTable(builder);
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new LookUpEditPropertyLinesProvider(typeof(LookUpEdit), PropertyTarget.Editor), false);
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new GridControlPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new TreeListControlPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new TableViewProperyLinesProvider());
#if !SL
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new CardViewPropertyLinesProvider());
#endif
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new TreeListViewPropertyLinesProvider());
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new BandPropertyLinesProvider());
			TypeDescriptor.AddAttributes(typeof(BandBase), new DevExpress.Xpf.Core.Design.SmartTags.DesignTimeParentAttribute(typeof(DataControlBase), typeof(GridBandViewProvider)));
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new ColumnPropertyLinesProvider());
			TypeDescriptor.AddAttributes(typeof(ColumnBase), new DevExpress.Xpf.Core.Design.SmartTags.DesignTimeParentAttribute(typeof(DataControlBase), typeof(GridColumnViewProvider)));
			TypeDescriptor.AddAttributes(typeof(DataViewBase), new DevExpress.Xpf.Core.Design.SmartTags.UseParentPropertyLinesAttribute(typeof(GridControl)));
			TypeDescriptor.AddAttributes(typeof(BaseEditSettings), new DevExpress.Xpf.Core.Design.SmartTags.UseParentPropertyLinesAttribute(typeof(ColumnBase)));
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(new LookUpEditPropertyLinesProvider(typeof(LookUpEditSettings), PropertyTarget.Grid), false);
		}
		protected override Assembly RuntimeAssembly { get { return typeof(GridControl).Assembly; } }
		protected override string ToolboxCategoryPath { get { return AssemblyInfo.DXTabNameData; } }
	}
	public class GridColumnViewProvider : IViewProvider {
		public FrameworkElement ProvideView(ModelItem item) {
			if(item == null || item.Parent == null || (item.Parent.View == null && !(item.Parent.GetCurrentValue() is BandBase)))
				return null;
			ColumnBase column = item.GetCurrentValue() as ColumnBase;
			if(column == null)
				return null;
			GridColumnHeader[] headers = GridControlHelper.GetColumnHeaderElements(GetDataControl(item), column);
			if(headers == null)
				return null;
			foreach(GridColumnHeader header in headers) {
				return header;
			}
			return null;
		}
		DataControlBase GetDataControl(ModelItem item) {
			ModelItem gridModel = BarManagerDesignTimeHelper.FindParentByType<DataControlBase>(item);
			if(gridModel == null)
				return null;
			return gridModel.GetCurrentValue() as DataControlBase;
		}
	}
	public class GridBandViewProvider : IViewProvider {
		public FrameworkElement ProvideView(ModelItem item) {
			if(item == null || item.Parent == null)
				return null;
			BandBase band = item.GetCurrentValue() as BandBase;
			if(band == null)
				return null;
			ModelItem gridModel = BarManagerDesignTimeHelper.FindParentByType<DataControlBase>(item);
			if(gridModel == null)
				return null;
			BandHeaderControl header = GridControlHelper.GetBandHeaderElement(gridModel.GetCurrentValue() as DataControlBase, band);
			return header;
		}
	}
	class ColumnContextMenuProvider : DevExpress.Xpf.Core.Design.ConditionalFormatting.ConditionalFormattingContextMenuProviderBase {
		ConditionalFormattingDialogServant servant = new ConditionalFormattingDialogServant();
		protected override bool IsConditionalFormattingMenuAllowed(ModelItem primarySelection) {
			return servant.IsConditionalFormattingMenuAllowed(primarySelection);
		}
		protected override IConditionalFormattingCommands GetCommands(ModelItem primarySelection) {
			return servant.GetCommands(primarySelection);
		}
		protected override void BeforeExecuteFormatConditionCommand(ModelItem primarySelection) {
			servant.PrepareForConditionCommand(primarySelection);
		}
		protected override void CreateClearConditionRules(MenuGroup clearRules) {
			CreateFormatConditionMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_ClearRules_FromAllColumns, x => x.ClearFormatConditionsFromAllColumns, clearRules);
			CreateFormatConditionMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_ClearRules_FromCurrentColumns, x => x.ClearFormatConditionsFromColumn, clearRules);
		}
		protected override object CreateDataBarFormatCondition(ModelItem primarySelection, string formatName) {
			return CreateIndicatorCondition<DataBarFormatCondition>(primarySelection, formatName);
		}
		protected override object CreateColorScaleFormatCondition(ModelItem primarySelection, string formatName) {
			return CreateIndicatorCondition<ColorScaleFormatCondition>(primarySelection, formatName);
		}
		protected override object CreateIconSetFormatCondition(ModelItem primarySelection, string formatName) {
			return CreateIndicatorCondition<IconSetFormatCondition>(primarySelection, formatName);
		}
		object CreateIndicatorCondition<T>(ModelItem columnModelItem, string formatName) where T : IndicatorFormatConditionBase, new() {
			T condition = new T();
			condition.PredefinedFormatName = formatName;
			GridControlHelper.ClearFormatProperty(condition);
			condition.FieldName = (string)columnModelItem.Properties[ColumnBase.FieldNameProperty.Name].Value.GetCurrentValue();
			return condition;
		}
	}
	class ConditionalFormattingDialogServant {
		public bool IsConditionalFormattingMenuAllowed(ModelItem columnModelItem) {
			ModelItem grid = GetGridModelItem(columnModelItem);
			if(grid == null)
				return false;
			ModelProperty viewProperty = GetViewProperty(grid);
			if(viewProperty == null)
				return false;
			if(!viewProperty.IsSet)
				return true;
			Type viewType = viewProperty.Value.ItemType;
			return typeof(TableView).IsAssignableFrom(viewType) || typeof(TreeListView).IsAssignableFrom(viewType);
		}
		public void PrepareForConditionCommand(ModelItem primarySelection) {
			DataViewBase view = GetView(primarySelection);
			if(view != null && !GetViewProperty(GetGridModelItem(primarySelection)).IsSet) {
				DataControlAdornerProvider provider = GridControlHelper.GetDesignTimeAdorner(view) as DataControlAdornerProvider;
				if(provider != null)
					provider.ChangeViewType(typeof(TableView));
			}
		}
		public IConditionalFormattingCommands GetCommands(ModelItem primarySelection) {
			DataViewBase view = GetView(primarySelection);
			if(view != null)
				return view.Commands as IConditionalFormattingCommands;
			return null;
		}
		ModelProperty GetViewProperty(ModelItem grid) {
			DependencyProperty viewProperty = null;
			if(typeof(GridControl).IsAssignableFrom(grid.ItemType))
				viewProperty = GridControl.ViewProperty;
			else if(typeof(TreeListControl).IsAssignableFrom(grid.ItemType))
				viewProperty = TreeListControl.ViewProperty;
			if(viewProperty == null)
				return null;
			return grid.Properties[viewProperty.Name];
		}
		ModelItem GetGridModelItem(ModelItem columnModelItem) {
			return BarManagerDesignTimeHelper.FindParentByType<DataControlBase>(columnModelItem);
		}
		DataViewBase GetView(ModelItem columnModelItem) {
			return GridControlHelper.GetView((DataControlBase)GetGridModelItem(columnModelItem).GetCurrentValue()) as DataViewBase;
		}
	}
}

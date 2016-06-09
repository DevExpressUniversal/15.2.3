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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using DevExpress.Xpf.PropertyGrid.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data.Filtering;
using System.Windows.Markup;
using DevExpress.Utils;
using DevExpress.Xpf.Bars.Native;
using System.Collections.Specialized;
using DataController = DevExpress.Xpf.PropertyGrid.Internal.DataController;
using DataView = DevExpress.Xpf.PropertyGrid.Internal.DataView;
namespace DevExpress.Xpf.PropertyGrid {
	public enum PropertyGridSortMode {
		Ascending,
		Descending,
		Custom,
		Definitions,
		NoSort,
		Unspecified
	};
	[Flags]
	public enum PropertyGridFilterMode {
		ByHeader = 0x1,
		ByValue = 0x2,
		ByHeaderAndValue = ByHeader | ByValue
	};
	[Flags]
	public enum DescriptionLocation {
		ToolTip = 0x1,
		Panel = 0x2,
		ToolTipAndPanel = ToolTip | Panel,
		None = 0x0
	}
	public enum ShowPropertiesMode {
		All,
		WithPropertyDefinitions
	}
	[Flags]
	public enum ShowMenuMode {
		OnRightClick = 0x1,
		OnMenuButtonClick = 0x2,
		Always = OnRightClick | OnMenuButtonClick
	}
	[DXToolboxBrowsable]
	[ContentProperty("PropertyDefinitions")]
	public class PropertyGridControl : Control, INavigationSupport {
		public static readonly RoutedEvent SortEvent;
		public static readonly RoutedEvent CellValueChangedEvent;
		public static readonly RoutedEvent CellValueChangingEvent;
		public static readonly RoutedEvent ValidateCellEvent;
		public static readonly RoutedEvent InvalidCellExceptionEvent;
		public static readonly RoutedEvent SelectionChangedEvent;
		public static readonly RoutedEvent CustomDisplayTextEvent;
		public static readonly RoutedEvent CustomExpandEvent;
		public static readonly RoutedEvent ShownEditorEvent;
		public static readonly RoutedEvent HiddenEditorEvent;
		public static readonly RoutedEvent ShowingEditorEvent;
		public static readonly DependencyProperty SelectedObjectProperty;
		public static readonly DependencyProperty SelectedObjectsProperty;
		public static readonly DependencyProperty ShowCategoriesProperty;
		public static readonly DependencyProperty ExpandCategoriesWhenSelectedObjectChangedProperty;
		public static readonly DependencyProperty SortModeProperty;
		public static readonly DependencyProperty PropertyDefinitionsProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty PropertyDefinitionsAttachedBehaviorProperty;
		public static readonly DependencyProperty PropertyDefinitionsSourceProperty;
		public static readonly DependencyProperty PropertyDefinitionStyleProperty;
		public static readonly DependencyProperty PropertyDefinitionTemplateProperty;
		public static readonly DependencyProperty PropertyDefinitionTemplateSelectorProperty;
		public static readonly DependencyProperty FilterCriteriaProperty;
		public static readonly DependencyProperty UserFilterCriteriaProperty;
		public static readonly DependencyProperty ShowDescriptionInProperty;
		public static readonly DependencyProperty ShowSearchBoxProperty;
		public static readonly DependencyProperty ShowToolPanelProperty;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected static readonly DependencyPropertyKey ActualDescriptionContainerStyleSelectorPropertyKey;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static readonly DependencyProperty ActualDescriptionContainerStyleSelectorProperty;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected static readonly DependencyPropertyKey ActualDescriptionTemplateSelectorPropertyKey;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static readonly DependencyProperty ActualDescriptionTemplateSelectorProperty;
		public static readonly DependencyProperty DescriptionContainerStyleSelectorProperty;
		public static readonly DependencyProperty DescriptionContainerStyleProperty;
		public static readonly DependencyProperty DescriptionTemplateSelectorProperty;
		public static readonly DependencyProperty DescriptionTemplateProperty;
		protected static readonly DependencyPropertyKey ActualDescriptionContainerStylePropertyKey;
		public static readonly DependencyProperty ActualDescriptionContainerStyleProperty;
		public static readonly DependencyProperty UseCollectionEditorProperty;
		public static readonly DependencyProperty AllowListItemInitializerProperty;
		public static readonly DependencyProperty AllowInstanceInitializerProperty;
		public static readonly DependencyProperty HeaderColumnWidthProperty; 
		public static readonly DependencyProperty ValueColumnWidthProperty; 
		public static readonly DependencyProperty HeaderColumnMaxWidthProperty; 
		public static readonly DependencyProperty HeaderColumnMinWidthProperty; 
		public static readonly DependencyProperty ValueColumnMinWidthProperty; 
		public static readonly DependencyProperty ValueColumnMaxWidthProperty; 
		public static readonly DependencyProperty HighlightNonDefaultValuesProperty;
		public static readonly DependencyProperty FilterModeProperty;
		public static readonly DependencyProperty ShowPropertiesProperty;
		public static readonly DependencyProperty ShowMenuButtonInRowsProperty;
		public static readonly DependencyProperty SelectedPropertyPathProperty;
		public static readonly DependencyProperty SelectedPropertyValueProperty;
		static readonly DependencyPropertyKey SelectedPropertyValuePropertyKey;
		public static readonly DependencyProperty AllowExpandingProperty;
		public static readonly DependencyProperty TrimDisplayTextProperty;
		public static readonly DependencyProperty ShowMenuProperty;
		public static readonly DependencyProperty UseOptimizedEditorsProperty;
		public static readonly DependencyProperty ReadOnlyProperty;
		public static readonly DependencyProperty AllowCommitOnValidationAttributeErrorProperty;
		static PropertyGridControl() {
			Type ownerType = typeof(PropertyGridControl);
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridControl), new FrameworkPropertyMetadata(typeof(PropertyGridControl)));
			NavigationManager.NavigationModeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(NavigationMode.Local));
			HeaderColumnWidthProperty = DependencyPropertyManager.Register("HeaderColumnWidth", typeof(GridLength), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
			HeaderColumnMaxWidthProperty = DependencyPropertyManager.Register("HeaderColumnMaxWidth", typeof(double), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
			HeaderColumnMinWidthProperty = DependencyPropertyManager.Register("HeaderColumnMinWidth", typeof(double), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
			ValueColumnWidthProperty = DependencyPropertyManager.Register("ValueColumnWidth", typeof(GridLength), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
			ValueColumnMinWidthProperty = DependencyPropertyManager.Register("ValueColumnMinWidth", typeof(double), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
			ValueColumnMaxWidthProperty = DependencyPropertyManager.Register("ValueColumnMaxWidth", typeof(double), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
			SortEvent = EventManager.RegisterRoutedEvent("Sort", RoutingStrategy.Direct, typeof(PropertyGridSortingEventHandler), typeof(PropertyGridControl));
			CellValueChangedEvent = EventManager.RegisterRoutedEvent("CellValueChanged", RoutingStrategy.Direct, typeof(CellValueChangedEventHandler), typeof(PropertyGridControl));
			CellValueChangingEvent = EventManager.RegisterRoutedEvent("CellValueChanging", RoutingStrategy.Direct, typeof(CellValueChangingEventHandler), typeof(PropertyGridControl));
			ValidateCellEvent = EventManager.RegisterRoutedEvent("ValidateCell", RoutingStrategy.Direct, typeof(ValidateCellEventHandler), typeof(PropertyGridControl));
			InvalidCellExceptionEvent = EventManager.RegisterRoutedEvent("InvalidCellException", RoutingStrategy.Direct, typeof(InvalidCellExceptionEventHandler), typeof(PropertyGridControl));
			SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(PropertyGridControl));
			CustomDisplayTextEvent = EventManager.RegisterRoutedEvent("CustomDisplayText", RoutingStrategy.Direct, typeof(CustomDisplayTextEventHandler), typeof(PropertyGridControl));
			CustomExpandEvent = EventManager.RegisterRoutedEvent("CustomExpand", RoutingStrategy.Direct, typeof(CustomExpandEventHandler), typeof(PropertyGridControl));
			ShownEditorEvent = EventManager.RegisterRoutedEvent("ShownEditor", RoutingStrategy.Direct, typeof(PropertyGridEditorEventHandler), typeof(PropertyGridControl));
			HiddenEditorEvent = EventManager.RegisterRoutedEvent("HiddenEditor", RoutingStrategy.Direct, typeof(PropertyGridEditorEventHandler), typeof(PropertyGridControl));
			ShowingEditorEvent = EventManager.RegisterRoutedEvent("ShowingEditor", RoutingStrategy.Direct, typeof(ShowingPropertyGridEditorEventHandler), typeof(PropertyGridControl));
			EventManager.RegisterClassHandler(typeof(PropertyGridControl), PreviewMouseDownEvent, new MouseButtonEventHandler(ProcessPreviewMouseLeftButtonDown), true);
			EventManager.RegisterClassHandler(typeof(PropertyGridControl), PreviewMouseUpEvent, new MouseButtonEventHandler(ProcessPreviewMouseUp), true);
			SelectedObjectProperty = DependencyPropertyManager.Register("SelectedObject", typeof(object), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnSelectedObjectChanged(e.OldValue, e.NewValue)));
			SelectedObjectsProperty = DependencyPropertyManager.Register("SelectedObjects", typeof(IEnumerable), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).SelectedObjectsChanged((IEnumerable)e.OldValue)));
			ShowCategoriesProperty = DependencyPropertyManager.Register("ShowCategories", typeof(bool), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(true, (d, e) => ((PropertyGridControl)d).OnShowCategoriesChanged((bool)e.OldValue),
					(d, value) => ((PropertyGridControl)d).OnShowCategoriesChanging((bool)value)));
			PropertyDefinitionsProperty = DependencyPropertyManager.Register("PropertyDefinitions", typeof(PropertyDefinitionCollection), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnPropertyDefinitionsChanged((PropertyDefinitionCollection)e.OldValue), (d, e) => ((PropertyGridControl)d).CoercePropertyDefinitions(e as PropertyDefinitionCollection)));
			SortModeProperty = DependencyPropertyManager.Register("SortMode", typeof(PropertyGridSortMode), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(PropertyGridSortMode.NoSort, (d, e) => ((PropertyGridControl)d).OnSortModeChanged((PropertyGridSortMode)e.OldValue, (PropertyGridSortMode)e.NewValue)));
			ExpandCategoriesWhenSelectedObjectChangedProperty = DependencyPropertyManager.Register("ExpandCategoriesWhenSelectedObjectChanged", typeof(bool), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(false));			
			FilterCriteriaProperty = DependencyPropertyManager.Register("FilterCriteria", typeof(CriteriaOperator), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnFilerCriteriaChanged((CriteriaOperator)e.OldValue, (CriteriaOperator)e.NewValue)));
			UserFilterCriteriaProperty = DependencyPropertyManager.Register("UserFilterCriteria", typeof(CriteriaOperator), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnUserFilterCriteriaChanged((CriteriaOperator)e.OldValue, (CriteriaOperator)e.NewValue)));
			PropertyDefinitionsSourceProperty = DependencyPropertyManager.Register("PropertyDefinitionsSource", typeof(IEnumerable), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnPropertyDefinitionsSourceChanged(e)));
			PropertyDefinitionStyleProperty = DependencyPropertyManager.Register("PropertyDefinitionStyle", typeof(Style), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnPropertyDefinitionStyleChanged(e)));
			PropertyDefinitionTemplateProperty = DependencyPropertyManager.Register("PropertyDefinitionTemplate", typeof(DataTemplate), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnPropertyDefinitionTemplateChanged(e)));
			PropertyDefinitionTemplateSelectorProperty = DependencyPropertyManager.Register("PropertyDefinitionTemplateSelector", typeof(DataTemplateSelector), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnPropertyDefinitionTemplateSelectorChanged(e)));
			PropertyDefinitionsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("PropertyDefinitiosAttachedBehavior",
				typeof(ItemsAttachedBehaviorCore<PropertyGridControl, PropertyDefinitionBase>), typeof(PropertyGridControl), new PropertyMetadata(null));
			ActualDescriptionContainerStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDescriptionContainerStyle", typeof(Style), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null));
			ActualDescriptionContainerStyleProperty = ActualDescriptionContainerStylePropertyKey.DependencyProperty;
			ActualDescriptionContainerStyleSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDescriptionContainerStyleSelector", typeof(StyleSelector),
				typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnActualDescriptionContainerStyleSelectorChanged((StyleSelector)e.OldValue, (StyleSelector)e.NewValue)));
			ActualDescriptionContainerStyleSelectorProperty = ActualDescriptionContainerStyleSelectorPropertyKey.DependencyProperty;
			ActualDescriptionTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDescriptionTemplateSelector", typeof(DataTemplateSelector), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null));
			ActualDescriptionTemplateSelectorProperty = ActualDescriptionTemplateSelectorPropertyKey.DependencyProperty;
			DescriptionContainerStyleSelectorProperty = DependencyPropertyManager.Register("DescriptionContainerStyleSelector", typeof(StyleSelector), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnDescriptionContainerStyleSelectorChanged((StyleSelector)e.OldValue, (StyleSelector)e.NewValue)));
			DescriptionContainerStyleProperty = DependencyPropertyManager.Register("DescriptionContainerStyle", typeof(Style), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnDescriptionContainerStyleChanged((Style)e.OldValue, (Style)e.NewValue)));
			DescriptionTemplateSelectorProperty = DependencyPropertyManager.Register("DescriptionTemplateSelector", typeof(DataTemplateSelector), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnDescriptionTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
			DescriptionTemplateProperty = DependencyPropertyManager.Register("DescriptionTemplate", typeof(DataTemplate), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnDescriptionTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
			HighlightNonDefaultValuesProperty = DependencyPropertyManager.Register("HighlightNonDefaultValues", typeof(bool), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(true, (d, e) => ((PropertyGridControl)d).OnHighlightNonDefaultValuesChanged((bool)e.NewValue)));
			FilterModeProperty = DependencyPropertyManager.Register("FilterMode", typeof(PropertyGridFilterMode), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(PropertyGridFilterMode.ByHeader, (d, e) => ((PropertyGridControl)d).OnFilterModeChanged((PropertyGridFilterMode)e.OldValue)));
			ShowDescriptionInProperty = DependencyPropertyManager.Register("ShowDescriptionIn", typeof(DescriptionLocation), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(DescriptionLocation.ToolTip));
			ShowSearchBoxProperty = DependencyPropertyManager.Register("ShowSearchBox", typeof(bool), typeof(PropertyGridControl), new FrameworkPropertyMetadata(true));
			ShowToolPanelProperty = DependencyPropertyManager.Register("ShowToolPanel", typeof(bool), typeof(PropertyGridControl), new FrameworkPropertyMetadata(true));
			UseCollectionEditorProperty = DependencyPropertyManager.Register("UseCollectionEditor", typeof(bool?), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnUseCollectionEditorChanged((bool?)e.OldValue, (bool?)e.NewValue)));
			AllowListItemInitializerProperty = DependencyPropertyManager.Register("AllowListItemInitializer", typeof(bool?), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnAllowListItemInitializerChanged((bool?)e.OldValue, (bool?)e.NewValue)));
			AllowInstanceInitializerProperty = DependencyPropertyManager.Register("AllowInstanceInitializer", typeof(bool?), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(null, (d, e) => ((PropertyGridControl)d).OnAllowInstanceInitializerChanged((bool?)e.OldValue, (bool?)e.NewValue)));
			ShowPropertiesProperty = DependencyPropertyManager.Register("ShowProperties", typeof(ShowPropertiesMode), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(ShowPropertiesMode.All,
					(d, e) => ((PropertyGridControl)d).OnShowPropertiesChanged((ShowPropertiesMode)e.OldValue, (ShowPropertiesMode)e.NewValue)));
			ShowMenuButtonInRowsProperty = DependencyPropertyManager.Register("ShowMenuButtonInRows", typeof(bool), typeof(PropertyGridControl), new FrameworkPropertyMetadata(true));
			SelectedPropertyPathProperty = DependencyPropertyManager.Register("SelectedPropertyPath", typeof(string), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None,
					(o, args) => ((PropertyGridControl)o).SelectedPropertyPathChanged((string)args.OldValue, (string)args.NewValue)));
			SelectedPropertyValuePropertyKey = DependencyPropertyManager.RegisterReadOnly("SelectedPropertyValue", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((PropertyGridControl)o).SelectedPropertyValueChanged(args.OldValue, args.NewValue)));
			SelectedPropertyValueProperty = SelectedPropertyValuePropertyKey.DependencyProperty;
			AllowExpandingProperty = DependencyPropertyManager.Register("AllowExpanding", typeof(AllowExpandingMode), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(AllowExpandingMode.Default, (d, e) => ((PropertyGridControl)d).OnAllowExpandingChanged((AllowExpandingMode)e.OldValue)));
			TrimDisplayTextProperty = DependencyPropertyManager.Register("TrimDisplayText", typeof(bool), typeof(PropertyGridControl),
				new FrameworkPropertyMetadata(true, (d, e) => ((PropertyGridControl)d).OnTrimDisplayTextChanged((bool)e.OldValue)));
			ShowMenuProperty = DependencyPropertyManager.Register("ShowMenu", typeof(ShowMenuMode), typeof(PropertyGridControl), new FrameworkPropertyMetadata(ShowMenuMode.OnMenuButtonClick));
			UseOptimizedEditorsProperty = DependencyProperty.Register("UseOptimizedEditors", typeof(bool), ownerType, new FrameworkPropertyMetadata(false,
				FrameworkPropertyMetadataOptions.None, (o, args) => ((PropertyGridControl)o).UseOptimizedEditorsChanged((bool)args.NewValue)));
			ReadOnlyProperty = DependencyPropertyManager.Register("ReadOnly", typeof(bool), typeof(PropertyGridControl), new FrameworkPropertyMetadata(false, (d, e) => ((PropertyGridControl)d).OnReadOnlyChanged()));
			AllowCommitOnValidationAttributeErrorProperty = DependencyPropertyManager.Register("AllowCommitOnValidationAttributeError", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
		}		
		protected virtual void UseOptimizedEditorsChanged(bool newValue) {
			useOptimizedEditor = newValue;
			DataView.Do(x => x.Invalidate(RowHandle.Root));
		}
		protected virtual void SelectedPropertyPathChanged(string oldValue, string newValue) {
			SelectionStrategy.SelectViaPath(newValue);
			RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
		}
		protected virtual void SelectedPropertyValueChanged(object oldValue, object newValue) {
		}
		protected virtual void OnPropertyDefinitionTemplateSelectorChanged(DependencyPropertyChangedEventArgs args) {
			ItemsAttachedBehaviorCore<PropertyGridControl, PropertyDefinitionBase>.OnItemsGeneratorTemplatePropertyChanged(this, args, PropertyDefinitionsAttachedBehaviorProperty);
		}
		protected virtual void OnPropertyDefinitionTemplateChanged(DependencyPropertyChangedEventArgs args) {
			ItemsAttachedBehaviorCore<PropertyGridControl, PropertyDefinitionBase>.OnItemsGeneratorTemplatePropertyChanged(this, args, PropertyDefinitionsAttachedBehaviorProperty);
		}
		protected virtual void OnPropertyDefinitionStyleChanged(DependencyPropertyChangedEventArgs args) {
			ItemsAttachedBehaviorCore<PropertyGridControl, PropertyDefinitionBase>.OnItemsGeneratorTemplatePropertyChanged(this, args, PropertyDefinitionsAttachedBehaviorProperty);
		}
		protected virtual void OnPropertyDefinitionsSourceChanged(DependencyPropertyChangedEventArgs args) {
			PropertyBuilder.BeginInit();
			ItemsAttachedBehaviorCore<PropertyGridControl, PropertyDefinitionBase>.OnItemsSourcePropertyChanged(this,
			args,
			PropertyDefinitionsAttachedBehaviorProperty,
			PropertyDefinitionTemplateProperty,
			PropertyDefinitionTemplateSelectorProperty,
			PropertyDefinitionStyleProperty,
			propertyGrid => propertyGrid.PropertyDefinitions,
			propertyGrid => new PropertyDefinition());
			PropertyBuilder.EndInit();
		}
		Lazy<IComparer<string>> stringComparer;
		IComparer<string> StringComparer { get { return stringComparer.Value; } }
		public object SelectedPropertyValue {
			get { return GetValue(SelectedPropertyValueProperty); }
			internal set { SetValue(SelectedPropertyValuePropertyKey, value); }
		}
		public string SelectedPropertyPath {
			get { return (string)GetValue(SelectedPropertyPathProperty); }
			set { SetValue(SelectedPropertyPathProperty, value); }
		}
		public IEnumerable SelectedObjects {
			get { return (IEnumerable)GetValue(SelectedObjectsProperty); }
			set { SetValue(SelectedObjectsProperty, value); }
		}
		public object SelectedObject {
			get { return GetValue(SelectedObjectProperty); }
			set { SetValue(SelectedObjectProperty, value); }
		}
		public bool ShowCategories {
			get { return (bool)GetValue(ShowCategoriesProperty); }
			set { SetValue(ShowCategoriesProperty, value); }
		}
		readonly CustomizationActionsField menuCustomizations;
		public IList<IControllerAction> MenuCustomizations { get { return menuCustomizations.Value; } }
		protected internal void ExecuteMenuCustomizations(DependencyObject context) { menuCustomizations.Execute(context); }
		public PropertyDefinitionCollection PropertyDefinitions {
			get { return (PropertyDefinitionCollection)GetValue(PropertyDefinitionsProperty); }
			set { SetValue(PropertyDefinitionsProperty, value); }
		}
		public bool ExpandCategoriesWhenSelectedObjectChanged {
			get { return (bool)GetValue(ExpandCategoriesWhenSelectedObjectChangedProperty); }
			set { SetValue(ExpandCategoriesWhenSelectedObjectChangedProperty, value); }
		}
		public IEnumerable PropertyDefinitionsSource {
			get { return (IEnumerable)GetValue(PropertyDefinitionsSourceProperty); }
			set { SetValue(PropertyDefinitionsSourceProperty, value); }
		}
		public Style PropertyDefinitionStyle {
			get { return (Style)GetValue(PropertyDefinitionStyleProperty); }
			set { SetValue(PropertyDefinitionStyleProperty, value); }
		}
		public DataTemplate PropertyDefinitionTemplate {
			get { return (DataTemplate)GetValue(PropertyDefinitionTemplateProperty); }
			set { SetValue(PropertyDefinitionTemplateProperty, value); }
		}
		public DataTemplateSelector PropertyDefinitionTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PropertyDefinitionTemplateSelectorProperty); }
			set { SetValue(PropertyDefinitionTemplateSelectorProperty, value); }
		}
		[TypeConverter(typeof(FilterCriteriaConverter))]
		public CriteriaOperator FilterCriteria {
			get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); }
			set { SetValue(FilterCriteriaProperty, value); }
		}
		[TypeConverter(typeof(FilterCriteriaConverter))]
		public CriteriaOperator UserFilterCriteria {
			get { return (CriteriaOperator)GetValue(UserFilterCriteriaProperty); }
			set { SetValue(UserFilterCriteriaProperty, value); }
		}
		public ShowPropertiesMode ShowProperties {
			get { return (ShowPropertiesMode)GetValue(ShowPropertiesProperty); }
			set { SetValue(ShowPropertiesProperty, value); }
		}
		public bool ShowMenuButtonInRows {
			get { return (bool)GetValue(ShowMenuButtonInRowsProperty); }
			set { SetValue(ShowMenuButtonInRowsProperty, value); }
		}
		public bool ShowToolPanel {
			get { return (bool)GetValue(ShowToolPanelProperty); }
			set { SetValue(ShowToolPanelProperty, value); }
		}
		public bool ShowSearchBox {
			get { return (bool)GetValue(ShowSearchBoxProperty); }
			set { SetValue(ShowSearchBoxProperty, value); }
		}
		public AllowExpandingMode AllowExpanding {
			get { return (AllowExpandingMode)GetValue(AllowExpandingProperty); }
			set { SetValue(AllowExpandingProperty, value); }
		}
		public bool TrimDisplayText {
			get { return (bool)GetValue(TrimDisplayTextProperty); }
			set { SetValue(TrimDisplayTextProperty, value); }
		}
		public DescriptionLocation ShowDescriptionIn {
			get { return (DescriptionLocation)GetValue(ShowDescriptionInProperty); }
			set { SetValue(ShowDescriptionInProperty, value); }
		}
		public ShowMenuMode ShowMenu {
			get { return (ShowMenuMode)GetValue(ShowMenuProperty); }
			set { SetValue(ShowMenuProperty, value); }
		}
		public PropertyGridSortMode SortMode {
			get { return (PropertyGridSortMode)GetValue(SortModeProperty); }
			set { SetValue(SortModeProperty, value); }
		}
		public event PropertyGridEditorEventHandler ShownEditor {
			add { AddHandler(ShownEditorEvent, value); }
			remove { RemoveHandler(ShownEditorEvent, value); }
		}
		public event PropertyGridEditorEventHandler HiddenEditor {
			add { AddHandler(HiddenEditorEvent, value); }
			remove { RemoveHandler(HiddenEditorEvent, value); }
		}
		public event ShowingPropertyGridEditorEventHandler ShowingEditor {
			add { AddHandler(ShowingEditorEvent, value); }
			remove { RemoveHandler(ShowingEditorEvent, value); }
		}
		public event RoutedEventHandler SelectionChanged {
			add { AddHandler(SelectionChangedEvent, value); }
			remove { RemoveHandler(SelectionChangedEvent, value); }
		}
		public event CustomDisplayTextEventHandler CustomDisplayText {
			add { AddHandler(CustomDisplayTextEvent, value); }
			remove { RemoveHandler(CustomDisplayTextEvent, value); }
		}
		public event PropertyGridSortingEventHandler Sort {
			add { AddHandler(SortEvent, value); }
			remove { RemoveHandler(SortEvent, value); }
		}
		public event CellValueChangingEventHandler CellValueChanging {
			add { AddHandler(CellValueChangingEvent, value); }
			remove { RemoveHandler(CellValueChangingEvent, value); }
		}
		public event CellValueChangedEventHandler CellValueChanged {
			add { AddHandler(CellValueChangedEvent, value); }
			remove { RemoveHandler(CellValueChangedEvent, value); }
		}
		public event ValidateCellEventHandler ValidateCell {
			add { AddHandler(ValidateCellEvent, value); }
			remove { RemoveHandler(ValidateCellEvent, value); }
		}
		public event InvalidCellExceptionEventHandler InvalidCellException {
			add { AddHandler(InvalidCellExceptionEvent, value); }
			remove { RemoveHandler(InvalidCellExceptionEvent, value); }
		}
		public event CustomExpandEventHandler CustomExpand {
			add { AddHandler(CustomExpandEvent, value); }
			remove { RemoveHandler(CustomExpandEvent, value); }
		}
		public DataTemplate DescriptionTemplate {
			get { return (DataTemplate)GetValue(DescriptionTemplateProperty); }
			set { SetValue(DescriptionTemplateProperty, value); }
		}
		public DataTemplateSelector DescriptionTemplateSelector {
			get { return (DataTemplateSelector)GetValue(DescriptionTemplateSelectorProperty); }
			set { SetValue(DescriptionTemplateSelectorProperty, value); }
		}
		public Style DescriptionContainerStyle {
			get { return (Style)GetValue(DescriptionContainerStyleProperty); }
			set { SetValue(DescriptionContainerStyleProperty, value); }
		}
		public StyleSelector DescriptionContainerStyleSelector {
			get { return (StyleSelector)GetValue(DescriptionContainerStyleSelectorProperty); }
			set { SetValue(DescriptionContainerStyleSelectorProperty, value); }
		}
		public bool? UseCollectionEditor {
			get { return (bool?)GetValue(UseCollectionEditorProperty); }
			set { SetValue(UseCollectionEditorProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTemplateSelector ActualDescriptionTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualDescriptionTemplateSelectorProperty); }
			protected internal set { SetValue(ActualDescriptionTemplateSelectorPropertyKey, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public StyleSelector ActualDescriptionContainerStyleSelector {
			get { return (StyleSelector)GetValue(ActualDescriptionContainerStyleSelectorProperty); }
			protected internal set { SetValue(ActualDescriptionContainerStyleSelectorPropertyKey, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Style ActualDescriptionContainerStyle {
			get { return (Style)GetValue(ActualDescriptionContainerStyleProperty); }
			protected internal set { SetValue(ActualDescriptionContainerStylePropertyKey, value); }
		}
		public bool? AllowInstanceInitializer {
			get { return (bool?)GetValue(AllowInstanceInitializerProperty); }
			set { SetValue(AllowInstanceInitializerProperty, value); }
		}
		public bool? AllowListItemInitializer {
			get { return (bool?)GetValue(AllowListItemInitializerProperty); }
			set { SetValue(AllowListItemInitializerProperty, value); }
		}
		public GridLength HeaderColumnWidth {
			get { return (GridLength)GetValue(HeaderColumnWidthProperty); }
			set { SetValue(HeaderColumnWidthProperty, value); }
		}
		public double HeaderColumnMaxWidth {
			get { return (double)GetValue(HeaderColumnMaxWidthProperty); }
			set { SetValue(HeaderColumnMaxWidthProperty, value); }
		}
		public double HeaderColumnMinWidth {
			get { return (double)GetValue(HeaderColumnMinWidthProperty); }
			set { SetValue(HeaderColumnMinWidthProperty, value); }
		}
		public GridLength ValueColumnWidth {
			get { return (GridLength)GetValue(ValueColumnWidthProperty); }
			set { SetValue(ValueColumnWidthProperty, value); }
		}
		public double ValueColumnMinWidth {
			get { return (double)GetValue(ValueColumnMinWidthProperty); }
			set { SetValue(ValueColumnMinWidthProperty, value); }
		}
		public double ValueColumnMaxWidth {
			get { return (double)GetValue(ValueColumnMaxWidthProperty); }
			set { SetValue(ValueColumnMaxWidthProperty, value); }
		}
		public PropertyGridFilterMode FilterMode {
			get { return (PropertyGridFilterMode)GetValue(FilterModeProperty); }
			set { SetValue(FilterModeProperty, value); }
		}
		public bool HighlightNonDefaultValues {
			get { return (bool)GetValue(HighlightNonDefaultValuesProperty); }
			set { SetValue(HighlightNonDefaultValuesProperty, value); }
		}
		public bool UseOptimizedEditors {
			get { return (bool)GetValue(UseOptimizedEditorsProperty); }
			set { SetValue(UseOptimizedEditorsProperty, value); }
		}
		public bool ReadOnly {
			get { return (bool)GetValue(ReadOnlyProperty); }
			set { SetValue(ReadOnlyProperty, value); }
		}
		public bool AllowCommitOnValidationAttributeError {
			get { return (bool)GetValue(AllowCommitOnValidationAttributeErrorProperty); }
			set { SetValue(AllowCommitOnValidationAttributeErrorProperty, value); }
		}
		bool useOptimizedEditor = true;
		readonly SelectSearchControlCommand selectSearchControlCommand;
		readonly WeakList<EventHandler> filterModeChangedHandlers = new WeakList<EventHandler>();
		protected internal RowDataGenerator RowDataGenerator { get; private set; }
		protected internal DataController DataController { get; private set; }
		protected internal SelectionStrategy SelectionStrategy { get; private set; }
		protected internal NavigationManager NavigationManager { get; private set; }
		protected internal PropertyBuilder PropertyBuilder { get; private set; }
		protected internal PropertyGridView View { get; private set; }
		protected internal ContentPresenter ViewPresenter { get; private set; }
		protected internal PropertyGridViewKindSelectorControl ViewKindSelector { get; private set; }
		protected internal PropertyGridSearchControl SearchControl { get; private set; }
		protected internal PropertyDescriptionPresenterControl DescriptionPresenter { get; private set; }
		protected internal DataViewBase DataView { get; private set; }
		protected internal virtual bool IsLockedBySelectionChanging { get { return SelectionStrategy.IsLockedBySelectionChanging; } }
		protected internal bool ActualUseOptimizedEditor { get { return useOptimizedEditor; } }
		protected internal event EventHandler FilterModeChanged {
			add { filterModeChangedHandlers.Add(value); }
			remove { filterModeChangedHandlers.Remove(value); }
		}
		protected override IEnumerator LogicalChildren {
			get { return new MergedEnumerator<object>(base.LogicalChildren.OfType<object>(), new SingleElementEnumerator<object>(PropertyBuilder)); }
		}
		protected internal PropertyDefinitionBase RootPropertyDefinition { get; private set; }
		public PropertyGridControl() {
			View = InitializeView();
			View.BeginInit();
			selectSearchControlCommand = new SelectSearchControlCommand(this);
			SelectionStrategy = new SelectionStrategy(this);
			NavigationManager = new NavigationManager(this);
			DataController = new DataController() { VisualClient = View };
			PropertyBuilder = new PropertyBuilder(this);
			RowDataGenerator = new RowDataGenerator();
			RowDataGenerator.BeginInit();
			RowDataGenerator.PropertyBuilder = PropertyBuilder;
			RowDataGenerator.View = View;
			UpdateDataViewByShowCategories();
			RowDataGenerator.EndInit();
			RootPropertyDefinition = new RootPropertyDefinition();
			PropertyDefinitions = new PropertyDefinitionCollection(RootPropertyDefinition);
			menuCustomizations = new CustomizationActionsField(this, true);
			InitializeInputBindings();
			AddLogicalChild(PropertyBuilder);
			PropertyGridHelper.SetPropertyGrid(this, this);
			View.EndInit();
			this.stringComparer = new Lazy<IComparer<string>>(() => CreateStringComparer());
		}
		#region API
		public virtual void Expand(string selectedPropertyPath) {
			RowDataGenerator.UpdateExpandAsync(selectedPropertyPath);
		}
		public virtual void Collapse(string selectedPropertyPath) {
			RowDataGenerator.UpdateCollapseAsync(selectedPropertyPath);
		}
		public object GetRowValueByRowPath(string propertyPath) {
			return DataView.GetValue(DataView.GetHandleByFieldName(propertyPath));
		}
		public Exception SetRowValueByRowPath(string propertyPath, object value) {
			return DataView.SetValue(DataView.GetHandleByFieldName(propertyPath), value);
		}
		public void ScrollIntoView(string path) {
			RowDataGenerator.ScrollToAsync(path);
		}		
		public void ShowEditor(bool selectAll = false) {
			View.ShowEditor(selectAll);
		}
		public void HideEditor() {
			View.HideEditor();
		}		
		public void CloseEditor() {
			View.CloseEditor();
		}		
		public void PostEditor() {
			View.PostEditor();
		}
		#endregion
		protected internal virtual void InitializeInputBindings() {
			InputBindings.Add(new InputBinding(selectSearchControlCommand, new KeyGesture(Key.F, ModifierKeys.Control)));
		}
		protected internal virtual PropertyGridView InitializeView() {
			PropertyGridView view = CreatePropertyGridView();
			view.PropertyGrid = this;			
			return view;
		}
		protected virtual PropertyGridView CreatePropertyGridView() {
			return new PropertyGridView();
		}
		protected internal virtual bool OnShowCategoriesChanging(bool newValue) {
			if (View == null || View.CheckCanNavigate())
				return newValue;
			return ShowCategories;
		}
		protected virtual void OnShowCategoriesChanged(bool oldValue) {
			UpdateDataViewByShowCategories();
		}
		protected virtual void OnPropertyDefinitionsChanged(PropertyDefinitionCollection oldValue) {
			PropertyBuilder.BeginInit();
			PropertyBuilder.Definitions = PropertyDefinitions;
			PropertyBuilder.EndInit();
		}
		protected virtual PropertyDefinitionCollection CoercePropertyDefinitions(PropertyDefinitionCollection e) {
			return e ?? new PropertyDefinitionCollection(RootPropertyDefinition);
		}
		protected virtual void SelectedObjectsChanged(IEnumerable oldValue) {
				(oldValue as INotifyCollectionChanged).Do(x => x.CollectionChanged -= SelectedObjectsCollectionChanged);
				(SelectedObjects as INotifyCollectionChanged).Do(x => x.CollectionChanged += SelectedObjectsCollectionChanged);
				ProcessChangeSelectedObjects();
		}
		void ProcessChangeSelectedObjects() {
			IEnumerable<object> selectedObjects = SelectedObjects.Return(x => x.Cast<object>(), () => null);
			DataController.IsMultiSource = selectedObjects != null && 1 < selectedObjects.Count();
			DataController.Source = DataController.IsMultiSource ? selectedObjects.ToArray() : selectedObjects.Return(x => x.FirstOrDefault(), () => null);
		}
		void SelectedObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ProcessChangeSelectedObjects();
		}		
		protected virtual void OnSelectedObjectChanged(object oldValue, object newValue) {
			DataController.Source = newValue;
		}
		protected virtual void OnAllowExpandingChanged(AllowExpandingMode oldValue) {
			InvalidateData();
		}
		protected virtual void OnTrimDisplayTextChanged(bool oldValue) {
			InvalidateData();
		}
		protected virtual void OnFilterModeChanged(PropertyGridFilterMode oldValue) {
			foreach (EventHandler e in filterModeChangedHandlers)
				e(this, new EventArgs());
			InvalidateData();
		}
		protected virtual void OnSortModeChanged(PropertyGridSortMode oldValue, PropertyGridSortMode newValue) {
			InvalidateData();
		}
		protected virtual internal bool CanShowEditor(CellEditor cellEditor) {
			if (RowDataGenerator.With(x => x.ItemsSource).Return(x => x.AsyncUpdatePending, () => false))
				return false;
			return cellEditor.With(x => x.RowData).With(x => x.Handle).Return(RaiseShowingEditorEvent, () => false);
		}
		protected internal virtual bool RaiseShowingEditorEvent(RowHandle handle) {
			return !new ShowingPropertyGridEditorEventArgs(handle, RowDataGenerator) { RoutedEvent = ShowingEditorEvent }.Do(RaiseEvent).Cancel;
		}
		protected internal virtual void RaiseShownEditorEvent(RowHandle handle, IBaseEdit edit) {
			RaiseEvent(new PropertyGridEditorEventArgs(handle, RowDataGenerator, edit) { RoutedEvent = ShownEditorEvent });
		}
		protected internal virtual void RaiseHiddenEditorEvent(RowHandle handle, IBaseEdit edit) {
			RaiseEvent(new PropertyGridEditorEventArgs(handle, RowDataGenerator, edit) { RoutedEvent = HiddenEditorEvent });
		}
		protected internal virtual void RaiseSortEvent(PropertyGridSortingEventArgs args, PropertyGridSortMode sortMode) {
			sortMode = sortMode == PropertyGridSortMode.Unspecified ? sortMode = (SortMode == PropertyGridSortMode.Unspecified ? PropertyGridSortMode.NoSort : SortMode) : sortMode;
			switch (sortMode) {
				case PropertyGridSortMode.Ascending:
					args.ResultCollection = args.SourceCollection.OrderBy(x => x.Header ?? "", StringComparer);
					return;
				case PropertyGridSortMode.Descending:
					args.ResultCollection = args.SourceCollection.OrderByDescending(x => x.Header ?? "", StringComparer);
					return;
				case PropertyGridSortMode.Definitions:
					args.ResultCollection = args.SourceCollection.OrderBy(x => PropertyDefinitionBase.GetIndexInCollection(x.Definition));
					break;
				case PropertyGridSortMode.Custom:
					args.RoutedEvent = SortEvent;
					this.RaiseEvent(args);
					return;
			}
		}
		protected virtual IComparer<string> CreateStringComparer() {
			return new NaturalStringComparer();
		}
		protected internal virtual void RaiseCellValueChanged(RowDataBase data, object oldValue, object newValue) {
			if (data == null || data.FullPath == null || data.RowDataGenerator == null || data.IsDirty)
				return;
			this.RaiseEvent(new CellValueChangedEventArgs(DataView.GetHandleByFieldName(data.FullPath), data.RowDataGenerator, oldValue, newValue) { RoutedEvent = CellValueChangedEvent });
		}
		protected internal virtual bool RaiseCellValueChanging(RowDataBase data, object oldValue, object newValue) {
			if (data == null || data.FullPath == null || data.RowDataGenerator == null || data.IsDirty)
				return true;
			CellValueChangingEventArgs args = new CellValueChangingEventArgs(DataView.GetHandleByFieldName(data.FullPath), data.RowDataGenerator, oldValue, newValue) { RoutedEvent = CellValueChangingEvent };
			this.RaiseEvent(args);
			return !args.Cancel;
		}
		protected internal virtual Exception RaiseValidateCell(RowDataBase data, object oldValue, object newValue) {
			if (data == null || data.FullPath == null || data.RowDataGenerator == null || data.IsDirty)
				return null;
			ValidateCellEventArgs args = new ValidateCellEventArgs(DataView.GetHandleByFieldName(data.FullPath), data.RowDataGenerator, oldValue, newValue) { RoutedEvent = ValidateCellEvent };
			this.RaiseEvent(args);
			return args.ValidationException;
		}
		protected internal virtual string RaiseCustomDisplayText(RowDataBase data, string displayText) {
			if (data == null || data.FullPath == null || data.RowDataGenerator == null || data.IsDirty)
				return displayText;
			CustomDisplayTextEventArgs args = new CustomDisplayTextEventArgs(DataView.GetHandleByFieldName(data.FullPath), data.RowDataGenerator, displayText) { RoutedEvent = CustomDisplayTextEvent };
			this.RaiseEvent(args);
			return args.Handled ? args.DisplayText : displayText;
		}
		protected internal bool? RaiseCustomExpand(RowHandle handle) {
			if (handle == null || handle.IsRoot || handle.IsInvalid)
				return true;
			var args = new CustomExpandEventArgs(handle, DataView, RowDataGenerator) { RoutedEvent = CustomExpandEvent };
			this.RaiseEvent(args);
			return args.ValueAssigned ? (bool?)args.IsExpanded : null;
		}
		protected internal string RaiseInvalidCellException(RowDataBase data, Exception exception) {
			if (data == null || data.FullPath == null || data.RowDataGenerator == null || data.IsDirty || exception == null)
				return exception == null ? null : exception.Message;
			var args = new InvalidCellExceptionEventArgs(DataView.GetHandleByFieldName(data.FullPath), data.RowDataGenerator, exception) { RoutedEvent = InvalidCellExceptionEvent };
			this.RaiseEvent(args);
			return args.Message;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (ViewPresenter != null)
				ViewPresenter.Content = null;
			ViewPresenter = GetTemplateChild("PART_ViewPresenter") as ContentPresenter;
			if (ViewKindSelector != null) {
				ViewKindSelector.PreviewKeyDown -= ViewKindSelector_PreviewKeyDown;
			}
			ViewKindSelector = GetTemplateChild("PART_Selector") as PropertyGridViewKindSelectorControl;
			if (ViewKindSelector != null) {
				ViewKindSelector.PreviewKeyDown += ViewKindSelector_PreviewKeyDown;
			}
			if (ViewPresenter != null)
				ViewPresenter.Content = View;
			DescriptionPresenter = GetTemplateChild("PART_DescriptionPresenter") as PropertyDescriptionPresenterControl;
			IList<String> mru = null;
			if (SearchControl != null) {
				SearchControl.PreviewKeyDown -= SearchControl_PreviewKeyDown;
				mru = SearchControl.MRU;
			}
			SearchControl = GetTemplateChild("PART_SearchControl") as PropertyGridSearchControl;
			if (SearchControl != null) {
				SearchControl.PreviewKeyDown += SearchControl_PreviewKeyDown;
				mru.Do(x => x.ForEach(el => SearchControl.MRU.Add(el)));
			}
			UpdateActualDescriptionContainerStyleSelector();
			UpdateActualDescriptionTemplateSelector();
		}
		void ViewKindSelector_PreviewKeyDown(object sender, KeyEventArgs e) {
			this.View.CellEditorOwner.ProcessKeyDown(e);
		}
		void SearchControl_PreviewKeyDown(object sender, KeyEventArgs e) {
			if (!SearchControl.GetIsPopupOpened() && e.Key != Key.Home && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.End)
				this.View.CellEditorOwner.ProcessKeyDown(e);
		}
		bool oldFocusWithin = false;
		protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnPreviewGotKeyboardFocus(e);
			oldFocusWithin = IsKeyboardFocusWithin;
			Dispatcher.BeginInvoke(new Action(() => oldFocusWithin = false));
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			if (oldFocusWithin)
				return;
			View.Focus();			
		}
		public override void BeginInit() {
			View.BeginInit();
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			View.EndInit();
		}
		static void ProcessPreviewMouseUp(object sender, MouseButtonEventArgs e) {
			PropertyGridControl pGrid = sender as PropertyGridControl;
			if (pGrid != null && pGrid.View != null)
				pGrid.View.ProcessMouseButtonUp(e);
		}
		static void ProcessPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			PropertyGridControl pGrid = sender as PropertyGridControl;
			if (pGrid != null && pGrid.View != null)
				pGrid.View.ProcessMouseLeftButtonDown(e);
		}
		protected internal virtual void UpdateDataViewByShowCategories() {
			DataView = ShowCategories ? (DataViewBase)new CategoryDataView(DataController) : (DataViewBase)new DataView(DataController);
			UpdateDataViewFilterCriteria();
			RowDataGenerator.DataView = DataView;
			InvalidateData();
		}
		protected virtual void OnUserFilterCriteriaChanged(CriteriaOperator oldValue, CriteriaOperator newValue) {
			UpdateDataViewFilterCriteria();
		}
		protected virtual void OnShowPropertiesChanged(ShowPropertiesMode oldValue, ShowPropertiesMode newValue) {
			UpdateDataViewFilterCriteria();
		}
		protected virtual void OnFilerCriteriaChanged(CriteriaOperator oldValue, CriteriaOperator newValue) {
			UpdateDataViewFilterCriteria();
		}
		protected internal virtual void UpdateDataViewFilterCriteria() {
			if (DataView == null)
				return;
			GroupOperator combinedCriteria = new GroupOperator(GroupOperatorType.And, CriteriaOperator.Parse("[IsVisible] = true"));
			var operands = combinedCriteria.Operands;
			if (ShowProperties == ShowPropertiesMode.WithPropertyDefinitions) {
				operands.Add(CriteriaOperator.Parse("[HasPropertyDefinition] = true"));
			}
			if (!CriteriaOperator.Equals(FilterCriteria, null)) {
				operands.Add(FilterCriteria);
			}
			DataView.FilterCriteria = combinedCriteria;
			DataView.SearchCriteria = UserFilterCriteria;
		}
		protected virtual void OnHighlightNonDefaultValuesChanged(bool newValue) {
			InvalidateData();
		}
		protected virtual void OnDescriptionContainerStyleSelectorChanged(StyleSelector oldValue, StyleSelector newValue) {
			UpdateActualDescriptionContainerStyleSelector();
		}
		protected virtual void OnDescriptionContainerStyleChanged(Style oldValue, Style newValue) {
			UpdateActualDescriptionContainerStyleSelector();
		}
		protected virtual void OnDescriptionTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) {
			UpdateActualDescriptionTemplateSelector();
		}
		protected virtual void OnDescriptionTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			UpdateActualDescriptionTemplateSelector();
		}
		protected virtual void OnActualDescriptionContainerStyleSelectorChanged(StyleSelector oldValue, StyleSelector newValue) {
			var selectedItem = View == null ? null : View.SelectedItem as RowDataBase;
			ActualDescriptionContainerStyle = ActualDescriptionContainerStyleSelector == null ? null : ActualDescriptionContainerStyleSelector.SelectStyle(selectedItem.With(x => x.Description), DescriptionPresenter);
		}
		protected internal void UpdateActualDescriptionContainerStyleSelector() {
			var selectedItemDefinition = (View.With(x => x.SelectedItem) as RowDataBase).With(x => x.Definition);
			ActualDescriptionContainerStyleSelector = new PrioritizedStyleSelector() {
				DefaultStyle = DescriptionContainerStyle,
				DefaultStyleSelector = DescriptionContainerStyleSelector,
				CustomStyle = selectedItemDefinition.With(x => x.DescriptionContainerStyle),
				CustomStyleSelector = selectedItemDefinition.With(x => x.DescriptionContainerStyleSelector)
			};
		}
		protected internal void UpdateActualDescriptionTemplateSelector() {
			var selectedItemDefinition = (View.With(x => x.SelectedItem) as RowDataBase).With(x => x.Definition);
			ActualDescriptionTemplateSelector = new PrioritizedDataTemplateSelector() {
				DefaultTemplate = DescriptionTemplate,
				DefaultTemplateSelector = DescriptionTemplateSelector,
				CustomTemplate = selectedItemDefinition.With(x => x.DescriptionTemplate),
				CustomTemplateSelector = selectedItemDefinition.With(x => x.DescriptionTemplateSelector)
			};
		}
		public ObservableCollection<string> GetAvailableColumns() {
			return new ObservableCollection<string>(RowInfo.Columns);
		}
		protected virtual void OnUseCollectionEditorChanged(bool? oldValue, bool? newValue) {
			InvalidateData();
		}
		protected virtual void OnAllowInstanceInitializerChanged(bool? oldValue, bool? newValue) {
			InvalidateData();
		}
		protected virtual void OnAllowListItemInitializerChanged(bool? oldValue, bool? newValue) {
			InvalidateData();
		}
		protected virtual void OnReadOnlyChanged() {
			InvalidateData();
		}
		public virtual void InvalidateData() {
			DataView.Invalidate(RowHandle.Root);
		}
		public virtual void UpdateData() {
			DataView.Update();
		}
		protected internal virtual void UpdateSelection(RowDataBase rowData) {
			SelectionStrategy.SelectViaPath(rowData.Return(x => x.Handle, () => RowHandle.Invalid).With(DataView.GetFieldNameByHandle), false);
			UpdateActualDescriptionContainerStyleSelector();
			UpdateActualDescriptionTemplateSelector();
		}
		#region INavigationSupport Members
		bool INavigationSupport.ProcessNavigation(NavigationDirection direction) {
			return false;
		}
		IEnumerable<FrameworkElement> INavigationSupport.GetChildren() {
			List<FrameworkElement> children = new List<FrameworkElement>();
			if (ShowToolPanel)
				children.Add(ViewKindSelector);
			if (ShowSearchBox)
				children.Add(SearchControl);
			children.Add(View);
			return children;
		}
		FrameworkElement INavigationSupport.GetParent() {
			return null;
		}
		bool INavigationSupport.GetSkipNavigation() {
			return true;
		}
		bool INavigationSupport.GetUseLinearNavigation() {
			return false;
		}
		#endregion
	}
}

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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Core;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid.Automation;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Utils.Serializing.Helpers;
using XpfFiltering = DevExpress.Xpf.Editors.Filtering;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.DataAnnotations;
#if !SL
using System.Data;
using DevExpress.Entity.Model;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
#else
using System.Windows.Media;
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Core.WPFCompatibility;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using FrameworkContentElement = System.Windows.DependencyObject;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
using DependencyObjectExtensions = DevExpress.Xpf.Core.Native.DependencyObjectExtensions;
using DevExpress.Entity.Model;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class GridDataControlBase : DataControlBase {
		protected GridDataControlBase(IDataControlOriginationElement dataControlOriginationElement)
			: base(dataControlOriginationElement) { }
	}
	public class DataControlFilteredComponent : XpfFiltering.IFilteredComponent {
		DataControlBase dataControl;
		public DataControlFilteredComponent(DataControlBase dataControl) {
			this.dataControl = dataControl;
		}
		public IEnumerable<FilterColumn> CreateFilterColumnCollection() {
			List<FilterColumn> list = new List<FilterColumn>();
			foreach(ColumnBase gridColumn in dataControl.ColumnsCore) {
				if(gridColumn.ActualAllowColumnFiltering || gridColumn.IsFiltered || dataControl.DesignTimeAdorner.ForceAllowUseColumnInFilterControl)
					list.Add(dataControl.GetFilterColumnFromGridColumn(gridColumn));
			}
			return list;
		}
		EventHandler propertiesChanged;
		event EventHandler IFilteredComponentBase.PropertiesChanged {
			add { propertiesChanged += value; }
			remove { propertiesChanged -= value; }
		}
		EventHandler rowFilterChanged;
		event EventHandler IFilteredComponentBase.RowFilterChanged {
			add { rowFilterChanged += value; }
			remove { rowFilterChanged -= value; }
		}
		CriteriaOperator IFilteredComponentBase.RowCriteria { get; set; }
		internal void RaiseFilterColumnsChanged() {
			if(propertiesChanged != null)
				propertiesChanged(this, new EventArgs());
		}
	}
	[DefaultBindingProperty("ItemsSource"), ComplexBindingProperties("ItemsSource")]
	public abstract class DataControlBase : Control, IValidationAttributeOwner, INotificationManager, INotifyPropertyChanged, IDXFilterable, IDXDomainDataSourceSupport, IDispalyMemberBindingClient, IEventArgsConverterSource, IWeakEventListener  {
		#region dependency properties
		internal static readonly ColumnSortOrder defaultColumnSortOrder = ColumnSortOrder.Ascending;
		public const int InvalidRowHandle = DevExpress.Data.DataController.InvalidRow;
		public const int InvalidRowIndex = -1;
		public const int AutoFilterRowHandle = -999997;
		public const int NewItemRowHandle = InvalidRowHandle + 1;
		public static bool AllowInfiniteGridSize = false;
		internal ObservableCollectionCore<CriteriaOperatorInfo> MRUFiltersInternal { get; private set; }
		[Browsable(false), XtraSerializableProperty(true, false, true, SerializationOrders.GridControl_MRUFilters), GridUIProperty]
		public ReadOnlyObservableCollection<CriteriaOperatorInfo> MRUFilters { get; private set; }
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty CurrentViewProperty;
		public static readonly DependencyProperty CurrentViewChangedListenerProperty;
		public static readonly DependencyProperty ActiveViewProperty;
		static readonly DependencyPropertyKey ActiveFilterInfoPropertyKey;
		public static readonly DependencyProperty ActiveFilterInfoProperty;
		public static readonly DependencyProperty AllowMRUFilterListProperty;
		public static readonly DependencyProperty MRUFilterListCountProperty;
		public static readonly DependencyProperty AllowColumnMRUFilterListProperty;
		public static readonly DependencyProperty MRUColumnFilterListCountProperty;
		internal const string FilterCriteriaPropertyName = "FilterCriteria";
		public static readonly DependencyProperty FilterCriteriaProperty;
		public static readonly DependencyProperty FixedFilterProperty;
		public static readonly DependencyProperty FilterStringProperty;
		public static readonly DependencyProperty IsFilterEnabledProperty;
		public static readonly DependencyProperty AutoPopulateColumnsProperty;
		public static readonly DependencyProperty AutoGenerateColumnsProperty;
		public static readonly DependencyProperty EnableSmartColumnsGenerationProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		public static readonly DependencyProperty DefaultSortingProperty;
		public static readonly RoutedEvent ItemsSourceChangedEvent;
		public static readonly RoutedEvent FilterChangedEvent;
		public static readonly RoutedEvent ColumnsPopulatedEvent;
		public static readonly RoutedEvent AutoGeneratingColumnEvent;
		public static readonly RoutedEvent AutoGeneratedColumnsEvent;
		public static readonly RoutedEvent CustomUniqueValuesEvent;
		public static readonly DependencyProperty ShowLoadingPanelProperty;
		public static readonly DependencyProperty DesignTimeShowSampleDataProperty;
		public static readonly DependencyProperty DesignTimeUseDistinctSampleValuesProperty;
		public static readonly DependencyProperty DesignTimeDataSourceRowCountProperty;
		public static readonly DependencyProperty DesignTimeDataObjectTypeProperty;
		public static readonly System.Windows.DependencyProperty ColumnGeneratorStyleProperty;
		public static readonly System.Windows.DependencyProperty ColumnGeneratorTemplateProperty;
		public static readonly System.Windows.DependencyProperty ColumnGeneratorTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly System.Windows.DependencyProperty ColumnsItemsAttachedBehaviorProperty;
		public static readonly System.Windows.DependencyProperty ColumnsSourceProperty;
		public static readonly System.Windows.DependencyProperty TotalSummaryGeneratorTemplateProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly System.Windows.DependencyProperty TotalSummaryItemsAttachedBehaviorProperty;
		public static readonly System.Windows.DependencyProperty TotalSummarySourceProperty;
		static readonly DependencyPropertyKey OwnerDetailDescriptorPropertyKey;
		public static readonly DependencyProperty OwnerDetailDescriptorProperty;
		public static readonly DependencyProperty ShowAllTableValuesInCheckedFilterPopupProperty;
		public static readonly DependencyProperty ShowAllTableValuesInFilterPopupProperty;
		public static readonly DependencyProperty CurrentItemProperty;
		public static readonly DependencyProperty CurrentColumnProperty;
		public static readonly DependencyProperty CurrentCellValueProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty SelectedItemsProperty;
		public static readonly DependencyProperty SelectionModeProperty;
		public static readonly DependencyProperty AllowUpdateTwoWayBoundPropertiesOnSynchronizationProperty;
		public static readonly DependencyProperty ImplyNullLikeEmptyStringWhenFilteringProperty;
		public static readonly DependencyProperty UseFieldNameForSerializationProperty;
		public static readonly DependencyProperty ClipboardCopyModeProperty;
		public static readonly RoutedEvent PastingFromClipboardEvent;
		public static readonly RoutedEvent CurrentItemChangedEvent;
		public static readonly RoutedEvent SelectedItemChangedEvent;
		public static readonly RoutedEvent CurrentColumnChangedEvent;
		#endregion
		static DataControlBase() {
			Type ownerType = typeof(DataControlBase);
			ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(object), ownerType, new FrameworkPropertyMetadata(null, OnDataSourceChanged));
			CurrentViewProperty = DependencyPropertyManager.RegisterAttached("CurrentView", typeof(DataViewBase), ownerType, new PropertyMetadata(null, OnCurrentViewChanged));
			CurrentViewChangedListenerProperty = DependencyPropertyManager.RegisterAttached("CurrentViewChangedListener", typeof(INotifyCurrentViewChanged), ownerType, new PropertyMetadata(null, OnCurrentViewChangedListenerChanged));
			ActiveViewProperty = DependencyPropertyManager.RegisterAttached("ActiveView", typeof(DataViewBase), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			ActiveFilterInfoPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActiveFilterInfo", typeof(CriteriaOperatorInfo), ownerType, new FrameworkPropertyMetadata(null));
			ActiveFilterInfoProperty = ActiveFilterInfoPropertyKey.DependencyProperty;
			AllowMRUFilterListProperty = DependencyPropertyManager.Register("AllowMRUFilterList", typeof(bool), ownerType, new PropertyMetadata(true));
			MRUFilterListCountProperty = DependencyPropertyManager.Register("MRUFilterListCount", typeof(Int32), ownerType, new PropertyMetadata(10, new PropertyChangedCallback(OnMRUFilterListCountChanged)));
			AllowColumnMRUFilterListProperty = DependencyPropertyManager.Register("AllowColumnMRUFilterList", typeof(bool), ownerType, new PropertyMetadata(true));
			ShowAllTableValuesInFilterPopupProperty = DependencyPropertyManager.Register("ShowAllTableValuesInFilterPopup", typeof(bool), ownerType, new PropertyMetadata(false));
			ShowAllTableValuesInCheckedFilterPopupProperty = DependencyPropertyManager.Register("ShowAllTableValuesInCheckedFilterPopup", typeof(bool), ownerType, new PropertyMetadata(true));
			MRUColumnFilterListCountProperty = DependencyPropertyManager.Register("MRUColumnFilterListCount", typeof(int), ownerType, new PropertyMetadata(5));
			FilterCriteriaProperty = DependencyPropertyManager.Register(FilterCriteriaPropertyName, typeof(CriteriaOperator), ownerType, new PropertyMetadata(null, OnFilterCriteriaChanged, CoerceFilterCriteria));
			FixedFilterProperty = DependencyProperty.Register("FixedFilter", typeof(CriteriaOperator), ownerType, new PropertyMetadata(null, OnFixedFilterChanged, CoerceFixedFilter));
			FilterStringProperty = DependencyPropertyManager.Register("FilterString", typeof(string), ownerType, new PropertyMetadata(string.Empty, OnFilterStringChanged, CoerceFilterString));
			IsFilterEnabledProperty = DependencyPropertyManager.Register("IsFilterEnabled", typeof(bool), ownerType, new PropertyMetadata(true, OnIsFilterEnabledChanged, CoerceIsFilterEnabled));
			AutoPopulateColumnsProperty = DependencyPropertyManager.Register("AutoPopulateColumns", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((DataControlBase)d).OnAutoPopulateColumnsChanged()));
			AutoGenerateColumnsProperty = DependencyPropertyManager.Register("AutoGenerateColumns", typeof(AutoGenerateColumnsMode), ownerType, new FrameworkPropertyMetadata(AutoGenerateColumnsMode.None, (d, e) => ((DataControlBase)d).OnAutoGenerateColumnsChanged()));
			EnableSmartColumnsGenerationProperty = DependencyPropertyManager.Register("EnableSmartColumnsGeneration", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((DataControlBase)d).OnEnableSmartColumnsGenerationChanged()));
			ShowBorderProperty = DependencyPropertyManager.RegisterAttached("ShowBorder", typeof(bool), ownerType, new PropertyMetadata(true));
			ShowLoadingPanelProperty = DependencyPropertyManager.Register("ShowLoadingPanel", typeof(bool), ownerType, new PropertyMetadata(false));
			DefaultSortingProperty = DependencyPropertyManager.Register("DefaultSorting", typeof(string), ownerType, new PropertyMetadata(null, (d, e) => ((DataControlBase)d).OnDefaultSortingChanged()));
			ItemsSourceChangedEvent = EventManager.RegisterRoutedEvent("ItemsSourceChanged", RoutingStrategy.Direct, typeof(ItemsSourceChangedEventHandler), ownerType);
			FilterChangedEvent = EventManager.RegisterRoutedEvent("FilterChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			ColumnsPopulatedEvent = EventManager.RegisterRoutedEvent("ColumnsPopulated", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			AutoGeneratingColumnEvent = EventManager.RegisterRoutedEvent("AutoGeneratingColumn", RoutingStrategy.Direct, typeof(AutoGeneratingColumnEventHandler), ownerType);
			AutoGeneratedColumnsEvent = EventManager.RegisterRoutedEvent("AutoGeneratedColumns", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			CustomUniqueValuesEvent = EventManager.RegisterRoutedEvent("CustomUniqueValues", RoutingStrategy.Direct, typeof(CustomUniqueValuesEventHandler), ownerType);
			DesignTimeShowSampleDataProperty = DependencyPropertyManager.RegisterAttached("DesignTimeShowSampleData", typeof(bool), ownerType, new PropertyMetadata(true, OnDesignTimePropertyChanged));
			DesignTimeUseDistinctSampleValuesProperty = DependencyPropertyManager.RegisterAttached("DesignTimeUseDistinctSampleValues", typeof(bool), ownerType, new PropertyMetadata(true, OnDesignTimePropertyChanged));
			DesignTimeDataSourceRowCountProperty = DependencyPropertyManager.RegisterAttached("DesignTimeDataSourceRowCount", typeof(int), ownerType, new PropertyMetadata(2, OnDesignTimePropertyChanged, (d, baseValue) => ((DataControlBase)d).CoerceDesignTimeDataSourceRowCount((int)baseValue)));
			DesignTimeDataObjectTypeProperty = DependencyPropertyManager.RegisterAttached("DesignTimeDataObjectType", typeof(Type), ownerType, new PropertyMetadata(null, OnDesignTimePropertyChanged));
			ColumnGeneratorStyleProperty = DependencyProperty.Register("ColumnGeneratorStyle", typeof(Style), ownerType, new System.Windows.PropertyMetadata(OnColumnsItemsGeneratorTemplatePropertyChanged));
			ColumnGeneratorTemplateProperty = DependencyProperty.Register("ColumnGeneratorTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(OnColumnsItemsGeneratorTemplatePropertyChanged));
			ColumnGeneratorTemplateSelectorProperty = DependencyProperty.Register("ColumnGeneratorTemplateSelector", typeof(DataTemplateSelector), ownerType, new System.Windows.PropertyMetadata(OnColumnsItemsGeneratorTemplatePropertyChanged));
			ColumnsItemsAttachedBehaviorProperty = DependencyProperty.Register("ColumnsItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<DataControlBase, ColumnBase>), ownerType, new System.Windows.PropertyMetadata(null));
			ColumnsSourceProperty = DependencyProperty.Register("ColumnsSource", typeof(IEnumerable), ownerType, new System.Windows.PropertyMetadata((d, e) => ItemsAttachedBehaviorCore<DataControlBase, ColumnBase>.OnItemsSourcePropertyChanged(d, e, ColumnsItemsAttachedBehaviorProperty, ColumnGeneratorTemplateProperty, ColumnGeneratorTemplateSelectorProperty, ColumnGeneratorStyleProperty, grid => grid.ColumnsCore, grid => grid.CreateColumn())));
			TotalSummaryGeneratorTemplateProperty = DependencyProperty.Register("TotalSummaryGeneratorTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(OnTotalSummaryItemsGeneratorTemplatePropertyChanged));
			TotalSummaryItemsAttachedBehaviorProperty = DependencyProperty.Register("TotalSummaryItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<DataControlBase, SummaryItemBase>), ownerType, new System.Windows.PropertyMetadata(null));
			TotalSummarySourceProperty = DependencyProperty.Register("TotalSummarySource", typeof(IEnumerable), ownerType, new System.Windows.PropertyMetadata((d, e) => ItemsAttachedBehaviorCore<DataControlBase, SummaryItemBase>.OnItemsSourcePropertyChanged(d, e, TotalSummaryItemsAttachedBehaviorProperty, TotalSummaryGeneratorTemplateProperty, null, null, grid => grid.TotalSummaryCore, grid => grid.CreateSummaryItem())));
			OwnerDetailDescriptorPropertyKey = DependencyPropertyManager.RegisterReadOnly("OwnerDetailDescriptor", typeof(DetailDescriptorBase), ownerType, new PropertyMetadata(null));
			OwnerDetailDescriptorProperty = OwnerDetailDescriptorPropertyKey.DependencyProperty;
			CurrentItemProperty = DependencyPropertyManager.Register("CurrentItem", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((DataControlBase)d).OnCurrentItemChanged(e.OldValue, true), (d, e) => ((DataControlBase)d).CoerceCurrentItem(e)));
			CurrentCellValueProperty = DependencyPropertyManager.Register("CurrentCellValue", typeof(object), ownerType, new FrameworkPropertyMetadata(null));
			CurrentColumnProperty = DependencyPropertyManager.Register("CurrentColumn", typeof(ColumnBase), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DataControlBase)d).OnCurrentColumnChanged((GridColumnBase)e.OldValue, (GridColumnBase)e.NewValue), (d, e) => ((DataControlBase)d).CoerceCurrentColumn((GridColumnBase)e)));
			CurrentItemChangedEvent = EventManager.RegisterRoutedEvent("CurrentItemChanged", RoutingStrategy.Direct, typeof(CurrentItemChangedEventHandler), ownerType);
			SelectedItemChangedEvent = EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Direct, typeof(SelectedItemChangedEventHandler), ownerType);
			CurrentColumnChangedEvent = EventManager.RegisterRoutedEvent("CurrentColumnChanged", RoutingStrategy.Direct, typeof(CurrentColumnChangedEventHandler), ownerType);
			SelectedItemsProperty = DependencyPropertyManager.Register("SelectedItems", typeof(IList), ownerType, new PropertyMetadata(null, (d, e) => ((DataControlBase)d).OnSelectedItemsChanged((IList)e.OldValue)));
			SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((DataControlBase)d).OnSelectedItemChanged(e.OldValue)));
			SelectionModeProperty = DependencyPropertyManager.Register("SelectionMode", typeof(MultiSelectMode), ownerType, new PropertyMetadata(MultiSelectMode.None, (d, e) => ((DataControlBase)d).OnSelectionModeChanged((MultiSelectMode)e.OldValue)));
			AllowUpdateTwoWayBoundPropertiesOnSynchronizationProperty = DependencyPropertyManager.Register("AllowUpdateTwoWayBoundPropertiesOnSynchronization", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ImplyNullLikeEmptyStringWhenFilteringProperty = DependencyPropertyManager.Register("ImplyNullLikeEmptyStringWhenFiltering", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			UseFieldNameForSerializationProperty = DependencyPropertyManager.Register("UseFieldNameForSerialization", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((DataControlBase)d).OnUseFieldNameForSerializationChanged()));
			ClipboardCopyModeProperty = DependencyPropertyManager.Register("ClipboardCopyMode", typeof(ClipboardCopyMode), ownerType, new PropertyMetadata(ClipboardCopyMode.Default));
			PastingFromClipboardEvent = EventManager.RegisterRoutedEvent("PastingFromClipboard", RoutingStrategy.Direct, typeof(PastingFromClipboardEventHandler), ownerType);
			EventManager.RegisterClassHandler(ownerType, DXSerializer.ClearCollectionEvent, new XtraItemRoutedEventHandler((s, e) => ((DataControlBase)s).OnDeserializeClearCollection(e)));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.CreateCollectionItemEvent, new XtraCreateCollectionItemEventHandler((s, e) => ((DataControlBase)s).OnDeserializeCreateCollectionItem(e)));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.FindCollectionItemEvent, new XtraFindCollectionItemEventHandler((s, e) => ((DataControlBase)s).OnDeserializeFindCollectionItem(e)));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.StartDeserializingEvent, new StartDeserializingEventHandler((s, e) => ((DataControlBase)s).OnDeserializeStart(e)));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.EndDeserializingEvent, new EndDeserializingEventHandler((s, e) => ((DataControlBase)s).OnDeserializeEnd(e)));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.AllowPropertyEvent, new AllowPropertyEventHandler((s, e) => ((DataControlBase)s).OnDeserializeAllowPropertyInternal(e)));
			EventManager.RegisterClassHandler(ownerType, DXSerializer.CustomShouldSerializePropertyEvent, new CustomShouldSerializePropertyEventHandler((s, e) => ((DataControlBase)s).OnCustomShouldSerializeProperty(e)));
			CloneDetailHelper.RegisterKnownPropertyKeys(ownerType, OwnerDetailDescriptorPropertyKey);
		}
		#region static
		static void OnColumnsItemsGeneratorTemplatePropertyChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<DataControlBase, ColumnBase>.OnItemsGeneratorTemplatePropertyChanged(d, e, ColumnsItemsAttachedBehaviorProperty);
		}
		static void OnTotalSummaryItemsGeneratorTemplatePropertyChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<DataControlBase, SummaryItemBase>.OnItemsGeneratorTemplatePropertyChanged(d, e, TotalSummaryItemsAttachedBehaviorProperty);
		}
		static void OnDesignTimePropertyChanged(DependencyObject dObject, DependencyPropertyChangedEventArgs e) {
			DataControlBase control = (DataControlBase)dObject;
			control.OnDesignTimePropertyChanged();
		}
		protected static void OnDataSourceChanged(DependencyObject dObject, DependencyPropertyChangedEventArgs e) {
			DataControlBase control = (DataControlBase)dObject;
			control.OnItemsSourceChanged(e.OldValue, e.NewValue);
		}
		protected static object OnCoerceView(DependencyObject d, object baseValue) {
			return baseValue ?? ((DataControlBase)d).CreateDefaultView();
		}
		protected static void OnViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataControlBase)d).OnViewChanged((DataViewBase)e.OldValue, (DataViewBase)e.NewValue);
		}
		internal static ColumnSortOrder GetActualSortOrder(ColumnSortOrder columnSortOrder) {
			return columnSortOrder == ColumnSortOrder.None ? ColumnSortOrder.Ascending : columnSortOrder;
		}
		static void OnFilterStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataControlBase)d).OnFilterStringChanged();
		}
		static object CoerceFilterString(DependencyObject d, object value) {
			return ((DataControlBase)d).CoerceFilterString((string)value);
		}
		static void OnIsFilterEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataControlBase)d).OnIsFilterEnabledChanged();
		}
		static object CoerceIsFilterEnabled(DependencyObject d, object value) {
			return ((DataControlBase)d).CoerceIsFilterEnabled((bool)value);
		}
		static void OnFilterCriteriaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataControlBase)d).OnFilterCriteriaChanged();
		}
		static void OnFixedFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataControlBase)d).OnFixedFilterChanged();
		}
		static object CoerceFilterCriteria(DependencyObject d, object value) {
			return ((DataControlBase)d).CoerceFilterCriteria((CriteriaOperator)value);
		}
		static object CoerceFixedFilter(DependencyObject d, object value) {
			return ((DataControlBase)d).CoerceFixedFilter((CriteriaOperator)value);
		}
		static void OnMRUFilterListCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DataControlBase)d).MRUFilterListCountChanged();
		}
		void MRUFilterListCountChanged() {
			if(MRUFiltersInternal.Count > MRUFilterListCount) {
				for(int i = MRUFilterListCount; i < MRUFiltersInternal.Count; ++i) {
					MRUFiltersInternal.RemoveAt(MRUFilterListCount);
				}
			}
		}
		public static DataViewBase GetCurrentView(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (DataViewBase)element.GetValue(CurrentViewProperty);
		}
		public static void SetCurrentView(DependencyObject element, DataViewBase value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(CurrentViewProperty, value);
		}
		public static INotifyCurrentViewChanged GetCurrentViewChangedListener(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (INotifyCurrentViewChanged)element.GetValue(CurrentViewChangedListenerProperty);
		}
		public static DataViewBase GetActiveView(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (DataViewBase)element.GetValue(ActiveViewProperty);
		}
		public static void SetActiveView(DependencyObject element, DataViewBase value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ActiveViewProperty, value);
		}
		internal static void SetCurrentViewInternal(DependencyObject element, DataViewBase value) {
			SetActiveView(element, value);
			SetCurrentView(element, value);
		}
		public static void SetCurrentViewChangedListener(DependencyObject element, INotifyCurrentViewChanged value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(CurrentViewChangedListenerProperty, value);
		}
		static void OnCurrentViewChangedListenerChanged(DependencyObject dObject, DependencyPropertyChangedEventArgs e) {
			INotifyCurrentViewChanged notifyCurrentViewChanged = GetCurrentViewChangedListener(dObject);
			if(GetCurrentView(dObject) != null && notifyCurrentViewChanged != null)
				notifyCurrentViewChanged.OnCurrentViewChanged(dObject);
		}
		static void OnCurrentViewChanged(DependencyObject dObject, DependencyPropertyChangedEventArgs e) {
			INotifyCurrentViewChanged notifyCurrentViewChanged = dObject as INotifyCurrentViewChanged;
			if(notifyCurrentViewChanged != null) {
				notifyCurrentViewChanged.OnCurrentViewChanged(dObject);
			}
			else {
				notifyCurrentViewChanged = GetCurrentViewChangedListener(dObject);
				if(notifyCurrentViewChanged != null)
					notifyCurrentViewChanged.OnCurrentViewChanged(dObject);
			}
		}
		internal static T FindElementWithAttachedPropertyValue<T>(DependencyObject d, DependencyProperty attachedProperty) where T : class {
			DependencyObject rowElement = LayoutHelper.FindLayoutOrVisualParentObject(d, element => element.GetValue(attachedProperty) != null);
			return rowElement != null ? (T)rowElement.GetValue(attachedProperty) : null;
		}
		internal static DataViewBase FindCurrentView(DependencyObject d) {
			return FindElementWithAttachedPropertyValue<DataViewBase>(d, CurrentViewProperty);
		}
		public class GridSortInfoComparer : IEqualityComparer<GridSortInfo> {
			public bool Equals(GridSortInfo x, GridSortInfo y) {
				return x.FieldName == y.FieldName;
			}
			public int GetHashCode(GridSortInfo obj) {
				return obj.FieldName.GetHashCode();
			}
		}
		static List<ColumnBase> SetGroupSortInfoAndBuildGroupedColumns(IColumnCollection columns, IList<GridSortInfo> sortInfo, int groupCount) {
			List<ColumnBase> groupedColumnsList = new List<ColumnBase>();
			foreach(ColumnBase column in columns) {
				column.GroupIndexCore = -1;
				column.SortIndex = -1;
				column.SetSortInfo(ColumnSortOrder.None, false);
			}
			IList<GridSortInfo> list = sortInfo.Distinct(new GridSortInfoComparer()).ToList();
			groupCount -= sortInfo.Where(info => info.IsGrouped).Count() - list.Where(info => info.IsGrouped).Count();
			for(int i = 0; i < list.Count; i++) {
				GridSortInfo info = list[i];
				ColumnBase column = columns[info.FieldName];
				info.SetGroupSortIndexes(-1, -1);
				if(column == null)
					continue;
				bool isGrouped = i < groupCount;
				column.SetSortInfo(info.GetSortOrder(), isGrouped);
				if(isGrouped) {
					column.GroupIndexCore = i;
					info.SetGroupSortIndexes(-1, i);
					groupedColumnsList.Add(column);
				}
				else {
					int sortIndex = i - groupCount;
					column.SortIndex = sortIndex;
					info.SetGroupSortIndexes(sortIndex, -1);
				}
			}
			return groupedColumnsList;
		}
		#endregion
		ContentControl themeLoader;
		bool isUnloaded;
		internal bool IsUnloaded { get { return isUnloaded; } set { isUnloaded = value; } }
		protected bool IsActive { get { return !IsLoading && !isUnloaded; } }
		internal abstract DataViewBase DataView { get; set; }
		internal Locker updateSortIndexesLocker = new Locker();
		internal Locker syncronizationLocker = new Locker();
		int loadingCount = 0;
		protected internal bool IsLoading { get { return loadingCount != 0; } }
		internal bool IsDeserializing { get; set; }
		IColumnCollection columns;
		internal IColumnCollection ColumnsCore {
			get {
				if(this.columns == null)
					this.columns = CreateColumns();
				return this.columns;
			}
		}
		internal virtual int ActualGroupCountCore { get { return 0; } set { } }
		protected DataProviderBase fDataProvider;
		protected internal DataProviderBase DataProviderBase {
			get {
				if(fDataProvider == null) {
					fDataProvider = CreateDataProvider();
				}
				return fDataProvider;
			}
		}
		internal void UpdateAllDetailViewIndents() {
			GetRootDataControl().UpdateChildrenDetailViewIndents(null);
		}
		internal void UpdateChildrenDetailViewIndents(ObservableCollection<DetailIndent> ownerIndents) {
			MasterDetailProvider.UpdateDetailViewIndents(ownerIndents);
		}
		internal virtual bool GetIsExpandButtonVisible() {
			return false;
		}
		CriteriaOperator extraFilter;
		protected internal CriteriaOperator ExtraFilter {
			get {
				return extraFilter;
			}
			set {
				if(ReferenceEquals(extraFilter,value))
					return;
				extraFilter = value;
				UpdateFilter();
			}
		}
		CriteriaOperator IDXFilterable.SearchFilterCriteria {
			get { return ExtraFilter; }
		}
		internal LogicalPeerCache logicalPeerCache;
		protected internal LogicalPeerCache LogicalPeerCache {
			get {
				if(logicalPeerCache == null)
					logicalPeerCache = new LogicalPeerCache();
				return logicalPeerCache;
			}
		}
		internal virtual bool BottomRowBelowOldVisibleRowCount { get { return false; } }
		PeerCacheBase peerCache;
		protected internal PeerCacheBase PeerCache {
			get {
				if(peerCache == null) 
					peerCache = CreatePeerCache();
				return peerCache;
			}
		}
		DataControlAutomationPeer peer = null;
		protected internal DataControlAutomationPeer AutomationPeer {
			get { return peer; }
			set { peer = value; }
		}
		internal Locker dataResetLocker = new Locker();
		bool isLoaded;
		readonly EndInitPostponedAction rebuildSortInfoPostponedAction;
		readonly EndInitPostponedAction rePopulateColumnsPostponedAction;
		internal readonly Locker DataSourceChangingLocker = new Locker();
		internal int countColumnFilteringTrue = 0;
		internal int countColumnFilteringDefault = 0;
		internal int countColumnCellMerge = 0;
		ObservableCollectionCore<GridSortInfo> actualSortInfoCore;
		ReadOnlyGridSortInfoCollection actualSortInfo;
		internal ObservableCollectionCore<GridSortInfo> ActualSortInfoCore { get { return actualSortInfoCore; } }
		internal ReadOnlyGridSortInfoCollection ActualSortInfo { get { return actualSortInfo; } }
		readonly SortInfoCollectionBase sortInfoCore;
		internal SortInfoCollectionBase SortInfoCore { get { return sortInfoCore; } }
		internal Window Window { get { return LayoutHelper.FindParentObject<Window>(this); } }
		internal bool IsDataResetLocked { get { return dataResetLocker.IsLocked; } }
		ISummaryItemOwner totalSummary;
		ISummaryItemOwner groupSummary;
		internal ISummaryItemOwner TotalSummaryCore { get { return totalSummary; } }
		internal ISummaryItemOwner GroupSummaryCore { get { return groupSummary; } }
		LockedPostponedAction updateFocusedRowDataposponedAction;
		internal LockedPostponedAction UpdateFocusedRowDataposponedAction { get { return updateFocusedRowDataposponedAction; } }
		bool lockUpdateLayout = false;
		internal bool LockUpdateLayout { get { return lockUpdateLayout; } set { lockUpdateLayout = value; } }
		readonly NamePropertyChangeListener namePropertyChangeListener;
		[Browsable(false), DefaultValue(false)]
		public bool SupportDomainDataSource { get; set; }
		bool CanAddViewToLogicalChildren {
			get {
#if SL
				return VisualTreeHelper.GetChildrenCount(this) == 0;
#else
				return true;
#endif
			}
		}
		internal bool IsServerMode { get { return (DataProviderBase.IsServerMode && !DataProviderBase.IsICollectionView) || DataProviderBase.IsAsyncServerMode; } }
		protected internal Locker CurrentItemChangedLocker = new Locker();
		bool updateCurrentItemWasLocked = false;
		internal bool HasSelectedItems { get { return SelectedItems != null && SelectedItems.Count != 0; } }
#if SILVERLIGHT
		[Obsolete("Columns should be in visual tree")]
		public static bool AllowAddColumnsToVisualTree = true;
		internal ResourceCollectionSyncronizer<DataControlBase> columnsCollectionSynchronizer;
		internal IList ColumnsHolder { get { return columnsCollectionSynchronizer.Holder; } }
		internal static string ColumnsHolderName = Guid.NewGuid().ToString();
#endif
#if DEBUGTEST
		internal static int InstanceCount { get; set; }
#endif
		internal DataViewBase viewCore;
		[Browsable(false)]
		public DataControlFilteredComponent FilteredComponent { get; private set; }
		IDataControlParent dataControlParent;
		internal IDataControlParent DataControlParent {
			get { return dataControlParent ?? NullDataControlParent.Instance; }
			set {
				dataControlParent = value;
				DataControlParent.ValidateMasterDetailConsistency(this);
			}
		}
		IDataControlOwner dataControlOwner;
		internal IDataControlOwner DataControlOwner {
			get { return dataControlOwner ?? NullDataControlOwner.Instance; }
			set { dataControlOwner = value; }
		}
		protected DataControlBase(IDataControlOriginationElement dataControlOriginationElement) {
#if DEBUGTEST
			InstanceCount++;
#endif
			this.dataControlOriginationElement = dataControlOriginationElement;
			namePropertyChangeListener = NamePropertyChangeListener.CreateDesignTimeOnly(this, () => DesignTimeAdorner.UpdateDesignTimeInfo());
			MRUFiltersInternal = new ObservableCollectionCore<CriteriaOperatorInfo>();
			MRUFilters = new ReadOnlyObservableCollection<CriteriaOperatorInfo>(MRUFiltersInternal);
			rePopulateColumnsPostponedAction = new EndInitPostponedAction(() => IsLoading);
			rebuildSortInfoPostponedAction = new EndInitPostponedAction(() => IsLoading);
			this.actualSortInfoCore = new ObservableCollectionCore<GridSortInfo>();
			this.actualSortInfo = new ReadOnlyGridSortInfoCollection(actualSortInfoCore);
			this.sortInfoCore = CreateSortInfo();
			this.totalSummary = CreateSummariesCollection(SummaryItemCollectionType.Total);
			this.groupSummary = CreateSummariesCollection(SummaryItemCollectionType.Group);
			updateFocusedRowDataposponedAction = new LockedPostponedAction();
			internalNotificationManager = new NotificationManager(this);
			ColumnsCore.CollectionChanged += OnColumnsCollectionChanged;
			SortInfoCore.CollectionChanged += OnSortInfoChanged;
			TotalSummaryCore.CollectionChanged += OnTotalSummaryCollectionChanged;
			Loaded += new System.Windows.RoutedEventHandler(OnLoaded);
			Unloaded += new System.Windows.RoutedEventHandler(OnUnloaded);
			DataViewBase.SetIsFocusedCell(this, false);
			DataViewBase.SetIsFocusedRow(this, false);
			NeedSynchronize = false;
			FilteredComponent = new DataControlFilteredComponent(this);
			eventArgsConverter = new EventArgsConverter(this);
		}
		Dictionary<string, List<ColumnBase>> dependentColumns = new Dictionary<string, List<ColumnBase>>();
		internal List<ColumnBase> GetDependentColumns(string fieldName) {
			List<ColumnBase> result = null;
			if(!dependentColumns.TryGetValue(fieldName, out result)) {
				dependentColumns[fieldName] = result = GetDependentColumnsCore(fieldName);
			}
			return result;
		}
		List<ColumnBase> GetDependentColumnsCore(string fieldName) {
			List<ColumnBase> result = new List<ColumnBase>();
			foreach(ColumnBase column in ColumnsCore) {
				if(IsDependentColumn(column, fieldName))
					result.Add(column);
			}
			return result;
		}
		bool IsDependentColumn(ColumnBase column, string fieldName) {
			if(column.ActualBinding != null)
				return false;
			if(column.FieldName == fieldName)
				return true;
			if(column.UnboundType != UnboundColumnType.Bound)
				return true;
			if(column.FieldName.StartsWith(fieldName + "."))
				return true;
			return false;
		}
		internal void UpdateOwnerDetailDescriptor() {
			if(this.GetRootDataControl() == this)
				OwnerDetailDescriptor = null;
			OwnerDetailDescriptor = GetOriginationDataControl().DataControlOwner as DetailDescriptorBase;
		}
		readonly IDictionary<string, GridSortInfo> invalidSortCache = new Dictionary<string, GridSortInfo>();
		readonly IDictionary<string, PropertyGroupDescription> invalidGroupCache = new Dictionary<string, PropertyGroupDescription>();
		protected internal IDictionary<string, GridSortInfo> InvalidSortCache { get { return this.invalidSortCache; } }
		protected internal IDictionary<string, PropertyGroupDescription> InvalidGroupCache { get { return this.invalidGroupCache; } }
		internal void UpdateSortingFromInvalidSortCache(ColumnBase column) {
			if(!(invalidSortCache.ContainsKey(column.FieldName))) return;
			SortByCore(column, GridSortInfo.GetColumnSortOrder(invalidSortCache[column.FieldName].SortOrder));
			invalidSortCache.Remove(column.FieldName);
		}
		internal void UpdateGroupingFromInvalidGroupCache(ColumnBase column) {
			if(!(InvalidGroupCache.ContainsKey(column.FieldName))) return;
			GroupByColumn(column);
			InvalidGroupCache.Remove(column.FieldName);
		}
		protected virtual void GroupByColumn(ColumnBase column) { }
		internal void InvalidateDependentColumns() {
			dependentColumns.Clear();
		}
		protected internal void RaiseVisibleRowCountChanged() {
			RaisePropertyChanged("VisibleRowCount");
		}
		protected void RaisePropertyChanged(string p) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(p));
		}
		internal abstract ISummaryItemOwner CreateSummariesCollection(SummaryItemCollectionType collectionType);
		internal abstract SortInfoCollectionBase CreateSortInfo();
		internal abstract IColumnCollection CreateColumns();
		protected abstract DataProviderBase CreateDataProvider();
		protected abstract PeerCacheBase CreatePeerCache();
		internal bool IsValidRowHandleCore(int rowHandle) {
			return DataProviderBase.IsValidRowHandle(rowHandle);
		}
		internal bool IsRowVisibleCore(int rowHandle) {
			return DataProviderBase.IsRowVisible(rowHandle);
		}
		internal bool IsGroupRowHandleCore(int rowHandle) {
			return DataProviderBase.IsGroupRowHandle(rowHandle);
		}
		#region public properties
		[CloneDetailMode(CloneDetailMode.Skip)]
		public IEnumerable ColumnsSource {
			get { return (IEnumerable)GetValue(ColumnsSourceProperty); }
			set { SetValue(ColumnsSourceProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public DataTemplate ColumnGeneratorTemplate {
			get { return (DataTemplate)GetValue(ColumnGeneratorTemplateProperty); }
			set { SetValue(ColumnGeneratorTemplateProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public DataTemplateSelector ColumnGeneratorTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ColumnGeneratorTemplateSelectorProperty); }
			set { SetValue(ColumnGeneratorTemplateSelectorProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public Style ColumnGeneratorStyle {
			get { return (Style)GetValue(ColumnGeneratorStyleProperty); }
			set { SetValue(ColumnGeneratorStyleProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public IEnumerable TotalSummarySource {
			get { return (IEnumerable)GetValue(TotalSummarySourceProperty); }
			set { SetValue(TotalSummarySourceProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public DataTemplate TotalSummaryGeneratorTemplate {
			get { return (DataTemplate)GetValue(TotalSummaryGeneratorTemplateProperty); }
			set { SetValue(TotalSummaryGeneratorTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseItemsSource"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(true), Category(Categories.Data), CloneDetailMode(CloneDetailMode.Skip)]
		public object ItemsSource {
			get { return GetValue(ItemsSourceProperty); }
			set {
				if(value == null) {
					ClearValue(ItemsSourceProperty);
				}
				else {
					SetValue(ItemsSourceProperty, value);
				}
			}
		}
		[Browsable(false)]
		internal object ActualItemsSource {
			get { return GetItemsSource(); }
		}
		protected virtual object GetItemsSource() {
			return ItemsSource;
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseDesignTimeShowSampleData"),
#endif
 Category(Categories.OptionsDesignTime), CloneDetailMode(CloneDetailMode.Skip)]
		public bool DesignTimeShowSampleData {
			get { return (bool)GetValue(DesignTimeShowSampleDataProperty); }
			set { SetValue(DesignTimeShowSampleDataProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseDesignTimeUseDistinctSampleValues"),
#endif
 Category(Categories.OptionsDesignTime), CloneDetailMode(CloneDetailMode.Skip)]
		public bool DesignTimeUseDistinctSampleValues {
			get { return (bool)GetValue(DesignTimeUseDistinctSampleValuesProperty); }
			set { SetValue(DesignTimeUseDistinctSampleValuesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseDesignTimeDataSourceRowCount"),
#endif
 Category(Categories.OptionsDesignTime), CloneDetailMode(CloneDetailMode.Skip)]
		public int DesignTimeDataSourceRowCount {
			get { return (int)GetValue(DesignTimeDataSourceRowCountProperty); }
			set { SetValue(DesignTimeDataSourceRowCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseDesignTimeDataObjectType"),
#endif
 Category(Categories.OptionsDesignTime), CloneDetailMode(CloneDetailMode.Skip)]
		public Type DesignTimeDataObjectType {
			get { return (Type)GetValue(DesignTimeDataObjectTypeProperty); }
			set { SetValue(DesignTimeDataObjectTypeProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("DataControlBaseVisibleRowCount")]
#endif
		public int VisibleRowCount { 
			get {
				int visibleCount = DataProviderBase.VisibleCount;
				if(DataView != null && DataView.ShouldDisplayBottomRow)
					visibleCount++;
				return visibleCount;
			} 
		}
		internal virtual void SetCellValueCore(int rowHandle, string fieldName, object value) {
			object oldValue = GetCellValue(rowHandle, fieldName);
			DataProviderBase.SetRowValue(rowHandle, fieldName, value);
			bool notifyChanges = DataProviderBase.GetRowValue(rowHandle) is INotifyPropertyChanged && (ColumnsCore[fieldName] == null || !ColumnsCore[fieldName].IsUnbound);
			if((ItemsSource is IBindingList || ItemsSource is DataTable || ItemsSource is INotifyCollectionChanged) && !notifyChanges)
				UpdateRowCore(rowHandle);
			DataView.RaiseCellValueChanged(rowHandle, ColumnsCore[fieldName], GetCellValue(rowHandle, fieldName), oldValue);
		}
		[Browsable(false)]
		public CriteriaOperatorInfo ActiveFilterInfo {
			get { return (CriteriaOperatorInfo)GetValue(ActiveFilterInfoProperty); }
			internal set { this.SetValue(ActiveFilterInfoPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseAllowMRUFilterList"),
#endif
 XtraSerializableProperty,
Category(Categories.OptionsFilter), CloneDetailMode(CloneDetailMode.Skip)]
		public bool AllowMRUFilterList {
			get { return (bool)GetValue(AllowMRUFilterListProperty); }
			set { SetValue(AllowMRUFilterListProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseShowAllTableValuesInCheckedFilterPopup"),
#endif
 XtraSerializableProperty,
Category(Categories.OptionsFilter), CloneDetailMode(CloneDetailMode.Skip)]
		public bool ShowAllTableValuesInCheckedFilterPopup {
			get { return (bool)GetValue(ShowAllTableValuesInCheckedFilterPopupProperty); }
			set { SetValue(ShowAllTableValuesInCheckedFilterPopupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseShowAllTableValuesInFilterPopup"),
#endif
 XtraSerializableProperty,
Category(Categories.OptionsFilter), CloneDetailMode(CloneDetailMode.Skip)]
		public bool ShowAllTableValuesInFilterPopup {
			get { return (bool)GetValue(ShowAllTableValuesInFilterPopupProperty); }
			set { SetValue(ShowAllTableValuesInFilterPopupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseMRUFilterListCount"),
#endif
 XtraSerializableProperty,
Category(Categories.OptionsFilter), CloneDetailMode(CloneDetailMode.Skip)]
		public int MRUFilterListCount {
			get { return (int)GetValue(MRUFilterListCountProperty); }
			set { SetValue(MRUFilterListCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseAllowColumnMRUFilterList"),
#endif
 XtraSerializableProperty,
Category(Categories.OptionsFilter)]
		public bool AllowColumnMRUFilterList {
			get { return (bool)GetValue(AllowColumnMRUFilterListProperty); }
			set { SetValue(AllowColumnMRUFilterListProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseMRUColumnFilterListCount"),
#endif
 XtraSerializableProperty,
Category(Categories.OptionsFilter)]
		public int MRUColumnFilterListCount {
			get { return (int)GetValue(MRUColumnFilterListCountProperty); }
			set { SetValue(MRUColumnFilterListCountProperty, value); }
		}
		[Category(Categories.OptionsBehavior)]
		public event ItemsSourceChangedEventHandler ItemsSourceChanged {
			add { AddHandler(ItemsSourceChangedEvent, value); }
			remove { RemoveHandler(ItemsSourceChangedEvent, value); }
		}
		[Category(Categories.OptionsFilter)]
		public event RoutedEventHandler FilterChanged {
			add { AddHandler(FilterChangedEvent, value); }
			remove { RemoveHandler(FilterChangedEvent, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseIsFilterEnabled"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty(SerializationOrders.GridControl_IsFilterEnabled), GridUIProperty]
		public bool IsFilterEnabled {
			get { return (bool)GetValue(IsFilterEnabledProperty); }
			set { SetValue(IsFilterEnabledProperty, value); }
		}
		[
		Category(Categories.Data),
		Browsable(false),
		]
		public CriteriaOperator FilterCriteria {
			get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); }
			set { SetValue(FilterCriteriaProperty, value); }
		}
		[
		Category(Categories.Data),
		Browsable(false),
		]
		public CriteriaOperator FixedFilter {
			get { return (CriteriaOperator)GetValue(FixedFilterProperty); }
			set { SetValue(FixedFilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseFilterString"),
#endif
 Category(Categories.Data), XtraSerializableProperty(SerializationOrders.GridControl_FilterString), GridUIProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public string FilterString {
			get { return (string)GetValue(FilterStringProperty); }
			set { SetValue(FilterStringProperty, value); }
		}
		[Obsolete("Use the AutoGenerateColumns property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), CloneDetailMode(CloneDetailMode.Skip)]
		public bool AutoPopulateColumns {
			get { return (bool)GetValue(AutoPopulateColumnsProperty); }
			set { SetValue(AutoPopulateColumnsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseAutoGenerateColumns"),
#endif
 Category(Categories.OptionsBehavior), CloneDetailMode(CloneDetailMode.Skip)]
		public AutoGenerateColumnsMode AutoGenerateColumns {
			get { return (AutoGenerateColumnsMode)GetValue(AutoGenerateColumnsProperty); }
			set { SetValue(AutoGenerateColumnsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseEnableSmartColumnsGeneration"),
#endif
 Category(Categories.OptionsBehavior), CloneDetailMode(CloneDetailMode.Skip)]
		public bool EnableSmartColumnsGeneration {
			get { return (bool)GetValue(EnableSmartColumnsGenerationProperty); }
			set { SetValue(EnableSmartColumnsGenerationProperty, value); }
		}
		[Obsolete("Use the AutoGeneratedColumns event instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event RoutedEventHandler ColumnsPopulated {
			add { AddHandler(ColumnsPopulatedEvent, value); }
			remove { RemoveHandler(ColumnsPopulatedEvent, value); }
		}
		[Category("Events")]
		public event RoutedEventHandler AutoGeneratedColumns {
			add { AddHandler(AutoGeneratedColumnsEvent, value); }
			remove { RemoveHandler(AutoGeneratedColumnsEvent, value); }
		}
		[Category("Events")]
		public event AutoGeneratingColumnEventHandler AutoGeneratingColumn {
			add { AddHandler(AutoGeneratingColumnEvent, value); }
			remove { RemoveHandler(AutoGeneratingColumnEvent, value); }
		}
		[Category("Events")]
		public event CustomUniqueValuesEventHandler CustomUniqueValues {
			add { AddHandler(CustomUniqueValuesEvent, value); }
			remove { RemoveHandler(CustomUniqueValuesEvent, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseShowBorder"),
#endif
 Category(Categories.Appearance), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseShowLoadingPanel"),
#endif
 Category(Categories.Appearance), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool ShowLoadingPanel {
			get { return (bool)GetValue(ShowLoadingPanelProperty); }
			set { SetValue(ShowLoadingPanelProperty, value); }
		}
		public string DefaultSorting {
			get { return (string)GetValue(DefaultSortingProperty); }
			set { SetValue(DefaultSortingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseCurrentItem"),
#endif
 Category(Categories.Data), CloneDetailMode(CloneDetailMode.Skip)]
		public object CurrentItem {
			get { return GetValue(CurrentItemProperty); }
			set { SetValue(CurrentItemProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseCurrentColumn"),
#endif
 Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public ColumnBase CurrentColumn {
			get { return (ColumnBase)GetValue(CurrentColumnProperty); }
			set { SetValue(CurrentColumnProperty, value); }
		}
		public object CurrentCellValue {
			get { return GetValue(CurrentCellValueProperty); }
			set { SetValue(CurrentCellValueProperty, value); }
		}
		[Category(Categories.Data), CloneDetailMode(CloneDetailMode.Skip)]
		public object SelectedItem {
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public IList SelectedItems {
			get { return (IList)GetValue(SelectedItemsProperty); }
			set { SetValue(SelectedItemsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseSelectionMode"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsSelection)]
		public MultiSelectMode SelectionMode {
			get { return (MultiSelectMode)GetValue(SelectionModeProperty); }
			set { SetValue(SelectionModeProperty, value); }
		}
		public bool AllowUpdateTwoWayBoundPropertiesOnSynchronization {
			get { return (bool)GetValue(AllowUpdateTwoWayBoundPropertiesOnSynchronizationProperty); }
			set { SetValue(AllowUpdateTwoWayBoundPropertiesOnSynchronizationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseImplyNullLikeEmptyStringWhenFiltering"),
#endif
 Category(Categories.OptionsFilter)]
		public bool ImplyNullLikeEmptyStringWhenFiltering {
			get { return (bool)GetValue(ImplyNullLikeEmptyStringWhenFilteringProperty); }
			set { SetValue(ImplyNullLikeEmptyStringWhenFilteringProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseUseFieldNameForSerialization"),
#endif
 XtraSerializableProperty(SerializationOrders.GridControl_UseFieldNameForSerialization), XtraResetProperty(ResetPropertyMode.None), GridStoreAlwaysProperty, Category(Categories.OptionsLayout)]
		public bool UseFieldNameForSerialization {
			get { return (bool)GetValue(UseFieldNameForSerializationProperty); }
			set { SetValue(UseFieldNameForSerializationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("DataControlBaseClipboardCopyMode"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsCopy)]
		public ClipboardCopyMode ClipboardCopyMode {
			get { return (ClipboardCopyMode)GetValue(ClipboardCopyModeProperty); }
			set { SetValue(ClipboardCopyModeProperty, value); }
		}
		public event PastingFromClipboardEventHandler PastingFromClipboard {
			add { AddHandler(PastingFromClipboardEvent, value); }
			remove { RemoveHandler(PastingFromClipboardEvent, value); }
		}
		public event CurrentItemChangedEventHandler CurrentItemChanged {
			add { AddHandler(CurrentItemChangedEvent, value); }
			remove { RemoveHandler(CurrentItemChangedEvent, value); }
		}
		public event SelectedItemChangedEventHandler SelectedItemChanged {
			add { AddHandler(SelectedItemChangedEvent, value); }
			remove { RemoveHandler(SelectedItemChangedEvent, value); }
		}
		public event CurrentColumnChangedEventHandler CurrentColumnChanged {
			add { AddHandler(CurrentColumnChangedEvent, value); }
			remove { RemoveHandler(CurrentColumnChangedEvent, value); }
		}
		#endregion
		protected internal DataControlBase GetMasterGridCore() {
			DataViewBase masterView = null;
			int masterVisibleIndex = -1;
			bool found = DataControlParent.FindMasterRow(out masterView, out masterVisibleIndex);
			return found ? masterView.DataControl : null;
		}
		public void RefreshData() {
			DataProviderBase.RefreshableSource.Do(x => x.Refresh());
			DataProviderBase.DoRefresh();
		}
		public void BeginDataUpdate() {
			DataProviderBase.BeginUpdate();
			DataProviderBase.BindingListAdapter.Do(x => x.RaisesItemChangedEvents = false);
		}
		public void EndDataUpdate() {
			DataProviderBase.BindingListAdapter.Do(x => x.RaisesItemChangedEvents = true);
			DataProviderBase.RefreshableSource.Do(x => x.Refresh());
			DataProviderBase.EndUpdate();
		}
		public override void BeginInit() {
			GetDataControlOriginationElement().NotifyBeginInit(this, dataControl => dataControl);
			this.loadingCount++;
			base.BeginInit();
		}
		public override void EndInit() {
			UpdateUnboundColumnsType();
			GetDataControlOriginationElement().NotifyEndInit(this, dataControl => dataControl);
			rebuildSortInfoPostponedAction.PerformActionOnEndInitIfNeeded(RebuildSortInfo);
			this.loadingCount--;
			rePopulateColumnsPostponedAction.PerformActionOnEndInitIfNeeded(OnColumnUnboundChanged);
			if(IsInitialized)
				OnInitialized(EventArgs.Empty);
			base.EndInit();
			if(DataView.Parent != this && CanAddViewToLogicalChildren) {
				AddLogicalChild(DataView);
			}
			OnDataChanged(true);
			InitializeSelection(true);
		}
		void UpdateUnboundColumnsType() {
			if(DataView == null || DataView.OriginationView == null)
				return;
			foreach(GridColumnBase column in ColumnsCore)
				column.SetUnboundType();
		}
		internal void ForceLoad() {
			if(!isLoaded) {
				OnDataChanged();
				isLoaded = true;
			}
		}
		protected bool LockRepopulateColumnsOnUnboundChanged { get; set; }
		protected bool NeedRepopulateColumnsOnUnboundChanged { get; set; }
#if DEBUGTEST
		public int RepopulateColumnsCallsCount;
#endif
		protected internal virtual void OnColumnUnboundChanged() {
			if(LockRepopulateColumnsOnUnboundChanged) {
				NeedRepopulateColumnsOnUnboundChanged = true;
				return;
			}
			BeginDataUpdate();
			try {
#if DEBUGTEST
				RepopulateColumnsCallsCount++;
#endif
				DataProviderBase.RePopulateColumns();
				if(DataView != null && !IsDeserializing) SynchronizeDataProvider();
			}
			finally {
				EndDataUpdate();
			}
			InvalidateDependentColumns();
		}
		protected internal void OnColumnUnboundChangedPosponed() {
			OnColumnUnboundChangedPosponed(ColumnsCore.IsLockUpdate);
		}
		void OnColumnUnboundChangedPosponed(bool isLocked) {
			if(isLocked) {
				doUnboundChangedOnColumnsEndUpdate = true;
			} else {
				rePopulateColumnsPostponedAction.PerformIfNotLoading(OnColumnUnboundChanged);
			}
		}
		protected void ApplyGroupSortIndexIfNotLoading(ColumnBase column, Action<ColumnBase> columnAction, Action<ColumnBase> columnActionWhenLoading = null) {
			rebuildSortInfoPostponedAction.PerformIfNotLoading(() => columnAction(column), () => {
				if(columnActionWhenLoading != null)
					columnActionWhenLoading(column);
			});
		}
		internal int GetRowHandleByVisibleIndexCore(int visibleIndex) {
			if(viewCore.ShouldDisplayBottomRow && visibleIndex == VisibleRowCount - 1)
				return NewItemRowHandle;
			return DataProviderBase.GetControllerRow(visibleIndex);
		}
		internal int GetRowVisibleIndexByHandleCore(int rowHandle) {
			if(viewCore.ShouldDisplayBottomRow && viewCore.IsNewItemRowHandle(rowHandle))
				return VisibleRowCount - 1;
			return DataProviderBase.GetRowVisibleIndexByHandle(rowHandle);
		}
		internal int GetCommonVisibleIndex(int rowHandle) {
			int visibleIndex = GetRowVisibleIndexByHandleCore(rowHandle);
			int commonVisibleIndex = 0;
			if(DataView.IsNewItemRowVisible && rowHandle == NewItemRowHandle) {
				visibleIndex = 0;
				commonVisibleIndex = -1;
			}
			EnumerateThisAndParentDataControls((dataControl, index) => {
				commonVisibleIndex += dataControl.MasterDetailProvider.CalcDetailRowsCountBeforeRow(index);
				commonVisibleIndex += index;
				if(dataControl.DataView.IsNewItemRowVisible)
					commonVisibleIndex++;
				if(dataControl != this)
					commonVisibleIndex++;
			}, visibleIndex);
			return commonVisibleIndex;
		}
		internal KeyValuePair<DataViewBase, int> FindViewAndVisibleIndexByCommonVisibleIndex(int commonVisibleIndex) {
			return GetRootDataControl().FindViewAndVisibleIndexByCommonVisibleIndexCore(commonVisibleIndex);
		}
		KeyValuePair<DataViewBase, int> FindViewAndVisibleIndexByCommonVisibleIndexCore(int commonVisibleIndex) {
			var info = MasterDetailProvider.CalcMasterRowNavigationInfo(commonVisibleIndex);
			if(info == null)
				return new KeyValuePair<DataViewBase, int>();
			if(!info.IsDetail) {
				int visibleIndex = info.StartVisibleIndex;
				if(DataView.IsNewItemRowVisible)
					visibleIndex--;
				return new KeyValuePair<DataViewBase, int>(DataView, visibleIndex);
			}
			int rowHandle = GetRowHandleByVisibleIndexCore(info.StartVisibleIndex);
			DataControlBase dataControl = MasterDetailProvider.FindVisibleDetailDataControl(rowHandle);
			return dataControl.FindViewAndVisibleIndexByCommonVisibleIndexCore(info.DetailStartVisibleIndex);
		}
		public object GetRow(int rowHandle) {
			return DataProviderBase.GetRowValue(rowHandle);
		}
		public object GetCellValue(int rowHandle, string fieldName) {
			return GetCellValueCore(rowHandle, fieldName);
		}
		internal virtual object GetCellValueCore(int rowHandle, string fieldName) {
			return DataProviderBase.GetRowValue(rowHandle, fieldName);
		}
		internal object GetCellValueCore(int rowHandle, ColumnBase column) {
			return GetCellValue(rowHandle, column.FieldName);
		}
		public string GetCellDisplayText(int rowHandle, string fieldName) {
			return GetCellDisplayTextCore(rowHandle, ColumnsCore[fieldName]);
		}
		protected string GetCellDisplayTextCore(int rowHandle, ColumnBase column) {
			return viewCore.GetColumnDisplayText(GetCellValue(rowHandle, column.FieldName), column, rowHandle);
		}
		internal void AddMRUFilter(CriteriaOperatorInfo filter) {
			if(filter == null) return;
			if(MRUFiltersInternal.Contains(filter)) {
				MRUFiltersInternal.Remove(filter);
			}
			MRUFiltersInternal.Insert(0, filter);
			if(MRUFiltersInternal.Count > MRUFilterListCount) {
				MRUFiltersInternal.RemoveAt(MRUFilterListCount);
			}
		}
		internal void RemoveMRUFilter(CriteriaOperatorInfo filter) {
			MRUFiltersInternal.Remove(filter);
		}
		public void AddMRUFilter(CriteriaOperator filterCriteria) {
			CriteriaOperatorInfo filter = null;
			if(DataView != null)
				filter = new CriteriaOperatorInfo(filterCriteria, DataView.GetFilterOperatorCustomText(filterCriteria));
			else filter = GetInfoFromCriteriaOperator(filterCriteria);
			AddMRUFilter(filter);
		}
		public void RemoveMRUFilter(CriteriaOperator filterCriteria) {
			CriteriaOperatorInfo filter = GetInfoFromCriteriaOperator(filterCriteria);
			RemoveMRUFilter(filter);
		}
		public void ClearMRUFilter() {
			MRUFiltersInternal.Clear();
		}
		protected internal CriteriaOperator CalcColumnFilterCriteriaByValue(ColumnBase column, object columnValue) {
			return DataProviderBase.CalcColumnFilterCriteriaByValue(column, columnValue);
		}
		protected internal object[] GetUniqueColumnValues(ColumnBase column, bool includeFilteredOut, bool roundDataTime) {
			CustomUniqueValuesEventArgs e = new CustomUniqueValuesEventArgs(column, includeFilteredOut, roundDataTime, 
					delegate(object valuesObject) {
						object[] values = valuesObject as object[];
						OnAsyncGetColumnValuesCompleted(column, values);
					}) { RoutedEvent = CustomUniqueValuesEvent };
			RaiseEventInOriginationGrid(e);
			if(e.Handled) {
				return e.UniqueValues ?? new object[] {AsyncServerModeDataController.NoValue};
			}
			return DataProviderBase.GetUniqueColumnValues(column, includeFilteredOut, roundDataTime, ImplyNullLikeEmptyStringWhenFiltering);
		}
		void OnAsyncGetColumnValuesCompleted(ColumnBase column, object[] values) {
			column.ColumnFilterInfo.UpdateCurrentPopupData(values);
		}
		public object GetTotalSummaryValue(DevExpress.Xpf.Grid.SummaryItemBase item) {
			return DataProviderBase.GetTotalSummaryValue(item);
		}
		internal void UpdateColumnFilteringCounters(DefaultBoolean? newAllowColumnFiltering, DefaultBoolean? oldAllowColumnFiltering) {
			if(newAllowColumnFiltering == DefaultBoolean.True)
				countColumnFilteringTrue++;
			else if(newAllowColumnFiltering == DefaultBoolean.Default)
				countColumnFilteringDefault++;
			if(oldAllowColumnFiltering == DefaultBoolean.True)
				countColumnFilteringTrue--;
			else if(oldAllowColumnFiltering == DefaultBoolean.Default)
				countColumnFilteringDefault--;
			if(DataView != null)
				DataView.UpdateShowEditFilterButtonCore();
		}
		internal void UpdateColumnCellMergeCounter(bool? oldValue, bool? newValue) {
			if(newValue.HasValue && newValue.Value)
				countColumnCellMerge++;
			if(oldValue.HasValue && oldValue.Value)
				countColumnCellMerge--;
			if(DataView == null) return;
			DataView.UpdateActualAllowCellMergeCore();
			DataView.UpdateCellMergingPanels();
		}
		void OnFilterCriteriaChanged() {
			string filterString = !object.ReferenceEquals(FilterCriteria, null) ? FilterCriteria.ToString() : string.Empty;
			if(filterString != FilterString)
				FilterString = filterString;
			foreach(ColumnBase column in ColumnsCore)
				column.UpdateAutoFilterValue();
			ApplyFilter(FilterCriteria);
			if(DataView != null) DataView.UpdateFilterPanel();
		}
		void OnFixedFilterChanged() {
			ApplyFilter(FilterCriteria);
		}
		internal void UpdateActiveFilterInfo() {
			CriteriaOperatorInfo info;
			if(!object.Equals(FilterCriteria, null) && DataView != null) {
				info = new CriteriaOperatorInfo(FilterCriteria, DataView.FilterPanelText);
			}
			else info = null;
			if(!IsDeserializing) {
				AddMRUFilter(ActiveFilterInfo);
				ActiveFilterInfo = info;
				RemoveMRUFilter(info);
			}
			else {
				ActiveFilterInfo = info;
			}
		}
		protected internal virtual void RaiseFilterChanged() {
			RaiseGridEventInOriginationGrid(FilterChangedEvent);
		}
		object CoerceFilterCriteria(CriteriaOperator newValue) {
			if(DataView != null && !DataView.CommitEditing())
				return DependencyObjectHelper.GetCoerceValue(this, FilterCriteriaProperty);
			return newValue;
		}
		object CoerceFixedFilter(CriteriaOperator newValue) {
			if(DataView != null && !DataView.CommitEditing())
				return DependencyObjectHelper.GetCoerceValue(this, FixedFilterProperty);
			return newValue;
		}
		void OnFilterStringChanged() {
			if(object.ReferenceEquals(FilterCriteria, null)) {
				if(FilterString == string.Empty) return;
			} else {
				if(FilterCriteria.ToString() == FilterString) return;
			}
			FilterCriteria = CriteriaOperator.TryParse(FilterString);
		}
		object CoerceFilterString(string newValue) {
			if(DataView != null && !DataView.RequestUIUpdate())
				return DependencyObjectHelper.GetCoerceValue(this, FilterStringProperty);
			return newValue;
		}
		internal void OnIsFilterEnabledChanged() {
			UpdateFilter();
		}
		internal void UpdateFilter() {
			ApplyFilter(GetActualFilterCriteria());
		}
		internal CriteriaOperator GetActualFilterCriteria() {
			return IsFilterEnabled ? FilterCriteria : null;
		}
		object CoerceIsFilterEnabled(bool newValue) {
			if(DataView != null && !DataView.RequestUIUpdate())
				return DependencyObjectHelper.GetCoerceValue(this, IsFilterEnabledProperty);
			return newValue;
		}
#if DEBUGTEST
		public int debug_ApplyFilterFireCount = 0;
#endif
		protected internal virtual void ApplyFilter(CriteriaOperator op) {
			if(IsLoading || IsDeserializing) return;
#if DEBUGTEST
			debug_ApplyFilterFireCount++;
#endif
			CriteriaOperator oldFilter = DataProviderBase.FilterCriteria;
			dataResetLocker.DoLockedActionIfNotLocked(delegate {
				DestroyFilterData();
				if(!object.ReferenceEquals(op, null))
					IsFilterEnabled = true;
				DataProviderBase.FilterCriteria = GroupOperator.And(op, ExtraFilter, FixedFilter);
				if(!DataProviderBase.IsUpdateLocked) {
					UpdateRowsCore(false);
					UpdateTotalSummaryCore();
					if((DataView != null) && (DataView.DataPresenter != null))
						DataView.ImmediateActionsManager.EnqueueAction(new ScrollRowsAfterApplyFilterAction(DataView));
				}
			});
			if(!object.ReferenceEquals(oldFilter, DataProviderBase.FilterCriteria)) RaiseFilterChanged();
		}
		internal void PerformDataResetLockedActionCore(Action action) {
			dataResetLocker.DoLockedAction(action);
		}
		protected internal virtual void OnUpdateRowsCore() {
			if(AutomationPeer != null) {
				AutomationPeer.ClearLogicalPeerCache();
				AutomationPeer.ResetDataPanelPeer();
			}
			if(viewCore != null)
				viewCore.OnUpdateRowsCore();
		}
		internal void UpdateRowsCore(bool updateColumnsViewInfo = true, bool updateDataObjects = true) {
			OnUpdateRowsCore();
			if(DataView != null) {
				DataView.Nodes.Clear();
				DataView.VisualDataTreeBuilder.GroupSummaryNodes.Clear();
				DataView.RootNodeContainer.ReGenerateMasterRootItems();
				DataView.UpdateDataObjects(updateColumnsViewInfo, updateDataObjects);
			}
		}
		protected virtual void UpdateTotalSummaryCore() {
			if(DataView != null)
				DataView.UpdateColumnsTotalSummary();
		}
		internal RowValidationError RowStateError { get; private set; }
		internal void SetRowStateError(int rowHandle, RowValidationError error) {
			RowStateError = error;
			DataView.UpdateRowDataByRowHandle(rowHandle, (rowData) => rowData.UpdateRowDataError());
		}
		protected virtual void OnAutoPopulateColumnsChanged() {
			SetCurrentValue(AutoGenerateColumnsProperty, (bool)GetValue(AutoPopulateColumnsProperty) ? (AutoGenerateColumns == AutoGenerateColumnsMode.None ? AutoGenerateColumnsMode.KeepOld : AutoGenerateColumns) : AutoGenerateColumnsMode.None);
		}
		protected virtual void OnAutoGenerateColumnsChanged() {
			SetCurrentValue(AutoPopulateColumnsProperty, AutoGenerateColumns != AutoGenerateColumnsMode.None);
			InvalidateDesignTimeDataSourceCore();
		}
		protected virtual void OnEnableSmartColumnsGenerationChanged() {
			InvalidateDesignTimeDataSourceCore();
		}
		protected virtual void OnDefaultSortingChanged() {
			UpdateDefaultSorting();
		}
		void UpdateDefaultSorting() {
			SortInfoCore.DefaultSorting = DefaultSorting;
			if(!String.IsNullOrEmpty(DefaultSorting)) {
				ColumnBase column = ColumnsCore[DefaultSorting];
				if(column != null) {
					SortInfoCore.DefaultSortOrder = (column.SortOrder == ColumnSortOrder.Descending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
				}
			}
		}
		protected virtual void OnItemsSourceChanged(object oldValue, object newValue) {
			DataSourceChangingLocker.DoLockedAction(delegate {
				updateFocusedRowDataposponedAction.PerformLockedAction(delegate {
					if(IsLoading)
						return;
					if(DataView != null) {
						DataView.CommitAndCleanEditor();
						DataView.ValidationError = null;
					}
					DestroyFilterData();
					SetDataSource();
					if(DataView != null) {
						DataView.OnDataSourceReset();
						DataView.UpdateContentLayout();
					}
				});
			});
			ICollectionView collectionView = newValue as ICollectionView;
			SubscribeCollectionViewCurrentItem(oldValue as ICollectionView, collectionView);
			UpdateUnboundColumnsAllowSorting(collectionView);
			ReturnCurrentItem();
			if(DataView != null && DataView.DataProviderBase.DataController != null)
				DataView.DataProviderBase.DataController.SummariesIgnoreNullValues = DataView.SummariesIgnoreNullValues;
			RaiseItemsSourceChanged(oldValue, newValue);
		}
		void SetDataSource() {
			DataProviderBase.DataSource = ActualItemsSource;
			if(DataView != null && !DataView.IsFocusedView)
				DataProviderBase.CurrentControllerRow = DataControlBase.InvalidRowHandle;
		}
		void UpdateUnboundColumnsAllowSorting(ICollectionView collectionView) {
			if(collectionView == null) return;
			foreach(ColumnBase column in ColumnsCore)
				UpdateUnboundColumnAllowSorting(column);
		}
		void SubscribeCollectionViewCurrentItem(ICollectionView previousSource, ICollectionView newSource) {
			if(previousSource != null)
				CurrentChangedEventManager.RemoveListener(previousSource, this);
			if(newSource != null) {
				CurrentChangedEventManager.AddListener(newSource, this);
				CollectionViewSyncCurrentItem(newSource);
			}
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(CurrentChangedEventManager)) {
				CollectionViewCurrentChanged(sender, e);
				return true;
			}
			return false;
		}
		void CollectionViewCurrentChanged(object sender, EventArgs e) {
			if(DataProviderBase.CollectionViewSource != null)
				CollectionViewSyncCurrentItem(DataProviderBase.CollectionViewSource);
		}
		internal void CollectionViewSyncCurrentItem(ICollectionView source) {
			if(DataView != null && DataView.IsSynchronizedWithCurrentItem
#if SL
				&& (DataView.FocusedRowHandle != DataControlBase.AutoFilterRowHandle)
#endif
				&& (DataView.FocusedRowHandle != DataControlBase.NewItemRowHandle)) {
#if !SL
				if(source.CurrentItem == CollectionView.NewItemPlaceholder)
					DataView.SetFocusedRowHandle(DataControlBase.NewItemRowHandle);
				else
#endif
				if(!DataView.FocusedRowHandleChangedLocker.IsLocked) 
					SetCurrentItemCore(source.CurrentItem);
			}
		}
		protected virtual void RaiseItemsSourceChanged(object oldValue, object newValue) {
			RaiseEventInOriginationGrid(new ItemsSourceChangedEventArgs(this, oldValue, newValue) { RoutedEvent = ItemsSourceChangedEvent });
		}
		public void RefreshRow(int rowHandle) {
			DataProviderBase.RefreshRow(rowHandle);
		}
		protected internal virtual void OnViewChanged(DataViewBase oldValue, DataViewBase newValue) {
			if(newValue == null) {
				DataView = newValue = DesignTimeAdorner.GetDefaultView(this);
			}
			viewCore = newValue;
			ValidateMasterDetailConsistency();
			ValidateDataProvider(newValue);
			if(newValue != null) {
				DestroyFilterData();
#if !SL
				if(newValue.Parent is DataControlBase) {
					((DataControlBase)newValue.Parent).DataView = null;
				}
#endif
				newValue.DataControl = this;
				UpdateViewActualColumnChooserTemplate();
				UpdateBandsLayoutProperties();
				SynchronizeCurrentColumn();
				if(!IsLoading)
					newValue.DataProviderBase.DataSource = ActualItemsSource;
				if((newValue != null && newValue.ViewBehavior.GetServiceUnboundColumns().Any()) || (oldValue != null && oldValue.ViewBehavior.GetServiceUnboundColumns().Any())) {
					AttachToFormatConditions(FormatConditionChangeType.All);
				}
			}
			foreach(ColumnBase columnItem in ColumnsCore)
				columnItem.ChangeOwner();
			ApplyNewView();
			if(newValue != null) {
				if(!IsLoading && CanAddViewToLogicalChildren)
					AddLogicalChild(newValue);
				newValue.InitMenu();
			}
			if(oldValue != null) {
				oldValue.ResetMenu();
				oldValue.DataControl = null;
				oldValue.ResetDataProvider();
				RemoveLogicalChild(oldValue);
			}
			UpdateSearchPanel(oldValue, newValue);
			DesignTimeAdorner.UpdateDesignTimeInfo();
			UpdateColumnSummaries(TotalSummaryCore, NotifyCollectionChangedAction.Reset, SummaryItemCollectionType.Total);
			UpdateColumnSummaries(GroupSummaryCore, NotifyCollectionChangedAction.Reset, SummaryItemCollectionType.Group);
			UpdateTotalSummary();
			UpdateAllowPartialGrouping();
#if SL
			ThemeManager.SetApplyApplicationTheme(DataView, true);
#endif
			UpdateHasDetailViews();
		}
		internal virtual void AttachToFormatConditions(FormatConditionChangeType changeType) {
			if(changeType.HasFlag(FormatConditionChangeType.UnboundColumn)) {
				OnColumnUnboundChangedPosponed();
				return;
			}
			if(changeType.HasFlag(FormatConditionChangeType.Summary))
				throw new NotSupportedException();
		}
		internal abstract void ValidateDataProvider(DataViewBase newValue);
		protected virtual void SynchronizeDataProvider() {
			DataView.ScrollAnimationLocker.DoLockedAction(() => {
				try {
					DataProviderBase.Syncronize(SortInfoCore, SortInfoCore.GroupCountCore, GroupOperator.And(GetActualFilterCriteria(), ExtraFilter));
				}
				catch(Exception e) {
					OnSynchronizeDataProviderException(e);
				}
			});
		}
		protected virtual void OnSynchronizeDataProviderException(Exception e) {
#if DEBUGTEST
			throw e;
#else
			MessageBox.Show(e.ToString());
#endif
		}
		protected virtual void ApplyNewView() {
			SetActiveView(this, DataView);
			OnDataChanged();
			UpdateMasterDetailProvider();
		}
		public void UpdateTotalSummary() {
			DataProviderBase.UpdateTotalSummary();
			if(DataView != null)
				DataView.UpdateColumnsTotalSummary();
		}
		internal virtual void UpdateAllowPartialGrouping() { }
		Locker updateDataSourceLocker = new Locker();
		void InvalidateDesignTimeDataSourceCore() {
			if(updateDataSourceLocker.IsLocked)
				return;
			updateDataSourceLocker.Lock();
			Dispatcher.BeginInvoke(new Action(() => {
				DesignTimeAdorner.InvalidateDataSource();
				updateDataSourceLocker.Unlock();
			}));
		}
		internal void InvalidateDesignTimeDataSource() {
			OnItemsSourceChanged(ItemsSource, ItemsSource);
		}
		internal virtual void OnColumnsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if(DataView != null && DataView.LockDataColumnsChanged)
				return;
			if(BandsLayoutCore == null) {
				GetDataControlOriginationElement().NotifyCollectionChanged(this,
					dataControl => dataControl.ColumnsCore,
					column => CloneDetailHelper.CloneElement<BaseColumn>((ColumnBase)column),
					e);
			}
			InvalidateDependentColumns();
			UpdateColumnSummaries(TotalSummaryCore, e.Action, SummaryItemCollectionType.Total);
			UpdateColumnSummaries(GroupSummaryCore, e.Action, SummaryItemCollectionType.Group);
			UpdateDefaultSorting();
			InvalidateDesignTimeDataSourceCore();
			DesignTimeAdorner.UpdateDesignTimeInfo();
			if(e.OldItems != null)
				foreach(ColumnBase column in e.OldItems)
					ClearColumnFilter(column);
#if SILVERLIGHT
			columnsCollectionSynchronizer.SynchronizeResources(e);
#endif
			if(DataView != null)
				DataView.ApplySearchColumns();
			OnDataCollectionChanged(sender, e);
		}
		bool ShouldSynchronizeAfterAddColumns(IList addedColumns) {
			foreach(GridSortInfo item in SortInfoCore) {
				if((addedColumns != null) && (addedColumns.Contains(ColumnsCore[item.FieldName])))
					return true;
			}
			return false;
		}
		void OnTotalSummaryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			OnSummaryCollectionChanged(sender as ISummaryItemOwner, e, SummaryItemCollectionType.Total);
		}
		internal void OnGroupSummaryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			OnSummaryCollectionChanged(sender as ISummaryItemOwner, e, SummaryItemCollectionType.Group);			
		}
		void OnSummaryCollectionChanged(ISummaryItemOwner summaries, NotifyCollectionChangedEventArgs e, SummaryItemCollectionType collectionType) {
			UpdateColumnSummaries(summaries, e.Action, collectionType);
			OnSummaryChanged();
		}
		void UpdateColumnSummaries(ISummaryItemOwner summaries, NotifyCollectionChangedAction action, SummaryItemCollectionType collectionType) {
			if(ColumnsCore.IsLockUpdate || !NeedCalculateSummaries(summaries))
				return;
			if(summaries.Count == 0 || action == NotifyCollectionChangedAction.Reset || action == NotifyCollectionChangedAction.Remove) {
				SummaryItemCollectionUpdater.ClearColumnSummaries(collectionType, ColumnsCore);
			}
			SummaryItemCollectionUpdater.Update(DataView, collectionType, summaries, ColumnsCore);
		}
		bool NeedCalculateSummaries(ISummaryItemOwner summaries) {
			if(summaries == null) return false;
			if(ColumnsCore.Count > 0) return true;
			return summaries.FirstOrDefault(summaryItem => summaryItem.SummaryType == SummaryItemType.Count && summaryItem.Alignment != GridSummaryItemAlignment.Default) != null;
		}
		internal void OnDataCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if(ColumnsCore.IsLockUpdate) return;
			if(DataView != null)
				DataView.OnColumnCollectionChanged(e);
			OnDataChanged(NeedSynchronize || ShouldSynchronizeAfterAddColumns(e.NewItems), true);
			NeedSynchronize = false;
		}
		internal bool NeedSynchronize { get; set; }
		internal void OnSortInfoChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			syncronizationLocker.DoIfNotLocked(() => {
				if(ColumnsCore.IsLockUpdate) {
					NeedSynchronize = true;
					return;
				}
				if(sender == SortInfoCore)
					ClearGroupSummarySortInfo();
				OnDataChanged(true);
			});
		}
		internal virtual void ClearGroupSummarySortInfo() {
		}
		void OnSummaryChanged() {
			PerformDataResetLockedActionCore(delegate {
				SynchronizeSummaryIfAcitve();
				if(DataView != null)
					DataView.OnSummaryDataChanged();
			});
		}
		internal void SynchronizeSummaryIfAcitve() {
			if(IsActive)
				DataProviderBase.SynchronizeSummary();
		}
		void OnDataChanged(bool synchronize = false, bool rebuildVisibleColumns = true) {
			if(IsActive && synchronize && DataView != null)
				SynchronizeDataProvider();
			if(DataView != null)
				DataView.OnDataChanged(rebuildVisibleColumns);
			FilteredComponent.RaiseFilterColumnsChanged();
		}
		void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
#if DEBUGTEST
			if(DataView == null)
				throw new InvalidOperationException();
#endif
			if(DataView != null) {
				DataView.ActualColumnChooser = null;
				ResetMenus();
			}
		}
		void ResetMenus() {
			UpdateAllDetailDataControls((dataControl) => { if(dataControl.viewCore != null) dataControl.viewCore.ResetMenu(); });
		}
		protected virtual void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			ForceLoad();
			InitializeCurrentColumn();
			if(DataView != null) {
				DataView.InitMenu();
				DataView.UpdateIsKeyboardFocusWithinView();
			}
			if(AutomationPeer != null) 
				AutomationPeer.ResetDataPanelPeerCache();
		}
		protected override IEnumerator LogicalChildren {
			get {
				IEnumerator detailDescriptorEnumerator = IsOriginationDataControl() ? GetSingleObjectEnumerator(DetailDescriptorCore) : (new object[0]).GetEnumerator();
				var list = new List<object>();
				var enumerator = GetSingleObjectEnumerator(DataView);
				while(enumerator.MoveNext()) list.Add(enumerator.Current);
				enumerator = detailDescriptorEnumerator;
				while(enumerator.MoveNext()) list.Add(enumerator.Current);
				enumerator = ColumnsCore.GetEnumerator();
				while(enumerator.MoveNext()) list.Add(enumerator.Current);
				enumerator = GetSingleObjectEnumerator(BandsLayoutCore);
				while(enumerator.MoveNext()) list.Add(enumerator.Current);
				return list.GetEnumerator();
			}
		}
		internal static IEnumerator GetSingleObjectEnumerator(object obj) {
			return (obj != null ? new object[] { obj } : new object[0]).GetEnumerator();
		}
		protected internal void AddChild(FrameworkContentElement child) {
			AddLogicalChild(child);
			if(child is ColumnBase)
				OnAddColumn((ColumnBase)child);
		}
		protected internal void RemoveChild(FrameworkContentElement child) {
			if(child is ColumnBase)
				OnRemoveColumn((ColumnBase)child);
			RemoveLogicalChild(child);
		}
		void OnAddColumn(ColumnBase column) {
			UpdateColumnFilteringCounters(column.AllowColumnFiltering, null);
			UpdateColumnCellMergeCounter(null, column.AllowCellMerge);
		}
		void OnRemoveColumn(ColumnBase column) {
			UpdateColumnFilteringCounters(null, column.AllowColumnFiltering);
			UpdateColumnCellMergeCounter(column.AllowCellMerge, null);
			DependencyObjectExtensions.SetDataContext(column, null);
		}
		public override void OnApplyTemplate() {
#if SL
			if(DataView != null)
				RemoveLogicalChild(DataView);
#endif
			if(themeLoader != null) {
				themeLoader.Content = null;
			}
			themeLoader = GetTemplateChild("PART_ThemesLoader") as ContentControl;
			themeLoader.SetBinding(ContentControl.ContentProperty, new Binding("View") { Source = this });
#if SL
			if(viewCore != null)
				viewCore.InitMenu();
#endif
			base.OnApplyTemplate();
		}
		internal bool isSync = false;
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			SetDataSource();
			ForceCreateView();
			isSync = true;
			if(DataProviderBase.CollectionViewSource != null)
				CollectionViewSyncCurrentItem(DataProviderBase.CollectionViewSource);
			DataView.RebuildVisibleColumns();
			InitializeSelection(false);
		}
		void InitializeSelection(bool isEndInit) {
			if(isEndInit && !IsOriginationDataControl())
				return;
			Action<DataControlBase> initializationAction = dataControl => {
				if(!isEndInit && dataControl.IsOriginationDataControlCore())
					return;
				if(dataControl.IsOriginationDataControl())
					dataControl.SynchronizeCurrentItem();
				dataControl.DataView.SelectionStrategy.OnDataControlInitialized();
			};
			if(IsOriginationDataControl()) {
				UpdateAllInitializedOriginationDataControls(dataControl => initializationAction(dataControl));
			}
			else {
				initializationAction(this);
			}
		}
		void UpdateAllInitializedOriginationDataControls(Action<DataControlBase> action) {
			UpdateAllOriginationDataControls(dataControl => {
				bool isSync = true;
				dataControl.EnumerateThisAndOwnerDataControls(owner => {
					isSync &= owner.isSync;
				});
				if(isSync)
					action(dataControl);
			});
		}
		internal void ForceCreateView() {
			if(DataView == null)
				DataView = CreateDefaultView();
		}
		internal bool HasValue(DependencyProperty property) {
			return GetValue(property) != null || GetBindingExpression(property) != null;
		}
		internal void ReInitializeCurrentColumn() {
			CurrentColumn = null;
			InitializeCurrentColumn();
		}
		internal void InitializeCurrentColumn() {
			if(!HasValue(CurrentColumnProperty) && DataView.NavigationStyle == GridViewNavigationStyle.Cell) {
				foreach(ColumnBase visibleColumn in DataView.VisibleColumnsCore) {
					if(visibleColumn.AllowFocus) {
						CurrentColumn = visibleColumn;
						break;
					}
				}
			}
		}
		void SynchronizeCurrentColumn() {
			if(HasValue(CurrentColumnProperty)) {
				DataView.CurrentColumnChanged(null);
				DataView.SetValue(DataView.GetFocusedColumnProperty(), CurrentColumn);
			}
			else {
				CurrentColumn = (ColumnBase)DataView.GetValue(DataView.GetFocusedColumnProperty());
			}
		}
		void SynchronizeCurrentItem() {
			if(HasValue(CurrentItemProperty)) {
				OnCurrentItemChanged(null, false);
			}
			else {
				DataView.SetFocusOnCurrentControllerRow();
				UpdateCurrentItem();
			}
		}
		public DependencyObject GetRowState(int rowHandle, bool createNewIfNotExist) {
			return DataView.ViewBehavior.GetRowState(rowHandle) ?? DataProviderBase.GetRowState(rowHandle, createNewIfNotExist);
		}
		void OnDesignTimePropertyChanged() {
			InvalidateDesignTimeDataSourceCore();
		}
		int CoerceDesignTimeDataSourceRowCount(int baseValue) {
			return Math.Max(Math.Min(baseValue, 100), 0);
		}
		#region filtering
		protected internal virtual void DestroyFilterData() { }
		public void MergeColumnFilters(string filterString) {
			MergeColumnFilters(CriteriaOperator.TryParse(filterString));
		}
		public void ClearColumnFilter(string fieldName) {
			IDictionary<OperandProperty, CriteriaOperator> splitted = CriteriaColumnAffinityResolver.SplitByColumns(FilterCriteria);
			splitted.Remove(new OperandProperty(fieldName));
			FilterCriteria = GroupOperator.And(splitted.Values);
		}
		public void ClearColumnFilter(ColumnBase column) {
			ClearColumnFilter(column.FieldName);
		}
		internal CriteriaOperatorInfo GetInfoFromCriteriaOperator(CriteriaOperator criteriaOperator) {
			CriteriaOperator op = DisplayCriteriaGenerator.Process(new DisplayCriteriaHelper(this.DataView), criteriaOperator);
			string filterText = DataViewBase.GetFilterPanelText(op);
			return new CriteriaOperatorInfo(criteriaOperator, filterText);
		}
		public CriteriaOperator GetColumnFilterCriteria(string fieldName) {
			CriteriaOperator rv;
			CriteriaColumnAffinityResolver.SplitByColumns(FilterCriteria).TryGetValue(new OperandProperty(fieldName), out rv);
			return rv;
		}
		public CriteriaOperator GetColumnFilterCriteria(ColumnBase column) {
			if(column == null)
				return GetColumnFilterCriteria(String.Empty);
			if(String.IsNullOrEmpty(column.FieldName))
				return null;
			return GetColumnFilterCriteria(column.FieldName);
		}
		public string GetColumnFilterString(string fieldName) {
			CriteriaOperator op = GetColumnFilterCriteria(fieldName);
			return object.ReferenceEquals(op, null) ? string.Empty : op.ToString();
		}
		public string GetColumnFilterString(ColumnBase column) {
			return GetColumnFilterString(column.FieldName);
		}
		public void MergeColumnFilters(CriteriaOperator filterCriteria) {
			var merged = new Dictionary<OperandProperty, CriteriaOperator>(CriteriaColumnAffinityResolver.SplitByColumns(FilterCriteria));
			IDictionary<OperandProperty, CriteriaOperator> newFilterSplitted = CriteriaColumnAffinityResolver.SplitByColumns(filterCriteria);
			foreach(var affinity in newFilterSplitted) {
				merged[affinity.Key] = affinity.Value;
			}
			FilterCriteria = GroupOperator.And(merged.Values);
		}
		#endregion
		#region sort/group
		protected virtual void RebuildSortInfo() {
			List<ColumnBase> sortedColumns = new List<ColumnBase>();
			List<ColumnBase> groupedColumns = new List<ColumnBase>();
			List<ColumnBase> columnsWithSortOrderOnly = new List<ColumnBase>();
			foreach(ColumnBase column in ColumnsCore) {
				if(column.GroupIndexCore >= 0)
					groupedColumns.Add(column);
				else if(column.SortIndex >= 0)
					sortedColumns.Add(column);
				else if(column.SortOrder != ColumnSortOrder.None)
					columnsWithSortOrderOnly.Add(column);
			}
			sortedColumns.Sort((column1, column2) => Comparer<int>.Default.Compare(column1.SortIndex, column2.SortIndex));
			RebuildGroupedColumnsInfo(groupedColumns);
			sortedColumns.ForEach(ApplyColumnSortIndexWithoutLoadingCheck);
			columnsWithSortOrderOnly.ForEach(ApplyColumnSortOrderWithoutLoadingCheck);
		}
		internal void ApplyColumnSortOrderWithoutLoadingCheck(ColumnBase column) {
			ApplyGroupSortIndexCore(column, ApplyColumnSortOrderCore);
		}
		internal void ApplyColumnSortIndexWithoutLoadingCheck(ColumnBase column) {
			ApplyGroupSortIndexCore(column, col => SortByCore(col, GetActualSortOrder(col.SortOrder), col.SortIndex));
		}
		void ClearSortInfoForCorrespondingColumn(ColumnBase column) {
			if(column.SortIndex == -1)
				SortInfoCore.Remove(SortInfoCore[column.FieldName]);
		}
		protected virtual void RebuildGroupedColumnsInfo(List<ColumnBase> groupedColumns) {
			ReassignGroupedColumns(groupedColumns);
		}
		void ApplyColumnSortOrderCore(ColumnBase column) {
			if(column.GroupIndexCore >= 0)
				CroupByCore(column);
			else if(column.SortIndex >= 0)
				SortByCore(column, column.SortOrder, column.SortIndex);
			else
				SortByCore(column, column.SortOrder);
		}
		internal virtual void CroupByCore(ColumnBase column) {
		}
		internal void ApplyGroupSortIndexCore(ColumnBase column, Action<ColumnBase> columnAction) {
			if(updateSortIndexesLocker.IsLocked || ColumnsCore.IsLockUpdate)
				return;
			updateSortIndexesLocker.Lock();
			try {
				columnAction(column);
			}
			finally {
				updateSortIndexesLocker.Unlock();
			}
		}
		protected virtual void RequestSynchronizationCore() {
			IDataControllerValidationSupport validation = DataProviderBase as IDataControllerValidationSupport;
			if(validation != null)
				validation.OnControllerItemChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
			OnDataChanged(true, false);
			 if(!DataSourceChangingLocker.IsLocked) ClearAndNotify();
			 if(DataView == null)
				 return;
			DataView.UpdateSummariesIgnoreNullValues();
			UpdateAllowPartialGrouping();
		}
		protected internal virtual void UpdateLayoutCore() {
			if(LockUpdateLayout)
				return;
			DataProviderBase.InvalidateRowPropertyDescriptors();
			ClearAndNotify();
		}
		protected virtual void UpdateRowCore(int controllerRowHandle) {
			DataView.UpdateRowDataByRowHandle(controllerRowHandle, (rowData) => rowData.UpdateData());
			DataView.UpdateCellMergingPanels();
		}
		internal bool needsDataReset = false;
		protected virtual void OnClearAndNotifyBase() {
			if(AutomationPeer != null) AutomationPeer.ClearLogicalPeerCache();
		}
#if DEBUGTEST
		internal static long ClearAndNotifyFireCount = 0;
		internal static long DataResetFireCount = 0;
#endif
		protected virtual void ClearAndNotify() {
			OnClearAndNotifyBase();
			if(needsDataReset) {
#if DEBUGTEST
				ClearAndNotifyFireCount++;
#endif
				needsDataReset = false;
				dataResetLocker.DoIfNotLocked(delegate {
					if(DataView != null) {
#if DEBUGTEST
						DataResetFireCount++;
#endif
						DataView.OnDataReset();
					}
				});
			}
			else {
				dataResetLocker.DoIfNotLocked(delegate {
					if(DataView != null)
						DataView.PerformDataResetAction();
				});
			}
		}
		protected internal void OnColumnAdded(ColumnBase column) {
			UpdateSortingFromInvalidSortCache(column);
			UpdateGroupingFromInvalidGroupCache(column);
			OnUnboundColumnAddedOrRemoved(column);
		}
		protected internal void OnColumnRemoved(ColumnBase column) {
			OnUnboundColumnAddedOrRemoved(column);
		}
		internal bool IsDissalowSortingColumn(ColumnBase column) {
			return column.IsUnbound && DataProviderBase != null && DataProviderBase.IsICollectionView;
		}
		internal void UpdateUnboundColumnAllowSorting(ColumnBase column) {
			if(IsDissalowSortingColumn(column))
				column.AllowSorting = DefaultBoolean.False;
		}
		bool doUnboundChangedOnColumnsEndUpdate;
		void OnUnboundColumnAddedOrRemoved(ColumnBase column) {
			UpdateUnboundColumnAllowSorting(column);
			if(column.ShouldRepopulateColumns)
				OnColumnUnboundChangedPosponed();
		}
		protected internal void ApplyColumnSortIndex(ColumnBase column) {
			ApplyGroupSortIndexIfNotLoading(column, ApplyColumnSortIndexWithoutLoadingCheck, ClearSortInfoForCorrespondingColumn);
		}
		protected internal void ApplyColumnSortOrder(ColumnBase column) {
			UpdateDefaultSorting();
			ApplyGroupSortIndexIfNotLoading(column, ApplyColumnSortOrderWithoutLoadingCheck);
		}
		protected internal void OnColumnAdding(ColumnBase column) {
			if(column.GroupIndexCore >= 0) {
				ApplyGroupSortIndexIfNotLoadingCore(column);
			}
			else if(column.SortIndex >= 0) {
				ApplyGroupSortIndexIfNotLoading(column, ApplyColumnSortIndex);
			}
			else if(column.SortOrder != ColumnSortOrder.None)
				ApplyGroupSortIndexIfNotLoading(column, ApplyColumnSortOrder);
			if(column.VisibleIndex >= 0 && DataView != null && !IsLoading && !ColumnsCore.IsLockUpdate)
				DataView.ApplyColumnVisibleIndex(column);
			if(column.ColumnFilterMode == ColumnFilterMode.DisplayText)
				DestroyFilterData();
		}
		protected internal void OnColumnCollectionEndUpdate() {
			PopulateUnboundColumnsIfNeeded();
			if(!IsDeserializing && (viewCore == null || (!viewCore.LockDataColumnsChanged && !syncronizationLocker.IsLocked))) {
				syncronizationLocker.DoLockedAction(RebuildSortInfo);
				OnDataChanged(true);
			}
			else {
				DataView.Do(view => view.RebuildVisibleColumns());
			}
		}
		void PopulateUnboundColumnsIfNeeded() {
			if(doUnboundChangedOnColumnsEndUpdate) {
				doUnboundChangedOnColumnsEndUpdate = false;
				OnColumnUnboundChangedPosponed(false);
			}
		}
		internal virtual void ApplyGroupSortIndexIfNotLoadingCore(ColumnBase column) {
		}
		protected internal virtual void ResetGridChildPeersIfNeeded() {
			if(AutomationPeer == null) return;
			AutomationPeer.ResetPeers();
		}
		internal bool SyncActualSortInfo(IList<IColumnInfo> sortList, int groupCount) {
			if(sortList.Count == 0 && ActualSortInfoCore.Count == 0 && ActualGroupCountCore == 0 && groupCount == 0) return false;
			ActualSortInfoCore.BeginUpdate();
			try {
				ActualGroupCountCore = groupCount;
				ActualSortInfoCore.Clear();
				foreach(IColumnInfo info in sortList) {
					ActualSortInfoCore.Add(new GridSortInfo(info.FieldName, GridSortInfo.GetSortDirectionBySortOrder(info.SortOrder)));
				}
			}
			finally {
				ActualSortInfoCore.EndUpdate();
			}
			return true;
		}
		protected internal virtual void SetIsGrouped(bool value) { }
		internal void SynchronizeSortInfo(IList<IColumnInfo> sortList, int groupCount) {
			if(!SyncActualSortInfo(sortList, groupCount)) return;
			SetIsGrouped(ActualItemsSource == null ? SortInfoCore.GroupCountCore > 0 : DataProviderBase.GroupedColumnCount > 0);
			RebuildGroupSortIndexesAndGroupedColumns();
		}
		internal void RebuildGroupSortIndexesAndGroupedColumns() {
			updateSortIndexesLocker.Lock();
			try {
				SyncSortBySummaryInfo();
				List<ColumnBase> groupedColumnsList = SetGroupSortInfoAndBuildGroupedColumns(ColumnsCore, ActualSortInfo, ActualGroupCountCore);
				UpdateOriginalSortInfo();
				ReassignGroupedColumns(groupedColumnsList);
				if(DataView != null && ActualGroupCountCore != 0)
					DataView.RebuildVisibleColumns();
			}
			finally {
				updateSortIndexesLocker.Unlock();
			}
		}
		internal virtual void SyncSortBySummaryInfo() { }
		internal virtual void ReassignGroupedColumns(List<ColumnBase> groupedColumnsList) { }
		void UpdateOriginalSortInfo() {
			foreach(GridSortInfo item in ActualSortInfo) {
				GridSortInfo originalSortInfo = SortInfoCore[item.FieldName];
				originalSortInfo.SetGroupSortIndexes(item.SortIndex, item.GroupIndex);
			}
		}
		#endregion
		#region input events
		internal DataViewBase FindTargetView(object source) {
			return MasterDetailProvider.FindTargetView(DataView, source);
		}
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonDown(e);
			DataView.ProcessMouseLeftButtonDown(e);
		}
		protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonUp(e);
			FindTargetView(e.OriginalSource).ViewBehavior.ProcessMouseLeftButtonUp(e);
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			ProcessSearchControlFocus();
			DataView.MasterRootRowsContainer.FocusedView.ProcessIsKeyboardFocusWithinChanged();
		}
		protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnPreviewLostKeyboardFocus(e);
			DataView.MasterRootRowsContainer.FocusedView.ProcessPreviewLostKeyboardFocus(e);
		}
#if !SL
		protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseRightButtonDown(e);
			DataView.ProcessMouseRightButtonDown(e);
		}
#endif
		#endregion
		#region INotificationManager Members
		INotificationManager internalNotificationManager;
		void INotificationManager.SubscribeRequireMeasure(NotificationType notification, NotificationEventHandler eventHandler) {
			internalNotificationManager.SubscribeRequireMeasure(notification, eventHandler);
		}
		void INotificationManager.UnsubscribeRequireMeasure(NotificationType notification, NotificationEventHandler eventHandler) {
			internalNotificationManager.UnsubscribeRequireMeasure(notification, eventHandler);
		}
		void INotificationManager.AcceptNotification(DependencyObject sender, NotificationType notification) {
			if(IsLoading)
				return;
			internalNotificationManager.AcceptNotification(sender, notification);
		}
		#endregion
		#region serialization
		internal protected virtual bool OnDeserializeAllowProperty(AllowPropertyEventArgs e) {
			switch(DXSerializer.GetStoreLayoutMode(this)) {
				case StoreLayoutMode.None:
					return false;
				case StoreLayoutMode.UI:
					if(e.IsSerializing && IsObsoleteProperty(e.Property)) return false;
					return e.PropertyId == GridUIPropertyAttribute.IdUI ||
						(e.IsSerializing && e.PropertyId == GridSerializeAlwaysPropertyAttribute.IdSerializeAlways);
				default:
					return true;
			}
		}
		bool IsObsoleteProperty(PropertyDescriptor p) {
			return p.Attributes[typeof(ObsoleteAttribute)] != null;
		}
		#endregion
		#region clipboard
		public void CopySelectedItemsToClipboard() {
			DataView.Do(view => view.CopySelectedRowsToClipboardCore());
		}
		public void CopyCurrentItemToClipboard() {
			DataView.Do(view => view.CopyFocusedRowToClipboardCore());
		}
		public void CopyRowsToClipboard(IEnumerable<int> rows) {
			DataView.Do(view => view.CopyRowsToClipboardCore(rows));
		}
		public void CopyRangeToClipboard(int startRowHandle, int endRowHandle) {
			DataView.Do(view => view.CopyRangeToClipboardCore(startRowHandle, endRowHandle));
		}
		public void CopyToClipboard() {
			DataView.Do(view => view.SelectionStrategy.CopyToClipboard());
		}
		protected internal abstract bool RaiseCopyingToClipboard(CopyingToClipboardEventArgsBase e);
		protected internal virtual bool RaisePastingFromClipboard() {
			PastingFromClipboardEventArgs e = new PastingFromClipboardEventArgs(this, DataControlBase.PastingFromClipboardEvent);
			GetOriginationDataControl().RaiseEvent(e);
			return e.Handled;
		}
		#endregion
		#region selection
		public void BeginSelection() {
			DataView.Do(view => view.BeginSelectionCore());
		}
		public void EndSelection() {
			DataView.Do(view => view.EndSelectionCore());
		}
		public void SelectAll() {
			DataView.Do(view => view.SelectAllCore());
		}
		public void UnselectAll() {
			DataView.Do(view => view.ClearSelectionCore());
		}
		public void SelectRange(int startRowHandle, int endRowHandle) {
			DataView.Do(view => view.SelectRangeCore(startRowHandle, endRowHandle));
		}
		public void SelectItem(int rowHandle) {
			DataView.Do(view => view.SelectRowCore(rowHandle));
		}
		public void UnselectItem(int rowHandle) {
			DataView.Do(view => view.UnselectRowCore(rowHandle));
		}
		public int[] GetSelectedRowHandles() {
			return DataView.Return(view => view.GetSelectedRowHandlesCore(), null);
		}
		protected internal void RaiseSelectionChanged(DevExpress.Data.SelectionChangedEventArgs e) {
			RaiseSelectionChanged(DataView.CreateSelectionChangedEventArgs(e));
		}
		protected internal virtual void RaiseSelectionChanged(GridSelectionChangedEventArgs e) { }
		void OnSelectionModeChanged(MultiSelectMode oldSelectionMode) {
			if(DataView == null)
				return;
			if(IsMultiRowSelection(oldSelectionMode) && IsMultiRowSelection(SelectionMode))
				return;
			DataView.OnMultiSelectModeChanged();
		}
		void OnUseFieldNameForSerializationChanged() {
			if(!IsDeserializing) return;
			shouldRestoreUseFieldNameForSerialization = false;
		}
		bool IsMultiRowSelection(MultiSelectMode selectionMode) {
			return selectionMode == MultiSelectMode.MultipleRow || selectionMode == MultiSelectMode.Row;
		}
		#endregion
		protected internal abstract DataViewBase CreateDefaultView();
		internal protected abstract Type ColumnType { get; }
		internal protected abstract Type BandType { get; }
		internal protected abstract BandBase CreateBand();
		IDesignTimeAdornerBase designTimeAdorner;
		protected internal IDesignTimeAdornerBase DesignTimeAdorner {
			get { return designTimeAdorner ?? EmptyDesignTimeAdorner; }
			set { designTimeAdorner = value; }
		}
		protected internal abstract IDesignTimeAdornerBase EmptyDesignTimeAdorner { get; }
		protected internal abstract IList<DevExpress.Xpf.Grid.SummaryItemBase> GetGroupSummaries();
		internal abstract object GetGroupSummaryValue(int rowHandle, int summaryItemIndex);
		internal abstract SummaryItemBase CreateSummaryItem();
		#region sort API
		public void SortBy(string fieldName) {
			SortByCore(ColumnsCore[fieldName]);
		}
		public void ClearSorting() {
			SortInfoCore.ClearSorting();
		}
		internal void SortByCore(ColumnBase column) {
			SortByCore(column, defaultColumnSortOrder);
		}
		internal void SortByCore(ColumnBase column, ColumnSortOrder sortedOrder) {
			SortByCore(column, sortedOrder, SortInfoCore.Count);
		}
		internal void SortByCore(ColumnBase column, ColumnSortOrder sortedOrder, int sortedIndex) {
			SortInfoCore.SortByColumn(column.FieldName, sortedOrder, sortedIndex);
		}
		#endregion
		#region populate columns
		internal protected ColumnBase CreateColumn() {
			return (ColumnBase)Activator.CreateInstance(ColumnType);
		}
		public void PopulateColumns() {
			PopulateColumns(true, true);
		}
		internal void PopulateColumns(bool autoPopulate, bool canGenerateNewColumns, DataProviderBase dataProvider = null) {
			IEditingContext context = new RuntimeEditingContext(this);
			PopulateColumnsAndApplyAttributes(context.GetRoot(), GetCreateColumnsPopulatorCallback(autoPopulate), autoPopulate, canGenerateNewColumns, EnableSmartColumnsGeneration, true, dataProvider, GetCreateColumnsSweeperCallback(autoPopulate, canGenerateNewColumns));
		}
		Func<IModelItem, IModelItemCollection, bool, AllColumnsInfo, ColumnCreatorBase> GetCreateColumnsPopulatorCallback(bool autoPopulate) {
			return (dataControl, columns, canCreateNewColumns, columnsInfo) => {
				if(IsAddNewMode(autoPopulate))
					return new AddNewColumnsPopulator(dataControl, columns, columnsInfo);
				return canCreateNewColumns ? new RuntimeDefaultColumnsPopulator(dataControl, columns, columnsInfo) : new SmartColumnsPopulator(dataControl, columns, columnsInfo);
			};
		}
		Func<IModelItemCollection, ColumnSweeperBase> GetCreateColumnsSweeperCallback(bool autoPopulate, bool canGenerateNewColumns) {
			if(IsAddNewMode(autoPopulate) || !canGenerateNewColumns)
				return (columns) => new AddNewColumnSweeper(columns);
			return null;
		}
		bool IsContainsSmartColumns() {
			foreach(ColumnBase column in ColumnsCore) {
				if(column.IsSmart)
					return true;
			}
			return false;
		}
		internal bool ShouldPopulateColumns() {
			return CanAutoPopulateColumns || IsContainsSmartColumns();
		}
		bool IsAddNewMode(bool autoPopulate) {
			return autoPopulate && AutoGenerateColumns == AutoGenerateColumnsMode.AddNew;
		}
		Func<IModelItem, IModelItemCollection, AllColumnsInfo, ModelGridColumnsGeneratorBase> GetCreateColumnsGeneratorCallback(Func<IModelItem, IModelItemCollection, bool, AllColumnsInfo, ColumnCreatorBase> createPopulatorCallback, bool canGenerateNewColumns, bool enableSmartColumnsGeneration) {
			return (dataControl, columns, columnsInfo) => {
				return CreateSmartModelGridColumnsGenerator(createPopulatorCallback(dataControl, columns, canGenerateNewColumns, columnsInfo), !enableSmartColumnsGeneration, GetSkipColumnXamlGenerationProperties(dataControl));
			};
		}
		static bool GetSkipColumnXamlGenerationProperties(IModelItem dataControl) {
			return ((DataControlBase)dataControl.GetCurrentValue()).DesignTimeAdorner.SkipColumnXamlGenerationProperties;
		}
		internal void ClearAutoGeneratedColumns() {
			IList columns = BandsCore.Count == 0 ? (IList)ColumnsCore : ((BandBase)BandsCore[0]).ColumnsCore;
			ColumnBase[] copyColumns = new ColumnBase[columns.Count];
			columns.CopyTo(copyColumns, 0);
			foreach(ColumnBase column in copyColumns) {
				if(column.IsAutoGenerated)
					columns.Remove(column);
			}
		}
		internal abstract ModelGridColumnsGeneratorBase CreateSmartModelGridColumnsGenerator(ColumnCreatorBase creator, bool applyOnlyForSmartColumns, bool skipXamlGenerationProperties);
		internal void PopulateColumnsAndApplyAttributes(IModelItem dataControl, Func<IModelItem, IModelItemCollection, bool, AllColumnsInfo, ColumnCreatorBase> createPopulatorCallback, bool autoPopulate, bool canGenerateNewColumns, bool enableSmartColumnsGeneration, bool isRuntime, DataProviderBase dataProvider = null, Func<IModelItemCollection, ColumnSweeperBase> createSweeperCallback = null) {
			if(dataProvider == null)
				dataProvider = DataProviderBase;
			if(GetValue(ColumnsItemsAttachedBehaviorProperty) == null &&
				(GetValue(ColumnGeneratorTemplateProperty) != null ||
				GetValue(ColumnGeneratorStyleProperty) != null ||
				GetValue(ColumnGeneratorTemplateSelectorProperty) != null)
				)
				ItemsAttachedBehaviorCore<DataControlBase, ColumnBase>.OnItemsSourcePropertyChanged(this, new System.Windows.DependencyPropertyChangedEventArgs(), ColumnsItemsAttachedBehaviorProperty, ColumnGeneratorTemplateProperty, ColumnGeneratorTemplateSelectorProperty, ColumnGeneratorStyleProperty, grid => null, grid => grid.CreateColumn());
			ColumnsCore.BeginUpdate();
			try {
				ClearBands(BandsCore);
				IModelItem columnsOwner = BandsCore.Count == 0 ? dataControl : dataControl.Properties[SerializationPropertiesNames.Bands].Collection[0];
				IModelItemCollection columns = columnsOwner.Properties["Columns"].Collection;
				(createSweeperCallback != null ? createSweeperCallback(columns) : new DefaultColumnSweeper(columns, dataControl)).ClearColumns();
				dataProvider.RePopulateColumns();
				if(dataProvider.IsReady) {
					DataColumnInfo[] dataColumnInfoCollection = dataProvider.Columns.Cast<DataColumnInfo>().Where(x => x.Visible && !x.Unbound).ToArray();
#if DEBUGTEST
					foreach(DataColumnInfo dc in dataColumnInfoCollection) {
						bool isColumnCorrect = dc.Name == dc.PropertyDescriptor.Name &&
								dc.Type == dc.PropertyDescriptor.PropertyType &&
								dc.ReadOnly == dc.PropertyDescriptor.IsReadOnly &&
								dc.Caption == MasterDetailHelper.GetDisplayName(dc.PropertyDescriptor);
						if(!isColumnCorrect)
							throw new InvalidOperationException();
					}
#endif
					Func<IModelItem, IModelItemCollection, AllColumnsInfo, EditorsGeneratorBase> createGeneratorCallback = (dc, cols, columnsInfo) => (EditorsGeneratorBase)GetCreateColumnsGeneratorCallback(createPopulatorCallback, canGenerateNewColumns, enableSmartColumnsGeneration)(dc, cols, columnsInfo);
					BandsGenerator bandsGenerator = new BandsGenerator(dataControl, columnsOwner, BandType, createGeneratorCallback, isRuntime, true, null, enableSmartColumnsGeneration);
					LayoutGroupInfo layoutGroupInfo = new LayoutGroupInfo(null, GroupView.Group, Orientation.Horizontal);
					IEntityProperties properties = new ReflectionEntityProperties(dataColumnInfoCollection.Select(x => x.PropertyDescriptor), DataProviderBase.ItemType, true);
					EditorsSource.GenerateEditors(layoutGroupInfo, properties.AllProperties, bandsGenerator, null, GenerateEditorOptions.ForGridRuntime(), false);
					if(DataView != null)
						DataView.RebuildVisibleColumns();
				}
			}
			finally {
				ColumnsCore.EndUpdate();
			}
			if(autoPopulate)
				dataProvider.ScheduleAutoPopulateColumns();
			RaiseEvent(new RoutedEventArgs(ColumnsPopulatedEvent));
			RaiseEvent(new RoutedEventArgs(AutoGeneratedColumnsEvent));
		}
		internal void ClearBands(IBandsCollection bandsCollection) {
			List<BandBase> autoGeneratedBands = new List<BandBase>();
			foreach(BandBase band in bandsCollection) {
				if(band.IsAutoGenerated) {
					autoGeneratedBands.Add(band);
					continue;
				}
				ClearBands(band.BandsCore);
			}
			autoGeneratedBands.ForEach(b => bandsCollection.Remove(b));
		}
		protected internal virtual bool RaiseAutoGeneratingColumn(ColumnBase column) {
			AutoGeneratingColumnEventArgs e = new AutoGeneratingColumnEventArgs(column) { RoutedEvent = AutoGeneratingColumnEvent };
			RaiseEvent(e);
			return !e.Cancel;
		}
		protected internal virtual ColumnBase CreateColumnFromColumnGenerator(PropertyDescriptor item) {
			return ItemsAttachedBehaviorCore<DataControlBase, ColumnBase>.CreateItem(this, ColumnsItemsAttachedBehaviorProperty, new ColumnGeneratorItemContext(this, item));
		}
		protected internal virtual bool CanAutoPopulateColumns { get { return AutoGenerateColumns == AutoGenerateColumnsMode.AddNew || AutoGenerateColumns == AutoGenerateColumnsMode.RemoveOld || (ColumnsCore.Count == 0 && AutoGenerateColumns == AutoGenerateColumnsMode.KeepOld); } }
		internal void PopulateColumnsIfNeeded(DataProviderBase dataProvider = null) {
			syncPropertyLocker.DoLockedAction(() => {
				DesignTimeAdorner.RemoveGeneratedColumns(this);
				if(ShouldPopulateColumns())
					PopulateColumns(true, CanAutoPopulateColumns, dataProvider);
			});
		}
		#endregion
		protected virtual Type GetDesignTimeFilterColumnType(ColumnBase column) {
			if(!DesignerProperties.GetIsInDesignMode(this))
				return null;
			return DesignTimeAdorner.GetDefaultColumnType(column);
		}
		protected internal virtual FilterColumn GetFilterColumnFromGridColumn(ColumnBase column) {
			if(column == null)
				return null;
			GridFilterColumn filterColumn = DataView.CreateFilterColumn(column, SupportDomainDataSource, IsWcfSource());
			filterColumn.FieldName = column.FieldName;
			filterColumn.ColumnCaption = column.HeaderCaption;
			filterColumn.HeaderTemplate = (column.FilterEditorHeaderTemplate != null) ? column.FilterEditorHeaderTemplate : (((column.HeaderTemplate != null) || (DataView == null)) ? column.HeaderTemplate : DataView.ColumnHeaderTemplate);
			filterColumn.HeaderTemplateSelector = ((column.HeaderTemplateSelector != null) || (DataView == null)) ? column.HeaderTemplateSelector : DataView.ColumnHeaderTemplateSelector;
			filterColumn.ColumnType = (DataProviderBase.Columns[column.FieldName] != null) ? GetDesignTimeFilterColumnType(column) ?? DataProviderBase.Columns[column.FieldName].Type : typeof(string);
			if(column.ColumnFilterMode == ColumnFilterMode.DisplayText) {
				ComboBoxEditSettings editSettings = new ComboBoxEditSettings();
				editSettings.IsTextEditable = true;
				editSettings.AutoComplete = true;
				editSettings.ItemsSource = DataProviderBase.GetUniqueColumnValues(column, true, false);
				filterColumn.EditSettings = editSettings;
				filterColumn.ColumnType = typeof(string);
			}
			else {
				filterColumn.EditSettings = column.ActualEditSettings;
			}
			return filterColumn;
		}
		readonly string[] WcfSources = new string[] { "WcfServerModeSource", "WcfInstantFeedbackSource", "RiaInstantFeedbackSource" };
		internal bool IsWcfSource() {
			if(ItemsSource == null) return false;
			Type itemsSourceType = ItemsSource.GetType();
			foreach(string wcfSource in WcfSources) {
				if(itemsSourceType.Name == wcfSource) {
					return true;
				}
			}
			return false;
		}
		#region ValidationAttributes
		bool IValidationAttributeOwner.CalculateValidationAttribute(string columnName, int controllerRow) {
			ColumnBase column = ColumnsCore[columnName];
			if(column != null)
				return column.ActualShowValidationAttributeErrors;
			return false;
		}
		protected internal virtual string GetValidationAttributesErrorText(object value, string columnName, int rowHandle) {
			return DataProviderBase != null ? DataProviderBase.GetValidationAttributesErrorText(value, rowHandle, columnName) : null;
		}
		#endregion
		public event PropertyChangedEventHandler PropertyChanged;
		#region serialization
		internal BandedViewSerializationHelper BandSerializationHelper { get; set; }
		protected virtual void OnDeserializeClearCollection(XtraItemRoutedEventArgs e) {
			switch(e.Item.Name) {
				case SerializationPropertiesNames.Columns:
				case SerializationPropertiesNames.Bands:
					if(!GetAddNewColumns())
						BandSerializationHelper.ClearCollection(e);
					break;
				case SerializationPropertiesNames.MRUFilters:
					MRUFiltersInternal.Clear();
					break;
			}
		}
		protected virtual void OnDeserializeCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			switch(e.CollectionName) {
				case SerializationPropertiesNames.MRUFilters:
					XtraPropertyInfo filterTextProperty = e.Item.ChildProperties["FilterText"];
					CriteriaOperator op = GetCriteriaOperator(e.Item);
					CriteriaOperatorInfo info = new CriteriaOperatorInfo(op, filterTextProperty.Value.ToString());
					e.CollectionItem = info;
					MRUFiltersInternal.Add(info);
					break;
				case SerializationPropertiesNames.TotalSummary:
					SummaryItemBase totalSummaryItem = CreateSummaryItem();
					e.CollectionItem = totalSummaryItem;
					TotalSummaryCore.Add(totalSummaryItem);
					break;
				case SerializationPropertiesNames.SortInfo:
					GridSortInfo sortInfoItem = new GridSortInfo();
					e.CollectionItem = sortInfoItem;
					SortInfoCore.Add(sortInfoItem);
					break;
				case SerializationPropertiesNames.Columns:
					if(!GetRemoveOldColumns())
						OnDeserializeCreateColumn(e);
					break;
				case SerializationPropertiesNames.Bands:
					if(!GetRemoveOldColumns())
						OnDeserializeCreateBand(e);
					break;
			}
		}
		internal protected abstract bool GetAddNewColumns();
		internal protected abstract bool GetRemoveOldColumns();
		protected virtual void OnDeserializeStart(StartDeserializingEventArgs e) {
			SaveUseFieldNameForSerializationPropertyValue();
			DataView.BeginUpdateColumnsLayout();
			ColumnsCore.BeginUpdate();
			BandsCore.BeginUpdate();
			SortInfoCore.BeginUpdate();
			BeginUpdateGroupSummary();
			TotalSummaryCore.BeginUpdate();
			MRUFiltersInternal.BeginUpdate();
			ClearMRUFilter();
			BandSerializationHelper = new BandedViewSerializationHelper(this);
			IsDeserializing = true;
		}
#if DEBUGTEST
		public
#endif
		bool shouldRestoreUseFieldNameForSerialization = false;
#if DEBUGTEST
		public
#endif
		bool oldUseFieldNameForSerialization = false;
		void SaveUseFieldNameForSerializationPropertyValue() {
			oldUseFieldNameForSerialization = UseFieldNameForSerialization;
			shouldRestoreUseFieldNameForSerialization = true;
			UseFieldNameForSerialization = false;
		}
		void RestoreUseFieldNameForSerializationPropertyValue() {
			if(!shouldRestoreUseFieldNameForSerialization) return;
			UseFieldNameForSerialization = oldUseFieldNameForSerialization;
			shouldRestoreUseFieldNameForSerialization = false;
		}
		protected virtual void OnDeserializeEnd(EndDeserializingEventArgs e) {
			for(int i = SortInfoCore.Count - 1; i >= 0; i--) {
				GridSortInfo sortInfo = SortInfoCore[i];
				if(ColumnsCore[sortInfo.FieldName] == null) {
					OnDeserializeEndBeforeRemoveSummary(i);
					SortInfoCore.RemoveAt(i);
				}
			}
			PopulateUnboundColumnsIfNeeded();
			TotalSummaryCore.EndUpdate();
			EndUpdateGroupSummary();
			SortInfoCore.EndUpdate();
			BandsCore.EndUpdate();
			if(BandsLayoutCore != null)
				BandsLayoutCore.FillColumns();
			ColumnsCore.EndUpdate();
			IsDeserializing = false;
			MRUFiltersInternal.EndUpdate();
			DataView.EndUpdateColumnsLayout();
			BandSerializationHelper = null;
			if(BandsLayoutCore != null)
				BandsLayoutCore.UpdateBandsLayout();
			UpdateFilter();
			RestoreUseFieldNameForSerializationPropertyValue();
		}
		protected virtual void OnDeserializeEndBeforeRemoveSummary(int summaryIndex) { }
		protected virtual void BeginUpdateGroupSummary() { }
		protected virtual void EndUpdateGroupSummary() { }
		void OnCustomShouldSerializeProperty(CustomShouldSerializePropertyEventArgs e) {
			switch(e.Property.Name) {
				case SerializationPropertiesNames.Columns:
					e.CustomShouldSerialize = BandsLayoutCore == null;
					break;
				case SerializationPropertiesNames.Bands:
					e.CustomShouldSerialize = BandsLayoutCore != null;
					break;
				case SerializationPropertiesNames.UseFieldNameForSerialization:
					e.CustomShouldSerialize = true;
					break;
			}
		}
		protected virtual void OnDeserializeFindCollectionItem(XtraFindCollectionItemEventArgs e) {
			switch(e.CollectionName) {
				case SerializationPropertiesNames.Columns:
					BandsCore.Clear();
					BandSerializationHelper.FindColumn(e);
					break;
				case SerializationPropertiesNames.Bands:
					BandSerializationHelper.FindBand(e, BandsLayoutCore);
					break;
			}
		}
		internal void OnDeserializeCreateColumn(XtraCreateCollectionItemEventArgs e) {
			IList collection = e.Collection as IList;
			if(collection == null)
				return;
			ColumnBase column = CreateColumn();
			XtraPropertyInfo fieldNamePropertyInfo = e.Item.ChildProperties[ColumnBase.FieldNamePropertyName];
			if(fieldNamePropertyInfo != null)
				column.FieldName = fieldNamePropertyInfo.Value as string;
			collection.Add(column);
			if(!ColumnsCore.Contains(column))
				ColumnsCore.Add(column);
			e.CollectionItem = column;
		}
		internal void OnDeserializeCreateBand(XtraCreateCollectionItemEventArgs e) {
			IList collection = e.Collection as IList;
			if(collection == null)
				return;
			BandBase band = CreateBand();
			collection.Add(band);
			if(e.Owner == this) {
				if(BandsLayoutCore == null)
					BandsLayoutCore = CreateBandsLayout();
				band.Owner = BandsLayoutCore;
			}
			else
				band.Owner = e.Owner as IBandsOwner;
			e.CollectionItem = band;
		}
		void OnDeserializeAllowPropertyInternal(AllowPropertyEventArgs e) {
			e.Allow = OnDeserializeAllowProperty(e);
		}
		CriteriaOperator GetCriteriaOperator(XtraPropertyInfo xtraPropertyInfo) {
			XtraPropertyInfo filterOperatorProperty = xtraPropertyInfo.ChildProperties["FilterOperator"];
			if(filterOperatorProperty == null) {
				XtraPropertyInfo filterStringProperty = xtraPropertyInfo.ChildProperties["FilterString"];
				String filterString = filterStringProperty.ValueToObject(typeof(String)) as String;
				return CriteriaOperator.Parse(filterString);
			}
			return filterOperatorProperty.ValueToObject(typeof(CriteriaOperator)) as CriteriaOperator;
		}
		#endregion
#if SL
		internal static string BandsLayoutHolderName = Guid.NewGuid().ToString();
		internal IList BandsLayoutHolder {
			get {
				if(!Resources.Contains(BandsLayoutHolderName))
					Resources.Add(BandsLayoutHolderName, new StackPanel());
				return ((Panel)Resources[BandsLayoutHolderName]).Children;
			}
		}
		void UpdateBandsLayoutInLogicalTree(BandsLayoutBase oldValue, BandsLayoutBase newValue) {
			BandsLayoutHolder.Clear();
			if(BandsLayoutCore != null) 
				BandsLayoutHolder.Add(newValue);
		}
#else
		void UpdateBandsLayoutInLogicalTree(BandsLayoutBase oldValue, BandsLayoutBase newValue) {
			if(oldValue != null)
				RemoveLogicalChild(oldValue);
			if(newValue != null)
				AddLogicalChild(newValue);
		}
#endif
		protected internal abstract BandsLayoutBase BandsLayoutCore { get; set; }
		IBandsCollection bandsCore;
		internal IBandsCollection BandsCore {
			get {
				if(bandsCore == null) {
					bandsCore = CreateBands();
					bandsCore.CollectionChanged += BandsCore_CollectionChanged;
				}
				return bandsCore;
			}
		}
		void BandsCore_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ValidateMasterDetailConsistency();
			if(BandsLayoutCore != null) BandsLayoutCore.OnGridControlBandsChanged(e);
			if(BandsCore.Count != 0) {
				if(BandsLayoutCore == null)
					BandsLayoutCore = CreateBandsLayout();
			} else if(GetOriginationDataControl().BandsCore.Count == 0){
				BandsLayoutCore = null;
			}
		}
		protected abstract BandsLayoutBase CreateBandsLayout();
		protected abstract IBandsCollection CreateBands();
		protected void OnBandsLayoutChanged(BandsLayoutBase oldValue, BandsLayoutBase newValue) {
			ColumnsCore.BeginUpdate();
			if(oldValue != null)
				oldValue.DataControl = null;
			if(newValue != null)
				newValue.DataControl = this;
			ColumnsCore.EndUpdate();
			UpdateBandsLayoutInLogicalTree(oldValue, newValue);
			UpdateViewActualColumnChooserTemplate();
			UpdateBandsLayoutProperties();
			NotifyBandsLayoutChanged();
		}
		void NotifyBandsLayoutChanged() {
			if(DataView != null)
				DataView.ViewBehavior.NotifyBandsLayoutChanged();
		}
		void UpdateBandsLayoutProperties() {
			if(DataView != null)
				DataView.ViewBehavior.UpdateBandsLayoutProperties();
		}
		internal void UpdateViewActualColumnChooserTemplate() {
			if(viewCore != null) {
				viewCore.UpdateActualColumnChooserTemplate();
				viewCore.UpdateColumnChooserCaption();
			}
		}
		#region CurrentItem
		internal bool HasCurrentItemBinding { get { return HasDefaultOrTwoWayBinding(this, CurrentItemProperty) || HasDefaultOrTwoWayBinding(DataView, DataViewBase.FocusedRowProperty) || (SelectionMode == MultiSelectMode.None && HasDefaultOrTwoWayBinding(this, SelectedItemProperty)); } }
		internal bool AllowUpdateCurrentItem {
			get {
				if(AllowUpdateTwoWayBoundPropertiesOnSynchronization)
					return !updateCurrentItemWasLocked || (DataView != null && DataView.IsSynchronizedWithCurrentItem && DataProviderBase.CollectionViewSource != null);
				return !DataSourceChangingLocker.IsLocked || !HasCurrentItemBinding;
			}
		}
		internal bool AllowUpdateSelectedItems { get { return !DataSourceChangingLocker.IsLocked || AllowUpdateTwoWayBoundPropertiesOnSynchronization || GetBindingExpression(SelectedItemsProperty) == null; } }
		bool HasDefaultOrTwoWayBinding(FrameworkElement element, DependencyProperty property) {
			BindingExpression bindingExpression = element.GetBindingExpression(property);
			if(bindingExpression == null || bindingExpression.ParentBinding == null)
				return false;
			BindingMode bindingMode = bindingExpression.ParentBinding.Mode;
#if !SL
			return bindingMode == BindingMode.Default || bindingMode == BindingMode.TwoWay;
#else
			return bindingMode == BindingMode.TwoWay;
#endif
		}
		internal void UpdateCurrentItem() {
			if(!isSync || IsOriginationDataControlCore()) return;
			if(DataSourceChangingLocker.IsLocked && HasCurrentItemBinding) { 
				updateCurrentItemWasLocked = true;
			}
			else {
				object currentItem = null;
				if(!DataProviderBase.IsGroupRowHandle(DataView.FocusedRowHandle))
					currentItem = DataProviderBase.GetRowValue(DataView.FocusedRowHandle);
				SetCurrentItem(currentItem);
				updateCurrentItemWasLocked = false;
			}
		}
		void OnCurrentItemChanged(object oldValue, bool raiseEvent) {
			if(IsOriginationDataControlCore()) {
				CurrentItemChangedLocker.DoLockedActionIfNotLocked(() => {
					SetCurrentItemInOriginationGrid();
				});
				return;
			}
			if(DataView != null) {
				if(!ReferenceEquals(CurrentItem, DataView.GetValue(DataViewBase.FocusedRowProperty)))
					DataView.SetValue(DataViewBase.FocusedRowProperty, CurrentItem);
				CurrentItemChangedLocker.DoLockedActionIfNotLocked(delegate {
					DataView.SetFocusOnCurrentItem();
				});
			}
			UpdateCurrentCellValue();
			if(!CurrentItemChangedLocker.IsLocked && raiseEvent)
				RaiseCurrentItemChanged(oldValue, CurrentItem);
		}
		internal void RaiseCurrentItemChanged(object oldValue, object newValue) {
			UpdateCurrentItemInOriginationGrid();
			if(!IsOriginationDataControlCore())
				RaiseEventInOriginationGrid(new CurrentItemChangedEventArgs(this, oldValue, newValue) { RoutedEvent = DataControlBase.CurrentItemChangedEvent });
		}
		void SetCurrentItem(object newValue) {
			if(CurrentItem == newValue) {
				if(!ReferenceEquals(DataView.GetValue(DataViewBase.FocusedRowProperty), CurrentItem))
					DataView.SetValue(DataViewBase.FocusedRowProperty, CurrentItem);
				return;
			}
			CurrentItemChangedLocker.DoLockedActionIfNotLocked(delegate {
				object oldCurrentItem = CurrentItem;
				SetCurrentItemCore(newValue);
				RaiseCurrentItemChanged(oldCurrentItem, CurrentItem);
			});
		}
		internal void SetCurrentItemCore(object currentItem) {
			SetCurrentValue(CurrentItemProperty, currentItem);
		}
		void ReturnCurrentItem() {
			if(DataView != null && updateCurrentItemWasLocked)
				DataView.SetFocusOnCurrentItem();
		}
		object CoerceCurrentItem(object newValue) {
			if(newValue != CurrentItem && DataView != null && DataView.FocusedRowHandle != DataControlBase.NewItemRowHandle)
				DataView.HideEditor(false);
			return newValue;
		}
		#endregion
		#region SelectedItem
		void OnSelectedItemChanged(object oldValue) {
			if(DataView != null)
				DataView.SelectionStrategy.OnSelectedItemChanged(oldValue);
		}
		protected internal virtual void RaiseSelectedItemChanged(object oldItem) {
			RaiseEventInOriginationGrid(new SelectedItemChangedEventArgs(this, oldItem, SelectedItem) { RoutedEvent = DataControlBase.SelectedItemChangedEvent });
		}
		#endregion
		#region SelectedItems
		void OnSelectedItemsChanged(IList oldValue) {
			if(oldValue is INotifyCollectionChanged)
				((INotifyCollectionChanged)oldValue).CollectionChanged -= OnSelectedItemsCollectionChanged;
			else if(oldValue is IBindingList)
				((IBindingList)oldValue).ListChanged -= OnSelectedItemsListChanged;
			if(SelectedItems is INotifyCollectionChanged)
				((INotifyCollectionChanged)SelectedItems).CollectionChanged += OnSelectedItemsCollectionChanged;
			else if(SelectedItems is IBindingList)
				((IBindingList)SelectedItems).ListChanged += OnSelectedItemsListChanged;
			ProcessSelectedItemsChanged();
		}
		void ProcessSelectedItemsChanged() {
			if(DataView != null)
				DataView.SelectionStrategy.ProcessSelectedItemsChanged();
		}
		void OnSelectedItemsListChanged(object sender, ListChangedEventArgs e) {
			NotifyCollectionChangedEventArgs eventArgs = null;
			switch(e.ListChangedType) {
				case ListChangedType.ItemAdded:
					eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, SelectedItems[e.NewIndex], e.NewIndex);
					break;
				case ListChangedType.ItemChanged:
				case ListChangedType.ItemDeleted:
				case ListChangedType.Reset:
					eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
					break;
			}
			OnSelectedItemsCollectionChanged(this, eventArgs);
		}
		void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(DataView != null)
				DataView.SelectionStrategy.OnSelectedItemsChanged(e);
		}
		internal void ResetSelectedItems(IList items) {
			DataView.SelectionStrategy.SelectItems(items);
		}
		#endregion
		#region MasterDetailSelection
		internal void SelectAllMasterDetail() {
			DataView.SelectionStrategy.SelectAllMasterDetail();
		}
		internal void ClearMasterDetailSelection() {
			Action<DataControlBase> clearSelectionAction = detail => {
				if(detail.SelectionMode != MultiSelectMode.MultipleRow)
					detail.UnselectAll();
			};
			UpdateAllDetailDataControls(clearSelectionAction, clearSelectionAction);
		}
		void UpdateCurrentItemInOriginationGrid() {
			DataControlBase originationDataControl = GetOriginationDataControl();
			if(originationDataControl == this)
				return;
			object originationCurrentItem = null;
			DataControlBase focusedDataControl = DataView.FocusedView.DataControl;
			if(focusedDataControl == null)
				return;
			if(focusedDataControl.GetOriginationDataControl() == originationDataControl)
				originationCurrentItem = focusedDataControl.CurrentItem;
			if(!originationDataControl.CurrentItemChangedLocker.IsLocked)
				originationDataControl.SetCurrentItem(originationCurrentItem);
		}
		void SetCurrentItemInOriginationGrid() {
			if(DataView != null && DataView.IsDesignTime)
				return;
			DataControlBase dataControl = DataControlOwner.FindDetailDataControlByRow(CurrentItem);
			if(dataControl != null) {
				dataControl.CurrentItem = CurrentItem;
			}
			else {
				DataControlBase focusedDataControl = GetRootDataControl().DataView.FocusedView.DataControl;
				if(focusedDataControl.GetOriginationDataControl() == this)
					focusedDataControl.SetCurrentItemCore(null);
			}
		}
		#endregion
		void OnCurrentColumnChanged(GridColumnBase oldValue, GridColumnBase newValue) {
			if(DataView != null) {
				if(!ReferenceEquals(CurrentColumn, (ColumnBase)DataView.GetValue(DataView.GetFocusedColumnProperty())))
					DataView.SetValue(DataView.GetFocusedColumnProperty(), CurrentColumn);
				DataView.CurrentColumnChanged(oldValue);
				UpdateCurrentCellValue();
			}
			RaiseCurrentColumnChanged(oldValue, newValue);
		}
		ColumnBase CoerceCurrentColumn(ColumnBase newValue) {
			if(DataView == null || !DataView.EditFormManager.IsEditFormVisible && DataView.RequestUIUpdate())
				return newValue;
			return (ColumnBase)this.GetCoerceOldValue(CurrentColumnProperty);
		}
		void RaiseCurrentColumnChanged(GridColumnBase oldValue, GridColumnBase newValue) {
			RaiseEventInOriginationGrid(new CurrentColumnChangedEventArgs(this, oldValue, newValue) { RoutedEvent = CurrentColumnChangedEvent });
		}
		internal void UpdateCurrentCellValue() {
			if(DataView == null || CurrentColumn == null || CurrentItem == null)
				CurrentCellValue = null;
			else
				CurrentCellValue = GetCellValue(DataView.FocusedRowHandle, CurrentColumn.FieldName);
		}
		#region Serialization API
		public void SaveLayoutToStream(Stream stream) {
			SaveLayoutCore(stream);
		}
		public void RestoreLayoutFromStream(Stream stream) {
			RestoreLayoutCore(stream);
		}
		public void SaveLayoutToXml(string path) {
			SaveLayoutCore(path);
		}
		public void RestoreLayoutFromXml(string path) {
			RestoreLayoutCore(path);
		}
		void SaveLayoutCore(object path) {
			DXSerializer.SerializeSingleObject(this, path, GetSerializationAppName());
		}
		void RestoreLayoutCore(object path) {
			DXSerializer.DeserializeSingleObject(this, path, GetSerializationAppName());
		}
		protected abstract string GetSerializationAppName();
		#endregion Serialization
		#region MasterDetail
		internal virtual void ThrowNotSupportedInDetailException() {
			throw new NotSupportedInMasterDetailException(NotSupportedInMasterDetailException.OnlyGridControlSupported);
		}
		internal void ValidateMasterDetailConsistency() {
			MasterDetailProvider.ValidateMasterDetailConsistency();
			DataControlParent.ValidateMasterDetailConsistency(this);
			DataControlOwner.ValidateMasterDetailConsistency();
		}
		internal virtual void ThrowNotSupportedInMasterDetailException() { }
		MasterDetailProviderBase masterDetailProvider = NullDetailProvider.Instance;
#if DEBUGTEST
		public
#else
		internal 
#endif
		MasterDetailProviderBase MasterDetailProvider { get { return masterDetailProvider; } }
		internal abstract DetailDescriptorBase DetailDescriptorCore { get; }
		[CloneDetailMode(CloneDetailMode.Force)]
		public DetailDescriptorBase OwnerDetailDescriptor {
			get { return (DetailDescriptorBase)GetValue(OwnerDetailDescriptorProperty); }
			private set { this.SetValue(OwnerDetailDescriptorPropertyKey, value); }
		}
		protected void OnDetailDescriptorChanged(DetailDescriptorBase oldValue) {
			if(CanAssignDetailDescriptorOwner(oldValue)) {
				RemoveLogicalChild(oldValue);
				oldValue.Owner = null;
			}
			if(CanAssignDetailDescriptorOwner(DetailDescriptorCore)) {
				AddLogicalChild(DetailDescriptorCore);
			}
			UpdateMasterDetailProvider();
			UpdateHasDetailViews();
			if(DataView != null)
				DataView.RootView.OnDataReset();
		}
		bool CanAssignDetailDescriptorOwner(DetailDescriptorBase detailDescriptor) {
			return (detailDescriptor != null) && detailDescriptor.Owner.CanAssignTo(this);
		}
		void UpdateMasterDetailProvider() {
			if((DetailDescriptorCore != null) && (DataView != null)) {
				masterDetailProvider = DataView.ViewBehavior.CreateMasterDetailProvider();
			} else {
				MasterDetailProvider.OnDetach();
				masterDetailProvider = NullDetailProvider.Instance;
			}
			if(CanAssignDetailDescriptorOwner(DetailDescriptorCore)) {
				DetailDescriptorCore.Owner = MasterDetailProvider as IDetailDescriptorOwner;
			}
		}
		protected virtual void UpdateHasDetailViews() { }
		internal void ChangeMasterRowExpanded(object parameter) {
			MasterDetailProvider.ChangeMasterRowExpanded(ObjectToInt(parameter) ?? DataControlBase.InvalidRowHandle);
		}
		internal void SetMasterRowExpanded(object parameter, bool expanded) {
			Int32? handle = ObjectToInt(parameter);
			if(handle.HasValue && expanded != MasterDetailProvider.IsMasterRowExpanded(handle.Value))
				MasterDetailProvider.ChangeMasterRowExpanded(handle.Value);
		}
		Int32? ObjectToInt(object obj) { 
			int handle = 0;
			if(Int32.TryParse(obj.ToString(), out handle))
				return handle;
			else
				return null;
		}
		internal void EnumerateThisAndParentDataControls(Action<DataControlBase, int> action, int visibleIndex) {
			action(this, visibleIndex);
			DataControlParent.EnumerateParentDataControls(action);
		}
		internal void EnumerateThisAndParentDataControls(Action<DataControlBase> action) {
			EnumerateThisAndParentDataControls((dataControl, index) => action(dataControl), -1);
		}
		internal void EnumerateThisAndOwnerDataControls(Action<DataControlBase> action) {
			action(this);
			DataControlOwner.EnumerateOwnerDataControls(action);
		}
		internal bool FindViewAndVisibleIndexByScrollIndex(int scrollIndex, bool forwardIfServiceRow, out DataViewBase targetView, out int targetVisibleIndex) {
			return GetRootDataControl().FindViewAndVisibleIndexByScrollIndexCore(scrollIndex, forwardIfServiceRow, out targetView, out targetVisibleIndex);
		}
		internal bool FindViewAndVisibleIndexByScrollIndexCore(int commonScrollIndex, bool forwardIfServiceRow, out DataViewBase targetView, out int targetVisibleIndex) {
			MasterRowScrollInfo masterRowScrollInfo = MasterDetailProvider.CalcMasterRowScrollInfo(commonScrollIndex);
			if(masterRowScrollInfo != null) {
				int localVisibleIndex = DataProviderBase.ConvertScrollIndexToVisibleIndex(masterRowScrollInfo.StartScrollIndex, DataView.AllowFixedGroupsCore);
				if(masterRowScrollInfo.WholeDetailScrolledOut) {
					targetView = DataView;
					targetVisibleIndex = localVisibleIndex;
					return true;
				} else {
					if(!MasterDetailProvider.FindViewAndVisibleIndexByScrollIndex(masterRowScrollInfo.DetailStartScrollIndex, localVisibleIndex, forwardIfServiceRow, out targetView, out targetVisibleIndex)) {
						targetView = DataView;
						if(forwardIfServiceRow) {
							if(localVisibleIndex == VisibleRowCount - 1) {
								targetView = null;
								targetVisibleIndex = -1;
								return false;
							} else {
								targetVisibleIndex = localVisibleIndex + 1;
							}
						} else {
							targetVisibleIndex = localVisibleIndex;
						}
					}
					return true;
				}
			}
			targetView = null;
			targetVisibleIndex = -1;
			return false;
		}
		internal int FindFirstInnerChildScrollIndex(int visibleIndex) {
			int rowHandle = GetRowHandleByVisibleIndexCore(visibleIndex);
			if(DataView.AllowFixedGroupsCore) {
				while(visibleIndex < VisibleRowCount - 1) {
					if(DataProviderBase.IsGroupRowHandle(rowHandle) && DataProviderBase.IsGroupRowExpanded(rowHandle)) {
						visibleIndex++;
						rowHandle = GetRowHandleByVisibleIndexCore(visibleIndex);
					} else {
						break;
					}
				}
			}
			return DataView.ConvertVisibleIndexToScrollIndex(visibleIndex) - MasterDetailProvider.CalcVisibleDetailRowsCountForRow(rowHandle);
		}
		internal int FindFirstInnerChildScrollIndex() {
			int scrollIndexWithDetails = 0;
			DataControlParent.EnumerateParentDataControls((dataControl, index) => {
				int scrollIndex = dataControl.DataProviderBase.ConvertVisibleIndexToScrollIndex(index, dataControl.DataView.AllowFixedGroupsCore);
				scrollIndexWithDetails += scrollIndex;
				scrollIndexWithDetails += dataControl.MasterDetailProvider.CalcVisibleDetailRowsCountBeforeRow(scrollIndex);
			});
			return scrollIndexWithDetails;
		}
		internal bool IsExpandedFixedRow(int visibleIndex) {
			int rowHandle = GetRowHandleByVisibleIndexCore(visibleIndex);
			bool result = false;
			if(DataProviderBase.IsGroupRowHandle(rowHandle)) {
				result = DataView.AllowFixedGroupsCore && DataProviderBase.IsGroupRowExpanded(rowHandle);
			} else {
				result = MasterDetailProvider.IsMasterRowExpanded(rowHandle);
			}
			return result;
		}
		internal DataViewBase FindLastInnerDetailView() {
			if(VisibleRowCount == 0 && !DataView.IsNewItemRowVisible) {
				return null;
			}
			DataViewBase lastInnerDetailView = MasterDetailProvider.FindLastInnerDetailView(VisibleRowCount - 1);
			if(lastInnerDetailView == null) {
				lastInnerDetailView = DataView;
			}
			return lastInnerDetailView;
		}
		internal bool FindNextOuterMasterRow(int visibleIndex, out DataViewBase targetView, out int targetVisibleIndex) {
			if(visibleIndex < VisibleRowCount - 1) {
				targetView = DataView;
				targetVisibleIndex = visibleIndex + 1;
				return true;
			}
			return DataControlParent.FindNextOuterMasterRow(out targetView, out targetVisibleIndex);
		}
		List<DataControlBase> detailClones = new List<DataControlBase>();
		internal List<DataControlBase> DetailClones { get { return detailClones; } }
		internal Locker syncPropertyLocker = new Locker();
		Locker cloneDetailLocker = new Locker();
		internal Locker CloneDetailLocker { get { return cloneDetailLocker; } }
		internal DataControlBase CloneDetail(MasterNodeContainer masterNodeContainer, MasterRowsContainer masterRowsContainer, object dataContext, BindingBase itemsSourceBinding, DataControlDetailInfo parent, bool cloneColumns = true) {
			ForceCreateView();
			if(dataControlOriginationElement == null)
				dataControlOriginationElement = new DataControlOriginationElement(this);
			DataControlBase clone = null;
			syncPropertyLocker.DoLockedAction(() => {
				clone = CloneDetailHelper.CreateElement<DataControlBase>(this, new object[] { dataControlOriginationElement });
				parent.DataControl = clone;
				parent.ForceCreateContainers();
				CloneDetailHelper.CloneElement<DataControlBase>(this, clone, dataControl => {
					if(cloneColumns) {
						if(BandsCore.Count > 0)
							BandsLayoutCore.CloneBandsCollection(dataControl.BandsCore);
						else
							CloneDetailHelper.CloneCollection<BaseColumn>(ColumnsCore, dataControl.ColumnsCore);
					}
					CloneDetailHelper.CloneCollection<SummaryItemBase>(TotalSummaryCore, dataControl.TotalSummaryCore);
					CloneDetailHelper.CloneCollection<SummaryItemBase>(GroupSummaryCore, dataControl.GroupSummaryCore);
					dataControl.DataView = CloneDetailHelper.CloneElement<DataViewBase>(DataView, null, null, new object[] { masterNodeContainer, masterRowsContainer, DataControlOwner });
					dataControl.DataContext = dataContext;
					dataControl.DataControlParent = parent;
					if(itemsSourceBinding != null) {
						dataControl.SetBinding(DataControlBase.ItemsSourceProperty,
	#if SL
						(Binding)
	#endif
						itemsSourceBinding);
					}
				}, dataControl => { return dataControl.CloneDetailLocker; });
			});
			clone.viewCore.UpdateUseLightweightTemplates();
			clone.FinalizeClonedDetail();
			detailClones.Add(clone);
			UpdateAllDetailViewIndents();
			return clone;
		}
		internal DataControlBase CloneDetailForPrint(MasterNodeContainer masterNodeContainer, MasterRowsContainer masterRowsContainer) {
			ForceCreateView();
			if(dataControlOriginationElement == null)
				dataControlOriginationElement = new DataControlOriginationElement(this);
			DataControlBase clone = null;
			DataControlBase result = null;
			syncPropertyLocker.DoLockedAction(() => {
				clone = CloneDetailHelper.CreateElement<DataControlBase>(this, new object[] { dataControlOriginationElement });
				CloneDetailHelper.CloneElement<DataControlBase>(this, clone, dataControl => {
					CloneDetailHelper.CloneCollection<BaseColumn>(ColumnsCore, dataControl.ColumnsCore);
					CloneDetailHelper.CloneCollection<SummaryItemBase>(TotalSummaryCore, dataControl.TotalSummaryCore);
					CloneDetailHelper.CloneCollection<SummaryItemBase>(GroupSummaryCore, dataControl.GroupSummaryCore);
					dataControl.DataView = CloneDetailHelper.CloneElement<DataViewBase>(DataView, null, null, new object[] { masterNodeContainer, masterRowsContainer, DataControlOwner });
					result = dataControl;
				}, dataControl => { return dataControl.CloneDetailLocker; });
			});
			clone.FinalizeClonedDetail();
			return result;
		}
		protected virtual void CloneGroupSummarySortInfo(DataControlBase dataControl) { }
		internal void CopyToDetail(DataControlBase clone) {
			syncPropertyLocker.DoLockedAction(() => {
				CloneDetailHelper.CopyToElement<DataControlBase>(this, clone, dataControl => {
					if(BandsCore.Count > 0) {
						BandsLayoutCore.CopyBandCollection(dataControl.BandsCore);
					} else
						CloneDetailHelper.CopyToCollection<BaseColumn>(ColumnsCore, dataControl.ColumnsCore);
					CloneDetailHelper.CopyToCollection<SummaryItemBase>(TotalSummaryCore, dataControl.TotalSummaryCore);
					CloneDetailHelper.CopyToCollection<SummaryItemBase>(GroupSummaryCore, dataControl.GroupSummaryCore);
					DataView.ViewBehavior.CopyToDetail(dataControl);
					CloneDetailHelper.CopyToElement<DataViewBase>(DataView, dataControl.DataView);
				});
				CloneGroupSummarySortInfo(clone);
			});
		}
		void FinalizeClonedDetail() {
			DataView.FinalizeClonedDetail();
		}
#if DEBUGTEST
		internal int originationElementNotifyChangedCount = 0;
#endif
		class DataControlOriginationElement : IDataControlOriginationElement {
			readonly DataControlBase dataControl;
			public DataControlOriginationElement(DataControlBase dataControl) {
				this.dataControl = dataControl;
			}
			void IDataControlOriginationElement.NotifyPropertyChanged(DataControlBase sourceControl, DependencyProperty property, Func<DataControlBase, DependencyObject> getTarget, Type baseComponentType) {
				PerformSyncAction(() => {
#if DEBUGTEST
					dataControl.originationElementNotifyChangedCount++;
#endif
					DependencyObject target = getTarget(dataControl);
					if(target == null)
						return;
					Type targetType = target.GetType();
					Type propertyOwnerType = property.GetOwnerType();
					if(!propertyOwnerType.IsAssignableFrom(targetType)) {
						if(!CloneDetailHelper.IsKnownAttachedProperty(property))
							return;
					}
					PropertyDescriptor propertyDescriptor = CloneDetailHelper.GetCloneProperties(targetType, baseComponentType)[property.GetName()];
					if(propertyDescriptor != null) {
						DependencyObject sourceElement = getTarget(sourceControl);
						object newValue = sourceElement.GetValue(property);
						DataControlOriginationElementHelper.EnumerateDependentElemets<DependencyObject>(sourceControl, getTarget,
							dObject => CloneDetailHelper.SetClonePropertyValue(target, propertyDescriptor, newValue, dObject));
					}
				});
			}
			void IDataControlOriginationElement.NotifyCollectionChanged(DataControlBase sourceControl, Func<DataControlBase, IList> getCollection, Func<object, object> convertAction, NotifyCollectionChangedEventArgs e) {
				PerformSyncAction(() => {
					IList sourceCollection = getCollection(sourceControl);
					DataControlOriginationElementHelper.EnumerateDependentElemets<IList>(sourceControl, getCollection, 
						list => ((ILockable)list).BeginUpdate());
					DataControlOriginationElementHelper.EnumerateDependentElemets<IList>(sourceControl, getCollection, 
						list => SyncCollectionHelper.SyncCollection(e, list, sourceCollection, convertAction), 
						list => list.Clear());
					DataControlOriginationElementHelper.EnumerateDependentElemets<IList>(sourceControl, getCollection, 
						list => ((ILockable)list).EndUpdate());
				});
			}
			void IDataControlOriginationElement.NotifyBeginInit(DataControlBase sourceControl, Func<DataControlBase, ISupportInitialize> getTarget) {
				PerformSyncAction(() => {
					DataControlOriginationElementHelper.EnumerateDependentElemets<ISupportInitialize>(sourceControl, getTarget, 
						supportInitialize => supportInitialize.BeginInit());
				});
			}
			void IDataControlOriginationElement.NotifyEndInit(DataControlBase sourceControl, Func<DataControlBase, ISupportInitialize> getTarget) {
				PerformSyncAction(() => {
					DataControlOriginationElementHelper.EnumerateDependentElemets<ISupportInitialize>(sourceControl, getTarget,
						supportInitialize => supportInitialize.EndInit());
				});
			}
			DataControlBase IDataControlOriginationElement.GetOriginationControl(DataControlBase sourceControl) { 
				return dataControl; 
			}
			void PerformSyncAction(Action action) {
				dataControl.syncPropertyLocker.DoLockedActionIfNotLocked(action);
			}
		}
		IDataControlOriginationElement dataControlOriginationElement;
		internal IDataControlOriginationElement GetDataControlOriginationElement() {
			return dataControlOriginationElement ?? NullDataControlOriginationElement.Instance; 
		}
		internal DataControlBase GetOriginationDataControl() {
			return GetDataControlOriginationElement().GetOriginationControl(this);
		}
		internal bool IsOriginationDataControl() {
			return this == GetOriginationDataControl();
		}
		internal bool IsOriginationDataControlCore() {
			return IsOriginationDataControl() && GetRootDataControl() != this;
		}
		internal void CheckIsOriginationDataControl() {
#if DEBUGTEST
			if(!IsOriginationDataControl())
				throw new NotImplementedException("Not implemented in master detail"); 
#endif
		}
		protected void RaiseEventInOriginationGrid(RoutedEventArgs e) {
			GetOriginationDataControl().RaiseEvent(e);
		}
		protected void RaiseGridEventInOriginationGrid(RoutedEvent routedEvent) {
			RaiseEventInOriginationGrid(new GridEventArgs(this, routedEvent));
		}
		internal DataControlBase GetRootDataControl() {
			DataControlBase originationControl = GetOriginationDataControl();
			DataControlBase rootDataControl = null;
			originationControl.EnumerateThisAndOwnerDataControls(dataControl => rootDataControl = dataControl);
			return rootDataControl;
		}
		internal List<KeyValuePair<DataViewBase, int>> GetViewVisibleIndexChain(int innerVisibleIndex) {
			List<KeyValuePair<DataViewBase, int>> result = new List<KeyValuePair<DataViewBase, int>>(3);
			if(innerVisibleIndex >= 0)
				result.Insert(0, new KeyValuePair<DataViewBase, int>(DataView, innerVisibleIndex));
			CollectViewVisibleIndexChain(result);
			return result;
		}
		internal void CollectViewVisibleIndexChain(List<KeyValuePair<DataViewBase, int>> chain) {
			DataControlParent.CollectViewVisibleIndexChain(chain);
		}
		internal List<int> GetParentFixedRowsScrollIndexes(int visibleIndex) {
			List<int> scrollIndexes = new List<int>();
			CollectParentFixedRowsScrollIndexes(visibleIndex, scrollIndexes);
			return scrollIndexes;
		}
		internal void CollectParentFixedRowsScrollIndexes(int visibleIndex, List<int> scrollIndexes) {
			if(DataView.AllowFixedGroupsCore) {
				int rowHandle = GetRowHandleByVisibleIndexCore(visibleIndex);
				int parentRowHandle = rowHandle;
				if((DataProviderBase.IsGroupRowHandle(rowHandle) && !DataProviderBase.IsGroupRowExpanded(rowHandle)) || !DataProviderBase.IsGroupRowHandle(parentRowHandle)) {
					parentRowHandle = DataProviderBase.GetParentRowHandle(rowHandle);
				}
				while(parentRowHandle != DataControlBase.InvalidRowHandle) {
					scrollIndexes.Add(DataView.ConvertVisibleIndexToScrollIndex(GetRowVisibleIndexByHandleCore(parentRowHandle)));
					parentRowHandle = DataProviderBase.GetParentRowHandle(parentRowHandle);
				}
			}
			DataControlParent.CollectParentFixedRowsScrollIndexes(scrollIndexes);
		}
		internal void NavigateToFirstMasterRow() {
			DataViewBase masterView = DataControlParent.FindMasterView();
			if(masterView != null) {
				masterView.MoveFocusedRow(0);
			}
		}
		internal void NavigateToLastMasterRow() {
			DataViewBase masterView = DataControlParent.FindMasterView();
			if(masterView != null) {
				masterView.MoveFocusedRow(masterView.DataControl.VisibleRowCount - 1);
			}
		}
		internal void NavigateToMasterRow() {
			DataViewBase targetView = null;
			int targetVisibleIndex = -1;
			if(DataControlParent.FindMasterRow(out targetView, out targetVisibleIndex)) {
				targetView.MoveFocusedRow(targetVisibleIndex);
			}
		}
		internal void NavigateToMasterCell(bool isTabNavigation) {
			DataViewBase targetView = null;
			int targetVisibleIndex = -1;
			if(DataControlParent.FindMasterRow(out targetView, out targetVisibleIndex)) {
				targetView.MoveFocusedRow(targetVisibleIndex);
				targetView.MoveLastNavigationIndex(isTabNavigation);
			}
		}
		internal void NavigateToNextOuterMasterRow() {
			DataViewBase targetView = null;
			int targetVisibleIndex = -1;
			if(DataControlParent.FindNextOuterMasterRow(out targetView, out targetVisibleIndex)) {
				targetView.MoveFocusedRow(targetVisibleIndex);
			}
		}
		internal void NavigateToNextOuterMasterCell(bool isTabNavigation) {
			DataViewBase targetView = null;
			int targetVisibleIndex = -1;
			if(DataControlParent.FindNextOuterMasterRow(out targetView, out targetVisibleIndex)) {
				targetView.MoveFocusedRow(targetVisibleIndex);
				targetView.MoveFirstNavigationIndex(isTabNavigation);
			}
		}
		internal bool NavigateToFirstChildDetailRow() {
			DataViewBase firstDetailView = MasterDetailProvider.FindFirstDetailView(DataProviderBase.CurrentIndex);
			if(firstDetailView != null) {
				firstDetailView.NavigateToFirstRow();
				return true;
			}
			return false;
		}
		internal bool NavigateToFirstChildDetailCell(bool isTabNavigation) {
			DataViewBase firstDetailView = MasterDetailProvider.FindFirstDetailView(DataProviderBase.CurrentIndex);
			if(firstDetailView != null) {
				firstDetailView.NavigateToFirstRow();
				firstDetailView.MoveFirstNavigationIndex(isTabNavigation);
				return true;
			}
			return false;
		}
		internal bool NavigateToPreviousInnerDetailRow() {
			if(DataProviderBase.CurrentIndex == 0)
				return false;
			DataViewBase previousInnerDetailView = MasterDetailProvider.FindLastInnerDetailView(DataProviderBase.CurrentIndex - 1);
			if(previousInnerDetailView != null) {
				previousInnerDetailView.NavigateToLastRow();
				return true;
			}
			return false;
		}
		internal bool NavigateToPreviousInnerDetailCell(bool isTabNavigation) {
			if(DataProviderBase.CurrentIndex == 0)
				return false;
			DataViewBase previousInnerDetailView = MasterDetailProvider.FindLastInnerDetailView(DataProviderBase.CurrentIndex - 1);
			if(previousInnerDetailView != null) {
				previousInnerDetailView.NavigateToLastRow();
				previousInnerDetailView.MoveLastNavigationIndex(isTabNavigation);
				return true;
			}
			return false;
		}
		internal int CalcTotalLevel(int visibleIndex) {
			return CalcTotalLevelByRowHandle(GetRowHandleByVisibleIndexCore(visibleIndex));
		}
		internal int CalcTotalLevelByRowHandle(int rowHandle) {
			int totalLevel = DataControlParent.CalcTotalLevel();
			if(DataView.AllowFixedGroupsCore) {
				totalLevel += DataProviderBase.GetRowLevelByControllerRow(rowHandle);
			}
			if(rowHandle == NewItemRowHandle && DataView.IsNewItemRowVisible) {
				totalLevel--;
			}
			return totalLevel;
		}
		internal void UpdateAllDetailAndOriginationDataControls(Action<DataControlBase> updateMethod) {
			UpdateAllDetailDataControls(updateMethod);
			UpdateAllOriginationDataControls(updateMethod);
		}
		internal void UpdateAllDetailDataControls(Action<DataControlBase> updateOpenDetailMethod, Action<DataControlBase> updateClosedDetailMethod = null) {
			DataControlBase rootDataControl = GetRootDataControl();
			updateOpenDetailMethod(rootDataControl);
			rootDataControl.MasterDetailProvider.UpdateDetailDataControls(updateOpenDetailMethod, updateClosedDetailMethod);
		}
		internal void UpdateAllOriginationDataControls(Action<DataControlBase> updateMethod) {
			DataControlBase rootDataControl = GetRootDataControl();
			updateMethod(rootDataControl);
			rootDataControl.MasterDetailProvider.UpdateOriginationDataControls(updateMethod);
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			GetDataControlOriginationElement().NotifyPropertyChanged(this, e.Property, dataControl => dataControl, typeof(DataControlBase));
			if(e.Property == FrameworkElement.LanguageProperty)
				UpdateLanguage();
		}
		void UpdateLanguage() {
			if(DataView == null)
				return;
			foreach(ColumnBase column in ColumnsCore)
				column.UpdateSimpleBindingLanguage();
			DataView.UpdateCellDataLanguage();
		}
		internal void InvalidateDetailScrollInfoCache() {
			EnumerateThisAndParentDataControls(dataControl => dataControl.MasterDetailProvider.InvalidateDetailScrollInfoCache());
		}
		protected internal virtual bool RaiseMasterRowExpandStateChanging(int rowHandle, bool isExpanded) {
			return true;
		}
		protected internal virtual void RaiseMasterRowExpandStateChanged(int rowHandle, bool isExpanded) {
		}
		#endregion
		#region IDispalyMemberBindingClient Members
		void IDispalyMemberBindingClient.UpdateColumns() {
			UpdateColumnsUnboundType();
		}
		void IDispalyMemberBindingClient.UpdateSimpleBinding() {
			foreach(ColumnBase column in ColumnsCore)
				column.UpdateDisplayMemberBindingData();
		}
		void UpdateColumnsUnboundType() {
			NeedRepopulateColumnsOnUnboundChanged = false;
			LockRepopulateColumnsOnUnboundChanged = true;
			foreach(ColumnBase column in ColumnsCore)
				column.SetUnboundType();
			LockRepopulateColumnsOnUnboundChanged = false;
			if(NeedRepopulateColumnsOnUnboundChanged)
				OnColumnUnboundChanged();
		}
		#endregion
		#region SearchControl
		void ProcessSearchControlFocus() {
			GridControlColumnProviderBase columnProvider = DataView.MasterRootRowsContainer.FocusedView.SearchPanelColumnProvider;
			if(!IsKeyboardFocusWithin || columnProvider == null || !columnProvider.IsSearchLookUpMode || DataView.MasterRootRowsContainer.FocusedView.SearchControl == null)
				return;
			DataView.MasterRootRowsContainer.FocusedView.SearchControl.Focus();
		}
		void UpdateSearchPanel(DataViewBase oldValue, DataViewBase newValue) {
			if(oldValue == null || newValue == null)
				return;
			if(newValue.SearchString != oldValue.SearchString)
				newValue.SearchString = oldValue.SearchString;
		}
		#endregion
		#region IEventArgsConverterSouce
		class EventArgsConverter : IDataRowEventArgsConverter, IDataCellEventArgsConverter {
			readonly DataControlBase dataControl;
			public EventArgsConverter(DataControlBase dataControl) {
				this.dataControl = dataControl;
			}
			object IDataRowEventArgsConverter.GetDataRow(System.Windows.RoutedEventArgs e) {
				DataViewBase view = dataControl.DataView;
				if(view == null)
					return null;
				IDataViewHitInfo hitInfo = view.CalcHitInfoCore(e.OriginalSource as DependencyObject);
				return hitInfo.InRow ? view.DataControl.GetRow(hitInfo.RowHandle) : null;
			}
			CellValue IDataCellEventArgsConverter.GetDataCell(System.Windows.RoutedEventArgs e) {
				DataViewBase view = dataControl.DataView;
				if(view == null)
					return null;
				IDataViewHitInfo hitInfo = view.CalcHitInfoCore(e.OriginalSource as DependencyObject);
				return hitInfo.IsRowCell ? new CellValue(view.DataControl.GetRow(hitInfo.RowHandle), hitInfo.Column.FieldName, view.DataControl.GetCellValue(hitInfo.RowHandle, hitInfo.Column.FieldName)) : null;
			}
		}
		readonly EventArgsConverter eventArgsConverter;
		object IEventArgsConverterSource.EventArgsConverter {
			get {
				return eventArgsConverter;
			}
		}
		#endregion
		internal bool AllowBandChooser { get { return BandsLayoutCore != null && BandsLayoutCore.ShowBandsInCustomizationForm; } }
#if DEBUGTEST && SL
		internal void UpdateFocusPropertiersSLDebugTest() {
			UpdateFocusProperties();
		}
#endif
		protected internal int LastRowIndex {
			get { return VisibleRowCount - 1; }
		}
		protected internal int CurrentIndex {
			get { return DataProviderBase.CurrentIndex; }
		}
		protected internal bool IsFirst(int index) {
			return index == 0;
		}
		protected internal bool IsLast(int index) {
			return index == LastRowIndex;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public System.Windows.Controls.Border SelectionRectangle { get { return GetTemplateChild("PART_selectionRectangle") as System.Windows.Controls.Border; } }
	}
	public class SelectedRowsCollection : ObservableCollectionCore<object> { }
}

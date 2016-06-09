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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Data.Filtering;
using DevExpress.Data.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public enum FindMode {
		Always,
		FindClick
	}
	[DXToolboxBrowsable(false)]
	[ComplexBindingProperties("ItemsSource", "ValueMember")]
	[LookupBindingProperties("ItemsSource", "DisplayMember", "ValueMember", "DisplayMember")]
	public abstract partial class LookUpEditBase : PopupBaseEdit, ISelectorEdit {
		#region static
		public static readonly DependencyProperty AllowCollectionViewProperty;
		public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemsPanelProperty;
		public static readonly DependencyProperty ItemContainerStyleProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		protected static readonly DependencyPropertyKey SelectedItemsPropertyKey;
		public static readonly DependencyProperty SelectedItemsProperty;
		public static readonly DependencyProperty SelectedIndexProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty DisplayMemberProperty;
		public static readonly DependencyProperty ValueMemberProperty;
		public static readonly RoutedEvent ProcessNewValueEvent;
		public static readonly RoutedEvent SelectedIndexChangedEvent;
		public static readonly RoutedEvent PopupContentSelectionChangedEvent;
		public static readonly DependencyProperty ApplyItemTemplateToSelectedItemProperty;
		public static readonly DependencyProperty SeparatorStringProperty;
		public static readonly DependencyProperty AutoCompleteProperty;
		public static readonly DependencyProperty ClearSelectionOnBackspaceProperty;
		public static readonly DependencyProperty IsCaseSensitiveSearchProperty;
		public static readonly DependencyProperty AutoSearchTextProperty;
		static readonly DependencyPropertyKey AutoSearchTextPropertyKey;
		public static readonly DependencyProperty ImmediatePopupProperty;
		protected static readonly DependencyPropertyKey SelectedItemValuePropertyKey;
		public static readonly DependencyProperty SelectedItemValueProperty;
		public static readonly DependencyProperty AllowItemHighlightingProperty;
		public static readonly DependencyProperty IncrementalFilteringProperty;
		public static readonly DependencyProperty AddNewButtonPlacementProperty;
		public static readonly DependencyProperty FindButtonPlacementProperty;
		public static readonly DependencyProperty FindModeProperty;
		public static readonly DependencyProperty FilterConditionProperty;
		public static readonly DependencyProperty FilterCriteriaProperty;
		public static readonly DependencyProperty AssignNullValueOnClearingEditTextProperty;
		static readonly DependencyPropertyKey IsTokenModePropertyKey;
		public static readonly DependencyProperty IsTokenModeProperty;
		public static readonly DependencyProperty IsAsyncOperationInProgressProperty;
		static readonly DependencyPropertyKey IsAsyncOperationInProgressPropertyKey;
		public static readonly DependencyProperty AllowRejectUnknownValuesProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty AllowLiveDataShapingProperty;
		public static readonly DependencyProperty ShowEditorWaitIndicatorProperty;
		public static readonly DependencyProperty ShowPopupWaitIndicatorProperty;
		static LookUpEditBase() {
			Type ownerType = typeof(LookUpEditBase);
			AllowLiveDataShapingProperty = DependencyProperty.Register("AllowLiveDataShaping", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, (d, e) => ((LookUpEditBase)d).AllowLiveDataShapingChanged((bool)e.NewValue)));
			FilterCriteriaProperty = DependencyPropertyManager.Register("FilterCriteria", typeof(CriteriaOperator), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => ((LookUpEditBase)d).FilterCriteriaChanged((CriteriaOperator)e.NewValue)));
			AllowCollectionViewProperty = DependencyPropertyManager.Register("AllowCollectionView", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None,
				(d, e) => ((LookUpEditBase)d).AllowCollectionViewChanged((bool)e.NewValue)));
			IsSynchronizedWithCurrentItemProperty = DependencyPropertyManager.Register("IsSynchronizedWithCurrentItem", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((LookUpEditBase)d).IsSynchronizedWithCurrentItemChanged((bool)e.NewValue)));
			ItemTemplateProperty = ItemsControl.ItemTemplateProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnItemTemplateChanged));
			ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(object), ownerType, new FrameworkPropertyMetadata(null, ItemsSourceChanged));
			ItemContainerStyleProperty = ComboBox.ItemContainerStyleProperty.AddOwner(ownerType);
			DisplayMemberProperty = DependencyPropertyManager.Register("DisplayMember", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((LookUpEditBase)d).OnDisplayMemberChanged((string)e.NewValue)));
			ValueMemberProperty = DependencyPropertyManager.Register("ValueMember", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((LookUpEditBase)d).OnValueMemberChanged((string)e.NewValue)));
			SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged, OnSelectedItemCoerce));
			SelectedIndexProperty = DependencyPropertyManager.Register("SelectedIndex", typeof(int), ownerType, new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedIndexChanged, OnSelectedIndexCoerce));
			SelectedItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("SelectedItems", typeof(ObservableCollection<object>), ownerType, new PropertyMetadata(null, OnSelectedItemsChanged));
			SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;
			SelectedIndexChangedEvent = EventManager.RegisterRoutedEvent("SelectedIndexChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), ownerType);
			PopupContentSelectionChangedEvent = EventManager.RegisterRoutedEvent("PopupContentSelectionChanged", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), ownerType);
			ProcessNewValueEvent = EventManager.RegisterRoutedEvent("ProcessNewValue", RoutingStrategy.Direct, typeof(ProcessNewValueEventHandler), ownerType);
			ApplyItemTemplateToSelectedItemProperty = DependencyPropertyManager.Register("ApplyItemTemplateToSelectedItem", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, ApplyItemTemplateToSelectedItemChanged));
			SeparatorStringProperty = DependencyPropertyManager.Register("SeparatorString", typeof(string), ownerType, new FrameworkPropertyMetadata(";", OnSeparatorStringChanged));
			ClearSelectionOnBackspaceProperty = DependencyPropertyManager.Register("ClearSelectionOnBackspace", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, (d, e) => ((LookUpEditBase)d).ClearSelectionOnBackspaceChanged((bool)e.NewValue)));
			AutoCompleteProperty = DependencyPropertyManager.Register("AutoComplete", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, (d, e) => ((LookUpEditBase)d).AutoCompleteChanged((bool)e.NewValue)));
			IsCaseSensitiveSearchProperty = DependencyPropertyManager.Register("IsCaseSensitiveSearch", typeof(bool), ownerType, new PropertyMetadata(false));
			AutoSearchTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("AutoSearchText", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, OnAutoSearchTextPropertyChanged));
			AutoSearchTextProperty = AutoSearchTextPropertyKey.DependencyProperty;
			ImmediatePopupProperty = DependencyPropertyManager.Register("ImmediatePopup", typeof(bool), ownerType, new PropertyMetadata(false));
			SelectedItemValuePropertyKey = DependencyPropertyManager.RegisterReadOnly("SelectedItemValue", typeof(object), ownerType, new PropertyMetadata(null));
			SelectedItemValueProperty = SelectedItemValuePropertyKey.DependencyProperty;
			AllowItemHighlightingProperty = DependencyPropertyManager.Register("AllowItemHighlighting", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IncrementalFilteringProperty = DependencyPropertyManager.Register("IncrementalFiltering", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null, OnIncrementalFilteringChanged));
			AddNewButtonPlacementProperty = DependencyPropertyManager.Register("AddNewButtonPlacement", typeof(EditorPlacement?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((LookUpEditBase)d).AddNewButtonPlacementChanged((EditorPlacement?)e.NewValue)));
			FindButtonPlacementProperty = DependencyPropertyManager.Register("FindButtonPlacement", typeof(EditorPlacement?), ownerType, new FrameworkPropertyMetadata(null));
			FindModeProperty = DependencyPropertyManager.Register("FindMode", typeof(FindMode?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((LookUpEditBase)d).FindModeChanged((FindMode?)e.NewValue)));
			FilterConditionProperty = DependencyPropertyManager.Register("FilterCondition", typeof(FilterCondition?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((LookUpEditBase)d).FilterConditionChanged((FilterCondition?)e.NewValue), (d, v) => { return ((LookUpEditBase)d).CoerceFilterCondition((FilterCondition?)v); }));
			AssignNullValueOnClearingEditTextProperty = DependencyPropertyManager.Register("AssignNullValueOnClearingEditText", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsTokenModePropertyKey = DependencyProperty.RegisterReadOnly("IsTokenMode", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((LookUpEditBase)d).OnIsTokenModeChanged()));
			IsTokenModeProperty = IsTokenModePropertyKey.DependencyProperty;
			ItemsPanelProperty = ItemsControl.ItemsPanelProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(ItemsControl.ItemsPanelProperty.DefaultMetadata.DefaultValue, OnItemsPanelChanged));
			ItemTemplateSelectorProperty = ItemsControl.ItemTemplateSelectorProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(null, OnItemTemplateSelectorChanged));
			IsAsyncOperationInProgressPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<LookUpEditBase, bool>(
				owner => owner.IsAsyncOperationInProgress, false, (owner, oldValue, newValue) => owner.IsAsyncOperationInProgressChanged(oldValue, newValue));
			IsAsyncOperationInProgressProperty = IsAsyncOperationInProgressPropertyKey.DependencyProperty;
			AllowRejectUnknownValuesProperty = DependencyPropertyRegistrator.Register<LookUpEditBase, bool>(
				owner => owner.AllowRejectUnknownValues, false, (@base, value, newValue) => @base.AllowRejectUnknownValuesChanged(newValue));
			ShowEditorWaitIndicatorProperty = DependencyPropertyManager.Register("ShowEditorWaitIndicator", typeof(bool), ownerType, new PropertyMetadata(true));
			ShowPopupWaitIndicatorProperty = DependencyPropertyManager.Register("ShowPopupWaitIndicator", typeof(bool), ownerType, new PropertyMetadata(true));
		}
		static void OnAutoSearchTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)d).OnAutoSearchTextChanged((string)e.NewValue);
		}
		static void ItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)obj).ItemsSourceChanged(e.NewValue);
		}
		static void OnItemTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)obj).OnItemTemplateChanged();
		}
#if !SL
		static void OnItemTemplateSelectorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)obj).OnItemTemplateSelectorChanged();
		}
#endif
		static void OnItemsPanelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)obj).OnItemsPanelChanged();
		}
		static void ApplyItemTemplateToSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)obj).OnApplyItemTemplateToSelectedItemChanged();
		}
		static void OnSeparatorStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)obj).OnSeparatorStringChanged();
		}
		protected static object OnSelectedIndexCoerce(DependencyObject obj, object baseValue) {
			return ((LookUpEditBase)obj).OnSelectedIndexCoerce(baseValue);
		}
		protected static object OnSelectedItemCoerce(DependencyObject obj, object baseValue) {
			return ((LookUpEditBase)obj).OnSelectedItemCoerce(baseValue);
		}
		static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)obj).OnSelectedItemChanged(e.OldValue, e.NewValue);
		}
		static void OnSelectedItemsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)obj).OnSelectedItemsChanged((IList)e.OldValue, (IList)e.NewValue);
		}
		static void OnSelectedIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)obj).OnSelectedIndexChanged((int)e.OldValue, (int)e.NewValue);
		}
		protected static void OnIncrementalFilteringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditBase)obj).OnIncrementalFilteringChanged();
		}
		#endregion
		readonly Locker selectedItemsInitialeLocker = new Locker();
		ICommand addNewCommand;
		ItemsProviderChangedEventHandler<LookUpEditBase> ItemsProviderChangedEventHandler { get; set; }
		ListChangedWeakEventHandler<LookUpEditBase> SelectedItemsSourceChangedEventHandler { get; set; }
		protected LookUpEditBase() {
			this.SetDefaultStyleKey(typeof(LookUpEditBase));
			using (selectedItemsInitialeLocker.Lock()) {
				ObservableCollection<object> observables = new ObservableCollection<object>();
				observables.CollectionChanged += OnSelectedItemsCollectionChanged;
				SelectedItems = observables;
			}
			SelectedValueRenderer = new SelectedItemValueRenderer(this);
			FindCommand = DelegateCommandFactory.Create<object>(FindInternal, false);
			AddNewCommand = DelegateCommandFactory.Create<object>(AddNewInternal, false);
			SelectAllItemsCommand = DelegateCommandFactory.Create<object>(ChangeSelection, CanChangeSelection, false);
			ItemsProviderChangedEventHandler = new ItemsProviderChangedEventHandler<LookUpEditBase>(this, (owner, o, e) => owner.EditStrategy.ItemProviderChanged(e));
		}
		void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			EditStrategy.SelectedItemsCollectionChanged(e);
		}
		protected internal new LookUpEditSettingsBase Settings { get { return base.Settings as LookUpEditSettingsBase; } }
		protected internal IItemsProvider2 ItemsProvider { get { return Settings.ItemsProvider; } }
		internal SelectedItemValueRenderer SelectedValueRenderer { get; private set; }
		public bool AllowLiveDataShaping {
			get { return (bool)GetValue(AllowLiveDataShapingProperty); }
			set { SetValue(AllowLiveDataShapingProperty, value); }
		}
		public bool AllowRejectUnknownValues {
			get { return (bool)GetValue(AllowRejectUnknownValuesProperty); }
			set { SetValue(AllowRejectUnknownValuesProperty, value); }
		}
		public bool IsAsyncOperationInProgress {
			get { return (bool)GetValue(IsAsyncOperationInProgressProperty); }
			internal set { SetValue(IsAsyncOperationInProgressPropertyKey, value); }
		}
		public bool ShowEditorWaitIndicator {
			get { return (bool)GetValue(ShowEditorWaitIndicatorProperty); }
			set { SetValue(ShowEditorWaitIndicatorProperty, value); }
		}
		public bool ShowPopupWaitIndicator {
			get { return (bool)GetValue(ShowPopupWaitIndicatorProperty); }
			set { SetValue(ShowPopupWaitIndicatorProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public EditorPlacement? FindButtonPlacement {
			get { return (EditorPlacement?)GetValue(FindButtonPlacementProperty); }
			set { SetValue(FindButtonPlacementProperty, value); }
		}
#if SL
		[TypeConverter(typeof(NullableConverter<FindMode>))]
#endif
		[Category(EditSettingsCategories.Behavior)]
		public FindMode? FindMode {
			get { return (FindMode?)GetValue(FindModeProperty); }
			set { SetValue(FindModeProperty, value); }
		}
#if SL
		[TypeConverter(typeof(NullableConverter<FilterCondition>))]
#endif
		[Category(EditSettingsCategories.Behavior)]
		public FilterCondition? FilterCondition {
			get { return (FilterCondition?)GetValue(FilterConditionProperty); }
			set { SetValue(FilterConditionProperty, value); }
		}
#if SL
		[TypeConverter(typeof(NullableConverter<EditorPlacement>))]
#endif
		[Category(EditSettingsCategories.Behavior)]
		public EditorPlacement? AddNewButtonPlacement {
			get { return (EditorPlacement?)GetValue(AddNewButtonPlacementProperty); }
			set { SetValue(AddNewButtonPlacementProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseApplyItemTemplateToSelectedItem"),
#endif
 Category("Behavior")]
		public bool ApplyItemTemplateToSelectedItem {
			get { return (bool)GetValue(ApplyItemTemplateToSelectedItemProperty); }
			set { SetValue(ApplyItemTemplateToSelectedItemProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseItemTemplate"),
#endif
 Browsable(false)]
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
#if !SL
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseItemTemplateSelector"),
#endif
 Browsable(false)]
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
#endif
#if !SL
	[DevExpressXpfCoreLocalizedDescription("LookUpEditBaseAllowItemHighlighting")]
#endif
		public bool AllowItemHighlighting {
			get { return (bool)GetValue(AllowItemHighlightingProperty); }
			set { SetValue(AllowItemHighlightingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseItemsPanel"),
#endif
 Browsable(false)]
		public ItemsPanelTemplate ItemsPanel {
			get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
			set { SetValue(ItemsPanelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseSelectedItem"),
#endif
 Category("Common Properties")]
		public object SelectedItem {
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseSelectedItems"),
#endif
 Category("Common Properties")]
		public ObservableCollection<object> SelectedItems {
			get { return (ObservableCollection<object>)GetValue(SelectedItemsProperty); }
			private set { this.SetValue(SelectedItemsPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseSelectedIndex"),
#endif
 Category("Common Properties")]
		public int SelectedIndex {
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
		[Category("Action")]
		public event RoutedEventHandler SelectedIndexChanged {
			add { this.AddHandler(SelectedIndexChangedEvent, value); }
			remove { this.RemoveHandler(SelectedIndexChangedEvent, value); }
		}
		[Category("Action")]
		public event ProcessNewValueEventHandler ProcessNewValue {
			add { this.AddHandler(ProcessNewValueEvent, value); }
			remove { this.RemoveHandler(ProcessNewValueEvent, value); }
		}
		[Category("Action")]
		public event SelectionChangedEventHandler PopupContentSelectionChanged {
			add { this.AddHandler(PopupContentSelectionChangedEvent, value); }
			remove { this.RemoveHandler(PopupContentSelectionChangedEvent, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseItemsSource"),
#endif
 Bindable(true), Category("Common Properties"),]
		public object ItemsSource {
			get { return (object)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseDisplayMember"),
#endif
		DefaultValue(""),
		Category("Common Properties")]
		public string DisplayMember {
			get { return (string)GetValue(DisplayMemberProperty); }
			set { SetValue(DisplayMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseValueMember"),
#endif
		DefaultValue(""),
		Category("Common Properties")]
		public string ValueMember {
			get { return (string)GetValue(ValueMemberProperty); }
			set { SetValue(ValueMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseItemContainerStyle"),
#endif
 Browsable(false)]
		public Style ItemContainerStyle {
			get { return (Style)GetValue(ItemContainerStyleProperty); }
			set { SetValue(ItemContainerStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseSeparatorString"),
#endif
 Category("Appearance")]
		public string SeparatorString {
			get { return (string)GetValue(SeparatorStringProperty); }
			set { SetValue(SeparatorStringProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseClearSelectionOnBackspace"),
#endif
 Category("Behavior")]
		public bool ClearSelectionOnBackspace {
			get { return (bool)GetValue(ClearSelectionOnBackspaceProperty); }
			set { SetValue(ClearSelectionOnBackspaceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseAutoComplete"),
#endif
 Category("Behavior")]
		public bool AutoComplete {
			get { return (bool)GetValue(AutoCompleteProperty); }
			set { SetValue(AutoCompleteProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseIsCaseSensitiveSearch"),
#endif
 Category("Behavior")]
		public bool IsCaseSensitiveSearch {
			get { return (bool)GetValue(IsCaseSensitiveSearchProperty); }
			set { SetValue(IsCaseSensitiveSearchProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseImmediatePopup"),
#endif
 Category("Behavior")]
		public bool ImmediatePopup {
			get { return (bool)GetValue(ImmediatePopupProperty); }
			set { SetValue(ImmediatePopupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("LookUpEditBaseIncrementalFiltering"),
#endif
 Category("Behavior")]
		public bool? IncrementalFiltering {
			get { return (bool?)GetValue(IncrementalFilteringProperty); }
			set { SetValue(IncrementalFilteringProperty, value); }
		}
		[Category("Behavior")]
		public bool IsSynchronizedWithCurrentItem {
			get { return (bool)GetValue(IsSynchronizedWithCurrentItemProperty); }
			set { SetValue(IsSynchronizedWithCurrentItemProperty, value); }
		}
		[Category("Behavior")]
		public bool AllowCollectionView {
			get { return (bool)GetValue(AllowCollectionViewProperty); }
			set { SetValue(AllowCollectionViewProperty, value); }
		}
		[Category("Behavior")]
		public CriteriaOperator FilterCriteria {
			get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); }
			set { SetValue(FilterCriteriaProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("LookUpEditBaseFindCommand")]
#endif
		public ICommand FindCommand { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("LookUpEditBaseAddNewCommand")]
#endif
		public ICommand AddNewCommand {
			get { return addNewCommand; }
			private set {
				if (value == addNewCommand)
					return;
				addNewCommand = value;
				AddNewCommandChanged();
			}
		}
		public ICommand SelectAllItemsCommand { get; private set; }
		#region API
		public int GetIndexByKeyValue(object keyValue) {
			return ItemsProvider.IndexOfValue(keyValue, ItemsProvider.CurrentDataViewHandle);
		}
		public object GetItemByKeyValue(object keyValue) {
			return ItemsProvider.GetItem(keyValue, ItemsProvider.CurrentDataViewHandle);
		}
		public object GetKeyValue(int index) {
			return ItemsProvider.GetValueByIndex(index, ItemsProvider.CurrentDataViewHandle);
		}
		public object GetDisplayValue(int index) {
			return ItemsProvider.GetDisplayValueByIndex(index, ItemsProvider.CurrentDataViewHandle);
		}
		public void RefreshData() {
			ItemsProvider.DoRefresh();
		}
		#endregion
		[Browsable(false)]
		public string AutoSearchText {
			get { return (string)GetValue(AutoSearchTextProperty); }
			internal set { this.SetValue(AutoSearchTextPropertyKey, value); }
		}
		[Browsable(false)]
		public object SelectedItemValue {
			get { return GetValue(SelectedItemValueProperty); }
			internal set { this.SetValue(SelectedItemValuePropertyKey, value); }
		}
		[Browsable(false)]
		public bool IsTokenMode {
			get { return (bool)GetValue(IsTokenModeProperty); }
			internal set { this.SetValue(IsTokenModePropertyKey, value); }
		}
		public bool AssignNullValueOnClearingEditText {
			get { return (bool)GetValue(AssignNullValueOnClearingEditTextProperty); }
			set { SetValue(AssignNullValueOnClearingEditTextProperty, value); }
		}
		new protected LookUpEditBasePropertyProvider PropertyProvider { get { return (LookUpEditBasePropertyProvider)base.PropertyProvider; } }
		protected internal override Type StyleSettingsType { get { return typeof(BaseLookUpStyleSettings); } }
		protected internal override bool CanShowPopup {
			get { return base.CanShowPopup && CanShowPopupCore(); }
		}
		protected virtual bool CanShowPopupCore() {
			return ItemsProvider != null && ItemsProvider.GetCount(EditStrategy.CurrentDataViewHandle) > 0;
		}
		protected internal override FrameworkElement PopupElement { get { return VisualClient.InnerEditor; } }
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new LookUpEditBasePropertyProvider(this);
		}
		bool autocompletesettings = false;
		protected internal override TextInputSettingsBase CreateTextInputSettings() {
			return autocompletesettings ? new TextInputAutoCompleteSettings(this) : base.CreateTextInputSettings();
		}
		protected internal new LookUpEditStrategyBase EditStrategy {
			get { return base.EditStrategy as LookUpEditStrategyBase; }
			set { base.EditStrategy = value; }
		}
		protected override void UnsubscribeFromSettings(BaseEditSettings settings) {
			base.UnsubscribeFromSettings(settings);
			if (settings != null)
				((LookUpEditSettingsBase)settings).ItemsProvider.ItemsProviderChanged -= ItemsProviderChangedEventHandler.Handler;
			EditStrategy.UpdateIncrementalFilteringSnapshot(PropertyProvider.EditMode, PropertyProvider.EditMode);
		}
		protected override void SubscribeToSettings(BaseEditSettings settings) {
			base.SubscribeToSettings(settings);
			if (settings != null)
				((LookUpEditSettingsBase)settings).ItemsProvider.ItemsProviderChanged += ItemsProviderChangedEventHandler.Handler;
			EditStrategy.UpdateIncrementalFilteringSnapshot(PropertyProvider.EditMode, PropertyProvider.EditMode);
		}
		protected virtual void OnAutoSearchTextChanged(string text) {
			EditStrategy.AutoSeachTextChanged(text);
		}
		protected virtual void ItemsSourceChanged(object itemsSource) {
			EditStrategy.ItemSourceChanged(itemsSource);
		}
		protected virtual void AddNewCommandChanged() {
			EditStrategy.AddNewCommandChanged();
		}
		void ResetSelectedIndex() {
			this.CoerceValue(SelectedIndexProperty);
		}
		protected virtual void OnItemTemplateChanged() {
			Settings.ItemTemplate = ItemTemplate;
		}
		protected virtual void OnItemsPanelChanged() {
			Settings.ItemsPanel = ItemsPanel;
		}
		protected virtual void OnApplyItemTemplateToSelectedItemChanged() {
			PropertyProvider.SetApplyItemTemplateToSelectedItem(ApplyItemTemplateToSelectedItem);
			UpdateActualEditorControlTemplate();
		}
		protected virtual void OnSeparatorStringChanged() {
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void OnItemTemplateSelectorChanged() {
			Settings.ItemTemplateSelector = ItemTemplateSelector;
		}
		protected virtual void OnDisplayMemberChanged(string displayMember) {
			EditStrategy.DisplayMemberChanged(displayMember);
		}
		protected virtual void OnValueMemberChanged(string valueMember) {
			EditStrategy.ValueMemberChanged(valueMember);
		}
		protected virtual void OnSelectedItemChanged(object oldValue, object newValue) {
			EditStrategy.SelectedItemChanged(oldValue, newValue);
		}
		protected virtual void OnSelectedItemsChanged(IList oldSelectedItems, IList selectedItems) {
			if (selectedItemsInitialeLocker.IsLocked)
				return;
			EditStrategy.SelectedItemsChanged(oldSelectedItems, selectedItems);
		}
		protected virtual void OnSelectedIndexChanged(int oldSelectedIndex, int selectedIndex) {
			EditStrategy.SelectedIndexChanged(oldSelectedIndex, selectedIndex);
		}
		protected virtual object OnSelectedIndexCoerce(object baseValue) {
			return EditStrategy.CoerceSelectedIndex((int)baseValue);
		}
		protected virtual object OnSelectedItemCoerce(object baseValue) {
			return EditStrategy.CoerceSelectedItem(baseValue);
		}
		protected internal override string GetDisplayText(object editValue, bool applyFormatting) {
			if (PropertyProvider.IsSingleSelection && !IsTokenMode)
				return base.GetDisplayText(editValue, applyFormatting);
			bool firstItem = true;
			IList items = editValue as IList;
			if (items != null && items.Count == 0)
				return base.GetDisplayText(null, applyFormatting);
			if (items == null)
				return base.GetDisplayText(editValue, applyFormatting);
			StringBuilder displayTextString = new StringBuilder();
			foreach (object item in items) {
				if (!firstItem)
					displayTextString.Append(SeparatorString);
				else
					firstItem = false;
				displayTextString.Append(base.GetDisplayText(item, applyFormatting));
			}
			return displayTextString.ToString();
		}
		protected override void OnPopupClosed() {
			base.OnPopupClosed();
			EditStrategy.PopupClosed();
		}
		protected internal override void DestroyPopupContent(EditorPopupBase popup) {
			base.DestroyPopupContent(popup);
			EditStrategy.PopupDestroyed();
		}
		protected override void AcceptPopupValue() {
			base.AcceptPopupValue();
			EditStrategy.AcceptPopupValue();
		}
		protected virtual void OnIncrementalFilteringChanged() {
			EditStrategy.OnIncrementalFilteringChanged();
		}
		protected override bool NeedsEnter(ModifierKeys modifiers) {
			return EditStrategy.NeedsEnterKey(modifiers);
		}
		#region ISelectorEdit members
#if !SL
		ObservableCollection<GroupStyle> ISelectorEdit.GroupStyle { get { return new ObservableCollection<GroupStyle>(); } }
#endif
		EditStrategyBase ISelectorEdit.EditStrategy { get { return EditStrategy; } }
		SelectionEventMode ISelectorEdit.SelectionEventMode { get { return SelectionEventMode.MouseDown; } }
		ListItemCollection ISelectorEdit.Items { get { return ((IItemsProviderOwner)Settings).Items; } }
		object ISelectorEdit.GetCurrentSelectedItem() {
			return EditStrategy.GetCurrentSelectedItem();
		}
		IEnumerable ISelectorEdit.GetCurrentSelectedItems() {
			return EditStrategy.GetCurrentSelectedItems();
		}
		ISelectionProvider ISelectorEdit.SelectionProvider { get { return new DummySelectionProvider(); } }
		IListNotificationOwner ISelectorEdit.ListNotificationOwner { get { return Settings; } }
		IItemsProvider2 ISelectorEdit.ItemsProvider { get { return ItemsProvider; } }
		SelectionMode ISelectorEdit.SelectionMode { get { return EditStrategy.StyleSettings.GetSelectionMode(this); } }
		bool ISelectorEdit.UseCustomItems { get { return PropertyProvider.ShowCustomItems(); } }
		object ISelectorEdit.GetPopupContentItemsSource() {
			return ((ISelectorEditStrategy)EditStrategy).GetInnerEditorItemsSource();
		}
		IEnumerable ISelectorEdit.GetPopupContentCustomItemsSource() {
			return ((ISelectorEditStrategy)EditStrategy).GetInnerEditorCustomItemsSource();
		}
		IEnumerable ISelectorEdit.GetPopupContentMRUItemsSource() {
			return ((ISelectorEditStrategy)EditStrategy).GetInnerEditorMRUItemsSource();
		}
		#endregion
		#region Helpers
		public static void SetupComboBoxSettingsEnumItemSource<T>(LookUpEditSettingsBase settings) {
			if (!typeof(T).IsEnum)
				throw new ArgumentException(@"Not enum type", "<T>");
			settings.ValueMember = "Value";
			settings.DisplayMember = "Text";
			List<LookUpEditEnumItem<int>> list = new List<LookUpEditEnumItem<int>>();
#if !SL
			foreach (string name in Enum.GetNames(typeof(T))) {
				T q = (T)Enum.Parse(typeof(T), name);
				list.Add(new LookUpEditEnumItem<int>() { Text = name, Value = Convert.ToInt32(q) });
			}
#else
			foreach(T q in DevExpress.Utils.EnumExtensions.GetValues(typeof(T)).Cast<T>())
				list.Add(new LookUpEditEnumItem<int>() { Text = q.ToString(), Value = Convert.ToInt32(q) });
#endif
			settings.ItemsSource = list;
		}
		public static void SetupComboBoxEnumItemSource<T, DataType>(LookUpEditBase comboBox) {
			if (!typeof(T).IsEnum)
				throw new ArgumentException("Not enum type", "<T>");
			comboBox.ValueMember = "Value";
			comboBox.DisplayMember = "Text";
			List<LookUpEditEnumItem<DataType>> list = new List<LookUpEditEnumItem<DataType>>();
#if !SL
			foreach (string name in Enum.GetNames(typeof(T))) {
				T q = (T)Enum.Parse(typeof(T), name);
				list.Add(new LookUpEditEnumItem<DataType>() { Text = name, Value = (DataType)Convert.ChangeType(q, typeof(DataType), CultureInfo.CurrentCulture) });
			}
#else 
			foreach(T q in DevExpress.Utils.EnumExtensions.GetValues(typeof(T)).Cast<T>())
				list.Add(new LookUpEditEnumItem<DataType>() { Text = q.ToString(), Value = (DataType)Convert.ChangeType(q, typeof(DataType), CultureInfo.CurrentCulture) });
#endif
			comboBox.ItemsSource = list;
		}
		#endregion
		protected internal virtual void RaisePopupContentSelectionChanged(IList removedItems, IList addedItems) {
			EditStrategy.RaisePopupContentSelectionChanged(removedItems ?? new List<object>(), addedItems ?? new List<object>());
		}
		protected override bool IsClosePopupWithCancelGesture(Key key, ModifierKeys modifiers) {
			return base.IsClosePopupWithCancelGesture(key, modifiers) && EditStrategy.IsClosePopupWithCancelGesture(key, modifiers);
		}
		protected override bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return base.IsClosePopupWithAcceptGesture(key, modifiers) || EditStrategy.IsClosePopupWithAcceptGesture(key, modifiers);
		}
		protected override void SubscribeEditEventsCore() {
			base.SubscribeEditEventsCore();
			if (IsTokenMode)
				SubcribeTokenEditorEvents();
		}
		protected override void UnsubscribeEditEventsCore() {
			base.UnsubscribeEditEventsCore();
			if (IsTokenMode)
				UnsubcribeTokenEditorEvents();
		}
		protected override void OnEditCoreAssigned() {
			base.OnEditCoreAssigned();
			UpdateEditBoxWrapperInTokenMode();
			EditStrategy.ApplyStyleSettings(PropertyProvider.StyleSettings);
		}
		private void UpdateEditBoxWrapperInTokenMode() {
			if (IsTokenMode)
				UpdateEditBoxWrapper();
		}
		protected virtual void UnsubcribeTokenEditorEvents() {
			var tokenEditor = EditCore as TokenEditor;
			if (tokenEditor != null) {
				tokenEditor.TextChanged -= OnTokenEditorTextChanged;
				tokenEditor.TokenClosed -= OnTokenEditorTokenClosed;
				tokenEditor.ValueChanged -= OnTokenEditorValueChanged;
			}
		}
		protected virtual void SubcribeTokenEditorEvents() {
			var tokenEditor = EditCore as TokenEditor;
			if (tokenEditor != null) {
				tokenEditor.TextChanged += OnTokenEditorTextChanged;
				tokenEditor.TokenClosed += OnTokenEditorTokenClosed;
				tokenEditor.ValueChanged += OnTokenEditorValueChanged;
			}
		}
		void OnTokenEditorValueChanged(object sender, EventArgs e) {
			EditStrategy.UpdateAutoSearchText(new ChangeTextItem() { Text = string.Empty }, false);
			EditStrategy.SetEditValueOnTokenEditorValueChanged();
		}
		void OnTokenEditorTokenClosed(object sender, EventArgs e) {
			EditStrategy.OnTokenEditorTokenClosed();
		}
		void OnTokenEditorTextChanged(object sender, EventArgs e) {
			EditStrategy.SyncWithEditor();
		}
		internal void FocusCore() {
			FocusEditCore();
		}
		protected virtual void FindModeChanged(FindMode? findMode) {
			EditStrategy.FindModeChanged(findMode);
		}
		protected virtual void FilterConditionChanged(FilterCondition? filterCondition) {
			EditStrategy.FilterConditionChanged(filterCondition);
		}
		protected virtual FilterCondition? CoerceFilterCondition(FilterCondition? filterCondition) {
			if (!filterCondition.HasValue ||
				filterCondition.Value == Data.Filtering.FilterCondition.StartsWith ||
				filterCondition.Value == Data.Filtering.FilterCondition.Contains ||
				filterCondition.Value == Data.Filtering.FilterCondition.Default)
				return filterCondition;
			return Data.Filtering.FilterCondition.Default;
		}
		protected virtual void AddNewButtonPlacementChanged(EditorPlacement? placement) {
			UpdatePopupElements();
		}
		protected virtual void AutoCompleteChanged(bool value) {
			EditStrategy.AutoCompleteChanged(value);
		}
		protected virtual void ClearSelectionOnBackspaceChanged(bool value) {
			EditStrategy.ClearSelectionOnBackspace(value);
		}
		protected virtual void IsSynchronizedWithCurrentItemChanged(bool value) {
			EditStrategy.IsSynchronizedWithCurrentItemChanged(value);
		}
		protected virtual void AllowCollectionViewChanged(bool value) {
			EditStrategy.AllowCollectionViewChanged(value);
		}
		protected virtual void FindInternal(object parameter) {
			EditStrategy.Find(parameter);
		}
		protected virtual void AddNewInternal(object parameter) {
			EditStrategy.AddNew(parameter);
		}
		protected override void InsertCommandButtonInfo(IList<ButtonInfoBase> collection) {
			base.InsertCommandButtonInfo(collection);
			if (AddNewButtonPlacement == EditorPlacement.EditBox) {
				collection.Insert(0, CreateAddNewButtonInfo());
			}
			if (ItemsProvider.IsAsyncServerMode && ShowEditorWaitIndicator)
				collection.Insert(0, CreateLoadingButtonInfo());
		}
		protected virtual ButtonInfoBase CreateLoadingButtonInfo() {
			ButtonInfo info = new ButtonInfo();
			info.ButtonKind = ButtonKind.Repeat;
			info.Template = FindResource(new ComboBoxEditThemeKeyExtension() { ResourceKey = ComboBoxThemeKeys.LoadingButtonTemplate, ThemeName = ThemeHelper.GetEditorThemeName(this) }) as DataTemplate;
			info.SetBinding(ButtonInfoBase.VisibilityProperty, new Binding("IsAsyncOperationInProgress") { Source = this, Converter = new BoolToVisibilityConverter() });
			return info;
		}
		protected virtual ButtonInfoBase CreateFindButtonInfo() {
			return new ButtonInfo() {
				GlyphKind = GlyphKind.Search,
				Content = EditorLocalizer.Active.GetLocalizedString(EditorStringId.LookUpFind),
				Command = FindCommand
			};
		}
		protected virtual ButtonInfoBase CreateAddNewButtonInfo() {
			return new ButtonInfo() {
				GlyphKind = GlyphKind.Plus,
				Content = EditorLocalizer.Active.GetLocalizedString(EditorStringId.LookUpAddNew),
				Command = AddNewCommand
			};
		}
		public virtual void SelectAllItems() {
			EditStrategy.SelectAllItems();
		}
		public virtual void UnselectAllItems() {
			EditStrategy.UnselectAllItems();
		}
		void ChangeSelection(object parameter) {
			if (parameter == null)
				return;
			ChangeSelection((ChangeSelectionAction)parameter);
		}
		protected virtual void OnIsTokenModeChanged() {
			EditStrategy.OnTokenModeChanged();
			UpdateEditBoxWrapper();
		}
		protected virtual bool CanChangeSelection(object parameter) {
			return !PropertyProvider.IsSingleSelection;
		}
		protected virtual void ChangeSelection(ChangeSelectionAction changeSelection) {
			if (changeSelection == ChangeSelectionAction.SelectAll)
				SelectAllItems();
			else if (changeSelection == ChangeSelectionAction.UnselectAll)
				UnselectAllItems();
		}
		protected virtual void SelectionModeChanged(SelectionMode? value) {
			EditStrategy.SelectionModeChanged(value);
		}
		protected virtual void FilterCriteriaChanged(CriteriaOperator criteriaOperator) {
			EditStrategy.FilterCriteriaChanged(criteriaOperator);
		}
		protected internal virtual void UnsubscribeToItemsProviderChanged() {
			ItemsProvider.ItemsProviderChanged -= ItemsProviderChangedEventHandler.Handler;
		}
		protected internal virtual void SubscribeToItemsProviderChanged() {
			ItemsProvider.ItemsProviderChanged += ItemsProviderChangedEventHandler.Handler;
		}
		protected override object GetTextValueInternal() {
			ITextExportSettings instance = this;
			return EditStrategy.IsInLookUpMode ? instance.Text : base.GetTextValueInternal();
		}
		protected override bool? GetXlsExportNativeFormatInternal() {
			return EditStrategy.IsInLookUpMode ? false : base.GetXlsExportNativeFormatInternal();
		}
		protected override EditBoxWrapper CreateEditBoxWrapper() {
			return (IsTokenMode && IsTokenEditorInEditCore()) ? CreateTokenEditorWrapper() : base.CreateEditBoxWrapper();
		}
		private bool IsTokenEditorInEditCore() {
			return EditCore != null && EditCore is TokenEditor;
		}
		private EditBoxWrapper CreateTokenEditorWrapper() {
			return new TokenEditorWrapper(this);
		}
		protected virtual void IsAsyncOperationInProgressChanged(bool oldValue, bool newValue) {
			LogBase.Add(this, newValue);
			PropertyProvider.ShowWaitIndicator = ShowPopupWaitIndicator && newValue;
		}
		internal bool GetIsEditorKeyboardFocused() { return IsEditorKeyboardFocused; }
		internal bool GetIsPopupCloseInProgress() { return IsPopupCloseInProgress; }
		protected virtual void AllowRejectUnknownValuesChanged(bool newValue) {
			EditStrategy.AllowRejectUnknownValuesChanged(newValue);
		}
		protected virtual void AllowLiveDataShapingChanged(bool newValue) {
			Settings.AllowLiveDataShaping = newValue;
		}
	}
	public enum ChangeSelectionAction {
		SelectAll,
		UnselectAll,
	}
}

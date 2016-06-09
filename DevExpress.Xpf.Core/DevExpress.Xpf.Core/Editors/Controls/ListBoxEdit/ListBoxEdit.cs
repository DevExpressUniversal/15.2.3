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

using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
#if !SL
using DevExpress.Utils.Design.DataAccess;
using DevExpress.Xpf.Editors.Filtering;
#else
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[ContentProperty("Items")]
	[ComplexBindingProperties("ItemsSource", "ValueMember")]
	[LookupBindingProperties("ItemsSource", "DisplayMember", "ValueMember", "DisplayMember")]
#if !SL
	[DataAccessMetadata("All", SupportedProcessingModes = "GridLookup")]
#endif
	public partial class ListBoxEdit : BaseEdit, ISelectorEdit, IEventArgsConverterSource, IFilteredComponent {
		#region static
		public static readonly DependencyProperty FilterCriteriaProperty;
		public static readonly DependencyProperty ShowCustomItemsProperty;
		public static readonly DependencyProperty AllowCollectionViewProperty;
		public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty SelectionModeProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		protected static readonly DependencyPropertyKey SelectedItemsPropertyKey;
		public static readonly DependencyProperty SelectedItemsProperty;
		public static readonly DependencyProperty SelectedIndexProperty;
		public static readonly DependencyProperty DisplayMemberProperty;
		public static readonly DependencyProperty ValueMemberProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemsPanelProperty;
		public static readonly DependencyProperty ItemContainerStyleProperty;
		public static readonly RoutedEvent SelectedIndexChangedEvent;
		public static readonly DependencyProperty AllowItemHighlightingProperty;
		public static readonly DependencyProperty AllowRejectUnknownValuesProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty AllowLiveDataShapingProperty;
		protected static readonly DependencyPropertyKey IsAsyncOperationInProgressPropertyKey;
		public static readonly DependencyProperty IsAsyncOperationInProgressProperty;
		public static readonly DependencyProperty ShowWaitIndicatorProperty;
		static ListBoxEdit() {
			Type ownerType = typeof(ListBoxEdit);
			FilterCriteriaProperty = DependencyPropertyManager.Register("FilterCriteria", typeof(CriteriaOperator), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((ListBoxEdit)d).FilterCriteriaChanged((CriteriaOperator)e.NewValue)));
			ShowCustomItemsProperty = DependencyPropertyManager.Register("ShowCustomItems", typeof(bool?), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((ListBoxEdit)d).ShowCustomItemsChanged((bool?)e.NewValue)));
			AllowCollectionViewProperty = DependencyPropertyManager.Register("AllowCollectionView", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, (d, e) => ((ListBoxEdit)d).AllowCollectionViewChanged((bool)e.NewValue)));
			IsSynchronizedWithCurrentItemProperty = DependencyPropertyManager.Register("IsSynchronizedWithCurrentItem", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((ListBoxEdit)d).IsSynchronizedWithCurrentItemChanged((bool)e.NewValue)));
			ItemContainerStyleProperty = ComboBox.ItemContainerStyleProperty.AddOwner(ownerType);
			ItemsPanelProperty = ItemsControl.ItemsPanelProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(ItemsControl.ItemsPanelProperty.GetMetadata(typeof(ItemsControl)).DefaultValue, new PropertyChangedCallback(OnItemsPanelChanged)));
			ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(object), ownerType, new FrameworkPropertyMetadata(null, OnItemsSourceChanged));
			SelectionModeProperty = ListBox.SelectionModeProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(SelectionMode.Single, SelectionModeChanged));
			SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged, OnSelectedItemCoerce));
			SelectedIndexProperty = DependencyPropertyManager.Register("SelectedIndex", typeof(int), ownerType,
				new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedIndexChanged, OnSelectedIndexCoerce));
			SelectedItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("SelectedItems", typeof(ObservableCollection<object>), ownerType, new PropertyMetadata(null, OnSelectedItemsChanged));
			SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;
			SelectedIndexChangedEvent = EventManager.RegisterRoutedEvent("SelectedIndexChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), ownerType);
			DisplayMemberProperty = DependencyPropertyManager.Register("DisplayMember", typeof(string), ownerType,
				new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, OnDisplayMemberChanged));
			ValueMemberProperty = DependencyPropertyManager.Register("ValueMember", typeof(string), ownerType,
				new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, OnValueMemberChanged));
			ItemTemplateProperty = ItemsControl.ItemTemplateProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnItemTemplateChanged));
			AllowItemHighlightingProperty = DependencyPropertyManager.Register("AllowItemHighlighting", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			AllowRejectUnknownValuesProperty = DependencyPropertyRegistrator.Register<ListBoxEdit, bool>(
				owner => owner.AllowRejectUnknownValues, false, (@base, value, newValue) => @base.AllowRejectUnknownValuesChanged(newValue));
			EditValueProperty.OverrideMetadata(ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, null, true, UpdateSourceTrigger.PropertyChanged));
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			ItemTemplateSelectorProperty = ItemsControl.ItemTemplateSelectorProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(null, OnItemTemplateSelectorChanged));
			AllowLiveDataShapingProperty = DependencyPropertyManager.Register("AllowLiveDataShaping", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, (o, args) => ((ListBoxEdit)o).AllowLiveDataShapingChanged((bool)args.NewValue)));
			IsAsyncOperationInProgressPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<ListBoxEdit, bool>(
				owner => owner.IsAsyncOperationInProgress, false, (owner, oldValue, newValue) => owner.IsAsyncOperationInProgressChanged(oldValue, newValue));
			IsAsyncOperationInProgressProperty = IsAsyncOperationInProgressPropertyKey.DependencyProperty;
			ShowWaitIndicatorProperty = DependencyPropertyManager.Register("ShowWaitIndicator", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		protected virtual void AllowLiveDataShapingChanged(bool newValue) {
			Settings.AllowLiveDataShaping = newValue;
		}
		protected virtual void AllowRejectUnknownValuesChanged(bool newValue) {
			EditStrategy.AllowRejectUnknownValuesChanged(newValue);
		}
		protected static object OnSelectedIndexCoerce(DependencyObject obj, object baseValue) {
			return ((ListBoxEdit)obj).OnSelectedIndexCoerce(baseValue);
		}
		protected static object OnSelectedItemCoerce(DependencyObject obj, object baseValue) {
			return ((ListBoxEdit)obj).OnSelectedItemCoerce(baseValue);
		}
		static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ListBoxEdit)obj).OnItemsSourceChanged(e.NewValue);
		}
		static void SelectionModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ListBoxEdit)obj).SelectionModeChanged();
		}
		static void OnDisplayMemberChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ListBoxEdit)obj).OnDisplayMemberChanged();
		}
		static void OnValueMemberChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ListBoxEdit)obj).OnValueMemberChanged();
		}
		static void OnItemTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ListBoxEdit)obj).OnItemTemplateChanged();
		}
		static void OnItemsPanelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ListBoxEdit)obj).OnItemsPanelChanged();
		}
		static void OnItemTemplateSelectorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ListBoxEdit)obj).OnItemTemplateSelectorChanged();
		}
		static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ListBoxEdit)obj).OnSelectedItemChanged(e.OldValue, e.NewValue);
		}
		static void OnSelectedItemsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ListBoxEdit)obj).OnSelectedItemsChanged((IList)e.NewValue);
		}
		static void OnSelectedIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ListBoxEdit)obj).OnSelectedIndexChanged((int)e.OldValue, (int)e.NewValue);
		}
		#endregion
		readonly Locker selectedItemsInitialeLocker = new Locker();
		ItemsProviderChangedEventHandler<ListBoxEdit> ItemsProviderChangedEventHandler { get; set; }
		public ListBoxEdit() {
			using (selectedItemsInitialeLocker.Lock()) {
				ObservableCollection<object> observables = new ObservableCollection<object>();
				observables.CollectionChanged += OnSelectedItemsCollectionChanged;
				SelectedItems = observables;
			}
			ItemsProviderChangedEventHandler = new ItemsProviderChangedEventHandler<ListBoxEdit>(this, (owner, o, e) => owner.EditStrategy.ItemsProviderChanged(e));
		}
		#region public properties
		public bool IsAsyncOperationInProgress {
			get { return (bool)GetValue(IsAsyncOperationInProgressProperty); }
			internal set { SetValue(IsAsyncOperationInProgressPropertyKey, value); }
		}
		public bool ShowWaitIndicator {
			get { return (bool)GetValue(ShowWaitIndicatorProperty); }
			set { SetValue(ShowWaitIndicatorProperty, value); }
		}
		public bool AllowLiveDataShaping {
			get { return (bool)GetValue(AllowLiveDataShapingProperty); }
			set { SetValue(AllowLiveDataShapingProperty, value); }
		}
		public bool AllowRejectUnknownValues {
			get { return (bool)GetValue(AllowRejectUnknownValuesProperty); }
			set { SetValue(AllowRejectUnknownValuesProperty, value); }
		}
		public ObservableCollection<GroupStyle> GroupStyle {
			get { return Settings.GroupStyle; }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditItemsSource"),
#endif
 Category("Common Properties"), Bindable(true)]
		public object ItemsSource {
			get { return (object)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSelectionMode"),
#endif
 Category("Common Properties")]
		public SelectionMode SelectionMode {
			get { return (SelectionMode)GetValue(SelectionModeProperty); }
			set { SetValue(SelectionModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSelectedIndex"),
#endif
 Category("Common Properties")]
		public int SelectedIndex {
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSelectedItem"),
#endif
 Category("Common Properties")]
		public object SelectedItem {
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditDisplayMember"),
#endif
 Category("Common Properties"), DefaultValue("")]
		public string DisplayMember {
			get { return (string)GetValue(DisplayMemberProperty); }
			set { SetValue(DisplayMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditValueMember"),
#endif
 Category("Common Properties"), DefaultValue("")]
		public string ValueMember {
			get { return (string)GetValue(ValueMemberProperty); }
			set { SetValue(ValueMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSelectedItems"),
#endif
 Category("Common Properties")]
		public ObservableCollection<object> SelectedItems {
			get { return (ObservableCollection<object>)GetValue(SelectedItemsProperty); }
			private set { this.SetValue(SelectedItemsPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ListBoxEditAllowItemHighlighting")]
#endif
		public bool AllowItemHighlighting {
			get { return (bool)GetValue(AllowItemHighlightingProperty); }
			set { SetValue(AllowItemHighlightingProperty, value); }
		}
		[Browsable(false)]
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		[Browsable(false)]
		public ItemsPanelTemplate ItemsPanel {
			get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
			set { SetValue(ItemsPanelProperty, value); }
		}
		[Browsable(false)]
		public Style ItemContainerStyle {
			get { return (Style)GetValue(ItemContainerStyleProperty); }
			set { SetValue(ItemContainerStyleProperty, value); }
		}
		[Browsable(false)]
		public CriteriaOperator FilterCriteria {
			get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); }
			set { SetValue(FilterCriteriaProperty, value); }
		}
#if !SL
		[Browsable(false)]
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
#endif
		[Category("Behavior")]
		public bool AllowCollectionView {
			get { return (bool)GetValue(AllowCollectionViewProperty); }
			set { SetValue(AllowCollectionViewProperty, value); }
		}
		[Category("Behavior")]
		public bool IsSynchronizedWithCurrentItem {
			get { return (bool)GetValue(IsSynchronizedWithCurrentItemProperty); }
			set { SetValue(IsSynchronizedWithCurrentItemProperty, value); }
		}
		#endregion
		#region events
		[Category("Action")]
		public event RoutedEventHandler SelectedIndexChanged {
			add { AddHandler(SelectedIndexChangedEvent, value); }
			remove { RemoveHandler(SelectedIndexChangedEvent, value); }
		}
		#endregion
		protected internal new ListBoxEditSettings Settings {
			get { return (ListBoxEditSettings)base.Settings; }
		}
		protected internal IItemsProvider2 ItemsProvider {
			get { return Settings.ItemsProvider; }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditItems"),
#endif
 Category("Common Properties"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ListItemCollection Items {
			get { return Settings.Items; }
		}
		public ICommand SelectAllItemsCommand { get; private set; }
		new ListBoxEditBasePropertyProvider PropertyProvider {
			get { return base.PropertyProvider as ListBoxEditBasePropertyProvider; }
		}
		public bool? ShowCustomItems {
			get { return (bool?)GetValue(ShowCustomItemsProperty); }
			set { SetValue(ShowCustomItemsProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		[Browsable(true)]
		public new BaseEditStyleSettings StyleSettings {
			get { return base.StyleSettings; }
			set { base.StyleSettings = value; }
		}
		protected internal override Type StyleSettingsType {
			get { return typeof(ListBoxEditStyleSettings); }
		}
		protected internal EditorListBox ListBoxCore {
			get { return EditCore as EditorListBox; }
		}
		protected new ListBoxEditStrategy EditStrategy {
			get { return base.EditStrategy as ListBoxEditStrategy; }
			set { base.EditStrategy = value; }
		}
		void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			EditStrategy.SelectedItemsChanged(null, SelectedItems);
		}
		protected virtual void IsAsyncOperationInProgressChanged(bool oldValue, bool newValue) {
			PropertyProvider.ShowWaitIndicator = ShowWaitIndicator && newValue;
		}
		protected virtual void OnItemsSourceChanged(object itemsSource) {
			EditStrategy.ItemSourceChanged(itemsSource);
			RaiseFilteredComponentPropertieChanged();
		}
		protected virtual void SelectionModeChanged() {
			EditStrategy.ApplyStyleSettings(PropertyProvider.StyleSettings);
			EditStrategy.SyncWithValue();
		}
		protected virtual int CoerceSelectedIndex(int index) {
			return EditStrategy.CoerceSelectedIndex(index);
		}
		protected virtual object CoerceSelectedItem(object item) {
			return EditStrategy.CoerceSelectedItem(item);
		}
		protected virtual void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			EditStrategy.SelectedItemsChanged(null, SelectedItems);
		}
		protected virtual void OnDisplayMemberChanged() {
			Settings.DisplayMember = DisplayMember;
			UpdateDisplayMemberPath();
			RaiseFilteredComponentPropertieChanged();
		}
		void UpdateDisplayMemberPath() {
			IValueConverter converter = new PopupListBoxDisplayMemberPathConverter();
			PropertyProvider.DisplayMemberPath = (string)converter.Convert(this, null, null, null);
		}
		protected virtual void OnValueMemberChanged() {
			Settings.ValueMember = ValueMember;
			RaiseFilteredComponentPropertieChanged();
		}
		protected override void UnsubscribeFromSettings(BaseEditSettings settings) {
			base.UnsubscribeFromSettings(settings);
			ListBoxEditSettings listBoxSettings = (ListBoxEditSettings)settings;
			if (listBoxSettings == null)
				return;
			listBoxSettings.ItemsProvider.ItemsProviderChanged -= ItemsProviderChangedEventHandler.Handler;
		}
		protected override void SubscribeToSettings(BaseEditSettings settings) {
			base.SubscribeToSettings(settings);
			ListBoxEditSettings listBoxSettings = (ListBoxEditSettings)settings;
			if (listBoxSettings == null)
				return;
			listBoxSettings.ItemsProvider.ItemsProviderChanged += ItemsProviderChangedEventHandler.Handler;
		}
		protected override void OnEditCoreAssigned() {
			base.OnEditCoreAssigned();
			EditStrategy.ApplyStyleSettings(PropertyProvider.StyleSettings);
		}
		protected internal override bool NeedsKey(Key key, ModifierKeys modifiers) {
			if (EditMode == EditMode.InplaceActive && !ModifierKeysHelper.IsCtrlPressed(modifiers))
				return false;
			return base.NeedsKey(key, modifiers);
		}
		protected virtual void OnItemTemplateChanged() {
			UpdateDisplayMemberPath();
		}
		protected virtual void OnItemsPanelChanged() {
			Settings.ItemsPanel = ItemsPanel;
		}
#if !SL
		protected virtual void OnItemTemplateSelectorChanged() {
			Settings.ItemTemplateSelector = ItemTemplateSelector;
		}
#endif
		protected override EditStrategyBase CreateEditStrategy() {
			return new ListBoxEditStrategy(this);
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new ListBoxEditBasePropertyProvider(this);
		}
		public override void SelectAll() {
			if (EditMode == Editors.EditMode.Standalone)
				EditStrategy.SelectAll();
			else
				UpdateSelectedItemFocus();
		}
		public void UnSelectAll() {
			EditStrategy.UnSelectAll();
		}
		public void ScrollIntoView(object item) {
			if (ListBoxCore != null)
				ListBoxCore.ScrollIntoView(item);
		}
		protected override void OnGotFocus(System.Windows.RoutedEventArgs e) {
			base.OnGotFocus(e);
#if !SL
			UpdateSelectedItemFocus();
#endif
		}
		void UpdateSelectedItemFocus() {
			if (ListBoxCore == null || EditMode == EditMode.InplaceInactive)
				return;
			ListBoxCore.FocusSelectedItem();
		}
		protected virtual void OnSelectedItemChanged(object oldValue, object newValue) {
			EditStrategy.SelectedItemChanged(oldValue, newValue);
		}
		protected virtual void OnSelectedItemsChanged(IList selectedItems) {
			if (selectedItemsInitialeLocker.IsLocked)
				return;
			EditStrategy.SelectedItemsChanged(null, selectedItems);
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
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new ListBoxEditAutomationPeer(this);
		}
		protected override bool FocusEditCore() {
			if (Focusable)
				UpdateSelectedItemFocus();
			return true;
		}
		protected virtual void IsSynchronizedWithCurrentItemChanged(bool value) {
			EditStrategy.IsSynchronizedWithCurrentItemChanged(value);
		}
		protected virtual void AllowCollectionViewChanged(bool value) {
			EditStrategy.AllowCollectionViewChanged(value);
		}
		protected virtual void ShowCustomItemsChanged(bool? value) {
			EditStrategy.ShowCustomItemsChanged(value);
		}
		protected virtual void FilterCriteriaChanged(CriteriaOperator criteriaOperator) {
			EditStrategy.FilterCriteriaChanged(criteriaOperator);
			RaiseFilteredComponentRowFilterChanged();
		}
		#region ISelectorEdit members
		EditStrategyBase ISelectorEdit.EditStrategy { get { return EditStrategy; } }
		SelectionEventMode ISelectorEdit.SelectionEventMode { get { return PropertyProvider.SelectionEventMode; } }
		object ISelectorEdit.GetCurrentSelectedItem() {
			return EditStrategy.GetCurrentSelectedItem();
		}
		IEnumerable ISelectorEdit.GetCurrentSelectedItems() {
			return EditStrategy.GetCurrentSelectedItems();
		}
		bool ISelectorEdit.UseCustomItems { get { return PropertyProvider.ShowCustomItems(); } }
		ISelectionProvider ISelectorEdit.SelectionProvider {
			get { return new SelectionProvider((ISelectorEditInnerListBox)EditCore); }
		}
		IListNotificationOwner ISelectorEdit.ListNotificationOwner {
			get { return Settings; }
		}
		IItemsProvider2 ISelectorEdit.ItemsProvider {
			get { return ItemsProvider; }
		}
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
		protected internal virtual void SubscribeToItemsProviderChanged() {
			ItemsProvider.ItemsProviderChanged += ItemsProviderChangedEventHandler.Handler;
		}
		object eventArgsConverter;
		object IEventArgsConverterSource.EventArgsConverter {
			get { return eventArgsConverter ?? (eventArgsConverter = new ListBoxEditEventArgsConverter(this)); }
		}
		#region IFilteredComponent Members
		System.Collections.Generic.IEnumerable<FilterColumn> IFilteredComponent.CreateFilterColumnCollection() {
			return FilteredComponentHelper.GetColumnsByItemsSource(this, ItemsSource);
		}
		void RaiseFilteredComponentPropertieChanged() {
			rowFilterChanged.Do(x => x(this, EventArgs.Empty));
		}
		void RaiseFilteredComponentRowFilterChanged() {
			rowFilterChanged.Do(x => x(this, EventArgs.Empty));
		}
		EventHandler propertiesChanged;
		EventHandler rowFilterChanged;
		event EventHandler IFilteredComponentBase.PropertiesChanged { add { propertiesChanged += value; } remove { propertiesChanged -= value; } }
		CriteriaOperator IFilteredComponentBase.RowCriteria { get { return FilterCriteria; } set { FilterCriteria = value; } }
		event EventHandler IFilteredComponentBase.RowFilterChanged { add { rowFilterChanged += value; } remove { rowFilterChanged -= value; } }
		#endregion
	}
	class ListBoxEditEventArgsConverter : IDataRowEventArgsConverter {
		ListBoxEdit OwnerEdit { get; set; }
		public ListBoxEditEventArgsConverter(ListBoxEdit dataControl) {
			OwnerEdit = dataControl;
		}
		object IDataRowEventArgsConverter.GetDataRow(System.Windows.RoutedEventArgs e) {
			return (OwnerEdit.ListBoxCore).With(x => x.SelectedItem);
		}
	}
}

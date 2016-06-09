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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Collections;
using System.Windows.Controls;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Utils.Design.DataAccess;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors.Settings {
#if !SL
	[DataAccessMetadata("All", SupportedProcessingModes = "GridLookup", DataSourceProperty = "ItemsSource", Platform = "WPF")]
	[LookupBindingProperties("ItemsSource", "DisplayMember", "ValueMember", "DisplayMember")]
#endif
	public class LookUpEditSettingsBase : PopupBaseEditSettings, IItemsProviderOwner, IListNotificationOwner {
		IItemsProvider2 itemsProvider;
		protected internal override bool RequireDisplayTextSorting { get { return true; } }
		#region static
		public static readonly DependencyProperty FilterCriteriaProperty;
		public static readonly DependencyProperty AllowCollectionViewProperty;
		public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemsPanelProperty;
		public static readonly DependencyProperty ApplyItemTemplateToSelectedItemProperty;
		public static readonly DependencyProperty AutoCompleteProperty;
		public static readonly DependencyProperty IsCaseSensitiveSearchProperty;
		public static readonly DependencyProperty ImmediatePopupProperty;
		public static readonly DependencyProperty DisplayMemberProperty;
		public static readonly DependencyProperty ValueMemberProperty;
		public static readonly DependencyProperty IncrementalFilteringProperty;
		public static readonly DependencyProperty SeparatorStringProperty;
		public static readonly DependencyProperty AddNewButtonPlacementProperty;
		public static readonly DependencyProperty FindButtonPlacementProperty;
		public static readonly DependencyProperty FilterConditionProperty;
		public static readonly DependencyProperty FindModeProperty;
		public static readonly DependencyProperty AllowRejectUnknownValuesProperty;
		public static readonly DependencyProperty AllowItemHighlightingProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty AllowLiveDataShapingProperty;
		static LookUpEditSettingsBase() {
			Type ownerType = typeof(LookUpEditSettingsBase);
			AllowCollectionViewProperty = DependencyPropertyManager.Register("AllowCollectionView", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None,
				(d, e) => ((LookUpEditSettingsBase)d).AllowCollectionViewChanged((bool)e.NewValue)));
			FilterCriteriaProperty = DependencyPropertyManager.Register("FilterCriteria", typeof(CriteriaOperator), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => ((LookUpEditSettingsBase)d).FilterCriteriaChanged((CriteriaOperator)e.NewValue)));
			IsSynchronizedWithCurrentItemProperty = DependencyPropertyManager.Register("IsSynchronizedWithCurrentItem", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((LookUpEditSettingsBase)d).IsSynchronizedWithCurrentItemChanged((bool)e.NewValue)));
			ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(object), ownerType, new PropertyMetadata(null, OnItemsSourceChanged));
			ItemTemplateProperty = LookUpEditBase.ItemTemplateProperty.AddOwner(ownerType);
			ItemsPanelProperty = LookUpEditBase.ItemsPanelProperty.AddOwner(ownerType);
			ApplyItemTemplateToSelectedItemProperty = LookUpEditBase.ApplyItemTemplateToSelectedItemProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(false));
			AutoCompleteProperty = LookUpEditBase.AutoCompleteProperty.AddOwner(ownerType);
			IsCaseSensitiveSearchProperty = LookUpEditBase.IsCaseSensitiveSearchProperty.AddOwner(ownerType);
			ImmediatePopupProperty = LookUpEditBase.ImmediatePopupProperty.AddOwner(ownerType);
			DisplayMemberProperty = LookUpEditBase.DisplayMemberProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(string.Empty, OnDisplayMemberChanged));
			ValueMemberProperty = LookUpEditBase.ValueMemberProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(string.Empty, OnValueMemberChanged));
			IncrementalFilteringProperty = LookUpEditBase.IncrementalFilteringProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(null, OnSettingsPropertyChanged));
			SeparatorStringProperty = LookUpEditBase.SeparatorStringProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(";", OnSettingsPropertyChanged));
			AddNewButtonPlacementProperty = LookUpEditBase.AddNewButtonPlacementProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(OnSettingsPropertyChanged));
			FindButtonPlacementProperty = LookUpEditBase.FindButtonPlacementProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(OnSettingsPropertyChanged));
			FilterConditionProperty = LookUpEditBase.FilterConditionProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((LookUpEditSettingsBase)d).FilterConditionChanged((FilterCondition?)e.NewValue)));
			FindModeProperty = LookUpEditBase.FindModeProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((LookUpEditSettingsBase)d).FindModeChanged((FindMode?)e.NewValue)));
			AllowRejectUnknownValuesProperty = LookUpEditBase.AllowRejectUnknownValuesProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((LookUpEditSettingsBase)d).AllowRejectUnknownValuesChanged((bool)e.NewValue)));
			AllowLiveDataShapingProperty = LookUpEditBase.AllowLiveDataShapingProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(true, (d, e) => ((LookUpEditSettingsBase)d).AllowLiveDataShapingChanged((bool)e.NewValue)));
			AllowItemHighlightingProperty = DependencyPropertyManager.Register("AllowItemHighlighting", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ItemTemplateSelectorProperty = ComboBoxEdit.ItemTemplateSelectorProperty.AddOwner(ownerType);
			HorizontalContentAlignmentProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(EditSettingsHorizontalAlignment.Left, OnSettingsPropertyChanged));
		}
		static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((LookUpEditSettingsBase)d).OnItemsSourceChanged(e.NewValue);
		}
		static void OnDisplayMemberChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditSettingsBase)obj).OnDisplayMemberChanged();
		}
		static void OnValueMemberChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LookUpEditSettingsBase)obj).OnValueMemberChanged();
		}
		#endregion
		public LookUpEditSettingsBase() {
		}
		public bool AllowLiveDataShaping {
			get { return (bool)GetValue(AllowLiveDataShapingProperty); }
			set { SetValue(AllowLiveDataShapingProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public EditorPlacement? FindButtonPlacement {
			get { return (EditorPlacement?)GetValue(FindButtonPlacementProperty); }
			set { SetValue(FindButtonPlacementProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public EditorPlacement? AddNewButtonPlacement {
			get { return (EditorPlacement?)GetValue(AddNewButtonPlacementProperty); }
			set { SetValue(AddNewButtonPlacementProperty, value); }
		}
		[ Category(EditSettingsCategories.Appearance)]
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
#if !SL
		[ Category(EditSettingsCategories.Appearance)]
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		[ Category(EditSettingsCategories.Appearance)]
		public bool AllowItemHighlighting {
			get { return (bool)GetValue(AllowItemHighlightingProperty); }
			set { SetValue(AllowItemHighlightingProperty, value); }
		}
#endif
		[ Category(EditSettingsCategories.Appearance)]
		public ItemsPanelTemplate ItemsPanel {
			get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
			set { SetValue(ItemsPanelProperty, value); }
		}
		[ Category(EditSettingsCategories.Behavior)]
		public bool ApplyItemTemplateToSelectedItem {
			get { return (bool)GetValue(ApplyItemTemplateToSelectedItemProperty); }
			set { SetValue(ApplyItemTemplateToSelectedItemProperty, value); }
		}
		[ Category(EditSettingsCategories.Behavior)]
		public bool AutoComplete {
			get { return (bool)GetValue(AutoCompleteProperty); }
			set { SetValue(AutoCompleteProperty, value); }
		}
		[ Category(EditSettingsCategories.Behavior)]
		public bool IsCaseSensitiveSearch {
			get { return (bool)GetValue(IsCaseSensitiveSearchProperty); }
			set { SetValue(IsCaseSensitiveSearchProperty, value); }
		}
		[ Category(EditSettingsCategories.Behavior)]
		public bool ImmediatePopup {
			get { return (bool)GetValue(ImmediatePopupProperty); }
			set { SetValue(ImmediatePopupProperty, value); }
		}
		[ Category(EditSettingsCategories.Behavior)]
		public bool? IncrementalFiltering {
			get { return (bool?)GetValue(IncrementalFilteringProperty); }
			set { SetValue(IncrementalFilteringProperty, value); }
		}
		[ Category(EditSettingsCategories.Behavior)]
		public string SeparatorString {
			get { return (string)GetValue(SeparatorStringProperty); }
			set { SetValue(SeparatorStringProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public bool IsSynchronizedWithCurrentItem {
			get { return (bool)GetValue(IsSynchronizedWithCurrentItemProperty); }
			set { SetValue(IsSynchronizedWithCurrentItemProperty, value); }
		}
		protected virtual IItemsProvider2 CreateItemsProvider() {
			return new ItemsProvider2(this);
		}
		protected internal IItemsProvider2 ItemsProvider { get { return itemsProvider ?? (itemsProvider = CreateItemsProvider()); } }
		[ Category(EditSettingsCategories.Data)]
		public object ItemsSource {
			get { return GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		[ Category(EditSettingsCategories.Data)]
		public string DisplayMember {
			get { return (string)GetValue(DisplayMemberProperty); }
			set { SetValue(DisplayMemberProperty, value); }
		}
		bool isEnumItemsSourceAssigned = false;
		string CachedValueMember { get; set; }
		string CachedDisplayMember { get; set; }
		bool CachedImageTemplateToSelectedItem { get; set; }
		string ActualValueMember { get; set; }
		string ActualDisplayMember { get; set; }
		bool ActualCachedImageTemplateToSelectedItem { get; set; }
		bool ActualAllowRejectUnknownValues { get; set; }
		[ Category(EditSettingsCategories.Data)]
		public string ValueMember {
			get { return (string)GetValue(ValueMemberProperty); }
			set { SetValue(ValueMemberProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public bool AllowCollectionView {
			get { return (bool)GetValue(AllowCollectionViewProperty); }
			set { SetValue(AllowCollectionViewProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public FilterCondition? FilterCondition {
			get { return (FilterCondition?)GetValue(FilterConditionProperty); }
			set { SetValue(FilterConditionProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public FindMode? FindMode {
			get { return (FindMode?)GetValue(FindModeProperty); }
			set { SetValue(FindModeProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public CriteriaOperator FilterCriteria {
			get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); }
			set { SetValue(FilterCriteriaProperty, value); }
		}
		[Category(EditSettingsCategories.Behavior)]
		public bool AllowRejectUnknownValues {
			get { return (bool)GetValue(AllowRejectUnknownValuesProperty); }
			set { SetValue(AllowRejectUnknownValuesProperty, value); }
		}
		void IListNotificationOwner.OnCollectionChanged(NotifyItemsProviderChangedEventArgs e) {
			OnCollectionChanged(e);
		}
		void IListNotificationOwner.OnCollectionChanged(NotifyItemsProviderSelectionChangedEventArgs e) {
			OnCollectionChanged(e);
		}
		protected virtual void AllowRejectUnknownValuesChanged(bool newValue) {
			ActualAllowRejectUnknownValues = newValue;
			ItemsProvider.UpdateValueMember();
			OnSettingsChanged();
		}
		void AllowLiveDataShapingChanged(bool newValue) {
			ItemsProvider.UpdateItemsSource();
			ItemsProvider.SyncWithCurrentItem();
			OnSettingsChanged();
		}
		protected virtual void OnDisplayMemberChanged() {
			CachedDisplayMember = DisplayMember;
			UpdateActualDisplayMember();
			ItemsProvider.UpdateDisplayMember();
			OnSettingsChanged();
		}
		void UpdateActualDisplayMember() {
			ActualDisplayMember = CalcActualDisplayMember();
		}
		string CalcActualDisplayMember() {
			return isEnumItemsSourceAssigned ? EnumSourceHelperCore.DisplayMemberName : CachedDisplayMember;
		}
		protected virtual void OnValueMemberChanged() {
			CachedValueMember = ValueMember;
			UpdateActualValueMember();
			ItemsProvider.UpdateValueMember();
			OnSettingsChanged();
		}
		void UpdateActualValueMember() {
			ActualValueMember = CalcActualValueMember();
		}
		string CalcActualValueMember() {
			return isEnumItemsSourceAssigned ? EnumSourceHelperCore.ValueMemberName : CachedValueMember;
		}
		protected virtual void OnCollectionChanged(NotifyItemsProviderSelectionChangedEventArgs e) {
			ItemsProvider.ProcessSelectionChanged(e);
			OnSettingsChanged();
		}
		protected virtual void OnCollectionChanged(NotifyItemsProviderChangedEventArgs e) {
			ItemsProvider.ProcessCollectionChanged(e);
			OnSettingsChanged();
		}
		public object GetItemFromValue(object value) {
			return ItemsProvider.GetItem(value, ItemsProvider.CurrentDataViewHandle);
		}
		public object GetValueFromItem(object item) {
			return ItemsProvider.GetValueFromItem(item, ItemsProvider.CurrentDataViewHandle);
		}
		protected virtual void OnItemsSourceChanged(object itemsSource) {
			SetupItemsSource(itemsSource);
			ItemsProvider.UpdateItemsSource();
			ItemsProvider.SyncWithCurrentItem();
			OnSettingsChanged();
		}
		void SetupItemsSource(object itemsSource) {
			isEnumItemsSourceAssigned = EnumItemsSource.IsEnumItemsSource(itemsSource);
			UpdateActualDisplayMember();
			UpdateActualValueMember();
		}
		protected virtual void IsSynchronizedWithCurrentItemChanged(bool value) {
			ItemsProvider.SyncWithCurrentItem();
			OnSettingsChanged();
		}
		protected virtual void AllowCollectionViewChanged(bool value) {
			ItemsProvider.Reset();
			OnSettingsChanged();
		}
		protected virtual void FindModeChanged(FindMode? findMode) {
			OnSettingsChanged();
		}
		protected virtual void FilterConditionChanged(FilterCondition? filterCondition) {
			ItemsProvider.UpdateFilterCriteria();
			OnSettingsChanged();
		}
		protected virtual void FilterCriteriaChanged(CriteriaOperator criteriaOperator) {
			ItemsProvider.UpdateFilterCriteria();
			OnSettingsChanged();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			IItemsProviderOwner This = this;
			This.IsLoaded = true;
			ItemsProvider.DoRefresh();
		}
		protected override void OnUnloaded() {
			base.OnUnloaded();
			IItemsProviderOwner This = this;
			This.IsLoaded = false;
			ItemsProvider.DoRefresh();
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			LookUpEditBase cb = edit as LookUpEditBase;
			if (cb == null)
				return;
			SetValueFromSettings(DisplayMemberProperty, () => cb.DisplayMember = DisplayMember);
			SetValueFromSettings(ValueMemberProperty, () => cb.ValueMember = ValueMember);
			SetValueFromSettings(AllowRejectUnknownValuesProperty, () => cb.AllowRejectUnknownValues = AllowRejectUnknownValues);
			SetValueFromSettings(AllowLiveDataShapingProperty, () => cb.AllowLiveDataShaping = AllowLiveDataShaping);
			SetValueFromSettings(ItemsSourceProperty, () => cb.ItemsSource = ItemsSource);
			SetValueFromSettings(ItemTemplateSelectorProperty, () => cb.ItemTemplateSelector = ItemTemplateSelector);
			SetValueFromSettings(AllowItemHighlightingProperty, () => cb.AllowItemHighlighting = AllowItemHighlighting);
			SetValueFromSettings(ItemsPanelProperty, () => cb.ItemsPanel = ItemsPanel);
			SetValueFromSettings(PopupMaxHeightProperty, () => cb.PopupMaxHeight = PopupMaxHeight);
			SetValueFromSettings(ApplyItemTemplateToSelectedItemProperty, () => cb.ApplyItemTemplateToSelectedItem = ApplyItemTemplateToSelectedItem);
			SetValueFromSettings(AutoCompleteProperty, () => cb.AutoComplete = AutoComplete);
			SetValueFromSettings(IsCaseSensitiveSearchProperty, () => cb.IsCaseSensitiveSearch = IsCaseSensitiveSearch);
			SetValueFromSettings(ImmediatePopupProperty, () => cb.ImmediatePopup = ImmediatePopup);
			SetValueFromSettings(SeparatorStringProperty, () => cb.SeparatorString = SeparatorString);
			SetValueFromSettings(IsSynchronizedWithCurrentItemProperty, () => cb.IsSynchronizedWithCurrentItem = IsSynchronizedWithCurrentItem);
			SetValueFromSettings(AllowCollectionViewProperty, () => cb.AllowCollectionView = AllowCollectionView);
			cb.IncrementalFiltering = IncrementalFiltering;
			SetValueFromSettings(AddNewButtonPlacementProperty, () => cb.AddNewButtonPlacement = AddNewButtonPlacement);
			SetValueFromSettings(FindButtonPlacementProperty, () => cb.FindButtonPlacement = FindButtonPlacement);
			SetValueFromSettings(FilterConditionProperty, () => cb.FindMode = FindMode);
			SetValueFromSettings(FilterConditionProperty, () => cb.FilterCondition = FilterCondition);
			SetValueFromSettings(ItemTemplateProperty,
				() => cb.ItemTemplate = ItemTemplate,
				() => ClearEditorPropertyIfNeeded(cb, LookUpEditBase.ItemTemplateProperty, ItemTemplateProperty));
		}
		#region IItemsProviderOwner implementation
		string IItemsProviderOwner.DisplayMember { get { return ActualDisplayMember; } set { DisplayMember = value; } }
		string IItemsProviderOwner.ValueMember { get { return ActualValueMember; } set { ValueMember = value; } }
		IItemsProvider2 IItemsProviderOwner.ItemsProvider { get { return ItemsProvider; } }
		bool IItemsProviderOwner.IsLoaded { get; set; }
		#endregion
		ListItemCollection IItemsProviderOwner.Items { get { return new ListItemCollection(this); } }
	}
}

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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Collections;
using System.Windows.Controls;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Utils;
using System.Collections.ObjectModel;
using DevExpress.Data;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
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
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Editors.Settings {
#if !SL
	[DataAccessMetadata("All", SupportedProcessingModes = "GridLookup", DataSourceProperty = "ItemsSource", Platform = "WPF")]
	[LookupBindingProperties("ItemsSource", "DisplayMember", "ValueMember", "DisplayMember")]
#endif
	public class ListBoxEditSettings : BaseEditSettings, IItemsProviderOwner, IListNotificationOwner {
		#region static
		public static readonly DependencyProperty FilterCriteriaProperty;
		public static readonly DependencyProperty AllowCollectionViewProperty;
		public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty SelectionModeProperty;
		public static readonly DependencyProperty DisplayMemberProperty;
		public static readonly DependencyProperty ValueMemberProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemsPanelProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty AllowRejectUnknownValuesProperty;
		public static readonly DependencyProperty AllowLiveDataShapingProperty;
		static ListBoxEditSettings() {
			Type ownerType = typeof(ListBoxEditSettings);
			FilterCriteriaProperty = DependencyPropertyManager.Register("FilterCriteria", typeof(CriteriaOperator), ownerType, new PropertyMetadata(null, (d, e) => ((ListBoxEditSettings)d).FilterCriteriaChanged((CriteriaOperator)e.NewValue)));
			ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(object), ownerType, new PropertyMetadata(null, OnItemsSourceChanged));
			SelectionModeProperty = ListBoxEdit.SelectionModeProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(SelectionMode.Single, OnSelectionModeChanged));
			DisplayMemberProperty = ListBoxEdit.DisplayMemberProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(string.Empty, OnDisplayMemberChanged));
			ValueMemberProperty = ListBoxEdit.ValueMemberProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(string.Empty, OnValueMemberChanged));
			ItemTemplateProperty = ListBoxEdit.ItemTemplateProperty.AddOwner(ownerType);
			ItemsPanelProperty = ListBoxEdit.ItemsPanelProperty.AddOwner(ownerType);
			AllowCollectionViewProperty = DependencyPropertyManager.Register("AllowCollectionView", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None,
				(d, e) => ((ListBoxEditSettings)d).AllowCollectionViewChanged((bool)e.NewValue)));
			;
			IsSynchronizedWithCurrentItemProperty = DependencyPropertyManager.Register("IsSynchronizedWithCurrentItem", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((ListBoxEditSettings)d).IsSynchronizedWithCurrentItemChanged((bool)e.NewValue)));
			ItemTemplateSelectorProperty = ListBoxEdit.ItemTemplateSelectorProperty.AddOwner(typeof(ListBoxEditSettings));
			AllowRejectUnknownValuesProperty = ListBoxEdit.AllowRejectUnknownValuesProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, (o, args) => ((ListBoxEditSettings)o).AllowRejectUnknownValuesChanged((bool)args.NewValue)));
			AllowLiveDataShapingProperty = ListBoxEdit.AllowLiveDataShapingProperty.AddOwner(ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, (o, args) => ((ListBoxEditSettings)o).AllowLiveDataShapingChanged((bool)args.NewValue)));
		}
		static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ListBoxEditSettings)d).OnItemsSourceChanged(e.NewValue);
		}
		static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ListBoxEditSettings)d).OnSelectionModeChanged();
		}
		static void OnDisplayMemberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ListBoxEditSettings)d).OnDisplayMemberChanged();
		}
		static void OnValueMemberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ListBoxEditSettings)d).OnValueMemberChanged();
		}
		#endregion
		IItemsProvider2 itemsProvider;
		ListItemCollection items;
		ObservableCollection<object> mruItems;
#if !SL
		ObservableCollection<GroupStyle> groupStyle;
#endif
		public ListBoxEditSettings() {
		}
		#region public properties
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSettingsItemsSource"),
#endif
		Category(EditSettingsCategories.Data)]
		public object ItemsSource {
			get { return (object)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSettingsSelectionMode"),
#endif
		Category(EditSettingsCategories.Behavior)]
		public SelectionMode SelectionMode {
			get { return (SelectionMode)GetValue(SelectionModeProperty); }
			set { SetValue(SelectionModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSettingsItemTemplate"),
#endif
		Category(EditSettingsCategories.Appearance)]
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSettingsItemTemplateSelector"),
#endif
		Category(EditSettingsCategories.Appearance)]
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSettingsItemsPanel"),
#endif
		Category(EditSettingsCategories.Appearance)]
		public ItemsPanelTemplate ItemsPanel {
			get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
			set { SetValue(ItemsPanelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSettingsDisplayMember"),
#endif
		Category(EditSettingsCategories.Data)]
		public string DisplayMember {
			get { return (string)GetValue(DisplayMemberProperty); }
			set { SetValue(DisplayMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ListBoxEditSettingsValueMember"),
#endif
		Category(EditSettingsCategories.Data)]
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
		public bool IsSynchronizedWithCurrentItem {
			get { return (bool)GetValue(IsSynchronizedWithCurrentItemProperty); }
			set { SetValue(IsSynchronizedWithCurrentItemProperty, value); }
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
		public bool AllowLiveDataShaping {
			get { return (bool)GetValue(AllowLiveDataShapingProperty); }
			set { SetValue(AllowLiveDataShapingProperty, value); }
		}
		#endregion
		protected virtual IItemsProvider2 CreateItemsProvider() {
			return new ItemsProvider2(this);
		}
		protected internal IItemsProvider2 ItemsProvider {
			get { return itemsProvider ?? (itemsProvider = CreateItemsProvider()); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ListBoxEditSettingsItems")]
#endif
		public ListItemCollection Items { get { return items ?? (items = new ListItemCollection(this)); } }
		public ObservableCollection<object> MRUItems { get { return mruItems ?? (mruItems = new ObservableCollection<object>()); } }
		public ObservableCollection<GroupStyle> GroupStyle { get { return groupStyle ?? (groupStyle = new ObservableCollection<GroupStyle>()); } }
		protected virtual void AllowRejectUnknownValuesChanged(bool newValue) {
			ItemsProvider.UpdateValueMember();
			OnSettingsChanged();
		}
		protected virtual void OnItemsSourceChanged(object itemsSource) {
			ItemsProvider.UpdateItemsSource();
			ItemsProvider.SyncWithCurrentItem();
			OnSettingsChanged();
		}
		protected virtual void OnSelectionModeChanged() {
			OnSettingsChanged();
		}
		protected virtual void OnDisplayMemberChanged() {
			ItemsProvider.UpdateDisplayMember();
			OnSettingsChanged();
		}
		void AllowLiveDataShapingChanged(bool newValue) {
			ItemsProvider.UpdateItemsSource();
			ItemsProvider.SyncWithCurrentItem();
			OnSettingsChanged();
		}
		protected virtual void OnValueMemberChanged() {
			ItemsProvider.UpdateValueMember();
			OnSettingsChanged();
		}
		void IListNotificationOwner.OnCollectionChanged(NotifyItemsProviderChangedEventArgs e) {
			OnCollectionChanged(e);
		}
		void IListNotificationOwner.OnCollectionChanged(NotifyItemsProviderSelectionChangedEventArgs e) {
			OnCollectionChanged(e);
		}
		protected virtual void OnCollectionChanged(NotifyItemsProviderChangedEventArgs e) {
			ItemsProvider.ProcessCollectionChanged(e);
			OnSettingsChanged();
		}
		protected virtual void OnCollectionChanged(NotifyItemsProviderSelectionChangedEventArgs e) {
			ItemsProvider.ProcessSelectionChanged(e);
			OnSettingsChanged();
		}
		protected virtual void IsSynchronizedWithCurrentItemChanged(bool value) {
			ItemsProvider.SyncWithCurrentItem();
			OnSettingsChanged();
		}
		protected virtual void AllowCollectionViewChanged(bool value) {
			ItemsProvider.Reset();
			OnSettingsChanged();
		}
		protected virtual void FilterCriteriaChanged(CriteriaOperator criteriaOperator) {
			ItemsProvider.UpdateFilterCriteria();
			OnSettingsChanged();
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			ListBoxEdit listBox = edit as ListBoxEdit;
			if (listBox == null)
				return;
			SetValueFromSettings(ItemsSourceProperty, () => listBox.ItemsSource = ItemsSource);
			SetValueFromSettings(DisplayMemberProperty, () => listBox.DisplayMember = DisplayMember);
			SetValueFromSettings(ValueMemberProperty, () => listBox.ValueMember = ValueMember);
			SetValueFromSettings(AllowRejectUnknownValuesProperty, () => listBox.AllowRejectUnknownValues = AllowRejectUnknownValues);
			SetValueFromSettings(AllowLiveDataShapingProperty, () => listBox.AllowLiveDataShaping = AllowLiveDataShaping);
			SetValueFromSettings(ItemTemplateProperty,
				() => listBox.ItemTemplate = ItemTemplate,
				() => ClearEditorPropertyIfNeeded(listBox, ListBoxEdit.ItemTemplateProperty, ItemTemplateProperty));
			SetValueFromSettings(SelectionModeProperty, () => listBox.SelectionMode = SelectionMode);
			SetValueFromSettings(IsSynchronizedWithCurrentItemProperty, () => listBox.IsSynchronizedWithCurrentItem = IsSynchronizedWithCurrentItem);
			SetValueFromSettings(AllowCollectionViewProperty, () => listBox.AllowCollectionView = AllowCollectionView);
			SetValueFromSettings(FilterCriteriaProperty, () => listBox.FilterCriteria = FilterCriteria);
#if !SL
			SetValueFromSettings(ItemTemplateSelectorProperty, () => listBox.ItemTemplateSelector = ItemTemplateSelector);
#endif
			if (ItemsPanel != null)
				SetValueFromSettings(ItemsPanelProperty, () => listBox.ItemsPanel = ItemsPanel);
			if (listBox.Settings != this)
				listBox.Items.Assign(Items);
		}
		IItemsProvider2 IItemsProviderOwner.ItemsProvider { get { return ItemsProvider; } }
		bool IItemsProviderOwner.IsLoaded { get; set; }
	}
}
#if !SL
namespace DevExpress.Xpf.Editors.Settings.Extension {
	public class ListBoxEditSettingsExtension : BaseSettingsExtension {
		public IEnumerable ItemsSource { get; set; }
		public ListBoxEditStyleSettings StyleSettings { get; set; }
		public string DisplayMember { get; set; }
		public string ValueMember { get; set; }
		public DataTemplate ItemTemplate { get; set; }
		public DataTemplateSelector ItemTemplateSelector { get; set; }
		public ItemsPanelTemplate ItemsPanel { get; set; }
		public SelectionMode SelectionMode { get; set; }
		public bool AllowCollectionView { get; set; }
		public ListBoxEditSettingsExtension() {
			DisplayMember = ValueMember = string.Empty;
			ItemTemplate = null;
			ItemTemplateSelector = null;
			ItemsPanel = (ItemsPanelTemplate)ListBoxEditSettings.ItemsPanelProperty.DefaultMetadata.DefaultValue;
			SelectionMode = (SelectionMode)ListBoxEditSettings.SelectionModeProperty.DefaultMetadata.DefaultValue;
			AllowCollectionView = false;
		}
		protected override BaseEditSettings CreateEditSettings() {
			return new ListBoxEditSettings()
			{
				ItemsSource = this.ItemsSource,
				StyleSettings = this.StyleSettings,
				SelectionMode = this.SelectionMode,
				DisplayMember = this.DisplayMember,
				ValueMember = this.ValueMember,
				ItemTemplate = this.ItemTemplate,
				ItemTemplateSelector = this.ItemTemplateSelector,
				ItemsPanel = this.ItemsPanel,
				AllowCollectionView = this.AllowCollectionView,
			};
		}
	}
	public class ListBoxEditEnumSettingsExtension : BaseSettingsExtension {
		public Type EnumType { get; set; }
		public ListBoxEditEnumSettingsExtension() { }
		protected override BaseEditSettings CreateEditSettings() {
			return new ListBoxEditSettings()
			{
				ItemsSource = EnumHelper.GetEnumSource(EnumType),
				ValueMember = EnumSourceHelperCore.ValueMemberName,
				DisplayMember = EnumSourceHelperCore.DisplayMemberName,
				SelectionMode = SelectionMode.Single
			};
		}
	}
}
#endif

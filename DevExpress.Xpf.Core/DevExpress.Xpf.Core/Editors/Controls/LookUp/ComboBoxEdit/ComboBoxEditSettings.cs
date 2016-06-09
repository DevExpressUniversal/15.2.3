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
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
#if !SL
using DevExpress.Utils.Design.DataAccess;
#else
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
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
	public class ComboBoxEditSettings : LookUpEditSettingsBase, IItemsProviderOwner {
		ListItemCollection items;
		static ComboBoxEditSettings() {
			Type ownerType = typeof(ComboBoxEditSettings);
#if !SL
			IncrementalFilteringProperty.OverrideMetadata(typeof(ComboBoxEditSettings), new FrameworkPropertyMetadata(null, OnSettingsPropertyChanged));
			PopupMaxHeightProperty.OverrideMetadata(typeof(ComboBoxEditSettings), new FrameworkPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0));
#endif
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ComboBoxEditSettingsItems"),
#endif
 Category(EditSettingsCategories.Data)]
		public ListItemCollection Items { get { return items ?? (items = new ListItemCollection(this)); } }
		ListItemCollection IItemsProviderOwner.Items { get { return Items; } }
#if !SL
		ObservableCollection<GroupStyle> groupStyle;
		public ObservableCollection<GroupStyle> GroupStyle { get { return groupStyle ?? (groupStyle = new ObservableCollection<GroupStyle>()); } }
#endif
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			ComboBoxEdit cb = edit as ComboBoxEdit;
			if (cb == null)
				return;
			if (cb.Settings != this)
				cb.Items.Assign(Items);
		}
		public bool GetApplyImageTemplateToSelectedItem() {
			return ((ComboBoxEdit)Editor).ApplyImageTemplateToSelectedItem;
		}
	}
}
namespace DevExpress.Xpf.Editors.Settings.Extension {
#if !SL
	public class ComboBoxSettingsExtension : PopupBaseSettingsExtension {
		public string SeparatorString { get; set; }
		public IEnumerable ItemsSource { get; set; }
		public string DisplayMember { get; set; }
		public string ValueMember { get; set; }
		public DataTemplate ItemTemplate { get; set; }
		public DataTemplateSelector ItemTemplateSelector { get; set; }
		public ItemsPanelTemplate ItemsPanel { get; set; }
		public bool ApplyItemTemplateToSelectedItem { get; set; }
		public bool AutoComplete { get; set; }
		public bool IsCaseSensitiveSearch { get; set; }
		public bool ImmediatePopup { get; set; }
		public bool AllowItemHighlighting { get; set; }
		public bool? IncrementalFiltering { get; set; }
		public bool AllowCollectionView { get; set; }
		public EditorPlacement? AddNewButtonPlacement { get; set; }
		public EditorPlacement? FindButtonPlacement { get; set; }
		public FindMode? FindMode { get; set; }
		public FilterCondition? FilterCondition { get; set; }
		public BaseComboBoxStyleSettings StyleSettings { get; set; }
		public ComboBoxSettingsExtension() {
			HorizontalContentAlignment = EditSettingsHorizontalAlignment.Left;
			DisplayMember = ValueMember = string.Empty;
			ItemTemplate = null;
			ItemTemplateSelector = null;
			ItemsPanel = ComboBoxEditSettings.ItemsPanelProperty.DefaultMetadata.DefaultValue as ItemsPanelTemplate;
			PopupMaxHeight = SystemParameters.PrimaryScreenHeight / 3.0;
			PopupMinHeight = 35d;
			ApplyItemTemplateToSelectedItem = false;
			AutoComplete = (bool)ComboBoxEdit.AutoCompleteProperty.DefaultMetadata.DefaultValue;
			IsCaseSensitiveSearch = (bool)ComboBoxEdit.IsCaseSensitiveSearchProperty.DefaultMetadata.DefaultValue;
			ImmediatePopup = (bool)ComboBoxEdit.ImmediatePopupProperty.DefaultMetadata.DefaultValue;
			AllowItemHighlighting = (bool)ComboBoxEdit.AllowItemHighlightingProperty.DefaultMetadata.DefaultValue;
			SeparatorString = (string)ComboBoxEdit.SeparatorStringProperty.DefaultMetadata.DefaultValue;
			AllowCollectionView = false;
			FilterCondition = null;
		}
		protected override PopupBaseEditSettings CreatePopupBaseEditSettings() {
			return new ComboBoxEditSettings() {
				ItemsSource = this.ItemsSource,
				DisplayMember = this.DisplayMember,
				ValueMember = this.ValueMember,
				IsTextEditable = this.IsTextEditable,
				ItemTemplate = this.ItemTemplate,
				ItemTemplateSelector = this.ItemTemplateSelector,
				ItemsPanel = this.ItemsPanel,
				ApplyItemTemplateToSelectedItem = this.ApplyItemTemplateToSelectedItem,
				StyleSettings = this.StyleSettings,
				AutoComplete = this.AutoComplete,
				IsCaseSensitiveSearch = this.IsCaseSensitiveSearch,
				ImmediatePopup = this.ImmediatePopup,
				AllowItemHighlighting = this.AllowItemHighlighting,
				IncrementalFiltering = this.IncrementalFiltering,
				SeparatorString = this.SeparatorString,
				FindMode = this.FindMode,
				FilterCondition = this.FilterCondition,
				AllowCollectionView = this.AllowCollectionView
			};
		}
		protected override void Assign(PopupBaseEditSettings settings) {
			base.Assign(settings);
			LookUpEditSettingsBase se = settings as LookUpEditSettingsBase;
			if (se == null)
				return;
			se.AddNewButtonPlacement = AddNewButtonPlacement;
			se.FindButtonPlacement = FindButtonPlacement;
		}
	}
	public class ComboBoxEnumSettingsExtension : PopupBaseSettingsExtension {
		public Type EnumType { get; set; }
		public bool UseUnderlyingEnumValue { get; set; }
		public ComboBoxEnumSettingsExtension() {
			HorizontalContentAlignment = EditSettingsHorizontalAlignment.Left;
			UseUnderlyingEnumValue = true;
		}
		protected override PopupBaseEditSettings CreatePopupBaseEditSettings() {
			return new ComboBoxEditSettings() {
				ItemsSource = EnumHelper.GetEnumSource(EnumType, UseUnderlyingEnumValue),
				ValueMember = EnumSourceHelperCore.ValueMemberName,
				DisplayMember = EnumSourceHelperCore.DisplayMemberName
			};
		}
		protected override bool? GetIsTextEditable() {
			return false;
		}
	}
#endif
}

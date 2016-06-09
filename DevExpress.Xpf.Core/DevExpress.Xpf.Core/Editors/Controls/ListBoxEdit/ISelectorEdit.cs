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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Helpers;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Popups;
#if SL
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
namespace DevExpress.Xpf.Editors.Internal {
	public interface ISelectorEdit : IBaseEdit {
#if !SL
		ObservableCollection<GroupStyle> GroupStyle { get; }
#endif
		EditStrategyBase EditStrategy { get; }
		ICommand SelectAllItemsCommand { get; }
		SelectionMode SelectionMode { get; }
		IItemsProvider2 ItemsProvider { get; }
		string DisplayMember { get; set; }
		string ValueMember { get; set; }
		DataTemplate ItemTemplate { get; }
		ListItemCollection Items { get; }
		ObservableCollection<object> SelectedItems { get; }
		object SelectedItem { get; set; }
		int SelectedIndex { get; set; }
		object ItemsSource { get; set; }
		object GetPopupContentItemsSource();
		IEnumerable GetPopupContentCustomItemsSource();
		IEnumerable GetPopupContentMRUItemsSource();
		IListNotificationOwner ListNotificationOwner { get; }
		ISelectionProvider SelectionProvider { get; }
		bool AllowCollectionView { get; }
		bool UseCustomItems { get; }
		CriteriaOperator FilterCriteria { get; set; }
		bool IsSynchronizedWithCurrentItem { get; }
		object GetCurrentSelectedItem();
		IEnumerable GetCurrentSelectedItems();
		SelectionEventMode SelectionEventMode { get; }
		bool AllowRejectUnknownValues { get; }
	}
	public interface IEditStrategy {
		bool IsInSupportInitialize { get; }
	}
	public interface ISelectorEditStrategy : IEditStrategy {
		ISelectorEdit Editor { get; }
		RoutedEvent SelectedIndexChangedEvent { get; }
		IItemsProvider2 ItemsProvider { get; }
		bool IsSingleSelection { get; }
		bool IsTokenMode { get; }
		object CurrentDataViewHandle { get; }
		object TokenDataViewHandle { get; }
		int EditableTokenIndex { get; }
		bool IsInProcessNewValue { get; }
		bool IsInLookUpMode { get; }
		void SyncWithValue(DependencyProperty dp, object oldValue, object newValue);
		bool ProvideEditValue(object value, out object provideValue, UpdateEditorSource updateSource);
		object GetSelectedItems(object editValue);
		object EditValue { get; set; }
		string SearchText { get; }
		TextSearchEngine TextSearchEngine { get; }
		void BringToView();
		object GetInnerEditorItemsSource();
		IEnumerable GetInnerEditorCustomItemsSource();
		IEnumerable GetInnerEditorMRUItemsSource();
		object GetCurrentEditableValue();
		object GetPrevValueFromSearchText(int startIndex);
		object GetNextValueFromSearchText(int startIndex);
	}
	public interface ISelectionProvider {
		void SelectAll();
		void UnselectAll();
		void SetSelectAll(bool? value);
	}
	public interface ISelectorEditInnerListBox {
#if !SL
		ObservableCollection<GroupStyle> GroupStyle { get; }
		DataTemplateSelector ItemTemplateSelector { get; set; }
#endif
		ItemsPanelTemplate ItemsPanel { get; }
		bool IsCustomItem(ICustomItem item);
		ICustomItem GetCustomItem(Func<object, bool> getNeedItem);
		ISelectorEdit OwnerEdit { get; }
		IEnumerable ItemsSource { get; set; }
		IList SelectedItems { get; }
		object SelectedItem { get; set; }
		int SelectedIndex { get; set; }
		void ScrollIntoView(object item);
		bool? IsSelectAll { get; }
		void SelectAll();
		void UnselectAll();
		ItemContainerGenerator ItemContainerGenerator { get; }
#if DEBUGTEST
		int ItemsSourceCount { get; }
		IEnumerable CustomItemsSource { get; }
		IEnumerable ContentItemsSource { get; }
#endif
	}
	public interface ISelectorEditStyleSettings {
		Style GetItemContainerStyle(ISelectorEdit editor);
	}
}

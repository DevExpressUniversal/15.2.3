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

using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors;
using System.Collections;
using DevExpress.Mvvm;
using DevExpress.Utils.Localization;
namespace DevExpress.Xpf.Navigation.NavigationBar.Customization {
	public partial class CustomizationControl : UserControl {
		#region Dependency props
		public bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { SetValue(IsCompactProperty, value); }
		}
		public static readonly DependencyProperty SupportCompactModeProperty =
			DependencyProperty.Register("SupportCompactMode", typeof(bool), typeof(CustomizationControl), new PropertyMetadata(false));
		public bool SupportCompactMode {
			get { return (bool)GetValue(SupportCompactModeProperty); }
			set { SetValue(SupportCompactModeProperty, value); }
		}
		public static readonly DependencyProperty IsCompactProperty =
			DependencyProperty.Register("IsCompact", typeof(bool), typeof(CustomizationControl), new PropertyMetadata(false));
		public int VisibleItemsCount {
			get { return (int)GetValue(VisibleItemsCountProperty); }
			set { SetValue(VisibleItemsCountProperty, value); }
		}
		public static readonly DependencyProperty VisibleItemsCountProperty = DependencyProperty.Register("VisibleItemsCount", typeof(int), typeof(CustomizationControl), new UIPropertyMetadata(0));
		public int ItemsCount {
			get { return (int)GetValue(ItemsCountProperty); }
			set { SetValue(ItemsCountProperty, value); }
		}
		public static readonly DependencyProperty ItemsCountProperty = DependencyProperty.Register("ItemsCount", typeof(int), typeof(CustomizationControl), new UIPropertyMetadata(0));
		public int SelectedIndex {
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof(int), typeof(CustomizationControl), new PropertyMetadata(0, OnSelectedIndexChanged));
		private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CustomizationControl)d).OnSelectedIndexChanged((int)e.OldValue, (int)e.NewValue);
		}
		public bool AllowMoveUp {
			get { return (bool)GetValue(AllowMoveUpProperty); }
			set { SetValue(AllowMoveUpProperty, value); }
		}
		public static readonly DependencyProperty AllowMoveUpProperty =
			DependencyProperty.Register("AllowMoveUp", typeof(bool), typeof(CustomizationControl), new PropertyMetadata(false));
		public bool AllowMoveDown {
			get { return (bool)GetValue(AllowMoveDownProperty); }
			set { SetValue(AllowMoveDownProperty, value); }
		}
		public static readonly DependencyProperty AllowMoveDownProperty =
			DependencyProperty.Register("AllowMoveDown", typeof(bool), typeof(CustomizationControl), new PropertyMetadata(false));
		#endregion
		OfficeNavigationBar Owner;
		ObservableCollectionCore<object> itemsSource = new ObservableCollectionCore<object>();
		public ObservableCollection<object> ItemsSource {
			get { return itemsSource; }
		}
		public CustomizationControl(OfficeNavigationBar owner) {
			InitializeComponent();
			DataContext = this;
			Owner = owner;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			PopulateItemsSource();
		}
		public void OnSelectedIndexChanged(int oldValue, int newValue) {
			AllowMoveUp = (newValue - 1) >= 0;
			AllowMoveDown = newValue + 1 < ItemsSource.Count;
		}
		void PopulateItemsSource() {
			PopulateItemsSource(Owner.Items);
		}
		void PopulateItemsSource(IList items) {
			itemsSource.Clear();
			itemsSource.BeginUpdate();
			foreach(var item in items) {
				var container = Owner.ItemContainerGenerator.ContainerFromItem(item) as NavigationBarItem;
				if(container != null) {
					object content = container.CustomizationCaption ?? (container.Content is UIElement ? container.Content.ToString() : container.Content);
					DataTemplate template = container.CustomizationCaptionTemplate ?? container.ContentTemplate;
					DataTemplateSelector templateSelector = container.CustomizationCaptionTemplateSelector ?? container.ContentTemplateSelector;
					if(content == null && template == null && templateSelector == null) content = container.GetType().Name;
					CustomizableItem customizableItem = new CustomizableItem() { Content = content, ContentTemplate = template, ContentTemplateSelector = templateSelector };
					itemsSource.Add(customizableItem);
				}
			}
			itemsSource.EndUpdate();
			int prevSelected = SelectedIndex;
			SelectedIndex = itemsSource.Count > 0 ? 0 : -1;
			OnSelectedIndexChanged(prevSelected, SelectedIndex);
			ItemsCount = itemsSource.Count;
			int ownerMaxItemCount = Owner.MaxItemCount;
			VisibleItemsCount = ownerMaxItemCount >= 0 && ownerMaxItemCount < ItemsCount ? ownerMaxItemCount : ItemsCount;
			IsCompact = Owner.IsCompact;
			SupportCompactMode = Owner.NavigationClient != null && Owner.NavigationClient.AcceptsCompactNavigation;
		}
		private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
			Owner.CustomizationHelper.CancelCustomization();
			Owner.CustomizationHelper.CloseCustomizationForm();
		}
		private void OnOKButtonClick(object sender, RoutedEventArgs e) {
			Owner.CustomizationHelper.ApplyCustomization();
			Owner.CustomizationHelper.CloseCustomizationForm();
		}
		private void OnResetButtonClick(object sender, RoutedEventArgs e) {
			Owner.CustomizationHelper.EnqueueCustomizationAction(new ResetCustomizationAction());
			var state = Owner.CustomizationHelper.GetCustomizationState();
			if(state != null) {
				PopulateItemsSource(state.OriginalItems);
				ItemsCount = itemsSource.Count;
				int ownerMaxItemCount = state.OriginalMaxItemsCount;
				VisibleItemsCount = ownerMaxItemCount >= 0 && ownerMaxItemCount < ItemsCount ? ownerMaxItemCount : ItemsCount;
			}
		}
		private void OnUpButtonClick(object sender, RoutedEventArgs e) {
			int fromIndex = listBox.SelectedIndex;
			int toIndex = fromIndex - 1;
			if(toIndex < 0) return;
			Owner.CustomizationHelper.EnqueueCustomizationAction(new MoveCustomizationAction() { FromIndex = fromIndex, ToIndex = toIndex });
			object item = listBox.SelectedItem;
			itemsSource.RemoveAt(fromIndex);
			itemsSource.Insert(toIndex, item);
			Dispatcher.BeginInvoke(new Action(() => {
				SelectedIndex = toIndex;
				listBox.ScrollIntoView(listBox.SelectedItem);
			}));
		}
		private void OnDownButtonClick(object sender, RoutedEventArgs e) {
			int fromIndex = listBox.SelectedIndex;
			int toIndex = fromIndex + 1;
			if(toIndex >= itemsSource.Count) return;
			Owner.CustomizationHelper.EnqueueCustomizationAction(new MoveCustomizationAction() { FromIndex = fromIndex, ToIndex = toIndex });
			object item = listBox.SelectedItem;
			itemsSource.RemoveAt(fromIndex);
			itemsSource.Insert(toIndex, item);
			Dispatcher.BeginInvoke(new Action(() => {
				SelectedIndex = toIndex;
				listBox.ScrollIntoView(listBox.SelectedItem);
			}));
		}
		public class CustomizableItem : BindableBase {
			public object Content {
				get { return GetProperty(() => Content); }
				set { SetProperty(() => Content, value); }
			}
			public DataTemplate ContentTemplate {
				get { return GetProperty(() => ContentTemplate); }
				set { SetProperty(() => ContentTemplate, value); }
			}
			public DataTemplateSelector ContentTemplateSelector {
				get { return GetProperty(() => ContentTemplateSelector); }
				set { SetProperty(() => ContentTemplateSelector, value); }
			}
		}
		private void SpinEdit_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			Owner.CustomizationHelper.EnqueueCustomizationAction(new PropertyCustomizationAction() { Property = OfficeNavigationBar.MaxItemCountProperty, Value = Convert.ToInt32(e.NewValue) });
		}
		private void CheckEdit_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			Owner.CustomizationHelper.EnqueueCustomizationAction(new PropertyCustomizationAction() { Property = OfficeNavigationBar.IsCompactProperty, Value = e.NewValue });
		}
	}
	public class NavigationStringIdConverter : StringIdConverter<NavigationStringId> {
		protected override XtraLocalizer<NavigationStringId> Localizer { get { return NavigationLocalizer.Active; } }
	}
}

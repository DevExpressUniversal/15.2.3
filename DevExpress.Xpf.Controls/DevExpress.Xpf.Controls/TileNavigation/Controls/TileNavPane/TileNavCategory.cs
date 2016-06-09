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

using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
namespace DevExpress.Xpf.Navigation {
	[ContentProperty("Items")]
	public class TileNavCategory : NavElementBase {
		#region static
		public static readonly DependencyProperty ItemsProperty;
		static readonly DependencyPropertyKey ItemsPropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemsAttachedBehaviorProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		static readonly DependencyPropertyKey HasItemsPropertyKey;
		public static readonly DependencyProperty HasItemsProperty;
		static TileNavCategory() {
			var dProp = new DependencyPropertyRegistrator<TileNavCategory>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			Type ownerType = typeof(TileNavCategory);
			ItemsPropertyKey = DependencyProperty.RegisterReadOnly("Items", typeof(TileNavItemCollection), ownerType, new PropertyMetadata());
			ItemsProperty = ItemsPropertyKey.DependencyProperty;
			ItemsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("ItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<TileNavCategory, TileNavItem>), ownerType, new UIPropertyMetadata(null));
			ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemsSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			HasItemsPropertyKey = DependencyProperty.RegisterReadOnly("HasItems", typeof(bool), ownerType, new PropertyMetadata(false));
			HasItemsProperty = HasItemsPropertyKey.DependencyProperty;
		}
		static void OnItemsSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((TileNavCategory)d).OnItemsSourceChanged(e);
		}
		static void OnItemTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((TileNavCategory)d).OnItemTemplateChanged(e);
		}
		#endregion
		public TileNavCategory() {
			Items = new TileNavItemCollection(this);
			Items.CollectionChanged += OnItemsCollectionChanged;
		}
		void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			HasItems = Items.Count > 0;
		}
		private void OnItemsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<TileNavCategory, TileNavItem>.OnItemsSourcePropertyChanged(this,
					e,
					ItemsAttachedBehaviorProperty,
					ItemTemplateProperty,
					ItemTemplateSelectorProperty,
					ItemStyleProperty,
					control => control.Items,
					control => control.CreateItem(),
					null,
					(item) => InitItem(item));
			}
		}
		private void OnItemTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<TileNavCategory, TileNavItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty);
		}
		ItemsControl _PartItemsControl;
		internal ItemsControl PartItemsControl {
			get {
				if(_PartItemsControl == null) {
					_PartItemsControl = new TileNavPaneBar();
					_PartItemsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("Items") { Source = this });
					_PartItemsControl.SetBinding(ItemsControl.ItemContainerStyleProperty, new Binding("ItemStyle") { Source = this });
				}
				return _PartItemsControl;
			}
		}
		protected virtual void InitItem(FrameworkElement item) {
			item.SetBinding(TileNavItem.ContentProperty, new Binding("DataContext") { Source = item });
		}
		protected virtual TileNavItem CreateItem() {
			return new TileNavItem();
		}
		protected override UIElement GetFlyoutContainer() {
			return PartItemsControl;
		}
		protected override void BeforeFlyoutOpened() {
			base.BeforeFlyoutOpened();
			PartItemsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("Items") { Source = this });
			PartItemsControl.SetBinding(ItemsControl.ItemContainerStyleProperty, new Binding("ItemStyle") { Source = this });
		}
		protected override bool GetHasItemsCore() {
			return true;
		}
		public object ItemsSource {
			get { return GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 0, XtraSerializationFlags.None)]
		public TileNavItemCollection Items {
			get { return (TileNavItemCollection)GetValue(ItemsProperty); }
			internal set { SetValue(ItemsPropertyKey, value); }
		}
		public bool HasItems {
			get { return (bool)GetValue(HasItemsProperty); }
			internal set { SetValue(HasItemsPropertyKey, value); }
		}
		protected override IEnumerator LogicalChildren {
			get { return Items.ToList().GetEnumerator(); }
		}
	}
	public class TileNavCategoryCollection : ObservableCollection<TileNavCategory> {
		TileNavPane Owner;
		internal TileNavCategoryCollection(TileNavPane owner) {
			Owner = owner;
		}
		protected override void InsertItem(int index, TileNavCategory item) {
			base.InsertItem(index, item);
			item.Click += item_Click;
			((INavElement)item).TileNavPane = Owner;
			((ILogicalOwner)Owner).AddChild(item);
		}
		protected override void RemoveItem(int index) {
			var item = this[index];
			if(item != null) {
				item.Click -= item_Click;
				((INavElement)item).TileNavPane = null;
				((ILogicalOwner)Owner).RemoveChild(item);
			}
			base.RemoveItem(index);
		}
		void item_Click(object sender, EventArgs e) {
		}
	}
}

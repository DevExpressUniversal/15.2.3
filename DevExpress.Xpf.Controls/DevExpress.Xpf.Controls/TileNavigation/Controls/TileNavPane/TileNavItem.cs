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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.WindowsUI.Internal;
using System.Windows.Data;
using DevExpress.Xpf.Navigation.Internal;
namespace DevExpress.Xpf.Navigation {
	[ContentProperty("Items")]
	public class TileNavItem : NavElementBase {
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
		static TileNavItem() {
			var dProp = new DependencyPropertyRegistrator<TileNavItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			Type ownerType = typeof(TileNavItem);
			ItemsPropertyKey = DependencyProperty.RegisterReadOnly("Items", typeof(TileNavSubItemCollection), ownerType, new PropertyMetadata());
			ItemsProperty = ItemsPropertyKey.DependencyProperty;
			ItemsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("ItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<TileNavItem, TileNavSubItem>), ownerType, new UIPropertyMetadata(null));
			ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemsSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			HasItemsPropertyKey = DependencyProperty.RegisterReadOnly("HasItems", typeof(bool), ownerType, new PropertyMetadata(false));
			HasItemsProperty = HasItemsPropertyKey.DependencyProperty;
		}
		static void OnItemsSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((TileNavItem)d).OnItemsSourceChanged(e);
		}
		static void OnItemTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((TileNavItem)d).OnItemTemplateChanged(e);
		}
		#endregion
		public TileNavItem() {
			Items = new TileNavSubItemCollection(this);
			Items.CollectionChanged += OnItemsCollectionChanged;
		}
		void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			HasItems = Items.Count > 0;
			CoerceValue(IsFlyoutButtonVisibleProperty);
		}
		protected override object OnCoerceIsFlyoutButtonVisible(bool value) {
			return ShowFlyoutButton && HasItems;
		}
		private void OnItemsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<TileNavItem, TileNavSubItem>.OnItemsSourcePropertyChanged(this,
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
			ItemsAttachedBehaviorCore<TileNavItem, TileNavSubItem>.OnItemsGeneratorTemplatePropertyChanged(this,
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
					if(Owner != null) {
						TileBar navBar = Owner as TileBar;
						if(navBar != null)
							((TileBar)PartItemsControl).SetBinding(TileBar.ShowItemShadowProperty, new Binding("ShowItemShadow") { Source = this.Owner });
					}
				}
				return _PartItemsControl;
			}
		}
		protected virtual void InitItem(FrameworkElement item) {
			item.SetBinding(TileNavSubItem.ContentProperty, new Binding("DataContext") { Source = item });
		}
		protected virtual TileNavSubItem CreateItem() {
			return new TileNavSubItem();
		}
		ContentControl _PartFlyoutContent;
		ContentControl PartFlyoutContent {
			get {
				if(_PartFlyoutContent == null) _PartFlyoutContent = new ContentControl();
				return _PartFlyoutContent;
			}
		}
		protected override UIElement GetFlyoutContainer() {
			return PartFlyoutContent;
		}
		protected override void BeforeFlyoutOpened() {
			base.BeforeFlyoutOpened();
			TileNavPane.SetFlyoutSourceType(PartItemsControl, FlyoutSourceType.FromTileBar);
			PartFlyoutContent.Content = PartItemsControl;
		}
		protected override void OnFlyoutClosed() {
			base.OnFlyoutClosed();
			PartFlyoutContent.ClearValue(ContentControl.ContentProperty);
		}
		protected override bool GetHasItemsCore() {
			return true;
		}
		protected override double GetFlyoutOffset() {
			return 7d;
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
		public TileNavSubItemCollection Items {
			get { return (TileNavSubItemCollection)GetValue(ItemsProperty); }
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
	public class TileNavItemCollection : ObservableCollection<TileNavItem> {
		INavElement Owner;
		internal TileNavItemCollection(INavElement owner) {
			Owner = owner;
		}
		protected override void InsertItem(int index, TileNavItem item) {
			base.InsertItem(index, item);
			item.AllowSelection = Owner.AllowSelection;
			if(Owner is NavButton) return;
			INavElement navElement = item;
			navElement.NavParent = Owner;
			Owner.AddChild(item);
		}
		protected override void RemoveItem(int index) {
			INavElement item = this[index];
			if(item != null && item.NavParent == Owner) {
				item.NavParent = null;
				Owner.RemoveChild(item);
			}
			base.RemoveItem(index);
		}
	}
}

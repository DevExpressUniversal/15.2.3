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
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public enum ToolbarListItemType { ShowBars, ShowBarsAndItems }
	public class ToolbarListItem : BarListItem {
		#region static 
		public static readonly DependencyProperty ListItemTypeProperty;
		public static readonly DependencyProperty SelectedToolbarProperty;
		static ToolbarListItem() {
			ListItemTypeProperty = DependencyPropertyManager.Register("ListItemType", typeof(ToolbarListItemType), typeof(ToolbarListItem), new FrameworkPropertyMetadata(ToolbarListItemType.ShowBars, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnListItemTypePropertyChanged)));
			SelectedToolbarProperty = DependencyPropertyManager.Register("SelectedToolbar", typeof(Bar), typeof(ToolbarListItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSelectedToolbarPropertyChanged)));
			BarManager.BarManagerProperty.OverrideMetadata(typeof(ToolbarListItem), new FrameworkPropertyMetadata((d, e) => ((ToolbarListItem)d).OnBarManagerChanged((BarManager)e.OldValue, (BarManager)e.NewValue)));			
		}		
		protected static void OnSelectedToolbarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ToolbarListItem)d).OnSelectedToolbarChanged(e);
		}
		protected static void OnListItemTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ToolbarListItem)d).OnListItemTypeChanged(e);
		}
		#endregion         
		protected virtual void OnBarManagerChanged(BarManager oldValue, BarManager newValue) {
			if (oldValue != null)
				UnSubscribeManagerEvents(oldValue);
			if (newValue != null)
				SubscribeManagerEvents(newValue);
			UpdateItems();
		}		
		protected virtual void UnSubscribeManagerEvents(BarManager manager) {
			manager.Bars.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnBarsCollectionChanged);
		}
		protected virtual void SubscribeManagerEvents(BarManager manager) { 
			manager.Bars.CollectionChanged += new NotifyCollectionChangedEventHandler(OnBarsCollectionChanged);
		}
		protected virtual void OnBarsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateItems();
		}
		protected override void UpdateItemLinksCore() {
			base.UpdateItemLinksCore();
			if (ItemLinks.Count <= 1 || !AllowCustomization) return;
			ItemLinks.Insert(ItemLinks.Count - 1, new BarItemLinkSeparator() { IsPrivate = true } );
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ToolbarListItemListItemType")]
#endif
		public ToolbarListItemType ListItemType {
			get { return (ToolbarListItemType)GetValue(ListItemTypeProperty); }
			set { SetValue(ListItemTypeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ToolbarListItemSelectedToolbar")]
#endif
		public Bar SelectedToolbar {
			get { return (Bar)GetValue(SelectedToolbarProperty); }
			set { SetValue(SelectedToolbarProperty, value); }
		}
		protected virtual ToolbarCheckItem CreateCheckListItem(Bar bar) {
			if(!bar.IsAllowHide) return null;
			ToolbarCheckItem item = new ToolbarCheckItem(bar);
			return item;
		}
		bool allowUpdateItem = true;
		internal bool AllowUpdateItem {
			get { return allowUpdateItem; }
			set {
				allowUpdateItem = value;
				UpdateItems();
			}
		}
		protected virtual BarSubItem CreateSubMenuListItem(Bar bar) {
			BarSubItem item = new BarSubItem();
			LinkListItem linkListItem = new LinkListItem();
			item.IsPrivate = true;
			CreateBindings(item, bar);
			linkListItem.IsPrivate = true;
			LinkListItemLink link = (LinkListItemLink)linkListItem.CreateLink(true);
			linkListItem.Source = bar.ItemLinks;
			item.ItemLinks.Add(link);
			return item;
		}
		protected virtual void CreateBindings(BarSubItem item, Bar bar) {
			Binding contentBinding = new Binding("Caption");
			contentBinding.Source = bar;
			BindingOperations.SetBinding(item, BarItem.ContentProperty, contentBinding);
		}
		protected virtual BarItem CreateListItem(Bar bar) {
			return ListItemType == ToolbarListItemType.ShowBars ? (BarItem)CreateCheckListItem(bar) : CreateSubMenuListItem(bar);
		}
		bool AllowCustomization { get { return (BarManager.GetBarManager(this) ?? SelectedToolbar.With(BarManager.GetBarManager)).Return(x => x.AllowCustomization, () => true); } }		
		protected internal override void OnLinkControlLoaded(object sender, BarItemLinkControlLoadedEventArgs e) {
			base.OnLinkControlLoaded(sender, e);			
		}
		protected override void OnScopeChanged(ScopeChangedEventArgs e) {
			base.OnScopeChanged(e);
			if (e.OldScope != null)
				e.OldScope[typeof(IFrameworkInputElement)].Changed -= RegistratorChanged;
			if (e.NewScope != null)
				e.NewScope[typeof(IFrameworkInputElement)].Changed += RegistratorChanged;
			UpdateItems();
		}
		void RegistratorChanged(ElementRegistrator sender, ElementRegistratorChangedArgs e) {
			if (e.Element is Bar)
				UpdateItems();
		}				
		protected override void UpdateItems() {			
			InternalItems.BeginUpdate();
			try {
				InternalItems.Clear();
				var bars = (SelectedToolbar ?? (DependencyObject)this).With(BarNameScope.GetService<IElementRegistratorService>).Return(x => x.GetElements<IFrameworkInputElement>().OfType<Bar>(), Enumerable.Empty<Bar>);
				foreach (Bar bar in bars) {
					if(bar.IsPrivate || bar.IsRemoved)
						continue;
					BarItem item = CreateListItem(bar);
					if(item != null)
						InternalItems.Add(item);
				}
				if (AllowCustomization)
					InternalItems.Add(CreateCustomizationItem());
			}
			finally { InternalItems.EndUpdate(); } 
		}
		protected override void OnLinksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {			
			base.OnLinksCollectionChanged(sender, e);
			UpdateItems();
		}				
		static int CustomizationItemIndex = 0;
		protected virtual BarButtonItem CreateCustomizationItem() {
			BarButtonItem customizeItem = new BarButtonItem();
			customizeItem.ItemClick += new ItemClickEventHandler(OnCustomizeItemClick);
			customizeItem.Name = "ToolbarCustomizeItem" + CustomizationItemIndex;
			CustomizationItemIndex++;
			customizeItem.Content = BarsLocalizer.GetString(BarsStringId.ToolbarListItem_CustomizationItemCaption);
			customizeItem.IsPrivate = true;
			return customizeItem;
		}
		protected virtual void OnCustomizeItemClick(object sender, ItemClickEventArgs e) {
			BarNameScope.GetService<ICustomizationService>((DependencyObject)SelectedToolbar ?? this).ShowCustomizationForm();
		}
		protected virtual void OnListItemTypeChanged(DependencyPropertyChangedEventArgs e) {
			UpdateItems();
		}
		protected virtual void OnSelectedToolbarChanged(DependencyPropertyChangedEventArgs e) {
			UpdateItems();
		}
	}
}

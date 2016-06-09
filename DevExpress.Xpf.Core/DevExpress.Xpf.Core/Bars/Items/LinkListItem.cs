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
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public class LinkListItem : BarListItem {
		#region static
		public static readonly DependencyProperty SourceProperty;
		static LinkListItem() { 
			SourceProperty = DependencyPropertyManager.Register("Source", typeof(BarItemLinkCollection), typeof(LinkListItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSourcePropertyChanged)));
		}
		protected static void OnSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((LinkListItem)obj).OnSourceChanged(e);
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("LinkListItemSource")]
#endif
		public BarItemLinkCollection Source {
			get { return (BarItemLinkCollection)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
			}
		protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs e) {
			UnSubscribeSourceEvents((BarItemLinkCollection)e.OldValue);
			SubscribeSourceEvents((BarItemLinkCollection)e.NewValue);
			UpdateItems();
		}
		protected virtual void SubscribeSourceEvents(BarItemLinkCollection coll) {
			if(coll != null)
				coll.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnSourceChanged);
		}
		protected virtual void UnSubscribeSourceEvents(BarItemLinkCollection coll) {
			if(coll != null)
				coll.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnSourceChanged);
		}
		protected virtual void OnSourceChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			UpdateItems();
		}
		protected override void UpdateItemLinksCore() {
			base.UpdateItemLinksCore();
			if(ItemLinks.Count <= 1 || !BarManager.GetBarManager(this).Return(x=>x.AllowCustomization, ()=>false)) return;
			ItemLinks.Insert(ItemLinks.Count - 1, new BarItemLinkSeparator() { IsPrivate = true});
		}
		static int CustomizationItemIndex = 0;
		protected virtual BarButtonItem CreateCustomizationItem() {
			BarButtonItem customizeItem = new BarButtonItem();
			customizeItem.ItemClick += new ItemClickEventHandler(OnCustomizeItemClick);
			customizeItem.Name = "LinkListCustomizationItem" + CustomizationItemIndex;
			CustomizationItemIndex++;
			customizeItem.Content = BarsLocalizer.GetString(BarsStringId.LinkListItem_CustomizationItemCaption);
			customizeItem.IsPrivate = true;
			return customizeItem;
		}
		protected override void UpdateItems() {
			InternalItems.BeginUpdate();
			try {
				InternalItems.Clear();
				if(Source == null)
					return;
				foreach(BarItemLinkBase linkBase in Source) {
					BarItemLink link = linkBase as BarItemLink;
					if(link == null || link is BarItemLinkSeparator || link.IsRemoved) continue;
					InternalItems.Add(new LinkListCheckItem(linkBase));
				}
				if (BarManager.GetBarManager(this).Return(x => x.AllowCustomization, () => false))
				InternalItems.Add(CreateCustomizationItem());
			}
			finally { InternalItems.EndUpdate(); }
		}
		protected virtual void OnCustomizeItemClick(object sender, ItemClickEventArgs e) {
			BarNameScope.GetService<ICustomizationService>(this).ShowCustomizationForm();
		}
	}
}

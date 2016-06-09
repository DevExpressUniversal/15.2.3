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
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public abstract class BarListItem : BarLinkContainerItem {
		#region static
		public static readonly RoutedEvent InListItemClickEvent;
		static BarListItem() {
			InListItemClickEvent = EventManager.RegisterRoutedEvent("InListItemClick", RoutingStrategy.Direct, typeof(InListItemClickEventHandler), typeof(BarListItem));
			EventManager.RegisterClassHandler(typeof(BarListItem), BarNameScope.ScopeChangedEvent, new ScopeChangedEventHandler((s, e) => ((BarListItem)s).OnScopeChanged(e)));
		}
		#endregion
		BarItemCollection internalItems;
		public BarListItem() { }
		protected BarItemCollection InternalItems {
			get {
				if(internalItems == null)
					internalItems = CreateItems();
				return internalItems;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarListItemItemLinks")]
#endif
		public override BarItemLinkCollection ItemLinks {
			get { return base.ItemLinks; }
		}
		protected virtual BarItemCollection CreateItems() {
			BarItemCollection res = new BarItemCollection();
			res.CollectionChanged += new NotifyCollectionChangedEventHandler(OnItemsCollectionChanged);
			res.OnEndUpdate += new EventHandler(OnItemsCollectionEndUpdate);
			return res;
		}
		void OnItemsCollectionEndUpdate(object sender, EventArgs e) {
			UpdateItemLinks();
		}
		protected virtual void OnScopeChanged(ScopeChangedEventArgs e) {
			var oldHelper = e.OldScope.With(x => x.GetService<ICustomizationService>()).With(x => x.CustomizationHelper);
			var newHelper = e.NewScope.With(x => x.GetService<ICustomizationService>()).With(x => x.CustomizationHelper);
			if (oldHelper != null)
				oldHelper.IsCustomizationModeChanged -= OnCustomizationModeChanged;
			if (newHelper != null)
				newHelper.IsCustomizationModeChanged += OnCustomizationModeChanged;
		}
		protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(e.NewItems != null) {
				foreach(BarItem item in e.NewItems) {
					SubscribeItemEvents(item);
				}
			}
			if(e.OldItems != null) {
				foreach(BarItem item in e.OldItems) {
					UnSubscribeItemEvents(item);
				}
			}
			if(!InternalItems.IsUpdating)
				UpdateItemLinks();
		}
		public event InListItemClickEventHandler InListItemClick {
			add { this.AddHandler(InListItemClickEvent, value); }
			remove { this.RemoveHandler(InListItemClickEvent, value); }
		}
		protected virtual void SubscribeItemEvents(BarItem item) {
			item.ItemClick += new ItemClickEventHandler(OnInListItemClick);
		}
		protected virtual void UnSubscribeItemEvents(BarItem item) {
			item.ItemClick -= new ItemClickEventHandler(OnInListItemClick);
		}
		protected virtual void OnInListItemClick(object sender, ItemClickEventArgs e) {
			RaiseInListItemClick(e.Item);
		}
		protected internal virtual void RaiseInListItemClick(BarItem inListItem) {
			this.RaiseEvent(new InListItemClickEventArgs(this, inListItem) { RoutedEvent = InListItemClickEvent });
		}
		protected virtual void UpdateItemLinksCore() {
			foreach(BarItem item in InternalItems) {
				Items.Add(item);
			}	
		}
		protected void UpdateItemLinks() {
			ItemLinks.Clear();			
			if(BarManagerCustomizationHelper.IsInCustomizationMode(this)) {
				ItemLinks.Add(CreateCustomizationStaticItem().CreateLink(true));
				ItemLinks[ItemLinks.Count - 1].IsPrivateLinkInCustomizationMode = true;
				ItemLinks[ItemLinks.Count - 1].AllowShowCustomizationMenu = false;
				return;
			}
			UpdateItemLinksCore();
		}
		protected virtual BarItem CreateCustomizationStaticItem() {
			BarStaticItem item = new BarStaticItem();
			item.IsPrivate = true;
			item.DataContext = this;
			item.Content = Name + " (" + GetType().Name + ")";
			return item;
		}
		protected override void UpdateCore() {
			UpdateItems();
		}		
		protected abstract void UpdateItems();
		protected virtual void OnCustomizationModeChanged(object sender, EventArgs e) {
			UpdateItemLinks();
		}		
		protected override void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			base.OnIsEnabledChanged(sender, e);
			foreach(BarItem item in InternalItems) {
				item.IsEnabled = IsEnabled;
			}
		}
	}
	public delegate void InListItemClickEventHandler(object sender, InListItemClickEventArgs e);
	public class InListItemClickEventArgs : RoutedEventArgs {
		public BarListItem ListItem { get; set; }
		public BarItem InListItem { get; set; }
		public InListItemClickEventArgs(BarListItem listItem, BarItem inListItem) {
			ListItem = listItem;
			InListItem = inListItem;
		}
	}
}

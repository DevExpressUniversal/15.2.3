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
using System.Collections;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using System.Windows;
using DevExpress.Xpf.Core.Serialization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Ribbon {
	[ContentProperty("Items")]
	public class BarButtonGroup : BarItem, ILinksHolder, ILogicalChildrenContainer {
		#region static
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemsAttachedBehaviorProperty =
			DependencyPropertyManager.RegisterAttached("ItemsAttachedBehaviorProperty", typeof(ItemsAttachedBehaviorCore<BarButtonGroup, BarItem>), typeof(BarButtonGroup), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty ItemTemplateSelectorProperty =
			DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(BarButtonGroup), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplateSelectorPropertyChanged)));
		public static readonly DependencyProperty ItemStyleProperty =
			DependencyProperty.Register("ItemStyle", typeof(Style), typeof(BarButtonGroup), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemStylePropertyChanged)));
		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(BarButtonGroup), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(BarButtonGroup), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemsSourcePropertyChanged)));		 
		static BarButtonGroup() {
			BarItemLinkCreator.Default.RegisterObject(typeof(BarButtonGroup), typeof(BarButtonGroupLink), delegate(object arg) { return new BarButtonGroupLink(); } );
			BarItemLinkControlCreator.Default.RegisterObject(typeof(BarButtonGroupLink), typeof(BarButtonGroupLinkControl), delegate(object arg) { return new BarButtonGroupLinkControl(); });
			EventManager.RegisterClassHandler(typeof(BarButtonGroup), DXSerializer.CreateCollectionItemEvent, new XtraCreateCollectionItemEventHandler(OnCreateCollectionItemEvent));
			EventManager.RegisterClassHandler(typeof(BarButtonGroup), DXSerializer.ClearCollectionEvent, new XtraItemRoutedEventHandler(OnClearCollectionEvent));
		}
		protected static void OnClearCollectionEvent(object sender, XtraItemRoutedEventArgs e) {
			((BarButtonGroup)sender).OnClearCollection(e);
		}
		protected static void OnCreateCollectionItemEvent(object sender, XtraCreateCollectionItemEventArgs e) {
			((BarButtonGroup)sender).OnCreateCollectionItem(e);
		}
		protected static void OnItemTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarButtonGroup)d).OnItemTemplateSelectorChanged(e);
		}
		protected static void OnItemStylePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarButtonGroup)d).OnItemStyleChanged(e);
		}
		protected static void OnItemTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarButtonGroup)d).OnItemTemplateChanged(e);
		}
		protected static void OnItemsSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarButtonGroup)d).OnItemsSourceChanged(e);
		}
		#endregion
		public BarButtonGroup() {
			ContextLayoutManagerHelper.AddLayoutUpdatedHandler(this, OnLayoutUpdated);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			immediateActionsManager.ExecuteActions();
		}
		BarItemLinkCollection itemLinks;
		BarItemLinkCollection mergedLinks;
		ObservableCollection<ILinksHolder> mergedLinksHolders;
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		CommonBarItemCollection itemsCore;
		public CommonBarItemCollection Items {
			get {
				if(itemsCore == null)
					itemsCore = new CommonBarItemCollection(this);
				return itemsCore;
			}
		}
		public BarItemLinkCollection ItemLinks {
			get {
				if(itemLinks == null)
					itemLinks = CreateItemLinksCollection();
				return itemLinks;
			}
		}
		protected ObservableCollection<ILinksHolder> MergedLinksHolders {
			get {
				if(mergedLinksHolders == null) {
					mergedLinksHolders = new ObservableCollection<ILinksHolder>();
					mergedLinksHolders.CollectionChanged += new NotifyCollectionChangedEventHandler(OnMergedLinksHoldersChanged);
				}
				return mergedLinksHolders;
			}
		}
		protected virtual void OnMergedLinksHoldersChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(e.OldItems!=null)
			foreach (ILinksHolder value in e.OldItems) {
				value.MergedParent = null;
			}
			if (e.NewItems != null)
			foreach (ILinksHolder value in e.NewItems) {
				value.MergedParent = this;
			}
		}
		protected virtual BarItemLinkCollection CreateItemLinksCollection() { 
			return new BarItemLinkCollection(this);
		}
		protected virtual void OnItemTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateItemsAttachedBehavior(e);
		}
		protected virtual void OnItemStyleChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateItemsAttachedBehavior(e);
		}
		protected virtual void OnItemTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateItemsAttachedBehavior(e);
		}
		protected virtual void OnItemsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemGeneratorHelper.OnItemsSourceChanged(e);
		}
		BarItemGeneratorHelper<BarButtonGroup> itemGeneratorHelper;
		protected BarItemGeneratorHelper<BarButtonGroup> ItemGeneratorHelper {
			get {
				if(itemGeneratorHelper == null)
					itemGeneratorHelper = new BarItemGeneratorHelper<BarButtonGroup>(this, ItemsAttachedBehaviorProperty, ItemStyleProperty, ItemTemplateProperty, Items, ItemTemplateSelectorProperty);
				return itemGeneratorHelper;
			}
		}
		protected virtual void UpdateItemsAttachedBehavior(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarButtonGroup, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty);
		}
		protected override IEnumerator LogicalChildren {
			get {
				return logicalChildrenContainerItems.GetEnumerator();
			}
		}		
		protected virtual void OnClearCollection(XtraItemRoutedEventArgs e) {
			if(e.Owner != this)
				return;
			ItemLinks.Clear();
		}
		protected virtual void OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			if(e.Owner != this)
				return;
			BarItemLinkBase link = BarItemLink.XtraCreateItemLinksItemCore(BarManager.GetBarManager(this), e);
			ItemLinks.Add(link);
			e.CollectionItem = link;
		}
		#region ILinksHolder Members
		bool ILinksHolder.ShowDescription { get { return false; } }
		BarItemLinkCollection ILinksHolder.Links {
			get { return ItemLinks; }
		}
		protected virtual BarItemLinkCollection CreateMergedLinks() { return new MergedItemLinkCollection(this); }
		BarItemLinkCollection ILinksHolder.MergedLinks {
			get {
				if(mergedLinks == null)
					mergedLinks = CreateMergedLinks();
				return mergedLinks;
			}
		}
		readonly ImmediateActionsManager immediateActionsManager = new ImmediateActionsManager();
		ImmediateActionsManager ILinksHolder.ImmediateActionsManager {
			get { return immediateActionsManager; }
		}
		BarItemLinkCollection ILinksHolder.ActualLinks {
			get { return ((ILinksHolder)this).IsMergedState ? ((ILinksHolder)this).MergedLinks : ((ILinksHolder)this).Links; }
		}
		ILinksHolder mergedParent;
		ILinksHolder ILinksHolder.MergedParent {
			get { return mergedParent; }
			set { mergedParent = value; }
		}
		void ILinksHolder.Merge(ILinksHolder holder) {
			if(MergedLinksHolders.Contains(holder))
				return;
			MergedLinksHolders.Add(holder);
		}
		void ILinksHolder.UnMerge(ILinksHolder holder) {
			MergedLinksHolders.Remove(holder);
		}
		void ILinksHolder.UnMerge() {
			MergedLinksHolders.Clear();
		}
		bool ILinksHolder.IsMergedState { get { return MergedLinksHolders.Count > 0; } }
		GlyphSize ILinksHolder.ItemsGlyphSize {
			get { return GlyphSize.Small; }
		}
		GlyphSize ILinksHolder.GetDefaultItemsGlyphSize(LinkContainerType linkContainerType) {
			return GlyphSize.Small;
		}
		IEnumerator ILinksHolder.GetLogicalChildrenEnumerator() {
			return new BarItemLinksAsLogicalChildrenEnumerator(this);
		}
		void ILinksHolder.OnLinkAdded(BarItemLinkBase link) {
			if(link.IsPrivate) return;
			AddLogicalChild(link);
		}
		void ILinksHolder.OnLinkRemoved(BarItemLinkBase link) {
			if(link.IsPrivate) return;
			RemoveLogicalChild(link);
		}
		LinksHolderType ILinksHolder.HolderType { get { return LinksHolderType.BarButtonGroup; } }
		#endregion
		protected override Type GetLinkType() {
			return typeof(BarButtonGroupLink);
		}
		#region ILogicalChildrenContainer
		readonly List<object> logicalChildrenContainerItems = new List<object>();
		void ILogicalChildrenContainer.AddLogicalChild(object child) {
			logicalChildrenContainerItems.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalChildrenContainer.RemoveLogicalChild(object child) {
			logicalChildrenContainerItems.Remove(child);
			RemoveLogicalChild(child);
		}
		#endregion
	}
}

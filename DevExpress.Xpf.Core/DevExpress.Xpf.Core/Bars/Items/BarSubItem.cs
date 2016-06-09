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
using DevExpress.Utils.Serializing;
using System.Windows;
using System.Collections;
using DevExpress.Xpf.Core.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Markup;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Bars {
	[ContentProperty("Items")]
	public class BarSubItem : BarButtonItem, ILinksHolder, ILogicalChildrenContainer {
		#region static
		public static readonly DependencyProperty SubItemsGlyphSizeProperty;
		public static readonly DependencyProperty ItemLinksSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemClickBehaviourProperty;
		public static readonly DependencyProperty FirstSectorIndexProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemLinksAttachedBehaviorProperty;  
		public static readonly DependencyProperty ItemLinksSourceElementGeneratesUniqueBarItemProperty;						
		static BarSubItem() {
			ItemClickBehaviourProperty = DependencyPropertyManager.Register("ItemClickBehaviour", typeof(PopupItemClickBehaviour), typeof(BarSubItem), new FrameworkPropertyMetadata(PopupItemClickBehaviour.Undefined, (d, e) => ((BarSubItem)d).OnItemClickBehaviourChanged((PopupItemClickBehaviour)e.OldValue)));
			SubItemsGlyphSizeProperty = DependencyPropertyManager.Register("SubItemsGlyphSize", typeof(GlyphSize), typeof(BarSubItem), new FrameworkPropertyMetadata(GlyphSize.Default, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSubItemsGlyphSizePropertyChanged)));			
			ItemLinksSourceProperty = DependencyProperty.Register("ItemLinksSource", typeof(IEnumerable), typeof(BarSubItem), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(BarSubItem), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(BarSubItem), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplateSelectorPropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), typeof(BarSubItem), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemLinksAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("ItemLinksAttachedBehavior", typeof(ItemsAttachedBehaviorCore<BarSubItem, BarItem>), typeof(BarSubItem), new UIPropertyMetadata(null));
			ItemLinksSourceElementGeneratesUniqueBarItemProperty = DependencyPropertyManager.Register("ItemLinksSourceElementGeneratesUniqueBarItem", typeof(bool), typeof(BarSubItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback((d, e) => ((BarSubItem)d).OnItemLinksSourceElementGeneratesUniqueBarItemChanged((bool)e.OldValue))));
			FirstSectorIndexProperty = DependencyPropertyManager.Register("FirstSectorIndex", typeof(int), typeof(BarSubItem), new PropertyMetadata(0));
		}		
		protected static void OnSubItemsGlyphSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarSubItem)d).OnSubItemGlyphSizeChanged(e);
		}
		protected static void OnItemLinksSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarSubItem)d).OnItemLinksSourceChanged(e);
		}
		protected static void OnItemLinksTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarSubItem)d).OnItemLinksTemplateChanged(e);
		}
		protected static void OnItemLinksTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarSubItem)d).OnItemLinksTemplateSelectorChanged(e);
		}
		#endregion
		public event EventHandler Popup, CloseUp, GetItemData;
		BarItemLinkCollection itemLinks;
		ObservableCollection<ILinksHolder> mergedLinksHolders;
		public BarSubItem() {
			ContextLayoutManagerHelper.AddLayoutUpdatedHandler(this, OnLayoutUpdated);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			immediateActionsManager.ExecuteActions();
		}
		CommonBarItemCollection itemsCore;
		public CommonBarItemCollection Items {
			get {
				if(itemsCore == null)
					itemsCore = new CommonBarItemCollection(this);
				return itemsCore;
			}
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarSubItemItemLinks"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]		
		public BarItemLinkCollection ItemLinks {
			get {
				if(itemLinks == null)
					itemLinks = CreateItemLinksCollection();
				return itemLinks;
			}
		}
		public PopupItemClickBehaviour ItemClickBehaviour {
			get { return (PopupItemClickBehaviour)GetValue(ItemClickBehaviourProperty); }
			set { SetValue(ItemClickBehaviourProperty, value); }
		}
		public IEnumerable ItemLinksSource {
			get { return (IEnumerable)GetValue(ItemLinksSourceProperty); }
			set { SetValue(ItemLinksSourceProperty, value); }
		}
		public bool ItemLinksSourceElementGeneratesUniqueBarItem {
			get { return (bool)GetValue(ItemLinksSourceElementGeneratesUniqueBarItemProperty); }
			set { SetValue(ItemLinksSourceElementGeneratesUniqueBarItemProperty, value); }
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
		public int FirstSectorIndex {
			get { return (int)GetValue(FirstSectorIndexProperty); }
			set { SetValue(FirstSectorIndexProperty, value); }
		}
		protected internal ObservableCollection<ILinksHolder> MergedLinksHolders {
			get {
				if(mergedLinksHolders == null) {
					mergedLinksHolders = new ObservableCollection<ILinksHolder>();
					mergedLinksHolders.CollectionChanged += new NotifyCollectionChangedEventHandler(OnMergedLinksHoldersChanged);
				}
				return mergedLinksHolders;
			}
		}
		readonly Locker collectionChangedLocker = new Locker();
		protected virtual void OnMergedLinksHoldersChanged(object sender, NotifyCollectionChangedEventArgs e) {
			collectionChangedLocker.DoLockedActionIfNotLocked(() => {
				BarItemLinkMergeHelper helper = new BarItemLinkMergeHelper();
				helper.Merge(((ILinksHolder)this).Links, MergedLinksHolders, ((ILinksHolder)this).MergedLinks);
				BarItemLinkBase.UpdateSeparatorsVisibility(this, false);
			});
			if(e.OldItems!=null)
			foreach (ILinksHolder value in e.OldItems) {
				value.MergedParent = null;
			}
			if(e.NewItems!=null)
			foreach (ILinksHolder value in e.NewItems) {
				value.MergedParent = this;
			}
		}
		protected internal virtual bool CanOpenMenu {
			get {
				if(!IsEnabled) return false;
				RaiseGetItemData();
				foreach(BarItemLinkBase linkBase in ((ILinksHolder)this).ActualLinks) {
					if((linkBase is BarItemLink) && ((BarItemLink)linkBase).Item is BarLinkContainerItem) {
						if(((BarLinkContainerItem)((BarItemLink)linkBase).Item).HasVisibleLinks)
							return true;						
					}
					else if(linkBase.ActualIsVisible)
						return true;
				}
				return false;
			}
		}
		protected override void OnNameChanged(string newValue, string oldValue) {
			base.OnNameChanged(newValue, oldValue);
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(ILinksHolder), oldValue, newValue);
		}
		protected virtual BarItemLinkCollection CreateItemLinksCollection() {
			return new BarItemLinkCollection(this);
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSubItemSubItemsGlyphSize")]
#endif
		public GlyphSize SubItemsGlyphSize {
			get { return (GlyphSize)GetValue(SubItemsGlyphSizeProperty); }
			set { SetValue(SubItemsGlyphSizeProperty, value); }
		}
		protected override IEnumerator LogicalChildren {
			get {
				return ((ILinksHolder)this).GetLogicalChildrenEnumerator();
			}
		}
		protected virtual void UpdateLinkHolders() {
			foreach(BarItemLinkBase link in Links) {
				if(!(link is BarSubItemLink)) continue;
				BarSubItemLinkControl linkControl = ((BarSubItemLink)link).LinkControl as BarSubItemLinkControl;
				if(linkControl == null) continue;
				linkControl.ForceUpdatePopupContentControlLinkHolder();
			}
		}		
		protected internal virtual void RaiseGetItemData() {
			if(GetItemData != null) {
				ItemLinks.Clear();
				GetItemData(this, EventArgs.Empty);
			}
		}
		protected internal virtual void RaisePopup() {
			if(Popup != null)
				Popup(this, EventArgs.Empty);
		}
		protected internal void RaiseCloseUp() {
			if(CloseUp != null)
				CloseUp(this, EventArgs.Empty);
		}
		BarItemLinkCollection ILinksHolder.Links { get { return ItemLinks; } }
		IEnumerable ILinksHolder.ItemsSource { get { return ItemLinksSource; } }
		BarItemLinkCollection mergedLinks;
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
			UpdateLinkHolders();
		}
		void ILinksHolder.UnMerge(ILinksHolder holder) {
			MergedLinksHolders.Remove(holder);
			UpdateLinkHolders();
		}
		void ILinksHolder.UnMerge() {
			MergedLinksHolders.Clear();
			UpdateLinkHolders();
		}
		bool ILinksHolder.IsMergedState { get { return MergedLinksHolders.Count > 0; } }
		bool ILinksHolder.ShowDescription { get { return false; } }
		GlyphSize ILinksHolder.ItemsGlyphSize { get { return SubItemsGlyphSize; } }
		GlyphSize ILinksHolder.GetDefaultItemsGlyphSize(LinkContainerType linkContainerType) {
			return BarManager.GetBarManager(this).If(x=>x.MenuGlyphSize != GlyphSize.Default).Return(x=>x.MenuGlyphSize, ()=>GlyphSize.Default);			
		}
		IEnumerator ILinksHolder.GetLogicalChildrenEnumerator() {
			return logicalChildrenContainerItems.GetEnumerator();
		}
		void ILinksHolder.OnLinkAdded(BarItemLinkBase link) {
			if (LogicalTreeHelper.GetParent(link) == null)
				AddLogicalChild(link);
		}
		void ILinksHolder.OnLinkRemoved(BarItemLinkBase link) {			
			RemoveLogicalChild(link);
		}
		LinksHolderType ILinksHolder.HolderType { get { return LinksHolderType.BarSubItem; } }		
		protected virtual void OnSubItemGlyphSizeChanged(DependencyPropertyChangedEventArgs e) {
			UpdateLinkControlActualGlyph();
		}
		protected virtual void OnItemLinksSourceElementGeneratesUniqueBarItemChanged(bool oldValue) {
			OnItemLinksSourceChanged(new DependencyPropertyChangedEventArgs(ItemLinksSourceProperty, ItemLinksSource, ItemLinksSource));
		}
		protected virtual void OnItemClickBehaviourChanged(PopupItemClickBehaviour oldValue) {			
			ExecuteActionOnLinkControls<BarSubItemLinkControl>(lc => lc.UpdateActualItemClickBehaviour());
		}
		private void OnItemLinksTemplateChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarSubItem, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e, ItemLinksAttachedBehaviorProperty);
		}
		private void OnItemLinksTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarSubItem, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemLinksAttachedBehaviorProperty);
		}
		protected virtual void OnItemLinksSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemGeneratorHelper.OnItemsSourceChanged(e);
		}
		protected internal virtual void UpdateLinkControlActualGlyph() {
			ExecuteActionOnLinkControls(lc => lc.UpdateActualGlyph());
		}		
		BarItemGeneratorHelper<BarSubItem> itemGeneratorHelper;
		protected BarItemGeneratorHelper<BarSubItem> ItemGeneratorHelper {
			get {
				if(itemGeneratorHelper == null)
					itemGeneratorHelper = new BarItemGeneratorHelper<BarSubItem>(this, ItemLinksAttachedBehaviorProperty, ItemStyleProperty, ItemTemplateProperty, Items, ItemTemplateSelectorProperty, ItemLinksSourceElementGeneratesUniqueBarItem);
				return itemGeneratorHelper;
			}
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
		protected override IEnumerable<object> GetRegistratorKeys() {
			return new object[] { typeof(ILinksHolder) };
		}
		protected override object GetRegistratorName(object registratorKey) {
			if (Equals(registratorKey, typeof(ILinksHolder)))
				return Name;
			return base.GetRegistratorName(registratorKey);
		}	   
	}
}

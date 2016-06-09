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
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Windows.Markup;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Bars {
	[ContentProperty("Items")]
	public class BarLinkContainerItem : BarItem, ILinksHolder, IInplaceLinksHolder, ILogicalChildrenContainer {
		#region static
		public static readonly DependencyProperty SubItemsGlyphSizeProperty;
		public static readonly DependencyProperty ItemLinksSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemStyleProperty;		
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemLinksAttachedBehaviorProperty;  
		public static readonly DependencyProperty ItemLinksSourceElementGeneratesUniqueBarItemProperty;		 
		static BarLinkContainerItem() {
			SubItemsGlyphSizeProperty = DependencyPropertyManager.Register("SubItemsGlyphSize", typeof(GlyphSize), typeof(BarLinkContainerItem), new FrameworkPropertyMetadata(GlyphSize.Default, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSubItemsGlyphSizePropertyChanged)));
			ItemLinksSourceProperty = DependencyProperty.Register("ItemLinksSource", typeof(IEnumerable), typeof(BarLinkContainerItem), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(BarLinkContainerItem), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(BarLinkContainerItem), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplateSelectorPropertyChanged)));
			ItemLinksAttachedBehaviorProperty = DependencyProperty.RegisterAttached("ItemLinksAttachedBehavior", typeof(ItemsAttachedBehaviorCore<BarLinkContainerItem, BarItem>), typeof(BarLinkContainerItem), new PropertyMetadata(null));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), typeof(BarLinkContainerItem), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemLinksSourceElementGeneratesUniqueBarItemProperty = DependencyPropertyManager.Register("ItemLinksSourceElementGeneratesUniqueBarItem", typeof(bool), typeof(BarLinkContainerItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback((d, e) => ((BarLinkContainerItem)d).OnItemLinksSourceElementGeneratesUniqueBarItemChanged((bool)e.OldValue))));			
		}		
		public BarLinkContainerItem() {
			ContextLayoutManagerHelper.AddLayoutUpdatedHandler(this, OnLayoutUpdated);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			immediateActionsManager.ExecuteActions();
		}		
		protected static void OnSubItemsGlyphSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarLinkContainerItem)d).OnSubItemGlyphSizeChanged(e);
		}
		protected static void OnItemLinksSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarLinkContainerItem)d).OnItemLinksSourceChangedAsync(e);
		}
		protected static void OnItemLinksTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarLinkContainerItem)d).OnItemLinksTemplateChanged(e);
		}
		protected static void OnItemLinksTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarLinkContainerItem)d).OnItemLinksTemplateSelectorChanged(e);
		}
		#endregion
		BarItemLinkCollection itemLinks;
		ObservableCollection<ILinksHolder> mergedLinksHolders;
		CommonBarItemCollection itemsCore;
		public CommonBarItemCollection Items { get { return itemsCore ?? (itemsCore = CreateCommonBarItemCollection()); } }
		protected virtual CommonBarItemCollection CreateCommonBarItemCollection() {
			return new CommonBarItemCollection(this);
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarLinkContainerItemItemLinks")]
#endif
		public virtual BarItemLinkCollection ItemLinks {
			get {
				if(itemLinks == null) {
					itemLinks = CreateItemLinksCollection();
					itemLinks.CollectionChanged += new NotifyCollectionChangedEventHandler(OnItemLinksCollectionChanged);
				}
				return itemLinks;
			}
		}
		protected virtual void OnItemLinksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(!((ILinksHolder)this).IsMergedState) {
				ExecuteActionOnLinks((l) =>  {
					foreach(BarItemLinkInfo linkInfo in l.LinkInfos) {
						linkInfo.OnChildLinkCollectionChanged(e);
					}
				});
			}
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
		protected virtual void OnMergedLinksHoldersChanged(object sender, NotifyCollectionChangedEventArgs e) {
			BarItemLinkMergeHelper helper = new BarItemLinkMergeHelper();
			helper.Merge(((ILinksHolder)this).Links, MergedLinksHolders, ((ILinksHolder)this).MergedLinks);
			if(e.OldItems!=null)
			foreach (ILinksHolder value in e.OldItems) {
				value.MergedParent = null;
			}
			if(e.NewItems!=null)
			foreach (ILinksHolder value in e.NewItems) {
				value.MergedParent = this;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarLinkContainerItemSubItemsGlyphSize")]
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
		protected virtual BarItemLinkCollection CreateItemLinksCollection() {
			return new BarItemLinkCollection(this);
		}		
		internal bool ForceRefreshLinksControl { get; set; }
		protected void ForceUpdateLinkControls() {
			foreach(BarItemLinkBase link in ItemLinks) {
				link.HoldersIsVisible = IsVisible;
				link.UpdateProperties();								
			}
		}
		internal bool HasVisibleLinks {
			get {
				foreach(BarItemLinkBase linkBase in ItemLinks) {
					if(linkBase.ActualIsVisible)
						return true;
				}
				return false;
			}
		}
		protected override void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			base.OnIsEnabledChanged(sender, e);
			ForceUpdateLinkControls();
		}
		protected override void OnIsVisibleChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsVisibleChanged(e);
			ForceUpdateLinkControls();
		}
		protected virtual void OnSubItemGlyphSizeChanged(DependencyPropertyChangedEventArgs e) {
			foreach(BarItemLinkBase link in ItemLinks) {
				link.ExecuteActionOnLinkControls((lc) => lc.UpdateActualGlyph());
			}
		}
		protected virtual void OnItemLinksSourceElementGeneratesUniqueBarItemChanged(bool oldValue) {
			OnItemLinksSourceChangedAsync(new System.Windows.DependencyPropertyChangedEventArgs(ItemLinksSourceProperty, ItemLinksSource, ItemLinksSource));
		}
		protected virtual void OnItemLinksTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarLinkContainerItem, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemLinksAttachedBehaviorProperty);
		}
		protected virtual void OnItemLinksTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarLinkContainerItem, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemLinksAttachedBehaviorProperty);
		}
		protected virtual void OnItemLinksSourceChangedAsync(System.Windows.DependencyPropertyChangedEventArgs e) {
			Dispatcher.BeginInvoke(new Action(() => OnItemLinksSourceChanged(e)), null);
		}
		protected virtual void OnItemLinksSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemGeneratorHelper.OnItemsSourceChanged(e);
		}
			#region ILinksHolder Members
			BarItemLinkCollection ILinksHolder.Links {
			get { return ItemLinks; }
		}
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
		bool ILinksHolder.ShowDescription { get { return false; } }
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
		GlyphSize ILinksHolder.ItemsGlyphSize { get { return SubItemsGlyphSize; } }
		GlyphSize ILinksHolder.GetDefaultItemsGlyphSize(LinkContainerType linkContainerType) {
			return GlyphSize.Default;
		}
		IEnumerator ILinksHolder.GetLogicalChildrenEnumerator() {
			return logicalChildrenContainerItems.GetEnumerator();
		}
		void ILinksHolder.OnLinkAdded(BarItemLinkBase link) {
			if(LogicalTreeHelper.GetParent(link)==null)
			AddLogicalChild(link);
		}
		void ILinksHolder.OnLinkRemoved(BarItemLinkBase link) {
			RemoveLogicalChild(link);
		}
		LinksHolderType ILinksHolder.HolderType { get { return LinksHolderType.BarLinkContainerItem; } }
		#endregion
		BarItemGeneratorHelper<BarLinkContainerItem> itemGeneratorHelper;
		protected BarItemGeneratorHelper<BarLinkContainerItem> ItemGeneratorHelper {
			get {
				if(itemGeneratorHelper == null)
					itemGeneratorHelper = new BarItemGeneratorHelper<BarLinkContainerItem>(this, ItemLinksAttachedBehaviorProperty, ItemStyleProperty, ItemTemplateProperty, Items, ItemTemplateSelectorProperty, ItemLinksSourceElementGeneratesUniqueBarItem);
				return itemGeneratorHelper;
			}
		}
		#region IInplaceLinksHolder Members
		protected virtual void UpdateCore() { 
		}
		void IInplaceLinksHolder.Update() {
			UpdateCore();			   
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
	public class BarItemSelector : BarLinkContainerItem {
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty AllowEmptySelectionProperty;
		public static readonly DependencyProperty ClearSelectionWhenItemsChangedProperty;
		readonly int groupIndex;
		readonly Locker initializationLocker;		
		static BarItemSelector() {
			SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem", typeof(object), typeof(BarItemSelector), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((BarItemSelector)d).OnSelectedItemChanged(e.OldValue)));
			AllowEmptySelectionProperty = DependencyPropertyManager.Register("AllowEmptySelection", typeof(bool), typeof(BarItemSelector), new FrameworkPropertyMetadata(false, (d, e) => ((BarItemSelector)d).OnAllowEmptySelectionChanged((bool)e.OldValue)));
			ClearSelectionWhenItemsChangedProperty = DependencyPropertyManager.Register("ClearSelectionWhenItemsChanged", typeof(bool), typeof(BarItemSelector), new FrameworkPropertyMetadata(false));
		}
		public BarItemSelector() {
			this.groupIndex = GetHashCode();
			this.initializationLocker = new Locker();
			initializationLocker.Unlocked += OnInitializationLockerUnlocked;
		}		
		public object SelectedItem {
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public bool AllowEmptySelection {
			get { return (bool)GetValue(AllowEmptySelectionProperty); }
			set { SetValue(AllowEmptySelectionProperty, value); }
		}
		public bool ClearSelectionWhenItemsChanged {
			get { return (bool)GetValue(ClearSelectionWhenItemsChangedProperty); }
			set { SetValue(ClearSelectionWhenItemsChangedProperty, value); }
		}
		protected int GroupIndex {
			get { return groupIndex; }
		}
		protected override CommonBarItemCollection CreateCommonBarItemCollection() {
			var result = new CommonBarItemCollection(this);
			result.CollectionChanged += OnItemsCollectionChanged;
			return result;
		}		
		protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null)
				foreach (IBarItem element in e.OldItems) {
					var item = GetBarItem(element);
					ClearItem(item);
					if (SelectedItem != null && CanSelectItem(item) && (item.With(x => x.DataContext) == SelectedItem || item == SelectedItem))
						if (ClearSelectionWhenItemsChanged)
							SelectedItem = null;
				}
			if (e.NewItems != null)
				foreach (IBarItem element in e.NewItems) {
					var item = GetBarItem(element);
					UpdateItem(item);
					if (CanSelectItem(item) && GetIsSelected(item).Return(x => x.Value, () => false))
						SelectedItem = item;
					if (CanSelectItem(item) && SelectedItem != null && Equals(GetSourceByItem(item), SelectedItem))
						SetIsSelected(item, true);
				}
		}
		void UpdateItem(BarItem item) {
			if (!CanSelectItem(item))
				return;
			IBarCheckItem cItem = item as IBarCheckItem;
			cItem.GroupIndex = GroupIndex;
			cItem.AllowUncheckInGroup = AllowEmptySelection;
			item.AddHandler(BarCheckItem.CheckedChangedEvent, new ItemClickEventHandler(OnItemCheckedChanged));
		}		
		void ClearItem(BarItem item) {
			if (!CanSelectItem(item))
				return;
			IBarCheckItem cItem = item as IBarCheckItem;
			cItem.GroupIndex = -1;
			cItem.AllowUncheckInGroup = false;
			item.RemoveHandler(BarCheckItem.CheckedChangedEvent, new ItemClickEventHandler(OnItemCheckedChanged));
		}
		protected virtual void OnAllowEmptySelectionChanged(bool oldValue) {
			foreach (var item in Items) {
				(GetBarItem(item) as IBarCheckItem).Do(x => x.AllowUncheckInGroup = AllowEmptySelection);
			}
			CheckEmptySelection();
		}		
		public override void BeginInit() {
			initializationLocker.Lock();
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			initializationLocker.Unlock();
		}
		protected virtual void OnInitializationLockerUnlocked(object sender, EventArgs e) {
			CheckEmptySelection();
		}
		protected virtual void CheckEmptySelection() {
			if (initializationLocker.IsLocked)
				return;
			if (Equals(null, SelectedItem) && !AllowEmptySelection) {
				var sItem = Items.Select(GetBarItem).OfType<IBarCheckItem>().FirstOrDefault();
				if (sItem == null)
					return;
				SelectedItem = GetSourceByItem((BarItem)sItem);
			}
		}
		protected virtual void OnItemCheckedChanged(object sender, ItemClickEventArgs e) {
			var cItem = (IBarCheckItem)sender;
			if(Equals(GetItemBySource(SelectedItem), cItem)) {
				if (!cItem.IsChecked.Return(x => x.Value, () => false))
					SelectedItem = null;
			} else {
				if (cItem.IsChecked.Return(x => x.Value, () => false))
					SelectedItem = GetSourceByItem((BarItem)sender);
			}
		}
		protected virtual void OnSelectedItemChanged(object oldValue) {
			var bi = GetItemBySource(oldValue).If(CanSelectItem);
			if (bi != null) {
				SetIsSelected(bi, false);
			}
			bi = GetItemBySource(SelectedItem).If(CanSelectItem);
			if (bi != null) {
				SetIsSelected(bi, true);
			}
		}
		protected override void OnItemLinksSourceChanged(DependencyPropertyChangedEventArgs e) {
			using (initializationLocker.Lock()) {
				base.OnItemLinksSourceChanged(e);
			}			
		}
		protected override void OnItemLinksTemplateChanged(DependencyPropertyChangedEventArgs e) {
			using (initializationLocker.Lock()) {
				base.OnItemLinksTemplateChanged(e);
			}
		}
		protected override void OnItemLinksTemplateSelectorChanged(DependencyPropertyChangedEventArgs e) {
			using (initializationLocker.Lock()) {
				base.OnItemLinksTemplateSelectorChanged(e);
			}
		}
		#region helpers
		protected virtual BarItem GetItemBySource(object source) {
			if (source == null)
				return null;
			if (source is IBarItem)
				return GetBarItem((IBarItem)source);
			var ien = ItemLinksSource as IEnumerable;
			if (ien == null)
				return null;
			int index = 0;
			foreach (var element in ien) {
				if (Equals(element, source) && Items.IsValidIndex(index))
					return GetBarItem(Items[index]);
				index++;
			}
			return null;
		}
		protected virtual BarItem GetBarItem(IBarItem source) { return source as BarItem ?? (source as BarItemLink).With(x => x.Item); }
		protected virtual object GetSourceByItem(IBarItem element) {
			if (element == null)
				return null;
			var ien = (ItemLinksSource as IEnumerable).With(Enumerable.OfType<object>);
			if (ien == null)
				return element;
			int index = 0;
			var lItem = GetBarItem(element);
			if (lItem == null)
				return null;
			foreach (var item in Items) {
				var rItem = GetBarItem(item);
				if (rItem == null) continue;
				if (Equals(lItem, rItem) && ien.ToList().IsValidIndex(index))
					return ien.ElementAt(index);
				index++;
			}
			return null;
		}
		protected bool CanSelectItem(BarItem item) {
			return item is IBarCheckItem;
		}
		protected virtual void SetIsSelected(BarItem item, bool value) {
			(item as IBarCheckItem).Do(x => x.IsChecked = value);
		}
		protected virtual bool? GetIsSelected(BarItem item) {
			if (item is IBarCheckItem) {
				return ((IBarCheckItem)item).IsChecked;
			}
			return null;
		}
#endregion
	}
	public class BarItemSelectorLink : BarLinkContainerItemLink {
	}	
}

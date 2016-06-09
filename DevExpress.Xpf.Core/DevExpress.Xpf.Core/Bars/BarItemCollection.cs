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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Utils.Serializing;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Bars {
	public enum MDIMergeStyle {
		Default,
		WhenChildActivated,
		Always,
		Never
	}
	public enum LinksHolderType { None, BarLinkContainerItem, BarSubItem, RibbonStatusBarLeft, RibbonStatusBarRight, ApplicationMenu, Bar, PopupMenu, RibbonGalleryBarItem, BarButtonGroup, RibbonPageGroup, RibbonQuickAccessToolbar, RibbonPageHeader, BarItemMenuHeader, RadialMenu };
	public interface ILinksHolder : IMultipleElementRegistratorSupport, ILogicalChildrenContainer {
		BarItemLinkCollection Links { get; }
		CommonBarItemCollection Items { get; }
		IEnumerable ItemsSource { get; }
		BarItemLinkCollection MergedLinks { get; }
		BarItemLinkCollection ActualLinks { get; }
		bool IsMergedState { get; }
		ILinksHolder MergedParent { get; set; }
		void Merge(ILinksHolder holder);
		void UnMerge(ILinksHolder holder);
		void UnMerge();
		GlyphSize ItemsGlyphSize { get; }
		bool ShowDescription { get; }
		GlyphSize GetDefaultItemsGlyphSize(LinkContainerType linkContainerType);
		IEnumerator GetLogicalChildrenEnumerator();
		void OnLinkAdded(BarItemLinkBase link);
		void OnLinkRemoved(BarItemLinkBase link);
		LinksHolderType HolderType { get; }
		ImmediateActionsManager ImmediateActionsManager { get; }
	}
	public interface IInplaceLinksHolder : ILinksHolder {
		void Update();
	}
	public class BarItemLinkCollectionEventArgs : EventArgs {
		public BarItem Item { get; protected set; }
		public int ItemIndex { get; protected set; }
		public BarItemLinkCollectionEventArgs(BarItem item, int itemIndex) {
			Item = item;
			ItemIndex = itemIndex;
		}
	}
	public delegate void BeforeRemoveBarItemEventHandler(object sender, BarItemLinkCollectionEventArgs e);
	public delegate void AfterInsertBarItemEventHandler(object sender, BarItemLinkCollectionEventArgs e);
	public class BarItemCollection : ObservableCollection<BarItem> {
		int updating;
		public event EventHandler OnBeginUpdate;
		public event EventHandler OnEndUpdate;
		public event BeforeRemoveBarItemEventHandler BeforeRemove;
		public event BeforeRemoveBarItemEventHandler AfterInsert;
		public BarItemCollection() { }
		public void BeginUpdate() {
			this.updating++;
			if(this.updating == 1 && OnBeginUpdate != null)
				OnBeginUpdate(this, new EventArgs());
		}
		public void EndUpdate() {
			this.updating--;
			if(this.updating < 0)
				this.updating = 0;
			if(!IsUpdating) 
				OnBarItemCollectionChanged();
			if(this.updating == 0 && OnEndUpdate != null)
				OnEndUpdate(this, new EventArgs());
		}
		public BarItem this[string name] { 
			get {
				foreach(BarItem item in this) {
					if(item.Name == name) return item;
				}
				return null;
			} 
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemCollectionIsUpdating")]
#endif
		public bool IsUpdating { get { return this.updating > 0; } }		
		protected override void InsertItem(int index, BarItem barItem) {
			base.InsertItem(index, barItem);
			OnInsertItem(barItem, index);
		}
		protected override void RemoveItem(int index) {
			OnRemoveItem(this[index], index);
			base.RemoveItem(index);
		}
		protected override void ClearItems() {
			while (Items.Count > 0)
				RemoveAt(0);			
		}
		protected override void SetItem(int index, BarItem item) {
			BarItem oldItem = this[index];
			OnRemoveItem(oldItem, index);
			base.SetItem(index, item);
			OnInsertItem(item, index);
		}		
		protected virtual void OnRemoveItem(BarItem barItem, int index) {
			if(BeforeRemove != null)
				BeforeRemove(this, new BarItemLinkCollectionEventArgs(barItem, index));
		}
		protected internal virtual void OnInsertItem(BarItem barItem, int index) {
			if(IsUpdating)
				return;
			if(AfterInsert != null)
				AfterInsert(this, new BarItemLinkCollectionEventArgs(barItem, index));
		}		
		protected internal virtual void OnBarItemCollectionChanged() {
			if(IsUpdating)
				return;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}
	public class BarItemLinkMergeHelper {
		public void Merge(BarItemLinkCollection mainColl, ObservableCollection<ILinksHolder> collArray, BarItemLinkCollection resultColl) {
			if(collArray == null)
				return;
			List<BarItemLinkMergeInfo> res = new List<BarItemLinkMergeInfo>();
			UnMerge(mainColl);
			foreach(BarItemLinkBase linkBase in mainColl)
				if(linkBase.ActualMergeType != BarItemMergeType.Remove) MergeLink_AddType(linkBase, res);
			foreach(ILinksHolder holder in collArray) Merge(holder, res);
			int collectionIndex = 0;
			foreach(BarItemLinkMergeInfo info in res) {
				info.CollectionIndex = collectionIndex;
				collectionIndex++;
			}
			res.Sort(new MergeLinksComparer());
			resultColl.BeginUpdate();
			resultColl.Clear();
			foreach(BarItemLinkMergeInfo info in res) {
				resultColl.Add(info.Link);
			}
			resultColl.EndUpdate();
		}
		public void UnMerge(BarItemLinkCollection coll) {
			foreach(BarItemLinkBase linkBase in coll) {
				BarItemLink link = linkBase as BarItemLink;
				if(link == null) continue;
				ILinksHolder holder = link.Item as ILinksHolder;
				if(holder == null) continue;
				holder.UnMerge();
			}
		}
		public void Merge(ILinksHolder coll, List<BarItemLinkMergeInfo> res) {
			BarItemLinkBase link;
			for(int i = 0; i < coll.ActualLinks.Count; i++) {
				link = coll.ActualLinks[i] as BarItemLinkBase;
				if(link == null || link.ActualMergeType == BarItemMergeType.Remove) continue;
				if(link.ActualMergeType == BarItemMergeType.Add) MergeLink_AddType(link, res);
				else if(link.ActualMergeType == BarItemMergeType.MergeItems) MergeLink_MergeItemsType(link, res);
				else if(link.ActualMergeType == BarItemMergeType.Replace) MergeLink_ReplaceType(link, res);
			}
		}
		public void MergeLink_AddType(BarItemLinkBase link, List<BarItemLinkMergeInfo> res) {
			BarItemLinkMergeInfo info = new BarItemLinkMergeInfo(link);
			if(link.ActualMergeOrder < res.Count && link.ActualMergeOrder != -1) 
				res.Insert(link.ActualMergeOrder, info);
			else res.Add(info);
		}
		public void MergeLink_ReplaceType(BarItemLinkBase link, List<BarItemLinkMergeInfo> res) {
			BarItemLink replaceLink = link as BarItemLink;
			if(replaceLink != null) {
				foreach(BarItemLinkMergeInfo info in res) {
					BarItemLink linkInColl = info.Link as BarItemLink;
					if(linkInColl == null || GetName(linkInColl) != GetName(replaceLink))
						continue;
					int index = res.IndexOf(info);
					res[index] = new BarItemLinkMergeInfo(replaceLink);
					return;
				}
			}
			MergeLink_AddType(link, res);
		}
		string GetName(BarItemLink dObj) {
			return MergingProperties.GetName(dObj).WithString(x => x) ?? dObj.With(x => x.Item).With(MergingProperties.GetName).WithString(x => x) ?? dObj.BarItemName;
		}
		public void MergeLink_MergeItemsType(BarItemLinkBase link, List<BarItemLinkMergeInfo> res) {
			BarItemLink secondaryLink = link as BarItemLink;
			BarItemLink primaryLink = FindLinkByContent(secondaryLink.ActualContent, res);
			ILinksHolder secondaryItem = secondaryLink != null ? secondaryLink.Item as ILinksHolder : null;
			ILinksHolder primaryItem = primaryLink != null ? primaryLink.Item as ILinksHolder : null;
			if(secondaryItem == null || primaryItem == null) {
				MergeLink_AddType(link, res);
				return;
			}
			((ILinksHolder)primaryItem).Merge((ILinksHolder)secondaryItem);
		}
		BarItemLink FindLinkByContent(object content, List<BarItemLinkMergeInfo> res) {
			foreach(BarItemLinkMergeInfo info in res) {
				BarItemLink link = info.Link as BarItemLink;
				if(link == null || link.ActualContent==null) continue;
				if(link.ActualContent.Equals(content))
					return link;
			}
			return null;
		}
		class MergeLinksComparer : IComparer<BarItemLinkMergeInfo> {
			int IComparer<BarItemLinkMergeInfo>.Compare(BarItemLinkMergeInfo link1, BarItemLinkMergeInfo link2) {
				if(link1 == link2) return 0;
				int res = link1.Link.ActualMergeOrder.CompareTo(link2.Link.ActualMergeOrder);
				if(res != 0 && link1.Link.ActualMergeOrder == -1) res = -1;
				if(res != 0 && link2.Link.ActualMergeOrder == -1) res = 1;
				if(res != 0) return res;
				res = link1.Index.CompareTo(link2.Index);
				if(res != 0)
					return res;
				return link1.CollectionIndex.CompareTo(link2.CollectionIndex);
			}
		} 
	}
	public class BarItemLinkMergeInfo {
		public BarItemLinkMergeInfo(BarItemLinkBase link) {
			Link = link;
		}
		public BarItemLinkBase Link { get; set; }
		public int Index { get; set; }
		public int CollectionIndex { get; set; }
	}
	public class WeakBarItemList : DevExpress.Xpf.Bars.Native.WeakList<BarItem> {
		public BarManager Manager { get; private set; }
		public WeakBarItemList(BarManager manager) {
			if(manager == null)
				throw new ArgumentNullException("manager");
			Manager = manager;
		}
		public override void Clear() {
			List<BarItem> items = new List<BarItem>();
			for(int i = 0; i < Count; i++) {
				items.Add(this[i]);
			}
			base.Clear();
			foreach(BarItem item in items) {
				Manager.RemoveLogicalChild(item);
			}
		}
		public override void Insert(int index, BarItem item) {
			base.Insert(index, item);
			if(item.Parent == null)
				Manager.AddLogicalChild(item);
		}
		public override void RemoveAt(int index) {
			var elem = this[index];
			Manager.RemoveLogicalChild(elem);
			base.RemoveAt(index);
		}
	}
	public class BarItemLinkCollection : SimpleLinkCollection {
		protected virtual bool EnableLinkCollectionLogic { get { return true; } }
		ILinksHolder holder;
		public BarItemLinkCollection() {
			holder = new Bar();
		}
		public BarItemLinkCollection(ILinksHolder holder)  {
			this.holder = holder;
			if (!EnableLinkCollectionLogic)
				return;
			var handler = new DependencyPropertyChangedEventHandler((o, e) => this.ForEach(x => x.OnItemIsEnabledChanged()));
			if (holder is UIElement) {
				((UIElement)holder).IsEnabledChanged += handler;
			}
			if (holder is ContentElement) {
				((ContentElement)holder).IsEnabledChanged += handler;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkCollectionHolder")]
#endif
		public ILinksHolder Holder { get { return holder; } }
		internal void RaiseReset() {
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		readonly Locker insertItemLocker = new Locker();
		readonly Locker removeItemLocker = new Locker();
		protected internal IDisposable LockItemInsertationToCommonCollection(){
			return insertItemLocker.Lock();
		}
		protected internal IDisposable LockItemRemovalFromCommonCollection() {
			return removeItemLocker.Lock();
		}
		protected override void InsertItem(int index, BarItemLinkBase itemLink) {
			if (!EnableLinkCollectionLogic) {
				base.InsertItem(index, itemLink);
				return;
			}
			itemLink.Index = index;
			bool shouldReturn = false;
			insertItemLocker.DoLockedActionIfNotLocked(() => { Holder.Items.Insert(index, itemLink); shouldReturn = true; });
			if (shouldReturn)
				return;
			if (addItemLocker.IsLocked)
				link = itemLink;
			if(Contains(itemLink))
				throw new InvalidOperationException("This ItemLink has already been added to the current collection");
			base.InsertItem(index, itemLink);
			if (!IsUpdateLocked)
				UpdateItemLinkOnAddToCollection(itemLink);						
		}
		void UpdateItemLinkOnAddToCollection(BarItemLinkBase itemLink) {
			itemLink.Links = this;			
			if(Holder != null)
				Holder.OnLinkAdded(itemLink);
		}
		internal bool lockLinkPropertyChanges = false;
		protected override void RemoveItem(int index) {
			if (!EnableLinkCollectionLogic) {
				base.RemoveItem(index);
				return;
			}
			bool shouldReturn = false;
			removeItemLocker.DoLockedActionIfNotLocked(() => { Holder.Items.RemoveAt(index); shouldReturn = true; });
			if (shouldReturn)
				return;
			BarItemLinkBase link = this[index];
			if(Holder != null && !lockLinkPropertyChanges)
				Holder.OnLinkRemoved(link);
			base.RemoveItem(index);
			ClearLink(link);
		}
		public BarItemLink this[string name] {
			get {
				foreach(BarItemLinkBase baseLink in this) {
					BarItemLink link = baseLink as BarItemLink;
					if(link != null && (string)link.Name == name) return link;
				}
				return null;
			}
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (!EnableLinkCollectionLogic) {				
				return;
			}	   
			if(e.Action == NotifyCollectionChangedAction.Reset && Count > 0) 
				foreach(BarItemLinkBase itemLink in this) UpdateItemLinkOnAddToCollection(itemLink);
		}
		readonly Locker addItemLocker = new Locker();
		BarItemLinkBase link;
		public BarItemLinkBase Add(BarItem item) {
			return Insert(Count, item);
		}
		public BarItemLinkBase Insert(int index, BarItem item) {
			if (!EnableLinkCollectionLogic) {
				throw new InvalidOperationException();
			}	   
			try {
				addItemLocker.DoLockedAction(() => { Holder.Items.Insert(index, item); });
				return link;
			} finally {
				link = null;
			}			
		}
		protected void ClearLink(BarItemLinkBase link) {
			if(!lockLinkPropertyChanges) {
				link.Clear(false);
			}			
		}		
		protected override void ClearItems() {
			if (!EnableLinkCollectionLogic) {
				base.ClearItems();
				return;
			}  
			Holder.Items.Clear();
		}		
		protected override void SetItem(int index, BarItemLinkBase item) {
			if (!EnableLinkCollectionLogic) {
				base.SetItem(index, item);
				return;
			}  
			BarItemLinkBase oldItem = this[index];
			if(Holder != null && !lockLinkPropertyChanges) Holder.OnLinkRemoved(oldItem);
			ClearLink(oldItem);
			base.SetItem(index, item);
			if(Holder != null)
				Holder.OnLinkAdded(item);
			UpdateItemLinkOnAddToCollection(item);
		}
		public virtual bool Contains(BarItem item) {			
			if(item == null)
				return false;
			foreach(BarItemLinkBase link in Items) {
				if(link is BarItemLink && item.Equals(((BarItemLink)link).Item))
					return true;
			}
			return false;
		}		
	}
	public class MergedItemLinkCollection : BarItemLinkCollection {
		protected override bool EnableLinkCollectionLogic { get { return false; } }
		public MergedItemLinkCollection(ILinksHolder holder) : base(holder) { }		
	}
	public class SimpleLinkCollection : LockableCollection<BarItemLinkBase> {
	}
	public class ReadOnlyLinkCollection : SimpleLinkCollection {
		public ReadOnlyLinkCollection() { 
		}
		internal bool AllowModifyCollection { get; set; }
		protected override void InsertItem(int index, BarItemLinkBase item) {
			if(AllowModifyCollection)
				base.InsertItem(index, item);
			else
				throw new InvalidOperationException("Add links to the ItemLinks collection instead.");
		}
		protected override void SetItem(int index, BarItemLinkBase item) {
			if(AllowModifyCollection)
				base.SetItem(index, item);
		}
		protected override void RemoveItem(int index) {
			if(AllowModifyCollection)
				base.RemoveItem(index);
		}
		protected override void ClearItems() {
			if(AllowModifyCollection)
				base.ClearItems();
		}
	}
}
namespace DevExpress.Xpf.Bars.Native {
	public class BarItemDefaultProperties : DependencyObject {
		#region static
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty LargeGlyphProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty CustomizationContentProperty;
		public static readonly DependencyProperty CustomizationGlyphProperty;
		public static readonly DependencyProperty DescriptionProperty;
		public static readonly DependencyProperty HintProperty;
		public static readonly DependencyProperty SuperTipProperty;
		public static readonly DependencyProperty HelpTextProperty;		
		static BarItemDefaultProperties() { 
			GlyphProperty = DependencyPropertyManager.RegisterAttached("Glyph", typeof(ImageSource), typeof(BarItemDefaultProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnGlyphPropertyChanged)));
			LargeGlyphProperty = DependencyPropertyManager.RegisterAttached("LargeGlyph", typeof(ImageSource), typeof(BarItemDefaultProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnLargeGlyphPropertyChanged)));
			ContentProperty = DependencyPropertyManager.RegisterAttached("Content", typeof(object), typeof(BarItemDefaultProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnContentPropertyChanged)));
			ContentTemplateProperty = DependencyPropertyManager.RegisterAttached("ContentTemplate", typeof(DataTemplate), typeof(BarItemDefaultProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnContentTemplatePropertyChanged)));
			CustomizationContentProperty = DependencyPropertyManager.RegisterAttached("CustomizationContent", typeof(object), typeof(BarItemDefaultProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnCustomizationContentPropertyChanged)));
			CustomizationGlyphProperty = DependencyPropertyManager.RegisterAttached("CustomizationGlyph", typeof(ImageSource), typeof(BarItemDefaultProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnCustomizationGlyphPropertyChanged)));
			DescriptionProperty = DependencyPropertyManager.RegisterAttached("Description", typeof(string), typeof(BarItemDefaultProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnDescriptionPropertyChanged)));
			HintProperty = DependencyPropertyManager.RegisterAttached("Hint", typeof(object), typeof(BarItemDefaultProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnHintPropertyChanged)));
			SuperTipProperty = DependencyPropertyManager.RegisterAttached("SuperTip", typeof(SuperTip), typeof(BarItemDefaultProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnSuperTipPropertyChanged)));
			HelpTextProperty = DependencyPropertyManager.RegisterAttached("HelpText", typeof(string), typeof(BarItemDefaultProperties), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => System.Windows.Automation.AutomationProperties.SetHelpText(d, e.NewValue as string))));
		}
		protected static void UpdateProperties(DependencyObject d) {
			BarItem item = d as BarItem;
			if(item != null)
				item.UpdateProperties();
		}
		protected static void OnGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is BarItem) {
				((BarItem)d).UpdateActualCustomizationGlyph();
				((BarItem)d).ExecuteActionOnLinkControls(lc => lc.OnSourceGlyphChanged());
			}
		}
		protected static void OnLargeGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is BarItem) {
				((BarItem)d).UpdateActualCustomizationLargeGlyph();
				((BarItem)d).ExecuteActionOnLinkControls(lc => lc.OnSourceLargeGlyphChanged());
			}
		}
		protected static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is BarItem) ((BarItem)d).OnSourceContentChanged();
		}
		protected static void OnCustomizationContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is BarItem) ((BarItem)d).UpdateActualCustomizationContent();
		}
		protected static void OnCustomizationGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is BarItem) ((BarItem)d).UpdateActualCustomizationGlyph();
		}
		protected static void OnContentTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is BarItem) {
				((BarItem)d).ExecuteActionOnLinks((l) => { if(l is BarItemLink) ((BarItemLink)l).UpdateActualContentTemplate(); });
				((BarItem)d).ExecuteActionOnLinkControls(lc => lc.OnSourceContentTemplateChanged());
			}
		}
		protected static void OnDescriptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is BarItem) ((BarItem)d).ExecuteActionOnLinkControls(lc => lc.OnSourceDescriptionChanged());
		}
		protected static void OnHintPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UpdateProperties(d);
		}
		protected static void OnSuperTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UpdateProperties(d);
		}
		#endregion
		public static string GetHelpText(DependencyObject obj) { return (string)obj.GetValue(HelpTextProperty); }
		public static void SetHelpText(DependencyObject obj, string value) { obj.SetValue(HelpTextProperty, value); }
		public static ImageSource GetGlyph(DependencyObject d) { return (ImageSource)d.GetValue(GlyphProperty); }
		public static void SetGlyph(DependencyObject d, ImageSource value) { d.SetValue(GlyphProperty, value); }
		public static ImageSource GetLargeGlyph(DependencyObject d) { return (ImageSource)d.GetValue(LargeGlyphProperty); }
		public static void SetLargeGlyph(DependencyObject d, ImageSource value) { d.SetValue(LargeGlyphProperty, value); }
		public static object GetContent(DependencyObject d) { return d.GetValue(ContentProperty); }
		public static void SetContent(DependencyObject d, object value) { d.SetValue(ContentProperty, value); }
		public static object GetCustomizationContent(DependencyObject d) { return d.GetValue(CustomizationContentProperty); }
		public static void SetCustomizationContent(DependencyObject d, object value) { d.SetValue(CustomizationContentProperty, value); }
		public static ImageSource GetCustomizationGlyph(DependencyObject d) { return (ImageSource)d.GetValue(CustomizationGlyphProperty); }
		public static void SetCustomizationGlyph(DependencyObject d, object value) { d.SetValue(CustomizationGlyphProperty, value); }
		public static DataTemplate GetContentTemplate(DependencyObject d) { return (DataTemplate)d.GetValue(ContentTemplateProperty); }
		public static void SetContentTemplate(DependencyObject d, DataTemplate value) { d.SetValue(ContentTemplateProperty, value); }
		public static string GetDescription(DependencyObject d) { return (string)d.GetValue(DescriptionProperty); }
		public static void SetDescription(DependencyObject d, string value) { d.SetValue(DescriptionProperty, value); }
		public static object GetHint(DependencyObject d) { return d.GetValue(HintProperty); }
		public static void SetHint(DependencyObject d, string value) { d.SetValue(HintProperty, value); }
		public static SuperTip GetSuperTip(DependencyObject d) { return (SuperTip)d.GetValue(SuperTipProperty); }
		public static void SetSuperTip(DependencyObject d, string value) { d.SetValue(SuperTipProperty, value); }
	}
}

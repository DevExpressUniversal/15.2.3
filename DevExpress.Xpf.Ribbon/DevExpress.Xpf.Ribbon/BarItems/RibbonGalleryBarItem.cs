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
using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Ribbon {
	public delegate void DropDownGalleryEventHandler(object sender, DropDownGalleryEventArgs e);
	[ContentProperty("DropDownMenuItems")]
	public class RibbonGalleryBarItem : BarItem, ILinksHolder, ILogicalChildrenContainer {
		#region static
		public static readonly DependencyProperty GalleryProperty;
		public static readonly DependencyProperty DropDownGalleryProperty;		
		public static readonly DependencyProperty DropDownGalleryMenuItemsGlyphSizeProperty;
		public static readonly DependencyProperty DropDownGalleryEnabledProperty;
		static object dropDownGalleryInitEventHandler;
		static object dropDownGalleryClosedEventHandler;
		static RibbonGalleryBarItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(RibbonGalleryBarItem), typeof(RibbonGalleryBarItemLink), delegate(object arg) { return new RibbonGalleryBarItemLink(); } );
			BarItemLinkControlCreator.Default.RegisterObject(typeof(RibbonGalleryBarItemLink), typeof(RibbonGalleryBarItemLinkControl), delegate(object arg) { return new RibbonGalleryBarItemLinkControl(); });
			GalleryProperty = DependencyPropertyManager.Register("Gallery", typeof(Gallery), typeof(RibbonGalleryBarItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGalleryPropertyChanged)));
			DropDownGalleryProperty = DependencyPropertyManager.Register("DropDownGallery", typeof(Gallery), typeof(RibbonGalleryBarItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDropDownGalleryPropertyChanged)));
			DropDownGalleryMenuItemsGlyphSizeProperty = DependencyPropertyManager.Register("DropDownGalleryMenuItemsGlyphSize", typeof(GlyphSize), typeof(RibbonGalleryBarItem),
				new FrameworkPropertyMetadata(GlyphSize.Default, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(DropDownGalleryMenuItemsGlyphSizePropertyChanged)));
			DropDownGalleryEnabledProperty = DependencyPropertyManager.Register("DropDownGalleryEnabled", typeof(bool), typeof(RibbonGalleryBarItem),
				new FrameworkPropertyMetadata(true, OnDropDownGalleryEnabledPropertyChanged));
			dropDownGalleryInitEventHandler = new object();
			dropDownGalleryClosedEventHandler = new object();
		}
		static void OnDropDownGalleryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((RibbonGalleryBarItem)obj).OnDropDownGalleryChanged(e.OldValue as Gallery);
		}
		static void DropDownGalleryMenuItemsGlyphSizePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((RibbonGalleryBarItem)obj).DropDownGalleryMenuItemsGlyphSizePropertyChanged();
		}
		static void OnGalleryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((RibbonGalleryBarItem)obj).OnGalleryChanged(e.OldValue as Gallery);
		}		
		protected static void OnDropDownGalleryEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonGalleryBarItem)d).OnDropDownGalleryEnabledChanged((bool)e.OldValue);
		}
		#endregion
		#region dep props
		public GlyphSize DropDownGalleryMenuItemsGlyphSize {
			get { return (GlyphSize)GetValue(DropDownGalleryMenuItemsGlyphSizeProperty); }
			set { SetValue(DropDownGalleryMenuItemsGlyphSizeProperty, value); }
		}
		public Gallery Gallery {
			get { return (Gallery)GetValue(GalleryProperty); }
			set { SetValue(GalleryProperty, value); }
		}
		public Gallery DropDownGallery {
			get { return (Gallery)GetValue(DropDownGalleryProperty); }
			set { SetValue(DropDownGalleryProperty, value); }
		}
		public bool DropDownGalleryEnabled {
			get { return (bool)GetValue(DropDownGalleryEnabledProperty); }
			set { SetValue(DropDownGalleryEnabledProperty, value); }
		}
		#endregion
		public RibbonGalleryBarItem() {
			ContextLayoutManagerHelper.AddLayoutUpdatedHandler(this, OnLayoutUpdated);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			immediateActionsManager.ExecuteActions();
		}
		CommonBarItemCollection dropDownMenuItemsCore;
		public CommonBarItemCollection DropDownMenuItems {
			get {
				if(dropDownMenuItemsCore == null)
					dropDownMenuItemsCore = new CommonBarItemCollection(this);
				return dropDownMenuItemsCore;
			}
		}
		BarItemLinkCollection dropDownMenuItemLinks;
		public BarItemLinkCollection DropDownMenuItemLinks {
			get {
				if(dropDownMenuItemLinks == null) dropDownMenuItemLinks = CreateDropDownMenuItemLinksCollection();
				return dropDownMenuItemLinks;
			}
		}
		protected virtual BarItemLinkCollection CreateDropDownMenuItemLinksCollection() {
			return new BarItemLinkCollection(this);	
		}
		ObservableCollection<ILinksHolder> mergedLinksHolders;
		protected ObservableCollection<ILinksHolder> MergedLinksHolders {
			get {
				if(mergedLinksHolders == null) {
					mergedLinksHolders = new ObservableCollection<ILinksHolder>();
					mergedLinksHolders.CollectionChanged += OnMergedLinksHoldersChanged;
				}
				return mergedLinksHolders;
			}
		}
		protected virtual void OnMergedLinksHoldersChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null)
			foreach (ILinksHolder value in e.OldItems) {
				value.MergedParent = null;
			}
			if (e.NewItems != null)
			foreach (ILinksHolder value in e.NewItems) {
				value.MergedParent = this;
			}
		}
		BarItemLinkCollection mergedLinks;
		protected virtual BarItemLinkCollection CreateMergedLinks() { return new MergedItemLinkCollection(this); }
		EventHandlerList events;
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		void RaiseEvent(object eventHandler, DropDownGalleryEventArgs args) {
			DropDownGalleryEventHandler h = Events[eventHandler] as DropDownGalleryEventHandler;
			if(h != null) h(this, args);
		}
		public event DropDownGalleryEventHandler DropDownGalleryInit {
			add { Events.AddHandler(dropDownGalleryInitEventHandler, value); }
			remove { Events.RemoveHandler(dropDownGalleryInitEventHandler, value); }
		}
		public event DropDownGalleryEventHandler DropDownGalleryClosed {
			add { Events.AddHandler(dropDownGalleryClosedEventHandler, value); }
			remove { Events.RemoveHandler(dropDownGalleryClosedEventHandler, value); }
		}
		protected internal virtual void OnDropDownGalleryInit(GalleryDropDownPopupMenu dropDownGalleryControl) {
			RaiseEvent(dropDownGalleryInitEventHandler, new DropDownGalleryEventArgs(dropDownGalleryControl));
		}
		protected internal virtual void OnDropDownGalleryClosed(GalleryDropDownPopupMenu dropDownGalleryControl) {
			RaiseEvent(dropDownGalleryClosedEventHandler, new DropDownGalleryEventArgs(dropDownGalleryControl));
		}
		protected virtual void DropDownGalleryMenuItemsGlyphSizePropertyChanged() {
			for(int i = 0; i < DropDownMenuItemLinks.Count; i++) {
				DropDownMenuItemLinks[i].UpdateLinkControlsActualGlyph();
			}
		}		
		protected virtual void OnGalleryChanged(Gallery oldValue) {
			if(oldValue != null) {
				RemoveLogicalChild(oldValue);
			}
			if(Gallery != null && Gallery.Parent==null) {
				AddLogicalChild(Gallery);
			}
			ExecuteActionOnBaseLinkControls((lc) => { if(lc is RibbonGalleryBarItemLinkControl) ((RibbonGalleryBarItemLinkControl)lc).OnSourceGalleryChanged(); });
		}
		protected virtual void OnDropDownGalleryChanged(Gallery oldGallery) {
			if(oldGallery != null) {
				RemoveLogicalChild(oldGallery);
			}
			if(DropDownGallery != null && DropDownGallery.Parent == null) {
				AddLogicalChild(DropDownGallery);
			}
		}
		protected virtual void OnDropDownGalleryEnabledChanged(bool oldValue) {
			ExecuteActionOnBaseLinkControls((lc) => { if(lc is RibbonGalleryBarItemLinkControl) ((RibbonGalleryBarItemLinkControl)lc).OnSourceDropDownGalleryEnabledChanged(); });
		}
		protected override IEnumerator LogicalChildren {
			get {
				return new MergedEnumerator(base.LogicalChildren, new SingleObjectEnumerator(Gallery), new SingleObjectEnumerator(DropDownGallery), logicalChildrenContainerItems.GetEnumerator());				
			}
		}		
		#region ILinksHolder Members
		bool ILinksHolder.ShowDescription { get { return false; } }
		IEnumerable ILinksHolder.ItemsSource { get { return null; } }
		public new BarItemLinkCollection Links {
			get { return DropDownMenuItemLinks; }
		}
		public BarItemLinkCollection MergedLinks {
			get {
				if(mergedLinks == null)
					mergedLinks = CreateMergedLinks();
				return mergedLinks;
			}
		}
		public BarItemLinkCollection ActualLinks {
			get { return ((ILinksHolder)this).IsMergedState ? ((ILinksHolder)this).MergedLinks : ((ILinksHolder)this).Links; }
		}
		public bool IsMergedState {
			get { return MergedLinksHolders.Count > 0; }
		}
		ILinksHolder mergedParent;
		ILinksHolder ILinksHolder.MergedParent {
			get { return mergedParent; }
			set { mergedParent = value; }
		}
		public void Merge(ILinksHolder holder) {
			if(!mergedLinksHolders.Contains(holder)) mergedLinksHolders.Add(holder);
		}
		public void UnMerge(ILinksHolder holder) {
			MergedLinksHolders.Remove(holder);
		}
		public void UnMerge() {
			MergedLinksHolders.Clear();
		}
		public GlyphSize ItemsGlyphSize {
			get { return DropDownGalleryMenuItemsGlyphSize; }
		}
		public GlyphSize GetDefaultItemsGlyphSize(LinkContainerType linkContainerType) {
			return BarManager.GetBarManager(this).If(x => x.MenuGlyphSize != GlyphSize.Default).Return(x => x.MenuGlyphSize, () => GlyphSize.Small);
		}
		public IEnumerator GetLogicalChildrenEnumerator() {
			return new BarItemLinksAsLogicalChildrenEnumerator(this);
		}
		public void OnLinkAdded(BarItemLinkBase link) {
			if(link.IsPrivate) return;
			AddLogicalChild(link);
		}
		public void OnLinkRemoved(BarItemLinkBase link) {
			if(link.IsPrivate) return;
			RemoveLogicalChild(link);
		}
		readonly ImmediateActionsManager immediateActionsManager = new ImmediateActionsManager();
		ImmediateActionsManager ILinksHolder.ImmediateActionsManager {
			get { return immediateActionsManager; }
		}
		LinksHolderType ILinksHolder.HolderType { get { return LinksHolderType.RibbonGalleryBarItem; } }
		CommonBarItemCollection ILinksHolder.Items { get { return DropDownMenuItems; } }
		BarItemLinkCollection ILinksHolder.Links { get { return Links; } }
		#endregion
		#region 
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
	public class DropDownGalleryEventArgs : EventArgs {
		public DropDownGalleryEventArgs(GalleryDropDownPopupMenu dropDownGallery) {
			DropDownGallery = dropDownGallery;
		}
		public GalleryDropDownPopupMenu DropDownGallery { get; set; }
	}
}

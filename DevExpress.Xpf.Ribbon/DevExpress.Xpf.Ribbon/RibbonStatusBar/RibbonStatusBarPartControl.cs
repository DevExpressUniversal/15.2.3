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

using DevExpress.Xpf.Bars;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using System;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonStatusBarPartControlBase : RibbonLinksControl, ILinksHolder, ILogicalChildrenContainer {
		static RibbonStatusBarPartControlBase() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(RibbonStatusBarPartControlBase), typeof(RibbonStatusBarPartControlAutomationPeer), owner => new RibbonStatusBarPartControlAutomationPeer((RibbonStatusBarPartControlBase)owner));
			NameProperty.OverrideMetadata(typeof(RibbonStatusBarPartControlBase), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnNamePropertyChanged)));			
		}
		protected static void OnNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonStatusBarPartControlBase)d).OnNameChanged(e.NewValue as string, e.OldValue as string);
		}		
		protected override NavigationManager CreateNavigationManager() {
			return null;
		}
		public RibbonStatusBarPartControlBase(ILogicalChildrenContainer owner) {
			Owner = owner;
		}				
		protected virtual void OnNameChanged(string newValue, string oldValue) {
		}				
		protected ILogicalChildrenContainer Owner { get; set; }
		protected override Size MeasureOverride(Size constraint) {
			bool skip = true;
			foreach(UIElement element in Items) { skip &= element.IsMeasureValid; }
			if(!skip) {
				FrameworkElement child = VisualTreeHelper.GetChildrenCount(this) != 0 ? VisualTreeHelper.GetChild(this, 0) as FrameworkElement : null;
				if(child != null)
					child.InvalidateMeasure();
				FrameworkElement parent = VisualTreeHelper.GetParent(this) as FrameworkElement;
				if(parent != null)
					parent.InvalidateMeasure();
			}
			return base.MeasureOverride(constraint);
		}
		CommonBarItemCollection commonItemsCore;
		public CommonBarItemCollection CommonItems {
			get {
				if(commonItemsCore == null)
					commonItemsCore = new CommonBarItemCollection(this);
				return commonItemsCore;
			}
		}
		BarItemLinkCollection itemLinks;
		public override BarItemLinkCollection ItemLinks {
			get {
				if (itemLinks == null) {
					itemLinks = CreateItemLinksCollection();
					RecreateItemsSource();
				}					
				return itemLinks;
			}
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override bool OpenPopupsAsMenu { get { return false; } }
		protected override void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			foreach(IBarItem item in CommonItems) {
				var barItem = item as BarItem;
				if(barItem != null)
					barItem.CoerceValue(BarItem.IsEnabledProperty);
			}
			base.OnIsEnabledChanged(sender, e);
		}
		ObservableCollection<ILinksHolder> mergedLinksHolders;
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
			BarItemLinkMergeHelper helper = new BarItemLinkMergeHelper();
			helper.Merge(((ILinksHolder)this).Links, MergedLinksHolders, ((ILinksHolder)this).MergedLinks);
			if (e.OldItems != null)
			foreach (ILinksHolder value in e.OldItems) {
				value.MergedParent = null;
			}
			if (e.NewItems != null)
			foreach (ILinksHolder value in e.NewItems) {
				value.MergedParent = this;
			}
			RecreateItemsSource();
		}
		protected virtual BarItemLinkCollection CreateItemLinksCollection() {
			BarItemLinkCollection coll = new BarItemLinkCollection(this);
			coll.CollectionChanged += new NotifyCollectionChangedEventHandler(OnItemLinksCollectionChanged);			
			return coll;
		}
		void OnItemLinksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			BarManagerHelper.UpdateSeparatorsVisibility(this);
		}
		#region ILinksHolder Members
		bool ILinksHolder.ShowDescription { get { return false; } }
		BarItemLinkCollection ILinksHolder.Links {
			get { return ItemLinks; }
		}
		ILinksHolder mergedParent;
		ILinksHolder ILinksHolder.MergedParent {
			get { return mergedParent; }
			set { mergedParent = value; }
		}
		void ILinksHolder.Merge(ILinksHolder holder) {
			MergeCore(holder);
		}		
		void ILinksHolder.UnMerge(ILinksHolder holder) {
			UnMergeCore(holder);
		}		
		void ILinksHolder.UnMerge() {
			MergedLinksHolders.Clear();
		}
		protected virtual void MergeCore(ILinksHolder holder) {
			if (MergedLinksHolders.Contains(holder))
				return;
			MergedLinksHolders.Add(holder);
		}
		protected virtual void UnMergeCore(ILinksHolder holder) {
			MergedLinksHolders.Remove(holder);
		}
		public virtual LinksHolderType HolderType { get { return LinksHolderType.None; } }
		BarItemLinkCollection mergedLinks;
		protected virtual BarItemLinkCollection CreateMergedLinks() { return new MergedItemLinkCollection(this); }
		BarItemLinkCollection ILinksHolder.MergedLinks {
			get {
				if(mergedLinks == null)
					mergedLinks = CreateMergedLinks();
				return mergedLinks;
			}
		}
		bool ILinksHolder.IsMergedState { get { return MergedLinksHolders.Count > 0; } }
		readonly ImmediateActionsManager immediateActionsManager = new ImmediateActionsManager();
		ImmediateActionsManager ILinksHolder.ImmediateActionsManager {
			get { return immediateActionsManager; }
		}
		BarItemLinkCollection ILinksHolder.ActualLinks {
			get { return ((ILinksHolder)this).IsMergedState ? ((ILinksHolder)this).MergedLinks : ((ILinksHolder)this).Links; }
		}
		CommonBarItemCollection ILinksHolder.Items { get { return CommonItems; } }
		GlyphSize ILinksHolder.ItemsGlyphSize {
			get { return GlyphSize.Default; }
		}
		GlyphSize ILinksHolder.GetDefaultItemsGlyphSize(LinkContainerType linkContainerType) {
			return GlyphSize.Default;
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				return ((ILinksHolder)this).GetLogicalChildrenEnumerator();
			}
		}
		System.Collections.IEnumerator ILinksHolder.GetLogicalChildrenEnumerator() {
			return logicalChildrenContainerItems.GetEnumerator();
		}
		void ILinksHolder.OnLinkAdded(BarItemLinkBase link) {
			if(link.IsPrivate)
				return;
			AddLogicalChild(link);
		}
		void ILinksHolder.OnLinkRemoved(BarItemLinkBase link) {
			if(link.IsPrivate)
				return;
			RemoveLogicalChild(link);
		}
		#endregion
		protected internal void RecreateItemsSource() {
			BarItemLinkInfoCollection oldValue = ItemsSource as BarItemLinkInfoCollection;
			ItemsSource = new BarItemLinkInfoCollection(((ILinksHolder)this).ActualLinks);
			if(oldValue != null)
				oldValue.Source = null;
			CalculateMaxGlyphSize();
		}
		protected override void OnLayoutUpdated(object sender, EventArgs e) {
			base.OnLayoutUpdated(sender, e);
			immediateActionsManager.ExecuteActions();
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			if (Manager != null) return;
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			base.OnUnloaded(sender, e);
		}
		readonly List<object> logicalChildrenContainerItems = new List<object>();
		void ILogicalChildrenContainer.AddLogicalChild(object child) {
			logicalChildrenContainerItems.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalChildrenContainer.RemoveLogicalChild(object child) {
			logicalChildrenContainerItems.Remove(child);
			RemoveLogicalChild(child);
		}
		#region IMultipleElementRegistratorSupport Members
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { return new object[] { typeof(ILinksHolder), typeof(IFrameworkInputElement) }; }
		}
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			if (Equals(registratorKey, typeof(IFrameworkInputElement)))
				return Name;
			if (Equals(registratorKey, typeof(ILinksHolder)))
				return Name;
			throw new ArgumentException();
		}
		#endregion
	}
}

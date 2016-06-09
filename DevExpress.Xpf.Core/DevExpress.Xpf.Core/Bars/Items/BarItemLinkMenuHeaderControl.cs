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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using System.Collections.Specialized;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using System.Windows.Markup;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Bars {
	[ContentProperty("Items")]
	public class BarItemMenuHeader : BarItem, ILinksHolder, ILogicalChildrenContainer {
		#region ItemsAttachedBehavior
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemsAttachedBehaviorProperty =
			DependencyPropertyManager.RegisterAttached("ItemsAttachedBehaviorProperty", typeof(ItemsAttachedBehaviorCore<BarItemMenuHeader, BarItem>), typeof(BarItemMenuHeader), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty ItemTemplateSelectorProperty =
			DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(BarItemMenuHeader), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplateSelectorPropertyChanged)));
		public static readonly DependencyProperty ItemStyleProperty =
			DependencyProperty.Register("ItemStyle", typeof(Style), typeof(BarItemMenuHeader), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemStylePropertyChanged)));
		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(BarItemMenuHeader), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(BarItemMenuHeader), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemsSourcePropertyChanged)));
		protected static void OnItemTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarItemMenuHeader)d).UpdateItemsAttachedBehavior(e);
		}
		protected static void OnItemStylePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarItemMenuHeader)d).UpdateItemsAttachedBehavior(e);
		}
		protected static void OnItemTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarItemMenuHeader)d).UpdateItemsAttachedBehavior(e);
		}
		protected static void OnItemsSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((BarItemMenuHeader)d).OnItemsSourceChanged(e);
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemMenuHeaderItemsSource")]
#endif
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemMenuHeaderItemTemplate")]
#endif
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemMenuHeaderItemStyle")]
#endif
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemMenuHeaderItemTemplateSelector")]
#endif
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		protected virtual void OnItemsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarItemMenuHeader, BarItem>.OnItemsSourcePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty,
				ItemTemplateProperty,
				ItemTemplateSelectorProperty,
				null,
				bar => bar.Items,
				bar => new BarButtonItem(),
				(index, item) => {
					if (index < 0 || index >= ItemLinks.Count)
						Items.Add(item as BarItem);
					else
						Items.Insert(index, item as BarItem);
				}, useDefaultTemplateSelector: true);
		}
		protected virtual void UpdateItemsAttachedBehavior(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<BarItemMenuHeader, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty);
		}
		#endregion
		#region static
		public static readonly DependencyProperty MinColCountProperty = DependencyPropertyManager.Register("MinColCount", typeof(int), typeof(BarItemMenuHeader), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnMinColCountPropertyChanged)));
		public static readonly DependencyProperty ItemsOrientationProperty = DependencyPropertyManager.Register("ItemsOrientation", typeof(HeaderOrientation), typeof(BarItemMenuHeader), new FrameworkPropertyMetadata(HeaderOrientation.Default, new PropertyChangedCallback(OnItemsOrientationPropertyChanged)));
		static BarItemMenuHeader() {
			BarItemLinkCreator.Default.RegisterObject(typeof(BarItemMenuHeader), typeof(BarItemLinkMenuHeader), delegate(object arg) { return new BarItemLinkMenuHeader(); });
			BarItemLinkControlCreator.Default.RegisterObject(typeof(BarItemLinkMenuHeader), typeof(BarItemLinkMenuHeaderControl), delegate(object arg) { return new BarItemLinkMenuHeaderControl(); });
		}
		protected static void OnMinColCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemMenuHeader)d).ExecuteActionOnLinkControls(lControl => ((BarItemLinkMenuHeaderControl)lControl).UpdateActualMinColCount());
		}
		protected static void OnItemsOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemMenuHeader)d).ExecuteActionOnLinkControls(lControl => ((BarItemLinkMenuHeaderControl)lControl).UpdateActualItemsOrientation());
		}		
		#endregion
		public BarItemMenuHeader() {
			ContextLayoutManagerHelper.AddLayoutUpdatedHandler(this, OnLayoutUpdated);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			immediateActionsManager.ExecuteActions();
		}
		BarItemLinkCollection itemLinks;
		BarItemLinkCollection mergedLinks;
		ObservableCollection<ILinksHolder> mergedLinksHolders;
		CommonBarItemCollection itemsCore;
		public CommonBarItemCollection Items {
			get {
				if(itemsCore == null)
					itemsCore = new CommonBarItemCollection(this);
				return itemsCore;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemMenuHeaderItemLinks")]
#endif
		public BarItemLinkCollection ItemLinks {
			get {
				if(itemLinks == null)
					itemLinks = CreateItemLinksCollection();
				return itemLinks;
			}
		}
		public HeaderOrientation ItemsOrientation {
			get { return (HeaderOrientation)GetValue(ItemsOrientationProperty); }
			set { SetValue(ItemsOrientationProperty, value); }
		}
		public int MinColCount {
			get { return (int)GetValue(MinColCountProperty); }
			set { SetValue(MinColCountProperty, value); }
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
			if (e.OldItems != null)
				foreach (ILinksHolder element in e.OldItems) {
					element.MergedParent = null;
				}
			if (e.NewItems != null)
				foreach (ILinksHolder element in e.NewItems) {
					element.MergedParent = this;
				}
		}
		protected virtual BarItemLinkCollection CreateItemLinksCollection() {
			return new BarItemLinkCollection(this);
		}
		protected override IEnumerator LogicalChildren {
			get {
				return ((ILinksHolder)this).GetLogicalChildrenEnumerator();
			}
		}				
		protected internal virtual void UpdateLinkControlsIsEmpty() {
			ExecuteActionOnLinkControls(lControl => ((BarItemLinkMenuHeaderControl)lControl).UpdateActualIsEmpty());
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
			return logicalChildrenContainerItems.GetEnumerator();
		}
		void ILinksHolder.OnLinkAdded(BarItemLinkBase link) {
			if (LogicalTreeHelper.GetParent(link) == null)
				AddLogicalChild(link);
			UpdateLinkControlsIsEmpty();
		}
		void ILinksHolder.OnLinkRemoved(BarItemLinkBase link) {			
			RemoveLogicalChild(link);
			UpdateLinkControlsIsEmpty();
		}
		LinksHolderType ILinksHolder.HolderType { get { return LinksHolderType.BarItemMenuHeader; } }
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
	public class BarItemLinkMenuHeaderControl : BarItemLinkControl {
		#region static
		public static readonly DependencyProperty ColumnIndentProperty;
		public static readonly DependencyProperty RowIndentProperty;
		protected static readonly DependencyPropertyKey RowPositionPropertyKey;
		protected static readonly DependencyPropertyKey ColumnPositionPropertyKey;
		public static readonly DependencyProperty ColumnPositionProperty;
		public static readonly DependencyProperty RowPositionProperty;
		protected static readonly DependencyPropertyKey ActualMinColCountPropertyKey;
		public static readonly DependencyProperty ActualMinColCountProperty;
		protected static readonly DependencyPropertyKey ActualItemsOrientationPropertyKey;
		public static readonly DependencyProperty ActualItemsOrientationProperty;
		public static readonly DependencyProperty IsEmptyProperty;
		protected internal static readonly DependencyPropertyKey IsEmptyPropertyKey;
		public static readonly DependencyProperty PrecedesHeaderProperty;								
		static BarItemLinkMenuHeaderControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarItemLinkMenuHeaderControl), new FrameworkPropertyMetadata(typeof(BarItemLinkMenuHeaderControl)));
			ColumnIndentProperty = DependencyPropertyManager.Register("ColumnIndent", typeof(double), typeof(BarItemLinkMenuHeaderControl), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
			RowIndentProperty = DependencyPropertyManager.Register("RowIndent", typeof(double), typeof(BarItemLinkMenuHeaderControl), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
			PrecedesHeaderProperty = DependencyPropertyManager.Register("PrecedesHeader", typeof(bool), typeof(BarItemLinkMenuHeaderControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnPrecedesHeaderPropertyChanged)));
			RowPositionPropertyKey = DependencyPropertyManager.RegisterReadOnly("RowPosition", typeof(VerticalAlignment), typeof(BarItemLinkMenuHeaderControl), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, new PropertyChangedCallback(OnRowPositionPropertyChanged)));
			ColumnPositionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ColumnPosition", typeof(HorizontalAlignment), typeof(BarItemLinkMenuHeaderControl), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, new PropertyChangedCallback(OnColumnPositionPropertyChanged)));
			IsEmptyPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsEmpty", typeof(bool), typeof(BarItemLinkMenuHeaderControl), new FrameworkPropertyMetadata(true, OnIsEmptyPropertyChanged));
			ActualMinColCountPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualMinColCount", typeof(int), typeof(BarItemLinkMenuHeaderControl), new FrameworkPropertyMetadata(1));
			ActualItemsOrientationPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemsOrientation", typeof(HeaderOrientation), typeof(BarItemLinkMenuHeaderControl), new FrameworkPropertyMetadata(HeaderOrientation.Default, (o, e) => ((BarItemLinkMenuHeaderControl)o).UpdateItemsControlItemsOrientation()));
			ColumnPositionProperty = ColumnPositionPropertyKey.DependencyProperty;
			RowPositionProperty = RowPositionPropertyKey.DependencyProperty;
			ActualMinColCountProperty = ActualMinColCountPropertyKey.DependencyProperty;
			ActualItemsOrientationProperty = ActualItemsOrientationPropertyKey.DependencyProperty;
			IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;
		}
		protected static void OnRowPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkMenuHeaderControl)d).OnRowPositionChanged((VerticalAlignment)e.OldValue);
		}
		protected static void OnColumnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkMenuHeaderControl)d).OnColumnPositionChanged((HorizontalAlignment)e.OldValue);
		}
		protected static void OnPrecedesHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkMenuHeaderControl)d).OnPrecedesHeaderChanged((bool)e.OldValue);
		}
		protected static void OnIsEmptyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkMenuHeaderControl)d).OnIsEmptyChanged((bool)e.OldValue);
		}
		#endregion
		#region dep props
		public double RowIndent {
			get { return (double)GetValue(RowIndentProperty); }
			set { SetValue(RowIndentProperty, value); }
		}
		public double ColumnIndent {
			get { return (double)GetValue(ColumnIndentProperty); }
			set { SetValue(ColumnIndentProperty, value); }
		}
		public HorizontalAlignment ColumnPosition {
			get { return (HorizontalAlignment)GetValue(ColumnPositionProperty); }
			protected internal set { this.SetValue(ColumnPositionPropertyKey, value); }
		}
		public VerticalAlignment RowPosition {
			get { return (VerticalAlignment)GetValue(RowPositionProperty); }
			protected internal set { this.SetValue(RowPositionPropertyKey, value); }
		}
		public bool IsEmpty {
			get { return (bool)GetValue(IsEmptyProperty); }
			protected internal set { this.SetValue(IsEmptyPropertyKey, value); }
		}
		public bool PrecedesHeader {
			get { return (bool)GetValue(PrecedesHeaderProperty); }
			set { SetValue(PrecedesHeaderProperty, value); }
		}
		public HeaderOrientation ActualItemsOrientation {
			get { return (HeaderOrientation)GetValue(ActualItemsOrientationProperty); }
			protected internal set { this.SetValue(ActualItemsOrientationPropertyKey, value); }
		}
		public int ActualMinColCount {
			get { return (int)GetValue(ActualMinColCountProperty); }
			protected internal set { this.SetValue(ActualMinColCountPropertyKey, value); }
		}
		#endregion
		public BarItemLinkMenuHeaderControl() {
		}		
		protected override DataTemplate GetContentTemplate() {
			if(MenuHeaderLink != null && MenuHeaderLink.contentTemplateWeakReference != null)
				return MenuHeaderLink.contentTemplateWeakReference.Target as DataTemplate;
			return base.GetContentTemplate();
		}
		public BarItemLinkMenuHeaderControl(BarItemLinkMenuHeader link)
			: base(link) {
		}
		protected virtual BarItemLinkMenuHeaderControlStatesResourceHolder StatesHolder {
			get { return LinksControl as SubMenuBarControl == null ? null : ((SubMenuBarControl)LinksControl).MenuHeaderStatesHolder; }
		}
		protected internal BarItemMenuHeaderItemsControl ItemsControl { get; set; }
		protected internal GlyphSideControl GlyphSide { get; set; }
		protected internal BarItemLinkMenuHeaderContentControl HeaderContentControl { get; set; }
		public BarItemLinkMenuHeader MenuHeaderLink { get { return LinkBase as BarItemLinkMenuHeader; } }
		public BarItemMenuHeader MenuHeader { get { return Item as BarItemMenuHeader; } }
		protected internal virtual void OnColumnPositionChanged(HorizontalAlignment oldValue) {
			UpdateVisualStates();
		}
		protected virtual void OnPrecedesHeaderChanged(bool oldValue) {
			UpdateVisualStates();
		}
		protected internal virtual void OnRowPositionChanged(VerticalAlignment oldValue) {
			UpdateVisualStates();
		}
		protected internal virtual void OnIsEmptyChanged(bool oldValue) {
			UpdateVisualStates();
		}
		protected internal override void UpdateStyleByContainerType(LinkContainerType type) { }
		protected internal override void UpdateTemplateByContainerType(LinkContainerType type) { }
		protected internal override void UpdateActualProperties() {
			base.UpdateActualProperties();
			UpdateActualMinColCount();
			UpdateActualItemsOrientation();
			UpdateActualIsEmpty();
		}
		protected internal virtual void UpdateItemsControlSource() {
			if(ItemsControl != null) ItemsControl.UpdateItemsSource();
		}
		protected internal virtual void UpdateItemsControlItemsOrientation() {
			UpdateVisualStates();
			if(ItemsControl != null) {
				ItemsControl.HorizontalItems = ActualItemsOrientation == HeaderOrientation.Horizontal;
			}
		}
		protected internal virtual void UpdateVisualStates() {
			if(StatesHolder == null) return;
			if(HeaderContentControl != null) {
				CornerRadius newCornerRadius = StatesHolder.GetCornerRadius(ColumnPosition, RowPosition, !IsEmpty, ActualItemsOrientation, PrecedesHeader);
				Thickness newThickness = StatesHolder.GetContentBorderThickness(ColumnPosition, RowPosition, !IsEmpty, ActualItemsOrientation, PrecedesHeader);
				if(!HeaderContentControl.ContentCornerRadius.Equals(newCornerRadius))
					HeaderContentControl.ContentCornerRadius = newCornerRadius;
				if(!HeaderContentControl.ContentBorderThickness.Equals(newThickness))
					HeaderContentControl.ContentBorderThickness = newThickness;
			}
			if(ItemsControl != null) {
				Thickness newThickness = StatesHolder.GetItemsBorderThickness(ColumnPosition, RowPosition, !IsEmpty, ActualItemsOrientation, PrecedesHeader);
				Thickness newMargin = StatesHolder.GetItemsPresenterMargin(ColumnPosition, RowPosition, !IsEmpty, ActualItemsOrientation, PrecedesHeader);
				if(!ItemsControl.ItemsBorderThickness.Equals(newThickness))
					ItemsControl.ItemsBorderThickness = newThickness;
				if(!ItemsControl.ItemsPresenterThickness.Equals(newMargin))
					ItemsControl.ItemsPresenterThickness = newMargin;
				ItemsControl.ItemsBorderVisibility = StatesHolder.GetItemsBorderVisibility(ColumnPosition, RowPosition, !IsEmpty, ActualItemsOrientation, PrecedesHeader);
			}
		}		
		protected internal virtual void UpdateActualItemsOrientation() {
			if(MenuHeaderLink != null && MenuHeaderLink.ItemsOrientation != HeaderOrientation.Default) {
				ActualItemsOrientation = MenuHeaderLink.ItemsOrientation;
				return;
			}
			if(MenuHeader != null && MenuHeader.ItemsOrientation != HeaderOrientation.Default) {
				ActualItemsOrientation = MenuHeader.ItemsOrientation;
				return;
			}
			ActualItemsOrientation = HeaderOrientation.Vertical;
		}
		protected internal virtual void UpdateActualMinColCount() {
			if(MenuHeaderLink != null && MenuHeaderLink.MinColCount != -1) {
				ActualMinColCount = MenuHeaderLink.MinColCount;
				return;
			}
			if(MenuHeader != null && MenuHeader.MinColCount != -1) {
				ActualMinColCount = MenuHeader.MinColCount;
				return;
			}
			ActualMinColCount = 1;
		}
		protected internal virtual void UpdateActualIsEmpty() {
			this.IsEmpty = Item == null ? true : (((ILinksHolder)Item).Links.Count == 0);
		}
		protected internal virtual bool HasHorizontalItemsWithLargeGlyph() {
			if(ItemsControl == null) return false;
			foreach(BarItemLinkInfo info in ItemsControl.Items) {
				BarItemLinkControl lc = info.LinkControl as BarItemLinkControl;
				if(lc == null || lc.LinkBase == null || !lc.LinkBase.CalculateVisibility(info))
					continue;
				if(lc.IsLargeGlyph)
					return true;
			}
			return false;
		}
		protected override bool ShouldHighlightItem() {
			return false;
		}
		public override void OnApplyTemplate() {
			if(ItemsControl != null) ItemsControl.LinkControl = null;
			base.OnApplyTemplate();
			ItemsControl = (BarItemMenuHeaderItemsControl)GetTemplateChild("PART_ItemsControl");
			if(ItemsControl != null) ItemsControl.LinkControl = this;
			UpdateItemsControlItemsOrientation();
			GlyphSide = (GlyphSideControl)GetTemplateChild("PART_GlyphSideControl");
			HeaderContentControl = (BarItemLinkMenuHeaderContentControl)GetTemplateChild("PART_Content");
			Dispatcher.BeginInvoke(new Action(UpdateVisualStates));
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateVisualStates();
		}
		protected override void OnLinkInfoChanged(BarItemLinkInfo oldValue) {
			base.OnLinkInfoChanged(oldValue);
			ClearMenuHeader();
			CheckMenuHeaders();
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected bool IsItemFake { get; private set; }
		void ClearMenuHeader() {
			if(!IsItemFake || Item == null)
				return;
			var item = (BarItemMenuHeader)Item;
			item.ItemLinks.ForEach(link => {
				if(BarItemLinkMenuHeaderHorizontalHelper.GetIsFakeLink(link)) {
					BarItemLinkMenuHeaderHorizontalHelper.SetIsFakeLink(link, false);
					link.IsVisible = true;
				}
			});
			IsItemFake = false;
		}
		void CheckMenuHeaders() {
			if(Item != null)
				return;
			var item = new BarItemMenuHeader();
			IsItemFake = true;
			int linkIndex = Link.Links.IndexOf(Link) + 1;
			for(int i = linkIndex; i < Link.Links.Count; i++) {
				var link = Link.Links[i] as BarItemLink;
				if(link is BarItemLinkMenuHeader)
					break;
				var clonedLink = ((ICloneable)Link.Links[i]).Clone() as BarItemLinkBase;
				item.ItemLinks.Add(clonedLink);
				BarItemLinkMenuHeaderHorizontalHelper.SetIsFakeLink(link, true);
				link.IsVisible = false;
		}
			Link.Link(item);
		}
 		protected internal override INavigationOwner GetBoundOwner() {
			return this.ItemsControl;
		}
	}
	public class BarItemMenuHeaderItemsControl : LinksControl {
		#region static
		public static readonly DependencyProperty HorizontalItemsProperty;
		public static readonly DependencyProperty HorizontalGlyphPaddingProperty;
		public static readonly DependencyProperty ColumnIndentProperty;
		public static readonly DependencyProperty RowIndentProperty;
		public static readonly DependencyProperty ItemsBorderThicknessProperty;		
		public static readonly DependencyProperty ItemsBorderVisibilityProperty;				
		public static readonly DependencyProperty ItemsPresenterThicknessProperty;   
		static BarItemMenuHeaderItemsControl() {
			HorizontalItemsProperty = DependencyPropertyManager.Register("HorizontalItems", typeof(bool), typeof(BarItemMenuHeaderItemsControl), new FrameworkPropertyMetadata(false, (o, e) => OnHorizontalItemsChanged(o, e)));
			HorizontalGlyphPaddingProperty = DependencyPropertyManager.Register("HorizontalGlyphPadding", typeof(Thickness), typeof(BarItemMenuHeaderItemsControl), new FrameworkPropertyMetadata(new Thickness()));
			ColumnIndentProperty = DependencyPropertyManager.Register("ColumnIndent", typeof(double), typeof(BarItemMenuHeaderItemsControl), new FrameworkPropertyMetadata(0d));
			RowIndentProperty = DependencyPropertyManager.Register("RowIndent", typeof(double), typeof(BarItemMenuHeaderItemsControl), new FrameworkPropertyMetadata(0d));
			ItemsBorderThicknessProperty = DependencyPropertyManager.Register("ItemsBorderThickness", typeof(Thickness), typeof(BarItemMenuHeaderItemsControl), new FrameworkPropertyMetadata(new Thickness()));
			ItemsPresenterThicknessProperty = DependencyPropertyManager.Register("ItemsPresenterThickness", typeof(Thickness), typeof(BarItemMenuHeaderItemsControl), new FrameworkPropertyMetadata(new Thickness()));
			ItemsBorderVisibilityProperty = DependencyPropertyManager.Register("ItemsBorderVisibility", typeof(Visibility), typeof(BarItemMenuHeaderItemsControl), new FrameworkPropertyMetadata(Visibility.Visible));
		}
		static void OnHorizontalItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((BarItemMenuHeaderItemsControl)o).ContainerType = ((BarItemMenuHeaderItemsControl)o).HorizontalItems ? LinkContainerType.MenuHeader : LinkContainerType.Menu;
			((BarItemMenuHeaderItemsControl)o).UpdateGlyphPadding();
		}
		#endregion
		public BarItemMenuHeaderItemsControl() {
			ContainerType = LinkContainerType.Menu;
			DefaultStyleKey = typeof(BarItemMenuHeaderItemsControl);
		}
		public Thickness ItemsPresenterThickness {
			get { return (Thickness)GetValue(ItemsPresenterThicknessProperty); }
			set { SetValue(ItemsPresenterThicknessProperty, value); }
		}
		public Visibility ItemsBorderVisibility {
			get { return (Visibility)GetValue(ItemsBorderVisibilityProperty); }
			set { SetValue(ItemsBorderVisibilityProperty, value); }
		}
		public Thickness ItemsBorderThickness {
			get { return (Thickness)GetValue(ItemsBorderThicknessProperty); }
			set { SetValue(ItemsBorderThicknessProperty, value); }
		}			 
		public bool HorizontalItems {
			get { return (bool)GetValue(HorizontalItemsProperty); }
			set { SetValue(HorizontalItemsProperty, value); }
		}
		public double RowIndent {
			get { return (double)GetValue(RowIndentProperty); }
			set { SetValue(RowIndentProperty, value); }
		}
		public double ColumnIndent {
			get { return (double)GetValue(ColumnIndentProperty); }
			set { SetValue(ColumnIndentProperty, value); }
		}
		public Thickness HorizontalGlyphPadding {
			get { return (Thickness)GetValue(HorizontalGlyphPaddingProperty); }
			set { SetValue(HorizontalGlyphPaddingProperty, value); }
		}
		BarItemLinkMenuHeaderControl linkControl;
		public BarItemLinkMenuHeaderControl MenuHeaderControl { get { return LayoutHelper.FindParentObject<BarItemLinkMenuHeaderControl>(this); } }
		public BarItemMenuHeader MenuHeader { get { return LinkControl == null ? null : LinkControl.MenuHeader; } }
		protected internal override bool OpenPopupsAsMenu { get { return false; } }
		protected internal BarItemLinkMenuHeaderControl LinkControl {
			get { return linkControl; }
			set {
				if(LinkControl == value)
					return;
				linkControl = value;
				OnLinkControlChanged();
			}
		}
		public override BarItemLinkCollection ItemLinks {
			get { return MenuHeader == null ? null : ((ILinksHolder)MenuHeader).ActualLinks; }
		}
		internal int rowsCount = 0;
		internal int columnsCount = 0;		
		protected internal override void OnItemClick(BarItemLinkControl linkControl) {
			base.OnItemClick(linkControl);
			if(MenuHeaderControl.LinksControl != null)
				MenuHeaderControl.LinksControl.OnItemClick(linkControl);
		}
		protected virtual void OnLinkControlChanged() {
			UpdateGlyphPadding();
			if(ItemsSource is BarItemLinkInfoCollection) {
				((BarItemLinkInfoCollection)ItemsSource).Source.Clear();
			}
			UpdateItemsSource();
		}
		protected virtual void UpdateGlyphPadding() {
			if(HorizontalItems) {
				GlyphPadding = HorizontalGlyphPadding;
			} else {
				GlyphPadding = LinkControl.LinksControl.GlyphPadding;
			}
		}
		protected internal void UpdateItemsSource() {
			BarItemLinkInfoCollection oldValue = ItemsSource as BarItemLinkInfoCollection;
			ItemsSource = new BarItemLinkInfoCollection(ItemLinks);
			if(oldValue != null)
				oldValue.Source = null;
		}
		#region INavigationOwner
		protected override bool GetCanEnterMenuMode() { return false; }
		protected override IBarsNavigationSupport GetNavigationParent() { return LayoutHelper.FindLayoutOrVisualParentObject(Parent, x => x is IBarsNavigationSupport, true) as IBarsNavigationSupport; }
		protected override Orientation GetNavigationOrientation() { return Orientation.Horizontal; }
		protected override NavigationKeys GetNavigationKeys() { return NavigationKeys.Arrows | NavigationKeys.Tab; }
		protected override KeyboardNavigationMode GetNavigationMode() { return KeyboardNavigationMode.Continue; }		
		protected override INavigationElement GetBoundElement() {
			return LinkControl.With(x => x.LinkInfo);
		}
		protected override int GetNavigationID() {
			return ((object)MenuHeaderControl.With(x => x.MenuHeader) ?? (object)MenuHeaderControl ?? (object)this).GetHashCode();
		}
		#endregion
	}
	public class BarItemLinkMenuHeaderItemsPanel : Panel {
		#region static
		public static readonly DependencyProperty HorizontalItemsProperty;
		static BarItemLinkMenuHeaderItemsPanel() {
			HorizontalItemsProperty = DependencyPropertyManager.Register("HorizontalItems", typeof(bool), typeof(BarItemLinkMenuHeaderItemsPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
		}
		#endregion
		const bool StretchRowsDefaultValue = true;
		BarItemLinkMenuHeaderControl headerControl = null;
		protected BarItemLinkMenuHeaderControl HeaderControl {
			get {
				if(headerControl == null)
					headerControl = LayoutHelper.FindParentObject<BarItemLinkMenuHeaderControl>(this);
				return headerControl;
			}
		}
		protected BarItemMenuHeaderItemsControl ItemsControl { get { return HeaderControl == null ? null : HeaderControl.ItemsControl; } }
		public bool HorizontalItems {
			get { return (bool)GetValue(HorizontalItemsProperty); }
			set { SetValue(HorizontalItemsProperty, value); }
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(HorizontalItems)
				return MeasureHorizontal(availableSize);
			return MeasureVertical(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(HorizontalItems)
				return ArrangeHorizontal(finalSize);
			return ArrangeVertical(finalSize);
		}
		#region horizontal layout
		Size MeasureHorizontal(Size availableSize) {
			List<BarItemLinkInfo> validChildren = new List<BarItemLinkInfo>();
			GetValidChildrenHorizontal(validChildren, null);
			if(Double.IsInfinity(availableSize.Width)) {
				double desiredWidth = 0d;
				double desiredHeight = 0d;
				for(int i = 0; i < validChildren.Count; i++) {
					validChildren[i].Measure(SizeHelper.Infinite);
					desiredWidth += validChildren[i].DesiredSize.Width;
					desiredHeight += Math.Max(desiredHeight, validChildren[i].DesiredSize.Height);
				}
				return new Size(desiredWidth, desiredHeight);
			}
			return MeasureArrangeHorizontal(availableSize, true);
		}
		Size ArrangeHorizontal(Size finalSize) {
			return MeasureArrangeHorizontal(finalSize, false);
		}
		Size MeasureArrangeHorizontal(Size availableSize, bool measure) {
			List<BarItemLinkInfo> validChildren = new List<BarItemLinkInfo>();
			List<BarItemLinkInfo> invalidChildren = new List<BarItemLinkInfo>();
			GetValidChildrenHorizontal(validChildren, invalidChildren);
			double rowIndent = ItemsControl == null ? 0d : ItemsControl.RowIndent;
			double columnIndent = ItemsControl == null ? 0d : ItemsControl.ColumnIndent;
			double currentRowWidth = 0d;
			double currentY = 0d;
			double heightCoeff = 1d;
			double maxRowHeight = 0d;
			int currentRow = 0;
			int currentColumn = 0;
			foreach(var child in validChildren) {
				if(measure)
					child.Measure(SizeHelper.Infinite);
				maxRowHeight = Math.Max(maxRowHeight, child.DesiredSize.Height);
			}
			var popupMenu = HeaderControl.Link.Links.Holder as PopupMenu;
			bool stretchRows = popupMenu == null ? StretchRowsDefaultValue : popupMenu.StretchRows;
			if(!measure && stretchRows)
				heightCoeff = availableSize.Height / DesiredSize.Height;
			for(int i = 0; i < validChildren.Count; i++) {
				var child = validChildren[i];
				if(currentRowWidth + child.DesiredSize.Width > availableSize.Width) {
					currentRow++;
					currentColumn = 0;
					currentY += maxRowHeight * heightCoeff + rowIndent;
					currentRowWidth = 0;
				}
				currentRowWidth += child.DesiredSize.Width + columnIndent;
				if(!measure) {
					child.Arrange(new Rect(currentRowWidth - child.DesiredSize.Width, currentY, child.DesiredSize.Width, maxRowHeight));
					BarItemLinkMenuHeaderHorizontalHelper.SetColumn(child.LinkControl, currentColumn);
					BarItemLinkMenuHeaderHorizontalHelper.SetRow(child.LinkControl, currentRow);
				} currentColumn++;
			}
			if(!measure) {
				foreach(var child in invalidChildren) {
					child.Arrange(new Rect(0, 0, 0, 0));
				}
			}
			currentY += maxRowHeight;
			return new Size(availableSize.Width, currentY);
		}
		void GetValidChildrenHorizontal(List<BarItemLinkInfo> validChildren, List<BarItemLinkInfo> invalidChildren) {
			List<BarItemLinkInfo> childrenToMeasure = new List<BarItemLinkInfo>();
			foreach(BarItemLinkInfo child in Children) {
				if(BarItemLinkMenuHeaderHorizontalHelper.IsValidLinkControl(child.LinkControl)) {
					if(validChildren != null) validChildren.Add(child);
				} else
					if(invalidChildren != null) invalidChildren.Add(child);
			}
		}
		#endregion
		#region vertical layout
		Size MeasureVertical(Size availableSize) {
			double measureHeight = 0d;
			double measureWidth = 0d;
			foreach(UIElement child in Children) {
				child.Measure(SizeHelper.Infinite);
				measureHeight += child.DesiredSize.Height;
				measureWidth = Math.Max(measureWidth, child.DesiredSize.Width);
			}
			return new Size(measureWidth, measureHeight);
		}
		Size ArrangeVertical(Size finalSize) {
			double currentY = 0d;
			double heightCoeff = 1d;
			var popupMenu = HeaderControl.Link.Links.Holder as PopupMenuBase;
			bool stretchRows = popupMenu == null ? StretchRowsDefaultValue : popupMenu.StretchRows;
			if(stretchRows) {
				double childrenHeight = 0d;
				foreach(UIElement child in Children) {
					childrenHeight += child.DesiredSize.Height;
				}
				heightCoeff = finalSize.Height / childrenHeight;
			}
			if (double.IsNaN(heightCoeff) || double.IsInfinity(heightCoeff))
				heightCoeff = 0d;
			foreach(UIElement child in Children) {
				double arrangeHeight = child.DesiredSize.Height * heightCoeff;
				child.Arrange(new Rect(0, currentY, finalSize.Width, arrangeHeight));
				currentY += arrangeHeight;
			}
			return finalSize;
		}
		#endregion
	}
	public class BarItemLinkMenuHeaderContentControl : ContentControl {
		public CornerRadius ContentCornerRadius {
			get { return (CornerRadius)GetValue(ContentCornerRadiusProperty); }
			set { SetValue(ContentCornerRadiusProperty, value); }
		}
		public Thickness ContentBorderThickness {
			get { return (Thickness)GetValue(ContentBorderThicknessProperty); }
			set { SetValue(ContentBorderThicknessProperty, value); }
		}
		public static readonly DependencyProperty ContentBorderThicknessProperty =
			DependencyPropertyManager.Register("ContentBorderThickness", typeof(Thickness), typeof(BarItemLinkMenuHeaderContentControl), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty ContentCornerRadiusProperty =
			DependencyPropertyManager.Register("ContentCornerRadius", typeof(CornerRadius), typeof(BarItemLinkMenuHeaderContentControl), new FrameworkPropertyMetadata(new CornerRadius()));
	}
	public class BarItemLinkMenuHeaderHorizontalHelper {
		public static int GetRow(DependencyObject obj) {
			return (int)obj.GetValue(RowProperty);
		}
		public static void SetRow(DependencyObject obj, int value) {
			obj.SetValue(RowProperty, value);
		}
		public static int GetColumn(DependencyObject obj) {
			return (int)obj.GetValue(ColumnProperty);
		}
		public static void SetColumn(DependencyObject obj, int value) {
			obj.SetValue(ColumnProperty, value);
		}
		public static bool GetIsFakeLink(DependencyObject obj) {
			return (bool)obj.GetValue(IsFakeLinkProperty);
		}
		public static void SetIsFakeLink(DependencyObject obj, bool value) {
			obj.SetValue(IsFakeLinkProperty, value);
		}
		public static readonly DependencyProperty IsFakeLinkProperty =
			DependencyPropertyManager.RegisterAttached("IsFakeLink", typeof(bool), typeof(BarItemLinkMenuHeaderHorizontalHelper), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty ColumnProperty =
			DependencyPropertyManager.RegisterAttached("Column", typeof(int), typeof(BarItemLinkMenuHeaderHorizontalHelper), new FrameworkPropertyMetadata(0));
		public static readonly DependencyProperty RowProperty =
			DependencyPropertyManager.RegisterAttached("Row", typeof(int), typeof(BarItemLinkMenuHeaderHorizontalHelper), new FrameworkPropertyMetadata(0));
		public static bool IsValidLinkControl(BarItemLinkControlBase linkControl) {
			return linkControl is BarButtonItemLinkControl
				|| linkControl is BarCheckItemLinkControl
				|| linkControl is BarSubItemLinkControl
				|| linkControl is BarSplitCheckItemLinkControl
				|| linkControl is BarItemLinkSeparatorControl
				|| linkControl is BarSplitButtonItemLinkControl;
		}
	}
	public class BarItemLinkMenuHeaderControlStatesResourceHolder {
		#region CornerRadius
		CornerRadius cornerRadiusCommon = new CornerRadius(double.PositiveInfinity);
		CornerRadius cornerRadiusTopLeft = new CornerRadius(double.PositiveInfinity);
		CornerRadius cornerRadiusTopRight = new CornerRadius(double.PositiveInfinity);
		CornerRadius cornerRadiusTopStretch = new CornerRadius(double.PositiveInfinity);
		CornerRadius cornerRadiusBottomLeft = new CornerRadius(double.PositiveInfinity);
		CornerRadius cornerRadiusBottomRight = new CornerRadius(double.PositiveInfinity);
		CornerRadius cornerRadiusBottomStretch = new CornerRadius(double.PositiveInfinity);
		public CornerRadius CornerRadiusCommon { get { return cornerRadiusCommon; } set { cornerRadiusCommon = value; } }
		public CornerRadius CornerRadiusTopLeft { get { return cornerRadiusTopLeft; } set { cornerRadiusTopLeft = value; } }
		public CornerRadius CornerRadiusTopRight { get { return cornerRadiusTopRight; } set { cornerRadiusTopRight = value; } }
		public CornerRadius CornerRadiusTopStretch { get { return cornerRadiusTopStretch; } set { cornerRadiusTopStretch = value; } }
		public CornerRadius CornerRadiusBottomLeft { get { return cornerRadiusBottomLeft; } set { cornerRadiusBottomLeft = value; } }
		public CornerRadius CornerRadiusBottomRight { get { return cornerRadiusBottomRight; } set { cornerRadiusBottomRight = value; } }
		public CornerRadius CornerRadiusBottomStretch { get { return cornerRadiusBottomStretch; } set { cornerRadiusBottomStretch = value; } }		
		#endregion
		#region ContentBorderThickness        
		Thickness contentBorderThicknessTop = new Thickness(double.NaN);
		Thickness contentBorderThicknessCenter = new Thickness(double.NaN);
		Thickness contentBorderThicknessBottom = new Thickness(double.NaN);
		public Thickness ContentBorderThicknessTop { get { return contentBorderThicknessTop; } set { contentBorderThicknessTop = value; } }
		public Thickness ContentBorderThicknessCenter { get { return contentBorderThicknessCenter; } set { contentBorderThicknessCenter = value; } }
		public Thickness ContentBorderThicknessBottom { get { return contentBorderThicknessBottom; } set { contentBorderThicknessBottom = value; } }		
		#endregion
		#region ItemsBorderThickness
		Thickness itemsBorderThicknessVisible = new Thickness(double.NaN);
		Thickness itemsBorderThicknessCollapsed = new Thickness(double.NaN);
		Visibility itemsBorderVisibilityVisible = Visibility.Visible;
		Visibility itemsBorderVisibilityCollapsed = Visibility.Collapsed;
		public Thickness ItemsBorderThicknessVisible { get { return itemsBorderThicknessVisible; } set { itemsBorderThicknessVisible = value; } }
		public Thickness ItemsBorderThicknessCollapsed { get { return itemsBorderThicknessCollapsed; } set { itemsBorderThicknessCollapsed = value; } }
		public Visibility ItemsBorderVisibilityVisible { get { return itemsBorderVisibilityVisible; } set { itemsBorderVisibilityVisible = value; } }
		public Visibility ItemsBorderVisibilityCollapsed { get { return itemsBorderVisibilityCollapsed; } set { itemsBorderVisibilityCollapsed = value; } }				
		#endregion
		#region ItemsOrientation
		Thickness verticalItemsPresenterThickness = new Thickness(double.NaN);
		Thickness horizontalItemsPresenterThickness = new Thickness(double.NaN);
		public Thickness VerticalItemsPresenterThickness { get { return verticalItemsPresenterThickness; } set { verticalItemsPresenterThickness = value; } }
		public Thickness HorizontalItemsPresenterThickness { get { return horizontalItemsPresenterThickness; } set { horizontalItemsPresenterThickness = value; } }				
		#endregion
		public virtual CornerRadius GetCornerRadius(HorizontalAlignment columnPosition, VerticalAlignment rowPosition, bool hasItems, HeaderOrientation itemsOrientation, bool precedesHeader) {
			switch(rowPosition) {
				case VerticalAlignment.Stretch:
				case VerticalAlignment.Top:
					switch(columnPosition) {
						case HorizontalAlignment.Left:
							if(double.IsPositiveInfinity(CornerRadiusTopLeft.TopLeft)) break;
							return CornerRadiusTopLeft;
						case HorizontalAlignment.Right:
							if(double.IsPositiveInfinity(CornerRadiusTopRight.TopLeft)) break;
							return CornerRadiusTopRight;
						case HorizontalAlignment.Stretch:
							if(double.IsPositiveInfinity(CornerRadiusTopStretch.TopLeft)) break;
							return CornerRadiusTopStretch;
						default:
							break;
					}
					break;
				case VerticalAlignment.Bottom:
					if(hasItems) break;
					switch(columnPosition) {
						case HorizontalAlignment.Left:
							if(double.IsPositiveInfinity(CornerRadiusBottomLeft.TopLeft)) break;
							return CornerRadiusBottomLeft;
						case HorizontalAlignment.Right:
							if(double.IsPositiveInfinity(CornerRadiusBottomRight.TopLeft)) break;
							return CornerRadiusBottomRight;
						case HorizontalAlignment.Stretch:
							if(double.IsPositiveInfinity(CornerRadiusBottomStretch.TopLeft)) break;
							return CornerRadiusBottomStretch;
						default:
							break;
					}
					break;
				default:
					break;
			}
			return CornerRadiusCommon;
		}
		public virtual Thickness GetContentBorderThickness(HorizontalAlignment columnPosition, VerticalAlignment rowPosition, bool hasItems, HeaderOrientation itemsOrientation, bool precedesHeader) {
			switch(rowPosition) {
				case VerticalAlignment.Stretch:
				case VerticalAlignment.Top:
					if(double.IsNaN(ContentBorderThicknessTop.Top)) break;
					return ContentBorderThicknessTop;
				case VerticalAlignment.Bottom:
					if(hasItems || double.IsNaN(ContentBorderThicknessBottom.Top)) break;
					return ContentBorderThicknessBottom;
				default:
					break;
			}
			return ContentBorderThicknessCenter;
		}
		public virtual Thickness GetItemsBorderThickness(HorizontalAlignment columnPosition, VerticalAlignment rowPosition, bool hasItems, HeaderOrientation itemsOrientation, bool precedesHeader) {
			if(rowPosition == VerticalAlignment.Bottom || !hasItems || itemsOrientation == HeaderOrientation.Vertical || precedesHeader) return ItemsBorderThicknessCollapsed;
			return ItemsBorderThicknessVisible;
		}
		public virtual Visibility GetItemsBorderVisibility(HorizontalAlignment columnPosition, VerticalAlignment rowPosition, bool hasItems, HeaderOrientation itemsOrientation, bool precedesHeader) {
			if(rowPosition == VerticalAlignment.Bottom || !hasItems || itemsOrientation == HeaderOrientation.Vertical || precedesHeader) return ItemsBorderVisibilityCollapsed;
			return ItemsBorderVisibilityVisible;
		}
		public virtual Thickness GetItemsPresenterMargin(HorizontalAlignment columnPosition, VerticalAlignment rowPosition, bool hasItems, HeaderOrientation itemsOrientation, bool precedesHeader) {
			if(itemsOrientation == HeaderOrientation.Vertical) return VerticalItemsPresenterThickness;
			return HorizontalItemsPresenterThickness;
		}
	}
}

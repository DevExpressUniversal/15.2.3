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
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Diagnostics;
using System.Windows.Media;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Native;
using System.Collections.Specialized;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public enum LinkContainerType { None, Bar, MainMenu, StatusBar, Menu, RibbonQuickAccessToolbar, RibbonPageGroup, BarButtonGroup, RibbonPageHeader, RibbonStatusBarLeft, RibbonStatusBarRight, ApplicationMenu, DropDownGallery, RibbonQuickAccessToolbarFooter, MenuHeader, RadialMenu }
	public abstract class LinksControl : BarItemsControl, IMutableNavigationOwner {
		#region static
		public static readonly DependencyProperty DropIndicatorStyleProperty;		
		protected static readonly DependencyPropertyKey MaxGlyphSizePropertyKey;
		public static readonly DependencyProperty MaxGlyphSizeProperty;
		public static readonly DependencyProperty GlyphPaddingProperty;
		public static readonly DependencyProperty SpacingModeProperty;
		public static readonly DependencyProperty LinksHolderProperty;
		static LinksControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LinksControl), new FrameworkPropertyMetadata(typeof(LinksControl)));
			LinksHolderProperty = DependencyPropertyManager.Register("LinksHolder", typeof(ILinksHolder), typeof(LinksControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnLinksHolderPropertyChanged)));
			DropIndicatorStyleProperty = DependencyPropertyManager.Register("DropIndicatorStyle", typeof(Style), typeof(LinksControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));			
			MaxGlyphSizePropertyKey = DependencyPropertyManager.RegisterReadOnly("MaxGlyphSize", typeof(Size), typeof(LinksControl), new FrameworkPropertyMetadata(new Size(0, 0), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMaxGlyphSizePropertyChanged)));
			MaxGlyphSizeProperty = MaxGlyphSizePropertyKey.DependencyProperty;
			GlyphPaddingProperty = DependencyPropertyManager.Register("GlyphPadding", typeof(Thickness), typeof(LinksControl), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback((d, e) => { ((LinksControl)d).OnGlyphPaddingChanged(e); })));
			SpacingModeProperty = DependencyPropertyManager.Register("SpacingMode", typeof(SpacingMode), typeof(LinksControl), new FrameworkPropertyMetadata(SpacingMode.Mouse, (d, e) => ((LinksControl)d).OnSpacingModeChanged((SpacingMode)e.OldValue)));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(LinksControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(LinksControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
			FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(LinksControl), new FrameworkPropertyMetadata(true));
			EventManager.RegisterClassHandler(typeof(LinksControl), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
		}
		protected static void OnLinksHolderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { 
			((LinksControl)d).OnLinksHolderChanged(e);
		}
		protected static void OnMaxGlyphSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((LinksControl)d).OnMaxGlyphChanged(e);
		}
		#endregion
		NavigationManager navigationManager;
		readonly PostponedAction updateGlypsSizeAction;
		public LinksControl() {
			Unloaded += new RoutedEventHandler(OnUnloaded);
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
			Loaded += new RoutedEventHandler(OnLoaded);
			navigationManager = CreateNavigationManager();
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
			updateGlypsSizeAction = new PostponedAction(() => true);
		}		
		void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			CoerceLinksIsEnableProperty();
		}
		static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e) {
			((LinksControl)sender).OnAccessKeyPressed(e);
		}
		protected virtual void OnAccessKeyPressed(AccessKeyPressedEventArgs e) { }
		protected virtual void OnGlyphPaddingChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void OnLayoutUpdated(object sender, EventArgs e) {
			updateGlypsSizeAction.Perform();
		}
		protected virtual NavigationManager CreateNavigationManager() {
			return new NavigationManager(this);
		}
		public ILinksHolder LinksHolder {
			get { return (ILinksHolder)GetValue(LinksHolderProperty); }
			set { SetValue(LinksHolderProperty, value); }
		}
		public Style DropIndicatorStyle {
			get { return (Style)GetValue(DropIndicatorStyleProperty); }
			set { SetValue(DropIndicatorStyleProperty, value); }
		}
		protected internal ItemsPresenter ItemsPresenter { get; set; }
		protected virtual void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, false);
		}
		protected void CoerceLinksIsEnableProperty() {
			if(ItemLinks == null)
				return;
			foreach(BarItemLinkBase link in ItemLinks) {
				link.CoerceValue(UIElement.IsEnabledProperty);
			}		   
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, true);
			CoerceLinksIsEnableProperty();
		}
		protected virtual void OnLinksHolderChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected internal virtual void InvalidateMeasurePanel() { }
		public abstract BarItemLinkCollection ItemLinks { get; }
		public NavigationManager NavigationManager {
			get { return navigationManager; }
		}
		public SpacingMode SpacingMode {
			get { return (SpacingMode)GetValue(SpacingModeProperty); }
			set { SetValue(SpacingModeProperty, value); }
		}
		CompatibilityAdornerContainer panelAdornerContainer;
		protected internal CompatibilityAdornerContainer PanelAdornerContainer { get { return panelAdornerContainer; } }
		protected internal virtual void OnManagerChanged(DependencyPropertyChangedEventArgs e) { }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ItemsPresenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
			LoadPanelAdornerContainer();
		}
		protected virtual void LoadPanelAdornerContainer() {
			this.panelAdornerContainer = GetTemplateChild("PART_Adorner") as CompatibilityAdornerContainer;
		}		
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			var linkInfo = element as BarItemLinkInfo;
			if(linkInfo == null)
				base.ClearContainerForItemOverride(element, item);
			else {
				var scope = FocusManager.GetFocusScope(this);
				if(LayoutHelper.IsChildElement(linkInfo, FocusManager.GetFocusedElement(scope) as DependencyObject)) {
					FocusManager.SetFocusedElement(scope, null);
				}
				linkInfo.ClearLinkControl();
			}
		}
		protected BarContainerControl Container { 
			get { return container;}
			set {
				if(Container == value) return;
				var oldValue = container;
				container = value;
				OnContainerChanged(oldValue);
			}
		}
		private LinkContainerType containerTypeCore = LinkContainerType.None;
		public LinkContainerType ContainerType {
			get { return containerTypeCore; }
			protected internal set {
				if(containerTypeCore == value)
					return;
				LinkContainerType oldValue = containerTypeCore;
				containerTypeCore = value;
				OnContainerTypeChanged(oldValue);
			}
		}
		protected virtual void OnContainerChanged(BarContainerControl oldContainer) { }
		protected virtual void OnContainerTypeChanged(LinkContainerType oldValue) {
			for(int i = 0; i < GetSnapshotItemsCount(); i++) {
				((BarItemLinkInfo)GetSnapshotItem(i)).LinkContainerType = ContainerType;
			}
		}
		protected virtual void OnSpacingModeChanged(SpacingMode oldValue) {
			UpdateSpacingMode();
		}
		private void UpdateSpacingMode() {
			if (ItemLinks == null)
				return;
			for (int i = 0; i < ItemLinks.Count; i++) {
				var linkInfo = ItemContainerGenerator.ContainerFromIndex(i) as BarItemLinkInfo;
				if (linkInfo == null || linkInfo.LinkControl == null)
					continue;
				var linkControl = linkInfo.LinkControl;
				linkControl.SpacingMode = SpacingMode;
			}
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			BarItemLinkInfo linkControlContainer = element as BarItemLinkInfo;
			InitializeLinkInfo(linkControlContainer);
		}
		protected virtual void InitializeLinkInfo(BarItemLinkInfo linkInfo) {
			linkInfo.ContentTemplate = null;
			linkInfo.Content = null;
			BarItemLinkInfo itemLinkInfo = linkInfo;
			if (Container == null)
				Container = BarManagerHelper.FindContainerControl(this);
			itemLinkInfo.Container = Container;
			itemLinkInfo.LinkContainerType = ContainerType;
			itemLinkInfo.CreateLinkControl();
			if (itemLinkInfo.LinkControl != null) {
				itemLinkInfo.LinkControl.SpacingMode = SpacingMode;
			}
		}		
		protected virtual void ClearControlItemLinkCollection(BarItemLinkInfoCollection coll) {
			if(coll == null) return;
			foreach(object obj in coll) {
				BarItemLinkInfo linkInfo = obj as BarItemLinkInfo;
				if(linkInfo != null) {
					linkInfo.LinkBase.LinkInfos.Remove(linkInfo);
					linkInfo.ClearLinkControl();
				}
			}
			ItemsSource = null;
			coll.Source = null;
		}
		void ClearCustomResources(BarItemLinkInfo linkInfo) {
			if(linkInfo == null) return;
			BarItemLinkControlBase linkControl = linkInfo.LinkControl;
			if(linkControl != null)
				linkControl.Resources = null;
		}
		public Size MaxGlyphSize {
			get { return (Size)GetValue(MaxGlyphSizeProperty); }
			internal set { this.SetValue(MaxGlyphSizePropertyKey, value); }
		}
		public Thickness GlyphPadding {
			get { return (Thickness)GetValue(GlyphPaddingProperty); }
			set { SetValue(GlyphPaddingProperty, value); }
		}
		protected virtual Size DefaultMaxGlyphSize { get { return new Size(16, 16); } }
		protected internal virtual void CalculateMaxGlyphSize() {
			Size res = DefaultMaxGlyphSize;
			for(int i = 0; i < GetSnapshotItemsCount(); i++) {
				BarItemLinkInfo li = GetSnapshotItem(i) as BarItemLinkInfo;
				BarItemLinkControl lc = li.LinkControl as BarItemLinkControl;
				if(lc == null || li.LinkBase == null || !li.LinkBase.CalculateVisibility(li))
					continue;
				if(lc.IsLargeGlyph) {
					res = new Size(32, 32);
					break;
				}
			}
			MaxGlyphSize = res;
		}
		protected virtual void OnMaxGlyphChanged(DependencyPropertyChangedEventArgs e) {
			for(int i = 0; i < GetSnapshotItemsCount(); i++) {
				BarItemLinkInfo info = GetSnapshotItem(i) as BarItemLinkInfo;
				if(info.LinkControl != null)
					info.LinkControl.OnMaxGlyphSizeChanged(MaxGlyphSize);
			}
		}
		protected virtual void UpdateLinksContainerType(NotifyCollectionChangedAction action, IList oldITems, IList newItems) {
			if (action == NotifyCollectionChangedAction.Remove)
				return;
			IEnumerator en = newItems.With(x => x.GetEnumerator()) ?? ((IEnumerable)Items).GetEnumerator();
			while(en.MoveNext()) {
				BarItemLinkInfo info = en.Current as BarItemLinkInfo;
				info.LinkContainerType = ContainerType;
			}
		}
		protected virtual bool IsItemsTypeValid() {
			foreach(object o in Items) {
				if(!(o is BarItemLinkInfo)) return false;
			}
			return true;
		}
		protected internal BarItemLinkControlBase GetLinkControl(int index) {
			BarItemLinkInfo info = GetSnapshotItem(index) as BarItemLinkInfo;
			if(info == null) return null;
			return info.LinkControl;
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			IsItemsSnapshotDirty = true;
			base.OnItemsChanged(e);
			OnItemsChangedCore(e.Action, e.OldItems, e.NewItems);			
		}
		bool IsItemsSnapshotDirty = true;
		object[] ItemsSnapshot = new object[0];
		void UpdateItemsSnapshot() {
			if(!IsItemsSnapshotDirty) return;
				ItemsSnapshot = new object[Items.Count];
				Items.CopyTo(ItemsSnapshot, 0);
			IsItemsSnapshotDirty = false;
		}
		protected object GetSnapshotItem(int index) {
			if(IsItemsSnapshotDirty) 
				UpdateItemsSnapshot();
			return ItemsSnapshot[index];
		}
		protected int GetSnapshotItemsCount() {
			if(IsItemsSnapshotDirty)
				UpdateItemsSnapshot();
			return ItemsSnapshot.Length;
		}
		protected virtual void OnItemsChangedCore(NotifyCollectionChangedAction action, IList oldITems, IList newItems) {
			if(!IsItemsTypeValid()) return;
			UpdateLinksContainerType(action, oldITems, newItems);
			updateGlypsSizeAction.PerformPostpone(ForceCalcMaxGlyphSize);
		}		
		protected virtual bool IsItemsValid {
			get {
				foreach(object obj in Items) {
					if(!(obj is BarItemLinkInfo))
						return false;
				}
				return true;
			}
		}
		protected virtual void ForceCalcMaxGlyphSize() {
			foreach(BarItemLinkInfo linkInfo in Items) {
				if(linkInfo.LinkControl is BarItemLinkControl)
					((BarItemLinkControl)linkInfo.LinkControl).UpdateActualGlyph();
			}
			CalculateMaxGlyphSize();
		}
		protected internal virtual bool OpenPopupsAsMenu { get { return true; } }
		BarContainerControl container;
		BarManager manager;
		public BarManager Manager {
			get {
				if(manager == null) {
					manager = BarManagerHelper.FindBarManager(this);
					if(manager != null)
						OnManagerChanged(new DependencyPropertyChangedEventArgs(BarManager.BarManagerProperty, null, manager));
				}				
				return manager;
			}
		}
		#region INavigationOwner
		IBarsNavigationSupport IBarsNavigationSupport.Parent { get { return GetNavigationParent(); } }		
		Orientation INavigationOwner.Orientation { get { return GetNavigationOrientation(); } }		
		NavigationKeys INavigationOwner.NavigationKeys { get { return GetNavigationKeys(); } }		
		KeyboardNavigationMode INavigationOwner.NavigationMode { get { return GetNavigationMode(); } }		
		NavigationManager INavigationOwner.NavigationManager { get { return NavigationManager; } }
		IList<INavigationElement> INavigationOwner.Elements { get { return GetNavigationElements(); } }
		bool INavigationOwner.CanEnterMenuMode { get { return GetCanEnterMenuMode(); } }
		INavigationElement INavigationOwner.BoundElement { get { return GetBoundElement(); } }		
		int IBarsNavigationSupport.ID { get { return GetNavigationID(); } }
		bool IBarsNavigationSupport.IsSelectable { get { return GetIsSelectable(); } }
		bool IBarsNavigationSupport.ExitNavigationOnMouseUp { get { return GetExitNavigationOnMouseUp(); } }		
		bool IBarsNavigationSupport.ExitNavigationOnFocusChangedWithin { get { return GetExitNavigationOnFocusChangedWithin(); } }		
		void INavigationOwner.OnAddedToSelection() { OnAddedToSelectionCore(); }		
		void INavigationOwner.OnRemovedFromSelection(bool destroying) { OnRemovedFromSelectionCore(destroying); }
		event EventHandler IMutableNavigationSupport.Changed {
			add { changed += value; }
			remove { changed -= value; }
		}
		EventHandler changed;
		void IMutableNavigationSupport.RaiseChanged() {
			RaiseChanged();
		}
		protected internal void RaiseChanged() {
			if (changed == null)
				return;
			changed(this, EventArgs.Empty);
		}
		#endregion
		protected virtual void OnAddedToSelectionCore() { }
		protected virtual void OnRemovedFromSelectionCore(bool destroying) { }
		protected virtual INavigationElement GetBoundElement() { return this as INavigationElement; }
		protected virtual bool GetIsSelectable() { return false; }
		protected virtual bool GetExitNavigationOnMouseUp() { return true; }
		protected virtual bool GetExitNavigationOnFocusChangedWithin() { return true; }
		protected abstract int GetNavigationID();
		protected abstract bool GetCanEnterMenuMode();
		protected abstract IBarsNavigationSupport GetNavigationParent();
		protected abstract Orientation GetNavigationOrientation();
		protected abstract NavigationKeys GetNavigationKeys();
		protected abstract KeyboardNavigationMode GetNavigationMode();
		protected virtual IList<INavigationElement> GetNavigationElements() {
			if (ItemsSource == null) {
				return Items.OfType<INavigationElement>().Where(x=>x.IsSelectable).ToList();
			}
			List<INavigationElement> elements = new List<INavigationElement>();
			foreach(INavigationElement element in ItemsSource) {
				if (!element.IsSelectable)
					continue;
				elements.Add(element);
			}
			return elements;
		}
		protected internal virtual bool ContainsLinkControl(BarItemLinkControlBase linkControl) { 
			foreach(BarItemLinkInfo info in Items) {
				if (info.LinkControl == linkControl) return true;
			}
			return false;
		}
		protected internal virtual void OnClear() {
			LinksHolder = null;
		}
		protected internal virtual void OnItemClick(BarItemLinkControl linkControl) {
		}
		protected internal virtual void OnPreviewItemClick(BarItemLinkControl linkControl) {
		}
		protected internal virtual void OnItemMouseEnter(BarItemLinkControl linkControl) { }
		protected internal virtual void OnItemMouseLeave(BarItemLinkControl linkControl, MouseEventArgs e) { }
	}
}

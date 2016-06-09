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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Utils;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Bars.Helpers;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public class SubMenuBarControl : LinksControl {
		#region static
		public static readonly DependencyProperty PopupProperty;
		public static readonly DependencyProperty ContentSideVisibilityProperty;
		public static readonly DependencyProperty GlyphSideVisibilityProperty;
		public static readonly DependencyProperty GlyphSidePanelWidthProperty;
		public static readonly DependencyProperty MenuHeaderStatesHolderProperty;				
		static SubMenuBarControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SubMenuBarControl), new FrameworkPropertyMetadata(typeof(SubMenuBarControl)));			
			FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(SubMenuBarControl), new FrameworkPropertyMetadata(true));
			InputMethod.IsInputMethodSuspendedProperty.OverrideMetadata(typeof(SubMenuBarControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
			PopupProperty = DependencyPropertyManager.Register("Popup", typeof(PopupMenuBase), typeof(SubMenuBarControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			MenuHeaderStatesHolderProperty = DependencyPropertyManager.Register("MenuHeaderStatesHolder", typeof(BarItemLinkMenuHeaderControlStatesResourceHolder), typeof(SubMenuBarControl), new FrameworkPropertyMetadata(null));
			ContentSideVisibilityProperty = DependencyPropertyManager.Register("ContentSideVisibility", typeof(Visibility), typeof(SubMenuBarControl), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsMeasure));
			GlyphSideVisibilityProperty = DependencyPropertyManager.Register("GlyphSideVisibility", typeof(Visibility), typeof(SubMenuBarControl), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsMeasure));
			GlyphSidePanelWidthProperty = DependencyPropertyManager.Register("GlyphSidePanelWidth", typeof(double), typeof(SubMenuBarControl), new FrameworkPropertyMetadata(0.0));
		}		
		#endregion
		public override BarItemLinkCollection ItemLinks {
			get {
				if(LinksHolder == null) return null;
				return LinksHolder.ActualLinks; 
			}
		}
		public SubMenuBarControl() {
			ContainerType = LinkContainerType.Menu;
		}
		GlyphSidePanel glyphSidePanel;
		public BarItemLinkMenuHeaderControlStatesResourceHolder MenuHeaderStatesHolder {
			get { return (BarItemLinkMenuHeaderControlStatesResourceHolder)GetValue(MenuHeaderStatesHolderProperty); }
			set { SetValue(MenuHeaderStatesHolderProperty, value); }
		}
		protected internal GlyphSidePanel GlyphSidePanel {
			get { return glyphSidePanel; }
			set {
				if(GlyphSidePanel == value)
					return;
				glyphSidePanel = value;
				OnGlyphSidePanelChanged();
			}
		}
		protected virtual void OnGlyphSidePanelChanged() {
			if(GlyphSidePanel != null) {
				GlyphSidePanel.SubMenu = this;
			}
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
		}
		protected override void OnLinksHolderChanged(DependencyPropertyChangedEventArgs e) {
			base.OnLinksHolderChanged(e);
			if(ItemsSource != null) {
				ClearControlItemLinkCollection(ItemsSource as BarItemLinkInfoCollection);
			}
			ILinksHolder holder = e.NewValue as ILinksHolder;
			if(holder != null)
				UpdateItemsSource(holder.ActualLinks);
		}
		protected internal virtual void UpdateItemsSource(BarItemLinkCollection itemLinks) {
			BarItemLinkInfoCollection oldValue = ItemsSource as BarItemLinkInfoCollection;
			ItemsSource = new BarItemLinkInfoCollection(itemLinks);
			if(oldValue != null)
				oldValue.Source = null;
			CalculateMaxGlyphSize();
		}
		protected internal virtual void OnIsOpenCoerce(object baseValue) {
			if(IsItemsTypeValid()) {
				UpdateLinksContainerType(System.Collections.Specialized.NotifyCollectionChangedAction.Reset, null, null);
				if ((bool)baseValue)
					ForceCalcMaxGlyphSize();
			}
		}
		protected override void OnAccessKeyPressed(AccessKeyPressedEventArgs e) {
			base.OnAccessKeyPressed(e);
			if(e.OriginalSource == this || (e.Target is DependencyObject && LayoutHelper.FindParentObject<LinksControl>((DependencyObject)e.Target) == this)) {
				e.Scope = this;
				e.Handled = true;
			}
		}
		BarManager ManagerCore { get; set; }
		protected internal virtual void OnIsOpenChanged(DependencyPropertyChangedEventArgs e) {
			UpdateFromPopup();
			if ((bool)e.NewValue)
				CaptureFocus();
			else
				ReleaseFocus();
		}
		public double GlyphSidePanelWidth {
			get { return (double)GetValue(GlyphSidePanelWidthProperty); }
			set { SetValue(GlyphSidePanelWidthProperty, value); }
		}
		void UpdateGlyphSidePanelWidth() {
			GlyphSidePanelWidth = MaxGlyphSize.Width + GlyphPadding.Left + GlyphPadding.Right;
		}
		protected internal override void CalculateMaxGlyphSize() {
			Size res = DefaultMaxGlyphSize;
			for(int i = 0; i < GetSnapshotItemsCount(); i++) {
				BarItemLinkInfo li = GetSnapshotItem(i) as BarItemLinkInfo;
				BarItemLinkControl lc = li.LinkControl as BarItemLinkControl;
				if(lc == null || li.LinkBase == null || !li.LinkBase.CalculateVisibility(li))
					continue;
				if(lc is BarItemLinkMenuHeaderControl && (lc as BarItemLinkMenuHeaderControl).HasHorizontalItemsWithLargeGlyph()) {
					res = new Size(32, 32);
					break;
				}
				if(lc.IsLargeGlyph) {
					res = new Size(32, 32);
					break;
				}
			}
			MaxGlyphSize = res;
		}
		protected override void OnMaxGlyphChanged(DependencyPropertyChangedEventArgs e) {
			base.OnMaxGlyphChanged(e);
			UpdateGlyphSidePanelWidth();
		}
		protected override void OnGlyphPaddingChanged(DependencyPropertyChangedEventArgs e) {
			base.OnGlyphPaddingChanged(e);
		}
		public Visibility ContentSideVisibility {
			get { return (Visibility)GetValue(ContentSideVisibilityProperty); }
			set { SetValue(ContentSideVisibilityProperty, value); }
		}
		public Visibility GlyphSideVisibility {
			get { return (Visibility)GetValue(GlyphSideVisibilityProperty); }
			set { SetValue(GlyphSideVisibilityProperty, value); }
		}
		protected internal virtual void UpdateFromPopup() {			
			UpdateButtonsVisibility();
		}		
		public PopupMenuBase Popup {
			get { return (PopupMenuBase)GetValue(PopupProperty); }
			set { SetValue(PopupProperty, value); }
		}		
		protected RepeatButton UpButton { get; private set; }
		protected RepeatButton DownButton { get; private set; }
		protected RepeatButton LeftButton { get; private set; }
		protected RepeatButton RightButton { get; private set; }
		protected internal SubMenuScrollViewer ScrollViewer { get; private set; }
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			BarItemLinkInfo linkControlContainer = element as BarItemLinkInfo;
			SubscribeEvents(linkControlContainer.LinkControl as BarItemLinkControl);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			BarItemLinkInfo linkControlContainer = element as BarItemLinkInfo;
			BarItemLinkControlBase linkControl = linkControlContainer.LinkControl;
			base.ClearContainerForItemOverride(element, item);
			if(linkControl == null) return;
			UnsubscribeEvents(linkControl as BarItemLinkControl);
			linkControl.Container = null;
		}
		protected override void InitializeLinkInfo(BarItemLinkInfo linkInfo) {
			base.InitializeLinkInfo(linkInfo);
			BarItemLinkControlBase linkControl = linkInfo.LinkControl;
			linkInfo.BarPopupExpandMode = Popup.Return(popup => popup.ExpandMode, () => BarPopupExpandMode.Classic);
			if(Popup != null && Popup.OwnerLinkControl != null && linkControl != null) {
				linkControl.Container = BarManagerHelper.FindContainerControl(Popup.OwnerLinkControl);
			}
		}
		protected virtual void UnsubscribeEvents(BarItemLinkControl linkControl) {
			if(linkControl == null || Popup == null)
				return;
			linkControl.IsSelectedChanged -= new RoutedEventHandler(Popup.OnLinkControlIsSelectedChanged);
			linkControl.IsHighlightedChanged -= new RoutedEventHandler(Popup.OnLinkControlIsHighlightedChanged);
		}
		protected virtual void SubscribeEvents(BarItemLinkControl linkControl) {
			if(linkControl == null || Popup == null)
				return;
			linkControl.IsSelectedChanged += new RoutedEventHandler(Popup.OnLinkControlIsSelectedChanged);
			linkControl.IsHighlightedChanged += new RoutedEventHandler(Popup.OnLinkControlIsHighlightedChanged);
		}
		protected internal override void OnItemClick(BarItemLinkControl linkControl) {
			base.OnItemClick(linkControl);
			if(Popup != null)
				Popup.OnLinkControlClick(linkControl,null);
		}
		protected internal override void OnPreviewItemClick(BarItemLinkControl linkControl) {
			base.OnPreviewItemClick(linkControl);
			if(Popup != null)
				Popup.OnPreviewLinkControlClick(linkControl, null);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UnsubscribeEvents();
			GlyphSidePanel = (GlyphSidePanel)GetTemplateChild("PART_GlyphSide");
			ScrollViewer = GetTemplateChild("PART_ScrollViewer") as SubMenuScrollViewer;
			UpButton = (RepeatButton)GetTemplateChild("PART_UpButton");
			DownButton = (RepeatButton)GetTemplateChild("PART_DownButton");
			LeftButton = (RepeatButton)GetTemplateChild("PART_LeftButton");
			RightButton = (RepeatButton)GetTemplateChild("PART_RightButton");
			SubscribeEvents();
			UpdateButtonsVisibility();
			UpdateGlyphSidePanelWidth();
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			DragManager.SetDropTargetFactory(this, new SubMenuBarControlDropTargetFactoryExtension());
			UpdateButtonsVisibility();
			UnsubscribeEvents();
			SubscribeEvents();
			UpdateGlyphSidePanelWidth();
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			base.OnUnloaded(sender, e);
			UnsubscribeEvents();
		}
		protected virtual void OnScrollViewerLayoutUpdated(object sender, EventArgs e) {
			UpdateButtonsVisibility();
		}
		protected virtual void UpdateButtonsVisibility() {
			if(ScrollViewer == null || UpButton == null || DownButton == null || LeftButton==null || RightButton==null)
				return;
			UpButton.Visibility = ScrollViewer.ShowTopScroll ? Visibility.Visible : Visibility.Collapsed;
			DownButton.Visibility = ScrollViewer.ShowBottomScroll ? Visibility.Visible : Visibility.Collapsed;
			LeftButton.Visibility = ScrollViewer.ShowLeftScroll ? Visibility.Visible : Visibility.Collapsed;
			RightButton.Visibility = ScrollViewer.ShowRightScroll ? Visibility.Visible : Visibility.Collapsed;
		}
		protected virtual void SubscribeEvents() {
			if(ScrollViewer == null)
				return;
			ScrollViewer.LayoutUpdated += OnScrollViewerLayoutUpdated;
			UpButton.Click += OnUpButtonClick;
			DownButton.Click += OnDownButtonClick;
			LeftButton.Click += OnLeftButtonClick;
			RightButton.Click += OnRightButtonClick;
		}		
		protected virtual void UnsubscribeEvents() {
			if(ScrollViewer == null)
				return;
			ScrollViewer.LayoutUpdated -= OnScrollViewerLayoutUpdated;
			UpButton.Click -= OnUpButtonClick;
			DownButton.Click -= OnDownButtonClick;
		}
		protected virtual void OnDownButtonClick(object sender, RoutedEventArgs e) {
			ScrollViewer.LineDown();
			UpdateButtonsVisibility();
		}
		protected virtual void OnUpButtonClick(object sender, RoutedEventArgs e) {
			ScrollViewer.LineUp();
			UpdateButtonsVisibility();
		}
		void OnLeftButtonClick(object sender, RoutedEventArgs e) {
			ScrollViewer.LineLeft();
			UpdateButtonsVisibility();
		}
		void OnRightButtonClick(object sender, RoutedEventArgs e) {
			ScrollViewer.LineRight();
			UpdateButtonsVisibility();
		}
		internal void CaptureFocus() {
			if (Popup.With(x => x.OwnerLinkControl) == null)
				FocusObserver.SaveFocus(true);
			Focus();
		}
		internal void ReleaseFocus() {
			if (Popup.With(x => x.OwnerLinkControl) == null)
				FocusObserver.RestoreFocus(false, () => TreeHelper.GetParent(Keyboard.FocusedElement as DependencyObject, x => x == this, true) != null);
		}
		#region INavigationOwner
		protected override NavigationManager CreateNavigationManager() {
			return new PopupMenuNavigationManager(this);
		}
		protected override bool GetCanEnterMenuMode() { return false; }
		protected override IBarsNavigationSupport GetNavigationParent() { return Popup.With(x => x.OwnerLinkControl).With(x => x.LinkInfo) ?? Popup.With(p=>TreeHelper.GetParent<IBarsNavigationSupport>(p, x => true, false, false)); }
		protected override Orientation GetNavigationOrientation() { return Orientation.Vertical; }
		protected override NavigationKeys GetNavigationKeys() { return NavigationKeys.Arrows | NavigationKeys.Tab | NavigationKeys.HomeEnd; }
		protected override KeyboardNavigationMode GetNavigationMode() { return KeyboardNavigationMode.Cycle; }		
		protected override void OnAddedToSelectionCore() {
			base.OnAddedToSelectionCore();
			var pTarget = Popup.OwnerLinkControl as UIElement ?? Popup.Owner as UIElement ?? Popup.PlacementTarget as UIElement;
			if (!Popup.IsOpen && pTarget!=null)
				Popup.ShowPopup(pTarget);
		}				
		protected override void OnRemovedFromSelectionCore(bool destroying) {
			base.OnRemovedFromSelectionCore(destroying);
			if (!destroying)
				Popup.ClosePopup();
		}
		protected override INavigationElement GetBoundElement() {
			return Popup.With(x => x.OwnerLinkControl).With(x => x.LinkInfo);
		}
		protected override int GetNavigationID() {
			if(Popup == null) return this.GetHashCode();
			var target = (object)Popup.OwnerLinkControl ?? (object)Popup ?? (object)this;
			return target.GetHashCode();
		}
		protected override bool GetExitNavigationOnMouseUp() { return false; }
		protected override bool GetExitNavigationOnFocusChangedWithin() { return false; }
		#endregion
	}
}

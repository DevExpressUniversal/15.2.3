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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FlyoutControl = DevExpress.Xpf.Editors.Flyout.FlyoutControl;
namespace DevExpress.Xpf.Navigation {
	public enum FlyoutShowDirection { Default, Inverted }
	public enum FlyoutShowMode { Adorner, Popup }
	[StyleTypedProperty(Property = "GroupHeaderStyle", StyleTargetType = typeof(TileBarGroupHeader))]
	[DevExpress.Xpf.Core.DXToolboxBrowsable]
	public class TileBar : veSelector , IFlyoutProvider {
		public const double DefaultGroupHeaderSpace = 5;
		#region static
		public static readonly DependencyProperty AllowItemSelectionProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ShowGroupHeadersProperty;
		[Obsolete("The TileBar.GroupHeader property is now obsolete. Use the TileBar.GroupStyle property and the CollectionView grouping functionality instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty GroupHeaderProperty;
		public static readonly DependencyProperty GroupHeaderSpacingProperty;
		public static readonly DependencyProperty GroupHeaderStyleProperty;
		public static readonly DependencyProperty GroupHeaderTemplateProperty;
		public static readonly DependencyProperty ItemColorModeProperty;
		public static readonly DependencyProperty FlyoutShowDirectionProperty;
		public static readonly DependencyProperty ItemSpacingProperty;
		public static readonly DependencyProperty FlyoutShowModeProperty;
		public static readonly DependencyProperty ShowItemShadowProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemsArrowDirectionProperty;
#pragma warning disable 612, 618
		static TileBar() {
			Type ownerType = typeof(TileBar);
			AllowItemSelectionProperty = DependencyProperty.Register("AllowItemSelection", typeof(bool), ownerType, new PropertyMetadata(true, OnAllowItemSelectionChanged));
			OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), ownerType, new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));
			ShowGroupHeadersProperty = DependencyProperty.Register("ShowGroupHeaders", typeof(bool), ownerType, new PropertyMetadata(true));
			GroupHeaderProperty = DependencyProperty.RegisterAttached("GroupHeader", typeof(object), ownerType, new PropertyMetadata(OnGroupHeaderChanged));
			GroupHeaderSpacingProperty = DependencyProperty.Register("GroupHeaderSpacing", typeof(double), ownerType, new PropertyMetadata(DefaultGroupHeaderSpace));
			GroupHeaderStyleProperty = DependencyProperty.Register("GroupHeaderStyle", typeof(Style), ownerType);
			GroupHeaderTemplateProperty = DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), ownerType);
			ItemColorModeProperty = DependencyProperty.Register("ItemColorMode", typeof(TileColorMode), ownerType, new PropertyMetadata(OnItemColorModeChanged));
			FlyoutShowDirectionProperty = DependencyProperty.Register("FlyoutShowDirection", typeof(FlyoutShowDirection), ownerType, new PropertyMetadata(FlyoutShowDirection.Default, OnFlyoutShowDirectionChanged));
			ItemSpacingProperty = DependencyProperty.Register("ItemSpacing", typeof(double), ownerType, new PropertyMetadata(10d));
			FlyoutShowModeProperty = DependencyProperty.Register("FlyoutShowMode", typeof(FlyoutShowMode), ownerType, new PropertyMetadata(FlyoutShowMode.Adorner, OnFlyoutShowModeChanged));
			ShowItemShadowProperty = DependencyProperty.Register("ShowItemShadow", typeof(bool), ownerType, new PropertyMetadata(false));
			ItemsArrowDirectionProperty = DependencyProperty.Register("ItemsArrowDirection", typeof(ButtonDirection), ownerType, new PropertyMetadata(ButtonDirection.Down));
		}
		static void OnGroupHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileBarItemsPanel.SetGroupHeader(d, e.NewValue);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static object GetGroupHeader(DependencyObject element) {
			return element.GetValue(GroupHeaderProperty);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetGroupHeader(DependencyObject element, object value) {
			element.SetValue(GroupHeaderProperty, value);
		}
#pragma warning restore 612, 618
		private static void OnItemColorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TileBar)d).OnItemColorModeChanged((TileColorMode)e.OldValue, (TileColorMode)e.NewValue);
		}
		static void OnFlyoutShowModeChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((TileBar)dObj).OnFlyoutShowModeChanged(e.OldValue, e.NewValue);
		}
		static void OnOrientationChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((TileBar)dObj).OnOrientationChanged(e.OldValue, e.NewValue);
		}
		static void OnFlyoutShowDirectionChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((TileBar)dObj).OnFlyoutShowDirectionChanged(e.OldValue, e.NewValue);
		}
		static void OnAllowItemSelectionChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((TileBar)dObj).OnAllowItemSelectionChanged((bool)e.NewValue);
		}
		#endregion
		public TileBar() {
			DefaultStyleKey = typeof(TileBar);
			FlyoutManager.SetFlyoutManager(this, FlyoutManager);
		}
		FlyoutControl PartFlyoutControl;
		FlyoutDecorator PartFlyoutDecorator;
		protected override void GetTemplateChildren() {
			base.GetTemplateChildren();
			EnsureFlyoutControl();
		}
		void EnsureFlyoutControl(bool onApplyTemplate = true) {
			if(PartFlyoutControl != null) {
				PartFlyoutControl.Content = null;
				PartFlyoutControl = null;
			}
			PartFlyoutDecorator = GetTemplateChild("PART_TileNavFlyoutDecorator") as FlyoutDecorator;
			if(PartFlyoutDecorator != null) {
				PartFlyoutDecorator.FlyoutShowMode = FlyoutShowMode;
				if(onApplyTemplate)
					PartFlyoutDecorator.SizeChanged += tileNavFlyoutDecorator_SizeChanged;
				else
					PartFlyoutControl = PartFlyoutDecorator.ActualFlyoutControl;
			} else {
				PartFlyoutControl = GetTemplateChild("PART_FlyoutControl") as FlyoutControl;
			}
		}
		void tileNavFlyoutDecorator_SizeChanged(object sender, SizeChangedEventArgs e) {
			PartFlyoutDecorator.SizeChanged -= tileNavFlyoutDecorator_SizeChanged;
			PartFlyoutControl = PartFlyoutDecorator.ActualFlyoutControl;
		}
		protected override void EnsureItemsPanelCore(Panel itemsPanel) {
			base.EnsureItemsPanelCore(itemsPanel);
			if(itemsPanel is TileBarItemsPanel) {
				itemsPanel.SetBinding(TileBarItemsPanel.ShowGroupHeadersProperty, new Binding("ShowGroupHeaders") { Source = this });
				itemsPanel.SetBinding(TileBarItemsPanel.GroupHeaderSpaceProperty, new Binding("GroupHeaderSpacing") { Source = this });
				itemsPanel.SetBinding(TileBarItemsPanel.GroupHeaderStyleProperty, new Binding("GroupHeaderStyle") { Source = this });
				itemsPanel.SetBinding(TileBarItemsPanel.GroupHeaderTemplateProperty, new Binding("GroupHeaderTemplate") { Source = this });
				itemsPanel.SetBinding(TileBarItemsPanel.ItemSpacingProperty, new Binding("ItemSpacing") { Source = this });
			}
			if(ShowGroupHeaders && !itemsPanel.IsItemsHost)
				ItemContainerSyncHelper.Attach(itemsPanel);
		}
		protected override void ReleaseItemsPanelCore(Panel itemsPanel) {
			if(ShowGroupHeaders && !itemsPanel.IsItemsHost)
				ItemContainerSyncHelper.Detach(itemsPanel);
			if(itemsPanel is TileBarItemsPanel) {
				(itemsPanel as DevExpress.Mvvm.ILockable).Do(x => x.BeginUpdate());
				itemsPanel.ClearValue(TileBarItemsPanel.ShowGroupHeadersProperty);
				itemsPanel.ClearValue(TileBarItemsPanel.GroupHeaderSpaceProperty);
				itemsPanel.ClearValue(TileBarItemsPanel.GroupHeaderStyleProperty);
				itemsPanel.ClearValue(TileBarItemsPanel.GroupHeaderTemplateProperty);
				itemsPanel.ClearValue(TileBarItemsPanel.ItemSpacingProperty);
				(itemsPanel as DevExpress.Mvvm.ILockable).Do(x => x.EndUpdate());
			}
			base.ReleaseItemsPanelCore(itemsPanel);
		}
		protected override void OnItemsPanelChanged(ItemsPanelTemplate oldItemsPanel, ItemsPanelTemplate newItemsPanel) {
			base.OnItemsPanelChanged(oldItemsPanel, newItemsPanel);
			if(ItemsPanel != null) {
			}
		}
		protected override ISelectorItem CreateSelectorItem() {
			return new TileBarItem();
		}
		void CoerceItemProperties(TileSelectorItem container) {
			container.CoerceValue(TileSelectorItem.ActualColorModeProperty);
			container.CoerceValue(TileSelectorItem.ActualSelectionPaddingProperty);
			container.CoerceValue(TileSelectorItem.IsSelectedProperty);
		}
		protected override void PrepareSelectorItem(ISelectorItem selectorItem, object item) {
			base.PrepareSelectorItem(selectorItem, item);
			var container = selectorItem as TileSelectorItem;
			if(container != null) {
				container.SetBinding(TileSelectorItem.ShowShadowProperty, new Binding("ShowItemShadow") { Source = this });
				container.SetBinding(TileSelectorItem.ArrowDirectionProperty, new Binding("ItemsArrowDirection") { Source = this });
				CoerceItemProperties(container);
			}
		}
		protected override void ClearSelectorItem(ISelectorItem selectorItem, object item) {
			var container = selectorItem as TileSelectorItem;
			if(container != null) {
				BindingOperations.ClearBinding(container, TileSelectorItem.ShowShadowProperty);
				BindingOperations.ClearBinding(container, TileSelectorItem.ArrowDirectionProperty);
				CoerceItemProperties(container);
			}
			base.ClearSelectorItem(selectorItem, item);
		}
		protected virtual void OnItemColorModeChanged(TileColorMode oldValue, TileColorMode newValue) {
			foreach(var item in Items) {
				var container = ItemContainerGenerator.ContainerFromItem(item) as TileSelectorItem;
				if(container != null) container.CoerceValue(TileSelectorItem.ActualColorModeProperty);
			}
		}
		public void CloseFlyout() {
			FlyoutManager.CloseAll();
		}
		protected override void OnSelectedItemChanged(object oldValue, object newValue) {
			base.OnSelectedItemChanged(oldValue, newValue);
			IScrollablePanel scrollablePanel = PartItemsPanel as IScrollablePanel;
			var container = ItemContainerGenerator.ContainerFromItem(newValue) as FrameworkElement;
			if(scrollablePanel != null && container != null) scrollablePanel.BringChildIntoView(container);
		}
		protected virtual void OnFlyoutShowModeChanged(object oldValue, object newValue) {
			if(PartFlyoutControl == null) {
				return;
			}
			FlyoutManager.CloseAll();
			EnsureFlyoutControl(false);
		}
		protected virtual void OnAllowItemSelectionChanged(bool newValue) {
			foreach(var item in Items) {
				var container = ItemContainerGenerator.ContainerFromItem(item) as TileSelectorItem;
				if(container != null) {
					container.CoerceValue(TileSelectorItem.ActualSelectionPaddingProperty);
					container.CoerceValue(TileSelectorItem.IsSelectedProperty);
				}
			}
		}
		protected virtual void OnOrientationChanged(object oldValue, object newValue) {
			ItemsArrowDirectionUpdate();
		}
		protected virtual void OnFlyoutShowDirectionChanged(object oldValue, object newValue) {
			ItemsArrowDirectionUpdate();
		}
		void ItemsArrowDirectionUpdate() {
			if(Orientation == Orientation.Horizontal) {
				ItemsArrowDirection = FlyoutShowDirection == FlyoutShowDirection.Default ? ButtonDirection.Down : ButtonDirection.Up;
			} else {
				ItemsArrowDirection = FlyoutShowDirection == FlyoutShowDirection.Default ? ButtonDirection.Right : ButtonDirection.Left;
			}
		}
		ItemContainerSyncHelper _ItemContainerSyncHelper;
		ItemContainerSyncHelper ItemContainerSyncHelper {
			get {
				if(_ItemContainerSyncHelper == null) _ItemContainerSyncHelper = new ItemContainerSyncHelper(ItemContainerGenerator);
				return _ItemContainerSyncHelper;
			}
		}
		protected override bool RequiresSelectedItem {
			get { return false; }
		}
		private FlyoutManager _FlyoutManager;
		internal FlyoutManager FlyoutManager {
			get {
				if(_FlyoutManager == null) {
					_FlyoutManager = new FlyoutManager(this);
				}
				return _FlyoutManager;
			}
		}
		public double ItemSpacing {
			get { return (double)GetValue(ItemSpacingProperty); }
			set { SetValue(ItemSpacingProperty, value); }
		}
		public FlyoutShowDirection FlyoutShowDirection {
			get { return (FlyoutShowDirection)GetValue(FlyoutShowDirectionProperty); }
			set { SetValue(FlyoutShowDirectionProperty, value); }
		}
		public TileColorMode ItemColorMode {
			get { return (TileColorMode)GetValue(ItemColorModeProperty); }
			set { SetValue(ItemColorModeProperty, value); }
		}
		public bool AllowItemSelection {
			get { return (bool)GetValue(AllowItemSelectionProperty); }
			set { SetValue(AllowItemSelectionProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public bool ShowGroupHeaders {
			get { return (bool)GetValue(ShowGroupHeadersProperty); }
			set { SetValue(ShowGroupHeadersProperty, value); }
		}
		[Obsolete("The TileBar.GroupHeaderSpacing property is now obsolete. Use the TileBar.GroupStyle property and the CollectionView grouping functionality instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public double GroupHeaderSpacing {
			get { return (double)GetValue(GroupHeaderSpacingProperty); }
			set { SetValue(GroupHeaderSpacingProperty, value); }
		}
		[Obsolete("The TileBar.GroupHeaderStyle property is now obsolete. Use the TileBar.GroupStyle property and the CollectionView grouping functionality instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Style GroupHeaderStyle {
			get { return (Style)GetValue(GroupHeaderStyleProperty); }
			set { SetValue(GroupHeaderStyleProperty, value); }
		}
		[Obsolete("The TileBar.GroupHeaderTemplate property is now obsolete. Use the TileBar.GroupStyle property and the CollectionView grouping functionality instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DataTemplate GroupHeaderTemplate {
			get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
			set { SetValue(GroupHeaderTemplateProperty, value); }
		}
		public FlyoutShowMode FlyoutShowMode {
			get { return (FlyoutShowMode)GetValue(FlyoutShowModeProperty); }
			set { SetValue(FlyoutShowModeProperty, value); }
		}
		public bool ShowItemShadow {
			get { return (bool)GetValue(ShowItemShadowProperty); }
			set { SetValue(ShowItemShadowProperty, value); }
		}
		ButtonDirection ItemsArrowDirection {
			get { return (ButtonDirection)GetValue(ItemsArrowDirectionProperty); }
			set { SetValue(ItemsArrowDirectionProperty, value); }
		}
		#region IFlyoutProvider Members
		FlyoutControl IFlyoutProvider.FlyoutControl {
			get { return PartFlyoutControl; }
		}
		Editors.Flyout.FlyoutPlacement IFlyoutProvider.Placement {
			get {
				bool isHorz = Orientation == System.Windows.Controls.Orientation.Horizontal;
				bool isNorm = FlyoutShowDirection == FlyoutShowDirection.Default;
				return isHorz ?
					(isNorm ? Editors.Flyout.FlyoutPlacement.Bottom : Editors.Flyout.FlyoutPlacement.Top) :
					(isNorm ? Editors.Flyout.FlyoutPlacement.Right : Editors.Flyout.FlyoutPlacement.Left);
			}
		}
		IFlyoutEventListener IFlyoutProvider.FlyoutEventListener {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		#endregion
	}
}
